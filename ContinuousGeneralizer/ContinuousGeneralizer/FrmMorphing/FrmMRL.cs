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
    public partial class FrmMRL : Form
    {
        private CDataRecords _DataRecords;                    //数据记录
        private CFrmOperation _FrmOperation;
        
        
        private CMRL _pMRL;


        private List<CPolyline> _RelativeInterpolationCplLt;
        private double _dblProportion = 0.5;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmMRL()
        {
            InitializeComponent();
        }

        public FrmMRL(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmMRL_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;
            ParameterInitialize.txtOverlapRatio = this.txtOverlapRatio;

            //ParameterInitialize.cboLayer = this.cboLargerScaleLayer;
            //ParameterInitialize.txtBendDelRatio = this.txtBendDelRatio;
            //ParameterInitialize.txtLengthBound = this.txtLengthBound;
            ParameterInitialize.strMorphingMethod = "MRL";
            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoadMulticbo();
        }

        public void btnRun_Click(object sender, EventArgs e)
        {

            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);
            //SFD.FileName .
            long lngStartTime = System.Environment.TickCount;

            //读取数据
            try
            {
                _pMRL = new CMRL(ParameterInitialize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }


            _pMRL.MRLMorphing();
            //try
            //{
            //    _pMRL.CreateCDT();
            //    _pMRL.MRLMorphing();
            //}
            //catch (Exception ex)
            //{
            //    MessageBox.Show(ex.Message);
            //    return;
            //}
            long lngEndTime = System.Environment.TickCount;
            long lngTime = lngEndTime - lngStartTime;
            ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            CParameterResult ParameterResult = _pMRL.ParameterResult;
            ParameterResult.lngTime = lngTime;
            _DataRecords.ParameterResult = ParameterResult;
            //CHelperFunctionExcel.KillExcel();
        }


        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.1;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.2;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.3;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.4;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.5;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.6;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.7;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.8;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.9;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProportion = 0;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.25;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.75;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProportion = 1;
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProportion = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);

        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                _dblProportion = _dblProportion - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
            }
            catch (Exception)
            {
                MessageBox.Show("不能再减小了！");
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion + 0.02;
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
            _RelativeInterpolationCplLt = CGeometricMethods.DisplayInterpolationMix(_DataRecords, _dblProportion);
        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CPolyline> cpllt = new List<CPolyline>();
            for (int i = 0; i < _RelativeInterpolationCplLt.Count; i++)
            {
                cpllt.Add(_RelativeInterpolationCplLt[i]);
            }
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
            double dblTranslation = pTranslation.CalTranslations();
            //double dblTranslation = pTranslation.CalRatioTranslations();
            this.txtEvaluation.Text = dblTranslation.ToString();
        }


        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < _DataRecords.ParameterResultToExcelLt .Count; i++)
            {
                CHelperFunctionExcel.ExportEvaluationToExcel(_DataRecords.ParameterResultToExcelLt[i], _DataRecords.ParameterInitialize,i.ToString()); 
            }            
            //CHelperFunctionExcel.KillExcel();
        }









    }
}