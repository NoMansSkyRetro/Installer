using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Net;
using System.Text.RegularExpressions;

namespace NMSLegacyVersionInstaller.Steps
{
    public partial class ExtractTemporaryFiles : UserControl
    {
        // Fallback URL if we can't scrape the direct download link
        private const string DotNet9DownloadPageUrl = "https://dotnet.microsoft.com/en-us/download/dotnet/9.0";

        public ExtractTemporaryFiles()
        {
            InitializeComponent();
        }

        private void ExtractTemporaryFiles_Load(object sender, EventArgs e)
        {
            Program.TempFileLocation = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
            Program.ExtractInstallerFiles("NMSLegacyVersionInstaller.InstallerFiles.", Program.TempFileLocation);
            Program.ExtractInstallerFiles("NMSLegacyVersionInstaller.InstallerFilesPlugins.", Path.Combine(Program.TempFileLocation,"Plugins"));

            // Verify DepotDownloader Will Work
            string dnOutput = string.Empty;

            using (Process process = new Process())
            {
                process.StartInfo.FileName = "dotnet";
                process.StartInfo.Arguments = "--info";
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;

                process.OutputDataReceived += (s, p) => dnOutput += p.Data + Environment.NewLine;
                process.ErrorDataReceived += (s, p) => dnOutput += p.Data + Environment.NewLine;

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                process.WaitForExit();
            }    

            // DepotDownloader (net9.0 console app) needs Microsoft.NETCore.App (the base .NET Runtime),
            // which is what the download link below installs. Desktop Runtime also works but isn't required.
            if (!dnOutput.Contains("Microsoft.NETCore.App 9"))
            {
                // Lookup .NET 9.0 Windows x64 runtime download link
                string downloadUrl = TryGetDotNet9RuntimeDownloadUrl();

                string message = "This program requires .NET 9.0 Runtime to run, but it was not found on your system." + Environment.NewLine;
  
                if (!string.IsNullOrEmpty(downloadUrl))
                {
                    message += "-------------" + Environment.NewLine;
                    message += "Do you wish to download and install the .NET 9.0 Runtime (Windows x64) now?" + Environment.NewLine;
                    message += Environment.NewLine + "Download link:" + Environment.NewLine + downloadUrl;

                    var output = MessageBox.Show(message, ".NET 9.0 Runtime Required", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (output == DialogResult.Yes)
                    {
                        Process.Start(downloadUrl);
                    }
                }
                else
                {
                    // Fallback: send the user to the download page to find the runtime manually
                    message += "-------------" + Environment.NewLine;
                    message += "Do you wish to open the .NET 9.0 download page in your browser?" + Environment.NewLine;
                    message += Environment.NewLine + "Page: " + DotNet9DownloadPageUrl + Environment.NewLine;
                    message += "Look for '.NET Runtime' → Windows → x64 Installer.";

                    var output = MessageBox.Show(message, ".NET 9.0 Runtime Required", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                    if (output == DialogResult.Yes)
                    {
                        Process.Start(DotNet9DownloadPageUrl);
                    }
                }
                Program.Container.Close();
            }


            Program.Container.Next();
        }

        private string TryGetDotNet9RuntimeDownloadUrl()
        {
            try
            {
                // Step 1: Scrape the download page to find the latest runtime x64 installer thank-you page URL.
                string downloadPageHtml = DownloadString(DotNet9DownloadPageUrl);
                if (string.IsNullOrEmpty(downloadPageHtml))
                    return null;

                // The download page has links like:
                //   href="/en-us/download/dotnet/thank-you/runtime-9.0.19-windows-x64-installer"
                // We need to match "runtime-VERSION-windows-x64-installer" but NOT "runtime-aspnetcore" or "runtime-desktop"
                // The href contains just the path; the link text is "x64".
                // We match the first occurrence (which is the latest version, since the page lists newest first).
                string thankYouUrl = ExtractFirstRuntimeThankYouUrl(downloadPageHtml);
                if (string.IsNullOrEmpty(thankYouUrl))
                    return null;

                // Step 2: Scrape the thank-you page to find the direct .exe download link.
                string thankYouPageHtml = DownloadString(thankYouUrl);
                if (string.IsNullOrEmpty(thankYouPageHtml))
                    return null;

                // The thank-you page has a direct link like:
                //   https://builds.dotnet.microsoft.com/dotnet/Runtime/9.0.19/dotnet-runtime-9.0.19-win-x64.exe
                string directUrl = ExtractDirectDownloadLink(thankYouPageHtml);
                return directUrl;
            }
            catch
            {
                // If anything goes wrong (network error, parsing error, etc.), return null to trigger fallback.
                return null;
            }
        }

        /// <summary>
        /// Finds the first (latest) .NET Runtime x64 installer thank-you page URL from the download page HTML.
        /// Excludes ASP.NET Core and Desktop Runtime links.
        /// </summary>
        private string ExtractFirstRuntimeThankYouUrl(string html)
        {
            // Match: thank-you/runtime-VERSION-windows-x64-installer
            // This pattern intentionally excludes "runtime-aspnetcore" and "runtime-desktop" by requiring
            // "runtime-" immediately followed by a version number.
            string pattern = @"thank-you/runtime-(\d+\.\d+\.\d+)-windows-x64-installer";
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return "https://dotnet.microsoft.com/en-us/download/dotnet/" + match.Value;
            }
            return null;
        }

        /// <summary>
        /// Extracts the direct download .exe URL from a thank-you page's HTML.
        /// Looks for links to builds.dotnet.microsoft.com containing .exe files.
        /// </summary>
        private string ExtractDirectDownloadLink(string html)
        {
            // The direct download link is on builds.dotnet.microsoft.com and ends with .exe
            string pattern = @"https?://builds\.dotnet\.microsoft\.com/[^\s""'<>]+\.exe";
            var match = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (match.Success)
            {
                return match.Value;
            }
            return null;
        }

        /// <summary>
        /// Downloads a string from a URL using WebClient. Returns null on failure.
        /// Uses TLS 1.2 which is required by the Microsoft download site.
        /// </summary>
        private string DownloadString(string url)
        {
            try
            {
                // Ensure TLS 1.2 is enabled (required by modern HTTPS sites)
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                using (WebClient client = new WebClient())
                {
                    // Set a realistic user agent to avoid being blocked
                    client.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36");
                    return client.DownloadString(url);
                }
            }
            catch
            {
                return null;
            }
        }
    }
}