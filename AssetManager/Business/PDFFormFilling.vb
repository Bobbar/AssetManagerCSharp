﻿Option Explicit On
Imports iTextSharp
Imports iTextSharp.text
Imports iTextSharp.text.pdf
Imports iTextSharp.text.xml
Imports System.IO
Imports System.Collections
Imports System.ComponentModel
Imports System.Text
Public Class PDFFormFilling
    Private ParentForm As Form
    Private CurrentDevice As Device_Info
    Private UnitPrice As String
    Private CurrentDialog As MyDialog
    Private UnitPriceTxtName As String = "txtUnitPrice"
    Sub New(Parent As Form, DeviceInfo As Device_Info, Type As PDFFormType)
        ParentForm = Parent
        CurrentDevice = DeviceInfo
        FillForm(Type)
    End Sub
    Public Sub ListFieldNames()
        Dim pdfReader As PdfReader = New PdfReader(My.Resources.Exh_K_02_Asset_Disposal_Form)
        Dim sb As New StringBuilder()
        Dim de As New KeyValuePair(Of String, iTextSharp.text.pdf.AcroFields.Item) 'DictionaryEntry
        For Each de In pdfReader.AcroFields.Fields
            sb.Append(de.Key.ToString() + Environment.NewLine)
        Next
        Debug.Print(sb.ToString())
    End Sub
    Private Function GetUnitPrice(Device As Device_Info) As String
        'CurrentDevice = Device
        Dim NewDialog As New MyDialog
        CurrentDialog = NewDialog
        With NewDialog
            .Text = "Input Unit Price"
            .AddTextBox(UnitPriceTxtName, "Enter Unit Price:")
            .AddButton("cmdReqSelect", "Select From Req.", AddressOf PriceFromMunis)
            .ShowDialog()
            If .DialogResult = DialogResult.OK Then
                Return .GetControlValue(UnitPriceTxtName)
            End If
        End With
    End Function
    Private Sub PriceFromMunis()
        '  Dim Device As Device_Info = CurrentDevice
        Message("Please Double-Click a MUNIS line item on the following window.", vbOKOnly + vbInformation, "Input Needed")
        Dim f As New View_Munis(ParentForm, True)
        f.Text = "Select a Line Item"
        f.LoadDevice(CurrentDevice)
        f.LoadMunisRequisitionGridByReqNo(Munis.Get_ReqNumber_From_PO(CurrentDevice.strPO), Munis.Get_FY_From_PO(CurrentDevice.strPO))
        f.ShowDialog(ParentForm)
        If f.DialogResult = DialogResult.OK Then
            UnitPrice = f.UnitPrice
            CurrentDialog.SetControlValue(UnitPriceTxtName, UnitPrice)
        Else
            UnitPrice = Nothing
        End If
    End Sub
    Private Sub FillForm(Type As PDFFormType)
        Try
            Dim di As DirectoryInfo = Directory.CreateDirectory(strTempPath)
            Dim strTimeStamp As String = Now.ToString("_hhmmss")
            Dim newFile As String = strTempPath & CurrentDevice.strDescription & strTimeStamp & ".pdf"
            Dim pdfStamper As PdfStamper
            Select Case Type
                Case PDFFormType.InputForm
                    Dim pdfReader As New PdfReader(My.Resources.Exh_K_01_Asset_Input_Formnew)
                    pdfStamper = New PdfStamper(pdfReader, New FileStream(newFile, FileMode.Create))
                    Dim pdfFormFields As AcroFields = InputFormFields(CurrentDevice, pdfStamper) 'pdfStamper.AcroFields
                    If IsNothing(pdfFormFields) Then
                        pdfStamper.Close()
                        Exit Sub
                    End If
                Case PDFFormType.TransferForm
                    Dim pdfReader As New PdfReader(My.Resources.Exh_K_03_Asset_Transfer_Form)
                    pdfStamper = New PdfStamper(pdfReader, New FileStream(newFile, FileMode.Create))
                    Dim pdfFormFields As AcroFields = TransferFormFields(CurrentDevice, pdfStamper)
                    If IsNothing(pdfFormFields) Then
                        pdfStamper.Close()
                        Exit Sub
                    End If
                Case PDFFormType.DisposeForm
                    Dim pdfReader As New PdfReader(My.Resources.Exh_K_02_Asset_Disposal_Form)
                    pdfStamper = New PdfStamper(pdfReader, New FileStream(newFile, FileMode.Create))
                    Dim pdfFormFields As AcroFields = DisposalFormFields(CurrentDevice, pdfStamper)
                    If IsNothing(pdfFormFields) Then
                        pdfStamper.Close()
                        Exit Sub
                    End If
            End Select
            pdfStamper.FormFlattening = FlattenPrompt() 'False
            ' close the pdf
            pdfStamper.Close()
            Process.Start(newFile)
        Catch ex As Exception
            ErrHandle(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name)
        End Try
    End Sub
    Private Function FlattenPrompt() As Boolean
        Dim blah As Object = Message("Select 'Yes' to save the PDF as an editable form. Select 'No' to save the PDF as a flattened, ready to print document.", vbQuestion + vbYesNo, "PDF Type")
        If blah = vbYes Then
            Return False
        Else
            Return True
        End If
    End Function
    Private Function DisposalFormFields(Device As Device_Info, ByRef pdfStamper As PdfStamper) As AcroFields
        Dim tmpFields As AcroFields = pdfStamper.AcroFields
        Dim newDialog As New MyDialog(True)
        With newDialog
            .Text = "Additional Input Required"
