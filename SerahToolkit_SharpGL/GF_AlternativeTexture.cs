using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

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
        private string path;
        private FileStream fs;
        private BinaryReader br;
        private byte[] palette;
        public AltTexture tex;

        internal struct AltTexture
        {
            public uint _0x08_TextureLimiter;
            public uint _0x14_TexturePointer;
            public uint[] TexturePointers;
        }

        public GF_AlternativeTexture(string path)
        {
            this.path = path;
            tex = new AltTexture();
            OpenFile();
        }

        internal bool Valid()
        {
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

        internal Bitmap DrawTexture(uint i, bool bLast)
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
                sizeToFetch = tex.TexturePointers[next + 1] - i; 

            }
            else
                sizeToFetch = (uint) (fs.Length - i);

            //Read Palette


            return null;
        }

        internal void CloseAll()
        {
            br.Close();
            fs.Close();
        }
    }
}
