using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.Forms.Gatekeeper
{
    public enum UpdateStatus
    {
        Queued,
        Starting,
        Running,
        Canceled,
        Done,
        Error
    }
}
