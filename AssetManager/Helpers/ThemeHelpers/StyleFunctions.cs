using System;
using System.Drawing;
using System.Windows.Forms;
namespace AssetManager.Helpers
{
    static class StyleFunctions
    {
        public static DataGridViewCellStyle DefaultGridStyles;
        public static DataGridViewCellStyle AlternatingRowDefaultStyles;

        public static Font DefaultGridFont = new Font("Consolas", 9.75f, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, Convert.ToByte(0));


        public static void SetGridStyle(DataGridView grid, GridTheme theme)
        {
            // grid.BackgroundColor = Colors.DefaultGridBackColor;
            grid.DefaultCellStyle = new DataGridViewCellStyle(DefaultGridStyles);
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle(AlternatingRowDefaultStyles);
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = theme.CellAltSelectColor;
            grid.DefaultCellStyle.SelectionBackColor = theme.CellSelectColor;
            grid.Font = DefaultGridStyles.Font;
        }

        public static void HighlightRow(DataGridView Grid, GridTheme Theme, int Row)
        {
            try
            {
                Color BackColor = Grid.CurrentRow.InheritedStyle.BackColor;
                Color SelectColor = Grid.CurrentRow.InheritedStyle.SelectionBackColor;

                if (Row > -1)
                {
                    Color c1 = Theme.RowHighlightColor;
                    Color c2 = SelectColor;
                    var BlendColorSelect = Colors.ColorAlphaBlend(c1, c2);
                    c2 = Color.FromArgb(BackColor.R, BackColor.G, BackColor.B);
                    var BlendColorBack = Colors.ColorAlphaBlend(c1, c2);
                    foreach (DataGridViewCell cell in Grid.Rows[Row].Cells)
                    {
                        cell.Style.SelectionBackColor = BlendColorSelect;
                        cell.Style.BackColor = BlendColorBack;
                    }

                }
            }
            catch
            {
            }
        }

        public static void LeaveRow(DataGridView Grid, int Row)
        {
            Color BackColor = Grid.Rows[Row].InheritedStyle.BackColor;
            Color SelectColor = Grid.CurrentRow.InheritedStyle.SelectionBackColor;

            if (Row > -1)
            {
                foreach (DataGridViewCell cell in Grid.Rows[Row].Cells)
                {
                    cell.Style.SelectionBackColor = SelectColor;
                    cell.Style.BackColor = BackColor;
                }
            }
        }

    }
}
