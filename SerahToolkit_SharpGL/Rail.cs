using System;
using System.IO;
using System.Collections.Generic;
using System.Globalization;

namespace SerahToolkit_SharpGL
{
    class Rail
    {
        private readonly string path;
        private byte[] railobj;
        private const int RailSize = 2048;
        private Byte FirstStop;
        private Byte SecondStop;
        private List<string> EncodedText;
        private int curroff;

        public Rail(string path)
        {
            this.path = path;
            ReadFile();
        }

        private void ReadFile()
        {
            railobj = File.ReadAllBytes(path);
        }

        public List<int> GetRails()
        {
            List<int> RailsOffsets = new List<int>();

            for(int i = 0; i!= railobj.Length; i+= RailSize)
            {
                RailsOffsets.Add(i);
            }

            return RailsOffsets;
        }


        public void rail(int offset)
        {
            curroff = offset;
            Byte Frames = railobj[offset];
            FirstStop = railobj[offset + 4];
            SecondStop = railobj[offset + 8];
            EncodedText = new List<string>();

            string PathOFD = Path.GetDirectoryName(path);
            PathOFD += string.Format(@"\{0}_{1}.obj", Path.GetFileNameWithoutExtension(path), curroff.ToString());

            if (File.Exists(PathOFD))
                File.Delete(PathOFD);

            StreamWriter sw = new StreamWriter(PathOFD);
            //STEP 2 - Header
            sw.WriteLine(@"#Made with Serah toolset by MaKiPL. Hit me up at makipol@gmail.com <3 :*");
            sw.WriteLine("usemtl rail");
            sw.WriteLine("");
            for (int i = offset+ 12; i!= offset+12+(Frames*16); i+= 16)
            {
                int x = BitConverter.ToInt32(railobj, i);
                int y = BitConverter.ToInt32(railobj, i+4);
                int z = BitConverter.ToInt32(railobj, i+8); 
                string enc = System.Text.Encoding.ASCII.GetString(railobj, i + 12, 4);

                EncodedText.Add(enc);
                /*
                Vertex v = new Vertex(((float)x)/10000.0f, ((float)y) / 10000.0f, ((float)z) / 10000.0f);
                rail.Vertices.Add(v); */


                //sw.Write("v {0} {1} {2}", ((float)x) / 10000.0f, ((float)y) / 10000.0f, ((float)z) / 10000.0f);
                string xa = (((float)x) / 10000.0f).ToString(CultureInfo.InvariantCulture); xa = xa.Replace(",", ".");
                string ya = (((float)y) / 10000.0f).ToString(CultureInfo.InvariantCulture); ya = ya.Replace(",", ".");
                string za = (((float)z) / 10000.0f).ToString(CultureInfo.InvariantCulture); za = za.Replace(",", ".");

                sw.Write("v {0} {1} {2}", xa, ya, za);
                sw.Write(Environment.NewLine);
                //sw.Write("v {0} {1} {2}", xa + 0.1f, za + 0.1f, ya + 0.1f);
                //sw.Write(Environment.NewLine);
                



            }

            for(int i = 1; i<= Frames-3; i+=2)
            {
                sw.Write(@"f {0} {1} {2}{3}", i, i + 1, i + 2, Environment.NewLine);
            }
            sw.Close();
        }

        public Tuple<Byte,Byte, List<string>> GetStops_And_string()
        {
            return new Tuple<byte, byte, List<string>>(FirstStop,SecondStop,EncodedText);
        }
    }
}
