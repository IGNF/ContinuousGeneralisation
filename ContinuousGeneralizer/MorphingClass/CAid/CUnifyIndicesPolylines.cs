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
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CAid
{
    public class CUnifyIndicesPolylines : CMorphingBaseCpl
    {


        public CUnifyIndicesPolylines()
        {

        }


        public CUnifyIndicesPolylines(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline>(ParameterInitialize);
        }


        public void UnifyIndicesPolylines()
        {
            var ParameterInitialize = _ParameterInitialize;
            //List<CPolyline> pCPlLt = this.ObjCGeoLtLt[0].Select(cgeo => cgeo as CPolyline).ToList();

            List<CPolyline> LSCPlLt = this.ObjCGeoLtLt[0].Select(cgeo => cgeo as CPolyline).ToList();
            List<CPolyline> SSCPlLt = this.ObjCGeoLtLt[1].Select(cgeo => cgeo as CPolyline).ToList();

            long lngStartTime = System.Environment.TickCount;

            MatchPolyline(ref LSCPlLt, ref SSCPlLt);
            List<CPolyline> LSAttentionCPlLt = new List<CPolyline>();
            List<CPolyline> SSAttentionCPlLt = new List<CPolyline>();
            List<CCorrSegment> pCorrCplLt = UnifyIndices(ref LSCPlLt, ref SSCPlLt, ref LSAttentionCPlLt, ref SSAttentionCPlLt);


            //CSaveFeature.SaveCGeoEb(SingleCPlLt, esriGeometryType.esriGeometryPolyline, "SingleCPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            CSaveFeature.SaveCGeoEb(SSAttentionCPlLt, esriGeometryType.esriGeometryPolyline, "SSAttentionCPl");
            CSaveFeature.SaveCGeoEb(LSAttentionCPlLt, esriGeometryType.esriGeometryPolyline, "LSAttentionCPl");
            
            List<CPolyline> pLSCPlLt = new List<CPolyline>();   //the code has to be checked**************************************
            List<CPolyline> pSSCPlLt = new List<CPolyline>();   //the code has to be checked**************************************
            for (int i = 0; i < pCorrCplLt.Count; i++)   //the code has to be checked**************************************
            {
                pLSCPlLt.Add(pCorrCplLt[i].CFrPolyline);
                pSSCPlLt.Add(pCorrCplLt[i].CToPolyline);
            }

            //pLSCPlLt and pSSCPlLt has to be saved because there are may be LSAttentionCPlLt and SSAttentionCPlLt
            CSaveFeature.SaveCGeoEb(pSSCPlLt, esriGeometryType.esriGeometryPolyline, ParameterInitialize.pFLayerLt[1].Name + "SSCPl");
            CSaveFeature.SaveCGeoEb(pLSCPlLt, esriGeometryType.esriGeometryPolyline, ParameterInitialize.pFLayerLt[0].Name + "LSCPl");
             
        }

        private void MatchPolyline(ref List<CPolyline> pLSCPlLt, ref List<CPolyline> pSSCPlLt)
        {
            CMatchAndMergePolylines.CreateBufferAndNewCorrCGeoLt(pLSCPlLt, pSSCPlLt, _ParameterInitialize);

            double dblOverlapRatio = Convert.ToDouble(_ParameterInitialize.txtOverlapRatio.Text);
            for (int i = 0; i < pLSCPlLt.Count; i++)
            {
                for (int j = 0; j < pSSCPlLt.Count; j++)
                {
                    double dblOverlapArea = CGeoFunc.CalOverlapArea(pLSCPlLt[i].pBufferGeo, pSSCPlLt[j].pBufferGeo);
                    double dblOverlapRatioLS = dblOverlapArea / pLSCPlLt[i].dblBufferArea;
                    double dblOverlapRatioSS = dblOverlapArea / pSSCPlLt[j].dblBufferArea;

                    if (dblOverlapRatioLS > dblOverlapRatio && dblOverlapRatioSS > dblOverlapRatio)
                    {
                        pLSCPlLt[i].CorrCGeoLt.Add(pSSCPlLt[j]);
                        pSSCPlLt[j].CorrCGeoLt.Add(pLSCPlLt[i]);
                    }
                }

            }
        }


        public List<CCorrSegment> UnifyIndices(ref List<CPolyline> pLSCPlLt, ref List<CPolyline> pSSCPlLt, ref List<CPolyline> LSAttentionCPlLt, ref List<CPolyline> SSAttentionCPlLt)
        {
            foreach (var lscpl in pLSCPlLt)
            {
                if (lscpl.CorrCGeoLt.Count != 1)
                {
                    LSAttentionCPlLt.Add(lscpl);
                }
            }

            foreach (var sscpl in pSSCPlLt)
            {
                if (sscpl.CorrCGeoLt.Count != 1)
                {
                    SSAttentionCPlLt.Add(sscpl);
                }
            }

            List<CCorrSegment> pCorrCplLt = new List<CCorrSegment>(pSSCPlLt.Count);
            foreach (var sscpl in pSSCPlLt)
            {
                if (sscpl.CorrCGeoLt.Count == 1 && sscpl.CorrCGeoLt[0].CorrCGeoLt.Count == 1)
                {
                    CCorrSegment pCorrCpl = new CCorrSegment(sscpl.CorrCGeoLt[0] as CPolyline, sscpl);
                    pCorrCplLt.Add(pCorrCpl);
                }
            }

            return pCorrCplLt;
        }
    }
}
