using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Tools;
using System;
using System.ComponentModel;
using System.Drawing;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.Gatekeeper
{
    public partial class UpdaterControl : UserControl, IDisposable
    {
        public UpdateStatus Status { get { return currentStatus; } }

        public Device Device
        {
            get { return currentDevice; }
        }

        private UpdateStatus currentStatus;
        private bool logVisible = false;
        private Device currentDevice;
        private Form parentForm;
        private Color prevStatusColor;
        private Bitmap statusLight;
        private PSExecWrapper psExec;

        public event EventHandler CriticalStopError;

        protected virtual void OnCriticalStopError(EventArgs e)
        {
            if (CriticalStopError != null)
            {
                CriticalStopError(this, e);
            }
        }

        public UpdaterControl(Form parentForm, Device targetDevice, int seq = 0)
        {
            InitializeComponent();
            currentDevice = targetDevice;
            this.parentForm = parentForm;
            this.Size = this.MinimumSize;
            this.DoubleBuffered = true;
            Panel1.DoubleBuffered(true);
            LogTextBox.DoubleBuffered(true);

            psExec = new PSExecWrapper(targetDevice.HostName);
            psExec.ErrorReceived += PsExec_ErrorReceived;
            psExec.OutputReceived += PsExec_OutputReceived;

            DeviceInfoLabel.Text = currentDevice.Serial + " - " + currentDevice.CurrentUser;

            SetStatus(UpdateStatus.Queued);

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
            psExec.StopProcess();
            SetStatus(UpdateStatus.Canceled);
        }

        public void StartUpdate()
        {
            try
            {
                if (currentStatus != UpdateStatus.Running)
                {
                    SetStatus(UpdateStatus.Starting);
                    BeginUpdate();
                }
            }
            catch (Exception ex)
            {
                SetStatus(UpdateStatus.Error);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public async void BeginUpdate()
        {
            try
            {
                int exitCode;
                var elapTimer = new System.Diagnostics.Stopwatch();
                elapTimer.Start();

                SetStatus(UpdateStatus.Starting);

                // Check for ping.
                if (!await CanPing(currentDevice.HostName))
                {
                    SetStatus(UpdateStatus.Error, "Cannot ping");
                    Log("Cannot ping target device.");
                    return;
                }

                Log("Executing Update...");

                // Collect command strings.
                var copyCommand = AssetManagerFunctions.GetDeployString("gatekeeper_update_copy");
                var extractCommand = AssetManagerFunctions.GetDeployString("gatekeeper_update_extract");
                var deleteTempCommand = AssetManagerFunctions.GetDeployString("gatekeeper_delete_temp");
                var existsCommand = AssetManagerFunctions.GetDeployString("gatekeeper_exists");

                // Make sure Gatekeeper is actually installed.
                SetStatus(UpdateStatus.Running, "Checking Previous Installation");

                exitCode = await psExec.ExecuteRemoteCommand(existsCommand);

                if (exitCode != 0)
                {
                    Log("Gatekeeper not installed on target device!");
                    SetStatus(UpdateStatus.Error);
                    return;
                }

                // Copy deployment files to temp directory on target.
                SetStatus(UpdateStatus.Running, "Copying Files");
                Log("Copying files...");

                exitCode = await psExec.ExecuteRemoteCommand(copyCommand);

                Log("Exit Code: " + exitCode);

                if (exitCode != 0)
                {
                    Log("Error while copying files!");
                    SetStatus(UpdateStatus.Error, "Could Not Copy Files");
                    return;
                }

                // Extract the updated files to the Gatekeeper directory, overwritting.
                SetStatus(UpdateStatus.Running, "Extracting Files");
                Log("Extracting update...");

                exitCode = await psExec.ExecuteRemoteCommand(extractCommand);

                Log("Exit Code: " + exitCode);

                if (exitCode != 0)
                {
                    Log("Update failed!");
                    SetStatus(UpdateStatus.Error, "Error While Extracting");
                    return;
                }

                // Delete the temp directory.
                SetStatus(UpdateStatus.Running, "Deleting Temp Directory");
                Log("Deleting temp directory...");

                exitCode = await psExec.ExecuteRemoteCommand(deleteTempCommand);

                Log("Exit Code: " + exitCode);

                if (exitCode != 0)
                {
                    Log("Error deleting temp directory!");
                    SetStatus(UpdateStatus.Error, "Error Deleting Temp Direcory");
                    return;
                }

                SetStatus(UpdateStatus.Done, "No Errors");
                elapTimer.Stop();
                Log("Update completed successfully.");
                Log(string.Format("Elapsed time: {0} s", elapTimer.Elapsed.Seconds));
            }
            catch (Win32Exception)
            {
                OnCriticalStopError(new EventArgs());
            }
            catch (Exception ex)
            {
                SetStatus(UpdateStatus.Error);
                Log("Error: " + ex.ToString());
            }
        }

        private async Task<bool> CanPing(string hostname)
        {
            var ping = new Ping();
            var reply = await Task.Run(() => { return ping.Send(hostname); });

            if (reply.Status == IPStatus.Success)
            {
                return true;
            }
            return false;
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

        private void Log(string text)
        {
            if (LogTextBox.InvokeRequired)
            {
                LogTextBox.BeginInvoke(new Action(() => Log(text)));
            }
            else
            {
                LogTextBox.AppendText(text + Environment.NewLine);
                LogTextBox.Invalidate();
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
            this.Size = this.MaximumSize;
            logVisible = true;
            ShowHideLabel.Text = "r";
            //"-"
        }

        private void SetStatus(UpdateStatus Status, string message = null)
        {
            currentStatus = Status;
            SetStatusLight(Status);
            SetStatusLabel(Status, message);
        }

        private void SetStatusLight(UpdateStatus Status)
        {
            switch (Status)
            {
                case UpdateStatus.Running:
                case UpdateStatus.Starting:
                    DrawLight(Color.LimeGreen);
                    break;

                case UpdateStatus.Queued:
                    DrawLight(Color.Yellow);
                    break;

                default:
                    DrawLight(Color.Red);
                    break;
            }
        }

        private void SetStatusLabel(UpdateStatus Status, string message = null)
        {
            switch (Status)
            {
                case UpdateStatus.Queued:
                    StatusLabel.Text = "Queued...";
                    break;

                case UpdateStatus.Starting:
                    StatusLabel.Text = "Starting...";
                    break;

                case UpdateStatus.Running:
                    StatusLabel.Text = "Running.";
                    break;

                case UpdateStatus.Canceled:
                    StatusLabel.Text = "Canceled.";
                    break;

                case UpdateStatus.Done:
                    StatusLabel.Text = "Done.";
                    break;

                case UpdateStatus.Error:
                    StatusLabel.Text = "ERROR!";
                    break;
            }

            if (message != null)
            {
                StatusLabel.Text += " - " + message;
            }
        }

        private void PsExec_OutputReceived(object sender, string e)
        {
            Log("Output: " + e);
        }

        private void PsExec_ErrorReceived(object sender, string e)
        {
            Log("Error: " + e);
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
            if (currentStatus == UpdateStatus.Running | currentStatus == UpdateStatus.Starting)
            {
                if (psExec.CurrentProcess != null)
                {
                    psExec.StopProcess();
                    SetStatus(UpdateStatus.Canceled);
                }
            }
            else
            {
                this.Dispose();
            }
        }

        private void pbRestart_Click(object sender, EventArgs e)
        {
            switch (currentStatus)
            {
                case UpdateStatus.Done:
                case UpdateStatus.Error:
                    StartUpdate();
                    break;

                case UpdateStatus.Running:
                    OtherFunctions.Message("Cannot restart an update that is currently running.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Already running!", parentForm);
                    break;

                case UpdateStatus.Queued:
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

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    //  remoteTransfer.Dispose();
                    statusLight?.Dispose();
                    components.Dispose();
                    psExec.ErrorReceived -= PsExec_ErrorReceived;
                    psExec.OutputReceived -= PsExec_OutputReceived;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}