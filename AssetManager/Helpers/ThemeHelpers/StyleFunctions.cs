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

        /// <summary>
        /// Alpha blend two colors.
        /// </summary>
        /// <param name="color"></param>
        /// <param name="blendColor"></param>
        /// <returns></returns>
        public static Color ColorAlphaBlend(Color color, Color blendColor)
        {
            // Color outColor = new Color();
            var outColor = Color.FromArgb(((color.A + blendColor.A) / 2),
                                      ((color.R + blendColor.R) / 2),
                                      ((color.G + blendColor.G) / 2),
                                      ((color.B + blendColor.B) / 2));
            return outColor;
        }

        public static void SetGridStyle(DataGridView grid, GridTheme theme)
        {
            grid.DefaultCellStyle = new DataGridViewCellStyle(DefaultGridStyles);
            grid.AlternatingRowsDefaultCellStyle = new DataGridViewCellStyle(AlternatingRowDefaultStyles);
            grid.AlternatingRowsDefaultCellStyle.SelectionBackColor = theme.CellAltSelectColor;
            grid.DefaultCellStyle.SelectionBackColor = theme.CellSelectColor;
            grid.Font = DefaultGridStyles.Font;
        }

        public static void HighlightRow(DataGridView grid, GridTheme theme, int row)
        {
            if (row > -1)
            {
                var backColor = grid.CurrentRow.InheritedStyle.BackColor;
                var selectColor = grid.CurrentRow.InheritedStyle.SelectionBackColor;

                var blendColorSelect = ColorAlphaBlend(theme.RowHighlightColor, selectColor);
                var blendColorBack = ColorAlphaBlend(theme.RowHighlightColor, backColor);

                for (int i = 0; i < grid.Rows[row].Cells.Count; i++)
                {
                    grid.Rows[row].Cells[i].Style.SelectionBackColor = blendColorSelect;
                    grid.Rows[row].Cells[i].Style.BackColor = blendColorBack;
                }
            }
        }

        public static void LeaveRow(DataGridView grid, int row)
        {
            if (row > -1)
            {
                var backColor = grid.Rows[row].InheritedStyle.BackColor;
                var selectColor = grid.CurrentRow.InheritedStyle.SelectionBackColor;

                for (int i = 0; i < grid.Rows[row].Cells.Count; i++)
                {
                    grid.Rows[row].Cells[i].Style.SelectionBackColor = selectColor;
                    grid.Rows[row].Cells[i].Style.BackColor = backColor;
                }
            }
        }
    }
}
