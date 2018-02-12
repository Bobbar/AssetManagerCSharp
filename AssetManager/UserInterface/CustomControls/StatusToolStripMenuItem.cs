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
                int lightXPos = this.Bounds.Width - 20;
                int lightYPos = (this.Bounds.Height / 2) - (onlineLightSize / 2);
                e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                e.Graphics.FillEllipse(Brushes.LimeGreen, lightXPos, lightYPos, onlineLightSize, onlineLightSize);
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