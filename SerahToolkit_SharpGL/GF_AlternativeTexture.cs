using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using SerahToolkit_SharpGL.FF8_Core;

namespace SerahToolkit_SharpGL
{
    /*
     * Handles the alternative GF texture
     * 
     * Getting real image:
     * offset = 0x14 - pointer to texture
pointerToTextureREAL = offset + UNKNOWN * 4
RealTexture = offset + pointerToTextureREAL

        How to draw it? x42 SKEW + 2       8 8 8    R G B (FAKE!)
*/

    class GF_AlternativeTexture
    {
        public static bool bDebug = false;

        private string path;
        private FileStream fs;
        private BinaryReader br;
        private byte[] palette;
        public AltTexture tex;
        private Palette[] pallete;

        internal struct AltTexture
        {
            public uint _0x08_TextureLimiter;
            public uint _0x14_TexturePointer;
            public uint[] TexturePointers;
        }

        internal struct Palette
        {
            public byte R;
            public byte G;
            public byte B;
        }

        internal enum ResolutionModes //trick workaround, to research
        {
            Default = 0,
            _32 = 1,
            _48 = 2,
            _64 = 3,
            _128 = 4,
            _256 = 5
        }

        internal void Debug_DumpPalette()
        {
            Console.WriteLine("=DEBUG=DDD: Debug_TestPaletteGFAlt==debbbug");
            Bitmap bmp = new Bitmap(16,16,PixelFormat.Format24bppRgb);
            int intt = 0;
            for(int i= 0; i!=16; i++)
                for (int k = 0; k != 16; k++)
                {
                    bmp.SetPixel(i, k, Color.FromArgb(pallete[intt].R, pallete[intt].G, pallete[intt].B));
                    intt++;
                }
            bmp.Save("D:\\test.bmp",ImageFormat.Bmp);
        }

        public GF_AlternativeTexture(string path)
        {
            this.path = path;
            tex = new AltTexture();
            OpenFile();
        }

        internal bool Valid()
        {
            if (fs.Length < 256)
                return false;
            fs.Seek(0x08, SeekOrigin.Begin);
            tex._0x08_TextureLimiter = br.ReadUInt32();
            fs.Seek(0x14, SeekOrigin.Begin);
            tex._0x14_TexturePointer = br.ReadUInt32();
            if (tex._0x08_TextureLimiter <= tex._0x14_TexturePointer)
                return false;
            if ((tex._0x08_TextureLimiter - tex._0x14_TexturePointer)%4 != 0) //limit-point MOD 4, because pointer scale
                return false;
            if (tex._0x14_TexturePointer > fs.Length || tex._0x08_TextureLimiter > fs.Length)
                return false;
            if (tex._0x14_TexturePointer > 0xFFFF || tex._0x08_TextureLimiter > 0xFFFF)
                return false;
            List<uint> Offsets = new List<uint>();
            fs.Seek(tex._0x14_TexturePointer, SeekOrigin.Begin); //ecx, [eax+14h]  /  ADD EAX, ECX
            while (true)
            {
                if (fs.Position >= tex._0x08_TextureLimiter)
                    break;
                uint temp = br.ReadUInt32();
                if(temp != 0x00)
                    Offsets.Add(temp+tex._0x14_TexturePointer);
            }
            if (Offsets.Count < 1)
                return false;

            tex.TexturePointers = Offsets.ToArray();
            return true; //Looks like we passed every logical test! :)
        }

        internal void OpenFile()
        {
            fs = new FileStream(path, FileMode.Open,FileAccess.Read);
            br = new BinaryReader(fs);
        }

