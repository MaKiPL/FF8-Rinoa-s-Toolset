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

        //private List<Vector3>
        private List<float> X;
        private List<float> Y;
        private List<float> Z;   
        //private Vector2 //Forget it
        private List<double> U;
        private List<double> U_pixel; 
        private List<double> V;
        private List<double> V_pixel;

        private int height; //Texture height for PAGE id calculation
        private int width; //As above

        private byte[] start = {0x01, 0x00, 0x01, 0x00};
        private byte[] verticesCount; //Count of vertices
        private byte[] TrianglesCount; //Count of triangles (polygons)
        private List<byte[]> Vertices; //All vertices byte
        private List<byte[]> VT; //All VertexCoords bytes.
        private List<byte[]> Polygon; //All ready polygons FACE+VT  bytes

        private Dictionary<int, byte[]> CLUT = new Dictionary<int, byte[]>
        {
            { 0,new byte[] {0x00, 0x3C} },
            { 1,new byte[] {0x40, 0x3C} },
            { 2,new byte[] {0x80, 0x3C} },
            { 3,new byte[] {0xC0, 0x3C} },
            { 4,new byte[] {0x00, 0x3D} },
            { 5,new byte[] {0x40, 0x3D} },
            { 6,new byte[] {0x80, 0x3D} },
            { 7,new byte[] {0xC0, 0x3D} },
            { 8,new byte[] {0x00, 0x3E} },
            { 9,new byte[] {0x40, 0x3E} },
            { 10,new byte[] {0x80, 0x3E} },
            { 11,new byte[] {0xC0, 0x3E} },
            { 12,new byte[] {0x00, 0x3F} },
            { 13,new byte[] {0x40, 0x3F} },
            { 14,new byte[] {0x80, 0x3F} },
            { 15,new byte[] {0xC0, 0x3F} },
        };

        public Parser(int segment, int height, int width)
        {
            InitializeComponent();
            this.Text = segment.ToString();
            this.height = height;
            this.width = width;
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

                    string[] temp = s.Replace(".", ",").Split(' ');
                    X.Add(float.Parse(temp[1])); Y.Add(float.Parse(temp[2]));
                    Z.Add(float.Parse(temp[3]));
                }
                else if (s.StartsWith("vt "))
                {
                    string[] temp = s.Replace(".", ",").Split(' ');
                    U.Add(double.Parse(temp[1])); V.Add(double.Parse(temp[2]));
                }
                else if (s.StartsWith("f "))
                {
                    string[] temp = s.Split(' '); // F[0] 1/1[1] 2/2[2] 3/3[3] 4/4[UNUSED]
                    string[] A = temp[1].Split('/'); // A/T
                    string[] B = temp[2].Split('/'); // B/T
                    string[] C = temp[3].Split('/'); // C/T
                }
            }
            Vertices = new List<byte[]>();

            UInt16 VertCount = Convert.ToUInt16(X.Count);
            verticesCount = new byte[2];
            verticesCount = BitConverter.GetBytes(VertCount);

            for (int i = 0; i != X.Count; i++)
            {
                X[i] = X[i] * 2000.0f; Y[i] = Y[i] * 2000.0f;
                Z[i] = Z[i] * 2000.0f;
                /*
                *Nope. 

                short xs = X[i].ToString().Length <= 6
                    ? short.Parse(X[i].ToString())
                    : short.Parse(X[i].ToString().Substring(0, X[i].ToString().Length + (6 - X[i].ToString().Length))); //eg 8+(6-8)= 8+(-2) = 8-2 = 6
                short ys = Y[i].ToString().Length <= 6
                    ? short.Parse(Y[i].ToString())
                    : short.Parse(Y[i].ToString().Substring(0, Y[i].ToString().Length + (6 - Y[i].ToString().Length)));
                short zs = Z[i].ToString().Length <= 6
                    ? short.Parse(Z[i].ToString())
                    : short.Parse(Z[i].ToString().Substring(0, Z[i].ToString().Length + (6 - Z[i].ToString().Length)));
                    */


                /*
                *Still don't work as intended. I don't know why

                int Deleteindex = X[i].ToString().IndexOf(",");
                short xs = Deleteindex == -1 || Deleteindex == 0
                    ? short.Parse(X[i].ToString())
                    : short.Parse(X[i].ToString().Substring(0, X[i].ToString().Length - Deleteindex));
                Deleteindex = Y[i].ToString().IndexOf(",");
                short ys = Deleteindex == -1 || Deleteindex == 0
                    ? short.Parse(Y[i].ToString())
                    : short.Parse(Y[i].ToString().Substring(0, Y[i].ToString().Length - Deleteindex));
                Deleteindex = Z[i].ToString().IndexOf(",");
                short zs = Deleteindex == -1 || Deleteindex == 0
                    ? short.Parse(Z[i].ToString())
                    : short.Parse(Z[i].ToString().Substring(0, Z[i].ToString().Length - Deleteindex));
                    */

                //Works now
                double d = Math.Round(X[i]); short xs = short.Parse(d.ToString());
                d = Math.Round(Y[i]); short ys = short.Parse(d.ToString());
                d = Math.Round(Z[i]); short zs = short.Parse(d.ToString());


                byte[] vertex = new byte[6];
                Buffer.BlockCopy(BitConverter.GetBytes(xs), 0, vertex, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(ys), 0, vertex, 2, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(zs), 0, vertex, 4, 2);
                Vertices.Add(vertex);

            }

            FileStream fs = new FileStream(@"D:\testsegment.bin", FileMode.Append);
            foreach (byte[] b in Vertices)
            {
                fs.Write(b, 0, b.Length);
            }


            //Vertex Texture

            /*
                    SOME LOGIC:
                    0.0 is 0
                    1.0 is height or width
                    TPage is 128

            Example:
                    vt 0.501953 0.996094
                    height=256
                    
                    100% = 256
                    0.501953%*100 = x
                    x= 128,499968
                    
            TPage routine:
                    IF x is GREATER OR EQUAL then x-128 and Tpage++ TILL x IS NOT greater or equal
                    example:
                    x is 673  [Need for while(true) loop to break if not greater)
                    673-128= 545    TPage=1                    
                    545-128= 417    TPage=2
                    417-128= 289    TPage=3
                    289-128= 161    TPage=4
                    161-128= 33     TPage=5
                    Parse 33 as V to byte and apply TPage=5
                    
    */
            for (int i = 0; i != U.Count; i++)
            {
                U_pixel.Add(Math.Round(((U[i]*100.0d)*height)/100.0d));  //  ( (0.501953*100) * 256 ) / 100.0
                V_pixel.Add(Math.Round(((V[i]*100.0d)*width)/100.0d));
            }

            //Face Indices
        }

        private byte[] CalculatePadding(int globalOffset)
        {
            //Plus two
            //return new byte[] = {0x00} ??
            switch (globalOffset%4)
            {
                case 0:
                    return new byte[2];
                case 1:
                    return new byte[1+2];
                    
                case 2:
                    return new byte[2+2];
                    
                case 3:
                    return new byte[3+2];
                    
                case 4:
                    return new byte[4+2];
                    
                default:
                    return new byte[2];
                    
                    
            }

        }
    }
}
