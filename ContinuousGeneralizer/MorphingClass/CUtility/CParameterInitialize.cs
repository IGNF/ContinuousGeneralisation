using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

namespace MorphingClass.CUtility
{
    /// <summary>
    /// 初始化参数类，继承于参数类
    /// </summary>
    /// <remarks></remarks>
    public class CParameterInitialize
    {
        private IFeatureLayer _pBSFLayer;         //大比例尺要素图层(LargerScaleFeatureLayer)
        private IFeatureLayer _pFeatureLayer;     //要素图层
        private IFeatureLayer _pSSFLayer;         //小比例尺要素图层(SmallerScaleFeatureLayer)
        private IFeatureLayer _pSgFLayer;         //Layer of single features        
        private List<IFeatureLayer> _pFLayerLt;
        //private List<List<CPolyline>> _CPlLtLt;

        
        public Form frmContinuousGeneralizer { get; set; }
        //public ESRI.ArcGIS.Controls.ax

        public AxMapControl pAxMapControl { get; set; }
        private IMap _pMap;                       //地图文档
        private IMap _m_mapPolyline;              //线图层集
        //private IMap _m_mapPoints;                //点图层集
        private IMap _m_mapAll;                   //所有图层集
        private IMap _m_mapFeature;                //点图层集

        //private ISpatialReference _pSpatialReference; //坐标参考系
        private IWorkspace _pWorkspace;            //工作区

        private IMapControl4 _m_mapControl;       //地图控件
        //private ITOCControl2 _m_tocControl;       //图层控件

        public CheckBox chkCoincidentPoints { get; set; }
        public CheckBox chkSmallest { get; set; }
        public CheckBox chkTesting { get; set; }
        //private CheckedListBox _clstFields;       //插值字段复选框

        private ComboBox _cboLargerScaleLayer;       //大比例尺图层
        private ComboBox _cboSmallerScaleLayer;     //小比例尺图层
        private ComboBox _cboSingleLayer;
        private List<ComboBox> _cboLayerLt;
        //private ComboBox _cboElevation;           //高程字段
        //private ComboBox _cboField;               //插值字段
        private ComboBox _cboLayer;               //插值图层        
        //private ComboBox _cboTimeType;            //考虑时间的方式
        private ComboBox _cboMorphingMethod;            //
        public ComboBox cboBuffer { get; set; }
        public ComboBox cboStandardVector { get; set; }
        public ComboBox cboEvaluationMethod { get; set; }
        public ComboBox cboIntMaxBackKforJ { get; set; }
        public ComboBox cboTransform { get; set; }
        public ComboBox cboShapeConstraint { get; set; }
        public Dictionary<int, int> intILPFailedIDDt { get; set; }

        //private double _dblBendDelRatio;          //弯曲删除比
        //private double _dblDistance;              //点搜索距离
        //private double _dblError;                 //Bezier曲线探测特征点阈值
        //private double _dblLengthBound;           //对应线判定阈值    
        //private double _dblAngleBound;           //对应线判定阈值 
        //private double _dblPower;                 //距离幂指数
        private double _dblOverlapRatio;          //重叠比率参数
        //private double _dblWeight;                //权重
        private double _dblLargerScale;          //一般情况下，存储了较大比例尺的分母
        private double _dblSmallerScale;         //一般情况下，存储了较小比例尺的分母
        private double _dblTargetScale;          //一般情况下，存储了目标比例尺的分母
        private double _dblLevelExponent;         //层次指数β
        private double _dblOrderExponent;         //等级指数α


        //private int _intMulti;                     //sometimes we need to look for the smallest sufficient look-back parameter, so we need to test a series (_intMulti) look-back parameters
        private int _intMaxBackK;                 //look-back parameter K
        //private int _intOrder;                    //多项式次数

