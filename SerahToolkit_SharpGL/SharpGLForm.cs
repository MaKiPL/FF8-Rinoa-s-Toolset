using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;
using SerahToolkit_SharpGL.FF8_Core;
using SerahToolkit_SharpGL.Forms;
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

        internal static class NativeMethods
        {
            [DllImport("kernel32")]
            internal static extern bool AllocConsole();
        }

        private List<string> _pathModels;
        private string _railPath;
        private string _lastKnownPath;
        private int _lastKnownTim;
        private Bitmap _bmp;
        private Bitmap _bmp2;
        private int _state;

        private static Label wm2SelectedChunk;
        private static NumericUpDown wm2numeric;
        private static PictureBox pbBox;

        private GfEnviro _gf;
        private WM_Section1 wm1;
        private WM_Section2 wm2;
        private WM_Section4 wm4;
        private Form dynamicForm;

        private const int StateBattleStageUv = 0;
        private const int StateRailDraw = 1;
        private const int StateWmset = 2;
        private const int StateWmsetModel = 3;
        private const int StateGFenviro = 4;
        private const int StateWmx = 5;
        private const int StateTexture = 6; //unused
        public static string GFEnviro;
        private string _wmxPath;


        OpenGL _gl;
        readonly List<Polygon> _polygons = new List<Polygon>();

        double _rotX = 0.0f;
        double _rotY = 807.97f;
        double _rotZ = 765.81f;
        double _transX = 0.0f;
        double _transY = 0.0f;
        double _transZ = 0.0f;
        double _w = 13.5f;
        private static bool mouseLeftDown, mouseMiddleDown, mouseRightDown;
        private static int downX, downY;


        #region OpenGL
        private void openGLControl_OpenGLDraw(object sender, RenderEventArgs e)
        {
            _gl = openGLControl.OpenGL;
            _gl.Clear(OpenGL.GL_COLOR_BUFFER_BIT | OpenGL.GL_DEPTH_BUFFER_BIT);
            _gl.LoadIdentity();
            _gl.LookAt(0, 0, _w, 0, 0, 0, 0, 1, 0); //-10, 10, _w, 0, 0, 0, 0, 1, 0
            _gl.Rotate((float)_rotX, (float)_rotY, (float)_rotZ);
            _gl.Translate(_transX, _transY, _transZ);
            foreach (Polygon polygon in _polygons)
                {
                    if (!polygon.IsEnabled) continue;
                    polygon.PushObjectSpace(_gl);
                    polygon.Render(_gl, SharpGL.SceneGraph.Core.RenderMode.Render);
                    polygon.PopObjectSpace(_gl);
                }
        }

        private void openGLControl_OpenGLInitialized(object sender, EventArgs e)
        {
            OpenGL gl = openGLControl.OpenGL;
            gl.ClearColor(0, 0, 0, 0);
            openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);
            openGLControl.OpenGL.Enable(OpenGL.GL_LIGHTING);
            openGLControl.OpenGL.Enable(OpenGL.GL_LIGHT0);
            openGLControl.OpenGL.Enable(OpenGL.GL_COLOR_MATERIAL);
            openGLControl.OpenGL.Enable(OpenGL.GL_BLEND);
            openGLControl.OpenGL.BlendFunc(OpenGL.GL_SRC_ALPHA, OpenGL.GL_ONE_MINUS_SRC_ALPHA);
            NativeMethods.AllocConsole();
            Console.WriteLine("===========Welcome!==========\nPlease do not close this console, it's connected to software. It may come handy.\nThis window will also let you see the output of many features.\n\n\n");
        }
        #endregion

        #region Menu functionality
        private void UpdateStatus(string status) => toolStripStatusLabel2.Text = status;

        private void railEditorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_state != StateRailDraw)
            {
                Console.WriteLine("ERROR! Did you try to open RailEditor working on other file!?");
                return;
            }
            int getRot = int.Parse(listBox1.SelectedItem.ToString());
            Console.WriteLine($"Starting rail editor for segment {listBox1.SelectedItem.ToString()}");
            RailEditor.RailEditor RailEditor = new RailEditor.RailEditor(File.ReadAllBytes(_railPath), getRot, _railPath);
            RailEditor.Show();
        }

        private void oBJToFF8ParserToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Parser parser = new Parser(Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]), pictureBox1.Image.Height, pictureBox1.Image.Width);
            parser.ShowDialog();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutBox1 ab = new AboutBox1();
            ab.ShowDialog();
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

        private void section1ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string test = Path.GetFileNameWithoutExtension(_lastKnownPath);
            if (test.Substring(0, 5) != "wmset")
            {
                Console.WriteLine("WMSET: Please open your lingual wmset file again");
                return;
            }
            string lingual = Wmset.ReturnLingual(_lastKnownPath);
            string dir = Path.GetDirectoryName(_lastKnownPath);
            string path = $"{dir}\\wmset{lingual}.Section16";
            _state = StateWmsetModel;
            Wmset wmset = new Wmset(path);
            UpdateStatus(_lastKnownPath);
            listBox1.Items.Clear();
            foreach (int i in wmset.ProduceOffset_sec16())
                listBox1.Items.Add(i);
        }

        private void dumpRAWDataToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (_state != StateBattleStageUv)
                return;

            SaveFileDialog sfd = new SaveFileDialog();
            if (sfd.ShowDialog() != DialogResult.OK) return;
            BattleStage bs = new BattleStage(_lastKnownPath);
            int nextoffset;
            if (listBox1.SelectedIndex == listBox1.Items.Count - 1)
                nextoffset = -1;
            else
                nextoffset = int.Parse(listBox1.Items[listBox1.SelectedIndex + 1].ToString());
            bs.DumpRaw(int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString()), sfd.FileName, nextoffset);
        }

        private void verticesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            int offset = int.Parse(listBox1.Items[listBox1.SelectedIndex].ToString());
            BsVertices bsv = new BsVertices(_lastKnownPath, offset, this);
            bsv.ShowDialog();
        }
        private void openToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "wmsetxx.obj|wmset*.obj" };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            if (Path.GetFileNameWithoutExtension(ofd.FileName) == "wmset")
            {
                Console.WriteLine("WMSET: Please use your language version of wmset instead of selected file!");
                return;
            }
            _lastKnownPath = ofd.FileName;
            repackToolStripMenuItem.Enabled = true;
            openUnpackedOffsetToolStripMenuItem.Enabled = true;
            UpdateStatus(_lastKnownPath);
            _state = StateWmset;
            Wmset wmset = new Wmset(_lastKnownPath);
            listBox1.Items.Clear();
            listBox1.Items.AddRange(wmset._Debug_GetSections().Item1.ToArray());
            Render3D();
            wmsetunpack();
        }

        private void wmsetunpack()
        {
            Wmset wmset = new Wmset(_lastKnownPath);
            var offsetList = wmset._Debug_GetSections().Item2.ToArray();
            listBox1.Items.Clear(); 
            listBox1.Items.AddRange(wmset._Debug_GetSections().Item1.ToArray());

            for (int i = 0; i != Wmset.DOffsetCount; i++)
            {
                string buildPath = Path.GetDirectoryName(_lastKnownPath);
                buildPath += $"\\{Path.GetFileNameWithoutExtension(_lastKnownPath)}.Section{(i + 1).ToString()}";
                if (File.Exists(buildPath))
                    File.Delete(buildPath);
                using (FileStream fs = new FileStream(_lastKnownPath, FileMode.Open))
                {
                    using (FileStream fsW = new FileStream(buildPath, FileMode.OpenOrCreate))
                    {
                        if (i + 1 != offsetList.Length)
                        {
                            byte[] tempMem = new byte[offsetList[i + 1] - offsetList[i]];
                            fs.Seek(offsetList[i], SeekOrigin.Begin);
                            fs.Read(tempMem, 0, tempMem.Length);
                            fsW.Write(tempMem, 0, tempMem.Length);
                        }
                        else
                        {
                            FileInfo fi = new FileInfo(_lastKnownPath);
                            long eof = fi.Length;
                            byte[] tempMem = new byte[eof - offsetList[i]];
                            fs.Seek(offsetList[i], SeekOrigin.Begin);
                            fs.Read(tempMem, 0, tempMem.Length);
                            fsW.Write(tempMem, 0, tempMem.Length);
                        }
                    }
                }
            }
            UpdateStatus("Unpacked wmsetus.obj");
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

        private void rosettaStonetextDecypherToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            RosettaStone.RosettaStone rs = new RosettaStone.RosettaStone();
            rs.Show();
        }

        private void fileScannerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FileScanner.FileScannerCore fsc;
            Console.WriteLine("FileScanner engine started\nYou will find output here");
            FolderBrowserDialog fbd = new FolderBrowserDialog();
            if (fbd.ShowDialog() != DialogResult.OK) return;
            fsc = new FileScanner.FileScannerCore(fbd.SelectedPath);
            fsc.Start();
        }

        private void openToolStripMenuItem3_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "wmx.obj|wmx.obj" };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _state = StateWmx;
            _lastKnownPath = ofd.FileName;
            PopulateWmx(_lastKnownPath);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open FF8 .X stage model (a0stgXXX.x)",
                Filter = "Final Fantasy VIII stage (.x)|*.X"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            Console.WriteLine("BS: Opening file...");
            _state = StateBattleStageUv;
            exportToolStripMenuItem.Enabled = true;
            importToolStripMenuItem.Enabled = true;
            SetLines(false);
            BattleStage bs = new BattleStage(ofd.FileName);
            listBox1.Items.Clear();
            UpdateStatus(ofd.FileName);
            bs.Process(true);
            pictureBox1.Image = bs.GetTexture();
            pictureBox1.BackgroundImage = bs.GetTexture();
            _bmp = bs.GetTexture();
            _bmp2 = bs.GetTexture(); 
            _lastKnownPath = ofd.FileName;
            _lastKnownTim = bs.GetLastTim();
            _pathModels = new List<string>();
            foreach (int i in bs.GetArrayOfObjects())
            {
                listBox1.Items.Add(i.ToString());
                var pathOfd = Path.GetDirectoryName(ofd.FileName);
                pathOfd += $@"\{Path.GetFileNameWithoutExtension(ofd.FileName)}_{i.ToString()}_t.obj";
                if (File.Exists(pathOfd))
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
        }

        private void openToolStripMenuItem2_Click(object sender, EventArgs e)
        {
            SetLines(true);
            OpenFileDialog ofd = new OpenFileDialog
            {
                Title = "Open FF8 rail.obj file (rail.obj)",
                Filter = "Final Fantasy VIII train file (rail.obj)|rail.obj"
            };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _state = StateRailDraw;
            _railPath = ofd.FileName;
            UpdateStatus(_railPath);
            Rail rail = new Rail(ofd.FileName);
            listBox1.Items.Clear();
            foreach (var i in rail.GetRails())
            {
                listBox1.Items.Add(i);
            }
            listBox1.SelectedIndex = 0;
            railEditorToolStripMenuItem.Enabled = true;
        }

        private void polygonModeFacesToolStripMenuItem_Click(object sender, EventArgs e) => openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, PolygonMode.Filled);

        private void polygonModePointsToolStripMenuItem_Click(object sender, EventArgs e) => SetLines(true);

        private void convertToOBJToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ToSingleObj tso = new ToSingleObj(_lastKnownPath, listBox1.Items.Count);
            tso.PerformSingleOBJ();
        }

        private void environmentToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "GF Mag files mag*|mag*" };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _gf = new GfEnviro(ofd.FileName);
            UpdateStatus(ofd.FileName);
            _lastKnownPath = ofd.FileName;
            _state = StateGFenviro;
            listBox1.Items.Clear();
            foreach (uint a in _gf.PopulateOffsets())
            {
                if (a == 0)
                {
                    Console.WriteLine("GFWorker: Error! Probably wrong file. No offsets found.");
                    return;
                }
                listBox1.Items.Add(a);
            }
        }

        private void manualGeometryRipperexpertToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ManualGeometryRipper mgr = new ManualGeometryRipper();
            mgr.Show();
        }
        #endregion

        #region Renderer
        private void Render3D(int wmsetInt = 0)
        {
            _polygons.Clear();
            if (_state == StateBattleStageUv)
            {
                foreach (string s in _pathModels)
                {
                    Scene scene = SerializationEngine.Instance.LoadScene(s);
                    if (scene == null) continue;
                    foreach (Polygon polygon in scene.SceneContainer.Traverse<Polygon>())
                        _polygons.Add(polygon);
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
                        foreach (Polygon polygon in scene.SceneContainer.Traverse<Polygon>())
                        {
                        polygon.IsEnabled = true;
                        _polygons.Add(polygon);
                        }
                    }
            }

            if (_state == StateGFenviro && !_gf.bForceNotDraw)
            {
                _gl.ClearColor(0, 0, 0, 0);
                Scene scene = SerializationEngine.Instance.LoadScene(GFEnviro);
                if (scene != null)
                {
                    foreach (Polygon polygon in scene.SceneContainer.Traverse<Polygon>())
                    {
                        polygon.IsEnabled = true;
                        _polygons.Add(polygon);
                    }
                }
            }
            else
                _gl.ClearColor(0, 0, 0, 0);

            if (_state == StateWmx)
            {
                _gl.ClearColor(0, 0, 0, 0);
                Scene scene = SerializationEngine.Instance.LoadScene(_wmxPath);
                if (scene != null)
                {
                    foreach (Polygon polygon in scene.SceneContainer.Traverse<Polygon>())
                    {
                        polygon.IsEnabled = true;
                        _polygons.Add(polygon);
                    }
                }
            }

            if (_state != StateWmsetModel) return;
            {
                string pathOfDe = Path.GetDirectoryName(_lastKnownPath) + @"\" + Path.GetFileNameWithoutExtension(_lastKnownPath) + "_" + Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]) + "_q.obj";
                if (File.Exists(pathOfDe))
                {
                    Scene scene = SerializationEngine.Instance.LoadScene(pathOfDe);
                    if (scene != null)
                    {
                        foreach (Polygon polygon in scene.SceneContainer.Traverse<Polygon>())
                        {
                            polygon.IsEnabled = true;
                            _polygons.Add(polygon);
                        }
                    }
                }
                pathOfDe = Path.GetDirectoryName(_lastKnownPath) + @"\" + Path.GetFileNameWithoutExtension(_lastKnownPath) + "_" + Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]) + "_t.obj";
                if (!File.Exists(pathOfDe)) return;
                {
                    Scene scene = SerializationEngine.Instance.LoadScene(pathOfDe);
                    if (scene != null)
                    {
                        foreach (Polygon polygon in scene.SceneContainer.Traverse<Polygon>())
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

        private void SetLines(bool bLines)
        {
            openGLControl.OpenGL.PolygonMode(FaceMode.FrontAndBack, bLines ? PolygonMode.Points : PolygonMode.Filled);
        }
        #endregion

        #region Button functions
        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if (_state == StateBattleStageUv)
                _polygons[listBox1.SelectedIndex].IsEnabled = checkBox1.Checked != true;
        }
        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save texture";
            sfd.Filter = "Bitmap Image (.bmp)|*.bmp|Gif Image (.gif)|*.gif|JPEG Image (.jpeg)|*.jpeg|Png Image (.png)|*.png|Tiff Image (.tiff)|*.tiff";
            if (sfd.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    pictureBox1.Image.Save(sfd.FileName);
                    Console.WriteLine("Texture saved.");
                }
                catch (Exception ee)
                {
                    Console.WriteLine($"We had an error with saving texture. Exception:{Environment.NewLine}{ee.ToString()}");
                }
            }
            else
                Console.WriteLine("Cancelled texture save as...");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            _rotY = 807.97f;
            _rotZ = 765.81f;
            _transX = 0.0f;
            _transY = 0.0f;
            _transZ = 0.0f;
        }
        #endregion

        #region BattleStage Editor
        public void BSVertEditor_Update(string ofd)
        {
            BattleStage bs = new BattleStage(ofd);
            listBox1.Items.Clear();
            bs.Process(false, true);
            BS_UpdateObjects(bs.GetArrayOfObjects(), ofd);
            Render3D();
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
        }
        #endregion

        #region ListBox_Logic
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.Items.Count == 1)
                return;
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
                case StateTexture:
                    TextureLogic();
                    break;
                default:
                    return;
            }
        }

        private void WMX_list()
        {
            Wmx wmx = new Wmx(listBox1.SelectedIndex, _lastKnownPath);
            if (wmx.GetModelPath() == null) return;
            Console.WriteLine($"WMX: Giving model to renderer...");
            _wmxPath = wmx.GetModelPath();
            Render3D();
        }

        private void TextureLogic()
        {
        }

        private void WMSET_Listbox()
        {
        }

        private void BattleStage_listbox(bool bGenerateTextures)
        {
            if (listBox1.SelectedIndex == -1)
                listBox1.SelectedIndex = 0;
            Image nullImageRes = _bmp2;
            _bmp = new Bitmap(nullImageRes);
            Graphics g = Graphics.FromImage(_bmp);
            int selected = Convert.ToInt32(listBox1.SelectedItems[0]);
            BattleStage bs = new BattleStage("UV");
            Tuple<List<double>, List<double>,int> uv = bs.GetUVpoints(selected, _lastKnownPath, _lastKnownTim);
            List<Point> uvPoint = new List<Point>();
            Pen pen = new Pen(Color.White, 1.0f);
            int index = 0;
            Tuple<int, int> texResTuple = bs.GetTextureRes();
            int width = texResTuple.Item1;
            int height = texResTuple.Item2;
            checkBox1.Checked = !_polygons[listBox1.SelectedIndex].IsEnabled;
            if (bGenerateTextures)
            {
                Console.WriteLine($"BS: Mixing textures...");
                Console.WriteLine($"BS: Collecting UV data and preparing bounding boxes");
                for (int i = 0; i != listBox1.Items.Count; i++)
                {
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

                    if (!File.Exists(pathText)) continue;
                    Console.WriteLine($"BS: Mixing {pathText}");
                    Bitmap loadBmp = new Bitmap(pathText);
                    PixelFormat pf = PixelFormat.Format24bppRgb;
                    int wid = (topRight.X - topLeft.X) + 4;
                    int hei = (bottomLeft.Y - topLeft.Y) + 4;
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
                        rawBmp[pixel] = rawLoadBmp[pixel];
                    Marshal.Copy(rawBmp, 0, workingptr, rawBmp.Length);
                    loadBmp.UnlockBits(sourBitmapData);
                    _bmp.UnlockBits(targetBitmapData);
                    Console.WriteLine($"BS: Mixing finished");
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
            if (bGenerateTextures) return;
            {
                Console.WriteLine($"BS: Drawing UV layout");
                while (true)
                {
                    double x1 = (((uv.Item1[index] * 100) * width) / 100);
                    double y1 = (((uv.Item2[index] * 100) * height) / 100);
                    double x2 = (((uv.Item1[index + 1] * 100) * width) / 100);
                    double y2 = (((uv.Item2[index + 1] * 100) * height) / 100);
                    Point xy1 = new Point((int)(Math.Round(x1)), 256 - (int)(Math.Round(y1)));
                    Point xy2 = new Point((int)(Math.Round(x2)), 256 - (int)(Math.Round(y2)));
                    uvPoint.Add(xy1);
                    uvPoint.Add(xy2);
                    g.DrawLine(pen, uvPoint[index], uvPoint[index + 1]);

                    if (index >= uv.Item1.Count - 3 && index >= uv.Item2.Count - 3)
                        break;
                    index += 2;
                }
                g.Dispose();
                pictureBox1.Image = _bmp;
            }
        }

        private void Rail_listbox()
        {
            Rail rail = new Rail(_railPath);
            rail.rail(Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]));
            Render3D();
        }

        private void WMSETmod_listbox()
        {
            Wmset wmset = new Wmset(_lastKnownPath.Substring(0,_lastKnownPath.Length-3) + "Section16");
            int selectedModel = Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]);
            wmset.Sector16(selectedModel, listBox1.SelectedIndex);
            Bitmap bmp = wmset.GetTexture();
            pictureBox1.Image = bmp;
            Render3D();
        }

        private void GfLogic()
        {
            _gf.ProcessGf(Convert.ToInt32(listBox1.Items[listBox1.SelectedIndex]));
            SetLines(_gf.OnlyVertex);
            if (GFEnviro != null)
                Render3D();
        }
        #endregion

        #region WMX
        private void PopulateWmx(string ofd)
        {
            const int size = 0x9000;
            listBox1.Items.Clear();
            for (int i = 0; i != 835; i++)
                listBox1.Items.Add(i*size);
        }
        #endregion



        private void fSArchiveUnpackerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SerahToolkit_SharpGL.Text fs = new SerahToolkit_SharpGL.Text(1);
            fs.ShowDialog();
        }

        private void lZSDecompressorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text_LZSS textLzss = new Text_LZSS();
            byte[] buffer = textLzss.TryDecompress();
            if (buffer != null)
            {
                using (SaveFileDialog sfd = new SaveFileDialog())
                {
                    sfd.Filter = $"{textLzss.fileSave}.unlzs|{textLzss.fileSave}.unlzs";
                    if (sfd.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(sfd.FileName, buffer);
                        Console.WriteLine("aLZSS: Succesfully decompressed file!");
                    }
                }
            }
            else Console.WriteLine("aLZSS: This file is not LZSS compressed?");
        }

        private void wm2fieldtblToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SerahToolkit_SharpGL.Text text = new Text(2);
            text.ShowDialog();
        }

        private void namedicbinToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Text text = new SerahToolkit_SharpGL.Text(0);
            text.ShowDialog();
        }

        private void moviePlayerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string s = null;
            using (OpenFileDialog ofd = new OpenFileDialog())
            {
                ofd.Filter = "Movie pak file (.pak)|*.pak";
                if (ofd.ShowDialog() == DialogResult.OK)
                    s = ofd.FileName;
            }
            FF8_Core.PlayMovie mov = new PlayMovie(s);
            SerahToolkit_SharpGL.Text text = new Text(3);
            mov.Read();
            text.TransferMP(mov);
            text.PopulateMP(); //ehh
            text.ShowDialog();
        }

        private void normalTIMToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter= "Tim Texture (.TIM)|*.TIM"};
            TIM tim;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                tim = new TIM(ofd.FileName);
                Bitmap bmp = tim.GetBitmap;
                if (bmp != null)
                    pictureBox1.Image = bmp;
                else Console.WriteLine("TIM: TIM normal class returned null bitmap??");
            }
            ofd.Dispose();
        }

        private void packedbinaryFileforceDrawToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Tim Texture (.*)|*.*" };
            if (ofd.ShowDialog() != DialogResult.OK) return;
            FileInfo fi = new FileInfo(ofd.FileName);
            if (fi.Length >= Math.Pow(1024, 3)/4)
            {
                Console.WriteLine("File is too big!");
                ofd.Dispose();
                return;
            }
            TIM tim = new TIM(fi.FullName, 1);
            Bitmap bmp = tim.DrawRaw();
            pictureBox1.Image = bmp;
            ofd.Dispose();
        }

        private void tEXTextureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = "TEX texture (.TEX)|*.TEX"};
            if (ofd.ShowDialog() != DialogResult.OK) return;
            FF8_Core.TEX tex = new TEX(ofd.FileName);
            Bitmap bmp = tex.GetTexture();
            pictureBox1.Image = bmp;
        }



        private void worldMapRegionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string test = Path.GetFileNameWithoutExtension(_lastKnownPath);
            if (test.Substring(0, 5) != "wmset")
            {
                Console.WriteLine("WMSET: Please open your lingual wmset file again");
                return;
            }
            string lingual = Wmset.ReturnLingual(_lastKnownPath);
            string dir = Path.GetDirectoryName(_lastKnownPath);
            string path = $"{dir}\\wmset{lingual}.Section2";
            wm2 = new WM_Section2(path);
            Form wm2Editor = new Form
            {
                Size = new Size(1250, 800),
                Text = "World Map regions editor",
                MaximizeBox = false,
                MinimizeBox = false
        };
            dynamicForm = wm2Editor;
            wm2Editor.Closing += (o, args) => wm2.EndJob();
            SplitContainer wm2SplitContainer = new SplitContainer();
            wm2SplitContainer.Dock = DockStyle.Fill;
            wm2SplitContainer.SplitterDistance = 0;
            wm2SplitContainer.IsSplitterFixed = true;
            pbBox = new PictureBox
            {
                Size = new Size(800,768),
                SizeMode = PictureBoxSizeMode.AutoSize,
                Image = Properties.Resources.map2,
                Dock = DockStyle.Fill
            };
            wm2SplitContainer.Panel2.Controls.Add(pbBox);
            wm2.ReceiveBitmap(new Bitmap(pbBox.Image));
            for (int i = 0; i < 768; i++)
                wm2.ColorizeBlock(i,wm2.ReadNextRegion());
            pbBox.Image = wm2.GetColored;
            pbBox.MouseClick += WM2UpdateBlockID;
            FlowLayoutPanel flow = new FlowLayoutPanel {Dock = DockStyle.Fill};
            wm2SelectedChunk = new Label {Text = $"Selected chunk: {wm2.selectedRegion}", AutoSize = true};
            wm2numeric = new NumericUpDown {Minimum = 0, Maximum = 0xFF, Value = wm2.regions[wm2.selectedRegion]};
            wm2numeric.ValueChanged += WM2UpdateRegion;
            flow.Controls.Add(wm2SelectedChunk);
            flow.Controls.Add(wm2numeric);
            flow.SetFlowBreak(wm2SelectedChunk, true);
            flow.SetFlowBreak(wm2numeric,true);
            wm2Editor.Controls.Add(wm2SplitContainer);
            wm2SplitContainer.Panel1.Controls.Add(flow);
            Button compileButton = new Button {Text= "Compile & save"};
            compileButton.Click += CompileButton_Click;
            flow.Controls.Add(compileButton);
            wm2Editor.FormBorderStyle = FormBorderStyle.Fixed3D;
            wm2Editor.Show();
        }

        private void CompileButton_Click(object sender, EventArgs e)
        {
            byte[] b = new byte[768+4];
            Array.Copy(wm2.regions, b, 768);
            dynamicForm.Close();
            string s = wm2.path;
            File.WriteAllBytes(s, b);
            Console.WriteLine("WMSET2: Saved section 2. Remember to repack whole wmset file!");
        }

        private void WM2UpdateBlockID(object sender, MouseEventArgs e)
        {
            double rowID = e.Y/32;
            int addY = (int) (Math.Round(rowID, 0)*32);
            double colX = e.X/32;
            int addX = (int) Math.Round(colX, 0);
            wm2.selectedRegion = (ushort) (addX + addY);
            WM2UpdateGUI(wm2.selectedRegion, wm2.regions[wm2.selectedRegion]);
        }

        /*
         *             int widthblock = blockID*32;
            int row = (int) Math.Round((double) (widthblock/1024), 1);
            int realwidth = row != 0 ? widthblock-1024*row : widthblock;*/

        public static void WM2UpdateGUI(ushort chunkID, byte value)
        {
            wm2SelectedChunk.Text = $"Selected chunk: {chunkID}";
            wm2numeric.Value = value;
        }

        private void WM2UpdateRegion(object sender, EventArgs e)
        {
            wm2.regions[wm2.selectedRegion] = (byte) (sender as NumericUpDown).Value;
            wm2.ResetBlockColor(wm2.selectedRegion);
            pbBox.Image = wm2.GetColored;
        }

        private void repackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DirectoryInfo di = null;
            if (_lastKnownPath.Contains("wmset") && !_lastKnownPath.Contains("wmset.obj"))
                di =
                    new DirectoryInfo(_lastKnownPath.Substring(0,
                        _lastKnownPath.Length - Path.GetFileName(_lastKnownPath).Length));
            else
            {
                MessageBox.Show(
                    "You initiated WMSET repack! Please direct me to folder\nwhere you keep unpacked wmset section files");
                FolderBrowserDialog fbd = new FolderBrowserDialog {Description = "WMSET unpacked folder"};
                if (fbd.ShowDialog() != DialogResult.OK) return;
                di = new DirectoryInfo(fbd.SelectedPath);
            }
            FileInfo[] fi = di.GetFiles("wmset*.section*");
            /*bool us, it, fr, es, de;
            foreach (FileInfo n in fi)
            {
                us = Path.GetFileName(n.FullName) == "wmsetus.Section1";
                it = Path.GetFileName(n.FullName) == "wmsetit.Section1";
                fr = Path.GetFileName(n.FullName) == "wmsetfr.Section1";
                es = Path.GetFileName(n.FullName) == "wmsetes.Section1";
                de = Path.GetFileName(n.FullName) == "wmsetde.Section1";
            }*/
            //Regex regex = new Regex("wmsetus.Section*");

            //Just practicing LINQ
            FileInfo[] us = fi.Where(a => a.FullName.Contains("wmsetus.Section")).ToArray();
            FileInfo[] it = fi.Where(a => a.FullName.Contains("wmsetit.Section")).ToArray();
            FileInfo[] fr = fi.Where(a => a.FullName.Contains("wmsetfr.Section")).ToArray();
            FileInfo[] es = (from n in fi where n.FullName.Contains("wmsetes.Section") select n).ToArray(); //testing
            FileInfo[] de = fi.Where(a => a.FullName.Contains("wmsetde.Section")).ToArray();
            if (us.Length == 48)
            {
                Console.WriteLine("WMSET: Found all US sections. Building file");
                CompileWMSETfile(us, "us");
            }
            else Console.WriteLine("WMSET: Not enough WMSETUS sections! Can't build file...");
            if (it.Length == 48)
            {
                Console.WriteLine("WMSET: Found all IT sections. Building file");
                CompileWMSETfile(it, "it");
            }
            else Console.WriteLine("WMSET: Not enough WMSETIT sections! Can't build file...");
            if (fr.Length == 48)
            {
                Console.WriteLine("WMSET: Found all FR sections. Building file");
                CompileWMSETfile(fr, "fr");
            }
            else Console.WriteLine("WMSET: Not enough WMSETFR sections! Can't build file...");
            if (es.Length == 48)
            {
                Console.WriteLine("WMSET: Found all ES sections. Building file");
                CompileWMSETfile(es, "es");
            }
            else Console.WriteLine("WMSET: Not enough WMSETES sections! Can't build file...");
            if (de.Length == 48)
            {
                Console.WriteLine("WMSET: Found all DE sections. Building file");
                CompileWMSETfile(de, "de");
            }
            else Console.WriteLine("WMSET: Not enough WMSETDE sections! Can't build file...");

        }

        private void CompileWMSETfile(FileInfo[] sections, string countyLiteral)
        {
            byte[] buffer = new byte [sections.Sum(fi => (int) fi.Length) + 49*4];
            uint[] sizes = new uint[48];
            for (int i = 1; i <= 48; i++)
            {
                int i1 = i;
                FileInfo[] f =
                    sections.Where(a => Path.GetFileName(a.FullName) == $"wmset{countyLiteral}.Section{i1}").ToArray();
                sizes[i - 1] = (uint) f[0].Length;
                uint globalLocation = 49*4;
                for (int k = 0; k != i-1; k++)
                    globalLocation += sizes[k];
                byte[] temp = BitConverter.GetBytes(globalLocation);
                Array.Copy(temp, 0, buffer,(i-1)*4, 4);
                temp = File.ReadAllBytes(f[0].FullName);
                Array.Copy(temp, 0, buffer, globalLocation, temp.Length);
            }
            SaveFileDialog sfd = new SaveFileDialog
            {
                Filter = $"wmset{countyLiteral}.obj|wmset{countyLiteral}.obj",
                Title = $"Save compiled wmset{countyLiteral} file!"
            };
            if (sfd.ShowDialog() == DialogResult.OK) File.WriteAllBytes(sfd.FileName, buffer);
            else return;
            Console.WriteLine($"WMSET: Succesfully saved {sfd.FileName} file");
        }

        private void worldMapEncountersToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string test = Path.GetFileNameWithoutExtension(_lastKnownPath);
            if (test.Substring(0, 5) != "wmset")
            {
                Console.WriteLine("WMSET: Please open your lingual wmset file again");
                return;
            }
            string lingual = Wmset.ReturnLingual(_lastKnownPath);
            string dir = Path.GetDirectoryName(_lastKnownPath);
            string path = $"{dir}\\wmset{lingual}.Section4";
            string sec1 = $"{dir}\\wmset{lingual}.Section1";
            wm4 = new WM_Section4(path);
            wm1 = new WM_Section1(sec1);
            Forms.wm4 wm4editor = new wm4(wm4,wm1);
            wm4.ProduceData();
            wm4editor.InitialUpdate(wm4.GetEncounters);
            wm4editor.Show();
        }

        private void opencharaoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = "chara.one|chara.one"};
            if (ofd.ShowDialog() != DialogResult.OK) return;
            _lastKnownPath = ofd.FileName;
            charaone.worldMode charaone = new charaone.worldMode(ofd.FileName);
        }

        private void mCHCharacterToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog {Filter = "*.mch|*.mch"};
            if(ofd.ShowDialog() != DialogResult.OK) return;
            FF8_Core.mch mch = new mch(ofd.FileName);
        }

        private void openGLControl_MouseDown(object sender, MouseEventArgs e)
        {
            mouseLeftDown = e.Button == MouseButtons.Left;
            mouseMiddleDown = e.Button == MouseButtons.Middle;
            mouseRightDown = e.Button == MouseButtons.Right;


            if (!mouseLeftDown && !mouseMiddleDown && !mouseRightDown) return;
            downX = MousePosition.X;
            downY = MousePosition.Y;
        }

        private void openGLControl_MouseUp(object sender, MouseEventArgs e)
        {
            mouseLeftDown = e.Button != MouseButtons.Left;
            mouseMiddleDown = e.Button != MouseButtons.Middle;
            mouseRightDown = e.Button != MouseButtons.Right;
        }

        private void openGLControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.None) //If no button is pressed, don't proceed
                return;

            if (MousePosition.X == downX && MousePosition.Y == downY) return;

            if (mouseLeftDown)
            {
                _rotY += (downX - MousePosition.X) * 0.010f;
                _rotZ += (downY - MousePosition.Y) * 0.010f;

                if (downX != MousePosition.X || downY != MousePosition.Y)
                    Cursor.Position = new Point(downX, downY);
            }

            if (mouseMiddleDown)
            {
                _transX += (MousePosition.X - downX) * 0.003f;
                _transY -= (MousePosition.Y - downY) * 0.003f;

                if (downX != MousePosition.X || downY != MousePosition.Y)
                    Cursor.Position = new Point(downX, downY);
            }
            if (mouseRightDown)
            {
                _w += (downX - MousePosition.X) * 0.010f;
                //_transY += (downX - MousePosition.X) * 0.010f;

                if (downX != MousePosition.X || downY != MousePosition.Y)
                    Cursor.Position = new Point(downX, downY);
            }


            downX = MousePosition.X;
            downY = MousePosition.Y;
        }
    }
}
