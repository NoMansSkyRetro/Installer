namespace NMSLegacyVersionInstaller.Steps
{
    partial class Complete
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.lblStepTitle = new System.Windows.Forms.Label();
            this.btnOpen = new System.Windows.Forms.Button();
            this.lblExplain = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pbDiscord = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btnDiscord = new System.Windows.Forms.Button();
            this.btnSmartSaveFolder = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDiscord)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            //
            // lblStepTitle
            //
            this.lblStepTitle.AutoSize = true;
            this.lblStepTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepTitle.Location = new System.Drawing.Point(5, 5);
            this.lblStepTitle.Name = "lblStepTitle";
            this.lblStepTitle.Size = new System.Drawing.Size(51, 13);
            this.lblStepTitle.TabIndex = 0;
            this.lblStepTitle.Text = "Complete";
            //
            // btnOpen
            //
            this.btnOpen.Location = new System.Drawing.Point(8, 50);
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(201, 51);
            this.btnOpen.TabIndex = 1;
            this.btnOpen.Text = "Open Installation Folder";
            this.btnOpen.UseVisualStyleBackColor = true;
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            //
            // lblExplain
            //
            this.lblExplain.AutoSize = true;
            this.lblExplain.Location = new System.Drawing.Point(5, 24);
            this.lblExplain.Name = "lblExplain";
            this.lblExplain.Size = new System.Drawing.Size(116, 13);
            this.lblExplain.TabIndex = 6;
            this.lblExplain.Text = "Installation is complete.";
            //
            // panel1
            //
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.btnDiscord);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pbDiscord);
            this.panel1.Location = new System.Drawing.Point(8, 118);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(605, 73);
            this.panel1.TabIndex = 7;
            //
            // panel2
            //
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.btnSmartSaveFolder);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.pictureBox1);
            this.panel2.Location = new System.Drawing.Point(8, 197);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(605, 73);
            this.panel2.TabIndex = 8;
            //
            // pbDiscord
            //
            this.pbDiscord.Image = global::NMSLegacyVersionInstaller.Properties.Resources.discord;
            this.pbDiscord.Location = new System.Drawing.Point(8, 10);
            this.pbDiscord.Name = "pbDiscord";
            this.pbDiscord.Size = new System.Drawing.Size(48, 48);
            this.pbDiscord.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pbDiscord.TabIndex = 0;
            this.pbDiscord.TabStop = false;
            //
            // pictureBox1
            //
            this.pictureBox1.Image = global::NMSLegacyVersionInstaller.Properties.Resources.smartsavefolder;
            this.pictureBox1.Location = new System.Drawing.Point(8, 11);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.StretchImage;
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            //
            // label1
            //
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(62, 10);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(401, 13);
            this.label1.TabIndex = 10;
            this.label1.Text = "Need help or want to talk about legacy NMS? Join the No Man\'s Sky Retro discord.";
            //
            // label2
            //
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(62, 11);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(389, 13);
            this.label2.TabIndex = 11;
            this.label2.Text = "Keep all your different NMS version savegames seperated with SmartSaveFolder.";
            //
            // btnDiscord
            //
            this.btnDiscord.Location = new System.Drawing.Point(65, 35);
            this.btnDiscord.Name = "btnDiscord";
            this.btnDiscord.Size = new System.Drawing.Size(266, 23);
            this.btnDiscord.TabIndex = 11;
            this.btnDiscord.Text = "Open Discord";
            this.btnDiscord.UseVisualStyleBackColor = true;
            this.btnDiscord.Click += new System.EventHandler(this.btnDiscord_Click);
            //
            // btnSmartSaveFolder
            //
            this.btnSmartSaveFolder.Location = new System.Drawing.Point(65, 36);
            this.btnSmartSaveFolder.Name = "btnSmartSaveFolder";
            this.btnSmartSaveFolder.Size = new System.Drawing.Size(266, 23);
            this.btnSmartSaveFolder.TabIndex = 12;
            this.btnSmartSaveFolder.Text = "Open SmartSaveFolder";
            this.btnSmartSaveFolder.UseVisualStyleBackColor = true;
            this.btnSmartSaveFolder.Click += new System.EventHandler(this.btnSmartSaveFolder_Click);
            //
            // Complete
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lblExplain);
            this.Controls.Add(this.btnOpen);
            this.Controls.Add(this.lblStepTitle);
            this.Name = "Complete";
            this.Size = new System.Drawing.Size(624, 470);
            this.Load += new System.EventHandler(this.Complete_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pbDiscord)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblStepTitle;
        private System.Windows.Forms.Button btnOpen;
        private System.Windows.Forms.Label lblExplain;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.PictureBox pbDiscord;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDiscord;
        private System.Windows.Forms.Button btnSmartSaveFolder;
    }
}
