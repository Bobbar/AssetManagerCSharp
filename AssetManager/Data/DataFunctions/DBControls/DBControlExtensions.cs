using AssetManager.Data.Classes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace AssetManager.Data.Functions
{
    public static class DBControlExtensions
    {
        private static Dictionary<string, int> ColumnLengths = new Dictionary<string, int>();

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
                        if (!ColumnLengths.ContainsKey(row["COLUMN_NAME"].ToString()))
                        {
                            ColumnLengths.Add(row["COLUMN_NAME"].ToString(), Convert.ToInt32(row["CHARACTER_MAXIMUM_LENGTH"]));
                        }
                    }
                }
            }
        }

        public static void SetDBInfo(this Control control, string dataColumn, bool required = false)
        {
            SetControlMaxLength(control, dataColumn);
            control.Tag = new DBControlInfo(dataColumn, required);
        }

        public static void SetDBInfo(this Control control, string dataColumn, ParseType parseType, bool required = false)
        {
            SetControlMaxLength(control, dataColumn);
            control.Tag = new DBControlInfo(dataColumn, parseType, required);
        }

        public static void SetDBInfo(this Control control, string dataColumn, DBCode[] attribIndex, bool required = false)
        {
            SetControlMaxLength(control, dataColumn);
            control.Tag = new DBControlInfo(dataColumn, attribIndex, required);
        }

        public static void SetDBInfo(this Control control, string dataColumn, DBCode[] attribIndex, ParseType parseType, bool required = false)
        {
            SetControlMaxLength(control, dataColumn);
            control.Tag = new DBControlInfo(dataColumn, attribIndex, parseType, required);
        }

        private static void SetControlMaxLength(Control control, string dataColumn)
        {
            if (ColumnLengths.ContainsKey(dataColumn))
            {
                if (control is TextBox)
                {
                    var txt = (TextBox)control;
                    txt.MaxLength = ColumnLengths[dataColumn];
                }
            }
        }
    }
}
