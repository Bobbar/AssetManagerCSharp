using System;

namespace AssetManager.Data.Communications
{
    public static class Queries
    {
        #region DeviceQueries

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="HistoricalDevicesCols.HistoryEntryUID"/> = <paramref name="entryGuid"/>
        /// </summary>
        /// <param name="entryGuid"></param>
        /// <returns></returns>
        public static string SelectHistoricalDeviceEntry(string entryGuid)
        {
            return "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.HistoryEntryUID + "='" + entryGuid + "'";
        }

        /// <summary>
        /// SELECT * FROM <see cref="DevicesCols.TableName"/> WHERE <see cref="DevicesCols.TableName"/> = <paramref name="deviceGuid"/>
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <returns></returns>
        public static string SelectDeviceByGuid(string deviceGuid)
        {
            return "SELECT * FROM " + DevicesCols.TableName + " WHERE " + DevicesCols.DeviceUID + " = '" + deviceGuid + "'";
        }

        /// <summary>
        /// SELECT * FROM <see cref="DevicesCols.TableName"/> ORDER BY <see cref="DevicesCols.InputDateTime"/> DESC
        /// </summary>
        public static string SelectDevicesTable { get; } = "SELECT * FROM " + DevicesCols.TableName + " ORDER BY " + DevicesCols.InputDateTime + " DESC";

        /// <summary>
        /// SELECT * FROM <see cref="DevicesCols.TableName"/> WHERE <see cref="DevicesCols.MunisEmpNum"/> = <paramref name="empNum"/>
        /// </summary>
        /// <param name="empNum"></param>
        /// <returns></returns>
        public static string SelectDevicesByEmpNum(string empNum)
        {
            return "SELECT * FROM " + DevicesCols.TableName + " WHERE " + DevicesCols.MunisEmpNum + "='" + empNum + "'";
        }

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> LIMIT 0
        /// </summary>
        public static string SelectEmptyHistoricalTable { get; } = "SELECT * FROM " + HistoricalDevicesCols.TableName + " LIMIT 0";

