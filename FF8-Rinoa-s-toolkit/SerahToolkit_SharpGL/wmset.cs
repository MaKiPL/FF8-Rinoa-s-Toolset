using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections;
using System.Linq; //FOR DEBUG ONLY DATA ANALIZE
using System.Windows.Forms; //FOR DEBUG ONLY DISPLAY

/*
Marcin Gomulak aka MaKiPL (13-08-2015)
    */


namespace SerahToolkit_SharpGL
{
    class wmset
    {
        private string path;
        private string path_Sec42;
        private readonly byte[] EOF_b = { 0x00, 0x00, 0x00, 0x00 };
        private int CURRrun;

        private int TextureStartINT;
        private byte[] SEC42;
        private Bitmap bmp;

        private ColorPalette cp;

        private const int Color_8BPP = 255;
        private const int Color_4BPP = 15;
        private const float ColorReal = 8.2580645129032f;

        public const int d_SizeHeader = 0xC4-4;
        public const int d_OffsetCount = d_SizeHeader / 4; //48?

        private int width;
        private int height;
        private UInt16 paletteX;
        private UInt16 paletteY;
        private UInt16 imageX;
        private UInt16 imageY;
        private UInt16 hardcoded_clut_size;

        private float UU1;
        private float VV1;
        private float UU2;
        private float VV2;
        private float UU3;
        private float VV3;
        private float UU4;
        private float VV4;

        

        //debug
        public bool bDebugInfo = false; //Change me to use specific messageBox with additional info
        private List<byte> U;
        private List<byte> V;  
        

        public wmset(string path)
        {
            this.path = path;
            path_Sec42 = this.path.Substring(0, path.Length - 2) + "42";
        }


        public Tuple<List<string>,List<int>> _Debug_GetSections()
        {
            byte[] debug_b = new byte[d_SizeHeader];
            using (var fs = new FileStream(path, FileMode.Open))
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(debug_b, 0, d_SizeHeader);
                fs.Close();
            }
            List<string> ret = new List<string>();
            List<int> ret2 = new List<int>();
            
            for(int i = 0; i!= d_OffsetCount; i++)
            {
                ret.Add(string.Format("Section {0}: {1}", (i+1).ToString(), BitConverter.ToUInt32(debug_b, i * 4).ToString()));
                ret2.Add((int)BitConverter.ToUInt32(debug_b, i * 4));
            }


                return new Tuple<List<string>, List<int>>(ret, ret2);
        }


        /*
        *   SECTION 16 
        *   -----------
        *   MODELS
        *
        */

        public void Sector16(int whichModel, int index = 0)
        {
            SetupMTL(path, whichModel);
            Process(whichModel,index);
        }

        public UInt32[] ProduceOffset_sec16()
        {
            List<UInt32> OffsetList = new List<UInt32>();
            using (var fs = new FileStream(path, FileMode.Open))
            {
                byte[] temp = new byte[4];
                int index = 0;
                while (true)
                {
                    fs.Seek(index, SeekOrigin.Begin);
                    fs.Read(temp, 0, 4);
                    if (BitConverter.ToUInt32(temp,0) != BitConverter.ToUInt32(EOF_b,0))
                        OffsetList.Add(BitConverter.ToUInt32(temp, 0));
                    else
                        break;
                    index += 4;
                }
                fs.Close();
            }

            return OffsetList.ToArray();
        }

