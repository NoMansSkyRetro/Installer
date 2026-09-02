using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Media.Animation;
using NMSRetroInstaller.Steam;

namespace NMSRetroInstaller;

/// <summary>
/// The working step: one progress bar and a console log. It covers the whole run - download,
/// patch, shader fix, extras and shortcuts - so there is no separate finalising step.
/// </summary>
public partial class InstallView : UserControl
{
    /// <summary>Lines kept on screen. The install log on disk keeps every one of them.</summary>
    const int VisibleLines = 500;

    static readonly Brush Chatter = Frozen(0xA8, 0xA4, 0xA8);
    static readonly Brush Phase = Frozen(0xDF, 0x3E, 0x60);
    static readonly Brush Done = Brushes.White;
    static readonly Brush Bad = Frozen(0xFF, 0x6B, 0x6B);

    static Brush Frozen(byte r, byte g, byte b)
    {
        var brush = new SolidColorBrush(Color.FromRgb(r, g, b));
        brush.Freeze();
        return brush;
    }

    readonly StringBuilder transcript = new();
    Storyboard? running;
    int lastShownTenths = -1;

    public InstallView()
    {
        InitializeComponent();
        TextElement.SetFontFamily(ProgressRow, LogoText.Analog);
        TextElement.SetFontFamily(LogRow, LogoText.Analog);
    }

    /// <summary>Brings the step on screen. Call before <see cref="RunAsync"/>.</summary>
    public Storyboard Play()
    {
        running?.Stop(this);
        LogText.Inlines.Clear();
        lock (transcript) transcript.Clear();
        Bar.Value = 0;
        TaskText.Text = "PREPARING";
        PercentText.Text = "0%";

        var sb = new Storyboard();
        Header.Play(sb, 0.00);
        Anim.Rise(sb, ProgressRow, 14, 0.30, 0.55);
        Anim.Rise(sb, LogRow, 18, 0.45, 0.55);

        running = sb;
        sb.Begin(this, isControllable: true);
        return sb;
    }

    /// <summary>
    /// Runs the whole install. Returns false if it stopped early, in which case the log on screen
    /// says why and the caller should stay on this step.
    /// </summary>
    public async Task<bool> RunAsync(
        SteamSession session, IReadOnlyList<GameVersion> versions, string root, CancellationToken ct)
    {
        try
        {
            Say($"install root  {root}", Chatter);
            Say($"signed in as  {session.AccountName}", Chatter);
            Say($"queued        {versions.Count} version(s)", Chatter);

            var gpu = ShaderFix.DetectGPU(s => Say("  " + s, Chatter));
            Say($"graphics      {gpu}", Chatter);

            // First, so the shortcut icons written below already exist when the .lnk points at them.
            SetTask("EXTRAS");
            Say("writing extras and shortcuts", Phase);
            WriteExtras(root);

            for (var i = 0; i < versions.Count; i++)
            {
                var version = versions[i];
                string folder = GameCatalog.GameFolder(root, version);

                SetTask(version.Title + " - DOWNLOAD");
                Say($"downloading {version.Title} ({version.Build}) to {folder}", Phase);
                await DepotDownload.RunAsync(
                    session, GameCatalog.AppId, GameCatalog.DepotId, version.Manifest, folder,
                    (written, total) => Progress(BarPercent(i, versions.Count, written, total)),
                    line => Say("  " + line, Chatter),
                    ct);

                // The steps below take seconds against the download's minutes, so they get no
                // share of the bar - the task line above is what says they are running.
                SetTask(version.Title + " - PATCH");
                Say("patching " + version.Title, Phase);
                Patcher.Apply(root, version, session.AccountName, line => Say("  " + line, Chatter));

                SetTask(version.Title + " - SHADER FIX");
                Say("applying the retro shader fix", Phase);
                Say("  " + ShaderFix.Apply(folder, version.Update, gpu, line => Say("  " + line, Chatter)), Chatter);

                Shortcuts.Create(
                    Path.Combine(root, version.ShortcutName + ".lnk"),
                    GameCatalog.ExePath(root, version),
                    icon: Path.Combine(ExtrasFolder(root), version.Icon));
                Say("  wrote " + version.ShortcutName + ".lnk", Chatter);

                // Close the slice out, in case the manifest's byte total ran slightly short.
                Progress(BarPercent(i, versions.Count, 1, 1));
            }

            SetTask("LAUNCHER");
            Say("installing the launcher", Phase);
            InstallLauncher(root);
            Progress(100);

            SetTask("COMPLETE");
            Say("", Chatter);
            Say("install complete.", Done);
            SaveTranscript(root);
            await System.Threading.Tasks.Task.Delay(700, ct);
            return true;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            SetTask("CANCELLED");
            Say("", Chatter);
            Say("install cancelled.", Bad);
            SaveTranscript(root);
            return false;
        }
        catch (Exception ex)
        {
            // Anything else, including a stray cancellation nobody asked for, is a failure with a
            // reason - never report it as the user having cancelled.
            var cause = Unwrap(ex);

            SetTask("FAILED");
            Say("", Chatter);
            Say(cause is SteamException ? cause.Message : cause.GetType().Name + ": " + cause.Message, Bad);
            Say("nothing was left half-installed on Steam's side - you can close this and try again.", Chatter);
            SaveTranscript(root);
            return false;
        }
    }

