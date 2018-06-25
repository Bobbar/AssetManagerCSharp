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
        private bool hasParentMenu = false;

        #endregion "Fields"

        #region "Constructors"

        public WindowList(ExtendedForm parentForm)
        {
            windowListForm = parentForm;
            windowListForm.ChildAdded += WindowListForm_ChildAdded;
            windowListForm.ChildRemoved += WindowListForm_ChildRemoved;
            this.parentForm = parentForm;
        }

        #endregion "Constructors"

        #region "Methods"

        public void InsertWindowList(OneClickToolStrip targetToolStrip)
        {
            InitializeDropDownButton(targetToolStrip);
            SetVisibility();
        }

        private void AddParentMenu()
        {
            if (parentForm.ParentForm != null)
            {
                var parentDropDown = NewMenuItem(parentForm.ParentForm);
                parentDropDown.Text = "[Parent] " + parentDropDown.Text;
                parentDropDown.ToolTipText = "Parent Form";
                dropDownControl.DropDownItems.Insert(0, parentDropDown);
                dropDownControl.DropDownItems.Add(new ToolStripSeparator());
                hasParentMenu = true;
            }
        }

        /// <summary>
        /// Adds a menu item for the specified child to the specified parent menu item.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        /// <param name="targetMenu"></param>
        private void AddChildMenu(ExtendedForm parent, ExtendedForm child, ToolStripItemCollection targetMenu)
        {
            // If this window list parent is the direct parent of the child,
            // add the child menu item to the top/root menu.
            if (parent == this.parentForm)
            {
                // Always add the Sibi form to the top of the menu,
                // all others can go below.
                if (child is SibiMainForm)
                {
                    targetMenu.Insert(0, NewMenuItem(child));
                }
                else
                {
                    targetMenu.Add(NewMenuItem(child));
                }
            }
            else
            {
                // If this window list parent is not the direct parent of the child,
                // iterate and recurse through all the menu items until we find the
                // item which targets the parent, and add to or start a new sub menu.
                foreach (var item in targetMenu)
                {
                    // Ignore seperators.
                    if (item is OnlineStatusMenuItem)
                    {
                        var statusItem = (OnlineStatusMenuItem)item;

                        // Add the child menu item to the sub menu of the matching parent item.
                        if (statusItem.TargetForm == parent)
                        {
                            statusItem.DropDownItems.Add(NewMenuItem(child));
                        }

                        // Recurse with sub menu items.
                        if (statusItem.HasDropDownItems)
                            AddChildMenu(parent, child, statusItem.DropDownItems);
                    }
                }
            }
        }

        /// <summary>
        /// Removes the menu item for the specified child form.
        /// </summary>
        /// <param name="child"></param>
        /// <param name="targetMenu"></param>
        private void RemoveChildMenu(ExtendedForm child, ToolStripItemCollection targetMenu)
        {
            // Must use a regular 'for' block because we are going to modify the collection.
            // Iterate and recurse all menu items and remove any items that match the specified child form.
            for (int i = 0; i < targetMenu.Count; i++)
            {
                // Ignore seperators.
                if (targetMenu[i] is OnlineStatusMenuItem)
                {
                    OnlineStatusMenuItem item = (OnlineStatusMenuItem)targetMenu[i];
                    if (item.TargetForm == child)
                    {
                        DisposeDropDownItem(item);
                    }

                    if (item.HasDropDownItems)
                        RemoveChildMenu(child, item.DropDownItems);
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
            // PROBLEM:
            // Because we are using the MouseUp event from the menu items,
            // and also removing/disposing those items as a result of that event,
            // the top level drop down button UI tends to hang and cause strange 
            // behaviour. The problem is particularly apparent after the last 
            // menu item has been removed. Small rectangles left in the cornor 
            // of the screen, and forms spontaneously popping infront of the 
            // active form are two of the most annoying problems.
            // 
            // REASON (maybe):
            // After much digging and testing, I've managed to narrow it down
            // to stuck/corrupt mouse event messages. These screwed up messages
            // tend to fire on other controls and cause unintended UI changes.
            // My best guess is that the item is disposed before the WM_LBUTTONUP
            // is actually received and processed by the menu control.
            //
            // I tried to find a decent logical way around it, found the following
            // to work the best.
            //
            // WORKAROUND:
            // We selectively call the PerfomClick method on menu items just
            // before they are removed and disposed. This seems to clear out
            // any remaining mouse button messages and allows the items to go
            // away cleanly and quietly. The only reason we don't call it on
            // every disposal is because it causes the top level menu to close,
            // and this is not desirable if users want to manage multiple items
            // without having to reopen the menu after every click.

            // If the item is not visible, always perform a click.
            if (!item.Visible)
            {
                item.PerformClick();
            }
            else
            {
                // If the item is the last item in a menu containing a fixed parent item, perform a click.
                // (Parent item + Separator + last item = 3)
                if (dropDownControl.DropDownItems.Count == 3 && hasParentMenu)
                {
                    item.PerformClick();
                }
                // If the item is the last item, regardless of a fixed parent item, perform a click.
                else if (dropDownControl.DropDownItems.Count == 1)
                {
                    item.PerformClick();
                }
            }

            // Do the usual clean up and disposal stuff.
            if (dropDownControl.DropDownItems.Count < 1)
            {
                dropDownControl.DropDownItems.Clear();
            }
            else
            {
                item.Visible = false;
                dropDownControl.DropDownItems.Remove(item);
            }

            item.MouseUp -= ItemClicked;
            item.Dispose();
        }

        private void DisposeAllDropDownItems()
        {
            for (int i = 0; i < dropDownControl.DropDownItems.Count; i++)
            {
                dropDownControl.DropDownItems[i].MouseUp -= ItemClicked;
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
                }
            }
            else if (e.Button == MouseButtons.Left)
            {
                if (!frm.IsDisposed)
                {
                    item.PerformClick();
                    frm.RestoreWindow();
                }
                else
                {
                    DisposeDropDownItem(item);
                }
            }
            else if (e.Button == MouseButtons.Middle)
            {
                if (!frm.IsDisposed)
                {
                    frm.WindowState = FormWindowState.Minimized;
                    parentForm.Focus();
                }
                else
                {
                    DisposeDropDownItem(item);
                }
            }
            SetVisibility();
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
            newitem.ToolTipText = "Right-Click to close. Middle-Click to minimize.";
            newitem.MouseUp += ItemClicked;
            return newitem;
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

        private void WindowListForm_ChildAdded(object sender, ExtendedForm e)
        {
            AddChildMenu((ExtendedForm)sender, e, dropDownControl.DropDownItems);
            dropDownControl.Text = CountText(windowListForm.ChildFormCount());
            SetVisibility();
        }

        private void WindowListForm_ChildRemoved(object sender, ExtendedForm e)
        {
            RemoveChildMenu(e, dropDownControl.DropDownItems);
            dropDownControl.Text = CountText(windowListForm.ChildFormCount());
            SetVisibility();
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
                    windowListForm.ChildAdded -= WindowListForm_ChildAdded;
                    windowListForm.ChildRemoved -= WindowListForm_ChildRemoved;
                    dropDownControl.DropDownClosed -= DropDownControl_DropDownClosed;
                    dropDownControl.DropDownOpened -= DropDownControl_DropDownOpened;
                }
            }
            disposedValue = true;
        }

        #endregion "IDisposable Support"
    }
}