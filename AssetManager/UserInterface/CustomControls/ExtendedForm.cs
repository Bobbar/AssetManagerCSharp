using AssetManager.Data.Classes;
using AssetManager.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
        private bool minimizeChildren = false;
        private bool restoreChildren = false;
        private FormWindowState previousWindowState;
        private bool startedHidden = false;

        #endregion Fields

        #region Constructors

        public ExtendedForm(ExtendedForm parentForm, string formGuid)
        {
            FormGuid = formGuid;
            ParentForm = parentForm;
            SubscribeEvents();
            SetDoubleBuffering();
        }

        public ExtendedForm() : this(null, string.Empty)
        {
        }

        public ExtendedForm(ExtendedForm parentForm) : this(parentForm, string.Empty)
        {
        }

        public ExtendedForm(ExtendedForm parentForm, MappableObject currentObject) : this(parentForm, currentObject.Guid)
        {
        }

        public ExtendedForm(ExtendedForm parentForm, MappableObject currentObject, bool startHidden = false) : this(parentForm, currentObject.Guid)
        {
            // If the form is starting without being shown, call for the handle to force its creation.
            if (startHidden)
            {
                var dummy = this.Handle;
                startedHidden = true;
            }
        }

        #endregion Constructors

        #region Events

        public event EventHandler<ExtendedForm> ChildAdded;
        public event EventHandler<ExtendedForm> ChildRemoved;

        private void OnChildAdded(ExtendedForm parent, ExtendedForm child)
        {
            ChildAdded?.Invoke(parent, child);
            parentForm?.OnChildAdded(parent, child);
        }

        private void OnChildRemoved(ExtendedForm parent, ExtendedForm child)
        {
            ChildRemoved?.Invoke(parent, child);
            parentForm?.OnChildRemoved(parent, child);
        }

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
        /// Gets or sets the value indicating whether child forms will be minimized with parent.
        /// </summary>
        [Browsable(true)]
        [Category("Extended Features")]
        [Description("Gets or sets the value indicating whether child forms will be minimized with parent.")]
        public bool MinimizeChildren
        {
            get
            {
                return minimizeChildren;
            }

            set
            {
                minimizeChildren = value;
            }
        }

        /// <summary>
        /// Gets or sets the value indicating whether child forms will be restored with parent.
        /// </summary>
        [Browsable(true)]
        [Category("Extended Features")]
        [Description("Gets or sets the value indicating whether child forms will be restored with parent.")]
        public bool RestoreChildren
        {
            get
            {
                return restoreChildren;
            }

            set
            {
                restoreChildren = value;
            }
        }

        [Browsable(true)]
        [Category("Extended Features")]
        [Description("Gets or sets the value indicating whether child forms will inherit the theme of their parent form.")]
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
        /// Unique identifying string used to locate specific instances of this form.
        /// </summary>
        /// <returns></returns>
        public string FormGuid { get; set; }

        /// <summary>
        /// Gets or sets the Grid Theme for the DataGridView controls within the form.
        /// </summary>
        /// <returns></returns>
        public GridTheme GridTheme { get; set; }

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

        public virtual void Waiting(string message = "")
        {
            SetWaitCursor(true);
        }

        public virtual void DoneWaiting()
        {
            SetWaitCursor(false);
        }

        /// <summary>
        /// Override and add code to refresh data from the database.
        /// </summary>
        public virtual void RefreshData()
        {
            this.Refresh();
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

        /// <summary>
        /// Disables double buffering for any future control instantiations.
        /// </summary>
        public void DisableDoubleBuffering()
        {
            doubleBuffering = false;
        }

        /// <summary>
        /// Sets window state to normal and activates the form.
        /// </summary>
        public void RestoreWindow()
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
            this.Activate();
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
            this.Resize += ExtendedForm_Resize;
            this.ResizeBegin += ExtendedForm_ResizeBegin;
        }

        private void UnSubscribeEvents()
        {
            this.Load -= ExtendedForm_Load;
            this.Disposed -= ExtendedForm_Disposed;
            this.FormClosing -= ExtendedForm_FormClosing;
            this.Resize -= ExtendedForm_Resize;
            this.ResizeBegin -= ExtendedForm_ResizeBegin;
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
        /// Centers this instance to its parent form if that location is does not clip the active screen. Otherwise, will center to screen.
        /// </summary>
        private void CenterToParentForm()
        {
            if (this.StartPosition == FormStartPosition.CenterParent & parentForm != null)
            {
                var newX = parentForm.Location.X + ((parentForm.Width / 2) - this.Width / 2);
                var newY = parentForm.Location.Y + ((parentForm.Height / 2) - this.Height / 2);
                var newRect = new Rectangle(newX, newY, this.Width, this.Height);
                // The work area containing the parent form.
                var workArea = Screen.GetWorkingArea(parentForm);

                // Make sure the new location is not off screen.
                if (workArea.Contains(newRect))
                {
                    // Center to parent form.
                    this.Location = new Point(newX, newY);
                }
                else
                {
                    // Center to screen.
                    this.CenterToScreen();
                }
            }
            else if (this.StartPosition == FormStartPosition.CenterScreen)
            {
                this.CenterToScreen();
            }
        }

        private void SetWaitCursor(bool waiting)
        {
            if (this.InvokeRequired)
            {
                var del = new Action(() => SetWaitCursor(waiting));
                this.BeginInvoke(del);
            }
            else
            {
                this.UseWaitCursor = waiting;

                if (waiting)
                {
                    Cursor.Current = Cursors.WaitCursor;
                }
                else
                {
                    Cursor.Current = Cursors.Default;
                    ResetDataGridCursors(this);
                }
            }
        }

        /// <summary>
        /// Recursively finds <see cref="DataGridView"/> controls and sets their cursors back to default.
        /// </summary>
        /// <param name="parentControl"></param>
        /// <remarks>
        /// This is done because sometimes the cursor gets stuck on waiting, and we need to directly set it back to default.
        /// </remarks>
        private void ResetDataGridCursors(Control parentControl)
        {
            foreach (Control control in parentControl.Controls)
            {
                if (control is DataGridView)
                {
                    control.Cursor = Cursors.Default;
                }

                if (control.HasChildren) ResetDataGridCursors(control);
            }
        }

        private void SetTheme(ExtendedForm parent)
        {
            Icon = parent.Icon;
            GridTheme = parent.GridTheme;
        }

        private void ExtendedForm_Disposed(object sender, System.EventArgs e)
        {
            if (!IsDisposed)
            {
                DoneWaiting();
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
            // If not started hidden, add this instance to the parent form if present.
            if (!startedHidden)
                parentForm?.AddChild(this);

            CenterToParentForm();
        }

        private void ExtendedForm_Resize(object sender, EventArgs e)
        {
            if (minimizeChildren && this.WindowState == FormWindowState.Minimized)
            {
                SetChildrenWindowState(FormWindowState.Minimized);
                previousWindowState = this.WindowState;
            }
            else if (restoreChildren && this.WindowState != previousWindowState && this.WindowState == FormWindowState.Normal)
            {
                if (previousWindowState != FormWindowState.Maximized)
                {
                    SetChildrenWindowState(FormWindowState.Normal);
                }
            }
        }

        private void ExtendedForm_ResizeBegin(object sender, EventArgs e)
        {
            previousWindowState = this.WindowState;
        }

        #region Child Form Methods

        private void SetChildrenWindowState(FormWindowState state)
        {
            foreach (var child in childForms)
            {
                child.WindowState = state;
            }
        }

        /// <summary>
        /// Indicates to the base class that the hidden form is ready and component dependent functions can proceed.
        /// </summary>
        public void HiddenFormReady()
        {
            if (startedHidden)
                parentForm?.AddChild(this);
        }

        public void AddChild(ExtendedForm child)
        {
            if (!childForms.Contains(child))
            {
                childForms.Add(child);
                child.Disposed += Child_Disposed;
                OnChildAdded(this, child);
            }
        }

        public void RemoveChild(ExtendedForm child)
        {
            childForms.Remove(child);
            child.Disposed -= Child_Disposed;
            OnChildRemoved(this, child);
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
                        child.RestoreWindow();
                        closeAllowed = false;
                    }
                }
            }
            return closeAllowed;
        }

        /// <summary>
        /// Returns a list of all child forms including the childs of all children.
        /// </summary>
        /// <returns></returns>
        public List<ExtendedForm> GetChildForms()
        {
            return GetChildFormsList(this);
        }

        private List<ExtendedForm> GetChildFormsList(ExtendedForm parent)
        {
            var childList = new List<ExtendedForm>();

            childList.AddRange(parent.ChildForms);

            foreach (var child in parent.childForms)
            {
                if (child.ChildForms.Count > 0)
                    childList.AddRange(GetChildFormsList(child));
            }

            return childList;
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