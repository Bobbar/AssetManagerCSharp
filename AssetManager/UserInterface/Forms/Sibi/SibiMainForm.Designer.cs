using System.Windows.Forms;
using AssetManager.UserInterface.CustomControls;

namespace AssetManager.UserInterface.Forms.Sibi
{

    partial class SibiMainForm
    {
        //Required by the Windows Form Designer
        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.

        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.searchSlider = new AssetManager.UserInterface.CustomControls.SliderLabel();
            this.ItemSearchButton = new System.Windows.Forms.Button();
            this.ItemSearchTextBox = new System.Windows.Forms.TextBox();
            this.txtRTNum = new System.Windows.Forms.TextBox();
            this.txtReq = new System.Windows.Forms.TextBox();
            this.txtPO = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.txtDescription = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.RefreshResetButton = new System.Windows.Forms.Button();
            this.Label1 = new System.Windows.Forms.Label();
            this.cmbDisplayYear = new System.Windows.Forms.ComboBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.SibiResultGrid = new System.Windows.Forms.DataGridView();
            this.ToolStrip1 = new AssetManager.UserInterface.CustomControls.OneClickToolStrip();
            this.cmdManage = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.Panel1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.SibiResultGrid)).BeginInit();
            this.ToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Panel1
            // 
            this.Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel1.Controls.Add(this.GroupBox2);
            this.Panel1.Controls.Add(this.GroupBox1);
            this.Panel1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Panel1.Location = new System.Drawing.Point(3, 28);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(1179, 596);
            this.Panel1.TabIndex = 0;
            // 
            // GroupBox2
            // 
            this.GroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox2.Controls.Add(this.groupBox3);
            this.GroupBox2.Controls.Add(this.txtRTNum);
            this.GroupBox2.Controls.Add(this.txtReq);
            this.GroupBox2.Controls.Add(this.txtPO);
            this.GroupBox2.Controls.Add(this.Label5);
            this.GroupBox2.Controls.Add(this.Label4);
            this.GroupBox2.Controls.Add(this.txtDescription);
            this.GroupBox2.Controls.Add(this.Label3);
            this.GroupBox2.Controls.Add(this.RefreshResetButton);
            this.GroupBox2.Controls.Add(this.Label1);
            this.GroupBox2.Controls.Add(this.cmbDisplayYear);
            this.GroupBox2.Controls.Add(this.Label2);
            this.GroupBox2.Location = new System.Drawing.Point(6, 16);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(1170, 81);
            this.GroupBox2.TabIndex = 22;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Filters:";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.searchSlider);
            this.groupBox3.Controls.Add(this.ItemSearchButton);
            this.groupBox3.Controls.Add(this.ItemSearchTextBox);
            this.groupBox3.Location = new System.Drawing.Point(756, 10);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(266, 62);
            this.groupBox3.TabIndex = 30;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Search Items";
            // 
            // searchSlider
            // 
            this.searchSlider.BackColor = System.Drawing.SystemColors.Control;
            this.searchSlider.DisplayTime = 4;
            this.searchSlider.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.searchSlider.Location = new System.Drawing.Point(6, 43);
            this.searchSlider.Name = "searchSlider";
            this.searchSlider.Size = new System.Drawing.Size(184, 16);
            this.searchSlider.SlideText = "";
            this.searchSlider.TabIndex = 31;
            // 
            // ItemSearchButton
            // 
            this.ItemSearchButton.Location = new System.Drawing.Point(196, 18);
            this.ItemSearchButton.Name = "ItemSearchButton";
            this.ItemSearchButton.Size = new System.Drawing.Size(56, 23);
            this.ItemSearchButton.TabIndex = 30;
            this.ItemSearchButton.Text = "Go";
            this.ItemSearchButton.UseVisualStyleBackColor = true;
            this.ItemSearchButton.Click += new System.EventHandler(this.ItemSearchButton_Click);
            // 
            // ItemSearchTextBox
            // 
            this.ItemSearchTextBox.Location = new System.Drawing.Point(6, 18);
            this.ItemSearchTextBox.Name = "ItemSearchTextBox";
            this.ItemSearchTextBox.Size = new System.Drawing.Size(184, 23);
            this.ItemSearchTextBox.TabIndex = 29;
            this.ItemSearchTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.ItemSearchTextBox_KeyDown);
            // 
            // txtRTNum
            // 
            this.txtRTNum.Location = new System.Drawing.Point(624, 40);
            this.txtRTNum.Name = "txtRTNum";
            this.txtRTNum.Size = new System.Drawing.Size(77, 23);
            this.txtRTNum.TabIndex = 27;
            this.txtRTNum.TextChanged += new System.EventHandler(this.txtRTNum_TextChanged);
            // 
            // txtReq
            // 
            this.txtReq.Location = new System.Drawing.Point(519, 40);
            this.txtReq.Name = "txtReq";
            this.txtReq.Size = new System.Drawing.Size(77, 23);
            this.txtReq.TabIndex = 23;
            this.txtReq.TextChanged += new System.EventHandler(this.txtReq_TextChanged);
            // 
            // txtPO
            // 
            this.txtPO.Location = new System.Drawing.Point(374, 40);
            this.txtPO.Name = "txtPO";
            this.txtPO.Size = new System.Drawing.Size(121, 23);
            this.txtPO.TabIndex = 21;
            this.txtPO.TextChanged += new System.EventHandler(this.txtPO_TextChanged);
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(621, 22);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(35, 15);
            this.Label5.TabIndex = 28;
            this.Label5.Text = "RT #";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(165, 22);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(84, 15);
            this.Label4.TabIndex = 26;
            this.Label4.Text = "Description";
            // 
            // txtDescription
            // 
            this.txtDescription.Location = new System.Drawing.Point(168, 40);
            this.txtDescription.Name = "txtDescription";
            this.txtDescription.Size = new System.Drawing.Size(184, 23);
            this.txtDescription.TabIndex = 25;
            this.txtDescription.TextChanged += new System.EventHandler(this.txtDescription_TextChanged);
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(516, 22);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(42, 15);
            this.Label3.TabIndex = 24;
            this.Label3.Text = "Req #";
            // 
            // RefreshResetButton
            // 
            this.RefreshResetButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RefreshResetButton.Location = new System.Drawing.Point(1050, 17);
            this.RefreshResetButton.Name = "RefreshResetButton";
            this.RefreshResetButton.Size = new System.Drawing.Size(111, 30);
            this.RefreshResetButton.TabIndex = 1;
            this.RefreshResetButton.Text = "Refresh/Reset";
            this.RefreshResetButton.UseVisualStyleBackColor = true;
            this.RefreshResetButton.Click += new System.EventHandler(this.RefreshResetButton_Click);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(371, 22);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(35, 15);
            this.Label1.TabIndex = 22;
            this.Label1.Text = "PO #";
            // 
            // cmbDisplayYear
            // 
            this.cmbDisplayYear.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cmbDisplayYear.FormattingEnabled = true;
            this.cmbDisplayYear.Location = new System.Drawing.Point(9, 40);
            this.cmbDisplayYear.Name = "cmbDisplayYear";
            this.cmbDisplayYear.Size = new System.Drawing.Size(115, 23);
            this.cmbDisplayYear.TabIndex = 19;
            this.cmbDisplayYear.SelectedIndexChanged += new System.EventHandler(this.cmbDisplayYear_SelectedIndexChanged);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.Label2.Location = new System.Drawing.Point(6, 22);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(91, 15);
            this.Label2.TabIndex = 20;
            this.Label2.Text = "Request Year";
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.Controls.Add(this.SibiResultGrid);
            this.GroupBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox1.Location = new System.Drawing.Point(6, 103);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(1170, 490);
            this.GroupBox1.TabIndex = 21;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Requests:";
            // 
            // SibiResultGrid
            // 
            this.SibiResultGrid.AllowUserToAddRows = false;
            this.SibiResultGrid.AllowUserToDeleteRows = false;
            this.SibiResultGrid.AllowUserToResizeRows = false;
            this.SibiResultGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SibiResultGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.SibiResultGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.SibiResultGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.SibiResultGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.SibiResultGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.SibiResultGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.SibiResultGrid.Location = new System.Drawing.Point(6, 22);
            this.SibiResultGrid.MultiSelect = false;
            this.SibiResultGrid.Name = "SibiResultGrid";
            this.SibiResultGrid.ReadOnly = true;
            this.SibiResultGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.SibiResultGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.SibiResultGrid.RowHeadersVisible = false;
            this.SibiResultGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.SibiResultGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.SibiResultGrid.ShowCellErrors = false;
            this.SibiResultGrid.ShowCellToolTips = false;
            this.SibiResultGrid.ShowEditingIcon = false;
            this.SibiResultGrid.Size = new System.Drawing.Size(1158, 462);
            this.SibiResultGrid.TabIndex = 18;
            this.SibiResultGrid.VirtualMode = true;
            this.SibiResultGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResultGrid_CellDoubleClick);
            this.SibiResultGrid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResultGrid_CellEnter);
            this.SibiResultGrid.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.ResultGrid_CellLeave);
            this.SibiResultGrid.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.ResultGrid_RowPostPaint);
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.AutoSize = false;
            this.ToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(213)))), ((int)(((byte)(255)))));
            this.ToolStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolStrip1.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cmdManage,
            this.ToolStripSeparator1});
            this.ToolStrip1.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ToolStrip1.Size = new System.Drawing.Size(1188, 37);
            this.ToolStrip1.Stretch = true;
            this.ToolStrip1.TabIndex = 2;
            this.ToolStrip1.Text = "ToolStrip1";
            // 
            // cmdManage
            // 
            this.cmdManage.Image = global::AssetManager.Properties.Resources.AddIcon;
            this.cmdManage.Name = "cmdManage";
            this.cmdManage.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.cmdManage.Size = new System.Drawing.Size(141, 34);
            this.cmdManage.Text = "New Request";
            this.cmdManage.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cmdManage.Click += new System.EventHandler(this.cmdManage_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 37);
            // 
            // SibiMainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1188, 636);
            this.Controls.Add(this.ToolStrip1);
            this.Controls.Add(this.Panel1);
            this.MinimumSize = new System.Drawing.Size(1121, 371);
            this.Name = "SibiMainForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sibi Acquisition Manager - Main";
            this.Panel1.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.SibiResultGrid)).EndInit();
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }
        internal Panel Panel1;
        internal DataGridView SibiResultGrid;
        internal Button RefreshResetButton;
        internal OneClickToolStrip ToolStrip1;
        internal ToolStripButton cmdManage;
        internal Label Label2;
        internal ComboBox cmbDisplayYear;
        internal ToolStripSeparator ToolStripSeparator1;
        internal GroupBox GroupBox1;
        internal GroupBox GroupBox2;
        internal Label Label3;
        internal TextBox txtReq;
        internal Label Label1;
        internal TextBox txtPO;
        internal Label Label4;
        internal TextBox txtDescription;
        internal Label Label5;
        internal TextBox txtRTNum;
        internal TextBox ItemSearchTextBox;
        private GroupBox groupBox3;
        private Button ItemSearchButton;
        private SliderLabel searchSlider;
    }
}
