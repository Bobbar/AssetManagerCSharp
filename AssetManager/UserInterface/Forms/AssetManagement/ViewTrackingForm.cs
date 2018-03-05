using AssetManager.UserInterface.CustomControls;
using AssetManager.Data.Classes;
using AssetManager.Data;
using AssetManager.Data.Communications;
using AssetManager.Helpers;
using System;
using System.Data;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class ViewTrackingForm : ExtendedForm
    {
        public ViewTrackingForm(ExtendedForm parentForm, string entryGuid, Device device) : base(parentForm,entryGuid)
        {
            InitializeComponent();
            ViewTrackingEntry(entryGuid, device);
            Show();
        }

        private void ViewTrackingEntry(string entryGuid, Device device)
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectTrackingEntryByGuid(entryGuid)))
                {
                    foreach (DataRow r in results.Rows)
                    {
                        txtTimeStamp.Text = DataConsistency.NoNull(r[TrackablesCols.DateStamp]);
                        txtCheckType.Text = DataConsistency.NoNull(r[TrackablesCols.CheckType]);
                        if (txtCheckType.Text == "IN")
                        {
                            txtCheckType.BackColor = Colors.CheckIn;
                        }
                        else if (txtCheckType.Text == "OUT")
                        {
                            txtCheckType.BackColor = Colors.CheckOut;
                        }
                        txtDescription.Text = device.Description;
                        txtGuid.Text = DataConsistency.NoNull(r[TrackablesCols.DeviceGuid]);
                        txtCheckOutUser.Text = DataConsistency.NoNull(r[TrackablesCols.CheckoutUser]);
                        txtCheckInUser.Text = DataConsistency.NoNull(r[TrackablesCols.CheckinUser]);
                        txtLocation.Text = DataConsistency.NoNull(r[TrackablesCols.UseLocation]);
                        txtAssetTag.Text = device.AssetTag;
                        txtCheckOutTime.Text = DataConsistency.NoNull(r[TrackablesCols.CheckoutTime]);
                        txtDueBack.Text = DataConsistency.NoNull(r[TrackablesCols.DueBackDate]);
                        txtSerial.Text = device.Serial;
                        txtCheckInTime.Text = DataConsistency.NoNull(r[TrackablesCols.CheckinTime]);
                        txtNotes.Text = DataConsistency.NoNull(r[TrackablesCols.Notes]);
                        txtEntryGuid.Text = DataConsistency.NoNull(r[TrackablesCols.Guid]);
                        this.Text = this.Text + " - " + DataConsistency.NoNull(r[TrackablesCols.DateStamp]);
                    }
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

        private void cmdClose_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }
    }
}