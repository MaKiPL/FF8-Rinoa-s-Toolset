using System;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.RailEditor
{
    internal partial class RailEditor : Form
    {
        private byte[] rail;
        private int rot;
        private string path;
        private const int AnimationFrameSize = 16;
        private bool _bInitializing;
        private bool _bUpdatingData;

        private byte _animationFrames;
        private byte _trainStopOne;
        private byte _trainStopTwo;
        private int _effectiveOffset;

        internal RailEditor(byte[] rail, int rot, string path)
        {
            this.rail = rail;
            this.rot = rot;
            this.path = path;
            InitializeComponent();
            Text = $"Rail editor- Track: {rot.ToString()}";
            Initialize();
        }

        private void Initialize()
        {
            _animationFrames = rail[rot];

            if(_animationFrames > 127)
            {
                Console.WriteLine("RailEditor: Current track has more than 127 frames. Something's wrong... Exiting");
                Close();
            }
            Console.WriteLine("RailEd: Preinitializing track...");
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
            XnumericUpDown3.Maximum = int.MaxValue;
            ZnumericUpDown6.Maximum = int.MaxValue;
            YnumericUpDown4.Maximum = int.MaxValue;
            UnumericUpDown5.Maximum = int.MaxValue;
            XnumericUpDown3.Minimum = int.MinValue;
            ZnumericUpDown6.Minimum = int.MinValue;
            YnumericUpDown4.Minimum = int.MinValue;
            UnumericUpDown5.Minimum = int.MinValue;
            _bInitializing = false;
            listBox1.SelectedIndex = 0;
            Console.WriteLine("RailEd: Ready!");
        }

        private void UpdateAxis(int value, int effectiveOffset)
        {
            byte[] buffer = BitConverter.GetBytes(value);
            Array.Copy(buffer, 0, rail, effectiveOffset, 4);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_bInitializing)
                return;
            _bUpdatingData = true;
            _effectiveOffset = rot+(listBox1.SelectedIndex * AnimationFrameSize)+12;
            XnumericUpDown3.Value = BitConverter.ToInt32(rail,_effectiveOffset);
            ZnumericUpDown6.Value = BitConverter.ToInt32(rail, _effectiveOffset+4);
            YnumericUpDown4.Value = BitConverter.ToInt32(rail, _effectiveOffset+8);
            UnumericUpDown5.Value = BitConverter.ToInt32(rail, _effectiveOffset+12);
            _bUpdatingData = false;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            if(!_bInitializing)
                rail[rot + 4] = (byte)((int)numericUpDown1.Value & 0xFF);
        }

        private void numericUpDown2_ValueChanged(object sender, EventArgs e)
        {
            if (!_bInitializing)
                rail[rot + 8] = (byte)((int)numericUpDown2.Value & 0xFF);
        }

        private void pictureBox3_Click(object sender, EventArgs e)
        {
            if(MessageBox.Show("Are you sure?\nThe file would be overwritten!", "Caution!", MessageBoxButtons.YesNo) == DialogResult.Yes)
            {
                if(_animationFrames >= _trainStopOne || _animationFrames >= _trainStopTwo)
                {
                    MessageBox.Show("Please correct the train stop indexes!\nIndex cannot be bigger than amount of frames!");
                    return;
                }
                if(_trainStopOne == _trainStopTwo)
                {
                    MessageBox.Show("Train cannot have two the same stops. Correct it!");
                    return;
                }
                System.IO.File.WriteAllBytes(path, rail);
                MessageBox.Show("File saved!");
            }
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            Console.WriteLine("RailEd: Changing display style...");
                XnumericUpDown3.Hexadecimal = checkBox1.Checked;
                YnumericUpDown4.Hexadecimal = checkBox1.Checked;
                ZnumericUpDown6.Hexadecimal = checkBox1.Checked;
                UnumericUpDown5.Hexadecimal = checkBox1.Checked;
        }

        private void XnumericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            if (_bUpdatingData)
                return;

            UpdateAxis((int)XnumericUpDown3.Value, _effectiveOffset);
        }

        private void ZnumericUpDown6_ValueChanged(object sender, EventArgs e)
        {
            if (_bUpdatingData)
                return;

            UpdateAxis((int)ZnumericUpDown6.Value, _effectiveOffset+4);
        }

        private void YnumericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            if (_bUpdatingData)
                return;

            UpdateAxis((int)YnumericUpDown4.Value, _effectiveOffset+8);
        }

        private void UnumericUpDown5_ValueChanged(object sender, EventArgs e)
        {
            if (_bUpdatingData)
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
            Console.WriteLine("RailEd: Added new keyframe!");
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            if (_animationFrames <= 4)
            {
                MessageBox.Show("Too few keyframes! Can't remove another!");
                return;
            }
            _bInitializing = true;
            int bufferLength = 2048 - 12 - (listBox1.SelectedIndex+1 * AnimationFrameSize);
            byte[] buffer = new byte[2048];
            Array.Copy(rail, _effectiveOffset + 16, buffer, 0, bufferLength);
            Array.Copy(buffer, 0, rail, _effectiveOffset, bufferLength);
            _animationFrames--;
            listBox1.Items.Clear();
            for (byte i = 0; i < _animationFrames; i++)
                listBox1.Items.Add($"Frame {i}");
            _bInitializing = false;
            Console.WriteLine("RailEd: Deleted one keyframe");
        }
    }
}
