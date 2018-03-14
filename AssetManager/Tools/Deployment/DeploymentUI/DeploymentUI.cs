using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools.Deployment
{
    public class DeploymentUI : IDisposable
    {
        private bool cancelOperation = false;
        private bool finished = false;
        private long lastActivity;
        private long startTime = 0;
        private ExtendedForm logView;
        private ExtendedForm parentForm;
        private PowerShellWrapper powerShellWrapper;
        private PSExecWrapper pSExecWrapper;
        private RichTextBox RTBLog;
        private int timeoutSeconds = 120;
        private Task watchDogTask;
        private CancellationTokenSource watchDogCancelTokenSource;

        private delegate void RTBLogDelegate(string message);

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

        public DeploymentUI(ExtendedForm parentForm)
        {
            this.parentForm = parentForm;

            watchDogCancelTokenSource = new CancellationTokenSource();
            watchDogTask = new Task(() => WatchDog(watchDogCancelTokenSource.Token), watchDogCancelTokenSource.Token);

            InitLogWindow();
        }

        public void UsePowerShell()
        {
            powerShellWrapper = new PowerShellWrapper();
            powerShellWrapper.InvocationStateChanged -= SessionStateChanged;
            powerShellWrapper.InvocationStateChanged += SessionStateChanged;
        }

        public void UsePsExec()
        {
            pSExecWrapper = new PSExecWrapper();
            pSExecWrapper.ErrorReceived -= PsExecErrorReceived;
            pSExecWrapper.ErrorReceived += PsExecErrorReceived;

            pSExecWrapper.OutputReceived -= PsExecOutputReceived;
            pSExecWrapper.OutputReceived += PsExecOutputReceived;
        }

        private void InitLogWindow()
        {
            logView = new ExtendedForm(parentForm);
            logView.FormClosing += new FormClosingEventHandler(LogClosed);
            logView.Text = "Deployment Log (Close to cancel)";
            logView.Width = 600;
            logView.Height = 700;
            logView.Owner = parentForm;
            logView.StartPosition = FormStartPosition.CenterScreen;
            RTBLog = new RichTextBox();
            RTBLog.Dock = DockStyle.Fill;
            RTBLog.Font = StyleFunctions.DefaultGridFont;
            RTBLog.WordWrap = false;
            RTBLog.ReadOnly = true;
            RTBLog.ScrollBars = RichTextBoxScrollBars.Both;
            logView.Controls.Add(RTBLog);
            logView.Show();

            watchDogTask.Start();
        }

        public async Task SimplePSExecCommand(Device targetDevice, string command, string title)
        {
            LogMessage("Starting " + title);
            var exitCode = await PSExecWrap.ExecuteRemoteCommand(targetDevice, command);
            if (exitCode == 0)
            {
                LogMessage(title + " complete!");
            }
            else
            {
                LogMessage(title + " failed! Exit code: " + exitCode.ToString());
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw new Exception("Error occurred while executing command");
            }
        }

        public async Task SimplePowerShellCommand(Device targetDevice, byte[] scriptBytes, string title)
        {
            LogMessage("Starting " + title);
            var success = await PowerShellWrap.ExecutePowerShellScript(targetDevice.HostName, scriptBytes);
            if (success)
            {
                LogMessage(title + " complete!");
            }
            else
            {
                LogMessage(title + " failed!");
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                throw new Exception("Error occurred while executing command");
            }
        }

        public void LogMessage(string message)
        {
            if (!cancelOperation) ActivityTick();
            if (RTBLog.InvokeRequired)
            {
                RTBLogDelegate d = new RTBLogDelegate(LogMessage);
                RTBLog.Invoke(d, message);
            }
            else
            {
                RTBLog.AppendText(DateTime.Now.ToString() + ": " + message + "\r\n");
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
            }
        }

        public void StartTimer()
        {
            if (startTime == 0)
            {
                startTime = DateTime.Now.Ticks;
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
            watchDogCancelTokenSource.Cancel();
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

        private void WatchDog(CancellationToken cancelToken)
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
                            LogMessage("Still running...");
                            TimeoutMessageSent = true;
                        }
                    }
                    else
                    {
                        TimeoutMessageSent = false;
                    }
                    if (cancelOperation && !CancelMessageSent)
                    {
                        LogMessage("Cancelling the operation...");
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
            LogMessage("Session state: " + args.InvocationStateInfo.State.ToString());
        }

        private void PsExecOutputReceived(object sender, EventArgs e)
        {
            var args = (DataReceivedEventArgs)e;
            var dataString = DataConsistency.CleanDBValue(args.Data).ToString();
            LogMessage("PsExec Output: " + dataString);
        }

        private void PsExecErrorReceived(object sender, EventArgs e)
        {
            var args = (DataReceivedEventArgs)e;
            var dataString = DataConsistency.CleanDBValue(args.Data).ToString();
            LogMessage("PsExec Output: " + dataString);
        }

        private void ActivityTick()
        {
            lastActivity = DateTime.Now.Ticks;
            if (cancelOperation)
            {
                LogMessage("The deployment has been canceled!");
                throw (new DeploymentCanceledException());
            }
        }

        // Wrapper to shorten calls.
        public string GetString(string stringName)
        {
            return AssetManagerFunctions.GetDeployString(stringName);
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
                    RTBLog.Dispose();
                    watchDogCancelTokenSource.Dispose();
                    watchDogTask.Dispose();
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