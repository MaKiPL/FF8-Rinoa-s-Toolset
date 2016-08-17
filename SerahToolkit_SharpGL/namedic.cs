using System;

namespace SerahToolkit_SharpGL
{
    static class namedic
    {
        public static ushort _count;
        public static string[] _text;
        public static ushort[] _offsets;

        public static string[] GetText(string path)
        {
            if (!System.IO.File.Exists(path))
                throw new Exception("Bad path!");
            byte[] buffer = System.IO.File.ReadAllBytes(path);
            _count = BitConverter.ToUInt16(buffer, 0);
            _text = new string[_count];
            _offsets = new ushort[_count];
            for(int i = 0; i!=_count; i++)
                _offsets[i] = BitConverter.ToUInt16(buffer, i * 2 + 2);
            RosettaStone.CharTableProvider ctp = new RosettaStone.CharTableProvider();
            for(int i = 0; i!=_count; i++)
            {
                string s = null;
                int index = _offsets[i];
                while(true)
                {
                    if (buffer[index] == 0) break;
                    s += (char)buffer[index];
                    index++;
                }
                _text[i] = ctp.Decipher(s);
            }
            return _text;
        }
    }


}
