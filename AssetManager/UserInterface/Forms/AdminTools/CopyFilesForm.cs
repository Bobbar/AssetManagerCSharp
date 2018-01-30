﻿using AssetManager.Data.Classes;
using AssetManager.Security;
using AssetManager.Tools;
using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.GK_Updater;
using System;
using System.Threading.Tasks;

namespace AssetManager.UserInterface.Forms.AdminTools
{
    public partial class CopyFilesForm : ExtendedForm
    {
        private Device TargetDevice;
        private string SourceDirectory;
        private string TargetDirectory;
        private GKProgressControl PushFilesControl;
        private bool Cancel = false;
        public CopyFilesForm(ExtendedForm parentForm, Device targetDevice, string sourceDirectory, string targetDirectory) : base(parentForm)
        {
            InitializeComponent();
            ImageCaching.CacheControlImages(this);
            this.Owner = parentForm;
            this.TargetDevice = targetDevice;
            this.SourceDirectory = sourceDirectory;
            this.TargetDirectory = targetDirectory;
            PushFilesControl = new GKProgressControl(this, targetDevice, true, sourceDirectory, targetDirectory);
            this.Controls.Add(PushFilesControl);
            PushFilesControl.CriticalStopError += new System.EventHandler(CopyCritcalError);
            this.Show();

        }

        public async Task<bool> StartCopy()
        {
            try
            {
                PushFilesControl.StartUpdate();

                var Done = await Task.Run(() =>
                {
                    while (!(PushFilesControl.ProgStatus != GKProgressControl.ProgressStatus.Running && PushFilesControl.ProgStatus != GKProgressControl.ProgressStatus.Starting))
                    {
                        if (Cancel)
                        {
                            PushFilesControl.CancelUpdate();
                            return false;
                        }
                        Task.Delay(1000).Wait();
                    }
                    if (PushFilesControl.ProgStatus != GKProgressControl.ProgressStatus.CompleteWithErrors && PushFilesControl.ProgStatus != GKProgressControl.ProgressStatus.Complete)
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
            Cancel = true;
            SecurityTools.ClearAdminCreds();
        }

        private void CopyFilesForm_FormClosed(object sender, System.Windows.Forms.FormClosedEventArgs e)
        {
            if (PushFilesControl != null)
            {
                PushFilesControl.Dispose();
            }
            Cancel = true;
        }
    }
}
