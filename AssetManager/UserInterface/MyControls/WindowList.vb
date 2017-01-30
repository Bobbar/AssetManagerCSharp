﻿Public Class WindowList
    Private WithEvents RefreshTimer As Timer
    Private MyParentForm As Form
    Private DropDownControl As ToolStripDropDownButton
    Private intFormCount As Integer
    Sub New(ParentForm As Form, DropDownCtl As ToolStripDropDownButton)
        MyParentForm = ParentForm
        DropDownControl = DropDownCtl
        DropDownControl.Visible = False
        Init()
    End Sub
    Private Sub Init()
        InitializeTimer()
    End Sub
    Private Sub InitializeTimer()
        RefreshTimer = New Timer
        RefreshTimer.Interval = 200
        RefreshTimer.Enabled = True
        AddHandler RefreshTimer.Tick, AddressOf RefreshTimer_Tick
    End Sub
    Private Sub RefreshTimer_Tick(sender As Object, e As EventArgs) Handles RefreshTimer.Tick
        If FormCount(MyParentForm) < 1 Then
            DropDownControl.Visible = False
        Else
            DropDownControl.Visible = True
        End If
        If FormCount(MyParentForm) <> intFormCount Then
            If Not DropDownControl.DropDown.Focused Then
                DropDownControl.DropDownItems.Clear()
                BuildWindowList(MyParentForm, DropDownControl.DropDownItems)
                intFormCount = FormCount(MyParentForm)
                DropDownControl.Text = CountText(intFormCount)
            End If
        End If
    End Sub
    Private Function FormCount(ParentForm As Form) As Integer
        Dim i As Integer = 0
        For Each frm As Form In My.Application.OpenForms
            If Not frm.IsDisposed And Not frm.Modal And frm IsNot ParentForm Then
                If frm.Tag Is ParentForm Then
                    i += 1
                    i += FormCount(frm)
                End If
            End If
        Next
        Return i
    End Function
    ''' <summary>
    ''' Recursively build ToolStripItemCollections of Forms and their Children and add them to the ToolStrip. Making sure to add SibiMain to the top of the list.
    ''' </summary>
    ''' <param name="ParentForm">Form to add to ToolStrip.</param>
    ''' <param name="TargetMenuItem">Item to add the Form item to.</param>
    Private Sub BuildWindowList(ParentForm As Form, ByRef TargetMenuItem As ToolStripItemCollection)
        For Each frm As Form In ListOfChilden(ParentForm)
            If HasChildren(frm) Then
                Dim NewDropDown As ToolStripMenuItem = NewMenuItem(frm)
                If TypeOf frm Is frmSibiMain Then
                    TargetMenuItem.Insert(0, NewDropDown)
                Else
                    TargetMenuItem.Add(NewDropDown)
                End If
                BuildWindowList(frm, NewDropDown.DropDownItems)
            Else
                If TypeOf frm Is frmSibiMain Then
                    TargetMenuItem.Insert(0, NewMenuItem(frm))
                Else
                    TargetMenuItem.Add(NewMenuItem(frm))
                End If
            End If
        Next
    End Sub
    Private Function NewMenuItem(frm As Form) As ToolStripMenuItem
        Dim newitem As New ToolStripMenuItem
        newitem.Text = frm.Text
        newitem.Image = frm.Icon.ToBitmap
        newitem.Tag = frm
        newitem.ToolTipText = "Right-Click to close."
        AddHandler newitem.MouseDown, AddressOf WindowClick
        Return newitem
    End Function
    Private Function HasChildren(ParentForm As Form) As Boolean
        For Each frm As Form In My.Application.OpenForms
            If frm.Tag Is ParentForm And Not frm.IsDisposed Then
                Return True
            End If
        Next
        Return False
    End Function
    Private Function ListOfChilden(ParentForm As Form) As List(Of Form)
        Dim tmpList As New List(Of Form)
        For Each frm As Form In My.Application.OpenForms
            If frm.Tag Is ParentForm And Not frm.IsDisposed Then
                tmpList.Add(frm)
            End If
        Next
        Return tmpList
    End Function
    Private Sub WindowClick(sender As Object, e As MouseEventArgs)
        Dim item As ToolStripItem = CType(sender, ToolStripItem)
        If e.Button = MouseButtons.Right Then
            Dim frm As Form = CType(item.Tag, Form)
            frm.Dispose()
            GC.Collect()
            If DropDownControl.DropDownItems.Count < 1 Then
                DropDownControl.Visible = False
                DropDownControl.DropDownItems.Clear()
                item.Dispose()
            Else
                item.Visible = False
                item.Dispose()
                DropDownControl.DropDownItems.Remove(item)
                intFormCount = FormCount(MyParentForm)
                DropDownControl.Text = CountText(intFormCount)
            End If
        ElseIf e.Button = MouseButtons.Left Then
            ActivateFormByHandle(CType(item.Tag, Form))
        End If
    End Sub
    Private Function CountText(count As Integer) As String
        Dim MainText As String = "Select Window"
        If count > 0 Then
            Return MainText & " (" & count & ")"
        Else
            Return MainText
        End If
    End Function
End Class
