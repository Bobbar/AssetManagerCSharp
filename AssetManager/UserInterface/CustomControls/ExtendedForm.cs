using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Tools;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections;
using System.Collections.Generic;
using System;

namespace AssetManager.UserInterface.CustomControls
{
    /// <summary>
    /// Custom form with project specific properties and methods.
    /// </summary>
    public class ExtendedForm : Form
    {
        private List<ExtendedForm> childForms = new List<ExtendedForm>();

        private bool inheritTheme = true;

        public bool InheritTheme
        {
            get
            {
                return this.inheritTheme;
            }
            set
            {
                this.inheritTheme = value;
            }
        }

        private ExtendedForm parentForm;

        /// <summary>
        /// Gets or sets the Grid Theme for the DataGridView controls within the form.
        /// </summary>
        /// <returns></returns>
        public GridTheme GridTheme { get; set; }

        /// <summary>
        /// Unique identifying string used to locate specific instances of this form.
        /// </summary>
        /// <returns></returns>
        public string FormGuid { get; set; }

        /// <summary>
        /// Overloads the stock ParentForm property with a read/writable one. And also sets the icon and <seealso cref="GridTheme"/> from the parent form.
        /// </summary>
        /// <returns></returns>
        public new ExtendedForm ParentForm
        {
            get
            {
                return parentForm;
            }

            set
            {
                this.parentForm = value;
                if (inheritTheme)
                {
                    SetTheme(this.parentForm);
                }
            }
        }

        public ExtendedForm()
        {
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        public ExtendedForm(ExtendedForm parentForm)
        {
            ParentForm = parentForm;
            MemoryTweaks.SetWorkingSet();
            parentForm.AddChild(this);
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        public ExtendedForm(ExtendedForm parentForm, MappableObject currentObject)
        {
            ParentForm = parentForm;
            FormGuid = currentObject.Guid;
            MemoryTweaks.SetWorkingSet();
            parentForm.AddChild(this);
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        public ExtendedForm(ExtendedForm parentForm, string formGuid)
        {
            ParentForm = parentForm;
            FormGuid = formGuid;
            MemoryTweaks.SetWorkingSet();
            parentForm.AddChild(this);
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        public ExtendedForm(ExtendedForm parentForm, bool inheritTheme = true)
        {
            this.inheritTheme = inheritTheme;
            ParentForm = parentForm;
            MemoryTweaks.SetWorkingSet();
            parentForm.AddChild(this);
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        public void AddChild(ExtendedForm child)
        {
            childForms.Add(child);
            parentForm?.AddChild(child);

            Console.WriteLine(this.Text + ": add : " + childForms.Count);
        }

        public void RemoveChild(ExtendedForm child)
        {
            childForms.Remove(child);
            parentForm?.RemoveChild(child);

            Console.WriteLine(this.Text + ": remove : " + childForms.Count);
        }


        private void ExtendedForm_Disposed(object sender, System.EventArgs e)
        {
            if (!IsDisposed)
            {
                this.Disposed -= ExtendedForm_Disposed;
                this.FormClosing -= ExtendedForm_FormClosing;
                CloseChildren();
                parentForm?.RemoveChild(this);
            }
        }

        private void ExtendedForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!OkToClose() || !OkToCloseChildren())
            {
                e.Cancel = true;
            }
        }

        public bool OkToCloseChildren()
        {
            bool closeAllowed = true;
            if (childForms.Count > 0)
            {
                foreach (var child in childForms)
                {
                    if (!child.OkToClose())
                    {
                        closeAllowed = false;
                    }
                }
            }
            return closeAllowed;
        }

        public void CloseChildren()
        {
            while (childForms.Count > 0)
            {
                childForms[0].Dispose();
            }
        }

        //protected override CreateParams CreateParams
        //{
        //    // Enables double-buffering.
        //    get
        //    {
        //        bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
        //        bool terminalSession = System.Windows.Forms.SystemInformation.TerminalServerSession;
        //        CreateParams cp = base.CreateParams;

        //        if (!designMode)
        //        {
        //            if (!terminalSession)
        //            {
        //                cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
        //            }
        //        }
        //        return cp;
        //    }
        //}

        public virtual bool OkToClose()
        {
            if (!this.Modal && this.Owner != null)
            {
                this.Owner.Activate();
                this.Owner.WindowState = FormWindowState.Normal;
                this.Focus();
                return false;
            }

            return true;
        }

        /// <summary>
        /// Override and add code to refresh data from the database.
        /// </summary>
        public virtual void RefreshData()
        {
            this.Refresh();
        }

        private void SetTheme(ExtendedForm parentForm)
        {
            Icon = parentForm.Icon;
            GridTheme = parentForm.GridTheme;
        }
    }
}