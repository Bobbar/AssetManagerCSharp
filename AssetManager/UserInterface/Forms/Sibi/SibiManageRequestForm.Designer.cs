using System.Windows.Forms;
using AssetManager.UserInterface.CustomControls;

namespace AssetManager.UserInterface.Forms.Sibi
{

    partial class SibiManageRequestForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.PopupMenuItems = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.PopulateFAMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.LookupDeviceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.GLBudgetMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyTextMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ImportDeviceMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.DeleteRequestMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.RTNumberTextBox = new System.Windows.Forms.TextBox();
            this.CreateDateTextBox = new System.Windows.Forms.TextBox();
            this.ModifyDateTextBox = new System.Windows.Forms.TextBox();
            this.ModifyByTextBox = new System.Windows.Forms.TextBox();
            this.PopupMenuNotes = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.NewNoteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.DeleteNoteMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ContentPanel = new System.Windows.Forms.ToolStripContentPanel();
            this.RequestPanel = new System.Windows.Forms.Panel();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.RequestNumTextBox = new System.Windows.Forms.TextBox();
            this.StatusComboBox = new System.Windows.Forms.ComboBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.ReqStatusLabel = new System.Windows.Forms.Label();
            this.POStatusLabel = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.ReqNumberTextBox = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.POTextBox = new System.Windows.Forms.TextBox();
            this.TypeComboBox = new System.Windows.Forms.ComboBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.NeedByDatePicker = new System.Windows.Forms.DateTimePicker();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.RequestUserTextBox = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.ButtonPanel = new System.Windows.Forms.Panel();
            this.EditButtonsPanel = new System.Windows.Forms.Panel();
            this.AcceptChangesButton = new System.Windows.Forms.Button();
            this.DiscardChangesButton = new System.Windows.Forms.Button();
            this.CreatePanel = new System.Windows.Forms.Panel();
            this.CreateNewButton = new System.Windows.Forms.Button();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.NotesGrid = new System.Windows.Forms.DataGridView();
            this.ItemsPanel = new System.Windows.Forms.Panel();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.RequestItemsGrid = new System.Windows.Forms.DataGridView();
            this.AllowDragCheckBox = new System.Windows.Forms.CheckBox();
            this.ToolStrip = new AssetManager.UserInterface.CustomControls.OneClickToolStrip();
            this.CreateMenuButton = new System.Windows.Forms.ToolStripButton();
            this.ModifyMenuButton = new System.Windows.Forms.ToolStripButton();
            this.DeleteMenuButton = new System.Windows.Forms.ToolStripButton();
            this.AddNoteMenuButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.AttachmentsMenuButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.RefreshMenuButton = new System.Windows.Forms.ToolStripButton();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.PopupMenuItems.SuspendLayout();
            this.PopupMenuNotes.SuspendLayout();
            this.RequestPanel.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.ButtonPanel.SuspendLayout();
            this.EditButtonsPanel.SuspendLayout();
            this.CreatePanel.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.Panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.NotesGrid)).BeginInit();
            this.ItemsPanel.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RequestItemsGrid)).BeginInit();
            this.ToolStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // PopupMenuItems
            // 
            this.PopupMenuItems.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.PopupMenuItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.PopulateFAMenuItem,
            this.LookupDeviceMenuItem,
            this.GLBudgetMenuItem,
            this.CopyTextMenuItem,
            this.ImportDeviceMenuItem,
            this.MenuSeparator,
            this.DeleteRequestMenuItem});
            this.PopupMenuItems.Name = "PopupMenu";
            this.PopupMenuItems.Size = new System.Drawing.Size(179, 166);
            // 
            // PopulateFAMenuItem
            // 
            this.PopulateFAMenuItem.Image = global::AssetManager.Properties.Resources.ImportIcon;
            this.PopulateFAMenuItem.Name = "PopulateFAMenuItem";
            this.PopulateFAMenuItem.Size = new System.Drawing.Size(178, 26);
            this.PopulateFAMenuItem.Text = "Populate From FA";
            this.PopulateFAMenuItem.Click += new System.EventHandler(this.PopulateFAMenuItem_Click);
            // 
            // LookupDeviceMenuItem
            // 
            this.LookupDeviceMenuItem.Image = global::AssetManager.Properties.Resources.SearchIcon;
            this.LookupDeviceMenuItem.Name = "LookupDeviceMenuItem";
            this.LookupDeviceMenuItem.Size = new System.Drawing.Size(178, 26);
            this.LookupDeviceMenuItem.Text = "Lookup Device";
            this.LookupDeviceMenuItem.Visible = false;
            this.LookupDeviceMenuItem.Click += new System.EventHandler(this.LookupDeviceMenuItem_Click);
            // 
            // GLBudgetMenuItem
            // 
            this.GLBudgetMenuItem.Image = global::AssetManager.Properties.Resources.MoneyCircle2Icon;
            this.GLBudgetMenuItem.Name = "GLBudgetMenuItem";
            this.GLBudgetMenuItem.Size = new System.Drawing.Size(178, 26);
            this.GLBudgetMenuItem.Text = "Lookup GL/Budget";
            this.GLBudgetMenuItem.Visible = false;
            this.GLBudgetMenuItem.Click += new System.EventHandler(this.GLBudgetMenuItem_Click);
            // 
            // CopyTextMenuItem
            // 
            this.CopyTextMenuItem.Image = global::AssetManager.Properties.Resources.CopyIcon;
            this.CopyTextMenuItem.Name = "CopyTextMenuItem";
            this.CopyTextMenuItem.Size = new System.Drawing.Size(178, 26);
            this.CopyTextMenuItem.Text = "Copy Selected";
            this.CopyTextMenuItem.Click += new System.EventHandler(this.CopyTextMenuItem_Click);
            // 
            // ImportDeviceMenuItem
            // 
            this.ImportDeviceMenuItem.Image = global::AssetManager.Properties.Resources.AddIcon;
            this.ImportDeviceMenuItem.Name = "ImportDeviceMenuItem";
            this.ImportDeviceMenuItem.Size = new System.Drawing.Size(178, 26);
            this.ImportDeviceMenuItem.Text = "Import New Asset";
            this.ImportDeviceMenuItem.Click += new System.EventHandler(this.ImportDeviceMenuItem_Click);
            // 
            // MenuSeparator
            // 
            this.MenuSeparator.Name = "MenuSeparator";
            this.MenuSeparator.Size = new System.Drawing.Size(175, 6);
            // 
            // DeleteRequestMenuItem
            // 
            this.DeleteRequestMenuItem.Image = global::AssetManager.Properties.Resources.DeleteRedIcon;
            this.DeleteRequestMenuItem.Name = "DeleteRequestMenuItem";
            this.DeleteRequestMenuItem.Size = new System.Drawing.Size(178, 26);
            this.DeleteRequestMenuItem.Text = "Delete Item";
            this.DeleteRequestMenuItem.Click += new System.EventHandler(this.DeleteRequestMenuItem_Click);
            // 
            // ToolTip
            // 
            this.ToolTip.AutomaticDelay = 0;
            this.ToolTip.AutoPopDelay = 5500;
            this.ToolTip.InitialDelay = 0;
            this.ToolTip.IsBalloon = true;
            this.ToolTip.ReshowDelay = 110;
            // 
            // RTNumberTextBox
            // 
            this.RTNumberTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RTNumberTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RTNumberTextBox.Location = new System.Drawing.Point(17, 152);
            this.RTNumberTextBox.Name = "RTNumberTextBox";
            this.RTNumberTextBox.Size = new System.Drawing.Size(137, 23);
            this.RTNumberTextBox.TabIndex = 7;
            this.ToolTip.SetToolTip(this.RTNumberTextBox, "Click to open RT Ticket.");
            this.RTNumberTextBox.Click += new System.EventHandler(this.RTNumberTextBox_Click);
            // 
            // CreateDateTextBox
            // 
            this.CreateDateTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.CreateDateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.CreateDateTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateDateTextBox.ForeColor = System.Drawing.Color.Black;
            this.CreateDateTextBox.Location = new System.Drawing.Point(581, 97);
            this.CreateDateTextBox.Name = "CreateDateTextBox";
            this.CreateDateTextBox.ReadOnly = true;
            this.CreateDateTextBox.Size = new System.Drawing.Size(154, 22);
            this.CreateDateTextBox.TabIndex = 23;
            this.CreateDateTextBox.TabStop = false;
            this.CreateDateTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ToolTip.SetToolTip(this.CreateDateTextBox, "Create Date");
            this.CreateDateTextBox.WordWrap = false;
            // 
            // ModifyDateTextBox
            // 
            this.ModifyDateTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ModifyDateTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ModifyDateTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModifyDateTextBox.ForeColor = System.Drawing.Color.Black;
            this.ModifyDateTextBox.Location = new System.Drawing.Point(581, 137);
            this.ModifyDateTextBox.Name = "ModifyDateTextBox";
            this.ModifyDateTextBox.ReadOnly = true;
            this.ModifyDateTextBox.Size = new System.Drawing.Size(154, 22);
            this.ModifyDateTextBox.TabIndex = 25;
            this.ModifyDateTextBox.TabStop = false;
            this.ModifyDateTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ModifyDateTextBox.WordWrap = false;
            // 
            // ModifyByTextBox
            // 
            this.ModifyByTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ModifyByTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.ModifyByTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ModifyByTextBox.ForeColor = System.Drawing.Color.Black;
            this.ModifyByTextBox.Location = new System.Drawing.Point(581, 177);
            this.ModifyByTextBox.Name = "ModifyByTextBox";
            this.ModifyByTextBox.ReadOnly = true;
            this.ModifyByTextBox.Size = new System.Drawing.Size(154, 22);
            this.ModifyByTextBox.TabIndex = 27;
            this.ModifyByTextBox.TabStop = false;
            this.ModifyByTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.ModifyByTextBox.WordWrap = false;
            // 
            // PopupMenuNotes
            // 
            this.PopupMenuNotes.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.NewNoteMenuItem,
            this.ToolStripSeparator1,
            this.DeleteNoteMenuItem});
            this.PopupMenuNotes.Name = "PopupMenu";
            this.PopupMenuNotes.Size = new System.Drawing.Size(137, 54);
            // 
            // NewNoteMenuItem
            // 
            this.NewNoteMenuItem.Image = global::AssetManager.Properties.Resources.AddNoteIcon;
            this.NewNoteMenuItem.Name = "NewNoteMenuItem";
            this.NewNoteMenuItem.Size = new System.Drawing.Size(136, 22);
            this.NewNoteMenuItem.Text = "Add Note";
            this.NewNoteMenuItem.Click += new System.EventHandler(this.NewNoteMenuItem_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(133, 6);
            // 
            // DeleteNoteMenuItem
            // 
            this.DeleteNoteMenuItem.Image = global::AssetManager.Properties.Resources.DeleteRedIcon;
            this.DeleteNoteMenuItem.Name = "DeleteNoteMenuItem";
            this.DeleteNoteMenuItem.Size = new System.Drawing.Size(136, 22);
            this.DeleteNoteMenuItem.Text = "Delete Note";
            this.DeleteNoteMenuItem.Click += new System.EventHandler(this.DeleteNoteMenuItem_Click);
            // 
            // ContentPanel
            // 
            this.ContentPanel.AutoScroll = true;
            this.ContentPanel.Size = new System.Drawing.Size(1014, 557);
            // 
            // RequestPanel
            // 
            this.RequestPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RequestPanel.Controls.Add(this.GroupBox1);
            this.RequestPanel.Controls.Add(this.GroupBox3);
            this.RequestPanel.Location = new System.Drawing.Point(8, 40);
            this.RequestPanel.Name = "RequestPanel";
            this.RequestPanel.Size = new System.Drawing.Size(1250, 277);
            this.RequestPanel.TabIndex = 5;
            // 
            // GroupBox1
            // 
            this.GroupBox1.Controls.Add(this.label12);
            this.GroupBox1.Controls.Add(this.ModifyByTextBox);
            this.GroupBox1.Controls.Add(this.label11);
            this.GroupBox1.Controls.Add(this.ModifyDateTextBox);
            this.GroupBox1.Controls.Add(this.label10);
            this.GroupBox1.Controls.Add(this.CreateDateTextBox);
            this.GroupBox1.Controls.Add(this.Label8);
            this.GroupBox1.Controls.Add(this.RequestNumTextBox);
            this.GroupBox1.Controls.Add(this.StatusComboBox);
            this.GroupBox1.Controls.Add(this.Label7);
            this.GroupBox1.Controls.Add(this.GroupBox2);
            this.GroupBox1.Controls.Add(this.TypeComboBox);
            this.GroupBox1.Controls.Add(this.Label4);
            this.GroupBox1.Controls.Add(this.NeedByDatePicker);
            this.GroupBox1.Controls.Add(this.Label3);
            this.GroupBox1.Controls.Add(this.Label2);
            this.GroupBox1.Controls.Add(this.RequestUserTextBox);
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.DescriptionTextBox);
            this.GroupBox1.Controls.Add(this.ButtonPanel);
            this.GroupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox1.Location = new System.Drawing.Point(5, 4);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(747, 264);
            this.GroupBox1.TabIndex = 0;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Request Info";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label12.Location = new System.Drawing.Point(578, 159);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(74, 15);
            this.label12.TabIndex = 28;
            this.label12.Text = "Modified By:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.Location = new System.Drawing.Point(578, 119);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 15);
            this.label11.TabIndex = 26;
            this.label11.Text = "Modified:";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.Location = new System.Drawing.Point(578, 79);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(75, 15);
            this.label10.TabIndex = 24;
            this.label10.Text = "Create Date:";
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.Location = new System.Drawing.Point(671, 18);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(66, 15);
            this.Label8.TabIndex = 16;
            this.Label8.Text = "Request #:";
            // 
            // RequestNumTextBox
            // 
            this.RequestNumTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.RequestNumTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RequestNumTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RequestNumTextBox.Location = new System.Drawing.Point(671, 36);
            this.RequestNumTextBox.Name = "RequestNumTextBox";
            this.RequestNumTextBox.ReadOnly = true;
            this.RequestNumTextBox.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.RequestNumTextBox.Size = new System.Drawing.Size(64, 25);
            this.RequestNumTextBox.TabIndex = 15;
            this.RequestNumTextBox.TabStop = false;
            this.RequestNumTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // StatusComboBox
            // 
            this.StatusComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusComboBox.FormattingEnabled = true;
            this.StatusComboBox.Location = new System.Drawing.Point(20, 225);
            this.StatusComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.StatusComboBox.Name = "StatusComboBox";
            this.StatusComboBox.Size = new System.Drawing.Size(164, 23);
            this.StatusComboBox.TabIndex = 4;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(17, 206);
            this.Label7.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(44, 15);
            this.Label7.TabIndex = 13;
            this.Label7.Text = "Status:";
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.ReqStatusLabel);
            this.GroupBox2.Controls.Add(this.POStatusLabel);
            this.GroupBox2.Controls.Add(this.Label9);
            this.GroupBox2.Controls.Add(this.RTNumberTextBox);
            this.GroupBox2.Controls.Add(this.Label6);
            this.GroupBox2.Controls.Add(this.ReqNumberTextBox);
            this.GroupBox2.Controls.Add(this.Label5);
            this.GroupBox2.Controls.Add(this.POTextBox);
            this.GroupBox2.Location = new System.Drawing.Point(394, 43);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(171, 193);
            this.GroupBox2.TabIndex = 11;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Add\'l Info (Click to View)";
            // 
            // ReqStatusLabel
            // 
            this.ReqStatusLabel.AutoSize = true;
            this.ReqStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReqStatusLabel.ForeColor = System.Drawing.Color.DimGray;
            this.ReqStatusLabel.Location = new System.Drawing.Point(15, 120);
            this.ReqStatusLabel.Name = "ReqStatusLabel";
            this.ReqStatusLabel.Size = new System.Drawing.Size(61, 12);
            this.ReqStatusLabel.TabIndex = 11;
            this.ReqStatusLabel.Text = "Status: NA";
            // 
            // POStatusLabel
            // 
            this.POStatusLabel.AutoSize = true;
            this.POStatusLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 6.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.POStatusLabel.ForeColor = System.Drawing.Color.DimGray;
            this.POStatusLabel.Location = new System.Drawing.Point(16, 63);
            this.POStatusLabel.Name = "POStatusLabel";
            this.POStatusLabel.Size = new System.Drawing.Size(61, 12);
            this.POStatusLabel.TabIndex = 10;
            this.POStatusLabel.Text = "Status: NA";
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(14, 136);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(36, 15);
            this.Label9.TabIndex = 9;
            this.Label9.Text = "RT #:";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(14, 78);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(82, 15);
            this.Label6.TabIndex = 7;
            this.Label6.Text = "Requisition #:";
            // 
            // ReqNumberTextBox
            // 
            this.ReqNumberTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ReqNumberTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReqNumberTextBox.Location = new System.Drawing.Point(17, 94);
            this.ReqNumberTextBox.Name = "ReqNumberTextBox";
            this.ReqNumberTextBox.Size = new System.Drawing.Size(137, 23);
            this.ReqNumberTextBox.TabIndex = 6;
            this.ReqNumberTextBox.Click += new System.EventHandler(this.ReqNumberTextBox_Click);
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(14, 21);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(37, 15);
            this.Label5.TabIndex = 5;
            this.Label5.Text = "PO #:";
            // 
            // POTextBox
            // 
            this.POTextBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.POTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.POTextBox.ForeColor = System.Drawing.SystemColors.WindowText;
            this.POTextBox.Location = new System.Drawing.Point(17, 37);
            this.POTextBox.Name = "POTextBox";
            this.POTextBox.Size = new System.Drawing.Size(137, 23);
            this.POTextBox.TabIndex = 5;
            this.POTextBox.Click += new System.EventHandler(this.POTextBox_Click);
            // 
            // TypeComboBox
            // 
            this.TypeComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TypeComboBox.FormattingEnabled = true;
            this.TypeComboBox.Location = new System.Drawing.Point(20, 133);
            this.TypeComboBox.Margin = new System.Windows.Forms.Padding(2);
            this.TypeComboBox.Name = "TypeComboBox";
            this.TypeComboBox.Size = new System.Drawing.Size(164, 23);
            this.TypeComboBox.TabIndex = 2;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(17, 114);
            this.Label4.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(36, 15);
            this.Label4.TabIndex = 8;
            this.Label4.Text = "Type:";
            // 
            // NeedByDatePicker
            // 
            this.NeedByDatePicker.CustomFormat = "MM/dd/yyyy";
            this.NeedByDatePicker.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NeedByDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.NeedByDatePicker.Location = new System.Drawing.Point(20, 180);
            this.NeedByDatePicker.Margin = new System.Windows.Forms.Padding(2);
            this.NeedByDatePicker.Name = "NeedByDatePicker";
            this.NeedByDatePicker.Size = new System.Drawing.Size(164, 23);
            this.NeedByDatePicker.TabIndex = 3;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(17, 161);
            this.Label3.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(56, 15);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "Need By:";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(17, 69);
            this.Label2.Margin = new System.Windows.Forms.Padding(2, 3, 2, 2);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(85, 15);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Request User:";
            // 
            // RequestUserTextBox
            // 
            this.RequestUserTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RequestUserTextBox.Location = new System.Drawing.Point(20, 88);
            this.RequestUserTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.RequestUserTextBox.Name = "RequestUserTextBox";
            this.RequestUserTextBox.Size = new System.Drawing.Size(164, 23);
            this.RequestUserTextBox.TabIndex = 1;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(17, 25);
            this.Label1.Margin = new System.Windows.Forms.Padding(2);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(121, 15);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Request Description:";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionTextBox.Location = new System.Drawing.Point(20, 43);
            this.DescriptionTextBox.Margin = new System.Windows.Forms.Padding(2);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(356, 23);
            this.DescriptionTextBox.TabIndex = 0;
            this.DescriptionTextBox.Tag = "";
            // 
            // ButtonPanel
            // 
            this.ButtonPanel.AutoSize = true;
            this.ButtonPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonPanel.Controls.Add(this.EditButtonsPanel);
            this.ButtonPanel.Controls.Add(this.CreatePanel);
            this.ButtonPanel.Location = new System.Drawing.Point(222, 131);
            this.ButtonPanel.Name = "ButtonPanel";
            this.ButtonPanel.Size = new System.Drawing.Size(147, 85);
            this.ButtonPanel.TabIndex = 22;
            // 
            // EditButtonsPanel
            // 
            this.EditButtonsPanel.Controls.Add(this.AcceptChangesButton);
            this.EditButtonsPanel.Controls.Add(this.DiscardChangesButton);
            this.EditButtonsPanel.Location = new System.Drawing.Point(3, 4);
            this.EditButtonsPanel.Name = "EditButtonsPanel";
            this.EditButtonsPanel.Size = new System.Drawing.Size(141, 78);
            this.EditButtonsPanel.TabIndex = 20;
            this.EditButtonsPanel.Visible = false;
            // 
            // AcceptChangesButton
            // 
            this.AcceptChangesButton.BackColor = System.Drawing.Color.PaleGreen;
            this.AcceptChangesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AcceptChangesButton.Location = new System.Drawing.Point(9, 3);
            this.AcceptChangesButton.Name = "AcceptChangesButton";
            this.AcceptChangesButton.Size = new System.Drawing.Size(119, 41);
            this.AcceptChangesButton.TabIndex = 8;
            this.AcceptChangesButton.Text = "Accept Changes";
            this.AcceptChangesButton.UseVisualStyleBackColor = false;
            this.AcceptChangesButton.Click += new System.EventHandler(this.AcceptChangesButton_Click);
            // 
            // DiscardChangesButton
            // 
            this.DiscardChangesButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(128)))));
            this.DiscardChangesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DiscardChangesButton.Location = new System.Drawing.Point(9, 50);
            this.DiscardChangesButton.Name = "DiscardChangesButton";
            this.DiscardChangesButton.Size = new System.Drawing.Size(119, 24);
            this.DiscardChangesButton.TabIndex = 9;
            this.DiscardChangesButton.Text = "Discard Changes";
            this.DiscardChangesButton.UseVisualStyleBackColor = false;
            this.DiscardChangesButton.Click += new System.EventHandler(this.DiscardChangesButton_Click);
            // 
            // CreatePanel
            // 
            this.CreatePanel.Controls.Add(this.CreateNewButton);
            this.CreatePanel.Location = new System.Drawing.Point(2, 14);
            this.CreatePanel.Name = "CreatePanel";
            this.CreatePanel.Size = new System.Drawing.Size(142, 55);
            this.CreatePanel.TabIndex = 21;
            this.CreatePanel.Visible = false;
            // 
            // CreateNewButton
            // 
            this.CreateNewButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CreateNewButton.Location = new System.Drawing.Point(9, 8);
            this.CreateNewButton.Name = "CreateNewButton";
            this.CreateNewButton.Size = new System.Drawing.Size(119, 41);
            this.CreateNewButton.TabIndex = 10;
            this.CreateNewButton.Text = "Create Request";
            this.CreateNewButton.UseVisualStyleBackColor = true;
            this.CreateNewButton.Click += new System.EventHandler(this.CreateNewButton_Click);
            // 
            // GroupBox3
            // 
            this.GroupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox3.Controls.Add(this.Panel2);
            this.GroupBox3.Location = new System.Drawing.Point(758, 4);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(489, 264);
            this.GroupBox3.TabIndex = 4;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Notes";
            // 
            // Panel2
            // 
            this.Panel2.Controls.Add(this.NotesGrid);
            this.Panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Panel2.Location = new System.Drawing.Point(3, 16);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(483, 245);
            this.Panel2.TabIndex = 0;
            // 
            // NotesGrid
            // 
            this.NotesGrid.AllowUserToAddRows = false;
            this.NotesGrid.AllowUserToDeleteRows = false;
            this.NotesGrid.AllowUserToResizeRows = false;
            this.NotesGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NotesGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.NotesGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.NotesGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.NotesGrid.ColumnHeadersHeight = 25;
            this.NotesGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.NotesGrid.ContextMenuStrip = this.PopupMenuNotes;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.NotesGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.NotesGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.NotesGrid.Location = new System.Drawing.Point(3, 3);
            this.NotesGrid.Name = "NotesGrid";
            this.NotesGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.NotesGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.NotesGrid.RowHeadersVisible = false;
            this.NotesGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.NotesGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.NotesGrid.ShowCellErrors = false;
            this.NotesGrid.ShowCellToolTips = false;
            this.NotesGrid.Size = new System.Drawing.Size(477, 239);
            this.NotesGrid.TabIndex = 19;
            this.NotesGrid.VirtualMode = true;
            this.NotesGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.NotesGrid_CellDoubleClick);
            // 
            // ItemsPanel
            // 
            this.ItemsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemsPanel.Controls.Add(this.GroupBox4);
            this.ItemsPanel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ItemsPanel.Location = new System.Drawing.Point(8, 314);
            this.ItemsPanel.Name = "ItemsPanel";
            this.ItemsPanel.Size = new System.Drawing.Size(1250, 384);
            this.ItemsPanel.TabIndex = 1;
            // 
            // GroupBox4
            // 
            this.GroupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox4.Controls.Add(this.RequestItemsGrid);
            this.GroupBox4.Controls.Add(this.AllowDragCheckBox);
            this.GroupBox4.Location = new System.Drawing.Point(5, 0);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(1242, 381);
            this.GroupBox4.TabIndex = 21;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Items";
            // 
            // RequestItemsGrid
            // 
            this.RequestItemsGrid.AllowDrop = true;
            this.RequestItemsGrid.AllowUserToResizeRows = false;
            this.RequestItemsGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.RequestItemsGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.RequestItemsGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RequestItemsGrid.ColumnHeadersHeight = 38;
            this.RequestItemsGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.RequestItemsGrid.ContextMenuStrip = this.PopupMenuItems;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(205)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.RequestItemsGrid.DefaultCellStyle = dataGridViewCellStyle3;
            this.RequestItemsGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditOnEnter;
            this.RequestItemsGrid.Location = new System.Drawing.Point(6, 32);
            this.RequestItemsGrid.Name = "RequestItemsGrid";
            this.RequestItemsGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            dataGridViewCellStyle4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle4.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.RequestItemsGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            this.RequestItemsGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.RequestItemsGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.RequestItemsGrid.ShowCellToolTips = false;
            this.RequestItemsGrid.Size = new System.Drawing.Size(1230, 343);
            this.RequestItemsGrid.TabIndex = 11;
            this.RequestItemsGrid.VirtualMode = true;
            this.RequestItemsGrid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.RequestItemsGrid_CellEnter);
            this.RequestItemsGrid.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.RequestItemsGrid_CellLeave);
            this.RequestItemsGrid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.RequestItemsGrid_CellMouseDown);
            this.RequestItemsGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.RequestItemsGrid_CellValidating);
            this.RequestItemsGrid.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.RequestItemsGrid_CellValueChanged);
            this.RequestItemsGrid.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.RequestItemsGrid_DataError);
            this.RequestItemsGrid.DefaultValuesNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.RequestItemsGrid_DefaultValuesNeeded);
            this.RequestItemsGrid.RowEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.RequestItemsGrid_RowEnter);
            this.RequestItemsGrid.RowPostPaint += new System.Windows.Forms.DataGridViewRowPostPaintEventHandler(this.RequestItemsGrid_RowPostPaint);
            this.RequestItemsGrid.DragDrop += new System.Windows.Forms.DragEventHandler(this.RequestItemsGrid_DragDrop);
            this.RequestItemsGrid.DragEnter += new System.Windows.Forms.DragEventHandler(this.RequestItemsGrid_DragEnter);
            this.RequestItemsGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.RequestItemsGrid_MouseDown);
            this.RequestItemsGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.RequestItemsGrid_MouseMove);
            // 
            // AllowDragCheckBox
            // 
            this.AllowDragCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AllowDragCheckBox.AutoSize = true;
            this.AllowDragCheckBox.Location = new System.Drawing.Point(1140, 13);
            this.AllowDragCheckBox.Name = "AllowDragCheckBox";
            this.AllowDragCheckBox.Size = new System.Drawing.Size(96, 19);
            this.AllowDragCheckBox.TabIndex = 20;
            this.AllowDragCheckBox.TabStop = false;
            this.AllowDragCheckBox.Text = "Allow Drag";
            this.AllowDragCheckBox.UseVisualStyleBackColor = true;
            this.AllowDragCheckBox.CheckedChanged += new System.EventHandler(this.AllowDragCheckBox_CheckedChanged);
            // 
            // ToolStrip
            // 
            this.ToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(185)))), ((int)(((byte)(205)))), ((int)(((byte)(255)))));
            this.ToolStrip.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolStrip.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.ToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CreateMenuButton,
            this.ModifyMenuButton,
            this.DeleteMenuButton,
            this.AddNoteMenuButton,
            this.ToolStripSeparator2,
            this.AttachmentsMenuButton,
            this.ToolStripSeparator3,
            this.RefreshMenuButton});
            this.ToolStrip.Location = new System.Drawing.Point(0, 0);
            this.ToolStrip.Name = "ToolStrip";
            this.ToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ToolStrip.Size = new System.Drawing.Size(1268, 37);
            this.ToolStrip.TabIndex = 6;
            this.ToolStrip.Text = "ToolStrip1";
            // 
            // CreateMenuButton
            // 
            this.CreateMenuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.CreateMenuButton.Image = global::AssetManager.Properties.Resources.AddIcon;
            this.CreateMenuButton.Name = "CreateMenuButton";
            this.CreateMenuButton.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.CreateMenuButton.Size = new System.Drawing.Size(39, 34);
            this.CreateMenuButton.Text = "New Request";
            this.CreateMenuButton.Click += new System.EventHandler(this.CreateMenuButton_Click);
            // 
            // ModifyMenuButton
            // 
            this.ModifyMenuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ModifyMenuButton.Image = global::AssetManager.Properties.Resources.EditIcon;
            this.ModifyMenuButton.Name = "ModifyMenuButton";
            this.ModifyMenuButton.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.ModifyMenuButton.Size = new System.Drawing.Size(39, 34);
            this.ModifyMenuButton.Text = "Modify";
            this.ModifyMenuButton.ToolTipText = "Modify";
            this.ModifyMenuButton.Click += new System.EventHandler(this.ModifyButton_Click);
            // 
            // DeleteMenuButton
            // 
            this.DeleteMenuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DeleteMenuButton.Image = global::AssetManager.Properties.Resources.DeleteRedIcon;
            this.DeleteMenuButton.Name = "DeleteMenuButton";
            this.DeleteMenuButton.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.DeleteMenuButton.Size = new System.Drawing.Size(39, 34);
            this.DeleteMenuButton.Text = "Delete";
            this.DeleteMenuButton.Click += new System.EventHandler(this.DeleteMenuButton_Click);
            // 
            // AddNoteMenuButton
            // 
            this.AddNoteMenuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AddNoteMenuButton.Image = global::AssetManager.Properties.Resources.AddNoteIcon;
            this.AddNoteMenuButton.Name = "AddNoteMenuButton";
            this.AddNoteMenuButton.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.AddNoteMenuButton.Size = new System.Drawing.Size(39, 34);
            this.AddNoteMenuButton.Text = "Add Note";
            this.AddNoteMenuButton.Click += new System.EventHandler(this.AddNoteMenuButton_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 37);
            // 
            // AttachmentsMenuButton
            // 
            this.AttachmentsMenuButton.Image = global::AssetManager.Properties.Resources.PaperClipIcon;
            this.AttachmentsMenuButton.Name = "AttachmentsMenuButton";
            this.AttachmentsMenuButton.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.AttachmentsMenuButton.Size = new System.Drawing.Size(136, 34);
            this.AttachmentsMenuButton.Text = "Attachments";
            this.AttachmentsMenuButton.Click += new System.EventHandler(this.AttachmentsMenuButton_Click);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 37);
            // 
            // RefreshMenuButton
            // 
            this.RefreshMenuButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RefreshMenuButton.Image = global::AssetManager.Properties.Resources.RefreshIcon;
            this.RefreshMenuButton.Name = "RefreshMenuButton";
            this.RefreshMenuButton.Size = new System.Drawing.Size(29, 34);
            this.RefreshMenuButton.ToolTipText = "Refresh";
            this.RefreshMenuButton.Click += new System.EventHandler(this.RefreshMenuButton_Click);
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.AutoSize = false;
            this.StatusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.StatusStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusStrip1.Location = new System.Drawing.Point(0, 701);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(1268, 22);
            this.StatusStrip1.TabIndex = 7;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // SibiManageRequestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ClientSize = new System.Drawing.Size(1268, 723);
            this.Controls.Add(this.ToolStrip);
            this.Controls.Add(this.ItemsPanel);
            this.Controls.Add(this.RequestPanel);
            this.Controls.Add(this.StatusStrip1);
            this.DoubleBuffered = true;
            this.MinimizeChildren = true;
            this.MinimumSize = new System.Drawing.Size(771, 443);
            this.Name = "SibiManageRequestForm";
            this.RestoreChildren = true;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Manage Request";
            this.PopupMenuItems.ResumeLayout(false);
            this.PopupMenuNotes.ResumeLayout(false);
            this.RequestPanel.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.ButtonPanel.ResumeLayout(false);
            this.EditButtonsPanel.ResumeLayout(false);
            this.CreatePanel.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.NotesGrid)).EndInit();
            this.ItemsPanel.ResumeLayout(false);
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RequestItemsGrid)).EndInit();
            this.ToolStrip.ResumeLayout(false);
            this.ToolStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal ContextMenuStrip PopupMenuItems;
        internal ToolStripMenuItem DeleteRequestMenuItem;
        internal ToolTip ToolTip;
        internal ContextMenuStrip PopupMenuNotes;
        internal ToolStripMenuItem DeleteNoteMenuItem;
        internal ToolStripMenuItem LookupDeviceMenuItem;
        internal ToolStripMenuItem NewNoteMenuItem;
        internal ToolStripSeparator ToolStripSeparator1;
        internal OneClickToolStrip ToolStrip;
        internal ToolStripButton CreateMenuButton;
        internal ToolStripButton ModifyMenuButton;
        internal ToolStripButton DeleteMenuButton;
        internal ToolStripButton AddNoteMenuButton;
        internal ToolStripButton AttachmentsMenuButton;
        internal Panel ItemsPanel;
        internal DataGridView RequestItemsGrid;
        internal Panel RequestPanel;
        internal GroupBox GroupBox1;
        internal TextBox CreateDateTextBox;
        internal Panel ButtonPanel;
        internal Panel EditButtonsPanel;
        internal Button AcceptChangesButton;
        internal Button DiscardChangesButton;
        internal Panel CreatePanel;
        internal Button CreateNewButton;
        internal Label Label8;
        internal TextBox RequestNumTextBox;
        internal ComboBox StatusComboBox;
        internal Label Label7;
        internal GroupBox GroupBox2;
        internal Label Label9;
        internal TextBox RTNumberTextBox;
        internal Label Label6;
        internal TextBox ReqNumberTextBox;
        internal Label Label5;
        internal TextBox POTextBox;
        internal ComboBox TypeComboBox;
        internal Label Label4;
        internal DateTimePicker NeedByDatePicker;
        internal Label Label3;
        internal Label Label2;
        internal TextBox RequestUserTextBox;
        internal Label Label1;
        internal TextBox DescriptionTextBox;
        internal GroupBox GroupBox3;
        internal Panel Panel2;
        internal DataGridView NotesGrid;
        internal ToolStripContentPanel ContentPanel;
        internal CheckBox AllowDragCheckBox;
        internal ToolStripSeparator MenuSeparator;
        internal Label POStatusLabel;
        internal ToolStripSeparator ToolStripSeparator2;
        internal ToolStripSeparator ToolStripSeparator3;
        internal Label ReqStatusLabel;
        internal GroupBox GroupBox4;
        internal ToolStripMenuItem CopyTextMenuItem;
        internal ToolStripButton RefreshMenuButton;
        internal ToolStripMenuItem PopulateFAMenuItem;
        internal ToolStripMenuItem GLBudgetMenuItem;
        internal ToolStripMenuItem ImportDeviceMenuItem;
        internal StatusStrip StatusStrip1;
        internal Label label12;
        internal TextBox ModifyByTextBox;
        internal Label label11;
        internal TextBox ModifyDateTextBox;
        internal Label label10;
    }
}
