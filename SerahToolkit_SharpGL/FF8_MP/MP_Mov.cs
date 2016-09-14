using System;
using System.IO;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.FF8_MP
{
    public partial class MP_Mov : Form
    {
        public MP_Mov()
        {
            InitializeComponent();
        }

        public void PlayMovie(string movie)
        {
            FileInfo fi = new FileInfo(movie);
            vlcControl1.SetMedia(fi);
            vlcControl1.Play();
        }
    }
}
