using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Tools.RemoteCommands;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools.Deployment
{
    public class DeployChrome
    {
        private ExtendedForm parentForm;
        private DeploymentUI deploy;

        public DeployChrome(ExtendedForm parentForm)
        {
            this.parentForm = parentForm;
            deploy = new DeploymentUI(parentForm);
            deploy.UsePowerShell();
        }

        public async Task<bool> DeployToDevice(Device targetDevice)
        {
            long startTime = 0;

            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    startTime = DateTime.Now.Ticks;

                    deploy.LogMessage("Starting Chrome deployment to: " + targetDevice.HostName);
                    if (await deploy.PowerShellWrap.ExecutePowerShellScript(targetDevice.HostName, Properties.Resources.UpdateChrome))
                    {
                        deploy.LogMessage("Deployment complete.");
                    }
                    else
                    {
                        deploy.LogMessage("Deployment failed!");
                        return false;
                    }
                    return true;
                }
                OtherFunctions.Message("The target device is null or does not have a hostname.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Missing Info", parentForm);
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
    }
}