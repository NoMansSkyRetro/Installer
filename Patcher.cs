using System;
using System.IO;

namespace NMSRetroInstaller;

/// <summary>
/// Makes a downloaded build run without Steam, by dropping in steam_api64.retro - a replacement
/// steam_api64.dll that answers the Steamworks calls these builds make and unwraps the SteamStub
/// DRM in memory as the game loads.
/// <para>
/// This is the whole patch. The Goldberg emulator, Steamless and the rewritten NMS.exe the old
/// installer left behind are all gone; NMS.exe is never touched.
/// </para>
/// </summary>
public static class Patcher
{
    public static void Apply(
        string root, GameVersion version, string accountName, GameLanguage language, Action<string> log)
    {
        var binaries = GameCatalog.BinariesFolder(root, version);
        var dll = Path.Combine(binaries, "steam_api64.dll");
        var backup = dll + ".bak";

        if (File.Exists(dll) && !File.Exists(backup))
        {
            File.Move(dll, backup);
            log("Kept the original steam_api64.dll as steam_api64.dll.bak");
        }

        Payload.Write("InstallerFiles.steam_api64.dll", dll);
        log("Installed steam_api64.retro");

        File.WriteAllText(Path.Combine(binaries, "steam_api64.txt"), Settings(version, accountName, language));
        log($"Wrote steam_api64.txt (steamid {version.SaveId}, saves in st_{version.SaveId}, language {language.Code})");
    }

    /// <summary>
    /// The DLL writes this file itself on first run; writing it here instead pins the save folder
    /// to the build, names the account the way Steam spells it, and turns on the two extras.
    /// </summary>
    static string Settings(GameVersion version, string accountName, GameLanguage language) =>
        $"""
        steamid={version.SaveId}
        name={accountName}
        language={language.Code}
        # true skips the mods-enabled warning screen at boot (1.13 and later)
        disablemodwarning=true
        # http://host:port or https://host sends the discoveries traffic to that server instead of Hello Games
        discoveriesserver={GameCatalog.DiscoveriesServer}

        """.ReplaceLineEndings("\r\n");
}
