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

using MorphingClass.CAid;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmCreatePointLayer : Form
    {
        private CFrmOperation _FrmOperation;
        
        CDataRecords _DataRecords;


        public FrmCreatePointLayer()
        {
            InitializeComponent();
        }

        public FrmCreatePointLayer(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmCreatePointLayer_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>();
            ParameterInitialize.cboLayerLt.Add(this.cboLayer);
           
            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //get parameters
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            CCreatePointLayer pCreatePointLayer = new CCreatePointLayer(ParameterInitialize);
            pCreatePointLayer.CreatePointLayer();
            MessageBox.Show("Done!");





            //IFeatureLayer pFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLayerLt[0].SelectedIndex);


            ////save path
            //CHelpFunc.SetSavePath(ParameterInitialize);

            //long lngStartTime = System.Environment.TickCount; //记录开始时间

            ////读取线数据
            //List<CPolyline> CPolylineLt = CHelpFunc.GetCPlLtByFeatureLayer(pFLayer);
            //List<List<CPoint>> cptltlt = new List<List<CPoint>>(CPolylineLt.Count);
            //for (int i = 0; i < CPolylineLt.Count; i++)
            //{
            //    List<CPoint> cptlt = CPolylineLt[i].CptLt;
            //    cptltlt.Add(cptlt);
            //}

            ////确定保存信息
            //CSaveFeature pSaveFeature = new CSaveFeature("PtOnPl", strPath, ParameterInitialize.m_mapControl, esriGeometryType.esriGeometryPoint);
            //List<List<object>> objectvalueltlt = new List<List<object>>();
            //List<object> objshapelt = new List<object>();
            //for (int i = 0; i < cptltlt.Count; i++)
            //{
            //    for (int j = 0; j < cptltlt[i].Count; j++)
            //    {
            //        objshapelt.Add(cptltlt[i][j]);
            //        List<object> objvaluelt = new List<object>();//每个要素有一个数据记录
            //        objvaluelt.Add(j);
            //        objectvalueltlt.Add(objvaluelt);
            //    }
            //}

            //List<esriFieldType> esriFieldTypeLt = new List<esriFieldType>();
            //esriFieldTypeLt.Add(esriFieldType.esriFieldTypeInteger);
            //List<string> strFieldNameLt = new List<string>();
            //strFieldNameLt.Add("OnID");

            //pSaveFeature.esriFieldTypeLt = esriFieldTypeLt;
            //pSaveFeature.objectShapeLt = objshapelt;
            //pSaveFeature.objectValueLtLt = objectvalueltlt;
            //pSaveFeature.strFieldNameLt = strFieldNameLt;

            ////保存点数据为要素图层
            //pSaveFeature.SaveFeaturesToLayer();

            //long lngEndTime = System.Environment.TickCount;//记录结束时间
            //_DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时
        }




















    }
}