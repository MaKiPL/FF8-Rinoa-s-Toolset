using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.Linq; //FOR DEBUG ONLY DATA ANALIZE
using System.Windows.Forms; //FOR DEBUG ONLY DISPLAY

/*
Marcin Gomulak aka MaKiPL (13-08-2015)
    */


namespace SerahToolkit_SharpGL
{
    class Wmset
    {
        private readonly string _path;
        private readonly string _pathSec42;
        private readonly byte[] _eofB = { 0x00, 0x00, 0x00, 0x00 };
        private int _curRrun;

        private int _textureStartInt;
        private byte[] _sec42;
        private Bitmap _bmp;

        private ColorPalette _cp;

        private const int Color_8Bpp = 255;
        private const int Color_4Bpp = 15;
        private const float ColorReal = 256/32;

        private const int DSizeHeader = 0xC4-4;
        public const int DOffsetCount = DSizeHeader / 4; //48?

        private int _width;
        private int _height;
        private UInt16 _paletteX;
        private UInt16 _paletteY;
        private UInt16 _imageX;
        private UInt16 _imageY;
        private UInt16 _hardcodedClutSize;

        private float _uu1;
        private float _vv1;
        private float _uu2;
        private float _vv2;
        private float _uu3;
        private float _vv3;
        private float _uu4;
        private float _vv4;

        

        //debug
        private bool _bDebugInfo = false; //Change me to use specific messageBox with additional info
        private List<byte> _u;
        private List<byte> _v;  
        

        public Wmset(string path)
        {
            _path = path;
            _pathSec42 = _path.Substring(0, path.Length - 2) + "42";
        }


        public Tuple<List<string>,List<int>> _Debug_GetSections()
        {
            byte[] debugB = new byte[DSizeHeader];
            using (var fs = new FileStream(_path, FileMode.Open))
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(debugB, 0, DSizeHeader);
                fs.Close();
            }
            List<string> ret = new List<string>();
            List<int> ret2 = new List<int>();
            
            for(int i = 0; i!= DOffsetCount; i++)
            {
                ret.Add($"Section {(i + 1).ToString()}: {BitConverter.ToUInt32(debugB, i*4).ToString()}");
                ret2.Add((int)BitConverter.ToUInt32(debugB, i * 4));
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
            SetupMtl(_path, whichModel);
            Process(whichModel,index);
            Console.WriteLine($"WMSet: Done!");
        }

        public UInt32[] ProduceOffset_sec16()
        {
            Console.WriteLine($"WMSet: Producing offsets for section 16");
            List<UInt32> offsetList = new List<UInt32>();
            using (var fs = new FileStream(_path, FileMode.Open))
            {
                byte[] temp = new byte[4];
                int index = 0;
                while (true)
                {
                    fs.Seek(index, SeekOrigin.Begin);
                    fs.Read(temp, 0, 4);
                    if (BitConverter.ToUInt32(temp,0) != BitConverter.ToUInt32(_eofB,0))
                        offsetList.Add(BitConverter.ToUInt32(temp, 0));
                    else
                        break;
                    index += 4;
                }
                fs.Close();
            }

            return offsetList.ToArray();
        }

        private void SetupMtl(string pathh, int offset)
        {
            Console.WriteLine($"WMSet: Setting up MTL file");
            string pathOfd = Path.GetDirectoryName(pathh);
            pathOfd += @"\" + Path.GetFileNameWithoutExtension(pathh) + offset + ".MTL";

            if (File.Exists(pathOfd))
            {
                File.Delete(pathOfd);
            }
            StreamWriter swe = new StreamWriter(pathOfd);
            swe.WriteLine("newmtl Textured");
            swe.WriteLine("Kd 1.000 1.000 1.000");
            swe.WriteLine("illum 2");
            swe.WriteLine("map_Kd " + Path.GetFileNameWithoutExtension(_path) + offset + ".png");
            swe.Close();

        }
        //SECTION 42
        private UInt32[] ProduceOffset_sec42()
        {
            Console.WriteLine($"WMSet: Producing offsets for section 42");
            List<UInt32> offsetList = new List<UInt32>();
            using (var fs = new FileStream(_pathSec42, FileMode.Open))
            {
                byte[] temp = new byte[4];
                int index = 0;
                while (true)
                {
                    fs.Seek(index, SeekOrigin.Begin);
                    fs.Read(temp, 0, 4);
                    if (BitConverter.ToUInt32(temp, 0) != BitConverter.ToUInt32(_eofB, 0))
                        offsetList.Add(BitConverter.ToUInt32(temp, 0));
                    else
                        break;
                    index += 4;
                }
                fs.Close();
            }

            return offsetList.ToArray();
        }
        //SECTION42 EOF