    /// <summary>The real exception out of whatever Parallel.ForEachAsync wrapped it in.</summary>
    static Exception Unwrap(Exception ex) =>
        ex is AggregateException aggregate ? Unwrap(aggregate.Flatten().InnerExceptions[0]) : ex;

    static string ExtrasFolder(string root) => Path.Combine(root, "Extras");

    /// <summary>
    /// Where the bar sits while version <paramref name="index"/> of <paramref name="count"/> is
    /// downloading. Each version owns an equal slice and its download fills the whole of it, so
    /// installing one version puts the bar on exactly the percentage the log is printing.
    /// </summary>
    internal static double BarPercent(int index, int count, long written, long total)
    {
        double slice = 100.0 / Math.Max(1, count);
        double ratio = total <= 0 ? 0 : Math.Clamp(written / (double)total, 0, 1);

        return index * slice + slice * ratio;
    }

    /// <summary>The bundled tools and links that sit alongside the games.</summary>
    void WriteExtras(string root)
    {
        var extras = ExtrasFolder(root);
        Payload.WriteFolder("InstallerExtras", extras);

        Shortcuts.Create(
            Path.Combine(root, "RetroShaderFix.lnk"),
            Path.Combine(extras, "RetroShaderFix.exe"),
            workingDirectory: root);

        Shortcuts.CreateUrl(
            Path.Combine(root, "No Man's Sky Retro Discord.url"),
            GameCatalog.Discord,
            Path.Combine(extras, "discord.ico"));

        Say("  wrote Extras, RetroShaderFix.lnk and the Discord link", Chatter);
    }

    /// <summary>
    /// The launcher is this same executable under another name, so the install folder gets one
    /// self-contained file rather than a second runtime's worth of them.
    /// </summary>
    void InstallLauncher(string root)
    {
        var target = Path.Combine(root, App.LauncherName);
        var self = Environment.ProcessPath
            ?? throw new InvalidOperationException("Could not work out where the installer is running from.");

        try
        {
            File.Copy(self, target, overwrite: true);
            Say("  wrote " + App.LauncherName, Chatter);
        }
        catch (IOException) when (File.Exists(target))
        {
            // Almost always the launcher from a previous install being open right now.
            Say("  " + App.LauncherName + " is in use - kept the copy already there", Chatter);
        }

        // The copy carries the installer's own icon, so the shortcuts just point at it.
        Shortcuts.Create(Shortcuts.Desktop("No Man's Sky Retro Launcher"), target, "--launcher", target, root);
        Shortcuts.Create(Shortcuts.StartMenu("Launcher"), target, "--launcher", target, root);
        Say($"  wrote the desktop shortcut and Start menu > {Shortcuts.StartMenuFolder} > Launcher", Chatter);
    }

    void SetTask(string task)
    {
        if (!Dispatcher.CheckAccess()) { Dispatcher.Invoke(() => SetTask(task)); return; }
        TaskText.Text = task;
    }

    /// <summary>
    /// Moves the bar. Called once per downloaded chunk, so it only touches the UI when the number
    /// on screen would actually change.
    /// </summary>
    void Progress(double percent)
    {
        var tenths = (int)Math.Round(Math.Clamp(percent, 0, 100) * 10);
        if (Interlocked.Exchange(ref lastShownTenths, tenths) == tenths) return;

        Dispatcher.BeginInvoke(() =>
        {
            Bar.BeginAnimation(RangeBase.ValueProperty, null);
            Bar.Value = tenths / 10.0;
            PercentText.Text = $"{Math.Round(Bar.Value)}%";
        });
    }

    /// <summary>
    /// Adds a line to the log. A depot is thousands of files and each one reports itself, so the
    /// transcript is written straight away and only the UI update is handed to the dispatcher -
    /// posted rather than waited on, so the download threads never queue up behind the screen.
    /// </summary>
    void Say(string line, Brush brush)
    {
        lock (transcript)
            transcript.AppendLine(line);

        if (Dispatcher.CheckAccess()) Append(line, brush);
        else Dispatcher.BeginInvoke(() => Append(line, brush));
    }

    void Append(string line, Brush brush)
    {
        var inlines = LogText.Inlines;
        inlines.Add(new Run(line + Environment.NewLine) { Foreground = brush });

        // The whole run is kept in the transcript; on screen only the tail is worth carrying,
        // and a TextBlock holding thousands of Runs gets slow to lay out.
        while (inlines.Count > VisibleLines)
            inlines.Remove(inlines.FirstInline);

        LogScroll.ScrollToBottom();
    }

    string Transcript()
    {
        lock (transcript)
            return transcript.ToString();
    }

    /// <summary>Keeps the same Log folder the WinForms installer wrote its transcripts into.</summary>
    void SaveTranscript(string root)
    {
        try
        {
            var folder = Path.Combine(root, "Log");
            Directory.CreateDirectory(folder);
            File.WriteAllText(
                Path.Combine(folder, "Install-" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss") + ".txt"),
                Transcript());
        }
        catch (Exception ex)
        {
            Debug.WriteLine("Could not write the install log: " + ex.Message);
        }
    }
}
