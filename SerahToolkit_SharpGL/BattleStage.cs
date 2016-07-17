using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace SerahToolkit_SharpGL
{
    internal class BattleStage
    {
        private int _tim;
        private byte[] _stage;
        private int _pass;
        private int _passOk;
        private int _index;
        private List<int> _itemOffsets;
        private readonly string _pathh;

        private int _triangles;
        private int _quads;
        private int _verts;
        private int _absolutePolygon;
        private int _width;
        private int _height;

        private byte _u1;
        private byte _u2;
        private byte _u3;
        private byte _u4;

        private int _u5;
        private int _t5;

        private byte _v1;
        private byte _v2;
        private byte _v3;
        private byte _v4;

        private string _v;
        private string _tt;
        private string _fv;

        private int _quadStop;
        private int _trisStop;
        private int _changeStop;
        private int _changeAdd;

        private int _quadOffset;
        private int _triangleOffset;

        private int _textureDataInt;

        private int _currRun;

        private StageTexture _st;
        private Bitmap _bmp;

        private ushort _u4U;

        private ushort _cluTsize;

        readonly byte[] _tiMtexture = { 0x10, 0x00, 0x00, 0x00, 0x09 };
        readonly byte[] _geom = { 0x01, 0x00, 0x01, 0x00 };


        public BattleStage(string path)
        {
            if (path == "UV") return;
            _pathh = path;
            _stage = File.ReadAllBytes(path);
        }
        public void Process(bool generateTexture = false, bool bUpdating = false)
        {
            Console.WriteLine("BS: Searching for objects...");
            SearchObjects();
            Console.WriteLine("BS: Reading texture metadata:");
            ResolveTex();
            if (!bUpdating)
            {
                Console.WriteLine($"BS: Setting up MTL");
                SetupMtl();
                _st = new StageTexture(_tim, _width, _height);
                byte[] textureByte = new byte[_stage.Length - _textureDataInt];
                byte[] clutByte = new byte[_stage.Length - (_textureDataInt - _tim)];
                Buffer.BlockCopy(_stage, _textureDataInt, textureByte, 0, textureByte.Length);
                Buffer.BlockCopy(_stage, _tim, clutByte, 0, _textureDataInt - _tim);
                Console.WriteLine($"BS: TextureWorker initialized");
                string pathOfd;
                if (generateTexture)
                {
                    for (int i = _cluTsize - 1; i > 0; i--)
                    {
                        Console.WriteLine($"BS: TextureWorker generating palette and texture");
                        _st.CreatePalettedTex(i, clutByte);
                        _st.CopyTextureBuffer(textureByte);
                        _bmp = _st.GetBmp();
                        pathOfd = Path.GetDirectoryName(_pathh);
                        _bmp.Save(pathOfd + @"\" + Path.GetFileNameWithoutExtension(_pathh) + @"_" + i + ".png", ImageFormat.Png);
                    }
                }
                Console.WriteLine($"BS: TextureWorker generating palette and texture");
                _st.CreatePalettedTex(0, clutByte);
                _st.CopyTextureBuffer(textureByte);
                _bmp = _st.GetBmp();
                pathOfd = Path.GetDirectoryName(_pathh);
                Console.WriteLine($"BS: TextureWorker saving texture");
                _bmp.Save(pathOfd + @"\" + Path.GetFileNameWithoutExtension(_pathh) + ".png", ImageFormat.Png);
                foreach (int off in _itemOffsets)
                {
                    _verts = BitConverter.ToUInt16(_stage, off + 4);
                    _absolutePolygon = off + 6 + (_verts * 6);
                    _triangles = BitConverter.ToUInt16(_stage, _absolutePolygon + 4 + (_absolutePolygon % 4));
                    _quads = BitConverter.ToUInt16(_stage, _absolutePolygon + 6 + (_absolutePolygon % 4));
                    Console.WriteLine($"BS: Offset: {off}, \n\tVertices: {_verts}\n\tTriangles: {_triangles}\n\tQuads: {_quads}");
                    if (_triangles != 0 && _quads != 0)  
                    {
                        Process_Step(true, off);
                        Process_Step(false, off);
                    }
                    else
                        if (_triangles != 0)
                        Process_Step(true, off);
                    else
                            if (_quads != 0)
                        Process_Step(false, off);
                }
            }
            else
            {
                foreach (int off in _itemOffsets)
                {
                    Console.WriteLine($"BS: Updating for BS_Verts editor");
                    _verts = BitConverter.ToUInt16(_stage, off + 4);
                    _absolutePolygon = off + 6 + (_verts * 6);
                    _triangles = BitConverter.ToUInt16(_stage, _absolutePolygon + 4 + (_absolutePolygon % 4));
                    _quads = BitConverter.ToUInt16(_stage, _absolutePolygon + 6 + (_absolutePolygon % 4));

                    if (_triangles != 0 && _quads != 0) 
                    {
                        Process_Step(true, off);
                        Process_Step(false, off);
                    }
                    else
                        if (_triangles != 0)
                        Process_Step(true, off);
                    else
                            if (_quads != 0)
                        Process_Step(false, off);
                }
            }
        }

        private void ResolveTex()
        {
            int tiMoffsetCluTetc = _tim + 18;
            _cluTsize = BitConverter.ToUInt16(_stage, tiMoffsetCluTetc);
            Console.WriteLine($"BS: CLUT size: {_cluTsize}");
            tiMoffsetCluTetc += 2 + (_cluTsize * 512) + 8;
            _textureDataInt = tiMoffsetCluTetc + 4;
            _width = (BitConverter.ToUInt16(_stage, tiMoffsetCluTetc) * 2);
            _height = BitConverter.ToUInt16(_stage, tiMoffsetCluTetc + 2);
            Console.WriteLine($"BS: Texture width: {_width}\nBS: Texture height: {_height}");
        }

        private void Process_Step(bool bTriangle,int off)
        {
            Console.WriteLine($"BS: Geometry parser: is triangle? {bTriangle}, offset: {off}");
            _v = null;
            _tt = null;
            _fv = null;
            string pathOfd = Path.GetDirectoryName(_pathh);
            if (bTriangle)
                pathOfd += $@"\{Path.GetFileNameWithoutExtension(_pathh)}_{off}_t.obj";
            else
                pathOfd += $@"\{Path.GetFileNameWithoutExtension(_pathh)}_{off}_q.obj";

            if (File.Exists(pathOfd))
                File.Delete(pathOfd);

            StreamWriter sw = new StreamWriter(pathOfd);
            sw.WriteLine(@"#Made with Rinoa's toolset by MaKiPL");
            sw.WriteLine(@"mtllib " + Path.GetFileNameWithoutExtension(_pathh) + ".mtl");
            sw.WriteLine(@"usemtl Textured");
            sw.WriteLine("");
            int vertexOffset = off + 6;
            int vertStop = vertexOffset + (6 * _verts);
            int loopIndexV = vertexOffset;
            Console.WriteLine($"BS: Calculating vertices from {loopIndexV} to {vertStop}");
            while (true)
            {
                short x = (BitConverter.ToInt16(_stage, loopIndexV));
                short y = (BitConverter.ToInt16(_stage, loopIndexV + 2));
                short z = (BitConverter.ToInt16(_stage, loopIndexV + 4));
                float xa = x / 2000.0f; 
                float ya = y / 2000.0f;
                float za = z / 2000.0f;
                string vline = $"v {xa} {ya} {za}" + Environment.NewLine;
                _v += vline;
                _v = _v.Replace(',', '.');
                loopIndexV += 6;
                if (loopIndexV == vertStop)
                    break;
            }
            if (bTriangle)
            {
                _triangleOffset = _absolutePolygon + 12 + (_absolutePolygon % 4);
                Console.WriteLine($"BS: Triangle offset: {_triangleOffset}");
                _currRun = _triangleOffset + 6;
                _trisStop = _currRun + (_triangles * 20);
                Console.WriteLine($"BS: Triangle end offset: {_trisStop}");
            }
            else
            {
                if (_triangles != 0)
                {
                    _quadOffset = _absolutePolygon + 12 + (_absolutePolygon % 4) + _triangles * 20;
                    Console.WriteLine($"BS: Quads offset: {_quadOffset}");
                }
                else
                {
                    _quadOffset = _absolutePolygon + 12 + (_absolutePolygon % 4);
                    Console.WriteLine($"BS: Quads offset: {_quadOffset}");
                }

                _currRun = _quadOffset + 8;
                _quadStop = (_currRun + (_quads * 24));
                _trisStop = (_currRun + (_triangles * 20));
            }
            while (true)
            {
                _u1 = _stage[_currRun];
                _v1 = _stage[_currRun + 1];
                if (bTriangle)
                {
                    _u2 = _stage[_currRun + 2];
                    _v2 = _stage[_currRun + 3];
                    _u3 = _stage[_currRun + 6];
                    _v3 = _stage[_currRun + 7];
                }
                else
                {
                    _u2 = _stage[_currRun + 4];
                    _v2 = _stage[_currRun + 5];
                    _u3 = _stage[_currRun + 8];
                    _v3 = _stage[_currRun + 9];
                    _u4 = _stage[_currRun + 10];
                    _v4 = _stage[_currRun + 11];
                }
                var add86 = bTriangle ? 8 : 6;
                string strByte = _stage[_currRun + add86].ToString("X2");
                strByte = "0" + strByte.Substring(1);
                byte page = byte.Parse(strByte);
                int pageInt = page * 128;
                float uu1 = _u1 / (float)_width + ((float)pageInt / _width);
                float vv1 = 1.0f - (_v1 / 256.0f);
                float uu2 = _u2 / (float)_width + ((float)pageInt / _width);
                float vv2 = 1.0f - (_v2 / 256.0f);
                float uu3 = _u3 / (float)_width + ((float)pageInt / _width);
                float vv3 = 1.0f - (_v3 / 256.0f);
                _tt += string.Format("vt {0} {1}" + Environment.NewLine, uu1, vv1);
                _tt += string.Format("vt {0} {1}" + Environment.NewLine, uu2, vv2);
                _tt += string.Format("vt {0} {1}" + Environment.NewLine, uu3, vv3);
                if (!bTriangle)
                {
                    float uu4 = _u4 / (float)_width + ((float)pageInt / _width);
                    float vv4 = 1.0f - (_v4 / 256.0f);
                    _tt += string.Format("vt {0} {1}" + Environment.NewLine, uu4, vv4);
                }
                if(bTriangle)
                {
                    _changeStop = _trisStop - 20;
                    _changeAdd = 20;
                }
                else
                {
                    _changeStop = _quadStop - 24;
                    _changeAdd = 24;
                }
                if (_currRun == _changeStop)
                    break;
                _currRun += _changeAdd;
            }
            Console.WriteLine($"BS: Calculated UVs and fixed output");
            _v = _v.Replace(',', '.');
            _tt = _tt.Replace(',', '.');
            Console.WriteLine($"BS: Preparing face indices parsing");
            int faceIndex = 1;
            var faceQuad = bTriangle ? _triangleOffset : _quadOffset;
            int quadStoPq;
            if (bTriangle)
                quadStoPq = _triangleOffset + (_triangles * _changeAdd);
            else
                quadStoPq = _quadOffset + (_quads * _changeAdd);
            _fv = null;
            Console.WriteLine($"BS: Polygon parsing...");
            while (true)
            {
                ushort u1U = BitConverter.ToUInt16(_stage, faceQuad);
                ushort u2U = BitConverter.ToUInt16(_stage, faceQuad + 2);
                ushort u3U = BitConverter.ToUInt16(_stage, faceQuad + 4);
                if(!bTriangle)
                    _u4U = BitConverter.ToUInt16(_stage, faceQuad + 6);

                int u1 = u1U + 1;
                int u2 = u2U + 1;
                int u3 = u3U + 1;
                if(!bTriangle)
                    _u5 = _u4U + 1;

                int t1 = faceIndex;
                int t2 = faceIndex + 1;
                int t3 = faceIndex + 2;
                if(!bTriangle)
                    _t5 = faceIndex + 3;
                if(!bTriangle)
                    _fv += $"f {u1}/{t1} {u2}/{t2} {_u5}/{_t5} {u3}/{t3}{Environment.NewLine}";
                else
                    _fv += $"f {u1}/{t2} {u2}/{t3} {u3}/{t1}{Environment.NewLine}";
                if (faceQuad == quadStoPq - _changeAdd)
                    break;
                faceQuad += _changeAdd;
                if (bTriangle)
                    faceIndex += 3;
                else
                    faceIndex += 4;
            }
            Console.WriteLine($"BS: Parsing done! Outputting to file...");
            sw.WriteLine(_v);
            sw.WriteLine(_tt);
            if (bTriangle)
                sw.WriteLine("g " + off + "_t");
            else
                sw.WriteLine("g " + off + "_q");
            sw.WriteLine(_fv);
            sw.Close();
            Console.WriteLine($"BS: Job is done!");
        }

        private void SetupMtl()
        {
            string pathOfd = Path.GetDirectoryName(_pathh);
            pathOfd += @"\" + Path.GetFileNameWithoutExtension(_pathh) + ".MTL";

            if (File.Exists(pathOfd))
                File.Delete(pathOfd);
            StreamWriter swe = new StreamWriter(pathOfd);
                swe.WriteLine("newmtl Textured");
                swe.WriteLine("Kd 1.000 1.000 1.000");
                swe.WriteLine("illum 2");
                swe.WriteLine("map_Kd " + Path.GetFileNameWithoutExtension(_pathh) + ".png");
                swe.Close();
        }

        private void SearchObjects()
        {
            _tim = SearchForByte.ByteSearch(_stage, _tiMtexture);
            _index = 0x5d4+1000;
            _itemOffsets = new List<int>();
            while (true)
            {
                _pass = SearchForByte.ByteSearch(_stage, _geom, _index);
                if (_pass == -1)
                    break;
                _passOk = _pass;
                _index = _pass + 1;
                if (_passOk < _tim)
                {
                    _verts = BitConverter.ToUInt16(_stage, _passOk + 4);
                    if (_verts < 10000)
                    {
                        _absolutePolygon = _passOk + 6 + (_verts * 6);
                        if (_absolutePolygon < _stage.Length)
                        {
                            _triangles = BitConverter.ToUInt16(_stage, _absolutePolygon + 4 + (_absolutePolygon % 4));
                            _quads = BitConverter.ToUInt16(_stage, _absolutePolygon + 6 + (_absolutePolygon % 4));


                            if (_triangles > 10000 || _quads > 10000 || _verts > 10000 || _verts == 0)
                            {

                            }
                            else
                            {
                                Console.WriteLine($"BS: Found working geometry at: {_passOk}");
                                _itemOffsets.Add(_passOk);
                            }
                        }
                    }
                }
                else
                    break;
            }

        }

        public int[] GetArrayOfObjects()
        {
            int[] array = new int[_itemOffsets.Count];

            if (_itemOffsets != null)
            {
                array = _itemOffsets.ToArray();
            }
            return array;
        }

        public Bitmap GetTexture() => _bmp;

        public Tuple<List<double>,List<double>,int> GetUVpoints(int offset, string stagePath, int lastKnownTim)
        {
            List<double> uv1 = new List<double>();
            List<double> uv2 = new List<double>();

            int clut = 0;
            _stage = File.ReadAllBytes(stagePath);
            int tim = lastKnownTim;
            int tiMoffsetCluTetc = tim + 18;
            ushort cluTsize = BitConverter.ToUInt16(_stage, tiMoffsetCluTetc);
            tiMoffsetCluTetc += 2 + (cluTsize * 512) + 8;
            _textureDataInt = tiMoffsetCluTetc + 4;
            ushort szerU = BitConverter.ToUInt16(_stage, tiMoffsetCluTetc);
            ushort wysoU = BitConverter.ToUInt16(_stage, tiMoffsetCluTetc + 2);
            _width = szerU * 2;
            _height = wysoU;
            _verts = BitConverter.ToUInt16(_stage, offset + 4);
            _absolutePolygon = offset + 6 + (_verts * 6);
            _triangles = BitConverter.ToUInt16(_stage, _absolutePolygon + 4 + (_absolutePolygon % 4));
            _quads = BitConverter.ToUInt16(_stage, _absolutePolygon + 6 + (_absolutePolygon % 4));
            if (_triangles != 0)
            {
                    _triangleOffset = _absolutePolygon + 12 + (_absolutePolygon % 4);
                    _currRun = _triangleOffset + 6;
                    _trisStop = _currRun + (_triangles * 20);
                while (true)
                {
                    _u1 = _stage[_currRun];
                    _v1 = _stage[_currRun + 1];
                    _u2 = _stage[_currRun + 2];
                    _v2 = _stage[_currRun + 3];

                    byte[] clutBuff = new byte[2];
                    Buffer.BlockCopy(_stage, _currRun + 4, clutBuff, 0, 2);
                    clut = ResolveClut(clutBuff);
                    _u3 = _stage[_currRun + 6];
                    _v3 = _stage[_currRun + 7];
                    string strByte = _stage[_currRun + 8].ToString("X2");
                    strByte = "0" + strByte.Substring(1);
                    byte page = byte.Parse(strByte);
                    int pageInt = page * 128;
                    double uu1 = _u1 / (float)_width + ((float)pageInt / _width);
                    double vv1 = 1.0f - (_v1 / 256.0f);
                    double uu2 = _u2 / (float)_width + ((float)pageInt / _width);
                    double vv2 = 1.0f - (_v2 / 256.0f);
                    double uu3 = _u3 / (float)_width + ((float)pageInt / _width);
                    double vv3 = 1.0f - (_v3 / 256.0f);
                    uv1.Add(uu1);
                    uv2.Add(vv1);
                    uv1.Add(uu2);
                    uv2.Add(vv2);
                    uv1.Add(uu3);
                    uv2.Add(vv3);
                        _changeStop = _trisStop - 20;
                        _changeAdd = 20;
                    if (_currRun == _changeStop)
                        break;
                    _currRun += _changeAdd;
                }
            }
            _currRun = 0;
            if (_quads != 0)
            {
                if (_triangles != 0)
                {
                    _quadOffset = _absolutePolygon + 12 + (_absolutePolygon % 4) + _triangles * 20;
                }
                else
                {
                    _quadOffset = _absolutePolygon + 12 + (_absolutePolygon % 4);
                }
                _currRun = _quadOffset + 8;
                _quadStop = (_currRun + (_quads * 24));
                _trisStop = (_currRun + (_triangles * 20));
                while (true)
                {
                    _u1 = _stage[_currRun];
                    _v1 = _stage[_currRun + 1];
                    byte[] clutBuff = new byte[2];
                    Buffer.BlockCopy(_stage, _currRun + 2, clutBuff, 0, 2);
                    clut = ResolveClut(clutBuff);
                    _u2 = _stage[_currRun + 4];
                    _v2 = _stage[_currRun + 5];
                    _u3 = _stage[_currRun + 8];
                    _v3 = _stage[_currRun + 9];
                    _u4 = _stage[_currRun + 10];
                    _v4 = _stage[_currRun + 11];
                    string strByte = _stage[_currRun + 6].ToString("X2");
                    strByte = "0" + strByte.Substring(1);
                    byte page = byte.Parse(strByte);
                    int pageInt = page * 128;

                    double uu1 = _u1 / (float)_width + ((float)pageInt / _width);
                    double vv1 = 1.0f - (_v1 / 256.0f);
                    double uu2 = _u2 / (float)_width + ((float)pageInt / _width);
                    double vv2 = 1.0f - (_v2 / 256.0f);
                    double uu3 = _u3 / (float)_width + ((float)pageInt / _width);
                    double vv3 = 1.0f - (_v3 / 256.0f);
                    double uu4 = _u4 / (float)_width + ((float)pageInt / _width);
                    double vv4 = 1.0f - (_v4 / 256.0f);
                    uv1.Add(uu1);
                    uv2.Add(vv1);
                    uv1.Add(uu2);
                    uv2.Add(vv2);
                    uv1.Add(uu3);
                    uv2.Add(vv3);
                    uv1.Add(uu4);
                    uv2.Add(vv4);
                    _changeStop = _quadStop - 24;
                    _changeAdd = 24;
                    if (_currRun == _changeStop)
                        break;
                    _currRun += _changeAdd;
                }
            }
            return new Tuple<List<double>, List<double>, int>(uv1, uv2, clut);
        }

        public int GetLastTim() => _tim;

        public Tuple<int,int> GetTextureRes() => new Tuple<int, int>(_width, _height);

        private static int ResolveClut(byte[] buffer)
        {
            byte[] bt = new byte[2];
            Buffer.BlockCopy(buffer, 1, bt, 0, 1);
            Buffer.BlockCopy(buffer, 0, bt, 1, 1);
            BitArray ba = new BitArray(bt);
            BitArray cluTbit = new BitArray(4)
            {
                [3] = ba[1],
                [2] = ba[0],
                [1] = ba[15],
                [0] = ba[14]
            };

            int[] clutArray = new int[1];
            cluTbit.CopyTo(clutArray, 0);
            return clutArray[0];
        }

        public void DumpRaw(int offset, string savePath, int nextOffset = -1)
        {
            if (nextOffset == -1)
                nextOffset = SearchForByte.ByteSearch(_tiMtexture, _stage, offset);

            byte[] rawData = new byte[nextOffset - offset];
            Buffer.BlockCopy(_stage, offset, rawData, 0, nextOffset - offset);
            File.WriteAllBytes(savePath, rawData);
        }
    }
}
