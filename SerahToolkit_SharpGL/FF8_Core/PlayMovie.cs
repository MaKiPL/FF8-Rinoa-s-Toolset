using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class PlayMovie
    {
        struct MovieClip
        {
            public Resolutions[] Resolutions;
            public uint Frames;
        };

        struct Resolutions
        {
            public uint Offset;
            public uint Size;
        }

        private MovieClip[] _mClips;
        public static bool bSuccess;

        private string path;
        public PlayMovie(string path)
        {
            this.path = path;
            _mClips = new MovieClip[256];
            for (int i = 0; i != 255; i++)
                _mClips[i].Resolutions = new Resolutions[2];
        }

        public void Read()
        {
            if(path == null)
                return;
            FileStream fs = new FileStream(path, FileMode.Open,FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            uint n = 0;
            uint len = (uint)fs.Length;
            int nClips = 0;
            while (n < len)
            {
                fs.Seek(n, SeekOrigin.Begin);
                uint header = br.ReadUInt32() & 0xFFFFFF;
                if (header != 0x503846)
                    return;
                fs.Seek(2, SeekOrigin.Current);
                _mClips[nClips].Frames = br.ReadUInt16();
                n += 8;
                fs.Seek(n+8, SeekOrigin.Begin);
                fs.Seek(_mClips[nClips].Frames*0x2C+(0x2C-8), SeekOrigin.Current);
                header = br.ReadUInt32() & 0xFFFFFF;
                if (header != 0x4B4942)
                    return;

                _mClips[nClips].Resolutions[0].Offset = (uint)fs.Position-4;
                _mClips[nClips].Resolutions[0].Size = br.ReadUInt32();
                _mClips[nClips].Frames = br.ReadUInt32();
                _mClips[nClips].Resolutions[0].Size += 8;
                n = _mClips[nClips].Resolutions[0].Size + _mClips[nClips].Resolutions[0].Offset;

                fs.Seek(n, SeekOrigin.Begin);
                header = br.ReadUInt32() & 0xFFFFFF;
                if(header != 0x4B4942)
                    return;
                _mClips[nClips].Resolutions[1].Offset = _mClips[nClips].Resolutions[0].Offset + _mClips[nClips].Resolutions[0].Size;
                _mClips[nClips].Resolutions[1].Size = br.ReadUInt32();
                _mClips[nClips].Resolutions[1].Size += 8;
                n += _mClips[nClips].Resolutions[1].Size;
                nClips++;
            }
            bSuccess = true;
            br.Dispose();
            fs.Dispose();

            for (n = 0; n < nClips; n++)
            {
                Console.WriteLine($"Clip {n+1}");
                Console.WriteLine($"{_mClips[n].Frames/900}:{(_mClips[n].Frames/15)%60}");
                Console.WriteLine(_mClips[n].Resolutions[0].Size / 1048576.0 + "M");
                Console.WriteLine($"{_mClips[n].Resolutions[1].Size / 1048576.0}M");
            }
        }

        private static string BuildPath(byte MovieID)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(@"Movies/");
            sb.Append("disc");
            sb.Append(MovieID <= 30 ? "00" :
                 MovieID > 30 && MovieID <= 30 + 34 ? "01" :
                 MovieID > 30 + 34 && MovieID <= 64 + 32 ? "02" :
                 MovieID > 64 + 32 && MovieID <= 64 + 32 + 7 ? "04" : "01");
            sb.Append(@"_");
            string temp;
            if (MovieID > 9)
                temp = MovieID.ToString();
            else
                temp = "0" + MovieID.ToString();
            sb.Append(temp + @"h");

            return sb.ToString();
        }
    }
}
