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
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmMorphingExtend
{
    public partial class FrmOrdinal : FrmBase
    {
        double[] _adblLength0 = new double[0];
        double[] _adblAngle0 = new double[0];
        double[] _adblAngleIntegr0 = new double[0];
        double[] _adblAngleIntegrAbs0 = new double[0];
        Timer _pTimer = new Timer();

        public FrmOrdinal()
        {
            //_ParameterInitialize.strMorphingMethod = "Ordinal";
        }

        public FrmOrdinal(CDataRecords pDataRecords)
        {
            this.Name = "FrmOrdinal";
            this.Text = "FrmOrdinal";
            pDataRecords.ParameterInitialize.strMorphingMethod = "Ordinal";
            _DataRecords = pDataRecords;


            //this.btnRun 
        }

        public override void btnRun_Click(object sender, EventArgs e)
        {

            ////static int ss=0; 
            ////for (int i = 0; i < _CPolylineLt .Count ; i++)
            ////{
            ////    if (_CPolylineLt[i].CptLt [0].Z >_CPolylineLt[i].CptLt [_CPolylineLt[i].CptLt.Count -1].Z)
            ////    {
            ////        CHelperFunction.ViewPolyline(_DataRecords.ParameterInitialize.m_mapControl, _CPolylineLt[i]);
            ////    }   
            ////}

            ////CHelperFunction.ViewPolyline(_DataRecords.ParameterInitialize.m_mapControl, _CPolylineLt[intCount]);
            ////intCount += 1;
            ////int kk = 5;

            ////以最后两个点为基点
            ////_CPolyline = _CPolylineLt[1];
            //for (int i = 0; i < _CPolylineLt.Count; i++)
            //{
            //    if (_CPolylineLt[i].ID == 2450)
            //    {
            //        _CPolyline = _CPolylineLt[i];
            //        break;
            //    }
            //}

            //CPolyline cpl = _CPolyline;
            //CHelperFunction.ViewPolyline(_DataRecords.ParameterInitialize.m_mapControl, cpl);
            //int intPtCount = cpl.CptLt.Count;

            ////计算长度初始值（全部计算）
            //_adblLength0 = new double[intPtCount - 1];
            //for (int i = 0; i < intPtCount - 1; i++)
            //{
            //    _adblLength0[i] = CGeometricMethods.CalDis(cpl.CptLt[i], cpl.CptLt[i + 1]);
            //}
            //_dblStandardAxisAngle = CGeometricMethods.CalAxisAngle(cpl.CptLt[cpl.CptLt.Count - 2], cpl.CptLt[cpl.CptLt.Count - 1]);

            //////计算角度初始值
            ////_adblAngle0 = new double[intPtCount - 2];
            ////double dblAngleIntegral = 0;
            ////for (int i = 0; i < intPtCount - 2; i++)
            ////{
            ////    _adblAngle0[i] = CGeometricMethods.CalAngle2(cpl.CptLt[i], cpl.CptLt[i + 1], cpl.CptLt[i + 2]);
            ////    dblAngleIntegral+=_adblAngle0[i];
            ////    _adblAngleIntegr0[i] = dblAngleIntegral;
            ////}

            ////计算角度初始值
            //_adblAngle0 = new double[intPtCount - 2];
            //_adblAngleIntegr0 = new double[intPtCount - 1];  //该数组大小本来应该为"intPtCount -2"，但考虑到后面应用需要，将其大小定义为"intPtCount -1"
            //_adblAngleIntegrAbs0 = new double[intPtCount - 1];  //该数组大小本来应该为"intPtCount -2"，但考虑到后面应用需要，将其大小定义为"intPtCount -1"

            //double dblAngleIntegral = 0;
            //double dblAngleIntegralAbs = 0;            
            //for (int i = intPtCount - 3; i >= 0; i--)
            //{
            //    _adblAngle0[i] = CGeometricMethods.CalAngle2(cpl.CptLt[i], cpl.CptLt[i + 1], cpl.CptLt[i + 2]);
            //    //夹角累计
            //    dblAngleIntegral += _adblAngle0[i] - Math.PI;
            //    _adblAngleIntegr0[i] = dblAngleIntegral;
            //    //夹角累计绝对值
            //    dblAngleIntegralAbs += Math.Abs(_adblAngle0[i] - Math.PI);
            //    _adblAngleIntegrAbs0[i] = dblAngleIntegralAbs;
            //}
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
                CPolyline cpl0 = GetTargetcpl(_dblProportion);
                CHelperFunction.ViewPolyline(_DataRecords.ParameterInitialize.m_mapControl, cpl0);
                return;
            }
            CPolyline cpl1 = GetTargetcpl(_dblProportion);
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);



            CHelperFunction.ViewPolyline(m_mapControl, cpl1);
        }

        public override void timerReduce_Tick(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion - 0.001;
            IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();

            if (_dblProportion < 0)
            {
                timerReduce.Enabled = false;
                _dblProportion = 0;

                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                CPolyline cpl0 = GetTargetcpl(_dblProportion);
                CHelperFunction.ViewPolyline(_DataRecords.ParameterInitialize.m_mapControl, cpl0);
                return;
            }
            CPolyline cpl1 = GetTargetcpl(_dblProportion);
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);



            CHelperFunction.ViewPolyline(m_mapControl, cpl1);

        }



        private CPolyline GetTargetcpl(double dblProportion)
        {
            ////dblProportion = 1;
            //CPolyline cpl = _CPolyline;
            //List<CPoint> cptlt = cpl.CptLt;
            //int intPtCount = cptlt.Count;
            //double dblProAngle = dblProportion * _adblAngleIntegrAbs0[0];   //计算需要旋转的总角度1.796
            //double dblResidual = 0;
            //int intIndex = 0;
            //for (int i = intPtCount-3; i >=0; i--)
            //{
            //    if (_adblAngleIntegrAbs0[i] >= dblProAngle)
            //    {
            //        dblResidual = dblProAngle - _adblAngleIntegrAbs0[i + 1];  //剩余需旋转角度
            //        intIndex = i;
            //        break;
            //    }
            //}

            //double dblStraightLength = 0;
            //for (int i = intIndex + 1; i < intPtCount-1; i++)
            //{
            //    dblStraightLength += _adblLength0[i];
            //}

            //List<CPoint> newcptlt = new List<CPoint>();
            //newcptlt.Insert(0, cptlt[intPtCount - 1]);  //添加最后一个点
            ////计算倒数第二个点
            //double dblRatio=dblStraightLength/_adblLength0 [intPtCount-2];
            //double dblNewX2 = cptlt[intPtCount - 1].X + dblRatio * (cptlt[intPtCount - 2].X - cptlt[intPtCount - 1].X);
            //double dblNewY2 = cptlt[intPtCount - 1].Y + dblRatio * (cptlt[intPtCount - 2].Y - cptlt[intPtCount - 1].Y);
            //CPoint cpt2 = new CPoint(intIndex, dblNewX2, dblNewY2);
            //newcptlt.Insert(0, cpt2);  //添加第二个点

            ////计算平移坐标
            //double dblMoveX = dblNewX2 - cptlt[intIndex + 1].X;
            //double dblMoveY = dblNewY2 - cptlt[intIndex + 1].Y;

            //////计算旋转角度
            ////if (_adblAngle0[intIndex - 1] < Math.PI)
            ////{

            ////}

            //List<CPoint> precptlt = new List<CPoint>();
            //precptlt = cptlt.GetRange(0, intIndex + 2);  //第intIndex个夹角之前(含夹角点)的顶点数量为 intIndex + 2
            //CPolyline precpl = new CPolyline(0, precptlt);
            //ITransform2D pTransform2D = precpl.pPolyline as ITransform2D;
            //pTransform2D.Move(dblMoveX, dblMoveY);

            //double dblTargetAngle = 0;   //目标绝对角
            //if (_adblAngle0[intIndex] > Math.PI)
            //{
            //    dblTargetAngle = 2 * Math.PI - (_adblAngle0[intIndex] - dblResidual - _dblStandardAxisAngle);
            //}
            //else
            //{
            //    dblTargetAngle = 2 * Math.PI - (_adblAngle0[intIndex] + dblResidual - _dblStandardAxisAngle);
            //}

            //double dblOrinialAngle = CGeometricMethods.CalAxisAngle(cptlt[intIndex + 1], cptlt[intIndex]); //原始绝对角
            //double dblCurrentAngle = _adblAngleIntegr0[intIndex + 1] + dblOrinialAngle;  //当前绝对夹角
            //double dblRotationAngle = dblTargetAngle - dblOrinialAngle;  //需旋转角度
            //pTransform2D.Rotate((IPoint)newcptlt[0], dblRotationAngle);
            
            //CPolyline newprecpl = new CPolyline(0, precpl.pPolyline);
            //newcptlt.InsertRange(0, newprecpl.CptLt); 
            //CPolyline newcpl = new CPolyline(0, newcptlt);
            //return newcpl;
            return null;
        }

        //private void InitializeComponent2(CDataRecords pDataRecords)
        //{
        //    this.Name = "FrmOrdinal";
        //    this.Text = "FrmOrdinal";


        //}
    }
}
