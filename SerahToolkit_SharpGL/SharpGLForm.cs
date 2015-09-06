using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Serialization;
using SharpGL.Enumerations;

namespace SerahToolkit_SharpGL
{
    public partial class SharpGLForm : Form
    {
        public SharpGLForm()
        {
            InitializeComponent();
            OpenGL gl = this.openGLControl.OpenGL;
            gl.Enable(OpenGL.GL_TEXTURE_2D);
        }

        private List<string> PathModels;
        private string RailPath;
        private string LastKnownPath;
        private int LastKnownTIM;
        private Bitmap bmp;
        private Bitmap bmp2;
        private int State;

        private GF_enviro gf;

        private const int State_BattleStageUV = 0;
        private const int State_RailDraw = 1;
        private const int State_wmset = 2;
        private const int State_wmsetModel = 3;
        private const int State_GFenviro = 4;


        OpenGL gl;
        List<Polygon> polygons = new List<Polygon>();
        SharpGL.SceneGraph.Cameras.PerspectiveCamera camera = new SharpGL.SceneGraph.Cameras.PerspectiveCamera();



        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            //  Get the OpenGL object, for quick access.
            gl = this.openGLControl.OpenGL;

            //  Clear and load the identity.
            gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            gl.LoadIdentity();

            //  View from a bit away the y axis and a few units above the ground.

            gl.LookAt(-10, 10, -10, 0, 0, 0, 0, 1, 0);
            gl.Rotate(0.0f, trackBar1.Value, trackBar2.Value);

            //  Move the objects down a bit so that they fit in the screen better.
            double x_trans = (double)trackBar3.Value / 100f;
            double y_trans = (double)trackBar4.Value / 100f;
            double z_trans = (double)trackBar5.Value / 100f;
            gl.Translate(x_trans, y_trans, z_trans);

            //  Draw every polygon in the collection.
            foreach (Polygon polygon in polygons)
                {
                if (polygon.IsEnabled)
                    {
                    polygon.PushObjectSpace(gl);
                    polygon.Render(gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                    polygon.PopObjectSpace(gl);
                    }
                }
        }



        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            //  TODO: Initialise OpenGL here.

            //  Get the OpenGL object.
            OpenGL gl = openGLControl.OpenGL;
            //  Set the clear color.
            gl.ClearColor(0, 0, 0, 0);
            openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
            openGLControl.OpenGL.Enable(OpenGL.GL_LIGHTING);
            openGLControl.OpenGL.Enable(OpenGL.GL_LIGHT0);
            openGLControl.OpenGL.Enable(OpenGL.GL_COLOR_MATERIAL);
            openGLControl.OpenGL.Enable(OpenGL.GL_BLEND);
            openGLControl.OpenGL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
        }

        private void openGLControl_Load(object sender, EventArgs e)
        {

        }

        public void Render3D(int WMSET_int = 0)
        {
            polygons.Clear();
            if (State == State_BattleStageUV)
            {
                
                foreach (string s in PathModels)
                {
                    Scene scene = SerializationEngine.Instance.LoadScene(s);
                    if (scene != null)
                    {
                        foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                        {
                            //  Get the bounds of the polygon. 
/*
                            BoundingVolume boundingVolume = polygon.BoundingVolume;
                            float[] extent = new float[3];
                            polygon.BoundingVolume.GetBoundDimensions(out extent[0], out extent[1], out extent[2]); 
                             

                            
                            //  Get the max extent.
                            float maxExtent = extent.Max(); 

                            //  Scale so that we are at most 10 units in size.
                            float scaleFactor = maxExtent > 20 ? 10.0f / maxExtent : 1;
                            polygon.Transformation.ScaleX = scaleFactor;
                            polygon.Transformation.ScaleY = scaleFactor;
                            polygon.Transformation.ScaleZ = scaleFactor;
                            polygon.Freeze(openGLControl.OpenGL); */
                            polygons.Add(polygon);
                        }
                    }
                }
            }
           if(State == State_RailDraw)
            {
                string PathOFDe = Path.GetDirectoryName(RailPath);
                PathOFDe += string.Format(@"\{0}_{1}.obj", Path.GetFileNameWithoutExtension(RailPath), listBox1.Items[listBox1.SelectedIndex]);
                gl.ClearColor(0, 0, 0, 0);
                openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Points);
                Scene scene = SerializationEngine.Instance.LoadScene(PathOFDe);
                    if (scene != null)
                    {
                        foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                        {
                        polygon.IsEnabled = true;
                        polygons.Add(polygon);
                        }
                    }
            }
           if(State == State_wmset)
            {
                //LOGIC HERE
                //WMSET_int
            } 

