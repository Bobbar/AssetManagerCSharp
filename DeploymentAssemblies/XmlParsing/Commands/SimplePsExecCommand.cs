using System;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeploymentAssemblies.XmlParsing.Commands
{
    public class SimplePsExecCommand : DeploymentCommand
    {
        private string _commandText;

        public string CommandText
        {
            get
            {
                return _commandText;
            }
        }

        public SimplePsExecCommand(XElement cmdElement, IDeploymentUI deployment) : base(cmdElement, deployment)
        {
            _commandText = XmlHelper.GetCommandText(cmdElement);
        }

        public async override Task<CommandCompletedResult> Execute()
        {
            _deploy.UsePsExec();

            _successful = await _deploy.SimplePSExecCommand(_commandText, _title);

            // Look for OnSuccess and/or OnFailure declarations and recurse with their commands if present.
            if (_successful)
            {
                var successElem = _commandElement.Element("OnSuccess");

                if (successElem != null)
                {
                    foreach (var cmd in successElem.Elements())
                    {
                        _commands.Add(cmd);
                    }
                }
            }
            else
            {
                var failElem = _commandElement.Element("OnFailure");

                if (failElem != null)
                {
                    // A True 'Continue' attribute means that failures here are OK and we should coninute with the next commands.
                    var continueVal = XmlHelper.GetAttribute(failElem, "Continue");
                    bool canContinue = Convert.ToBoolean(continueVal);

                    // Set success back to true since failures are allowed.
                    if (!canContinue)
                        _stopDeployment = true;

                    foreach (var cmd in failElem.Elements())
                    {
                        _commands.Add(cmd);
                    }
                }
                else
                {
                    _stopDeployment = true;
                }
            }

            return new CommandCompletedResult(_successful, _stopDeployment, _commands);
        }
    }
}