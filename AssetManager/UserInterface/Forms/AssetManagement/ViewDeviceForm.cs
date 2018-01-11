using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.Attachments;
using AssetManager.UserInterface.Forms.Sibi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class ViewDeviceForm : ExtendedForm
    {
        #region Fields

        public MunisEmployeeStruct MunisUser = new MunisEmployeeStruct();
        private bool FieldsInvalid;
        private bool GridFilling = false;
        private string CurrentHash;
        private DeviceMapObject CurrentViewDevice;
        private DBControlParser DataParser;
        private bool EditMode = false;
        private LiveBox MyLiveBox;
        private MunisToolBar MyMunisToolBar;
        private WindowList MyWindowList;
        private SliderLabel StatusSlider;

        #endregion Fields

        #region Delegates

        private delegate void StatusVoidDelegate(string text);

        #endregion Delegates

        #region Constructors

        public ViewDeviceForm(ExtendedForm parentForm, DataMapObject device) : base(parentForm, device)
        {
            CurrentViewDevice = (DeviceMapObject)device;

            DataParser = new DBControlParser(this);

            InitializeComponent();

            DefaultFormTitle = this.Text;

            MyLiveBox = new LiveBox(this);
            MyLiveBox.AttachToControl(CurrentUserTextBox, DevicesCols.CurrentUser, LiveBoxType.UserSelect, DevicesCols.MunisEmpNum);
            MyLiveBox.AttachToControl(DescriptionTextBox, DevicesCols.Description, LiveBoxType.SelectValue);

            MyMunisToolBar = new MunisToolBar(this);
            MyMunisToolBar.InsertMunisDropDown(ToolStrip1, 6);

            MyWindowList = new WindowList(this);
            MyWindowList.InsertWindowList(ToolStrip1);

            StatusSlider = new SliderLabel();
            StatusStrip1.Items.Add(StatusSlider.ToToolStripControl(StatusStrip1));

            ImageCaching.CacheControlImages(this);

            InitDBControls();

            RefreshCombos();

            DataGridHistory.DoubleBufferedDataGrid(true);
            TrackingGrid.DoubleBufferedDataGrid(true);

            SetEditMode(false);

            LoadDevice();
        }

        #endregion Constructors

        #region Methods

        public async void SetPingHistoryLink()
        {
            bool hasPingHist = await GlobalInstances.AssetFunc.HasPingHistory(CurrentViewDevice);
            PingHistLabel.Visible = hasPingHist;
        }

        public void LoadDevice()
        {
            try
            {
                GridFilling = true;
                LoadHistoryAndFields();
                if (CurrentViewDevice.IsTrackable)
                {
                    LoadTracking(CurrentViewDevice.GUID);
                }
                SetPingHistoryLink();
                SetTracking(CurrentViewDevice.IsTrackable, CurrentViewDevice.Tracking.IsCheckedOut);
                this.Text = this.DefaultFormTitle + FormTitle(CurrentViewDevice);
                this.Show();
                DataGridHistory.ClearSelection();
                GridFilling = false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        public override bool OKToClose()
        {
            bool CanClose = true;
            if (!Helpers.ChildFormControl.OKToCloseChildren(this))
            {
                CanClose = false;
            }
            if (EditMode && !CancelModify())
            {
                CanClose = false;
            }
            return CanClose;
        }

        public override void RefreshData()
        {
            if (EditMode)
            {
                CancelModify();
            }
            else
            {
                ActiveDirectoryBox.Visible = false;
                remoteToolsControl.Visible = false;
                CurrentViewDevice = new DeviceMapObject(CurrentViewDevice.GUID);
                LoadDevice();
            }
        }

        public void UpdateAttachCountHandler(object sender, EventArgs e)
        {
            GlobalInstances.AssetFunc.SetAttachmentCount(AttachmentsToolButton, CurrentViewDevice.GUID, new DeviceAttachmentsCols());
        }

        private void AcceptChanges()
        {
            try
            {
                if (!CheckFields())
                {
                    OtherFunctions.Message("Some required fields are missing or invalid.  Please check and fill all highlighted fields.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Missing Data", this);
                    FieldsInvalid = true;
                    return;
                }
                using (UpdateDev UpdateDia = new UpdateDev(this))
                {
                    if (UpdateDia.DialogResult == DialogResult.OK)
                    {
                        if (!ConcurrencyCheck())
                        {
                            CancelModify();
                            return;
                        }
                        else
                        {
                            UpdateDevice(UpdateDia.UpdateInfo);
                        }
                    }
                    else
                    {
                        CancelModify();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void AddErrorIcon(Control ctl)
        {
            if (ReferenceEquals(fieldErrorIcon.GetError(ctl), string.Empty))
            {
                fieldErrorIcon.SetIconAlignment(ctl, ErrorIconAlignment.MiddleRight);
                fieldErrorIcon.SetIconPadding(ctl, 4);
                fieldErrorIcon.SetError(ctl, "Required or Invalid Field");
            }
        }

        private void AddNewNote()
        {
            try
            {
                if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifyDevice))
                {
                    return;
                }
                using (UpdateDev UpdateDia = new UpdateDev(this, true))
                {
                    if (UpdateDia.DialogResult == DialogResult.OK)
                    {
                        if (!ConcurrencyCheck())
                        {
                            RefreshData();
                        }
                        else
                        {
                            UpdateDevice(UpdateDia.UpdateInfo);
                        }
                    }
                    else
                    {
                        RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private bool CancelModify()
        {
            if (EditMode)
            {
                this.WindowState = FormWindowState.Normal;
                this.Activate();
                var blah = OtherFunctions.Message("Are you sure you want to discard all changes?", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Question, "Discard Changes?", this);
                if (blah == DialogResult.Yes)
                {
                    FieldsInvalid = false;
                    fieldErrorIcon.Clear();
                    SetEditMode(false);
                    ResetBackColors();
                    RefreshData();
                    return true;
                }
            }
            return false;
        }

        private bool CheckFields()
        {
            bool MissingField = false;
            MissingField = false;
            fieldErrorIcon.Clear();
            foreach (Control c in DataParser.GetDBControls(this))
            {
                DBControlInfo DBInfo = (DBControlInfo)c.Tag;
                if (c is TextBox)
                {
                    if (DBInfo.Required)
                    {
                        if (c.Text.Trim() == "")
                        {
                            MissingField = true;
                            c.BackColor = Colors.MissingField;
                            AddErrorIcon(c);
                        }
                        else
                        {
                            c.BackColor = Color.Empty;
                            ClearErrorIcon(c);
                        }
                        c.TextChanged -= RecheckFieldsEvent;
                        c.TextChanged += RecheckFieldsEvent;
                    }
                }
                else if (c is ComboBox)
                {
                    ComboBox cmb = (ComboBox)c;
                    if (DBInfo.Required)
                    {
                        if (cmb.SelectedIndex == -1)
                        {
                            MissingField = true;
                            cmb.BackColor = Colors.MissingField;
                            AddErrorIcon(cmb);
                        }
                        else
                        {
                            cmb.BackColor = Color.Empty;
                            ClearErrorIcon(cmb);
                        }
                        cmb.SelectedIndexChanged -= RecheckFieldsEvent;
                        cmb.SelectedIndexChanged += RecheckFieldsEvent;
                    }
                }
            }
            if (!DataConsistency.ValidPhoneNumber(PhoneNumberTextBox.Text))
            {
                MissingField = true;
                PhoneNumberTextBox.BackColor = Colors.MissingField;
                AddErrorIcon(PhoneNumberTextBox);
            }
            else
            {
                PhoneNumberTextBox.BackColor = Color.Empty;
                ClearErrorIcon(PhoneNumberTextBox);
            }
            return !MissingField; //if fields are missing return false to trigger a message if needed
        }

        private void RecheckFieldsEvent(object sender, EventArgs e)
        {
            if (FieldsInvalid)
            {
                CheckFields();
            }
        }

        private void ClearErrorIcon(Control ctl)
        {
            fieldErrorIcon.SetError(ctl, string.Empty);
        }

        private void CollectCurrentTracking(DataTable results)
        {
            CurrentViewDevice.Tracking.MapClassProperties(results);
        }

        private bool ConcurrencyCheck()
        {
            using (var DeviceResults = GetDevicesTable(CurrentViewDevice.GUID))
            {
                using (var HistoricalResults = GetHistoricalTable(CurrentViewDevice.GUID))
                {
                    DeviceResults.TableName = DevicesCols.TableName;
                    HistoricalResults.TableName = HistoricalDevicesCols.TableName;
                    var DBHash = GetHash(DeviceResults, HistoricalResults);
                    if (DBHash != CurrentHash)
                    {
                        OtherFunctions.Message("This record appears to have been modified by someone else since the start of this modification.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Concurrency Error", this);
                        return false;
                    }
                    return true;
                }
            }
        }

        private void DeleteDevice()
        {
            if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.DeleteDevice))
            {
                return;
            }
            var blah = OtherFunctions.Message("Are you absolutely sure?  This cannot be undone and will delete all historical data, tracking and attachments.", (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Exclamation, "WARNING", this);
            if (blah == DialogResult.Yes)
            {
                if (GlobalInstances.AssetFunc.DeleteFtpAndSql(CurrentViewDevice.GUID, EntryType.Device))
                {
                    OtherFunctions.Message("Device deleted successfully.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Device Deleted", this);
                    CurrentViewDevice = null;
                    Helpers.ChildFormControl.MainFormInstance().RefreshData();
                }
                else
                {
                    Logging.Logger("*****DELETION ERROR******: " + CurrentViewDevice.GUID);
                    OtherFunctions.Message("Failed to delete device succesfully!  Please let Bobby Lovell know about this.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Stop, "Delete Failed", this);
                    CurrentViewDevice = null;
                }
                this.Dispose();
            }
            else
            {
                return;
            }
        }

        private void DeleteSelectedHistoricalEntry()
        {
            if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifyDevice))
            {
                return;
            }
            try
            {
                string entryGUID = GridFunctions.GetCurrentCellValue(DataGridHistory, HistoricalDevicesCols.HistoryEntryUID);
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectHistoricalDeviceEntry(entryGUID)))
                using (var Info = new DeviceMapObject(results))
                {
                    var blah = OtherFunctions.Message("Are you sure you want to delete this entry?  This cannot be undone!" + "\r\n" + "\r\n" + "Entry info: " + Info.Historical.ActionDateTime + " - " + AttributeFunctions.GetDisplayValueFromCode(GlobalInstances.DeviceAttribute.ChangeType, Info.Historical.ChangeType) + " - " + entryGUID, (int)MessageBoxButtons.YesNo + (int)MessageBoxIcon.Exclamation, "Are you sure?", this);
                    if (blah == DialogResult.Yes)
                    {
                        int affectedRows = DBFactory.GetDatabase().ExecuteQuery(Queries.DeleteHistoricalEntryByGUID(entryGUID));
                        if (affectedRows > 0)
                        {
                            SetStatusBar("Entry deleted successfully.");
                            RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void SetEditMode(bool editMode)
        {
            if (editMode)
            {
                EditMode = true;
                EnableControlsRecursive(this);
                ActiveDirectoryBox.Visible = false;
                MunisSibiPanel.Visible = false;
                MunisSearchButton.Visible = true;
                this.Text = "View" + FormTitle(CurrentViewDevice) + "  *MODIFYING**";
                AcceptCancelToolStrip.Visible = true;
            }
            else
            {
                EditMode = false;
                DisableControlsRecursive(this);
                MunisSibiPanel.Visible = true;
                MunisSearchButton.Visible = false;
                this.Text = DefaultFormTitle;
                AcceptCancelToolStrip.Visible = false;
                TrackingToolStrip.Visible = false;
            }
        }

        private void DisableControlsRecursive(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = (TextBox)c;
                    txt.ReadOnly = true;
                }
                else if (c is MaskedTextBox)
                {
                    MaskedTextBox txt = (MaskedTextBox)c;
                    txt.ReadOnly = true;
                }
                else if (c is ComboBox)
                {
                    ComboBox cmb = (ComboBox)c;
                    cmb.Enabled = false;
                }
                else if (c is DateTimePicker)
                {
                    DateTimePicker dtp = (DateTimePicker)c;
                    dtp.Enabled = false;
                }
                else if (c is CheckBox)
                {
                    c.Enabled = false;
                }
                else if (c is Label)
                {
                    //do nut-zing
                }
                if (c.HasChildren)
                {
                    DisableControlsRecursive(c);
                }
            }
        }

        private void DisableSorting(DataGridView Grid)
        {
            foreach (DataGridViewColumn c in Grid.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
        }

        private void DoneWaiting()
        {
            OtherFunctions.SetWaitCursor(false, this);
        }

        private void EnableControlsRecursive(Control control)
        {
            foreach (Control c in control.Controls)
            {
                if (c is TextBox)
                {
                    TextBox txt = (TextBox)c;
                    txt.ReadOnly = false;
                }
                else if (c is MaskedTextBox)
                {
                    MaskedTextBox txt = (MaskedTextBox)c;
                    txt.ReadOnly = false;
                }
                else if (c is ComboBox)
                {
                    ComboBox cmb = (ComboBox)c;
                    cmb.Enabled = true;
                }
                else if (c is DateTimePicker)
                {
                    DateTimePicker dtp = (DateTimePicker)c;
                    dtp.Enabled = true;
                }
                else if (c is CheckBox)
                {
                    c.Enabled = true;
                }
                else if (c is Label)
                {
                    //do nut-zing
                }

                if (c.HasChildren)
                {
                    EnableControlsRecursive(c);
                }
            }
        }

        private void ExpandSplitter()
        {
            if (remoteToolsControl.Visible || TrackingBox.Visible)
            {
                InfoDataSplitter.Panel2Collapsed = false;
            }
            else if (!remoteToolsControl.Visible && !TrackingBox.Visible)
            {
                InfoDataSplitter.Panel2Collapsed = true;
            }
        }

        private void ExpandSplitter(bool shouldExpand)
        {
            InfoDataSplitter.Panel2Collapsed = !shouldExpand;
        }

        private void FillTrackingBox()
        {
            if (CurrentViewDevice.Tracking.IsCheckedOut)
            {
                TrackingStatusTextBox.BackColor = Colors.CheckOut;
                TrackingLocationTextBox.Text = CurrentViewDevice.Tracking.UseLocation;
                CheckTimeLabel.Text = "CheckOut Time:";
                CheckTimeTextBox.Text = CurrentViewDevice.Tracking.CheckoutTime.ToString();
                CheckUserLabel.Text = "CheckOut User:";
                CheckUserTextBox.Text = CurrentViewDevice.Tracking.CheckoutUser;
                DueBackLabel.Visible = true;
                DueBackTextBox.Visible = true;
                DueBackTextBox.Text = CurrentViewDevice.Tracking.DueBackTime.ToString();
            }
            else
            {
                TrackingStatusTextBox.BackColor = Colors.CheckIn;
                TrackingLocationTextBox.Text = AttributeFunctions.GetDisplayValueFromCode(GlobalInstances.DeviceAttribute.Locations, CurrentViewDevice.Location);
                CheckTimeLabel.Text = "CheckIn Time:";
                CheckTimeTextBox.Text = CurrentViewDevice.Tracking.CheckinTime.ToString();
                CheckUserLabel.Text = "CheckIn User:";
                CheckUserTextBox.Text = CurrentViewDevice.Tracking.CheckinUser;
                DueBackLabel.Visible = false;
                DueBackTextBox.Visible = false;
            }
            TrackingStatusTextBox.Text = (CurrentViewDevice.Tracking.IsCheckedOut ? "Checked Out" : "Checked In").ToString();
        }

        private string FormTitle(DeviceMapObject Device)
        {
            return " - " + Device.CurrentUser + " - " + Device.AssetTag + " - " + Device.Description;
        }

        private DataTable GetDevicesTable(string deviceUID)
        {
            return DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectDeviceByGUID(deviceUID));
        }

        private string GetHash(DataTable deviceTable, DataTable historicalTable)
        {
            return SecurityTools.GetSHAOfTable(deviceTable) + SecurityTools.GetSHAOfTable(historicalTable);
        }

        private DataTable GetHistoricalTable(string deviceUID)
        {
            var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectDeviceHistoricalTable(deviceUID));
            results.TableName = HistoricalDevicesCols.TableName;
            return results;
        }

        private DataTable GetInsertTable(string selectQuery, DeviceUpdateInfoStruct UpdateInfo)
        {
            var tmpTable = DataParser.ReturnInsertTable(selectQuery);
            var DBRow = tmpTable.Rows[0];
            //Add Add'l info
            DBRow[HistoricalDevicesCols.ChangeType] = UpdateInfo.ChangeType;
            DBRow[HistoricalDevicesCols.Notes] = UpdateInfo.Note;
            DBRow[HistoricalDevicesCols.ActionUser] = GlobalConstants.LocalDomainUser;
            DBRow[HistoricalDevicesCols.DeviceUID] = CurrentViewDevice.GUID;
            return tmpTable;
        }

        private DataTable GetUpdateTable(string selectQuery)
        {
            var tmpTable = DataParser.ReturnUpdateTable(selectQuery);
            var DBRow = tmpTable.Rows[0];
            //Add Add'l info
            if (MunisUser.Number != null && MunisUser.Number != string.Empty)
            {
                DBRow[DevicesCols.CurrentUser] = MunisUser.Name;
                DBRow[DevicesCols.MunisEmpNum] = MunisUser.Number;
            }
            else
            {
                if (CurrentViewDevice.CurrentUser != CurrentUserTextBox.Text.Trim())
                {
                    DBRow[DevicesCols.CurrentUser] = CurrentUserTextBox.Text.Trim();
                    DBRow[DevicesCols.MunisEmpNum] = DBNull.Value;
                }
                else
                {
                    DBRow[DevicesCols.CurrentUser] = CurrentViewDevice.CurrentUser;
                    DBRow[DevicesCols.MunisEmpNum] = CurrentViewDevice.CurrentUserEmpNum;
                }
            }
            DBRow[DevicesCols.SibiLinkUID] = DataConsistency.CleanDBValue(CurrentViewDevice.SibiLink);
            DBRow[DevicesCols.LastModUser] = GlobalConstants.LocalDomainUser;
            DBRow[DevicesCols.LastModDate] = DateTime.Now;
            MunisUser = new MunisEmployeeStruct();//null;
            return tmpTable;
        }

        private List<DataGridColumn> HistoricalGridColumns()
        {
            List<DataGridColumn> ColList = new List<DataGridColumn>();
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.ActionDateTime, "Time Stamp", typeof(DateTime)));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.ChangeType, "Change Type", GlobalInstances.DeviceAttribute.ChangeType, ColumnFormatTypes.AttributeDisplayMemberOnly));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.ActionUser, "Action User", typeof(string)));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.Notes, "Note Peek", typeof(string), ColumnFormatTypes.NotePreview));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.CurrentUser, "User", typeof(string)));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.AssetTag, "Asset ID", typeof(string)));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.Serial, "Serial", typeof(string)));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.Description, "Description", typeof(string)));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.Location, "Location", GlobalInstances.DeviceAttribute.Locations, ColumnFormatTypes.AttributeDisplayMemberOnly));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.PurchaseDate, "Purchase Date", typeof(DateTime)));
            ColList.Add(new DataGridColumn(HistoricalDevicesCols.HistoryEntryUID, "GUID", typeof(string)));
            return ColList;
        }

        private void InitDBControls()
        {
            //Required Fields
            AssetTagTextBox.Tag = new DBControlInfo(DevicesBaseCols.AssetTag, true);
            SerialTextBox.Tag = new DBControlInfo(DevicesBaseCols.Serial, true);
            CurrentUserTextBox.Tag = new DBControlInfo(DevicesBaseCols.CurrentUser, true);
            DescriptionTextBox.Tag = new DBControlInfo(DevicesBaseCols.Description, true);
            PurchaseDatePicker.Tag = new DBControlInfo(DevicesBaseCols.PurchaseDate, true);
            EquipTypeComboBox.Tag = new DBControlInfo(DevicesBaseCols.EQType, GlobalInstances.DeviceAttribute.EquipType, true);
            LocationComboBox.Tag = new DBControlInfo(DevicesBaseCols.Location, GlobalInstances.DeviceAttribute.Locations, true);
            OSVersionComboBox.Tag = new DBControlInfo(DevicesBaseCols.OSVersion, GlobalInstances.DeviceAttribute.OSType, true);
            StatusComboBox.Tag = new DBControlInfo(DevicesBaseCols.Status, GlobalInstances.DeviceAttribute.StatusType, true);

            //Non-required and Misc Fields
            PONumberTextBox.Tag = new DBControlInfo(DevicesBaseCols.PO, false);
            ReplaceYearTextBox.Tag = new DBControlInfo(DevicesBaseCols.ReplacementYear, false);
            PhoneNumberTextBox.Tag = new DBControlInfo(DevicesBaseCols.PhoneNumber, false);
            GUIDLabel.Tag = new DBControlInfo(DevicesBaseCols.DeviceUID, ParseType.DisplayOnly, false);
            TrackableCheckBox.Tag = new DBControlInfo(DevicesBaseCols.Trackable, false);
            HostnameTextBox.Tag = new DBControlInfo(DevicesBaseCols.HostName, false);
            iCloudTextBox.Tag = new DBControlInfo(DevicesBaseCols.iCloudAccount, false);
        }

        private void LoadHistoryAndFields()
        {
            using (var HistoricalResults = GetHistoricalTable(CurrentViewDevice.GUID))
            {
                CurrentHash = GetHash(CurrentViewDevice.PopulatingTable, HistoricalResults);
                DataParser.FillDBFields(CurrentViewDevice.PopulatingTable);
                SetMunisEmpStatus();
                GridFunctions.PopulateGrid(DataGridHistory, HistoricalResults, HistoricalGridColumns());
                DataGridHistory.FastAutoSizeColumns();
                UpdateAttachCountHandler(this, new EventArgs());
                SetADInfo();
                remoteToolsControl.Device = CurrentViewDevice;
            }
        }

        private void LoadTracking(string deviceGUID)
        {
            using (DataTable Results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectTrackingByDevGUID(deviceGUID)))
            {
                if (Results.Rows.Count > 0)
                {
                    CollectCurrentTracking(Results);
                    GridFunctions.PopulateGrid(TrackingGrid, Results, TrackingGridColumns());
                    TrackingGrid.Columns[TrackablesCols.CheckType].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    TrackingGrid.Columns[TrackablesCols.CheckType].DefaultCellStyle.Font = new Font(TrackingGrid.Font, FontStyle.Bold);
                    DisableSorting(TrackingGrid);
                }
                else
                {
                    TrackingGrid.DataSource = null;
                }
                FillTrackingBox();
            }
        }

        private void ModifyDevice()
        {
            if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifyDevice))
            {
                return;
            }
            SetEditMode(true);
        }

        private void NewEntryView()
        {
            string entryUID = GridFunctions.GetCurrentCellValue(DataGridHistory, HistoricalDevicesCols.HistoryEntryUID);
            if (!Helpers.ChildFormControl.FormIsOpenByUID(typeof(ViewHistoryForm), entryUID))
            {
                Waiting();
                ViewHistoryForm NewEntry = new ViewHistoryForm(this, entryUID, CurrentViewDevice.GUID);
                DoneWaiting();
            }
        }

        private void NewTrackingView(string GUID)
        {
            Waiting();
            ViewTrackingForm NewTracking = new ViewTrackingForm(this, GUID, CurrentViewDevice);
            DoneWaiting();
        }

        private void OpenSibiLink(DeviceMapObject LinkDevice)
        {
            try
            {
                if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ViewSibi))
                {
                    return;
                }
                if (string.IsNullOrEmpty(LinkDevice.PO))
                {
                    OtherFunctions.Message("A valid PO Number is required.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Missing Info", this);
                    return;
                }
                else
                {
                    string SibiUID = GlobalInstances.AssetFunc.GetSqlValue(SibiRequestCols.TableName, SibiRequestCols.PO, LinkDevice.PO, SibiRequestCols.UID);

                    if (string.IsNullOrEmpty(SibiUID))
                    {
                        OtherFunctions.Message("No Sibi request found with matching PO number.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Not Found", this);
                    }
                    else
                    {
                        if (!Helpers.ChildFormControl.FormIsOpenByUID(typeof(SibiManageRequestForm), SibiUID))
                        {
                            SibiManageRequestForm NewRequest = new SibiManageRequestForm(this, SibiUID);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void RefreshCombos()
        {
            AttributeFunctions.FillComboBox(GlobalInstances.DeviceAttribute.EquipType, EquipTypeComboBox);
            AttributeFunctions.FillComboBox(GlobalInstances.DeviceAttribute.Locations, LocationComboBox);
            AttributeFunctions.FillComboBox(GlobalInstances.DeviceAttribute.OSType, OSVersionComboBox);
            AttributeFunctions.FillComboBox(GlobalInstances.DeviceAttribute.StatusType, StatusComboBox);
        }

        private void ResetBackColors()
        {
            foreach (Control c in DataParser.GetDBControls(this))
            {
                c.BackColor = Color.Empty;
            }
        }

        private async void SetADInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(CurrentViewDevice.HostName))
                {
                    ActiveDirectoryWrapper ADWrap = new ActiveDirectoryWrapper(CurrentViewDevice.HostName);
                    if (await ADWrap.LoadResultsAsync())
                    {
                        ADOUTextBox.Text = ADWrap.GetDeviceOU();
                        ADOSTextBox.Text = ADWrap.GetAttributeValue("operatingsystem");
                        ADOSVerTextBox.Text = ADWrap.GetAttributeValue("operatingsystemversion");
                        ADLastLoginTextBox.Text = ADWrap.GetAttributeValue("lastlogon");
                        ADCreatedTextBox.Text = ADWrap.GetAttributeValue("whencreated");
                        ActiveDirectoryBox.Visible = true;
                        return;
                    }
                }
                ActiveDirectoryBox.Visible = false;
            }
            catch
            {
                ActiveDirectoryBox.Visible = false;
            }
        }

        private void SetMunisEmpStatus()
        {
            ToolTip1.SetToolTip(CurrentUserTextBox, "");
            if (!string.IsNullOrEmpty(CurrentViewDevice.CurrentUserEmpNum))
            {
                CurrentUserTextBox.BackColor = Colors.EditColor;
                ToolTip1.SetToolTip(CurrentUserTextBox, "Munis Linked Employee");
            }
        }

        private void SetStatusBar(string text)
        {
            if (StatusStrip1.InvokeRequired)
            {
                StatusVoidDelegate d = new StatusVoidDelegate(SetStatusBar);
                StatusStrip1.Invoke(d, new object[] { text });
            }
            else
            {
                // StatusLabel.Text = text
                StatusSlider.SlideText = text;
                StatusStrip1.Update();
            }
        }

        private void SetTracking(bool isTrackable, bool isCheckedOut)
        {
            if (isTrackable)
            {
                if (!TabControl1.TabPages.Contains(TrackingTab))
                {
                    TabControl1.TabPages.Insert(1, TrackingTab);
                }
                ExpandSplitter(true);
                TrackingBox.Visible = true;
                TrackingToolStrip.Visible = isTrackable;
                CheckOutTool.Visible = !isCheckedOut;
                CheckInTool.Visible = isCheckedOut;
            }
            else
            {
                TrackingToolStrip.Visible = isTrackable;
                TabControl1.TabPages.Remove(TrackingTab);
                TrackingBox.Visible = false;
                ExpandSplitter();
            }
            StyleFunctions.SetGridStyle(DataGridHistory, GridTheme);
            StyleFunctions.SetGridStyle(TrackingGrid, GridTheme);
        }

        private void StartTrackDeviceForm()
        {
            if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.Tracking))
            {
                return;
            }
            Waiting();
            TrackDeviceForm NewTracking = new TrackDeviceForm(CurrentViewDevice, this);
            DoneWaiting();
        }

        private List<DataGridColumn> TrackingGridColumns()
        {
            List<DataGridColumn> ColList = new List<DataGridColumn>();
            ColList.Add(new DataGridColumn(TrackablesCols.DateStamp, "Date", typeof(DateTime)));
            ColList.Add(new DataGridColumn(TrackablesCols.CheckType, "Check Type", typeof(string)));
            ColList.Add(new DataGridColumn(TrackablesCols.CheckoutUser, "Check Out User", typeof(string)));
            ColList.Add(new DataGridColumn(TrackablesCols.CheckinUser, "Check In User", typeof(string)));
            ColList.Add(new DataGridColumn(TrackablesCols.CheckoutTime, "Check Out", typeof(DateTime)));
            ColList.Add(new DataGridColumn(TrackablesCols.CheckinTime, "Check In", typeof(DateTime)));
            ColList.Add(new DataGridColumn(TrackablesCols.DueBackDate, "Due Back", typeof(DateTime)));
            ColList.Add(new DataGridColumn(TrackablesCols.UseLocation, "Location", typeof(string)));
            ColList.Add(new DataGridColumn(TrackablesCols.UID, "GUID", typeof(string)));
            return ColList;
        }

        private void UpdateDevice(DeviceUpdateInfoStruct UpdateInfo)
        {
            SetEditMode(false);
            int affectedRows = 0;
            string SelectQry = Queries.SelectDeviceByGUID(CurrentViewDevice.GUID);
            string InsertQry = Queries.SelectEmptyHistoricalTable;
            using (var trans = DBFactory.GetDatabase().StartTransaction())
            {
                using (var conn = trans.Connection)
                {
                    try
                    {
                        affectedRows += DBFactory.GetDatabase().UpdateTable(SelectQry, GetUpdateTable(SelectQry), trans);
                        affectedRows += DBFactory.GetDatabase().UpdateTable(InsertQry, GetInsertTable(InsertQry, UpdateInfo), trans);

                        if (affectedRows == 2)
                        {
                            trans.Commit();
                            RefreshData();
                            SetStatusBar("Update successful!");
                        }
                        else
                        {
                            trans.Rollback();
                            RefreshData();
                            OtherFunctions.Message("Unsuccessful! The number of affected rows was not what was expected.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Unexpected Result", this);
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        if (ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod()))
                        {
                            RefreshData();
                        }
                    }
                }
            }
        }

        private void ViewAttachments()
        {
            if (!SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ViewAttachment))
            {
                return;
            }
            if (!Helpers.ChildFormControl.AttachmentsIsOpen(this))
            {
                AttachmentsForm NewAttachments = new AttachmentsForm(this, new DeviceAttachmentsCols(), CurrentViewDevice, UpdateAttachCountHandler);
            }
        }

        private void Waiting()
        {
            OtherFunctions.SetWaitCursor(true, this);
        }

        #region Control Events
        private void PingHistLabel_Click(object sender, EventArgs e)
        {
            GlobalInstances.AssetFunc.ShowPingHistory(this, CurrentViewDevice);
        }
        private void AssetDisposalFormToolItem_Click(object sender, EventArgs e)
        {
            PdfFormFilling PDFForm = new PdfFormFilling(this, CurrentViewDevice, PdfFormType.DisposeForm);
        }

        private void AttachmentsToolButton_Click(object sender, EventArgs e)
        {
            ViewAttachments();
        }

        private void CheckInTool_Click(object sender, EventArgs e)
        {
            StartTrackDeviceForm();
        }

        private void CheckOutTool_Click(object sender, EventArgs e)
        {
            StartTrackDeviceForm();
        }

        private void AcceptToolButton_Click(object sender, EventArgs e)
        {
            AcceptChanges();
        }

        private void CancelToolButton_Click(object sender, EventArgs e)
        {
            CancelModify();
        }

        private void MunisInfoButton_Click(object sender, EventArgs e)
        {
            try
            {
                GlobalInstances.MunisFunc.LoadMunisInfoByDevice(CurrentViewDevice, this);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void MunisSearchButton_Click(object sender, EventArgs e)
        {
            MunisUser = GlobalInstances.MunisFunc.MunisUserSearch(this);
            if (!string.IsNullOrEmpty(MunisUser.Name))
            {
                CurrentUserTextBox.Text = MunisUser.Name;
                CurrentUserTextBox.ReadOnly = true;
            }
        }

        private void SibiViewButton_Click(object sender, EventArgs e)
        {
            OpenSibiLink(CurrentViewDevice);
        }

        private void DataGridHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            NewEntryView();
        }

        private void DataGridHistory_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!GridFilling)
            {
                StyleFunctions.HighlightRow(DataGridHistory, GridTheme, e.RowIndex);
            }
        }

        private void DataGridHistory_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(DataGridHistory, e.RowIndex);
        }

        private void DataGridHistory_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                DataGridHistory.CurrentCell = DataGridHistory[e.ColumnIndex, e.RowIndex];
            }
        }

        private void DeleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedHistoricalEntry();
        }

        private void GUIDLabel_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(GUIDLabel.Text);
            OtherFunctions.Message("GUID Copied to clipboard.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Clipboard", this);
        }

        private void RefreshToolButton_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void TabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            TrackingGrid.Refresh();
        }

        private void TrackingGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var EntryUID = GridFunctions.GetCurrentCellValue(TrackingGrid, TrackablesCols.UID);
            if (!Helpers.ChildFormControl.FormIsOpenByUID(typeof(ViewTrackingForm), EntryUID))
            {
                NewTrackingView(EntryUID);
            }
        }

        private void TrackingGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            int CheckTypeColIndex = GridFunctions.GetColIndex(TrackingGrid, TrackablesCols.CheckType);
            string CheckTypeValue = TrackingGrid.Rows[e.RowIndex].Cells[CheckTypeColIndex].Value.ToString();
            DataGridViewCell CheckTypeCell = TrackingGrid.Rows[e.RowIndex].Cells[CheckTypeColIndex];
            CheckTypeCell.Style.ForeColor = Color.Black;
            if (CheckTypeValue == CheckType.Checkin)
            {
                CheckTypeCell.Style.BackColor = Colors.CheckIn;
            }
            else if (CheckTypeValue == CheckType.Checkout)
            {
                CheckTypeCell.Style.BackColor = Colors.CheckOut;
            }
        }

        private void DeleteDeviceToolButton_Click(object sender, EventArgs e)
        {
            DeleteDevice();
        }

        private void ModifyToolButton_Click(object sender, EventArgs e)
        {
            ModifyDevice();
        }

        private void NewNoteToolButton_Click(object sender, EventArgs e)
        {
            AddNewNote();
        }

        private void AssetInputFormToolItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CurrentViewDevice.PO))
            {
                PdfFormFilling PDFForm = new PdfFormFilling(this, CurrentViewDevice, PdfFormType.InputForm);
            }
            else
            {
                OtherFunctions.Message("Please add a valid PO number to this device.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Missing Info", this);
            }
        }

        private void AssetTransferFormToolItem_Click(object sender, EventArgs e)
        {
            PdfFormFilling PDFForm = new PdfFormFilling(this, CurrentViewDevice, PdfFormType.TransferForm);
        }

        private void PhoneNumberTextBox_Leave(object sender, EventArgs e)
        {
            if (PhoneNumberTextBox.Text.Trim() != "" && !DataConsistency.ValidPhoneNumber(PhoneNumberTextBox.Text))
            {
                OtherFunctions.Message("Invalid phone number.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Error", this);
            }
        }

        private void ViewDeviceForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Helpers.ChildFormControl.MinimizeChildren(this);
            }
        }

        private void ViewDeviceForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!OKToClose())
            {
                e.Cancel = true;
            }
        }

        private void ViewDeviceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            MyWindowList.Dispose();
            MyLiveBox.Dispose();
            MyMunisToolBar.Dispose();
            CurrentViewDevice.Dispose();
            Helpers.ChildFormControl.CloseChildren(this);
        }

        private void remoteToolsControl_VisibleChanging(object sender, bool e)
        {
            if (e)
            {
                ExpandSplitter(true);
            }
            else
            {
                ExpandSplitter();
            }
        }

        private void remoteToolsControl_NewStatusPrompt(object sender, CustomControls.RemoteTools.RemoteToolsControl.StatusPrompt e)
        {
            StatusSlider.NewSlideMessage(e.Message, e.DisplayTime);
        }

        #endregion Control Events

        #endregion Methods
              
    }
}