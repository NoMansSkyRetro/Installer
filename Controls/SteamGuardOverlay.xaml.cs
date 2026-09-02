using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using SteamKit2.Authentication;

namespace NMSRetroInstaller;

/// <summary>
/// Asks for a Steam Guard code on the sign-in screen itself, in the same panel the rest of the
/// installer is built from, rather than opening a second window over it.
/// </summary>
public partial class SteamGuardOverlay : UserControl
{
    TaskCompletionSource<string?>? pending;

    public SteamGuardOverlay()
    {
        InitializeComponent();
        TextElement.SetFontFamily(this, LogoText.Analog);

        // The overlay is what has keyboard focus while it is up, so it can take these itself.
        KeyDown += (_, e) =>
        {
            if (e.Key == Key.Enter) { Accept(); e.Handled = true; }
            if (e.Key == Key.Escape) { Finish(null); e.Handled = true; }
        };
    }

    /// <summary>
    /// Raises the panel and completes with the code entered, or null if it was cancelled.
    /// Safe to call from the Steam threads: it hops to the UI thread itself.
    /// </summary>
    public Task<string?> AskAsync(string blurb, bool previousCodeWasWrong)
    {
        if (!Dispatcher.CheckAccess())
            return Dispatcher.InvokeAsync(() => AskAsync(blurb, previousCodeWasWrong)).Task.Unwrap();

        Blurb.Text = blurb;
        Warning.Visibility = previousCodeWasWrong ? Visibility.Visible : Visibility.Collapsed;
        CodeBox.Clear();

        Visibility = Visibility.Visible;
        pending = new TaskCompletionSource<string?>();

        var sb = new Storyboard();
        Anim.FadeIn(sb, this, 0.00, 0.25);
        Anim.Rise(sb, Panel, 16, 0.05, 0.35);
        sb.Begin(this);

        // After layout, not before: focusing an element the same tick it becomes visible does
        // nothing, and the code would then be typed into whatever had focus underneath.
        Dispatcher.BeginInvoke(DispatcherPriority.Input, () => Keyboard.Focus(CodeBox));

        return pending.Task;
    }

    void OnContinue(object sender, RoutedEventArgs e) => Accept();

    void OnCancel(object sender, RoutedEventArgs e) => Finish(null);

    void Accept()
    {
        var code = CodeBox.Text.Trim();
        if (code.Length > 0)
            Finish(code);
    }

    void Finish(string? code)
    {
        var waiting = pending;
        pending = null;

        BeginAnimation(OpacityProperty, null);
        Opacity = 0;
        Visibility = Visibility.Collapsed;

        waiting?.TrySetResult(code);
    }
}

/// <summary>
/// Answers SteamKit's Steam Guard questions with the overlay above. Mobile-app confirmations are
/// accepted without asking - there is nothing for the user to type, only to tap.
/// </summary>
public sealed class SteamGuardPrompt(SteamGuardOverlay overlay, Action<string> status) : IAuthenticator
{
    public async Task<string> GetDeviceCodeAsync(bool previousCodeWasIncorrect)
    {
        return await Ask("Enter the code from your Steam Mobile Authenticator app.", previousCodeWasIncorrect);
    }

    public async Task<string> GetEmailCodeAsync(string email, bool previousCodeWasIncorrect)
    {
        return await Ask($"Steam has emailed a code to {email}. Enter it here.", previousCodeWasIncorrect);
    }

    public Task<bool> AcceptDeviceConfirmationAsync()
    {
        status("CONFIRM THE SIGN-IN IN THE STEAM MOBILE APP");
        return Task.FromResult(true);
    }

    async Task<string> Ask(string blurb, bool previousCodeWasWrong)
    {
        status("WAITING FOR STEAM GUARD");

        var code = await overlay.AskAsync(blurb, previousCodeWasWrong)
            ?? throw new OperationCanceledException("Steam Guard cancelled.");

        status("CHECKING THE CODE");
        return code;
    }
}
