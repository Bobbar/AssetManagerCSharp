using DeploymentAssemblies;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Tools.Deployment.XmlParsing.Commands
{
    public sealed class SimplePowerShellScript : DeployCommand
    {
        public List<UIPrompt> OnCompletePrompts = new List<UIPrompt>();

        private byte[] _commandBytes;

        public SimplePowerShellScript(IDeploymentUI ui, string commandText, string title) : base(ui, commandText, title)
        {
            _commandBytes = Encoding.ASCII.GetBytes(commandText);
        }

        public SimplePowerShellScript(IDeploymentUI ui, string commandText, string title, UIPrompt onCompletePrompt) : base(ui, commandText, title)
        {
            OnCompletePrompts.Add(onCompletePrompt);
            _commandBytes = Encoding.ASCII.GetBytes(commandText);
        }

        public SimplePowerShellScript(IDeploymentUI ui, string commandText, string title, List<UIPrompt> onCompletePrompts) : base(ui, commandText, title)
        {
            OnCompletePrompts = onCompletePrompts;
            _commandBytes = Encoding.ASCII.GetBytes(commandText);
        }

        public async override Task<bool> Execute()
        {
            _deploy.UsePowerShell();

            await _deploy.SimplePowerShellScript(_commandBytes, Title);

            OnCompletePrompts.ForEach(p => p.Display());

            return true;
        }
    }
}