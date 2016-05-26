using System;
using System.Collections.Generic;
using System.IO;

namespace SerahToolkit_SharpGL
{
    class GF_enviro
    {
        //MODELS ARE UPSIDE DOWN XZY ?
        //Mag005_01 uses other format... 
        //mag201 ??
        //mag205 To research 



        private string _path;
        private readonly byte[] _file;
        private readonly byte[] BadHeader = new byte[8];
        private uint[] subOffsets;

        private const int EnviroOffset = 0xC; //12th byte


        //process vars
        private int ObjCount;
        private int relativeJump;
        private const int _passFromStart = 24;
        private int VertexCount;
        private int VerticesOffset;
        private string v;
        private string _vt;
        private string f;
        private uint pointer;


        private Dictionary<UInt16,int> PolygonType = new Dictionary<ushort, int>
        {
            { 0x7, 20},     //BAD?
            { 0x8, 20}, //OK 
            { 0x9, 28}, //OK
            {0x10, 20},     //BAD?
            {0x12, 24}, //OK
            {0x13, 36}, //OK
            {0x18, 0x18}    //24 BAD?
        };
         
        


        public GF_enviro(string path)
        {
            _path = path;
            _file = File.ReadAllBytes(_path);
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

        public int[] PopulateOffsets()
        {
            pointer = BitConverter.ToUInt32(_file, EnviroOffset);
            UInt32 Count = BitConverter.ToUInt32(_file, (int)pointer);
            subOffsets = new uint[Count];
            for (int i = 0; i != Count; i++)
            {
                uint temp = BitConverter.ToUInt32(_file,  4 + (i*4) + (int)pointer);
                if (temp == 0)
                    subOffsets[i] = 0;
                else
                    subOffsets[i] = temp;

                if (i > 0x24)
                    break; //CRASH handler
            }

            List<int> safeList = new List<int>();
            foreach (int a in subOffsets)
            {
                if (a != 0) 
                    safeList.Add(a);
            }
            

            return safeList.ToArray();


        }

        public void ProcessGF(int offset)
        {
            offset += (int)pointer;
            ObjCount = BitConverter.ToInt32(_file, offset);
            if(ObjCount > 0x12)
                goto NOPE;
            relativeJump = BitConverter.ToInt32(_file, offset + 8);
            VertexCount = BitConverter.ToUInt16(_file, offset + _passFromStart) * 8;
            VerticesOffset = BitConverter.ToUInt16(_file, offset + _passFromStart - 4);
            int updateOffset = offset + relativeJump;
            //Examine polygon type
            v = null;
            _vt = null;
            f = null;

            
            

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
            ProcessVertices(VerticesOffset + offset, VertexCount);
            Console.WriteLine(v);

        NOPE:
            ;

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
