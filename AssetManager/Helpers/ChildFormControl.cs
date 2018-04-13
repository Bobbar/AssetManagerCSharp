using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AssetManagement;
using AssetManager.UserInterface.Forms.Gatekeeper;
using AssetManager.UserInterface.Forms;
using AssetManager.Data.Classes;
using System;
using System.Windows.Forms;
using System.Diagnostics.CodeAnalysis;

namespace AssetManager.Helpers
{
    public static class ChildFormControl
    {
        public static void ActivateForm(ExtendedForm form)
        {
            if (!form.IsDisposed)
            {
                form.Show();
                form.Activate();
                form.WindowState = FormWindowState.Normal;
            }
        }

        public static bool AttachmentsIsOpen(ExtendedForm parentForm)
        {
            var attachForm = FindChildOfType(parentForm, typeof(AttachmentsForm));
            if (attachForm != null)
            {
                ActivateForm(attachForm);
                return true;
            }
            return false;
        }

        [SuppressMessage("Microsoft.Design", "CA1806")]
        public static void LookupDevice(ExtendedForm parentForm, Device device)
        {
            if (device != null)
            {
                if (!FormIsOpenByGuid(typeof(ViewDeviceForm), device.Guid))
                {
                    new ViewDeviceForm(parentForm, device);
                }
            }
            else
            {
                OtherFunctions.Message("Device not found.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Error", parentForm);
            }
        }

        public static void MinimizeChildren(ExtendedForm parentForm)
        {
            foreach (var child in parentForm.ChildForms)
            {
                child.WindowState = FormWindowState.Minimized;
            }
        }

        public static void RestoreChildren(ExtendedForm parentForm)
        {
            foreach (var child in parentForm.ChildForms)
            {
                child.WindowState = FormWindowState.Normal;
            }
        }

        public static ExtendedForm FindChildOfType(ExtendedForm parentForm, Type childType)
        {
            var findForm = parentForm.ChildForms.Find(f => f.GetType() == childType);
            if (findForm != null)
            {
                return findForm;
            }
            return null;
        }

        public static bool FormTypeIsOpen(Type formType)
        {
            foreach (ExtendedForm frm in Application.OpenForms)
            {
                if (frm.GetType() == formType)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Find a form by its type and returns it if found.
        /// </summary>
        /// <param name="formType"></param>
        /// <returns></returns>
        public static ExtendedForm FindFormByType(Type formType)
        {
            foreach (ExtendedForm frm in Application.OpenForms)
            {
                if (frm.GetType() == formType)
                {
                    return frm;
                }
            }
            return null;
        }

        public static bool FormIsOpenByGuid(Type formType, string guid)
        {
            foreach (ExtendedForm frm in Application.OpenForms)
            {
                if (frm.GetType() == formType && frm.FormGuid == guid)
                {
                    ActivateForm(frm);
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Returns the current instance of <see cref="GKUpdaterForm"/>. If one does not exists, creates new instance and returns it.
        /// </summary>
        /// <returns></returns>
        public static GKUpdaterForm GKUpdaterInstance()
        {
            GKUpdaterForm currentGKUpdInstance;
            //Check for current instance.
            if (!FormTypeIsOpen(typeof(GKUpdaterForm)))
            {
                //If no current instance, create a new one and return it.
                currentGKUpdInstance = new GKUpdaterForm(MainFormInstance());
                return currentGKUpdInstance;
            }
            else
            {
                //If an instance is found, return the current instance.
                currentGKUpdInstance = (GKUpdaterForm)FindFormByType(typeof(GKUpdaterForm));
                if (currentGKUpdInstance != null)
                {
                    return currentGKUpdInstance;
                }
            }
            return null;
        }

        public static SplashScreenForm SplashScreenInstance()
        {
            SplashScreenForm currentSplashScreenInstance;

            if (!FormTypeIsOpen(typeof(SplashScreenForm)))
            {
                currentSplashScreenInstance = new SplashScreenForm();
                return currentSplashScreenInstance;
            }
            else
            {
                currentSplashScreenInstance = (SplashScreenForm)FindFormByType(typeof(SplashScreenForm));
                if (currentSplashScreenInstance != null)
                {
                    return currentSplashScreenInstance;
                }
            }
            return null;
        }

        public static MainForm MainFormInstance()
        {
            return (MainForm)FindFormByType(typeof(MainForm));
        }
    }
}