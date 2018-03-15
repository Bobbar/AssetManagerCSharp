using System.Windows.Forms;
namespace AssetManager.UserInterface.Forms.Sibi
{

    partial class SibiNotesForm
    {
        //Required by the Windows Form Designer
        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.

        private void InitializeComponent()
        {
            this.NotesTextBox = new System.Windows.Forms.RichTextBox();
            this.CloseButton = new System.Windows.Forms.Button();
            this.OkButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // NotesTextBox
            // 
            this.NotesTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.NotesTextBox.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.NotesTextBox.Location = new System.Drawing.Point(12, 12);
            this.NotesTextBox.Name = "NotesTextBox";
            this.NotesTextBox.Size = new System.Drawing.Size(448, 216);
            this.NotesTextBox.TabIndex = 0;
            this.NotesTextBox.Text = "";
            this.NotesTextBox.LinkClicked += new System.Windows.Forms.LinkClickedEventHandler(this.NotesTextBox_LinkClicked);
            // 
            // CloseButton
            // 
            this.CloseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.CloseButton.Location = new System.Drawing.Point(12, 234);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(75, 30);
            this.CloseButton.TabIndex = 1;
            this.CloseButton.Text = "Close";
            this.CloseButton.UseVisualStyleBackColor = true;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // OkButton
            // 
            this.OkButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.OkButton.Location = new System.Drawing.Point(334, 234);
            this.OkButton.Name = "OkButton";
            this.OkButton.Size = new System.Drawing.Size(126, 30);
            this.OkButton.TabIndex = 2;
            this.OkButton.Text = "OK";
            this.OkButton.UseVisualStyleBackColor = true;
            this.OkButton.Click += new System.EventHandler(this.OkButton_Click);
            // 
            // SibiNotesForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(472, 276);
            this.Controls.Add(this.OkButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.NotesTextBox);
            this.MinimumSize = new System.Drawing.Size(253, 160);
            this.Name = "SibiNotesForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Sibi Note";
            this.ResumeLayout(false);

        }
        internal RichTextBox NotesTextBox;
        internal Button CloseButton;
        internal Button OkButton;
    }
}
