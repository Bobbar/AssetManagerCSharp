namespace AssetManager.Data
{
    static class ServerInfo
    {
        public static bool ServerPinging = false;
        public const string MySQLServerIP = "10.10.0.89";

        private static DatabaseName _currentDataBase = DatabaseName.asset_manager;
        public static DatabaseName CurrentDataBase
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
