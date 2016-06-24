using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SerahToolkit_SharpGL
{
    class GfEnviro
    {
        //MODELS ARE UPSIDE DOWN XZY ?
        //Mag005_09 compatible in 75%
        //mag201 ??
        //mag205 To research 


        private readonly byte[] _file;
        private uint[] _subOffsets;

        private const int EnviroOffset = 0xC; //12th byte


        //process vars
        private uint _objCount;
        private int _relativeJump;
        private const int PassFromStart = 24;
        private int _vertexCount;
        private int _verticesOffset;
        private string _v;
        //private string _vt;
        private string f;
        private uint _pointer;
        private string _path;
        private bool _generatefaces;


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
            _path = path;
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
            if (_pointer >= _file.Length)
                return new int[] { 0 };   //Crash handler         
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
            _generatefaces = true;
            offset += (int)_pointer;
            _objCount = BitConverter.ToUInt32(_file, offset);
            if (_objCount > 12u)
                goto NOPE;
            _relativeJump = BitConverter.ToInt32(_file, offset + 8);
            _vertexCount = BitConverter.ToUInt16(_file, offset + PassFromStart) * 8;
            _verticesOffset = BitConverter.ToUInt16(_file, offset + PassFromStart - 4);
            //int updateOffset = offset + _relativeJump;
            //Examine polygon type
            _v = null;
            //_vt = null;
            f = null;
            int polygons = BitConverter.ToUInt16(_file, offset+_relativeJump+2);
            if (BitConverter.ToUInt16(_file, offset + _relativeJump) == 0x09)
                _generatefaces = false;
            int localoffset = offset + _relativeJump + 4;
            if (!_generatefaces)
            {
                Console.WriteLine("GFWorker: This polygon type is supported! Reading data...");
                for (int i = 0; i < polygons*28; i+=28)
                {
                    f += $"f {GetPolygonType9(localoffset+i+18)} {GetPolygonType9(localoffset + i +20)} {GetPolygonType9(localoffset + i +22)}\n";
                }
            }


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
            string ppath = $"{_path}{offset.ToString()}.obj";
            StreamWriter sw = new StreamWriter(ppath, false);
            sw.Write(_v);
            sw.Close();
            sw = new StreamWriter(ppath, true);

            string[] countmebitch = _v.Split('\n');


            if (_generatefaces)
            {
                Console.WriteLine("GFWorker: Unsupported polygon data. Generating fake face indices.");
                for (int i = 1; i < countmebitch.Length - 2; i += 2)
                {
                    sw.WriteLine($"f {i} {i + 1} {i + 2}");
                }
            }
            else
                sw.Write(f);
            SharpGlForm.GFEnviro = _path + offset.ToString() + ".obj";
            sw.Close();
            return;
        NOPE:
            Console.WriteLine($"GFWorker: The file is probably bad. The objCount is{_objCount}");

        }

        private int GetPolygonType9(int offset)
        {
            Console.WriteLine("GFWorker: Polygon type 0x09");
            UInt16 byteb = BitConverter.ToUInt16(_file, offset);
            return byteb == 0 ? 1 : byteb/8 + 1;
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
            Console.WriteLine("GFWorker: Vertices parsed succesfully!");
        }
    }
}
