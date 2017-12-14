namespace AssetManager.UserInterface.CustomControls
{
    partial class LabeledComboBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.label = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // comboBox
            // 
            this.comboBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(0, 19);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(140, 21);
            this.comboBox.TabIndex = 0;
            // 
            // label
            // 
            this.label.AutoSize = true;
            this.label.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.label.Location = new System.Drawing.Point(0, 3);
            this.label.Name = "label";
            this.label.Padding = new System.Windows.Forms.Padding(0, 0, 0, 3);
            this.label.Size = new System.Drawing.Size(35, 16);
            this.label.TabIndex = 1;
            this.label.Text = "label1";
            // 
            // LabeledComboBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label);
            this.Controls.Add(this.comboBox);
            this.Name = "LabeledComboBox";
            this.Size = new System.Drawing.Size(140, 40);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.Label label;
    }
}
