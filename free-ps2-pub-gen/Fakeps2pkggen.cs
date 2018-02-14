using free_ps2_pub_gen.Properties;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace free_ps2_pub_gen {
    /// <summary>
    /// Fake Ps2 PKG Generator.
    /// </summary>
    public partial class Fakeps2pkggen : Form {
        #region Vars
        /// <summary>
        /// Defines the default text viewer to use for editing the script file.
        /// </summary>
        public static string textViewer = string.Empty;

        /// <summary>
        /// Some Privates.
        /// </summary>
        private string lastIsoPath = string.Empty;
        private string lastOutPath = string.Empty;
        private string ps2Iso = string.Empty;
        private string pkgOut = string.Empty;
        private string make_fself = string.Empty;
        private string orbis_pub_cmd = string.Empty;
        private string db = string.Empty;
        private string newPass = string.Empty;
        private string tempPath = string.Empty;
        private string defaultBrowser = string.Empty;
        private string[][] Apps;
        private string[][] Auths;
        private string[] elfs;
        private string titleID = string.Empty;
        private bool _iso = false;
        private static bool error = false;
        private StringComparison ignore = StringComparison.InvariantCultureIgnoreCase;
        private List<string> Fws;
        #endregion Vars

        #region Functions
        /// <summary>
        /// Program Initializer.
        /// </summary>
        public Fakeps2pkggen() { InitializeComponent(); }

        /// <summary>
        /// Open a File Fialog to Choose the ISO.
        /// </summary>
        private void ChooseISO() {
            string iso = MessagBox.ShowOpenFile("Select PS2 ISO", "ISO Image (*.iso)|*.iso", lastIsoPath);
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
        private string GetDecimalBytes(string titleId) {
            byte[] titleIdBytes = Convert.ToDecimal(titleId).GetBytes();
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
            else if (!File.Exists(tempPath + @"\app0\eboot.elf")) return false;
            else if (!File.Exists(tempPath + @"\app0\ps2-emu-compiler.elf")) return false;
            else if (!File.Exists(tempPath + @"\app0\sce_module\libSceFios2.prx")) return false;
            else if (!File.Exists(tempPath + @"\app0\sce_module\libc.prx")) return false;
            else if (!File.Exists(tempPath + @"\app0\config-emu-ps4.txt")) return false;
            else if (!IsElfDecrypted()) return false;
            return true;
        }

        /// <summary>
        /// Pre Clean the template_pkg folder before reusing it.
        /// </summary>
        private void PreClean() {
            if (File.Exists(tempPath + @"\app0\image\disc01.iso")) File.Delete(tempPath + @"\app0\image\disc01.iso");
            if (File.Exists(tempPath + @"\app0\eboot.fself")) File.Delete(tempPath + @"\app0\eboot.fself");
            if (File.Exists(tempPath + @"\app0\ps2-emu-compiler.fself")) File.Delete(tempPath + @"\app0\ps2-emu-compiler.fself");
            if (File.Exists(tempPath + @"\app0\sce_module\libSceFios2.fself")) File.Delete(tempPath + @"\app0\sce_module\libSceFios2.fself");
            if (File.Exists(tempPath + @"\app0\sce_module\libc.fself")) File.Delete(tempPath + @"\app0\sce_module\libc.fself");

            string[] projects = Directory.GetFiles(tempPath, "*.gp4");
            if (projects.Length == 2) {
                if (projects[0].GetName().XReplace(@"(\w{2})(\d{4})-(\w{4})(\d{5})_(\d{2})-([a-zA-Z0-9]{16})", "").Replace(".gp4", "") == string.Empty) File.Delete(projects[0]);
                else if (projects[1].GetName().XReplace(@"(\w{2})(\d{4})-(\w{4})(\d{5})_(\d{2})-([a-zA-Z0-9]{16})", "").Replace(".gp4", "") == string.Empty) File.Delete(projects[1]);
            } else if (projects.Length > 2) {
                if (MessagBox.Question("Found more then two project files ! (.gp4)\nShall i delete all of them ? (excluding the template for sure)") == DialogResult.OK) {
                    foreach (string line in projects) { if (!line.Contain("template.gp4") && line.Contain(".gp4")) File.Delete(line); }
                }
            }
        }

        /// <summary>
        /// Check the projects gp4 and add entrys if needed.
        /// </summary>
        private string PatchGp4() {
            bool check = false;
            bool ps2TitlePatch = false;            
            string[] lines = File.ReadAllLines(tempPath + @"\template.gp4");

            foreach (string line in lines) {
                if (line.Contain("<package content_id=")) line.XReplace(@"(\w{2})(\d{4})-(\w{4})(\d{5})_(\d{2})-([a-zA-Z0-9]{16})", textContentID.Text);
                if (line.Contain("<package content_id=passcode=")) {
                    if (newPass != "00000000000000000000000000000000") line.Replace("00000000000000000000000000000000", newPass);
                } else if (line.Contain("sce_sys/nptitle.dat")) {
                    if (nptitlenpbindToolStrip.Checked) {
                        line.Replace("<!--", "").Replace(" -->", "");
                        PatchNpDat();
                    } else check = true;
                } else if (line.Contain("sce_sys/npbind.dat")) {
                    if (nptitlenpbindToolStrip.Checked) {
                        line.Replace("<!--", "").Replace(" -->", "");
                    } else check = true;
                } else if (line.Contain("sce_sys/trophy/trophy00.trp")) {
                    if (trophydataToolStrip.Checked) line.Replace("<!--", "").Replace(" -->", "");
                    else check = true;
                } else if (line.Contain("eboot.bin") && !line.Contain("eboot.fself")) line.Replace(@"app0\", "XX").XReplace(@"XXeboot.(\w+)", @"app0\eboot.fself");
                else if (line.Contain("ps2-emu-compiler.self") && !line.Contain("ps2-emu-compiler.fself")) line.Replace(@"app0\", "XX").XReplace(@"XXps2-emu-compiler.(\w+)", @"app0\ps2-emu-compiler.fself");
                else if (line.Contain("libSceFios2.prx") && !line.Contain("libSceFios2.fself")) line.Replace(@"app0\sce_module\", "XX").XReplace(@"XXlibSceFios2.(\w+)", @"app0\sce_module\libSceFios2.fself");
                else if (line.Contain("libc.prx") && !line.Contain("libc.fself")) line.Replace(@"app0\sce_module\", "XX").XReplace(@"XXlibc.(\w+)", @"app0\sce_module\libc.fself");
                else if (line.Contain("_cli.conf")) {
                    if (patchesCliConfToolStrip.Checked) {
                        ps2TitlePatch = true;
                        string[] getFile = Directory.GetFiles(tempPath + @"\app0\patches\", "*_cli.conf");
                        if (getFile.Length == 0) return "No single titleID_cli.conf found but you defined to patch this file back into the project !";
                        else if (getFile.Length == 1) File.Move(getFile[0], getFile[0].XReplace(@"(\w{4})-(\d{5})", titleID.Insert(4, "-")));
                        else if (getFile.Length > 1) return "To many titleID_cli.conf found !\nPlease clean " + tempPath + @"\app0\patches\";

                    } else check = true;
                } else if (line.Contain("_config.lua")) {
                    if (patchesConfLuaToolStrip.Checked) {
                        ps2TitlePatch = true;
                        string[] getFile = Directory.GetFiles(tempPath + @"\app0\patches\", "*_config.lua");
                        if (getFile.Length == 0) return "No single titleID__config.lua found but you defined to patch this file back into the project !";
                        else if (getFile.Length == 1) File.Move(getFile[0], getFile[0].XReplace(@"(\w{4})-(\d{5})", titleID.Insert(4, "-")));
                        else if (getFile.Length > 1) return "To many titleID__config.lua found !\nPlease clean " + tempPath + @"\app0\patches\";

                    } else check = true;
                } else if (line.Contain("feature_data")) {
                    if (featuredataToolStrip.Checked) ps2TitlePatch = true;
                    else check = true;
                }

                if (ps2TitlePatch) {
                    line.Replace("<!--", "").Replace(" -->", "");
                    line.XReplace(@"(\w{4})-(\d{5})", titleID);
                    ps2TitlePatch = false;
                }
                if (check) {
                    if (!line.Contain("<!--<file")) line.Replace("<file", "<!--<file");
                    if (!line.Contain("/> -->")) line.Replace("/>", "/> -->");
                    check = false;
                }
            }

            string gp4 = pkgOut + @"\" + textContentID.Text + ".gp4";
            File.Create(gp4).Close();
            File.WriteAllLines(gp4, lines);
            File.Copy(gp4, tempPath + gp4.GetName());
            return gp4;
        }

        /// <summary>
        /// Patch param.sfo.
        /// </summary>
        private bool PatchSFO() {
            if (File.Exists(tempPath + @"\app0\sce_sys\param.sfo")) {
                ASCIIEncoding encode = new ASCIIEncoding();
                byte[] contID = encode.GetBytes(textContentID.Text);
                byte[] titID = encode.GetBytes(titleID);
                byte[] gName = encode.GetBytes(textBoxGameName.Text);

                try {
                    using (BinaryWriter binWriter = new BinaryWriter(new FileStream(tempPath + @"\app0\sce_sys\param.sfo", FileMode.Open, FileAccess.ReadWrite))) {
                        binWriter.BaseStream.Seek(0x2F8, SeekOrigin.Begin); // Content ID.
                        binWriter.Write(contID, 0, contID.Length);

                        binWriter.BaseStream.Seek(0x5D0, SeekOrigin.Begin); // Game Name.
                        binWriter.Write(gName, 0, gName.Length);

                        binWriter.BaseStream.Seek(0x650, SeekOrigin.Begin); // Title ID.
                        binWriter.Write(titID, 0, titID.Length);
                        binWriter.Close();
                    } return true;
                } catch (Exception exc) { MessagBox.Error(exc.ToString()); }
            } else MessagBox.Error("Can't access '" + tempPath + @"\app0\sce_sys\param.sfo'" + " !");
            return false;
        }

        /// <summary>
        /// Check the emus config and add entrys if needed.
        /// </summary>
        private void PatchConfig(){
            string[] lines = File.ReadAllLines(tempPath + @"\app0\config-emu-ps4.txt");

            foreach (string line in lines) {
                if (line.Contain("--path-patches='/app0/patches'")) {
                    if (patchesCliConfToolStrip.Checked || patchesConfLuaToolStrip.Checked) line.Replace("#", "");
                    else if (!line.Contain("#")) line.Insert(0, "#");
                } else if (line.Contain("--path-trophydata='/app0/trophy_data'")) {
                    if (trophydataToolStrip.Checked) line.Replace("#", "");
                    else if (!line.Contain("#")) line.Insert(0, "#");
                } else if (line.Contain("--path-featuredata='/app0/feature_data'")) {
                    if (featuredataToolStrip.Checked) line.Replace("#", "");
                    else if (!line.Contain("#")) line.Insert(0, "#");
                } else if (line.Contain("--path-toolingscript='/app0/patches'")) {
                    if (toolingscriptToolStrip.Checked) line.Replace("#", "");
                    else if (!line.Contain("#")) line.Insert(0, "#");
                } else if (line.Contain("--path-featuredata='/app0/patches'")) {
                    if (featuredatapatchToolStrip.Checked) line.Replace("#", "");
                    else if (!line.Contain("#")) line.Insert(0, "#");
                } else if (line.Contain("--ps2-title-id=")) line.XReplace(@"(\w{4})-(\d{5})", titleID.Insert(4, "-"));
                else if (line.Contain("--trophy-support=")) {
                    if (trophydataToolStrip.Checked) line.XReplace(@"(\d{1})", "1");
                    else line.XReplace(@"(\d{1})", "0");
                }
            }
            File.Delete(tempPath + @"\app0\config-emu-ps4.txt");
            File.Create(tempPath + @"\app0\config-emu-ps4.txt").Close();
            File.WriteAllLines(tempPath + @"\app0\config-emu-ps4.txt", lines);
        }

        /// <summary>
        /// Patch Title ID into nptitle.dat.
        /// </summary>
        private void PatchNpDat() {
            if (!File.Exists(tempPath + @"\app0\sce_sys\nptitle.dat")) { MessagBox.Error("Can not access '" + tempPath + @"\app0\sce_sys\nptitle.dat' !"); return; }
            using (BinaryWriter binWriter = new BinaryWriter(new FileStream(tempPath + @"\app0\sce_sys\nptitle.dat", FileMode.Open, FileAccess.ReadWrite))) {
                binWriter.BaseStream.Seek(0x10, SeekOrigin.Begin);
                byte[] _titleId = Encoding.ASCII.GetBytes(titleID);
                binWriter.Write(_titleId, 0, _titleId.Length);
                binWriter.Close();
            }
        }

        /// <summary>
        /// Check if ELFs of the base are Decrypted.
        /// </summary>
        /// <param name="path">Path to the template folder.</param>
        private bool IsElfDecrypted() {
            byte[] magic = new byte[4] { 0x7F, 0x45, 0x4C, 0x46, };

            foreach (string elf in elfs) {
                using (BinaryReader binReader = new BinaryReader(new FileStream(elf, FileMode.Open, FileAccess.Read))) {
                    byte[] fmagic = new byte[4];
                    binReader.Read(fmagic, 0, 4);
                    if (!magic.Contains(fmagic)) return false;
                    binReader.Close();
                }
            }
            return true;
        }

        /// <summary>
        /// Fake Sign and spoof authentication informations.
        /// </summary>
        /// <param name="deci">The TitleIDs decimal value as a hex string.</param>
        private bool FakeSign(string deci) {
            int index = toolStripComboBox.SelectedIndex;
            string _elfs = string.Empty;
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
                auth = deci + auth.Substring(4, auth.Length - 4);

                _elfs = string.Empty;
                foreach (string found in elfs) { if (found.Contain(elf)) _elfs = found; }
                if (_elfs == string.Empty) { MessagBox.Error("Couldn't find: " + elf); return false; }

                run.Arguments = "--paid " + auth.Substring(0, 16).EndianSwapp() + " --auth-info " + auth + " " + _elfs + " " + _elfs.Replace(".elf", "fself").Replace(".prx", "fself");
                MessagBox.Debug(run.Arguments);
                call.StartInfo = run;

                try { call.Start(); }
                catch (Exception io) { MessagBox.Error(io.ToString()); err = true; break; }

                call.BeginErrorReadLine();
                call.WaitForExit();
                count++;
            }

            if (err) return false;
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
        #endregion Functions
                
        #region Events
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
            textViewer = sett.TxtViewer;
            MessagBox.ButtonPosition = ButtonPosition.Center;
            _iso = true;

            // Just in case.
            string currDir = Directory.GetCurrentDirectory();
            if (make_fself == string.Empty) make_fself = currDir + @"\make_fself.py";
            if (orbis_pub_cmd == string.Empty) orbis_pub_cmd = currDir + @"\orbis-pub-cmd-ps2.exe";
            if (db == string.Empty) db = currDir + @"\template\authinfo_emu.txt";
            if (!File.Exists(make_fself)) MessagBox.Error("Can not find make_fself.py.\nEither drop the file into me, so i know where it is or\npleace it into my directory so i can acces it.");
            if (!File.Exists(orbis_pub_cmd)) MessagBox.Error("Can not find orbis-pub-cmd-ps2.exe.\nEither drop the file into me, so i know where it is or\npleace it into my directory so i can acces it.");
            if (!Directory.Exists(currDir + @"\template")) MessagBox.Error("Can not find the Template folder !\nPlease place the Template folder within same dir.");
            else tempPath = currDir + @"\template";
            
            // Get default text viewer if the setttings value is empty.
            if (textViewer == string.Empty) {
                textViewer = SwissKnife.GetDefaultApp(@"textfile\");
                if (textViewer == string.Empty) MessagBox.Info("Can't get the default TXT Viewer for editing the script.\nPlease tell me which app to use.\nOptions > Paths > Default Text/Code Viewer");
            }

            // Set elfs path.
            elfs = new string[4] {
                tempPath + @"\app0\eboot.elf",
                tempPath + @"\app0\ps2-emu-compiler.elf",
                tempPath + @"\app0\sce_module\libSceFios2.prx",
                tempPath + @"\app0\sce_module\libc.prx",
            };

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
        /// On close if form do.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void Fakeps2pkggen_FormClosing(object sender, FormClosingEventArgs e) {
            Settings sett = new Settings();
            sett.LastIsoPath = lastIsoPath;
            sett.LastOutPath = lastOutPath;
            sett.MakeFSELF = make_fself;
            sett.PubCmd = orbis_pub_cmd;
            sett.DB = db;
            sett.TxtViewer = textViewer;
            sett.Save();
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
        /// On TextBox CotnentID text changed.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void TextContentID_TextChanged(object sender, EventArgs e) {
            if (textContentID.Text.Length >= 16) textBoxPs2TID.Text = textContentID.Text.Substring(7, 9);
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
        private void CloseToolStrip_Click(object sender, EventArgs e) { Close(); }

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
        /// On Options Tool Strip Default Text and Code Viewer click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void OptionsDefTxtCVieToolStrip_Click(object sender, EventArgs e) {
            if (MessagBox.Question(Buttons.YesNo, textViewer + Environment.NewLine + Environment.NewLine + "Do you want to use a other Application ?") == DialogResult.Yes) {
                string newEditor = MessagBox.ShowOpenFile("Choose Editor", "All Files (*.*)|*.*", textViewer);
                if (newEditor != string.Empty) textViewer = newEditor;
            }
        }

        /// <summary>
        /// On Tool Strip Patches Cli conf click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void PatchesCliConfToolStrip_Click(object sender, EventArgs e) {
            if (patchesCliConfToolStrip.Checked) patchesCliConfToolStrip.Checked = true;
            else patchesCliConfToolStrip.Checked = true;
        }

        /// <summary>
        /// On Tool Strip Conf Lua click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void PatchesConfLuaToolStrip_Click(object sender, EventArgs e) {
            if (patchesConfLuaToolStrip.Checked) patchesConfLuaToolStrip.Checked = true;
            else patchesConfLuaToolStrip.Checked = true;
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
        /// On Emulator Tool Strip click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void EmulatorToolStrip_Click(object sender, EventArgs e) {
            EmulatorOptions emuOpt = new EmulatorOptions();
            emuOpt.ShowDialog();
            emuOpt.Dispose();
        }

        /// <summary>
        /// On Tool Strip About click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void AboutToolStripA_Click(object sender, EventArgs e) {
            About about = new About();
            about.ShowDialog();
        }

        /// <summary>
        /// On Tool Strip Build click.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguments.</param>
        private void ToolStripBuild_Click(object sender, EventArgs e) {
            if (toolStripComboBox.SelectedIndex > -1) {
                if (textContentID.Text != string.Empty) {
                    if (textBoxGameName.Text != string.Empty) {
                        if (HaseContentIDFormat(textContentID.Text)) {
                            if (ps2Iso != string.Empty) {
                                if (pkgOut != string.Empty) {
                                    if (BaseIsFine()) {
                                        // Copy ISO and check if destination dir is empty.
                                        idl.Text = "Cleaning old files up";
                                        PreClean();
                                        idl.Text = "Coping ISO";
                                        File.Copy(ps2Iso, tempPath + @"\app0\image\disc01.iso");

                                        // Get decimal of ItelID and convert to byte string, then overload to hte fake signing routine.
                                        idl.Text = "Fake Signing ELFs";
                                        titleID = textBoxPs2TID.Text;
                                        if (!FakeSign(GetDecimalBytes(titleID.Substring(4, 5)))) { idl.Text = "Error Fake Signing ELFs"; return; }

                                        // Copy and check the gp4 file and change all needed stuff.
                                        string gp4 = PatchGp4();
                                        if (gp4 == string.Empty) {
                                            MessagBox.Error("Error to many _cli.conf/_config.lua files with a title ID found.\nI don't know which one to use.\nPlease clean 'app0/patches/'.");
                                            idl.Text = "Error to many _cli.conf/_config.lua files.";
                                            return;
                                        }

                                        // Do the same with the config file.
                                        PatchConfig();

                                        // And do not forget the sfo.
                                        if (!PatchSFO()) return;

                                        // Call orbis pub cmd.
                                        idl.Text = "Generating PKG";
                                        MakePKG(gp4);
                                        if (!error) MessagBox.Show("Done !");
                                    } else MessagBox.Error("Base is not Ok !");
                                } else MessagBox.Error("No Output Folder set !");
                            } else MessagBox.Error("No PS2 ISO selected !");
                        } else MessagBox.Error("Content ID is not in Form of: XXYYYY-XXXXYYYYY_YY-XXXXXXXXXXXXXXXX");
                    } else MessagBox.Error("Game Name is empty !");
                } else MessagBox.Error("Content ID is empty !");
            } else MessagBox.Error("No FW Selected !");
        }
        #endregion Events
    }
}
