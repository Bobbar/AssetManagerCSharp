using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.Gatekeeper
{
    public partial class GKUpdaterForm : ExtendedForm
    {
        private bool queueIsRunning = true;
        private int concurrentUpdates = 10;
        private BindingList<GKUpdate> updates = new BindingList<GKUpdate>();
        private GridState previousGridState = null;

        public GKUpdaterForm(ExtendedForm parentForm) : base(parentForm)
        {
            InitializeComponent();
            this.DisableDoubleBuffering();
            StatusGrid.DoubleBuffered(true);
            LogTextBox.DoubleBuffered(true);
            UpdateLogSplitter.DoubleBuffered(true);

            MaxUpdates.Value = concurrentUpdates;

            updates.ListChanged += Updates_ListChanged;

            StatusGrid.AutoGenerateColumns = false;
            StatusLightCol.DataPropertyName = nameof(GKUpdate.StatusLight);
            TargetNameCol.DataPropertyName = nameof(GKUpdate.TargetText);
            StatusTextCol.DataPropertyName = nameof(GKUpdate.StatusText);
            CancelRemoveCol.DataPropertyName = nameof(GKUpdate.CancelRemoveButtonText);
            StartRestartCol.DataPropertyName = nameof(GKUpdate.StartRestartButtonText);

            StatusGrid.DataSource = updates;

            DoQueueCheckerLoop();
        }

        public void AddMultipleUpdates(List<Device> devices)
        {
            try
            {
                Waiting();
                foreach (var device in devices)
                {
                    if (!Exists(device) && !string.IsNullOrEmpty(device.HostName))
                    {
                        var newUpdate = new GKUpdate(this, device, updates.Count + 1);
                        updates.Add(newUpdate);
                        newUpdate.CriticalStopError += CriticalStop;
                    }
                }

                ProcessUpdates();
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
            finally
            {
                DoneWaiting();
            }
        }

        public void AddUpdate(Device device)
        {
            try
            {
                if (!Exists(device) && !string.IsNullOrEmpty(device.HostName))
                {
                    var newUpdate = new GKUpdate(this, device, updates.Count + 1);
                    updates.Add(newUpdate);
                    newUpdate.CriticalStopError += CriticalStop;
                    ProcessUpdates();
                }
                else
                {
                    var blah = OtherFunctions.Message("An update for device " + device.Serial + " already exists.  Do you want to restart the update for this device?", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, "Duplicate Update", this);
                    if (blah == DialogResult.OK)
                    {
                        StartUpdateByDevice(device);
                    }
                }
            }
            finally
            {
                this.Show();
                this.WindowState = FormWindowState.Normal;
                this.Activate();
            }
        }

        private void StartUpdateByDevice(Device device)
        {
            updates.Find(upd => upd.Device.Guid == device.Guid).StartUpdate();
        }

        private bool Exists(Device device)
        {
            return updates.Exists(upd => upd.Device.Guid == device.Guid);
        }

        public bool ActiveUpdates()
        {
            return updates.Exists(upd => upd.Status == UpdateStatus.Running | upd.Status == UpdateStatus.Starting);
        }

        private void CancelAll()
        {
            updates.ForEach(upd => { if (upd.Status == UpdateStatus.Running) upd.CancelUpdate(); });
        }

        private void DisposeUpdates()
        {
            updates.Clear();
        }

        /// <summary>
        /// Returns True if number of running updates is less than the maximum simultaneous allowed updates and RunQueue is True.
        /// </summary>
        /// <returns></returns>
        private bool CanRunMoreUpdates()
        {
            if (queueIsRunning)
            {
                int runningUpdates = 0;
                foreach (var upd in updates)
                {
                    switch (upd.Status)
                    {
                        case UpdateStatus.Running:
                        case UpdateStatus.Starting:
                            runningUpdates += 1;
                            break;
                    }
                }
                if (runningUpdates < concurrentUpdates)
                    return true;
            }
            return false;
        }

        private void CancelAllButton_Click(object sender, EventArgs e)
        {
            CancelAll();
            StopQueue();
        }

        private void PauseResumeButton_Click(object sender, EventArgs e)
        {
            if (queueIsRunning)
            {
                StopQueue();
            }
            else
            {
                StartQueue();
            }
        }

        private async void CriticalStop(object sender, EventArgs e)
        {
            StopQueue();
            OtherFunctions.Message("The queue was stopped because of an access error. Please re-enter your credentials.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Queue Stopped", this);
            SecurityTools.ClearAdminCreds();

            if (await SecurityTools.VerifyAdminCreds())
            {
                StartQueue();
            }
        }

        public override bool OkToClose()
        {
            if (ActiveUpdates())
            {
                OtherFunctions.Message("There are still updates running!  Cancel the updates or wait for them to finish.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Close Aborted", this);
                this.Activate();
                this.WindowState = FormWindowState.Normal;
                return false;
            }
            return true;
        }

        private void MaxUpdates_ValueChanged(object sender, EventArgs e)
        {
            concurrentUpdates = (int)MaxUpdates.Value;
        }

        private void ProcessUpdates()
        {
            if (CanRunMoreUpdates())
            {
                StartNextUpdate();
            }
            SetStats();
        }

        private async Task DoQueueCheckerLoop()
        {
            while (!this.IsDisposed)
            {
                ProcessUpdates();
                await Task.Delay(250);
            }
        }

        private void SetStats()
        {
            int queued = 0;
            int running = 0;
            int complete = 0;

            foreach (var upd in updates)
            {
                switch (upd.Status)
                {
                    case UpdateStatus.Queued:
                        queued += 1;
                        break;

                    case UpdateStatus.Running:
                        running += 1;
                        break;

                    case UpdateStatus.Done:
                        complete += 1;
                        break;
                }
            }

            QueuedUpdatesLabel.Text = "Queued: " + queued;
            RunningUpdatesLabel.Text = "Running: " + running;
            CompleteUpdatesLabel.Text = "Complete: " + complete;
            TotalUpdatesLabel.Text = "Tot Updates: " + updates.Count;
        }

        /// <summary>
        /// Starts the next update that has a queued status.
        /// </summary>
        private void StartNextUpdate()
        {
            var nextUpdate = updates.Find(upd => upd.Status == UpdateStatus.Queued);
            if (nextUpdate != null) nextUpdate.StartUpdate();
        }

        private void StartQueue()
        {
            queueIsRunning = true;
            SetQueueButton();
        }

        private void StopQueue()
        {
            queueIsRunning = false;
            SetQueueButton();
        }

        private void SetQueueButton()
        {
            if (queueIsRunning)
            {
                PauseResumeButton.Text = "Pause Queue";
            }
            else
            {
                PauseResumeButton.Text = "Resume Queue";
            }
        }

        private void SetCurrentLog()
        {
            if (StatusGrid.CurrentRow != null)
            {
                // Set the binding for the log textbox to the current selected update.
                var selectedUpdate = (GKUpdate)StatusGrid.CurrentRow.DataBoundItem;
                LogTextBox.DataBindings.Clear();
                LogTextBox.DataBindings.Add("Text", selectedUpdate, nameof(GKUpdate.LogMessages));

                // Set the log checkbox column to reflect the currently displayed log.
                foreach (DataGridViewRow row in StatusGrid.Rows)
                {
                    row.Cells[nameof(LogViewCol)].Value = false;
                }

                StatusGrid.CurrentRow.Cells[nameof(LogViewCol)].Value = true;
            }
        }

        private void GKUpdaterForm_Shown(object sender, EventArgs e)
        {
            SetQueueButton();
        }

        private void LogTextBox_TextChanged(object sender, EventArgs e)
        {
            // Scroll to the bottom then resume drawing.
            LogTextBox.SelectionStart = LogTextBox.Text.Length;
            LogTextBox.ScrollToCaret();
            LogTextBox.Resume();
            LogTextBox.ResumeLayout();
        }

        private void ViewDeviceMenuItem_Click(object sender, EventArgs e)
        {
            if (StatusGrid.CurrentRow != null)
            {
                var selectedUpdate = (GKUpdate)StatusGrid.CurrentRow.DataBoundItem;
                ChildFormControl.LookupDevice(Helpers.ChildFormControl.MainFormInstance(), selectedUpdate.Device);
            }
        }

        private void Updates_ListChanged(object sender, ListChangedEventArgs e)
        {
            // Save the state of the grid when the bound list is changing.
            previousGridState = new GridState(StatusGrid);

            // Only suspend the log textbox if it's bound proptery is changing.
            if (e.PropertyDescriptor != null && LogTextBox.DataBindings.Count > 0)
            {
                if (e.PropertyDescriptor.Name == LogTextBox.DataBindings[0].BindingMemberInfo.BindingMember)
                {
                    // Suspend drawing and layout to reduce flicker.
                    LogTextBox.Suspend();
                    LogTextBox.SuspendLayout();
                }
            }
        }

        private void StatusGrid_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Restore the grid state after binding has updated.
            // This keeps the selected item, and current scroll location from changing everytime an item is updated.
            previousGridState?.RestoreState();
            previousGridState = null;
        }

        private void StatusGrid_CellMouseUp(object sender, DataGridViewCellMouseEventArgs e)
        {
            SetCurrentLog();
        }

        private void StatusGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (e.RowIndex >= 0)
                {
                    StatusGrid.Rows[e.RowIndex].Selected = true;
                    StatusGrid.CurrentCell = StatusGrid[e.ColumnIndex, e.RowIndex];
                }
            }
        }

        private void StatusGrid_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            // Handle item button clicks.
            var senderGrid = (DataGridView)sender;

            if (senderGrid.Columns[e.ColumnIndex] is DataGridViewButtonColumn && e.RowIndex >= 0)
            {
                var selectedUpdate = (GKUpdate)senderGrid.CurrentCell.OwningRow.DataBoundItem;

                if (senderGrid.CurrentCell.OwningColumn == StartRestartCol)
                {
                    if (selectedUpdate.Status != UpdateStatus.Running && selectedUpdate.Status != UpdateStatus.Starting)
                        selectedUpdate.StartRestart();
                }
                else if (senderGrid.CurrentCell.OwningColumn == CancelRemoveCol)
                {
                    if (selectedUpdate.Status != UpdateStatus.Running && selectedUpdate.Status != UpdateStatus.Starting)
                    {
                        updates.Remove(selectedUpdate);
                        LogTextBox.DataBindings.Clear();
                        LogTextBox.Clear();
                    }
                    else if (selectedUpdate.Status == UpdateStatus.Running || selectedUpdate.Status == UpdateStatus.Starting)
                    {
                        selectedUpdate.CancelUpdate();
                    }
                }
            }
        }

        private void StatusGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            // Handle sorting.
            var column = StatusGrid.Columns[e.ColumnIndex];

            if (column.SortMode != DataGridViewColumnSortMode.Programmatic)
                return;

            var sortGlyph = column.HeaderCell.SortGlyphDirection;

            switch (sortGlyph)
            {
                case SortOrder.None:
                case SortOrder.Ascending:
                    updates = updates.Sort(SortOrder.Descending, column.DataPropertyName);
                    updates.ResetBindings();
                    StatusGrid.DataSource = updates;
                    column.HeaderCell.SortGlyphDirection = SortOrder.Descending;
                    break;

                case SortOrder.Descending:
                    updates = updates.Sort(SortOrder.Ascending, column.DataPropertyName);
                    updates.ResetBindings();
                    StatusGrid.DataSource = updates;
                    column.HeaderCell.SortGlyphDirection = SortOrder.Ascending;
                    break;
            }
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }

                    DisposeUpdates();
                    updates.ListChanged -= Updates_ListChanged;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}