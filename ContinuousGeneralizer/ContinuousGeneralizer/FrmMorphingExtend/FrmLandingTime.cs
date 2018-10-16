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
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CMorphingExtend;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmMorphingExtend
{
    public partial class FrmLandingTime : FrmMorphingExtend.FrmBase
    {
       private double[] _adblLength0 = new double[0];
       private double[] _adblAngle0 = new double[0];
       private double[] _adblAngleIntegr0 = new double[0];
       private double[] _adblAngleIntegrAbs0 = new double[0];
       private Timer _pTimer = new Timer();
       private CPAL _pCAL = new CPAL();

        public FrmLandingTime()
        {
            //_CConstants.strMethod = "LandingTime";
        }

        public FrmLandingTime(CDataRecords pDataRecords)
        {
            this.Name = "FrmLandingTime";
            this.Text = "FrmLandingTime";
            CConstants.strMethod = "LandingTime";
            _DataRecords = pDataRecords;


            //this.btnRun 
        }

        public override void btnRun_Click(object sender, EventArgs e)
        {
            CParameterResult pParameterResult = _DataRecords.ParameterResult;
            CLandingTime pLandingTime = new CLandingTime(_DataRecords);
            pLandingTime.GetLandingTimeCPlLt();


            for (int i = 0; i < pParameterResult.CInitialPlLt.Count; i++)
            {
                if (pParameterResult.CInitialPlLt[i].ID == 2026)
                {
                    double dblTX = pParameterResult.CInitialPlLt[i].pPolyline.Length / pParameterResult.CInitialPlLt[i].CptLt.Count / 100000;   //计算阈值参数
                    pParameterResult.FromCpl = pParameterResult.CInitialPlLt[i];
                    pParameterResult.ToCpl = pParameterResult.CResultPlLt[i];
                    pParameterResult.CCorrCptsLt = pLandingTime.GetCorrCptsLt(pParameterResult.CInitialPlLt[i], pParameterResult.CResultPlLt[i]);
                    _pCAL = new CPAL(_DataRecords, dblTX);
                    break;
                }
            }

            //_dblTX = FromCpl.pPolyline.Length / FromCpl.CptLt .Count  / 1000000;   //计算阈值参数



            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            if (SFD.FileName == null || SFD.FileName == "") return;
            ParameterInitialize.strSavePath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);





        }



        public override void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }

        public override void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProp, _DataRecords.ParameterInitialize.m_mapControl);
        }

        private CPolyline DisplayInterpolation(double dblProp, IMapControl4 m_mapControl)
        {
            CPolyline cpl = _pCAL.GetTargetcpl(dblProp);

            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            CDrawInActiveView.ViewPolyline(m_mapControl, cpl);
            //CDrawInActiveView.ViewPolyline(m_mapControl, _DataRecords.ParameterResult.FromCpl);
            //CDrawInActiveView.ViewPolyline(m_mapControl, _DataRecords.ParameterResult.ToCpl);


            return cpl;
        }



        public override void timerAdd_Tick(object sender, EventArgs e)
        {
            //_dblProp = _dblProp + 0.001;
            //IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            //IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            //pGra.DeleteAllElements();

            //if (_dblProp > 1)
            //{
            //    timerAdd.Enabled = false;
            //    _dblProp = 1;
            //    pbScale.Value = Convert.ToInt16(100 * _dblProp);
            //    CPolyline cpl0 =_pCAL.CGeoFunc.GetTargetcpl(_dblProp);
            //    CDrawInActiveView.ViewPolyline(m_mapControl, cpl0);
            //    return;
            //}
            //CPolyline cpl1 = _pCAL.CGeoFunc.GetTargetcpl(_dblProp);
            //pbScale.Value = Convert.ToInt16(100 * _dblProp);



            //CDrawInActiveView.ViewPolyline(m_mapControl, cpl1);
        }

        public override void timerReduce_Tick(object sender, EventArgs e)
        {
            //_dblProp = _dblProp - 0.001;
            //IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            //IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            //pGra.DeleteAllElements();

            //if (_dblProp < 0)
            //{
            //    timerReduce.Enabled = false;
            //    _dblProp = 0;

            //    pbScale.Value = Convert.ToInt16(100 * _dblProp);
            //    CPolyline cpl0 = _pCAL.CGeoFunc.GetTargetcpl(_dblProp);
            //    CDrawInActiveView.ViewPolyline(m_mapControl, cpl0);
            //    return;
            //}
            //CPolyline cpl1 = _pCAL.CGeoFunc.GetTargetcpl(_dblProp);
            //pbScale.Value = Convert.ToInt16(100 * _dblProp);



            //CDrawInActiveView.ViewPolyline(m_mapControl, cpl1);

        }



      
    }
}
