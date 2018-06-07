using System;
using System.Threading.Tasks;

namespace AdobeModule
{
    public class DeployAdobe : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Adobe Reader";
            }
        }

        public void InitUI(DeploymentAssemblies.IDeploymentUI ui)
        {
            deploy = ui;
            deploy.UsePsExec();
        }

        public async Task<bool> DeployToDevice()
        {
            try
            {
                await deploy.SimplePSExecCommand(deploy.GetString("adobe_install"), "Adobe Reader Install");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}