﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.COMSupport;
//using ESRI.ArcGIS.ADF.Resources;
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
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;
using MorphingClass.CAid;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmMergeAndSplitPolylines : Form
    {
        

        CDataRecords _DataRecords;


        public FrmMergeAndSplitPolylines()
        {
            InitializeComponent();
        }

        public FrmMergeAndSplitPolylines(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmMergeAndSplitPolylines_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>();
            ParameterInitialize.cboLayerLt.Add(this.cboLayer);

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            CMergeAndSplitPolylines pMergeAndSplitPolylines = new CMergeAndSplitPolylines(ParameterInitialize);
            pMergeAndSplitPolylines.MergeAndSplitPolylines();

            MessageBox.Show("Done!");
        }

    }
}
