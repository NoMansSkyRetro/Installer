using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NMSRetroInstaller;

/// <summary>
/// An installed game, presented the same way as a <see cref="VersionCard"/> but as a plain push
/// button that starts the game. Used by the completion step and by the standalone launcher.
/// </summary>
public class GameTile : Button
{
    public static readonly DependencyProperty CoverProperty = DependencyProperty.Register(
        nameof(Cover), typeof(ImageSource), typeof(GameTile), new FrameworkPropertyMetadata(null));

    public static readonly DependencyProperty TitleProperty = DependencyProperty.Register(
        nameof(Title), typeof(string), typeof(GameTile), new FrameworkPropertyMetadata(""));

    public static readonly DependencyProperty BuildProperty = DependencyProperty.Register(
        nameof(Build), typeof(string), typeof(GameTile), new FrameworkPropertyMetadata(""));

    public ImageSource? Cover
    {
        get => (ImageSource?)GetValue(CoverProperty);
        set => SetValue(CoverProperty, value);
    }

    public string Title
    {
        get => (string)GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Build
    {
        get => (string)GetValue(BuildProperty);
        set => SetValue(BuildProperty, value);
    }
}
