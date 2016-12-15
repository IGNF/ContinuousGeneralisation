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
using MorphingClass.CCorrepondObjects;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CMorphingExtend;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmMorphingLSA
{
    public partial class FrmALLMulti : FrmStraightLine
    {
        private CALLMulti _pALLMulti = new CALLMulti();
        private int _intI = 0;

        public FrmALLMulti()
        {
            InitializeComponent();
            //_ParameterInitialize.strMorphingMethod = "LandingTime";
        }

        public FrmALLMulti(CDataRecords pDataRecords)
        {
            InitializeComponent();

            CParameterInitialize ParameterInitialize = pDataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLayer  = this.cboLayer;
            ParameterInitialize.txtInterpolationNum = this.txtInterpolatedNum;
            ParameterInitialize.txtIterationNum = this.txtIterationNum;
            ParameterInitialize.strMorphingMethod = "ALLMulti";
            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoad();
            _DataRecords = pDataRecords;
           
        }

        public override void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);
            _pALLMulti = new CALLMulti(_DataRecords);
            _pALLMulti.ALLMultiMorphing();

            CHelperFunction.SaveCPlLt(_DataRecords.ParameterResult.CResultPlLt, "ALLMulti", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }

        public override void btnReduce_Click(object sender, EventArgs e)
        {
            this.timerAdd.Enabled = false;
            this.timerReduce.Enabled = true;
        }

        public override void btnAdd_Click(object sender, EventArgs e)
        {
            this.timerAdd.Enabled = true;
            this.timerReduce.Enabled = false;
        }


    }
}
