using System;
using System.Drawing;
using System.IO;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Windows.Forms;
using SerahToolkit_SharpGL.FF8_Core;
using SerahToolkit_SharpGL.Properties;

namespace SerahToolkit_SharpGL
{
    public partial class Text : Form
    {
        private readonly string _path;
        bool _bError = false;
        private readonly byte mode;

        private FF8_Core.ArchiveWorker aWorker;
        private SerahToolkit_SharpGL.wm2field.wm2f[] wm2fCol;
        private SerahToolkit_SharpGL.FF8_Core.PlayMovie mP;

        /// <summary>
        /// Mode: 
        /// 0= namedic
        /// 1= FS
        /// 2= wm2field
        /// 3= MovieExtractor
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
                if (mode == 2)
                    ofd.Filter = "wm2field.tbl|wm2field.tbl";
                if(mode != 3)
                    if (ofd.ShowDialog() == DialogResult.OK)
                        _path = ofd.FileName;
            }
            if (_path == null && mode != 3)
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
                case 2:
                    wm2field();
                    Initializewm2fComponent();
                    break;
                case 3:
                    MoviePlayer();
                    break;
                default:
                    Close();
                    break; //for compilers sake...
            }

        }

        private void MoviePlayer()
        {
            this.Text = "Movie extractor";
            PictureBox ExLow = new PictureBox();
            ExLow.Image = SerahToolkit_SharpGL.Properties.Resources.Save_icon;
            ExLow.SizeMode = PictureBoxSizeMode.StretchImage;
            ExLow.Size = new Size(32,32);
            ExLow.Text = "Extract high resolution";
            ExLow.Tag = "h";
            ExLow.Click += MP_Extract;
            PictureBox ExHigh = new PictureBox();
            ExHigh.Image = SerahToolkit_SharpGL.Properties.Resources.Save_icon1;
            ExHigh.SizeMode = PictureBoxSizeMode.StretchImage;
            ExHigh.Size = new Size(16, 16);
            ExHigh.Text = "Extract low resolution";
            ExHigh.Tag = "l";
            Label extL = new Label {Text = "Extract high res: "};
            Label extH = new Label {Text = " Extract low res: "};
            flowLayoutPanel1.Controls.Add(extL);
            flowLayoutPanel1.Controls.Add(ExLow); //That's in fact LOW RES
            flowLayoutPanel1.Controls.Add(extH);
            flowLayoutPanel1.Controls.Add(ExHigh);  //That's in fact HIGH RES
            ExHigh.Click += new EventHandler(MP_Extract); //Lambdas?
            dataGridView1.ReadOnly = true;
            dataGridView1.Columns[2].HeaderText = "Length";
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
        }

        public void PopulateMP()
        {
            for (int i = 0; i != mP.nClips; i++)
                dataGridView1.Rows.Add(i+1, mP._mClips[i].Resolutions[0].Offset, $"{mP._mClips[i].Frames/900}:{(mP._mClips[i].Frames/15)%60}");
        }

        private void MP_Extract(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog {Filter = "Bink Movie (.bik)|*.bik"};
            if (sfd.ShowDialog() != DialogResult.OK) return;
            int clipid = dataGridView1.SelectedCells[0].RowIndex;
            byte ResSwitch = 0;
            ResSwitch = (sender as PictureBox).Tag as string == "h" ? (byte)0 : (byte)1;
            byte[] buffer = new byte[mP._mClips[clipid].Resolutions[ResSwitch].Size];
            using (FileStream fs = new FileStream(mP.path, FileMode.Open, FileAccess.Read))
                using (BinaryReader br = new BinaryReader(fs))
                {
                    fs.Seek(mP._mClips[clipid].Resolutions[ResSwitch].Offset, SeekOrigin.Begin);
                    buffer = br.ReadBytes(buffer.Length);
                }
            File.WriteAllBytes(sfd.FileName, buffer);
            sfd.Dispose();
        }

        public void TransferMP(object sender) => mP = sender as PlayMovie; //Less accesible bla bla bla trick

        private void Initializewm2fComponent()
        {
            PictureBox wm2b = new PictureBox();
            wm2b.Image = SerahToolkit_SharpGL.Properties.Resources.Save_icon1;
            wm2b.SizeMode = PictureBoxSizeMode.StretchImage;
            wm2b.Size = new Size(32, 32);
            flowLayoutPanel1.Controls.Add(wm2b);
            wm2b.Click += wm2b_Click;
        }

        private void wm2b_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog {Filter = "wm2field.tbl|wm2field.tbl"};
            if(sfd.ShowDialog() != DialogResult.OK) return;

            using (FileStream fs = new FileStream(sfd.FileName, FileMode.Create, FileAccess.Write))
            {
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    foreach (DataGridViewRow row in dataGridView1.Rows)
                    {
                        short fX = short.Parse(Convert.ToString(row.Cells[1].Value));
                        short fY = short.Parse(Convert.ToString(row.Cells[2].Value));
                        ushort fZ = ushort.Parse(Convert.ToString(row.Cells[3].Value));
                        ushort fID = ushort.Parse(Convert.ToString(row.Cells[4].Value));
                        byte bUnk = byte.Parse(Convert.ToString(row.Cells[5].Value));

                        bw.Write(fX); bw.Write(fY); bw.Write(fZ); bw.Write(fID);
                        bw.Write(bUnk); bw.Write(bUnk); bw.Write(bUnk); bw.Write(bUnk);
                        bw.Write(new byte[12]);
                    }
                }
            }
            sfd.Dispose();

        }

        private void wm2field()
        {
            Text = "WM2FIELD";
            dataGridView1.Refresh();
            dataGridView1.Columns[0].HeaderText = "Entry ID";
            dataGridView1.Columns[1].HeaderText = "FieldX";
            dataGridView1.Columns[2].HeaderText = "FieldY";
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns[3].HeaderText = "FieldZ";
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns[4].HeaderText = "FieldID";
            dataGridView1.Columns.Add(new DataGridViewTextBoxColumn());
            dataGridView1.Columns[5].HeaderText = "UnknownPointer";
            dataGridView1.Columns[0].ReadOnly = true;
            dataGridView1.Refresh();

            wm2fCol = new wm2field.wm2f[SerahToolkit_SharpGL.wm2field.ENTRIES];
            SerahToolkit_SharpGL.wm2field wm2 = new wm2field(_path);
            for (int i = 0; i != 72; i++)
            {
                wm2fCol[i] = wm2.ReadEntry((uint) i);
                dataGridView1.Rows.Add(i, wm2fCol[i].FieldX, wm2fCol[i].FieldY, wm2fCol[i].FieldZ, wm2fCol[i].FieldID,
                    wm2fCol[i].UnknownPointer);
            }
        }

        private void WM2_Update(object sender, byte arg0)
        {
            Console.WriteLine(sender.ToString());
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
