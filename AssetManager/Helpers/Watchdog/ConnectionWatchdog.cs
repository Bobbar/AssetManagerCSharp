using AssetManager.Data;
using AssetManager.Data.Functions;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace AssetManager.Helpers.Watchdog
{
    public class ConnectionWatchdog : IDisposable
    {
        private int failedPings = 0;
        private bool serverIsOnline;
        private bool cacheIsAvailable;
        private bool inCachedMode;
        private WatchdogConnectionStatus currentWatchdogStatus = WatchdogConnectionStatus.Online;
        private WatchdogConnectionStatus previousWatchdogStatus;
        private const int maxFailedPings = 2;
        private const int watcherInterval = 5000;
        private Task watcherTask;
        private Ping pinger = new Ping();

        public event EventHandler StatusChanged;

        public event EventHandler RebuildCache;

        public event EventHandler WatcherTick;

        public ConnectionWatchdog(bool cachedMode)
        {
            inCachedMode = cachedMode;
        }

        public ConnectionWatchdog()
        {
        }

        private void OnStatusChanged(WatchdogStatusEventArgs e)
        {
            StatusChanged?.Invoke(null, e);
        }

        private void OnRebuildCache(EventArgs e)
        {
            RebuildCache?.Invoke(null, e);
        }

        private void OnWatcherTick(EventArgs e)
        {
            WatcherTick?.Invoke(null, e);
        }

        public void StartWatcher()
        {
            watcherTask = new Task(() => Watcher(), TaskCreationOptions.LongRunning);
            watcherTask.Start();
            if (inCachedMode)
            {
                currentWatchdogStatus = WatchdogConnectionStatus.CachedMode;
                OnStatusChanged(new WatchdogStatusEventArgs(WatchdogConnectionStatus.CachedMode));
            }
        }

        public void StartWatcher(bool cachedMode)
        {
            inCachedMode = cachedMode;
            watcherTask = new Task(() => Watcher(), TaskCreationOptions.LongRunning);
            watcherTask.Start();
            if (inCachedMode)
            {
                currentWatchdogStatus = WatchdogConnectionStatus.CachedMode;
                OnStatusChanged(new WatchdogStatusEventArgs(WatchdogConnectionStatus.CachedMode));
            }
        }

        private async void Watcher()
        {
            while (!disposedValue)
            {
                // Can we talk to the server?
                serverIsOnline = ServerPinging();

                // Is the DB Cache available?
                cacheIsAvailable = DBCacheFunctions.CacheUpToDate(serverIsOnline);

                // Get status based on values of server online and cache mode.
                var status = GetWatchdogStatus();
                if (status != currentWatchdogStatus)
                {
                    currentWatchdogStatus = status;
                    OnStatusChanged(new WatchdogStatusEventArgs(currentWatchdogStatus));
                }

                if (serverIsOnline)
                {
                    //Fire tick event to update server datatime.
                    OnWatcherTick(new WatchdogTickEventArgs(GetServerTime()));
                }

                CheckForCacheRebuild();

                await Task.Delay(watcherInterval);
            }
        }

        private void CheckForCacheRebuild()
        {
            if (currentWatchdogStatus == WatchdogConnectionStatus.Online)
            {
                if (!cacheIsAvailable)
                {
                    OnRebuildCache(new EventArgs());
                }
                if (previousWatchdogStatus == WatchdogConnectionStatus.CachedMode || previousWatchdogStatus == WatchdogConnectionStatus.Offline)
                {
                    OnRebuildCache(new EventArgs());
                }
            }
            previousWatchdogStatus = currentWatchdogStatus;
        }

        private string GetServerTime()
        {
            try
            {
                return DBFactory.GetMySqlDatabase().ExecuteScalarFromQueryString("SELECT NOW()").ToString();
            }
            catch
            {
                return string.Empty;
            }
        }

        private bool ServerPinging()
        {
            for (int i = 0; i <= maxFailedPings; i++)
            {
                if (!CanTalkToServer())
                {
                    failedPings += 1;
                    if (failedPings >= maxFailedPings)
                    {
                        return false;
                    }
                }
                else
                {
                    failedPings = 0;
                    return true;
                }
                Task.Delay(1000).Wait();
            }
            return true;
        }

        private bool CanTalkToServer()
        {
            try
            {
                //using (Ping pinger = new Ping())
                //{
                bool canPing = false;
                var reply = pinger.Send(ServerInfo.MySQLServerIP);
                if (reply.Status == IPStatus.Success)
                {
                    canPing = true;
                }
                else
                {
                    canPing = false;
                }

                reply = null;

                //If server pinging, try to open a connection.
                if (canPing)
                {
                    using (var conn = DBFactory.GetMySqlDatabase().NewConnection())
                    {
                        return DBFactory.GetMySqlDatabase().OpenConnection(conn, true);
                    }
                }
                else
                {
                    //Not pinging. Return false.
                    return false;
                }
                //}
            }
            catch
            {
                //Catch ping or SQL exceptions, and return false.
                return false;
            }
        }

        private WatchdogConnectionStatus GetWatchdogStatus()
        {
            if (serverIsOnline & !inCachedMode)
            {
                return WatchdogConnectionStatus.Online;
            }
            else if (serverIsOnline & inCachedMode)
            {
                inCachedMode = false;
                return WatchdogConnectionStatus.Online;
            }
            else if (!serverIsOnline & cacheIsAvailable)
            {
                inCachedMode = true;
                return WatchdogConnectionStatus.CachedMode;
            }
            else
            {
                return WatchdogConnectionStatus.Offline;
            }
        }

        #region "IDisposable Support"

        private bool disposedValue;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    watcherTask.Dispose();
                    pinger?.Dispose();
                }
            }
            disposedValue = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion "IDisposable Support"
    }

    //public class WatchdogTickEventArgs : EventArgs
    //{
    //    public string ServerTime { get; }

    //    public WatchdogTickEventArgs(string serverTime)
    //    {
    //        this.ServerTime = serverTime;
    //    }
    //}

    //public class WatchdogStatusEventArgs : EventArgs
    //{
    //    public WatchdogConnectionStatus ConnectionStatus { get; set; }

    //    public WatchdogStatusEventArgs(WatchdogConnectionStatus connectionStatus)
    //    {
    //        this.ConnectionStatus = connectionStatus;
    //    }
    //}

    //public enum WatchdogConnectionStatus
    //{
    //    Online,
    //    Offline,
    //    CachedMode
    //}
}