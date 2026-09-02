using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace NMSRetroInstaller;

/// <summary>
/// Reusable animation primitives for the installer's intro sequence.
/// Everything appends to a caller-owned <see cref="Storyboard"/>, so a whole
/// multi-part sequence is one timeline that can be replayed or seeked.
/// Times are seconds from the storyboard's start.
/// </summary>
public static class Anim
{
    public static readonly IEasingFunction Smooth = new CubicEase { EasingMode = EasingMode.EaseInOut };
    public static readonly IEasingFunction Settle = new CubicEase { EasingMode = EasingMode.EaseOut };

    /// <summary>Animate an element's opacity.</summary>
    public static void Fade(Storyboard sb, UIElement target, double from, double to,
                            double begin, double duration, IEasingFunction? ease = null)
    {
        target.Opacity = from;
        Add(sb, target, UIElement.OpacityProperty, from, to, begin, duration, ease ?? Smooth);
    }

    public static void FadeIn(Storyboard sb, UIElement target, double begin, double duration,
                              IEasingFunction? ease = null)
        => Fade(sb, target, 0, 1, begin, duration, ease);

    public static void FadeOut(Storyboard sb, UIElement target, double begin, double duration,
                               IEasingFunction? ease = null)
        => Add(sb, target, UIElement.OpacityProperty, 1, 0, begin, duration, ease ?? Smooth);

    /// <summary>
    /// "Draws" a stroked path by walking a single dash along it, so the stroke grows from the
    /// figure's start point to its end. The dash pattern is expressed in multiples of the stroke
    /// thickness, hence the division.
    /// </summary>
    public static void Draw(Storyboard sb, Path path, double begin, double duration,
                            IEasingFunction? ease = null)
    {
        double units = Length(path.Data) / Math.Max(path.StrokeThickness, 1e-4);
        path.StrokeDashArray = new DoubleCollection { units, units };
        path.StrokeDashCap = PenLineCap.Flat;
        path.StrokeDashOffset = units;
        Add(sb, path, Shape.StrokeDashOffsetProperty, units, 0, begin, duration, ease ?? Settle);
    }

    /// <summary>
    /// Draws every contour of a shape at once, each finishing together regardless of its own
    /// length. Returns the paths so the caller can parent them.
    /// </summary>
    public static IReadOnlyList<Path> DrawOutline(Storyboard sb, Geometry shape, Brush stroke,
                                                  double thickness, double begin, double duration,
                                                  IEasingFunction? ease = null)
    {
        var paths = new List<Path>();
        foreach (var contour in Contours(shape))
        {
            var p = new Path
            {
                Data = contour,
                Stroke = stroke,
                StrokeThickness = thickness,
                StrokeLineJoin = PenLineJoin.Round,
                IsHitTestVisible = false,
            };
            Draw(sb, p, begin, duration, ease);
            paths.Add(p);
        }
        return paths;
    }

    /// <summary>
    /// Animates an element's scale and offset together. The scale pivots on the element's
    /// RenderTransformOrigin, so set that to 0.5,0.5 in XAML for a centred pop.
    /// </summary>
    public static void Move(Storyboard sb, UIElement target,
                            double fromScale, double toScale, Vector from, Vector to,
                            double begin, double duration, IEasingFunction? ease = null)
    {
        var (scale, offset) = Rig(target);
        scale.ScaleX = scale.ScaleY = fromScale;
        offset.X = from.X;
        offset.Y = from.Y;
        ease ??= Smooth;
        // A storyboard cannot target a bare Freezable, so reach the transforms by property path.
        Add(sb, target, "RenderTransform.Children[0].ScaleX", fromScale, toScale, begin, duration, ease);
        Add(sb, target, "RenderTransform.Children[0].ScaleY", fromScale, toScale, begin, duration, ease);
        Add(sb, target, "RenderTransform.Children[1].X", from.X, to.X, begin, duration, ease);
        Add(sb, target, "RenderTransform.Children[1].Y", from.Y, to.Y, begin, duration, ease);
    }

    /// <summary>Slides an element into place from an offset.</summary>
    public static void SlideIn(Storyboard sb, UIElement target, Vector from,
                               double begin, double duration, IEasingFunction? ease = null)
        => Move(sb, target, 1, 1, from, new Vector(), begin, duration, ease);