        //private TextBox _txtBendDelRatio;         //弯曲删除比
        private TextBox _txtMulti;                //sometimes we need to look for the smallest sufficient look-back parameter, so we need to test a series (_intMulti) look-back parameters
        private TextBox _txtIncrease;
        private TextBox _txtMaxBackK;             //look-back parameter K
        private TextBox _txtError;                //Bezier曲线探测特征点阈值
        private TextBox _txtLengthBound;          //对应线判定阈值
        private TextBox _txtAngleBound;          //对应线判定阈值
        //private TextBox _txtOrder;                //多项式次数
        private TextBox _txtOverlapRatio;         //重叠比率参数
        //private TextBox _txtPath;                 //生成栅格的存储路径
        //private TextBox _txtPower;                //距离幂指数
        //private TextBox _txtWeight;               //权重
        private TextBox _txtLargerScale;          //一般情况下，输入较大比例尺的分母
        private TextBox _txtSmallerScale;         //一般情况下，输入较小比例尺的分母
        private TextBox _txtLevelExponent;        //层次指数β
        private TextBox _txtOrderExponent;        //等级指数α
        private TextBox _txtInterpolationNum;     //内插线状要素数量
        private TextBox _txtVtPV;                 //平差中的一个值
        private TextBox _txtT;                    //变形程度参数t
        private TextBox _txtIterationNum;         //迭代次数
        public TextBox txtEvaluation { get; set; }
        public TextBox txtAttributeOfKnown { get; set; }

        private StatusStrip _ststMain;            //静态显示栏
        private ToolStripStatusLabel _tsslTime;   //用时静态显示窗口
        private ToolStripStatusLabel _tsslMessage;   //状态信息静态显示窗口
        private ToolStripProgressBar _tspbMain;   //主窗体进度条

        //private string _strElevation;             //高程字段
        //private string _strFieldName;             //字段名称
        //private string _strGeneralizerMethod;        //Generalizer方法
        //private string _strNewFileName;           //新建图层的名字

        private string _strSavePath;              //存储路径名
        private string _strSaveFolder;              //存储路径名
        //private string _strTimeType;              //考虑时间的方式(算术平均；反幂指数；反时间距离)        

        /// <summary>The whole path to strSaveFolder, already including "\\" at the end, strSavePath == strPath + strLastName + "\\"</summary>
        public string strSavePathBackSlash { get; set; }
        public string strAreaAggregation { get; set; }

        /// <summary>The whole path to Data Folder, already including "\\" at the end</summary>
        public string strMxdPath { get; set; }
        public string strMxdPathBackSlash { get; set; }

        /// <summary>The whole path to strSaveFolder, without "\\" at the end, strSavePath == strPath + strLastName</summary>
        public string strSavePath { get; set; }

        /// <summary>The name of the folder used to save results, not a path</summary>
        public string strSaveFolderName { get; set; }

        /// <summary>choices of time and scale</summary>
        public string strTS { get; set; }

        /// <summary>属性：工作区</summary>
        public IWorkspace pWorkspace { get; set; }
        public IWorkspace pFileGdbWorkspace { get; set; }

        //private List<string> _strFieldLt;         //字段列表，创建图层时需添加的属性字段
        //private List<string> _strclstFieldsLt;    //参加时间插值字段名

        private Form _frmCurrentForm;

        /// <summary>属性：大比例尺要素图层(LargerScaleFeatureLayer)</summary>
        public IFeatureLayer pBSFLayer
        {
            get { return _pBSFLayer; }
            set { _pBSFLayer = value; }
        }

        /// <summary>属性：要素图层</summary>
        public IFeatureLayer pFeatureLayer
        {
            get { return _pFeatureLayer; }
            set { _pFeatureLayer = value; }
        }

        /// <summary>属性：小比例尺要素图层(SmallerScaleFeatureLayer)</summary>
        public IFeatureLayer pSSFLayer
        {
            get { return _pSSFLayer; }
            set { _pSSFLayer = value; }
        }


        /// <summary>属性：Layer of single features</summary>
        public IFeatureLayer pSgFLayer
        {
            get { return _pSgFLayer; }
            set { _pSgFLayer = value; }
        }

        public List<IFeatureLayer> pFLayerLt
        {
            get { return _pFLayerLt; }
            set { _pFLayerLt = value; }
        }


