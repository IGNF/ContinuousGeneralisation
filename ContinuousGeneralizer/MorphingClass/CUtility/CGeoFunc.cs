using System;
using System.Diagnostics;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using SCG = System.Collections.Generic;
//using C5;
using Microsoft.Office.Interop;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.esriSystem;

using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry;
using MorphingClass.CEntity;
using MorphingClass.CGeometry.CGeometryBase;
using VBClass;

using ClipperLib;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace MorphingClass.CUtility
{
    public static class CGeoFunc
    {
        ////we do this way because we can avoid the error CPolyline : CPolyBase<CPolyline>
        //public static void CalDistanceParameters<T, CGeo>(T pT) 
        //    where T : CPolyBase<CGeo>
        //    where CGeo : class
        //{
        //    var pTLt = new List<T>(1);
        //    pTLt.Add(pT);
        //    CalDistanceParameters<T, CGeo>(pTLt);
        //}

        //public static void CalDistanceParameters<T>(List<T> pTLt) where T : CPolyBase<T>
        //{
        //    CalDistanceParameters<T, T>(pTLt);
        //}

        public static void CalDistanceParameters<T>(List<T> pTLt, bool blnCalDistanceParameters = true)
            where T : CPolyBase
        {
            double dblMidLength = 0;
            if (blnCalDistanceParameters == true)
            {
                dblMidLength = CalMidLength<T>(pTLt);
            }
            else
            {
                dblMidLength = 1;
            }

            CConstants.dblMidLength = dblMidLength;
            //CConstants.dblSmallDis = dblMidLength / CConstants.dblSmallDisDenominator;
            CConstants.dblVerySmallCoord = dblMidLength / CConstants.dblVerySmallDenominator;
            CConstants.dblVerySmallCoordFixed = CConstants.dblVerySmallCoord;
        }

        /// <summary>计算一个中值</summary>
        /// <param name="CPlLt">大比例尺线段数组</param>
        /// <returns>极小值</returns>
        /// <remarks>计算方法：所有线段长度中值，然后再除以1000</remarks>
        public static double CalMidLength<T>(List<T> pTLt)
            where T : CPolyBase
        {
            int intSegmentCount = pTLt.GetCountCpt<T>() - pTLt.Count;
            List<double> dblLengthLt = new List<double>(intSegmentCount);

            foreach (T pT in pTLt)
            {
                var pPolyBase = pT as CPolyBase;
                pPolyBase.SetEdgeLengthOnToCpt();
                for (int i = 1; i < pPolyBase.CptLt.Count; i++)
                {
                    dblLengthLt.Add(pPolyBase.CptLt[i].dblEdgeLength);
                }
            }
            double dblMidDis = CMathStatistic.CalMid(dblLengthLt);
            CConstants.dblMidLength = dblMidDis;
            return dblMidDis;
        }

        public static double CalLengthForCptEb(IEnumerable<CPoint> cptEb)
        {
            var cptEt = cptEb.GetEnumerator();
            cptEt.MoveNext();
            var lastcpt = cptEt.Current;

            double dblLength = 0;
            while (cptEt.MoveNext())
            {
                var cpt = cptEt.Current;
                dblLength += CalDis(lastcpt, cpt);
                lastcpt = cpt;
            }

            return dblLength;
        }

        //public static double CalLengthForPtEb<T, TOrder>(IEnumerable<T> ptEb, 
        //    Func<T, TOrder> orderFuncX, Func<T, TOrder> orderFuncY)
        //{
        //    var ptEt = ptEb.GetEnumerator();
        //    ptEt.MoveNext();
        //    var lastcpt = ptEt.Current;

        //    double dblLength = 0;
        //    while (ptEt.MoveNext())
        //    {
        //        var cpt = ptEt.Current;
        //        dblLength += CalDis((double)orderFuncX(lastcpt), (double)orderFuncX(lastcpt),
        //            (double)orderFuncX(lastcpt), (double)orderFuncX(lastcpt));
        //        lastcpt = cpt;
        //    }

        //    return dblLength;
        //}

        public static double CalLengthForIntptEb(IEnumerable<IntPoint> cptEb)
        {
            var cptEt = cptEb.GetEnumerator();
            cptEt.MoveNext();
            var lastcpt = cptEt.Current;

            double dblLength = 0;
            while (cptEt.MoveNext())
            {
                var cpt = cptEt.Current;
                dblLength += CalDis(lastcpt.X, lastcpt.Y, cpt.X, cpt.Y);
                lastcpt = cpt;
            }

            return dblLength;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cptEb"></param>
        /// <returns></returns>
        /// <remarks>the first vertex and the last vertex must be the same</remarks>
        public static double CalAreaForCptEb(IEnumerable<CPoint> cptEb)
        {
            var cptEt = cptEb.GetEnumerator();
            cptEt.MoveNext();
            var lastcpt = cptEt.Current;

            double dblArea = 0;
            while (cptEt.MoveNext())
            {
                var cpt = cptEt.Current;
                dblArea += (cpt.X - lastcpt.X) * (cpt.Y + lastcpt.Y);

                lastcpt = cpt;
            }

            return Math.Abs(dblArea) / 2;
        }







        public static double CalDisSquare(CPoint cpt1, CPoint cpt2)
        {
            return CalDisSquare(cpt1.X, cpt1.Y, cpt2.X, cpt2.Y);
        }

        public static double CalDisSquare(double dblx1, double dbly1, double dblx2, double dbly2)
        {
            return (dblx1 - dblx2) * (dblx1 - dblx2) + (dbly1 - dbly2) * (dbly1 - dbly2);
        }


        public static double CalDis(CPoint cpt1, CPoint cpt2)
        {
            return CalDis(cpt1.X, cpt1.Y, cpt2.X, cpt2.Y);
        }

        public static double CalDis(IPoint ipt1, IPoint ipt2)
        {
            return CalDis(ipt1.X, ipt1.Y, ipt2.X, ipt2.Y);
        }

        public static double CalDis(CMoveVector cmv1, CMoveVector cmv2)
        {
            return CalDis(cmv1.X, cmv1.Y, cmv2.X, cmv2.Y);
        }

        public static double CalDis(double dblx1, double dbly1, double dblx2, double dbly2)
        {
            return Math.Sqrt((dblx1 - dblx2) * (dblx1 - dblx2) + (dbly1 - dbly2) * (dbly1 - dbly2));
        }

        //public static double CalDis(double dbldiffx, double dbldiffy)
        //{
        //    return Math.Sqrt(CalSquareSum(dbldiffx, dbldiffy));
        //}

        public static double CalDisPointToLine(CPoint cpt1, CPoint cpt2, CPoint querycpt)
        {
            double ans = 0;
            double a, b, c;

            a = cpt1.DistanceTo(cpt2);
            b = cpt1.DistanceTo(querycpt);
            c = cpt2.DistanceTo(querycpt);

            //double xx = c + b;
            //double diff = c + b - a;
            if (c + b <= a)  //we use "<" to handle digital problems
            {//点在线段上
                ans = 0;
                return ans;
            }

            if (a == 0)
            {//不是线段，是一个点
                ans = b;
                return ans;
            }

            // 组成锐角三角形，则求三角形的高
            double p0 = (a + b + c) / 2;// 半周长
            //double pa = p0 - a;
            //double pb = p0 - b;
            //double pc = p0 - c;
            double ps = p0 * (p0 - a) * (p0 - b) * (p0 - c);
            double s = Math.Sqrt(p0 * (p0 - a) * (p0 - b) * (p0 - c));// 海伦公式求面积
            ans = 2 * s / a;// 返回点到线的距离（利用三角形面积公式求高）
            return ans;
        }

 
        public static CptEdgeDis CalDisBetweenCptCEdge(CPoint cpt, CEdge cedge, bool blnHeight = false, CEdge thisCEdge = null)
        {
            cedge.JudgeAndSetLength();
            CPoint projection;
            double t;
            if (cedge.dblLength == 0)
            {
                projection = cedge.FrCpt;
                t = 0;
            }
            else
            {
                t = CGeoFunc.DotProduct(cedge.FrCpt, cpt, cedge.ToCpt)
                    / (cedge.dblLength * cedge.dblLength);

                if (blnHeight == false)
                {
                    t = Math.Max(0, Math.Min(1, t));
                }

                t = CCmpMethods.SnapValueToTarget(t, 0, CConstants.dblVerySmallConst);
                t = CCmpMethods.SnapValueToTarget(t, 1, CConstants.dblVerySmallConst);


                if (t == 0)
                {
                    projection = cedge.FrCpt;
                }
                else if (t == 1)
                {
                    projection = cedge.ToCpt;
                }
                else
                {
                    projection = CGeoFunc.GetInbetweenCpt(cedge.FrCpt, cedge.ToCpt, t);
                }
            }

            //const vec2 projection = v + t * (w - v);  // Projection falls on the segment
            return new CGeometry.CptEdgeDis
                    (cpt.DistanceTo(projection), t, cpt, thisCEdge, projection, cedge, blnHeight);
        }


        public static void SetCPlScaleEdgeLengthPtBelong(ref List<CPolyline> cpllt, CEnumScale enumScale)
        {
            foreach (CPolyline cpl in cpllt)
            {
                SetCPlScaleEdgeLengthPtBelong(cpl, enumScale);
            }
        }

        public static void SetCPlScaleEdgeLengthPtBelong(CPolyline cpl, CEnumScale enumScale)
        {
            cpl.SetEdgeLengthOnToCpt();
            cpl.enumScale = enumScale;
            cpl.SetCptBelongedCpl();
        }



        //public static void CalAbsRatioLengthsFromStart(List<CPoint> cptlt, 
        //ref double[] adblAbsLengthFromStart, ref double[] adblRatioLengthFromStart)
        //{
        //    CalAbsLengthsFromStart(cptlt, ref adblAbsLengthFromStart);
        //    int intSegmentNum = cptlt.Count - 1;
        //    int intLastIndex = intSegmentNum - 1;
        //    adblRatioLengthFromStart = new double[intSegmentNum];
        //    for (int i = 0; i < intSegmentNum; i++)
        //    {
        //        adblRatioLengthFromStart[i] = adblAbsLengthFromStart[i] / adblAbsLengthFromStart[intLastIndex];
        //    }
        //}

        //public static void CalAbsLengthsFromStart(List<CPoint> cptlt, ref double[] adblAbsLengthFromStart)
        //{
        //    int intSegmentNum = cptlt.Count - 1;
        //    adblAbsLengthFromStart = new double[intSegmentNum];
        //    adblAbsLengthFromStart[0] = CalDis(cptlt[0], cptlt[1]);
        //    for (int i = 1; i < intSegmentNum; i++)
        //    {
        //        adblAbsLengthFromStart[i] = adblAbsLengthFromStart[i - 1] + CalDis(cptlt[i], cptlt[i + 1]);
        //    }
        //}



        public static CPolyline GetTargetcpl(List<CPoint> CResultPtLt, double dblProp)
        {
            List<CPoint> CTargetPtLt = new List<CPoint>(CResultPtLt.Count);
            for (int i = 0; i < CResultPtLt.Count; i++)
            {
                for (int j = 0; j < CResultPtLt[i].CorrespondingPtLt.Count; j++)
                {
                    CPoint cpt = GetInbetweenCpt(CResultPtLt[i], CResultPtLt[i].CorrespondingPtLt[j], dblProp);
                    //if (CResultPtLt[i].CorrespondingPtLt[j].isCtrl ==true )
                    //{
                    //    cpt.isCtrl = true;
                    //}
                    CTargetPtLt.Add(cpt);
                }
            }

            CPolyline cpl = new CPolyline(0, CTargetPtLt);
            return cpl;
        }

        public static CPolyline GetTargetcpl(int intID, List<CPoint> CResultPtLt, double dblProp)
        {
            List<CPoint> CTargetPtLt = new List<CPoint>(CResultPtLt.Count);
            for (int i = 0; i < CResultPtLt.Count; i++)
            {
                for (int j = 0; j < CResultPtLt[i].CorrespondingPtLt.Count; j++)
                {
                    CPoint cpt = GetInbetweenCpt(CResultPtLt[i], CResultPtLt[i].CorrespondingPtLt[j], dblProp);
                    //if (CResultPtLt[i].CorrespondingPtLt[j].isCtrl ==true )
                    //{
                    //    cpt.isCtrl = true;
                    //}
                    CTargetPtLt.Add(cpt);
                }
            }

            CPolyline cpl = new CPolyline(intID, CTargetPtLt);
            return cpl;
        }



        ///// <summary>
        ///// Get Target polylines
        ///// </summary>
        ///// <remarks>before use this function, make sure that the input is InterCPlLt </remarks>
        //public static List<CPolyline> GetTargetcpl(List<CPolyline> InterCPlLt, double dblProp)
        //{
        //    List<CPolyline> TargetCPlLt = new List<CPolyline>(InterCPlLt.Count);
        //    foreach (CPolyline intercpl in InterCPlLt)
        //    {
        //        TargetCPlLt.Add(GetTargetcpl(intercpl, dblProp));
        //    }

        //    return TargetCPlLt;
        //}

        public static CPolyline GetTargetcpl(CPolyline InterCPl, double dblProp)
        {
            List<CPoint> InterCPtLt = InterCPl.CptLt;
            List<CPoint> TargetCPtLt = new List<CPoint>(InterCPtLt.Count);

            foreach (CPoint intercpt in InterCPtLt)
            {
                foreach (CPoint corrcpt in intercpt.CorrespondingPtLt)
                {
                    CPoint targetcpt = GetInbetweenCpt(intercpt, corrcpt, dblProp);
                    TargetCPtLt.Add(targetcpt);
                }
            }

            CPolyline cpl = new CPolyline(InterCPl.ID, TargetCPtLt);
            return cpl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pPoint"></param>
        /// <param name="pFeature"></param>
        /// <returns></returns>
        /// <remarks>GetTargetIplWithOutLastPoint </remarks>
        public static IPolyline5 GetTargetIplWOLastPoint(List<CPoint> CResultPtLt, double dblProp)
        {
            IPointCollection4 pCol = new PolylineClass();
            for (int i = 0; i < CResultPtLt.Count; i++)
            {
                for (int j = 0; j < CResultPtLt[i].CorrespondingPtLt.Count; j++)
                {
                    IPoint pPoint = new PointClass();
                    double dblDiffX = CResultPtLt[i].CorrespondingPtLt[j].X - CResultPtLt[i].X;
                    double dblDiffY = CResultPtLt[i].CorrespondingPtLt[j].Y - CResultPtLt[i].Y;
                    double dblNewX = CResultPtLt[i].X + dblDiffX * dblProp;
                    double dblNewY = CResultPtLt[i].Y + dblDiffY * dblProp;
                    pPoint.PutCoords(dblNewX, dblNewY);
                    pCol.AddPoint(pPoint);
                }
            }
            pCol.RemovePoints(pCol.PointCount - 1, 1);
            IPolyline5 ipl = pCol as IPolyline5;
            return ipl;
        }

        /// <summary>
        /// 指定点沿着曲线到起点的距离
        /// </summary>
        /// <param name="pPoint">待计算的点</param>
        /// <param name="pPolyline">点所在的曲线</param>
        /// <param name="blnasRatio">指定计算相对距离还是绝对距离</param>
        /// <returns></returns>
        public static double CalDistanceFromStartPoint(IPolyline5 pPolyline, IPoint pPoint, bool blnasRatio)
        {
            IPoint outPoint = new PointClass();
            double distanceAlongCurve = 0;//该点在曲线上最近的点距曲线起点的距离
            double distanceFromCurve = 0;//该点到曲线的直线距离
            bool bRightSide = false;
            pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, 
                pPoint, blnasRatio, outPoint, ref distanceAlongCurve, ref distanceFromCurve, ref bRightSide);
            return distanceAlongCurve;
        }


        public static CPoint QueryCPointByLength(CPoint querycpt, List<CPoint> targetptlt, ref int intKnownIndex)
        {
            double dblQueryLength = querycpt.dblRatioLengthFromStart * targetptlt[targetptlt.Count - 1].dblAbsLengthFromStart;
            for (int i = intKnownIndex + 1; i < targetptlt.Count; i++)
            {
                if (targetptlt[i].dblAbsLengthFromStart >= dblQueryLength)
                {
                    int intPreIndex = i - 1;
                    intKnownIndex = intPreIndex;

                    double dblRatio = (dblQueryLength - targetptlt[intPreIndex].dblAbsLengthFromStart) / targetptlt[i].dblEdgeLength;
                    return GetInbetweenCpt(targetptlt[intPreIndex], targetptlt[i], dblRatio);
                }
            }
            return null;
        }

        public static CPoint QueryCPointByLength(CPoint querycpt, int intTargetIndex, List<CPoint> targetcptlt)
        {
            double dblRatioOnTargetEdge = (querycpt.dblRatioLengthFromStart - targetcptlt[intTargetIndex - 1].dblRatioLengthFromStart) 
                / (targetcptlt[intTargetIndex].dblRatioLengthFromStart - targetcptlt[intTargetIndex - 1].dblRatioLengthFromStart);
            return GetInbetweenCpt(targetcptlt[intTargetIndex - 1], targetcptlt[intTargetIndex], dblRatioOnTargetEdge);

        }

        //public static void IntegrateStandardVectorCpt(List<CPolyline> cpllt, CPoint StandardVectorCpt)
        //{
        //    foreach (var cpl in cpllt)
        //    {
        //        foreach (var cpt in cpl.CptLt)
        //        {
        //            cpt.X += StandardVectorCpt.X;
        //            cpt.Y += StandardVectorCpt.Y;
        //        }
        //    }
        //}

        public static void IntegrateStandardVectorCpt(List<List<CPoint>> cptltlt, CPoint StandardVectorCpt)
        {
            foreach (var cptlt in cptltlt)
            {
                foreach (var cpt in cptlt)
                {
                    cpt.X += StandardVectorCpt.X;
                    cpt.Y += StandardVectorCpt.Y;
                }
            }
        }



        //public static void RemoveStandardVectorCpt(List<CPolyline> cpllt, CPoint StandardVectorCpt)
        //{
        //    foreach (var cpl in cpllt)
        //    {
        //        RemoveStandardVectorCpt(cpl, StandardVectorCpt);
        //    }
        //}

        public static void RemoveStandardVectorCpt(CPolyline cpl, CPoint StandardVectorCpt)
        {
            foreach (var cpt in cpl.CptLt)
            {
                cpt.X -= StandardVectorCpt.X;
                cpt.Y -= StandardVectorCpt.Y;
            }
        }

        public static void RemoveStandardVectorCpt(List<CPoint> cptlt, CPoint StandardVectorCpt)
        {
            foreach (var cpt in cptlt)
            {
                cpt.X -= StandardVectorCpt.X;
                cpt.Y -= StandardVectorCpt.Y;
            }
        }

        public static double CalInbetweenCptProportion(CPoint cpt, CPoint frcpt, CPoint tocpt)
        {
            double dblXDiff = tocpt.X - frcpt.X;
            if (CCmpMethods.CmpDbl_CoordVerySmall(dblXDiff, 0) != 0)
            {
                return ((cpt.X - frcpt.X) / dblXDiff);
            }
            else
            {
                double dblYDiff = tocpt.Y - frcpt.Y;
                if (CCmpMethods.CmpDbl_CoordVerySmall(dblYDiff, 0) != 0)
                {
                    return ((cpt.Y - frcpt.Y) / dblYDiff);
                }
                else
                {
                    return 0;
                }
            }
        }


        public static CPolyline GetInbetweenCpl(CPolyline frcpl, CPolyline tocpl, double dblProp)
        {
            var frcptlt = frcpl.CptLt;
            var tocptlt = tocpl.CptLt;

            var newcptlt = new List<CPoint>(frcptlt.Count);
            for (int i = 0; i < frcptlt.Count; i++)
            {
                newcptlt.Add(GetInbetweenCpt(frcptlt[i], tocptlt[i], dblProp));
            }

            return new CPolyline(frcpl.ID, newcptlt);
        }

        /// <summary>
        /// if you know the edge from frcpt to tocpt, you can use a function in CEdge class, which is faster
        /// </summary>
        public static CPoint GetInbetweenCpt(CPoint frcpt, CPoint tocpt, double dblProp, int intID = -1)
        {
            double dblInbetweenX = GetInbetweenDbl(frcpt.X, tocpt.X, dblProp);
            double dblInbetweenY = GetInbetweenDbl(frcpt.Y, tocpt.Y, dblProp);

            if (intID == -1)
            {
                intID = frcpt.ID;
            }
            return new CPoint(intID, dblInbetweenX, dblInbetweenY);
        }

        public static double GetInbetweenDbl(double dblfr, double dblto, double dblProp)
        {
            return (dblfr + dblProp * (dblto - dblfr));
        }

        public static CPoint GetInterpolatedCpt(CPoint cpt, CMoveVector pMoveVector, double dblProp)
        {
            double dblTargetX = cpt.X + dblProp * pMoveVector.X;
            double dblTargetY = cpt.Y + dblProp * pMoveVector.Y;

            return new CPoint(cpt.ID, dblTargetX, dblTargetY);
        }

        public static IPoint GetInterpolatedIpt(CPoint cpt, CMoveVector pMoveVector, double dblProp)
        {
            double dblTargetX = cpt.X + dblProp * pMoveVector.X;
            double dblTargetY = cpt.Y + dblProp * pMoveVector.Y;

            IPoint ipt = new PointClass();
            ipt.ID = cpt.ID;
            ipt.PutCoords(dblTargetX, dblTargetY);

            return ipt;
            //return null;
        }

        public static void CalBarycentricCoordinates(CPoint cpt0, 
            CPoint cpt1, CPoint cpt2, CPoint cpt3, out double dblLamda1, out double dblLamda2, out double dblLamda3)
        {
            double dbldetT = (cpt2.Y - cpt3.Y) * (cpt1.X - cpt3.X) + (cpt3.X - cpt2.X) * (cpt1.Y - cpt3.Y);
            dblLamda1 = ((cpt2.Y - cpt3.Y) * (cpt0.X - cpt3.X) + (cpt3.X - cpt2.X) * (cpt0.Y - cpt3.Y)) / dbldetT;
            dblLamda2 = ((cpt3.Y - cpt1.Y) * (cpt0.X - cpt3.X) + (cpt1.X - cpt3.X) * (cpt0.Y - cpt3.Y)) / dbldetT;
            dblLamda3 = 1 - dblLamda1 - dblLamda2;
        }

        public static CPoint CalCartesianCoordinates(CPoint 
            cpt1, CPoint cpt2, CPoint cpt3, double dblLamda1, double dblLamda2, double dblLamda3, int intID = -1)
        {
            double dblX = dblLamda1 * cpt1.X + dblLamda2 * cpt2.X + dblLamda3 * cpt3.X;
            double dblY = dblLamda1 * cpt1.Y + dblLamda2 * cpt2.Y + dblLamda3 * cpt3.Y;
            return new CPoint(intID, dblX, dblY);

        }

        public static void SetEdgeLengthOnToCpt(List<CPoint> cptlt)
        {
            //cptlt[0].dblEdgeLength = 0;
            for (int i = 1; i < cptlt.Count; i++)
            {
                cptlt[i].dblEdgeLength = CalDis(cptlt[i - 1], cptlt[i]);
            }
        }

        public static void CalAbsLengthFromStart(List<CPoint> cptlt)
        {
            cptlt[0].dblAbsLengthFromStart = 0;
            for (int i = 1; i < cptlt.Count; i++)
            {
                cptlt[i].dblAbsLengthFromStart = cptlt[i - 1].dblAbsLengthFromStart + cptlt[i].dblEdgeLength;
            }
        }

        public static void CalRatioLengthFromStart(List<CPoint> cptlt)
        {
            cptlt[0].dblRatioLengthFromStart = 0;
            double dblSumAbsLengthFromStart = cptlt[cptlt.Count - 1].dblAbsLengthFromStart;
            for (int i = 1; i < cptlt.Count; i++)
            {
                cptlt[i].dblRatioLengthFromStart = cptlt[i].dblAbsLengthFromStart / dblSumAbsLengthFromStart;
            }
        }

        /// <summary>calculate the absolute length and the ratio length of a point from the start of the polyline</summary>
        /// <param name="isDP">whether the algorithm is a Dynamic Programming algorithm:
        /// if it is not Dynamic Programming (isDP==false), then we have to calculate the length of every edge;
        /// if it is     Dynamic Programming (isDP==true ), then the lengths of the edges have already been calculated. 
        /// To save time, we don't caculated them again</param>
        /// <remarks></remarks>
        public static void CalAbsAndRatioLengthFromStart(List<CPoint> cptlt, bool isDP)
        {
            cptlt[0].dblRatioLengthFromStart = 0;
            cptlt[0].dblAbsLengthFromStart = 0;


            if (isDP == false)
            {
                for (int i = 1; i < cptlt.Count; i++)
                {
                    cptlt[i].dblAbsLengthFromStart = cptlt[i - 1].dblAbsLengthFromStart + cptlt[i].DistanceTo(cptlt[i - 1]);
                }
            }
            else
            {
                for (int i = 1; i < cptlt.Count; i++)
                {
                    cptlt[i].dblAbsLengthFromStart = cptlt[i - 1].dblAbsLengthFromStart + cptlt[i].dblEdgeLength;
                }
            }

            //adblRatioLength
            double dblSumAbsLengthFromStart = cptlt.Last().dblAbsLengthFromStart;
            for (int i = 1; i < cptlt.Count; i++)
            {
                cptlt[i].dblRatioLengthFromStart = cptlt[i].dblAbsLengthFromStart / dblSumAbsLengthFromStart;
            }
        }

        /// <summary>处理有可能的相交问题</summary>
        /// <param name="pMainCpl">主线段(干流)</param>
        /// <param name="ipl">支线段(支流)</param>
        /// <param name="Tocpt2">目标点</param>
        /// <param name="dblProp">变形参数</param>
        /// <returns>处理完相交问题的线段</returns>
        /// <remarks></remarks>
        private static IPolyline5 DWIntersect(CPolyline pMainCpl, IPolyline5 ipl, CPoint Tocpt2, double dblProp)
        {
            double dblDiffX = Tocpt2.CorrespondingPtLt[0].X - Tocpt2.X;
            double dblDiffY = Tocpt2.CorrespondingPtLt[0].Y - Tocpt2.Y;
            double dbllastX = Tocpt2.X + dblDiffX * dblProp;
            double dbllastY = Tocpt2.Y + dblDiffY * dblProp;

            //指定最后一个点为干流上对应点平移后的点
            IPoint iptlast = new PointClass();
            iptlast.PutCoords(dbllastX, dbllastY);
            IPointCollection4 pColipl = ipl as IPointCollection4;
            pColipl.AddPoint(iptlast);

            //判断是否与干流相交
            ipl = pColipl as IPolyline5;
            IRelationalOperator pRel = ipl as IRelationalOperator;
            bool isCrosses = pRel.Crosses(pMainCpl.pPolyline);  //为了提高运行效率，判断是否相交
            while (isCrosses == true)//每次删除倒数第二个点，直到两线段不再相交
            {
                pColipl.RemovePoints(pColipl.PointCount - 2, 1);
                ipl = pColipl as IPolyline5;
                pRel = ipl as IRelationalOperator;
                isCrosses = pRel.Crosses(pMainCpl.pPolyline);
            }

            //if (isCrosses == true) //如果该河流与其干流相交，则取折线上离“FromPoint”最近的交点到“FromPoint”之间的线段为最终线段
            //{
            //    ITopologicalOperator pTop = ipl as ITopologicalOperator;
            //    IGeometry pGeoIntersect = pTop.Intersect(pMainCpl, esriGeometryDimension.esriGeometry0Dimension);
            //    IPointCollection4 pColIntersect = pGeoIntersect as IPointCollection4;
            //    //如果造成两根线有多个交点，则取折线上离“FromPoint”最近的交点到“FromPoint”之间的线段为最终线段
            //    double dblMinDisFromStart = double.MaxValue;
            //    for (int j = 0; j < pColIntersect.PointCount; j++)
            //    {
            //        double dblDis = CalDistanceFromStartPoint(pColIntersect.get_Point(j), ipl);
            //        if (dblDis < dblMinDisFromStart)
            //        {
            //            dblMinDisFromStart = dblDis;
            //        }
            //    }
            //    ICurve pSubCurve = new PolylineClass();
            //    ipl.GetSubcurve(0, dblMinDisFromStart, false, out pSubCurve);
            //    ipl = pSubCurve as IPolyline5;
            //}

            return ipl;
        }

        /// <summary>计算夹角</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>用余弦的方法求夹角，无法区分夹角的方向</remarks>
        public static double CalAngle(double dblfrDiffX, double dblfrDiffY, double dbltoDiffX, double dbltoDiffY)
        {
            //计算a的平方
            double dblA2 = dblfrDiffX * dblfrDiffX + dblfrDiffY * dblfrDiffY;

            //计算b的平方
            double dblB2 = dbltoDiffX * dbltoDiffX + dbltoDiffY * dbltoDiffY;

            //计算c的平方
            double dblC2 = (dblfrDiffX - dbltoDiffX) * (dblfrDiffX - dbltoDiffX) + 
                (dblfrDiffY - dbltoDiffY) * (dblfrDiffY - dbltoDiffY);

            //计算夹角弧度值
            double dblAcos = (dblA2 + dblB2 - dblC2) / (2 * Math.Sqrt(dblA2) * Math.Sqrt(dblB2));

            if (dblAcos < -1)
            {
                return Math.PI;
            }
            else if (dblAcos < 1)
            {
                return Math.Acos(dblAcos);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>计算夹角</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>用余弦的方法求夹角，无法区分夹角的方向</remarks>
        public static double CalAngle(CPoint frprecpt, CPoint frtocpt, CPoint toprecpt, CPoint totocpt)
        {
            double dblfrDiffX = frtocpt.X - frprecpt.X;
            double dblfrDiffY = frtocpt.Y - frprecpt.Y;
            double dbltoDiffX = totocpt.X - toprecpt.X;
            double dbltoDiffY = totocpt.Y - toprecpt.Y;

            //计算a的平方
            double dblA2 = dblfrDiffX * dblfrDiffX + dblfrDiffY * dblfrDiffY;

            //计算b的平方
            double dblB2 = dbltoDiffX * dbltoDiffX + dbltoDiffY * dbltoDiffY;

            //计算c的平方
            double dblC2 = (dblfrDiffX - dbltoDiffX) * (dblfrDiffX - dbltoDiffX) + 
                (dblfrDiffY - dbltoDiffY) * (dblfrDiffY - dbltoDiffY);

            //计算夹角弧度值
            double dblAcos = (dblA2 + dblB2 - dblC2) / (2 * Math.Sqrt(dblA2) * Math.Sqrt(dblB2));

            if (dblAcos < -1)
            {
                return Math.PI;
            }
            else if (dblAcos < 1)
            {
                return Math.Acos(dblAcos);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>计算夹角</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>用余弦的方法求夹角，无法区分夹角的方向</remarks>
        public static double CalAngle(CPoint precpt, CPoint midcpt, CPoint folcpt)
        {
            //计算a的平方
            double dblfrpreDiffX = precpt.X - midcpt.X;
            double dblfrpreDiffY = precpt.Y - midcpt.Y;
            double dblA2 = dblfrpreDiffX * dblfrpreDiffX + dblfrpreDiffY * dblfrpreDiffY;

            //计算b的平方
            double dblfrfolDiffX = folcpt.X - midcpt.X;
            double dblfrfolDiffY = folcpt.Y - midcpt.Y;
            double dblB2 = dblfrfolDiffX * dblfrfolDiffX + dblfrfolDiffY * dblfrfolDiffY;

            //计算c的平方
            double dblC2 = (dblfrpreDiffX - dblfrfolDiffX) * (dblfrpreDiffX - dblfrfolDiffX) + 
                (dblfrpreDiffY - dblfrfolDiffY) * (dblfrpreDiffY - dblfrfolDiffY);

            //计算夹角弧度值
            double dblAcos = (dblA2 + dblB2 - dblC2) / (2 * Math.Sqrt(dblA2) * Math.Sqrt(dblB2));

            if (dblAcos < -1)
            {
                return Math.PI;
            }
            else if (dblAcos < 1)
            {
                return Math.Acos(dblAcos);
            }
            else
            {
                return 0;
            }
        }

        /// <summary>计算夹角(逆时针为正)</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>the angle from edge (midcpt, precpt) to edge (midcpt, folcpt) in counter-clocwise</remarks>
        public static double CalAngle_Counterclockwise(CPoint precpt, CPoint midcpt, CPoint folcpt)
        {
            double dblpreDiffX = precpt.X - midcpt.X;
            double dblpreDiffY = precpt.Y - midcpt.Y;

            double dblfolDiffX = folcpt.X - midcpt.X;
            double dblfolDiffY = folcpt.Y - midcpt.Y;

            return CalAngle_Counterclockwise(dblpreDiffX, dblpreDiffY, dblfolDiffX, dblfolDiffY);
        }

        /// <summary>计算夹角(逆时针)</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>用求差的方法求夹角，可区分夹角的方向</remarks>
        public static double CalAngle_Counterclockwise(CEdge frcedge, CEdge tocedge)
        {
            return CalAngle_Counterclockwise(frcedge.FrCpt, frcedge.ToCpt, tocedge.FrCpt, tocedge.ToCpt);
        }

        /// <summary>计算夹角(逆时针)</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>用求差的方法求夹角，可区分夹角的方向</remarks>
        public static double CalAngle_Counterclockwise(CPoint prefrcpt, CPoint pretocpt, CPoint folfrcpt, CPoint foltocpt)
        {
            double dblpreDiffX = pretocpt.X - prefrcpt.X;
            double dblpreDiffY = pretocpt.Y - prefrcpt.Y;

            double dblfolDiffX = foltocpt.X - folfrcpt.X;
            double dblfolDiffY = foltocpt.Y - folfrcpt.Y;

            return CalAngle_Counterclockwise(dblpreDiffX, dblpreDiffY, dblfolDiffX, dblfolDiffY); ;
        }

        /// <summary>计算夹角(逆时针)</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>用求差的方法求夹角，可区分夹角的方向</remarks>
        public static double CalAngle_Counterclockwise(double dblX1, double dblY1, 
            double dblX2, double dblY2, double dblX3, double dblY3)
        {
            double dblStartDiffX = dblX1 - dblX2;
            double dblStartDiffY = dblY1 - dblY2;

            double dblEndDiffX = dblX3 - dblX2;
            double dblEndDiffY = dblY3 - dblY2;

            return CalAngle_Counterclockwise(dblStartDiffX, dblStartDiffY, dblEndDiffX, dblEndDiffY);
        }

        /// <summary>compute the angle of two vectors (counter clock-wise)</summary>
        /// <returns>the radian angle</returns>
        /// <remarks>the value is in [0,2*pi)</remarks>
        public static double CalAngle_Counterclockwise(double dblStartDiffX, double dblStartDiffY,
            double dblEndDiffX, double dblEndDiffY)
        {
            //计算始向量与坐标横轴的夹角
            double dblStartAngle = CalAxisAngle(dblStartDiffX, dblStartDiffY);

            //计算末向量与坐标横轴的夹角
            double dblEndAngle = CalAxisAngle(dblEndDiffX, dblEndDiffY);

            return CalAngle_Counterclockwise(dblStartAngle, dblEndAngle);
        }

        public static double CalReversedCEdgeAxisAngle(double dblAngle)
        {
            return CheckPlus2PI(dblAngle - Math.PI);
        }

        /// <summary>
        /// the angle in [0, 360) from dblStartAngle to dblEndAngle according to counter-clockwise
        /// </summary>
        /// <param name="dblStartAngle"></param>
        /// <param name="dblEndAngle"></param>
        /// <returns></returns>
        public static double CalAngle_Counterclockwise(double dblStartAxisAngle, double dblEndAxisAngle)
        {
            return CheckPlus2PI(dblEndAxisAngle - dblStartAxisAngle);
        }

        public static bool IsInbetween_Counterclockwise(double dblStartAxisAngle, double dblAngle,  double dblEndAxisAngle)
        {
            if (dblAngle== dblStartAxisAngle || dblAngle == dblEndAxisAngle)
            {
                return false;
            }

            var dblAngle1 = CGeoFunc.CalAngle_Counterclockwise(dblStartAxisAngle, dblAngle);
            var dblAngle2 = CGeoFunc.CalAngle_Counterclockwise(dblAngle, dblEndAxisAngle);

            if (dblAngle1 + dblAngle2 >= CConstants.dblTwoPI)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        ////the new edge starts from a point which constitues the boundary polygon (cpg)
        //newCEdge.SetAxisAngle();
        //            var dblAngle1 = CGeoFunc.CalAngle_Counterclockwise(
        //               cpg.AxisAngleLt[newCEdge.FrCpt.indexID], newCEdge.dblAxisAngle);
        //var dblAngle2 = CGeoFunc.CalAngle_Counterclockwise(
        //    newCEdge.dblAxisAngle, cpg.ReverseAxisAngleLt[newCEdge.FrCpt.indexID]);

        //            if (dblAngle1 + dblAngle2 > CConstants.dblTwoPI)
        //            {
        //                //the new edge is outside the boundary polygon (cpg)
        //                continue;
        //            }

        ///// <summary>计算向量与坐标横轴的夹角[0, 2*Pi]</summary>
        ///// <returns>夹角弧度值</returns>
        ///// <remarks>为什么不用Math.Atan2，因为该值的范围是[-Pi, Pi]</remarks>
        //public static double CalAxisAngle(CEdge cedge)
        //{
        //    return CalAxisAngle(cedge.FrCpt, cedge.ToCpt);
        //}

        /// <summary>计算向量与坐标横轴的夹角[0, 2*Pi]</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>为什么不用Math.Atan2，因为该值的范围是[-Pi, Pi]</remarks>
        public static double CalAxisAngle(CPoint frcpt, CPoint tocpt)
        {
            double dblX = tocpt.X - frcpt.X;
            double dblY = tocpt.Y - frcpt.Y;

            return CalAxisAngle(dblX, dblY);
        }

        /// <summary>计算向量与坐标横轴的夹角[0, 2*Pi]</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>为什么不用Math.Atan2，因为该值的范围是[-Pi, Pi]</remarks>
        public static double CalAxisAngle(double dblfrX, double dblfrY, double dbltoX, double dbltoY)
        {
            double dblX = dbltoX - dblfrX;
            double dblY = dbltoY - dblfrY;

            return CalAxisAngle(dblX, dblY);
        }

        /// <summary>计算向量与坐标横轴的夹角[0, 2*Pi)</summary>
        /// <returns>夹角弧度值</returns>
        /// <remarks>Math.Atan2, [-Pi, Pi]</remarks>
        public static double CalAxisAngle(double dblX, double dblY)
        {
            return CheckPlus2PI(Math.Atan2(dblY, dblX));
        }

        public static double CheckPlus2PI(double dblAxisAngle)
        {
            dblAxisAngle= CCmpMethods.SnapValueToTarget(dblAxisAngle, 0, CConstants.dblVerySmallConst);

            if (dblAxisAngle < 0)
            {
                dblAxisAngle += CConstants.dblTwoPI;
            }
            return dblAxisAngle;
        }

        ///// <summary>确定缓冲区半径大小</summary>
        ///// <param name="CPlLt">线数组</param>
        ///// <returns>缓冲区半径大小</returns>
        ///// <remarks>计算方法：大比例尺表达所有线段长度中值与小比例尺表达所有线段长度中值的平均值</remarks>
        //public static double CalBuffer(List<CPolyline> CPlLt)
        //{
        //    //准备要计算的数据
        //    List<double> dblDataLt = new List<double>();
        //    for (int i = 0; i < CPlLt.Count; i++)
        //    {
        //        List<CPoint> cptlt = CPlLt[i].CptLt;
        //        for (int j = 0; j < cptlt.Count - 1; j++)
        //        {
        //            double dbldata = CalDis(cptlt[j], cptlt[j + 1]);
        //            dblDataLt.Add(dbldata);
        //        }
        //    }

        //    double dblMid = CMathStatistic.CalMid(dblDataLt);
        //    return dblMid;
        //}

        ///// <summary>确定缓冲区半径大小</summary>
        ///// <param name="LSCPlLt">较大比例尺线数组</param>
        ///// <param name="SSCPlLt">较小比例尺线数组</param>
        ///// <returns>缓冲区半径大小</returns>
        ///// <remarks>计算方法：大比例尺表达所有线段长度中值与小比例尺表达所有线段长度中值的平均值</remarks>
        //public static double CalBuffer(List<CPolyline> LSCPlLt, List<CPolyline> SSCPlLt)
        //{
        //    //准备要计算的数据
        //    List<double> dblBSDataLt = new List<double>();
        //    for (int i = 0; i < LSCPlLt.Count; i++)
        //    {
        //        List<CPoint> cptlt = LSCPlLt[i].CptLt;
        //        for (int j = 0; j < cptlt.Count - 1; j++)
        //        {
        //            double dbldata = CalDis(cptlt[j], cptlt[j + 1]);
        //            dblBSDataLt.Add(dbldata);
        //        }
        //    }

        //    List<double> dblSSDataLt = new List<double>();
        //    for (int i = 0; i < SSCPlLt.Count; i++)
        //    {
        //        List<CPoint> cptlt = SSCPlLt[i].CptLt;
        //        for (int j = 0; j < cptlt.Count - 1; j++)
        //        {
        //            double dbldata = CalDis(cptlt[j], cptlt[j + 1]);
        //            dblSSDataLt.Add(dbldata);
        //        }
        //    }

        //    double dblBSMid = CMathStatistic.CalMid(dblBSDataLt);
        //    double dblSSMid = CMathStatistic.CalMid(dblSSDataLt);

        //    double dblMidAvg = (dblBSMid + dblSSMid) / 2;
        //    return dblMidAvg;
        //}

        /// <summary>判断两河流是否为同一河流</summary>
        /// <param name="BSRiver">大比例尺表达河流</param>
        /// <param name="SSRiver">小比例尺表达河流</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <returns>若两河流为同一河流，返回true，否则返回false</returns>
        /// <remarks>判断方法：相交缓冲区多边形面积的两倍除以两缓冲区面积之和</remarks>
        public static bool IsOverlap(CRiver pBSRiver, CRiver pSSRiver, double dblOverlapRatio)
        {
            bool isoverlap = false;

            ITopologicalOperator pBSTop = pBSRiver.pBufferGeo as ITopologicalOperator;
            IGeometry pIntersectGeo = pBSTop.Intersect(pSSRiver.pBufferGeo, esriGeometryDimension.esriGeometry2Dimension);

            if (pIntersectGeo != null)
            {
                IArea pIntersectArea = pIntersectGeo as IArea;
                double dblOverlap = 2 * pIntersectArea.Area / (pBSRiver.dblBufferArea + pSSRiver.dblBufferArea);
                if (dblOverlap >= dblOverlapRatio)
                {
                    isoverlap = true;
                }
            }
            return isoverlap;
        }

        /// <summary>the Overlap area of two geometires</summary>
        /// <param name="BSRiver">大比例尺表达河流</param>
        /// <param name="SSRiver">小比例尺表达河流</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <returns>若两河流为同一河流，返回true，否则返回false</returns>
        /// <remarks>判断方法：相交缓冲区多边形面积的两倍除以两缓冲区面积之和</remarks>
        public static double CalOverlapArea(IGeometry pGeo1, IGeometry pGeo2)
        {
            ITopologicalOperator pTop = pGeo1 as ITopologicalOperator;
            IGeometry pIntersectGeo = pTop.Intersect(pGeo2, esriGeometryDimension.esriGeometry2Dimension);
            IArea pIntersectArea = pIntersectGeo as IArea;

            return pIntersectArea.Area;
        }





        ///// <summary>计算归一化的河流重要性值</summary>
        ///// <param name="CRiverLt">河流数组</param>
        ///// <remarks>有间隔修正</remarks>
        //public static void CalWeightinessUnitary(List<CRiver> CRiverLt)
        //{
        //    double dblWMin = double.MaxValue;
        //    double dblWMax = double.MinValue;

        //    //查找最大值和最小值
        //    for (int i = 0; i < CRiverLt.Count; i++)
        //    {
        //        if (dblWMin > CRiverLt[i].dblWeightiness)
        //        {
        //            dblWMin = CRiverLt[i].dblWeightiness;
        //        }

        //        if (dblWMax < CRiverLt[i].dblWeightiness)
        //        {
        //            dblWMax = CRiverLt[i].dblWeightiness;
        //        }
        //    }

        //    //间隔修正
        //    double dblInterval = (dblWMax - dblWMin) / (CRiverLt.Count - 1);
        //    dblWMax = dblWMax + dblInterval;
        //    dblWMin = dblWMin - dblInterval;

        //    //计算归一化后的值
        //    double dblDis = dblWMax - dblWMin;
        //    for (int i = 0; i < CRiverLt.Count; i++)
        //    {
        //        CRiverLt[i].dblWeightinessUnitary = (CRiverLt[i].dblWeightiness - dblWMin) / dblDis;
        //    }
        //}

        /// <summary>记录线状要素各线段的长度</summary>
        /// <param name="cpl">线状要素</param>
        public static List<double> RecordLengths(CPolyline cpl)
        {
            List<CPoint> cptlt = cpl.CptLt;
            List<double> dbllengthlt = new List<double>();
            for (int i = 0; i < cptlt.Count - 1; i++)
            {
                double dblLength = cptlt[i].DistanceTo(cptlt[i + 1]);
                dbllengthlt.Add(dblLength);
            }
            return dbllengthlt;
        }

        /// <summary>记录线状要素各顶点处夹角（线状要素右侧）</summary>
        /// <param name="cpl">线状要素</param>
        public static List<double> RecordAngles(CPolyline cpl)
        {
            var cptlt = cpl.CptLt;
            var dblanglelt = new List<double>();
            for (int i = 0; i < cptlt.Count - 2; i++)
            {
                double dblAngle = CalAngle_Counterclockwise(cptlt[i], cptlt[i + 1], cptlt[i + 2]);
                dblanglelt.Add(dblAngle);
            }
            return dblanglelt;
        }

        public static IEnumerable<CPoint> MoveCptEb(IEnumerable<CPoint> cptEb, double dblMoveX, double dblMoveY, double dblMoveZ = 0)
        {
            foreach (var cpt in cptEb)
            {
                yield return cpt.Move(dblMoveX, dblMoveY, dblMoveZ);
            }
        }

        public static double FindFarthestDisCpls(CPolyline frcpl, CPolyline tocpl)
        {
            var frcptlt = frcpl.CptLt;
            var tocptlt = tocpl.CptLt;

            var dblDisLt = new List<double>(frcptlt.Count);
            for (int i = 0; i < frcptlt.Count; i++)
            {
                dblDisLt.Add(frcptlt[i].DistanceTo(tocptlt[i]));
            }

            return dblDisLt.Max();
        }

        /// <summary>计算由arctan求方位角时各参数的导数</summary>
        /// <param name="dblDis">两顶点之间的距离</param>
        /// <param name="strTarget">目标参数</param>
        /// <returns>目标参数的导数</returns>
        public static double DerArctan(double dblX1, double dblY1, double dblX2, double dblY2, double dblDis, string strTarget)
        {
            double dblDis2 = dblDis * dblDis;
            double dblResult = 0;
            switch (strTarget)
            {
                case "x1":
                    {
                        dblResult = (dblY2 - dblY1) / dblDis2;
                        break;
                    }
                case "y1":
                    {
                        dblResult = -(dblX2 - dblX1) / dblDis2;
                        break;
                    }
                case "x2":
                    {
                        dblResult = -(dblY2 - dblY1) / dblDis2;
                        break;
                    }
                case "y2":
                    {
                        dblResult = (dblX2 - dblX1) / dblDis2;
                        break;
                    }
                default:
                    {
                        MessageBox.Show("对夹角求导时，参数输入有误！");
                        break;
                    }
            }

            return dblResult;
        }


        /// <summary>计算向量的长度</summary>
        /// <param name="mat">矩阵</param>
        /// <returns>矩阵中各值平方和开方根</returns>
        public static double CalLengthofVector(VBMatrix mat)
        {
            double dblSquareSum = 0;
            for (int i = 0; i < mat.Row; i++)
            {
                for (int j = 0; j < mat.Col; j++)
                {
                    dblSquareSum += Math.Pow(mat[i, j], 2);
                }
            }
            double dblLength = Math.Sqrt(dblSquareSum);
            return dblLength;
        }




        public static CEnvelope GetEnvelope(List<CPolyline> cpllt)
        {
            List<CPoint> cptlt = new List<CPoint>(cpllt.Count * 2);
            foreach (CPolyline cpl in cpllt)
            {
                cptlt.AddRange(cpl.CptLt);
            }

            return GetEnvelope(cptlt);
        }

        public static CEnvelope GetEnvelope(List<CEdge> cedgelt)
        {
            List<CPoint> cptlt = new List<CPoint>(cedgelt.Count * 2);
            foreach (CEdge cedge in cedgelt)
            {
                cptlt.Add(cedge.FrCpt);
                cptlt.Add(cedge.ToCpt);
            }
            return GetEnvelope(cptlt);
        }

        public static CEnvelope GetEnvelope(List<CPoint> cptlt)
        {
            double dblMinX = cptlt.Min(cpt => cpt.X);
            double dblMinY = cptlt.Min(cpt => cpt.Y);
            double dblMaxX = cptlt.Max(cpt => cpt.X);
            double dblMaxY = cptlt.Max(cpt => cpt.Y);

            CEnvelope CEnv = new CEnvelope(dblMinX, dblMinY, dblMaxX, dblMaxY);
            return CEnv;
        }

        //public static List<CPolygon> CreateVoronoiForCEdges(List<CEdge> pedgelt)
        //{
        //    double dblMaxLength = 0;
        //    for (int i = 0; i < pedgelt.Count ; i++)
        //    {
        //        if (pedgelt[i].dblLength >dblMaxLength)
        //        {
        //            dblMaxLength = pedgelt[i].dblLength;
        //        }
        //    }

        //    double dblSubLength = dblMaxLength / 100;
        //    for (int i = 0; i < pedgelt; i++)
        //    {

        //        List<object> objShapeLt = new List<object>();
        //        CPoint cpt0 = new CPoint(i, pedgelt[i].FromPoint);
        //        objShapeLt.Add(cpt0);

        //        int int


        //        pedgelt[i].QueryPoint();


        //    }


        //    return null;


        //}


        public static double CoordinateTransform(double dblValue, double dblBase, double dblNewBase, double dblFactor)
        {
            double dblResult = (dblValue - dblBase) * dblFactor + dblNewBase;
            return dblResult;
        }

        //public static void Test<T, CGeo>(CPolyBase<CGeo> cpg) where CGeo : class
        //{

        //}

        public static List<CEdge> GetCEdgeLtFromCpbLt<Cpb, CGeo>(IEnumerable<Cpb> fcpbeb)
            where Cpb : CPolyBase
        {
            List<CEdge> CEdgeLt = new List<CEdge>();
            foreach (var cpb in fcpbeb)
            {
                cpb.JudgeAndFormCEdgeLt();
                CEdgeLt.AddRange(cpb.CEdgeLt);
            }

            return CEdgeLt;
        }

        //public static int CountEdgeNum<Cpb, CGeo>(IEnumerable<Cpb> cpbeb) 
        //    where Cpb : CPolyBase<CGeo>
        //    where CGeo : class
        //{
        //    int intEdgeNum = 0;
        //    foreach (var cpb in cpbeb)
        //    {
        //        intEdgeNum += cpb.CEdgeLt.Count;
        //    }
        //    return intEdgeNum;
        //}

        /// <summary>
        /// If there are at least two polylines meeting at the same point, we call the meeting point an intersection
        /// </summary>
        /// <param name="CorrCptsLt"></param>
        /// <returns></returns>
        public static int GetNumofIntersections(List<CCorrCpts> CorrCptsLt)
        {
            foreach (var CorrCpt in CorrCptsLt)
            {
                CorrCpt.FrCpt.isTraversed = false;
                CorrCpt.ToCpt.isTraversed = false;
            }

            int intCount = 0;
            foreach (var CorrCpt in CorrCptsLt)
            {
                if (CorrCpt.FrCpt.isTraversed == false && CorrCpt.ToCpt.isTraversed == false)
                {
                    intCount++;
                }

                CorrCpt.FrCpt.isTraversed = true;
                CorrCpt.ToCpt.isTraversed = true;
            }
            return intCount;
        }


        public static int GetNumofAloneEnds(List<CPoint> CEndPtLt, List<CCorrCpts> CorrCptsLt)
        {
            foreach (var cpt in CEndPtLt)
            {
                cpt.isTraversed = false;
            }

            foreach (var CorrCpt in CorrCptsLt)
            {
                CorrCpt.FrCpt.isTraversed = true;
                CorrCpt.ToCpt.isTraversed = true;
            }

            int intAloneEnds = 0;
            foreach (var cpt in CEndPtLt)
            {
                if (cpt.isTraversed == false)
                {
                    intAloneEnds++;
                }
            }

            return intAloneEnds;
        }

        #region LookingForNeighboursByGrids
        public static List<CCorrCpts> LookingForNeighboursByGrids(List<CPoint> cptlt1, List<CPoint> cptlt2, double dblThreshold)
        {
            CEnvelope pEnvelope1 = GetEnvelope(cptlt1);
            CEnvelope pEnvelope2 = GetEnvelope(cptlt2);

            //IEnvelope pUnionEnvelope = new EnvelopeClass();
            double XMin = Math.Min(pEnvelope1.XMin, pEnvelope2.XMin);
            double YMin = Math.Min(pEnvelope1.YMin, pEnvelope2.YMin);
            double XMax = Math.Max(pEnvelope1.XMax, pEnvelope2.XMax);
            double YMax = Math.Max(pEnvelope1.YMax, pEnvelope2.YMax);
            CEnvelope pUnionEnvelope = new CEnvelope(XMin, YMin, XMax, YMax);

            int intMaxCount = Math.Max(cptlt1.Count, cptlt2.Count);
            //dblRowCount*dblColCount==n
            double dblGridSize = Math.Sqrt(pUnionEnvelope.Width * pUnionEnvelope.Height / Convert.ToDouble(intMaxCount));  
            dblGridSize = Math.Max(dblGridSize, dblThreshold);

            //+1, so that the bordered point can be covered
            int intRowCount = Convert.ToInt32(Math.Truncate(pUnionEnvelope.Height / dblGridSize)) + 1;  
            int intColCount = Convert.ToInt32(Math.Truncate(pUnionEnvelope.Width / dblGridSize)) + 1;


            //double dblHWRatio = pUnionEnvelope.Height / pUnionEnvelope.Width;

            ////dblRowCount*dblColCount==n and dblRowCount/dblColCount==dblHWRatio
            //double dblRowCount = Math.Sqrt(Convert.ToDouble(intMaxCount) * dblHWRatio);
            //double dblColCount = Math.Sqrt(Convert.ToDouble(intMaxCount) / dblHWRatio);

            //double dblGridSize = pUnionEnvelope.Height / dblRowCount;
            //int intRowCount = Convert.ToInt32(Math.Truncate(dblRowCount)) + 1;  //+1, so that the bordered point can be covered
            //int intColCount = Convert.ToInt32(Math.Truncate(dblColCount)) + 1;   //+1, so that the bordered point can be covered

            //int intRowColCount = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(intMaxCount)));

            //double dblWidth = Math.Max((pUnionEnvelope.XMax - pUnionEnvelope.XMin) / Convert.ToDouble(intRowColCount), dblThreshold);
            //double dblHeight = Math.Max((pUnionEnvelope.YMax - pUnionEnvelope.YMin) / Convert.ToDouble(intRowColCount), dblThreshold);

            //intRowColCount++;  //+1, so that the bordered point can be covered
            var aCptLkGrid1 = new List<CPoint>[intRowCount, intColCount];
            var aCptLkGrid2 = new List<CPoint>[intRowCount, intColCount];
            for (int i = 0; i < intRowCount; i++)
            {
                for (int j = 0; j < intColCount; j++)
                {
                    aCptLkGrid1[i, j] = new List<CPoint>();
                    aCptLkGrid2[i, j] = new List<CPoint>();
                }
            }

            //Fill points in Grids
            for (int i = 0; i < cptlt1.Count; i++)
            {
                FillCptinGrid(cptlt1[i], dblGridSize, dblGridSize, pUnionEnvelope, aCptLkGrid1);
            }

            //Fill points in Grids
            for (int i = 0; i < cptlt2.Count; i++)
            {
                FillCptinGrid(cptlt2[i], dblGridSize, dblGridSize, pUnionEnvelope, aCptLkGrid2);
            }

            var CorrCptsLt = new List<CCorrCpts>();
            for (int i = 0; i < intRowCount; i++)
            {
                for (int j = 0; j < intColCount; j++)
                {
                    //the first column
                    int intJ = j - 1;
                    if (intJ >= 0)
                    {
                        //Lower
                        if (i - 1 >= 0)
                        {
                            LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid1[i, j], aCptLkGrid2[i - 1, intJ], dblThreshold);
                        }
                        //Midle
                        LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid1[i, j], aCptLkGrid2[i, intJ], dblThreshold);
                        //Upper
                        if (i + 1 < intRowCount)
                        {
                            LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid1[i, j], aCptLkGrid2[i + 1, intJ], dblThreshold);
                        }
                    }

                    //the second column
                    intJ = j;
                    //Lower
                    if (i - 1 >= 0)
                    {
                        LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid1[i, j], aCptLkGrid2[i - 1, intJ], dblThreshold);
                    }
                    //Midle
                    LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid1[i, j], aCptLkGrid2[i, intJ], dblThreshold);
                    //Upper
                    if (i + 1 < intRowCount)
                    {
                        LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid1[i, j], aCptLkGrid2[i + 1, intJ], dblThreshold);
                    }

                    //the third column
                    intJ = j + 1;
                    if (intJ < intColCount)
                    {
                        //Lower
                        if (i - 1 >= 0)
                        {
                            LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid1[i, j], aCptLkGrid2[i - 1, intJ], dblThreshold);
                        }
                        //Midle
                        LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid1[i, j], aCptLkGrid2[i, intJ], dblThreshold);
                        //Upper
                        if (i + 1 < intRowCount)
                        {
                            LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid1[i, j], aCptLkGrid2[i + 1, intJ], dblThreshold);
                        }
                    }
                }
            }
            return CorrCptsLt;
        }



        public static List<CCorrCpts> LookingForNeighboursByGrids(List<CPoint> cptlt, double dblThreshold)
        {
            var CorrCptsLt = new List<CCorrCpts>();
            if (cptlt.Count == 0)
            {
                return CorrCptsLt;
            }

            CEnvelope pEnvelope = GetEnvelope(cptlt);
            //dblRowCount*dblColCount==n
            double dblGridSize = Math.Sqrt(pEnvelope.Width * pEnvelope.Height / Convert.ToDouble(cptlt.Count));  
            dblGridSize = Math.Max(dblGridSize, dblThreshold);

            //+1, so that the bordered point can be covered
            int intRowCount = Convert.ToInt32(Math.Truncate(pEnvelope.Height / dblGridSize)) + 1;  
            int intColCount = Convert.ToInt32(Math.Truncate(pEnvelope.Width / dblGridSize)) + 1;   

            //intRowColCount++;  //+1, so that the bordered point can be covered
            var aCptLkGrid = new List<CPoint>[intRowCount, intColCount];
            for (int i = 0; i < intRowCount; i++)
            {
                for (int j = 0; j < intColCount; j++)
                {
                    aCptLkGrid[i, j] = new List<CPoint>();
                }
            }

            for (int i = 0; i < cptlt.Count; i++)
            {
                FillCptinGrid(cptlt[i], dblGridSize, dblGridSize, pEnvelope, aCptLkGrid);
            }

            for (int i = 0; i < intRowCount; i++)
            {
                for (int j = 0; j < intColCount; j++)
                {
                    LookingForNeighboursInGridItself(ref CorrCptsLt, aCptLkGrid[i, j], dblThreshold);

                    if (j + 1 < intColCount)  //Right
                    {
                        LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid[i, j], aCptLkGrid[i, j + 1], dblThreshold);
                    }

                    if (i + 1 < intRowCount) //Upper
                    {
                        if (j - 1 >= 0)  //UpperLeft
                        {
                            LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid[i, j], aCptLkGrid[i + 1, j - 1], dblThreshold);
                        }
                        //UpperMiddle
                        LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid[i, j], aCptLkGrid[i + 1, j], dblThreshold);

                        if (j + 1 < intColCount)  //UpperRight
                        {
                            LookingForNeighboursInGrids(ref CorrCptsLt, aCptLkGrid[i, j], aCptLkGrid[i + 1, j + 1], dblThreshold);
                        }
                    }
                }
            }
            return CorrCptsLt;
        }

        /// <summary>Fill the points into cells of the grid</summary>
        /// <remarks>Notice that the cells start from the left down corner of the grid.
        /// If a point on a horizontal line share by two cells, 
        /// then this point will be filled in the cell above the horizontal line.
        /// If a point on a vertical line share by two cells, 
        /// then this point will be filled in the cell to the right of the vertical line </remarks>
        public static void FillCptinGrid(CPoint cpt, double dblWidth, double dblHeight, CEnvelope pEnvelope, 
            List<CPoint>[,] aCptLkGrid)
        {
            int rownum = Convert.ToInt32(Math.Truncate((cpt.Y - pEnvelope.YMin) / dblHeight));
            int colnum = Convert.ToInt32(Math.Truncate((cpt.X - pEnvelope.XMin) / dblWidth));
            aCptLkGrid[rownum, colnum].Add(cpt);
        }

        private static void LookingForNeighboursInGridItself(ref List<CCorrCpts> CorrCptsLt, 
            List<CPoint> aCptLkGrid, double dblThreshold)
        {
            for (int i = 0; i < aCptLkGrid.Count - 1; i++)
            {
                CPoint cpti = aCptLkGrid[i];
                for (int j = i + 1; j < aCptLkGrid.Count; j++)
                {
                    CPoint cptj = aCptLkGrid[j];
                    double dblXDiff = Math.Abs(cpti.X - cptj.X);
                    double dblYDiff = Math.Abs(cpti.Y - cptj.Y);
                    if (dblXDiff <= dblThreshold && dblYDiff <= dblThreshold)
                    {
                        CorrCptsLt.Add(new CCorrCpts(cpti, cptj));
                    }
                }
            }
        }

        private static void LookingForNeighboursInGrids(ref List<CCorrCpts> CorrCptsLt, 
            List<CPoint> aCptLkGrid1, List<CPoint> aCptLkGrid2, double dblThreshold)
        {
            foreach (CPoint cpti in aCptLkGrid1)
            {
                foreach (CPoint cptj in aCptLkGrid2)
                {
                    double dblXDiff = Math.Abs(cpti.X - cptj.X);
                    double dblYDiff = Math.Abs(cpti.Y - cptj.Y);
                    if (dblXDiff <= dblThreshold && dblYDiff <= dblThreshold)
                    {
                        CorrCptsLt.Add(new CCorrCpts(cpti, cptj));
                    }
                }
            }
        }

        public static int CountAlonePt(List<CPoint> cptlt)
        {
            int intCountAlonePt = 0;
            foreach (CPoint cpt in cptlt)
            {
                if (cpt.PairCorrCpt == null)
                {
                    intCountAlonePt++;
                }
            }
            return intCountAlonePt;
        }
        #endregion




        public static CEdge DetectCloestLeftCEdge(CPoint rayfrcpt, CEdgeGrid pEdgeGrid)
        {
            List<CEdge>[,] aCEdgeLtCell = pEdgeGrid.aCEdgeLtCell;

            bool IsFoundEdge = false;
            int intRow = pEdgeGrid.GetRow(rayfrcpt.Y);
            int intCol = pEdgeGrid.GetCol(rayfrcpt.X);

            CPoint raytocpt = new CPoint(-1, pEdgeGrid.pEnvelope.XMin - pEdgeGrid.dblCellWidth, rayfrcpt.Y);

            CIntersection pIntersectionRightMost = new CIntersection();
            pIntersectionRightMost.IntersectCpt = raytocpt;
            CEdge raycedge = new CEdge(rayfrcpt, raytocpt);
            for (int i = intCol; i >= 0; i--)
            {
                if (aCEdgeLtCell[intRow, i].Count > 0)
                {
                    double dblXMinColi = pEdgeGrid.GetCellXMin(i);
                    foreach (CEdge cedge in aCEdgeLtCell[intRow, i])
                    {
                        CIntersection pIntersection = raycedge.IntersectWith(cedge);
                        if (pIntersection.enumIntersectionType == CEnumIntersectionType.InFr || 
                            pIntersection.enumIntersectionType == CEnumIntersectionType.InIn || 
                            pIntersection.enumIntersectionType == CEnumIntersectionType.InTo)
                        {
                            //we don't need to care about CEnumIntersectionType.Overlap. Because if there is an overlap, 
                            //there is also an CEnumIntersectionType.InFr or 
                            //CEnumIntersectionType.InTo with the successor or predecessor of cedge
                            if (pIntersection.IntersectCpt.X >= dblXMinColi)
                            {
                                if (pIntersection.IntersectCpt.X > pIntersectionRightMost.IntersectCpt.X)
                                {
                                    pIntersectionRightMost = pIntersection;
                                }
                                else if (pIntersection.IntersectCpt.X == pIntersectionRightMost.IntersectCpt.X)
                                {
                                    if (pIntersection.enumIntersectionType == CEnumIntersectionType.InIn || 
                                        pIntersectionRightMost.enumIntersectionType == CEnumIntersectionType.InIn)
                                    {
                                        throw new ArgumentOutOfRangeException("I didn't expect this case!");
                                    }

                                    CEdge tempOldCEdge2 = new CEdge(pIntersectionRightMost.IntersectCpt, 
                                        pIntersectionRightMost.CEdge2.GetInbetweenCpt(0.5));
                                    CEdge tempNewCEdge2 = new CEdge(pIntersectionRightMost.IntersectCpt, 
                                        pIntersection.CEdge2.GetInbetweenCpt(0.5));

                                    double dblDiffYOld = tempOldCEdge2.GetIncrY();
                                    double dblDiffYNew = tempNewCEdge2.GetIncrY();

                                    double dblDiffYProd = dblDiffYOld * dblDiffYNew;
                                    if (dblDiffYProd < 0)
                                    {
                                        //do nothing, we can use either pIntersection or pIntersectionRightMost 
                                    }
                                    else if (dblDiffYProd == 0)
                                    {
                                        throw new ArgumentOutOfRangeException(
                                            "I didn't expect this case because of the previous constraints!");
                                    }
                                    else
                                    {
                                        double dblCorrect = Math.Min(Math.Abs(dblDiffYOld), Math.Abs(dblDiffYNew));
                                        if (dblDiffYOld < 0) //then dblDiffYNew>0
                                        {
                                            dblCorrect = -dblCorrect;
                                        }

                                        pIntersectionRightMost = ComputeCloestIntersectionByAdjusting(
                                            raycedge, dblCorrect / 2, pIntersectionRightMost, pIntersection);
                                    }
                                }
                                IsFoundEdge = true;
                            }
                        }
                    }

                    if (IsFoundEdge == true)
                    {
                        rayfrcpt.ClosestLeftCIntersection = pIntersectionRightMost;
                        break;
                    }
                }
            }

            if (IsFoundEdge == true)
            {
                return rayfrcpt.ClosestLeftCIntersection.CEdge2;
            }
            else
            {
                return null;
            }
        }

        private static CIntersection ComputeCloestIntersectionByAdjusting(
            CEdge raycedge, double dblY, CIntersection pIntersectionRightMost, CIntersection pIntersection)
        {
            CEdge adjustedraycedge = new CEdge(new CPoint(-1, raycedge.FrCpt.X, raycedge.FrCpt.Y + dblY), 
                new CPoint(-1, raycedge.ToCpt.X, raycedge.ToCpt.Y + dblY));

            var adjustedintersectionrightmost = adjustedraycedge.IntersectWith(pIntersectionRightMost.CEdge2);
            var adjustedintersection = adjustedraycedge.IntersectWith(pIntersection.CEdge2);

            if (adjustedintersectionrightmost.IntersectCpt.X > adjustedintersection.IntersectCpt.X)
            {
                return pIntersectionRightMost;
            }
            else if (adjustedintersectionrightmost.IntersectCpt.X < adjustedintersection.IntersectCpt.X)
            {
                return adjustedintersection;
            }
            else
            {
                throw new ArgumentOutOfRangeException("Do the two edges overlap each other?");
            }
        }

        /// <summary>
        /// Get the edge of the face in which the point is. the edge or its twin edge
        /// </summary>
        /// <param name="holeleftcptlt"></param>
        public static CEdge GetCorrectEdge(CPoint cpt)
        {
            if (cpt.ClosestLeftCIntersection != null)
            {
                CIntersection pIntersection = cpt.ClosestLeftCIntersection;
                double dblAngle = CGeoFunc.CalAngle_Counterclockwise(pIntersection.CEdge1, pIntersection.CEdge2);
                if (dblAngle > Math.PI)
                {
                    pIntersection.CEdge2 = pIntersection.CEdge2.cedgeTwin;
                }
                pIntersection.CEdge2IncidentFace = pIntersection.CEdge2.cpgIncidentFace;
                return cpt.ClosestLeftCIntersection.CEdge2;
            }
            else
            {
                return null;
            }

        }


        public static IRing2 GenerateRingByCptlt(List<CPoint> cptlt)
        {
            IPointCollection4 pRingCol = new RingClass();
            foreach (var cpt in cptlt)
            {
                pRingCol.AddPoint(cpt.JudgeAndSetPoint());
            }

            return pRingCol as IRing2;
        }

        /// <summary>Detect intersections of edges</summary>
        /// <remarks>
        /// enumIntersectionType can be -1, 0, 1, 2, 3, 4
        /// -1, not intersection; 3, intersection
        /// one has to specify type 0, types 1 & 2, and type 4 respectively by blnTouchBothEnds, blnTouchEndEdge, and blnOverlap
        /// </remarks>
        public static List<CEdgeRelation> DetectCEdgeRelations(List<CEdge> cedgelt, 
            double dblRangeDis, bool NotCheckBelonged = false)
        {
            var CEdgeRelationLt = new List<CEdgeRelation>();
            foreach (CEdge cedge in cedgelt)
            {
                cedge.isTraversed = false;
            }

            CEdgeGrid pEdgeGrid = new CEdgeGrid(cedgelt);   //put edges in the cells of a grid
            int intRowRange = Convert.ToInt32(Math.Ceiling(dblRangeDis / pEdgeGrid.dblCellHeight));
            int intColRange = Convert.ToInt32(Math.Ceiling(dblRangeDis / pEdgeGrid.dblCellWidth));
            foreach (CEdge cedge in cedgelt)
            {
                cedge.isTraversed = true;

                List<CEdge> TraversedCEdgeLt = new List<CEdge>();
                foreach (var RowColVp in cedge.RowColVpLt)
                {
                    var intFrRow = Math.Max(RowColVp.val1 - intRowRange, 0);
                    var intFrCol = Math.Max(RowColVp.val2 - intColRange, 0);
                    var intToRow = Math.Min(RowColVp.val1 + intRowRange, pEdgeGrid.intRowCount - 1);
                    var intToCol = Math.Min(RowColVp.val2 + intColRange, pEdgeGrid.intColCount - 1);

                    for (int i = intFrRow; i <= intToRow; i++)
                    {
                        for (int j = intFrCol; j <= intToCol; j++)
                        {
                            foreach (var pcedge in pEdgeGrid.aCEdgeLtCell[i, j])
                            {
                                if (pcedge.isTraversed == true)
                                {
                                    continue;
                                }
                                if (NotCheckBelonged == true && cedge.BelongedCpg.GID == pcedge.BelongedCpg.GID)
                                {
                                    continue;
                                }

                                pcedge.isTraversed = true;
                                // we will set IsTraversed of these TraversedCEdge to false 
                                //after looking for intersection for this cedge
                                TraversedCEdgeLt.Add(pcedge);

                                CEdgeRelation pEdgeRelation = new CEdgeRelation(cedge, pcedge);
                                pEdgeRelation.DetectRelation();
                                if (pEdgeRelation.cptEdgeDis.dblDis < dblRangeDis)
                                {
                                    CEdgeRelationLt.Add(pEdgeRelation);
                                }

                            }
                        }
                    }
                }

                //Set the TraversedCEdge to false
                foreach (CEdge TraversedCEdge in TraversedCEdgeLt)
                {
                    TraversedCEdge.isTraversed = false;
                }
            }

            return CEdgeRelationLt;
        }


        //public static int SumPointsNumber(List<CPolyline> cpllt)
        //{
        //    int intCount = 0;
        //    foreach (CPolyline cpl in cpllt)
        //    {
        //        intCount += cpl.CptLt.Count;
        //    }
        //    return intCount;
        //}

        //public static void GetPolylineFromResultPtLt(ref List<CPoint> CResultPtLt, ref List<object> objlt)
        //{
        //    for (int j = 0; j < CResultPtLt.Count; j++)
        //    {
        //        for (int k = 0; k < CResultPtLt[j].CorrespondingPtLt.Count; k++)
        //        {
        //            IPolyline5 pPolyline = GetPolylineSegment(CResultPtLt[j], CResultPtLt[j].CorrespondingPtLt[k]);
        //            objlt.Add(pPolyline);
        //        }
        //    }
        //}

        //Compute the dot product AB . AC
        public static double DotProduct(CPoint A, CPoint B, CPoint C)
        {
            return DotProduct(B.X - A.X, B.Y - A.Y, C.X - A.X, C.Y - A.Y);
        }

        public static double DotProduct(double dblVecX1, double dblVecY1, double dblVecX2, double dblVecY2)
        {
            return dblVecX1 * dblVecX2 + dblVecY1 * dblVecY2;
        }

        //generate transformed single polylines
        public static List<CPolyline> GenerateCplLtByCorrCpt(List<CPolyline> CplLt)
        {
            List<CPolyline> NewCPlLt = new List<CPolyline>(CplLt.Count);
            foreach (CPolyline cpl in CplLt)
            {
                List<CPoint> NewCptLt = new List<CPoint>(cpl.CptLt.Count);
                foreach (CPoint cpt in cpl.CptLt)
                {
                    NewCptLt.Add(cpt.PairCorrCpt.ToCpt);
                }
                CPolyline newcpl = new CPolyline(cpl.ID, NewCptLt);
                NewCPlLt.Add(newcpl);
            }

            return NewCPlLt;
        }

        /// <summary>
        /// Construct the edge list for a list of points
        /// </summary>
        /// <returns>a list of edges, there are only two vertices are stored</returns>
        /// <remarks>if the set of points are from a polygon, 
        /// then first point and the last point must have the same coordinates</remarks>
        /// <ret>
        public static List<List<CEdge>> FormCEdgeLtLt(IEnumerable<IEnumerable<CPoint>> cptebeb)
        {
            List<List<CEdge>> cedgeltlt = new List<List<CEdge>>();
            foreach (var cpteb in cptebeb)
            {
                cedgeltlt.Add(FormCEdgeEb(cpteb).ToList());
            }
            return cedgeltlt;
        }


        /// <summary>
        /// Construct the edge list for a list of points
        /// </summary>
        /// <param name="blnIdentical">blnIdentical == true means the first point and the last point are the same</param>
        /// <returns>a list of edges, there are only two vertices are stored</returns>
        /// <remarks>if the set of points are from a polygon, 
        /// then first point and the last point must have the same coordinates</remarks>
        public static IEnumerable<CEdge> FormCEdgeEb(IEnumerable<CPoint> cptEb, bool AddCEdge_lastcptTofirstcpt = false)
        {
            var cptEt = cptEb.GetEnumerator();
            cptEt.MoveNext();
            var firstcpt = cptEt.Current;
            var lastcpt = cptEt.Current;

            while (cptEt.MoveNext())
            {
                yield return new CEdge(lastcpt, cptEt.Current);
                lastcpt = cptEt.Current;
            }

            if (AddCEdge_lastcptTofirstcpt == true)
            {
                yield return new CEdge(lastcpt, firstcpt);
            }
        }

        public static IPolyline5 GetPolylineSegment(CPoint cpt1, CPoint cpt2)
        {
            List<CPoint> cptlt = new List<CPoint>(2);
            cptlt.Add(cpt1);
            cptlt.Add(cpt2);
            return GetIplFromCptLt(cptlt);
        }

        public static IPolyline5 GetIplFromCptLt(List<CPoint> cptlt)
        {
            IPointCollection4 pCol = GetIptColFromCptLt(cptlt, esriGeometryType.esriGeometryPolyline);
            return (IPolyline5)pCol;
        }

        public static IPolygon4 GetIpgFromCptLt(List<CPoint> cptlt)
        {
            IPointCollection4 pCol = GetIptColFromCptLt(cptlt, esriGeometryType.esriGeometryPolygon);
            return (IPolygon4)pCol;
        }

        public static IRing2 GetIrgFromCptLt(List<CPoint> cptlt)
        {
            IPointCollection4 pCol = GetIptColFromCptLt(cptlt, esriGeometryType.esriGeometryRing);
            return (IRing2)pCol;
        }

        public static IPointCollection4 GetIptColFromCptLt(List<CPoint> cptlt,
    esriGeometryType pesriGeometryType)
        {
            IPointCollection4 pCol = null;
            switch (pesriGeometryType)
            {
                case esriGeometryType.esriGeometryPolygon:
                    pCol = new PolygonClass();
                    break;
                case esriGeometryType.esriGeometryPolyline:
                    pCol = new PolylineClass();
                    break;
                case esriGeometryType.esriGeometryRing:
                    pCol = new RingClass();
                    break;
                default:
                    break;
            }

            foreach (var cpt in cptlt)
            {
                cpt.JudgeAndSetPoint();
                pCol.AddPoint(cpt.pPoint);
            }

            return pCol;
        }


        public static IEnumerable<CPolygon> GenerateHoleCpgLt(IEnumerable<List<CPoint>> holecptltEb)
        {
            if (holecptltEb == null)
            {
                throw new ArgumentOutOfRangeException("There is no element!");
            }

            int intCount = 0;
            foreach (var cptlt in holecptltEb)
            {
                var cpg = new CPolygon(intCount++, cptlt);
                cpg.IsHole = true;
                yield return cpg;
            }
        }


        //public static void SetEndBelongedCpl(List<CPolyline> TransSgCPlLt)
        //{
        //    foreach (CPolyline transsgcpl in TransSgCPlLt)
        //    {
        //        transsgcpl.enumScale = CEnumScale.Single;
        //        SetEndBelongedCpl(transsgcpl.CptLt[0], transsgcpl);
        //        SetEndBelongedCpl(transsgcpl.CptLt[transsgcpl.CptLt.Count - 1], transsgcpl);
        //    }
        //}

        //public static void SetEndBelongedCpl(CPoint sgcpt, CPolyline transsgcpl)
        //{
        //    CPoint frcpt = sgcpt.PairCorrCpt.FrCpt;
        //    if (frcpt.BelongedCPolyline.enumScale == CEnumScale.Larger)
        //    {
        //        sgcpt.BelongedCPolyline = frcpt.BelongedCPolyline;
        //    }
        //    else if (frcpt.BelongedCPolyline.enumScale == CEnumScale.Single)
        //    {
        //        sgcpt.BelongedCPolyline = transsgcpl;
        //    }
        //}





        public static List<List<CCorrCpts>> GetCorrCptsLtLt(List<CPolyline> InterLSCPlLt, List<CPolyline> InterSSCPlLt)
        {
            List<List<CCorrCpts>> pCorrCptsLtLt = new List<List<CCorrCpts>>(InterLSCPlLt.Count);
            for (int i = 0; i < InterLSCPlLt.Count; i++)
            {
                List<CPoint> bscptlt = InterLSCPlLt[i].CptLt;
                List<CPoint> sscptlt = InterSSCPlLt[i].CptLt;
                List<CCorrCpts> pCorrCptsLt = new List<CCorrCpts>(bscptlt.Count);
                for (int j = 0; j < bscptlt.Count; j++)
                {
                    var pCorrCpts = new CCorrCpts(bscptlt[j], sscptlt[j]);
                    //pCorrCpts.SetMoveVector();
                    pCorrCptsLt.Add(pCorrCpts);
                }
                pCorrCptsLtLt.Add(pCorrCptsLt);
            }
            return pCorrCptsLtLt;
        }


        public static CPolygon CreateRegularCpg(IEnvelope pEnv, int intNumber = 3, bool blnClockwise=true)
        {
            return CreateRegularCpg(pEnv.XMin, pEnv.YMin, pEnv.Width / 2, intNumber, blnClockwise);
        }

        public static double CalCompRegularPolygon(int intEdgeNum)
        {
            if (intEdgeNum < 3)
            {
                throw new ArgumentOutOfRangeException("intEdgeNum should be larger than 2!");
            }

            double dblPIOverNum = Math.PI / intEdgeNum;
            return Math.Sqrt(dblPIOverNum / Math.Tan(dblPIOverNum));
        }

        public static double CalCompactness(double dblArea, double dblLength)
        {
            //return dblLength / Math.Sqrt(dblArea);
            return CConstants.dblTwoSqrtPI * Math.Sqrt(dblArea) / dblLength;
        }

        //public static CPolygon CreateRegularCpg(CPoint centrecpt, double dblRadius = 1, int intNumber = 3)
        //{
        //    return CreateRegularCpg(centrecpt.X, centrecpt.Y, dblRadius, intNumber);
        //}

        public static CPolygon CreateRegularCpg(double dblCentreX = 0, double dblCentreY = 0, 
            double dblRadius = 1, int intNumber = 3, bool blnClockwise = true)
        {
            double dblAngleInterval = CConstants.dblTwoPI / intNumber;
            double dblAxisAngle = Math.PI / 2;
            List<CPoint> cptlt = new List<CPoint>(intNumber + 1);
            cptlt.Add(new CPoint(0, dblCentreX, dblCentreY + dblRadius));
            for (int i = 0; i < intNumber; i++)
            {
                if (blnClockwise == true)
                {
                    dblAxisAngle -= dblAngleInterval;
                }
                else
                {
                    dblAxisAngle += dblAngleInterval;
                }

                cptlt.Add(new CPoint(i + 1, dblCentreX + dblRadius * Math.Cos(dblAxisAngle),
                    dblCentreY + dblRadius * Math.Sin(dblAxisAngle)));
            }

            return new CPolygon(-1, cptlt);
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="CpgEb"></param>
        /// <param name="intID"></param>
        /// <returns></returns>
        /// <remarks>there should be no holes in the merged polygons; 
        /// any of the merged polygons should touch at least another one</remarks>
        public static CPolygon MergeCpgLtDCEL(IEnumerable<CPolygon> CpgEb, int intID = -1)
        {
            var CEdgeLt = GetCEdgeLtFromCpbLt<CPolygon, CPolygon>(CpgEb);

            //for (int i = 0; i < CpgLt .Count ; i++)
            //{
            //    CpgLt[i].Clear();
            //}

            CDCEL pDCEL = new CDCEL(CEdgeLt);
            pDCEL.ConstructDCEL();

            CPolygon SuperFaceCpg = new CPolygon();
            int intSuperFaceCount = 0;
            foreach (var FaceCpg in pDCEL.FaceCpgLt)
            {
                if (FaceCpg.OuterCmptCEdge == null)
                {
                    intSuperFaceCount++;
                    SuperFaceCpg = FaceCpg;
                }
            }

            if (intSuperFaceCount != 1)
            {
                throw new ArgumentException("MergeCpgLt occurs a problem!");
            }

            var cptlt = SuperFaceCpg.GetInnerCptLtLt(false)[0];  //I need to check if we realy need a counter clockwise direction

            //create a new polygon
            var newcptlt = new List<CPoint>(cptlt.Count);
            for (int i = 0; i < cptlt.Count; i++)
            {
                newcptlt.Add(cptlt[i].Copy());
            }
            return new CPolygon(intID, cptlt);
        }


        public static IPolygon4 MergeCpgEbToIpg(IEnumerable<CPolygon> CpgEb, int intID = -1)
        {
            IGeometry geometryBag = new GeometryBagClass();

            ////Define the spatial reference of the bag before adding geometries to it.
            //geometryBag.SpatialReference = geoDataset.SpatialReference;
            IGeometryCollection geometryCollection = geometryBag as IGeometryCollection;
            object missing = Type.Missing;
            foreach (var cpg in CpgEb)
            {
                geometryCollection.AddGeometry(cpg.JudgeAndSetPolygon() as IGeometry, ref missing, ref missing);
            }

            // Create the polygon that will be the union of the features returned from the search cursor.
            // The spatial reference of this feature does not need to be set ahead of time. The 
            // ConstructUnion method defines the constructed polygon's spatial reference to be the 
            // same as the input geometry bag.
            ITopologicalOperator unionedPolygon = new PolygonClass();
            unionedPolygon.ConstructUnion(geometryBag as IEnumGeometry);

            return unionedPolygon as IPolygon4;
        }


        public static void CheckShortEdges(IEnumerable<CEdge> cedgeEb)
        {
            foreach (var cedge in cedgeEb)
            {
                if (CCmpMethods.CmpCptXY(cedge.FrCpt, cedge.ToCpt) == 0)
                {
                    throw new ArgumentException("There is a small edge!");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="TEnumerable"></param>
        /// <param name="cmp"></param>
        /// <returns></returns>
        public static bool ExistDuplicate<T>(IEnumerable<T> TEnumerable, IEqualityComparer<T> EqCmp)
        {
            IEnumerator<T> selfEnumerator = TEnumerable.GetEnumerator();
            if (!selfEnumerator.MoveNext()) throw new ArgumentException("List is empty.", "self");
            if (EqCmp == null) { EqCmp = SCG.EqualityComparer<T>.Default; }

            var ExistingHS = new SCG.HashSet<T>(EqCmp);
            do
            {
                if (ExistingHS.Add(selfEnumerator.Current) == false)
                {
                    return true;
                }
            } while (selfEnumerator.MoveNext());

            return false;
        }


        public static List<CPoint> SetCptLtDirection(List<CPoint> cptlt, bool blnClockwise, bool blnIdentical)
        {
            if (blnClockwise != IsClockwise(cptlt, blnIdentical))
            {
                cptlt.Reverse();
            }
            return cptlt;
        }

        /// <summary>
        /// this function is not helpful for deciding if a cut of a polygon is valid
        /// </summary>
        /// <param name="cptlt"></param>
        /// <param name="blnIdentical"></param>
        /// <returns></returns>
        public static bool IsClockwise(List<CPoint> cptlt, bool blnIdentical)
        {
            //find the left-most lower point
            var llcpt = new CPoint(-1, double.MaxValue, double.MaxValue);  //lower left point            
            foreach (var cpt in cptlt)
            {
                if (CCmpMethods.CmpCptXY(cpt, llcpt, false) == -1)
                {
                    llcpt = cpt;
                }
            }

            cptlt.SetIndexID();
            CPoint precpt;
            CPoint succpt;

            if (llcpt.indexID == 0 || llcpt.indexID == cptlt.Count - 1)
            {
                if (blnIdentical == true)  //in the list, if the first vertex and the last vertex are identical
                {
                    precpt = cptlt[cptlt.Count - 2];
                    succpt = cptlt[1];
                }
                else
                {
                    if (llcpt.indexID == 0)
                    {
                        precpt = cptlt[cptlt.Count - 1];
                        succpt = cptlt[1];
                    }
                    else //if (llcpt.indexID == cptlt.Count-1)
                    {
                        precpt = cptlt[cptlt.Count - 2];
                        succpt = cptlt[0];
                    }
                }
            }
            else
            {
                precpt = cptlt[llcpt.indexID - 1];
                succpt = cptlt[llcpt.indexID + 1];
            }

            return IsClockwise(precpt, llcpt, succpt);
        }

        public static bool IsClockwise(CPoint precpt, CPoint llcpt, CPoint succpt)
        {
            double dblAngle = CGeoFunc.CalAngle_Counterclockwise(precpt, llcpt, succpt);

            if (dblAngle < Math.PI)
            {
                return true;
            }
            else if (dblAngle > Math.PI)
            {
                return false;
            }
            else //it's impossible that dblAngle == Math.PI
            {
                throw new ArgumentException("what happened about the angle?");
            }

            //*********************************************************
            //if you have a incremental list, try following idea:
            //imagine we have a subcptlt which contains points from cpgcptlt[i] to cpgcptlt[j-1]
            //now we add a new point, cpgcptlt[j], into subcptlt. 
            //we want to know if new subcptlt is clockwise or not
            //for (int j = i + 2; j < intCptNum; j++)
            //{
            //    bool blnNewllcpt = (CCmpMethods.CmpCptXY(cpgcptlt[j], llcpt, false) == -1);
            //    if (blnNewllcpt == false &&
            //        llcpt.GID != cpgcptlt[i].GID && llcpt.GID != cpgcptlt[j - 1].GID)
            //    {
            //        //we do not have new llcpt, and llcpt is somewhere in the middle of subcptlt
            //        //blnClockwise not changed
            //    }
            //    else
            //    {
            //        if (blnNewllcpt == false)
            //        {
            //            if (llcpt.GID == cpgcptlt[i].GID)
            //            {
            //                precpt = cpgcptlt[j];
            //            }
            //            else if (llcpt.GID == cpgcptlt[j - 1].GID)
            //            {
            //                succpt = cpgcptlt[j];
            //            }
            //            else
            //            {
            //                throw new ArgumentOutOfRangeException("an impossible case!");
            //            }
            //        }
            //        else
            //        {
            //            llcpt = cpgcptlt[j];
            //            precpt = cpgcptlt[j - 1];
            //            succpt = cpgcptlt[i];
            //        }

            //        blnClockwise = CGeoFunc.IsClockwise(precpt, llcpt, succpt);
            //    }
            //}
        }


        public static IEnumerable<IEnumerable<CPoint>> RemoveClosePointsForCptEbEb(
            IEnumerable<IEnumerable<CPoint>> cptebeb, bool blnIdentical = true )
        {
            foreach (var cpteb in cptebeb)
            {
                yield return RemoveClosePointsForCptEb(cpteb, blnIdentical);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpteb"></param>
        /// <param name="blnIdentical"></param>
        /// <returns></returns>
        /// <remarks>we can guarantee that the first and last vertices of cpteb will be returned
        /// if (blnIdentical == true) or (blnIdentical == false && the first vertex and last vertex are not identical)</remarks>
        public static IEnumerable<CPoint> RemoveClosePointsForCptEb(IEnumerable<CPoint> cpteb, bool blnIdentical = true)
        {
            var cptet = cpteb.GetEnumerator();
            cptet.MoveNext();
            var firstcpt = cptet.Current;
            yield return firstcpt;

            var precpt = firstcpt;
            if (cptet.MoveNext())
            {
               var midcpt = cptet.Current;
                if (cptet.MoveNext())
                {
                    CPoint succpt;
                    do
                    {
                        succpt = cptet.Current;
                        if (CCmpMethods.CmpCptYX(midcpt, precpt) != 0 && CCmpMethods.CmpCptYX(midcpt, succpt) != 0)
                        {
                            yield return midcpt;
                            precpt = midcpt;  //move forward
                        }
                        midcpt = succpt;  //move forward
                    } while (cptet.MoveNext());

                    //now succpt is the last vertex of cpteb
                    if (blnIdentical == true)
                    {
                        yield return succpt;  //return the last vertex
                    }
                    else
                    {
                        //blnIdentical == false means that the first and the last vertices should not be identical
                        if (CCmpMethods.CmpCptYX(succpt, firstcpt) != 0)
                        {
                            yield return succpt;
                        }
                    }
                }
                else //if there are only two vertices, we may return the second vertex
                {
                    if (CCmpMethods.CmpCptYX(precpt, midcpt) != 0)
                    {
                        yield return midcpt;
                    }
                }                
            }
        }

        public static IEnumerable<Path> GetClosedPathEb(Paths paths)
        {
            foreach (var path in paths)
            {
                var closedpath = new Path(path);
                closedpath.Add(path[0]);
                yield return closedpath;
            }

        }

        //public static IEnumerable<CEdge> RemoveShortEdges(IEnumerable<CEdge> cedgeEb)
        //{
        //    foreach (var cedge in cedgeEb)
        //    {
        //        if (CCmpMethods.CmpCptXY(cedge.FrCpt, cedge.ToCpt) == 1)
        //        {
        //            throw new ArgumentException("There is a small edge!");
        //        }
        //    }
        //}

        //public static List<CEdge> RemoveDuplicated()
        //{

        //    return null;
        //}

        #region DisplayInterpolationMix
        /// <summary>
        /// 显示并返回多个插值线段(其中包括缩减的情况)
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>插值线段</returns>
        public static List<CPolyline> DisplayInterpolationMix(CDataRecords pDataRecords, double dblProp)
        {
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return null;
            }
            List<CPolyline> cpllt = new List<CPolyline>();
            List<CCorrespondRiverNet> pCorrespondRiverNetLt = pDataRecords.ParameterResult.CResultCorrespondRiverNetLt;
            for (int i = 0; i < pCorrespondRiverNetLt.Count; i++)
            {
                CRiver pBSMasterRiver = pCorrespondRiverNetLt[i].CBSRiverNet.CMasterStream;
                if (pBSMasterRiver.CCorrRiver != null)
                {
                    CPolyline cpl = CGeoFunc.GetTargetcpl(pBSMasterRiver.CResultPtLt, dblProp); //注意：此处用的是结果点数据“.CResultPtLt”
                    pBSMasterRiver.DisplayCpl = cpl;
                    cpllt.Add(cpl);
                    //处理其支流
                    for (int j = 0; j < pBSMasterRiver.CTributaryLt.Count; j++)
                    {
                        RecursiveDisplayExistCorr(cpllt, pBSMasterRiver.CTributaryLt[j], dblProp);
                    }
                }
                else
                {
                    CPolyline cpl = ReductionIpl(pBSMasterRiver as IPolyline5, dblProp);
                    pBSMasterRiver.DisplayCpl = cpl;
                    cpllt.Add(cpl);
                    //处理其支流
                    for (int j = 0; i < pBSMasterRiver.CTributaryLt.Count; j++)
                    {
                        RecursiveDisplayNotExistCorr(cpllt, pBSMasterRiver.CTributaryLt[j], dblProp);
                    }
                }
            }

            // 清除绘画痕迹
            IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            m_mapControl.ActiveView.Refresh();
            CDrawInActiveView.ViewPolylines(m_mapControl, cpllt);
            return cpllt;
        }

        /// <summary>处理上一级河流有对应河流的情况</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="dblLengthSumRatio">小比例尺表达河流</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <returns>若两河流为同一河流，返回true，否则返回false</returns>
        /// <remarks>RecursiveDisplayExistCorr：RecursiveDealWithExistCorrepondenceRiver
        /// 此处要考虑两个情况，即在其干流有对应河流的情况下，本级河流可能有对应河流，也可能没有对应河流</remarks>
        private static void RecursiveDisplayExistCorr(List<CPolyline> cpllt, CRiver pBSRiver, double dblProp)
        {

            if (pBSRiver.CCorrRiver != null)  //干流有对应河流，且本级河流也有对应河流
            {
                IPolyline5 ipl = GetTargetIplWOLastPoint(pBSRiver.CResultPtLt, dblProp);  //注意：此处用的是结果点数据“.CResultPtLt”
                IPolyline5 displayipl = DWIntersect(pBSRiver.CMainStream.DisplayCpl, ipl, pBSRiver.Tocpt2, dblProp);//添加最后一个点并处理相交问题
                CPolyline cpl = new CPolyline(0, displayipl);
                pBSRiver.DisplayCpl = cpl;
                cpllt.Add(cpl);

                //处理其支流
                for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
                {
                    RecursiveDisplayExistCorr(cpllt, pBSRiver.CTributaryLt[i], dblProp);
                }
            }
            else
            {
                //生成不包含最后一个点的线段
                IPointCollection4 pColBSRiver = pBSRiver as IPointCollection4;
                IPointCollection4 pCol = new PolylineClass();
                pCol.AddPointCollection(pColBSRiver);
                pCol.RemovePoints(pCol.PointCount - 1, 1);
                IPolyline5 ipl = pCol as IPolyline5;
                IPolyline5 cuttedipl = DWIntersect(pBSRiver.CMainStream.DisplayCpl, ipl, pBSRiver.Tocpt2, dblProp);//添加最后一个点并处理相交问题
                CPolyline displaycpl = ReductionIpl(cuttedipl, dblProp);
                pBSRiver.DisplayCpl = displaycpl;
                cpllt.Add(displaycpl);

                //处理其支流
                for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
                {
                    RecursiveDisplayNotExistCorr(cpllt, pBSRiver.CTributaryLt[i], dblProp);
                }
            }
        }

        /// <summary>处理上一级河流没有对应河流的情况</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="pBSRiver">当前大比例尺表达河流</param>
        /// <param name="dblMainReductionRatio">上一级河流缩减比率</param>
        /// <remarks>RecursiveDisplayNotExistCorr：RecursiveDealWithNotExistCorrepondenceRiver</remarks>
        private static void RecursiveDisplayNotExistCorr(List<CPolyline> cpllt, CRiver pBSRiver, double dblProp)
        {
            double dblReductionRatio = pBSRiver.dblReductionRatio * dblProp;
            if (dblReductionRatio < 1)
            {
                CPolyline displaycpl = ReductionIpl(pBSRiver as IPolyline5, dblReductionRatio);
                cpllt.Add(displaycpl);

                //处理其支流
                for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
                {
                    RecursiveDisplayNotExistCorr(cpllt, pBSRiver.CTributaryLt[i], dblProp);
                }
            }
        }

        /// <summary>收缩线段</summary>
        /// <param name="ipl">被收缩的线段</param>
        /// <param name="dblReductionRatio">缩减的量</param>
        /// <returns >收缩后的线段</returns>
        private static CPolyline ReductionIpl(IPolyline5 ipl, double dblReductionRatio)
        {
            ICurve pCurve = new PolylineClass();
            ipl.GetSubcurve(dblReductionRatio, 1, true, out pCurve);
            CPolyline cpl = new CPolyline(0, pCurve);
            return cpl;
        }
        #endregion

        #region DisplayInterpolationMixCut
        /// <summary>
        /// 显示并返回多个插值线段(其中包括筛选的情况)
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>插值线段</returns>
        public static List<CPolyline> DisplayInterpolationMixCut(CDataRecords pDataRecords, double dblProp)
        {
            long lngStartTime = System.Environment.TickCount;
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return null;
            }
            List<CPolyline> cpllt = new List<CPolyline>();
            List<CCorrespondRiverNet> pCorrespondRiverNetLt = pDataRecords.ParameterResult.CResultCorrespondRiverNetLt;
            for (int i = 0; i < pCorrespondRiverNetLt.Count; i++)
            {
                CRiver pBSMasterRiver = pCorrespondRiverNetLt[i].CBSRiverNet.CMasterStream;
                if (pBSMasterRiver.CCorrRiver != null)
                {
                    CPolyline cpl = CGeoFunc.GetTargetcpl(pBSMasterRiver.CResultPtLt, dblProp); //注意：此处用的是结果点数据“.CResultPtLt”
                    pBSMasterRiver.DisplayCpl = cpl;
                    cpllt.Add(cpl);
                    //处理其支流
                    for (int j = 0; j < pBSMasterRiver.CTributaryLt.Count; j++)
                    {
                        RecursiveDisplayExistCorrCut(cpllt, pBSMasterRiver.CTributaryLt[j], dblProp);
                    }
                }
                else
                {
                    if (pBSMasterRiver.dblCutTime > dblProp)
                    {
                        CPolyline cpl = new CPolyline(pBSMasterRiver);
                        cpllt.Add(cpl);
                        //处理其支流
                        for (int j = 0; i < pBSMasterRiver.CTributaryLt.Count; j++)
                        {
                            RecursiveDisplayNotExistCorrCut(cpllt, pBSMasterRiver.CTributaryLt[j], dblProp);
                        }
                    }
                }
            }

            // 清除绘画痕迹
            IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            m_mapControl.ActiveView.Refresh();
            CDrawInActiveView.ViewPolylines(m_mapControl, cpllt);

            //计算时间
            long lngEndTime = System.Environment.TickCount;
            long lngTime = lngEndTime - lngStartTime;
            pDataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            return cpllt;
        }

        /// <summary>处理上一级河流有对应河流的情况</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="dblLengthSumRatio">小比例尺表达河流</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <returns>若两河流为同一河流，返回true，否则返回false</returns>
        /// <remarks>RecursiveDisplayExistCorr：RecursiveDealWithExistCorrepondenceRiver
        /// 此处要考虑两个情况，即在其干流有对应河流的情况下，本级河流可能有对应河流，也可能没有对应河流</remarks>
        private static void RecursiveDisplayExistCorrCut(List<CPolyline> cpllt, CRiver pBSRiver, double dblProp)
        {
            if (pBSRiver.CCorrRiver != null)  //干流有对应河流，且本级河流也有对应河流
            {
                IPolyline5 ipl = GetTargetIplWOLastPoint(pBSRiver.CResultPtLt, dblProp);  //注意：此处用的是结果点数据“.CResultPtLt”
                IPolyline5 displayipl = DWIntersect(pBSRiver.CMainStream.DisplayCpl, ipl, pBSRiver.Tocpt2, dblProp);//添加最后一个点并处理相交问题
                CPolyline cpl = new CPolyline(0, displayipl);
                pBSRiver.DisplayCpl = cpl;
                cpllt.Add(cpl);

                //处理其支流
                for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
                {
                    RecursiveDisplayExistCorrCut(cpllt, pBSRiver.CTributaryLt[i], dblProp);
                }
            }
            else
            {
                if (pBSRiver.dblCutTime > dblProp)
                {
                    //生成不包含最后一个点的线段
                    IPointCollection4 pColBSRiver = pBSRiver as IPointCollection4;
                    IPointCollection4 pCol = new PolylineClass();
                    pCol.AddPointCollection(pColBSRiver);
                    pCol.RemovePoints(pCol.PointCount - 1, 1);
                    IPolyline5 ipl = pCol as IPolyline5;
                    IPolyline5 cuttedipl = DWIntersect(pBSRiver.CMainStream.DisplayCpl, ipl, pBSRiver.Tocpt2, dblProp);//添加最后一个点并处理相交问题
                    CPolyline displaycpl = new CPolyline(0, cuttedipl);
                    cpllt.Add(displaycpl);

                    //处理其支流
                    for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
                    {
                        RecursiveDisplayNotExistCorrCut(cpllt, pBSRiver.CTributaryLt[i], dblProp);
                    }
                }
            }
        }

        /// <summary>处理上一级河流没有对应河流的情况(切割)</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="pBSRiver">当前大比例尺表达河流</param>
        /// <param name="dblMainReductionRatio">上一级河流缩减比率</param>
        /// <remarks>RecursiveDisplayNotExistCorr：RecursiveDealWithNotExistCorrepondenceRiver</remarks>
        private static void RecursiveDisplayNotExistCorrCut(List<CPolyline> cpllt, CRiver pBSRiver, double dblProp)
        {
            if (pBSRiver.dblCutTime > dblProp)
            {
                CPolyline cpl = new CPolyline(pBSRiver);
                cpllt.Add(cpl);
                //处理其支流
                for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
                {
                    RecursiveDisplayNotExistCorrCut(cpllt, pBSRiver.CTributaryLt[i], dblProp);
                }
            }
        }
        #endregion



    }
}
