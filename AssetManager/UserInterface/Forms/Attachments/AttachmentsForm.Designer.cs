using System.Windows.Forms;
using AssetManager.UserInterface.CustomControls;
namespace AssetManager.UserInterface.Forms
{


    partial class AttachmentsForm
    {
        //Required by the Windows Form Designer
        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AttachmentsForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.UploadButton = new System.Windows.Forms.Button();
            this.RightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.OpenTool = new System.Windows.Forms.ToolStripMenuItem();
            this.SaveToMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.RenameStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.NewFolderMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.CopyTextTool = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.DeleteAttachmentToolItem = new System.Windows.Forms.ToolStripMenuItem();
            this.DeleteButton = new System.Windows.Forms.Button();
            this.OpenButton = new System.Windows.Forms.Button();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.AttachContainer = new AssetManager.UserInterface.CustomControls.HotTrackSplitContainer();
            this.FolderListView = new System.Windows.Forms.ListView();
            this.ColumnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.FolderImageList = new System.Windows.Forms.ImageList(this.components);
            this.AttachGrid = new System.Windows.Forms.DataGridView();
            this.AllowDragCheckBox = new System.Windows.Forms.CheckBox();
            this.SibiGroup = new System.Windows.Forms.GroupBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.ReqNumberTextBox = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.RequestNumTextBox = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.ReqPOTextBox = new System.Windows.Forms.TextBox();
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ProgressBar1 = new System.Windows.Forms.ToolStripProgressBar();
            this.ToolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.Spinner = new System.Windows.Forms.ToolStripStatusLabel();
            this.ThroughputLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.ToolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.CancelToolButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.DeviceGroup = new System.Windows.Forms.GroupBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.DeviceDescTextBox = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.SerialTextBox = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.AssetTagTextBox = new System.Windows.Forms.TextBox();
            this.Panel2 = new System.Windows.Forms.Panel();
            this.RightClickMenu.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AttachContainer)).BeginInit();
            this.AttachContainer.Panel1.SuspendLayout();
            this.AttachContainer.Panel2.SuspendLayout();
            this.AttachContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.AttachGrid)).BeginInit();
            this.SibiGroup.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.DeviceGroup.SuspendLayout();
            this.Panel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // UploadButton
            // 
            this.UploadButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.UploadButton.Location = new System.Drawing.Point(29, 46);
            this.UploadButton.Name = "UploadButton";
            this.UploadButton.Size = new System.Drawing.Size(92, 46);
            this.UploadButton.TabIndex = 0;
            this.UploadButton.Text = "Upload";
            this.UploadButton.UseVisualStyleBackColor = true;
            this.UploadButton.Click += new System.EventHandler(this.cmdUpload_Click);
            // 
            // RightClickMenu
            // 
            this.RightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.OpenTool,
            this.SaveToMenuItem,
            this.RenameStripMenuItem,
            this.NewFolderMenuItem,
            this.CopyTextTool,
            this.ToolStripSeparator1,
            this.DeleteAttachmentToolItem});
            this.RightClickMenu.Name = "RightClickMenu";
            this.RightClickMenu.Size = new System.Drawing.Size(174, 142);
            // 
            // OpenTool
            // 
            this.OpenTool.Image = global::AssetManager.Properties.Resources.DownloadIcon;
            this.OpenTool.Name = "OpenTool";
            this.OpenTool.Size = new System.Drawing.Size(173, 22);
            this.OpenTool.Text = "Open";
            this.OpenTool.Click += new System.EventHandler(this.OpenTool_Click);
            // 
            // SaveToMenuItem
            // 
            this.SaveToMenuItem.Image = global::AssetManager.Properties.Resources.SaveIcon;
            this.SaveToMenuItem.Name = "SaveToMenuItem";
            this.SaveToMenuItem.Size = new System.Drawing.Size(173, 22);
            this.SaveToMenuItem.Text = "Save To";
            this.SaveToMenuItem.Click += new System.EventHandler(this.SaveToMenuItem_Click);
            // 
            // RenameStripMenuItem
            // 
            this.RenameStripMenuItem.Image = global::AssetManager.Properties.Resources.EditIcon;
            this.RenameStripMenuItem.Name = "RenameStripMenuItem";
            this.RenameStripMenuItem.Size = new System.Drawing.Size(173, 22);
            this.RenameStripMenuItem.Text = "Rename";
            this.RenameStripMenuItem.Click += new System.EventHandler(this.RenameStripMenuItem_Click);
            // 
            // NewFolderMenuItem
            // 
            this.NewFolderMenuItem.Image = global::AssetManager.Properties.Resources.NewFolderIcon;
            this.NewFolderMenuItem.Name = "NewFolderMenuItem";
            this.NewFolderMenuItem.Size = new System.Drawing.Size(173, 22);
            this.NewFolderMenuItem.Text = "New Folder";
            this.NewFolderMenuItem.Click += new System.EventHandler(this.NewFolderMenuItem_Click);
            // 
            // CopyTextTool
            // 
            this.CopyTextTool.Image = global::AssetManager.Properties.Resources.CopyIcon;
            this.CopyTextTool.Name = "CopyTextTool";
            this.CopyTextTool.Size = new System.Drawing.Size(173, 22);
            this.CopyTextTool.Text = "Copy Text";
            this.CopyTextTool.Click += new System.EventHandler(this.CopyTextTool_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(170, 6);
            // 
            // DeleteAttachmentToolItem
            // 
            this.DeleteAttachmentToolItem.Image = global::AssetManager.Properties.Resources.DeleteRedIcon;
            this.DeleteAttachmentToolItem.Name = "DeleteAttachmentToolItem";
            this.DeleteAttachmentToolItem.Size = new System.Drawing.Size(173, 22);
            this.DeleteAttachmentToolItem.Text = "Delete Attachment";
            this.DeleteAttachmentToolItem.Click += new System.EventHandler(this.DeleteAttachmentToolItem_Click);
            // 
            // DeleteButton
            // 
            this.DeleteButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeleteButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DeleteButton.Location = new System.Drawing.Point(29, 160);
            this.DeleteButton.Name = "DeleteButton";
            this.DeleteButton.Size = new System.Drawing.Size(92, 25);
            this.DeleteButton.TabIndex = 4;
            this.DeleteButton.Text = "Delete";
            this.DeleteButton.UseVisualStyleBackColor = true;
            this.DeleteButton.Click += new System.EventHandler(this.DeleteButton_Click);
            // 
            // OpenButton
            // 
            this.OpenButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OpenButton.Location = new System.Drawing.Point(29, 98);
            this.OpenButton.Name = "OpenButton";
            this.OpenButton.Size = new System.Drawing.Size(92, 27);
            this.OpenButton.TabIndex = 5;
            this.OpenButton.Text = "Open";
            this.OpenButton.UseVisualStyleBackColor = true;
            this.OpenButton.Click += new System.EventHandler(this.OpenButton_Click);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GroupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.GroupBox1.Controls.Add(this.Panel1);
            this.GroupBox1.Controls.Add(this.OpenButton);
            this.GroupBox1.Controls.Add(this.DeleteButton);
            this.GroupBox1.Controls.Add(this.UploadButton);
            this.GroupBox1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox1.Location = new System.Drawing.Point(12, 113);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(793, 440);
            this.GroupBox1.TabIndex = 6;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Attachments";
            // 
            // Panel1
            // 
            this.Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel1.Controls.Add(this.AttachContainer);
            this.Panel1.Controls.Add(this.AllowDragCheckBox);
            this.Panel1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Panel1.Location = new System.Drawing.Point(150, 21);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(637, 413);
            this.Panel1.TabIndex = 19;
            // 
            // AttachContainer
            // 
            this.AttachContainer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.AttachContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.AttachContainer.HotTracking = true;
            this.AttachContainer.Location = new System.Drawing.Point(3, 22);
            this.AttachContainer.Name = "AttachContainer";
            // 
            // AttachContainer.Panel1
            // 
            this.AttachContainer.Panel1.Controls.Add(this.FolderListView);
            // 
            // AttachContainer.Panel2
            // 
            this.AttachContainer.Panel2.Controls.Add(this.AttachGrid);
            this.AttachContainer.Size = new System.Drawing.Size(629, 386);
            this.AttachContainer.SplitterDistance = 148;
            this.AttachContainer.TabIndex = 23;
            // 
            // FolderListView
            // 
            this.FolderListView.AllowDrop = true;
            this.FolderListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.FolderListView.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.FolderListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.ColumnHeader1});
            this.FolderListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FolderListView.ForeColor = System.Drawing.Color.White;
            this.FolderListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.FolderListView.Location = new System.Drawing.Point(0, 0);
            this.FolderListView.MultiSelect = false;
            this.FolderListView.Name = "FolderListView";
            this.FolderListView.Size = new System.Drawing.Size(148, 386);
            this.FolderListView.StateImageList = this.FolderImageList;
            this.FolderListView.TabIndex = 0;
            this.FolderListView.UseCompatibleStateImageBehavior = false;
            this.FolderListView.View = System.Windows.Forms.View.Details;
            this.FolderListView.ItemActivate += new System.EventHandler(this.FolderListView_ItemActivate);
            this.FolderListView.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.FolderListView_ItemSelectionChanged);
            this.FolderListView.DragDrop += new System.Windows.Forms.DragEventHandler(this.FolderListView_DragDrop);
            this.FolderListView.DragOver += new System.Windows.Forms.DragEventHandler(this.FolderListView_DragOver);
            this.FolderListView.DragLeave += new System.EventHandler(this.FolderListView_DragLeave);
            // 
            // ColumnHeader1
            // 
            this.ColumnHeader1.Text = "Folder";
            this.ColumnHeader1.Width = 148;
            // 
            // FolderImageList
            // 
            this.FolderImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("FolderImageList.ImageStream")));
            this.FolderImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.FolderImageList.Images.SetKeyName(0, "icons8-Folder-48.png");
            this.FolderImageList.Images.SetKeyName(1, "icons8-Open-48.png");
            // 
            // AttachGrid
            // 
            this.AttachGrid.AllowDrop = true;
            this.AttachGrid.AllowUserToAddRows = false;
            this.AttachGrid.AllowUserToDeleteRows = false;
            this.AttachGrid.AllowUserToResizeRows = false;
            this.AttachGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.AttachGrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.AttachGrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.AttachGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.AttachGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.AttachGrid.ContextMenuStrip = this.RightClickMenu;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(10, 0, 10, 0);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(152)))), ((int)(((byte)(39)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.Black;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.AttachGrid.DefaultCellStyle = dataGridViewCellStyle1;
            this.AttachGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.AttachGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.AttachGrid.Location = new System.Drawing.Point(0, 0);
            this.AttachGrid.MultiSelect = false;
            this.AttachGrid.Name = "AttachGrid";
            this.AttachGrid.ReadOnly = true;
            this.AttachGrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.White;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.AttachGrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.AttachGrid.RowHeadersVisible = false;
            this.AttachGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.AttachGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.AttachGrid.ShowCellErrors = false;
            this.AttachGrid.ShowCellToolTips = false;
            this.AttachGrid.ShowEditingIcon = false;
            this.AttachGrid.Size = new System.Drawing.Size(477, 386);
            this.AttachGrid.TabIndex = 18;
            this.AttachGrid.VirtualMode = true;
            this.AttachGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.AttachGrid_CellDoubleClick);
            this.AttachGrid.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.AttachGrid_CellEndEdit);
            this.AttachGrid.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.AttachGrid_CellEnter);
            this.AttachGrid.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.AttachGrid_CellFormatting);
            this.AttachGrid.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.AttachGrid_CellLeave);
            this.AttachGrid.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.AttachGrid_CellMouseDown);
            this.AttachGrid.CellMouseUp += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.AttachGrid_CellMouseUp);
            this.AttachGrid.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.AttachGrid_CellValidating);
            this.AttachGrid.DragDrop += new System.Windows.Forms.DragEventHandler(this.AttachGrid_DragDrop);
            this.AttachGrid.DragOver += new System.Windows.Forms.DragEventHandler(this.AttachGrid_DragOver);
            this.AttachGrid.DragLeave += new System.EventHandler(this.AttachGrid_DragLeave);
            this.AttachGrid.QueryContinueDrag += new System.Windows.Forms.QueryContinueDragEventHandler(this.AttachGrid_QueryContinueDrag);
            this.AttachGrid.KeyDown += new System.Windows.Forms.KeyEventHandler(this.AttachGrid_KeyDown);
            this.AttachGrid.MouseDown += new System.Windows.Forms.MouseEventHandler(this.AttachGrid_MouseDown);
            this.AttachGrid.MouseMove += new System.Windows.Forms.MouseEventHandler(this.AttachGrid_MouseMove);
            // 
            // AllowDragCheckBox
            // 
            this.AllowDragCheckBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.AllowDragCheckBox.AutoSize = true;
            this.AllowDragCheckBox.Checked = true;
            this.AllowDragCheckBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.AllowDragCheckBox.Location = new System.Drawing.Point(543, 3);
            this.AllowDragCheckBox.Name = "AllowDragCheckBox";
            this.AllowDragCheckBox.Size = new System.Drawing.Size(89, 19);
            this.AllowDragCheckBox.TabIndex = 21;
            this.AllowDragCheckBox.Text = "Drag-Drop";
            this.AllowDragCheckBox.UseVisualStyleBackColor = true;
            this.AllowDragCheckBox.Click += new System.EventHandler(this.AllowDragCheckBox_Click);
            // 
            // SibiGroup
            // 
            this.SibiGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.SibiGroup.Controls.Add(this.Label4);
            this.SibiGroup.Controls.Add(this.ReqNumberTextBox);
            this.SibiGroup.Controls.Add(this.Label3);
            this.SibiGroup.Controls.Add(this.DescriptionTextBox);
            this.SibiGroup.Controls.Add(this.Label2);
            this.SibiGroup.Controls.Add(this.RequestNumTextBox);
            this.SibiGroup.Controls.Add(this.Label1);
            this.SibiGroup.Controls.Add(this.ReqPOTextBox);
            this.SibiGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SibiGroup.Location = new System.Drawing.Point(0, 3);
            this.SibiGroup.Name = "SibiGroup";
            this.SibiGroup.Size = new System.Drawing.Size(793, 95);
            this.SibiGroup.TabIndex = 7;
            this.SibiGroup.TabStop = false;
            this.SibiGroup.Text = "Request Info";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(598, 29);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(47, 16);
            this.Label4.TabIndex = 7;
            this.Label4.Text = "Req #:";
            // 
            // ReqNumberTextBox
            // 
            this.ReqNumberTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReqNumberTextBox.Location = new System.Drawing.Point(601, 49);
            this.ReqNumberTextBox.Name = "ReqNumberTextBox";
            this.ReqNumberTextBox.ReadOnly = true;
            this.ReqNumberTextBox.Size = new System.Drawing.Size(120, 25);
            this.ReqNumberTextBox.TabIndex = 6;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(20, 29);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(79, 16);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "Description:";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionTextBox.Location = new System.Drawing.Point(23, 49);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.ReadOnly = true;
            this.DescriptionTextBox.Size = new System.Drawing.Size(259, 25);
            this.DescriptionTextBox.TabIndex = 4;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(317, 29);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(72, 16);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Request #:";
            // 
            // RequestNumTextBox
            // 
            this.RequestNumTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.RequestNumTextBox.Location = new System.Drawing.Point(320, 49);
            this.RequestNumTextBox.Name = "RequestNumTextBox";
            this.RequestNumTextBox.ReadOnly = true;
            this.RequestNumTextBox.Size = new System.Drawing.Size(83, 25);
            this.RequestNumTextBox.TabIndex = 2;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(439, 29);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(40, 16);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "PO #:";
            // 
            // ReqPOTextBox
            // 
            this.ReqPOTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReqPOTextBox.Location = new System.Drawing.Point(442, 49);
            this.ReqPOTextBox.Name = "ReqPOTextBox";
            this.ReqPOTextBox.ReadOnly = true;
            this.ReqPOTextBox.Size = new System.Drawing.Size(120, 25);
            this.ReqPOTextBox.TabIndex = 0;
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel,
            this.ProgressBar1,
            this.ToolStripStatusLabel2,
            this.Spinner,
            this.ThroughputLabel,
            this.ToolStripStatusLabel1,
            this.CancelToolButton});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 556);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(817, 22);
            this.StatusStrip1.TabIndex = 8;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Padding = new System.Windows.Forms.Padding(0, 0, 10, 0);
            this.StatusLabel.Size = new System.Drawing.Size(86, 17);
            this.StatusLabel.Text = "%STATUS%";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // ProgressBar1
            // 
            this.ProgressBar1.AutoSize = false;
            this.ProgressBar1.Name = "ProgressBar1";
            this.ProgressBar1.Size = new System.Drawing.Size(150, 16);
            this.ProgressBar1.Step = 1;
            this.ProgressBar1.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.ProgressBar1.Visible = false;
            // 
            // ToolStripStatusLabel2
            // 
            this.ToolStripStatusLabel2.AutoSize = false;
            this.ToolStripStatusLabel2.Name = "ToolStripStatusLabel2";
            this.ToolStripStatusLabel2.Size = new System.Drawing.Size(10, 17);
            // 
            // Spinner
            // 
            this.Spinner.Image = global::AssetManager.Properties.Resources.LoadingAni;
            this.Spinner.Name = "Spinner";
            this.Spinner.Size = new System.Drawing.Size(16, 17);
            this.Spinner.Visible = false;
            // 
            // ThroughputLabel
            // 
            this.ThroughputLabel.Font = new System.Drawing.Font("Segoe UI Semibold", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ThroughputLabel.Name = "ThroughputLabel";
            this.ThroughputLabel.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.ThroughputLabel.Size = new System.Drawing.Size(10, 17);
            // 
            // ToolStripStatusLabel1
            // 
            this.ToolStripStatusLabel1.Name = "ToolStripStatusLabel1";
            this.ToolStripStatusLabel1.Size = new System.Drawing.Size(696, 17);
            this.ToolStripStatusLabel1.Spring = true;
            // 
            // CancelToolButton
            // 
            this.CancelToolButton.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelToolButton.Image = global::AssetManager.Properties.Resources.CloseCancelDeleteIcon;
            this.CancelToolButton.Name = "CancelToolButton";
            this.CancelToolButton.ShowDropDownArrow = false;
            this.CancelToolButton.Size = new System.Drawing.Size(63, 20);
            this.CancelToolButton.Text = "Cancel";
            this.CancelToolButton.Visible = false;
            this.CancelToolButton.Click += new System.EventHandler(this.CancelToolButton_Click);
            // 
            // DeviceGroup
            // 
            this.DeviceGroup.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.DeviceGroup.Controls.Add(this.Label5);
            this.DeviceGroup.Controls.Add(this.DeviceDescTextBox);
            this.DeviceGroup.Controls.Add(this.Label6);
            this.DeviceGroup.Controls.Add(this.SerialTextBox);
            this.DeviceGroup.Controls.Add(this.Label7);
            this.DeviceGroup.Controls.Add(this.AssetTagTextBox);
            this.DeviceGroup.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeviceGroup.Location = new System.Drawing.Point(817, 3);
            this.DeviceGroup.Name = "DeviceGroup";
            this.DeviceGroup.Size = new System.Drawing.Size(633, 95);
            this.DeviceGroup.TabIndex = 9;
            this.DeviceGroup.TabStop = false;
            this.DeviceGroup.Text = "Device Info";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(331, 29);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(79, 16);
            this.Label5.TabIndex = 5;
            this.Label5.Text = "Description:";
            // 
            // DeviceDescTextBox
            // 
            this.DeviceDescTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeviceDescTextBox.Location = new System.Drawing.Point(334, 49);
            this.DeviceDescTextBox.Name = "DeviceDescTextBox";
            this.DeviceDescTextBox.ReadOnly = true;
            this.DeviceDescTextBox.Size = new System.Drawing.Size(259, 25);
            this.DeviceDescTextBox.TabIndex = 4;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(175, 29);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(46, 16);
            this.Label6.TabIndex = 3;
            this.Label6.Text = "Serial:";
            // 
            // SerialTextBox
            // 
            this.SerialTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialTextBox.Location = new System.Drawing.Point(178, 49);
            this.SerialTextBox.Name = "SerialTextBox";
            this.SerialTextBox.ReadOnly = true;
            this.SerialTextBox.Size = new System.Drawing.Size(105, 25);
            this.SerialTextBox.TabIndex = 2;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(30, 29);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(73, 16);
            this.Label7.TabIndex = 1;
            this.Label7.Text = "Asset Tag:";
            // 
            // AssetTagTextBox
            // 
            this.AssetTagTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AssetTagTextBox.Location = new System.Drawing.Point(33, 49);
            this.AssetTagTextBox.Name = "AssetTagTextBox";
            this.AssetTagTextBox.ReadOnly = true;
            this.AssetTagTextBox.Size = new System.Drawing.Size(105, 25);
            this.AssetTagTextBox.TabIndex = 0;
            // 
            // Panel2
            // 
            this.Panel2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel2.Controls.Add(this.DeviceGroup);
            this.Panel2.Controls.Add(this.SibiGroup);
            this.Panel2.Location = new System.Drawing.Point(12, 12);
            this.Panel2.Name = "Panel2";
            this.Panel2.Size = new System.Drawing.Size(793, 116);
            this.Panel2.TabIndex = 10;
            // 
            // AttachmentsForm
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ClientSize = new System.Drawing.Size(817, 578);
            this.Controls.Add(this.StatusStrip1);
            this.Controls.Add(this.GroupBox1);
            this.Controls.Add(this.Panel2);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(833, 370);
            this.Name = "AttachmentsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Attachments";
            this.Load += new System.EventHandler(this.AttachmentsForm_Load);
            this.Shown += new System.EventHandler(this.AttachmentsForm_Shown);
            this.RightClickMenu.ResumeLayout(false);
            this.GroupBox1.ResumeLayout(false);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.AttachContainer.Panel1.ResumeLayout(false);
            this.AttachContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AttachContainer)).EndInit();
            this.AttachContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.AttachGrid)).EndInit();
            this.SibiGroup.ResumeLayout(false);
            this.SibiGroup.PerformLayout();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.DeviceGroup.ResumeLayout(false);
            this.DeviceGroup.PerformLayout();
            this.Panel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal Button UploadButton;
        internal ContextMenuStrip RightClickMenu;
        internal ToolStripMenuItem DeleteAttachmentToolItem;
        internal Button DeleteButton;
        internal Button OpenButton;
        internal GroupBox GroupBox1;
        internal GroupBox SibiGroup;
        internal Label Label3;
        internal TextBox DescriptionTextBox;
        internal Label Label2;
        internal TextBox RequestNumTextBox;
        internal Label Label1;
        internal TextBox ReqPOTextBox;
        internal StatusStrip StatusStrip1;
        internal ToolStripStatusLabel StatusLabel;
        internal ToolStripStatusLabel Spinner;
        internal ToolStripMenuItem OpenTool;
        internal ToolStripSeparator ToolStripSeparator1;
        internal ToolStripProgressBar ProgressBar1;
        internal ToolStripStatusLabel ToolStripStatusLabel2;
        internal ToolStripStatusLabel ThroughputLabel;
        internal DataGridView AttachGrid;
        internal ToolStripMenuItem CopyTextTool;
        internal Panel Panel1;
        internal ToolStripStatusLabel ToolStripStatusLabel1;
        internal ToolStripDropDownButton CancelToolButton;
        internal ToolStripMenuItem RenameStripMenuItem;
        internal CheckBox AllowDragCheckBox;
        internal GroupBox DeviceGroup;
        internal Label Label5;
        internal TextBox DeviceDescTextBox;
        internal Label Label6;
        internal TextBox SerialTextBox;
        internal Label Label7;
        internal TextBox AssetTagTextBox;
        internal Panel Panel2;
        internal ToolStripMenuItem SaveToMenuItem;
        internal Label Label4;
        internal TextBox ReqNumberTextBox;
        internal HotTrackSplitContainer AttachContainer;
        internal ListView FolderListView;
        internal ColumnHeader ColumnHeader1;
        internal ImageList FolderImageList;
        internal ToolStripMenuItem NewFolderMenuItem;

    }
}