#Region "Section2"
            .AddLabel("Reason for asset disposal-please check one:", True)
            .AddCheckBox("chkAuction", "Prep for public auction:")
            .AddCheckBox("chkObsolete", "Functional obsolescence:")
            .AddCheckBox("chkTradeIn", "Trade-in or exchange:")
            .AddCheckBox("chkDamaged", "Asset is damaged beyond repair:")
            .AddCheckBox("chkScrap", "Sold as scrap, not at a public sale:")
            .AddCheckBox("chkParts", "Used for parts:")
            .AddCheckBox("chkOther", "Other:")
            .AddRichTextBox("rtbOther", "If Other, Please explain:")
#End Region

#Region "Section3"
            .AddLabel("Method of asset disposal-please check one:", True)
            .AddCheckBox("chkHand", "Hand carried by:")
            .AddRichTextBox("rtbHand", "")
            .AddCheckBox("chkCarrier", "Carrier company:")
            .AddRichTextBox("rtbCarrier", "")
            .AddCheckBox("chkShipping", "Shipping receipt number:")
            .AddRichTextBox("rtbShipping", "")
            .AddCheckBox("chkDisposed", "Disposed of on premises:")
            .AddRichTextBox("rtbDisposed", "")
            .AddCheckBox("chkOtherMethod", "Other. Please explain:")
            .AddRichTextBox("rtpOtherMethod", "")

#End Region

#Region "Section4"
            .AddTextBox("txtSaleAmount", "List the amount of proceedes from the sale of the diposed asset, if any.")
            .AddLabel("If the asset item was traded, procide the following information for the asset BEGING ACQUIRED:", True)
            .AddTextBox("txtAssetTag", "Asset/Tag Number:")
            .AddTextBox("txtSerial", "Serial Number:")
            .AddTextBox("txtDescription", "Description:")
#End Region
            .ShowDialog()
        End With
        If newDialog.DialogResult <> DialogResult.OK Then Return Nothing
        With tmpFields
            .SetField("topmostSubform[0].Page1[0].AssetTag_number[0]", Device.strAssetTag)
            .SetField("topmostSubform[0].Page1[0].Mfg_serial_number_1[0]", Device.strSerial)
            .SetField("topmostSubform[0].Page1[0].Mfg_serial_number_2[0]", Device.strDescription)
            .SetField("topmostSubform[0].Page1[0].Mfg_serial_number_3[0]", "FCBDD")
            .SetField("topmostSubform[0].Page1[0].County_s_possession[0]", Now.ToString("MM/dd/yyyy"))
#Region "Section 2"
            .SetField("topmostSubform[0].Page1[0].Preparation_for_public_auction[0]", CheckValueToString(newDialog.GetControlValue("chkAuction")))
            .SetField("topmostSubform[0].Page1[0].Functional_obsolescence[0]", CheckValueToString(newDialog.GetControlValue("chkObsolete")))
            .SetField("topmostSubform[0].Page1[0].Trade-in_or_exchange[0]", CheckValueToString(newDialog.GetControlValue("chkTradeIn")))
            .SetField("topmostSubform[0].Page1[0].Asset_is_damaged_beyond_repair[0]", CheckValueToString(newDialog.GetControlValue("chkDamaged")))
            .SetField("topmostSubform[0].Page1[0].Sold_as_scrap__not_at_a_public_sale[0]", CheckValueToString(newDialog.GetControlValue("chkScrap")))
            .SetField("topmostSubform[0].Page1[0].Used_for_parts[0]", CheckValueToString(newDialog.GetControlValue("chkParts")))
            .SetField("topmostSubform[0].Page1[0].undefined[0]", CheckValueToString(newDialog.GetControlValue("chkOther")))
            .SetField("topmostSubform[0].Page1[0].Other__Please_explain_2[0]", newDialog.GetControlValue("rtbOther"))
#End Region

