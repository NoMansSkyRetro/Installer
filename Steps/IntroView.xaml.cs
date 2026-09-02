using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Path = System.Windows.Shapes.Path;

namespace NMSRetroInstaller;

/// <summary>
/// The opening screen: the logo draws itself, steps aside, and hands over to the Steam login and
/// the disclaimer. Raises <see cref="LoginRequested"/> when the user submits their details.
/// </summary>
public partial class IntroView : UserControl
{
    // Ink rectangles measured off logo_installer.png, in its own 1050x450 space.
    static readonly Rect TitleInk = new(331, 54, 690, 79);
    static readonly Rect RetroInk = new(332, 153, 691, 163);
    static readonly Rect SubInk = new(331, 335, 437, 78);

    // Where the logo ends up once it steps aside, measured off login_window.png: the 997x414 ink
    // block at (30,25) has to land as a 481x199 block at (14,26).
    const double ParkedScale = 481.0 / 997.0;
    static readonly Vector ParkedAt = new(14 - 30 * ParkedScale, 26 - 25 * ParkedScale);
    static readonly Rect HeadingInk = new(22, 274, 178, 27);

    // The arrow's ring is a near-complete circle whose tail sits at about 241 degrees; it is
    // drawn counter-clockwise from there, finishing at the arrowhead.
    //
    // The wedge starts at 252 rather than right on the tail because it is blurred by 9px, which
    // at the ring's radius is around 7 degrees of arc - starting any closer leaves that soft edge
    // lying across the tail before the sweep has moved, so the arrow begins part-drawn. The sweep
    // is lengthened to match, so it still finishes where it did.
    const double ArrowSweep = -365;

    // Intro runs to 4.4s, then holds before the installer chrome arrives.
    const double Hold = 2.0;
    const double Phase2 = 4.4 + Hold;

    const string Blurb = "A Steam account with No Man's Sky is required.";

    static readonly IEasingFunction Sharp = new QuarticEase { EasingMode = EasingMode.EaseInOut };

    Storyboard? running;

    /// <summary>Raised when LOGIN is pressed. Credentials are on the view for the caller to read.</summary>
    public event EventHandler? LoginRequested;

    public string Username => UsernameBox.Text;

    public string Password => PasswordBox.Password;

    /// <summary>Puts a failed sign-in where the blurb was, so there is no extra dialog to dismiss.</summary>
    public void ShowError(string message)
    {
        SteamBlurb.Text = message;
        SteamBlurb.Foreground = (Brush)FindResource("Accent");
        PasswordBox.Clear();
        PasswordBox.Focus();
    }

    /// <summary>Puts the blurb back, ready for another attempt.</summary>
    public void ClearError()
    {
        SteamBlurb.Text = Blurb;
        SteamBlurb.Foreground = (Brush)FindResource("Muted");
    }

    public IntroView()
    {
        InitializeComponent();

        // FontFamily inherits, so one call covers every run of text on the screen.
        TextElement.SetFontFamily(this, LogoText.Analog);

        SteamBlurb.Text = Blurb;
        DisclaimerText.Text = LoadDisclaimer();
        DisclaimerHeading.Data = LogoText.Combined("DISCLAIMER", LogoText.GeoSans, HeadingInk);

        // Enter walks the form the way a sign-in box is expected to.
        UsernameBox.KeyDown += (_, e) =>
        {
            if (e.Key != Key.Enter) return;
            PasswordBox.Focus();
            e.Handled = true;
        };
        PasswordBox.KeyDown += (_, e) =>
        {
            if (e.Key != Key.Enter) return;
            OnLogin(this, new RoutedEventArgs());
            e.Handled = true;
        };
    }

