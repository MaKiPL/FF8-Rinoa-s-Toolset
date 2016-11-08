using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SerahToolkit_SharpGL.FF8_Core
{
    class mch
    {
        private string path;
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
            for (int i = 0; i > textureoffsets.Length; i++)
            {
                ; //TIM
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
            for (int i = 0; i < vertices.Length; i++)
            {
                wavefront.Add(
                    $"v {vertices[i].X.ToString().Replace(',', '.')} {vertices[i].Z.ToString().Replace(',', '.')} {vertices[i].Y.ToString().Replace(',', '.')}");
            }
            wavefront.Add("g triangles");
            wavefront.AddRange(triangless);
            wavefront.Add("g quads");
            wavefront.AddRange(quadss);
            string[] obj = wavefront.ToArray();
            SaveFileDialog sfd = new SaveFileDialog {Filter = "*.obj|*.obj"};
            if (sfd.ShowDialog() != DialogResult.OK) return;
            File.WriteAllLines(sfd.FileName, obj);
        }

        private void ReadFaces()
        {
            fs.Seek(facesoffset + modeloffset, SeekOrigin.Begin);
            List<string> triangles = new List<string>();
            List<string> quads = new List<string>();
            for (int i = 0; i < numoffaces; i++)
            {
                uint testvar = br.ReadUInt32();
                bool bisTriangle = testvar == (uint)0x25010607;
                fs.Seek(8, SeekOrigin.Current);
                string s;
                s = bisTriangle
                    ? $"f {br.ReadUInt16()+1} {br.ReadUInt16()+1} {br.ReadUInt16()+1}"
                    : $"f {br.ReadUInt16()+1} {br.ReadUInt16()+1}";
                if (bisTriangle)
                    br.ReadUInt16();
                else
                {
                    ushort d = br.ReadUInt16();
                    s += $" {br.ReadUInt16()+1} {d+1}";
                }
                fs.Seek(44, SeekOrigin.Current);
                if(bisTriangle)
                    triangles.Add(s);
                else quads.Add(s);
            }
            this.triangless = triangles.ToArray();
            this.quadss = quads.ToArray();
        }

        private void ReadVertices()
        {
            vertices = new Vertex[numofverts];
            fs.Seek(vertsoffset+modeloffset, SeekOrigin.Begin);
            for (int i = 0; i < vertices.Length; i++)
            {
                vertices[i].X = br.ReadInt16()/500.0f;
                vertices[i].Z = br.ReadInt16()/500.0f;
                vertices[i].Y = br.ReadInt16()/500.0f;
                vertices[i].W = br.ReadInt16()/500.0f;
            }
        }
    }
}
