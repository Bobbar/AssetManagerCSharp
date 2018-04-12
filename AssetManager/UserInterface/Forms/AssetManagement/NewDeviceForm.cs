using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class NewDeviceForm : ExtendedForm, ILiveBox
    {
        #region Fields

        private MunisEmployee _munisUser;

        private DBControlParser controlParser;

        private LiveBox liveBox;

        private string newGuid;

        private int replacementYears = 4;

        public MunisEmployee MunisUser
        {
            get
            {
                return _munisUser;
            }
            set
            {
                _munisUser = value;
                LockUnlockUserField();
            }
        }

        #endregion Fields

        #region Constructors

        public NewDeviceForm(ExtendedForm parentForm) : base(parentForm)
        {
            InitializeComponent();
            InitDBControls();
            controlParser = new DBControlParser(this);
            controlParser.EnableFieldValidation();
            ClearAll();
            this.Show();
            this.Activate();
        }

        #endregion Constructors

        #region Methods

        void ILiveBox.DynamicSearch()
        {
            DynamicSearch();
        }

        public void ImportFromSibi(string itemGuid)
        {
            string itemQuery = Queries.SelectSibiRequestAndItemByItemGuid(itemGuid);
            DateTime POPurchaseDate = default(DateTime);
            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(itemQuery))
            {
                controlParser.FillDBFields(results, ImportColumnRemaps());
                MunisUser = LevenshteinSearch.SmartEmployeeSearch(results.Rows[0][SibiRequestItemsCols.User].ToString().ToUpper());
                POPurchaseDate = MunisFunctions.GetPODate(results.Rows[0][SibiRequestCols.PO].ToString());
            }

            CurrentUserTextBox.Text = MunisUser.Name;
            CheckFields();
            PurchaseDatePicker.Value = POPurchaseDate;
        }

        void ILiveBox.LoadDevice(string deviceGuid)
        {
            LoadDevice(deviceGuid);
        }

        public override bool OkToClose()
        {
            var prompt = OtherFunctions.Message("Close this window?", MessageBoxButtons.OKCancel, MessageBoxIcon.Question, "Are you sure?", this);
            if (prompt == DialogResult.OK)
            {
                return true;
            }
            return false;
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

                    liveBox.Dispose();
                    controlParser.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        protected void DynamicSearch()
        {
            throw new NotImplementedException();
        }

        protected void LoadDevice(string deviceGuid)
        {
            throw new NotImplementedException();
        }
        private void AddButton_Click(object sender, EventArgs e)
        {
            AddDevice();
        }

        private void AddDevice()
        {
            try
            {
                if (!CheckFields())
                {
                    OtherFunctions.Message("Some required fields are missing or invalid.  Please fill and/or verify all highlighted fields.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Data", this);
                    return;
                }
                else
                {
                    if (AssetManagerFunctions.DeviceExists(AssetTagTextBox.Text.ToString().Trim(), SerialTextBox.Text.ToString().Trim()))
                    {
                        OtherFunctions.Message("A device with that serial and/or asset tag already exists.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Duplicate Device", this);
                        return;
                    }
                    bool Success = AddNewDevice();
                    if (Success)
                    {
                        ParentForm.RefreshData();
                        var blah = OtherFunctions.Message("New Device Added.   Add another?", MessageBoxButtons.YesNo, MessageBoxIcon.Information, "Complete", this);
                        if (!NoClearCheckBox.Checked)
                        {
                            ClearAll();
                        }
                        if (blah == DialogResult.No)
                        {
                            this.Dispose();
                        }
                    }
                    else
                    {
                        OtherFunctions.Message("Something went wrong while adding a new device.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Unexpected Result", this);
                    }

                    return;
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                OtherFunctions.Message("Unable to add new device.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
            }
        }

        private bool AddNewDevice()
        {
            using (var trans = DBFactory.GetDatabase().StartTransaction())
            {
                using (var conn = trans.Connection)
                {
                    try
                    {
                        newGuid = Guid.NewGuid().ToString();
                        int rows = 0;
                        string DeviceInsertQry = "SELECT * FROM " + DevicesCols.TableName + " LIMIT 0";
                        string HistoryInsertQry = "SELECT * FROM " + HistoricalDevicesCols.TableName + " LIMIT 0";

                        rows += DBFactory.GetDatabase().UpdateTable(DeviceInsertQry, DeviceInsertTable(DeviceInsertQry), trans);
                        rows += DBFactory.GetDatabase().UpdateTable(HistoryInsertQry, HistoryInsertTable(HistoryInsertQry), trans);

                        if (rows == 2)
                        {
                            trans.Commit();
                            return true;
                        }
                        else
                        {
                            trans.Rollback();
                            return false;
                        }
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
                        return false;
                    }
                }
            }
        }

        private bool CheckFields()
        {
            bool validFields = true;
            validFields = controlParser.ValidateFields();

            LockUnlockUserField();

            if (PhoneNumTextBox.Text.Trim() != "" && !DataConsistency.ValidPhoneNumber(PhoneNumTextBox.Text))
            {
                validFields = false;
                controlParser.SetError(PhoneNumTextBox, false);
                OtherFunctions.Message("Invalid phone number.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
            }

            return validFields;
        }

        private void ClearAll()
        {
            RefreshCombos();
            MunisUser = new MunisEmployee();
            ClearFields(this);
            PurchaseDatePicker.Value = DateTime.Now;
            StatusComboBox.SetSelectedAttribute(Attributes.DeviceAttributes.StatusType["INSRV"]);
            TrackableCheckBox.Checked = false;
            NoClearCheckBox.Checked = false;
            controlParser.ClearErrors();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
        }

        private void ClearFields(Control Parent)
        {
            foreach (Control ctl in Parent.Controls)
            {
                if (ctl is TextBox)
                {
                    TextBox txt = (TextBox)ctl;
                    txt.Text = "";
                    txt.ReadOnly = false;
                }
                if (ctl is ComboBox)
                {
                    ComboBox cmb = (ComboBox)ctl;
                    cmb.SelectedIndex = -1;
                }
                if (ctl.HasChildren)
                {
                    ClearFields(ctl);
                }
            }
        }

        private void CurrentUserTextBox_DoubleClick(object sender, EventArgs e)
        {
            CurrentUserTextBox.ReadOnly = false;
            MunisUser = new MunisEmployee();
            CurrentUserTextBox.SelectAll();
        }

        private DataTable DeviceInsertTable(string selectQuery)
        {
            var tmpTable = controlParser.ReturnInsertTable(selectQuery);
            var DBRow = tmpTable.Rows[0];
            //Add Add'l info
            if (!string.IsNullOrEmpty(MunisUser.Number))
            {
                DBRow[DevicesCols.CurrentUser] = MunisUser.Name;
                DBRow[DevicesCols.MunisEmpNum] = MunisUser.Number;
            }
            DBRow[DevicesCols.LastModUser] = NetworkInfo.LocalDomainUser;
            DBRow[DevicesCols.LastModDate] = DateTime.Now;
            DBRow[DevicesCols.DeviceGuid] = newGuid;
            DBRow[DevicesCols.CheckedOut] = false;
            return tmpTable;
        }

        private DataTable HistoryInsertTable(string selectQuery)
        {
            var tmpTable = controlParser.ReturnInsertTable(selectQuery);
            var DBRow = tmpTable.Rows[0];
            //Add Add'l info
            DBRow[HistoricalDevicesCols.ChangeType] = "NEWD";
            DBRow[HistoricalDevicesCols.Notes] = NotesTextBox.Text.ToString().Trim();
            DBRow[HistoricalDevicesCols.ActionUser] = NetworkInfo.LocalDomainUser;
            DBRow[HistoricalDevicesCols.DeviceGuid] = newGuid;
            return tmpTable;
        }

        private List<DBRemappingInfo> ImportColumnRemaps()
        {
            List<DBRemappingInfo> newMap = new List<DBRemappingInfo>();
            newMap.Add(new DBRemappingInfo(SibiRequestItemsCols.User, DevicesCols.CurrentUser));
            newMap.Add(new DBRemappingInfo(SibiRequestItemsCols.NewAsset, DevicesCols.AssetTag));
            newMap.Add(new DBRemappingInfo(SibiRequestItemsCols.NewSerial, DevicesCols.Serial));
            newMap.Add(new DBRemappingInfo(SibiRequestItemsCols.Description, DevicesCols.Description));
            newMap.Add(new DBRemappingInfo(SibiRequestItemsCols.Location, DevicesCols.Location));
            newMap.Add(new DBRemappingInfo(SibiRequestCols.PO, DevicesCols.PO));
            return newMap;
        }

        private void InitDBControls()
        {
            DescriptionTextBox.SetDBInfo(DevicesBaseCols.Description, true);
            AssetTagTextBox.SetDBInfo(DevicesBaseCols.AssetTag, true);
            SerialTextBox.SetDBInfo(DevicesBaseCols.Serial, true);
            PurchaseDatePicker.SetDBInfo(DevicesBaseCols.PurchaseDate, true);
            ReplaceYearTextBox.SetDBInfo(DevicesBaseCols.ReplacementYear, false);
            LocationComboBox.SetDBInfo(DevicesBaseCols.Location, Attributes.DeviceAttributes.Locations, true);
            CurrentUserTextBox.SetDBInfo(DevicesBaseCols.CurrentUser, true);
            // txtNotes.SetDBInfo(historical_dev.Notes, False)
            OSTypeComboBox.SetDBInfo(DevicesBaseCols.OSVersion, Attributes.DeviceAttributes.OSType, true);
            PhoneNumTextBox.SetDBInfo(DevicesBaseCols.PhoneNumber, false);
            EquipTypeComboBox.SetDBInfo(DevicesBaseCols.EQType, Attributes.DeviceAttributes.EquipType, true);
            StatusComboBox.SetDBInfo(DevicesBaseCols.Status, Attributes.DeviceAttributes.StatusType, true);
            TrackableCheckBox.SetDBInfo(DevicesBaseCols.Trackable, false);
            POTextBox.SetDBInfo(DevicesBaseCols.PO, false);
            HostnameTextBox.SetDBInfo(DevicesBaseCols.HostName, false);
            iCloudTextBox.SetDBInfo(DevicesBaseCols.iCloudAccount, false);
        }

        private void LockUnlockUserField()
        {
            if (!string.IsNullOrEmpty(MunisUser.Number))
            {
                CurrentUserTextBox.BackColor = Colors.EditColor;
                CurrentUserTextBox.ReadOnly = true;
                ToolTip1.SetToolTip(CurrentUserTextBox, "Munis Linked Employee - Double-Click to change.");
            }
            else
            {
                CurrentUserTextBox.BackColor = Color.Empty;
                CurrentUserTextBox.ReadOnly = false;
                ToolTip1.SetToolTip(CurrentUserTextBox, "");
            }
        }

        private void MunisSearchButton_Click(object sender, EventArgs e)
        {
            MunisUser = MunisFunctions.MunisUserSearch(this);
            if (!string.IsNullOrEmpty(MunisUser.Number))
            {
                CurrentUserTextBox.Text = MunisUser.Name;
                CurrentUserTextBox.ReadOnly = true;
            }
        }

        private void NewDeviceForm_Load(object sender, EventArgs e)
        {
            liveBox = new LiveBox(this);
            liveBox.AttachToControl(CurrentUserTextBox, DevicesCols.CurrentUser, LiveBoxSelectAction.UserSelect, DevicesCols.MunisEmpNum);
            liveBox.AttachToControl(DescriptionTextBox, DevicesCols.Description, LiveBoxSelectAction.SelectValue);
        }

        private void OSTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetHostname();
        }

        private void PhoneNumTextBox_Leave(object sender, EventArgs e)
        {
            if (PhoneNumTextBox.Text.ToString().Trim() != "" && !DataConsistency.ValidPhoneNumber(PhoneNumTextBox.Text))
            {
                OtherFunctions.Message("Invalid phone number.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
                PhoneNumTextBox.Focus();
            }
        }

        private void PurchaseDatePicker_ValueChanged(object sender, EventArgs e)
        {
            SetReplacementYear(PurchaseDatePicker.Value);
        }

        private void RefreshCombos()
        {
            LocationComboBox.FillComboBox(Attributes.DeviceAttributes.Locations);
            EquipTypeComboBox.FillComboBox(Attributes.DeviceAttributes.EquipType);
            OSTypeComboBox.FillComboBox(Attributes.DeviceAttributes.OSType);
            StatusComboBox.FillComboBox(Attributes.DeviceAttributes.StatusType);
        }

        private void SerialTextBox_TextChanged(object sender, EventArgs e)
        {
            SetHostname();
        }

        private void SetHostname()
        {
            if (OSTypeComboBox.SelectedValue != null)
            {
                bool windowsSelected = OSTypeComboBox.Text.ToString().ToUpper().Contains("WIN");
                if (SerialTextBox.Text != "" && windowsSelected)
                {
                    HostnameTextBox.Text = DataConsistency.DeviceHostnameFormat(SerialTextBox.Text);
                }
                else
                {
                    HostnameTextBox.Text = string.Empty;
                }
            }
        }

        private void SetReplacementYear(DateTime PurDate)
        {
            int ReplaceYear = PurDate.Year + replacementYears;
            ReplaceYearTextBox.Text = ReplaceYear.ToString();
        }

        #endregion Methods

    }
}