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
    public partial class FrmAtBdMorphing : Form
    {
        private CDataRecords _DataRecords;                    //数据记录
        private CFrmOperation _FrmOperation;
        
        
        private CAtBdMorphing _pAtBdMorphing;

        private List<CPolyline> _RelativeInterpolationCplLt;
        private double _dblProportion = 0.5;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmAtBdMorphing()
        {
            InitializeComponent();
        }

        public FrmAtBdMorphing(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmAtBdMorphing_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;
            ParameterInitialize.cboSingleLayer = this.cboSingleLayer;

            ParameterInitialize.strMorphingMethod = "AtBdMorphing";
            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoadThreecbo();
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.dblLargerScale = Convert.ToDouble(this.txtLargerScale.Text);
            ParameterInitialize.dblSmallerScale  = Convert.ToDouble(this.txtSmallerScale .Text);

            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.strSaveFolder = System.IO.Path.GetFileNameWithoutExtension(SFD.FileName);
            ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);


            //读取数据
           
            _pAtBdMorphing = new CAtBdMorphing(ParameterInitialize);


            _pAtBdMorphing.AtBdMorphing();

            _DataRecords.ParameterResult = _pAtBdMorphing.ParameterResult;
            //CHelperFunctionExcel.KillExcel();
            MessageBox.Show("Done!");
        }


        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.1;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.2;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.3;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.4;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.5;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.6;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.7;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.8;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.9;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProportion = 0;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.25;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.75;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProportion = 1;
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProportion = Convert.ToDouble(this.txtProportion.Text);
            _pAtBdMorphing.DisplayAtBd(_dblProportion);

        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                _dblProportion = _dblProportion - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                _pAtBdMorphing.DisplayAtBd(_dblProportion);
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
            _pAtBdMorphing.DisplayAtBd(_dblProportion);
        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //CParameterResult ParameterResult = _DataRecords.ParameterResult;
            //string strFileName = _dblProportion.ToString();
            //CHelperFunction.SaveCPlLt(ParameterResult.DisplayCPlLt, strFileName+"_morphing", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //CHelperFunction.SaveCPlLt(ParameterResult.FadedDisplayCPlLt, strFileName + "_DPFade", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            
            
         


            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //List<CPolyline> cpllt = new List<CPolyline>();
            //for (int i = 0; i < _RelativeInterpolationCplLt.Count; i++)
            //{
            //    cpllt.Add(_RelativeInterpolationCplLt[i]);
            //}
            //string strFileName = _dblProportion.ToString();
            //CHelperFunction.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
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
            for (int i = 0; i < _DataRecords.ParameterResultToExcelLt.Count; i++)
            {
                CHelperFunctionExcel.ExportEvaluationToExcel(_DataRecords.ParameterResultToExcelLt[i], _DataRecords.ParameterInitialize, i.ToString());
            }
            //CHelperFunctionExcel.KillExcel();
        }

        private void btntest_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //ParameterInitialize.dblLargerScale = Convert.ToDouble(this.txtLargerScale.Text);
            //ParameterInitialize.dblSmallerScale = Convert.ToDouble(this.txtSmallerScale.Text);

            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //if (SFD.FileName == null || SFD.FileName == "") return;
            //ParameterInitialize.strSavePath = SFD.FileName;
            //ParameterInitialize.strFolderName = System.IO.Path.GetFileNameWithoutExtension(SFD.FileName);
            //ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);


            ////读取数据

            //_pAtBdMorphing = new CAtBdMorphing(ParameterInitialize);

            //CEnvelope pEnvelopeIpe = new CEnvelope(0, 0, 600, 600);
            //CHelperFunction.SaveToIpe(_pAtBdMorphing._LSCPlLt, 0, 0, 0, "normal",true , "Ipe" + "_" + "0.5", ParameterInitialize.pBSFLayer.AreaOfInterest, pEnvelopeIpe, ParameterInitialize);
            //CHelperFunction.SaveToIpe(_pAtBdMorphing._SgCPlLt, 100, 100, 100, "normal", true, "Ipe" + "_" + "0.5", ParameterInitialize.pBSFLayer.AreaOfInterest, pEnvelopeIpe, ParameterInitialize);
            ////_pAtBdMorphing.AtBdMorphing();

            ////_DataRecords.ParameterResult = _pAtBdMorphing.ParameterResult;
            //////CHelperFunctionExcel.KillExcel();
            //MessageBox.Show("Done!");
        }









    }
}