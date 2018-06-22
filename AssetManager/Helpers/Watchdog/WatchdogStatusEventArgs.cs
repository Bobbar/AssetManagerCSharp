using System;

namespace AssetManager.Helpers.Watchdog
{
    public class WatchdogStatusEventArgs : EventArgs
    {
        public WatchdogConnectionStatus ConnectionStatus { get; set; }

        public WatchdogStatusEventArgs(WatchdogConnectionStatus connectionStatus)
        {
            this.ConnectionStatus = connectionStatus;
        }
    }
}