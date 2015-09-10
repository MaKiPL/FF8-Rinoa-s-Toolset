using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing;
using System.Collections;

namespace SerahToolkit_SharpGL
{
    class BattleStage
    {
        private int TIM;
        private Byte[] Stage;
        private int pass;
        private int passOK;
        private int index;
        private List<int> itemOffsets;
        private string pathh;

        private int triangles;
        private int quads;
        private int verts;
        private int AbsolutePolygon;
        private int width;
        private int height;


        private byte U1;
        private byte U2;
        private byte U3;
        private byte U4;

        private int U5;
        private int T5;

        private byte V1;
        private byte V2;
        private byte V3;
        private byte V4;

        private string v;
        private string tt;
        private string fv;

        private int QuadSTOP;
        private int TrisSTOP;
        private int ChangeSTOP;
        private int ChangeADD;

        private int QuadOffset;
        private int TriangleOffset;

        private int TextureDataInt;

        private int currRUN;

        private StageTexture st;
        private Bitmap bmp;

        private UInt16 U4u;

        private UInt16 CLUTsize;



        //CONST
        Byte[] TIMtexture = { 0x10, 0x00, 0x00, 0x00, 0x09 };
        Byte[] GEOM = { 0x01, 0x00, 0x01, 0x00 };


        public BattleStage(string path)
        {
            if (path != "UV")
            {
                pathh = path;
                Stage = File.ReadAllBytes(path);
            }

        }

        /*
        private void PrepareToMix(StageTexture st)
        {
            //Pętla musi być przez wszystkie listbox!! :/
            GetUVpoints(0, "t", 0); //TUTAJ

            SharpGLForm sgl = new SharpGLForm();
            int[] SegmentArray = sgl.ListOfSegments();

            for (int i = 0; i != SegmentArray.Length; i++)
            {
                Tuple<List<double>, List<double>> UV = GetUVpoints(SegmentArray[i], LastKnownPath, LastKnownTIM);
                List<Point> UV_point = new List<Point>();


                int clute = 1;

                string PathText = Path.GetDirectoryName(pathh);
                PathText = PathText + @"\" + Path.GetFileNameWithoutExtension(pathh) + @"_" + clute + ".png";

                double U1Min = MemoryUV.Item1.Min(); double U1Max = MemoryUV.Item1.Max();
                double V1Min = MemoryUV.Item2.Min(); double V1Max = MemoryUV.Item2.Max();

                double X1 = (((U1Min * 100) * width) / 100);
                double Y1 = (((V1Min * 100) * height) / 100);
                double X2 = (((U1Max * 100) * width) / 100);
                double Y2 = (((V1Max * 100) * height) / 100);

                Point TopLeft = new Point((int)(Math.Round(X1)), 256 - (int)(Math.Round(Y2)));
                Point TopRight = new Point((int)(Math.Round(X2)), 256 - (int)(Math.Round(Y2)));
                Point BottomLeft = new Point((int)(Math.Round(X1)), 256 - (int)(Math.Round(Y1)));
                Point BottomRight = new Point((int)(Math.Round(X2)), 256 - (int)(Math.Round(Y1)));

                st.MixTextures(clute, TopLeft, TopRight, BottomLeft, BottomRight, PathText);
            }
        }

            */

