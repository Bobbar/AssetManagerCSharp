using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace Database.Data
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

            if (sqliteConnection == null)
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

        public async Task<DataTable> DataTableFromQueryStringAsync(string query)
        {
            using (DataTable results = new DataTable())
            using (var conn = (SQLiteConnection)NewConnection())
            using (var cmd = new SQLiteCommand(query, conn))
            using (DbDataAdapter da = new SQLiteDataAdapter())
            {
                cmd.Connection = conn;
                da.SelectCommand = cmd;
                await Task.Run(() => da.Fill(results));
                da.SelectCommand.Connection.Dispose();
                return results;
            }
        }

        public DataTable DataTableFromCommand(DbCommand command, DbTransaction transaction = null)
        {
            if (transaction != null)
            {
                var conn = (SQLiteConnection)transaction.Connection;
                using (DbDataAdapter da = new SQLiteDataAdapter())
                using (DataTable results = new DataTable())
                {
                    command.Connection = conn;
                    da.SelectCommand = command;
                    da.Fill(results);
                    command.Dispose();
                    return results;
                }
            }
            else
            {
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
        }

        public DataTable DataTableFromParameters(string query, List<DBQueryParameter> @params)
        {
            using (var cmd = GetCommandFromParams(query, @params))
            using (var results = DataTableFromCommand(cmd))
            {
                return results;
            }
        }

        public DataTable DataTableFromParameters(string query, DBQueryParameter parameters)
        {
            var parms = new List<DBQueryParameter>() { parameters };

            using (var cmd = GetCommandFromParams(query, parms))
            using (var results = DataTableFromCommand(cmd))
            {
                return results;
            }
        }

        public object ExecuteScalarFromCommand(DbCommand command)
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

        public object ExecuteScalarFromQueryString(string query)
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

        public async Task<int> ExecuteNonQueryAsync(string query, DbTransaction transaction = null)
        {
            if (transaction == null)
            {
                using (var conn = (SQLiteConnection)NewConnection())
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    // See async note.
                    return await Task.Run(() =>
                    {
                        OpenConnection(conn);
                        return cmd.ExecuteNonQuery();
                    });
                }
            }
            else
            {
                var conn = (SQLiteConnection)transaction.Connection;
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    // See async note.
                    return await Task.Run(() => cmd.ExecuteNonQuery());
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
                using (var adapter = new SQLiteDataAdapter(cmd))
                using (var builder = new SQLiteCommandBuilder(adapter))
                {
                    return adapter.Update(table);
                }
            }
            else
            {
                using (var conn = (SQLiteConnection)NewConnection())
                using (var adapter = new SQLiteDataAdapter(selectQuery, conn))
                using (var builder = new SQLiteCommandBuilder(adapter))
                {
                    OpenConnection(conn);
                    return adapter.Update(table);
                }
            }
        }

        public async Task<int> UpdateTableAsync(string selectQuery, DataTable table, DbTransaction transaction = null)
        {
            if (transaction != null)
            {
                var conn = (SQLiteConnection)transaction.Connection;
                using (var cmd = new SQLiteCommand(selectQuery, conn, (SQLiteTransaction)transaction))
                using (var adapter = new SQLiteDataAdapter(cmd))
                using (var builder = new SQLiteCommandBuilder(adapter))
                {
                    return await Task.Run(() => { return adapter.Update(table); });
                }
            }
            else
            {
                using (var conn = (SQLiteConnection)NewConnection())
                using (var adapter = new SQLiteDataAdapter(selectQuery, conn))
                using (var builder = new SQLiteCommandBuilder(adapter))
                {
                    await conn.OpenAsync();
                    return await Task.Run(() => { return adapter.Update(table); });
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

        public DbCommand GetCommandFromParams(string query, List<DBQueryParameter> parameters)
        {
            var cmd = new SQLiteCommand();
            DBQueryParameter.AddParamsToCommand(cmd, query, parameters);
            return cmd;
        }

        public DbDataAdapter GetDataAdapter(string query, bool acceptChanges = true)
        {
            throw (new NotImplementedException());
        }

        #endregion IDataBase

        #endregion Methods
    }
}