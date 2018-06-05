using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentAssemblies;

namespace GatekeeperModule
{
    public class DeployGatekeeper : DeploymentAssemblies.IDeployment
    {
        private DeploymentAssemblies.IDeploymentUI deploy;

        public string DeploymentName
        {
            get
            {
                return "Getkeeper Install";
            }
        }

        public void InitUI(DeploymentAssemblies.IDeploymentUI ui)
        {
            deploy = ui;
            deploy.UsePowerShell();
        }

        public async Task<bool> DeployToDevice()
        {

            await deploy.SimplePSExecCommand(deploy.GetString("gk_client"), "Gatekeeper Client Install");
            await deploy.SimplePSExecCommand(deploy.GetString("gk_update"), "Gatekeeper Update Install");

            deploy.LogMessage("Applying Gatekeeper Registry Fix...");
            deploy.LogMessage("Starting remote session...");

            deploy.LogMessage("Invoking script...");

            var regFixCommand = GetRegFixCommand();

            if (await deploy.SimplePowershellCommand(regFixCommand))
            {
                deploy.LogMessage("GK Registry fix applied!");
            }
            else
            {
                deploy.LogMessage("Failed to apply GK Registry fix!");
                deploy.UserPrompt("Error occurred while executing command!", "Gatekeeper Deployment Error");
                return false;
            }

            return false;
        }

        private PowerShellCommand GetRegFixCommand()
        {
            var command = new PowerShellCommand("Remove-ItemProperty");
            command.Parameters.Add("Path", deploy.GetString("gk_regfix"));
            command.Parameters.Add("Name", "CommLinks");

            return command;
        }


    }
}