        /// <summary>
        /// SELECT * FROM <see cref="TrackablesCols.TableName"/> WHERE <see cref="TrackablesCols.DeviceUID"/> = <paramref name="deviceGuid"/> ORDER BY <see cref="TrackablesCols.DateStamp"/> DESC
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <returns></returns>
        public static string SelectTrackingByDevGuid(string deviceGuid)
        {
            return "SELECT * FROM " + TrackablesCols.TableName + " WHERE " + TrackablesCols.DeviceUID + " = '" + deviceGuid + "' ORDER BY " + TrackablesCols.DateStamp + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="TrackablesCols.TableName"/> WHERE <see cref="TrackablesCols.UID"/> = <paramref name="entryGuid"/>
        /// </summary>
        /// <param name="entryGuid"></param>
        /// <returns></returns>
        public static string SelectTrackingEntryByGuid(string entryGuid)
        {
            return "SELECT * FROM " + TrackablesCols.TableName + " WHERE  " + TrackablesCols.UID + " = '" + entryGuid + "'";
        }

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="DevicesBaseCols.DeviceUID"/> = <paramref name="deviceGuid"/> ORDER BY <see cref=" HistoricalDevicesCols.ActionDateTime"/> DESC
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <returns></returns>
        public static string SelectDeviceHistoricalTable(string deviceGuid)
        {
            return "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.DeviceUID + " = '" + deviceGuid + "' ORDER BY " + HistoricalDevicesCols.ActionDateTime + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="DevicesBaseCols.DeviceUID"/> = <paramref name="deviceGuid"/> AND <see cref="HistoricalDevicesCols.ActionDateTime"/> "LESS THAN" <paramref name="startDate"/> ORDER BY <see cref="HistoricalDevicesCols.ActionDateTime"/> DESC
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public static string SelectDevHistoricalEntriesOlderThan(string deviceGuid, DateTime startDate)
        {
            return "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.DeviceUID + " = '" + deviceGuid + "' AND " + HistoricalDevicesCols.ActionDateTime + " < '" + startDate.ToString(DataConsistency.strDBDateTimeFormat) + "' ORDER BY " + HistoricalDevicesCols.ActionDateTime + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="DevicesCols.TableName "/> WHERE
        /// </summary>
        public static string SelectDevicesPartial { get; } = "SELECT * FROM " + DevicesCols.TableName + " WHERE ";

        /// <summary>
        /// "DELETE FROM <see cref="DevicesCols.TableName"/> WHERE <see cref="DevicesCols.DeviceUID"/> = <paramref name="deviceGuid"/>
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <returns></returns>
        public static string DeleteDeviceByGuid(string deviceGuid)
        {
            return "DELETE FROM " + DevicesCols.TableName + " WHERE " + DevicesCols.DeviceUID + "='" + deviceGuid + "'";
        }

        /// <summary>
        /// "DELETE FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="HistoricalDevicesCols.HistoryEntryUID"/> = <paramref name="entryGuid"/>
        /// </summary>
        /// <param name="entryGuid"></param>
        /// <returns></returns>
        public static string DeleteHistoricalEntryByGuid(string entryGuid)
        {
            return "DELETE FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.HistoryEntryUID + "='" + entryGuid + "'";
        }

        #endregion DeviceQueries

        #region SibiQueries

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestItemsCols.TableName"/> INNER JOIN <see cref="SibiRequestCols.TableName"/> ON <see cref="SibiRequestItemsCols.RequestUID"/> = <see cref="SibiRequestCols.UID"/> WHERE <see cref="SibiRequestItemsCols.ItemUID"/> = <paramref name="itemGuid"/>
        /// </summary>
        /// <param name="itemGuid"></param>
        /// <returns></returns>
        public static string SelectSibiRequestAndItemByItemGuid(string itemGuid)
        {
            return "SELECT * FROM " + SibiRequestItemsCols.TableName + " INNER JOIN " + SibiRequestCols.TableName + " ON " + SibiRequestItemsCols.RequestUID + " = " + SibiRequestCols.UID + " WHERE " + SibiRequestItemsCols.ItemUID + "='" + itemGuid + "'";
        }

        /// <summary>
        /// SELECT DISTINCT <see cref="SibiRequestCols.DateStamp"/> FROM <see cref="SibiRequestCols.TableName"/> ORDER BY <see cref="SibiRequestCols.DateStamp"/> DESC
        /// </summary>
        public static string SelectSibiDisplayYears { get; } = "SELECT DISTINCT " + SibiRequestCols.DateStamp + " FROM " + SibiRequestCols.TableName + " ORDER BY " + SibiRequestCols.DateStamp + " DESC";

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestCols.TableName"/> FROM <see cref="SibiRequestCols.TableName"/> ORDER BY <see cref="SibiRequestCols.RequestNumber"/> DESC
        /// </summary>
        public static string SelectSibiRequestsTable { get; } = "SELECT * FROM " + SibiRequestCols.TableName + " ORDER BY " + SibiRequestCols.RequestNumber + " DESC";

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestCols.TableName"/> WHERE <see cref="SibiRequestCols.DateStamp"/> LIKE '% <paramref name="year"/> %' ORDER BY <see cref="SibiRequestCols.RequestNumber"/> DESC
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static string SelectSibiRequestsByYear(string year)
        {
            return "SELECT * FROM " + SibiRequestCols.TableName + " WHERE " + SibiRequestCols.DateStamp + " LIKE '%" + year + "%' ORDER BY " + SibiRequestCols.RequestNumber + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestCols.TableName"/> WHERE <see cref="SibiRequestCols.UID"/> = <see cref="requestGuid"/>
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <returns></returns>
        public static string SelectSibiRequestsByGuid(string requestGuid)
        {
            return "SELECT * FROM " + SibiRequestCols.TableName + " WHERE " + SibiRequestCols.UID + "='" + requestGuid + "'";
        }

        /// <summary>
        /// SELECT <paramref name="columnsString"/> FROM <see cref="SibiRequestItemsCols.TableName"/> LIMIT 0
        /// </summary>
        /// <param name="columnsString"></param>
        /// <returns></returns>
        public static string SelectEmptySibiItemsTable(string columnsString)
        {
            return "SELECT " + columnsString + " FROM " + SibiRequestItemsCols.TableName + " LIMIT 0";
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestCols.TableName"/> LIMIT 0
        /// </summary>
        public static string SelectEmptySibiRequestTable { get; } = "SELECT * FROM " + SibiRequestCols.TableName + " LIMIT 0";

        /// <summary>
        /// SELECT <paramref name="columnsString"/> FROM <see cref="SibiRequestItemsCols.TableName"/> WHERE <see cref="SibiRequestItemsCols.RequestUID"/> = <paramref name="requestGuid"/> ORDER BY <see cref="SibiRequestItemsCols.Timestamp"/>
        /// </summary>
        /// <param name="columnsString"></param>
        /// <param name="requestGuid"></param>
        /// <returns></returns>
        public static string SelectSibiRequestItems(string columnsString, string requestGuid)
        {
            return "SELECT " + columnsString + " FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.RequestUID + "='" + requestGuid + "' ORDER BY " + SibiRequestItemsCols.Timestamp;
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiNotesCols.TableName"/> WHERE <see cref="SibiNotesCols.RequestUID"/> = <paramref name="requestGuid"/> ORDER BY <see cref="SibiNotesCols.DateStamp"/> DESC
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <returns></returns>
        public static string SelectSibiNotes(string requestGuid)
        {
            return "SELECT * FROM " + SibiNotesCols.TableName + " WHERE " + SibiNotesCols.RequestUID + "='" + requestGuid + "' ORDER BY " + SibiNotesCols.DateStamp + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiNotesCols.TableName"/> WHERE <see cref="SibiNotesCols.NoteUID"/> = <paramref name="noteGuid"/>
        /// </summary>
        /// <param name="noteGuid"></param>
        /// <returns></returns>
        public static string SelectNoteByGuid(string noteGuid)
        {
            return "SELECT * FROM " + SibiNotesCols.TableName + " WHERE " + SibiNotesCols.NoteUID + "='" + noteGuid + "'";
        }

        /// <summary>
        /// DELETE FROM <see cref="SibiNotesCols.TableName"/> WHERE <see cref="SibiNotesCols.NoteUID"/> = <paramref name="noteGuid"/>
        /// </summary>
        /// <param name="noteGuid"></param>
        /// <returns></returns>
        public static string DeleteSibiNote(string noteGuid)
        {
            return "DELETE FROM " + SibiNotesCols.TableName + " WHERE " + SibiNotesCols.NoteUID + "='" + noteGuid + "'";
        }

        /// <summary>
        /// DELETE FROM <see cref="SibiRequestCols.TableName"/> WHERE <see cref="SibiRequestCols.UID"/> = <paramref name="requestGuid"/>
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <returns></returns>
        public static string DeleteSibiRequestByGuid(string requestGuid)
        {
            return "DELETE FROM " + SibiRequestCols.TableName + " WHERE " + SibiRequestCols.UID + "='" + requestGuid + "'";
        }

        #endregion SibiQueries

        #region AttribAndUserQueries

        /// <summary>
        /// SELECT * FROM <paramref name="attribTable"/> LEFT OUTER JOIN munis_codes on <paramref name="attribName"/>.db_value = munis_codes.asset_man_code WHERE type_name ='<paramref name="attribName"/>' ORDER BY <see cref="ComboCodesBaseCols.DisplayValue"/>
        /// </summary>
        /// <param name="attribTable"></param>
        /// <param name="attribName"></param>
        /// <returns></returns>
        public static string SelectAttributeCodes(string attribTable, string attribName)
        {
            return "SELECT * FROM " + attribTable + " LEFT OUTER JOIN munis_codes on " + attribTable + ".db_value = munis_codes.asset_man_code WHERE type_name ='" + attribName + "' ORDER BY " + ComboCodesBaseCols.DisplayValue;
        }

        /// <summary>
        /// SELECT * FROM <see cref="UsersCols.TableName"/> WHERE <see cref="UsersCols.UserName"/> = <paramref name="username"/>
        /// </summary>
        /// <param name="username"></param>
        /// <returns></returns>
        public static string SelectUserByName(string username)
        {
            return "SELECT * FROM " + UsersCols.TableName + " WHERE " + UsersCols.UserName + "='" + username + "' LIMIT 1";
        }

        /// <summary>
        /// SELECT * FROM <see cref="UsersCols.TableName"/>
        /// </summary>
        public static string SelectUsersTable { get; } = "SELECT * FROM " + UsersCols.TableName;

        /// <summary>
        /// SELECT * FROM <see cref="SecurityCols.TableName"/> ORDER BY <see cref="SecurityCols.AccessLevel"/>
        /// </summary>
        public static string SelectSecurityTable { get; } = "SELECT * FROM " + SecurityCols.TableName + " ORDER BY " + SecurityCols.AccessLevel;

        #endregion AttribAndUserQueries

        /// <summary>
        /// SELECT apitoken FROM teamviewer_info
        /// </summary>
        public static string SelectTVApiToken { get; } = "SELECT apitoken FROM teamviewer_info";
    }
}