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
            public RES[] res;
            public uint nFrames;
        };

        struct RES
        {
            public uint uOffset;
            public uint uLength;
        }

        private FMVCLIP[] clips;

        private string path;
        public PlayMovie(string path)
        {
            this.path = path;
            clips = new FMVCLIP[256];
            for (int i = 0; i != 255; i++)
                clips[i].res = new RES[2];
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
                    break;
                }
                fs.Seek(2, SeekOrigin.Current);
                clips[nClips].nFrames = br.ReadUInt16();
                n += 8;
                fs.Seek(n+8, SeekOrigin.Begin);
                fs.Seek(clips[nClips].nFrames*0x2C+(0x2C-8), SeekOrigin.Current);
                header = br.ReadUInt32() & 0xFFFFFF;
                if (header != 0x4B4942)
                    break;

                clips[nClips].res[0].uOffset = (uint)fs.Position-4;
                clips[nClips].res[0].uLength = br.ReadUInt32();
                clips[nClips].nFrames = br.ReadUInt32();
                clips[nClips].res[0].uLength += 8;
                n = clips[nClips].res[0].uLength + clips[nClips].res[0].uOffset;

                fs.Seek(n, SeekOrigin.Begin);
                header = br.ReadUInt32() & 0xFFFFFF;
                if(header != 0x4B4942)
                    return;
                clips[nClips].res[1].uOffset = clips[nClips].res[0].uOffset + clips[nClips].res[0].uLength;
                clips[nClips].res[1].uLength = br.ReadUInt32();
                clips[nClips].res[1].uLength += 8;
                n += clips[nClips].res[1].uLength;
                nClips++;
            }
            br.Dispose();
            fs.Dispose();

            for (n = 0; n < nClips; n++)
            {
                Console.WriteLine($"Clip {n+1}");
                Console.WriteLine($"{clips[n].nFrames/900}:{(clips[n].nFrames/15)%60}");
                Console.WriteLine(clips[n].res[0].uLength / 1048576.0 + "M");
                Console.WriteLine($"{clips[n].res[1].uLength / 1048576.0}M");
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
