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
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;

using MorphingClass.CAid;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmCreatePointLayer : Form
    {
        
        
        CDataRecords _DataRecords;


        public FrmCreatePointLayer()
        {
            InitializeComponent();
        }

        public FrmCreatePointLayer(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmCreatePointLayer_Load(object sender, EventArgs e)
        {
            var ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>();
            ParameterInitialize.cboLayerLt.Add(this.cboLayer);
           
            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            ParameterInitialize.cboLayerLt[0].Items.Add("All visible layers");
            ParameterInitialize.cboLayerLt[0].Text = "All visible layers";
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            var ParameterInitialize = _DataRecords.ParameterInitialize;

            if (ParameterInitialize.cboLayerLt[0].Text == "All visible layers")
            {
                var pFLayerLt = CHelpFunc.GetVisibleLayers(ParameterInitialize);
                var pCreatePointLayer = new CCreatePointLayer(ParameterInitialize, pFLayerLt);
                pCreatePointLayer.CreatePointLayer(Convert.ToDouble(txtSize.Text));
            }
            else
            {
                var pCreatePointLayer = new CCreatePointLayer(ParameterInitialize, ParameterInitialize.cboLayerLt[0].SelectedIndex, 1);
                pCreatePointLayer.CreatePointLayer(Convert.ToDouble(txtSize.Text));
            }
            MessageBox.Show("Done!");
        }




















    }
}