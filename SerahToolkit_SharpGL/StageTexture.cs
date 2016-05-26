using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace SerahToolkit_SharpGL
{
    class StageTexture
    {
        private int index;
        private int width;
        private int height;

        private Bitmap bmp;

        private Byte[] textureBuffer;
        private byte[] palBuffer;

        private ColorPalette cp;

        private const int Color_MaxBit = 31;
        private const int Color_color = 255;
        private const float ColorReal = 8.2580645129032f;

        public StageTexture(int index, int width, int height)
        {
            this.index = index;
            this.width = width;
            this.height = height;
        }

        public StageTexture(bool debug)
        {
            if(debug)
            {
                this.index = 36948;
                this.width = 768;
                this.height = 256;
            }
        }

        //Only 8BPP this time!
        //width*4 dla 4BPP
        //width*2 dla 8BPP
        //width*1 dla 16BPP
        //width*1,5 dla 24BPP

        public void CopyTextureBuffer(byte[] buffer)
        {
            textureBuffer = buffer;
            CreateTexture();
        }

        public void CreatePalettedTEX(int CLUTid, byte[] buffer)
        {
            palBuffer = buffer;
            Bitmap bmpPalette = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            cp = bmpPalette.Palette;
            UInt16 CLUTs = BitConverter.ToUInt16(palBuffer, 18);
            if (CLUTid > CLUTs)
                throw new Exception("Given clut is bigger than data!");
            else
            {
                int startBuffer = 20+ (CLUTid*512);
                for(int i = 0; i!= 255; i++)
                {
                    byte[] CLUTcolor = new byte[2];
                    Buffer.BlockCopy(palBuffer, startBuffer, CLUTcolor, 0, 2);
                    BitArray ba = new BitArray(CLUTcolor);

                    BitArray B = new BitArray(5);
                    BitArray R = new BitArray(5);
                    BitArray G = new BitArray(5);
                    BitArray A = new BitArray(1);
                    B[0] = ba[10]; B[1] = ba[11]; B[2] = ba[12]; B[3] = ba[13]; B[4] = ba[14]; //R
                    R[0] = ba[0]; R[1] = ba[1]; R[2] = ba[2]; R[3] = ba[3]; R[4] = ba[4]; //G
                    G[0] = ba[5]; G[1] = ba[6]; G[2] = ba[7]; G[3] = ba[8]; G[4] = ba[9]; //B
                    A[0] = ba[15]; //Alpha if 0
                    int[] b_ = new int[1]; B.CopyTo(b_,0);  //b_[0] = (b_[0] * Color_color) / Color_MaxBit;
                    int[] r_ = new int[1]; R.CopyTo(r_, 0); //r_[0] = (r_[0] * Color_color) / Color_MaxBit;
                    int[] g_ = new int[1]; G.CopyTo(g_, 0); //g_[0] = (g_[0] * Color_color) / Color_MaxBit;
                    int[] a_ = new int[1]; A.CopyTo(a_, 0);

                    int alpha = 0;
                    alpha = a_[0] == 0 ? 0 : 255;

                    double b = Math.Round(b_[0] * ColorReal);
                    double r = Math.Round(r_[0] * ColorReal);
                    double g = Math.Round(g_[0] * ColorReal);

                    if (b > 255)
                        b--;
                    if (r > 255)
                        r--;
                    if (g > 255)
                        g--;


                    cp.Entries[i] = Color.FromArgb(alpha,int.Parse(b.ToString()),int.Parse(g.ToString()),int.Parse(r.ToString()));
                    startBuffer += 2;
                }
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

            bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmp.Palette = cp;
            Rectangle rect = new Rectangle(0,0, bmp.Width, bmp.Height);
            BitmapData bmpData = bmp.LockBits(rect, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
            IntPtr bmPtr = bmpData.Scan0;
            byte[] rawBytes = new byte[bmpData.Stride * bmpData.Height];
            Marshal.Copy(bmPtr,rawBytes,0,rawBytes.Length);
            int index = 0;
            for (int i = 0; i < rawBytes.Length-4; i+=4)
            {
                byte R = cp.Entries[textureBuffer[index + 2]].B; byte G = cp.Entries[textureBuffer[index + 1]].G; byte B = cp.Entries[textureBuffer[index]].R;
                rawBytes[i] = B; rawBytes[i+1] = G; rawBytes[i+2] = R;
                rawBytes[i + 3] = 255;

                if (index < width * height - 4)
                    index++;
            }

            Marshal.Copy(rawBytes, 0, bmPtr, rawBytes.Length);
            bmp.UnlockBits(bmpData);

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

            bmp.MakeTransparent(Color.Black);

        }
        
        public Bitmap GetBMP()
        {
            return bmp;
        }
    }
}
