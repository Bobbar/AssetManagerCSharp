using AssetDatabase.Data;
using AssetManager.Security;
using System.Collections.Generic;

namespace AssetManager.Data
{
    public static class DBFactory
    {
        private const string sqlitePass = "X9ow0zCwpGKyVeFR6K3yB4A7lQ2HgOgU";
        private const string mySqlCryptPass = "N9WzUK5qv2gOgB1odwfduM13ISneU/DG";
        private static string mySqlPass;
        private const string mySqlUser = "asset_mgr_usr";

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
            return new MySQLDatabase(ServerInfo.MySQLServerIP, mySqlUser, MySqlPassword, ServerInfo.CurrentDataBase.ToString());
        }

        public static IDataBase GetSqliteDatabase()
        {
            return new SqliteDatabase(Paths.SQLitePath, SecurityTools.DecodePassword(sqlitePass));
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