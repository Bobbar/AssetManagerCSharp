using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AdminTools;
using System;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools.Deployment
{
    public class DeployTeamViewer : IDisposable
    {
        private string filesDirectory;
        private string tempDirectory;
        private string fullTempDirectory;

        private ExtendedForm parentForm;

        private DeploymentUI deploy;

        public DeployTeamViewer(ExtendedForm parentForm, Device targetDevice)
        {
            this.parentForm = parentForm;
            deploy = new DeploymentUI(parentForm, targetDevice);
            deploy.UsePowerShell();
            deploy.UsePsExec();
            GetDirectories();
        }

        public DeployTeamViewer(ExtendedForm parentForm, DeploymentUI deployUI)
        {
            this.parentForm = parentForm;
            deploy = deployUI;
            deploy.UsePowerShell();
            GetDirectories();
        }

        private void GetDirectories()
        {
            filesDirectory = deploy.GetString("teamviewer_deploy_dir");
            tempDirectory = deploy.GetString("teamviewer_temp_dir");
            fullTempDirectory = "C:" + tempDirectory;
        }

        public async Task<bool> DeployToDevice(Device targetDevice)
        {
            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    deploy.SetTitle(targetDevice.CurrentUser);

                    bool TVExists = false;

                    deploy.LogMessage("Starting new TeamViewer deployment to " + targetDevice.HostName);
                    deploy.LogMessage("-------------------");

                    using (CopyFilesForm PushForm = new CopyFilesForm(parentForm, targetDevice, filesDirectory, tempDirectory))
                    {
                        deploy.LogMessage("Pushing files to target computer...");
                        if (await PushForm.StartCopy())
                        {
                            deploy.LogMessage("Push successful!");
                            PushForm.Dispose();
                        }
                        else
                        {
                            deploy.LogMessage("Push failed!");
                            OtherFunctions.Message("Error occurred while pushing deployment files to device!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }
                    }

                    deploy.LogMessage("Checking for previous installation...");

                    TVExists = await TeamViewerInstalled(targetDevice);
                    if (TVExists)
                    {
                        deploy.LogMessage("TeamViewer already installed.");
                    }
                    else
                    {
                        deploy.LogMessage("TeamViewer not installed.");
                    }

                    if (TVExists)
                    {
                        deploy.LogMessage("Uninstalling TeamViewer...");
                        var tvReinstallExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(GetTVUninstallString());
                        if (tvReinstallExitCode == 0)
                        {
                            deploy.LogMessage("Uninstall complete!");
                        }
                        else
                        {
                            deploy.LogMessage("Uninstall failed!");
                            OtherFunctions.Message("Error occurred while executing deployment command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                            return false;
                        }

                    }

                    deploy.LogMessage("Starting TeamViewer install...");
                    var tvExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(GetTVInstallString());
                    if (tvExitCode == 0)
                    {
                        deploy.LogMessage("Install complete!");
                    }
                    else
                    {
                        deploy.LogMessage("Install failed!");
                        OtherFunctions.Message("Error occurred while executing deployment command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    deploy.LogMessage("Waiting 10 seconds.");
                    for (var i = 10; i >= 1; i--)
                    {
                        await Task.Delay(1000);
                        deploy.LogMessage(i + "...");
                    }

                    deploy.LogMessage("Starting TeamViewer assignment...");
                    var assignExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(GetTVAssignString());
                    if (assignExitCode == 0)
                    {
                        deploy.LogMessage("Assignment complete!");
                    }
                    else
                    {
                        deploy.LogMessage("Assignment failed!");
                        OtherFunctions.Message("Error occurred while executing assignment command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    deploy.LogMessage("Deleting temp files...");
                    if (!await deploy.PowerShellWrap.InvokePowerShellCommand(GetDeleteDirectoryCommand()))
                    {
                        deploy.LogMessage("Delete failed!");
                        return false;
                    }

                    deploy.LogMessage("Done.");
                    deploy.LogMessage("-------------------");
                    deploy.LogMessage("TeamView deployment is complete!");
                    deploy.LogMessage("NOTE: The target computer may need rebooted or the user may need to open the application before TeamViewer will connect.");
                    return true;
                }
                else
                {
                    OtherFunctions.Message("The target device is null or does not have a hostname.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Missing Info", parentForm);
                }

                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                deploy.DoneOrError();
            }
        }

        private string GetTVInstallString()
        {
            return "msiexec.exe /i " + fullTempDirectory + deploy.GetString("teamviewer_install");
        }

        private string GetTVUninstallString()
        {
            return "msiexec.exe /x " + fullTempDirectory + deploy.GetString("teamviewer_install");
        }

        private string GetTVAssignString()
        {
            string apiToken = AssetManagerFunctions.GetTVApiToken();
            return fullTempDirectory + deploy.GetString("teamviewer_assign_exe") + " -apitoken " + apiToken + " -datafile " + deploy.GetString("teamviewer_assign_json");
        }

        private Command GetDeleteDirectoryCommand()
        {
            var cmd = new Command("Remove-Item", false, true);
            cmd.Parameters.Add("Recurse");
            cmd.Parameters.Add("Force");
            cmd.Parameters.Add("Path", fullTempDirectory);
            return cmd;
        }

        private async Task<bool> TeamViewerInstalled(Device targetDevice)
        {
            try
            {
                var resultString = await Task.Run(() =>
                {
                    return deploy.PowerShellWrap.ExecuteRemotePSScript(Properties.Resources.CheckForTVRegistryValue, SecurityTools.AdminCreds);
                });
                var result = Convert.ToBoolean(resultString);
                return result;
            }
            catch (FormatException)
            {
                return false;
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
                    deploy?.Dispose();
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