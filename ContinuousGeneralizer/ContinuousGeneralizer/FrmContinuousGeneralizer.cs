using System;
using System.Collections.Generic;
using SCG = System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geoprocessor;

using MorphingClass;
using MorphingClass.CCommon;
using MorphingClass.CGeometry;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using ContinuousGeneralizer.FrmEvaluate;
using ContinuousGeneralizer.FrmAid;
using ContinuousGeneralizer.FrmMorphing;
using ContinuousGeneralizer.FrmGeneralization;
using ContinuousGeneralizer.FrmMorphingLSA;
using ContinuousGeneralizer.FrmFishEye;
using ContinuousGeneralizer.FrmMorphingExtend;
using ContinuousGeneralizer.FrmGISCommand;
using ContinuousGeneralizer.FrmSimilarity;
using ContinuousGeneralizer.RoadNetwork;
using ContinuousGeneralizer.FrmKARS;

using VBClass;

using ILOG.Concert;
using ILOG.CPLEX;

namespace ContinuousGeneralizer
{
    public partial class FrmContinuousGeneralizer : Form
    {
        private void FrmContinuousGeneralizer_Shown(object sender, EventArgs e)
        {
            //Geoprocessor gp = null;

            //try
            //{
            //    gp = new Geoprocessor();
            //}
            //catch (System.Exception ex)
            //{

            //    throw;
            //}

            //var frmCurrent = new FrmSelectRandomly(_DataRecords);
            //var frmCurrent = new FrmToIpe(_DataRecords);
            var frmCurrent = new FrmBldgGrow(_DataRecords);
            //var frmCurrent = new FrmAreaAgg(_DataRecords);
            //var frmCurrent = new FrmCGABM(_DataRecords);
            //var frmCurrent = new FrmExtractPossibleFiles();
            frmCurrent.Show();
            frmCurrent.btnRun_Click(sender, e);
            //frmCurrent.btn020_Click(sender, e);
            //frmCurrent.btnMultiResults_Click(sender, e);

            //frmCurrent.btn090_Click(sender, e);
        }

        private object _Missing = Type.Missing;
        private CDataRecords _DataRecords = new CDataRecords();
        private ESRI.ArcGIS.Controls.IMapControl4 m_mapControl = null;
        private ESRI.ArcGIS.Controls.IPageLayoutControl2 m_pageLayoutControl = null;
        private MorphingClass.CCommon.ControlsSynchronizer m_controlsSynchronizer = null;
        
        

        //private IMapDocument pMapDocument;

        //菜单相关
        //TOCControl控件变量
        private ITOCControl2 m_tocControl = null;
        //TOCControl中Map菜单
        private IToolbarMenu m_menuMap = null;
        //TOCControl中图层菜单
        private IToolbarMenu m_menuLayer = null;

        //当前X坐标
        private double m_dblX = 0;
        //当前Y坐标
        private double m_dblY = 0;


        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmContinuousGeneralizer()
        {
            InitializeComponent();

            //鼠标滚动代码
            this.MouseWheel += new MouseEventHandler(this.axMapControl_MouseWheel);
        }


        private void frmContinuousGeneralizer_Load(object sender, EventArgs e)
        {
            //取得 MapControl 和 PageLayoutControl 的引用
            m_mapControl = (IMapControl4)this.axMapControl.Object;
            m_pageLayoutControl = (IPageLayoutControl2)this.axPageLayoutControl.Object;
            //取得 TOCControl 的引用
            m_tocControl = (ITOCControl2)this.axTOCControl.Object;
            //初始化controls synchronization calss
            m_controlsSynchronizer = new MorphingClass.CCommon.ControlsSynchronizer(m_mapControl, m_pageLayoutControl);
            //把MapControl和PageLayoutControl绑定起来(两个都指向同一个Map),然后设置MapControl为活动的Control
            m_controlsSynchronizer.BindControls(true);
            //为了在切换MapControl和PageLayoutControl视图同步，要添加Framework Control
            m_controlsSynchronizer.AddFrameworkControl(axToolbarControl.Object);
            m_controlsSynchronizer.AddFrameworkControl(this.axTOCControl.Object);
            // 添加打开命令按钮到工具条
            MorphingClass.CCommon.OpenNewMapDocument openMapDoc = new MorphingClass.CCommon.OpenNewMapDocument(m_controlsSynchronizer);
            axToolbarControl.AddItem(openMapDoc, -1, 0, false, -1, esriCommandStyles.esriCommandStyleIconOnly);


            //菜单的创建
            m_menuMap = new ToolbarMenuClass();
            m_menuLayer = new ToolbarMenuClass();
            //添加自定义菜单项到TOCCOntrol的Map菜单中
            //打开文档菜单
            m_menuMap.AddItem(new MorphingClass.CCommon.OpenNewMapDocument(m_controlsSynchronizer), -1, 0, false, esriCommandStyles.esriCommandStyleIconAndText);
            //添加数据菜单
            m_menuMap.AddItem(new ControlsAddDataCommandClass(), -1, 1, false, esriCommandStyles.esriCommandStyleIconAndText);
            //打开全部图层菜单
            m_menuMap.AddItem(new MorphingClass.CCommon.LayerVisibility(), 1, 2, true, esriCommandStyles.esriCommandStyleTextOnly);
            //关闭全部图层菜单
            m_menuMap.AddItem(new MorphingClass.CCommon.LayerVisibility(), 2, 3, false, esriCommandStyles.esriCommandStyleTextOnly);
            //以二级菜单的形式添加内置的“选择”菜单
            m_menuMap.AddSubMenu("esriControls.ControlsFeatureSelectionMenu", 4, true);
            //以二级菜单的形式添加内置的“地图浏览”菜单
            m_menuMap.AddSubMenu("esriControls.ControlsMapViewMenu", 5, true);


            //添加自定义菜单项到TOCCOntrol的图层菜单中
            m_menuLayer = new ToolbarMenuClass();
            //添加“移除图层”菜单项
            m_menuLayer.AddItem(new MorphingClass.CCommon.RemoveLayer(), -1, 0, false, esriCommandStyles.esriCommandStyleTextOnly);
            //添加“放大到整个图层”菜单项
            m_menuLayer.AddItem(new MorphingClass.CCommon.ZoomToLayer(), -1, 1, true, esriCommandStyles.esriCommandStyleTextOnly);
            //设置菜单的Hook
            m_menuLayer.SetHook(m_mapControl);
            m_menuMap.SetHook(m_mapControl);



            CDataRecords pDataRecords = new CDataRecords();
            //pDataRecords.ParameterInitialize.pAxMapControl = this.axMapControl;
            pDataRecords.ParameterInitialize.m_mapControl = m_mapControl;
            pDataRecords.ParameterInitialize.ststMain = this.ststMain;
            pDataRecords.ParameterInitialize.tsslTime = this.tsslTime;
            pDataRecords.ParameterInitialize.tsslMessage = this.tsslMessage;
            pDataRecords.ParameterInitialize.tspbMain = this.tspbMain;
            pDataRecords.ParameterInitialize.txtVtPV = this.txtVtPV;
            pDataRecords.ParameterInitialize.txtT = this.txtT;
            _DataRecords = pDataRecords;



            openMapDoc.OnCreate(m_controlsSynchronizer.MapControl.Object);
            openMapDoc.OnClickAutomatic(CHelpFunc.strPath + CHelpFunc.strDataFolderName + ".mxd");




        }



