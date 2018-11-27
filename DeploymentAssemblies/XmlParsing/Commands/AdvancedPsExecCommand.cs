using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace DeploymentAssemblies.XmlParsing.Commands
{
    public class AdvancedPsExecCommand : DeploymentCommand
    {
        private string _commandText;

        public string CommandText
        {
            get
            {
                return _commandText;
            }
        }

        public AdvancedPsExecCommand(XElement cmdElement, IDeploymentUI deployment) : base(cmdElement, deployment)
        {
            _commandText = XmlHelper.GetCommandText(cmdElement);
        }

        public async override Task<CommandCompletedResult> Execute()
        {
            _deploy.UsePsExec();

            var exitCode = await _deploy.AdvancedPSExecCommand(_commandText, _title);

            var exitCodeElem = _commandElement.Element("ExitCodeResponse");

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
                    var isSuccessAttrib = XmlHelper.GetAttribute(codeElem, "IsSuccess");

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
                        var goodCodeVals = XmlHelper.GetAttribute(codeElem, "Value").Split(',');

                        // Check if the defined exit codes contain the exit code result.
                        if (goodCodeVals.Contains(exitCode.ToString()))
                        {
                            // Iterate and recurse with the commands.
                            foreach (var codeCmd in codeElem.Elements())
                            {
                                _commands.Add(codeCmd);
                            }

                            // Set successful true and return.
                            _successful = true;
                            return new CommandCompletedResult(_successful, _stopDeployment, _commands);
                        }
                    }
                    else
                    {
                        // Same as above, but we set the successful bool to false to ensure that the
                        // deployment is actually marked as failed once it completes.
                        var badCodeVals = XmlHelper.GetAttribute(codeElem, "Value").Split(',');

                        if (badCodeVals.Contains(exitCode.ToString()) || badCodeVals.Length == 0)
                        {
                            foreach (var codeCmd in codeElem.Elements())
                            {
                                _commands.Add(codeCmd);
                            }

                            // Set successfull false, set stopDeployment true and return to stop additional commands from executing.
                            _successful = false;
                            _stopDeployment = true;
                            return new CommandCompletedResult(_successful, _stopDeployment, _commands);
                        }
                    }
                }

                // If none of the specified exit code blocks have returned by this point,
                // then we know that the exit code was not expected/specified and we should
                // prompt the user and stop deployment.
                _deploy.LogMessage($@"Unexpected exit code: {exitCode}", MessageType.Error);
                _successful = false;
                _stopDeployment = true;
            }
            else
            {
                // If no exit code blocks are present, we assume
                // that the status of this command is not critical
                // and therefore it is safe to continue.
                _successful = true;
            }

            return new CommandCompletedResult(_successful, _stopDeployment, _commands);
        }
    }
}