﻿Imports System.ComponentModel

Public Class GKUpdaterForm
    Private bolRunQueue As Boolean = True
    Private MaxSimUpdates As Integer = 4
    Private MyUpdates As New List(Of GKProgressControl)
    Private bolStarting As Boolean = True
    Private bolCheckForDups As Boolean = True
    Private bolCreateMissingDirs As Boolean = False
    Private PackFileReady As Boolean = False
    Private PackFunc As New ManagePackFile

    Sub New()

        ' This call is required by the designer.
        InitializeComponent()
        bolStarting = False
        MaxUpdates.Value = MaxSimUpdates
        ' Add any initialization after the InitializeComponent() call.

        DoubleBufferedFlowLayout(Updater_Table, True)

    End Sub

    Public Async Sub AddUpdate(ByVal Device As Device_Info)

        If bolCheckForDups AndAlso Not Exists(Device) Then
            Await CheckPackFile()
            Dim NewProgCtl As New GKProgressControl(Me, Device, bolCreateMissingDirs, GKExtractDir, MyUpdates.Count + 1)
            Updater_Table.Controls.Add(NewProgCtl)
            MyUpdates.Add(NewProgCtl)
            AddHandler NewProgCtl.CriticalStopError, AddressOf CriticalStop
            ProcessUpdates()
            Me.WindowState = FormWindowState.Normal
            Me.Activate()
        Else
            Dim blah = Message("An update for device " & Device.strSerial & " already exists.  Do you want to restart the update for this device?", vbOKCancel + vbExclamation, "Duplicate Update", Me)
            If blah = vbOK Then
                Await CheckPackFile()
                StartUpdateByDevice(Device)
                Me.WindowState = FormWindowState.Normal
                Me.Activate()
            End If
        End If

    End Sub

    Private Sub StartUpdateByDevice(Device As Device_Info)
        MyUpdates.Find(Function(upd) upd.Device.strGUID = Device.strGUID).StartUpdate()
    End Sub

    Private Function Exists(Device As Device_Info) As Boolean
        Return MyUpdates.Exists(Function(upd) upd.Device.strGUID = Device.strGUID)
    End Function

    Public Function ActiveUpdates() As Boolean
        Return MyUpdates.Exists(Function(upd) upd.ProgStatus = GKProgressControl.Progress_Status.Running)
    End Function

    Private Sub CancelAll()
        For Each upd As GKProgressControl In MyUpdates
            If upd.ProgStatus = GKProgressControl.Progress_Status.Running Then
                upd.CancelUpdate()
            End If
        Next
    End Sub

    Private Sub DisposeUpdates()
        For Each upd As GKProgressControl In MyUpdates
            If Not upd.IsDisposed Then
                upd.Dispose()
            End If
        Next
    End Sub

    ''' <summary>
    ''' Returns True if number of running updates is less than the maximum simultaneous allowed updates and RunQueue is True.
    ''' </summary>
    ''' <returns></returns>
    Private Function CanRunMoreUpdates() As Boolean
        If bolRunQueue Then
            Dim RunningUpdates As Integer = 0
            For Each upd As GKProgressControl In MyUpdates
                Select Case upd.ProgStatus
                    Case GKProgressControl.Progress_Status.Running, GKProgressControl.Progress_Status.Starting, GKProgressControl.Progress_Status.Paused
                        If Not upd.IsDisposed Then RunningUpdates += 1
                End Select
            Next
            If RunningUpdates < MaxSimUpdates Then Return True
        End If
        Return False
    End Function

    Private Sub cmdCancelAll_Click(sender As Object, e As EventArgs) Handles cmdCancelAll.Click
        CancelAll()
        StopQueue()
    End Sub

    Private Sub cmdPauseResume_Click(sender As Object, e As EventArgs) Handles cmdPauseResume.Click
        If bolRunQueue Then
            StopQueue()
        Else
            StartQueue()
        End If
    End Sub

    Private Sub cmdSort_Click(sender As Object, e As EventArgs) Handles cmdSort.Click
        SortUpdates()
    End Sub

    Private Sub CriticalStop(sender As Object, e As EventArgs)
        StopQueue()
        Message("The queue was stopped because of an access error. Please re-enter your credentials.", vbExclamation + vbOKOnly, "Queue Stopped", Me)
        AdminCreds = Nothing
        If VerifyAdminCreds() Then
            bolRunQueue = True
        End If
    End Sub

    Private Sub GKUpdater_Form_Closing(sender As Object, e As CancelEventArgs) Handles Me.Closing
        If Not OKToClose() Then
            e.Cancel = True
        Else
            DisposeUpdates()
            Me.Dispose()
        End If
    End Sub
    Public Function OKToClose() As Boolean
        If ActiveUpdates() Then
            Message("There are still updates running!  Cancel the updates or wait for them to finish.", vbOKOnly + vbExclamation, "Close Aborted", Me)
            Me.Activate()
            Me.WindowState = FormWindowState.Normal
            Return False
        End If
        Return True
    End Function

    Private Sub MaxUpdates_ValueChanged(sender As Object, e As EventArgs) Handles MaxUpdates.ValueChanged
        If Not bolStarting Then MaxSimUpdates = CInt(MaxUpdates.Value)
    End Sub

    Private Sub ProcessUpdates()
        If CanRunMoreUpdates() Then
            StartNextUpdate()
        End If
        PruneQueue()
        SetStats()
    End Sub

    ''' <summary>
    ''' Removes disposed update fragments from list.
    ''' </summary>
    Private Sub PruneQueue()
        MyUpdates = MyUpdates.FindAll(Function(upd) Not upd.IsDisposed)
    End Sub

    Private Sub QueueChecker_Tick(sender As Object, e As EventArgs) Handles QueueChecker.Tick
        ProcessUpdates()
    End Sub

    Private Sub SetStats()
        Dim intQueued, intRunning, intComplete As Integer
        Dim TransferRateSum As Double
        For Each upd As GKProgressControl In MyUpdates
            Select Case upd.ProgStatus
                Case GKProgressControl.Progress_Status.Queued
                    intQueued += 1
                Case GKProgressControl.Progress_Status.Running
                    TransferRateSum += upd.MyUpdater.UpdateStatus.CurTransferRate
                    intRunning += 1
                Case GKProgressControl.Progress_Status.Complete
                    intComplete += 1
            End Select
        Next
        lblQueued.Text = "Queued: " & intQueued
        lblRunning.Text = "Running: " & intRunning
        lblComplete.Text = "Complete: " & intComplete
        lblTotUpdates.Text = "Tot Updates: " & MyUpdates.Count
        lblTransferRate.Text = "Transfer Rate: " & TransferRateSum.ToString("0.00") & " MB/s"
    End Sub

    ''' <summary>
    ''' Sorts all the GKProgressControls in order of the Progress_Status enum.
    ''' </summary>
    Private Sub SortUpdates()
        Dim sortUpdates As New List(Of GKProgressControl)
        For Each status As GKProgressControl.Progress_Status In [Enum].GetValues(GetType(GKProgressControl.Progress_Status))
            sortUpdates.AddRange(MyUpdates.FindAll(Function(upd) upd.ProgStatus = status))
        Next
        Updater_Table.Controls.Clear()
        Updater_Table.Controls.AddRange(sortUpdates.ToArray)
    End Sub

    ''' <summary>
    ''' Starts the next update that has a queued status.
    ''' </summary>
    Private Sub StartNextUpdate()
        Dim NextUpd = MyUpdates.Find(Function(upd) upd.ProgStatus = GKProgressControl.Progress_Status.Queued)
        If NextUpd IsNot Nothing Then NextUpd.StartUpdate()
    End Sub

    Private Sub StartQueue()
        bolRunQueue = True
        SetQueueButton()
    End Sub

    Private Sub StopQueue()
        bolRunQueue = False
        SetQueueButton()
    End Sub
    Private Sub SetQueueButton()
        If bolRunQueue Then
            cmdPauseResume.Text = "Pause Queue"
        Else
            cmdPauseResume.Text = "Resume Queue"
        End If
    End Sub

    Private Sub tsmCreateDirs_Click(sender As Object, e As EventArgs) Handles tsmCreateDirs.Click
        bolCreateMissingDirs = tsmCreateDirs.Checked
    End Sub

    Private Sub GKUpdaterForm_Shown(sender As Object, e As EventArgs) Handles Me.Shown
        SetQueueButton()
    End Sub

    Private Sub ProcessPackFile()
        Using NewUnPack As New PackFileForm(False)
            NewUnPack.ShowDialog(Me)
            PackFileReady = NewUnPack.PackVerified
            bolRunQueue = PackFileReady
            SetQueueButton()
        End Using
    End Sub

    Private Async Function CheckPackFile() As Task(Of Boolean)
        PackFileReady = Await PackFunc.VerifyPackFile
        If bolRunQueue And Not PackFileReady Then bolRunQueue = False
        SetQueueButton()
        If Not PackFileReady Then
            CancelAll()
            Message("The local pack file does not match the server. All running updates will be stopped and a new copy will now be downloaded and unpacked.", vbOKOnly + vbExclamation, "Pack file out of date", Me)
            ProcessPackFile()
        End If
        Return True
    End Function

    Private Sub GKPackageVeriToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles GKPackageVeriToolStripMenuItem.Click
        If Not FormTypeIsOpen(GetType(PackFileForm)) Then
            Dim NewUnPack As New PackFileForm(True)
            NewUnPack.Show()
        End If
    End Sub

End Class