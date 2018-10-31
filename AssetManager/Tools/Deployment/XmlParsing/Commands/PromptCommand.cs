using DeploymentAssemblies;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace AssetManager.Tools.Deployment.XmlParsing.Commands
{
    public sealed class PromptCommand : DeployCommand
    {
        public readonly List<UIPrompt> Prompts;

        public PromptCommand(IDeploymentUI ui) : base(ui, null, null)
        {
        }

        public PromptCommand(IDeploymentUI ui, UIPrompt prompt) : base(ui, null, null)
        {
            Prompts.Add(prompt);
        }

        public PromptCommand(IDeploymentUI ui, List<UIPrompt> prompts) : base(ui, null, null)
        {
            Prompts = prompts;
        }

        public async override Task<bool> Execute()
        {
            Prompts.ForEach(p => p.Display());

            return true;
        }
    }
}