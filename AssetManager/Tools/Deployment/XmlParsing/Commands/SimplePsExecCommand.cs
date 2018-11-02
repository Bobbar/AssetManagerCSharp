using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentAssemblies;

namespace AssetManager.Tools.Deployment.XmlParsing
{
    public sealed class SimplePsExecCommand : DeployCommand
    {
        public SimplePsExecCommand(IDeploymentUI ui, string command, string title) : base(ui, command, title)
        {
        }

        public async override Task<bool> ExecuteReturnSuccess()
        {
            _deploy.UsePsExec();

            return await _deploy.SimplePSExecCommand(CommandText, Title);
        }

        public override Task<int> ExecuteReturnExitCode()
        {
            throw new NotImplementedException();
        }
    }
}
