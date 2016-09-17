using System;
using System.Drawing;
using System.IO;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class TIM
    {
        public byte?[] CLUT;
        public byte[] RawImage;

        private static byte[] _8BPP = { 0x01, 0x00, 0x00, 0x00, 0x09};
        private static byte[] _4BPP = { 0x01, 0x00, 0x00, 0x00, 0x08};
        private static byte[] _16BPP = { 0x01, 0x00, 0x00, 0x00, 0x02 };
        private static byte[] _24BPP = { 0x01, 0x00, 0x00, 0x00, 0x03 };
        private FileStream fs;
        private System.IO.BinaryReader br;
        private Texture texture;

        public struct Texture
        {
            public ushort PaletteX;
            public ushort PaletteY;
            public ushort NumOfCluts;
            public byte?[] ClutData;
            public ushort ImageOrgX;
            public ushort ImageOrgY;
            public ushort Width;
            public ushort Height;
        }

        public TIM(string path)
        {
            fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            br = new BinaryReader(fs);
            texture = new Texture();
            sbyte bpp = RecognizeBPP();
            if (bpp == -1)
            {
                Console.WriteLine("TIM: This is not TIM texture!");
                return;
            }
            ReadParameters(bpp);
            Bitmap bmp = DrawTexture();
            br.Dispose();
            fs.Dispose();
        }

        private Bitmap DrawTexture()
        {
            return null;
        }

        private void ReadParameters(sbyte bpp)
        {
            if (bpp == 4)
            {
                fs.Seek(4, SeekOrigin.Current);
                texture.PaletteX = br.ReadUInt16();
                texture.PaletteY = br.ReadUInt16();
                fs.Seek(2, SeekOrigin.Current);
                texture.NumOfCluts = br.ReadUInt16();
                byte?[] buffer = new byte?[texture.NumOfCluts * 32];
                for (int i = 0; i != buffer.Length; i++)
                    buffer[i] = br.ReadByte();
                texture.ClutData = buffer;
                fs.Seek(4, SeekOrigin.Current);
                texture.ImageOrgX = br.ReadUInt16();
                texture.ImageOrgY = br.ReadUInt16();
                texture.Width = (ushort) (br.ReadUInt16()*4);
                texture.Height = br.ReadUInt16();
                return;
            }
            if (bpp == 8)
            {
                fs.Seek(4, SeekOrigin.Current);
                texture.PaletteX = br.ReadUInt16();
                texture.PaletteY = br.ReadUInt16();
                fs.Seek(2, SeekOrigin.Current);
                texture.NumOfCluts = br.ReadUInt16();
                byte?[] buffer = new byte?[texture.NumOfCluts * 512];
                for (int i = 0; i != buffer.Length; i++)
                    buffer[i] = br.ReadByte();
                texture.ClutData = buffer;
                fs.Seek(4, SeekOrigin.Current);
                texture.ImageOrgX = br.ReadUInt16();
                texture.ImageOrgY = br.ReadUInt16();
                texture.Width = (ushort)(br.ReadUInt16() * 2);
                texture.Height = br.ReadUInt16();
                return;
            }
            if (bpp == 16)
            {
                fs.Seek(4, SeekOrigin.Current);
                texture.ImageOrgX = br.ReadUInt16();
                texture.ImageOrgY = br.ReadUInt16();
                texture.Width = br.ReadUInt16();
                texture.Height = br.ReadUInt16();
                return;
            }
            if (bpp != 24) return;
            fs.Seek(4, SeekOrigin.Current);
            texture.ImageOrgX = br.ReadUInt16();
            texture.ImageOrgY = br.ReadUInt16();
            texture.Width = (ushort) (br.ReadUInt16()/ 1.5);
            texture.Height = br.ReadUInt16();
        }

        private sbyte RecognizeBPP()
        {
            byte[] buffer = br.ReadBytes(5);
            fs.Seek(3, SeekOrigin.Current);
            if (buffer.Equals(_4BPP))
                return 4;
            if (buffer.Equals(_8BPP))
                return 8;
            if (buffer.Equals(_16BPP))
                return 16;
            if (buffer.Equals(_24BPP))
                return 24;
            return -1;
            
        }
    }
}
