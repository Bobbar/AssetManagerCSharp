using System.Windows.Forms;
namespace AssetManager.UserInterface.Forms.AssetManagement
{

    partial class MunisUserForm
    {

        //Form overrides dispose to clean up the component list.
        
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

        private System.ComponentModel.IContainer components  = null;
        //NOTE: The following procedure is required by the Windows Form Designer
        //It can be modified using the Windows Form Designer.  
        //Do not modify it using the code editor.
        
        private void InitializeComponent()
        {
            this.MunisResults = new System.Windows.Forms.DataGridView();
            this.GroupBox1 = new System.Windows.Forms.GroupBox();
            this.SelectedEmpLabel = new System.Windows.Forms.Label();
            this.SelectButton = new System.Windows.Forms.Button();
            this.SearchPanel = new System.Windows.Forms.GroupBox();
            this.WorkSpinner = new System.Windows.Forms.PictureBox();
            this.Label1 = new System.Windows.Forms.Label();
            this.SearchButton = new System.Windows.Forms.Button();
            this.SearchNameTextBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.MunisResults)).BeginInit();
            this.GroupBox1.SuspendLayout();
            this.SearchPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WorkSpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // MunisResults
            // 
            this.MunisResults.AllowUserToAddRows = false;
            this.MunisResults.AllowUserToDeleteRows = false;
            this.MunisResults.AllowUserToResizeRows = false;
            this.MunisResults.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.MunisResults.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.MunisResults.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.MunisResults.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.MunisResults.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.MunisResults.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.MunisResults.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.MunisResults.EnableHeadersVisualStyles = false;
            this.MunisResults.Location = new System.Drawing.Point(6, 19);
            this.MunisResults.Name = "MunisResults";
            this.MunisResults.ReadOnly = true;
            this.MunisResults.RowHeadersVisible = false;
            this.MunisResults.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.MunisResults.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.MunisResults.ShowCellToolTips = false;
            this.MunisResults.ShowEditingIcon = false;
            this.MunisResults.Size = new System.Drawing.Size(672, 240);
            this.MunisResults.TabIndex = 42;
            this.MunisResults.VirtualMode = true;
            this.MunisResults.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.MunisResults_CellClick);
            this.MunisResults.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.MunisResults_CellDoubleClick);
            // 
            // GroupBox1
            // 
            this.GroupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.GroupBox1.Controls.Add(this.SelectedEmpLabel);
            this.GroupBox1.Controls.Add(this.SelectButton);
            this.GroupBox1.Controls.Add(this.MunisResults);
            this.GroupBox1.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.GroupBox1.Location = new System.Drawing.Point(12, 103);
            this.GroupBox1.Name = "GroupBox1";
            this.GroupBox1.Size = new System.Drawing.Size(686, 360);
            this.GroupBox1.TabIndex = 43;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Results";
            // 
            // SelectedEmpLabel
            // 
            this.SelectedEmpLabel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SelectedEmpLabel.Location = new System.Drawing.Point(6, 277);
            this.SelectedEmpLabel.Name = "SelectedEmpLabel";
            this.SelectedEmpLabel.Size = new System.Drawing.Size(672, 18);
            this.SelectedEmpLabel.TabIndex = 44;
            this.SelectedEmpLabel.Text = "Selected Emp:";
            this.SelectedEmpLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SelectButton
            // 
            this.SelectButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.SelectButton.Location = new System.Drawing.Point(287, 310);
            this.SelectButton.Name = "SelectButton";
            this.SelectButton.Size = new System.Drawing.Size(92, 29);
            this.SelectButton.TabIndex = 43;
            this.SelectButton.Text = "Select";
            this.SelectButton.UseVisualStyleBackColor = true;
            this.SelectButton.Click += new System.EventHandler(this.SelectButton_Click);
            // 
            // SearchPanel
            // 
            this.SearchPanel.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.SearchPanel.Controls.Add(this.WorkSpinner);
            this.SearchPanel.Controls.Add(this.Label1);
            this.SearchPanel.Controls.Add(this.SearchButton);
            this.SearchPanel.Controls.Add(this.SearchNameTextBox);
            this.SearchPanel.Font = new System.Drawing.Font("Consolas", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SearchPanel.Location = new System.Drawing.Point(12, 12);
            this.SearchPanel.Name = "SearchPanel";
            this.SearchPanel.Size = new System.Drawing.Size(686, 85);
            this.SearchPanel.TabIndex = 44;
            this.SearchPanel.TabStop = false;
            this.SearchPanel.Text = "Search";
            // 
            // WorkSpinner
            // 
            this.WorkSpinner.Image = global::AssetManager.Properties.Resources.LoadingAni;
            this.WorkSpinner.Location = new System.Drawing.Point(178, 48);
            this.WorkSpinner.Name = "WorkSpinner";
            this.WorkSpinner.Size = new System.Drawing.Size(22, 22);
            this.WorkSpinner.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.WorkSpinner.TabIndex = 0;
            this.WorkSpinner.TabStop = false;
            this.WorkSpinner.Visible = false;
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(15, 32);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(140, 14);
            this.Label1.TabIndex = 2;
            this.Label1.Text = "First or Last Name:";
            // 
            // SearchButton
            // 
            this.SearchButton.Location = new System.Drawing.Point(213, 45);
            this.SearchButton.Name = "SearchButton";
            this.SearchButton.Size = new System.Drawing.Size(131, 26);
            this.SearchButton.TabIndex = 1;
            this.SearchButton.Text = "Search";
            this.SearchButton.UseVisualStyleBackColor = true;
            this.SearchButton.Click += new System.EventHandler(this.SearchButton_Click);
            // 
            // SearchNameTextBox
            // 
            this.SearchNameTextBox.Location = new System.Drawing.Point(18, 48);
            this.SearchNameTextBox.Name = "SearchNameTextBox";
            this.SearchNameTextBox.Size = new System.Drawing.Size(157, 22);
            this.SearchNameTextBox.TabIndex = 0;
            this.SearchNameTextBox.KeyDown += new System.Windows.Forms.KeyEventHandler(this.SearchNameTextBox_KeyDown);
            // 
            // MunisUserForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(713, 477);
            this.Controls.Add(this.SearchPanel);
            this.Controls.Add(this.GroupBox1);
            this.MinimumSize = new System.Drawing.Size(391, 441);
            this.Name = "MunisUserForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Employee Search";
            this.Load += new System.EventHandler(this.MunisUser_Load);
            this.Shown += new System.EventHandler(this.MunisUserForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.MunisResults)).EndInit();
            this.GroupBox1.ResumeLayout(false);
            this.SearchPanel.ResumeLayout(false);
            this.SearchPanel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.WorkSpinner)).EndInit();
            this.ResumeLayout(false);

        }

        internal DataGridView MunisResults;
        internal GroupBox GroupBox1;
        internal Button SelectButton;
        internal GroupBox SearchPanel;
        internal Label Label1;
        internal TextBox SearchNameTextBox;
        internal Button SearchButton;
        internal Label SelectedEmpLabel;
        internal PictureBox WorkSpinner;
    }
}
