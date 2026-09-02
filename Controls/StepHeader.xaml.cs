using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace NMSRetroInstaller;

/// <summary>
/// Shared masthead for every step: the diamond, a heading set in the logo face, a line of
/// explanation and the accent rule that wipes in beneath them.
/// </summary>
public partial class StepHeader : UserControl
{
    public static readonly DependencyProperty HeadingProperty = DependencyProperty.Register(
        nameof(Heading), typeof(string), typeof(StepHeader),
        new FrameworkPropertyMetadata("", (d, _) => ((StepHeader)d).Redraw()));

    public static readonly DependencyProperty SubtitleProperty = DependencyProperty.Register(
        nameof(Subtitle), typeof(string), typeof(StepHeader),
        new FrameworkPropertyMetadata("", (d, e) => ((StepHeader)d).SubtitleText.Text = (string)e.NewValue));

    public string Heading
    {
        get => (string)GetValue(HeadingProperty);
        set => SetValue(HeadingProperty, value);
    }

    public string Subtitle
    {
        get => (string)GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public StepHeader()
    {
        InitializeComponent();
        TextElement.SetFontFamily(this, LogoText.Analog);
    }

    /// <summary>Fades the header up and wipes the rule out from the left.</summary>
    public void Play(Storyboard sb, double begin)
    {
        Anim.Rise(sb, this, 12, begin, 0.55);
        Anim.Wipe(sb, Rule, new Size(1002, 2), begin + 0.20, 0.70);
    }

    void Redraw() =>
        HeadingPath.Data = LogoText.Heading(Heading, LogoText.GeoSans, new Point(82, 20), 30);
}
