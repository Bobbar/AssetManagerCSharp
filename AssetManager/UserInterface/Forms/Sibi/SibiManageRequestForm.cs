using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AssetManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiManageRequestForm : ExtendedForm
    {
        #region Fields

        private DBControlParser controlParser;
        private string currentHash;
        private SibiRequest currentRequest = new SibiRequest();
        private bool isDragging = false;
        private bool isGridFilling = false;
        private bool isModifying = false;
        private bool isNewRequest = false;
        private Point mouseStartPos;
        private MunisToolBar munisToolBar;
        private FormWindowState prevWindowState;
        private SliderLabel statusSlider;
        private string titleText = "Manage Request";
        private WindowList windowList;
        #endregion Fields

        #region Constructors

        public SibiManageRequestForm(ExtendedForm parentForm, string requestGuid) : base(parentForm, requestGuid)
        {
            munisToolBar = new MunisToolBar(this);
            windowList = new WindowList(this);

            InitializeComponent();
            InitForm();

            OpenRequest(requestGuid);
        }

        public SibiManageRequestForm(ExtendedForm parentForm) : base(parentForm)
        {
            munisToolBar = new MunisToolBar(this);
            windowList = new WindowList(this);

            InitializeComponent();
            InitForm();
            Text += " - *New Request*";
            NewRequest();
        }

        #endregion Constructors

        #region Methods

        public void ClearAttachCount()
        {
            AttachmentsMenuButton.Text = "(0)";
            AttachmentsMenuButton.ToolTipText = "Attachments " + AttachmentsMenuButton.Text;
        }

        public override bool OkToClose()
        {
            bool canClose = true;
            if (isModifying && !CancelModify())
            {
                canClose = false;
            }
            return canClose;
        }

        public override void RefreshData()
        {
            OpenRequest(currentRequest.Guid);
        }

        public void UpdateAttachCountHandler(object sender, EventArgs e)
        {
            AssetManagerFunctions.SetAttachmentCount(AttachmentsMenuButton, currentRequest.Guid, new SibiAttachmentsCols());
        }

        private void AcceptChanges()
        {
            RequestItemsGrid.EndEdit();
            if (!ValidateFields())
            {
                return;
            }
            DisableControls();
            ToolStrip.BackColor = Colors.SibiToolBarColor;
            HideEditControls();
            UpdateRequest();
            isModifying = false;
        }

        private bool AddNewNote(string requestGuid, string noteValue)
        {
            string noteGuid = Guid.NewGuid().ToString();
            try
            {
                ParamCollection noteParams = new ParamCollection();
                noteParams.Add(SibiNotesCols.RequestGuid, requestGuid);
                noteParams.Add(SibiNotesCols.NoteGuid, noteGuid);
                noteParams.Add(SibiNotesCols.Note, noteValue);
                if (DBFactory.GetDatabase().InsertFromParameters(SibiNotesCols.TableName, noteParams.Parameters) > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        private void AddNewRequest()
        {
            SecurityTools.CheckForAccess(SecurityGroups.AddSibi);

            if (!ValidateFields())
            {
                return;
            }
            SibiRequest requestData = GetRequestItems();
            using (var trans = DBFactory.GetDatabase().StartTransaction())
            using (var conn = trans.Connection)
            {
                try
                {
                    string insertRequestQry = Queries.SelectEmptySibiRequestTable;
                    string insertRequestItemsQry = Queries.SelectEmptySibiItemsTable(GridColumnFunctions.ColumnsString(RequestItemsColumns()));
                    DBFactory.GetDatabase().UpdateTable(insertRequestQry, GetInsertTable(insertRequestQry, currentRequest.Guid), trans);
                    DBFactory.GetDatabase().UpdateTable(insertRequestItemsQry, requestData.RequestItems, trans);
                    CreatePanel.Visible = false;
                    trans.Commit();
                    isModifying = false;
                    isNewRequest = false;
                    ParentForm.RefreshData();
                    this.RefreshData();
                    OtherFunctions.Message("New Request Added.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Complete", this);
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
        }

        private void AllowDragChanged()
        {
            if (AllowDragCheckBox.Checked)
            {
                RequestItemsGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                RequestItemsGrid.MultiSelect = false;
            }
            else
            {
                RequestItemsGrid.SelectionMode = DataGridViewSelectionMode.CellSelect;
                RequestItemsGrid.MultiSelect = true;
            }
        }
        private void BeginDragDrop(Point mouseLocation)
        {
            if (RequestItemsGrid.SelectedRows.Count > 0)
            {
                if (AllowDragCheckBox.Checked && !isDragging)
                {
                    if (MouseIsDragging(mouseLocation))
                    {
                        isDragging = true;
                        RequestItemsGrid.DoDragDrop(RequestItemsGrid.SelectedRows[0], DragDropEffects.All);
                        isDragging = false;
                    }
                }
            }
        }

        private bool CancelModify()
        {
            if (isModifying)
            {
                if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
                this.Activate();
                var blah = OtherFunctions.Message("Are you sure you want to discard all changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Discard Changes?", this);
                if (blah == DialogResult.Yes)
                {
                    if (isNewRequest)
                    {
                        isModifying = false;
                        return true;
                    }
                    else
                    {
                        RefreshData();
                        return true;
                    }
                }
            }
            return false;
        }

        private void CellSelected(int columnIndex, int rowIndex)
        {
            if (columnIndex >= -1 && rowIndex >= 0)
            {
                int ColIndex = (columnIndex == -1 ? 0 : columnIndex);
                if (!RequestItemsGrid[ColIndex, rowIndex].Selected)
                {
                    RequestItemsGrid.Rows[rowIndex].Selected = true;
                    RequestItemsGrid.CurrentCell = RequestItemsGrid[ColIndex, rowIndex];
                }
                SetToolStripItems();
            }
        }

        private async void CheckForPO()
        {
            if (!string.IsNullOrEmpty(currentRequest.RequisitionNumber) && string.IsNullOrEmpty(currentRequest.PO))
            {
                string po = await MunisFunctions.GetPOFromReqNumberAsync(currentRequest.RequisitionNumber, currentRequest.NeedByDate.Year.ToString());
                if (!string.IsNullOrEmpty(po))
                {
                    var blah = OtherFunctions.Message("PO Number " + po + " was detected in the Requisition. Do you wish to add it to this request?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "New PO Detected", this);
                    if (blah == DialogResult.Yes)
                    {
                        InsertPONumber(po);
                        OpenRequest(currentRequest.Guid);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void ClearAll()
        {
            this.SuspendLayout();
            HideEditControls();
            NotesGrid.DataSource = null;
            CreatePanel.Visible = false;
            currentRequest.Dispose();
            currentRequest = null;
            DisableControls();
            ToolStrip.BackColor = Colors.SibiToolBarColor;
            isModifying = false;
            isNewRequest = false;
            controlParser.ClearErrors();
            ClearAttachCount();
            SetMunisStatus();
            this.ResumeLayout();
        }

        private void ClearControls(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    var txt = (TextBox)c;
                    txt.Clear();
                }
                else if (c is ComboBox)
                {
                    var cmb = (ComboBox)c;
                    cmb.SelectedIndex = -1;
                    cmb.Text = string.Empty;
                }
                else if (c is DateTimePicker)
                {
                    var dtp = (DateTimePicker)c;
                    dtp.Value = DateTime.Now;
                }
                else if (c is CheckBox)
                {
                    var chk = (CheckBox)c;
                    chk.Checked = false;
                }
                if (c.HasChildren)
                {
                    ClearControls(c);
                }
            }
        }

        private void CollectRequestInfo(DataTable requestResults, DataTable itemsResults)
        {
            try
            {
                currentRequest = new SibiRequest(requestResults);
                currentRequest.RequestItems = itemsResults;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private bool ConcurrencyCheck()
        {
            using (var requestTable = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestsByGuid(currentRequest.Guid)))
            using (var itemTable = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestItems(GridColumnFunctions.ColumnsString(RequestItemsColumns()), currentRequest.Guid)))
            {
                requestTable.TableName = SibiRequestCols.TableName;
                itemTable.TableName = SibiRequestItemsCols.TableName;
                string dbHash = GetHash(requestTable, itemTable);
                if (dbHash != currentHash)
                {
                    return false;
                }
                return true;
            }
        }

        private void DeleteCurrentNote()
        {
            SecurityTools.CheckForAccess(SecurityGroups.ModifySibi);

            string noteGuid = NotesGrid.CurrentRowStringValue(SibiNotesCols.NoteGuid);

            if (!string.IsNullOrEmpty(noteGuid))
            {
                var blah = OtherFunctions.Message("Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Delete Note", this);
                if (blah == DialogResult.Yes)
                {
                    if (DeleteNote(noteGuid))
                    {
                        statusSlider.NewSlideMessage("Note deleted successfully!");
                        OpenRequest(currentRequest.Guid);
                    }
                }
            }
        }

        private void DeleteCurrentSibiReqest()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.DeleteSibi);

                if (ReferenceEquals(currentRequest.RequestItems, null))
                {
                    return;
                }
                var blah = OtherFunctions.Message("Are you absolutely sure?  This cannot be undone and will delete all data including attachments.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, "WARNING", this);
                if (blah == DialogResult.Yes)
                {
                    OtherFunctions.SetWaitCursor(true, this);
                    if (AssetManagerFunctions.DeleteSibiRequest(currentRequest.Guid))
                    {
                        OtherFunctions.Message("Sibi Request deleted successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Device Deleted", this);
                        currentRequest = null;
                        ParentForm.RefreshData();
                        this.Dispose();
                    }
                    else
                    {
                        Logging.Logger("*****DELETION ERROR******: " + currentRequest.Guid);
                        OtherFunctions.Message("Failed to delete request successfully!  Please let Bobby Lovell know about this.", MessageBoxButtons.OK, MessageBoxIcon.Error, "Delete Failed", this);
                        currentRequest = null;
                        this.Dispose();
                    }
                }
                else
                {
                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private bool DeleteItemFromLocal(int rowIndex)
        {
            try
            {
                if (!RequestItemsGrid.Rows[rowIndex].IsNewRow)
                {
                    RequestItemsGrid.Rows.Remove(RequestItemsGrid.Rows[rowIndex]);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        private bool DeleteNote(string noteGuid)
        {
            try
            {
                int rowsAffected = DBFactory.GetDatabase().ExecuteNonQuery(Queries.DeleteSibiNote(noteGuid));
                if (rowsAffected > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        private void DeleteSelectedRequestItem()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.ModifySibi);

                var blah = OtherFunctions.Message("Delete selected row?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Delete Item Row", this);
                if (blah == DialogResult.Yes)
                {
                    if (!DeleteItemFromLocal(RequestItemsGrid.CurrentRow.Index))
                    {
                        blah = OtherFunctions.Message("Failed to delete row.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
                    }
                }
                else
                {
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void DisableControls()
        {
            DisableControlsRecursive(this);
            DisableGrid();
        }

        private void DisableControlsRecursive(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    var txt = (TextBox)c;
                    txt.ReadOnly = true;
                }
                else if (c is ComboBox)
                {
                    var cmb = (ComboBox)c;
                    cmb.Enabled = false;
                }
                else if (c is DateTimePicker)
                {
                    var dtp = (DateTimePicker)c;
                    dtp.Enabled = false;
                }
                else if (c is CheckBox)
                {
                    if (c != AllowDragCheckBox)
                    {
                        c.Enabled = false;
                    }
                }

                if (c.HasChildren)
                {
                    DisableControlsRecursive(c);
                }
            }
        }

        private void DisableGrid()
        {
            RequestItemsGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            RequestItemsGrid.AllowUserToAddRows = false;
            RequestItemsGrid.MultiSelect = true;
        }

        private void EnableControls()
        {
            EnableControlsRecursive(this);
            EnableGrid();
        }

        private void EnableControlsRecursive(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    var txt = (TextBox)c;
                    if (txt != RequestNumTextBox && txt != CreateDateTextBox)
                    {
                        txt.ReadOnly = false;
                    }
                }
                else if (c is ComboBox)
                {
                    var cmb = (ComboBox)c;
                    cmb.Enabled = true;
                }
                else if (c is DateTimePicker)
                {
                    var dtp = (DateTimePicker)c;
                    dtp.Enabled = true;
                }
                else if (c is CheckBox)
                {
                    c.Enabled = true;
                }

                if (c.HasChildren)
                {
                    EnableControlsRecursive(c);
                }
            }
        }

        private void EnableGrid()
        {
            RequestItemsGrid.EditMode = DataGridViewEditMode.EditOnEnter;
            RequestItemsGrid.AllowUserToAddRows = true;
            RequestItemsGrid.MultiSelect = false;
            RequestItemsGrid.FastAutoSizeColumns();
        }

        private void FillCombos()
        {
            StatusComboBox.FillComboBox(Attributes.SibiAttribute.StatusType);
            TypeComboBox.FillComboBox(Attributes.SibiAttribute.RequestType);
        }

        private string GetHash(DataTable requestTable, DataTable itemsTable)
        {
            string requestHash = SecurityTools.GetSHAOfTable(requestTable);
            string itemHash = SecurityTools.GetSHAOfTable(itemsTable);
            return requestHash + itemHash;
        }

        private DataTable GetInsertTable(string selectQuery, string requestGuid)
        {
            try
            {
                var tmpTable = controlParser.ReturnInsertTable(selectQuery);
                var row = tmpTable.Rows[0];
                //Add Add'l info
                row[SibiRequestCols.Guid] = requestGuid;
                return tmpTable;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return null;
            }
        }

        private SibiRequest GetRequestItems()
        {
            RequestItemsGrid.EndEdit();

            SibiRequest request = new SibiRequest();
            request.RequestItems = (DataTable)RequestItemsGrid.DataSource;
            request.Guid = currentRequest.Guid;

            MarkupRequestItems(request.RequestItems);

            return request;
        }

        private DataTable GetUpdateTable(string selectQuery)
        {
            try
            {
                var tmpTable = controlParser.ReturnUpdateTable(selectQuery);
                //Add Add'l info
                return tmpTable;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return null;
            }
        }

        private void HideEditControls()
        {
            EditButtonsPanel.Visible = false;
        }

        private void HighlightCurrentRow(int rowIndex)
        {
            try
            {
                if (!isGridFilling)
                {
                    StyleFunctions.HighlightRow(RequestItemsGrid, GridTheme, rowIndex);
                }
            }
            catch
            {
            }
        }

        private void ImportNewDevice()
        {
            SecurityTools.CheckForAccess(SecurityGroups.AddDevice);

            var newDev = new NewDeviceForm(this);
            newDev.ImportFromSibi(RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.ItemGuid));
        }

        private void InitDBControls()
        {
            DescriptionTextBox.SetDBInfo(SibiRequestCols.Description, true);
            RequestUserTextBox.SetDBInfo(SibiRequestCols.RequestUser, true);
            TypeComboBox.SetDBInfo(SibiRequestCols.Type, Attributes.SibiAttribute.RequestType, true);
            NeedByDatePicker.SetDBInfo(SibiRequestCols.NeedBy, true);
            StatusComboBox.SetDBInfo(SibiRequestCols.Status, Attributes.SibiAttribute.StatusType, true);
            POTextBox.SetDBInfo(SibiRequestCols.PO, false);
            ReqNumberTextBox.SetDBInfo(SibiRequestCols.RequisitionNumber, false);
            RequestNumTextBox.SetDBInfo(SibiRequestCols.RequestNumber, ParseType.DisplayOnly, false);
            RTNumberTextBox.SetDBInfo(SibiRequestCols.RTNumber, false);
            CreateDateTextBox.SetDBInfo(SibiRequestCols.DateStamp, ParseType.DisplayOnly, false);
        }

        private void InitForm()
        {
            statusSlider = new SliderLabel();
            StatusStrip1.Items.Insert(0, statusSlider.ToToolStripControl(StatusStrip1));

            InitDBControls();

            FillCombos();

            controlParser = new DBControlParser(this);
            controlParser.EnableFieldValidation();

            RequestItemsGrid.DoubleBufferedDataGrid(true);
            NotesGrid.DoubleBufferedDataGrid(true);
            munisToolBar.InsertMunisDropDown(ToolStrip);
            windowList.InsertWindowList(ToolStrip);
            StyleFunctions.SetGridStyle(RequestItemsGrid, GridTheme);
            StyleFunctions.SetGridStyle(NotesGrid, GridTheme);
            ToolStrip.BackColor = Colors.SibiToolBarColor;
        }

        private void InsertPONumber(string po)
        {
            try
            {
                AssetManagerFunctions.UpdateSqlValue(SibiRequestCols.TableName, SibiRequestCols.PO, po, SibiRequestCols.Guid, currentRequest.Guid);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private bool IsRightClickColumn()
        {
            try
            {
                var colIndex = RequestItemsGrid.CurrentCell.ColumnIndex;
                if (colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.ReplaceAsset))
                {
                    return true;
                }
                else if (colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.ReplaceSerial))
                {
                    return true;
                }
                else if (colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.NewAsset))
                {
                    return true;
                }
                else if (colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.NewSerial))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                return false;
            }
        }

        private void LoadNotes(string requestGuid)
        {
            NotesGrid.SuspendLayout();
            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiNotes(requestGuid)))
            {
                NotesGrid.Populate(results, NotesGridColumns());
            }
            NotesGrid.FastAutoSizeColumns();
            NotesGrid.ClearSelection();
            NotesGrid.ResumeLayout();
        }

        private void LookupDevice()
        {
            try
            {
                int colIndex = RequestItemsGrid.CurrentCell.ColumnIndex;
                if (colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.ReplaceAsset) || colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.NewAsset))
                {
                    ChildFormControl.LookupDevice(this, AssetManagerFunctions.FindDeviceFromAssetOrSerial(RequestItemsGrid[colIndex, RequestItemsGrid.CurrentRow.Index].Value.ToString(), AssetManagerFunctions.FindDevType.AssetTag));
                }
                else if (colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.ReplaceSerial) || colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.NewSerial))
                {
                    ChildFormControl.LookupDevice(this, AssetManagerFunctions.FindDeviceFromAssetOrSerial(RequestItemsGrid[colIndex, RequestItemsGrid.CurrentRow.Index].Value.ToString(), AssetManagerFunctions.FindDevType.Serial));
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        /// <summary>
        /// Cleans up the request items data in prep for DB insertion.
        /// </summary>
        /// <param name="itemsData"></param>
        private void MarkupRequestItems(DataTable itemsData)
        {
            foreach (DataRow row in itemsData.Rows)
            {
                foreach (DataColumn col in itemsData.Columns)
                {
                    // If the row has been modified.
                    if (row.RowState != DataRowState.Unchanged && row.RowState != DataRowState.Deleted)
                    {
                        // Add the request Guid to the row if needed.
                        if (col.ColumnName == SibiRequestItemsCols.RequestGuid)
                        {
                            if (row[col] == null || string.IsNullOrEmpty(row[col].ToString()))
                            {
                                row[SibiRequestItemsCols.RequestGuid] = currentRequest.Guid;
                            }
                        }

                        if (row[col] != null)
                        {
                            // If the cell is empty, set it to DBNull.
                            if (string.IsNullOrEmpty(row[col].ToString()))
                            {
                                row[col] = DBNull.Value;
                            }
                            else
                            {
                                // Otherwise, trim the cell text if it's a string type column.
                                if (row[col] is string)
                                {
                                    row[col] = row[col].ToString().Trim();
                                }
                            }
                        }

                        // Add the modified time and user.
                        if (row.RowState == DataRowState.Added || row.RowState == DataRowState.Modified)
                        {
                            row[SibiRequestItemsCols.ModifiedBy] = NetworkInfo.LocalDomainUser;
                            row[SibiRequestItemsCols.ModifiedDate] = DateTime.Now;
                        }
                    }
                }
            }
        }

        private void ModifyRequest()
        {
            SecurityTools.CheckForAccess(SecurityGroups.ModifySibi);

            if (!string.IsNullOrEmpty(currentRequest.Guid) && !isModifying)
            {
                SetModifyMode();
            }
        }

        private bool MouseIsDragging(Point mousePosition)
        {
            int mouseMoveThreshold = 50;
            var distanceMoved = Math.Sqrt(Math.Pow((mouseStartPos.X - mousePosition.X), 2) + Math.Pow((mouseStartPos.Y - mousePosition.Y), 2));
            if (System.Convert.ToInt32(distanceMoved) > mouseMoveThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void NewNote()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.ModifySibi);

                if (!string.IsNullOrEmpty(currentRequest.Guid) && !isNewRequest)
                {
                    using (var newNote = new SibiNotesForm(this, currentRequest))
                    {
                        if (newNote.DialogResult == DialogResult.OK)
                        {
                            AddNewNote(newNote.Request.Guid, newNote.Note);
                            LoadNotes(currentRequest.Guid);
                        }
                    }
                }
                else
                {
                    if (isNewRequest)
                    {
                        OtherFunctions.Message("You must create a new request before adding notes.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Error", this);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void NewRequest()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.AddSibi);

                OtherFunctions.SetWaitCursor(true, this);

                if (isModifying)
                {
                    var blah = OtherFunctions.Message("All current changes will be lost. Are you sure you want to start a new request?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, "Create New Request", this);
                    if (blah != DialogResult.OK)
                    {
                        return;
                    }
                }
                ClearAll();
                ClearControls(this);
                isNewRequest = true;
                SetTitle(true);
                currentRequest = new SibiRequest();
                this.FormGuid = currentRequest.Guid;
                isModifying = true;
                //Set the datasource to a new empty DB table.
                var EmptyTable = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectEmptySibiItemsTable(GridColumnFunctions.ColumnsString(RequestItemsColumns())));
                RequestItemsGrid.Populate(EmptyTable, RequestItemsColumns());
                EnableControls();
                CreatePanel.Visible = true;
                this.Show();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                this.Dispose();
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private List<GridColumnAttrib> NotesGridColumns()
        {
            List<GridColumnAttrib> ColList = new List<GridColumnAttrib>();
            ColList.Add(new GridColumnAttrib(SibiNotesCols.Note, "Note", ColumnFormatType.NotePreview));
            ColList.Add(new GridColumnAttrib(SibiNotesCols.DateStamp, "Date Stamp"));
            ColList.Add(new GridColumnAttrib(SibiNotesCols.NoteGuid, "Guid", true, false));
            return ColList;
        }

        private void OpenRequest(string requestGuid)
        {
            OtherFunctions.SetWaitCursor(true, this);
            try
            {
                using (DataTable requestResults = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestsByGuid(requestGuid)))
                using (DataTable itemsResults = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestItems(GridColumnFunctions.ColumnsString(RequestItemsColumns()), requestGuid)))
                {
                    requestResults.TableName = SibiRequestCols.TableName;
                    itemsResults.TableName = SibiRequestItemsCols.TableName;
                    currentHash = GetHash(requestResults, itemsResults);
                    ClearAll();
                    CollectRequestInfo(requestResults, itemsResults);
                    controlParser.FillDBFields(requestResults);
                    SendToGrid(itemsResults);
                    LoadNotes(currentRequest.Guid);
                    SetTitle(false);
                    UpdateAttachCountHandler(this, new EventArgs());
                    this.Show();
                    this.Activate();
                    SetMunisStatus();
                    isGridFilling = false;
                }
            }
            catch (Exception ex)
            {
                OtherFunctions.Message("An error occurred while opening the request. It may have been deleted.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                this.Dispose();
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private void PopulateCurrentFAItem()
        {
            SecurityTools.CheckForAccess(SecurityGroups.ModifySibi);

            try
            {
                if (IsRightClickColumn())
                {
                    PopulateFromFA(RequestItemsGrid.CurrentCell.OwningColumn.Name);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private async void PopulateFromFA(string columnName)
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                string itemGuid = RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.ItemGuid);
                string updateValue = string.Empty;

                if (columnName == SibiRequestItemsCols.NewSerial)
                {
                    updateValue = await MunisFunctions.GetSerialFromAsset(RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.NewAsset));
                }
                else if (columnName == SibiRequestItemsCols.NewAsset)
                {
                    updateValue = await MunisFunctions.GetAssetFromSerial(RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.NewSerial));
                }
                else if (columnName == SibiRequestItemsCols.ReplaceSerial)
                {
                    updateValue = await MunisFunctions.GetSerialFromAsset(RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.ReplaceAsset));
                }
                else if (columnName == SibiRequestItemsCols.ReplaceAsset)
                {
                    updateValue = await MunisFunctions.GetAssetFromSerial(RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.ReplaceSerial));
                }

                if (!string.IsNullOrEmpty(updateValue))
                {
                    AssetManagerFunctions.UpdateSqlValue(SibiRequestItemsCols.TableName, columnName, updateValue, SibiRequestItemsCols.ItemGuid, itemGuid);
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private void ProcessDragDrop(IDataObject dropDataObject)
        {
            // Drag - drop rows from other data grids, adding new Guids and the current FKey (RequestGuid).
            // This was a tough nut to crack. In the end I just ended up building an item array and adding it to the receiving grids datasource.
            try
            {
                if (isModifying)
                {
                    var dropRow = dropDataObject.GetData(typeof(DataGridViewRow)) as DataGridViewRow; //Cast the DGVRow

                    if (dropRow != null && dropRow.DataBoundItem != null)
                    {
                        var newDataRow = ((DataRowView)dropRow.DataBoundItem).Row; //Get the databound row
                        List<object> itemArray = new List<object>();
                        foreach (DataColumn col in newDataRow.Table.Columns) //Iterate through columns and build a new item list
                        {
                            if (col.ColumnName == SibiRequestItemsCols.ItemGuid)
                            {
                                itemArray.Add(Guid.NewGuid().ToString());
                            }
                            else if (col.ColumnName == SibiRequestItemsCols.RequestGuid)
                            {
                                itemArray.Add(currentRequest.Guid);
                            }
                            else
                            {
                                itemArray.Add(newDataRow[col]);
                            }
                        }
                        ((DataTable)RequestItemsGrid.DataSource).Rows.Add(itemArray.ToArray()); //Add the item list as an array
                    }
                    isDragging = false;
                }
                else
                {
                    if (!isDragging)
                    {
                        OtherFunctions.Message("You must be modifying this request before you can drag-drop rows from another request.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Not Allowed", this);
                    }
                }
            }
            catch (InvalidCastException)
            {
                // InvalidCastException expected when drag/drop is performed from another instance of this application.
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private List<GridColumnAttrib> RequestItemsColumns()
        {
            List<GridColumnAttrib> ColList = new List<GridColumnAttrib>();
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.User, "User"));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Description, "Description"));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Qty, "Qty"));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Location, "Location", Attributes.DeviceAttribute.Locations, ColumnFormatType.AttributeCombo));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Status, "Status", Attributes.SibiAttribute.ItemStatusType, ColumnFormatType.AttributeCombo));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ReplaceAsset, "Replace Asset"));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ReplaceSerial, "Replace Serial"));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.NewAsset, "New Asset"));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.NewSerial, "New Serial"));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.OrgCode, "Org Code"));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ObjectCode, "Object Code"));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Timestamp, "Created", true, true));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ModifiedDate, "Modified", true, true));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ModifiedBy, "Modified By", true, true));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ItemGuid, "Item Guid", true, true));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.RequestGuid, "Request Guid", true, false));

            return ColList;
        }

        private void SendToGrid(DataTable results)
        {
            isGridFilling = true;
            RequestItemsGrid.SuspendLayout();
            RequestItemsGrid.Populate(results, RequestItemsColumns(), true);
            RequestItemsGrid.ClearSelection();
            RequestItemsGrid.FastAutoSizeColumns();
            RequestItemsGrid.ResumeLayout();
        }

        private void SetGLBudgetContextMenu()
        {
            if (RequestItemsGrid.CurrentCell != null)
            {
                if (RequestItemsGrid.CurrentCell.OwningColumn.Name == SibiRequestItemsCols.ObjectCode || RequestItemsGrid.CurrentCell.OwningColumn.Name == SibiRequestItemsCols.OrgCode)
                {
                    if (!string.IsNullOrEmpty(RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.ObjectCode)) && !string.IsNullOrEmpty(RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.OrgCode)))
                    {
                        GLBudgetMenuItem.Visible = true;
                    }
                    else
                    {
                        GLBudgetMenuItem.Visible = false;
                    }
                }
                else
                {
                    GLBudgetMenuItem.Visible = false;
                }
            }
        }

        private void SetModifyMode()
        {
            if (!ConcurrencyCheck())
            {
                RefreshData();
                OtherFunctions.Message("This request has been modified since it's been open and has been refreshed with the current data.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Concurrency Check", this);
            }
            EnableControls();
            ToolStrip.BackColor = Colors.EditColor;
            ShowEditControls();
            isModifying = true;
        }

        private void SetMunisStatus()
        {
            if (!GlobalSwitches.CachedMode)
            {
                if (currentRequest != null)
                {
                    SetReqStatus(currentRequest.RequisitionNumber, currentRequest.NeedByDate.Year);
                    CheckForPO();
                    SetPOStatus(currentRequest.PO);
                }
                else
                {
                    SetReqStatus(string.Empty, -1);
                    SetPOStatus(string.Empty);
                }
            }
        }

        private void SetNewItemRowGuid(int rowIndex)
        {
            if (!isGridFilling)
            {
                if (ReferenceEquals(RequestItemsGrid.Rows[rowIndex].Cells[SibiRequestItemsCols.ItemGuid].Value, null))
                {
                    RequestItemsGrid.Rows[rowIndex].Cells[SibiRequestItemsCols.ItemGuid].Value = Guid.NewGuid().ToString();
                }
            }
        }

        private async void SetPOStatus(string po)
        {
            int poInteger = 0;
            POStatusLabel.Text = "Status: NA";
            if (!string.IsNullOrEmpty(po) && int.TryParse(po, out poInteger))
            {
                string statusString = await MunisFunctions.GetPOStatusFromPO(poInteger);
                if (!string.IsNullOrEmpty(statusString))
                {
                    POStatusLabel.Text = "Status: " + statusString;
                }
            }
        }

        private async void SetReqStatus(string reqNum, int fy)
        {
            int reqInteger = 0;
            ReqStatusLabel.Text = "Status: NA";
            if (fy > 0)
            {
                if (!string.IsNullOrEmpty(reqNum) && int.TryParse(reqNum, out reqInteger))
                {
                    string statusString = await MunisFunctions.GetReqStatusFromReqNum(reqNum, fy);
                    if (!string.IsNullOrEmpty(statusString))
                    {
                        ReqStatusLabel.Text = "Status: " + statusString;
                    }
                }
            }
        }

        private void SetTitle(bool isNew = false)
        {
            if (!isNew)
            {
                this.Text = titleText + " - " + currentRequest.Description;
            }
            else
            {
                this.Text = titleText + " - *New Request*";
            }
        }

        private void SetToolStripItems()
        {
            if (RequestItemsGrid.CurrentCell != null)
            {
                if (IsRightClickColumn())
                {
                    PopulateFAMenuItem.Visible = true;
                    MenuSeparator.Visible = true;
                    if (RequestItemsGrid.CurrentCell.Value != null && RequestItemsGrid.CurrentCell.Value.ToString() != "")
                    {
                        LookupDeviceMenuItem.Visible = true;
                    }
                    else
                    {
                        LookupDeviceMenuItem.Visible = false;
                    }
                }
                else
                {
                    PopulateFAMenuItem.Visible = false;
                    MenuSeparator.Visible = false;
                    LookupDeviceMenuItem.Visible = false;
                }
                if (isModifying)
                {
                    DeleteRequestMenuItem.Visible = true;
                }
                else
                {
                    DeleteRequestMenuItem.Visible = false;
                }
            }
            SetGLBudgetContextMenu();
        }

        private void ShowEditControls()
        {
            EditButtonsPanel.Visible = true;
        }

        private void UpdateRequest()
        {
            using (var trans = DBFactory.GetDatabase().StartTransaction())
            using (var conn = trans.Connection)
            {
                try
                {
                    if (!ConcurrencyCheck())
                    {
                        OtherFunctions.Message("It appears that someone else has modified this request. Please refresh and try again.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Concurrency Failure", this);
                        return;
                    }
                    SibiRequest RequestData = GetRequestItems();
                    if (ReferenceEquals(RequestData.RequestItems, null))
                    {
                        return;
                    }
                    string RequestUpdateQry = Queries.SelectSibiRequestsByGuid(currentRequest.Guid);
                    string RequestItemsUpdateQry = Queries.SelectSibiRequestItems(GridColumnFunctions.ColumnsString(RequestItemsColumns()), currentRequest.Guid);

                    DBFactory.GetDatabase().UpdateTable(RequestUpdateQry, GetUpdateTable(RequestUpdateQry), trans);
                    DBFactory.GetDatabase().UpdateTable(RequestItemsUpdateQry, RequestData.RequestItems, trans);

                    trans.Commit();
                    ParentForm.RefreshData();
                    this.RefreshData();
                    statusSlider.NewSlideMessage("Update successful!");
                }
                catch (Exception ex)
                {
                    trans.Rollback();
                    ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
        }

        private void ValidateCell(int columnIndex, int rowIndex, string value)
        {
            if (RequestItemsGrid[columnIndex, rowIndex].OwningColumn is DataGridViewComboBoxColumn)
            {
                if (!string.IsNullOrEmpty(value))
                {
                    RequestItemsGrid[columnIndex, rowIndex].ErrorText = null;
                }
            }
        }
        private bool ValidateFields()
        {
            bool validFields = controlParser.ValidateFields();
            if (validFields)
            {
                validFields = ValidateRequestItems();
            }
            if (!validFields)
            {
                OtherFunctions.Message("Some required fields are missing. Please enter data into all require fields.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Data", this);
            }
            return validFields;
        }

        private bool ValidateRequestItems()
        {
            bool RowsValid = true;
            foreach (DataGridViewRow row in RequestItemsGrid.Rows)
            {
                if (!row.IsNewRow)
                {
                    foreach (DataGridViewCell dcell in row.Cells)
                    {
                        string CellString = "";
                        if (dcell.Value != null)
                        {
                            CellString = dcell.Value.ToString();
                        }
                        else
                        {
                            CellString = "";
                        }
                        if (ReferenceEquals(dcell.OwningColumn.CellType, typeof(DataGridViewComboBoxCell)))
                        {
                            if (ReferenceEquals(dcell.Value, null) || string.IsNullOrEmpty(CellString))
                            {
                                RowsValid = false;
                                dcell.ErrorText = "Required Field!";
                            }
                            else
                            {
                                dcell.ErrorText = null;
                            }
                        }
                        if (dcell.OwningColumn.Name == SibiRequestItemsCols.Qty)
                        {
                            if (ReferenceEquals(dcell.Value, null) || string.IsNullOrEmpty(CellString))
                            {
                                RowsValid = false;
                                dcell.ErrorText = "Required Field!";
                            }
                            else
                            {
                                dcell.ErrorText = null;
                            }
                        }
                    }
                }
            }
            return RowsValid;
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void ViewAttachments()
        {
            SecurityTools.CheckForAccess(SecurityGroups.ViewAttachment);

            if (!Helpers.ChildFormControl.AttachmentsIsOpen(this))
            {
                if (!string.IsNullOrEmpty(currentRequest.Guid) && !isNewRequest)
                {
                    new AttachmentsForm(this, new SibiAttachmentsCols(), currentRequest, UpdateAttachCountHandler);
                }
            }
        }

        private void ViewGLBudget()
        {
            try
            {
                var org = RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.OrgCode);
                var obj = RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.ObjectCode);
                var fy = currentRequest.DateStamp.Year.ToString();
                MunisFunctions.NewOrgObjView(org, obj, fy, this);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void ViewNote()
        {
            try
            {
                var NoteGuid = NotesGrid.CurrentRowStringValue(SibiNotesCols.NoteGuid);
                if (!Helpers.ChildFormControl.FormIsOpenByGuid(typeof(SibiNotesForm), NoteGuid))
                {
                    new SibiNotesForm(this, NoteGuid);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void ViewPOInfo()
        {
            var po = POTextBox.Text.Trim();
            if (!isModifying && !string.IsNullOrEmpty(po))
            {
                MunisFunctions.NewMunisPOSearch(po, this);
            }
        }

        private void ViewRequestTracker()
        {
            try
            {
                string rtNum = RTNumberTextBox.Text.Trim();
                if (!isModifying && !string.IsNullOrEmpty(rtNum))
                {
                    Process.Start("http://rt.co.fairfield.oh.us/rt/Ticket/Display.html?id=" + rtNum);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private async void ViewRequisition()
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                string reqNum = ReqNumberTextBox.Text.Trim();
                if (!isModifying && !string.IsNullOrEmpty(reqNum))
                {
                    var unused = await MunisFunctions.NewMunisReqSearch(reqNum, currentRequest.NeedByDate.Year.ToString(), this);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        #endregion Methods

        #region ControlEvents

        private void AcceptChangesButton_Click(object sender, EventArgs e)
        {
            AcceptChanges();
        }

        private void AddNoteMenuButton_Click(object sender, EventArgs e)
        {
            NewNote();
        }

        private void AllowDragCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AllowDragChanged();
        }
        private void AttachmentsMenuButton_Click(object sender, EventArgs e)
        {
            ViewAttachments();
        }

        private void CopyTextMenuItem_Click(object sender, EventArgs e)
        {
            RequestItemsGrid.CopyToClipboard(false);
        }

        private void CreateMenuButton_Click(object sender, EventArgs e)
        {
            NewRequest();
        }

        private void CreateNewButton_Click(object sender, EventArgs e)
        {
            AddNewRequest();
        }
        private void DeleteMenuButton_Click(object sender, EventArgs e)
        {
            DeleteCurrentSibiReqest();
        }

        private void DeleteNoteMenuItem_Click(object sender, EventArgs e)
        {
            DeleteCurrentNote();
        }

        private void DeleteRequestMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedRequestItem();
        }

        private void DiscardChangesButton_Click(object sender, EventArgs e)
        {
            CancelModify();
        }

        private void GLBudgetMenuItem_Click(object sender, EventArgs e)
        {
            ViewGLBudget();
        }

        private void ImportDeviceMenuItem_Click(object sender, EventArgs e)
        {
            ImportNewDevice();
        }

        private void LookupDeviceMenuItem_Click(object sender, EventArgs e)
        {
            LookupDevice();
        }

        private void ModifyButton_Click(object sender, EventArgs e)
        {
            ModifyRequest();
        }

        private void NewNoteMenuItem_Click(object sender, EventArgs e)
        {
            NewNote();
        }
        private void NotesGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ViewNote();
        }

        private void PopulateFAMenuItem_Click(object sender, EventArgs e)
        {
            PopulateCurrentFAItem();
        }

        private void POTextBox_Click(object sender, EventArgs e)
        {
            ViewPOInfo();
        }

        private void RefreshMenuButton_Click(object sender, EventArgs e)
        {
            if (!isNewRequest & CancelModify())
            {
                OpenRequest(currentRequest.Guid);
            }
        }

        private void ReqNumberTextBox_Click(object sender, EventArgs e)
        {
            ViewRequisition();
        }

        private void RequestItemsGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            HighlightCurrentRow(e.RowIndex);
        }

        private void RequestItemsGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(RequestItemsGrid, e.RowIndex);
        }

        private void RequestItemsGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CellSelected(e.ColumnIndex, e.RowIndex);
            }
        }

        private void RequestItemsGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            ValidateCell(e.ColumnIndex, e.RowIndex, e.FormattedValue.ToString());
        }

        private void RequestItemsGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            RequestItemsGrid.FastAutoSizeColumns();
        }
        private void RequestItemsGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            OtherFunctions.Message("DataGrid Error: " + "\u0022" + e.Exception.Message + "\u0022" + "   Col/Row:" + e.ColumnIndex + "/" + e.RowIndex, MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "DataGrid Error", this);
        }

        private void RequestItemsGrid_DefaultValuesNeeded(object sender, DataGridViewRowEventArgs e)
        {
            e.Row.Cells[SibiRequestItemsCols.Qty].Value = 1;
        }

        private void RequestItemsGrid_DragDrop(object sender, DragEventArgs e)
        {
            ProcessDragDrop(e.Data);
        }

        private void RequestItemsGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void RequestItemsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            mouseStartPos = e.Location;
        }

        private void RequestItemsGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                BeginDragDrop(e.Location);
            }
        }

        private void RequestItemsGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            SetNewItemRowGuid(e.RowIndex);
        }

        private void RequestItemsGrid_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            //Draw row numbers in row header.
            using (SolidBrush b = new SolidBrush(Color.Black))
            {
                e.Graphics.DrawString((e.RowIndex + 1).ToString(),
                    RequestItemsGrid.DefaultCellStyle.Font,
                    b,
                    e.RowBounds.Location.X + 20,
                    e.RowBounds.Location.Y + 4);
            }
        }
        private void RTNumberTextBox_Click(object sender, EventArgs e)
        {
            ViewRequestTracker();
        }
        // METODO: Maybe move this to ExtendedForm class.
        private void SibiManageRequestForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                ChildFormControl.MinimizeChildren(this);
                prevWindowState = this.WindowState;
            }
            else if (this.WindowState != prevWindowState && this.WindowState == FormWindowState.Normal)
            {
                if (prevWindowState != FormWindowState.Maximized)
                {
                    ChildFormControl.RestoreChildren(this);
                }
            }
        }

        private void SibiManageRequestForm_ResizeBegin(object sender, EventArgs e)
        {
            prevWindowState = this.WindowState;
        }

        #endregion ControlEvents

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }

                    currentRequest.Dispose();
                    munisToolBar.Dispose();
                    windowList.Dispose();
                    controlParser.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}