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
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmSimilarity
{
    public partial class FrmSimAngles : Form
    {
        //Overlapping Ratio of (Rivers)' Buffers,可以作为两线状要素（河流）的相似性判断依据



        private CFrmOperation _FrmOperation;
        
        CDataRecords _DataRecords;


        public FrmSimAngles()
        {
            InitializeComponent();
        }

        public FrmSimAngles(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmORB_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoadMulticbo();
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            string strLargerLayer = this.cboLargerScaleLayer .Text;
            string strSmallerLayer = this.cboSmallerScaleLayer.Text;

            IFeatureLayer pLargerFLayer = null;
            IFeatureLayer pSmallerFLayer = null;

            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            try
            {
                for (int i = 0; i < ParameterInitialize.m_mapFeature.LayerCount; i++)
                {
                    if (strLargerLayer == ParameterInitialize.m_mapFeature.get_Layer(i).Name)
                    {
                        pLargerFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(i);
                    }
                    else if (strSmallerLayer == ParameterInitialize.m_mapFeature.get_Layer(i).Name)
                    {
                        pSmallerFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(i);
                    }
                }
 
            }
            catch (Exception)
            {
                MessageBox.Show("请选择要素图层！");
                return;
            }
          
            double dblTargetScale = Convert.ToDouble(this.txtTargetScale.Text);
            double dblBuffer = 2 * dblTargetScale * 0.00001;  //人眼分辨率(0.01mm)的两倍
            //dblBuffer = 500;  //基于人眼分辨率为0.01mm

            long lngStartTime = System.Environment.TickCount; //记录开始时间

            //获取线数组
            List <CPolyline > LSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pLargerFLayer);
            List<CPolyline> SSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSmallerFLayer);

            ITopologicalOperator pTopBS = LSCPlLt[0].pPolyline as ITopologicalOperator;
            ITopologicalOperator pTopSS = SSCPlLt[0].pPolyline as ITopologicalOperator;
            IGeometry pGeo1 = pTopBS.Buffer(dblBuffer);
            IGeometry pGeo2 = pTopSS.Buffer(dblBuffer);
            
            ITopologicalOperator pTop1 = pGeo1 as ITopologicalOperator;
            IGeometry pIntersectGeo = pTop1.Intersect(pGeo2, esriGeometryDimension.esriGeometry2Dimension);

            IArea pIntersectArea = pIntersectGeo as IArea;
            IArea pArea1 = pGeo1 as IArea;
            IArea pArea2 = pGeo2 as IArea;
            double dblOverlap = 2 * pIntersectArea.Area / (pArea1.Area + pArea2.Area );

            long lngEndTime = System.Environment.TickCount;//记录结束时间
            _DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时

            MessageBox.Show(dblOverlap.ToString ());
        }

        



    }
}