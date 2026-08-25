using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace NMSLegacyVersionInstaller
{
    // Pre-scales an image to fit a box using high-quality bicubic so a stock
    // PictureBox (SizeMode = Zoom) shows it crisp instead of aliased.
    // Replaces the old HqPictureBox control, which the WinForms designer couldn't render.
    internal static class ImageScaler
    {
        public static Bitmap FitHighQuality(Image source, Size box)
        {
            float scale = Math.Min((float)box.Width / source.Width, (float)box.Height / source.Height);
            int w = Math.Max(1, (int)Math.Round(source.Width * scale));
            int h = Math.Max(1, (int)Math.Round(source.Height * scale));
            var bmp = new Bitmap(w, h);
            using (var g = Graphics.FromImage(bmp))
            {
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.SmoothingMode = SmoothingMode.HighQuality;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.DrawImage(source, 0, 0, w, h);
            }
            return bmp;
        }
    }
}
