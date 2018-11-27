using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeploymentAssemblies.XmlParsing.Commands
{
    public class SimplePowerShellScriptCommand : DeploymentCommand
    {
        private byte[] _scriptBytes;

        public SimplePowerShellScriptCommand(XElement cmdElement, IDeploymentUI deployment) : base(cmdElement, deployment)
        {
            var scriptText = XmlHelper.GetCommandText(cmdElement);
            _scriptBytes = Encoding.ASCII.GetBytes(scriptText);
        }

        public async override Task<CommandCompletedResult> Execute()
        {
            _deploy.UsePowerShell();

            await _deploy.SimplePowerShellScript(_scriptBytes, _title);

            var completeElem = _commandElement.Element("OnComplete");

            _successful = true;

            if (completeElem != null)
            {
                foreach (var cmd in completeElem.Elements())
                {
                    _commands.Add(cmd);
                }
            }

            return new CommandCompletedResult(_successful, _stopDeployment, _commands);
        }
    }
}