using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentAssemblies;

namespace MVPSModule
{
    public class DeployMVPS : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "MVPS Install";
            }
        }

        public DeployMVPS() { }

        public void InitUI(IDeploymentUI ui)
        {
            deploy = ui;
            deploy.UsePsExec();
        }
        public async Task<bool> DeployToDevice()
        {
            try
            {
                await deploy.SimplePSExecCommand(deploy.GetString("mvps_install"), "MVPS Hosts File Install");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

      
    }
}