        //public AxMapControl pAxMapControl
        //{
        //    get { return _pAxMapControl; }
        //    set { _pAxMapControl = value; }
        //}


        ///// <summary>属性：地图文档</summary>
        //public IMap pMap
        //{
        //    get { return _pMap; }
        //    set { _pMap = value; }
        //}

        /// <summary>属性：线图层集</summary>
        public IMap m_mapPolyline
        {
            get { return _m_mapPolyline; }
            set { _m_mapPolyline = value; }
        }

        //public IMap m_mapPoints
        //{
        //    get { return _m_mapPoints; }
        //    set { _m_mapPoints = value; }
        //}

        public IMap m_mapFeature
        {
            get { return _m_mapFeature; }
            set { _m_mapFeature = value; }
        }

        ///// <summary>属性：所有图层集</summary>
        //public IMap m_mapAll
        //{
        //    get { return _m_mapAll; }
        //    set { _m_mapAll = value; }
        //}

        /// <summary>属性：地图控件</summary>
        public IMapControl4 m_mapControl
        {
            get { return _m_mapControl; }
            set { _m_mapControl = value; }
        }

        ///// <summary>属性：图层控件</summary>
        //public ITOCControl2 m_tocControl
        //{
        //    get { return _m_tocControl; }
        //    set { _m_tocControl = value; }
        //}

        ///// <summary>属性：图层控件</summary>
        //public ISpatialReference pSpatialReference
        //{
        //    get { return _pSpatialReference; }
        //    set { _pSpatialReference = value; }
        //}



        ///// <summary>属性：插值字段复选框</summary>
        //public CheckedListBox clstFields
        //{
        //    get { return _clstFields; }
        //    set { _clstFields = value; }
        //}

        /// <summary>属性：大比例尺图层</summary>
        public ComboBox cboLargerScaleLayer
        {
            get { return _cboLargerScaleLayer; }
            set { _cboLargerScaleLayer = value; }
        }

        ///// <summary>属性：高程字段</summary>
        //public ComboBox cboElevation
        //{
        //    get { return _cboElevation; }
        //    set { _cboElevation = value; }
        //}

        ///// <summary>属性：插值字段</summary>
        //public ComboBox cboField
        //{
        //    get { return _cboField; }
        //    set { _cboField = value; }
        //}

        /// <summary>属性：插值图层</summary>
        public ComboBox cboLayer
        {
            get { return _cboLayer; }
            set { _cboLayer = value; }
        }

        /// <summary>属性：小比例尺图层</summary>
        public ComboBox cboSmallerScaleLayer
        {
            get { return _cboSmallerScaleLayer; }
            set { _cboSmallerScaleLayer = value; }
        }

        /// <summary>属性：</summary>
        public ComboBox cboSingleLayer
        {
            get { return _cboSingleLayer; }
            set { _cboSingleLayer = value; }
        }

        public List<ComboBox> cboLayerLt
        {
            get { return _cboLayerLt; }
            set { _cboLayerLt = value; }
        }

        ///// <summary>属性：考虑时间的方式</summary>
        //public ComboBox cboTimeType
        //{
        //    get { return _cboTimeType; }
        //    set { _cboTimeType = value; }
        //}

        public ComboBox cboMorphingMethod
        {
            get { return _cboMorphingMethod; }
            set { _cboMorphingMethod = value; }
        }


        /// <summary>属性：look-back parameter K</summary>
        public int intMaxBackK
        {
            get { return _intMaxBackK; }
            set { _intMaxBackK = value; }
        }

        //        /// <summary>属性：look-back parameter K</summary>
        //public int intMulti
        //{
        //    get { return _intMulti; }
        //    set { _intMulti = value; }
        //}


        ///// <summary>属性：弯曲删除比</summary>
        //public double dblBendDelRatio
        //{
        //    get { return _dblBendDelRatio; }
        //    set { _dblBendDelRatio = value; }
        //}

