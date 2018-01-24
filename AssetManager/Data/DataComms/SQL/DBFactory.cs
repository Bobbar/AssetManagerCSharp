using AssetDatabase.Data;
using AssetManager.Security;
using System.Collections.Generic;

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