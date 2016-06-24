using System.Collections.Generic;
using System.Numerics;

namespace SerahToolkit_SharpGL.FileScanner
{
    public class FileScanner_Headers
    {
        //I think Dictionary could be better, but it's too late now. :P
        public static BigInteger[] Headers = new BigInteger[]
        {
            BigInteger.Parse(0xE8FFBD2701000224.ToString()), //BS
            BigInteger.Parse(0xE8FFBD2703000224.ToString()), //BS
            BigInteger.Parse(0x89504E470D0A1A0A.ToString()),    //PNG
            BigInteger.Parse(0x1000000008000000.ToString()), //TIM 4
            BigInteger.Parse(0x1000000009000000.ToString()), //TIM 8
            BigInteger.Parse(0x0B00000034000000.ToString()), //BattleChara
            BigInteger.Parse(0x0200000010000000.ToString()), //null
            BigInteger.Parse(0x0700000024000000.ToString()), //Weapon
            BigInteger.Parse(0x0800000028000000.ToString()), //Weapon and R0win
            BigInteger.Parse(0x050000001C000000.ToString()), //Weapon
            BigInteger.Parse(0x0A00000030000000.ToString()), //BS
            BigInteger.Parse(0x00000000000000007C000000.ToString()), //GF Enviro
            BigInteger.Parse(0x8400000053434F54.ToString()), //SCOT Sequence GF
            BigInteger.Parse(0x0681001300000080.ToString()) //SceneOUT
        };

        public static List<byte[]> DebugHeaders = new List<byte[]>()
        {
            new byte[] { 0xE8, 0xFF, 0xBD, 0x27, 0x01, 0x00, 0x02, 0x24 }, //BS
            new byte[] {0xE8,0xFF,0xBD,0x27,0x03,0x00,0x02,0x24 }, //BS
            new byte[] {0x89,0x50,0x4E,0x47,0x0D,0x0A,0x1A,0x0A },    //PNG
            new byte[] {0x10,0x00,0x00,0x00,0x08,0x00,0x00,0x00 }, //TIM 4
            new byte[] {0x10,0x00,0x00,0x00,0x09,0x00,0x00,0x00 }, //TIM 8
            new byte[] {0x0B,0x00,0x00,0x00,0x34,0x00,0x00,0x00 }, //BattleChara
            new byte[] {0x02,0x00,0x00,0x00,0x10,0x00,0x00,0x00 }, //null
            new byte[] {0x07,0x00,0x00,0x00,0x24,00,00,00 }, //Weapon
            new byte[] {0x08,00,00,00,0x28,00,00,00 }, //Weapon and R0win
            new byte[] {0x05,00,00,00,0x1C,00,00,00 }, //Weapon
            new byte[] {0x0A,00,00,00,0x30,00,00,00 }, //BS
            new byte[] {0x00,00,00,00,00,00,00,00,0x7C,00,00,00 }, //GF Enviro
            new byte[] {0x84,00,00,00,0x53,0x43,0x4F,0x54 }, //SCOT Sequence GF
            new byte[] {0x06,0x81,00,0x13,00,00,00,0x80 } //SceneOUT
        };

        public static string[] Names = new string[] // : byte
        {
            "Battle Stage file (.x) [I SUPPORT THIS FILE! :)  ]",
            "Battle Stage file (.x) [I SUPPORT THIS FILE! :)  ]",
            "Normal PNG file",
            "TIM Texture 4 BPP",
            "TIM Texture 8 BPP",
            "Battle character and/or enemy",
            "Battle character null file 127",
            "Battle weapon or animation",
            "Battle weapon or r0win.dat",
            "Battle weapon or animation",
            "Battle weapon or animation",
            "GF Environment file [I SUPPORT THIS FILE! :)  ]",
            "GF Sequence (SCOT)",
            "Scene.out",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
            "TODO",
        };
    }
}
