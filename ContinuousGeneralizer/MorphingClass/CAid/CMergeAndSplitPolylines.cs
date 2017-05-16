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
    public class CMergeAndSplitPolylines : CMorphingBaseCpl
    {


        public CMergeAndSplitPolylines()
        {

        }


        public CMergeAndSplitPolylines(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline ,CPolyline>(ParameterInitialize, 1);
        }


        public void MergeAndSplitPolylines()
        {
            CParameterInitialize ParameterInitialize = _ParameterInitialize;
            List<CPolyline> pCPlLt = this.ObjCGeoLtLt[0].AsExpectedClass<CPolyline, object>().ToList();

            long lngStartTime = System.Environment.TickCount;

            CDCEL pDCEL = new CDCEL(pCPlLt);
            pDCEL.ConstructDCEL();

            pDCEL.HalfEdgeLt.ForEach(cedge => cedge.isTraversed = false);
            List<CPolyline> OutputCPlLt = new List<CPolyline>();

            int intID = 0;
            foreach (var cedge in pDCEL.HalfEdgeLt)
            {
                if (cedge.isTraversed == true)
                {
                    continue;
                }

                var PreCptLt = new List<CPoint>();
                var SucCptLt = new List<CPoint>();
                SucCptLt.Add(cedge.ToCpt);
                GetCptLtUntilIntersection(SucCptLt, cedge.cedgeNext);
                if (SucCptLt.Count == 1 || SucCptLt.GetLastT().GID != SucCptLt[0].GID)   //if it is not a "hole"
                {
                    PreCptLt.Add(cedge.FrCpt);
                    cedge.isTraversed = true;
                    cedge.cedgeTwin.isTraversed = true;

                    GetCptLtUntilIntersection(PreCptLt, cedge.cedgeTwin.cedgeNext);
                }

                PreCptLt.Reverse();
                PreCptLt.AddRange(SucCptLt);
                OutputCPlLt.Add(new CPolyline(intID++, PreCptLt));
            }
            CSaveFeature.SaveCGeoEb(OutputCPlLt, esriGeometryType.esriGeometryPolyline, ParameterInitialize.pFLayerLt[0].Name + "MergeAndSplit", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            long lngEndTime = System.Environment.TickCount;//记录结束时间
            ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时
        }

        public void GetCptLtUntilIntersection(List<CPoint> CptLt, CEdge cedge)
        {
            //var CptLt = new List<CPoint>();
            

            var CurrentCEdge = cedge;
            while (CDCEL.IsVertexIntersection(CurrentCEdge.FrCpt) == false && CurrentCEdge.isTraversed == false)  //CurrentCEdge.isTraversed == false is useful when it is a "hole"
            {
                CptLt.Add(CurrentCEdge.ToCpt);
                CurrentCEdge.isTraversed = true;
                CurrentCEdge.cedgeTwin.isTraversed = true;

                CurrentCEdge = CurrentCEdge.cedgeNext;
            }


            //do
            //{
            //    CptLt.Add(CurrentCEdge.ToCpt);
            //    CurrentCEdge.isTraversed = true;
            //    CurrentCEdge.cedgeTwin.isTraversed = true;

            //    CurrentCEdge = CurrentCEdge.cedgeNext;
            //} while (CDCEL.IsVertexIntersection(CurrentCEdge.FrCpt) == false);

            //return CptLt;
        }
    }
}
