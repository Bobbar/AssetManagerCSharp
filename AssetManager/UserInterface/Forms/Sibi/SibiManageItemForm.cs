using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AssetManager.UserInterface.CustomControls;
using System.Data.Common;

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiManageItemForm : ExtendedForm
    {
        DBControlParser dbParser;
        private bool isNew = false;
        private SibiRequestMapObject request;
        private string currentItemUID;
        private DbTransaction currentTransaction = null;

        public SibiManageItemForm(ExtendedForm parentForm, SibiRequestMapObject request, DbTransaction transaction) : base(parentForm)
        {
            InitializeComponent();
            InitDBControls();
            dbParser = new DBControlParser(this);
            dbParser.EnableFieldValidation(errorProvider);
            this.Owner = parentForm;
            this.request = request;
            this.currentTransaction = transaction;
        }

        public void NewItem()
        {
            isNew = true;
        }


        private void InitDBControls()
        {

            UserTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.User, true);
            DescriptionTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.Description, true);
            QuantityTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.Qty, true);
            LocationComboBox.Tag = new DBControlInfo(SibiRequestItemsCols.Location, GlobalInstances.DeviceAttribute.Locations, true);
            StatusComboBox.Tag = new DBControlInfo(SibiRequestItemsCols.Status, GlobalInstances.SibiAttribute.ItemStatusType, true);
            ReplaceAssetTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.ReplaceAsset, false);
            ReplaceSerialTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.ReplaceSerial, false);
            NewAssetTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.NewAsset, false);
            NewSerialTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.NewSerial, false);
            OrgTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.OrgCode, false);
            ObjectTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.ObjectCode, false);
            ApproverTextBox.Tag = new DBControlInfo(SibiRequestItemsCols.ApproverId, false);
            ApprovalRequiredCheckBox.Tag = new DBControlInfo(SibiRequestItemsCols.RequiresApproval, false);

            AttribIndexFunctions.FillComboBox(GlobalInstances.DeviceAttribute.Locations, LocationComboBox.ComboBox);
            AttribIndexFunctions.FillComboBox(GlobalInstances.SibiAttribute.ItemStatusType, StatusComboBox.ComboBox);


        }

        private void AcceptChanges()
        {
            if (dbParser.ValidateFields(errorProvider))
            {

                string modifyStatus = "";

                modifyStatus = GetChangeStatus().ToString();
                Dictionary<string, object> addlValues;
                DataTable itemDataTable;


                if (isNew)
                {
                    var itemInsertQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " LIMIT 0";
                    addlValues = new Dictionary<string, object>
                    {
                        { SibiRequestItemsCols.RequestUID,request.GUID },
                        { SibiRequestItemsCols.ItemUID, Guid.NewGuid().ToString() },
                        { SibiRequestItemsCols.ModifyStatus, modifyStatus },
                        { SibiRequestItemsCols.RequestorId, "1" }//TODO: Get requestor info from approval_users table
                    };
                    itemDataTable = dbParser.ReturnInsertTable(itemInsertQuery, addlValues);
                    DBFactory.GetDatabase().UpdateTable(itemInsertQuery, itemDataTable, currentTransaction);

                }
                else
                {
                    var itemSelectQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.ItemUID + " = '" + currentItemUID + "'";
                    addlValues = new Dictionary<string, object>
                    {
                        { SibiRequestItemsCols.ModifyStatus, modifyStatus },
                        { SibiRequestItemsCols.RequestorId, "1" }//TODO: Get requestor info from approval_users table
                    };
                    itemDataTable = dbParser.ReturnUpdateTable(itemSelectQuery, addlValues);
                    DBFactory.GetDatabase().UpdateTable(itemSelectQuery, itemDataTable, currentTransaction);
                }


                itemDataTable.Dispose();

                ParentForm.RefreshData();
                this.Close();
            }
            else
            {
                Console.WriteLine("Fields INVALID");
            }
        }


        /// <summary>
        /// Checks if approval is required, checks if the item is new, and checks which values have been changed to determine the correct item state.
        /// </summary>
        /// <returns></returns>
        private ItemChangeStatus GetChangeStatus()
        {
            // If approval is required, continue with more conditions.
            if (ApprovalRequiredCheckBox.Checked)
            {
                // New item. Set to New state.
                if (isNew)
                {
                    return ItemChangeStatus.MODNEW;
                }
                else
                {

                    // List of columns that require only a notification when changed.
                    var notifyOnlyColumns = new List<string>()
                    {
                        { SibiRequestItemsCols.Status },
                        { SibiRequestItemsCols.NewAsset },
                        { SibiRequestItemsCols.NewSerial }
                    };

                    // Get list of changed columns from db parser.
                    var changedColumns = dbParser.GetChangedColumns();


                    // If there are changes.
                    if (changedColumns.Count > 0)
                    {

                        // Get a collection containing all changed columns except for the notify only columns.
                        // If the remaining collection has elements, we know that columns requiring an approval have been changed.
                        var approvalColumns = changedColumns.Except(notifyOnlyColumns);

                        if (approvalColumns.Count() > 0)
                        {
                            // Items requiring approval have been changed. Set to Changed.
                            return ItemChangeStatus.MODCHAN;
                        }
                        else
                        {
                            // Notify only items changed. Set to Status Change.
                            return ItemChangeStatus.MODSTCH;
                        }
                    }
                    else // There are no changes. Set to Current (up-to-date) state.
                    {
                        return ItemChangeStatus.MODCURR;
                    }
                }
            }
            // Approval is not required. Set to Current (up-to-date) state.
            return ItemChangeStatus.MODCURR;
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            AcceptChanges();
        }


        public void LoadItem(string itemUID)
        {
            currentItemUID = itemUID;
            var selectItemQuery = "SELECT * FROM " + SibiRequestItemsCols.TableName + " WHERE " + SibiRequestItemsCols.ItemUID + " = '" + itemUID + "'";

            //using (var itemTable = DBFactory.GetDatabase().DataTableFromQueryString(selectItemQuery))
            using (var itemCmd = DBFactory.GetDatabase().GetCommand(selectItemQuery))
            using (var itemTable = DBFactory.GetDatabase().DataTableFromCommand(itemCmd, currentTransaction))
            {

                dbParser.FillDBFields(itemTable);

            }

            this.Show();

        }

        private void SibiManageItemForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            request.Dispose();
        }

    }
}
