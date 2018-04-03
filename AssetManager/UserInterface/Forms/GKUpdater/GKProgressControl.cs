using GKUpdaterLib;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using AssetManager.Data.Classes;
using AssetManager.Tools;
using AssetManager.Helpers;
using AssetManager.Security;


namespace AssetManager.UserInterface.Forms.GKUpdater
{
    public partial class GKProgressControl : IDisposable
    {
        public GKUpdaterLibClass MyUpdater;
        public ProgressStatus ProgStatus;
        private bool logVisible = false;
        private GKUpdaterLibClass.Status_Stats currentStatus;
        private Device currentDevice;
        private string logBuffer = "";
        private Form MyParentForm;
        private Color prevColor;
        private Bitmap statusLight;

        public Device Device
        {
            get { return currentDevice; }
        }

        public GKProgressControl()
        {
            Disposed += GK_Progress_Fragment_Disposed;
            // This call is required by the designer.
            InitializeComponent();

            // Add any initialization after the InitializeComponent() call.
        }

        public GKProgressControl(Form parentForm, Device device, bool createMissingDirs, string gkPath, int seq = 0)
        {
            Disposed += GK_Progress_Fragment_Disposed;
            InitializeComponent();
             this.Size = this.MinimumSize;
            MyParentForm = parentForm;
            currentDevice = device;
            MyUpdater = new GKUpdaterLibClass(currentDevice.HostName, gkPath);
            MyUpdater.CreateMissingDirectories = createMissingDirs;
            this.DoubleBuffered = true;
            lblInfo.Text = currentDevice.Serial + " - " + currentDevice.CurrentUser;
            lblTransRate.Text = "0.00MB/s";
            SetStatus(ProgressStatus.Queued);
            if (seq > 0)
            {
                lblSeq.Text = "#" + seq;
            }
            else
            {
                lblSeq.Text = "";
            }
            MyUpdater.LogEvent += GKLogEvent;
            MyUpdater.StatusUpdate += GKStatusUpdateEvent;
            MyUpdater.UpdateComplete += GKUpdate_Complete;
            MyUpdater.UpdateCanceled += GKUpdate_Cancelled;
            Panel1.DoubleBuffered(true);
        }

        public GKProgressControl(Form parentForm, Device device, bool createMissingDirs, string sourcePath, string destPath, int seq = 0)
        {
            Disposed += GK_Progress_Fragment_Disposed;
            InitializeComponent();
            this.Size = this.MinimumSize;
            MyParentForm = parentForm;
            currentDevice = device;
            MyUpdater = new GKUpdaterLibClass(currentDevice.HostName, sourcePath, destPath);
            MyUpdater.CreateMissingDirectories = createMissingDirs;
            this.DoubleBuffered = true;
            lblInfo.Text = currentDevice.Serial + " - " + currentDevice.CurrentUser;
            lblTransRate.Text = "0.00MB/s";
            SetStatus(ProgressStatus.Queued);
            if (seq > 0)
            {
                lblSeq.Text = "#" + seq;
            }
            else
            {
                lblSeq.Text = "";
            }
            MyUpdater.LogEvent += GKLogEvent;
            MyUpdater.StatusUpdate += GKStatusUpdateEvent;
            MyUpdater.UpdateComplete += GKUpdate_Complete;
            MyUpdater.UpdateCanceled += GKUpdate_Cancelled;
            Panel1.DoubleBuffered(true);
        }

        public event EventHandler CriticalStopError;

        // METODO: Un-nest.
        public enum ProgressStatus
        {
            Starting,
            Running,
            Paused,
            Queued,
            Complete,
            CompleteWithErrors,
            Canceled,
            Errors
        }

        public void CancelUpdate()
        {
            if (!MyUpdater.IsDisposed) MyUpdater.CancelUpdate();
        }

