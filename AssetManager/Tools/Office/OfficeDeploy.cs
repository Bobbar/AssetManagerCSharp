using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AdminTools;
using System;
using System.ComponentModel;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools.Office
{
    public class OfficeDeploy : IDisposable
    {


        #region Fields

        private const string deploymentFilesDirectory = "\\\\core.co.fairfield.oh.us\\dfs1\\fcdd\\files\\Information Technology\\Software\\Office\\RemoteDeploy";
        private const string deployTempDirectory = "\\Temp\\OfficeDeploy";
        private const string fullDeployTempDir = "C:" + deployTempDirectory;
        private const string removeOfficeScriptPath = fullDeployTempDir + "\\Remove-PreviousOfficeInstalls";
        private bool cancelOperation = false;
        private bool finished = false;
        private long lastActivity;
        private ExtendedForm logView;
        private ExtendedForm parentForm;
        private PowerShellWrapper PSWrapper = new PowerShellWrapper();
        private RichTextBox RTBLog;
        private int timeoutSeconds = 120;
        private Task watchDogTask;
        private CancellationTokenSource watchDogCancelTokenSource;

        #endregion Fields

        #region Delegates

        private delegate void RTBLogDelegate(string message);

        #endregion Delegates

        #region Constructors

        public OfficeDeploy()
        {
            PSWrapper.InvocationStateChanged += SessionStateChanged;
            watchDogCancelTokenSource = new CancellationTokenSource();
            watchDogTask = new Task(() => WatchDog(watchDogCancelTokenSource.Token), watchDogCancelTokenSource.Token);
        }

        #endregion Constructors

        #region Methods

        public async Task<bool> DeployToDevice(ExtendedForm parentForm, Device targetDevice)
        {
            long startTime = 0;
            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    startTime = DateTime.Now.Ticks;

                    InitLogWindow(parentForm);

                    DepLog("Starting new Office 365 deployment to " + targetDevice.HostName);
                    DepLog("-------------------");

                    watchDogTask.Start();

                    using (CopyFilesForm PushForm = new CopyFilesForm(parentForm, targetDevice, deploymentFilesDirectory, deployTempDirectory))
                    {
                        DepLog("Pushing files to target computer...");
                        if (await PushForm.StartCopy())
                        {
                            DepLog("Push successful!");
                            PushForm.Dispose();
                        }
                        else
                        {
                            DepLog("Push failed!");
                            OtherFunctions.Message("Error occurred while pushing deployment files to device!");
                            return false;
                        }
                    }

                    DepLog("Removing previous Office installations...");

                    DepLog("Starting remote session...");
                    var officeDeploySession = await GetRemoveOfficeSession(targetDevice);

                    DepLog("Invoking removal script...");
                    if (await PSWrapper.InvokePowerShellSession(officeDeploySession))
                    {
                        DepLog("Previous Office installations removed.");
                    }
                    else
                    {
                        DepLog("Failed to remove previous installations!");
                        OtherFunctions.Message("Error occurred while executing deployment command!");
                        return false;
                    }

                    DepLog("Starting Office 356 deployment...");
                    if (await PSWrapper.InvokePowerShellCommand(targetDevice.HostName, GetO365InstallCommand()))
                    {
                        DepLog("Deployment complete!");
                    }
                    else
                    {
                        DepLog("Deployment failed!");
                        OtherFunctions.Message("Error occurred while executing deployment command!");
                        return false;
                    }

                    DepLog("Deleting temp files...");
                    if (!await PSWrapper.InvokePowerShellCommand(targetDevice.HostName, GetDeleteDirectoryCommand()))
                    {
                        DepLog("Delete failed!");
                        return false;
                    }

                    DepLog("Done.");
                    DepLog("-------------------");
                    DepLog("Office 365 deployment is complete!");
                    return true;
                }
                else
                {
                    OtherFunctions.Message("The target device is null or does not have a hostname.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Missing Info", parentForm);
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                if (startTime > 0)
                {
                    var runTimeSeconds = ((DateTime.Now.Ticks - startTime) / 10000) / 1000;
                    DepLog("Run Time: " + runTimeSeconds + " s");
                }

                DoneOrError();
            }
        }

        private void DoneOrError()
        {
            finished = true;
            watchDogCancelTokenSource.Cancel();
        }

        private void DepLog(string message)
        {
            if (!cancelOperation) ActivityTick();
            if (RTBLog.InvokeRequired)
            {
                RTBLogDelegate d = new RTBLogDelegate(DepLog);
                RTBLog.Invoke(d, message);
            }
            else
            {
                RTBLog.AppendText(DateTime.Now.ToString() + ": " + message + "\r\n");
            }
        }

        private async Task<PowerShell> GetRemoveOfficeSession(Device targetDevice)
        {
            var session = await PSWrapper.GetNewPSSession(targetDevice.HostName, SecurityTools.AdminCreds);

            // Change directory to script location.
            var setLocationCommand = new Command("Set-Location");
            setLocationCommand.Parameters.Add("Path", removeOfficeScriptPath);
            session.Commands.AddCommand(setLocationCommand);

            // Execute the remove office script.
            var removeCommand = new Command(removeOfficeScriptPath + "\\Remove-PreviousOfficeInstalls.ps1", true, true);
            removeCommand.Parameters.Add("Wait");
            removeCommand.Parameters.Add("NoNewWindow");
            session.Commands.AddCommand(removeCommand);

            return session;
        }
        
        private Command GetO365InstallCommand()
        {
            var cmd = new Command("Start-Process", false, true);
            cmd.Parameters.Add("FilePath", fullDeployTempDir + "\\setup.exe");
            cmd.Parameters.Add("ArgumentList", "/configure " + fullDeployTempDir + "\\configuration.xml");
            cmd.Parameters.Add("Wait");
            cmd.Parameters.Add("NoNewWindow");
            return cmd;
        }

        private Command GetDeleteDirectoryCommand()
        {
            var cmd = new Command("Remove-Item", false, true);
            cmd.Parameters.Add("Recurse");
            cmd.Parameters.Add("Force");
            cmd.Parameters.Add("Path", fullDeployTempDir);
            return cmd;
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
                        PSWrapper.StopPowerShellCommand();
                        PSWrapper.StopPiplineCommand();
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
                            PSWrapper.StopPowerShellCommand();
                            PSWrapper.StopPiplineCommand();
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

        private void SessionStateChanged(object sender, EventArgs e)
        {
            var args = (PSInvocationStateChangedEventArgs)e;
            DepLog("Session state: " + args.InvocationStateInfo.State.ToString());
        }
        
        private void ActivityTick()
        {
            lastActivity = DateTime.Now.Ticks;
            if (cancelOperation)
            {
                DepLog("The deployment has been canceled!");
                throw (new DeploymentCanceledException());
            }
        }

        private int SecondsSinceLastActivity()
        {
            return System.Convert.ToInt32(((DateTime.Now.Ticks - lastActivity) / 10000f) / 1000f);
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
                            DepLog("Still waiting...");
                            TimeoutMessageSent = true;
                        }
                    }
                    else
                    {
                        TimeoutMessageSent = false;
                    }
                    if (cancelOperation && !CancelMessageSent)
                    {
                        DepLog("Cancelling the operation...");
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

        #endregion Methods

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
        #endregion


    }
}
