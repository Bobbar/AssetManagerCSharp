﻿Option Explicit On
Imports MySql.Data.MySqlClient
Public Class AddNew
    Private Device As Device_Info
    Private bolCheckFields As Boolean
    Private Sub cmdAdd_Click(sender As Object, e As EventArgs) Handles cmdAdd.Click
        AddNewDevice()
    End Sub
    Private Sub AddNewDevice()
        Try
            Dim strUID As String = Guid.NewGuid.ToString
            Dim rows As Integer
            If Not CheckFields() Then
                Dim blah = MsgBox("Some required fields are missing.  Please fill in all highlighted fields.", vbOKOnly + vbExclamation, "Missing Data")
                bolCheckFields = True
                Exit Sub
            End If
            GetDBValues()
            Dim strSqlQry1 = "INSERT INTO devices (dev_UID,dev_description,dev_location,dev_cur_user,dev_serial,dev_asset_tag,dev_purchase_date,dev_po,dev_replacement_year,dev_eq_type,dev_osversion,dev_status,dev_lastmod_user,dev_trackable) VALUES(@dev_UID,@dev_description,@dev_location,@dev_cur_user,@dev_serial,@dev_asset_tag,@dev_purchase_date,@dev_po,@dev_replacement_year,@dev_eq_type,@dev_osversion,@dev_status,@dev_lastmod_user,@dev_trackable)"
            Dim cmd As New MySqlCommand
            cmd.Parameters.AddWithValue("@dev_UID", strUID)
            cmd.Parameters.AddWithValue("@dev_description", Device.strDescription)
            cmd.Parameters.AddWithValue("@dev_location", Device.strLocation)
            cmd.Parameters.AddWithValue("@dev_cur_user", Device.strCurrentUser)
            cmd.Parameters.AddWithValue("@dev_serial", Device.strSerial)
            cmd.Parameters.AddWithValue("@dev_asset_tag", Device.strAssetTag)
            cmd.Parameters.AddWithValue("@dev_purchase_date", Device.dtPurchaseDate)
            cmd.Parameters.AddWithValue("@dev_po", Device.strPO)
            cmd.Parameters.AddWithValue("@dev_replacement_year", Device.strReplaceYear)
            cmd.Parameters.AddWithValue("@dev_eq_type", Device.strEqType)
            cmd.Parameters.AddWithValue("@dev_osversion", Device.strOSVersion)
            cmd.Parameters.AddWithValue("@dev_status", Device.strStatus)
            cmd.Parameters.AddWithValue("@dev_lastmod_user", strLocalUser)
            cmd.Parameters.AddWithValue("@dev_trackable", Convert.ToInt32(Device.bolTrackable))
            cmd.Connection = GlobalConn
            cmd.CommandText = strSqlQry1
            rows = rows + cmd.ExecuteNonQuery()
            Dim strSqlQry2 = "INSERT INTO historical (hist_change_type, hist_notes, hist_serial, hist_description, hist_location, hist_cur_user, hist_asset_tag, hist_purchase_date, hist_replacement_year, hist_po, hist_osversion, hist_dev_UID, hist_action_user, hist_eq_type, hist_status, hist_trackable) VALUES(@hist_change_type, @hist_notes, @hist_serial, @hist_description, @hist_location, @hist_cur_user, @hist_asset_tag, @hist_purchase_date, @hist_replacement_year, @hist_po, @hist_osversion, @hist_dev_UID, @hist_action_user, @hist_eq_type, @hist_status, @hist_trackable)"
            cmd.Parameters.AddWithValue("@hist_change_type", "NEWD")
            cmd.Parameters.AddWithValue("@hist_notes", Device.strNote)
            cmd.Parameters.AddWithValue("@hist_serial", Device.strSerial)
            cmd.Parameters.AddWithValue("@hist_description", Device.strDescription)
            cmd.Parameters.AddWithValue("@hist_location", Device.strLocation)
            cmd.Parameters.AddWithValue("@hist_cur_user", Device.strCurrentUser)
            cmd.Parameters.AddWithValue("@hist_asset_tag", Device.strAssetTag)
            cmd.Parameters.AddWithValue("@hist_purchase_date", Device.dtPurchaseDate)
            cmd.Parameters.AddWithValue("@hist_replacement_year", Device.strReplaceYear)
            cmd.Parameters.AddWithValue("@hist_po", Device.strPO)
            cmd.Parameters.AddWithValue("@hist_osversion", Device.strOSVersion)
            cmd.Parameters.AddWithValue("@hist_dev_UID", strUID)
            cmd.Parameters.AddWithValue("@hist_action_user", strLocalUser)
            cmd.Parameters.AddWithValue("@hist_eq_type", Device.strEqType)
            cmd.Parameters.AddWithValue("@hist_status", Device.strStatus)
            cmd.Parameters.AddWithValue("@hist_trackable", Convert.ToInt32(Device.bolTrackable))
            cmd.CommandText = strSqlQry2
            rows = rows + cmd.ExecuteNonQuery()
            If rows = 2 Then 'ExecuteQuery returns the number of rows affected. We can check this to make sure the qry completed successfully.
                Dim blah = MsgBox("New Device Added.   Add another?", vbYesNo + vbInformation, "Complete")
                If Not chkNoClear.Checked Then ClearAll()
                If blah = vbNo Then Me.Hide()
            Else
                Dim blah = MsgBox("Unsuccessful! The number of affected rows was not what was expected.", vbOKOnly + vbExclamation, "Unexpected Result")
            End If
            Exit Sub
        Catch ex As Exception
            If ErrHandleNew(ex, System.Reflection.MethodInfo.GetCurrentMethod().Name) Then
                Exit Sub
            Else
                EndProgram()
            End If
        End Try
    End Sub
    Private Function CheckFields() As Boolean
        Dim bolMissingField As Boolean
        bolMissingField = False
        Dim c As Control
        For Each c In GroupBox1.Controls
            Select Case True
                Case TypeOf c Is TextBox
                    If c.Name.Contains("REQ") Then
                        If Trim(c.Text) = "" Then
                            bolMissingField = True
                            c.BackColor = colMissingField
                        Else
                            c.BackColor = Color.Empty
                        End If
                    End If
                Case TypeOf c Is ComboBox
                    Dim cmb As ComboBox = c
                    If cmb.Name.Contains("REQ") Then
                        If cmb.SelectedIndex = -1 Then
                            bolMissingField = True
                            cmb.BackColor = colMissingField
                        Else
                            cmb.BackColor = Color.Empty
                        End If
                    End If
            End Select
        Next
        Return Not bolMissingField 'if fields are missing return false to trigger a message if needed
    End Function
    Private Sub cmdClear_Click(sender As Object, e As EventArgs) Handles cmdClear.Click
        ClearAll()
    End Sub
    Private Sub GetDBValues() 'cleanup user input for db
        Device.strSerial = Trim(txtSerial_REQ.Text)
        Device.strDescription = Trim(txtDescription_REQ.Text)
        Device.strAssetTag = Trim(txtAssetTag_REQ.Text)
        'strPurchaseDate = Format(dtPurchaseDate.Text, strDBDateFormat)
        Device.dtPurchaseDate = dtPurchaseDate_REQ.Text
        Device.strReplaceYear = Trim(txtReplaceYear.Text)
        Device.strLocation = GetDBValue(ComboType.Location, cmbLocation_REQ.SelectedIndex)
        Device.strCurrentUser = Trim(txtCurUser_REQ.Text)
        Device.strNote = Trim(txtNotes.Text)
        Device.strOSVersion = GetDBValue(ComboType.OSType, cmbOSType_REQ.SelectedIndex)
        Device.strEqType = GetDBValue(ComboType.EquipType, cmbEquipType_REQ.SelectedIndex)
        Device.strStatus = GetDBValue(ComboType.StatusType, cmbStatus_REQ.SelectedIndex)
        Device.bolTrackable = chkTrackable.Checked
        Device.strPO = Trim(txtPO.Text)

        'strOSVersion =
    End Sub
    Private Sub AddNew_Load(sender As Object, e As EventArgs) Handles Me.Load
        ClearAll()
    End Sub
    Private Sub ClearAll()
        FillLocationCombo()
        FillEquipTypeCombo()
        FillOSTypeCombo()
        FillStatusTypeCombo()
        ClearFields()
        dtPurchaseDate_REQ.Value = Now
        cmbStatus_REQ.SelectedIndex = GetComboIndexFromShort(ComboType.StatusType, "INSRV")
        ResetBackColors()
        chkTrackable.Checked = False
        chkNoClear.Checked = False
        bolCheckFields = False
    End Sub
    Private Sub ResetBackColors()
        Dim c As Control
        For Each c In GroupBox1.Controls
            ' c.BackColor = Color.Empty
            Select Case True
                Case TypeOf c Is TextBox
                    c.BackColor = Color.Empty
                Case TypeOf c Is ComboBox
                    c.BackColor = Color.Empty
            End Select
        Next
    End Sub
    Private Sub ClearFields()
        Dim c As Control
        For Each c In GroupBox1.Controls
            If TypeOf c Is TextBox Then
                Dim txt As TextBox = c
                txt.Text = ""
            End If
            If TypeOf c Is ComboBox Then
                Dim cmb As ComboBox = c
                cmb.SelectedIndex = -1
            End If
        Next
    End Sub
    Private Sub FillLocationCombo()
        Dim i As Integer
        cmbLocation_REQ.Items.Clear()
        cmbLocation_REQ.Text = ""
        For i = 0 To UBound(Locations)
            cmbLocation_REQ.Items.Insert(i, Locations(i).strLong)
        Next
    End Sub
    Private Sub FillEquipTypeCombo()
        Dim i As Integer
        cmbEquipType_REQ.Items.Clear()
        cmbEquipType_REQ.Text = ""
        For i = 0 To UBound(EquipType)
            cmbEquipType_REQ.Items.Insert(i, EquipType(i).strLong)
        Next
    End Sub
    Private Sub FillOSTypeCombo()
        Dim i As Integer
        cmbOSType_REQ.Items.Clear()
        cmbOSType_REQ.Text = ""
        For i = 0 To UBound(EquipType)
            cmbOSType_REQ.Items.Insert(i, OSType(i).strLong)
        Next
    End Sub
    Private Sub FillStatusTypeCombo()
        Dim i As Integer
        cmbStatus_REQ.Items.Clear()
        cmbStatus_REQ.Text = ""
        For i = 0 To UBound(StatusType)
            cmbStatus_REQ.Items.Insert(i, StatusType(i).strLong)
        Next
    End Sub
    Private Sub Button1_Click(sender As Object, e As EventArgs)
        If Not CheckFields() Then Dim blah = MsgBox("Some required fields are missing.  Please fill in all highlighted fields.", vbOKOnly + vbExclamation, "Missing Data")
    End Sub
    Private Sub txtSerial_REQ_TextChanged(sender As Object, e As EventArgs) Handles txtSerial_REQ.TextChanged
        If bolCheckFields Then CheckFields()
    End Sub
    Private Sub txtCurUser_REQ_TextChanged(sender As Object, e As EventArgs) Handles txtCurUser_REQ.TextChanged
        If bolCheckFields Then CheckFields()
    End Sub
    Private Sub cmbLocation_REQ_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbLocation_REQ.SelectedIndexChanged
        If bolCheckFields Then CheckFields()
    End Sub
    Private Sub cmbEquipType_REQ_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbEquipType_REQ.SelectedIndexChanged
        If bolCheckFields Then CheckFields()
    End Sub
    Private Sub cmbStatus_REQ_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbStatus_REQ.SelectedIndexChanged
        If bolCheckFields Then CheckFields()
    End Sub
    Private Sub txtAssetTag_REQ_TextChanged(sender As Object, e As EventArgs) Handles txtAssetTag_REQ.TextChanged
        If bolCheckFields Then CheckFields()
    End Sub
    Private Sub txtDescription_REQ_TextChanged(sender As Object, e As EventArgs) Handles txtDescription_REQ.TextChanged
        If bolCheckFields Then CheckFields()
    End Sub
    Private Sub dtPurchaseDate_REQ_ValueChanged(sender As Object, e As EventArgs) Handles dtPurchaseDate_REQ.ValueChanged
        If bolCheckFields Then CheckFields()
    End Sub
    Private Sub cmbOSType_REQ_SelectedIndexChanged(sender As Object, e As EventArgs) Handles cmbOSType_REQ.SelectedIndexChanged
        If bolCheckFields Then CheckFields()
    End Sub
    Private Sub cmbLocation_REQ_DropDown(sender As Object, e As EventArgs) Handles cmbLocation_REQ.DropDown
        AdjustComboBoxWidth(sender, e)
    End Sub
    Private Sub cmbOSType_REQ_DropDown(sender As Object, e As EventArgs) Handles cmbOSType_REQ.DropDown
        AdjustComboBoxWidth(sender, e)
    End Sub
    Private Sub cmbStatus_REQ_DropDown(sender As Object, e As EventArgs) Handles cmbStatus_REQ.DropDown
        AdjustComboBoxWidth(sender, e)
    End Sub
    Private Sub cmbEquipType_REQ_DropDown(sender As Object, e As EventArgs) Handles cmbEquipType_REQ.DropDown
        AdjustComboBoxWidth(sender, e)
    End Sub
End Class
