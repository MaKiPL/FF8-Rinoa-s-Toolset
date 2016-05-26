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
        //private string vt = null;
        //private string v = null;


        public Wmx(int id,string path)
        {
            _id = id;
            _path = path;

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
                Process(_blockOffsets[i]);
            }
            Console.WriteLine(_f);
        }

        private void ProduceOffsets(byte[] buffer)
        {
            for (int i = 0; i != Blocks; i++)
            {
                _blockOffsets[i] = BitConverter.ToInt32(buffer, i*4);
            }
        }

        private void Process(int offset)
        {
            //initialize count
            byte polygon = _segBuffer[offset];
            byte vertices = _segBuffer[offset + 1];
            byte shadow = _segBuffer[offset + 2];

            int currIndex = offset + 4;
            //Polygon is 16 bytes!
            


            for (int i = 0; i != polygon; i++)
            {
                int vt = (i * 3)+1; // VT index

                TriangleAdd(_segBuffer[currIndex], _segBuffer[currIndex+1], _segBuffer[currIndex+2], vt);
                TriangleAdd(_segBuffer[currIndex+3], _segBuffer[currIndex+4], _segBuffer[currIndex+5], vt);

                //CALCULATE vt here

                currIndex += 16;
            }

        }

        private void TriangleAdd(int a, int b, int c, int vt)
        {
            _f += string.Format("f {0}/{3} {1}/{4} {2}/{5}{6}", a, b, c, vt, vt + 1, vt + 2,Environment.NewLine);
        }
    }
}
