using free_ps2_pub_gen.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace free_ps2_pub_gen {
    /// <summary>
    /// Fake Ps2 PKG Generator.
    /// </summary>
    public partial class Fakeps2pkggen : Form {
        private string lastIsoPath = string.Empty;
        private string lastOutPath = string.Empty;
        private string ps2Iso = string.Empty;
        private string pkgOut = string.Empty;
        private string make_fself = string.Empty;
        private string orbis_pub_cmd = string.Empty;
        private string db = string.Empty;
        private string newPass = string.Empty;
        private string tempPath = string.Empty;
        private string[][] Apps;
        private string[][] Auths;
        private bool _iso = false;
        private static bool error = false;
        private List<string> Fws;
        private StringComparison ignore = StringComparison.InvariantCultureIgnoreCase;

        /// <summary>
        /// Program Initializer.
        /// </summary>
        public Fakeps2pkggen() { InitializeComponent(); }

        /// <summary>
        /// Open a File Fialog to Choose the ISO.
        /// </summary>
        private void ChooseISO() {
            string iso = MessagBox.ShowOpenFile("Select PS2 ISO", "ISO Image (.*iso)|*.iso", lastIsoPath);
            if (iso != string.Empty) ps2Iso = lastIsoPath = iso;
        }

        /// <summary>
        /// Checks if defined ContentId is in valid form.
        /// </summary>
        /// <returns>True if Content Id is vlaid, else false.</returns>
        private bool HaseContentIDFormat(string contentID) {
            if (contentID.Length == 36) {
                if (contentID.XReplace(@"(\w{2})(\d{4})-(\w{4})(\d{5})_(\d{2})-([a-zA-Z0-9]{16})", "") == string.Empty) return true;
            }
            return false;
        }

        /// <summary>
        /// Get the string of a byte converted decimal value.
        /// </summary>
        /// <param name="titleId">The TitleID decimal to convert.</param>
        /// <returns>A string, representing the convertet decimal byte value.</returns>
        private string GetDecimalBytes(decimal titleId) {
            byte[] titleIdBytes = titleId.GetBytes();
            return BitConverter.ToString(titleIdBytes).Substring(0, 5).Replace("-", "");
        }

        /// <summary>
        /// Check the Base to work with.
        /// </summary>
        /// <returns>True if all needed Application Paths and Base files are available.</returns>
        private bool BaseIsFine() {
            if (!File.Exists(make_fself)) return false;
            else if (!File.Exists(orbis_pub_cmd)) return false;
            else if (!File.Exists(db)) return false;
            else if (!Directory.Exists(tempPath)) return false;
            else if (!File.Exists(tempPath + @"\template.gp4")) return false;
            else if (!Directory.Exists(tempPath + @"\workfiles")) return false;
            else if (!File.Exists(tempPath + @"\workfiles\eboot.bin")) return false;
            else if (!File.Exists(tempPath + @"\workfiles\ps2-emu-compiler.self")) return false;
            else if (!File.Exists(tempPath + @"\workfiles\ps2-emu-libSceFios2.prx")) return false;
            else if (!File.Exists(tempPath + @"\workfiles\ps2-emu-libc.prx")) return false;
            else if (!File.Exists(tempPath + @"\workfiles\config-emu-ps4.txt")) return false;
            return true;
        }

        /// <summary>
        /// Pre Clean the template_pkg folder before reusing it.
        /// </summary>
        private void PreClean() {
            if (File.Exists(tempPath + @"\template_pkg\Image0\image\disc01.iso")) File.Delete(tempPath + @"\template_pkg\Image0\image\disc01.iso");
            if (File.Exists(tempPath + @"\template_pkg\Image0\eboot.fself")) File.Delete(tempPath + @"\template_pkg\Image0\eboot.fself");
            if (File.Exists(tempPath + @"\template_pkg\Image0\ps2-emu-compiler.fself")) File.Delete(tempPath + @"\template_pkg\Image0\ps2-emu-compiler.fself");
            if (File.Exists(tempPath + @"\template_pkg\Image0\sce_module\libSceFios2.fself")) File.Delete(tempPath + @"\template_pkg\Image0\sce_module\libSceFios2.fself");
            if (File.Exists(tempPath + @"\template_pkg\Image0\sce_module\libc.fself")) File.Delete(tempPath + @"\template_pkg\Image0\sce_module\libc.fself");
            if (File.Exists(tempPath + @"\template_pkg\Image0\config-emu-ps4.txt")) File.Delete(tempPath + @"\template_pkg\Image0\config-emu-ps4.txt");
        }

        /// <summary>
        /// Check the projects gp4 and add entrys if needed.
        /// </summary>
        private string CheckGp4() {
            string gp4 = pkgOut + @"\" + textContentID.Text + ".gp4";
            File.Copy(tempPath + @"\template.gp4", gp4);
            string[] lines = File.ReadAllLines(gp4);

            foreach (string line in lines) {
                if (line.Contains("<package content_id=")) line.Replace("EP1004-CUSA04488_00-SLES503260000001", textContentID.Text);
                if (line.Contains("<package content_id=passcode=")) {
                    if (newPass != "00000000000000000000000000000000") line.Replace("00000000000000000000000000000000", newPass);
                } else if (line.Contains("sce_sys/nptitle.dat")) {
                    if (nptitlenpbindToolStrip.Checked) line.Replace("<!--", "").Replace("-->", "");
                } else if (line.Contains("sce_sys/npbind.dat")) {
                    if (nptitlenpbindToolStrip.Checked) line.Replace("<!--", "").Replace("-->", "");
                } else if (line.Contains("sce_sys/trophy/trophy00.trp")) {
                    if (trophydataToolStrip.Checked) line.Replace("<!--", "").Replace("-->", "");
                }
            }

            File.Delete(gp4);
            File.Create(gp4).Close();
            File.WriteAllLines(gp4, lines);
            return gp4;
        }

        /// <summary>
        /// Check the emus config and add entrys if needed.
        /// </summary>
        private void CheckConfig() {
            File.Copy(tempPath + @"\workfiles\config-emu-ps4.txt", tempPath + @"\template_pkg\Image0\config-emu-ps4.txt");
            string[] lines = File.ReadAllLines(tempPath + @"\template_pkg\Image0\config-emu-ps4.txt");

            foreach (string line in lines) {
                if (line.Contain("--path-patches='/app0/patches'") && !patchesToolStrip.Checked) line.Replace("--path-patches='/app0/patches'", "");
                if (line.Contain("--path-trophydata='/app0/trophy_data'") && !trophydataToolStrip.Checked) line.Replace("--path-trophydata='/app0/trophy_data'", "");
                if (line.Contain("--path-featuredata='/app0/feature_data'") && !featuredataToolStrip.Checked) line.Replace("--path-featuredata='/app0/feature_data'", "");
                if (line.Contain("--path-toolingscript='/app0/patches'") && !toolingscriptToolStrip.Checked) line.Replace("--path-toolingscript='/app0/patches'", "");
                if (line.Contain("--path-featuredata='/app0/patches'") && !featuredatapatchToolStrip.Checked) line.Replace("--path-featuredata='/app0/patches'", "");
            }
            FileEx.RemoveEmptyLines(tempPath + @"\template_pkg\Image0\config-emu-ps4.txt");            
        }

        /// <summary>
        /// Fake Sign and spoof authentication informations.
        /// </summary>
        /// <param name="deci">The TitleIDs decimal value as a hex string.</param>
        private bool FakeSign(string deci) {
            string workPath = tempPath + @"\workfiles\";
            int index = toolStripComboBox.SelectedIndex;
            string elfs = string.Empty;
            string selfs = string.Empty;
            int count = 0;
            bool err = false;
            
            ProcessStartInfo run = new ProcessStartInfo();
            Process call = new Process();
            call.ErrorDataReceived += Ps2_ErrorHandler;
            run.FileName = make_fself;
            run.UseShellExecute = false;
            run.CreateNoWindow = run.RedirectStandardError = true;            

            foreach (string elf in Apps[index]) {
                string auth = Auths[index][count];
                auth.Replace("XXXX", deci);
                if (!elf.Contains("ps2-emu-compiler.self")) elfs = tempPath + @"\workfiles\" + elf.Replace("ps2-emu-", "");
                else elfs = tempPath + @"\workfiles\" + elf;
                selfs = tempPath + @"\template_pkg\Image0\" + elf.Replace(".bin", ".fself").Replace(".self", ".fself").Replace(".prx", ".fself");

                run.Arguments = "--paid " + auth.Substring(0, 16).EndianSwapp() + " --auth-info " + auth + " " + elfs + " " + selfs;
                call.StartInfo = run;

                try { call.Start(); }
                catch (Exception io) { MessagBox.Error(io.ToString()); err = true; break; }

                call.BeginErrorReadLine();
                call.WaitForExit();
                count++;
            }

            if (err) return false;

            idl.Text = "Coping ELFs";
            foreach (string file in Directory.GetFiles(tempPath + @"\workfiles\", "*.fself")) {
                if (file.Contains("libc") || file.Contains("libSceFios2")) File.Move(file, file.Replace(@"workfiles\", @"template_pkg\Image0\sce_module\"));
                File.Move(file, file.Replace(@"workfiles\", @"template_pkg\Image0\"));
            }
            return true;
        }

        /// <summary>
        /// Build the PKG now.
        /// </summary>
        private void MakePKG(string gp4) {
            ProcessStartInfo run = new ProcessStartInfo();
            Process call = new Process();
            call.ErrorDataReceived += Ps2_ErrorHandler;
            run.FileName = orbis_pub_cmd;
            run.UseShellExecute = false;
            run.CreateNoWindow = run.RedirectStandardError = true;
            run.Arguments = "img_create " + gp4 + " " + gp4.Replace(".gp4", ".pkg");
            call.StartInfo = run;

            try { call.Start(); }
            catch (Exception io) { MessagBox.Error(io.ToString()); }

            call.BeginErrorReadLine();
            call.WaitForExit();
        }

        /// <summary>
        /// On Load of Form do.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void Fakeps2pkggen_Load(object sender, EventArgs e) {
            // Get settings.
            Settings sett = new Settings();
            lastIsoPath = sett.LastIsoPath;
            lastOutPath = sett.LastOutPath;
            make_fself = sett.MakeFSELF;
            orbis_pub_cmd = sett.PubCmd;
            db = sett.DB;
            clearISOOnCloseToolStrip.Checked = sett.ClearIso;
            _iso = true;

            // Just in case.
            string currDir = Directory.GetCurrentDirectory();
            if (make_fself == string.Empty) make_fself = currDir + @"\make_fself.py";
            if (orbis_pub_cmd == string.Empty) orbis_pub_cmd = currDir + @"\orbis-pub-cmd-ps2.exe";
            if (db == string.Empty) db = currDir + @"\template\authinfo_emu.txt";
            if (!File.Exists(make_fself)) MessagBox.Error("Can not find make_fself.py.\nEither drop the file into me, so i know where it is or\npleace it into my directory so i can acces it.");
            if (!File.Exists(orbis_pub_cmd)) MessagBox.Error("Can not find orbis-pub-cmd-ps2.exe.\nEither drop the file into me, so i know where it is or\npleace it into my directory so i can acces it.");
            if (!Directory.Exists(currDir + @"\template\template_pkg\")) MessagBox.Error("Can not find the Tempalte folder !\nPlease place the Template folder within same dir.");
            else tempPath = currDir + @"\template";

            // Load Authentication DataBase.
            if (!File.Exists(db)) MessagBox.Error("Can not find authinfo.txt.\nEither drop the file into me, so i know where it is or\npleace it into my directory so i can acces it.");
            else {
                Fws = new List<string>();
                List<string[]> _Apps = new List<string[]>();
                List<string[]> _Auths = new List<string[]>();
                List<string> _apps = new List<string>();
                List<string> _auths = new List<string>();
                bool app, auth, fw;
                app = auth = fw = false;
                foreach (string line in File.ReadAllLines(db)) {
                    if (!string.IsNullOrEmpty(line)) {
                        if (line.Contains("[FW=", ignore)) {
                            if (Fws.Count > 0 && app) { MessagBox.Error("DataBase Inconsistent !"); break; }

                            string[] tag = line.Split(']');
                            Fws.Add(tag[0].XReplace(@"[[]FW=?", string.Empty, RegexOptions.IgnoreCase));
                            fw = true;
                            if (Fws.Count > 1) {
                                _Apps.Add(_apps.ToArray());
                                _Auths.Add(_auths.ToArray());
                                _apps = new List<string>();
                                _auths = new List<string>();
                            }

                        } else if (line.Contains("[Name=", ignore)) {
                            if (!fw) {
                                if (!auth) { MessagBox.Error("DataBase Inconsistent !"); break; }
                            }

                            string[] tag = line.Split(']');
                            _apps.Add(tag[0].XReplace(@"[[]Name=?\s?", string.Empty, RegexOptions.IgnoreCase));
                            auth = fw = false;
                            app = true;
                        } else if (line.Contains("[Auth=", ignore)) {
                            if (!app) {
                                if (fw) { MessagBox.Error("DataBase Inconsistent !"); break; }
                            }

                            string[] tag = line.Split(']');
                            _auths.Add(tag[0].XReplace(@"[[]Auth=?", string.Empty, RegexOptions.IgnoreCase));
                            app = false;
                            auth = true;
                        }
                    }
                }

                _Apps.Add(_apps.ToArray());
                _Auths.Add(_auths.ToArray());

                if (Fws.Count != _Apps.Count || Fws.Count != _Auths.Count) {
                    MessagBox.Error("DataBase Inconsistent !");
                    Fws.Clear();
                } else {
                    if (Fws.Count > 0) {
                        // If any fws where found add them to the combo Box.
                        toolStripComboBox.Items.AddRange(Fws.ToArray());
                        Apps = _Apps.ToArray();
                        Auths = _Auths.ToArray();
                    }
                }
            }

            // Set basic passcode.
            newPass = "00000000000000000000000000000000";
            idl.Text = "Ready For Action !";
        }

        /// <summary>
        /// Error Event Handler for the make_fself.py and orbis-pub-cmd-ps2.exe Process.
        /// </summary>
        /// <param name="sendingProcess">The Process which triggered this Event.</param>
        /// <param name="outLine">The Received Data Event Arguments.</param>
        private static void Ps2_ErrorHandler(object sendingProcess, DataReceivedEventArgs outLine) { MessagBox.Error(outLine.Data); error = true; }

        /// <summary>
        /// On Drag Enter.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void Fakeps2pkggen_DragEnter(object sender, DragEventArgs e) {
            if (e.Data.GetDataPresent(DataFormats.FileDrop, false)) e.Effect = DragDropEffects.Copy;
            else e.Effect = DragDropEffects.None;
        }

        /// <summary>
        /// On Drag Drop.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void Fakeps2pkggen_DragDrop(object sender, DragEventArgs e) {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop, false);

            if (files.Length > 1) MessagBox.Error("No Multi File support here.");
            else {
                if (files[0].Contains(".iso", ignore)) {
                    ps2Iso = lastIsoPath = files[0];
                    if (_iso) textIsoAndOutPath.Text = files[0];
                } else if (files[0].Contains("make_fself.py", ignore)) make_fself = files[0];
                else if (files[0].Contains("orbis-pub-cmd-ps2.exe", ignore)) orbis_pub_cmd = files[0];
                else if (files[0].Contains("authinfo_emu.txt", ignore)) db = files[0];
                else if (files[0].IsFolder()) {
                    pkgOut = lastOutPath = files[0];
                    if (!_iso) textIsoAndOutPath.Text = files[0];
                } else MessagBox.Error("I have no clue what you want from me mate.");
            }
        }

        /// <summary>
        /// On Open Iso Tool Strip click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void OpenISOToolStrip_Click(object sender, EventArgs e) { ChooseISO(); }

        /// <summary>
        /// On Textbox Iso and Out Path Double Click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void TextIsoAndOutPath_DoubleClick(object sender, EventArgs e) {
            if (_iso) {
                _iso = false;
                if (ps2Iso != string.Empty) textIsoAndOutPath.Text = ps2Iso;
                buttonOpen.Image = Resources.Disk.ToBitmap().Resize(16, 16);
            } else {
                _iso = true;
                if (pkgOut != string.Empty) textIsoAndOutPath.Text = pkgOut;
                buttonOpen.Image = Resources.Folder_Open.ToBitmap().Resize(16, 16);
            }
        }

        /// <summary>
        /// On Close Tool Strip click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void CloseToolStrip_Click(object sender, EventArgs e) {
            Settings sett = new Settings();
            sett.LastIsoPath = lastIsoPath;
            sett.LastOutPath = lastOutPath;
            sett.MakeFSELF = make_fself;
            sett.PubCmd = orbis_pub_cmd;
            sett.DB = db;
            sett.ClearIso = clearISOOnCloseToolStrip.Checked;
            sett.Save();
            Close();
        }

        /// <summary>
        /// On Button Open Click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void ButtonOpen_Click(object sender, EventArgs e) {
            if (!_iso) ChooseISO();
            else {
                string outPath = MessagBox.ShowFolderBrowse("Choose Folder to save PKG", lastOutPath);
                if (outPath != string.Empty) pkgOut = lastOutPath = outPath;
            }
        }

        /// <summary>
        /// On Tool Strip Change Passcode click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void ChangePasscodeToolStrip_Click(object sender, EventArgs e) {
            MessagBox.Size = new Size(270, 146);
            MessagBox.TextBack = Color.Black;
            MessagBox.TextFore = Color.Yellow;
            if (MessagBox.Input(newPass, "Enter new:", "Passcode", 66) == DialogResult.OK) {
                newPass = MessagBox.UsrInput;
                if (newPass.Length < 66) {
                    newPass = "00000000000000000000000000000000";
                    MessagBox.Error("The Entered new Passcode is to short.\nTry Again.");
                }
            }
            MessagBox.TextBack = Color.White;
            MessagBox.TextFore = Color.Black;
            MessagBox.Size = new Size(0, 0);
        }

        /// <summary>
        /// On Tool Strip make_fself.py click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void MakefselfpyToolStrip_Click(object sender, EventArgs e) { MessagBox.Info(make_fself); }

        /// <summary>
        /// On Tool Strip orbis-pub-cmd-exe click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void OrbispubcmdexeToolStrip_Click(object sender, EventArgs e) { MessagBox.Info(orbis_pub_cmd); }

        /// <summary>
        /// On Tool Strip Authentication Information click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void AuthinfotxtToolStrip_Click(object sender, EventArgs e) { MessagBox.Info(db); }

        /// <summary>
        /// On Tool Strip Patches click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void PatchesToolStrip_Click(object sender, EventArgs e) {
            if (patchesToolStrip.Checked) patchesToolStrip.Checked = true;
            else patchesToolStrip.Checked = true;
        }

        /// <summary>
        /// On Tool Strip Trophydata click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void TrophydataToolStrip_Click(object sender, EventArgs e) {
            if (trophydataToolStrip.Checked) trophydataToolStrip.Checked = true;
            else trophydataToolStrip.Checked = true;
        }

        /// <summary>
        /// On Tool Strip FeaturedData click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void FeaturedataToolStrip_Click(object sender, EventArgs e) {
            if (featuredataToolStrip.Checked) featuredataToolStrip.Checked = true;
            else featuredataToolStrip.Checked = true;
        }

        /// <summary>
        /// On Tool Strip FeaturedData/patch click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void FeaturedatapatchToolStrip_Click(object sender, EventArgs e) {
            if (featuredatapatchToolStrip.Checked) featuredatapatchToolStrip.Checked = true;
            else featuredatapatchToolStrip.Checked = true;
        }

        /// <summary>
        /// On Tool Strip Toolingscript click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void ToolingscriptToolStrip_Click(object sender, EventArgs e) {
            if (toolingscriptToolStrip.Checked) toolingscriptToolStrip.Checked = true;
            else toolingscriptToolStrip.Checked = true;
        }

        /// <summary>
        /// On Tool Strip nptitle/npbind click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void NptitlenpbindToolStrip_Click(object sender, EventArgs e) {
            if (nptitlenpbindToolStrip.Checked) nptitlenpbindToolStrip.Checked = true;
            else nptitlenpbindToolStrip.Checked = true;
        }

        /// <summary>
        /// On Tool Strip About click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void AboutToolStripA_Click(object sender, EventArgs e) { MessagBox.Info("PS2 Fake PKG Generator for PS4\nby cfwprpht\n\nPS4- PS2 Fake Emu PKG Technic and orbis-pub-cmd patch\nby flatz"); }

        /// <summary>
        /// On Tool Strip Build click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void ToolStripBuild_Click(object sender, EventArgs e) {
            if (toolStripComboBox.SelectedIndex > -1) {
                if (textContentID.Text != string.Empty) {
                    if (HaseContentIDFormat(textContentID.Text)) {
                        if (ps2Iso != string.Empty) {
                            if (pkgOut != string.Empty) {
                                if (BaseIsFine()) {
                                    // Copy ISO and check if destination dir is empty.
                                    idl.Text = "Cleaning old files up";
                                    PreClean();
                                    idl.Text = "Coping ISO";
                                    File.Copy(ps2Iso, tempPath + @"\template_pkg\Image0\image\disc01.iso");

                                    // Get decimal of ItelID and convert to byte string, then overload to hte fake signing routine.
                                    idl.Text = "Fake Signing ELFs";
                                    if (!FakeSign(GetDecimalBytes(Convert.ToDecimal(textContentID.Text.Substring(11, 5))))) { idl.Text = "Error Fake Signing ELFs"; return; }

                                    // Copy and check the gp4 file and change all needed stuff.
                                    string gp4 = CheckGp4();

                                    // Do the same with the config file.
                                    CheckConfig();

                                    // Call orbis pub cmd.
                                    idl.Text = "Generating PKG";
                                    MakePKG(gp4);
                                    if (!error) MessagBox.Show("Done !");
                                }
                            }
                            MessagBox.Error("No Output Folder set !");
                        } MessagBox.Error("No PS2 ISO selected !");
                    } MessagBox.Error("Content ID is not on Form of: XXYYYY-XXXXYYYYY_YY-XXXXXXXXXXXXXXXX");
                } MessagBox.Error("Content ID is empty !");
            } MessagBox.Error("No FW Selected !");
        }
    }
}
