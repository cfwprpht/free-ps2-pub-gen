namespace free_ps2_pub_gen {
    partial class EmulatorOptions {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EmulatorOptions));
            this.labelGsVert = new System.Windows.Forms.Label();
            this.labelIdec = new System.Windows.Forms.Label();
            this.labelGsOpt = new System.Windows.Forms.Label();
            this.labelFpu = new System.Windows.Forms.Label();
            this.labelCop2 = new System.Windows.Forms.Label();
            this.labelVu1Di = new System.Windows.Forms.Label();
            this.labelGsOverride = new System.Windows.Forms.Label();
            this.labelAssertPath = new System.Windows.Forms.Label();
            this.labelEeIgnore = new System.Windows.Forms.Label();
            this.checkGsOpt = new System.Windows.Forms.CheckBox();
            this.checkFpu = new System.Windows.Forms.CheckBox();
            this.checkCop2 = new System.Windows.Forms.CheckBox();
            this.checkVu1Di = new System.Windows.Forms.CheckBox();
            this.checkGsOverride = new System.Windows.Forms.CheckBox();
            this.checkAssertPath = new System.Windows.Forms.CheckBox();
            this.textBoxGsVert = new System.Windows.Forms.TextBox();
            this.textBoxIdec = new System.Windows.Forms.TextBox();
            this.comboEeIgnore = new System.Windows.Forms.ComboBox();
            this.buttonSave = new System.Windows.Forms.Button();
            this.buttonReset = new System.Windows.Forms.Button();
            this.rtbEeHook = new System.Windows.Forms.RichTextBox();
            this.labelEeHook = new System.Windows.Forms.Label();
            this.buttonEditScript = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // labelGsVert
            // 
            this.labelGsVert.AutoSize = true;
            this.labelGsVert.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGsVert.Location = new System.Drawing.Point(11, 9);
            this.labelGsVert.Name = "labelGsVert";
            this.labelGsVert.Size = new System.Drawing.Size(104, 15);
            this.labelGsVert.TabIndex = 0;
            this.labelGsVert.Text = "gs-vert-precision=";
            // 
            // labelIdec
            // 
            this.labelIdec.AutoSize = true;
            this.labelIdec.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelIdec.Location = new System.Drawing.Point(11, 32);
            this.labelIdec.Name = "labelIdec";
            this.labelIdec.Size = new System.Drawing.Size(122, 15);
            this.labelIdec.TabIndex = 1;
            this.labelIdec.Text = "idec-cycles-per-qwc=";
            // 
            // labelGsOpt
            // 
            this.labelGsOpt.AutoSize = true;
            this.labelGsOpt.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGsOpt.Location = new System.Drawing.Point(11, 54);
            this.labelGsOpt.Name = "labelGsOpt";
            this.labelGsOpt.Size = new System.Drawing.Size(112, 15);
            this.labelGsOpt.TabIndex = 2;
            this.labelGsOpt.Text = "gs-optimize-30fps=";
            // 
            // labelFpu
            // 
            this.labelFpu.AutoSize = true;
            this.labelFpu.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelFpu.Location = new System.Drawing.Point(11, 75);
            this.labelFpu.Name = "labelFpu";
            this.labelFpu.Size = new System.Drawing.Size(104, 15);
            this.labelFpu.TabIndex = 3;
            this.labelFpu.Text = "fpu-no-clamping=";
            // 
            // labelCop2
            // 
            this.labelCop2.AutoSize = true;
            this.labelCop2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelCop2.Location = new System.Drawing.Point(11, 97);
            this.labelCop2.Name = "labelCop2";
            this.labelCop2.Size = new System.Drawing.Size(114, 15);
            this.labelCop2.TabIndex = 4;
            this.labelCop2.Text = "cop2-no-clamping=";
            // 
            // labelVu1Di
            // 
            this.labelVu1Di.AutoSize = true;
            this.labelVu1Di.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelVu1Di.Location = new System.Drawing.Point(11, 119);
            this.labelVu1Di.Name = "labelVu1Di";
            this.labelVu1Di.Size = new System.Drawing.Size(70, 15);
            this.labelVu1Di.TabIndex = 5;
            this.labelVu1Di.Text = "vu1-di-bits=";
            // 
            // labelGsOverride
            // 
            this.labelGsOverride.AutoSize = true;
            this.labelGsOverride.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelGsOverride.Location = new System.Drawing.Point(11, 141);
            this.labelGsOverride.Name = "labelGsOverride";
            this.labelGsOverride.Size = new System.Drawing.Size(152, 15);
            this.labelGsOverride.TabIndex = 6;
            this.labelGsOverride.Text = "gs-override-small-tri-area=";
            // 
            // labelAssertPath
            // 
            this.labelAssertPath.AutoSize = true;
            this.labelAssertPath.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelAssertPath.Location = new System.Drawing.Point(11, 164);
            this.labelAssertPath.Name = "labelAssertPath";
            this.labelAssertPath.Size = new System.Drawing.Size(100, 15);
            this.labelAssertPath.TabIndex = 7;
            this.labelAssertPath.Text = "assert-path1-ad=";
            // 
            // labelEeIgnore
            // 
            this.labelEeIgnore.AutoSize = true;
            this.labelEeIgnore.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEeIgnore.Location = new System.Drawing.Point(11, 186);
            this.labelEeIgnore.Name = "labelEeIgnore";
            this.labelEeIgnore.Size = new System.Drawing.Size(114, 15);
            this.labelEeIgnore.TabIndex = 8;
            this.labelEeIgnore.Text = "ee-ignore-segfault=";
            // 
            // checkGsOpt
            // 
            this.checkGsOpt.AutoSize = true;
            this.checkGsOpt.Location = new System.Drawing.Point(169, 56);
            this.checkGsOpt.Name = "checkGsOpt";
            this.checkGsOpt.Size = new System.Drawing.Size(15, 14);
            this.checkGsOpt.TabIndex = 9;
            this.checkGsOpt.UseVisualStyleBackColor = true;
            this.checkGsOpt.CheckedChanged += new System.EventHandler(this.CheckGsOpt_CheckedChanged);
            // 
            // checkFpu
            // 
            this.checkFpu.AutoSize = true;
            this.checkFpu.Location = new System.Drawing.Point(169, 77);
            this.checkFpu.Name = "checkFpu";
            this.checkFpu.Size = new System.Drawing.Size(15, 14);
            this.checkFpu.TabIndex = 10;
            this.checkFpu.UseVisualStyleBackColor = true;
            this.checkFpu.CheckedChanged += new System.EventHandler(this.CheckFpu_CheckedChanged);
            // 
            // checkCop2
            // 
            this.checkCop2.AutoSize = true;
            this.checkCop2.Location = new System.Drawing.Point(169, 98);
            this.checkCop2.Name = "checkCop2";
            this.checkCop2.Size = new System.Drawing.Size(15, 14);
            this.checkCop2.TabIndex = 11;
            this.checkCop2.UseVisualStyleBackColor = true;
            this.checkCop2.CheckedChanged += new System.EventHandler(this.CheckCop2_CheckedChanged);
            // 
            // checkVu1Di
            // 
            this.checkVu1Di.AutoSize = true;
            this.checkVu1Di.Location = new System.Drawing.Point(169, 121);
            this.checkVu1Di.Name = "checkVu1Di";
            this.checkVu1Di.Size = new System.Drawing.Size(15, 14);
            this.checkVu1Di.TabIndex = 12;
            this.checkVu1Di.UseVisualStyleBackColor = true;
            this.checkVu1Di.CheckedChanged += new System.EventHandler(this.CheckVu1Di_CheckedChanged);
            // 
            // checkGsOverride
            // 
            this.checkGsOverride.AutoSize = true;
            this.checkGsOverride.Location = new System.Drawing.Point(169, 143);
            this.checkGsOverride.Name = "checkGsOverride";
            this.checkGsOverride.Size = new System.Drawing.Size(15, 14);
            this.checkGsOverride.TabIndex = 13;
            this.checkGsOverride.UseVisualStyleBackColor = true;
            this.checkGsOverride.CheckedChanged += new System.EventHandler(this.CheckGsOverride_CheckedChanged);
            // 
            // checkAssertPath
            // 
            this.checkAssertPath.AutoSize = true;
            this.checkAssertPath.Location = new System.Drawing.Point(169, 166);
            this.checkAssertPath.Name = "checkAssertPath";
            this.checkAssertPath.Size = new System.Drawing.Size(15, 14);
            this.checkAssertPath.TabIndex = 14;
            this.checkAssertPath.UseVisualStyleBackColor = true;
            this.checkAssertPath.CheckedChanged += new System.EventHandler(this.CheckAssertPath_CheckedChanged);
            // 
            // textBoxGsVert
            // 
            this.textBoxGsVert.BackColor = System.Drawing.SystemColors.WindowText;
            this.textBoxGsVert.ForeColor = System.Drawing.Color.Yellow;
            this.textBoxGsVert.Location = new System.Drawing.Point(169, 8);
            this.textBoxGsVert.MaxLength = 2;
            this.textBoxGsVert.Name = "textBoxGsVert";
            this.textBoxGsVert.Size = new System.Drawing.Size(20, 20);
            this.textBoxGsVert.TabIndex = 15;
            this.textBoxGsVert.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxGsVert.TextChanged += new System.EventHandler(this.TextBoxGsVert_TextChanged);
            // 
            // textBoxIdec
            // 
            this.textBoxIdec.BackColor = System.Drawing.SystemColors.WindowText;
            this.textBoxIdec.ForeColor = System.Drawing.Color.Yellow;
            this.textBoxIdec.Location = new System.Drawing.Point(169, 31);
            this.textBoxIdec.MaxLength = 3;
            this.textBoxIdec.Name = "textBoxIdec";
            this.textBoxIdec.Size = new System.Drawing.Size(27, 20);
            this.textBoxIdec.TabIndex = 16;
            this.textBoxIdec.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.textBoxIdec.TextChanged += new System.EventHandler(this.TextBoxIdec_TextChanged);
            // 
            // comboEeIgnore
            // 
            this.comboEeIgnore.BackColor = System.Drawing.SystemColors.WindowText;
            this.comboEeIgnore.ForeColor = System.Drawing.Color.Yellow;
            this.comboEeIgnore.FormattingEnabled = true;
            this.comboEeIgnore.Location = new System.Drawing.Point(169, 185);
            this.comboEeIgnore.Name = "comboEeIgnore";
            this.comboEeIgnore.Size = new System.Drawing.Size(54, 21);
            this.comboEeIgnore.TabIndex = 17;
            this.comboEeIgnore.SelectedIndexChanged += new System.EventHandler(this.ComboEeIgnore_SelectedIndexChanged);
            // 
            // buttonSave
            // 
            this.buttonSave.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonSave.Location = new System.Drawing.Point(14, 358);
            this.buttonSave.Name = "buttonSave";
            this.buttonSave.Size = new System.Drawing.Size(97, 42);
            this.buttonSave.TabIndex = 18;
            this.buttonSave.Text = "Save";
            this.buttonSave.UseVisualStyleBackColor = true;
            this.buttonSave.Click += new System.EventHandler(this.ButtonSave_Click);
            // 
            // buttonReset
            // 
            this.buttonReset.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonReset.Location = new System.Drawing.Point(125, 358);
            this.buttonReset.Name = "buttonReset";
            this.buttonReset.Size = new System.Drawing.Size(97, 42);
            this.buttonReset.TabIndex = 19;
            this.buttonReset.Text = "Reset";
            this.buttonReset.UseVisualStyleBackColor = true;
            this.buttonReset.Click += new System.EventHandler(this.ButtonReset_Click);
            // 
            // rtbEeHook
            // 
            this.rtbEeHook.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.rtbEeHook.BackColor = System.Drawing.SystemColors.WindowText;
            this.rtbEeHook.ForeColor = System.Drawing.Color.Yellow;
            this.rtbEeHook.Location = new System.Drawing.Point(14, 228);
            this.rtbEeHook.Name = "rtbEeHook";
            this.rtbEeHook.Size = new System.Drawing.Size(208, 124);
            this.rtbEeHook.TabIndex = 20;
            this.rtbEeHook.Text = "";
            this.rtbEeHook.TextChanged += new System.EventHandler(this.RtbEeHook_TextChanged);
            // 
            // labelEeHook
            // 
            this.labelEeHook.AutoSize = true;
            this.labelEeHook.Font = new System.Drawing.Font("Microsoft Sans Serif", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.labelEeHook.Location = new System.Drawing.Point(12, 209);
            this.labelEeHook.Name = "labelEeHook";
            this.labelEeHook.Size = new System.Drawing.Size(59, 15);
            this.labelEeHook.TabIndex = 21;
            this.labelEeHook.Text = "ee-hook=";
            // 
            // buttonEditScript
            // 
            this.buttonEditScript.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonEditScript.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonEditScript.Location = new System.Drawing.Point(14, 409);
            this.buttonEditScript.Name = "buttonEditScript";
            this.buttonEditScript.Size = new System.Drawing.Size(208, 43);
            this.buttonEditScript.TabIndex = 22;
            this.buttonEditScript.Text = "Edit Script";
            this.buttonEditScript.UseVisualStyleBackColor = true;
            this.buttonEditScript.Click += new System.EventHandler(this.ButtonEditScript_Click);
            // 
            // EmulatorOptions
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.LightSkyBlue;
            this.ClientSize = new System.Drawing.Size(234, 464);
            this.Controls.Add(this.buttonEditScript);
            this.Controls.Add(this.labelEeHook);
            this.Controls.Add(this.rtbEeHook);
            this.Controls.Add(this.buttonReset);
            this.Controls.Add(this.buttonSave);
            this.Controls.Add(this.comboEeIgnore);
            this.Controls.Add(this.textBoxIdec);
            this.Controls.Add(this.textBoxGsVert);
            this.Controls.Add(this.checkAssertPath);
            this.Controls.Add(this.checkGsOverride);
            this.Controls.Add(this.checkVu1Di);
            this.Controls.Add(this.checkCop2);
            this.Controls.Add(this.checkFpu);
            this.Controls.Add(this.checkGsOpt);
            this.Controls.Add(this.labelEeIgnore);
            this.Controls.Add(this.labelAssertPath);
            this.Controls.Add(this.labelGsOverride);
            this.Controls.Add(this.labelVu1Di);
            this.Controls.Add(this.labelCop2);
            this.Controls.Add(this.labelFpu);
            this.Controls.Add(this.labelGsOpt);
            this.Controls.Add(this.labelIdec);
            this.Controls.Add(this.labelGsVert);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "EmulatorOptions";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Emu Options";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.EmulatorOptions_FormClosing);
            this.Load += new System.EventHandler(this.EmulatorOptions_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label labelGsVert;
        private System.Windows.Forms.Label labelIdec;
        private System.Windows.Forms.Label labelGsOpt;
        private System.Windows.Forms.Label labelFpu;
        private System.Windows.Forms.Label labelCop2;
        private System.Windows.Forms.Label labelVu1Di;
        private System.Windows.Forms.Label labelGsOverride;
        private System.Windows.Forms.Label labelAssertPath;
        private System.Windows.Forms.Label labelEeIgnore;
        private System.Windows.Forms.CheckBox checkGsOpt;
        private System.Windows.Forms.CheckBox checkFpu;
        private System.Windows.Forms.CheckBox checkCop2;
        private System.Windows.Forms.CheckBox checkVu1Di;
        private System.Windows.Forms.CheckBox checkGsOverride;
        private System.Windows.Forms.CheckBox checkAssertPath;
        private System.Windows.Forms.TextBox textBoxGsVert;
        private System.Windows.Forms.TextBox textBoxIdec;
        private System.Windows.Forms.ComboBox comboEeIgnore;
        private System.Windows.Forms.Button buttonSave;
        private System.Windows.Forms.Button buttonReset;
        private System.Windows.Forms.RichTextBox rtbEeHook;
        private System.Windows.Forms.Label labelEeHook;
        private System.Windows.Forms.Button buttonEditScript;
    }
}