using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace AssetManager.Helpers
{
    public static class GridPopulation
    {
        public static void PopulateGrid(DataGridView grid, DataTable data, List<GridColumnAttrib> columns, bool forceRawData)
        {
            SetupGrid(grid, columns);
            using (data)
            {
                grid.DataSource = null;
                grid.DataSource = BuildDataSource(data, columns, forceRawData);
            }
        }
        
        private static void SetupGrid(DataGridView grid, List<GridColumnAttrib> columns)
        {
            grid.DataSource = null;
            grid.Rows.Clear();
            grid.Columns.Clear();
            grid.AutoGenerateColumns = false;
            foreach (GridColumnAttrib col in columns)
            {
                grid.Columns.Add(GetColumn(col));
            }
        }

        private static DataTable BuildDataSource(DataTable data, List<GridColumnAttrib> columns, bool forceRawData)
        {
            var needsRebuilt = ColumnsRequireRebuild(columns);
            if (needsRebuilt & !forceRawData)
            {
                DataTable NewTable = new DataTable();
                foreach (GridColumnAttrib col in columns)
                {
                    NewTable.Columns.Add(col.ColumnName, col.ColumnType);
                }
                foreach (DataRow row in data.Rows)
                {
                    DataRow NewRow = null;
                    NewRow = NewTable.NewRow();

                    foreach (GridColumnAttrib col in columns)
                    {
                        switch (col.ColumnFormatType)
                        {
                            case ColumnFormatType.DefaultFormat:
                            case ColumnFormatType.AttributeCombo:
                                NewRow[col.ColumnName] = row[col.ColumnName];

                                break;

                            case ColumnFormatType.AttributeDisplayMemberOnly:
                                NewRow[col.ColumnName] = AttributeFunctions.GetDisplayValueFromCode(col.AttributeIndex, row[col.ColumnName].ToString());

                                break;

                            case ColumnFormatType.NotePreview:
                                var NoteText = OtherFunctions.RTFToPlainText(row[col.ColumnName].ToString());
                                NewRow[col.ColumnName] = OtherFunctions.NotePreview(NoteText);

                                break;

                            case ColumnFormatType.FileSize:
                                string HumanFileSize = Math.Round((Convert.ToInt32(row[col.ColumnName]) / 1024d), 1) + " KB";
                                NewRow[col.ColumnName] = HumanFileSize;

                                break;

                            case ColumnFormatType.Image:
                                NewRow[col.ColumnName] = FileIcon.GetFileIcon(row[col.ColumnName].ToString());

                                break;
                        }
                    }
                    NewTable.Rows.Add(NewRow);
                }
                return NewTable;
            }
            else
            {
                return data;
            }
        }

        private static bool ColumnsRequireRebuild(List<GridColumnAttrib> columns)
        {
            bool RebuildRequired = false;
            foreach (GridColumnAttrib col in columns)
            {
                switch (col.ColumnFormatType)
                {
                    case ColumnFormatType.AttributeDisplayMemberOnly:
                    case ColumnFormatType.NotePreview:
                    case ColumnFormatType.FileSize:
                    case ColumnFormatType.Image:
                        RebuildRequired = true;
                        break;
                }
            }
            return RebuildRequired;
        }

        private static DataGridViewColumn GetColumn(GridColumnAttrib column)
        {
            switch (column.ColumnFormatType)
            {
                case ColumnFormatType.DefaultFormat:
                case ColumnFormatType.AttributeDisplayMemberOnly:
                case ColumnFormatType.NotePreview:
                case ColumnFormatType.FileSize:
                    return GenericColumn(column);

                case ColumnFormatType.AttributeCombo:
                    return DataGridComboColumn(column.AttributeIndex, column.ColumnCaption, column.ColumnName);

                case ColumnFormatType.Image:
                    return DataGridImageColumn(column);
            }
            return null;
        }

        private static DataGridViewColumn DataGridImageColumn(GridColumnAttrib column)
        {
            DataGridViewImageColumn NewCol = new DataGridViewImageColumn();
            NewCol.Name = column.ColumnName;
            NewCol.DataPropertyName = column.ColumnName;
            NewCol.HeaderText = column.ColumnCaption;
            NewCol.ValueType = column.ColumnType;
            NewCol.CellTemplate = new DataGridViewImageCell();
            NewCol.SortMode = DataGridViewColumnSortMode.Automatic;
            NewCol.ReadOnly = column.ColumnReadOnly;
            NewCol.Visible = column.ColumnVisible;
            NewCol.Width = 40;
            return NewCol;
        }

        private static DataGridViewColumn GenericColumn(GridColumnAttrib column)
        {
            DataGridViewColumn NewCol = new DataGridViewColumn();
            NewCol.Name = column.ColumnName;
            NewCol.DataPropertyName = column.ColumnName;
            NewCol.HeaderText = column.ColumnCaption;
            NewCol.ValueType = column.ColumnType;
            NewCol.CellTemplate = new DataGridViewTextBoxCell();
            NewCol.SortMode = DataGridViewColumnSortMode.Automatic;
            NewCol.ReadOnly = column.ColumnReadOnly;
            NewCol.Visible = column.ColumnVisible;
            return NewCol;
        }

        private static DataGridViewComboBoxColumn DataGridComboColumn(CodeAttribute[] indexType, string headerText, string name)
        {
            DataGridViewComboBoxColumn NewCombo = new DataGridViewComboBoxColumn();
            NewCombo.Items.Clear();
            NewCombo.HeaderText = headerText;
            NewCombo.DataPropertyName = name;
            NewCombo.Name = name;
            NewCombo.Width = 200;
            NewCombo.SortMode = DataGridViewColumnSortMode.Automatic;
            NewCombo.DisplayMember = nameof(CodeAttribute.DisplayValue);
            NewCombo.ValueMember = nameof(CodeAttribute.Code);
            NewCombo.DataSource = indexType;
            return NewCombo;
        }



    }
}