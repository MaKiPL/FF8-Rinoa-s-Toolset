using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class Singleton
    {
        #region AudioData
        public static Dictionary<UInt16, string> Songs = new Dictionary<UInt16, string>()
        {
            {0, "Lose" },
            {1, "Win" },
            {2, "Open" },
            {3, "Combat" },
            {4, "Run" },
            {5, "Battle" },
            {6, "Funsui" },
            {7, "End" },
            {8, "Antenna" },
            {9, "Waiting" },
            {10, "Ante " },
            {11, "Wind" },
            {12, "Crab" },
            {13, "Battle2" },
            {14, "Friend2" },
            {15, "Fuan2" },
            {16, "March2" },
            {17, "Land" },
            {18, "Julia" },
            {19, "Waltz" },
            {20, "Friend " },
            {21, "Dungeon" },
            {22, "Pianosolo" },
            {23, "Parade" },
            {24, "March1" },
            {25, "Secret" },
            {26, "Garden" },
            {27, "Fuan " },
            {28, "Polka2" },
            {29, "Anthem" },
            {30, "FlangChorus" },
            {31, "DubChorus" },
            {32, "SoloChorus" },
            {33, "FemaleChorus" },
            {34, "Chorus" },
            {35, "M7F5" },
            {36, "Sorceress" },
            {37, "Reet" },
            {38, "Soyo" },
            {39, "Rouka" },
            {40, "Night" },
            {41, "Field" },
            {42, "Guitar" },
            {43, "Concert" },
            {44, "Sea" },
            {45, "Silent" },
            {46, "Resistance" },
            {47, "Kaiso" },
            {48, "Horizon" },
            {49, "Master" },
            {50, "Battle2" },
            {51, "Rinoa" },
            {52, "Trabia" },
            {53, "Horizon2" },
            {54, "Truth" },
            {55, "Prison" },
            {56, "GalbadiaGarden" },
            {57, "Timber" },
            {58, "Galbadia " },
            {59, "Pinchi" },
            {60, "Scene1" },
            {61, "Pub" },
            {62, "Bat3" },
            {63, "Stage" },
            {64, "Choco" },
            {65, "White" },
            {66, "Majomv" },
            {67, "Musho" },
            {68, "Missile" },
            {69, "Speech" },
            {70, "Card" },
            {71, "Gomon" },
            {72, "Soto" },
            {73, "Majobat" },
            {74, "Train" },
            {75, "Garden2" },
            {76, "Bossbat2" },
            {77, "LastDungeon" },
            {78, "Gafly" },
            {79, "Demo" },
            {80, "Spy" },
            {81, "VoiceDeChocobo" },
            {82, "Salt" },
            {83, "Alien" },
            {84, "Sekichu" },
            {85, "Esta" },
            {86, "Moonmv" },
            {87, "Mdmotor" },
            {88, "Moonmv2" },
            {89, "Fly" },
            {90, "BossBat1" },
            {91, "Rag1" },
            {92, "Rag2" },
            {93, "LastBoss" },
            {94, "Lastwhite" },
            {95, "Lasbl" },
            {96, "Keisho" },
            {97, "Compression" },
        };


        #endregion

        #region MainMenuData
        public static string[] MainMenuLines =
        {
            "NEW GAME",
            "Continue",
            "DEBUG!"
        };
        #endregion

        public static class Archives
        {
            public const string A_BATTLE = "Content/FF8/battle";
            public const string A_FIELD = "Content/FF8/field";
            public const string A_MAGIC = "Content/FF8/magic";
            public const string A_MAIN = "Content/FF8/main";
            public const string A_MENU = "Content/FF8/menu";
            public const string A_WORLD = "Content/FF8/world";

            public const string B_FileList = ".fl";
            public const string B_FileIndex = ".fi";
            public const string B_FileArchive = ".fs";
        }

        public static class FieldHolder
        {
            //public static string[] MapList;
            public static UInt16 FieldID = 65535;
        }
    }
}
