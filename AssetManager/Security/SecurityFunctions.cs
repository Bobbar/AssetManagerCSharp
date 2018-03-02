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
using System.Text;
using System.Windows.Forms;
using System.Security.Principal;

namespace AssetManager.Security
{
    public static class SecurityTools
    {
        private static NetworkCredential adminCreds = null;

        public static NetworkCredential AdminCreds { get { return adminCreds; } }

        private static Dictionary<string, Data.Classes.AccessGroup> AccessGroups = new Dictionary<string, Data.Classes.AccessGroup>();
        private static LocalUser LocalUserAccess;

        private const string CryptKey = "r7L$aNjE6eiVj&zhap_@|Gz_";
        public static bool VerifyAdminCreds(string credentialDescription = "")
        {
            bool ValidCreds = false;
            if (AdminCreds == null)
            {
                using (GetCredentialsForm NewGetCreds = new GetCredentialsForm(credentialDescription))
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
            ValidCreds = CredentialIsValid(AdminCreds);
            if (!ValidCreds)
            {
                ClearAdminCreds();
                if (OtherFunctions.Message("Could not authenticate with provided credentials.  Do you wish to re-enter?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, "Auth Error") == DialogResult.OK)
                {
                    return VerifyAdminCreds(credentialDescription);
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return ValidCreds;
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
        
        public static string DecodePassword(string cypherString)
        {
            using (Simple3Des wrapper = new Simple3Des(CryptKey))
            {
                return wrapper.DecryptData(cypherString);
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
            return AccessGroups[accessGroupName].Level;
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
                        LocalUserAccess.UserName = r[UsersCols.UserName].ToString();
                        LocalUserAccess.Fullname = r[UsersCols.FullName].ToString();
                        LocalUserAccess.AccessLevel = (int)r[UsersCols.AccessLevel];
                        LocalUserAccess.Guid = r[UsersCols.UID].ToString();
                    }
                    else
                    {
                        LocalUserAccess.AccessLevel = 0;
                    }
                }
            }
            catch (Exception ex)
            {
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
                        AccessGroups.Add(row[SecurityCols.SecModule].ToString(), new Data.Classes.AccessGroup(row));
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static bool CanAccess(string recModule, int AccessLevel = -1)
        {
            //bitwise access levels
            int mask = 1;
            int calc_level;
            int UsrLevel;
            if (AccessLevel == -1)
            {
                UsrLevel = LocalUserAccess.AccessLevel;
            }
            else
            {
                UsrLevel = AccessLevel;
            }
            foreach (Data.Classes.AccessGroup group in AccessGroups.Values)
            {
                calc_level = UsrLevel & mask;
                if (calc_level != 0)
                {
                    if (group.AccessModule == recModule)
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
        /// Checks if current user has access to the specified module. Throws a <see cref="InvalidAccessException"/> if they do not. 
        /// </summary>
        /// <param name="recModule"></param>
        public static void CheckForAccess(string recModule)
        {
            if (!CanAccess(recModule))
            {
                if (GlobalSwitches.CachedMode)
                {
                    string errMessage = "You cannot access this function. Some features are disabled while running in cached mode.";
                    throw new InvalidAccessException(errMessage);
                }
                else
                {
                    string errMessage = "You do not have the required access rights for this function. Must have access to '" + recModule + "'.";
                    throw new InvalidAccessException(errMessage);
                }
            }
        }

        // METODO: Un-nest
        public sealed class AccessGroup
        {
            public const string AddDevice = "add";
            public const string CanRun = "can_run";
            public const string DeleteDevice = "delete";
            public const string ManageAttachment = "manage_attach";
            public const string ModifyDevice = "modify";
            public const string Tracking = "track";
            public const string ViewAttachment = "view_attach";
            public const string ViewSibi = "sibi_view";
            public const string AddSibi = "sibi_add";
            public const string ModifySibi = "sibi_modify";
            public const string DeleteSibi = "sibi_delete";
            public const string IsAdmin = "admin";
            public const string AdvancedSearch = "advanced_search";
            public const string CanStartTransaction = "start_transaction";
        }

    }
}