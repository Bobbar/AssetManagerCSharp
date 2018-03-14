using System.Windows.Forms;
namespace AssetManager.UserInterface.Forms
{

    partial class GetCredentialsForm
    {
        //Required by the Windows Form Designer
        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.

        private void InitializeComponent()
        {
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.CredDescriptionLabel = new System.Windows.Forms.Label();
            this.AcceptCredsButton = new System.Windows.Forms.Button();
            this.Label2 = new System.Windows.Forms.Label();
            this.Label1 = new System.Windows.Forms.Label();
            this.PasswordTextBox = new System.Windows.Forms.TextBox();
            this.UsernameTextBox = new System.Windows.Forms.TextBox();
            this.GroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.Controls.Add(this.CredDescriptionLabel);
            this.GroupBox1.Controls.Add(this.AcceptCredsButton);
            this.GroupBox1.Controls.Add(this.Label2);
            this.GroupBox1.Controls.Add(this.Label1);
            this.GroupBox1.Controls.Add(this.PasswordTextBox);
            this.GroupBox1.Controls.Add(this.UsernameTextBox);
            this.GroupBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox1.Location = new System.Drawing.Point(7, 9);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(279, 224);
            this.GroupBox1.TabIndex = 0;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Enter Credentials";
            // 
            // CredDescriptionLabel
            // 
            this.CredDescriptionLabel.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CredDescriptionLabel.ForeColor = System.Drawing.Color.Gray;
            this.CredDescriptionLabel.Location = new System.Drawing.Point(2, 19);
            this.CredDescriptionLabel.Name = "CredDescriptionLabel";
            this.CredDescriptionLabel.Size = new System.Drawing.Size(274, 19);
            this.CredDescriptionLabel.TabIndex = 5;
            this.CredDescriptionLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmdAccept
            // 
            this.AcceptCredsButton.Location = new System.Drawing.Point(36, 161);
            this.AcceptCredsButton.Name = "cmdAccept";
            this.AcceptCredsButton.Size = new System.Drawing.Size(199, 36);
            this.AcceptCredsButton.TabIndex = 4;
            this.AcceptCredsButton.Text = "Accept";
            this.AcceptCredsButton.UseVisualStyleBackColor = true;
            this.AcceptCredsButton.Click += new System.EventHandler(this.AcceptCredsButton_Click);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(33, 99);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(63, 15);
            this.Label2.TabIndex = 3;
            this.Label2.Text = "Password";
            this.Label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(33, 42);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(63, 15);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "Username";
            this.Label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtPassword
            // 
            this.PasswordTextBox.Location = new System.Drawing.Point(35, 117);
            this.PasswordTextBox.Name = "txtPassword";
            this.PasswordTextBox.Size = new System.Drawing.Size(200, 23);
            this.PasswordTextBox.TabIndex = 1;
            this.PasswordTextBox.UseSystemPasswordChar = true;
            this.PasswordTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.PasswordTextBox_KeyDown);
            this.PasswordTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.PasswordTextBox_KeyPress);
            this.PasswordTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.PasswordTextBox_KeyUp);
            // 
            // txtUsername
            // 
            this.UsernameTextBox.Location = new System.Drawing.Point(35, 62);
            this.UsernameTextBox.Name = "txtUsername";
            this.UsernameTextBox.Size = new System.Drawing.Size(200, 23);
            this.UsernameTextBox.TabIndex = 0;
            // 
            // GetCredentialsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(295, 245);
            this.Controls.Add(this.GroupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GetCredentialsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "LA Credentials";
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        internal GroupBox GroupBox1;
        internal Button AcceptCredsButton;
        internal Label Label2;
        internal Label Label1;
        internal TextBox PasswordTextBox;
        internal TextBox UsernameTextBox;
        internal Label CredDescriptionLabel;
    }
}
