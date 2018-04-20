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

        public ViewHistoryForm(ExtendedForm parentForm, string entryGuid, string deviceGuid) : base(parentForm, entryGuid)
        {
            controlParser = new DBControlParser(this);
            InitializeComponent();
            InitDBControls();
            this.deviceGuid = deviceGuid;
            ViewEntry(entryGuid);
        }

        //TODO: Iterate through properties and dynamically generate controls at runtime.
        private void InitDBControls()
        {
            EntryTimeTextBox.SetDBInfo(HistoricalDevicesCols.ActionDateTime, ParseType.DisplayOnly, false);
            ActionUserTextBox.SetDBInfo(HistoricalDevicesCols.ActionUser, ParseType.DisplayOnly, false);
            ChangeTypeTextBox.SetDBInfo(HistoricalDevicesCols.ChangeType, Attributes.DeviceAttributes.ChangeType, ParseType.DisplayOnly, false);
            DescriptionTextBox.SetDBInfo(HistoricalDevicesCols.Description, ParseType.DisplayOnly, false);
            GuidTextBox.SetDBInfo(HistoricalDevicesCols.DeviceGuid, ParseType.DisplayOnly, false);
            CurrentUserTextBox.SetDBInfo(HistoricalDevicesCols.CurrentUser, ParseType.DisplayOnly, false);
            LocationTextBox.SetDBInfo(HistoricalDevicesCols.Location, Attributes.DeviceAttributes.Locations, ParseType.DisplayOnly, false);
            PONumberTextBox.SetDBInfo(HistoricalDevicesCols.PO, ParseType.DisplayOnly, false);
            AssetTagTextBox.SetDBInfo(HistoricalDevicesCols.AssetTag, ParseType.DisplayOnly, false);
            PurchaseDateTextBox.SetDBInfo(HistoricalDevicesCols.PurchaseDate, ParseType.DisplayOnly, false);
            OSVersionTextBox.SetDBInfo(HistoricalDevicesCols.OSVersion, Attributes.DeviceAttributes.OSType, ParseType.DisplayOnly, false);
            SerialTextBox.SetDBInfo(HistoricalDevicesCols.Serial, ParseType.DisplayOnly, false);
            ReplaceYearTextBox.SetDBInfo(HistoricalDevicesCols.ReplacementYear, ParseType.DisplayOnly, false);
            EQTypeTextBox.SetDBInfo(HistoricalDevicesCols.EQType, Attributes.DeviceAttributes.EquipType, ParseType.DisplayOnly, false);
            NotesTextBox.SetDBInfo(HistoricalDevicesCols.Notes, ParseType.DisplayOnly, false);
            StatusTextBox.SetDBInfo(HistoricalDevicesCols.Status, Attributes.DeviceAttributes.StatusType, ParseType.DisplayOnly, false);
            EntryGuidTextBox.SetDBInfo(HistoricalDevicesCols.HistoryEntryGuid, ParseType.DisplayOnly, false);
            chkTrackable.SetDBInfo(HistoricalDevicesCols.Trackable, ParseType.DisplayOnly, false);
            PhoneNumberTextBox.SetDBInfo(HistoricalDevicesCols.PhoneNumber, ParseType.DisplayOnly, false);
            HostnameTextBox.SetDBInfo(HistoricalDevicesCols.HostName, ParseType.DisplayOnly, false);
            iCloudTextBox.SetDBInfo(HistoricalDevicesCols.iCloudAccount, ParseType.DisplayOnly, false);
        }

        private void FillControls(DataTable data)
        {
            controlParser.FillDBFields(data);
            this.Text = this.Text + " - " + DataConsistency.NoNull(data.Rows[0][HistoricalDevicesCols.ActionDateTime]);
        }

        private void ViewEntry(string entryGuid)
        {
            OtherFunctions.SetWaitCursor(true, this);
            try
            {
                using (var results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectHistoricalDeviceEntry(entryGuid)))
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
            var changedControls = new List<Control>();
            var currentTimeStamp = (System.DateTime)currentData.Rows[0][HistoricalDevicesCols.ActionDateTime];
           
            // Query for all rows with a timestamp older than the current historical entry.
            using (var olderData = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectDevHistoricalEntriesOlderThan(deviceGuid, currentTimeStamp)))
            {
                if (olderData.Rows.Count > 0)
                {
                    // Declare the current and previous DataRows.
                    DataRow previousRow = olderData.Rows[0];
                    DataRow currentRow = currentData.Rows[0];
                    var changedColumns = new List<string>();

                    // Iterate through the current row item array and compare them to the previous row items.
                    for (int i = 0; i <= currentRow.ItemArray.Length - 1; i++)
                    {
                        if (previousRow[i].ToString() != currentRow[i].ToString())
                        {
                            //Add column names to a list if the item values don't match.
                            changedColumns.Add(previousRow.Table.Columns[i].ColumnName);
                        }
                    }

                    // Get a list of all the controls with DBControlInfo tags.
                    var controlList = controlParser.GetDBControls(this);

                    // Get a list of all the controls whose data columns match the ChangedColumns.
                    foreach (string col in changedColumns)
                    {
                        changedControls.Add(controlList.Find(c => ((DBControlInfo)c.Tag).ColumnName == col));
                    }
                }
            }
            return changedControls;
        }

        private void HighlightChangedFields(DataTable currentData)
        {
            // Iterate through the list of changed fields and set the background color to highlight them.
            foreach (var ctl in GetChangedFields(currentData))
            {
                ctl.BackColor = Colors.CheckIn;
            }

            NotesTextBox.BackColor = Color.White;
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