using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace NMSRetroInstaller;

/// <summary>
/// The last step: a header over the shared <see cref="LaunchPad"/>, which is the same component
/// the standalone launcher puts in its own window.
/// </summary>
public partial class CompleteView : UserControl
{
    Storyboard? running;

    public CompleteView() => InitializeComponent();

    public Storyboard Play(IReadOnlyList<GameVersion> versions, string root)
    {
        running?.Stop(this);
        Pad.Load(versions, root);

        var sb = new Storyboard();
        Header.Play(sb, 0.00);
        Pad.Play(sb, 0.30);

        running = sb;
        sb.Begin(this, isControllable: true);
        return sb;
    }
}
