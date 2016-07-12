using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace SerahToolkit_SharpGL
{
    class Rail
    {
        private readonly string _path;
        private byte[] _railobj;
        private const int RailSize = 2048;
        private Byte _firstStop;
        private Byte _secondStop;
        private int _curroff;

        public Rail(string path)
        {
            _path = path;
            ReadFile();
        }

        private void ReadFile()
        {
            _railobj = File.ReadAllBytes(_path);
        }

        public List<int> GetRails()
        {
            List<int> railsOffsets = new List<int>();

            for(int i = 0; i!= _railobj.Length; i+= RailSize)
            {
                railsOffsets.Add(i);
            }

            return railsOffsets;
        }


        public void rail(int offset)
        {
            Console.WriteLine($"Rail: Reading rail data at {offset}");
            _curroff = offset;
            Byte frames = _railobj[offset];
            _firstStop = _railobj[offset + 4];
            _secondStop = _railobj[offset + 8];

            string pathOfd = Path.GetDirectoryName(_path);
            pathOfd += $@"\{Path.GetFileNameWithoutExtension(_path)}_{_curroff}.obj";

            if (File.Exists(pathOfd))
                File.Delete(pathOfd);

            StreamWriter sw = new StreamWriter(pathOfd);
            //STEP 2 - Header
            sw.WriteLine(@"#Made with Rinoa's toolset by MaKiPL.");
            sw.WriteLine("usemtl rail");
            sw.WriteLine("Genrated polygons are fake. You can delete them!");
            sw.WriteLine("");
            for (int i = offset+ 12; i!= offset+12+(frames*16); i+= 16)
            {
                int x = BitConverter.ToInt32(_railobj, i);
                int y = BitConverter.ToInt32(_railobj, i+4);
                int z = BitConverter.ToInt32(_railobj, i+8); 
                string xa = (x / 10000.0f).ToString(CultureInfo.InvariantCulture); xa = xa.Replace(",", ".");
                string ya = (y / 10000.0f).ToString(CultureInfo.InvariantCulture); ya = ya.Replace(",", ".");
                string za = (z / 10000.0f).ToString(CultureInfo.InvariantCulture); za = za.Replace(",", ".");

                sw.Write("v {0} {1} {2}", xa, ya, za);
                sw.Write(Environment.NewLine);
            }

            //fake polygons
            for(int i = 1; i<= frames-3; i+=2)
            {
                sw.Write(@"f {0} {1} {2}{3}", i, i + 1, i + 2, Environment.NewLine);
            }
            sw.Close();
        }
    }
}
