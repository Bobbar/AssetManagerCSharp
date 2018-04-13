using AssetManager.Data.Classes;
using AssetManager.Security;
using AssetManager.Tools;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.Gatekeeper;
using System;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.Forms.AdminTools
{
    public partial class CopyFilesForm : ExtendedForm
    {
        private FileTransferUI pushFilesControl;
        private bool cancel = false;
        public CopyFilesForm(ExtendedForm parentForm, Device targetDevice, string sourceDirectory, string targetDirectory) : base(parentForm)
        {
            InitializeComponent();
            this.Owner = parentForm;
            pushFilesControl = new FileTransferUI(this, targetDevice, true, sourceDirectory, targetDirectory, "Push Files");
            this.Controls.Add(pushFilesControl);
            pushFilesControl.CriticalStopError += new System.EventHandler(CopyCritcalError);
            this.Show();
        }

        public async Task<bool> StartCopy()
        {
            try
            {
                pushFilesControl.StartUpdate();

                var Done = await Task.Run(() =>
                {
                    while (!(pushFilesControl.ProgStatus != ProgressStatus.Running && pushFilesControl.ProgStatus != ProgressStatus.Starting))
                    {
                        if (cancel)
                        {
                            pushFilesControl.CancelUpdate();
                            return false;
                        }
                        Task.Delay(1000).Wait();
                    }
                    if (pushFilesControl.ProgStatus != ProgressStatus.CompleteWithErrors && pushFilesControl.ProgStatus != ProgressStatus.Complete)
                    {
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                });
                return Done;
            }
            catch (Exception ex)
            {
                throw (ex);
            }

        }

        private void CopyCritcalError(object sender, EventArgs e)
        {
            cancel = true;
            SecurityTools.ClearAdminCreds();
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

                    pushFilesControl?.Dispose();
                    cancel = true;
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}
