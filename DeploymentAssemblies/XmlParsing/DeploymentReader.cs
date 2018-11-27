using DeploymentAssemblies.XmlParsing.Commands;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeploymentAssemblies.XmlParsing
{
    /// <summary>
    /// XML deployment reader for parsing and executing <see cref="DeploymentCommand"/> created from XML deployment markup files.
    /// </summary>
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

        /// <summary>
        /// Creates a new instance of a <see cref="DeploymentReader"/>. Initial XML format parsing will occur at construction.
        /// </summary>
        /// <param name="scriptFileStream">Stream containing XML deployment markup.</param>
        /// <param name="deployment">The instance of a <see cref="IDeploymentUI"/> to be used with this deployment.</param>
        public DeploymentReader(Stream scriptFileStream, IDeploymentUI deployment)
        {
            _deploymentElem = XElement.Load(scriptFileStream, LoadOptions.None);
            _deploy = deployment;

            ParseDeploymentInfo();
        }

        /// <summary>
        /// Begins parsing the XML deployment markup and starts executing the commands.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> StartDeployment()
        {
            return await RunDeployment(_deploymentElem.Elements());
        }

        /// <summary>
        /// Parse additional attributes from the XML file.
        /// </summary>
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

        /// <summary>
        /// Recursively parses and executes the commands within the XML file.
        /// </summary>
        /// <param name="cmdElement">The root element containing command blocks.</param>
        /// <returns>Returns true if no errors occured and all the commands executed without unexpected results.</returns>
        private async Task<bool> ExecuteCommandElement(XElement cmdElement)
        {
            var command = GetCommandFromElement(cmdElement);

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

        /// <summary>
        /// Parses the specified XML element and returns a <see cref="DeploymentCommand"/> instance.
        /// </summary>
        /// <param name="cmdElement">XML element containing a deployment command declaration.</param>
        private DeploymentCommand GetCommandFromElement(XElement cmdElement)
        {
            CommandType cmdType;
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

            // Switch on the command type and return the appropriate command instance.
            switch (cmdType)
            {
                case CommandType.Prompt: // UI prompt/message to user.
                    return new PromptCommand(cmdElement, _deploy);

                case CommandType.Sleep: // Sleep for # seconds.
                    return new SleepCommand(cmdElement, _deploy);

                case CommandType.SimplePsExec:  // Simple PsExec command; success if error code is 0, failure for anything else.
                    return new SimplePsExecCommand(cmdElement, _deploy);

                case CommandType.SimplePowerShell: // Simple PowerShell command: Just excutes and returns when finished. No result supported.
                    return new SimplePowerShellScriptCommand(cmdElement, _deploy);

                case CommandType.SimplePowerShellCommand:
                    return new SimplePowerShellCommand(cmdElement, _deploy);

                case CommandType.AdvancedPsExec: // Advanced PsExec command: Returns the exit code upon completion.
                    return new AdvancedPsExecCommand(cmdElement, _deploy);
            }

            return null;
        }
    }
}