using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Tools;
using System.ComponentModel;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    /// <summary>
    /// Custom form with project specific properties and methods.
    /// </summary>
    public class ExtendedForm : Form
    {
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
        }

        public ExtendedForm(ExtendedForm parentForm)
        {
            ParentForm = parentForm;
            MemoryTweaks.SetWorkingSet();
        }

        public ExtendedForm(ExtendedForm parentForm, MappableObject currentObject)
        {
            ParentForm = parentForm;
            FormGuid = currentObject.Guid;
            MemoryTweaks.SetWorkingSet();
        }

        public ExtendedForm(ExtendedForm parentForm, string formGuid)
        {
            ParentForm = parentForm;
            FormGuid = formGuid;
            MemoryTweaks.SetWorkingSet();
        }

        public ExtendedForm(ExtendedForm parentForm, bool inheritTheme = true)
        {
            this.inheritTheme = inheritTheme;
            ParentForm = parentForm;
            MemoryTweaks.SetWorkingSet();
        }

        protected override CreateParams CreateParams
        {
            // Enables double-buffering.
            get
            {
                bool designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
                bool terminalSession = System.Windows.Forms.SystemInformation.TerminalServerSession;
                CreateParams cp = base.CreateParams;

                if (!designMode)
                {
                    if (!terminalSession)
                    {
                        cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
                    }
                }
                return cp;
            }
        }

        public virtual bool OkToClose()
        {
            if (this.Owner != null)
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