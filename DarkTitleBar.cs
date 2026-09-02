using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using Microsoft.Win32;

namespace NMSRetroInstaller;

/// <summary>
/// Gives a window the dark title bar when Windows is set to dark mode, which is what every other
/// dark-themed desktop app does. Without it a white caption bar sits on top of the installer's
/// near-black chrome.
/// </summary>
public static class DarkTitleBar
{
    // DWMWA_USE_IMMERSIVE_DARK_MODE. It was 19 before Windows 10 20H1 and 20 from then on, and
    // asking for the wrong one is a harmless non-zero return, so try the current one first.
    const int UseImmersiveDarkMode = 20;
    const int UseImmersiveDarkModeBefore20H1 = 19;

    [DllImport("dwmapi.dll")]
    static extern int DwmSetWindowAttribute(IntPtr hwnd, int attribute, ref int value, int size);

    /// <summary>Applies it once the window has an HWND to set it on.</summary>
    public static void Apply(Window window) => window.SourceInitialized += (_, _) =>
    {
        if (!WindowsIsInDarkMode())
            return;

        var hwnd = new WindowInteropHelper(window).Handle;
        if (hwnd == IntPtr.Zero)
            return;

        var on = 1;
        if (DwmSetWindowAttribute(hwnd, UseImmersiveDarkMode, ref on, sizeof(int)) != 0)
            DwmSetWindowAttribute(hwnd, UseImmersiveDarkModeBefore20H1, ref on, sizeof(int));
    };

    // ponytail: read once at startup, not watched. The installer is a short sit-down job, and
    // following a live theme switch would mean an appearance-change hook for nothing.
    static bool WindowsIsInDarkMode() =>
        Registry.GetValue(
            @"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Themes\Personalize",
            "AppsUseLightTheme", 1) is int light && light == 0;
}
