﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;

using ContinuousGeneralizer;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeneralizationMethods;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmGeneralization
{
    public partial class FrmBldgGrow : Form
    {
        private CDataRecords _DataRecords;                    //records of data
        


        private CBldgGrow _pBldgGrow;


        private double _dblProp = 0.5;

        public FrmBldgGrow()
        {
            InitializeComponent();
        }

        public FrmBldgGrow(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            pDataRecords.ParameterInitialize.frmCurrentForm = this;
        }

        private void FrmBldgGrow_Load(object sender, EventArgs e)
        {
            var ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>(4);
            ParameterInitialize.cboLayerLt.Add(this.cboLargerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboSmallerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboLSRoad);
            ParameterInitialize.cboLayerLt.Add(this.cboSSRoad);
            CConstants.strMethod = "BldgGrow";

            this.cboBufferStyle.SelectedIndex = 0;
            this.cboSimplification.SelectedIndex = 2;


            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
        }



        public void btnRun_Click(object sender, EventArgs e)
        {

            var ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.tsslMessage.Text = this.Name + ": Computing...";
            //Read Datasets
            _pBldgGrow = new CBldgGrow(ParameterInitialize);
            _pBldgGrow.BldgGrow(
                this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text),
                this.cboSimplification.Text,
                Convert.ToDouble(this.txtLargerScale.Text),Convert.ToDouble(this.txtSmallerScale.Text),
                Convert.ToInt32(this.txtOutput.Text));

            ParameterInitialize.tsslMessage.Text = this.Name + ": Ready!";
        }

        private void btnResultFolder_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@_DataRecords.ParameterInitialize.strSavePath);
        }



        public void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
    this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
    this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text, 
                this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
    this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
    this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
    this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
        public void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
    this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _pBldgGrow.Output(_dblProp, this.cboSimplification.Text,
    this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                _dblProp = _dblProp - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                //_pBldgGrow.Output(_dblProp);
            }
            catch (System.Exception)
            {
                MessageBox.Show("不能再减小了！");
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _dblProp = _dblProp + 0.02;
            pbScale.Value = Convert.ToInt16(100 * _dblProp);
            //_pBldgGrow.Output(_dblProp);
        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //CParameterResult ParameterResult = _DataRecords.ParameterResult;
            //string strFileName = _dblProp.ToString();
            //CHelpFunc.SaveCPlLt(ParameterResult.DisplayCPlLt, strFileName+"_morphing", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCPlLt(ParameterResult.FadedDisplayCPlLt, strFileName + "_DPFade", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);





            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //List<CPolyline> cpllt = new List<CPolyline>();
            //for (int i = 0; i < _RelativeInterpolationCplLt.Count; i++)
            //{
            //    cpllt.Add(_RelativeInterpolationCplLt[i]);
            //}
            //string strFileName = _dblProp.ToString();
            //CHelpFunc.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }

        public void btnMultiResults_Click(object sender, EventArgs e)
        {
            int intCount = 10;
            var pStopwatch = Stopwatch.StartNew();
            for (int i = 1; i < intCount + 1; i++)
            {
                _dblProp = Convert.ToDouble(i) / intCount;
                _pBldgGrow.Output(_dblProp, this.cboSimplification.Text, this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
            }
            CHelpFunc.DisplayRunTime(pStopwatch.ElapsedMilliseconds);
        }

        private void btnDetailToIpe_Click(object sender, EventArgs e)
        {
            _pBldgGrow.MakeAnimations(this.cboSimplification.Text, this.cboBufferStyle.Text, Convert.ToDouble(this.txtMiterLimit.Text));
        }
    }
}
