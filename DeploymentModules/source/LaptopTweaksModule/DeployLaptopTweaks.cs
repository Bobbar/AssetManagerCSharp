using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentAssemblies;

namespace LaptopTweaksModule
{
    public class DeployLaptopTweaks : IDeployment
    {
        private IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Laptop/Windows Tablet Tweaks";
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
            deploy.UsePsExec();
        }

        public async Task<bool> DeployToDevice()
        {
            deploy.LogMessage("Applying tweaks for laptop/tablet devices...", MessageType.Notice);

            // Disable PROSet.
            deploy.LogMessage("*** DISABLE IntelPROSet SERVICE ***", MessageType.Notice);

            await deploy.AdvancedPSExecCommand(deploy.GetString("regtweak_disableproset"), "Disable IntelPROSet Service");
            deploy.LogMessage("Done. Disregard any exit codes.", MessageType.Success);
            deploy.LogMessage(" ");

            // Add FCDD-PRIVATE Wifi.
            deploy.LogMessage("*** ADD FCDD-PRIVATE PUBLIC WIFI PROFILE ***", MessageType.Notice);

            var exitCode = await deploy.AdvancedPSExecCommand(deploy.GetString("regtweak_copywifi"), "Copy FCDD-PRIVATE Profile");

            if (exitCode == 0 || exitCode == 1)
            {
                deploy.LogMessage("Profile copied.", MessageType.Success);
            }
            else
            {
                deploy.LogMessage("Failed to copy profile to device!", MessageType.Error);
                return false;
            }

            deploy.LogMessage("Deleting any existing profiles...", MessageType.Notice);

            await deploy.SimplePSExecCommand(deploy.GetString("regtweak_deletewifi"), "Delete Existing Wifi Profile");

            deploy.LogMessage("Importing FCDD-PRIVATE WiFi profile...", MessageType.Notice);

            var importSuccess = await deploy.SimplePSExecCommand(deploy.GetString("regtweak_importwifi"), "Import Wifi Profile");

            if (!importSuccess)
            {
                deploy.LogMessage("Failed to import the Wifi profile!", MessageType.Error);
                return false;
            }

            deploy.LogMessage("Deleting temp directory...");

            var deleteSuccess = await deploy.SimplePSExecCommand(deploy.GetString("regtweak_delete_temp"), "Delete Temp Directory");
            
            if (!deleteSuccess)
            {
                deploy.LogMessage("Failed to delete temp directory!", MessageType.Error);
                // We'll let this pass without a return and continue with the next step.
            } 

            deploy.LogMessage(" ");

            // Disable sleep on close.
            deploy.LogMessage("*** DISABLE SLEEP ON CLOSE ***", MessageType.Notice);

            await deploy.AdvancedPSExecCommand(deploy.GetString("regtweak_disablesleepclose"), "Disable Sleep On Close");

            deploy.LogMessage("Done. Disregard any exit codes.", MessageType.Success);


            return true;
        }
    }
}
