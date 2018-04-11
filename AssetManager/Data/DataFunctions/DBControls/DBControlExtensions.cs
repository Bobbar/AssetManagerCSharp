using AssetManager.Data.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace AssetManager.Data.Functions
{
    public static class DBControlExtensions
    {
        private static Dictionary<string, int> columnLengths = new Dictionary<string, int>();

        public static void GetFieldLengths()
        {
            if (GlobalSwitches.CachedMode) return;

            string query = "SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '" + ServerInfo.CurrentDataBase.ToString() + "'";

            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(query))
            {
                foreach (DataRow row in results.Rows)
                {
                    if (!(row["CHARACTER_MAXIMUM_LENGTH"] is DBNull))
                    {
                        if (!columnLengths.ContainsKey(row["COLUMN_NAME"].ToString()))
                        {
                            columnLengths.Add(row["COLUMN_NAME"].ToString(), Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]));
                        }
                    }
                }
            }
        }

        public static void SetDBInfo(this Control control, string columnName)
        {
            SetDBInfo(control, columnName, null, ParseType.DisplayOnly, false);
        }

        public static void SetDBInfo(this Control control, string columnName, bool required)
        {
            SetDBInfo(control, columnName, null, ParseType.UpdateAndDisplay, required);
        }

        public static void SetDBInfo(this Control control, string columnName, ParseType parseType, bool required)
        {
            SetDBInfo(control, columnName, null, parseType, required);
        }

        public static void SetDBInfo(this Control control, string columnName, DbAttributes attribs, bool required = false)
        {
            SetDBInfo(control, columnName, attribs, ParseType.UpdateAndDisplay, required);
        }

        public static void SetDBInfo(this Control control, string columnName, DbAttributes attribs, ParseType parseType, bool required = false)
        {
            SetControlMaxLength(control, columnName);
            control.Tag = new DBControlInfo(columnName, attribs, parseType, required);
        }

        private static void SetControlMaxLength(Control control, string dataColumn)
        {
            if (columnLengths.ContainsKey(dataColumn))
            {
                if (control is TextBox)
                {
                    var txt = (TextBox)control;
                    txt.MaxLength = columnLengths[dataColumn];
                }
            }
        }
    }
}