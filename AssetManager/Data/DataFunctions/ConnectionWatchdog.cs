using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;

namespace AssetManager.Data.Functions
{
    public class ConnectionWatchdog : IDisposable
    {
        public ConnectionWatchdog(bool cachedMode)
        {
            inCachedMode = cachedMode;
        }

        public event EventHandler StatusChanged;

        public event EventHandler RebuildCache;

        public event EventHandler WatcherTick;

        protected virtual void OnStatusChanged(WatchDogStatusEventArgs e)
        {
            if (StatusChanged != null)
            {
                StatusChanged(this, e);
            }
        }

        protected virtual void OnRebuildCache(EventArgs e)
        {
            if (RebuildCache != null)
            {
                RebuildCache(this, e);
            }
        }

        protected virtual void OnWatcherTick(EventArgs e)
        {
            if (WatcherTick != null)
            {
                WatcherTick(this, e);
            }
        }

        private int failedPings = 0;

        private bool serverIsOnline;
        private bool cacheIsAvailable;

        private bool inCachedMode;
        private WatchDogConnectionStatus currentWatchdogStatus = WatchDogConnectionStatus.Online;

        private WatchDogConnectionStatus previousWatchdogStatus;
        private const int maxFailedPings = 2;
        private const int watcherInterval = 5000;

        private Task watcherTask;

        public void StartWatcher()
        {
            watcherTask = new Task(() => Watcher());
            watcherTask.Start();
            if (inCachedMode)
            {
                currentWatchdogStatus = WatchDogConnectionStatus.CachedMode;
                OnStatusChanged(new WatchDogStatusEventArgs(WatchDogConnectionStatus.CachedMode));
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
                    OnStatusChanged(new WatchDogStatusEventArgs(currentWatchdogStatus));
                }

                if (serverIsOnline)
                {
                    //Fire tick event to update server datatime.
                    OnWatcherTick(new WatchDogTickEventArgs(GetServerTime()));
                }

                CheckForCacheRebuild();

                await Task.Delay(watcherInterval);
            }
        }

        private void CheckForCacheRebuild()
        {
            if (currentWatchdogStatus == WatchDogConnectionStatus.Online)
            {
                if (!cacheIsAvailable)
                {
                    OnRebuildCache(new EventArgs());
                }
                if (previousWatchdogStatus == WatchDogConnectionStatus.CachedMode || previousWatchdogStatus == WatchDogConnectionStatus.Offline)
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
                using (Ping pinger = new Ping())
                {
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
                }
            }
            catch
            {
                //Catch ping or SQL exceptions, and return false.
                return false;
            }
        }

        private WatchDogConnectionStatus GetWatchdogStatus()
        {
            if (serverIsOnline & !inCachedMode)
            {
                return WatchDogConnectionStatus.Online;
            }
            else if (serverIsOnline & inCachedMode)
            {
                inCachedMode = false;
                return WatchDogConnectionStatus.Online;
            }
            else if (!serverIsOnline & cacheIsAvailable)
            {
                inCachedMode = true;
                return WatchDogConnectionStatus.CachedMode;
            }
            else
            {
                return WatchDogConnectionStatus.Offline;
            }
        }

        #region "IDisposable Support"

        // To detect redundant calls
        private bool disposedValue;

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        //Protected Overrides Sub Finalize()
        //    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            // TODO: uncomment the following line if Finalize() is overridden above.
            // GC.SuppressFinalize(Me)
        }

        #endregion "IDisposable Support"
    }

    public class WatchDogTickEventArgs : EventArgs
    {
        public string ServerTime { get; }

        public WatchDogTickEventArgs(string serverTime)
        {
            this.ServerTime = serverTime;
        }
    }

    public class WatchDogStatusEventArgs : EventArgs
    {
        public WatchDogConnectionStatus ConnectionStatus { get; set; }

        public WatchDogStatusEventArgs(WatchDogConnectionStatus connectionStatus)
        {
            this.ConnectionStatus = connectionStatus;
        }
    }

    public enum WatchDogConnectionStatus
    {
        Online,
        Offline,
        CachedMode
    }
}