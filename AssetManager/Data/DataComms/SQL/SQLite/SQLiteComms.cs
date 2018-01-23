﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using AssetManager.Security;
using AssetManager.Data.Functions;
using AssetManager.Helpers;

namespace AssetManager.Data.Communications.Old
{
    public class SQLiteDatabase : IDisposable, IDataBase
    {
        #region Fields

        private const string EncSQLitePass = "X9ow0zCwpGKyVeFR6K3yB4A7lQ2HgOgU";
        private SQLiteConnection Connection { get; set; }
        private string SQLiteConnectString;

        #endregion Fields

        #region Constructors

        public SQLiteDatabase(bool openConnectionOnCall = true)
        {
            SQLiteConnectString = "Data Source=" + Paths.SQLitePath + ";Password=" + SecurityTools.DecodePassword(EncSQLitePass);

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

        public SQLiteConnection NewConnection()
        {
            return new SQLiteConnection(SQLiteConnectString);
        }

        public bool OpenConnection()
        {
            if (ReferenceEquals(Connection, null))
            {
                Connection = NewConnection();
            }
            if (Connection.State != ConnectionState.Open)
            {
                CloseConnection();
                Connection = NewConnection();
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

        #endregion Connection Methods

        #region CacheManagement

        public bool CheckLocalCacheHash()
        {
            List<string> RemoteHashes = new List<string>();
            RemoteHashes = RemoteTableHashList();
            return CompareTableHashes(RemoteHashes, DBCacheFunctions.SQLiteTableHashes);
        }

        public bool CompareTableHashes(List<string> tableHashesA, List<string> tableHashesB)
        {
            try
            {
                if (ReferenceEquals(tableHashesA, null) || ReferenceEquals(tableHashesB, null))
                {
                    return false;
                }
                for (int i = 0; i <= tableHashesA.Count - 1; i++)
                {
                    if (tableHashesA[i] != tableHashesB[i])
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public int GetSchemaVersion()
        {
            using (SQLiteCommand cmd = new SQLiteCommand("pragma schema_version"))
            {
                cmd.Connection = Connection;
                return System.Convert.ToInt32(cmd.ExecuteScalar());
            }
        }

        public void RefreshSqlCache()
        {
            try
            {
                if (DBCacheFunctions.SQLiteTableHashes != null && CheckLocalCacheHash())
                {
                    return;
                }

                Logging.Logger("Rebuilding local DB cache...");
                CloseConnection();
                GC.Collect();
                if (!File.Exists(Paths.SQLiteDir))
                {
                    Directory.CreateDirectory(Paths.SQLiteDir);
                }
                if (File.Exists(Paths.SQLitePath))
                {
                    File.Delete(Paths.SQLitePath);
                }
                SQLiteConnection.CreateFile(Paths.SQLitePath);
                Connection = NewConnection();
                Connection.SetPassword(SecurityTools.DecodePassword(EncSQLitePass));
                OpenConnection();
                using (var trans = Connection.BeginTransaction())
                {
                    foreach (var table in TableList())
                    {
                        AddTable(table, trans);
                    }
                    trans.Commit();
                }

                DBCacheFunctions.SQLiteTableHashes = LocalTableHashList();
                DBCacheFunctions.RemoteTableHashes = RemoteTableHashList();
                Logging.Logger("Local DB cache complete...");
            }
            catch (Exception ex)
            {
                Logging.Logger("Errors during cache rebuild!");
                Logging.Logger("STACK TRACE: " + ex.ToString());
            }
        }

        public List<string> LocalTableHashList()
        {
            try
            {
                List<string> hashList = new List<string>();
                foreach (var table in TableList())
                {
                    using (var results = ToStringTable(DataTableFromQueryString("SELECT * FROM " + table)))
                    {
                        results.TableName = table;
                        hashList.Add(SecurityTools.GetSHAOfTable(results));
                    }
                }
                return hashList;
            }
            catch (Exception)
            {
                return default(List<string>);
            }
        }

        public List<string> RemoteTableHashList()
        {
            List<string> hashList = new List<string>();
            using (MySQLDatabase MySQLDB = new MySQLDatabase())
            {
                foreach (var table in TableList())
                {
                    using (var results = ToStringTable(MySQLDB.DataTableFromQueryString("SELECT * FROM " + table)))
                    {
                        results.TableName = table;
                        hashList.Add(SecurityTools.GetSHAOfTable(results));
                    }
                }
                return hashList;
            }
        }

        private void AddTable(string tableName, SQLiteTransaction transaction)
        {
            CreateCacheTable(tableName, transaction);
            ImportDatabase(tableName, transaction);
        }

        /// <summary>
        /// Builds a SQLite compatible CREATE statement from a MySQL 'SHOW FULL COLUMNS FROM' query result.
        /// </summary>
        /// <param name="columnResults"></param>
        /// <returns></returns>
        private string BuildCreateStatement(DataTable columnResults)
        {

            // List for primary keys.
            var keys = new List<string>();

            string statement = "CREATE TABLE ";

            // Add the table name from the results parameter.
            // **REMEMEBER TO ADD THE TABLE NAME TO THE RESULTS DATATABLE BEFORE CALLING THIS FUNCTION**
            statement += " `" + columnResults.TableName + "` ( ";

            // Iterate through the table rows.
            foreach (DataRow row in columnResults.Rows)
            {
                // Add the field/column name and data type to the statement.
                statement += "`" + row["Field"].ToString() + "` ";
                statement += row["Type"].ToString();

                // If the current field/column is a primary key, add it to the keys list.
                if (row["Key"].ToString() == "PRI")
                {
                    keys.Add(row["Field"].ToString());
                }

                // Add a column delimiter if we are not on the last item.
                if (columnResults.Rows.IndexOf(row) != (columnResults.Rows.Count - 1)) statement += ", ";
            }


            // Add primary keys declaration.
            if (keys.Count > 0)
            {
                // Declaration header and open parentheses.
                statement += ", PRIMARY KEY (";

                foreach (string key in keys)
                {
                    // Add keys string and delimiter, if needed.
                    statement += key;
                    if (keys.IndexOf(key) != (keys.Count - 1)) statement += ", ";
                }

                // Close parentheses.
                statement += ")";
            }

            // End of statement close parentheses.
            statement += ");";

            return statement;
        }

        private void CreateCacheTable(string tableName, SQLiteTransaction transaction)
        {
            string createQry;
            using (DataTable tableColumns = GetTableColumns(tableName))
            {
                createQry = BuildCreateStatement(tableColumns);
            }

            using (SQLiteCommand cmd = new SQLiteCommand(createQry, Connection))
            {
                cmd.Transaction = transaction;
                cmd.ExecuteNonQuery();
            }

        }

        private DataTable GetRemoteDBTable(string tableName)
        {
            string qry = "SELECT * FROM " + tableName;
            using (MySQLDatabase MySQLDB = new MySQLDatabase())
            {
                using (DataTable results = new DataTable())
                {
                    using (var conn = MySQLDB.NewConnection())
                    {
                        using (var adapter = MySQLDB.ReturnMySqlAdapter(qry, conn))
                        {
                            adapter.AcceptChangesDuringFill = false;
                            adapter.Fill(results);
                            results.TableName = tableName;
                            return results;
                        }
                    }
                }
            }
        }

        private DataTable GetTableColumns(string tableName)
        {
            string qry = "SHOW FULL COLUMNS FROM " + tableName;
            using (MySQLDatabase MySQLDB = new MySQLDatabase())
            {
                using (var results = MySQLDB.DataTableFromQueryString(qry))
                {
                    results.TableName = tableName;
                    return results;
                }
            }
        }

        private void ImportDatabase(string tableName, SQLiteTransaction transaction)
        {
            OpenConnection();
            using (var cmd = Connection.CreateCommand())
            {
                using (var adapter = new SQLiteDataAdapter(cmd))
                {
                    using (SQLiteCommandBuilder builder = new SQLiteCommandBuilder(adapter))
                    {
                        cmd.Transaction = transaction;
                        cmd.CommandText = "SELECT * FROM " + tableName;
                        adapter.Update(GetRemoteDBTable(tableName));
                    }
                }
            }
        }

        private List<string> TableList()
        {
            List<string> list = new List<string>();
            list.Add(DevicesCols.TableName);
            list.Add(HistoricalDevicesCols.TableName);
            list.Add(TrackablesCols.TableName);
            list.Add(SibiRequestCols.TableName);
            list.Add(SibiRequestItemsCols.TableName);
            list.Add(SibiNotesCols.TableName);
            list.Add(DeviceComboCodesCols.TableName);
            list.Add(SibiComboCodesCols.TableName);
            list.Add("munis_codes");
            list.Add(SecurityCols.TableName);
            list.Add(UsersCols.TableName);
            list.Add("device_ping_history");
            list.Add("munis_departments");
            return list;
        }

        private DataTable ToStringTable(DataTable table)
        {
            DataTable tmpTable = table.Clone();
            for (var i = 0; i <= tmpTable.Columns.Count - 1; i++)
            {
                tmpTable.Columns[i].DataType = typeof(string);
            }
            foreach (DataRow row in table.Rows)
            {
                tmpTable.ImportRow(row);
            }
            table.Dispose();
            return tmpTable;
        }

        #endregion CacheManagement

        #region IDataBase

        public DbTransaction StartTransaction()
        {
            throw (new NotImplementedException());
        }

        public DataTable DataTableFromQueryString(string query)
        {
            using (DataTable results = new DataTable())
            using (DbDataAdapter da = new SQLiteDataAdapter())
            using (var cmd = new SQLiteCommand(query))
            using (var conn = NewConnection())
            {
                cmd.Connection = conn;
                da.SelectCommand = cmd;
                da.Fill(results);
                return results;
                //						da.SelectCommand.Connection.Dispose();
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
            using (var conn = NewConnection())
            using (SQLiteCommand cmd = new SQLiteCommand(query, conn))
            {
                cmd.Connection.Open();
                return cmd.ExecuteScalar();
            }
        }

        public int ExecuteQuery(string query)
        {
            throw (new NotImplementedException());
        }

        public int InsertFromParameters(string tableName, List<DBParameter> @params, DbTransaction transaction = null)
        {
            throw (new NotImplementedException());
        }

        public int UpdateTable(string selectQuery, DataTable table, DbTransaction transaction = null)
        {
            throw (new NotImplementedException());
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