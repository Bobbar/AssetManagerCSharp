using System.Windows.Forms;
namespace AssetManager.UserInterface.Forms.Gatekeeper
{

    partial class GKUpdaterForm
    {
        //Required by the Windows Form Designer

        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.UpdateLogSplitter = new AssetManager.UserInterface.CustomControls.HotTrackSplitContainer();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.StatusGrid = new System.Windows.Forms.DataGridView();
            this.PopupMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ViewDeviceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.LogTextBox = new System.Windows.Forms.RichTextBox();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.MaxUpdates = new System.Windows.Forms.NumericUpDown();
            this.CancelAllButton = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.PauseResumeButton = new System.Windows.Forms.Button();
            this.TotalUpdatesLabel = new System.Windows.Forms.Label();
            this.CompleteUpdatesLabel = new System.Windows.Forms.Label();
            this.RunningUpdatesLabel = new System.Windows.Forms.Label();
            this.QueuedUpdatesLabel = new System.Windows.Forms.Label();
            this.StatusLightCol = new System.Windows.Forms.DataGridViewImageColumn();
            this.TargetNameCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StatusTextCol = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StartRestartCol = new System.Windows.Forms.DataGridViewButtonColumn();
            this.CancelRemoveCol = new System.Windows.Forms.DataGridViewButtonColumn();
            ((System.ComponentModel.ISupportInitialize)(this.UpdateLogSplitter)).BeginInit();
            this.UpdateLogSplitter.Panel1.SuspendLayout();
            this.UpdateLogSplitter.Panel2.SuspendLayout();
            this.UpdateLogSplitter.SuspendLayout();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StatusGrid)).BeginInit();
            this.PopupMenu.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUpdates)).BeginInit();
            this.SuspendLayout();
            // 
            // UpdateLogSplitter
            // 
            this.UpdateLogSplitter.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.UpdateLogSplitter.HotTracking = true;
            this.UpdateLogSplitter.Location = new System.Drawing.Point(12, 97);
            this.UpdateLogSplitter.Name = "UpdateLogSplitter";
            // 
            // UpdateLogSplitter.Panel1
            // 
            this.UpdateLogSplitter.Panel1.Controls.Add(this.groupBox4);
            // 
            // UpdateLogSplitter.Panel2
            // 
            this.UpdateLogSplitter.Panel2.Controls.Add(this.groupBox2);
            this.UpdateLogSplitter.Size = new System.Drawing.Size(1150, 523);
            this.UpdateLogSplitter.SplitterDistance = 723;
            this.UpdateLogSplitter.TabIndex = 4;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.StatusGrid);
            this.groupBox4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox4.Location = new System.Drawing.Point(0, 3);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(720, 517);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Updates";
            // 
            // StatusGrid
            // 
            this.StatusGrid.AllowUserToAddRows = false;
            this.StatusGrid.AllowUserToDeleteRows = false;
            this.StatusGrid.AllowUserToResizeRows = false;
            this.StatusGrid.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.StatusGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.ControlDark;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.StatusGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.StatusGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.StatusGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.StatusLightCol,
            this.TargetNameCol,
            this.StatusTextCol,
            this.StartRestartCol,
            this.CancelRemoveCol});
            this.StatusGrid.ContextMenuStrip = this.PopupMenu;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.StatusGrid.DefaultCellStyle = dataGridViewCellStyle4;
            this.StatusGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.StatusGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.StatusGrid.Location = new System.Drawing.Point(3, 18);
            this.StatusGrid.Name = "StatusGrid";
            this.StatusGrid.ReadOnly = true;
            this.StatusGrid.RowHeadersVisible = false;
            this.StatusGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.StatusGrid.Size = new System.Drawing.Size(714, 496);
            this.StatusGrid.TabIndex = 3;
            this.StatusGrid.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.StatusGrid_CellContentClick);
            this.StatusGrid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.StatusGrid_CellMouseDown);
            this.StatusGrid.ColumnHeaderMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.StatusGrid_ColumnHeaderMouseClick);
            this.StatusGrid.DataBindingComplete += new System.Windows.Forms.DataGridViewBindingCompleteEventHandler(this.StatusGrid_DataBindingComplete);
            this.StatusGrid.SelectionChanged += new System.EventHandler(this.StatusGrid_SelectionChanged);
            // 
            // PopupMenu
            // 
            this.PopupMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewDeviceMenuItem});
            this.PopupMenu.Name = "PopupMenu";
            this.PopupMenu.Size = new System.Drawing.Size(138, 26);
            // 
            // ViewDeviceMenuItem
            // 
            this.ViewDeviceMenuItem.Image = global::AssetManager.Properties.Resources.ViewIcon3;
            this.ViewDeviceMenuItem.Name = "ViewDeviceMenuItem";
            this.ViewDeviceMenuItem.Size = new System.Drawing.Size(137, 22);
            this.ViewDeviceMenuItem.Text = "View Device";
            this.ViewDeviceMenuItem.Click += new System.EventHandler(this.ViewDeviceMenuItem_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.LogTextBox);
            this.groupBox2.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.groupBox2.Location = new System.Drawing.Point(3, 3);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(417, 517);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Selected Log";
            // 
            // LogTextBox
            // 
            this.LogTextBox.BackColor = System.Drawing.Color.White;
            this.LogTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.LogTextBox.DetectUrls = false;
            this.LogTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.LogTextBox.HideSelection = false;
            this.LogTextBox.Location = new System.Drawing.Point(3, 18);
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.ShortcutsEnabled = false;
            this.LogTextBox.Size = new System.Drawing.Size(411, 496);
            this.LogTextBox.TabIndex = 0;
            this.LogTextBox.Text = "";
            this.LogTextBox.WordWrap = false;
            this.LogTextBox.TextChanged += new System.EventHandler(this.LogTextBox_TextChanged);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.MaxUpdates);
            this.GroupBox3.Controls.Add(this.CancelAllButton);
            this.GroupBox3.Controls.Add(this.Label1);
            this.GroupBox3.Controls.Add(this.PauseResumeButton);
            this.GroupBox3.Controls.Add(this.TotalUpdatesLabel);
            this.GroupBox3.Controls.Add(this.CompleteUpdatesLabel);
            this.GroupBox3.Controls.Add(this.RunningUpdatesLabel);
            this.GroupBox3.Controls.Add(this.QueuedUpdatesLabel);
            this.GroupBox3.Location = new System.Drawing.Point(12, 12);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(577, 79);
            this.GroupBox3.TabIndex = 0;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Queue Control";
            // 
            // MaxUpdates
            // 
            this.MaxUpdates.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaxUpdates.Location = new System.Drawing.Point(302, 40);
            this.MaxUpdates.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.MaxUpdates.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MaxUpdates.Name = "MaxUpdates";
            this.MaxUpdates.Size = new System.Drawing.Size(59, 22);
            this.MaxUpdates.TabIndex = 5;
            this.MaxUpdates.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.MaxUpdates.ValueChanged += new System.EventHandler(this.MaxUpdates_ValueChanged);
            // 
            // CancelAllButton
            // 
            this.CancelAllButton.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelAllButton.Location = new System.Drawing.Point(441, 45);
            this.CancelAllButton.Name = "CancelAllButton";
            this.CancelAllButton.Size = new System.Drawing.Size(120, 27);
            this.CancelAllButton.TabIndex = 7;
            this.CancelAllButton.Text = "Cancel All";
            this.CancelAllButton.UseVisualStyleBackColor = true;
            this.CancelAllButton.Click += new System.EventHandler(this.CancelAllButton_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(277, 18);
            this.Label1.Name = "Label1";
            this.Label1.Padding = new System.Windows.Forms.Padding(5);
            this.Label1.Size = new System.Drawing.Size(115, 24);
            this.Label1.TabIndex = 6;
            this.Label1.Text = "Max Concurrent";
            // 
            // PauseResumeButton
            // 
            this.PauseResumeButton.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PauseResumeButton.Location = new System.Drawing.Point(441, 12);
            this.PauseResumeButton.Name = "PauseResumeButton";
            this.PauseResumeButton.Size = new System.Drawing.Size(120, 27);
            this.PauseResumeButton.TabIndex = 4;
            this.PauseResumeButton.Text = "Pause Queue";
            this.PauseResumeButton.UseVisualStyleBackColor = true;
            this.PauseResumeButton.Click += new System.EventHandler(this.PauseResumeButton_Click);
            // 
            // TotalUpdatesLabel
            // 
            this.TotalUpdatesLabel.AutoSize = true;
            this.TotalUpdatesLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TotalUpdatesLabel.Location = new System.Drawing.Point(6, 18);
            this.TotalUpdatesLabel.Name = "TotalUpdatesLabel";
            this.TotalUpdatesLabel.Padding = new System.Windows.Forms.Padding(5);
            this.TotalUpdatesLabel.Size = new System.Drawing.Size(94, 24);
            this.TotalUpdatesLabel.TabIndex = 3;
            this.TotalUpdatesLabel.Text = "[# Updates]";
            // 
            // CompleteUpdatesLabel
            // 
            this.CompleteUpdatesLabel.AutoSize = true;
            this.CompleteUpdatesLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CompleteUpdatesLabel.Location = new System.Drawing.Point(145, 42);
            this.CompleteUpdatesLabel.Name = "CompleteUpdatesLabel";
            this.CompleteUpdatesLabel.Padding = new System.Windows.Forms.Padding(5);
            this.CompleteUpdatesLabel.Size = new System.Drawing.Size(87, 24);
            this.CompleteUpdatesLabel.TabIndex = 2;
            this.CompleteUpdatesLabel.Text = "[Complete]";
            // 
            // RunningUpdatesLabel
            // 
            this.RunningUpdatesLabel.AutoSize = true;
            this.RunningUpdatesLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RunningUpdatesLabel.Location = new System.Drawing.Point(145, 18);
            this.RunningUpdatesLabel.Name = "RunningUpdatesLabel";
            this.RunningUpdatesLabel.Padding = new System.Windows.Forms.Padding(5);
            this.RunningUpdatesLabel.Size = new System.Drawing.Size(80, 24);
            this.RunningUpdatesLabel.TabIndex = 1;
            this.RunningUpdatesLabel.Text = "[Running]";
            // 
            // QueuedUpdatesLabel
            // 
            this.QueuedUpdatesLabel.AutoSize = true;
            this.QueuedUpdatesLabel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.QueuedUpdatesLabel.Location = new System.Drawing.Point(6, 42);
            this.QueuedUpdatesLabel.Name = "QueuedUpdatesLabel";
            this.QueuedUpdatesLabel.Padding = new System.Windows.Forms.Padding(5);
            this.QueuedUpdatesLabel.Size = new System.Drawing.Size(73, 24);
            this.QueuedUpdatesLabel.TabIndex = 0;
            this.QueuedUpdatesLabel.Text = "[Queued]";
            // 
            // StatusLightCol
            // 
            this.StatusLightCol.HeaderText = "";
            this.StatusLightCol.Name = "StatusLightCol";
            this.StatusLightCol.ReadOnly = true;
            this.StatusLightCol.Width = 25;
            // 
            // TargetNameCol
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.TargetNameCol.DefaultCellStyle = dataGridViewCellStyle2;
            this.TargetNameCol.HeaderText = "Target";
            this.TargetNameCol.Name = "TargetNameCol";
            this.TargetNameCol.ReadOnly = true;
            this.TargetNameCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.TargetNameCol.Width = 200;
            // 
            // StatusTextCol
            // 
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.StatusTextCol.DefaultCellStyle = dataGridViewCellStyle3;
            this.StatusTextCol.HeaderText = "Status";
            this.StatusTextCol.Name = "StatusTextCol";
            this.StatusTextCol.ReadOnly = true;
            this.StatusTextCol.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Programmatic;
            this.StatusTextCol.Width = 310;
            // 
            // StartRestartCol
            // 
            this.StartRestartCol.HeaderText = "";
            this.StartRestartCol.Name = "StartRestartCol";
            this.StartRestartCol.ReadOnly = true;
            this.StartRestartCol.Width = 70;
            // 
            // CancelRemoveCol
            // 
            this.CancelRemoveCol.HeaderText = "";
            this.CancelRemoveCol.Name = "CancelRemoveCol";
            this.CancelRemoveCol.ReadOnly = true;
            this.CancelRemoveCol.Width = 70;
            // 
            // GKUpdaterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1174, 643);
            this.Controls.Add(this.UpdateLogSplitter);
            this.Controls.Add(this.GroupBox3);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(1010, 428);
            this.Name = "GKUpdaterForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gatekeeper Updater";
            this.Shown += new System.EventHandler(this.GKUpdaterForm_Shown);
            this.UpdateLogSplitter.Panel1.ResumeLayout(false);
            this.UpdateLogSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.UpdateLogSplitter)).EndInit();
            this.UpdateLogSplitter.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.StatusGrid)).EndInit();
            this.PopupMenu.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUpdates)).EndInit();
            this.ResumeLayout(false);

        }
        internal GroupBox GroupBox3;
        internal Label QueuedUpdatesLabel;
        internal Label RunningUpdatesLabel;
        internal Label CompleteUpdatesLabel;
        internal Label TotalUpdatesLabel;
        internal Button PauseResumeButton;
        internal Label Label1;
        internal NumericUpDown MaxUpdates;
        internal Button CancelAllButton;
        private DataGridView StatusGrid;
        private CustomControls.HotTrackSplitContainer UpdateLogSplitter;
        private GroupBox groupBox2;
        private RichTextBox LogTextBox;
        private GroupBox groupBox4;
        private ContextMenuStrip PopupMenu;
        private ToolStripMenuItem ViewDeviceMenuItem;
        private DataGridViewImageColumn StatusLightCol;
        private DataGridViewTextBoxColumn TargetNameCol;
        private DataGridViewTextBoxColumn StatusTextCol;
        private DataGridViewButtonColumn StartRestartCol;
        private DataGridViewButtonColumn CancelRemoveCol;
    }
}
