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
            pbShader.Image = ImageScaler.FitHighQuality(Properties.Resources.retroshaderfixgui, pbShader.Size);
            pbSkip.Image = ImageScaler.FitHighQuality(Properties.Resources.exit, pbSkip.Size);
        }

        private void lblCredit_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            try { System.Diagnostics.Process.Start("https://github.com/EthanRDoesMC/RetroShaderFix"); }
            catch { /* no default browser available; ignore */ }
        }
    }
}
