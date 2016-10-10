using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class TEX
    {
        private string path;
        private Texture texture;
        private FileStream fs;
        private BinaryReader br;


        struct Texture //RawImage after paletteData
        {
            public uint Width; //0x3C
            public uint Height; //0x40
            public byte NumOfPalettes; //0x30
            public byte PaletteFlag; //0x4C
            public uint PaletteSize; //0x58
            public byte[] paletteData; //0xEC
        }

        struct Color
        {
            public byte Red;
            public byte Green;
            public byte Blue;
            public byte Alpha;
        }


        public TEX(string path)
        {
            this.path = path;
            texture = new Texture();
            fs = new FileStream(path, FileMode.Open,FileAccess.Read);
            br = new BinaryReader(fs);
            ReadParameters();
        }

        public void ReadParameters()
        {
            fs.Seek(0x3C, SeekOrigin.Begin);
            texture.Width = br.ReadUInt32();
            texture.Height = br.ReadUInt32();
            fs.Seek(0x30, SeekOrigin.Begin);
            texture.NumOfPalettes = (byte)(br.ReadUInt32() & 0xFF);
            fs.Seek(0x4C, SeekOrigin.Begin);
            texture.PaletteFlag = (byte)(br.ReadUInt32() & 0xFF);
            fs.Seek(0x58, SeekOrigin.Begin);
            texture.PaletteSize = br.ReadUInt32();
            fs.Seek(0xEC, SeekOrigin.Begin);
            if(texture.PaletteFlag != 0)
                texture.paletteData = br.ReadBytes((int) texture.PaletteSize*4); //BGRA
            fs.Seek(0xEC + texture.PaletteSize*4, SeekOrigin.Begin);
        }

        public Bitmap GetTexture()
        {
            Color[] colors;
            if (texture.PaletteFlag != 0)
            {
                colors = new Color[texture.paletteData.Length/4];
                int k = 0;
                for (int i = 0; i < texture.paletteData.Length; i+=4)
                {
                    colors[k].Alpha = texture.paletteData[i];
                    colors[k].Red = texture.paletteData[i+1];
                    colors[k].Green = texture.paletteData[i+2];
                    colors[k].Blue = texture.paletteData[i+3];
                    k++;
                }
                Bitmap bmp = new Bitmap((int) texture.Width, (int)texture.Height, PixelFormat.Format32bppArgb);
                BitmapData bmpdata = bmp.LockBits(new Rectangle(0, 0, bmp.Width, bmp.Height), ImageLockMode.WriteOnly,
                    PixelFormat.Format32bppArgb);
                IntPtr scan0 = bmpdata.Scan0;
                byte[] buffer = new byte[bmpdata.Stride * bmpdata.Height];
                Marshal.Copy(scan0, buffer, 0, buffer.Length);

                for (int i = 0; i < buffer.Length; i+=4)
                {
                    byte colorkey = br.ReadByte();
                    buffer[i] = colors[colorkey].Alpha;
                    buffer[i+1] = colors[colorkey].Red;
                    buffer[i+2] = colors[colorkey].Green;
                    buffer[i+3] = colors[colorkey].Blue;
                }
                Marshal.Copy(buffer, 0 , scan0, buffer.Length);
                bmp.UnlockBits(bmpdata);
                return bmp;
            }
            else return null;
        }     
    }
}
