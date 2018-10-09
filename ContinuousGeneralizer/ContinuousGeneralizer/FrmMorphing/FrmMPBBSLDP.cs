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
    public partial class FrmMPBBSLDP : Form
    {
        private CDataRecords _DataRecords;                    //records of data
        
        
        
        private CMPBBSLDP _pMPBBSLDP;


        private CPolyline _RelativeInterpolationCpl;
        private double _dblProp = 0.5;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmMPBBSLDP()
        {
            InitializeComponent();
        }

        public FrmMPBBSLDP(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmMPBBSLDP_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;
            
            CConstants.strMethod = "MPBBSLDP";
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

            //读取数据
            try
            {
                _pMPBBSLDP = new CMPBBSLDP(ParameterInitialize);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return;
            }

            _pMPBBSLDP.MPBBSLDPMorphing();

            _DataRecords.ParameterResult = _pMPBBSLDP.ParameterResult;
        }


        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);

        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            if (_dblProp <= 0.02)
            {
                _dblProp = 0;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
            }
            else
            {
                _dblProp = _dblProp - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (_dblProp>=0.98)
            {
                            _dblProp = 1;
            pbScale.Value = Convert.ToInt16(100 * _dblProp);
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
            }
            else
            {
                _dblProp = _dblProp + 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
            }
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
            CTranslation pTranslation = new CTranslation(_DataRecords);
            double dblTranslation = pTranslation.CalTranslation();
            //double dblTranslation = pTranslation.CalRatioTranslation();
            this.txtEvaluation.Text = dblTranslation.ToString();
        }


        private void btnExportToExcel_Click(object sender, EventArgs e)
        {
            CHelpFuncExcel.ExportEvaluationToExcel(_DataRecords.ParameterResultToExcel, _DataRecords.ParameterInitialize, "0");
            //CHelpFuncExcel.KillExcel();
        }

        private void btnStatistic_Click(object sender, EventArgs e)
        {
            double dblProp = 0;
            List < double> dbllt = new List<double>();
            List<CPoint> CResultPtLt = _DataRecords.ParameterResult.CResultPtLt;
            for (int i = 0; i <= 100; i++)
            {                
                CPolyline cpl = CGeoFunc.GetTargetcpl(CResultPtLt, dblProp);
                dbllt.Add(cpl.pPolyline.Length);
                dblProp = dblProp + 0.01;
            }

            CHelpFuncExcel.ExportDataltToExcel(dbllt, "Length", _DataRecords.ParameterInitialize.strSavePath);
            //if (dblProp < 0 || dblProp > 1)
            //{
            //    MessageBox.Show("请输入正确参数！");
            //    return null;
            //}
            //List<CPoint> CResultPtLt = pDataRecords.ParameterResult.CResultPtLt;
            //CPolyline cpl = this.CGeoFunc.GetTargetcpl(CResultPtLt, dblProp);

            //// 清除绘画痕迹
            //IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
            //IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            //pGra.DeleteAllElements();
            //m_mapControl.ActiveView.Refresh();
            //this.ViewPolyline(m_mapControl, cpl);  //显示生成的线段
            //return cpl;
        }









    }
}