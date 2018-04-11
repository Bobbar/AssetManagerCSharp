using AssetManager.Data.Classes;
using AssetManager.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace AssetManager.Data.Functions
{
    /// <summary>
    /// Determines how the parser handles the updating and filling of a control.
    /// </summary>
    public enum ParseType
    {
        /// <summary>
        /// The control is filled only.
        /// </summary>
        DisplayOnly,

        /// <summary>
        /// The control is filled and will also be added to Update and Insert tables.
        /// </summary>
        UpdateAndDisplay
    }
}

namespace AssetManager.Data.Functions
{
    /// <summary>
    /// Instantiate and assign to <see cref="Control.Tag"/> property to enable DBParsing functions.
    /// </summary>
    public class DBControlInfo
    {
        #region "Fields"

        private DbAttributes dbAttrib;
        private string columnName;
        private ParseType parseType;

        private bool db_required;

        #endregion "Fields"

        #region "Constructors"

        public DBControlInfo()
        {
            columnName = "";
            db_required = false;
            parseType = ParseType.DisplayOnly;
            dbAttrib = null;
        }

        public DBControlInfo(string dataColumn, ParseType parseType, bool required = false)
        {
            columnName = dataColumn;
            db_required = required;
            this.parseType = parseType;
            dbAttrib = null;
        }

        public DBControlInfo(string dataColumn, bool required = false)
        {
            columnName = dataColumn;
            db_required = required;
            parseType = ParseType.UpdateAndDisplay;
            dbAttrib = null;
        }

        public DBControlInfo(string dataColumn, DbAttributes attribute, bool required = false)
        {
            columnName = dataColumn;
            db_required = required;
            parseType = ParseType.UpdateAndDisplay;
            dbAttrib = attribute;
        }

        public DBControlInfo(string dataColumn, DbAttributes attribute, ParseType parseType, bool required = false)
        {
            columnName = dataColumn;
            db_required = required;
            this.parseType = parseType;
            dbAttrib = attribute;
        }

        #endregion "Constructors"

        #region "Properties"

        /// <summary>
        /// Gets or sets the <see cref="DbAttributes"/> for <see cref="ComboBox"/> controls.
        /// </summary>
        /// <returns></returns>
        public DbAttributes Attributes
        {
            get { return dbAttrib; }
            set { dbAttrib = value; }
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
            get { return db_required; }
            set { db_required = value; }
        }

        #endregion "Properties"
    }
}

namespace AssetManager.Data.Functions
{
    public struct DBRemappingInfo
    {
        public string FromColumnName { get; set; }
        public string ToColumnName { get; set; }

        public DBRemappingInfo(string fromColumn, string toColumn)
        {
            FromColumnName = fromColumn;
            ToColumnName = toColumn;
        }
    }
}

namespace AssetManager.Data.Functions
{
    public class DBControlParser : IDisposable
    {
        #region "Fields"

        private Form parentForm;
        private ErrorProvider errorProvider;

        public ErrorProvider ErrorProvider
        {
            get { return errorProvider; }
        }

        #endregion "Fields"

        #region "Constructors"

        /// <summary>
        /// Instantiate new instance of <see cref="DBControlParser"/>
        /// </summary>
        /// <param name="parentForm">Form that contains controls initiated with <see cref="DBControlInfo"/> </param>
        public DBControlParser(Form parentForm)
        {
            this.parentForm = parentForm;
        }

        #endregion "Constructors"

        #region "Methods"

