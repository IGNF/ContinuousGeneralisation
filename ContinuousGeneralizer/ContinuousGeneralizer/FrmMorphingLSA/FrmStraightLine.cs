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
    public partial class FrmStraightLine : Form
    {
        protected CDataRecords _DataRecords;                    //records of data
        //protected //CHelpFunc _pHelperFunction = new CHelpFunc();
        //protected CHelpFuncExcel _HelperFunctionExcel = new CHelpFuncExcel();


        protected CPolyline _RelativeInterpolationCpl;
        protected double _dblProp = 0;

        private CStraightLine _StraightLine = new CStraightLine();

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmStraightLine()
        {
            if (!DesignMode)
            {
                InitializeComponent();
            }
        }

        public FrmStraightLine(CDataRecords pDataRecords)
        {
            if (!DesignMode)
            {

                InitializeComponent();
                _DataRecords = pDataRecords;
            }
        }

        private void FrmStraightLine_Load(object sender, EventArgs e)
        {
            if (!DesignMode)
            {

                CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
                CConstants.strMethod = "StraightLine";

            }

            
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFG = new OpenFileDialog();
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            if (OFG.FileName == null || OFG.FileName == "") return;
            _DataRecords.ParameterResult = CHelpFuncExcel.InputDataResultPtLt(OFG.FileName);
        }

        private void btnInputReverse_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFG = new OpenFileDialog();
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            if (OFG.FileName == null || OFG.FileName == "") return;
            _DataRecords.ParameterResult = CHelpFuncExcel.InputDataResultPtLt(OFG.FileName, true);
        }

        public virtual void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);
            _StraightLine = new CStraightLine(_DataRecords);
        }

        public virtual void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }
        public virtual void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
        }

        public virtual void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);

        }

        public virtual void btnReduce_Click(object sender, EventArgs e)
        {
            this.timerAdd.Enabled = false;
            this.timerReduce.Enabled = true;
        }

        public virtual void btnAdd_Click(object sender, EventArgs e)
        {
            this.timerAdd.Enabled = true;
            this.timerReduce.Enabled = false;
            _DataRecords.ParameterResult.CResultPlLt = new List<CPolyline>();
        }


        public virtual void timerAdd_Tick(object sender, EventArgs e)
        {            
            if (_dblProp > 1)
            {
                this.timerAdd.Enabled = false;
                _dblProp = 1;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
                _DataRecords.ParameterResult.CResultPlLt.Add(_RelativeInterpolationCpl);
            }
            else
            {
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
                _DataRecords.ParameterResult.CResultPlLt.Add(_RelativeInterpolationCpl);
                _dblProp = _dblProp + 0.01;
            }
        }

        public virtual void timerReduce_Tick(object sender, EventArgs e)
        {

            if (_dblProp < 0)
            {
                this.timerReduce.Enabled = false;
                _dblProp = 0;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
            }
            else
            {
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _StraightLine.DisplayInterpolation(_dblProp);
                _dblProp = _dblProp - 0.01;
            } 
        }

        private void btnStop_Click(object sender, EventArgs e)
        {
            timerAdd.Enabled = false;
            timerReduce.Enabled = false;
        }

        public virtual void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //List<CPolyline> cpllt = new List<CPolyline>();
            //cpllt.Add(_RelativeInterpolationCpl);
            //string strFileName = _dblProp.ToString();
            //CHelpFunc.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);


            ////保存控制点
            //CHelpFunc.SaveControlptlt(_RelativeInterpolationCpl.CptLt,strFileName + "CtrlPt", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

        }

        private void btnSaveTrajectory_Click(object sender, EventArgs e)
        {
            //try
            //{
                CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
                List<CPolyline> cpllt = _DataRecords.ParameterResult.CResultPlLt;
                List<CPolyline> ctjpllt = new List<CPolyline>();
                for (int j = 0; j < cpllt[0].CptLt.Count; j++)
                {
                    List<CPoint> ctjptlt = new List<CPoint>();
                    for (int i = 0; i < cpllt.Count; i++)
                    {
                        ctjptlt.Add(cpllt[i].CptLt[j]);
                    }
                    CPolyline ctjpl = new CPolyline(j, ctjptlt);
                    ctjpllt.Add(ctjpl);
                }
                _DataRecords.ParameterResult.CTrajectoryPlLt = ctjpllt;
                CHelpFunc.SaveCPlLt(ctjpllt, "Trajectories", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            //}
            //catch
            //{
            //    MessageBox.Show("No data or other errors! (Have you already implemented timerAdd_Tick?)");
            //}
        }

        private void btnSaveLengthandAngles_Click(object sender, EventArgs e)
        {
            try
            {
                CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
                List<CPolyline> cpllt = _DataRecords.ParameterResult.CResultPlLt;
                List<List<double>> dbllengthltlt = new List<List<double>>();
                List<List<double>> dblangleltlt = new List<List<double>>();
                for (int i = 0; i < cpllt.Count ; i++)
                {
                    dbllengthltlt.Add(CGeoFunc.RecordLengths(cpllt[i]));
                    dblangleltlt.Add(CGeoFunc.RecordAngles(cpllt[i]));
                }

                CHelpFuncExcel.ExportDataltltToExcel(dbllengthltlt, "lengthltlt", ParameterInitialize.strSavePath);
                CHelpFuncExcel.ExportDataltltToExcel(dblangleltlt, "angleltlt", ParameterInitialize.strSavePath);
            }
            catch
            {
                MessageBox.Show("No data or other errors! (Have you already implemented timerAdd_Tick?)");
            }
        }

        public virtual void btnInputResults_Click(object sender, EventArgs e)
        {

        }

        public virtual void btnConvergence_Click(object sender, EventArgs e)
        {

        }





    }
}