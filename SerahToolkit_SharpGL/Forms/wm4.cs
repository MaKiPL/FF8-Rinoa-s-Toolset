using System;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.Forms
{
    //just because I'm too lazy to make it via dynamic generated form... >_>
    public partial class wm4 : Form
    {
        private WM_Section4 wm4s;
        public wm4(object obj)
        {
            wm4s = obj as WM_Section4;
            InitializeComponent();
        }

        public void InitialUpdate(ushort[] enc)
        {
            dataGridView1.MultiSelect = false;
            dataGridView1.AllowUserToAddRows = false;
            dataGridView1.AllowUserToDeleteRows = false;
            DataGridViewTextBoxColumn idColumn = new DataGridViewTextBoxColumn {HeaderText = "ID", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells};
            DataGridViewTextBoxColumn regionColumn = new DataGridViewTextBoxColumn { HeaderText = "Region", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells };
            DataGridViewTextBoxColumn groundColumn = new DataGridViewTextBoxColumn { HeaderText = "Ground type", ReadOnly = true, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells };
            DataGridViewTextBoxColumn encColumn = new DataGridViewTextBoxColumn { HeaderText = "Encounter", ReadOnly = false, AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells };
            dataGridView1.Columns.Add(idColumn);
            dataGridView1.Columns.Add(regionColumn);
            dataGridView1.Columns.Add(encColumn);

            for (int i = 0; i < 672; i++)
                dataGridView1.Rows.Add(i, returnRegion(i), wm4s.GetEncounters[i]);
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) => wm4s.GetEncounters[e.RowIndex] = ushort.Parse(dataGridView1.Rows[e.RowIndex].Cells[2].Value as string);

        private string returnRegion(int i) => WM_Section4.esiDividors[i/8];

        private string returnGroundType(int i)
        {
            int esicat = i/8;
            return WM_Section4.groundTypes[(byte)esicat];
        }

        //private int returnRegion(int i) => (int) Math.Round((decimal) (i/48), 0); <--deprecated

        private void compileSaveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ushort[] collection = wm4s.GetEncounters;
            byte[] buffer = new byte[collection.Length*2+4];
            Buffer.BlockCopy(collection,0,buffer,0,buffer.Length-4);
            System.IO.File.WriteAllBytes(wm4s.path, buffer);
            this.Close();
        }
    }
}
