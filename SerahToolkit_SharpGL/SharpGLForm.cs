using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using SharpGL;
using SharpGL.SceneGraph;
using SharpGL.SceneGraph.Primitives;
using SharpGL.Serialization;
using SharpGL.Enumerations;

namespace SerahToolkit_SharpGL
{
    public partial class SharpGlForm : Form
    {
        public SharpGlForm()
        {
            InitializeComponent();
            OpenGL gl = openGLControl.OpenGL;
            gl.Enable(OpenGL.GL_TEXTURE_2D);
        }

        private List<string> _pathModels;
        private string _railPath;
        private string _lastKnownPath;
        private int _lastKnownTim;
        private Bitmap _bmp;
        private Bitmap _bmp2;
        private int _state;

        private GfEnviro _gf;

        private const int StateBattleStageUv = 0;
        private const int StateRailDraw = 1;
        private const int StateWmset = 2;
        private const int StateWmsetModel = 3;
        private const int StateGFenviro = 4;
        private const int StateWmx = 5;
        public static string GFEnviro;


        OpenGL _gl;
        readonly List<Polygon> _polygons = new List<Polygon>();
        SharpGL.SceneGraph.Cameras.PerspectiveCamera _camera = new SharpGL.SceneGraph.Cameras.PerspectiveCamera();

        [DllImport("kernel32")]
        static extern bool AllocConsole();



        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            //  Get the OpenGL object, for quick access.
            _gl = openGLControl.OpenGL;

            //  Clear and load the identity.
            _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            _gl.LoadIdentity();

            //  View from a bit away the y axis and a few units above the ground.

            _gl.LookAt(-10, 10, -10, 0, 0, 0, 0, 1, 0);
            _gl.Rotate(0.0f, trackBar1.Value, trackBar2.Value);

            //  Move the objects down a bit so that they fit in the screen better.
            double xTrans = (double)trackBar3.Value / 100f;
            double yTrans = (double)trackBar4.Value / 100f;
            double zTrans = (double)trackBar5.Value / 100f;
            _gl.Translate(xTrans, yTrans, zTrans);

            //  Draw every polygon in the collection.
            foreach (Polygon polygon in _polygons)
                {
                if (polygon.IsEnabled)
                    {
                    polygon.PushObjectSpace(_gl);
                    polygon.Render(_gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                    polygon.PopObjectSpace(_gl);
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
            AllocConsole();
            Console.WriteLine("===========Welcome!==========\nPlease do not close this console, it's connected to software. It may come handy.\nThis window will also let you see the output of FileScanner and WMX section.\n\n\n");
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
        }

        private void Render3D(int wmsetInt = 0)
        {
            _polygons.Clear();
            if (_state == StateBattleStageUv)
            {
                
                foreach (string s in _pathModels)
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
                            _polygons.Add(polygon);
                        }
                    }
                }
            }
           if(_state == StateRailDraw)
            {
                string pathOfDe = Path.GetDirectoryName(_railPath);
                pathOfDe +=
                    $@"\{Path.GetFileNameWithoutExtension(_railPath)}_{listBox1.Items[listBox1.SelectedIndex]}.obj";
                _gl.ClearColor(0, 0, 0, 0);
                openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Points);
                Scene scene = SerializationEngine.Instance.LoadScene(pathOfDe);
                    if (scene != null)
                    {
                        foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                        {
                        polygon.IsEnabled = true;
                        _polygons.Add(polygon);
                        }
                    }
            }
            if (_state == StateGFenviro && !_gf._bForceNotDraw)
            {
                _gl.ClearColor(0, 0, 0, 0);
                Scene scene = SerializationEngine.Instance.LoadScene(GFEnviro);
                if (scene != null)
                {
                    foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                    {
                        polygon.IsEnabled = true;
                        _polygons.Add(polygon);
                    }
                }
            }
            else
                _gl.ClearColor(0, 0, 0, 0);

           if(_state == StateWmset)
            {
                //LOGIC HERE
                //WMSET_int
            } 

