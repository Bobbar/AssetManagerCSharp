using MyDialogLib;
using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AssetManager.Helpers
{
    internal static class OtherFunctions
    {
        public static Stopwatch stpw = new Stopwatch();

        private static int intTimerHits = 0;

        public static void StartTimer()
        {
            stpw.Stop();
            stpw.Reset();
            stpw.Start();
        }

        public static string StopTimer()
        {
            stpw.Stop();
            intTimerHits += 1;
            string Results = intTimerHits + "  Stopwatch: MS:" + stpw.ElapsedMilliseconds + " Ticks: " + stpw.ElapsedTicks;
            Debug.Print(Results);
            return Results;
        }

        public static string ElapTime()
        {
            string results = intTimerHits + "  Elapsed: MS:" + stpw.ElapsedMilliseconds + " Ticks: " + stpw.ElapsedTicks;
            Debug.Print(results);
            return results;
        }

        public static void EndProgram(bool forceEnd = false)
        {
            GlobalSwitches.ProgramEnding = true;
            Logging.Logger("Ending Program...");
            PurgeTempDir();

            // Forcefully kill the process. Used when unhandled or critical exceptions occur.
            if (forceEnd)
            {
                Environment.Exit(-1);
            }
        }

        public static void PurgeTempDir()
        {
            try
            {
                if (Directory.Exists(Paths.DownloadPath))
                {
                    Directory.Delete(Paths.DownloadPath, true);
                }
            }
            catch
            {
                // Sometimes a file will be in use, so we don't care if this fails.
            }
        }

        public static string NotePreview(string Note, int CharLimit = 50)
        {
            if (!string.IsNullOrEmpty(Note))
            {
                if (Note.Length > CharLimit)
                {
                    return Note.Substring(0, CharLimit) + "...";
                }
                else
                {
                    return Note;
                }
            }
            else
            {
                return string.Empty;
            }
        }

        public static DialogResult Message(string Prompt, MessageBoxButtons button = MessageBoxButtons.OK, MessageBoxIcon icon = MessageBoxIcon.Information, string Title = null, Form ParentFrm = null)
        {
            SetWaitCursor(false, ParentFrm);
            using (var newMessage = new AdvancedDialog(ParentFrm))
            {
                return newMessage.DialogMessage(Prompt, button, icon, Title, ParentFrm);
            }
        }

        public static bool OKToEnd()
        {
            if (GlobalSwitches.BuildingCache)
            {
                Message("Still building DB Cache. Please wait and try again.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Critical Function Running");
                return false;
            }
            return true;
        }

        public delegate void SetWaitCursorVoidDelegate(bool waiting, Form parentForm = null);

        public static void SetWaitCursor(bool waiting, Form parentForm = null)
        {
            if (parentForm == null)
            {
                Application.UseWaitCursor = waiting;
            }
            else
            {
                if (parentForm.InvokeRequired)
                {
                    SetWaitCursorVoidDelegate d = new SetWaitCursorVoidDelegate(SetWaitCursor);
                    parentForm.BeginInvoke(d, new object[] { waiting, parentForm });
                }
                else
                {
                    if (waiting)
                    {
                        parentForm.UseWaitCursor = true;
                    }
                    else
                    {
                        parentForm.UseWaitCursor = false;
                    }
                    parentForm.Update();
                }
            }
        }

        private static RichTextBox rtfBox = new RichTextBox();

        public static string RTFToPlainText(string rtfText)
        {
            try
            {
                if (rtfText.StartsWith("{\\rtf"))
                {
                    rtfBox.Rtf = rtfText;
                    return rtfBox.Text;
                }
                else
                {
                    return rtfText;
                }
            }
            catch (ArgumentException)
            {
                //If we get an argument error, that means the text is not RTF so we return the plain text.
                return rtfText;
            }
        }
    }
}