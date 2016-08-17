using System;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL
{
    public partial class Text : Form
    {
        private string _path;

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
        }
    }
}
