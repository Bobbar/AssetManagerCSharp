﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class View
    Inherits System.Windows.Forms.Form
    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()>
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub
    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer
    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()>
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(View))
        Me.MenuStrip1 = New System.Windows.Forms.MenuStrip()
        Me.ActionsToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.EditToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.AddNoteToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.DeleteDeviceToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripMenuItem1 = New System.Windows.Forms.ToolStripSeparator()
        Me.TrackingToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckInToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckOutToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.pnlViewControls = New System.Windows.Forms.Panel()
        Me.chkTrackable = New System.Windows.Forms.CheckBox()
        Me.cmdCancel = New System.Windows.Forms.Button()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtGUID = New System.Windows.Forms.TextBox()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.cmbStatus_REQ = New System.Windows.Forms.ComboBox()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.cmbOSVersion_REQ = New System.Windows.Forms.ComboBox()
        Me.cmdUpdate = New System.Windows.Forms.Button()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.cmbEquipType_View_REQ = New System.Windows.Forms.ComboBox()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.txtReplacementYear_View = New System.Windows.Forms.TextBox()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.dtPurchaseDate_View_REQ = New System.Windows.Forms.DateTimePicker()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cmbLocation_View_REQ = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.txtDescription_View_REQ = New System.Windows.Forms.TextBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.txtCurUser_View_REQ = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.txtSerial_View_REQ = New System.Windows.Forms.TextBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtAssetTag_View_REQ = New System.Windows.Forms.TextBox()
        Me.RightClickMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteEntryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.HistoryTab = New System.Windows.Forms.TabPage()
        Me.DataGridHistory = New System.Windows.Forms.DataGridView()
        Me.TrackingTab = New System.Windows.Forms.TabPage()
        Me.TrackingGrid = New System.Windows.Forms.DataGridView()
        Me.MenuStrip1.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.pnlViewControls.SuspendLayout()
        Me.RightClickMenu.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.HistoryTab.SuspendLayout()
        CType(Me.DataGridHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TrackingTab.SuspendLayout()
        CType(Me.TrackingGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'MenuStrip1
        '
        Me.MenuStrip1.BackColor = System.Drawing.SystemColors.Control
        Me.MenuStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ActionsToolStripMenuItem})
        Me.MenuStrip1.Location = New System.Drawing.Point(0, 0)
        Me.MenuStrip1.Name = "MenuStrip1"
        Me.MenuStrip1.Size = New System.Drawing.Size(992, 24)
        Me.MenuStrip1.TabIndex = 36
        Me.MenuStrip1.Text = "MenuStrip1"
        '
        'ActionsToolStripMenuItem
        '
        Me.ActionsToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.EditToolStripMenuItem, Me.AddNoteToolStripMenuItem, Me.DeleteDeviceToolStripMenuItem, Me.ToolStripMenuItem1, Me.TrackingToolStripMenuItem})
        Me.ActionsToolStripMenuItem.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ActionsToolStripMenuItem.Name = "ActionsToolStripMenuItem"
        Me.ActionsToolStripMenuItem.Size = New System.Drawing.Size(60, 20)
        Me.ActionsToolStripMenuItem.Text = "Actions"
        '
        'EditToolStripMenuItem
        '
        Me.EditToolStripMenuItem.Image = Global.AssetManager.My.Resources.Resources.Edit
        Me.EditToolStripMenuItem.Name = "EditToolStripMenuItem"
        Me.EditToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.EditToolStripMenuItem.Text = "Modify Device"
        '
        'AddNoteToolStripMenuItem
        '
        Me.AddNoteToolStripMenuItem.Image = Global.AssetManager.My.Resources.Resources.Add
        Me.AddNoteToolStripMenuItem.Name = "AddNoteToolStripMenuItem"
        Me.AddNoteToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.AddNoteToolStripMenuItem.Text = "Add Note"
        '
        'DeleteDeviceToolStripMenuItem
        '
        Me.DeleteDeviceToolStripMenuItem.Image = Global.AssetManager.My.Resources.Resources.delete_icon
        Me.DeleteDeviceToolStripMenuItem.Name = "DeleteDeviceToolStripMenuItem"
        Me.DeleteDeviceToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.DeleteDeviceToolStripMenuItem.Text = "Delete Device"
        '
        'ToolStripMenuItem1
        '
        Me.ToolStripMenuItem1.Name = "ToolStripMenuItem1"
        Me.ToolStripMenuItem1.Size = New System.Drawing.Size(152, 6)
        '
        'TrackingToolStripMenuItem
        '
        Me.TrackingToolStripMenuItem.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CheckInToolStripMenuItem, Me.CheckOutToolStripMenuItem})
        Me.TrackingToolStripMenuItem.Image = Global.AssetManager.My.Resources.Resources.check_out
        Me.TrackingToolStripMenuItem.Name = "TrackingToolStripMenuItem"
        Me.TrackingToolStripMenuItem.Size = New System.Drawing.Size(155, 22)
        Me.TrackingToolStripMenuItem.Text = "Tracking"
        '
        'CheckInToolStripMenuItem
        '
        Me.CheckInToolStripMenuItem.Name = "CheckInToolStripMenuItem"
        Me.CheckInToolStripMenuItem.Size = New System.Drawing.Size(132, 22)
        Me.CheckInToolStripMenuItem.Text = "Check In"
        '
        'CheckOutToolStripMenuItem
        '
        Me.CheckOutToolStripMenuItem.Name = "CheckOutToolStripMenuItem"
        Me.CheckOutToolStripMenuItem.Size = New System.Drawing.Size(132, 22)
        Me.CheckOutToolStripMenuItem.Text = "Check Out"
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.pnlViewControls)
        Me.GroupBox1.Location = New System.Drawing.Point(12, 27)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(965, 209)
        Me.GroupBox1.TabIndex = 39
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Current Info"
        '
        'pnlViewControls
        '
        Me.pnlViewControls.Controls.Add(Me.chkTrackable)
        Me.pnlViewControls.Controls.Add(Me.cmdCancel)
        Me.pnlViewControls.Controls.Add(Me.Label10)
        Me.pnlViewControls.Controls.Add(Me.txtGUID)
        Me.pnlViewControls.Controls.Add(Me.Label9)
        Me.pnlViewControls.Controls.Add(Me.cmbStatus_REQ)
        Me.pnlViewControls.Controls.Add(Me.Label8)
        Me.pnlViewControls.Controls.Add(Me.cmbOSVersion_REQ)
        Me.pnlViewControls.Controls.Add(Me.cmdUpdate)
        Me.pnlViewControls.Controls.Add(Me.Label13)
        Me.pnlViewControls.Controls.Add(Me.cmbEquipType_View_REQ)
        Me.pnlViewControls.Controls.Add(Me.Label7)
        Me.pnlViewControls.Controls.Add(Me.txtReplacementYear_View)
        Me.pnlViewControls.Controls.Add(Me.Label6)
        Me.pnlViewControls.Controls.Add(Me.dtPurchaseDate_View_REQ)
        Me.pnlViewControls.Controls.Add(Me.Label5)
        Me.pnlViewControls.Controls.Add(Me.cmbLocation_View_REQ)
        Me.pnlViewControls.Controls.Add(Me.Label4)
        Me.pnlViewControls.Controls.Add(Me.txtDescription_View_REQ)
        Me.pnlViewControls.Controls.Add(Me.Label3)
        Me.pnlViewControls.Controls.Add(Me.txtCurUser_View_REQ)
        Me.pnlViewControls.Controls.Add(Me.Label2)
        Me.pnlViewControls.Controls.Add(Me.txtSerial_View_REQ)
        Me.pnlViewControls.Controls.Add(Me.Label1)
        Me.pnlViewControls.Controls.Add(Me.txtAssetTag_View_REQ)
        Me.pnlViewControls.Location = New System.Drawing.Point(13, 23)
        Me.pnlViewControls.Name = "pnlViewControls"
        Me.pnlViewControls.Size = New System.Drawing.Size(932, 180)
        Me.pnlViewControls.TabIndex = 38
        '
        'chkTrackable
        '
        Me.chkTrackable.AutoSize = True
        Me.chkTrackable.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkTrackable.Location = New System.Drawing.Point(516, 122)
        Me.chkTrackable.Name = "chkTrackable"
        Me.chkTrackable.Size = New System.Drawing.Size(89, 20)
        Me.chkTrackable.TabIndex = 45
        Me.chkTrackable.Text = "Trackable"
        Me.chkTrackable.UseVisualStyleBackColor = True
        '
        'cmdCancel
        '
        Me.cmdCancel.Location = New System.Drawing.Point(759, 142)
        Me.cmdCancel.Name = "cmdCancel"
        Me.cmdCancel.Size = New System.Drawing.Size(169, 24)
        Me.cmdCancel.TabIndex = 42
        Me.cmdCancel.Text = "Cancel"
        Me.cmdCancel.UseVisualStyleBackColor = True
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(658, 5)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(90, 16)
        Me.Label10.TabIndex = 41
        Me.Label10.Text = "Device GUID:"
        '
        'txtGUID
        '
        Me.txtGUID.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtGUID.Location = New System.Drawing.Point(661, 24)
        Me.txtGUID.Name = "txtGUID"
        Me.txtGUID.ReadOnly = True
        Me.txtGUID.Size = New System.Drawing.Size(268, 22)
        Me.txtGUID.TabIndex = 40
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(489, 56)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(48, 16)
        Me.Label9.TabIndex = 39
        Me.Label9.Text = "Status:"
        '
        'cmbStatus_REQ
        '
        Me.cmbStatus_REQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbStatus_REQ.FormattingEnabled = True
        Me.cmbStatus_REQ.Location = New System.Drawing.Point(492, 75)
        Me.cmbStatus_REQ.Name = "cmbStatus_REQ"
        Me.cmbStatus_REQ.Size = New System.Drawing.Size(124, 24)
        Me.cmbStatus_REQ.TabIndex = 38
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(349, 56)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(79, 16)
        Me.Label8.TabIndex = 37
        Me.Label8.Text = "OS Version:"
        '
        'cmbOSVersion_REQ
        '
        Me.cmbOSVersion_REQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbOSVersion_REQ.FormattingEnabled = True
        Me.cmbOSVersion_REQ.Location = New System.Drawing.Point(352, 75)
        Me.cmbOSVersion_REQ.Name = "cmbOSVersion_REQ"
        Me.cmbOSVersion_REQ.Size = New System.Drawing.Size(120, 24)
        Me.cmbOSVersion_REQ.TabIndex = 36
        '
        'cmdUpdate
        '
        Me.cmdUpdate.BackColor = System.Drawing.Color.FromArgb(CType(CType(149, Byte), Integer), CType(CType(255, Byte), Integer), CType(CType(191, Byte), Integer))
        Me.cmdUpdate.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdUpdate.Location = New System.Drawing.Point(755, 81)
        Me.cmdUpdate.Name = "cmdUpdate"
        Me.cmdUpdate.Size = New System.Drawing.Size(174, 47)
        Me.cmdUpdate.TabIndex = 35
        Me.cmdUpdate.Text = "Confirm Modification"
        Me.cmdUpdate.UseVisualStyleBackColor = False
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(478, 5)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(110, 16)
        Me.Label13.TabIndex = 34
        Me.Label13.Text = "Equipment Type:"
        '
        'cmbEquipType_View_REQ
        '
        Me.cmbEquipType_View_REQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbEquipType_View_REQ.FormattingEnabled = True
        Me.cmbEquipType_View_REQ.Location = New System.Drawing.Point(481, 24)
        Me.cmbEquipType_View_REQ.Name = "cmbEquipType_View_REQ"
        Me.cmbEquipType_View_REQ.Size = New System.Drawing.Size(156, 24)
        Me.cmbEquipType_View_REQ.TabIndex = 33
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(363, 112)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(124, 16)
        Me.Label7.TabIndex = 32
        Me.Label7.Text = "Replacement Year:"
        '
        'txtReplacementYear_View
        '
        Me.txtReplacementYear_View.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtReplacementYear_View.Location = New System.Drawing.Point(376, 131)
        Me.txtReplacementYear_View.Name = "txtReplacementYear_View"
        Me.txtReplacementYear_View.Size = New System.Drawing.Size(66, 22)
        Me.txtReplacementYear_View.TabIndex = 31
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(156, 109)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(100, 16)
        Me.Label6.TabIndex = 30
        Me.Label6.Text = "Purchase Date:"
        '
        'dtPurchaseDate_View_REQ
        '
        Me.dtPurchaseDate_View_REQ.CustomFormat = "yyyy-MM-dd"
        Me.dtPurchaseDate_View_REQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtPurchaseDate_View_REQ.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtPurchaseDate_View_REQ.Location = New System.Drawing.Point(160, 128)
        Me.dtPurchaseDate_View_REQ.Name = "dtPurchaseDate_View_REQ"
        Me.dtPurchaseDate_View_REQ.Size = New System.Drawing.Size(182, 22)
        Me.dtPurchaseDate_View_REQ.TabIndex = 29
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(157, 56)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(62, 16)
        Me.Label5.TabIndex = 28
        Me.Label5.Text = "Location:"
        '
        'cmbLocation_View_REQ
        '
        Me.cmbLocation_View_REQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbLocation_View_REQ.FormattingEnabled = True
        Me.cmbLocation_View_REQ.Location = New System.Drawing.Point(158, 75)
        Me.cmbLocation_View_REQ.Name = "cmbLocation_View_REQ"
        Me.cmbLocation_View_REQ.Size = New System.Drawing.Size(168, 24)
        Me.cmbLocation_View_REQ.TabIndex = 27
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(157, 6)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(79, 16)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "Description:"
        '
        'txtDescription_View_REQ
        '
        Me.txtDescription_View_REQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDescription_View_REQ.Location = New System.Drawing.Point(158, 25)
        Me.txtDescription_View_REQ.Name = "txtDescription_View_REQ"
        Me.txtDescription_View_REQ.Size = New System.Drawing.Size(304, 22)
        Me.txtDescription_View_REQ.TabIndex = 25
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(2, 109)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(85, 16)
        Me.Label3.TabIndex = 24
        Me.Label3.Text = "Current User:"
        '
        'txtCurUser_View_REQ
        '
        Me.txtCurUser_View_REQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCurUser_View_REQ.Location = New System.Drawing.Point(6, 128)
        Me.txtCurUser_View_REQ.Name = "txtCurUser_View_REQ"
        Me.txtCurUser_View_REQ.Size = New System.Drawing.Size(132, 22)
        Me.txtCurUser_View_REQ.TabIndex = 23
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(3, 57)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(46, 16)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Serial:"
        '
        'txtSerial_View_REQ
        '
        Me.txtSerial_View_REQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSerial_View_REQ.Location = New System.Drawing.Point(6, 76)
        Me.txtSerial_View_REQ.Name = "txtSerial_View_REQ"
        Me.txtSerial_View_REQ.Size = New System.Drawing.Size(133, 22)
        Me.txtSerial_View_REQ.TabIndex = 21
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(3, 6)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 16)
        Me.Label1.TabIndex = 20
        Me.Label1.Text = "Asset Tag:"
        '
        'txtAssetTag_View_REQ
        '
        Me.txtAssetTag_View_REQ.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAssetTag_View_REQ.Location = New System.Drawing.Point(6, 25)
        Me.txtAssetTag_View_REQ.Name = "txtAssetTag_View_REQ"
        Me.txtAssetTag_View_REQ.Size = New System.Drawing.Size(134, 22)
        Me.txtAssetTag_View_REQ.TabIndex = 19
        '
        'RightClickMenu
        '
        Me.RightClickMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteEntryToolStripMenuItem})
        Me.RightClickMenu.Name = "RightClickMenu"
        Me.RightClickMenu.Size = New System.Drawing.Size(138, 26)
        '
        'DeleteEntryToolStripMenuItem
        '
        Me.DeleteEntryToolStripMenuItem.Image = Global.AssetManager.My.Resources.Resources.delete_icon
        Me.DeleteEntryToolStripMenuItem.Name = "DeleteEntryToolStripMenuItem"
        Me.DeleteEntryToolStripMenuItem.Size = New System.Drawing.Size(137, 22)
        Me.DeleteEntryToolStripMenuItem.Text = "Delete Entry"
        '
        'TabControl1
        '
        Me.TabControl1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TabControl1.Controls.Add(Me.HistoryTab)
        Me.TabControl1.Controls.Add(Me.TrackingTab)
        Me.TabControl1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControl1.ItemSize = New System.Drawing.Size(61, 21)
        Me.TabControl1.Location = New System.Drawing.Point(12, 242)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(967, 296)
        Me.TabControl1.TabIndex = 40
        '
        'HistoryTab
        '
        Me.HistoryTab.Controls.Add(Me.DataGridHistory)
        Me.HistoryTab.Location = New System.Drawing.Point(4, 25)
        Me.HistoryTab.Name = "HistoryTab"
        Me.HistoryTab.Padding = New System.Windows.Forms.Padding(3)
        Me.HistoryTab.Size = New System.Drawing.Size(959, 267)
        Me.HistoryTab.TabIndex = 0
        Me.HistoryTab.Text = "History"
        Me.HistoryTab.UseVisualStyleBackColor = True
        '
        'DataGridHistory
        '
        Me.DataGridHistory.AllowUserToAddRows = False
        Me.DataGridHistory.AllowUserToDeleteRows = False
        Me.DataGridHistory.AllowUserToResizeRows = False
        Me.DataGridHistory.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridHistory.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.DataGridHistory.ContextMenuStrip = Me.RightClickMenu
        Me.DataGridHistory.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.DataGridHistory.Location = New System.Drawing.Point(6, 6)
        Me.DataGridHistory.MultiSelect = False
        Me.DataGridHistory.Name = "DataGridHistory"
        Me.DataGridHistory.ReadOnly = True
        Me.DataGridHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.DataGridHistory.ShowCellToolTips = False
        Me.DataGridHistory.ShowEditingIcon = False
        Me.DataGridHistory.Size = New System.Drawing.Size(947, 255)
        Me.DataGridHistory.TabIndex = 40
        '
        'TrackingTab
        '
        Me.TrackingTab.Controls.Add(Me.TrackingGrid)
        Me.TrackingTab.Location = New System.Drawing.Point(4, 25)
        Me.TrackingTab.Name = "TrackingTab"
        Me.TrackingTab.Padding = New System.Windows.Forms.Padding(3)
        Me.TrackingTab.Size = New System.Drawing.Size(975, 435)
        Me.TrackingTab.TabIndex = 1
        Me.TrackingTab.Text = "Tracking"
        Me.TrackingTab.UseVisualStyleBackColor = True
        '
        'TrackingGrid
        '
        Me.TrackingGrid.AllowUserToAddRows = False
        Me.TrackingGrid.AllowUserToDeleteRows = False
        Me.TrackingGrid.AllowUserToResizeRows = False
        Me.TrackingGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TrackingGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.TrackingGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.TrackingGrid.ContextMenuStrip = Me.RightClickMenu
        Me.TrackingGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.TrackingGrid.Location = New System.Drawing.Point(6, 6)
        Me.TrackingGrid.MultiSelect = False
        Me.TrackingGrid.Name = "TrackingGrid"
        Me.TrackingGrid.ReadOnly = True
        Me.TrackingGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.TrackingGrid.ShowCellToolTips = False
        Me.TrackingGrid.ShowEditingIcon = False
        Me.TrackingGrid.Size = New System.Drawing.Size(963, 423)
        Me.TrackingGrid.TabIndex = 41
        '
        'View
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(992, 547)
        Me.Controls.Add(Me.TabControl1)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.MenuStrip1)
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MainMenuStrip = Me.MenuStrip1
        Me.MinimumSize = New System.Drawing.Size(1008, 400)
        Me.Name = "View"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "View"
        Me.MenuStrip1.ResumeLayout(False)
        Me.MenuStrip1.PerformLayout()
        Me.GroupBox1.ResumeLayout(False)
        Me.pnlViewControls.ResumeLayout(False)
        Me.pnlViewControls.PerformLayout()
        Me.RightClickMenu.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.HistoryTab.ResumeLayout(False)
        CType(Me.DataGridHistory, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TrackingTab.ResumeLayout(False)
        CType(Me.TrackingGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)
        Me.PerformLayout()
    End Sub
    Friend WithEvents MenuStrip1 As MenuStrip
    Friend WithEvents ActionsToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents EditToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents AddNoteToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents pnlViewControls As Panel
    Friend WithEvents Label9 As Label
    Friend WithEvents cmbStatus_REQ As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents cmbOSVersion_REQ As ComboBox
    Friend WithEvents cmdUpdate As Button
    Friend WithEvents Label13 As Label
    Friend WithEvents cmbEquipType_View_REQ As ComboBox
    Friend WithEvents Label7 As Label
    Friend WithEvents txtReplacementYear_View As TextBox
    Friend WithEvents Label6 As Label
    Friend WithEvents dtPurchaseDate_View_REQ As DateTimePicker
    Friend WithEvents Label5 As Label
    Friend WithEvents cmbLocation_View_REQ As ComboBox
    Friend WithEvents Label4 As Label
    Friend WithEvents txtDescription_View_REQ As TextBox
    Friend WithEvents Label3 As Label
    Friend WithEvents txtCurUser_View_REQ As TextBox
    Friend WithEvents Label2 As Label
    Friend WithEvents txtSerial_View_REQ As TextBox
    Friend WithEvents Label1 As Label
    Friend WithEvents txtAssetTag_View_REQ As TextBox
    Friend WithEvents Label10 As Label
    Friend WithEvents txtGUID As TextBox
    Friend WithEvents DeleteDeviceToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents cmdCancel As Button
    Friend WithEvents RightClickMenu As ContextMenuStrip
    Friend WithEvents DeleteEntryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents ToolStripMenuItem1 As ToolStripSeparator
    Friend WithEvents TrackingToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents chkTrackable As CheckBox
    Friend WithEvents CheckInToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents CheckOutToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents HistoryTab As TabPage
    Friend WithEvents DataGridHistory As DataGridView
    Friend WithEvents TrackingTab As TabPage
    Friend WithEvents TrackingGrid As DataGridView
End Class