        private void Process(int offset, int texindex)
        {
            Console.WriteLine($"WMSet: Starting process...");
            string fT = null;
            string fQ = null;
            string vt = null;
            string vtT = null;
            string v = null;
            _curRrun = 0;

            byte[] holdStage = File.ReadAllBytes(_path);

            UInt16 triangleCount = BitConverter.ToUInt16(holdStage, offset);
            UInt16 quadCount = BitConverter.ToUInt16(holdStage, offset+2);
            UInt16 texturePage = BitConverter.ToUInt16(holdStage, offset+4);
            UInt16 vertexCount = BitConverter.ToUInt16(holdStage, offset+6);
            _curRrun = offset+8;

            Console.WriteLine($"WMSet: TriangleCount: {triangleCount}, QuadCount: {quadCount}, TexturePage: {texturePage}, VerticesCount: {vertexCount}");

            uint[] debugPreProduceOffset = ProduceOffset_sec42();
            int textureOffsetInSec42 = (int)debugPreProduceOffset[texindex];
            _textureStartInt = textureOffsetInSec42;
            _sec42 = File.ReadAllBytes(_pathSec42);
            //Declare BITness
            byte bpp = _sec42[textureOffsetInSec42 + 4];
            _hardcodedClutSize = _sec42[textureOffsetInSec42 + 8];
            _paletteX = _sec42[textureOffsetInSec42 + 12];
            _paletteY = _sec42[textureOffsetInSec42 + 14]; //I should do the generic TIM reader... :C
            _imageX = _sec42[textureOffsetInSec42 + 12 + _hardcodedClutSize];
            _imageY = _sec42[textureOffsetInSec42 + 14 + _hardcodedClutSize];
            Console.WriteLine($"WMSet: Reading Texture...");
            Console.WriteLine($"WMSet: PaletteX:{_paletteX}, PaletteY:{_paletteY}, ImageX:{_imageX}, ImageY:{_imageY}");


            _width = GetTextureDimension_ByBPP(bpp, textureOffsetInSec42).Item1;
            _height = GetTextureDimension_ByBPP(bpp, textureOffsetInSec42).Item2;

            
            int startBuffer = _textureStartInt+20;
            Console.WriteLine($"WMSet: Texture start buffer");

            int whichBpp = bpp == 8 ? Color_4Bpp : Color_8Bpp;
            _bmp = bpp == 9 ? new Bitmap(_width, _height, PixelFormat.Format8bppIndexed) : new Bitmap(_width, _height, PixelFormat.Format4bppIndexed);
            Console.WriteLine($"WMSet: Texture BPP colors: {whichBpp}");
            _cp = _bmp.Palette;
            Console.WriteLine($"WMSet: Building palette data");
            for (int i = 0; i != whichBpp-1; i++)
            {
                byte[] cluTcolor = new byte[2];
                Buffer.BlockCopy(_sec42, startBuffer, cluTcolor, 0, 2);
                BitArray ba = new BitArray(cluTcolor);

                /*
                BitArray B = BPP == 8 ? new BitArray(4) : new BitArray(5);
                BitArray R = BPP == 8 ? new BitArray(4) : new BitArray(5);
                BitArray G = BPP == 8 ? new BitArray(4) : new BitArray(5);
                BitArray A = BPP == 8 ? new BitArray(4) : new BitArray(5);*/

                BitArray B = new BitArray(5);
                BitArray R = new BitArray(5);
                BitArray G = new BitArray(5);
                BitArray a = new BitArray(1);

                    B[0] = ba[10]; B[1] = ba[11]; B[2] = ba[12]; B[3] = ba[13]; B[4] = ba[14]; //R
                    R[0] = ba[0]; R[1] = ba[1]; R[2] = ba[2]; R[3] = ba[3]; R[4] = ba[4]; //G
                    G[0] = ba[5]; G[1] = ba[6]; G[2] = ba[7]; G[3] = ba[8]; G[4] = ba[9]; //B
                    a[0] = ba[15]; //Alpha if 0

                    /*
                    B[0] = ba[8]; B[1] = ba[9]; B[2] = ba[10]; B[3] = ba[11];
                    R[0] = ba[0]; R[1] = ba[1]; R[2] = ba[2]; R[3] = ba[3];
                    G[0] = ba[4]; G[1] = ba[5]; G[2] = ba[6]; G[3] = ba[7];
                    //A ba0123
                    */
                
                int[] b_ = new int[1]; B.CopyTo(b_, 0);  //b_[0] = (b_[0] * Color_color) / Color_MaxBit;
                int[] r_ = new int[1]; R.CopyTo(r_, 0); //r_[0] = (r_[0] * Color_color) / Color_MaxBit;
                int[] g_ = new int[1]; G.CopyTo(g_, 0); //g_[0] = (g_[0] * Color_color) / Color_MaxBit;
                int[] aa = new int[1]; a.CopyTo(aa, 0);

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
                    _cp.Entries[i] = Color.FromArgb(alpha, int.Parse(b.ToString(CultureInfo.InvariantCulture)), int.Parse(g.ToString(CultureInfo.InvariantCulture)), int.Parse(r.ToString(CultureInfo.InvariantCulture)));
                /*else
                    cp.Entries[i] = Color.FromArgb(alpha, int.Parse(r.ToString()), int.Parse(g.ToString()), int.Parse(b.ToString()));*/
                startBuffer += 2;
            }


            int startTexture = GetTextureDimension_ByBPP(bpp, textureOffsetInSec42).Item3;

            Console.WriteLine($"WMSet: Drawing texture...");
            _bmp = new Bitmap(_width, _height, PixelFormat.Format32bppArgb) {Palette = _cp};
            int index = startTexture;
            UInt16 size = BitConverter.ToUInt16(_sec42, startTexture - 12);
            size -= 12;
            //int Pixels = width * height;

            int debugWidth = _width; //Hm? 

            if (bpp == 9)
            {
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < debugWidth; x++)
                    {
                        //Color palette = cp.Entries[index];

                        _bmp.SetPixel(x, y, Color.FromArgb(255, _cp.Entries[_sec42[index]].B, _cp.Entries[_sec42[index]].G, _cp.Entries[_sec42[index]].R));
                        //bmp.SetPixel(x, y, Color.FromArgb(255, SEC42[index + 2], SEC42[index + 1], SEC42[index]));
                        if (index < debugWidth * _height - 4 + startTexture)
                            index++;

                    }
                }
            }
            else
            {
                //BIT shit FUK U

                byte[] fukUBits = new byte[size*2];
                Buffer.BlockCopy(_sec42, index, fukUBits, 0, size);
                BitArray bmpFukU = new BitArray(fukUBits);

                int ind = 0;
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < debugWidth; x++)
                    {
                        BitArray r = new BitArray(4); 
                        BitArray g = new BitArray(4);
                        BitArray b = new BitArray(4);
                        r[0] = bmpFukU[ind]; r[1] = bmpFukU[1 + ind]; r[2] = bmpFukU[2 + ind]; r[3] = bmpFukU[3 + ind];
                        //_g[0] = BMP_fukU[4 + ind]; _g[1] = BMP_fukU[5 + ind]; _g[2] = BMP_fukU[6 + ind]; _g[3] = BMP_fukU[7 + ind];
                        //_b[0] = BMP_fukU[8 + ind]; _b[1] = BMP_fukU[9 + ind]; _b[2] = BMP_fukU[10 + ind]; _b[3] = BMP_fukU[11 + ind];
                        int[] rr = new int[1]; r.CopyTo(rr, 0);
                        //int[] G = new int[1]; _g.CopyTo(G, 0);
                        //int[] B = new int[1]; _b.CopyTo(B, 0);


                        //bmp.SetPixel(x, y, Color.FromArgb(255, cp.Entries[R[0]].B, cp.Entries[G[0]].G, cp.Entries[B[0]].R));
                        _bmp.SetPixel(x, y, Color.FromArgb(255, _cp.Entries[rr[0]].B, _cp.Entries[rr[0]].G, _cp.Entries[rr[0]].R));
                        if (ind < (debugWidth * _height)*4)
                            ind+=4;
                    }
                }
            }

            Console.WriteLine($"WMSet: Drawing texture transparency");
            _bmp.MakeTransparent(Color.Black);

            Console.WriteLine($"WMSet: Reading UVs...");
            if (triangleCount != 0)
            {
                int a = _curRrun;
                _u = new List<byte>();
                _v = new List<byte>();
                if (_imageX < 192 && bpp != 9)
                {
                    while (true)
                    {
                        _u.Add(holdStage[a + 4]);
                        _v.Add(holdStage[a + 5]);
                        _u.Add(holdStage[a + 6]);
                        _v.Add(holdStage[a + 7]);
                        _u.Add(holdStage[a + 8]);
                        _v.Add(holdStage[a + 9]);
                        if (a == offset + 8 + (triangleCount * 12) - 12)
                            break;
                        else
                            a += 12;
                    }
                }
                int c = 1;
                a = _curRrun;
                Console.WriteLine($"WMSet: Calculating UV wireframe for Wavefront OBJ");
                while (true)
                {
                    int u1 = holdStage[a] + 1;
                    int u2 = holdStage[a + 1] + 1;
                    int u3 = holdStage[a + 2] + 1;

                    if (_imageX < 192 && bpp != 9)
                    {
                        _uu1 = ((float)holdStage[a + 4] - _u.Min()) / debugWidth;
                        _vv1 = 1.0f - ((float)holdStage[a + 5] - _imageY) / _height;
                        _uu2 = ((float)holdStage[a + 6] - _u.Min()) / debugWidth;
                        _vv2 = 1.0f - ((float)holdStage[a + 7] - _imageY) / _height;
                        _uu3 = ((float)holdStage[a + 8] - _u.Min()) / debugWidth;
                        _vv3 = 1.0f - ((float)holdStage[a + 9] - _imageY) / _height;
                    }
                    else
                    {
                        _uu1 = bpp == 9
                            ? (float) holdStage[a + 4]/debugWidth
                            : ((float) holdStage[a + 4] - (_imageX - 192)*4)/debugWidth;
                        _vv1 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 5]/_height)
                            : 1.0f - ((float) holdStage[a + 5] - _imageY)/_height;
                        _uu2 = bpp == 9
                            ? (float) holdStage[a + 6]/debugWidth
                            : ((float) holdStage[a + 6] - (_imageX - 192)*4)/debugWidth;
                        _vv2 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 7]/_height)
                            : 1.0f - ((float) holdStage[a + 7] - _imageY)/_height;
                        _uu3 = bpp == 9
                            ? (float) holdStage[a + 8]/debugWidth
                            : ((float) holdStage[a + 8] - (_imageX - 192)*4)/debugWidth;
                        _vv3 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 9]/_height)
                            : 1.0f - ((float) holdStage[a + 9] - _imageY)/_height;
                    }

                    vtT += "vt " + _uu1 + " " + _vv1 + Environment.NewLine;
                    vtT += "vt " + _uu2 + " " + _vv2 + Environment.NewLine;
                    vtT += "vt " + _uu3 + " " + _vv3 + Environment.NewLine;

                    fT += string.Format("f {0}/{3} {1}/{4} {2}/{5}" + Environment.NewLine, u1, u2, u3, c, c+1, c+2);
                    vtT = vtT.Replace(",", ".");
                    c += 3;
                    if (a == offset + 8 + (triangleCount * 12) - 12)
                        break;
                    else
                    {
                        a += 12;
                    }
                }
                _curRrun = a + 12;
            }


            //TEX COORD END
            Console.WriteLine($"WMSet: Processing quads");
            if (quadCount != 0)
            {
                int a = _curRrun;
                _u = new List<byte>();
                _v = new List<byte>();
                if (_imageX < 192 && bpp != 9)
                {
                    while (true)
                    {
                        _u.Add(holdStage[a + 4]);
                        _v.Add(holdStage[a + 5]);
                        _u.Add(holdStage[a + 6]);
                        _v.Add(holdStage[a + 7]);
                        _u.Add(holdStage[a + 8]);
                        _v.Add(holdStage[a + 9]);
                        _u.Add(holdStage[a + 10]);
                        _v.Add(holdStage[a + 11]);
                        if (a == _curRrun + (quadCount * 16) - 16)
                            break;
                        else
                        {
                            a += 16;
                        }
                    }
                }
                a = _curRrun;
                int c = 1;
                while (true)
                {
                    int u1 = holdStage[a] + 1;
                    int u2 = holdStage[a + 1] + 1;
                    int u3 = holdStage[a + 2] + 1;
                    int u4 = holdStage[a + 3] + 1;

                    /*
                    float UU1 = BPP == 9 ? (float)holdStage[a + 4] / _debug_width : _debug_width != 128 ? (float)holdStage[a+4] > 64 ? (float)holdStage[a + 4] / (float)((_debug_width*2)) : (float)holdStage[a + 4] / ((_debug_width)) : (float)holdStage[a + 4] / _debug_width;
                    float VV1 = BPP == 9 ? 1.0f - ((float)holdStage[a + 5] / height) : (float)((_debug_width / 4) / 16) - (float)holdStage[a + 5] / height;
                    float UU2 = BPP == 9 ? (float)holdStage[a + 6] / _debug_width : _debug_width != 128 ? (float)holdStage[a + 6] > 64 ? (float)holdStage[a + 6] / (float)((_debug_width*2)) : (float)holdStage[a + 6] / ((_debug_width)) : (float)holdStage[a + 6] / _debug_width;
                    float VV2 = BPP == 9 ? 1.0f - ((float)holdStage[a + 7] / height) : (float)((_debug_width / 4) / 16) - (float)holdStage[a + 7] / height;
                    float UU3 = BPP == 9 ? (float)holdStage[a + 8] / _debug_width : _debug_width != 128 ? (float)holdStage[a + 8] > 64 ? (float)holdStage[a + 8] / (float)((_debug_width*2)) : (float)holdStage[a + 8] / ((_debug_width)) : (float)holdStage[a + 8] / _debug_width;
                    float VV3 = BPP == 9 ? 1.0f - ((float)holdStage[a + 9] / height) : (float)((_debug_width / 4) / 16) - (float)holdStage[a + 9] / height;
                    float UU4 = BPP == 9 ? (float)holdStage[a + 10] / _debug_width : _debug_width != 128 ? (float)holdStage[a + 10] > 64 ? (float)holdStage[a + 10] / (float)((_debug_width*2)) : (float)holdStage[a + 10] / ((_debug_width)) : (float)holdStage[a + 10] / _debug_width;
                    float VV4 = BPP == 9 ? 1.0f - ((float)holdStage[a + 11] / height) : (float)((_debug_width / 4) / 16) - (float)holdStage[a + 11] / height; */

                    if (_imageX < 192 && bpp != 9)
                    {
                        _uu1 = ((float) holdStage[a + 4] - _u.Min())/debugWidth;

                        _vv1 = 1.0f - ((float) holdStage[a + 5] - _imageY)/_height;

                        _uu2 = ((float) holdStage[a + 6] - _u.Min())/debugWidth;
                        
                        _vv2 = 1.0f - ((float) holdStage[a + 7] - _imageY)/_height;
                        
                        _uu3 = ((float) holdStage[a + 8] - _u.Min())/debugWidth;
                        
                        _vv3 = 1.0f - ((float) holdStage[a + 9] - _imageY)/_height;
                        
                        _uu4 = ((float) holdStage[a + 10] - _u.Min())/debugWidth;

                        _vv4 = 1.0f - ((float) holdStage[a + 11] - _imageY)/_height;
                    }
                    else
                    {
                        _uu1 = bpp == 9
                            ? (float) holdStage[a + 4]/debugWidth
                            : ((float) holdStage[a + 4] - (_imageX - 192)*4)/debugWidth;
                        _u.Add((holdStage[a + 4]));
                        _vv1 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 5]/_height)
                            : 1.0f - ((float) holdStage[a + 5] - _imageY)/_height;
                        _v.Add((holdStage[a + 5]));
                        _uu2 = bpp == 9
                            ? (float) holdStage[a + 6]/debugWidth
                            : ((float) holdStage[a + 6] - (_imageX - 192)*4)/debugWidth;
                        _u.Add((holdStage[a + 6]));
                        _vv2 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 7]/_height)
                            : 1.0f - ((float) holdStage[a + 7] - _imageY)/_height;
                        _v.Add((holdStage[a + 7]));
                        _uu3 = bpp == 9
                            ? (float) holdStage[a + 8]/debugWidth
                            : ((float) holdStage[a + 8] - (_imageX - 192)*4)/debugWidth;
                        _u.Add((holdStage[a + 8]));
                        _vv3 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 9]/_height)
                            : 1.0f - ((float) holdStage[a + 9] - _imageY)/_height;
                        _v.Add((holdStage[a + 9]));
                        _uu4 = bpp == 9
                            ? (float) holdStage[a + 10]/debugWidth
                            : ((float) holdStage[a + 10] - (_imageX - 192)*4)/debugWidth;
                        _u.Add((holdStage[a + 10]));
                        _vv4 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 11]/_height)
                            : 1.0f - ((float) holdStage[a + 11] - _imageY)/_height;
                        _v.Add((holdStage[a + 11]));
                    }
                    fQ += string.Format("f {0}/{4} {1}/{5} {2}/{7} {3}/{6}" + Environment.NewLine, u1, u2, u4, u3, c, c + 1, c + 2, c + 3);
                        vt += "vt " + _uu1 + " " + _vv1 + Environment.NewLine;
                        vt += "vt " + _uu2 + " " + _vv2 + Environment.NewLine;
                        vt += "vt " + _uu3 + " " + _vv3 + Environment.NewLine;
                        vt += "vt " + _uu4 + " " + _vv4 + Environment.NewLine;
                        vt = vt.Replace(",", ".");
                        vt = vt.Replace("-", "");
                        c += 4;

                    if (a == _curRrun + (quadCount * 16) - 16)
                        break;
                    else
                    {
                        a += 16;
                    }
                }
                if (_imageX < 192 && bpp != 9)
                {
                    
                }

                _curRrun = a + 16;
            }
            if(_u != null && _bDebugInfo)
                MessageBox.Show(
                    $"U:{Environment.NewLine}{_u.Min()},{_u.Max()}{Environment.NewLine}V:{Environment.NewLine}{_v.Min()},{_v.Max()}{Environment.NewLine}H:{_height} W:{_width} palX:{_paletteX} palY:{_paletteY} texX:{_imageX} texY:{_imageY}");

            int start = _curRrun;
            Console.WriteLine($"WMSet: Reading vertices data");
            while (true)
            {
                float xa = (BitConverter.ToInt16(holdStage, _curRrun)) / 100.0f;
                float ya = (BitConverter.ToInt16(holdStage, _curRrun+2)) / 100.0f;
                float za = (BitConverter.ToInt16(holdStage, _curRrun+4)) / 100.0f;


                v += $"v {xa} {ya} {za}" + Environment.NewLine;
                v = v.Replace(',', '.');
                _curRrun += 8;
                if (_curRrun == start + (vertexCount * 8))
                    break;
            }
            if (triangleCount != 0)
            {
                string pathofd = Path.GetDirectoryName(_path) + @"\" + Path.GetFileNameWithoutExtension(_path) + "_" + offset + "_t.obj";
                if (File.Exists(pathofd))
                    File.Delete(pathofd);
                using (var fs = new StreamWriter(pathofd))
                {
                    fs.WriteLine(@"#Made with Rinoa's toolset by MaKiPL.");
                    fs.WriteLine(@"mtllib " + Path.GetFileNameWithoutExtension(_path) + offset + ".mtl");
                    fs.WriteLine(@"usemtl Textured");
                    fs.WriteLine("");
                    fs.WriteLine(v + Environment.NewLine + vtT + Environment.NewLine + fT);
                    fs.Close();
                }

            }
            if(quadCount != 0)
            {
                string pathofd = Path.GetDirectoryName(_path) + @"\" + Path.GetFileNameWithoutExtension(_path) + "_" + offset + "_q.obj";
                if (File.Exists(pathofd))
                    File.Delete(pathofd);
                using (var fs = new StreamWriter(pathofd))
                {
                    fs.WriteLine(@"#Made with Rinoa's toolset by MaKiPL.");
                    fs.WriteLine(@"mtllib " + Path.GetFileNameWithoutExtension(_path) + offset + ".mtl");
                    fs.WriteLine(@"usemtl Textured");
                    fs.WriteLine("");
                    fs.WriteLine(v + Environment.NewLine + vt + Environment.NewLine + fQ);
                    fs.Close();
                }
            }
            Console.WriteLine($"WMSet: Saving bitmap");
            _bmp.Save(Path.GetDirectoryName(_path) + @"\" + Path.GetFileNameWithoutExtension(_path) + offset +".png", ImageFormat.Png);

        }



        //EOF - SECTION 16-------------------------------------------------------------- 

        private Tuple<int,int, int> GetTextureDimension_ByBPP(int bpp, int textureOffsetInSec42)
        {
            Console.WriteLine("WMSet: Getting texture dimension basing on BPP");
            int tiMoffsetCluTetc = textureOffsetInSec42 + 18;
            int cluTsize = BitConverter.ToUInt16(_sec42, tiMoffsetCluTetc-2);
            cluTsize = bpp == 8 ? cluTsize / 16 : cluTsize / 256;



            if (bpp == 8)
            {
                //4BPP - width * 4
                //4 bpp mode: Each byte contains 2 pixels (4 bits per pixel). Each 4 bits is a numeric refernce to the CLUT Data.

                tiMoffsetCluTetc += 2 + (cluTsize * 32) + 8;

            }
            if (bpp == 9)
            {
                //8BPP - width * 2
                //8 bpp mode: Each byte contains 1 pixel(8 bits per pixel).Each byte is a numeric refernce to the CLUT Data.

                tiMoffsetCluTetc += 2 + (cluTsize * 512) + 8;
            }

            int textureDataInt = tiMoffsetCluTetc + 4;
            UInt16 szerU = BitConverter.ToUInt16(_sec42, tiMoffsetCluTetc);
            UInt16 wysoU = BitConverter.ToUInt16(_sec42, tiMoffsetCluTetc + 2);

            var width = bpp == 8 ? szerU * 4 : szerU * 2;
            int height = wysoU;

            Tuple<int, int, int> ret = new Tuple<int, int, int>(width, height, textureDataInt);
            Console.WriteLine($"WMSet: Texture width: {width} height: {height}, DataInt: {textureDataInt}");
            return ret;
        }

        public Bitmap GetTexture() => _bmp;
    }

    
}
