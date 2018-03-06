using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms
{
    public partial class GridForm : ExtendedForm
    {
        #region "Fields"

        private bool gridFilling = true;
        private List<DataGridView> gridList = new List<DataGridView>();

        private DataGridViewRow lastDoubleClickRow;

        #endregion "Fields"

        #region "Constructors"

        public GridForm(ExtendedForm parentForm, string title = "") : base(parentForm)
        {
            Load += GridForm_Load;
            Disposed += GridForm_Disposed;
            Resize += GridForm_Resize;
            Closing += GridForm_Closing;
            // This call is required by the designer.
            InitializeComponent();
            if (!string.IsNullOrEmpty(title))
                this.Text = title;
            // Add any initialization after the InitializeComponent() call.
            GridPanel.DoubleBufferedTableLayout(true);
            Panel1.DoubleBufferedPanel(true);
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
            var NewGrid = GetNewGrid(name, label + " (" + datatable.Rows.Count.ToString() + " rows)");
            FillGrid(NewGrid, datatable);
            gridList.Add(NewGrid);
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

        private DataGridView GetNewGrid(string name, string label)
        {
            DataGridView newGrid = new DataGridView();
            newGrid.Name = name;
            newGrid.Tag = label;
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
            newGrid.CellDoubleClick += GridDoubleClickCell;
            newGrid.DoubleBufferedDataGrid(true);
            return newGrid;
        }

        private void GridDoubleClickCell(object sender, EventArgs e)
        {
            DataGridView SenderGrid = (DataGridView)sender;
            lastDoubleClickRow = SenderGrid.CurrentRow;
            this.DialogResult = DialogResult.OK;
        }

        private void GridEnterCell(object sender, DataGridViewCellEventArgs e)
        {
            if (!gridFilling)
            {
                StyleFunctions.HighlightRow((DataGridView)sender, this.GridTheme, e.RowIndex);
            }
        }

        private void GridForm_Closing(object sender, CancelEventArgs e)
        {
            if (!Modal)
                this.Dispose();
        }

        private void GridForm_Resize(object sender, EventArgs e)
        {
            if (!gridFilling)
                ResizeGridPanel();
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

        private void GridLeaveCell(object sender, DataGridViewCellEventArgs e)
        {
            StyleFunctions.LeaveRow((DataGridView)sender, e.RowIndex);
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

        private void SendToNewGridForm_Click(object sender, EventArgs e)
        {
            GetActiveGrid().CopyToGridForm(ParentForm);
        }

        private void GridForm_Disposed(object sender, EventArgs e)
        {
            foreach (DataGridView grid in gridList)
            {
                ((DataTable)grid.DataSource).Dispose();
                grid.Dispose();
            }
            if (lastDoubleClickRow != null)
                lastDoubleClickRow.Dispose();
        }

        private void GridForm_Load(object sender, EventArgs e)
        {
            AddGridsToForm();
            ResizeGrids();
            gridFilling = false;
        }

        #endregion "Methods"
    }
}