using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.Threading.Tasks;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Helpers;


namespace AssetManager.Data.Functions
{
    public static class AssetManagerFunctions
    {
        #region "Methods"

        public static async Task<bool> HasPingHistory(Device device)
        {
            string query = "SELECT device_guid FROM device_ping_history WHERE device_guid = '" + device.GUID + "'";

            return await Task.Run(() =>
              {
                  Task.Delay(1000).Wait();
                  using (var results = DBFactory.GetDatabase().DataTableFromQueryString(query))
                  {
                      if (results.Rows.Count > 0) return true;
                  }
                  return false;
              });

        }

        public static void ShowPingHistory(ExtendedForm parentForm, Device device)
        {
            string query = "SELECT timestamp, hostname, ip FROM device_ping_history WHERE device_guid = '" + device.GUID + "' ORDER BY timestamp DESC";

            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(query))
            {
                if (results.Rows.Count > 0)
                {

                    results.Columns.Add("location");

                    foreach (DataRow row in results.Rows)
                    {
                        row["location"] = NetworkInfo.LocationOfIP(row["ip"].ToString());
                    }

                    var newGrid = new GridForm(parentForm, "Ping History - " + device.HostName);
                    newGrid.AddGrid("pingGrid", "Ping History", results);
                    newGrid.Show();

                }
            }

        }


