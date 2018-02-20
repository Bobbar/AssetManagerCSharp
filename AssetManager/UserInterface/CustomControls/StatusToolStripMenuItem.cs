using System;
using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    partial class StatusToolStripMenuItem : ToolStripMenuItem
    {
        public bool HostOnline = false;

        private const int onlineLightSize = 10;
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
                HostOnline = linkedForm.OnlineStatus;
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

            if (HostOnline)
            {
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.FillRectangle(Brushes.LimeGreen, this.Image.Width, 0, 3, this.Bounds.Height);
            }
        }

        public void HostOnlineStatusChanged(object sender, EventArgs e)
        {
            var statusArgs = (ExtendedForm.OnlineStatusChangedEventArgs)e;
            if (statusArgs.OnlineStatus != HostOnline)
            {
                HostOnline = statusArgs.OnlineStatus;
                this.Invalidate();
            }
        }
    }
}