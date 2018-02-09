using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AdminTools;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssetManager.Tools.RemoteCommands;


namespace AssetManager.Tools.Office
{
    public class DeployOffice
    {

        private const string deploymentFilesDirectory = "\\\\core.co.fairfield.oh.us\\dfs1\\fcdd\\files\\Information Technology\\Software\\Office\\RemoteDeploy";
        private const string deployTempDirectory = "\\Temp\\OfficeDeploy";
        private const string fullDeployTempDir = "C:" + deployTempDirectory;
        private const string removeOfficeScriptPath = fullDeployTempDir + "\\Remove-PreviousOfficeInstalls";
        private ExtendedForm parentForm;

        private DeploymentUI deploy;

        public DeployOffice(ExtendedForm parentForm)
        {
            this.parentForm = parentForm;
            deploy = new DeploymentUI(parentForm);
            deploy.UsePowerShell();
            deploy.UsePsExec();

        }


        public async Task<bool> DeployToDevice(Device targetDevice)
        {
            long startTime = 0;
            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    startTime = DateTime.Now.Ticks;

                    deploy.LogMessage("Starting new Office 365 deployment to " + targetDevice.HostName);
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

                    deploy.LogMessage("Removing previous Office installations...");

                    deploy.LogMessage("Starting remote session...");
                    var officeRemoveSession = await GetRemoveOfficeSession(targetDevice);

                    deploy.LogMessage("Invoking removal script...");
                    if (await deploy.PowerShellWrap.InvokePowerShellSession(officeRemoveSession))
                    {
                        deploy.LogMessage("Previous Office installations removed.");
                    }
                    else
                    {
                        deploy.LogMessage("Failed to remove previous installations!");
                        OtherFunctions.Message("Error occurred while executing deployment command!");
                        return false;
                    }

                    deploy.LogMessage("Starting Office 356 deployment...");

                    var installExitCode = await deploy.PsExecWrap.ExecuteRemoteCommand(targetDevice, GetO365InstallString());
                    if (installExitCode == 0)
                    {
                        deploy.LogMessage("Deployment complete!");
                    }
                    else
                    {
                        deploy.LogMessage("Deployment failed! Exit code: " + installExitCode.ToString());
                        OtherFunctions.Message("Error occurred while executing deployment command!");
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
                    deploy.LogMessage("Office 365 deployment is complete!");
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


        private async Task<PowerShell> GetRemoveOfficeSession(Device targetDevice)
        {
            var session = await deploy.PowerShellWrap.GetNewPSSession(targetDevice.HostName, SecurityTools.AdminCreds);

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

        private string GetO365InstallString()
        {
            string installString = fullDeployTempDir + "\\setup.exe /configure " + fullDeployTempDir + "\\configuration_local1.xml";
            return installString;
        }

        private Command GetDeleteDirectoryCommand()
        {
            var cmd = new Command("Remove-Item", false, true);
            cmd.Parameters.Add("Recurse");
            cmd.Parameters.Add("Force");
            cmd.Parameters.Add("Path", fullDeployTempDir);
            return cmd;
        }


    }
}
