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
        }
    }
}
