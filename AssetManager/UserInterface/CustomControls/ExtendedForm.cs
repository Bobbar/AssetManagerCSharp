using AssetManager.Data.Classes;
using AssetManager.Helpers;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

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
        private ExtendedForm parentForm;
        private bool doubleBuffering = true;

        #endregion Fields

        #region Constructors

        public ExtendedForm(ExtendedForm parentForm, string formGuid, bool inheritTheme)
        {
            this.inheritTheme = inheritTheme;
            FormGuid = formGuid;
            ParentForm = parentForm;
            SubscribeEvents();
            SetDoubleBuffering();
        }

        public ExtendedForm() : this(null, string.Empty, true)
        {
        }

        public ExtendedForm(ExtendedForm parentForm) : this(parentForm, string.Empty, true)
        {
        }

        public ExtendedForm(ExtendedForm parentForm, MappableObject currentObject) : this(parentForm, currentObject.Guid, true)
        {
        }

        public ExtendedForm(ExtendedForm parentForm, string formGuid) : this(parentForm, formGuid, true)
        {
        }

        public ExtendedForm(ExtendedForm parentForm, bool inheritTheme) : this(parentForm, string.Empty, inheritTheme)
        {
        }

        #endregion Constructors

        #region Events

        public event EventHandler<EventArgs> ChildCountChanged;

        public event EventHandler<EventArgs> RefreshWindowList;

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
        /// Replaces the stock ParentForm property with a read/writable one. And also sets the icon and <seealso cref="GridTheme"/> from the parent form.
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
                if (inheritTheme && parentForm != null)
                {
                    SetTheme(this.parentForm);
                }
            }
        }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Disables double buffering for any future control instantiations.
        /// </summary>
        public void DisableDoubleBuffering()
        {
            doubleBuffering = false;
        }

        /// <summary>
        /// Disables double buffering if the current environment is in the designer or a terminal session.
        /// </summary>
        private void SetDoubleBuffering()
        {
            var designMode = (System.Diagnostics.Process.GetCurrentProcess().ProcessName == "devenv");
            var terminalSession = System.Windows.Forms.SystemInformation.TerminalServerSession;

            if (designMode || terminalSession) doubleBuffering = false;
        }

        private void SubscribeEvents()
        {
            this.Load += ExtendedForm_Load;
            this.Disposed += ExtendedForm_Disposed;
            this.FormClosing += ExtendedForm_FormClosing;
        }

        private void UnSubscribeEvents()
        {
            this.Load -= ExtendedForm_Load;
            this.Disposed -= ExtendedForm_Disposed;
            this.FormClosing -= ExtendedForm_FormClosing;
        }

        /// <summary>
        /// Override and return true if the form is in a state that is ok to close. (ie. Not in the middle of editing.)
        /// </summary>
        /// <returns></returns>
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

        protected override CreateParams CreateParams
        {
            // Enables double-buffering.
            get
            {
                CreateParams cp = base.CreateParams;

                if (doubleBuffering)
                {
                    cp.ExStyle |= 0x02000000;  // Turn on WS_EX_COMPOSITED
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

        public void ForceWindowListRefresh()
        {
            OnRefreshWindowList(new EventArgs());
        }

        private void ExtendedForm_Disposed(object sender, System.EventArgs e)
        {
            if (!IsDisposed)
            {
                UnSubscribeEvents();
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
        }

        private void OnWindowCountChanged(EventArgs e)
        {
            ChildCountChanged?.Invoke(this, e);
            parentForm?.OnWindowCountChanged(e);
        }

        private void OnRefreshWindowList(EventArgs e)
        {
            RefreshWindowList?.Invoke(this, e);
            parentForm?.OnRefreshWindowList(e);
        }

        private void SetTheme(ExtendedForm parent)
        {
            Icon = parent.Icon;
            GridTheme = parent.GridTheme;
        }

        #region Child Form Methods

        public void AddChild(ExtendedForm child)
        {
            childForms.Add(child);
            child.Disposed += Child_Disposed;
            OnWindowCountChanged(new EventArgs());
        }

        public void RemoveChild(ExtendedForm child)
        {
            childForms.Remove(child);
            child.Disposed -= Child_Disposed;
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

        public bool OkToCloseChildren()
        {
            bool closeAllowed = true;
            if (childForms.Count > 0)
            {
                foreach (var child in childForms)
                {
                    if (!child.OkToClose() || !child.OkToCloseChildren())
                    {
                        child.WindowState = FormWindowState.Normal;
                        child.Activate();
                        closeAllowed = false;
                    }
                }
            }
            return closeAllowed;
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

        private void Child_Disposed(object sender, EventArgs e)
        {
            RemoveChild((ExtendedForm)sender);
        }

        #endregion Child Form Methods

        #endregion Methods
    }
}