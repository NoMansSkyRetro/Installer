using System;
using System.IO;
using System.Windows;

namespace NMSRetroInstaller;

/// <summary>
/// One executable, two faces. Run normally it is the installer; run as the copy the installer
/// leaves in the install folder - by name or with <c>--launcher</c> - it is the launcher.
/// <para>
/// They share the whole UI, so shipping them as two self-contained executables would have meant
/// two copies of the .NET runtime in one download.
/// </para>
/// </summary>
public partial class App : Application
{
    /// <summary>Filename the installer copies itself to, and what the shortcuts point at.</summary>
    public const string LauncherName = "No Man's Sky Retro Launcher.exe";

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        if (Array.Exists(e.Args, a => string.Equals(a, "--selfcheck", StringComparison.OrdinalIgnoreCase)))
        {
            Shutdown(SelfCheck.Run());
            return;
        }

        Window window = IsLauncher(e.Args) ? new LauncherWindow() : new MainWindow();
        MainWindow = window;
        window.Show();
    }

    static bool IsLauncher(string[] args)
    {
        foreach (var arg in args)
            if (string.Equals(arg, "--launcher", StringComparison.OrdinalIgnoreCase))
                return true;

        var exe = Path.GetFileName(Environment.ProcessPath) ?? "";
        return exe.Contains("Launcher", StringComparison.OrdinalIgnoreCase);
    }
}
