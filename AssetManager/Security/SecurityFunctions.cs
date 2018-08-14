using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Helpers;
using AssetManager.UserInterface.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using System.Windows.Forms;

namespace AssetManager.Security
{
    public static class SecurityTools
    {
        private static NetworkCredential adminCreds = null;

        public static NetworkCredential AdminCreds { get { return adminCreds; } }

        private static Dictionary<string, AccessGroup> accessGroups = new Dictionary<string, AccessGroup>();
        private static LocalUser localUser;

        private const string cryptKey = "r7L$aNjE6eiVj&zhap_@|Gz_";

        public static bool VerifyAdminCreds(string credentialDescription = "", string lastUsername = "")
        {
            bool validCreds = false;
            if (AdminCreds == null)
            {
                //using (GetCredentialsForm NewGetCreds = new GetCredentialsForm(credentialDescription, lastUsername))
                //{
                //    NewGetCreds.ShowDialog();
                //    if (NewGetCreds.DialogResult == DialogResult.OK)
                //    {
                //        adminCreds = NewGetCreds.Credentials;
                //    }
                //    else
                //    {
                //        ClearAdminCreds();
                //        return false;
                //    }
                //}

                adminCreds = new NetworkCredential("la_rl12184", "C4nt533M330", NetworkInfo.CurrentDomain);

                validCreds = CredentialIsValid(AdminCreds);
                if (!validCreds)
                {
                    string currentUsername = AdminCreds.UserName;
                    ClearAdminCreds();
                    if (OtherFunctions.Message("Could not authenticate with provided credentials.  Do you wish to re-enter?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, "Auth Error") == DialogResult.OK)
                    {
                        return VerifyAdminCreds(credentialDescription, currentUsername);
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return validCreds;
                }
            }
            return true;
        }

        private static bool CredentialIsValid(NetworkCredential creds)
        {
            bool valid = false;
            try
            {
                using (PrincipalContext context = new PrincipalContext(ContextType.Domain, NetworkInfo.CurrentDomain))
                {
                    valid = context.ValidateCredentials(creds.UserName, creds.Password);
                }
                return valid;
            }
            catch (PrincipalServerDownException)
            {
                // Return true when we cannot contact the server so the user doesn't get prompted repeatedly.
                return true;
            }
        }

        public static bool IsAdministrator()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }

        public static void ClearAdminCreds()
        {
            adminCreds = null;
        }

        public static string DecodePassword(string cypherValue)
        {
            using (Simple3Des wrapper = new Simple3Des(cryptKey))
            {
                return wrapper.DecryptData(cypherValue);
            }
        }

        public static string GetSHAOfTable(DataTable table)
        {
            var serializer = new DataContractSerializer(typeof(DataTable));
            using (var memoryStream = new MemoryStream())
            {
                using (var sha = new SHA1CryptoServiceProvider())
                {
                    serializer.WriteObject(memoryStream, table);
                    byte[] serializedData = memoryStream.ToArray();
                    byte[] hash = sha.ComputeHash(serializedData);
                    return Convert.ToBase64String(hash);
                }
            }
        }

        public static string GetMD5OfFile(string filePath)
        {
            using (FileStream fStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read, 16 * 1024 * 1024))
            {
                using (MD5 hash = MD5.Create())
                {
                    return GetMD5OfStream(fStream);
                }
            }
        }

        public static string GetMD5OfStream(Stream stream)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                stream.Position = 0;
                byte[] hash = md5Hash.ComputeHash(stream);
                var sBuilder = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sBuilder.Append(hash[i].ToString("x2"));
                }
                stream.Position = 0;
                return sBuilder.ToString();
            }
        }

        public static int GetSecGroupValue(string accessGroupName)
        {
            return accessGroups[accessGroupName].Level;
        }

        public static void PopulateUserAccess()
        {
            try
            {
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectUserByName(NetworkInfo.LocalDomainUser)))
                {
                    if (results.Rows.Count > 0)
                    {
                        DataRow r = results.Rows[0];
                        localUser = new LocalUser(
                            r[UsersCols.UserName].ToString(),
                            r[UsersCols.FullName].ToString(),
                            (int)r[UsersCols.AccessLevel],
                            r[UsersCols.Guid].ToString());
                    }
                    else
                    {
                        localUser = new LocalUser();
                    }
                }
            }
            catch (Exception ex)
            {
                localUser = new LocalUser();
                throw ex;
            }
        }

        public static void PopulateAccessGroups()
        {
            using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSecurityTable))
            {
                foreach (DataRow row in results.Rows)
                {
                    accessGroups.Add(row[SecurityCols.SecModule].ToString(), new AccessGroup(row));
                }
            }
        }

        /// <summary>
        /// Returns true if the specified user level has access the the specified security group.
        /// </summary>
        /// <param name="groupName">Name of the security group.</param>
        public static bool CanAccess(string groupName)
        {
            return CanAccess(groupName, localUser.AccessLevel);
        }

        /// <summary>
        /// Returns true if the specified user level has access the the specified security group.
        /// </summary>
        /// <param name="groupName">Name of the security group.</param>
        /// <param name="userLevel">Access level to check against.</param>
        /// <remarks>
        /// Bitwise mask operation.
        /// The user access level will be the sum of one or more groups, whos values increment
        /// by multiple of 2. (ie. 1,2,4,8,16,32,64,...) So a user access level of 5 contains
        /// only groups 1 and 3.
        ///
        /// A mask integer with the initial value of 1 is shifted left by 1 bit on each iteration.
        /// A left bit shift increments the integer value by multiples of 2, same as the above example.
        /// Then a bit AND operation is performed against the users access level and the mask. The AND
        /// operation will result in the value of the mask only if the user access level contains
        /// the mask bit.  So if groupMask = 4 and userLevel = 5, the value of (userLevel AND groupMask)
        /// will equal 4. If levelMask = 2 the value of (userLevel AND groupMask) will equal 0.
        /// </remarks>
        public static bool CanAccess(string groupName, int userLevel)
        {
            int groupMask = 1;

            foreach (AccessGroup group in accessGroups.Values)
            {
                // If the AND operation != 0 then the userLevel contains the current groupMask bit.
                bool hasGroup = (userLevel & groupMask) != 0;

                if (hasGroup)
                {
                    // We know that the current access level contains this group bit.
                    // Now we need to see if the group name matches the specified value.
                    if (group.Name == groupName)
                    {
                        // If we are in offline cached mode, see if the current
                        // is allowed to be accessed offline. Otherwise, return true.
                        if (GlobalSwitches.CachedMode)
                        {
                            if (group.AvailableOffline)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }

                // Bitwise Left shift.
                groupMask <<= 1;
            }
            return false;
        }

        /// <summary>
        /// Checks if current user has access to the specified group. Throws a <see cref="InvalidAccessException"/> if they do not.
        /// </summary>
        /// <param name="securityGroup"></param>
        public static void CheckForAccess(string securityGroup)
        {
            if (!CanAccess(securityGroup))
            {
                if (GlobalSwitches.CachedMode)
                {
                    string errMessage = "You cannot access this function. Some features are disabled while running in cached mode.";
                    throw new InvalidAccessException(errMessage);
                }
                else
                {
                    string errMessage = "You do not have the required access rights for this function. Must have access to '" + securityGroup + "'.";
                    throw new InvalidAccessException(errMessage);
                }
            }
        }
    }
}