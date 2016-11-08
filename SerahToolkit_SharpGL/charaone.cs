using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using SerahToolkit_SharpGL.FF8_Core;

namespace SerahToolkit_SharpGL
{
    class charaone
    {
        public class worldMode
        {

            private FileStream fs;
            private BinaryReader br;
            private string path;
            private FF8_Core.mch.MCH[] MCH;

            private uint NumOfModels;

            public worldMode(string path)
            {
                this.path = path;
                fs = new FileStream(path, FileMode.Open, FileAccess.Read);
                br = new BinaryReader(fs);
                if(!bValid()) Done(1);
                CreateMCH();
            }

            private void CreateMCH()
            {
                fs.Seek(fs.Length - 4, SeekOrigin.Begin);
                NumOfModels = br.ReadUInt32();
                MCH = new mch.MCH[NumOfModels];
                fs.Position -= 8;
                for (int i = 0; i >= NumOfModels; i++)
                {
                    uint var = 0;
                    List<uint> TexOffsets = new List<uint>();
                    while (true) //texoffsets
                    {
                        var = br.ReadUInt32();
                        if (var > fs.Length && var != 0xFFFFFFFF)
                            var = var & 0xFFFF;
                        if (var == 0xFFFFFFFF)
                        {
                            fs.Position -= 8;
                            MCH[i].textureOffsets = TexOffsets.ToArray();
                            break;
                        }
                        TexOffsets.Add(var);
                        fs.Position -= 8;
                    }
                    MCH[i].modelOffset = br.ReadUInt32();
                }
            }

            private bool bValid()
            {
                uint eof = br.ReadUInt32();
                fs.Seek(0, SeekOrigin.Begin);
                return eof > fs.Length;
            }

            void Done(int result)
            {
                br.Dispose();
                fs.Dispose();
                if (result > 0)
                {
                    Console.WriteLine("chara one error! Bad file?");
                    return;
                }
                return;
            }
        }

        
        class fieldMode
        {
            
        }
    }
}