           if(State == State_wmsetModel)
            {
                string pathOFDe = Path.GetDirectoryName(LastKnownPath) + @"\" + Path.GetFileNameWithoutExtension(LastKnownPath) + "_" + Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]) + "_q.obj";
                if (File.Exists(pathOFDe))
                {
                    Scene scene = SerializationEngine.Instance.LoadScene(pathOFDe);
                    if (scene != null)
                    {
                        foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                        {
                            polygon.IsEnabled = true;
                            polygons.Add(polygon);
                        }
                    }
                }
                pathOFDe = Path.GetDirectoryName(LastKnownPath) + @"\" + Path.GetFileNameWithoutExtension(LastKnownPath) + "_" + Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]) + "_t.obj";
                if (File.Exists(pathOFDe))
                {
                    Scene scene = SerializationEngine.Instance.LoadScene(pathOFDe);
                    if (scene != null)
                    {
                        foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                        {
                            polygon.IsEnabled = true;
                            polygons.Add(polygon);
                        }
                    }
                }
            }
        }

        private void showFPSToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (openGLControl.DrawFPS)
            {
                openGLControl.DrawFPS = false;
                showFPSToolStripMenuItem.Checked = false;
            }
            else
            {
                openGLControl.DrawFPS = true;
                showFPSToolStripMenuItem.Checked = true;
            }
        }

        private void wireframeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            texturedWLightToolStripMenuItem.Checked = false;
            wireframeToolStripMenuItem.Checked = true;
            texturedToolStripMenuItem.Checked = false;
            openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Lines);
            openGLControl.OpenGL.Disable(OpenGL.GL_LIGHTING);
        }

        private void texturedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            texturedWLightToolStripMenuItem.Checked = false;
            texturedToolStripMenuItem.Checked = true;
            wireframeToolStripMenuItem.Checked = false;
            openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
            openGLControl.OpenGL.Enable(OpenGL.GL_BLEND);
            openGLControl.OpenGL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            openGLControl.OpenGL.Enable(OpenGL.GL_LIGHTING);
            openGLControl.OpenGL.Enable(OpenGL.GL_LIGHT0);
            openGLControl.OpenGL.Enable(OpenGL.GL_COLOR_MATERIAL);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open FF8 .X stage model (a0stgXXX.x)";
            ofd.Filter = "Final Fantasy VIII stage (.x)|*.X";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                State = State_BattleStageUV;
                exportToolStripMenuItem.Enabled = true;
                importToolStripMenuItem.Enabled = true;
                SetLines(false);
                BattleStage bs = new BattleStage(ofd.FileName);
                listBox1.Items.Clear();
                UpdateSTATUS(ofd.FileName);
                bs.Process(true);

                //DEBUG INSTANCE FOR BS+Texture

                pictureBox1.Image = bs.GetTexture();
                pictureBox1.BackgroundImage = bs.GetTexture();
                bmp = bs.GetTexture(); //FOR edit
                bmp2 = bs.GetTexture(); //FOR RESCUE
                LastKnownPath = ofd.FileName;
                LastKnownTIM = bs.GetLastTIM();

                //END OF DEBUG

                string PathOFD = null;
                PathModels = new List<string>();
                foreach(int i in bs.GetArrayOfObjects())
                {
                    listBox1.Items.Add(i.ToString());
                    PathOFD = Path.GetDirectoryName(ofd.FileName);
                    PathOFD += string.Format(@"\{0}_{1}_t.obj", Path.GetFileNameWithoutExtension(ofd.FileName), i.ToString());
                    if(File.Exists(PathOFD))
                        PathModels.Add(PathOFD);

                    string PathOFD2 = Path.GetDirectoryName(ofd.FileName);
                    PathOFD2 += string.Format(@"\{0}_{1}_q.obj", Path.GetFileNameWithoutExtension(ofd.FileName), i.ToString());
                    if (File.Exists(PathOFD2))
                        PathModels.Add(PathOFD2);

                    if (File.Exists(PathOFD) && File.Exists(PathOFD2))
                        listBox1.Items.Add(i.ToString());
                }
                Render3D();
                    BattleStage_listbox(true);
            }
            
            
        }

        private void texturedWLightToolStripMenuItem_Click(object sender, EventArgs e)
        {
            texturedWLightToolStripMenuItem.Checked = true;
            texturedToolStripMenuItem.Checked = false;
            wireframeToolStripMenuItem.Checked = false;
            openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
            openGLControl.OpenGL.Enable(OpenGL.GL_BLEND);
            openGLControl.OpenGL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            openGLControl.OpenGL.Disable(OpenGL.GL_LIGHTING);
            openGLControl.OpenGL.Disable(OpenGL.GL_LIGHT0);
            //openGLControl.OpenGL.Enable(OpenGL.GL_COLOR_MATERIAL);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 1)
                goto endofcode;
            switch (State)
            {
                case State_BattleStageUV:
                    BattleStage_listbox(false);
                    break;
                case State_RailDraw:
                    Rail_listbox();
                    break;
                case State_wmset:
                    WMSET_Listbox();
                    break;
                case State_wmsetModel:
                    WMSETmod_listbox();
                    break;
                case State_GFenviro:
                    GFLogic();
                    break;
                default:
                    goto endofcode;
            }


            endofcode:
            ;
        }

        private void WMSET_Listbox()
        {
            //LOGIC HERE
        }

        private void BattleStage_listbox(bool bGenerateTextures)
        {
            if (listBox1.SelectedIndex == -1)
                listBox1.SelectedIndex = 0;
            bmp = new Bitmap(pictureBox1.BackgroundImage);
            Graphics g = Graphics.FromImage(bmp);
            int selected = Convert.ToInt32(listBox1.SelectedItems[0]);
            BattleStage bs = new BattleStage("UV");
            Tuple<List<double>, List<double>,int> UV = bs.GetUVpoints(selected, LastKnownPath, LastKnownTIM);
            List<Point> UV_point = new List<Point>();



            Pen pen = new Pen(Color.White, 1.0f);

            int index = 0;

            //GET RESOLUTION
            Tuple<int, int> TexResTuple = bs.GetTextureRes();
            int width = TexResTuple.Item1;
            int height = TexResTuple.Item2;


            //Hide logic
            checkBox1.Checked = !polygons[listBox1.SelectedIndex].IsEnabled;

            if (bGenerateTextures)
            {
                StageTexture st = new StageTexture(0, width, height);


                for (int i = 0; i != listBox1.Items.Count; i++)
                {
                    //Tuple<List<double>, List<double>> UV = GetUVpoints(SegmentArray[i], LastKnownPath, LastKnownTIM);
                    UV = bs.GetUVpoints(int.Parse(listBox1.Items[i].ToString()), LastKnownPath, LastKnownTIM);
                    int clute = UV.Item3;

                    string PathText = Path.GetDirectoryName(LastKnownPath);
                    if(clute!=0)
                        PathText = PathText + @"\" + Path.GetFileNameWithoutExtension(LastKnownPath) + @"_" + clute + ".png";
                    else
                        PathText = PathText + @"\" + Path.GetFileNameWithoutExtension(LastKnownPath) + ".png";

                    double U1Min = UV.Item1.Min(); double U1Max = UV.Item1.Max();
                    double V1Min = UV.Item2.Min(); double V1Max = UV.Item2.Max();

                    double X1 = (((U1Min * 100) * width) / 100);
                    double Y1 = (((V1Min * 100) * height) / 100);
                    double X2 = (((U1Max * 100) * width) / 100);
                    double Y2 = (((V1Max * 100) * height) / 100);

                    Point TopLeft = new Point((int)(Math.Round(X1)), height - (int)(Math.Round(Y2)));
                    Point TopRight = new Point((int)(Math.Round(X2)), height - (int)(Math.Round(Y2)));
                    Point BottomLeft = new Point((int)(Math.Round(X1)), height - (int)(Math.Round(Y1)));
                    Point BottomRight = new Point((int)(Math.Round(X2)), height - (int)(Math.Round(Y1)));

                    st.MixTextures(TopLeft, TopRight, BottomLeft, BottomRight, PathText, bmp);

                    if (File.Exists(PathText))
                    {
                        Bitmap LoadBMP = new Bitmap(PathText);


                        for (int y = height - (int)(Math.Round(Y2)); y < height - (int)(Math.Round(Y1)); y++)
                        {
                            for (int x = (int)(Math.Round(X1)); x < (int)(Math.Round(X2)); x++)
                            {
                                bmp.SetPixel(x, y, LoadBMP.GetPixel(x, y));
                                if (index < width * height - 4)
                                    index++;

                            }
                        }
                    }

                     pictureBox1.Image = bmp;
                    
                }
                string PathTexte = Path.GetDirectoryName(LastKnownPath);
                PathTexte = PathTexte + @"\" + Path.GetFileNameWithoutExtension(LastKnownPath) + "_col.png";
                if (File.Exists(PathTexte))
                    File.Delete(PathTexte);
                bmp.MakeTransparent(Color.Black);
                bmp.Save(PathTexte);

                PathTexte = Path.GetDirectoryName(LastKnownPath);
                PathTexte = PathTexte + @"\" + Path.GetFileNameWithoutExtension(LastKnownPath) + ".MTL";

                string[] newfile = File.ReadAllLines(PathTexte);
                newfile[newfile.Length-1] = "map_Kd " + Path.GetFileNameWithoutExtension(LastKnownPath) + "_col.png";
                File.WriteAllLines(PathTexte, newfile);
                pictureBox1.BackgroundImage = bmp;
                Render3D();

               
            }
            if (!bGenerateTextures)
            {
                while (true)
                {

                    double X1 = (((UV.Item1[index] * 100) * width) / 100);
                    double Y1 = (((UV.Item2[index] * 100) * height) / 100);
                    double X2 = (((UV.Item1[index + 1] * 100) * width) / 100);
                    double Y2 = (((UV.Item2[index + 1] * 100) * height) / 100);

                    /*
                    Test
                    double X1 = (UV.Item1[index] * pictureBox1.Size.Height);
                    double Y1 = (UV.Item2[index] * pictureBox1.Size.Width);
                    double X2 = (UV.Item1[index+1] * pictureBox1.Size.Height);
                    double Y2 = (UV.Item2[index+1] * pictureBox1.Size.Width); */

                    Point XY1 = new Point((int)(Math.Round(X1)), 256 - (int)(Math.Round(Y1)));
                    Point XY2 = new Point((int)(Math.Round(X2)), 256 - (int)(Math.Round(Y2)));
                    UV_point.Add(XY1);
                    UV_point.Add(XY2);

                    g.DrawLine(pen, UV_point[index], UV_point[index + 1]);

                    if (index >= UV.Item1.Count - 3 && index >= UV.Item2.Count - 3)
                        break;
                    else
                        index += 2;
                }
                g.Dispose();
                pictureBox1.Image = bmp;
            }


        }

        private void Rail_listbox()
        {
            Rail rail = new Rail(RailPath);
            //polygons.Add(rail.rail((int)listBox1.Items[listBox1.SelectedIndex]));
            rail.rail((int)listBox1.Items[listBox1.SelectedIndex]);
            Render3D();
        }

        private void openToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetLines(true);
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open FF8 rail.obj file (rail.obj)";
            ofd.Filter = "Final Fantasy VIII train file (rail.obj)|rail.obj";
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                State = State_RailDraw;
                RailPath = ofd.FileName;
                UpdateSTATUS(RailPath);
                Rail rail = new Rail(ofd.FileName);
                listBox1.Items.Clear();
                foreach(var i in rail.GetRails())
                {
                    listBox1.Items.Add(i);
                }
            }
        }

        private void polygonModeFacesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
        }

        private void polygonModePointsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetLines(true);   
        }

        private void SetLines(bool bLines)
        {
            if(bLines)
                openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Points);
            else
                openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);

        }

        private void button1_Click(object sender, EventArgs e)
        {
            trackBar1.Value = 750;
            trackBar2.Value = 723;
            trackBar3.Value = 0;
            trackBar4.Value = 0;
            trackBar5.Value = 0;
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(State == State_BattleStageUV)
            {
                if (checkBox1.Checked == true)
                    polygons[listBox1.SelectedIndex].IsEnabled = false;
                else
                    polygons[listBox1.SelectedIndex].IsEnabled = true;
            }
        }



        /* poly freeze
                    foreach (var poly in polygons)
                        poly.Freeze(openGLControl1.OpenGL);
         */

        public int[] ListOfSegments()
        {
            List<int> a = new List<int>();
            foreach(var i in listBox1.Items)
            {
                a.Add(int.Parse(i.ToString()));
            }
            return a.ToArray();



        }

        private void dumpRAWDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (State != State_BattleStageUV)
                goto eof;


            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                BattleStage bs = new BattleStage(LastKnownPath);
                int nextoffset;
                if (listBox1.SelectedIndex == listBox1.Items.Count-1)
                    nextoffset = -1;
                else
                    nextoffset = int.Parse(listBox1.Items[listBox1.SelectedIndex + 1].ToString());

                bs.DumpRAW(int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString()),sfd.FileName, nextoffset);
            }


            eof:
                ;
        }

        private void verticesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int offset = int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString());
            BS_Vertices bsv = new BS_Vertices(LastKnownPath, offset);
            bsv.ShowDialog();
        }

        private void rosettaStonetextDecypherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            RosettaStone.RosettaStone rs = new RosettaStone.RosettaStone();
            rs.ShowDialog();
        }

        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "wmsetxx.obj|wmset*.obj";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                LastKnownPath = ofd.FileName;
                unpackToolStripMenuItem.Enabled = true;
                UpdateSTATUS(LastKnownPath);
                State = State_wmset;
                wmset wmset = new wmset(LastKnownPath);
                listBox1.Items.Clear(); //JustInCase
                listBox1.Items.AddRange(wmset._Debug_GetSections().Item1.ToArray());


                Render3D(); //Render3D(INT)
            }
        }


        private void unpackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            wmset wmset = new wmset(LastKnownPath);
            var OffsetList = wmset._Debug_GetSections().Item2.ToArray();
            listBox1.Items.Clear(); //JustInCase
            listBox1.Items.AddRange(wmset._Debug_GetSections().Item1.ToArray());

            for (int i=0; i!=wmset.d_OffsetCount; i++)
            {
                string buildPath = Path.GetDirectoryName(LastKnownPath);
                buildPath += string.Format("\\{0}.Section{1}", Path.GetFileNameWithoutExtension(LastKnownPath), (i+1).ToString());
                if (File.Exists(buildPath))
                    File.Delete(buildPath);
                using (var fs = new FileStream(LastKnownPath, FileMode.Open))
                {
                    using (var fs_w = new FileStream(buildPath, FileMode.OpenOrCreate))
                    {
                        if (i+1 != OffsetList.Length)
                        {
                            byte[] tempMem = new byte[OffsetList[i+1]-OffsetList[i]];
                            fs.Seek(OffsetList[i], SeekOrigin.Begin);
                            fs.Read(tempMem, 0, tempMem.Length);
                            fs.Close();
                            fs_w.Write(tempMem, 0, tempMem.Length);
                            fs_w.Close();
                        }
                        else
                        {
                            FileInfo fi = new FileInfo(LastKnownPath);
                            long EOF = fi.Length;
                            byte[] tempMem = new byte[EOF - OffsetList[i]];
                            fs.Seek(OffsetList[i], SeekOrigin.Begin);
                            fs.Read(tempMem, 0, tempMem.Length);
                            fs.Close();
                            fs_w.Write(tempMem, 0, tempMem.Length);
                            fs_w.Close();
                        }
                    }
                }
            }
            UpdateSTATUS("Unpacked wmsetus.obj");
        }

        private void UpdateSTATUS(string status)
        {
            toolStripStatusLabel2.Text = status;
        }

        private void State_Menu(int State) //?
        {
            switch (State)
            {
                case State_BattleStageUV:
                    {
                        //DO LOGIC
                        break;
                    }
                case State_RailDraw:
                    {
                        //DO LOGIC
                        break;
                    }
                case State_wmset:
                    {
                        //DO LOGIC
                        break;
                    }
                default:
                    break;
            }
        }

        private void section1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            State = State_wmsetModel;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "wmsetxx.obj/Sector16|wmset**.Section16";
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                wmset wmset = new wmset(ofd.FileName);
                LastKnownPath = ofd.FileName;
                UpdateSTATUS(LastKnownPath);
                listBox1.Items.Clear();
                foreach(int i in wmset.ProduceOffset_sec16())
                {
                    listBox1.Items.Add(i);
                }
                
            }
        }

        private void WMSETmod_listbox()
        {
            wmset wmset = new wmset(LastKnownPath);
            int SelectedModel = Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]);
            wmset.Sector16(SelectedModel, listBox1.SelectedIndex);
            Bitmap bmp = wmset.GetTexture();
            pictureBox1.Image = bmp;
            Render3D();
        }

        private void cFF8SearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Analize_CFF8search ff8s = new Analize_CFF8search();
            ff8s.ShowDialog();
        }

        private void convertToOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            toSingleOBJ tso = new toSingleOBJ(LastKnownPath, listBox1.Items.Count);
            tso.JustDoIt();
        }

        private void environmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "GF Mag files mag*|mag*";
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                gf = new GF_enviro(ofd.FileName);
                LastKnownPath = ofd.FileName;
                if (!gf.bValidHeader())
                    MessageBox.Show("Bad file!");
                else
                {
                    State = State_GFenviro;
                    listBox1.Items.Clear();
                    foreach (int a in gf.PopulateOffsets())
                    {
                        listBox1.Items.Add(a);
                    }
                }
            }
        }

        private void GFLogic()
        {
            gf.ProcessGF((int)listBox1.Items[listBox1.SelectedIndex]);
        }

        private void oBJToFF8ParserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Parser parser = new Parser(Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]));
            parser.ShowDialog();
        }
    }
}
