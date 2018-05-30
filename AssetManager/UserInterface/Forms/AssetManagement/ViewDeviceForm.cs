using AssetManager.Business;
using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.Tools;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.Sibi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class ViewDeviceForm : ExtendedForm, ILiveBox, IOnlineStatus
    {
        #region Fields

        private MunisEmployee munisUser = new MunisEmployee();
        private string defaultFormTitle;

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

        public event EventHandler<bool> OnlineStatusChanged;

        #endregion Fields

        #region Constructors

        public ViewDeviceForm(ExtendedForm parentForm, MappableObject device) : base(parentForm, device)
        {
            currentViewDevice = (Device)device;

            InitializeComponent();
            InitDBControls();

            controlParser = new DBControlParser(this);
            controlParser.EnableFieldValidation();

            defaultFormTitle = this.Text;

            liveBox = new LiveBox(this);
            liveBox.AttachToControl(CurrentUserTextBox, DevicesCols.CurrentUser, LiveBoxSelectAction.UserSelect, DevicesCols.MunisEmpNum);
            liveBox.AttachToControl(DescriptionTextBox, DevicesCols.Description, LiveBoxSelectAction.SelectValue);

            munisToolBar = new MunisToolBar(this);
            munisToolBar.InsertMunisDropDown(ToolStrip1, 6);

            windowList = new WindowList(this);
            windowList.InsertWindowList(ToolStrip1);

            statusSlider = new SliderLabel();
            statusSlider.NewMessageDisplayed += StatusSlider_NewMessageDisplayed;
            StatusStrip.Items.Add(statusSlider.ToToolStripControl(StatusStrip));

            RefreshCombos();

            DataGridHistory.DoubleBuffered(true);
            TrackingGrid.DoubleBuffered(true);

            SetEditMode(false);

            LoadCurrentDevice();
        }

        #endregion Constructors

        #region Methods

        public async void SetPingHistoryLink()
        {
            bool hasPingHist = await AssetManagerFunctions.HasPingHistory(currentViewDevice);
            PingHistLabel.Visible = hasPingHist;
        }

        public void LoadCurrentDevice()
        {
            try
            {
                gridFilling = true;
                LoadHistoryAndFields();
                if (currentViewDevice.IsTrackable)
                {
                    LoadTracking(currentViewDevice.Guid);
                }
                SetPingHistoryLink();
                SetTracking(currentViewDevice.IsTrackable, currentViewDevice.Tracking.IsCheckedOut);
                this.Text = this.defaultFormTitle + FormTitle(currentViewDevice);
                this.Show();
                DataGridHistory.ClearSelection();
                gridFilling = false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public override bool OkToClose()
        {
            bool canClose = true;
            if (editMode && !CancelModify())
            {
                canClose = false;
            }
            return canClose;
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
                RemoteToolsControl.Visible = false;
                currentViewDevice = new Device(currentViewDevice.Guid);
                LoadCurrentDevice();
            }
            base.RefreshData();
        }

        public void UpdateAttachCountHandler(object sender, EventArgs e)
        {
            AssetManagerFunctions.SetAttachmentCount(AttachmentsToolButton, currentViewDevice.Guid, new DeviceAttachmentsCols());
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
                using (UpdateTypeForm updateTypePrompt = new UpdateTypeForm(this))
                {
                    if (updateTypePrompt.ShowDialog(this) == DialogResult.OK)
                    {
                        if (!ConcurrencyCheck())
                        {
                            CancelModify();
                            return;
                        }
                        else
                        {
                            UpdateDevice(updateTypePrompt.UpdateInfo);
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void AddNewNote()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.ModifyDevice);

                using (var updateTypePrompt = new UpdateTypeForm(this, true))
                {
                    if (updateTypePrompt.ShowDialog(this) == DialogResult.OK)
                    {
                        if (!ConcurrencyCheck())
                        {
                            RefreshData();
                        }
                        else
                        {
                            UpdateDevice(updateTypePrompt.UpdateInfo);
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
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
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
            using (var deviceResults = GetDevicesTable(currentViewDevice.Guid))
            {
                using (var historicalResults = GetHistoricalTable(currentViewDevice.Guid))
                {
                    var dbHash = GetHash(deviceResults, historicalResults);
                    if (dbHash != currentHash)
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
            SecurityTools.CheckForAccess(SecurityGroups.DeleteDevice);

            var blah = OtherFunctions.Message("Are you absolutely sure?  This cannot be undone and will delete all historical data, tracking and attachments.", MessageBoxButtons.YesNo, MessageBoxIcon.Exclamation, "WARNING", this);
            if (blah == DialogResult.Yes)
            {
                if (AssetManagerFunctions.DeleteDevice(currentViewDevice.Guid))
                {
                    OtherFunctions.Message("Device deleted successfully.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Device Deleted", this);
                    currentViewDevice = null;
                    ParentForm.RefreshData();
                }
                else
                {
                    Logging.Logger("*****DELETION ERROR******: " + currentViewDevice.Guid);
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
            SecurityTools.CheckForAccess(SecurityGroups.ModifyDevice);

            try
            {
                string entryGuid = DataGridHistory.CurrentRowStringValue(HistoricalDevicesCols.HistoryEntryGuid);
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectHistoricalDeviceEntry(entryGuid)))
                {
                    string dateStamp = results.Rows[0][HistoricalDevicesCols.ActionDateTime].ToString();
                    string actionType = Attributes.DeviceAttributes.ChangeType[results.Rows[0][HistoricalDevicesCols.ChangeType].ToString()].DisplayValue;
                    var blah = OtherFunctions.Message("Are you sure you want to delete this entry?  This cannot be undone!" + "\r\n" + "\r\n" + "Entry info: " + dateStamp + " - " + actionType + " - " + entryGuid, MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Are you sure?", this);
                    if (blah == DialogResult.Yes)
                    {
                        int affectedRows = DBFactory.GetDatabase().ExecuteNonQuery(Queries.DeleteHistoricalEntryByGuid(entryGuid));
                        if (affectedRows > 0)
                        {
                            StatusPrompt("Entry deleted!", Color.Green);
                            RefreshData();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void SetEditMode(bool inEditMode)
        {
            if (inEditMode)
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
                this.Text = defaultFormTitle;
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
            if (RemoteToolsControl.Visible || TrackingBox.Visible)
            {
                InfoDataSplitter.Panel2Collapsed = false;
            }
            else if (!RemoteToolsControl.Visible && !TrackingBox.Visible)
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
                TrackingLocationTextBox.Text = Attributes.DeviceAttributes.Locations[currentViewDevice.Location].DisplayValue;
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

        private DataTable GetDevicesTable(string deviceGuid)
        {
            var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectDeviceByGuid(deviceGuid));
            results.TableName = DevicesCols.TableName;
            return results;
        }

        private string GetHash(DataTable deviceTable, DataTable historicalTable)
        {
            return SecurityTools.GetSHAOfTable(deviceTable) + SecurityTools.GetSHAOfTable(historicalTable);
        }

        private DataTable GetHistoricalTable(string deviceGuid)
        {
            var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectDeviceHistoricalTable(deviceGuid));
            results.TableName = HistoricalDevicesCols.TableName;
            return results;
        }

        private DataTable GetInsertTable(string selectQuery, DeviceUpdateInfo updateInfo)
        {
            var tmpTable = controlParser.ReturnInsertTable(selectQuery);
            var row = tmpTable.Rows[0];
            //Add Add'l info
            row[HistoricalDevicesCols.ChangeType] = updateInfo.ChangeType;
            row[HistoricalDevicesCols.Notes] = updateInfo.Note;
            row[HistoricalDevicesCols.ActionUser] = NetworkInfo.LocalDomainUser;
            row[HistoricalDevicesCols.DeviceGuid] = currentViewDevice.Guid;
            return tmpTable;
        }

        private DataTable GetUpdateTable(string selectQuery)
        {
            var tmpTable = controlParser.ReturnUpdateTable(selectQuery);
            var row = tmpTable.Rows[0];
            //Add Add'l info
            if (MunisUser.Number != null && MunisUser.Number != string.Empty)
            {
                row[DevicesCols.CurrentUser] = MunisUser.Name;
                row[DevicesCols.MunisEmpNum] = MunisUser.Number;
            }
            else
            {
                if (currentViewDevice.CurrentUser != CurrentUserTextBox.Text.Trim())
                {
                    row[DevicesCols.CurrentUser] = CurrentUserTextBox.Text.Trim();
                    row[DevicesCols.MunisEmpNum] = DBNull.Value;
                }
                else
                {
                    row[DevicesCols.CurrentUser] = currentViewDevice.CurrentUser;
                    row[DevicesCols.MunisEmpNum] = currentViewDevice.CurrentUserEmpNum;
                }
            }
            row[DevicesCols.SibiLinkGuid] = DataConsistency.CleanDBValue(currentViewDevice.SibiLink);
            row[DevicesCols.LastModUser] = NetworkInfo.LocalDomainUser;
            row[DevicesCols.LastModDate] = DateTime.Now;
            return tmpTable;
        }

        private List<GridColumnAttrib> HistoricalGridColumns()
        {
            var columnList = new List<GridColumnAttrib>();
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.ActionDateTime, "Time Stamp"));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.ChangeType, "Change Type", Attributes.DeviceAttributes.ChangeType, ColumnFormatType.AttributeDisplayMemberOnly));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.ActionUser, "Action User"));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.Notes, "Note Peek", ColumnFormatType.NotePreview));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.CurrentUser, "User"));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.AssetTag, "Asset ID"));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.Serial, "Serial"));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.Description, "Description"));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.Location, "Location", Attributes.DeviceAttributes.Locations, ColumnFormatType.AttributeDisplayMemberOnly));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.PurchaseDate, "Purchase Date"));
            columnList.Add(new GridColumnAttrib(HistoricalDevicesCols.HistoryEntryGuid, "Guid"));
            return columnList;
        }

        private void InitDBControls()
        {
            //Required Fields
            AssetTagTextBox.SetDBInfo(DevicesBaseCols.AssetTag, true);
            SerialTextBox.SetDBInfo(DevicesBaseCols.Serial, true);
            CurrentUserTextBox.SetDBInfo(DevicesBaseCols.CurrentUser, true);
            DescriptionTextBox.SetDBInfo(DevicesBaseCols.Description, true);
            PurchaseDatePicker.SetDBInfo(DevicesBaseCols.PurchaseDate, true);
            EquipTypeComboBox.SetDBInfo(DevicesBaseCols.EQType, Attributes.DeviceAttributes.EquipType, true);
            LocationComboBox.SetDBInfo(DevicesBaseCols.Location, Attributes.DeviceAttributes.Locations, true);
            OSVersionComboBox.SetDBInfo(DevicesBaseCols.OSVersion, Attributes.DeviceAttributes.OSType, true);
            StatusComboBox.SetDBInfo(DevicesBaseCols.Status, Attributes.DeviceAttributes.StatusType, true);

            //Non-required and Misc Fields
            PONumberTextBox.SetDBInfo(DevicesBaseCols.PO, false);
            ReplaceYearTextBox.SetDBInfo(DevicesBaseCols.ReplacementYear, false);
            PhoneNumberTextBox.SetDBInfo(DevicesBaseCols.PhoneNumber, false);
            GuidLabel.SetDBInfo(DevicesBaseCols.DeviceGuid, ParseType.DisplayOnly, false);
            TrackableCheckBox.SetDBInfo(DevicesBaseCols.Trackable, false);
            HostnameTextBox.SetDBInfo(DevicesBaseCols.HostName, false);
            iCloudTextBox.SetDBInfo(DevicesBaseCols.iCloudAccount, false);
        }

        private void LoadHistoryAndFields()
        {
            using (var historicalResults = GetHistoricalTable(currentViewDevice.Guid))
            {
                currentHash = GetHash(currentViewDevice.PopulatingTable, historicalResults);
                controlParser.FillDBFields(currentViewDevice.PopulatingTable);
                MunisUser = new MunisEmployee(currentViewDevice.CurrentUser, currentViewDevice.CurrentUserEmpNum);
                SetMunisEmpStatus();

                DataGridHistory.SuspendLayout();
                DataGridHistory.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                DataGridHistory.ColumnHeadersHeight = 38;
                DataGridHistory.Populate(historicalResults, HistoricalGridColumns());
                DataGridHistory.FastAutoSizeColumns();
                DataGridHistory.ResumeLayout();

                UpdateAttachCountHandler(this, new EventArgs());
                SetADInfo();
                RemoteToolsControl.Device = currentViewDevice;
            }
        }

        private void LoadTracking(string deviceGuid)
        {
            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectTrackingByDevGuid(deviceGuid)))
            {
                if (results.Rows.Count > 0)
                {
                    CollectCurrentTracking(results);
                    TrackingGrid.Populate(results, TrackingGridColumns());
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
            SecurityTools.CheckForAccess(SecurityGroups.ModifyDevice);
            this.RefreshData();
            SetEditMode(true);
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void NewEntryView()
        {
            string entryGuid = DataGridHistory.CurrentRowStringValue(HistoricalDevicesCols.HistoryEntryGuid);
            if (!Helpers.ChildFormControl.FormIsOpenByGuid(typeof(ViewHistoryForm), entryGuid))
            {
                Waiting();
                new ViewHistoryForm(this, entryGuid, currentViewDevice.Guid);
                DoneWaiting();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void NewTrackingView(string itemGuid)
        {
            Waiting();
            new ViewTrackingForm(this, itemGuid, currentViewDevice);
            DoneWaiting();
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void OpenSibiLink(Device device)
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.ViewSibi);

                if (string.IsNullOrEmpty(device.PO))
                {
                    OtherFunctions.Message("A valid PO Number is required.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Missing Info", this);
                    return;
                }
                else
                {
                    string sibiGuid = AssetManagerFunctions.GetSqlValue(SibiRequestCols.TableName, SibiRequestCols.PO, device.PO, SibiRequestCols.Guid);

                    if (string.IsNullOrEmpty(sibiGuid))
                    {
                        OtherFunctions.Message("No Sibi request found with matching PO number.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Not Found", this);
                    }
                    else
                    {
                        if (!Helpers.ChildFormControl.FormIsOpenByGuid(typeof(SibiManageRequestForm), sibiGuid))
                        {
                            new SibiManageRequestForm(this, sibiGuid);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void RefreshCombos()
        {
            EquipTypeComboBox.FillComboBox(Attributes.DeviceAttributes.EquipType);
            LocationComboBox.FillComboBox(Attributes.DeviceAttributes.Locations);
            OSVersionComboBox.FillComboBox(Attributes.DeviceAttributes.OSType);
            StatusComboBox.FillComboBox(Attributes.DeviceAttributes.StatusType);
        }

        private async void SetADInfo()
        {
            try
            {
                if (!string.IsNullOrEmpty(currentViewDevice.HostName))
                {
                    if (ServerInfo.CurrentDataBase == DatabaseName.vintondd)
                    {
                        if (!SecurityTools.VerifyAdminCreds("Credentials for Vinton AD"))
                        {
                            ActiveDirectoryBox.Visible = false;
                            return;
                        }
                    }

                    var activeDir = new ActiveDirectoryWrapper(currentViewDevice.HostName);
                    if (await activeDir.LoadResultsAsync())
                    {
                        ADOUTextBox.Text = activeDir.GetDeviceOU();
                        ADOSTextBox.Text = activeDir.GetAttributeValue("operatingsystem");
                        ADOSVerTextBox.Text = activeDir.GetAttributeValue("operatingsystemversion");
                        ADLastLoginTextBox.Text = activeDir.GetAttributeValue("lastlogon");
                        ADCreatedTextBox.Text = activeDir.GetAttributeValue("whencreated");
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

        private void StatusPrompt(string text)
        {
            StatusPrompt(text, Color.Black);
        }

        private void StatusPrompt(string text, Color color, int displayTime = -1)
        {
            if (StatusStrip.InvokeRequired)
            {
                var del = new Action(() => StatusPrompt(text, color));
                StatusStrip.BeginInvoke(del);
            }
            else
            {
                statusSlider.QueueMessage(text, color, displayTime);
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

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void StartTrackDeviceForm()
        {
            SecurityTools.CheckForAccess(SecurityGroups.Tracking);

            Waiting();
            new TrackDeviceForm(currentViewDevice, this);
            DoneWaiting();
        }

        private List<GridColumnAttrib> TrackingGridColumns()
        {
            var columnList = new List<GridColumnAttrib>();
            columnList.Add(new GridColumnAttrib(TrackablesCols.DateStamp, "Date"));
            columnList.Add(new GridColumnAttrib(TrackablesCols.CheckType, "Check Type"));
            columnList.Add(new GridColumnAttrib(TrackablesCols.CheckoutUser, "Check Out User"));
            columnList.Add(new GridColumnAttrib(TrackablesCols.CheckinUser, "Check In User"));
            columnList.Add(new GridColumnAttrib(TrackablesCols.CheckoutTime, "Check Out"));
            columnList.Add(new GridColumnAttrib(TrackablesCols.CheckinTime, "Check In"));
            columnList.Add(new GridColumnAttrib(TrackablesCols.DueBackDate, "Due Back"));
            columnList.Add(new GridColumnAttrib(TrackablesCols.UseLocation, "Location"));
            columnList.Add(new GridColumnAttrib(TrackablesCols.Guid, "Guid"));
            return columnList;
        }

        private void UpdateDevice(DeviceUpdateInfo UpdateInfo)
        {
            SetEditMode(false);
            int affectedRows = 0;
            string selectQuery = Queries.SelectDeviceByGuid(currentViewDevice.Guid);
            string insertQuery = Queries.SelectEmptyHistoricalTable;

            using (var trans = DBFactory.GetDatabase().StartTransaction())
            using (var conn = trans.Connection)
            {
                try
                {
                    affectedRows += DBFactory.GetDatabase().UpdateTable(selectQuery, GetUpdateTable(selectQuery), trans);
                    affectedRows += DBFactory.GetDatabase().UpdateTable(insertQuery, GetInsertTable(insertQuery, UpdateInfo), trans);

                    if (affectedRows == 2)
                    {
                        trans.Commit();
                        RefreshData();
                        StatusPrompt("Update successful!", Color.Green);
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
                    if (ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod()))
                    {
                        RefreshData();
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void ViewAttachments()
        {
            SecurityTools.CheckForAccess(SecurityGroups.ViewAttachment);

            if (!Helpers.ChildFormControl.AttachmentsIsOpen(this))
            {
                new AttachmentsForm(this, new DeviceAttachmentsCols(), currentViewDevice, UpdateAttachCountHandler);
            }
        }

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

                    windowList.Dispose();
                    liveBox.Dispose();
                    munisToolBar.Dispose();
                    currentViewDevice?.Dispose();
                    controlParser.Dispose();
                    statusSlider.NewMessageDisplayed -= StatusSlider_NewMessageDisplayed;
                    statusSlider.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion Methods

        #region Control Events

        private void StatusSlider_NewMessageDisplayed(object sender, MessageEventArgs e)
        {
            var flashColor = StyleFunctions.ColorAlphaBlend(e.Message.TextColor, Color.White);
            StatusStrip.FlashStrip(flashColor, 3);
        }

        void ILiveBox.DynamicSearch()
        {
            DynamicSearch();
        }

        void ILiveBox.ViewDevice(string deviceGuid)
        {
            LoadDevice(deviceGuid);
        }

        protected void DynamicSearch()
        {
            throw new NotImplementedException();
        }

        protected void LoadDevice(string deviceGuid)
        {
            throw new NotImplementedException();
        }

        private void AcceptToolButton_Click(object sender, EventArgs e)
        {
            AcceptChanges();
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void AssetDisposalFormToolItem_Click(object sender, EventArgs e)
        {
            new PdfFormFilling(this, currentViewDevice, PdfFormFilling.PdfFormType.DisposeForm);
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void AssetInputFormToolItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentViewDevice.PO))
            {
                new PdfFormFilling(this, currentViewDevice, PdfFormFilling.PdfFormType.InputForm);
            }
            else
            {
                OtherFunctions.Message("Please add a valid PO number to this device.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Info", this);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void AssetTransferFormToolItem_Click(object sender, EventArgs e)
        {
            new PdfFormFilling(this, currentViewDevice, PdfFormFilling.PdfFormType.TransferForm);
        }

        private void AttachmentsToolButton_Click(object sender, EventArgs e)
        {
            ViewAttachments();
        }

        private void CancelToolButton_Click(object sender, EventArgs e)
        {
            CancelModify();
        }

        private void CheckInTool_Click(object sender, EventArgs e)
        {
            StartTrackDeviceForm();
        }

        private void CheckOutTool_Click(object sender, EventArgs e)
        {
            StartTrackDeviceForm();
        }

        private void CurrentUserTextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (editMode)
            {
                MunisUser = new MunisEmployee();
            }
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

        private void DeleteDeviceToolButton_Click(object sender, EventArgs e)
        {
            DeleteDevice();
        }

        private void DeleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedHistoricalEntry();
        }

        private void GuidLabel_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(GuidLabel.Text);
            StatusPrompt("GUID copied to clipboard");
        }

        private void ModifyToolButton_Click(object sender, EventArgs e)
        {
            ModifyDevice();
        }

        private void MunisInfoButton_Click(object sender, EventArgs e)
        {
            try
            {
                MunisFunctions.LoadMunisInfoByDevice(currentViewDevice, this);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void MunisSearchButton_Click(object sender, EventArgs e)
        {
            MunisUser = MunisFunctions.MunisUserSearch(this);
        }

        private void NewNoteToolButton_Click(object sender, EventArgs e)
        {
            AddNewNote();
        }

        private void PhoneNumberTextBox_Leave(object sender, EventArgs e)
        {
            if (!DataConsistency.ValidPhoneNumber(PhoneNumberTextBox.Text))
            {
                OtherFunctions.Message("Invalid phone number.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
            }
        }

        private void PingHistLabel_Click(object sender, EventArgs e)
        {
            AssetManagerFunctions.ShowPingHistory(this, currentViewDevice);
        }

        private void RefreshToolButton_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RemoteToolsControl_HostBackOnline(object sender, EventArgs e)
        {
            TaskBarNotify.FlashWindow(this.Handle, true, true, 10);
        }

        private void RemoteToolsControl_HostOnlineStatus(object sender, bool e)
        {
            // If OnlineStatusChanged handle is not null, Invoke it.
            OnlineStatusChanged?.Invoke(this, e);
        }

        private void remoteToolsControl_NewStatusPrompt(object sender, UserPromptEventArgs e)
        {
            StatusPrompt(e.Text, e.Color, e.DisplayTime);
        }

        private void RemoteToolsControl_VisibleChanging(object sender, bool e)
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

        private void SibiViewButton_Click(object sender, EventArgs e)
        {
            OpenSibiLink(currentViewDevice);
        }

        private void TabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            TrackingGrid.Refresh();
        }

        private void TrackingGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var EntryGuid = TrackingGrid.CurrentRowStringValue(TrackablesCols.Guid);
            if (!Helpers.ChildFormControl.FormIsOpenByGuid(typeof(ViewTrackingForm), EntryGuid))
            {
                NewTrackingView(EntryGuid);
            }
        }

        private void TrackingGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            string checkTypeValue = TrackingGrid.Rows[e.RowIndex].Cells[TrackablesCols.CheckType].Value.ToString();
            var checkTypeCell = TrackingGrid.Rows[e.RowIndex].Cells[TrackablesCols.CheckType];
            checkTypeCell.Style.ForeColor = Color.Black;

            if (checkTypeValue == CheckType.Checkin)
            {
                checkTypeCell.Style.BackColor = Colors.CheckIn;
            }
            else if (checkTypeValue == CheckType.Checkout)
            {
                checkTypeCell.Style.BackColor = Colors.CheckOut;
            }
        }

        #endregion Control Events
    }
}