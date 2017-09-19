﻿Option Explicit On

Imports System.IO
Imports MyDialogLib

Module OtherFunctions
    Public stpw As New Stopwatch
    Private intTimerHits As Integer = 0

    Public Sub StartTimer()
        stpw.Stop()
        stpw.Reset()
        stpw.Start()
    End Sub

    Public Function StopTimer() As String
        stpw.Stop()
        intTimerHits += 1
        Dim Results As String = intTimerHits & "  Stopwatch: MS:" & stpw.ElapsedMilliseconds & " Ticks: " & stpw.ElapsedTicks
        Debug.Print(Results)
        Return Results
    End Function

    Public Sub Logger(Message As String)
        Dim MaxLogSizeKiloBytes As Short = 500
        Dim DateStamp As String = DateTime.Now.ToString
        Dim infoReader As FileInfo
        infoReader = My.Computer.FileSystem.GetFileInfo(strLogPath)
        If Not File.Exists(strLogPath) Then
            Directory.CreateDirectory(strAppDir)
            Using sw As StreamWriter = File.CreateText(strLogPath)
                sw.WriteLine(DateStamp & ": Log Created...")
                sw.WriteLine(DateStamp & ": " & Message)
            End Using
        Else
            If (infoReader.Length / 1000) < MaxLogSizeKiloBytes Then
                Using sw As StreamWriter = File.AppendText(strLogPath)
                    sw.WriteLine(DateStamp & ": " & Message)
                End Using
            Else
                If RotateLogs() Then
                    Using sw As StreamWriter = File.AppendText(strLogPath)
                        sw.WriteLine(DateStamp & ": " & Message)
                    End Using
                End If
            End If
        End If
    End Sub

    Private Function RotateLogs() As Boolean
        Try
            File.Copy(strLogPath, strLogPath + ".old", True)
            File.Delete(strLogPath)
            Return True
        Catch ex As Exception
            Return False
        End Try
    End Function

    Public Sub EndProgram()
        GlobalSwitches.ProgramEnding = True
        Logger("Ending Program...")
        PurgeTempDir()
        Application.Exit()
    End Sub

    Public Sub PurgeTempDir()
        Try
            Directory.Delete(DownloadPath, True)
        Catch
        End Try
    End Sub

    Public Sub AdjustComboBoxWidth(ByVal sender As Object, ByVal e As EventArgs)
        Dim senderComboBox = DirectCast(sender, ComboBox)
        Dim width As Integer = senderComboBox.DropDownWidth
        Dim g As Graphics = senderComboBox.CreateGraphics()
        Dim font As Font = senderComboBox.Font
        Dim vertScrollBarWidth As Integer = If((senderComboBox.Items.Count > senderComboBox.MaxDropDownItems), SystemInformation.VerticalScrollBarWidth, 0)
        Dim newWidth As Integer
        For Each s As String In DirectCast(sender, ComboBox).Items
            newWidth = CInt(g.MeasureString(s, font).Width) + vertScrollBarWidth
            If width < newWidth Then
                width = newWidth
            End If
        Next
        senderComboBox.DropDownWidth = width
    End Sub

    Public Function NotePreview(Note As String, Optional CharLimit As Integer = 50) As String
        If Note <> "" Then
            Return Strings.Left(Note, CharLimit) & IIf(Len(Note) > CharLimit, "...", "").ToString
        Else
            Return ""
        End If
    End Function

    Public Function Message(ByVal Prompt As String, Optional ByVal Buttons As Integer = vbOKOnly + vbInformation, Optional ByVal Title As String = Nothing, Optional ByVal ParentFrm As Form = Nothing) As MsgBoxResult
        SetWaitCursor(False, ParentFrm)
        Dim NewMessage As New AdvancedDialog(ParentFrm)
        Return NewMessage.DialogMessage(Prompt, Buttons, Title, ParentFrm)
    End Function

    Public Function OKToEnd() As Boolean
        If GlobalSwitches.BuildingCache Then
            Message("Still building DB Cache. Please wait and try again.", vbOKOnly + vbInformation, "Critical Function Running")
            Return False
        End If
        If GKUpdaterForm.Visible AndAlso Not GKUpdaterForm.OkToClose() Then Return False
        Return True
    End Function

    Public Sub SetWaitCursor(Waiting As Boolean, Optional parentForm As Form = Nothing)
        If parentForm Is Nothing Then
            Application.UseWaitCursor = Waiting
            Application.DoEvents()
        Else
            If Waiting Then
                parentForm.Cursor = Cursors.AppStarting
            Else
                parentForm.Cursor = Cursors.Default
            End If
            parentForm.Update()
        End If

    End Sub

End Module