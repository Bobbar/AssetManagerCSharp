using DeploymentAssemblies;
using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace AssetManager.Tools.Deployment.XmlParsing.Commands
{
    public sealed class PromptCommand : DeployCommand
    {
        public readonly List<UIPrompt> Prompts;

        public PromptCommand(IDeploymentUI ui) : base(ui, string.Empty, null)
        {
        }

        public PromptCommand(IDeploymentUI ui, UIPrompt prompt) : base(ui, string.Empty, null)
        {
            Prompts.Add(prompt);
        }

        public PromptCommand(IDeploymentUI ui, List<UIPrompt> prompts) : base(ui, string.Empty, null)
        {
            Prompts = prompts;
        }

        public async override Task<bool> ExecuteReturnSuccess()
        {
            Prompts.ForEach(p => p.Display());

            return true;
        }

        public async override Task<int> ExecuteReturnExitCode()
        {
            throw new NotImplementedException();
        }
    }
}