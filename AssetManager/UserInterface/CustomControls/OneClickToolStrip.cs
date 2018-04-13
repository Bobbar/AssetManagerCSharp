using System;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    /// <summary>
    /// Modified Toolstrip that responds to clicks as soon as the parent form is activated. As opposed to requiring two clicks (one to activate, another to focus).
    /// </summary>
    public partial class OneClickToolStrip
    {
        private const uint WM_LBUTTONDOWN = 0x201;
        private const uint WM_LBUTTONUP = 0x202;
        private static bool down = false;

        public OneClickToolStrip() : base()
        {
            InitializeComponent();
        }

        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_LBUTTONUP && !down)
            {
                m.Msg = Convert.ToInt32(WM_LBUTTONDOWN);
                base.WndProc(ref m);
                m.Msg = Convert.ToInt32(WM_LBUTTONUP);
            }

            if (m.Msg == WM_LBUTTONDOWN)
            {
                down = true;
            }

            if (m.Msg == WM_LBUTTONUP)
            {
                down = false;
            }

            base.WndProc(ref m);
        }
    }
}