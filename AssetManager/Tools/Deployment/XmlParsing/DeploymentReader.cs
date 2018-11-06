using DeploymentAssemblies;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

        /// <summary>
        /// A one way bool. If this is set to false, it will stay false for the remainder of the deployment.
        /// </summary>
        private bool DeploySuccessful
        {
            get
            {
                return _deploySuccessful;
            }

            set
            {
                // Only allow to be set to false.
                if (value == false)
                {
                    _deploySuccessful = value;
                }
            }
        }

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

        private void ParseDeploymentInfo()
        {
            _deploymentName = GetAttribute(_deploymentElem, "Name");
            var orderVal = GetAttribute(_deploymentElem, "OrderPriority");

            if (!string.IsNullOrEmpty(orderVal))
            {
                _orderPriority = Convert.ToInt32(orderVal);
            }
        }

        public async Task<bool> StartDeployment()
        {
            return await RunDeployment(_deploymentElem.Elements());
        }

        private async Task<bool> RunDeployment(IEnumerable<XElement> cmdElements)
        {
            bool success = false;

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

            // Get the command attributes.
            string cmdTitle = GetAttribute(cmdElement, "Title");
            string cmdText = GetCommandText(cmdElement);
            string cmdTypeString = GetAttribute(cmdElement, "Type");

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

                    DeploySuccessful = ExecutePromptCommand(cmdElement);

                    break;

                case CommandType.Sleep: // Sleep for # seconds.

                    DeploySuccessful = await ExecuteSleepCommand(cmdElement);

                    break;

                case CommandType.SimplePsExec:  // Simple PsExec command; success if error code is 0, failure for anything else.

                    DeploySuccessful = await ExecuteSimplePsExec(cmdElement);

                    break;

                case CommandType.SimplePowerShell: // Simple PowerShell command: Just excutes and returns when finished. No result supported.

                    DeploySuccessful = await ExecuteSimplePowerShell(cmdElement);

                    break;

                case CommandType.SimplePowerShellCommand:

                    DeploySuccessful = await ExecuteSimplePowerShellCommand(cmdElement);

                    break;

                case CommandType.AdvancedPsExec: // Advanced PsExec command: Returns the exit code upon completion.

                    DeploySuccessful = await ExecuteAdvancedPsExec(cmdElement);

                    break;
            }

            return DeploySuccessful;
        }

        private bool ExecutePromptCommand(XElement cmdElement)
        {
            var prompts = GetPrompts(cmdElement);

            prompts.ForEach(p => p.Display());

            return true;
        }

        private async Task<bool> ExecuteSleepCommand(XElement cmdElement)
        {
            var sleepTimeVal = GetElementValueOrValueAttrib(cmdElement);

            int sleepTime = Convert.ToInt32(sleepTimeVal);

            for (var i = sleepTime; i >= 1; i--)
            {
                await Task.Delay(1000);
                _deploy.LogMessage(i + "...", MessageType.Notice);
            }

            return true;
        }

        private async Task<bool> ExecuteSimplePsExec(XElement cmdElement)
        {
            var title = GetAttribute(cmdElement, "Title");
            var command = GetCommandText(cmdElement);

            _deploy.UsePsExec();

            var success = await _deploy.SimplePSExecCommand(command, title);

            // Look for OnSuccess and/or OnFailure declarations and recurse with their commands if present.
            if (success)
            {
                var successElem = cmdElement.Element("OnSuccess");

                if (successElem != null)
                {
                    foreach (var cmd in successElem.Elements())
                    {
                        if (!await ExecuteCommandElement(cmd))
                            success = false;
                    }
                }
            }
            else
            {
                success = false;

                var failElem = cmdElement.Element("OnFailure");

                if (failElem != null)
                {
                    // A True 'Continue' attribute means that failures here are OK and we should coninute with the next commands.
                    var continueVal = GetAttribute(failElem, "Continue");
                    bool canContinue = Convert.ToBoolean(continueVal);

                    // Set success back to true since failures are allowed.
                    if (canContinue)
                        success = true;

                    foreach (var cmd in failElem.Elements())
                    {
                        if (!await ExecuteCommandElement(cmd))
                            success = false;
                    }
                }
            }

            return success;
        }

        private async Task<bool> ExecuteSimplePowerShell(XElement cmdElement)
        {
            var title = GetAttribute(cmdElement, "Title");
            var command = GetCommandText(cmdElement);
            byte[] commandBytes = Encoding.ASCII.GetBytes(command);

            _deploy.UsePowerShell();

            await _deploy.SimplePowerShellScript(commandBytes, title);

            // Look for OnComplete declaration and recurse on commands if present.
            var completeElem = cmdElement.Element("OnComplete");

            bool success = true;

            if (completeElem != null)
            {
                foreach (var cmd in completeElem.Elements())
                {
                    if (!await ExecuteCommandElement(cmd))
                        success = false;
                }
            }

            return success;
        }

        private async Task<bool> ExecuteSimplePowerShellCommand(XElement cmdElement)
        {
            var title = GetAttribute(cmdElement, "Title");
            var commands = GetPSCommands(cmdElement.Element("PSCommands"));

            _deploy.UsePowerShell();

            var success = await _deploy.SimplePowerShellCommand(commands.ToArray());

            if (success)
            {
                var successElem = cmdElement.Element("OnSuccess");

                if (successElem != null)
                {
                    foreach (var cmd in successElem.Elements())
                    {
                        if (!await ExecuteCommandElement(cmd))
                            success = false;
                    }
                }
            }
            else
            {
                success = false;

                var failElem = cmdElement.Element("OnFailure");

                if (failElem != null)
                {
                    // A True 'Continue' attribute means that failures here are OK and we should coninute with the next commands.
                    var continueVal = GetAttribute(failElem, "Continue");
                    bool canContinue = Convert.ToBoolean(continueVal);

                    // Set success back to true since failures are allowed.
                    if (canContinue)
                        success = true;

                    foreach (var cmd in failElem.Elements())
                    {
                        if (!await ExecuteCommandElement(cmd))
                            success = false;
                    }
                }
            }

            return success;
        }

        private async Task<bool> ExecuteAdvancedPsExec(XElement cmdElement)
        {
            var title = GetAttribute(cmdElement, "Title");
            var command = GetCommandText(cmdElement);

            _deploy.UsePsExec();

            var exitCode = await _deploy.AdvancedPSExecCommand(command, title);

            var exitCodeElem = cmdElement.Element("ExitCodeResponse");

            bool success = true;

            if (exitCodeElem != null)
            {
                // Iterate the exit code elements then parse and test them against the exit code result.
                foreach (var codeElem in exitCodeElem.Elements("ExitCode"))
                {
                    // Is this exit code block defining successful codes?
                    // We check for the IsSuccess attribute and parse accordingly.
                    // If no attribute is present, we default to IsSuccess = True.
                    bool isSuccessBlock = true;

                    // Get the attribute.
                    var isSuccessAttrib = GetAttribute(codeElem, "IsSuccess");

                    // If the attribute is present, convert the string to bool.
                    if (!string.IsNullOrEmpty(isSuccessAttrib))
                    {
                        isSuccessBlock = Convert.ToBoolean(isSuccessAttrib);
                    }

                    // Get the defined exit codes and test for each one.
                    if (isSuccessBlock)
                    {
                        // Split the code values to an array.
                        //
                        // TODO: Maybe remove the split and check the entire string.
                        // This might allow the use of any delimiter. (Spaces, commas, bars, etc)
                        var goodCodeVals = GetAttribute(codeElem, "Value").Split(',');

                        // Check if the defined exit codes contain the exit code result.
                        if (goodCodeVals.Contains(exitCode.ToString()))
                        {
                            // Iterate and recurse with the commands.
                            foreach (var codeCmd in codeElem.Elements())
                            {
                                if (!await ExecuteCommandElement(codeCmd))
                                    success = false;
                            }

                            return success;
                        }
                    }
                    else
                    {
                        // Same as above, but we set the successful bool to false to ensure that the
                        // deployment is actually marked as failed once it completes.
                        var badCodeVals = GetAttribute(codeElem, "Value").Split(',');

                        if (badCodeVals.Contains(exitCode.ToString()) || badCodeVals.Length == 0)
                        {
                            foreach (var codeCmd in codeElem.Elements())
                            {
                                if (!await ExecuteCommandElement(codeCmd))
                                    success = false;
                            }

                            return success;
                        }
                    }
                }

                _deploy.LogMessage($@"Unexpected exit code: {exitCode}", MessageType.Error);
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Parse out UI prompt declarations from within the specified parent element.
        /// </summary>
        /// <param name="parentElement">Element containing UI prompt declarations.</param>
        /// <returns></returns>
        private List<UIPrompt> GetPrompts(XElement parentElement)
        {
            var prompts = new List<UIPrompt>();

            foreach (var prompt in parentElement.Elements())
            {
                var promptType = (PromptType)Enum.Parse(typeof(PromptType), prompt.Name.LocalName, true);

                switch (promptType)
                {
                    case PromptType.LogMessage:
                        prompts.Add(new LogMessage(_deploy, GetElementValueOrValueAttrib(prompt), GetAttribute(prompt, "Type")));
                        break;

                    case PromptType.DialogPrompt:
                        prompts.Add(new DialogPrompt(_deploy, GetElementValueOrValueAttrib(prompt), GetAttribute(prompt, "Title")));
                        break;
                }
            }

            return prompts;
        }

        /// <summary>
        /// Returns the elements internal value or the elements "Value" attribute depending on which one is present.
        /// </summary>
        /// <param name="promptElement"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException">
        /// If neither or both internal value and "Value" attributes are present. 
        /// </exception>
        private string GetElementValueOrValueAttrib(XElement promptElement)
        {
            var elementValue = promptElement.Value;
            var attributeValue = GetAttribute(promptElement, "Value");

            if (!string.IsNullOrEmpty(elementValue) && !string.IsNullOrEmpty(attributeValue))
                throw new InvalidOperationException("Both internal value and 'Value' attributes are present. Make sure the element is populated with only one.");

            if (string.IsNullOrEmpty(elementValue) && string.IsNullOrEmpty(attributeValue))
                throw new InvalidOperationException("Neither internal value or 'Value' attributes are present. Make sure the element is populated with atleast one.");


            if (!string.IsNullOrEmpty(elementValue))
            {
                return elementValue.Trim();
            }
            else
            {
                return attributeValue.Trim();
            }
        }

        private List<PowerShellCommand> GetPSCommands(XElement pscommandElem)
        {
            var commands = new List<PowerShellCommand>();

            foreach (var cmd in pscommandElem.Elements())
            {
                var isScriptAttrib = GetAttribute(cmd, "IsScript");
                var useLocalScopeAttrib = GetAttribute(cmd, "UseLocalScope");

                bool isScript = false;
                bool useLocalScope = false;

                if (!string.IsNullOrEmpty(isScriptAttrib))
                    isScript = Convert.ToBoolean(isScriptAttrib);

                if (!string.IsNullOrEmpty(useLocalScopeAttrib))
                    useLocalScope = Convert.ToBoolean(useLocalScopeAttrib);

                var psCmd = new PowerShellCommand(GetElementValueOrValueAttrib(cmd), isScript, useLocalScope);

                var paramElements = cmd.Elements("PSParameter");

                if (paramElements != null)
                {
                    foreach (var parm in paramElements)
                    {
                        var name = GetAttribute(parm, "Name");
                        var value = GetAttribute(parm, "Value");

                        psCmd.Parameters.Add(name, value);
                    }
                }

                commands.Add(psCmd);
            }

            return commands;
        }

        /// <summary>
        /// Gets the attribute value with the specifed name from the specified element. Not case sensitive.
        /// </summary>
        /// <param name="element">Target element.</param>
        /// <param name="name">The name of the attribute to return the value from.</param>
        /// <returns></returns>
        private string GetAttribute(XElement element, string name)
        {
            try
            {
                return element.Attributes().Where(a => a.Name.LocalName.ToUpper() == name.ToUpper()).First().Value;
            }
            catch
            {
                return null;
            }
        }

        private string GetCommandText(XElement cmd)
        {
            var stringElement = cmd.Element("String");

            if (stringElement != null)
            {
                return stringElement.Value.Trim();
            }

            return string.Empty;
        }
    }
}