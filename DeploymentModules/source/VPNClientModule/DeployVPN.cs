using DeploymentAssemblies;
using System;
using System.Threading.Tasks;

namespace VPNClientModule
{
    public class DeployVPN : IDeployment
    {
        private IDeploymentUI deploy;

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
                // Make sure this deployment always occurs last, as it causes
                // network disconnects that could interfere with other deployments.
                return 999;
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
                deploy.LogMessage("Installing VPN Client...", MessageType.Notice);

                var exitCode = await deploy.AdvancedPSExecCommand(deploy.GetString("vpn_install"), "VPN Client Install");

                deploy.LogMessage("Exit code: " + exitCode.ToString());

                // Exit code -9 is a connection failure.
                // But this is OK because the VPN install caused a brief network disconnect.
                // So we are assuming that install is successful if this occurs.
                if (exitCode == 0 || exitCode == -9)
                {
                    deploy.LogMessage("VPN Client installed. Exit code -9 is OK.", MessageType.Success);
                }
                else
                {
                    deploy.LogMessage("VPN Install failed!  Unexpected exit code: " + exitCode, MessageType.Error);
                    return false;
                }

                // Pause for a few moments to make sure the network connection is back and the install is finished.
                deploy.LogMessage("Waiting 10 seconds for install to finish...", MessageType.Notice);
                await Task.Delay(10000);

                deploy.LogMessage("Making VPN sites public for all users...", MessageType.Notice);

                var success = await deploy.SimplePSExecCommand(deploy.GetString("vpn_make_public"), "Copy VPN Sites to Public Profile");
                if (success)
                {
                    deploy.LogMessage("Successfully made VPN sites public.", MessageType.Success);
                }
                else
                {
                    deploy.LogMessage("Failed to copy VPN sites to public profile!", MessageType.Error);
                    return false;
                }

                return true;
            }
            catch (Exception ex)
            {
                deploy.LogMessage("UNEXPECTED ERROR: " + ex.ToString(), MessageType.Error);
                return false;
            }
        }
    }
}