using System;
using System.IO;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL
{

    public partial class ManualGeometryRipper : Form
    {
        private const string _note = "This is manual geometry ripper!\nTo use it correctly you need to know what vertices, face indices, UV mapping is\nFinal Fantasy VIII uses MANY model structures and MANY ways of writing polygons\nThis works only for binary type of models and should be used only for testing and research\n\nThe options available here are only for most probably FFVIII structure\nIf you'd like similar software that would include much more ways of storing data like vertex in float, then see: \"HEX2OBJ\" by shakotay2";
        private string _path;
        private long _fileSize;

        public ManualGeometryRipper()
        {
            InitializeComponent();
            listBox1.SelectedIndex = 0;
            listBox2.SelectedIndex = 0;
        }

        private void importantInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_note, "Please read", MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("====How to use?====\nOpen file by clicking on menu option. After that you can build full model or only generate point-of-cloud/ parse polygons\nEvery parse button for every type of data makes the textbox on right get filled by converted data. Due to fact, that you'd maybe want to parse full model, then the textbox doesn't clear itself after clicking button. Please use \"reset\" button or delete it manually.\nAfter parsing copy all text in textbox to new text file and give it .obj extension\nIf you are still unsure, see the tutorial in official Qhimm topic");
        }

        private void button2_Click(object sender, EventArgs e)
        {
            if (!CheckFile(_path))
                return;

            byte vertexsize = radioButton1.Checked ? (byte)6 : (byte)8;
            if(numericUpDown2.Value*vertexsize + numericUpDown1.Value >= _fileSize)
            {
                Console.WriteLine("MGR: You set the values wrong. The amount of data you want to read is bigger than whole file size!");
                return;
            }
            byte[] buffer;
            using (FileStream fs = new FileStream(_path, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fs.Seek((long)numericUpDown1.Value,SeekOrigin.Begin);
                    buffer = br.ReadBytes((int)numericUpDown2.Value * vertexsize);
                }
            }
            if(buffer == null)
            {
                Console.WriteLine("MGR: Something went wrong with buffer reading from file...");
                return;
            }
            string f = null;
            for(int i = 0; i<buffer.Length; i+= vertexsize)
            {
                f += $"v {BitConverter.ToInt16(buffer, i)/2000.0f} {(BitConverter.ToInt16(buffer, i+2) / 2000.0f)*-1.0f} {BitConverter.ToInt16(buffer, i+4) / 2000.0f}\n";
            }
            if(f == null)
            {
                Console.WriteLine("MGR: Something went wrong with vertices parsing. Nothing parsed...");
                return;
            }
            richTextBox1.AppendText($"{f.Replace(',','.')}\n");
        }

        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _path = ofd.FileName;
                toolStripStatusLabel1.Text = _path;
            }

        }

        private bool CheckFile(string path)
        {
            if(path == null)
            {
                Console.WriteLine("MGR: No file opened?");
                return false;
            }
            if(!System.IO.File.Exists(path))
            {
                Console.WriteLine("MGR: Input file doesn't exist?");
                return false;
            }
            System.IO.FileInfo fi = new System.IO.FileInfo(path);
            if(fi.Length <= 5)
            {
                Console.WriteLine("MGR: This file is too small! You can't even write single vertex in there!");
                return false;
            }
            _fileSize = fi.Length;
            return true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            richTextBox1.Clear();
        }

        private void radioButton4_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown6.Enabled = true;
            label6.Text = "F4 (C)";
            label5.Text = "F3 (D)";
        }

        private void radioButton3_CheckedChanged(object sender, EventArgs e)
        {
            numericUpDown6.Enabled = false;
            label6.Text = "N/A";
            label5.Text = "F3 (C)";
        }

        private void button3_Click(object sender, EventArgs e)
        {
            if (!CheckFile(_path))
                return;

            if (numericUpDown8.Value + numericUpDown9.Value*numericUpDown7.Value >= _fileSize)
            {
                Console.WriteLine("MGR: You set the values wrong. The amount of data you want to read is bigger than whole file size!");
                return;
            }
            byte[] buffer;
            using (FileStream fs = new FileStream(_path, FileMode.Open))
            {
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fs.Seek((long)numericUpDown8.Value, SeekOrigin.Begin);
                    buffer = br.ReadBytes((int)numericUpDown9.Value * (int)numericUpDown7.Value);
                }
            }
            if (buffer == null)
            {
                Console.WriteLine("MGR: Something went wrong with buffer reading from file...");
                return;
            }

            string f = null;

            for(int i = 0; i <buffer.Length; i+= (int)numericUpDown7.Value)
            {
                int? d = 0;
                int a = listBox1.SelectedIndex != 0 ? BitConverter.ToUInt16(buffer, i+(int)numericUpDown3.Value) : buffer[i+(int)numericUpDown3.Value];
                int b = listBox1.SelectedIndex != 0 ? BitConverter.ToUInt16(buffer, i + (int)numericUpDown4.Value) : buffer[i + (int)numericUpDown4.Value];
                int c = listBox1.SelectedIndex != 0 ? BitConverter.ToUInt16(buffer, i + (int)numericUpDown5.Value) : buffer[i + (int)numericUpDown5.Value];
                if(radioButton4.Checked)
                    d = listBox1.SelectedIndex != 0 ? BitConverter.ToUInt16(buffer, i + (int)numericUpDown6.Value) : buffer[i + (int)numericUpDown6.Value];

                if(listBox2.SelectedIndex == 1) //GF like
                {
                    a = (int)Math.Round((double)(a / 8));
                    b = (int)Math.Round((double)(a / 8));
                    c = (int)Math.Round((double)(a / 8));
                    if(radioButton4.Checked)
                        d = (int)Math.Round((double)(a / 8));
                }

                if (listBox2.SelectedIndex == 2) //FF8.exe GF hidden geometry structures
                {
                    a &= 0x3F;
                    b &= 0x3F;
                    c &= 0x3F;
                    if (radioButton4.Checked)
                        d &= 0x3F;
                }

                f += $"f {a+1} {b+1} ";
                if (radioButton3.Checked)
                    f += $"{c+1}\n";
                else f += $"{d+1} {c+1}\n";
            }
            if (f == null)
            {
                Console.WriteLine("MGR: Something went wrong with vertices parsing. Nothing parsed...");
                return;
            }
            richTextBox1.AppendText($"{f.Replace(',', '.')}\n");
        }
    }
}
