using AssetManager.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Threading.Tasks;

namespace AssetManager.Data.Functions
{
    public static class DBCacheFunctions
    {
        private static string CacheVersionGuid;

        public static void RefreshLocalDBCache()
        {
            try
            {
                RefreshSqlCache();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                GlobalSwitches.BuildingCache = false;
            }
        }

        private static string GetCacheVersion()
        {
            var guid = DBFactory.GetSqliteDatabase().ExecuteScalarFromQueryString("SELECT guid FROM db_guid").ToString();
            return guid;
        }

        private static string GetDBVersion()
        {
            var guid = DBFactory.GetMySqlDatabase().ExecuteScalarFromQueryString("SELECT guid FROM db_guid").ToString();
            return guid;
        }

        public static bool CacheUpToDate(bool connectedToDB = true)
        {
            try
            {
                if (GetSchemaVersion() > 0)
                {
                    if (connectedToDB)
                    {
                        CacheVersionGuid = GetCacheVersion();
                        var DBVersion = GetDBVersion();

                        if (CacheVersionGuid != DBVersion)
                        {
                            return false;
                        }
                        else
                        {
                            return true;
                        }
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    CacheVersionGuid = string.Empty;
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public static int GetSchemaVersion()
        {
            try
            {
                var query = "pragma schema_version";
                return System.Convert.ToInt32(DBFactory.GetSqliteDatabase().ExecuteScalarFromQueryString(query));
            }
            catch
            {
                return -1;
            }
        }

        public static void RefreshSqlCache()
        {
            try
            {
                if (CacheUpToDate())
                {
                    return;
                }

                var startTime = DateTime.Now.Ticks;

                Logging.Logger("Rebuilding local DB cache...");

                //GC.Collect();
                //GC.WaitForPendingFinalizers();

                if (!File.Exists(Paths.SQLiteDir))
                {
                    Directory.CreateDirectory(Paths.SQLiteDir);
                }
                if (File.Exists(Paths.SQLitePath))
                {
                    //File.Delete(Paths.SQLitePath);
                    DropTables();
                }

                using (var trans = DBFactory.GetSqliteDatabase().StartTransaction())
                using (var conn = trans.Connection)
                {
                    foreach (var table in TableList())
                    {
                        AddTable(table, trans);
                    }
                    trans.Commit();
                }

                var elapTime = (DateTime.Now.Ticks - startTime) / 10000;

                Logging.Logger("Local DB cache complete. (" + elapTime + "ms)");
            }
            catch (Exception ex)
            {
                Logging.Logger("Errors during cache rebuild!");
                Logging.Logger("STACK TRACE: " + ex.ToString());
            }
        }

        private static void DropTables()
        {
            string query = "SELECT * FROM sqlite_master WHERE type='table'";

            using (var trans = DBFactory.GetSqliteDatabase().StartTransaction())
            using (var conn = trans.Connection)
            using (var results = DBFactory.GetSqliteDatabase().DataTableFromQueryString(query))
            {
                try
                {
                    foreach (DataRow row in results.Rows)
                    {
                        string dropQuery = "DROP TABLE " + row["name"];
                        DBFactory.GetSqliteDatabase().ExecuteNonQuery(dropQuery, trans);
                    }
                    trans.Commit();
                }
                catch
                {
                    trans.Rollback();
                }
            }
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
            list.Add("db_guid");
            return list;
        }
    }
}