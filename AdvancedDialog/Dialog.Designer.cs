namespace AdvancedDialog
{
    partial class Dialog
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.ButtonAreaPanel = new System.Windows.Forms.Panel();
            this.ButtonsPanel = new System.Windows.Forms.Panel();
            this.YesNoPanel = new System.Windows.Forms.TableLayoutPanel();
            this.YesButton = new System.Windows.Forms.Button();
            this.NoButton = new System.Windows.Forms.Button();
            this.OKCancelPanel = new System.Windows.Forms.TableLayoutPanel();
            this.CancelButtonUI = new System.Windows.Forms.Button();
            this.OKButton = new System.Windows.Forms.Button();
            this.IconPanel = new System.Windows.Forms.Panel();
            this.IconBox = new System.Windows.Forms.PictureBox();
            this.MainLayoutTable = new System.Windows.Forms.TableLayoutPanel();
            this.ControlsMainPanel = new System.Windows.Forms.Panel();
            this.ControlsPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.ButtonAreaPanel.SuspendLayout();
            this.ButtonsPanel.SuspendLayout();
            this.YesNoPanel.SuspendLayout();
            this.OKCancelPanel.SuspendLayout();
            this.IconPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.IconBox)).BeginInit();
            this.MainLayoutTable.SuspendLayout();
            this.ControlsMainPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ButtonAreaPanel
            // 
            this.ButtonAreaPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainLayoutTable.SetColumnSpan(this.ButtonAreaPanel, 2);
            this.ButtonAreaPanel.Controls.Add(this.ButtonsPanel);
            this.ButtonAreaPanel.Location = new System.Drawing.Point(3, 109);
            this.ButtonAreaPanel.Name = "ButtonAreaPanel";
            this.ButtonAreaPanel.Size = new System.Drawing.Size(435, 35);
            this.ButtonAreaPanel.TabIndex = 2;
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.ButtonsPanel.AutoSize = true;
            this.ButtonsPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ButtonsPanel.Controls.Add(this.YesNoPanel);
            this.ButtonsPanel.Controls.Add(this.OKCancelPanel);
            this.ButtonsPanel.Location = new System.Drawing.Point(92, 0);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(344, 35);
            this.ButtonsPanel.TabIndex = 6;
            // 
            // YesNoPanel
            // 
            this.YesNoPanel.AutoSize = true;
            this.YesNoPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.YesNoPanel.ColumnCount = 2;
            this.YesNoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.YesNoPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.YesNoPanel.Controls.Add(this.YesButton, 0, 0);
            this.YesNoPanel.Controls.Add(this.NoButton, 1, 0);
            this.YesNoPanel.Location = new System.Drawing.Point(18, 3);
            this.YesNoPanel.Name = "YesNoPanel";
            this.YesNoPanel.RowCount = 1;
            this.YesNoPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.YesNoPanel.Size = new System.Drawing.Size(162, 29);
            this.YesNoPanel.TabIndex = 0;
            this.YesNoPanel.Visible = false;
            // 
            // YesButton
            // 
            this.YesButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.YesButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.YesButton.Location = new System.Drawing.Point(3, 3);
            this.YesButton.Name = "YesButton";
            this.YesButton.Size = new System.Drawing.Size(75, 23);
            this.YesButton.TabIndex = 50;
            this.YesButton.Text = "&Yes";
            this.YesButton.UseVisualStyleBackColor = true;
            this.YesButton.Click += new System.EventHandler(this.YesButton_Click);
            // 
            // NoButton
            // 
            this.NoButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.NoButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NoButton.Location = new System.Drawing.Point(84, 3);
            this.NoButton.Name = "NoButton";
            this.NoButton.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.NoButton.Size = new System.Drawing.Size(75, 23);
            this.NoButton.TabIndex = 51;
            this.NoButton.Text = "&No";
            this.NoButton.UseVisualStyleBackColor = true;
            this.NoButton.Click += new System.EventHandler(this.NoButton_Click);
            // 
            // OKCancelPanel
            // 
            this.OKCancelPanel.AutoSize = true;
            this.OKCancelPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.OKCancelPanel.ColumnCount = 2;
            this.OKCancelPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.OKCancelPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.OKCancelPanel.Controls.Add(this.CancelButtonUI, 0, 0);
            this.OKCancelPanel.Controls.Add(this.OKButton, 0, 0);
            this.OKCancelPanel.Location = new System.Drawing.Point(179, 3);
            this.OKCancelPanel.Name = "OKCancelPanel";
            this.OKCancelPanel.RowCount = 1;
            this.OKCancelPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.OKCancelPanel.Size = new System.Drawing.Size(162, 29);
            this.OKCancelPanel.TabIndex = 1;
            // 
            // CancelButtonUI
            // 
            this.CancelButtonUI.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.CancelButtonUI.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CancelButtonUI.Location = new System.Drawing.Point(84, 3);
            this.CancelButtonUI.Name = "CancelButtonUI";
            this.CancelButtonUI.Size = new System.Drawing.Size(75, 23);
            this.CancelButtonUI.TabIndex = 53;
            this.CancelButtonUI.Text = "&Cancel";
            this.CancelButtonUI.UseVisualStyleBackColor = true;
            this.CancelButtonUI.Click += new System.EventHandler(this.CancelButtonUI_Click);
            // 
            // OKButton
            // 
            this.OKButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.OKButton.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.OKButton.Location = new System.Drawing.Point(3, 3);
            this.OKButton.Name = "OKButton";
            this.OKButton.Size = new System.Drawing.Size(75, 23);
            this.OKButton.TabIndex = 52;
            this.OKButton.Text = "&OK";
            this.OKButton.UseVisualStyleBackColor = true;
            this.OKButton.Click += new System.EventHandler(this.OKButton_Click);
            // 
            // IconPanel
            // 
            this.IconPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.IconPanel.Controls.Add(this.IconBox);
            this.IconPanel.Location = new System.Drawing.Point(350, 3);
            this.IconPanel.Name = "IconPanel";
            this.IconPanel.Size = new System.Drawing.Size(88, 100);
            this.IconPanel.TabIndex = 4;
            // 
            // IconBox
            // 
            this.IconBox.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.IconBox.Location = new System.Drawing.Point(9, 15);
            this.IconBox.Name = "IconBox";
            this.IconBox.Size = new System.Drawing.Size(70, 70);
            this.IconBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.IconBox.TabIndex = 1;
            this.IconBox.TabStop = false;
            // 
            // MainLayoutTable
            // 
            this.MainLayoutTable.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MainLayoutTable.AutoSize = true;
            this.MainLayoutTable.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.MainLayoutTable.ColumnCount = 2;
            this.MainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.MainLayoutTable.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 95F));
            this.MainLayoutTable.Controls.Add(this.ControlsMainPanel, 0, 0);
            this.MainLayoutTable.Controls.Add(this.ButtonAreaPanel, 1, 0);
            this.MainLayoutTable.Controls.Add(this.IconPanel, 1, 0);
            this.MainLayoutTable.Location = new System.Drawing.Point(6, 5);
            this.MainLayoutTable.Name = "MainLayoutTable";
            this.MainLayoutTable.RowCount = 1;
            this.MainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainLayoutTable.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.MainLayoutTable.Size = new System.Drawing.Size(441, 147);
            this.MainLayoutTable.TabIndex = 5;
            // 
            // ControlsMainPanel
            // 
            this.ControlsMainPanel.AutoSize = true;
            this.ControlsMainPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ControlsMainPanel.Controls.Add(this.ControlsPanel);
            this.ControlsMainPanel.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ControlsMainPanel.Location = new System.Drawing.Point(3, 3);
            this.ControlsMainPanel.Name = "ControlsMainPanel";
            this.ControlsMainPanel.Size = new System.Drawing.Size(340, 100);
            this.ControlsMainPanel.TabIndex = 6;
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
            this.ControlsPanel.Size = new System.Drawing.Size(327, 79);
            this.ControlsPanel.TabIndex = 4;
            // 
            // Dialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.ClientSize = new System.Drawing.Size(453, 158);
            this.Controls.Add(this.MainLayoutTable);
            this.DoubleBuffered = true;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(1041, 791);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(469, 197);
            this.Name = "Dialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Dialog";
            this.Load += new System.EventHandler(this.Dialog_Load);
            this.Shown += new System.EventHandler(this.Dialog_Shown);
            this.ButtonAreaPanel.ResumeLayout(false);
            this.ButtonAreaPanel.PerformLayout();
            this.ButtonsPanel.ResumeLayout(false);
            this.ButtonsPanel.PerformLayout();
            this.YesNoPanel.ResumeLayout(false);
            this.OKCancelPanel.ResumeLayout(false);
            this.IconPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.IconBox)).EndInit();
            this.MainLayoutTable.ResumeLayout(false);
            this.MainLayoutTable.PerformLayout();
            this.ControlsMainPanel.ResumeLayout(false);
            this.ControlsMainPanel.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Panel ButtonAreaPanel;
        private System.Windows.Forms.Button NoButton;
        private System.Windows.Forms.Button YesButton;
        private System.Windows.Forms.Panel IconPanel;
        private System.Windows.Forms.PictureBox IconBox;
        private System.Windows.Forms.TableLayoutPanel MainLayoutTable;
        private System.Windows.Forms.Panel ControlsMainPanel;
        private System.Windows.Forms.Panel ButtonsPanel;
        private System.Windows.Forms.TableLayoutPanel YesNoPanel;
        private System.Windows.Forms.TableLayoutPanel OKCancelPanel;
        private System.Windows.Forms.Button CancelButtonUI;
        private System.Windows.Forms.Button OKButton;
        internal System.Windows.Forms.FlowLayoutPanel ControlsPanel;
    }
}