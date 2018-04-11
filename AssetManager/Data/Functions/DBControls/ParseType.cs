using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Data.Functions
{
    /// <summary>
    /// Determines how the parser handles the updating and filling of a control.
    /// </summary>
    public enum ParseType
    {
        /// <summary>
        /// The control is filled only.
        /// </summary>
        DisplayOnly,

        /// <summary>
        /// The control is filled and will also be added to Update and Insert tables.
        /// </summary>
        UpdateAndDisplay
    }
}
