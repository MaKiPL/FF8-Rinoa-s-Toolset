using System;
using System.Collections.Generic;
using System.IO;

namespace SerahToolkit_SharpGL
{
    class GF_enviro
    {
        //MODELS ARE UPSIDE DOWN XZY ?


        private string _path;
        private byte[] _file;
        private byte[] BadHeader = {0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};
        private uint[] subOffsets;

        private const int EnviroOffset = 0xC;


        //process vars
        private int ObjCount;
        private int relativeJump;
        private const int _passFromStart = 24;
        private int VertexCount;
        private string v;
        private string vt;
        private string f;
        private uint pointer;


        Dictionary<UInt16,int> PolygonType;
         
        


        public GF_enviro(string path)
        {
            this._path = path;
            _file = File.ReadAllBytes(_path);
            PolygonType = new Dictionary<ushort, int>();
            PolygonType.Add(0x8, 20);
            PolygonType.Add(0x9, 28);
            PolygonType.Add(0x12, 24);
            PolygonType.Add(0x13,36);
        }

        public bool bValidHeader()
        {
            byte[] buffer = new byte[8];
            Buffer.BlockCopy(_file,0,buffer,0,buffer.Length);
            if (buffer == BadHeader)
                return false;
            else
                return true;
        }

        public uint[] PopulateOffsets()
        {
            pointer = BitConverter.ToUInt32(_file, EnviroOffset);
            UInt32 Count = BitConverter.ToUInt32(_file, (int)pointer);
            subOffsets = new uint[Count];
            for (int i = 0; i != Count; i++)
            {
                uint temp = BitConverter.ToUInt32(_file,  4 + (i*4) + (int)pointer);
                if (temp == 0)
                    subOffsets[i] = subOffsets[i - 1];
                else
                    subOffsets[i] = temp;
            }
            return subOffsets;
        }

        public void ProcessGF(int offset)
        {
            offset += (int)pointer;
            ObjCount = BitConverter.ToInt32(_file, offset);
            relativeJump = BitConverter.ToInt32(_file, offset + 8);
            VertexCount = BitConverter.ToUInt16(_file, offset + _passFromStart) * 8;
            int updateOffset = offset + relativeJump;
            //Examine polygon type
            v = null;
            vt = null;
            f = null;

            
            

            while (true)
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
            }
            ProcessVertices(updateOffset+4, VertexCount);
            Console.WriteLine(v);



        }

        private void ProcessPolygon(int BPP, int Effectiveoffset, int Length)
        {
            /* TODO
            f += string.Format(@"f {0}/{1} {2}/{3} {4}/{5}", null, null, null, null, null, null);
            // END OF TODO*/
        }

        private void ProcessVertices(int EffectiveOffset, int length)
        {
            if(length % 8 != 0)
                throw new Exception("Bad file!");

            for (int i = 0; i != length/8; i++)
            {
                float X = (BitConverter.ToInt16(_file, EffectiveOffset + i*8)) / 2000.0f;
                float Y = (BitConverter.ToInt16(_file, EffectiveOffset + i * 8 + 2)) / 2000.0f;
                float Z = (BitConverter.ToInt16(_file, EffectiveOffset + i * 8 + 4)) / 2000.0f;
                v += string.Format("v {0} {1} {2}{3}", X, Y, Z, Environment.NewLine);
            }
            v = v.Replace(',', '.');

        }
    }
}
