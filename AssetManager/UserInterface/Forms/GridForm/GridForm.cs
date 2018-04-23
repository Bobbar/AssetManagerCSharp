using AssetManager.Data;
using AssetManager.Data.Classes;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;
using MyDialogLib;

namespace AssetManager.UserInterface.Forms
{
    public partial class GridForm : ExtendedForm
    {
        #region "Fields"

        private bool gridFilling = true;
        private List<DataGridView> gridList = new List<DataGridView>();
        private DataGridViewRow lastDoubleClickRow;
        private string titlePrefix = "GridForm - ";

        #endregion "Fields"

        #region "Constructors"

        public GridForm(ExtendedForm parentForm, string title = "") : base(parentForm)
        {
            InitializeComponent();
            if (!string.IsNullOrEmpty(title)) this.Text = titlePrefix + title;

            GridPanel.DoubleBuffered(true);
            Panel1.DoubleBuffered(true);
            GridPanel.RowStyles.Clear();
        }

        #endregion "Constructors"

        #region "Properties"

        public int GridCount
        {
            get { return gridList.Count; }
        }

        public DataGridViewRow SelectedRow
        {
            get { return lastDoubleClickRow; }
        }

        #endregion "Properties"

        #region "Methods"

        public void AddGrid(string name, string label, DataTable datatable)
        {
            var newGrid = GetNewGrid(name, label + " (" + datatable.Rows.Count.ToString() + " rows)", DoubleClickAction.ViewOnly);
            FillGrid(newGrid, datatable);
            gridList.Add(newGrid);
        }

        public void AddGrid(string name, string label, DoubleClickAction type, DataTable datatable)
        {
            var newGrid = GetNewGrid(name, label + " (" + datatable.Rows.Count.ToString() + " rows)", type);
            FillGrid(newGrid, datatable);
            gridList.Add(newGrid);
        }

        private void CopySelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            GetActiveGrid().CopyToClipboard();
        }

