using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.COMSupport;
//using ESRI.ArcGIS.ADF.Resources;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;

using ContinuousGeneralizer;
using MorphingClass.CAid;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CCorrepondObjects;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmUnifyDirections : Form
    {
        private CFrmOperation _FrmOperation;
        
        CDataRecords _DataRecords;

        public FrmUnifyDirections()
        {
            InitializeComponent();
        }




        public FrmUnifyDirections(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmUnifyDirections_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>(2);

            ParameterInitialize.cboLayerLt.Add(this.cboLargerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboSmallerScaleLayer);

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            CUnifyDirectionsPolylines pUnifyDirectionsPolylines = new CUnifyDirectionsPolylines(ParameterInitialize);
            pUnifyDirectionsPolylines.UnifyDirectionsPolylines();

            MessageBox.Show("Done!");












            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            ////弹出保存对话框
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //string strPath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(strPath);

            ////获取当前选择的点要素图层
            ////大比例尺要素图层
            //IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLargerScaleLayer.SelectedIndex);
                                                                       
            ////小比例尺要素图层
            //IFeatureLayer pSSFLayer =(IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboSmallerScaleLayer.SelectedIndex);
                                                           

            //ParameterInitialize.pBSFLayer = pBSFLayer;       //"Fixed layer"
            //ParameterInitialize.pSSFLayer = pSSFLayer;       //"(layer) would be modified"

            ////获取线数组
            //List<CPolyline> LSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pBSFLayer);
            //List<CPolyline> SSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pSSFLayer);

            //for (int i = 0; i < LSCPlLt.Count; i++)
            //{
            //    LSCPlLt[i].SetBaseLine();
            //    SSCPlLt[i].SetBaseLine();

            //    double dblFrDiffX = LSCPlLt[i].pBaseLine.ToCpt.X - LSCPlLt[i].pBaseLine.FrCpt.X;
            //    double dblFrDiffY = LSCPlLt[i].pBaseLine.ToCpt.Y - LSCPlLt[i].pBaseLine.FrCpt.Y;
            //    double dblToDiffX = SSCPlLt[i].pBaseLine.ToCpt.X - SSCPlLt[i].pBaseLine.FrCpt.X;
            //    double dblToDiffY = SSCPlLt[i].pBaseLine.ToCpt.Y - SSCPlLt[i].pBaseLine.FrCpt.Y;
            //    double dblAngleDiff = CGeometricMethods.CalAngle2(dblFrDiffX, dblFrDiffY, dblToDiffX, dblToDiffY);

            //    if ((Math.Abs(dblAngleDiff) > (Math.PI / 2) && Math.Abs(dblAngleDiff) < (3 * Math.PI / 2)))
            //    {
            //        SSCPlLt[i].ReverseCpl();
            //        SSCPlLt[i].SetPolyline();
            //    }
            //}

            ////Save
            //CHelperFunction.SaveCPlLt(SSCPlLt, ParameterInitialize.pSSFLayer.Name + "UnifiedDirections", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

        }


    }
}
