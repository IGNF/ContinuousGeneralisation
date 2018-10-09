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
    public partial class FrmSimultaneity : FrmBase
    {
        static int intCount = 0;
        double[] _adblLength0 = new double[0];
        double[] _adblAngle0 = new double[0];
        Timer _pTimer = new Timer();

        public FrmSimultaneity()
        {
            //CConstants.strMethod = "Simultaneity";
        }

        public FrmSimultaneity(CDataRecords pDataRecords)
        {
            this.Name = "FrmSimultaneity";
            this.Text = "FrmSimultaneity";
            CConstants.strMethod = "Simultaneity";
            _DataRecords = pDataRecords;
            

            //this.btnRun 
        }

        public override void btnRun_Click(object sender, EventArgs e)
        {
            //List<int> intSetoff = new List<int>();
            //for (int i = 0; i < _CPolylineLt.Count; i++)
            //{
            //    if (_CPolylineLt[i].CptLt[0].Z > _CPolylineLt[i].CptLt[_CPolylineLt[i].CptLt.Count - 1].Z)
            //    {
            //        CHelpFunc.ViewPolyline(_DataRecords.ParameterInitialize.m_mapControl, _CPolylineLt[i]);
            //    }
            //    else
            //    {
            //        intSetoff.Add(_CPolylineLt[i].ID);
            //    }
            //}

            //return;
            ////以最后两个点为基点
            ////_CPolyline = _CPolylineLt[1];
            ////for (int i = 0; i < _CPolylineLt.Count; i++)
            ////{
            ////    if (_CPolylineLt[i].ID==10)
            ////    {
            ////        _CPolyline = _CPolylineLt[i];
            ////        break;
            ////    }
            ////}

            //CPolyline cpl = _CPolyline;
            //CHelpFunc.ViewPolyline(_DataRecords.ParameterInitialize.m_mapControl, cpl);
            //int intPtCount = cpl.CptLt.Count;

            ////计算长度初始值（全部计算）
            //_adblLength0 = new double[intPtCount - 2];
            //for (int i = 0; i < intPtCount - 2; i++)
            //{
            //    _adblLength0[i] = CGeoFunc.CalDis(cpl.CptLt[i], cpl.CptLt[i + 1]); 
            //}


            ////计算角度初始值
            //_adblAngle0 = new double[intPtCount - 2];
            //for (int i = 0; i < intPtCount - 2; i++)
            //{
            //    _adblAngle0[i] = CGeoFunc.CalAngle_Counterclockwise(cpl.CptLt[i], cpl.CptLt[i + 1], cpl.CptLt[i + 2]);                
            //}

        }

        public override void timerAdd_Tick(object sender, EventArgs e)
        {
            _dblProp = _dblProp + 0.001;
            IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();

            if (_dblProp > 1)
            {
                timerAdd.Enabled = false;
                _dblProp = 1;

                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                CPolyline cpl0 = GetTargetcpl(_dblProp);
                CHelpFunc.ViewPolyline(_DataRecords.ParameterInitialize.m_mapControl, cpl0);
                return;
            }
            CPolyline cpl1 = GetTargetcpl(_dblProp);
            pbScale.Value = Convert.ToInt16(100 * _dblProp);


           
            CHelpFunc.ViewPolyline(m_mapControl, cpl1);
        }
        
        public override void timerReduce_Tick(object sender, EventArgs e)
        {
            _dblProp = _dblProp - 0.001;
            IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();

            if (_dblProp <0)
            {
                timerReduce.Enabled = false;
                _dblProp = 0;

                pbScale.Value = Convert.ToInt16(100 * _dblProp);
                CPolyline cpl0 = GetTargetcpl(_dblProp);
                CHelpFunc.ViewPolyline(_DataRecords.ParameterInitialize.m_mapControl, cpl0);
                return;
            }
            CPolyline cpl1 = GetTargetcpl(_dblProp);
            pbScale.Value = Convert.ToInt16(100 * _dblProp);


            
            CHelpFunc.ViewPolyline(m_mapControl, cpl1);

        }
       


        private CPolyline GetTargetcpl(double dblProp)
        {
            //CPolyline cpl = _CPolyline;
            //List<CPoint> newcptlt = new List<CPoint>();
            //newcptlt.Insert(0, cpl.CptLt[cpl.CptLt.Count - 1]);
            //newcptlt.Insert(0, cpl.CptLt[cpl.CptLt.Count - 2]);
            //double dblPreAxisAngle = CGeoFunc.CalAxisAngle(cpl.CptLt[cpl.CptLt.Count - 2], cpl.CptLt[cpl.CptLt.Count - 1]);   //前一个线段的绝对角
            //for (int i = cpl.CptLt.Count - 3; i >= 0; i--)
            //{
            //    //目标夹角大小
            //    double dblAngle = (1 - dblProp) * _adblAngle0[i] + dblProp * Math.PI;
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
            //    //新坐标
            //    double dblNewX = newcptlt[0].X + _adblLength0[i] * Math.Cos(dblCurAxisAngle);
            //    double dblNewY = newcptlt[0].Y + _adblLength0[i] * Math.Sin(dblCurAxisAngle);
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
            //return newcpl;
            return null;
        }

        //private void InitializeComponent2(CDataRecords pDataRecords)
        //{
        //    this.Name = "FrmSimultaneity";
        //    this.Text = "FrmSimultaneity";


        //}
    }
}