        /// <summary>
        /// Populates all Controls in the ParentForm that have been initiated via <see cref="DBControlInfo"/> with their corresponding column names.
        /// </summary>
        /// <param name="data">DataTable that contains the rows and columns associated with the controls.</param>
        /// <param name="remappingList">List of remapping objects for mapping between different column names.</param>
        public void FillDBFields(DataTable data, List<DBRemappingInfo> remappingList = null)
        {
            var dataRow = data.Rows[0];
            foreach (var ctl in GetDBControls(parentForm))
            {
                var dbInfo = (DBControlInfo)ctl.Tag;
                string columnName = null;

                if (remappingList != null)
                {
                    columnName = GetRemappedColumnName(dbInfo.ColumnName, remappingList);
                }
                else
                {
                    columnName = dbInfo.ColumnName;
                }

                if (dataRow.Table.Columns.Contains(columnName))
                {
                    Type ctlType = ctl.GetType();

                    if (ctlType == typeof(TextBox))
                    {
                        TextBox dbTxt = (TextBox)ctl;
                        if (dbInfo.Attributes != null)
                        {
                            dbTxt.Text = dbInfo.Attributes[(dataRow[columnName].ToString())].DisplayValue;
                        }
                        else
                        {
                            dbTxt.Text = dataRow[columnName].ToString();
                        }
                    }
                    else if (ctlType == typeof(MaskedTextBox))
                    {
                        MaskedTextBox dbMaskTxt = (MaskedTextBox)ctl;
                        dbMaskTxt.Text = dataRow[columnName].ToString();
                    }
                    else if (ctlType == typeof(DateTimePicker))
                    {
                        DateTimePicker dbDtPick = (DateTimePicker)ctl;
                        dbDtPick.Value = DateTime.Parse(dataRow[columnName].ToString());
                    }
                    else if (ctlType == typeof(ComboBox))
                    {
                        ComboBox dbCmb = (ComboBox)ctl;
                        dbCmb.SetSelectedAttribute(dbInfo.Attributes[dataRow[columnName].ToString()]);
                    }
                    else if (ctlType == typeof(Label))
                    {
                        Label dbLbl = (Label)ctl;
                        dbLbl.Text = dataRow[columnName].ToString();
                    }
                    else if (ctlType == typeof(CheckBox))
                    {
                        CheckBox dbChk = (CheckBox)ctl;
                        dbChk.Checked = Convert.ToBoolean(dataRow[columnName]);
                    }
                    else if (ctlType == typeof(RichTextBox))
                    {
                        RichTextBox dbRtb = (RichTextBox)ctl;
                        dbRtb.TextOrRtf(dataRow[columnName].ToString());
                    }
                    else
                    {
                        throw new Exception("Unexpected type.");
                    }
                }
            }
        }

        /// <summary>
        /// Get remapped column name for the specified column and remapping info. Returns new column name if a match is found in the map; otherwise, returns the original column name.
        /// </summary>
        /// <param name="columnName"></param>
        /// <param name="mappingInfo"></param>
        /// <returns></returns>
        public string GetRemappedColumnName(string columnName, List<DBRemappingInfo> mappingInfo)
        {
            if (mappingInfo.Exists(m => m.ToColumnName == columnName))
            {
                return mappingInfo.Find(m => m.ToColumnName == columnName).FromColumnName;
            }
            else
            {
                return columnName;
            }
        }

        /// <summary>
        /// Instantiates an error provider and sets all required DBControls validation events.
        /// </summary>
        public void EnableFieldValidation()
        {
            errorProvider = new ErrorProvider();
            errorProvider.BlinkStyle = ErrorBlinkStyle.NeverBlink;
            errorProvider.Icon = Properties.Resources.fieldErrorIcon_Icon;
            SetValidateEvents();
        }

        private void SetValidateEvents()
        {
            foreach (Control ctl in GetDBControls(parentForm))
            {
                var DBInfo = (DBControlInfo)ctl.Tag;
                if (DBInfo.Required)
                {
                    ctl.Validated += ControlValidateEvent;
                }
            }
        }

        private void ControlValidateEvent(object sender, EventArgs e)
        {
            if (errorProvider != null)
            {
                Control ctl = (Control)sender;
                if (ctl.Enabled)
                {
                    ValidateControl(ctl);
                }
            }
        }

        private bool ValidateControl(Control control)
        {
            var dbInfo = (DBControlInfo)control.Tag;

            if (dbInfo.Required)
            {
                if (control is ComboBox)
                {
                    ComboBox dbCmb = (ComboBox)control;
                    if (dbCmb.SelectedIndex < 0)
                    {
                        SetError(dbCmb, false);
                        return false;
                    }
                    else
                    {
                        SetError(dbCmb, true);
                        return true;
                    }
                }
                else
                {
                    // Don't validate read-only textboxes.
                    if (control is TextBox)
                    {
                        var txtBox = (TextBox)control;
                        if (txtBox.ReadOnly) return true;
                    }

                    if (string.IsNullOrEmpty(control.Text.Trim()))
                    {
                        SetError(control, false);
                        return false;
                    }
                    else
                    {
                        SetError(control, true);
                        return true;
                    }
                }
            }
            return true;
        }

        public bool ValidateFields()
        {
            bool fieldsValid = true;
            foreach (Control ctl in GetDBControls(parentForm))
            {
                if (!ValidateControl(ctl))
                {
                    fieldsValid = false;
                }
            }
            return fieldsValid;
        }

        public void SetError(Control control, bool isValid)
        {
            if (!isValid)
            {
                if (ReferenceEquals(errorProvider.GetError(control), string.Empty))
                {
                    control.BackColor = Colors.MissingField;
                    errorProvider.SetIconAlignment(control, ErrorIconAlignment.MiddleRight);
                    errorProvider.SetIconPadding(control, 4);
                    errorProvider.SetError(control, "Required or Invalid Field");
                }
            }
            else
            {
                control.BackColor = System.Drawing.Color.Empty;
                errorProvider.SetError(control, string.Empty);
            }
        }

