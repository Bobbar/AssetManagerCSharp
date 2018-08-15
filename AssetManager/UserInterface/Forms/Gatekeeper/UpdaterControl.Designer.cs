using System.Windows.Forms;
namespace AssetManager.UserInterface.Forms.Gatekeeper
{

    partial class UpdaterControl : System.Windows.Forms.UserControl
    {




        //Required by the Windows Form Designer

        private System.ComponentModel.IContainer components = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.StatusLabel = new System.Windows.Forms.Label();
            this.DeviceInfoLabel = new System.Windows.Forms.Label();
            this.LogTextBox = new System.Windows.Forms.RichTextBox();
            this.ShowHideLabel = new System.Windows.Forms.Label();
            this.ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.RestartPictureBox = new System.Windows.Forms.PictureBox();
            this.CancelClosePictureBox = new System.Windows.Forms.PictureBox();
            this.SequenceLabel = new System.Windows.Forms.Label();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.StatusPictureBox = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.RestartPictureBox)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.CancelClosePictureBox)).BeginInit();
            this.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StatusPictureBox)).BeginInit();
            this.SuspendLayout();
            // 
            // StatusLabel
            // 
            this.StatusLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.StatusLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StatusLabel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.StatusLabel.Location = new System.Drawing.Point(4, 33);
            this.StatusLabel.Name = "StatusLabel";
            this.StatusLabel.Size = new System.Drawing.Size(490, 22);
            this.StatusLabel.TabIndex = 1;
            this.StatusLabel.Text = "[Status/File]";
            this.StatusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // DeviceInfoLabel
            // 
            this.DeviceInfoLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DeviceInfoLabel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.DeviceInfoLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.DeviceInfoLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.DeviceInfoLabel.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.DeviceInfoLabel.ForeColor = System.Drawing.Color.White;
            this.DeviceInfoLabel.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.DeviceInfoLabel.Location = new System.Drawing.Point(75, 4);
            this.DeviceInfoLabel.Name = "DeviceInfoLabel";
            this.DeviceInfoLabel.Size = new System.Drawing.Size(354, 17);
            this.DeviceInfoLabel.TabIndex = 2;
            this.DeviceInfoLabel.Text = "[Computer Info]";
            this.DeviceInfoLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.ToolTip.SetToolTip(this.DeviceInfoLabel, "Click to view device.");
            this.DeviceInfoLabel.Click += new System.EventHandler(this.InfoLabel_Click);
            // 
            // LogTextBox
            // 
            this.LogTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.LogTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.LogTextBox.DetectUrls = false;
            this.LogTextBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LogTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.LogTextBox.HideSelection = false;
            this.LogTextBox.Location = new System.Drawing.Point(4, 71);
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.Size = new System.Drawing.Size(491, 224);
            this.LogTextBox.TabIndex = 4;
            this.LogTextBox.Text = "";
            // 
            // ShowHideLabel
            // 
            this.ShowHideLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowHideLabel.AutoSize = true;
            this.ShowHideLabel.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ShowHideLabel.Font = new System.Drawing.Font("Wingdings 3", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.ShowHideLabel.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.ShowHideLabel.Location = new System.Drawing.Point(474, 56);
            this.ShowHideLabel.Name = "ShowHideLabel";
            this.ShowHideLabel.Size = new System.Drawing.Size(17, 12);
            this.ShowHideLabel.TabIndex = 5;
            this.ShowHideLabel.Text = "s";
            this.ToolTip.SetToolTip(this.ShowHideLabel, "Show/Hide Log");
            this.ShowHideLabel.Click += new System.EventHandler(this.ShowHideLabel_Click);
            // 
            // RestartPictureBox
            // 
            this.RestartPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.RestartPictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.RestartPictureBox.Image = global::AssetManager.Properties.Resources.RestartIcon;
            this.RestartPictureBox.Location = new System.Drawing.Point(450, 2);
            this.RestartPictureBox.Name = "RestartPictureBox";
            this.RestartPictureBox.Size = new System.Drawing.Size(20, 20);
            this.RestartPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RestartPictureBox.TabIndex = 7;
            this.RestartPictureBox.TabStop = false;
            this.ToolTip.SetToolTip(this.RestartPictureBox, "Start/Restart Update");
            this.RestartPictureBox.Click += new System.EventHandler(this.pbRestart_Click);
            // 
            // CancelClosePictureBox
            // 
            this.CancelClosePictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.CancelClosePictureBox.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CancelClosePictureBox.Image = global::AssetManager.Properties.Resources.CloseCancelDeleteIcon;
            this.CancelClosePictureBox.Location = new System.Drawing.Point(476, 2);
            this.CancelClosePictureBox.Name = "CancelClosePictureBox";
            this.CancelClosePictureBox.Size = new System.Drawing.Size(20, 20);
            this.CancelClosePictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.CancelClosePictureBox.TabIndex = 3;
            this.CancelClosePictureBox.TabStop = false;
            this.ToolTip.SetToolTip(this.CancelClosePictureBox, "Cancel/Close");
            this.CancelClosePictureBox.Click += new System.EventHandler(this.pbCancelClose_Click);
            // 
            // SequenceLabel
            // 
            this.SequenceLabel.AutoSize = true;
            this.SequenceLabel.Location = new System.Drawing.Point(1, 2);
            this.SequenceLabel.Name = "SequenceLabel";
            this.SequenceLabel.Size = new System.Drawing.Size(42, 14);
            this.SequenceLabel.TabIndex = 6;
            this.SequenceLabel.Text = "[Seq]";
            // 
            // Panel1
            // 
            this.Panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Panel1.BackColor = System.Drawing.Color.Silver;
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel1.Controls.Add(this.StatusPictureBox);
            this.Panel1.Controls.Add(this.SequenceLabel);
            this.Panel1.Controls.Add(this.RestartPictureBox);
            this.Panel1.Controls.Add(this.ShowHideLabel);
            this.Panel1.Controls.Add(this.CancelClosePictureBox);
            this.Panel1.Controls.Add(this.LogTextBox);
            this.Panel1.Controls.Add(this.DeviceInfoLabel);
            this.Panel1.Controls.Add(this.StatusLabel);
            this.Panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.Panel1.Location = new System.Drawing.Point(0, 0);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(500, 300);
            this.Panel1.TabIndex = 8;
            // 
            // StatusPictureBox
            // 
            this.StatusPictureBox.Location = new System.Drawing.Point(7, 27);
            this.StatusPictureBox.Name = "StatusPictureBox";
            this.StatusPictureBox.Size = new System.Drawing.Size(37, 30);
            this.StatusPictureBox.TabIndex = 8;
            this.StatusPictureBox.TabStop = false;
            // 
            // UpdaterControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.Panel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximumSize = new System.Drawing.Size(500, 300);
            this.MinimumSize = new System.Drawing.Size(500, 75);
            this.Name = "UpdaterControl";
            this.Size = new System.Drawing.Size(500, 300);
            ((System.ComponentModel.ISupportInitialize)(this.RestartPictureBox)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.CancelClosePictureBox)).EndInit();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.StatusPictureBox)).EndInit();
            this.ResumeLayout(false);

        }
        internal Label StatusLabel;
        internal Label DeviceInfoLabel;
        internal PictureBox CancelClosePictureBox;
        internal RichTextBox LogTextBox;
        internal Label ShowHideLabel;
        internal ToolTip ToolTip;
        internal Label SequenceLabel;
        internal PictureBox RestartPictureBox;
        internal Panel Panel1;
        internal PictureBox StatusPictureBox;
    }
}
