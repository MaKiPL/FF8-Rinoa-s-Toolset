using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SerahToolkit_SharpGL.FF8_Core;
using SerahToolkit_SharpGL.Properties;

namespace SerahToolkit_SharpGL
{
    public partial class Text : Form
    {
        private string _path;
        bool _bError = false;
        private byte mode;

        private FF8_Core.ArchiveWorker aWorker;

        /// <summary>
        /// Mode: 
        /// 0= namedic
        /// </summary>
        /// <param name="mode"></param>
        public Text(byte mode)
        {
            InitializeComponent();
            this.mode = mode;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (mode == 0)
                    ofd.Filter = "namedic.bin|namedic.bin";
                if (mode == 1)
                    ofd.Filter = "FS Archive|*.FS";
                if (ofd.ShowDialog() == DialogResult.OK)
                    _path = ofd.FileName;
            }
            if (_path == null)
            {
                //Close();
                //this.Dispose();
                //Hide();
                return;
            }
            switch(mode)
            {
                case 0:
                    Namedic();
                    InitializeNamedicComponent();
                    break;
                case 1:
                    FS();
                    InitializeFSComponent();
                    break;
                default:
                    Close();
                    break; //for compilers sake...
            }

        }

        private void Namedic()
        {
            string[] buffer = namedic.GetText(_path);
            Text = "Namedic.bin";
            ushort[] off = namedic._offsets;
            for (int i = 0; i != namedic._count; i++)
                dataGridView1.Rows.Add(i, off[i].ToString("X2"), buffer[i]);
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
        }

        private void FS()
        {
            Text = "FS Archive";
            dataGridView1.Refresh(); //data grid view is sooooo broken...
            dataGridView1.Columns[0].HeaderText = "ID";
//            dataGridView1.Columns[0].CellTemplate = DataGridViewTextBoxCell;
            dataGridView1.Columns[1].HeaderText = "Size";
            dataGridView1.Columns[2].HeaderText = "FileName";
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns[3].HeaderText = "LZSS?";
            aWorker = new ArchiveWorker(_path);
            ArchiveWorker.FI[] fi = aWorker.GetFI();
            string[] file = aWorker.GetListOfFiles();
            for (int i = 0; i <= aWorker.GetListOfFiles().Length-1; i++)
                dataGridView1.Rows.Add(i, fi[i].LengthOfUnpackedFile, file[i], fi[i].LZSS >= 1 ? "YES" : "NO");
        }

        private void InitializeNamedicComponent()
        {
            PictureBox pb = new PictureBox();
            pb.Image = SerahToolkit_SharpGL.Properties.Resources.Save_icon1;
            pb.SizeMode = PictureBoxSizeMode.StretchImage;
            pb.Size = new Size(32, 32);
            flowLayoutPanel1.Controls.Add(pb);
            pb.Click += Pb_Click;
        }

        private void InitializeFSComponent()
        {
            PictureBox fs = new PictureBox();
            fs.Image = SerahToolkit_SharpGL.Properties.Resources.Settings_icon;
            fs.Size = new Size(32,32);
            fs.SizeMode = PictureBoxSizeMode.StretchImage;
            flowLayoutPanel1.Controls.Add(fs);
            fs.Click += fs_Click;
            fs.Text = Resources.Text_InitializeFSComponent_Export; //test
            dataGridView1.ReadOnly = true;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.MultiSelect = true;
            dataGridView1.Refresh();
        }

        private void fs_Click(object sender, EventArgs e)
        {
            DataGridViewSelectedCellCollection rowCollection = dataGridView1.SelectedCells;
            if(rowCollection.Count == 1)
                FS_SingleFile(rowCollection[0].Value);
                //FS_SingleFile(rowCollection[0].Cells[2].Value);
            if(rowCollection.Count > 1)
                FS_MultiFiles(rowCollection);
        }

