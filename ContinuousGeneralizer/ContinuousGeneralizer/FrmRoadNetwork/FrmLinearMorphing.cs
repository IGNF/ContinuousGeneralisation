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
using MorphingClass.CGeneralizationMethods;
using ContinuousGeneralizer;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;
using MorphingClass.CCorrepondObjects;

namespace ContinuousGeneralizer.RoadNetwork
{

    public partial class FrmLinearMorphing : Form
    {
        private CDataRecords _DataRecords;                    //数据记录
        private CFrmOperation _FrmOperation;
        public FrmLinearMorphing()
        {
            InitializeComponent();
        }
        public FrmLinearMorphing(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmLinearMorphing_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            ParameterInitialize.m_mapPolyline = new MapClass();
            
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;


            //进行Load操作，初始化变量

            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
        }
        public void btnRun_Click(object sender, EventArgs e)
        {

        }
    }
}
