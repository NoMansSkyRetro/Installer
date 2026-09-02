using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NMSRetroInstaller.Steam;

namespace NMSRetroInstaller;

/// <summary>
/// Shell for the installer. Owns the scaled board, the step views on it, and the busy overlay;
/// each step keeps its own layout and animation to itself.
/// </summary>
public partial class MainWindow : Window
{
    readonly CancellationTokenSource closing = new();
    SteamSession? session;
    bool signingIn;

    public MainWindow()
    {
        InitializeComponent();
        DarkTitleBar.Apply(this);
        Intro.LoginRequested += OnLoginRequested;
        Versions.InstallRequested += OnInstallRequested;
        Loaded += (_, _) => { Show(Intro); Intro.Play(); };
        Closed += (_, _) =>
        {
            closing.Cancel();
            session?.Dispose();
        };
    }

    /// <summary>Signs in to Steam, then moves on to the version picker.</summary>
    async void OnLoginRequested(object? sender, EventArgs e)
    {
        // The overlay swallows the mouse but not the keyboard, and Enter in the password box
        // raises this - without the guard a second sign-in starts on top of the first.
        if (signingIn) return;
        signingIn = true;

        Intro.ClearError();
        Busy.Show("CONNECTING TO STEAM");

        try
        {
            session = await SteamSession.LoginAsync(
                Intro.Username,
                Intro.Password,
                new SteamGuardPrompt(Guard, status => Busy.StatusText = status),
                line => Busy.StatusText = line.TrimEnd('.').ToUpperInvariant(),
                closing.Token);
        }
        catch (OperationCanceledException)
        {
            signingIn = false;
            await Busy.HideAsync();
            return;
        }
        catch (Exception ex)
        {
            signingIn = false;
            await Busy.HideAsync();
            Intro.ShowError(ex is SteamException or TimeoutException
                ? ex.Message
                : "Could not sign in to Steam: " + ex.Message);
            return;
        }

        Busy.StatusText = "LOADING VERSIONS";
        Show(Versions);
        Versions.Play();

        await Busy.HideAsync();
    }

    /// <summary>
    /// Runs the install, then hands the picked versions to the completion step. The shader fix and
    /// the Steam patch are part of the run, not choices, so there is nothing to ask in between.
    /// </summary>
    async void OnInstallRequested(object? sender, EventArgs e)
    {
        if (session is null) return;

        var picked = Versions.Selected;
        string root = Versions.InstallPath;

        Show(Install);
        Install.Play();

        if (!await Install.RunAsync(session, picked, root, closing.Token))
            return;   // the log on screen says what went wrong; leave it up

        Show(Complete);
        Complete.Play(picked, root);
    }

    /// <summary>Only one step is on the board at a time.</summary>
    void Show(UserControl step)
    {
        foreach (UserControl child in Stage.Children)
            child.Visibility = child == step ? Visibility.Visible : Visibility.Collapsed;
    }
}
