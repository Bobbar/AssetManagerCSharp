using System;
using System.Drawing;

namespace AssetManager.Helpers
{
    internal static class Colors
    {
        public static Color MissingField { get; } = ColorTranslator.FromHtml("#ffcccc");
        public static Color CheckIn { get; } = ColorTranslator.FromHtml("#B6FCC0");
        public static Color CheckOut { get; } = ColorTranslator.FromHtml("#FCB6B6");
        public static Color HighlightBlue { get; } = Color.FromArgb(46, 112, 255);
        public static Color SibiSelectColor { get; } = Color.FromArgb(185, 205, 255);
        public static Color SibiSelectAltColor { get; } = Color.FromArgb(172, 191, 237);
        public static Color OrangeHighlightColor { get; } = ColorTranslator.FromHtml("#FF6600");
        public static Color OrangeSelectColor { get; } = Color.FromArgb(255, 185, 23);
        public static Color OrangeSelectAltColor { get; } = Color.FromArgb(255, 152, 30);
        public static Color EditColor { get; } = ColorTranslator.FromHtml("#81EAAA");
        public static Color DefaultFormBackColor { get; } = Color.FromArgb(232, 232, 232);
        public static Color StatusBarProblem { get; } = ColorTranslator.FromHtml("#FF9696");
        public static Color AssetToolBarColor { get; } = Color.FromArgb(249, 226, 166);
        public static Color SibiToolBarColor { get; } = Color.FromArgb(185, 205, 255);

        public static Color DefaultGridBackColor { get; set; }

        
    }
}