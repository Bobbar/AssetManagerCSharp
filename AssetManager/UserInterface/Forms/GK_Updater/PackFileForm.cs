using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms.GK_Updater
{
    public partial class PackFileForm : ExtendedForm
    {
        public bool PackVerified { get; set; }
        private bool working = false;

        private ManagePackFile packManager = new ManagePackFile();

        public PackFileForm(bool showFunctions) : base()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.asset_icon;
            FunctionPanel.Visible = showFunctions;
            if (!showFunctions)
            {
                CheckPackFile();
            }
        }

        private async void CheckPackFile()
        {
            try
            {
                working = true;
                PackVerified = await packManager.ProcessPackFile();
                working = false;
                if (this.Modal)
                    this.Close();
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void ProgressTimer_Tick(object sender, EventArgs e)
        {
            try
            {
                if (packManager.Progress.Percent > 0 && working)
                {
                    ProgressBar.Value = packManager.Progress.Percent;
                    packManager.Progress.Tick();
                    if (packManager.Progress.Throughput > 0 & packManager.Progress.Percent < 100)
                    {
                        if (!SpeedLabel.Visible)
                            SpeedLabel.Visible = true;
                        SpeedLabel.Text = packManager.Progress.Throughput.ToString() + " MB/s";
                    }
                    else
                    {
                        SpeedLabel.Visible = false;
                    }
                }
                StatusLabel.Text = packManager.Status;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void VerifyPackButton_Click(object sender, EventArgs e)
        {
            if (working) return;
            try
            {
                var GKFormInstance = Helpers.ChildFormControl.GKUpdaterInstance();
                if (!GKFormInstance.ActiveUpdates())
                {
                    CheckPackFile();
                }
                else
                {
                    OtherFunctions.Message("This process will interfere with the active running updates. Please stop all updates and try again.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Cannot Continue", this);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private async void NewPackFile()
        {
            try
            {
                working = true;
                if (!await packManager.CreateNewPackFile())
                {
                    OtherFunctions.Message("Error while creating a new pack file.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", this);
                }
                working = false;
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void NewPackButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (working) return;

                var prompt = OtherFunctions.Message("Are you sure? This will replace the packfile on the server with a new one created from the local GK Directory.", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation, "Warning", this);
                if (prompt == DialogResult.OK)
                {
                    NewPackFile();
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod());
            }
        }

        private void PackFileForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (working)
            {
                var prompt = OtherFunctions.Message("Are you sure you want to cancel the operation?", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "Cancel?", this);

                if (prompt == DialogResult.Yes)
                {
                    packManager.CancelCopy();
                    e.Cancel = true;
                }
                else
                {
                    e.Cancel = true;
                }
            }
        }
    }
}