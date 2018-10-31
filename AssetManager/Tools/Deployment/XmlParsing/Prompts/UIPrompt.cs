using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DeploymentAssemblies;

namespace AssetManager.Tools.Deployment.XmlParsing
{
    public abstract class UIPrompt
    {
        protected string _message;
        protected IDeploymentUI _deploy;

        public UIPrompt(IDeploymentUI ui, string message)
        {
            _deploy = ui;
            _message = message;
        }

        public abstract void Display();
         
    }
}
