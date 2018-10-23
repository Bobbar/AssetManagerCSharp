using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeploymentAssemblies
{
    /// <summary>
    /// Determines styling/color of the message UI output.
    /// </summary>
    public enum MessageType
    {
        /// <summary>
        /// Black text.
        /// </summary>
        Default,
        /// <summary>
        /// Green text.
        /// </summary>
        Success,
        /// <summary>
        /// Red text.
        /// </summary>
        Error,
        /// <summary>
        /// Yellow text.
        /// </summary>
        Warning,
        /// <summary>
        /// Blue text.
        /// </summary>
        Notice
    }
}
