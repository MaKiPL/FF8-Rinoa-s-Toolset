using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.Forms
{
    //just because I'm too lazy to make it via dynamic generated form... >_>
    public partial class wm4 : Form
    {
        private WM_Section1 wm1s;
        private WM_Section4 wm4s;
        private WM_Section1.ENTRY[] entries;

        public static int ____a; //delegated lambda onClose dynamic form variable (lol)

        public wm4(object obj, object obj2)
        {
            wm1s = obj2 as WM_Section1;
            wm1s.ReadData();
            entries = wm1s.entries;
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
            dataGridView1.Columns.Add(groundColumn);
            dataGridView1.Columns.Add(encColumn);
            //672
            //wm4s.getEncounters[index]

            
            for (int i = 0; i < 672; i+=8)
            {
                int index = -1;
                int esi = i/8;
                bool bMultiRegion = false;
                int[] s = new int[2];
                for (int k = 0; k < entries.Length; k++)
                    if (entries[k].ESI == esi)
                    {
                        if (index != -1)
                        {
                            s[0] = index;
                            s[1] = k;
                            bMultiRegion = true;
                            break;
                        }
                        if (index == -1)
                            index = k;
                    }
                string region = null;
                if(index != -1)
                    region = bMultiRegion ? returnRegion(s) : entries[index].regionID.ToString();

                if (index != -1)
                {
                    dataGridView1.Rows.Add(i + 0, region, returnGroundType(entries[index].GroundID),
                        wm4s.GetEncounters[i + 0]);
                    dataGridView1.Rows.Add(i + 1, region, returnGroundType(entries[index].GroundID),
                        wm4s.GetEncounters[i + 1]);
                    dataGridView1.Rows.Add(i + 2, region, returnGroundType(entries[index].GroundID),
                        wm4s.GetEncounters[i + 2]);
                    dataGridView1.Rows.Add(i + 3, region, returnGroundType(entries[index].GroundID),
                        wm4s.GetEncounters[i + 3]);
                    dataGridView1.Rows.Add(i + 4, region, returnGroundType(entries[index].GroundID),
                        wm4s.GetEncounters[i + 4]);
                    dataGridView1.Rows.Add(i + 5, region, returnGroundType(entries[index].GroundID),
                        wm4s.GetEncounters[i + 5]);
                    dataGridView1.Rows.Add(i + 6, region, returnGroundType(entries[index].GroundID),
                        wm4s.GetEncounters[i + 6]);
                    dataGridView1.Rows.Add(i + 7, region, returnGroundType(entries[index].GroundID),
                        wm4s.GetEncounters[i + 7]);
                }
                else
                {
                    dataGridView1.Rows.Add(i + 0, "unused/special", "unused",
                        wm4s.GetEncounters[i + 0]);
                    dataGridView1.Rows.Add(i + 1, "unused/special", "unused",
                        wm4s.GetEncounters[i + 1]);
                    dataGridView1.Rows.Add(i + 2, "unused/special", "unused",
                        wm4s.GetEncounters[i + 2]);
                    dataGridView1.Rows.Add(i + 3, "unused/special", "unused",
                        wm4s.GetEncounters[i + 3]);
                    dataGridView1.Rows.Add(i + 4, "unused/special", "unused",
                        wm4s.GetEncounters[i + 4]);
                    dataGridView1.Rows.Add(i + 5, "unused/special", "unused",
                        wm4s.GetEncounters[i + 5]);
                    dataGridView1.Rows.Add(i + 6, "unused/special", "unused",
                        wm4s.GetEncounters[i + 6]);
                    dataGridView1.Rows.Add(i + 7, "unused/special", "unused",
                        wm4s.GetEncounters[i + 7]);
                }
            }
            dataGridView1.CellValueChanged += DataGridView1_CellValueChanged;
        }

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e) => wm4s.GetEncounters[e.RowIndex] = ushort.Parse(dataGridView1.Rows[e.RowIndex].Cells[3].Value.ToString()); 

        private string returnRegion(int[] i) => $"{entries[i[0]].regionID} and {entries[i[1]].regionID}";
        

        private string returnGroundType(int i)
        {
            switch (i)
            {
                case 4:
                    return "(4) BALAMB FOREST";
                case 6:
                    return "(6) PLAINS";
                case 10:
                    return "(10) BEACH";
                default:
                    return i.ToString();
            }
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

        private void editGroundIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int entryID = WhichEntry(this.dataGridView1);
            Form dialoger = new Form
            {
                Text = "Ground ID Editor",
                MinimizeBox = false,
                MaximizeBox = false,
                Size = new Size(200, 200)
            };
            Label lbltext = new Label {Text = "Please input ground ID:"};
            TextBox tbtBox = new TextBox {Text="0"};
            FlowLayoutPanel flp = new FlowLayoutPanel();
            flp.Controls.Add(lbltext);
            flp.SetFlowBreak(lbltext, true);
            flp.Controls.Add(tbtBox);
            //dialoger.Controls.Add(new FlowLayoutPanel());
            dialoger.Controls.Add(flp);
            dialoger.Closing += (o, args) => dialogerDelegateClose(o);
            dialoger.ShowDialog();
            dialoger.Dispose();

            entries[entryID].GroundID = (byte) ____a;

            int getRealOffset = entryID>0 ? entryID-(entryID%8) : 0;
            //int moduloFromEight = entryID%8;
            for (int i = getRealOffset; i < getRealOffset+8; i++)
                dataGridView1.Rows[i].Cells[2].Value = returnGroundType(entries[entryID].GroundID);

            dataGridView1.Rows[entryID].Cells[2].Value = returnGroundType(entries[entryID].GroundID);

            int realWMSET1index = (int)____a/8; //redundant int cast on purpouse
            realWMSET1index *= 4;

            using (FileStream fs = new FileStream(WM_Section1.ppath, FileMode.Open, FileAccess.ReadWrite))
                using (BinaryWriter bw = new BinaryWriter(fs))
                {
                    fs.Seek(realWMSET1index, SeekOrigin.Begin);
                    fs.Seek(2, SeekOrigin.Current); //jump to ground id
                    bw.Write(entries[entryID].GroundID);
                }
        }

        private void dialogerDelegateClose(object sender)
        {
            //getControl
            FlowLayoutPanel flp = (sender as Form).Controls[0] as FlowLayoutPanel;
            ____a = int.Parse(flp.Controls[1].Text);
        }

        private int WhichEntry(DataGridView a)
        {
            int loc = -1;
            if (a.SelectedRows.Count > 0)
                loc = a.SelectedRows[0].Index;
            if (a.SelectedCells.Count > 0)
                loc = a.SelectedCells[0].RowIndex;
            return loc;
        }
    }
}
