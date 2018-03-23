using System;
using System.Diagnostics;
using System.Windows.Forms;
using System.IO;
using MyDialogLib;
namespace AssetManager.Helpers
{

    static class OtherFunctions
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

        public static void EndProgram()
        {
            GlobalSwitches.ProgramEnding = true;
            Logging.Logger("Ending Program...");
            PurgeTempDir();
            // Application.Exit();
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
                Application.DoEvents();
            }
            else
            {
                if (parentForm.InvokeRequired)
                {
                    SetWaitCursorVoidDelegate d = new SetWaitCursorVoidDelegate(SetWaitCursor);
                    parentForm.Invoke(d, new object[] { waiting, parentForm });
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
