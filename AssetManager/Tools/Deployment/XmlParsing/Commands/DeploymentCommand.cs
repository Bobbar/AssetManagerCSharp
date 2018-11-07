using DeploymentAssemblies;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AssetManager.Tools.Deployment.XmlParsing.Commands
{
    public abstract class DeploymentCommand
    {
        protected string _title;
        protected bool _successful = true;
        protected bool _stopDeployment = false;
        protected List<XElement> _commands = new List<XElement>();
        protected XElement _commandElement;
        protected IDeploymentUI _deploy;

        public DeploymentCommand(XElement cmdElement, IDeploymentUI deployment)
        {
            _commandElement = cmdElement;
            _title = XmlHelper.GetAttribute(_commandElement, "Title");
            _deploy = deployment;
        }

        /// <summary>
        /// Executes the implemented command logic and returns a <see cref="CommandCompletedResult"/>
        /// </summary>
        public abstract Task<CommandCompletedResult> Execute();
    }
}