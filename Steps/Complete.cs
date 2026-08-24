using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace NMSLegacyVersionInstaller.Steps
{
    public partial class Complete : UserControl
    {
        public Complete()
        {
            InitializeComponent();
        }
        string extras;
        private void Complete_Load(object sender, EventArgs e)
        {
            var depotDownloader = NMSLegacyVersionInstaller.Container.FindStep<DepotDownloader>();
            extras = Path.Combine(depotDownloader.InstallationPath, "Extras");

            // SmartSaveFolder only applies when using a real Steam ID (shared save folder).
            if (NMSLegacyVersionInstaller.Container.FindStep<SaveGameStep>().rbVersionId.Checked)
                panel2.Visible = false;
        }

        private void btnOpen_Click(object sender, EventArgs e)
        {
            var depotdownloader = NMSLegacyVersionInstaller.Container.FindStep<DepotDownloader>();
            
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = "explorer.exe";
            startInfo.Arguments = depotdownloader.InstallationPath;

            // Start the process.
            Process.Start(startInfo);

        }

        private void btnDiscord_Click(object sender, EventArgs e)
        {
            Process.Start("https://discord.gg/YcQ8Aq2FA6");
        }

        private void btnSmartSaveFolder_Click(object sender, EventArgs e)
        {
            var smartSaveFolder = Path.Combine(extras, "SmartSaveFolder.exe");
            Process.Start(smartSaveFolder);
        }
    }
}