        ///// <summary>属性：对应线判定阈值</summary>
        //public double dblLengthBound
        //{
        //    get { return _dblLengthBound; }
        //    set { _dblLengthBound = value; }
        //}

        //public double dblAngleBound
        //{
        //    get { return _dblAngleBound; }
        //    set { _dblAngleBound = value; }
        //}

        ///// <summary>属性：Bezier曲线探测特征点阈值</summary>
        //public double dblError
        //{
        //    get { return _dblError; }
        //    set { _dblError = value; }
        //}

        ///// <summary>属性：点搜索距离</summary>
        //public double dblDistance
        //{
        //    get { return _dblDistance; }
        //    set { _dblDistance = value; }
        //}

        ///// <summary>属性：距离幂指数</summary>
        //public double dblPower
        //{
        //    get { return _dblPower; }
        //    set { _dblPower = value; }
        //}

        /// <summary>属性：重叠比率参数</summary>
        public double dblOverlapRatio
        {
            get { return _dblOverlapRatio; }
            set { _dblOverlapRatio = value; }
        }

        /// <summary>属性：层次指数β</summary>
        public double dblLevelExponent
        {
            get { return _dblLevelExponent; }
            set { _dblLevelExponent = value; }
        }

        /// <summary>属性：等级指数α</summary>
        public double dblOrderExponent
        {
            get { return _dblOrderExponent; }
            set { _dblOrderExponent = value; }
        }

        /// <summary>属性：一般情况下，存储了较大比例尺的分母</summary>
        public double dblLargerScale
        {
            get { return _dblLargerScale; }
            set { _dblLargerScale = value; }
        }

        /// <summary>属性：一般情况下，存储了较小比例尺的分母</summary>
        public double dblSmallerScale
        {
            get { return _dblSmallerScale; }
            set { _dblSmallerScale = value; }
        }

        /// <summary>属性：一般情况下，存储了目标比例尺的分母</summary>
        public double dblTargetScale
        {
            get { return _dblTargetScale; }
            set { _dblTargetScale = value; }
        }

        ///// <summary>属性：多项式次数</summary>
        //public int intOrder
        //{
        //    get { return _intOrder; }
        //    set { _intOrder = value; }
        //}

        ///// <summary>属性：弯曲删除比</summary>
        //public TextBox txtBendDelRatio
        //{
        //    get { return _txtBendDelRatio; }
        //    set { _txtBendDelRatio = value; }
        //}

        /// <summary>属性：look-back parameter K</summary>
        public TextBox txtMaxBackK
        {
            get { return _txtMaxBackK; }
            set { _txtMaxBackK = value; }
        }

        /// <summary>属性：look-back parameter K</summary>
        public TextBox txtMulti
        {
            get { return _txtMulti; }
            set { _txtMulti = value; }
        }

        public TextBox txtIncrease
        {
            get { return _txtIncrease; }
            set { _txtIncrease = value; }
        }


        /// <summary>属性：Bezier曲线探测特征点阈值</summary>
        public TextBox txtError
        {
            get { return _txtError; }
            set { _txtError = value; }
        }

        /// <summary>属性：对应线判定阈值</summary>
        public TextBox txtLengthBound
        {
            get { return _txtLengthBound; }
            set { _txtLengthBound = value; }
        }

        public TextBox txtAngleBound
        {
            get { return _txtAngleBound; }
            set { _txtAngleBound = value; }
        }

        ///// <summary>属性：多项式次数</summary>
        //public TextBox txtOrder
        //{
        //    get { return _txtOrder; }
        //    set { _txtOrder = value; }
        //}

        /// <summary>属性：重叠比率参数</summary>
        public TextBox txtOverlapRatio
        {
            get { return _txtOverlapRatio; }
            set { _txtOverlapRatio = value; }
        }

        ///// <summary>属性：存储路径</summary>
        //public TextBox txtPath
        //{
        //    get { return _txtPath; }
        //    set { _txtPath = value; }
        //}

        ///// <summary>属性：距离幂指数</summary>
        //public TextBox txtPower
        //{
        //    get { return _txtPower; }
        //    set { _txtPower = value; }
        //}

