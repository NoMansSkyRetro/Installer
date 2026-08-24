namespace NMSLegacyVersionInstaller.Steps
{
    partial class SaveGameStep
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
            this.grpSave = new System.Windows.Forms.GroupBox();
            this.pbOrb1 = new System.Windows.Forms.PictureBox();
            this.pbOrb2 = new System.Windows.Forms.PictureBox();
            this.pbOrb3 = new System.Windows.Forms.PictureBox();
            this.pbOrb4 = new System.Windows.Forms.PictureBox();
            this.rbVersionId = new System.Windows.Forms.RadioButton();
            this.lblVersionDesc = new System.Windows.Forms.Label();
            this.pbSmart = new System.Windows.Forms.PictureBox();
            this.rbRealId = new System.Windows.Forms.RadioButton();
            this.lblRealDesc = new System.Windows.Forms.Label();
            this.grpSave.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOrb1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOrb2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOrb3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOrb4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSmart)).BeginInit();
            this.SuspendLayout();
            //
            // lblStepTitle
            //
            this.lblStepTitle.AutoSize = true;
            this.lblStepTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepTitle.Location = new System.Drawing.Point(5, 5);
            this.lblStepTitle.Name = "lblStepTitle";
            this.lblStepTitle.Size = new System.Drawing.Size(107, 13);
            this.lblStepTitle.TabIndex = 0;
            this.lblStepTitle.Text = "Save Game Location";
            //
            // lblExplain
            //
            this.lblExplain.Location = new System.Drawing.Point(5, 26);
            this.lblExplain.Name = "lblExplain";
            this.lblExplain.Size = new System.Drawing.Size(610, 24);
            this.lblExplain.TabIndex = 1;
            this.lblExplain.Text = "Choose how your save games are stored. Each legacy version can keep its own saves automatically, or you can tie them to your real Steam account.";
            //
            // grpSave
            //
            this.grpSave.Controls.Add(this.pbOrb1);
            this.grpSave.Controls.Add(this.pbOrb2);
            this.grpSave.Controls.Add(this.pbOrb3);
            this.grpSave.Controls.Add(this.pbOrb4);
            this.grpSave.Controls.Add(this.rbVersionId);
            this.grpSave.Controls.Add(this.lblVersionDesc);
            this.grpSave.Controls.Add(this.pbSmart);
            this.grpSave.Controls.Add(this.rbRealId);
            this.grpSave.Controls.Add(this.lblRealDesc);
            this.grpSave.Location = new System.Drawing.Point(12, 60);
            this.grpSave.Name = "grpSave";
            this.grpSave.Size = new System.Drawing.Size(600, 240);
            this.grpSave.TabIndex = 2;
            this.grpSave.TabStop = false;
            this.grpSave.Text = "Save Games";
            //
            // pbOrb1
            //
            this.pbOrb1.Image = global::NMSLegacyVersionInstaller.Properties.Resources.release_orb;
            this.pbOrb1.Location = new System.Drawing.Point(22, 30);
            this.pbOrb1.Name = "pbOrb1";
            this.pbOrb1.Size = new System.Drawing.Size(32, 32);
            this.pbOrb1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbOrb1.TabIndex = 0;
            this.pbOrb1.TabStop = false;
            //
            // pbOrb2
            //
            this.pbOrb2.Image = global::NMSLegacyVersionInstaller.Properties.Resources.foundation_orb;
            this.pbOrb2.Location = new System.Drawing.Point(58, 30);
            this.pbOrb2.Name = "pbOrb2";
            this.pbOrb2.Size = new System.Drawing.Size(32, 32);
            this.pbOrb2.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbOrb2.TabIndex = 1;
            this.pbOrb2.TabStop = false;
            //
            // pbOrb3
            //
            this.pbOrb3.Image = global::NMSLegacyVersionInstaller.Properties.Resources.path_finder_orb;
            this.pbOrb3.Location = new System.Drawing.Point(22, 66);
            this.pbOrb3.Name = "pbOrb3";
            this.pbOrb3.Size = new System.Drawing.Size(32, 32);
            this.pbOrb3.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbOrb3.TabIndex = 2;
            this.pbOrb3.TabStop = false;
            //
            // pbOrb4
            //
            this.pbOrb4.Image = global::NMSLegacyVersionInstaller.Properties.Resources.atlas_rises_orb;
            this.pbOrb4.Location = new System.Drawing.Point(58, 66);
            this.pbOrb4.Name = "pbOrb4";
            this.pbOrb4.Size = new System.Drawing.Size(32, 32);
            this.pbOrb4.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbOrb4.TabIndex = 3;
            this.pbOrb4.TabStop = false;
            //
            // rbVersionId
            //
            this.rbVersionId.AutoSize = true;
            this.rbVersionId.Checked = true;
            this.rbVersionId.Location = new System.Drawing.Point(115, 32);
            this.rbVersionId.Name = "rbVersionId";
            this.rbVersionId.Size = new System.Drawing.Size(268, 19);
            this.rbVersionId.TabIndex = 0;
            this.rbVersionId.TabStop = true;
            this.rbVersionId.Text = "Version-based Steam ID (recommended)";
            this.rbVersionId.UseVisualStyleBackColor = true;
            //
            // lblVersionDesc
            //
            this.lblVersionDesc.Location = new System.Drawing.Point(117, 55);
            this.lblVersionDesc.Name = "lblVersionDesc";
            this.lblVersionDesc.Size = new System.Drawing.Size(465, 45);
            this.lblVersionDesc.TabIndex = 4;
            this.lblVersionDesc.Text = "Every installed version keeps its own separate save folder automatically, using a unique Steam ID per version. No extra tools to manage - recommended for most people.";
            //
            // pbSmart
            //
            this.pbSmart.Image = global::NMSLegacyVersionInstaller.Properties.Resources.smartsavefolder;
            this.pbSmart.Location = new System.Drawing.Point(30, 140);
            this.pbSmart.Name = "pbSmart";
            this.pbSmart.Size = new System.Drawing.Size(48, 48);
            this.pbSmart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSmart.TabIndex = 5;
            this.pbSmart.TabStop = false;
            //
            // rbRealId
            //
            this.rbRealId.AutoSize = true;
            this.rbRealId.Location = new System.Drawing.Point(115, 140);
            this.rbRealId.Name = "rbRealId";
            this.rbRealId.Size = new System.Drawing.Size(285, 19);
            this.rbRealId.TabIndex = 1;
            this.rbRealId.Text = "Use your real Steam User ID (SmartSaveFolder)";
            this.rbRealId.UseVisualStyleBackColor = true;
            //
            // lblRealDesc
            //
            this.lblRealDesc.Location = new System.Drawing.Point(117, 163);
            this.lblRealDesc.Name = "lblRealDesc";
            this.lblRealDesc.Size = new System.Drawing.Size(465, 45);
            this.lblRealDesc.TabIndex = 6;
            this.lblRealDesc.Text = "All versions share one save folder tied to your real Steam account. SmartSaveFolder is included so you can switch saves between versions.";
            //
            // SaveGameStep
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.grpSave);
            this.Controls.Add(this.lblExplain);
            this.Controls.Add(this.lblStepTitle);
            this.Name = "SaveGameStep";
            this.Size = new System.Drawing.Size(624, 470);
            this.grpSave.ResumeLayout(false);
            this.grpSave.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbOrb1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOrb2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOrb3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbOrb4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pbSmart)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private System.Windows.Forms.Label lblStepTitle;
        private System.Windows.Forms.Label lblExplain;
        private System.Windows.Forms.GroupBox grpSave;
        private System.Windows.Forms.PictureBox pbOrb1;
        private System.Windows.Forms.PictureBox pbOrb2;
        private System.Windows.Forms.PictureBox pbOrb3;
        private System.Windows.Forms.PictureBox pbOrb4;
        public System.Windows.Forms.RadioButton rbVersionId;
        private System.Windows.Forms.Label lblVersionDesc;
        private System.Windows.Forms.PictureBox pbSmart;
        public System.Windows.Forms.RadioButton rbRealId;
        private System.Windows.Forms.Label lblRealDesc;
    }
}
