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
    public class DeployTeamViewer
    {
        private const string deploymentFilesDirectory = "\\\\core.co.fairfield.oh.us\\dfs1\\fcdd\\files\\Information Technology\\Software\\Tools\\TeamViewer\\Deploy";
        private const string deployTempDirectory = "\\Temp\\TVDeploy";

        private ExtendedForm parentForm;

        private DeploymentUI deploy;

        public DeployTeamViewer(ExtendedForm parentForm)
        {
            this.parentForm = parentForm;
            deploy = new DeploymentUI(parentForm);
            deploy.UsePowerShell();
        }

        public async Task<bool> DeployToDevice(Device targetDevice)
        {
            long startTime = 0;

            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    startTime = DateTime.Now.Ticks;

                    bool TVExists = false;

                    deploy.LogMessage("Starting new TeamViewer deployment to " + targetDevice.HostName);
                    deploy.LogMessage("-------------------");

                    using (CopyFilesForm PushForm = new CopyFilesForm(parentForm, targetDevice, deploymentFilesDirectory, deployTempDirectory))
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
                            OtherFunctions.Message("Error occurred while pushing deployment files to device!");
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
                        deploy.LogMessage("Reinstalling TeamViewer...");

                        if (await deploy.PowerShellWrap.InvokePowerShellCommand(targetDevice.HostName, GetTVReinstallCommand()))
                        {
                            deploy.LogMessage("Deployment complete!");
                        }
                        else
                        {
                            deploy.LogMessage("Deployment failed!");
                            OtherFunctions.Message("Error occurred while executing deployment command!");
                            return false;
                        }
                    }
                    else
                    {
                        deploy.LogMessage("Starting TeamViewer deployment...");
                        if (await deploy.PowerShellWrap.InvokePowerShellCommand(targetDevice.HostName, GetTVInstallCommand()))
                        {
                            deploy.LogMessage("Deployment complete!");
                        }
                        else
                        {
                            deploy.LogMessage("Deployment failed!");
                            OtherFunctions.Message("Error occurred while executing deployment command!");
                            return false;
                        }
                    }

                    deploy.LogMessage("Waiting 10 seconds.");
                    for (var i = 10; i >= 1; i--)
                    {
                        await Task.Delay(1000);
                        deploy.LogMessage(i + "...");
                    }

                    deploy.LogMessage("Starting TeamViewer assignment...");
                    if (await deploy.PowerShellWrap.InvokePowerShellCommand(targetDevice.HostName, GetTVAssignCommand()))
                    {
                        deploy.LogMessage("Assignment complete!");
                    }
                    else
                    {
                        deploy.LogMessage("Assignment failed!");
                        OtherFunctions.Message("Error occurred while executing assignment command!");
                        return false;
                    }

                    deploy.LogMessage("Deleting temp files...");
                    if (!await deploy.PowerShellWrap.InvokePowerShellCommand(targetDevice.HostName, GetDeleteDirectoryCommand()))
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
                    deploy.LogMessage("Run Time: " + runTimeSeconds + " s");
                }

                deploy.DoneOrError();
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
                    return deploy.PowerShellWrap.ExecuteRemotePSScript(targetDevice.HostName, Properties.Resources.CheckForTVRegistryValue, SecurityTools.AdminCreds);
                });
                var result = Convert.ToBoolean(resultString);
                return result;
            }
            catch (FormatException)
            {
                return false;
            }
        }
    }
}