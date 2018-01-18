using System;

namespace AssetManager//.Data.DataComms.AssetManager.SQL
{
    public static class Queries
    {
        #region DeviceQueries

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="HistoricalDevicesCols.HistoryEntryUID"/> = <paramref name="entryGUID"/>
        /// </summary>
        /// <param name="entryGUID"></param>
        /// <returns></returns>
        public static string SelectHistoricalDeviceEntry(string entryGUID)
        {
            return "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.HistoryEntryUID + "='" + entryGUID + "'";
        }

        /// <summary>
        /// SELECT * FROM <see cref="DevicesCols.TableName"/> WHERE <see cref="DevicesCols.TableName"/> = <paramref name="deviceGUID"/>
        /// </summary>
        /// <param name="deviceGUID"></param>
        /// <returns></returns>
        public static string SelectDeviceByGUID(string deviceGUID)
        {
            return "SELECT * FROM " + DevicesCols.TableName + " WHERE " + DevicesCols.DeviceUID + " = '" + deviceGUID + "'";
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
        /// SELECT * FROM <see cref="TrackablesCols.TableName"/> WHERE <see cref="TrackablesCols.DeviceUID"/> = <paramref name="deviceGUID"/> ORDER BY <see cref="TrackablesCols.DateStamp"/> DESC
        /// </summary>
        /// <param name="deviceGUID"></param>
        /// <returns></returns>
        public static string SelectTrackingByDevGUID(string deviceGUID)
        {
            return "SELECT * FROM " + TrackablesCols.TableName + " WHERE " + TrackablesCols.DeviceUID + " = '" + deviceGUID + "' ORDER BY " + TrackablesCols.DateStamp + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="TrackablesCols.TableName"/> WHERE <see cref="TrackablesCols.UID"/> = <paramref name="entryGUID"/>
        /// </summary>
        /// <param name="entryGUID"></param>
        /// <returns></returns>
        public static string SelectTrackingEntryByGUID(string entryGUID)
        {
            return "SELECT * FROM " + TrackablesCols.TableName + " WHERE  " + TrackablesCols.UID + " = '" + entryGUID + "'";
        }

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="DevicesBaseCols.DeviceUID"/> = <paramref name="deviceGUID"/> ORDER BY <see cref=" HistoricalDevicesCols.ActionDateTime"/> DESC
        /// </summary>
        /// <param name="deviceGUID"></param>
        /// <returns></returns>
        public static string SelectDeviceHistoricalTable(string deviceGUID)
        {
            return "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.DeviceUID + " = '" + deviceGUID + "' ORDER BY " + HistoricalDevicesCols.ActionDateTime + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="DevicesBaseCols.DeviceUID"/> = <paramref name="deviceGUID"/> AND <see cref="HistoricalDevicesCols.ActionDateTime"/> "LESS THAN" <paramref name="startDate"/> ORDER BY <see cref="HistoricalDevicesCols.ActionDateTime"/> DESC
        /// </summary>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public static string SelectDevHistoricalEntriesOlderThan(string deviceGUID, DateTime startDate)
        {
            return "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.DeviceUID + " = '" + deviceGUID + "' AND " + HistoricalDevicesCols.ActionDateTime + " < '" + startDate.ToString(DataConsistency.strDBDateTimeFormat) + "' ORDER BY " + HistoricalDevicesCols.ActionDateTime + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="DevicesCols.TableName "/> WHERE
        /// </summary>
        public static string SelectDevicesPartial { get; } = "SELECT * FROM " + DevicesCols.TableName + " WHERE ";

        /// <summary>
        /// "DELETE FROM <see cref="DevicesCols.TableName"/> WHERE <see cref="DevicesCols.DeviceUID"/> = <paramref name="deviceGUID"/>
        /// </summary>
        /// <param name="deviceGUID"></param>
        /// <returns></returns>
        public static string DeleteDeviceByGUID(string deviceGUID)
        {
            return "DELETE FROM " + DevicesCols.TableName + " WHERE " + DevicesCols.DeviceUID + "='" + deviceGUID + "'";
        }

        /// <summary>
        /// "DELETE FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="HistoricalDevicesCols.HistoryEntryUID"/> = <paramref name="entryGUID"/>
        /// </summary>
        /// <param name="entryGUID"></param>
        /// <returns></returns>
        public static string DeleteHistoricalEntryByGUID(string entryGUID)
        {
            return "DELETE FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.HistoryEntryUID + "='" + entryGUID + "'";
        }

        #endregion DeviceQueries

        #region SibiQueries

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestItemsCols.TableName"/> INNER JOIN <see cref="SibiRequestCols.TableName"/> ON <see cref="SibiRequestItemsCols.RequestUID"/> = <see cref="SibiRequestCols.UID"/> WHERE <see cref="SibiRequestItemsCols.ItemUID"/> = <paramref name="itemGUID"/>
        /// </summary>
        /// <param name="itemGUID"></param>
        /// <returns></returns>
        public static string SelectSibiRequestAndItemByItemGUID(string itemGUID)
        {
            return "SELECT * FROM " + SibiRequestItemsCols.TableName + " INNER JOIN " + SibiRequestCols.TableName + " ON " + SibiRequestItemsCols.RequestUID + " = " + SibiRequestCols.UID + " WHERE " + SibiRequestItemsCols.ItemUID + "='" + itemGUID + "'";
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
        /// SELECT * FROM <see cref="SibiRequestCols.TableName"/> WHERE <see cref="SibiRequestCols.UID"/> = <see cref="requestGUID"/>
        /// </summary>
        /// <param name="requestGUID"></param>
        /// <returns></returns>
        public static string SelectSibiRequestsByGUID(string requestGUID)
        {
            return "SELECT * FROM " + SibiRequestCols.TableName + " WHERE " + SibiRequestCols.UID + "='" + requestGUID + "'";
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
        /// SELECT <paramref name="columnsString"/> FROM <see cref="SibiRequestItemsCols.TableName"/> WHERE <see cref="SibiRequestItemsCols.RequestUID"/> = <paramref name="requestGUID"/> ORDER BY <see cref="SibiRequestItemsCols.Timestamp"/>
        /// </summary>
        /// <param name="columnsString"></param>
        /// <param name="requestGUID"></param>
        /// <returns></returns>
        public static string SelectSibiRequestItems(string columnsString, string requestGUID)
        {
            return "SELECT " + columnsString + " FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.RequestUID + "='" + requestGUID + "' ORDER BY " + SibiRequestItemsCols.Timestamp;
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiNotesCols.TableName"/> WHERE <see cref="SibiNotesCols.RequestUID"/> = <paramref name="requestGUID"/> ORDER BY <see cref="SibiNotesCols.DateStamp"/> DESC
        /// </summary>
        /// <param name="requestGUID"></param>
        /// <returns></returns>
        public static string SelectSibiNotes(string requestGUID)
        {
            return "SELECT * FROM " + SibiNotesCols.TableName + " WHERE " + SibiNotesCols.RequestUID + "='" + requestGUID + "' ORDER BY " + SibiNotesCols.DateStamp + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiNotesCols.TableName"/> WHERE <see cref="SibiNotesCols.NoteUID"/> = <paramref name="noteGUID"/>
        /// </summary>
        /// <param name="noteGUID"></param>
        /// <returns></returns>
        public static string SelectNoteByGuid(string noteGUID)
        {
            return "SELECT * FROM " + SibiNotesCols.TableName + " WHERE " + SibiNotesCols.NoteUID + "='" + noteGUID + "'";
        }

        /// <summary>
        /// DELETE FROM <see cref="SibiNotesCols.TableName"/> WHERE <see cref="SibiNotesCols.NoteUID"/> = <paramref name="noteGUID"/>
        /// </summary>
        /// <param name="noteGUID"></param>
        /// <returns></returns>
        public static string DeleteSibiNote(string noteGUID)
        {
            return "DELETE FROM " + SibiNotesCols.TableName + " WHERE " + SibiNotesCols.NoteUID + "='" + noteGUID + "'";
        }

        /// <summary>
        /// DELETE FROM <see cref="SibiRequestCols.TableName"/> WHERE <see cref="SibiRequestCols.UID"/> = <paramref name="requestGUID"/>
        /// </summary>
        /// <param name="requestGUID"></param>
        /// <returns></returns>
        public static string DeleteSibiRequestByGUID(string requestGUID)
        {
            return "DELETE FROM " + SibiRequestCols.TableName + " WHERE " + SibiRequestCols.UID + "='" + requestGUID + "'";
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