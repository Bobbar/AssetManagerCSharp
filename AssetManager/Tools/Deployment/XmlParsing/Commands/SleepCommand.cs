using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DeploymentAssemblies;

namespace AssetManager.Tools.Deployment.XmlParsing.Commands
{
    public class SleepCommand : DeploymentCommand
    {
        private int _sleepTime;

        public SleepCommand(XElement cmdElement, IDeploymentUI deployment) : base(cmdElement, deployment)
        {
            var sleepTimeVal = XmlHelper.GetElementValueOrValueAttrib(cmdElement);
            _sleepTime = Convert.ToInt32(sleepTimeVal);
        }

        public async override Task<CommandCompletedResult> Execute()
        {
            for (var i = _sleepTime; i >= 1; i--)
            {
                await Task.Delay(1000);
                _deploy.LogMessage(i + "...", MessageType.Notice);
            }

            return new CommandCompletedResult();
        }
    }
}
