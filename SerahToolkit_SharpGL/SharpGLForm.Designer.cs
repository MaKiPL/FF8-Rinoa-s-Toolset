namespace SerahToolkit_SharpGL
{
    partial class SharpGLForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openGLControl = new SharpGL.OpenGLControl();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.battleStageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.dumpRAWDataToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.convertToOBJToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.importToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.verticesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oBJToFF8ParserToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.oBJToFF8ParserToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.parseVerticesForSegmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.worldMapToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem3 = new System.Windows.Forms.ToolStripMenuItem();
            this.worldMapObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.openUnpackedOffsetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.section1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sectionXToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.unpackToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.railobjToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.gFToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.environmentToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sceneoutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.advancedAnalyzeToolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cFF8SearcherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rosettaStonetextDecypherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.displayToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wireframeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showFPSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.texturedToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.texturedWLightToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.polygonModeFacesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.polygonModePointsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.trackBar2 = new System.Windows.Forms.TrackBar();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.trackBar3 = new System.Windows.Forms.TrackBar();
            this.trackBar4 = new System.Windows.Forms.TrackBar();
            this.trackBar5 = new System.Windows.Forms.TrackBar();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // openGLControl
            // 
            this.openGLControl.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.openGLControl.DrawFPS = false;
            this.openGLControl.Location = new System.Drawing.Point(199, 24);
            this.openGLControl.Name = "openGLControl";
            this.openGLControl.OpenGLVersion = SharpGL.Version.OpenGLVersion.OpenGL2_1;
            this.openGLControl.RenderContextType = SharpGL.RenderContextType.FBO;
            this.openGLControl.RenderTrigger = SharpGL.RenderTrigger.TimerBased;
            this.openGLControl.Size = new System.Drawing.Size(809, 504);
            this.openGLControl.TabIndex = 0;
            this.openGLControl.OpenGLInitialized += new System.EventHandler(this.openGLControl_OpenGLInitialized);
            this.openGLControl.OpenGLDraw += new SharpGL.RenderEventHandler(this.openGLControl_OpenGLDraw);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.battleStageToolStripMenuItem,
            this.worldMapToolStripMenuItem,
            this.worldMapObjectToolStripMenuItem,
            this.railobjToolStripMenuItem,
            this.gFToolStripMenuItem,
            this.sceneoutToolStripMenuItem,
            this.aboutToolStripMenuItem,
            this.displayToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1059, 24);
            this.menuStrip1.TabIndex = 1;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // battleStageToolStripMenuItem
            // 
            this.battleStageToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem,
            this.exportToolStripMenuItem,
            this.importToolStripMenuItem});
            this.battleStageToolStripMenuItem.Name = "battleStageToolStripMenuItem";
            this.battleStageToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.battleStageToolStripMenuItem.Text = "Battle Stage";
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.openToolStripMenuItem.Text = "Open";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // exportToolStripMenuItem
            // 
            this.exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.dumpRAWDataToolStripMenuItem,
            this.convertToOBJToolStripMenuItem});
            this.exportToolStripMenuItem.Enabled = false;
            this.exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            this.exportToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.exportToolStripMenuItem.Text = "Export...";
            // 
            // dumpRAWDataToolStripMenuItem
            // 
            this.dumpRAWDataToolStripMenuItem.Name = "dumpRAWDataToolStripMenuItem";
            this.dumpRAWDataToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.dumpRAWDataToolStripMenuItem.Text = "Dump RAW data";
            this.dumpRAWDataToolStripMenuItem.Click += new System.EventHandler(this.dumpRAWDataToolStripMenuItem_Click);
            // 
            // convertToOBJToolStripMenuItem
            // 
            this.convertToOBJToolStripMenuItem.Name = "convertToOBJToolStripMenuItem";
            this.convertToOBJToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.convertToOBJToolStripMenuItem.Text = "Convert to single OBJ";
            this.convertToOBJToolStripMenuItem.Click += new System.EventHandler(this.convertToOBJToolStripMenuItem_Click);
            // 
            // importToolStripMenuItem
            // 
            this.importToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.verticesToolStripMenuItem,
            this.oBJToFF8ParserToolStripMenuItem1});
            this.importToolStripMenuItem.Enabled = false;
            this.importToolStripMenuItem.Name = "importToolStripMenuItem";
            this.importToolStripMenuItem.Size = new System.Drawing.Size(121, 22);
            this.importToolStripMenuItem.Text = "Modify...";
            // 
            // verticesToolStripMenuItem
            // 
            this.verticesToolStripMenuItem.Name = "verticesToolStripMenuItem";
            this.verticesToolStripMenuItem.Size = new System.Drawing.Size(164, 22);
            this.verticesToolStripMenuItem.Text = "Single vertex edit";
            this.verticesToolStripMenuItem.Click += new System.EventHandler(this.verticesToolStripMenuItem_Click);
            // 
            // oBJToFF8ParserToolStripMenuItem1
            // 
            this.oBJToFF8ParserToolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.oBJToFF8ParserToolStripMenuItem,
            this.parseVerticesForSegmentToolStripMenuItem});
            this.oBJToFF8ParserToolStripMenuItem1.Name = "oBJToFF8ParserToolStripMenuItem1";
            this.oBJToFF8ParserToolStripMenuItem1.Size = new System.Drawing.Size(164, 22);
            this.oBJToFF8ParserToolStripMenuItem1.Text = "OBJ to FF8 parser";
            // 
            // oBJToFF8ParserToolStripMenuItem
            // 
            this.oBJToFF8ParserToolStripMenuItem.Name = "oBJToFF8ParserToolStripMenuItem";
            this.oBJToFF8ParserToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.oBJToFF8ParserToolStripMenuItem.Text = "Complete segment replacer (Beta)";
            this.oBJToFF8ParserToolStripMenuItem.Click += new System.EventHandler(this.oBJToFF8ParserToolStripMenuItem_Click_1);
            // 
            // parseVerticesForSegmentToolStripMenuItem
            // 
            this.parseVerticesForSegmentToolStripMenuItem.Name = "parseVerticesForSegmentToolStripMenuItem";
            this.parseVerticesForSegmentToolStripMenuItem.Size = new System.Drawing.Size(254, 22);
            this.parseVerticesForSegmentToolStripMenuItem.Text = "Parse vertices for segment";
            this.parseVerticesForSegmentToolStripMenuItem.Click += new System.EventHandler(this.parseVerticesForSegmentToolStripMenuItem_Click);
            // 
            // worldMapToolStripMenuItem
            // 
            this.worldMapToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem3});
            this.worldMapToolStripMenuItem.Name = "worldMapToolStripMenuItem";
            this.worldMapToolStripMenuItem.Size = new System.Drawing.Size(78, 20);
            this.worldMapToolStripMenuItem.Text = "World map";
            // 
            // openToolStripMenuItem3
            // 
            this.openToolStripMenuItem3.Name = "openToolStripMenuItem3";
            this.openToolStripMenuItem3.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem3.Text = "Open";
            this.openToolStripMenuItem3.Click += new System.EventHandler(this.openToolStripMenuItem3_Click);
            // 
            // worldMapObjectToolStripMenuItem
            // 
            this.worldMapObjectToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem1,
            this.openUnpackedOffsetToolStripMenuItem,
            this.unpackToolStripMenuItem});
            this.worldMapObjectToolStripMenuItem.Name = "worldMapObjectToolStripMenuItem";
            this.worldMapObjectToolStripMenuItem.Size = new System.Drawing.Size(124, 20);
            this.worldMapObjectToolStripMenuItem.Text = "wmsetus/it/fr/gr/sp";
            // 
            // openToolStripMenuItem1
            // 
            this.openToolStripMenuItem1.Name = "openToolStripMenuItem1";
            this.openToolStripMenuItem1.Size = new System.Drawing.Size(191, 22);
            this.openToolStripMenuItem1.Text = "Open";
            this.openToolStripMenuItem1.Click += new System.EventHandler(this.openToolStripMenuItem1_Click);
            // 
            // openUnpackedOffsetToolStripMenuItem
            // 
            this.openUnpackedOffsetToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.section1ToolStripMenuItem,
            this.sectionXToolStripMenuItem});
            this.openUnpackedOffsetToolStripMenuItem.Name = "openUnpackedOffsetToolStripMenuItem";
            this.openUnpackedOffsetToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.openUnpackedOffsetToolStripMenuItem.Text = "Open unpacked offset";
            // 
            // section1ToolStripMenuItem
            // 
            this.section1ToolStripMenuItem.Name = "section1ToolStripMenuItem";
            this.section1ToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.section1ToolStripMenuItem.Text = "Models (Section 16)";
            this.section1ToolStripMenuItem.Click += new System.EventHandler(this.section1ToolStripMenuItem_Click);
            // 
            // sectionXToolStripMenuItem
            // 
            this.sectionXToolStripMenuItem.Enabled = false;
            this.sectionXToolStripMenuItem.Name = "sectionXToolStripMenuItem";
            this.sectionXToolStripMenuItem.Size = new System.Drawing.Size(178, 22);
            this.sectionXToolStripMenuItem.Text = "UNPACK FIRST!";
            // 
            // unpackToolStripMenuItem
            // 
            this.unpackToolStripMenuItem.Enabled = false;
            this.unpackToolStripMenuItem.Name = "unpackToolStripMenuItem";
            this.unpackToolStripMenuItem.Size = new System.Drawing.Size(191, 22);
            this.unpackToolStripMenuItem.Text = "Unpack";
            this.unpackToolStripMenuItem.Click += new System.EventHandler(this.unpackToolStripMenuItem_Click);
            // 
            // railobjToolStripMenuItem
            // 
            this.railobjToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.openToolStripMenuItem2});
            this.railobjToolStripMenuItem.Name = "railobjToolStripMenuItem";
            this.railobjToolStripMenuItem.Size = new System.Drawing.Size(55, 20);
            this.railobjToolStripMenuItem.Text = "rail.obj";
            // 
            // openToolStripMenuItem2
            // 
            this.openToolStripMenuItem2.Name = "openToolStripMenuItem2";
            this.openToolStripMenuItem2.Size = new System.Drawing.Size(103, 22);
            this.openToolStripMenuItem2.Text = "Open";
            this.openToolStripMenuItem2.Click += new System.EventHandler(this.openToolStripMenuItem2_Click);
            // 
            // gFToolStripMenuItem
            // 
            this.gFToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.environmentToolStripMenuItem});
            this.gFToolStripMenuItem.Name = "gFToolStripMenuItem";
            this.gFToolStripMenuItem.Size = new System.Drawing.Size(33, 20);
            this.gFToolStripMenuItem.Text = "GF";
            // 
            // environmentToolStripMenuItem
            // 
            this.environmentToolStripMenuItem.Name = "environmentToolStripMenuItem";
            this.environmentToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.environmentToolStripMenuItem.Text = "Environment";
            this.environmentToolStripMenuItem.Click += new System.EventHandler(this.environmentToolStripMenuItem_Click);
            // 
            // sceneoutToolStripMenuItem
            // 
            this.sceneoutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.advancedAnalyzeToolsToolStripMenuItem,
            this.rosettaStonetextDecypherToolStripMenuItem});
            this.sceneoutToolStripMenuItem.Name = "sceneoutToolStripMenuItem";
            this.sceneoutToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.sceneoutToolStripMenuItem.Text = "Other Tools";
            // 
            // advancedAnalyzeToolsToolStripMenuItem
            // 
            this.advancedAnalyzeToolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.cFF8SearcherToolStripMenuItem});
            this.advancedAnalyzeToolsToolStripMenuItem.Name = "advancedAnalyzeToolsToolStripMenuItem";
            this.advancedAnalyzeToolsToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.advancedAnalyzeToolsToolStripMenuItem.Text = "Advanced analyze tools";
            // 
            // cFF8SearcherToolStripMenuItem
            // 
            this.cFF8SearcherToolStripMenuItem.Name = "cFF8SearcherToolStripMenuItem";
            this.cFF8SearcherToolStripMenuItem.Size = new System.Drawing.Size(160, 22);
            this.cFF8SearcherToolStripMenuItem.Text = "C:\\\\FF8 searcher";
            this.cFF8SearcherToolStripMenuItem.Click += new System.EventHandler(this.cFF8SearcherToolStripMenuItem_Click);
            // 
            // rosettaStonetextDecypherToolStripMenuItem
            // 
            this.rosettaStonetextDecypherToolStripMenuItem.Name = "rosettaStonetextDecypherToolStripMenuItem";
            this.rosettaStonetextDecypherToolStripMenuItem.Size = new System.Drawing.Size(227, 22);
            this.rosettaStonetextDecypherToolStripMenuItem.Text = "Rosetta stone (text decypher)";
            this.rosettaStonetextDecypherToolStripMenuItem.Click += new System.EventHandler(this.rosettaStonetextDecypherToolStripMenuItem_Click_1);
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(52, 20);
            this.aboutToolStripMenuItem.Text = "About";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // displayToolStripMenuItem
            // 
            this.displayToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wireframeToolStripMenuItem,
            this.showFPSToolStripMenuItem,
            this.texturedToolStripMenuItem,
            this.texturedWLightToolStripMenuItem,
            this.polygonModeFacesToolStripMenuItem,
            this.polygonModePointsToolStripMenuItem});
            this.displayToolStripMenuItem.Name = "displayToolStripMenuItem";
            this.displayToolStripMenuItem.Size = new System.Drawing.Size(57, 20);
            this.displayToolStripMenuItem.Text = "Display";
            // 
            // wireframeToolStripMenuItem
            // 
            this.wireframeToolStripMenuItem.Name = "wireframeToolStripMenuItem";
            this.wireframeToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.wireframeToolStripMenuItem.Text = "Wireframe";
            this.wireframeToolStripMenuItem.Click += new System.EventHandler(this.wireframeToolStripMenuItem_Click);
            // 
            // showFPSToolStripMenuItem
            // 
            this.showFPSToolStripMenuItem.Name = "showFPSToolStripMenuItem";
            this.showFPSToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.showFPSToolStripMenuItem.Text = "Show FPS";
            this.showFPSToolStripMenuItem.Click += new System.EventHandler(this.showFPSToolStripMenuItem_Click);
            // 
            // texturedToolStripMenuItem
            // 
            this.texturedToolStripMenuItem.Checked = true;
            this.texturedToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.texturedToolStripMenuItem.Name = "texturedToolStripMenuItem";
            this.texturedToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.texturedToolStripMenuItem.Text = "Textured w/ Light";
            this.texturedToolStripMenuItem.Click += new System.EventHandler(this.texturedToolStripMenuItem_Click);
            // 
            // texturedWLightToolStripMenuItem
            // 
            this.texturedWLightToolStripMenuItem.Name = "texturedWLightToolStripMenuItem";
            this.texturedWLightToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.texturedWLightToolStripMenuItem.Text = "Textured";
            this.texturedWLightToolStripMenuItem.Click += new System.EventHandler(this.texturedWLightToolStripMenuItem_Click);
            // 
            // polygonModeFacesToolStripMenuItem
            // 
            this.polygonModeFacesToolStripMenuItem.Name = "polygonModeFacesToolStripMenuItem";
            this.polygonModeFacesToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.polygonModeFacesToolStripMenuItem.Text = "Polygon mode: Faces";
            this.polygonModeFacesToolStripMenuItem.Click += new System.EventHandler(this.polygonModeFacesToolStripMenuItem_Click);
            // 
            // polygonModePointsToolStripMenuItem
            // 
            this.polygonModePointsToolStripMenuItem.Name = "polygonModePointsToolStripMenuItem";
            this.polygonModePointsToolStripMenuItem.Size = new System.Drawing.Size(187, 22);
            this.polygonModePointsToolStripMenuItem.Text = "Polygon mode: Lines";
            this.polygonModePointsToolStripMenuItem.Click += new System.EventHandler(this.polygonModePointsToolStripMenuItem_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(199, 534);
            this.trackBar1.Maximum = 1500;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(835, 45);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Value = 750;
            // 
            // trackBar2
            // 
            this.trackBar2.Location = new System.Drawing.Point(1014, 24);
            this.trackBar2.Maximum = 1500;
            this.trackBar2.Name = "trackBar2";
            this.trackBar2.Orientation = System.Windows.Forms.Orientation.Vertical;
            this.trackBar2.Size = new System.Drawing.Size(45, 504);
            this.trackBar2.TabIndex = 3;
            this.trackBar2.Value = 723;
            // 
            // listBox1
            // 
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Items.AddRange(new object[] {
            "No items found."});
            this.listBox1.Location = new System.Drawing.Point(0, 0);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(193, 476);
            this.listBox1.TabIndex = 4;
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 503);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(178, 91);
            this.label1.TabIndex = 6;
            this.label1.Text = "NOTE!\r\n\r\nThis software automatically converts\r\nparts of model to .OBJ, you can fi" +
    "nd\r\nthem along with ripped .PNG texture\r\nand .MTL file in directory where the\r\ns" +
    "ource file is.";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(933, 24);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(75, 23);
            this.button1.TabIndex = 7;
            this.button1.Text = "reset cam";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // trackBar3
            // 
            this.trackBar3.Location = new System.Drawing.Point(40, 24);
            this.trackBar3.Maximum = 2000;
            this.trackBar3.Minimum = -2000;
            this.trackBar3.Name = "trackBar3";
            this.trackBar3.Size = new System.Drawing.Size(157, 45);
            this.trackBar3.TabIndex = 9;
            // 
            // trackBar4
            // 
            this.trackBar4.Location = new System.Drawing.Point(40, 86);
            this.trackBar4.Maximum = 2000;
            this.trackBar4.Minimum = -2000;
            this.trackBar4.Name = "trackBar4";
            this.trackBar4.Size = new System.Drawing.Size(157, 45);
            this.trackBar4.TabIndex = 10;
            // 
            // trackBar5
            // 
            this.trackBar5.Location = new System.Drawing.Point(40, 56);
            this.trackBar5.Maximum = 2000;
            this.trackBar5.Minimum = -2000;
            this.trackBar5.Name = "trackBar5";
            this.trackBar5.Size = new System.Drawing.Size(157, 45);
            this.trackBar5.TabIndex = 11;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(186, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(14, 65);
            this.label2.TabIndex = 12;
            this.label2.Text = "X\r\n\r\nY\r\n\r\nZ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(39, 8);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(62, 13);
            this.label3.TabIndex = 13;
            this.label3.Text = "Translation:";
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(88, 164);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(60, 17);
            this.checkBox1.TabIndex = 14;
            this.checkBox1.Text = "Hidden";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(88, 134);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(109, 13);
            this.label4.TabIndex = 15;
            this.label4.Text = "Battle stage segment:";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 858);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(1059, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 19;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(49, 17);
            this.toolStripStatusLabel2.Text = "NO FILE";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.listBox1);
            this.panel1.Location = new System.Drawing.Point(0, 24);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(193, 476);
            this.panel1.TabIndex = 20;
            // 
            // panel2
            // 
            this.panel2.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.panel2.Controls.Add(this.label2);
            this.panel2.Controls.Add(this.trackBar4);
            this.panel2.Controls.Add(this.trackBar5);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.trackBar3);
            this.panel2.Controls.Add(this.label4);
            this.panel2.Controls.Add(this.checkBox1);
            this.panel2.Location = new System.Drawing.Point(798, 597);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(236, 261);
            this.panel2.TabIndex = 21;
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = global::SerahToolkit_SharpGL.Properties.Resources.tex;
            this.pictureBox1.Location = new System.Drawing.Point(0, 597);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(768, 256);
            this.pictureBox1.SizeMode = System.Windows.Forms.PictureBoxSizeMode.AutoSize;
            this.pictureBox1.TabIndex = 5;
            this.pictureBox1.TabStop = false;
            // 
            // SharpGLForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(1046, 878);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.pictureBox1);
            this.Controls.Add(this.trackBar2);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.openGLControl);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "SharpGLForm";
            this.Text = "FF8 - SerahToolkit by MaKiPL v.0.9.00";
            ((System.ComponentModel.ISupportInitialize)(this.openGLControl)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar5)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private SharpGL.OpenGLControl openGLControl;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem battleStageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dumpRAWDataToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem convertToOBJToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem importToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem verticesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem worldMapObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem displayToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem wireframeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showFPSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem texturedToolStripMenuItem;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.TrackBar trackBar2;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.ToolStripMenuItem texturedWLightToolStripMenuItem;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ToolStripMenuItem railobjToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem worldMapToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem polygonModeFacesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem polygonModePointsToolStripMenuItem;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TrackBar trackBar3;
        private System.Windows.Forms.TrackBar trackBar4;
        private System.Windows.Forms.TrackBar trackBar5;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripMenuItem sceneoutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem unpackToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openUnpackedOffsetToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem sectionXToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem section1ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem advancedAnalyzeToolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cFF8SearcherToolStripMenuItem;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.ToolStripMenuItem gFToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem environmentToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem rosettaStonetextDecypherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem oBJToFF8ParserToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem oBJToFF8ParserToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem parseVerticesForSegmentToolStripMenuItem;
    }
}

