using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Security;
using RemoteFileTransferTool;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.CustomControls
{
    public partial class FileTransferUI : UserControl, IDisposable
    {
        public RemoteTransfer remoteTransfer { get; }

        public ProgressStatus ProgStatus { get { return progStatus; } }

        public Device Device
        {
            get { return currentDevice; }
        }

        private ProgressStatus progStatus;
        private bool logVisible = false;
        private RemoteTransfer.TranferStatus currentStatus;
        private Device currentDevice;
        private string logBuffer = "";
        private Form parentForm;
        private Color prevStatusColor;
        private Bitmap statusLight;

        public event EventHandler CriticalStopError;

        public FileTransferUI(Form parentForm, Device targetDevice, bool createMissingDirs, string sourcePath, string destPath, string transferDescription, int seq = 0)
        {
            InitializeComponent();
            currentDevice = targetDevice;
            this.parentForm = parentForm;
            this.Disposed += GKProgressControl_Disposed;
            this.Size = this.MinimumSize;
            this.DoubleBuffered = true;
            Panel1.DoubleBuffered(true);
            LogTextBox.DoubleBuffered(true);

            remoteTransfer = new RemoteTransfer(currentDevice.HostName, sourcePath, destPath, transferDescription);
            remoteTransfer.CreateMissingDirectories = createMissingDirs;
            remoteTransfer.LogEvent += LogEvent;
            remoteTransfer.StatusUpdate += StatusUpdateEvent;
            remoteTransfer.TransferComplete += UpdateCompleteEvent;
            remoteTransfer.TransferCanceled += UpdateCanceledEvent;

            DeviceInfoLabel.Text = currentDevice.Serial + " - " + currentDevice.CurrentUser;
            TransferRateLabel.Text = "0.00MB/s";

            SetStatus(ProgressStatus.Queued);

            if (seq > 0)
            {
                SequenceLabel.Text = "#" + seq;
            }
            else
            {
                SequenceLabel.Text = "";
            }
        }

        public void CancelUpdate()
        {
            if (!remoteTransfer.IsDisposed) remoteTransfer.CancelTransfer();
        }

        public void StartUpdate()
        {
            try
            {
                if (progStatus != ProgressStatus.Running)
                {
                    logBuffer = "";
                    SetStatus(ProgressStatus.Starting);
                    DoUIUpdateLoop();
                    remoteTransfer.StartTransfer(SecurityTools.AdminCreds);
                }
            }
            catch (Exception ex)
            {
                SetStatus(ProgressStatus.Errors);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        protected virtual void OnCriticalStopError(EventArgs e)
        {
            if (CriticalStopError != null)
            {
                CriticalStopError(this, e);
            }
        }

        private void DrawLight(Color color)
        {
            if (color != prevStatusColor)
            {
                prevStatusColor = color;

                if (statusLight == null)
                {
                    statusLight = new Bitmap(StatusPictureBox.Width, StatusPictureBox.Height);
                }

                using (SolidBrush brush = new SolidBrush(color))
                using (Pen strokePen = new Pen(Color.Black, 1.5f))
                using (Graphics gr = Graphics.FromImage(statusLight))
                {
                    gr.Clear(StatusPictureBox.BackColor);
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                    float size = 20F;
                    float xLoc = Convert.ToSingle(StatusPictureBox.Width / 2 - size / 2); ;
                    float yLoc = Convert.ToSingle(StatusPictureBox.Height / 2 - size / 2);

                    gr.FillEllipse(brush, xLoc, yLoc, size, size);
                    gr.DrawEllipse(strokePen, xLoc, yLoc, size, size);

                    StatusPictureBox.Image = statusLight;
                }
            }
        }

        private void GKProgressControl_Disposed(object sender, EventArgs e)
        {
            remoteTransfer.LogEvent -= LogEvent;
            remoteTransfer.StatusUpdate -= StatusUpdateEvent;
            remoteTransfer.TransferComplete -= UpdateCompleteEvent;
            remoteTransfer.TransferCanceled -= UpdateCanceledEvent;
            remoteTransfer.Dispose();
            statusLight?.Dispose();
        }

        /// <summary>
        /// Log message event from GKUpdater.  This even can fire very rapidly. So the result is stored in a buffer to be added to the rtbLog control in a more controlled manner.
        /// </summary>
        private void LogEvent(object sender, EventArgs e)
        {
            var LogEvent = (RemoteTransfer.LogEventArgs)e;
            Log(LogEvent.Message.Message);
        }

        private void Log(string text)
        {
            logBuffer += text + Environment.NewLine;
        }

        private void StatusUpdateEvent(object sender, EventArgs e)
        {
            var UpdateEvent = (RemoteTransfer.TransferStatusEventArgs)e;
            SetStatus(ProgressStatus.Running);
            currentStatus = UpdateEvent.Status;
            TotalProgressBar.Maximum = currentStatus.TotalFileCount;
            TotalProgressBar.Value = currentStatus.CurrentFileIdx;
            StatusLabel.Text = currentStatus.DestinationFileName;
        }

        private void UpdateCanceledEvent(object sender, EventArgs e)
        {
            SetStatus(ProgressStatus.Canceled);
        }

        private void UpdateCompleteEvent(object sender, EventArgs e)
        {
            var CompleteEvent = (RemoteTransfer.TransferCompleteEventArgs)e;
            if (CompleteEvent.HasErrors)
            {
                SetStatus(ProgressStatus.Errors);
                // ErrorHandling.ErrHandle(CompleteEvent.Errors, System.Reflection.MethodInfo.GetCurrentMethod());

                if (CompleteEvent.Errors is Win32Exception)
                {
                    var err = (Win32Exception)CompleteEvent.Errors;
                    //Check for invalid credentials error and fire critical stop event.
                    //We want to stop all updates if the credentials are wrong as to avoid locking the account.

                    //TODO: Try to let these errors bubble up to ErrHandler.
                    if (err.NativeErrorCode == 1326 | err.NativeErrorCode == 86)
                    {
                        OnCriticalStopError(new EventArgs());
                    }
                }
                else
                {
                    if (CompleteEvent.Errors is RemoteTransfer.MissingDirectoryException)
                    {
                        Log("Enable 'Create Missing Directories' option and re-enqueue this device to force creation.");
                    }
                }
            }
            else
            {
                if (remoteTransfer.ErrorList.Count == 0)
                {
                    SetStatus(ProgressStatus.Complete);
                }
                else
                {
                    SetStatus(ProgressStatus.CompleteWithErrors);
                }
            }
        }

        private void HideLog()
        {
            this.Size = this.MinimumSize;
            logVisible = false;
            ShowHideLabel.Text = "s";
            //"+"
        }

        private void ShowLog()
        {
            UpdateLogBox();
            this.Size = this.MaximumSize;
            logVisible = true;
            ShowHideLabel.Text = "r";
            //"-"
        }

        private void InfoLabel_Click(object sender, EventArgs e)
        {
            ChildFormControl.LookupDevice(Helpers.ChildFormControl.MainFormInstance(), currentDevice);
        }

        private void ShowHideLabel_Click(object sender, EventArgs e)
        {
            if (!logVisible)
            {
                ShowLog();
            }
            else
            {
                HideLog();
            }
        }

        private void pbCancelClose_Click(object sender, EventArgs e)
        {
            if (progStatus == ProgressStatus.Running | progStatus == ProgressStatus.Paused)
            {
                if (!remoteTransfer.IsDisposed)
                {
                    remoteTransfer.CancelTransfer();
                    SetStatus(ProgressStatus.Canceled);
                }
                else
                {
                    this.Dispose();
                }
            }
            else
            {
                this.Dispose();
            }
        }

        private void pbRestart_Click(object sender, EventArgs e)
        {
            switch (progStatus)
            {
                case ProgressStatus.Paused:
                    remoteTransfer.ResumeTransfer();
                    SetStatus(ProgressStatus.Running);
                    break;

                case ProgressStatus.Running:
                    remoteTransfer.PauseTransfer();
                    SetStatus(ProgressStatus.Paused);
                    break;

                case ProgressStatus.Queued:
                    var blah = OtherFunctions.Message("This update is queued. Starting it may exceed the maximum concurrent updates. Are you sure you want to start it?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Warning", parentForm);
                    if (blah == DialogResult.Yes)
                    {
                        StartUpdate();
                    }
                    break;

                default:
                    StartUpdate();
                    break;
            }
        }

        private void SetStatus(ProgressStatus Status)
        {
            if (progStatus != Status)
            {
                progStatus = Status;
                SetStatusLight(Status);
                SetButtons(Status);
                SetStatusLabel(Status);
            }
        }

        private void SetStatusLight(ProgressStatus Status)
        {
            switch (Status)
            {
                case ProgressStatus.Running:
                case ProgressStatus.Starting:
                    DrawLight(Color.LimeGreen);
                    break;

                case ProgressStatus.Queued:
                case ProgressStatus.Paused:
                    DrawLight(Color.Yellow);
                    break;

                default:
                    DrawLight(Color.Red);
                    break;
            }
        }

        private void SetButtons(ProgressStatus Status)
        {
            switch (Status)
            {
                case ProgressStatus.Running:
                    RestartPictureBox.Image = Properties.Resources.PauseIcon;
                    ToolTip.SetToolTip(RestartPictureBox, "Pause");
                    break;

                case ProgressStatus.Paused:
                case ProgressStatus.Queued:
                    RestartPictureBox.Image = Properties.Resources.PlayIcon;
                    ToolTip.SetToolTip(RestartPictureBox, "Resume");
                    break;

                default:
                    RestartPictureBox.Image = Properties.Resources.RestartIcon;
                    ToolTip.SetToolTip(RestartPictureBox, "Restart");
                    break;
            }
        }

        private void SetStatusLabel(ProgressStatus Status)
        {
            switch (Status)
            {
                case ProgressStatus.Queued:
                    StatusLabel.Text = "Queued...";
                    break;

                case ProgressStatus.Canceled:
                    StatusLabel.Text = "Canceled!";
                    break;

                case ProgressStatus.Errors:
                    StatusLabel.Text = "ERROR!";
                    break;

                case ProgressStatus.CompleteWithErrors:
                    StatusLabel.Text = "Completed with errors: " + remoteTransfer.ErrorList.Count;
                    break;

                case ProgressStatus.Complete:
                    StatusLabel.Text = "Complete!";
                    break;

                case ProgressStatus.Starting:
                    StatusLabel.Text = "Starting...";
                    break;

                case ProgressStatus.Paused:
                    StatusLabel.Text = "Paused.";
                    break;
            }
        }

        private async Task DoUIUpdateLoop()
        {
            while (!this.IsDisposed)
            {
                if (logVisible)
                {
                    UpdateLogBox();
                }
                if (progStatus == ProgressStatus.Running)
                {
                    // Set the progress bar to a value over the actual, then immediately set to the actual value
                    // This overrides the Windows animation and makes the bar go to the intended value instantly.
                    if (remoteTransfer.TransferStatus.CurrentFileProgress < 100)
                        FileProgressBar.Value = remoteTransfer.TransferStatus.CurrentFileProgress + 1;
                    FileProgressBar.Value = remoteTransfer.TransferStatus.CurrentFileProgress;
                    TransferRateLabel.Text = remoteTransfer.TransferStatus.CurrentTransferRate.ToString("0.00") + "MB/s";
                }

                await Task.Delay(250);
            }

        }

        /// <summary>
        /// Timer that updates the rtbLog control with chunks of data from the log buffer.
        /// </summary>
        private void UI_Timer_Tick(object sender, EventArgs e)
        {
            if (logVisible & !string.IsNullOrEmpty(logBuffer))
            {
                UpdateLogBox();
            }
            if (progStatus == ProgressStatus.Running)
            {
                FileProgressBar.Value = remoteTransfer.TransferStatus.CurrentFileProgress;
                if (FileProgressBar.Value > 1) FileProgressBar.Value = FileProgressBar.Value - 1;
                //doing this bypasses the progressbar control animation. This way it doesn't lag behind and fills completely
                FileProgressBar.Value = remoteTransfer.TransferStatus.CurrentFileProgress;
                TransferRateLabel.Text = remoteTransfer.TransferStatus.CurrentTransferRate.ToString("0.00") + "MB/s";
                this.Update();
            }
        }

        private void UpdateLogBox()
        {
            if (!string.IsNullOrEmpty(logBuffer))
            {
                LogTextBox.AppendText(logBuffer);
                LogTextBox.Invalidate();
                logBuffer = "";
            }
        }
    }
}