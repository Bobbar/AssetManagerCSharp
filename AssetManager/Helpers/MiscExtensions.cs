using System;
using System.ComponentModel;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

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

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern bool LockWindowUpdate(IntPtr hWndLock);

        public static void Suspend(this Control control)
        {
            LockWindowUpdate(control.Handle);
        }

        public static void Resume(this Control control)
        {
            LockWindowUpdate(IntPtr.Zero);
        }


        public static T Find<T>(this BindingList<T> list, Predicate<T> match)
        {
            foreach (var item in list)
            {
                if (match(item))
                    return item;
            }
            return default(T);
        }

        public static bool Exists<T>(this BindingList<T> list, Predicate<T> match)
        {
            foreach (var item in list)
            {
                if (match(item))
                    return true;
            }
            return false;
        }

        public static void ForEach<T>(this BindingList<T> list, Action<T> action)
        {
            foreach (var item in list)
            {
                action(item);
            }
        }

        public static BindingList<T> Sort<T>(this BindingList<T> list, SortOrder direction, string propertyName = null)
        {
            T[] arr = new T[list.Count];
            list.CopyTo(arr, 0);

            if (propertyName != null)
            {
                if (direction == SortOrder.Ascending)
                {
                    var sorted = arr.OrderBy(item => item.GetType().GetProperty(propertyName).GetValue(item, null));
                    list = new BindingList<T>(sorted.ToList());
                }
                else
                {
                    var sorted = arr.OrderByDescending(item => item.GetType().GetProperty(propertyName).GetValue(item, null));
                    list = new BindingList<T>(sorted.ToList());
                }
            }
            else
            {
                if (direction == SortOrder.Ascending)
                {
                    var sorted = arr.OrderBy(item => item);
                    list = new BindingList<T>(sorted.ToList());
                }
                else
                {
                    var sorted = arr.OrderByDescending(item => item);
                    list = new BindingList<T>(sorted.ToList());
                }
            }

            return list;
        }
    }
}