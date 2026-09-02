using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NMSRetroInstaller;

/// <summary>
/// Runs everything the install does apart from the Steam download, against a throwaway folder.
/// A renamed embedded file or a broken patch config fails here instead of half way through
/// somebody's install. Run with <c>--selfcheck</c>; exit code 0 means everything passed.
/// </summary>
public static class SelfCheck
{
    public static int Run()
    {
        var failures = new List<string>();
        var temp = Path.Combine(Path.GetTempPath(), "nmsretro-selfcheck-" + Path.GetRandomFileName());

        void Check(string what, bool ok)
        {
            Console.Error.WriteLine((ok ? "  ok   " : "  FAIL ") + what);
            if (!ok) failures.Add(what);
        }

        try
        {
            Directory.CreateDirectory(temp);
            Console.Error.WriteLine("self-check in " + temp);

            // The catalog has to keep the versions apart, or they overwrite each other's saves.
            Check("catalog folders are unique", GameCatalog.All.Select(v => v.Folder).Distinct().Count() == GameCatalog.All.Count);
            Check("catalog save ids are unique", GameCatalog.All.Select(v => v.SaveId).Distinct().Count() == GameCatalog.All.Count);
            Check("catalog manifests are unique", GameCatalog.All.Select(v => v.Manifest).Distinct().Count() == GameCatalog.All.Count);

            // Installing one version, the bar has to read what the download log is printing.
            Check("one version: bar tracks the download exactly",
                InstallView.BarPercent(0, 1, 6425, 10000) == 64.25);
            Check("one version: bar ends at 100", InstallView.BarPercent(0, 1, 10, 10) == 100);
            Check("four versions: each owns a quarter", InstallView.BarPercent(2, 4, 1, 2) == 62.5);

            // Extras: the shortcut icons, the Discord icon and RetroShaderFix.
            var extras = Path.Combine(temp, "Extras");
            Payload.WriteFolder("InstallerExtras", extras);
            Check("RetroShaderFix.exe is bundled", File.Exists(Path.Combine(extras, "RetroShaderFix.exe")));
            Check("discord.ico is bundled", File.Exists(Path.Combine(extras, "discord.ico")));

            foreach (var version in GameCatalog.All)
            {
                Check($"{version.Title}: icon {version.Icon} is bundled", File.Exists(Path.Combine(extras, version.Icon)));

                // A stock install as the depot download leaves it.
                var binaries = GameCatalog.BinariesFolder(temp, version);
                Directory.CreateDirectory(binaries);
                Directory.CreateDirectory(Path.Combine(GameCatalog.GameFolder(temp, version), "GAMEDATA", "PCBANKS"));
                File.WriteAllText(Path.Combine(binaries, "steam_api64.dll"), "stock");
                File.WriteAllText(GameCatalog.ExePath(temp, version), "stub");

                Patcher.Apply(temp, version, "SteamPersonName", _ => { });

                var dll = Path.Combine(binaries, "steam_api64.dll");
                Check($"{version.Title}: original dll backed up", File.Exists(dll + ".bak"));
                Check($"{version.Title}: steam_api64.dll replaced", new FileInfo(dll).Length > 1024);

                var settings = File.ReadAllText(Path.Combine(binaries, "steam_api64.txt"));
                Check($"{version.Title}: steamid={version.SaveId}", settings.Contains($"steamid={version.SaveId}\r\n"));
                Check($"{version.Title}: name is the Steam spelling", settings.Contains("name=SteamPersonName\r\n"));
                Check($"{version.Title}: mod warning disabled", settings.Contains("disablemodwarning=true\r\n"));
                Check($"{version.Title}: discoveries rerouted",
                    settings.Contains("discoveriesserver=" + GameCatalog.DiscoveriesServer + "\r\n"));

                // Both vendors, so a missing pak shows up whichever card the user has.
                foreach (var gpu in new[] { Enums.GPU.AMD, Enums.GPU.NVIDIA })
                {
                    var result = ShaderFix.Apply(GameCatalog.GameFolder(temp, version), version.Update, gpu, _ => { });
                    Check($"{version.Title}/{gpu}: shader fix ran ({result})",
                        result.StartsWith("Shader fix applied") || result.StartsWith("No shader fix available"));
                }

                var shortcut = Path.Combine(temp, version.ShortcutName + ".lnk");
                Shortcuts.Create(shortcut, GameCatalog.ExePath(temp, version), icon: Path.Combine(extras, version.Icon));
                Check($"{version.Title}: shortcut written", File.Exists(shortcut));
            }

            Check("launcher sees every installed version", GameCatalog.Installed(temp).Count == GameCatalog.All.Count);
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("  FAIL threw " + ex);
            failures.Add(ex.Message);
        }
        finally
        {
            try { Directory.Delete(temp, recursive: true); } catch { }
        }

        Console.Error.WriteLine(failures.Count == 0
            ? "self-check passed"
            : $"self-check FAILED ({failures.Count})");

        return failures.Count == 0 ? 0 : 1;
    }
}
