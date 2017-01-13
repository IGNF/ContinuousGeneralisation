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
    public partial class FrmOptCor : Form
    {
        private CDataRecords _DataRecords;                    //���ݼ�¼
        private CFrmOperation _FrmOperation;
        private COptCor _pOptCor;

        private List < CPolyline> _RelativeInterpolationCplLt;
        private double _dblProportion = 0.5;

        /// <summary>���ԣ����ݼ�¼</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmOptCor()
        {
            Initialize();
        }

        public FrmOptCor(CDataRecords pDataRecords)
        {
            Initialize();
            _DataRecords = pDataRecords;
        }

        public void Initialize()
        {
            InitializeComponent();

        }

        public void FrmOptCor_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>(2);
            ParameterInitialize.cboLayerLt.Add(this.cboLargerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboSmallerScaleLayer);
            ParameterInitialize.txtMaxBackK = this.txtMaxBackK;
            ParameterInitialize.txtMulti = this.txtMulti;
            ParameterInitialize.txtIncrease = this.txtIncrease;
            ParameterInitialize.txtEvaluation = this.txtEvaluation;
            ParameterInitialize.cboMorphingMethod = this.cboMorphingMethod;
            ParameterInitialize.chkCoincidentPoints = this.chkCoincidentPoints;
            ParameterInitialize.cboStandardVector = this.cboStandardVector;
            ParameterInitialize.cboEvaluationMethod = this.cboEvaluationMethod;
            ParameterInitialize.cboIntMaxBackKforJ = this.cboIntMaxBackKforJ;
            ParameterInitialize.txtAttributeOfKnown = this.txtAttributeOfKnown;

            this.cboMorphingMethod.SelectedIndex = 1;
            this.cboStandardVector.SelectedIndex = 0;
            this.cboEvaluationMethod.SelectedIndex = 0;
            this.cboIntMaxBackKforJ.SelectedIndex = 3;



            //txtEvaluation
            //����Load��������ʼ������
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
        }


        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;


            _pOptCor = new COptCor(ParameterInitialize);
            _DataRecords.ParameterResult = _pOptCor.OptCorMorphing();
            MessageBox.Show("Done!");
        }


        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.1;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.2;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.3;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.4;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.5;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.6;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.7;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.8;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.9;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProportion = 0;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.25;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.75;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProportion = 1;
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProportion = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);

        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                _dblProportion = _dblProportion - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
            }
            catch (Exception)
            {
                MessageBox.Show("�����ټ�С�ˣ�");
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion + 0.02;
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
            _RelativeInterpolationCplLt = CHelperFunction.GetAndSaveInterpolation(_DataRecords, _dblProportion);
        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //List<CPolyline> cpllt = new List<CPolyline>();
            //cpllt.Add(_RelativeInterpolationCpl);
            //string strFileName = _dblProportion.ToString();
            //CHelperFunction.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }


        private void btnIntegral_Click(object sender, EventArgs e)
        {
            CDeflectionEvaluation pDeflectionEvaluation = new CDeflectionEvaluation(_DataRecords);
            double dblEvaluation = pDeflectionEvaluation.CalDeflectionEvaluation();
            this.txtEvaluation.Text = dblEvaluation.ToString();
        }


        private void btnTranslation_Click(object sender, EventArgs e)
        {
            CTranslationEvaluation pTranslationEvaluation = new CTranslationEvaluation(_DataRecords);
            double dblEvaluation = pTranslationEvaluation.CalTranslationEvaluation();
            this.txtEvaluation.Text = dblEvaluation.ToString();

        }


        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            CHelperFunctionExcel.ExportEvaluationToExcelCorr(_DataRecords.ParameterResult, _DataRecords.ParameterInitialize.strSavePath);
        }

        private void btnStatistic_Click(object sender, EventArgs e)
        {
            double dblProportion = 0;
            List<double> dbllt = new List<double>();
            List<CPoint> CResultPtLt = _DataRecords.ParameterResult.CResultPtLt;
            for (int i = 0; i <= 100; i++)
            {
                CPolyline cpl = CGeometricMethods.GetTargetcpl(CResultPtLt, dblProportion);
                dbllt.Add(cpl.pPolyline.Length);
                dblProportion = dblProportion + 0.01;
            }

            CHelperFunctionExcel.ExportDataltToExcel(dbllt, "Length", _DataRecords.ParameterInitialize.strSavePath);
        }

        private void btnstatisticEX_Click(object sender, EventArgs e)
        {
            double dblProportion = -1;
            List<double> dbllt = new List<double>();
            List<CPoint> CResultPtLt = _DataRecords.ParameterResult.CResultPtLt;
            for (int i = 0; i <= 300; i++)
            {
                CPolyline cpl = CGeometricMethods.GetTargetcpl(CResultPtLt, dblProportion);
                dbllt.Add(cpl.pPolyline.Length);
                dblProportion = dblProportion + 0.01;
            }

            CHelperFunctionExcel.ExportDataltToExcel(dbllt, "Length", _DataRecords.ParameterInitialize.strSavePath);
        }











    }
}