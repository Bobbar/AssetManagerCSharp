using Databases.Data;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace AssetManager.Data.Communications
{
    public static class MunisComms
    {
        private const string msSqlConnectString = "server=svr-munis5.core.co.fairfield.oh.us; database=mu_live; trusted_connection=True; Connection Timeout=5";

        public static SqlCommand ReturnSqlCommand(string query)
        {
            SqlConnection conn = new SqlConnection(msSqlConnectString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = query;
            return cmd;
        }

        public static DataTable ReturnSqlTable(string query)
        {
            using (SqlConnection conn = new SqlConnection(msSqlConnectString))
            using (DataTable newTable = new DataTable())
            using (SqlDataAdapter da = new SqlDataAdapter())
            {
                da.SelectCommand = new SqlCommand(query);
                da.SelectCommand.Connection = conn;
                da.Fill(newTable);
                return newTable;
            }
        }

        public static async Task<DataTable> ReturnSqlTableAsync(string query)
        {
            using (SqlConnection conn = new SqlConnection(msSqlConnectString))
            using (DataTable newTable = new DataTable())
            using (SqlCommand cmd = new SqlCommand(query, conn))
            {
                await conn.OpenAsync();
                SqlDataReader dr = await cmd.ExecuteReaderAsync();
                newTable.Load(dr);
                return newTable;
            }
        }

        public static DataTable ReturnSqlTableFromCmd(SqlCommand cmd)
        {
            using (DataTable newTable = new DataTable())
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(newTable);
                cmd.Dispose();
                return newTable;
            }
        }

        public static async Task<DataTable> ReturnSqlTableFromCmdAsync(SqlCommand cmd)
        {
            using (var conn = cmd.Connection)
            using (DataTable newTable = new DataTable())
            {
                await conn.OpenAsync();
                SqlDataReader dr = await cmd.ExecuteReaderAsync();
                newTable.Load(dr);
                cmd.Dispose();
                return newTable;
            }
        }

        public static object ReturnSqlValue(string table, object fieldIn, object valueIn, string fieldOut, object fieldIn2 = null, object valueIn2 = null)
        {
            string query = "";
            QueryParamCollection queryParams = new QueryParamCollection();

            if (fieldIn2 != null && valueIn2 != null)
            {
                query = "SELECT TOP 1 " + fieldOut + " FROM " + table;

                queryParams.Add(fieldIn.ToString(), valueIn.ToString(), true);
                queryParams.Add(fieldIn2.ToString(), valueIn2.ToString(), true);
            }
            else
            {
                query = "SELECT TOP 1 " + fieldOut + " FROM " + table;

                queryParams.Add(fieldIn.ToString(), valueIn.ToString(), true);
            }
            using (var cmd = GetSqlCommandFromParams(query, queryParams.Parameters))
            using (var conn = cmd.Connection)
            {
                cmd.Connection.Open();
                return cmd.ExecuteScalar();
            }
        }

        public static async Task<string> ReturnSqlValueAsync(string table, object fieldIn, object valueIn, string fieldOut, object fieldIn2 = null, object valueIn2 = null)
        {
            string query = "";

            QueryParamCollection queryParams = new QueryParamCollection();

            if (fieldIn2 != null && valueIn2 != null)
            {
                query = "SELECT TOP 1 " + fieldOut + " FROM " + table;

                queryParams.Add(fieldIn.ToString(), valueIn.ToString(), true);
                queryParams.Add(fieldIn2.ToString(), valueIn2.ToString(), true);
            }
            else
            {
                query = "SELECT TOP 1 " + fieldOut + " FROM " + table;

                queryParams.Add(fieldIn.ToString(), valueIn.ToString(), true);
            }

            using (var cmd = GetSqlCommandFromParams(query, queryParams.Parameters))
            using (var conn = cmd.Connection)
            {
                await cmd.Connection.OpenAsync();
                var Value = await cmd.ExecuteScalarAsync();
                if (Value != null)
                {
                    return Value.ToString();
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Takes a partial query string without the WHERE operator, and a list of <see cref="DBQueryParameter"/> and returns a parameterized <see cref="SqlCommand"/>.
        /// </summary>
        /// <param name="partialQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static SqlCommand GetSqlCommandFromParams(string partialQuery, List<DBQueryParameter> parameters)
        {
            var cmd = ReturnSqlCommand(partialQuery);
            cmd.CommandText += " WHERE";
            string paramString = "";
            int valueSeq = 1;
            foreach (var fld in parameters)
            {
                if (fld.IsExact)
                {
                    paramString += " " + fld.FieldName + "=@Value" + valueSeq.ToString();
                    cmd.Parameters.AddWithValue("@Value" + valueSeq.ToString(), fld.Value);
                }
                else
                {
                    paramString += " " + fld.FieldName + " LIKE CONCAT('%', @Value" + valueSeq.ToString() + ", '%')";
                    cmd.Parameters.AddWithValue("@Value" + valueSeq.ToString(), fld.Value);
                }

                // Add operator if we are not on the last parameter.
                if (parameters.IndexOf(fld) < parameters.Count - 1)
                {
                    paramString += " " + fld.OperatorString;
                }

                valueSeq++;
            }

            cmd.CommandText += paramString;
            return cmd;
        }
    }
}