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

namespace AssetManager.UserInterface.Forms.Sibi
{
    public partial class SibiManageItemForm : ExtendedForm
    {
        DBControlParser dbParser;
        private bool isNew = false;
        private SibiRequestMapObject request;
        public SibiManageItemForm(ExtendedForm parentForm, SibiRequestMapObject request) : base(parentForm)
        {
            InitializeComponent();
            InitDBControls();
            dbParser = new DBControlParser(this);
            dbParser.EnableFieldValidation(errorProvider);
            this.Owner = parentForm;
            this.request = request;
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



            AttribIndexFunctions.FillComboBox(GlobalInstances.DeviceAttribute.Locations, LocationComboBox.ComboBox);
            AttribIndexFunctions.FillComboBox(GlobalInstances.SibiAttribute.ItemStatusType, StatusComboBox.ComboBox);


        }

        private void AcceptChanges()
        {
            if (dbParser.ValidateFields(errorProvider))
            {
                // Do lots of stuff
                Console.WriteLine("Valid Fields");


                var addlValues = new Dictionary<string, object>
                {
                    { SibiRequestItemsCols.RequestUID,request.GUID },
                    { SibiRequestItemsCols.ItemUID, Guid.NewGuid().ToString() },
                    { SibiRequestItemsCols.RequiresApproval, ApprovalRequiredCheckBox.Checked }
                };

                var newItemDataTable = dbParser.ReturnInsertTable("SELECT * FROM " + SibiRequestItemsCols.TableName + " LIMIT 0", addlValues);

                DBFactory.GetDatabase().UpdateTable("SELECT * FROM " + SibiRequestItemsCols.TableName + " LIMIT 0", newItemDataTable);










            }
            else
            {
                Console.WriteLine("Fields INVALID");
            }
        }

        private void AcceptButton_Click(object sender, EventArgs e)
        {
            AcceptChanges();
        }






        //private void AddErrorIcon(Control ctl)
        //{
        //    if (ReferenceEquals(errorProvider.GetError(ctl), string.Empty))
        //    {
        //        errorProvider.SetIconAlignment(ctl, ErrorIconAlignment.MiddleRight);
        //        errorProvider.SetIconPadding(ctl, 4);
        //        errorProvider.SetError(ctl, "Required or Invalid Field");
        //    }
        //}
        //private void ClearErrorIcon(Control ctl)
        //{
        //    errorProvider.SetError(ctl, string.Empty);
        //}


    }
}
