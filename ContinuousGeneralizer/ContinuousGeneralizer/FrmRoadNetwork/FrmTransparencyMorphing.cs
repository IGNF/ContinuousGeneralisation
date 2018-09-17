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
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeneralizationMethods;

namespace ContinuousGeneralizer.RoadNetwork
{
    public partial class FrmTransparencyMorphing : Form
    {
        private CDataRecords _DataRecords;                    //records of data
        
        //private CDPSimplify _pDPSimplify = new CDPSimplify();
        public FrmTransparencyMorphing()
        {
            InitializeComponent();
        }
        public FrmTransparencyMorphing(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmIntersectionExtract_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            ParameterInitialize.m_mapPolyline = new MapClass();
            
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
        }




        public void btnRun_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ////获取当前选择的点要素图层
            ////大比例尺要素图层
            //ParameterInitialize.strSavePath = txtPath.Text;
            //ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);
            //IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapPolyline.get_Layer(ParameterInitialize.cboLargerScaleLayer.Items.Count
            //                                                           - ParameterInitialize.cboLargerScaleLayer.SelectedIndex - 1);
            ////小比例尺要素图层
            //IFeatureLayer pSSFLayer = (IFeatureLayer)ParameterInitialize.m_mapPolyline.get_Layer(ParameterInitialize.cboSmallerScaleLayer.Items.Count
            //                                               - ParameterInitialize.cboSmallerScaleLayer.SelectedIndex - 1);

            //ParameterInitialize.pBSFLayer = pBSFLayer;
            //ParameterInitialize.pSSFLayer = pSSFLayer;
            //double dblProportion = Convert.ToDouble(this.TxtProportion.Text);

            ////获取线数组
            //List<CPolyline> LSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pBSFLayer);
            //List<CPolyline> SSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSSFLayer);

            //int intProportion = Convert.ToInt16(dblProportion * 255);

            //List<object> objlt = new List<object>();
            //for (int i = 0; i < LSCPlLt.Count; i++)
            //{
            //    objlt.Add(LSCPlLt[i]);
            //}
            //CSaveFeature pSaveFeature = new CSaveFeature(objlt, "圆形路口", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl, esriGeometryType.esriGeometryPolyline,255 - intProportion, 255 - intProportion, 255 - intProportion);
            //pSaveFeature.SaveFeaturesToLayer();

            //List<object> objlt1 = new List<object>();
            //for (int i = 0; i < SSCPlLt.Count; i++)
            //{
            //    objlt1.Add(SSCPlLt[i]);
            //}
            //CSaveFeature pSaveFeature1 = new CSaveFeature(objlt1, "直角路口", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl, esriGeometryType.esriGeometryPolyline,intProportion, intProportion, intProportion);
            //pSaveFeature1.SaveFeaturesToLayer();


        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            //退出窗口
            this.Hide();
        }

        private void Choose_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            //建立文件夹，保存之后生成的要素图层
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            txtPath.Text = SFD.FileName;
        }

    }
}
