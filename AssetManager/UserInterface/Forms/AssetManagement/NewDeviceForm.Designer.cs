using System.Windows.Forms;
namespace AssetManager.UserInterface.Forms.AssetManagement
{

    partial class NewDeviceForm
    {
        //Form overrides dispose to clean up the component list.
        
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && components != null)
                {
                    components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
        //Required by the Windows Form Designer
        private System.ComponentModel.IContainer components  = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.GroupBox2 = new System.Windows.Forms.GroupBox();
            this.SerialTextBox = new System.Windows.Forms.TextBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.CurrentUserTextBox = new System.Windows.Forms.TextBox();
            this.MunisSearchButton = new System.Windows.Forms.Button();
            this.Label3 = new System.Windows.Forms.Label();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label12 = new System.Windows.Forms.Label();
            this.AssetTagTextBox = new System.Windows.Forms.TextBox();
            this.NoClearCheckBox = new System.Windows.Forms.CheckBox();
            this.TrackableCheckBox = new System.Windows.Forms.CheckBox();
            this.ClearButton = new System.Windows.Forms.Button();
            this.Label11 = new System.Windows.Forms.Label();
            this.StatusComboBox = new System.Windows.Forms.ComboBox();
            this.Label10 = new System.Windows.Forms.Label();
            this.OSTypeComboBox = new System.Windows.Forms.ComboBox();
            this.Label9 = new System.Windows.Forms.Label();
            this.POTextBox = new System.Windows.Forms.TextBox();
            this.Label8 = new System.Windows.Forms.Label();
            this.EquipTypeComboBox = new System.Windows.Forms.ComboBox();
            this.NotesTextBox = new System.Windows.Forms.TextBox();
            this.AddButton = new System.Windows.Forms.Button();
            this.Label6 = new System.Windows.Forms.Label();
            this.ReplaceYearTextBox = new System.Windows.Forms.TextBox();
            this.lbPurchaseDate = new System.Windows.Forms.Label();
            this.PurchaseDatePicker = new System.Windows.Forms.DateTimePicker();
            this.Label5 = new System.Windows.Forms.Label();
            this.LocationComboBox = new System.Windows.Forms.ComboBox();
            this.Label4 = new System.Windows.Forms.Label();
            this.DescriptionTextBox = new System.Windows.Forms.TextBox();
            this.GroupBox3 = new System.Windows.Forms.GroupBox();
            this.GroupBox4 = new System.Windows.Forms.GroupBox();
            this.GroupBox5 = new System.Windows.Forms.GroupBox();
            this.iCloudTextBox = new System.Windows.Forms.TextBox();
            this.Label14 = new System.Windows.Forms.Label();
            this.HostnameTextBox = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.PhoneNumTextBox = new System.Windows.Forms.MaskedTextBox();
            this.Label13 = new System.Windows.Forms.Label();
            this.GroupBox6 = new System.Windows.Forms.GroupBox();
            this.GroupBox7 = new System.Windows.Forms.GroupBox();
            this.ToolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.GroupBox2.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            this.GroupBox6.SuspendLayout();
            this.GroupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox2
            // 
            this.GroupBox2.Controls.Add(this.SerialTextBox);
            this.GroupBox2.Controls.Add(this.Label1);
            this.GroupBox2.Controls.Add(this.CurrentUserTextBox);
            this.GroupBox2.Controls.Add(this.MunisSearchButton);
            this.GroupBox2.Controls.Add(this.Label3);
            this.GroupBox2.Controls.Add(this.Label2);
            this.GroupBox2.Controls.Add(this.Label12);
            this.GroupBox2.Controls.Add(this.AssetTagTextBox);
            this.GroupBox2.Location = new System.Drawing.Point(12, 12);
            this.GroupBox2.Name = "GroupBox2";
            this.GroupBox2.Size = new System.Drawing.Size(236, 294);
            this.GroupBox2.TabIndex = 52;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Unique Info";
            // 
            // SerialTextBox
            // 
            this.SerialTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SerialTextBox.Location = new System.Drawing.Point(17, 46);
            this.SerialTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.SerialTextBox.Name = "SerialTextBox";
            this.SerialTextBox.Size = new System.Drawing.Size(178, 25);
            this.SerialTextBox.TabIndex = 0;
            this.SerialTextBox.TextChanged += new System.EventHandler(this.SerialTextBox_TextChanged);
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label1.Location = new System.Drawing.Point(14, 26);
            this.Label1.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(43, 16);
            this.Label1.TabIndex = 25;
            this.Label1.Text = "Serial";
            // 
            // CurrentUserTextBox
            // 
            this.CurrentUserTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CurrentUserTextBox.Location = new System.Drawing.Point(17, 144);
            this.CurrentUserTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.CurrentUserTextBox.Name = "CurrentUserTextBox";
            this.CurrentUserTextBox.Size = new System.Drawing.Size(178, 25);
            this.CurrentUserTextBox.TabIndex = 2;
            this.CurrentUserTextBox.DoubleClick += new System.EventHandler(this.CurrentUserTextBox_DoubleClick);
            // 
            // MunisSearchButton
            // 
            this.MunisSearchButton.Location = new System.Drawing.Point(36, 173);
            this.MunisSearchButton.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.MunisSearchButton.Name = "MunisSearchButton";
            this.MunisSearchButton.Size = new System.Drawing.Size(141, 23);
            this.MunisSearchButton.TabIndex = 50;
            this.MunisSearchButton.Text = "Munis Search";
            this.MunisSearchButton.UseVisualStyleBackColor = true;
            this.MunisSearchButton.Click += new System.EventHandler(this.MunisSearchButton_Click);
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label3.Location = new System.Drawing.Point(14, 124);
            this.Label3.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(37, 16);
            this.Label3.TabIndex = 29;
            this.Label3.Text = "User";
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label2.Location = new System.Drawing.Point(14, 75);
            this.Label2.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(70, 16);
            this.Label2.TabIndex = 26;
            this.Label2.Text = "Asset Tag";
            // 
            // Label12
            // 
            this.Label12.Font = new System.Drawing.Font("Tahoma", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label12.ForeColor = System.Drawing.Color.Gray;
            this.Label12.Location = new System.Drawing.Point(92, 79);
            this.Label12.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label12.Name = "Label12";
            this.Label12.Size = new System.Drawing.Size(112, 13);
            this.Label12.TabIndex = 51;
            this.Label12.Text = "(\"NA\" if not available.)";
            this.Label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AssetTagTextBox
            // 
            this.AssetTagTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AssetTagTextBox.Location = new System.Drawing.Point(17, 95);
            this.AssetTagTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.AssetTagTextBox.Name = "AssetTagTextBox";
            this.AssetTagTextBox.Size = new System.Drawing.Size(178, 25);
            this.AssetTagTextBox.TabIndex = 1;
            // 
            // NoClearCheckBox
            // 
            this.NoClearCheckBox.AutoSize = true;
            this.NoClearCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NoClearCheckBox.Location = new System.Drawing.Point(732, 392);
            this.NoClearCheckBox.Name = "NoClearCheckBox";
            this.NoClearCheckBox.Size = new System.Drawing.Size(91, 20);
            this.NoClearCheckBox.TabIndex = 16;
            this.NoClearCheckBox.Text = "Don\'t clear";
            this.NoClearCheckBox.UseVisualStyleBackColor = true;
            // 
            // TrackableCheckBox
            // 
            this.TrackableCheckBox.AutoSize = true;
            this.TrackableCheckBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TrackableCheckBox.Location = new System.Drawing.Point(31, 196);
            this.TrackableCheckBox.Name = "TrackableCheckBox";
            this.TrackableCheckBox.Size = new System.Drawing.Size(135, 20);
            this.TrackableCheckBox.TabIndex = 12;
            this.TrackableCheckBox.Text = "Trackable Device";
            this.TrackableCheckBox.UseVisualStyleBackColor = true;
            // 
            // ClearButton
            // 
            this.ClearButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ClearButton.Location = new System.Drawing.Point(693, 427);
            this.ClearButton.Name = "ClearButton";
            this.ClearButton.Size = new System.Drawing.Size(170, 23);
            this.ClearButton.TabIndex = 17;
            this.ClearButton.Text = "Clear";
            this.ClearButton.UseVisualStyleBackColor = true;
            this.ClearButton.Click += new System.EventHandler(this.ClearButton_Click);
            // 
            // Label11
            // 
            this.Label11.AutoSize = true;
            this.Label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label11.Location = new System.Drawing.Point(14, 226);
            this.Label11.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label11.Name = "Label11";
            this.Label11.Size = new System.Drawing.Size(91, 16);
            this.Label11.TabIndex = 47;
            this.Label11.Text = "Device Status";
            // 
            // StatusComboBox
            // 
            this.StatusComboBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusComboBox.FormattingEnabled = true;
            this.StatusComboBox.Location = new System.Drawing.Point(17, 246);
            this.StatusComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.StatusComboBox.Name = "StatusComboBox";
            this.StatusComboBox.Size = new System.Drawing.Size(251, 26);
            this.StatusComboBox.TabIndex = 7;
            // 
            // Label10
            // 
            this.Label10.AutoSize = true;
            this.Label10.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label10.Location = new System.Drawing.Point(14, 126);
            this.Label10.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label10.Name = "Label10";
            this.Label10.Size = new System.Drawing.Size(115, 16);
            this.Label10.TabIndex = 45;
            this.Label10.Text = "Operating System";
            // 
            // OSTypeComboBox
            // 
            this.OSTypeComboBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OSTypeComboBox.FormattingEnabled = true;
            this.OSTypeComboBox.Location = new System.Drawing.Point(17, 146);
            this.OSTypeComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.OSTypeComboBox.Name = "OSTypeComboBox";
            this.OSTypeComboBox.Size = new System.Drawing.Size(251, 26);
            this.OSTypeComboBox.TabIndex = 5;
            this.OSTypeComboBox.SelectedIndexChanged += new System.EventHandler(this.OSTypeComboBox_SelectedIndexChanged);
            // 
            // Label9
            // 
            this.Label9.AutoSize = true;
            this.Label9.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label9.Location = new System.Drawing.Point(11, 135);
            this.Label9.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label9.Name = "Label9";
            this.Label9.Size = new System.Drawing.Size(78, 16);
            this.Label9.TabIndex = 43;
            this.Label9.Text = "PO Number";
            // 
            // POTextBox
            // 
            this.POTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.POTextBox.Location = new System.Drawing.Point(14, 154);
            this.POTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.POTextBox.Name = "POTextBox";
            this.POTextBox.Size = new System.Drawing.Size(170, 25);
            this.POTextBox.TabIndex = 10;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label8.Location = new System.Drawing.Point(14, 76);
            this.Label8.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(107, 16);
            this.Label8.TabIndex = 41;
            this.Label8.Text = "Equipment Type";
            // 
            // EquipTypeComboBox
            // 
            this.EquipTypeComboBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.EquipTypeComboBox.FormattingEnabled = true;
            this.EquipTypeComboBox.Location = new System.Drawing.Point(17, 96);
            this.EquipTypeComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.EquipTypeComboBox.Name = "EquipTypeComboBox";
            this.EquipTypeComboBox.Size = new System.Drawing.Size(251, 26);
            this.EquipTypeComboBox.TabIndex = 4;
            // 
            // NotesTextBox
            // 
            this.NotesTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NotesTextBox.Location = new System.Drawing.Point(6, 21);
            this.NotesTextBox.MaxLength = 200;
            this.NotesTextBox.Multiline = true;
            this.NotesTextBox.Name = "NotesTextBox";
            this.NotesTextBox.Size = new System.Drawing.Size(366, 86);
            this.NotesTextBox.TabIndex = 14;
            // 
            // AddButton
            // 
            this.AddButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.AddButton.Location = new System.Drawing.Point(693, 342);
            this.AddButton.Name = "AddButton";
            this.AddButton.Size = new System.Drawing.Size(170, 44);
            this.AddButton.TabIndex = 15;
            this.AddButton.Text = "Add Device";
            this.AddButton.UseVisualStyleBackColor = true;
            this.AddButton.Click += new System.EventHandler(this.AddButton_Click);
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label6.Location = new System.Drawing.Point(10, 81);
            this.Label6.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(92, 16);
            this.Label6.TabIndex = 36;
            this.Label6.Text = "Replace Year";
            // 
            // ReplaceYearTextBox
            // 
            this.ReplaceYearTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ReplaceYearTextBox.Location = new System.Drawing.Point(14, 100);
            this.ReplaceYearTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.ReplaceYearTextBox.Name = "ReplaceYearTextBox";
            this.ReplaceYearTextBox.Size = new System.Drawing.Size(170, 25);
            this.ReplaceYearTextBox.TabIndex = 9;
            // 
            // lbPurchaseDate
            // 
            this.lbPurchaseDate.AutoSize = true;
            this.lbPurchaseDate.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbPurchaseDate.Location = new System.Drawing.Point(10, 27);
            this.lbPurchaseDate.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.lbPurchaseDate.Name = "lbPurchaseDate";
            this.lbPurchaseDate.Size = new System.Drawing.Size(97, 16);
            this.lbPurchaseDate.TabIndex = 34;
            this.lbPurchaseDate.Text = "Purchase Date";
            // 
            // PurchaseDatePicker
            // 
            this.PurchaseDatePicker.CustomFormat = "yyyy-MM-dd";
            this.PurchaseDatePicker.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PurchaseDatePicker.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.PurchaseDatePicker.Location = new System.Drawing.Point(13, 46);
            this.PurchaseDatePicker.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.PurchaseDatePicker.Name = "PurchaseDatePicker";
            this.PurchaseDatePicker.Size = new System.Drawing.Size(171, 25);
            this.PurchaseDatePicker.TabIndex = 8;
            this.PurchaseDatePicker.Value = new System.DateTime(2016, 4, 14, 0, 0, 0, 0);
            this.PurchaseDatePicker.ValueChanged += new System.EventHandler(this.PurchaseDatePicker_ValueChanged);
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label5.Location = new System.Drawing.Point(14, 176);
            this.Label5.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(59, 16);
            this.Label5.TabIndex = 32;
            this.Label5.Text = "Location";
            // 
            // LocationComboBox
            // 
            this.LocationComboBox.DropDownWidth = 171;
            this.LocationComboBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LocationComboBox.FormattingEnabled = true;
            this.LocationComboBox.Location = new System.Drawing.Point(17, 196);
            this.LocationComboBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.LocationComboBox.Name = "LocationComboBox";
            this.LocationComboBox.Size = new System.Drawing.Size(251, 26);
            this.LocationComboBox.TabIndex = 6;
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label4.Location = new System.Drawing.Point(14, 28);
            this.Label4.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(76, 16);
            this.Label4.TabIndex = 30;
            this.Label4.Text = "Description";
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DescriptionTextBox.Location = new System.Drawing.Point(17, 47);
            this.DescriptionTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(251, 25);
            this.DescriptionTextBox.TabIndex = 3;
            // 
            // GroupBox3
            // 
            this.GroupBox3.Controls.Add(this.Label4);
            this.GroupBox3.Controls.Add(this.DescriptionTextBox);
            this.GroupBox3.Controls.Add(this.LocationComboBox);
            this.GroupBox3.Controls.Add(this.Label5);
            this.GroupBox3.Controls.Add(this.EquipTypeComboBox);
            this.GroupBox3.Controls.Add(this.Label11);
            this.GroupBox3.Controls.Add(this.Label8);
            this.GroupBox3.Controls.Add(this.StatusComboBox);
            this.GroupBox3.Controls.Add(this.OSTypeComboBox);
            this.GroupBox3.Controls.Add(this.Label10);
            this.GroupBox3.Location = new System.Drawing.Point(254, 12);
            this.GroupBox3.Name = "GroupBox3";
            this.GroupBox3.Size = new System.Drawing.Size(305, 294);
            this.GroupBox3.TabIndex = 53;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Add\'l Info";
            // 
            // GroupBox4
            // 
            this.GroupBox4.Controls.Add(this.lbPurchaseDate);
            this.GroupBox4.Controls.Add(this.PurchaseDatePicker);
            this.GroupBox4.Controls.Add(this.ReplaceYearTextBox);
            this.GroupBox4.Controls.Add(this.Label6);
            this.GroupBox4.Controls.Add(this.POTextBox);
            this.GroupBox4.Controls.Add(this.Label9);
            this.GroupBox4.Location = new System.Drawing.Point(565, 12);
            this.GroupBox4.Name = "GroupBox4";
            this.GroupBox4.Size = new System.Drawing.Size(214, 294);
            this.GroupBox4.TabIndex = 54;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Purchase Info";
            // 
            // GroupBox5
            // 
            this.GroupBox5.Controls.Add(this.iCloudTextBox);
            this.GroupBox5.Controls.Add(this.Label14);
            this.GroupBox5.Controls.Add(this.HostnameTextBox);
            this.GroupBox5.Controls.Add(this.Label7);
            this.GroupBox5.Controls.Add(this.PhoneNumTextBox);
            this.GroupBox5.Controls.Add(this.Label13);
            this.GroupBox5.Controls.Add(this.TrackableCheckBox);
            this.GroupBox5.Location = new System.Drawing.Point(785, 12);
            this.GroupBox5.Name = "GroupBox5";
            this.GroupBox5.Size = new System.Drawing.Size(205, 294);
            this.GroupBox5.TabIndex = 55;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Misc";
            // 
            // iCloudTextBox
            // 
            this.iCloudTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iCloudTextBox.Location = new System.Drawing.Point(14, 154);
            this.iCloudTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.iCloudTextBox.Name = "iCloudTextBox";
            this.iCloudTextBox.Size = new System.Drawing.Size(178, 25);
            this.iCloudTextBox.TabIndex = 13;
            // 
            // Label14
            // 
            this.Label14.AutoSize = true;
            this.Label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label14.Location = new System.Drawing.Point(11, 134);
            this.Label14.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label14.Name = "Label14";
            this.Label14.Size = new System.Drawing.Size(97, 16);
            this.Label14.TabIndex = 62;
            this.Label14.Text = "iCloud Account";
            // 
            // HostnameTextBox
            // 
            this.HostnameTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HostnameTextBox.Location = new System.Drawing.Point(14, 100);
            this.HostnameTextBox.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.HostnameTextBox.Name = "HostnameTextBox";
            this.HostnameTextBox.Size = new System.Drawing.Size(178, 25);
            this.HostnameTextBox.TabIndex = 12;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label7.Location = new System.Drawing.Point(11, 80);
            this.Label7.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(70, 16);
            this.Label7.TabIndex = 60;
            this.Label7.Text = "Hostname";
            // 
            // PhoneNumTextBox
            // 
            this.PhoneNumTextBox.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PhoneNumTextBox.Location = new System.Drawing.Point(14, 46);
            this.PhoneNumTextBox.Mask = "(999) 000-0000";
            this.PhoneNumTextBox.Name = "PhoneNumTextBox";
            this.PhoneNumTextBox.Size = new System.Drawing.Size(178, 25);
            this.PhoneNumTextBox.TabIndex = 11;
            this.PhoneNumTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.ExcludePromptAndLiterals;
            this.PhoneNumTextBox.Leave += new System.EventHandler(this.PhoneNumTextBox_Leave);
            // 
            // Label13
            // 
            this.Label13.AutoSize = true;
            this.Label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Label13.Location = new System.Drawing.Point(11, 25);
            this.Label13.Margin = new System.Windows.Forms.Padding(2, 2, 40, 2);
            this.Label13.Name = "Label13";
            this.Label13.Size = new System.Drawing.Size(98, 16);
            this.Label13.TabIndex = 50;
            this.Label13.Text = "Phone Number";
            // 
            // GroupBox6
            // 
            this.GroupBox6.Controls.Add(this.NotesTextBox);
            this.GroupBox6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox6.Location = new System.Drawing.Point(84, 19);
            this.GroupBox6.Name = "GroupBox6";
            this.GroupBox6.Size = new System.Drawing.Size(378, 119);
            this.GroupBox6.TabIndex = 56;
            this.GroupBox6.TabStop = false;
            this.GroupBox6.Text = "Notes";
            // 
            // GroupBox7
            // 
            this.GroupBox7.Controls.Add(this.GroupBox6);
            this.GroupBox7.Location = new System.Drawing.Point(12, 312);
            this.GroupBox7.Name = "GroupBox7";
            this.GroupBox7.Size = new System.Drawing.Size(547, 151);
            this.GroupBox7.TabIndex = 57;
            this.GroupBox7.TabStop = false;
            // 
            // NewDeviceForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(232)))), ((int)(((byte)(232)))));
            this.ClientSize = new System.Drawing.Size(997, 472);
            this.Controls.Add(this.NoClearCheckBox);
            this.Controls.Add(this.ClearButton);
            this.Controls.Add(this.GroupBox7);
            this.Controls.Add(this.AddButton);
            this.Controls.Add(this.GroupBox5);
            this.Controls.Add(this.GroupBox2);
            this.Controls.Add(this.GroupBox4);
            this.Controls.Add(this.GroupBox3);
            this.DoubleBuffered = true;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "NewDeviceForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Add New Device";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.NewDeviceForm_FormClosed);
            this.Load += new System.EventHandler(this.NewDeviceForm_Load);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox4.PerformLayout();
            this.GroupBox5.ResumeLayout(false);
            this.GroupBox5.PerformLayout();
            this.GroupBox6.ResumeLayout(false);
            this.GroupBox6.PerformLayout();
            this.GroupBox7.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        internal Label Label10;
        internal ComboBox OSTypeComboBox;
        internal Label Label9;
        internal TextBox POTextBox;
        internal Label Label8;
        internal ComboBox EquipTypeComboBox;
        internal TextBox NotesTextBox;
        internal Button AddButton;
        internal Label Label6;
        internal TextBox ReplaceYearTextBox;
        internal Label lbPurchaseDate;
        internal DateTimePicker PurchaseDatePicker;
        internal Label Label5;
        internal ComboBox LocationComboBox;
        internal Label Label4;
        internal Label Label3;
        internal TextBox DescriptionTextBox;
        internal TextBox CurrentUserTextBox;
        internal Label Label2;
        internal Label Label1;
        internal TextBox AssetTagTextBox;
        internal TextBox SerialTextBox;
        internal Label Label11;
        internal ComboBox StatusComboBox;
        internal Button ClearButton;
        internal CheckBox TrackableCheckBox;
        internal CheckBox NoClearCheckBox;
        internal Button MunisSearchButton;
        internal Label Label12;
        internal GroupBox GroupBox2;
        internal GroupBox GroupBox7;
        internal GroupBox GroupBox6;
        internal GroupBox GroupBox5;
        internal Label Label13;
        internal GroupBox GroupBox4;
        internal GroupBox GroupBox3;
        internal MaskedTextBox PhoneNumTextBox;
        internal TextBox HostnameTextBox;
        internal Label Label7;
        internal TextBox iCloudTextBox;
        internal Label Label14;
        internal ToolTip ToolTip1;
    }
}
