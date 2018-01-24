using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using AssetManager.Helpers;
using AssetManager.Data.Communications;
using AssetDatabase.Data;
using AssetManager.Security;

namespace AssetManager.Data.Functions
{
    public static class DBCacheFunctions
    {

        public static List<string> SQLiteTableHashes;

        public static List<string> RemoteTableHashes;

        public static void RefreshLocalDBCache()
        {
            try
            {
                RefreshSqlCache();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                GlobalSwitches.BuildingCache = false;
            }
        }

        /// <summary>
        /// Builds local hash list and compares to previously built remote hash list. Returns False for mismatch.
        /// </summary>
        /// <param name="cachedMode">When true, only checks for Schema Version since a remote table hash will likely be unavailable.</param>
        /// <returns></returns>
        public static async Task<bool> VerifyLocalCacheHashOnly(bool cachedMode)
        {
            if (!cachedMode)
            {
                if (RemoteTableHashes == null) return false;

                return await Task.Run(() =>
                {
                    List<string> LocalHashes = new List<string>();
                    LocalHashes = LocalTableHashList();
                    return CompareTableHashes(LocalHashes, RemoteTableHashes);
                });
            }
            else
            {
                if (GetSchemaVersion() > 0) return true;
            }
            return false;
        }

        /// <summary>
        /// Builds hash lists for both local and remote tables and compares them.  Returns False for mismatch.
        /// </summary>
        /// <param name="connectedToDB"></param>
        /// <returns></returns>
        public static bool VerifyCacheHashes(bool connectedToDB = true)
        {
            try
            {
                if (GetSchemaVersion() > 0)
                {
                    if (connectedToDB)
                    {
                        SQLiteTableHashes = LocalTableHashList();
                        RemoteTableHashes = RemoteTableHashList();
                        return CompareTableHashes(SQLiteTableHashes, RemoteTableHashes);
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    SQLiteTableHashes = null;
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static bool CheckLocalCacheHash()
        {
            List<string> RemoteHashes = new List<string>();
            RemoteHashes = RemoteTableHashList();
            return CompareTableHashes(RemoteHashes, SQLiteTableHashes);
        }

        public static bool CompareTableHashes(List<string> tableHashesA, List<string> tableHashesB)
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

        public static int GetSchemaVersion()
        {
            var query = "pragma schema_version";
            return System.Convert.ToInt32(DBFactory.GetSqliteDatabase().ExecuteScalarFromQueryString(query));
        }

        public static void RefreshSqlCache()
        {
            try
            {
                if (SQLiteTableHashes != null && CheckLocalCacheHash())
                {
                    return;
                }

                Logging.Logger("Rebuilding local DB cache...");
                GC.Collect();
                if (!File.Exists(Paths.SQLiteDir))
                {
                    Directory.CreateDirectory(Paths.SQLiteDir);
                }
                if (File.Exists(Paths.SQLitePath))
                {
                    File.Delete(Paths.SQLitePath);
                }

                using (var trans = DBFactory.GetSqliteDatabase().StartTransaction())
                {
                    foreach (var table in TableList())
                    {
                        AddTable(table, trans);
                    }
                    trans.Commit();
                }

                SQLiteTableHashes = LocalTableHashList();
                RemoteTableHashes = RemoteTableHashList();
                Logging.Logger("Local DB cache complete...");
            }
            catch (Exception ex)
            {
                Logging.Logger("Errors during cache rebuild!");
                Logging.Logger("STACK TRACE: " + ex.ToString());
            }
        }

        public static List<string> LocalTableHashList()
        {
            try
            {
                List<string> hashList = new List<string>();
                foreach (var table in TableList())
                {
                    using (var results = ToStringTable(DBFactory.GetSqliteDatabase().DataTableFromQueryString("SELECT * FROM " + table)))
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

        public static List<string> RemoteTableHashList()
        {
            List<string> hashList = new List<string>();

            var MySQLDB = DBFactory.GetMySqlDatabase();
            foreach (var table in TableList())
            {
                using (var results = ToStringTable(DBFactory.GetMySqlDatabase().DataTableFromQueryString("SELECT * FROM " + table)))
                {
                    results.TableName = table;
                    hashList.Add(SecurityTools.GetSHAOfTable(results));
                }
            }
            return hashList;
        }

        private static void AddTable(string tableName, DbTransaction transaction)
        {
            CreateCacheTable(tableName, transaction);
            ImportDatabase(tableName, transaction);
        }

        /// <summary>
        /// Builds a Sqlite compatible CREATE statement from a MySQL 'SHOW FULL COLUMNS FROM' query result.
        /// </summary>
        /// <param name="columnResults"></param>
        /// <returns></returns>
        private static string BuildCreateStatement(DataTable columnResults)
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

        private static void CreateCacheTable(string tableName, DbTransaction transaction)
        {
            string createQry;
            using (DataTable tableColumns = GetTableColumns(tableName))
            {
                createQry = BuildCreateStatement(tableColumns);
            }
            DBFactory.GetSqliteDatabase().ExecuteNonQuery(createQry, transaction);
        }

        private static DataTable GetRemoteDBTable(string tableName)
        {
            string qry = "SELECT * FROM " + tableName;

            using (var adapter = DBFactory.GetMySqlDatabase().GetDataAdapter(qry, false))
            using (DataTable results = new DataTable(tableName))
            {
                adapter.Fill(results);
                adapter.SelectCommand.Connection.Close();
                return results;
            }

        }

        private static DataTable GetTableColumns(string tableName)
        {
            string qry = "SHOW FULL COLUMNS FROM " + tableName;
            using (var results = DBFactory.GetMySqlDatabase().DataTableFromQueryString(qry))
            {
                results.TableName = tableName;
                return results;
            }
        }

        private static void ImportDatabase(string tableName, DbTransaction transaction)
        {
            var query = "SELECT * FROM " + tableName;

            using (var remoteTable = GetRemoteDBTable(tableName))
            {
                DBFactory.GetSqliteDatabase().UpdateTable(query, remoteTable, transaction);
            }
        }

        private static List<string> TableList()
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

        private static DataTable ToStringTable(DataTable table)
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




    }
}
