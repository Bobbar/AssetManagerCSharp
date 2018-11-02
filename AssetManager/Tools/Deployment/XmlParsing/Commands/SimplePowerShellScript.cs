using DeploymentAssemblies;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System;

namespace AssetManager.Tools.Deployment.XmlParsing.Commands
{
    public sealed class SimplePowerShellScript : DeployCommand
    {
        private byte[] _commandBytes;

        public SimplePowerShellScript(IDeploymentUI ui, string command, string title) : base(ui, command, title)
        {
            _commandBytes = Encoding.ASCII.GetBytes(command);
        }

        public SimplePowerShellScript(IDeploymentUI ui, byte[] command, string title) : base(ui, command, title)
        {
            _commandBytes = command;
        }

        public async override Task<bool> ExecuteReturnSuccess()
        {
            _deploy.UsePowerShell();

            await _deploy.SimplePowerShellScript(_commandBytes, Title);

            return true;
        }

        public override Task<int> ExecuteReturnExitCode()
        {
            throw new NotImplementedException();
        }
    }
}