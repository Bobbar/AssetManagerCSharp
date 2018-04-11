using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    public partial class UpdateTypeForm : ExtendedForm
    {
        public DeviceUpdateInfo UpdateInfo
        {
            get { return NewUpdateInfo; }
        }

        private DeviceUpdateInfo NewUpdateInfo;

        public UpdateTypeForm(ExtendedForm parentForm, bool isNoteOnly = false) : base(parentForm)
        {
            InitializeComponent();
            UpdateTypeCombo.FillComboBox(Attributes.DeviceAttributes.ChangeType);
            if (isNoteOnly)
            {
                UpdateTypeCombo.SetSelectedAttribute(Attributes.DeviceAttributes.ChangeType["NOTE"]);
                UpdateTypeCombo.Enabled = false;
                ValidateUpdateType();
            }
            else
            {
                UpdateTypeCombo.SelectedIndex = -1;
            }
        }

        private void SubmitButton_Click(object sender, EventArgs e)
        {
            NewUpdateInfo.Note = NotesTextBox.Rtf.Trim();
            NewUpdateInfo.ChangeType = UpdateTypeCombo.SelectedValue.ToString();
            NotesTextBox.Text = "";
            UpdateTypeCombo.Enabled = true;
            this.DialogResult = DialogResult.OK;
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
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