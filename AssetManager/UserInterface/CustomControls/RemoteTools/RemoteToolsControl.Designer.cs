namespace AssetManager.UserInterface.CustomControls
{
    partial class RemoteToolsControl
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.ToolsGroupBox = new System.Windows.Forms.GroupBox();
            this.ShowIPButton = new System.Windows.Forms.Button();
            this.ToolsLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.RestartDeviceButton = new System.Windows.Forms.PictureBox();
            this.BrowseFilesButton = new System.Windows.Forms.Button();
            this.StartRDPButton = new System.Windows.Forms.Button();
            this.EventViewerButton = new System.Windows.Forms.Button();
            this.PowerShellButton = new System.Windows.Forms.Button();
            this.PSExecButton = new System.Windows.Forms.Button();
            this.GKUpdateButton = new System.Windows.Forms.Button();
            this.DeployTVButton = new System.Windows.Forms.Button();
            this.UpdateChromeButton = new System.Windows.Forms.Button();
            this.DeployOfficeButton = new System.Windows.Forms.Button();
            this.NewDeployButton = new System.Windows.Forms.Button();
            this.ToolsGroupBox.SuspendLayout();
            this.ToolsLayoutPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RestartDeviceButton)).BeginInit();
            this.SuspendLayout();
            // 
            // ToolsGroupBox
            // 
            this.ToolsGroupBox.Controls.Add(this.ShowIPButton);
            this.ToolsGroupBox.Controls.Add(this.ToolsLayoutPanel);
            this.ToolsGroupBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.ToolsGroupBox.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ToolsGroupBox.Location = new System.Drawing.Point(0, 0);
            this.ToolsGroupBox.Name = "ToolsGroupBox";
            this.ToolsGroupBox.Size = new System.Drawing.Size(421, 108);
            this.ToolsGroupBox.TabIndex = 0;
            this.ToolsGroupBox.TabStop = false;
            this.ToolsGroupBox.Text = "Remote Tools";
            // 
            // ShowIPButton
            // 
            this.ShowIPButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.ShowIPButton.BackColor = System.Drawing.Color.Black;
            this.ShowIPButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.ShowIPButton.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.ShowIPButton.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ShowIPButton.ForeColor = System.Drawing.Color.White;
            this.ShowIPButton.Location = new System.Drawing.Point(329, 10);
            this.ShowIPButton.Name = "ShowIPButton";
            this.ShowIPButton.Size = new System.Drawing.Size(90, 90);
            this.ShowIPButton.TabIndex = 59;
            this.ShowIPButton.TextAlign = System.Drawing.ContentAlignment.TopRight;
            this.ShowIPButton.UseVisualStyleBackColor = false;
            this.ShowIPButton.Click += new System.EventHandler(this.ShowIPButton_Click);
            // 
            // ToolsLayoutPanel
            // 
            this.ToolsLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right)));
            this.ToolsLayoutPanel.AutoScroll = true;
            this.ToolsLayoutPanel.Controls.Add(this.RestartDeviceButton);
            this.ToolsLayoutPanel.Controls.Add(this.BrowseFilesButton);
            this.ToolsLayoutPanel.Controls.Add(this.StartRDPButton);
            this.ToolsLayoutPanel.Controls.Add(this.EventViewerButton);
            this.ToolsLayoutPanel.Controls.Add(this.PowerShellButton);
            this.ToolsLayoutPanel.Controls.Add(this.PSExecButton);
            this.ToolsLayoutPanel.Controls.Add(this.GKUpdateButton);
            this.ToolsLayoutPanel.Controls.Add(this.DeployTVButton);
            this.ToolsLayoutPanel.Controls.Add(this.UpdateChromeButton);
            this.ToolsLayoutPanel.Controls.Add(this.DeployOfficeButton);
            this.ToolsLayoutPanel.Controls.Add(this.NewDeployButton);
            this.ToolsLayoutPanel.Location = new System.Drawing.Point(5, 14);
            this.ToolsLayoutPanel.Margin = new System.Windows.Forms.Padding(0);
            this.ToolsLayoutPanel.Name = "ToolsLayoutPanel";
            this.ToolsLayoutPanel.Size = new System.Drawing.Size(320, 86);
            this.ToolsLayoutPanel.TabIndex = 58;
            // 
            // RestartDeviceButton
            // 
            this.RestartDeviceButton.BackColor = System.Drawing.SystemColors.ControlLight;
            this.RestartDeviceButton.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.RestartDeviceButton.Image = global::AssetManager.Properties.Resources.RestartIcon;
            this.RestartDeviceButton.Location = new System.Drawing.Point(1, 1);
            this.RestartDeviceButton.Margin = new System.Windows.Forms.Padding(1);
            this.RestartDeviceButton.Name = "RestartDeviceButton";
            this.RestartDeviceButton.Size = new System.Drawing.Size(45, 45);
            this.RestartDeviceButton.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.RestartDeviceButton.TabIndex = 56;
            this.RestartDeviceButton.TabStop = false;
            this.toolTip.SetToolTip(this.RestartDeviceButton, "Restart Computer");
            this.RestartDeviceButton.Click += new System.EventHandler(this.RestartDeviceButton_Click);
            // 
            // BrowseFilesButton
            // 
            this.BrowseFilesButton.BackgroundImage = global::AssetManager.Properties.Resources.FolderIcon;
            this.BrowseFilesButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.BrowseFilesButton.Location = new System.Drawing.Point(48, 1);
            this.BrowseFilesButton.Margin = new System.Windows.Forms.Padding(1);
            this.BrowseFilesButton.Name = "BrowseFilesButton";
            this.BrowseFilesButton.Size = new System.Drawing.Size(45, 45);
            this.BrowseFilesButton.TabIndex = 52;
            this.toolTip.SetToolTip(this.BrowseFilesButton, "Browse Files");
            this.BrowseFilesButton.UseVisualStyleBackColor = true;
            this.BrowseFilesButton.Click += new System.EventHandler(this.BrowseFilesButton_Click);
            // 
            // StartRDPButton
            // 
            this.StartRDPButton.BackgroundImage = global::AssetManager.Properties.Resources.RDPIcon;
            this.StartRDPButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.StartRDPButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.StartRDPButton.Location = new System.Drawing.Point(95, 1);
            this.StartRDPButton.Margin = new System.Windows.Forms.Padding(1);
            this.StartRDPButton.Name = "StartRDPButton";
            this.StartRDPButton.Size = new System.Drawing.Size(45, 45);
            this.StartRDPButton.TabIndex = 46;
            this.toolTip.SetToolTip(this.StartRDPButton, "Start Remote Desktop");
            this.StartRDPButton.UseVisualStyleBackColor = true;
            this.StartRDPButton.Click += new System.EventHandler(this.StartRDPButton_Click);
            // 
            // EventViewerButton
            // 
            this.EventViewerButton.BackgroundImage = global::AssetManager.Properties.Resources.EventIcon;
            this.EventViewerButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.EventViewerButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.EventViewerButton.Location = new System.Drawing.Point(142, 1);
            this.EventViewerButton.Margin = new System.Windows.Forms.Padding(1);
            this.EventViewerButton.Name = "EventViewerButton";
            this.EventViewerButton.Size = new System.Drawing.Size(45, 45);
            this.EventViewerButton.TabIndex = 59;
            this.toolTip.SetToolTip(this.EventViewerButton, "View Event Logs");
            this.EventViewerButton.UseVisualStyleBackColor = true;
            this.EventViewerButton.Click += new System.EventHandler(this.EventViewerButton_Click);
            // 
            // PowerShellButton
            // 
            this.PowerShellButton.BackgroundImage = global::AssetManager.Properties.Resources.PowerShellIcon2;
            this.PowerShellButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PowerShellButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.PowerShellButton.Location = new System.Drawing.Point(189, 1);
            this.PowerShellButton.Margin = new System.Windows.Forms.Padding(1);
            this.PowerShellButton.Name = "PowerShellButton";
            this.PowerShellButton.Size = new System.Drawing.Size(45, 45);
            this.PowerShellButton.TabIndex = 62;
            this.toolTip.SetToolTip(this.PowerShellButton, "Start Remote PowerShell");
            this.PowerShellButton.UseVisualStyleBackColor = true;
            this.PowerShellButton.Click += new System.EventHandler(this.PowerShellButton_Click);
            // 
            // PSExecButton
            // 
            this.PSExecButton.BackgroundImage = global::AssetManager.Properties.Resources.PsExecIcon;
            this.PSExecButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.PSExecButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.PSExecButton.Location = new System.Drawing.Point(236, 1);
            this.PSExecButton.Margin = new System.Windows.Forms.Padding(1);
            this.PSExecButton.Name = "PSExecButton";
            this.PSExecButton.Size = new System.Drawing.Size(45, 45);
            this.PSExecButton.TabIndex = 63;
            this.toolTip.SetToolTip(this.PSExecButton, "Execute PSExec Commands");
            this.PSExecButton.UseVisualStyleBackColor = true;
            this.PSExecButton.Click += new System.EventHandler(this.PSExecButton_Click);
            // 
            // GKUpdateButton
            // 
            this.GKUpdateButton.BackgroundImage = global::AssetManager.Properties.Resources.GK__UpdateIcon;
            this.GKUpdateButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.GKUpdateButton.Location = new System.Drawing.Point(1, 48);
            this.GKUpdateButton.Margin = new System.Windows.Forms.Padding(1);
            this.GKUpdateButton.Name = "GKUpdateButton";
            this.GKUpdateButton.Size = new System.Drawing.Size(45, 45);
            this.GKUpdateButton.TabIndex = 55;
            this.toolTip.SetToolTip(this.GKUpdateButton, "Enqueue GK Update");
            this.GKUpdateButton.UseVisualStyleBackColor = true;
            this.GKUpdateButton.Click += new System.EventHandler(this.GKUpdateButton_Click);
            // 
            // DeployTVButton
            // 
            this.DeployTVButton.BackgroundImage = global::AssetManager.Properties.Resources.TeamViewerIcon;
            this.DeployTVButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.DeployTVButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DeployTVButton.Location = new System.Drawing.Point(48, 48);
            this.DeployTVButton.Margin = new System.Windows.Forms.Padding(1);
            this.DeployTVButton.Name = "DeployTVButton";
            this.DeployTVButton.Size = new System.Drawing.Size(45, 45);
            this.DeployTVButton.TabIndex = 57;
            this.toolTip.SetToolTip(this.DeployTVButton, "Deploy TeamViewer");
            this.DeployTVButton.UseVisualStyleBackColor = true;
            this.DeployTVButton.Click += new System.EventHandler(this.DeployTVButton_Click);
            // 
            // UpdateChromeButton
            // 
            this.UpdateChromeButton.BackgroundImage = global::AssetManager.Properties.Resources.ChromeIcon;
            this.UpdateChromeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.UpdateChromeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.UpdateChromeButton.Location = new System.Drawing.Point(95, 48);
            this.UpdateChromeButton.Margin = new System.Windows.Forms.Padding(1);
            this.UpdateChromeButton.Name = "UpdateChromeButton";
            this.UpdateChromeButton.Size = new System.Drawing.Size(45, 45);
            this.UpdateChromeButton.TabIndex = 58;
            this.toolTip.SetToolTip(this.UpdateChromeButton, "Update/Install Chrome");
            this.UpdateChromeButton.UseVisualStyleBackColor = true;
            this.UpdateChromeButton.Click += new System.EventHandler(this.UpdateChromeButton_Click);
            // 
            // DeployOfficeButton
            // 
            this.DeployOfficeButton.BackgroundImage = global::AssetManager.Properties.Resources.O365SquareLogo;
            this.DeployOfficeButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.DeployOfficeButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.DeployOfficeButton.Location = new System.Drawing.Point(142, 48);
            this.DeployOfficeButton.Margin = new System.Windows.Forms.Padding(1);
            this.DeployOfficeButton.Name = "DeployOfficeButton";
            this.DeployOfficeButton.Size = new System.Drawing.Size(45, 45);
            this.DeployOfficeButton.TabIndex = 60;
            this.toolTip.SetToolTip(this.DeployOfficeButton, "Deploy Office 365");
            this.DeployOfficeButton.UseVisualStyleBackColor = true;
            this.DeployOfficeButton.Click += new System.EventHandler(this.DeployOfficeButton_Click);
            // 
            // NewDeployButton
            // 
            this.NewDeployButton.BackgroundImage = global::AssetManager.Properties.Resources.DeployIcon;
            this.NewDeployButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.NewDeployButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.NewDeployButton.Location = new System.Drawing.Point(189, 48);
            this.NewDeployButton.Margin = new System.Windows.Forms.Padding(1);
            this.NewDeployButton.Name = "NewDeployButton";
            this.NewDeployButton.Size = new System.Drawing.Size(45, 45);
            this.NewDeployButton.TabIndex = 61;
            this.toolTip.SetToolTip(this.NewDeployButton, "Software Deployments");
            this.NewDeployButton.UseVisualStyleBackColor = true;
            this.NewDeployButton.Click += new System.EventHandler(this.NewDeployButton_Click);
            // 
            // RemoteToolsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.ToolsGroupBox);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "RemoteToolsControl";
            this.Size = new System.Drawing.Size(421, 108);
            this.Load += new System.EventHandler(this.RemoteToolsControl_Load);
            this.VisibleChanged += new System.EventHandler(this.RemoteToolsControl_VisibleChanged);
            this.ToolsGroupBox.ResumeLayout(false);
            this.ToolsLayoutPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RestartDeviceButton)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox ToolsGroupBox;
        internal System.Windows.Forms.FlowLayoutPanel ToolsLayoutPanel;
        internal System.Windows.Forms.Button GKUpdateButton;
        internal System.Windows.Forms.Button BrowseFilesButton;
        internal System.Windows.Forms.PictureBox RestartDeviceButton;
        internal System.Windows.Forms.Button StartRDPButton;
        internal System.Windows.Forms.Button DeployTVButton;
        internal System.Windows.Forms.Button UpdateChromeButton;
        internal System.Windows.Forms.Button ShowIPButton;
        internal System.Windows.Forms.Button EventViewerButton;
        private System.Windows.Forms.ToolTip toolTip;
        internal System.Windows.Forms.Button DeployOfficeButton;
        internal System.Windows.Forms.Button NewDeployButton;
        internal System.Windows.Forms.Button PowerShellButton;
        internal System.Windows.Forms.Button PSExecButton;
    }
}
