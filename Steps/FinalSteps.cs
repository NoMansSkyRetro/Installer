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
using System.Threading;
using System.Diagnostics;

namespace NMSLegacyVersionInstaller.Steps
{
    public partial class FinalSteps : UserControl
    {
        public FinalSteps()
        {
            InitializeComponent();
        }
        private LogBox console;
        public int currentCommandIndex = 0;
        public DepotDownloader depotDownloader;
        public string extras;
        public bool useVersionId;
        public string realSteamId;   // SteamID64 picked on the SaveGameStep (empty = emulator default)
        public bool autoShaders;
        public Enums.GPU gpu;

        private void FinalSteps_Load(object sender, EventArgs e)
        {
            // Log Set Up (same LogBox component as the download step)
            console = new LogBox { Dock = System.Windows.Forms.DockStyle.Fill };
            pnlConsole.Controls.Add(console);

            Program.Container.SetStepsEnabled(false);
            depotDownloader = NMSLegacyVersionInstaller.Container.FindStep<DepotDownloader>();

            useVersionId = NMSLegacyVersionInstaller.Container.FindStep<SaveGameStep>().rbVersionId.Checked;
            autoShaders = NMSLegacyVersionInstaller.Container.FindStep<ShaderFixStep>().rbShaderAuto.Checked;

            // Extras
            extras = Path.Combine(depotDownloader.InstallationPath, "Extras");
            console.AppendLine("Extracting Extras..." + Environment.NewLine, Color.Lime);
            Program.ExtractInstallerFiles("NMSLegacyVersionInstaller.InstallerExtras.", extras);
            Program.CreateShortcutWithIcon(Path.Combine(depotDownloader.InstallationPath, "RetroShaderFix.lnk"), Path.Combine(extras, "RetroShaderFix.exe"), "", depotDownloader.InstallationPath);

            // Discord invite: a .url internet shortcut (opens in the browser) with the bundled discord.ico.
            File.WriteAllText(Path.Combine(depotDownloader.InstallationPath, "No Man's Sky Retro Discord.url"),
                "[InternetShortcut]" + Environment.NewLine +
                "URL=https://discord.gg/YcQ8Aq2FA6" + Environment.NewLine +
                "IconFile=" + Path.Combine(extras, "discord.ico") + Environment.NewLine +
                "IconIndex=0" + Environment.NewLine);

            if (useVersionId)
            {
                // Each version gets its own dummy Steam ID, so their save folders never collide - no SmartSaveFolder needed.
                console.AppendLine("Using version-based save folders (unique Steam ID per version)." + Environment.NewLine, Color.Lime);
            }
            else
            {
                // Real Steam ID: all versions share one save folder, so ship SmartSaveFolder to switch between them.
                Program.CreateShortcutWithIcon(Path.Combine(depotDownloader.InstallationPath, "SmartSaveFolder.lnk"), Path.Combine(extras, "SmartSaveFolder.exe"));
                realSteamId = NMSLegacyVersionInstaller.Container.FindStep<SaveGameStep>().SelectedSteamId;
                if (!string.IsNullOrEmpty(realSteamId))
                    console.AppendLine("Save folder Steam ID: " + realSteamId + Environment.NewLine, Color.Lime);
                else
                    console.AppendLine("No Steam account detected - using the emulator's default user." + Environment.NewLine, Color.Orange);
            }

            if (autoShaders)
            {
                gpu = ShaderFix.DetectGPU(s => console.AppendLine(s + Environment.NewLine, Color.Silver));
                console.AppendLine("Shader fix enabled - detected GPU vendor: " + gpu + Environment.NewLine, Color.Lime);
            }

            // Start Task
            currentCommandIndex = 0;
            BeforeUnpack();
        }

