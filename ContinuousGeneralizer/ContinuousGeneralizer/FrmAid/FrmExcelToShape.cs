using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

using ContinuousGeneralizer;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmExcelToShape : Form
    {
        private CDataRecords _DataRecords;                    //数据记录
        
        
        private List<CPolyline> _CPolylineLt = new List<CPolyline>();


        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }

        public FrmExcelToShape()
        {
            InitializeComponent();
        }

        public FrmExcelToShape(CDataRecords pDataRecords)
        {
            _DataRecords = pDataRecords;
            InitializeComponent();
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFG = new OpenFileDialog();
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            if (OFG.FileName == null || OFG.FileName == "") return;
            _CPolylineLt = CHelperFunctionExcel.InputDataLtXYZT(OFG.FileName);
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            //IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            //List<CPolyline> cpllt = _CPolylineLt;

            ////弹出保存对话框
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //string strPath = SFD.FileName;

            ////数据准备
            //CSaveFeature pSaveFeature = new CSaveFeature("CDGLanding", strPath, m_mapControl, esriGeometryType.esriGeometryPolyline);
            //List<List<object>> objectvalueltlt = new List<List<object>>();
            //List<object> objshapelt = new List<object>();
            //for (int i = 0; i < cpllt.Count; i++)
            //{
            //    objshapelt.Add(cpllt[i]);
            //    List<object> objvaluelt = new List<object>();//每个要素有一个数据记录
            //    objvaluelt.Add(cpllt[i].ID);
            //    objectvalueltlt.Add(objvaluelt);
            //}

            //pSaveFeature.objectShapeLt = objshapelt;

            //List<esriFieldType> esriFieldTypeLt = new List<esriFieldType>();
            //esriFieldTypeLt.Add(esriFieldType.esriFieldTypeInteger);
            //List<string> strFieldNameLt = new List<string>();
            //strFieldNameLt.Add("Id");

            //pSaveFeature.esriFieldTypeLt = esriFieldTypeLt;
            //pSaveFeature.objectShapeLt = objshapelt;
            //pSaveFeature.objectValueLtLt = objectvalueltlt;
            //pSaveFeature.strFieldNameLt = strFieldNameLt;

            //pSaveFeature.SaveFeaturesToLayer();
        }
    }
}
