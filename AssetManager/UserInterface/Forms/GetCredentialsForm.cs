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
        private NetworkCredential MyCreds;

        private SecureString SecurePwd = new SecureString();

        public NetworkCredential Credentials
        {
            get { return MyCreds; }
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
            if (!string.IsNullOrEmpty(Username) & SecurePwd.Length > 0)
            {
                SecurePwd.MakeReadOnly();
                MyCreds = new NetworkCredential(Username, SecurePwd, NetworkInfo.CurrentDomain);
                SecurePwd.Dispose();
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
                if (SecurePwd.Length > 0)
                {
                    SecurePwd.RemoveAt(SecurePwd.Length - 1);
                }
            }
        }

        private void txtPassword_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Back) & e.KeyChar != Convert.ToChar(Keys.Enter))
            {
                SecurePwd.AppendChar(e.KeyChar);
            }
        }

        private void txtPassword_KeyUp(object sender, KeyEventArgs e)
        {
            SetText();
        }

        private void SetText()
        {
            txtPassword.Text = BlankText(SecurePwd.Length);
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
    }
}