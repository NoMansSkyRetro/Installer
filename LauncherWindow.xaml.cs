using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Animation;

namespace NMSRetroInstaller;

/// <summary>
/// Standalone launcher, meant to sit in the install root next to the version folders. It is the
/// installer's completion step on its own: same header, same <see cref="LaunchPad"/>.
/// </summary>
public partial class LauncherWindow : Window
{
    readonly string? forcedRoot;

    public LauncherWindow() : this(null) { }

    /// <param name="root">Install folder to show. Null means work it out from where we are.</param>
    public LauncherWindow(string? root)
    {
        InitializeComponent();
        DarkTitleBar.Apply(this);
        forcedRoot = root;
        Loaded += (_, _) => Load();
    }

    void Load()
    {
        string root = forcedRoot ?? InstallRoot();
        var installed = GameCatalog.Installed(root);

        Header.Subtitle = installed.Count > 0
            ? $"{installed.Count} version(s) ready. Pick one to play."
            : $"Nothing installed under {root}. Put this launcher in your install folder.";

        Pad.Load(installed, root);

        var sb = new Storyboard();
        Header.Play(sb, 0.00);
        Pad.Play(sb, 0.30);
        sb.Begin(this, isControllable: true);
    }

    /// <summary>
    /// A folder given on the command line, otherwise wherever the launcher is sitting - which is
    /// the install root once the installer has copied it there.
    /// </summary>
    static string InstallRoot()
    {
        foreach (var arg in Environment.GetCommandLineArgs()[1..])
            if (!arg.StartsWith('-') && Directory.Exists(arg))
                return arg;

        // Not AppContext.BaseDirectory: for a single-file build that is the extraction folder.
        return Path.GetDirectoryName(Environment.ProcessPath) ?? Environment.CurrentDirectory;
    }
}
