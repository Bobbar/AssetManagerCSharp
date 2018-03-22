using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics.CodeAnalysis;

namespace AssetManager.UserInterface.CustomControls
{
    
    public interface IOnlineStatus
    {
        [SuppressMessage("Microsoft.Design", "CA1009")]
        event EventHandler<bool> OnlineStatusChanged;
    }
}
