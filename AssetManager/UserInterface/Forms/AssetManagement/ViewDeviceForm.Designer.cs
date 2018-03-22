using System.Windows.Forms;
using AssetManager.UserInterface.CustomControls;

namespace AssetManager.UserInterface.Forms.AssetManagement
{

    partial class ViewDeviceForm
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
            this.HostnameTextBox = new System.Windows.Forms.TextBox();
            this.Label15 = new System.Windows.Forms.Label();
            this.PhoneNumberTextBox = new System.Windows.Forms.MaskedTextBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.GuidLabel = new System.Windows.Forms.Label();
            this.MunisSearchButton = new System.Windows.Forms.Button();
            this.MunisSibiPanel = new System.Windows.Forms.Panel();
            this.MunisInfoButton = new System.Windows.Forms.Button();
            this.SibiViewButton = new System.Windows.Forms.Button();
            this.Label12 = new System.Windows.Forms.Label();
            this.PONumberTextBox = new System.Windows.Forms.TextBox();
            this.TrackableCheckBox = new System.Windows.Forms.CheckBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.AssetTagTextBox = new System.Windows.Forms.TextBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.SerialTextBox = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label9 = new System.Windows.Forms.Label();
            this.CurrentUserTextBox = new System.Windows.Forms.TextBox();
            this.StatusComboBox = new System.Windows.Forms.ComboBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label8 = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.OSVersionComboBox = new System.Windows.Forms.ComboBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.LocationComboBox = new System.Windows.Forms.ComboBox();
            this.Label13 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.EquipTypeComboBox = new System.Windows.Forms.ComboBox();
            this.PurchaseDatePicker = new System.Windows.Forms.DateTimePicker();
            this.Label7 = new System.Windows.Forms.Label();
            this.Label6 = new System.Windows.Forms.Label();
            this.ReplaceYearTextBox = new System.Windows.Forms.TextBox();
            this.RightClickMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.DeleteEntryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.TabControl1 = new System.Windows.Forms.TabControl();
            this.HistoryTab = new System.Windows.Forms.TabPage();
            this.DataGridHistory = new System.Windows.Forms.DataGridView();
            this.TrackingTab = new System.Windows.Forms.TabPage();
            this.TrackingGrid = new System.Windows.Forms.DataGridView();
            this.TrackingBox = new System.Windows.Forms.GroupBox();
            this.Panel3 = new System.Windows.Forms.Panel();
            this.TrackingLocationTextBox = new System.Windows.Forms.TextBox();
            this.Label16 = new System.Windows.Forms.Label();
            this.DueBackTextBox = new System.Windows.Forms.TextBox();
            this.DueBackLabel = new System.Windows.Forms.Label();
            this.CheckUserTextBox = new System.Windows.Forms.TextBox();
            this.CheckUserLabel = new System.Windows.Forms.Label();
            this.CheckTimeTextBox = new System.Windows.Forms.TextBox();
            this.CheckTimeLabel = new System.Windows.Forms.Label();
            this.TrackingStatusTextBox = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.StatusStrip1 = new System.Windows.Forms.StatusStrip();
            this.StatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.AcceptCancelToolStrip = new AssetManager.UserInterface.CustomControls.OneClickToolStrip();
            this.AcceptToolButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.CancelToolButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.FieldsPanel = new System.Windows.Forms.Panel();
            this.InfoDataSplitter = new System.Windows.Forms.SplitContainer();
            this.FieldTabs = new System.Windows.Forms.TabControl();
            this.AssetInfoTab = new System.Windows.Forms.TabPage();
            this.MiscInfoTab = new System.Windows.Forms.TabPage();
            this.PingHistLabel = new System.Windows.Forms.Label();
            this.ActiveDirectoryBox = new System.Windows.Forms.GroupBox();
            this.Label22 = new System.Windows.Forms.Label();
            this.ADCreatedTextBox = new System.Windows.Forms.TextBox();
            this.Label21 = new System.Windows.Forms.Label();
            this.ADLastLoginTextBox = new System.Windows.Forms.TextBox();
            this.Label20 = new System.Windows.Forms.Label();
            this.ADOSVerTextBox = new System.Windows.Forms.TextBox();
            this.Label19 = new System.Windows.Forms.Label();
            this.ADOSTextBox = new System.Windows.Forms.TextBox();
            this.Label18 = new System.Windows.Forms.Label();
            this.ADOUTextBox = new System.Windows.Forms.TextBox();
            this.iCloudTextBox = new System.Windows.Forms.TextBox();
            this.Label17 = new System.Windows.Forms.Label();
            this.RemoteTrackingPanel = new System.Windows.Forms.Panel();
            this.remoteToolsControl = new AssetManager.UserInterface.CustomControls.RemoteToolsControl();
            this.ToolStripContainer1 = new System.Windows.Forms.ToolStripContainer();
            this.TrackingToolStrip = new System.Windows.Forms.ToolStrip();
            this.TrackingDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.CheckOutTool = new System.Windows.Forms.ToolStripMenuItem();
            this.CheckInTool = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStrip1 = new AssetManager.UserInterface.CustomControls.OneClickToolStrip();
            this.ModifyToolButton = new System.Windows.Forms.ToolStripButton();
            this.NewNoteToolButton = new System.Windows.Forms.ToolStripButton();
            this.DeleteDeviceToolButton = new System.Windows.Forms.ToolStripButton();
            this.RefreshToolButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.AttachmentsToolButton = new System.Windows.Forms.ToolStripButton();
            this.ToolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
            this.AssetControlDropDown = new System.Windows.Forms.ToolStripDropDownButton();
            this.AssetInputFormToolItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AssetTransferFormToolItem = new System.Windows.Forms.ToolStripMenuItem();
            this.AssetDisposalFormToolItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ToolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.MunisSibiPanel.SuspendLayout();
            this.RightClickMenu.SuspendLayout();
            this.TabControl1.SuspendLayout();
            this.HistoryTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.DataGridHistory)).BeginInit();
            this.TrackingTab.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.TrackingGrid)).BeginInit();
            this.TrackingBox.SuspendLayout();
            this.Panel3.SuspendLayout();
            this.StatusStrip1.SuspendLayout();
            this.AcceptCancelToolStrip.SuspendLayout();
            this.FieldsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.InfoDataSplitter)).BeginInit();
            this.InfoDataSplitter.Panel1.SuspendLayout();
            this.InfoDataSplitter.Panel2.SuspendLayout();
            this.InfoDataSplitter.SuspendLayout();
            this.FieldTabs.SuspendLayout();
            this.AssetInfoTab.SuspendLayout();
            this.MiscInfoTab.SuspendLayout();
            this.ActiveDirectoryBox.SuspendLayout();
            this.RemoteTrackingPanel.SuspendLayout();
            this.ToolStripContainer1.ContentPanel.SuspendLayout();
            this.ToolStripContainer1.TopToolStripPanel.SuspendLayout();
            this.ToolStripContainer1.SuspendLayout();
            this.TrackingToolStrip.SuspendLayout();
            this.ToolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // HostnameTextBox
            // 
            this.HostnameTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HostnameTextBox.Location = new System.Drawing.Point(26, 42);
            this.HostnameTextBox.Name = "HostnameTextBox";
            this.HostnameTextBox.Size = new System.Drawing.Size(177, 23);
            this.HostnameTextBox.TabIndex = 58;
            // 
            // Label15
            // 
            this.Label15.AutoSize = true;
            this.Label15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label15.Location = new System.Drawing.Point(23, 23);
            this.Label15.Name = "Label15";
            this.Label15.Size = new System.Drawing.Size(73, 16);
            this.Label15.TabIndex = 59;
            this.Label15.Text = "Hostname:";
            // 
            // PhoneNumberTextBox
            // 
            this.PhoneNumberTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PhoneNumberTextBox.Location = new System.Drawing.Point(26, 94);
            this.PhoneNumberTextBox.Mask = "(999) 000-0000";
            this.PhoneNumberTextBox.Name = "PhoneNumberTextBox";
            this.PhoneNumberTextBox.Size = new System.Drawing.Size(177, 23);
            this.PhoneNumberTextBox.TabIndex = 57;
            this.PhoneNumberTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.PhoneNumberTextBox.Leave += new System.EventHandler(this.PhoneNumberTextBox_Leave);
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label14.Location = new System.Drawing.Point(23, 75);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(60, 16);
            this.Label14.TabIndex = 56;
            this.Label14.Text = "Phone #:";
            // 
            // GuidLabel
            // 
            this.GuidLabel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.GuidLabel.Cursor = System.Windows.Forms.Cursors.Default;
            this.GuidLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GuidLabel.Location = new System.Drawing.Point(26, 149);
            this.GuidLabel.Name = "GuidLabel";
            this.GuidLabel.Size = new System.Drawing.Size(272, 23);
            this.GuidLabel.TabIndex = 54;
            this.GuidLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.ToolTip1.SetToolTip(this.GuidLabel, "Click to copy Guid.");
            this.GuidLabel.Click += new System.EventHandler(this.GuidLabel_Click);
            // 
            // MunisSearchButton
            // 
            this.MunisSearchButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MunisSearchButton.Location = new System.Drawing.Point(20, 155);
            this.MunisSearchButton.Name = "MunisSearchButton";
            this.MunisSearchButton.Size = new System.Drawing.Size(135, 23);
            this.MunisSearchButton.TabIndex = 3;
            this.MunisSearchButton.Text = "Munis Search";
            this.MunisSearchButton.UseVisualStyleBackColor = true;
            this.MunisSearchButton.Visible = false;
            this.MunisSearchButton.Click += new System.EventHandler(this.MunisSearchButton_Click);
            // 
            // MunisSibiPanel
            // 
            this.MunisSibiPanel.Controls.Add(this.MunisInfoButton);
            this.MunisSibiPanel.Controls.Add(this.SibiViewButton);
            this.MunisSibiPanel.Location = new System.Drawing.Point(584, 168);
            this.MunisSibiPanel.Name = "MunisSibiPanel";
            this.MunisSibiPanel.Size = new System.Drawing.Size(194, 61);
            this.MunisSibiPanel.TabIndex = 51;
            // 
            // MunisInfoButton
            // 
            this.MunisInfoButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.MunisInfoButton.Location = new System.Drawing.Point(14, 3);
            this.MunisInfoButton.Name = "MunisInfoButton";
            this.MunisInfoButton.Size = new System.Drawing.Size(170, 23);
            this.MunisInfoButton.TabIndex = 46;
            this.MunisInfoButton.Text = "View MUNIS";
            this.ToolTip1.SetToolTip(this.MunisInfoButton, "View Munis");
            this.MunisInfoButton.UseVisualStyleBackColor = true;
            this.MunisInfoButton.Click += new System.EventHandler(this.MunisInfoButton_Click);
            // 
            // SibiViewButton
            // 
            this.SibiViewButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.SibiViewButton.Location = new System.Drawing.Point(14, 32);
            this.SibiViewButton.Name = "SibiViewButton";
            this.SibiViewButton.Size = new System.Drawing.Size(170, 23);
            this.SibiViewButton.TabIndex = 49;
            this.SibiViewButton.Text = "View Sibi";
            this.SibiViewButton.UseVisualStyleBackColor = true;
            this.SibiViewButton.Click += new System.EventHandler(this.SibiViewButton_Click);
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label12.Location = new System.Drawing.Point(595, 110);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(81, 16);
            this.Label12.TabIndex = 48;
            this.Label12.Text = "PO Number:";
            // 
            // PONumberTextBox
            // 
            this.PONumberTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PONumberTextBox.Location = new System.Drawing.Point(598, 129);
            this.PONumberTextBox.Name = "PONumberTextBox";
            this.PONumberTextBox.Size = new System.Drawing.Size(169, 23);
            this.PONumberTextBox.TabIndex = 11;
            // 
            // TrackableCheckBox
            // 
            this.TrackableCheckBox.AutoSize = true;
            this.TrackableCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackableCheckBox.Location = new System.Drawing.Point(26, 190);
            this.TrackableCheckBox.Name = "TrackableCheckBox";
            this.TrackableCheckBox.Size = new System.Drawing.Size(89, 20);
            this.TrackableCheckBox.TabIndex = 13;
            this.TrackableCheckBox.Text = "Trackable";
            this.TrackableCheckBox.UseVisualStyleBackColor = true;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(18, 62);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(73, 16);
            this.Label1.TabIndex = 20;
            this.Label1.Text = "Asset Tag:";
            // 
            // AssetTagTextBox
            // 
            this.AssetTagTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AssetTagTextBox.Location = new System.Drawing.Point(21, 81);
            this.AssetTagTextBox.Name = "AssetTagTextBox";
            this.AssetTagTextBox.Size = new System.Drawing.Size(134, 23);
            this.AssetTagTextBox.TabIndex = 1;
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label10.Location = new System.Drawing.Point(22, 130);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(85, 16);
            this.Label10.TabIndex = 41;
            this.Label10.Text = "Device Guid:";
            // 
            // SerialTextBox
            // 
            this.SerialTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialTextBox.Location = new System.Drawing.Point(21, 33);
            this.SerialTextBox.Name = "SerialTextBox";
            this.SerialTextBox.Size = new System.Drawing.Size(134, 23);
            this.SerialTextBox.TabIndex = 0;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(18, 14);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(46, 16);
            this.Label2.TabIndex = 22;
            this.Label2.Text = "Serial:";
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(390, 110);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(48, 16);
            this.Label9.TabIndex = 39;
            this.Label9.Text = "Status:";
            // 
            // CurrentUserTextBox
            // 
            this.CurrentUserTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentUserTextBox.Location = new System.Drawing.Point(21, 129);
            this.CurrentUserTextBox.Name = "CurrentUserTextBox";
            this.CurrentUserTextBox.Size = new System.Drawing.Size(134, 23);
            this.CurrentUserTextBox.TabIndex = 2;
            this.CurrentUserTextBox.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.CurrentUserTextBox_MouseDoubleClick);
            // 
            // StatusComboBox
            // 
            this.StatusComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusComboBox.FormattingEnabled = true;
            this.StatusComboBox.Location = new System.Drawing.Point(393, 129);
            this.StatusComboBox.Name = "StatusComboBox";
            this.StatusComboBox.Size = new System.Drawing.Size(177, 23);
            this.StatusComboBox.TabIndex = 8;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(17, 110);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(85, 16);
            this.Label3.TabIndex = 24;
            this.Label3.Text = "Current User:";
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.Location = new System.Drawing.Point(178, 110);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(79, 16);
            this.Label8.TabIndex = 37;
            this.Label8.Text = "OS Version:";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionTextBox.Location = new System.Drawing.Point(181, 33);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(389, 23);
            this.DescriptionTextBox.TabIndex = 4;
            // 
            // OSVersionComboBox
            // 
            this.OSVersionComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OSVersionComboBox.FormattingEnabled = true;
            this.OSVersionComboBox.Location = new System.Drawing.Point(181, 129);
            this.OSVersionComboBox.Name = "OSVersionComboBox";
            this.OSVersionComboBox.Size = new System.Drawing.Size(177, 23);
            this.OSVersionComboBox.TabIndex = 6;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(176, 14);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(79, 16);
            this.Label4.TabIndex = 26;
            this.Label4.Text = "Description:";
            // 
            // LocationComboBox
            // 
            this.LocationComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocationComboBox.FormattingEnabled = true;
            this.LocationComboBox.Location = new System.Drawing.Point(393, 81);
            this.LocationComboBox.Name = "LocationComboBox";
            this.LocationComboBox.Size = new System.Drawing.Size(177, 23);
            this.LocationComboBox.TabIndex = 7;
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label13.Location = new System.Drawing.Point(178, 62);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(110, 16);
            this.Label13.TabIndex = 34;
            this.Label13.Text = "Equipment Type:";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(390, 62);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(62, 16);
            this.Label5.TabIndex = 28;
            this.Label5.Text = "Location:";
            // 
            // EquipTypeComboBox
            // 
            this.EquipTypeComboBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EquipTypeComboBox.FormattingEnabled = true;
            this.EquipTypeComboBox.Location = new System.Drawing.Point(181, 81);
            this.EquipTypeComboBox.Name = "EquipTypeComboBox";
            this.EquipTypeComboBox.Size = new System.Drawing.Size(177, 23);
            this.EquipTypeComboBox.TabIndex = 5;
            // 
            // PurchaseDatePicker
            // 
            this.PurchaseDatePicker.CustomFormat = "yyyy-MM-dd";
            this.PurchaseDatePicker.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PurchaseDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PurchaseDatePicker.Location = new System.Drawing.Point(600, 33);
            this.PurchaseDatePicker.Name = "PurchaseDatePicker";
            this.PurchaseDatePicker.Size = new System.Drawing.Size(168, 23);
            this.PurchaseDatePicker.TabIndex = 9;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(595, 62);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(95, 16);
            this.Label7.TabIndex = 32;
            this.Label7.Text = "Replace Year:";
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(595, 14);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(100, 16);
            this.Label6.TabIndex = 30;
            this.Label6.Text = "Purchase Date:";
            // 
            // ReplaceYearTextBox
            // 
            this.ReplaceYearTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReplaceYearTextBox.Location = new System.Drawing.Point(598, 81);
            this.ReplaceYearTextBox.Name = "ReplaceYearTextBox";
            this.ReplaceYearTextBox.Size = new System.Drawing.Size(169, 23);
            this.ReplaceYearTextBox.TabIndex = 10;
            // 
            // RightClickMenu
            // 
            this.RightClickMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.DeleteEntryToolStripMenuItem});
            this.RightClickMenu.Name = "RightClickMenu";
            this.RightClickMenu.Size = new System.Drawing.Size(138, 26);
            // 
            // DeleteEntryToolStripMenuItem
            // 
            this.DeleteEntryToolStripMenuItem.Image = global::AssetManager.Properties.Resources.DeleteIcon;
            this.DeleteEntryToolStripMenuItem.Name = "DeleteEntryToolStripMenuItem";
            this.DeleteEntryToolStripMenuItem.Size = new System.Drawing.Size(137, 22);
            this.DeleteEntryToolStripMenuItem.Text = "Delete Entry";
            this.DeleteEntryToolStripMenuItem.Click += new System.EventHandler(this.DeleteEntryToolStripMenuItem_Click);
            // 
            // TabControl1
            // 
            this.TabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.TabControl1.Controls.Add(this.HistoryTab);
            this.TabControl1.Controls.Add(this.TrackingTab);
            this.TabControl1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TabControl1.ItemSize = new System.Drawing.Size(69, 21);
            this.TabControl1.Location = new System.Drawing.Point(11, 301);
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            this.TabControl1.Size = new System.Drawing.Size(1268, 265);
            this.TabControl1.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.TabControl1.TabIndex = 40;
            this.TabControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.TabControl1_MouseDown);
            // 
            // HistoryTab
            // 
            this.HistoryTab.Controls.Add(this.DataGridHistory);
            this.HistoryTab.Location = new System.Drawing.Point(4, 25);
            this.HistoryTab.Name = "HistoryTab";
            this.HistoryTab.Padding = new System.Windows.Forms.Padding(3);
            this.HistoryTab.Size = new System.Drawing.Size(1260, 236);
            this.HistoryTab.TabIndex = 0;
            this.HistoryTab.Text = "History";
            this.HistoryTab.UseVisualStyleBackColor = true;
            // 
            // DataGridHistory
            // 
            this.DataGridHistory.AllowUserToAddRows = false;
            this.DataGridHistory.AllowUserToDeleteRows = false;
            this.DataGridHistory.AllowUserToResizeColumns = false;
            this.DataGridHistory.AllowUserToResizeRows = false;
            this.DataGridHistory.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.DataGridHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.DataGridHistory.ContextMenuStrip = this.RightClickMenu;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle1.Padding = new System.Windows.Forms.Padding(10);
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.DataGridHistory.DefaultCellStyle = dataGridViewCellStyle1;
            this.DataGridHistory.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataGridHistory.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.DataGridHistory.Location = new System.Drawing.Point(3, 3);
            this.DataGridHistory.Name = "DataGridHistory";
            this.DataGridHistory.ReadOnly = true;
            this.DataGridHistory.RowHeadersVisible = false;
            this.DataGridHistory.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.DataGridHistory.ShowCellToolTips = false;
            this.DataGridHistory.ShowEditingIcon = false;
            this.DataGridHistory.Size = new System.Drawing.Size(1254, 230);
            this.DataGridHistory.TabIndex = 40;
            this.DataGridHistory.TabStop = false;
            this.DataGridHistory.VirtualMode = true;
            this.DataGridHistory.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridHistory_CellDoubleClick);
            this.DataGridHistory.CellEnter += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridHistory_CellEnter);
            this.DataGridHistory.CellLeave += new System.Windows.Forms.DataGridViewCellEventHandler(this.DataGridHistory_CellLeave);
            this.DataGridHistory.CellMouseDown += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.DataGridHistory_CellMouseDown);
            // 
            // TrackingTab
            // 
            this.TrackingTab.Controls.Add(this.TrackingGrid);
            this.TrackingTab.Location = new System.Drawing.Point(4, 25);
            this.TrackingTab.Name = "TrackingTab";
            this.TrackingTab.Padding = new System.Windows.Forms.Padding(3);
            this.TrackingTab.Size = new System.Drawing.Size(1260, 236);
            this.TrackingTab.TabIndex = 1;
            this.TrackingTab.Text = "Tracking";
            this.TrackingTab.UseVisualStyleBackColor = true;
            // 
            // TrackingGrid
            // 
            this.TrackingGrid.AllowUserToAddRows = false;
            this.TrackingGrid.AllowUserToDeleteRows = false;
            this.TrackingGrid.AllowUserToResizeColumns = false;
            this.TrackingGrid.AllowUserToResizeRows = false;
            this.TrackingGrid.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.TrackingGrid.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.TrackingGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.TrackingGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.TrackingGrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.TrackingGrid.Location = new System.Drawing.Point(3, 3);
            this.TrackingGrid.MultiSelect = false;
            this.TrackingGrid.Name = "TrackingGrid";
            this.TrackingGrid.ReadOnly = true;
            this.TrackingGrid.RowHeadersVisible = false;
            this.TrackingGrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.TrackingGrid.ShowCellToolTips = false;
            this.TrackingGrid.ShowEditingIcon = false;
            this.TrackingGrid.Size = new System.Drawing.Size(1254, 230);
            this.TrackingGrid.TabIndex = 41;
            this.TrackingGrid.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.TrackingGrid_CellDoubleClick);
            this.TrackingGrid.RowPrePaint += new System.Windows.Forms.DataGridViewRowPrePaintEventHandler(this.TrackingGrid_RowPrePaint);
            // 
            // TrackingBox
            // 
            this.TrackingBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.TrackingBox.BackColor = System.Drawing.SystemColors.Control;
            this.TrackingBox.Controls.Add(this.Panel3);
            this.TrackingBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackingBox.Location = new System.Drawing.Point(3, 116);
            this.TrackingBox.Name = "TrackingBox";
            this.TrackingBox.Size = new System.Drawing.Size(421, 165);
            this.TrackingBox.TabIndex = 41;
            this.TrackingBox.TabStop = false;
            this.TrackingBox.Text = "Tracking Info";
            this.TrackingBox.Visible = false;
            // 
            // Panel3
            // 
            this.Panel3.AutoScroll = true;
            this.Panel3.Controls.Add(this.TrackingLocationTextBox);
            this.Panel3.Controls.Add(this.Label16);
            this.Panel3.Controls.Add(this.DueBackTextBox);
            this.Panel3.Controls.Add(this.DueBackLabel);
            this.Panel3.Controls.Add(this.CheckUserTextBox);
            this.Panel3.Controls.Add(this.CheckUserLabel);
            this.Panel3.Controls.Add(this.CheckTimeTextBox);
            this.Panel3.Controls.Add(this.CheckTimeLabel);
            this.Panel3.Controls.Add(this.TrackingStatusTextBox);
            this.Panel3.Controls.Add(this.Label11);
            this.Panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel3.Location = new System.Drawing.Point(3, 18);
            this.Panel3.Name = "Panel3";
            this.Panel3.Padding = new System.Windows.Forms.Padding(0, 0, 5, 0);
            this.Panel3.Size = new System.Drawing.Size(415, 144);
            this.Panel3.TabIndex = 58;
            // 
            // TrackingLocationTextBox
            // 
            this.TrackingLocationTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.TrackingLocationTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.TrackingLocationTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackingLocationTextBox.Location = new System.Drawing.Point(224, 19);
            this.TrackingLocationTextBox.Name = "TrackingLocationTextBox";
            this.TrackingLocationTextBox.ReadOnly = true;
            this.TrackingLocationTextBox.Size = new System.Drawing.Size(134, 22);
            this.TrackingLocationTextBox.TabIndex = 57;
            this.TrackingLocationTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label16
            // 
            this.Label16.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Label16.AutoSize = true;
            this.Label16.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label16.Location = new System.Drawing.Point(221, 0);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(62, 16);
            this.Label16.TabIndex = 56;
            this.Label16.Text = "Location:";
            // 
            // DueBackTextBox
            // 
            this.DueBackTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.DueBackTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.DueBackTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DueBackTextBox.Location = new System.Drawing.Point(142, 108);
            this.DueBackTextBox.Name = "DueBackTextBox";
            this.DueBackTextBox.ReadOnly = true;
            this.DueBackTextBox.Size = new System.Drawing.Size(134, 22);
            this.DueBackTextBox.TabIndex = 55;
            this.DueBackTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // DueBackLabel
            // 
            this.DueBackLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.DueBackLabel.AutoSize = true;
            this.DueBackLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DueBackLabel.Location = new System.Drawing.Point(139, 89);
            this.DueBackLabel.Name = "DueBackLabel";
            this.DueBackLabel.Size = new System.Drawing.Size(70, 16);
            this.DueBackLabel.TabIndex = 54;
            this.DueBackLabel.Text = "Due Back:";
            // 
            // CheckUserTextBox
            // 
            this.CheckUserTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CheckUserTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CheckUserTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckUserTextBox.Location = new System.Drawing.Point(224, 65);
            this.CheckUserTextBox.Name = "CheckUserTextBox";
            this.CheckUserTextBox.ReadOnly = true;
            this.CheckUserTextBox.Size = new System.Drawing.Size(134, 22);
            this.CheckUserTextBox.TabIndex = 53;
            this.CheckUserTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CheckUserLabel
            // 
            this.CheckUserLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CheckUserLabel.AutoSize = true;
            this.CheckUserLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckUserLabel.Location = new System.Drawing.Point(221, 46);
            this.CheckUserLabel.Name = "CheckUserLabel";
            this.CheckUserLabel.Size = new System.Drawing.Size(101, 16);
            this.CheckUserLabel.TabIndex = 52;
            this.CheckUserLabel.Text = "CheckOut User:";
            // 
            // CheckTimeTextBox
            // 
            this.CheckTimeTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CheckTimeTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.CheckTimeTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckTimeTextBox.Location = new System.Drawing.Point(74, 65);
            this.CheckTimeTextBox.Name = "CheckTimeTextBox";
            this.CheckTimeTextBox.ReadOnly = true;
            this.CheckTimeTextBox.Size = new System.Drawing.Size(134, 22);
            this.CheckTimeTextBox.TabIndex = 51;
            this.CheckTimeTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // CheckTimeLabel
            // 
            this.CheckTimeLabel.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.CheckTimeLabel.AutoSize = true;
            this.CheckTimeLabel.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckTimeLabel.Location = new System.Drawing.Point(71, 46);
            this.CheckTimeLabel.Name = "CheckTimeLabel";
            this.CheckTimeLabel.Size = new System.Drawing.Size(103, 16);
            this.CheckTimeLabel.TabIndex = 50;
            this.CheckTimeLabel.Text = "CheckOut Time:";
            // 
            // TrackingStatusTextBox
            // 
            this.TrackingStatusTextBox.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.TrackingStatusTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.TrackingStatusTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackingStatusTextBox.Location = new System.Drawing.Point(74, 19);
            this.TrackingStatusTextBox.Name = "TrackingStatusTextBox";
            this.TrackingStatusTextBox.ReadOnly = true;
            this.TrackingStatusTextBox.Size = new System.Drawing.Size(134, 22);
            this.TrackingStatusTextBox.TabIndex = 49;
            this.TrackingStatusTextBox.Text = "STATUS";
            this.TrackingStatusTextBox.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // Label11
            // 
            this.Label11.Anchor = System.Windows.Forms.AnchorStyles.Top;
            this.Label11.AutoSize = true;
            this.Label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label11.Location = new System.Drawing.Point(71, 0);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(48, 16);
            this.Label11.TabIndex = 48;
            this.Label11.Text = "Status:";
            // 
            // ToolTip1
            // 
            this.ToolTip1.AutoPopDelay = 5000;
            this.ToolTip1.InitialDelay = 100;
            this.ToolTip1.ReshowDelay = 100;
            // 
            // StatusStrip1
            // 
            this.StatusStrip1.AutoSize = false;
            this.StatusStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.StatusStrip1.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.StatusLabel});
            this.StatusStrip1.Location = new System.Drawing.Point(0, 680);
            this.StatusStrip1.Name = "StatusStrip1";
            this.StatusStrip1.Size = new System.Drawing.Size(1291, 22);
            this.StatusStrip1.Stretch = false;
            this.StatusStrip1.TabIndex = 45;
            this.StatusStrip1.Text = "StatusStrip1";
            // 
            // StatusLabel
            // 
            this.StatusLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // AcceptCancelToolStrip
            // 
            this.AcceptCancelToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(88)))), ((int)(((byte)(237)))), ((int)(((byte)(118)))));
            this.AcceptCancelToolStrip.CanOverflow = false;
            this.AcceptCancelToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.AcceptCancelToolStrip.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AcceptCancelToolStrip.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.AcceptCancelToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AcceptToolButton,
            this.ToolStripSeparator3,
            this.CancelToolButton,
            this.ToolStripSeparator2});
            this.AcceptCancelToolStrip.Location = new System.Drawing.Point(3, 0);
            this.AcceptCancelToolStrip.Name = "AcceptCancelToolStrip";
            this.AcceptCancelToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.AcceptCancelToolStrip.Size = new System.Drawing.Size(312, 37);
            this.AcceptCancelToolStrip.TabIndex = 44;
            this.AcceptCancelToolStrip.Text = "ToolStrip1";
            // 
            // AcceptToolButton
            // 
            this.AcceptToolButton.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AcceptToolButton.Image = global::AssetManager.Properties.Resources.CheckedBoxIcon;
            this.AcceptToolButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AcceptToolButton.Name = "AcceptToolButton";
            this.AcceptToolButton.Padding = new System.Windows.Forms.Padding(50, 5, 5, 0);
            this.AcceptToolButton.Size = new System.Drawing.Size(146, 34);
            this.AcceptToolButton.Text = "Accept";
            this.AcceptToolButton.Click += new System.EventHandler(this.AcceptToolButton_Click);
            // 
            // ToolStripSeparator3
            // 
            this.ToolStripSeparator3.Name = "ToolStripSeparator3";
            this.ToolStripSeparator3.Size = new System.Drawing.Size(6, 37);
            // 
            // CancelToolButton
            // 
            this.CancelToolButton.Font = new System.Drawing.Font("Segoe UI Semibold", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelToolButton.Image = global::AssetManager.Properties.Resources.CloseCancelDeleteIcon;
            this.CancelToolButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.CancelToolButton.Name = "CancelToolButton";
            this.CancelToolButton.Padding = new System.Windows.Forms.Padding(50, 5, 5, 0);
            this.CancelToolButton.Size = new System.Drawing.Size(142, 34);
            this.CancelToolButton.Text = "Cancel";
            this.CancelToolButton.Click += new System.EventHandler(this.CancelToolButton_Click);
            // 
            // ToolStripSeparator2
            // 
            this.ToolStripSeparator2.Name = "ToolStripSeparator2";
            this.ToolStripSeparator2.Size = new System.Drawing.Size(6, 37);
            // 
            // FieldsPanel
            // 
            this.FieldsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.FieldsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.FieldsPanel.Controls.Add(this.InfoDataSplitter);
            this.FieldsPanel.Location = new System.Drawing.Point(11, 8);
            this.FieldsPanel.Name = "FieldsPanel";
            this.FieldsPanel.Size = new System.Drawing.Size(1268, 288);
            this.FieldsPanel.TabIndex = 53;
            // 
            // InfoDataSplitter
            // 
            this.InfoDataSplitter.BackColor = System.Drawing.SystemColors.Control;
            this.InfoDataSplitter.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.InfoDataSplitter.Dock = System.Windows.Forms.DockStyle.Fill;
            this.InfoDataSplitter.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.InfoDataSplitter.Location = new System.Drawing.Point(0, 0);
            this.InfoDataSplitter.Name = "InfoDataSplitter";
            // 
            // InfoDataSplitter.Panel1
            // 
            this.InfoDataSplitter.Panel1.Controls.Add(this.FieldTabs);
            // 
            // InfoDataSplitter.Panel2
            // 
            this.InfoDataSplitter.Panel2.Controls.Add(this.RemoteTrackingPanel);
            this.InfoDataSplitter.Panel2MinSize = 327;
            this.InfoDataSplitter.Size = new System.Drawing.Size(1268, 288);
            this.InfoDataSplitter.SplitterDistance = 833;
            this.InfoDataSplitter.TabIndex = 55;
            // 
            // FieldTabs
            // 
            this.FieldTabs.Controls.Add(this.AssetInfoTab);
            this.FieldTabs.Controls.Add(this.MiscInfoTab);
            this.FieldTabs.Dock = System.Windows.Forms.DockStyle.Fill;
            this.FieldTabs.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FieldTabs.HotTrack = true;
            this.FieldTabs.Location = new System.Drawing.Point(0, 0);
            this.FieldTabs.Multiline = true;
            this.FieldTabs.Name = "FieldTabs";
            this.FieldTabs.SelectedIndex = 0;
            this.FieldTabs.Size = new System.Drawing.Size(829, 284);
            this.FieldTabs.SizeMode = System.Windows.Forms.TabSizeMode.Fixed;
            this.FieldTabs.TabIndex = 53;
            // 
            // AssetInfoTab
            // 
            this.AssetInfoTab.AutoScroll = true;
            this.AssetInfoTab.BackColor = System.Drawing.SystemColors.Control;
            this.AssetInfoTab.Controls.Add(this.MunisSibiPanel);
            this.AssetInfoTab.Controls.Add(this.Label2);
            this.AssetInfoTab.Controls.Add(this.Label3);
            this.AssetInfoTab.Controls.Add(this.CurrentUserTextBox);
            this.AssetInfoTab.Controls.Add(this.SerialTextBox);
            this.AssetInfoTab.Controls.Add(this.AssetTagTextBox);
            this.AssetInfoTab.Controls.Add(this.MunisSearchButton);
            this.AssetInfoTab.Controls.Add(this.Label12);
            this.AssetInfoTab.Controls.Add(this.Label1);
            this.AssetInfoTab.Controls.Add(this.PONumberTextBox);
            this.AssetInfoTab.Controls.Add(this.Label4);
            this.AssetInfoTab.Controls.Add(this.EquipTypeComboBox);
            this.AssetInfoTab.Controls.Add(this.Label5);
            this.AssetInfoTab.Controls.Add(this.PurchaseDatePicker);
            this.AssetInfoTab.Controls.Add(this.Label9);
            this.AssetInfoTab.Controls.Add(this.Label7);
            this.AssetInfoTab.Controls.Add(this.Label13);
            this.AssetInfoTab.Controls.Add(this.Label6);
            this.AssetInfoTab.Controls.Add(this.ReplaceYearTextBox);
            this.AssetInfoTab.Controls.Add(this.StatusComboBox);
            this.AssetInfoTab.Controls.Add(this.LocationComboBox);
            this.AssetInfoTab.Controls.Add(this.Label8);
            this.AssetInfoTab.Controls.Add(this.OSVersionComboBox);
            this.AssetInfoTab.Controls.Add(this.DescriptionTextBox);
            this.AssetInfoTab.Location = new System.Drawing.Point(4, 23);
            this.AssetInfoTab.Name = "AssetInfoTab";
            this.AssetInfoTab.Padding = new System.Windows.Forms.Padding(3);
            this.AssetInfoTab.Size = new System.Drawing.Size(821, 257);
            this.AssetInfoTab.TabIndex = 0;
            this.AssetInfoTab.Text = "Asset Info.";
            // 
            // MiscInfoTab
            // 
            this.MiscInfoTab.AutoScroll = true;
            this.MiscInfoTab.BackColor = System.Drawing.SystemColors.Control;
            this.MiscInfoTab.Controls.Add(this.PingHistLabel);
            this.MiscInfoTab.Controls.Add(this.ActiveDirectoryBox);
            this.MiscInfoTab.Controls.Add(this.iCloudTextBox);
            this.MiscInfoTab.Controls.Add(this.Label17);
            this.MiscInfoTab.Controls.Add(this.GuidLabel);
            this.MiscInfoTab.Controls.Add(this.PhoneNumberTextBox);
            this.MiscInfoTab.Controls.Add(this.Label10);
            this.MiscInfoTab.Controls.Add(this.Label14);
            this.MiscInfoTab.Controls.Add(this.TrackableCheckBox);
            this.MiscInfoTab.Controls.Add(this.HostnameTextBox);
            this.MiscInfoTab.Controls.Add(this.Label15);
            this.MiscInfoTab.Cursor = System.Windows.Forms.Cursors.Default;
            this.MiscInfoTab.Location = new System.Drawing.Point(4, 23);
            this.MiscInfoTab.Name = "MiscInfoTab";
            this.MiscInfoTab.Padding = new System.Windows.Forms.Padding(3);
            this.MiscInfoTab.Size = new System.Drawing.Size(821, 257);
            this.MiscInfoTab.TabIndex = 1;
            this.MiscInfoTab.Text = "Misc.";
            // 
            // PingHistLabel
            // 
            this.PingHistLabel.AutoSize = true;
            this.PingHistLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.PingHistLabel.Font = new System.Drawing.Font("Consolas", 8F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PingHistLabel.ForeColor = System.Drawing.Color.Blue;
            this.PingHistLabel.Location = new System.Drawing.Point(102, 25);
            this.PingHistLabel.Name = "PingHistLabel";
            this.PingHistLabel.Size = new System.Drawing.Size(91, 13);
            this.PingHistLabel.TabIndex = 62;
            this.PingHistLabel.Text = "(Ping History)";
            this.PingHistLabel.Visible = false;
            this.PingHistLabel.Click += new System.EventHandler(this.PingHistLabel_Click);
            // 
            // ActiveDirectoryBox
            // 
            this.ActiveDirectoryBox.Controls.Add(this.Label22);
            this.ActiveDirectoryBox.Controls.Add(this.ADCreatedTextBox);
            this.ActiveDirectoryBox.Controls.Add(this.Label21);
            this.ActiveDirectoryBox.Controls.Add(this.ADLastLoginTextBox);
            this.ActiveDirectoryBox.Controls.Add(this.Label20);
            this.ActiveDirectoryBox.Controls.Add(this.ADOSVerTextBox);
            this.ActiveDirectoryBox.Controls.Add(this.Label19);
            this.ActiveDirectoryBox.Controls.Add(this.ADOSTextBox);
            this.ActiveDirectoryBox.Controls.Add(this.Label18);
            this.ActiveDirectoryBox.Controls.Add(this.ADOUTextBox);
            this.ActiveDirectoryBox.Location = new System.Drawing.Point(488, 15);
            this.ActiveDirectoryBox.Name = "ActiveDirectoryBox";
            this.ActiveDirectoryBox.Size = new System.Drawing.Size(305, 226);
            this.ActiveDirectoryBox.TabIndex = 0;
            this.ActiveDirectoryBox.TabStop = false;
            this.ActiveDirectoryBox.Text = "Active Directory Info:";
            this.ActiveDirectoryBox.Visible = false;
            // 
            // Label22
            // 
            this.Label22.AutoSize = true;
            this.Label22.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label22.Location = new System.Drawing.Point(24, 163);
            this.Label22.Name = "Label22";
            this.Label22.Size = new System.Drawing.Size(59, 16);
            this.Label22.TabIndex = 72;
            this.Label22.Text = "Created:";
            // 
            // ADCreatedTextBox
            // 
            this.ADCreatedTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ADCreatedTextBox.Location = new System.Drawing.Point(27, 182);
            this.ADCreatedTextBox.Name = "ADCreatedTextBox";
            this.ADCreatedTextBox.Size = new System.Drawing.Size(244, 23);
            this.ADCreatedTextBox.TabIndex = 71;
            // 
            // Label21
            // 
            this.Label21.AutoSize = true;
            this.Label21.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label21.Location = new System.Drawing.Point(24, 118);
            this.Label21.Name = "Label21";
            this.Label21.Size = new System.Drawing.Size(72, 16);
            this.Label21.TabIndex = 70;
            this.Label21.Text = "Last Login:";
            // 
            // ADLastLoginTextBox
            // 
            this.ADLastLoginTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ADLastLoginTextBox.Location = new System.Drawing.Point(27, 137);
            this.ADLastLoginTextBox.Name = "ADLastLoginTextBox";
            this.ADLastLoginTextBox.Size = new System.Drawing.Size(244, 23);
            this.ADLastLoginTextBox.TabIndex = 69;
            // 
            // Label20
            // 
            this.Label20.AutoSize = true;
            this.Label20.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label20.Location = new System.Drawing.Point(149, 73);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(79, 16);
            this.Label20.TabIndex = 68;
            this.Label20.Text = "OS Version:";
            // 
            // ADOSVerTextBox
            // 
            this.ADOSVerTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ADOSVerTextBox.Location = new System.Drawing.Point(152, 92);
            this.ADOSVerTextBox.Name = "ADOSVerTextBox";
            this.ADOSVerTextBox.Size = new System.Drawing.Size(119, 23);
            this.ADOSVerTextBox.TabIndex = 67;
            // 
            // Label19
            // 
            this.Label19.AutoSize = true;
            this.Label19.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label19.Location = new System.Drawing.Point(24, 73);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(30, 16);
            this.Label19.TabIndex = 66;
            this.Label19.Text = "OS:";
            // 
            // ADOSTextBox
            // 
            this.ADOSTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ADOSTextBox.Location = new System.Drawing.Point(27, 92);
            this.ADOSTextBox.Name = "ADOSTextBox";
            this.ADOSTextBox.Size = new System.Drawing.Size(119, 23);
            this.ADOSTextBox.TabIndex = 65;
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label18.Location = new System.Drawing.Point(24, 28);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(61, 16);
            this.Label18.TabIndex = 64;
            this.Label18.Text = "OU Path:";
            // 
            // ADOUTextBox
            // 
            this.ADOUTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ADOUTextBox.Location = new System.Drawing.Point(27, 47);
            this.ADOUTextBox.Name = "ADOUTextBox";
            this.ADOUTextBox.Size = new System.Drawing.Size(244, 23);
            this.ADOUTextBox.TabIndex = 63;
            // 
            // iCloudTextBox
            // 
            this.iCloudTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iCloudTextBox.Location = new System.Drawing.Point(232, 42);
            this.iCloudTextBox.Name = "iCloudTextBox";
            this.iCloudTextBox.Size = new System.Drawing.Size(219, 23);
            this.iCloudTextBox.TabIndex = 60;
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label17.Location = new System.Drawing.Point(229, 23);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(100, 16);
            this.Label17.TabIndex = 61;
            this.Label17.Text = "iCloud Account:";
            // 
            // RemoteTrackingPanel
            // 
            this.RemoteTrackingPanel.Controls.Add(this.remoteToolsControl);
            this.RemoteTrackingPanel.Controls.Add(this.TrackingBox);
            this.RemoteTrackingPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RemoteTrackingPanel.Location = new System.Drawing.Point(0, 0);
            this.RemoteTrackingPanel.Name = "RemoteTrackingPanel";
            this.RemoteTrackingPanel.Size = new System.Drawing.Size(427, 284);
            this.RemoteTrackingPanel.TabIndex = 54;
            // 
            // remoteToolsControl
            // 
            this.remoteToolsControl.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.remoteToolsControl.Device = null;
            this.remoteToolsControl.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.remoteToolsControl.Location = new System.Drawing.Point(3, 7);
            this.remoteToolsControl.MaxFailedUntilNotify = 5;
            this.remoteToolsControl.Name = "remoteToolsControl";
            this.remoteToolsControl.Size = new System.Drawing.Size(422, 103);
            this.remoteToolsControl.TabIndex = 42;
            this.remoteToolsControl.Visible = false;
            this.remoteToolsControl.HostBackOnline += new System.EventHandler(this.remoteToolsControl_HostBackOnline);
            this.remoteToolsControl.NewStatusPrompt += new AssetManager.UserInterface.CustomControls.RemoteToolsControl.UserPromptEventHandler(this.remoteToolsControl_NewStatusPrompt);
            this.remoteToolsControl.VisibleChanging += new System.EventHandler<bool>(this.remoteToolsControl_VisibleChanging);
            this.remoteToolsControl.HostOnlineStatus += new System.EventHandler<bool>(this.remoteToolsControl_HostOnlineStatus);
            // 
            // ToolStripContainer1
            // 
            this.ToolStripContainer1.BottomToolStripPanelVisible = false;
            // 
            // ToolStripContainer1.ContentPanel
            // 
            this.ToolStripContainer1.ContentPanel.Controls.Add(this.TabControl1);
            this.ToolStripContainer1.ContentPanel.Controls.Add(this.FieldsPanel);
            this.ToolStripContainer1.ContentPanel.Size = new System.Drawing.Size(1291, 569);
            this.ToolStripContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolStripContainer1.LeftToolStripPanelVisible = false;
            this.ToolStripContainer1.Location = new System.Drawing.Point(0, 0);
            this.ToolStripContainer1.Name = "ToolStripContainer1";
            this.ToolStripContainer1.RightToolStripPanelVisible = false;
            this.ToolStripContainer1.Size = new System.Drawing.Size(1291, 680);
            this.ToolStripContainer1.TabIndex = 54;
            this.ToolStripContainer1.Text = "ToolStripContainer1";
            // 
            // ToolStripContainer1.TopToolStripPanel
            // 
            this.ToolStripContainer1.TopToolStripPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ToolStripContainer1.TopToolStripPanel.Controls.Add(this.AcceptCancelToolStrip);
            this.ToolStripContainer1.TopToolStripPanel.Controls.Add(this.TrackingToolStrip);
            this.ToolStripContainer1.TopToolStripPanel.Controls.Add(this.ToolStrip1);
            // 
            // TrackingToolStrip
            // 
            this.TrackingToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.TrackingToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.TrackingToolStrip.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.TrackingToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.TrackingDropDown,
            this.ToolStripSeparator4});
            this.TrackingToolStrip.Location = new System.Drawing.Point(16, 37);
            this.TrackingToolStrip.Name = "TrackingToolStrip";
            this.TrackingToolStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.TrackingToolStrip.Size = new System.Drawing.Size(134, 37);
            this.TrackingToolStrip.TabIndex = 46;
            // 
            // TrackingDropDown
            // 
            this.TrackingDropDown.AutoSize = false;
            this.TrackingDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.CheckOutTool,
            this.CheckInTool});
            this.TrackingDropDown.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackingDropDown.Image = global::AssetManager.Properties.Resources.CheckOutIcon;
            this.TrackingDropDown.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.TrackingDropDown.Name = "TrackingDropDown";
            this.TrackingDropDown.Padding = new System.Windows.Forms.Padding(10, 0, 0, 0);
            this.TrackingDropDown.Size = new System.Drawing.Size(116, 34);
            this.TrackingDropDown.Text = "Tracking";
            // 
            // CheckOutTool
            // 
            this.CheckOutTool.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckOutTool.Image = global::AssetManager.Properties.Resources.CheckedBoxRedIcon;
            this.CheckOutTool.Name = "CheckOutTool";
            this.CheckOutTool.Size = new System.Drawing.Size(135, 22);
            this.CheckOutTool.Text = "Check Out";
            this.CheckOutTool.Click += new System.EventHandler(this.CheckOutTool_Click);
            // 
            // CheckInTool
            // 
            this.CheckInTool.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CheckInTool.Image = global::AssetManager.Properties.Resources.CheckedBoxGreenIcon;
            this.CheckInTool.Name = "CheckInTool";
            this.CheckInTool.Size = new System.Drawing.Size(135, 22);
            this.CheckInTool.Text = "Check In";
            this.CheckInTool.Click += new System.EventHandler(this.CheckInTool_Click);
            // 
            // ToolStripSeparator4
            // 
            this.ToolStripSeparator4.Name = "ToolStripSeparator4";
            this.ToolStripSeparator4.Size = new System.Drawing.Size(6, 37);
            // 
            // ToolStrip1
            // 
            this.ToolStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ToolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.ToolStrip1.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolStrip1.ImageScalingSize = new System.Drawing.Size(25, 25);
            this.ToolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ModifyToolButton,
            this.NewNoteToolButton,
            this.DeleteDeviceToolButton,
            this.RefreshToolButton,
            this.ToolStripSeparator1,
            this.AttachmentsToolButton,
            this.ToolStripSeparator7,
            this.AssetControlDropDown,
            this.ToolStripSeparator9});
            this.ToolStrip1.Location = new System.Drawing.Point(3, 74);
            this.ToolStrip1.Name = "ToolStrip1";
            this.ToolStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.ToolStrip1.Size = new System.Drawing.Size(404, 37);
            this.ToolStrip1.TabIndex = 45;
            this.ToolStrip1.Text = "MyToolStrip1";
            // 
            // ModifyToolButton
            // 
            this.ModifyToolButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ModifyToolButton.Image = global::AssetManager.Properties.Resources.EditIcon;
            this.ModifyToolButton.Name = "ModifyToolButton";
            this.ModifyToolButton.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.ModifyToolButton.Size = new System.Drawing.Size(39, 34);
            this.ModifyToolButton.Text = "Modify";
            this.ModifyToolButton.Click += new System.EventHandler(this.ModifyToolButton_Click);
            // 
            // NewNoteToolButton
            // 
            this.NewNoteToolButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.NewNoteToolButton.Image = global::AssetManager.Properties.Resources.AddNoteIcon;
            this.NewNoteToolButton.Name = "NewNoteToolButton";
            this.NewNoteToolButton.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.NewNoteToolButton.Size = new System.Drawing.Size(39, 34);
            this.NewNoteToolButton.Text = "Add Note";
            this.NewNoteToolButton.Click += new System.EventHandler(this.NewNoteToolButton_Click);
            // 
            // DeleteDeviceToolButton
            // 
            this.DeleteDeviceToolButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DeleteDeviceToolButton.Image = global::AssetManager.Properties.Resources.DeleteRedIcon;
            this.DeleteDeviceToolButton.Name = "DeleteDeviceToolButton";
            this.DeleteDeviceToolButton.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.DeleteDeviceToolButton.Size = new System.Drawing.Size(39, 34);
            this.DeleteDeviceToolButton.Text = "Delete Device";
            this.DeleteDeviceToolButton.Click += new System.EventHandler(this.DeleteDeviceToolButton_Click);
            // 
            // RefreshToolButton
            // 
            this.RefreshToolButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RefreshToolButton.Image = global::AssetManager.Properties.Resources.RefreshIcon;
            this.RefreshToolButton.Name = "RefreshToolButton";
            this.RefreshToolButton.Size = new System.Drawing.Size(29, 34);
            this.RefreshToolButton.Text = "ToolStripButton1";
            this.RefreshToolButton.ToolTipText = "Refresh";
            this.RefreshToolButton.Click += new System.EventHandler(this.RefreshToolButton_Click);
            // 
            // ToolStripSeparator1
            // 
            this.ToolStripSeparator1.Name = "ToolStripSeparator1";
            this.ToolStripSeparator1.Size = new System.Drawing.Size(6, 37);
            // 
            // AttachmentsToolButton
            // 
            this.AttachmentsToolButton.Image = global::AssetManager.Properties.Resources.PaperClipIcon;
            this.AttachmentsToolButton.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.AttachmentsToolButton.Name = "AttachmentsToolButton";
            this.AttachmentsToolButton.Padding = new System.Windows.Forms.Padding(5, 5, 5, 0);
            this.AttachmentsToolButton.Size = new System.Drawing.Size(39, 34);
            this.AttachmentsToolButton.Click += new System.EventHandler(this.AttachmentsToolButton_Click);
            // 
            // ToolStripSeparator7
            // 
            this.ToolStripSeparator7.Name = "ToolStripSeparator7";
            this.ToolStripSeparator7.Size = new System.Drawing.Size(6, 37);
            // 
            // AssetControlDropDown
            // 
            this.AssetControlDropDown.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.AssetInputFormToolItem,
            this.AssetTransferFormToolItem,
            this.AssetDisposalFormToolItem});
            this.AssetControlDropDown.Image = global::AssetManager.Properties.Resources.FormIcon;
            this.AssetControlDropDown.Name = "AssetControlDropDown";
            this.AssetControlDropDown.Size = new System.Drawing.Size(189, 34);
            this.AssetControlDropDown.Text = "Asset Control Forms";
            // 
            // AssetInputFormToolItem
            // 
            this.AssetInputFormToolItem.Image = global::AssetManager.Properties.Resources.ImportIcon;
            this.AssetInputFormToolItem.Name = "AssetInputFormToolItem";
            this.AssetInputFormToolItem.Size = new System.Drawing.Size(230, 32);
            this.AssetInputFormToolItem.Text = "Asset Input Form";
            this.AssetInputFormToolItem.Click += new System.EventHandler(this.AssetInputFormToolItem_Click);
            // 
            // AssetTransferFormToolItem
            // 
            this.AssetTransferFormToolItem.Image = global::AssetManager.Properties.Resources.TransferArrowsIcon;
            this.AssetTransferFormToolItem.Name = "AssetTransferFormToolItem";
            this.AssetTransferFormToolItem.Size = new System.Drawing.Size(230, 32);
            this.AssetTransferFormToolItem.Text = "Asset Transfer Form";
            this.AssetTransferFormToolItem.Click += new System.EventHandler(this.AssetTransferFormToolItem_Click);
            // 
            // AssetDisposalFormToolItem
            // 
            this.AssetDisposalFormToolItem.Image = global::AssetManager.Properties.Resources.TrashIcon;
            this.AssetDisposalFormToolItem.Name = "AssetDisposalFormToolItem";
            this.AssetDisposalFormToolItem.Size = new System.Drawing.Size(230, 32);
            this.AssetDisposalFormToolItem.Text = "Asset Disposal Form";
            this.AssetDisposalFormToolItem.Click += new System.EventHandler(this.AssetDisposalFormToolItem_Click);
            // 
            // ToolStripSeparator9
            // 
            this.ToolStripSeparator9.Name = "ToolStripSeparator9";
            this.ToolStripSeparator9.Size = new System.Drawing.Size(6, 37);
            // 
            // ViewDeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ClientSize = new System.Drawing.Size(1291, 702);
            this.Controls.Add(this.ToolStripContainer1);
            this.Controls.Add(this.StatusStrip1);
            this.DoubleBuffered = true;
            this.MinimumSize = new System.Drawing.Size(1161, 559);
            this.Name = "ViewDeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View";
            this.Resize += new System.EventHandler(this.ViewDeviceForm_Resize);
            this.MunisSibiPanel.ResumeLayout(false);
            this.RightClickMenu.ResumeLayout(false);
            this.TabControl1.ResumeLayout(false);
            this.HistoryTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.DataGridHistory)).EndInit();
            this.TrackingTab.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.TrackingGrid)).EndInit();
            this.TrackingBox.ResumeLayout(false);
            this.Panel3.ResumeLayout(false);
            this.Panel3.PerformLayout();
            this.StatusStrip1.ResumeLayout(false);
            this.StatusStrip1.PerformLayout();
            this.AcceptCancelToolStrip.ResumeLayout(false);
            this.AcceptCancelToolStrip.PerformLayout();
            this.FieldsPanel.ResumeLayout(false);
            this.InfoDataSplitter.Panel1.ResumeLayout(false);
            this.InfoDataSplitter.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.InfoDataSplitter)).EndInit();
            this.InfoDataSplitter.ResumeLayout(false);
            this.FieldTabs.ResumeLayout(false);
            this.AssetInfoTab.ResumeLayout(false);
            this.AssetInfoTab.PerformLayout();
            this.MiscInfoTab.ResumeLayout(false);
            this.MiscInfoTab.PerformLayout();
            this.ActiveDirectoryBox.ResumeLayout(false);
            this.ActiveDirectoryBox.PerformLayout();
            this.RemoteTrackingPanel.ResumeLayout(false);
            this.ToolStripContainer1.ContentPanel.ResumeLayout(false);
            this.ToolStripContainer1.TopToolStripPanel.ResumeLayout(false);
            this.ToolStripContainer1.TopToolStripPanel.PerformLayout();
            this.ToolStripContainer1.ResumeLayout(false);
            this.ToolStripContainer1.PerformLayout();
            this.TrackingToolStrip.ResumeLayout(false);
            this.TrackingToolStrip.PerformLayout();
            this.ToolStrip1.ResumeLayout(false);
            this.ToolStrip1.PerformLayout();
            this.ResumeLayout(false);

        }
        internal Label Label9;
        internal ComboBox StatusComboBox;
        internal Label Label8;
        internal ComboBox OSVersionComboBox;
        internal Label Label13;
        internal ComboBox EquipTypeComboBox;
        internal Label Label7;
        internal TextBox ReplaceYearTextBox;
        internal Label Label6;
        internal DateTimePicker PurchaseDatePicker;
        internal Label Label5;
        internal ComboBox LocationComboBox;
        internal Label Label4;
        internal TextBox DescriptionTextBox;
        internal Label Label3;
        internal TextBox CurrentUserTextBox;
        internal Label Label2;
        internal TextBox SerialTextBox;
        internal Label Label1;
        internal TextBox AssetTagTextBox;
        internal Label Label10;
        internal ContextMenuStrip RightClickMenu;
        internal ToolStripMenuItem DeleteEntryToolStripMenuItem;
        internal CheckBox TrackableCheckBox;
        internal TabControl TabControl1;
        internal TabPage HistoryTab;
        internal DataGridView DataGridHistory;
        internal TabPage TrackingTab;
        internal DataGridView TrackingGrid;
        internal GroupBox TrackingBox;
        internal TextBox TrackingStatusTextBox;
        internal Label Label11;
        internal TextBox CheckTimeTextBox;
        internal Label CheckTimeLabel;
        internal TextBox CheckUserTextBox;
        internal Label CheckUserLabel;
        internal TextBox TrackingLocationTextBox;
        internal Label Label16;
        internal TextBox DueBackTextBox;
        internal Label DueBackLabel;
        internal ToolTip ToolTip1;
        internal OneClickToolStrip AcceptCancelToolStrip;
        internal ToolStripSeparator ToolStripSeparator3;
        internal StatusStrip StatusStrip1;
        internal ToolStripButton CancelToolButton;
        internal ToolStripButton AcceptToolButton;
        internal Button MunisInfoButton;
        internal Label Label12;
        internal TextBox PONumberTextBox;
        internal Button SibiViewButton;
        internal Panel MunisSibiPanel;
        internal Button MunisSearchButton;
        internal Label GuidLabel;
        internal Label Label14;
        internal MaskedTextBox PhoneNumberTextBox;
        internal Panel FieldsPanel;
        internal ToolStripContainer ToolStripContainer1;
        internal OneClickToolStrip ToolStrip1;
        internal ToolStripButton ModifyToolButton;
        internal ToolStripButton NewNoteToolButton;
        internal ToolStripButton DeleteDeviceToolButton;
        internal ToolStripButton AttachmentsToolButton;
        internal ToolStripSeparator ToolStripSeparator7;
        internal ToolStripDropDownButton AssetControlDropDown;
        internal ToolStripMenuItem AssetInputFormToolItem;
        internal ToolStripMenuItem AssetTransferFormToolItem;
        internal ToolStripMenuItem AssetDisposalFormToolItem;
        internal ToolStripSeparator ToolStripSeparator9;
        internal ToolStripSeparator ToolStripSeparator1;
        internal ToolStrip TrackingToolStrip;
        internal ToolStripDropDownButton TrackingDropDown;
        internal ToolStripMenuItem CheckOutTool;
        internal ToolStripMenuItem CheckInTool;
        internal ToolStripSeparator ToolStripSeparator2;
        internal ToolStripSeparator ToolStripSeparator4;
        internal TextBox HostnameTextBox;
        internal Label Label15;
        internal TabControl FieldTabs;
        internal TabPage AssetInfoTab;
        internal TabPage MiscInfoTab;
        internal Panel RemoteTrackingPanel;
        internal TextBox iCloudTextBox;
        internal Label Label17;
        internal TextBox ADOUTextBox;
        internal Label Label18;
        internal GroupBox ActiveDirectoryBox;
        internal Label Label22;
        internal TextBox ADCreatedTextBox;
        internal Label Label21;
        internal TextBox ADLastLoginTextBox;
        internal Label Label20;
        internal TextBox ADOSVerTextBox;
        internal Label Label19;
        internal TextBox ADOSTextBox;
        internal ToolStripButton RefreshToolButton;
        internal ToolStripStatusLabel StatusLabel;
        internal SplitContainer InfoDataSplitter;
        internal Panel Panel3;
        private RemoteToolsControl remoteToolsControl;
        private Label PingHistLabel;
    }
}
