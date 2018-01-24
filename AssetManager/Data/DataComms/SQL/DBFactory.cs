using AssetDatabase.Data;
using AssetManager.Security;

namespace AssetManager.Data
{
    public static class DBFactory
    {
        private const string EncSQLitePass = "X9ow0zCwpGKyVeFR6K3yB4A7lQ2HgOgU";
        private const string EncMySqlPass = "N9WzUK5qv2gOgB1odwfduM13ISneU/DG";

        public static IDataBase GetDatabase()
        {
            if (GlobalSwitches.CachedMode)
            {
                return GetSqliteDatabase();
            }
            else
            {
                return GetMySqlDatabase();
            }
        }

        public static IDataBase GetMySqlDatabase()
        {
            return new MySQLDatabase(ServerInfo.MySQLServerIP, "asset_mgr_usr", SecurityTools.DecodePassword(EncMySqlPass), ServerInfo.CurrentDataBase.ToString());
        }

        public static IDataBase GetSqliteDatabase()
        {
            return new SqliteDatabase(Paths.SQLitePath, SecurityTools.DecodePassword(EncSQLitePass));
        }
    }
}