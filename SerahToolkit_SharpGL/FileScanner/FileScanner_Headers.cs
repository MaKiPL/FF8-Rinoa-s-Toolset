using System.Collections.Generic;
using System.Numerics;

namespace SerahToolkit_SharpGL.FileScanner
{
    public class FileScanner_Headers
    {
        public static BigInteger[] Headers = new BigInteger[]
        {
            BigInteger.Parse(0xE8FFBD2701000224.ToString()),
            BigInteger.Parse(0xE8FFBD2703000224.ToString())
        };

        public static string[] Names = new string[] // : byte
        {
            "Battle Stage file (.x)",
            "Battle Stage file (.x)"
        };
    }
}
