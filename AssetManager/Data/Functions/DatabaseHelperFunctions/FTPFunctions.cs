using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using AssetManager.Data.Communications;
using AssetManager.Helpers;

namespace AssetManager.Data.Functions
{
    public static class FtpFunctions
    {
        #region Fields

        private static FtpComms ftpComms = new FtpComms();

        #endregion Fields

        #region Methods

        public static bool DeleteFtpAttachment(string fileGuid, string fKey)
        {
            try
            {
                using (var resp = (FtpWebResponse)(ftpComms.ReturnFtpResponse("ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/" + fKey + "/" + fileGuid, WebRequestMethods.Ftp.DeleteFile)))
                {
                    if (resp.StatusCode == FtpStatusCode.FileActionOK)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                return ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static bool DeleteFtpFolder(string folderGuid)
        {
            try
            {
                var files = ListDirectory("ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/" + folderGuid + "/");
                int i = 0;
                foreach (string file in files) //delete each file counting for successes
                {
                    if (DeleteFtpAttachment(file, folderGuid))
                    {
                        i++;
                    }
                }
                if (files.Count == i) // if successful deletions = total # of files, delete the directory
                {
                    using (var deleteResp = (FtpWebResponse)(ftpComms.ReturnFtpResponse("ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/" + folderGuid, WebRequestMethods.Ftp.RemoveDirectory)))
                    {
                        if (deleteResp.StatusCode == FtpStatusCode.FileActionOK)
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        public static bool HasFtpFolder(string itemGuid)
        {
            try
            {
                using (var resp = (FtpWebResponse)(ftpComms.ReturnFtpResponse("ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/" + itemGuid + "/", WebRequestMethods.Ftp.ListDirectory)))
                {
                    if (resp.StatusCode == FtpStatusCode.OpeningData)
                    {
                        return true;
                    }
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void ScanAttachements()
        {
            try
            {
                Logging.Logger("***********************************");
                Logging.Logger("******Attachment Scan Results******");

                var FTPDirs = ListDirectory("ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/");
                var FTPFiles = ListFTPFiles(FTPDirs);
                var SQLFiles = ListSQLFiles();

                var MissingFTPDirs = ListMissingFTPDirs(FTPDirs);
                var MissingSQLDirs = ListMissingSQLDirs(SQLFiles, FTPDirs);

                var MissingFTPFiles = ListMissingFTPFiles(SQLFiles, FTPFiles);
                var MissingSQLFiles = ListMissingSQLFiles(SQLFiles, FTPFiles);

                if (MissingFTPDirs.Count > 0 || MissingSQLDirs.Count > 0 || MissingFTPFiles.Count > 0 || MissingSQLFiles.Count > 0)
                {
                    string StatsText = "";
                    Logging.Logger("Orphan Files/Directories found!");
                    StatsText = @"Orphan Files/Directories found!  Do you want to delete the corrupt SQL/FTP entries?

FTP:
Missing Dirs: " + MissingFTPDirs.Count + @"
Missing Files: " + MissingFTPFiles.Count + @"

SQL:
Missing Dirs: " + MissingSQLDirs.Count + @"
Missing Files: " + MissingSQLFiles.Count;

                    var blah = OtherFunctions.Message(StatsText, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, "Orphans Found");
                    if (blah == DialogResult.Yes)
                    {
                        //clean it up
                        Logging.Logger("Cleaning attachments...");
                        int itemsCleaned = 0;
                        itemsCleaned += CleanFTPFiles(MissingFTPFiles);
                        itemsCleaned += CleanFTPDirs(MissingFTPDirs);
                        itemsCleaned += CleanSQLFiles(MissingSQLFiles);
                        itemsCleaned += CleanSQLEntries(MissingSQLDirs);
                        OtherFunctions.Message("Cleaned " + itemsCleaned.ToString() + " orphans.", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ScanAttachements();
                    }
                }
                else
                {
                    Logging.Logger("No Orphans Found.");
                    OtherFunctions.Message("No issues found.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Scan OK");
                }
                Logging.Logger("**********End Scan Results*********");
                Logging.Logger("***********************************");
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        /// <summary>
        /// Checks if supplied Guid exists in Devices or Sibi tables.
        /// </summary>
        /// <returns></returns>
        private static bool CheckForPrimaryItem(string itemGuid)
        {
            bool exists = false;
            if (!string.IsNullOrEmpty(AssetManagerFunctions.GetSqlValue(DevicesCols.TableName, DevicesCols.DeviceGuid, itemGuid, DevicesCols.DeviceGuid)))
            {
                exists = true;
            }
            if (!string.IsNullOrEmpty(AssetManagerFunctions.GetSqlValue(SibiRequestCols.TableName, SibiRequestCols.Guid, itemGuid, SibiRequestCols.Guid)))
            {
                exists = true;
            }
            return exists;
        }

        private static int CleanSQLFiles(List<AttachScanInfo> missingSQLFiles)
        {
            if (missingSQLFiles.Count > 0)
            {
                DeviceAttachmentsCols DeviceTable = new DeviceAttachmentsCols();
                SibiAttachmentsCols SibiTable = new SibiAttachmentsCols();
                int deletions = 0;
                foreach (var sqlItem in missingSQLFiles)
                {
                    var DeviceRows = DBFactory.GetDatabase().ExecuteNonQuery("DELETE FROM " + DeviceTable.TableName + " WHERE " + DeviceTable.FileGuid + "='" + sqlItem.FileGuid + "'");
                    if (DeviceRows > 0)
                    {
                        deletions += DeviceRows;
                        Logging.Logger("Deleted Device SQL File: " + sqlItem.FKey + "/" + sqlItem.FileGuid);
                    }

                    var SibiRows = DBFactory.GetDatabase().ExecuteNonQuery("DELETE FROM " + SibiTable.TableName + " WHERE " + SibiTable.FileGuid + "='" + sqlItem.FileGuid + "'");
                    if (SibiRows > 0)
                    {
                        deletions += SibiRows;
                        Logging.Logger("Deleted Sibi SQL File: " + sqlItem.FKey + "/" + sqlItem.FileGuid);
                    }
                }
                return deletions;
            }
            return 0;
        }

        private static int CleanSQLEntries(List<AttachScanInfo> missingSQLDirs)
        {
            if (missingSQLDirs.Count > 0)
            {
                DeviceAttachmentsCols DeviceTable = new DeviceAttachmentsCols();
                SibiAttachmentsCols SibiTable = new SibiAttachmentsCols();
                int deletions = 0;
                foreach (var sqlItem in missingSQLDirs)
                {
                    if (!CheckForPrimaryItem(sqlItem.FKey))
                    {
                        var DeviceRows = DBFactory.GetDatabase().ExecuteNonQuery("DELETE FROM " + DeviceTable.TableName + " WHERE " + DeviceTable.FKey + "='" + sqlItem.FKey + "'");
                        if (DeviceRows > 0)
                        {
                            deletions += DeviceRows;
                            Logging.Logger("Deleted " + DeviceRows.ToString() + " Device SQL Entries For: " + sqlItem.FKey);
                        }

                        var SibiRows = DBFactory.GetDatabase().ExecuteNonQuery("DELETE FROM " + SibiTable.TableName + " WHERE " + SibiTable.FKey + "='" + sqlItem.FKey + "'");
                        if (SibiRows > 0)
                        {
                            deletions += SibiRows;
                            Logging.Logger("Deleted " + SibiRows.ToString() + " Sibi SQL Entries For: " + sqlItem.FKey);
                        }
                    }
                }
                return deletions;
            }
            return 0;
        }

        private static int CleanFTPFiles(List<AttachScanInfo> missingFTPFiles)
        {
            int deletions = 0;
            foreach (var file in missingFTPFiles)
            {
                if (DeleteFtpAttachment(file.FileGuid, file.FKey))
                {
                    deletions++;
                    Logging.Logger("Deleted SQL File: " + file.FKey + "/" + file.FileGuid);
                }
            }
            return deletions;
        }

        private static int CleanFTPDirs(List<string> missingFTPDirs)
        {
            int deletions = 0;
            foreach (var fDir in missingFTPDirs)
            {
                if (DeleteFtpFolder(fDir))
                {
                    deletions++;
                    Logging.Logger("Deleted SQL Directory: " + fDir);
                }
            }
            return deletions;
        }

        /// <summary>
        /// Returns list of SQL entries not found in FTP directory list.
        /// </summary>
        /// <param name="sqlFiles"></param>
        /// <param name="ftpDirs"></param>
        /// <returns></returns>
        private static List<AttachScanInfo> ListMissingSQLDirs(List<AttachScanInfo> sqlFiles, List<string> ftpDirs)
        {
            List<AttachScanInfo> MissingDirs = new List<AttachScanInfo>();
            foreach (var SQLfile in sqlFiles)
            {
                bool match = false;
                ftpDirs.ForEach(f =>
                {
                    if (SQLfile.FKey == f)
                    {
                        match = true;
                    }
                });
                if (!match)
                {
                    if (!MissingDirs.Exists(d => SQLfile.FKey == d.FKey))
                    {
                        MissingDirs.Add(SQLfile);
                    }
                }
            }
            MissingDirs.ForEach(d => Logging.Logger("Orphan SQL Dir Found: " + d.FKey));
            return MissingDirs;
        }

        /// <summary>
        /// Returns list of FTP dirs that do not have an associated Device or Sibi request.
        /// </summary>
        /// <param name="ftpDirs"></param>
        /// <returns></returns>
        private static List<string> ListMissingFTPDirs(List<string> ftpDirs)
        {
            var MissingDirs = ftpDirs.FindAll(f => !CheckForPrimaryItem(f));
            MissingDirs.ForEach(f => Logging.Logger("Orphan FTP Dir Found: " + f));
            return MissingDirs;
        }

        /// <summary>
        /// Returns list of SQL files not found in FTP file list.
        /// </summary>
        /// <param name="sqlFiles"></param>
        /// <param name="ftpFiles"></param>
        /// <returns></returns>
        private static List<AttachScanInfo> ListMissingSQLFiles(List<AttachScanInfo> sqlFiles, List<AttachScanInfo> ftpFiles)
        {
            var MissingFiles = sqlFiles.Except(ftpFiles).ToList();
            MissingFiles.ForEach(f => Logging.Logger("Orphan SQL File Found: " + f.FKey + "/" + f.FileGuid));
            return sqlFiles.Except(ftpFiles).ToList();
        }

        /// <summary>
        /// Returns list of FTP files not found in SQL file list.
        /// </summary>
        /// <param name="sqlFiles"></param>
        /// <param name="ftpFiles"></param>
        /// <returns></returns>
        private static List<AttachScanInfo> ListMissingFTPFiles(List<AttachScanInfo> sqlFiles, List<AttachScanInfo> ftpFiles)
        {
            var MissingFiles = ftpFiles.Except(sqlFiles).ToList();
            MissingFiles.ForEach(f => Logging.Logger("Orphan FTP File Found: " + f.FKey + "/" + f.FileGuid));
            return MissingFiles;
        }

        private static List<AttachScanInfo> ListFTPFiles(List<string> ftpDirs)
        {
            List<AttachScanInfo> FTPFileList = new List<AttachScanInfo>();
            foreach (var fDir in ftpDirs)
            {
                foreach (var file in ListDirectory("ftp://" + ServerInfo.MySQLServerIP + "/attachments/" + ServerInfo.CurrentDataBase.ToString() + "/" + fDir + "/"))
                {
                    FTPFileList.Add(new AttachScanInfo(fDir, file));
                }
            }
            return FTPFileList;
        }

        private static List<AttachScanInfo> ListSQLFiles()
        {
            DeviceAttachmentsCols DeviceTable = new DeviceAttachmentsCols();
            SibiAttachmentsCols SibiTable = new SibiAttachmentsCols();
            List<AttachScanInfo> SQLFileList = new List<AttachScanInfo>();

            var devFiles = DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + DeviceTable.TableName);
            foreach (DataRow file in devFiles.Rows)
            {
                SQLFileList.Add(new AttachScanInfo(file[DeviceTable.FKey].ToString(), file[DeviceTable.FileGuid].ToString()));
            }

            var sibiFiles = DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + SibiTable.TableName);
            foreach (DataRow file in sibiFiles.Rows)
            {
                SQLFileList.Add(new AttachScanInfo(file[SibiTable.FKey].ToString(), file[SibiTable.FileGuid].ToString()));
            }

            return SQLFileList;
        }

        private static List<string> ListDirectory(string uri)
        {
            try
            {
                using (var resp = (FtpWebResponse)(ftpComms.ReturnFtpResponse(uri, WebRequestMethods.Ftp.ListDirectory)))
                {
                    System.IO.Stream responseStream = resp.GetResponseStream();
                    using (StreamReader reader = new StreamReader(responseStream))
                    {
                        List<string> files = new List<string>();
                        while (!reader.EndOfStream) //collect list of files in directory
                        {
                            files.Add(reader.ReadLine());
                        }
                        return files;
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return default(List<string>);
            }
        }

        private struct AttachScanInfo
        {
            public string FKey;
            public string FileGuid;

            public AttachScanInfo(string FKey, string FileGuid)
            {
                this.FKey = FKey;
                this.FileGuid = FileGuid;
            }
        }

        #endregion Methods
    }
}