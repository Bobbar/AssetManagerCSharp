using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Helpers.Watchdog;
using AssetManager.Security;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AdminTools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Deployment.Application;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class MainForm : ExtendedForm, ILiveBox
    {
        #region "Fields"

        private bool gridIsFilling = false;
        private bool queryRunning = false;
        private DbCommand lastCommand;
        private LiveBox liveBox;
        private MunisToolBar munisToolBar;
        private WindowList windowList;
        private DbTransaction currentTransaction = null;

        public MunisEmployee MunisUser
        {
            get
            {
                throw new NotSupportedException();
            }

            set
            {
                throw new NotSupportedException();
            }
        }

        #endregion "Fields"

        #region "Methods"

        public MainForm() : base()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.asset_icon;
            ShowAll();
            liveBox = new LiveBox(this);
            munisToolBar = new CustomControls.MunisToolBar(this);
            windowList = new CustomControls.WindowList(this);

            DateTimeLabel.Text = DateTime.Now.ToString();
            ToolStrip1.BackColor = Colors.AssetToolBarColor;
            ResultGrid.DoubleBuffered(true);
            CheckForAdmin();
            GetGridStyles();

            WatchdogInstance.Watchdog.StatusChanged += WatchdogStatusChanged;
            WatchdogInstance.Watchdog.RebuildCache += WatchdogRebuildCache;
            WatchdogInstance.Watchdog.WatcherTick += WatchdogTick;
            WatchdogInstance.Watchdog.StartWatcher(GlobalSwitches.CachedMode);

            munisToolBar.InsertMunisDropDown(ToolStrip1, 2);
            windowList.InsertWindowList(ToolStrip1);
            InitLiveBox();

            InitDBControls();
            ClearAll();
            ShowTestDBWarning();
            InitDBCombo();
        }

        private void StartTransaction()
        {
            SecurityTools.CheckForAccess(SecurityGroups.CanStartTransaction);

            if (OtherFunctions.Message("This will allow unchecked changes to the database. Incorrect inputs WILL BREAK THINGS! \r\n" + Environment.NewLine + "Changes must be 'applied' and 'committed' before they will be permanently stored in the database.", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, "WARNING", this) == DialogResult.OK)
            {
                currentTransaction = DBFactory.GetDatabase().StartTransaction();
                RefreshData();
                GridEditMode(true);
                DoneWaiting();
            }
        }

        private void CommitTransaction()
        {
            if (currentTransaction != null)
            {
                if (OtherFunctions.Message("Are you sure? This will permanently apply the changes to the database.", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Commit Transaction", this) == DialogResult.Yes)
                {
                    currentTransaction.Commit();
                    currentTransaction.Connection.Dispose();
                    currentTransaction.Dispose();
                    currentTransaction = null;
                    RefreshData();
                    GridEditMode(false);
                    DoneWaiting();
                }
            }
        }

        private void RollbackTransaction()
        {
            if (currentTransaction != null)
            {
                if (OtherFunctions.Message("Restore database to original state?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Rollback Transaction", this) == DialogResult.Yes)
                {
                    currentTransaction.Rollback();
                    currentTransaction.Connection.Dispose();
                    currentTransaction.Dispose();
                    currentTransaction = null;
                    RefreshData();
                    GridEditMode(false);
                    DoneWaiting();
                }
            }
        }

        private void UpdateRecords()
        {
            if (currentTransaction != null)
            {
                DBFactory.GetDatabase().UpdateTable(Queries.SelectDevicesTable, (DataTable)ResultGrid.DataSource, currentTransaction);
                RefreshData();
                DoneWaiting();
            }
        }

        private void GridEditMode(bool canEdit)
        {
            if (canEdit)
            {
                ResultGrid.ReadOnly = false;
                ResultGrid.EditMode = DataGridViewEditMode.EditOnEnter;
                TransactionBox.Visible = true;
            }
            else
            {
                ResultGrid.ReadOnly = true;
                TransactionBox.Visible = false;
                ResultGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            }
        }

        /// <summary>
        /// Builds a query from the populated search controls.
        /// </summary>
        public void DynamicSearch()
        {
            try
            {
                QueryParamCollection searchParams = BuildSearchList();
                string query;
                if (HistoricalCheckBox.Checked)
                {
                    query = "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE";
                    var searchCommand = DBFactory.GetDatabase().GetCommandFromParams(query, searchParams.Parameters);
                    searchCommand.CommandText += " GROUP BY " + DevicesCols.DeviceGuid;
                    StartBigQuery(searchCommand);
                }
                else
                {
                    query = "SELECT * FROM " + DevicesCols.TableName + " WHERE";
                    var searchCommand = DBFactory.GetDatabase().GetCommandFromParams(query, searchParams.Parameters);
                    StartBigQuery(searchCommand);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        public void ViewDevice(string deviceGuid, bool startHidden = false)
        {
            if (string.IsNullOrEmpty(deviceGuid)) return;

            try
            {
                if (!ChildFormControl.FormIsOpenByGuid(typeof(ViewDeviceForm), deviceGuid))
                {
                    if (!startHidden) Waiting();
                    new ViewDeviceForm(this, new Device(deviceGuid), startHidden);
                }
            }
            catch (Exception ex)
            {
                if (ex is IndexOutOfRangeException)
                {
                    OtherFunctions.Message("That device was not found!  It may have been deleted.  Re-execute your search.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Not Found", this);
                }
                else
                {
                    ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                }
            }
            finally
            {
                if (!startHidden) DoneWaiting();
            }
        }

        /// <summary>
        /// Loads and displays devices for all currently selected rows.
        /// </summary>
        private async Task ViewSelectedDevices()
        {
            try
            {
                var uniqueRows = new HashSet<int>();
                foreach (DataGridViewCell cell in ResultGrid.SelectedCells)
                {
                    uniqueRows.Add(cell.RowIndex);
                }

                SetStatusBar("Loading " + uniqueRows.Count + " devices...");

                foreach (var index in uniqueRows)
                {
                    var row = ResultGrid.Rows[index];
                    var guid = row.Cells[DevicesCols.DeviceGuid].Value.ToString();

                    if (!string.IsNullOrEmpty(guid))
                    {
                        ViewDevice(guid, true);
                    }
                    // Delay for just a moment to keep the UI somewhat alive.
                    await Task.Delay(20);
                }

                uniqueRows.Clear();
                uniqueRows = null;
            }
            finally
            {
                DoneWaiting();
            }
        }

        public override void RefreshData()
        {
            StartBigQuery(lastCommand);
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void AddNewDevice()
        {
            SecurityTools.CheckForAccess(SecurityGroups.AddDevice);

            var newDevForm = Helpers.ChildFormControl.FindChildOfType(this, typeof(NewDeviceForm));
            if (newDevForm == null)
            {
                new NewDeviceForm(this);
            }
            else
            {
                newDevForm.RestoreWindow();
            }
        }

        private QueryParamCollection BuildSearchList()
        {
            var searchParams = new QueryParamCollection();

            using (var controlParser = new DBControlParser(this))
            {
                foreach (var ctl in controlParser.GetDBControls(this))
                {
                    var ctlValue = controlParser.GetDBControlValue(ctl);
                    if (ctlValue != DBNull.Value && !string.IsNullOrEmpty(ctlValue.ToString()))
                    {
                        var dbInfo = (DBControlInfo)ctl.Tag;
                        bool isExact = false;
                        switch (dbInfo.ColumnName)
                        {
                            //case DevicesCols.OSVersion:
                            //    IsExact = true;
                            //    break;

                            case DevicesCols.EQType:
                                isExact = true;
                                break;

                            case DevicesCols.Location:
                                isExact = true;
                                break;

                            case DevicesCols.Status:
                                isExact = true;
                                break;

                            default:
                                isExact = false;
                                break;
                        }

                        searchParams.Add(dbInfo.ColumnName, ctlValue, isExact);
                    }
                }
                return searchParams;
            }
        }

        private void ClearAll()
        {
            AssetTagTextBox.Clear();
            AssetTagSearchTextBox.Clear();
            SerialTextBox.Clear();
            SerialSearchTextBox.Clear();
            CurrrentUserTextBox.Clear();
            DescriptionTextBox.Clear();
            ReplaceYearTextBox.Clear();
            PurchaseDateTextBox.Clear();

            TrackablesCheckBox.Checked = false;
            HistoricalCheckBox.Checked = false;
            RefreshCombos();
        }

        private async void WatchdogRebuildCache(object sender, EventArgs e)
        {
            if (GlobalSwitches.BuildingCache) return;

            GlobalSwitches.BuildingCache = true;
            try
            {
                SetStatusBar("Rebuilding DB Cache...");
                await Task.Run(() =>
                {
                    DBCacheFunctions.RefreshLocalDBCache();
                });
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                GlobalSwitches.BuildingCache = false;
                DoneWaiting();
            }
        }

        private void WatchdogTick(object sender, EventArgs e)
        {
            var tickEvent = (WatchdogTickEventArgs)e;
            if (DateTimeLabel.Text != tickEvent.ServerTime)
            {
                SetServerTime(tickEvent.ServerTime);
            }
        }

        private void WatchdogStatusChanged(object sender, EventArgs e)
        {
            var connectionEventArgs = (WatchdogStatusEventArgs)e;
            Logging.Logger("Connection Status changed to: " + connectionEventArgs.ConnectionStatus.ToString());
            switch (connectionEventArgs.ConnectionStatus)
            {
                case WatchdogConnectionStatus.Online:
                    ConnectStatus("Connected", Color.Green, Colors.DefaultFormBackColor, "Connection OK");
                    GlobalSwitches.CachedMode = false;
                    ServerInfo.ServerPinging = true;
                    CheckForAdmin();
                    break;

                case WatchdogConnectionStatus.Offline:
                    ConnectStatus("Offline", Color.Red, Colors.StatusBarProblem, "No connection. Cache unavailable.");
                    GlobalSwitches.CachedMode = false;
                    ServerInfo.ServerPinging = false;
                    AdminDropDown.Visible = false;
                    break;

                case WatchdogConnectionStatus.CachedMode:
                    ConnectStatus("Cached Mode", Color.Black, Colors.StatusBarProblem, "Server Offline. Using Local DB Cache.");
                    GlobalSwitches.CachedMode = true;
                    ServerInfo.ServerPinging = false;
                    AdminDropDown.Visible = false;
                    break;
            }
        }

        private void ConnectStatus(string text, Color foreColor, Color backColor, string toolTipText)
        {
            if (StatusStrip1.InvokeRequired)
            {
                var del = new Action(() => ConnectStatus(text, foreColor, backColor, toolTipText));
                StatusStrip1.BeginInvoke(del);
            }
            else
            {
                ConnStatusLabel.Text = text;
                ConnStatusLabel.ToolTipText = toolTipText;
                ConnStatusLabel.ForeColor = foreColor;
                StatusStrip1.BackColor = backColor;
                StatusStrip1.Update();
            }
        }

        private void DisplayRecordCount(int count)
        {
            RecordsCountLabel.Text = "Records: " + count;
        }

        public override void Waiting(string message = "Processing...")
        {
            base.Waiting();

            SetStatusBar(message);
        }

        public override void DoneWaiting()
        {
            base.DoneWaiting();

            StripSpinner.Visible = false;
            SetStatusBar("Idle...");
        }

        private async void EnqueueGKUpdate()
        {
            if (await SecurityTools.VerifyAdminCreds())
            {
                var selectedDevices = new List<Device>();
                var rows = new HashSet<int>();
                //Iterate selected cells and collect unique row indexes via a HashSet.(HashSet only allows unique values to be added to collection).
                foreach (DataGridViewCell cell in ResultGrid.SelectedCells)
                {
                    rows.Add(cell.RowIndex);
                }

                foreach (var row in rows)
                {
                    var devGuid = ResultGrid[DevicesCols.DeviceGuid, row].Value.ToString();
                    selectedDevices.Add(new Device(devGuid));
                }

                Helpers.ChildFormControl.GKUpdaterInstance().AddMultipleUpdates(selectedDevices);
            }
        }

        private void GetGridStyles()
        {
            //Set default styles for all grid forms.
            Colors.DefaultGridBackColor = ResultGrid.DefaultCellStyle.BackColor;
            ResultGrid.DefaultCellStyle.SelectionBackColor = Colors.OrangeSelectColor;
            this.GridTheme = new GridTheme(Colors.OrangeHighlightColor, Colors.OrangeSelectColor, Colors.OrangeSelectAltColor, ResultGrid.DefaultCellStyle.BackColor);
            StyleFunctions.DefaultGridStyles = new DataGridViewCellStyle(ResultGrid.DefaultCellStyle);
            StyleFunctions.AlternatingRowDefaultStyles = new DataGridViewCellStyle(ResultGrid.AlternatingRowsDefaultCellStyle);
            StyleFunctions.SetGridStyle(ResultGrid, GridTheme);
        }

        private void InitDBControls()
        {
            SerialSearchTextBox.SetDBInfo(DevicesCols.Serial);
            AssetTagSearchTextBox.SetDBInfo(DevicesCols.AssetTag);
            DescriptionTextBox.SetDBInfo(DevicesCols.Description);
            EquipTypeComboBox.SetDBInfo(DevicesCols.EQType, Attributes.DeviceAttributes.EquipType);
            ReplaceYearTextBox.SetDBInfo(DevicesCols.ReplacementYear);
            OSTypeComboBox.SetDBInfo(DevicesCols.OSVersion, Attributes.DeviceAttributes.OSType);
            LocationComboBox.SetDBInfo(DevicesCols.Location, Attributes.DeviceAttributes.Locations);
            CurrrentUserTextBox.SetDBInfo(DevicesCols.CurrentUser);
            StatusComboBox.SetDBInfo(DevicesCols.Status, Attributes.DeviceAttributes.StatusType);
            TrackablesCheckBox.SetDBInfo(DevicesCols.Trackable);
            PurchaseDateTextBox.SetDBInfo(DevicesCols.PurchaseDate);
        }

        private void InitLiveBox()
        {
            liveBox.AttachToControl(DescriptionTextBox, DevicesCols.Description, LiveBoxSelectAction.DynamicSearch);
            liveBox.AttachToControl(CurrrentUserTextBox, DevicesCols.CurrentUser, LiveBoxSelectAction.DynamicSearch);
            liveBox.AttachToControl(SerialTextBox, DevicesCols.Serial, LiveBoxSelectAction.LoadDevice, DevicesCols.DeviceGuid);
            liveBox.AttachToControl(AssetTagTextBox, DevicesCols.AssetTag, LiveBoxSelectAction.LoadDevice, DevicesCols.DeviceGuid);
        }

        private void InitDBCombo()
        {
            foreach (var item in Enum.GetValues(typeof(DatabaseName)))
            {
                DatabaseToolCombo.Items.Add(item.ToString());
            }
            DatabaseToolCombo.SelectedIndex = (int)ServerInfo.CurrentDataBase;
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void NewTextCrypterForm()
        {
            SecurityTools.CheckForAccess(SecurityGroups.IsAdmin);
            new CrypterForm(this);
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void OpenSibiMainForm()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.ViewSibi);

                Waiting();
                var sibiForm = Helpers.ChildFormControl.FindChildOfType(this, typeof(Sibi.SibiMainForm));
                if (sibiForm == null)
                {
                    new Sibi.SibiMainForm(this);
                }
                else
                {
                    sibiForm.RestoreWindow();
                }
            }
            finally
            {
                DoneWaiting();
            }
        }

        public override bool OkToClose()
        {
            if (currentTransaction != null)
            {
                OtherFunctions.Message("There is currently an active transaction. Please commit or rollback before closing.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Cannot Close");
                return false;
            }

            if (!OtherFunctions.OKToEnd())
            {
                return false;
            }
            return true;
        }

        private void RefreshCombos()
        {
            EquipTypeComboBox.FillComboBox(Attributes.DeviceAttributes.EquipType);
            LocationComboBox.FillComboBox(Attributes.DeviceAttributes.Locations);
            StatusComboBox.FillComboBox(Attributes.DeviceAttributes.StatusType);
            OSTypeComboBox.FillComboBox(Attributes.DeviceAttributes.OSType);
        }

        private List<GridColumnAttrib> ResultGridColumns()
        {
            ColumnFormatType attribColumnType;
            if (currentTransaction != null)
            {
                attribColumnType = ColumnFormatType.AttributeCombo;
            }
            else
            {
                attribColumnType = ColumnFormatType.AttributeDisplayMemberOnly;
            }
            var columnList = new List<GridColumnAttrib>();
            columnList.Add(new GridColumnAttrib(DevicesCols.CurrentUser, "User"));
            columnList.Add(new GridColumnAttrib(DevicesCols.AssetTag, "Asset ID"));
            columnList.Add(new GridColumnAttrib(DevicesCols.Serial, "Serial"));
            columnList.Add(new GridColumnAttrib(DevicesCols.EQType, "Device Type", Attributes.DeviceAttributes.EquipType, attribColumnType));
            columnList.Add(new GridColumnAttrib(DevicesCols.Description, "Description"));
            columnList.Add(new GridColumnAttrib(DevicesCols.OSVersion, "OS Version", Attributes.DeviceAttributes.OSType, attribColumnType));
            columnList.Add(new GridColumnAttrib(DevicesCols.Location, "Location", Attributes.DeviceAttributes.Locations, attribColumnType));
            columnList.Add(new GridColumnAttrib(DevicesCols.PO, "PO Number"));
            columnList.Add(new GridColumnAttrib(DevicesCols.PurchaseDate, "Purchase Date"));
            columnList.Add(new GridColumnAttrib(DevicesCols.ReplacementYear, "Replace Year"));
            columnList.Add(new GridColumnAttrib(DevicesCols.LastModDate, "Modified"));
            columnList.Add(new GridColumnAttrib(DevicesCols.DeviceGuid, "Guid"));
            return columnList;
        }

        private void SendToGrid(ref DataTable results)
        {
            if (results == null) return;

            using (results)
            {
                ResultGrid.SuspendLayout();
                ResultGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                ResultGrid.ColumnHeadersHeight = 38;
                ResultGrid.ScrollBars = ScrollBars.None;
                ResultGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                gridIsFilling = true;

                if (currentTransaction != null)
                {
                    ResultGrid.Populate(results, ResultGridColumns(), true);
                }
                else
                {
                    ResultGrid.Populate(results, ResultGridColumns());
                }

                ResultGrid.FastAutoSizeColumns();
                gridIsFilling = false;
                DisplayRecordCount(ResultGrid.Rows.Count);
                ResultGrid.ScrollBars = ScrollBars.Both;
                ResultGrid.ResumeLayout();
            }
        }

        private void ShowAll()
        {
            var cmd = DBFactory.GetDatabase().GetCommand(Queries.SelectDevicesTable);
            StartBigQuery(cmd);
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void StartAdvancedSearch()
        {
            SecurityTools.CheckForAccess(SecurityGroups.AdvancedSearch);

            new AdvancedSearchForm(this);
        }

        private void StartAttachScan()
        {
            SecurityTools.CheckForAccess(SecurityGroups.ManageAttachment);
            FtpFunctions.ScanAttachements();
        }

        private async void StartBigQuery(DbCommand QryCommand)
        {
            try
            {
                if (!queryRunning)
                {
                    Waiting("Background query running...");
                    StripSpinner.Visible = true;
                    queryRunning = true;
                    var Results = await Task.Run(() =>
                    {
                        lastCommand = QryCommand;
                        return DBFactory.GetDatabase().DataTableFromCommand(QryCommand, currentTransaction);
                    });
                    QryCommand.Dispose();
                    SendToGrid(ref Results);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                queryRunning = false;
                DoneWaiting();
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void StartUserManager()
        {
            SecurityTools.CheckForAccess(SecurityGroups.IsAdmin);
            new UserManagerForm(this);
        }

        private void SetStatusBar(string text)
        {
            if (StatusStrip1.InvokeRequired)
            {
                var del = new Action(() => SetStatusBar(text));
                StatusStrip1.BeginInvoke(del);
            }
            else
            {
                StatusLabel.Text = text;
                StatusStrip1.Update();
            }
        }

        private void SetServerTime(string serverTime)
        {
            if (StatusStrip1.InvokeRequired)
            {
                var del = new Action(() => SetServerTime(serverTime));
                StatusStrip1.BeginInvoke(del);
            }
            else
            {
                DateTimeLabel.Text = serverTime;
                StatusStrip1.Update();
            }
        }

        private void CheckForAdmin()
        {
            if (SecurityTools.CanAccess(SecurityGroups.IsAdmin))
            {
                AdminDropDown.Visible = true;
            }
            else
            {
                AdminDropDown.Visible = false;
            }
        }

        private void ChangeDatabase(DatabaseName database)
        {
            try
            {
                if (currentTransaction == null)
                {
                    if (!GlobalSwitches.CachedMode & ServerInfo.ServerPinging)
                    {
                        if (database != ServerInfo.CurrentDataBase)
                        {
                            var blah = OtherFunctions.Message("Are you sure? This will close all open forms.", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Change Database", this);
                            if (blah == DialogResult.Yes)
                            {
                                if (this.OkToCloseChildren())
                                {
                                    this.CloseChildren();
                                    ServerInfo.CurrentDataBase = database;
                                    AttributeFunctions.PopulateAttributeIndexes();
                                    RefreshCombos();
                                    SecurityTools.PopulateUserAccess();
                                    InitDBControls();
                                    GlobalSwitches.BuildingCache = true;
                                    Task.Run(() => DBCacheFunctions.RefreshLocalDBCache());
                                    ShowTestDBWarning();
                                    SetDatabaseTitleText();
                                    ShowAll();
                                }
                            }
                        }
                    }
                    else
                    {
                        OtherFunctions.Message("Cannot switch database while Offline or in Cached Mode.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Unavailable", this);
                    }
                }
                else
                {
                    OtherFunctions.Message("There is currently an active transaction. Please commit or rollback before switching databases.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Stop");
                }
            }
            finally
            {
                DatabaseToolCombo.SelectedIndex = (int)ServerInfo.CurrentDataBase;
            }
        }

        private void SetDatabaseTitleText()
        {
            switch (ServerInfo.CurrentDataBase)
            {
                case DatabaseName.asset_manager:
                    this.Text = "Asset Manager - Main";
                    break;

                case DatabaseName.test_db:
                    this.Text = "Asset Manager - Main - ****TEST DATABASE****";
                    break;

                case DatabaseName.vintondd:
                    this.Text = "Asset Manager - Main - Vinton DD";
                    break;
            }
        }

        private void ShowTestDBWarning()
        {
            if (ServerInfo.CurrentDataBase == DatabaseName.test_db)
            {
                // OtherFunctions.Message("TEST DATABASE IN USE", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "WARNING");//, this);
                this.BackColor = Color.DarkRed;
                this.Text += " - ****TEST DATABASE****";
            }
            else
            {
                this.BackColor = Color.FromArgb(232, 232, 232);
                this.Text = "Asset Manager - Main";
            }
        }

        #region "Control Event Methods"

        private void AdvancedSearchMenuItem_Click(object sender, EventArgs e)
        {
            StartAdvancedSearch();
        }

        private void AddDeviceToolButton_Click(object sender, EventArgs e)
        {
            AddNewDevice();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void SearchButton_Click(object sender, EventArgs e)
        {
            DynamicSearch();
        }

        private void ShowAllButon_Click(object sender, EventArgs e)
        {
            ShowAll();
        }

        private void StartSibiButton_Click(object sender, EventArgs e)
        {
            OpenSibiMainForm();
        }

        private void DevBySupButton_Click(object sender, EventArgs e)
        {
            new Hierarchy(this);
        }

        private void CopySelectedMenuItem_Click(object sender, EventArgs e)
        {
            ResultGrid.CopyToClipboard();
        }

        private void DateTimeLabel_Click(object sender, EventArgs e)
        {
            if (ApplicationDeployment.IsNetworkDeployed)
            {
                OtherFunctions.Message(ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString(), MessageBoxButtons.OK, MessageBoxIcon.Information, "Version", this);
            }
            else
            {
                OtherFunctions.Message("Debug", MessageBoxButtons.OK, MessageBoxIcon.Information, "Version", this);
            }
        }

        private void PanelNoScrollOnFocus1_Scroll(object sender, ScrollEventArgs e)
        {
            liveBox.HideLiveBox();
        }

        private void ResultGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!gridIsFilling)
            {
                StyleFunctions.HighlightRow(ResultGrid, GridTheme, e.RowIndex);
            }
        }

        private void ResultGrid_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(ResultGrid, e.RowIndex);
        }

        private void ResultGrid_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.ColumnIndex >= 0 & e.RowIndex >= 0)
            {
                if (e.Button == MouseButtons.Right & !ResultGrid[e.ColumnIndex, e.RowIndex].Selected)
                {
                    ResultGrid.Rows[e.RowIndex].Selected = true;
                    ResultGrid.CurrentCell = ResultGrid[e.ColumnIndex, e.RowIndex];
                }
            }
        }

        private void ResultGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                ViewDevice(ResultGrid.CurrentRowStringValue(DevicesCols.DeviceGuid));
                e.SuppressKeyPress = true;
            }
        }

        private void ScanAttachmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartAttachScan();
        }

        private void TextEnCrypterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewTextCrypterForm();
        }

        private void AddGKUpdateMenuItem_Click(object sender, EventArgs e)
        {
            EnqueueGKUpdate();
        }

        private void GKUpdaterMenuItem_Click(object sender, EventArgs e)
        {
            var GKUpdater = Helpers.ChildFormControl.GKUpdaterInstance();
            if (!GKUpdater.Visible)
            {
                GKUpdater.Show();
            }
            else
            {
                GKUpdater.WindowState = FormWindowState.Normal;
                GKUpdater.Activate();
            }
        }

        private void SendToGridFormMenuItem_Click(object sender, EventArgs e)
        {
            ResultGrid.CopyToGridForm(this, DoubleClickAction.ViewDevice);
        }

        private void UserManagerMenuItem_Click(object sender, EventArgs e)
        {
            StartUserManager();
        }

        private void GuidTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                ViewDevice(GuidTextBox.Text.Trim());
                GuidTextBox.Clear();
            }
        }

        private void ViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var selectedGuid = ResultGrid.CurrentRowStringValue(DevicesCols.DeviceGuid);
            ViewDevice(selectedGuid);
        }

        private void LoadSelectedMenuItem_Click(object sender, EventArgs e)
        {
            ViewSelectedDevices();
        }

        private void ReEnterLACredentialsMenuItem_Click(object sender, EventArgs e)
        {
            SecurityTools.ClearAdminCreds();
            SecurityTools.VerifyAdminCreds();
        }

        private void ViewLogToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ProcessStartInfo StartInfo = new ProcessStartInfo();
            StartInfo.FileName = Paths.LogPath;
            Process.Start(StartInfo);
        }

        private void DatabaseToolCombo_DropDownClosed(object sender, EventArgs e)
        {
            ChangeDatabase((DatabaseName)DatabaseToolCombo.SelectedIndex);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Application.DoEvents();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Helpers.ChildFormControl.SplashScreenInstance().Dispose();
        }

        private void CommitButton_Click(object sender, EventArgs e)
        {
            CommitTransaction();
        }

        private void RollbackButton_Click(object sender, EventArgs e)
        {
            RollbackTransaction();
        }

        private void StartTransactionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            StartTransaction();
        }

        private void UpdateButton_Click(object sender, EventArgs e)
        {
            UpdateRecords();
        }

        private void ResultGrid_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            ViewDevice(ResultGrid.CurrentRowStringValue(DevicesCols.DeviceGuid));
        }

        #endregion "Control Event Methods"

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

                    lastCommand?.Dispose();
                    liveBox.Dispose();
                    munisToolBar.Dispose();
                    windowList.Dispose();
                    OtherFunctions.EndProgram();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion "Methods"
    }
}