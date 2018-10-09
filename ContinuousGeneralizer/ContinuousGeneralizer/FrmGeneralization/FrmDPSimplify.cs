using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
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
using MorphingClass.CGeometry;
using MorphingClass.CGeneralizationMethods;

namespace ContinuousGeneralizer.FrmGeneralization
{
    public partial class FrmDPSimplify : Form
    {

        
        
        private CDataRecords _DataRecords;
        private CDPSimplify _pDPSimplify;
        private bool isDivideByDP = false;

        public FrmDPSimplify()
        {
            InitializeComponent();
        }

        public FrmDPSimplify(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmDPSimplify_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>();
            ParameterInitialize.cboLayerLt.Add(this.cboLayer);
            CConstants.strMethod = "DPSimplify";

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
        }

        private void btnDivideByDP_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            _pDPSimplify = new CDPSimplify(ParameterInitialize);
            isDivideByDP = true;
        }

        private void btnSimplify_Click(object sender, EventArgs e)
        {
            if (isDivideByDP == false)
            {
                MessageBox.Show("please press DivideByDP first!");
                return;
            }
            
            double dblParameter = Convert.ToDouble(txtParameter.Text);
            if (rdoDistance.Checked == true)
            {
                _pDPSimplify.DPSimplify(dblParameter, -1, -1, -1);
            }
            else if (rdoRemainRatio.Checked == true)
            {
                _pDPSimplify.DPSimplify(-1, dblParameter, -1, -1);
            }
            else if (rdoRemainNum.Checked == true)
            {
                _pDPSimplify.DPSimplify(-1, -1, dblParameter, -1);
            }

            else if (rdoDeleteNum.Checked == true)
            {
                _pDPSimplify.DPSimplify(-1, -1, -1, dblParameter);
            }


            //we are saving memory here, one can do better
            var CResultsPlLt = _pDPSimplify.ParameterResult.CResultPlLt;
            CSaveFeature.SaveCplEb(CResultsPlLt, "DPSimplify_" + dblParameter.ToString());

        }

        private void btnDPMorph_Click(object sender, EventArgs e)
        {
            if (isDivideByDP == false)
            {
                MessageBox.Show("please press DivideByDP first!");
                return;
            }

            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            double dblProp = Convert.ToDouble(txtProportion.Text);
            double dblParameter = Convert.ToDouble(txtParameter.Text);
            if (rdoDistance.Checked == true)
            {
                _pDPSimplify.DPMorph(dblParameter, -1, -1, dblProp);
            }
            else if (rdoRemainRatio.Checked == true)
            {
                _pDPSimplify.DPMorph(-1, dblParameter, -1, dblProp);
            }
            else if (rdoRemainNum.Checked == true)
            {
                _pDPSimplify.DPMorph(-1, -1, dblParameter, dblProp);
            }

            //_pDPSimplify.DPSimplify(-1, 0.8, -1);
            CHelpFunc.SaveCPlLt(_pDPSimplify.ParameterResult.CResultPlLt, "DPMorph" + "_" + dblParameter.ToString() 
                + "_" + dblProp.ToString(), ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);


            _DataRecords.ParameterResult = _pDPSimplify.ParameterResult;
        }






    }
}
