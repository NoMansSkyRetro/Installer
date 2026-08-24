using System.Windows.Forms;

namespace NMSLegacyVersionInstaller.Steps
{
    // Passive choice step (shown after the download): how save games are stored.
    // Read by FinalSteps and Complete via rbVersionId.Checked.
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
        }
    }
}
