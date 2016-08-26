using System;
using System.IO;
using System.Linq;
using System.Text;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class ArchiveWorker
    {
        static UInt32 UnpackedFileSize;
        static UInt32 LocationInFS;
        static bool Compressed;

        public static void OpenArchive()
        {
            throw new System.Exception("NOT IMPLEMENTED!");
        }

        public static byte[] GetBinaryFile(string ArchiveName, string fileName)
        {
            string a = @"C:\ff8\data\eng\" + fileName;

            byte[] IsComp = GetBin(ArchiveName, a);
            if (Compressed)
                return LZSS.decompressAll(IsComp, (uint)IsComp.Length, (int)UnpackedFileSize);
            else
                return IsComp;
        }
        /// <summary>
        /// Give me three archives as bytes uncompressed please!
        /// </summary>
        /// <param name="FI">FileIndex</param>
        /// <param name="FS">FileSystem</param>
        /// <param name="FL">FileList</param>
        /// <param name="filename">Filename of the file to get</param>
        /// <returns></returns>
        public static byte[] FileInTwoArchives(byte[] FI, byte[] FS, byte[] FL, string filename)
        {
            string a = @"C:\ff8\data\eng\" + filename;

            string FL_Text = System.Text.Encoding.UTF8.GetString(FL);
            FL_Text = FL_Text.Replace(Convert.ToString(0x0d), "");
            int Loc = -1;
            string[] Files = FL_Text.Split((char)0x0a);
            for (int i = 0; i != Files.Length - 1; i++)
            {
                string testme = Files[i].Substring(0, Files[i].Length - 1).ToUpper();
                if (testme == a.ToUpper())
                {
                    Loc = i;
                    break;
                }
            }
            if (Loc == -1)
                throw new Exception("ArchiveWorker: No such file!");


            UInt32 FSLen = BitConverter.ToUInt32(FI, Loc * 12);
            UInt32 FSpos = BitConverter.ToUInt32(FI, (Loc * 12) + 4);
            bool compe = BitConverter.ToUInt32(FI, (Loc * 12) + 8) == 0 ? false : true;

            byte[] File = new byte[BitConverter.ToUInt32(FS, (int)FSpos) + 4];

            Array.Copy(FS, FSpos, File, 0, File.Length);
            if (compe)
                return LZSS.decompressAll(File, (uint)File.Length, (int)FSLen);
            else return File;
        }

        private static byte[] GetBin(string ArchiveName, string fileName)
        {
            if (fileName.Length < 1 || ArchiveName.Length < 1)
                throw new System.Exception("NO FILENAME OR ARCHIVE!");

            string ArchivePath = ArchiveName + Singleton.Archives.B_FileArchive;
            string ArchiveIndexPath = ArchiveName + Singleton.Archives.B_FileIndex;
            string ArchiveNamesPath = ArchiveName + Singleton.Archives.B_FileList;
            int Loc = -1;

            FileStream fs = new FileStream(ArchiveNamesPath, FileMode.Open);
            TextReader TR = new StreamReader(fs);
            string locTR = TR.ReadToEnd();
            TR.Dispose();
            fs.Close();
            locTR = locTR.Replace(Convert.ToString(0x0d), "");
            string[] Files = locTR.Split((char)0x0a);
            for (int i = 0; i != Files.Length - 1; i++)
            {
                string testme = Files[i].Substring(0, Files[i].Length - 1).ToUpper();
                if (testme == fileName.ToUpper())
                {
                    Loc = i;
                    break;
                }
            }
            if (Loc == -1)
                throw new Exception("ArchiveWorker: No such file!");

            fs = new FileStream(ArchiveIndexPath, FileMode.Open);
            BinaryReader br = new BinaryReader(fs);
            fs.Seek(Loc * 12, SeekOrigin.Begin);
            UnpackedFileSize = br.ReadUInt32(); //fs.Seek(4, SeekOrigin.Current);
            LocationInFS = br.ReadUInt32();
            Compressed = br.ReadUInt32() == 0 ? false : true;
            fs.Close();

            fs = new FileStream(ArchivePath, FileMode.Open);
            fs.Seek(LocationInFS, SeekOrigin.Begin);

            br = new BinaryReader(fs);
            int HowMany = Compressed ? br.ReadInt32() : (int)UnpackedFileSize;

            byte[] temp;
            if (Compressed)
            {
                fs.Seek(-4, SeekOrigin.Current);
                temp = br.ReadBytes(HowMany + 4);

            }
            else
                temp = br.ReadBytes(HowMany);

            fs.Close();


            return temp;
        }

        private byte[] Decomp()
        {
            return null;
        }
    }
}
