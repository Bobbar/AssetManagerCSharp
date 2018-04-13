using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    public class LiveBox : IDisposable
    {
        #region "Fields"

        private Font liveBoxFont = new Font("Consolas", 11.25f, FontStyle.Bold);
        private LiveBoxConfig currentLiveBoxConfig;
        private ListBox liveListBox;
        private List<LiveBoxConfig> liveBoxConfigs = new List<LiveBoxConfig>();
        private int rowLimit = 30;
        private bool queryRunning = false;
        private string previousSearchString;
        private int previousIndex = -1;
        private Graphics liveBoxGraphics;
        private bool liveBoxSelected = false;

        #endregion "Fields"

        #region "Constructors"

        public LiveBox(Form parentForm)
        {
            InitializeLiveListBox(parentForm);
        }

        #endregion "Constructors"

        #region "Methods"

        public void AttachToControl(TextBox targetTextBox, string displayMember, LiveBoxSelectAction selectAction, string valueMember = null)
        {
            var config = new LiveBoxConfig(targetTextBox, displayMember, selectAction, valueMember);
            liveBoxConfigs.Add(config);
            config.TargetTextBox.KeyDown += TargetTextBox_KeyDown;
            config.TargetTextBox.LostFocus += TargetTextBox_LostFocus;
            config.TargetTextBox.ReadOnlyChanged += TargetTextBox_ReadOnlyChanged;
            config.TargetTextBox.TextChanged += TargetTextBox_TextChanged;
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
            if (liveListBox.Visible)
            {
                liveListBox.Visible = false;
                liveListBox.DataSource = null;
            }
        }

        private void TargetTextBox_TextChanged(object sender, EventArgs e)
        {
            if (!liveBoxSelected)
            {
                var config = GetSenderConfig(sender);
                if (!config.TargetTextBox.ReadOnly)
                {
                    StartLiveSearch(config);
                }
            }
        }

        private void TargetTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Down)
            {
                GiveLiveBoxFocus();
            }
            else if (e.KeyCode == Keys.Escape)
            {
                HideLiveBox();
            }
        }

        private void TargetTextBox_LostFocus(object sender, EventArgs e)
        {
            if (currentLiveBoxConfig == null) return;

            if (currentLiveBoxConfig.TargetTextBox != null)
            {
                if (!currentLiveBoxConfig.TargetTextBox.Focused & !liveListBox.Focused)
                {
                    HideLiveBox();
                }

                if (!currentLiveBoxConfig.TargetTextBox.Enabled)
                {
                    HideLiveBox();
                }

                if (currentLiveBoxConfig.TargetTextBox.ReadOnly)
                {
                    HideLiveBox();
                }
            }
        }

        private void TargetTextBox_ReadOnlyChanged(object sender, EventArgs e)
        {
            if (((TextBox)sender).ReadOnly) HideLiveBox();
        }

        private void DisplayLiveBox(DataTable results)
        {
            try
            {
                if (results.Rows.Count > 0)
                {
                    liveListBox.SuspendLayout();
                    liveListBox.ValueMember = currentLiveBoxConfig.ValueMember;
                    liveListBox.DisplayMember = currentLiveBoxConfig.DisplayMember;
                    liveListBox.DataSource = results;
                    liveListBox.ClearSelected();
                    PosistionLiveBox();
                    liveListBox.Visible = true;
                    liveListBox.ResumeLayout();

                    if (previousSearchString != currentLiveBoxConfig.TargetTextBox.Text.Trim())
                    {
                        StartLiveSearch(currentLiveBoxConfig);
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

        private LiveBoxConfig GetSenderConfig(object sender)
        {
            foreach (var config in liveBoxConfigs)
            {
                if (ReferenceEquals(config.TargetTextBox, (TextBox)sender))
                {
                    return config;
                }
            }
            return null;
        }

        private void InitializeLiveListBox(Form parentForm)
        {
            liveListBox = new ListBox();
            liveListBox.Parent = parentForm;
            liveListBox.BringToFront();
            AddLiveBoxEvents();
            liveListBox.DoubleBuffered(true);
            liveListBox.Visible = false;
            liveBoxGraphics = liveListBox.CreateGraphics();
            SetStyle();
        }

        private void AddLiveBoxEvents()
        {
            liveListBox.MouseDown += LiveBox_MouseDown;
            liveListBox.MouseMove += LiveBox_MouseMove;
            liveListBox.KeyDown += LiveBox_KeyDown;
            liveListBox.LostFocus += LiveBox_LostFocus;
            liveListBox.PreviewKeyDown += LiveBox_PreviewKeyDown;
        }

        private void RemoveLiveBoxEvents()
        {
            liveListBox.MouseDown -= LiveBox_MouseDown;
            liveListBox.MouseMove -= LiveBox_MouseMove;
            liveListBox.KeyDown -= LiveBox_KeyDown;
            liveListBox.LostFocus -= LiveBox_LostFocus;
            liveListBox.PreviewKeyDown -= LiveBox_PreviewKeyDown;
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
            try
            {
                liveBoxSelected = true;
                string selectedText = liveListBox.Text;
                string selectedValue = liveListBox.SelectedValue.ToString();

                HideLiveBox();

                var parentForm = currentLiveBoxConfig.TargetTextBox.FindForm();
                if (parentForm is ILiveBox)
                {
                    var liveboxForm = (ILiveBox)parentForm;

                    switch (currentLiveBoxConfig.SelectionType)
                    {
                        case LiveBoxSelectAction.DynamicSearch:
                            currentLiveBoxConfig.TargetTextBox.Text = selectedText;
                            liveboxForm.DynamicSearch();
                            break;

                        case LiveBoxSelectAction.LoadDevice:
                            currentLiveBoxConfig.TargetTextBox.Text = "";
                            liveboxForm.LoadDevice(selectedValue);
                            break;

                        case LiveBoxSelectAction.SelectValue:
                            currentLiveBoxConfig.TargetTextBox.Text = selectedText;
                            break;

                        case LiveBoxSelectAction.UserSelect:
                            currentLiveBoxConfig.TargetTextBox.Text = selectedText;
                            liveboxForm.MunisUser = new MunisEmployee(selectedText, selectedValue);
                            break;
                    }
                }
                else
                {
                    throw new Exception(nameof(ILiveBox) + " is not implemented in parent Form.");
                }
            }
            finally
            {
                liveBoxSelected = false;
            }

        }

        private void PosistionLiveBox()
        {
            // Get top-left location of the target control.
            var targetLocation = liveListBox.Parent.PointToClient(currentLiveBoxConfig.TargetTextBox.Parent.PointToScreen(currentLiveBoxConfig.TargetTextBox.Location));

            // Shift the location down to the bottom of the target control.
            targetLocation.Y += currentLiveBoxConfig.TargetTextBox.Height - 1;

            // Set the listbox location.
            liveListBox.Location = targetLocation;

            // Set the listbox width to fit the contents.
            liveListBox.Width = PreferredWidth();

            // Get the height of the listbox and the bounds of the parent form
            // and adjust the height as needed to keep the listbox within the form.
            var formBounds = liveListBox.Parent.ClientRectangle;
            var prefHeight = liveListBox.PreferredHeight;

            if (liveListBox.Top + prefHeight > formBounds.Bottom)
            {
                liveListBox.Height = formBounds.Bottom - liveListBox.Top - liveListBox.Padding.Bottom;
            }
            else
            {
                liveListBox.Height = prefHeight;
            }
        }

        private int PreferredWidth()
        {
            // Cast and convert items to an array.
            var itemArray = liveListBox.Items.Cast<DataRowView>().ToArray();

            // Sort the items by the display member string length.
            itemArray = itemArray.OrderBy((i) => i[currentLiveBoxConfig.DisplayMember].ToString().Length).ToArray();

            // Get the last and longest string.
            var longestItem = itemArray.Last()[currentLiveBoxConfig.DisplayMember].ToString();

            // Measure the size of the string and collect the width.
            float maxLen = liveBoxGraphics.MeasureString(longestItem, liveListBox.Font).Width;

            // If the longest string is wider than the target textbox width, return the new width.
            if (maxLen > currentLiveBoxConfig.TargetTextBox.Width)
            {
                return (int)maxLen;
            }
            else
            {
                return currentLiveBoxConfig.TargetTextBox.Width;
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

                DataTable results = await Task.Run(() =>
                {
                    string query;

                    if (currentLiveBoxConfig.ValueMember == null)
                    {
                        query = "SELECT " + DevicesCols.DeviceGuid + "," + currentLiveBoxConfig.DisplayMember + " FROM " + DevicesCols.TableName + " WHERE " + currentLiveBoxConfig.DisplayMember + " LIKE  @Search_Value  GROUP BY " + currentLiveBoxConfig.DisplayMember + " ORDER BY " + currentLiveBoxConfig.DisplayMember + " LIMIT " + rowLimit;
                    }
                    else
                    {
                        query = "SELECT " + DevicesCols.DeviceGuid + "," + currentLiveBoxConfig.DisplayMember + "," + currentLiveBoxConfig.ValueMember + " FROM " + DevicesCols.TableName + " WHERE " + currentLiveBoxConfig.DisplayMember + " LIKE  @Search_Value  GROUP BY " + currentLiveBoxConfig.DisplayMember + " ORDER BY " + currentLiveBoxConfig.DisplayMember + " LIMIT " + rowLimit;
                    }

                    using (var cmd = DBFactory.GetDatabase().GetCommand(query))
                    {
                        cmd.AddParameterWithValue("@Search_Value", "%" + searchString + "%");
                        return DBFactory.GetDatabase().DataTableFromCommand(cmd);
                    }
                });
                previousSearchString = searchString;
                queryRunning = false;

                DisplayLiveBox(results);
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

        private void StartLiveSearch(LiveBoxConfig args)
        {
            currentLiveBoxConfig = args;
            string searchString = currentLiveBoxConfig.TargetTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(searchString))
            {
                ProcessSearch(searchString);
            }
            else
            {
                HideLiveBox();
            }
        }

        private void RemoveTargetEvents()
        {
            foreach (var config in liveBoxConfigs)
            {
                config.TargetTextBox.TextChanged -= TargetTextBox_TextChanged;
                config.TargetTextBox.KeyDown -= TargetTextBox_KeyDown;
                config.TargetTextBox.LostFocus -= TargetTextBox_LostFocus;
                config.TargetTextBox.ReadOnlyChanged -= TargetTextBox_LostFocus;
            }
        }

        #endregion "Methods"

        #region "Structs"

        private class LiveBoxConfig
        {
            public TextBox TargetTextBox { get; }
            public string DisplayMember { get; }
            public LiveBoxSelectAction SelectionType { get; }
            public string ValueMember { get; }

            public LiveBoxConfig(TextBox targetTextBox, string displayMember, LiveBoxSelectAction selectAction, string valueMember)
            {
                this.TargetTextBox = targetTextBox;
                this.DisplayMember = displayMember;
                this.SelectionType = selectAction;
                this.ValueMember = valueMember;
            }
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
                    RemoveTargetEvents();
                    RemoveLiveBoxEvents();
                    liveListBox.Dispose();
                    liveBoxConfigs.Clear();
                    liveBoxFont.Dispose();
                    liveBoxGraphics.Dispose();
                }
            }
            disposedValue = true;
        }

        #endregion "IDisposable Support"
    }
}