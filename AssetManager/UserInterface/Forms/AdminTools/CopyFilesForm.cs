using AssetManager.Data.Classes;
using AssetManager.Security;
using AssetManager.Tools;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.GKUpdater;
using System;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.Forms.AdminTools
{
    public partial class CopyFilesForm : ExtendedForm
    {
        private GKProgressControl pushFilesControl;
        private bool cancel = false;
        public CopyFilesForm(ExtendedForm parentForm, Device targetDevice, string sourceDirectory, string targetDirectory) : base(parentForm)
        {
            InitializeComponent();
            this.Owner = parentForm;
            ImageCaching.CacheControlImages(this);
            pushFilesControl = new GKProgressControl(this, targetDevice, true, sourceDirectory, targetDirectory);
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
                    while (!(pushFilesControl.ProgStatus != GKProgressControl.ProgressStatus.Running && pushFilesControl.ProgStatus != GKProgressControl.ProgressStatus.Starting))
                    {
                        if (cancel)
                        {
                            pushFilesControl.CancelUpdate();
                            return false;
                        }
                        Task.Delay(1000).Wait();
                    }
                    if (pushFilesControl.ProgStatus != GKProgressControl.ProgressStatus.CompleteWithErrors && pushFilesControl.ProgStatus != GKProgressControl.ProgressStatus.Complete)
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

        private void CopyFilesForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (pushFilesControl != null)
            {
                pushFilesControl.Dispose();
            }
            cancel = true;
        }
    }
}