        private void FS_MultiFiles(DataGridViewSelectedCellCollection collections)
        {
            string dir;
            using (FolderBrowserDialog fbd = new FolderBrowserDialog())
            {
                fbd.Description = "Select folder where you want to extract selected files";
                if (fbd.ShowDialog() == DialogResult.OK)
                    dir = fbd.SelectedPath;
                else return;
            }
            for (int i = 0; i != collections.Count; i++)
                saveFSfile(dir,
                    //collections[i].Cells[2].Value.ToString().Substring(16),
                    collections[i].Value.ToString().Substring(16),
                    ArchiveWorker.GetBinaryFile(
                        $"{Path.GetDirectoryName(aWorker._path)}\\{Path.GetFileNameWithoutExtension(aWorker._path)}",
                        collections[i].Value.ToString()));
        }

        private void saveFSfile(string path, string filename, byte[] buffer)
        {
            Console.WriteLine($"Exported file: {filename}");
            System.IO.Directory.CreateDirectory($"{path}\\{Path.GetDirectoryName(filename)}");
            File.WriteAllBytes($"{path}\\{filename}", buffer);
        }

        private void FS_SingleFile(object file)
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = $"{Path.GetFileName(file.ToString())}|{Path.GetFileName(file.ToString())}";
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    File.WriteAllBytes(sfd.FileName,
                        ArchiveWorker.GetBinaryFile(
                            $"{Path.GetDirectoryName(aWorker._path)}\\{Path.GetFileNameWithoutExtension(aWorker._path)}",
                            file.ToString()));
                    Console.WriteLine($"Saved file: {file.ToString().Substring(16)}");
                }
            }
        }

        private void Pb_Click(object sender, EventArgs e)
        {
            if (_bError)
            {
                MessageBox.Show("Can't save, one cell may be empty?");
                return;
            }
            string pt = null;
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "namedic.bin|namedic.bin";
                if (sfd.ShowDialog() != DialogResult.OK)
                    return;
                pt = sfd.FileName;
            }
            System.IO.File.WriteAllBytes(pt, namedic.BuildFile());
        }



        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (mode == 0)
            {
                //MessageBox.Show(dataGridView1.CurrentCell.Value.ToString().Length.ToString());
                int effectiveRow = dataGridView1.CurrentRow.Index;
                if (dataGridView1.CurrentCell.Value == null)
                {
                    MessageBox.Show("Cell cannot be null!");
                    _bError = true;
                    return;
                }
                _bError = false;

                int length = dataGridView1.CurrentCell.Value.ToString().Length;
                length = length - namedic._text[effectiveRow].Length;

                //length = length - int.Parse(dataGridView1.Rows[dataGridView1.CurrentRow.Index].Cells[1].Value.ToString());

                /* C# broken algorithm - causes stack overflow every possible time, no matter what
                 *             int i = effectiveRow;
                while (true)
                {
                    //I don't know why, but datagridview resets my [i] to 0 making stack overflow
                    int stackOverflowBug = i;
                    int value = (namedic._offsets[stackOverflowBug] + length);
                    string stackoverflow = value.ToString("X2");
                    dataGridView1.Rows[stackOverflowBug].Cells[1].Value = stackoverflow;
                    namedic._offsets[stackOverflowBug] = (ushort)(namedic._offsets[stackOverflowBug] + length);
                    if (i == dataGridView1.Rows.Count) break;
                    i++;
                }
                */

                //stack overflow workout
                for (int i = effectiveRow+1; i != dataGridView1.Rows.Count; i++)
                    namedic._offsets[i] += (ushort)length;
                namedic._text[effectiveRow] = dataGridView1.CurrentCell.Value.ToString();
                dataGridView1.Rows.Clear();
                dataGridView1.Refresh();
                for (int i = 0; i != namedic._count; i++)
                    dataGridView1.Rows.Add(i, namedic._offsets[i].ToString("X2"), namedic._text[i]);
            }
        }
    }

    public class Text_LZSS
    {
        public string fileSave;

        public /*bool*/ byte[] TryDecompress()
        {
            string s;
            using (OpenFileDialog ofd = new OpenFileDialog())
                if (ofd.ShowDialog() == DialogResult.OK)
                    s = ofd.FileName;
                else return null/*false*/;
            fileSave = Path.GetFileName(s);
            byte[] buffer = System.IO.File.ReadAllBytes(s);
            //return LZSS.DecompressAll(buffer, (uint)buffer.Length).Length > 0 ? false:g
            return LZSS.DecompressAll(buffer, (uint) buffer.Length);
        }
    }
}
