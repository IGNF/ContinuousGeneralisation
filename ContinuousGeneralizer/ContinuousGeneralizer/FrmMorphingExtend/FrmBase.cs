﻿using System;
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
        protected CFrmOperation _FrmOperation;
        protected CDataRecords _DataRecords;                    //数据记录

        protected CPolyline _CPolyline = new CPolyline();
        protected List<CPolyline> _CPolylineLt = new List<CPolyline>();
        protected CPolyline _RelativeInterpolationCpl;
        protected double _dblProportion;

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
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;
            CConstants.strMethod = "Linear";

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoadMulticbo();
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
            _CPolylineLt = CHelperFunctionExcel.InputDataLtXYZT(OFG.FileName);
            _DataRecords.ParameterResult.CInitialPlLt = _CPolylineLt;
            //_CPolyline = CHelperFunctionExcel.InputDataXYZT(OFG.FileName);
        }

        public virtual void btnRun_Click(object sender, EventArgs e)
        {

        }

        public virtual void btn010_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.1;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn020_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.2;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn030_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.3;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn040_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.4;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn050_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.5;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn060_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.6;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn070_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.7;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn080_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.8;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn090_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.9;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn000_Click(object sender, EventArgs e)
        {
            _dblProportion = 0;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn025_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.25;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn075_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.75;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }
        public virtual void btn100_Click(object sender, EventArgs e)
        {
            _dblProportion = 1;
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }

        public virtual void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProportion = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);

        }



        private void btnAdd_Click(object sender, EventArgs e)
        {
            this.timerAdd.Enabled = true;
            this.timerReduce.Enabled = false;
        }
        public virtual void timerAdd_Tick(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion + 0.01;
            if (_dblProportion > 1)
            {
                this.timerAdd.Enabled = false;
                _dblProportion = 1;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
                return;
            }
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            this.timerAdd.Enabled = false;
            this.timerReduce.Enabled = true;
        }
        public virtual void timerReduce_Tick(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion - 0.01;
            if (_dblProportion < 0)
            {
                this.timerReduce.Enabled = false;
                _dblProportion = 0;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
                return;
            }
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
            _RelativeInterpolationCpl = CHelperFunction.DisplayInterpolation(_DataRecords, _dblProportion);
        }

        public virtual void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CPolyline> cpllt = new List<CPolyline>();
            cpllt.Add(_RelativeInterpolationCpl);
            string strFileName = _dblProportion.ToString();
            CHelperFunction.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }


    }
}