           if(_state == StateWmsetModel)
            {
                string pathOfDe = Path.GetDirectoryName(_lastKnownPath) + @"\" + Path.GetFileNameWithoutExtension(_lastKnownPath) + "_" + Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]) + "_q.obj";
                if (File.Exists(pathOfDe))
                {
                    Scene scene = SerializationEngine.Instance.LoadScene(pathOfDe);
                    if (scene != null)
                    {
                        foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                        {
                            polygon.IsEnabled = true;
                            _polygons.Add(polygon);
                        }
                    }
                }
                pathOfDe = Path.GetDirectoryName(_lastKnownPath) + @"\" + Path.GetFileNameWithoutExtension(_lastKnownPath) + "_" + Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]) + "_t.obj";
                if (File.Exists(pathOfDe))
                {
                    Scene scene = SerializationEngine.Instance.LoadScene(pathOfDe);
                    if (scene != null)
                    {
                        foreach (var polygon in scene.SceneContainer.Traverse<Polygon>())
                        {
                            polygon.IsEnabled = true;
                            _polygons.Add(polygon);
                        }
                    }
                }
            }
        }

        public void ForceRendererUpdate()
        {
            Render3D();
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

        public void BSVertEditor_Update(string ofd)
        {
            BattleStage bs = new BattleStage(ofd);
            listBox1.Items.Clear();
            bs.Process(false, true);
            BS_UpdateObjects(bs.GetArrayOfObjects(), ofd);
            Render3D();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open FF8 .X stage model (a0stgXXX.x)",
                Filter = "Final Fantasy VIII stage (.x)|*.X"
            };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                Console.WriteLine("BS: Opening file...");
                _state = StateBattleStageUv;
                exportToolStripMenuItem.Enabled = true;
                importToolStripMenuItem.Enabled = true;
                SetLines(false);
                BattleStage bs = new BattleStage(ofd.FileName);
                listBox1.Items.Clear();
                UpdateStatus(ofd.FileName);
                bs.Process(true);

                //DEBUG INSTANCE FOR BS+Texture

                pictureBox1.Image = bs.GetTexture();
                pictureBox1.BackgroundImage = bs.GetTexture();
                _bmp = bs.GetTexture(); //FOR edit
                _bmp2 = bs.GetTexture(); //FOR RESCUE
                _lastKnownPath = ofd.FileName;
                _lastKnownTim = bs.GetLastTim();

                //END OF DEBUG

                _pathModels = new List<string>();
                foreach(int i in bs.GetArrayOfObjects())
                {
                    listBox1.Items.Add(i.ToString());
                    var pathOfd = Path.GetDirectoryName(ofd.FileName);
                    pathOfd += $@"\{Path.GetFileNameWithoutExtension(ofd.FileName)}_{i.ToString()}_t.obj";
                    if(File.Exists(pathOfd))
                        _pathModels.Add(pathOfd);

                    string pathOfd2 = Path.GetDirectoryName(ofd.FileName);
                    pathOfd2 += $@"\{Path.GetFileNameWithoutExtension(ofd.FileName)}_{i.ToString()}_q.obj";
                    if (File.Exists(pathOfd2))
                        _pathModels.Add(pathOfd2);

                    if (File.Exists(pathOfd) && File.Exists(pathOfd2))
                        listBox1.Items.Add(i.ToString());
                }
                Render3D();
                    BattleStage_listbox(true);
                pictureBox1.BackgroundImage = null;
            }
            
            
        }


        public void BS_UpdateObjects(int[] bs, string ofd)
        {
            _pathModels = new List<string>();
            foreach (int i in bs)
            {
                listBox1.Items.Add(i.ToString());
                var pathOfd = Path.GetDirectoryName(ofd);
                pathOfd += $@"\{Path.GetFileNameWithoutExtension(ofd)}_{i.ToString()}_t.obj";
                if (File.Exists(pathOfd))
                    _pathModels.Add(pathOfd);

                string pathOfd2 = Path.GetDirectoryName(ofd);
                pathOfd2 += $@"\{Path.GetFileNameWithoutExtension(ofd)}_{i.ToString()}_q.obj";
                if (File.Exists(pathOfd2))
                    _pathModels.Add(pathOfd2);

                if (File.Exists(pathOfd) && File.Exists(pathOfd2))
                    listBox1.Items.Add(i.ToString());
            }
            Render3D();
            //BattleStage_listbox(true);
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
            switch (_state)
            {
                case StateBattleStageUv:
                    BattleStage_listbox(false);
                    break;
                case StateRailDraw:
                    Rail_listbox();
                    break;
                case StateWmset:
                    WMSET_Listbox();
                    break;
                case StateWmsetModel:
                    WMSETmod_listbox();
                    break;
                case StateGFenviro:
                    GfLogic();
                    break;
                case StateWmx:
                    WMX_list();
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
            Image nullImageRes = _bmp2; //bug fix by trick
            _bmp = new Bitmap(nullImageRes);
            Graphics g = Graphics.FromImage(_bmp);
            int selected = Convert.ToInt32(listBox1.SelectedItems[0]);
            BattleStage bs = new BattleStage("UV");
            Tuple<List<double>, List<double>,int> uv = bs.GetUVpoints(selected, _lastKnownPath, _lastKnownTim);
            List<Point> uvPoint = new List<Point>();



            Pen pen = new Pen(Color.White, 1.0f);

            int index = 0;

            //GET RESOLUTION
            Tuple<int, int> texResTuple = bs.GetTextureRes();
            int width = texResTuple.Item1;
            int height = texResTuple.Item2;


            //Hide logic
            checkBox1.Checked = !_polygons[listBox1.SelectedIndex].IsEnabled;

            if (bGenerateTextures)
            {
                Console.WriteLine($"BS: Mixing textures...");
                StageTexture st = new StageTexture(0, width, height);

                Console.WriteLine($"BS: Collecting UV data and preparing bounding boxes");
                for (int i = 0; i != listBox1.Items.Count; i++)
                {
                    //Tuple<List<double>, List<double>> UV = GetUVpoints(SegmentArray[i], LastKnownPath, LastKnownTIM);
                    uv = bs.GetUVpoints(int.Parse(listBox1.Items[i].ToString()), _lastKnownPath, _lastKnownTim);
                    int clute = uv.Item3;

                    string pathText = Path.GetDirectoryName(_lastKnownPath);
                    if(clute!=0)
                        pathText = pathText + @"\" + Path.GetFileNameWithoutExtension(_lastKnownPath) + @"_" + clute + ".png";
                    else
                        pathText = pathText + @"\" + Path.GetFileNameWithoutExtension(_lastKnownPath) + ".png";

                    double u1Min = uv.Item1.Min(); double u1Max = uv.Item1.Max();
                    double v1Min = uv.Item2.Min(); double v1Max = uv.Item2.Max();

                    double x1 = Math.Floor(((u1Min * 100) * width) / 100);
                    double y1 = Math.Floor(((v1Min * 100) * height) / 100) ;
                    double x2 = Math.Floor(((u1Max * 100) * width) / 100);
                    double y2 = Math.Floor(((v1Max * 100) * height) / 100) ;

                    x1 = x1 <= 0 ? 1 : x1; x2 = x2 <= 0 ? 1 : x2;

                    Point topLeft = new Point((int)(Math.Round(x1)-1), height - (int)(Math.Round(y2)));
                    Point topRight = new Point((int)(Math.Round(x2)-1), height - (int)(Math.Round(y2)));
                    Point bottomLeft = new Point((int)(Math.Round(x1)), height - (int)(Math.Round(y1)));
                    Point bottomRight = new Point((int)(Math.Round(x2)), height - (int)(Math.Round(y1)));

                    if (File.Exists(pathText))
                    {
                        Console.WriteLine($"BS: Mixing {pathText}");
                        //LoadBMP - COPIES FROM
                        //BMP - The mixed final
                        Bitmap loadBmp = new Bitmap(pathText);
                        PixelFormat pf = PixelFormat.Format24bppRgb;
                        /*Rectangle rectangle = new Rectangle((int) Math.Round(X1), (int) Math.Round(Y1),
                            (int) Math.Round(X2), height - (int) Math.Round(Y2));*/
                        int wid = (topRight.X - topLeft.X) + 4;
                        int hei = (bottomLeft.Y - topLeft.Y) + 4;
                        //Wid = Wid > LoadBMP.Width - Wid ? Wid - 2 : Wid;
                        //Hei = Hei > LoadBMP.Height - Hei ? Hei - 2 : Hei;
                        wid = topLeft.X + wid > width ? wid - 4 : wid;
                        hei = bottomRight.Y + hei > height ? hei - 4 : hei;

                        Size sz = new Size(wid,hei);
                        Rectangle rectangle = new Rectangle(topLeft, sz);

                        BitmapData targetBitmapData = _bmp.LockBits(rectangle, ImageLockMode.WriteOnly, pf);
                        BitmapData sourBitmapData = loadBmp.LockBits(rectangle, ImageLockMode.ReadOnly, pf);
                        IntPtr workingptr = targetBitmapData.Scan0;
                        IntPtr sourceptr = sourBitmapData.Scan0;
                        byte[] rawLoadBmp = new byte[sourBitmapData.Stride * sourBitmapData.Height];
                        byte[] rawBmp = new byte[targetBitmapData.Stride * targetBitmapData.Height];

                        Marshal.Copy(workingptr, rawBmp, 0, rawBmp.Length);
                        Marshal.Copy(sourceptr, rawLoadBmp, 0, rawLoadBmp.Length);

                        for (int pixel = 0; pixel != rawLoadBmp.Length; pixel++)
                        {
                            rawBmp[pixel] = rawLoadBmp[pixel];
                        }

                        Marshal.Copy(rawBmp, 0, workingptr, rawBmp.Length);
                        loadBmp.UnlockBits(sourBitmapData);
                        _bmp.UnlockBits(targetBitmapData);

                        /*for (int y = height - (int)(Math.Round(Y2)); y < height - (int)(Math.Round(Y1)); y++)
                        {
                            for (int x = (int)(Math.Round(X1)); x < (int)(Math.Round(X2)); x++)
                            {
                                bmp.SetPixel(x, y, LoadBMP.GetPixel(x, y));
                                if (index < width * height - 4)
                                    index++;
                            }
                        }*/
                        Console.WriteLine($"BS: Mixing finished");
                    }

                     
                    
                }
                pictureBox1.Image = _bmp;
                _bmp2 = _bmp;
                string pathTexte = Path.GetDirectoryName(_lastKnownPath);
                Console.WriteLine($"BS: Saving final texture");
                pathTexte = pathTexte + @"\" + Path.GetFileNameWithoutExtension(_lastKnownPath) + "_col.png";
                if (File.Exists(pathTexte))
                    File.Delete(pathTexte);
                Console.WriteLine($"BS: Setting transparency on final texture");
                _bmp.MakeTransparent(Color.Black);
                _bmp.Save(pathTexte);

                pathTexte = Path.GetDirectoryName(_lastKnownPath);
                pathTexte = pathTexte + @"\" + Path.GetFileNameWithoutExtension(_lastKnownPath) + ".MTL";

                string[] newfile = File.ReadAllLines(pathTexte);
                newfile[newfile.Length-1] = "map_Kd " + Path.GetFileNameWithoutExtension(_lastKnownPath) + "_col.png";
                File.WriteAllLines(pathTexte, newfile);
                pictureBox1.BackgroundImage = _bmp;
                Console.WriteLine($"BS: Finished!");
                Console.WriteLine($"BS: Delivered to renderer.");
                Render3D();

               
            }
            if (!bGenerateTextures)
            {
                Console.WriteLine($"BS: Drawing UV layout");
                while (true)
                {

                    double x1 = (((uv.Item1[index] * 100) * width) / 100);
                    double y1 = (((uv.Item2[index] * 100) * height) / 100);
                    double x2 = (((uv.Item1[index + 1] * 100) * width) / 100);
                    double y2 = (((uv.Item2[index + 1] * 100) * height) / 100);

                    /*
                    Test
                    double X1 = (UV.Item1[index] * pictureBox1.Size.Height);
                    double Y1 = (UV.Item2[index] * pictureBox1.Size.Width);
                    double X2 = (UV.Item1[index+1] * pictureBox1.Size.Height);
                    double Y2 = (UV.Item2[index+1] * pictureBox1.Size.Width); */

                    Point xy1 = new Point((int)(Math.Round(x1)), 256 - (int)(Math.Round(y1)));
                    Point xy2 = new Point((int)(Math.Round(x2)), 256 - (int)(Math.Round(y2)));
                    uvPoint.Add(xy1);
                    uvPoint.Add(xy2);

                    g.DrawLine(pen, uvPoint[index], uvPoint[index + 1]);

                    if (index >= uv.Item1.Count - 3 && index >= uv.Item2.Count - 3)
                        break;
                    else
                        index += 2;
                }
                g.Dispose();
                pictureBox1.Image = _bmp;
            }


        }

        private void Rail_listbox()
        {
            Rail rail = new Rail(_railPath);
            //polygons.Add(rail.rail((int)listBox1.Items[listBox1.SelectedIndex]));
            rail.rail((int)listBox1.Items[listBox1.SelectedIndex]);
            Render3D();
        }

        private void openToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetLines(true);
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open FF8 rail.obj file (rail.obj)",
                Filter = "Final Fantasy VIII train file (rail.obj)|rail.obj"
            };
            if(ofd.ShowDialog()==DialogResult.OK)
            {
                _state = StateRailDraw;
                _railPath = ofd.FileName;
                UpdateStatus(_railPath);
                Rail rail = new Rail(ofd.FileName);
                listBox1.Items.Clear();
                foreach(var i in rail.GetRails())
                {
                    listBox1.Items.Add(i);
                }
                listBox1.SelectedIndex = 0;
                railEditorToolStripMenuItem.Enabled = true;
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
            openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, bLines ? PolygonMode.Points : PolygonMode.Filled);
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
            if(_state == StateBattleStageUv)
            {
                _polygons[listBox1.SelectedIndex].IsEnabled = checkBox1.Checked != true;
            }
        }



        /* poly freeze
                    foreach (var poly in polygons)
                        poly.Freeze(openGLControl1.OpenGL);
         */



        private void dumpRAWDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_state != StateBattleStageUv)
                goto eof;


            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                BattleStage bs = new BattleStage(_lastKnownPath);
                int nextoffset;
                if (listBox1.SelectedIndex == listBox1.Items.Count-1)
                    nextoffset = -1;
                else
                    nextoffset = int.Parse(listBox1.Items[listBox1.SelectedIndex + 1].ToString());

                bs.DumpRaw(int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString()),sfd.FileName, nextoffset);
            }


            eof:
                ;
        }

        private void verticesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int offset = int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString());
            BsVertices bsv = new BsVertices(_lastKnownPath, offset, this);
            bsv.ShowDialog();
        }
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = "wmsetxx.obj|wmset*.obj"};
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                _lastKnownPath = ofd.FileName;
                unpackToolStripMenuItem.Enabled = true;
                UpdateStatus(_lastKnownPath);
                _state = StateWmset;
                Wmset wmset = new Wmset(_lastKnownPath);
                listBox1.Items.Clear(); //JustInCase
                listBox1.Items.AddRange(wmset._Debug_GetSections().Item1.ToArray());


                Render3D(); //Render3D(INT)
            }
        }


        private void unpackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Wmset wmset = new Wmset(_lastKnownPath);
            var offsetList = wmset._Debug_GetSections().Item2.ToArray();
            listBox1.Items.Clear(); //JustInCase
            listBox1.Items.AddRange(wmset._Debug_GetSections().Item1.ToArray());

            for (int i=0; i!=Wmset.DOffsetCount; i++)
            {
                string buildPath = Path.GetDirectoryName(_lastKnownPath);
                buildPath += $"\\{Path.GetFileNameWithoutExtension(_lastKnownPath)}.Section{(i + 1).ToString()}";
                if (File.Exists(buildPath))
                    File.Delete(buildPath);
                using (var fs = new FileStream(_lastKnownPath, FileMode.Open))
                {
                    using (var fsW = new FileStream(buildPath, FileMode.OpenOrCreate))
                    {
                        if (i+1 != offsetList.Length)
                        {
                            byte[] tempMem = new byte[offsetList[i+1]-offsetList[i]];
                            fs.Seek(offsetList[i], SeekOrigin.Begin);
                            fs.Read(tempMem, 0, tempMem.Length);
                            fs.Close();
                            fsW.Write(tempMem, 0, tempMem.Length);
                            fsW.Close();
                        }
                        else
                        {
                            FileInfo fi = new FileInfo(_lastKnownPath);
                            long eof = fi.Length;
                            byte[] tempMem = new byte[eof - offsetList[i]];
                            fs.Seek(offsetList[i], SeekOrigin.Begin);
                            fs.Read(tempMem, 0, tempMem.Length);
                            fs.Close();
                            fsW.Write(tempMem, 0, tempMem.Length);
                            fsW.Close();
                        }
                    }
                }
            }
            UpdateStatus("Unpacked wmsetus.obj");
        }

        private void UpdateStatus(string status)
        {
            toolStripStatusLabel2.Text = status;
        }

        private void section1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = "wmsetxx.obj/Sector16|wmset**.Section16"};
            if(ofd.ShowDialog() == DialogResult.OK)
            {
                _state = StateWmsetModel;
                Wmset wmset = new Wmset(ofd.FileName);
                _lastKnownPath = ofd.FileName;
                UpdateStatus(_lastKnownPath);
                listBox1.Items.Clear();
                foreach(int i in wmset.ProduceOffset_sec16())
                {
                    listBox1.Items.Add(i);
                }
                
            }
        }

        private void WMSETmod_listbox()
        {
            Wmset wmset = new Wmset(_lastKnownPath);
            int selectedModel = Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]);
            wmset.Sector16(selectedModel, listBox1.SelectedIndex);
            Bitmap bmp = wmset.GetTexture();
            pictureBox1.Image = bmp;
            Render3D();
        }

        private void cFF8SearcherToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AnalizeCff8Search ff8S = new AnalizeCff8Search();
            ff8S.ShowDialog();
        }

        private void convertToOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToSingleObj tso = new ToSingleObj(_lastKnownPath, listBox1.Items.Count);
            tso.JustDoIt();
        }

        private void environmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = "GF Mag files mag*|mag*"};
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _gf = new GfEnviro(ofd.FileName);
                UpdateStatus(ofd.FileName);
                _lastKnownPath = ofd.FileName;

                /*if (!_gf.BValidHeader())
                    MessageBox.Show("Bad file!");
                else
                { */
                    _state = StateGFenviro;
                    listBox1.Items.Clear();
                    foreach (int a in _gf.PopulateOffsets())
                    {
                    if (a == 0)
                    {
                        Console.WriteLine("GFWorker: Error! Probably wrong file. No offsets found.");
                        return;
                    }
                        listBox1.Items.Add(a);
                    }
                //}
            }
        }

        private void GfLogic()
        {
            _gf.ProcessGf((int)listBox1.Items[listBox1.SelectedIndex]);
            if (_gf._onlyVertex)
                SetLines(true);
            else
                SetLines(false);
            if(GFEnviro != null)
                Render3D();
        }

        private void WMX_list()
        {
            Wmx wmx = new Wmx(listBox1.SelectedIndex, _lastKnownPath);
        }

        private void parseVerticesForSegmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //TODO
        }


        private void openToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = "wmx.obj|wmx.obj"};
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                _state = StateWmx;
                _lastKnownPath = ofd.FileName;
                PopulateWmx(_lastKnownPath);
            }
        }

        private void PopulateWmx(string ofd)
        {
            int count = 835;
            int size = 0x9000;
            listBox1.Items.Clear();
            for (int i = 0; i != count; i++)
            {
                listBox1.Items.Add(i*size);
            }
        }

        private void rosettaStonetextDecypherToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            RosettaStone.RosettaStone rs = new RosettaStone.RosettaStone();
            rs.Show();
        }

        private void fileScannerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileScanner.FileScanner_Core fsc;
            Console.WriteLine("FileScanner engine started\nYou will find output here");
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() == DialogResult.OK)
            {
                fsc = new FileScanner.FileScanner_Core(fbd.SelectedPath);
                fsc.Start();
            }
            else return;
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save texture";
            sfd.Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {/*
                    ImageFormat imageFormat;

                    switch(Path.GetExtension(sfd.FileName).ToUpper())
                    {
                        case "BMP":
                            imageFormat = ImageFormat.Bmp;
                            break;
                        case "GIF":
                            imageFormat = ImageFormat.Gif;
                            break;
                        case "JPEG":
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        case "JPG":
                            imageFormat = ImageFormat.Jpeg;
                            break;
                        case "PNG":
                            imageFormat = ImageFormat.Png;
                            break;
                        case "TIFF":
                            imageFormat = ImageFormat.Tiff;
                            break;
                        default:
                            Console.WriteLine("Wrong extension. Saving as PNG");
                            imageFormat = ImageFormat.Png;
                            break;
                    }
                    */
                    pictureBox1.Image.Save(sfd.FileName/*, imageFormat*/);
                    Console.WriteLine("Texture saved.");
                }
                catch(Exception ee)
                {
                    Console.WriteLine($"We had an error with saving texture. Exception:{Environment.NewLine}{ee.ToString()}");
                }
            }
            else
                Console.WriteLine("Cancelled texture save as...");
        }

        private void railEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(_state != StateRailDraw)
            {
                Console.WriteLine("ERROR! Did you try to open RailEditor working on other file!?");
                return;
            }
            int getRot = int.Parse(listBox1.SelectedItem.ToString());
            Console.WriteLine($"Starting rail editor for segment {listBox1.SelectedItem.ToString()}");
            RailEditor.RailEditor RailEditor = new SerahToolkit_SharpGL.RailEditor.RailEditor(File.ReadAllBytes(_railPath), getRot);
            RailEditor.Show();
        }

        private void oBJToFF8ParserToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Parser parser = new Parser(Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]), pictureBox1.Image.Height, pictureBox1.Image.Width);
            parser.ShowDialog();
        }

        private void battleStageEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_state != StateBattleStageUv)
            {
                Console.WriteLine("Did you try to open Battle Stage editor with other file!?");
                return;
            }
            BattleStageEditor.BSEdit BSEdit = new BattleStageEditor.BSEdit(_lastKnownPath, Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]));

        }
    }
}
