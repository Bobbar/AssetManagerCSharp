using DeploymentAssemblies;
using System;
using System.Threading.Tasks;

namespace IntellivueModule
{
    public class DeployIntellivue : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Install Intellivue";
            }
        }

        public DeployIntellivue()
        {
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
                await deploy.SimplePSExecCommand(deploy.GetString("ivue_install"), "Intellivue Install");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}