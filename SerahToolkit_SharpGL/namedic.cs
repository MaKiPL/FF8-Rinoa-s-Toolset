using System;
using System.Runtime.InteropServices;

namespace SerahToolkit_SharpGL
{
    static class namedic
    {
        public static ushort _count;
        public static string[] _text;
        public static ushort[] _offsets;

        private static RosettaStone.CharTableProvider ctp;

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
            ctp = new RosettaStone.CharTableProvider();
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

        public static byte[] BuildFile()
        {
            byte[] buffer = new byte[sizeof(ushort) + (sizeof(ushort) * _offsets.Length) + (MathExtended.TotalLength(_text) + _offsets.Length) + sizeof(ushort)]; //count+offsets+text+terminator+end
            Array.Copy(BitConverter.GetBytes(_count), 0, buffer, 0, sizeof(ushort)); //count
            for(int i = 0; i!=_count; i++)
                Array.Copy(BitConverter.GetBytes(_offsets[i]), 0, buffer, 2+i*2, sizeof(ushort)); //offsets
            string[] ciphered = GetCiphered();
            for (int i = 0; i != _count; i++)
                Array.Copy(System.Text.Encoding.ASCII.GetBytes(ciphered[i]), 0, buffer, _offsets[i], ciphered[i].Length); //text
            return buffer;
        }

        private static string[] GetCiphered()
        {
            string[] ciphered = new string[_count];
            for (int i = 0; i != _text.Length; i++)
                ciphered[i] = ctp.Cipher(_text[i]) + (char)0x00; //terminator
            return ciphered;
        }
    }


}
