using AssetManager.Data.Communications;
using System;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using System.Data.Common;

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

        private int cycles = 0;
        private bool serverIsOnline;
        private bool cacheIsAvailable;

        private bool inCachedMode;
        private WatchDogConnectionStatus currentWatchdogStatus = WatchDogConnectionStatus.Online;

        private WatchDogConnectionStatus previousWatchdogStatus;
        const int maxFailedPings = 2;
        const int cyclesTillHashCheck = 60;
        const int watcherInterval = 5000;

        private Task watcherTask;// = new Task(() => Watcher());
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
            while (!(disposedValue))
            {
                serverIsOnline = await GetServerStatus();
                cacheIsAvailable = await DBCacheFunctions.VerifyLocalCacheHashOnly(inCachedMode);

                var Status = GetWatchdogStatus();
                if (Status != currentWatchdogStatus)
                {
                    currentWatchdogStatus = Status;
                    OnStatusChanged(new WatchDogStatusEventArgs(currentWatchdogStatus));
                }

                if (serverIsOnline)
                {
                    //Fire tick event to update server datatime.
                    OnWatcherTick(new WatchDogTickEventArgs(await GetServerTime()));
                }

                CheckForCacheRebuild();

                await Task.Delay(watcherInterval);
            }
        }

        private void CheckForCacheRebuild()
        {
            if (currentWatchdogStatus == WatchDogConnectionStatus.Online)
            {
                if (cacheIsAvailable)
                {
                    cycles += 1;
                    if (cycles >= cyclesTillHashCheck)
                    {
                        cycles = 0;
                        OnRebuildCache(new EventArgs());
                    }
                }
                else
                {
                    OnRebuildCache(new EventArgs());
                }
                if (previousWatchdogStatus == WatchDogConnectionStatus.CachedMode | previousWatchdogStatus == WatchDogConnectionStatus.Offline)
                {
                    OnRebuildCache(new EventArgs());
                }
            }
            previousWatchdogStatus = currentWatchdogStatus;
        }

        private async Task<string> GetServerTime()
        {
            try
            {
                return await Task.Run(() =>
                {
                    return DBFactory.GetMySqlDatabase().ExecuteScalarFromQueryString("SELECT NOW()").ToString();
                });
            }
            catch
            {
                return string.Empty;
            }
        }

        private async Task<bool> GetServerStatus()
        {
            return await Task.Run(() =>
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
                    Task.Delay(1000);
                }
                return true;
            });
        }

        private bool CanTalkToServer()
        {
            try
            {
                using (Ping ServerPing = new Ping())
                {
                    bool CanPing = false;
                    var Reply = ServerPing.Send(ServerInfo.MySQLServerIP);
                    if (Reply.Status == IPStatus.Success)
                    {
                        CanPing = true;
                    }
                    else
                    {
                        CanPing = false;
                    }

                    Reply = null;

                    //If server pinging, try to open a connection.
                    if (CanPing)
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
                    if (watcherTask != null) watcherTask.Dispose();
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

        #endregion

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
