using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.Gatekeeper
{
    public partial class GKUpdaterForm : ExtendedForm
    {
        private bool queueIsRunning = false;
        private int concurrentUpdates = 4;
        private List<FileTransferUI> progressControls = new List<FileTransferUI>();
        private bool createMissingDirs = false;
        private bool packFileReady = false;

        public GKUpdaterForm(ExtendedForm parentForm) : base(parentForm)
        {
            InitializeComponent();
            this.DisableDoubleBuffering();
            MaxUpdates.Value = concurrentUpdates;
            ProgressControlsTable.DoubleBuffered(true);
            DoQueueCheckerLoop();
        }

        public void AddMultipleUpdates(List<Device> devices)
        {
            try
            {
                Waiting();
                foreach (var device in devices)
                {
                    if (!Exists(device))
                    {
                        var newProgCtl = new FileTransferUI(this, device, createMissingDirs, Paths.GKExtractDir, Paths.GKRemoteDir, "Gatekeeper Update", progressControls.Count + 1);
                        progressControls.Add(newProgCtl);
                        newProgCtl.CriticalStopError += CriticalStop;
                    }
                }

                ProgressControlsTable.SuspendLayout();
                this.SuspendLayout();

                ProgressControlsTable.Controls.AddRange(progressControls.ToArray());

                ProgressControlsTable.ResumeLayout();
                this.ResumeLayout();

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
                if (!Exists(device))
                {
                    var newProgCtl = new FileTransferUI(this, device, createMissingDirs, Paths.GKExtractDir, Paths.GKRemoteDir, "Gatekeeper Update", progressControls.Count + 1);
                    ProgressControlsTable.Controls.Add(newProgCtl);
                    progressControls.Add(newProgCtl);
                    newProgCtl.CriticalStopError += CriticalStop;
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
            progressControls.Find(upd => upd.Device.Guid == device.Guid).StartUpdate();
        }

        private bool Exists(Device device)
        {
            return progressControls.Exists(upd => upd.Device.Guid == device.Guid);
        }

        public bool ActiveUpdates()
        {
            return progressControls.Exists(upd => upd.ProgStatus == ProgressStatus.Running | upd.ProgStatus == ProgressStatus.Paused);
        }

        private void CancelAll()
        {
            foreach (FileTransferUI upd in progressControls)
            {
                if (upd.ProgStatus == ProgressStatus.Running)
                {
                    upd.CancelUpdate();
                }
            }
        }

        private void DisposeUpdates()
        {
            foreach (FileTransferUI upd in progressControls)
            {
                if (!upd.IsDisposed)
                {
                    upd.Dispose();
                }
            }
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
                foreach (FileTransferUI upd in progressControls)
                {
                    switch (upd.ProgStatus)
                    {
                        case ProgressStatus.Running:
                        case ProgressStatus.Starting:
                        case ProgressStatus.Paused:
                            if (!upd.IsDisposed)
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

        private void SortButton_Click(object sender, EventArgs e)
        {
            SortUpdates();
        }

        private void CriticalStop(object sender, EventArgs e)
        {
            StopQueue();
            OtherFunctions.Message("The queue was stopped because of an access error. Please re-enter your credentials.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Queue Stopped", this);
            SecurityTools.ClearAdminCreds();

            if (SecurityTools.VerifyAdminCreds())
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
            PruneQueue();
            SetStats();
        }

        /// <summary>
        /// Removes disposed update fragments from list.
        /// </summary>
        private void PruneQueue()
        {
            progressControls = progressControls.FindAll(upd => !upd.IsDisposed);
        }

        private async Task DoQueueCheckerLoop()
        {
            while (!this.IsDisposed)
            {
                ProcessUpdates();
                await Task.Delay(250);
            }
        }

        private void QueueChecker_Tick(object sender, EventArgs e)
        {
            ProcessUpdates();
        }

        private void SetStats()
        {
            int queued = 0;
            int running = 0;
            int complete = 0;
            double transferRateSum = 0;

            foreach (FileTransferUI upd in progressControls)
            {
                switch (upd.ProgStatus)
                {
                    case ProgressStatus.Queued:
                        queued += 1;
                        break;

                    case ProgressStatus.Running:
                        transferRateSum += upd.remoteTransfer.TransferStatus.CurrentTransferRate;
                        running += 1;
                        break;

                    case ProgressStatus.Complete:
                        complete += 1;
                        break;
                }
            }

            QueuedUpdatesLabel.Text = "Queued: " + queued;
            RunningUpdatesLabel.Text = "Running: " + running;
            CompleteUpdatesLabel.Text = "Complete: " + complete;
            TotalUpdatesLabel.Text = "Tot Updates: " + progressControls.Count;
            TransferRateLabel.Text = "Total Transfer Rate: " + transferRateSum.ToString("0.00") + " MB/s";
        }

        /// <summary>
        /// Sorts all the GKProgressControls in order of the <see cref="ProgressStatus"/> enum.
        /// </summary>
        private void SortUpdates()
        {
            var sortUpdates = new List<FileTransferUI>();

            foreach (ProgressStatus status in Enum.GetValues(typeof(ProgressStatus)))
            {
                sortUpdates.AddRange(progressControls.FindAll(upd => upd.ProgStatus == status));
            }

            ProgressControlsTable.Controls.Clear();
            ProgressControlsTable.Controls.AddRange(sortUpdates.ToArray());
        }

        /// <summary>
        /// Starts the next update that has a queued status.
        /// </summary>
        private void StartNextUpdate()
        {
            var nextUpdate = progressControls.Find(upd => upd.ProgStatus == ProgressStatus.Queued);
            if (nextUpdate != null) nextUpdate.StartUpdate();
        }

        private void RunQueue(bool canRunQueue)
        {
            if (canRunQueue)
            {
                StartQueue();
            }
            else
            {
                StopQueue();
            }
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

        private void CreateDirsToolItem_Click(object sender, EventArgs e)
        {
            createMissingDirs = CreateDirsToolItem.Checked;
        }

        private async void GKUpdaterForm_Shown(object sender, EventArgs e)
        {
            SetQueueButton();
            await CheckPackFile();
        }

        private void ProcessPackFile()
        {
            using (var packFileForm = new PackFileForm(false))
            {
                packFileForm.ShowDialog(this);
                packFileReady = packFileForm.PackVerified;
                RunQueue(packFileReady);
            }
        }

        private async Task<bool> CheckPackFile()
        {
            var packFileManager = new ManagePackFile();
            packFileReady = await packFileManager.VerifyPackFile();

            RunQueue(packFileReady);

            if (!packFileReady)
            {
                CancelAll();
                OtherFunctions.Message("The local pack file does not match the server. All running updates will be stopped and a new copy will now be downloaded and unpacked.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Pack file out of date", this);
                ProcessPackFile();
            }
            return true;
        }

        private void VerifyPackFileToolItem_Click(object sender, EventArgs e)
        {
            if (!Helpers.ChildFormControl.FormTypeIsOpen(typeof(PackFileForm)))
            {
                var newPackFileForm = new PackFileForm(true);
                newPackFileForm.Show();
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
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}