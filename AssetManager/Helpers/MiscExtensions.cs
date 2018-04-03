using System;
using System.Data.Common;
using System.Reflection;
using System.Windows.Forms;
using AssetManager.UserInterface.CustomControls;

namespace AssetManager.Helpers
{
    internal static class MiscExtensions
    {
        // METODO: Create a single extension method for the base Control type.

        public static void DoubleBufferedDataGrid(this DataGridView dgv, bool setting)
        {
            Type dgvType = dgv.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(dgv, setting, null);
        }

        public static void DoubleBufferedListBox(this ListBox listBox, bool setting)
        {
            Type dgvType = listBox.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(listBox, setting, null);
        }

        public static void DoubleBufferedFlowLayout(this FlowLayoutPanel flowPanel, bool setting)
        {
            Type dgvType = flowPanel.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(flowPanel, setting, null);
        }

        public static void DoubleBufferedTableLayout(this TableLayoutPanel tablePanel, bool setting)
        {
            Type dgvType = tablePanel.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(tablePanel, setting, null);
        }

        public static void DoubleBufferedTreeView(this CorrectedTreeView treeView, bool setting)
        {
            Type dgvType = treeView.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(treeView, setting, null);
        }

        public static void DoubleBufferedPanel(this Panel panel, bool setting)
        {
            Type dgvType = panel.GetType();
            PropertyInfo pi = dgvType.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(panel, setting, null);
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
        /// Sets the Rtf property if Rtf format is detected. Otherwise sets the Text property.
        /// </summary>
        /// <param name="richTextBox"></param>
        /// <param name="text"></param>
        public static void TextOrRtf(this RichTextBox richTextBox, string text)
        {
            if (text.StartsWith("{\\rtf"))
            {
                richTextBox.Rtf = text;
            }
            else
            {
                richTextBox.Text = text;
            }
        }
    }
}