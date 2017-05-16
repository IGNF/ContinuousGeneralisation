using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

using ContinuousGeneralizer;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmFishEye
{
    public partial class FrmCDTLSA : Form
    {
        private CDataRecords _DataRecords;                    //数据记录
        private CFrmOperation _FrmOperation;
        
        

        private CPolyline _RelativeInterpolationCpl;
        private double _dblProportion;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }

        /// <summary>构造函数</summary>
        public FrmCDTLSA()
        {
            InitializeComponent();
        }

        /// <summary>构造函数</summary>
        public FrmCDTLSA(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmCDTLoad(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLayer = this.cboLayer;
            CConstants.strMethod = "CDTLSA";
            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoad();
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            ////获取用户输入参数
            //double dblCenterX = Convert.ToDouble(this.txtCenterX.Text);
            //double dblCenterY = Convert.ToDouble(this.txtCenterY.Text);
            //double dblR = Convert.ToDouble(this.txtR.Text);
            //double dblZ = Convert.ToDouble(this.txtZ.Text);

            ////获取当前选择的要素图层
            //IFeatureLayer pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLayer.Items.Count
            //                                                          
            //ParameterInitialize.pFeatureLayer = pFeatureLayer;

            ////获取多边形数组
            //List<CPolygon> CPolygonLt = CHelpFunc.GetCPolygonLtByFeatureLayer(pFeatureLayer);

            ////将所有顶点到处到cptlt
            //List <CPoint > cptlt=new List<CPoint> ();
            //for (int i = 0; i < CPolygonLt.Count ; i++)
            //{
            //    cptlt.AddRange (CPolygonLt[i].CptLt );
            //}

            ////建立文件夹，保存之后生成的要素图层
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //ParameterInitialize.strSavePath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);

            ////创建约束三角网
            //double dblVerySmall=0.000001;
            //CTriangulator OptCDT = new CTriangulator();
            //List<CTriangle> CDTLt = OptCDT.CreateCDT(pFeatureLayer, cptlt, dblVerySmall);

            ////保存三角网
            //List<CTriangle> CTriangleLt = new List<CTriangle>();
            //for (int i = 0; i < CDTLt.Count; i++)
            //{
            //    if (CDTLt[i].strTriType != "I")
            //    {
            //        CTriangleLt.Add(CDTLt[i]);
            //    }
            //}
            //CHelpFunc.SaveTriangles(CDTLt, "CDT", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

























            //long lngStartTime = System.Environment.TickCount;
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            //CParameterResult ParameterResult = pCDTLSAM.ParameterResult;
            //ParameterResult.lngTime = lngTime;
            //_DataRecords.ParameterResult = ParameterResult;

        }





    }
}