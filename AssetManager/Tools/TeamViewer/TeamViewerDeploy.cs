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
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools.TeamViewer
{
    public class TeamViewerDeploy : IDisposable
    {
        #region Fields

        private const string deploymentFilesDirectory = "\\\\core.co.fairfield.oh.us\\dfs1\\fcdd\\files\\Information Technology\\Software\\Tools\\TeamViewer\\Deploy";
        private const string deployTempDirectory = "\\Temp\\TVDeploy";
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

        #region Methods

        #region Constructors

        public TeamViewerDeploy()
        {
            watchDogCancelTokenSource = new CancellationTokenSource();
            watchDogTask = new Task(() => WatchDog(watchDogCancelTokenSource.Token), watchDogCancelTokenSource.Token);
        }

        #endregion Constructors

        public async Task<bool> DeployToDevice(ExtendedForm parentForm, Device targetDevice)
        {
            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    bool TVExists = false;

                    InitLogWindow(parentForm);

                    DepLog("Starting new TeamViewer deployment to " + targetDevice.HostName);
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

                    DepLog("Checking for previous installation...");

                    TVExists = await TeamViewerInstalled(targetDevice);
                    if (TVExists)
                    {
                        DepLog("TeamViewer already installed.");
                    }
                    else
                    {
                        DepLog("TeamViewer not installed.");
                    }

                    if (TVExists)
                    {
                        DepLog("Reinstalling TeamViewer...");

                        if (await PSWrapper.InvokePowerShellCommand(targetDevice.HostName, GetTVReinstallCommand()))
                        {
                            DepLog("Deployment complete!");
                        }
                        else
                        {
                            DepLog("Deployment failed!");
                            OtherFunctions.Message("Error occurred while executing deployment command!");
                            return false;
                        }
                    }
                    else
                    {
                        DepLog("Starting TeamViewer deployment...");
                        if (await PSWrapper.InvokePowerShellCommand(targetDevice.HostName, GetTVInstallCommand()))
                        {
                            DepLog("Deployment complete!");
                        }
                        else
                        {
                            DepLog("Deployment failed!");
                            OtherFunctions.Message("Error occurred while executing deployment command!");
                            return false;
                        }
                    }

                    DepLog("Waiting 10 seconds.");
                    for (var i = 10; i >= 1; i--)
                    {
                        await Task.Delay(1000);
                        DepLog(i + "...");
                    }

                    DepLog("Starting TeamViewer assignment...");
                    if (await PSWrapper.InvokePowerShellCommand(targetDevice.HostName, GetTVAssignCommand()))
                    {
                        DepLog("Assignment complete!");
                    }
                    else
                    {
                        DepLog("Assignment failed!");
                        OtherFunctions.Message("Error occurred while executing assignment command!");
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
                    DepLog("TeamView deployment is complete!");
                    DepLog("NOTE: The target computer may need rebooted or the user may need to open the application before TeamViewer will connect.");
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

        private Command GetTVAssignCommand()
        {
            string ApiToken = AssetManagerFunctions.GetTVApiToken();
            var cmd = new Command("Start-Process", false, true);
            cmd.Parameters.Add("FilePath", "C:\\Temp\\TVDeploy\\Assignment\\TeamViewer_Assignment.exe");
            cmd.Parameters.Add("ArgumentList", "-apitoken " + ApiToken + " -datafile ${ProgramFiles}\\TeamViewer\\AssignmentData.json");
            cmd.Parameters.Add("Wait");
            cmd.Parameters.Add("NoNewWindow");
            return cmd;
        }

        private Command GetTVReinstallCommand()
        {
            var cmd = new Command("Start-Process", false, true);
            cmd.Parameters.Add("FilePath", "msiexec.exe");
            cmd.Parameters.Add("ArgumentList", "/i C:\\Temp\\TVDeploy\\TeamViewer_Host-idcjnfzfgb.msi REINSTALL=ALL REINSTALLMODE=omus /qn");
            cmd.Parameters.Add("Wait");
            cmd.Parameters.Add("NoNewWindow");
            return cmd;
        }

        private Command GetTVInstallCommand()
        {
            var cmd = new Command("Start-Process", false, true);
            cmd.Parameters.Add("FilePath", "msiexec.exe");
            cmd.Parameters.Add("ArgumentList", "/i C:\\Temp\\TVDeploy\\TeamViewer_Host-idcjnfzfgb.msi /qn");
            cmd.Parameters.Add("Wait");
            cmd.Parameters.Add("NoNewWindow");
            return cmd;
        }

        private Command GetDeleteDirectoryCommand()
        {
            var cmd = new Command("Remove-Item", false, true);
            cmd.Parameters.Add("Recurse");
            cmd.Parameters.Add("Force");
            cmd.Parameters.Add("Path", "C:\\Temp\\TVDeploy\\");
            return cmd;
        }

        private async Task<bool> TeamViewerInstalled(Device targetDevice)
        {
            try
            {
                var resultString = await Task.Run(() =>
                {
                    return PSWrapper.ExecuteRemotePSScript(targetDevice.HostName, Properties.Resources.CheckForTVRegistryValue, SecurityTools.AdminCreds);
                });
                var result = Convert.ToBoolean(resultString);
                return result;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void InitLogWindow(ExtendedForm parentForm)
        {
            this.parentForm = parentForm;
            logView = new ExtendedForm(parentForm);
            logView.FormClosing += new FormClosingEventHandler(LogClosed);
            logView.Text = "Deployment Log (Close to cancel)";
            logView.Width = 500;
            logView.Owner = parentForm;
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
                            DepLog("The operation is taking a long time...");
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
        // ~TeamViewerDeploy() {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
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