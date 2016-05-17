﻿Imports System.IO
Imports MySql.Data.MySqlClient
Class Attachments
    Private Structure Attach_Struct
        Public strFilename As String
        Public strFileType As String
        Public FileSize As Int32
        Public strFileUID As String
    End Structure
    Private AttachIndex() As Attach_Struct
    Private Sub Button1_Click(sender As Object, e As EventArgs) Handles cmdUpload.Click
        Dim fd As OpenFileDialog = New OpenFileDialog()
        Dim strFileName As String
        fd.Title = "Open File Dialog"
        fd.InitialDirectory = "C:\"
        fd.Filter = "All files (*.*)|*.*|All files (*.*)|*.*"
        fd.FilterIndex = 2
        fd.RestoreDirectory = True
        If fd.ShowDialog() = DialogResult.OK Then
            strFileName = fd.FileName
        Else
            Exit Sub
        End If
        UploadFile(fd.FileName)
        ListAttachments(CurrentDevice.strGUID)
    End Sub
    Private Sub UploadFile(FilePath As String)
        Dim ConnID As String = Guid.NewGuid.ToString
        Dim table As New DataTable
        Dim cmd As New MySqlCommand
        Dim conn As New MySqlConnection
        conn = GetConnection(ConnID).DBConnection
        Dim SQL As String
        Dim FileSize As UInt32
        Dim rawData() As Byte
        Dim fs As FileStream
        Try
            fs = New FileStream(FilePath, FileMode.Open, FileAccess.Read)
            FileSize = fs.Length
            Dim strFilename As String = Path.GetFileNameWithoutExtension(FilePath)
            Dim strFileType As String = Path.GetExtension(FilePath)
            rawData = New Byte(FileSize) {}
            fs.Read(rawData, 0, FileSize)
            fs.Close()
            SQL = "INSERT INTO attachments (`attach_dev_UID`, `attach_file_name`, `attach_file_type`, `attach_file_binary`, `attach_file_size`) VALUES(@attach_dev_UID, @attach_file_name, @attach_file_type, @attach_file_binary, @attach_file_size)"
            cmd.Connection = conn
            cmd.CommandText = SQL
            cmd.Parameters.AddWithValue("@attach_dev_UID", CurrentDevice.strGUID)
            cmd.Parameters.AddWithValue("@attach_file_name", strFilename)
            cmd.Parameters.AddWithValue("@attach_file_type", strFileType)
            cmd.Parameters.AddWithValue("@attach_file_binary", rawData)
            cmd.Parameters.AddWithValue("@attach_file_size", FileSize)
            cmd.ExecuteNonQuery()
            MessageBox.Show("File Inserted into database successfully!",
            "Success!", MessageBoxButtons.OK, MessageBoxIcon.Asterisk)
            conn.Close()
        Catch ex As Exception
            MessageBox.Show("There was an error: " & ex.Message, "Error",
                MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Sub ListAttachments(DeviceUID As String)
        Waiting()
        On Error GoTo errs
        Dim ConnID As String = Guid.NewGuid.ToString
        Dim reader As MySqlDataReader
        Dim table As New DataTable
        Dim strQry = "Select UID,attach_file_name,attach_file_type,attach_file_size,attach_upload_date FROM attachments WHERE attach_dev_UID='" & DeviceUID & "'"
        Dim cmd As New MySqlCommand(strQry, GetConnection(ConnID).DBConnection)
        reader = cmd.ExecuteReader
        Dim strFullFilename As String
        Dim row As Integer
        Dim NewLine As ListViewItem
        ListView1.Items.Clear()
        ReDim AttachIndex(0)
        With reader
            Do While .Read()
                Dim strFileSizeHuman As String = Math.Round((!attach_file_size / 1024), 1) & " KB"
                strFullFilename = !attach_file_name &!attach_file_type
                ListView1.Items.Add(strFullFilename)
                ListView1.Items(ListView1.Items.Count - 1).SubItems.Add(strFileSizeHuman)
                ListView1.Items(ListView1.Items.Count - 1).SubItems.Add(!attach_upload_date)
                ReDim Preserve AttachIndex(row)
                AttachIndex(row).strFilename = !attach_file_name
                AttachIndex(row).strFileType = !attach_file_type
                AttachIndex(row).FileSize = !attach_file_size
                AttachIndex(row).strFileUID = !UID
                row += 1
            Loop
        End With
        CloseConnection(ConnID)
        ListView1.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent)
        DoneWaiting()
        Exit Sub