        ///// <summary>属性：权重</summary>
        //public TextBox txtWeight
        //{
        //    get { return _txtWeight; }
        //    set { _txtWeight = value; }
        //}

        /// <summary>属性：层次指数β</summary>
        public TextBox txtLevelExponent
        {
            get { return _txtLevelExponent; }
            set { _txtLevelExponent = value; }
        }

        /// <summary>属性：等级指数α</summary>
        public TextBox txtOrderExponent
        {
            get { return _txtOrderExponent; }
            set { _txtOrderExponent = value; }
        }

        /// <summary>属性：一般情况下，输入较大比例尺的分母</summary>
        public TextBox txtLargerScale
        {
            get { return _txtLargerScale; }
            set { _txtLargerScale = value; }
        }

        /// <summary>属性：一般情况下，输入较小比例尺的分母</summary>
        public TextBox txtSmallerScale
        {
            get { return _txtSmallerScale; }
            set { _txtSmallerScale = value; }
        }

        /// <summary>属性：内插线状要素数量</summary>
        public TextBox txtInterpolationNum
        {
            get { return _txtInterpolationNum; }
            set { _txtInterpolationNum = value; }
        }

        /// <summary>属性：内插线状要素数量</summary>
        public TextBox txtVtPV
        {
            get { return _txtVtPV; }
            set { _txtVtPV = value; }
        }

        /// <summary>属性：变形程度参数</summary>
        public TextBox txtT
        {
            get { return _txtT; }
            set { _txtT = value; }
        }

        /// <summary>属性：迭代次数</summary>
        public TextBox txtIterationNum
        {
            get { return _txtIterationNum; }
            set { _txtIterationNum = value; }
        }

        /// <summary>属性：静态显示栏</summary>
        public StatusStrip ststMain
        {
            get { return _ststMain; }
            set { _ststMain = value; }
        }

        /// <summary>属性：用时静态显示窗口</summary>
        public ToolStripStatusLabel tsslTime
        {
            get { return _tsslTime; }
            set { _tsslTime = value; }
        }

        /// <summary>属性：状态信息静态显示窗口</summary>
        public ToolStripStatusLabel tsslMessage
        {
            get { return _tsslMessage; }
            set { _tsslMessage = value; }
        }

        /// <summary>属性：主窗体进度条</summary>
        public ToolStripProgressBar tspbMain
        {
            get { return _tspbMain; }
            set { _tspbMain = value; }
        }

        ///// <summary>属性：高程字段</summary>
        //public string strElevation
        //{
        //    get { return _strElevation; }
        //    set { _strElevation = value; }
        //}

        ///// <summary>属性：字段名称</summary>
        //public string strFieldName
        //{
        //    get { return _strFieldName; }
        //    set { _strFieldName = value; }
        //}


        ///// <summary>属性：Generalizer方法</summary>
        //public string strGeneralizerMethod
        //{
        //    get { return _strGeneralizerMethod; }
        //    set { _strGeneralizerMethod = value; }
        //}

        ///// <summary>属性：新建图层的名字</summary>
        //public string strNewFileName
        //{
        //    get { return _strNewFileName; }
        //    set { _strNewFileName = value; }
        //}







        ///// <summary>属性：考虑时间的方式(算术平均；反幂指数；反时间距离)</summary>
        //public string strTimeType
        //{
        //    get { return _strTimeType; }
        //    set { _strTimeType = value; }
        //}


        ///// <summary>属性：字段列表，创建图层时需添加的属性字段</summary>
        //public List<string> strFieldLt
        //{
        //    get { return _strFieldLt; }
        //    set { _strFieldLt = value; }
        //}

        ///// <summary>属性：字段列表，创建图层时需添加的属性字段</summary>
        //public List<string> strclstFieldsLt
        //{
        //    get { return _strclstFieldsLt; }
        //    set { _strclstFieldsLt = value; }
        //}


        public Form frmCurrentForm
        {
            get { return _frmCurrentForm; }
            set { _frmCurrentForm = value; }
        }






    }
}
