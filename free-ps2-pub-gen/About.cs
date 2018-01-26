using free_ps2_pub_gen.Properties;
using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace free_ps2_pub_gen {
    /// <summary>
    /// About Message Form.
    /// </summary>
    public partial class About : Form {
        /// <summary>
        /// Instanze Initializer.
        /// </summary>
        public About() { InitializeComponent(); }

        /// <summary>
        /// On load of form.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguemtns.</param>
        private void About_Load(object sender, EventArgs e) {
            pictureBox1.Image = Resources.Info.Resize(32, 32);
            label1.Text = "PS2 Fake PKG Generator for PS4\nand orbis-pub-cmd.exe patch by\n";
            linkLabel1.Text = "@cfwprpht";
            label3.Text = "PS4- PS2 PKG Technic by\n";
            linkLabel2.Text = "@flat_z";
            label5.Text = "Template PS2 Emu PKG provided by\n";
            linkLabel3.Text = "@Celest123";
        }

        /// <summary>
        /// Link Label clicked.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguemtns.</param>
        private void LinkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            linkLabel1.LinkVisited = true;
            Process.Start("https://twitter.com/cfwprophet");
        }

        /// <summary>
        /// Link Label clicked.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguemtns.</param>
        private void LinkLabel2_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            linkLabel2.LinkVisited = true;
            Process.Start("https://twitter.com/flat_z");
        }

        /// <summary>
        /// Link Label clicked.
        /// </summary>
        /// <param name="sender">The Sender.</param>
        /// <param name="e">The Event Arguemtns.</param>
        private void LlinkLabel3_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
            linkLabel3.LinkVisited = true;
            Process.Start("https://twitter.com/CelesteBlue123");
        }
    }
}
