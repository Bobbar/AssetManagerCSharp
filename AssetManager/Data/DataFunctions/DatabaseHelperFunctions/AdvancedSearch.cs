using System.Collections.Generic;
using System.Data;
using AssetManager.Data.Communications;
using AssetManager.Helpers;
namespace AssetManager.Data.Functions
{
    public class AdvancedSearch
    {
        #region "Fields"

        private string _searchString;

        private List<TableInfo> _searchTables;

        #endregion "Fields"

        #region "Constructors"

        public AdvancedSearch(string searchString, List<TableInfo> searchTables)
        {
            _searchString = searchString;
            _searchTables = searchTables;
        }

        public AdvancedSearch()
        {
            _searchString = string.Empty;
            _searchTables = null;
        }

        #endregion "Constructors"

        #region "Methods"

        public List<string> GetColumns(string table)
        {
            List<string> colList = new List<string>();
            var SQLQry = "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = '" + ServerInfo.CurrentDataBase.ToString() + "' AND TABLE_NAME = '" + table + "'";
            var results = DBFactory.GetDatabase().DataTableFromQueryString(SQLQry);
            foreach (DataRow row in results.Rows)
            {
                colList.Add(row["COLUMN_NAME"].ToString());
            }
            return colList;
        }


        public List<DataTable> GetResults()
        {
            List<DataTable> resultsList = new List<DataTable>();
            foreach (TableInfo table in _searchTables)
            {
                string qry = "SELECT " + BuildSelectString(table) + " FROM " + table.TableName + " WHERE ";
                qry += BuildFieldString(table);
                using (var cmd = DBFactory.GetMySqlDatabase().GetCommand(qry))
                {
                    cmd.AddParameterWithValue("@" + "SEARCHVAL", _searchString);
                    var results = DBFactory.GetMySqlDatabase().DataTableFromCommand(cmd);
                    results.TableName = table.TableName;
                    resultsList.Add(results);
                    results.Dispose();
                }
            }
            return resultsList;
        }

        public DataTable GetSingleTableResults(string searchString, string tableName)
        {
            var searchTableInfo = new TableInfo(tableName, GetColumns(tableName));
            string qry = "SELECT " + BuildSelectString(searchTableInfo) + " FROM " + searchTableInfo.TableName + " WHERE ";
            qry += BuildFieldString(searchTableInfo);
            using (var cmd = DBFactory.GetMySqlDatabase().GetCommand(qry))
            {
                cmd.AddParameterWithValue("@" + "SEARCHVAL", searchString);
                var results = DBFactory.GetMySqlDatabase().DataTableFromCommand(cmd);
                results.TableName = searchTableInfo.TableName;
                return results;
            }
        }

        private string BuildFieldString(TableInfo table)
        {
            string Fields = "";
            foreach (string col in table.Columns)
            {
                Fields += table.TableName + "." + col + " LIKE CONCAT('%', @SEARCHVAL, '%')";
                if (table.Columns.IndexOf(col) != table.Columns.Count - 1)
                    Fields += " OR ";
            }
            return Fields;
        }

        private string BuildSelectString(TableInfo table)
        {
            string SelectString = "";
            foreach (string column in table.Columns)
            {
                SelectString += column;
                if (table.Columns.IndexOf(column) != table.Columns.Count - 1)
                    SelectString += ",";
            }
            return SelectString;
        }

        #endregion "Methods"
    }

    #region "Structs"

    public struct TableInfo
    {
        #region "Fields"

        public List<string> Columns { get; }
        public string TableKey { get; set; }
        public string TableName { get; set; }

        #endregion "Fields"

        #region "Constructors"

        public TableInfo(string name, List<string> cols)
        {
            TableName = name;
            Columns = cols;
            TableKey = string.Empty;
        }

        #endregion "Constructors"
    }

    #endregion "Structs"
}