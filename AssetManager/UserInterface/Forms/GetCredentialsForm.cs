using AssetManager.Data;
using AssetManager.Helpers;
using AssetManager.UserInterface.CustomControls;
using System;
using System.Net;
using System.Security;
using System.Windows.Forms;

namespace AssetManager.UserInterface.Forms
{
    public partial class GetCredentialsForm : ExtendedForm
    {
        private NetworkCredential newCreds;

        private SecureString securePwd = new SecureString();

        public NetworkCredential Credentials
        {
            get { return newCreds; }
        }

        public GetCredentialsForm() : base()
        {
            InitializeComponent();
        }

        public GetCredentialsForm(string credentialDescription) : base()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.asset_icon;
            CredDescriptionLabel.Text = credentialDescription;
        }

        private void Accept()
        {
            string Username = null;
            Username = txtUsername.Text.Trim();
            if (!string.IsNullOrEmpty(Username) & securePwd.Length > 0)
            {
                securePwd.MakeReadOnly();
                newCreds = new NetworkCredential(Username, securePwd, NetworkInfo.CurrentDomain);
                securePwd.Dispose();
                DialogResult = DialogResult.OK;
            }
            else
            {
                OtherFunctions.Message("Username or Password incomplete.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Info", this);
            }
        }

        private void cmdAccept_Click(object sender, EventArgs e)
        {
            Accept();
        }

        private void txtPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Accept();
            }
            if (e.KeyCode == Keys.Back)
            {
                if (securePwd.Length > 0)
                {
                    securePwd.RemoveAt(securePwd.Length - 1);
                }
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Back) & e.KeyChar != Convert.ToChar(Keys.Enter))
            {
                securePwd.AppendChar(e.KeyChar);
            }
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            SetText();
        }

        private void SetText()
        {
            txtPassword.Text = BlankText(securePwd.Length);
            txtPassword.SelectionStart = txtPassword.Text.Length + 1;
        }

        private string BlankText(int length)
        {
            if (length > 0)
            {
                return new string(char.Parse("*"), length);
            }
            return string.Empty;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing)
                {
                    if (components != null)
                    {
                        components.Dispose();
                    }

                    newCreds?.SecurePassword?.Dispose();
                    securePwd?.Dispose();

                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }
    }
}