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
        public static void CopyToGridForm(this DataGridView grid, ExtendedForm parentForm)
        {
            GridForm NewGridForm = new GridForm(parentForm, grid.Name + " Copy");
            NewGridForm.AddGrid(grid.Name, grid.Name, ((DataTable)grid.DataSource).Copy());
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
            return grid.CurrentRow.Cells[columnName].Value;
        }

        /// <summary>
        /// Returns the string value of the cell in the current row at the specified column.
        /// </summary>
        /// <param name="grid"></param>
        /// <param name="columnName"></param>
        /// <returns></returns>
        public static string CurrentRowStringValue(this DataGridView grid, string columnName)
        {
            return grid.CurrentRow.Cells[columnName].Value.ToString();
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
            // Cast out a DataTable from the target grid datasource.
            // We need to iterate through all the data in the grid and a DataTable supports enumeration.
            var gridTable = (DataTable)targetGrid.DataSource;

            // Create a graphics object from the target grid. Used for measuring text size.
            using (var gfx = targetGrid.CreateGraphics())
            {
                // Iterate through the columns.
                for (int i = 0; i < gridTable.Columns.Count; i++)
                {
                    // Leverage Linq enumerator to rapidly collect all the rows into a string array, making sure to exclude null values.
                    string[] colStringCollection = gridTable.AsEnumerable().Where(r => r.Field<object>(i) != null).Select(r => r.Field<object>(i).ToString()).ToArray();

                    // Make sure the Linq query returned results.
                    if (colStringCollection.Length > 0)
                    {
                        // Measure all the strings in the column.
                        var colStringSizeCollection = MeasureStrings(colStringCollection, gfx, targetGrid.DefaultCellStyle.Font);

                        // Sort the array by string widths.
                        colStringSizeCollection = colStringSizeCollection.OrderBy((x) => x.Size.Width).ToArray();

                        // Get the last and widest string in the array.
                        var newColumnWidth = colStringSizeCollection.Last().Size;

                        // Measure the width of the header text.
                        var headerWidth = gfx.MeasureString(targetGrid.Columns[i].HeaderText, targetGrid.ColumnHeadersDefaultCellStyle.Font);

                        // Compare current header width to measured header width and choose the largest.
                        int maxHeaderWidth = 0;
                        if (targetGrid.Columns[i].HeaderCell.Size.Width > headerWidth.Width)
                        {
                            maxHeaderWidth = targetGrid.Columns[i].HeaderCell.Size.Width;
                        }
                        else
                        {
                            maxHeaderWidth = (int)headerWidth.Width;
                        }

                        // If the calulated max column width is larger than the max header width, set the new column width.
                        if (newColumnWidth.Width > maxHeaderWidth)
                        {
                            targetGrid.Columns[i].Width = (int)newColumnWidth.Width;
                        }
                        else // Otherwise, set the column width to the header width.
                        {
                            targetGrid.Columns[i].Width = maxHeaderWidth;
                        }
                    }
                }
            }
        }

        private static StringSize[] MeasureStrings(string[] stringArray, Graphics gfx, Font font)
        {
            var tempArray = new StringSize[stringArray.Length];
            for (int i = 0; i < stringArray.Length; i++)
            {
                tempArray[i] = new StringSize(stringArray[i], gfx.MeasureString(stringArray[i], font));
            }
            return tempArray;
        }

        private class StringSize
        {
            public string Text { get; set; } = string.Empty;
            public SizeF Size { get; set; } = new SizeF();

            public StringSize()
            {
                Text = string.Empty;
                Size = new SizeF();
            }

            public StringSize(string text, SizeF size)
            {
                Text = text;
                Size = size;
            }
        }

    }
}