        #region 基本控制命令


        /// <summary>
        /// 新建地图命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuNew_Click(object sender, EventArgs e)
        {
            // 本命令涉及到 MapControl 和 PageLayoutControl 同步问题
            //询问是否保存当前地图
            DialogResult res = MessageBox.Show("是否保存当前地图?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (res == DialogResult.Yes)
            {
                //如果要保存，调用另存为对话框
                ICommand command = new ControlsSaveAsDocCommandClass();
                if (m_mapControl != null)
                    command.OnCreate(m_controlsSynchronizer.MapControl.Object);
                else
                    command.OnCreate(m_controlsSynchronizer.PageLayoutControl.Object);
                command.OnClick();
            }
            //创建新的地图实例
            IMap map = new MapClass();
            map.Name = "Map";
            m_controlsSynchronizer.MapControl.DocumentFilename = string.Empty;
            //更新新建地图实例的共享地图文档
            m_controlsSynchronizer.ReplaceMap(map);
        }

        /// <summary>
        /// 打开地图文档 Mxd 命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuOpen_Click(object sender, EventArgs e)
        {
            // 本命令涉及到 MapControl 和 PageLayoutControl 同步问题
            if (this.axMapControl.LayerCount > 0)
            {
                DialogResult result = MessageBox.Show("是否保存当前地图？", "警告", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
                if (result == DialogResult.Cancel) return;
                if (result == DialogResult.Yes) this.mnuSave_Click(null, null);
            }
            MorphingClass.CCommon.OpenNewMapDocument openMapDoc = new MorphingClass.CCommon.OpenNewMapDocument(m_controlsSynchronizer);
            openMapDoc.OnCreate(m_controlsSynchronizer.MapControl.Object);
            openMapDoc.OnClick();


        }

        /// <summary>
        /// 保存地图文档命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuSave_Click(object sender, EventArgs e)
        {
            // 首先确认当前地图文档是否有效
            if (null != m_pageLayoutControl.DocumentFilename && m_mapControl.CheckMxFile(m_pageLayoutControl.DocumentFilename))
            {
                // 创建一个新的地图文档实例
                IMapDocument mapDoc = new MapDocumentClass();
                // 打开当前地图文档
                mapDoc.Open(m_pageLayoutControl.DocumentFilename, string.Empty);
                // 用 PageLayout 中的文档替换当前文档中的 PageLayout 部分
                mapDoc.ReplaceContents((IMxdContents)m_pageLayoutControl.PageLayout);
                // 保存地图文档
                mapDoc.Save(mapDoc.UsesRelativePaths, false);
                mapDoc.Close();
            }
        }
        /// <summary>
        /// 另存为地图文档命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuSaveAs_Click(object sender, EventArgs e)
        {
            //在另存为地图文档时，有可能会丢失数据，因此我们需要提示用户以确认操作，

            //如果当前视图为MapControl时，提示用户另存为操作将丢失PageLayoutControl中的设置
            if (m_controlsSynchronizer.ActiveControl is IMapControl4)
            {
                if (MessageBox.Show("另存为地图文档将丢失制版视图的设置\r\n您要继续吗?", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                    return;
            }
            //调用另存为命令
            ICommand command = new ControlsSaveAsDocCommandClass();
            command.OnCreate(m_controlsSynchronizer.ActiveControl);
            command.OnClick();
        }

        /// <summary>
        /// 添加数据命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuAddData_Click(object sender, EventArgs e)
        {
            int currentLayerCount = this.axMapControl.LayerCount;
            ICommand pCommand = new ControlsAddDataCommandClass();
            pCommand.OnCreate(this.axMapControl.Object);
            pCommand.OnClick();
            //在添加数据AddData时，我们也要进行地图共享，故在AddData_Click函数后面添加如下代码：
            IMap pMap = this.axMapControl.Map;
            this.m_controlsSynchronizer.ReplaceMap(pMap);
        }

        /// <summary>
        /// 退出程序
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuExit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //显示当前所用工具信息


        //显示当前比例尺及当前坐标
        private void axMapControl_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            double dblX = Math.Round(e.mapX, 6);
            double dblY = Math.Round(e.mapY, 6);

            //显示当前坐标
            CoordinateLabel.Text = " Coordinate:  X = " + dblX.ToString() + "  Y = " + dblY.ToString() + "  " + this.axMapControl.MapUnits.ToString();
            //CoordinateLabel.Text = " 当前坐标 X = " + e.mapX.ToString() + " Y = " + e.mapY.ToString() + " " + this.axMapControl1.MapUnits.ToString().Substring(4);

            //对全局变量进行赋值，以用在其它地方
            m_dblX = dblX;
            m_dblY = dblY;
        }

        /// <summary>
        /// 地图控件上的滚轮事件（向前：放大；向后：缩小）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e">鼠标在窗体"FrmContinuousGeneralizer"上的像素坐标（不包括上层边缘。。。）</param>
        public void axMapControl_MouseWheel(object sender, MouseEventArgs e)
        {
            System.Drawing.Point MouseScreenPoint = this.PointToScreen(e.Location);                   //将鼠标在窗体"FrmContinuousGeneralizer"上的像素坐标转换为屏幕坐标
            System.Drawing.Point axMapControlScreenPoint = axMapControl.PointToScreen(axMapControl.Location);   //将控件“axMapControl”左上角的坐标转换为屏幕坐标

            //放到屏幕坐标系下面进行比较，看判断鼠标是否在空间“axMapControl”中
            if (((MouseScreenPoint.X) < axMapControlScreenPoint.X) || (MouseScreenPoint.Y) < axMapControlScreenPoint.Y ||
                ((MouseScreenPoint.X) > (axMapControlScreenPoint.X + axMapControl.Width)) || ((MouseScreenPoint.Y) > (axMapControlScreenPoint.Y + axMapControl.Height)))
            {
                return;
            }

            IEnvelope pEnvelope = axMapControl.Extent.Envelope;
            IEnvelope pNewEnvelope = new EnvelopeClass();

            int intEDelta = e.Delta;
            double dblScale = 0.8;
            if (intEDelta > 0)
            {
                //计算新的Envelope
                double dblNewWidth = pEnvelope.Width * dblScale;
                double dblNewXMin = m_dblX - ((m_dblX - pEnvelope.XMin) / pEnvelope.Width) * dblNewWidth;
                double dblNewXMax = dblNewXMin + dblNewWidth;

                double dblNewHeight = dblScale * pEnvelope.Height;
                double dblNewYMin = m_dblY - ((m_dblY - pEnvelope.YMin) / pEnvelope.Height) * dblNewHeight;
                double dblNewYMax = dblNewYMin + dblNewHeight;

                pNewEnvelope.PutCoords(dblNewXMin, dblNewYMin, dblNewXMax, dblNewYMax);
                axMapControl.Extent = pNewEnvelope;
            }
            //滚轮向后，zoom out
            else if (intEDelta < 0)
            {
                //计算新的Envelope
                double dblNewWidth = pEnvelope.Width / dblScale;
                double dblNewXMin = m_dblX - ((m_dblX - pEnvelope.XMin) / pEnvelope.Width) * dblNewWidth;
                double dblNewXMax = dblNewXMin + dblNewWidth;

                double dblNewHeight = pEnvelope.Height / dblScale;
                double dblNewYMin = m_dblY - ((m_dblY - pEnvelope.YMin) / pEnvelope.Height) * dblNewHeight;
                double dblNewYMax = dblNewYMin + dblNewHeight;

                pNewEnvelope.PutCoords(dblNewXMin, dblNewYMin, dblNewXMax, dblNewYMax);
                axMapControl.Extent = pNewEnvelope;
            }
        }

        //载入地图到鹰眼控件
        //当地图载入到主 Map 控件时，同时也载入到鹰眼控件
        private void axMapControl_OnMapReplaced(object sender, IMapControlEvents2_OnMapReplacedEvent e)
        {
            // 当主地图显示控件的地图更换时，鹰眼中的地图也跟随更换
            this.axMapControlEagleEye.Map = new MapClass();
            // 添加主地图控件中的所有图层到鹰眼控件中
            for (int i = 1; i <= this.axMapControl.LayerCount; i++)
            {
                this.axMapControlEagleEye.AddLayer(this.axMapControl.get_Layer(this.axMapControl.LayerCount - i));
            }
            // 设置 MapControl 显示范围至数据的全局范围
            this.axMapControlEagleEye.Extent = this.axMapControl.Extent;
            // 刷新鹰眼控件地图
            this.axMapControlEagleEye.Refresh();
        }

        //绘制鹰眼矩形框
        //为鹰眼控件 MapControl 添加 OnExtentUpdated 事件，此事件是在主 Map 控件的显示范围改变时响应，从而相应更新鹰眼控件中的矩形框
        private void axMapControl_OnExtentUpdated(object sender, IMapControlEvents2_OnExtentUpdatedEvent e)
        {
            // 得到新范围
            IEnvelope pEnv = (IEnvelope)e.newEnvelope;
            IGraphicsContainer pGra = axMapControlEagleEye.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            // 在绘制前，清除 axMapControlEagleEye 中的任何图形元素
            pGra.DeleteAllElements();
            IRectangleElement pRectangleEle = new RectangleElementClass();
            IElement pEle = pRectangleEle as IElement;
            pEle.Geometry = pEnv;
            // 设置鹰眼图中的红线框
            IRgbColor pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 255;
            // 产生一个线符号对象
            ILineSymbol pOutline = new SimpleLineSymbolClass();
            pOutline.Width = 2;
            pOutline.Color = pColor;
            // 设置颜色属性
            pColor = new RgbColorClass();
            pColor.Red = 255;
            pColor.Green = 0;
            pColor.Blue = 0;
            pColor.Transparency = 0;
            // 设置填充符号的属性
            IFillSymbol pFillSymbol = new SimpleFillSymbolClass();
            pFillSymbol.Color = pColor;
            pFillSymbol.Outline = pOutline;
            IFillShapeElement pFillShapeEle = pEle as IFillShapeElement;
            pFillShapeEle.Symbol = pFillSymbol;
            pGra.AddElement((IElement)pFillShapeEle, 0);
            // 刷新
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }

        private void axMapControl_OnAfterScreenDraw(object sender, IMapControlEvents2_OnAfterScreenDrawEvent e)
        {
            axMapControlEagleEye.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        //鹰眼与主 Map 控件互动
        //为鹰眼控件 axMapControlEagleEye 添加 OnMouseDown 事件
        private void axMapControlEagleEye_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (this.axMapControlEagleEye.Map.LayerCount != 0)
            {
                // 按下鼠标左键移动矩形框
                if (e.button == 1)
                {
                    IPoint pPoint = new PointClass();
                    pPoint.PutCoords(e.mapX, e.mapY);
                    IEnvelope pEnvelope = this.axMapControl.Extent;
                    pEnvelope.CenterAt(pPoint);
                    this.axMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
                // 按下鼠标右键绘制矩形框
                else if (e.button == 2)
                {
                    IEnvelope pEnvelop = this.axMapControlEagleEye.TrackRectangle();
                    this.axMapControl.Extent = pEnvelop;
                    this.axMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
                }
            }
        }

        //为鹰眼控件 MapControlEagleEye 添加 OnMouseMove 事件，主要实现按下鼠标左键的时候移动矩形框，同时也改变主的图控件的显示范围
        private void axMapControlEagleEye_OnMouseMove(object sender, IMapControlEvents2_OnMouseMoveEvent e)
        {
            // 如果不是左键按下就直接返回
            if (e.button != 1) return;
            IPoint pPoint = new PointClass();
            pPoint.PutCoords(e.mapX, e.mapY);
            this.axMapControl.CenterAt(pPoint);
            this.axMapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
        }

        //顾名思义，右键菜单是在鼠标右键按下的时候弹出，所以我们要添加TOCControl控件的OnMouseDown事件
        private void axTOCControl_OnMouseDown(object sender, ITOCControlEvents_OnMouseDownEvent e)
        {
            //如果不是右键按下直接返回
            if (e.button != 2) return;
            esriTOCControlItem item = esriTOCControlItem.esriTOCControlItemNone;
            IBasicMap map = null;
            ILayer layer = null;
            object other = null;
            object index = null;
            //判断所选菜单的类型
            m_tocControl.HitTest(e.x, e.y, ref item, ref map, ref layer, ref other, ref index);
            //确定选定的菜单类型，Map或是图层菜单
            if (item == esriTOCControlItem.esriTOCControlItemMap)
                m_tocControl.SelectItem(map, null);
            else
                m_tocControl.SelectItem(layer, null);
            //设置CustomProperty为layer (用于自定义的Layer命令)
            m_mapControl.CustomProperty = layer;
            //弹出右键菜单
            if (item == esriTOCControlItem.esriTOCControlItemMap)
                m_menuMap.PopupMenu(e.x, e.y, m_tocControl.hWnd);
            if (item == esriTOCControlItem.esriTOCControlItemLayer)
            {
                //动态添加OpenAttributeTable菜单项
                m_menuLayer.AddItem(new ContinuousGeneralizer.CCommon.OpenAttributeTable(ref m_mapControl, layer), -1, 2, true, esriCommandStyles.esriCommandStyleTextOnly);
                m_menuLayer.PopupMenu(e.x, e.y, m_tocControl.hWnd);
                //移除OpenAttributeTable菜单项，以防止重复添加
                m_menuLayer.Remove(2);
            }
        }

        /// <summary>
        /// 主地图控件的右键响应函数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        /// <remarks>我们也可以实现主地图控件的右键菜单，以方便地图浏览。添加MapControl1控件的OnMouseDown事件</remarks>
        private void axMapControl_OnMouseDown(object sender, IMapControlEvents2_OnMouseDownEvent e)
        {
            if (e.button == 2)
            {
                //弹出右键菜单
                m_menuMap.PopupMenu(e.x, e.y, m_mapControl.hWnd);
            }
        }

        //图层符号选择器的实现
        /// <summary>
        /// 双击TOCControl控件时触发的事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void axTOCControl_OnDoubleClick(object sender, ITOCControlEvents_OnDoubleClickEvent e)
        {

            #region 直接调用ArcMap中的符号选择器
            esriTOCControlItem toccItem = esriTOCControlItem.esriTOCControlItemNone;
            ILayer iLayer = null;
            IBasicMap iBasicMap = null;
            object unk = null;
            object data = null;
            if (e.button == 1)
            {
                axTOCControl.HitTest(e.x, e.y, ref toccItem, ref iBasicMap, ref iLayer, ref unk, ref data);
                System.Drawing.Point pos = new System.Drawing.Point(e.x, e.y);
                if (toccItem == esriTOCControlItem.esriTOCControlItemLegendClass)
                {
                    ESRI.ArcGIS.Carto.ILegendClass pLC = new LegendClassClass();
                    ESRI.ArcGIS.Carto.ILegendGroup pLG = new LegendGroupClass();
                    if (unk is ILegendGroup)
                    {
                        pLG = (ILegendGroup)unk;
                    }
                    pLC = pLG.get_Class((int)data);
                    ISymbol pSym;
                    pSym = pLC.Symbol;
                    ESRI.ArcGIS.DisplayUI.ISymbolSelector pSS = new ESRI.ArcGIS.DisplayUI.SymbolSelectorClass();
                    bool bOK = false;
                    pSS.AddSymbol(pSym);
                    bOK = pSS.SelectSymbol(0);
                    if (bOK)
                    {
                        pLC.Symbol = pSS.GetSymbolAt(0);
                    }
                    this.axMapControl.ActiveView.Refresh();
                    this.axTOCControl.Refresh();
                }
            }
            #endregion

            //esriTOCControlItem itemType = esriTOCControlItem.esriTOCControlItemNone;
            //IBasicMap basicMap = null;
            //ILayer layer = null;
            //object unk = null;
            //object data = null;
            //axTOCControl.HitTest(e.x, e.y, ref itemType, ref basicMap, ref layer, ref unk, ref data);
            //if (e.button == 1)
            //{
            //    if (itemType == esriTOCControlItem.esriTOCControlItemLegendClass)
            //    {
            //        //取得图例
            //        ILegendClass pLegendClass = ((ILegendGroup)unk).get_Class((int)data);
            //        //创建符号选择器SymbolSelector实例
            //        ContinuousGeneralizer.FrmGISCommand.FrmSymbolSelector SymbolSelectorFrm = new ContinuousGeneralizer.FrmGISCommand.FrmSymbolSelector(pLegendClass, layer);
            //        if (SymbolSelectorFrm.ShowDialog() == DialogResult.OK)
            //        {
            //            //局部更新主Map控件
            //            m_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            //            //设置新的符号
            //            pLegendClass.Symbol = SymbolSelectorFrm.pSymbol;
            //            //更新主Map控件和图层控件
            //            this.axMapControl.ActiveView.Refresh();
            //            this.axTOCControl.Refresh();
            //        }
            //    }
            //}
        }


        /// <summary>
        /// 将当前视图导出为JPG格式文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void mnuExportViewToImage_Click(object sender, EventArgs e)
        {
            //确定导出路径
            SaveFileDialog sfdjpg = new SaveFileDialog();
            //sfdjpg.Filter = "jpg files (*.jpg)|*.jpg|All files (*.*)|*.*";
            if (sfdjpg.ShowDialog() == DialogResult.Cancel)
            {
                return;
            }
            string strFileName;
            strFileName = sfdjpg.FileName;

            MorphingClass.CCommon.ExportActiveViewCS_Net pExportActiveViewCS_Net = new MorphingClass.CCommon.ExportActiveViewCS_Net();

            pExportActiveViewCS_Net.ExportActiveViewParameterized(this.axMapControl.ActiveView, 300, 1, "JPEG", strFileName, false);

        }

        //清除绘画痕迹
        private void mnuClear_Click(object sender, EventArgs e)
        {

        }

        private void mnuKillExcel_Click(object sender, EventArgs e)
        {
            CHelpFuncExcel.KillExcel();
        }

        private double ComputeWeight(double dblArea1, double dblPrmt1, double dblArea2, double dblPrmt2, double dblSumPrmt, double dblTotalArea)
        {
            double dblSumArea = dblArea1 + dblArea2;
            double dblWeight = (dblArea1 * Math.Abs(Math.Sqrt(dblArea1) / dblPrmt1 - Math.Sqrt(dblSumArea) / dblSumPrmt)
                             + (dblArea2 * Math.Abs(Math.Sqrt(dblArea2) / dblPrmt2 - Math.Sqrt(dblSumArea) / dblSumPrmt)))
                             / dblTotalArea;

            Console.WriteLine("w= " + dblWeight);
            return dblWeight;



        }

        private double ComputeFu(double dblArea1, double dblPrmt1, double dblArea2, double dblPrmt2, double dblArea3, double dblPrmt3, double dblSumPrmt, double dblTotalArea)
        {
            double dblSumArea = dblArea1 + dblArea2 + dblArea3;
            double dblWeight = (dblArea1 * Math.Abs(Math.Sqrt(dblArea1) / dblPrmt1 - Math.Sqrt(dblSumArea) / dblSumPrmt)
                              + dblArea2 * Math.Abs(Math.Sqrt(dblArea2) / dblPrmt2 - Math.Sqrt(dblSumArea) / dblSumPrmt)
                              + dblArea3 * Math.Abs(Math.Sqrt(dblArea3) / dblPrmt3 - Math.Sqrt(dblSumArea) / dblSumPrmt))
                             / dblTotalArea;

            double dbl1 = Math.Sqrt(dblArea1) / dblPrmt1;
            double dbl2 = Math.Sqrt(dblArea2) / dblPrmt2;
            double dbl3 = Math.Sqrt(dblArea3) / dblPrmt3;
            double dbl4 = Math.Sqrt(dblSumArea) / dblSumPrmt;

            double dbl5 = Math.Sqrt(dblArea1) / dblPrmt1 - Math.Sqrt(dblSumArea) / dblSumPrmt;
            double dbl6 = Math.Sqrt(dblArea2) / dblPrmt2 - Math.Sqrt(dblSumArea) / dblSumPrmt;
            double dbl7 = Math.Sqrt(dblArea3) / dblPrmt3 - Math.Sqrt(dblSumArea) / dblSumPrmt;

            Console.WriteLine("f(u)= " + dblWeight);
            return dblWeight;



        }


        private void mnuAnyTest_Click(object sender, EventArgs e)
        {
            int intNum = 999;
            var intltltltltlt = new List<List<List<List<List<int>>>>>(intNum);
            for (int i = 0; i < intNum; i++)
            {
                var intltltltlt = new List<List<List<List<int>>>>(intNum);
                for (int j = 0; j < intNum; j++)
                {
                    var intltltlt= new List<List<List<int>>>(intNum);
                    for (int k = 0; k < intNum; k++)
                    {
                        var intltlt = new List<List<int>>(intNum);
                        for (int l = 0; l < intNum; l++)
                        {
                            var intlt = new List<int>(intNum);
                            for (int m = 0; m < intNum; m++)
                            {
                                intlt.Add(m);
                            }
                            intltlt.Add(intlt);
                        }
                        intltltlt.Add(intltlt);
                    }
                    intltltltlt.Add(intltltlt);
                }
                intltltltltlt.Add(intltltltlt);
            }


            tspbMain.Value = 15;

            //var cptlt= CHelpFunc .GetTestCptEb().ToList();

            //CPolygon cpg = new CPolygon(1, cptlt);
            //cpg.CptLtLt.Add(cptlt);
            //cpg.CptLtLt.Add(cptlt);






            //CHelpFunc.testmemory();

            ////IList<IList<double>> intltlt = new List<IList<double>>();

            ////while (intltlt.Count < 40)
            ////{
            ////    intltlt.Add(testttt());
            ////}


            ////while (true)
            ////{
            ////    intltlt.Add(testttt());
            ////}

            ////int st = 5;        
        }

        private IList<double> testttt()
        {
            int intLimit = 8000000;
            IList<double> intlt = new List<double>(intLimit);
            while (intlt.Count < intLimit)
            {
                intlt.Add(3.0);
            }
            return intlt;
        }

        private IEnumerable<int> Test()
        {

            for (int i = 0; i < 10; i++)
            {
                Console.WriteLine("do stupid!");
                yield return i;
            }

        }
        //public static int CountT<T>(ICollection<T> pTCol, int intLevel = 1)
        //{
        //    int intCount = 0;
        //    if (intLevel > 0)
        //    {
        //        intLevel--;
        //        foreach (T pT in pTCol)
        //        {
        //            ICollection<T> subTCol = pT as ICollection<T>;   //PROBLEM: I get "null" for sub subTCol
        //            intCount += CountT(subTCol, intLevel);
        //        }
        //    }
        //    else if (intLevel == 0)
        //    {
        //        intCount = pTCol.Count;
        //    }
        //    return intCount;
        //}

        //public static int CountT2<T, T2>(ICollection<T> pTCol, int intLevel = 1)
        //{
        //    int intCount = 0;
        //    if (intLevel > 0)
        //    {
        //        intLevel--;
        //        foreach (T pT in pTCol)
        //        {
        //            ICollection<T2> subTCol = pT as ICollection<T2>;
        //            intCount += CountT2(subTCol, intLevel);   //PROBLEM: The type arguments for method cannot be inferred from the usage
        //        }
        //    }
        //    else if (intLevel == 0)
        //    {
        //        intCount = pTCol.Count;
        //    }
        //    return intCount;
        //}












        private void test()
        {
            long lngMemory6 = GC.GetTotalMemory(true);
            List<IPoint> cptlt3 = new List<IPoint>(1000000);
            for (int i = 0; i < 1000000; i++)
            {
                IPoint ipt = new PointClass();
                ipt.PutCoords(i, i);
                ipt.ID = i;
                cptlt3.Add(ipt);
            }
            long lngMemory7 = GC.GetTotalMemory(true);
        }
        #endregion


        #region 调用各窗体的命令
        private void mnuLinear_Click(object sender, EventArgs e)
        {
            FrmLinear frmLinear = new FrmLinear(_DataRecords);
            frmLinear.Show();
        }

        private void mnuLinearMulti_Click(object sender, EventArgs e)
        {
            FrmLinearMulti frmLinearMulti = new FrmLinearMulti(_DataRecords);
            frmLinearMulti.Show();
        }

        private void mnuCreatePointLayer_Click(object sender, EventArgs e)
        {
            FrmCreatePointLayer frmCreatePointLayer = new FrmCreatePointLayer(_DataRecords);
            frmCreatePointLayer.Show();
        }

        private void mnuVertexInsertion_Click(object sender, EventArgs e)
        {
            FrmVertexInsertion frmVertexInsertion = new FrmVertexInsertion(_DataRecords);
            frmVertexInsertion.Show();
        }

        private void mnuMPBDP_Click(object sender, EventArgs e)
        {
            FrmMPBDP frmMPBDP = new FrmMPBDP(_DataRecords);
            frmMPBDP.Show();
        }

        private void mnuMPBDPBL_Click(object sender, EventArgs e)
        {
            FrmMPBDPBL frmMPBDPBL = new FrmMPBDPBL(_DataRecords);
            frmMPBDPBL.Show();
        }

        private void mnuBezierDetectPoint_Click(object sender, EventArgs e)
        {
            FrmBezierDetectPoint frmBezierDetectPoint = new FrmBezierDetectPoint(_DataRecords);
            frmBezierDetectPoint.Show();
        }

        private void mnuBezierDetectPointfarthest_Click(object sender, EventArgs e)
        {
            FrmBezierDetectPointfarthest frmBezierDetectPointfarthest = new FrmBezierDetectPointfarthest(_DataRecords);
            frmBezierDetectPointfarthest.Show();
        }

        private void mnuORB_Click(object sender, EventArgs e)
        {
            FrmSimAngles frmORB = new FrmSimAngles(_DataRecords);
            frmORB.Show();
        }


        private void mnuMRL_Click(object sender, EventArgs e)
        {
            FrmMRL frmMRL = new FrmMRL(_DataRecords);
            frmMRL.Show();
        }

        private void mnuMRLCut_Click(object sender, EventArgs e)
        {
            FrmMRLCut frmMRLCut = new FrmMRLCut(_DataRecords);
            frmMRLCut.Show();
        }

        private void mnuOptCor_Click(object sender, EventArgs e)
        {
            FrmOptCor frmOptCor = new FrmOptCor(_DataRecords);
            frmOptCor.Show();
        }

        private void mnuOptCorBezier_Click(object sender, EventArgs e)
        {
            FrmOptCorBezier frmOptCorBezier = new FrmOptCorBezier(_DataRecords);
            frmOptCorBezier.Show();
        }

        private void mnuMPBBSL_Click(object sender, EventArgs e)
        {
            FrmMPBBSL frmMPBBSL = new FrmMPBBSL(_DataRecords);
            frmMPBBSL.Show();
        }

        private void mnuMPBBSLDP_Click(object sender, EventArgs e)
        {
            FrmMPBBSLDP frmMPBBSLDP = new FrmMPBBSLDP(_DataRecords);
            frmMPBBSLDP.Show();
        }

        private void mnuBLGOptCorMMSimplified_Click(object sender, EventArgs e)
        {
            FrmBLGOptCorMMSimplified frmBLGOptCorMMSimplified = new FrmBLGOptCorMMSimplified(_DataRecords);
            frmBLGOptCorMMSimplified.Show();
        }

        private void mnuBSBLGOptCorMMSimplified_Click(object sender, EventArgs e)
        {
            FrmBSBLGOptCorMMSimplified frmBSBLGOptCorMMSimplified = new FrmBSBLGOptCorMMSimplified(_DataRecords);
            frmBSBLGOptCorMMSimplified.Show();
        }

        private void mnuBSBLGOptCor_Click(object sender, EventArgs e)
        {
            FrmBSBLGOptCor frmBSBLGOptCor = new FrmBSBLGOptCor(_DataRecords);
            frmBSBLGOptCor.Show();
        }

        private void mnuBSBLGOptCorMM_Click(object sender, EventArgs e)
        {
            FrmBSBLGOptCorMM frmBSBLGOptCorMM = new FrmBSBLGOptCorMM(_DataRecords);
            frmBSBLGOptCorMM.Show();
        }

        private void mnuRIBS_Click(object sender, EventArgs e)
        {
            FrmRIBS frmRIBS = new FrmRIBS(_DataRecords);
            frmRIBS.Show();
        }

        private void mnuRIBSBLG_Click(object sender, EventArgs e)
        {
            FrmRIBSBLG frmRIBSBLG = new FrmRIBSBLG(_DataRecords);
            frmRIBSBLG.Show();
        }

        private void mnuRIBSBLGOptCor_Click(object sender, EventArgs e)
        {
            FrmRIBSBLGOptCor frmRIBSBLGOptCor = new FrmRIBSBLGOptCor(_DataRecords);
            frmRIBSBLGOptCor.Show();
        }

        private void mnuAtBdMorphing_Click(object sender, EventArgs e)
        {
            FrmAtBdMorphing frmAtBdMorphing = new FrmAtBdMorphing(_DataRecords);
            frmAtBdMorphing.Show();
        }

        private void mnuPAL_Click(object sender, EventArgs e)
        {
            FrmPAL frmPAL = new FrmPAL(_DataRecords);
            frmPAL.Show();
        }

        private void mnuAL_AL_Click(object sender, EventArgs e)
        {
            FrmAL_AL frmAL_AL = new FrmAL_AL(_DataRecords);
            frmAL_AL.Show();
        }

        private void mnuLinear_AL_Click(object sender, EventArgs e)
        {
            FrmLinear_AL frmLinear_AL = new FrmLinear_AL(_DataRecords);
            frmLinear_AL.Show();
        }

        private void mnuSimAngle3_Click(object sender, EventArgs e)
        {
            FrmSimAngle3 frmSimAngle3 = new FrmSimAngle3(_DataRecords);
            frmSimAngle3.Show();
        }

        private void mnuSimDeflection_Click(object sender, EventArgs e)
        {
            FrmSimDeflection frmSimDeflection = new FrmSimDeflection(_DataRecords);
            frmSimDeflection.Show();
        }

        private void mnuTF_Click(object sender, EventArgs e)
        {
            FrmTF frmTF = new FrmTF(_DataRecords);
            frmTF.Show();
        }

        private void mnuSimultaneity_Click(object sender, EventArgs e)
        {
            FrmSimultaneity frmSimultaneity = new FrmSimultaneity(_DataRecords);
            frmSimultaneity.Show();
        }

        private void mnuOrdinal_Click(object sender, EventArgs e)
        {
            FrmOrdinal frmOrdinal = new FrmOrdinal(_DataRecords);
            frmOrdinal.Show();
        }

        private void mnuExcelToShape_Click(object sender, EventArgs e)
        {
            FrmExcelToShape frmExcelToShape = new FrmExcelToShape(_DataRecords);
            frmExcelToShape.Show();
        }

        private void mnuLandingTime_Click(object sender, EventArgs e)
        {
            FrmLandingTime frmLandingTime = new FrmLandingTime(_DataRecords);
            frmLandingTime.Show();
        }

        private void mnuLandingTimeLI_Click(object sender, EventArgs e)
        {
            FrmLandingTimeLI frmLandingTimeLI = new FrmLandingTimeLI(_DataRecords);
            frmLandingTimeLI.Show();
        }

        private void mnuALm_Click(object sender, EventArgs e)
        {
            FrmALm frmCALCMix = new FrmALm(_DataRecords);
            frmCALCMix.Show();
        }

        private void mnuLMulti_Click(object sender, EventArgs e)
        {
            FrmLMulti frmLMulti = new FrmLMulti(_DataRecords);
            frmLMulti.Show();
        }

        private void mnuALLMulti_Click(object sender, EventArgs e)
        {
            FrmALLMulti frmALLMulti = new FrmALLMulti(_DataRecords);
            frmALLMulti.Show();
        }

        private void mnuALAMulti_Click(object sender, EventArgs e)
        {
            FrmALAMulti frmALAMulti = new FrmALAMulti(_DataRecords);
            frmALAMulti.Show();
        }

        private void mnuALALMulti_Click(object sender, EventArgs e)
        {
            FrmALALMulti frmALALMulti = new FrmALALMulti(_DataRecords);
            frmALALMulti.Show();
        }

        private void mnuApLALMulti_Click(object sender, EventArgs e)
        {
            FrmApLALMulti frmApLALMulti = new FrmApLALMulti(_DataRecords);
            frmApLALMulti.Show();
        }

        private void mnuAL_Click(object sender, EventArgs e)
        {
            FrmAL frmAL = new FrmAL(_DataRecords);
            frmAL.Show();
        }

        private void mnuALL_Click(object sender, EventArgs e)
        {
            FrmALL frmALL = new FrmALL(_DataRecords);
            frmALL.Show();
        }

        private void mnuApLL_Click(object sender, EventArgs e)
        {
            FrmApLL frmApLL = new FrmApLL(_DataRecords);
            frmApLL.Show();
        }

        private void mnuStraightLine_Click(object sender, EventArgs e)
        {
            FrmStraightLine frmStraightLine = new FrmStraightLine(_DataRecords);
            frmStraightLine.Show();
        }

        private void mnuCDTLSA_Click(object sender, EventArgs e)
        {
            FrmCDTLSA frmCDTLSA = new FrmCDTLSA(_DataRecords);
            frmCDTLSA.Show();
        }

        private void mnuEnlargeLayer_Click(object sender, EventArgs e)
        {
            FrmEnlargeLayer frmEnlargeLayer = new FrmEnlargeLayer(_DataRecords);
            frmEnlargeLayer.Show();
        }

        private void mnuDPSimplify_Click(object sender, EventArgs e)
        {
            FrmDPSimplify frmDPSimplify = new FrmDPSimplify(_DataRecords);
            frmDPSimplify.Show();
        }

        private void mnuLookingForNeighboursDT2_Click(object sender, EventArgs e)
        {
            FrmLookingForNeighboursDT frmLookingForNeighboursDT = new FrmLookingForNeighboursDT(_DataRecords);
            frmLookingForNeighboursDT.Show();
        }

        private void mnuCreateRandomPointLayer_Click(object sender, EventArgs e)
        {
            FrmCreateRandomPointLayer frmCreateRandomPointLayer = new FrmCreateRandomPointLayer(_DataRecords);
            frmCreateRandomPointLayer.Show();
        }

        private void mnuLookingForNeighboursSweepLine_Click(object sender, EventArgs e)
        {
            FrmLookingForNeighboursSweepLine frmLookingForNeighboursSweepLine = new FrmLookingForNeighboursSweepLine(_DataRecords);
            frmLookingForNeighboursSweepLine.Show();
        }

        private void mnuLookingForNeighboursGrids_Click(object sender, EventArgs e)
        {
            FrmLookingForNeighboursGrids frmLookingForNeighboursGrids = new FrmLookingForNeighboursGrids(_DataRecords);
            frmLookingForNeighboursGrids.Show();
        }

        private void mnuCoordinatesTransformation_Click(object sender, EventArgs e)
        {
            FrmCoordinatesTransformation frmCoordinatesTransformation = new FrmCoordinatesTransformation(_DataRecords);
            frmCoordinatesTransformation.Show();
        }

        private void mnuGaussianPerturbation_Click(object sender, EventArgs e)
        {
            FrmGaussianPerturbation frmGaussianPerturbation = new FrmGaussianPerturbation(_DataRecords);
            frmGaussianPerturbation.Show();
        }

        private void mnuDeletePointsWithSameCoordinates_Click(object sender, EventArgs e)
        {
            FrmDeletePointsWithSameCoordinates frmDeletePointsWithSameCoordinates = new FrmDeletePointsWithSameCoordinates(_DataRecords);
            frmDeletePointsWithSameCoordinates.Show();
        }

        private void mnuClassification_Click(object sender, EventArgs e)
        {
            FrmClassification frmClassification = new FrmClassification(_DataRecords);
            frmClassification.Show();
        }

        private void mnuRightAngelDPS_Click(object sender, EventArgs e)
        {
            FrmRightAngelDPS frmRightAngelDPS = new FrmRightAngelDPS(_DataRecords);
            frmRightAngelDPS.Show();
        }

        private void mnuLinearMorphing_Click(object sender, EventArgs e)
        {
            FrmLinearMorphing frmLinearMorphing = new FrmLinearMorphing(_DataRecords);
            frmLinearMorphing.Show();
        }

        private void mnuTransparencyMorphing_Click(object sender, EventArgs e)
        {
            FrmTransparencyMorphing frmTransparencyMorphing = new FrmTransparencyMorphing(_DataRecords);
            frmTransparencyMorphing.Show();
        }

        private void mnuUnifyDirections_Click(object sender, EventArgs e)
        {
            FrmUnifyDirections frmUnifyDirections = new FrmUnifyDirections(_DataRecords);
            frmUnifyDirections.Show();
        }

        private void mnuToIpe(object sender, EventArgs e)
        {
            FrmToIpe frmToIpe = new FrmToIpe(_DataRecords);
            frmToIpe.Show();
        }

        private void mnuCGABM_Click(object sender, EventArgs e)
        {
            FrmCGABM frmCGABM = new FrmCGABM(_DataRecords);
            frmCGABM.Show();
        }

        private void mnuTopologyChecker_Click(object sender, EventArgs e)
        {
            FrmTopologyChecker frmTopologyChecker = new FrmTopologyChecker(_DataRecords);
            frmTopologyChecker.Show();
        }

        private void mnuStatisticsOfDataSets_Click(object sender, EventArgs e)
        {
            FrmStatisticsOfDataSets frmStatisticsOfDataSets = new FrmStatisticsOfDataSets(_DataRecords);
            frmStatisticsOfDataSets.Show();
        }

        private void mnuTestDistanceMetric_Click(object sender, EventArgs e)
        {
            FrmTestDistanceMetric frmTestDistanceMetric = new FrmTestDistanceMetric(_DataRecords);
            frmTestDistanceMetric.Show();
        }

        private void mnuTest_Click(object sender, EventArgs e)
        {
            //FrmTest frmTest = new FrmTest(_DataRecords);
            //frmTest.Show();
        }

        private void mnuMatchAndMergePolylines_Click(object sender, EventArgs e)
        {
            FrmMatchAndMergePolylines frmMatchAndMergePolylines = new FrmMatchAndMergePolylines(_DataRecords);
            frmMatchAndMergePolylines.Show();
        }

        private void mnuMergeAndSplitPolylines_Click(object sender, EventArgs e)
        {
            FrmMergeAndSplitPolylines frmMergeAndSplitPolylines = new FrmMergeAndSplitPolylines(_DataRecords);
            frmMergeAndSplitPolylines.Show();
        }


        private void mnuMatchAndMergePolygons_Click(object sender, EventArgs e)
        {
            FrmMatchAndMergePolygons frmMatchAndMergePolygons = new FrmMatchAndMergePolygons(_DataRecords);
            frmMatchAndMergePolygons.Show();
        }

        private void mnuUnifyIndicesPolylines_Click(object sender, EventArgs e)
        {
            FrmUnifyIndicesPolylines frmUnifyIndicesPolylines = new FrmUnifyIndicesPolylines(_DataRecords);
            frmUnifyIndicesPolylines.Show();
        }

        private void mnuTemporary_Click(object sender, EventArgs e)
        {
            FrmTemporary frmTemporary = new FrmTemporary(_DataRecords);
            frmTemporary.Show();
        }

        private void mnuIdentifyCorrCpgAddRegionNum_Click(object sender, EventArgs e)
        {
            FrmIdentifyCorrCpgAddRegionNum frmIdentifyCorrCpgAddRegionNum = new FrmIdentifyCorrCpgAddRegionNum(_DataRecords);
            frmIdentifyCorrCpgAddRegionNum.Show();
        }

        private void mnuAreaAgg_AStar_Click(object sender, EventArgs e)
        {
            FrmAreaAgg frmAreaAgg_AStar = new FrmAreaAgg(_DataRecords);
            frmAreaAgg_AStar.Show();
        }

        private void mnuCalGeo_Ipe_Click(object sender, EventArgs e)
        {
            FrmCalGeo_Ipe frmCalGeo_Ipe = new FrmCalGeo_Ipe();
            frmCalGeo_Ipe.Show();
        }

        private void mnuBldgGrow_Click(object sender, EventArgs e)
        {
            FrmBldgGrow frmBldgGrow = new FrmBldgGrow(_DataRecords);
            frmBldgGrow.Show();
        }

        private void mnuCompareExcel_Click(object sender, EventArgs e)
        {
            FrmCompareExcel frmCompareExcel = new FrmCompareExcel();
            frmCompareExcel.Show();
        }
        #endregion

        private void mnuExtractPossibleFiles_Click(object sender, EventArgs e)
        {
            FrmExtractPossibleFiles frmExtractPossibleFiles = new FrmExtractPossibleFiles();
            frmExtractPossibleFiles.Show();
        }

        private void mnuSelectRandomly_Click(object sender, EventArgs e)
        {
            FrmSelectRandomly frmSelectRandomly = new FrmSelectRandomly(_DataRecords);
            frmSelectRandomly.Show();
        }
    }
}