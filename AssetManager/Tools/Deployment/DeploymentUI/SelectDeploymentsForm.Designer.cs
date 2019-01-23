namespace AssetManager.Tools.Deployment
{
    partial class SelectDeploymentsForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.StartButton = new System.Windows.Forms.Button();
            this.CancelButton = new System.Windows.Forms.Button();
            this.hotTrackSplitContainer1 = new AssetManager.UserInterface.CustomControls.HotTrackSplitContainer();
            this.ChooseGroupBox = new System.Windows.Forms.GroupBox();
            this.SelectListBox = new System.Windows.Forms.ListView();
            this.DeploymentsHeader = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.tableLayoutPanel2 = new System.Windows.Forms.TableLayoutPanel();
            this.SelectNoneButton = new System.Windows.Forms.Button();
            this.SelectAllButton = new System.Windows.Forms.Button();
            this.DescriptionGroup = new System.Windows.Forms.GroupBox();
            this.DescriptionTextBox = new System.Windows.Forms.RichTextBox();
            ((System.ComponentModel.ISupportInitialize)(this.hotTrackSplitContainer1)).BeginInit();
            this.hotTrackSplitContainer1.Panel1.SuspendLayout();
            this.hotTrackSplitContainer1.Panel2.SuspendLayout();
            this.hotTrackSplitContainer1.SuspendLayout();
            this.ChooseGroupBox.SuspendLayout();
            this.tableLayoutPanel2.SuspendLayout();
            this.DescriptionGroup.SuspendLayout();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.StartButton.Location = new System.Drawing.Point(536, 502);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(104, 42);
            this.StartButton.TabIndex = 1;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = true;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // CancelButton
            // 
            this.CancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelButton.Location = new System.Drawing.Point(646, 502);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(135, 42);
            this.CancelButton.TabIndex = 2;
            this.CancelButton.Text = "Cancel";
            this.CancelButton.UseVisualStyleBackColor = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // hotTrackSplitContainer1
            // 
            this.hotTrackSplitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hotTrackSplitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.hotTrackSplitContainer1.HotTracking = false;
            this.hotTrackSplitContainer1.Location = new System.Drawing.Point(12, 12);
            this.hotTrackSplitContainer1.Name = "hotTrackSplitContainer1";
            // 
            // hotTrackSplitContainer1.Panel1
            // 
            this.hotTrackSplitContainer1.Panel1.Controls.Add(this.ChooseGroupBox);
            // 
            // hotTrackSplitContainer1.Panel2
            // 
            this.hotTrackSplitContainer1.Panel2.Controls.Add(this.DescriptionGroup);
            this.hotTrackSplitContainer1.Size = new System.Drawing.Size(769, 481);
            this.hotTrackSplitContainer1.SplitterDistance = 341;
            this.hotTrackSplitContainer1.TabIndex = 3;
            // 
            // ChooseGroupBox
            // 
            this.ChooseGroupBox.Controls.Add(this.SelectListBox);
            this.ChooseGroupBox.Controls.Add(this.tableLayoutPanel2);
            this.ChooseGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ChooseGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ChooseGroupBox.Name = "ChooseGroupBox";
            this.ChooseGroupBox.Size = new System.Drawing.Size(341, 481);
            this.ChooseGroupBox.TabIndex = 1;
            this.ChooseGroupBox.TabStop = false;
            this.ChooseGroupBox.Text = "Select Items To Deploy";
            // 
            // SelectListBox
            // 
            this.SelectListBox.Activation = System.Windows.Forms.ItemActivation.TwoClick;
            this.SelectListBox.Alignment = System.Windows.Forms.ListViewAlignment.Left;
            this.SelectListBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectListBox.AutoArrange = false;
            this.SelectListBox.CheckBoxes = true;
            this.SelectListBox.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.DeploymentsHeader});
            this.SelectListBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SelectListBox.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.SelectListBox.Location = new System.Drawing.Point(7, 22);
            this.SelectListBox.MultiSelect = false;
            this.SelectListBox.Name = "SelectListBox";
            this.SelectListBox.ShowGroups = false;
            this.SelectListBox.Size = new System.Drawing.Size(326, 415);
            this.SelectListBox.TabIndex = 2;
            this.SelectListBox.UseCompatibleStateImageBehavior = false;
            this.SelectListBox.View = System.Windows.Forms.View.Details;
            this.SelectListBox.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.SelectListBox_ItemChecked);
            this.SelectListBox.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.SelectListBox_ItemSelectionChanged);
            // 
            // DeploymentsHeader
            // 
            this.DeploymentsHeader.Text = "Deployments";
            this.DeploymentsHeader.Width = 61;
            // 
            // tableLayoutPanel2
            // 
            this.tableLayoutPanel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel2.ColumnCount = 2;
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Controls.Add(this.SelectNoneButton, 0, 0);
            this.tableLayoutPanel2.Controls.Add(this.SelectAllButton, 1, 0);
            this.tableLayoutPanel2.Location = new System.Drawing.Point(7, 440);
            this.tableLayoutPanel2.Name = "tableLayoutPanel2";
            this.tableLayoutPanel2.RowCount = 1;
            this.tableLayoutPanel2.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel2.Size = new System.Drawing.Size(326, 35);
            this.tableLayoutPanel2.TabIndex = 1;
            // 
            // SelectNoneButton
            // 
            this.SelectNoneButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectNoneButton.Location = new System.Drawing.Point(3, 3);
            this.SelectNoneButton.Name = "SelectNoneButton";
            this.SelectNoneButton.Size = new System.Drawing.Size(157, 29);
            this.SelectNoneButton.TabIndex = 1;
            this.SelectNoneButton.Text = "Select None";
            this.SelectNoneButton.UseVisualStyleBackColor = true;
            this.SelectNoneButton.Click += new System.EventHandler(this.SelectNoneButton_Click);
            // 
            // SelectAllButton
            // 
            this.SelectAllButton.Dock = System.Windows.Forms.DockStyle.Fill;
            this.SelectAllButton.Location = new System.Drawing.Point(166, 3);
            this.SelectAllButton.Name = "SelectAllButton";
            this.SelectAllButton.Size = new System.Drawing.Size(157, 29);
            this.SelectAllButton.TabIndex = 0;
            this.SelectAllButton.Text = "Select All";
            this.SelectAllButton.UseVisualStyleBackColor = true;
            this.SelectAllButton.Click += new System.EventHandler(this.SelectAllButton_Click);
            // 
            // DescriptionGroup
            // 
            this.DescriptionGroup.Controls.Add(this.DescriptionTextBox);
            this.DescriptionGroup.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DescriptionGroup.Location = new System.Drawing.Point(0, 0);
            this.DescriptionGroup.Name = "DescriptionGroup";
            this.DescriptionGroup.Size = new System.Drawing.Size(424, 481);
            this.DescriptionGroup.TabIndex = 2;
            this.DescriptionGroup.TabStop = false;
            this.DescriptionGroup.Text = "Description";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DescriptionTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.DescriptionTextBox.Location = new System.Drawing.Point(8, 22);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.ReadOnly = true;
            this.DescriptionTextBox.Size = new System.Drawing.Size(408, 450);
            this.DescriptionTextBox.TabIndex = 0;
            this.DescriptionTextBox.Text = "";
            // 
            // SelectDeploymentsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(793, 556);
            this.Controls.Add(this.hotTrackSplitContainer1);
            this.Controls.Add(this.CancelButton);
            this.Controls.Add(this.StartButton);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SelectDeploymentsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Select Deployments";
            this.hotTrackSplitContainer1.Panel1.ResumeLayout(false);
            this.hotTrackSplitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.hotTrackSplitContainer1)).EndInit();
            this.hotTrackSplitContainer1.ResumeLayout(false);
            this.ChooseGroupBox.ResumeLayout(false);
            this.tableLayoutPanel2.ResumeLayout(false);
            this.DescriptionGroup.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button CancelButton;
        private UserInterface.CustomControls.HotTrackSplitContainer hotTrackSplitContainer1;
        private System.Windows.Forms.GroupBox ChooseGroupBox;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel2;
        private System.Windows.Forms.Button SelectNoneButton;
        private System.Windows.Forms.Button SelectAllButton;
        private System.Windows.Forms.GroupBox DescriptionGroup;
        private System.Windows.Forms.RichTextBox DescriptionTextBox;
        private System.Windows.Forms.ListView SelectListBox;
        private System.Windows.Forms.ColumnHeader DeploymentsHeader;
    }
}