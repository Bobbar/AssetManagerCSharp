using DeploymentAssemblies;
using System;
using System.Threading.Tasks;

namespace MapWinGisModule
{
    public class DeployMapWinGis : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "MapWinGIS";
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
                await deploy.SimplePSExecCommand(deploy.GetString("vcredist_install"), "Visual C++ Redist Install");
                await deploy.SimplePSExecCommand(deploy.GetString("mapwingis_install"), "MapWinGIS Install");
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}