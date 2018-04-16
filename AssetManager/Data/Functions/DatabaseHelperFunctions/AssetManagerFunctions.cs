using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Data.Functions
{
    public static class AssetManagerFunctions
    {
        #region "Methods"


        public static string GetDeployString(string name)
        {
            string query = "SELECT value FROM deployment_strings WHERE name = '" + name + "'";
            var results = DBFactory.GetDatabase().ExecuteScalarFromQueryString(query);

            if (results != null)
            {
                return results.ToString();
            }
            return string.Empty;
        }

        public static async Task<bool> HasPingHistory(Device device)
        {
            string query = "SELECT device_guid FROM device_ping_history WHERE device_guid = '" + device.Guid + "'";

            return await Task.Run(() =>
              {
                  using (var results = DBFactory.GetDatabase().DataTableFromQueryString(query))
                  {
                      if (results.Rows.Count > 0) return true;
                  }
                  return false;
              });
        }

        public static void ShowPingHistory(ExtendedForm parentForm, Device device)
        {
            string query = "SELECT timestamp, hostname, ip FROM device_ping_history WHERE device_guid = '" + device.Guid + "' ORDER BY timestamp DESC";

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
                    string newGuid = Guid.NewGuid().ToString();
                    ParamCollection insertParams = new ParamCollection();
                    insertParams.Add(EmployeesCols.Name, empInfo.Name);
                    insertParams.Add(EmployeesCols.Number, empInfo.Number);
                    insertParams.Add(EmployeesCols.Guid, newGuid);
                    DBFactory.GetDatabase().InsertFromParameters(EmployeesCols.TableName, insertParams.Parameters);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private static bool DeleteMasterSqlEntry(string sqlGuid, EntryType type)
        {
            try
            {
                string DeleteQuery = "";
                switch (type)
                {
                    case EntryType.Device:
                        DeleteQuery = Queries.DeleteDeviceByGuid(sqlGuid);
                        break;

                    case EntryType.Sibi:
                        DeleteQuery = Queries.DeleteSibiRequestByGuid(sqlGuid);
                        break;
                }
                if (DBFactory.GetDatabase().ExecuteNonQuery(DeleteQuery) > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        public static bool DeleteDevice(string guid)
        {
            try
            {
                // if has attachments, delete ftp directory, then delete the sql records.
                if (FtpFunctions.HasFtpFolder(guid))
                {
                    if (!FtpFunctions.DeleteFtpFolder(guid)) return false;
                }
                //delete sql records
                return DeleteMasterSqlEntry(guid, EntryType.Device);
            }
            catch (Exception ex)
            {
                return ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static bool DeleteSibiRequest(string guid)
        {
            try
            {
                // if has attachments, delete ftp directory, then delete the sql records.
                if (FtpFunctions.HasFtpFolder(guid))
                {
                    if (!FtpFunctions.DeleteFtpFolder(guid)) return false;
                }
                //delete sql records
                return DeleteMasterSqlEntry(guid, EntryType.Sibi);
            }
            catch (Exception ex)
            {
                return ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public static int DeleteSqlAttachment(Attachment attachment)
        {
            try
            {
                //Delete FTP Attachment
                if (FtpFunctions.DeleteFtpAttachment(attachment.FileGuid, attachment.FolderGuid))
                {
                    //delete SQL entry
                    var deleteQuery = "DELETE FROM " + attachment.AttachTable.TableName + " WHERE " + attachment.AttachTable.FileGuid + "='" + attachment.FileGuid + "'";
                    return DBFactory.GetDatabase().ExecuteNonQuery(deleteQuery);
                }
                return -1;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return -1;
            }
        }

        public static bool DeviceExists(string assetTag, string serial)
        {
            bool assetExists = false;
            bool serialExists = false;
            //Allow NA value because users do not always have an Asset Tag for new devices.
            if (assetTag == "NA")
            {
                assetExists = false;
            }
            else
            {
                string assetResult = GetSqlValue(DevicesCols.TableName, DevicesCols.AssetTag, assetTag, DevicesCols.AssetTag);
                if (!string.IsNullOrEmpty(assetResult))
                {
                    assetExists = true;
                }
                else
                {
                    assetExists = false;
                }
            }

            string serialResult = GetSqlValue(DevicesCols.TableName, DevicesCols.Serial, serial, DevicesCols.Serial);
            if (!string.IsNullOrEmpty(serialResult))
            {
                serialExists = true;
            }
            else
            {
                serialExists = false;
            }
            if (serialExists | assetExists)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsEmployeeInDB(string empNumber)
        {
            string empName = GetSqlValue(EmployeesCols.TableName, EmployeesCols.Number, empNumber, EmployeesCols.Name);
            if (!string.IsNullOrEmpty(empName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static Device FindDeviceFromAssetOrSerial(string searchValue, FindDevType type)
        {
            try
            {
                if (type == FindDevType.AssetTag)
                {
                    QueryParamCollection searchParam = new QueryParamCollection();
                    searchParam.Add(DevicesCols.AssetTag, searchValue, true);
                    return new Device(DBFactory.GetDatabase().DataTableFromParameters(Queries.SelectDevicesPartial, searchParam.Parameters));
                }
                else if (type == FindDevType.Serial)
                {

                    QueryParamCollection searchParam = new QueryParamCollection();
                    searchParam.Add(DevicesCols.Serial, searchValue, true);
                    return new Device(DBFactory.GetDatabase().DataTableFromParameters(Queries.SelectDevicesPartial, searchParam.Parameters));
                }
                return null;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
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

        public static string GetMunisCodeFromAssetCode(string assetCode)
        {
            return GetSqlValue("munis_codes", "asset_man_code", assetCode, "munis_code");
        }

        public static string GetSqlValue(string table, string fieldIn, string valueIn, string fieldOut)
        {
            string sqlQRY = "SELECT " + fieldOut + " FROM " + table + " WHERE ";

            QueryParamCollection searchParam = new QueryParamCollection();
            searchParam.Add(fieldIn, valueIn, true);
            var result = DBFactory.GetDatabase().ExecuteScalarFromCommand(DBFactory.GetDatabase().GetCommandFromParams(sqlQRY, searchParam.Parameters));
            if (result != null)
            {
                return result.ToString();
            }
            else
            {
                return string.Empty;
            }
        }

        public static void SetAttachmentCount(ToolStripButton targetTool, string attachFolderGuid, AttachmentsBaseCols attachTable)
        {
            if (!GlobalSwitches.CachedMode)
            {
                try
                {
                    int attachCount = Convert.ToInt32(GetSqlValue(attachTable.TableName, attachTable.FKey, attachFolderGuid, "COUNT(*)"));
                    targetTool.Text = "(" + attachCount.ToString() + ")";
                    targetTool.ToolTipText = "Attachments " + targetTool.Text;
                }
                catch (Exception ex)
                {
                    ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
        }

        public static int UpdateSqlValue(string table, string fieldIn, string valueIn, string idField, string idValue)
        {
            return DBFactory.GetDatabase().UpdateValue(table, fieldIn, valueIn, idField, idValue);
        }

        #endregion "Methods"

        private enum EntryType
        {
            Sibi,
            Device
        }

        // METODO: Un-nest.
        public enum FindDevType
        {
            AssetTag,
            Serial
        }
    }
}