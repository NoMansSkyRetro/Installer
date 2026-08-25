using System.Windows.Forms;

namespace NMSLegacyVersionInstaller.Steps
{
    // Passive choice step (shown after the download): how save games are stored.
    // Read by FinalSteps and Complete via rbVersionId.Checked; the picked real account via SelectedSteamId.
    // The save-folder table (tlpFolders) is built from Labels in the designer; the ids shown there
    // mirror Steam.DummySteamID - keep them in sync if the version ids ever change.
    public partial class SaveGameStep : UserControl
    {
        public SaveGameStep()
        {
            InitializeComponent();
            pbOrb1.Image = ImageScaler.FitHighQuality(Properties.Resources.release_orb, pbOrb1.Size);
            pbOrb2.Image = ImageScaler.FitHighQuality(Properties.Resources.foundation_orb, pbOrb2.Size);
            pbOrb3.Image = ImageScaler.FitHighQuality(Properties.Resources.path_finder_orb, pbOrb3.Size);
            pbOrb4.Image = ImageScaler.FitHighQuality(Properties.Resources.atlas_rises_orb, pbOrb4.Size);
            pbSmart.Image = ImageScaler.FitHighQuality(Properties.Resources.smartsavefolder, pbSmart.Size);

            // Fill the account picker from the Steam accounts detected on this machine.
            cmbSteamUser.Items.AddRange(Steam.GetSteamUsers().ToArray());
            cmbSteamUser.SelectedIndex = 0;
            cmbSteamUser.Enabled = rbRealId.Checked;
            rbRealId.CheckedChanged += (s, e) => cmbSteamUser.Enabled = rbRealId.Checked;
        }

        // SteamID64 of the account picked in the dropdown, or "" for the emulator default ("Default User").
        public string SelectedSteamId
        {
            get
            {
                var user = cmbSteamUser.SelectedItem as Steam.SteamUser;
                return user != null ? user.Id64 : "";
            }
        }
    }
}
