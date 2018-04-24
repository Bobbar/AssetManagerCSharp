using AssetDatabase.Data;
using AssetManager.Security;
using System.Collections.Generic;
using AssetManager.Helpers;

namespace AssetManager.Data
{
    public static class DBFactory
    {
        private const string sqlitePass = "X9ow0zCwpGKyVeFR6K3yB4A7lQ2HgOgU";
        private const string mySqlCryptPass = "N9WzUK5qv2gOgB1odwfduM13ISneU/DG";
        private static string mySqlPass;
        private const string mySqlUser = "asset_mgr_usr";
        private static MySQLDatabase mySqlDb;
        private static SqliteDatabase sqliteDb;
        private static string previousDatabase;

        private static string MySqlPassword
        {
            get
            {
                if (string.IsNullOrEmpty(mySqlPass))
                {
                    mySqlPass = SecurityTools.DecodePassword(mySqlCryptPass);
                }

                return mySqlPass;
            }
        }

        public static IDatabase GetDatabase()
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

        public static IDatabase GetMySqlDatabase()
        {
            if (mySqlDb == null | DatabaseChanged(ServerInfo.CurrentDataBase.ToString()))
            {
                mySqlDb = new MySQLDatabase(ServerInfo.MySQLServerIP, mySqlUser, MySqlPassword, ServerInfo.CurrentDataBase.ToString());
            }

            return mySqlDb;
        }

        public static IDatabase GetSqliteDatabase()
        {
            if (sqliteDb == null)
            {
                sqliteDb = new SqliteDatabase(Paths.SQLitePath, SecurityTools.DecodePassword(sqlitePass));
            }

            return sqliteDb;
        }

        private static bool DatabaseChanged(string database)
        {
            if (previousDatabase != database)
            {
                previousDatabase = database;
                return true;
            }
            return false;
        }
    }

    /// <summary>
    /// Wrapper for DBParameter
    /// </summary>
    internal class ParamCollection
    {
        public List<DBParameter> Parameters;

        public ParamCollection()
        {
            Parameters = new List<DBParameter>();
        }

        public void Add(string fieldName, object fieldValue)
        {
            Parameters.Add(new DBParameter(fieldName, fieldValue));
        }
    }

    /// <summary>
    /// Wrapper for DBQueryParameter
    /// </summary>
    internal class QueryParamCollection
    {
        public List<DBQueryParameter> Parameters;

        public QueryParamCollection()
        {
            Parameters = new List<DBQueryParameter>();
        }

        public void Add(DBQueryParameter parameter)
        {
            Parameters.Add(new DBQueryParameter(parameter.FieldName, parameter.Value, parameter.IsExact, parameter.OperatorString));
        }

        public void Add(string fieldName, object fieldValue, string operatorString)
        {
            Parameters.Add(new DBQueryParameter(fieldName, fieldValue, operatorString));
        }

        public void Add(string fieldName, object fieldValue, bool isExact)
        {
            Parameters.Add(new DBQueryParameter(fieldName, fieldValue, isExact));
        }

        public void Add(string fieldName, object fieldValue, bool isExact, string operatorString)
        {
            Parameters.Add(new DBQueryParameter(fieldName, fieldValue, isExact, operatorString));
        }
    }
}