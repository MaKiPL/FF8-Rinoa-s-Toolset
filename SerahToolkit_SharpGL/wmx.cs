using System;
using System.IO;

//Vertices VerteX and VertexY calculation based on block from segment is copied from
//Simo Ollonen's WMX2OBJ by author permission from 17 November 2015
//You can see wmx2obj software at: https://github.com/SimoOllonen/wmx2obj

namespace SerahToolkit_SharpGL
{
    internal class Wmx
    {
        private const int Size = 0x9000;
        private const int Blocks = 0x10;
        private readonly int[] _blockOffsets = new int[Blocks];
        private readonly byte[] _segBuffer;

        private string _f;
        private string _vt;
        private string _v;

        private int _absolutePolygon;

        //wmx2obj source code
        private int _offsetX;
        private int _offsetZ;
        private int _size = 2048;
        private int _columnsPerRow = 4;
        //END

        private string buildPath;

        public Wmx(int id,string path)
        {
            Console.WriteLine($"WMX: Initialized engine for ID: {id}, Path:{path}");
            _vt = null;
            _v = null;
            _f = null;
            _absolutePolygon = 0;

            //wmx2obj
            _offsetX = _size * (id % _columnsPerRow);
            _offsetZ = -_size * (id / _columnsPerRow);
            //end

            Console.WriteLine($"WMX: Reading file");
            byte[] buffer = new byte[Blocks*4];
            FileStream fs = new FileStream(path,FileMode.Open);
            fs.Seek((id*Size)+4, SeekOrigin.Begin);
            fs.Read(buffer, 0, buffer.Length);
            ProduceOffsets(buffer);
            Console.WriteLine($"WMX: Copying segment to buffer");
            _segBuffer = new byte[Size];
            fs.Seek(id*Size, SeekOrigin.Begin);
            fs.Read(_segBuffer, 0, Size);
            fs.Close();
            for (int i = 0; i != Blocks; i++)
            {
                Console.WriteLine($"WMX: Processing block {i+1}/16");
                Process(_blockOffsets[i], i);
            }
            buildPath = $"{Path.GetDirectoryName(path)}\\wmx_sector{id.ToString()}.obj";
            if (File.Exists(buildPath))
                File.Delete(buildPath);
            Console.WriteLine($"WMX: Saving Wavefront OBJ model to {buildPath}");
            StreamWriter sw = new StreamWriter(buildPath);
            Console.WriteLine($"WMX: Writing vertices...");
            sw.WriteLine(_v);
            Console.WriteLine($"WMX: Writing UV...");
            sw.WriteLine(_vt);
            Console.WriteLine($"WMX: Writing triangles...");
            sw.WriteLine(_f);
            Console.WriteLine($"WMX: Everything saved!");
            sw.Close();
        }

        private void ProduceOffsets(byte[] buffer)
        {
            Console.WriteLine($"WMX: Producing 16 blocks offsets");
            for (int i = 0; i != Blocks; i++)
                _blockOffsets[i] = BitConverter.ToInt32(buffer, i*4);
        }

        private void Process(int offset, int id)
        {
            byte polygon = _segBuffer[offset];
            byte vertices = _segBuffer[offset + 1];
            byte shadow = _segBuffer[offset + 2];
            int currIndex = offset + 4;
            Console.WriteLine($"WMX: Block {id+1} contains:\n\tPolygons: {polygon}\n\tVertices: {vertices}\n\tNormals: {shadow}");

            //wmx2obj
            _offsetX = _size * (id % _columnsPerRow);
            _offsetZ = -_size * (id / _columnsPerRow);
            //end

            Console.WriteLine($"WMX: Block {id+1}: Processing polygons and UV coordinates");
            for (int i = 0; i != polygon; i++)
            {
                int vt = (i * 3)+1;

                TriangleAdd(_segBuffer[currIndex], _segBuffer[currIndex+1], _segBuffer[currIndex+2], vt);

                _vt += $"vt {_segBuffer[currIndex+6]/256.0f} {_segBuffer[currIndex+7]/256.0f}\n";
                _vt += $"vt {_segBuffer[currIndex + 8] / 256.0f} {_segBuffer[currIndex + 9] / 256.0f}\n";
                _vt += $"vt {_segBuffer[currIndex + 10] / 256.0f} {_segBuffer[currIndex + 11] / 256.0f}\n";
                //+12 is clut / TODO?
                currIndex += 16;
            }
            Console.WriteLine($"WMX: Block {id+1}: Processing vertices");
            for (int i = 0; i<= vertices; i++)
            {
                _v += $"v {((BitConverter.ToInt16(_segBuffer, currIndex)+_offsetX)/1000.0f).ToString().Replace(',','.')} {((BitConverter.ToInt16(_segBuffer, currIndex+2) * -1.0f)/1000.0f).ToString().Replace(',', '.')} {((BitConverter.ToInt16(_segBuffer, currIndex+4)+_offsetZ)/1000.0f).ToString().Replace(',', '.')}\n";
                currIndex += 8;
            }
            _vt = _vt.Replace(',', '.');
            Console.WriteLine($"WMX: Block {id+1} finished! Preparing variables for next block.");
            _absolutePolygon += vertices+1;
        }

        private void TriangleAdd(int a, int b, int c, int vt) => _f += string.Format($"f {a+1+_absolutePolygon}/{vt} {b+1+_absolutePolygon}/{vt + 1} {c+1+_absolutePolygon}/{vt + 2}{Environment.NewLine}");

        public string GetModelPath()
        {
            if(buildPath == null)
            {
                Console.WriteLine("WMX: Error! Model path is null. Probably something failed at saving?");
                return null;
            }
            Console.WriteLine($"WMX: Done! Building model path for renderer!");
            return buildPath;
        }
    }
}
