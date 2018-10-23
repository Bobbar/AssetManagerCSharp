using DeploymentAssemblies;
using System;
using System.Threading.Tasks;

namespace ChromeModule
{
    public class DeployChrome : IDeployment
    {
        private IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Google Chrome";
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
        }

        public async Task<bool> DeployToDevice()
        {
            try
            {
                await deploy.SimplePowerShellScript(Resources.UpdateChrome, "Chrome Install");

                deploy.LogMessage("Chrome Deployment Complete.");

                return true;

            }
            catch (Exception)
            {
                deploy.LogMessage("Deployment failed!", MessageType.Error);
                return false;
            }

        }


    }
}
