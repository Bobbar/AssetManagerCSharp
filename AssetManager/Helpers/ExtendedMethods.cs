using System;
using System.Windows.Forms;
using System.Data.Common;
using System.Reflection;
using System.Data;
using System.Linq;

namespace AssetManager
{

    static class ExtendedMethods
    {

        public static void DoubleBufferedDataGrid(DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedListView(ListView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedListBox(ListBox dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedFlowLayout(FlowLayoutPanel dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedTableLayout(TableLayoutPanel dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedPanel(Panel dgv, bool setting)
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

                    // Sort the string array by string lengths.
                    colStringCollection = colStringCollection.OrderBy((x) => x.Length).ToArray();

                    // Get the last and longest string in the array.
                    string longestColString = colStringCollection.Last();

                    // Use the graphics object to measure the string size.
                    var colWidth = gfx.MeasureString(longestColString, targetGrid.Font);

                    // If the calulated width is longer than the column header width, set the new column width.
                    if (colWidth.Width > targetGrid.Columns[i].HeaderCell.Size.Width)
                    {
                        targetGrid.Columns[i].Width = (int)colWidth.Width;
                    }
                    else // Otherwise, set the column width to the header width.
                    {
                        targetGrid.Columns[i].Width = targetGrid.Columns[i].HeaderCell.Size.Width;
                    }
                }
            }
        }

    }
}
