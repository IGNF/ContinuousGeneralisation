using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using MorphingClass.CEntity;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CAid
{
    public class CStatisticsOfDataSets : CMorphingBaseCpl
    {


        public CStatisticsOfDataSets()
        {

        }


        public CStatisticsOfDataSets(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline, CPolyline>(ParameterInitialize, 1);
        }


        public void StatisticsOfDataSets()
        {
            CParameterInitialize ParameterInitialize = _ParameterInitialize;
            List<CPolyline> pCPlLt = this.ObjCGeoLtLt[0].ToExpectedClass<CPolyline, object>().ToList();

            long lngStartTime = System.Environment.TickCount;

            double dbInterLSumLength = 0;
            int intPtCount = 0;
            int intInnerPtNum = 0;
            List<CPoint> EndPtLt = new List<CPoint>(pCPlLt.Count * 2);
            List<CPoint> AllPtLt = new List<CPoint>(pCPlLt.Count * 2);
            foreach (CPolyline cpl in pCPlLt)
            {
                dbInterLSumLength += cpl.pPolyline.Length;

                List<CPoint> cptlt = cpl.CptLt;
                intPtCount += cptlt.Count;
                intInnerPtNum += (cptlt.Count - 2);

                EndPtLt.Add(cptlt[0]);
                EndPtLt.Add(cptlt[cptlt.Count - 1]);
                AllPtLt.AddRange(cptlt);
            }

            C5.LinkedList<CCorrCpts> CorrCptsLt = CGeometricMethods.LookingForNeighboursByGrids(EndPtLt, CConstants.dblVerySmall);
            int intIntersection = CGeometricMethods.GetNumofIntersections(CorrCptsLt);
            int intAlonePt = CGeometricMethods.CountAlonePt(EndPtLt);
            int intRealPtNum = intInnerPtNum + intIntersection + intAlonePt;


            double dblPtLengthRatio = Convert.ToDouble(intPtCount) / Convert.ToDouble(dbInterLSumLength);
            double dblInnerPtLengthRatio = Convert.ToDouble(intInnerPtNum + intIntersection) / Convert.ToDouble(dbInterLSumLength);
            double dblRealPtLengthRatio = Convert.ToDouble(intRealPtNum) / Convert.ToDouble(dbInterLSumLength);   //you may need this

            int intEdgeCount = GetAllReadCEdgeLt<CPolyline>().Count;

            var pCEnv = CGeometricMethods.GetEnvelope(pCPlLt);


            long lngEndTime = System.Environment.TickCount;//记录结束时间
            ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时
        }
    }
}
