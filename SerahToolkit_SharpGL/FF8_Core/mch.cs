using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class mch
    {
        private string path;
        public string objpath;
        private FileStream fs;
        private BinaryReader br;


        private uint[] textureoffsets;
        private uint modeloffset;

        private uint numofverts;
        private uint numoffaces;
        private ushort triangles;
        private ushort quads;
        private uint vertsoffset;
        private uint facesoffset;
        private string[] facestring;
        private Vertex[] vertices;
        private string[] triangless;
        private string[] quadss;
        private string[] uvs;
        private byte whichTextureID;

        private const uint isTriangle = 0x07060125;
        private const uint isQuad = 0x0907012d;


        public mch(string path)
        {
            this.path = path;
            HandleStreams();
        }

        public struct MCH
        {
            public uint[] textureOffsets;
            public uint modelOffset;
            public byte[] data;
        }

        public struct Vertex
        {
            public float X;
            public float Y;
            public float Z;
            public float W;
        }

        private void HandleStreams()
        {
            using (fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                using (br = new BinaryReader(fs))
                    ReadMCH();
        }

        private void ReadMCH()
        {
            List<uint> texoffsets = new List<uint>();
            while (true)
            {
                uint offset = br.ReadUInt32();
                if (offset == 0xFFFFFFFF)
                {
                    textureoffsets = texoffsets.ToArray();
                    break;
                }
                texoffsets.Add((offset << 8)>>8);
            }
            modeloffset = br.ReadUInt32();
            ulong rememberposition = (ulong) fs.Position;
            string[] texPaths = new string[textureoffsets.Length];
            for (int i = 0; i < textureoffsets.Length; i++) //USEMTL for multiTextures
            {
                byte[] buf;
                if (i == textureoffsets.Length - 1)
                {
                    uint size = modeloffset - textureoffsets[i];
                    buf = new byte[size];
                    fs.Seek(textureoffsets[i], SeekOrigin.Begin);
                    buf = br.ReadBytes(buf.Length);
                    texPaths[i] = $"{Path.GetDirectoryName(path)}\\{Path.GetFileNameWithoutExtension(path)}_{i}.tim";
                    File.WriteAllBytes(texPaths[i], buf);
                }
                else
                {
                    uint size = textureoffsets[i + 1] - textureoffsets[i];
                    
                    buf= new byte[size];
                    fs.Seek(textureoffsets[i], SeekOrigin.Begin);
                    buf = br.ReadBytes(buf.Length);
                    texPaths[i] = $"{Path.GetDirectoryName(path)}\\{Path.GetFileNameWithoutExtension(path)}_{i}.tim";
                    File.WriteAllBytes(texPaths[i], buf);
                }
                fs.Position = (long) rememberposition;
                TIM tim = new TIM(texPaths[i]);
                Bitmap texture = tim.GetBitmap;
                texture.Save($"{texPaths[i]}.png");
            }
            fs.Seek(modeloffset+4, SeekOrigin.Begin);
            numofverts = br.ReadUInt32();
            br.ReadUInt32();
            numoffaces = br.ReadUInt32();
            fs.Seek(modeloffset + 0x1C, SeekOrigin.Begin);
            triangles = br.ReadUInt16();
            quads = br.ReadUInt16();
            fs.Seek(modeloffset + 0x24, SeekOrigin.Begin);
            vertsoffset = br.ReadUInt32();
            fs.Seek(4, SeekOrigin.Current);
            facesoffset = br.ReadUInt32();

            ReadVertices();
            ReadFaces();
            ConstructOBJ();
        }

        private void ConstructOBJ()
        {
            List<string> wavefront = new List<string>();
            wavefront.Add($"mtllib {Path.GetFileNameWithoutExtension(path)}.mtl");
            for (int i = 0; i < vertices.Length; i++)
                wavefront.Add(
                    $"v {vertices[i].X.ToString().Replace(',', '.')} {vertices[i].Z.ToString().Replace(',', '.')} {vertices[i].Y.ToString().Replace(',', '.')}");
            wavefront.AddRange(uvs);
            wavefront.Add("g triangles");
            wavefront.AddRange(triangless);
            wavefront.Add("g quads");
            wavefront.AddRange(quadss);
            string[] obj = wavefront.ToArray();
            string constructPath = objpath =$"{Path.GetDirectoryName(path)}\\{Path.GetFileNameWithoutExtension(path)}.obj";
            File.WriteAllLines(constructPath, obj);
            ConstructMTL();
        }

        private void ConstructMTL()
        {
            string[] mtlLib = new string[textureoffsets.Length*2];
            for (int i = 0; i < textureoffsets.Length; i++)
            {
                char a = (char) 0x41;
                mtlLib[i*2] = $"newmtl mat{Convert.ToChar(a+ i)}";
                mtlLib[i*2+1] = $"map_Kd {Path.GetFileNameWithoutExtension(path)}_{i}.tim.png";
            }
            File.WriteAllLines($"{ Path.GetDirectoryName(path)}\\{ Path.GetFileNameWithoutExtension(path)}.mtl", mtlLib);
        }

        private void ReadFaces()
        {
            fs.Seek(facesoffset + modeloffset, SeekOrigin.Begin);
            List<string> triangles = new List<string>();
            List<string> quads = new List<string>();
            List<string> uvList = new List<string>();
            int vtIndex = 1;
            for (int i = 0; i < numoffaces; i++)
            {
                uint testvar = br.ReadUInt32();
                bool bisTriangle = testvar == (uint)0x25010607;
                fs.Seek(8, SeekOrigin.Current);
                string s;
                s = bisTriangle
                    ? $"f {br.ReadUInt16()+1}/{vtIndex} {br.ReadUInt16()+1}/{vtIndex+1} {br.ReadUInt16()+1}/{vtIndex+2}"
                    : $"f {br.ReadUInt16()+1}/{vtIndex} {br.ReadUInt16()+1}/{vtIndex+1}";
                if (bisTriangle)
                {
                    br.ReadUInt16();
                    vtIndex += 3;
                }
                else
                {
                    ushort d = br.ReadUInt16();
                    s += $" {br.ReadUInt16() + 1}/{vtIndex + 3} {d + 1}/{vtIndex + 2}";
                    vtIndex += 4;
                }
                fs.Seek(34, SeekOrigin.Current);
                if (bisTriangle)
                {
                    triangles.Add(CreateMatTag());
                    triangles.Add(s);
                }
                else
                {
                    quads.Add(CreateMatTag());
                    quads.Add(s);
                }
                fs.Seek(-44, SeekOrigin.Current);
                ReadUVs(uvList, bisTriangle);
                fs.Seek(12, SeekOrigin.Current);
            }
            this.triangless = triangles.ToArray();
            this.quadss = quads.ToArray();
        }

        private void ReadUVs(List<string> uvList, bool bTriangle )
        {
            fs.Seek(24, SeekOrigin.Current);
            TIM parametersOnly = new TIM($"{Path.GetDirectoryName(path)}\\{Path.GetFileNameWithoutExtension(path)}_{whichTextureID}.tim",0,true);
            TIM.Texture paramTex = parametersOnly.GetParameters;
            int terminator = bTriangle ? 3 : 4;
            for (int i = 0; i < terminator; i++)
            {
                double u = br.ReadByte() / (float)paramTex.Width;
                double v = br.ReadByte() / (float)paramTex.Height;
                uvList.Add(
                    $"vt {u.ToString().Replace(',', '.')} {v.ToString().Replace(',', '.')}");
            }
            if (bTriangle)
            {
                br.ReadByte();
                br.ReadByte();
            }
            uvs = uvList.ToArray();
        }

        private string CreateMatTag()
        {
            char A = (char) 0x41;
            A += (char)(br.ReadUInt16() & 0x0F);
            whichTextureID = (byte) (A - 'A');
            fs.Seek(8, SeekOrigin.Current);            
            return $"usemtl mat{A}";
        }

        private void ReadVertices()
        {
            vertices = new Vertex[numofverts];
            fs.Seek(vertsoffset+modeloffset, SeekOrigin.Begin);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].X = br.ReadInt16()/200.0f;
                vertices[i].Z = br.ReadInt16()/200.0f;
                vertices[i].Y = br.ReadInt16()/200.0f;
                vertices[i].W = br.ReadInt16();
            }
        }
    }
}
