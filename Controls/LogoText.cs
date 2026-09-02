using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace NMSRetroInstaller;

/// <summary>
/// Turns a logo line into outline geometry sized to land exactly on a target ink rectangle
/// measured off the reference artwork. Every letter is kept separate so RETRO can be traced
/// one letter at a time.
/// </summary>
public static class LogoText
{
    public static readonly FontFamily GeoSans = Embedded("GeosansLight-NMS");
    public static readonly FontFamily Neon = Embedded("NEON LED Light");
    public static readonly FontFamily Analog = Embedded("Analog Whispers FREE");

    // Base pack URI plus the family name, which is how a font compiled into this assembly is named.
    static FontFamily Embedded(string familyName) =>
        new(new Uri("pack://application:,,,/"), "./Resources/#" + familyName);

    /// <summary>One geometry per character, laid out at em 100 with no fitting applied.</summary>
    static Geometry[] Unfitted(string text, FontFamily family, out Rect natural)
    {
        var typeface = new Typeface(family, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
        const double em = 100;

        var glyphs = new Geometry[text.Length];
        double pen = 0;
        for (int i = 0; i < text.Length; i++)
        {
            var ft = new FormattedText(text[i].ToString(), CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight, typeface, em, Brushes.White, 1.0);
            // BuildGeometry hands back a frozen geometry; clone so the layout transform can be set.
            glyphs[i] = ft.BuildGeometry(new Point(pen, 0)).Clone();
            pen += ft.WidthIncludingTrailingWhitespace;
        }

        natural = Rect.Empty;
        foreach (var g in glyphs)
            if (!g.Bounds.IsEmpty) natural.Union(g.Bounds);
        return glyphs;
    }

    /// <summary>One geometry per character, positioned so the whole line fills <paramref name="ink"/>.</summary>
    public static Geometry[] Letters(string text, FontFamily family, Rect ink)
    {
        var glyphs = Unfitted(text, family, out var natural);
        if (natural.IsEmpty || natural.Height <= 0) return glyphs;

        // The reference artwork was set by stretching each line to fit its box, so match with a
        // non-uniform scale rather than uniform scale plus letter-spacing.
        double sx = ink.Width / natural.Width;
        double sy = ink.Height / natural.Height;

        for (int i = 0; i < glyphs.Length; i++)
            glyphs[i].Transform = new MatrixTransform(
                sx, 0, 0, sy, ink.X - natural.X * sx, ink.Y - natural.Y * sy);

        return glyphs;
    }

    /// <summary>
    /// A heading laid out at its natural proportions: the capitals are set to
    /// <paramref name="capHeight"/> and the width follows from the font, so nothing is stretched.
    /// </summary>
    public static Geometry Heading(string text, FontFamily family, Point at, double capHeight)
    {
        Unfitted(text, family, out var natural);
        double width = natural.IsEmpty || natural.Height <= 0
            ? capHeight
            : capHeight * natural.Width / natural.Height;
        return Combined(text, family, new Rect(at.X, at.Y, width, capHeight));
    }

    /// <summary>The whole line as one geometry.</summary>
    public static Geometry Combined(string text, FontFamily family, Rect ink)
    {
        // Nonzero matches how TrueType winds its counters, and keeps overlapping glyphs solid.
        var group = new GeometryGroup { FillRule = FillRule.Nonzero };
        foreach (var g in Letters(text, family, ink)) group.Children.Add(g);
        return group;
    }
}
