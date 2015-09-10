using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SerahToolkit_SharpGL.RosettaStone
{
    public partial class RosettaStone : Form
    {
        private string Modelpath;
        private UInt32 START;
        private UInt32 END;
        private Byte[] b;

        public RosettaStone()
        {
            InitializeComponent();
            
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                Modelpath = ofd.FileName;
            CheckFile(Modelpath);

        }

        private void CheckFile(string path)
        {
            if (File.Exists(path))
            {
                label1.Text = "Opened: " + path.ToString();
                button1.Enabled = true;
            }
            else
            {
                button1.Enabled = false;
                label1.Text = "File error";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
            START = (UInt32)numericUpDown1.Value;
            END = (UInt32)numericUpDown2.Value + 1;
            b = new byte[END - START];
            using (var fs = new FileStream(Modelpath, FileMode.Open))
            {
                fs.Seek(START, SeekOrigin.Begin);
                fs.Read(b, 0, (int)END - (int)START);
            }
            CharTable_provider ct = new CharTable_provider();
            string[] ret = ct.Decipher(b);
            foreach (string a in ret)
            {
                if (a != null)
                    richTextBox1.AppendText(a);
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "*.txt|*.txt";
            if (sfd.ShowDialog() == DialogResult.OK)
                File.WriteAllText(sfd.FileName, richTextBox1.Text);
        }
    }
}
