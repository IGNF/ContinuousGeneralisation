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
    public partial class FrmBLGOptCorMMSimplified : Form
    {
        protected CDataRecords _DataRecords;                    //records of data

        private CBLGOptCorMMSimplified _pBLGOptCorMMSimplified;


        protected CPolyline _RelativeInterpolationCpl;
        protected double _dblProp = 0.5;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmBLGOptCorMMSimplified()
        {
            InitializeComponent();
        }

        public FrmBLGOptCorMMSimplified(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmBLGOptCorMMSimplified_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;
            ParameterInitialize.txtMaxBackK = this.txtMaxBackK;
            CConstants.strMethod = "BLGOptCorMMSimplified";
            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
        }

        public virtual void btnRun_Click(object sender, EventArgs e)
        {

            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);

            //读取数据
            try
            {
                _pBLGOptCorMMSimplified = new CBLGOptCorMMSimplified(ParameterInitialize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            _pBLGOptCorMMSimplified.BLGOptCorMMSimplifiedMorphing();

            _DataRecords.ParameterResult = _pBLGOptCorMMSimplified.ParameterResult;
        }


        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);

        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            this.timerAdd.Enabled = false;
            this.timerReduce.Enabled = true;
        }
        public virtual void timerReduce_Tick(object sender, EventArgs e)
        {
            _dblProp = _dblProp - 0.01;
            if (_dblProp < 0)
            {
                this.timerReduce.Enabled = false;
                _dblProp = 0;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
                return;
            }
            pbScale.Value = Convert.ToInt16(100 * _dblProp);
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.timerAdd.Enabled = true;
            this.timerReduce.Enabled = false;
        }
        public virtual void timerAdd_Tick(object sender, EventArgs e)
        {
            _dblProp = _dblProp + 0.01;
            if (_dblProp > 1)
            {
                this.timerAdd.Enabled = false;
                _dblProp = 1;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
                return;
            }
            pbScale.Value = Convert.ToInt16(100 * _dblProp);
            _RelativeInterpolationCpl = CDrawInActiveView.DisplayInterpolation(_DataRecords, _dblProp);
        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CPolyline> cpllt = new List<CPolyline>();
            cpllt.Add(_RelativeInterpolationCpl);
            string strFileName = _dblProp.ToString();
            CHelpFunc.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }


        private void btnIntegral_Click(object sender, EventArgs e)
        {
            CIntegral pIntegral = new CIntegral(_DataRecords);
            double dblIntegral = pIntegral.CalIntegral();
            this.txtEvaluation.Text = dblIntegral.ToString();
        }


        private void btnTranslation_Click(object sender, EventArgs e)
        {
            //CTranslation pTranslation = new CTranslation(_DataRecords);
            //double dblTranslation = pTranslation.CalTranslation();
            ////double dblTranslation = pTranslation.CalDTranslation();
            //this.txtEvaluation.Text = dblTranslation.ToString();

            CDeflection pDeflection = new CDeflection(_DataRecords);
            double dblDeflection = pDeflection.CalDeflection();
            this.txtEvaluation.Text = dblDeflection.ToString();
        }


        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            CHelpFuncExcel.ExportEvaluationToExcel(_DataRecords.ParameterResultToExcel, _DataRecords.ParameterInitialize, "0");
            //CHelpFuncExcel.KillExcel();
        }


    }
}