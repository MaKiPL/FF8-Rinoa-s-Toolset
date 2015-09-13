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
        private List<UInt16> A;
        private List<int> At;
        private List<int> Bt;
        private List<int> Ct; 
        private List<UInt16> B;
        private List<UInt16> C;   

        private int height; //Texture height for PAGE id calculation
        private int width; //As above

        private byte[] start = {0x01, 0x00, 0x01, 0x00};
        private byte[] verticesCount; //Count of vertices
        private byte[] TrianglesCount; //Count of triangles (polygons)
        private List<byte[]> Vertices; //All vertices byte
        private List<byte[]> Polygon; //All ready polygons FACE+VT  bytes
        private int[] TPage; 

        private Dictionary<decimal, byte[]> CLUT = new Dictionary<decimal, byte[]>
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
                richTextBox1.Clear();
                richTextBox1.AppendText("Opened: " + ofd.FileName);
                Process();
            }
        }

        private void Process()
        {
            

            //VERTICES

            X = new List<float>(); Y = new List<float>(); Z = new List<float>();
            U = new List<double>(); V = new List<double>();
            U_pixel = new List<double>(); V_pixel = new List<double>();
            A = new List<ushort>(); B = new List<ushort>(); C = new List<ushort>();
            At = new List<int>(); Bt = new List<int>(); Ct = new List<int>();

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
                    string[] Aa = temp[1].Split('/'); // A/T
                    A.Add(UInt16.Parse(Aa[0]));
                    At.Add(int.Parse(Aa[1]));
                    string[] Bb = temp[2].Split('/'); // B/T
                    B.Add(UInt16.Parse(Bb[0]));
                    Bt.Add(int.Parse(Bb[1]));
                    string[] Cc = temp[3].Split('/'); // C/T
                    C.Add(UInt16.Parse(Cc[0]));
                    Ct.Add(int.Parse(Cc[1]));
                }
            }
            Vertices = new List<byte[]>();

            UInt16 VertCount = Convert.ToUInt16(X.Count);
            verticesCount = new byte[2];
            verticesCount = BitConverter.GetBytes(VertCount);
            richTextBox1.AppendText(Environment.NewLine + "Vertices: " + VertCount);
            richTextBox1.AppendText(Environment.NewLine + "VT: " + U.Count);
            richTextBox1.AppendText(Environment.NewLine + "Triangles: " + C.Count);

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

            richTextBox1.AppendText(Environment.NewLine + "Vertices calculated and converted to FF8 format");
            /*
            FileStream fs = new FileStream(@"D:\testsegment.bin", FileMode.Append);
            foreach (byte[] b in Vertices)
            {
                fs.Write(b, 0, b.Length);
            }*/


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
            TPage = new int[U.Count];
            for (int i = 0; i != U.Count; i++)
            {
                U_pixel.Add(Math.Round(((U[i]*100.0d)*height)/100.0d));  //  ( (0.501953*100) * 256 ) / 100.0
                V_pixel.Add(Math.Round(((V[i]*100.0d)*width)/100.0d));
                
                while (true)
                {
                    if (U_pixel[i] <= 128.0d)
                        break;

                    U_pixel[i] -= 128.0d;
                    TPage[i]++; 

                }
            }

            richTextBox1.AppendText(Environment.NewLine + "TPaging calculated and parsed");
            richTextBox1.AppendText(Environment.NewLine + "Mapping was calculated and parsed");

            //Face Indices

            //  Wing order:     U1/T2 U2/T3 U3/T1 
            // FACE - 1 REMEMBER
            Polygon = new List<byte[]>();

            UInt16 tempTrianglesCount = Convert.ToUInt16(A.Count);
            TrianglesCount = BitConverter.GetBytes(tempTrianglesCount);

            byte TPagePreOperand = 0xB0;

            for (int i = 0; i != tempTrianglesCount; i++)
            {
                byte[] triangle = new byte[20];
                Buffer.BlockCopy(BitConverter.GetBytes(A[i]-1),0,triangle,0,2); //A
                Buffer.BlockCopy(BitConverter.GetBytes(B[i]-1), 0, triangle, 2, 2); //B
                Buffer.BlockCopy(BitConverter.GetBytes(C[i]-1), 0, triangle, 4, 2); //C
                Buffer.BlockCopy(BitConverter.GetBytes(U[At[i]-1]),0,triangle,6,1); //U1
                Buffer.BlockCopy(BitConverter.GetBytes(V[At[i]-1]), 0, triangle, 7, 1); //V1
                Buffer.BlockCopy(BitConverter.GetBytes(U[Bt[i]-1]), 0, triangle, 8, 1); //U2
                Buffer.BlockCopy(BitConverter.GetBytes(V[Bt[i]-1]), 0, triangle, 9, 1); //V2
                Buffer.BlockCopy(CLUT[numericUpDown1.Value],0,triangle,10,2); //CLUTid
                Buffer.BlockCopy(BitConverter.GetBytes(U[Ct[i]-1]), 0, triangle, 12, 1); //U3
                Buffer.BlockCopy(BitConverter.GetBytes(V[Ct[i]-1]), 0, triangle, 13, 1); //V3
                byte[] tempTP = BitConverter.GetBytes(TPage[At[i] - 1]);
                var a = tempTP[0] | TPagePreOperand; //BITwise 0xB0 OR 0x0? = 0xB?
                triangle[14] = Convert.ToByte(a); // TPage Bitwised with 0xB0 (UNKNOWN)
                //Buffer.BlockCopy(ARRAY[here], 0, triangle, 14, 1); //TPAGE !!!!

                //PASS bHide = 0, and triangle[15] is NULL (00);
                triangle[16] = 0x80; triangle[17] = 0x80; triangle[18] = 0x80; // R G B
                triangle[19] = 0x2c; //PSOne GPU


                Polygon.Add(triangle);
            }
            richTextBox1.AppendText(Environment.NewLine + "Triangle data forged succesfully");
            richTextBox1.AppendText(Environment.NewLine + "Preparing to save segment...");
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Segment file used to replace *.xBIN|*.xBIN";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                FileStream fs = new FileStream(sfd.FileName, FileMode.Append);
                fs.Write(start, 0, start.Length);
                fs.Write(verticesCount, 0, verticesCount.Length);
                foreach (byte[] b in Vertices)
                {
                    fs.Write(b, 0, b.Length);
                }
                fs.Write(CalculatePadding(4), 0, CalculatePadding(4).Length); // <---- EDIT ME
                fs.Write(TrianglesCount, 0, 2);
                fs.Write(new byte[6], 0, 2); // NULL + padding
                foreach (var variable in Polygon)
                {
                    fs.Write(variable, 0, 20);
                }
                richTextBox1.AppendText("Succesfully saved " + fs.Length.ToString() + " bytes.");
                fs.Close();
            }
            else
                richTextBox1.AppendText("Save segment failed");
        }

        public byte[] CalculatePadding(int globalOffset)
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
