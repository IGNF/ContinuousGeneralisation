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
    public partial class FrmLinearMulti : Form
    {
        private CDataRecords _DataRecords;                    //records of data
        
        
        
        private CLinearMulti _pLinearMulti;


        private List<CPolyline> _RelativeInterpolationCplLt;
        private double _dblProp = 0.5;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmLinearMulti()
        {
            InitializeComponent();
        }

        public FrmLinearMulti(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmLinearMulti_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;
            ParameterInitialize.txtOverlapRatio = this.txtOverlapRatio;
            CConstants.strMethod = "LinearMulti";
            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
        }

        public void btnRun_Click(object sender, EventArgs e)
        {

            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);
            //SFD.FileName .
            long lngStartTime = System.Environment.TickCount;

            //Read Datasets
            try
            {
                _pLinearMulti = new CLinearMulti(ParameterInitialize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            _pLinearMulti.LinearMultiMorphing();
            //try
            //{
            //    _pLinearMulti.CreateCDT();
            //    _pLinearMulti.LinearMultiMorphing();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return;
            //}
            long lngEndTime = System.Environment.TickCount;
            long lngTime = lngEndTime - lngStartTime;
            ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            CParameterResult ParameterResult = _pLinearMulti.ParameterResult;
            ParameterResult.lngTime = lngTime;
            _DataRecords.ParameterResult = ParameterResult;
            //CHelpFuncExcel.KillExcel();
        }


        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);

        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                _dblProp = _dblProp - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
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
            _RelativeInterpolationCplLt = CDrawInActiveView.DisplayInterpolations(_DataRecords, _dblProp);
        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CPolyline> cpllt = new List<CPolyline>();
            for (int i = 0; i < _RelativeInterpolationCplLt.Count; i++)
            {
                cpllt.Add(_RelativeInterpolationCplLt[i]);
            }
            string strFileName = _dblProp.ToString();
            CHelpFunc.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }


        private void btnIntegral_Click(object sender, EventArgs e)
        {
            //CIntegral pIntegral = new CIntegral(_DataRecords);
            //double dblIntegral = pIntegral.CalIntegral();
            //this.txtEvaluation.Text = dblIntegral.ToString();
        }


        private void btnTranslation_Click(object sender, EventArgs e)
        {
            CTranslation pTranslation = new CTranslation(_DataRecords);
            double dblTranslation = pTranslation.CalTranslations();
            //double dblTranslation = pTranslation.CalRatioTranslations();
            this.txtEvaluation.Text = dblTranslation.ToString();
        }


        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _DataRecords.ParameterResultToExcelLt.Count; i++)
            {
                CHelpFuncExcel.ExportEvaluationToExcel(_DataRecords.ParameterResultToExcelLt[i], _DataRecords.ParameterInitialize, i.ToString());
            }
            //CHelpFuncExcel.KillExcel();
        }









    }
}