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
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CMorphingExtend;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmMorphingExtend
{
    public partial class FrmLandingTimeLI : FrmBase
    {
        private double[] _adblFrLength = new double[0];
        private double[] _adblFrAngle = new double[0];
        private double[] _adblToLength = new double[0];
        private double[] _adblToAngle = new double[0];
        private CPolyline _FrCpl = new CPolyline();
        private CPolyline _ToCpl = new CPolyline();
        private Timer _pTimer = new Timer();


        public FrmLandingTimeLI()
        {
            //CConstants.strMethod = "LandingTimeLI";
        }

        public FrmLandingTimeLI(CDataRecords pDataRecords)
        {
            this.Name = "FrmLandingTimeLI";
            this.Text = "FrmLandingTimeLI";
            CConstants.strMethod = "LandingTimeLI";
            _DataRecords = pDataRecords;


            //this.btnRun 
        }


        public override void btnRunShp_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //ParameterInitialize.strSavePath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);

            //CLandingTimeLI pLandingTimeLI = new CLandingTimeLI(ParameterInitialize);
            //_FrCpl = pLandingTimeLI._LSCPlLt[0];
            //_ToCpl = pLandingTimeLI._SSCPlLt[0];

            //CLinearInterpolationA pLinearInterpolationA = new CLinearInterpolationA();
            //List<CPoint> Resultcptlt = pLinearInterpolationA.CLI(_FrCpl, _ToCpl);
            //List<CCorrCpts> pCorrCptsLt = CHelperFunction.TransferResultptltToCorrCptsLt(Resultcptlt);
            //_DataRecords.ParameterResult.CCorrCptsLt = pCorrCptsLt;

            //int intPtNum = pCorrCptsLt.Count;

            ////计算长度初始值（全部计算）    
            //_adblFrLength = new double[intPtNum - 2];
            //_adblFrAngle = new double[intPtNum - 2];
            //_adblToLength = new double[intPtNum - 2];
            //_adblToAngle = new double[intPtNum - 2];

            //for (int j = 0; j < intPtNum - 2; j++)
            //{
            //    _adblFrLength[j] = CGeometricMethods.CalDis(pCorrCptsLt[j].FrCpt, pCorrCptsLt[j + 1].FrCpt);
            //    _adblToLength[j] = CGeometricMethods.CalDis(pCorrCptsLt[j].ToCpt, pCorrCptsLt[j + 1].ToCpt);

            //    _adblFrAngle[j] = CGeometricMethods.CalAngle_Counterclockwise(pCorrCptsLt[j].FrCpt, pCorrCptsLt[j + 1].FrCpt, pCorrCptsLt[j + 2].FrCpt);
            //    _adblToAngle[j] = CGeometricMethods.CalAngle_Counterclockwise(pCorrCptsLt[j].ToCpt, pCorrCptsLt[j + 1].ToCpt, pCorrCptsLt[j + 2].ToCpt);
            //}


        }










        public override void btnRun_Click(object sender, EventArgs e)
        {
            //CParameterResult pParameterResult = _DataRecords.ParameterResult;
            //CLandingTime pLandingTime = new CLandingTime(_DataRecords);
            //pLandingTime.GetLandingTimeCPlLt();


            //for (int i = 0; i < pParameterResult.CInitialPlLt.Count; i++)
            //{
            //    if (pParameterResult.CInitialPlLt[i].ID == 2026)
            //    {
            //        CPolyline FrCpl = pParameterResult.CInitialPlLt[i];
            //        CPolyline ToCpl = pParameterResult.CResultPlLt[i];
            //        pParameterResult.FromCpl = FrCpl;
            //        pParameterResult.ToCpl = ToCpl;
            //        pParameterResult.CCorrCptsLt = pLandingTime.GetCorrCptsLt(FrCpl, ToCpl);

            //        int intPtNum = FrCpl.CptLt .Count ;

            //        //计算长度初始值（全部计算）    
            //        _adblFrLength = new double[intPtNum - 2];
            //        _adblFrAngle = new double[intPtNum - 2];
            //        _adblToLength = new double[intPtNum - 2];
            //        _adblToAngle = new double[intPtNum - 2];



            //        for (int j = 0; j < intPtNum - 2; j++)
            //        {
            //            _adblFrLength[j] = CGeometricMethods.CalDis(FrCpl.CptLt[j], FrCpl.CptLt[j + 1]);
            //            _adblToLength[j] = CGeometricMethods.CalDis(ToCpl.CptLt[j], ToCpl.CptLt[j + 1]);

            //            _adblFrAngle[j] = CGeometricMethods.CalAngle_Counterclockwise(FrCpl.CptLt[j], FrCpl.CptLt[j + 1], FrCpl.CptLt[j + 2]);
            //            _adblToAngle[j] = CGeometricMethods.CalAngle_Counterclockwise(ToCpl.CptLt[j], ToCpl.CptLt[j + 1], ToCpl.CptLt[j + 2]);
            //        }

            //        break;
            //    }
            //}            


            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //if (SFD.FileName == null || SFD.FileName == "") return;
            //ParameterInitialize.strSavePath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);





        }



        public override void btn010_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.1;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn020_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.2;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn030_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.3;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn040_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.4;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn050_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.5;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn060_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.6;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn070_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.7;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn080_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.8;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn090_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.9;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn000_Click(object sender, EventArgs e)
        {
            _dblProportion = 0;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn025_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.25;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn075_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.75;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        public override void btn100_Click(object sender, EventArgs e)
        {
            _dblProportion = 1;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }

        public override void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProportion = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }




        public override void timerAdd_Tick(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion + 0.001;
            IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();

            if (_dblProportion > 1)
            {
                timerAdd.Enabled = false;
                _dblProportion = 1;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                CPolyline cpl0 = DisplayInterpolation(_dblProportion,m_mapControl);
                return;
            }
            CPolyline cpl1 = DisplayInterpolation(_dblProportion, m_mapControl);
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
        }

        public override void timerReduce_Tick(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion - 0.001;
            IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();

            if (_dblProportion < 0)
            {
                timerAdd.Enabled = false;
                _dblProportion =0;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                CPolyline cpl0 = DisplayInterpolation(_dblProportion, m_mapControl);
                return;
            }
            CPolyline cpl1 = DisplayInterpolation(_dblProportion, m_mapControl);
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
        }



        private CPolyline DisplayInterpolation(double dblProportion, IMapControl4 m_mapControl)
        {
            //List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;
            //int intPtNum = pCorrCptsLt.Count;

            //List<CPoint> newcptlt = new List<CPoint>();
            ////先确定最后两个点
            //double dblLastX = (1 - dblProportion) * pCorrCptsLt[intPtNum - 1].FrCpt.X + dblProportion * pCorrCptsLt[intPtNum - 1].ToCpt.X;
            //double dblLastY = (1 - dblProportion) * pCorrCptsLt[intPtNum - 1].FrCpt.Y + dblProportion * pCorrCptsLt[intPtNum - 1].ToCpt.Y;
            //CPoint newlastcpt = new CPoint(intPtNum - 1, dblLastX, dblLastY);
            //double dblLast1X = (1 - dblProportion) * pCorrCptsLt[intPtNum - 2].FrCpt.X + dblProportion * pCorrCptsLt[intPtNum - 2].ToCpt.X;
            //double dblLast1Y = (1 - dblProportion) * pCorrCptsLt[intPtNum - 2].FrCpt.Y + dblProportion * pCorrCptsLt[intPtNum - 2].ToCpt.Y;
            //CPoint newlast1cpt = new CPoint(intPtNum - 2, dblLast1X, dblLast1Y);
            //newcptlt.Insert(0, newlastcpt);
            //newcptlt.Insert(0, newlast1cpt);

            //double dblPreAxisAngle = CGeometricMethods.CalAxisAngle(newlast1cpt, newlastcpt);   //前一个线段的绝对角
            //for (int i = pCorrCptsLt.Count - 3; i >= 0; i--)
            //{
            //    //目标夹角大小
            //    double dblAngle = (1 - dblProportion) * _adblFrAngle[i] + dblProportion * _adblToAngle[i];
            //    //绝对角
            //    double dblCurAxisAngle = dblPreAxisAngle - dblAngle;      //当前线段的绝对角(0到2*PI)
            //    if (dblCurAxisAngle >= 2 * Math.PI)
            //    {
            //        dblCurAxisAngle = dblCurAxisAngle - 2 * Math.PI;
            //    }
            //    else if (dblCurAxisAngle < 0)
            //    {
            //        dblCurAxisAngle = dblCurAxisAngle + 2 * Math.PI;
            //    }

            //    //目标长度
            //    double dblLength = (1 - dblProportion) * _adblFrLength[i] + dblProportion * _adblToLength[i];
            //    //新坐标
            //    double dblNewX = newcptlt[0].X + dblLength * Math.Cos(dblCurAxisAngle);
            //    double dblNewY = newcptlt[0].Y + dblLength * Math.Sin(dblCurAxisAngle);
            //    CPoint newcpt = new CPoint(i, dblNewX, dblNewY);
            //    newcptlt.Insert(0, newcpt);
            //    //更新dblPreAxisAngle(将向量旋转180°)
            //    if (dblCurAxisAngle >= Math.PI)
            //    {
            //        dblPreAxisAngle = dblCurAxisAngle - Math.PI;
            //    }
            //    else
            //    {
            //        dblPreAxisAngle = dblCurAxisAngle + Math.PI;
            //    }
            //}
            //CPolyline newcpl = new CPolyline(0, newcptlt);

            //IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            //pGra.DeleteAllElements();
            //CHelperFunction.ViewPolyline(m_mapControl, newcpl);
            //return newcpl;
            return null;
        }

  





    }
}
