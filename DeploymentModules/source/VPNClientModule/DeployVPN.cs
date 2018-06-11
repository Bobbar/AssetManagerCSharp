using DeploymentAssemblies;
using System;
using System.Threading.Tasks;

namespace VPNClientModule
{
    public class DeployVPN : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "ShrewSoft VPN Client";
            }
        }

        public int DeployOrderPriority
        {
            get
            {
                return 99;
            }
        }

        public void InitUI(IDeploymentUI ui)
        {
            deploy = ui;
            deploy.UsePsExec();
        }

        public async Task<bool> DeployToDevice()
        {
            try
            {
                deploy.LogMessage("Installing VPN Client... (Remember to open client and set FCBDD Profile to 'Public')");
                await deploy.SimplePSExecCommand(deploy.GetString("vpn_install"), "VPN Client Install");
                return true;
            }
            catch (Exception)
            {
                deploy.LogMessage("##### NOTE:  Errors are expected due to the installation causing the device to momentarily disconnect.");
                // Return true because errors are expected and we don't want to stop any proceeding deployments.
                return true;
            }
        }
    }
}