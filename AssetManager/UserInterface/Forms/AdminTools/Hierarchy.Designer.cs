namespace AssetManager.UserInterface.Forms.AdminTools
{
    partial class Hierarchy
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.workingSpinner = new System.Windows.Forms.ToolStripStatusLabel();
            this.HierarchyTree = new AssetManager.UserInterface.CustomControls.CorrectedTreeView();
            this.groupBox1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.statusStrip1);
            this.groupBox1.Controls.Add(this.HierarchyTree);
            this.groupBox1.Location = new System.Drawing.Point(0, 3);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(399, 480);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Supervisor && Employee Hierarchy";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.workingSpinner});
            this.statusStrip1.Location = new System.Drawing.Point(3, 455);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip1.Size = new System.Drawing.Size(393, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // workingSpinner
            // 
            this.workingSpinner.Image = global::AssetManager.Properties.Resources.LoadingAni;
            this.workingSpinner.Name = "workingSpinner";
            this.workingSpinner.Size = new System.Drawing.Size(16, 17);
            this.workingSpinner.Visible = false;
            // 
            // HierarchyTree
            // 
            this.HierarchyTree.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.HierarchyTree.BackColor = System.Drawing.Color.FromArgb(64,64,64);
            this.HierarchyTree.Font = new System.Drawing.Font("Consolas", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.HierarchyTree.ForeColor = System.Drawing.Color.FromArgb(224,224,224);
            this.HierarchyTree.LineColor = System.Drawing.Color.DarkGray;
            this.HierarchyTree.Location = new System.Drawing.Point(6, 20);
            this.HierarchyTree.Name = "HierarchyTree";
            this.HierarchyTree.Size = new System.Drawing.Size(387, 432);
            this.HierarchyTree.TabIndex = 0;
            // 
            // Hierarchy
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(399, 483);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, 0);
            this.MinimumSize = new System.Drawing.Size(334, 296);
            this.Name = "Hierarchy";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Hierarchy";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private CustomControls.CorrectedTreeView HierarchyTree;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel statusLabel;
        private System.Windows.Forms.ToolStripStatusLabel workingSpinner;
    }
}