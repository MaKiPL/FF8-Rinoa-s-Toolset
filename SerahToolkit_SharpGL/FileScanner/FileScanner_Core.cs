using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SerahToolkit_SharpGL.FileScanner
{
    class FileScanner_Core
    {
        private string _path;
        private List<string> _listOfFiles;

        public FileScanner_Core(string path)
        {
            _path = path;
        }

        public void Start()
        {
            if (_path == null)
                throw new Exception("FileScanner not initialized or not path specified");
            Console.WriteLine($"==========NEW SCAN INITIATED!============\nScanning in: {_path}\n\n");
            _listOfFiles = new List<string>();
            _listOfFiles = Directory.EnumerateFiles(_path).ToList();
            foreach (var file in _listOfFiles)
            {
                    Check(file);
            }
            _path = null; //Just in case?
        }

        /*private void Check(string file)
        {
            var bytes = Header64(file);
            if (bytes == null)
                return;
            //foreach (FileScanner_Headers fsh in Enum.GetValues(typeof(FileScanner_Headers)))
            byte safeValue = 0;
            bool bOutOfOptions = true;
            foreach (BigInteger fsh in FileScanner_Headers.Headers)
            {
                long? b64, b128 = null; //Nullable<T>
                var perform = fsh.ToByteArray();//.Reverse();
                Array.Reverse(perform);
                byte[] newPerform = new byte[32];
                Array.Copy(perform, 1, newPerform, 0, perform.Length-1);
                if (perform.Length >= 16)
                    b128 = BitConverter.ToInt64(newPerform, 16);
                b64 = BitConverter.ToInt64(newPerform, 0);

                if (b128 != null)
                {
                    if (bytes.Item1 == b64 && bytes.Item2 == b128)
                    {
                        Console.WriteLine($"==>>> Engine found file!=={Path.GetFileName(file)} is {FileScanner_Headers.Names[safeValue]}");
                        bOutOfOptions = false;
                        return; //Break out of foreach
                    }
                }
                else if (bytes.Item1 == b64)
                {
                    Console.WriteLine($"==>>> Engine found file!== {Path.GetFileName(file)} is {FileScanner_Headers.Names[safeValue]}");
                    bOutOfOptions = false;
                    return;
                }
                safeValue++;
            }
                if(bOutOfOptions)
                    Console.WriteLine($"{Path.GetFileName(file)} is UNKNOWN. MAGIC: 0x{BitConverter.ToString(BitConverter.GetBytes(bytes.Item1)).Replace("-", "")}"); //Works fine
                //Console.WriteLine($"{Path.GetFileName(file)} is {Enum.GetName(typeof(FileScanner_Headers),fsh)}");
            
        }*/

        //Fast new version
        private void Check(string file)
        {
            var bytes = TemporaryGet32Bytes(file);
            if (bytes == null)
                return;
            
            byte safeValue = 0;
            bool bOutOfOptions = true;

            foreach(byte[] b in FileScanner_Headers.DebugHeaders)
            {
                byte[] buffer = new byte[b.Length];
                Array.Copy(bytes, buffer, buffer.Length); //Test only the same size

                if(buffer.SequenceEqual(b))
                {
                    Console.WriteLine($"==>>> Engine found file!=={Path.GetFileName(file)} is {FileScanner_Headers.Names[safeValue]}");
                    bOutOfOptions = false;
                    return; //Break out of foreach
                }
                safeValue++;
            }
             if (bOutOfOptions)
                 Console.WriteLine($"{Path.GetFileName(file)} is UNKNOWN. MAGIC: 0x{BitConverter.ToString(bytes).Replace("-", "")}"); //Works fine
        }

        private byte[] TemporaryGet32Bytes(string file)
        {
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        if (fs.Length < 32)
                            return null;
                        return br.ReadBytes(32);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with: {Path.GetFileName(file)}, {e.ToString()}");
                return null;
            }
        }

        //Deprecated?
        private Tuple<Int64, Int64> Header64(string file)
        {
            try
            {
                using (FileStream fs = new FileStream(file, FileMode.Open))
                {
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        if (fs.Length < 32)
                            return null;
                        return new Tuple<long, long>(br.ReadInt64(), br.ReadInt64());
                    }
                }
            }
            catch(Exception e)
            {
                Console.WriteLine($"Error with: {Path.GetFileName(file)}, {e.ToString()}");
                return null;
            }
        }
    }
}
