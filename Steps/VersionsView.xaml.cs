using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace NMSRetroInstaller;

/// <summary>
/// Version picker: the installable builds laid out left to right, with the base install path
/// underneath. Selection is multiple - the original installer lets you take several at once.
/// </summary>
public partial class VersionsView : UserControl
{
    const string DefaultPath = @"C:\NMSLegacy\";
    const double CardTop = 110, CardLeft = 24, CardPitch = 255;

    readonly List<VersionCard> cards = [];
    Storyboard? running;

    /// <summary>Raised when INSTALL is pressed. Read <see cref="Selected"/> for what to install.</summary>
    public event EventHandler? InstallRequested;

    public VersionsView()
    {
        InitializeComponent();
        TextElement.SetFontFamily(this, LogoText.Analog);

        for (int i = 0; i < GameCatalog.All.Count; i++)
        {
            var version = GameCatalog.All[i];
            var card = new VersionCard
            {
                Cover = GameCatalog.Cover(version),
                Title = version.Title,
                Build = version.Build,
                IsChecked = true,
                Opacity = 0,
                RenderTransformOrigin = new Point(0.5, 1),
                Tag = version,
            };
            Canvas.SetLeft(card, CardLeft + i * CardPitch);
            Canvas.SetTop(card, CardTop);
            card.Checked += (_, _) => Refresh();
            card.Unchecked += (_, _) => Refresh();

            cards.Add(card);
            Board.Children.Add(card);
        }

        PathBox.Text = DefaultPath;

        // The popup lives outside this control's visual tree, so it gets the face explicitly.
        LanguageBox.FontFamily = LogoText.Analog;
        LanguageBox.ItemsSource = GameCatalog.Languages;
        LanguageBox.SelectedItem = GameCatalog.DefaultLanguage;

        Refresh();
    }

    /// <summary>The picked versions, in left-to-right order.</summary>
    public IReadOnlyList<GameVersion> Selected =>
        cards.Where(c => c.IsChecked == true).Select(c => (GameVersion)c.Tag).ToArray();

    public string InstallPath => PathBox.Text;

    /// <summary>The language the games will be told to run in.</summary>
    public GameLanguage SelectedLanguage =>
        LanguageBox.SelectedItem as GameLanguage ?? GameCatalog.DefaultLanguage;

    public Storyboard Play()
    {
        running?.Stop(this);
        var sb = new Storyboard();

        Header.Play(sb, 0.00);

        // Cards deal in from the left, growing off their base.
        for (int i = 0; i < cards.Count; i++)
        {
            double t = 0.35 + i * 0.12;
            Anim.Move(sb, cards[i], 0.94, 1, new Vector(0, 26), new Vector(), t, 0.55, Anim.Settle);
            Anim.FadeIn(sb, cards[i], t, 0.55);
        }

        Anim.Rise(sb, PathRow, 14, 0.90, 0.55);
        Anim.Rise(sb, LanguageRow, 14, 1.00, 0.55);
        Anim.Rise(sb, ActionRow, 14, 1.10, 0.55);

        running = sb;
        sb.Begin(this, isControllable: true);
        return sb;
    }

    void Refresh() => InstallButton.IsEnabled = Selected.Count > 0;

    void OnInstall(object sender, RoutedEventArgs e) =>
        InstallRequested?.Invoke(this, EventArgs.Empty);

    void OnBrowse(object sender, RoutedEventArgs e)
    {
        var dialog = new Microsoft.Win32.OpenFolderDialog
        {
            Title = "Base folder to install into",
            InitialDirectory = NearestExisting(PathBox.Text),
        };
        if (dialog.ShowDialog(Window.GetWindow(this)) == true)
            PathBox.Text = dialog.FolderName.TrimEnd('\\') + @"\";
    }

    /// <summary>Walks up until it finds a folder that exists, so Browse opens somewhere sensible.</summary>
    static string NearestExisting(string path)
    {
        while (!string.IsNullOrEmpty(path) && !Directory.Exists(path))
            path = Path.GetDirectoryName(path.TrimEnd(Path.DirectorySeparatorChar)) ?? "";
        return path;
    }
}
