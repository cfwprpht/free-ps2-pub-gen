using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace free_ps2_pub_gen {
    /// <summary>
    /// Emulator Options.
    /// </summary>
    public partial class EmulatorOptions : Form {
        private bool gsopt, fpu, cop2, vu1, gsoverr, assert, saved, userChange, init;
        private string gsvert, idec, eeignore, configFile, scriptFile;
        private List<string> eehook;
        
        /// <summary>
        /// Main Entry of the Form.
        /// </summary>
        public EmulatorOptions() { InitializeComponent(); }

        /// <summary>
        /// On Load of Form.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void EmulatorOptions_Load(object sender, EventArgs e) {
            init = false;
            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\template\app0\patches\")) {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\template\app0\patches\", "*_cli.conf");
                if (files.Length == 1) {
                    configFile = files[0];
                    if (File.Exists(configFile)) {
                        eehook = new List<string>();
                        comboEeIgnore.Items.Add("Read");
                        comboEeIgnore.Items.Add("Write");
                        gsopt = fpu = cop2 = vu1 = gsoverr = assert = saved = userChange = false;
                        gsvert = idec = eeignore = string.Empty;

                        string[] config = File.ReadAllLines(configFile);
                        foreach (string line in config) {
                            if (line.Contain("--ee-hook=")) rtbEeHook.Text += line.Replace("--ee-hook=", "") + Environment.NewLine;
                            else if (line.Contain("--gs-vert-precision=")) textBoxGsVert.Text = line.Replace("--gs-vert-precision=", "");
                            else if (line.Contain("--idec-cycles-per-qwc=")) textBoxIdec.Text = line.Replace("--idec-cycles-per-qwc=", "");
                            else if (line.Contain("--gs-optimize-30fps=")) {
                                if (line.Replace("--gs-optimize-30fps=", "").Equals("1")) checkGsOpt.Checked = true;
                                else checkGsOpt.Checked = false;
                            } else if (line.Contain("--fpu-no-clamping=")) {
                                if (line.Replace("--fpu-no-clamping=", "").Equals("1")) checkFpu.Checked = true;
                                else checkFpu.Checked = false;
                            } else if (line.Contain("--cop2-no-clamping=")) {
                                if (line.Replace("--cop2-no-clamping=", "").Equals("1")) checkCop2.Checked = true;
                                else checkCop2.Checked = false;
                            } else if (line.Contain("--vu1-di-bits=")) {
                                if (line.Replace("--vu1-di-bits=", "").Equals("1")) checkVu1Di.Checked = true;
                                else checkVu1Di.Checked = false;
                            } else if (line.Contain("--gs-override-small-tri-area=")) {
                                if (line.Replace("--gs-override-small-tri-area=", "").Equals("1")) checkGsOverride.Checked = true;
                                else checkGsOverride.Checked = false;
                            } else if (line.Contain("--assert-path1-ad=")) {
                                if (line.Replace("--assert-path1-ad=", "").Equals("1")) checkAssertPath.Checked = true;
                                else checkAssertPath.Checked = false;
                            } else if (line.Contain("--ee-ignore-segfault=")) {
                                if (line.Replace("--ee-ignore-segfault=", "").Equals("Read")) comboEeIgnore.SelectedText = "Read";
                                else comboEeIgnore.SelectedText = "Write";
                            }
                        }
                        eehook.AddRange(rtbEeHook.Lines);
                        eeignore = comboEeIgnore.SelectedText;
                        gsvert = textBoxGsVert.Text;
                        idec = textBoxIdec.Text;
                        gsopt = checkGsOpt.Checked;
                        fpu = checkFpu.Checked;
                        cop2 = checkCop2.Checked;
                        vu1 = checkVu1Di.Checked;
                        gsoverr = checkGsOverride.Checked;
                        assert = checkAssertPath.Checked;
                    } else {
                        MessagBox.Error("Cant access the config file !\nWill return.");
                        DialogResult = DialogResult.Abort;
                    }
                } else {
                    MessagBox.Error("I found more then one TitleID config file within the patches folder !\nDon't know which to use.");
                    DialogResult = DialogResult.Abort;
                }
            } else {
                MessagBox.Error("Can't access " + Directory.GetCurrentDirectory() + @"\template\app0\patches\" + " \nMake sure the path exists.");
                DialogResult = DialogResult.Abort;
            }
            init = true;
        }

        /// <summary>
        /// On Form Closing.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void EmulatorOptions_FormClosing(object sender, FormClosingEventArgs e) {
            if (userChange && !saved) {
                if (eehook.Count != rtbEeHook.Lines.Length) goto changed;

                int maxlen = 0;
                if (eehook.Count > rtbEeHook.Lines.Length) maxlen = rtbEeHook.Lines.Length;
                else maxlen = eehook.Count;

                string[] backup = eehook.ToArray();
                string[] actual = rtbEeHook.Lines;
                for (int i = 0; i < maxlen; i++) { if (!backup[i].Equals(actual[i])) goto changed; }

                if (comboEeIgnore.SelectedText != eeignore) goto changed;
                else if (textBoxGsVert.Text != gsvert) goto changed;
                else if (textBoxIdec.Text != idec) goto changed;
                else if (checkGsOpt.Checked != gsopt) goto changed;
                else if (checkFpu.Checked != fpu) goto changed;
                else if (checkCop2.Checked != cop2) goto changed;
                else if (checkVu1Di.Checked != vu1) goto changed;
                else if (checkGsOverride.Checked != gsoverr) goto changed;
                else if (checkAssertPath.Checked != assert) goto changed;
                else return;

                changed:
                    if (MessagBox.Question(Buttons.YesNo, "You changed some options but didn't save.\nAre you sure to Close the App now ?") == DialogResult.No) e.Cancel = true;
            }
        }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void CheckFpu_CheckedChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void CheckCop2_CheckedChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void CheckVu1Di_CheckedChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void CheckGsOverride_CheckedChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void CheckAssertPath_CheckedChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void ComboEeIgnore_SelectedIndexChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void RtbEeHook_TextChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void CheckGsOpt_CheckedChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void TextBoxIdec_TextChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On User Change.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void TextBoxGsVert_TextChanged(object sender, EventArgs e) { if (init) userChange = true; }

        /// <summary>
        /// On Button Save click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void ButtonSave_Click(object sender, EventArgs e) {
            if (comboEeIgnore.SelectedIndex == -1) { MessagBox.Error("No 'Ignore Segfault' mode choosen !"); return; }
            List<string> newConf = new List<string>();

            if (rtbEeHook.Lines.Length > 0) {
                foreach (string line in rtbEeHook.Lines) newConf.Add("--ee-hook=" + line);
            }
            newConf.Add("--gs-vert-precision=" + textBoxGsVert.Text);
            newConf.Add("--idec-cycles-per-qwc==" + textBoxIdec.Text);
            if (checkGsOpt.Checked) newConf.Add("--gs-optimize-30fps=1");
            else newConf.Add("--gs-optimize-30fps=0");
            if (checkFpu.Checked) newConf.Add("--fpu-no-clamping=1");
            else newConf.Add("--fpu-no-clamping=0");
            if (checkCop2.Checked) newConf.Add("--cop2-no-clamping=1");
            else newConf.Add("--cop2-no-clamping=0");
            if (checkVu1Di.Checked) newConf.Add("--vu1-di-bits=1");
            else newConf.Add("--vu1-di-bits=0");
            if (checkGsOverride.Checked) newConf.Add("--gs-override-small-tri-area=1");
            else newConf.Add("--gs-override-small-tri-area=0");
            if (checkAssertPath.Checked) newConf.Add("--assert-path1-ad=1");
            else newConf.Add("--assert-path1-ad=0");
            newConf.Add(comboEeIgnore.SelectedText);

            try {
                File.Delete(configFile);
                File.Create(configFile).Close();
                File.WriteAllLines(configFile, newConf.ToArray());

            } catch (IOException io) { MessagBox.Error("IO Exception !\n\n" + io); }
            finally { saved = true; }
        }

        /// <summary>
        /// On Button Reset click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void ButtonReset_Click(object sender, EventArgs e) {
            rtbEeHook.Lines = eehook.ToArray();
            comboEeIgnore.SelectedText = eeignore;
            textBoxGsVert.Text = gsvert;
            textBoxIdec.Text = idec;
            checkGsOpt.Checked = gsopt;
            checkFpu.Checked = fpu;
            checkCop2.Checked = cop2;
            checkVu1Di.Checked = vu1;
            checkGsOverride.Checked = gsoverr;
            checkAssertPath.Checked = assert;
        }

        /// <summary>
        /// On Button Edit Script click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void ButtonEditScript_Click(object sender, EventArgs e) {
            if (Fakeps2pkggen.textViewer == string.Empty) {
                MessagBox.Error("No default text viewer set.\nPlease tell me which app you want to use to edit your script file.\nOptions > Paths > Default Text/Code Viewer");
                return;
            }

            if (Directory.Exists(Directory.GetCurrentDirectory() + @"\template\app0\patches\")) {
                string[] files = Directory.GetFiles(Directory.GetCurrentDirectory() + @"\template\app0\patches\", "*_config.lua");
                if (files.Length == 1) {
                    scriptFile = files[0];
                    if (File.Exists(scriptFile)) {
                        Process editor = new Process();
                        ProcessStartInfo run = new ProcessStartInfo();
                        run.FileName = Fakeps2pkggen.textViewer;
                        run.Arguments = scriptFile;
                        editor.StartInfo = run;
                        editor.Start();
                        editor.WaitForExit();
                    } else MessagBox.Error("Cant access the script file !\nWill return.");
                } else MessagBox.Error("I found more then one TitleID script file within the patches folder !\nDon't know which to use.");
            } else MessagBox.Error("Someting went wrong !\nCan't access " + Directory.GetCurrentDirectory() + @"\template\app0\patches\" + " \nMake sure the path exists.");
        }
    }
}
