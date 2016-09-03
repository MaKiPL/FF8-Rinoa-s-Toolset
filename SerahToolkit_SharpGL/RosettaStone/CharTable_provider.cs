using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace SerahToolkit_SharpGL.RosettaStone
{
    //DEPRECATED
    internal class CharTableProvider
    {
        readonly string[] _charstable;
        private readonly string _chartable =
        @" , ,1,2,3,4,5,6,7,8,9,%,/,:,!,?,…,+,-,=,*,&,「,」,(,),·,.,,,~,“,”,‘,#,$,',_,A,B,C,D,E,F,G,H,I,J,K,L,M,N,O,P,Q,R,S,T,U,V,W,X,Y,Z,a,b,c,d,e,f,g,h,i,j,k,l,m,n,o,p,q,r,s,t,u,v,w,x,y,z,À,Á,Â,Ä,Ç,È,É,Ê,Ë,Ì,Í,Î,Ï,Ñ,Ò,Ó,Ô,Ö,Ù,Ú,Û,Ü,Œ,ß,à,á,â,ä,ç,è,é,ê,ë,ì,í,î,ï,ñ,ò";

        public CharTableProvider()
        {
            _charstable = _chartable.Split(',');
        }

        public string[] Decipher(byte[] _in)
        {
            if (_in.Length == 0)
                return null;

            string[] process = new string[_in.Length];
            int index = 0;
            foreach(byte a in _in)
            {
                if(a-31>=0 && a<_charstable.Length)
                    process[index] = _charstable[a - 31];
                index++;
            }
            return process;
        }
        public string Decipher(string _in)
        {
            if (_in.Length == 0)
                return null;
            StringBuilder sb = new StringBuilder(_in.Length);
            foreach (byte a in _in)
            {
                if (a - 31 >= 0 && a < _charstable.Length)
                    sb.Append(_charstable[a - 31]);
            }
            return sb.ToString();
        }

        //broken
        //public string Cipher(string _in)
        //{
        //    if (_in.Length == 0)
        //        return null;
        //    StringBuilder sb = new StringBuilder(_in.Length);
        //    foreach (byte a in _in)
        //        sb.Append((byte)(LocateChar(a)+31));
        //    return sb.ToString();
        //}

        public byte[] Cipher(string _in)
        {
            if (_in.Length == 0)
                return null;
            byte[] buffer = new byte[_in.Length];
            for(int i=0; i!=buffer.Length; i++)
                buffer[i] = (byte)(LocateChar((byte)_in[i]) + 31);
            return buffer;
        }

        private uint LocateChar(byte a)
        {
            uint index = 0;
            while (true)
            {
                again:
                if (index >= _charstable.Length)
                    break;
                if (_charstable[index].Length == 0)
                {
                    index++;
                    goto again;
                }
                if ((char)a == _charstable[index][0])
                    return index;
                else index++;
            }
            return 0;
        }
    }

    //FF8Text originally developed for Doomtrain
        internal class FF8Text
        {
            //CONST variables
            private const string _AngeloVar = @"{Var_Angelo}";
            private const string _UnknownVariable = @"{TODO}";
            private const string _UnknownVariable2 = @"{TODO}";
            private const string _UnknownVariable3 = @"{TODO}";
            private const string _UnknownVariable4 = @"{TODO}";


            #region CharTable
            private static Dictionary<byte, string> chartable = new Dictionary<byte, string>
        {
            {0x00, "t"},
            {0x02, "\n"},
            {0x03, _AngeloVar},
            {0x04, _UnknownVariable2 }, //Probably
            {0x0E, _UnknownVariable3 }, //Probably
            {0x20, " "},
            {0x21, "0"},
            {0x22, "1"},
            {0x23, "2"},
            {0x24, "3"},
            {0x25, "4"},
            {0x26, "5"},
            {0x27, "6"},
            {0x28, "7"},
            {0x29, "8"},
            {0x2A, "9"},
            {0x2B, "%"},
            {0x2C, "/"},
            {0x2D, ":"},
            {0x2E, "!"},
            {0x2F, "?"},
            {0x30, "…"},
            {0x31, "+"},
            {0x32, "-"},
            {0x33, "SPECIAL CHARACTER TODO"},
            {0x34, "*"},
            {0x35, "&"},
            {0x36, "SPECIAL CHARACTER TODO" },
            {0x37, "SPECIAL CHARACTER TODO" },
            {0x38, "("},
            {0x39, ")"},
            {0x3A, "SPECIAL CHARACTER TODO"},
            {0x3B, "."},
            {0x3C, ","},
            {0x3D, "~"},
            {0x3E, "SPECIAL CHARACTER TODO"},
            {0x3F, "SPECIAL CHARACTER TODO"},
            {0x40, "'"},
            {0x41, "#"},
            {0x42, "$"},
            {0x43, "`"},
            {0x44, "_"},
            {0x45, "A"},
            {0x46, "B"},
            {0x47, "C"},
            {0x48, "D"},
            {0x49, "E"},
            {0x4A, "F"},
            {0x4B, "G"},
            {0x4C, "H"},
            {0x4D, "I"},
            {0x4E, "J"},
            {0x4F, "K"},
            {0x50, "L"},
            {0x51, "M"},
            {0x52, "N"},
            {0x53, "O"},
            {0x54, "P"},
            {0x55, "Q"},
            {0x56, "R"},
            {0x57, "S"},
            {0x58, "T"},
            {0x59, "U"},
            {0x5A, "V"},
            {0x5B, "W"},
            {0x5C, "X"},
            {0x5D, "Y"},
            {0x5E, "Z"},
            {0x5F, "a"},
            {0x60, "b"},
            {0x61, "c"},
            {0x62, "d"},
            {0x63, "e"},
            {0x64, "f"},
            {0x65, "g"},
            {0x66, "h"},
            {0x67, "i"},
            {0x68, "j"},
            {0x69, "k"},
            {0x6A, "l"},
            {0x6B, "m"},
            {0x6C, "n"},
            {0x6D, "o"},
            {0x6E, "p"},
            {0x6F, "q"},
            {0x70, "r"},
            {0x71, "s"},
            {0x72, "t"},
            {0x73, "u"},
            {0x74, "v"},
            {0x75, "w"},
            {0x76, "x"},
            {0x77, "y"},
            {0x78, "z"},
            {0x79, "Ł"},
            {0x7C, "Ä"},
            {0x88, "Ó"},
            {0x8A, "Ö"},
            {0x8E, "Ü"},
            {0x90, "ß"},
            {0x94, "ä"},
            {0xA0, "ó"},
            {0xA2, "ö"},
            {0xA6, "ü"},
            {0xA8, "Ⅷ"},
            {0xA9, "["},
            {0xAA, "]"},
            {0xAB, "[SQUARE]"},
            {0xAC, "@"},
            {0xAD, "[SSQUARE]"},
            {0xAE, "{"},
            {0xAF, "}"},
            {0xC6, "Ⅵ"},
            {0xC7, "Ⅱ"},
            {0xC9, "™"},
            {0xCA, "<"},
            {0xCB, ">"},
            {0xE8, "in"},
            {0xE9, "e "},
            {0xEA, "ne"},
            {0xEB, "to"},
            {0xEC, "re"},
            {0xED, "HP"},
            {0xEE, "l "},
            {0xEF, "ll"},
            {0xF0, "GF"},
            {0xF1, "nt"},
            {0xF2, "il"},
            {0xF3, "o "},
            {0xF4, "ef"},
            {0xF5, "on"},
            {0xF6, " w"},
            {0xF7, " r"},
            {0xF8, "wi"},
            {0xF9, "fi"},
            {0xFB, "s "},
            {0xFC, "ar"},
            {0xFE, " S"},
            {0xFF, "ag"}
        };
            #endregion

            private static byte[] buffer;

            internal static string BuildString(int index)
            {
                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    if (buffer[index] == 0x00 || index >= buffer.Length - 1)
                        break;
                if (chartable.ContainsKey(buffer[index]))
                    sb.Append(chartable[buffer[index++]]);
                }
                return sb.ToString();
            }

        internal static string BuildString(string s)
        {
            StringBuilder sb = new StringBuilder();
            for (int index = 0; index < s.Length; index++)
            {
                char c = s[index];
                sb.Append(chartable[(byte) c]);
            }
            return sb.ToString();
        }

        /// <summary>
        /// Same as BuildString(index), but takes buffer from input. Same as BuildString_b, but returns directly string
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="index"></param>
        /// <param name="bIgnoreTerminator"></param>
        /// <returns></returns>
        internal static string BuildString(byte[] buffer, int index = 0, bool bIgnoreTerminator = false)
            {
                StringBuilder sb = new StringBuilder();
                while (true)
                {
                    if (buffer[index] == 0x00 && !bIgnoreTerminator || index >= buffer.Length-1)
                        break;
                    if (chartable.ContainsKey(buffer[index]))
                        sb.Append(chartable[buffer[index++]]);
                    else index++;
                }
                return sb.ToString();
            }

        /// <summary>
        /// Builds byte[] decoded string with buffer specified in SetKernel
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        internal static byte[] BuildString_b(int index)
            {
                List<byte> DynamicBuffer = new List<byte>();
                List<byte> DynamicReturn = new List<byte>();
                while (true)
                {
                    if (buffer[index] == 0x00 || index > buffer.Length - 1)
                        break;
                    DynamicBuffer.Add(buffer[index++]);
                }
                int ind = 0;
                while (true)
                {
                    DynamicReturn.AddRange(chartable[DynamicBuffer[ind++]].Select(c => (byte)c));
                    if (ind == DynamicBuffer.Count)
                        break;
                }
                return DynamicReturn.ToArray();
            }

            /// <summary>
            /// Builds byte[] decoded string with buffer specified as input
            /// </summary>
            /// <param name="buffer"></param>
            /// <param name="index"></param>
            /// <returns></returns>
            internal static byte[] BuildString_b(byte[] buffer, int index = 0)
            {
                List<byte> DynamicBuffer = new List<byte>();
                List<byte> DynamicReturn = new List<byte>();
                while (true)
                {
                    if (buffer[index] == 0x00 || index >= buffer.Length - 1)
                        break;
                    DynamicBuffer.Add(buffer[index++]);
                }
                int ind = 0;
                while (true)
                {
                    DynamicReturn.AddRange(chartable[DynamicBuffer[ind++]].Select(c => (byte)c));
                    if (ind == DynamicBuffer.Count)
                        break;
                }
                return DynamicReturn.ToArray();
            }

            /// <summary>
            /// Use only for one character outputs
            /// </summary>
            /// <param name="index"></param>
            /// <returns></returns>
            internal static char BuildString_c(int index) => chartable[buffer[index]].ToCharArray()[0];
            internal static char BuildString_c(char cc) => chartable[(byte)cc][0];

        internal static byte[] Cipher(string _in)
            {
                List<byte> DynamicCipheredBuffer = new List<byte>();

                for (int i = 0; i != _in.Length;)
                {
                    if (_in.Length-i >= _AngeloVar.Length &&
                        _in.Length-i >= _UnknownVariable.Length &&
                        _in.Length-i >= _UnknownVariable2.Length &&
                        _in.Length-i >= _UnknownVariable3.Length &&
                        _in.Length-i >= _UnknownVariable4.Length)
                    {
                        //ANGELO:
                        if (string.Equals(_in.Substring(i, _AngeloVar.Length), _AngeloVar))
                        {
                            DynamicCipheredBuffer.Add(0x04);
                            i += _AngeloVar.Length;
                        }
                        //TODO some other unknown variable and so on:
                        if (string.Equals(_in.Substring(i, _UnknownVariable.Length), _UnknownVariable))
                        {
                            DynamicCipheredBuffer.Add(0x04);
                            i += 0; //Sizeof Squirrel
                        }
                    }
                    if (i >= _in.Length)
                        break;
                    byte key = chartable.FirstOrDefault(x => x.Value == _in[i].ToString()).Key;
                    if (key < 0x20) //Variables are {, but what if user wants to input { that is not variable? All locals are under 0x20, so it's easy to solve it by doing this <---
                        key = 0xAE;
                    DynamicCipheredBuffer.Add(key);
                    i++;
                }
                DynamicCipheredBuffer.Add(0x00); //NULL terminator
                return DynamicCipheredBuffer.ToArray();
            }

            internal static byte[] Cipher(byte[] _in)
            {
                string s = System.Text.Encoding.UTF8.GetString(_in);
                return Cipher(s);
            }

            internal static void SetKernel(byte[] b) => buffer = b; //Okay, that's what Lambda** look like
        }
    }