        private void AddGridsToForm()
        {
            this.SuspendLayout();
            Panel1.SuspendLayout();
            GridPanel.SuspendLayout();
            foreach (DataGridView grid in gridList)
            {
                GroupBox gridBox = new GroupBox();
                gridBox.Text = (string)grid.Tag;
                gridBox.Dock = DockStyle.Fill;
                gridBox.Controls.Add(grid);
                GridPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, GridHeight()));
                GridPanel.Controls.Add(gridBox);
            }
            this.ResumeLayout();
            Panel1.ResumeLayout();
            GridPanel.ResumeLayout();
        }

        private void FillGrid(DataGridView grid, DataTable datatable)
        {
            if (datatable != null) grid.DataSource = datatable;
        }

        private DataGridView GetActiveGrid()
        {
            if (this.ActiveControl is DataGridView)
            {
                return (DataGridView)this.ActiveControl;
            }
            return null;
        }

        private DataGridView GetNewGrid(string name, string label, DoubleClickAction type)
        {
            DataGridView newGrid = new DataGridView();
            var gridLabel = label;
            newGrid.Name = name;
            newGrid.Dock = DockStyle.Fill;
            newGrid.RowHeadersVisible = false;
            newGrid.EditMode = DataGridViewEditMode.EditProgrammatically;
            newGrid.AllowUserToResizeRows = false;
            newGrid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            newGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
            newGrid.AllowUserToAddRows = false;
            newGrid.AllowUserToDeleteRows = false;
            newGrid.Padding = new Padding(0, 0, 0, 10);
            newGrid.ContextMenuStrip = PopUpMenu;
            StyleFunctions.SetGridStyle(newGrid, ParentForm.GridTheme);
            newGrid.CellLeave += GridLeaveCell;
            newGrid.CellEnter += GridEnterCell;

            // Set double-click event according to specified action.
            switch (type)
            {
                case DoubleClickAction.ViewDevice:
                    gridLabel += " - [Double-Click to view]";
                    newGrid.CellDoubleClick += DoubleClickDevice;
                    break;

                case DoubleClickAction.SelectValue:
                    newGrid.CellDoubleClick += DoubleClickSelect;
                    break;

                case DoubleClickAction.ViewOnly:
                    // Don't subscribe double-click event.
                    break;
            }

            newGrid.Tag = gridLabel;
            newGrid.DoubleBuffered(true);
            return newGrid;
        }

        private void DoubleClickSelect(object sender, EventArgs e)
        {
            var clickedGrid = (DataGridView)sender;
            lastDoubleClickRow = clickedGrid.CurrentRow;
            this.DialogResult = DialogResult.OK;
        }

        private void DoubleClickDevice(object sender, EventArgs e)
        {
            var clickedGrid = (DataGridView)sender;
            var selectedGuid = clickedGrid.CurrentRowStringValue(DevicesBaseCols.DeviceGuid);

            if (!string.IsNullOrEmpty(selectedGuid))
            {
                var device = new Device(selectedGuid);
                ChildFormControl.LookupDevice(this.ParentForm, device);
            }
        }

        private void GridEnterCell(object sender, DataGridViewCellEventArgs e)
        {
            if (!gridFilling)
            {
                StyleFunctions.HighlightRow((DataGridView)sender, this.GridTheme, e.RowIndex);
            }
        }

        private int GridHeight()
        {
            int MinHeight = 200;
            int CalcHeight = Convert.ToInt32((this.ClientSize.Height - 30) / gridList.Count);
            if (CalcHeight < MinHeight)
            {
                return MinHeight;
            }
            else
            {
                return CalcHeight;
            }
        }

        private void ResizeGridPanel()
        {
            var NewHeight = GridHeight();
            foreach (DataGridView grid in gridList)
            {
                var row = gridList.IndexOf(grid);
                GridPanel.RowStyles[row].Height = NewHeight;
            }
        }

        private void ResizeGrids()
        {
            foreach (DataGridView grid in gridList)
            {
                grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;
                grid.FastAutoSizeColumns();
                grid.AllowUserToResizeColumns = true;
            }
        }

        /// <summary>
        /// Prompts user for a new form label/title text, sets the form text, and refreshes window lists.
        /// </summary>
        private void ReLabelGridForm()
        {
            using (var renameDialog = new AdvancedDialog(this))
            using (var nameTextBox = new TextBox())
            {
                nameTextBox.Visible = true;
                nameTextBox.Width = 100;

                renameDialog.Text = "Grid Form Label";
                renameDialog.AddCustomControl("nameTextBox", "Enter New Label:", nameTextBox);
                renameDialog.ShowDialog();

                if (renameDialog.DialogResult == DialogResult.OK)
                {
                    var newName = nameTextBox.Text.Trim();
                    if (!string.IsNullOrEmpty(newName))
                    {
                        this.Text = titlePrefix + newName;
                        this.RefreshData();
                    }
                }
            }
        }

        private void GridForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (!Modal) this.Dispose();
        }

        private void GridForm_Resize(object sender, EventArgs e)
        {
            if (!gridFilling) ResizeGridPanel();
        }

        private void GridLeaveCell(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow((DataGridView)sender, e.RowIndex);
        }

        private void SendToNewGridForm_Click(object sender, EventArgs e)
        {
            GetActiveGrid().CopyToGridForm(ParentForm);
        }

        private void GridForm_Load(object sender, EventArgs e)
        {
            AddGridsToForm();
            ResizeGrids();
            gridFilling = false;
        }

        private void RenameFormStripButton_Click(object sender, EventArgs e)
        {
            ReLabelGridForm();
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }

                    foreach (DataGridView grid in gridList)
                    {
                        ((DataTable)grid.DataSource).Dispose();
                        grid.Dispose();
                    }

                    lastDoubleClickRow?.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        #endregion "Methods"
    }
}