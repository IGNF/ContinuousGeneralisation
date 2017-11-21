namespace ContinuousGeneralizer
{
    partial class FrmContinuousGeneralizer
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
            //Ensures that any ESRI libraries that have been used are unloaded in the correct order. 
            //Failure to do this may result in random crashes on exit due to the operating system unloading 
            //the libraries in the incorrect order. 
            ESRI.ArcGIS.ADF.COMSupport.AOUninitialize.Shutdown();

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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmContinuousGeneralizer));
            this.mnuORB = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSimAngle3 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSimDeflection = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTF = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLandingTimeLI = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLandingTime = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBezierDetectPointfarthest = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBezierDetectPoint = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCreatePointLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLookingForNeighboursDT = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTemporary = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExcelToShape = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuEnlargeLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMatchAndMergePolylines = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLookingForNeighboursSweepLine = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLookingForNeighboursDT2 = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLookingForNeighboursGrids = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCreateRandomPointLayer = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCoordinatesTransformation = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGaussianPerturbation = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDeletePointsWithSameCoordinates = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUnifyDirections = new System.Windows.Forms.ToolStripMenuItem();
            this.toIpeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCalGeo_Ipe = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTopologyChecker = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStatisticsOfDataSets = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTestDistanceMetric = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMatchAndMergePolygons = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMergeAndSplitPolylines = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuUnifyIndicesPolylines = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuIdentifyCorrCpgAddRegionNum = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCompareExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSelectRandomly = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOrdinal = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSimultaneity = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuApLALMulti = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLinear_AL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuPAL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuGeneralization = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuDPSimplify = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAreaAgg_AStar = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBldgGrow = new System.Windows.Forms.ToolStripMenuItem();
            this.morphingExtendToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCDTLSA = new System.Windows.Forms.ToolStripMenuItem();
            this.fishEyeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tbcRight = new System.Windows.Forms.TabControl();
            this.tabPageMap = new System.Windows.Forms.TabPage();
            this.axLicenseControl1 = new ESRI.ArcGIS.Controls.AxLicenseControl();
            this.axMapControl = new ESRI.ArcGIS.Controls.AxMapControl();
            this.splitContainer2 = new System.Windows.Forms.SplitContainer();
            this.txtT = new System.Windows.Forms.TextBox();
            this.txtVtPV = new System.Windows.Forms.TextBox();
            this.tabPageLayout = new System.Windows.Forms.TabPage();
            this.axPageLayoutControl = new ESRI.ArcGIS.Controls.AxPageLayoutControl();
            this.propertyGrid = new System.Windows.Forms.PropertyGrid();
            this.tabPageProperty = new System.Windows.Forms.TabPage();
            this.axTOCControl = new ESRI.ArcGIS.Controls.AxTOCControl();
            this.axMapControlEagleEye = new ESRI.ArcGIS.Controls.AxMapControl();
            this.tabPage1Layer = new System.Windows.Forms.TabPage();
            this.tbcLeft = new System.Windows.Forms.TabControl();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.splitContainer = new System.Windows.Forms.SplitContainer();
            this.axToolbarControl = new ESRI.ArcGIS.Controls.AxToolbarControl();
            this.mnuAnyTest = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuALALMulti = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuALAMulti = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCALMulti = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOpen = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuNew = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuFile = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.mnuSave = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuSaveAs = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuAddData = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExportViewToImage = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.mnuExit = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.morphingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLinear = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLinearMulti = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuVertexInsertion = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMPBDP = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMPBDPBL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMRL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMRLCut = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptCor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuOptCorBezier = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMPBBSL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuMPBBSLDP = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBLGOptCorMMSimplified = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBSBLGOptCorMMSimplified = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBSBLGOptCor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuBSBLGOptCorMM = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRIBS = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRIBSBLG = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRIBSBLGOptCor = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAtBdMorphing = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuCGABM = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTest = new System.Windows.Forms.ToolStripMenuItem();
            this.MorphingLSAToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuStraightLine = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAL_AL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuAL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuALL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuApLL = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuALm = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLMulti = new System.Windows.Forms.ToolStripMenuItem();
            this.roadnetworkToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClassification = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuRightAngelDPS = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuLinearMorphing = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuTransparencyMorphing = new System.Windows.Forms.ToolStripMenuItem();
            this.otherToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuKillExcel = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuClear = new System.Windows.Forms.ToolStripMenuItem();
            this.kARSToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuExtractPossibleFiles = new System.Windows.Forms.ToolStripMenuItem();
            this.mnuHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.CoordinateLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.Blank = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.ststMain = new System.Windows.Forms.StatusStrip();
            this.tsslTime = new System.Windows.Forms.ToolStripStatusLabel();
            this.tspbMain = new System.Windows.Forms.ToolStripProgressBar();
            this.tbcRight.SuspendLayout();
            this.tabPageMap.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
            this.splitContainer2.Panel1.SuspendLayout();
            this.splitContainer2.Panel2.SuspendLayout();
            this.splitContainer2.SuspendLayout();
            this.tabPageLayout.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axPageLayoutControl)).BeginInit();
            this.tabPageProperty.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControlEagleEye)).BeginInit();
            this.tabPage1Layer.SuspendLayout();
            this.tbcLeft.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).BeginInit();
            this.splitContainer.Panel1.SuspendLayout();
            this.splitContainer.Panel2.SuspendLayout();
            this.splitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl)).BeginInit();
            this.menuStrip.SuspendLayout();
            this.ststMain.SuspendLayout();
            this.SuspendLayout();
            // 
            // mnuORB
            // 
            this.mnuORB.Name = "mnuORB";
            this.mnuORB.Size = new System.Drawing.Size(148, 22);
            this.mnuORB.Text = "ORB";
            this.mnuORB.Click += new System.EventHandler(this.mnuORB_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuORB,
            this.mnuSimAngle3,
            this.mnuSimDeflection,
            this.mnuTF});
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(68, 20);
            this.toolStripMenuItem1.Text = "Similarity";
            // 
            // mnuSimAngle3
            // 
            this.mnuSimAngle3.Name = "mnuSimAngle3";
            this.mnuSimAngle3.Size = new System.Drawing.Size(148, 22);
            this.mnuSimAngle3.Text = "SimAngle3";
            this.mnuSimAngle3.Click += new System.EventHandler(this.mnuSimAngle3_Click);
            // 
            // mnuSimDeflection
            // 
            this.mnuSimDeflection.Name = "mnuSimDeflection";
            this.mnuSimDeflection.Size = new System.Drawing.Size(148, 22);
            this.mnuSimDeflection.Text = "SimDeflection";
            this.mnuSimDeflection.Click += new System.EventHandler(this.mnuSimDeflection_Click);
            // 
            // mnuTF
            // 
            this.mnuTF.Name = "mnuTF";
            this.mnuTF.Size = new System.Drawing.Size(148, 22);
            this.mnuTF.Text = "TF";
            this.mnuTF.Click += new System.EventHandler(this.mnuTF_Click);
            // 
            // mnuLandingTimeLI
            // 
            this.mnuLandingTimeLI.Name = "mnuLandingTimeLI";
            this.mnuLandingTimeLI.Size = new System.Drawing.Size(153, 22);
            this.mnuLandingTimeLI.Text = "LandingTimeLI";
            this.mnuLandingTimeLI.Click += new System.EventHandler(this.mnuLandingTimeLI_Click);
            // 
            // mnuLandingTime
            // 
            this.mnuLandingTime.Name = "mnuLandingTime";
            this.mnuLandingTime.Size = new System.Drawing.Size(153, 22);
            this.mnuLandingTime.Text = "LandingTime";
            this.mnuLandingTime.Click += new System.EventHandler(this.mnuLandingTime_Click);
            // 
            // mnuBezierDetectPointfarthest
            // 
            this.mnuBezierDetectPointfarthest.Name = "mnuBezierDetectPointfarthest";
            this.mnuBezierDetectPointfarthest.Size = new System.Drawing.Size(258, 22);
            this.mnuBezierDetectPointfarthest.Text = "BezierDetectPointfarthest";
            this.mnuBezierDetectPointfarthest.Click += new System.EventHandler(this.mnuBezierDetectPointfarthest_Click);
            // 
            // mnuBezierDetectPoint
            // 
            this.mnuBezierDetectPoint.Name = "mnuBezierDetectPoint";
            this.mnuBezierDetectPoint.Size = new System.Drawing.Size(258, 22);
            this.mnuBezierDetectPoint.Text = "BezierDetectPoint";
            this.mnuBezierDetectPoint.Click += new System.EventHandler(this.mnuBezierDetectPoint_Click);
            // 
            // mnuCreatePointLayer
            // 
            this.mnuCreatePointLayer.Name = "mnuCreatePointLayer";
            this.mnuCreatePointLayer.Size = new System.Drawing.Size(258, 22);
            this.mnuCreatePointLayer.Text = "生成点图层";
            this.mnuCreatePointLayer.Click += new System.EventHandler(this.mnuCreatePointLayer_Click);
            // 
            // mnuLookingForNeighboursDT
            // 
            this.mnuLookingForNeighboursDT.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuTemporary,
            this.mnuCreatePointLayer,
            this.mnuBezierDetectPoint,
            this.mnuBezierDetectPointfarthest,
            this.mnuExcelToShape,
            this.mnuEnlargeLayer,
            this.mnuMatchAndMergePolylines,
            this.mnuLookingForNeighboursSweepLine,
            this.mnuLookingForNeighboursDT2,
            this.mnuLookingForNeighboursGrids,
            this.mnuCreateRandomPointLayer,
            this.mnuCoordinatesTransformation,
            this.mnuGaussianPerturbation,
            this.mnuDeletePointsWithSameCoordinates,
            this.mnuUnifyDirections,
            this.toIpeToolStripMenuItem,
            this.mnuCalGeo_Ipe,
            this.mnuTopologyChecker,
            this.mnuStatisticsOfDataSets,
            this.mnuTestDistanceMetric,
            this.mnuMatchAndMergePolygons,
            this.mnuMergeAndSplitPolylines,
            this.mnuUnifyIndicesPolylines,
            this.mnuIdentifyCorrCpgAddRegionNum,
            this.mnuCompareExcel,
            this.mnuSelectRandomly});
            this.mnuLookingForNeighboursDT.Name = "mnuLookingForNeighboursDT";
            this.mnuLookingForNeighboursDT.Size = new System.Drawing.Size(52, 20);
            this.mnuLookingForNeighboursDT.Text = "Aid(&T)";
            // 
            // mnuTemporary
            // 
            this.mnuTemporary.Name = "mnuTemporary";
            this.mnuTemporary.Size = new System.Drawing.Size(258, 22);
            this.mnuTemporary.Text = "Temporary";
            this.mnuTemporary.Click += new System.EventHandler(this.mnuTemporary_Click);
            // 
            // mnuExcelToShape
            // 
            this.mnuExcelToShape.Name = "mnuExcelToShape";
            this.mnuExcelToShape.Size = new System.Drawing.Size(258, 22);
            this.mnuExcelToShape.Text = "ExcelToShape";
            this.mnuExcelToShape.Click += new System.EventHandler(this.mnuExcelToShape_Click);
            // 
            // mnuEnlargeLayer
            // 
            this.mnuEnlargeLayer.Name = "mnuEnlargeLayer";
            this.mnuEnlargeLayer.Size = new System.Drawing.Size(258, 22);
            this.mnuEnlargeLayer.Text = "EnlargeLayer";
            this.mnuEnlargeLayer.Click += new System.EventHandler(this.mnuEnlargeLayer_Click);
            // 
            // mnuMatchAndMergePolylines
            // 
            this.mnuMatchAndMergePolylines.Name = "mnuMatchAndMergePolylines";
            this.mnuMatchAndMergePolylines.Size = new System.Drawing.Size(258, 22);
            this.mnuMatchAndMergePolylines.Text = "MatchAndMergePolylines";
            this.mnuMatchAndMergePolylines.Click += new System.EventHandler(this.mnuMatchAndMergePolylines_Click);
            // 
            // mnuLookingForNeighboursSweepLine
            // 
            this.mnuLookingForNeighboursSweepLine.Name = "mnuLookingForNeighboursSweepLine";
            this.mnuLookingForNeighboursSweepLine.Size = new System.Drawing.Size(258, 22);
            this.mnuLookingForNeighboursSweepLine.Text = "LookingForNeighboursSweepLine";
            this.mnuLookingForNeighboursSweepLine.Click += new System.EventHandler(this.mnuLookingForNeighboursSweepLine_Click);
            // 
            // mnuLookingForNeighboursDT2
            // 
            this.mnuLookingForNeighboursDT2.Name = "mnuLookingForNeighboursDT2";
            this.mnuLookingForNeighboursDT2.Size = new System.Drawing.Size(258, 22);
            this.mnuLookingForNeighboursDT2.Text = "LookingForNeighboursDT";
            this.mnuLookingForNeighboursDT2.Click += new System.EventHandler(this.mnuLookingForNeighboursDT2_Click);
            // 
            // mnuLookingForNeighboursGrids
            // 
            this.mnuLookingForNeighboursGrids.Name = "mnuLookingForNeighboursGrids";
            this.mnuLookingForNeighboursGrids.Size = new System.Drawing.Size(258, 22);
            this.mnuLookingForNeighboursGrids.Text = "LookingForNeighboursGrids";
            this.mnuLookingForNeighboursGrids.Click += new System.EventHandler(this.mnuLookingForNeighboursGrids_Click);
            // 
            // mnuCreateRandomPointLayer
            // 
            this.mnuCreateRandomPointLayer.Name = "mnuCreateRandomPointLayer";
            this.mnuCreateRandomPointLayer.Size = new System.Drawing.Size(258, 22);
            this.mnuCreateRandomPointLayer.Text = "CreateRandomPointLayer";
            this.mnuCreateRandomPointLayer.Click += new System.EventHandler(this.mnuCreateRandomPointLayer_Click);
            // 
            // mnuCoordinatesTransformation
            // 
            this.mnuCoordinatesTransformation.Name = "mnuCoordinatesTransformation";
            this.mnuCoordinatesTransformation.Size = new System.Drawing.Size(258, 22);
            this.mnuCoordinatesTransformation.Text = "CoordinatesTransformation";
            this.mnuCoordinatesTransformation.Click += new System.EventHandler(this.mnuCoordinatesTransformation_Click);
            // 
            // mnuGaussianPerturbation
            // 
            this.mnuGaussianPerturbation.Name = "mnuGaussianPerturbation";
            this.mnuGaussianPerturbation.Size = new System.Drawing.Size(258, 22);
            this.mnuGaussianPerturbation.Text = "GaussianPerturbation";
            this.mnuGaussianPerturbation.Click += new System.EventHandler(this.mnuGaussianPerturbation_Click);
            // 
            // mnuDeletePointsWithSameCoordinates
            // 
            this.mnuDeletePointsWithSameCoordinates.Name = "mnuDeletePointsWithSameCoordinates";
            this.mnuDeletePointsWithSameCoordinates.Size = new System.Drawing.Size(258, 22);
            this.mnuDeletePointsWithSameCoordinates.Text = "DeletePointsWithSameCoordinates";
            this.mnuDeletePointsWithSameCoordinates.Click += new System.EventHandler(this.mnuDeletePointsWithSameCoordinates_Click);
            // 
            // mnuUnifyDirections
            // 
            this.mnuUnifyDirections.Name = "mnuUnifyDirections";
            this.mnuUnifyDirections.Size = new System.Drawing.Size(258, 22);
            this.mnuUnifyDirections.Text = "UnifyDirections";
            this.mnuUnifyDirections.Click += new System.EventHandler(this.mnuUnifyDirections_Click);
            // 
            // toIpeToolStripMenuItem
            // 
            this.toIpeToolStripMenuItem.Name = "toIpeToolStripMenuItem";
            this.toIpeToolStripMenuItem.Size = new System.Drawing.Size(258, 22);
            this.toIpeToolStripMenuItem.Text = "ToIpe";
            this.toIpeToolStripMenuItem.Click += new System.EventHandler(this.mnuToIpe);
            // 
            // mnuCalGeo_Ipe
            // 
            this.mnuCalGeo_Ipe.Name = "mnuCalGeo_Ipe";
            this.mnuCalGeo_Ipe.Size = new System.Drawing.Size(258, 22);
            this.mnuCalGeo_Ipe.Text = "CalGeo_Ipe";
            this.mnuCalGeo_Ipe.Click += new System.EventHandler(this.mnuCalGeo_Ipe_Click);
            // 
            // mnuTopologyChecker
            // 
            this.mnuTopologyChecker.Name = "mnuTopologyChecker";
            this.mnuTopologyChecker.Size = new System.Drawing.Size(258, 22);
            this.mnuTopologyChecker.Text = "TopologyChecker";
            this.mnuTopologyChecker.Click += new System.EventHandler(this.mnuTopologyChecker_Click);
            // 
            // mnuStatisticsOfDataSets
            // 
            this.mnuStatisticsOfDataSets.Name = "mnuStatisticsOfDataSets";
            this.mnuStatisticsOfDataSets.Size = new System.Drawing.Size(258, 22);
            this.mnuStatisticsOfDataSets.Text = "StatisticsOfDataSets";
            this.mnuStatisticsOfDataSets.Click += new System.EventHandler(this.mnuStatisticsOfDataSets_Click);
            // 
            // mnuTestDistanceMetric
            // 
            this.mnuTestDistanceMetric.Name = "mnuTestDistanceMetric";
            this.mnuTestDistanceMetric.Size = new System.Drawing.Size(258, 22);
            this.mnuTestDistanceMetric.Text = "TestDistanceMetric";
            this.mnuTestDistanceMetric.Click += new System.EventHandler(this.mnuTestDistanceMetric_Click);
            // 
            // mnuMatchAndMergePolygons
            // 
            this.mnuMatchAndMergePolygons.Name = "mnuMatchAndMergePolygons";
            this.mnuMatchAndMergePolygons.Size = new System.Drawing.Size(258, 22);
            this.mnuMatchAndMergePolygons.Text = "MatchAndMergePolygons";
            this.mnuMatchAndMergePolygons.Click += new System.EventHandler(this.mnuMatchAndMergePolygons_Click);
            // 
            // mnuMergeAndSplitPolylines
            // 
            this.mnuMergeAndSplitPolylines.Name = "mnuMergeAndSplitPolylines";
            this.mnuMergeAndSplitPolylines.Size = new System.Drawing.Size(258, 22);
            this.mnuMergeAndSplitPolylines.Text = "MergeAndSplitPolylines";
            this.mnuMergeAndSplitPolylines.Click += new System.EventHandler(this.mnuMergeAndSplitPolylines_Click);
            // 
            // mnuUnifyIndicesPolylines
            // 
            this.mnuUnifyIndicesPolylines.Name = "mnuUnifyIndicesPolylines";
            this.mnuUnifyIndicesPolylines.Size = new System.Drawing.Size(258, 22);
            this.mnuUnifyIndicesPolylines.Text = "UnifyIndicesPolylines";
            this.mnuUnifyIndicesPolylines.Click += new System.EventHandler(this.mnuUnifyIndicesPolylines_Click);
            // 
            // mnuIdentifyCorrCpgAddRegionNum
            // 
            this.mnuIdentifyCorrCpgAddRegionNum.Name = "mnuIdentifyCorrCpgAddRegionNum";
            this.mnuIdentifyCorrCpgAddRegionNum.Size = new System.Drawing.Size(258, 22);
            this.mnuIdentifyCorrCpgAddRegionNum.Text = "IdentifyCorrCpgAddRegionNum";
            this.mnuIdentifyCorrCpgAddRegionNum.Click += new System.EventHandler(this.mnuIdentifyCorrCpgAddRegionNum_Click);
            // 
            // mnuCompareExcel
            // 
            this.mnuCompareExcel.Name = "mnuCompareExcel";
            this.mnuCompareExcel.Size = new System.Drawing.Size(258, 22);
            this.mnuCompareExcel.Text = "CompareExcel";
            this.mnuCompareExcel.Click += new System.EventHandler(this.mnuCompareExcel_Click);
            // 
            // mnuSelectRandomly
            // 
            this.mnuSelectRandomly.Name = "mnuSelectRandomly";
            this.mnuSelectRandomly.Size = new System.Drawing.Size(258, 22);
            this.mnuSelectRandomly.Text = "SelectRandomly";
            this.mnuSelectRandomly.Click += new System.EventHandler(this.mnuSelectRandomly_Click);
            // 
            // mnuOrdinal
            // 
            this.mnuOrdinal.Name = "mnuOrdinal";
            this.mnuOrdinal.Size = new System.Drawing.Size(153, 22);
            this.mnuOrdinal.Text = "Ordinal";
            this.mnuOrdinal.Click += new System.EventHandler(this.mnuOrdinal_Click);
            // 
            // mnuSimultaneity
            // 
            this.mnuSimultaneity.Name = "mnuSimultaneity";
            this.mnuSimultaneity.Size = new System.Drawing.Size(153, 22);
            this.mnuSimultaneity.Text = "Simultaneity";
            this.mnuSimultaneity.Click += new System.EventHandler(this.mnuSimultaneity_Click);
            // 
            // mnuApLALMulti
            // 
            this.mnuApLALMulti.Name = "mnuApLALMulti";
            this.mnuApLALMulti.Size = new System.Drawing.Size(137, 22);
            this.mnuApLALMulti.Text = "ApLALMulti";
            this.mnuApLALMulti.Click += new System.EventHandler(this.mnuApLALMulti_Click);
            // 
            // mnuLinear_AL
            // 
            this.mnuLinear_AL.Name = "mnuLinear_AL";
            this.mnuLinear_AL.Size = new System.Drawing.Size(137, 22);
            this.mnuLinear_AL.Text = "Linear_AL";
            this.mnuLinear_AL.Click += new System.EventHandler(this.mnuLinear_AL_Click);
            // 
            // mnuPAL
            // 
            this.mnuPAL.Name = "mnuPAL";
            this.mnuPAL.Size = new System.Drawing.Size(137, 22);
            this.mnuPAL.Text = "PAL";
            this.mnuPAL.Click += new System.EventHandler(this.mnuPAL_Click);
            // 
            // mnuGeneralization
            // 
            this.mnuGeneralization.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuDPSimplify,
            this.mnuAreaAgg_AStar,
            this.mnuBldgGrow});
            this.mnuGeneralization.Name = "mnuGeneralization";
            this.mnuGeneralization.Size = new System.Drawing.Size(94, 20);
            this.mnuGeneralization.Text = "Generalization";
            // 
            // mnuDPSimplify
            // 
            this.mnuDPSimplify.Name = "mnuDPSimplify";
            this.mnuDPSimplify.Size = new System.Drawing.Size(153, 22);
            this.mnuDPSimplify.Text = "DPSimplify";
            this.mnuDPSimplify.Click += new System.EventHandler(this.mnuDPSimplify_Click);
            // 
            // mnuAreaAgg_AStar
            // 
            this.mnuAreaAgg_AStar.Name = "mnuAreaAgg_AStar";
            this.mnuAreaAgg_AStar.Size = new System.Drawing.Size(153, 22);
            this.mnuAreaAgg_AStar.Text = "AreaAgg_AStar";
            this.mnuAreaAgg_AStar.Click += new System.EventHandler(this.mnuAreaAgg_AStar_Click);
            // 
            // mnuBldgGrow
            // 
            this.mnuBldgGrow.Name = "mnuBldgGrow";
            this.mnuBldgGrow.Size = new System.Drawing.Size(153, 22);
            this.mnuBldgGrow.Text = "BldgGrow";
            this.mnuBldgGrow.Click += new System.EventHandler(this.mnuBldgGrow_Click);
            // 
            // morphingExtendToolStripMenuItem
            // 
            this.morphingExtendToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuSimultaneity,
            this.mnuOrdinal,
            this.mnuLandingTime,
            this.mnuLandingTimeLI});
            this.morphingExtendToolStripMenuItem.Name = "morphingExtendToolStripMenuItem";
            this.morphingExtendToolStripMenuItem.Size = new System.Drawing.Size(107, 20);
            this.morphingExtendToolStripMenuItem.Text = "MorphingExtend";
            // 
            // mnuCDTLSA
            // 
            this.mnuCDTLSA.Name = "mnuCDTLSA";
            this.mnuCDTLSA.Size = new System.Drawing.Size(117, 22);
            this.mnuCDTLSA.Text = "CDTLSA";
            this.mnuCDTLSA.Click += new System.EventHandler(this.mnuCDTLSA_Click);
            // 
            // fishEyeToolStripMenuItem
            // 
            this.fishEyeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuCDTLSA});
            this.fishEyeToolStripMenuItem.Name = "fishEyeToolStripMenuItem";
            this.fishEyeToolStripMenuItem.Size = new System.Drawing.Size(58, 20);
            this.fishEyeToolStripMenuItem.Text = "FishEye";
            // 
            // tbcRight
            // 
            this.tbcRight.Controls.Add(this.tabPageMap);
            this.tbcRight.Controls.Add(this.tabPageLayout);
            this.tbcRight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcRight.Location = new System.Drawing.Point(0, 0);
            this.tbcRight.Name = "tbcRight";
            this.tbcRight.SelectedIndex = 0;
            this.tbcRight.Size = new System.Drawing.Size(911, 640);
            this.tbcRight.TabIndex = 0;
            // 
            // tabPageMap
            // 
            this.tabPageMap.Controls.Add(this.axLicenseControl1);
            this.tabPageMap.Controls.Add(this.axMapControl);
            this.tabPageMap.Controls.Add(this.splitContainer2);
            this.tabPageMap.Location = new System.Drawing.Point(4, 22);
            this.tabPageMap.Name = "tabPageMap";
            this.tabPageMap.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageMap.Size = new System.Drawing.Size(903, 614);
            this.tabPageMap.TabIndex = 0;
            this.tabPageMap.Text = "Map";
            this.tabPageMap.UseVisualStyleBackColor = true;
            // 
            // axLicenseControl1
            // 
            this.axLicenseControl1.Enabled = true;
            this.axLicenseControl1.Location = new System.Drawing.Point(355, 218);
            this.axLicenseControl1.Name = "axLicenseControl1";
            this.axLicenseControl1.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axLicenseControl1.OcxState")));
            this.axLicenseControl1.Size = new System.Drawing.Size(32, 32);
            this.axLicenseControl1.TabIndex = 5;
            // 
            // axMapControl
            // 
            this.axMapControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axMapControl.Location = new System.Drawing.Point(3, 3);
            this.axMapControl.Name = "axMapControl";
            this.axMapControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMapControl.OcxState")));
            this.axMapControl.Size = new System.Drawing.Size(897, 585);
            this.axMapControl.TabIndex = 7;
            this.axMapControl.OnMouseMove += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseMoveEventHandler(this.axMapControl_OnMouseMove);
            this.axMapControl.OnAfterScreenDraw += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnAfterScreenDrawEventHandler(this.axMapControl_OnAfterScreenDraw);
            this.axMapControl.OnExtentUpdated += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnExtentUpdatedEventHandler(this.axMapControl_OnExtentUpdated);
            this.axMapControl.OnMapReplaced += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMapReplacedEventHandler(this.axMapControl_OnMapReplaced);
            // 
            // splitContainer2
            // 
            this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.splitContainer2.Location = new System.Drawing.Point(3, 588);
            this.splitContainer2.Name = "splitContainer2";
            // 
            // splitContainer2.Panel1
            // 
            this.splitContainer2.Panel1.Controls.Add(this.txtT);
            // 
            // splitContainer2.Panel2
            // 
            this.splitContainer2.Panel2.Controls.Add(this.txtVtPV);
            this.splitContainer2.Size = new System.Drawing.Size(897, 23);
            this.splitContainer2.SplitterDistance = 297;
            this.splitContainer2.TabIndex = 3;
            // 
            // txtT
            // 
            this.txtT.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtT.Location = new System.Drawing.Point(0, 0);
            this.txtT.Name = "txtT";
            this.txtT.ReadOnly = true;
            this.txtT.Size = new System.Drawing.Size(297, 20);
            this.txtT.TabIndex = 2;
            // 
            // txtVtPV
            // 
            this.txtVtPV.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtVtPV.Location = new System.Drawing.Point(0, 0);
            this.txtVtPV.Name = "txtVtPV";
            this.txtVtPV.ReadOnly = true;
            this.txtVtPV.Size = new System.Drawing.Size(596, 20);
            this.txtVtPV.TabIndex = 1;
            // 
            // tabPageLayout
            // 
            this.tabPageLayout.Controls.Add(this.axPageLayoutControl);
            this.tabPageLayout.Location = new System.Drawing.Point(4, 22);
            this.tabPageLayout.Name = "tabPageLayout";
            this.tabPageLayout.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLayout.Size = new System.Drawing.Size(903, 614);
            this.tabPageLayout.TabIndex = 1;
            this.tabPageLayout.Text = "Layout";
            this.tabPageLayout.UseVisualStyleBackColor = true;
            // 
            // axPageLayoutControl
            // 
            this.axPageLayoutControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axPageLayoutControl.Location = new System.Drawing.Point(3, 3);
            this.axPageLayoutControl.Name = "axPageLayoutControl";
            this.axPageLayoutControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axPageLayoutControl.OcxState")));
            this.axPageLayoutControl.Size = new System.Drawing.Size(897, 608);
            this.axPageLayoutControl.TabIndex = 0;
            // 
            // propertyGrid
            // 
            this.propertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGrid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.propertyGrid.Location = new System.Drawing.Point(3, 3);
            this.propertyGrid.Name = "propertyGrid";
            this.propertyGrid.Size = new System.Drawing.Size(234, 427);
            this.propertyGrid.TabIndex = 0;
            // 
            // tabPageProperty
            // 
            this.tabPageProperty.Controls.Add(this.propertyGrid);
            this.tabPageProperty.Location = new System.Drawing.Point(4, 4);
            this.tabPageProperty.Name = "tabPageProperty";
            this.tabPageProperty.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageProperty.Size = new System.Drawing.Size(240, 433);
            this.tabPageProperty.TabIndex = 1;
            this.tabPageProperty.Text = "Property";
            this.tabPageProperty.UseVisualStyleBackColor = true;
            // 
            // axTOCControl
            // 
            this.axTOCControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axTOCControl.Location = new System.Drawing.Point(3, 3);
            this.axTOCControl.Name = "axTOCControl";
            this.axTOCControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axTOCControl.OcxState")));
            this.axTOCControl.Size = new System.Drawing.Size(234, 427);
            this.axTOCControl.TabIndex = 0;
            this.axTOCControl.OnMouseDown += new ESRI.ArcGIS.Controls.ITOCControlEvents_Ax_OnMouseDownEventHandler(this.axTOCControl_OnMouseDown);
            this.axTOCControl.OnDoubleClick += new ESRI.ArcGIS.Controls.ITOCControlEvents_Ax_OnDoubleClickEventHandler(this.axTOCControl_OnDoubleClick);
            // 
            // axMapControlEagleEye
            // 
            this.axMapControlEagleEye.Dock = System.Windows.Forms.DockStyle.Fill;
            this.axMapControlEagleEye.Location = new System.Drawing.Point(0, 0);
            this.axMapControlEagleEye.Name = "axMapControlEagleEye";
            this.axMapControlEagleEye.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axMapControlEagleEye.OcxState")));
            this.axMapControlEagleEye.Size = new System.Drawing.Size(248, 177);
            this.axMapControlEagleEye.TabIndex = 0;
            this.axMapControlEagleEye.OnMouseDown += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseDownEventHandler(this.axMapControlEagleEye_OnMouseDown);
            this.axMapControlEagleEye.OnMouseMove += new ESRI.ArcGIS.Controls.IMapControlEvents2_Ax_OnMouseMoveEventHandler(this.axMapControlEagleEye_OnMouseMove);
            // 
            // tabPage1Layer
            // 
            this.tabPage1Layer.Controls.Add(this.axTOCControl);
            this.tabPage1Layer.Location = new System.Drawing.Point(4, 4);
            this.tabPage1Layer.Name = "tabPage1Layer";
            this.tabPage1Layer.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1Layer.Size = new System.Drawing.Size(240, 433);
            this.tabPage1Layer.TabIndex = 0;
            this.tabPage1Layer.Text = "Layer";
            this.tabPage1Layer.UseVisualStyleBackColor = true;
            // 
            // tbcLeft
            // 
            this.tbcLeft.Alignment = System.Windows.Forms.TabAlignment.Bottom;
            this.tbcLeft.Controls.Add(this.tabPage1Layer);
            this.tbcLeft.Controls.Add(this.tabPageProperty);
            this.tbcLeft.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbcLeft.Location = new System.Drawing.Point(0, 0);
            this.tbcLeft.Name = "tbcLeft";
            this.tbcLeft.SelectedIndex = 0;
            this.tbcLeft.Size = new System.Drawing.Size(248, 459);
            this.tbcLeft.TabIndex = 1;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 0);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.tbcLeft);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.axMapControlEagleEye);
            this.splitContainer1.Size = new System.Drawing.Size(248, 640);
            this.splitContainer1.SplitterDistance = 459;
            this.splitContainer1.TabIndex = 0;
            // 
            // splitContainer
            // 
            this.splitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer.Location = new System.Drawing.Point(0, 52);
            this.splitContainer.Name = "splitContainer";
            // 
            // splitContainer.Panel1
            // 
            this.splitContainer.Panel1.Controls.Add(this.splitContainer1);
            // 
            // splitContainer.Panel2
            // 
            this.splitContainer.Panel2.Controls.Add(this.tbcRight);
            this.splitContainer.Size = new System.Drawing.Size(1163, 640);
            this.splitContainer.SplitterDistance = 248;
            this.splitContainer.TabIndex = 16;
            // 
            // axToolbarControl
            // 
            this.axToolbarControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.axToolbarControl.Location = new System.Drawing.Point(0, 24);
            this.axToolbarControl.Name = "axToolbarControl";
            this.axToolbarControl.OcxState = ((System.Windows.Forms.AxHost.State)(resources.GetObject("axToolbarControl.OcxState")));
            this.axToolbarControl.Size = new System.Drawing.Size(1163, 28);
            this.axToolbarControl.TabIndex = 15;
            // 
            // mnuAnyTest
            // 
            this.mnuAnyTest.Name = "mnuAnyTest";
            this.mnuAnyTest.Size = new System.Drawing.Size(62, 20);
            this.mnuAnyTest.Text = "AnyTest";
            this.mnuAnyTest.Click += new System.EventHandler(this.mnuAnyTest_Click);
            // 
            // mnuALALMulti
            // 
            this.mnuALALMulti.Name = "mnuALALMulti";
            this.mnuALALMulti.Size = new System.Drawing.Size(137, 22);
            this.mnuALALMulti.Text = "ALALMulti";
            this.mnuALALMulti.Click += new System.EventHandler(this.mnuALALMulti_Click);
            // 
            // mnuALAMulti
            // 
            this.mnuALAMulti.Name = "mnuALAMulti";
            this.mnuALAMulti.Size = new System.Drawing.Size(137, 22);
            this.mnuALAMulti.Text = "ALAMulti";
            this.mnuALAMulti.Click += new System.EventHandler(this.mnuALAMulti_Click);
            // 
            // mnuCALMulti
            // 
            this.mnuCALMulti.Name = "mnuCALMulti";
            this.mnuCALMulti.Size = new System.Drawing.Size(137, 22);
            this.mnuCALMulti.Text = "ALLMulti";
            this.mnuCALMulti.Click += new System.EventHandler(this.mnuALLMulti_Click);
            // 
            // mnuOpen
            // 
            this.mnuOpen.Image = ((System.Drawing.Image)(resources.GetObject("mnuOpen.Image")));
            this.mnuOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuOpen.Name = "mnuOpen";
            this.mnuOpen.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.mnuOpen.Size = new System.Drawing.Size(203, 22);
            this.mnuOpen.Text = "打开(&O)";
            this.mnuOpen.Click += new System.EventHandler(this.mnuOpen_Click);
            // 
            // mnuNew
            // 
            this.mnuNew.Image = ((System.Drawing.Image)(resources.GetObject("mnuNew.Image")));
            this.mnuNew.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuNew.Name = "mnuNew";
            this.mnuNew.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.mnuNew.Size = new System.Drawing.Size(203, 22);
            this.mnuNew.Text = "新建(&N)";
            this.mnuNew.Click += new System.EventHandler(this.mnuNew_Click);
            // 
            // mnuFile
            // 
            this.mnuFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuNew,
            this.mnuOpen,
            this.toolStripSeparator,
            this.mnuSave,
            this.mnuSaveAs,
            this.toolStripSeparator2,
            this.mnuAddData,
            this.toolStripSeparator1,
            this.mnuExportViewToImage,
            this.toolStripSeparator4,
            this.mnuExit});
            this.mnuFile.Name = "mnuFile";
            this.mnuFile.Size = new System.Drawing.Size(51, 20);
            this.mnuFile.Text = "File(&F)";
            // 
            // toolStripSeparator
            // 
            this.toolStripSeparator.Name = "toolStripSeparator";
            this.toolStripSeparator.Size = new System.Drawing.Size(200, 6);
            // 
            // mnuSave
            // 
            this.mnuSave.Image = ((System.Drawing.Image)(resources.GetObject("mnuSave.Image")));
            this.mnuSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.mnuSave.Name = "mnuSave";
            this.mnuSave.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
            this.mnuSave.Size = new System.Drawing.Size(203, 22);
            this.mnuSave.Text = "保存(&S)";
            this.mnuSave.Click += new System.EventHandler(this.mnuSave_Click);
            // 
            // mnuSaveAs
            // 
            this.mnuSaveAs.Name = "mnuSaveAs";
            this.mnuSaveAs.Size = new System.Drawing.Size(203, 22);
            this.mnuSaveAs.Text = "另存为(&A)";
            this.mnuSaveAs.Click += new System.EventHandler(this.mnuSaveAs_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(200, 6);
            // 
            // mnuAddData
            // 
            this.mnuAddData.Image = ((System.Drawing.Image)(resources.GetObject("mnuAddData.Image")));
            this.mnuAddData.Name = "mnuAddData";
            this.mnuAddData.Size = new System.Drawing.Size(203, 22);
            this.mnuAddData.Text = "添加数据";
            this.mnuAddData.Click += new System.EventHandler(this.mnuAddData_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
            // 
            // mnuExportViewToImage
            // 
            this.mnuExportViewToImage.Image = ((System.Drawing.Image)(resources.GetObject("mnuExportViewToImage.Image")));
            this.mnuExportViewToImage.Name = "mnuExportViewToImage";
            this.mnuExportViewToImage.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.E)));
            this.mnuExportViewToImage.Size = new System.Drawing.Size(203, 22);
            this.mnuExportViewToImage.Text = "导出视图为JPG(&E)";
            this.mnuExportViewToImage.Click += new System.EventHandler(this.mnuExportViewToImage_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(200, 6);
            // 
            // mnuExit
            // 
            this.mnuExit.Image = ((System.Drawing.Image)(resources.GetObject("mnuExit.Image")));
            this.mnuExit.Name = "mnuExit";
            this.mnuExit.Size = new System.Drawing.Size(203, 22);
            this.mnuExit.Text = "退出(&X)";
            this.mnuExit.Click += new System.EventHandler(this.mnuExit_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuFile,
            this.morphingToolStripMenuItem,
            this.MorphingLSAToolStripMenuItem,
            this.mnuGeneralization,
            this.roadnetworkToolStripMenuItem,
            this.fishEyeToolStripMenuItem,
            this.morphingExtendToolStripMenuItem,
            this.toolStripMenuItem1,
            this.mnuLookingForNeighboursDT,
            this.mnuAnyTest,
            this.otherToolStripMenuItem,
            this.kARSToolStripMenuItem,
            this.mnuHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(1163, 24);
            this.menuStrip.TabIndex = 13;
            this.menuStrip.Text = "menuStrip1";
            // 
            // morphingToolStripMenuItem
            // 
            this.morphingToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuLinear,
            this.mnuLinearMulti,
            this.mnuVertexInsertion,
            this.mnuMPBDP,
            this.mnuMPBDPBL,
            this.mnuMRL,
            this.mnuMRLCut,
            this.mnuOptCor,
            this.mnuOptCorBezier,
            this.mnuMPBBSL,
            this.mnuMPBBSLDP,
            this.mnuBLGOptCorMMSimplified,
            this.mnuBSBLGOptCorMMSimplified,
            this.mnuBSBLGOptCor,
            this.mnuBSBLGOptCorMM,
            this.mnuRIBS,
            this.mnuRIBSBLG,
            this.mnuRIBSBLGOptCor,
            this.mnuAtBdMorphing,
            this.mnuCGABM,
            this.mnuTest});
            this.morphingToolStripMenuItem.Name = "morphingToolStripMenuItem";
            this.morphingToolStripMenuItem.Size = new System.Drawing.Size(72, 20);
            this.morphingToolStripMenuItem.Text = "Morphing";
            // 
            // mnuLinear
            // 
            this.mnuLinear.Name = "mnuLinear";
            this.mnuLinear.Size = new System.Drawing.Size(222, 22);
            this.mnuLinear.Text = "Linear";
            this.mnuLinear.Click += new System.EventHandler(this.mnuLinear_Click);
            // 
            // mnuLinearMulti
            // 
            this.mnuLinearMulti.Name = "mnuLinearMulti";
            this.mnuLinearMulti.Size = new System.Drawing.Size(222, 22);
            this.mnuLinearMulti.Text = "LinearMulti";
            this.mnuLinearMulti.Click += new System.EventHandler(this.mnuLinearMulti_Click);
            // 
            // mnuVertexInsertion
            // 
            this.mnuVertexInsertion.Name = "mnuVertexInsertion";
            this.mnuVertexInsertion.Size = new System.Drawing.Size(222, 22);
            this.mnuVertexInsertion.Text = "VertexInsertion";
            // 
            // mnuMPBDP
            // 
            this.mnuMPBDP.Name = "mnuMPBDP";
            this.mnuMPBDP.Size = new System.Drawing.Size(222, 22);
            this.mnuMPBDP.Text = "MPBDP";
            this.mnuMPBDP.Click += new System.EventHandler(this.mnuMPBDP_Click);
            // 
            // mnuMPBDPBL
            // 
            this.mnuMPBDPBL.Name = "mnuMPBDPBL";
            this.mnuMPBDPBL.Size = new System.Drawing.Size(222, 22);
            this.mnuMPBDPBL.Text = "MPBDPBL";
            this.mnuMPBDPBL.Click += new System.EventHandler(this.mnuMPBDPBL_Click);
            // 
            // mnuMRL
            // 
            this.mnuMRL.Name = "mnuMRL";
            this.mnuMRL.Size = new System.Drawing.Size(222, 22);
            this.mnuMRL.Text = "MRL";
            this.mnuMRL.Click += new System.EventHandler(this.mnuMRL_Click);
            // 
            // mnuMRLCut
            // 
            this.mnuMRLCut.Name = "mnuMRLCut";
            this.mnuMRLCut.Size = new System.Drawing.Size(222, 22);
            this.mnuMRLCut.Text = "MRLCut";
            this.mnuMRLCut.Click += new System.EventHandler(this.mnuMRLCut_Click);
            // 
            // mnuOptCor
            // 
            this.mnuOptCor.Name = "mnuOptCor";
            this.mnuOptCor.Size = new System.Drawing.Size(222, 22);
            this.mnuOptCor.Text = "OptCor";
            this.mnuOptCor.Click += new System.EventHandler(this.mnuOptCor_Click);
            // 
            // mnuOptCorBezier
            // 
            this.mnuOptCorBezier.Name = "mnuOptCorBezier";
            this.mnuOptCorBezier.Size = new System.Drawing.Size(222, 22);
            this.mnuOptCorBezier.Text = "OptCorBezier";
            this.mnuOptCorBezier.Click += new System.EventHandler(this.mnuOptCorBezier_Click);
            // 
            // mnuMPBBSL
            // 
            this.mnuMPBBSL.Name = "mnuMPBBSL";
            this.mnuMPBBSL.Size = new System.Drawing.Size(222, 22);
            this.mnuMPBBSL.Text = "MPBBSL";
            this.mnuMPBBSL.Click += new System.EventHandler(this.mnuMPBBSL_Click);
            // 
            // mnuMPBBSLDP
            // 
            this.mnuMPBBSLDP.Name = "mnuMPBBSLDP";
            this.mnuMPBBSLDP.Size = new System.Drawing.Size(222, 22);
            this.mnuMPBBSLDP.Text = "MPBBSLDP";
            this.mnuMPBBSLDP.Click += new System.EventHandler(this.mnuMPBBSLDP_Click);
            // 
            // mnuBLGOptCorMMSimplified
            // 
            this.mnuBLGOptCorMMSimplified.Name = "mnuBLGOptCorMMSimplified";
            this.mnuBLGOptCorMMSimplified.Size = new System.Drawing.Size(222, 22);
            this.mnuBLGOptCorMMSimplified.Text = "BLGOptCorMMSimplified";
            this.mnuBLGOptCorMMSimplified.Click += new System.EventHandler(this.mnuBLGOptCorMMSimplified_Click);
            // 
            // mnuBSBLGOptCorMMSimplified
            // 
            this.mnuBSBLGOptCorMMSimplified.Name = "mnuBSBLGOptCorMMSimplified";
            this.mnuBSBLGOptCorMMSimplified.Size = new System.Drawing.Size(222, 22);
            this.mnuBSBLGOptCorMMSimplified.Text = "BSBLGOptCorMMSimplified";
            this.mnuBSBLGOptCorMMSimplified.Click += new System.EventHandler(this.mnuBSBLGOptCorMMSimplified_Click);
            // 
            // mnuBSBLGOptCor
            // 
            this.mnuBSBLGOptCor.Name = "mnuBSBLGOptCor";
            this.mnuBSBLGOptCor.Size = new System.Drawing.Size(222, 22);
            this.mnuBSBLGOptCor.Text = "BSBLGOptCor";
            this.mnuBSBLGOptCor.Click += new System.EventHandler(this.mnuBSBLGOptCor_Click);
            // 
            // mnuBSBLGOptCorMM
            // 
            this.mnuBSBLGOptCorMM.Name = "mnuBSBLGOptCorMM";
            this.mnuBSBLGOptCorMM.Size = new System.Drawing.Size(222, 22);
            this.mnuBSBLGOptCorMM.Text = "BSBLGOptCorMM";
            this.mnuBSBLGOptCorMM.Click += new System.EventHandler(this.mnuBSBLGOptCorMM_Click);
            // 
            // mnuRIBS
            // 
            this.mnuRIBS.Name = "mnuRIBS";
            this.mnuRIBS.Size = new System.Drawing.Size(222, 22);
            this.mnuRIBS.Text = "RIBS";
            this.mnuRIBS.Click += new System.EventHandler(this.mnuRIBS_Click);
            // 
            // mnuRIBSBLG
            // 
            this.mnuRIBSBLG.Name = "mnuRIBSBLG";
            this.mnuRIBSBLG.Size = new System.Drawing.Size(222, 22);
            this.mnuRIBSBLG.Text = "RIBSBLG";
            this.mnuRIBSBLG.Click += new System.EventHandler(this.mnuRIBSBLG_Click);
            // 
            // mnuRIBSBLGOptCor
            // 
            this.mnuRIBSBLGOptCor.Name = "mnuRIBSBLGOptCor";
            this.mnuRIBSBLGOptCor.Size = new System.Drawing.Size(222, 22);
            this.mnuRIBSBLGOptCor.Text = "RIBSBLGOptCor";
            this.mnuRIBSBLGOptCor.Click += new System.EventHandler(this.mnuRIBSBLGOptCor_Click);
            // 
            // mnuAtBdMorphing
            // 
            this.mnuAtBdMorphing.Name = "mnuAtBdMorphing";
            this.mnuAtBdMorphing.Size = new System.Drawing.Size(222, 22);
            this.mnuAtBdMorphing.Text = "AtBdMorphing";
            this.mnuAtBdMorphing.Click += new System.EventHandler(this.mnuAtBdMorphing_Click);
            // 
            // mnuCGABM
            // 
            this.mnuCGABM.Name = "mnuCGABM";
            this.mnuCGABM.Size = new System.Drawing.Size(222, 22);
            this.mnuCGABM.Text = "CGABM";
            this.mnuCGABM.Click += new System.EventHandler(this.mnuCGABM_Click);
            // 
            // mnuTest
            // 
            this.mnuTest.Name = "mnuTest";
            this.mnuTest.Size = new System.Drawing.Size(222, 22);
            this.mnuTest.Text = "Test";
            this.mnuTest.Click += new System.EventHandler(this.mnuTest_Click);
            // 
            // MorphingLSAToolStripMenuItem
            // 
            this.MorphingLSAToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuStraightLine,
            this.mnuAL_AL,
            this.mnuAL,
            this.mnuALL,
            this.mnuApLL,
            this.mnuALm,
            this.mnuLMulti,
            this.mnuCALMulti,
            this.mnuALAMulti,
            this.mnuALALMulti,
            this.mnuPAL,
            this.mnuLinear_AL,
            this.mnuApLALMulti});
            this.MorphingLSAToolStripMenuItem.Name = "MorphingLSAToolStripMenuItem";
            this.MorphingLSAToolStripMenuItem.Size = new System.Drawing.Size(92, 20);
            this.MorphingLSAToolStripMenuItem.Text = "MorphingLSA";
            // 
            // mnuStraightLine
            // 
            this.mnuStraightLine.Name = "mnuStraightLine";
            this.mnuStraightLine.Size = new System.Drawing.Size(137, 22);
            this.mnuStraightLine.Text = "StraightLine";
            this.mnuStraightLine.Click += new System.EventHandler(this.mnuStraightLine_Click);
            // 
            // mnuAL_AL
            // 
            this.mnuAL_AL.Name = "mnuAL_AL";
            this.mnuAL_AL.Size = new System.Drawing.Size(137, 22);
            this.mnuAL_AL.Text = "AL_AL";
            this.mnuAL_AL.Click += new System.EventHandler(this.mnuAL_AL_Click);
            // 
            // mnuAL
            // 
            this.mnuAL.Name = "mnuAL";
            this.mnuAL.Size = new System.Drawing.Size(137, 22);
            this.mnuAL.Text = "AL";
            this.mnuAL.Click += new System.EventHandler(this.mnuAL_Click);
            // 
            // mnuALL
            // 
            this.mnuALL.Name = "mnuALL";
            this.mnuALL.Size = new System.Drawing.Size(137, 22);
            this.mnuALL.Text = "ALL";
            this.mnuALL.Click += new System.EventHandler(this.mnuALL_Click);
            // 
            // mnuApLL
            // 
            this.mnuApLL.Name = "mnuApLL";
            this.mnuApLL.Size = new System.Drawing.Size(137, 22);
            this.mnuApLL.Text = "ApLL";
            this.mnuApLL.Click += new System.EventHandler(this.mnuApLL_Click);
            // 
            // mnuALm
            // 
            this.mnuALm.Name = "mnuALm";
            this.mnuALm.Size = new System.Drawing.Size(137, 22);
            this.mnuALm.Text = "ALm";
            this.mnuALm.Click += new System.EventHandler(this.mnuALm_Click);
            // 
            // mnuLMulti
            // 
            this.mnuLMulti.Name = "mnuLMulti";
            this.mnuLMulti.Size = new System.Drawing.Size(137, 22);
            this.mnuLMulti.Text = "LMulti";
            this.mnuLMulti.Click += new System.EventHandler(this.mnuLMulti_Click);
            // 
            // roadnetworkToolStripMenuItem
            // 
            this.roadnetworkToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuClassification,
            this.mnuRightAngelDPS,
            this.mnuLinearMorphing,
            this.mnuTransparencyMorphing});
            this.roadnetworkToolStripMenuItem.Name = "roadnetworkToolStripMenuItem";
            this.roadnetworkToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.roadnetworkToolStripMenuItem.Text = "RoadNetwork";
            // 
            // mnuClassification
            // 
            this.mnuClassification.Name = "mnuClassification";
            this.mnuClassification.Size = new System.Drawing.Size(198, 22);
            this.mnuClassification.Text = "Classification";
            this.mnuClassification.Click += new System.EventHandler(this.mnuClassification_Click);
            // 
            // mnuRightAngelDPS
            // 
            this.mnuRightAngelDPS.Name = "mnuRightAngelDPS";
            this.mnuRightAngelDPS.Size = new System.Drawing.Size(198, 22);
            this.mnuRightAngelDPS.Text = "RightAngelDPS";
            this.mnuRightAngelDPS.Click += new System.EventHandler(this.mnuRightAngelDPS_Click);
            // 
            // mnuLinearMorphing
            // 
            this.mnuLinearMorphing.Name = "mnuLinearMorphing";
            this.mnuLinearMorphing.Size = new System.Drawing.Size(198, 22);
            this.mnuLinearMorphing.Text = "LinearMorphing";
            this.mnuLinearMorphing.Click += new System.EventHandler(this.mnuLinearMorphing_Click);
            // 
            // mnuTransparencyMorphing
            // 
            this.mnuTransparencyMorphing.Name = "mnuTransparencyMorphing";
            this.mnuTransparencyMorphing.Size = new System.Drawing.Size(198, 22);
            this.mnuTransparencyMorphing.Text = "TransparencyMorphing";
            this.mnuTransparencyMorphing.Click += new System.EventHandler(this.mnuTransparencyMorphing_Click);
            // 
            // otherToolStripMenuItem
            // 
            this.otherToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuKillExcel,
            this.mnuClear});
            this.otherToolStripMenuItem.Name = "otherToolStripMenuItem";
            this.otherToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.otherToolStripMenuItem.Text = "Other";
            // 
            // mnuKillExcel
            // 
            this.mnuKillExcel.Name = "mnuKillExcel";
            this.mnuKillExcel.Size = new System.Drawing.Size(141, 22);
            this.mnuKillExcel.Text = "mnuKillExcel";
            this.mnuKillExcel.Click += new System.EventHandler(this.mnuKillExcel_Click);
            // 
            // mnuClear
            // 
            this.mnuClear.Name = "mnuClear";
            this.mnuClear.Size = new System.Drawing.Size(141, 22);
            this.mnuClear.Text = "Clear";
            this.mnuClear.Click += new System.EventHandler(this.mnuClear_Click);
            // 
            // kARSToolStripMenuItem
            // 
            this.kARSToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.mnuExtractPossibleFiles});
            this.kARSToolStripMenuItem.Name = "kARSToolStripMenuItem";
            this.kARSToolStripMenuItem.Size = new System.Drawing.Size(47, 20);
            this.kARSToolStripMenuItem.Text = "KARS";
            // 
            // mnuExtractPossibleFiles
            // 
            this.mnuExtractPossibleFiles.Name = "mnuExtractPossibleFiles";
            this.mnuExtractPossibleFiles.Size = new System.Drawing.Size(175, 22);
            this.mnuExtractPossibleFiles.Text = "ExtractPossibleFiles";
            this.mnuExtractPossibleFiles.Click += new System.EventHandler(this.mnuExtractPossibleFiles_Click);
            // 
            // mnuHelp
            // 
            this.mnuHelp.Name = "mnuHelp";
            this.mnuHelp.Size = new System.Drawing.Size(61, 20);
            this.mnuHelp.Text = "Help(&H)";
            // 
            // CoordinateLabel
            // 
            this.CoordinateLabel.AutoSize = false;
            this.CoordinateLabel.Name = "CoordinateLabel";
            this.CoordinateLabel.Size = new System.Drawing.Size(400, 19);
            this.CoordinateLabel.Text = "Coordinate";
            // 
            // Blank
            // 
            this.Blank.AccessibleDescription = "s";
            this.Blank.Name = "Blank";
            this.Blank.Size = new System.Drawing.Size(307, 19);
            this.Blank.Spring = true;
            // 
            // tsslMessage
            // 
            this.tsslMessage.Name = "tsslMessage";
            this.tsslMessage.Size = new System.Drawing.Size(39, 19);
            this.tsslMessage.Text = "Ready";
            // 
            // ststMain
            // 
            this.ststMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslMessage,
            this.Blank,
            this.CoordinateLabel,
            this.tsslTime,
            this.tspbMain});
            this.ststMain.Location = new System.Drawing.Point(0, 692);
            this.ststMain.Name = "ststMain";
            this.ststMain.Size = new System.Drawing.Size(1163, 24);
            this.ststMain.TabIndex = 14;
            this.ststMain.Text = "ststMain";
            // 
            // tsslTime
            // 
            this.tsslTime.AutoSize = false;
            this.tsslTime.Name = "tsslTime";
            this.tsslTime.Size = new System.Drawing.Size(300, 19);
            this.tsslTime.Text = "Running Time";
            // 
            // tspbMain
            // 
            this.tspbMain.CausesValidation = false;
            this.tspbMain.Name = "tspbMain";
            this.tspbMain.Size = new System.Drawing.Size(100, 18);
            // 
            // FrmContinuousGeneralizer
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1163, 716);
            this.Controls.Add(this.splitContainer);
            this.Controls.Add(this.axToolbarControl);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.ststMain);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Location = new System.Drawing.Point(400, 200);
            this.Name = "FrmContinuousGeneralizer";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "ContinuousGeneralizer";
            this.Load += new System.EventHandler(this.frmContinuousGeneralizer_Load);
            this.Shown += new System.EventHandler(this.FrmContinuousGeneralizer_Shown);
            this.tbcRight.ResumeLayout(false);
            this.tabPageMap.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axLicenseControl1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControl)).EndInit();
            this.splitContainer2.Panel1.ResumeLayout(false);
            this.splitContainer2.Panel1.PerformLayout();
            this.splitContainer2.Panel2.ResumeLayout(false);
            this.splitContainer2.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
            this.splitContainer2.ResumeLayout(false);
            this.tabPageLayout.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axPageLayoutControl)).EndInit();
            this.tabPageProperty.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axTOCControl)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.axMapControlEagleEye)).EndInit();
            this.tabPage1Layer.ResumeLayout(false);
            this.tbcLeft.ResumeLayout(false);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.splitContainer.Panel1.ResumeLayout(false);
            this.splitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer)).EndInit();
            this.splitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.axToolbarControl)).EndInit();
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ststMain.ResumeLayout(false);
            this.ststMain.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.ToolStripMenuItem mnuORB;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem mnuSimAngle3;
        private System.Windows.Forms.ToolStripMenuItem mnuSimDeflection;
        private System.Windows.Forms.ToolStripMenuItem mnuTF;
        private System.Windows.Forms.ToolStripMenuItem mnuLandingTimeLI;
        private System.Windows.Forms.ToolStripMenuItem mnuLandingTime;
        private System.Windows.Forms.ToolStripMenuItem mnuBezierDetectPointfarthest;
        private System.Windows.Forms.ToolStripMenuItem mnuBezierDetectPoint;
        private System.Windows.Forms.ToolStripMenuItem mnuCreatePointLayer;
        private System.Windows.Forms.ToolStripMenuItem mnuLookingForNeighboursDT;
        private System.Windows.Forms.ToolStripMenuItem mnuExcelToShape;
        private System.Windows.Forms.ToolStripMenuItem mnuEnlargeLayer;
        private System.Windows.Forms.ToolStripMenuItem mnuMatchAndMergePolylines;
        private System.Windows.Forms.ToolStripMenuItem mnuLookingForNeighboursSweepLine;
        private System.Windows.Forms.ToolStripMenuItem mnuLookingForNeighboursDT2;
        private System.Windows.Forms.ToolStripMenuItem mnuLookingForNeighboursGrids;
        private System.Windows.Forms.ToolStripMenuItem mnuCreateRandomPointLayer;
        private System.Windows.Forms.ToolStripMenuItem mnuCoordinatesTransformation;
        private System.Windows.Forms.ToolStripMenuItem mnuOrdinal;
        private System.Windows.Forms.ToolStripMenuItem mnuSimultaneity;
        private System.Windows.Forms.ToolStripMenuItem mnuApLALMulti;
        private System.Windows.Forms.ToolStripMenuItem mnuLinear_AL;
        private System.Windows.Forms.ToolStripMenuItem mnuPAL;
        private System.Windows.Forms.ToolStripMenuItem mnuGeneralization;
        private System.Windows.Forms.ToolStripMenuItem mnuDPSimplify;
        private System.Windows.Forms.ToolStripMenuItem morphingExtendToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuCDTLSA;
        private System.Windows.Forms.ToolStripMenuItem fishEyeToolStripMenuItem;
        private System.Windows.Forms.TabControl tbcRight;
        private System.Windows.Forms.TabPage tabPageMap;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.TextBox txtT;
        private System.Windows.Forms.TextBox txtVtPV;
        private System.Windows.Forms.TabPage tabPageLayout;
        private ESRI.ArcGIS.Controls.AxPageLayoutControl axPageLayoutControl;
        private System.Windows.Forms.PropertyGrid propertyGrid;
        private System.Windows.Forms.TabPage tabPageProperty;
        private ESRI.ArcGIS.Controls.AxTOCControl axTOCControl;
        private ESRI.ArcGIS.Controls.AxMapControl axMapControlEagleEye;
        private System.Windows.Forms.TabPage tabPage1Layer;
        private System.Windows.Forms.TabControl tbcLeft;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer;
        private ESRI.ArcGIS.Controls.AxToolbarControl axToolbarControl;
        private System.Windows.Forms.ToolStripMenuItem mnuAnyTest;
        private System.Windows.Forms.ToolStripMenuItem mnuALALMulti;
        private System.Windows.Forms.ToolStripMenuItem mnuALAMulti;
        private System.Windows.Forms.ToolStripMenuItem mnuCALMulti;
        private System.Windows.Forms.ToolStripMenuItem mnuOpen;
        private System.Windows.Forms.ToolStripMenuItem mnuNew;
        private System.Windows.Forms.ToolStripMenuItem mnuFile;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator;
        private System.Windows.Forms.ToolStripMenuItem mnuSave;
        private System.Windows.Forms.ToolStripMenuItem mnuSaveAs;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem mnuAddData;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem mnuExportViewToImage;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripMenuItem mnuExit;
        private System.Windows.Forms.MenuStrip menuStrip;
        private System.Windows.Forms.ToolStripMenuItem morphingToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuLinear;
        private System.Windows.Forms.ToolStripMenuItem mnuLinearMulti;
        private System.Windows.Forms.ToolStripMenuItem mnuVertexInsertion;
        private System.Windows.Forms.ToolStripMenuItem mnuMPBDP;
        private System.Windows.Forms.ToolStripMenuItem mnuMPBDPBL;
        private System.Windows.Forms.ToolStripMenuItem mnuMRL;
        private System.Windows.Forms.ToolStripMenuItem mnuMRLCut;
        private System.Windows.Forms.ToolStripMenuItem mnuOptCor;
        private System.Windows.Forms.ToolStripMenuItem mnuOptCorBezier;
        private System.Windows.Forms.ToolStripMenuItem mnuMPBBSL;
        private System.Windows.Forms.ToolStripMenuItem mnuMPBBSLDP;
        private System.Windows.Forms.ToolStripMenuItem mnuBLGOptCorMMSimplified;
        private System.Windows.Forms.ToolStripMenuItem mnuBSBLGOptCorMMSimplified;
        private System.Windows.Forms.ToolStripMenuItem mnuBSBLGOptCor;
        private System.Windows.Forms.ToolStripMenuItem mnuBSBLGOptCorMM;
        private System.Windows.Forms.ToolStripMenuItem mnuRIBS;
        private System.Windows.Forms.ToolStripMenuItem mnuRIBSBLG;
        private System.Windows.Forms.ToolStripMenuItem mnuRIBSBLGOptCor;
        private System.Windows.Forms.ToolStripMenuItem mnuAtBdMorphing;
        private System.Windows.Forms.ToolStripMenuItem MorphingLSAToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuStraightLine;
        private System.Windows.Forms.ToolStripMenuItem mnuAL_AL;
        private System.Windows.Forms.ToolStripMenuItem mnuAL;
        private System.Windows.Forms.ToolStripMenuItem mnuALL;
        private System.Windows.Forms.ToolStripMenuItem mnuApLL;
        private System.Windows.Forms.ToolStripMenuItem mnuALm;
        private System.Windows.Forms.ToolStripMenuItem mnuLMulti;
        private System.Windows.Forms.ToolStripMenuItem mnuHelp;
        private System.Windows.Forms.ToolStripStatusLabel CoordinateLabel;
        private System.Windows.Forms.ToolStripStatusLabel Blank;
        private System.Windows.Forms.ToolStripStatusLabel tsslMessage;
        private System.Windows.Forms.StatusStrip ststMain;
        private System.Windows.Forms.ToolStripStatusLabel tsslTime;
        private System.Windows.Forms.ToolStripProgressBar tspbMain;
        private ESRI.ArcGIS.Controls.AxLicenseControl axLicenseControl1;
        private System.Windows.Forms.ToolStripMenuItem mnuGaussianPerturbation;
        private System.Windows.Forms.ToolStripMenuItem mnuDeletePointsWithSameCoordinates;
        private System.Windows.Forms.ToolStripMenuItem roadnetworkToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuClassification;
        private System.Windows.Forms.ToolStripMenuItem mnuRightAngelDPS;
        private System.Windows.Forms.ToolStripMenuItem mnuLinearMorphing;
        private System.Windows.Forms.ToolStripMenuItem mnuTransparencyMorphing;
        private System.Windows.Forms.ToolStripMenuItem mnuUnifyDirections;
        private System.Windows.Forms.ToolStripMenuItem toIpeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuCGABM;
        private System.Windows.Forms.ToolStripMenuItem mnuTopologyChecker;
        private System.Windows.Forms.ToolStripMenuItem mnuStatisticsOfDataSets;
        private System.Windows.Forms.ToolStripMenuItem mnuTest;
        private System.Windows.Forms.ToolStripMenuItem mnuTestDistanceMetric;
        private System.Windows.Forms.ToolStripMenuItem mnuMatchAndMergePolygons;
        private System.Windows.Forms.ToolStripMenuItem mnuMergeAndSplitPolylines;
        private System.Windows.Forms.ToolStripMenuItem mnuUnifyIndicesPolylines;
        private System.Windows.Forms.ToolStripMenuItem mnuTemporary;
        private System.Windows.Forms.ToolStripMenuItem mnuIdentifyCorrCpgAddRegionNum;
        private System.Windows.Forms.ToolStripMenuItem mnuAreaAgg_AStar;
        private System.Windows.Forms.ToolStripMenuItem mnuCalGeo_Ipe;
        private System.Windows.Forms.ToolStripMenuItem mnuBldgGrow;
        private System.Windows.Forms.ToolStripMenuItem mnuCompareExcel;
        private System.Windows.Forms.ToolStripMenuItem otherToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuKillExcel;
        private System.Windows.Forms.ToolStripMenuItem mnuClear;
        private System.Windows.Forms.ToolStripMenuItem kARSToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem mnuExtractPossibleFiles;
        private ESRI.ArcGIS.Controls.AxMapControl axMapControl;
        private System.Windows.Forms.ToolStripMenuItem mnuSelectRandomly;
    }
}

