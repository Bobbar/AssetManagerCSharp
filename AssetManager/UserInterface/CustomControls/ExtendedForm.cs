using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.Tools;
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.ComponentModel;

namespace AssetManager.UserInterface.CustomControls
{
    /// <summary>
    /// Custom form with project specific properties, methods and child form handling.
    /// </summary>
    public class ExtendedForm : Form, IWindowList
    {
        #region Fields

        private List<ExtendedForm> childForms = new List<ExtendedForm>();

        private bool inheritTheme = true;
        private bool cacheImages = true;

        private ExtendedForm parentForm;

        #endregion Fields

        #region Constructors

        public ExtendedForm()
        {
            this.Load += ExtendedForm_Load;
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        public ExtendedForm(ExtendedForm parentForm)
        {
            ParentForm = parentForm;
            this.Load += ExtendedForm_Load;
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        public ExtendedForm(ExtendedForm parentForm, MappableObject currentObject)
        {
            ParentForm = parentForm;
            FormGuid = currentObject.Guid;
            this.Load += ExtendedForm_Load;
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        public ExtendedForm(ExtendedForm parentForm, string formGuid)
        {
            ParentForm = parentForm;
            FormGuid = formGuid;
            this.Load += ExtendedForm_Load;
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        public ExtendedForm(ExtendedForm parentForm, bool inheritTheme = true)
        {
            this.inheritTheme = inheritTheme;
            ParentForm = parentForm;
            this.Load += ExtendedForm_Load;
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        #endregion Constructors

        #region Events

        public event EventHandler<EventArgs> ChildCountChanged;

        #endregion Events

        #region Properties

        public List<ExtendedForm> ChildForms
        {
            get
            {
                return childForms;
            }
        }

        /// <summary>
        /// Unique identifying string used to locate specific instances of this form.
        /// </summary>
        /// <returns></returns>
        public string FormGuid { get; set; }

        /// <summary>
        /// Gets or sets the Grid Theme for the DataGridView controls within the form.
        /// </summary>
        /// <returns></returns>
        public GridTheme GridTheme { get; set; }

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

        /// <summary>
        /// Gets or sets the value indicating whether the controls will have their images cached with a global reference. See <see cref="ImageCaching"/>.
        /// </summary>
        public bool CacheControlImages
        {
            get
            {
                return cacheImages;
            }

            set
            {
                this.cacheImages = value;
            }
        }


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

        #endregion Properties

        #region Methods

        public void AddChild(ExtendedForm child)
        {
            childForms.Add(child);
            child.Disposed += Child_Disposed;
            OnWindowCountChanged(new EventArgs());
        }

        public int ChildFormCount()
        {
            return GetChildFormCount(this);
        }

        public void CloseChildren()
        {
            while (childForms.Count > 0)
            {
                childForms[0].Dispose();
            }
        }

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

        public bool OkToCloseChildren()
        {
            bool closeAllowed = true;
            if (childForms.Count > 0)
            {
                foreach (var child in childForms)
                {
                    if (!child.OkToClose() || !child.OkToCloseChildren())
                    {
                        closeAllowed = false;
                    }
                }
            }
            return closeAllowed;
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

        /// <summary>
        /// Override and add code to refresh data from the database.
        /// </summary>
        public virtual void RefreshData()
        {
            this.Refresh();
        }

        public void RemoveChild(ExtendedForm child)
        {
            childForms.Remove(child);
            child.Disposed -= Child_Disposed;
            OnWindowCountChanged(new EventArgs());
        }

        private void Child_Disposed(object sender, EventArgs e)
        {
            RemoveChild((ExtendedForm)sender);
        }

        private void ExtendedForm_Disposed(object sender, System.EventArgs e)
        {
            if (!IsDisposed)
            {
                this.Load -= ExtendedForm_Load;
                this.Disposed -= ExtendedForm_Disposed;
                this.FormClosing -= ExtendedForm_FormClosing;
                CloseChildren();
                parentForm = null;
                childForms.Clear();
                childForms = null;
            }
        }

        private void ExtendedForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!OkToClose() || !OkToCloseChildren())
            {
                e.Cancel = true;
            }
        }

        private void ExtendedForm_Load(object sender, EventArgs e)
        {
            parentForm?.AddChild(this);
            if (cacheImages) ImageCaching.CacheControlImages(this);
            MemoryTweaks.SetWorkingSet();
        }

        /// <summary>
        /// Recursively counts the number of child forms within the parent/child tree.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        private int GetChildFormCount(IWindowList parent)
        {
            int childCount = 0;

            foreach (var child in parent.ChildForms)
            {
                childCount += GetChildFormCount(child) + 1;
            }

            return childCount;
        }

        private void OnWindowCountChanged(EventArgs e)
        {
            ChildCountChanged?.Invoke(this, e);
            parentForm?.OnWindowCountChanged(e);
        }

        private void SetTheme(ExtendedForm parentForm)
        {
            Icon = parentForm.Icon;
            GridTheme = parentForm.GridTheme;
        }

        #endregion Methods
    }
}