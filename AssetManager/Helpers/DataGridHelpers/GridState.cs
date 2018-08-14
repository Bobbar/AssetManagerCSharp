using System.Windows.Forms;

namespace AssetManager.Helpers
{
    public sealed class GridState
    {
        public int ScrollRowIndex { get; set; }
        public int HScrollOffset { get; set; }
        public int SelectedRowIndex { get; set; }
        public int RowCount { get; set; }
        public int SortColumnIndex { get; set; }
        public SortOrder SortOrder { get; set; }

        private DataGridView stateGrid;

        public GridState(DataGridView grid)
        {
            stateGrid = grid;
            ScrollRowIndex = grid.FirstDisplayedScrollingRowIndex;
            HScrollOffset = grid.HorizontalScrollingOffset;
            SelectedRowIndex = grid.SelectedRows.Count > 0 ? grid.SelectedRows[0].Index : -1;
            RowCount = grid.Rows.Count;
            SortColumnIndex = grid.SortedColumn != null ? grid.SortedColumn.Index : -1;
            SortOrder = grid.SortOrder;
        }

        public void RestoreState()
        {
            if (stateGrid.Rows.Count == RowCount)
            {
                if (SelectedRowIndex > 0)
                {
                    //stateGrid.ClearSelection();
                    //stateGrid.CurrentCell = stateGrid.Rows[SelectedRowIndex].Cells[0];
                }

                stateGrid.HorizontalScrollingOffset = HScrollOffset;

                if (ScrollRowIndex > 0)
                    stateGrid.FirstDisplayedScrollingRowIndex = ScrollRowIndex;

                if (SortOrder != SortOrder.None && SortColumnIndex > -1)
                {
                    if (SortOrder == SortOrder.Ascending)
                    {
                        stateGrid.Sort(stateGrid.Columns[SortColumnIndex], System.ComponentModel.ListSortDirection.Ascending);
                    }
                    else
                    {
                        stateGrid.Sort(stateGrid.Columns[SortColumnIndex], System.ComponentModel.ListSortDirection.Descending);
                    }
                }
            }
        }
    }
}