using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace NMSLegacyVersionInstaller
{
    // PictureBox that scales its image with high-quality interpolation.
    // The default PictureBox aliases badly when shrinking large images (e.g. 256px orbs down to 32px).
    public class HqPictureBox : PictureBox
    {
        protected override void OnPaint(PaintEventArgs pe)
        {
            pe.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            pe.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            pe.Graphics.SmoothingMode = SmoothingMode.HighQuality;
            pe.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            base.OnPaint(pe);
        }
    }
}
