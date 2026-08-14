using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using Microsoft.WindowsAPICodePack.Dialogs;

namespace NMSLegacyVersionInstaller.Steps
{
    public partial class SelectVersion : UserControl
    {
        public SelectVersion()
        {
            InitializeComponent();
        }

        private void SelectVersion_Load(object sender, EventArgs e)
        {

        }

        private void btnBrowse_Click(object sender, EventArgs e)
        {
            string currentPath = txtPath.Text;
            // If the path doesn't exist, walk up to find a parent that does
            string initialPath = currentPath;
            while (!string.IsNullOrEmpty(initialPath) && !System.IO.Directory.Exists(initialPath))
            {
                try
                {
                    initialPath = System.IO.Path.GetDirectoryName(initialPath.TrimEnd(System.IO.Path.DirectorySeparatorChar, System.IO.Path.AltDirectorySeparatorChar));
                }
                catch
                {
                    initialPath = null;
                    break;
                }
            }

            // Pick Path
            CommonOpenFileDialog dialog = new CommonOpenFileDialog();
            dialog.InitialDirectory = initialPath;
            dialog.IsFolderPicker = true;
            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
                txtPath.Text = dialog.FileName + "\\";
 
        }
    }
}