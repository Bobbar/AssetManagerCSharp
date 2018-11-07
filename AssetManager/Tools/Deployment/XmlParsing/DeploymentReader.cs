using AssetManager.Tools.Deployment.XmlParsing.Commands;
using DeploymentAssemblies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AssetManager.Tools.Deployment.XmlParsing
{
    public class DeploymentReader
    {
        public string DeploymentName
        {
            get
            {
                return _deploymentName;
            }
        }

        public int OrderPriority
        {
            get
            {
                return _orderPriority;
            }
        }

        private string _deploymentName;
        private int _orderPriority = 0;
        private XElement _deploymentElem;
        private IDeploymentUI _deploy;
        private bool _deploySuccessful = true;

        
        public DeploymentReader(string scriptFile)
        {
            _deploymentElem = XElement.Load(scriptFile, LoadOptions.None);

            ParseDeploymentInfo();
        }

        public DeploymentReader(Stream scriptFileStream)
        {
            _deploymentElem = XElement.Load(scriptFileStream, LoadOptions.None);

            ParseDeploymentInfo();
        }

        public DeploymentReader(Stream scriptFileStream, IDeploymentUI deployment)
        {
            _deploymentElem = XElement.Load(scriptFileStream, LoadOptions.None);
            _deploy = deployment;

            ParseDeploymentInfo();
        }

        public async Task<bool> StartDeployment()
        {
            return await RunDeployment(_deploymentElem.Elements());
        }

        private void ParseDeploymentInfo()
        {
            _deploymentName = XmlHelper.GetAttribute(_deploymentElem, "Name");
            var orderVal = XmlHelper.GetAttribute(_deploymentElem, "OrderPriority");

            if (!string.IsNullOrEmpty(orderVal))
            {
                _orderPriority = Convert.ToInt32(orderVal);
            }
        }

        private async Task<bool> RunDeployment(IEnumerable<XElement> cmdElements)
        {
            bool success = false;

            // Parse and execute all the command elements and return if any are unsuccessful.
            foreach (var cmd in cmdElements)
            {
                success = await ExecuteCommandElement(cmd);

                if (!success)
                    return success;
            }

            return success;
        }

        private async Task<bool> ExecuteCommandElement(XElement cmdElement)
        {
            CommandType cmdType;
            DeploymentCommand command = null;
            string cmdTypeString = XmlHelper.GetAttribute(cmdElement, "Type");

            // Parse the type and make sure it's an expected value.
            var valid = Enum.TryParse(cmdTypeString, true, out cmdType);

            // If the type is not expected, stop deployment and throw errors.
            if (!valid)
            {
                _deploy.LogMessage($@"Deployment Parse Error: '{ cmdTypeString }'  is not a recognized command type!");
                _deploy.DoneOrError();
                throw new Exception($@"Deployment Parse Error: '{ cmdTypeString }'  is not a recognized command type!");
            }

            // Parse and execute the deployment elements.
            switch (cmdType)
            {
                case CommandType.Prompt: // UI prompt/message to user.
                    command = new PromptCommand(cmdElement, _deploy);
                    break;

                case CommandType.Sleep: // Sleep for # seconds.
                    command = new SleepCommand(cmdElement, _deploy);
                    break;

                case CommandType.SimplePsExec:  // Simple PsExec command; success if error code is 0, failure for anything else.
                    command = new SimplePsExecCommand(cmdElement, _deploy);
                    break;

                case CommandType.SimplePowerShell: // Simple PowerShell command: Just excutes and returns when finished. No result supported.
                    command = new SimplePowerShellScriptCommand(cmdElement, _deploy);
                    break;

                case CommandType.SimplePowerShellCommand:
                    command = new SimplePowerShellCommand(cmdElement, _deploy);
                    break;

                case CommandType.AdvancedPsExec: // Advanced PsExec command: Returns the exit code upon completion.
                    command = new AdvancedPsExecCommand(cmdElement, _deploy);
                    break;
            }

            if (command != null)
            {
                // Execute the parsed command object and collect the result.
                var result = await command.Execute();

                // If it completed without error, check for additional commands and recurse with them.
                if (result.Successful)
                {
                    if (result.HasCommands)
                    {
                        foreach (var cmd in result.Commands)
                        {
                            // Recurse with additional commands.
                            await ExecuteCommandElement(cmd);
                        }
                    }
                }
                else
                {
                    // If errors occured, excute additional commands
                    // then stop deployment unless otherwise specified in the deployment script.
                    if (result.HasCommands)
                    {
                        foreach (var cmd in result.Commands)
                        {
                            await ExecuteCommandElement(cmd);
                        }
                    }

                    // If an OnFailure block has the Continue=True attribute,
                    // StopDeployment will be set to False and deployment will proceed to the next command.
                    // By default, StopDeployment is set to True when errors occur.
                    if (result.StopDeployment)
                        return false;
                }
            }

            return true;
        }
    }
}