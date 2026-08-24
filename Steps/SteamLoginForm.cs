using System;
using System.Windows.Forms;

namespace NMSLegacyVersionInstaller.Steps
{
    public partial class SteamLoginForm : Form
    {
        public SteamLoginForm()
        {
            InitializeComponent();
            pbLogo.Image = Properties.Resources.steam_logo_transparent;
            if (!string.IsNullOrEmpty(Steam.Username))
                txtUsername.Text = Steam.Username;
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrEmpty(txtPassword.Text))
            {
                lblError.Text = "Please enter your Steam username and password.";
                return;
            }
            Steam.Username = txtUsername.Text.Trim();
            Steam.Password = txtPassword.Text;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
