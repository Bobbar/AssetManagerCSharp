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
    public sealed class GKUpdate : INotifyPropertyChanged
    {
        public UpdateStatus Status { get { return currentStatus; } }

        public Bitmap StatusLight
        {
            get { return statusLight; }

            set
            {
                statusLight = value;
                OnPropertyChanged(nameof(StatusLight));
            }
        }

        public string TargetText
        {
            get { return targetText; }

            set
            {
                if (targetText != value)
                {
                    targetText = value;
                    OnPropertyChanged(nameof(TargetText));
                }
            }
        }

        public string StatusText
        {
            get { return statusText; }

            set
            {
                if (statusText != value)
                {
                    statusText = value;
                    OnPropertyChanged(nameof(StatusText));
                }
            }
        }

        public int Sequence { get; set; }

        public string CancelRemoveButtonText
        {
            get { return cancelRemoveButtonText; }

            set
            {
                if (cancelRemoveButtonText != value)
                {
                    cancelRemoveButtonText = value;
                    OnPropertyChanged(nameof(CancelRemoveButtonText));
                }
            }
        }

        public string StartRestartButtonText
        {
            get { return startRestartButtonText; }

            set
            {
                if (startRestartButtonText != value)
                {
                    startRestartButtonText = value;
                    OnPropertyChanged(nameof(StartRestartButtonText));
                }
            }
        }

        public string LogMessages
        {
            get { return logMessages; }

            set
            {
                if (logMessages != value)
                {
                    logMessages = value;
                    OnPropertyChanged(nameof(LogMessages));
                }
            }
        }

        public Device Device
        {
            get { return targetDevice; }
        }

        private Bitmap statusLight;
        private string targetText;
        private string statusText;
        private string cancelRemoveButtonText;
        private string startRestartButtonText;
        private string logMessages;

        private string hostname;
        private Device targetDevice;
        private Form parentForm;
        private UpdateStatus currentStatus;
        private PSExecWrapper psExec;

        public event EventHandler CriticalStopError;

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnCriticalStopError(EventArgs e)
        {
            CriticalStopError?.Invoke(this, e);
        }

        private void OnPropertyChanged(string name)
        {
            if (parentForm.InvokeRequired)
            {
                parentForm.Invoke(new Action(() => OnPropertyChanged(name)));
            }
            else
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            }
        }

        public GKUpdate(Form parentForm, Device targetDevice, int seq = 0)
        {
            this.parentForm = parentForm;
            this.targetDevice = targetDevice;
            hostname = targetDevice.HostName;
            Sequence = seq;
            TargetText = targetDevice.HostName + " - " + targetDevice.CurrentUser;

            psExec = new PSExecWrapper(targetDevice.HostName);
            psExec.ErrorReceived += PsExec_ErrorReceived;
            psExec.OutputReceived += PsExec_OutputReceived;

            DrawLight(Color.Yellow);
            StatusText = "Queued";
            CancelRemoveButtonText = "Remove";
            StartRestartButtonText = "Start";
            OnPropertyChanged(null);
        }

        private void PsExec_OutputReceived(object sender, string e)
        {
            Log("Output: " + e);
        }

        private void PsExec_ErrorReceived(object sender, string e)
        {
            Log("Error: " + e);
        }

        public void CancelUpdate()
        {
            psExec.StopProcess();
            SetStatus(UpdateStatus.Canceled);
        }

        public void StartRestart()
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
            Log("-----START-----");
            var elapTimer = new System.Diagnostics.Stopwatch();
            elapTimer.Start();

            try
            {
                int exitCode;

                SetStatus(UpdateStatus.Starting);

                // Check for ping.
                if (!await CanPing(targetDevice.HostName))
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

                if (exitCode != 0 && exitCode != 1)
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
               
                Log("Update completed successfully.");
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
            finally
            {
                elapTimer.Stop();
                Log(string.Format("Elapsed time: {0} s", Math.Round(elapTimer.Elapsed.TotalSeconds,2)));
                Log("-----END-----");
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
            if (StatusLight == null)
            {
                StatusLight = new Bitmap(20, 20);
            }

            using (SolidBrush brush = new SolidBrush(color))
            using (Pen strokePen = new Pen(Color.Black, 1.5f))
            using (Graphics gr = Graphics.FromImage(StatusLight))
            {
                gr.Clear(Color.Transparent);
                gr.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

                float size = 18F;

                gr.FillEllipse(brush, 0, 0, size, size);
                gr.DrawEllipse(strokePen, 0, 0, size, size);
            }
        }

        private void SetStatus(UpdateStatus Status, string message = null)
        {
            currentStatus = Status;
            SetStatusLight(Status);
            SetStatusText(Status, message);
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

                case UpdateStatus.Done:
                    DrawLight(Color.DeepSkyBlue);
                    break;

                default:
                    DrawLight(Color.Red);
                    break;
            }

            OnPropertyChanged(nameof(StatusLight));
        }

        private void SetStatusText(UpdateStatus Status, string message = null)
        {
            switch (Status)
            {
                case UpdateStatus.Queued:
                    StatusText = "Queued";
                    StartRestartButtonText = "Start";
                    CancelRemoveButtonText = "Remove";
                    break;

                case UpdateStatus.Starting:
                    StatusText = "Starting";
                    StartRestartButtonText = "Restart";
                    CancelRemoveButtonText = "Cancel";
                    break;

                case UpdateStatus.Running:
                    StatusText = "Running";
                    StartRestartButtonText = "Restart";
                    CancelRemoveButtonText = "Cancel";
                    break;

                case UpdateStatus.Canceled:
                    StatusText = "Canceled";
                    StartRestartButtonText = "Restart";
                    CancelRemoveButtonText = "Remove";
                    break;

                case UpdateStatus.Done:
                    StatusText = "Done";
                    StartRestartButtonText = "Restart";
                    CancelRemoveButtonText = "Remove";
                    break;

                case UpdateStatus.Error:
                    StatusText = "ERROR!";
                    StartRestartButtonText = "Restart";
                    CancelRemoveButtonText = "Remove";
                    break;
            }

            if (message != null)
            {
                StatusText += " - " + message;
            }
        }

        private void Log(string message)
        {
            LogMessages += message + Environment.NewLine;
        }
    }
}