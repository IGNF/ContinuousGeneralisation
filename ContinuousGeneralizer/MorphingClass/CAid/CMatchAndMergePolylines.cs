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
    public class CMatchAndMergePolylines : CMorphingBaseCpl
    {


        public CMatchAndMergePolylines()
        {

        }


        public CMatchAndMergePolylines(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline>(ParameterInitialize, 2);
        }


        public void MatchAndMergePolylines()
        {
            var ParameterInitialize = _ParameterInitialize;
            //List<CPolyline> pCPlLt = this.ObjCGeoLtLt[0].Select(cgeo => cgeo as CPolyline).ToList();

            List<CPolyline> LSCPlLt = this.ObjCGeoLtLt[0].Select(cgeo => cgeo as CPolyline).ToList();
            List<CPolyline> SSCPlLt = this.ObjCGeoLtLt[1].Select(cgeo => cgeo as CPolyline).ToList();

            long lngStartTime = System.Environment.TickCount;
            List<CPolyline> LSAttentionCPlLt = new List<CPolyline>();
            List<CPolyline> SingleCPlLt = MatchPolyline(ref LSCPlLt, ref SSCPlLt, ref LSAttentionCPlLt);
            List<CPolyline> SSAttentionCPlLt = new List<CPolyline>();
            List<CCorrSegment> pCorrCplLt = UnionLSCPl(ref SSCPlLt, ref LSAttentionCPlLt, ref SSAttentionCPlLt);

            //Save


            CSaveFeature.SaveCGeoEb(SSAttentionCPlLt, esriGeometryType.esriGeometryPolyline, "SSAttentionCPl");
            CSaveFeature.SaveCGeoEb(LSAttentionCPlLt, esriGeometryType.esriGeometryPolyline, "LSAttentionCPl");
            CSaveFeature.SaveCGeoEb(SingleCPlLt, esriGeometryType.esriGeometryPolyline, "SingleCPl");
            
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

        private List<CPolyline> MatchPolyline(ref List<CPolyline> pLSCPlLt, ref List<CPolyline> pSSCPlLt, ref List<CPolyline> LSAttentionCPlLt)
        {
           double dblBuffer= CreateBufferAndNewCorrCGeoLt(pLSCPlLt, pSSCPlLt, _ParameterInitialize);


            //find single polylines and correspondences
            List<CPolyline> pSingleCPlLt = new List<CPolyline>();
            for (int i = 0; i < pLSCPlLt.Count; i++)
            {
                if (pLSCPlLt[i].pPolyline.Length < 2 * dblBuffer)  //if the length is too short then we have to care about it
                {
                    LSAttentionCPlLt.Add(pLSCPlLt[i]);
                    //pSingleCPlLt.Add(pLSCPlLt[i]);
                    continue;
                }
                else
                {
                    var pCorrCplInfoSD = new SortedDictionary<double, CCorrCplInfo>(new CDblReverseCompare());
                    double dblBSBufferArea = pLSCPlLt[i].dblBufferArea;

                    //for the larger-scale polyline pLSCPlLt[i], we calculate a pCorrCplInfo for each polyline in pSSCPlLt
                    for (int j = 0; j < pSSCPlLt.Count; j++)
                    {
                        double dblOverlapArea = CGeoFunc.CalOverlapArea(pLSCPlLt[i].pBufferGeo, pSSCPlLt[j].pBufferGeo);
                        double dblOverlapRatio = dblOverlapArea / dblBSBufferArea;
                        var pCorrCplInfo = new CCorrCplInfo(pSSCPlLt[j], dblOverlapRatio, dblOverlapArea);
                        pCorrCplInfoSD.Add(dblOverlapRatio, pCorrCplInfo);
                    }

                    //we count that how many smaller-scale polylines with dblOverlapRatio > 0.5
                    //if the count = 0, there is no correspondence for the pLSCPlLt[i]
                    //if the count = 1, there is one correspondence for the pLSCPlLt[i]
                    //if the count > 1, there is a problem
                    int intCount = 0;
                    foreach (var pCorrCplInfoKvp in pCorrCplInfoSD)
                    {
                        if (pCorrCplInfoKvp.Value.dblOverlapRatio > 0.5)
                        {
                            intCount++;
                        }
                        else
                        {
                            break;
                        }
                    }

                    if (intCount == 0)
                    {
                        pSingleCPlLt.Add(pLSCPlLt[i]);
                    }
                    else if (intCount == 1)
                    {
                        CCorrCplInfo pCorrCplInfo2 = pCorrCplInfoSD.Values.First();
                        pLSCPlLt[i].CorrCGeoLt.Add(pCorrCplInfo2.pCorrCpl);
                        pCorrCplInfo2.pCorrCpl.CorrCGeoLt.Add(pLSCPlLt[i]);
                    }
                    else
                    {
                        LSAttentionCPlLt.Add(pLSCPlLt[i]);
                        //pSingleCPlLt.Add(pLSCPlLt[i]);
                    }
                }
            }


            return pSingleCPlLt;
        }


        public static double CreateBufferAndNewCorrCGeoLt(List<CPolyline> pLSCPlLt, List<CPolyline> pSSCPlLt, 
            CParameterInitialize ParameterInitialize)
        {
            //double dblBuffer = 0;
            //switch (ParameterInitialize.cboBuffer.SelectedIndex)
            //{
            //    case 0:
            //        dblBuffer = CConstants.dblMidLength;
            //        break;
            //    case 1:
            //        dblBuffer = CGeoFunc.CalMidLength<CPolyline, CPolyline>(pSSCPlLt);
            //        break;
            //    case 2:
            //        dblBuffer = CConstants.dblVerySmallCoord;  //if we are sure the corresponding polylines are exactly the same
            //        break;
            //    default:
            //        break;
            //}


            ////create buffers and new the list CorrCPlLt
            //List<IGeometry> LSBufferLt = new List<IGeometry>(pLSCPlLt.Count);
            //for (int i = 0; i < pLSCPlLt.Count; i++)
            //{
            //    pLSCPlLt[i].CreateBuffer(dblBuffer);
            //    pLSCPlLt[i].CorrCGeoLt = new List<CPolyline>();

            //    LSBufferLt.Add(pLSCPlLt[i].pBufferGeo);
            //}
            //List<IGeometry> SSBufferLt = new List<IGeometry>(pSSCPlLt.Count);
            //for (int i = 0; i < pSSCPlLt.Count; i++)
            //{
            //    pSSCPlLt[i].CreateBuffer(dblBuffer);
            //    pSSCPlLt[i].CorrCGeoLt = new List<CPolyline>();

            //    SSBufferLt.Add(pSSCPlLt[i].pBufferGeo);
            //}

            ////the code has to be checked**************************************
            //CSaveFeature.SaveIGeoEb(LSBufferLt, esriGeometryType.esriGeometryPolygon, "LSBuffer", blnVisible: false);   
            //CSaveFeature.SaveIGeoEb(SSBufferLt, esriGeometryType.esriGeometryPolygon, "SSBuffer", blnVisible: false);

            //return dblBuffer;
            return 0;
        }

        public List<CCorrSegment> UnionLSCPl(ref List<CPolyline> pSSCPlLt, ref List<CPolyline> LSAttentionCPlLt, ref List<CPolyline> SSAttentionCPlLt)
        {
            for (int i = 0; i < pSSCPlLt.Count; i++)
            {
                pSSCPlLt[i].SetBaseLine(true);
                if (pSSCPlLt[i].CorrCGeoLt.Count == 0)  //if a Smaller-scale polyline doesn't have any correspondecnes, we have to care about it
                {
                    SSAttentionCPlLt.Add(pSSCPlLt[i]);
                }
                else  //the codes here can be improved, using a SortedSet
                {
                    //merge larger-scale polylines according to smaller-scale polylines
                    List<CPolyline> CorrCPlLt = new List<CPolyline>();
                    CorrCPlLt.AddRange(pSSCPlLt[i].CorrCGeoLt as IEnumerable<CPolyline>);

                    CPolyline pUnionPolyline = CorrCPlLt[0].CopyCpl();
                    CorrCPlLt.RemoveAt(0);

                    bool isSuccessful = true;
                    while (CorrCPlLt.Count > 0)
                    {
                        bool isUnioned = false;
                        for (int j = 0; j < CorrCPlLt.Count; j++)
                        {
                            pUnionPolyline.UnionCpl(CorrCPlLt[j], ref isUnioned);

                            if (isUnioned == true)
                            {
                                CorrCPlLt.RemoveAt(j);
                                break;
                            }
                        }

                        if (isUnioned == false)
                        {
                            isSuccessful = false;
                            break;
                        }
                    }
                    pUnionPolyline.SetBaseLine(true);


                    var LSFrX = pUnionPolyline.FrCpt.X;
                    var LSFrY = pUnionPolyline.FrCpt.Y;
                    var LSToX = pUnionPolyline.ToCpt.X;
                    var LSToY = pUnionPolyline.ToCpt.Y;

                    var SSFrX = pSSCPlLt[i].FrCpt.X;
                    var SSFrY = pSSCPlLt[i].FrCpt.Y;
                    var SSToX = pSSCPlLt[i].ToCpt.X;
                    var SSToY = pSSCPlLt[i].ToCpt.Y;



                    //judge whether the two polylines are really corresponding polylines
                    double dblRatio = pUnionPolyline.pBaseLine.dblLength / pSSCPlLt[i].pBaseLine.dblLength;
                    if (dblRatio < CConstants.dblLowerBoundLoose || dblRatio > CConstants.dblUpperBoundLoose)
                    {
                        isSuccessful = false;
                    }

                    if (isSuccessful == false)
                    {
                        LSAttentionCPlLt.AddRange(pSSCPlLt[i].CorrCGeoLt as IEnumerable<CPolyline>);
                        SSAttentionCPlLt.Add(pSSCPlLt[i]);
                    }
                    else
                    {
                        pSSCPlLt[i].CorrCGeo = pUnionPolyline;
                    }
                }
            }

            //ReverseCpl
            //double dblFrDiffX = LSCPlLt[i].pBaseLine.ToCpt.X - LSCPlLt[i].pBaseLine.FrCpt.X;
            //double dblFrDiffY = LSCPlLt[i].pBaseLine.ToCpt.Y - LSCPlLt[i].pBaseLine.FrCpt.Y;
            //double dblToDiffX = SSCPlLt[i].pBaseLine.ToCpt.X - SSCPlLt[i].pBaseLine.FrCpt.X;
            //double dblToDiffY = SSCPlLt[i].pBaseLine.ToCpt.Y - SSCPlLt[i].pBaseLine.FrCpt.Y;
            //double dblAngleDiff = CGeoFunc.CalAngle_Counterclockwise(dblFrDiffX, dblFrDiffY, dblToDiffX, dblToDiffY);

            //if ((Math.Abs(dblAngleDiff) > (Math.PI / 2) && Math.Abs(dblAngleDiff) < (3 * Math.PI / 2)))
            //{
            //    SSCPlLt[i].ReverseCpl();
            //    SSCPlLt[i].SetBaseLine();
            //}

            List<CCorrSegment> pCorrCplLt = new List<CCorrSegment>(pSSCPlLt.Count);
            for (int i = 0; i < pSSCPlLt.Count; i++)
            {
                if (pSSCPlLt[i].CorrCGeo != null && pSSCPlLt[i] != null)
                {
                    CCorrSegment pCorrCpl = new CCorrSegment(pSSCPlLt[i].CorrCGeo as CPolyline, pSSCPlLt[i]);
                    pCorrCplLt.Add(pCorrCpl);
                }
            }
            return pCorrCplLt;
        }
    }
}
