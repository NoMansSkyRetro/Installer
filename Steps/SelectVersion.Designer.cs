namespace NMSLegacyVersionInstaller.Steps
{
    partial class SelectVersion
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
            this.pb01 = new System.Windows.Forms.PictureBox();
            this.pb02 = new System.Windows.Forms.PictureBox();
            this.pb03 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.lblStepTitle = new System.Windows.Forms.Label();
            this.rb01 = new System.Windows.Forms.CheckBox();
            this.grpOptions = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.txtPath = new System.Windows.Forms.TextBox();
            this.lblPath = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.rb02 = new System.Windows.Forms.CheckBox();
            this.panel3 = new System.Windows.Forms.Panel();
            this.rb03 = new System.Windows.Forms.CheckBox();
            this.panel4 = new System.Windows.Forms.Panel();
            this.rb04 = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.pb01)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb02)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb03)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            this.grpOptions.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.SuspendLayout();
            //
            // pb01
            //
            this.pb01.Image = global::NMSLegacyVersionInstaller.Properties.Resources.banner01;
            this.pb01.Location = new System.Drawing.Point(12, 30);
            this.pb01.Name = "pb01";
            this.pb01.Size = new System.Drawing.Size(600, 65);
            this.pb01.TabIndex = 0;
            this.pb01.TabStop = false;
            //
            // pb02
            //
            this.pb02.Image = global::NMSLegacyVersionInstaller.Properties.Resources.banner02;
            this.pb02.Location = new System.Drawing.Point(12, 110);
            this.pb02.Name = "pb02";
            this.pb02.Size = new System.Drawing.Size(600, 65);
            this.pb02.TabIndex = 1;
            this.pb02.TabStop = false;
            //
            // pb03
            //
            this.pb03.Image = global::NMSLegacyVersionInstaller.Properties.Resources.banner03;
            this.pb03.Location = new System.Drawing.Point(12, 190);
            this.pb03.Name = "pb03";
            this.pb03.Size = new System.Drawing.Size(600, 65);
            this.pb03.TabIndex = 2;
            this.pb03.TabStop = false;
            //
            // pictureBox4
            //
            this.pictureBox4.Image = global::NMSLegacyVersionInstaller.Properties.Resources.banner04;
            this.pictureBox4.Location = new System.Drawing.Point(12, 270);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(600, 65);
            this.pictureBox4.TabIndex = 3;
            this.pictureBox4.TabStop = false;
            //
            // lblStepTitle
            //
            this.lblStepTitle.AutoSize = true;
            this.lblStepTitle.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStepTitle.Location = new System.Drawing.Point(5, 5);
            this.lblStepTitle.Name = "lblStepTitle";
            this.lblStepTitle.Size = new System.Drawing.Size(122, 13);
            this.lblStepTitle.TabIndex = 4;
            this.lblStepTitle.Text = "Select Versions to Install";
            //
            // rb01
            //
            this.rb01.Checked = true;
            this.rb01.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rb01.Location = new System.Drawing.Point(3, 3);
            this.rb01.Name = "rb01";
            this.rb01.Size = new System.Drawing.Size(70, 20);
            this.rb01.TabIndex = 5;
            this.rb01.Text = "Install";
            this.rb01.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rb01.UseVisualStyleBackColor = true;
            //
            // grpOptions
            //
            this.grpOptions.Controls.Add(this.btnBrowse);
            this.grpOptions.Controls.Add(this.txtPath);
            this.grpOptions.Controls.Add(this.lblPath);
            this.grpOptions.Location = new System.Drawing.Point(12, 347);
            this.grpOptions.Name = "grpOptions";
            this.grpOptions.Size = new System.Drawing.Size(600, 93);
            this.grpOptions.TabIndex = 13;
            this.grpOptions.TabStop = false;
            this.grpOptions.Text = "Installation Options";
            //
            // btnBrowse
            //
            this.btnBrowse.Location = new System.Drawing.Point(555, 39);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(30, 23);
            this.btnBrowse.TabIndex = 2;
            this.btnBrowse.Text = "...";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            //
            // txtPath
            //
            this.txtPath.Location = new System.Drawing.Point(9, 41);
            this.txtPath.Name = "txtPath";
            this.txtPath.Size = new System.Drawing.Size(540, 20);
            this.txtPath.TabIndex = 1;
            this.txtPath.Text = "C:\\NMSLegacy\\";
            //
            // lblPath
            //
            this.lblPath.AutoSize = true;
            this.lblPath.Location = new System.Drawing.Point(6, 25);
            this.lblPath.Name = "lblPath";
            this.lblPath.Size = new System.Drawing.Size(108, 13);
            this.lblPath.TabIndex = 0;
            this.lblPath.Text = "Base path to install to";
            //
            // panel1
            //
            this.panel1.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel1.Controls.Add(this.rb01);
            this.panel1.Location = new System.Drawing.Point(526, 45);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(80, 30);
            this.panel1.TabIndex = 14;
            //
            // panel2
            //
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel2.Controls.Add(this.rb02);
            this.panel2.Location = new System.Drawing.Point(526, 125);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(80, 30);
            this.panel2.TabIndex = 15;
            //
            // rb02
            //
            this.rb02.Checked = true;
            this.rb02.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rb02.Location = new System.Drawing.Point(3, 3);
            this.rb02.Name = "rb02";
            this.rb02.Size = new System.Drawing.Size(70, 20);
            this.rb02.TabIndex = 5;
            this.rb02.Text = "Install";
            this.rb02.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rb02.UseVisualStyleBackColor = true;
            //
            // panel3
            //
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.rb03);
            this.panel3.Location = new System.Drawing.Point(526, 205);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(80, 30);
            this.panel3.TabIndex = 16;
            //
            // rb03
            //
            this.rb03.Checked = true;
            this.rb03.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rb03.Location = new System.Drawing.Point(3, 3);
            this.rb03.Name = "rb03";
            this.rb03.Size = new System.Drawing.Size(70, 20);
            this.rb03.TabIndex = 5;
            this.rb03.Text = "Install";
            this.rb03.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rb03.UseVisualStyleBackColor = true;
            //
            // panel4
            //
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.rb04);
            this.panel4.Location = new System.Drawing.Point(526, 285);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(80, 30);
            this.panel4.TabIndex = 17;
            //
            // rb04
            //
            this.rb04.Checked = true;
            this.rb04.CheckState = System.Windows.Forms.CheckState.Checked;
            this.rb04.Location = new System.Drawing.Point(3, 3);
            this.rb04.Name = "rb04";
            this.rb04.Size = new System.Drawing.Size(70, 20);
            this.rb04.TabIndex = 5;
            this.rb04.Text = "Install";
            this.rb04.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.rb04.UseVisualStyleBackColor = true;
            //
            // SelectVersion
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.grpOptions);
            this.Controls.Add(this.lblStepTitle);
            this.Controls.Add(this.pictureBox4);
            this.Controls.Add(this.pb03);
            this.Controls.Add(this.pb02);
            this.Controls.Add(this.pb01);
            this.Name = "SelectVersion";
            this.Size = new System.Drawing.Size(624, 470);
            this.Load += new System.EventHandler(this.SelectVersion_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb01)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb02)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pb03)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            this.grpOptions.ResumeLayout(false);
            this.grpOptions.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel3.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.PictureBox pb01;
        private System.Windows.Forms.PictureBox pb02;
        private System.Windows.Forms.PictureBox pb03;
        private System.Windows.Forms.PictureBox pictureBox4;
        private System.Windows.Forms.Label lblStepTitle;
        private System.Windows.Forms.GroupBox grpOptions;
        private System.Windows.Forms.Label lblPath;
        public System.Windows.Forms.CheckBox rb01;
        public System.Windows.Forms.TextBox txtPath;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        public System.Windows.Forms.CheckBox rb02;
        private System.Windows.Forms.Panel panel3;
        public System.Windows.Forms.CheckBox rb03;
        private System.Windows.Forms.Panel panel4;
        public System.Windows.Forms.CheckBox rb04;
    }
}
