using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.CustomControls
{
    public interface IOnlineStatus
    {
        event EventHandler<bool> OnlineStatusChanged;
    }
}
