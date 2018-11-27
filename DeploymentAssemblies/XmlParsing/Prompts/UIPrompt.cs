namespace DeploymentAssemblies.XmlParsing.Prompts
{
    /// <summary>
    /// Base class for UI prompts.
    /// </summary>
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