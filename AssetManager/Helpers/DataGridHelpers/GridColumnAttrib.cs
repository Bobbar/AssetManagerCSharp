using System;
using System.Drawing;
using System.Collections.Generic;

namespace AssetManager
{
    public class GridColumnAttrib
    {
        public string ColumnName { get; set; }
        public string ColumnCaption { get; set; }
        public Type ColumnType { get; set; }
        public bool ColumnReadOnly { get; set; }
        public bool ColumnVisible { get; set; }
        public AttributeDataStruct[] AttributeIndex { get; set; }
        public ColumnFormatTypes ColumnFormatType { get; set; }

        public GridColumnAttrib(string colName, string caption, Type type)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            ColumnType = type;
            ColumnReadOnly = false;
            ColumnVisible = true;
            AttributeIndex = null;
            ColumnFormatType = ColumnFormatTypes.DefaultFormat;
        }

        public GridColumnAttrib(string colName, string caption, Type type, ColumnFormatTypes displayMode)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            ColumnType = type;
            ColumnReadOnly = false;
            ColumnVisible = true;
            AttributeIndex = null;
            ColumnFormatType = displayMode;
        }

        public GridColumnAttrib(string colName, string caption, AttributeDataStruct[] attribIndex)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            ColumnType = typeof(string);
            ColumnReadOnly = false;
            ColumnVisible = true;
            this.AttributeIndex = attribIndex;
            ColumnFormatType = ColumnFormatTypes.AttributeCombo;
        }

        public GridColumnAttrib(string colName, string caption, AttributeDataStruct[] attribIndex, ColumnFormatTypes displayMode)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            ColumnType = typeof(string);
            ColumnReadOnly = false;
            ColumnVisible = true;
            this.AttributeIndex = attribIndex;
            ColumnFormatType = displayMode;
        }

        public GridColumnAttrib(string colName, string caption, Type type, bool isReadOnly, bool visible)
        {
            ColumnName = colName;
            ColumnCaption = caption;
            ColumnType = type;
            ColumnReadOnly = isReadOnly;
            ColumnVisible = visible;
            AttributeIndex = null;
            ColumnFormatType = ColumnFormatTypes.DefaultFormat;
        }
    }
}

namespace AssetManager
{
    public enum ColumnFormatTypes
    {
        DefaultFormat,
        AttributeCombo,
        AttributeDisplayMemberOnly,
        NotePreview,
        Image,
        FileSize
    }
}

namespace AssetManager
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

namespace AssetManager
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
