using System.Windows.Forms;

namespace AdvancedDialog
{
    partial class Dialog : Form
    {
       
      
        private System.ComponentModel.IContainer components = null;


        private void InitializeComponent()
        {
            this.tblOkCancel = new System.Windows.Forms.TableLayoutPanel();
            this.OK_Button = new System.Windows.Forms.Button();
            this.Cancel_Button = new System.Windows.Forms.Button();
            this.pnlControls = new System.Windows.Forms.FlowLayoutPanel();
            this.tblYesNo = new System.Windows.Forms.TableLayoutPanel();
            this.Yes_Button = new System.Windows.Forms.Button();
            this.No_Button = new System.Windows.Forms.Button();
            this.pnlIcon = new System.Windows.Forms.Panel();
            this.pbIcon = new System.Windows.Forms.PictureBox();
            this.pnlControls_Main = new System.Windows.Forms.Panel();
            this.pnlMaster = new System.Windows.Forms.Panel();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.tblOkCancel.SuspendLayout();
            this.tblYesNo.SuspendLayout();
            this.pnlIcon.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).BeginInit();
            this.pnlControls_Main.SuspendLayout();
            this.pnlMaster.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.SuspendLayout();
            // 
            // tblOkCancel
            // 
            this.tblOkCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.tblOkCancel.AutoSize = true;
            this.tblOkCancel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblOkCancel.ColumnCount = 2;
            this.tblOkCancel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblOkCancel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblOkCancel.Controls.Add(this.OK_Button, 0, 0);
            this.tblOkCancel.Controls.Add(this.Cancel_Button, 1, 0);
            this.tblOkCancel.Location = new System.Drawing.Point(165, 6);
            this.tblOkCancel.Name = "tblOkCancel";
            this.tblOkCancel.RowCount = 1;
            this.tblOkCancel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblOkCancel.Size = new System.Drawing.Size(162, 29);
            this.tblOkCancel.TabIndex = 0;
            // 
            // OK_Button
            // 
            this.OK_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OK_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OK_Button.Location = new System.Drawing.Point(3, 3);
            this.OK_Button.Name = "OK_Button";
            this.OK_Button.Size = new System.Drawing.Size(75, 23);
            this.OK_Button.TabIndex = 0;
            this.OK_Button.Text = "&OK";
            this.OK_Button.Click += new System.EventHandler(this.OK_Button_Click);
            // 
            // Cancel_Button
            // 
            this.Cancel_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Cancel_Button.DialogResult = System.Windows.Forms.DialogResult.No;
            this.Cancel_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Cancel_Button.Location = new System.Drawing.Point(84, 3);
            this.Cancel_Button.Name = "Cancel_Button";
            this.Cancel_Button.Size = new System.Drawing.Size(75, 23);
            this.Cancel_Button.TabIndex = 1;
            this.Cancel_Button.Text = "&Cancel";
            this.Cancel_Button.Click += new System.EventHandler(this.Cancel_Button_Click);
            // 
            // pnlControls
            // 
            this.pnlControls.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlControls.AutoScroll = true;
            this.pnlControls.AutoSize = true;
            this.pnlControls.BackColor = System.Drawing.SystemColors.Control;
            this.pnlControls.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.pnlControls.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlControls.ForeColor = System.Drawing.Color.Black;
            this.pnlControls.Location = new System.Drawing.Point(3, 3);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Padding = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.pnlControls.Size = new System.Drawing.Size(336, 83);
            this.pnlControls.TabIndex = 3;
            // 
            // tblYesNo
            // 
            this.tblYesNo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.tblYesNo.AutoSize = true;
            this.tblYesNo.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tblYesNo.ColumnCount = 2;
            this.tblYesNo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblYesNo.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tblYesNo.Controls.Add(this.Yes_Button, 0, 0);
            this.tblYesNo.Controls.Add(this.No_Button, 1, 0);
            this.tblYesNo.Location = new System.Drawing.Point(3, 6);
            this.tblYesNo.Name = "tblYesNo";
            this.tblYesNo.RowCount = 1;
            this.tblYesNo.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tblYesNo.Size = new System.Drawing.Size(162, 29);
            this.tblYesNo.TabIndex = 4;
            this.tblYesNo.Visible = false;
            // 
            // Yes_Button
            // 
            this.Yes_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.Yes_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Yes_Button.Location = new System.Drawing.Point(3, 3);
            this.Yes_Button.Name = "Yes_Button";
            this.Yes_Button.Size = new System.Drawing.Size(75, 23);
            this.Yes_Button.TabIndex = 0;
            this.Yes_Button.Text = "&Yes";
            this.Yes_Button.Click += new System.EventHandler(this.Yes_Button_Click);
            // 
            // No_Button
            // 
            this.No_Button.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.No_Button.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.No_Button.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.No_Button.Location = new System.Drawing.Point(84, 3);
            this.No_Button.Name = "No_Button";
            this.No_Button.Size = new System.Drawing.Size(75, 23);
            this.No_Button.TabIndex = 1;
            this.No_Button.Text = "&No";
            this.No_Button.Click += new System.EventHandler(this.No_Button_Click);
            // 
            // pnlIcon
            // 
            this.pnlIcon.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlIcon.Controls.Add(this.pbIcon);
            this.pnlIcon.Location = new System.Drawing.Point(345, 3);
            this.pnlIcon.Name = "pnlIcon";
            this.pnlIcon.Size = new System.Drawing.Size(100, 90);
            this.pnlIcon.TabIndex = 6;
            // 
            // pbIcon
            // 
            this.pbIcon.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.pbIcon.BackColor = System.Drawing.SystemColors.Control;
            this.pbIcon.Location = new System.Drawing.Point(17, 10);
            this.pbIcon.MinimumSize = new System.Drawing.Size(65, 65);
            this.pbIcon.Name = "pbIcon";
            this.pbIcon.Size = new System.Drawing.Size(70, 70);
            this.pbIcon.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbIcon.TabIndex = 4;
            this.pbIcon.TabStop = false;
            // 
            // pnlControls_Main
            // 
            this.pnlControls_Main.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlControls_Main.AutoSize = true;
            this.pnlControls_Main.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlControls_Main.BackColor = System.Drawing.SystemColors.Control;
            this.pnlControls_Main.Controls.Add(this.pnlControls);
            this.pnlControls_Main.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.pnlControls_Main.Location = new System.Drawing.Point(3, 4);
            this.pnlControls_Main.MinimumSize = new System.Drawing.Size(330, 90);
            this.pnlControls_Main.Name = "pnlControls_Main";
            this.pnlControls_Main.Size = new System.Drawing.Size(340, 90);
            this.pnlControls_Main.TabIndex = 7;
            // 
            // pnlMaster
            // 
            this.pnlMaster.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlMaster.AutoSize = true;
            this.pnlMaster.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlMaster.Controls.Add(this.pnlButtons);
            this.pnlMaster.Controls.Add(this.pnlControls_Main);
            this.pnlMaster.Controls.Add(this.pnlIcon);
            this.pnlMaster.Location = new System.Drawing.Point(9, 8);
            this.pnlMaster.MinimumSize = new System.Drawing.Size(444, 135);
            this.pnlMaster.Name = "pnlMaster";
            this.pnlMaster.Size = new System.Drawing.Size(444, 137);
            this.pnlMaster.TabIndex = 8;
            // 
            // pnlButtons
            // 
            this.pnlButtons.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.pnlButtons.AutoSize = true;
            this.pnlButtons.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.pnlButtons.Controls.Add(this.tblYesNo);
            this.pnlButtons.Controls.Add(this.tblOkCancel);
            this.pnlButtons.Cursor = System.Windows.Forms.Cursors.Default;
            this.pnlButtons.Location = new System.Drawing.Point(111, 96);
            this.pnlButtons.Name = "pnlButtons";
            this.pnlButtons.Size = new System.Drawing.Size(330, 38);
            this.pnlButtons.TabIndex = 9;
            // 
            // Dialog
            // 
            this.AcceptButton = this.OK_Button;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.CancelButton = this.Cancel_Button;
            this.ClientSize = new System.Drawing.Size(461, 151);
            this.Controls.Add(this.pnlMaster);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1068, 678);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(477, 190);
            this.Name = "Dialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.Dialog_Load);
            this.ResizeBegin += new System.EventHandler(this.Dialog_ResizeBegin);
            this.tblOkCancel.ResumeLayout(false);
            this.tblYesNo.ResumeLayout(false);
            this.pnlIcon.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pbIcon)).EndInit();
            this.pnlControls_Main.ResumeLayout(false);
            this.pnlControls_Main.PerformLayout();
            this.pnlMaster.ResumeLayout(false);
            this.pnlMaster.PerformLayout();
            this.pnlButtons.ResumeLayout(false);
            this.pnlButtons.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        internal System.Windows.Forms.TableLayoutPanel tblOkCancel; /* TODO ERROR didn't convert: WithEvents */
        internal System.Windows.Forms.Button OK_Button; /* TODO ERROR didn't convert: WithEvents */
        internal System.Windows.Forms.Button Cancel_Button; /* TODO ERROR didn't convert: WithEvents */
        internal FlowLayoutPanel pnlControls; /* TODO ERROR didn't convert: WithEvents */
        internal TableLayoutPanel tblYesNo; /* TODO ERROR didn't convert: WithEvents */
        internal Button Yes_Button; /* TODO ERROR didn't convert: WithEvents */
        internal Button No_Button; /* TODO ERROR didn't convert: WithEvents */
        internal PictureBox pbIcon; /* TODO ERROR didn't convert: WithEvents */
        internal Panel pnlIcon; /* TODO ERROR didn't convert: WithEvents */
        internal Panel pnlControls_Main; /* TODO ERROR didn't convert: WithEvents */
        internal Panel pnlMaster; /* TODO ERROR didn't convert: WithEvents */
        internal Panel pnlButtons; /* TODO ERROR didn't convert: WithEvents */
    }
}