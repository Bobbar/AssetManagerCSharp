using System.Drawing;
using System.Windows.Forms;

namespace AssetManager.UserInterface.CustomControls
{
    partial class OnlineStatusMenuItem : ToolStripMenuItem
    {
        private ExtendedForm targetForm;

        public ExtendedForm TargetForm
        {
            get
            {
                return targetForm;
            }

            set
            {
                targetForm = value;

                if (targetForm != null)
                {
                    targetForm.TextChanged -= TargetForm_TextChanged;
                    targetForm.TextChanged += TargetForm_TextChanged;
                }
            }
        }

        private void TargetForm_TextChanged(object sender, System.EventArgs e)
        {
            this.Text = ((ExtendedForm)sender).Text;
        }

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
            targetForm.TextChanged -= TargetForm_TextChanged;
            TargetForm = null;
            onlineStatusInterface = null;
            base.Dispose(disposing);
        }
    }
}