using AssetManager.Business;
using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.Tools;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class ViewDeviceForm
    {
        #region Control Events

        private void StatusSlider_NewMessageDisplayed(object sender, MessageEventArgs e)
        {
            var flashColor = StyleFunctions.ColorAlphaBlend(e.Message.TextColor, Color.White);
            StatusStrip.FlashStrip(flashColor, 3);
        }

        void ILiveBox.DynamicSearch()
        {
            DynamicSearch();
        }

        void ILiveBox.LoadDevice(string deviceGuid)
        {
            LoadDevice(deviceGuid);
        }

        protected void DynamicSearch()
        {
            throw new NotImplementedException();
        }

        protected void LoadDevice(string deviceGuid)
        {
            throw new NotImplementedException();
        }

        private void AcceptToolButton_Click(object sender, EventArgs e)
        {
            AcceptChanges();
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void AssetDisposalFormToolItem_Click(object sender, EventArgs e)
        {
            new PdfFormFilling(this, currentViewDevice, PdfFormFilling.PdfFormType.DisposeForm);
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void AssetInputFormToolItem_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(currentViewDevice.PO))
            {
                new PdfFormFilling(this, currentViewDevice, PdfFormFilling.PdfFormType.InputForm);
            }
            else
            {
                OtherFunctions.Message("Please add a valid PO number to this device.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Info", this);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void AssetTransferFormToolItem_Click(object sender, EventArgs e)
        {
            new PdfFormFilling(this, currentViewDevice, PdfFormFilling.PdfFormType.TransferForm);
        }

        private void AttachmentsToolButton_Click(object sender, EventArgs e)
        {
            ViewAttachments();
        }

        private void CancelToolButton_Click(object sender, EventArgs e)
        {
            CancelModify();
        }

        private void CheckInTool_Click(object sender, EventArgs e)
        {
            StartTrackDeviceForm();
        }

        private void CheckOutTool_Click(object sender, EventArgs e)
        {
            StartTrackDeviceForm();
        }

        private void CurrentUserTextBox_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (editMode)
            {
                MunisUser = new MunisEmployee();
            }
        }

        private void DataGridHistory_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            NewEntryView();
        }

        private void DataGridHistory_CellEnter(object sender, DataGridViewCellEventArgs e)
        {
            if (!gridFilling)
            {
                StyleFunctions.HighlightRow(DataGridHistory, GridTheme, e.RowIndex);
            }
        }

        private void DataGridHistory_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow(DataGridHistory, e.RowIndex);
        }

        private void DataGridHistory_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right && e.ColumnIndex > -1 && e.RowIndex > -1)
            {
                DataGridHistory.CurrentCell = DataGridHistory[e.ColumnIndex, e.RowIndex];
            }
        }

        private void DeleteDeviceToolButton_Click(object sender, EventArgs e)
        {
            DeleteDevice();
        }

        private void DeleteEntryToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteSelectedHistoricalEntry();
        }

        private void GuidLabel_Click(object sender, EventArgs e)
        {
            Clipboard.SetText(GuidLabel.Text);
            StatusPrompt("GUID copied to clipboard");
        }

        private void ModifyToolButton_Click(object sender, EventArgs e)
        {
            ModifyDevice();
        }

        private void MunisInfoButton_Click(object sender, EventArgs e)
        {
            try
            {
                MunisFunctions.LoadMunisInfoByDevice(currentViewDevice, this);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void MunisSearchButton_Click(object sender, EventArgs e)
        {
            MunisUser = MunisFunctions.MunisUserSearch(this);
        }

        private void NewNoteToolButton_Click(object sender, EventArgs e)
        {
            AddNewNote();
        }

        private void PhoneNumberTextBox_Leave(object sender, EventArgs e)
        {
            if (!DataConsistency.ValidPhoneNumber(PhoneNumberTextBox.Text))
            {
                OtherFunctions.Message("Invalid phone number.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
            }
        }

        private void PingHistLabel_Click(object sender, EventArgs e)
        {
            AssetManagerFunctions.ShowPingHistory(this, currentViewDevice);
        }

        private void RefreshToolButton_Click(object sender, EventArgs e)
        {
            RefreshData();
        }

        private void RemoteToolsControl_HostBackOnline(object sender, EventArgs e)
        {
            TaskBarNotify.FlashWindow(this.Handle, true, true, 10);
        }

        private void RemoteToolsControl_HostOnlineStatus(object sender, bool e)
        {
            // If OnlineStatusChanged handle is not null, Invoke it.
            OnlineStatusChanged?.Invoke(this, e);
        }

        private void remoteToolsControl_NewStatusPrompt(object sender, UserPromptEventArgs e)
        {
            StatusPrompt(e.Text, e.Color, e.DisplayTime);
        }

        private void RemoteToolsControl_VisibleChanging(object sender, bool e)
        {
            if (e)
            {
                ExpandSplitter(true);
            }
            else
            {
                ExpandSplitter(TrackingBox.Visible);
            }
        }

        private void SibiViewButton_Click(object sender, EventArgs e)
        {
            OpenSibiLink(currentViewDevice);
        }

        private void TabControl1_MouseDown(object sender, MouseEventArgs e)
        {
            TrackingGrid.Refresh();
        }

        private void TrackingGrid_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            var EntryGuid = TrackingGrid.CurrentRowStringValue(TrackablesCols.Guid);
            if (!Helpers.ChildFormControl.FormIsOpenByGuid(typeof(ViewTrackingForm), EntryGuid))
            {
                NewTrackingView(EntryGuid);
            }
        }

        private void TrackingGrid_RowPrePaint(object sender, DataGridViewRowPrePaintEventArgs e)
        {
            string checkTypeValue = TrackingGrid.Rows[e.RowIndex].Cells[TrackablesCols.CheckType].Value.ToString();
            DataGridViewCell CheckTypeCell = TrackingGrid.Rows[e.RowIndex].Cells[TrackablesCols.CheckType];
            CheckTypeCell.Style.ForeColor = Color.Black;
            if (checkTypeValue == CheckType.Checkin)
            {
                CheckTypeCell.Style.BackColor = Colors.CheckIn;
            }
            else if (checkTypeValue == CheckType.Checkout)
            {
                CheckTypeCell.Style.BackColor = Colors.CheckOut;
            }
        }

        private void ViewDeviceForm_Resize(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                Helpers.ChildFormControl.MinimizeChildren(this);
            }
        }

        #endregion Control Events
    }
}