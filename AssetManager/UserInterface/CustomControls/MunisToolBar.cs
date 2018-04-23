using AssetManager.Data.Functions;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Forms;

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
                if (!(targetToolStrip.Items[locationIndex + 1] is ToolStripSeparator))
                {
                    targetToolStrip.Items.Insert(locationIndex + 1, blankSeparator);
                }
                if (!(targetToolStrip.Items[locationIndex - 1] is ToolStripSeparator))
                {
                    targetToolStrip.Items.Insert(locationIndex, blankSeparator);
                }
            }
            else
            {
                if (!(targetToolStrip.Items[locationIndex] is ToolStripSeparator))
                {
                    targetToolStrip.Items.Add(blankSeparator);
                }
                if (!(targetToolStrip.Items[locationIndex - 1] is ToolStripSeparator))
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

        [SuppressMessage("Microsoft.Design", "CA1806")]
        private void InitToolItems(ExtendedForm parentForm)
        {
            var toolItemList = new List<ToolStripMenuItem>();
            toolItemList.Add(NewToolItem("tsmUserOrgObLookup", "User Lookup", () => MunisFunctions.NameSearch(parentForm)));
            toolItemList.Add(NewToolItem("tsmOrgObLookup", "Org/Obj Lookup", () => MunisFunctions.OrgObjSearch(parentForm)));
            toolItemList.Add(NewToolItem("tsmPOLookUp", "PO Lookup", () => MunisFunctions.POSearch(parentForm)));
            toolItemList.Add(NewToolItem("tsmReqNumLookUp", "Requisition # Lookup", () => MunisFunctions.ReqSearch(parentForm)));
            toolItemList.Add(NewToolItem("tsmDeviceLookUp", "Device Lookup", () => MunisFunctions.AssetSearch(parentForm)));

            foreach (var item in toolItemList)
            {
                AddToolItem(item);
            }
        }

        private ToolStripMenuItem NewToolItem(string name, string text, Action clickMethod)
        {
            var newItem = new ToolStripMenuItem();
            newItem.Name = name;
            newItem.Text = text;
            newItem.Tag = clickMethod;
            return newItem;
        }

        private void ToolItemClicked(object sender, EventArgs e)
        {
            var clickedItem = (ToolStripMenuItem)sender;
            var clickAction = (Action)clickedItem.Tag;
            clickAction();
        }

        #endregion "Methods"

        #region "IDisposable Support"

        // To detect redundant calls
        private bool disposedValue;

        public void Dispose()
        {
            Dispose(true);
        }

        // IDisposable
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    blankSeparator.Dispose();
                    munisDropDown.Dispose();
                }
            }
            disposedValue = true;
        }

        #endregion "IDisposable Support"
    }
}