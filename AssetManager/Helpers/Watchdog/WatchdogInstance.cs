using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AssetManager.Helpers.Watchdog
{
    public static class WatchdogInstance
    {
        public static ConnectionWatchdog Watchdog = new ConnectionWatchdog();

        //public static void StartWatchdog(bool cachedMode)
        //{
        //    Watchdog.StartWatcher(cachedMode);

        //}
    }
}
