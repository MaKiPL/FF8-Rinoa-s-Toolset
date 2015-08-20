using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace SerahToolkit_SharpGL
{
    public partial class BS_Vertices : Form
    {
        private byte[] parsedByte;
        private UInt16 vertices;
        private int offsetFirst;
        private string LastPath;
        private int current_forsave;

        private short x;
        private short y;
        private short z;


        public BS_Vertices(string LastPath, int offset)
        {
            InitializeComponent();
            this.LastPath = LastPath;
            var fs = new FileStream(LastPath, FileMode.Open);
            fs.Seek(offset+4, SeekOrigin.Begin);
            Byte[] buffer = new byte[2];
            fs.Read(buffer, 0, 2);
            vertices = BitConverter.ToUInt16(buffer,0);
            fs.Close();

            offsetFirst = offset + 6;
            PopulateList();


            textBox1.TextChanged += TextBox1_TextChanged;
            textBox2.TextChanged += TextBox2_TextChanged;
            textBox3.TextChanged += TextBox3_TextChanged;
        }

        private void TextBox3_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string st = textBox3.Text;//.Replace(",", ".");
                if (st.Length == 0)
                    goto ending;
                double d = double.Parse(st); d = d * 2000.0f; d = Math.Round(d);
                z = short.Parse(d.ToString());
                ParseToHex();
            ending:;
            }
            catch
            {

            }
            
        }

        private void TextBox2_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string st = textBox2.Text;//.Replace(",", ".");
                if (st.Length == 0)
                    goto ending;
                double d = double.Parse(st); d = d * 2000.0f; d = Math.Round(d);
                y = short.Parse(d.ToString());
                ParseToHex();
            ending:;
            }
            catch
            {

            }
            
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            try
            {
                string st = textBox1.Text;//.Replace(",", ".");
                if (st.Length == 0)
                    goto ending;
                double d = double.Parse(st); d = d * 2000.0f; d = Math.Round(d);
                x = short.Parse(d.ToString());
                ParseToHex();
            ending:;
            }
            catch
            {

            }
            
        }

        private void ParseToHex()
        {
            parsedByte = new byte[6];
            Buffer.BlockCopy(BitConverter.GetBytes(x), 0, parsedByte, 0, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(y), 0, parsedByte, 2, 2);
            Buffer.BlockCopy(BitConverter.GetBytes(z), 0, parsedByte, 4, 2);
            updateHEX(parsedByte);
        }

        private void updateHEX(byte[] parsedbyte)
        {
            textBox4.Text = BitConverter.ToString(parsedbyte).Replace("-"," ");
        }

        private void PopulateList()
        {
            listBox1.Items.Clear();
            for(int i= 0; i!=vertices; i++)
            {
                listBox1.Items.Add(i);
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CalculateXYZ(LastPath, int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString()));
        }

        private void CalculateXYZ(string path, int which)
        {
            int current = offsetFirst + (which * 6);
            current_forsave = current;

            var fs = new FileStream(path, FileMode.Open);
            fs.Seek(current, SeekOrigin.Begin);
            byte[] tempRead = new byte[6];
            fs.Read(tempRead, 0, 6);
            fs.Close();
            textBox1.Text = (BitConverter.ToInt16(tempRead, 0)/2000.0f).ToString();
            textBox2.Text = (BitConverter.ToInt16(tempRead, 2) / 2000.0f).ToString();
            textBox3.Text = (BitConverter.ToInt16(tempRead, 4) / 2000.0f).ToString();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var fs = new FileStream(LastPath, FileMode.Open);
            fs.Seek(current_forsave, SeekOrigin.Begin);
            fs.Write(parsedByte, 0, 6);
            fs.Close();
        }
    }
}