        private void BeforeUnpack()
        {
            BeginInvoke((MethodInvoker)(() =>
            { // Threadsafe
            var thisCommand = depotDownloader.DepotDownloaderCommands[currentCommandIndex];
            var binaries = Path.Combine(thisCommand.folder, "Binaries");
            var NMSexePath = Path.Combine(binaries, "NMS.exe");

            console.AppendLine("Processing " + Path.GetFileName(thisCommand.folder) + Environment.NewLine, Color.Lime);

            // Steam Emulator
            console.AppendLine("[Goldberg Steam Emulator] Replace steam_api64.dll..." + Environment.NewLine, Color.Orange);
            if(!File.Exists(Path.Combine(binaries, "steam_api64.dll.bak")))
                File.Move(Path.Combine(binaries, "steam_api64.dll"), Path.Combine(binaries, "steam_api64.dll.bak"));
            File.Copy(Path.Combine(Program.TempFileLocation, "steam_api64.dll"), Path.Combine(binaries, "steam_api64.dll"), true);
            
            // Steam Emulator User Configuration

            // Fix based on
            // https://gitlab.com/Mr_Goldberg/goldberg_emulator/-/blob/master/Readme_release.txt
            // "You can also make the emu ignore certain global settings by using a force_account_name.txt, force_language.txt, force_listen_port.txt or force_steamid.txt that you put in the <path where my emu lib is>\steam_settings\ folder."

            console.AppendLine("[Goldberg Steam Emulator] Configure User..." + Environment.NewLine, Color.Orange);
            Directory.CreateDirectory(Path.Combine(binaries, "steam_settings"));
            File.WriteAllText(Path.Combine(binaries, "steam_settings", "force_account_name.txt"), Steam.Username);
            string steamId = useVersionId ? Steam.DummySteamID(thisCommand.update) : realSteamId;
            if (!string.IsNullOrEmpty(steamId))
            {
                File.WriteAllText(Path.Combine(binaries, "steam_settings", "force_steamid.txt"), steamId);
                console.AppendLine("[Goldberg Steam Emulator] Save folder Steam ID: " + steamId + Environment.NewLine, Color.Orange);
            }

            // Steam Emulator Offline Mode
            console.AppendLine("[Goldberg Steam Emulator] Enable Offline Mode..." + Environment.NewLine, Color.Orange);            
            File.WriteAllText(Path.Combine(binaries, "steam_settings", "offline.txt"),"");

            // Steamless
            console.AppendLine("[Steamless] Running Steamless..." + Environment.NewLine, Color.Orange);
            RunSteamless(NMSexePath);
            }));
        }

        // Runs Steamless.CLI on a background thread, piping its output to the log, then calls AfterUnpack on exit.
        private void RunSteamless(string nmsExePath)
        {
            new Thread(() =>
            {
                Thread.Sleep(3000); // let the steam_api64 swap / file moves settle before Steamless reads NMS.exe
                try
                {
                    var p = new Process
                    {
                        StartInfo = new ProcessStartInfo
                        {
                            FileName = Path.Combine(Program.TempFileLocation, "Steamless.CLI.exe"),
                            Arguments = "\"" + nmsExePath + "\"",
                            WorkingDirectory = Program.TempFileLocation,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        }
                    };
                    p.OutputDataReceived += (s, ev) => { if (ev.Data != null) console.AppendLine(ev.Data); };
                    p.ErrorDataReceived += (s, ev) => { if (ev.Data != null) console.AppendLine(ev.Data); };
                    p.Start();
                    p.BeginOutputReadLine();
                    p.BeginErrorReadLine();
                    p.WaitForExit();
                }
                catch (Exception ex)
                {
                    console.AppendLine("[Steamless] Error: " + ex.Message, Color.Red);
                }
                BeginInvoke((MethodInvoker)(() => AfterUnpack()));
            })
            { IsBackground = true }.Start();
        }

        private void AfterUnpack()
        {
            BeginInvoke((MethodInvoker)(() =>
            { // Threadsafe
                var thisCommand = depotDownloader.DepotDownloaderCommands[currentCommandIndex];

                // After Steamless
                var binaries = Path.Combine(thisCommand.folder, "Binaries");
                var NMSexePath = Path.Combine(binaries, "NMS.exe");
                var unpackedNMSexePath = Path.Combine(binaries, "NMS.exe.unpacked.exe");

                if (File.Exists(unpackedNMSexePath))
                {
                    console.AppendLine("[Steamless] Moving Unpacked File..." + Environment.NewLine, Color.Orange);
                    File.Move(NMSexePath, NMSexePath + ".bak");
                    File.Move(unpackedNMSexePath, NMSexePath);
                }

                console.AppendLine("Creating Shortcut..." + Environment.NewLine, Color.Orange);
                var iconPath = Path.Combine(extras, thisCommand.icon);
                Program.CreateShortcutWithIcon(Path.Combine(depotDownloader.InstallationPath, thisCommand.name + ".lnk"), NMSexePath, iconPath);

                if (autoShaders)
                {
                    try
                    {
                        console.AppendLine("[RetroShaderFix] Applying shader fix (by Ethan)..." + Environment.NewLine, Color.Orange);
                        string result = ShaderFix.Apply(thisCommand.folder, thisCommand.update, gpu,
                            s => console.AppendLine("[RetroShaderFix] " + s + Environment.NewLine, Color.Orange));
                        console.AppendLine("[RetroShaderFix] " + result + Environment.NewLine, Color.Lime);
                    }
                    catch (Exception ex)
                    {
                        console.AppendLine("[RetroShaderFix] Error: " + ex.Message + Environment.NewLine, Color.Red);
                    }
                }

                if (currentCommandIndex < depotDownloader.DepotDownloaderCommands.Count - 1)
                {
                    currentCommandIndex++;
                    BeforeUnpack(); // Next Unpack
                }
                else
                {
                    console.AppendLine("Complete" + Environment.NewLine, Color.Lime);
                    File.WriteAllText(Path.Combine(depotDownloader.InstallationLogPath, "02_FinalStepsLog-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt"), console.Text);

                    Program.Container.SetStepsEnabled(true);
                    Program.Container.Next();
                }
            }));
        }


    }
}
