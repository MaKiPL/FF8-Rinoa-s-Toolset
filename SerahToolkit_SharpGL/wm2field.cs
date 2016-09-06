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
        public const int ENTRIES = 72;
        private const int ENTRY_SIZE = 24;


        public struct wm2f
        {
            public short FieldX; //DX
            public short FieldY; //CX
            public ushort FieldZ; //DX
            public ushort FieldID; //CX
            public byte UnknownPointer; //8th Byte
            public byte[] Unknown2; //dword ptr + null?
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
            //why anyway bitconverter accepts int instead of uint? Index is never negative...
            //I'm also unsure about XYZ axis space... FF8 everywhere else uses X-ZY...
            _wm2f.FieldX = BitConverter.ToInt16(buffer, (int)localIndex); //movsx   ecx, wm2field_FieldX
            _wm2f.FieldY = BitConverter.ToInt16(buffer, (int)localIndex+2); //movsx   ecx, wm2field_FieldY
            _wm2f.FieldZ = BitConverter.ToUInt16(buffer, (int)localIndex+4); //mov     cx, wm2field_FieldZ

            /*int fieldX = preFieldX << 0xC; //shl     ecx, 0Ch
            int fieldY = preFieldX << 0xC; //shl     ecx, 0Ch*/

            _wm2f.FieldID = BitConverter.ToUInt16(buffer, (int)localIndex+6);
            _wm2f.UnknownPointer = buffer[localIndex + 8];
            byte[] unknown2 = new byte[12];
            unknown2[0] = _wm2f.UnknownPointer;
            unknown2[1] = _wm2f.UnknownPointer;
            unknown2[2] = _wm2f.UnknownPointer;
            _wm2f.Unknown2 = unknown2;
            return _wm2f;
        }



    }
}
