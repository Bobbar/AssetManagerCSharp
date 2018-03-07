using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using MyDialogLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools.Deployment
{
    public class NewDeviceDeployment : IDisposable
    {
        private const string deploymentFilesDirectory = "\\\\svr-file1\\dd_files\\Information Technology\\Software\\Office\\RemoteDeploy";// "\\\\core.co.fairfield.oh.us\\dfs1\\fcdd\\files\\Information Technology\\Software\\Office\\RemoteDeploy";
        private const string deployTempDirectory = "\\Temp\\OfficeDeploy";
        private const string fullDeployTempDir = "C:" + deployTempDirectory;
        private const string removeOfficeScriptPath = fullDeployTempDir + "\\Remove-PreviousOfficeInstalls";

        private Queue<Func<Task<bool>>> deployments = new Queue<Func<Task<bool>>>();

        private ExtendedForm parentForm;

        private DeploymentUI deploy;

        public NewDeviceDeployment(ExtendedForm parentForm)
        {
            this.parentForm = parentForm;
            deploy = new DeploymentUI(parentForm);
            deploy.UsePowerShell();
            deploy.UsePsExec();
        }

        /// <summary>
        /// Container for holding deployment methods and a descriptive name.
        /// </summary>
        private struct TaskInfo
        {
            public Func<Task<bool>> TaskMethod { get; set; }
            public string TaskName { get; set; }

            public TaskInfo(Func<Task<bool>> taskMethod, string taskName)
            {
                TaskMethod = taskMethod;
                TaskName = taskName;
            }
        }

        /// <summary>
        /// List of available deployment methods.
        /// </summary>
        /// <param name="targetDevice"></param>
        /// <returns></returns>
        private List<TaskInfo> DeploymentTasks(Device targetDevice)
        {
            var depList = new List<TaskInfo>();
            depList.Add(new TaskInfo(() => EnableAdmin(targetDevice), "Enable Local Admin"));
            depList.Add(new TaskInfo(() => InstallMVPS(targetDevice), "MVPS Hosts File"));
            depList.Add(new TaskInfo(() => InstallOffice(targetDevice), "Office 365"));
            depList.Add(new TaskInfo(() => InstallGatekeeper(targetDevice), "Gatekeeper"));
            depList.Add(new TaskInfo(() => InstallIntellivue(targetDevice), "Intellivue"));
            depList.Add(new TaskInfo(() => InstallChrome(targetDevice), "Chrome"));
            depList.Add(new TaskInfo(() => InstallCarbonBlack(targetDevice), "Carbon Black"));
            depList.Add(new TaskInfo(() => InstallVPNClient(targetDevice), "Shrewsoft VPN"));
            return depList;
        }

        /// <summary>
        /// Prompt user for items to be deployed to the device.
        /// </summary>
        /// <param name="targetDevice"></param>
        private void ChooseDeployments(Device targetDevice)
        {
            using (var newDialog = new AdvancedDialog(parentForm))
            {
                var depList = DeploymentTasks(targetDevice);

                var selectListBox = new CheckedListBox();
                selectListBox.CheckOnClick = true;
                selectListBox.Size = new System.Drawing.Size(300, 200);
                selectListBox.DisplayMember = nameof(TaskInfo.TaskName);

                foreach (var d in depList)
                {
                    selectListBox.Items.Add(d, true);
                }

                newDialog.AddCustomControl("TaskList", "Select items to install:", selectListBox);
                newDialog.ShowDialog();
                if (newDialog.DialogResult == DialogResult.OK)
                {
                    foreach (TaskInfo task in selectListBox.CheckedItems)
                    {
                        deployments.Enqueue(task.TaskMethod);
                    }
                }
            }
        }

        public async Task<bool> DeployToDevice(Device targetDevice)
        {
            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    deploy.StartTimer();

                    deploy.LogMessage("Starting new device deployment to " + targetDevice.HostName);
                    deploy.LogMessage("-------------------");

                    ChooseDeployments(targetDevice);

                    // Run through the queue and invoke the items sequentially.
                    while (deployments.Any())
                    {
                        // Dequeue returns the next method.
                        var d = deployments.Dequeue();

                        // Invoke the method and return only on failures.
                        if (!await d.Invoke())
                        {
                            return false;
                        }
                    }

                    deploy.LogMessage("Done.");
                    deploy.LogMessage("-------------------");
                    deploy.LogMessage("New Device deployment is complete!");
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

        #region DeploymentMethods

        private async Task<bool> EnableAdmin(Device targetDevice)
        {
            deploy.LogMessage("Enabling Local Admin Account...");
            deploy.LogMessage("Starting remote session...");
            var enableAdminSession = await GetSetLocalAdminSession(targetDevice);
            deploy.LogMessage("Invoking script...");
            if (await deploy.PowerShellWrap.InvokePowerShellSession(enableAdminSession))
            {
                deploy.LogMessage("Local Admin enabled!");
            }
            else
            {
                deploy.LogMessage("Failed to enable local Admin!");
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private async Task<bool> InstallMVPS(Device targetDevice)
        {
            deploy.LogMessage("Installing MVPS...");
            var installMVPSExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(targetDevice, GetMVPSInstallString());
            if (installMVPSExitCode == 0)
            {
                deploy.LogMessage("MVPS Installed!");
            }
            else
            {
                deploy.LogMessage("MVPS Install failed! Exit code: " + installMVPSExitCode.ToString());
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private async Task<bool> InstallGatekeeper(Device targetDevice)
        {
            deploy.LogMessage("Installing Gatekeeper Client...");
            var installGKClientExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(targetDevice, GetGKClientInstallString());
            if (installGKClientExitCode == 0)
            {
                deploy.LogMessage("Gatekeeper Client Installed!");
            }
            else
            {
                deploy.LogMessage("Gatekeeper Client Install failed! Exit code: " + installGKClientExitCode.ToString());
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            deploy.LogMessage("Installing Gatekeeper Update...");
            var installGKUpdateExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(targetDevice, GetGKUpdateString());
            if (installGKUpdateExitCode == 0)
            {
                deploy.LogMessage("Gatekeeper Update Installed!");
            }
            else
            {
                deploy.LogMessage("Gatekeeper Update Install failed! Exit code: " + installGKUpdateExitCode.ToString());
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            deploy.LogMessage("Applying Gatekeepr Registry Fix...");
            deploy.LogMessage("Starting remote session...");
            var applyGKRegFixSession = await GetGKRegFixSession(targetDevice);

            deploy.LogMessage("Invoking script...");
            if (await deploy.PowerShellWrap.InvokePowerShellSession(applyGKRegFixSession))
            {
                deploy.LogMessage("GK Registry fix applied!");
            }
            else
            {
                deploy.LogMessage("Failed to apply GK Registry fix!");
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private async Task<bool> InstallIntellivue(Device targetDevice)
        {
            deploy.LogMessage("Installing Intellivue...");
            var installIVueExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(targetDevice, GetIVueInstallString());
            if (installIVueExitCode == 0)
            {
                deploy.LogMessage("Intellivue Installed!");
            }
            else
            {
                deploy.LogMessage("Intellivue Install failed! Exit code: " + installIVueExitCode.ToString());
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }

            return true;
        }

        private async Task<bool> InstallChrome(Device targetDevice)
        {
            deploy.LogMessage("Installing Chrome...");
            if (await deploy.PowerShellWrap.ExecutePowerShellScript(targetDevice.HostName, Properties.Resources.UpdateChrome))
            {
                deploy.LogMessage("Chrome install complete.");
            }
            else
            {
                deploy.LogMessage("Chrome install failed!");
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private async Task<bool> InstallCarbonBlack(Device targetDevice)
        {
            deploy.LogMessage("Installing Carbon Black...");
            var installCBExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(targetDevice, GetCarbonBlackInstallString());
            if (installCBExitCode == 0)
            {
                deploy.LogMessage("Carbon Black Installed!");
            }
            else
            {
                deploy.LogMessage("Carbon Black Install failed! Exit code: " + installCBExitCode.ToString());
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }

        private async Task<bool> InstallOffice(Device targetDevice)
        {
            var newOfficeDeploy = new DeployOffice(parentForm, deploy);
            return await newOfficeDeploy.DeployToDevice(targetDevice);
        }


        private async Task<bool> InstallVPNClient(Device targetDevice)
        {
            deploy.LogMessage("Installing VPN Client... (Remember to open client and set FCBDD Profile to 'Public')");
            var installVPNExitCode = await deploy.PSExecWrap.ExecuteRemoteCommand(targetDevice, GetVPNInstallString());
            if (installVPNExitCode == 0)
            {
                deploy.LogMessage("VPN Client Installed!");
            }
            else
            {
                deploy.LogMessage("VPN Client Install failed! Exit code: " + installVPNExitCode.ToString());
                OtherFunctions.Message("Error occurred while executing command!", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
            return true;
        }


        #endregion DeploymentMethods


        #region DeploymentSupportMethods

        private async Task<PowerShell> GetSetLocalAdminSession(Device targetDevice)
        {
            var session = await deploy.PowerShellWrap.GetNewPSSession(targetDevice.HostName, SecurityTools.AdminCreds);

            var setAdminPassCommand = new Command("net user Administrator \"057|750\"", true);
            var setAdminActiveCommand = new Command(@"net user Administrator /active:yes", true);

            session.Commands.AddCommand(setAdminPassCommand);
            session.Commands.AddCommand(setAdminActiveCommand);

            return session;
        }

        private string GetMVPSInstallString()
        {
            string installString = "PowerShell.exe \"& \"\"\\\\svr-file1\\dd_files\\Information Technology\\Software\\MVPS\\powershell_install.ps1\"\"\"";
            return installString;
        }

        private string GetGKClientInstallString()
        {
            string installString = @"msiexec.exe /i \\svr-ddas1.core.co.fairfield.oh.us\PSiServ\Gatekeeper\Install\Full\ClientFull.msi REINSTALL=ALL REINSTALLMODE=omus /qn";
            return installString;
        }

        private string GetGKUpdateString()
        {
            string updateString = @"msiexec.exe /i \\svr-ddas1.core.co.fairfield.oh.us\PSiServ\Gatekeeper\Install\Upgrade\Setup.msi REINSTALL=ALL REINSTALLMODE=omus /qn";
            return updateString;
        }

        private async Task<PowerShell> GetGKRegFixSession(Device targetDevice)
        {
            var session = await deploy.PowerShellWrap.GetNewPSSession(targetDevice.HostName, SecurityTools.AdminCreds);

            var registryFixCommand = new Command("Remove-ItemProperty");
            registryFixCommand.Parameters.Add("Path", @"HKLM:\SOFTWARE\Wow6432Node\ODBC\ODBC.INI\Gatekeeper");
            registryFixCommand.Parameters.Add("Name", "CommLinks");
            session.Commands.AddCommand(registryFixCommand);

            return session;
        }

        private string GetIVueInstallString()
        {
            string installString = @"cmd /c \\ddad-svr-fs02\Intellivue\GXInstaller\IVueInstaller.exe";
            return installString;
        }

        private string GetCarbonBlackInstallString()
        {
            string installString = @"msiexec.exe /i ""\\core.co.fairfield.oh.us\dfs1\pub\misc\CbDefense\installer_vista_win7_win8-64-2.1.0.9.msi"" ALLUSERS=1 /qn /norestart /log C:\Temp\output.log COMPANY_CODE=3JZ1Q5E4 GROUP_NAME=default BYPASS=0";
            return installString;
        }

        private string GetVPNInstallString()
        {
            string installString = @"""\\core.co.fairfield.oh.us\dfs1\fcdd\files\Information Technology\Software\vpn-client-2.2.2-release.exe"" / S / STANDARD";
            return installString;
        }
        #endregion DeploymentSupportMethods

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