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
        private Device currentTrackingDevice;
        private TrackingEntry checkData;

        public TrackDeviceForm(Device device, ExtendedForm parentForm) : base(parentForm)
        {
            InitializeComponent();
            currentTrackingDevice = device;
            ClearAll();
            SetDates();
            SetGroups();
            LoadTracking();
            Show();
        }

        private bool GetCheckData()
        {
            checkData = new TrackingEntry();
            if (!currentTrackingDevice.Tracking.IsCheckedOut)
            {
                foreach (Control c in CheckOutBox.Controls)
                {
                    if (c is TextBox)
                    {
                        if (c.Visible)
                        {
                            if (c.Text.Trim() == "")
                            {
                                OtherFunctions.Message("Please complete all fields.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Data", this);
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
                            OtherFunctions.Message("Please complete all fields.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Data", this);
                            return false;
                        }
                    }
                }
            }
            checkData.CheckoutTime = dtCheckOut.Value;
            checkData.DueBackTime = dtDueBack.Value;
            checkData.UseLocation = txtUseLocation.Text.Trim().ToUpper();
            checkData.UseReason = txtUseReason.Text.Trim().ToUpper();
            checkData.CheckinNotes = txtCheckInNotes.Text.Trim().ToUpper();
            checkData.GUID = currentTrackingDevice.GUID;
            checkData.CheckoutUser = NetworkInfo.LocalDomainUser;
            checkData.CheckinTime = dtCheckIn.Value;
            checkData.CheckinUser = NetworkInfo.LocalDomainUser;
            return true;
        }

        private void LoadTracking()
        {
            txtAssetTag.Text = currentTrackingDevice.AssetTag;
            txtDescription.Text = currentTrackingDevice.Description;
            txtSerial.Text = currentTrackingDevice.Serial;
            txtDeviceType.Text = AttributeFunctions.GetDisplayValueFromCode(GlobalInstances.DeviceAttribute.EquipType, currentTrackingDevice.EquipmentType);
            if (currentTrackingDevice.Tracking.IsCheckedOut)
            {
                dtCheckOut.Value = currentTrackingDevice.Tracking.CheckoutTime;
                dtDueBack.Value = currentTrackingDevice.Tracking.DueBackTime;
                txtUseLocation.Text = currentTrackingDevice.Tracking.UseLocation;
                txtUseReason.Text = currentTrackingDevice.Tracking.UseReason;
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
            CheckInBox.Enabled = currentTrackingDevice.Tracking.IsCheckedOut;
            CheckOutBox.Enabled = !currentTrackingDevice.Tracking.IsCheckedOut;
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
                        rows += DBFactory.GetDatabase().UpdateValue(DevicesCols.TableName, DevicesCols.CheckedOut, 1, DevicesCols.DeviceUID, currentTrackingDevice.GUID, trans);

                        ParamCollection checkParams = new ParamCollection();
                        checkParams.Add(TrackablesCols.CheckType, CheckType.Checkout);
                        checkParams.Add(TrackablesCols.CheckoutTime, checkData.CheckoutTime);
                        checkParams.Add(TrackablesCols.DueBackDate, checkData.DueBackTime);
                        checkParams.Add(TrackablesCols.CheckoutUser, checkData.CheckoutUser);
                        checkParams.Add(TrackablesCols.UseLocation, checkData.UseLocation);
                        checkParams.Add(TrackablesCols.Notes, checkData.UseReason);
                        checkParams.Add(TrackablesCols.DeviceUID, checkData.GUID);
                        rows += DBFactory.GetDatabase().InsertFromParameters(TrackablesCols.TableName, checkParams.Parameters, trans);

                        if (rows == 2)
                        {
                            trans.Commit();
                            OtherFunctions.Message("Device Checked Out!", MessageBoxButtons.OK, MessageBoxIcon.Information, "Success", this);
                        }
                        else
                        {
                            trans.Rollback();
                            OtherFunctions.Message("Unsuccessful! The number of affected rows was not expected.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Unexpected Result", this);
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
                        rows += DBFactory.GetDatabase().UpdateValue(DevicesCols.TableName, DevicesCols.CheckedOut, 0, DevicesCols.DeviceUID, currentTrackingDevice.GUID, trans);

                        ParamCollection checkParams = new ParamCollection();

                        checkParams.Add(TrackablesCols.CheckType, CheckType.Checkin);
                        checkParams.Add(TrackablesCols.CheckoutTime, checkData.CheckoutTime);
                        checkParams.Add(TrackablesCols.DueBackDate, checkData.DueBackTime);
                        checkParams.Add(TrackablesCols.CheckinTime, checkData.CheckinTime);
                        checkParams.Add(TrackablesCols.CheckoutUser, checkData.CheckoutUser);
                        checkParams.Add(TrackablesCols.CheckinUser, checkData.CheckinUser);
                        checkParams.Add(TrackablesCols.UseLocation, checkData.UseLocation);
                        checkParams.Add(TrackablesCols.Notes, checkData.CheckinNotes);
                        checkParams.Add(TrackablesCols.DeviceUID, checkData.GUID);
                        rows += DBFactory.GetDatabase().InsertFromParameters(TrackablesCols.TableName, checkParams.Parameters, trans);

                        if (rows == 2)
                        {
                            trans.Commit();
                            OtherFunctions.Message("Device Checked In!", MessageBoxButtons.OK, MessageBoxIcon.Information, "Success", this);
                        }
                        else
                        {
                            trans.Rollback();
                            OtherFunctions.Message("Unsuccessful! The number of affected rows was not what was expected.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Unexpected Result", this);
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