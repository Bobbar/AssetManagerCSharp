using System;

namespace AssetManager.Helpers.Watchdog
{
    public class WatchdogTickEventArgs : EventArgs
    {
        public string ServerTime { get; }

        public WatchdogTickEventArgs(string serverTime)
        {
            this.ServerTime = serverTime;
        }
    }
}