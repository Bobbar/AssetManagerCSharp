using AssetManager.UserInterface.CustomControls;
using AssetManager.UserInterface.Forms.AssetManagement;
using AssetManager.UserInterface.Forms.GKUpdater;
using AssetManager.UserInterface.Forms.Sibi;
using AssetManager.UserInterface.Forms;
using AssetManager.Data.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;


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
            foreach (ExtendedForm frm in GetChildren(parentForm))
            {
                if (frm is AttachmentsForm & object.ReferenceEquals(frm.ParentForm, parentForm))
                {
                    ActivateForm(frm);
                    return true;
                }
            }
            return false;
        }

        public static List<ExtendedForm> GetChildren(ExtendedForm parentForm)
        {
            return Application.OpenForms.OfType<ExtendedForm>().ToList().FindAll(f => object.ReferenceEquals(f.ParentForm, parentForm) & !f.IsDisposed);
        }

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
            foreach (ExtendedForm child in GetChildren(parentForm))
            {
                child.WindowState = FormWindowState.Minimized;
            }
        }

        public static void RestoreChildren(ExtendedForm parentForm)
        {
            foreach (ExtendedForm child in GetChildren(parentForm))
            {
                child.WindowState = FormWindowState.Normal;
            }
        }

        public static ExtendedForm GetChildOfType(ExtendedForm parentForm, Type childType)
        {
            return GetChildren(parentForm).Find(f => f.GetType() == childType);
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
                    return frm;
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
                currentGKUpdInstance = new GKUpdaterForm();
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