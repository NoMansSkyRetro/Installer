namespace NMSLegacyVersionInstaller.Steps
{
    partial class ShaderFixStep
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        private void InitializeComponent()
        {
            this.lblStepTitle = new System.Windows.Forms.Label();
            this.lblExplain = new System.Windows.Forms.Label();
            this.grpShader = new System.Windows.Forms.GroupBox();
            this.pbShader = new System.Windows.Forms.PictureBox();
            this.rbShaderAuto = new System.Windows.Forms.RadioButton();
            this.lblAutoDesc = new System.Windows.Forms.Label();
            this.pbSkip = new System.Windows.Forms.PictureBox();
            this.rbShaderSkip = new System.Windows.Forms.RadioButton();
            this.lblSkipDesc = new System.Windows.Forms.Label();
            this.lblCredit = new System.Windows.Forms.LinkLabel();
            this.grpShader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbShader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSkip)).BeginInit();
            this.SuspendLayout();
            //
            // lblStepTitle
            //
            this.lblStepTitle.AutoSize = true;
            this.lblStepTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepTitle.Location = new System.Drawing.Point(5, 5);
            this.lblStepTitle.Name = "lblStepTitle";
            this.lblStepTitle.Size = new System.Drawing.Size(56, 13);
            this.lblStepTitle.TabIndex = 0;
            this.lblStepTitle.Text = "Shader Fix";
            //
            // lblExplain
            //
            this.lblExplain.Location = new System.Drawing.Point(5, 26);
            this.lblExplain.Name = "lblExplain";
            this.lblExplain.Size = new System.Drawing.Size(610, 24);
            this.lblExplain.TabIndex = 1;
            this.lblExplain.Text = "Legacy versions can show graphical glitches on modern graphics cards. RetroShaderFix corrects the shaders so the game renders the way it should.";
            //
            // grpShader
            //
            this.grpShader.Controls.Add(this.pbShader);
            this.grpShader.Controls.Add(this.rbShaderAuto);
            this.grpShader.Controls.Add(this.lblAutoDesc);
            this.grpShader.Controls.Add(this.pbSkip);
            this.grpShader.Controls.Add(this.rbShaderSkip);
            this.grpShader.Controls.Add(this.lblSkipDesc);
            this.grpShader.Controls.Add(this.lblCredit);
            this.grpShader.Location = new System.Drawing.Point(12, 60);
            this.grpShader.Name = "grpShader";
            this.grpShader.Size = new System.Drawing.Size(600, 240);
            this.grpShader.TabIndex = 2;
            this.grpShader.TabStop = false;
            this.grpShader.Text = "Shader Fix";
            //
            // pbShader
            //
            this.pbShader.Image = global::NMSLegacyVersionInstaller.Properties.Resources.retroshaderfixgui;
            this.pbShader.Location = new System.Drawing.Point(24, 30);
            this.pbShader.Name = "pbShader";
            this.pbShader.Size = new System.Drawing.Size(56, 56);
            this.pbShader.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbShader.TabIndex = 0;
            this.pbShader.TabStop = false;
            //
            // rbShaderAuto
            //
            this.rbShaderAuto.AutoSize = true;
            this.rbShaderAuto.Checked = true;
            this.rbShaderAuto.Location = new System.Drawing.Point(115, 34);
            this.rbShaderAuto.Name = "rbShaderAuto";
            this.rbShaderAuto.Size = new System.Drawing.Size(231, 17);
            this.rbShaderAuto.TabIndex = 0;
            this.rbShaderAuto.TabStop = true;
            this.rbShaderAuto.Text = "Automatically fix shaders (recommended)";
            this.rbShaderAuto.UseVisualStyleBackColor = true;
            //
            // lblAutoDesc
            //
            this.lblAutoDesc.Location = new System.Drawing.Point(117, 55);
            this.lblAutoDesc.Name = "lblAutoDesc";
            this.lblAutoDesc.Size = new System.Drawing.Size(465, 45);
            this.lblAutoDesc.TabIndex = 3;
            this.lblAutoDesc.Text = "Detects your graphics card (AMD or NVIDIA) and applies the correct shader fix to each installed version. Best for most people.";
            //
            // pbSkip
            //
            this.pbSkip.Image = global::NMSLegacyVersionInstaller.Properties.Resources.exit;
            this.pbSkip.Location = new System.Drawing.Point(30, 138);
            this.pbSkip.Name = "pbSkip";
            this.pbSkip.Size = new System.Drawing.Size(44, 44);
            this.pbSkip.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSkip.TabIndex = 7;
            this.pbSkip.TabStop = false;
            //
            // rbShaderSkip
            //
            this.rbShaderSkip.AutoSize = true;
            this.rbShaderSkip.Location = new System.Drawing.Point(115, 142);
            this.rbShaderSkip.Name = "rbShaderSkip";
            this.rbShaderSkip.Size = new System.Drawing.Size(45, 17);
            this.rbShaderSkip.TabIndex = 1;
            this.rbShaderSkip.Text = "Skip";
            this.rbShaderSkip.UseVisualStyleBackColor = true;
            //
            // lblSkipDesc
            //
            this.lblSkipDesc.Location = new System.Drawing.Point(117, 163);
            this.lblSkipDesc.Name = "lblSkipDesc";
            this.lblSkipDesc.Size = new System.Drawing.Size(465, 32);
            this.lblSkipDesc.TabIndex = 4;
            this.lblSkipDesc.Text = "Leave shaders unchanged. You can run RetroShaderFix manually later from your install folder.";
            //
            // lblCredit
            //
            this.lblCredit.AutoSize = true;
            this.lblCredit.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblCredit.LinkArea = new System.Windows.Forms.LinkArea(33, 38);
            this.lblCredit.Location = new System.Drawing.Point(26, 212);
            this.lblCredit.Name = "lblCredit";
            this.lblCredit.Size = new System.Drawing.Size(320, 13);
            this.lblCredit.TabIndex = 5;
            this.lblCredit.TabStop = true;
            this.lblCredit.Text = "RetroShaderFix by EthanRDoesMC - github.com/EthanRDoesMC/RetroShaderFix";
            this.lblCredit.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.lblCredit_LinkClicked);
            //
            // ShaderFixStep
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpShader);
            this.Controls.Add(this.lblExplain);
            this.Controls.Add(this.lblStepTitle);
            this.Name = "ShaderFixStep";
            this.Size = new System.Drawing.Size(624, 470);
            this.grpShader.ResumeLayout(false);
            this.grpShader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbShader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSkip)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblStepTitle;
        private System.Windows.Forms.Label lblExplain;
        private System.Windows.Forms.GroupBox grpShader;
        private System.Windows.Forms.PictureBox pbShader;
        public System.Windows.Forms.RadioButton rbShaderAuto;
        private System.Windows.Forms.Label lblAutoDesc;
        private System.Windows.Forms.PictureBox pbSkip;
        public System.Windows.Forms.RadioButton rbShaderSkip;
        private System.Windows.Forms.Label lblSkipDesc;
        private System.Windows.Forms.LinkLabel lblCredit;
    }
}
