using AssetManager.UserInterface.CustomControls;
using System.Deployment.Application;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms
{
    public partial class SplashScreenForm : ExtendedForm
    {

        private void SplashScreen1_Load(object sender, System.EventArgs e)
        {
            this.FormBorderStyle = FormBorderStyle.None;

            using (System.Drawing.Drawing2D.GraphicsPath p = new System.Drawing.Drawing2D.GraphicsPath())
            {
                int roundedSize = 70;
                p.StartFigure();
                // Top-left
                p.AddArc(new Rectangle(-1, -1, roundedSize, roundedSize), 180, 90);
                // Top
                p.AddLine(roundedSize, -1, this.Width - roundedSize, -1);
                // Top-right
                p.AddArc(new Rectangle(this.Width - roundedSize, -1, roundedSize, roundedSize), -90, 90);
                // Right
                p.AddLine(this.Width, roundedSize, this.Width, this.Height - roundedSize);
                // Bottom-right
                p.AddArc(new Rectangle(this.Width - roundedSize, this.Height - roundedSize, roundedSize, roundedSize), 0, 90);
                // Bottom
                p.AddLine(this.Width - roundedSize, this.Height, roundedSize, this.Height);
                // Bottom-left;
                p.AddArc(new Rectangle(-1, this.Height - roundedSize, roundedSize, roundedSize), 90, 90);
                p.CloseFigure();
                this.Region = new Region(p);
            }

            if (ApplicationDeployment.IsNetworkDeployed)
            {
                Version.Text = ApplicationDeployment.CurrentDeployment.CurrentVersion.ToString();
            }
            else
            {
                Version.Text = "Debug";
            }
            Assembly asm = Assembly.GetExecutingAssembly();
            string copyright = ((AssemblyCopyrightAttribute)asm.GetCustomAttribute(typeof(AssemblyCopyrightAttribute))).Copyright;
            Copyright.Text = copyright;
        }

        public void SetStatus(string text)
        {
            lblStatus.Text = text;
            this.Refresh();
        }

        public SplashScreenForm()
        {
            Load += SplashScreen1_Load;
            InitializeComponent();
        }
    }
}