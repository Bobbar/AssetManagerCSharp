namespace AssetManager.Data
{
    static class ServerInfo
    {
        public static bool ServerPinging = false;
        public const string MySQLServerIP = "10.10.0.89";

        private static NetworkInfo.Databases _currentDataBase = NetworkInfo.Databases.asset_manager;
        public static NetworkInfo.Databases CurrentDataBase
        {
            get { return _currentDataBase; }
            set
            {
                _currentDataBase = value;
                NetworkInfo.SetCurrentDomain(_currentDataBase);
            }
        }

    }
}