        public void Process(bool generateTexture = false)
        {
            SearchObjects();
            SetupMTL();
            ResolveTex();
            st = new StageTexture(TIM, width, height);
            
                //Give texture to StageTexture class
                Byte[] TextureByte = new byte[Stage.Length - TextureDataInt];
                byte[] ClutByte = new byte[Stage.Length - (TextureDataInt - TIM)];
                Buffer.BlockCopy(Stage, TextureDataInt, TextureByte, 0, TextureByte.Length);
                Buffer.BlockCopy(Stage, TIM, ClutByte, 0, TextureDataInt-TIM);


            string PathOFD;
            if (generateTexture)
            {
                for (int i = CLUTsize-1; i > 0; i--)
                {
                        st.CreatePalettedTEX(i, ClutByte);
                        st.CopyTextureBuffer(TextureByte);
                        bmp = st.GetBMP();
                        PathOFD = Path.GetDirectoryName(pathh);
                        bmp.Save(PathOFD + @"\" + Path.GetFileNameWithoutExtension(pathh) + @"_" + i.ToString() + ".png", System.Drawing.Imaging.ImageFormat.Png);
                    

                }
            }
                st.CreatePalettedTEX(0, ClutByte);
                st.CopyTextureBuffer(TextureByte);
                bmp = st.GetBMP();
                PathOFD = Path.GetDirectoryName(pathh);
                bmp.Save(PathOFD + @"\" + Path.GetFileNameWithoutExtension(pathh) + ".png", System.Drawing.Imaging.ImageFormat.Png);
            


            //if (generateTexture)
              //  PrepareToMix(st);

            foreach(int off in itemOffsets)
            {
                verts = BitConverter.ToUInt16(Stage, off + 4);
                AbsolutePolygon = off + 6 + (verts * 6);
                triangles = BitConverter.ToUInt16(Stage, AbsolutePolygon + 4 + (AbsolutePolygon % 4));
                quads = BitConverter.ToUInt16(Stage, AbsolutePolygon + 6 + (AbsolutePolygon % 4));

                if (triangles != 0 && quads != 0)  //HEY!
                {
                    Process_Step(true, off);
                    Process_Step(false, off);
                }
                else
                    if (triangles != 0)
                        Process_Step(true, off);
                    else
                        if (quads != 0)
                            Process_Step(false, off);

            }

        }

        private void ResolveTex()
        {
            // Source: VT Calc v.1.3 //13-07-2015
            // Modified, faster code now... :)
            int TIMoffsetCLUTetc = TIM + 18;
            CLUTsize = BitConverter.ToUInt16(Stage, TIMoffsetCLUTetc);
            //DETERMINE HOW MUCH TO PASS
            TIMoffsetCLUTetc += 2 + (CLUTsize * 512) + 8;
            TextureDataInt = TIMoffsetCLUTetc + 4;
            UInt16 szerU = BitConverter.ToUInt16(Stage, TIMoffsetCLUTetc);
            UInt16 wysoU = BitConverter.ToUInt16(Stage, TIMoffsetCLUTetc + 2);
            width = szerU * 2;
            height = wysoU;
        }

        private void Process_Step(bool bTriangle,int off)
        {
            //wipe data - Extremely MANDATORY!
            v = null;
            tt = null;
            fv = null;
            //Modified FF8 Stage to OBJ converter - ProcessQuad(int B=0) function... 

            //Step 1 Open stream writer to sfd
            string PathOFD = Path.GetDirectoryName(pathh);
            if (bTriangle)
                PathOFD += string.Format(@"\{0}_{1}_t.obj", Path.GetFileNameWithoutExtension(pathh), off.ToString());
            else
                PathOFD += string.Format(@"\{0}_{1}_q.obj", Path.GetFileNameWithoutExtension(pathh), off.ToString());

            if (File.Exists(PathOFD))
                File.Delete(PathOFD);

            StreamWriter sw = new StreamWriter(PathOFD);

            //STEP 2 - Header
            sw.WriteLine(@"#Made with Serah toolset by MaKiPL. Hit me up at makipol@gmail.com <3 :*");
            sw.WriteLine(@"mtllib " + Path.GetFileNameWithoutExtension(pathh) + ".mtl");
            sw.WriteLine(@"usemtl Textured");
            sw.WriteLine("");

            //LET THE RIP BEGIN!

            /*
             * 
             * VARIABLES SETUP
             * 
             */

            int VertexOffset = off + 6;



            //STEP 1 - Calculate vertices

            int VertSTOP = VertexOffset + (6 * verts);
            int LoopIndexV = VertexOffset;

            while (true)
            {
                short X = (BitConverter.ToInt16(Stage, LoopIndexV));
                short Y = (BitConverter.ToInt16(Stage, LoopIndexV + 2));
                short Z = (BitConverter.ToInt16(Stage, LoopIndexV + 4));
                float Xa = X / 2000.0f; 
                float Ya = Y / 2000.0f;
                float Za = Z / 2000.0f;
                String Vline = string.Format("v {0} {1} {2}", Xa, Ya, Za) + Environment.NewLine;
                v += Vline;
                v = v.Replace(',', '.');
                LoopIndexV += 6;
                if (LoopIndexV == VertSTOP)
                    break;
            }
            //STEP 2 - Calculate VT

            if (bTriangle)
            {
                TriangleOffset = AbsolutePolygon + 12 + (AbsolutePolygon % 4);
                currRUN = TriangleOffset + 6;
                TrisSTOP = currRUN + (triangles * 20);
            }
            else
            {
                if (triangles != 0)
                {
                    QuadOffset = AbsolutePolygon + 12 + (AbsolutePolygon % 4) + triangles * 20;
                }
                else
                {
                    QuadOffset = AbsolutePolygon + 12 + (AbsolutePolygon % 4);
                }

                currRUN = QuadOffset + 8;
                QuadSTOP = (currRUN + (quads * 24));
                TrisSTOP = (currRUN + (triangles * 20));
            }


            while (true)
            {
                U1 = Stage[currRUN];
                V1 = Stage[currRUN + 1];
                if (bTriangle)
                {
                    U2 = Stage[currRUN + 2];
                    V2 = Stage[currRUN + 3];
                    U3 = Stage[currRUN + 6];
                    V3 = Stage[currRUN + 7];
                }
                else
                {
                    U2 = Stage[currRUN + 4];
                    V2 = Stage[currRUN + 5];
                    U3 = Stage[currRUN + 8];
                    V3 = Stage[currRUN + 9];
                    U4 = Stage[currRUN + 10];
                    V4 = Stage[currRUN + 11];
                }

                //Get Bit
                int Add_86;
                if (bTriangle)
                    Add_86 = 8;
                else
                    Add_86 = 6;
                string StrByte = Stage[currRUN + Add_86].ToString("X2");
                StrByte = "0" + StrByte.Substring(1);
                Byte TPage = Byte.Parse(StrByte);
                int TPageINT = TPage * 128;

                float UU1 = (float)U1 / (float)width + ((float)TPageINT / width);
                float VV1 = 1.0f - (V1 / 256.0f);
                float UU2 = (float)U2 / (float)width + ((float)TPageINT / width);
                float VV2 = 1.0f - (V2 / 256.0f);
                float UU3 = (float)U3 / (float)width + ((float)TPageINT / width);
                float VV3 = 1.0f - ((float)V3 / 256.0f);
                tt += string.Format("vt {0} {1}" + Environment.NewLine, UU1, VV1);
                tt += string.Format("vt {0} {1}" + Environment.NewLine, UU2, VV2);
                tt += string.Format("vt {0} {1}" + Environment.NewLine, UU3, VV3);
                if (!bTriangle)
                {
                    float UU4 = (float)U4 / (float)width + ((float)TPageINT / width);
                    float VV4 = 1.0f - ((float)V4 / 256.0f);
                    tt += string.Format("vt {0} {1}" + Environment.NewLine, UU4, VV4);
                }

                if(bTriangle)
                {
                    ChangeSTOP = TrisSTOP - 20;
                    ChangeADD = 20;
                }
                else
                {
                    ChangeSTOP = QuadSTOP - 24;
                    ChangeADD = 24;
                }
                if (currRUN == ChangeSTOP)
                {
                    break;
                }
                else
                    currRUN += ChangeADD;
            }


            v = v.Replace(',', '.');
            tt = tt.Replace(',', '.');

            //STEP 3 - Face indices + VT
            int FaceIndex = 1;
            int FaceQuad;
            if (bTriangle)
                FaceQuad = TriangleOffset;
            else
                FaceQuad = QuadOffset;
            int QuadSTOPq = 0;
            if (bTriangle)
                QuadSTOPq = TriangleOffset + (triangles * ChangeADD);
            else
                QuadSTOPq = QuadOffset + (quads * ChangeADD);

            fv = null;

            while (true)
            {
                UInt16 U1u = BitConverter.ToUInt16(Stage, FaceQuad);
                UInt16 U2u = BitConverter.ToUInt16(Stage, FaceQuad + 2);
                UInt16 U3u = BitConverter.ToUInt16(Stage, FaceQuad + 4);
                if(!bTriangle)
                    U4u = BitConverter.ToUInt16(Stage, FaceQuad + 6);

                int U1 = U1u + 1;
                int U2 = U2u + 1;
                int U3 = U3u + 1;
                if(!bTriangle)
                    U5 = U4u + 1;

                int T1 = FaceIndex;
                int T2 = FaceIndex + 1;
                int T3 = FaceIndex + 2;
                if(!bTriangle)
                    T5 = FaceIndex + 3;

                if(!bTriangle)
                    fv += string.Format("f {0}/{1} {2}/{3} {4}/{5} {6}/{7}" + Environment.NewLine, U1, T1, U2, T2, U5, T5, U3, T3);
                else
                    fv += string.Format("f {0}/{1} {2}/{3} {4}/{5}" + Environment.NewLine, U1, T2, U2, T3, U3, T1);

                if (FaceQuad == QuadSTOPq - ChangeADD)
                    break;
                else
                {
                    FaceQuad += ChangeADD;
                    if (bTriangle)
                        FaceIndex += 3;
                    else
                        FaceIndex += 4;
                }
            }



            sw.WriteLine(v);
            sw.WriteLine(tt);
            if (bTriangle)
                sw.WriteLine("g " + off.ToString() + "_t");
            else
                sw.WriteLine("g " + off.ToString() + "_q");
            sw.WriteLine(fv);
            sw.Close();
        }






        private void SetupMTL()
        {
            //Build Path
            string PathOFD = Path.GetDirectoryName(pathh);
            PathOFD += @"\" + Path.GetFileNameWithoutExtension(pathh) + ".MTL";

            if (File.Exists(PathOFD))
            {
                File.Delete(PathOFD);
            }
                //SETUP - check MTL
                //OFD HERE
                StreamWriter swe = new StreamWriter(PathOFD);
                swe.WriteLine("newmtl Textured");
                swe.WriteLine("Kd 1.000 1.000 1.000");
                swe.WriteLine("illum 2");
                swe.WriteLine("map_Kd " + Path.GetFileNameWithoutExtension(pathh) + ".png");
                swe.Close();

        }

        private void SearchObjects()
        {
            TIM = SearchForByte.ByteSearch(Stage, TIMtexture, 0);
            index = 0x5d4+1000;
            itemOffsets = new List<int>();
            while (true)
            {
                pass = SearchForByte.ByteSearch(Stage, GEOM, index);
                if (pass == -1)
                    break;
                else
                {
                    passOK = pass;
                    index = pass + 1;
                    if (passOK < TIM)
                    {
                        verts = BitConverter.ToUInt16(Stage, passOK + 4);
                        if (verts < 10000)
                        {
                            AbsolutePolygon = passOK + 6 + (verts * 6);
                            if (AbsolutePolygon < Stage.Length)
                            {
                                triangles = BitConverter.ToUInt16(Stage, AbsolutePolygon + 4 + (AbsolutePolygon % 4));
                                quads = BitConverter.ToUInt16(Stage, AbsolutePolygon + 6 + (AbsolutePolygon % 4));


                                if (triangles > 10000 || quads > 10000 || verts > 10000 || verts == 0)
                                {

                                }
                                else
                                    itemOffsets.Add(passOK);
                            }
                        }
                    }
                    else
                        break;
                }
            }

        }

        public int[] GetArrayOfObjects()
        {
            int[] Array = new int[itemOffsets.Count];

            if (itemOffsets != null)
            {
                Array = itemOffsets.ToArray();
            }
            return Array;
        }

        public Bitmap GetTexture()
        {
            return bmp;
        }

        public Tuple<List<double>,List<double>,int> GetUVpoints(int offset, string StagePath, int LastKnownTIM)
        {
            List<double> UV1 = new List<double>();
            List<double> UV2 = new List<double>();

            int CLUT = 0;
            Stage = File.ReadAllBytes(StagePath);
            int TIM = LastKnownTIM;
            int TIMoffsetCLUTetc = TIM + 18;
            UInt16 CLUTsize = BitConverter.ToUInt16(Stage, TIMoffsetCLUTetc);
            //DETERMINE HOW MUCH TO PASS
            TIMoffsetCLUTetc += 2 + (CLUTsize * 512) + 8;
            TextureDataInt = TIMoffsetCLUTetc + 4;
            UInt16 szerU = BitConverter.ToUInt16(Stage, TIMoffsetCLUTetc);
            UInt16 wysoU = BitConverter.ToUInt16(Stage, TIMoffsetCLUTetc + 2);
            width = szerU * 2;
            height = wysoU;

            verts = BitConverter.ToUInt16(Stage, offset + 4);
            AbsolutePolygon = offset + 6 + (verts * 6);
            triangles = BitConverter.ToUInt16(Stage, AbsolutePolygon + 4 + (AbsolutePolygon % 4));
            quads = BitConverter.ToUInt16(Stage, AbsolutePolygon + 6 + (AbsolutePolygon % 4));

            if (triangles != 0)
            {
                    TriangleOffset = AbsolutePolygon + 12 + (AbsolutePolygon % 4);
                    currRUN = TriangleOffset + 6;
                    TrisSTOP = currRUN + (triangles * 20);


                while (true)
                {
                    U1 = Stage[currRUN];
                    V1 = Stage[currRUN + 1];
                    U2 = Stage[currRUN + 2];
                    V2 = Stage[currRUN + 3];

                    byte[] clutBuff = new byte[2];
                    Buffer.BlockCopy(Stage, currRUN + 4, clutBuff, 0, 2);
                    CLUT = ResolveCLUT(clutBuff);

                    U3 = Stage[currRUN + 6];
                    V3 = Stage[currRUN + 7];

                    //Get Bit
                    string StrByte = Stage[currRUN + 8].ToString("X2");
                    StrByte = "0" + StrByte.Substring(1);
                    Byte TPage = Byte.Parse(StrByte);
                    int TPageINT = TPage * 128;

                    double UU1 = (float)U1 / (float)width + ((float)TPageINT / width);
                    double VV1 = 1.0f - (V1 / 256.0f);
                    double UU2 = (float)U2 / (float)width + ((float)TPageINT / width);
                    double VV2 = 1.0f - (V2 / 256.0f);
                    double UU3 = (float)U3 / (float)width + ((float)TPageINT / width);
                    double VV3 = 1.0f - ((float)V3 / 256.0f);

                    /*
                    int A1 = Convert.ToInt32(Math.Ceiling(UU1 * 100));
                    int B1 = Convert.ToInt32(Math.Ceiling(VV1 * 100));
                    int A2 = Convert.ToInt32(Math.Ceiling(UU2 * 100));
                    int B2 = Convert.ToInt32(Math.Ceiling(VV2 * 100));
                    int A3 = Convert.ToInt32(Math.Ceiling(UU3 * 100));
                    int B3 = Convert.ToInt32(Math.Ceiling(VV3 * 100));*/

                    UV1.Add(UU1);
                    UV2.Add(VV1);
                    UV1.Add(UU2);
                    UV2.Add(VV2);
                    UV1.Add(UU3);
                    UV2.Add(VV3);

                        ChangeSTOP = TrisSTOP - 20;
                        ChangeADD = 20;

                    if (currRUN == ChangeSTOP)
                    {
                        break;
                    }
                    else
                        currRUN += ChangeADD;
                }
            }
            currRUN = 0;
            if (quads != 0)
            {
                if (triangles != 0)
                {
                    QuadOffset = AbsolutePolygon + 12 + (AbsolutePolygon % 4) + triangles * 20;
                }
                else
                {
                    QuadOffset = AbsolutePolygon + 12 + (AbsolutePolygon % 4);
                }

                currRUN = QuadOffset + 8;
                QuadSTOP = (currRUN + (quads * 24));
                TrisSTOP = (currRUN + (triangles * 20));


                while (true)
                {
                    U1 = Stage[currRUN];
                    V1 = Stage[currRUN + 1];
                    byte[] clutBuff = new byte[2];
                    Buffer.BlockCopy(Stage, currRUN + 2, clutBuff, 0, 2);
                    CLUT = ResolveCLUT(clutBuff);
                    U2 = Stage[currRUN + 4];
                    V2 = Stage[currRUN + 5];
                    U3 = Stage[currRUN + 8];
                    V3 = Stage[currRUN + 9];
                    U4 = Stage[currRUN + 10];
                    V4 = Stage[currRUN + 11];

                    //Get Bit
                    string StrByte = Stage[currRUN + 6].ToString("X2");
                    StrByte = "0" + StrByte.Substring(1);
                    Byte TPage = Byte.Parse(StrByte);
                    int TPageINT = TPage * 128;

                    double UU1 = (float)U1 / (float)width + ((float)TPageINT / width);
                    double VV1 = 1.0f - (V1 / 256.0f);
                    double UU2 = (float)U2 / (float)width + ((float)TPageINT / width);
                    double VV2 = 1.0f - (V2 / 256.0f);
                    double UU3 = (float)U3 / (float)width + ((float)TPageINT / width);
                    double VV3 = 1.0f - ((float)V3 / 256.0f);
                    double UU4 = (float)U4 / (float)width + ((float)TPageINT / width);
                    double VV4 = 1.0f - ((float)V4 / 256.0f);
                    /*
                                        int A1 = Convert.ToInt32(Math.Ceiling(UU1 * 100));
                                        int B1 = Convert.ToInt32(Math.Ceiling(VV1 * 100));
                                        int A2 = Convert.ToInt32(Math.Ceiling(UU2 * 100));
                                        int B2 = Convert.ToInt32(Math.Ceiling(VV2 * 100));
                                        int A3 = Convert.ToInt32(Math.Ceiling(UU3 * 100));
                                        int B3 = Convert.ToInt32(Math.Ceiling(VV3 * 100));
                                        int A4 = Convert.ToInt32(Math.Ceiling(UU4 * 100));
                                        int B4 = Convert.ToInt32(Math.Ceiling(VV4 * 100)); 
                                        */

                    UV1.Add(UU1);
                    UV2.Add(VV1);
                    UV1.Add(UU2);
                    UV2.Add(VV2);
                    UV1.Add(UU3);
                    UV2.Add(VV3);
                    UV1.Add(UU4);
                    UV2.Add(VV4);

                    ChangeSTOP = QuadSTOP - 24;
                    ChangeADD = 24;

                    if (currRUN == ChangeSTOP)
                    {
                        break;
                    }
                    else
                        currRUN += ChangeADD;
                }
            }


            Tuple< List<double>, List<double>,int> tupRet= new Tuple<List<double>, List<double>,int>(UV1, UV2, CLUT);
            //MemoryUV = new Tuple<List<double>, List<double>>(UV1, UV2);
            return tupRet;
        }

        public int GetLastTIM()
        {
            return TIM;
        }

        public Tuple<int,int> GetTextureRes()
        {
            Tuple<int, int> TextureSize_Tuple = new Tuple<int, int>(width, height);
            return TextureSize_Tuple;
        }

        private int ResolveCLUT(byte[] buffer)
        {
            byte[] bt = new byte[2];
            Buffer.BlockCopy(buffer, 1, bt, 0, 1); //00 
            Buffer.BlockCopy(buffer, 0, bt, 1, 1); //3C
            BitArray ba = new BitArray(bt);
            BitArray CLUTbit = new BitArray(4);
            CLUTbit[3] = ba[1]; CLUTbit[2] = ba[0];
            CLUTbit[1] = ba[15]; CLUTbit[0] = ba[14];

            int[] ClutArray = new int[1];
            CLUTbit.CopyTo(ClutArray, 0);
            return ClutArray[0];

        }

        public void DumpRAW(int offset, string SavePath, int NextOffset = -1)
        {
            if(NextOffset == -1)
            {
                NextOffset = SearchForByte.ByteSearch(TIMtexture, Stage, offset);
            }

            Byte[] rawData = new byte[NextOffset - offset];
            Buffer.BlockCopy(Stage, offset, rawData, 0, NextOffset - offset);
            File.WriteAllBytes(SavePath, rawData);
        }
    }
}
