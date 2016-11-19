using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using AForge.Imaging.Filters;

namespace SerahToolkit_SharpGL.Forms
{
    public partial class wm35 : Form
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        private Timer timer;

        private WM_Section35 wm;
        private WM_Section35.DrawPointEntry[] dpe;
        private int huetransformator = 10;

        private Rectangle lastRect;

        private Bitmap originalMap;
        private Bitmap Colored;

        public wm35(WM_Section35 wm)
        {
            this.wm = wm;
            InitializeComponent();
            originalMap = new Bitmap(pictureBox3.Image);
            timer = new Timer {Interval = 250};

            UpdateWM();
        }

        private void UpdateWM()
        {
            dpe = wm.GetEntries();
            for (int i = 0; i < dpe.Length; i++)
                dataGridView1.Rows.Add(i + 0x80,dpe[i].X, dpe[i].Y, dpe[i].UNK);
            ColorizeBlock(CalculateRectangle(dpe[0].X, dpe[0].Y));
            timer.Tick += Timer_Tick;
            timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (lastRect == null) return;
            if (huetransformator > 20)
                huetransformator = 0;
            huetransformator += 10;
            ColorizeBlock(lastRect);
        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (dataGridView1.SelectedRows.Count != 0)
                index = dataGridView1.SelectedRows[0].Index;
            if(dataGridView1.SelectedCells[0].RowIndex != 0)
                index = dataGridView1.SelectedCells[0].RowIndex;
            if (index == 0)
            {
                Console.WriteLine("WMSET35: You can't move this row up!");
                return;
            }
            DataGridViewRow uprow = dataGridView1.Rows[index];
            DataGridViewRow upper = dataGridView1.Rows[index - 1];
            dataGridView1.Rows.Remove(upper);
            dataGridView1.Rows.Remove(uprow);
            dataGridView1.Rows.Insert(index-1, uprow);
            dataGridView1.Rows.Insert(index, upper);
            dataGridView1.ClearSelection();
            dataGridView1.Rows[uprow.Index].Selected = true;

            UpdateMagicIDs();
        }

        private void UpdateMagicIDs()
        {
            for (int i = 0; i < dataGridView1.RowCount; i++)
                dataGridView1.Rows[i].Cells[0].Value = i + 0x80;
            dataGridView1.Refresh();
        }

        private void pictureBox2_Click(object sender, EventArgs e)
        {
            int index = 0;
            if (dataGridView1.SelectedRows.Count != 0)
                index = dataGridView1.SelectedRows[0].Index;
            if (dataGridView1.SelectedCells[0].RowIndex != 0)
                index = dataGridView1.SelectedCells[0].RowIndex;
            if (index == dataGridView1.RowCount-1)
            {
                Console.WriteLine("WMSET35: You can't move this row down!");
                return;
            }
            DataGridViewRow uprow = dataGridView1.Rows[index];
            DataGridViewRow upper = dataGridView1.Rows[index + 1];
            dataGridView1.Rows.Remove(upper);
            dataGridView1.Rows.Remove(uprow);
            dataGridView1.Rows.Insert(index, uprow);
            dataGridView1.Rows.Insert(index, upper);
            dataGridView1.ClearSelection();
            dataGridView1.Rows[uprow.Index].Selected = true;

            UpdateMagicIDs();
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            int index = -1;
                    if (dataGridView1.SelectedCells.Count > 0)
                        index = dataGridView1.SelectedCells[0].RowIndex;
                    if(dataGridView1.SelectedRows.Count > 0)
                        index = dataGridView1.SelectedRows[0].Index;
            if (index == -1) return;

            ResetBlockColor(int.Parse(dataGridView1.Rows[index].Cells[1].Value.ToString()),
                int.Parse(dataGridView1.Rows[index].Cells[2].Value.ToString()));
        }

        private Rectangle CalculateRectangle(int x, int y)
        {
            int realX = x*4;
            int realY = y*16;
            return lastRect = new Rectangle(realX,realY, 16,16);
        }

        public unsafe void ResetBlockColor(int x, int y)
        {
            if (x >= 1024 || x+16 >= 1024) return;
            if (y >= 512 || y+16 >= 512) return;
            /*BitmapData original = originalMap.LockBits(CalculateRectangle(x,y), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            BitmapData colored = Colored.LockBits(CalculateRectangle(x,y), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            IntPtr originscan = original.Scan0;
            IntPtr colorscan = colored.Scan0;
            memcpy(colorscan, originscan, new UIntPtr((uint)(colored.Stride * colored.Height)));
            Colored.UnlockBits(colored);
            originalMap.UnlockBits(original);*/
            pictureBox3.Image = originalMap;
            Colored = new Bitmap(originalMap);
            ColorizeBlock(CalculateRectangle(x,y));
        }

        public void ColorizeBlock(Rectangle rect)
        {
            if (Colored == null)
                Colored = new Bitmap(originalMap);
            HueModifier hue = new HueModifier(huetransformator);
            hue.ApplyInPlace(Colored, rect);
            pictureBox3.Image = Colored;
        }

        private void button1_Click(object sender, EventArgs e)
        {
        }
    }
}
