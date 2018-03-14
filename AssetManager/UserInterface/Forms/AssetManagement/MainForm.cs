using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Security;
using AssetManager.Tools;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AdminTools;
using MyDialogLib;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Deployment.Application;
using System.Diagnostics;
using System.Drawing;
using System.Management.Automation.Runspaces;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class MainForm : ExtendedForm, ILiveBox
    {
        #region "Fields"

        private bool bolGridFilling = false;
        private DbCommand LastCommand;

        private LiveBox MyLiveBox;
        private MunisToolBar MyMunisToolBar;
        private WindowList MyWindowList;
        private bool QueryRunning = false;
        private ConnectionWatchdog WatchDog;

        private DbTransaction CurrentTransaction = null;

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

        #region "Delegates"

        private delegate void ConnectStatusVoidDelegate(string text, Color foreColor, Color backColor, string toolTipText);

        private delegate void StatusVoidDelegate(string text, int timeOut = 0);

        private delegate void ServerTimeVoidDelegate(string serverTime);

        #endregion "Delegates"

        #region "Methods"

        public MainForm() : base()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.asset_icon;
            ShowAll();
            MyLiveBox = new LiveBox(this);
            MyMunisToolBar = new CustomControls.MunisToolBar(this);
            MyWindowList = new CustomControls.WindowList(this);

            DateTimeLabel.Text = DateTime.Now.ToString();
            ToolStrip1.BackColor = Colors.AssetToolBarColor;
            ResultGrid.DoubleBufferedDataGrid(true);
            CheckForAdmin();
            GetGridStyles();

            WatchDog = new ConnectionWatchdog(GlobalSwitches.CachedMode);
            WatchDog.StatusChanged += WatchDogStatusChanged;
            WatchDog.RebuildCache += WatchDogRebuildCache;
            WatchDog.WatcherTick += WatchDogTick;
            WatchDog.StartWatcher();

            MyMunisToolBar.InsertMunisDropDown(ToolStrip1, 2);
            MyWindowList.InsertWindowList(ToolStrip1);
            ImageCaching.CacheControlImages(this);
            InitLiveBox();

            InitDBControls();
            Clear_All();
            ShowTestDBWarning();
            InitDBCombo();
        }

        private void StartTransaction()
        {
            SecurityTools.CheckForAccess(SecurityGroups.CanStartTransaction);

            if (OtherFunctions.Message("This will allow unchecked changes to the database. Incorrect inputs WILL BREAK THINGS! \r\n" + Environment.NewLine + "Changes must be 'applied' and 'committed' before they will be permanently stored in the database.", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, "WARNING", this) == DialogResult.OK)
            {
                CurrentTransaction = DBFactory.GetDatabase().StartTransaction();
                RefreshData();
                GridEditMode(true);
                DoneWaiting();
            }
        }

        private void CommitTransaction()
        {
            if (CurrentTransaction != null)
            {
                if (OtherFunctions.Message("Are you sure? This will permanently apply the changes to the database.", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Commit Transaction", this) == DialogResult.Yes)
                {
                    CurrentTransaction.Commit();
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                    RefreshData();
                    GridEditMode(false);
                    DoneWaiting();
                }
            }
        }

        private void RollbackTransaction()
        {
            if (CurrentTransaction != null)
            {
                if (OtherFunctions.Message("Restore database to original state?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Rollback Transaction", this) == DialogResult.Yes)
                {
                    CurrentTransaction.Rollback();
                    CurrentTransaction.Dispose();
                    CurrentTransaction = null;
                    RefreshData();
                    GridEditMode(false);
                    DoneWaiting();
                }
            }
        }

        private void UpdateRecords()
        {
            if (CurrentTransaction != null)
            {
                DBFactory.GetDatabase().UpdateTable(Queries.SelectDevicesTable, (DataTable)ResultGrid.DataSource, CurrentTransaction);
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

        //dynamically creates sql query using any combination of search filters the users wants
        public void DynamicSearch()
        {
            try
            {
                QueryParamCollection searchParams = BuildSearchList();
                string strStartQry;
                if (chkHistorical.Checked)
                {
                    strStartQry = "SELECT * FROM " + HistoricalDevicesCols.TableName + " WHERE";
                    var searchCommand = DBFactory.GetDatabase().GetCommandFromParams(strStartQry, searchParams.Parameters);
                    searchCommand.CommandText += " GROUP BY " + DevicesCols.DeviceGuid;
                    StartBigQuery(searchCommand);
                }
                else
                {
                    strStartQry = "SELECT * FROM " + DevicesCols.TableName + " WHERE";
                    var searchCommand = DBFactory.GetDatabase().GetCommandFromParams(strStartQry, searchParams.Parameters);
                    StartBigQuery(searchCommand);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        public void LoadDevice(string deviceGuid)
        {
            try
            {
                if (!ChildFormControl.FormIsOpenByGuid(typeof(ViewDeviceForm), deviceGuid))
                {
                    Waiting();
                    new ViewDeviceForm(this, new Device(deviceGuid));
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
                DoneWaiting();
            }
        }

        public override void RefreshData()
        {
            StartBigQuery(LastCommand);
        }

        private void AddNewDevice()
        {
            SecurityTools.CheckForAccess(SecurityGroups.AddDevice);

            var NewDevForm = Helpers.ChildFormControl.FindChildOfType(this, typeof(NewDeviceForm));
            if (NewDevForm == null)
            {
                new NewDeviceForm(this);
            }
            else
            {
                Helpers.ChildFormControl.ActivateForm(NewDevForm);
            }
        }

        private QueryParamCollection BuildSearchList()
        {
            QueryParamCollection searchParams = new QueryParamCollection();

            using (DBControlParser controlParser = new DBControlParser(this))
            {
                foreach (Control ctl in controlParser.GetDBControls(this))
                {
                    object CtlValue = controlParser.GetDBControlValue(ctl);
                    if (!object.ReferenceEquals(CtlValue, DBNull.Value) && !string.IsNullOrEmpty(CtlValue.ToString()))
                    {
                        var DBInfo = (DBControlInfo)ctl.Tag;
                        bool IsExact = false;
                        switch (DBInfo.DataColumn)
                        {
                            //case DevicesCols.OSVersion:
                            //    IsExact = true;
                            //    break;

                            case DevicesCols.EQType:
                                IsExact = true;
                                break;

                            case DevicesCols.Location:
                                IsExact = true;
                                break;

                            case DevicesCols.Status:
                                IsExact = true;
                                break;

                            default:
                                IsExact = false;
                                break;
                        }

                        searchParams.Add(DBInfo.DataColumn, CtlValue, IsExact);
                    }
                }
                return searchParams;
            }
        }

        private void Clear_All()
        {
            txtAssetTag.Clear();
            txtAssetTagSearch.Clear();
            txtSerial.Clear();
            txtSerialSearch.Clear();
            txtCurUser.Clear();
            txtDescription.Clear();
            txtReplaceYear.Clear();
            chkTrackables.Checked = false;
            chkHistorical.Checked = false;
            RefreshCombos();
        }

        private async void WatchDogRebuildCache(object sender, EventArgs e)
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

        private void WatchDogTick(object sender, EventArgs e)
        {
            var TickEvent = (WatchDogTickEventArgs)e;
            if (DateTimeLabel.Text != TickEvent.ServerTime)
            {
                SetServerTime(TickEvent.ServerTime);
            }
        }

        private void WatchDogStatusChanged(object sender, EventArgs e)
        {
            var ConnectionEventArgs = (WatchDogStatusEventArgs)e;
            Logging.Logger("Connection Status changed to: " + ConnectionEventArgs.ConnectionStatus.ToString());
            switch (ConnectionEventArgs.ConnectionStatus)
            {
                case WatchDogConnectionStatus.Online:
                    ConnectStatus("Connected", Color.Green, Colors.DefaultFormBackColor, "Connection OK");
                    GlobalSwitches.CachedMode = false;
                    ServerInfo.ServerPinging = true;
                    CheckForAdmin();
                    break;

                case WatchDogConnectionStatus.Offline:
                    ConnectStatus("Offline", Color.Red, Colors.StatusBarProblem, "No connection. Cache unavailable.");
                    GlobalSwitches.CachedMode = false;
                    ServerInfo.ServerPinging = false;
                    AdminDropDown.Visible = false;
                    break;

                case WatchDogConnectionStatus.CachedMode:
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
                ConnectStatusVoidDelegate d = new ConnectStatusVoidDelegate(ConnectStatus);
                StatusStrip1.Invoke(d, new object[] {
                text,
                foreColor,
                backColor,
                toolTipText
            });
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

        private void DisplayRecords(int NumberOf)
        {
            lblRecords.Text = "Records: " + NumberOf;
        }

        private void DoneWaiting()
        {
            OtherFunctions.SetWaitCursor(false, this);
            StripSpinner.Visible = false;
            SetStatusBar("Idle...");
        }

        private void EnqueueGKUpdate()
        {
            if (SecurityTools.VerifyAdminCreds())
            {
                List<Device> SelectedDevices = new List<Device>();
                HashSet<int> Rows = new HashSet<int>();
                //Iterate selected cells and collect unique row indexes via a HashSet.(HashSet only allows unique values to be added to collection).
                foreach (DataGridViewCell cell in ResultGrid.SelectedCells)
                {
                    Rows.Add(cell.RowIndex);
                }

                foreach (var row in Rows)
                {
                    string DevGuid = ResultGrid[DevicesCols.DeviceGuid, row].Value.ToString();
                    SelectedDevices.Add(new Device(DevGuid));
                }

                Helpers.ChildFormControl.GKUpdaterInstance().AddMultipleUpdates(SelectedDevices);
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
            txtSerialSearch.Tag = new DBControlInfo(DevicesCols.Serial);
            txtAssetTagSearch.Tag = new DBControlInfo(DevicesCols.AssetTag);
            txtDescription.Tag = new DBControlInfo(DevicesCols.Description);
            cmbEquipType.Tag = new DBControlInfo(DevicesCols.EQType, Attributes.DeviceAttribute.EquipType);
            txtReplaceYear.Tag = new DBControlInfo(DevicesCols.ReplacementYear);
            cmbOSType.Tag = new DBControlInfo(DevicesCols.OSVersion, Attributes.DeviceAttribute.OSType);
            cmbLocation.Tag = new DBControlInfo(DevicesCols.Location, Attributes.DeviceAttribute.Locations);
            txtCurUser.Tag = new DBControlInfo(DevicesCols.CurrentUser);
            cmbStatus.Tag = new DBControlInfo(DevicesCols.Status, Attributes.DeviceAttribute.StatusType);
            chkTrackables.Tag = new DBControlInfo(DevicesCols.Trackable);
        }

        private void InitLiveBox()
        {
            MyLiveBox.AttachToControl(txtDescription, DevicesCols.Description, LiveBox.LiveBoxSelectionType.DynamicSearch);
            MyLiveBox.AttachToControl(txtCurUser, DevicesCols.CurrentUser, LiveBox.LiveBoxSelectionType.DynamicSearch);
            MyLiveBox.AttachToControl(txtSerial, DevicesCols.Serial, LiveBox.LiveBoxSelectionType.LoadDevice, DevicesCols.DeviceGuid);
            MyLiveBox.AttachToControl(txtAssetTag, DevicesCols.AssetTag, LiveBox.LiveBoxSelectionType.LoadDevice, DevicesCols.DeviceGuid);
        }

        private void InitDBCombo()
        {
            foreach (var item in Enum.GetValues(typeof(Database)))
            {
                DatabaseToolCombo.Items.Add(item.ToString());
            }
            DatabaseToolCombo.SelectedIndex = (int)ServerInfo.CurrentDataBase;
        }

        private void NewTextCrypterForm()
        {
            SecurityTools.CheckForAccess(SecurityGroups.IsAdmin);
            new CrypterForm(this);
        }

        private void OpenSibiMainForm()
        {
            try
            {
                SecurityTools.CheckForAccess(SecurityGroups.ViewSibi);

                Waiting();
                var SibiForm = Helpers.ChildFormControl.FindChildOfType(this, typeof(Sibi.SibiMainForm));
                if (SibiForm == null)
                {
                    new Sibi.SibiMainForm(this);
                }
                else
                {
                    Helpers.ChildFormControl.ActivateForm(SibiForm);
                }
            }
            finally
            {
                DoneWaiting();
            }
        }

        private void RefreshCombos()
        {
            cmbEquipType.FillComboBox(Attributes.DeviceAttribute.EquipType);
            cmbLocation.FillComboBox(Attributes.DeviceAttribute.Locations);
            cmbStatus.FillComboBox(Attributes.DeviceAttribute.StatusType);
            cmbOSType.FillComboBox(Attributes.DeviceAttribute.OSType);
        }

        private List<GridColumnAttrib> ResultGridColumns()
        {
            ColumnFormatType AttribColumnType;
            if (CurrentTransaction != null)
            {
                AttribColumnType = ColumnFormatType.AttributeCombo;
            }
            else
            {
                AttribColumnType = ColumnFormatType.AttributeDisplayMemberOnly;
            }
            List<GridColumnAttrib> ColList = new List<GridColumnAttrib>();
            ColList.Add(new GridColumnAttrib(DevicesCols.CurrentUser, "User"));
            ColList.Add(new GridColumnAttrib(DevicesCols.AssetTag, "Asset ID"));
            ColList.Add(new GridColumnAttrib(DevicesCols.Serial, "Serial"));
            ColList.Add(new GridColumnAttrib(DevicesCols.EQType, "Device Type", Attributes.DeviceAttribute.EquipType, AttribColumnType));
            ColList.Add(new GridColumnAttrib(DevicesCols.Description, "Description"));
            ColList.Add(new GridColumnAttrib(DevicesCols.OSVersion, "OS Version", Attributes.DeviceAttribute.OSType, AttribColumnType));
            ColList.Add(new GridColumnAttrib(DevicesCols.Location, "Location", Attributes.DeviceAttribute.Locations, AttribColumnType));
            ColList.Add(new GridColumnAttrib(DevicesCols.PO, "PO Number"));
            ColList.Add(new GridColumnAttrib(DevicesCols.PurchaseDate, "Purchase Date"));
            ColList.Add(new GridColumnAttrib(DevicesCols.ReplacementYear, "Replace Year"));
            ColList.Add(new GridColumnAttrib(DevicesCols.LastModDate, "Modified"));
            ColList.Add(new GridColumnAttrib(DevicesCols.DeviceGuid, "Guid"));
            return ColList;
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
                bolGridFilling = true;
                if (CurrentTransaction != null)
                {
                    ResultGrid.Populate(results, ResultGridColumns(), true);
                }
                else
                {
                    ResultGrid.Populate(results, ResultGridColumns());
                }
                ResultGrid.FastAutoSizeColumns();

                bolGridFilling = false;
                DisplayRecords(ResultGrid.Rows.Count);
                ResultGrid.ScrollBars = ScrollBars.Both;
                ResultGrid.ResumeLayout();
            }
        }

        private void ShowAll()
        {
            var cmd = DBFactory.GetDatabase().GetCommand(Queries.SelectDevicesTable);
            StartBigQuery(cmd);
        }

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
                if (!QueryRunning)
                {
                    OtherFunctions.SetWaitCursor(true, this);
                    StripSpinner.Visible = true;
                    SetStatusBar("Background query running...");
                    QueryRunning = true;
                    var Results = await Task.Run(() =>
                    {
                        LastCommand = QryCommand;
                        return DBFactory.GetDatabase().DataTableFromCommand(QryCommand, CurrentTransaction);
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
                QueryRunning = false;
                DoneWaiting();
            }
        }

        private void StartUserManager()
        {
            SecurityTools.CheckForAccess(SecurityGroups.IsAdmin);
            new UserManagerForm(this);
        }

        private void SetStatusBar(string text, int timeOut = 0)
        {
            if (StatusStrip1.InvokeRequired)
            {
                StatusVoidDelegate d = new StatusVoidDelegate(SetStatusBar);
                StatusStrip1.Invoke(d, new object[] { text, timeOut });
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
                ServerTimeVoidDelegate d = new ServerTimeVoidDelegate(SetServerTime);
                StatusStrip1.Invoke(d, new object[] { serverTime });
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

        private void ChangeDatabase(Database database)
        {
            try
            {
                if (CurrentTransaction == null)
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
                                    SecurityTools.GetUserAccess();
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
                case Database.asset_manager:
                    this.Text = "Asset Manager - Main";
                    break;

                case Database.test_db:
                    this.Text = "Asset Manager - Main - ****TEST DATABASE****";
                    break;

                case Database.vintondd:
                    this.Text = "Asset Manager - Main - Vinton DD";
                    break;
            }
        }

        private void ShowTestDBWarning()
        {
            if (ServerInfo.CurrentDataBase == Database.test_db)
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

        private void Waiting()
        {
            OtherFunctions.SetWaitCursor(true, this);
            SetStatusBar("Processing...");
        }

        #region "Control Event Methods"

        private void AdvancedSearchMenuItem_Click(object sender, EventArgs e)
        {
            StartAdvancedSearch();
        }

        private void AddDeviceTool_Click(object sender, EventArgs e)
        {
            AddNewDevice();
        }

        private void cmdClear_Click(object sender, EventArgs e)
        {
            Clear_All();
        }

        private void cmdSearch_Click(object sender, EventArgs e)
        {
            DynamicSearch();
        }

        private void cmdShowAll_Click(object sender, EventArgs e)
        {
            ShowAll();
        }

        private void cmdSibi_Click(object sender, EventArgs e)
        {
            OpenSibiMainForm();
        }

        private void cmdSupDevSearch_Click(object sender, EventArgs e)
        {
            try
            {
                DataTable results = AssetManagerFunctions.DevicesBySupervisor(this);
                if (results != null)
                {
                    var newGridForm = new GridForm(this, "Devices By Supervisor");
                    newGridForm.AddGrid("DevBySup", "Devices", results);
                    newGridForm.Show();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
            finally
            {
                DoneWaiting();
            }
        }

        private void CopyTool_Click(object sender, EventArgs e)
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
            MyLiveBox.HideLiveBox();
        }

        private void ResultGrid_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!bolGridFilling)
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
                LoadDevice(ResultGrid.CurrentRowStringValue(DevicesCols.DeviceGuid));
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

        private void tsmAddGKUpdate_Click(object sender, EventArgs e)
        {
            EnqueueGKUpdate();
        }

        private void tsmGKUpdater_Click(object sender, EventArgs e)
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

        private void tsmSendToGridForm_Click(object sender, EventArgs e)
        {
            ResultGrid.CopyToGridForm(this);
        }

        private void tsmUserManager_Click(object sender, EventArgs e)
        {
            StartUserManager();
        }

        private void txtGuid_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                LoadDevice(txtGuid.Text.Trim());
                txtGuid.Clear();
            }
        }

        private void ViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LoadDevice(ResultGrid.CurrentRowStringValue(DevicesCols.DeviceGuid));
        }

        private void ReEnterLACredentialsToolStripMenuItem_Click(object sender, EventArgs e)
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
            ChangeDatabase((Database)DatabaseToolCombo.SelectedIndex);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            Application.DoEvents();
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Helpers.ChildFormControl.SplashScreenInstance().Dispose();
            MemoryTweaks.SetWorkingSet();
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
            LoadDevice(ResultGrid.CurrentRowStringValue(DevicesCols.DeviceGuid));
        }

        public override bool OkToClose()
        {
            if (CurrentTransaction != null)
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

                    LastCommand.Dispose();
                    MyLiveBox.Dispose();
                    MyMunisToolBar.Dispose();
                    MyWindowList.Dispose();
                    WatchDog.Dispose();
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