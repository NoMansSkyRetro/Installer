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
            this.tlpFolders = new System.Windows.Forms.TableLayoutPanel();
            this.lblHdrVersion = new System.Windows.Forms.Label();
            this.lblHdrFolder = new System.Windows.Forms.Label();
            this.lblName1 = new System.Windows.Forms.Label();
            this.lblPre1 = new System.Windows.Forms.Label();
            this.lblVer1 = new System.Windows.Forms.Label();
            this.lblName2 = new System.Windows.Forms.Label();
            this.lblPre2 = new System.Windows.Forms.Label();
            this.lblVer2 = new System.Windows.Forms.Label();
            this.lblName3 = new System.Windows.Forms.Label();
            this.lblPre3 = new System.Windows.Forms.Label();
            this.lblVer3 = new System.Windows.Forms.Label();
            this.lblName4 = new System.Windows.Forms.Label();
            this.lblPre4 = new System.Windows.Forms.Label();
            this.lblVer4 = new System.Windows.Forms.Label();
            this.lblFolderNote = new System.Windows.Forms.Label();
            this.pbSmart = new System.Windows.Forms.PictureBox();
            this.rbRealId = new System.Windows.Forms.RadioButton();
            this.lblRealDesc = new System.Windows.Forms.Label();
            this.cmbSteamUser = new System.Windows.Forms.ComboBox();
            this.grpSave.SuspendLayout();
            this.tlpFolders.SuspendLayout();
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
            this.grpSave.Controls.Add(this.tlpFolders);
            this.grpSave.Controls.Add(this.lblFolderNote);
            this.grpSave.Controls.Add(this.pbSmart);
            this.grpSave.Controls.Add(this.rbRealId);
            this.grpSave.Controls.Add(this.lblRealDesc);
            this.grpSave.Controls.Add(this.cmbSteamUser);
            this.grpSave.Location = new System.Drawing.Point(12, 60);
            this.grpSave.Name = "grpSave";
            this.grpSave.Size = new System.Drawing.Size(600, 370);
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
            // tlpFolders
            //
            this.tlpFolders.AutoSize = true;
            this.tlpFolders.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.tlpFolders.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(64)))));
            this.tlpFolders.CellBorderStyle = System.Windows.Forms.TableLayoutPanelCellBorderStyle.None;
            this.tlpFolders.Padding = new System.Windows.Forms.Padding(5);
            this.tlpFolders.ColumnCount = 3;
            this.tlpFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFolders.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
            this.tlpFolders.Controls.Add(this.lblHdrVersion, 0, 0);
            this.tlpFolders.Controls.Add(this.lblHdrFolder, 1, 0);
            this.tlpFolders.Controls.Add(this.lblName1, 0, 1);
            this.tlpFolders.Controls.Add(this.lblPre1, 1, 1);
            this.tlpFolders.Controls.Add(this.lblVer1, 2, 1);
            this.tlpFolders.Controls.Add(this.lblName2, 0, 2);
            this.tlpFolders.Controls.Add(this.lblPre2, 1, 2);
            this.tlpFolders.Controls.Add(this.lblVer2, 2, 2);
            this.tlpFolders.Controls.Add(this.lblName3, 0, 3);
            this.tlpFolders.Controls.Add(this.lblPre3, 1, 3);
            this.tlpFolders.Controls.Add(this.lblVer3, 2, 3);
            this.tlpFolders.Controls.Add(this.lblName4, 0, 4);
            this.tlpFolders.Controls.Add(this.lblPre4, 1, 4);
            this.tlpFolders.Controls.Add(this.lblVer4, 2, 4);
            this.tlpFolders.SetColumnSpan(this.lblHdrFolder, 2);
            this.tlpFolders.ForeColor = System.Drawing.Color.Cyan;
            this.tlpFolders.Location = new System.Drawing.Point(117, 105);
            this.tlpFolders.Name = "tlpFolders";
            this.tlpFolders.RowCount = 5;
            this.tlpFolders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFolders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFolders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFolders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFolders.RowStyles.Add(new System.Windows.Forms.RowStyle());
            this.tlpFolders.Size = new System.Drawing.Size(300, 105);
            this.tlpFolders.TabIndex = 7;
            //
            // lblHdrVersion
            //
            this.lblHdrVersion.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblHdrVersion.AutoSize = true;
            this.lblHdrVersion.ForeColor = System.Drawing.Color.Lime;
            this.lblHdrVersion.Name = "lblHdrVersion";
            this.lblHdrVersion.Text = "Version";
            //
            // lblHdrFolder
            //
            this.lblHdrFolder.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblHdrFolder.AutoSize = true;
            this.lblHdrFolder.ForeColor = System.Drawing.Color.Lime;
            this.lblHdrFolder.Name = "lblHdrFolder";
            this.lblHdrFolder.Text = "Save folder";
            //
            // lblName1
            //
            this.lblName1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblName1.AutoSize = true;
            this.lblName1.Name = "lblName1";
            this.lblName1.Text = "Initial Release";
            //
            // lblPre1
            //
            this.lblPre1.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPre1.AutoSize = true;
            this.lblPre1.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblPre1.Name = "lblPre1";
            this.lblPre1.Text = "st_76561197960266";
            //
            // lblVer1
            //
            this.lblVer1.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVer1.AutoSize = true;
            this.lblVer1.ForeColor = System.Drawing.Color.Orange;
            this.lblVer1.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lblVer1.Name = "lblVer1";
            this.lblVer1.Text = "109";
            //
            // lblName2
            //
            this.lblName2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblName2.AutoSize = true;
            this.lblName2.Name = "lblName2";
            this.lblName2.Text = "Foundation";
            //
            // lblPre2
            //
            this.lblPre2.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPre2.AutoSize = true;
            this.lblPre2.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblPre2.Name = "lblPre2";
            this.lblPre2.Text = "st_76561197960266";
            //
            // lblVer2
            //
            this.lblVer2.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVer2.AutoSize = true;
            this.lblVer2.ForeColor = System.Drawing.Color.Orange;
            this.lblVer2.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lblVer2.Name = "lblVer2";
            this.lblVer2.Text = "113";
            //
            // lblName3
            //
            this.lblName3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblName3.AutoSize = true;
            this.lblName3.Name = "lblName3";
            this.lblName3.Text = "Path Finder";
            //
            // lblPre3
            //
            this.lblPre3.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPre3.AutoSize = true;
            this.lblPre3.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblPre3.Name = "lblPre3";
            this.lblPre3.Text = "st_76561197960266";
            //
            // lblVer3
            //
            this.lblVer3.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVer3.AutoSize = true;
            this.lblVer3.ForeColor = System.Drawing.Color.Orange;
            this.lblVer3.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lblVer3.Name = "lblVer3";
            this.lblVer3.Text = "124";
            //
            // lblName4
            //
            this.lblName4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblName4.AutoSize = true;
            this.lblName4.Name = "lblName4";
            this.lblName4.Text = "Atlas Rises";
            //
            // lblPre4
            //
            this.lblPre4.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.lblPre4.AutoSize = true;
            this.lblPre4.Margin = new System.Windows.Forms.Padding(3, 3, 0, 3);
            this.lblPre4.Name = "lblPre4";
            this.lblPre4.Text = "st_76561197960266";
            //
            // lblVer4
            //
            this.lblVer4.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.lblVer4.AutoSize = true;
            this.lblVer4.ForeColor = System.Drawing.Color.Orange;
            this.lblVer4.Margin = new System.Windows.Forms.Padding(0, 3, 3, 3);
            this.lblVer4.Name = "lblVer4";
            this.lblVer4.Text = "138";
            //
            // lblFolderNote
            //
            this.lblFolderNote.AutoSize = true;
            this.lblFolderNote.ForeColor = System.Drawing.SystemColors.GrayText;
            this.lblFolderNote.Location = new System.Drawing.Point(117, 230);
            this.lblFolderNote.Name = "lblFolderNote";
            this.lblFolderNote.Size = new System.Drawing.Size(300, 13);
            this.lblFolderNote.TabIndex = 8;
            this.lblFolderNote.Text = "Highlighted = version number. Each version saves to its own folder.";
            //
            // pbSmart
            //
            this.pbSmart.Image = global::NMSLegacyVersionInstaller.Properties.Resources.smartsavefolder;
            this.pbSmart.Location = new System.Drawing.Point(30, 255);
            this.pbSmart.Name = "pbSmart";
            this.pbSmart.Size = new System.Drawing.Size(48, 48);
            this.pbSmart.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pbSmart.TabIndex = 5;
            this.pbSmart.TabStop = false;
            //
            // rbRealId
            //
            this.rbRealId.AutoSize = true;
            this.rbRealId.Location = new System.Drawing.Point(115, 258);
            this.rbRealId.Name = "rbRealId";
            this.rbRealId.Size = new System.Drawing.Size(285, 19);
            this.rbRealId.TabIndex = 1;
            this.rbRealId.Text = "Use your real Steam User ID (SmartSaveFolder)";
            this.rbRealId.UseVisualStyleBackColor = true;
            //
            // lblRealDesc
            //
            this.lblRealDesc.Location = new System.Drawing.Point(117, 281);
            this.lblRealDesc.Name = "lblRealDesc";
            this.lblRealDesc.Size = new System.Drawing.Size(465, 45);
            this.lblRealDesc.TabIndex = 6;
            this.lblRealDesc.Text = "All versions share one save folder tied to your real Steam account. SmartSaveFolder is included so you can switch saves between versions.";
            //
            // cmbSteamUser
            //
            this.cmbSteamUser.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSteamUser.FormattingEnabled = true;
            this.cmbSteamUser.Location = new System.Drawing.Point(117, 330);
            this.cmbSteamUser.Name = "cmbSteamUser";
            this.cmbSteamUser.Size = new System.Drawing.Size(260, 21);
            this.cmbSteamUser.TabIndex = 9;
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
            this.tlpFolders.ResumeLayout(false);
            this.tlpFolders.PerformLayout();
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
        private System.Windows.Forms.TableLayoutPanel tlpFolders;
        private System.Windows.Forms.Label lblHdrVersion;
        private System.Windows.Forms.Label lblHdrFolder;
        private System.Windows.Forms.Label lblName1;
        private System.Windows.Forms.Label lblPre1;
        private System.Windows.Forms.Label lblVer1;
        private System.Windows.Forms.Label lblName2;
        private System.Windows.Forms.Label lblPre2;
        private System.Windows.Forms.Label lblVer2;
        private System.Windows.Forms.Label lblName3;
        private System.Windows.Forms.Label lblPre3;
        private System.Windows.Forms.Label lblVer3;
        private System.Windows.Forms.Label lblName4;
        private System.Windows.Forms.Label lblPre4;
        private System.Windows.Forms.Label lblVer4;
        private System.Windows.Forms.Label lblFolderNote;
        private System.Windows.Forms.PictureBox pbSmart;
        public System.Windows.Forms.RadioButton rbRealId;
        private System.Windows.Forms.Label lblRealDesc;
        private System.Windows.Forms.ComboBox cmbSteamUser;
    }
}
