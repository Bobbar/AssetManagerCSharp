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

        public DeployCommand(IDeploymentUI ui, string command, string title)
        {
            CommandText = command;
            Title = title;
            _deploy = ui;
        }

        public DeployCommand(IDeploymentUI ui, byte[] command, string title)
        {
            CommandText = System.Text.Encoding.Default.GetString(command);
            Title = title;
            _deploy = ui;
        }

        public abstract Task<bool> ExecuteReturnSuccess();

        public abstract Task<int> ExecuteReturnExitCode();

    }
}