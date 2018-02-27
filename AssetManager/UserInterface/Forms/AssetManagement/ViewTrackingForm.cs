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
        public ViewTrackingForm(ExtendedForm parentForm, string entryGUID, Device device) : base(parentForm,entryGUID)
        {
            InitializeComponent();
            ViewTrackingEntry(entryGUID, device);
            Show();
        }

        private void ViewTrackingEntry(string entryUID, Device device)
        {
            try
            {
                OtherFunctions.SetWaitCursor(true, this);
                using (DataTable results = DBFactory.GetDatabase().DataTableFromQueryString(Queries.SelectTrackingEntryByGUID(entryUID)))
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
                        txtGUID.Text = DataConsistency.NoNull(r[TrackablesCols.DeviceUID]);
                        txtCheckOutUser.Text = DataConsistency.NoNull(r[TrackablesCols.CheckoutUser]);
                        txtCheckInUser.Text = DataConsistency.NoNull(r[TrackablesCols.CheckinUser]);
                        txtLocation.Text = DataConsistency.NoNull(r[TrackablesCols.UseLocation]);
                        txtAssetTag.Text = device.AssetTag;
                        txtCheckOutTime.Text = DataConsistency.NoNull(r[TrackablesCols.CheckoutTime]);
                        txtDueBack.Text = DataConsistency.NoNull(r[TrackablesCols.DueBackDate]);
                        txtSerial.Text = device.Serial;
                        txtCheckInTime.Text = DataConsistency.NoNull(r[TrackablesCols.CheckinTime]);
                        txtNotes.Text = DataConsistency.NoNull(r[TrackablesCols.Notes]);
                        txtEntryGUID.Text = DataConsistency.NoNull(r[TrackablesCols.UID]);
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