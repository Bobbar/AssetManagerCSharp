using System;
using System.Windows.Forms;
using System.Data.Common;
using System.Reflection;
using System.Data;
using System.Linq;
using System.Drawing;

namespace AssetManager
{

    static class ExtendedMethods
    {

        public static void DoubleBufferedDataGrid(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedListBox(this ListBox dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedFlowLayout(this FlowLayoutPanel dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedTableLayout(this TableLayoutPanel dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedPanel(this Panel dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        /// <summary>
        /// Adds a parameter to the command.
        /// </summary>
        /// <param name="command">
        /// The command.
        /// </param>
        /// <param name="parameterName">
        /// Name of the parameter.
        /// </param>
        /// <param name="parameterValue">
        /// The parameter value.
        /// </param>
        /// <remarks>
        /// </remarks>
        public static void AddParameterWithValue(this DbCommand command, string parameterName, object parameterValue)
        {
            var parameter = command.CreateParameter();
            parameter.ParameterName = parameterName;
            parameter.Value = parameterValue;
            command.Parameters.Add(parameter);
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
