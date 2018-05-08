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
            if (data.Rows.Count > 0)
            {
                SetupGrid(grid, columns);
                using (data)
                {
                    grid.DataSource = BuildDataSource(data, columns, forceRawData);
                }
            }
            else
            {
                grid.DataSource = null;
                grid.Columns.Clear();
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
                    DataRow newRow = newTable.NewRow();

                    foreach (GridColumnAttrib col in columns)
                    {
                        switch (col.ColumnFormatType)
                        {
                            case ColumnFormatType.DefaultFormat:
                            case ColumnFormatType.AttributeCombo:
                                newRow[col.ColumnName] = row[col.ColumnName];

                                break;

                            case ColumnFormatType.AttributeDisplayMemberOnly:
                                newRow[col.ColumnName] = col.Attributes[row[col.ColumnName].ToString()].DisplayValue;

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
            bool rebuildRequired = false;
            foreach (GridColumnAttrib col in columns)
            {
                switch (col.ColumnFormatType)
                {
                    case ColumnFormatType.AttributeDisplayMemberOnly:
                    case ColumnFormatType.NotePreview:
                    case ColumnFormatType.FileSize:
                    case ColumnFormatType.Image:
                        rebuildRequired = true;
                        break;
                }
            }
            return rebuildRequired;
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
                    return DataGridComboColumn(column.Attributes, column.ColumnCaption, column.ColumnName);

                case ColumnFormatType.Image:
                    return DataGridImageColumn(column);
            }
            return null;
        }

        private static DataGridViewColumn DataGridImageColumn(GridColumnAttrib column)
        {
            var newCol = new DataGridViewImageColumn();
            newCol.Name = column.ColumnName;
            newCol.DataPropertyName = column.ColumnName;
            newCol.HeaderText = column.ColumnCaption;
            newCol.ValueType = column.ColumnType;
            newCol.CellTemplate = new DataGridViewImageCell();
            newCol.SortMode = DataGridViewColumnSortMode.Automatic;
            newCol.ReadOnly = column.ColumnReadOnly;
            newCol.Visible = column.ColumnVisible;
            newCol.Width = 40;
            return newCol;
        }

        private static DataGridViewColumn GenericColumn(GridColumnAttrib column)
        {
            var newCol = new DataGridViewColumn();
            newCol.Name = column.ColumnName;
            newCol.DataPropertyName = column.ColumnName;
            newCol.HeaderText = column.ColumnCaption;
            newCol.ValueType = column.ColumnType;
            newCol.CellTemplate = new DataGridViewTextBoxCell();
            newCol.SortMode = DataGridViewColumnSortMode.Automatic;
            newCol.ReadOnly = column.ColumnReadOnly;
            newCol.Visible = column.ColumnVisible;
            return newCol;
        }

        private static DataGridViewComboBoxColumn DataGridComboColumn(DbAttributes attributes, string headerText, string name)
        {
            var newCombo = new DataGridViewComboBoxColumn();
            newCombo.Items.Clear();
            newCombo.HeaderText = headerText;
            newCombo.DataPropertyName = name;
            newCombo.Name = name;
            newCombo.Width = 200;
            newCombo.SortMode = DataGridViewColumnSortMode.Automatic;
            newCombo.DisplayMember = nameof(DbAttribute.DisplayValue);
            newCombo.ValueMember = nameof(DbAttribute.Code);
            newCombo.DataSource = attributes.GetArray();
            return newCombo;
        }



    }
}