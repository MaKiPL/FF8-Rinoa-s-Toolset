using System;
using System.IO;

namespace SerahToolkit_SharpGL
{
    class Wmx
    {
        private const int Size = 0x9000;
        private const int Segments = 835;
        private const int Blocks = 0x10;
        private readonly int[] _blockOffsets = new int[Blocks];
        private int _id;
        private string _path;
        private readonly byte[] _segBuffer;
        //private int shadowSize = 8;

        private string _f = null;
        private string _vt = null;
        private string _v = null;

        private int _absolutePolygon = 0;
        private float _absoluteVertice = 0;

        //From wmx2obj source code
        private int _offsetX;
        private int _offsetZ;
        private int _size = 2048;
        private int _columnsPerRow = 4;

        private string buildPath;
        //END


        public Wmx(int id,string path)
        {
            _id = id;
            _path = path;

            _vt = null;
            _v = null;
            _f = null;
            _absolutePolygon = 0;
            _absoluteVertice = 0;

            //wmx2obj
            _offsetX = _size * (id % _columnsPerRow);
            _offsetZ = -_size * (id / _columnsPerRow);
            //end

            byte[] buffer = new byte[Blocks*4];
            FileStream fs = new FileStream(path,FileMode.Open);
            fs.Seek((id*Size)+4, SeekOrigin.Begin);
            fs.Read(buffer, 0, buffer.Length);
            ProduceOffsets(buffer);

            _segBuffer = new byte[Size];
            fs.Seek(id*Size, SeekOrigin.Begin);
            fs.Read(_segBuffer, 0, Size);
            fs.Close();


            for (int i = 0; i != Blocks; i++)
            {
                Process(_blockOffsets[i], i);
            }

            buildPath = $"{Path.GetDirectoryName(path)}\\wmx_sector{id.ToString()}.obj";
            if (File.Exists(buildPath))
                File.Delete(buildPath);

            StreamWriter sw = new StreamWriter(buildPath);
            sw.WriteLine(_v);
            sw.WriteLine(_vt);
            sw.WriteLine(_f);
            sw.Close();

            //Console.WriteLine($"WMX: debug output: {_v}{_f}");
        }

        private void ProduceOffsets(byte[] buffer)
        {
            for (int i = 0; i != Blocks; i++)
            {
                _blockOffsets[i] = BitConverter.ToInt32(buffer, i*4);
            }
        }

        private void Process(int offset, int id)
        {
            //initialize count
            byte polygon = _segBuffer[offset];
            byte vertices = _segBuffer[offset + 1];
            byte shadow = _segBuffer[offset + 2];

            int currIndex = offset + 4;
            //Polygon is 16 bytes!

            _offsetX = _size * (id % _columnsPerRow);
            _offsetZ = -_size * (id / _columnsPerRow);

            for (int i = 0; i != polygon; i++)
            {
                int vt = (i * 3)+1; // VT index

                TriangleAdd(_segBuffer[currIndex], _segBuffer[currIndex+1], _segBuffer[currIndex+2], vt);
                //TriangleAdd(_segBuffer[currIndex+3], _segBuffer[currIndex+4], _segBuffer[currIndex+5], vt);

                _vt += $"vt {_segBuffer[currIndex+6]/256.0f} {_segBuffer[currIndex+7]/256.0f}\n";
                _vt += $"vt {_segBuffer[currIndex + 8] / 256.0f} {_segBuffer[currIndex + 9] / 256.0f}\n";
                _vt += $"vt {_segBuffer[currIndex + 10] / 256.0f} {_segBuffer[currIndex + 11] / 256.0f}\n";

                //+12 is clut

                currIndex += 16;
            }

            for(int i = 0; i<= vertices; i++)
            {
                _v += $"v {((BitConverter.ToInt16(_segBuffer, currIndex)+_offsetX)/1000.0f).ToString().Replace(',','.')} {((BitConverter.ToInt16(_segBuffer, currIndex+2))/1000.0f).ToString().Replace(',', '.')} {((BitConverter.ToInt16(_segBuffer, currIndex+4)+_offsetZ)/1000.0f).ToString().Replace(',', '.')}\n";
                currIndex += 8;
            }
            _absoluteVertice += vertices;
            _vt = _vt.Replace(',', '.');
            _absolutePolygon += 0;
        }

        private void TriangleAdd(int a, int b, int c, int vt)
        {
            _f += string.Format("f {0}/{3} {1}/{4} {2}/{5}{6}", a+1+_absolutePolygon, b+1+_absolutePolygon, c+1+_absolutePolygon, vt, vt + 1, vt + 2,Environment.NewLine);
        }

        public string GetModelPath()
        {
            if(buildPath == null)
            {
                Console.WriteLine("WMX: Error! Model path is null. Probably something failed at saving?");
                return null;
            }
            return buildPath;
        }
    }
}
