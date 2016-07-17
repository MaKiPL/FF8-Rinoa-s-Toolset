using System;
using System.Data;
using System.Windows.Forms;
using System.IO;

namespace SerahToolkit_SharpGL
{
    internal partial class AnalizeCff8Search : Form
    {
        private const string Ff8 = "FF8";
        private const int Ff8Size = 128;

        public AnalizeCff8Search()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(ofd.FileName))
                    SEARCH_ok(ofd.FileName);
            }
        }

        private void SEARCH_ok(string path)
        {
            dataGridView1.Rows.Clear();
            FileInfo fi = new FileInfo(path);
            byte[] wholeFile = File.ReadAllBytes(path);

            for (int index = 0; index!=fi.Length; index++)
                {
                        var debugWhSuBstr = System.Text.Encoding.ASCII.GetString(wholeFile,index,Ff8Size);
                if (index >= fi.Length - 2049)
                    break;
                debugWhSuBstr = debugWhSuBstr.Substring(0, 3);
                if (debugWhSuBstr == Ff8)
                    {
                    int index3 = index;
                        for(int index2 = index; index2!=index+Ff8Size; index2++)
                        {
                            if(wholeFile[index2] == 0x00)
                            {
                                index3 = index2;
                                break;
                            }
                        }
                    string buildString = System.Text.Encoding.ASCII.GetString(wholeFile, index, index3 - index);
                    dataGridView1.Rows.Add(index.ToString(), buildString);
                    }
                }
            
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            if(sfd.ShowDialog() == DialogResult.OK)
            {
                if (File.Exists(sfd.FileName))
                    File.Delete(sfd.FileName);
                DataTable dt = new DataTable("RIP data");
                foreach (DataGridViewColumn a in dataGridView1.Columns)
                    dt.Columns.Add(a.HeaderText);
                foreach(DataGridViewRow row in dataGridView1.Rows)
                {
                    DataRow drow = dt.NewRow();
                    foreach(DataGridViewCell cell in row.Cells)
                    {
                        drow[cell.ColumnIndex] = cell.Value;
                    }
                    dt.Rows.Add(drow);
                }
                dt.WriteXml(sfd.FileName);
            }
        }
    }
}
