﻿using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls.LiveBox
{
    public class LiveBox : IDisposable
    {
        #region "Fields"

        private LiveBoxArgs CurrentLiveBoxArgs;
        private ListBox LiveListBox;
        private List<LiveBoxArgs> LiveBoxControls = new List<LiveBoxArgs>();
        private int rowLimit = 30;

        private bool queryRunning = false;

        private string previousSearchString;

        #endregion "Fields"

        #region "Constructors"

        public LiveBox(Form parentForm)
        {
            InitializeLiveListBox(parentForm);
        }

        #endregion "Constructors"

        #region "Methods"

        public void AttachToControl(TextBox control, string displayMember, LiveBoxSelectionType type, string valueMember = null)
        {
            LiveBoxArgs ControlArgs = new LiveBoxArgs(control, displayMember, type, valueMember);
            LiveBoxControls.Add(ControlArgs);
            ControlArgs.Control.KeyUp += Control_KeyUp;
            ControlArgs.Control.KeyDown += Control_KeyDown;
            ControlArgs.Control.LostFocus += Control_LostFocus;
            ControlArgs.Control.ReadOnlyChanged += Control_LostFocus;
        }

        public void GiveLiveBoxFocus()
        {
            LiveListBox.Focus();
            if (LiveListBox.SelectedIndex == -1)
            {
                LiveListBox.SelectedIndex = 0;
            }
        }

        public void HideLiveBox()
        {
            LiveListBox.Visible = false;
            LiveListBox.DataSource = null;
        }

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                GiveLiveBoxFocus();
            }
        }

        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                HideLiveBox();
            }
            else
            {
                //don't respond to non-alpha keys
                if (e.KeyCode == Keys.ShiftKey || e.KeyCode == Keys.Alt || e.KeyCode == Keys.ControlKey || e.KeyCode == Keys.Menu)
                {
                    //do nothing
                }
                else
                {
                    LiveBoxArgs arg = GetSenderArgs(sender);
                    if (!arg.Control.ReadOnly)
                    {
                        StartLiveSearch(arg);
                    }
                }
            }
        }

        private void Control_LostFocus(object sender, EventArgs e)
        {
            if (CurrentLiveBoxArgs.Control != null)
            {
                if (!CurrentLiveBoxArgs.Control.Focused & !LiveListBox.Focused)
                {
                    if (LiveListBox.Visible)
                        HideLiveBox();
                }
                if (!CurrentLiveBoxArgs.Control.Enabled)
                {
                    if (LiveListBox.Visible)
                        HideLiveBox();
                }
                if (CurrentLiveBoxArgs.Control is TextBox)
                {
                    TextBox txt = CurrentLiveBoxArgs.Control;
                    if (txt.ReadOnly)
                    {
                        if (LiveListBox.Visible)
                            HideLiveBox();
                    }
                }
            }
        }

        private void DrawLiveBox(DataTable results)
        {
            try
            {
                if (results.Rows.Count > 0)
                {
                    LiveListBox.SuspendLayout();
                    LiveListBox.BeginUpdate();
                    LiveListBox.DataSource = results;
                    LiveListBox.DisplayMember = CurrentLiveBoxArgs.DisplayMember;
                    LiveListBox.ValueMember = CurrentLiveBoxArgs.ValueMember;
                    LiveListBox.ClearSelected();
                    PosistionLiveBox();
                    LiveListBox.Visible = true;
                    LiveListBox.EndUpdate();
                    LiveListBox.ResumeLayout();
                    if (previousSearchString != CurrentLiveBoxArgs.Control.Text.Trim())
                    {
                        StartLiveSearch(CurrentLiveBoxArgs);
                        //if search string has changed since last completion, run again.
                    }
                }
                else
                {
                    LiveListBox.Visible = false;
                }
            }
            catch
            {
                HideLiveBox();
            }
        }

        private LiveBoxArgs GetSenderArgs(object sender)
        {
            foreach (LiveBoxArgs arg in LiveBoxControls)
            {
                if (object.ReferenceEquals(arg.Control, (TextBox)sender))
                {
                    return arg;
                }
            }
            return null;
        }

        private void InitializeLiveListBox(Form parentForm)
        {
            LiveListBox = new ListBox();
            LiveListBox.Parent = parentForm;
            LiveListBox.BringToFront();
            LiveListBox.MouseDown += LiveBox_MouseDown;
            LiveListBox.MouseMove += LiveBox_MouseMove;
            LiveListBox.KeyDown += LiveBox_KeyDown;
            LiveListBox.LostFocus += LiveBox_LostFocus;
            LiveListBox.PreviewKeyDown += LiveBox_PreviewKeyDown;
            LiveListBox.DoubleBufferedListBox(true);
            LiveListBox.Visible = false;
            CurrentLiveBoxArgs = new LiveBoxArgs();
            SetStyle();
        }

        private void LiveBox_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            if (e.KeyCode == Keys.Tab)
            {
                LiveBoxSelect();
            }
        }

        private void LiveBox_LostFocus(object sender, EventArgs e)
        {
            HideLiveBox();
        }

        private void LiveBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                LiveBoxSelect();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                HideLiveBox();
            }
        }

        private void LiveBox_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                LiveBoxSelect();
            }
            else if (e.Button == MouseButtons.Right)
            {
                HideLiveBox();
            }
        }

        private void LiveBox_MouseMove(object sender, MouseEventArgs e)
        {
            LiveListBox.SelectedIndex = LiveListBox.IndexFromPoint(e.Location);
        }

        private void LiveBoxSelect()
        {
            string selectedText = LiveListBox.Text;
            string selectedValue = LiveListBox.SelectedValue.ToString();
            HideLiveBox();

            var parentForm = CurrentLiveBoxArgs.Control.FindForm();
            if (parentForm is ILiveBox)
            {
                var liveboxForm = (ILiveBox)parentForm;

                switch (CurrentLiveBoxArgs.Type)
                {
                    case LiveBoxSelectionType.DynamicSearch:
                        CurrentLiveBoxArgs.Control.Text = selectedText;
                        liveboxForm.DynamicSearch();
                        break;

                    case LiveBoxSelectionType.LoadDevice:
                        CurrentLiveBoxArgs.Control.Text = "";
                        liveboxForm.LoadDevice(selectedValue);
                        break;

                    case LiveBoxSelectionType.SelectValue:
                        CurrentLiveBoxArgs.Control.Text = selectedText;
                        break;

                    case LiveBoxSelectionType.UserSelect:
                        CurrentLiveBoxArgs.Control.Text = selectedText;
                        liveboxForm.MunisUser = new MunisEmployee(selectedText, selectedValue);
                        break;
                }
            }
            else
            {
                throw new Exception(nameof(ILiveBox) + " is not implemented in parent Form.");
            }
        }

        private void PosistionLiveBox()
        {
            Point ScreenPos = LiveListBox.Parent.PointToClient(CurrentLiveBoxArgs.Control.Parent.PointToScreen(CurrentLiveBoxArgs.Control.Location));
            ScreenPos.Y += CurrentLiveBoxArgs.Control.Height - 1;
            LiveListBox.Location = ScreenPos;
            LiveListBox.Width = PreferredWidth();
            Rectangle FormBounds = LiveListBox.Parent.ClientRectangle;
            if (LiveListBox.PreferredHeight + LiveListBox.Top > FormBounds.Bottom)
            {
                LiveListBox.Height = FormBounds.Bottom - LiveListBox.Top - LiveListBox.Padding.Bottom;
            }
            else
            {
                LiveListBox.Height = LiveListBox.PreferredHeight;
            }
        }

        private int PreferredWidth()
        {
            using (Graphics gfx = LiveListBox.CreateGraphics())
            {
                int MaxLen = 0;
                foreach (DataRowView row in LiveListBox.Items)
                {
                    int ItemLen = (int)gfx.MeasureString(row[CurrentLiveBoxArgs.DisplayMember].ToString(), LiveListBox.Font).Width;
                    if (ItemLen > MaxLen)
                    {
                        MaxLen = ItemLen;
                    }
                }
                if (MaxLen > CurrentLiveBoxArgs.Control.Width)
                {
                    return MaxLen;
                }
                else
                {
                    return CurrentLiveBoxArgs.Control.Width;
                }
            }
        }

        /// <summary>
        /// Runs the DB query Asynchronously.
        /// </summary>
        /// <param name="searchString"></param>
        private async void ProcessSearch(string searchString)
        {
            try
            {
                if (queryRunning) return;
                queryRunning = true;
                DataTable Results = await Task.Run(() =>
                {
                    string strQry;

                    if (CurrentLiveBoxArgs.ValueMember == null)
                    {
                        strQry = "SELECT " + DevicesCols.DeviceUID + "," + CurrentLiveBoxArgs.DisplayMember + " FROM " + DevicesCols.TableName + " WHERE " + CurrentLiveBoxArgs.DisplayMember + " LIKE  @Search_Value  GROUP BY " + CurrentLiveBoxArgs.DisplayMember + " ORDER BY " + CurrentLiveBoxArgs.DisplayMember + " LIMIT " + rowLimit;
                    }
                    else
                    {
                        strQry = "SELECT " + DevicesCols.DeviceUID + "," + CurrentLiveBoxArgs.DisplayMember + "," + CurrentLiveBoxArgs.ValueMember + " FROM " + DevicesCols.TableName + " WHERE " + CurrentLiveBoxArgs.DisplayMember + " LIKE  @Search_Value  GROUP BY " + CurrentLiveBoxArgs.DisplayMember + " ORDER BY " + CurrentLiveBoxArgs.DisplayMember + " LIMIT " + rowLimit;
                    }

                    using (var cmd = DBFactory.GetDatabase().GetCommand(strQry))
                    {
                        cmd.AddParameterWithValue("@Search_Value", "%" + searchString + "%");
                        return DBFactory.GetDatabase().DataTableFromCommand(cmd);
                    }
                });
                previousSearchString = searchString;
                queryRunning = false;
                DrawLiveBox(Results);
            }
            catch (Exception ex)
            {
                queryRunning = false;
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void SetStyle()
        {
            Font LiveBoxFont = new Font("Consolas", 11.25f, FontStyle.Bold);
            LiveListBox.BackColor = Color.FromArgb(255, 208, 99);
            LiveListBox.BorderStyle = BorderStyle.FixedSingle;
            LiveListBox.Font = LiveBoxFont;
            LiveListBox.ForeColor = Color.Black;
            LiveListBox.Padding = new Padding(0, 0, 0, 10);
        }

        private void StartLiveSearch(LiveBoxArgs args)
        {
            CurrentLiveBoxArgs = args;
            string strSearchString = CurrentLiveBoxArgs.Control.Text.Trim();
            if (!string.IsNullOrEmpty(strSearchString))
            {
                ProcessSearch(strSearchString);
            }
            else
            {
                HideLiveBox();
            }
        }

        private void RemovedHandlers()
        {
            foreach (var control in LiveBoxControls)
            {
                control.Control.KeyUp -= Control_KeyUp;
                control.Control.KeyDown -= Control_KeyDown;
                control.Control.LostFocus -= Control_LostFocus;
                control.Control.ReadOnlyChanged -= Control_LostFocus;
            }
        }

        #endregion "Methods"

        #region "Structs"

        private class LiveBoxArgs
        {
            public TextBox Control { get; set; }
            public string DisplayMember { get; set; }
            public Nullable<LiveBoxSelectionType> Type { get; set; }
            public string ValueMember { get; set; }

            public LiveBoxArgs(TextBox control, string displayMember, LiveBoxSelectionType type, string valueMember)
            {
                this.Control = control;
                this.DisplayMember = displayMember;
                this.Type = type;
                this.ValueMember = valueMember;
            }

            public LiveBoxArgs()
            {
                this.Control = null;
                this.DisplayMember = null;
                this.Type = null;
                this.ValueMember = null;
            }
        }


        // METODO: Un-nest.
        public enum LiveBoxSelectionType
        {
            DynamicSearch,
            LoadDevice,
            SelectValue,
            UserSelect
        }

        #endregion "Structs"

        #region "IDisposable Support"

        // To detect redundant calls
        private bool disposedValue;

        // TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        //Protected Overrides Sub Finalize()
        //    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub
        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            // TODO: uncomment the following line if Finalize() is overridden above.
            // GC.SuppressFinalize(Me)
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    RemovedHandlers();
                    LiveListBox.Dispose();
                    LiveBoxControls.Clear();
                }
                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            disposedValue = true;
        }

        #endregion "IDisposable Support"
    }
}