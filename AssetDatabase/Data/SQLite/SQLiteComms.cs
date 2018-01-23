using System.Data.SQLite;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace AssetDatabase.Data
{
    public class SqliteDatabase : IDisposable, IDataBase
    {
        #region Fields

        public string SqlitePath { get; set; } = string.Empty;
        public string SqlitePass { get; set; } = string.Empty;



        private SQLiteConnection Connection { get; set; }
        private string SqliteConnectString;

        #endregion Fields

        #region Constructors

        public SqliteDatabase(bool openConnectionOnCall = true)
        {

            SqliteConnectString = "Data Source=" + SqlitePath + ";Password=" + SqlitePass;

            if (openConnectionOnCall)
            {
                if (!OpenConnection())
                {
                }
            }
        }

        #endregion Constructors

        #region Methods

        #region Connection Methods

        public void CloseConnection()
        {
            if (Connection != null)
            {

                Connection.Close();
                Connection.Dispose();
            }
        }

        public DbConnection NewConnection()
        {
            SqliteConnectString = "Data Source=" + SqlitePath;
            return new SQLiteConnection(SqliteConnectString);
        }

        public bool OpenConnection()
        {
            if (ReferenceEquals(Connection, null))
            {
                Connection = (SQLiteConnection)NewConnection();
                if (!string.IsNullOrEmpty(SqlitePass))
                {
                    Connection.SetPassword(SqlitePass);
                }

            }
            if (Connection.State != ConnectionState.Open)
            {
                CloseConnection();
                Connection = (SQLiteConnection)NewConnection();
                Connection.Open();
            }
            if (Connection.State == ConnectionState.Open)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool OpenConnection(DbConnection connection, bool overrideNoPing = false)
        {
            throw new NotImplementedException();
        }

        #endregion Connection Methods

        #region CacheManagement

        public void CreateFile(string path)
        {
            using (SQLiteConnection conn = (SQLiteConnection)NewConnection())
            {
                conn.Open();
            }
        }

        //public bool CheckLocalCacheHash()
        //{
        //    List<string> RemoteHashes = new List<string>();
        //    RemoteHashes = RemoteTableHashList();
        //    return CompareTableHashes(RemoteHashes, DBCacheFunctions.SqliteTableHashes);
        //}

        //public bool CompareTableHashes(List<string> tableHashesA, List<string> tableHashesB)
        //{
        //    try
        //    {
        //        if (ReferenceEquals(tableHashesA, null) || ReferenceEquals(tableHashesB, null))
        //        {
        //            return false;
        //        }
        //        for (int i = 0; i <= tableHashesA.Count - 1; i++)
        //        {
        //            if (tableHashesA[i] != tableHashesB[i])
        //            {
        //                return false;
        //            }
        //        }
        //        return true;
        //    }
        //    catch (Exception)
        //    {
        //        return false;
        //    }
        //}

        //public int GetSchemaVersion()
        //{
        //    using (SqliteCommand cmd = new SqliteCommand("pragma schema_version"))
        //    {
        //        cmd.Connection = Connection;
        //        return System.Convert.ToInt32(cmd.ExecuteScalar());
        //    }
        //}

        //public void RefreshSqlCache()
        //{
        //    try
        //    {
        //        if (DBCacheFunctions.SqliteTableHashes != null && CheckLocalCacheHash())
        //        {
        //            return;
        //        }

        //        Logging.Logger("Rebuilding local DB cache...");
        //        CloseConnection();
        //        GC.Collect();
        //        if (!File.Exists(Paths.SqliteDir))
        //        {
        //            Directory.CreateDirectory(Paths.SqliteDir);
        //        }
        //        if (File.Exists(Paths.SqlitePath))
        //        {
        //            File.Delete(Paths.SqlitePath);
        //        }
        //        SqliteConnection.CreateFile(Paths.SqlitePath);
        //        Connection = NewConnection();
        //        Connection.SetPassword(SecurityTools.DecodePassword(EncSqlitePass));
        //        OpenConnection();
        //        using (var trans = Connection.BeginTransaction())
        //        {
        //            foreach (var table in TableList())
        //            {
        //                AddTable(table, trans);
        //            }
        //            trans.Commit();
        //        }

        //        DBCacheFunctions.SqliteTableHashes = LocalTableHashList();
        //        DBCacheFunctions.RemoteTableHashes = RemoteTableHashList();
        //        Logging.Logger("Local DB cache complete...");
        //    }
        //    catch (Exception ex)
        //    {
        //        Logging.Logger("Errors during cache rebuild!");
        //        Logging.Logger("STACK TRACE: " + ex.ToString());
        //    }
        //}

        //public List<string> LocalTableHashList()
        //{
        //    try
        //    {
        //        List<string> hashList = new List<string>();
        //        foreach (var table in TableList())
        //        {
        //            using (var results = ToStringTable(DataTableFromQueryString("SELECT * FROM " + table)))
        //            {
        //                results.TableName = table;
        //                hashList.Add(SecurityTools.GetSHAOfTable(results));
        //            }
        //        }
        //        return hashList;
        //    }
        //    catch (Exception)
        //    {
        //        return default(List<string>);
        //    }
        //}

        //public List<string> RemoteTableHashList()
        //{
        //    List<string> hashList = new List<string>();
        //    using (MySQLDatabase MySQLDB = new MySQLDatabase())
        //    {
        //        foreach (var table in TableList())
        //        {
        //            using (var results = ToStringTable(MySQLDB.DataTableFromQueryString("SELECT * FROM " + table)))
        //            {
        //                results.TableName = table;
        //                hashList.Add(SecurityTools.GetSHAOfTable(results));
        //            }
        //        }
        //        return hashList;
        //    }
        //}

        //private void AddTable(string tableName, SqliteTransaction transaction)
        //{
        //    CreateCacheTable(tableName, transaction);
        //    ImportDatabase(tableName, transaction);
        //}

        ///// <summary>
        ///// Builds a Sqlite compatible CREATE statement from a MySQL 'SHOW FULL COLUMNS FROM' query result.
        ///// </summary>
        ///// <param name="columnResults"></param>
        ///// <returns></returns>
        //private string BuildCreateStatement(DataTable columnResults)
        //{

        //    // List for primary keys.
        //    var keys = new List<string>();

        //    string statement = "CREATE TABLE ";

        //    // Add the table name from the results parameter.
        //    // **REMEMEBER TO ADD THE TABLE NAME TO THE RESULTS DATATABLE BEFORE CALLING THIS FUNCTION**
        //    statement += " `" + columnResults.TableName + "` ( ";

        //    // Iterate through the table rows.
        //    foreach (DataRow row in columnResults.Rows)
        //    {
        //        // Add the field/column name and data type to the statement.
        //        statement += "`" + row["Field"].ToString() + "` ";
        //        statement += row["Type"].ToString();

        //        // If the current field/column is a primary key, add it to the keys list.
        //        if (row["Key"].ToString() == "PRI")
        //        {
        //            keys.Add(row["Field"].ToString());
        //        }

        //        // Add a column delimiter if we are not on the last item.
        //        if (columnResults.Rows.IndexOf(row) != (columnResults.Rows.Count - 1)) statement += ", ";
        //    }


        //    // Add primary keys declaration.
        //    if (keys.Count > 0)
        //    {
        //        // Declaration header and open parentheses.
        //        statement += ", PRIMARY KEY (";

        //        foreach (string key in keys)
        //        {
        //            // Add keys string and delimiter, if needed.
        //            statement += key;
        //            if (keys.IndexOf(key) != (keys.Count - 1)) statement += ", ";
        //        }

        //        // Close parentheses.
        //        statement += ")";
        //    }

        //    // End of statement close parentheses.
        //    statement += ");";

        //    return statement;
        //}

        //private void CreateCacheTable(string tableName, SqliteTransaction transaction)
        //{
        //    string createQry;
        //    using (DataTable tableColumns = GetTableColumns(tableName))
        //    {
        //        createQry = BuildCreateStatement(tableColumns);
        //    }

        //    using (SqliteCommand cmd = new SqliteCommand(createQry, Connection))
        //    {
        //        cmd.Transaction = transaction;
        //        cmd.ExecuteNonQuery();
        //    }

        //}

        //private DataTable GetRemoteDBTable(string tableName)
        //{
        //    string qry = "SELECT * FROM " + tableName;
        //    using (MySQLDatabase MySQLDB = new MySQLDatabase())
        //    {
        //        using (DataTable results = new DataTable())
        //        {
        //            using (var conn = MySQLDB.NewConnection())
        //            {
        //                using (var adapter = MySQLDB.ReturnMySqlAdapter(qry, conn))
        //                {
        //                    adapter.AcceptChangesDuringFill = false;
        //                    adapter.Fill(results);
        //                    results.TableName = tableName;
        //                    return results;
        //                }
        //            }
        //        }
        //    }
        //}

        //private DataTable GetTableColumns(string tableName)
        //{
        //    string qry = "SHOW FULL COLUMNS FROM " + tableName;
        //    using (MySQLDatabase MySQLDB = new MySQLDatabase())
        //    {
        //        using (var results = MySQLDB.DataTableFromQueryString(qry))
        //        {
        //            results.TableName = tableName;
        //            return results;
        //        }
        //    }
        //}

        //private void ImportDatabase(string tableName, SqliteTransaction transaction)
        //{
        //    OpenConnection();
        //    using (var cmd = Connection.CreateCommand())
        //    {
        //        using (var adapter = new SqliteDataAdapter(cmd))
        //        {
        //            using (SqliteCommandBuilder builder = new SqliteCommandBuilder(adapter))
        //            {
        //                cmd.Transaction = transaction;
        //                cmd.CommandText = "SELECT * FROM " + tableName;
        //                adapter.Update(GetRemoteDBTable(tableName));
        //            }
        //        }
        //    }
        //}

        //private List<string> TableList()
        //{
        //    List<string> list = new List<string>();
        //    list.Add(DevicesCols.TableName);
        //    list.Add(HistoricalDevicesCols.TableName);
        //    list.Add(TrackablesCols.TableName);
        //    list.Add(SibiRequestCols.TableName);
        //    list.Add(SibiRequestItemsCols.TableName);
        //    list.Add(SibiNotesCols.TableName);
        //    list.Add(DeviceComboCodesCols.TableName);
        //    list.Add(SibiComboCodesCols.TableName);
        //    list.Add("munis_codes");
        //    list.Add(SecurityCols.TableName);
        //    list.Add(UsersCols.TableName);
        //    list.Add("device_ping_history");
        //    list.Add("munis_departments");
        //    return list;
        //}

        //private DataTable ToStringTable(DataTable table)
        //{
        //    DataTable tmpTable = table.Clone();
        //    for (var i = 0; i <= tmpTable.Columns.Count - 1; i++)
        //    {
        //        tmpTable.Columns[i].DataType = typeof(string);
        //    }
        //    foreach (DataRow row in table.Rows)
        //    {
        //        tmpTable.ImportRow(row);
        //    }
        //    table.Dispose();
        //    return tmpTable;
        //}

        #endregion CacheManagement

        #region IDataBase

        public DbTransaction StartTransaction()
        {

            var conn = (SQLiteConnection)NewConnection();
            conn.Open();
            return conn.BeginTransaction();

            //throw (new NotImplementedException());
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
                    command.Connection.Open();
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
                cmd.Connection.Open();
                return cmd.ExecuteScalar();
            }
        }

        public int ExecuteNonQuery(string query, DbTransaction transaction = null)
        {
            if (transaction == null)
            {
                using (var conn = (SQLiteConnection)NewConnection())
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    conn.Open();
                    return cmd.ExecuteNonQuery();
                }

            }
            else
            {
                var conn = (SQLiteConnection)transaction.Connection;
                using (var cmd = new SQLiteCommand(query, conn))
                {
                    //conn.Open();
                    return cmd.ExecuteNonQuery();
                }
            }

            //throw (new NotImplementedException());
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
                    conn.Open();
                    return Adapter.Update(table);
                }
            }



            // throw (new NotImplementedException());
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

        #region IDisposable Support

        private bool disposedValue; // To detect redundant calls

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            // TODO: uncomment the following line if Finalize() is overridden above.
            // GC.SuppressFinalize(Me)
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }
                CloseConnection();

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

        #endregion IDisposable Support
    }
}