using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    public class LiveBox : IDisposable
    {
        #region "Fields"
        private Font liveBoxFont = new Font("Consolas", 11.25f, FontStyle.Bold);
        private LiveBoxArgs currentLiveBoxArgs;
        private ListBox liveListBox;
        private List<LiveBoxArgs> liveBoxControls = new List<LiveBoxArgs>();
        private int rowLimit = 30;
        private bool queryRunning = false;
        private string previousSearchString;
        private int previousIndex = -1;

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
            LiveBoxArgs controlArgs = new LiveBoxArgs(control, displayMember, type, valueMember);
            liveBoxControls.Add(controlArgs);
            controlArgs.Control.KeyUp += Control_KeyUp;
            controlArgs.Control.KeyDown += Control_KeyDown;
            controlArgs.Control.LostFocus += Control_LostFocus;
            controlArgs.Control.ReadOnlyChanged += Control_LostFocus;
        }

        public void GiveLiveBoxFocus()
        {
            liveListBox.Focus();
            if (liveListBox.SelectedIndex == -1)
            {
                liveListBox.SelectedIndex = 0;
            }
        }

        public void HideLiveBox()
        {
            liveListBox.Visible = false;
            liveListBox.DataSource = null;
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
            if (currentLiveBoxArgs.Control != null)
            {
                if (!currentLiveBoxArgs.Control.Focused & !liveListBox.Focused)
                {
                    if (liveListBox.Visible)
                        HideLiveBox();
                }
                if (!currentLiveBoxArgs.Control.Enabled)
                {
                    if (liveListBox.Visible)
                        HideLiveBox();
                }
                if (currentLiveBoxArgs.Control is TextBox)
                {
                    TextBox txt = currentLiveBoxArgs.Control;
                    if (txt.ReadOnly)
                    {
                        if (liveListBox.Visible)
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
                    liveListBox.SuspendLayout();
                    liveListBox.BeginUpdate();
                    liveListBox.DataSource = results;
                    liveListBox.DisplayMember = currentLiveBoxArgs.DisplayMember;
                    liveListBox.ValueMember = currentLiveBoxArgs.ValueMember;
                    liveListBox.ClearSelected();
                    PosistionLiveBox();
                    liveListBox.Visible = true;
                    liveListBox.EndUpdate();
                    liveListBox.ResumeLayout();
                    if (previousSearchString != currentLiveBoxArgs.Control.Text.Trim())
                    {
                        StartLiveSearch(currentLiveBoxArgs);
                        //if search string has changed since last completion, run again.
                    }
                }
                else
                {
                    liveListBox.Visible = false;
                }
            }
            catch
            {
                HideLiveBox();
            }
        }

        private LiveBoxArgs GetSenderArgs(object sender)
        {
            foreach (LiveBoxArgs arg in liveBoxControls)
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
            liveListBox = new ListBox();
            liveListBox.Parent = parentForm;
            liveListBox.BringToFront();
            liveListBox.MouseDown += LiveBox_MouseDown;
            liveListBox.MouseMove += LiveBox_MouseMove;
            liveListBox.KeyDown += LiveBox_KeyDown;
            liveListBox.LostFocus += LiveBox_LostFocus;
            liveListBox.PreviewKeyDown += LiveBox_PreviewKeyDown;
            liveListBox.DoubleBuffered(true);
            liveListBox.Visible = false;
            currentLiveBoxArgs = new LiveBoxArgs();
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
            int currentIndex = liveListBox.IndexFromPoint(e.Location);

            if (previousIndex != currentIndex)
            {
                previousIndex = currentIndex;
                liveListBox.SetSelected(currentIndex, true);
            }
        }

        private void LiveBoxSelect()
        {
            string selectedText = liveListBox.Text;
            string selectedValue = liveListBox.SelectedValue.ToString();
            HideLiveBox();

            var parentForm = currentLiveBoxArgs.Control.FindForm();
            if (parentForm is ILiveBox)
            {
                var liveboxForm = (ILiveBox)parentForm;

                switch (currentLiveBoxArgs.Type)
                {
                    case LiveBoxSelectionType.DynamicSearch:
                        currentLiveBoxArgs.Control.Text = selectedText;
                        liveboxForm.DynamicSearch();
                        break;

                    case LiveBoxSelectionType.LoadDevice:
                        currentLiveBoxArgs.Control.Text = "";
                        liveboxForm.LoadDevice(selectedValue);
                        break;

                    case LiveBoxSelectionType.SelectValue:
                        currentLiveBoxArgs.Control.Text = selectedText;
                        break;

                    case LiveBoxSelectionType.UserSelect:
                        currentLiveBoxArgs.Control.Text = selectedText;
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
            Point ScreenPos = liveListBox.Parent.PointToClient(currentLiveBoxArgs.Control.Parent.PointToScreen(currentLiveBoxArgs.Control.Location));
            ScreenPos.Y += currentLiveBoxArgs.Control.Height - 1;
            liveListBox.Location = ScreenPos;
            liveListBox.Width = PreferredWidth();
            Rectangle FormBounds = liveListBox.Parent.ClientRectangle;
            if (liveListBox.PreferredHeight + liveListBox.Top > FormBounds.Bottom)
            {
                liveListBox.Height = FormBounds.Bottom - liveListBox.Top - liveListBox.Padding.Bottom;
            }
            else
            {
                liveListBox.Height = liveListBox.PreferredHeight;
            }
        }

        private int PreferredWidth()
        {
            using (Graphics gfx = liveListBox.CreateGraphics())
            {
                int MaxLen = 0;
                foreach (DataRowView row in liveListBox.Items)
                {
                    int ItemLen = (int)gfx.MeasureString(row[currentLiveBoxArgs.DisplayMember].ToString(), liveListBox.Font).Width;
                    if (ItemLen > MaxLen)
                    {
                        MaxLen = ItemLen;
                    }
                }
                if (MaxLen > currentLiveBoxArgs.Control.Width)
                {
                    return MaxLen;
                }
                else
                {
                    return currentLiveBoxArgs.Control.Width;
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

                    if (currentLiveBoxArgs.ValueMember == null)
                    {
                        strQry = "SELECT " + DevicesCols.DeviceGuid + "," + currentLiveBoxArgs.DisplayMember + " FROM " + DevicesCols.TableName + " WHERE " + currentLiveBoxArgs.DisplayMember + " LIKE  @Search_Value  GROUP BY " + currentLiveBoxArgs.DisplayMember + " ORDER BY " + currentLiveBoxArgs.DisplayMember + " LIMIT " + rowLimit;
                    }
                    else
                    {
                        strQry = "SELECT " + DevicesCols.DeviceGuid + "," + currentLiveBoxArgs.DisplayMember + "," + currentLiveBoxArgs.ValueMember + " FROM " + DevicesCols.TableName + " WHERE " + currentLiveBoxArgs.DisplayMember + " LIKE  @Search_Value  GROUP BY " + currentLiveBoxArgs.DisplayMember + " ORDER BY " + currentLiveBoxArgs.DisplayMember + " LIMIT " + rowLimit;
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
            liveListBox.BackColor = Color.FromArgb(255, 208, 99);
            liveListBox.BorderStyle = BorderStyle.FixedSingle;
            liveListBox.Font = liveBoxFont;
            liveListBox.ForeColor = Color.Black;
            liveListBox.Padding = new Padding(0, 0, 0, 10);
        }

        private void StartLiveSearch(LiveBoxArgs args)
        {
            currentLiveBoxArgs = args;
            string strSearchString = currentLiveBoxArgs.Control.Text.Trim();
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
            foreach (var control in liveBoxControls)
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


        private bool disposedValue;

        public void Dispose()
        {
            Dispose(true);
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    RemovedHandlers();
                    liveListBox.Dispose();
                    liveBoxControls.Clear();
                    liveBoxFont.Dispose();
                }
            }
            disposedValue = true;
        }

        #endregion "IDisposable Support"
    }
}