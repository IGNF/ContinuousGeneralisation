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
    public class CUnifyIndicesPolylines : CMorphingBaseCpl
    {


        public CUnifyIndicesPolylines()
        {

        }


        public CUnifyIndicesPolylines(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline ,CPolyline>(ParameterInitialize, 2);
        }


        public void UnifyIndicesPolylines()
        {
            CParameterInitialize ParameterInitialize = _ParameterInitialize;
            //List<CPolyline> pCPlLt = this.ObjCGeoLtLt[0].ToExpectedClass<CPolyline, object>().ToList();

            List<CPolyline> LSCPlLt = this.ObjCGeoLtLt[0].ToExpectedClass<CPolyline, object>().ToList();
            List<CPolyline> SSCPlLt = this.ObjCGeoLtLt[1].ToExpectedClass<CPolyline, object>().ToList();

            long lngStartTime = System.Environment.TickCount;

            MatchPolyline(ref LSCPlLt, ref SSCPlLt);
            List<CPolyline> LSAttentionCPlLt = new List<CPolyline>();
            List<CPolyline> SSAttentionCPlLt = new List<CPolyline>();
            List<CCorrespondSegment> pCorrespondCPlLt = UnifyIndices(ref LSCPlLt, ref SSCPlLt, ref LSAttentionCPlLt, ref SSAttentionCPlLt);


            //CSaveFeature.SaveCGeoEb(SingleCPlLt, esriGeometryType.esriGeometryPolyline, "SingleCPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            CSaveFeature.SaveCGeoEb(SSAttentionCPlLt, esriGeometryType.esriGeometryPolyline, "SSAttentionCPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            CSaveFeature.SaveCGeoEb(LSAttentionCPlLt, esriGeometryType.esriGeometryPolyline, "LSAttentionCPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            
            List<CPolyline> pLSCPlLt = new List<CPolyline>();   //the code has to be checked**************************************
            List<CPolyline> pSSCPlLt = new List<CPolyline>();   //the code has to be checked**************************************
            for (int i = 0; i < pCorrespondCPlLt.Count; i++)   //the code has to be checked**************************************
            {
                pLSCPlLt.Add(pCorrespondCPlLt[i].CFrPolyline);
                pSSCPlLt.Add(pCorrespondCPlLt[i].CToPolyline);
            }

            //pLSCPlLt and pSSCPlLt has to be saved because there are may be LSAttentionCPlLt and SSAttentionCPlLt
            CSaveFeature.SaveCGeoEb(pSSCPlLt, esriGeometryType.esriGeometryPolyline, ParameterInitialize.pFLayerLt[1].Name + "SSCPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            CSaveFeature.SaveCGeoEb(pLSCPlLt, esriGeometryType.esriGeometryPolyline, ParameterInitialize.pFLayerLt[0].Name + "LSCPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
             
        }

        private void MatchPolyline(ref List<CPolyline> pLSCPlLt, ref List<CPolyline> pSSCPlLt)
        {
            CMatchAndMergePolylines.CreateBufferAndNewCorrCGeoLt(pLSCPlLt, pSSCPlLt, _ParameterInitialize);

            double dblOverlapRatio = Convert.ToDouble(_ParameterInitialize.txtOverlapRatio.Text);
            for (int i = 0; i < pLSCPlLt.Count; i++)
            {
                for (int j = 0; j < pSSCPlLt.Count; j++)
                {
                    double dblOverlapArea = CGeometricMethods.CalOverlapArea(pLSCPlLt[i].pBufferGeo, pSSCPlLt[j].pBufferGeo);
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


        public List<CCorrespondSegment> UnifyIndices(ref List<CPolyline> pLSCPlLt, ref List<CPolyline> pSSCPlLt, ref List<CPolyline> LSAttentionCPlLt, ref List<CPolyline> SSAttentionCPlLt)
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

            List<CCorrespondSegment> pCorrespondCPlLt = new List<CCorrespondSegment>(pSSCPlLt.Count);
            foreach (var sscpl in pSSCPlLt)
            {
                if (sscpl.CorrCGeoLt.Count == 1 && sscpl.CorrCGeoLt[0].CorrCGeoLt.Count == 1)
                {
                    CCorrespondSegment pCorrespondCPl = new CCorrespondSegment(sscpl.CorrCGeoLt[0], sscpl);
                    pCorrespondCPlLt.Add(pCorrespondCPl);
                }
            }

            return pCorrespondCPlLt;
        }
    }
}
