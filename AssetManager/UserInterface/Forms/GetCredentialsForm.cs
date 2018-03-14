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

        public GetCredentialsForm(string credentialDescription, string lastUsername) : base()
        {
            InitializeComponent();
            this.Icon = Properties.Resources.asset_icon;
            CredDescriptionLabel.Text = credentialDescription;
            if (!string.IsNullOrEmpty(lastUsername))
            {
                UsernameTextBox.Text = lastUsername;
                this.ActiveControl = PasswordTextBox;
            }
        }

        private void Accept()
        {
            string username = null;
            username = UsernameTextBox.Text.Trim();
            if (!string.IsNullOrEmpty(username) & securePwd.Length > 0)
            {
                securePwd.MakeReadOnly();
                newCreds = new NetworkCredential(username, securePwd, NetworkInfo.CurrentDomain);
                securePwd.Dispose();
                DialogResult = DialogResult.OK;
            }
            else
            {
                OtherFunctions.Message("Username or Password incomplete.", MessageBoxButtons.OK, MessageBoxIcon.Exclamation, "Missing Info", this);
            }
        }

        private void AcceptCredsButton_Click(object sender, EventArgs e)
        {
            Accept();
        }

        private void PasswordTextBox_KeyDown(object sender, KeyEventArgs e)
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

        private void PasswordTextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != Convert.ToChar(Keys.Back) & e.KeyChar != Convert.ToChar(Keys.Enter))
            {
                securePwd.AppendChar(e.KeyChar);
            }
        }

        private void PasswordTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            SetText();
        }

        private void SetText()
        {
            PasswordTextBox.Text = BlankText(securePwd.Length);
            PasswordTextBox.SelectionStart = PasswordTextBox.Text.Length + 1;
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