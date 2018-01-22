using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.UserInterface.CustomControls;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class UpdateDev : ExtendedForm
    {
        public DeviceUpdateInfoStruct UpdateInfo
        {
            get { return NewUpdateInfo; }
        }

        private DeviceUpdateInfoStruct NewUpdateInfo;

        public UpdateDev(ExtendedForm parentForm, bool isNoteOnly = false)
        {
            InitializeComponent();
            this.ParentForm = parentForm;
            AttributeFunctions.FillComboBox(GlobalInstances.DeviceAttribute.ChangeType, UpdateTypeCombo);
            if (isNoteOnly)
            {
                UpdateTypeCombo.SelectedIndex = AttributeFunctions.GetComboIndexFromCode(GlobalInstances.DeviceAttribute.ChangeType, "NOTE");
                UpdateTypeCombo.Enabled = false;
                ValidateUpdateType();
            }
            else
            {
                UpdateTypeCombo.SelectedIndex = -1;
            }
            ShowDialog(parentForm);
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            NewUpdateInfo.Note = NotesTextBox.Rtf.Trim();
            NewUpdateInfo.ChangeType = AttributeFunctions.GetDBValue(GlobalInstances.DeviceAttribute.ChangeType, UpdateTypeCombo.SelectedIndex);
            NotesTextBox.Text = "";
            UpdateTypeCombo.Enabled = true;
            this.DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private bool ValidateUpdateType()
        {
            if (UpdateTypeCombo.SelectedIndex > -1)
            {
                ErrorProvider.SetError(UpdateTypeCombo, "");
                SubmitButton.Enabled = true;
                return true;
            }
            else
            {
                SubmitButton.Enabled = false;
                UpdateTypeCombo.Focus();
                ErrorProvider.SetError(UpdateTypeCombo, "Please select a change type.");
            }
            return false;
        }

        private void UpdateTypeCombo_ChangeType_Validating(object sender, CancelEventArgs e)
        {
            e.Cancel = !ValidateUpdateType();
        }

        private void UpdateTypeCombo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            ValidateUpdateType();
        }
    }
}