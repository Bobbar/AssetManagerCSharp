﻿using AssetManager.Data.Classes;
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
            this.CacheControlImages = false;
            this.Owner = parentForm;
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
