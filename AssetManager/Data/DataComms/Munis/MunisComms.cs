using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using AssetManager.Helpers;
using AssetDatabase.Data;

namespace AssetManager.Data.Communications
{

    public class MunisComms
    {

        #region Fields

        private const string msSqlConnectString = "server=svr-munis5.core.co.fairfield.oh.us; database=mu_live; trusted_connection=True;";

        #endregion

        #region Methods

        public SqlCommand ReturnSqlCommand(string sqlQry)
        {
            SqlConnection conn = new SqlConnection(msSqlConnectString);
            SqlCommand cmd = new SqlCommand();
            cmd.Connection = conn;
            cmd.CommandText = sqlQry;
            return cmd;
        }

        public DataTable ReturnSqlTable(string sqlQry)
        {
            using (SqlConnection conn = new SqlConnection(msSqlConnectString))
            using (DataTable NewTable = new DataTable())
            using (SqlDataAdapter da = new SqlDataAdapter())
            {
                da.SelectCommand = new SqlCommand(sqlQry);
                da.SelectCommand.Connection = conn;
                da.Fill(NewTable);
                return NewTable;
            }
        }

        public async Task<DataTable> ReturnSqlTableAsync(string sqlQry)
        {
            using (SqlConnection conn = new SqlConnection(msSqlConnectString))
            using (DataTable NewTable = new DataTable())
            using (SqlCommand cmd = new SqlCommand(sqlQry, conn))
            {
                await conn.OpenAsync();
                SqlDataReader dr = await cmd.ExecuteReaderAsync();
                NewTable.Load(dr);
                return NewTable;
            }
        }

        public DataTable ReturnSqlTableFromCmd(SqlCommand cmd)
        {
            using (DataTable NewTable = new DataTable())
            using (SqlDataAdapter da = new SqlDataAdapter(cmd))
            {
                da.Fill(NewTable);
                cmd.Dispose();
                return NewTable;
            }
        }

        public async Task<DataTable> ReturnSqlTableFromCmdAsync(SqlCommand cmd)
        {
            using (var conn = cmd.Connection)
            using (DataTable NewTable = new DataTable())
            {
                await conn.OpenAsync();
                SqlDataReader dr = await cmd.ExecuteReaderAsync();
                NewTable.Load(dr);
                cmd.Dispose();
                return NewTable;
            }
        }

        public object ReturnSqlValue(string table, object fieldIn, object valueIn, string fieldOut, object fieldIn2 = null, object valueIn2 = null)
        {
            string sqlQRY = "";
            QueryParamCollection queryParams = new QueryParamCollection();

            if (fieldIn2 != null && valueIn2 != null)
            {
                sqlQRY = "SELECT TOP 1 " + fieldOut + " FROM " + table;

                queryParams.Add(fieldIn.ToString(), valueIn.ToString(), true);
                queryParams.Add(fieldIn2.ToString(), valueIn2.ToString(), true);
            }
            else
            {
                sqlQRY = "SELECT TOP 1 " + fieldOut + " FROM " + table;

                queryParams.Add(fieldIn.ToString(), valueIn.ToString(), true);
            }
            using (var cmd = GetSqlCommandFromParams(sqlQRY, queryParams.Parameters))
            using (var conn = cmd.Connection)
            {
                cmd.Connection.Open();
                return cmd.ExecuteScalar();
            }
        }

        public async Task<string> ReturnSqlValueAsync(string table, object fieldIn, object valueIn, string fieldOut, object fieldIn2 = null, object valueIn2 = null)
        {
            try
            {
                string sqlQRY = "";

                QueryParamCollection queryParams = new QueryParamCollection();

                if (fieldIn2 != null && valueIn2 != null)
                {
                    sqlQRY = "SELECT TOP 1 " + fieldOut + " FROM " + table;

                    queryParams.Add(fieldIn.ToString(), valueIn.ToString(), true);
                    queryParams.Add(fieldIn2.ToString(), valueIn2.ToString(), true);
                }
                else
                {
                    sqlQRY = "SELECT TOP 1 " + fieldOut + " FROM " + table;

                    queryParams.Add(fieldIn.ToString(), valueIn.ToString(), true);
                }

                using (var cmd = GetSqlCommandFromParams(sqlQRY, queryParams.Parameters))
                using (var conn = cmd.Connection)
                {
                    await cmd.Connection.OpenAsync();
                    var Value = await cmd.ExecuteScalarAsync();
                    if (Value != null)
                    {
                        return Value.ToString();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            return string.Empty;
        }

        /// <summary>
        /// Takes a partial query string without the WHERE operator, and a list of <see cref="DBQueryParameter"/> and returns a parameterized <see cref="SqlCommand"/>.
        /// </summary>
        /// <param name="partialQuery"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public SqlCommand GetSqlCommandFromParams(string partialQuery, List<DBQueryParameter> parameters)
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

        #endregion

    }
}