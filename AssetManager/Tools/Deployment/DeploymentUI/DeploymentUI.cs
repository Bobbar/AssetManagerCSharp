using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AdminTools;
using DeploymentAssemblies;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace AssetManager.Tools.Deployment
{
    public class DeploymentUI : IDisposable, IDeploymentUI
    {
        private bool cancelOperation = false;
        private bool finished = false;
        private long lastActivity;
        private long startTime = 0;
        private string defaultTitle = "Deployment Log (Close to cancel)";
        private ExtendedForm logView;
        private ExtendedForm parentForm;
        private PowerShellWrapper powerShellWrapper;
        private PSExecWrapper pSExecWrapper;
        private RichTextBox logTextBox;
        private int timeoutSeconds = 120;
        private Task watchdogTask;
        private CancellationTokenSource watchdogCancelTokenSource;
        private Device targetDevice;

        public Form ParentForm
        {
            get
            {
                return parentForm;
            }
        }

        public string TargetHostname
        {
            get
            {
                return targetDevice.HostName;
            }
        }

        public PowerShellWrapper PowerShellWrap
        {
            get
            {
                if (powerShellWrapper == null)
                {
                    throw new NotSupportedException("Please call " + nameof(DeploymentUI.UsePowerShell) + " first.");
                }
                return powerShellWrapper;
            }
        }

        public PSExecWrapper PSExecWrap
        {
            get
            {
                if (pSExecWrapper == null)
                {
                    throw new NotSupportedException("Please call " + nameof(DeploymentUI.UsePsExec) + " first.");
                }
                return pSExecWrapper;
            }
        }

        public DeploymentUI(ExtendedForm parentForm, Device targetDevice)
        {
            this.targetDevice = targetDevice;
            this.parentForm = parentForm;

            watchdogCancelTokenSource = new CancellationTokenSource();
            watchdogTask = new Task(() => Watchdog(watchdogCancelTokenSource.Token), watchdogCancelTokenSource.Token);

            InitLogWindow();
        }

        public void UsePowerShell()
        {
            if (powerShellWrapper == null)
            {
                powerShellWrapper = new PowerShellWrapper(targetDevice.HostName);
                powerShellWrapper.InvocationStateChanged -= SessionStateChanged;
                powerShellWrapper.InvocationStateChanged += SessionStateChanged;

                powerShellWrapper.PowershellOutput -= PowerShellWrapper_PowershellOutput;
                powerShellWrapper.PowershellOutput += PowerShellWrapper_PowershellOutput;
            }
        }

        public void UsePsExec()
        {
            if (pSExecWrapper == null)
            {
                pSExecWrapper = new PSExecWrapper(targetDevice.HostName);
                pSExecWrapper.ErrorReceived -= PsExecErrorReceived;
                pSExecWrapper.ErrorReceived += PsExecErrorReceived;

                pSExecWrapper.OutputReceived -= PsExecOutputReceived;
                pSExecWrapper.OutputReceived += PsExecOutputReceived;
            }
        }

        public void SetTitle(string value)
        {
            if (logView != null)
            {
                logView.Text = value + " - " + defaultTitle;
            }
        }

        private void InitLogWindow()
        {
            logView = new ExtendedForm(parentForm);
            logView.FormClosing += new FormClosingEventHandler(LogClosed);
            logView.Text = targetDevice.CurrentUser + " - " + defaultTitle;
            logView.Width = 600;
            logView.Height = 700;
            logView.MinimumSize = new System.Drawing.Size(400, 200);
            logView.Owner = parentForm;
            logView.StartPosition = FormStartPosition.CenterParent;
            logView.DisableDoubleBuffering();

            logTextBox = new RichTextBox();
            logTextBox.Dock = DockStyle.Fill;
            logTextBox.Font = StyleFunctions.DefaultGridFont;
            logTextBox.WordWrap = false;
            logTextBox.ReadOnly = true;
            logTextBox.ScrollBars = RichTextBoxScrollBars.Both;
            logTextBox.DetectUrls = false;
            logView.Controls.Add(logTextBox);
            logView.Show();
        }

        public void UserPrompt(string prompt, string title = "")
        {
            OtherFunctions.Message(prompt, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, title, parentForm);
        }

        public ICopyFiles NewFilePush(string source, string destination)
        {
            return new CopyFilesForm(parentForm, targetDevice, source, destination);
        }

        public async Task<bool> SimplePSExecCommand(string command, string title)
        {
            LogMessage("Starting " + title + "...", MessageType.Notice);
            var exitCode = await PSExecWrap.ExecuteRemoteCommand(command);
            if (exitCode == 0)
            {
                LogMessage(title + " Complete!", MessageType.Success);
                return true;
            }
            else
            {
                LogMessage(title + " failed! Exit code: " + exitCode.ToString(), MessageType.Error);
                parentForm.RestoreWindow();
                parentForm.FlashWindow(5);
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Deployment Error", logView);
                throw new Exception("Error occurred while executing command");
            }
        }

        public async Task<int> AdvancedPSExecCommand(string command, string title)
        {
            LogMessage("Starting " + title + "...", MessageType.Notice);
            return await PSExecWrap.ExecuteRemoteCommand(command);
        }

        public async Task SimplePowerShellScript(byte[] script, string title)
        {
            LogMessage("Starting " + title + "...", MessageType.Notice);
            var success = await PowerShellWrap.ExecutePowerShellScript(script);
            if (success)
            {
                LogMessage(title + " Complete!", MessageType.Success);
            }
            else
            {
                LogMessage(title + " failed!", MessageType.Error);
                parentForm.RestoreWindow();
                parentForm.FlashWindow(5);
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Deployment Error", logView);
                throw new Exception("Error occurred while executing command");
            }
        }

        public async Task<string> AdvancedPowerShellScript(byte[] script)
        {
            return await Task.Run(() => { return PowerShellWrap.ExecuteRemotePSScript(script, Security.SecurityTools.AdminCreds); });
        }

        public async Task<bool> SimplePowerShellCommand(PowerShellCommand command)
        {
            var session = await PowerShellWrap.GetNewPSSession(Security.SecurityTools.AdminCreds);
            var shellCommand = new Command(command.CommandText, command.IsScript, command.UseLocalScope);

            foreach (var param in command.Parameters)
            {
                shellCommand.Parameters.Add(param.Name, param.Value);
            }

            session.Commands.AddCommand(shellCommand);

            return await PowerShellWrap.InvokePowerShellSession(session);
        }

        public async Task<bool> SimplePowerShellCommand(PowerShellCommand[] commands)
        {
            var session = await PowerShellWrap.GetNewPSSession(Security.SecurityTools.AdminCreds);

            foreach (var cmd in commands)
            {
                var shellCommand = new Command(cmd.CommandText, cmd.IsScript, cmd.UseLocalScope);

                foreach (var param in cmd.Parameters)
                {
                    shellCommand.Parameters.Add(param.Name, param.Value);
                }

                session.Commands.AddCommand(shellCommand);
            }

            return await PowerShellWrap.InvokePowerShellSession(session);
        }

        public void LogMessage(string message, MessageType type)
        {
            if (!cancelOperation) ActivityTick();
            if (logTextBox.InvokeRequired)
            {
                var del = new Action(() => LogMessage(message));
                logTextBox.BeginInvoke(del);
            }
            else
            {
                logTextBox.SelectionStart = logTextBox.TextLength;
                logTextBox.SelectionLength = 0;
                logTextBox.SelectionColor = ColorOfType(type);
                logTextBox.AppendText(DateTime.Now.ToString() + ": " + message + "\r\n");
                logTextBox.SelectionColor = logTextBox.ForeColor;
                logTextBox.SelectionStart = logTextBox.Text.Length;
                logTextBox.ScrollToCaret();
            }
        }

        public void LogMessage(string message)
        {
            LogMessage(message, MessageType.Default);
        }

        private Color ColorOfType(MessageType type)
        {
            switch (type)
            {
                case MessageType.Default:
                    return Color.Black;

                case MessageType.Success:
                    return Color.Green;

                case MessageType.Error:
                    return Color.Red;

                case MessageType.Notice:
                    return Color.Blue;

                case MessageType.Warning:
                    return Color.Goldenrod;

                default:
                    return Color.Black;
            }
        }

        private void LogClosed(object sender, CancelEventArgs e)
        {
            if (!finished)
            {
                if (!cancelOperation)
                {
                    if (OtherFunctions.Message("Cancel the current operation?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Cancel?", parentForm) == DialogResult.Yes)
                    {
                        cancelOperation = true;
                        StopRemoteProcesses();
                    }
                    e.Cancel = true;
                }
                else
                {
                    if (finished)
                    {
                        e.Cancel = false;
                    }
                    else
                    {
                        if (SecondsSinceLastActivity() > timeoutSeconds)
                        {
                            StopRemoteProcesses();
                        }

                        e.Cancel = true;
                    }
                }
            }
            else
            {
                e.Cancel = false;
                this.Dispose();
            }
        }

        public void StartTimer()
        {
            if (startTime == 0)
            {
                startTime = DateTime.Now.Ticks;

                if (watchdogTask.Status != TaskStatus.Running) watchdogTask.Start();
            }
        }

        private void PrintElapsedTime()
        {
            if (startTime > 0)
            {
                double runTimeSeconds = ((DateTime.Now.Ticks - startTime) / 10000f) / 1000f;
                var timeSpan = TimeSpan.FromSeconds(runTimeSeconds);
                string elapTimeString = string.Format("Run Time: {0}h:{1}m:{2}s", timeSpan.Hours, timeSpan.Minutes, timeSpan.Seconds);
                LogMessage(elapTimeString);
            }
        }

        public void DoneOrError()
        {
            PrintElapsedTime();
            finished = true;
            watchdogCancelTokenSource.Cancel();
        }

        private void StopRemoteProcesses()
        {
            if (powerShellWrapper != null)
            {
                powerShellWrapper.StopPowerShellCommand();
                powerShellWrapper.StopPiplineCommand();
            }

            if (pSExecWrapper != null)
            {
                pSExecWrapper.StopProcess();
            }
        }

        private int SecondsSinceLastActivity()
        {
            if (lastActivity > 0)
            {
                return System.Convert.ToInt32(((DateTime.Now.Ticks - lastActivity) / 10000f) / 1000f);
            }
            return -1;
        }

        private void Watchdog(CancellationToken cancelToken)
        {
            try
            {
                bool TimeoutMessageSent = false;
                bool CancelMessageSent = false;
                while (!cancelToken.IsCancellationRequested)
                {
                    if (SecondsSinceLastActivity() > timeoutSeconds)
                    {
                        if (!TimeoutMessageSent)
                        {
                            LogMessage("Still running...", MessageType.Notice);
                            TimeoutMessageSent = true;
                        }
                    }
                    else
                    {
                        TimeoutMessageSent = false;
                    }
                    if (cancelOperation && !CancelMessageSent)
                    {
                        LogMessage("Cancelling the operation...", MessageType.Warning);
                        CancelMessageSent = true;
                    }
                    Task.Delay(1000).Wait(cancelToken);
                }
            }
            catch (TaskCanceledException)
            {
                // Catch this and make it quiet.
            }
            catch (OperationCanceledException)
            {
                // Catch this and make it quiet.
            }
        }

        private void SessionStateChanged(object sender, EventArgs e)
        {
            var args = (PSInvocationStateChangedEventArgs)e;
            LogMessage("Session state: " + args.InvocationStateInfo.State.ToString(), MessageType.Notice);
        }

        private void PowerShellWrapper_PowershellOutput(object sender, string e)
        {
            LogMessage($@"Output: {e}");
        }

        private void PsExecOutputReceived(object sender, string data)
        {
            LogMessage("Output: " + data);
        }

        private void PsExecErrorReceived(object sender, string data)
        {
            LogMessage("Output(err): " + data);
        }

        private void ActivityTick()
        {
            lastActivity = DateTime.Now.Ticks;
            if (cancelOperation)
            {
                LogMessage("The deployment has been canceled!", MessageType.Error);
                throw (new DeploymentCanceledException());
            }
        }

        // Wrapper to shorten calls.
        public string GetString(string name)
        {
            return AssetManagerFunctions.GetDeployString(name);
        }

        private class DeploymentCanceledException : Exception
        {
            public DeploymentCanceledException()
            {
            }

            public DeploymentCanceledException(string message) : base(message)
            {
            }

            public DeploymentCanceledException(string message, Exception inner) : base(message, inner)
            {
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    logView.Dispose();
                    logTextBox.Dispose();
                    watchdogCancelTokenSource.Dispose();
                    watchdogTask.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}