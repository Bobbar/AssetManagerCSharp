﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()>
Partial Class ViewDeviceForm
    Inherits ThemedForm
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
        Dim DataGridViewCellStyle1 As System.Windows.Forms.DataGridViewCellStyle = New System.Windows.Forms.DataGridViewCellStyle()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(ViewDeviceForm))
        Me.txtHostname = New System.Windows.Forms.TextBox()
        Me.Label15 = New System.Windows.Forms.Label()
        Me.txtPhoneNumber = New System.Windows.Forms.MaskedTextBox()
        Me.Label14 = New System.Windows.Forms.Label()
        Me.lblGUID = New System.Windows.Forms.Label()
        Me.cmdMunisSearch = New System.Windows.Forms.Button()
        Me.pnlOtherFunctions = New System.Windows.Forms.Panel()
        Me.cmdMunisInfo = New System.Windows.Forms.Button()
        Me.cmdSibiLink = New System.Windows.Forms.Button()
        Me.Label12 = New System.Windows.Forms.Label()
        Me.txtPONumber = New System.Windows.Forms.TextBox()
        Me.chkTrackable = New System.Windows.Forms.CheckBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.txtAssetTag_View_REQ = New System.Windows.Forms.TextBox()
        Me.Label10 = New System.Windows.Forms.Label()
        Me.txtSerial_View_REQ = New System.Windows.Forms.TextBox()
        Me.Label2 = New System.Windows.Forms.Label()
        Me.Label9 = New System.Windows.Forms.Label()
        Me.txtCurUser_View_REQ = New System.Windows.Forms.TextBox()
        Me.cmbStatus_REQ = New System.Windows.Forms.ComboBox()
        Me.Label3 = New System.Windows.Forms.Label()
        Me.Label8 = New System.Windows.Forms.Label()
        Me.txtDescription_View_REQ = New System.Windows.Forms.TextBox()
        Me.cmbOSVersion_REQ = New System.Windows.Forms.ComboBox()
        Me.Label4 = New System.Windows.Forms.Label()
        Me.cmbLocation_View_REQ = New System.Windows.Forms.ComboBox()
        Me.Label13 = New System.Windows.Forms.Label()
        Me.Label5 = New System.Windows.Forms.Label()
        Me.cmbEquipType_View_REQ = New System.Windows.Forms.ComboBox()
        Me.dtPurchaseDate_View_REQ = New System.Windows.Forms.DateTimePicker()
        Me.Label7 = New System.Windows.Forms.Label()
        Me.Label6 = New System.Windows.Forms.Label()
        Me.txtReplacementYear_View = New System.Windows.Forms.TextBox()
        Me.grpNetTools = New System.Windows.Forms.GroupBox()
        Me.cmdRestart = New System.Windows.Forms.PictureBox()
        Me.cmdGKUpdate = New System.Windows.Forms.Button()
        Me.cmdShowIP = New System.Windows.Forms.Button()
        Me.cmdBrowseFiles = New System.Windows.Forms.Button()
        Me.cmdRDP = New System.Windows.Forms.Button()
        Me.RightClickMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.DeleteEntryToolStripMenuItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.TabControl1 = New System.Windows.Forms.TabControl()
        Me.HistoryTab = New System.Windows.Forms.TabPage()
        Me.DataGridHistory = New System.Windows.Forms.DataGridView()
        Me.TrackingTab = New System.Windows.Forms.TabPage()
        Me.TrackingGrid = New System.Windows.Forms.DataGridView()
        Me.TrackingBox = New System.Windows.Forms.GroupBox()
        Me.txtCheckLocation = New System.Windows.Forms.TextBox()
        Me.Label16 = New System.Windows.Forms.Label()
        Me.txtDueBack = New System.Windows.Forms.TextBox()
        Me.lblDueBack = New System.Windows.Forms.Label()
        Me.txtCheckUser = New System.Windows.Forms.TextBox()
        Me.lblCheckUser = New System.Windows.Forms.Label()
        Me.txtCheckTime = New System.Windows.Forms.TextBox()
        Me.lblCheckTime = New System.Windows.Forms.Label()
        Me.txtCheckOut = New System.Windows.Forms.TextBox()
        Me.Label11 = New System.Windows.Forms.Label()
        Me.ToolTip1 = New System.Windows.Forms.ToolTip(Me.components)
        Me.StatusStrip1 = New System.Windows.Forms.StatusStrip()
        Me.StatusLabel = New System.Windows.Forms.ToolStripStatusLabel()
        Me.tmr_RDPRefresher = New System.Windows.Forms.Timer(Me.components)
        Me.fieldErrorIcon = New System.Windows.Forms.ErrorProvider(Me.components)
        Me.tsSaveModify = New AssetManager.OneClickToolStrip()
        Me.cmdAccept_Tool = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator3 = New System.Windows.Forms.ToolStripSeparator()
        Me.cmdCancel_Tool = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator2 = New System.Windows.Forms.ToolStripSeparator()
        Me.Panel1 = New System.Windows.Forms.Panel()
        Me.Panel2 = New System.Windows.Forms.Panel()
        Me.FieldTabs = New System.Windows.Forms.TabControl()
        Me.AssetInfo = New System.Windows.Forms.TabPage()
        Me.MiscInfo = New System.Windows.Forms.TabPage()
        Me.iCloudTextBox = New System.Windows.Forms.TextBox()
        Me.Label17 = New System.Windows.Forms.Label()
        Me.ToolStripContainer1 = New System.Windows.Forms.ToolStripContainer()
        Me.tsTracking = New System.Windows.Forms.ToolStrip()
        Me.ToolStripDropDownButton1 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.CheckOutTool = New System.Windows.Forms.ToolStripMenuItem()
        Me.CheckInTool = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator4 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStrip1 = New AssetManager.OneClickToolStrip()
        Me.tsbModify = New System.Windows.Forms.ToolStripButton()
        Me.tsbNewNote = New System.Windows.Forms.ToolStripButton()
        Me.tsbDeleteDevice = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator1 = New System.Windows.Forms.ToolStripSeparator()
        Me.AttachmentTool = New System.Windows.Forms.ToolStripButton()
        Me.ToolStripSeparator7 = New System.Windows.Forms.ToolStripSeparator()
        Me.ToolStripDropDownButton2 = New System.Windows.Forms.ToolStripDropDownButton()
        Me.tsmAssetInputForm = New System.Windows.Forms.ToolStripMenuItem()
        Me.tsmAssetTransferForm = New System.Windows.Forms.ToolStripMenuItem()
        Me.AssetDisposalForm = New System.Windows.Forms.ToolStripMenuItem()
        Me.ToolStripSeparator9 = New System.Windows.Forms.ToolStripSeparator()
        Me.pnlOtherFunctions.SuspendLayout()
        Me.grpNetTools.SuspendLayout()
        CType(Me.cmdRestart, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.RightClickMenu.SuspendLayout()
        Me.TabControl1.SuspendLayout()
        Me.HistoryTab.SuspendLayout()
        CType(Me.DataGridHistory, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TrackingTab.SuspendLayout()
        CType(Me.TrackingGrid, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.TrackingBox.SuspendLayout()
        Me.StatusStrip1.SuspendLayout()
        CType(Me.fieldErrorIcon, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.tsSaveModify.SuspendLayout()
        Me.Panel1.SuspendLayout()
        Me.Panel2.SuspendLayout()
        Me.FieldTabs.SuspendLayout()
        Me.AssetInfo.SuspendLayout()
        Me.MiscInfo.SuspendLayout()
        Me.ToolStripContainer1.ContentPanel.SuspendLayout()
        Me.ToolStripContainer1.TopToolStripPanel.SuspendLayout()
        Me.ToolStripContainer1.SuspendLayout()
        Me.tsTracking.SuspendLayout()
        Me.ToolStrip1.SuspendLayout()
        Me.SuspendLayout()
        '
        'txtHostname
        '
        Me.txtHostname.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtHostname.Location = New System.Drawing.Point(166, 47)
        Me.txtHostname.Name = "txtHostname"
        Me.txtHostname.Size = New System.Drawing.Size(177, 23)
        Me.txtHostname.TabIndex = 58
        '
        'Label15
        '
        Me.Label15.AutoSize = True
        Me.Label15.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label15.Location = New System.Drawing.Point(163, 28)
        Me.Label15.Name = "Label15"
        Me.Label15.Size = New System.Drawing.Size(73, 16)
        Me.Label15.TabIndex = 59
        Me.Label15.Text = "Hostname:"
        '
        'txtPhoneNumber
        '
        Me.txtPhoneNumber.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPhoneNumber.Location = New System.Drawing.Point(166, 100)
        Me.txtPhoneNumber.Mask = "(999) 000-0000"
        Me.txtPhoneNumber.Name = "txtPhoneNumber"
        Me.txtPhoneNumber.Size = New System.Drawing.Size(177, 23)
        Me.txtPhoneNumber.TabIndex = 57
        Me.txtPhoneNumber.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals
        '
        'Label14
        '
        Me.Label14.AutoSize = True
        Me.Label14.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label14.Location = New System.Drawing.Point(163, 81)
        Me.Label14.Name = "Label14"
        Me.Label14.Size = New System.Drawing.Size(60, 16)
        Me.Label14.TabIndex = 56
        Me.Label14.Text = "Phone #:"
        '
        'lblGUID
        '
        Me.lblGUID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.lblGUID.Cursor = System.Windows.Forms.Cursors.Default
        Me.lblGUID.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblGUID.Location = New System.Drawing.Point(166, 155)
        Me.lblGUID.Name = "lblGUID"
        Me.lblGUID.Size = New System.Drawing.Size(272, 23)
        Me.lblGUID.TabIndex = 54
        Me.lblGUID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter
        Me.ToolTip1.SetToolTip(Me.lblGUID, "Click to copy GUID.")
        '
        'cmdMunisSearch
        '
        Me.cmdMunisSearch.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdMunisSearch.Location = New System.Drawing.Point(20, 155)
        Me.cmdMunisSearch.Name = "cmdMunisSearch"
        Me.cmdMunisSearch.Size = New System.Drawing.Size(135, 23)
        Me.cmdMunisSearch.TabIndex = 3
        Me.cmdMunisSearch.Text = "Munis Search"
        Me.cmdMunisSearch.UseVisualStyleBackColor = True
        Me.cmdMunisSearch.Visible = False
        '
        'pnlOtherFunctions
        '
        Me.pnlOtherFunctions.Controls.Add(Me.cmdMunisInfo)
        Me.pnlOtherFunctions.Controls.Add(Me.cmdSibiLink)
        Me.pnlOtherFunctions.Location = New System.Drawing.Point(584, 168)
        Me.pnlOtherFunctions.Name = "pnlOtherFunctions"
        Me.pnlOtherFunctions.Size = New System.Drawing.Size(194, 61)
        Me.pnlOtherFunctions.TabIndex = 51
        '
        'cmdMunisInfo
        '
        Me.cmdMunisInfo.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdMunisInfo.Location = New System.Drawing.Point(14, 3)
        Me.cmdMunisInfo.Name = "cmdMunisInfo"
        Me.cmdMunisInfo.Size = New System.Drawing.Size(170, 23)
        Me.cmdMunisInfo.TabIndex = 46
        Me.cmdMunisInfo.Text = "View MUNIS"
        Me.ToolTip1.SetToolTip(Me.cmdMunisInfo, "View Munis")
        Me.cmdMunisInfo.UseVisualStyleBackColor = True
        '
        'cmdSibiLink
        '
        Me.cmdSibiLink.Anchor = CType((System.Windows.Forms.AnchorStyles.Bottom Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.cmdSibiLink.Location = New System.Drawing.Point(14, 32)
        Me.cmdSibiLink.Name = "cmdSibiLink"
        Me.cmdSibiLink.Size = New System.Drawing.Size(170, 23)
        Me.cmdSibiLink.TabIndex = 49
        Me.cmdSibiLink.Text = "View Sibi"
        Me.cmdSibiLink.UseVisualStyleBackColor = True
        '
        'Label12
        '
        Me.Label12.AutoSize = True
        Me.Label12.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label12.Location = New System.Drawing.Point(595, 110)
        Me.Label12.Name = "Label12"
        Me.Label12.Size = New System.Drawing.Size(81, 16)
        Me.Label12.TabIndex = 48
        Me.Label12.Text = "PO Number:"
        '
        'txtPONumber
        '
        Me.txtPONumber.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtPONumber.Location = New System.Drawing.Point(598, 129)
        Me.txtPONumber.Name = "txtPONumber"
        Me.txtPONumber.Size = New System.Drawing.Size(169, 23)
        Me.txtPONumber.TabIndex = 11
        '
        'chkTrackable
        '
        Me.chkTrackable.AutoSize = True
        Me.chkTrackable.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.chkTrackable.Location = New System.Drawing.Point(166, 195)
        Me.chkTrackable.Name = "chkTrackable"
        Me.chkTrackable.Size = New System.Drawing.Size(89, 20)
        Me.chkTrackable.TabIndex = 13
        Me.chkTrackable.Text = "Trackable"
        Me.chkTrackable.UseVisualStyleBackColor = True
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(18, 62)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(73, 16)
        Me.Label1.TabIndex = 20
        Me.Label1.Text = "Asset Tag:"
        '
        'txtAssetTag_View_REQ
        '
        Me.txtAssetTag_View_REQ.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtAssetTag_View_REQ.Location = New System.Drawing.Point(21, 81)
        Me.txtAssetTag_View_REQ.Name = "txtAssetTag_View_REQ"
        Me.txtAssetTag_View_REQ.Size = New System.Drawing.Size(134, 23)
        Me.txtAssetTag_View_REQ.TabIndex = 1
        '
        'Label10
        '
        Me.Label10.AutoSize = True
        Me.Label10.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label10.Location = New System.Drawing.Point(162, 135)
        Me.Label10.Name = "Label10"
        Me.Label10.Size = New System.Drawing.Size(90, 16)
        Me.Label10.TabIndex = 41
        Me.Label10.Text = "Device GUID:"
        '
        'txtSerial_View_REQ
        '
        Me.txtSerial_View_REQ.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtSerial_View_REQ.Location = New System.Drawing.Point(21, 33)
        Me.txtSerial_View_REQ.Name = "txtSerial_View_REQ"
        Me.txtSerial_View_REQ.Size = New System.Drawing.Size(134, 23)
        Me.txtSerial_View_REQ.TabIndex = 0
        '
        'Label2
        '
        Me.Label2.AutoSize = True
        Me.Label2.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label2.Location = New System.Drawing.Point(18, 14)
        Me.Label2.Name = "Label2"
        Me.Label2.Size = New System.Drawing.Size(46, 16)
        Me.Label2.TabIndex = 22
        Me.Label2.Text = "Serial:"
        '
        'Label9
        '
        Me.Label9.AutoSize = True
        Me.Label9.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label9.Location = New System.Drawing.Point(390, 110)
        Me.Label9.Name = "Label9"
        Me.Label9.Size = New System.Drawing.Size(48, 16)
        Me.Label9.TabIndex = 39
        Me.Label9.Text = "Status:"
        '
        'txtCurUser_View_REQ
        '
        Me.txtCurUser_View_REQ.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCurUser_View_REQ.Location = New System.Drawing.Point(21, 129)
        Me.txtCurUser_View_REQ.Name = "txtCurUser_View_REQ"
        Me.txtCurUser_View_REQ.Size = New System.Drawing.Size(134, 23)
        Me.txtCurUser_View_REQ.TabIndex = 2
        '
        'cmbStatus_REQ
        '
        Me.cmbStatus_REQ.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbStatus_REQ.FormattingEnabled = True
        Me.cmbStatus_REQ.Location = New System.Drawing.Point(393, 129)
        Me.cmbStatus_REQ.Name = "cmbStatus_REQ"
        Me.cmbStatus_REQ.Size = New System.Drawing.Size(177, 23)
        Me.cmbStatus_REQ.TabIndex = 8
        '
        'Label3
        '
        Me.Label3.AutoSize = True
        Me.Label3.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label3.Location = New System.Drawing.Point(17, 110)
        Me.Label3.Name = "Label3"
        Me.Label3.Size = New System.Drawing.Size(85, 16)
        Me.Label3.TabIndex = 24
        Me.Label3.Text = "Current User:"
        '
        'Label8
        '
        Me.Label8.AutoSize = True
        Me.Label8.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label8.Location = New System.Drawing.Point(178, 110)
        Me.Label8.Name = "Label8"
        Me.Label8.Size = New System.Drawing.Size(79, 16)
        Me.Label8.TabIndex = 37
        Me.Label8.Text = "OS Version:"
        '
        'txtDescription_View_REQ
        '
        Me.txtDescription_View_REQ.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDescription_View_REQ.Location = New System.Drawing.Point(181, 33)
        Me.txtDescription_View_REQ.Name = "txtDescription_View_REQ"
        Me.txtDescription_View_REQ.Size = New System.Drawing.Size(389, 23)
        Me.txtDescription_View_REQ.TabIndex = 4
        '
        'cmbOSVersion_REQ
        '
        Me.cmbOSVersion_REQ.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbOSVersion_REQ.FormattingEnabled = True
        Me.cmbOSVersion_REQ.Location = New System.Drawing.Point(181, 129)
        Me.cmbOSVersion_REQ.Name = "cmbOSVersion_REQ"
        Me.cmbOSVersion_REQ.Size = New System.Drawing.Size(177, 23)
        Me.cmbOSVersion_REQ.TabIndex = 6
        '
        'Label4
        '
        Me.Label4.AutoSize = True
        Me.Label4.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label4.Location = New System.Drawing.Point(176, 14)
        Me.Label4.Name = "Label4"
        Me.Label4.Size = New System.Drawing.Size(79, 16)
        Me.Label4.TabIndex = 26
        Me.Label4.Text = "Description:"
        '
        'cmbLocation_View_REQ
        '
        Me.cmbLocation_View_REQ.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbLocation_View_REQ.FormattingEnabled = True
        Me.cmbLocation_View_REQ.Location = New System.Drawing.Point(393, 81)
        Me.cmbLocation_View_REQ.Name = "cmbLocation_View_REQ"
        Me.cmbLocation_View_REQ.Size = New System.Drawing.Size(177, 23)
        Me.cmbLocation_View_REQ.TabIndex = 7
        '
        'Label13
        '
        Me.Label13.AutoSize = True
        Me.Label13.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label13.Location = New System.Drawing.Point(178, 62)
        Me.Label13.Name = "Label13"
        Me.Label13.Size = New System.Drawing.Size(110, 16)
        Me.Label13.TabIndex = 34
        Me.Label13.Text = "Equipment Type:"
        '
        'Label5
        '
        Me.Label5.AutoSize = True
        Me.Label5.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label5.Location = New System.Drawing.Point(390, 62)
        Me.Label5.Name = "Label5"
        Me.Label5.Size = New System.Drawing.Size(62, 16)
        Me.Label5.TabIndex = 28
        Me.Label5.Text = "Location:"
        '
        'cmbEquipType_View_REQ
        '
        Me.cmbEquipType_View_REQ.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmbEquipType_View_REQ.FormattingEnabled = True
        Me.cmbEquipType_View_REQ.Location = New System.Drawing.Point(181, 81)
        Me.cmbEquipType_View_REQ.Name = "cmbEquipType_View_REQ"
        Me.cmbEquipType_View_REQ.Size = New System.Drawing.Size(177, 23)
        Me.cmbEquipType_View_REQ.TabIndex = 5
        '
        'dtPurchaseDate_View_REQ
        '
        Me.dtPurchaseDate_View_REQ.CustomFormat = "yyyy-MM-dd"
        Me.dtPurchaseDate_View_REQ.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.dtPurchaseDate_View_REQ.Format = System.Windows.Forms.DateTimePickerFormat.Custom
        Me.dtPurchaseDate_View_REQ.Location = New System.Drawing.Point(600, 33)
        Me.dtPurchaseDate_View_REQ.Name = "dtPurchaseDate_View_REQ"
        Me.dtPurchaseDate_View_REQ.Size = New System.Drawing.Size(168, 23)
        Me.dtPurchaseDate_View_REQ.TabIndex = 9
        '
        'Label7
        '
        Me.Label7.AutoSize = True
        Me.Label7.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label7.Location = New System.Drawing.Point(595, 62)
        Me.Label7.Name = "Label7"
        Me.Label7.Size = New System.Drawing.Size(95, 16)
        Me.Label7.TabIndex = 32
        Me.Label7.Text = "Replace Year:"
        '
        'Label6
        '
        Me.Label6.AutoSize = True
        Me.Label6.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label6.Location = New System.Drawing.Point(595, 14)
        Me.Label6.Name = "Label6"
        Me.Label6.Size = New System.Drawing.Size(100, 16)
        Me.Label6.TabIndex = 30
        Me.Label6.Text = "Purchase Date:"
        '
        'txtReplacementYear_View
        '
        Me.txtReplacementYear_View.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtReplacementYear_View.Location = New System.Drawing.Point(598, 81)
        Me.txtReplacementYear_View.Name = "txtReplacementYear_View"
        Me.txtReplacementYear_View.Size = New System.Drawing.Size(169, 23)
        Me.txtReplacementYear_View.TabIndex = 10
        '
        'grpNetTools
        '
        Me.grpNetTools.Controls.Add(Me.cmdRestart)
        Me.grpNetTools.Controls.Add(Me.cmdGKUpdate)
        Me.grpNetTools.Controls.Add(Me.cmdShowIP)
        Me.grpNetTools.Controls.Add(Me.cmdBrowseFiles)
        Me.grpNetTools.Controls.Add(Me.cmdRDP)
        Me.grpNetTools.FlatStyle = System.Windows.Forms.FlatStyle.System
        Me.grpNetTools.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.grpNetTools.Location = New System.Drawing.Point(3, 3)
        Me.grpNetTools.Name = "grpNetTools"
        Me.grpNetTools.Size = New System.Drawing.Size(320, 96)
        Me.grpNetTools.TabIndex = 52
        Me.grpNetTools.TabStop = False
        Me.grpNetTools.Text = "Remote Mgmt"
        Me.grpNetTools.Visible = False
        '
        'cmdRestart
        '
        Me.cmdRestart.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle
        Me.cmdRestart.Image = Global.AssetManager.My.Resources.Resources.RestartIcon
        Me.cmdRestart.Location = New System.Drawing.Point(118, 29)
        Me.cmdRestart.Name = "cmdRestart"
        Me.cmdRestart.Size = New System.Drawing.Size(50, 50)
        Me.cmdRestart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage
        Me.cmdRestart.TabIndex = 56
        Me.cmdRestart.TabStop = False
        Me.ToolTip1.SetToolTip(Me.cmdRestart, "Reboot Device")
        '
        'cmdGKUpdate
        '
        Me.cmdGKUpdate.BackgroundImage = Global.AssetManager.My.Resources.Resources.GK__UpdateIcon
        Me.cmdGKUpdate.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdGKUpdate.Location = New System.Drawing.Point(14, 29)
        Me.cmdGKUpdate.Margin = New System.Windows.Forms.Padding(1)
        Me.cmdGKUpdate.Name = "cmdGKUpdate"
        Me.cmdGKUpdate.Size = New System.Drawing.Size(50, 50)
        Me.cmdGKUpdate.TabIndex = 55
        Me.ToolTip1.SetToolTip(Me.cmdGKUpdate, "Enqueue GK Update")
        Me.cmdGKUpdate.UseVisualStyleBackColor = True
        '
        'cmdShowIP
        '
        Me.cmdShowIP.BackColor = System.Drawing.Color.Black
        Me.cmdShowIP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom
        Me.cmdShowIP.FlatStyle = System.Windows.Forms.FlatStyle.Popup
        Me.cmdShowIP.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdShowIP.ForeColor = System.Drawing.Color.White
        Me.cmdShowIP.Location = New System.Drawing.Point(236, 11)
        Me.cmdShowIP.Name = "cmdShowIP"
        Me.cmdShowIP.Size = New System.Drawing.Size(80, 80)
        Me.cmdShowIP.TabIndex = 53
        Me.cmdShowIP.TextAlign = System.Drawing.ContentAlignment.TopRight
        Me.cmdShowIP.UseVisualStyleBackColor = False
        '
        'cmdBrowseFiles
        '
        Me.cmdBrowseFiles.BackgroundImage = Global.AssetManager.My.Resources.Resources.FolderIcon
        Me.cmdBrowseFiles.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdBrowseFiles.Location = New System.Drawing.Point(66, 29)
        Me.cmdBrowseFiles.Margin = New System.Windows.Forms.Padding(1)
        Me.cmdBrowseFiles.Name = "cmdBrowseFiles"
        Me.cmdBrowseFiles.Size = New System.Drawing.Size(50, 50)
        Me.cmdBrowseFiles.TabIndex = 52
        Me.ToolTip1.SetToolTip(Me.cmdBrowseFiles, "Browse Files")
        Me.cmdBrowseFiles.UseVisualStyleBackColor = True
        '
        'cmdRDP
        '
        Me.cmdRDP.BackgroundImage = Global.AssetManager.My.Resources.Resources.RDPIcon
        Me.cmdRDP.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch
        Me.cmdRDP.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft
        Me.cmdRDP.Location = New System.Drawing.Point(170, 29)
        Me.cmdRDP.Margin = New System.Windows.Forms.Padding(1)
        Me.cmdRDP.Name = "cmdRDP"
        Me.cmdRDP.Size = New System.Drawing.Size(50, 50)
        Me.cmdRDP.TabIndex = 46
        Me.ToolTip1.SetToolTip(Me.cmdRDP, "Launch Remote Desktop")
        Me.cmdRDP.UseVisualStyleBackColor = True
        '
        'RightClickMenu
        '
        Me.RightClickMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.DeleteEntryToolStripMenuItem})
        Me.RightClickMenu.Name = "RightClickMenu"
        Me.RightClickMenu.Size = New System.Drawing.Size(138, 26)
        '
        'DeleteEntryToolStripMenuItem
        '
        Me.DeleteEntryToolStripMenuItem.Image = Global.AssetManager.My.Resources.Resources.DeleteIcon
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
        Me.TabControl1.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TabControl1.ItemSize = New System.Drawing.Size(69, 21)
        Me.TabControl1.Location = New System.Drawing.Point(11, 296)
        Me.TabControl1.Name = "TabControl1"
        Me.TabControl1.SelectedIndex = 0
        Me.TabControl1.Size = New System.Drawing.Size(1139, 321)
        Me.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.TabControl1.TabIndex = 40
        '
        'HistoryTab
        '
        Me.HistoryTab.Controls.Add(Me.DataGridHistory)
        Me.HistoryTab.Location = New System.Drawing.Point(4, 25)
        Me.HistoryTab.Name = "HistoryTab"
        Me.HistoryTab.Padding = New System.Windows.Forms.Padding(3)
        Me.HistoryTab.Size = New System.Drawing.Size(1131, 292)
        Me.HistoryTab.TabIndex = 0
        Me.HistoryTab.Text = "History"
        Me.HistoryTab.UseVisualStyleBackColor = True
        '
        'DataGridHistory
        '
        Me.DataGridHistory.AllowUserToAddRows = False
        Me.DataGridHistory.AllowUserToDeleteRows = False
        Me.DataGridHistory.AllowUserToResizeColumns = False
        Me.DataGridHistory.AllowUserToResizeRows = False
        Me.DataGridHistory.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.DataGridHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.DataGridHistory.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.DataGridHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.DataGridHistory.ContextMenuStrip = Me.RightClickMenu
        DataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter
        DataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window
        DataGridViewCellStyle1.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        DataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText
        DataGridViewCellStyle1.Padding = New System.Windows.Forms.Padding(10)
        DataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight
        DataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText
        DataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.[False]
        Me.DataGridHistory.DefaultCellStyle = DataGridViewCellStyle1
        Me.DataGridHistory.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.DataGridHistory.Location = New System.Drawing.Point(6, 6)
        Me.DataGridHistory.Name = "DataGridHistory"
        Me.DataGridHistory.ReadOnly = True
        Me.DataGridHistory.RowHeadersVisible = False
        Me.DataGridHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect
        Me.DataGridHistory.ShowCellToolTips = False
        Me.DataGridHistory.ShowEditingIcon = False
        Me.DataGridHistory.Size = New System.Drawing.Size(1119, 280)
        Me.DataGridHistory.TabIndex = 40
        Me.DataGridHistory.TabStop = False
        Me.DataGridHistory.VirtualMode = True
        '
        'TrackingTab
        '
        Me.TrackingTab.Controls.Add(Me.TrackingGrid)
        Me.TrackingTab.Location = New System.Drawing.Point(4, 25)
        Me.TrackingTab.Name = "TrackingTab"
        Me.TrackingTab.Padding = New System.Windows.Forms.Padding(3)
        Me.TrackingTab.Size = New System.Drawing.Size(1131, 292)
        Me.TrackingTab.TabIndex = 1
        Me.TrackingTab.Text = "Tracking"
        Me.TrackingTab.UseVisualStyleBackColor = True
        '
        'TrackingGrid
        '
        Me.TrackingGrid.AllowUserToAddRows = False
        Me.TrackingGrid.AllowUserToDeleteRows = False
        Me.TrackingGrid.AllowUserToResizeColumns = False
        Me.TrackingGrid.AllowUserToResizeRows = False
        Me.TrackingGrid.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TrackingGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells
        Me.TrackingGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText
        Me.TrackingGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize
        Me.TrackingGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically
        Me.TrackingGrid.Location = New System.Drawing.Point(6, 6)
        Me.TrackingGrid.MultiSelect = False
        Me.TrackingGrid.Name = "TrackingGrid"
        Me.TrackingGrid.ReadOnly = True
        Me.TrackingGrid.RowHeadersVisible = False
        Me.TrackingGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect
        Me.TrackingGrid.ShowCellToolTips = False
        Me.TrackingGrid.ShowEditingIcon = False
        Me.TrackingGrid.Size = New System.Drawing.Size(1119, 280)
        Me.TrackingGrid.TabIndex = 41
        '
        'TrackingBox
        '
        Me.TrackingBox.BackColor = System.Drawing.Color.FromArgb(CType(CType(232, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(232, Byte), Integer))
        Me.TrackingBox.Controls.Add(Me.txtCheckLocation)
        Me.TrackingBox.Controls.Add(Me.Label16)
        Me.TrackingBox.Controls.Add(Me.txtDueBack)
        Me.TrackingBox.Controls.Add(Me.lblDueBack)
        Me.TrackingBox.Controls.Add(Me.txtCheckUser)
        Me.TrackingBox.Controls.Add(Me.lblCheckUser)
        Me.TrackingBox.Controls.Add(Me.txtCheckTime)
        Me.TrackingBox.Controls.Add(Me.lblCheckTime)
        Me.TrackingBox.Controls.Add(Me.txtCheckOut)
        Me.TrackingBox.Controls.Add(Me.Label11)
        Me.TrackingBox.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.TrackingBox.Location = New System.Drawing.Point(3, 105)
        Me.TrackingBox.Name = "TrackingBox"
        Me.TrackingBox.Size = New System.Drawing.Size(320, 167)
        Me.TrackingBox.TabIndex = 41
        Me.TrackingBox.TabStop = False
        Me.TrackingBox.Text = "Tracking Info"
        Me.TrackingBox.Visible = False
        '
        'txtCheckLocation
        '
        Me.txtCheckLocation.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.txtCheckLocation.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCheckLocation.Location = New System.Drawing.Point(169, 42)
        Me.txtCheckLocation.Name = "txtCheckLocation"
        Me.txtCheckLocation.ReadOnly = True
        Me.txtCheckLocation.Size = New System.Drawing.Size(134, 22)
        Me.txtCheckLocation.TabIndex = 57
        Me.txtCheckLocation.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label16
        '
        Me.Label16.AutoSize = True
        Me.Label16.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label16.Location = New System.Drawing.Point(166, 23)
        Me.Label16.Name = "Label16"
        Me.Label16.Size = New System.Drawing.Size(62, 16)
        Me.Label16.TabIndex = 56
        Me.Label16.Text = "Location:"
        '
        'txtDueBack
        '
        Me.txtDueBack.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.txtDueBack.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtDueBack.Location = New System.Drawing.Point(87, 131)
        Me.txtDueBack.Name = "txtDueBack"
        Me.txtDueBack.ReadOnly = True
        Me.txtDueBack.Size = New System.Drawing.Size(134, 22)
        Me.txtDueBack.TabIndex = 55
        Me.txtDueBack.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblDueBack
        '
        Me.lblDueBack.AutoSize = True
        Me.lblDueBack.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblDueBack.Location = New System.Drawing.Point(84, 112)
        Me.lblDueBack.Name = "lblDueBack"
        Me.lblDueBack.Size = New System.Drawing.Size(70, 16)
        Me.lblDueBack.TabIndex = 54
        Me.lblDueBack.Text = "Due Back:"
        '
        'txtCheckUser
        '
        Me.txtCheckUser.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.txtCheckUser.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCheckUser.Location = New System.Drawing.Point(169, 88)
        Me.txtCheckUser.Name = "txtCheckUser"
        Me.txtCheckUser.ReadOnly = True
        Me.txtCheckUser.Size = New System.Drawing.Size(134, 22)
        Me.txtCheckUser.TabIndex = 53
        Me.txtCheckUser.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblCheckUser
        '
        Me.lblCheckUser.AutoSize = True
        Me.lblCheckUser.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCheckUser.Location = New System.Drawing.Point(166, 69)
        Me.lblCheckUser.Name = "lblCheckUser"
        Me.lblCheckUser.Size = New System.Drawing.Size(101, 16)
        Me.lblCheckUser.TabIndex = 52
        Me.lblCheckUser.Text = "CheckOut User:"
        '
        'txtCheckTime
        '
        Me.txtCheckTime.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.txtCheckTime.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCheckTime.Location = New System.Drawing.Point(19, 88)
        Me.txtCheckTime.Name = "txtCheckTime"
        Me.txtCheckTime.ReadOnly = True
        Me.txtCheckTime.Size = New System.Drawing.Size(134, 22)
        Me.txtCheckTime.TabIndex = 51
        Me.txtCheckTime.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'lblCheckTime
        '
        Me.lblCheckTime.AutoSize = True
        Me.lblCheckTime.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblCheckTime.Location = New System.Drawing.Point(16, 69)
        Me.lblCheckTime.Name = "lblCheckTime"
        Me.lblCheckTime.Size = New System.Drawing.Size(103, 16)
        Me.lblCheckTime.TabIndex = 50
        Me.lblCheckTime.Text = "CheckOut Time:"
        '
        'txtCheckOut
        '
        Me.txtCheckOut.BackColor = System.Drawing.Color.FromArgb(CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer), CType(CType(224, Byte), Integer))
        Me.txtCheckOut.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.txtCheckOut.Location = New System.Drawing.Point(19, 42)
        Me.txtCheckOut.Name = "txtCheckOut"
        Me.txtCheckOut.ReadOnly = True
        Me.txtCheckOut.Size = New System.Drawing.Size(134, 22)
        Me.txtCheckOut.TabIndex = 49
        Me.txtCheckOut.Text = "STATUS"
        Me.txtCheckOut.TextAlign = System.Windows.Forms.HorizontalAlignment.Center
        '
        'Label11
        '
        Me.Label11.AutoSize = True
        Me.Label11.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label11.Location = New System.Drawing.Point(16, 23)
        Me.Label11.Name = "Label11"
        Me.Label11.Size = New System.Drawing.Size(48, 16)
        Me.Label11.TabIndex = 48
        Me.Label11.Text = "Status:"
        '
        'ToolTip1
        '
        Me.ToolTip1.AutoPopDelay = 5000
        Me.ToolTip1.InitialDelay = 100
        Me.ToolTip1.ReshowDelay = 100
        '
        'StatusStrip1
        '
        Me.StatusStrip1.BackColor = System.Drawing.Color.FromArgb(CType(CType(232, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(232, Byte), Integer))
        Me.StatusStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.StatusLabel})
        Me.StatusStrip1.Location = New System.Drawing.Point(0, 740)
        Me.StatusStrip1.Name = "StatusStrip1"
        Me.StatusStrip1.Size = New System.Drawing.Size(1161, 22)
        Me.StatusStrip1.TabIndex = 45
        Me.StatusStrip1.Text = "StatusStrip1"
        '
        'StatusLabel
        '
        Me.StatusLabel.Font = New System.Drawing.Font("Segoe UI", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.StatusLabel.Name = "StatusLabel"
        Me.StatusLabel.Size = New System.Drawing.Size(76, 17)
        Me.StatusLabel.Text = "%STATUS%"
        '
        'tmr_RDPRefresher
        '
        Me.tmr_RDPRefresher.Interval = 1000
        '
        'fieldErrorIcon
        '
        Me.fieldErrorIcon.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
        Me.fieldErrorIcon.ContainerControl = Me
        Me.fieldErrorIcon.Icon = CType(resources.GetObject("fieldErrorIcon.Icon"), System.Drawing.Icon)
        '
        'tsSaveModify
        '
        Me.tsSaveModify.BackColor = System.Drawing.Color.FromArgb(CType(CType(88, Byte), Integer), CType(CType(237, Byte), Integer), CType(CType(118, Byte), Integer))
        Me.tsSaveModify.CanOverflow = False
        Me.tsSaveModify.Dock = System.Windows.Forms.DockStyle.None
        Me.tsSaveModify.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.tsSaveModify.ImageScalingSize = New System.Drawing.Size(25, 25)
        Me.tsSaveModify.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.cmdAccept_Tool, Me.ToolStripSeparator3, Me.cmdCancel_Tool, Me.ToolStripSeparator2})
        Me.tsSaveModify.Location = New System.Drawing.Point(3, 0)
        Me.tsSaveModify.Name = "tsSaveModify"
        Me.tsSaveModify.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.tsSaveModify.Size = New System.Drawing.Size(312, 37)
        Me.tsSaveModify.TabIndex = 44
        Me.tsSaveModify.Text = "ToolStrip1"
        '
        'cmdAccept_Tool
        '
        Me.cmdAccept_Tool.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdAccept_Tool.Image = Global.AssetManager.My.Resources.Resources.CheckedBoxIcon
        Me.cmdAccept_Tool.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdAccept_Tool.Name = "cmdAccept_Tool"
        Me.cmdAccept_Tool.Padding = New System.Windows.Forms.Padding(50, 5, 5, 0)
        Me.cmdAccept_Tool.Size = New System.Drawing.Size(146, 34)
        Me.cmdAccept_Tool.Text = "Accept"
        '
        'ToolStripSeparator3
        '
        Me.ToolStripSeparator3.Name = "ToolStripSeparator3"
        Me.ToolStripSeparator3.Size = New System.Drawing.Size(6, 37)
        '
        'cmdCancel_Tool
        '
        Me.cmdCancel_Tool.Font = New System.Drawing.Font("Segoe UI Semibold", 12.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdCancel_Tool.Image = Global.AssetManager.My.Resources.Resources.CloseCancelDeleteIcon
        Me.cmdCancel_Tool.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.cmdCancel_Tool.Name = "cmdCancel_Tool"
        Me.cmdCancel_Tool.Padding = New System.Windows.Forms.Padding(50, 5, 5, 0)
        Me.cmdCancel_Tool.Size = New System.Drawing.Size(142, 34)
        Me.cmdCancel_Tool.Text = "Cancel"
        '
        'ToolStripSeparator2
        '
        Me.ToolStripSeparator2.Name = "ToolStripSeparator2"
        Me.ToolStripSeparator2.Size = New System.Drawing.Size(6, 37)
        '
        'Panel1
        '
        Me.Panel1.BackColor = System.Drawing.Color.FromArgb(CType(CType(232, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(232, Byte), Integer))
        Me.Panel1.Controls.Add(Me.Panel2)
        Me.Panel1.Controls.Add(Me.FieldTabs)
        Me.Panel1.Location = New System.Drawing.Point(11, 8)
        Me.Panel1.Name = "Panel1"
        Me.Panel1.Size = New System.Drawing.Size(1136, 282)
        Me.Panel1.TabIndex = 53
        '
        'Panel2
        '
        Me.Panel2.Controls.Add(Me.grpNetTools)
        Me.Panel2.Controls.Add(Me.TrackingBox)
        Me.Panel2.Location = New System.Drawing.Point(801, 3)
        Me.Panel2.Name = "Panel2"
        Me.Panel2.Size = New System.Drawing.Size(330, 276)
        Me.Panel2.TabIndex = 54
        '
        'FieldTabs
        '
        Me.FieldTabs.Controls.Add(Me.AssetInfo)
        Me.FieldTabs.Controls.Add(Me.MiscInfo)
        Me.FieldTabs.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.FieldTabs.HotTrack = True
        Me.FieldTabs.Location = New System.Drawing.Point(3, 3)
        Me.FieldTabs.Multiline = True
        Me.FieldTabs.Name = "FieldTabs"
        Me.FieldTabs.SelectedIndex = 0
        Me.FieldTabs.Size = New System.Drawing.Size(796, 276)
        Me.FieldTabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed
        Me.FieldTabs.TabIndex = 53
        '
        'AssetInfo
        '
        Me.AssetInfo.BackColor = System.Drawing.SystemColors.Control
        Me.AssetInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.AssetInfo.Controls.Add(Me.pnlOtherFunctions)
        Me.AssetInfo.Controls.Add(Me.Label2)
        Me.AssetInfo.Controls.Add(Me.Label3)
        Me.AssetInfo.Controls.Add(Me.txtCurUser_View_REQ)
        Me.AssetInfo.Controls.Add(Me.txtSerial_View_REQ)
        Me.AssetInfo.Controls.Add(Me.txtAssetTag_View_REQ)
        Me.AssetInfo.Controls.Add(Me.cmdMunisSearch)
        Me.AssetInfo.Controls.Add(Me.Label12)
        Me.AssetInfo.Controls.Add(Me.Label1)
        Me.AssetInfo.Controls.Add(Me.txtPONumber)
        Me.AssetInfo.Controls.Add(Me.Label4)
        Me.AssetInfo.Controls.Add(Me.cmbEquipType_View_REQ)
        Me.AssetInfo.Controls.Add(Me.Label5)
        Me.AssetInfo.Controls.Add(Me.dtPurchaseDate_View_REQ)
        Me.AssetInfo.Controls.Add(Me.Label9)
        Me.AssetInfo.Controls.Add(Me.Label7)
        Me.AssetInfo.Controls.Add(Me.Label13)
        Me.AssetInfo.Controls.Add(Me.Label6)
        Me.AssetInfo.Controls.Add(Me.txtReplacementYear_View)
        Me.AssetInfo.Controls.Add(Me.cmbStatus_REQ)
        Me.AssetInfo.Controls.Add(Me.cmbLocation_View_REQ)
        Me.AssetInfo.Controls.Add(Me.Label8)
        Me.AssetInfo.Controls.Add(Me.cmbOSVersion_REQ)
        Me.AssetInfo.Controls.Add(Me.txtDescription_View_REQ)
        Me.AssetInfo.Location = New System.Drawing.Point(4, 23)
        Me.AssetInfo.Name = "AssetInfo"
        Me.AssetInfo.Padding = New System.Windows.Forms.Padding(3)
        Me.AssetInfo.Size = New System.Drawing.Size(788, 249)
        Me.AssetInfo.TabIndex = 0
        Me.AssetInfo.Text = "Asset Info."
        '
        'MiscInfo
        '
        Me.MiscInfo.BackColor = System.Drawing.SystemColors.Control
        Me.MiscInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D
        Me.MiscInfo.Controls.Add(Me.iCloudTextBox)
        Me.MiscInfo.Controls.Add(Me.Label17)
        Me.MiscInfo.Controls.Add(Me.lblGUID)
        Me.MiscInfo.Controls.Add(Me.txtPhoneNumber)
        Me.MiscInfo.Controls.Add(Me.Label10)
        Me.MiscInfo.Controls.Add(Me.Label14)
        Me.MiscInfo.Controls.Add(Me.chkTrackable)
        Me.MiscInfo.Controls.Add(Me.txtHostname)
        Me.MiscInfo.Controls.Add(Me.Label15)
        Me.MiscInfo.Cursor = System.Windows.Forms.Cursors.Default
        Me.MiscInfo.Location = New System.Drawing.Point(4, 23)
        Me.MiscInfo.Name = "MiscInfo"
        Me.MiscInfo.Padding = New System.Windows.Forms.Padding(3)
        Me.MiscInfo.Size = New System.Drawing.Size(788, 249)
        Me.MiscInfo.TabIndex = 1
        Me.MiscInfo.Text = "Misc."
        '
        'iCloudTextBox
        '
        Me.iCloudTextBox.Font = New System.Drawing.Font("Consolas", 9.75!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.iCloudTextBox.Location = New System.Drawing.Point(385, 47)
        Me.iCloudTextBox.Name = "iCloudTextBox"
        Me.iCloudTextBox.Size = New System.Drawing.Size(219, 23)
        Me.iCloudTextBox.TabIndex = 60
        '
        'Label17
        '
        Me.Label17.AutoSize = True
        Me.Label17.Font = New System.Drawing.Font("Microsoft Sans Serif", 9.75!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label17.Location = New System.Drawing.Point(382, 28)
        Me.Label17.Name = "Label17"
        Me.Label17.Size = New System.Drawing.Size(100, 16)
        Me.Label17.TabIndex = 61
        Me.Label17.Text = "iCloud Account:"
        '
        'ToolStripContainer1
        '
        Me.ToolStripContainer1.BottomToolStripPanelVisible = False
        '
        'ToolStripContainer1.ContentPanel
        '
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.TabControl1)
        Me.ToolStripContainer1.ContentPanel.Controls.Add(Me.Panel1)
        Me.ToolStripContainer1.ContentPanel.Size = New System.Drawing.Size(1161, 629)
        Me.ToolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill
        Me.ToolStripContainer1.LeftToolStripPanelVisible = False
        Me.ToolStripContainer1.Location = New System.Drawing.Point(0, 0)
        Me.ToolStripContainer1.Name = "ToolStripContainer1"
        Me.ToolStripContainer1.RightToolStripPanelVisible = False
        Me.ToolStripContainer1.Size = New System.Drawing.Size(1161, 740)
        Me.ToolStripContainer1.TabIndex = 54
        Me.ToolStripContainer1.Text = "ToolStripContainer1"
        '
        'ToolStripContainer1.TopToolStripPanel
        '
        Me.ToolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.Color.Silver
        Me.ToolStripContainer1.TopToolStripPanel.Controls.Add(Me.tsSaveModify)
        Me.ToolStripContainer1.TopToolStripPanel.Controls.Add(Me.tsTracking)
        Me.ToolStripContainer1.TopToolStripPanel.Controls.Add(Me.ToolStrip1)
        '
        'tsTracking
        '
        Me.tsTracking.BackColor = System.Drawing.SystemColors.Control
        Me.tsTracking.Dock = System.Windows.Forms.DockStyle.None
        Me.tsTracking.ImageScalingSize = New System.Drawing.Size(25, 25)
        Me.tsTracking.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.ToolStripDropDownButton1, Me.ToolStripSeparator4})
        Me.tsTracking.Location = New System.Drawing.Point(16, 37)
        Me.tsTracking.Name = "tsTracking"
        Me.tsTracking.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.tsTracking.Size = New System.Drawing.Size(134, 37)
        Me.tsTracking.TabIndex = 46
        '
        'ToolStripDropDownButton1
        '
        Me.ToolStripDropDownButton1.AutoSize = False
        Me.ToolStripDropDownButton1.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.CheckOutTool, Me.CheckInTool})
        Me.ToolStripDropDownButton1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ToolStripDropDownButton1.Image = Global.AssetManager.My.Resources.Resources.CheckOutIcon
        Me.ToolStripDropDownButton1.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.ToolStripDropDownButton1.Name = "ToolStripDropDownButton1"
        Me.ToolStripDropDownButton1.Padding = New System.Windows.Forms.Padding(10, 0, 0, 0)
        Me.ToolStripDropDownButton1.Size = New System.Drawing.Size(116, 34)
        Me.ToolStripDropDownButton1.Text = "Tracking"
        '
        'CheckOutTool
        '
        Me.CheckOutTool.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CheckOutTool.Image = Global.AssetManager.My.Resources.Resources.CheckedBoxRedIcon
        Me.CheckOutTool.Name = "CheckOutTool"
        Me.CheckOutTool.Size = New System.Drawing.Size(135, 22)
        Me.CheckOutTool.Text = "Check Out"
        '
        'CheckInTool
        '
        Me.CheckInTool.Font = New System.Drawing.Font("Segoe UI", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.CheckInTool.Image = Global.AssetManager.My.Resources.Resources.CheckedBoxGreenIcon
        Me.CheckInTool.Name = "CheckInTool"
        Me.CheckInTool.Size = New System.Drawing.Size(135, 22)
        Me.CheckInTool.Text = "Check In"
        '
        'ToolStripSeparator4
        '
        Me.ToolStripSeparator4.Name = "ToolStripSeparator4"
        Me.ToolStripSeparator4.Size = New System.Drawing.Size(6, 37)
        '
        'ToolStrip1
        '
        Me.ToolStrip1.BackColor = System.Drawing.SystemColors.Control
        Me.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None
        Me.ToolStrip1.Font = New System.Drawing.Font("Segoe UI", 12.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.ToolStrip1.ImageScalingSize = New System.Drawing.Size(25, 25)
        Me.ToolStrip1.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsbModify, Me.tsbNewNote, Me.tsbDeleteDevice, Me.ToolStripSeparator1, Me.AttachmentTool, Me.ToolStripSeparator7, Me.ToolStripDropDownButton2, Me.ToolStripSeparator9})
        Me.ToolStrip1.Location = New System.Drawing.Point(3, 74)
        Me.ToolStrip1.Name = "ToolStrip1"
        Me.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional
        Me.ToolStrip1.Size = New System.Drawing.Size(375, 37)
        Me.ToolStrip1.TabIndex = 45
        Me.ToolStrip1.Text = "MyToolStrip1"
        '
        'tsbModify
        '
        Me.tsbModify.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbModify.Image = Global.AssetManager.My.Resources.Resources.EditIcon
        Me.tsbModify.Name = "tsbModify"
        Me.tsbModify.Padding = New System.Windows.Forms.Padding(5, 5, 5, 0)
        Me.tsbModify.Size = New System.Drawing.Size(39, 34)
        Me.tsbModify.Text = "Modify"
        '
        'tsbNewNote
        '
        Me.tsbNewNote.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbNewNote.Image = Global.AssetManager.My.Resources.Resources.AddNoteIcon
        Me.tsbNewNote.Name = "tsbNewNote"
        Me.tsbNewNote.Padding = New System.Windows.Forms.Padding(5, 5, 5, 0)
        Me.tsbNewNote.Size = New System.Drawing.Size(39, 34)
        Me.tsbNewNote.Text = "Add Note"
        '
        'tsbDeleteDevice
        '
        Me.tsbDeleteDevice.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image
        Me.tsbDeleteDevice.Image = Global.AssetManager.My.Resources.Resources.DeleteRedIcon
        Me.tsbDeleteDevice.Name = "tsbDeleteDevice"
        Me.tsbDeleteDevice.Padding = New System.Windows.Forms.Padding(5, 5, 5, 0)
        Me.tsbDeleteDevice.Size = New System.Drawing.Size(39, 34)
        Me.tsbDeleteDevice.Text = "Delete Device"
        '
        'ToolStripSeparator1
        '
        Me.ToolStripSeparator1.Name = "ToolStripSeparator1"
        Me.ToolStripSeparator1.Size = New System.Drawing.Size(6, 37)
        '
        'AttachmentTool
        '
        Me.AttachmentTool.Image = Global.AssetManager.My.Resources.Resources.PaperClipIcon
        Me.AttachmentTool.ImageAlign = System.Drawing.ContentAlignment.MiddleRight
        Me.AttachmentTool.Name = "AttachmentTool"
        Me.AttachmentTool.Padding = New System.Windows.Forms.Padding(5, 5, 5, 0)
        Me.AttachmentTool.Size = New System.Drawing.Size(39, 34)
        '
        'ToolStripSeparator7
        '
        Me.ToolStripSeparator7.Name = "ToolStripSeparator7"
        Me.ToolStripSeparator7.Size = New System.Drawing.Size(6, 37)
        '
        'ToolStripDropDownButton2
        '
        Me.ToolStripDropDownButton2.DropDownItems.AddRange(New System.Windows.Forms.ToolStripItem() {Me.tsmAssetInputForm, Me.tsmAssetTransferForm, Me.AssetDisposalForm})
        Me.ToolStripDropDownButton2.Image = Global.AssetManager.My.Resources.Resources.FormIcon
        Me.ToolStripDropDownButton2.Name = "ToolStripDropDownButton2"
        Me.ToolStripDropDownButton2.Size = New System.Drawing.Size(189, 34)
        Me.ToolStripDropDownButton2.Text = "Asset Control Forms"
        '
        'tsmAssetInputForm
        '
        Me.tsmAssetInputForm.Image = Global.AssetManager.My.Resources.Resources.ImportIcon
        Me.tsmAssetInputForm.Name = "tsmAssetInputForm"
        Me.tsmAssetInputForm.Size = New System.Drawing.Size(230, 32)
        Me.tsmAssetInputForm.Text = "Asset Input Form"
        '
        'tsmAssetTransferForm
        '
        Me.tsmAssetTransferForm.Image = Global.AssetManager.My.Resources.Resources.TransferArrowsIcon
        Me.tsmAssetTransferForm.Name = "tsmAssetTransferForm"
        Me.tsmAssetTransferForm.Size = New System.Drawing.Size(230, 32)
        Me.tsmAssetTransferForm.Text = "Asset Transfer Form"
        '
        'AssetDisposalForm
        '
        Me.AssetDisposalForm.Image = Global.AssetManager.My.Resources.Resources.TrashIcon
        Me.AssetDisposalForm.Name = "AssetDisposalForm"
        Me.AssetDisposalForm.Size = New System.Drawing.Size(230, 32)
        Me.AssetDisposalForm.Text = "Asset Disposal Form"
        '
        'ToolStripSeparator9
        '
        Me.ToolStripSeparator9.Name = "ToolStripSeparator9"
        Me.ToolStripSeparator9.Size = New System.Drawing.Size(6, 37)
        '
        'ViewDeviceForm
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.BackColor = System.Drawing.Color.FromArgb(CType(CType(232, Byte), Integer), CType(CType(232, Byte), Integer), CType(CType(232, Byte), Integer))
        Me.ClientSize = New System.Drawing.Size(1161, 762)
        Me.Controls.Add(Me.ToolStripContainer1)
        Me.Controls.Add(Me.StatusStrip1)
        Me.DoubleBuffered = True
        Me.MinimumSize = New System.Drawing.Size(1177, 528)
        Me.Name = "ViewDeviceForm"
        Me.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen
        Me.Text = "View"
        Me.pnlOtherFunctions.ResumeLayout(False)
        Me.grpNetTools.ResumeLayout(False)
        CType(Me.cmdRestart, System.ComponentModel.ISupportInitialize).EndInit()
        Me.RightClickMenu.ResumeLayout(False)
        Me.TabControl1.ResumeLayout(False)
        Me.HistoryTab.ResumeLayout(False)
        CType(Me.DataGridHistory, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TrackingTab.ResumeLayout(False)
        CType(Me.TrackingGrid, System.ComponentModel.ISupportInitialize).EndInit()
        Me.TrackingBox.ResumeLayout(False)
        Me.TrackingBox.PerformLayout()
        Me.StatusStrip1.ResumeLayout(False)
        Me.StatusStrip1.PerformLayout()
        CType(Me.fieldErrorIcon, System.ComponentModel.ISupportInitialize).EndInit()
        Me.tsSaveModify.ResumeLayout(False)
        Me.tsSaveModify.PerformLayout()
        Me.Panel1.ResumeLayout(False)
        Me.Panel2.ResumeLayout(False)
        Me.FieldTabs.ResumeLayout(False)
        Me.AssetInfo.ResumeLayout(False)
        Me.AssetInfo.PerformLayout()
        Me.MiscInfo.ResumeLayout(False)
        Me.MiscInfo.PerformLayout()
        Me.ToolStripContainer1.ContentPanel.ResumeLayout(False)
        Me.ToolStripContainer1.TopToolStripPanel.ResumeLayout(False)
        Me.ToolStripContainer1.TopToolStripPanel.PerformLayout()
        Me.ToolStripContainer1.ResumeLayout(False)
        Me.ToolStripContainer1.PerformLayout()
        Me.tsTracking.ResumeLayout(False)
        Me.tsTracking.PerformLayout()
        Me.ToolStrip1.ResumeLayout(False)
        Me.ToolStrip1.PerformLayout()
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents Label9 As Label
    Friend WithEvents cmbStatus_REQ As ComboBox
    Friend WithEvents Label8 As Label
    Friend WithEvents cmbOSVersion_REQ As ComboBox
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
    Friend WithEvents RightClickMenu As ContextMenuStrip
    Friend WithEvents DeleteEntryToolStripMenuItem As ToolStripMenuItem
    Friend WithEvents chkTrackable As CheckBox
    Friend WithEvents TabControl1 As TabControl
    Friend WithEvents HistoryTab As TabPage
    Friend WithEvents DataGridHistory As DataGridView
    Friend WithEvents TrackingTab As TabPage
    Friend WithEvents TrackingGrid As DataGridView
    Friend WithEvents TrackingBox As GroupBox
    Friend WithEvents txtCheckOut As TextBox
    Friend WithEvents Label11 As Label
    Friend WithEvents txtCheckTime As TextBox
    Friend WithEvents lblCheckTime As Label
    Friend WithEvents txtCheckUser As TextBox
    Friend WithEvents lblCheckUser As Label
    Friend WithEvents txtCheckLocation As TextBox
    Friend WithEvents Label16 As Label
    Friend WithEvents txtDueBack As TextBox
    Friend WithEvents lblDueBack As Label
    Friend WithEvents ToolTip1 As ToolTip
    Friend WithEvents tsSaveModify As OneClickToolStrip
    Friend WithEvents ToolStripSeparator3 As ToolStripSeparator
    Friend WithEvents StatusStrip1 As StatusStrip
    Friend WithEvents StatusLabel As ToolStripStatusLabel
    Friend WithEvents cmdCancel_Tool As ToolStripButton
    Friend WithEvents cmdAccept_Tool As ToolStripButton
    Friend WithEvents cmdMunisInfo As Button
    Friend WithEvents cmdRDP As Button
    Friend WithEvents Label12 As Label
    Friend WithEvents txtPONumber As TextBox
    Friend WithEvents tmr_RDPRefresher As Timer
    Friend WithEvents cmdSibiLink As Button
    Friend WithEvents pnlOtherFunctions As Panel
    Friend WithEvents fieldErrorIcon As ErrorProvider
    Friend WithEvents cmdBrowseFiles As Button
    Friend WithEvents grpNetTools As GroupBox
    Friend WithEvents cmdMunisSearch As Button
    Friend WithEvents lblGUID As Label
    Friend WithEvents cmdShowIP As Button
    Friend WithEvents Label14 As Label
    Friend WithEvents cmdGKUpdate As Button
    Friend WithEvents txtPhoneNumber As MaskedTextBox
    Friend WithEvents Panel1 As Panel
    Friend WithEvents ToolStripContainer1 As ToolStripContainer
    Friend WithEvents ToolStrip1 As OneClickToolStrip
    Friend WithEvents tsbModify As ToolStripButton
    Friend WithEvents tsbNewNote As ToolStripButton
    Friend WithEvents tsbDeleteDevice As ToolStripButton
    Friend WithEvents AttachmentTool As ToolStripButton
    Friend WithEvents ToolStripSeparator7 As ToolStripSeparator
    Friend WithEvents ToolStripDropDownButton2 As ToolStripDropDownButton
    Friend WithEvents tsmAssetInputForm As ToolStripMenuItem
    Friend WithEvents tsmAssetTransferForm As ToolStripMenuItem
    Friend WithEvents AssetDisposalForm As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator9 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator1 As ToolStripSeparator
    Friend WithEvents tsTracking As ToolStrip
    Friend WithEvents ToolStripDropDownButton1 As ToolStripDropDownButton
    Friend WithEvents CheckOutTool As ToolStripMenuItem
    Friend WithEvents CheckInTool As ToolStripMenuItem
    Friend WithEvents ToolStripSeparator2 As ToolStripSeparator
    Friend WithEvents ToolStripSeparator4 As ToolStripSeparator
    Friend WithEvents cmdRestart As PictureBox
    Friend WithEvents txtHostname As TextBox
    Friend WithEvents Label15 As Label
    Friend WithEvents FieldTabs As TabControl
    Friend WithEvents AssetInfo As TabPage
    Friend WithEvents MiscInfo As TabPage
    Friend WithEvents Panel2 As Panel
    Friend WithEvents iCloudTextBox As TextBox
    Friend WithEvents Label17 As Label
End Class
