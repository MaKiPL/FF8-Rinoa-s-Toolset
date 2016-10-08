using System;
using System.IO;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class TEX
    {
        private string path;
        private Texture texture;


        struct Texture //RawImage after paletteData
        {
            uint Width; //0x3C
            uint Height; //0x40
            byte NumOfPalettes; //0x30
            byte PaletteFlag; //0x4C
            private uint PaletteSize; //0x58
            byte[] paletteData; //0xEC
        }



        public TEX(string path)
        {
            this.path = path;
            Texture texture = new Texture();
        }
    }
}
