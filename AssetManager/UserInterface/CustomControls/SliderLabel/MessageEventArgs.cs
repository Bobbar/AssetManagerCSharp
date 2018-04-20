using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.CustomControls
{

    /// <summary>
    /// EventArg wrapper for message events.
    /// </summary>
    public class MessageEventArgs : EventArgs
    {
        public MessageParameters Message
        {
            get
            {
                return message;
            }
        }

        private MessageParameters message;

        public MessageEventArgs(MessageParameters message)
        {
            this.message = message;
        }
    }
}
