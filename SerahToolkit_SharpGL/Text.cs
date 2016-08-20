using System;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL
{
    public partial class Text : Form
    {
        private string _path;
        bool _bError = false;

        /// <summary>
        /// Mode: 
        /// 0= namedic
        /// </summary>
        /// <param name="mode"></param>
        public Text(byte mode)
        {
            InitializeComponent();
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                if (mode == 0)
                    ofd.Filter = "namedic.bin|namedic.bin";
                if (ofd.ShowDialog() == DialogResult.OK)
                    _path = ofd.FileName;
                else Close();
            }
            if (_path == null)
                Close();
                switch(mode)
            {
                case 0:
                    Namedic();
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

        private void DataGridView1_CellValueChanged(object sender, DataGridViewCellEventArgs e)
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
            for (int i = effectiveRow; i!= dataGridView1.Rows.Count; i++)
                namedic._offsets[i] += (ushort)length;
            namedic._text[effectiveRow] = dataGridView1.CurrentCell.Value.ToString();
            dataGridView1.Rows.Clear();
            dataGridView1.Refresh();
            for (int i = 0; i != namedic._count; i++)
                dataGridView1.Rows.Add(i, namedic._offsets[i].ToString("X2"), namedic._text[i]);
        }
    }
}
