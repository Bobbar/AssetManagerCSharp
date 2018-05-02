using System.Collections.Generic;
using System.Data;
using System.Data.Common;
namespace AssetDatabase.Data
{

    public interface IDatabase
    {

        DbConnection NewConnection();

        /// <summary>
        /// Trys to open the specified connection. Returns true if successful.
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="overrideNoPing">True if connection will try to be opened even if a server-offline flag is set.</param>
        /// <returns></returns>
        bool OpenConnection(DbConnection connection, bool overrideNoPing);

        /// <summary>
        /// Trys to open the specified connection. Returns true if successful.
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        bool OpenConnection(DbConnection connection);

        /// <summary>
        /// Returns a new <see cref="DbTransaction"/>.
        /// </summary>
        /// <returns></returns>
        DbTransaction StartTransaction();

        /// <summary>
        /// Returns a DataTable from a SQL query string.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        DataTable DataTableFromQueryString(string query);

        /// <summary>
        /// Returns a DataTable from a <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        DataTable DataTableFromCommand(DbCommand command, DbTransaction transaction = null);

        /// <summary>
        /// Returns a DataTable from a partial SQL query string and a <see cref="List{T}"/> of <see cref="DBQueryParameter"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        DataTable DataTableFromParameters(string query, List<DBQueryParameter> @params);

        /// <summary>
        /// Returns an object value from a <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="command"></param>
        /// <returns></returns>
        object ExecuteScalarFromCommand(DbCommand command);

        /// <summary>
        ///  Returns an object value from a SQL query string.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        object ExecuteScalarFromQueryString(string query);

        /// <summary>
        /// Executes a non query and returns the number of rows affected.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        int ExecuteNonQuery(string query, DbTransaction transaction = null);

        /// <summary>
        /// Inserts a list of <see cref="DBParameter"/> into the specified table. Returns the number of rows affected.
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="params"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int InsertFromParameters(string tableName, List<DBParameter> @params, DbTransaction transaction = null);

        /// <summary>
        /// Updates the table returned by the <paramref name="selectQuery"/> with the specified DataTable. Returns rows affected.
        /// </summary>
        /// <param name="selectQuery"></param>
        /// <param name="table"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int UpdateTable(string selectQuery, DataTable table, DbTransaction transaction = null);

        /// <summary>
        /// Updates a single value in the database. Values are parameterized before execution. 
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="fieldIn"></param>
        /// <param name="valueIn"></param>
        /// <param name="idField"></param>
        /// <param name="idValue"></param>
        /// <param name="transaction"></param>
        /// <returns></returns>
        int UpdateValue(string tableName, string fieldIn, object valueIn, string idField, string idValue, DbTransaction transaction = null);

        /// <summary>
        /// Returns a new <see cref="DbCommand"/>.
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        DbCommand GetCommand(string query = "");

        /// <summary>
        /// Returns a new <see cref="DbCommand"/> with the specified parameters added.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="params"></param>
        /// <returns></returns>
        DbCommand GetCommandFromParams(string query, List<DBQueryParameter> @params);


        /// <summary>
        /// Returns a new data adapter.
        /// </summary>
        /// <param name="query"></param>
        /// <param name="acceptChanges">False if rows returned by this adapter will be marked as new or added. 
        /// Useful for inserting data from this adapter into another database.</param>
        /// <returns></returns>
        DbDataAdapter GetDataAdapter(string query, bool acceptChanges = true);

    }
}
