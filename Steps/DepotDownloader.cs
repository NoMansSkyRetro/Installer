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
using System.Threading;

namespace NMSLegacyVersionInstaller.Steps
{
    public partial class DepotDownloader : UserControl, IStepEnter
    {
        public class DepotDownloaderCommand
        {
            public DepotDownloaderCommand(string folder, string manifest, string name, string icon, ShaderFix.Update update)
            {
                this.folder = folder;
                this.manifest = manifest;
                this.name = name;
                this.icon = icon;
                this.update = update;
            }
            public string folder { get; set; }
            public string manifest { get; set; }
            public string name { get; set; }   // Used on final step
            public string icon { get; set; }   // Used on final step
            public ShaderFix.Update update { get; set; } // Used for shader fix + save-folder id
        }

        public string DepotDownloaderPath;
        public List<DepotDownloaderCommand> DepotDownloaderCommands;
        public string InstallationPath;
        public string InstallationLogPath;

        private int cmdIndex;
        private int attempt;
        private bool sawError;
        private int lastExitCode;
        private bool aborted;
        private Process proc;
        private StreamWriter procInput;

        private bool uiBuilt;
        private bool downloadStarted;

        // Clean progress UI (replaces the raw interactive console - less janky, more credible)
        private Label lblStatus;
        private ProgressBar progressBar;
        private LogBox txtLog;

        public DepotDownloader()
        {
            InitializeComponent();
        }

        // Called by the Container each time this becomes the current step. Deterministic - fires on the
        // first entry AND on re-entry after a login cancel (unlike Load, which only fires once).
        public void OnStepEnter()
        {
            if (downloadStarted)
                return; // never restart after a successful download
            BeginEntry();
        }

        private void BeginEntry()
        {
            if (!uiBuilt)
            {
                BuildProgressUI();
                uiBuilt = true;
            }
            else
            {
                txtLog.Clear();
                SetProgress(0);
                SetStatus("Preparing download...");
            }

            PrepareCommands();

            // Keep Next/Back disabled for the whole download - only re-enabled when it finishes.
            Program.Container.SetStepsEnabled(false);

            // Show the Steam login popup right before the download starts (deferred so navigation finishes first).
            Program.Container.BeginInvoke((MethodInvoker)(() =>
            {
                bool ok;
                using (var login = new SteamLoginForm())
                    ok = login.ShowDialog(Program.Container) == DialogResult.OK;

                if (!ok)
                {
                    // User cancelled login - return to version select. Login re-shows if they proceed again.
                    Program.Container.SetStepsEnabled(true);
                    Program.Container.Back();
                    return;
                }

                downloadStarted = true;
                AppendLog("Logging in to Steam as " + Steam.Username + "...");
                cmdIndex = 0;
                attempt = 1;
                RunCommand(0);
            }));
        }

        private void PrepareCommands()
        {
            var selectversion = NMSLegacyVersionInstaller.Container.FindStep<SelectVersion>();
            InstallationPath = selectversion.txtPath.Text;
            InstallationLogPath = Path.Combine(InstallationPath, "Log");

            if (!Directory.Exists(InstallationPath))
                Directory.CreateDirectory(InstallationPath);
            if (!Directory.Exists(InstallationLogPath))
                Directory.CreateDirectory(InstallationLogPath);

            DepotDownloaderPath = Path.Combine(Program.TempFileLocation, "DepotDownloader.exe");

            DepotDownloaderCommands = new List<DepotDownloaderCommand>();
            if (selectversion.rb01.Checked)
                DepotDownloaderCommands.Add(new DepotDownloaderCommand(
                    Path.Combine(InstallationPath, "no_mans_sky_v1.09.1"), "7324577403707723494",
                    "No Man's Sky Initial Release", "01_icon.ico", ShaderFix.Update.Release));
            if (selectversion.rb02.Checked)
                DepotDownloaderCommands.Add(new DepotDownloaderCommand(
                    Path.Combine(InstallationPath, "no_mans_sky_v1.13"), "2123008115602074603",
                    "No Man's Sky Foundation", "02_icon.ico", ShaderFix.Update.Foundation));
            if (selectversion.rb03.Checked)
                DepotDownloaderCommands.Add(new DepotDownloaderCommand(
                    Path.Combine(InstallationPath, "no_mans_sky_v1.24"), "3749359456608052294",
                    "No Man's Sky Path Finder", "03_icon.ico", ShaderFix.Update.PathFinder));
            if (selectversion.rb04.Checked)
                DepotDownloaderCommands.Add(new DepotDownloaderCommand(
                    Path.Combine(InstallationPath, "no_mans_sky_v1.38"), "8262658978126728861",
                    "No Man's Sky Atlas Rises", "04_icon.ico", ShaderFix.Update.AtlasRises));
        }