    /// <summary>Scales an element up to its resting size.</summary>
    public static void PopIn(Storyboard sb, UIElement target, double fromScale,
                             double begin, double duration, IEasingFunction? ease = null)
        => Move(sb, target, fromScale, 1, new Vector(), new Vector(), begin, duration, ease);

    /// <summary>Fades an element in while it drifts up into place.</summary>
    public static void Rise(Storyboard sb, UIElement target, double from,
                            double begin, double duration)
    {
        SlideIn(sb, target, new Vector(0, from), begin, duration, Settle);
        FadeIn(sb, target, begin, duration);
    }

    /// <summary>Reveals an element left to right by growing a clip rectangle over it.</summary>
    public static void Wipe(Storyboard sb, UIElement target, Size size,
                            double begin, double duration, IEasingFunction? ease = null)
    {
        var closed = new Rect(0, 0, 0, size.Height);
        target.Clip = new RectangleGeometry(closed);
        var a = new RectAnimation(closed, new Rect(0, 0, size.Width, size.Height),
                                  TimeSpan.FromSeconds(duration))
        {
            BeginTime = TimeSpan.FromSeconds(begin),
            EasingFunction = ease ?? Settle,
        };
        Storyboard.SetTarget(a, target);
        Storyboard.SetTargetProperty(a, new PropertyPath("Clip.Rect"));
        sb.Children.Add(a);
    }

    /// <summary>Sweeps a <see cref="Wedge"/> mask, revealing what it masks in a spiral.</summary>
    public static void Spiral(Storyboard sb, Wedge wedge, double fromSweep, double toSweep,
                              double begin, double duration, IEasingFunction? ease = null)
    {
        wedge.Sweep = fromSweep;
        Add(sb, wedge, Wedge.SweepProperty, fromSweep, toSweep, begin, duration, ease ?? Smooth);
    }

    /// <summary>Gives an element a scale + translate transform pair, reusing one if already set.</summary>
    static (ScaleTransform Scale, TranslateTransform Offset) Rig(UIElement target)
    {
        if (target.RenderTransform is TransformGroup existing && existing.Children.Count == 2)
            return ((ScaleTransform)existing.Children[0], (TranslateTransform)existing.Children[1]);

        var scale = new ScaleTransform();
        var offset = new TranslateTransform();
        target.RenderTransform = new TransformGroup { Children = { scale, offset } };
        return (scale, offset);
    }

    static void Add(Storyboard sb, DependencyObject target, DependencyProperty property,
                    double from, double to, double begin, double duration, IEasingFunction ease)
        => Add(sb, target, new PropertyPath(property), from, to, begin, duration, ease);

    static void Add(Storyboard sb, DependencyObject target, string path,
                    double from, double to, double begin, double duration, IEasingFunction ease)
        => Add(sb, target, new PropertyPath(path), from, to, begin, duration, ease);

    static void Add(Storyboard sb, DependencyObject target, PropertyPath path,
                    double from, double to, double begin, double duration, IEasingFunction ease)
    {
        var a = new DoubleAnimation(from, to, TimeSpan.FromSeconds(duration))
        {
            BeginTime = TimeSpan.FromSeconds(begin),
            EasingFunction = ease,
        };
        Storyboard.SetTarget(a, target);
        Storyboard.SetTargetProperty(a, path);
        sb.Children.Add(a);
    }

    /// <summary>Splits a geometry into one single-figure geometry per contour.</summary>
    public static IEnumerable<PathGeometry> Contours(Geometry g)
    {
        var pg = PathGeometry.CreateFromGeometry(g);
        foreach (var figure in pg.Figures)
            yield return new PathGeometry(new[] { figure.Clone() }) { Transform = pg.Transform };
    }

    /// <summary>Approximate outline length, used to size the dash pattern.</summary>
    public static double Length(Geometry g)
    {
        var flat = g.GetFlattenedPathGeometry(0.05, ToleranceType.Absolute);
        double total = 0;
        foreach (var figure in flat.Figures)
        {
            Point start = figure.StartPoint, p = start;
            foreach (var seg in figure.Segments)
            {
                switch (seg)
                {
                    case PolyLineSegment poly:
                        foreach (var q in poly.Points) { total += (q - p).Length; p = q; }
                        break;
                    case LineSegment line:
                        total += (line.Point - p).Length; p = line.Point;
                        break;
                }
            }
            if (figure.IsClosed) total += (start - p).Length;
        }
        return total;
    }
}
