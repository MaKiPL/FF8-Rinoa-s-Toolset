using System;
using System.Collections.Generic;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using AForge.Imaging.Filters;

namespace SerahToolkit_SharpGL
{
    internal class Wmset
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
        public const int DOffsetCount = DSizeHeader / 4;

        private int _width;
        private int _height;
        private ushort _paletteX;
        private ushort _paletteY;
        private ushort _imageX;
        private ushort _imageY;
        private ushort _hardcodedClutSize;

        private float _uu1;
        private float _vv1;
        private float _uu2;
        private float _vv2;
        private float _uu3;
        private float _vv3;
        private float _uu4;
        private float _vv4;

        private List<byte> _u;

        public static string ReturnLingual(string path)
        {
            string s = Path.GetFileNameWithoutExtension(path);
            return s.Substring(5, 2);
        }
        public Wmset(string path)
        {
            _path = path;
            _pathSec42 = _path.Substring(0, path.Length - 2) + "42";
        }


        public Tuple<List<string>,List<int>> _Debug_GetSections()
        {
            byte[] debugB = new byte[DSizeHeader];
            using (FileStream fs = new FileStream(_path, FileMode.Open))
            {
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(debugB, 0, DSizeHeader);
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

        public uint[] ProduceOffset_sec16()
        {
            Console.WriteLine($"WMSet: Producing offsets for section 16");
            List<uint> offsetList = new List<uint>();
            using (FileStream fs = new FileStream(_path, FileMode.Open))
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
        private uint[] ProduceOffset_sec42()
        {
            Console.WriteLine($"WMSet: Producing offsets for section 42");
            List<uint> offsetList = new List<uint>();
            using (FileStream fs = new FileStream(_pathSec42, FileMode.Open))
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
            }

            return offsetList.ToArray();
        }

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
            ushort triangleCount = BitConverter.ToUInt16(holdStage, offset);
            ushort quadCount = BitConverter.ToUInt16(holdStage, offset+2);
            ushort texturePage = BitConverter.ToUInt16(holdStage, offset+4);
            ushort vertexCount = BitConverter.ToUInt16(holdStage, offset+6);
            _curRrun = offset+8;
            Console.WriteLine($"WMSet: TriangleCount: {triangleCount}, QuadCount: {quadCount}, TexturePage: {texturePage}, VerticesCount: {vertexCount}");
            uint[] debugPreProduceOffset = ProduceOffset_sec42();
            int textureOffsetInSec42 = (int)debugPreProduceOffset[texindex];
            _textureStartInt = textureOffsetInSec42;
            _sec42 = File.ReadAllBytes(_pathSec42);
            byte bpp = _sec42[textureOffsetInSec42 + 4];
            _hardcodedClutSize = _sec42[textureOffsetInSec42 + 8];
            _paletteX = _sec42[textureOffsetInSec42 + 12];
            _paletteY = _sec42[textureOffsetInSec42 + 14];
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
                BitArray B = new BitArray(5);
                BitArray R = new BitArray(5);
                BitArray G = new BitArray(5);
                BitArray a = new BitArray(1);
                    B[0] = ba[10]; B[1] = ba[11]; B[2] = ba[12]; B[3] = ba[13]; B[4] = ba[14]; //R
                    R[0] = ba[0]; R[1] = ba[1]; R[2] = ba[2]; R[3] = ba[3]; R[4] = ba[4]; //G
                    G[0] = ba[5]; G[1] = ba[6]; G[2] = ba[7]; G[3] = ba[8]; G[4] = ba[9]; //B
                    a[0] = ba[15]; //Alpha if 0
                int[] b_ = new int[1]; B.CopyTo(b_, 0);
                int[] r_ = new int[1]; R.CopyTo(r_, 0);
                int[] g_ = new int[1]; G.CopyTo(g_, 0);
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
                _cp.Entries[i] = Color.FromArgb(alpha, int.Parse(b.ToString(CultureInfo.InvariantCulture)), int.Parse(g.ToString(CultureInfo.InvariantCulture)), int.Parse(r.ToString(CultureInfo.InvariantCulture)));
                startBuffer += 2;
            }
            int startTexture = GetTextureDimension_ByBPP(bpp, textureOffsetInSec42).Item3;
            Console.WriteLine($"WMSet: Drawing texture...");
            _bmp = new Bitmap(_width, _height, PixelFormat.Format32bppArgb) {Palette = _cp};
            int index = startTexture;
            ushort size = BitConverter.ToUInt16(_sec42, startTexture - 12);
            size -= 12;
            int debugWidth = _width;
            if (bpp == 9)
            {
                for (int y = 0; y < _height; y++)
                    for (int x = 0; x < debugWidth; x++)
                    {
                        _bmp.SetPixel(x, y,
                            Color.FromArgb(255, _cp.Entries[_sec42[index]].B, _cp.Entries[_sec42[index]].G,
                                _cp.Entries[_sec42[index]].R));
                        if (index < debugWidth*_height - 4 + startTexture)
                            index++;
                    }
            }
            else
            {
                byte[] uBits = new byte[size*2];
                Buffer.BlockCopy(_sec42, index, uBits, 0, size);
                BitArray bitArray = new BitArray(uBits);
                int ind = 0;
                for (int y = 0; y < _height; y++)
                    for (int x = 0; x < debugWidth; x++)
                    {
                        BitArray r = new BitArray(4)
                        {
                            [0] = bitArray[ind],
                            [1] = bitArray[1 + ind],
                            [2] = bitArray[2 + ind],
                            [3] = bitArray[3 + ind]
                        };
                        int[] rr = new int[1];
                        r.CopyTo(rr, 0);
                        _bmp.SetPixel(x, y,
                            Color.FromArgb(255, _cp.Entries[rr[0]].B, _cp.Entries[rr[0]].G, _cp.Entries[rr[0]].R));
                        if (ind < (debugWidth*_height)*4)
                            ind += 4;
                    }
            }

            Console.WriteLine($"WMSet: Drawing texture transparency");
            _bmp.MakeTransparent(Color.Black);

            Console.WriteLine($"WMSet: Reading UVs...");
            if (triangleCount != 0)
            {
                int a = _curRrun;
                _u = new List<byte>();
                if (_imageX < 192 && bpp != 9)
                {
                    while (true)
                    {
                        _u.Add(holdStage[a + 4]);
                        _u.Add(holdStage[a + 6]);
                        _u.Add(holdStage[a + 8]);
                        if (a == offset + 8 + (triangleCount * 12) - 12)
                            break;
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
                    a += 12;
                }
                _curRrun = a + 12;
            }
            Console.WriteLine($"WMSet: Processing quads");
            if (quadCount != 0)
            {
                int a = _curRrun;
                _u = new List<byte>();
                if (_imageX < 192 && bpp != 9)
                {
                    while (true)
                    {
                        _u.Add(holdStage[a + 4]);
                        _u.Add(holdStage[a + 6]);
                        _u.Add(holdStage[a + 8]);                        
                        _u.Add(holdStage[a + 10]);
                        if (a == _curRrun + (quadCount * 16) - 16)
                            break;
                        a += 16;
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
                        _uu2 = bpp == 9
                            ? (float) holdStage[a + 6]/debugWidth
                            : ((float) holdStage[a + 6] - (_imageX - 192)*4)/debugWidth;
                        _u.Add((holdStage[a + 6]));
                        _vv2 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 7]/_height)
                            : 1.0f - ((float) holdStage[a + 7] - _imageY)/_height;
                        _uu3 = bpp == 9
                            ? (float) holdStage[a + 8]/debugWidth
                            : ((float) holdStage[a + 8] - (_imageX - 192)*4)/debugWidth;
                        _u.Add((holdStage[a + 8]));
                        _vv3 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 9]/_height)
                            : 1.0f - ((float) holdStage[a + 9] - _imageY)/_height;
                        _uu4 = bpp == 9
                            ? (float) holdStage[a + 10]/debugWidth
                            : ((float) holdStage[a + 10] - (_imageX - 192)*4)/debugWidth;
                        _u.Add((holdStage[a + 10]));
                        _vv4 = bpp == 9
                            ? 1.0f - ((float) holdStage[a + 11]/_height)
                            : 1.0f - ((float) holdStage[a + 11] - _imageY)/_height;
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
                    a += 16;
                }
                _curRrun = a + 16;
            }

            int start = _curRrun;
            Console.WriteLine($"WMSet: Reading vertices data");
            while (true)
            {
                float xa = (BitConverter.ToInt16(holdStage, _curRrun)) / 100.0f;
                float ya = (BitConverter.ToInt16(holdStage, _curRrun+2)) / 100.0f;
                float za = (BitConverter.ToInt16(holdStage, _curRrun+4)) / 100.0f;
                v += $"v {xa} {ya*-1f} {za}" + Environment.NewLine;
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
                using (StreamWriter fs = new StreamWriter(pathofd))
                {
                    fs.WriteLine(@"#Made with Rinoa's toolset by MaKiPL.");
                    fs.WriteLine(@"mtllib " + Path.GetFileNameWithoutExtension(_path) + offset + ".mtl");
                    fs.WriteLine(@"usemtl Textured");
                    fs.WriteLine("");
                    fs.WriteLine(v + Environment.NewLine + vtT + Environment.NewLine + fT);
                }
            }
            if(quadCount != 0)
            {
                string pathofd = Path.GetDirectoryName(_path) + @"\" + Path.GetFileNameWithoutExtension(_path) + "_" + offset + "_q.obj";
                if (File.Exists(pathofd))
                    File.Delete(pathofd);
                using (StreamWriter fs = new StreamWriter(pathofd))
                {
                    fs.WriteLine(@"#Made with Rinoa's toolset by MaKiPL.");
                    fs.WriteLine(@"mtllib " + Path.GetFileNameWithoutExtension(_path) + offset + ".mtl");
                    fs.WriteLine(@"usemtl Textured");
                    fs.WriteLine("");
                    fs.WriteLine(v + Environment.NewLine + vt + Environment.NewLine + fQ);
                }
            }
            Console.WriteLine($"WMSet: Saving bitmap");
            _bmp.Save(Path.GetDirectoryName(_path) + @"\" + Path.GetFileNameWithoutExtension(_path) + offset +".png", ImageFormat.Png);
        }
        private Tuple<int,int, int> GetTextureDimension_ByBPP(int bpp, int textureOffsetInSec42)
        {
            Console.WriteLine("WMSet: Getting texture dimension basing on BPP");
            int tiMoffsetCluTetc = textureOffsetInSec42 + 18;
            int cluTsize = BitConverter.ToUInt16(_sec42, tiMoffsetCluTetc-2);
            cluTsize = bpp == 8 ? cluTsize / 16 : cluTsize / 256;
            if (bpp == 8)
                tiMoffsetCluTetc += 2 + (cluTsize*32) + 8;
            if (bpp == 9)
                tiMoffsetCluTetc += 2 + (cluTsize*512) + 8;
            int textureDataInt = tiMoffsetCluTetc + 4;
            ushort szerU = BitConverter.ToUInt16(_sec42, tiMoffsetCluTetc);
            ushort wysoU = BitConverter.ToUInt16(_sec42, tiMoffsetCluTetc + 2);
            var width = bpp == 8 ? szerU * 4 : szerU * 2;
            int height = wysoU;
            Console.WriteLine($"WMSet: Texture width: {width} height: {height}, DataInt: {textureDataInt}");
            return new Tuple<int, int, int>(width, height, textureDataInt);
        }

        public Bitmap GetTexture() => _bmp;
    }

    internal class WM_Section2
    {
        [DllImport("msvcrt.dll", EntryPoint = "memcpy", CallingConvention = CallingConvention.Cdecl, SetLastError = false)]
        public static extern IntPtr memcpy(IntPtr dest, IntPtr src, UIntPtr count);

        public string path;
        private Bitmap originalMap;
        private Bitmap Colored;
        private BinaryReader br;
        private FileStream fs;
        public byte[] regions;
        public ushort selectedRegion = 0;

        public WM_Section2(string path)
        {
            this.path = path;
            fs = new FileStream(this.path, FileMode.Open, FileAccess.ReadWrite);
            br = new BinaryReader(fs);
            regions = new byte[768];
        }

        private int[] regionHUE = {20, 40, 60, 80, 100, 120, 140, 160, 180, 200, 220, 240, 260, 280, 300, 320, 340, 360, 0};

        public void EndJob()
        {
            br.Dispose();
            fs.Dispose();
        }
        public Bitmap GetColored => Colored;
        public void ReceiveBitmap(Bitmap bmp) => originalMap = bmp;
        public byte ReadNextRegion() => br.ReadByte();

        public void ColorizeBlock(int blockID, byte regionID)
        {
            regions[blockID] = regionID;
            if(Colored == null)
                Colored = new Bitmap(originalMap);

            Rectangle rect = CalculateRectangle(blockID);
            int huetransform;
            if (regionID < regionHUE.Length)
                huetransform = regionID == 0xFF ? 0 : regionHUE[regionID];
            else huetransform = 0;
            if (regionID == 255) return;
            HueModifier hue = new HueModifier(huetransform);
            hue.ApplyInPlace(Colored, rect);
        }

        private Rectangle CalculateRectangle(int blockID)
        {
            int widthblock = blockID * 32;
            int row = (int)Math.Round((double)(widthblock / 1024), 1);
            int realwidth = row != 0 ? widthblock - 1024 * row : widthblock;
            return new Rectangle(realwidth, row * 32, 32, 32);
        }

        public unsafe void ResetBlockColor(int blockID)
        {
            BitmapData original = originalMap.LockBits(CalculateRectangle(blockID), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            BitmapData colored = Colored.LockBits(CalculateRectangle(blockID), ImageLockMode.ReadWrite,
                PixelFormat.Format24bppRgb);
            IntPtr originscan = original.Scan0;
            IntPtr colorscan = colored.Scan0;
            memcpy(colorscan,originscan,new UIntPtr((uint) (colored.Stride*colored.Height)));
            Colored.UnlockBits(colored);
            originalMap.UnlockBits(original);
            ColorizeBlock(blockID,regions[selectedRegion]);
        }
    }

    internal class WM_Section4
    {
        public const byte BYTESPERREGION = 48;
        public const byte REGIONSCOUNT = 14;
        public string path;
        private FileStream fs;
        private BinaryReader br;

        private ushort[] encounters;

        public WM_Section4(string path)
        {
            this.path = path;
            fs = new FileStream(path, FileMode.Open, FileAccess.ReadWrite);
            br = new BinaryReader(fs);
            encounters = new ushort[BYTESPERREGION*REGIONSCOUNT];
        }

        public void EndJob()
        {
            br.Dispose();
            fs.Dispose();
        }

        public void ProduceData()
        {
            for (int i = 0x0; i < encounters.Length; i++)
                encounters[i] = br.ReadUInt16();
            EndJob();
        }

        public ushort[] GetEncounters => encounters;
    }

    internal class WM_Section1
    {
        private string path;
        public ENTRY[] entries;

        public struct ENTRY
        {
            public byte regionID;
            public byte GroundID;
            public byte ESI;
            byte padding;
        }

        public WM_Section1(string path)
        {
            this.path = path;
        }

        public void ReadData()
        {
            FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            BinaryReader br = new BinaryReader(fs);

            uint size = br.ReadUInt32();
            uint count = (size - 4)/4;
            entries = new ENTRY[count];
            for (int i = 0; i < count; i++)
            {
                entries[i].regionID = br.ReadByte();
                entries[i].GroundID = br.ReadByte();
                entries[i].ESI = br.ReadByte();
                br.ReadByte(); //null
            }
            br.Dispose();
            fs.Dispose();
        }
    }
}
