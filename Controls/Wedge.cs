using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace NMSRetroInstaller;

/// <summary>
/// A pie slice from <see cref="StartAngle"/> spanning <see cref="Sweep"/> degrees.
/// Angles are degrees clockwise from twelve o'clock; a negative sweep runs counter-clockwise.
/// Animating <see cref="Sweep"/> and using the wedge as an opacity mask gives a radial
/// "being drawn" reveal.
/// </summary>
public class Wedge : Shape
{
    public static readonly DependencyProperty CenterProperty = Reg(nameof(Center), new Point());
    public static readonly DependencyProperty RadiusProperty = Reg(nameof(Radius), 0d);
    public static readonly DependencyProperty StartAngleProperty = Reg(nameof(StartAngle), 0d);
    public static readonly DependencyProperty SweepProperty = Reg(nameof(Sweep), 0d);

    public Point Center { get => (Point)GetValue(CenterProperty); set => SetValue(CenterProperty, value); }
    public double Radius { get => (double)GetValue(RadiusProperty); set => SetValue(RadiusProperty, value); }
    public double StartAngle { get => (double)GetValue(StartAngleProperty); set => SetValue(StartAngleProperty, value); }
    public double Sweep { get => (double)GetValue(SweepProperty); set => SetValue(SweepProperty, value); }

    static DependencyProperty Reg(string name, object def) => DependencyProperty.Register(
        name, def.GetType(), typeof(Wedge),
        new FrameworkPropertyMetadata(def, FrameworkPropertyMetadataOptions.AffectsRender));

    protected override Geometry DefiningGeometry
    {
        get
        {
            double sweep = Sweep, r = Radius;
            if (r <= 0 || Math.Abs(sweep) < 0.01) return Geometry.Empty;
            if (Math.Abs(sweep) >= 359.99) return new EllipseGeometry(Center, r, r);

            Point from = Polar(Center, r, StartAngle);
            Point to = Polar(Center, r, StartAngle + sweep);
            var figure = new PathFigure { StartPoint = Center, IsClosed = true, IsFilled = true };
            figure.Segments.Add(new LineSegment(from, false));
            figure.Segments.Add(new ArcSegment(to, new Size(r, r), 0,
                Math.Abs(sweep) > 180,
                sweep > 0 ? SweepDirection.Clockwise : SweepDirection.Counterclockwise, false));
            return new PathGeometry(new[] { figure });
        }
    }

    static Point Polar(Point centre, double radius, double degrees)
    {
        double t = degrees * Math.PI / 180.0;
        return new Point(centre.X + radius * Math.Sin(t), centre.Y - radius * Math.Cos(t));
    }
}
