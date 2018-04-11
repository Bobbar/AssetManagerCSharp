using System;
using System.Drawing;
using System.Collections.Generic;
using AssetManager.Data.Classes;

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

        public GridColumnAttrib(string colName, string caption, ColumnFormatType displayMode)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            if (displayMode == ColumnFormatType.Image)
            {
                ColumnType = typeof(Image);
            }
            else
            {
                ColumnType = typeof(string);
            }
            ColumnReadOnly = false;
            ColumnVisible = true;
            Attributes = null;
            ColumnFormatType = displayMode;
        }

        public GridColumnAttrib(string colName, string caption, DbAttributes attribs, ColumnFormatType displayMode)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            ColumnType = typeof(string);
            ColumnReadOnly = false;
            ColumnVisible = true;
            this.Attributes = attribs;
            ColumnFormatType = displayMode;
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
}

namespace AssetManager.Helpers
{
    public enum ColumnFormatType
    {
        DefaultFormat,
        AttributeCombo,
        AttributeDisplayMemberOnly,
        NotePreview,
        Image,
        FileSize
    }
}

namespace AssetManager.Helpers
{
    public struct StatusColumnColor
    {
        public string StatusID;

        public Color StatusColor;

        public StatusColumnColor(string id, Color color)
        {
            StatusID = id;
            StatusColor = color;
        }
    }
}

namespace AssetManager.Helpers
{
    public static class GridColumnFunctions
    {
        /// <summary>
        /// Returns a comma separated string containing the DB columns within a List(Of ColumnStruct). For use in queries.
        /// </summary>
        /// <param name="columns"></param>
        /// <returns></returns>
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
