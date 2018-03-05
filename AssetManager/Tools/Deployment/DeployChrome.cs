using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.Tools.Deployment
{
    public class DeployChrome : IDisposable
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
            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    deploy.StartTimer();

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
                OtherFunctions.Message("The target device is null or does not have a hostname.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Missing Info", parentForm);
                return false;
            }
            catch (Exception)
            {
                return false;
            }
            finally
            {
                deploy.DoneOrError();
            }
        }

        public void Dispose()
        {
            ((IDisposable)deploy).Dispose();
        }
    }
}