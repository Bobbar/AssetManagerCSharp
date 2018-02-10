using AssetManager.Data;
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
        private ExtendedForm logView;
        private ExtendedForm parentForm;
        private PowerShellWrapper powerShellWrapper;
        private PsExecWrapper psExecWrapper;
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
                    throw new Exception("Please call " + nameof(DeploymentUI.UsePowerShell) + " first.");
                }
                return powerShellWrapper;
            }
        }

        public PsExecWrapper PsExecWrap
        {
            get
            {
                if (psExecWrapper == null)
                {
                    throw new Exception("Please call " + nameof(DeploymentUI.UsePsExec) + " first.");
                }
                return psExecWrapper;
            }
        }

        public DeploymentUI(ExtendedForm parentForm)
        {
            this.parentForm = parentForm;

            watchDogCancelTokenSource = new CancellationTokenSource();
            watchDogTask = new Task(() => WatchDog(watchDogCancelTokenSource.Token), watchDogCancelTokenSource.Token);

            InitLogWindow(parentForm);
        }

        public void UsePowerShell()
        {
            powerShellWrapper = new PowerShellWrapper();
            powerShellWrapper.InvocationStateChanged -= SessionStateChanged;
            powerShellWrapper.InvocationStateChanged += SessionStateChanged;
        }

        public void UsePsExec()
        {
            psExecWrapper = new PsExecWrapper();
            psExecWrapper.ErrorReceived -= PsExecErrorReceived;
            psExecWrapper.ErrorReceived += PsExecErrorReceived;

            psExecWrapper.OutputReceived -= PsExecOutputReceived;
            psExecWrapper.OutputReceived += PsExecOutputReceived;
        }

        private void InitLogWindow(ExtendedForm parentForm)
        {
            this.parentForm = parentForm;
            logView = new ExtendedForm(parentForm);
            logView.FormClosing += new FormClosingEventHandler(LogClosed);
            logView.Text = "Deployment Log (Close to cancel)";
            logView.Width = 500;
            logView.Owner = parentForm;
            logView.StartPosition = FormStartPosition.CenterScreen;
            RTBLog = new RichTextBox();
            RTBLog.Dock = DockStyle.Fill;
            RTBLog.Font = StyleFunctions.DefaultGridFont;
            logView.Controls.Add(RTBLog);
            logView.Show();

            watchDogTask.Start();
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
                    if (OtherFunctions.Message("Cancel the current operation?", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Question, "Cancel?", parentForm) == DialogResult.Yes)
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

        public void DoneOrError()
        {
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

            if (psExecWrapper != null)
            {
                psExecWrapper.StopProcess();
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
                            LogMessage("Still waiting...");
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
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion IDisposable Support
    }
}