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
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.ProgressControlsTable = new System.Windows.Forms.FlowLayoutPanel();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.MaxUpdates = new System.Windows.Forms.NumericUpDown();
            this.CancelAllButton = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.PauseResumeButton = new System.Windows.Forms.Button();
            this.TotalUpdatesLabel = new System.Windows.Forms.Label();
            this.CompleteUpdatesLabel = new System.Windows.Forms.Label();
            this.RunningUpdatesLabel = new System.Windows.Forms.Label();
            this.QueuedUpdatesLabel = new System.Windows.Forms.Label();
            this.QueueChecker = new System.Windows.Forms.Timer(this.components);
            this.TransferRateLabel = new System.Windows.Forms.Label();
            this.SortButton = new System.Windows.Forms.Button();
            this.MenuStrip = new System.Windows.Forms.MenuStrip();
            this.OptionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CreateDirsToolItem = new System.Windows.Forms.ToolStripMenuItem();
            this.FunctionsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.VerifyPackFileToolItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GroupBox1.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUpdates)).BeginInit();
            this.MenuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.BackColor = System.Drawing.SystemColors.Control;
            this.GroupBox1.Controls.Add(this.ProgressControlsTable);
            this.GroupBox1.Location = new System.Drawing.Point(12, 114);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(1070, 504);
            this.GroupBox1.TabIndex = 2;
            this.GroupBox1.TabStop = false;
            // 
            // ProgressControlsTable
            // 
            this.ProgressControlsTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ProgressControlsTable.AutoScroll = true;
            this.ProgressControlsTable.BackColor = System.Drawing.SystemColors.Control;
            this.ProgressControlsTable.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.ProgressControlsTable.Location = new System.Drawing.Point(9, 21);
            this.ProgressControlsTable.Margin = new System.Windows.Forms.Padding(10);
            this.ProgressControlsTable.Name = "ProgressControlsTable";
            this.ProgressControlsTable.Size = new System.Drawing.Size(1053, 473);
            this.ProgressControlsTable.TabIndex = 2;
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
            this.GroupBox3.Location = new System.Drawing.Point(12, 36);
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
            10,
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
            // QueueChecker
            // 
            this.QueueChecker.Interval = 250;
            this.QueueChecker.Tick += new System.EventHandler(this.QueueChecker_Tick);
            // 
            // TransferRateLabel
            // 
            this.TransferRateLabel.AutoSize = true;
            this.TransferRateLabel.Location = new System.Drawing.Point(695, 68);
            this.TransferRateLabel.Name = "TransferRateLabel";
            this.TransferRateLabel.Size = new System.Drawing.Size(112, 14);
            this.TransferRateLabel.TabIndex = 3;
            this.TransferRateLabel.Text = "[Transfer Rate]";
            // 
            // SortButton
            // 
            this.SortButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.SortButton.Location = new System.Drawing.Point(996, 91);
            this.SortButton.Name = "SortButton";
            this.SortButton.Size = new System.Drawing.Size(86, 24);
            this.SortButton.TabIndex = 4;
            this.SortButton.Text = "Sort";
            this.SortButton.UseVisualStyleBackColor = true;
            this.SortButton.Click += new System.EventHandler(this.SortButton_Click);
            // 
            // MenuStrip
            // 
            this.MenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OptionsToolStripMenuItem,
            this.FunctionsToolStripMenuItem});
            this.MenuStrip.Location = new System.Drawing.Point(0, 0);
            this.MenuStrip.Name = "MenuStrip";
            this.MenuStrip.Size = new System.Drawing.Size(1094, 24);
            this.MenuStrip.TabIndex = 5;
            this.MenuStrip.Text = "MenuStrip1";
            // 
            // OptionsToolStripMenuItem
            // 
            this.OptionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateDirsToolItem});
            this.OptionsToolStripMenuItem.Name = "OptionsToolStripMenuItem";
            this.OptionsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.OptionsToolStripMenuItem.Text = "Options";
            // 
            // CreateDirsToolItem
            // 
            this.CreateDirsToolItem.CheckOnClick = true;
            this.CreateDirsToolItem.Name = "CreateDirsToolItem";
            this.CreateDirsToolItem.Size = new System.Drawing.Size(211, 22);
            this.CreateDirsToolItem.Text = "Create Missing Directories";
            this.CreateDirsToolItem.Click += new System.EventHandler(this.CreateDirsToolItem_Click);
            // 
            // FunctionsToolStripMenuItem
            // 
            this.FunctionsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.VerifyPackFileToolItem});
            this.FunctionsToolStripMenuItem.Name = "FunctionsToolStripMenuItem";
            this.FunctionsToolStripMenuItem.Size = new System.Drawing.Size(71, 20);
            this.FunctionsToolStripMenuItem.Text = "Functions";
            // 
            // VerifyPackFileToolItem
            // 
            this.VerifyPackFileToolItem.Name = "VerifyPackFileToolItem";
            this.VerifyPackFileToolItem.Size = new System.Drawing.Size(182, 22);
            this.VerifyPackFileToolItem.Text = "Manage GK Package";
            this.VerifyPackFileToolItem.Click += new System.EventHandler(this.VerifyPackFileToolItem_Click);
            // 
            // GKUpdaterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1094, 638);
            this.Controls.Add(this.SortButton);
            this.Controls.Add(this.TransferRateLabel);
            this.Controls.Add(this.GroupBox3);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.MenuStrip);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.MenuStrip;
            this.MinimumSize = new System.Drawing.Size(1010, 428);
            this.Name = "GKUpdaterForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Gatekeeper Updater";
            this.Shown += new System.EventHandler(this.GKUpdaterForm_Shown);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MaxUpdates)).EndInit();
            this.MenuStrip.ResumeLayout(false);
            this.MenuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal GroupBox GroupBox1;
        internal Timer QueueChecker;
        internal GroupBox GroupBox3;
        internal Label QueuedUpdatesLabel;
        internal Label RunningUpdatesLabel;
        internal Label CompleteUpdatesLabel;
        internal Label TotalUpdatesLabel;
        internal Button PauseResumeButton;
        internal Label Label1;
        internal NumericUpDown MaxUpdates;
        internal Button CancelAllButton;
        internal FlowLayoutPanel ProgressControlsTable;
        internal Label TransferRateLabel;
        internal Button SortButton;
        internal MenuStrip MenuStrip;
        internal ToolStripMenuItem OptionsToolStripMenuItem;
        internal ToolStripMenuItem CreateDirsToolItem;
        internal ToolStripMenuItem FunctionsToolStripMenuItem;
        internal ToolStripMenuItem VerifyPackFileToolItem;
    }
}
