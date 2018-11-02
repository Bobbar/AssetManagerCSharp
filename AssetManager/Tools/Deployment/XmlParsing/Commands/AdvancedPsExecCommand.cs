using DeploymentAssemblies;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AssetManager.Tools.Deployment.XmlParsing
{
    public sealed class AdvancedPsExecCommand : DeployCommand
    {
        public AdvancedPsExecCommand(IDeploymentUI ui, string commandText, string title) : base(ui, commandText, title)
        {
        }
      
        public override Task<bool> ExecuteReturnSuccess()
        {
            throw new NotImplementedException();
        }

        public async override Task<int> ExecuteReturnExitCode()
        {
            _deploy.UsePsExec();

            return await _deploy.AdvancedPSExecCommand(CommandText, Title);
        }
    }
}