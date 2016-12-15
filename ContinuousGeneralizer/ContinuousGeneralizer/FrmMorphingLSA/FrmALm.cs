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
using MorphingClass.CCorrepondObjects;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CMorphingExtend;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmMorphingLSA
{
    public partial class FrmALm : FrmStraightLine
    {
        private CALm _pALm = new CALm();

        public FrmALm()
        {
            //_ParameterInitialize.strMorphingMethod = "LandingTime";
        }

        public FrmALm(CDataRecords pDataRecords)
        {
            this.Name = "ALm";
            this.Text = "ALm";
            pDataRecords.ParameterInitialize.strMorphingMethod = "ALm";
            _DataRecords = pDataRecords;
        }

        public override void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);
            _pALm = new CALm(_DataRecords);
        }

        public override void btn010_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.1;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn020_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.2;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn030_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.3;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn040_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.4;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn050_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.5;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn060_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.6;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn070_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.7;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn080_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.8;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn090_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.9;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn000_Click(object sender, EventArgs e)
        {
            _dblProportion = 0;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn025_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.25;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn075_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.75;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }
        public override void btn100_Click(object sender, EventArgs e)
        {
            _dblProportion = 1;
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }

        public override void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProportion = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);

        }

        public override void btnReduce_Click(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion - 0.01;
            if (_dblProportion < 0)
            {
                _dblProportion = 0;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
                return;
            }
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }

        public override void btnAdd_Click(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion + 0.01;
            if (_dblProportion > 1)
            {
                _dblProportion = 1;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
                return;
            }
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
            _RelativeInterpolationCpl = _pALm.DisplayInterpolation(_dblProportion);
        }

        public override void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CPolyline> cpllt = new List<CPolyline>();
            cpllt.Add(_RelativeInterpolationCpl);
            string strFileName = _dblProportion.ToString();
            CHelperFunction.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }






    }
}
