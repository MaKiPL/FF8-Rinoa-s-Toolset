using System;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.RailEditor
{
    public partial class RailEditor : Form
    {
        private byte[] rail;
        private int rot;
        private string path;
        private const int _entrySize = 2048;
        private const int _animationFrameSize = 16;
        private bool bInitializing;
        private bool bUpdatingData;

        private byte _animationFrames;
        private byte _trainStopOne;
        private byte _trainStopTwo;
        private int _effectiveOffset;


        public RailEditor(byte[] rail, int rot, string path)
        {
            this.rail = rail;
            this.rot = rot;
            this.path = path;
            InitializeComponent();
            this.Text = $"Rail editor- Track: {rot.ToString()}";
            Initialize();
        }

        private void Initialize()
        {
            _animationFrames = rail[rot];

            if(_animationFrames > 127)
            {
                Console.WriteLine("RailEditor: Current track has more than 127 frames. Something's wrong... Exiting");
                this.Close();
            }

            _trainStopOne = rail[rot + 4];
            _trainStopTwo = rail[rot + 8];
            listBox1.Items.Clear();

            for (byte i = 0; i< _animationFrames; i++)
                listBox1.Items.Add($"Frame {i}");

            textBox1.Text = _animationFrames.ToString();
            numericUpDown1.Maximum = _animationFrames;
            numericUpDown2.Maximum = _animationFrames;
            numericUpDown1.Value = _trainStopOne;
            numericUpDown2.Value = _trainStopTwo;
            XnumericUpDown3.Maximum = Int32.MaxValue;
            ZnumericUpDown6.Maximum = Int32.MaxValue;
            YnumericUpDown4.Maximum = Int32.MaxValue;
            UnumericUpDown5.Maximum = Int32.MaxValue;
            XnumericUpDown3.Minimum = Int32.MinValue;
            ZnumericUpDown6.Minimum = Int32.MinValue;
            YnumericUpDown4.Minimum = Int32.MinValue;
            UnumericUpDown5.Minimum = Int32.MinValue;
            bInitializing = false;
            listBox1.SelectedIndex = 0;
        }

        private void UpdateAxis(int value, int effectiveOffset)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Copy(buffer, 0, rail, effectiveOffset, 4);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {

        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (bInitializing)
                return;
            bUpdatingData = true;
            _effectiveOffset = rot+(listBox1.SelectedIndex * _animationFrameSize)+12;
            XnumericUpDown3.Value = BitConverter.ToInt32(rail,_effectiveOffset);
            ZnumericUpDown6.Value = BitConverter.ToInt32(rail, _effectiveOffset+4);
            YnumericUpDown4.Value = BitConverter.ToInt32(rail, _effectiveOffset+8);
            UnumericUpDown5.Value = BitConverter.ToInt32(rail, _effectiveOffset+12);
            bUpdatingData = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if(!bInitializing)
                rail[rot + 4] = (byte)((int)numericUpDown1.Value & 0xFF);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (!bInitializing)
                rail[rot + 8] = (byte)((int)numericUpDown2.Value & 0xFF);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure?\nThe file would be overwritten!", "Caution!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                System.IO.File.WriteAllBytes(path, rail);
                MessageBox.Show("File saved!");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
                XnumericUpDown3.Hexadecimal = checkBox1.Checked;
                YnumericUpDown4.Hexadecimal = checkBox1.Checked;
                ZnumericUpDown6.Hexadecimal = checkBox1.Checked;
                UnumericUpDown5.Hexadecimal = checkBox1.Checked;
        }

        private void XnumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (bUpdatingData)
                return;

            UpdateAxis((int)XnumericUpDown3.Value, _effectiveOffset);
        }

        private void ZnumericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (bUpdatingData)
                return;

            UpdateAxis((int)ZnumericUpDown6.Value, _effectiveOffset+4);
        }

        private void YnumericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (bUpdatingData)
                return;

            UpdateAxis((int)YnumericUpDown4.Value, _effectiveOffset+8);
        }

        private void UnumericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (bUpdatingData)
                return;

            UpdateAxis((int)UnumericUpDown5.Value, _effectiveOffset+12);
        }

        private void pictureBox1_Click_1(object sender, EventArgs e)
        {
            if(_animationFrames >= 126)
            {
                MessageBox.Show("Maximum animation frames exceeded!");
                return;
            }

            listBox1.Items.Add($"Frame {_animationFrames++}");
            rail[rot] = _animationFrames;
            textBox1.Text = _animationFrames.ToString();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (_animationFrames <= 4)
            {
                MessageBox.Show("Too few animation frames! Can't remove another!");
                return;
            }
            bInitializing = true;
            int bufferLength = 2048 - 12 - (listBox1.SelectedIndex+1 * _animationFrameSize);
            byte[] buffer = new byte[2048];
            Array.Copy(rail, _effectiveOffset + 16, buffer, 0, bufferLength);
            Array.Copy(buffer, 0, rail, _effectiveOffset, bufferLength);
            _animationFrames--;
            listBox1.Items.Clear();
            for (byte i = 0; i < _animationFrames; i++)
                listBox1.Items.Add($"Frame {i}");
            bInitializing = false;


        }
    }
}
