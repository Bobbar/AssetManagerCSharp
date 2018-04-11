using AssetManager.Data.Classes;
using System.Windows.Forms;

namespace AssetManager.Data.Functions
{
    /// <summary>
    /// Instantiate and assign to <see cref="Control.Tag"/> property to enable DBParsing functions.
    /// </summary>
    public class DBControlInfo
    {
        #region "Fields"

        private DbAttributes attribute;
        private string columnName;
        private ParseType parseType;
        private bool isRequired;

        #endregion "Fields"

        #region "Constructors"

        public DBControlInfo(string columnName, DbAttributes attribute, ParseType parseType, bool required)
        {
            this.columnName = columnName;
            isRequired = required;
            this.parseType = parseType;
            this.attribute = attribute;
        }

        #endregion "Constructors"

        #region "Properties"

        /// <summary>
        /// Gets or sets the <see cref="DbAttributes"/> for <see cref="ComboBox"/> controls.
        /// </summary>
        /// <returns></returns>
        public DbAttributes Attributes
        {
            get { return attribute; }
            set { attribute = value; }
        }

        /// <summary>
        /// Gets or sets the Database Column used to update and/or populate the assigned control.
        /// </summary>
        /// <returns></returns>
        public string ColumnName
        {
            get { return columnName; }
            set { columnName = value; }
        }

        /// <summary>
        /// Gets or sets <seealso cref="ParseType"/>
        /// </summary>
        /// <returns></returns>
        public ParseType ParseType
        {
            get { return parseType; }
            set { parseType = value; }
        }

        /// <summary>
        /// Is the Control a required field?
        /// </summary>
        /// <returns></returns>
        public bool Required
        {
            get { return isRequired; }
            set { isRequired = value; }
        }

        #endregion "Properties"
    }
}
