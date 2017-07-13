﻿Public Structure Combo_Data
    Public Property strLong As String
    Public Property strShort As String
    Public Property strID As String
End Structure

Public Structure Device_Info
    Public strAssetTag As String
    Public strDescription As String
    Public strEqType As String
    Public strSerial As String
    Public strLocation As String
    Public strCurrentUser As String
    Public strCurrentUserEmpNum As String
    Public strFiscalYear As String
    Public dtPurchaseDate As Date
    Public strReplaceYear As String
    Public strOSVersion As String
    Public strPhoneNumber As String
    Public strGUID As String
    Public strPO As String
    Public strStatus As String
    Public strNote As String
    Public bolTrackable As Boolean
    Public strSibiLink As String
    Public CheckSum As String
    Public Tracking As Track_Info
    Public Historical As Hist_Info
End Structure

Public Structure Request_Info
    Public strUID As String
    Public strUser As String
    Public strDescription As String
    Public dtDateStamp As Date
    Public dtNeedBy As Date
    Public strStatus As String
    Public strType As String
    Public strPO As String
    Public strRequisitionNumber As String
    Public strReplaceAsset As String
    Public strReplaceSerial As String
    Public strRequestNumber As String
    Public strRTNumber As String
    Public RequestItems As DataTable
End Structure

Public Structure Hist_Info
    Public strChangeType As String
    Public strHistUID As String
    Public strNote As String
    Public strActionUser As String
    Public dtActionDateTime As Date
End Structure

Public Structure Track_Info
    Public dtCheckOutTime As Date
    Public dtDueBackTime As Date
    Public dtCheckInTime As Date
    Public strCheckOutUser As String
    Public strCheckInUser As String
    Public strUseLocation As String
    Public strUseReason As String
    Public bolCheckedOut As Boolean
End Structure

Public Structure Access_Info
    Public strModule As String
    Public intLevel As Integer
    Public strDesc As String
    Public bolAvailOffline As Boolean
End Structure

Public Structure Update_Info
    Public strNote As String
    Public strChangeType As String
End Structure

Public Structure User_Info
    Public strUsername As String
    Public strFullname As String

    'Public bolIsAdmin As Boolean
    Public intAccessLevel As Integer

    Public strUID As String
End Structure

Public Structure Emp_Info
    Public Number As String
    Public Name As String
    Public UID As String
End Structure

Public Structure FTPScan_Parms
    Public IsOrphan As Boolean
    Public strTable As String
End Structure

Public Structure CheckStruct
    Public strCheckOutTime As String
    Public strDueDate As String
    Public strUseLocation As String
    Public strUseReason As String
    Public strCheckInNotes As String
    Public strDeviceUID As String
    Public strCheckOutUser As String
    Public strCheckInUser As String
    Public strCheckInTime As String
End Structure

Public Structure ColumnStruct
    Public ColumnName As String
    Public ColumnCaption As String
    Public ColumnType As Type
    Public ColumnReadOnly As Boolean
    Public ColumnVisible As Boolean
    Public ComboIndex As Combo_Data()

    Sub New(Name As String, Caption As String, Type As Type)
        ColumnName = Name
        ColumnCaption = Caption
        ColumnType = Type
        ColumnReadOnly = False
        ColumnVisible = True
        ComboIndex = Nothing
    End Sub

    Sub New(Name As String, Caption As String, Type As Type, ComboIdx() As Combo_Data)
        ColumnName = Name
        ColumnCaption = Caption
        ColumnType = Type
        ColumnReadOnly = False
        ColumnVisible = True
        ComboIndex = ComboIdx
    End Sub

    Sub New(Name As String, Caption As String, Type As Type, IsReadOnly As Boolean, Visible As Boolean)
        ColumnName = Name
        ColumnCaption = Caption
        ColumnType = Type
        ColumnReadOnly = IsReadOnly
        ColumnVisible = Visible
        ComboIndex = Nothing
    End Sub

End Structure

Public Structure StatusColorStruct
    Public StatusID As String
    Public StatusColor As Color

    Sub New(ID As String, Color As Color)
        StatusID = ID
        StatusColor = Color
    End Sub

End Structure