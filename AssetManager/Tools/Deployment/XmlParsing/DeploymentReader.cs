using AssetManager.Tools.Deployment.XmlParsing.Commands;
using DeploymentAssemblies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace AssetManager.Tools.Deployment.XmlParsing
{
    public class DeploymentReader
    {
        private XElement _deploymentElem;

        public DeploymentReader(string settingsFileUri)
        {
            _deploymentElem = XElement.Load(settingsFileUri, LoadOptions.None);
        }

        public DeploymentReader(Stream settingsFileStream)
        {
            _deploymentElem = XElement.Load(settingsFileStream, LoadOptions.None);
        }

        public List<DeployCommand> ExecuteDeployment(IDeploymentUI deploy)
        {
            var depCmds = new List<DeployCommand>();
            var depName = GetAttribute(_deploymentElem, "Name");

            // Get the command elements within the deployment parent.
            var cmdElems = _deploymentElem.Elements("Command");

            // Iterate the command elements and parse out deployment objects.
            foreach (var cmd in cmdElems)
            {
                DeployCommand command = null;
                string cmdTitle = string.Empty;
                string cmdText = string.Empty;
                CommandType cmdType;

                // Get the command type attribute.
                var cmdTypeString = GetAttribute(cmd, "Type");

                // Parse the type and make sure it's recognized.
                var success = Enum.TryParse(cmdTypeString, true, out cmdType);

                if (!success)
                {
                    deploy.LogMessage($@"Deployment Parse Error: '{ cmdTypeString }'  is not a recognized command type!");
                    deploy.DoneOrError();
                    throw new Exception($@"Deployment Parse Error: '{ cmdTypeString }'  is not a recognized command type!");
                }

                // Parse and build the deployment objects depending on type.
                switch (cmdType)
                {
                    // UI prompt/message to user.
                    case CommandType.Prompt:

                        var promptcmd = new PromptCommand(deploy, GetPrompts(deploy, cmd));

                        command = promptcmd;
                        break;
                    
                    // Simple PsExec command; success if error code is 0, failure for anything else.
                    case CommandType.SimplePsExec:

                        cmdTitle = GetAttribute(cmd, "Title");
                        cmdText = GetCommandText(cmd);

                        var spsexec = new SimplePsExecCommand(deploy, cmdText, cmdTitle);

                        // Get the success and failure blocks.
                        var successElem = cmd.Element("OnSuccess");
                        spsexec.SuccessPrompts = GetPrompts(deploy, successElem);

                        var failElem = cmd.Element("OnFailure");
                        spsexec.FailPrompts = GetPrompts(deploy, failElem);

                        command = spsexec;
                        break;
                    
                    
                    case CommandType.SimplePowerShell:

                        cmdTitle = GetAttribute(cmd, "Title");
                        cmdText = GetCommandText(cmd);

                        var spshell = new SimplePowerShellScript(deploy, cmdText, cmdTitle);

                        var completeElem = cmd.Element("OnComplete");
                        spshell.OnCompletePrompts = GetPrompts(deploy, completeElem);

                        command = spshell;
                        break;
                    case CommandType.AdvancedPsExec:

                        cmdTitle = GetAttribute(cmd, "Title");
                        cmdText = GetCommandText(cmd);

                        var exitResonses = GetExitReponses(deploy, cmd);
                        var apshell = new AdvancedPsExecCommand(deploy, cmdText, cmdTitle, exitResonses);

                        command = apshell;
                        break;

                }

                if (command != null)
                    depCmds.Add(command);

            }

            return depCmds;
        }

        private List<ExitCodeResponse> GetExitReponses(IDeploymentUI deploy, XElement element)
        {
            var responses = new List<ExitCodeResponse>();

            // Enter the ExitCodeResponse element and return the ExitCode elements.
            var reponseElems = element.Descendants("ExitCodeResponse").Elements("ExitCode");

            foreach (var response in reponseElems)
            {
                if (response.Name.LocalName != "ExitCode")
                    throw new Exception($@"Unexpected exit code reponse node '{response.Name.LocalName}'");

                var codeVals = GetAttribute(response, "Value").Split(',');
                bool isSuccess = Convert.ToBoolean(GetAttribute(response, "IsSuccess"));

                foreach (var code in codeVals)
                {
                    int exitCode = int.Parse(code);
                    var goodExit = new ExitCodeResponse(exitCode, isSuccess);
                    goodExit.Prompts = GetPrompts(deploy, response);

                    responses.Add(goodExit);
                }
            }

            return responses;
        }


        private List<UIPrompt> GetPrompts(IDeploymentUI deploy, XElement onElement)
        {
            var prompts = new List<UIPrompt>();

            foreach (var prompt in onElement.Elements())
            {
                var promptType = (PromptType)Enum.Parse(typeof(PromptType), prompt.Name.LocalName, true);

                switch (promptType)
                {
                    case PromptType.LogMessage:
                        prompts.Add(new LogMessage(deploy, prompt.Value.Trim(), GetAttribute(prompt, "Type")));
                        break;

                    case PromptType.DialogPrompt:
                        prompts.Add(new DialogPrompt(deploy, prompt.Value.Trim(), GetAttribute(prompt, "Title")));
                        break;
                }
            }

            return prompts;
        }

        private string GetAttribute(XElement element, string name)
        {
            try
            {
                return element.Attributes().Where(a => a.Name.LocalName == name).First().Value;
            }
            catch
            {
                return string.Empty;
            }
        }

        private string GetCommandText(XElement cmd)
        {
            return cmd.Element("String").Value.Trim();
        }
    }
}