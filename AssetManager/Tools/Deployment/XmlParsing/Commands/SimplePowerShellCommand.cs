using DeploymentAssemblies;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AssetManager.Tools.Deployment.XmlParsing.Commands
{
    public class SimplePowerShellCommand : DeploymentCommand
    {
        private List<PowerShellCommand> _psCommands;

        public SimplePowerShellCommand(XElement cmdElement, IDeploymentUI deployment) : base(cmdElement, deployment)
        {
            _psCommands = GetPSCommands(cmdElement.Element("PSCommands"));
        }

        public async override Task<CommandCompletedResult> Execute()
        {
            _deploy.UsePowerShell();

            _successful = await _deploy.SimplePowerShellCommand(_psCommands.ToArray());

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

        private List<PowerShellCommand> GetPSCommands(XElement pscommandElem)
        {
            var commands = new List<PowerShellCommand>();

            foreach (var cmd in pscommandElem.Elements())
            {
                var isScriptAttrib = XmlHelper.GetAttribute(cmd, "IsScript");
                var useLocalScopeAttrib = XmlHelper.GetAttribute(cmd, "UseLocalScope");

                bool isScript = false;
                bool useLocalScope = false;

                if (!string.IsNullOrEmpty(isScriptAttrib))
                    isScript = Convert.ToBoolean(isScriptAttrib);

                if (!string.IsNullOrEmpty(useLocalScopeAttrib))
                    useLocalScope = Convert.ToBoolean(useLocalScopeAttrib);

                var psCmd = new PowerShellCommand(XmlHelper.GetElementValueOrValueAttrib(cmd), isScript, useLocalScope);

                var paramElements = cmd.Elements("PSParameter");

                if (paramElements != null)
                {
                    foreach (var parm in paramElements)
                    {
                        var name = XmlHelper.GetAttribute(parm, "Name");
                        var value = XmlHelper.GetAttribute(parm, "Value");

                        psCmd.Parameters.Add(name, value);
                    }
                }

                commands.Add(psCmd);
            }

            return commands;
        }
    }
}