using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace NMSRetroInstaller;

/// <summary>
/// The installer's push button: a flat face with a two-tone bezel, in the logo's own colours,
/// carrying a centred label and optionally an icon beside it.
/// <para>
/// Vertical centring is done on the label's capitals rather than its line box. A display face with
/// a tall ascent - Analog Whispers has one - leaves a lot of empty space under its baseline, so a
/// plainly centred line box makes the text ride high in the button.
/// </para>
/// </summary>
public class AccentButton : Button
{
    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
        nameof(Text), typeof(string), typeof(AccentButton), new FrameworkPropertyMetadata(""));

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(
        nameof(Icon), typeof(ImageSource), typeof(AccentButton), new FrameworkPropertyMetadata(null));

    static readonly DependencyPropertyKey CapNudgeKey = DependencyProperty.RegisterReadOnly(
        nameof(CapNudge), typeof(Thickness), typeof(AccentButton),
        new FrameworkPropertyMetadata(new Thickness()));

    public static readonly DependencyProperty CapNudgeProperty = CapNudgeKey.DependencyProperty;

    public string Text
    {
        get => (string)GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    /// <summary>Optional glyph shown to the left of the label. Null hides it entirely.</summary>
    public ImageSource? Icon
    {
        get => (ImageSource?)GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    /// <summary>Margin the template applies to the label to centre its capitals. Read-only.</summary>
    public Thickness CapNudge => (Thickness)GetValue(CapNudgeProperty);

    /// <summary>The label, exposed so an entrance animation can bring it in on its own beat.</summary>
    public UIElement? LabelPart { get; private set; }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        LabelPart = GetTemplateChild("PART_Label") as UIElement;
        MeasureCapNudge();
    }

    protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
    {
        base.OnPropertyChanged(e);
        if (e.Property == FontSizeProperty || e.Property == FontFamilyProperty)
            MeasureCapNudge();
    }

    /// <summary>
    /// Distance from the line box's centre down to the centre of the capitals, as a symmetric
    /// margin so the label shifts without its layout box growing.
    /// </summary>
    void MeasureCapNudge()
    {
        var family = FontFamily;
        if (family is null) return;

        double caps = 0.7;
        if (new Typeface(family, FontStyle, FontWeight, FontStretch)
                .TryGetGlyphTypeface(out var glyphs))
            caps = glyphs.CapsHeight;

        double drop = (family.LineSpacing / 2 - family.Baseline + caps / 2) * FontSize;
        SetValue(CapNudgeKey, new Thickness(0, drop, 0, -drop));
    }
}