    /// <summary>Builds and starts the intro. The storyboard is returned (and controllable) so it
    /// can be paused or seeked, which is how the frame-grab harness renders stills.</summary>
    public Storyboard Play()
    {
        running?.Stop(this);
        TextLayer.Children.Clear();
        DisclaimerScroll.ScrollToTop();
        SteamGroup.IsHitTestVisible = false;
        var sb = new Storyboard();

        // ---------- phase one: the logo draws itself ----------

        // Diamond fades in, then the arrow is spiralled on over the top of it.
        Anim.FadeIn(sb, LogoBase, begin: 0.00, duration: 1.10);
        Anim.Spiral(sb, ArrowWedge, 0, ArrowSweep, begin: 1.55, duration: 2.05);

        // NO MAN'S SKY - straight fade.
        var title = Ink(LogoText.Combined("NO MAN'S SKY", LogoText.GeoSans, TitleInk));
        TextLayer.Children.Add(title);
        Anim.FadeIn(sb, title, begin: 0.55, duration: 1.00);

        // RETRO - every letter traced by a thin pen, then inked solid behind the pen stroke.
        const double retroStart = 1.30, trace = 0.70, stagger = 0.34, fill = 0.32;
        var letters = LogoText.Letters("RETRO", LogoText.Neon, RetroInk);
        for (int i = 0; i < letters.Length; i++)
        {
            double t = retroStart + i * stagger;

            var pen = new Canvas { IsHitTestVisible = false };
            foreach (var stroke in Anim.DrawOutline(sb, letters[i], Brushes.White, 2.4, t, trace))
                pen.Children.Add(stroke);
            TextLayer.Children.Add(pen);

            var solid = Ink(letters[i]);
            TextLayer.Children.Add(solid);
            Anim.FadeIn(sb, solid, t + trace - 0.10, fill);
            Anim.FadeOut(sb, pen, t + trace - 0.10, fill);
        }

        // INSTALLER - straight fade, landing last.
        var sub = Ink(LogoText.Combined("INSTALLER", LogoText.GeoSans, SubInk));
        TextLayer.Children.Add(sub);
        Anim.FadeIn(sb, sub, begin: 3.50, duration: 0.90);

        // ---------- phase two: the logo steps aside and the installer arrives ----------

        Anim.Move(sb, Logo, 1, ParkedScale, new Vector(), ParkedAt, Phase2, 1.10, Sharp);

        // Steam panel slides in from the right, then fills itself in from the top down.
        Anim.SlideIn(sb, SteamGroup, new Vector(150, 0), Phase2 + 0.80, 0.70, Anim.Settle);
        Anim.FadeIn(sb, SteamGroup, Phase2 + 0.80, 0.45);
        Anim.PopIn(sb, SteamOrb, 0.45, Phase2 + 1.20, 0.55, Anim.Settle);
        Anim.FadeIn(sb, SteamOrb, Phase2 + 1.20, 0.35);
        Anim.Wipe(sb, SteamWord, new Size(121, 25), Phase2 + 1.50, 0.55);
        Anim.FadeIn(sb, SteamWord, Phase2 + 1.50, 0.20);
        Anim.Rise(sb, SteamBlurb, 8, Phase2 + 1.75, 0.45);
        Anim.Rise(sb, UserField, 10, Phase2 + 1.95, 0.45);
        Anim.Rise(sb, PassField, 10, Phase2 + 2.10, 0.45);
        LoginButton.ApplyTemplate();
        Anim.PopIn(sb, LoginButton, 0.88, Phase2 + 2.30, 0.45, Anim.Settle);
        Anim.FadeIn(sb, LoginButton, Phase2 + 2.30, 0.35);
        Anim.FadeIn(sb, LoginButton.LabelPart!, Phase2 + 2.55, 0.40);

        // Disclaimer box rises from the bottom edge; its text follows.
        Anim.SlideIn(sb, DisclaimerGroup, new Vector(0, 70), Phase2 + 1.15, 0.75, Anim.Settle);
        Anim.FadeIn(sb, DisclaimerGroup, Phase2 + 1.15, 0.50);
        Anim.Rise(sb, DisclaimerScroll, 14, Phase2 + 2.10, 0.70);

        // Last thing on screen, once everything else has settled.
        Anim.FadeIn(sb, Footnote, Phase2 + 2.95, 0.80);

        sb.Completed += (_, _) =>
        {
            SteamGroup.IsHitTestVisible = true;
            UsernameBox.Focus();
        };

        running = sb;
        sb.Begin(this, isControllable: true);
        return sb;
    }

    void OnLogin(object sender, RoutedEventArgs e) => LoginRequested?.Invoke(this, EventArgs.Empty);

    void OnWebsite(object sender, RoutedEventArgs e) => System.Diagnostics.Process.Start(
        new System.Diagnostics.ProcessStartInfo(GameCatalog.Website) { UseShellExecute = true });

    static Path Ink(Geometry g) =>
        new() { Data = g, Fill = Brushes.White, Opacity = 0, IsHitTestVisible = false };

    static string LoadDisclaimer()
    {
        var info = Application.GetResourceStream(
            new Uri("pack://application:,,,/Resources/Disclaimer.txt"));
        if (info is null) return "";
        using var reader = new StreamReader(info.Stream);
        return reader.ReadToEnd().Trim();
    }
}
