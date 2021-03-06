﻿using System;

namespace AssetManager.Data.Communications
{
    public static class Queries
    {
        #region DeviceQueries

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="HistoricalDevicesCols.HistoryEntryGuid"/> = <paramref name="entryGuid"/>
        /// </summary>
        /// <param name="entryGuid"></param>
        /// <returns></returns>
        public static string SelectHistoricalDeviceEntry(string entryGuid)
        {
            return "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.HistoryEntryGuid + "='" + entryGuid + "'";
        }

        /// <summary>
        /// SELECT * FROM <see cref="DevicesCols.TableName"/> WHERE <see cref="DevicesCols.TableName"/> = <paramref name="deviceGuid"/>
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <returns></returns>
        public static string SelectDeviceByGuid(string deviceGuid)
        {
            return "SELECT * FROM " + DevicesCols.TableName + " WHERE " + DevicesCols.DeviceGuid + " = '" + deviceGuid + "'";
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
        /// SELECT * FROM <see cref="TrackablesCols.TableName"/> WHERE <see cref="TrackablesCols.DeviceGuid"/> = <paramref name="deviceGuid"/> ORDER BY <see cref="TrackablesCols.DateStamp"/> DESC
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <returns></returns>
        public static string SelectTrackingByDevGuid(string deviceGuid)
        {
            return "SELECT * FROM " + TrackablesCols.TableName + " WHERE " + TrackablesCols.DeviceGuid + " = '" + deviceGuid + "' ORDER BY " + TrackablesCols.DateStamp + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="TrackablesCols.TableName"/> WHERE <see cref="TrackablesCols.Guid"/> = <paramref name="entryGuid"/>
        /// </summary>
        /// <param name="entryGuid"></param>
        /// <returns></returns>
        public static string SelectTrackingEntryByGuid(string entryGuid)
        {
            return "SELECT * FROM " + TrackablesCols.TableName + " WHERE  " + TrackablesCols.Guid + " = '" + entryGuid + "'";
        }

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="DevicesBaseCols.DeviceGuid"/> = <paramref name="deviceGuid"/> ORDER BY <see cref=" HistoricalDevicesCols.ActionDateTime"/> DESC
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <returns></returns>
        public static string SelectDeviceHistoricalTable(string deviceGuid)
        {
            return "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.DeviceGuid + " = '" + deviceGuid + "' ORDER BY " + HistoricalDevicesCols.ActionDateTime + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="DevicesBaseCols.DeviceGuid"/> = <paramref name="deviceGuid"/> AND <see cref="HistoricalDevicesCols.ActionDateTime"/> "LESS THAN" <paramref name="startDate"/> ORDER BY <see cref="HistoricalDevicesCols.ActionDateTime"/> DESC
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <param name="startDate"></param>
        /// <returns></returns>
        public static string SelectDevHistoricalEntriesOlderThan(string deviceGuid, DateTime startDate)
        {
            return "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.DeviceGuid + " = '" + deviceGuid + "' AND " + HistoricalDevicesCols.ActionDateTime + " < '" + startDate.ToString(DataConsistency.DBDateTimeFormat) + "' ORDER BY " + HistoricalDevicesCols.ActionDateTime + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="DevicesCols.TableName "/> WHERE
        /// </summary>
        public static string SelectDevicesPartial { get; } = "SELECT * FROM " + DevicesCols.TableName + " WHERE ";

        /// <summary>
        /// "DELETE FROM <see cref="DevicesCols.TableName"/> WHERE <see cref="DevicesBaseCols.DeviceGuid"/> = <paramref name="deviceGuid"/>
        /// </summary>
        /// <param name="deviceGuid"></param>
        /// <returns></returns>
        public static string DeleteDeviceByGuid(string deviceGuid)
        {
            return "DELETE FROM " + DevicesCols.TableName + " WHERE " + DevicesCols.DeviceGuid + "='" + deviceGuid + "'";
        }

        /// <summary>
        /// "DELETE FROM <see cref="HistoricalDevicesCols.TableName"/> WHERE <see cref="HistoricalDevicesCols.HistoryEntryGuid"/> = <paramref name="entryGuid"/>
        /// </summary>
        /// <param name="entryGuid"></param>
        /// <returns></returns>
        public static string DeleteHistoricalEntryByGuid(string entryGuid)
        {
            return "DELETE FROM " + HistoricalDevicesCols.TableName + " WHERE " + HistoricalDevicesCols.HistoryEntryGuid + "='" + entryGuid + "'";
        }

        #endregion DeviceQueries

        #region SibiQueries

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestItemsCols.TableName"/> INNER JOIN <see cref="SibiRequestCols.TableName"/> ON <see cref="SibiRequestItemsCols.RequestGuid"/> = <see cref="SibiRequestCols.Guid"/> WHERE <see cref="SibiRequestItemsCols.ItemGuid"/> = <paramref name="itemGuid"/>
        /// </summary>
        /// <param name="itemGuid"></param>
        /// <returns></returns>
        public static string SelectSibiRequestAndItemByItemGuid(string itemGuid)
        {
            return "SELECT * FROM " + SibiRequestItemsCols.TableName + " INNER JOIN " + SibiRequestCols.TableName + " ON " + SibiRequestItemsCols.RequestGuid + " = " + SibiRequestCols.Guid + " WHERE " + SibiRequestItemsCols.ItemGuid + "='" + itemGuid + "'";
        }

        /// <summary>
        /// SELECT DISTINCT <see cref="SibiRequestCols.CreateDate"/> FROM <see cref="SibiRequestCols.TableName"/> ORDER BY <see cref="SibiRequestCols.CreateDate"/> DESC
        /// </summary>
        public static string SelectSibiDisplayYears { get; } = "SELECT DISTINCT " + SibiRequestCols.CreateDate + " FROM " + SibiRequestCols.TableName + " ORDER BY " + SibiRequestCols.CreateDate + " DESC";

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestCols.TableName"/> FROM <see cref="SibiRequestCols.TableName"/> ORDER BY <see cref="SibiRequestCols.RequestNumber"/> DESC
        /// </summary>
        public static string SelectSibiRequestsTable { get; } = "SELECT * FROM " + SibiRequestCols.TableName + " ORDER BY " + SibiRequestCols.RequestNumber + " DESC";

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestCols.TableName"/> WHERE <see cref="SibiRequestCols.CreateDate"/> LIKE '% <paramref name="year"/> %' ORDER BY <see cref="SibiRequestCols.RequestNumber"/> DESC
        /// </summary>
        /// <param name="year"></param>
        /// <returns></returns>
        public static string SelectSibiRequestsByYear(string year)
        {
            return "SELECT * FROM " + SibiRequestCols.TableName + " WHERE " + SibiRequestCols.CreateDate + " LIKE '%" + year + "%' ORDER BY " + SibiRequestCols.RequestNumber + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestCols.TableName"/> WHERE <see cref="SibiRequestCols.Guid"/> = <paramref name="requestGuid"/>
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <returns></returns>
        public static string SelectSibiRequestsByGuid(string requestGuid)
        {
            return "SELECT * FROM " + SibiRequestCols.TableName + " WHERE " + SibiRequestCols.Guid + "='" + requestGuid + "'";
        }

        /// <summary>
        /// SELECT <paramref name="columnsValue"/> FROM <see cref="SibiRequestItemsCols.TableName"/> LIMIT 0
        /// </summary>
        /// <param name="columnsValue"></param>
        /// <returns></returns>
        public static string SelectEmptySibiItemsTable(string columnsValue)
        {
            return "SELECT " + columnsValue + " FROM " + SibiRequestItemsCols.TableName + " LIMIT 0";
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiRequestCols.TableName"/> LIMIT 0
        /// </summary>
        public static string SelectEmptySibiRequestTable { get; } = "SELECT * FROM " + SibiRequestCols.TableName + " LIMIT 0";

        /// <summary>
        /// SELECT <paramref name="columnsValue"/> FROM <see cref="SibiRequestItemsCols.TableName"/> WHERE <see cref="SibiRequestItemsCols.RequestGuid"/> = <paramref name="requestGuid"/> ORDER BY <see cref="SibiRequestItemsCols.Timestamp"/>
        /// </summary>
        /// <param name="columnsValue"></param>
        /// <param name="requestGuid"></param>
        /// <returns></returns>
        public static string SelectSibiRequestItems(string columnsValue, string requestGuid)
        {
            return "SELECT " + columnsValue + " FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.RequestGuid + "='" + requestGuid + "' ORDER BY " + SibiRequestItemsCols.Timestamp;
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiNotesCols.TableName"/> WHERE <see cref="SibiNotesCols.RequestGuid"/> = <paramref name="requestGuid"/> ORDER BY <see cref="SibiNotesCols.DateStamp"/> DESC
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <returns></returns>
        public static string SelectSibiNotes(string requestGuid)
        {
            return "SELECT * FROM " + SibiNotesCols.TableName + " WHERE " + SibiNotesCols.RequestGuid + "='" + requestGuid + "' ORDER BY " + SibiNotesCols.DateStamp + " DESC";
        }

        /// <summary>
        /// SELECT * FROM <see cref="SibiNotesCols.TableName"/> WHERE <see cref="SibiNotesCols.NoteGuid"/> = <paramref name="noteGuid"/>
        /// </summary>
        /// <param name="noteGuid"></param>
        /// <returns></returns>
        public static string SelectNoteByGuid(string noteGuid)
        {
            return "SELECT * FROM " + SibiNotesCols.TableName + " WHERE " + SibiNotesCols.NoteGuid + "='" + noteGuid + "'";
        }

        /// <summary>
        /// DELETE FROM <see cref="SibiNotesCols.TableName"/> WHERE <see cref="SibiNotesCols.NoteGuid"/> = <paramref name="noteGuid"/>
        /// </summary>
        /// <param name="noteGuid"></param>
        /// <returns></returns>
        public static string DeleteSibiNote(string noteGuid)
        {
            return "DELETE FROM " + SibiNotesCols.TableName + " WHERE " + SibiNotesCols.NoteGuid + "='" + noteGuid + "'";
        }

        /// <summary>
        /// DELETE FROM <see cref="SibiRequestCols.TableName"/> WHERE <see cref="SibiRequestCols.Guid"/> = <paramref name="requestGuid"/>
        /// </summary>
        /// <param name="requestGuid"></param>
        /// <returns></returns>
        public static string DeleteSibiRequestByGuid(string requestGuid)
        {
            return "DELETE FROM " + SibiRequestCols.TableName + " WHERE " + SibiRequestCols.Guid + "='" + requestGuid + "'";
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
        /// SELECT * FROM <see cref="UsersCols.TableName"/> WHERE <see cref="UsersCols.UserName"/> = <paramref name="userName"/>
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public static string SelectUserByName(string userName)
        {
            return "SELECT * FROM " + UsersCols.TableName + " WHERE " + UsersCols.UserName + "='" + userName + "' LIMIT 1";
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