        internal Bitmap DrawTexture(uint i, bool bLast, int ResolutionMode = 0)
        {
            //uint offset = tex.TexturePointers[i];
            int next = 0;
            uint sizeToFetch;
            if (!bLast)
            {
                while (true)
                    if (i == tex.TexturePointers[next++]) //gets index of value in array
                        break;
                //from current in tex.TexturePointers where current == i select 
                sizeToFetch = tex.TexturePointers[next] - i; 

            }
            else
                sizeToFetch = (uint) (fs.Length - i) - 512;

            //Read Palette
            //fs.Seek(i, SeekOrigin.Begin); //beginning of image
            //fs.Seek(sizeToFetch, SeekOrigin.Current); //palette start
            pallete = new Palette[256]; //BGRA 15+1
            fs.Seek(-512, SeekOrigin.End);
            for (int k = 0; k != 255; k++)
            {
                ushort colorbuffer = br.ReadUInt16();
                //Console.WriteLine($"k: {k}\tR:{ (colorbuffer >> 10) & 0x1F }\tG:{ (colorbuffer >> 5) & 0x1F }\tR:{ (colorbuffer >> 0) & 0x1F }\tA:{(colorbuffer >> 15 & 1)  } ");
                //Console.WriteLine($"K:{k}\t{Convert.ToString(colorbuffer,2)}\t\t{colorbuffer.ToString("X8")}");
                pallete[k].R = (byte)((byte)((colorbuffer >> 10) & 0x1F) * TIM._5bitColor);
                pallete[k].G = (byte)((byte)((colorbuffer >> 5) & 0x1F) * TIM._5bitColor);
                pallete[k].B = (byte)((byte)(colorbuffer & 0x1F) * TIM._5bitColor);
            }

            fs.Seek(i, SeekOrigin.Begin); //beginning of image
            //sizeToFetch -= 512;
            int width = 0;
            int height = 0;
            if (sizeToFetch >= 0x2000)
                width = 32;
            if (sizeToFetch >= 0x4000)
                width = 64;
            if (sizeToFetch >= 0x8000)
                width = 128;
            if (sizeToFetch >= 0x10000)
                width = 256;
            height = width;


            /* Eh.... 
             * 
            if (ResolutionMode == (int) ResolutionModes.Default)
                width = TestResolutions((int) Math.Ceiling(Math.Sqrt(sizeToFetch)));
            else
            switch (ResolutionMode)
            {
                case (int)ResolutionModes._32:
                    width = 32;
                    break;
                case (int)ResolutionModes._48:
                    width = 48;
                    break;
                case (int)ResolutionModes._64:
                    width = 64;
                    break;
                case (int)ResolutionModes._128:
                    width = 128;
                    break;
                case (int)ResolutionModes._256:
                    width = 256;
                    break;
                default:
                    goto case (int) ResolutionModes._64;
            }
            if (ResolutionMode == (int) ResolutionModes.Default)
            {
                uint realBytes = (uint) (width*3); //fake stride
                if (realBytes*32 > sizeToFetch)
                    height = 32;
                if (realBytes*48 > sizeToFetch)
                    height = 48;
                if (realBytes*64 > sizeToFetch)
                    height = 64;
                if (realBytes*128 > sizeToFetch)
                    height = 128;
                if (realBytes*256 > sizeToFetch)
                    height = 256;
                if (realBytes*512 > sizeToFetch)
                    height = 512;
            }
            else height = width; //1:1
            if (height*width > sizeToFetch)
                while (height*width < sizeToFetch)
                    height /= 2;
            else
                while (height * width < sizeToFetch)
                    height *= 2;
                    */
            Bitmap bmp = new Bitmap(width,height, PixelFormat.Format24bppRgb);
            Rectangle rect = new Rectangle(0,0,bmp.Width,bmp.Height);
            BitmapData bmpdata = bmp.LockBits(rect, ImageLockMode.WriteOnly, PixelFormat.Format24bppRgb);
            byte[] buffer = new byte[bmpdata.Stride*bmpdata.Height];
            Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);
            uint m = 0;
            for (int n = 0; n < width*height - 1; n++)
            {

                byte color = br.ReadByte();
                buffer[m] = color;
                buffer[m + 1] = color;
                buffer[m + 2] = color;
                /* No palette !
                buffer[m] = pallete[color].R;
                buffer[m+1] = pallete[color].G;
                buffer[m+2] = pallete[color].B;*/
                m += 3;
            }
            Marshal.Copy(buffer,0,bmpdata.Scan0, buffer.Length);
            bmp.UnlockBits(bmpdata);

            return bmp;
        }

        private int TestResolutions(int width)
        {
            if (width - 48 <= 0)
                return 1.0f-(float)(48 - width) / (48 - 32) > 0.5f ? 48 : 32;
            if (width - 64 <= 0)
                return 1.0f-(float)(64-width) / (64-48) > 0.5f ? 64 : 48;
            if (width - 128 <= 0)
                return 1.0f-(float)(128 - width) / (128 - 64) > 0.5f ? 128 : 64;
            if (width - 256 <= 0)
                return 1.0f - (float)(256 - width) / (256 - 128) > 0.5f ? 256 : 128;
            return 64; //error handling
        }

        internal void CloseAll()
        {
            br.Close();
            fs.Close();
        }

        public Bitmap DrawMODE1Texture()
        {
            OpenFile();
            int resolution;
            if (fs.Length < 0x1000)
            {
                Console.WriteLine("GfAlt: WriteSingleTex - error, file smaller than 0x1000. Abnormal size!");
                return null;
            }
            switch (fs.Length)
            {
                case 0x4000:
                    resolution = 64;
                    break;
                case 0x8000:
                    resolution = 128;
                    break;
                case 0x10000:
                    resolution = 256;
                    break;
                case 0x2000:
                    resolution = 32;
                    break;
                default:
                    goto case 0x4000;
            }
            Bitmap bmp = new Bitmap(resolution,resolution,PixelFormat.Format24bppRgb);
            BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                PixelFormat.Format24bppRgb);
            byte[] buffer = new byte[bmpdata.Stride*bmpdata.Height];
            Marshal.Copy(bmpdata.Scan0, buffer, 0, buffer.Length);            
            for (int i = 0; i < buffer.Length; i += 3)
            {
                byte color = br.ReadByte();
                buffer[i] = color;
                buffer[1 + i] = color;
                buffer[i + 2] = color;
            }
            Marshal.Copy(buffer,0,bmpdata.Scan0,buffer.Length);
            bmp.UnlockBits(bmpdata);
            return bmp;
        }
    }
}
