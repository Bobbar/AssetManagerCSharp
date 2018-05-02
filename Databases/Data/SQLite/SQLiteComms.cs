using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;

namespace AssetDatabase.Data
{
    public class SqliteDatabase : IDatabase
    {
        #region Fields

        public string SqlitePath { get; set; } = string.Empty;
        public string SqlitePass { get; set; } = string.Empty;

        private string sqliteConnectString
        {
            get
            {
                return "Data Source=" + SqlitePath + ";Password=" + SqlitePass;
            }
        }

        #endregion Fields

        #region Constructors

        public SqliteDatabase(string dbpath, string dbpass = "")
        {
            SqlitePath = dbpath;
            SqlitePass = dbpass;
        }

        #endregion Constructors

        #region Methods

        #region Connection Methods

        public void CloseConnection(DbConnection connection)
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
            }
        }

        public DbConnection NewConnection()
        {
            return new SQLiteConnection(sqliteConnectString);
        }

        public bool OpenConnection(DbConnection connection)
        {
            var sqliteConnection = (SQLiteConnection)connection;

            if (ReferenceEquals(sqliteConnection, null))
            {
                sqliteConnection = (SQLiteConnection)NewConnection();
            }
            if (sqliteConnection.State != ConnectionState.Open)
            {
                if (!string.IsNullOrEmpty(SqlitePass))
                {
                    sqliteConnection.SetPassword(SqlitePass);
                }

                sqliteConnection.Open();
            }
            if (sqliteConnection.State == ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool OpenConnection(DbConnection connection, bool overrideNoPing)
        {
            throw new NotImplementedException();
        }

        #endregion Connection Methods

        #region IDataBase

        public DbTransaction StartTransaction()
        {
            var conn = (SQLiteConnection)NewConnection();
            OpenConnection(conn);
            return conn.BeginTransaction();
        }

        public DataTable DataTableFromQueryString(string query)
        {
            using (DataTable results = new DataTable())
            using (var conn = (SQLiteConnection)NewConnection())
            using (var cmd = new SQLiteCommand(query, conn))
            using (DbDataAdapter da = new SQLiteDataAdapter())
            {
                cmd.Connection = conn;
                da.SelectCommand = cmd;
                da.Fill(results);
                da.SelectCommand.Connection.Dispose();
                return results;
            }
        }

        public DataTable DataTableFromCommand(DbCommand command, DbTransaction transaction = null)
        {
            if (transaction != null)
            {
                throw (new NotImplementedException());
            }
            using (DbDataAdapter da = new SQLiteDataAdapter())
            using (DataTable results = new DataTable())
            using (var conn = NewConnection())

            {
                command.Connection = conn;
                da.SelectCommand = command;
                da.Fill(results);
                command.Dispose();
                return results;
            }
        }

        public DataTable DataTableFromParameters(string query, List<DBQueryParameter> @params)
        {
            using (var cmd = GetCommandFromParams(query, @params))
            using (var results = DataTableFromCommand(cmd))
            {
                return results;
            }
        }

        public dynamic ExecuteScalarFromCommand(DbCommand command)
        {
            try
            {
                using (var conn = NewConnection())
                {
                    command.Connection = conn;
                    OpenConnection(command.Connection);
                    return command.ExecuteScalar();
                }
            }
            finally
            {
                command.Dispose();
            }
        }

        public dynamic ExecuteScalarFromQueryString(string query)
        {
            using (var conn = (SQLiteConnection)NewConnection())
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                OpenConnection(conn);
                var value = cmd.ExecuteScalar();
                conn.Close();
                cmd.Connection.Close();
                return value;
            }
        }

        public int ExecuteNonQuery(string query, DbTransaction transaction = null)
        {
            if (transaction == null)
            {
                using (var conn = (SQLiteConnection)NewConnection())
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    OpenConnection(conn);
                    return cmd.ExecuteNonQuery();
                }
            }
            else
            {
                var conn = (SQLiteConnection)transaction.Connection;
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    return cmd.ExecuteNonQuery();
                }
            }
        }

        public int InsertFromParameters(string tableName, List<DBParameter> @params, DbTransaction transaction = null)
        {
            throw (new NotImplementedException());
        }

        public int UpdateTable(string selectQuery, DataTable table, DbTransaction transaction = null)
        {
            if (transaction != null)
            {
                var conn = (SQLiteConnection)transaction.Connection;
                using (var cmd = new SQLiteCommand(selectQuery, conn, (SQLiteTransaction)transaction))
                using (var Adapter = new SQLiteDataAdapter(cmd))
                using (var Builder = new SQLiteCommandBuilder(Adapter))
                {
                    return Adapter.Update(table);
                }
            }
            else
            {
                using (var conn = (SQLiteConnection)NewConnection())
                using (var Adapter = new SQLiteDataAdapter(selectQuery, conn))
                using (var Builder = new SQLiteCommandBuilder(Adapter))
                {
                    OpenConnection(conn);
                    return Adapter.Update(table);
                }
            }
        }

        public int UpdateValue(string tableName, string fieldIn, object valueIn, string idField, string idValue, DbTransaction transaction = null)
        {
            throw (new NotImplementedException());
        }

        public DbCommand GetCommand(string qryString = "")
        {
            return new SQLiteCommand(qryString);
        }

        public DbCommand GetCommandFromParams(string query, List<DBQueryParameter> @params)
        {
            var cmd = new SQLiteCommand();
            string ParamQuery = "";
            foreach (var param in @params)
            {
                if (param.Value is bool)
                {
                    ParamQuery += " " + param.FieldName + "=@" + param.FieldName;
                    cmd.Parameters.AddWithValue("@" + param.FieldName, Convert.ToInt32(param.Value));
                }
                else
                {
                    if (param.IsExact)
                    {
                        ParamQuery += " " + param.FieldName + "=@" + param.FieldName;
                        cmd.Parameters.AddWithValue("@" + param.FieldName, param.Value);
                    }
                    else
                    {
                        ParamQuery += " " + param.FieldName + " LIKE @" + param.FieldName;
                        cmd.Parameters.AddWithValue("@" + param.FieldName, "%" + param.Value.ToString() + "%");
                    }
                }
                //Add operator if we are not on the last entry
                if (@params.IndexOf(param) < @params.Count - 1)
                {
                    ParamQuery += " " + param.OperatorString;
                }
            }
            cmd.CommandText = query + ParamQuery;
            return cmd;
        }

        public System.Data.Common.DbDataAdapter GetDataAdapter(string query, bool acceptChanges = true)
        {
            throw (new NotImplementedException());
        }

        #endregion IDataBase

        #endregion Methods
    }
}