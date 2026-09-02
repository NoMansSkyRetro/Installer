using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media.Animation;

namespace NMSRetroInstaller;

/// <summary>
/// Dims everything behind it and shows a spinner with a running status line. Swallows input while
/// visible, so nothing underneath can be clicked mid-step.
/// </summary>
public partial class BusyOverlay : UserControl
{
    static readonly Duration FadeTime = new(TimeSpan.FromSeconds(0.35));

    /// <summary>The line under the spinner. Set it as the work reports its own progress.</summary>
    public string StatusText
    {
        get => Status.Text;
        set
        {
            if (Dispatcher.CheckAccess()) Status.Text = value;
            else Dispatcher.Invoke(() => Status.Text = value);
        }
    }

    public BusyOverlay()
    {
        InitializeComponent();
        TextElement.SetFontFamily(this, LogoText.Analog);
    }

    /// <summary>Brings the overlay up with a first status line and starts the spinner.</summary>
    public void Show(string status)
    {
        StatusText = status;
        Visibility = Visibility.Visible;
        Spin.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, new DoubleAnimation
        {
            From = 0,
            To = 360,
            Duration = new Duration(TimeSpan.FromSeconds(1.1)),
            RepeatBehavior = RepeatBehavior.Forever,
        });
        BeginAnimation(OpacityProperty, new DoubleAnimation(0, 1, FadeTime));
    }

    public async Task HideAsync()
    {
        BeginAnimation(OpacityProperty, new DoubleAnimation(Opacity, 0, FadeTime));
        await Task.Delay(FadeTime.TimeSpan);
        Hide();
    }

    /// <summary>Drops the overlay immediately and stops the spinner.</summary>
    public void Hide()
    {
        BeginAnimation(OpacityProperty, null);
        Opacity = 0;
        Visibility = Visibility.Collapsed;
        Spin.BeginAnimation(System.Windows.Media.RotateTransform.AngleProperty, null);
    }
}