        private void BuildProgressUI()
        {
            lblStatus = new Label
            {
                Location = new Point(4, 4),
                Size = new Size(pnlConsole.Width - 8, 18),
                Text = "Preparing download..."
            };
            progressBar = new ProgressBar
            {
                Location = new Point(4, 26),
                Size = new Size(pnlConsole.Width - 8, 18),
                Minimum = 0,
                Maximum = 100,
                Style = ProgressBarStyle.Continuous
            };
            txtLog = new LogBox
            {
                Location = new Point(4, 50),
                Size = new Size(pnlConsole.Width - 8, pnlConsole.Height - 54)
            };
            pnlConsole.Controls.Add(lblStatus);
            pnlConsole.Controls.Add(progressBar);
            pnlConsole.Controls.Add(txtLog);
        }

        private string BuildArgs(DepotDownloaderCommand c)
        {
            return " -app 275850 -depot 275851 -manifest " + c.manifest +
                   " -dir \"" + c.folder + "\" -username " + Steam.Username +
                   " -password " + Steam.Password + " -remember-password";
        }

        private void RunCommand(int index)
        {
            if (index >= DepotDownloaderCommands.Count)
            {
                AppendLog("Complete");
                SetStatus("Download complete");
                SetProgress(100);
                File.WriteAllText(Path.Combine(InstallationLogPath, "01_DepotDownloaderLog-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt"), txtLog.Text);
                Program.Container.SetStepsEnabled(true);
                Program.Container.Next();
                return;
            }

            var cmd = DepotDownloaderCommands[index];
            cmdIndex = index;
            sawError = false;
            SetProgress(0);
            SetStatus("Downloading " + Path.GetFileName(cmd.folder) + (attempt > 1 ? " (retry " + attempt + ")" : ""));
            AppendLog("Downloading " + Path.GetFileName(cmd.folder));

            var t = new Thread(() =>
            {
                try
                {
                    proc = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = DepotDownloaderPath,
                            Arguments = BuildArgs(cmd),
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            RedirectStandardInput = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    proc.Start();
                    procInput = proc.StandardInput;

                    var outT = new Thread(() => ReadStream(proc.StandardOutput)) { IsBackground = true };
                    var errT = new Thread(() => ReadStream(proc.StandardError)) { IsBackground = true };
                    outT.Start();
                    errT.Start();

                    proc.WaitForExit();
                    outT.Join();
                    errT.Join();
                    lastExitCode = proc.ExitCode;
                }
                catch (Exception ex)
                {
                    lastExitCode = -1;
                    BeginInvoke((MethodInvoker)(() => AppendLog("Error: " + ex.Message)));
                }

                BeginInvoke((MethodInvoker)(() => AfterCommand(index)));
            })
            { IsBackground = true };
            t.Start();
        }

        private void AfterCommand(int index)
        {
            if (aborted)
            {
                FinishWithFailure("Download cancelled.");
                return;
            }

            bool failed = lastExitCode != 0 || sawError;
            if (failed)
            {
                if (attempt < 3)
                {
                    attempt++;
                    AppendLog("Error downloading, retrying...");
                    RunCommand(index);
                    return;
                }
                FinishWithFailure("Unable to download after 3 attempts. Check your credentials and connection, then restart the installer.");
                return;
            }

            attempt = 1;
            RunCommand(index + 1);
        }