        private void SetupMTL(string pathh, int offset)
        {
            string PathOFD = Path.GetDirectoryName(pathh);
            PathOFD += @"\" + Path.GetFileNameWithoutExtension(pathh) + offset + ".MTL";

            if (File.Exists(PathOFD))
            {
                File.Delete(PathOFD);
            }
            StreamWriter swe = new StreamWriter(PathOFD);
            swe.WriteLine("newmtl Textured");
            swe.WriteLine("Kd 1.000 1.000 1.000");
            swe.WriteLine("illum 2");
            swe.WriteLine("map_Kd " + Path.GetFileNameWithoutExtension(path) + offset + ".png");
            swe.Close();

        }
        //SECTION 42
        private UInt32[] ProduceOffset_sec42()
        {
            List<UInt32> OffsetList = new List<UInt32>();
            using (var fs = new FileStream(path_Sec42, FileMode.Open))
            {
                byte[] temp = new byte[4];
                int index = 0;
                while (true)
                {
                    fs.Seek(index, SeekOrigin.Begin);
                    fs.Read(temp, 0, 4);
                    if (BitConverter.ToUInt32(temp, 0) != BitConverter.ToUInt32(EOF_b, 0))
                        OffsetList.Add(BitConverter.ToUInt32(temp, 0));
                    else
                        break;
                    index += 4;
                }
                fs.Close();
            }

            return OffsetList.ToArray();
        }
        //SECTION42 EOF

