using System;
using System.Data.Common;
using System.Reflection;
using System.Windows.Forms;
using System.Drawing;
using System.Threading.Tasks;

namespace AssetManager.Helpers
{
    internal static class MiscExtensions
    {
        /// <summary>
        /// Sets the protected Control.DoubleBuffered property. Does not set if we are running within a terminal session (RDP).
        /// </summary>
        /// <param name="control"></param>
        /// <param name="setting"></param>
        public static void DoubleBuffered(this Control control, bool setting)
        {
            if (SystemInformation.TerminalServerSession) return;
            Type type = control.GetType();
            PropertyInfo pi = type.GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic);
            pi.SetValue(control, setting, null);
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

        public async static void FlashStrip(this StatusStrip statusStrip, Color flashColor, int flashes)
        {
            var originalColor = statusStrip.BackColor;
            int flashDelay = 200;

            for (int i = 0; i < flashes; i++)
            {
                statusStrip.BackColor = flashColor;
                statusStrip.Refresh();

                await Task.Delay(flashDelay);

                statusStrip.BackColor = originalColor;
                statusStrip.Refresh();

                await Task.Delay(flashDelay);
            }

            statusStrip.BackColor = originalColor;
        }
    }
}