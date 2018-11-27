using System;

namespace DeploymentAssemblies.XmlParsing.Prompts
{
    public sealed class LogMessage : UIPrompt
    {
        private MessageType _type;

        public LogMessage(IDeploymentUI ui, string message, MessageType type = MessageType.Default) : base(ui, message)
        {
            _type = type;
        }

        public LogMessage(IDeploymentUI ui, string message, string messageTypeString = "") : base(ui, message)
        {
            if (string.IsNullOrEmpty(messageTypeString))
            {
                _type = MessageType.Default;
            }
            else
            {
                bool success = Enum.TryParse(messageTypeString, out _type);

                if (!success)
                    _type = MessageType.Default;
            }
        }

        public override void Display()
        {
            _deploy.LogMessage(_message, _type);
        }
    }
}