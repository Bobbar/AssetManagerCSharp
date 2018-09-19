using AssetManager.Data.Classes;
using AssetManager.Data.Functions;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using iTextSharp.text.pdf;
using AdvancedDialog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace AssetManager.Business
{
    public class PdfFormFilling
    {
        private ExtendedForm parentForm;
        private Device currentDevice;
        private Dialog currentDialog;
        private string unitPriceTxtName = "txtUnitPrice";

        public PdfFormFilling(ExtendedForm parentForm, Device device, PdfFormType pdfType)
        {
            this.parentForm = parentForm;
            currentDevice = device;
            FillForm(pdfType);
        }

        //public void ListFieldNames()
        //{
        //    PdfReader pdfReader = new PdfReader(Properties.Resources.Exh_K_02_Asset_Disposal_Form);
        //    StringBuilder sb = new StringBuilder();
        //    // var de = new KeyValuePair<string, AcroFields.Item>();

        //    foreach (KeyValuePair<string, AcroFields.Item> de in pdfReader.AcroFields.Fields)
        //    {
        //        sb.Append(de.Key.ToString() + Environment.NewLine);
        //    }
        //    Debug.Print(sb.ToString());
        //}

        private string GetUnitPrice()
        {
            using (var newDialog = new Dialog(parentForm))
            {
                currentDialog = newDialog;
                newDialog.Text = "Input Unit Price";
                newDialog.AddTextBox(unitPriceTxtName, "Enter Unit Price:");
                newDialog.AddButton("cmdReqSelect", "Select From Req.", PriceFromMunis);
                newDialog.ShowDialog();
                if (newDialog.DialogResult == DialogResult.OK)
                {
                    return newDialog.GetControlValue(unitPriceTxtName).ToString();
                }
            }

            return string.Empty;
        }

        private async void PriceFromMunis()
        {
            try
            {
                OtherFunctions.Message("Please Double-Click a MUNIS line item on the following window.", MessageBoxButtons.OK, MessageBoxIcon.Information, "Input Needed");
                var SelectedPrice = await MunisFunctions.NewMunisReqSearch(MunisFunctions.GetReqNumberFromPO(currentDevice.PO), MunisFunctions.GetFYFromPO(currentDevice.PO), parentForm, true);
                decimal decPrice = Convert.ToDecimal(SelectedPrice);
                var SelectedUnitPrice = decPrice.ToString("C");
                currentDialog.SetControlValue(unitPriceTxtName, SelectedUnitPrice);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private void FillForm(PdfFormType type)
        {
            try
            {
                Directory.CreateDirectory(Paths.DownloadPath);
                string strTimeStamp = DateTime.Now.ToString("_hhmmss");
                string newFile;

                if (currentDevice.AssetTag == "NA")
                {
                    newFile = Paths.DownloadPath + currentDevice.Serial + " - " + currentDevice.Description + strTimeStamp + ".pdf";
                }
                else
                {
                    newFile = Paths.DownloadPath + currentDevice.AssetTag + " - " + currentDevice.Description + strTimeStamp + ".pdf";
                }

                if (type == PdfFormType.InputForm)
                {
                    using (PdfReader pdfReader = new PdfReader(Properties.Resources.Exh_K_01_Asset_Input_Formnew))
                    {
                        using (var newFileStream = new FileStream(newFile, FileMode.Create))
                        using (var pdfStamper = new PdfStamper(pdfReader, newFileStream))
                        {
                            AcroFields pdfFormFields = InputFormFields(currentDevice, pdfStamper);
                            if (pdfFormFields == null) return;
                            pdfStamper.FormFlattening = FlattenPrompt();
                        }
                    }

                }
                else if (type == PdfFormType.TransferForm)
                {
                    using (PdfReader pdfReader = new PdfReader(Properties.Resources.Exh_K_03_Asset_Transfer_Form))
                    {

                        using (var newFileStream = new FileStream(newFile, FileMode.Create))
                        using (var pdfStamper = new PdfStamper(pdfReader, newFileStream))
                        {
                            AcroFields pdfFormFields = TransferFormFields(currentDevice, pdfStamper);
                            if (pdfFormFields == null) return;
                            pdfStamper.FormFlattening = FlattenPrompt();
                        }
                    }

                }
                else if (type == PdfFormType.DisposeForm)
                {
                    using (PdfReader pdfReader = new PdfReader(Properties.Resources.Exh_K_02_Asset_Disposal_Form))
                    {
                        using (var newFileStream = new FileStream(newFile, FileMode.Create))
                        using (var pdfStamper = new PdfStamper(pdfReader, newFileStream))
                        {
                            AcroFields pdfFormFields = DisposalFormFields(currentDevice, pdfStamper);
                            if (pdfFormFields == null) return;
                            pdfStamper.FormFlattening = FlattenPrompt();
                        }
                    }

                }

                Process.Start(newFile);
            }
            catch (Exception ex)
            {
                ErrorHandling.ErrHandle(ex, System.Reflection.MethodBase.GetCurrentMethod());
            }
        }

        private bool FlattenPrompt()
        {
            var blah = OtherFunctions.Message("Select 'Yes' to save the PDF as an editable form. Select 'No' to save the PDF as a flattened, ready to print document.", MessageBoxButtons.YesNo, MessageBoxIcon.Question, "PDF Type");
            if (blah == DialogResult.Yes)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private AcroFields DisposalFormFields(Device device, PdfStamper pdfStamper)
        {
            AcroFields tmpFields = pdfStamper.AcroFields;
            using (var newDialog = new Dialog(parentForm, true))
            {
                newDialog.Text = "Additional Input Required";

                #region Section2

                newDialog.AddLabel("Reason for asset disposal-please check one:", true);
                newDialog.AddCheckBox("chkAuction", "Prep for public auction:");
                newDialog.AddCheckBox("chkObsolete", "Functional obsolescence:");
                newDialog.AddCheckBox("chkTradeIn", "Trade-in or exchange:");
                newDialog.AddCheckBox("chkDamaged", "Asset is damaged beyond repair:");
                newDialog.AddCheckBox("chkScrap", "Sold as scrap, not at a public sale:");
                newDialog.AddCheckBox("chkParts", "Used for parts:");
                newDialog.AddCheckBox("chkOther", "Other:");
                newDialog.AddRichTextBox("rtbOther", "If Other, Please explain:");

                #endregion

                #region Section3

                newDialog.AddLabel("Method of asset disposal-please check one:", true);
                newDialog.AddCheckBox("chkHand", "Hand carried by:");
                newDialog.AddRichTextBox("rtbHand", "");
                newDialog.AddCheckBox("chkCarrier", "Carrier company:");
                newDialog.AddRichTextBox("rtbCarrier", "");
                newDialog.AddCheckBox("chkShipping", "Shipping receipt number:");
                newDialog.AddRichTextBox("rtbShipping", "");
                newDialog.AddCheckBox("chkDisposed", "Disposed of on premises:");
                newDialog.AddRichTextBox("rtbDisposed", "");
                newDialog.AddCheckBox("chkOtherMethod", "Other. Please explain:");
                newDialog.AddRichTextBox("rtpOtherMethod", "");

                #endregion

                #region Section4

                newDialog.AddTextBox("txtSaleAmount", "List the amount of proceeds from the sale of the disposed asset, if any.");
                newDialog.AddLabel("If the asset item was traded, provide the following information for the asset BEGING ACQUIRED:", true);
                newDialog.AddTextBox("txtAssetTag", "Asset/Tag Number:");
                newDialog.AddTextBox("txtSerial", "Serial Number:");
                newDialog.AddTextBox("txtDescription", "Description:");

                #endregion

                newDialog.ShowDialog();
                if (newDialog.DialogResult != DialogResult.OK)
                {
                    return null;
                }
                tmpFields.SetField("topmostSubform[0].Page1[0].AssetTag_number[0]", device.AssetTag);
                tmpFields.SetField("topmostSubform[0].Page1[0].Mfg_serial_number_1[0]", device.Serial);
                tmpFields.SetField("topmostSubform[0].Page1[0].Mfg_serial_number_2[0]", device.Description);
                tmpFields.SetField("topmostSubform[0].Page1[0].Mfg_serial_number_3[0]", AttributeFunctions.DepartmentOf(device.Location));
                tmpFields.SetField("topmostSubform[0].Page1[0].County_s_possession[0]", DateTime.Now.ToString("MM/dd/yyyy"));

                #region Section 2

                tmpFields.SetField("topmostSubform[0].Page1[0].Preparation_for_public_auction[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkAuction"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Functional_obsolescence[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkObsolete"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Trade-in_or_exchange[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkTradeIn"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Asset_is_damaged_beyond_repair[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkDamaged"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Sold_as_scrap__not_at_a_public_sale[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkScrap"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Used_for_parts[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkParts"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].undefined[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkOther"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Other__Please_explain_2[0]", newDialog.GetControlValue("rtbOther").ToString());

                #endregion

                #region Section 3

                tmpFields.SetField("topmostSubform[0].Page1[0].Method_of_asset_disposal_please_check_one[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkHand"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Hand_carried_by[0]", newDialog.GetControlValue("rtbHand").ToString());
                tmpFields.SetField("topmostSubform[0].Page1[0]._1[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkCarrier"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Carrier_company[0]", newDialog.GetControlValue("rtbCarrier").ToString());
                tmpFields.SetField("topmostSubform[0].Page1[0]._2[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkShipping"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Shipping_receipt_number[0]", newDialog.GetControlValue("rtbShipping").ToString());
                tmpFields.SetField("topmostSubform[0].Page1[0]._3[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkDisposed"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Disposed_of_on_premises[0]", newDialog.GetControlValue("rtbDisposed").ToString());
                tmpFields.SetField("topmostSubform[0].Page1[0]._4[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkOtherMethod"))));
                tmpFields.SetField("topmostSubform[0].Page1[0].Other__Please_explain_3[0]", newDialog.GetControlValue("rtpOtherMethod").ToString());

                #endregion

                #region Section 4

                tmpFields.SetField("topmostSubform[0].Page1[0].List_the_amount_of_proceeds_from_the_sale_of_the_disposed_asset__if_any[0]", newDialog.GetControlValue("txtSaleAmount").ToString());
                tmpFields.SetField("topmostSubform[0].Page1[0].AssetTag_number_2[0]", newDialog.GetControlValue("txtAssetTag").ToString());
                tmpFields.SetField("topmostSubform[0].Page1[0].Serial_number[0]", newDialog.GetControlValue("txtSerial").ToString());
                tmpFields.SetField("topmostSubform[0].Page1[0].Description_of_asset[0]", newDialog.GetControlValue("txtDescription").ToString());
                tmpFields.SetField("topmostSubform[0].Page1[0].Department_1[0]", AttributeFunctions.DepartmentOf(device.Location));
                tmpFields.SetField("topmostSubform[0].Page1[0].Date[0]", DateTime.Now.ToString("MM/dd/yyyy"));

                #endregion

            }

            return tmpFields;
        }

        private AcroFields InputFormFields(Device device, PdfStamper pdfStamper)
        {
            AcroFields tmpFields = pdfStamper.AcroFields;
            string unitPrice = GetUnitPrice();
            if (string.IsNullOrEmpty(unitPrice))
            {
                return null;
            }
            tmpFields.SetField("topmostSubform[0].Page1[0].Department[0]", "FCBDD");
            // .SetField("topmostSubform[0].Page1[0].Asterisked_items_____must_be_completed_by_the_department[0]", CurrentDevice.strAssetTag)
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined[0]", device.Serial);
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_2[0]", MunisFunctions.GetVendorNameFromPO(device.PO));
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_3[0]", device.Description);
            //.SetField("topmostSubform[0].Page1[0]._1[0]", "6")
            // .SetField("topmostSubform[0].Page1[0]._2[0]", "7")
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_4[0]", device.PO);
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_5[0]", AssetManagerFunctions.GetMunisCodeFromAssetCode(device.Location));
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_6[0]", AttributeFunctions.DepartmentOf(device.Location));
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_7[0]", AssetManagerFunctions.GetMunisCodeFromAssetCode(device.EquipmentType));
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_8[0]", "GP");
            //.SetField("topmostSubform[0].Page1[0].undefined_9[0]", "13")
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_10[0]", "1");
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_11[0]", unitPrice);
            tmpFields.SetField("topmostSubform[0].Page1[0].undefined_12[0]", device.PurchaseDate.ToString("MM/dd/yyyy"));
            tmpFields.SetField("topmostSubform[0].Page1[0].Date[0]", DateTime.Now.ToString("MM/dd/yyyy"));
            return tmpFields;
        }

        private AcroFields TransferFormFields(Device device, PdfStamper pdfStamper)
        {
            try
            {
                AcroFields tmpFields = pdfStamper.AcroFields;
                using (var newDialog = new Dialog(parentForm))
                {
                    newDialog.Text = "Additional Input Required";

                    ComboBox cmbFrom = new ComboBox();
                    cmbFrom.FillComboBox(Attributes.DeviceAttributes.Locations);
                    newDialog.AddCustomControl("cmbFromLoc", "Transfer FROM:", (Control)cmbFrom);

                    ComboBox cmbTo = new ComboBox();
                    cmbTo.FillComboBox(Attributes.DeviceAttributes.Locations);
                    newDialog.AddCustomControl("cmbToLoc", "Transfer TO:", (Control)cmbTo);

                    newDialog.AddLabel("Reason For Transfer-Check One:", true);
                    newDialog.AddCheckBox("chkBetterU", "Better Use of asset:");
                    newDialog.AddCheckBox("chkTradeIn", "Trade-in or exchange:");
                    newDialog.AddCheckBox("chkExcess", "Excess assets:");
                    newDialog.AddCheckBox("chkOther", "Other:");
                    newDialog.AddRichTextBox("rtbOther", "If Other, Please explain:");
                    newDialog.ShowDialog();

                    if (newDialog.DialogResult != DialogResult.OK)
                    {
                        return null;
                    }

                    string fromLocationCode = cmbFrom.SelectedValue.ToString();
                    string fromLocDescription = cmbFrom.Text;
                    string toLocationCode = cmbTo.SelectedValue.ToString();
                    string toLocDescription = cmbTo.Text;

                    tmpFields.SetField("topmostSubform[0].Page1[0].AssetTag_number[0]", device.AssetTag);
                    tmpFields.SetField("topmostSubform[0].Page1[0].Serial_number[0]", device.Serial);
                    tmpFields.SetField("topmostSubform[0].Page1[0].Description_of_asset[0]", device.Description);
                    tmpFields.SetField("topmostSubform[0].Page1[0].Department[0]", AttributeFunctions.DepartmentOf(fromLocationCode));
                    tmpFields.SetField("topmostSubform[0].Page1[0].Location[0]", fromLocDescription);
                    tmpFields.SetField("topmostSubform[0].Page1[0].Department_2[0]", AttributeFunctions.DepartmentOf(toLocationCode));
                    tmpFields.SetField("topmostSubform[0].Page1[0].Location_2[0]", toLocDescription);
                    tmpFields.SetField("topmostSubform[0].Page1[0].Better_utilization_of_assets[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkBetterU"))));
                    tmpFields.SetField("topmostSubform[0].Page1[0].Trade-in_or_exchange_with_Other_Departments[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkTradeIn"))));
                    tmpFields.SetField("topmostSubform[0].Page1[0].Excess_assets[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkExcess"))));
                    tmpFields.SetField("topmostSubform[0].Page1[0].undefined[0]", CheckValueToString(System.Convert.ToBoolean(newDialog.GetControlValue("chkOther"))));
                    tmpFields.SetField("topmostSubform[0].Page1[0].Other__Please_explain_1[0]", newDialog.GetControlValue("rtbOther").ToString());
                    //key
                    //topmostSubform[0].Page1[0].AssetTag_number[0]
                    //topmostSubform[0].Page1[0].Serial_number[0]
                    //topmostSubform[0].Page1[0].Description_of_asset[0]
                    //topmostSubform[0].Page1[0].Department[0]
                    //topmostSubform[0].Page1[0].Location[0]
                    //topmostSubform[0].Page1[0].Department_2[0]
                    //topmostSubform[0].Page1[0].Location_2[0]
                    //topmostSubform[0].Page1[0].Better_utilization_of_assets[0]
                    //topmostSubform[0].Page1[0].Trade-in_or_exchange_with_Other_Departments[0]
                    //topmostSubform[0].Page1[0].Excess_assets[0]
                    //topmostSubform[0].Page1[0].undefined[0]
                    //topmostSubform[0].Page1[0].Other__Please_explain_1[0]
                    //topmostSubform[0].Page1[0].Other__Please_explain_2[0]
                    //topmostSubform[0].Page1[0].Method_of_Delivery_or_Shipping_Please_Check_One[0]
                    //topmostSubform[0].Page1[0].Hand-carried_by[0]
                    //topmostSubform[0].Page1[0].undefined_2[0]
                    //topmostSubform[0].Page1[0].Carrier_company[0]
                    //topmostSubform[0].Page1[0].US_Mail[0]
                    //topmostSubform[0].Page1[0].Shipping_receipt_number[0]
                    //topmostSubform[0].Page1[0].Date_of_shipment_or_transfer[0]
                    //topmostSubform[0].Page1[0].Signature_of_SENDING_official[0]
                    //topmostSubform[0].Page1[0].Department_3[0]
                    //topmostSubform[0].Page1[0].Date[0]
                    //topmostSubform[0].Page1[0].Signature_of_RECEIVING_official[0]
                    //topmostSubform[0].Page1[0].Department_4[0]
                    //topmostSubform[0].Page1[0].Date_2[0]
                    //topmostSubform[0].Page1[0].PrintButton1[0]
                }

                return tmpFields;
            }
            catch (Exception ex)
            {
                // Log exception and fail silently.
                Logging.Exception(ex);
            }

            return null;
        }

        private string CheckValueToString(bool checkedValue)
        {
            if (checkedValue)
            {
                return "X";
            }
            else
            {
                return "";
            }
        }

        public enum PdfFormType
        {
            InputForm,
            TransferForm,
            DisposeForm
        }


    }
}