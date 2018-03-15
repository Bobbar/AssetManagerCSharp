using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    partial class OnlineStatusMenuItem : ToolStripMenuItem
    {
        public ExtendedForm TargetForm { get; set; }

        private bool onlineStatus = false;

        private IOnlineStatus onlineStatusInterface = null;

        public void SetOnlineStatusInterface(IOnlineStatus statusObject)
        {
            onlineStatusInterface = statusObject;
            onlineStatusInterface.OnlineStatusChanged -= OnlineStatusChanged;
            onlineStatusInterface.OnlineStatusChanged += OnlineStatusChanged;
        }

        public OnlineStatusMenuItem() : base()
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (onlineStatus)
            {
                e.Graphics.FillRectangle(Brushes.LimeGreen, this.Image.Width, 0, 3, this.Bounds.Height);
            }
        }

        private void OnlineStatusChanged(object sender, bool e)
        {
            var newOnlineStatus = e;
            if (newOnlineStatus != onlineStatus)
            {
                onlineStatus = newOnlineStatus;
                this.Invalidate();
            }
        }

        protected override void Dispose(bool disposing)
        {
            TargetForm = null;
            onlineStatusInterface = null;
            base.Dispose(disposing);
        }
    }
}