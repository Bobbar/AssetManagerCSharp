using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Communications;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.CustomControls.LiveBox;
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
               
        private int replacementYears = 4;
        private DBControlParser controlParser;
        private LiveBox liveBox;
        private string newUID;

        #endregion Fields

        #region Methods

        public NewDeviceForm(ExtendedForm parentForm) : base(parentForm)
        {
            InitializeComponent();
            InitDBControls();
            controlParser = new DBControlParser(this);
            controlParser.EnableFieldValidation();
            this.Owner = parentForm;
            ClearAll();
            this.Show();
            this.Activate();
        }

        public void ImportFromSibi(string itemUID)
        {
            string itemQuery = Queries.SelectSibiRequestAndItemByItemGUID(itemUID);
            DateTime POPurchaseDate = default(DateTime);
            using (var results = DBFactory.GetDatabase().DataTableFromQueryString(itemQuery))
            {
                controlParser.FillDBFields(results, ImportColumnRemaps());
                MunisUser = AssetManagerFunctions.SmartEmployeeSearch(results.Rows[0][SibiRequestItemsCols.User].ToString().ToUpper());
                POPurchaseDate = MunisFunctions.GetPODate(results.Rows[0][SibiRequestCols.PO].ToString());
            }

            CurrentUserTextBox.Text = MunisUser.Name;
            CheckFields();
            PurchaseDatePicker.Value = POPurchaseDate;
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
                        var blah = OtherFunctions.Message("New Device Added.   Add another?", MessageBoxButtons.YesNo, MessageBoxIcon.Information, "Complete", this);
                        if (!NoClearCheckBox.Checked)
                        {
                            ClearAll();
                        }
                        if (blah == DialogResult.No)
                        {
                            this.Dispose();
                        }
                        ParentForm.RefreshData();
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
                        newUID = Guid.NewGuid().ToString();
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

        private void ClearAll()
        {
            RefreshCombos();
            MunisUser = new MunisEmployee();
            ClearFields(this);
            PurchaseDatePicker.Value = DateTime.Now;
            StatusComboBox.SelectedIndex = AttributeFunctions.GetComboIndexFromCode(Attributes.DeviceAttribute.StatusType, "INSRV");
            TrackableCheckBox.Checked = false;
            NoClearCheckBox.Checked = false;
            controlParser.ClearErrors();
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
            DBRow[DevicesCols.DeviceUID] = newUID;
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
            DBRow[HistoricalDevicesCols.DeviceUID] = newUID;
            return tmpTable;
        }

        private void InitDBControls()
        {
            DescriptionTextBox.Tag = new DBControlInfo(DevicesBaseCols.Description, true);
            AssetTagTextBox.Tag = new DBControlInfo(DevicesBaseCols.AssetTag, true);
            SerialTextBox.Tag = new DBControlInfo(DevicesBaseCols.Serial, true);
            PurchaseDatePicker.Tag = new DBControlInfo(DevicesBaseCols.PurchaseDate, true);
            ReplaceYearTextBox.Tag = new DBControlInfo(DevicesBaseCols.ReplacementYear, false);
            LocationComboBox.Tag = new DBControlInfo(DevicesBaseCols.Location, Attributes.DeviceAttribute.Locations, true);
            CurrentUserTextBox.Tag = new DBControlInfo(DevicesBaseCols.CurrentUser, true);
            // txtNotes.Tag = New DBControlInfo(historical_dev.Notes, False)
            OSTypeComboBox.Tag = new DBControlInfo(DevicesBaseCols.OSVersion, Attributes.DeviceAttribute.OSType, true);
            PhoneNumTextBox.Tag = new DBControlInfo(DevicesBaseCols.PhoneNumber, false);
            EquipTypeComboBox.Tag = new DBControlInfo(DevicesBaseCols.EQType, Attributes.DeviceAttribute.EquipType, true);
            StatusComboBox.Tag = new DBControlInfo(DevicesBaseCols.Status, Attributes.DeviceAttribute.StatusType, true);
            TrackableCheckBox.Tag = new DBControlInfo(DevicesBaseCols.Trackable, false);
            POTextBox.Tag = new DBControlInfo(DevicesBaseCols.PO, false);
            HostnameTextBox.Tag = new DBControlInfo(DevicesBaseCols.HostName, false);
            iCloudTextBox.Tag = new DBControlInfo(DevicesBaseCols.iCloudAccount, false);
        }

        private void RefreshCombos()
        {
            LocationComboBox.FillComboBox(Attributes.DeviceAttribute.Locations);
            EquipTypeComboBox.FillComboBox(Attributes.DeviceAttribute.EquipType);
            OSTypeComboBox.FillComboBox(Attributes.DeviceAttribute.OSType);
            StatusComboBox.FillComboBox(Attributes.DeviceAttribute.StatusType);
        }

        private void SetReplacementYear(DateTime PurDate)
        {
            int ReplaceYear = PurDate.Year + replacementYears;
            ReplaceYearTextBox.Text = ReplaceYear.ToString();
        }

        private void AddButton_Click(object sender, EventArgs e)
        {
            AddDevice();
        }

        private void ClearButton_Click(object sender, EventArgs e)
        {
            ClearAll();
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

        private void CurrentUserTextBox_DoubleClick(object sender, EventArgs e)
        {
            CurrentUserTextBox.ReadOnly = false;
            MunisUser = new MunisEmployee();
            CurrentUserTextBox.SelectAll();
        }

        private void PhoneNumTextBox_Leave(object sender, EventArgs e)
        {
            if (PhoneNumTextBox.Text.ToString().Trim() != "" && !DataConsistency.ValidPhoneNumber(PhoneNumTextBox.Text))
            {
                OtherFunctions.Message("Invalid phone number.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
                PhoneNumTextBox.Focus();
            }
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

        private void NewDeviceForm_Load(object sender, EventArgs e)
        {
            liveBox = new LiveBox(this);
            liveBox.AttachToControl(CurrentUserTextBox, DevicesCols.CurrentUser, LiveBox.LiveBoxSelectionType.UserSelect, DevicesCols.MunisEmpNum);
            liveBox.AttachToControl(DescriptionTextBox, DevicesCols.Description, LiveBox.LiveBoxSelectionType.SelectValue);
        }

        private void NewDeviceForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            liveBox.Dispose();
        }

        #endregion Methods

        private void OSTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            SetHostname();
        }

        private void PurchaseDatePicker_ValueChanged(object sender, EventArgs e)
        {
            SetReplacementYear(PurchaseDatePicker.Value);
        }

        void ILiveBox.LoadDevice(string deviceGUID)
        {
            throw new NotImplementedException();
        }

        void ILiveBox.DynamicSearch()
        {
            throw new NotImplementedException();
        }
    }
}