using AssetManager.UserInterface.CustomControls;

namespace AssetManager.UserInterface.Forms.AssetManagement
{
    //
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        //
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.ResultGrid = new System.Windows.Forms.DataGridView();
            this.ContextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.ViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AddGKUpdateMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.SendToGridFormMemuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.CopySelectedMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RecordsCountLabel = new System.Windows.Forms.Label();
            this.InstantSearchGroupBox = new System.Windows.Forms.GroupBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.AssetTagTextBox = new System.Windows.Forms.TextBox();
            this.SerialTextBox = new System.Windows.Forms.TextBox();
            this.SearchGroupBox = new System.Windows.Forms.GroupBox();
            this.SearchPanel = new System.Windows.Forms.Panel();
            this.HistoricalCheckBox = new System.Windows.Forms.CheckBox();
            this.DevBySupButton = new System.Windows.Forms.Button();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.ReplaceYearTextBox = new System.Windows.Forms.TextBox();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.OSTypeComboBox = new System.Windows.Forms.ComboBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.Label10 = new System.Windows.Forms.Label();
            this.StatusComboBox = new System.Windows.Forms.ComboBox();
            this.TrackablesCheckBox = new System.Windows.Forms.CheckBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.EquipTypeComboBox = new System.Windows.Forms.ComboBox();
            this.SerialSearchTextBox = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.LocationComboBox = new System.Windows.Forms.ComboBox();
            this.CurrrentUserTextBox = new System.Windows.Forms.TextBox();
            this.AssetTagSearchTextBox = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.SearchButton = new System.Windows.Forms.Button();
            this.ClearButton = new System.Windows.Forms.Button();
            this.ShowAllButon = new System.Windows.Forms.Button();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.StripSpinner = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.ConnStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel4 = new System.Windows.Forms.ToolStripStatusLabel();
            this.DateTimeLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.TransactionBox = new System.Windows.Forms.GroupBox();
            this.UpdateButton = new System.Windows.Forms.Button();
            this.CommitButton = new System.Windows.Forms.Button();
            this.RollbackButton = new System.Windows.Forms.Button();
            this.ToolStrip1 = new AssetManager.UserInterface.CustomControls.OneClickToolStrip();
            this.AddDeviceToolButton = new System.Windows.Forms.ToolStripButton();
            this.AdminDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.GuidTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.DatabaseToolCombo = new System.Windows.Forms.ToolStripComboBox();
            this.UserManagerMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ReEnterLACredentialsMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ViewLogToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TextEnCrypterToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ScanAttachmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GKUpdaterMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AdvancedSearchMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.StartTransactionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.StartSibiButton = new System.Windows.Forms.ToolStripButton();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.ResultGrid)).BeginInit();
            this.ContextMenuStrip1.SuspendLayout();
            this.InstantSearchGroupBox.SuspendLayout();
            this.Panel1.SuspendLayout();
            this.SearchGroupBox.SuspendLayout();
            this.SearchPanel.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.TransactionBox.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GroupBox1.Controls.Add(this.ResultGrid);
            this.GroupBox1.Controls.Add(this.RecordsCountLabel);
            this.GroupBox1.Location = new System.Drawing.Point(12, 267);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(1356, 513);
            this.GroupBox1.TabIndex = 4;
            this.GroupBox1.TabStop = false;
            // 
            // ResultGrid
            // 
            this.ResultGrid.AllowUserToAddRows = false;
            this.ResultGrid.AllowUserToDeleteRows = false;
            this.ResultGrid.AllowUserToResizeRows = false;
            dataGridViewCellStyle5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(30)))));
            this.ResultGrid.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle5;
            this.ResultGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ResultGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ResultGrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.ResultGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ResultGrid.ContextMenuStrip = this.ContextMenuStrip1;
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle7.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.ResultGrid.DefaultCellStyle = dataGridViewCellStyle7;
            this.ResultGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.ResultGrid.Location = new System.Drawing.Point(9, 19);
            this.ResultGrid.Name = "ResultGrid";
            this.ResultGrid.ReadOnly = true;
            this.ResultGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle8.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle8.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.ResultGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle8;
            this.ResultGrid.RowHeadersVisible = false;
            this.ResultGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.ResultGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.ResultGrid.ShowCellErrors = false;
            this.ResultGrid.ShowCellToolTips = false;
            this.ResultGrid.ShowEditingIcon = false;
            this.ResultGrid.Size = new System.Drawing.Size(1335, 475);
            this.ResultGrid.TabIndex = 17;
            this.ResultGrid.VirtualMode = true;
            this.ResultGrid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResultGrid_CellEnter);
            this.ResultGrid.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResultGrid_CellLeave);
            this.ResultGrid.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ResultGrid_CellMouseDoubleClick);
            this.ResultGrid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.ResultGrid_CellMouseDown);
            this.ResultGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ResultGrid_KeyDown);
            // 
            // ContextMenuStrip1
            // 
            this.ContextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ViewToolStripMenuItem,
            this.AddGKUpdateMenuItem,
            this.SendToGridFormMemuItem,
            this.ToolStripSeparator3,
            this.CopySelectedMenuItem});
            this.ContextMenuStrip1.Name = "ContextMenuStrip1";
            this.ContextMenuStrip1.Size = new System.Drawing.Size(180, 98);
            // 
            // ViewToolStripMenuItem
            // 
            this.ViewToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ViewToolStripMenuItem.Image = global::AssetManager.Properties.Resources.DetailsIcon;
            this.ViewToolStripMenuItem.Name = "ViewToolStripMenuItem";
            this.ViewToolStripMenuItem.Size = new System.Drawing.Size(179, 22);
            this.ViewToolStripMenuItem.Text = "View";
            this.ViewToolStripMenuItem.Click += new System.EventHandler(this.ViewToolStripMenuItem_Click);
            // 
            // AddGKUpdateMenuItem
            // 
            this.AddGKUpdateMenuItem.Image = global::AssetManager.Properties.Resources.GK_SmallIcon;
            this.AddGKUpdateMenuItem.Name = "AddGKUpdateMenuItem";
            this.AddGKUpdateMenuItem.Size = new System.Drawing.Size(179, 22);
            this.AddGKUpdateMenuItem.Text = "Enqueue GK Update";
            this.AddGKUpdateMenuItem.Click += new System.EventHandler(this.AddGKUpdateMenuItem_Click);
            // 
            // SendToGridFormMemuItem
            // 
            this.SendToGridFormMemuItem.Image = global::AssetManager.Properties.Resources.TransferArrowsIcon;
            this.SendToGridFormMemuItem.Name = "SendToGridFormMemuItem";
            this.SendToGridFormMemuItem.Size = new System.Drawing.Size(179, 22);
            this.SendToGridFormMemuItem.Text = "Send to Grid Form";
            this.SendToGridFormMemuItem.Click += new System.EventHandler(this.SendToGridFormMemuItem_Click);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(176, 6);
            // 
            // CopySelectedMenuItem
            // 
            this.CopySelectedMenuItem.Image = global::AssetManager.Properties.Resources.CopyIcon;
            this.CopySelectedMenuItem.Name = "CopySelectedMenuItem";
            this.CopySelectedMenuItem.Size = new System.Drawing.Size(179, 22);
            this.CopySelectedMenuItem.Text = "Copy Selected";
            this.CopySelectedMenuItem.Click += new System.EventHandler(this.CopySelectedMenuItem_Click);
            // 
            // RecordsCountLabel
            // 
            this.RecordsCountLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RecordsCountLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(53)))), ((int)(((byte)(53)))), ((int)(((byte)(53)))));
            this.RecordsCountLabel.Location = new System.Drawing.Point(15, 497);
            this.RecordsCountLabel.Name = "RecordsCountLabel";
            this.RecordsCountLabel.Size = new System.Drawing.Size(1329, 13);
            this.RecordsCountLabel.TabIndex = 18;
            this.RecordsCountLabel.Text = "Records: 0";
            this.RecordsCountLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // InstantSearchGroupBox
            // 
            this.InstantSearchGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.InstantSearchGroupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.InstantSearchGroupBox.Controls.Add(this.Panel1);
            this.InstantSearchGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.InstantSearchGroupBox.Location = new System.Drawing.Point(9, 15);
            this.InstantSearchGroupBox.Name = "InstantSearchGroupBox";
            this.InstantSearchGroupBox.Size = new System.Drawing.Size(177, 205);
            this.InstantSearchGroupBox.TabIndex = 34;
            this.InstantSearchGroupBox.TabStop = false;
            this.InstantSearchGroupBox.Text = "Instant Lookup";
            // 
            // Panel1
            // 
            this.Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.Panel1.Controls.Add(this.Label9);
            this.Panel1.Controls.Add(this.Label8);
            this.Panel1.Controls.Add(this.AssetTagTextBox);
            this.Panel1.Controls.Add(this.SerialTextBox);
            this.Panel1.Location = new System.Drawing.Point(6, 20);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(165, 177);
            this.Panel1.TabIndex = 39;
            // 
            // Label9
            // 
            this.Label9.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label9.AutoSize = true;
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(11, 81);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(46, 16);
            this.Label9.TabIndex = 38;
            this.Label9.Text = "Serial:";
            // 
            // Label8
            // 
            this.Label8.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Label8.AutoSize = true;
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.Location = new System.Drawing.Point(11, 32);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(73, 16);
            this.Label8.TabIndex = 37;
            this.Label8.Text = "Asset Tag:";
            // 
            // AssetTagTextBox
            // 
            this.AssetTagTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.AssetTagTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AssetTagTextBox.Location = new System.Drawing.Point(14, 51);
            this.AssetTagTextBox.MaxLength = 45;
            this.AssetTagTextBox.Name = "AssetTagTextBox";
            this.AssetTagTextBox.Size = new System.Drawing.Size(135, 23);
            this.AssetTagTextBox.TabIndex = 36;
            // 
            // SerialTextBox
            // 
            this.SerialTextBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.SerialTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialTextBox.Location = new System.Drawing.Point(14, 100);
            this.SerialTextBox.MaxLength = 45;
            this.SerialTextBox.Name = "SerialTextBox";
            this.SerialTextBox.Size = new System.Drawing.Size(135, 23);
            this.SerialTextBox.TabIndex = 35;
            // 
            // SearchGroupBox
            // 
            this.SearchGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchGroupBox.Controls.Add(this.SearchPanel);
            this.SearchGroupBox.Controls.Add(this.SearchButton);
            this.SearchGroupBox.Controls.Add(this.ClearButton);
            this.SearchGroupBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchGroupBox.Location = new System.Drawing.Point(192, 15);
            this.SearchGroupBox.Name = "SearchGroupBox";
            this.SearchGroupBox.Size = new System.Drawing.Size(839, 205);
            this.SearchGroupBox.TabIndex = 31;
            this.SearchGroupBox.TabStop = false;
            this.SearchGroupBox.Text = "Custom Search";
            // 
            // SearchPanel
            // 
            this.SearchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.SearchPanel.AutoScrollMargin = new System.Drawing.Size(10, 20);
            this.SearchPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.SearchPanel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SearchPanel.Controls.Add(this.HistoricalCheckBox);
            this.SearchPanel.Controls.Add(this.DevBySupButton);
            this.SearchPanel.Controls.Add(this.Label6);
            this.SearchPanel.Controls.Add(this.Label4);
            this.SearchPanel.Controls.Add(this.ReplaceYearTextBox);
            this.SearchPanel.Controls.Add(this.DescriptionTextBox);
            this.SearchPanel.Controls.Add(this.OSTypeComboBox);
            this.SearchPanel.Controls.Add(this.Label2);
            this.SearchPanel.Controls.Add(this.Label5);
            this.SearchPanel.Controls.Add(this.Label1);
            this.SearchPanel.Controls.Add(this.Label10);
            this.SearchPanel.Controls.Add(this.StatusComboBox);
            this.SearchPanel.Controls.Add(this.TrackablesCheckBox);
            this.SearchPanel.Controls.Add(this.Label12);
            this.SearchPanel.Controls.Add(this.EquipTypeComboBox);
            this.SearchPanel.Controls.Add(this.SerialSearchTextBox);
            this.SearchPanel.Controls.Add(this.Label3);
            this.SearchPanel.Controls.Add(this.LocationComboBox);
            this.SearchPanel.Controls.Add(this.CurrrentUserTextBox);
            this.SearchPanel.Controls.Add(this.AssetTagSearchTextBox);
            this.SearchPanel.Controls.Add(this.Label11);
            this.SearchPanel.Location = new System.Drawing.Point(6, 20);
            this.SearchPanel.Name = "SearchPanel";
            this.SearchPanel.Size = new System.Drawing.Size(715, 177);
            this.SearchPanel.TabIndex = 52;
            this.SearchPanel.Scroll += new System.Windows.Forms.ScrollEventHandler(this.PanelNoScrollOnFocus1_Scroll);
            // 
            // HistoricalCheckBox
            // 
            this.HistoricalCheckBox.AutoSize = true;
            this.HistoricalCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HistoricalCheckBox.Location = new System.Drawing.Point(433, 127);
            this.HistoricalCheckBox.Name = "HistoricalCheckBox";
            this.HistoricalCheckBox.Size = new System.Drawing.Size(89, 22);
            this.HistoricalCheckBox.TabIndex = 56;
            this.HistoricalCheckBox.Text = "Historical";
            this.HistoricalCheckBox.UseVisualStyleBackColor = true;
            // 
            // DevBySupButton
            // 
            this.DevBySupButton.Location = new System.Drawing.Point(551, 115);
            this.DevBySupButton.Name = "DevBySupButton";
            this.DevBySupButton.Size = new System.Drawing.Size(125, 44);
            this.DevBySupButton.TabIndex = 55;
            this.DevBySupButton.Text = "Devices By Supervisor";
            this.DevBySupButton.UseVisualStyleBackColor = true;
            this.DevBySupButton.Click += new System.EventHandler(this.DevBySupButton_Click);
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(172, 109);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(95, 16);
            this.Label6.TabIndex = 54;
            this.Label6.Text = "Replace Year:";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(11, 13);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(73, 16);
            this.Label4.TabIndex = 48;
            this.Label4.Text = "Asset Tag:";
            // 
            // ReplaceYearTextBox
            // 
            this.ReplaceYearTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReplaceYearTextBox.Location = new System.Drawing.Point(175, 128);
            this.ReplaceYearTextBox.MaxLength = 200;
            this.ReplaceYearTextBox.Name = "ReplaceYearTextBox";
            this.ReplaceYearTextBox.Size = new System.Drawing.Size(100, 23);
            this.ReplaceYearTextBox.TabIndex = 53;
            this.ReplaceYearTextBox.TabStop = false;
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionTextBox.Location = new System.Drawing.Point(175, 79);
            this.DescriptionTextBox.MaxLength = 200;
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(330, 23);
            this.DescriptionTextBox.TabIndex = 43;
            this.DescriptionTextBox.TabStop = false;
            // 
            // OSTypeComboBox
            // 
            this.OSTypeComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OSTypeComboBox.FormattingEnabled = true;
            this.OSTypeComboBox.Location = new System.Drawing.Point(14, 128);
            this.OSTypeComboBox.Name = "OSTypeComboBox";
            this.OSTypeComboBox.Size = new System.Drawing.Size(135, 23);
            this.OSTypeComboBox.TabIndex = 51;
            this.OSTypeComboBox.TabStop = false;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(172, 60);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(79, 16);
            this.Label2.TabIndex = 44;
            this.Label2.Text = "Description:";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(11, 109);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(65, 16);
            this.Label5.TabIndex = 52;
            this.Label5.Text = "OS Type:";
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(523, 60);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(48, 16);
            this.Label1.TabIndex = 42;
            this.Label1.Text = "Status:";
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label10.Location = new System.Drawing.Point(343, 13);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(110, 16);
            this.Label10.TabIndex = 36;
            this.Label10.Text = "Equipment Type:";
            // 
            // StatusComboBox
            // 
            this.StatusComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusComboBox.FormattingEnabled = true;
            this.StatusComboBox.Location = new System.Drawing.Point(526, 79);
            this.StatusComboBox.Name = "StatusComboBox";
            this.StatusComboBox.Size = new System.Drawing.Size(165, 23);
            this.StatusComboBox.TabIndex = 41;
            this.StatusComboBox.TabStop = false;
            // 
            // TrackablesCheckBox
            // 
            this.TrackablesCheckBox.AutoSize = true;
            this.TrackablesCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackablesCheckBox.Location = new System.Drawing.Point(312, 127);
            this.TrackablesCheckBox.Name = "TrackablesCheckBox";
            this.TrackablesCheckBox.Size = new System.Drawing.Size(100, 22);
            this.TrackablesCheckBox.TabIndex = 50;
            this.TrackablesCheckBox.Text = "Trackables";
            this.TrackablesCheckBox.UseVisualStyleBackColor = true;
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label12.Location = new System.Drawing.Point(523, 13);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(62, 16);
            this.Label12.TabIndex = 40;
            this.Label12.Text = "Location:";
            // 
            // EquipTypeComboBox
            // 
            this.EquipTypeComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EquipTypeComboBox.FormattingEnabled = true;
            this.EquipTypeComboBox.Location = new System.Drawing.Point(346, 31);
            this.EquipTypeComboBox.Name = "EquipTypeComboBox";
            this.EquipTypeComboBox.Size = new System.Drawing.Size(159, 23);
            this.EquipTypeComboBox.TabIndex = 35;
            this.EquipTypeComboBox.TabStop = false;
            // 
            // SerialSearchTextBox
            // 
            this.SerialSearchTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialSearchTextBox.Location = new System.Drawing.Point(14, 79);
            this.SerialSearchTextBox.MaxLength = 45;
            this.SerialSearchTextBox.Name = "SerialSearchTextBox";
            this.SerialSearchTextBox.Size = new System.Drawing.Size(135, 23);
            this.SerialSearchTextBox.TabIndex = 46;
            this.SerialSearchTextBox.TabStop = false;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(11, 60);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(46, 16);
            this.Label3.TabIndex = 49;
            this.Label3.Text = "Serial:";
            // 
            // LocationComboBox
            // 
            this.LocationComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocationComboBox.FormattingEnabled = true;
            this.LocationComboBox.Location = new System.Drawing.Point(526, 31);
            this.LocationComboBox.Name = "LocationComboBox";
            this.LocationComboBox.Size = new System.Drawing.Size(165, 23);
            this.LocationComboBox.TabIndex = 39;
            this.LocationComboBox.TabStop = false;
            // 
            // CurrrentUserTextBox
            // 
            this.CurrrentUserTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrrentUserTextBox.Location = new System.Drawing.Point(175, 31);
            this.CurrrentUserTextBox.MaxLength = 45;
            this.CurrrentUserTextBox.Name = "CurrrentUserTextBox";
            this.CurrrentUserTextBox.Size = new System.Drawing.Size(148, 23);
            this.CurrrentUserTextBox.TabIndex = 37;
            this.CurrrentUserTextBox.TabStop = false;
            // 
            // AssetTagSearchTextBox
            // 
            this.AssetTagSearchTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AssetTagSearchTextBox.Location = new System.Drawing.Point(14, 31);
            this.AssetTagSearchTextBox.MaxLength = 45;
            this.AssetTagSearchTextBox.Name = "AssetTagSearchTextBox";
            this.AssetTagSearchTextBox.Size = new System.Drawing.Size(135, 23);
            this.AssetTagSearchTextBox.TabIndex = 47;
            this.AssetTagSearchTextBox.TabStop = false;
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label11.Location = new System.Drawing.Point(172, 13);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(85, 16);
            this.Label11.TabIndex = 38;
            this.Label11.Text = "Current User:";
            // 
            // SearchButton
            // 
            this.SearchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchButton.Location = new System.Drawing.Point(736, 45);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(88, 35);
            this.SearchButton.TabIndex = 45;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // ClearButton
            // 
            this.ClearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearButton.Location = new System.Drawing.Point(736, 144);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(88, 35);
            this.ClearButton.TabIndex = 18;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // ShowAllButon
            // 
            this.ShowAllButon.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowAllButon.Location = new System.Drawing.Point(1046, 60);
            this.ShowAllButon.Name = "ShowAllButon";
            this.ShowAllButon.Size = new System.Drawing.Size(134, 35);
            this.ShowAllButon.TabIndex = 27;
            this.ShowAllButon.Text = "Show All";
            this.ShowAllButon.UseVisualStyleBackColor = true;
            this.ShowAllButon.Click += new System.EventHandler(this.ShowAllButon_Click);
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.StatusStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.StripSpinner,
            this.ToolStripStatusLabel1,
            this.ConnStatusLabel,
            this.ToolStripStatusLabel4,
            this.DateTimeLabel});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 787);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.ShowItemToolTips = true;
            this.StatusStrip1.Size = new System.Drawing.Size(1381, 22);
            this.StatusStrip1.TabIndex = 5;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(101, 17);
            this.StatusLabel.Text = "%StatusLabel%";
            // 
            // StripSpinner
            // 
            this.StripSpinner.Image = global::AssetManager.Properties.Resources.LoadingAni;
            this.StripSpinner.Name = "StripSpinner";
            this.StripSpinner.Size = new System.Drawing.Size(16, 17);
            this.StripSpinner.Visible = false;
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(1104, 17);
            this.ToolStripStatusLabel1.Spring = true;
            // 
            // ConnStatusLabel
            // 
            this.ConnStatusLabel.ForeColor = System.Drawing.Color.Green;
            this.ConnStatusLabel.Name = "ConnStatusLabel";
            this.ConnStatusLabel.Size = new System.Drawing.Size(73, 17);
            this.ConnStatusLabel.Text = "Connected";
            // 
            // ToolStripStatusLabel4
            // 
            this.ToolStripStatusLabel4.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolStripStatusLabel4.Name = "ToolStripStatusLabel4";
            this.ToolStripStatusLabel4.Size = new System.Drawing.Size(12, 17);
            this.ToolStripStatusLabel4.Text = "|";
            // 
            // DateTimeLabel
            // 
            this.DateTimeLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DateTimeLabel.Name = "DateTimeLabel";
            this.DateTimeLabel.Size = new System.Drawing.Size(76, 17);
            this.DateTimeLabel.Text = "ServerTime";
            this.DateTimeLabel.ToolTipText = "Server Time";
            this.DateTimeLabel.Click += new System.EventHandler(this.DateTimeLabel_Click);
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.TransactionBox);
            this.GroupBox2.Controls.Add(this.SearchGroupBox);
            this.GroupBox2.Controls.Add(this.ShowAllButon);
            this.GroupBox2.Controls.Add(this.InstantSearchGroupBox);
            this.GroupBox2.Location = new System.Drawing.Point(12, 40);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(1196, 226);
            this.GroupBox2.TabIndex = 7;
            this.GroupBox2.TabStop = false;
            // 
            // TransactionBox
            // 
            this.TransactionBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.TransactionBox.Controls.Add(this.UpdateButton);
            this.TransactionBox.Controls.Add(this.CommitButton);
            this.TransactionBox.Controls.Add(this.RollbackButton);
            this.TransactionBox.Location = new System.Drawing.Point(1046, 118);
            this.TransactionBox.Name = "TransactionBox";
            this.TransactionBox.Size = new System.Drawing.Size(134, 92);
            this.TransactionBox.TabIndex = 38;
            this.TransactionBox.TabStop = false;
            this.TransactionBox.Text = "Transaction";
            this.TransactionBox.Visible = false;
            // 
            // UpdateButton
            // 
            this.UpdateButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.UpdateButton.Location = new System.Drawing.Point(6, 18);
            this.UpdateButton.Name = "UpdateButton";
            this.UpdateButton.Size = new System.Drawing.Size(122, 22);
            this.UpdateButton.TabIndex = 38;
            this.UpdateButton.Text = "Apply Changes";
            this.UpdateButton.UseVisualStyleBackColor = false;
            this.UpdateButton.Click += new System.EventHandler(this.UpdateButton_Click);
            // 
            // CommitButton
            // 
            this.CommitButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.CommitButton.Location = new System.Drawing.Point(6, 41);
            this.CommitButton.Name = "CommitButton";
            this.CommitButton.Size = new System.Drawing.Size(122, 22);
            this.CommitButton.TabIndex = 36;
            this.CommitButton.Text = "Commit Changes";
            this.CommitButton.UseVisualStyleBackColor = false;
            this.CommitButton.Click += new System.EventHandler(this.CommitButton_Click);
            // 
            // RollbackButton
            // 
            this.RollbackButton.BackColor = System.Drawing.Color.Red;
            this.RollbackButton.Location = new System.Drawing.Point(6, 64);
            this.RollbackButton.Name = "RollbackButton";
            this.RollbackButton.Size = new System.Drawing.Size(122, 22);
            this.RollbackButton.TabIndex = 37;
            this.RollbackButton.Text = "Rollback/Cancel";
            this.RollbackButton.UseVisualStyleBackColor = false;
            this.RollbackButton.Click += new System.EventHandler(this.RollbackButton_Click);
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.AutoSize = false;
            this.ToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(226)))), ((int)(((byte)(166)))));
            this.ToolStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolStrip1.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AddDeviceToolButton,
            this.AdminDropDown,
            this.ToolStripSeparator5,
            this.StartSibiButton});
            this.ToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ToolStrip1.Size = new System.Drawing.Size(1381, 37);
            this.ToolStrip1.Stretch = true;
            this.ToolStrip1.TabIndex = 6;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // AddDeviceToolButton
            // 
            this.AddDeviceToolButton.Image = global::AssetManager.Properties.Resources.AddIcon;
            this.AddDeviceToolButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AddDeviceToolButton.Name = "AddDeviceToolButton";
            this.AddDeviceToolButton.Padding = new System.Windows.Forms.Padding(20, 0, 0, 0);
            this.AddDeviceToolButton.Size = new System.Drawing.Size(137, 34);
            this.AddDeviceToolButton.Text = "Add Device";
            this.AddDeviceToolButton.Click += new System.EventHandler(this.AddDeviceToolButton_Click);
            // 
            // AdminDropDown
            // 
            this.AdminDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.GuidTextBox,
            this.DatabaseToolCombo,
            this.UserManagerMenuItem,
            this.ReEnterLACredentialsMenuItem,
            this.ViewLogToolStripMenuItem,
            this.TextEnCrypterToolStripMenuItem,
            this.ScanAttachmentToolStripMenuItem,
            this.GKUpdaterMenuItem,
            this.AdvancedSearchMenuItem,
            this.StartTransactionToolStripMenuItem});
            this.AdminDropDown.Image = global::AssetManager.Properties.Resources.AdminIcon;
            this.AdminDropDown.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AdminDropDown.Name = "AdminDropDown";
            this.AdminDropDown.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.AdminDropDown.Size = new System.Drawing.Size(143, 34);
            this.AdminDropDown.Text = "Admin Tools";
            // 
            // GuidTextBox
            // 
            this.GuidTextBox.AutoSize = false;
            this.GuidTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.GuidTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GuidTextBox.Name = "GuidTextBox";
            this.GuidTextBox.Size = new System.Drawing.Size(150, 23);
            this.GuidTextBox.ToolTipText = "Guid Lookup. (Press Enter)";
            this.GuidTextBox.Visible = false;
            this.GuidTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.GuidTextBox_KeyDown);
            // 
            // DatabaseToolCombo
            // 
            this.DatabaseToolCombo.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DatabaseToolCombo.Name = "DatabaseToolCombo";
            this.DatabaseToolCombo.Size = new System.Drawing.Size(121, 25);
            this.DatabaseToolCombo.ToolTipText = "Change Database";
            this.DatabaseToolCombo.DropDownClosed += new System.EventHandler(this.DatabaseToolCombo_DropDownClosed);
            // 
            // UserManagerMenuItem
            // 
            this.UserManagerMenuItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.UserManagerMenuItem.Name = "UserManagerMenuItem";
            this.UserManagerMenuItem.Size = new System.Drawing.Size(242, 26);
            this.UserManagerMenuItem.Text = "User Manager";
            this.UserManagerMenuItem.Click += new System.EventHandler(this.UserManagerMenuItem_Click);
            // 
            // ReEnterLACredentialsMenuItem
            // 
            this.ReEnterLACredentialsMenuItem.Name = "ReEnterLACredentialsMenuItem";
            this.ReEnterLACredentialsMenuItem.Size = new System.Drawing.Size(242, 26);
            this.ReEnterLACredentialsMenuItem.Text = "Re-Enter Credentials";
            this.ReEnterLACredentialsMenuItem.Click += new System.EventHandler(this.ReEnterLACredentialsMenuItem_Click);
            // 
            // ViewLogToolStripMenuItem
            // 
            this.ViewLogToolStripMenuItem.Name = "ViewLogToolStripMenuItem";
            this.ViewLogToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.ViewLogToolStripMenuItem.Text = "View Log";
            this.ViewLogToolStripMenuItem.Click += new System.EventHandler(this.ViewLogToolStripMenuItem_Click);
            // 
            // TextEnCrypterToolStripMenuItem
            // 
            this.TextEnCrypterToolStripMenuItem.Name = "TextEnCrypterToolStripMenuItem";
            this.TextEnCrypterToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.TextEnCrypterToolStripMenuItem.Text = "Text Encrypter";
            this.TextEnCrypterToolStripMenuItem.Click += new System.EventHandler(this.TextEnCrypterToolStripMenuItem_Click);
            // 
            // ScanAttachmentToolStripMenuItem
            // 
            this.ScanAttachmentToolStripMenuItem.Name = "ScanAttachmentToolStripMenuItem";
            this.ScanAttachmentToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.ScanAttachmentToolStripMenuItem.Text = "Scan Attachments";
            this.ScanAttachmentToolStripMenuItem.Click += new System.EventHandler(this.ScanAttachmentToolStripMenuItem_Click);
            // 
            // GKUpdaterMenuItem
            // 
            this.GKUpdaterMenuItem.Name = "GKUpdaterMenuItem";
            this.GKUpdaterMenuItem.Size = new System.Drawing.Size(242, 26);
            this.GKUpdaterMenuItem.Text = "GK Updater";
            this.GKUpdaterMenuItem.Click += new System.EventHandler(this.GKUpdaterMenuItem_Click);
            // 
            // AdvancedSearchMenuItem
            // 
            this.AdvancedSearchMenuItem.Name = "AdvancedSearchMenuItem";
            this.AdvancedSearchMenuItem.Size = new System.Drawing.Size(242, 26);
            this.AdvancedSearchMenuItem.Text = "Advanced Search";
            this.AdvancedSearchMenuItem.Click += new System.EventHandler(this.AdvancedSearchMenuItem_Click);
            // 
            // StartTransactionToolStripMenuItem
            // 
            this.StartTransactionToolStripMenuItem.Name = "StartTransactionToolStripMenuItem";
            this.StartTransactionToolStripMenuItem.Size = new System.Drawing.Size(242, 26);
            this.StartTransactionToolStripMenuItem.Text = "Start Manual Edit Mode";
            this.StartTransactionToolStripMenuItem.Click += new System.EventHandler(this.StartTransactionToolStripMenuItem_Click);
            // 
            // ToolStripSeparator5
            // 
            this.ToolStripSeparator5.Name = "ToolStripSeparator5";
            this.ToolStripSeparator5.Size = new System.Drawing.Size(6, 37);
            // 
            // StartSibiButton
            // 
            this.StartSibiButton.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.StartSibiButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartSibiButton.Image = global::AssetManager.Properties.Resources.SibiLogo;
            this.StartSibiButton.Name = "StartSibiButton";
            this.StartSibiButton.Padding = new System.Windows.Forms.Padding(20, 0, 20, 0);
            this.StartSibiButton.Size = new System.Drawing.Size(232, 34);
            this.StartSibiButton.Text = "Sibi Acquisition Manager";
            this.StartSibiButton.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.StartSibiButton.Click += new System.EventHandler(this.StartSibiButton_Click);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ClientSize = new System.Drawing.Size(1381, 809);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.ToolStrip1);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.GroupBox1);
            this.MinimumSize = new System.Drawing.Size(1256, 443);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Asset Manager - Main";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Shown += new System.EventHandler(this.MainForm_Shown);
            this.GroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.ResultGrid)).EndInit();
            this.ContextMenuStrip1.ResumeLayout(false);
            this.InstantSearchGroupBox.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.SearchGroupBox.ResumeLayout(false);
            this.SearchPanel.ResumeLayout(false);
            this.SearchPanel.PerformLayout();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.TransactionBox.ResumeLayout(false);
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal System.Windows.Forms.GroupBox GroupBox1;
        internal System.Windows.Forms.Button ShowAllButon;
        internal System.Windows.Forms.Button ClearButton;
        internal System.Windows.Forms.DataGridView ResultGrid;
        internal System.Windows.Forms.Button SearchButton;
        internal System.Windows.Forms.GroupBox SearchGroupBox;
        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.ComboBox StatusComboBox;
        internal System.Windows.Forms.Label Label12;
        internal System.Windows.Forms.ComboBox LocationComboBox;
        internal System.Windows.Forms.Label Label11;
        internal System.Windows.Forms.TextBox CurrrentUserTextBox;
        internal System.Windows.Forms.Label Label10;
        internal System.Windows.Forms.ComboBox EquipTypeComboBox;
        internal System.Windows.Forms.ContextMenuStrip ContextMenuStrip1;
        internal System.Windows.Forms.ToolStripMenuItem ViewToolStripMenuItem;
        internal System.Windows.Forms.StatusStrip StatusStrip1;
        internal System.Windows.Forms.ToolStripStatusLabel StatusLabel;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox DescriptionTextBox;
        internal System.Windows.Forms.GroupBox InstantSearchGroupBox;
        internal System.Windows.Forms.Label Label9;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.TextBox AssetTagTextBox;
        internal System.Windows.Forms.TextBox SerialTextBox;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.TextBox AssetTagSearchTextBox;
        internal System.Windows.Forms.TextBox SerialSearchTextBox;
        internal System.Windows.Forms.ToolStripStatusLabel StripSpinner;
        internal OneClickToolStrip ToolStrip1;
        internal System.Windows.Forms.ToolStripButton AddDeviceToolButton;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator3;
        internal System.Windows.Forms.ToolStripMenuItem CopySelectedMenuItem;
        internal System.Windows.Forms.ToolStripStatusLabel ConnStatusLabel;
        internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel1;
        internal System.Windows.Forms.ToolStripStatusLabel DateTimeLabel;
        internal System.Windows.Forms.ToolStripDropDownButton AdminDropDown;
        internal System.Windows.Forms.GroupBox GroupBox2;
        internal System.Windows.Forms.CheckBox TrackablesCheckBox;
        internal System.Windows.Forms.ToolStripTextBox GuidTextBox;
        internal System.Windows.Forms.ComboBox OSTypeComboBox;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.TextBox ReplaceYearTextBox;
        internal System.Windows.Forms.Panel SearchPanel;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.ToolStripSeparator ToolStripSeparator5;
        internal System.Windows.Forms.ToolStripButton StartSibiButton;
        internal System.Windows.Forms.ToolStripMenuItem UserManagerMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem TextEnCrypterToolStripMenuItem;
        internal System.Windows.Forms.ToolStripStatusLabel ToolStripStatusLabel4;
        internal System.Windows.Forms.Label RecordsCountLabel;
        internal System.Windows.Forms.ToolStripMenuItem ScanAttachmentToolStripMenuItem;
        internal System.Windows.Forms.Button DevBySupButton;
        internal System.Windows.Forms.CheckBox HistoricalCheckBox;
        internal System.Windows.Forms.ToolStripMenuItem AddGKUpdateMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem GKUpdaterMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem AdvancedSearchMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem SendToGridFormMemuItem;
        internal System.Windows.Forms.ToolStripMenuItem ReEnterLACredentialsMenuItem;
        internal System.Windows.Forms.ToolStripMenuItem ViewLogToolStripMenuItem;
        internal System.Windows.Forms.ToolStripComboBox DatabaseToolCombo;
        internal System.Windows.Forms.Button RollbackButton;
        internal System.Windows.Forms.Button CommitButton;
        internal System.Windows.Forms.ToolStripMenuItem StartTransactionToolStripMenuItem;
        internal System.Windows.Forms.GroupBox TransactionBox;
        internal System.Windows.Forms.Button UpdateButton;
    }
}
