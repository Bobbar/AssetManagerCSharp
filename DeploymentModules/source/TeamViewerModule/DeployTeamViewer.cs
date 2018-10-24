using DeploymentAssemblies;
using System;
using System.Threading.Tasks;

namespace TeamViewerModule
{
    public class DeployTeamViewer : IDeployment
    {
        private IDeploymentUI deploy;

        private string tempDirectory;
        private string fullTempDirectory;

        public string DeploymentName
        {
            get
            {
                return "TeamViewer";
            }
        }

        public int DeployOrderPriority
        {
            get
            {
                return 0;
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

                var copyExitCode = await deploy.AdvancedPSExecCommand(deploy.GetString("teamviewer_copy"), "Copy Deployement Files");

                if (copyExitCode == 0 || copyExitCode == 1)
                {
                    deploy.LogMessage("Copy successful!", MessageType.Success);
                }
                else
                {
                    deploy.LogMessage("Copy failed!", MessageType.Error);
                    return false;
                }

                deploy.LogMessage("Checking for previous installation...", MessageType.Notice);

                tvExists = await TeamViewerInstalled();

                if (tvExists)
                {
                    deploy.LogMessage("TeamViewer already installed.", MessageType.Warning);
                    deploy.LogMessage("Uninstalling TeamViewer...", MessageType.Notice);

                    if (await deploy.SimplePSExecCommand(GetTVUninstallString(), "Uninstall TeamViewer"))
                    {
                        deploy.LogMessage("Uninstall complete!", MessageType.Success);
                    }
                    else
                    {
                        deploy.LogMessage("Uninstall failed!", MessageType.Error);
                        deploy.UserPrompt("Error occurred while executing deployment command!");
                        return false;
                    }
                }
                else
                {
                    deploy.LogMessage("TeamViewer not installed.", MessageType.Notice);
                }

                if (await deploy.SimplePSExecCommand(GetTVInstallString(), "Install TeamViewer"))
                {
                    deploy.LogMessage("Install complete!", MessageType.Success);
                }
                else
                {
                    deploy.LogMessage("Install failed!", MessageType.Error);
                    deploy.UserPrompt("Error occurred while executing deployment command!");
                    return false;
                }

                deploy.LogMessage("Waiting 10 seconds.", MessageType.Notice);
                for (var i = 10; i >= 1; i--)
                {
                    await Task.Delay(1000);
                    deploy.LogMessage(i + "...", MessageType.Notice);
                }

                if (await deploy.SimplePSExecCommand(GetTVAssignString(), "TeamView Assignment"))
                {
                    deploy.LogMessage("Assignment complete!", MessageType.Success);
                }
                else
                {
                    deploy.LogMessage("Assignment failed!", MessageType.Error);
                    deploy.UserPrompt("Error occurred while executing assignment command!");
                    return false;
                }

                deploy.LogMessage("Deleting temp files...", MessageType.Notice);

                if (!await deploy.SimplePowerShellCommand(GetDeleteDirectoryCommand()))
                {
                    deploy.LogMessage("Delete failed!", MessageType.Error);
                    return false;
                }

                deploy.LogMessage("Done.", MessageType.Success);
                deploy.LogMessage("TeamView deployment is complete!");
                deploy.LogMessage("NOTE: The target computer may need rebooted or the user may need to open the application before TeamViewer will connect.", MessageType.Notice);
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
            tempDirectory = deploy.GetString("teamviewer_temp_dir");
            fullTempDirectory = "C:" + tempDirectory;
        }
    }
}