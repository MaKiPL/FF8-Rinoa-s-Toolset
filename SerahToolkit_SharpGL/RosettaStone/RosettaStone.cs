using System;
using System.Windows.Forms;
using System.IO;

namespace SerahToolkit_SharpGL.RosettaStone
{
    internal partial class RosettaStone : Form
    {
        private string _modelpath;
        private uint _start;
        private uint _end;
        private byte[] _b;

        public RosettaStone()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
                _modelpath = ofd.FileName;
            CheckFile(_modelpath);
        }

        private void CheckFile(string path)
        {
            if (File.Exists(path))
            {
                label1.Text = $"Opened: {path}";
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
            _start = (uint)numericUpDown1.Value;
            _end = (uint)numericUpDown2.Value + 1;
            if (_start > _end || _end == 0)
                return;
            _b = new byte[_end - _start];
            using (FileStream fs = new FileStream(_modelpath, FileMode.Open))
            {
                fs.Seek(_start, SeekOrigin.Begin);
                fs.Read(_b, 0, (int)_end - (int)_start);
            }
            string ret = FF8Text.BuildString(_b, 0, true);
            richTextBox1.Text = ret;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog {Filter = "*.txt|*.txt"};
            if (sfd.ShowDialog() == DialogResult.OK)
                File.WriteAllText(sfd.FileName, richTextBox1.Text);
        }
    }
}
