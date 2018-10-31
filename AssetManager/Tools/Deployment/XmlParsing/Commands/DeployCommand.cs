using DeploymentAssemblies;
using System;
using System.Threading.Tasks;

namespace AssetManager.Tools.Deployment.XmlParsing
{
    public abstract class DeployCommand
    {
        public readonly string CommandText;
        public readonly string Title;

        protected Type _returnType;
        protected IDeploymentUI _deploy;

        public DeployCommand(IDeploymentUI ui, string commandText, string title)
        {
            CommandText = commandText;
            Title = title;
            _deploy = ui;
        }

        public abstract Task<bool> Execute();
    }
}