#Region "Section 3"
            .SetField("topmostSubform[0].Page1[0].Method_of_asset_disposal_please_check_one[0]", CheckValueToString(newDialog.GetControlValue("chkHand")))
            .SetField("topmostSubform[0].Page1[0].Hand_carried_by[0]", newDialog.GetControlValue("rtbHand"))
            .SetField("topmostSubform[0].Page1[0]._1[0]", CheckValueToString(newDialog.GetControlValue("chkCarrier")))
            .SetField("topmostSubform[0].Page1[0].Carrier_company[0]", newDialog.GetControlValue("rtbCarrier"))
            .SetField("topmostSubform[0].Page1[0]._2[0]", CheckValueToString(newDialog.GetControlValue("chkShipping")))
            .SetField("topmostSubform[0].Page1[0].Shipping_receipt_number[0]", newDialog.GetControlValue("rtbShipping"))
            .SetField("topmostSubform[0].Page1[0]._3[0]", CheckValueToString(newDialog.GetControlValue("chkDisposed")))
            .SetField("topmostSubform[0].Page1[0].Disposed_of_on_premises[0]", newDialog.GetControlValue("rtbDisposed"))
            .SetField("topmostSubform[0].Page1[0]._4[0]", CheckValueToString(newDialog.GetControlValue("chkOtherMethod")))
            .SetField("topmostSubform[0].Page1[0].Other__Please_explain_3[0]", newDialog.GetControlValue("rtpOtherMethod"))
#End Region

#Region "Section 4"
            .SetField("topmostSubform[0].Page1[0].List_the_amount_of_proceeds_from_the_sale_of_the_disposed_asset__if_any[0]", newDialog.GetControlValue("txtSaleAmount"))
            .SetField("topmostSubform[0].Page1[0].AssetTag_number_2[0]", newDialog.GetControlValue("txtAssetTag"))
            .SetField("topmostSubform[0].Page1[0].Serial_number[0]", newDialog.GetControlValue("txtSerial"))
            .SetField("topmostSubform[0].Page1[0].Description_of_asset[0]", newDialog.GetControlValue("txtDescription"))
            .SetField("topmostSubform[0].Page1[0].Department_1[0]", "FCBDD")
            .SetField("topmostSubform[0].Page1[0].Date[0]", Now.ToString("MM/dd/yyyy"))
