using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SerahToolkit_SharpGL
{
    class wmx
    {
        private const int Size = 0x9000;
        private const int Segments = 835;
        private const int Blocks = 0x10;
        private int[] _blockOffsets = new int[Blocks];
        private int ID;
        private string Path;
        private byte[] segBuffer;
        private int shadowSize = 8;

        private string f = null;
        private string vt = null;
        private string v = null;


        public wmx(int ID,string Path)
        {
            this.ID = ID;
            this.Path = Path;

            byte[] buffer = new byte[Blocks*4];
            FileStream fs = new FileStream(Path,FileMode.Open);
            fs.Seek((ID*Size)+4, SeekOrigin.Begin);
            fs.Read(buffer, 0, buffer.Length);
            ProduceOffsets(buffer);

            segBuffer = new byte[Size];
            fs.Seek(ID*Size, SeekOrigin.Begin);
            fs.Read(segBuffer, 0, Size);
            fs.Close();


            for (int i = 0; i != Blocks; i++)
            {
                Process(_blockOffsets[i]);
            }
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
            byte polygon = segBuffer[offset];
            byte vertices = segBuffer[offset + 1];
            byte shadow = segBuffer[offset + 2];

            int currIndex = offset + 4;
            //Polygon is 16 bytes!
            


            for (int i = 0; i != polygon; i++)
            {
                int vt = (i * 3)+1; // VT index

                TriangleAdd(segBuffer[currIndex], segBuffer[currIndex+1], segBuffer[currIndex+2], vt);
                TriangleAdd(segBuffer[currIndex+3], segBuffer[currIndex+4], segBuffer[currIndex+5], vt);

                //CALCULATE vt here

                currIndex += 16;
            }



        }

        private void TriangleAdd(int A, int B, int C, int vt)
        {
            f += string.Format("f {0}/{3} {1}/{4} {2}/{5}{6}", A, B, C, vt, vt + 1, vt + 2,Environment.NewLine);
        }
    }
}
