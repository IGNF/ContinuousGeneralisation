using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.COMSupport;
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
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeneralizationMethods;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmGeneralization
{
    public partial class FrmBuildingGrowing : Form
    {
        private CDataRecords _DataRecords;                    //数据记录
        private CFrmOperation _FrmOperation;


        private CBuildingGrowing _pBuildingGrowing;


        private double _dblProportion = 0.5;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmBuildingGrowing()
        {
            InitializeComponent();
        }

        public FrmBuildingGrowing(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            pDataRecords.ParameterInitialize.frmCurrentForm = this;
        }

        private void FrmBuildingGrowing_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>(2);
            ParameterInitialize.cboLayerLt.Add(this.cboLargerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboSmallerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboLSRoad);
            ParameterInitialize.cboLayerLt.Add(this.cboSSRoad);
            CConstants.strMethod = "BuildingGrowing";

            this.cboBufferStyle.SelectedIndex = 0;


            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
        }



        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            //读取数据
            _pBuildingGrowing = new CBuildingGrowing(ParameterInitialize);
            _pBuildingGrowing.BuildingGrowing(Convert.ToDouble(this.txtBufferRadius.Text), this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));

            MessageBox.Show("Done!");
        }

        private void btnResultFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@_DataRecords.ParameterInitialize.strSavePath);
        }



        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.1;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.2;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.3;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.4;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.5;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.6;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.7;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.8;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.9;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProportion = 0;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.25;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.75;
            //_pBuildingGrowing.Output(_dblProportion);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProportion = 1;
            //_pBuildingGrowing.Output(_dblProportion);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProportion = Convert.ToDouble(this.txtProportion.Text);
            //_pBuildingGrowing.Output(_dblProportion);

        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                _dblProportion = _dblProportion - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                //_pBuildingGrowing.Output(_dblProportion);
            }
            catch (System.Exception)
            {
                MessageBox.Show("不能再减小了！");
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion + 0.02;
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
            //_pBuildingGrowing.Output(_dblProportion);
        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //CParameterResult ParameterResult = _DataRecords.ParameterResult;
            //string strFileName = _dblProportion.ToString();
            //CHelperFunction.SaveCPlLt(ParameterResult.DisplayCPlLt, strFileName+"_morphing", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //CHelperFunction.SaveCPlLt(ParameterResult.FadedDisplayCPlLt, strFileName + "_DPFade", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);





            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //List<CPolyline> cpllt = new List<CPolyline>();
            //for (int i = 0; i < _RelativeInterpolationCplLt.Count; i++)
            //{
            //    cpllt.Add(_RelativeInterpolationCplLt[i]);
            //}
            //string strFileName = _dblProportion.ToString();
            //CHelperFunction.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }

        private void btnMultiResults_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < 11; i++)
            {
                _dblProportion = i * 0.1;
                //_pBuildingGrowing.Output(_dblProportion);
            }
        }
    }
}