        private void FinishWithFailure(string message)
        {
            SetStatus(message);
            AppendLog(message);
            SetProgress(0);
            Program.Container.SetStepsEnabled(true);
            Program.Container.btnNext.Text = "Finish";
            Program.Container.btnBack.Enabled = false;
        }

        // Reads a stream char-by-char, flushing a line on newline OR on ':' when the buffer looks like a
        // Steam prompt (Steam Guard prompts have no trailing newline). Ported from the Fractal413 approach.
        private void ReadStream(StreamReader stream)
        {
            var buffer = new StringBuilder();
            var chunk = new char[1024];
            int read;
            try
            {
                while ((read = stream.Read(chunk, 0, chunk.Length)) > 0)
                {
                    for (int i = 0; i < read; i++)
                    {
                        char c = chunk[i];
                        buffer.Append(c);
                        if (c == '\n' || (c == ':' && IsEnterPrompt(buffer.ToString())))
                        {
                            HandleLine(buffer.ToString());
                            buffer.Clear();
                        }
                    }
                }
                if (buffer.Length > 0)
                    HandleLine(buffer.ToString());
            }
            catch { }
        }

        private static bool IsEnterPrompt(string line)
        {
            string l = line.ToLower();
            return l.Contains("please enter") || l.TrimStart().StartsWith("enter");
        }

        // Runs on a reader thread.
        private void HandleLine(string line)
        {
            string trimmed = line.TrimEnd('\r', '\n');
            if (string.IsNullOrWhiteSpace(trimmed))
                return;

            if (trimmed.Contains(":") && IsEnterPrompt(trimmed))
            {
                // Steam is asking for a code - pop the Steam Guard dialog and feed the answer to stdin.
                string code = PromptForCode(trimmed);
                if (code == null)
                {
                    aborted = true;
                    TryKill();
                }
                else
                {
                    try { procInput.WriteLine(code); procInput.Flush(); } catch { }
                    // Restore the status - otherwise it stays stuck on "Steam Guard code required".
                    SetStatus("Downloading " + Path.GetFileName(DepotDownloaderCommands[cmdIndex].folder));
                }
                return;
            }

            if (trimmed.ToLower().Contains("error"))
                sawError = true;

            UpdateProgressFromLine(trimmed);
            AppendLog(trimmed);
        }

        // Blocks the reader thread until the user enters a code (or cancels).
        private string PromptForCode(string promptText)
        {
            string code = null;
            try
            {
                this.Invoke((MethodInvoker)(() =>
                {
                    SetStatus("Steam Guard code required");
                    using (var dlg = new SteamGuardForm(promptText))
                    {
                        if (dlg.ShowDialog(FindForm()) == DialogResult.OK)
                            code = dlg.Code;
                    }
                }));
            }
            catch { }
            return code;
        }

        private void TryKill()
        {
            try { if (proc != null && !proc.HasExited) proc.Kill(); } catch { }
        }

        private void UpdateProgressFromLine(string line)
        {
            int pct = line.IndexOf('%');
            if (pct <= 0) return;
            int start = pct - 1;
            while (start >= 0 && (char.IsDigit(line[start]) || line[start] == '.')) start--;
            string num = line.Substring(start + 1, pct - (start + 1));
            float val;
            if (float.TryParse(num, out val))
                SetProgress(val);
        }

        private void AppendLog(string text)
        {
            txtLog.AppendLine(text);
        }

        private void SetStatus(string text)
        {
            if (lblStatus.InvokeRequired) { BeginInvoke((MethodInvoker)(() => SetStatus(text))); return; }
            lblStatus.Text = text;
        }

        private void SetProgress(float value)
        {
            if (progressBar.InvokeRequired) { BeginInvoke((MethodInvoker)(() => SetProgress(value))); return; }
            int v = (int)Math.Round(value);
            if (v < 0) v = 0;
            if (v > 100) v = 100;
            progressBar.Value = v;
        }
    }
}
