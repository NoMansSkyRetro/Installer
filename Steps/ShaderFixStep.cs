using System.Windows.Forms;

namespace NMSLegacyVersionInstaller.Steps
{
    // Passive choice step (shown after the download): whether to auto-apply RetroShaderFix.
    // Read by FinalSteps via rbShaderAuto.Checked. Shader fix (the .pak files) by Ethan (EthanRDoesMC).
    public partial class ShaderFixStep : UserControl
    {
        public ShaderFixStep()
        {
            InitializeComponent();
        }
    }
}
