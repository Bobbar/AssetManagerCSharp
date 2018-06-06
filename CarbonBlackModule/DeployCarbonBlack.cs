using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentAssemblies;

namespace CarbonBlackModule
{
    public class DeployCarbonBlack : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Install Carbon Black";
            }
        }

        public DeployCarbonBlack()
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
                await deploy.SimplePSExecCommand(deploy.GetString("carbonblack_install"), "Carbon Black Install");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
