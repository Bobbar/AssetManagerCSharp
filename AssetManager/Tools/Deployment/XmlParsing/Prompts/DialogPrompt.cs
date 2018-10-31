using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentAssemblies;

namespace AssetManager.Tools.Deployment.XmlParsing
{
    public sealed class DialogPrompt : UIPrompt
    {
        private string _title;

        public DialogPrompt(IDeploymentUI ui, string message, string title) : base(ui, message)
        {
            _title = title;
        }

        public override void Display()
        {
            _deploy.UserPrompt(_message, _title);
        }
    }
}
