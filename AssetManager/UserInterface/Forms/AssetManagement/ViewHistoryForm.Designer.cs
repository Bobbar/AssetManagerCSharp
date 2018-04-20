using System.Windows.Forms;namespace AssetManager.UserInterface.Forms.AssetManagement
{

    partial class ViewHistoryForm
    {
        //Required by the Windows Form Designer
        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.

        private void InitializeComponent()
        {
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.NotesTextBox = new System.Windows.Forms.RichTextBox();
            this.Label20 = new System.Windows.Forms.Label();
            this.iCloudTextBox = new System.Windows.Forms.TextBox();
            this.Label19 = new System.Windows.Forms.Label();
            this.HostnameTextBox = new System.Windows.Forms.TextBox();
            this.Label18 = new System.Windows.Forms.Label();
            this.PhoneNumberTextBox = new System.Windows.Forms.TextBox();
            this.chkTrackable = new System.Windows.Forms.CheckBox();
            this.Label17 = new System.Windows.Forms.Label();
            this.EntryGuidTextBox = new System.Windows.Forms.TextBox();
            this.Label16 = new System.Windows.Forms.Label();
            this.StatusTextBox = new System.Windows.Forms.TextBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.ActionUserTextBox = new System.Windows.Forms.TextBox();
            this.Label13 = new System.Windows.Forms.Label();
            this.EntryTimeTextBox = new System.Windows.Forms.TextBox();
            this.Label12 = new System.Windows.Forms.Label();
            this.GuidTextBox = new System.Windows.Forms.TextBox();
            this.Label11 = new System.Windows.Forms.Label();
            this.EQTypeTextBox = new System.Windows.Forms.TextBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.OSVersionTextBox = new System.Windows.Forms.TextBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.PONumberTextBox = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.ReplaceYearTextBox = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.PurchaseDateTextBox = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.LocationTextBox = new System.Windows.Forms.TextBox();
            this.Label5 = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.SerialTextBox = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.AssetTagTextBox = new System.Windows.Forms.TextBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.CurrentUserTextBox = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.ChangeTypeTextBox = new System.Windows.Forms.TextBox();
            this.GroupBox1.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.GroupBox1.Controls.Add(this.GroupBox2);
            this.GroupBox1.Controls.Add(this.Label20);
            this.GroupBox1.Controls.Add(this.iCloudTextBox);
            this.GroupBox1.Controls.Add(this.Label19);
            this.GroupBox1.Controls.Add(this.HostnameTextBox);
            this.GroupBox1.Controls.Add(this.Label18);
            this.GroupBox1.Controls.Add(this.PhoneNumberTextBox);
            this.GroupBox1.Controls.Add(this.chkTrackable);
            this.GroupBox1.Controls.Add(this.Label17);
            this.GroupBox1.Controls.Add(this.EntryGuidTextBox);
            this.GroupBox1.Controls.Add(this.Label16);
            this.GroupBox1.Controls.Add(this.StatusTextBox);
            this.GroupBox1.Controls.Add(this.Label14);
            this.GroupBox1.Controls.Add(this.ActionUserTextBox);
            this.GroupBox1.Controls.Add(this.Label13);
            this.GroupBox1.Controls.Add(this.EntryTimeTextBox);
            this.GroupBox1.Controls.Add(this.Label12);
            this.GroupBox1.Controls.Add(this.GuidTextBox);
            this.GroupBox1.Controls.Add(this.Label11);
            this.GroupBox1.Controls.Add(this.EQTypeTextBox);
            this.GroupBox1.Controls.Add(this.Label10);
            this.GroupBox1.Controls.Add(this.OSVersionTextBox);
            this.GroupBox1.Controls.Add(this.Label9);
            this.GroupBox1.Controls.Add(this.PONumberTextBox);
            this.GroupBox1.Controls.Add(this.Label8);
            this.GroupBox1.Controls.Add(this.ReplaceYearTextBox);
            this.GroupBox1.Controls.Add(this.Label7);
            this.GroupBox1.Controls.Add(this.PurchaseDateTextBox);
            this.GroupBox1.Controls.Add(this.Label6);
            this.GroupBox1.Controls.Add(this.LocationTextBox);
            this.GroupBox1.Controls.Add(this.Label5);
            this.GroupBox1.Controls.Add(this.DescriptionTextBox);
            this.GroupBox1.Controls.Add(this.Label4);
            this.GroupBox1.Controls.Add(this.SerialTextBox);
            this.GroupBox1.Controls.Add(this.Label3);
            this.GroupBox1.Controls.Add(this.AssetTagTextBox);
            this.GroupBox1.Controls.Add(this.Label2);
            this.GroupBox1.Controls.Add(this.CurrentUserTextBox);
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.ChangeTypeTextBox);
            this.GroupBox1.Location = new System.Drawing.Point(13, 14);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(807, 407);
            this.GroupBox1.TabIndex = 0;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Device Info Snapshot";
            // 
            // GroupBox2
            // 
            this.GroupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox2.Controls.Add(this.NotesTextBox);
            this.GroupBox2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox2.Location = new System.Drawing.Point(6, 230);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(795, 171);
            this.GroupBox2.TabIndex = 43;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Notes";
            // 
            // NotesTextBox
            // 
            this.NotesTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.NotesTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NotesTextBox.Location = new System.Drawing.Point(3, 19);
            this.NotesTextBox.Name = "NotesTextBox";
            this.NotesTextBox.ReadOnly = true;
            this.NotesTextBox.Size = new System.Drawing.Size(789, 149);
            this.NotesTextBox.TabIndex = 42;
            this.NotesTextBox.TabStop = false;
            this.NotesTextBox.Text = "";
            // 
            // Label20
            // 
            this.Label20.AutoSize = true;
            this.Label20.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label20.Location = new System.Drawing.Point(560, 162);
            this.Label20.Name = "Label20";
            this.Label20.Size = new System.Drawing.Size(84, 15);
            this.Label20.TabIndex = 41;
            this.Label20.Text = "iCloud Acct";
            // 
            // iCloudTextBox
            // 
            this.iCloudTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.iCloudTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iCloudTextBox.Location = new System.Drawing.Point(563, 181);
            this.iCloudTextBox.Name = "iCloudTextBox";
            this.iCloudTextBox.ReadOnly = true;
            this.iCloudTextBox.Size = new System.Drawing.Size(228, 23);
            this.iCloudTextBox.TabIndex = 40;
            this.iCloudTextBox.TabStop = false;
            // 
            // Label19
            // 
            this.Label19.AutoSize = true;
            this.Label19.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label19.Location = new System.Drawing.Point(14, 162);
            this.Label19.Name = "Label19";
            this.Label19.Size = new System.Drawing.Size(70, 15);
            this.Label19.TabIndex = 39;
            this.Label19.Text = "Hostname:";
            // 
            // HostnameTextBox
            // 
            this.HostnameTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.HostnameTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HostnameTextBox.Location = new System.Drawing.Point(17, 181);
            this.HostnameTextBox.Name = "HostnameTextBox";
            this.HostnameTextBox.ReadOnly = true;
            this.HostnameTextBox.Size = new System.Drawing.Size(155, 23);
            this.HostnameTextBox.TabIndex = 38;
            this.HostnameTextBox.TabStop = false;
            // 
            // Label18
            // 
            this.Label18.AutoSize = true;
            this.Label18.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label18.Location = new System.Drawing.Point(14, 119);
            this.Label18.Name = "Label18";
            this.Label18.Size = new System.Drawing.Size(49, 15);
            this.Label18.TabIndex = 37;
            this.Label18.Text = "Phone:";
            // 
            // PhoneNumberTextBox
            // 
            this.PhoneNumberTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.PhoneNumberTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PhoneNumberTextBox.Location = new System.Drawing.Point(17, 138);
            this.PhoneNumberTextBox.Name = "PhoneNumberTextBox";
            this.PhoneNumberTextBox.ReadOnly = true;
            this.PhoneNumberTextBox.Size = new System.Drawing.Size(155, 23);
            this.PhoneNumberTextBox.TabIndex = 36;
            this.PhoneNumberTextBox.TabStop = false;
            // 
            // chkTrackable
            // 
            this.chkTrackable.AutoSize = true;
            this.chkTrackable.Enabled = false;
            this.chkTrackable.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.chkTrackable.Location = new System.Drawing.Point(563, 210);
            this.chkTrackable.Name = "chkTrackable";
            this.chkTrackable.Size = new System.Drawing.Size(89, 19);
            this.chkTrackable.TabIndex = 35;
            this.chkTrackable.TabStop = false;
            this.chkTrackable.Text = "Trackable";
            this.chkTrackable.UseVisualStyleBackColor = true;
            // 
            // Label17
            // 
            this.Label17.AutoSize = true;
            this.Label17.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label17.Location = new System.Drawing.Point(560, 75);
            this.Label17.Name = "Label17";
            this.Label17.Size = new System.Drawing.Size(77, 15);
            this.Label17.TabIndex = 34;
            this.Label17.Text = "Entry Guid";
            // 
            // EntryGuidTextBox
            // 
            this.EntryGuidTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.EntryGuidTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EntryGuidTextBox.Location = new System.Drawing.Point(563, 94);
            this.EntryGuidTextBox.Name = "EntryGuidTextBox";
            this.EntryGuidTextBox.ReadOnly = true;
            this.EntryGuidTextBox.Size = new System.Drawing.Size(228, 23);
            this.EntryGuidTextBox.TabIndex = 33;
            this.EntryGuidTextBox.TabStop = false;
            // 
            // Label16
            // 
            this.Label16.AutoSize = true;
            this.Label16.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label16.Location = new System.Drawing.Point(560, 119);
            this.Label16.Name = "Label16";
            this.Label16.Size = new System.Drawing.Size(49, 15);
            this.Label16.TabIndex = 32;
            this.Label16.Text = "Status";
            // 
            // StatusTextBox
            // 
            this.StatusTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.StatusTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusTextBox.Location = new System.Drawing.Point(563, 138);
            this.StatusTextBox.Name = "StatusTextBox";
            this.StatusTextBox.ReadOnly = true;
            this.StatusTextBox.Size = new System.Drawing.Size(108, 23);
            this.StatusTextBox.TabIndex = 31;
            this.StatusTextBox.TabStop = false;
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label14.Location = new System.Drawing.Point(14, 75);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(84, 15);
            this.Label14.TabIndex = 27;
            this.Label14.Text = "Action User";
            // 
            // ActionUserTextBox
            // 
            this.ActionUserTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.ActionUserTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ActionUserTextBox.Location = new System.Drawing.Point(17, 94);
            this.ActionUserTextBox.Name = "ActionUserTextBox";
            this.ActionUserTextBox.ReadOnly = true;
            this.ActionUserTextBox.Size = new System.Drawing.Size(155, 23);
            this.ActionUserTextBox.TabIndex = 26;
            this.ActionUserTextBox.TabStop = false;
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label13.Location = new System.Drawing.Point(14, 28);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(77, 15);
            this.Label13.TabIndex = 25;
            this.Label13.Text = "Time Stamp";
            // 
            // EntryTimeTextBox
            // 
            this.EntryTimeTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.EntryTimeTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EntryTimeTextBox.Location = new System.Drawing.Point(17, 47);
            this.EntryTimeTextBox.Name = "EntryTimeTextBox";
            this.EntryTimeTextBox.ReadOnly = true;
            this.EntryTimeTextBox.Size = new System.Drawing.Size(155, 23);
            this.EntryTimeTextBox.TabIndex = 24;
            this.EntryTimeTextBox.TabStop = false;
            // 
            // Label12
            // 
            this.Label12.AutoSize = true;
            this.Label12.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label12.Location = new System.Drawing.Point(560, 28);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(84, 15);
            this.Label12.TabIndex = 23;
            this.Label12.Text = "Device Guid";
            // 
            // GuidTextBox
            // 
            this.GuidTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.GuidTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GuidTextBox.Location = new System.Drawing.Point(563, 47);
            this.GuidTextBox.Name = "GuidTextBox";
            this.GuidTextBox.ReadOnly = true;
            this.GuidTextBox.Size = new System.Drawing.Size(228, 23);
            this.GuidTextBox.TabIndex = 22;
            this.GuidTextBox.TabStop = false;
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label11.Location = new System.Drawing.Point(438, 162);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(56, 15);
            this.Label11.TabIndex = 21;
            this.Label11.Text = "EQ Type";
            // 
            // EQTypeTextBox
            // 
            this.EQTypeTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.EQTypeTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EQTypeTextBox.Location = new System.Drawing.Point(441, 181);
            this.EQTypeTextBox.Name = "EQTypeTextBox";
            this.EQTypeTextBox.ReadOnly = true;
            this.EQTypeTextBox.Size = new System.Drawing.Size(108, 23);
            this.EQTypeTextBox.TabIndex = 20;
            this.EQTypeTextBox.TabStop = false;
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label10.Location = new System.Drawing.Point(438, 119);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(77, 15);
            this.Label10.TabIndex = 19;
            this.Label10.Text = "OS Version";
            // 
            // OSVersionTextBox
            // 
            this.OSVersionTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.OSVersionTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OSVersionTextBox.Location = new System.Drawing.Point(441, 138);
            this.OSVersionTextBox.Name = "OSVersionTextBox";
            this.OSVersionTextBox.ReadOnly = true;
            this.OSVersionTextBox.Size = new System.Drawing.Size(108, 23);
            this.OSVersionTextBox.TabIndex = 18;
            this.OSVersionTextBox.TabStop = false;
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(438, 75);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(70, 15);
            this.Label9.TabIndex = 17;
            this.Label9.Text = "PO Number";
            // 
            // PONumberTextBox
            // 
            this.PONumberTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.PONumberTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PONumberTextBox.Location = new System.Drawing.Point(441, 94);
            this.PONumberTextBox.Name = "PONumberTextBox";
            this.PONumberTextBox.ReadOnly = true;
            this.PONumberTextBox.Size = new System.Drawing.Size(108, 23);
            this.PONumberTextBox.TabIndex = 16;
            this.PONumberTextBox.TabStop = false;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.Location = new System.Drawing.Point(317, 162);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(119, 15);
            this.Label8.TabIndex = 15;
            this.Label8.Text = "Replacement Year";
            // 
            // ReplaceYearTextBox
            // 
            this.ReplaceYearTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.ReplaceYearTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReplaceYearTextBox.Location = new System.Drawing.Point(320, 181);
            this.ReplaceYearTextBox.Name = "ReplaceYearTextBox";
            this.ReplaceYearTextBox.ReadOnly = true;
            this.ReplaceYearTextBox.Size = new System.Drawing.Size(108, 23);
            this.ReplaceYearTextBox.TabIndex = 14;
            this.ReplaceYearTextBox.TabStop = false;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(317, 119);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(98, 15);
            this.Label7.TabIndex = 13;
            this.Label7.Text = "Purchase Date";
            // 
            // PurchaseDateTextBox
            // 
            this.PurchaseDateTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.PurchaseDateTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PurchaseDateTextBox.Location = new System.Drawing.Point(320, 138);
            this.PurchaseDateTextBox.Name = "PurchaseDateTextBox";
            this.PurchaseDateTextBox.ReadOnly = true;
            this.PurchaseDateTextBox.Size = new System.Drawing.Size(108, 23);
            this.PurchaseDateTextBox.TabIndex = 12;
            this.PurchaseDateTextBox.TabStop = false;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(317, 75);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(63, 15);
            this.Label6.TabIndex = 11;
            this.Label6.Text = "Location";
            // 
            // LocationTextBox
            // 
            this.LocationTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.LocationTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocationTextBox.Location = new System.Drawing.Point(320, 94);
            this.LocationTextBox.Name = "LocationTextBox";
            this.LocationTextBox.ReadOnly = true;
            this.LocationTextBox.Size = new System.Drawing.Size(108, 23);
            this.LocationTextBox.TabIndex = 10;
            this.LocationTextBox.TabStop = false;
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(317, 28);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(84, 15);
            this.Label5.TabIndex = 9;
            this.Label5.Text = "Description";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.DescriptionTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionTextBox.Location = new System.Drawing.Point(320, 47);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.ReadOnly = true;
            this.DescriptionTextBox.Size = new System.Drawing.Size(229, 23);
            this.DescriptionTextBox.TabIndex = 8;
            this.DescriptionTextBox.TabStop = false;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(191, 162);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(49, 15);
            this.Label4.TabIndex = 7;
            this.Label4.Text = "Serial";
            // 
            // SerialTextBox
            // 
            this.SerialTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.SerialTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialTextBox.Location = new System.Drawing.Point(194, 181);
            this.SerialTextBox.Name = "SerialTextBox";
            this.SerialTextBox.ReadOnly = true;
            this.SerialTextBox.Size = new System.Drawing.Size(108, 23);
            this.SerialTextBox.TabIndex = 6;
            this.SerialTextBox.TabStop = false;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(191, 119);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(70, 15);
            this.Label3.TabIndex = 5;
            this.Label3.Text = "Asset Tag";
            // 
            // AssetTagTextBox
            // 
            this.AssetTagTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.AssetTagTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AssetTagTextBox.Location = new System.Drawing.Point(194, 138);
            this.AssetTagTextBox.Name = "AssetTagTextBox";
            this.AssetTagTextBox.ReadOnly = true;
            this.AssetTagTextBox.Size = new System.Drawing.Size(108, 23);
            this.AssetTagTextBox.TabIndex = 4;
            this.AssetTagTextBox.TabStop = false;
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(189, 75);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(35, 15);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "User";
            // 
            // CurrentUserTextBox
            // 
            this.CurrentUserTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.CurrentUserTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentUserTextBox.Location = new System.Drawing.Point(192, 94);
            this.CurrentUserTextBox.Name = "CurrentUserTextBox";
            this.CurrentUserTextBox.ReadOnly = true;
            this.CurrentUserTextBox.Size = new System.Drawing.Size(108, 23);
            this.CurrentUserTextBox.TabIndex = 2;
            this.CurrentUserTextBox.TabStop = false;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(189, 28);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(84, 15);
            this.Label1.TabIndex = 1;
            this.Label1.Text = "Change Type";
            // 
            // ChangeTypeTextBox
            // 
            this.ChangeTypeTextBox.BackColor = System.Drawing.SystemColors.Window;
            this.ChangeTypeTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ChangeTypeTextBox.Location = new System.Drawing.Point(192, 47);
            this.ChangeTypeTextBox.Name = "ChangeTypeTextBox";
            this.ChangeTypeTextBox.ReadOnly = true;
            this.ChangeTypeTextBox.Size = new System.Drawing.Size(110, 23);
            this.ChangeTypeTextBox.TabIndex = 0;
            this.ChangeTypeTextBox.TabStop = false;
            // 
            // ViewHistoryForm
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ClientSize = new System.Drawing.Size(832, 431);
            this.Controls.Add(this.GroupBox1);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MinimumSize = new System.Drawing.Size(848, 385);
            this.Name = "ViewHistoryForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "View Entry";
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.GroupBox2.ResumeLayout(false);
            this.ResumeLayout(false);

        }
        internal GroupBox GroupBox1;
        internal Label Label2;
        internal TextBox CurrentUserTextBox;
        internal Label Label1;
        internal TextBox ChangeTypeTextBox;
        internal Label Label3;
        internal TextBox AssetTagTextBox;
        internal Label Label12;
        internal TextBox GuidTextBox;
        internal Label Label11;
        internal TextBox EQTypeTextBox;
        internal Label Label10;
        internal TextBox OSVersionTextBox;
        internal Label Label9;
        internal TextBox PONumberTextBox;
        internal Label Label8;
        internal TextBox ReplaceYearTextBox;
        internal Label Label7;
        internal TextBox PurchaseDateTextBox;
        internal Label Label6;
        internal TextBox LocationTextBox;
        internal Label Label5;
        internal TextBox DescriptionTextBox;
        internal Label Label4;
        internal TextBox SerialTextBox;
        internal Label Label13;
        internal TextBox EntryTimeTextBox;
        internal Label Label14;
        internal TextBox ActionUserTextBox;
        internal Label Label16;
        internal TextBox StatusTextBox;
        internal Label Label17;
        internal TextBox EntryGuidTextBox;
        internal CheckBox chkTrackable;
        internal Label Label18;
        internal TextBox PhoneNumberTextBox;
        internal Label Label19;
        internal TextBox HostnameTextBox;
        internal Label Label20;
        internal TextBox iCloudTextBox;
        internal GroupBox GroupBox2;
        internal RichTextBox NotesTextBox;
    }
}
