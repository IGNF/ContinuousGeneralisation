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
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmMorphingLSA
{
    /// <summary>
    /// 顾及面状要素面积和周长的变形方法
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>LSA:Least Squares Adjustment;AL:Area and Length</remarks>
    public partial class FrmPAL : FrmStraightLine
    {

        private CPAL _pCAL;




        public FrmPAL()
        {
            InitializeComponent();
        }

        public FrmPAL(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmCAL_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            CConstants.strMethod = "CAL";            
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFG = new OpenFileDialog();
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            if (OFG.FileName == null || OFG.FileName == "") return;
            _DataRecords.ParameterResult = CHelpFuncExcel.InputDataResultPtLt(OFG.FileName);            
        }

        public override void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);
            _pCAL = new CPAL(_DataRecords);
        }

        public override void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }
        public override void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
        }

        public override void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);

        }

        public override void timerAdd_Tick(object sender, EventArgs e)
        {
            _DataRecords.ParameterResult.CResultPlLt = new List<CPolyline>();
            if (_dblProp > 1)
            {
                this.timerAdd.Enabled = false;
                _dblProp = 1;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
            }
            else
            {
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
            }
            _DataRecords.ParameterResult.CResultPlLt.Add(_RelativeInterpolationCpl);

            _dblProp = _dblProp + 0.01;
        }

        public override void timerReduce_Tick(object sender, EventArgs e)
        {

            if (_dblProp < 0)
            {
                this.timerReduce.Enabled = false;
                _dblProp = 0;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
            }
            else
            {
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _pCAL.DisplayInterpolation(_dblProp);
            }
            _dblProp = _dblProp - 0.01;
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            timerAdd.Enabled = false;
            timerReduce.Enabled = false;
        }

        public override void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CPolyline> cpllt = new List<CPolyline>();
            cpllt.Add(_RelativeInterpolationCpl);
            string strFileName = _dblProp.ToString();
            CHelpFunc.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }

        private void btnSaveTrajectory_Click(object sender, EventArgs e)
        {
            try
            {
                CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
                List<CPolyline> cpllt = _DataRecords.ParameterResult.CResultPlLt;
                List<CPolyline> ctjpllt = new List<CPolyline>();
                for (int j = 0; j < cpllt[0].CptLt.Count ; j++)
                {
                    List<CPoint> ctjptlt = new List<CPoint>();
                    for (int i = 0; i < cpllt.Count ; i++)
                    {
                        ctjptlt.Add(cpllt[i].CptLt[j]);
                    }
                    CPolyline ctjpl = new CPolyline(j, ctjptlt);
                    ctjpllt.Add(ctjpl);
                }
                _DataRecords.ParameterResult.CTrajectoryPlLt = ctjpllt;
                CHelpFunc.SaveCPlLt(ctjpllt, "Trajectories", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            }
            catch
            {
                MessageBox.Show("No data or other errors! (Have you already implemented timerAdd_Tick?)");
            }
        }






    }
}