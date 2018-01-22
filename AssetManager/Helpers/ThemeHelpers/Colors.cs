using System.Drawing;
using System;
namespace AssetManager.Helpers
{
    internal static class Colors
    {
        public static Color MissingField { get; } = ColorTranslator.FromHtml("#ffcccc");

        //"#82C1FF") '"#FF9827") '"#75BAFF")
        public static Color CheckIn { get; } = ColorTranslator.FromHtml("#B6FCC0");

        public static Color CheckOut { get; } = ColorTranslator.FromHtml("#FCB6B6");
        public static Color HighlightBlue { get; } = Color.FromArgb(46, 112, 255);

        //ColorTranslator.FromHtml("#8BCEE8")
        public static Color SibiSelectColor { get; } = Color.FromArgb(185, 205, 255);

        public static Color SibiSelectAltColor { get; } = Color.FromArgb(172, 191, 237);

        //(146, 148, 255) '(31, 47, 155)
        public static Color OrangeHighlightColor { get; } = ColorTranslator.FromHtml("#FF6600");

        public static Color OrangeSelectColor { get; } = Color.FromArgb(255, 185, 23);
        public static Color OrangeSelectAltColor { get; } = Color.FromArgb(255, 152, 30);
        public static Color EditColor { get; } = ColorTranslator.FromHtml("#81EAAA");
        public static Color DefaultFormBackColor { get; } = Color.FromArgb(232, 232, 232);
        public static Color StatusBarProblem { get; } = ColorTranslator.FromHtml("#FF9696");
        public static Color AssetToolBarColor { get; } = Color.FromArgb(249, 226, 166);
        public static Color SibiToolBarColor { get; } = Color.FromArgb(185, 205, 255);

        //(148, 213, 255)
        public static Color DefaultGridBackColor { get; set; }

        public static Color DefaultGridSelectColor { get; set; }


        /// <summary>
        /// Alpha blend two colors.
        /// </summary>
        /// <param name="InColor"></param>
        /// <param name="BlendColor"></param>
        /// <returns></returns>
        public static Color ColorAlphaBlend(Color InColor, Color BlendColor)
        {
            Color OutColor = default(Color);
            OutColor = Color.FromArgb(Convert.ToInt32((Convert.ToInt32(InColor.A) + Convert.ToInt32(BlendColor.A)) / 2),
                Convert.ToInt32((Convert.ToInt32(InColor.R) + Convert.ToInt32(BlendColor.R)) / 2),
                Convert.ToInt32((Convert.ToInt32(InColor.G) + Convert.ToInt32(BlendColor.G)) / 2),
                Convert.ToInt32((Convert.ToInt32(InColor.B) + Convert.ToInt32(BlendColor.B)) / 2));
            return OutColor;
        }



    }
}