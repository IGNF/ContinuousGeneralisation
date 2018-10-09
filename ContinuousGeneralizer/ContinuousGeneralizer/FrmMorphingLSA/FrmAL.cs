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
    public partial class FrmAL : ContinuousGeneralizer.FrmMorphingLSA.FrmStraightLine
    {
        private CAL _pAL = new CAL();

        public FrmAL()
        {
            //CConstants.strMethod = "LandingTime";
        }

        public FrmAL(CDataRecords pDataRecords)
        {
            InitializeComponent();
            CConstants.strMethod = "AL";
            pDataRecords.ParameterInitialize.txtIterationNum = this.txtIterationNum;
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
            _pAL = new CAL(_DataRecords);
            //_pAL.intIterationNum = Convert.ToInt32(this.txtIterationNum.Text);     //the maximum itrative times
            //List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;
            //List <CPoint > cptlt=new List<CPoint> ();
            //for (int i = 0; i < pCorrCptsLt.Count ; i++)
            //{
            //    cptlt.Add(pCorrCptsLt[i].FrCpt);
            //}
            //_RelativeInterpolationCpl = new CPolyline(0, cptlt);

            _RelativeInterpolationCpl = _DataRecords.ParameterResult.FromCpl;
            _DataRecords.ParameterResult.CResultPlLt = new List<CPolyline>();
        }

        public override void btn010_Click(object sender, EventArgs e)
        {
            _dblProp = 0.1;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn020_Click(object sender, EventArgs e)
        {
            _dblProp = 0.2;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn030_Click(object sender, EventArgs e)
        {
            _dblProp = 0.3;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn040_Click(object sender, EventArgs e)
        {
            _dblProp = 0.4;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn050_Click(object sender, EventArgs e)
        {
            _dblProp = 0.5;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn060_Click(object sender, EventArgs e)
        {
            _dblProp = 0.6;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn070_Click(object sender, EventArgs e)
        {
            _dblProp = 0.7;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn080_Click(object sender, EventArgs e)
        {
            _dblProp = 0.8;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn090_Click(object sender, EventArgs e)
        {
            _dblProp = 0.9;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn000_Click(object sender, EventArgs e)
        {
            _dblProp = 0;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn025_Click(object sender, EventArgs e)
        {
            _dblProp = 0.25;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn075_Click(object sender, EventArgs e)
        {
            _dblProp = 0.75;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }
        public override void btn100_Click(object sender, EventArgs e)
        {
            _dblProp = 1;
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
        }

        public override void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProp = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);

        }


        public override void btnReduce_Click(object sender, EventArgs e)
        {
            //if (_dblProp <= 0)
            //{
            //    _dblProp = 0;
            //    pbScale.Value = Convert.ToInt16(100 * _dblProp);
            //    _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp, _RelativeInterpolationCpl);
            //}
            //else
            //{
            //    pbScale.Value = Convert.ToInt16(100 * _dblProp);
            //    _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp, _RelativeInterpolationCpl);
            //    _dblProp = _dblProp - 0.01;
            //}








            this.timerAdd.Enabled = false;
            this.timerReduce.Enabled = true;
        }

        public override void btnAdd_Click(object sender, EventArgs e)
        {
            //if (_dblProp >= 1)
            //{
            //    _dblProp = 0.9999;
            //    pbScale.Value = Convert.ToInt16(100 * _dblProp);
            //    _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp, _RelativeInterpolationCpl);
            //    _DataRecords.ParameterResult.CResultPlLt.Add(_RelativeInterpolationCpl);
            //}
            //else
            //{
            //    pbScale.Value = Convert.ToInt16(100 * _dblProp);
            //    _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp, _RelativeInterpolationCpl);
            //    _DataRecords.ParameterResult.CResultPlLt.Add(_RelativeInterpolationCpl);
            //    _dblProp = _dblProp + 0.01;
            //}


            this.timerAdd.Enabled = true;
            this.timerReduce.Enabled = false;
            _DataRecords.ParameterResult.CResultPlLt = new List<CPolyline>();
        }

        public override void timerReduce_Tick(object sender, EventArgs e)
        {            
            if (_dblProp < 0)
            {
                this.timerReduce.Enabled = false;
                _dblProp = 0;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
             }
            else
            {
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
                _dblProp = _dblProp - 0.01;
            }            
        }

        public override void timerAdd_Tick(object sender, EventArgs e)
        {            
            if (_dblProp > 1)
            {
                this.timerAdd.Enabled = false;
                _dblProp = 1;
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
                _DataRecords.ParameterResult.CResultPlLt.Add(_RelativeInterpolationCpl);
            }
            else
            {
                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                _RelativeInterpolationCpl = _pAL.DisplayInterpolation(_dblProp,_RelativeInterpolationCpl);
                _DataRecords.ParameterResult.CResultPlLt.Add(_RelativeInterpolationCpl);
                _dblProp = _dblProp + 0.01;
            }           
        }

        public override void btnConvergence_Click(object sender, EventArgs e)
        {
            CPolyline testcpl = new CPolyline(_RelativeInterpolationCpl.ID, _RelativeInterpolationCpl.CptLt);
            List<CPolyline> testresultlt = new List<CPolyline>();
            

            int intIterationNum = 200;
            for (int i = 0; i < 100; i++)
            {
                this.txtIterationNum.Text = intIterationNum.ToString ();
                testresultlt.Add(_pAL.DisplayInterpolation(_dblProp, testcpl));
                intIterationNum++;
            }

            //保存测试结果
            string strFileName = _dblProp.ToString();
            CHelpFunc.SaveCPlLt(testresultlt, strFileName + "test", _DataRecords.ParameterInitialize.pWorkspace, _DataRecords.ParameterInitialize.m_mapControl);

        }

        //public override void btnSaveInterpolation_Click(object sender, EventArgs e)
        //{
        //    CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
        //    List<CPolyline> cpllt = new List<CPolyline>();
        //    cpllt.Add(_RelativeInterpolationCpl);
        //    string strFileName = _dblProp.ToString();
        //    CHelpFunc.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

        //    //保存控制点
        //    CHelpFunc.SaveControlptlt(_RelativeInterpolationCpl.CptLt, "CtrlPt", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

        //}

    }
}
