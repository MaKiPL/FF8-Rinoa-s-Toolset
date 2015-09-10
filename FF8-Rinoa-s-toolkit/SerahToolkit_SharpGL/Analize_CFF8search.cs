using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SerahToolkit_SharpGL
{
    public partial class Analize_CFF8search : Form
    {
        const string FF8 = "FF8";
        const int FF8_Size = 256;
        const byte EOF = 0;


        public Analize_CFF8search()
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
            string _debug_WH_SUBstr = null;

                for (int index = 0; index!=fi.Length; index++)
                {
                        _debug_WH_SUBstr = System.Text.Encoding.ASCII.GetString(wholeFile,index,FF8_Size);
                if (index >= fi.Length - 2049)
                    break;
                _debug_WH_SUBstr = _debug_WH_SUBstr.Substring(0, 3);
                if (_debug_WH_SUBstr == FF8)
                    {
                    int index3 = index;
                        for(int index2 = index; index2!=index+FF8_Size; index2++)
                        {
                            if(wholeFile[index2] == 0x00)
                            {
                                index3 = index2;
                                break;
                            }
                        }
                    string BuildString = System.Text.Encoding.ASCII.GetString(wholeFile, index, index3 - index);

                    dataGridView1.Rows.Add(index.ToString(), BuildString);
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
