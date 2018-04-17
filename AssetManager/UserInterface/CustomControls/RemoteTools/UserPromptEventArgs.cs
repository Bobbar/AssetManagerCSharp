using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace AssetManager.UserInterface.CustomControls
{
    /// <summary>
    /// Custom event args which contains message info to be displayed to the user.
    /// </summary>
    public class UserPromptEventArgs : EventArgs
    {
        /// <summary>
        /// The text to be displayed in the user prompt.
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// Color of the text.
        /// </summary>
        public Color Color { get; set; }

        /// <summary>
        /// How long to display the message in seconds.
        /// </summary>
        public int DisplayTime { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPromptEventArgs"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="displayTime"></param>
        public UserPromptEventArgs(string message, int displayTime) : this(message, Color.Black, displayTime)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="UserPromptEventArgs"/> class.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="displayTime"></param>
        public UserPromptEventArgs(string message, Color color, int displayTime)
        {
            Text = message;
            Color = color;
            DisplayTime = displayTime;
        }

    }
}
