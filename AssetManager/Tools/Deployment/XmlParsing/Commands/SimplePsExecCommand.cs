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
        public List<UIPrompt> SuccessPrompts = new List<UIPrompt>();
        public List<UIPrompt> FailPrompts = new List<UIPrompt>();

        public SimplePsExecCommand(IDeploymentUI ui, string commandText, string title) : base(ui, commandText, title)
        {
        }

        public SimplePsExecCommand(IDeploymentUI ui, string commandText, string title, UIPrompt successPrompt, UIPrompt failPrompt) : base(ui, commandText, title)
        {
            SuccessPrompts.Add(successPrompt);
            FailPrompts.Add(failPrompt);
        }

        public SimplePsExecCommand(IDeploymentUI ui, string commandText, string title, List<UIPrompt> successPrompts, List<UIPrompt> failPrompts) : base(ui, commandText, title)
        {
            SuccessPrompts = successPrompts;
            FailPrompts = failPrompts;
        }

        public async override Task<bool> Execute()
        {
            _deploy.UsePsExec();

            if (await _deploy.SimplePSExecCommand(CommandText, Title))
            {
                SuccessPrompts.ForEach(p => p.Display());
            }
            else
            {
                FailPrompts.ForEach(p => p.Display());
                return false;
            }

            return true;
        }
    }
}
