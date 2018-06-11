using AdvancedDialog;
using DeploymentAssemblies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace OfficeModule
{
    public class DeployOffice : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        private string deployFilesDirectory;
        private string deployTempDirectory;
        private string fullDeployTempDir;
        private string removeOfficeScriptPath;

        public string DeploymentName
        {
            get
            {
                return "Office 365";
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
                string configFile = SelectConfigFile();

                if (string.IsNullOrEmpty(configFile)) return false;

                deploy.LogMessage("Starting new Office 365 deployment to " + deploy.TargetHostname);
                deploy.LogMessage("Config = '" + deployFilesDirectory + "\\" + configFile + "'");
                deploy.LogMessage("-------------------");

                var filePush = deploy.NewFilePush(deployFilesDirectory, deployTempDirectory);

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

                deploy.LogMessage("Removing previous Office installations...");

                deploy.LogMessage("Starting remote session and invoking scripts...");

                if (await deploy.SimplePowerShellCommand(GetRemovalCommands()))
                {
                    deploy.LogMessage("Previous Office installations removed.");
                }
                else
                {
                    deploy.LogMessage("Failed to remove previous installations!");
                    deploy.UserPrompt("Error occurred while executing deployment command!");
                    return false;
                }

                var installCommand = GetO365InstallString(configFile);

                if (await deploy.SimplePSExecCommand(installCommand, "Office 365 Install"))
                {
                    deploy.LogMessage("Deployment complete!");
                }
                else
                {
                    deploy.LogMessage("Deployment failed!");
                    deploy.UserPrompt("Error occurred while executing deployment command!");
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
                deploy.LogMessage("Office 365 deployment is complete!");
                return true;
            }
            catch (Exception ex)
            {
                deploy.LogMessage("Error: " + ex.Message);
                return false;
            }
        }

        private PowerShellCommand GetDeleteDirectoryCommand()
        {
            var cmd = new PowerShellCommand("Remove-Item", false, true);
            cmd.Parameters.Add("Recurse");
            cmd.Parameters.Add("Force");
            cmd.Parameters.Add("Path", fullDeployTempDir);
            return cmd;
        }

        private string GetO365InstallString(string configFile)
        {
            string installString = fullDeployTempDir + "\\setup.exe /configure " + fullDeployTempDir + "\\" + configFile;
            return installString;
        }

        private PowerShellCommand[] GetRemovalCommands()
        {
            var commmands = new List<PowerShellCommand>();

            var removeHubCmd = new PowerShellCommand(deploy.GetString("remove_office_hub"), true, true);
            removeHubCmd.Parameters.Add("Wait");
            removeHubCmd.Parameters.Add("NoNewWindow");
            commmands.Add(removeHubCmd);

            var removeDesktopAppsCmd = new PowerShellCommand(deploy.GetString("remove_office_desktop"), true, true);
            removeDesktopAppsCmd.Parameters.Add("Wait");
            removeDesktopAppsCmd.Parameters.Add("NoNewWindow");
            commmands.Add(removeDesktopAppsCmd);

            var setLocationCmd = new PowerShellCommand("Set-Location");
            setLocationCmd.Parameters.Add("Path", removeOfficeScriptPath);
            commmands.Add(setLocationCmd);

            var removeOfficeCmd = new PowerShellCommand(removeOfficeScriptPath + deploy.GetString("office_remove_script_name"), true, true);
            removeOfficeCmd.Parameters.Add("Wait");
            removeOfficeCmd.Parameters.Add("NoNewWindow");
            commmands.Add(removeOfficeCmd);

            return commmands.ToArray();
        }

        private void GetDirectories()
        {
            deployFilesDirectory = deploy.GetString("office_deploy_dir");
            deployTempDirectory = deploy.GetString("office_temp_dir");
            fullDeployTempDir = "C:" + deployTempDirectory;
            removeOfficeScriptPath = fullDeployTempDir + deploy.GetString("office_remove_script_dir");
        }

        private string SelectConfigFile()
        {
            var configFiles = GetConfigFiles();

            var fileCombo = new ComboBox();
            fileCombo.Name = "fileCombo";
            fileCombo.DataSource = configFiles;
            fileCombo.Width = 250;

            using (var selectDialog = new Dialog(deploy.ParentForm))
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

        private List<string> GetConfigFiles()
        {
            var files = Directory.GetFiles(deployFilesDirectory, "*.xml", SearchOption.TopDirectoryOnly);
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
    }
}