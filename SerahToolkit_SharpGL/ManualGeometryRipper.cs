using System;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL
{
    public partial class ManualGeometryRipper : Form
    {
        private const string _note = "This is manual geometry ripper!\nTo use it correctly you need to know what vertices, face indices, UV mapping is\nFinal Fantasy VIII uses MANY model structures and MANY ways of writing polygons\nThis works only for binary type of models and should be used only for testing and research\n\nThe options available here are only for most probably FFVIII structure\nIf you'd like similar software that would include much more ways of storing data like vertex in float, then see: \"HEX2OBJ\" by shakotay2";

        public ManualGeometryRipper()
        {
            InitializeComponent();
        }

        private void importantInfoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(_note, "Please read", MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void howToUseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("====How to use?====\nOpen file by clicking on menu option. After that you can build full model or only generate point-of-cloud/ parse polygons\nEvery parse button for every type of data makes the textbox on right get filled by converted data. Due to fact, that you'd maybe want to parse full model, then the textbox doesn't clear itself after clicking button. Please use \"reset\" button or delete it manually.\nAfter parsing copy all text in textbox to new text file and give it .obj extension\nIf you are still unsure, see the tutorial in official Qhimm topic");
        }
    }
}
