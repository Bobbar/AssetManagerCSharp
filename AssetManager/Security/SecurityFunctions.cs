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
                using (GetCredentialsForm NewGetCreds = new GetCredentialsForm(credentialDescription, lastUsername))
                {
                    NewGetCreds.ShowDialog();
                    if (NewGetCreds.DialogResult == DialogResult.OK)
                    {
                        adminCreds = NewGetCreds.Credentials;
                    }
                    else
                    {
                        ClearAdminCreds();
                        return false;
                    }
                }
            }
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
                using (var SHA = new SHA1CryptoServiceProvider())
                {
                    serializer.WriteObject(memoryStream, table);
                    byte[] serializedData = memoryStream.ToArray();
                    byte[] hash = SHA.ComputeHash(serializedData);
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
                StringBuilder sBuilder = new StringBuilder();
                int i;
                for (i = 0; i <= hash.Length - 1; i++)
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

        public static void GetUserAccess()
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static void PopulateAccessGroups()
        {
            try
            {
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSecurityTable))
                {
                    foreach (DataRow row in results.Rows)
                    {
                        accessGroups.Add(row[SecurityCols.SecModule].ToString(), new AccessGroup(row));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static bool CanAccess(string module, int accessLevel = -1)
        {
            //bitwise access levels
            int mask = 1;
            int calc_level;
            int usrLevel;
            if (accessLevel == -1)
            {
                usrLevel = localUser.AccessLevel;
            }
            else
            {
                usrLevel = accessLevel;
            }
            foreach (AccessGroup group in accessGroups.Values)
            {
                calc_level = usrLevel & mask;
                if (calc_level != 0)
                {
                    if (group.AccessModule == module)
                    {
                        if (GlobalSwitches.CachedMode)
                        {
                            if (group.AvailableOffline)
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        else
                        {
                            return true;
                        }
                    }
                }
                mask = mask << 1;
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