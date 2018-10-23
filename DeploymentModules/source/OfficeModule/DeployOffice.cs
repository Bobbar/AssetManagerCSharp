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

                deploy.LogMessage("Copying files to target computer...", MessageType.Notice);

                var copyExitCode = await deploy.AdvancedPSExecCommand(deploy.GetString("office_copy_files"), "Copy Deployement Files");

                if (copyExitCode == 0 || copyExitCode == 1)
                {
                    deploy.LogMessage("Copy successful!", MessageType.Success);
                }
                else
                {
                    deploy.LogMessage("Copy failed!", MessageType.Error);
                    return false;
                }

                deploy.LogMessage("Removing previous Office installations...", MessageType.Notice);

                deploy.LogMessage("Starting remote session and invoking scripts...", MessageType.Notice);

                if (await deploy.SimplePowerShellCommand(GetRemovalCommands()))
                {
                    deploy.LogMessage("Previous Office installations removed.", MessageType.Success);
                }
                else
                {
                    deploy.LogMessage("Failed to remove previous installations!", MessageType.Error);
                    deploy.UserPrompt("Error occurred while executing deployment command!");
                    return false;
                }
             
                var installCommand = GetO365InstallString(configFile);

                if (await deploy.SimplePSExecCommand(installCommand, "Office 365 Install"))
                {
                    deploy.LogMessage("Deployment complete!", MessageType.Success);
                }
                else
                {
                    deploy.LogMessage("Deployment failed!", MessageType.Error);
                    deploy.UserPrompt("Error occurred while executing deployment command!");
                    return false;
                }

                deploy.LogMessage("Deleting temp files...", MessageType.Notice);

                if (!await deploy.SimplePowerShellCommand(GetDeleteDirectoryCommand()))
                {
                    deploy.LogMessage("Delete failed!", MessageType.Error);
                    return false;
                }

                deploy.LogMessage("Done.");
                deploy.LogMessage("-------------------");
                deploy.LogMessage("Office 365 deployment is complete!", MessageType.Success);
                return true;
            }
            catch (Exception ex)
            {
                deploy.LogMessage("Error: " + ex.Message, MessageType.Error);
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

            var removeHubCmd = new PowerShellCommand(removeOfficeScriptPath + deploy.GetString("remove_office_hub"), true, true);
            removeHubCmd.Parameters.Add("Wait");
            removeHubCmd.Parameters.Add("NoNewWindow");
            commmands.Add(removeHubCmd);

            var removeOneNoteCmd = new PowerShellCommand(removeOfficeScriptPath + deploy.GetString("remove_office_onenote"), true, true);
            removeOneNoteCmd.Parameters.Add("Wait");
            removeOneNoteCmd.Parameters.Add("NoNewWindow");
            commmands.Add(removeOneNoteCmd);

            var removeDesktopAppsCmd = new PowerShellCommand(removeOfficeScriptPath + deploy.GetString("remove_office_desktop"), true, true);
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