namespace AssetManager.UserInterface.Forms.Sibi
{
    partial class SibiManageItemForm
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
            this.components = new System.ComponentModel.Container();
            this.ItemGroupBox = new System.Windows.Forms.GroupBox();
            this.BudgetGroupBox = new System.Windows.Forms.GroupBox();
            this.ApprovalRequiredCheckBox = new System.Windows.Forms.CheckBox();
            this.OrgTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.BudgetLineTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.ObjectTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.ApproverTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.FindSupervisorButton = new System.Windows.Forms.Button();
            this.AcceptButton = new System.Windows.Forms.Button();
            this.NewSerialTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.NewAssetTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.ReplaceSerialTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.ReplaceAssetTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.StatusComboBox = new AssetManager.UserInterface.CustomControls.LabeledComboBox();
            this.LocationComboBox = new AssetManager.UserInterface.CustomControls.LabeledComboBox();
            this.QuantityTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.DescriptionTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.UserTextBox = new AssetManager.UserInterface.CustomControls.LabeledTextBox();
            this.errorProvider = new System.Windows.Forms.ErrorProvider(this.components);
            this.ItemGroupBox.SuspendLayout();
            this.BudgetGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).BeginInit();
            this.SuspendLayout();
            // 
            // ItemGroupBox
            // 
            this.ItemGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ItemGroupBox.Controls.Add(this.BudgetGroupBox);
            this.ItemGroupBox.Controls.Add(this.AcceptButton);
            this.ItemGroupBox.Controls.Add(this.NewSerialTextBox);
            this.ItemGroupBox.Controls.Add(this.NewAssetTextBox);
            this.ItemGroupBox.Controls.Add(this.ReplaceSerialTextBox);
            this.ItemGroupBox.Controls.Add(this.ReplaceAssetTextBox);
            this.ItemGroupBox.Controls.Add(this.StatusComboBox);
            this.ItemGroupBox.Controls.Add(this.LocationComboBox);
            this.ItemGroupBox.Controls.Add(this.QuantityTextBox);
            this.ItemGroupBox.Controls.Add(this.DescriptionTextBox);
            this.ItemGroupBox.Controls.Add(this.UserTextBox);
            this.ItemGroupBox.Location = new System.Drawing.Point(12, 12);
            this.ItemGroupBox.Name = "ItemGroupBox";
            this.ItemGroupBox.Size = new System.Drawing.Size(736, 399);
            this.ItemGroupBox.TabIndex = 0;
            this.ItemGroupBox.TabStop = false;
            this.ItemGroupBox.Text = "Item Info";
            // 
            // BudgetGroupBox
            // 
            this.BudgetGroupBox.Controls.Add(this.ApprovalRequiredCheckBox);
            this.BudgetGroupBox.Controls.Add(this.OrgTextBox);
            this.BudgetGroupBox.Controls.Add(this.BudgetLineTextBox);
            this.BudgetGroupBox.Controls.Add(this.ObjectTextBox);
            this.BudgetGroupBox.Controls.Add(this.ApproverTextBox);
            this.BudgetGroupBox.Controls.Add(this.FindSupervisorButton);
            this.BudgetGroupBox.Location = new System.Drawing.Point(331, 160);
            this.BudgetGroupBox.Name = "BudgetGroupBox";
            this.BudgetGroupBox.Size = new System.Drawing.Size(369, 158);
            this.BudgetGroupBox.TabIndex = 16;
            this.BudgetGroupBox.TabStop = false;
            this.BudgetGroupBox.Text = "Budget Info";
            // 
            // ApprovalRequiredCheckBox
            // 
            this.ApprovalRequiredCheckBox.AutoSize = true;
            this.ApprovalRequiredCheckBox.Location = new System.Drawing.Point(26, 124);
            this.ApprovalRequiredCheckBox.Name = "ApprovalRequiredCheckBox";
            this.ApprovalRequiredCheckBox.Size = new System.Drawing.Size(145, 19);
            this.ApprovalRequiredCheckBox.TabIndex = 16;
            this.ApprovalRequiredCheckBox.Text = "Approval Required";
            this.ApprovalRequiredCheckBox.UseVisualStyleBackColor = true;
            // 
            // OrgTextBox
            // 
            this.OrgTextBox.LabelText = "Org Code:";
            this.OrgTextBox.Location = new System.Drawing.Point(22, 25);
            this.OrgTextBox.Name = "OrgTextBox";
            this.OrgTextBox.Size = new System.Drawing.Size(149, 40);
            this.OrgTextBox.TabIndex = 10;
            // 
            // BudgetLineTextBox
            // 
            this.BudgetLineTextBox.LabelText = "Budget Line #:";
            this.BudgetLineTextBox.Location = new System.Drawing.Point(22, 71);
            this.BudgetLineTextBox.Name = "BudgetLineTextBox";
            this.BudgetLineTextBox.Size = new System.Drawing.Size(149, 40);
            this.BudgetLineTextBox.TabIndex = 15;
            // 
            // ObjectTextBox
            // 
            this.ObjectTextBox.LabelText = "Object Code:";
            this.ObjectTextBox.Location = new System.Drawing.Point(189, 25);
            this.ObjectTextBox.Name = "ObjectTextBox";
            this.ObjectTextBox.Size = new System.Drawing.Size(149, 40);
            this.ObjectTextBox.TabIndex = 11;
            // 
            // ApproverTextBox
            // 
            this.ApproverTextBox.LabelText = "Approver:";
            this.ApproverTextBox.Location = new System.Drawing.Point(189, 71);
            this.ApproverTextBox.Name = "ApproverTextBox";
            this.ApproverTextBox.Size = new System.Drawing.Size(149, 40);
            this.ApproverTextBox.TabIndex = 12;
            // 
            // FindSupervisorButton
            // 
            this.FindSupervisorButton.Location = new System.Drawing.Point(189, 117);
            this.FindSupervisorButton.Name = "FindSupervisorButton";
            this.FindSupervisorButton.Size = new System.Drawing.Size(149, 26);
            this.FindSupervisorButton.TabIndex = 13;
            this.FindSupervisorButton.Text = "Find";
            this.FindSupervisorButton.UseVisualStyleBackColor = true;
            // 
            // AcceptButton
            // 
            this.AcceptButton.Location = new System.Drawing.Point(308, 342);
            this.AcceptButton.Name = "AcceptButton";
            this.AcceptButton.Size = new System.Drawing.Size(117, 41);
            this.AcceptButton.TabIndex = 14;
            this.AcceptButton.Text = "Accept";
            this.AcceptButton.UseVisualStyleBackColor = true;
            this.AcceptButton.Click += new System.EventHandler(this.AcceptButton_Click);
            // 
            // NewSerialTextBox
            // 
            this.NewSerialTextBox.LabelText = "New Serial:";
            this.NewSerialTextBox.Location = new System.Drawing.Point(520, 77);
            this.NewSerialTextBox.Name = "NewSerialTextBox";
            this.NewSerialTextBox.Size = new System.Drawing.Size(149, 40);
            this.NewSerialTextBox.TabIndex = 9;
            // 
            // NewAssetTextBox
            // 
            this.NewAssetTextBox.LabelText = "New Asset:";
            this.NewAssetTextBox.Location = new System.Drawing.Point(376, 77);
            this.NewAssetTextBox.Name = "NewAssetTextBox";
            this.NewAssetTextBox.Size = new System.Drawing.Size(128, 40);
            this.NewAssetTextBox.TabIndex = 8;
            // 
            // ReplaceSerialTextBox
            // 
            this.ReplaceSerialTextBox.LabelText = "Replace Serial:";
            this.ReplaceSerialTextBox.Location = new System.Drawing.Point(520, 30);
            this.ReplaceSerialTextBox.Name = "ReplaceSerialTextBox";
            this.ReplaceSerialTextBox.Size = new System.Drawing.Size(149, 40);
            this.ReplaceSerialTextBox.TabIndex = 7;
            // 
            // ReplaceAssetTextBox
            // 
            this.ReplaceAssetTextBox.LabelText = "Replace Asset:";
            this.ReplaceAssetTextBox.Location = new System.Drawing.Point(376, 31);
            this.ReplaceAssetTextBox.Name = "ReplaceAssetTextBox";
            this.ReplaceAssetTextBox.Size = new System.Drawing.Size(128, 40);
            this.ReplaceAssetTextBox.TabIndex = 6;
            // 
            // StatusComboBox
            // 
            this.StatusComboBox.LabelText = "Status:";
            this.StatusComboBox.Location = new System.Drawing.Point(16, 215);
            this.StatusComboBox.Name = "StatusComboBox";
            this.StatusComboBox.SelectedIndex = -1;
            this.StatusComboBox.Size = new System.Drawing.Size(195, 40);
            this.StatusComboBox.TabIndex = 5;
            // 
            // LocationComboBox
            // 
            this.LocationComboBox.LabelText = "Location:";
            this.LocationComboBox.Location = new System.Drawing.Point(16, 169);
            this.LocationComboBox.Name = "LocationComboBox";
            this.LocationComboBox.SelectedIndex = -1;
            this.LocationComboBox.Size = new System.Drawing.Size(195, 40);
            this.LocationComboBox.TabIndex = 4;
            // 
            // QuantityTextBox
            // 
            this.QuantityTextBox.LabelText = "Quantity:";
            this.QuantityTextBox.Location = new System.Drawing.Point(16, 123);
            this.QuantityTextBox.Name = "QuantityTextBox";
            this.QuantityTextBox.Size = new System.Drawing.Size(73, 40);
            this.QuantityTextBox.TabIndex = 3;
            // 
            // DescriptionTextBox
            // 
            this.DescriptionTextBox.LabelText = "Description:";
            this.DescriptionTextBox.Location = new System.Drawing.Point(16, 77);
            this.DescriptionTextBox.Name = "DescriptionTextBox";
            this.DescriptionTextBox.Size = new System.Drawing.Size(286, 40);
            this.DescriptionTextBox.TabIndex = 2;
            // 
            // UserTextBox
            // 
            this.UserTextBox.LabelText = "User:";
            this.UserTextBox.Location = new System.Drawing.Point(16, 31);
            this.UserTextBox.Name = "UserTextBox";
            this.UserTextBox.Size = new System.Drawing.Size(286, 40);
            this.UserTextBox.TabIndex = 1;
            // 
            // errorProvider
            // 
            this.errorProvider.BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink;
            this.errorProvider.ContainerControl = this;
            // 
            // SibiManageItemForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(762, 425);
            this.Controls.Add(this.ItemGroupBox);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SibiManageItemForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Requisition Item";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SibiManageItemForm_FormClosing);
            this.ItemGroupBox.ResumeLayout(false);
            this.BudgetGroupBox.ResumeLayout(false);
            this.BudgetGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.errorProvider)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ItemGroupBox;
        private CustomControls.LabeledTextBox UserTextBox;
        private CustomControls.LabeledTextBox QuantityTextBox;
        private CustomControls.LabeledTextBox DescriptionTextBox;
        private CustomControls.LabeledComboBox LocationComboBox;
        private CustomControls.LabeledComboBox StatusComboBox;
        private CustomControls.LabeledTextBox NewSerialTextBox;
        private CustomControls.LabeledTextBox NewAssetTextBox;
        private CustomControls.LabeledTextBox ReplaceSerialTextBox;
        private CustomControls.LabeledTextBox ReplaceAssetTextBox;
        private CustomControls.LabeledTextBox ObjectTextBox;
        private CustomControls.LabeledTextBox OrgTextBox;
        private CustomControls.LabeledTextBox ApproverTextBox;
        private System.Windows.Forms.Button FindSupervisorButton;
        private System.Windows.Forms.Button AcceptButton;
        private CustomControls.LabeledTextBox BudgetLineTextBox;
        private System.Windows.Forms.GroupBox BudgetGroupBox;
        private System.Windows.Forms.CheckBox ApprovalRequiredCheckBox;
        private System.Windows.Forms.ErrorProvider errorProvider;
    }
}