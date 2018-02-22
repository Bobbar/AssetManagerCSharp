using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    partial class StatusToolStripMenuItem : ToolStripMenuItem
    {
        private bool hostOnline = false;

        private ExtendedForm linkedForm;

        public ExtendedForm LinkedForm
        {
            get
            {
                return linkedForm;
            }

            set
            {
                linkedForm = value;
                hostOnline = linkedForm.OnlineStatus;
                linkedForm.OnlineStatusChanged -= HostOnlineStatusChanged;
                linkedForm.OnlineStatusChanged += HostOnlineStatusChanged;
                this.Invalidate();
            }
        }

        public StatusToolStripMenuItem() : base()
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (hostOnline)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.FillRectangle(Brushes.LimeGreen, this.Image.Width, 0, 3, this.Bounds.Height);
            }
        }

        private void HostOnlineStatusChanged(object sender, bool e)
        {
            var onlineStatus = e;
            if (onlineStatus != hostOnline)
            {
                hostOnline = onlineStatus;
                this.Invalidate();
            }
        }
    }
}