using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using AssetManager.UserInterface.CustomControls;

namespace AssetManager
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

namespace AssetManager
{
    /// <summary>
    /// Instantiate and assign to <see cref="Control.Tag"/> property to enable DBParsing functions.
    /// </summary>
    public class DBControlInfo
    {
        #region "Fields"

        private AttributeDataStruct[] db_attrib_index;
        private string db_column;
        private ParseType db_parse_type;

        private bool db_required;

        #endregion "Fields"

        #region "Constructors"

        public DBControlInfo()
        {
            db_column = "";
            db_required = false;
            db_parse_type = ParseType.DisplayOnly;
            db_attrib_index = null;
        }

        public DBControlInfo(string dataColumn, ParseType parseType, bool required = false)
        {
            db_column = dataColumn;
            db_required = required;
            db_parse_type = parseType;
            db_attrib_index = null;
        }

        public DBControlInfo(string dataColumn, bool required = false)
        {
            db_column = dataColumn;
            db_required = required;
            db_parse_type = ParseType.UpdateAndDisplay;
            db_attrib_index = null;
        }

        public DBControlInfo(string dataColumn, AttributeDataStruct[] attribIndex, bool required = false)
        {
            db_column = dataColumn;
            db_required = required;
            db_parse_type = ParseType.UpdateAndDisplay;
            db_attrib_index = attribIndex;
        }

        public DBControlInfo(string dataColumn, AttributeDataStruct[] attribIndex, ParseType parseType, bool required = false)
        {
            db_column = dataColumn;
            db_required = required;
            db_parse_type = parseType;
            db_attrib_index = attribIndex;
        }

        #endregion "Constructors"

        #region "Properties"

        /// <summary>
        /// Gets or sets the <see cref="AttributeDataStruct"/> index for <see cref="ComboBox"/> controls.
        /// </summary>
        /// <returns></returns>
        public AttributeDataStruct[] AttribIndex
        {
            get { return db_attrib_index; }
            set { db_attrib_index = value; }
        }

        /// <summary>
        /// Gets or sets the Database Column used to update and/or populate the assigned control.
        /// </summary>
        /// <returns></returns>
        public string DataColumn
        {
            get { return db_column; }
            set { db_column = value; }
        }

