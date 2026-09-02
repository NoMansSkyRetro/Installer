using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace NMSRetroInstaller;

/// <summary>
/// A row of installed games that start when clicked, plus the two links people want afterwards.
/// Shared by the installer's completion step and by the standalone launcher, so the games are
/// started the same way from both.
/// </summary>
public partial class LaunchPad : UserControl
{
    string root = "";

    public LaunchPad()
    {
        InitializeComponent();
        TextElement.SetFontFamily(this, LogoText.Analog);
    }

    /// <summary>Fills the pad with the given versions, all rooted at <paramref name="installRoot"/>.</summary>
    public void Load(IReadOnlyList<GameVersion> versions, string installRoot)
    {
        root = installRoot;
        Tiles.Children.Clear();

        foreach (var version in versions)
        {
            var tile = new GameTile
            {
                Cover = GameCatalog.Cover(version),
                Title = version.Title,
                Build = version.Build,
                Margin = new Thickness(9, 0, 9, 0),
                Tag = version,
                Opacity = 0,
            };
            tile.Click += (s, _) => Launch((GameVersion)((GameTile)s!).Tag);
            Tiles.Children.Add(tile);
        }

        Empty.Opacity = versions.Count == 0 ? 1 : 0;
        FolderButton.IsEnabled = Directory.Exists(installRoot);
    }

    /// <summary>Deals the tiles in left to right, then brings the footer up.</summary>
    public void Play(Storyboard sb, double begin)
    {
        for (int i = 0; i < Tiles.Children.Count; i++)
        {
            double t = begin + i * 0.12;
            var tile = (UIElement)Tiles.Children[i];
            Anim.Move(sb, tile, 0.94, 1, new Vector(0, 26), new Vector(), t, 0.55, Anim.Settle);
            Anim.FadeIn(sb, tile, t, 0.55);
        }
        Anim.Rise(sb, Footer, 14, begin + 0.35, 0.55);
    }

    void Launch(GameVersion version)
    {
        string exe = GameCatalog.ExePath(root, version);
        if (!File.Exists(exe)) return;

        // NMS.exe expects to start from its own Binaries folder.
        Process.Start(new ProcessStartInfo(exe)
        {
            WorkingDirectory = Path.GetDirectoryName(exe)!,
            UseShellExecute = true,
        });
    }

    void OnOpenFolder(object sender, RoutedEventArgs e)
    {
        if (Directory.Exists(root))
            Process.Start(new ProcessStartInfo(root) { UseShellExecute = true });
    }

    void OnDiscord(object sender, RoutedEventArgs e) => Open(GameCatalog.Discord);

    void OnWebsite(object sender, RoutedEventArgs e) => Open(GameCatalog.Website);

    static void Open(string url) =>
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
}
