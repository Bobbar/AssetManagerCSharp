using System;
using System.Collections.Generic;
using System.Windows.Forms;
using AssetManager.Data.Functions;
using AssetManager.UserInterface.Forms.AdminTools;
using System.Diagnostics.CodeAnalysis;

namespace AssetManager.UserInterface.CustomControls
{
    public class MunisToolBar : IDisposable
    {
        #region "Fields"

        private ToolStripSeparator blankSeparator = new ToolStripSeparator();
        private ToolStripDropDownButton munisDropDown = new ToolStripDropDownButton();

        #endregion "Fields"

        #region "Constructors"

        public MunisToolBar(ExtendedForm parentForm)
        {
            InitDropDown();
            InitToolItems(parentForm);
        }

        #endregion "Constructors"

        #region "Methods"

        /// <summary>
        /// Inserts the MunisToolBar into the specified toolstrip.
        /// </summary>
        /// <param name="targetStrip"></param>
        /// <param name="locationIndex"></param>
        public void InsertMunisDropDown(OneClickToolStrip targetStrip, int locationIndex = -1)
        {
            if (locationIndex >= 0)
            {
                targetStrip.Items.Insert(locationIndex, munisDropDown);
                AddSeperators(ref targetStrip, locationIndex);
            }
            else
            {
                targetStrip.Items.Add(munisDropDown);
                AddSeperators(ref targetStrip, targetStrip.Items.Count - 1);
            }
        }

        private void AddSeperators(ref OneClickToolStrip targetToolStrip, int locationIndex)
        {
            if (targetToolStrip.Items.Count - 1 >= locationIndex + 1)
            {
                if (!object.ReferenceEquals(targetToolStrip.Items[locationIndex + 1].GetType(), typeof(ToolStripSeparator)))
                {
                    targetToolStrip.Items.Insert(locationIndex + 1, blankSeparator);
                }
                if (!object.ReferenceEquals(targetToolStrip.Items[locationIndex - 1].GetType(), typeof(ToolStripSeparator)))
                {
                    targetToolStrip.Items.Insert(locationIndex, blankSeparator);
                }
            }
            else
            {
                if (!object.ReferenceEquals(targetToolStrip.Items[locationIndex].GetType(), typeof(ToolStripSeparator)))
                {
                    targetToolStrip.Items.Add(blankSeparator);
                }
                if (!object.ReferenceEquals(targetToolStrip.Items[locationIndex - 1].GetType(), typeof(ToolStripSeparator)))
                {
                    targetToolStrip.Items.Insert(locationIndex, blankSeparator);
                }
            }
        }

        private void AddToolItem(ToolStripMenuItem toolItem)
        {
            munisDropDown.DropDownItems.Add(toolItem);
            toolItem.Click += ToolItemClicked;
        }

        private void InitDropDown()
        {
            munisDropDown.Image = Properties.Resources.SearchIcon;
            munisDropDown.Name = "MunisTools";
            munisDropDown.Size = new System.Drawing.Size(87, 29);
            munisDropDown.Text = "MUNIS Tools";
            munisDropDown.AutoSize = true;
        }

        private void InitToolItems(ExtendedForm parentForm)
        {
            List<ToolStripMenuItem> ToolItemList = new List<ToolStripMenuItem>();
            ToolItemList.Add(NewToolItem("tsmUserOrgObLookup", "User Lookup", () => MunisFunctions.NameSearch(parentForm)));
            ToolItemList.Add(NewToolItem("tsmOrgObLookup", "Org/Obj Lookup", () => MunisFunctions.OrgObjSearch(parentForm)));
            ToolItemList.Add(NewToolItem("tsmPOLookUp", "PO Lookup", () => MunisFunctions.POSearch(parentForm)));
            ToolItemList.Add(NewToolItem("tsmReqNumLookUp", "Requisition # Lookup", () => MunisFunctions.ReqSearch(parentForm)));
            ToolItemList.Add(NewToolItem("tsmDeviceLookUp", "Device Lookup", () => MunisFunctions.AssetSearch(parentForm)));
            ToolItemList.Add(NewToolItem("HierarchySearch", "View Supervisor Hierarchy", () => new Hierarchy(parentForm)));
            foreach (ToolStripMenuItem item in ToolItemList)
            {
                AddToolItem(item);
            }
        }

        private ToolStripMenuItem NewToolItem(string name, string text, Action clickMethod)
        {
            ToolStripMenuItem newItem = new ToolStripMenuItem();
            newItem.Name = name;
            newItem.Text = text;
            newItem.Tag = clickMethod;
            return newItem;
        }

        private void ToolItemClicked(object sender, EventArgs e)
        {
            ToolStripMenuItem ClickedItem = (ToolStripMenuItem)sender;
            Action ClickAction = (Action)ClickedItem.Tag;
            ClickAction();
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
                    blankSeparator.Dispose();
                    munisDropDown.Dispose();
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