#End Region

        End With
        Return tmpFields
    End Function
    Private Function InputFormFields(Device As Device_Info, ByRef pdfStamper As PdfStamper) As AcroFields
        Dim tmpFields As AcroFields = pdfStamper.AcroFields
        Dim strUnitPrice As String = GetUnitPrice(Device)
        If strUnitPrice = "" Or IsNothing(strUnitPrice) Then
            Exit Function
        End If
        With tmpFields
            .SetField("topmostSubform[0].Page1[0].Department[0]", "FCBDD")
            ' .SetField("topmostSubform[0].Page1[0].Asterisked_items_____must_be_completed_by_the_department[0]", CurrentDevice.strAssetTag)
            .SetField("topmostSubform[0].Page1[0].undefined[0]", Device.strSerial)
            .SetField("topmostSubform[0].Page1[0].undefined_2[0]", Munis.Get_VendorName_From_PO(Device.strPO))
            .SetField("topmostSubform[0].Page1[0].undefined_3[0]", Device.strDescription)
            '.SetField("topmostSubform[0].Page1[0]._1[0]", "6") 
            ' .SetField("topmostSubform[0].Page1[0]._2[0]", "7")
            .SetField("topmostSubform[0].Page1[0].undefined_4[0]", Device.strPO)
            .SetField("topmostSubform[0].Page1[0].undefined_5[0]", Get_MunisCode_From_AssetCode(Device.strLocation))
            .SetField("topmostSubform[0].Page1[0].undefined_6[0]", "5200")
            .SetField("topmostSubform[0].Page1[0].undefined_7[0]", Get_MunisCode_From_AssetCode(Device.strEqType))
            .SetField("topmostSubform[0].Page1[0].undefined_8[0]", "GP")
            '.SetField("topmostSubform[0].Page1[0].undefined_9[0]", "13")
            .SetField("topmostSubform[0].Page1[0].undefined_10[0]", "1")
            .SetField("topmostSubform[0].Page1[0].undefined_11[0]", strUnitPrice)
            .SetField("topmostSubform[0].Page1[0].undefined_12[0]", Device.dtPurchaseDate.ToString("MM/dd/yyyy"))
            .SetField("topmostSubform[0].Page1[0].Date[0]", Now.ToString("MM/dd/yyyy"))
        End With
        Return tmpFields
    End Function
    Private Function TransferFormFields(Device As Device_Info, ByRef pdfStamper As PdfStamper) As AcroFields
        Dim tmpFields As AcroFields = pdfStamper.AcroFields
        Dim newDialog As New MyDialog
        With newDialog
            .Text = "Additional Input Required"
            .AddComboBox("cmbFromLoc", "Transfer FROM:", DeviceIndex.Locations)
            .AddComboBox("cmbToLoc", "Transfer TO:", DeviceIndex.Locations)
            .AddLabel("Reason For Transfer-Check One:", True)
            .AddCheckBox("chkBetterU", "Better Use of asset:")
            .AddCheckBox("chkTradeIn", "Trade-in or exchange:")
            .AddCheckBox("chkExcess", "Excess assets:")
            .AddCheckBox("chkOther", "Other:")
            .AddRichTextBox("rtbOther", "If Other, Please explain:")
            .ShowDialog()
        End With
        If newDialog.DialogResult <> DialogResult.OK Then Return Nothing
        With tmpFields
            .SetField("topmostSubform[0].Page1[0].AssetTag_number[0]", Device.strAssetTag)
            .SetField("topmostSubform[0].Page1[0].Serial_number[0]", Device.strSerial)
            .SetField("topmostSubform[0].Page1[0].Description_of_asset[0]", Device.strDescription)
            .SetField("topmostSubform[0].Page1[0].Department[0]", "FCBDD - 5200")
            .SetField("topmostSubform[0].Page1[0].Location[0]", GetHumanValueFromIndex(DeviceIndex.Locations, newDialog.GetControlValue("cmbFromLoc")) & " - " & Get_MunisCode_From_AssetCode(GetDBValue(DeviceIndex.Locations, newDialog.GetControlValue("cmbFromLoc"))))
            .SetField("topmostSubform[0].Page1[0].Department_2[0]", "FCBDD - 5200")
            .SetField("topmostSubform[0].Page1[0].Location_2[0]", GetHumanValueFromIndex(DeviceIndex.Locations, newDialog.GetControlValue("cmbToLoc")) & " - " & Get_MunisCode_From_AssetCode(GetDBValue(DeviceIndex.Locations, newDialog.GetControlValue("cmbToLoc"))))
            .SetField("topmostSubform[0].Page1[0].Better_utilization_of_assets[0]", CheckValueToString(newDialog.GetControlValue("chkBetterU")))
            .SetField("topmostSubform[0].Page1[0].Trade-in_or_exchange_with_Other_Departments[0]", CheckValueToString(newDialog.GetControlValue("chkTradeIn")))
            .SetField("topmostSubform[0].Page1[0].Excess_assets[0]", CheckValueToString(newDialog.GetControlValue("chkExcess")))
            .SetField("topmostSubform[0].Page1[0].undefined[0]", CheckValueToString(newDialog.GetControlValue("chkOther")))
            .SetField("topmostSubform[0].Page1[0].Other__Please_explain_1[0]", newDialog.GetControlValue("rtbOther"))
            'key
            'topmostSubform[0].Page1[0].AssetTag_number[0]
            'topmostSubform[0].Page1[0].Serial_number[0]
            'topmostSubform[0].Page1[0].Description_of_asset[0]
            'topmostSubform[0].Page1[0].Department[0]
            'topmostSubform[0].Page1[0].Location[0]
            'topmostSubform[0].Page1[0].Department_2[0]
            'topmostSubform[0].Page1[0].Location_2[0]
            'topmostSubform[0].Page1[0].Better_utilization_of_assets[0]
            'topmostSubform[0].Page1[0].Trade-in_or_exchange_with_Other_Departments[0]
            'topmostSubform[0].Page1[0].Excess_assets[0]
            'topmostSubform[0].Page1[0].undefined[0]
            'topmostSubform[0].Page1[0].Other__Please_explain_1[0]
            'topmostSubform[0].Page1[0].Other__Please_explain_2[0]
            'topmostSubform[0].Page1[0].Method_of_Delivery_or_Shipping_Please_Check_One[0]
            'topmostSubform[0].Page1[0].Hand-carried_by[0]
            'topmostSubform[0].Page1[0].undefined_2[0]
            'topmostSubform[0].Page1[0].Carrier_company[0]
            'topmostSubform[0].Page1[0].US_Mail[0]
            'topmostSubform[0].Page1[0].Shipping_receipt_number[0]
            'topmostSubform[0].Page1[0].Date_of_shipment_or_transfer[0]
            'topmostSubform[0].Page1[0].Signature_of_SENDING_official[0]
            'topmostSubform[0].Page1[0].Department_3[0]
            'topmostSubform[0].Page1[0].Date[0]
            'topmostSubform[0].Page1[0].Signature_of_RECEIVING_official[0]
            'topmostSubform[0].Page1[0].Department_4[0]
            'topmostSubform[0].Page1[0].Date_2[0]
            'topmostSubform[0].Page1[0].PrintButton1[0]
        End With
        newDialog.Dispose()
        Return tmpFields
    End Function
    Private Function CheckValueToString(CheckValue As CheckState) As String
        If CheckValue = CheckState.Checked Then
            Return "X"
        Else
            Return ""
        End If
    End Function
End Class