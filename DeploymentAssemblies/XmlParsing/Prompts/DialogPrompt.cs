namespace DeploymentAssemblies.XmlParsing.Prompts
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