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
using MorphingClass.CCorrepondObjects;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CMorphingExtend;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmMorphingLSA
{
    public partial class FrmLMulti : FrmALLMulti
    {
        private CIntervalALMulti _pLMulti = new CIntervalALMulti();
        public FrmLMulti()
        {
            InitializeComponent();
        }

        public FrmLMulti(CDataRecords pDataRecords)
        {
            InitializeComponent();

            CParameterInitialize ParameterInitialize = pDataRecords.ParameterInitialize;
            
            ParameterInitialize.m_mapPolyline = new  MapClass ();
            
            ParameterInitialize.cboLayer  = this.cboLayer;
            ParameterInitialize.txtInterpolationNum = this.txtInterpolatedNum;
            ParameterInitialize.txtIterationNum = this.txtIterationNum;
            CConstants.strMethod = "LMulti";
            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
            _DataRecords = pDataRecords;
           
        }

        public override void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);
            _pLMulti = new CIntervalALMulti(_DataRecords);
            _pLMulti.IntervalALMultiMorphing();

            CHelpFunc.SaveCPlLt(_DataRecords.ParameterResult.CResultPlLt, "LMulti", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }
    }
}
