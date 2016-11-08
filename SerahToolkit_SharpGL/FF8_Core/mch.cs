using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class mch
    {
        public struct MCH
        {
            public uint[] textureOffsets;
            public uint modelOffset;
            public byte[] data;
        }

        private MCH[] MCHarray;

        public void ReadMCH(FileStream fs, BinaryReader br)
        {
            
        }

        public MCH[] ReturnMCHarray()
        {
            return MCHarray;
        }
    }
}
