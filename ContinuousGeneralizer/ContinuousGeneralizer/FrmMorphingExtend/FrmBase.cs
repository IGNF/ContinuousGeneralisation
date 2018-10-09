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

namespace ContinuousGeneralizer.FrmMorphingExtend
{
    public partial class FrmBase : Form
    {
        protected CDataRecords _DataRecords;                    //records of data

        protected CPolyline _CPolyline = new CPolyline();
        protected List<CPolyline> _CPolylineLt = new List<CPolyline>();
        protected CPolyline _RelativeInterpolationCpl;
        protected double _dblProp;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmBase()
        {
            InitializeComponent();
        }

        /// <summary>构造函数</summary>
        public FrmBase(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmBase_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;
            CConstants.strMethod = "Linear";

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
        }

        public virtual void btnRunShp_Click(object sender, EventArgs e)
        {

        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFG = new OpenFileDialog();
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            if (OFG.FileName == null || OFG.FileName == "") return;
            _CPolylineLt = CHelpFuncExcel.InputDataLtXYZT(OFG.FileName);
            _DataRecords.ParameterResult.CInitialPlLt = _CPolylineLt;
            //_CPolyline = CHelpFuncExcel.InputDataXYZT(OFG.FileName);
        }

        public virtual void btnRun_Click(object sender, EventArgs e)
        {

        }

        public virtual void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }
        public virtual void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }

        public virtual void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);

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
                _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
                return;
            }
            pbScale.Value = Convert.ToInt16(100 * _dblProp);
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
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
                _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
                return;
            }
            pbScale.Value = Convert.ToInt16(100 * _dblProp);
            _RelativeInterpolationCpl = CHelpFunc.DisplayInterpolation(_DataRecords, _dblProp);
        }

        public virtual void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CPolyline> cpllt = new List<CPolyline>();
            cpllt.Add(_RelativeInterpolationCpl);
            string strFileName = _dblProp.ToString();
            CHelpFunc.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }


    }
}
