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
        public bool _onlyVertex = false;
        private ushort polygonType;
        public bool _bForceNotDraw = false;
        private int[] _knownPolygons = new int[] 
        {
            0x7,
            0x8,
            0x9,
            0xC,
            0x10,
            0x12,
            0x11,
            0x13,
        };


        private Dictionary<UInt16,int> _polygonType = new Dictionary<ushort, int>
        {
            { 0x7, 20},     //BAD?
            { 0x8, 20}, //OK 
            { 0x9, 28}, //OK
            { 0xC, 28}, //OK
            {0x10, 20},     //BAD?
            {0x13, 36}, //OK
            {0x18, 0x18},    //24 BAD?
            //0x13 = 36
            //0x7 = 20
            //0x10 = 12
            {17, 24 }
        };

         
        /*Polygon is:
         * 
         * uint16 - type
         * uint16 - count
         * FF FF FF FF ends object
         */



        //0x8 structure:
        /*
         * 0xA - A
         * 0xC - B
         * 0xE - C
         * 
         */
        

        //0x9 structure:
            /*
             * 20, 22, 24?? is polygon?
             * 18, 20, 22 is divide by 8.
             * 
             * 0x12: Divide by 8
             * 12 A
             * 14 B
             * 16 C
             * 18 D
             */


        public GfEnviro(string path)
        {
            _path = path;
            _file = File.ReadAllBytes(path);
        }

        public int[] PopulateOffsets()
        {
            _pointer = BitConverter.ToUInt32(_file, EnviroOffset);
            if (_pointer >= _file.Length)
                return new int[] { 0 };   //Crash handler         
            if (_pointer == 0)
                Console.WriteLine("GFWorker: BAD FILE! This file has no section offsets");
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
            _bForceNotDraw = false;
            _onlyVertex = true;
            _generatefaces = true;
            offset += (int)_pointer;
            _objCount = BitConverter.ToUInt32(_file, offset);
            if (_objCount > 12u)
                goto NOPE;
            if (_objCount != 2u)
                _onlyVertex = false;
            _relativeJump = BitConverter.ToInt32(_file, offset + 8);
            _vertexCount = BitConverter.ToUInt16(_file, offset + PassFromStart) * 8;
            _verticesOffset = BitConverter.ToUInt16(_file, offset + PassFromStart - 4);
            if (_onlyVertex)
            {
                Console.WriteLine("GFWorker: This specific model contains only vertices data. Probably used for animation purpouses?");
                Console.WriteLine("GFWorker: Switching renderer over to point cloud mode");
                Console.WriteLine("GFWorker: Fake face indices are still generated and saved to file. Ignore them");
                goto vertex;
            }
            //Examine polygon type
            _v = null;
            //_vt = null;
            f = null;
            int polygons = BitConverter.ToUInt16(_file, offset+_relativeJump+2);
            polygonType = BitConverter.ToUInt16(_file, offset + _relativeJump);

            foreach(int a in _knownPolygons)
            {
                if (a == polygonType)
                    _generatefaces = false;
            }

            int localoffset = offset + _relativeJump + 4;
            if (!_generatefaces)
            {
                Console.WriteLine($"GFWorker: This polygon type {polygonType.ToString()} is supported! Reading data...");
                int safeHandle = 0;
                cheeseBurger:

                if (polygonType == 0x07)
                {
                    for (int i = 0; i < polygons * 20; i += 20)
                    {
                        f += $"f {GetPolygon(localoffset + i + 0xC)} {GetPolygon(localoffset + i + 0xE)} {GetPolygon(localoffset + i + 0x10)}\n";
                    }
                    safeHandle = polygons * 20;
                }
                if (polygonType == 0x09)
                {
                    for (int i = 0; i < polygons * 28; i += 28)
                    {
                        f += $"f {GetPolygon(localoffset + i + 18)} {GetPolygon(localoffset + i + 20)} {GetPolygon(localoffset + i + 22)}\n";
                    }
                    safeHandle = polygons * 28;
                }

                if (polygonType == 0x08)
                {
                    for (int i = 0; i < polygons * 20; i += 20)
                    {
                        f += $"f {GetPolygon(localoffset + i + 0xA)} {GetPolygon(localoffset + i + 0xC)} {GetPolygon(localoffset + i + 0xE)}\n";
                    }
                    safeHandle = polygons * 20;
                }

                if (polygonType == 12)
                {
                    for (int i = 0; i < polygons * 28; i += 28)
                    {
                        f += $"f {GetPolygon(localoffset + i + 20)} {GetPolygon(localoffset + i + 22)} {GetPolygon(localoffset + i + 26)} {GetPolygon(localoffset + i + 24)}\n";
                    }
                    safeHandle = polygons * 28;
                }


                if (polygonType == 0x12)
                {
                    for (int i = 0; i < polygons * 24; i += 24)
                    {
                        f += $"f {GetPolygon(localoffset + i + 12)} {GetPolygon(localoffset + i + 14)} {GetPolygon(localoffset + i + 18)} {GetPolygon(localoffset + i + 16)}\n";
                    }
                    safeHandle = polygons * 24;
                }

                if (polygonType == 0x13)
                {
                    for (int i = 0; i < polygons * 36; i += 36)
                    {
                        f += $"f {GetPolygon(localoffset + i + 24)} {GetPolygon(localoffset + i + 26)} {GetPolygon(localoffset + i + 30)} {GetPolygon(localoffset + i + 28)}\n";
                    }
                    safeHandle = polygons * 36;
                }
                if (polygonType == 0x11)
                {
                    for (int i = 0; i < polygons * 24; i += 24)
                    {
                        f += $"f {GetPolygon(localoffset + i + 16)} {GetPolygon(localoffset + i + 18)} {GetPolygon(localoffset + i + 22)} {GetPolygon(localoffset + i + 20)}\n";
                    }
                    safeHandle = polygons * 24;
                }

                if (polygonType == 0x10)
                {
                    for (int i = 0; i < polygons * 12; i += 12)
                    {
                        f += $"f {GetPolygon(localoffset + i + 4)} {GetPolygon(localoffset + i + 6)} {GetPolygon(localoffset + i + 10)} {GetPolygon(localoffset + i + 8)}\n";
                    }
                    safeHandle = polygons * 12;
                }

                uint isNext = BitConverter.ToUInt32(_file, localoffset + safeHandle);
                if (isNext != 0xFFFFFFFF)
                {
                    Console.WriteLine("GFWorker: There are more polygons for this object! Reading again...");
                    localoffset += safeHandle;
                    polygonType = BitConverter.ToUInt16(_file, localoffset);
                    polygons = BitConverter.ToUInt16(_file, localoffset + 2);
                    Console.WriteLine($"GFWorker: New polygon is {polygonType}");
                    localoffset += 4;
                    goto cheeseBurger;
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

            vertex:
            ProcessVertices(_verticesOffset + offset, _vertexCount);
            string ppath = $"{_path}{offset.ToString()}.obj";
            StreamWriter sw = new StreamWriter(ppath, false);
            sw.Write(_v);
            sw.Close();
            sw = new StreamWriter(ppath, true);

            string[] countmebitch = _v.Split('\n');


            if (_generatefaces)
            {
                if(!_onlyVertex)
                    Console.WriteLine($"GFWorker: Unsupported polygon {polygonType} data. Generating fake face indices.");
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
            _bForceNotDraw = true;

        }

        private int GetPolygon(int offset)
        {
            UInt16 byteb = BitConverter.ToUInt16(_file, offset);
            return byteb == 0 ? 1 : byteb/8 + 1;
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
                _v += $"v {x} {y*-1f /*Math.Abs(y) * -1.0f*/} {z}{Environment.NewLine}";
            }
            _v = _v.Replace(',', '.');
            Console.WriteLine("GFWorker: Vertices parsed succesfully!");
        }
    }
}
