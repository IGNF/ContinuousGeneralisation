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
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmMorphing
{
    public partial class FrmVertexInsertion : Form
    {
        private CDataRecords _DataRecords;                    //数据记录
        private CFrmOperation _FrmOperation;
        
        

        private CPolyline _RelativeInterpolationCpl;
        private double _dblProportion;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }

        /// <summary>构造函数</summary>
        public FrmVertexInsertion()
        {
            InitializeComponent();
        }

        /// <summary>构造函数</summary>
        public FrmVertexInsertion(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmVertexInsertion_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLayer = this.cboLayer;
            ParameterInitialize.strMorphingMethod = "VertexInsertion";
            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoad();
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);

            CVertexInsertionM pVertexInsertionM = new CVertexInsertionM(ParameterInitialize);
            long lngStartTime = System.Environment.TickCount;
            pVertexInsertionM.VIMorphing ();
            long lngEndTime = System.Environment.TickCount;
            long lngTime = lngEndTime - lngStartTime;
            ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            CParameterResult ParameterResult = pVertexInsertionM.ParameterResult;
            ParameterResult.lngTime = lngTime;
            _DataRecords.ParameterResult = ParameterResult;

        }


        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.1;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.2;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.3;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.4;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.5;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.6;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.7;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.8;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.9;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProportion = 0;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.25;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.75;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProportion = 1;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProportion = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);

        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CPolyline> cpllt = new List<CPolyline>();
            cpllt.Add(_RelativeInterpolationCpl);
            string strFileName = _dblProportion.ToString();
            CHelperFunction.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }


        private void btnIntegral_Click(object sender, EventArgs e)
        {
            CIntegral pIntegral = new CIntegral(_DataRecords);
            double dblIntegral = pIntegral.CalIntegral();
            this.txtEvaluation.Text = dblIntegral.ToString();
        }


        private void btnTranslation_Click(object sender, EventArgs e)
        {
            CTranslation pTranslation = new CTranslation(_DataRecords);
            double dblTranslation = pTranslation.CalTranslation();
            //double dblTranslation = pTranslation.CalDTranslation();
            this.txtEvaluation.Text = dblTranslation.ToString();
        }


        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            CHelperFunctionExcel.ExportEvaluationToExcel(_DataRecords.ParameterResultToExcel, _DataRecords.ParameterInitialize, "0");
            //CHelperFunctionExcel.KillExcel();
        }




    }
}