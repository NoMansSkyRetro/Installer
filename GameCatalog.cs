using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace NMSRetroInstaller;

/// <summary>One installable build of the game.</summary>
/// <param name="Folder">Folder it installs into, under the chosen root.</param>
/// <param name="Title">Update name, as shown on the card.</param>
/// <param name="Build">Version label.</param>
/// <param name="CoverAsset">Base filename of the cover art.</param>
/// <param name="Manifest">Steam manifest ID of this build in depot 275851.</param>
/// <param name="SaveId">
/// Value written as <c>steamid</c> in steam_api64.txt. The game only uses it to name its save
/// folder, so giving each build its own keeps their saves apart.
/// </param>
/// <param name="ShortcutName">Name of the desktop-style shortcut written into the install root.</param>
/// <param name="Icon">Embedded icon used for that shortcut.</param>
/// <param name="Update">Which shader fix applies to this build.</param>
public sealed record GameVersion(
    string Folder,
    string Title,
    string Build,
    string CoverAsset,
    ulong Manifest,
    uint SaveId,
    string ShortcutName,
    string Icon,
    Enums.Update Update);

/// <summary>
/// The four versions the installer knows about. One list, shared by the picker, the install run,
/// the completion screen and the launcher - folder names have to agree across all of them or the
/// launcher will not find what the installer wrote.
/// </summary>
public static class GameCatalog
{
    public const string Discord = "https://discord.gg/YcQ8Aq2FA6";
    public const string Website = "https://nomansskyretro.com";

    /// <summary>Where the patched game sends its discoveries traffic instead of Hello Games.</summary>
    public const string DiscoveriesServer = "https://discoveries.nomansskyretro.com";

    public const uint AppId = 275850;
    public const uint DepotId = 275851;

    public static readonly IReadOnlyList<GameVersion> All =
    [
        new("no_mans_sky_v1.09.1", "INITIAL RELEASE", "VERSION 1.09", "01_release",
            7324577403707723494, 109, "No Man's Sky Initial Release", "01_icon.ico", Enums.Update.Release),
        new("no_mans_sky_v1.13", "FOUNDATION", "VERSION 1.13", "02_foundation",
            2123008115602074603, 113, "No Man's Sky Foundation", "02_icon.ico", Enums.Update.Foundation),
        new("no_mans_sky_v1.24", "PATH FINDER", "VERSION 1.24", "03_pathfinder",
            3749359456608052294, 124, "No Man's Sky Path Finder", "03_icon.ico", Enums.Update.PathFinder),
        new("no_mans_sky_v1.38", "ATLAS RISES", "VERSION 1.38", "04_atlasrises",
            8262658978126728861, 138, "No Man's Sky Atlas Rises", "04_icon.ico", Enums.Update.AtlasRises),
    ];

    public static ImageSource Cover(GameVersion version) => new BitmapImage(
        new Uri($"pack://application:,,,/Resources/covers/{version.CoverAsset}.png"));

    public static string GameFolder(string root, GameVersion version) =>
        Path.Combine(root, version.Folder);

    public static string BinariesFolder(string root, GameVersion version) =>
        Path.Combine(GameFolder(root, version), "Binaries");

    public static string ExePath(string root, GameVersion version) =>
        Path.Combine(BinariesFolder(root, version), "NMS.exe");

    /// <summary>The versions actually sitting under an install root.</summary>
    public static IReadOnlyList<GameVersion> Installed(string root) =>
        All.Where(v => File.Exists(ExePath(root, v))).ToArray();
}
