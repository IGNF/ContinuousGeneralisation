using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using MorphingClass.CEntity;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CAid
{
    public class CMatchAndMergePolygons : CMorphingBaseCpg
    {


        public CMatchAndMergePolygons()
        {
            
        }


        public CMatchAndMergePolygons(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolygon>(ParameterInitialize, 2);
        }


        public void MatchAndMergePolygons()
        {
            var ParameterInitialize = _ParameterInitialize;
            var pLSCPgLt = this.ObjCGeoLtLt[0].Select(cgeo => cgeo as CPolygon).ToList();
            var pSSCPgLt = this.ObjCGeoLtLt[1].Select(cgeo => cgeo as CPolygon).ToList();

            long lngStartTime = System.Environment.TickCount;
            List<CPolygon> InterLSAttentionCPgLt = new List<CPolygon>();
            List<CPolygon> SSAttentionCPgLt = new List<CPolygon>();
            MatchCpg(ref pLSCPgLt, ref pSSCPgLt, ref InterLSAttentionCPgLt, ref SSAttentionCPgLt);
            List<CPolygon> ResultCPgLt = MergeCpg(ref pSSCPgLt);

            CSaveFeature.SaveCGeoEb(SSAttentionCPgLt, esriGeometryType.esriGeometryPolygon, 
                ParameterInitialize.cboLayerLt[1].Text + "_Attention");            
            CSaveFeature.SaveCGeoEb(InterLSAttentionCPgLt, esriGeometryType.esriGeometryPolygon,
                ParameterInitialize.cboLayerLt[0].Text + "_Attention");
            CSaveFeature.SaveCGeoEb(ResultCPgLt, esriGeometryType.esriGeometryPolygon, 
                ParameterInitialize.cboLayerLt[0].Text + "_Merged");
         }

        public static void MatchCpg(ref List<CPolygon> pLSCPgLt, ref List<CPolygon> pSSCPgLt, ref List<CPolygon> InterLSAttentionCPgLt, ref List<CPolygon> SSAttentionCPgLt)
        {
            //for a SSCPg, we calculate the intersect area with each exsiting InterLSCPg, and calcuate the intersect area
            foreach (var cpg in pLSCPgLt)
            {
                cpg.isMatched = false;
            }

            foreach (var pSSCPg in pSSCPgLt)
            {
                pSSCPg.CorrCGeoLt = new List<CGeoBase>();
                ITopologicalOperator pSSCPgTop = pSSCPg.pPolygon as ITopologicalOperator;

                foreach (var cpg in pLSCPgLt)
                {
                    if (cpg.isMatched == false)
                    {
                        IPolygon4 ipg = cpg.pPolygon;
                        double dblIntersectArea = (pSSCPgTop.Intersect(ipg as IGeometry, esriGeometryDimension.esriGeometry2Dimension) as IArea).Area;
                        double dblIntersectRatio = dblIntersectArea / (ipg as IArea).Area;

                        if (dblIntersectRatio > 0.5)
                        {
                            pSSCPg.CorrCGeoLt.Add(cpg);
                            cpg.isMatched = true;
                        }
                    }
                }

                if (pSSCPg.CorrCGeoLt.Count == 0)
                {
                    SSAttentionCPgLt.Add(pSSCPg);
                }

            }


            //InterLSAttentionCPgLt
            foreach (var lscpg in pLSCPgLt)
            {
                if (lscpg.isMatched == false)
                {
                    InterLSAttentionCPgLt.Add(lscpg);
                }
            }
        }


        public static List<CPolygon> MergeCpg(ref List<CPolygon> pSSCPgLt)
        {
            var ResultCPgLt = new List<CPolygon>(pSSCPgLt.Count);
            foreach (var pSSCPg in pSSCPgLt)
            {
                ResultCPgLt.Add(CGeoFunc.MergeCpgLtDCEL(pSSCPg.CorrCGeoLt as IEnumerable<CPolygon>));

                //for (int j = 0; j < pSSCPg.CorrCGeoLt.Count; j++)
                //{
                //    pSSCPg.CorrCGeoLt[j].Clear();
                //}
            }

            return ResultCPgLt;
        } 









    }
}
