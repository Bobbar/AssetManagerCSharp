using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using System.Drawing;
using AssetManager.UserInterface.Forms;
using AssetManager.UserInterface.CustomControls;
using AssetManager.Data.Classes;

namespace AssetManager.Helpers
{
    public static class DataGridViewExtensions
    {

        /// <summary>
        /// Copies current selection to clipboard.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="includeHeaders">False to exclude column headers. Default: True. </param>
        public static void CopyToClipboard(this DataGridView grid, bool includeHeaders = true)
        {
            var originalCopyMode = grid.ClipboardCopyMode;

            if (includeHeaders)
            {
                grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            }
            else
            {
                grid.ClipboardCopyMode = DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            }

            Clipboard.SetDataObject(grid.GetClipboardContent());
            grid.ClipboardCopyMode = originalCopyMode;
        }

        /// <summary>
        /// Sends a copy of this grid to a new <see cref="GridForm"/> instance.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="parentForm"></param>
        public static void CopyToGridForm(this DataGridView grid, ExtendedForm parentForm, DoubleClickAction type = DoubleClickAction.ViewOnly)
        {
            GridForm NewGridForm = new GridForm(parentForm, grid.Name + " Copy");
            NewGridForm.AddGrid(grid.Name, grid.Name, type, ((DataTable)grid.DataSource).Copy());
            NewGridForm.Show();
        }

        /// <summary>
        /// Returns the object value of the cell in the current row at the specified column.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static object CurrentRowValue(this DataGridView grid, string columnName)
        {
            if (grid.CurrentRow != null)
            {
                var cellValue = grid.CurrentRow.Cells[columnName].Value;
                if (cellValue != null)
                {
                    return cellValue;
                }
            }
            return null;
        }

        /// <summary>
        /// Returns the string value of the cell in the current row at the specified column.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string CurrentRowStringValue(this DataGridView grid, string columnName)
        {
            var cellValue = CurrentRowValue(grid, columnName);
            if (cellValue != null)
            {
                return cellValue.ToString();
            }
            return string.Empty;
        }

        /// <summary>
        /// Returns the index for the specified column name.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static int ColumnIndex(this DataGridView grid, string columnName)
        {
            try
            {
                return grid.Columns[columnName].Index;
            }
            catch
            {
                return -1;
            }
        }

        /// <summary>
        /// Populates this grid from a <see cref="DataTable"/> and <see cref="List{T}"/> of <see cref="GridColumnAttrib"/>.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="data"></param>
        /// <param name="columns">List of <see cref="GridColumnAttrib"/> used to parse column attributes for type, visibility, caption, etc.</param>
        /// <param name="forceRawData">True if data will not be recreated into a new datatable that conforms to the columns property. Used for grids that should be directly bound to a datatable. Default: false.</param>
        public static void Populate(this DataGridView grid, DataTable data, List<GridColumnAttrib> columns, bool forceRawData = false)
        {
            GridPopulation.PopulateGrid(grid, data, columns, forceRawData);
        }

        /// <summary>
        /// Provides much faster column resizing than the built-in AutoResizeColumns method.
        /// </summary>
        /// <param name="targetGrid"></param>
        public static void FastAutoSizeColumns(this DataGridView targetGrid)
        {
            // Padding added to final column size
            const int widthPadding = 20;

            // Cast out a DataTable from the target grid datasource.
            // We need to iterate through all the data in the grid,
            // and a DataTable supports enumeration which will allow us
            // to access the data much more quickly than directly accessing
            // the grid control.
            var gridTable = (DataTable)targetGrid.DataSource;

            // Create a graphics object from the target grid. Used for measuring text size.
            using (var gfx = targetGrid.CreateGraphics())
            {
                // Iterate through each column in the data grid.
                for (int c = 0; c < targetGrid.Columns.Count; c++)
                {
                    // Array to hold all the row values;
                    string[] rowStringCollection = new string[0];

                    // If the column is a ComboBox type, then we need to get the full FormattedValue.
                    // Accessing the formatted value is much slower than enumerating with the data table, 
                    // but will yeild correct results in this case.
                    if (targetGrid.Columns[c].CellType == typeof(DataGridViewComboBoxCell))
                    {
                        // Set drop down width for combobox column.
                        SetComboDropWidth((DataGridViewComboBoxColumn)targetGrid.Columns[c], gfx);

                        // Iterate through all the rows and add the formatted values to the array.
                        rowStringCollection = new string[targetGrid.Rows.Count];
                        for (int r = 0; r < targetGrid.Rows.Count; r++)
                        {
                            rowStringCollection[r] = targetGrid[c, r].FormattedValue.ToString();
                        }
                    }
                    else
                    {
                        // Get the name of the current column.
                        // Since the displayed grid columns can vary from the data source,
                        // using the column name makes sure that we reference the correct
                        // column in the gridTable.
                        string columnName = targetGrid.Columns[c].Name;

                        // Leverage Linq enumerator to rapidly collect all the rows into the array, making sure to exclude null values.
                        rowStringCollection = gridTable.AsEnumerable().Where(r => (r.RowState != DataRowState.Deleted && r.Field<object>(columnName) != null)).Select(r => r.Field<object>(columnName).ToString()).ToArray();
                    }

                    // Make sure the Linq query returned results.
                    if (rowStringCollection.Length > 0)
                    {
                        // Measure all the strings in the column.
                        var rowStringSizes = MeasureStrings(rowStringCollection, gfx, targetGrid.DefaultCellStyle.Font);

                        // Sort the array by string widths.
                        rowStringSizes = rowStringSizes.OrderBy((x) => x.Width).ToArray();

                        // Get the last and longest string in the array.
                        var longestStringSize = rowStringSizes.Last();

                        // Measure the width of the header text.
                        var headerSize = gfx.MeasureString(targetGrid.Columns[c].HeaderText, targetGrid.ColumnHeadersDefaultCellStyle.Font);

                        // If the longest string width is larger than the header width, set to the new column width.
                        if (longestStringSize.Width > (int)headerSize.Width)
                        {
                            targetGrid.Columns[c].Width = (int)longestStringSize.Width + widthPadding;
                        }
                        else // Otherwise, set the column width to the header width.
                        {
                            targetGrid.Columns[c].Width = (int)headerSize.Width + widthPadding;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Sets the drop down width of the specified <see cref="DataGridViewComboBoxColumn"/> to fit all the drop down items.
        /// </summary>
        /// <param name="comboColumn"></param>
        /// <param name="gfx"></param>
        private static void SetComboDropWidth(DataGridViewComboBoxColumn comboColumn, Graphics gfx)
        {
            var comboData = (DbAttribute[])comboColumn.DataSource;
            var itemsArray = new string[comboData.Length];

            for (int i = 0; i < itemsArray.Length; i++)
            {
                itemsArray[i] = comboData[i].DisplayValue;
            }

            itemsArray = itemsArray.OrderBy((x) => x.Length).ToArray();
            var maxSize = gfx.MeasureString(itemsArray.Last(), comboColumn.InheritedStyle.Font);
            comboColumn.DropDownWidth = (int)maxSize.Width;
        }

        private static SizeF[] MeasureStrings(string[] stringArray, Graphics gfx, Font font)
        {
            var tempArray = new SizeF[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
            {
                tempArray[i] = new SizeF(gfx.MeasureString(stringArray[i], font));
            }
            return tempArray;
        }
    }
}
