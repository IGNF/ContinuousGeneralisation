using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Linq;
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

namespace ContinuousGeneralizer.FrmMorphing
{
    public partial class FrmBSBLGOptCorMMSimplified : FrmBLGOptCorMMSimplified
    {

        private CBSBLGOptCorMMSimplified _pBSBLGOptCorMMSimplified;



        public FrmBSBLGOptCorMMSimplified()
        {
            InitializeComponent();
        }

        public FrmBSBLGOptCorMMSimplified(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmBSBLGOptCorMMSimplified_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;
            ParameterInitialize.txtMaxBackK = this.txtMaxBackK;
            ParameterInitialize.strMorphingMethod = "BSBLGOptCorMMSimplified";
            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoadMulticbo();
        }

        public override void btnRun_Click(object sender, EventArgs e)
        {

            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);

            //读取数据
            try
            {
                _pBSBLGOptCorMMSimplified = new CBSBLGOptCorMMSimplified(ParameterInitialize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            _pBSBLGOptCorMMSimplified.BSBLGOptCorMMSimplifiedMorphing();

            _DataRecords.ParameterResult = _pBSBLGOptCorMMSimplified.ParameterResult;
        }

    }
}
