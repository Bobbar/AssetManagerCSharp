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
                DataTable newTable = new DataTable();

                // Add columns to the new table.
                foreach (GridColumnAttrib col in columns)
                {
                    if (col.ColumnType != null)
                    {
                        newTable.Columns.Add(col.ColumnName, col.ColumnType);
                    }
                    else
                    {
                        newTable.Columns.Add(col.ColumnName, data.Columns[col.ColumnName].DataType);
                    }
                }

                foreach (DataRow row in data.Rows)
                {
                    DataRow newRow = null;
                    newRow = newTable.NewRow();

                    foreach (GridColumnAttrib col in columns)
                    {
                        switch (col.ColumnFormatType)
                        {
                            case ColumnFormatType.DefaultFormat:
                            case ColumnFormatType.AttributeCombo:
                                newRow[col.ColumnName] = row[col.ColumnName];

                                break;

                            case ColumnFormatType.AttributeDisplayMemberOnly:
                                newRow[col.ColumnName] = AttributeFunctions.GetDisplayValueFromCode(col.AttributeIndex, row[col.ColumnName].ToString());

                                break;

                            case ColumnFormatType.NotePreview:
                                var noteText = OtherFunctions.RTFToPlainText(row[col.ColumnName].ToString());
                                newRow[col.ColumnName] = OtherFunctions.NotePreview(noteText);

                                break;

                            case ColumnFormatType.FileSize:
                                string humanFileSize = Math.Round((Convert.ToInt32(row[col.ColumnName]) / 1024d), 1) + " KB";
                                newRow[col.ColumnName] = humanFileSize;

                                break;

                            case ColumnFormatType.Image:
                                newRow[col.ColumnName] = FileIcon.GetFileIcon(row[col.ColumnName].ToString());

                                break;
                        }
                    }
                    newTable.Rows.Add(newRow);
                }
                return newTable;
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

        private static DataGridViewComboBoxColumn DataGridComboColumn(DBCode[] indexType, string headerText, string name)
        {
            DataGridViewComboBoxColumn NewCombo = new DataGridViewComboBoxColumn();
            NewCombo.Items.Clear();
            NewCombo.HeaderText = headerText;
            NewCombo.DataPropertyName = name;
            NewCombo.Name = name;
            NewCombo.Width = 200;
            NewCombo.SortMode = DataGridViewColumnSortMode.Automatic;
            NewCombo.DisplayMember = nameof(DBCode.DisplayValue);
            NewCombo.ValueMember = nameof(DBCode.Code);
            NewCombo.DataSource = indexType;
            return NewCombo;
        }



    }
}