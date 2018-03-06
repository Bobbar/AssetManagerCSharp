using AssetManager.UserInterface.Forms.Sibi;
using System;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    public class WindowList : IDisposable
    {
        #region "Fields"

        private Timer RefreshTimer;
        private ToolStripDropDownButton DropDownControl = new ToolStripDropDownButton();
        private int intFormCount;
        private ExtendedForm MyParentForm;

        #endregion "Fields"

        #region "Constructors"

        public WindowList(ExtendedForm parentForm)
        {
            MyParentForm = parentForm;
        }

        #endregion "Constructors"

        #region "Methods"

        public void InsertWindowList(OneClickToolStrip targetToolStrip)
        {
            InitializeDropDownButton(targetToolStrip);
            InitializeTimer();
            RefreshWindowList();
        }

        private void AddParentMenu()
        {
            if (MyParentForm.ParentForm != null)
            {
                ToolStripMenuItem ParentDropDown = NewMenuItem(MyParentForm.ParentForm);
                ParentDropDown.Text = "[Parent] " + ParentDropDown.Text;
                ParentDropDown.ToolTipText = "Parent Form";
                DropDownControl.DropDownItems.Insert(0, ParentDropDown);
                DropDownControl.DropDownItems.Add(new ToolStripSeparator());
            }
        }

        /// <summary>
        /// Recursively build ToolStripItemCollections of Forms and their Children and add them to the ToolStrip. Making sure to add SibiMain to the top of the list.
        /// </summary>
        /// <param name="parentForm">Form to add to ToolStrip.</param>
        /// <param name="targetMenuItem">Item to add the Form item to.</param>
        private void BuildWindowList(ExtendedForm parentForm, ToolStripItemCollection targetMenuItem)
        {
            foreach (ExtendedForm frm in Helpers.ChildFormControl.GetChildren(parentForm))
            {
                if (HasChildren(frm))
                {
                    var NewDropDown = NewMenuItem(frm);
                    if (frm is SibiMainForm)
                    {
                        targetMenuItem.Insert(0, NewDropDown);
                    }
                    else
                    {
                        targetMenuItem.Add(NewDropDown);
                    }
                    BuildWindowList(frm, NewDropDown.DropDownItems);
                }
                else
                {
                    if (frm is SibiMainForm)
                    {
                        targetMenuItem.Insert(0, NewMenuItem(frm));
                    }
                    else
                    {
                        targetMenuItem.Add(NewMenuItem(frm));
                    }
                }
            }
        }

        private string CountText(int count)
        {
            string MainText = "Select Window";
            if (count > 0)
            {
                return MainText + " (" + count + ")";
            }
            else
            {
                return MainText;
            }
        }

        private int FormCount(ExtendedForm parentForm)
        {
            int i = 0;
            foreach (ExtendedForm frm in Helpers.ChildFormControl.GetChildren(parentForm))
            {
                if (!frm.Modal & !object.ReferenceEquals(frm, parentForm))
                {
                    i += FormCount(frm) + 1;
                }
            }
            return i;
        }

        private bool HasChildren(ExtendedForm parentForm)
        {
            var Children = Helpers.ChildFormControl.GetChildren(parentForm);
            if (Children.Count == 0)
            {
                return false;
            }
            else
            {
                foreach (ExtendedForm frm in Children)
                {
                    if (!frm.IsDisposed)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void InitializeDropDownButton(OneClickToolStrip targetToolStrip)
        {
            DropDownControl.Visible = false;
            DropDownControl.Font = targetToolStrip.Font;
            DropDownControl.Text = "Select Window";
            DropDownControl.Name = "WindowList";
            DropDownControl.Image = Properties.Resources.CascadeIcon;
            AddParentMenu();
            targetToolStrip.Items.Insert(targetToolStrip.Items.Count, DropDownControl);
        }

        private void InitializeTimer()
        {
            RefreshTimer = new Timer();
            RefreshTimer.Interval = 500;
            RefreshTimer.Enabled = true;
            RefreshTimer.Tick += RefreshTimer_Tick;
        }

        private OnlineStatusMenuItem NewMenuItem(ExtendedForm frm)
        {
            var newitem = new OnlineStatusMenuItem();
            newitem.Name = frm.Name;
            newitem.Font = DropDownControl.Font;
            newitem.Text = frm.Text;
            newitem.Image = frm.Icon.ToBitmap();
            newitem.TargetForm = frm;

            if (frm is IOnlineStatus)
            {
                newitem.SetOnlineStatusInterface((IOnlineStatus)frm);
            }

            newitem.ToolTipText = "Right-Click to close.";
            newitem.MouseDown += WindowClick;
            return newitem;
        }

        private void RefreshTimer_Tick(object sender, EventArgs e)
        {
            RefreshWindowList();
        }

        private void RefreshWindowList()
        {
            var NumOfForms = FormCount(MyParentForm);
            if (MyParentForm.ParentForm == null & NumOfForms < 1)
            {
                DropDownControl.Visible = false;
            }
            else
            {
                DropDownControl.Visible = true;
            }
            if (NumOfForms != intFormCount)
            {
                if (!DropDownControl.DropDown.Focused)
                {
                    DropDownControl.DropDownItems.Clear();
                    AddParentMenu();
                    BuildWindowList(MyParentForm, DropDownControl.DropDownItems);
                    intFormCount = NumOfForms;
                    DropDownControl.Text = CountText(intFormCount);
                }
            }
        }

        private void WindowClick(object sender, MouseEventArgs e)
        {
            OnlineStatusMenuItem item = (OnlineStatusMenuItem)sender;
            var frm = item.TargetForm;
            if (e.Button == MouseButtons.Right)
            {
                if ((!object.ReferenceEquals(frm, MyParentForm.ParentForm)))
                {
                    frm.Close();
                    if (frm.Disposing | frm.IsDisposed)
                    {
                        DisposeDropDownItem(item);
                    }
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (!frm.IsDisposed)
                {
                    Helpers.ChildFormControl.ActivateForm(frm);
                }
                else
                {
                    DisposeDropDownItem(item);
                }
            }
        }

        private void DisposeDropDownItem(ToolStripItem item)
        {
            if (DropDownControl.DropDownItems.Count < 1)
            {
                DropDownControl.Visible = false;
                DropDownControl.DropDownItems.Clear();
                item.Dispose();
            }
            else
            {
                item.Visible = false;
                item.Dispose();
                DropDownControl.DropDownItems.Remove(item);
                intFormCount = FormCount(MyParentForm);
                DropDownControl.Text = CountText(intFormCount);
            }
        }

        #endregion "Methods"

        #region "IDisposable Support"

        // To detect redundant calls
        private bool disposedValue;

        // This code added by Visual Basic to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
            Dispose(true);
            // TODO: uncomment the following line if Finalize() is overridden above.
            // GC.SuppressFinalize(Me)
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    RefreshTimer.Dispose();
                    DropDownControl.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override Finalize() below.
                // TODO: set large fields to null.
            }
            disposedValue = true;
        }

        // TODO: override Finalize() only if Dispose(disposing As Boolean) above has code to free unmanaged resources.
        //Protected Overrides Sub Finalize()
        //    ' Do not change this code.  Put cleanup code in Dispose(disposing As Boolean) above.
        //    Dispose(False)
        //    MyBase.Finalize()
        //End Sub

        #endregion "IDisposable Support"
    }
}