        public static void AddNewEmp(MunisEmployee empInfo)
        {
            try
            {
                if (!IsEmployeeInDB(empInfo.Number))
                {
                    string UID = Guid.NewGuid().ToString();
                    List<DBParameter> InsertEmployeeParams = new List<DBParameter>();
                    InsertEmployeeParams.Add(new DBParameter(EmployeesCols.Name, empInfo.Name));
                    InsertEmployeeParams.Add(new DBParameter(EmployeesCols.Number, empInfo.Number));
                    InsertEmployeeParams.Add(new DBParameter(EmployeesCols.UID, UID));
                    DBFactory.GetDatabase().InsertFromParameters(EmployeesCols.TableName, InsertEmployeeParams);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        /// <summary>
        /// Searches the database for the best possible match to the specified search name using a Levenshtein distance algorithm.
        /// </summary>
        /// <param name="empSearchName"></param>
        /// <param name="MinSearchDistance"></param>
        /// <returns></returns>
        public static MunisEmployee SmartEmployeeSearch(string empSearchName, int MinSearchDistance = 10)
        {
            if (empSearchName.Trim() != "")
            {
                // Split the name string by spaces to try and separate first/last names.
                string[] SplitName = empSearchName.Split(char.Parse(" "));

                // Init new list of search result objects.
                List<SmartEmpSearchInfo> Results = new List<SmartEmpSearchInfo>();

                // Get results for complete name from employees table
                Results.AddRange(GetEmpSearchResults(EmployeesCols.TableName, empSearchName, EmployeesCols.Name, EmployeesCols.Number));

                // Get results for complete name from devices table
                Results.AddRange(GetEmpSearchResults(DevicesCols.TableName, empSearchName, DevicesCols.CurrentUser, DevicesCols.MunisEmpNum));

                foreach (string s in SplitName)
                {
                    //Get results for partial name from employees table
                    Results.AddRange(GetEmpSearchResults(EmployeesCols.TableName, s, EmployeesCols.Name, EmployeesCols.Number));

                    //Get results for partial name from devices table
                    Results.AddRange(GetEmpSearchResults(DevicesCols.TableName, s, DevicesCols.CurrentUser, DevicesCols.MunisEmpNum));
                }


                if (Results.Count > 0)
                {
                    Results = NarrowResults(Results);
                    var BestMatch = FindBestSmartSearchMatch(Results);
                    if (BestMatch.MatchDistance < MinSearchDistance)
                    {
                        return BestMatch.SearchResult;
                    }
                }
            }
            return new MunisEmployee();
        }

        /// <summary>
        /// Reprocesses the search results to obtain a more accurate Levenshtein distance calculation.
        /// </summary>
        /// <param name="results"></param>
        /// <remarks>This is done because the initial calculations are performed against the full length
        /// of the returned names (First and last name), and the distance between the search string and name string may be inaccurate.</remarks>
        /// <returns></returns>
        private static List<SmartEmpSearchInfo> NarrowResults(List<SmartEmpSearchInfo> results)
        {
            List<SmartEmpSearchInfo> newResults = new List<SmartEmpSearchInfo>();
            //Iterate through results
            foreach (var result in results)
            {
                //Split the results returned string by spaces
                var resultSplit = result.SearchResult.Name.Split(char.Parse(" "));
                if (resultSplit.Count() > 0)
                {
                    //Iterate through the separate strings
                    foreach (var resultString in resultSplit)
                    {
                        //Make sure the result string contains the search string
                        if (resultString.Contains(result.SearchString) && resultString.StartsWith(result.SearchString))
                        {
                            //Get a new Levenshtein distance.
                            var NewDistance = Fastenshtein.Levenshtein.Distance(resultString, result.SearchString);
                            //If the strings are closer together, add the new data.
                            if (NewDistance < result.MatchDistance)
                            {
                                newResults.Add(new SmartEmpSearchInfo(result.SearchResult, result.SearchString, NewDistance));
                            }
                            else
                            {
                                newResults.Add(result);
                            }
                        }
                        else
                        {
                            newResults.Add(result);
                        }
                    }
                }
            }
            return newResults;
        }

        /// <summary>
        /// Finds the best match within the results. The item with shortest Levenshtein distance and the longest match length (string length) is preferred.
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        private static SmartEmpSearchInfo FindBestSmartSearchMatch(List<SmartEmpSearchInfo> results)
        {
            //Initial minimum distance
            int MinDist = results.First().MatchDistance;
            //Initial minimum match
            SmartEmpSearchInfo MinMatch = results.First();
            SmartEmpSearchInfo LongestMatch = new SmartEmpSearchInfo();
            List<SmartEmpSearchInfo> DeDupDist = new List<SmartEmpSearchInfo>();
            //Iterate through the results and determine the result with the shortest Levenshtein distance.
            foreach (var result in results)
            {
                if (result.MatchDistance < MinDist)
                {
                    MinDist = result.MatchDistance;
                    MinMatch = result;
                }
            }
            //De-duplicate the results and iterate to determine which result of the Levenshtein shortest distances has the longest match length. (Greatest number of matches)
            DeDupDist = results.Distinct().ToList();
            if (DeDupDist.Count > 0)
            {
                int MaxMatchLen = 0;
                foreach (var dup in DeDupDist)
                {
                    if (dup.MatchDistance == MinDist)
                    {
                        if (dup.MatchLength > MaxMatchLen)
                        {
                            MaxMatchLen = dup.MatchLength;
                            LongestMatch = dup;
                        }
                    }
                }
                //Return best match by length and Levenshtein distance.
                return LongestMatch;
            }
            //Return best match by Levenshtein distance only. (If no duplicates)
            return MinMatch;
        }

        /// <summary>
        /// Queries the database for a list of results that contains the employee name result and computed Levenshtein distance to the search string.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="searchEmpName"></param>
        /// <param name="empNameColumn"></param>
        /// <param name="empNumColumn"></param>
        /// <returns></returns>
        private static List<SmartEmpSearchInfo> GetEmpSearchResults(string tableName, string searchEmpName, string empNameColumn, string empNumColumn)
        {
            List<SmartEmpSearchInfo> tmpResults = new List<SmartEmpSearchInfo>();
            List<DBQueryParameter> EmpSearchParams = new List<DBQueryParameter>();
            EmpSearchParams.Add(new DBQueryParameter(empNameColumn, searchEmpName, false));
            using (DataTable data = DBFactory.GetDatabase().DataTableFromParameters("SELECT * FROM " + tableName + " WHERE", EmpSearchParams))
            {
                foreach (DataRow row in data.Rows)
                {
                    tmpResults.Add(new SmartEmpSearchInfo(new MunisEmployee(row[empNameColumn].ToString(), row[empNumColumn].ToString()), searchEmpName, Fastenshtein.Levenshtein.Distance(searchEmpName.ToUpper(), row[empNameColumn].ToString().ToUpper())));
                }
            }
            return tmpResults;
        }

        public static bool DeleteMasterSqlEntry(string sqlGUID, EntryType type)
        {
            try
            {
                string DeleteQuery = "";
                switch (type)
                {
                    case EntryType.Device:
                        DeleteQuery = Queries.DeleteDeviceByGUID(sqlGUID);
                        break;

                    case EntryType.Sibi:
                        DeleteQuery = Queries.DeleteSibiRequestByGUID(sqlGUID);
                        break;
                }
                if (DBFactory.GetDatabase().ExecuteQuery(DeleteQuery) > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return false;
            }
        }

        public static bool DeleteFtpAndSql(string sqlGUID, EntryType type)
        {
            try
            {
                if (GlobalInstances.FTPFunc.HasFtpFolder(sqlGUID))
                {
                    if (GlobalInstances.FTPFunc.DeleteFtpFolder(sqlGUID))
                        return DeleteMasterSqlEntry(sqlGUID, type);
                    // if has attachments, delete ftp directory, then delete the sql records.
                }
                else
                {
                    return DeleteMasterSqlEntry(sqlGUID, type);
                    //delete sql records
                }
            }
            catch (Exception ex)
            {
                return ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            return false;
        }

        public static int DeleteSqlAttachment(Attachment attachment)
        {
            try
            {
                var AttachmentFolderID = GetSqlValue(attachment.AttachTable.TableName, attachment.AttachTable.FileUID, attachment.FileUID, attachment.AttachTable.FKey);
                //Delete FTP Attachment
                if (GlobalInstances.FTPFunc.DeleteFtpAttachment(attachment.FileUID, AttachmentFolderID))
                {
                    //delete SQL entry
                    var SQLDeleteQry = "DELETE FROM " + attachment.AttachTable.TableName + " WHERE " + attachment.AttachTable.FileUID + "='" + attachment.FileUID + "'";
                    return DBFactory.GetDatabase().ExecuteQuery(SQLDeleteQry);
                }
                return -1;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return -1;
            }
        }

        public static bool DeviceExists(string assetTag, string serial)
        {
            bool bolAsset = false;
            bool bolSerial = false;
            //Allow NA value because users do not always have an Asset Tag for new devices.
            if (assetTag == "NA")
            {
                bolAsset = false;
            }
            else
            {
                string CheckAsset = GetSqlValue(DevicesCols.TableName, DevicesCols.AssetTag, assetTag, DevicesCols.AssetTag);
                if (!string.IsNullOrEmpty(CheckAsset))
                {
                    bolAsset = true;
                }
                else
                {
                    bolAsset = false;
                }
            }

            string CheckSerial = GetSqlValue(DevicesCols.TableName, DevicesCols.Serial, serial, DevicesCols.Serial);
            if (!string.IsNullOrEmpty(CheckSerial))
            {
                bolSerial = true;
            }
            else
            {
                bolSerial = false;
            }
            if (bolSerial | bolAsset)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static DataTable DevicesBySupervisor(ExtendedForm parentForm)
        {
            try
            {
                MunisEmployee SupInfo = default(MunisEmployee);
                SupInfo = GlobalInstances.MunisFunc.MunisUserSearch(parentForm);
                if (!string.IsNullOrEmpty(SupInfo.Number))
                {
                    OtherFunctions.SetWaitCursor(true, parentForm);
                    using (DataTable DeviceList = new DataTable())
                    {
                        using (DataTable EmpList = GlobalInstances.MunisFunc.ListOfEmpsBySup(SupInfo.Number))
                        {
                            foreach (DataRow r in EmpList.Rows)
                            {
                                using (DataTable tmpTable = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectDevicesByEmpNum(r["a_employee_number"].ToString())))
                                {
                                    DeviceList.Merge(tmpTable);
                                }
                            }
                            return DeviceList;
                        }
                    }
                }
                else
                {
                    return null;
                }
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, parentForm);
            }
        }

        public static bool IsEmployeeInDB(string empNum)
        {
            string EmpName = GetSqlValue(EmployeesCols.TableName, EmployeesCols.Number, empNum, EmployeesCols.Name);
            if (!string.IsNullOrEmpty(EmpName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Device FindDeviceFromAssetOrSerial(string searchVal, FindDevType type)
        {
            try
            {
                if (type == FindDevType.AssetTag)
                {
                    List<DBQueryParameter> Params = new List<DBQueryParameter>();
                    Params.Add(new DBQueryParameter(DevicesCols.AssetTag, searchVal, true));
                    return new Device(DBFactory.GetDatabase().DataTableFromParameters(Queries.SelectDevicesPartial, Params));//"SELECT * FROM " + DevicesCols.TableName + " WHERE ", Params));
                }
                else if (type == FindDevType.Serial)
                {
                    List<DBQueryParameter> Params = new List<DBQueryParameter>();
                    Params.Add(new DBQueryParameter(DevicesCols.Serial, searchVal, true));
                    return new Device(DBFactory.GetDatabase().DataTableFromParameters(Queries.SelectDevicesPartial, Params));
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return null;
            }
        }

        public static string GetTVApiToken()
        {
            using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectTVApiToken))
            {
                var token = results.Rows[0]["apitoken"].ToString();
                return token;
            }
        }

        //public DeviceObject GetDeviceInfoFromGUID(string deviceGUID)
        //{
        //    return new DeviceObject(AssetManager.DBFactory.GetDatabase().DataTableFromQueryString("SELECT * FROM " + DevicesCols.TableName + " WHERE " + DevicesCols.DeviceUID + "='" + deviceGUID + "'"));
        //}

        public static string GetMunisCodeFromAssetCode(string assetCode)
        {
            return GetSqlValue("munis_codes", "asset_man_code", assetCode, "munis_code");
        }

        public static string GetSqlValue(string table, string fieldIn, string valueIn, string fieldOut)
        {
            string sqlQRY = "SELECT " + fieldOut + " FROM " + table + " WHERE ";
            List<DBQueryParameter> Params = new List<DBQueryParameter>();
            Params.Add(new DBQueryParameter(fieldIn, valueIn, true));
            var Result = DBFactory.GetDatabase().ExecuteScalarFromCommand(DBFactory.GetDatabase().GetCommandFromParams(sqlQRY, Params));
            if (Result != null)
            {
                return Result.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static void SetAttachmentCount(ToolStripButton targetTool, string attachFolderUID, AttachmentsBaseCols attachTable)
        {
            if (!GlobalSwitches.CachedMode)
            {
                try
                {
                    int attachCount = Convert.ToInt32(GetSqlValue(attachTable.TableName, attachTable.FKey, attachFolderUID, "COUNT(*)"));
                    targetTool.Text = "(" + attachCount.ToString() + ")";
                    targetTool.ToolTipText = "Attachments " + targetTool.Text;
                }
                catch (Exception ex)
                {
                    ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                }
            }
        }

        public static int UpdateSqlValue(string table, string fieldIn, string valueIn, string idField, string idValue)
        {
            return DBFactory.GetDatabase().UpdateValue(table, fieldIn, valueIn, idField, idValue);
        }

        #endregion "Methods"
    }
}