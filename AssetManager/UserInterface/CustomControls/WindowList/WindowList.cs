using AssetManager.UserInterface.Forms.Sibi;
using System;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    public class WindowList : IDisposable
    {
        #region "Fields"

        private ToolStripDropDownButton dropDownControl = new ToolStripDropDownButton();
        private bool dropDownOpen = false;
        private ExtendedForm parentForm;
        private IWindowList windowListForm;
        #endregion "Fields"

        #region "Constructors"

        public WindowList(ExtendedForm parentForm)
        {
            windowListForm = parentForm;
            windowListForm.ChildCountChanged += WindowListForm_WindowCountChanged;
            windowListForm.RefreshWindowList += WindowListForm_RefreshWindowList;
            this.parentForm = parentForm;
        }

        #endregion "Constructors"

        #region "Methods"

        public void InsertWindowList(OneClickToolStrip targetToolStrip)
        {
            InitializeDropDownButton(targetToolStrip);
            RefreshWindowList();
        }

        private void AddParentMenu()
        {
            if (parentForm.ParentForm != null)
            {
                ToolStripMenuItem ParentDropDown = NewMenuItem(parentForm.ParentForm);
                ParentDropDown.Text = "[Parent] " + ParentDropDown.Text;
                ParentDropDown.ToolTipText = "Parent Form";
                dropDownControl.DropDownItems.Insert(0, ParentDropDown);
                dropDownControl.DropDownItems.Add(new ToolStripSeparator());
            }
        }

        /// <summary>
        /// Recursively build ToolStripItemCollections of Forms and their Children and add them to the ToolStrip. Making sure to add SibiMain to the top of the list.
        /// </summary>
        /// <param name="target">Form to add to ToolStrip.</param>
        /// <param name="targetMenuItem">Item to add the Form item to.</param>
        private void BuildWindowList(IWindowList target, ToolStripItemCollection targetMenuItem)
        {
            foreach (var frm in target.ChildForms)
            {
                if (frm.ChildForms.Count > 0)
                {
                    var newDropDown = NewMenuItem(frm);
                    if (frm is SibiMainForm)
                    {
                        targetMenuItem.Insert(0, newDropDown);
                    }
                    else
                    {
                        targetMenuItem.Add(newDropDown);
                    }
                    BuildWindowList(frm, newDropDown.DropDownItems);
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

        private void DisposeDropDownItem(ToolStripItem item)
        {
            if (dropDownControl.DropDownItems.Count < 1)
            {
                dropDownControl.DropDownItems.Clear();
            }
            else
            {
                item.Visible = false;
                dropDownControl.DropDownItems.Remove(item);
            }

            item.MouseDown -= ItemClicked;
            item.Dispose();

            SetVisibility();
        }

        private void DisposeAllDropDownItems()
        {
            for (int i = 0; i < dropDownControl.DropDownItems.Count; i++)
            {
                dropDownControl.DropDownItems[i].MouseDown -= ItemClicked;
                dropDownControl.DropDownItems[i].Dispose();
            }
            dropDownControl.DropDownItems.Clear();
        }

        private void DropDownControl_DropDownClosed(object sender, EventArgs e)
        {
            dropDownOpen = false;
        }

        private void DropDownControl_DropDownOpened(object sender, EventArgs e)
        {
            dropDownOpen = true;
        }

        private void InitializeDropDownButton(OneClickToolStrip targetToolStrip)
        {
            dropDownControl.Visible = false;
            dropDownControl.Font = targetToolStrip.Font;
            dropDownControl.Text = "Select Window";
            dropDownControl.Name = "WindowList";
            dropDownControl.Image = Properties.Resources.CascadeIcon;
            dropDownControl.DropDownClosed += DropDownControl_DropDownClosed;
            dropDownControl.DropDownOpened += DropDownControl_DropDownOpened;
            AddParentMenu();
            targetToolStrip.Items.Insert(targetToolStrip.Items.Count, dropDownControl);
        }
        private void ItemClicked(object sender, MouseEventArgs e)
        {
            var item = (OnlineStatusMenuItem)sender;
            var frm = item.TargetForm;
            if (e.Button == MouseButtons.Right)
            {
                if ((frm != parentForm.ParentForm))
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
                    frm.WindowState = FormWindowState.Normal;
                    frm.Activate();
                }
                else
                {
                    DisposeDropDownItem(item);
                }
            }
        }

        private OnlineStatusMenuItem NewMenuItem(ExtendedForm frm)
        {
            var newitem = new OnlineStatusMenuItem();
            newitem.Name = frm.Name;
            newitem.Font = dropDownControl.Font;
            newitem.Text = frm.Text;
            newitem.Image = frm.Icon.ToBitmap();
            newitem.TargetForm = frm;

            if (frm is IOnlineStatus)
            {
                newitem.SetOnlineStatusInterface((IOnlineStatus)frm);
            }
            newitem.ToolTipText = "Right-Click to close.";
            newitem.MouseDown += ItemClicked;
            return newitem;
        }

        private void RefreshWindowList()
        {
            dropDownControl.Text = CountText(windowListForm.ChildFormCount());

            if (!dropDownOpen)
            {
                DisposeAllDropDownItems();
                AddParentMenu();
                BuildWindowList(parentForm, dropDownControl.DropDownItems);
            }

            SetVisibility();
        }

        private void SetVisibility()
        {
            if (dropDownControl.DropDownItems.Count > 0)
            {
                dropDownControl.Visible = true;
            }
            else
            {
                dropDownControl.Visible = false;
            }
        }

        private void WindowListForm_WindowCountChanged(object sender, EventArgs e)
        {
            RefreshWindowList();
        }

        private void WindowListForm_RefreshWindowList(object sender, EventArgs e)
        {
            RefreshWindowList();
        }

        #endregion "Methods"

        #region "IDisposable Support"

        private bool disposedValue;

        public void Dispose()
        {
            Dispose(true);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    DisposeAllDropDownItems();
                    dropDownControl.Dispose();
                    windowListForm.ChildCountChanged -= WindowListForm_WindowCountChanged;
                    windowListForm.RefreshWindowList -= WindowListForm_RefreshWindowList;
                    dropDownControl.DropDownClosed -= DropDownControl_DropDownClosed;
                    dropDownControl.DropDownOpened -= DropDownControl_DropDownOpened;
                }
            }
            disposedValue = true;
        }

        #endregion "IDisposable Support"
    }
}