using DeploymentAssemblies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EnableAdminModule
{
    public class EnableAdmin : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Enable Local Admin";
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
        }

        public async Task<bool> DeployToDevice()
        {
            deploy.LogMessage("Enabling Local Admin Account...");
            deploy.LogMessage("Starting remote session and invoking commands...");

            if (await deploy.SimplePowerShellCommand(GetSetLocalAdminCommands()))
            {
                deploy.LogMessage("Local Admin enabled!");
            }
            else
            {
                deploy.LogMessage("Failed to enable local Admin!");
                deploy.UserPrompt("Error occurred while executing command!");
                return false;
            }
            return true;
        }

        private PowerShellCommand[] GetSetLocalAdminCommands()
        {
            var commands = new List<PowerShellCommand>();

            commands.Add(new PowerShellCommand(deploy.GetString("set_admin_pass"), true));
            commands.Add(new PowerShellCommand(deploy.GetString("set_admin_active"), true));

            return commands.ToArray();
        }
    }
}