        public void StartUpdate()
        {
            try
            {
                if (ProgStatus != ProgressStatus.Running)
                {
                    logBuffer = "";
                    SetStatus(ProgressStatus.Starting);
                    MyUpdater.StartUpdate(SecurityTools.AdminCreds);
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
            if (color != prevColor)
            {
                prevColor = color;

                if (statusLight == null)
                {
                    statusLight = new Bitmap(pbStatus.Width, pbStatus.Height);
                }

                using (SolidBrush MyBrush = new SolidBrush(color))
                using (Pen StrokePen = new Pen(Color.Black, 1.5f))
                using (Graphics gr = Graphics.FromImage(statusLight))
                {
                    gr.Clear(pbStatus.BackColor);
                    gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
                    float XLoc = 0;
                    float YLoc = 0;
                    float Size = 0;
                    Size = 20;
                    XLoc = Convert.ToSingle(pbStatus.Width / 2 - Size / 2);
                    YLoc = Convert.ToSingle(pbStatus.Height / 2 - Size / 2);
                    gr.FillEllipse(MyBrush, XLoc, YLoc, Size, Size);
                    gr.DrawEllipse(StrokePen, XLoc, YLoc, Size, Size);
                    pbStatus.Image = statusLight;
                }
            }
        }

        private void GK_Progress_Fragment_Disposed(object sender, EventArgs e)
        {
            MyUpdater.LogEvent -= GKLogEvent;
            MyUpdater.StatusUpdate -= GKStatusUpdateEvent;
            MyUpdater.UpdateComplete -= GKUpdate_Complete;
            MyUpdater.UpdateCanceled -= GKUpdate_Cancelled;
            MyUpdater.Dispose();
            statusLight?.Dispose();
        }

        /// <summary>
        /// Log message event from GKUpdater.  This even can fire very rapidly. So the result is stored in a buffer to be added to the rtbLog control in a more controlled manner.
        /// </summary>
        private void GKLogEvent(object sender, EventArgs e)
        {
            var LogEvent = (GKUpdaterLibClass.LogEvents)e;
            Log(LogEvent.LogData.Message);
        }

        private void Log(string Message)
        {
            logBuffer += Message + Environment.NewLine;
        }

        private void GKStatusUpdateEvent(object sender, EventArgs e)
        {
            var UpdateEvent = (GKUpdaterLibClass.GKUpdateEvents)e;
            SetStatus(ProgressStatus.Running);
            currentStatus = UpdateEvent.CurrentStatus;
            pbarProgress.Maximum = currentStatus.TotFiles;
            pbarProgress.Value = currentStatus.CurFileIdx;
            lblStatus.Text = currentStatus.CurFileName;
        }

        private void GKUpdate_Cancelled(object sender, EventArgs e)
        {
            SetStatus(ProgressStatus.Canceled);
        }

        private void GKUpdate_Complete(object sender, EventArgs e)
        {
            var CompleteEvent = (GKUpdaterLibClass.GKUpdateCompleteEvents)e;
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
                    if (CompleteEvent.Errors is GKUpdaterLibClass.MissingDirectoryException)
                    {
                        Log("Enable 'Create Missing Directories' option and re-enqueue this device to force creation.");
                    }
                }
            }
            else
            {
                if (MyUpdater.ErrorList.Count == 0)
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
            lblShowHide.Text = "s";
            //"+"
        }

        private void ShowLog()
        {
            UpdateLogBox();
            this.Size = this.MaximumSize;
            logVisible = true;
            lblShowHide.Text = "r";
            //"-"
        }

        private void lblInfo_Click(object sender, EventArgs e)
        {
            Helpers.ChildFormControl.LookupDevice(Helpers.ChildFormControl.MainFormInstance(), currentDevice);
        }

        private void lblShowHide_Click(object sender, EventArgs e)
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
            if (ProgStatus == ProgressStatus.Running | ProgStatus == ProgressStatus.Paused)
            {
                if (!MyUpdater.IsDisposed)
                {
                    MyUpdater.CancelUpdate();
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
            switch (ProgStatus)
            {
                case ProgressStatus.Paused:
                    MyUpdater.ResumeUpdate();
                    SetStatus(ProgressStatus.Running);
                    break;

                case ProgressStatus.Running:
                    MyUpdater.PauseUpdate();
                    SetStatus(ProgressStatus.Paused);
                    break;

                case ProgressStatus.Queued:
                    var blah = OtherFunctions.Message("This update is queued. Starting it may exceed the maximum concurrent updates. Are you sure you want to start it?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Warning", MyParentForm);
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
            if (ProgStatus != Status)
            {
                ProgStatus = Status;
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
                    pbRestart.Image = Properties.Resources.PauseIcon;
                    MyToolTip.SetToolTip(pbRestart, "Pause");
                    break;

                case ProgressStatus.Paused:
                case ProgressStatus.Queued:
                    pbRestart.Image = Properties.Resources.PlayIcon;
                    MyToolTip.SetToolTip(pbRestart, "Resume");
                    break;

                default:
                    pbRestart.Image = Properties.Resources.RestartIcon;
                    MyToolTip.SetToolTip(pbRestart, "Restart");
                    break;
            }
        }

        private void SetStatusLabel(ProgressStatus Status)
        {
            switch (Status)
            {
                case ProgressStatus.Queued:
                    lblStatus.Text = "Queued...";
                    break;

                case ProgressStatus.Canceled:
                    lblStatus.Text = "Canceled!";
                    break;

                case ProgressStatus.Errors:
                    lblStatus.Text = "ERROR!";
                    break;

                case ProgressStatus.CompleteWithErrors:
                    lblStatus.Text = "Completed with errors: " + MyUpdater.ErrorList.Count;
                    break;

                case ProgressStatus.Complete:
                    lblStatus.Text = "Complete!";
                    break;

                case ProgressStatus.Starting:
                    lblStatus.Text = "Starting...";
                    break;

                case ProgressStatus.Paused:
                    lblStatus.Text = "Paused.";
                    break;
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
            if (ProgStatus == ProgressStatus.Running)
            {
                pbarFileProgress.Value = MyUpdater.UpdateStatus.CurFileProgress;
                if (pbarFileProgress.Value > 1)
                    pbarFileProgress.Value = pbarFileProgress.Value - 1;
                //doing this bypasses the progressbar control animation. This way it doesn't lag behind and fills completely
                pbarFileProgress.Value = MyUpdater.UpdateStatus.CurFileProgress;
                lblTransRate.Text = MyUpdater.UpdateStatus.CurTransferRate.ToString("0.00") + "MB/s";
                this.Update();
            }
        }

        private void UpdateLogBox()
        {
            rtbLog.AppendText(logBuffer);
            rtbLog.Refresh();
            logBuffer = "";
        }
    }
}