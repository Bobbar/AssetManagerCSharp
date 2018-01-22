using AssetManager.Data.Communications;

namespace AssetManager.Data
{
    public static class DBFactory
    {

        public static IDataBase GetDatabase()
        {
            if (GlobalSwitches.CachedMode)
            {
                return new SQLiteDatabase(false);
            }
            else
            {
                return new MySQLDatabase();
            }
        }

    }
}
