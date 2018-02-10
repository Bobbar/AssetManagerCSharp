using System.Windows.Forms;
using AssetManager.Helpers;
using AssetManager.Data.Classes;
using AssetManager.Tools;
using System;

namespace AssetManager.UserInterface.CustomControls
{
    /// <summary>
    /// Custom form with project specific properties and methods.
    /// </summary>
    public class ExtendedForm : Form
    {
        private bool inheritTheme = true;

        private bool onlineStatus = false;

        public bool OnlineStatus
        {
            get
            {
                return onlineStatus;
            }
            set
            {
                this.onlineStatus = value;
            }
        }

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

        public event EventHandler OnlineStatusChanged;
        
        private ExtendedForm myParentForm;

        public string DefaultFormTitle { get; set; }

        /// <summary>
        /// Gets or sets the Grid Theme for the DataGridView controls within the form.
        /// </summary>
        /// <returns></returns>
        public GridTheme GridTheme { get; set; }

        /// <summary>
        /// Unique identifying string used to locate specific instances of this form.
        /// </summary>
        /// <returns></returns>
        public string FormUID { get; set; }

        /// <summary>
        /// Overloads the stock ParentForm property with a read/writable one. And also sets the icon and <seealso cref="GridTheme"/> from the parent form.
        /// </summary>
        /// <returns></returns>
        public new ExtendedForm ParentForm
        {
            get
            {
                return myParentForm;
            }

            set
            {
                this.myParentForm = value;
                if (inheritTheme)
                {
                    SetTheme(this.myParentForm);
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
            FormUID = currentObject.GUID;
            MemoryTweaks.SetWorkingSet();
        }

        public ExtendedForm(ExtendedForm parentForm, string formUID)
        {
            ParentForm = parentForm;
            FormUID = formUID;
            MemoryTweaks.SetWorkingSet();
        }

        public ExtendedForm(ExtendedForm parentForm, bool inheritTheme = true)
        {
            this.inheritTheme = inheritTheme;
            this.ParentForm = parentForm;
            MemoryTweaks.SetWorkingSet();
        }

        public virtual bool OKToClose()
        {
            return true;
        }

        /// <summary>
        /// Override and add code to refresh data from the database.
        /// </summary>
        public virtual void RefreshData()
        {
            this.Refresh();
        }

        public virtual void OnOnlineStatusChanged(OnlineStatusChangedEventArgs e)
        {
            OnlineStatusChanged(this, e);
            onlineStatus = e.OnlineStatus;
        }
        
        private void SetTheme(ExtendedForm parentForm)
        {
            Icon = parentForm.Icon;
            GridTheme = parentForm.GridTheme;
        }
        
        public class OnlineStatusChangedEventArgs : EventArgs
        {
            public bool OnlineStatus { get; set; }
            
            public OnlineStatusChangedEventArgs(bool isOnline)
            {
                OnlineStatus = isOnline;
            }
        }
    }
}