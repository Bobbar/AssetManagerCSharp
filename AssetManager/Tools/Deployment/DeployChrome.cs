using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Threading.Tasks;
using System.Windows.Forms;
using DeploymentAssemblies;


namespace AssetManager.Tools.Deployment
{
    public class DeployChrome : IDisposable
    {
        private ExtendedForm parentForm;
        private IDeploymentUI deploy;
        private Device targetDevice;

        public DeployChrome(ExtendedForm parentForm, Device targetDevice)
        {
            this.targetDevice = targetDevice;
            this.parentForm = parentForm;
            deploy = new DeploymentUI(parentForm, targetDevice);
            deploy.UsePowerShell();
        }

        public void InitUI(IDeploymentUI ui)
        {
            //this.parentForm = parentForm;
            //deploy = new DeploymentUI(parentForm);
            //deploy.UsePowerShell();
        }

        public async Task<bool> DeployToDevice()
        {
            try
            {
                if (targetDevice != null && !string.IsNullOrEmpty(targetDevice.HostName))
                {
                    // deploy.SetTitle(targetDevice.CurrentUser);
                    deploy.StartTimer();

                    deploy.LogMessage("Starting Chrome deployment to: " + targetDevice.HostName);

                    await deploy.SimplePowerShellScript(Properties.Resources.UpdateChrome, "Chrome Install");
                    deploy.LogMessage("Deployment complete.");
                    return true;

                    //deploy.LogMessage("Deployment failed!");
                    //return false;
                }
                else
                {
                    OtherFunctions.Message("The target device is null or does not have a hostname.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Missing Info", parentForm);
                    return false;
                }
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

            #endregion IDisposable Support
        }


    }
}