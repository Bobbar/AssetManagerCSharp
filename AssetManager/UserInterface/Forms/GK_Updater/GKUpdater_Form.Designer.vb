﻿<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class GKUpdater_Form
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
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
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Dim resources As System.ComponentModel.ComponentResourceManager = New System.ComponentModel.ComponentResourceManager(GetType(GKUpdater_Form))
        Me.Updater_Table = New System.Windows.Forms.TableLayoutPanel()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.GroupBox3 = New System.Windows.Forms.GroupBox()
        Me.Label1 = New System.Windows.Forms.Label()
        Me.MaxUpdates = New System.Windows.Forms.NumericUpDown()
        Me.cmdPauseResume = New System.Windows.Forms.Button()
        Me.lblTotUpdates = New System.Windows.Forms.Label()
        Me.lblComplete = New System.Windows.Forms.Label()
        Me.lblRunning = New System.Windows.Forms.Label()
        Me.lblQueued = New System.Windows.Forms.Label()
        Me.QueueChecker = New System.Windows.Forms.Timer(Me.components)
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.GroupBox3.SuspendLayout()
        CType(Me.MaxUpdates, System.ComponentModel.ISupportInitialize).BeginInit()
        Me.SuspendLayout()
        '
        'Updater_Table
        '
        Me.Updater_Table.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Updater_Table.AutoScroll = True
        Me.Updater_Table.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink
        Me.Updater_Table.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.InsetDouble
        Me.Updater_Table.ColumnCount = 2
        Me.Updater_Table.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.Updater_Table.ColumnStyles.Add(New System.Windows.Forms.ColumnStyle())
        Me.Updater_Table.Location = New System.Drawing.Point(7, 20)
        Me.Updater_Table.Name = "Updater_Table"
        Me.Updater_Table.RowCount = 1
        Me.Updater_Table.RowStyles.Add(New System.Windows.Forms.RowStyle())
        Me.Updater_Table.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 562.0!))
        Me.Updater_Table.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 562.0!))
        Me.Updater_Table.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 562.0!))
        Me.Updater_Table.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 562.0!))
        Me.Updater_Table.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 562.0!))
        Me.Updater_Table.RowStyles.Add(New System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 562.0!))
        Me.Updater_Table.Size = New System.Drawing.Size(850, 565)
        Me.Updater_Table.TabIndex = 1
        '
        'GroupBox1
        '
        Me.GroupBox1.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.GroupBox1.Controls.Add(Me.Updater_Table)
        Me.GroupBox1.Location = New System.Drawing.Point(392, 13)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(864, 592)
        Me.GroupBox1.TabIndex = 2
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "Current Updates"
        '
        'GroupBox2
        '
        Me.GroupBox2.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left), System.Windows.Forms.AnchorStyles)
        Me.GroupBox2.Controls.Add(Me.GroupBox3)
        Me.GroupBox2.Location = New System.Drawing.Point(14, 13)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(369, 592)
        Me.GroupBox2.TabIndex = 3
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "Manage Updates"
        '
        'GroupBox3
        '
        Me.GroupBox3.Controls.Add(Me.Label1)
        Me.GroupBox3.Controls.Add(Me.MaxUpdates)
        Me.GroupBox3.Controls.Add(Me.cmdPauseResume)
        Me.GroupBox3.Controls.Add(Me.lblTotUpdates)
        Me.GroupBox3.Controls.Add(Me.lblComplete)
        Me.GroupBox3.Controls.Add(Me.lblRunning)
        Me.GroupBox3.Controls.Add(Me.lblQueued)
        Me.GroupBox3.Location = New System.Drawing.Point(17, 33)
        Me.GroupBox3.Name = "GroupBox3"
        Me.GroupBox3.Size = New System.Drawing.Size(332, 185)
        Me.GroupBox3.TabIndex = 0
        Me.GroupBox3.TabStop = False
        Me.GroupBox3.Text = "Queue Control"
        '
        'Label1
        '
        Me.Label1.AutoSize = True
        Me.Label1.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Label1.Location = New System.Drawing.Point(219, 28)
        Me.Label1.Name = "Label1"
        Me.Label1.Size = New System.Drawing.Size(105, 14)
        Me.Label1.TabIndex = 6
        Me.Label1.Text = "Max Concurrent"
        '
        'MaxUpdates
        '
        Me.MaxUpdates.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.MaxUpdates.Location = New System.Drawing.Point(223, 44)
        Me.MaxUpdates.Maximum = New Decimal(New Integer() {10, 0, 0, 0})
        Me.MaxUpdates.Minimum = New Decimal(New Integer() {1, 0, 0, 0})
        Me.MaxUpdates.Name = "MaxUpdates"
        Me.MaxUpdates.Size = New System.Drawing.Size(92, 22)
        Me.MaxUpdates.TabIndex = 5
        Me.MaxUpdates.Value = New Decimal(New Integer() {1, 0, 0, 0})
        '
        'cmdPauseResume
        '
        Me.cmdPauseResume.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.cmdPauseResume.Location = New System.Drawing.Point(103, 139)
        Me.cmdPauseResume.Name = "cmdPauseResume"
        Me.cmdPauseResume.Size = New System.Drawing.Size(120, 27)
        Me.cmdPauseResume.TabIndex = 4
        Me.cmdPauseResume.Text = "Pause Queue"
        Me.cmdPauseResume.UseVisualStyleBackColor = True
        '
        'lblTotUpdates
        '
        Me.lblTotUpdates.AutoSize = True
        Me.lblTotUpdates.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblTotUpdates.Location = New System.Drawing.Point(37, 28)
        Me.lblTotUpdates.Name = "lblTotUpdates"
        Me.lblTotUpdates.Size = New System.Drawing.Size(84, 14)
        Me.lblTotUpdates.TabIndex = 3
        Me.lblTotUpdates.Text = "[# Updates]"
        '
        'lblComplete
        '
        Me.lblComplete.AutoSize = True
        Me.lblComplete.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblComplete.Location = New System.Drawing.Point(37, 100)
        Me.lblComplete.Name = "lblComplete"
        Me.lblComplete.Size = New System.Drawing.Size(77, 14)
        Me.lblComplete.TabIndex = 2
        Me.lblComplete.Text = "[Complete]"
        '
        'lblRunning
        '
        Me.lblRunning.AutoSize = True
        Me.lblRunning.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblRunning.Location = New System.Drawing.Point(37, 75)
        Me.lblRunning.Name = "lblRunning"
        Me.lblRunning.Size = New System.Drawing.Size(70, 14)
        Me.lblRunning.TabIndex = 1
        Me.lblRunning.Text = "[Running]"
        '
        'lblQueued
        '
        Me.lblQueued.AutoSize = True
        Me.lblQueued.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.lblQueued.Location = New System.Drawing.Point(37, 52)
        Me.lblQueued.Name = "lblQueued"
        Me.lblQueued.Size = New System.Drawing.Size(63, 14)
        Me.lblQueued.TabIndex = 0
        Me.lblQueued.Text = "[Queued]"
        '
        'QueueChecker
        '
        Me.QueueChecker.Enabled = True
        Me.QueueChecker.Interval = 500
        '
        'GKUpdater_Form
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(7.0!, 14.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(1270, 618)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.GroupBox1)
        Me.DoubleBuffered = True
        Me.Font = New System.Drawing.Font("Consolas", 9.0!, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, CType(0, Byte))
        Me.Icon = CType(resources.GetObject("$this.Icon"), System.Drawing.Icon)
        Me.MinimumSize = New System.Drawing.Size(1010, 428)
        Me.Name = "GKUpdater_Form"
        Me.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show
        Me.Text = "Gatekeeper Updater"
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox3.ResumeLayout(False)
        Me.GroupBox3.PerformLayout()
        CType(Me.MaxUpdates, System.ComponentModel.ISupportInitialize).EndInit()
        Me.ResumeLayout(False)

    End Sub
    Friend WithEvents Updater_Table As TableLayoutPanel
    Friend WithEvents GroupBox1 As GroupBox
    Friend WithEvents GroupBox2 As GroupBox
    Friend WithEvents QueueChecker As Timer
    Friend WithEvents GroupBox3 As GroupBox
    Friend WithEvents lblQueued As Label
    Friend WithEvents lblRunning As Label
    Friend WithEvents lblComplete As Label
    Friend WithEvents lblTotUpdates As Label
    Friend WithEvents cmdPauseResume As Button
    Friend WithEvents Label1 As Label
    Friend WithEvents MaxUpdates As NumericUpDown
End Class
