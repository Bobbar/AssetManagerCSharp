using System.Windows.Forms;

namespace AdvancedDialog
{
    partial class Dialog : Form
    {
        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.OkCancelPanel = new System.Windows.Forms.TableLayoutPanel();
            this.OKButton = new System.Windows.Forms.Button();
            this.CancelButtonUI = new System.Windows.Forms.Button();
            this.ControlsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.YesNoPanel = new System.Windows.Forms.TableLayoutPanel();
            this.YesButton = new System.Windows.Forms.Button();
            this.NoButton = new System.Windows.Forms.Button();
            this.IconPanel = new System.Windows.Forms.Panel();
            this.IconBox = new System.Windows.Forms.PictureBox();
            this.ControlsMainPanel = new System.Windows.Forms.Panel();
            this.MasterPanel = new System.Windows.Forms.Panel();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.OkCancelPanel.SuspendLayout();
            this.YesNoPanel.SuspendLayout();
            this.IconPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IconBox)).BeginInit();
            this.ControlsMainPanel.SuspendLayout();
            this.MasterPanel.SuspendLayout();
            this.ButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // OkCancelPanel
            // 
            this.OkCancelPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.OkCancelPanel.AutoSize = true;
            this.OkCancelPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.OkCancelPanel.ColumnCount = 2;
            this.OkCancelPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.OkCancelPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.OkCancelPanel.Controls.Add(this.OKButton, 0, 0);
            this.OkCancelPanel.Controls.Add(this.CancelButtonUI, 1, 0);
            this.OkCancelPanel.Location = new System.Drawing.Point(165, 6);
            this.OkCancelPanel.Name = "OkCancelPanel";
            this.OkCancelPanel.RowCount = 1;
            this.OkCancelPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OkCancelPanel.Size = new System.Drawing.Size(162, 29);
            this.OkCancelPanel.TabIndex = 0;
            // 
            // OKButton
            // 
            this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OKButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKButton.Location = new System.Drawing.Point(3, 3);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 0;
            this.OKButton.Text = "&OK";
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // CancelButtonUI
            // 
            this.CancelButtonUI.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelButtonUI.DialogResult = System.Windows.Forms.DialogResult.No;
            this.CancelButtonUI.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButtonUI.Location = new System.Drawing.Point(84, 3);
            this.CancelButtonUI.Name = "CancelButtonUI";
            this.CancelButtonUI.Size = new System.Drawing.Size(75, 23);
            this.CancelButtonUI.TabIndex = 1;
            this.CancelButtonUI.Text = "&Cancel";
            this.CancelButtonUI.Click += new System.EventHandler(this.CancelButtonUI_Click);
            // 
            // ControlsPanel
            // 
            this.ControlsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlsPanel.AutoScroll = true;
            this.ControlsPanel.AutoSize = true;
            this.ControlsPanel.BackColor = System.Drawing.SystemColors.Control;
            this.ControlsPanel.FlowDirection = System.Windows.Forms.FlowDirection.TopDown;
            this.ControlsPanel.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ControlsPanel.ForeColor = System.Drawing.Color.Black;
            this.ControlsPanel.Location = new System.Drawing.Point(3, 3);
            this.ControlsPanel.Name = "ControlsPanel";
            this.ControlsPanel.Padding = new System.Windows.Forms.Padding(10, 10, 10, 0);
            this.ControlsPanel.Size = new System.Drawing.Size(336, 83);
            this.ControlsPanel.TabIndex = 3;
            // 
            // YesNoPanel
            // 
            this.YesNoPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.YesNoPanel.AutoSize = true;
            this.YesNoPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.YesNoPanel.ColumnCount = 2;
            this.YesNoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.YesNoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.YesNoPanel.Controls.Add(this.YesButton, 0, 0);
            this.YesNoPanel.Controls.Add(this.NoButton, 1, 0);
            this.YesNoPanel.Location = new System.Drawing.Point(3, 6);
            this.YesNoPanel.Name = "YesNoPanel";
            this.YesNoPanel.RowCount = 1;
            this.YesNoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.YesNoPanel.Size = new System.Drawing.Size(162, 29);
            this.YesNoPanel.TabIndex = 4;
            this.YesNoPanel.Visible = false;
            // 
            // YesButton
            // 
            this.YesButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.YesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.YesButton.Location = new System.Drawing.Point(3, 3);
            this.YesButton.Name = "YesButton";
            this.YesButton.Size = new System.Drawing.Size(75, 23);
            this.YesButton.TabIndex = 0;
            this.YesButton.Text = "&Yes";
            this.YesButton.Click += new System.EventHandler(this.YesButton_Click);
            // 
            // NoButton
            // 
            this.NoButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NoButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.NoButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NoButton.Location = new System.Drawing.Point(84, 3);
            this.NoButton.Name = "NoButton";
            this.NoButton.Size = new System.Drawing.Size(75, 23);
            this.NoButton.TabIndex = 1;
            this.NoButton.Text = "&No";
            this.NoButton.Click += new System.EventHandler(this.NoButton_Click);
            // 
            // IconPanel
            // 
            this.IconPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IconPanel.Controls.Add(this.IconBox);
            this.IconPanel.Location = new System.Drawing.Point(347, 3);
            this.IconPanel.Name = "IconPanel";
            this.IconPanel.Size = new System.Drawing.Size(100, 92);
            this.IconPanel.TabIndex = 6;
            // 
            // IconBox
            // 
            this.IconBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.IconBox.BackColor = System.Drawing.SystemColors.Control;
            this.IconBox.Location = new System.Drawing.Point(17, 11);
            this.IconBox.MinimumSize = new System.Drawing.Size(65, 65);
            this.IconBox.Name = "IconBox";
            this.IconBox.Size = new System.Drawing.Size(70, 70);
            this.IconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.IconBox.TabIndex = 4;
            this.IconBox.TabStop = false;
            // 
            // ControlsMainPanel
            // 
            this.ControlsMainPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ControlsMainPanel.AutoSize = true;
            this.ControlsMainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ControlsMainPanel.BackColor = System.Drawing.SystemColors.Control;
            this.ControlsMainPanel.Controls.Add(this.ControlsPanel);
            this.ControlsMainPanel.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ControlsMainPanel.Location = new System.Drawing.Point(3, 4);
            this.ControlsMainPanel.MinimumSize = new System.Drawing.Size(330, 90);
            this.ControlsMainPanel.Name = "ControlsMainPanel";
            this.ControlsMainPanel.Size = new System.Drawing.Size(340, 90);
            this.ControlsMainPanel.TabIndex = 7;
            // 
            // MasterPanel
            // 
            this.MasterPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MasterPanel.AutoSize = true;
            this.MasterPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MasterPanel.Controls.Add(this.ButtonsPanel);
            this.MasterPanel.Controls.Add(this.ControlsMainPanel);
            this.MasterPanel.Controls.Add(this.IconPanel);
            this.MasterPanel.Location = new System.Drawing.Point(6, 8);
            this.MasterPanel.MinimumSize = new System.Drawing.Size(444, 135);
            this.MasterPanel.Name = "MasterPanel";
            this.MasterPanel.Size = new System.Drawing.Size(446, 139);
            this.MasterPanel.TabIndex = 8;
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonsPanel.AutoSize = true;
            this.ButtonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonsPanel.Controls.Add(this.YesNoPanel);
            this.ButtonsPanel.Controls.Add(this.OkCancelPanel);
            this.ButtonsPanel.Cursor = System.Windows.Forms.Cursors.Default;
            this.ButtonsPanel.Location = new System.Drawing.Point(113, 98);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(330, 38);
            this.ButtonsPanel.TabIndex = 9;
            // 
            // Dialog
            // 
            this.AcceptButton = this.OKButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(461, 151);
            this.Controls.Add(this.MasterPanel);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1068, 678);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(477, 190);
            this.Name = "Dialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dialog Prompt";
            this.Load += new System.EventHandler(this.Dialog_Load);
            this.OkCancelPanel.ResumeLayout(false);
            this.YesNoPanel.ResumeLayout(false);
            this.IconPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.IconBox)).EndInit();
            this.ControlsMainPanel.ResumeLayout(false);
            this.ControlsMainPanel.PerformLayout();
            this.MasterPanel.ResumeLayout(false);
            this.MasterPanel.PerformLayout();
            this.ButtonsPanel.ResumeLayout(false);
            this.ButtonsPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        internal System.Windows.Forms.TableLayoutPanel OkCancelPanel;
        internal System.Windows.Forms.Button OKButton;
        internal System.Windows.Forms.Button CancelButtonUI;
        internal FlowLayoutPanel ControlsPanel;
        internal TableLayoutPanel YesNoPanel;
        internal Button YesButton;
        internal Button NoButton;
        internal PictureBox IconBox;
        internal Panel IconPanel;
        internal Panel ControlsMainPanel;
        internal Panel MasterPanel;
        internal Panel ButtonsPanel;
    }
}