        /// <summary>
        /// Gets or sets <seealso cref="ParseType"/>
        /// </summary>
        /// <returns></returns>
        public ParseType ParseType
        {
            get { return db_parse_type; }
            set { db_parse_type = value; }
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

namespace AssetManager
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

namespace AssetManager
{
    public class DBControlParser
    {
        #region "Fields"

        public Dictionary<string, object> InitialControlValues = new Dictionary<string, object>();
        private Form ParentForm;
        private ErrorProvider errorProvider;

        #endregion "Fields"

        #region "Constructors"

        /// <summary>
        /// Instantiate new instance of <see cref="DBControlParser"/>
        /// </summary>
        /// <param name="parentForm">Form that contains controls initiated with <see cref="DBControlInfo"/> </param>
        public DBControlParser(Form parentForm)
        {
            this.ParentForm = parentForm;
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
            InitialControlValues.Clear();
            DataRow Row = data.Rows[0];
            foreach (Control ctl in GetDBControls(ParentForm))
            {
                DBControlInfo DBInfo = (DBControlInfo)ctl.Tag;
                string DBColumn = null;

                if (remappingList != null)
                {
                    DBColumn = GetRemappedColumnName(DBInfo.DataColumn, remappingList);
                }
                else
                {
                    DBColumn = DBInfo.DataColumn;
                }

                if (Row.Table.Columns.Contains(DBColumn))
                {


                    Type ctlType = ctl.GetType();

                    if (ctlType == typeof(TextBox))
                    {
                        TextBox dbTxt = (TextBox)ctl;
                        if (DBInfo.AttribIndex != null)
                        {
                            dbTxt.Text = AttribIndexFunctions.GetDisplayValueFromCode(DBInfo.AttribIndex, Row[DBColumn].ToString());
                        }
                        else
                        {
                            dbTxt.Text = Row[DBColumn].ToString();
                        }
                        InitialControlValues.Add(DBColumn, dbTxt.Text);
                    }
                    else if (ctlType == typeof(LabeledTextBox))
                    {
                        LabeledTextBox dbTxt = (LabeledTextBox)ctl;
                        if (DBInfo.AttribIndex != null)
                        {
                            dbTxt.Text = AttribIndexFunctions.GetDisplayValueFromCode(DBInfo.AttribIndex, Row[DBColumn].ToString());
                        }
                        else
                        {
                            dbTxt.Text = Row[DBColumn].ToString();
                        }
                        InitialControlValues.Add(DBColumn, dbTxt.Text);
                    }
                    else if (ctlType == typeof(MaskedTextBox))
                    {
                        MaskedTextBox dbMaskTxt = (MaskedTextBox)ctl;
                        dbMaskTxt.Text = Row[DBColumn].ToString();
                        InitialControlValues.Add(DBColumn, dbMaskTxt.Text);
                    }
                    else if (ctlType == typeof(DateTimePicker))
                    {
                        DateTimePicker dbDtPick = (DateTimePicker)ctl;
                        dbDtPick.Value = DateTime.Parse(Row[DBColumn].ToString());
                        InitialControlValues.Add(DBColumn, dbDtPick.Value);
                    }
                    else if (ctlType == typeof(ComboBox))
                    {
                        ComboBox dbCmb = (ComboBox)ctl;
                        dbCmb.SelectedIndex = AttribIndexFunctions.GetComboIndexFromCode(DBInfo.AttribIndex, Row[DBColumn].ToString());
                        InitialControlValues.Add(DBColumn, Row[DBColumn].ToString());
                    }
                    else if (ctlType == typeof(LabeledComboBox))
                    {
                        LabeledComboBox dbCmb = (LabeledComboBox)ctl;
                        dbCmb.SelectedIndex = AttribIndexFunctions.GetComboIndexFromCode(DBInfo.AttribIndex, Row[DBColumn].ToString());
                        InitialControlValues.Add(DBColumn, Row[DBColumn].ToString());
                    }
                    else if (ctlType == typeof(Label))
                    {
                        Label dbLbl = (Label)ctl;
                        dbLbl.Text = Row[DBColumn].ToString();
                        InitialControlValues.Add(DBColumn, dbLbl.Text);
                    }
                    else if (ctlType == typeof(CheckBox))
                    {
                        CheckBox dbChk = (CheckBox)ctl;
                        dbChk.Checked = Convert.ToBoolean(Row[DBColumn]);
                        InitialControlValues.Add(DBColumn, dbChk.Checked);
                    }
                    else if (ctlType == typeof(RichTextBox))
                    {
                        RichTextBox dbRtb = (RichTextBox)ctl;
                        OtherFunctions.SetRichTextBox(dbRtb, Row[DBColumn].ToString());
                        InitialControlValues.Add(DBColumn, Row[DBColumn].ToString());
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

        public void EnableFieldValidation(ErrorProvider errorProvider)
        {
            this.errorProvider = errorProvider;
            SetValidateEvents();
        }


        private void SetValidateEvents()
        {
            foreach (Control ctl in GetDBControls(ParentForm))
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
                ValidateFields(errorProvider);
            }

        }

        public bool ControlValueHasChanged(string columnName)
        {
            foreach (Control ctl in GetDBControls(ParentForm))
            {
                var DBInfo = (DBControlInfo)ctl.Tag;
                if (DBInfo.DataColumn == columnName)
                {
                    if (GetDBControlValue(ctl) != InitialControlValues[columnName])
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            return false;
        }

        public List<string> GetChangedColumns()
        {
            var changedColumns = new List<string>();
            foreach (Control ctl in GetDBControls(ParentForm))
            {
                var DBInfo = (DBControlInfo)ctl.Tag;
                Console.WriteLine(ctl.Name + " - " + GetDBControlValue(ctl).ToString() + " - " + InitialControlValues[DBInfo.DataColumn]);
                if (GetDBControlValue(ctl).ToString() != InitialControlValues[DBInfo.DataColumn].ToString())
                {
                    changedColumns.Add(DBInfo.DataColumn);
                }
            }
            return changedColumns;
        }

        public bool ValidateFields(ErrorProvider errorProvider)
        {
            bool fieldsValid = true;
            foreach (Control ctl in GetDBControls(ParentForm))
            {
                var DBInfo = (DBControlInfo)ctl.Tag;

                if (DBInfo.Required)
                {
                    //if (ctl is TextBox || ctl is MaskedTextBox || ctl is LabeledTextBox)
                    //{
                    //    if (string.IsNullOrEmpty(ctl.Text.Trim()))
                    //    {
                    //        SetErrorIcon(errorProvider, ctl, false);
                    //    }
                    //    else
                    //    {
                    //        SetErrorIcon(errorProvider, ctl, true);
                    //    }
                    //}
                    //else if (ctl is ComboBox)
                    if (ctl is ComboBox)
                    {
                        ComboBox dbCmb = (ComboBox)ctl;
                        if (dbCmb.SelectedIndex < 0)
                        {
                            SetErrorIcon(errorProvider, dbCmb, false);
                            fieldsValid = false;
                        }
                        else
                        {
                            SetErrorIcon(errorProvider, dbCmb, true);
                        }
                    }
                    else if (ctl is LabeledComboBox)
                    {
                        var LblCmb = (LabeledComboBox)ctl;
                        var dbCmb = LblCmb.ComboBox;
                        if (dbCmb.SelectedIndex < 0)
                        {
                            SetErrorIcon(errorProvider, LblCmb, false);
                            fieldsValid = false;
                        }
                        else
                        {
                            SetErrorIcon(errorProvider, LblCmb, true);
                        }

                    }
                    else
                    {
                        if (string.IsNullOrEmpty(ctl.Text.Trim()))
                        {
                            SetErrorIcon(errorProvider, ctl, false);
                            fieldsValid = false;
                        }
                        else
                        {
                            SetErrorIcon(errorProvider, ctl, true);
                        }

                        // throw new Exception("Unexpected type.");
                        //return null;
                    }
                }

                //else if (ctl is CheckBox)
                //{
                //    CheckBox dbChk = (CheckBox)ctl;

                //}
                //else if (ctl is MaskedTextBox)
                //{
                //    MaskedTextBox dbMaskTxt = (MaskedTextBox)ctl;

                //}
                //else if (ctl is DateTimePicker)
                //{
                //    DateTimePicker dbDtPick = (DateTimePicker)ctl;
                //}

            }
            return fieldsValid;
        }

        private void SetErrorIcon(ErrorProvider errorProvider, Control control, bool isValid)
        {
            if (!isValid)
            {
                if (ReferenceEquals(errorProvider.GetError(control), string.Empty))
                {
                    errorProvider.SetIconAlignment(control, ErrorIconAlignment.MiddleRight);
                    errorProvider.SetIconPadding(control, 4);
                    errorProvider.SetError(control, "Required or Invalid Field");
                }
            }
            else
            {
                errorProvider.SetError(control, string.Empty);
            }
        }

        /// <summary>
        /// Recursively collects list of controls initiated with <see cref="DBControlInfo"/> tags within Parent control.
        /// </summary>
        /// <param name="parentForm">Parent control. Usually a Form to being.</param>
        /// <param name="controlList">Blank List of Control to be filled.</param>
        public List<Control> GetDBControls(Control parentForm, List<Control> controlList = null)
        {
            if (controlList == null)
                controlList = new List<Control>();
            foreach (Control ctl in parentForm.Controls)
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
            var DBInfo = (DBControlInfo)dbControl.Tag;

            if (dbControl is TextBox || dbControl is LabeledTextBox)
            {
                //TextBox dbTxt = (TextBox)dbControl;
                //return DataConsistency.CleanDBValue(dbTxt.Text);
                return DataConsistency.CleanDBValue(dbControl.Text);
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
                return AttribIndexFunctions.GetDBValue(DBInfo.AttribIndex, dbCmb.SelectedIndex);
            }
            else if (dbControl is LabeledComboBox)
            {
                var lblCmb = (LabeledComboBox)dbControl;
                return AttribIndexFunctions.GetDBValue(DBInfo.AttribIndex, lblCmb.SelectedIndex);
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
            tmpTable = AssetManager.DBFactory.GetDatabase().DataTableFromQueryString(selectQry);
            tmpTable.Rows.Add();
            UpdateDBControlRow(tmpTable.Rows[0]);
            return tmpTable;
        }

        /// <summary>
        /// Collects an EMPTY DataTable via a SQL Select statement and adds a new row for SQL Insertion.
        /// </summary>
        /// <remarks>
        /// The SQL SELECT statement should return an EMPTY table. A new row will be added to this table via <see cref="UpdateDBControlRow(DataRow)"/>
        /// </remarks>
        /// <param name="selectQry">A SQL Select query string that will return an EMPTY table. Ex: "SELECT * FROM table LIMIT 0"</param>
        /// <param name="addlValues">A <see cref="Dictionary{TKey, TValue}"/> that contains additional column names (TKey) and values (TValue) to be added to the table. </param>
        /// <returns>
        /// Returns a DataTable with a new row added via <see cref="UpdateDBControlRow(DataRow)"/>
        /// </returns>
        public DataTable ReturnInsertTable(string selectQry, Dictionary<string, object> addlValues)
        {
            DataTable tmpTable = null;
            tmpTable = AssetManager.DBFactory.GetDatabase().DataTableFromQueryString(selectQry);
            tmpTable.Rows.Add();
            var DBRow = tmpTable.Rows[0];
            UpdateDBControlRow(DBRow);
            foreach (var item in addlValues)
            {
                DBRow[item.Key] = item.Value;
            }
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
            tmpTable = AssetManager.DBFactory.GetDatabase().DataTableFromQueryString(selectQry);
            tmpTable.TableName = "UpdateTable";
            UpdateDBControlRow(tmpTable.Rows[0]);
            return tmpTable;
        }

        public DataTable ReturnUpdateTable(string selectQry, Dictionary<string, object> addlValues)
        {
            DataTable tmpTable = new DataTable();
            tmpTable = AssetManager.DBFactory.GetDatabase().DataTableFromQueryString(selectQry);
            tmpTable.TableName = "UpdateTable";
            var DBRow = tmpTable.Rows[0];
            UpdateDBControlRow(tmpTable.Rows[0]);
            foreach (var item in addlValues)
            {
                DBRow[item.Key] = item.Value;
            }
            return tmpTable;
        }

        /// <summary>
        /// Modifies a DataRow with data parsed from controls collected by <see cref="GetDBControls(Control, List{Control})"/>
        /// </summary>
        /// <param name="DBRow">DataRow to be modified.</param>
        private void UpdateDBControlRow(DataRow DBRow)
        {
            foreach (Control ctl in GetDBControls(ParentForm))
            {
                DBControlInfo DBInfo = (DBControlInfo)ctl.Tag;
                if (DBInfo.ParseType != ParseType.DisplayOnly)
                {
                    DBRow[DBInfo.DataColumn] = GetDBControlValue(ctl);
                }
            }
        }

        #endregion "Methods"
    }
}