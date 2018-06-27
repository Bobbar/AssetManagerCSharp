using AssetManager.Data.Classes;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace AssetManager.Helpers
{
    public class GridColumnAttrib
    {
        public string ColumnName { get; set; }
        public string ColumnCaption { get; set; }
        public Type ColumnType { get; set; }
        public bool ColumnReadOnly { get; set; }
        public bool ColumnVisible { get; set; }
        public DbAttributes Attributes { get; set; }
        public ColumnFormatType ColumnFormatType { get; set; }

        public GridColumnAttrib(string colName, string caption)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            ColumnType = null;
            ColumnReadOnly = false;
            ColumnVisible = true;
            Attributes = null;
            ColumnFormatType = ColumnFormatType.DefaultFormat;
        }

        public GridColumnAttrib(string colName, string caption, ColumnFormatType format)
        {
            ColumnName = colName;
            ColumnCaption = caption;

            if (format == ColumnFormatType.Image)
            {
                ColumnType = typeof(Image);
            }
            else if (format == ColumnFormatType.FileSize)
            {
                ColumnType = typeof(double);
            }
            else
            {
                ColumnType = typeof(string);
            }

            ColumnReadOnly = false;
            ColumnVisible = true;
            Attributes = null;
            ColumnFormatType = format;
        }

        public GridColumnAttrib(string colName, string caption, DbAttributes attribs, ColumnFormatType format)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            ColumnType = typeof(string);
            ColumnReadOnly = false;
            ColumnVisible = true;
            this.Attributes = attribs;
            ColumnFormatType = format;
        }

        public GridColumnAttrib(string colName, string caption, bool isReadOnly, bool visible)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            ColumnType = null;
            ColumnReadOnly = isReadOnly;
            ColumnVisible = visible;
            Attributes = null;
            ColumnFormatType = ColumnFormatType.DefaultFormat;
        }
    }

    public static class GridColumnFunctions
    {
        /// <summary>
        /// Returns a comma separated string containing the DB columns within a List of <see cref="GridColumnAttrib"/>. For use in queries.
        /// </summary>
        /// <param name="columns"></param>
        public static string ColumnsString(List<GridColumnAttrib> columns)
        {
            string colString = "";
            foreach (GridColumnAttrib column in columns)
            {
                colString += column.ColumnName;
                if (columns.IndexOf(column) != columns.Count - 1) colString += ",";
            }
            return colString;
        }
    }
}