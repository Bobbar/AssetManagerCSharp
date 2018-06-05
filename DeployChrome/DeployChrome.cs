using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ChromeModule
{
    [Serializable]
    public class DeployChrome : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Chrome Install";
            }
        }

        public DeployChrome() { }

        public void InitUI(DeploymentAssemblies.IDeploymentUI ui)
        {
            deploy = ui;
            deploy.UsePowerShell();
        }

        public async Task<bool> DeployToDevice()
        {
            try
            {

                if (string.IsNullOrEmpty(deploy.TargetHostname))
                {
                    deploy.UserPrompt("The target device is null or does not have a hostname.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Missing Info");
                    return false;
                }

                deploy.StartTimer();

                deploy.LogMessage("Starting Chrome deployment to: " + deploy.TargetHostname);

                await deploy.SimplePowerShellScript(Resources.UpdateChrome, "Chrome Install");

                deploy.LogMessage("Chrome Deployment Complete.");

                return true;

            }
            catch (Exception)
            {
                deploy.LogMessage("Deployment failed!");
                return false;
            }
            finally
            {
                deploy.DoneOrError();
             
            }

        }


    }
}
