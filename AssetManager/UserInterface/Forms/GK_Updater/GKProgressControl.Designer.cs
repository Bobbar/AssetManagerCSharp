using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Diagnostics;
using System.Windows.Forms;
using System.Linq;
using System.Threading.Tasks;
namespace AssetManager.UserInterface.Forms.GK_Updater
{
    
    partial class GKProgressControl : System.Windows.Forms.UserControl
    {

        //UserControl overrides dispose to clean up the component list.
        
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

        private System.ComponentModel.IContainer components;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.pbarFileProgress = new System.Windows.Forms.ProgressBar();
            this.lblStatus = new System.Windows.Forms.Label();
            this.lblInfo = new System.Windows.Forms.Label();
            this.rtbLog = new System.Windows.Forms.RichTextBox();
            this.lblShowHide = new System.Windows.Forms.Label();
            this.UI_Timer = new System.Windows.Forms.Timer(this.components);
            this.MyToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.pbRestart = new System.Windows.Forms.PictureBox();
            this.pbCancelClose = new System.Windows.Forms.PictureBox();
            this.lblSeq = new System.Windows.Forms.Label();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.lblTransRate = new System.Windows.Forms.Label();
            this.pbarProgress = new System.Windows.Forms.ProgressBar();
            this.pbStatus = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pbRestart)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCancelClose)).BeginInit();
            this.Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).BeginInit();
            this.SuspendLayout();
            // 
            // pbarFileProgress
            // 
            this.pbarFileProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbarFileProgress.Location = new System.Drawing.Point(75, 49);
            this.pbarFileProgress.Name = "pbarFileProgress";
            this.pbarFileProgress.Size = new System.Drawing.Size(254, 12);
            this.pbarFileProgress.TabIndex = 0;
            // 
            // lblStatus
            // 
            this.lblStatus.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblStatus.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStatus.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblStatus.Location = new System.Drawing.Point(2, 61);
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(395, 14);
            this.lblStatus.TabIndex = 1;
            this.lblStatus.Text = "[Status/File]";
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblInfo
            // 
            this.lblInfo.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lblInfo.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.lblInfo.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.lblInfo.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblInfo.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblInfo.ForeColor = System.Drawing.Color.White;
            this.lblInfo.ImageAlign = System.Drawing.ContentAlignment.TopCenter;
            this.lblInfo.Location = new System.Drawing.Point(75, 4);
            this.lblInfo.Name = "lblInfo";
            this.lblInfo.Size = new System.Drawing.Size(254, 17);
            this.lblInfo.TabIndex = 2;
            this.lblInfo.Text = "[Computer Info]";
            this.lblInfo.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            this.MyToolTip.SetToolTip(this.lblInfo, "Click to view device.");
            this.lblInfo.Click += new System.EventHandler(this.lblInfo_Click);
            // 
            // rtbLog
            // 
            this.rtbLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbLog.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.rtbLog.DetectUrls = false;
            this.rtbLog.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.rtbLog.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(234)))), ((int)(((byte)(234)))), ((int)(((byte)(234)))));
            this.rtbLog.Location = new System.Drawing.Point(4, 87);
            this.rtbLog.Name = "rtbLog";
            this.rtbLog.ReadOnly = true;
            this.rtbLog.Size = new System.Drawing.Size(391, 208);
            this.rtbLog.TabIndex = 4;
            this.rtbLog.Text = "";
            // 
            // lblShowHide
            // 
            this.lblShowHide.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblShowHide.AutoSize = true;
            this.lblShowHide.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblShowHide.Font = new System.Drawing.Font("Wingdings 3", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(2)));
            this.lblShowHide.ImageAlign = System.Drawing.ContentAlignment.BottomCenter;
            this.lblShowHide.Location = new System.Drawing.Point(373, 72);
            this.lblShowHide.Name = "lblShowHide";
            this.lblShowHide.Size = new System.Drawing.Size(17, 12);
            this.lblShowHide.TabIndex = 5;
            this.lblShowHide.Text = "s";
            this.MyToolTip.SetToolTip(this.lblShowHide, "Show/Hide Log");
            this.lblShowHide.Click += new System.EventHandler(this.lblShowHide_Click);
            // 
            // UI_Timer
            // 
            this.UI_Timer.Enabled = true;
            this.UI_Timer.Tick += new System.EventHandler(this.UI_Timer_Tick);
            // 
            // pbRestart
            // 
            this.pbRestart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbRestart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbRestart.Image = global::AssetManager.Properties.Resources.RestartIcon;
            this.pbRestart.Location = new System.Drawing.Point(350, 2);
            this.pbRestart.Name = "pbRestart";
            this.pbRestart.Size = new System.Drawing.Size(20, 20);
            this.pbRestart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbRestart.TabIndex = 7;
            this.pbRestart.TabStop = false;
            this.MyToolTip.SetToolTip(this.pbRestart, "Start/Restart Update");
            this.pbRestart.Click += new System.EventHandler(this.pbRestart_Click);
            // 
            // pbCancelClose
            // 
            this.pbCancelClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.pbCancelClose.Cursor = System.Windows.Forms.Cursors.Hand;
            this.pbCancelClose.Image = global::AssetManager.Properties.Resources.CloseCancelDeleteIcon;
            this.pbCancelClose.Location = new System.Drawing.Point(376, 2);
            this.pbCancelClose.Name = "pbCancelClose";
            this.pbCancelClose.Size = new System.Drawing.Size(20, 20);
            this.pbCancelClose.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbCancelClose.TabIndex = 3;
            this.pbCancelClose.TabStop = false;
            this.MyToolTip.SetToolTip(this.pbCancelClose, "Cancel/Close");
            this.pbCancelClose.Click += new System.EventHandler(this.pbCancelClose_Click);
            // 
            // lblSeq
            // 
            this.lblSeq.AutoSize = true;
            this.lblSeq.Location = new System.Drawing.Point(1, 2);
            this.lblSeq.Name = "lblSeq";
            this.lblSeq.Size = new System.Drawing.Size(42, 14);
            this.lblSeq.TabIndex = 6;
            this.lblSeq.Text = "[Seq]";
            // 
            // Panel1
            // 
            this.Panel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
            | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.Panel1.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Panel1.BackColor = System.Drawing.Color.Silver;
            this.Panel1.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.Panel1.Controls.Add(this.lblTransRate);
            this.Panel1.Controls.Add(this.pbarProgress);
            this.Panel1.Controls.Add(this.pbStatus);
            this.Panel1.Controls.Add(this.lblSeq);
            this.Panel1.Controls.Add(this.pbRestart);
            this.Panel1.Controls.Add(this.lblShowHide);
            this.Panel1.Controls.Add(this.pbCancelClose);
            this.Panel1.Controls.Add(this.rtbLog);
            this.Panel1.Controls.Add(this.lblStatus);
            this.Panel1.Controls.Add(this.pbarFileProgress);
            this.Panel1.Controls.Add(this.lblInfo);
            this.Panel1.Location = new System.Drawing.Point(0, 0);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(400, 300);
            this.Panel1.TabIndex = 8;
            // 
            // lblTransRate
            // 
            this.lblTransRate.AutoSize = true;
            this.lblTransRate.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTransRate.Location = new System.Drawing.Point(327, 48);
            this.lblTransRate.Name = "lblTransRate";
            this.lblTransRate.Size = new System.Drawing.Size(43, 13);
            this.lblTransRate.TabIndex = 10;
            this.lblTransRate.Text = "[MBps]";
            // 
            // pbarProgress
            // 
            this.pbarProgress.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
            | System.Windows.Forms.AnchorStyles.Right)));
            this.pbarProgress.Location = new System.Drawing.Point(39, 27);
            this.pbarProgress.Name = "pbarProgress";
            this.pbarProgress.Size = new System.Drawing.Size(327, 20);
            this.pbarProgress.TabIndex = 9;
            // 
            // pbStatus
            // 
            this.pbStatus.Location = new System.Drawing.Point(2, 25);
            this.pbStatus.Name = "pbStatus";
            this.pbStatus.Size = new System.Drawing.Size(37, 30);
            this.pbStatus.TabIndex = 8;
            this.pbStatus.TabStop = false;
            // 
            // GKProgressControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Controls.Add(this.Panel1);
            this.DoubleBuffered = true;
            this.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MaximumSize = new System.Drawing.Size(400, 300);
            this.MinimumSize = new System.Drawing.Size(400, 87);
            this.Name = "GKProgressControl";
            this.Size = new System.Drawing.Size(400, 300);
            ((System.ComponentModel.ISupportInitialize)(this.pbRestart)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbCancelClose)).EndInit();
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbStatus)).EndInit();
            this.ResumeLayout(false);

        }

        internal ProgressBar pbarFileProgress;
        internal Label lblStatus;
        internal Label lblInfo;
        internal PictureBox pbCancelClose;
        internal RichTextBox rtbLog;
        internal Label lblShowHide;
        internal Timer UI_Timer;
        internal ToolTip MyToolTip;
        internal Label lblSeq;
        internal PictureBox pbRestart;
        internal Panel Panel1;
        internal PictureBox pbStatus;
        internal ProgressBar pbarProgress;
        internal Label lblTransRate;
    }
}