        public void ClearErrors()
        {
            errorProvider.Clear();

            foreach (Control c in GetDBControls(parentForm))
            {
                c.BackColor = System.Drawing.Color.Empty;
            }
        }

        /// <summary>
        /// Recursively collects list of controls initiated with <see cref="DBControlInfo"/> tags within Parent control.
        /// </summary>
        /// <param name="parentControl">Parent control. Usually a Form to start from.</param>
        /// <param name="controlList">Blank List of Control to be filled.</param>
        public List<Control> GetDBControls(Control parentControl, List<Control> controlList = null)
        {
            if (controlList == null) controlList = new List<Control>();

            foreach (Control ctl in parentControl.Controls)
            {
                if (ctl.Tag is DBControlInfo)
                {
                    controlList.Add(ctl);
                }

                if (ctl.HasChildren)
                {
                    controlList.AddRange(GetDBControls(ctl));
                }
            }
            return controlList;
        }

        public object GetDBControlValue(Control dbControl)
        {
            if (dbControl is TextBox)
            {
                TextBox dbTxt = (TextBox)dbControl;
                return DataConsistency.CleanDBValue(dbTxt.Text);
            }
            else if (dbControl is MaskedTextBox)
            {
                MaskedTextBox dbMaskTxt = (MaskedTextBox)dbControl;
                return DataConsistency.CleanDBValue(dbMaskTxt.Text);
            }
            else if (dbControl is DateTimePicker)
            {
                DateTimePicker dbDtPick = (DateTimePicker)dbControl;
                return dbDtPick.Value;
            }
            else if (dbControl is ComboBox)
            {
                ComboBox dbCmb = (ComboBox)dbControl;
                if (dbCmb.SelectedIndex > -1)
                {
                    return dbCmb.SelectedValue.ToString();
                }
                else
                {
                    return dbCmb.Text;
                }
            }
            else if (dbControl is CheckBox)
            {
                CheckBox dbChk = (CheckBox)dbControl;
                return dbChk.Checked;
            }
            else
            {
                throw new Exception("Unexpected type.");
                //return null;
            }
        }

        /// <summary>
        /// Collects an EMPTY DataTable via a SQL Select statement and adds a new row for SQL Insertion.
        /// </summary>
        /// <remarks>
        /// The SQL SELECT statement should return an EMPTY table. A new row will be added to this table via <see cref="UpdateDBControlRow(DataRow)"/>
        /// </remarks>
        /// <param name="selectQry">A SQL Select query string that will return an EMPTY table. Ex: "SELECT * FROM table LIMIT 0"</param>
        /// <returns>
        /// Returns a DataTable with a new row added via <see cref="UpdateDBControlRow(DataRow)"/>
        /// </returns>
        public DataTable ReturnInsertTable(string selectQry)
        {
            DataTable tmpTable = null;
            tmpTable = DBFactory.GetDatabase().DataTableFromQueryString(selectQry);
            tmpTable.Rows.Add();
            UpdateDBControlRow(tmpTable.Rows[0]);
            return tmpTable;
        }

        /// <summary>
        /// Collects a DataTable via a SQL Select statement and modifies the topmost row with data parsed from <seealso cref="UpdateDBControlRow(DataRow)"/>
        /// </summary>
        /// <remarks>
        /// The SQL SELECT statement should return a single row only. This is will be the table row that you wish to update.
        /// </remarks>
        /// <param name="selectQry">A SQL Select query string that will return the table row that is to be updated.</param>
        /// <returns>
        /// Returns a DataTable modified by the controls identified by <see cref="GetDBControls(Control, List{Control})"/>
        /// </returns>
        public DataTable ReturnUpdateTable(string selectQry)
        {
            DataTable tmpTable = new DataTable();
            tmpTable = DBFactory.GetDatabase().DataTableFromQueryString(selectQry);
            tmpTable.TableName = "UpdateTable";
            UpdateDBControlRow(tmpTable.Rows[0]);
            return tmpTable;
        }

        /// <summary>
        /// Modifies a DataRow with data parsed from controls collected by <see cref="GetDBControlValue(Control)"/>
        /// </summary>
        /// <param name="DBRow">DataRow to be modified.</param>
        private void UpdateDBControlRow(DataRow DBRow)
        {
            foreach (Control ctl in GetDBControls(parentForm))
            {
                DBControlInfo DBInfo = (DBControlInfo)ctl.Tag;
                if (DBInfo.ParseType != ParseType.DisplayOnly)
                {
                    DBRow[DBInfo.ColumnName] = GetDBControlValue(ctl);
                }
            }
        }

        #region IDisposable Support

        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    errorProvider?.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support

        #endregion "Methods"
    }
}