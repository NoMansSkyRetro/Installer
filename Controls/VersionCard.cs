using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace NMSRetroInstaller;

/// <summary>
/// One installable game version. The cover art stands proud of the plate that carries the name,
/// build and checkbox; unpicked cards dim back so the chosen ones read at full colour.
/// </summary>
public class VersionCard : ToggleButton
{
    public static readonly DependencyProperty CoverProperty = DependencyProperty.Register(
        nameof(Cover), typeof(ImageSource), typeof(VersionCard), new FrameworkPropertyMetadata(null));

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(VersionCard), new FrameworkPropertyMetadata(""));

    public static readonly DependencyProperty BuildProperty = DependencyProperty.Register(
        nameof(Build), typeof(string), typeof(VersionCard), new FrameworkPropertyMetadata(""));

    public ImageSource? Cover
    {
        get => (ImageSource?)GetValue(CoverProperty);
        set => SetValue(CoverProperty, value);
    }

    /// <summary>Update name shown on the plate, e.g. "ATLAS RISES".</summary>
    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    /// <summary>Build label shown under the title, e.g. "VERSION 1.09".</summary>
    public string Build
    {
        get => (string)GetValue(BuildProperty);
        set => SetValue(BuildProperty, value);
    }
}
