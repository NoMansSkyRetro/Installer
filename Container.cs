using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Management;
using System.Diagnostics;
using System.Threading;

namespace NMSLegacyVersionInstaller
{
    // A step that wants a callback every time it becomes the current step (not just once like Load).
    public interface IStepEnter
    {
        void OnStepEnter();
    }

    public partial class Container : Form
    {
        public Container()
        {
            InitializeComponent();
        }

        public static List<Control> Sequence = new List<Control>() {
            new Steps.ExtractTemporaryFiles(),
            new Steps.Disclaimer(),
            new Steps.SelectVersion(),
            new Steps.DepotDownloader(),
            new Steps.SaveGameStep(),
            new Steps.ShaderFixStep(),
            new Steps.FinalSteps(),
            new Steps.Complete(),
        };
        public static int Step = 0;

        public static T FindStep<T>() where T : Control {
            foreach (Control step in Sequence) {
                if (step is T) {
                    return step as T;
                }
            }

            return null;
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Container_Load(object sender, EventArgs e)
        {
            pnlStep.Controls.Add(Sequence[0]);
        }

        private void btnNext_Click(object sender, EventArgs e)
        {
            Next();
        }

        private void btnBack_Click(object sender, EventArgs e)
        {
            Back();
        }

        public void Next()
        {
            if (btnNext.Text == "Finish")
            {
                this.Close();
                return;
            }
            pnlStep.Controls.Clear();
            Step++;
            pnlStep.Controls.Add(Sequence[Step]);
            NotifyStepEnter();
            if (Step == Sequence.Count - 1)
            {
                btnBack.Enabled = false;
                btnNext.Text = "Finish";
            }
            else
            {
                btnBack.Enabled = StepsEnabled;
            }
            // No going back into the download once it's done (would restart the login/download).
            if (Sequence[Step] is Steps.SaveGameStep)
                btnBack.Enabled = false;
        }

        private void NotifyStepEnter()
        {
            var enter = Sequence[Step] as IStepEnter;
            if (enter != null)
                enter.OnStepEnter();
        }
        public void Back()
        {
            btnNext.Text = "Next >";
            pnlStep.Controls.Clear();
            Step--;
            pnlStep.Controls.Add(Sequence[Step]);
            NotifyStepEnter();
            if (Step == 0)
            {
                btnBack.Enabled = false;
            }
            // No going back into the download once it's done (would restart the login/download).
            if (Sequence[Step] is Steps.SaveGameStep)
                btnBack.Enabled = false;
        }

        public bool StepsEnabled = true;
        public void SetStepsEnabled(bool IsEnabled)
        {
            StepsEnabled = IsEnabled;
            btnNext.Enabled = StepsEnabled;
            btnBack.Enabled = StepsEnabled;
        }

        private void Container_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Kill any child processes
            int currentProcessId = Process.GetCurrentProcess().Id;
            ManagementObjectSearcher searcher = new ManagementObjectSearcher
                    ("Select * From Win32_Process Where ParentProcessID=" + currentProcessId);
            ManagementObjectCollection moc = searcher.Get();
            foreach (ManagementObject mo in moc)
            {
                Process proc = Process.GetProcessById(Convert.ToInt32(mo["ProcessID"]));
                try
                {
                    proc.Kill();
                    Thread.Sleep(5000);
                } catch {}
            }            
                
            // Delete Temp Folder            
            try
            {
                Directory.Delete(Program.TempFileLocation, true);
            }
            catch { }
        }
    }
}
