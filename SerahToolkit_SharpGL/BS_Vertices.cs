using System;
using System.Globalization;
using System.Windows.Forms;
using System.IO;

namespace SerahToolkit_SharpGL
{
    public partial class BsVertices : Form
    {
        private byte[] _parsedByte;
        private readonly UInt16 _vertices;
        private readonly int _offsetFirst;
        private readonly string _lastPath;
        private int _currentForsave;

        private short _x;
        private short _y;
        private short _z;

        private static SharpGlForm _sl;
        private bool _forceNoProcess = false;


        public BsVertices(string lastPath, int offset, SharpGlForm sl)
        {
            _sl = sl;
            InitializeComponent();
            _lastPath = lastPath;
            var fs = new FileStream(lastPath, FileMode.Open);
            fs.Seek(offset+4, SeekOrigin.Begin);
            Byte[] buffer = new byte[2];
            fs.Read(buffer, 0, 2);
            _vertices = BitConverter.ToUInt16(buffer,0);
            fs.Close();

            _offsetFirst = offset + 6;
            PopulateList();


            textBox1.TextChanged += TextBox1_TextChanged;
            textBox2.TextChanged += TextBox2_TextChanged;
            textBox3.TextChanged += TextBox3_TextChanged;
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string st = textBox3.Text.Replace(".", ",");
                if (st.Length == 0)
                    goto ending;
                double d = double.Parse(st); d = d * 2000.0f; d = Math.Round(d);
                _z = short.Parse(d.ToString(CultureInfo.InvariantCulture));
                ParseToHex();
            ending:;
            }
            catch
            {
                // ignored
            }
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string st = textBox2.Text.Replace(".", ",");
                if (st.Length == 0)
                    goto ending;
                double d = double.Parse(st); d = d * 2000.0f; d = Math.Round(d);
                _y = short.Parse(d.ToString(CultureInfo.InvariantCulture));
                ParseToHex();
            ending:;
            }
            catch
            {
                // ignored
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string st = textBox1.Text.Replace(".", ",");
                if (st.Length == 0)
                    goto ending;
                double d = double.Parse(st); d = d * 2000.0f; d = Math.Round(d);
                _x = short.Parse(d.ToString(CultureInfo.InvariantCulture));
                ParseToHex();
            ending:;
            }
            catch
            {
                // ignored
            }
        }

        private void ParseToHex()
        {
            _parsedByte = new byte[6];
            Buffer.BlockCopy(BitConverter.GetBytes(_x), 0, _parsedByte, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(_y), 0, _parsedByte, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(_z), 0, _parsedByte, 4, 2);
            UpdateHex(_parsedByte);
            var fs = new FileStream(_lastPath, FileMode.Open);
            fs.Seek(_currentForsave, SeekOrigin.Begin);
            fs.Write(_parsedByte, 0, 6);
            fs.Close();
            if (checkBox1.Checked && !_forceNoProcess)
            {
                _sl.BSVertEditor_Update(_lastPath);
                //_sl.ForceRendererUpdate();
            }
        }

        private void UpdateHex(byte[] parsedbyte)
        {
            textBox4.Text = BitConverter.ToString(parsedbyte).Replace("-"," ");
        }

        private void PopulateList()
        {
            listBox1.Items.Clear();
            for(int i= 0; i!=_vertices; i++)
            {
                listBox1.Items.Add(i);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateXyz(_lastPath, int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString()));
        }

        private void CalculateXyz(string path, int which)
        {
            _forceNoProcess = true;
            int current = _offsetFirst + (which * 6);
            _currentForsave = current;

            var fs = new FileStream(path, FileMode.Open);
            fs.Seek(current, SeekOrigin.Begin);
            byte[] tempRead = new byte[6];
            fs.Read(tempRead, 0, 6);
            fs.Close();
            textBox1.Text = (BitConverter.ToInt16(tempRead, 0)/2000.0f).ToString(CultureInfo.InvariantCulture);
            textBox2.Text = (BitConverter.ToInt16(tempRead, 2) / 2000.0f).ToString(CultureInfo.InvariantCulture);
            textBox3.Text = (BitConverter.ToInt16(tempRead, 4) / 2000.0f).ToString(CultureInfo.InvariantCulture);
            _forceNoProcess = false;
            ParseToHex();
        }
    }
}
