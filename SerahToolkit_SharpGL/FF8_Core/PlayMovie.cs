using System;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class PlayMovie
    {
        struct FMVCLIP
        {
            public struct LOWRES
            {
                uint uOffset;
                uint uLength;
            }
            public struct HIGHRES
            {
                uint uOffset;
                uint uLength;
            }
            public ushort nFrames;
        };

        private FMVCLIP[] clips;

        private string path;
        public PlayMovie(string path)
        {
            this.path = path;
            clips = new FMVCLIP[256];
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
                {
                    Console.WriteLine("BAD FILE!");
                    return;
                }
                fs.Seek(2, SeekOrigin.Current);
                clips[nClips].nFrames = br.ReadUInt16();
                n += 8;

                fs.Seek(n, SeekOrigin.Current);
                header = br.ReadUInt32() & 0xFFFFFF;
                while (header != 0x4B4942)
                {
                    n += 0x2C;
                    fs.Seek(0x2C - 4, SeekOrigin.Current);
                    header = br.ReadUInt32() & 0xFFFFFF;
                }

                //TODO
            }
            

            br.Dispose();
            fs.Dispose();
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
