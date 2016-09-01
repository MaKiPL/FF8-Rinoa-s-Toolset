using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SerahToolkit_SharpGL
{
    class wm2field
    {
        private string path;
        private byte[] buffer;
        private const int ENTRIES = 72;
        private const int ENTRY_SIZE = 24;


        public struct wm2f
        {
            public short FieldX; //12 bit
            public short FieldY; //12 bit
            public short FieldZ; //12 bit
            public byte Unknown; //4 bit
            public ushort FieldID; //16 bit
            public byte[] Unknown2; //17 BYTES
        }

        public wm2field(string path)
        {
            this.path = path;
            buffer = File.ReadAllBytes(path);
        }

        public wm2f ReadEntry(uint index)
        {
            wm2f _wm2f = new wm2f();
            uint localIndex = index*ENTRY_SIZE;
            short preFieldX = BitConverter.ToInt16(buffer, (int)localIndex); //why anyway bitconverter accepts int instead of uint? Index is never negative...
            _wm2f.FieldX = (short)(preFieldX & 0xFFF); //NO...

            return _wm2f;
        }



    }
}
