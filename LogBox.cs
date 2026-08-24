using System;
using System.Drawing;
using System.Windows.Forms;

namespace NMSLegacyVersionInstaller
{
    // Read-only console-style log: cyan text on dark blue by default, with thread-safe coloured line appends.
    // Shared by the download step and the final-setup step.
    public class LogBox : RichTextBox
    {
        public static readonly Color DarkBlue = Color.FromArgb(0, 0, 64);
        public static readonly Color Cyan = Color.Cyan;

        public LogBox()
        {
            ReadOnly = true;
            BackColor = DarkBlue;
            ForeColor = Cyan;
            Font = new Font("Consolas", 8.25F);
            BorderStyle = BorderStyle.None;
            WordWrap = true;
            ScrollBars = RichTextBoxScrollBars.Vertical;
        }

        public void AppendLine(string text)
        {
            AppendLine(text, ForeColor);
        }

        public void AppendLine(string text, Color color)
        {
            if (InvokeRequired)
            {
                if (IsHandleCreated)
                    BeginInvoke((Action)(() => AppendLine(text, color)));
                return;
            }
            SelectionStart = TextLength;
            SelectionLength = 0;
            SelectionColor = color;
            AppendText((text ?? "").TrimEnd('\r', '\n') + Environment.NewLine);
            SelectionColor = ForeColor;
            SelectionStart = TextLength;
            ScrollToCaret();
        }
    }
}
