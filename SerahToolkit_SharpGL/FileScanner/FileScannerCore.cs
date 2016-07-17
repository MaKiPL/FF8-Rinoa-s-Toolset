using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SerahToolkit_SharpGL.FileScanner
{
    internal class FileScannerCore
    {
        private string _path;
        private List<string> _listOfFiles;

        public FileScannerCore(string path)
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
            _path = null;
        }
        private void Check(string file)
        {
            var bytes = TemporaryGet32Bytes(file);
            if (bytes == null)
                return;
            
            byte safeValue = 0;
            bool bOutOfOptions = true;
            FileScannerHeaders headers = new FileScannerHeaders();

            foreach(byte[] b in headers.Headers)
            {
                byte[] buffer = new byte[b.Length];
                Array.Copy(bytes, buffer, buffer.Length); 

                if(buffer.SequenceEqual(b))
                {
                    Console.WriteLine($"==>>> Engine found file!=={Path.GetFileName(file)} is {headers.Names[safeValue]}");
                    bOutOfOptions = false;
                    return; 
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
                FileStream fs = new FileStream(file, FileMode.Open);
                BinaryReader br = new BinaryReader(fs);
                        return fs.Length < 32 ? null : br.ReadBytes(32);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error with: {Path.GetFileName(file)}, {e.ToString()}");
                return null;
            }
        }
    }
}
