using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using DeploymentAssemblies;

namespace AssetManager.Tools.Deployment.XmlParsing.Commands
{
    public class PromptCommand : DeploymentCommand
    {
        private List<UIPrompt> _prompts;

        public PromptCommand(XElement cmdElement, IDeploymentUI deployment) : base(cmdElement, deployment)
        {
            _prompts = GetPrompts(cmdElement);
        }

        public async override Task<CommandCompletedResult> Execute()
        {
            _prompts.ForEach(p => p.Display());

            return new CommandCompletedResult();
        }


        /// <summary>
        /// Parse out UI prompt declarations from within the specified parent element.
        /// </summary>
        /// <param name="promptElement">Element containing UI prompt declarations.</param>
        /// <returns></returns>
        private List<UIPrompt> GetPrompts(XElement promptElement)
        {
            var prompts = new List<UIPrompt>();

            foreach (var prompt in promptElement.Elements())
            {
                var promptType = (PromptType)Enum.Parse(typeof(PromptType), prompt.Name.LocalName, true);

                switch (promptType)
                {
                    case PromptType.LogMessage:
                        prompts.Add(new LogMessage(_deploy, XmlHelper.GetElementValueOrValueAttrib(prompt), XmlHelper.GetAttribute(prompt, "Type")));
                        break;

                    case PromptType.DialogPrompt:
                        prompts.Add(new DialogPrompt(_deploy, XmlHelper.GetElementValueOrValueAttrib(prompt), XmlHelper.GetAttribute(prompt, "Title")));
                        break;
                }
            }

            return prompts;
        }
    }
}
