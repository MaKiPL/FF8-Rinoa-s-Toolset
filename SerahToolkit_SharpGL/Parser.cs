using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.IO;

namespace SerahToolkit_SharpGL
{
    //Vertices done
    //VT Next


    public partial class Parser : Form
    {
        private string[] _file ;

        private List<float> X;
        private List<float> Y;
        private List<float> Z;   
        private string[] TextureCoordinates;
        private string[] FaceIndices;

        private byte[] start = {0x01, 0x00, 0x01, 0x00};
        private byte[] verticesCount;
        private List<byte[]> Vertices; 



        public Parser(int segment)
        {
            InitializeComponent();
            this.Text = segment.ToString();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Wavefront OBJ file .obj|*.obj";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _file = File.ReadAllLines(ofd.FileName);
                Process();
            }
        }

        private void Process()
        {


            //VERTICES

            X = new List<float>(); Y = new List<float>(); Z = new List<float>();
            foreach (var s in _file)
            {
                

                if (s.StartsWith("v "))
                {

                    string[] temp = s.Replace(".",",").Split(' ');
                    X.Add(float.Parse(temp[1])); Y.Add(float.Parse(temp[2]));
                    Z.Add(float.Parse(temp[3]));
                }
            }
            Vertices = new List<byte[]>();


            verticesCount = new byte[2]; verticesCount = BitConverter.GetBytes((UInt16)X.Count);

            for (int i = 0; i != X.Count; i++)
            {
                X[i] = X[i]*2000.0f; Y[i] = Y[i] * 2000.0f;
                Z[i] = Z[i]*2000.0f;

                short xs = short.Parse(X[i].ToString());
                short ys = short.Parse(Y[i].ToString());
                short zs = short.Parse(Z[i].ToString());

                byte[] vertex = new byte[6];
                Buffer.BlockCopy(BitConverter.GetBytes(xs),0,vertex,0,2);
                Buffer.BlockCopy(BitConverter.GetBytes(ys), 0, vertex, 2, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(zs), 0, vertex, 4, 2);
                Vertices.Add(vertex);

            }

            //Vertex Texture

            //Face Indices
        }
    }
}
