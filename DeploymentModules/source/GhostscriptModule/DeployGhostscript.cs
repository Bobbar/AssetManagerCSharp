using DeploymentAssemblies;
using System.Threading.Tasks;

namespace GhostscriptModule
{
    public class DeployGhostscript : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Ghostscript PDF";
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
            deploy.UsePsExec();
        }

        public async Task<bool> DeployToDevice()
        {
            if (await deploy.SimplePSExecCommand(deploy.GetString("ghostscript_driver"), "Ghostscript PDF Driver Install"))
            {
                deploy.LogMessage("Ghostscript Driver Installed.");
            }
            else
            {
                deploy.LogMessage("Ghostscript Driver Install Failed!");
                return false;
            }

            if (await deploy.SimplePSExecCommand(deploy.GetString("ghostscript_install"), "Ghostscript PDF Printer Install"))
            {
                deploy.LogMessage("Ghostscript PDF Printer Installed.");
            }
            else
            {
                deploy.LogMessage("Ghostscript PDF Printer Install Failed!");
                return false;
            }

            return true;
        }
    }
}