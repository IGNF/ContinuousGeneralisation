using System;
using System.Collections.Generic;
using System.Linq;
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
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;
using MorphingClass.CAid;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmTopologyChecker : Form
    {
        
        private CDataRecords _DataRecords;


        public FrmTopologyChecker()
        {
            InitializeComponent();
        }

        public FrmTopologyChecker(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            pDataRecords.ParameterInitialize.frmCurrentForm = this;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmTopologyChecker_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>(2);
            ParameterInitialize.cboLayerLt.Add(this.cboLargerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboSmallerScaleLayer);

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //if (SFD.FileName == null || SFD.FileName == "") return;
            //ParameterInitialize.strSavePath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);

            CTopologyChecker pTopologyChecker = new CTopologyChecker(ParameterInitialize, Convert.ToInt16(this.txtLayerCount.Text));
            pTopologyChecker.TopologyCheck();
            MessageBox.Show("Done!");

        }


    }
}