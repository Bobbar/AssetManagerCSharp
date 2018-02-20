using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.Sibi;
using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Data.Communications;
using AssetManager.Data;
using AssetManager.Helpers;
using AssetManager.Tools;
using AssetManager.Security;
using AssetManager.Business;
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

        private MunisEmployee munisUser = new MunisEmployee();

        public MunisEmployee MunisUser
        {
            get { return munisUser; }

            set
            {
                munisUser = value;
                SetMunisEmpStatus();
            }
        }
        private bool gridFilling = false;
        private string currentHash;
        private Device currentViewDevice;
        private DBControlParser controlParser;
        private bool editMode = false;
        private LiveBox liveBox;
        private MunisToolBar munisToolBar;
        private WindowList windowList;
        private SliderLabel statusSlider;

        #endregion Fields

        #region Delegates

        private delegate void StatusVoidDelegate(string text);

        #endregion Delegates

        #region Constructors

        public ViewDeviceForm(ExtendedForm parentForm, MappableObject device) : base(parentForm, device)
        {
            currentViewDevice = (Device)device;

            InitializeComponent();
            InitDBControls();

            controlParser = new DBControlParser(this);
            controlParser.EnableFieldValidation();

            DefaultFormTitle = this.Text;

            liveBox = new LiveBox(this);
            liveBox.AttachToControl(CurrentUserTextBox, DevicesCols.CurrentUser, LiveBox.LiveBoxType.UserSelect, DevicesCols.MunisEmpNum);
            liveBox.AttachToControl(DescriptionTextBox, DevicesCols.Description, LiveBox.LiveBoxType.SelectValue);

            munisToolBar = new MunisToolBar(this);
            munisToolBar.InsertMunisDropDown(ToolStrip1, 6);

            windowList = new WindowList(this);
            windowList.InsertWindowList(ToolStrip1);

            statusSlider = new SliderLabel();
            StatusStrip1.Items.Add(statusSlider.ToToolStripControl(StatusStrip1));

            ImageCaching.CacheControlImages(this);

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
            bool hasPingHist = await AssetManagerFunctions.HasPingHistory(currentViewDevice);
            PingHistLabel.Visible = hasPingHist;
        }

        public void LoadDevice()
        {
            try
            {
                gridFilling = true;
                LoadHistoryAndFields();
                if (currentViewDevice.IsTrackable)
                {
                    LoadTracking(currentViewDevice.GUID);
                }
                SetPingHistoryLink();
                SetTracking(currentViewDevice.IsTrackable, currentViewDevice.Tracking.IsCheckedOut);
                this.Text = this.DefaultFormTitle + FormTitle(currentViewDevice);
                this.Show();
                DataGridHistory.ClearSelection();
                gridFilling = false;
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
            if (editMode && !CancelModify())
            {
                CanClose = false;
            }
            return CanClose;
        }

        public override void RefreshData()
        {
            if (editMode)
            {
                CancelModify();
            }
            else
            {
                ActiveDirectoryBox.Visible = false;
                remoteToolsControl.Visible = false;
                currentViewDevice = new Device(currentViewDevice.GUID);
                LoadDevice();
            }
        }

        public void UpdateAttachCountHandler(object sender, EventArgs e)
        {
            AssetManagerFunctions.SetAttachmentCount(AttachmentsToolButton, currentViewDevice.GUID, new DeviceAttachmentsCols());
        }

        private void AcceptChanges()
        {
            try
            {
                if (!FieldsValid())
                {
                    OtherFunctions.Message("Some required fields are missing or invalid.  Please check and fill all highlighted fields.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Data", this);
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

        private void AddNewNote()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifyDevice);

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
            if (editMode)
            {
                this.WindowState = FormWindowState.Normal;
                this.Activate();
                var blah = OtherFunctions.Message("Are you sure you want to discard all changes?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Discard Changes?", this);
                if (blah == DialogResult.Yes)
                {
                    controlParser.ClearErrors();
                    SetEditMode(false);
                    RefreshData();
                    return true;
                }
            }
            return false;
        }

        private bool FieldsValid()
        {
            bool fieldsValid = true;
            fieldsValid = controlParser.ValidateFields();
            if (!DataConsistency.ValidPhoneNumber(PhoneNumberTextBox.Text))
            {
                fieldsValid = false;
                controlParser.SetError(PhoneNumberTextBox, false);
            }
            else
            {
                controlParser.SetError(PhoneNumberTextBox, true);
            }
            return fieldsValid; //if fields are missing return false to trigger a message if needed
        }

        private void CollectCurrentTracking(DataTable results)
        {
            currentViewDevice.Tracking.MapClassProperties(results);
        }

        private bool ConcurrencyCheck()
        {
            using (var DeviceResults = GetDevicesTable(currentViewDevice.GUID))
            {
                using (var HistoricalResults = GetHistoricalTable(currentViewDevice.GUID))
                {
                    DeviceResults.TableName = DevicesCols.TableName;
                    HistoricalResults.TableName = HistoricalDevicesCols.TableName;
                    var DBHash = GetHash(DeviceResults, HistoricalResults);
                    if (DBHash != currentHash)
                    {
                        OtherFunctions.Message("This record appears to have been modified by someone else since the start of this modification.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Concurrency Error", this);
                        return false;
                    }
                    return true;
                }
            }
        }

        private void DeleteDevice()
        {
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.DeleteDevice);

            var blah = OtherFunctions.Message("Are you absolutely sure?  This cannot be undone and will delete all historical data, tracking and attachments.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, "WARNING", this);
            if (blah == DialogResult.Yes)
            {
                if (AssetManagerFunctions.DeleteDevice(currentViewDevice.GUID))
                {
                    OtherFunctions.Message("Device deleted successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Device Deleted", this);
                    currentViewDevice = null;
                    Helpers.ChildFormControl.MainFormInstance().RefreshData();
                }
                else
                {
                    Logging.Logger("*****DELETION ERROR******: " + currentViewDevice.GUID);
                    OtherFunctions.Message("Failed to delete device succesfully!  Please let Bobby Lovell know about this.", MessageBoxButtons.OK, MessageBoxIcon.Error, "Delete Failed", this);
                    currentViewDevice = null;
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
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifyDevice);

            try
            {
                string entryGUID = DataGridHistory.CurrentRowStringValue(HistoricalDevicesCols.HistoryEntryUID);
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectHistoricalDeviceEntry(entryGUID)))
                {
                    string dateStamp = results.Rows[0][HistoricalDevicesCols.ActionDateTime].ToString();
                    string actionType = AttributeFunctions.GetDisplayValueFromCode(GlobalInstances.DeviceAttribute.ChangeType, results.Rows[0][HistoricalDevicesCols.ChangeType].ToString());
                    var blah = OtherFunctions.Message("Are you sure you want to delete this entry?  This cannot be undone!" + "\r\n" + "\r\n" + "Entry info: " + dateStamp + " - " + actionType + " - " + entryGUID, MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, "Are you sure?", this);
                    if (blah == DialogResult.Yes)
                    {
                        int affectedRows = DBFactory.GetDatabase().ExecuteNonQuery(Queries.DeleteHistoricalEntryByGUID(entryGUID));
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
                this.editMode = true;
                EnableControlsRecursive(this);
                SetMunisEmpStatus();
                ActiveDirectoryBox.Visible = false;
                MunisSibiPanel.Visible = false;
                MunisSearchButton.Visible = true;
                this.Text = "View" + FormTitle(currentViewDevice) + "  *MODIFYING**";
                AcceptCancelToolStrip.Visible = true;
            }
            else
            {
                this.editMode = false;
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
            if (currentViewDevice.Tracking.IsCheckedOut)
            {
                TrackingStatusTextBox.BackColor = Colors.CheckOut;
                TrackingLocationTextBox.Text = currentViewDevice.Tracking.UseLocation;
                CheckTimeLabel.Text = "CheckOut Time:";
                CheckTimeTextBox.Text = currentViewDevice.Tracking.CheckoutTime.ToString();
                CheckUserLabel.Text = "CheckOut User:";
                CheckUserTextBox.Text = currentViewDevice.Tracking.CheckoutUser;
                DueBackLabel.Visible = true;
                DueBackTextBox.Visible = true;
                DueBackTextBox.Text = currentViewDevice.Tracking.DueBackTime.ToString();
            }
            else
            {
                TrackingStatusTextBox.BackColor = Colors.CheckIn;
                TrackingLocationTextBox.Text = AttributeFunctions.GetDisplayValueFromCode(GlobalInstances.DeviceAttribute.Locations, currentViewDevice.Location);
                CheckTimeLabel.Text = "CheckIn Time:";
                CheckTimeTextBox.Text = currentViewDevice.Tracking.CheckinTime.ToString();
                CheckUserLabel.Text = "CheckIn User:";
                CheckUserTextBox.Text = currentViewDevice.Tracking.CheckinUser;
                DueBackLabel.Visible = false;
                DueBackTextBox.Visible = false;
            }
            TrackingStatusTextBox.Text = (currentViewDevice.Tracking.IsCheckedOut ? "Checked Out" : "Checked In").ToString();
        }

        private string FormTitle(Device Device)
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

        private DataTable GetInsertTable(string selectQuery, DeviceUpdateInfo UpdateInfo)
        {
            var tmpTable = controlParser.ReturnInsertTable(selectQuery);
            var DBRow = tmpTable.Rows[0];
            //Add Add'l info
            DBRow[HistoricalDevicesCols.ChangeType] = UpdateInfo.ChangeType;
            DBRow[HistoricalDevicesCols.Notes] = UpdateInfo.Note;
            DBRow[HistoricalDevicesCols.ActionUser] = NetworkInfo.LocalDomainUser;
            DBRow[HistoricalDevicesCols.DeviceUID] = currentViewDevice.GUID;
            return tmpTable;
        }

        private DataTable GetUpdateTable(string selectQuery)
        {
            var tmpTable = controlParser.ReturnUpdateTable(selectQuery);
            var DBRow = tmpTable.Rows[0];
            //Add Add'l info
            if (MunisUser.Number != null && MunisUser.Number != string.Empty)
            {
                DBRow[DevicesCols.CurrentUser] = MunisUser.Name;
                DBRow[DevicesCols.MunisEmpNum] = MunisUser.Number;
            }
            else
            {
                if (currentViewDevice.CurrentUser != CurrentUserTextBox.Text.Trim())
                {
                    DBRow[DevicesCols.CurrentUser] = CurrentUserTextBox.Text.Trim();
                    DBRow[DevicesCols.MunisEmpNum] = DBNull.Value;
                }
                else
                {
                    DBRow[DevicesCols.CurrentUser] = currentViewDevice.CurrentUser;
                    DBRow[DevicesCols.MunisEmpNum] = currentViewDevice.CurrentUserEmpNum;
                }
            }
            DBRow[DevicesCols.SibiLinkUID] = DataConsistency.CleanDBValue(currentViewDevice.SibiLink);
            DBRow[DevicesCols.LastModUser] = NetworkInfo.LocalDomainUser;
            DBRow[DevicesCols.LastModDate] = DateTime.Now;
            return tmpTable;
        }

        private List<GridColumnAttrib> HistoricalGridColumns()
        {
            List<GridColumnAttrib> ColList = new List<GridColumnAttrib>();
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.ActionDateTime, "Time Stamp", typeof(DateTime)));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.ChangeType, "Change Type", GlobalInstances.DeviceAttribute.ChangeType, ColumnFormatTypes.AttributeDisplayMemberOnly));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.ActionUser, "Action User", typeof(string)));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.Notes, "Note Peek", typeof(string), ColumnFormatTypes.NotePreview));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.CurrentUser, "User", typeof(string)));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.AssetTag, "Asset ID", typeof(string)));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.Serial, "Serial", typeof(string)));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.Description, "Description", typeof(string)));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.Location, "Location", GlobalInstances.DeviceAttribute.Locations, ColumnFormatTypes.AttributeDisplayMemberOnly));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.PurchaseDate, "Purchase Date", typeof(DateTime)));
            ColList.Add(new GridColumnAttrib(HistoricalDevicesCols.HistoryEntryUID, "GUID", typeof(string)));
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
            using (var HistoricalResults = GetHistoricalTable(currentViewDevice.GUID))
            {
                currentHash = GetHash(currentViewDevice.PopulatingTable, HistoricalResults);
                controlParser.FillDBFields(currentViewDevice.PopulatingTable);
                MunisUser = new MunisEmployee(currentViewDevice.CurrentUser, currentViewDevice.CurrentUserEmpNum);
                SetMunisEmpStatus();

                DataGridHistory.SuspendLayout();
                DataGridHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                DataGridHistory.ColumnHeadersHeight = 38;
                DataGridHistory.Populate(HistoricalResults, HistoricalGridColumns());
                DataGridHistory.FastAutoSizeColumns();
                DataGridHistory.ResumeLayout();

                UpdateAttachCountHandler(this, new EventArgs());
                SetADInfo();
                remoteToolsControl.Device = currentViewDevice;
            }
        }

        private void LoadTracking(string deviceGUID)
        {
            using (DataTable Results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectTrackingByDevGUID(deviceGUID)))
            {
                if (Results.Rows.Count > 0)
                {
                    CollectCurrentTracking(Results);
                    TrackingGrid.Populate(Results, TrackingGridColumns());
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
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ModifyDevice);

            SetEditMode(true);
        }

        private void NewEntryView()
        {
            string entryUID = DataGridHistory.CurrentRowStringValue(HistoricalDevicesCols.HistoryEntryUID);
            if (!Helpers.ChildFormControl.FormIsOpenByUID(typeof(ViewHistoryForm), entryUID))
            {
                Waiting();
                ViewHistoryForm NewEntry = new ViewHistoryForm(this, entryUID, currentViewDevice.GUID);
                DoneWaiting();
            }
        }

        private void NewTrackingView(string GUID)
        {
            Waiting();
            ViewTrackingForm NewTracking = new ViewTrackingForm(this, GUID, currentViewDevice);
            DoneWaiting();
        }

        private void OpenSibiLink(Device LinkDevice)
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ViewSibi);
                
                if (string.IsNullOrEmpty(LinkDevice.PO))
                {
                    OtherFunctions.Message("A valid PO Number is required.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Missing Info", this);
                    return;
                }
                else
                {
                    string SibiUID = AssetManagerFunctions.GetSqlValue(SibiRequestCols.TableName, SibiRequestCols.PO, LinkDevice.PO, SibiRequestCols.UID);

                    if (string.IsNullOrEmpty(SibiUID))
                    {
                        OtherFunctions.Message("No Sibi request found with matching PO number.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Not Found", this);
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

        private async void SetADInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(currentViewDevice.HostName))
                {
                    ActiveDirectoryWrapper ADWrap = new ActiveDirectoryWrapper(currentViewDevice.HostName);
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
            ToolTip1.SetToolTip(CurrentUserTextBox, string.Empty);
            if (MunisUser != null)
            {
                CurrentUserTextBox.Text = MunisUser.Name;

                if (!string.IsNullOrEmpty(MunisUser.Number))
                {
                    if (editMode)
                    {
                        CurrentUserTextBox.ReadOnly = true;
                        CurrentUserTextBox.BackColor = Colors.EditColor;
                        ToolTip1.SetToolTip(CurrentUserTextBox, "Double-Click to change.");
                    }
                    else
                    {
                        CurrentUserTextBox.ReadOnly = true;
                        CurrentUserTextBox.BackColor = Colors.EditColor;
                        ToolTip1.SetToolTip(CurrentUserTextBox, "Munis Linked Employee");
                    }
                    controlParser.ErrorProvider.SetError(CurrentUserTextBox, string.Empty);
                }
                else
                {
                    if (editMode)
                    {
                        CurrentUserTextBox.ReadOnly = false;
                        CurrentUserTextBox.BackColor = Color.Empty;
                    }
                    else
                    {
                        CurrentUserTextBox.ReadOnly = true;
                        CurrentUserTextBox.BackColor = Color.Empty;
                    }
                }
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
                statusSlider.SlideText = text;
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
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.Tracking);
           
            Waiting();
            TrackDeviceForm NewTracking = new TrackDeviceForm(currentViewDevice, this);
            DoneWaiting();
        }

        private List<GridColumnAttrib> TrackingGridColumns()
        {
            List<GridColumnAttrib> ColList = new List<GridColumnAttrib>();
            ColList.Add(new GridColumnAttrib(TrackablesCols.DateStamp, "Date", typeof(DateTime)));
            ColList.Add(new GridColumnAttrib(TrackablesCols.CheckType, "Check Type", typeof(string)));
            ColList.Add(new GridColumnAttrib(TrackablesCols.CheckoutUser, "Check Out User", typeof(string)));
            ColList.Add(new GridColumnAttrib(TrackablesCols.CheckinUser, "Check In User", typeof(string)));
            ColList.Add(new GridColumnAttrib(TrackablesCols.CheckoutTime, "Check Out", typeof(DateTime)));
            ColList.Add(new GridColumnAttrib(TrackablesCols.CheckinTime, "Check In", typeof(DateTime)));
            ColList.Add(new GridColumnAttrib(TrackablesCols.DueBackDate, "Due Back", typeof(DateTime)));
            ColList.Add(new GridColumnAttrib(TrackablesCols.UseLocation, "Location", typeof(string)));
            ColList.Add(new GridColumnAttrib(TrackablesCols.UID, "GUID", typeof(string)));
            return ColList;
        }

        private void UpdateDevice(DeviceUpdateInfo UpdateInfo)
        {
            SetEditMode(false);
            int affectedRows = 0;
            string SelectQry = Queries.SelectDeviceByGUID(currentViewDevice.GUID);
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
                            OtherFunctions.Message("Unsuccessful! The number of affected rows was not what was expected.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Unexpected Result", this);
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
            SecurityTools.CheckForAccess(SecurityTools.AccessGroup.ViewAttachment);
            
            if (!Helpers.ChildFormControl.AttachmentsIsOpen(this))
            {
                AttachmentsForm NewAttachments = new AttachmentsForm(this, new DeviceAttachmentsCols(), currentViewDevice, UpdateAttachCountHandler);
            }
        }

        private void Waiting()
        {
            OtherFunctions.SetWaitCursor(true, this);
        }

        #region Control Events
        private void PingHistLabel_Click(object sender, EventArgs e)
        {
            AssetManagerFunctions.ShowPingHistory(this, currentViewDevice);
        }
        private void AssetDisposalFormToolItem_Click(object sender, EventArgs e)
        {
            PdfFormFilling PDFForm = new PdfFormFilling(this, currentViewDevice, PdfFormFilling.PdfFormType.DisposeForm);
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
                MunisFunctions.LoadMunisInfoByDevice(currentViewDevice, this);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void MunisSearchButton_Click(object sender, EventArgs e)
        {
            MunisUser = MunisFunctions.MunisUserSearch(this);
        }

        private void SibiViewButton_Click(object sender, EventArgs e)
        {
            OpenSibiLink(currentViewDevice);
        }

        private void DataGridHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            NewEntryView();
        }

        private void DataGridHistory_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!gridFilling)
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
            OtherFunctions.Message("GUID Copied to clipboard.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Clipboard", this);
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
            var EntryUID = TrackingGrid.CurrentRowStringValue(TrackablesCols.UID);
            if (!Helpers.ChildFormControl.FormIsOpenByUID(typeof(ViewTrackingForm), EntryUID))
            {
                NewTrackingView(EntryUID);
            }
        }

        private void TrackingGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            string checkTypeValue = TrackingGrid.Rows[e.RowIndex].Cells[TrackablesCols.CheckType].Value.ToString();
            DataGridViewCell CheckTypeCell = TrackingGrid.Rows[e.RowIndex].Cells[TrackablesCols.CheckType];
            CheckTypeCell.Style.ForeColor = Color.Black;
            if (checkTypeValue == CheckType.Checkin)
            {
                CheckTypeCell.Style.BackColor = Colors.CheckIn;
            }
            else if (checkTypeValue == CheckType.Checkout)
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
            if (!string.IsNullOrEmpty(currentViewDevice.PO))
            {
                PdfFormFilling PDFForm = new PdfFormFilling(this, currentViewDevice, PdfFormFilling.PdfFormType.InputForm);
            }
            else
            {
                OtherFunctions.Message("Please add a valid PO number to this device.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Info", this);
            }
        }

        private void AssetTransferFormToolItem_Click(object sender, EventArgs e)
        {
            PdfFormFilling PDFForm = new PdfFormFilling(this, currentViewDevice, PdfFormFilling.PdfFormType.TransferForm);
        }

        private void PhoneNumberTextBox_Leave(object sender, EventArgs e)
        {
            if (PhoneNumberTextBox.Text.Trim() != "" && !DataConsistency.ValidPhoneNumber(PhoneNumberTextBox.Text))
            {
                OtherFunctions.Message("Invalid phone number.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
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
            windowList.Dispose();
            liveBox.Dispose();
            munisToolBar.Dispose();
            currentViewDevice.Dispose();
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
                ExpandSplitter(TrackingBox.Visible);
            }
        }

        private void remoteToolsControl_NewStatusPrompt(object sender, RemoteToolsControl.StatusPrompt e)
        {
            statusSlider.NewSlideMessage(e.Message, e.DisplayTime);
        }
        private void CurrentUserTextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (editMode)
            {
                MunisUser = new MunisEmployee();
            }
        }
        #endregion Control Events

        #endregion Methods
    }
}