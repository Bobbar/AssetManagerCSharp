using System.Collections.Generic;
using System.Xml.Linq;

namespace DeploymentAssemblies.XmlParsing.Commands
{
    public sealed class CommandCompletedResult
    {
        public readonly bool Successful;
        public readonly bool StopDeployment;
        public List<XElement> Commands = new List<XElement>();

        public bool HasCommands
        {
            get
            {
                if (Commands.Count > 0)
                    return true;

                return false;
            }
        }

        public CommandCompletedResult(bool wasSuccessful, bool stopDeployment, List<XElement> onCompleteCommands)
        {
            Successful = wasSuccessful;
            StopDeployment = stopDeployment;
            Commands = onCompleteCommands;
        }

        public CommandCompletedResult(bool wasSuccessful, bool stopDeployment)
        {
            Successful = wasSuccessful;
            StopDeployment = stopDeployment;
        }

        public CommandCompletedResult()
        {
            Successful = true;
            StopDeployment = false;
        }
    }
}