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
using MorphingClass.CAid;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CCorrepondObjects;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmUnifyIndicesPolylines : Form
    {

        

        CDataRecords _DataRecords;

        public FrmUnifyIndicesPolylines()
        {
            InitializeComponent();
        }




        public FrmUnifyIndicesPolylines(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmUnifyIndicesPolylines_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>(2);
            ParameterInitialize.cboLayerLt.Add(this.cboLargerScaleLayer);
            ParameterInitialize.cboLayerLt.Add(this.cboSmallerScaleLayer);

            this.cboBuffer.SelectedIndex = 0;
            ParameterInitialize.cboBuffer = this.cboBuffer;
            ParameterInitialize.txtOverlapRatio = this.txtOverlapRatio;

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            CUnifyIndicesPolylines pUnifyIndicesPolylines = new CUnifyIndicesPolylines(ParameterInitialize);
            pUnifyIndicesPolylines.UnifyIndicesPolylines();

            MessageBox.Show("Done!");
        }



    }
}
