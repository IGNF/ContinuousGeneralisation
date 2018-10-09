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
    public partial class FrmCGABM : Form
    {
        private CDataRecords _DataRecords;                    //records of data
        

        private CCGABM _pCGABM;

        //private List<CPolyline> _RelativeInterpolationCplLt;
        private double _dblProp = 0.5;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmCGABM()
        {
            InitializeComponent();
        }

        public FrmCGABM(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            pDataRecords.ParameterInitialize.frmCurrentForm = this;
        }

        private void FrmCGABM_Load(object sender, EventArgs e)
        {
            FrmCGABMLoad();  //for some unknown reason we cannot put the "load" codes here, otherwise we cannot inherit this class perferctly
        }

        private void FrmCGABMLoad()
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>(6);
            ParameterInitialize.cboLayerLt.Add(this.cboLargerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboSmallerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboInterBS);
            ParameterInitialize.cboLayerLt.Add(this.cboInterSS);
            ParameterInitialize.cboLayerLt.Add(this.cboSingleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboSingleSmallerScaleLayer);
            CConstants.strMethod = "CGABM";
            ParameterInitialize.cboTransform = this.cboTransform;

            this.cboTransform.SelectedIndex = 0;

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
        }

        private void btnAddFaceNumber_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;


            //读取数据
            _pCGABM = new CCGABM(ParameterInitialize,3,2);
            _pCGABM.IdentifyAddFaceNumber();

            _DataRecords.ParameterResult = _pCGABM.ParameterResult;
            //CHelpFuncExcel.KillExcel();
            MessageBox.Show("Done!");
        }

        public void btnTransform_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            //读取数据
            _pCGABM = new CCGABM(ParameterInitialize,3,2,false);
            _pCGABM.Transform();

            _DataRecords.ParameterResult = _pCGABM.ParameterResult;
            //CHelpFuncExcel.KillExcel();
            MessageBox.Show("Done!");
        }

        private void btnDeleteNumber_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            var cboLayer2=ParameterInitialize.cboLayerLt[2];
            var cboLayer3=ParameterInitialize.cboLayerLt[3];
            ParameterInitialize.cboLayerLt.RemoveAt(3);
            ParameterInitialize.cboLayerLt.RemoveAt(2);

            //读取数据
            _pCGABM = new CCGABM(ParameterInitialize, 4);

            this.txtDeleteNumber.Text = _pCGABM.ComputeDeleteNumber().ToString();

            ParameterInitialize.cboLayerLt.Insert(2, cboLayer2);
            ParameterInitialize.cboLayerLt.Insert(3, cboLayer3);

            _DataRecords.ParameterResult = _pCGABM.ParameterResult;
            //CHelpFuncExcel.KillExcel();
            MessageBox.Show("Done!");
        }

        public void btnRun_Click(object sender, EventArgs e)  // public btnRun_Click
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.dblLargerScale = Convert.ToDouble(this.txtLargerScale.Text);
            ParameterInitialize.dblSmallerScale = Convert.ToDouble(this.txtSmallerScale.Text);

            //读取数据
            _pCGABM = new CCGABM(ParameterInitialize,4,2);
            _pCGABM.CGABM();
            _DataRecords.ParameterResult = _pCGABM.ParameterResult;
            //CHelpFuncExcel.KillExcel();
            MessageBox.Show("Done!");
        }


        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _pCGABM.DisplayAtBd(_dblProp);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _pCGABM.DisplayAtBd(_dblProp);
        }


        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                _dblProp = _dblProp - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _pCGABM.DisplayAtBd(_dblProp);
            }
            catch (Exception)
            {
                MessageBox.Show("不能再减小了！");
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _dblProp = _dblProp + 0.02;
            pbScale.Value = Convert.ToInt16(100 * _dblProp);
            _pCGABM.DisplayAtBd(_dblProp);
        }

        private void btnMultiResults_Click(object sender, EventArgs e)
        {
            int intMultiNum = Convert.ToInt16(txtMultiNum.Text);
            double dblInterval = 1 / Convert.ToDouble(intMultiNum - 1);

            double dblProp = dblInterval;
            for (int i = 1; i < intMultiNum - 1; i++)
            {
                _pCGABM.DisplayAtBd(dblProp);
                dblProp += dblInterval;
            }

            //the last result
            //_pCGABM.DisplayAtBd(1);
            //_pCGABM.DisplayAtBd(0);

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
            for (int i = 0; i < _DataRecords.ParameterResultToExcelLt.Count; i++)
            {
                CHelpFuncExcel.ExportEvaluationToExcel(_DataRecords.ParameterResultToExcelLt[i], _DataRecords.ParameterInitialize, i.ToString());
            }
            //CHelpFuncExcel.KillExcel();
        }

        private void btnOutputScale_Click(object sender, EventArgs e)
        {
            double dblLSFactor = Convert.ToDouble(txtLargerScale.Text);
            double dblSSFactor = Convert.ToDouble(txtSmallerScale.Text);
            double dblOSFactor = Convert.ToDouble(txtOutputScale.Text);
            _dblProp = Math.Log(dblLSFactor / dblOSFactor, dblLSFactor / dblSSFactor);

            _pCGABM.DisplayAtBd(_dblProp);
        }

















    }
}