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
    public partial class ViewHistoryForm : ExtendedForm
    {
        private DBControlParser controlParser;

        private string deviceGuid;

        public ViewHistoryForm(ExtendedForm parentForm, string entryUID, string deviceGuid) : base(parentForm,entryUID)
        {
            controlParser = new DBControlParser(this);
            InitializeComponent();
            InitDBControls();
            this.deviceGuid = deviceGuid;
            ViewEntry(entryUID);
        }

        //TODO: Iterate through properties and dynamically generate controls at runtime.
        private void InitDBControls()
        {
            txtEntryTime.Tag = new DBControlInfo(HistoricalDevicesCols.ActionDateTime, ParseType.DisplayOnly, false);
            txtActionUser.Tag = new DBControlInfo(HistoricalDevicesCols.ActionUser, ParseType.DisplayOnly, false);
            txtChangeType.Tag = new DBControlInfo(HistoricalDevicesCols.ChangeType, Attributes.DeviceAttribute.ChangeType, ParseType.DisplayOnly, false);
            txtDescription.Tag = new DBControlInfo(HistoricalDevicesCols.Description, ParseType.DisplayOnly, false);
            txtGuid.Tag = new DBControlInfo(HistoricalDevicesCols.DeviceUID, ParseType.DisplayOnly, false);
            txtCurrentUser.Tag = new DBControlInfo(HistoricalDevicesCols.CurrentUser, ParseType.DisplayOnly, false);
            txtLocation.Tag = new DBControlInfo(HistoricalDevicesCols.Location, Attributes.DeviceAttribute.Locations, ParseType.DisplayOnly, false);
            txtPONumber.Tag = new DBControlInfo(HistoricalDevicesCols.PO, ParseType.DisplayOnly, false);
            txtAssetTag.Tag = new DBControlInfo(HistoricalDevicesCols.AssetTag, ParseType.DisplayOnly, false);
            txtPurchaseDate.Tag = new DBControlInfo(HistoricalDevicesCols.PurchaseDate, ParseType.DisplayOnly, false);
            txtOSVersion.Tag = new DBControlInfo(HistoricalDevicesCols.OSVersion, Attributes.DeviceAttribute.OSType, ParseType.DisplayOnly, false);
            txtSerial.Tag = new DBControlInfo(HistoricalDevicesCols.Serial, ParseType.DisplayOnly, false);
            txtReplaceYear.Tag = new DBControlInfo(HistoricalDevicesCols.ReplacementYear, ParseType.DisplayOnly, false);
            txtEQType.Tag = new DBControlInfo(HistoricalDevicesCols.EQType, Attributes.DeviceAttribute.EquipType, ParseType.DisplayOnly, false);
            NotesTextBox.Tag = new DBControlInfo(HistoricalDevicesCols.Notes, ParseType.DisplayOnly, false);
            txtStatus.Tag = new DBControlInfo(HistoricalDevicesCols.Status, Attributes.DeviceAttribute.StatusType, ParseType.DisplayOnly, false);
            txtEntryGuid.Tag = new DBControlInfo(HistoricalDevicesCols.HistoryEntryUID, ParseType.DisplayOnly, false);
            chkTrackable.Tag = new DBControlInfo(HistoricalDevicesCols.Trackable, ParseType.DisplayOnly, false);
            txtPhoneNumber.Tag = new DBControlInfo(HistoricalDevicesCols.PhoneNumber, ParseType.DisplayOnly, false);
            txtHostname.Tag = new DBControlInfo(HistoricalDevicesCols.HostName, ParseType.DisplayOnly, false);
            iCloudTextBox.Tag = new DBControlInfo(HistoricalDevicesCols.iCloudAccount, ParseType.DisplayOnly, false);
        }

        private void FillControls(DataTable data)
        {
            controlParser.FillDBFields(data);
            this.Text = this.Text + " - " + DataConsistency.NoNull(data.Rows[0][HistoricalDevicesCols.ActionDateTime]);
        }

        private void ViewEntry(string entryUID)
        {
            OtherFunctions.SetWaitCursor(true, this);
            try
            {
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectHistoricalDeviceEntry(entryUID)))
                {
                    HighlightChangedFields(results);
                    FillControls(results);
                    Show();
                    Activate();
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

        /// <summary>
        /// Returns a list of controls whose value differs from the previous historical state.
        /// </summary>
        /// <param name="currentData"></param>
        /// <returns></returns>
        private List<Control> GetChangedFields(DataTable currentData)
        {
            List<Control> ChangedControls = new List<Control>();
            System.DateTime CurrentTimeStamp = (System.DateTime)currentData.Rows[0][HistoricalDevicesCols.ActionDateTime];
            //Query for all rows with a timestamp older than the current historical entry.
            using (DataTable olderData = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectDevHistoricalEntriesOlderThan(deviceGuid, CurrentTimeStamp)))
            {
                if (olderData.Rows.Count > 0)
                {
                    //Declare the current and previous DataRows.
                    DataRow PreviousRow = olderData.Rows[0];
                    DataRow CurrentRow = currentData.Rows[0];
                    List<string> ChangedColumns = new List<string>();
                    //Iterate through the CurrentRow item array and compare them to the PreviousRow items.
                    for (int i = 0; i <= CurrentRow.ItemArray.Length - 1; i++)
                    {
                        if (PreviousRow[i].ToString() != CurrentRow[i].ToString())
                        {
                            //Add column names to a list if the item values don't match.
                            ChangedColumns.Add(PreviousRow.Table.Columns[i].ColumnName);
                        }
                    }
                    //Get a list of all the controls with DBControlInfo tags.
                    var ControlList = controlParser.GetDBControls(this);
                    //Get a list of all the controls whose data columns match the ChangedColumns.
                    foreach (string col in ChangedColumns)
                    {
                        ChangedControls.Add(ControlList.Find(c => ((DBControlInfo)c.Tag).DataColumn == col));
                    }
                }
            }
            return ChangedControls;
        }

        private void HighlightChangedFields(DataTable currentData)
        {
            //Iterate through the list of changed fields and set the background color to highlight them.
            foreach (Control ctl in GetChangedFields(currentData))
            {
                ctl.BackColor = Colors.CheckIn;
            }
            NotesTextBox.BackColor = Color.White;
        }

        private void ViewHistoryForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Dispose();
        }
    }
}