using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.Tools;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AssetManagement;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiManageRequestForm : ExtendedForm
    {
        #region Fields

        private SibiRequest CurrentRequest = new SibiRequest();
        private string CurrentHash;
        private bool IsModifying = false;
        private bool IsNewRequest = false;
        private bool bolDragging = false;
        private bool bolGridFilling = false;
        private DBControlParser controlParser;
        private Point MouseStartPos;
        private MunisToolBar MyMunisToolBar;
        private string TitleText = "Manage Request";
        private WindowList MyWindowList;
        private FormWindowState PrevWindowState;
        private SliderLabel StatusSlider;

        #endregion Fields

        #region Constructors

        public SibiManageRequestForm(ExtendedForm parentForm, string requestUID) : base(parentForm, requestUID)
        {
            MyMunisToolBar = new MunisToolBar(this);
            MyWindowList = new WindowList(this);

            InitializeComponent();
            InitForm(parentForm, requestUID);

            OpenRequest(requestUID);
        }

        public SibiManageRequestForm(ExtendedForm parentForm) : base(parentForm)
        {
            controlParser = new DBControlParser(this);
            MyMunisToolBar = new MunisToolBar(this);
            MyWindowList = new WindowList(this);

            InitializeComponent();
            InitForm(parentForm);
            Text += " - *New Request*";
            NewRequest();
        }

        #endregion Constructors

        #region Methods

        private bool CancelModify()
        {
            if (IsModifying)
            {
                if (WindowState == FormWindowState.Minimized) WindowState = FormWindowState.Normal;
                this.Activate();
                var blah = OtherFunctions.Message("Are you sure you want to discard all changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Discard Changes?", this);
                if (blah == DialogResult.Yes)
                {
                    if (IsNewRequest)
                    {
                        IsModifying = false;
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

        private void ClearAll()
        {
            this.SuspendLayout();
            ClearControls(this);
            HideEditControls();
            NotesGrid.DataSource = null;
            FillCombos();
            pnlCreate.Visible = false;
            CurrentRequest.Dispose();
            CurrentRequest = null;
            DisableControls();
            ToolStrip.BackColor = Colors.SibiToolBarColor;
            IsModifying = false;
            IsNewRequest = false;
            controlParser.ClearErrors();
            ClearAttachCount();
            SetMunisStatus();
            this.ResumeLayout();
        }

        private void NewRequest()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityTools.AccessGroup.AddSibi);

                OtherFunctions.SetWaitCursor(true, this);

                if (IsModifying)
                {
                    var blah = OtherFunctions.Message("All current changes will be lost. Are you sure you want to start a new request?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, "Create New Request", this);
                    if (blah != DialogResult.OK)
                    {
                        return;
                    }
                }
                ClearAll();
                IsNewRequest = true;
                SetTitle(true);
                CurrentRequest = new SibiRequest();
                this.FormUID = CurrentRequest.GUID;
                IsModifying = true;
                //Set the datasource to a new empty DB table.
                var EmptyTable = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectEmptySibiItemsTable(GridColumnFunctions.ColumnsString(RequestItemsColumns())));
                RequestItemsGrid.Populate(EmptyTable, RequestItemsColumns());
                EnableControls();
                pnlCreate.Visible = true;
                this.Show();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                this.Dispose();
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private void OpenRequest(string RequestUID)
        {
            OtherFunctions.SetWaitCursor(true, this);
            try
            {
                using (DataTable RequestResults = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestsByGUID(RequestUID)))
                using (DataTable RequestItemsResults = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestItems(GridColumnFunctions.ColumnsString(RequestItemsColumns()), RequestUID)))
                {
                    RequestResults.TableName = SibiRequestCols.TableName;
                    RequestItemsResults.TableName = SibiRequestItemsCols.TableName;
                    CurrentHash = GetHash(RequestResults, RequestItemsResults);
                    ClearAll();
                    CollectRequestInfo(RequestResults, RequestItemsResults);
                    controlParser.FillDBFields(RequestResults);
                    SendToGrid(RequestItemsResults);
                    LoadNotes(CurrentRequest.GUID);
                    SetTitle(false);
                    UpdateAttachCountHandler(this, new EventArgs());
                    this.Show();
                    this.Activate();
                    SetMunisStatus();
                    bolGridFilling = false;
                }
            }
            catch (Exception ex)
            {
                OtherFunctions.Message("An error occurred while opening the request. It may have been deleted.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                this.Dispose();
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private string GetHash(DataTable RequestTable, DataTable ItemsTable)
        {
            string RequestHash = SecurityTools.GetSHAOfTable(RequestTable);
            string ItemHash = SecurityTools.GetSHAOfTable(ItemsTable);
            return RequestHash + ItemHash;
        }

        private bool ConcurrencyCheck()
        {
            using (var RequestTable = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestsByGUID(CurrentRequest.GUID)))
            {
                using (var ItemTable = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiRequestItems(GridColumnFunctions.ColumnsString(RequestItemsColumns()), CurrentRequest.GUID)))
                {
                    RequestTable.TableName = SibiRequestCols.TableName;
                    ItemTable.TableName = SibiRequestItemsCols.TableName;
                    string DBHash = GetHash(RequestTable, ItemTable);
                    if (DBHash != CurrentHash)
                    {
                        return false;
                    }
                    return true;
                }
            }
        }

        public void UpdateAttachCountHandler(object sender, EventArgs e)
        {
            AssetManagerFunctions.SetAttachmentCount(cmdAttachments, CurrentRequest.GUID, new SibiAttachmentsCols());
        }

        public void ClearAttachCount()
        {
            cmdAttachments.Text = "(0)";
            cmdAttachments.ToolTipText = "Attachments " + cmdAttachments.Text;
        }

        private bool AddNewNote(string RequestUID, string Note)
        {
            string NoteUID = Guid.NewGuid().ToString();
            try
            {
                ParamCollection noteParams = new ParamCollection();
                noteParams.Add(SibiNotesCols.RequestUID, RequestUID);
                noteParams.Add(SibiNotesCols.NoteUID, NoteUID);
                noteParams.Add(SibiNotesCols.Note, Note);
                if (DBFactory.GetDatabase().InsertFromParameters(SibiNotesCols.TableName, noteParams.Parameters) > 0)
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return false;
            }
        }

        private void AddNewRequest()
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.AddSibi);

            if (!ValidateFields())
            {
                return;
            }
            SibiRequest RequestData = GetRequestItems();
            using (var trans = DBFactory.GetDatabase().StartTransaction())
            {
                using (var conn = trans.Connection)
                {
                    try
                    {
                        string InsertRequestQry = Queries.SelectEmptySibiRequestTable;
                        string InsertRequestItemsQry = Queries.SelectEmptySibiItemsTable(GridColumnFunctions.ColumnsString(RequestItemsColumns()));
                        DBFactory.GetDatabase().UpdateTable(InsertRequestQry, GetInsertTable(InsertRequestQry, CurrentRequest.GUID), trans);
                        DBFactory.GetDatabase().UpdateTable(InsertRequestItemsQry, RequestData.RequestItems, trans);
                        pnlCreate.Visible = false;
                        trans.Commit();
                        IsModifying = false;
                        IsNewRequest = false;
                        ParentForm.RefreshData();
                        this.RefreshData();
                        OtherFunctions.Message("New Request Added.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Complete", this);
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                    }
                }
            }
        }

        private void AddNote()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifySibi);

                if (!string.IsNullOrEmpty(CurrentRequest.GUID) && !IsNewRequest)
                {
                    SibiNotesForm NewNote = new SibiNotesForm(this, CurrentRequest);
                    if (NewNote.DialogResult == DialogResult.OK)
                    {
                        AddNewNote(NewNote.Request.GUID, NewNote.Note);
                        LoadNotes(CurrentRequest.GUID);
                    }
                }
                else
                {
                    if (IsNewRequest)
                    {
                        OtherFunctions.Message("You must create a new request before adding notes.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Error", this);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private async void CheckForPO()
        {
            if (!string.IsNullOrEmpty(CurrentRequest.RequisitionNumber) && string.IsNullOrEmpty(CurrentRequest.PO))
            {
                string GetPO = await MunisFunctions.GetPOFromReqNumberAsync(CurrentRequest.RequisitionNumber, CurrentRequest.NeedByDate.Year.ToString());
                if (GetPO != null && GetPO.Length > 1)
                {
                    var blah = OtherFunctions.Message("PO Number " + GetPO + " was detected in the Requisition. Do you wish to add it to this request?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "New PO Detected", this);
                    if (blah == DialogResult.Yes)
                    {
                        InsertPONumber(GetPO);
                        OpenRequest(CurrentRequest.GUID);
                    }
                    else
                    {
                        return;
                    }
                }
            }
        }

        private void chkAllowDrag_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAllowDrag.Checked)
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
                    cmb.Text = null;
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

        private void cmdAccept_Click(object sender, EventArgs e)
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
            IsModifying = false;
        }

        private void cmdAddNew_Click(object sender, EventArgs e)
        {
            AddNewRequest();
        }

        private void cmdAddNote_Click(object sender, EventArgs e)
        {
            AddNote();
        }

        private void ViewAttachments()
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ViewAttachment);

            if (!Helpers.ChildFormControl.AttachmentsIsOpen(this))
            {
                if (!string.IsNullOrEmpty(CurrentRequest.GUID) && !IsNewRequest)
                {
                    AttachmentsForm NewAttach = new AttachmentsForm(this, new SibiAttachmentsCols(), CurrentRequest, UpdateAttachCountHandler);
                }
            }
        }

        private void cmdAttachments_Click(object sender, EventArgs e)
        {
            ViewAttachments();
        }

        private void cmdCreate_Click(object sender, EventArgs e)
        {
            NewRequest();
        }

        private void DeleteCurrentSibiReqest()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityTools.AccessGroup.DeleteSibi);

                if (ReferenceEquals(CurrentRequest.RequestItems, null))
                {
                    return;
                }
                var blah = OtherFunctions.Message("Are you absolutely sure?  This cannot be undone and will delete all data including attachments.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, "WARNING", this);
                if (blah == DialogResult.Yes)
                {
                    OtherFunctions.SetWaitCursor(true, this);
                    if (AssetManagerFunctions.DeleteSibiRequest(CurrentRequest.GUID))
                    {
                        OtherFunctions.Message("Sibi Request deleted successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Device Deleted", this);
                        CurrentRequest = null;
                        ParentForm.RefreshData();
                        this.Dispose();
                    }
                    else
                    {
                        Logging.Logger("*****DELETION ERROR******: " + CurrentRequest.GUID);
                        OtherFunctions.Message("Failed to delete request successfully!  Please let Bobby Lovell know about this.", MessageBoxButtons.OK, MessageBoxIcon.Error, "Delete Failed", this);
                        CurrentRequest = null;
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private void cmdDelete_Click(object sender, EventArgs e)
        {
            DeleteCurrentSibiReqest();
        }

        private void DeleteCurrentNote()
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifySibi);

            if (NotesGrid.CurrentRow != null && NotesGrid.CurrentRow.Index > -1)
            {
                var blah = OtherFunctions.Message("Are you sure?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Delete Note", this);
                if (blah == DialogResult.Yes)
                {
                    string NoteUID = NotesGrid.CurrentRowStringValue(SibiNotesCols.NoteUID);
                    if (!string.IsNullOrEmpty(NoteUID))
                    {
                        OtherFunctions.Message(DeleteNote(NoteUID) + " Rows affected.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Delete Item", this);
                        OpenRequest(CurrentRequest.GUID);
                    }
                }
            }
        }

        private void cmdDeleteNote_Click(object sender, EventArgs e)
        {
            DeleteCurrentNote();
        }

        private void cmdDiscard_Click(object sender, EventArgs e)
        {
            CancelModify();
        }

        private void cmdNewNote_Click(object sender, EventArgs e)
        {
            AddNote();
        }

        private void ModifyRequest()
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifySibi);

            if (!string.IsNullOrEmpty(CurrentRequest.GUID) && !IsModifying)
            {
                SetModifyMode();
            }
        }

        private void ModifyButton_Click(object sender, EventArgs e)
        {
            ModifyRequest();
        }

        private SibiRequest GetRequestItems()
        {
            RequestItemsGrid.EndEdit();

            SibiRequest request = new SibiRequest();
            request.RequestItems = (DataTable)RequestItemsGrid.DataSource;
            request.GUID = CurrentRequest.GUID;

            MarkupRequestItems(request.RequestItems);

            return request;
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
                    if (row.RowState != DataRowState.Unchanged)
                    {
                        // Add the request UID to the row if needed.
                        if (col.ColumnName == SibiRequestItemsCols.RequestUID)
                        {
                            if (row[col] == null || string.IsNullOrEmpty(row[col].ToString()))
                            {
                                row[SibiRequestItemsCols.RequestUID] = CurrentRequest.GUID;
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

        private void CollectRequestInfo(DataTable RequestResults, DataTable RequestItemsResults)
        {
            try
            {
                CurrentRequest = new SibiRequest(RequestResults);
                CurrentRequest.RequestItems = RequestItemsResults;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private bool DeleteItem_FromLocal(int RowIndex)
        {
            try
            {
                if (!RequestItemsGrid.Rows[RowIndex].IsNewRow)
                {
                    RequestItemsGrid.Rows.Remove(RequestItemsGrid.Rows[RowIndex]);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return false;
            }
        }

        private int DeleteNote(string noteUID)
        {
            try
            {
                return DBFactory.GetDatabase().ExecuteNonQuery(Queries.DeleteSibiNote(noteUID));
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return -1;
            }
        }

        private void NotesGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            ViewNote();
        }

        private void ViewNote()
        {
            try
            {
                var NoteUID = NotesGrid.CurrentRowStringValue(SibiNotesCols.NoteUID);
                if (!Helpers.ChildFormControl.FormIsOpenByUID(typeof(SibiNotesForm), NoteUID))
                {
                    SibiNotesForm ViewNote = new SibiNotesForm(this, NoteUID);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
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
                    if (c != chkAllowDrag)
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

        private void DisableControls()
        {
            DisableControlsRecursive(this);
            DisableGrid();
        }

        private void DisableGrid()
        {
            RequestItemsGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            RequestItemsGrid.AllowUserToAddRows = false;
            RequestItemsGrid.MultiSelect = true;
        }

        private void EnableControlsRecursive(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    var txt = (TextBox)c;
                    if (txt != txtRequestNum && txt != txtCreateDate)
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

        private void EnableControls()
        {
            EnableControlsRecursive(this);
            EnableGrid();
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
            AttributeFunctions.FillComboBox(GlobalInstances.SibiAttribute.StatusType, cmbStatus);
            AttributeFunctions.FillComboBox(GlobalInstances.SibiAttribute.RequestType, cmbType);
        }

        public override bool OKToClose()
        {
            bool CanClose = true;
            if (!Helpers.ChildFormControl.OKToCloseChildren(this))
            {
                CanClose = false;
            }
            if (IsModifying && !CancelModify())
            {
                CanClose = false;
            }
            return CanClose;
        }

        private DataTable GetInsertTable(string selectQuery, string UID)
        {
            try
            {
                var tmpTable = controlParser.ReturnInsertTable(selectQuery);
                var DBRow = tmpTable.Rows[0];
                //Add Add'l info
                DBRow[SibiRequestCols.UID] = UID;
                return tmpTable;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return null;
            }
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return null;
            }
        }

        private void HideEditControls()
        {
            pnlEditButtons.Visible = false;
        }

        private void HighlightCurrentRow(int Row)
        {
            try
            {
                if (!bolGridFilling)
                {
                    StyleFunctions.HighlightRow(RequestItemsGrid, GridTheme, Row);
                }
            }
            catch
            {
            }
        }

        private void InitDBControls()
        {
            txtDescription.Tag = new DBControlInfo(SibiRequestCols.Description, true);
            txtUser.Tag = new DBControlInfo(SibiRequestCols.RequestUser, true);
            cmbType.Tag = new DBControlInfo(SibiRequestCols.Type, GlobalInstances.SibiAttribute.RequestType, true);
            dtNeedBy.Tag = new DBControlInfo(SibiRequestCols.NeedBy, true);
            cmbStatus.Tag = new DBControlInfo(SibiRequestCols.Status, GlobalInstances.SibiAttribute.StatusType, true);
            txtPO.Tag = new DBControlInfo(SibiRequestCols.PO, false);
            txtReqNumber.Tag = new DBControlInfo(SibiRequestCols.RequisitionNumber, false);
            txtRequestNum.Tag = new DBControlInfo(SibiRequestCols.RequestNumber, ParseType.DisplayOnly, false);
            txtRTNumber.Tag = new DBControlInfo(SibiRequestCols.RTNumber, false);
            txtCreateDate.Tag = new DBControlInfo(SibiRequestCols.DateStamp, ParseType.DisplayOnly, false);
        }

        private void InitForm(ExtendedForm parentForm, string UID = "")
        {
            StatusSlider = new SliderLabel();
            StatusStrip1.Items.Insert(0, StatusSlider.ToToolStripControl(StatusStrip1));

            InitDBControls();

            controlParser = new DBControlParser(this);
            controlParser.EnableFieldValidation();

            RequestItemsGrid.DoubleBufferedDataGrid(true);
            NotesGrid.DoubleBufferedDataGrid(true);
            MyMunisToolBar.InsertMunisDropDown(ToolStrip);
            ImageCaching.CacheControlImages(this);
            MyWindowList.InsertWindowList(ToolStrip);
            StyleFunctions.SetGridStyle(RequestItemsGrid, GridTheme);
            StyleFunctions.SetGridStyle(NotesGrid, GridTheme);
            ToolStrip.BackColor = Colors.SibiToolBarColor;
        }

        private void InsertPONumber(string PO)
        {
            try
            {
                AssetManagerFunctions.UpdateSqlValue(SibiRequestCols.TableName, SibiRequestCols.PO, PO, SibiRequestCols.UID, CurrentRequest.GUID);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void LoadNotes(string RequestUID)
        {
            NotesGrid.SuspendLayout();
            using (DataTable Results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectSibiNotes(RequestUID)))
            {
                NotesGrid.Populate(Results, NotesGridColumns());
            }
            NotesGrid.FastAutoSizeColumns();
            NotesGrid.ClearSelection();
            NotesGrid.ResumeLayout();
        }

        private bool MouseIsDragging(Point CurrentPos)
        {
            int intMouseMoveThreshold = 50;
            var intDistanceMoved = Math.Sqrt(Math.Pow((MouseStartPos.X - CurrentPos.X), 2) + Math.Pow((MouseStartPos.Y - CurrentPos.Y), 2));
            if (System.Convert.ToInt32(intDistanceMoved) > intMouseMoveThreshold)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private async void PopulateFromFA(string columnName)
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                string itemUID = RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.ItemUID);
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
                    AssetManagerFunctions.UpdateSqlValue(SibiRequestItemsCols.TableName, columnName, updateValue, SibiRequestItemsCols.ItemUID, itemUID);
                    RefreshData();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        public override void RefreshData()
        {
            OpenRequest(CurrentRequest.GUID);
        }

        private List<GridColumnAttrib> NotesGridColumns()
        {
            List<GridColumnAttrib> ColList = new List<GridColumnAttrib>();
            ColList.Add(new GridColumnAttrib(SibiNotesCols.Note, "Note", typeof(string), ColumnFormatTypes.NotePreview));
            ColList.Add(new GridColumnAttrib(SibiNotesCols.DateStamp, "Date Stamp", typeof(DateTime)));
            ColList.Add(new GridColumnAttrib(SibiNotesCols.NoteUID, "UID", typeof(string), true, false));
            return ColList;
        }

        private List<GridColumnAttrib> RequestItemsColumns()
        {
            List<GridColumnAttrib> ColList = new List<GridColumnAttrib>();
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.User, "User", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Description, "Description", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Qty, "Qty", typeof(int)));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Location, "Location", GlobalInstances.DeviceAttribute.Locations));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Status, "Status", GlobalInstances.SibiAttribute.ItemStatusType));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ReplaceAsset, "Replace Asset", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ReplaceSerial, "Replace Serial", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.NewAsset, "New Asset", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.NewSerial, "New Serial", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.OrgCode, "Org Code", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ObjectCode, "Object Code", typeof(string)));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.Timestamp, "Created", typeof(DateTime), true, true));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ModifiedDate, "Modified", typeof(DateTime), true, true));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ModifiedBy, "Modified By", typeof(string), true, true));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.ItemUID, "Item UID", typeof(string), true, true));
            ColList.Add(new GridColumnAttrib(SibiRequestItemsCols.RequestUID, "Request UID", typeof(string), true, false));

            return ColList;
        }

        private void RequestItemsGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            HighlightCurrentRow(e.RowIndex);
        }

        private void RequestItemsGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(RequestItemsGrid, e.RowIndex);
        }

        private void RequestItemsGrid_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (RequestItemsGrid[e.ColumnIndex, e.RowIndex].OwningColumn is DataGridViewComboBoxColumn)
            {
                if (!string.IsNullOrEmpty(e.FormattedValue.ToString()))
                {
                    RequestItemsGrid[e.ColumnIndex, e.RowIndex].ErrorText = null;
                }
            }
        }

        private void RequestItemsGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            RequestItemsGrid.FastAutoSizeColumns();
        }

        private void RequestItemsGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (e.Button == MouseButtons.Right)
                {
                    if (e.ColumnIndex >= -1 && e.RowIndex >= 0)
                    {
                        int ColIndex = (e.ColumnIndex == -1 ? 0 : e.ColumnIndex);
                        if (!RequestItemsGrid[ColIndex, e.RowIndex].Selected)
                        {
                            RequestItemsGrid.Rows[e.RowIndex].Selected = true;
                            RequestItemsGrid.CurrentCell = RequestItemsGrid[ColIndex, e.RowIndex];
                        }
                        SetToolStripItems();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
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
            //Drag-drop rows from other data grids, adding new UIDs and the current FKey (RequestUID).
            //This was a tough nut to crack. In the end I just ended up building an item array and adding it to the receiving grids datasource.
            try
            {
                if (IsModifying)
                {
                    var R = (DataGridViewRow)(e.Data.GetData(typeof(DataGridViewRow))); //Cast the DGVRow
                    if (R.DataBoundItem != null)
                    {
                        DataRow NewDataRow = ((DataRowView)R.DataBoundItem).Row; //Get the databound row
                        List<object> ItemArr = new List<object>();
                        foreach (DataColumn col in NewDataRow.Table.Columns) //Iterate through columns and build a new item list
                        {
                            if (col.ColumnName == SibiRequestItemsCols.ItemUID)
                            {
                                ItemArr.Add(Guid.NewGuid().ToString());
                            }
                            else if (col.ColumnName == SibiRequestItemsCols.RequestUID)
                            {
                                ItemArr.Add(CurrentRequest.GUID);
                            }
                            else
                            {
                                ItemArr.Add(NewDataRow[col]);
                            }
                        }
                        ((DataTable)RequestItemsGrid.DataSource).Rows.Add(ItemArr.ToArray()); //Add the item list as an array
                    }
                    bolDragging = false;
                }
                else
                {
                    if (!bolDragging)
                    {
                        OtherFunctions.Message("You must be modifying this request before you can drag-drop rows from another request.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Not Allowed", this);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void RequestItemsGrid_DragEnter(object sender, DragEventArgs e)
        {
            e.Effect = DragDropEffects.Copy;
        }

        private void RequestItemsGrid_MouseDown(object sender, MouseEventArgs e)
        {
            MouseStartPos = e.Location;
        }

        private void RequestItemsGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (RequestItemsGrid.SelectedRows.Count > 0)
            {
                if (chkAllowDrag.Checked && !bolDragging)
                {
                    if (e.Button == MouseButtons.Left)
                    {
                        if (MouseIsDragging(CurrentPos: e.Location))
                        {
                            bolDragging = true;
                            RequestItemsGrid.DoDragDrop(RequestItemsGrid.SelectedRows[0], DragDropEffects.All);
                            bolDragging = false;
                        }
                    }
                }
            }
        }

        private void RequestItemsGrid_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!bolGridFilling)
            {
                if (ReferenceEquals(RequestItemsGrid.Rows[e.RowIndex].Cells[SibiRequestItemsCols.ItemUID].Value, null))
                {
                    RequestItemsGrid.Rows[e.RowIndex].Cells[SibiRequestItemsCols.ItemUID].Value = Guid.NewGuid().ToString();
                }
            }
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

        private void SendToGrid(DataTable Results)
        {
            bolGridFilling = true;
            RequestItemsGrid.SuspendLayout();
            RequestItemsGrid.Populate(Results, RequestItemsColumns(), true);
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
                        tsmGLBudget.Visible = true;
                    }
                    else
                    {
                        tsmGLBudget.Visible = false;
                    }
                }
                else
                {
                    tsmGLBudget.Visible = false;
                }
            }
        }

        private void SetMunisStatus()
        {
            if (!GlobalSwitches.CachedMode)
            {
                if (CurrentRequest != null)
                {
                    SetReqStatus(CurrentRequest.RequisitionNumber, CurrentRequest.NeedByDate.Year);
                    CheckForPO();
                    SetPOStatus(CurrentRequest.PO);
                }
                else
                {
                    SetReqStatus(string.Empty, -1);
                    SetPOStatus(string.Empty);
                }
            }
        }

        private async void SetPOStatus(string PO)
        {
            int intPO = 0;
            lblPOStatus.Text = "Status: NA";
            if (!string.IsNullOrEmpty(PO) && int.TryParse(PO, out intPO))
            {
                string GetStatusString = await MunisFunctions.GetPOStatusFromPO(intPO);
                if (!string.IsNullOrEmpty(GetStatusString))
                {
                    lblPOStatus.Text = "Status: " + GetStatusString;
                }
            }
        }

        private async void SetReqStatus(string ReqNum, int FY)
        {
            int intReq = 0;
            lblReqStatus.Text = "Status: NA";
            if (FY > 0)
            {
                if (!string.IsNullOrEmpty(ReqNum) && int.TryParse(ReqNum, out intReq))
                {
                    string GetStatusString = await MunisFunctions.GetReqStatusFromReqNum(ReqNum, FY);
                    if (!string.IsNullOrEmpty(GetStatusString))
                    {
                        lblReqStatus.Text = "Status: " + GetStatusString;
                    }
                }
            }
        }

        private void SetTitle(bool NewRequest = false)
        {
            if (!NewRequest)
            {
                this.Text = TitleText + " - " + CurrentRequest.Description;
            }
            else
            {
                this.Text = TitleText + " - *New Request*";
            }
        }

        private void SetToolStripItems()
        {
            if (RequestItemsGrid.CurrentCell != null)
            {
                if (ValidColumn())
                {
                    tsmPopFA.Visible = true;
                    tsmSeparator.Visible = true;
                    if (RequestItemsGrid.CurrentCell.Value != null && RequestItemsGrid.CurrentCell.Value.ToString() != "")
                    {
                        tsmLookupDevice.Visible = true;
                    }
                    else
                    {
                        tsmLookupDevice.Visible = false;
                    }
                }
                else
                {
                    tsmPopFA.Visible = false;
                    tsmSeparator.Visible = false;
                    tsmLookupDevice.Visible = false;
                }
                if (IsModifying)
                {
                    tsmDeleteItem.Visible = true;
                }
                else
                {
                    tsmDeleteItem.Visible = false;
                }
            }
            SetGLBudgetContextMenu();
        }

        private void ShowEditControls()
        {
            pnlEditButtons.Visible = true;
        }

        private void tsbRefresh_Click(object sender, EventArgs e)
        {
            if (!IsNewRequest)
            {
                OpenRequest(CurrentRequest.GUID);
            }
        }

        private void tsmCopyText_Click(object sender, EventArgs e)
        {
            RequestItemsGrid.CopyToClipboard(false);
        }

        private void DeleteSelectedRequestItem()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifySibi);

                var blah = OtherFunctions.Message("Delete selected row?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Delete Item Row", this);
                if (blah == DialogResult.Yes)
                {
                    if (!DeleteItem_FromLocal(RequestItemsGrid.CurrentRow.Index))
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void tsmDeleteItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedRequestItem();
        }

        private void tsmGLBudget_Click(object sender, EventArgs e)
        {
            try
            {
                var Org = RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.OrgCode);
                var Obj = RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.ObjectCode);
                var FY = CurrentRequest.DateStamp.Year.ToString();
                MunisFunctions.NewOrgObView(Org, Obj, FY, this);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void tsmLookupDevice_Click(object sender, EventArgs e)
        {
            try
            {
                int colIndex = RequestItemsGrid.CurrentCell.ColumnIndex;
                if (colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.ReplaceAsset) || colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.NewAsset))
                {
                    Helpers.ChildFormControl.LookupDevice(this, AssetManagerFunctions.FindDeviceFromAssetOrSerial(RequestItemsGrid[colIndex, RequestItemsGrid.CurrentRow.Index].Value.ToString(), AssetManagerFunctions.FindDevType.AssetTag));
                }
                else if (colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.ReplaceSerial) || colIndex == RequestItemsGrid.ColumnIndex(SibiRequestItemsCols.NewSerial))
                {
                    Helpers.ChildFormControl.LookupDevice(this, AssetManagerFunctions.FindDeviceFromAssetOrSerial(RequestItemsGrid[colIndex, RequestItemsGrid.CurrentRow.Index].Value.ToString(), AssetManagerFunctions.FindDevType.Serial));
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void PopulateCurrentFAItem()
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifySibi);

            try
            {
                if (ValidColumn())
                {
                    PopulateFromFA(RequestItemsGrid.CurrentCell.OwningColumn.Name);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void tsmPopFA_Click(object sender, EventArgs e)
        {
            PopulateCurrentFAItem();
        }

        private void txtPO_Click(object sender, EventArgs e)
        {
            string PO = txtPO.Text.Trim();
            if (!IsModifying && !string.IsNullOrEmpty(PO))
            {
                MunisFunctions.NewMunisPOSearch(PO, this);
            }
        }

        private void txtReqNumber_Click(object sender, EventArgs e)
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                string ReqNum = txtReqNumber.Text.Trim();
                if (!IsModifying && !string.IsNullOrEmpty(ReqNum))
                {
                    MunisFunctions.NewMunisReqSearch(ReqNum, CurrentRequest.NeedByDate.Year.ToString(), this);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
            finally
            {
                OtherFunctions.SetWaitCursor(false, this);
            }
        }

        private void txtRTNumber_Click(object sender, EventArgs e)
        {
            try
            {
                string RTNum = txtRTNumber.Text.Trim();
                if (!IsModifying && !string.IsNullOrEmpty(RTNum))
                {
                    Process.Start("http://rt.co.fairfield.oh.us/rt/Ticket/Display.html?id=" + RTNum);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
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
            IsModifying = true;
        }

        private void UpdateRequest()
        {
            using (var trans = DBFactory.GetDatabase().StartTransaction())
            {
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
                        string RequestUpdateQry = Queries.SelectSibiRequestsByGUID(CurrentRequest.GUID);
                        string RequestItemsUpdateQry = Queries.SelectSibiRequestItems(GridColumnFunctions.ColumnsString(RequestItemsColumns()), CurrentRequest.GUID);

                        DBFactory.GetDatabase().UpdateTable(RequestUpdateQry, GetUpdateTable(RequestUpdateQry), trans);
                        DBFactory.GetDatabase().UpdateTable(RequestItemsUpdateQry, RequestData.RequestItems, trans);

                        trans.Commit();
                        ParentForm.RefreshData();
                        this.RefreshData();
                        StatusSlider.NewSlideMessage("Update successful!");
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                    }
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

        private bool ValidColumn()
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                return false;
            }
        }

        private void NewDeviceMenuItem_Click(object sender, EventArgs e)
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.AddDevice);

            NewDeviceForm NewDev = new NewDeviceForm(this);
            NewDev.ImportFromSibi(RequestItemsGrid.CurrentRowStringValue(SibiRequestItemsCols.ItemUID));
        }

        private void SibiManageRequestForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!OKToClose())
            {
                e.Cancel = true;
            }
        }

        private void SibiManageRequestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyMunisToolBar.Dispose();
            MyWindowList.Dispose();
            Helpers.ChildFormControl.CloseChildren(this);
        }

        private void SibiManageRequestForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Helpers.ChildFormControl.MinimizeChildren(this);
                PrevWindowState = this.WindowState;
            }
            else if (this.WindowState != PrevWindowState && this.WindowState == FormWindowState.Normal)
            {
                if (PrevWindowState != FormWindowState.Maximized)
                {
                    Helpers.ChildFormControl.RestoreChildren(this);
                }
            }
        }

        private void SibiManageRequestForm_ResizeBegin(object sender, EventArgs e)
        {
            PrevWindowState = this.WindowState;
        }

        #endregion Methods
    }
}