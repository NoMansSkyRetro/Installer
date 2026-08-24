using System;
using System.Windows.Forms;

namespace NMSLegacyVersionInstaller.Steps
{
    public partial class SteamGuardForm : Form
    {
        public SteamGuardForm(string promptText)
        {
            InitializeComponent();
            pbLogo.Image = Properties.Resources.steam_logo_transparent;
            if (!string.IsNullOrWhiteSpace(promptText))
                lblMessage.Text = promptText.Trim();
        }

        public string Code { get { return txtCode.Text.Trim(); } }

        private void btnConfirm_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtCode.Text))
                return;
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}
