using System;
using System.IO;
using System.Runtime.InteropServices;

namespace NMSRetroInstaller;

/// <summary>Writes .lnk and .url shortcuts, through the Windows Script Host shell object.</summary>
public static class Shortcuts
{
    /// <summary>Start menu folder the launcher shortcut goes into.</summary>
    public const string StartMenuFolder = "No Man's Sky Retro";

    public static void Create(
        string shortcutPath, string target,
        string arguments = "", string icon = "", string workingDirectory = "")
    {
        // Late-bound COM: no interop assembly to reference, and nothing to break at publish time.
        var shellType = Type.GetTypeFromProgID("WScript.Shell")
            ?? throw new InvalidOperationException("Windows Script Host is not available.");

        dynamic shell = Activator.CreateInstance(shellType)!;
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(shortcutPath)!);

            dynamic shortcut = shell.CreateShortcut(shortcutPath);
            shortcut.TargetPath = target;
            shortcut.Arguments = arguments;
            shortcut.WorkingDirectory = string.IsNullOrEmpty(workingDirectory)
                ? Path.GetDirectoryName(target)
                : workingDirectory;

            if (!string.IsNullOrEmpty(icon))
                shortcut.IconLocation = icon;

            shortcut.Save();
            Marshal.FinalReleaseComObject(shortcut);
        }
        finally
        {
            Marshal.FinalReleaseComObject(shell);
        }
    }

    /// <summary>An internet shortcut, which opens in the browser rather than launching a program.</summary>
    public static void CreateUrl(string shortcutPath, string url, string icon = "")
    {
        var body = $"[InternetShortcut]{Environment.NewLine}URL={url}{Environment.NewLine}";
        if (!string.IsNullOrEmpty(icon))
            body += $"IconFile={icon}{Environment.NewLine}IconIndex=0{Environment.NewLine}";

        File.WriteAllText(shortcutPath, body);
    }

    public static string Desktop(string name) =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory), name + ".lnk");

    public static string StartMenu(string name) => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Programs), StartMenuFolder, name + ".lnk");
}