        private void Process(int offset, int texindex)
        {
            string F_t = null;
            string F_q = null;
            string vt = null;
            string vt_t = null;
            string v = null;
            CURRrun = 0;

            byte[] holdStage = File.ReadAllBytes(path);

            UInt16 triangle_count = BitConverter.ToUInt16(holdStage, offset);
            UInt16 quad_count = BitConverter.ToUInt16(holdStage, offset+2);
            UInt16 texture_page = BitConverter.ToUInt16(holdStage, offset+4);
            UInt16 vertex_count = BitConverter.ToUInt16(holdStage, offset+6);
            CURRrun = offset+8;


            uint[] _debug_preProduceOffset = ProduceOffset_sec42();
            int TextureOffset_inSec42 = (int)_debug_preProduceOffset[texindex];
            TextureStartINT = TextureOffset_inSec42;
            SEC42 = File.ReadAllBytes(path_Sec42);
            //Declare BITness
            byte BPP = SEC42[TextureOffset_inSec42 + 4];
            hardcoded_clut_size = SEC42[TextureOffset_inSec42 + 8];
            paletteX = SEC42[TextureOffset_inSec42 + 12];
            paletteY = SEC42[TextureOffset_inSec42 + 14]; //I should do the generic TIM reader... :C
            imageX = SEC42[TextureOffset_inSec42 + 12 + hardcoded_clut_size];
            imageY = SEC42[TextureOffset_inSec42 + 14 + hardcoded_clut_size];



            width = GetTextureDimension_ByBPP(BPP, TextureOffset_inSec42).Item1;
            height = GetTextureDimension_ByBPP(BPP, TextureOffset_inSec42).Item2;

            
            int startBuffer = TextureStartINT+20;

            int WhichBPP = BPP == 8 ? Color_4BPP : Color_8BPP;
            if (BPP == 9)
                bmp = new Bitmap(width, height, PixelFormat.Format8bppIndexed);
            else
                bmp = new Bitmap(width, height, PixelFormat.Format4bppIndexed);

            cp = bmp.Palette;

            for (int i = 0; i != WhichBPP-1; i++)
            {
                byte[] CLUTcolor = new byte[2];
                Buffer.BlockCopy(SEC42, startBuffer, CLUTcolor, 0, 2);
                BitArray ba = new BitArray(CLUTcolor);

                /*
                BitArray B = BPP == 8 ? new BitArray(4) : new BitArray(5);
                BitArray R = BPP == 8 ? new BitArray(4) : new BitArray(5);
                BitArray G = BPP == 8 ? new BitArray(4) : new BitArray(5);
                BitArray A = BPP == 8 ? new BitArray(4) : new BitArray(5);*/

                BitArray B = new BitArray(5);
                BitArray R = new BitArray(5);
                BitArray G = new BitArray(5);
                BitArray A = new BitArray(1);

                    B[0] = ba[10]; B[1] = ba[11]; B[2] = ba[12]; B[3] = ba[13]; B[4] = ba[14]; //R
                    R[0] = ba[0]; R[1] = ba[1]; R[2] = ba[2]; R[3] = ba[3]; R[4] = ba[4]; //G
                    G[0] = ba[5]; G[1] = ba[6]; G[2] = ba[7]; G[3] = ba[8]; G[4] = ba[9]; //B
                    A[0] = ba[15]; //Alpha if 0

                    /*
                    B[0] = ba[8]; B[1] = ba[9]; B[2] = ba[10]; B[3] = ba[11];
                    R[0] = ba[0]; R[1] = ba[1]; R[2] = ba[2]; R[3] = ba[3];
                    G[0] = ba[4]; G[1] = ba[5]; G[2] = ba[6]; G[3] = ba[7];
                    //A ba0123
                    */
                
                int[] b_ = new int[1]; B.CopyTo(b_, 0);  //b_[0] = (b_[0] * Color_color) / Color_MaxBit;
                int[] r_ = new int[1]; R.CopyTo(r_, 0); //r_[0] = (r_[0] * Color_color) / Color_MaxBit;
                int[] g_ = new int[1]; G.CopyTo(g_, 0); //g_[0] = (g_[0] * Color_color) / Color_MaxBit;
                int[] a_ = new int[1]; A.CopyTo(a_, 0);

                int alpha = 255;

                double b = Math.Round((b_[0]) * ColorReal);
                double r = Math.Round((r_[0]) * ColorReal);
                double g = Math.Round((g_[0]) * ColorReal);

                if (b > 255)
                    b--;
                if (r > 255)
                    r--;
                if (g > 255)
                    g--;

                //if(BPP == 9)
                    cp.Entries[i] = Color.FromArgb(alpha, int.Parse(b.ToString()), int.Parse(g.ToString()), int.Parse(r.ToString()));
                /*else
                    cp.Entries[i] = Color.FromArgb(alpha, int.Parse(r.ToString()), int.Parse(g.ToString()), int.Parse(b.ToString()));*/
                startBuffer += 2;
            }


            int StartTexture = GetTextureDimension_ByBPP(BPP, TextureOffset_inSec42).Item3;

            bmp = new Bitmap(width, height, PixelFormat.Format32bppArgb);
            bmp.Palette = cp;
            int index = StartTexture;
            UInt16 Size = BitConverter.ToUInt16(SEC42, StartTexture - 12);
            Size -= 12;
            //int Pixels = width * height;

            int _debug_width = BPP == 8 ? width : width; //Hm? 

            if (BPP == 9)
            {
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < _debug_width; x++)
                    {
                        //Color palette = cp.Entries[index];

                        bmp.SetPixel(x, y, Color.FromArgb(255, cp.Entries[SEC42[index + 2]].B, cp.Entries[SEC42[index + 1]].G, cp.Entries[SEC42[index]].R));
                        //bmp.SetPixel(x, y, Color.FromArgb(255, SEC42[index + 2], SEC42[index + 1], SEC42[index]));
                        if (index < _debug_width * height - 4 + StartTexture)
                            index++;

                    }
                }
            }
            else
            {
                //BIT shit FUK U

                byte[] FukU_Bits = new byte[Size*2];
                Buffer.BlockCopy(SEC42, index, FukU_Bits, 0, Size);
                BitArray BMP_fukU = new BitArray(FukU_Bits);

                int ind = 0;
                for (int y = 0; y < height; y++)
                {
                    for (int x = 0; x < _debug_width; x++)
                    {
                        BitArray _r = new BitArray(4); 
                        BitArray _g = new BitArray(4);
                        BitArray _b = new BitArray(4);
                        _r[0] = BMP_fukU[ind]; _r[1] = BMP_fukU[1 + ind]; _r[2] = BMP_fukU[2 + ind]; _r[3] = BMP_fukU[3 + ind];
                        //_g[0] = BMP_fukU[4 + ind]; _g[1] = BMP_fukU[5 + ind]; _g[2] = BMP_fukU[6 + ind]; _g[3] = BMP_fukU[7 + ind];
                        //_b[0] = BMP_fukU[8 + ind]; _b[1] = BMP_fukU[9 + ind]; _b[2] = BMP_fukU[10 + ind]; _b[3] = BMP_fukU[11 + ind];
                        int[] R = new int[1]; _r.CopyTo(R, 0);
                        //int[] G = new int[1]; _g.CopyTo(G, 0);
                        //int[] B = new int[1]; _b.CopyTo(B, 0);


                        //bmp.SetPixel(x, y, Color.FromArgb(255, cp.Entries[R[0]].B, cp.Entries[G[0]].G, cp.Entries[B[0]].R));
                        bmp.SetPixel(x, y, Color.FromArgb(255, cp.Entries[R[0]].B, cp.Entries[R[0]].G, cp.Entries[R[0]].R));
                        if (ind < (_debug_width * height)*4)
                            ind+=4;

                        //HELLO DEAR SOURCE VIEWER!
                        /* Here is the story! The viewer drawn texture only to it's 1/4 of height
                        * I spent 1+ hour. ONE painful hour to watch code, try some wicked ideas
                        * And nothing. I said to myself, that I'd leave it for tomorrow
                        * Maybe something would get on my mind
                        * I wrote a bmp saver to file and configured .MTL to use. This time I opened mspaint
                        * to see at which pixel does it stop
                        * That pixel was y=16
                        * I also remembered "32768" - this was used somewhere.((32x64)*16(bits))
                        * >32768< and what is height*width? 8192
                        * 32768/8192 is...?
                        * FUKIN' FOUR.
                        * (_debug_width * height) * MOTHERFKNG FOUR
                        * Voila... I lost one hour, because I didn't add four.
                        * PS> I like cats.
                         */

                    }
                }
            }


            bmp.MakeTransparent(Color.Black);

            if (triangle_count != 0)
            {
                int a = CURRrun;
                U = new List<byte>();
                V = new List<byte>();
                if (imageX < 192 && BPP != 9)
                {
                    while (true)
                    {
                        U.Add(holdStage[a + 4]);
                        V.Add(holdStage[a + 5]);
                        U.Add(holdStage[a + 6]);
                        V.Add(holdStage[a + 7]);
                        U.Add(holdStage[a + 8]);
                        V.Add(holdStage[a + 9]);
                        if (a == offset + 8 + (triangle_count * 12) - 12)
                            break;
                        else
                            a += 12;
                    }
                }
                int c = 1;
                a = CURRrun;
                while (true)
                {
                    int U1 = holdStage[a] + 1;
                    int U2 = holdStage[a + 1] + 1;
                    int U3 = holdStage[a + 2] + 1;

                    if (imageX < 192 && BPP != 9)
                    {
                        UU1 = ((float)holdStage[a + 4] - U.Min()) / _debug_width;
                        VV1 = 1.0f - ((float)holdStage[a + 5] - imageY) / height;
                        UU2 = ((float)holdStage[a + 6] - U.Min()) / _debug_width;
                        VV2 = 1.0f - ((float)holdStage[a + 7] - imageY) / height;
                        UU3 = ((float)holdStage[a + 8] - U.Min()) / _debug_width;
                        VV3 = 1.0f - ((float)holdStage[a + 9] - imageY) / height;
                    }
                    else
                    {
                        UU1 = BPP == 9
                            ? (float) holdStage[a + 4]/_debug_width
                            : ((float) holdStage[a + 4] - (imageX - 192)*4)/_debug_width;
                        VV1 = BPP == 9
                            ? 1.0f - ((float) holdStage[a + 5]/height)
                            : 1.0f - ((float) holdStage[a + 5] - imageY)/height;
                        UU2 = BPP == 9
                            ? (float) holdStage[a + 6]/_debug_width
                            : ((float) holdStage[a + 6] - (imageX - 192)*4)/_debug_width;
                        VV2 = BPP == 9
                            ? 1.0f - ((float) holdStage[a + 7]/height)
                            : 1.0f - ((float) holdStage[a + 7] - imageY)/height;
                        UU3 = BPP == 9
                            ? (float) holdStage[a + 8]/_debug_width
                            : ((float) holdStage[a + 8] - (imageX - 192)*4)/_debug_width;
                        VV3 = BPP == 9
                            ? 1.0f - ((float) holdStage[a + 9]/height)
                            : 1.0f - ((float) holdStage[a + 9] - imageY)/height;
                    }

                    vt_t += "vt " + UU1 + " " + VV1 + Environment.NewLine;
                    vt_t += "vt " + UU2 + " " + VV2 + Environment.NewLine;
                    vt_t += "vt " + UU3 + " " + VV3 + Environment.NewLine;

                    F_t += string.Format("f {0}/{3} {1}/{4} {2}/{5}" + Environment.NewLine, U1, U2, U3, c, c+1, c+2);
                    vt_t = vt_t.Replace(",", ".");
                    c += 3;
                    if (a == offset + 8 + (triangle_count * 12) - 12)
                        break;
                    else
                    {
                        a += 12;
                    }
                }
                CURRrun = a + 12;
            }


            //TEX COORD END

            if (quad_count != 0)
            {
                int a = CURRrun;
                U = new List<byte>();
                V = new List<byte>();
                if (imageX < 192 && BPP != 9)
                {
                    while (true)
                    {
                        U.Add(holdStage[a + 4]);
                        V.Add(holdStage[a + 5]);
                        U.Add(holdStage[a + 6]);
                        V.Add(holdStage[a + 7]);
                        U.Add(holdStage[a + 8]);
                        V.Add(holdStage[a + 9]);
                        U.Add(holdStage[a + 10]);
                        V.Add(holdStage[a + 11]);
                        if (a == CURRrun + (quad_count * 16) - 16)
                            break;
                        else
                        {
                            a += 16;
                        }
                    }
                }
                a = CURRrun;
                int c = 1;
                while (true)
                {
                    int U1 = holdStage[a] + 1;
                    int U2 = holdStage[a + 1] + 1;
                    int U3 = holdStage[a + 2] + 1;
                    int U4 = holdStage[a + 3] + 1;

                    /*
                    float UU1 = BPP == 9 ? (float)holdStage[a + 4] / _debug_width : _debug_width != 128 ? (float)holdStage[a+4] > 64 ? (float)holdStage[a + 4] / (float)((_debug_width*2)) : (float)holdStage[a + 4] / ((_debug_width)) : (float)holdStage[a + 4] / _debug_width;
                    float VV1 = BPP == 9 ? 1.0f - ((float)holdStage[a + 5] / height) : (float)((_debug_width / 4) / 16) - (float)holdStage[a + 5] / height;
                    float UU2 = BPP == 9 ? (float)holdStage[a + 6] / _debug_width : _debug_width != 128 ? (float)holdStage[a + 6] > 64 ? (float)holdStage[a + 6] / (float)((_debug_width*2)) : (float)holdStage[a + 6] / ((_debug_width)) : (float)holdStage[a + 6] / _debug_width;
                    float VV2 = BPP == 9 ? 1.0f - ((float)holdStage[a + 7] / height) : (float)((_debug_width / 4) / 16) - (float)holdStage[a + 7] / height;
                    float UU3 = BPP == 9 ? (float)holdStage[a + 8] / _debug_width : _debug_width != 128 ? (float)holdStage[a + 8] > 64 ? (float)holdStage[a + 8] / (float)((_debug_width*2)) : (float)holdStage[a + 8] / ((_debug_width)) : (float)holdStage[a + 8] / _debug_width;
                    float VV3 = BPP == 9 ? 1.0f - ((float)holdStage[a + 9] / height) : (float)((_debug_width / 4) / 16) - (float)holdStage[a + 9] / height;
                    float UU4 = BPP == 9 ? (float)holdStage[a + 10] / _debug_width : _debug_width != 128 ? (float)holdStage[a + 10] > 64 ? (float)holdStage[a + 10] / (float)((_debug_width*2)) : (float)holdStage[a + 10] / ((_debug_width)) : (float)holdStage[a + 10] / _debug_width;
                    float VV4 = BPP == 9 ? 1.0f - ((float)holdStage[a + 11] / height) : (float)((_debug_width / 4) / 16) - (float)holdStage[a + 11] / height; */

                    if (imageX < 192 && BPP != 9)
                    {
                        UU1 = ((float) holdStage[a + 4] - U.Min())/_debug_width;

                        VV1 = 1.0f - ((float) holdStage[a + 5] - imageY)/height;

                        UU2 = ((float) holdStage[a + 6] - U.Min())/_debug_width;
                        
                        VV2 = 1.0f - ((float) holdStage[a + 7] - imageY)/height;
                        
                        UU3 = ((float) holdStage[a + 8] - U.Min())/_debug_width;
                        
                        VV3 = 1.0f - ((float) holdStage[a + 9] - imageY)/height;
                        
                        UU4 = ((float) holdStage[a + 10] - U.Min())/_debug_width;

                        VV4 = 1.0f - ((float) holdStage[a + 11] - imageY)/height;
                    }
                    else
                    {
                        UU1 = BPP == 9
                            ? (float) holdStage[a + 4]/_debug_width
                            : ((float) holdStage[a + 4] - (imageX - 192)*4)/_debug_width;
                        U.Add((holdStage[a + 4]));
                        VV1 = BPP == 9
                            ? 1.0f - ((float) holdStage[a + 5]/height)
                            : 1.0f - ((float) holdStage[a + 5] - imageY)/height;
                        V.Add((holdStage[a + 5]));
                        UU2 = BPP == 9
                            ? (float) holdStage[a + 6]/_debug_width
                            : ((float) holdStage[a + 6] - (imageX - 192)*4)/_debug_width;
                        U.Add((holdStage[a + 6]));
                        VV2 = BPP == 9
                            ? 1.0f - ((float) holdStage[a + 7]/height)
                            : 1.0f - ((float) holdStage[a + 7] - imageY)/height;
                        V.Add((holdStage[a + 7]));
                        UU3 = BPP == 9
                            ? (float) holdStage[a + 8]/_debug_width
                            : ((float) holdStage[a + 8] - (imageX - 192)*4)/_debug_width;
                        U.Add((holdStage[a + 8]));
                        VV3 = BPP == 9
                            ? 1.0f - ((float) holdStage[a + 9]/height)
                            : 1.0f - ((float) holdStage[a + 9] - imageY)/height;
                        V.Add((holdStage[a + 9]));
                        UU4 = BPP == 9
                            ? (float) holdStage[a + 10]/_debug_width
                            : ((float) holdStage[a + 10] - (imageX - 192)*4)/_debug_width;
                        U.Add((holdStage[a + 10]));
                        VV4 = BPP == 9
                            ? 1.0f - ((float) holdStage[a + 11]/height)
                            : 1.0f - ((float) holdStage[a + 11] - imageY)/height;
                        V.Add((holdStage[a + 11]));
                    }
                    F_q += string.Format("f {0}/{4} {1}/{5} {2}/{7} {3}/{6}" + Environment.NewLine, U1, U2, U4, U3, c, c + 1, c + 2, c + 3);
                        vt += "vt " + UU1 + " " + VV1 + Environment.NewLine;
                        vt += "vt " + UU2 + " " + VV2 + Environment.NewLine;
                        vt += "vt " + UU3 + " " + VV3 + Environment.NewLine;
                        vt += "vt " + UU4 + " " + VV4 + Environment.NewLine;
                        vt = vt.Replace(",", ".");
                        vt = vt.Replace("-", "");
                        c += 4;

                    if (a == CURRrun + (quad_count * 16) - 16)
                        break;
                    else
                    {
                        a += 16;
                    }
                }
                if (imageX < 192 && BPP != 9)
                {
                    
                }

                CURRrun = a + 16;
            }
            if(U != null && bDebugInfo)
                MessageBox.Show(string.Format("U:{0}{1},{2}{3}V:{4}{5},{6}{7}H:{8} W:{9} palX:{10} palY:{11} texX:{12} texY:{13}", Environment.NewLine, U.Min(), U.Max(), Environment.NewLine, Environment.NewLine, V.Min(), V.Max(), Environment.NewLine, height, width, paletteX, paletteY, imageX, imageY));

            int start = CURRrun;
            while(true)
            {
                float Xa = (BitConverter.ToInt16(holdStage, CURRrun)) / 100.0f;
                float Ya = (BitConverter.ToInt16(holdStage, CURRrun+2)) / 100.0f;
                float Za = (BitConverter.ToInt16(holdStage, CURRrun+4)) / 100.0f;


                v += string.Format("v {0} {1} {2}", Xa, Ya, Za) + Environment.NewLine;
                v = v.Replace(',', '.');
                CURRrun += 8;
                if (CURRrun == start + (vertex_count * 8))
                    break;
            }
            if (triangle_count != 0)
            {
                string pathofd = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + "_" + offset + "_t.obj";
                if (File.Exists(pathofd))
                    File.Delete(pathofd);
                using (var fs = new StreamWriter(pathofd))
                {
                    fs.WriteLine(@"#Made with Serah toolset by MaKiPL. Hit me up at makipol@gmail.com <3 :*");
                    fs.WriteLine(@"mtllib " + Path.GetFileNameWithoutExtension(path) + offset + ".mtl");
                    fs.WriteLine(@"usemtl Textured");
                    fs.WriteLine("");
                    fs.WriteLine(v + Environment.NewLine + vt_t + Environment.NewLine + F_t);
                    fs.Close();
                }

            }
            if(quad_count != 0)
            {
                string pathofd = Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + "_" + offset + "_q.obj";
                if (File.Exists(pathofd))
                    File.Delete(pathofd);
                using (var fs = new StreamWriter(pathofd))
                {
                    fs.WriteLine(@"#Made with Serah toolset by MaKiPL. Hit me up at makipol@gmail.com <3 :*");
                    fs.WriteLine(@"mtllib " + Path.GetFileNameWithoutExtension(path) + offset + ".mtl");
                    fs.WriteLine(@"usemtl Textured");
                    fs.WriteLine("");
                    fs.WriteLine(v + Environment.NewLine + vt + Environment.NewLine + F_q);
                    fs.Close();
                }
            }

            bmp.Save(Path.GetDirectoryName(path) + @"\" + Path.GetFileNameWithoutExtension(path) + offset +".png", System.Drawing.Imaging.ImageFormat.Png);

        }



        //EOF - SECTION 16-------------------------------------------------------------- 

        private Tuple<int,int, int> GetTextureDimension_ByBPP(int BPP, int TextureOffset_inSec42)
        {

            int TIMoffsetCLUTetc = TextureOffset_inSec42 + 18;
            int CLUTsize = BitConverter.ToUInt16(SEC42, TIMoffsetCLUTetc-2);
            CLUTsize = BPP == 8 ? CLUTsize / 16 : CLUTsize / 256;



            if (BPP == 8)
            {
                //4BPP - width * 4
                //4 bpp mode: Each byte contains 2 pixels (4 bits per pixel). Each 4 bits is a numeric refernce to the CLUT Data.

                TIMoffsetCLUTetc += 2 + (CLUTsize * 32) + 8;

            }
            if (BPP == 9)
            {
                //8BPP - width * 2
                //8 bpp mode: Each byte contains 1 pixel(8 bits per pixel).Each byte is a numeric refernce to the CLUT Data.

                TIMoffsetCLUTetc += 2 + (CLUTsize * 512) + 8;
            }

            int TextureDataInt = TIMoffsetCLUTetc + 4;
            UInt16 szerU = BitConverter.ToUInt16(SEC42, TIMoffsetCLUTetc);
            UInt16 wysoU = BitConverter.ToUInt16(SEC42, TIMoffsetCLUTetc + 2);

            int width;
            width = BPP == 8 ? szerU * 4 : szerU * 2; //Huh, that's really faster and better way :) //*4? 
            int height = wysoU;

            Tuple<int, int, int> ret = new Tuple<int, int, int>(width, height, TextureDataInt);
            return ret;
        }

        public Bitmap GetTexture() => bmp;
    }

    
}
