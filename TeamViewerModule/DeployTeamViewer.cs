using DeploymentAssemblies;
using System;
using System.Threading.Tasks;

namespace TeamViewerModule
{
    public class DeployTeamViewer : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        private string filesDirectory;
        private string tempDirectory;
        private string fullTempDirectory;

        public string DeploymentName
        {
            get
            {
                return "TeamViewer";
            }
        }

        public void InitUI(IDeploymentUI ui)
        {
            deploy = ui;
            deploy.UsePowerShell();
            deploy.UsePsExec();
            GetDirectories();
        }

        public async Task<bool> DeployToDevice()
        {
            try
            {
                bool tvExists = false;

                deploy.LogMessage("Starting new TeamViewer deployment to " + deploy.TargetHostname);
                deploy.LogMessage("-------------------");

                var filePush = deploy.NewFilePush(filesDirectory, tempDirectory);

                deploy.LogMessage("Pushing files to target computer...");

                if (await filePush.StartCopy())
                {
                    deploy.LogMessage("Push successful!");
                    filePush.Dispose();
                }
                else
                {
                    deploy.LogMessage("Push failed!");
                    deploy.UserPrompt("Error occurred while pushing deployment files to device!");
                    return false;
                }

                deploy.LogMessage("Checking for previous installation...");

                tvExists = await TeamViewerInstalled();

                if (tvExists)
                {
                    deploy.LogMessage("TeamViewer already installed.");
                    deploy.LogMessage("Uninstalling TeamViewer...");

                    if (await deploy.SimplePSExecCommand(GetTVUninstallString(), "Uninstall TeamViewer"))
                    {
                        deploy.LogMessage("Uninstall complete!");
                    }
                    else
                    {
                        deploy.LogMessage("Uninstall failed!");
                        deploy.UserPrompt("Error occurred while executing deployment command!");
                        return false;
                    }
                }
                else
                {
                    deploy.LogMessage("TeamViewer not installed.");
                }

                deploy.LogMessage("Starting TeamViewer install...");

                if (await deploy.SimplePSExecCommand(GetTVInstallString(), "Install TeamViewer"))
                {
                    deploy.LogMessage("Install complete!");
                }
                else
                {
                    deploy.LogMessage("Install failed!");
                    deploy.UserPrompt("Error occurred while executing deployment command!");
                    return false;
                }

                deploy.LogMessage("Waiting 10 seconds.");
                for (var i = 10; i >= 1; i--)
                {
                    await Task.Delay(1000);
                    deploy.LogMessage(i + "...");
                }

                deploy.LogMessage("Starting TeamViewer assignment...");

                if (await deploy.SimplePSExecCommand(GetTVAssignString(), "TeamView Assignment"))
                {
                    deploy.LogMessage("Assignment complete!");
                }
                else
                {
                    deploy.LogMessage("Assignment failed!");
                    deploy.UserPrompt("Error occurred while executing assignment command!");
                    return false;
                }

                deploy.LogMessage("Deleting temp files...");

                if (!await deploy.SimplePowerShellCommand(GetDeleteDirectoryCommand()))
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
            catch (Exception)
            {
                return false;
            }
        }

        private string GetTVAssignString()
        {
            var apiToken = deploy.GetString("teamviewer_apitoken");
            return fullTempDirectory + deploy.GetString("teamviewer_assign_exe") + " -apitoken " + apiToken + " -datafile " + deploy.GetString("teamviewer_assign_json");
        }

        private string GetTVInstallString()
        {
            return "msiexec.exe /i " + fullTempDirectory + deploy.GetString("teamviewer_install");
        }

        private string GetTVUninstallString()
        {
            return "msiexec.exe /x " + fullTempDirectory + deploy.GetString("teamviewer_install");
        }

        private PowerShellCommand GetDeleteDirectoryCommand()
        {
            var cmd = new PowerShellCommand("Remove-Item", false, true);
            cmd.Parameters.Add("Recurse");
            cmd.Parameters.Add("Force");
            cmd.Parameters.Add("Path", fullTempDirectory);
            return cmd;
        }

        private async Task<bool> TeamViewerInstalled()
        {
            try
            {
                var resultString = await deploy.AdvancedPowerShellScript(Resources.CheckForTVRegistryValue);
                var result = Convert.ToBoolean(resultString);
                return result;
            }
            catch (FormatException)
            {
                return false;
            }
        }

        private void GetDirectories()
        {
            filesDirectory = deploy.GetString("teamviewer_deploy_dir");
            tempDirectory = deploy.GetString("teamviewer_temp_dir");
            fullTempDirectory = "C:" + tempDirectory;
        }
    }
}