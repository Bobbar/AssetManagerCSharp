using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;
using AssetManager.UserInterface.CustomControls;

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

        public StatusToolStripMenuItem() : base() { }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (HostOnline)
            {
                int lightXPos = this.Bounds.Width - 20;
                int lightYPos = (this.Bounds.Height / 2) - (onlineLightSize / 2);
                e.Graphics.FillEllipse(Brushes.LimeGreen, lightXPos, lightYPos, onlineLightSize, onlineLightSize);
            }
        }

        public void HostOnlineStatusChanged(object sender, EventArgs e)
        {
            var statusArgs = (ExtendedForm.OnlineStatusChangedEventArgs)e;
            HostOnline = statusArgs.OnlineStatus;
            this.Invalidate();
        }

    }
}
