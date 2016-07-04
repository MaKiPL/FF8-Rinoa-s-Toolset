using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.BattleStageEditor
{
    public partial class BSEdit : Form
    {
        struct triangle
        {

        }
        struct quad
        {

        }
        struct Vertex3
        {
            Int16 x;
            Int16 y;
            Int16 z;
        }


        public BSEdit(string path, int offset)
        {
            InitializeComponent();
            BattleStage bs = new BattleStage(path);
            bs.Editor(offset);
        }
    }
}
