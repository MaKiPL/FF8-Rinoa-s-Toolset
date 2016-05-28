using System;
using System.Collections.Generic;
using System.IO;

namespace SerahToolkit_SharpGL
{
    class GfEnviro
    {
        //MODELS ARE UPSIDE DOWN XZY ?
        //Mag005_01 uses other format... 
        //mag201 ??
        //mag205 To research 


        private readonly byte[] _file;
        private uint[] _subOffsets;

        private const int EnviroOffset = 0xC; //12th byte


        //process vars
        private int _objCount;
        private int _relativeJump;
        private const int PassFromStart = 24;
        private int _vertexCount;
        private int _verticesOffset;
        private string _v;
        //private string _vt;
        //private string f;
        private uint _pointer;


        private Dictionary<UInt16,int> _polygonType = new Dictionary<ushort, int>
        {
            { 0x7, 20},     //BAD?
            { 0x8, 20}, //OK 
            { 0x9, 28}, //OK
            {0x10, 20},     //BAD?
            {0x12, 24}, //OK
            {0x13, 36}, //OK
            {0x18, 0x18}    //24 BAD?
        };
         
        


        public GfEnviro(string path)
        {
            _file = File.ReadAllBytes(path);
        }

        public bool BValidHeader()
        {
            //New byte[8] trick doesn't work??
            return (_file[0] == 0x00 && _file[1] == 0x00 && _file[2] == 0x00 && _file[3] == 0x00 && _file[4] == 0x00 && _file[5] == 0x00 && _file[6] == 0x00 && _file[7] == 0x00);
        }

        public int[] PopulateOffsets()
        {
            _pointer = BitConverter.ToUInt32(_file, EnviroOffset);
            UInt32 count = BitConverter.ToUInt32(_file, (int)_pointer);
            _subOffsets = new uint[count];
            for (int i = 0; i != count; i++)
            {
                uint temp = BitConverter.ToUInt32(_file,  4 + (i*4) + (int)_pointer);
                if (temp == 0)
                    _subOffsets[i] = 0;
                else
                    _subOffsets[i] = temp;

                if (i > 0x24)
                    break; //CRASH handler
            }

            List<int> safeList = new List<int>();
            foreach (int a in _subOffsets)
            {
                if (a != 0) 
                    safeList.Add(a);
            }
            

            return safeList.ToArray();


        }

        public void ProcessGf(int offset)
        {
            offset += (int)_pointer;
            _objCount = BitConverter.ToInt32(_file, offset);
            if(_objCount > 0x12)
                goto NOPE;
            _relativeJump = BitConverter.ToInt32(_file, offset + 8);
            _vertexCount = BitConverter.ToUInt16(_file, offset + PassFromStart) * 8;
            _verticesOffset = BitConverter.ToUInt16(_file, offset + PassFromStart - 4);
            //int updateOffset = offset + _relativeJump;
            //Examine polygon type
            _v = null;
            //_vt = null;
            //f = null;

            
            

           /* while (true)
            {
                int passB = PolygonType[BitConverter.ToUInt16(_file, updateOffset)];
                UInt16 polyLenght = BitConverter.ToUInt16(_file, updateOffset + 2);
                ProcessPolygon(passB, updateOffset+4, polyLenght*passB); //Polygon process here
                if (BitConverter.ToUInt32(_file, (updateOffset + 4) + (polyLenght*passB)) == 0xffffffff) //MOD 4!
                {
                    updateOffset += 4 + (polyLenght*passB);
                    break;
                }
                else
                    updateOffset += 4 + polyLenght*passB;
            }*/
            ProcessVertices(_verticesOffset + offset, _vertexCount);
            Console.WriteLine(_v);

        NOPE:
            ;

        }

        private void ProcessPolygon(int bpp, int effectiveoffset, int length)
        {
            /* TODO
            f += string.Format(@"f {0}/{1} {2}/{3} {4}/{5}", null, null, null, null, null, null);
            // END OF TODO*/
        }

        private void ProcessVertices(int effectiveOffset, int length)
        {
            if(length % 8 != 0)
                throw new Exception("Bad file!");

            for (int i = 0; i != length/8; i++)
            {
                float x = (BitConverter.ToInt16(_file, effectiveOffset + i*8)) / 2000.0f;
                float y = (BitConverter.ToInt16(_file, effectiveOffset + i * 8 + 2)) / 2000.0f;
                float z = (BitConverter.ToInt16(_file, effectiveOffset + i * 8 + 4)) / 2000.0f;
                _v += $"v {x} {y} {z}{Environment.NewLine}";
            }
            _v = _v.Replace(',', '.');

        }
    }
}