errs:
        If ErrHandle(Err.Number, Err.Description, System.Reflection.MethodInfo.GetCurrentMethod().Name) Then
            DoneWaiting()
            Resume Next
        Else
            EndProgram()
        End If
    End Sub
    Private Function GetUIDFromIndex(Index As Integer) As String
        Return AttachIndex(Index).strFileUID
    End Function
    Private Sub ListBox1_SelectedIndexChanged(sender As Object, e As EventArgs)
    End Sub
    Private Sub OpenAttachment(AttachUID As String)
        Waiting()
        Dim ConnID As String = Guid.NewGuid.ToString
        Dim reader As MySqlDataReader
        Dim table As New DataTable
        Dim strQry = "Select * FROM attachments WHERE UID='" & AttachUID & "'"
        Dim cmd As New MySqlCommand(strQry, GetConnection(ConnID).DBConnection)
        reader = cmd.ExecuteReader
        Try
            Dim conn As New MySqlConnection
            conn = GetConnection(ConnID).DBConnection
            Dim FileSize As UInt32
            Dim strFilename As String, strFiletype As String, strFullPath As String
            Dim di As DirectoryInfo = Directory.CreateDirectory(strTempPath)
            Dim rawData() As Byte
            With reader
                While .Read()
                    strFilename = !attach_file_name
                    strFiletype = !attach_file_type
                    strFullPath = strTempPath & strFilename & strFiletype
                    Debug.Print(strFullPath)
                    FileSize = !attach_file_size
                    rawData = New Byte(FileSize) {}
                    .GetBytes(.GetOrdinal("attach_file_binary"), 0, rawData, 0, FileSize)
                End While
            End With
            CloseConnection(ConnID)
            Dim fs As FileStream = New FileStream(strFullPath, FileMode.Create)
            fs.Write(rawData, 0, FileSize)
            fs.Close()
            Process.Start(strFullPath)
            DoneWaiting()
        Catch ex As Exception
            DoneWaiting()
            MessageBox.Show("There was an error: " & ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error)
        End Try
    End Sub
    Private Function DeleteAttachment(AttachUID As String) As Integer
        Try
            Waiting()
            Dim ConnID As String = Guid.NewGuid.ToString
            Dim cmd As New MySqlCommand
            Dim rows
            Dim strSQLQry As String = "DELETE FROM attachments WHERE UID='" & AttachUID & "'"
            cmd.Connection = GetConnection(ConnID).DBConnection
            cmd.CommandText = strSQLQry
            rows = cmd.ExecuteNonQuery()
            CloseConnection(ConnID)
            DoneWaiting()
            Return rows
            Exit Function
        Catch ex As Exception
            If ErrHandle(ex.HResult, ex.Message, System.Reflection.MethodInfo.GetCurrentMethod().Name) Then
                DoneWaiting()
                Exit Try
            Else
                EndProgram()
            End If
        End Try
        Return -1
    End Function
    Private Sub Waiting()
        Me.Cursor = Cursors.WaitCursor
    End Sub
    Private Sub DoneWaiting()
        Me.Cursor = Cursors.Default
    End Sub
    Private Sub Attachments_Load(sender As Object, e As EventArgs) Handles Me.Load
        Waiting()
        FillDeviceInfo()
        ListAttachments(CurrentDevice.strGUID)
        DoneWaiting()
    End Sub
    Private Sub FillDeviceInfo()
        txtAssetTag.Text = CurrentDevice.strAssetTag
        txtSerial.Text = CurrentDevice.strSerial
        txtDescription.Text = CurrentDevice.strDescription
    End Sub
    Private Sub ListView1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles ListView1.SelectedIndexChanged
    End Sub
    Private Sub ListView1_DoubleClick(sender As Object, e As EventArgs) Handles ListView1.DoubleClick
        OpenAttachment(GetUIDFromIndex(ListView1.FocusedItem.Index))
    End Sub
    Private Sub DeleteAttachmentToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteAttachmentToolStripMenuItem.Click
        If ListView1.FocusedItem IsNot Nothing Then
            StartAttachDelete()
        End If
    End Sub
    Private Sub StartAttachDelete()
        Dim strFilename As String
        strFilename = AttachIndex(ListView1.FocusedItem.Index).strFilename & AttachIndex(ListView1.FocusedItem.Index).strFileType
        Dim blah
        blah = MsgBox("Are you sure you want to delete '" & strFilename & "'?", vbYesNo + vbQuestion, "Confirm Delete")
        If blah = vbYes Then
            If DeleteAttachment(GetUIDFromIndex(ListView1.FocusedItem.Index)) > 0 Then
                blah = MsgBox("'" & strFilename & "' has been deleted.", vbOKOnly + vbInformation, "Deleted")
                ListAttachments(CurrentDevice.strGUID)
            Else
                blah = MsgBox("Deletion failed!", vbOKOnly + vbExclamation, "Unexpected Results")
            End If
        End If
    End Sub
    Private Sub RightClickMenu_Opening(sender As Object, e As System.ComponentModel.CancelEventArgs) Handles RightClickMenu.Opening
    End Sub
    Private Sub cmdDelete_Click(sender As Object, e As EventArgs) Handles cmdDelete.Click
        If ListView1.FocusedItem IsNot Nothing Then
            StartAttachDelete()
        End If
    End Sub
    Private Sub cmdOpen_Click(sender As Object, e As EventArgs) Handles cmdOpen.Click
        If ListView1.FocusedItem IsNot Nothing Then
            OpenAttachment(GetUIDFromIndex(ListView1.FocusedItem.Index))
        End If
    End Sub
End Class