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
    public partial class FrmALAMulti : FrmALLMulti
    {
        private CALAMulti _pALAMulti = new CALAMulti();
 
        public FrmALAMulti()
        {
            InitializeComponent();
        }

        public FrmALAMulti(CDataRecords pDataRecords)
        {
            InitializeComponent();

            CParameterInitialize ParameterInitialize = pDataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLayer = this.cboLayer;
            ParameterInitialize.txtInterpolationNum = this.txtInterpolatedNum;
            CConstants.strMethod = "ALAMulti";
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
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);
            _pALAMulti = new CALAMulti(_DataRecords);
            _pALAMulti.ALAMultiMorphing();

            CHelpFunc.SaveCPlLt(_DataRecords.ParameterResult.CResultPlLt, "ALAMulti", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {

        }
    }
}
