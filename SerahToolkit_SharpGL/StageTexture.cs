using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;

namespace SerahToolkit_SharpGL
{
    class StageTexture
    {
        private int _index;
        private readonly int _width;
        private readonly int _height;

        private Bitmap _bmp;

        private Byte[] _textureBuffer;
        private byte[] _palBuffer;

        private ColorPalette _cp;

        private const int ColorMaxBit = 32;
        private const int ColorColor = 256;
        private const float ColorReal = 256 / 32;  //8.2580645129032f;

        public StageTexture(int index, int width, int height)
        {
            _index = index;
            _width = width;
            _height = height;
        }

        //Only 8BPP this time!
        //width*4 dla 4BPP
        //width*2 dla 8BPP
        //width*1 dla 16BPP
        //width*1,5 dla 24BPP

        public void CopyTextureBuffer(byte[] buffer)
        {
            _textureBuffer = buffer;
            CreateTexture();
        }

        public void CreatePalettedTex(int cluTid, byte[] buffer)
        {
            _palBuffer = buffer;
            Bitmap bmpPalette = new Bitmap(_width, _height, PixelFormat.Format8bppIndexed);
            _cp = bmpPalette.Palette;
            UInt16 cluTs = BitConverter.ToUInt16(_palBuffer, 18);
            if (cluTid > cluTs)
                throw new Exception("Given clut is bigger than data!");
            int startBuffer = 20+ (cluTid*512);
            for(int i = 0; i!= 255; i++)
            {
                byte[] cluTcolor = new byte[2];
                Buffer.BlockCopy(_palBuffer, startBuffer, cluTcolor, 0, 2);
                BitArray ba = new BitArray(cluTcolor);

                BitArray B = new BitArray(5);
                BitArray R = new BitArray(5);
                BitArray G = new BitArray(5);
                BitArray a = new BitArray(1);
                B[0] = ba[10]; B[1] = ba[11]; B[2] = ba[12]; B[3] = ba[13]; B[4] = ba[14]; //R
                R[0] = ba[0]; R[1] = ba[1]; R[2] = ba[2]; R[3] = ba[3]; R[4] = ba[4]; //G
                G[0] = ba[5]; G[1] = ba[6]; G[2] = ba[7]; G[3] = ba[8]; G[4] = ba[9]; //B
                a[0] = ba[15]; //Alpha if 0
                int[] b_ = new int[1]; B.CopyTo(b_,0);  //b_[0] = (b_[0] * Color_color) / Color_MaxBit;
                int[] r_ = new int[1]; R.CopyTo(r_, 0); //r_[0] = (r_[0] * Color_color) / Color_MaxBit;
                int[] g_ = new int[1]; G.CopyTo(g_, 0); //g_[0] = (g_[0] * Color_color) / Color_MaxBit;
                int[] aa = new int[1]; a.CopyTo(aa, 0);
                var alpha = aa[0] == 0 ? 0 : 255;

                double b = Math.Round(b_[0] * ColorReal);
                double r = Math.Round(r_[0] * ColorReal);
                double g = Math.Round(g_[0] * ColorReal);

                if (b > 255)
                    b--;
                if (r > 255)
                    r--;
                if (g > 255)
                    g--;


                _cp.Entries[i] = Color.FromArgb(alpha,int.Parse(b.ToString(CultureInfo.InvariantCulture)),int.Parse(g.ToString(CultureInfo.InvariantCulture)),int.Parse(r.ToString(CultureInfo.InvariantCulture)));
                startBuffer += 2;
            }
        }

        private void CreateTexture()
        {
            //GDI+ - up to 300 ms for single texture
            //New bitlock - 19 ms for single texture !!!


            /*
            bmp = new Bitmap(width, height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);
            index = 0;
            int Pixels = width * height;
            for (int y = 0; y < height; y++)
            {
                for(int x = 0; x < width; x++)
                {
                    bmp.SetPixel(x, y, Color.FromArgb(textureBuffer[index], textureBuffer[index + 1], textureBuffer[index + 2]));
                    if(index<width*height-3)
                        index++;

                }
            } */

            _bmp = new Bitmap(_width, _height, PixelFormat.Format32bppArgb) {Palette = _cp};
            Rectangle rect = new Rectangle(0,0, _bmp.Width, _bmp.Height);
            BitmapData bmpData = _bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr bmPtr = bmpData.Scan0;
            byte[] rawBytes = new byte[bmpData.Stride * bmpData.Height];
            Marshal.Copy(bmPtr,rawBytes,0,rawBytes.Length);
            int index = 0;
            for (int i = 0; i < rawBytes.Length-3; i+=4)
            {
                byte r = _cp.Entries[_textureBuffer[index]].R; byte g = _cp.Entries[_textureBuffer[index]].G; byte b = _cp.Entries[_textureBuffer[index]].B;
                rawBytes[i] = r; rawBytes[i+1] = g; rawBytes[i+2] = b;
                rawBytes[i + 3] = 255;

                if (index < _width * _height - 3)
                    index++;
            }

            /*
            byte[] safeimg = rawBytes;

            for (int i = 0; i < rawBytes.Length - 4; i += 4)
            {
                //
            }*/

            Marshal.Copy(rawBytes, 0, bmPtr, rawBytes.Length);
            _bmp.UnlockBits(bmpData);


            /*
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    //Color palette = cp.Entries[index];

                    bmp.SetPixel(x, y, Color.FromArgb(cp.Entries[textureBuffer[index+2]].B,cp.Entries[textureBuffer[index+1]].G,cp.Entries[textureBuffer[index]].R));
                    if (index < width * height - 4)
                        index++;

                }
            }*/
            

            //GrayscaleToRGB gs = new GrayscaleToRGB();
            //gs.Apply(bmp);

            _bmp.MakeTransparent(Color.Black);

        }
        
        public Bitmap GetBmp()
        {
            return _bmp;
        }
    }
}
