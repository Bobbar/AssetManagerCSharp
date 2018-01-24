using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class TrackDeviceForm : ExtendedForm
    {
        private Device CurrentTrackingDevice = new Device();
        private TrackingEntry CheckData = new TrackingEntry();

        public TrackDeviceForm(Device device, ExtendedForm parentForm)
        {
            InitializeComponent();
            CurrentTrackingDevice = device;
            this.ParentForm = parentForm;
            ClearAll();
            SetDates();
            SetGroups();
            LoadTracking();
            Show();
        }

        private bool GetCheckData()
        {
            if (!CurrentTrackingDevice.Tracking.IsCheckedOut)
            {
                foreach (Control c in CheckOutBox.Controls)
                {
                    if (c is TextBox)
                    {
                        if (c.Visible)
                        {
                            if (c.Text.Trim() == "")
                            {
                                OtherFunctions.Message("Please complete all fields.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Missing Data", this);
                                return false;
                            }
                        }
                    }
                }
            }
            else
            {
                foreach (Control c in CheckInBox.Controls)
                {
                    if (c is TextBox)
                    {
                        if (c.Text.Trim() == "")
                        {
                            OtherFunctions.Message("Please complete all fields.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Missing Data", this);
                            return false;
                        }
                    }
                }
            }
            CheckData.CheckoutTime = dtCheckOut.Value;
            CheckData.DueBackTime = dtDueBack.Value;
            CheckData.UseLocation = txtUseLocation.Text.Trim().ToUpper();
            CheckData.UseReason = txtUseReason.Text.Trim().ToUpper();
            CheckData.CheckinNotes = txtCheckInNotes.Text.Trim().ToUpper();
            CheckData.GUID = CurrentTrackingDevice.GUID;
            CheckData.CheckoutUser = NetworkInfo.LocalDomainUser;
            CheckData.CheckinTime = dtCheckIn.Value;
            CheckData.CheckinUser = NetworkInfo.LocalDomainUser;
            return true;
        }

        private void LoadTracking()
        {
            txtAssetTag.Text = CurrentTrackingDevice.AssetTag;
            txtDescription.Text = CurrentTrackingDevice.Description;
            txtSerial.Text = CurrentTrackingDevice.Serial;
            txtDeviceType.Text = AttributeFunctions.GetDisplayValueFromCode(GlobalInstances.DeviceAttribute.EquipType, CurrentTrackingDevice.EquipmentType);
            if (CurrentTrackingDevice.Tracking.IsCheckedOut)
            {
                dtCheckOut.Value = CurrentTrackingDevice.Tracking.CheckoutTime;
                dtDueBack.Value = CurrentTrackingDevice.Tracking.DueBackTime;
                txtUseLocation.Text = CurrentTrackingDevice.Tracking.UseLocation;
                txtUseReason.Text = CurrentTrackingDevice.Tracking.UseReason;
            }
        }

        private void ClearAll()
        {
            foreach (Control c in this.Controls)
            {
                if (c is GroupBox)
                {
                    foreach (Control gc in c.Controls)
                    {
                        if (gc is TextBox)
                        {
                            TextBox txt = (TextBox)gc;
                            txt.Text = "";
                        }
                    }
                }
            }
        }

        private void SetDates()
        {
            dtCheckOut.Value = DateTime.Now;
            dtCheckIn.Value = DateTime.Now;
            dtCheckOut.Enabled = false;
            dtCheckIn.Enabled = false;
        }

        private void SetGroups()
        {
            CheckInBox.Enabled = CurrentTrackingDevice.Tracking.IsCheckedOut;
            CheckOutBox.Enabled = !CurrentTrackingDevice.Tracking.IsCheckedOut;
        }

        private void CheckOut()
        {
            using (var trans = DBFactory.GetDatabase().StartTransaction())
            {
                using (var conn = trans.Connection)
                {
                    try
                    {
                        if (!GetCheckData())
                        {
                            return;
                        }
                        OtherFunctions.SetWaitCursor(true, this);
                        int rows = 0;
                        rows += DBFactory.GetDatabase().UpdateValue(DevicesCols.TableName, DevicesCols.CheckedOut, 1, DevicesCols.DeviceUID, CurrentTrackingDevice.GUID, trans);

                        ParamCollection checkParams = new ParamCollection();
                        checkParams.Add(TrackablesCols.CheckType, CheckType.Checkout);
                        checkParams.Add(TrackablesCols.CheckoutTime, CheckData.CheckoutTime);
                        checkParams.Add(TrackablesCols.DueBackDate, CheckData.DueBackTime);
                        checkParams.Add(TrackablesCols.CheckoutUser, CheckData.CheckoutUser);
                        checkParams.Add(TrackablesCols.UseLocation, CheckData.UseLocation);
                        checkParams.Add(TrackablesCols.Notes, CheckData.UseReason);
                        checkParams.Add(TrackablesCols.DeviceUID, CheckData.GUID);
                        rows += DBFactory.GetDatabase().InsertFromParameters(TrackablesCols.TableName, checkParams.Parameters, trans);

                        if (rows == 2)
                        {
                            trans.Commit();
                            OtherFunctions.Message("Device Checked Out!", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Success", this);
                        }
                        else
                        {
                            trans.Rollback();
                            OtherFunctions.Message("Unsuccessful! The number of affected rows was not expected.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Unexpected Result", this);
                        }
                        ParentForm.RefreshData();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                    }
                    finally
                    {
                        this.Dispose();
                    }
                }
            }
        }

        private void CheckIn()
        {
            using (var trans = DBFactory.GetDatabase().StartTransaction())
            {
                using (var conn = trans.Connection)
                {
                    try
                    {
                        if (!GetCheckData())
                        {
                            return;
                        }
                        OtherFunctions.SetWaitCursor(true, this);
                        int rows = 0;
                        rows += DBFactory.GetDatabase().UpdateValue(DevicesCols.TableName, DevicesCols.CheckedOut, 0, DevicesCols.DeviceUID, CurrentTrackingDevice.GUID, trans);

                        ParamCollection checkParams = new ParamCollection();

                        checkParams.Add(TrackablesCols.CheckType, CheckType.Checkin);
                        checkParams.Add(TrackablesCols.CheckoutTime, CheckData.CheckoutTime);
                        checkParams.Add(TrackablesCols.DueBackDate, CheckData.DueBackTime);
                        checkParams.Add(TrackablesCols.CheckinTime, CheckData.CheckinTime);
                        checkParams.Add(TrackablesCols.CheckoutUser, CheckData.CheckoutUser);
                        checkParams.Add(TrackablesCols.CheckinUser, CheckData.CheckinUser);
                        checkParams.Add(TrackablesCols.UseLocation, CheckData.UseLocation);
                        checkParams.Add(TrackablesCols.Notes, CheckData.CheckinNotes);
                        checkParams.Add(TrackablesCols.DeviceUID, CheckData.GUID);
                        rows += DBFactory.GetDatabase().InsertFromParameters(TrackablesCols.TableName, checkParams.Parameters, trans);

                        if (rows == 2)
                        {
                            trans.Commit();
                            OtherFunctions.Message("Device Checked In!", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Information, "Success", this);
                        }
                        else
                        {
                            trans.Rollback();
                            OtherFunctions.Message("Unsuccessful! The number of affected rows was not what was expected.", (int)MessageBoxButtons.OK + (int)MessageBoxIcon.Exclamation, "Unexpected Result", this);
                        }
                        ParentForm.RefreshData();
                    }
                    catch (Exception ex)
                    {
                        trans.Rollback();
                        ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
                    }
                    finally
                    {
                        this.Dispose();
                    }
                }
            }
        }

        private void cmdCheckOut_Click(object sender, EventArgs e)
        {
            CheckOut();
        }

        private void cmdCheckIn_Click(object sender, EventArgs e)
        {
            CheckIn();
        }

        private void txtUseLocation_LostFocus(object sender, EventArgs e)
        {
            txtUseLocation.Text = txtUseLocation.Text.Trim().ToUpper();
        }

        private void txtUseReason_LostFocus(object sender, EventArgs e)
        {
            txtUseReason.Text = txtUseReason.Text.Trim().ToUpper();
        }

        private void txtCheckInNotes_LostFocus(object sender, EventArgs e)
        {
            txtCheckInNotes.Text = txtCheckInNotes.Text.Trim().ToUpper();
        }
    }
}