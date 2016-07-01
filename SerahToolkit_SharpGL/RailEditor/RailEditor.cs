using System;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.RailEditor
{
    public partial class RailEditor : Form
    {
        private byte[] rail;
        private int rot;


        public RailEditor(byte[] rail, int rot)
        {
            this.rail = rail;
            this.rot = rot;
            InitializeComponent();
            this.Text = $"Rail editor. Track: {rot.ToString()}";

        }
    }
}
