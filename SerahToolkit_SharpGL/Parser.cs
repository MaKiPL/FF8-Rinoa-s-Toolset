using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Globalization;
using System.IO;


namespace SerahToolkit_SharpGL
{
    internal partial class Parser : Form
    {
        private string[] _file ;
        private List<float> _x;
        private List<float> _y;
        private List<float> _z;   
        private List<double> _u;
        private List<double> _uPixel; 
        private List<double> _v;
        private List<double> _vPixel;
        private List<ushort> _a;
        private List<int> _at;
        private List<int> _bt;
        private List<int> _ct; 
        private List<ushort> _b;
        private List<ushort> _c;   

        private readonly int _height;
        private readonly int _width;

        private readonly byte[] _start = {0x01, 0x00, 0x01, 0x00};
        private byte[] _verticesCount;
        private byte[] _trianglesCount;
        private List<byte[]> _vertices;
        private List<byte[]> _polygon;
        private int[] _page;

        private byte R;
        private byte G;
        private byte B;
        private byte NormalMode;
        private const byte Inverted = 0x2C;
        private const byte Non_Inverted = 0x24;
        

        private readonly Dictionary<decimal, byte[]> _clut = new Dictionary<decimal, byte[]>
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
            Text = segment.ToString();
            _height = height;
            _width = width;
            listBox1.SelectedIndex = 0;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = "Wavefront OBJ file .obj|*.obj"};
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _file = File.ReadAllLines(ofd.FileName);
            richTextBox1.Clear();
            richTextBox1.AppendText("Opened: " + ofd.FileName);
            Process();
        }

        private void Process()
        {
            R = (byte)numericUpDown2.Value;
            G = (byte)numericUpDown2.Value;
            B = (byte)numericUpDown2.Value;
            NormalMode = listBox1.SelectedIndex == 0 ? Non_Inverted : Inverted;

            _x = new List<float>(); _y = new List<float>(); _z = new List<float>();
            _u = new List<double>(); _v = new List<double>();
            _uPixel = new List<double>(); _vPixel = new List<double>();
            _a = new List<ushort>(); _b = new List<ushort>(); _c = new List<ushort>();
            _at = new List<int>(); _bt = new List<int>(); _ct = new List<int>();
            foreach (string s in _file)
            {
                if (s.StartsWith("v "))
                {
                    string[] temp = s.Replace(".", ",").Trim().Split(' ');
                    int passSpace = 0;
                    for(int i = temp.Length-1;i>0;i--)
                        if (temp[i] == "")
                            passSpace++;
                    _x.Add(float.Parse(temp[1+passSpace])); _y.Add(float.Parse(temp[2+passSpace]));
                    _z.Add(float.Parse(temp[3+passSpace]));
                }
                else if (s.StartsWith("vt "))
                {
                    string[] temp = s.Replace(".", ",").Trim().Split(' ');
                    int passSpace = 0;
                    for (int i = temp.Length - 1; i > 0; i--)
                        if (temp[i] == "")
                            passSpace++;
                    _u.Add(double.Parse(temp[1+passSpace])); _v.Add(double.Parse(temp[2+passSpace]));
                }
                else if (s.StartsWith("f "))
                {
                    string[] temp = s.Trim().Split(' '); // F[0] 1/1[1] 2/2[2] 3/3[3] 4/4[UNUSED]
                    int passSpace = 0;
                    for (int i = temp.Length - 1; i > 0; i--)
                        if (temp[i] == "")
                            passSpace++;
                    string[] aa = temp[1+passSpace].Split('/'); // A/T
                    _a.Add(ushort.Parse(aa[0]));
                    _at.Add(int.Parse(aa[1]));
                    string[] bb = temp[2+passSpace].Split('/'); // B/T
                    _b.Add(ushort.Parse(bb[0]));
                    _bt.Add(int.Parse(bb[1]));
                    string[] cc = temp[3+passSpace].Split('/'); // C/T
                    _c.Add(ushort.Parse(cc[0]));
                    _ct.Add(int.Parse(cc[1]));
                }
            }
            _vertices = new List<byte[]>();
            ushort vertCount = Convert.ToUInt16(_x.Count);
            _verticesCount = new byte[2];
            _verticesCount = BitConverter.GetBytes(vertCount);
            richTextBox1.AppendText(Environment.NewLine + "Vertices: " + vertCount);
            richTextBox1.AppendText(Environment.NewLine + "VT: " + _u.Count);
            richTextBox1.AppendText(Environment.NewLine + "Triangles: " + _c.Count);

            for (int i = 0; i != _x.Count; i++)
            {
                _x[i] = _x[i] * 2000.0f; _y[i] = _y[i] * 2000.0f;
                _z[i] = _z[i] * 2000.0f;
                short xs = 0;
                short ys = 0;
                short zs = 0;
                try
                {
                    double d = Math.Round(_x[i]);
                    xs = short.Parse(d.ToString(CultureInfo.InvariantCulture));
                    d = Math.Round(_y[i]);
                    ys = short.Parse(d.ToString(CultureInfo.InvariantCulture));
                    d = Math.Round(_z[i]);
                    zs = short.Parse(d.ToString(CultureInfo.InvariantCulture));
                }
                catch (Exception e)
                {
                    MessageBox.Show(
                        "Converting float to short variables failed!\nProbably your model is too big!\nThe meshes are shrinked 2000x times after exporting and then rescaled 2000x up again\nTry to import original exported mesh and test the scales!\nThe biggest X Y or Z coordinate multiplied by 2000 shouldn't be bigger than 65535!");
                    return;
                }
                byte[] vertex = new byte[6];
                Buffer.BlockCopy(BitConverter.GetBytes(xs), 0, vertex, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(ys), 0, vertex, 2, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(zs), 0, vertex, 4, 2);
                _vertices.Add(vertex);
            }

            richTextBox1.AppendText(Environment.NewLine + "Vertices calculated and converted to FF8 format");
            _page = new int[_u.Count];
            for (int i = 0; i != _u.Count; i++)
            {
                _uPixel.Add(Math.Round(((_u[i]*100.0d)*_height)/100.0d));
                _vPixel.Add(Math.Round(((_v[i]*100.0d)*_width)/100.0d));
                
                while (true)
                {
                    if (_uPixel[i] <= 128.0d)
                        break;

                    _uPixel[i] -= 128.0d;
                    _page[i]++; 
                }
            }
            richTextBox1.AppendText(Environment.NewLine + "TPaging calculated and parsed");
            richTextBox1.AppendText(Environment.NewLine + "Mapping was calculated and parsed");
            _polygon = new List<byte[]>();
            ushort tempTrianglesCount = Convert.ToUInt16(_a.Count);
            _trianglesCount = BitConverter.GetBytes(tempTrianglesCount);
            const byte pagePreOperand = 0xB0;

            //UVs needs tweaking
            byte[] u = new byte[_u.Count];
            byte[] v = new byte[_v.Count];

            for (int i = 0; i < u.Length; i++)
                u[i] = (byte) (255*_u[i]);

            for (int i = 0; i < v.Length; i++)
                v[i] = (byte)(255 * _v[i]);

            for (int i = 0; i != tempTrianglesCount; i++)
            {
                byte[] triangle = new byte[20];
                Buffer.BlockCopy(BitConverter.GetBytes(_b[i]-1),0,triangle,0,2); //A
                Buffer.BlockCopy(BitConverter.GetBytes(_c[i]-1), 0, triangle, 2, 2); //B
                Buffer.BlockCopy(BitConverter.GetBytes(_a[i]-1), 0, triangle, 4, 2); //C
                Buffer.BlockCopy(BitConverter.GetBytes(u[_at[i]-1]),0,triangle,6,1); //U1
                Buffer.BlockCopy(BitConverter.GetBytes(v[_at[i] - 1]), 0, triangle, 7, 1); //V1
                Buffer.BlockCopy(BitConverter.GetBytes(u[_bt[i] - 1]), 0, triangle, 8, 1); //U2
                Buffer.BlockCopy(BitConverter.GetBytes(v[_bt[i] - 1]), 0, triangle, 9, 1); //V2
                Buffer.BlockCopy(_clut[numericUpDown1.Value],0,triangle,10,2); //CLUTid
                Buffer.BlockCopy(BitConverter.GetBytes(u[_ct[i] - 1]), 0, triangle, 12, 1); //U3
                Buffer.BlockCopy(BitConverter.GetBytes(v[_ct[i] - 1]), 0, triangle, 13, 1); //V3
                byte[] tempTp = BitConverter.GetBytes(_page[_at[i] - 1]);
                var a = tempTp[0] | pagePreOperand; //BITwise 0xB0 OR 0x0? = 0xB?
                triangle[14] = Convert.ToByte(a & 0xFF); // TPage Bitwised with 0xB0 (UNKNOWN)
                //Buffer.BlockCopy(ARRAY[here], 0, triangle, 14, 1); //TPAGE !!!!
                //PASS bHide = 0, and triangle[15] is NULL (00);
                triangle[16] = R; triangle[17] = G; triangle[18] = B;
                triangle[19] = NormalMode; //PSOne GPU  0x2C and/or 0x24
                _polygon.Add(triangle);
            }
            richTextBox1.AppendText(Environment.NewLine + "Triangle data forged succesfully");
            richTextBox1.AppendText(Environment.NewLine + "Preparing to save segment...");
            SaveFileDialog sfd = new SaveFileDialog {Filter = "Segment file used to replace *.xBIN|*.xBIN"};
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                if(File.Exists(sfd.FileName))
                    File.Delete(sfd.FileName);
                FileStream fs = new FileStream(sfd.FileName, FileMode.Append);
                fs.Write(_start, 0, _start.Length);
                fs.Write(_verticesCount, 0, _verticesCount.Length);
                foreach (byte[] b in _vertices)
                    fs.Write(b, 0, b.Length);
                fs.Write(new byte[2], 0, 2);
                byte[] padding = CalculatePadding((int)fs.Length);
                fs.Write(padding, 0, padding.Length);
                fs.Write(_trianglesCount, 0, 2);
                fs.Write(new byte[6], 0, 6);
                foreach (var variable in _polygon)
                    fs.Write(variable, 0, 20);
                richTextBox1.AppendText("Succesfully saved " + fs.Length + " bytes.");
                fs.Close();
            }
            else
                richTextBox1.AppendText("Save segment failed");
        }

        private static byte[] CalculatePadding(int globalOffset)
        {
            switch (globalOffset%4)
            {
                case 0:
                    return new byte[0];
                case 1:
                    return new byte[1];
                    
                case 2:
                    return new byte[2];
                    
                case 3:
                    return new byte[3];
                    
                case 4:
                    return new byte[4];
                    
                default:
                    return new byte[2];
            }
        }
    }
}
