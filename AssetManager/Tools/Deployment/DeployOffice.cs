using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AdminTools;
using MyDialogLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools.Deployment
{
    public class DeployOffice : IDisposable
    {
        private const string deploymentFilesDirectory = "\\\\svr-file1\\dd_files\\Information Technology\\Software\\Office\\RemoteDeploy";// "\\\\core.co.fairfield.oh.us\\dfs1\\fcdd\\files\\Information Technology\\Software\\Office\\RemoteDeploy";
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

        public DeployOffice(ExtendedForm parentForm, DeploymentUI deployUI)
        {
            this.parentForm = parentForm;
            deploy = deployUI;
            deploy.UsePowerShell();
            deploy.UsePsExec();
        }

        private List<string> GetConfigFiles()
        {
            var files = Directory.GetFiles(deploymentFilesDirectory, "*.xml", SearchOption.TopDirectoryOnly);
            var configFiles = new List<string>();

            if (files.Length > 0)
            {
                foreach (var file in files)
                {
                    var fileInfo = new FileInfo(file);
                    configFiles.Add(fileInfo.Name);
                }
            }

            return configFiles;
        }

        private string SelectConfigFile()
        {
            var configFiles = GetConfigFiles();

            var fileCombo = new ComboBox();
            fileCombo.Name = "fileCombo";
            fileCombo.DataSource = configFiles;
            fileCombo.Width = 250;

            using (AdvancedDialog selectDialog = new AdvancedDialog(parentForm))
            {
                selectDialog.Text = "Choose Install Configuration File";
                selectDialog.AddCustomControl("fileCombo", "Config Files:", fileCombo);
                selectDialog.ShowDialog();
                if (selectDialog.DialogResult == DialogResult.OK)
                {
                    return fileCombo.SelectedValue.ToString();
                }
            }
            return string.Empty;
        }

        public async Task<bool> DeployToDevice(Device targetDevice)
        {
            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    string configFile = SelectConfigFile();

                    if (string.IsNullOrEmpty(configFile)) return false;

                    deploy.StartTimer();

                    deploy.LogMessage("Starting new Office 365 deployment to " + targetDevice.HostName);
                    deploy.LogMessage("Config = '" + deploymentFilesDirectory + "\\" + configFile + "'");
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
                            OtherFunctions.Message("Error occurred while pushing deployment files to device!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                        OtherFunctions.Message("Error occurred while executing deployment command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        return false;
                    }

                    deploy.LogMessage("Starting Office 356 deployment...");

                    var installExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(targetDevice, GetO365InstallString(configFile));
                    if (installExitCode == 0)
                    {
                        deploy.LogMessage("Deployment complete!");
                    }
                    else
                    {
                        deploy.LogMessage("Deployment failed! Exit code: " + installExitCode.ToString());
                        OtherFunctions.Message("Error occurred while executing deployment command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
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
                    OtherFunctions.Message("The target device is null or does not have a hostname.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Missing Info", parentForm);
                }

                return false;
            }
            catch (Exception ex)
            {
                Logging.Logger(ex.ToString());
                deploy.LogMessage("Error: " + ex.Message);
                return false;
            }
            finally
            {
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

        private string GetO365InstallString(string configFile)
        {
            string installString = fullDeployTempDir + "\\setup.exe /configure " + fullDeployTempDir + "\\" + configFile;
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