using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;


using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Editor;

using C5;
using SCG = System.Collections.Generic;

using MorphingClass.CEntity;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeneralizationMethods;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CMorphingMethods
{
    /// <summary>CGABM: Countinuous Generalization of Administrative Boundaries based on Morphing</summary>
    /// <remarks>we set slope for every edge when we put the edges in a grid</remarks>
    public class CCGABM : CMorphingBaseCpl
    {
        //private CDPSimplify _pDPSimplify = new CDPSimplify();

        protected

        delegate IEnumerable<CPolyline> DlgTransform(CParameterInitialize pParameterInitialize, List<IPolyline5> pInterLSIPlLt, List<IPolyline5> pInterSSIPlLt, List<IPolyline5> pSgIPlLt);

        protected int _intLS = 0;
        protected int _intSS = 1;
        protected int _intInterLS = 2;
        protected int _intInterSS = 3;
        protected int _intSg = 4;
        protected int _intInterSg = 5;
        protected int _intTransSg = 5;

        public CCGABM()
        {

        }



        public CCGABM(CParameterInitialize ParameterInitialize, int intLayerCount = 2, int intStartLayer = 0, bool blnIGeoToCGeo = true)
        {
            Construct<CPolyline, CPolyline>(ParameterInitialize, intLayerCount, intStartLayer, blnIGeoToCGeo);
        }

        #region IdentifyAddFaceNumber

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>construct DCEL for InterLSCPlLt, and set the two face numbers for every Cpl. since InterSSCPlLt are stored with the same order as InterLSCPlLt, we also set the same faces numbers for every Cpl from InterSSCPlLt
        /// for a SgCpl, we get a point on it and test in which face this point is</remarks>
        public void IdentifyAddFaceNumber()
        {
            CParameterInitialize pParameterInitialize = _ParameterInitialize;
            _intInterLS = 0;
            _intInterSS = 1;
            _intSg = 2;


            List<CPolyline> pInterLSCPlLt = this.ObjCGeoLtLt[_intInterLS].AsExpectedClass<CPolyline, object>().ToList();
            List<CPolyline> pSgCPlLt = this.ObjCGeoLtLt[_intSg].AsExpectedClass<CPolyline, object>().ToList();

            CDCEL pInterLSDCEL = new CDCEL(pInterLSCPlLt);
            pInterLSDCEL.ConstructDCEL();

            List<object> InterLSobjlt1 = new List<object>(pInterLSCPlLt.Count);
            List<object> InterLSobjlt2 = new List<object>(pInterLSCPlLt.Count);
            for (int i = 0; i < pInterLSCPlLt.Count; i++)
            {
                int indexID1 = pInterLSCPlLt[i].CEdgeLt[0].cpgIncidentFace.indexID;
                int indexID2 = pInterLSCPlLt[i].CEdgeLt[0].cedgeTwin.cpgIncidentFace.indexID;

                int intSmallerindexID;
                int intLargerindexID;
                CHelpFunc.CompareAndOrder(indexID1, indexID2, ID => ID, out intSmallerindexID, out intLargerindexID);  //we store the smaller index at "FaceNum_1", and store the larger index at "FaceNum_2"

                InterLSobjlt1.Add(intSmallerindexID);
                InterLSobjlt2.Add(intLargerindexID);
            }

            //it should be true that a polyline of pInterSSCPlLt has the same indices of faces as the corresponding polyline of pInterLSCPlLt does
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[_intInterLS].FeatureClass, esriFieldType.esriFieldTypeInteger, "FaceNum1", InterLSobjlt1);
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[_intInterLS].FeatureClass, esriFieldType.esriFieldTypeInteger, "FaceNum2", InterLSobjlt2);
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[_intInterSS].FeatureClass, esriFieldType.esriFieldTypeInteger, "FaceNum1", InterLSobjlt1);
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[_intInterSS].FeatureClass, esriFieldType.esriFieldTypeInteger, "FaceNum2", InterLSobjlt2);


            List<object> Sgobjlt = new List<object>(pSgCPlLt.Count);
            for (int i = 0; i < pSgCPlLt.Count; i++)
            {
                Sgobjlt.Add(DetectFaceForSg(pSgCPlLt[i], pInterLSDCEL).indexID);
            }
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[_intSg].FeatureClass, esriFieldType.esriFieldTypeInteger, "FaceNum", Sgobjlt);

        }
        #endregion

        #region Transform

        public void Transform()
        {


            _intInterLS = 0;
            _intInterSS = 1;
            _intSg = 2;


            CParameterInitialize pParameterInitialize = _ParameterInitialize;
            var pstrFieldNameLtLt = this.strFieldNameLtLt;
            var pObjValueLtLtLt = this.ObjValueLtLtLt;

            var InterLSIplLt = this.ObjIGeoLtLt[_intInterLS].AsExpectedClass<IPolyline5, object>().ToList();
            var InterSSIplLt = this.ObjIGeoLtLt[_intInterSS].AsExpectedClass<IPolyline5, object>().ToList();
            var SgIplLt = this.ObjIGeoLtLt[_intSg].AsExpectedClass<IPolyline5, object>().ToList();

            Stopwatch pStopwatch = new Stopwatch();
            pStopwatch.Start();

            var intInterLSFaceNumIndex1 = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[_intInterLS], "FaceNum1");
            var intInterLSFaceNumIndex2 = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[_intInterLS], "FaceNum2");
            var intSgFaceNumIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[_intSg], "FaceNum");

            //count the faces
            var intInterLSFaceNumSS = new SortedSet<int>();
            foreach (var objlt in pObjValueLtLtLt[_intInterLS])
            {
                intInterLSFaceNumSS.Add((int)objlt[intInterLSFaceNumIndex1]);
                intInterLSFaceNumSS.Add((int)objlt[intInterLSFaceNumIndex2]);
            }
            var intInterLSFaceCount = intInterLSFaceNumSS.Count;

            //record the ipolylines into corresponding faces (lists)
            var InterLSIplLtLt = new List<List<IPolyline5>>(intInterLSFaceCount);
            var InterSSIplLtLt = new List<List<IPolyline5>>(intInterLSFaceCount);
            InterLSIplLtLt.EveryElementNew();
            InterSSIplLtLt.EveryElementNew();

            var pInterLSObjValueLtLt = pObjValueLtLtLt[_intInterLS];  //the value table of LSLayer
            for (int i = 0; i < pInterLSObjValueLtLt.Count; i++)
            {
                InterLSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex1]].Add(InterLSIplLt[i]);   //pLSObjValueLtLt[i][intInterLSFaceNumIndex1] is the index of a face
                InterLSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex2]].Add(InterLSIplLt[i]);   //pLSObjValueLtLt[i][intInterLSFaceNumIndex1] is the index of a face

                InterSSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex1]].Add(InterSSIplLt[i]);   //we use the same index, i.e.,pLSObjValueLtLt[i][intInterLSFaceNumIndex1], as we use for LS
                InterSSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex2]].Add(InterSSIplLt[i]);
            }

            var SgIplLtLt = new List<List<IPolyline5>>(intInterLSFaceCount);
            var intSgIndexLtLt = new List<List<int>>(intInterLSFaceCount);
            SgIplLtLt.EveryElementNew();
            intSgIndexLtLt.EveryElementNew();

            var pSgObjValueLtLt = pObjValueLtLtLt[_intSg];  //the value table of LSLayer

            for (int i = 0; i < pSgObjValueLtLt.Count; i++)
            {
                SgIplLtLt[(int)pSgObjValueLtLt[i][intSgFaceNumIndex]].Add(SgIplLt[i]);   //pLSObjValueLtLt[i][intInterLSFaceNumIndex1] is the index of a face
                intSgIndexLtLt[(int)pSgObjValueLtLt[i][intSgFaceNumIndex]].Add(i);   //we record the index of every polyline so that later we can store the transformed polylines with the same orders as SgIplLt
            }

            var dlgTransform = SetDlgTransform(pParameterInitialize.cboTransform.Text);
            var TransSgIGeoLt = new List<IGeometry>(SgIplLt.Count);
            TransSgIGeoLt.EveryElementValue(null);
            CHelpFunc.Displaytspb(0.5, intInterLSFaceCount, pParameterInitialize.tspbMain);
            for (int i = 0; i < intInterLSFaceCount; i++)
            {
                Console.WriteLine("Face Num: " + i);
                if (SgIplLtLt[i].Count != 0)
                {
                    var TransSgCplEb = dlgTransform(pParameterInitialize, InterLSIplLtLt[i], InterSSIplLtLt[i], SgIplLtLt[i]);

                    int intCount = 0;
                    foreach (var TransSgCpl in TransSgCplEb)
                    {
                        TransSgIGeoLt[intSgIndexLtLt[i][intCount++]] = TransSgCpl.JudgeAndSetAEGeometry();
                    }
                }
                CHelpFunc.Displaytspb(i + 1, intInterLSFaceCount, pParameterInitialize.tspbMain);
            }

            pStopwatch.Stop();
            pParameterInitialize.tsslTime.Text = pStopwatch.ElapsedMilliseconds.ToString();
            CSaveFeature.SaveIGeoEb(TransSgIGeoLt, esriGeometryType.esriGeometryPolyline, "TransSgCPlLt" + pParameterInitialize.strSaveFolderName,
                 this.strFieldNameLtLt[_intSg], this.esriFieldTypeLtLt[_intSg], _ObjValueLtLtLt[_intSg]);
        }



        #region CTTransform (compatible triangulation)
        private IEnumerable<CPolyline> CTTransform(CParameterInitialize pParameterInitialize, List<IPolyline5> InterLSIplLt, List<IPolyline5> InterSSIplLt, List<IPolyline5> SgIplLt)
        {
            CConstants.dblVerySmallCoord /= 10;  //this assignment should equal to _dblVerySmallDenominator = 10000000

            var InterLSCplLt = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(InterLSIplLt).ToList();
            var InterSSCplLt = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(InterSSIplLt).ToList();
            var SgCplEb = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(SgIplLt);


            CDCEL pInterLSDCEL = new CDCEL(InterLSCplLt);
            pInterLSDCEL.ConstructDCEL();

            CDCEL pInterSSDCEL = new CDCEL(InterSSCplLt);
            pInterSSDCEL.ConstructDCEL();

            
            //I need to check if we realy need a counter clockwise direction
            var pInterLSCptLt = pInterLSDCEL.FaceCpgLt[1].GetOuterCptEb(false).ToList();  //there are only two faces: the super face and a normal face
            //I need to check if we realy need a counter clockwise direction
            pInterSSDCEL.FaceCpgLt[1].GetOuterCptEb(false).ToList();  //there are only two faces: the super face and a normal face

            var pInterLSCptSD = pInterLSCptLt.ToSD(cpt => cpt, new CCmpCptYX_VerySmall()); //we maintaine this SD so that for a point from single polyline, we can know whether this single point overlaps a point of a larger-scale polyline

            CCptbCtgl pCptbCtgl = new CCptbCtgl(pInterLSDCEL.FaceCpgLt[1], pInterSSDCEL.FaceCpgLt[1], pParameterInitialize);
            pCptbCtgl.ConstructCcptbCtgl();
            var TransSgCPlLt = new List<CPolyline>(SgIplLt.Count);
            foreach (var SgCpl in SgCplEb)
            {
                yield return GenerateCorrSgCpl(pCptbCtgl, SgCpl, pInterLSCptSD);
            }

            CConstants.dblVerySmallCoord *= 10;
        }








        //private List<List<CPolyline>> FindSgCplInSameLSFace(CDCEL pInterLSDCEL, List<CPolyline> pSgCPlLt)
        //{
        //    //initial InsideSameFaceCptLtLt
        //    List<List<CPolyline>> InsideSameLSFaceSgCplLtLt = new List<List<CPolyline>>(pInterLSDCEL.FaceCpgLt.Count);
        //    InsideSameLSFaceSgCplLtLt.EveryElementNew();

        //    foreach (var SgCpl in pSgCPlLt)
        //    {
        //        int intIndex = DetectFaceForSg(SgCpl, pInterLSDCEL).indexID;   //comparing to the method which traverses along DCEL, this method only needs to detect the face once 
        //        InsideSameLSFaceSgCplLtLt[intIndex].Add(SgCpl);
        //    }

        //    return InsideSameLSFaceSgCplLtLt;
        //}

        private CPolygon DetectFaceForSg(CPolyline SgCpl, CDCEL pInterLSDCEL)
        {
            var identitycpt = CGeoFunc.GetInbetweenCpt(SgCpl.CptLt[0], SgCpl.CptLt[1], 0.5);
            return pInterLSDCEL.DetectCloestLeftCorrectCEdge(identitycpt).cpgIncidentFace;   //comparing to the method which traverses along DCEL, this method only needs to detect the face once 
        }

        //private void SgCplTraverse(CTriangulation pFrCtgl, List <CEdge > SgCEdgeLt, ref int intIndex)
        private CPolyline GenerateCorrSgCpl(CCptbCtgl pCptbCtgl, CPolyline SgCpl, SortedDictionary<CPoint, CPoint> pInterLSCptSD)
        {
            CTriangulation pFrCtgl = pCptbCtgl.FrCtgl;
            CTriangulation pToCtgl = pCptbCtgl.ToCtgl;
            var AffineCptLt = new List<CPoint>(SgCpl.CptLt.Count);
            AffineCptLt.Add(CalFirstAffineCpt(pCptbCtgl, SgCpl.CptLt[0], pInterLSCptSD));   //the first vertex

            SgCpl.JudgeAndFormCEdgeLt();
            var CareCEdgeLt = FindCareCEdgeLtForFirstCEdge(pFrCtgl, SgCpl.CEdgeLt[0], pInterLSCptSD);  //CareCEdge is an edge that current edge may cross with
            int intEdgeCount = 0;
            var CurrentCEdge = SgCpl.CEdgeLt[intEdgeCount++];
            do
            {
                bool isFoundExit = false;  //whether this edge can go out current polygon (a triangle)
                foreach (var carecedge in CareCEdgeLt)
                {
                    var pIntersection = CurrentCEdge.IntersectWith(carecedge);

                    switch (pIntersection.enumIntersectionType)
                    {
                        case CEnumIntersectionType.NoNo:
                            break;
                        case CEnumIntersectionType.FrFr:  //this case is actually only for the fisrt vertex of a SgCpl, because for other intersections which coincide a triangulation node, we would not add the two neighbour edges in to CareCEdgeLt
                            CareCEdgeLt = GetCareCEdgeLtCptCoincident(carecedge.FrCpt, CurrentCEdge);
                            isFoundExit = true;
                            break;
                        case CEnumIntersectionType.FrIn:  //this case is actually only for the fisrt vertex of a SgCpl, because for other intersections which coincide a triangulation node, we would not add the two neighbour edges in to CareCEdgeLt
                            CareCEdgeLt = GetCareCEdgeLt(carecedge.cedgeTwin, false);  //this can happen, when the first SgCpt is on an triangle edge of FrCtgl
                            isFoundExit = true;
                            break;
                        case CEnumIntersectionType.FrTo:  //this case is actually only for the fisrt vertex of a SgCpl, because for other intersections which coincide a triangulation node, we would not add the two neighbour edges in to CareCEdgeLt
                            CareCEdgeLt = GetCareCEdgeLtCptCoincident(carecedge.ToCpt, CurrentCEdge);  //this can happen
                            isFoundExit = true;
                            break;
                        case CEnumIntersectionType.InFr:
                            AffineCptLt.Add(pToCtgl.CptLt[carecedge.FrCpt.indexID]);
                            CareCEdgeLt = GetCareCEdgeLtCptCoincident(carecedge.FrCpt, CurrentCEdge);
                            isFoundExit = true;
                            break;
                        case CEnumIntersectionType.InIn:
                            AffineCptLt.Add(ComputeAffineCptForInbetween(pToCtgl, pIntersection));
                            CareCEdgeLt = GetCareCEdgeLt(carecedge.cedgeTwin, false);
                            isFoundExit = true;
                            break;
                        case CEnumIntersectionType.InTo:
                            AffineCptLt.Add(pToCtgl.CptLt[carecedge.ToCpt.indexID]);
                            CareCEdgeLt = GetCareCEdgeLtCptCoincident(carecedge.ToCpt, CurrentCEdge);
                            isFoundExit = true;
                            break;
                        case CEnumIntersectionType.ToFr:   //come to the end of an edge
                            AffineCptLt.Add(pToCtgl.CptLt[carecedge.FrCpt.indexID]);
                            if (intEdgeCount < SgCpl.CEdgeLt.Count)
                            {
                                CurrentCEdge = SgCpl.CEdgeLt[intEdgeCount];
                                CareCEdgeLt = GetCareCEdgeLtCptCoincident(carecedge.FrCpt, CurrentCEdge);
                            }
                            intEdgeCount++;
                            isFoundExit = true;
                            break;
                        case CEnumIntersectionType.ToIn:   //come to the end of an edge
                            AffineCptLt.Add(ComputeAffineCptForInbetween(pToCtgl, pIntersection));
                            if (intEdgeCount < SgCpl.CEdgeLt.Count)
                            {
                                CurrentCEdge = SgCpl.CEdgeLt[intEdgeCount];
                                CareCEdgeLt = GetCareCEdgeLt(carecedge.cedgeTwin, false);
                            }
                            intEdgeCount++;
                            isFoundExit = true;
                            break;
                        case CEnumIntersectionType.ToTo:   //come to the end of an edge
                            AffineCptLt.Add(pToCtgl.CptLt[carecedge.ToCpt.indexID]);
                            if (intEdgeCount < SgCpl.CEdgeLt.Count)
                            {
                                CurrentCEdge = SgCpl.CEdgeLt[intEdgeCount];
                                CareCEdgeLt = GetCareCEdgeLtCptCoincident(carecedge.ToCpt, CurrentCEdge);
                            }
                            intEdgeCount++;
                            isFoundExit = true;
                            break;
                        case CEnumIntersectionType.Overlap:  //maybe we can just ignore overlap, because if there is overlap, then there is also other cases
                            MessageBox.Show("we didn't consider Overlap when GenerateCorrSgCpl in:" + this.ToString() + ".cs   ");
                            break;
                        default:
                            break;
                    }

                    if (isFoundExit == true)
                    {
                        break;
                    }
                }

                if (isFoundExit == false)   //come to the end of an edge
                {
                    AffineCptLt.Add(CalAffineCpt(CurrentCEdge.ToCpt, CareCEdgeLt[0], pFrCtgl, pToCtgl));
                    if (intEdgeCount < SgCpl.CEdgeLt.Count)
                    {
                        CurrentCEdge = SgCpl.CEdgeLt[intEdgeCount];
                        CareCEdgeLt = GetCareCEdgeLt(CareCEdgeLt[0], true);
                    }
                    intEdgeCount++;
                }
            } while (intEdgeCount <= SgCpl.CEdgeLt.Count);

            return new CPolyline(SgCpl.ID, AffineCptLt);
        }

        private CPoint ComputeAffineCptForInbetween(CTriangulation pToCtgl, CIntersection pIntersection)
        {
            var dblProportion = pIntersection.CEdge2.CalInbetweenCptProportion(pIntersection.IntersectCpt);
            return pToCtgl.HalfEdgeLt[pIntersection.CEdge2.indexID].GetInbetweenCpt(dblProportion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pCptbCtgl"></param>
        /// <param name="SgCpt"></param>
        /// <returns></returns>
        /// <remarks >the first or last vertex of a SgCpl can be computed many times, but it doesn't matter</remarks>
        private CPoint CalFirstAffineCpt(CCptbCtgl pCptbCtgl, CPoint SgCpt, SortedDictionary<CPoint, CPoint> pInterLSCptSD)
        {
            CTriangulation pFrCtgl = pCptbCtgl.FrCtgl;
            CTriangulation pToCtgl = pCptbCtgl.ToCtgl;

            CPoint AffineCpt = null;
            CPoint outcpt;
            bool blnContainsKey = pInterLSCptSD.TryGetValue(SgCpt, out outcpt);
            if (blnContainsKey == true)
            {
                AffineCpt = pToCtgl.CptLt[outcpt.indexID];
            }
            else
            {
                AffineCpt = CalFirstSingleAffineCpt(SgCpt, pFrCtgl, pToCtgl);
            }


            //switch (SgCpt.BelongedCPolyline.enumScale)   //we must calculate first affine vertex from Larger or Single seperately, because a vertex from Larger may not have a CloestLeftCEdge
            //{
            //    case CEnumScale.Larger:
            //        AffineCpt = pToCtgl.CptLt[SgCpt.indexID];
            //        break;
            //    case CEnumScale.Single:
            //        AffineCpt = CalFirstSingleAffineCpt(SgCpt, pFrCtgl, pToCtgl);
            //        break;
            //    default:
            //        MessageBox.Show("impossible case in: " + this.ToString() + ".cs   " + "CalAffineCpt");
            //        break;
            //}
            return AffineCpt;
        }

        private CPoint CalFirstSingleAffineCpt(CPoint SgCpt, CTriangulation pFrCtgl, CTriangulation pToCtgl)
        {
            var CurrentCEdge = pFrCtgl.DetectCloestLeftCorrectCEdge(SgCpt);
            return CalAffineCpt(SgCpt, CurrentCEdge, pFrCtgl, pToCtgl);
        }

        private CPoint CalAffineCpt(CPoint SgCpt, CEdge CareCEdge, CTriangulation pFrCtgl, CTriangulation pToCtgl)
        {
            var CurrentCEdge = CareCEdge;
            var trianglecpt = new CPoint[3];
            for (int i = 0; i < 3; i++)
            {
                trianglecpt[i] = CurrentCEdge.FrCpt;
                CurrentCEdge = CurrentCEdge.cedgeNext;
            }

            double dblLamda1, dblLamda2, dblLamda3;
            CGeoFunc.CalBarycentricCoordinates(SgCpt, trianglecpt[0], trianglecpt[1], trianglecpt[2], out dblLamda1, out dblLamda2, out dblLamda3);
            CPoint AffineCpt = CGeoFunc.CalCartesianCoordinates(pToCtgl.CptLt[trianglecpt[0].indexID], pToCtgl.CptLt[trianglecpt[1].indexID],
                                                                         pToCtgl.CptLt[trianglecpt[2].indexID], dblLamda1, dblLamda2, dblLamda3, SgCpt.ID);
            return AffineCpt;
        }

        private List<CEdge> FindCareCEdgeLtForFirstCEdge(CTriangulation pFrCtgl, CEdge FirstSgCEdge, SortedDictionary<CPoint, CPoint> pInterLSCptSD)
        {
            var pFrCpt = FirstSgCEdge.FrCpt;
            List<CEdge> CareCEdgeLt = new List<CEdge>();

            CPoint outcpt;
            bool blnContainsKey = pInterLSCptSD.TryGetValue(pFrCpt, out outcpt);
            if (blnContainsKey == true)
            {
                CareCEdgeLt = GetCareCEdgeLtCptCoincident(outcpt, FirstSgCEdge);
            }
            else
            {
                CareCEdgeLt = GetCareCEdgeLt(pFrCpt.ClosestLeftCIntersection.CEdge2, true);  //we have already computed FirstSgCEdge.FrCpt.ClosestLeftCIntersection.CEdge2 when we computed the affine point for the first point
            }

            return CareCEdgeLt;
        }


        private List<CEdge> GetCareCEdgeLtCptCoincident(CPoint LSNodeCpt, CEdge SgCEdge)
        {
            var SmallerAxisAngleCEdge = CDCEL.FindSmallerAxisAngleCEdgebyCEdge(LSNodeCpt, SgCEdge);
            List<CEdge> CareCEdgeLt = new List<CEdge>();
            CareCEdgeLt.Add(SmallerAxisAngleCEdge.cedgeNext);

            return CareCEdgeLt;
        }

        private List<CEdge> GetCareCEdgeLt(CEdge cedge, bool blnIncludeSelf)
        {
            List<CEdge> CareCEdgeLt;
            if (blnIncludeSelf == false)
            {
                CareCEdgeLt = new List<CEdge>(2);
            }
            else
            {
                CareCEdgeLt = new List<CEdge>(3);
                CareCEdgeLt.Add(cedge);
            }

            var CurrentCEdge = cedge.cedgeNext;
            do
            {
                CareCEdgeLt.Add(CurrentCEdge);
                CurrentCEdge = CurrentCEdge.cedgeNext;

            } while (CurrentCEdge.GID != cedge.GID);

            return CareCEdgeLt;
        }
        #endregion

        #region RSTransform (rubber sheeting)
        //private List<CPolyline> ByRubbersheeting(CParameterInitialize pParameterInitialize, List<CPolyline> pInterLSCPlLt, List<CPolyline> pInterSSCPlLt, List<CPolyline> pSgCPlLt)

        public void ByRubbersheeting()
        {
            //_intInterLS = 0;
            //_intInterSS = 1;
            //_intSg = 2;


            //CParameterInitialize pParameterInitialize = _ParameterInitialize;
            //var pstrFieldNameLtLt = this.strFieldNameLtLt;
            //var pObjValueLtLtLt = this.ObjValueLtLtLt;

            //var InterLSIplLt = this.ObjIGeoLtLt[_intInterLS].AsExpectedClass<IPolyline5, object>().ToList();
            //var InterSSIplLt = this.ObjIGeoLtLt[_intInterSS].AsExpectedClass<IPolyline5, object>().ToList();
            //var SgIplLt = this.ObjIGeoLtLt[_intSg].AsExpectedClass<IPolyline5, object>().ToList();

            //var intInterLSFaceNumIndex1 = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[_intInterLS], "FaceNum1");
            //var intInterLSFaceNumIndex2 = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[_intInterLS], "FaceNum2");
            ////var intInterSSFaceNumIndex1=CSaveFeature .FindFieldNameIndex (pstrFieldNameLtLt[_intInterSS],"FaceNum1");
            ////var intInterSSFaceNumIndex2=CSaveFeature .FindFieldNameIndex (pstrFieldNameLtLt[_intInterSS],"FaceNum2");
            //var intSgFaceNumIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[_intSg], "FaceNum");

            ////count the faces
            //var intInterLSFaceNumSS = new SortedSet<int>();
            //foreach (var objlt in pObjValueLtLtLt[_intInterLS])
            //{
            //    intInterLSFaceNumSS.Add((int)objlt[intInterLSFaceNumIndex1]);
            //    intInterLSFaceNumSS.Add((int)objlt[intInterLSFaceNumIndex2]);
            //}
            //var intInterLSFaceCount = intInterLSFaceNumSS.Count;

            ////record the ipolylines into corresponding faces (lists)
            //var InterLSIplLtLt = new List<List<IPolyline5>>(intInterLSFaceCount);
            //var InterSSIplLtLt = new List<List<IPolyline5>>(intInterLSFaceCount);

            //InterLSIplLtLt.EveryElementNew();
            //InterSSIplLtLt.EveryElementNew();


            //var pInterLSObjValueLtLt = pObjValueLtLtLt[_intInterLS];  //the value table of LSLayer
            //for (int i = 0; i < pInterLSObjValueLtLt.Count; i++)
            //{
            //    InterLSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex1]].Add(InterLSIplLt[i]);   //pLSObjValueLtLt[i][intInterLSFaceNumIndex1] is the index of a face
            //    InterLSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex2]].Add(InterLSIplLt[i]);   //pLSObjValueLtLt[i][intInterLSFaceNumIndex1] is the index of a face


            //    //var test = pInterLSObjValueLtLt[i];
            //    //var test2 = test[intInterLSFaceNumIndex2];
            //    //var test3 = InterLSIplLtLt[(int)test2];

            //    InterSSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex1]].Add(InterSSIplLt[i]);   //we use the same index, i.e.,pLSObjValueLtLt[i][intInterLSFaceNumIndex1], as we use for LS
            //    InterSSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex2]].Add(InterSSIplLt[i]);
            //}

            //var SgIplLtLt = new List<List<IPolyline5>>(intInterLSFaceCount);
            //var intSgIndexLtLt = new List<List<int>>(intInterLSFaceCount);
            //SgIplLtLt.EveryElementNew();
            //intSgIndexLtLt.EveryElementNew();

            //var pSgObjValueLtLt = pObjValueLtLtLt[_intSg];  //the value table of LSLayer

            //for (int i = 0; i < pSgObjValueLtLt.Count; i++)
            //{
            //    SgIplLtLt[(int)pSgObjValueLtLt[i][intSgFaceNumIndex]].Add(SgIplLt[i]);   //pLSObjValueLtLt[i][intInterLSFaceNumIndex1] is the index of a face
            //    intSgIndexLtLt[(int)pSgObjValueLtLt[i][intSgFaceNumIndex]].Add(i);   //we record the index of every polyline so that later we can store the transformed polylines with the same orders as SgIplLt
            //}

            ////CSaveFeature pSFTTransSg = new CSaveFeature(esriGeometryType.esriGeometryPolyline, "TransSgCPlLt" + pParameterInitialize.strSaveFolder, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);

            //var TransSgIGeoLt = new List<IGeometry>(SgIplLt.Count);
            //TransSgIGeoLt.EveryElementNull();
            //for (int i = 0; i < intInterLSFaceCount; i++)
            //{
            //    Console.WriteLine("Face Num: " + i);
            //    if (SgIplLtLt[i].Count != 0)
            //    {
            //        var TransSgCplEb = RubbersheetingTrans(pParameterInitialize, InterLSIplLtLt[i], InterSSIplLtLt[i], SgIplLtLt[i]);

            //        int intCount = 0;
            //        foreach (var TransSgCpl in TransSgCplEb)
            //        {
            //            TransSgIGeoLt[intSgIndexLtLt[i][intCount++]] = TransSgCpl.JudgeAndSetAEGeometry();
            //        }
            //    }
            //    CHelpFunc.Displaytspb(i + 1, intInterLSFaceCount, pParameterInitialize.tspbMain);
            //}

            //CHelpFunc.SaveIGeoLt(TransSgIGeoLt, esriGeometryType.esriGeometryPolyline, "TransSgCPlLt" + pParameterInitialize.strSaveFolder, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl, this.strFieldNameLtLt[_intSg], this.esriFieldTypeLtLt[_intSg], _ObjValueLtLtLt[_intSg]);












            #region old codes (handle all the polygons together)

            //CDCEL pInterLSDCEL, pInterLSMixSgDCEL;
            //ConstructDCELs(pParameterInitialize, pInterLSCPlLt, pSgCPlLt, out pInterLSDCEL, out pInterLSMixSgDCEL);
            //List<CPolyline> TransSgCPlLt = RubberSheetingTransformation(pInterLSMixSgDCEL.HalfEdgeLt, pSgCPlLt, pInterLSDCEL.FaceCpgLt);
            //return TransSgCPlLt;
            #endregion


        }


        private IEnumerable<CPolyline> RSTransform(CParameterInitialize pParameterInitialize, List<IPolyline5> InterLSIplLt, List<IPolyline5> InterSSIplLt, List<IPolyline5> SgIplLt)
        {
            var pInterLSCplLt = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(InterLSIplLt).ToList();
            var pInterSSCplLt = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(InterSSIplLt).ToList();
            var SgCplEb = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(SgIplLt);

            var pCorrCptsLtLt = CGeoFunc.GetCorrCptsLtLt(pInterLSCplLt, pInterSSCplLt);
            CHelpFunc.SetMoveVectorForCorrCptsLtLt(pCorrCptsLtLt);

            var pInterLSCptSD = pInterLSCplLt.GetAllCptEb<CPolyline, CPolyline>().ToSD(cpt => cpt, new CCmpCptYX_VerySmall());
            foreach (var SgCpl in SgCplEb)
            {
                yield return RSGenerateCorrSgCpl(SgCpl, pInterLSCptSD);
            }
        }

        private CPolyline RSGenerateCorrSgCpl(CPolyline SgCpl, SortedDictionary<CPoint, CPoint> pInterLSCptSD)
        {
            var sgcptlt = SgCpl.CptLt;

            var cptlt = new List<CPoint>(sgcptlt.Count);
            cptlt.Add(RSComputeEndCpt(sgcptlt.First(), pInterLSCptSD));  //the first point
            for (int i = 1; i < sgcptlt.Count - 1; i++)                      //other points
            {
                cptlt.Add(RSComputeCpt(sgcptlt[i], pInterLSCptSD));
            }
            cptlt.Add(RSComputeEndCpt(sgcptlt.GetLastT(), pInterLSCptSD));  //the last point

            return new CPolyline(SgCpl.ID, cptlt);
        }

        private CPoint RSComputeEndCpt(CPoint cpt, SortedDictionary<CPoint, CPoint> CptSD)
        {
            CPoint outcpt;
            bool blnContainsKey = CptSD.TryGetValue(cpt, out outcpt);
            if (blnContainsKey == true)
            {
                return outcpt.PairCorrCpt.ToCpt;
            }
            else
            {
                return RSComputeCpt(cpt, CptSD);
            }
        }

        /// <summary>
        /// by IDW
        /// </summary>
        /// <param name="cpt"></param>
        /// <param name="CptSD"></param>
        /// <returns></returns>
        private CPoint RSComputeCpt(CPoint cpt, SortedDictionary<CPoint, CPoint> CptSD)
        {
            double dblIntegralWeightedMoveX = 0;
            double dblIntegralWeightedMoveY = 0;
            double dblIntegralWeight = 0;

            foreach (var cptkvp in CptSD)
            {
                var dblDisSquare = CGeoFunc.CalDisSquare(cpt, cptkvp.Key);
                var dblWeight = 1 / (dblDisSquare);
                var pMoveVector = cptkvp.Key.PairCorrCpt.pMoveVector;
                dblIntegralWeightedMoveX += dblWeight * pMoveVector.X;
                dblIntegralWeightedMoveY += dblWeight * pMoveVector.Y;
                dblIntegralWeight += dblWeight;
            }

            double dblMoveX = dblIntegralWeightedMoveX / dblIntegralWeight;
            double dblMoveY = dblIntegralWeightedMoveY / dblIntegralWeight;

            return new CPoint(cpt.ID, cpt.X + dblMoveX, cpt.Y + dblMoveY);
        }



        private List<CPoint> GenerateFieldCpt(CDCEL pDCEL)
        {
            CEnvelope pEnvelope = pDCEL.pEdgeGrid.pEnvelope;
            double dblCellSize = pDCEL.pEdgeGrid.dblCellWidth / 3;
            int intRow = Convert.ToInt32(Math.Truncate(pEnvelope.Height / dblCellSize)) + 1;
            int intCol = Convert.ToInt32(Math.Truncate(pEnvelope.Width / dblCellSize)) + 1;

            List<CPoint> FieldCptLt = new List<CPoint>(intRow * intCol);
            double dblX = pEnvelope.XMin;
            double dblY = pEnvelope.YMin;
            int intCount = 0;
            for (int i = 0; i < intRow; i++)
            {
                dblX = pEnvelope.XMin;
                for (int j = 0; j < intCol; j++)
                {
                    FieldCptLt.Add(new CPoint(intCount, dblX, dblY));
                    dblX += dblCellSize;
                    intCount++;
                }
                dblY += dblCellSize;
            }

            pDCEL.DetectCloestLeftCorrectCEdge(FieldCptLt);

            return FieldCptLt;
        }

        public List<CPolyline> GenerateFieldCpl(List<CPoint> FieldCptLt)
        {
            List<CPolyline> FieldCplLt = new List<CPolyline>(FieldCptLt.Count);
            foreach (CPoint cpt in FieldCptLt)
            {
                if (cpt.ClosestLeftCIntersection != null)
                {
                    CalSgMoveVectorIDW(cpt, cpt.ClosestLeftCIntersection.CEdge2.cpgIncidentFace2);
                    FieldCplLt.Add(new CPolyline(cpt.PairCorrCpt));
                }
                else
                {
                    FieldCplLt.Add(new CPolyline(0, cpt, cpt));
                }
            }
            return FieldCplLt;
        }

        #region We don't need anymore (for RubberSheeting)
        private List<CPolyline> RubberSheetingTransformation(List<CEdge> LSMixSgHalfEdgeLt, List<CPolyline> SgCPlLt, List<CPolygon> LSFaceLt)
        {
            List<List<CEdge>> InsideSameFaceHalfCEdgeLtLt;
            var InsideSameFaceCptLtLt = FindCptOfSingleInLSFace(LSMixSgHalfEdgeLt, LSFaceLt, out InsideSameFaceHalfCEdgeLtLt);
            for (int i = 0; i < InsideSameFaceCptLtLt.Count; i++)
            {
                if (InsideSameFaceCptLtLt[i].Count > 0)
                {
                    CalSgMoveVectorIDW(InsideSameFaceCptLtLt[i], LSFaceLt[i]);
                }
            }

            //generate transformed single polylines
            List<CPolyline> TransSgCPlLt = new List<CPolyline>(SgCPlLt.Count);
            return CGeoFunc.GenerateCplLtByCorrCpt(SgCPlLt);
        }

        private List<List<CPoint>> FindCptOfSingleInLSFace(List<CEdge> LSMixSgHalfEdgeLt, List<CPolygon> LSFaceLt, out List<List<CEdge>> InsideSameFaceHalfCEdgeLtLt)
        {
            foreach (CEdge cedge in LSMixSgHalfEdgeLt)
            {
                cedge.isTraversed = false;
                cedge.FrCpt.isAdded = false;
            }

            //initial InsideSameFaceCptLtLt
            var InsideSameFaceCptLtLt = new List<List<CPoint>>(LSFaceLt.Count);
            InsideSameFaceHalfCEdgeLtLt = new List<List<CEdge>>(LSFaceLt.Count);
            for (int i = 0; i < LSFaceLt.Count; i++)
            {
                var InsideSameFaceCptLt = new List<CPoint>();
                InsideSameFaceCptLtLt.Add(InsideSameFaceCptLt);

                var InsideSameFaceHalfCEdgeLt = new List<CEdge>();
                InsideSameFaceHalfCEdgeLtLt.Add(InsideSameFaceHalfCEdgeLt);
            }

            //look for points inside the same face
            foreach (CEdge cedge in LSMixSgHalfEdgeLt)
            {
                var InsideSameFaceCptLt = new List<CPoint>();
                var InsideSameFaceHalfCEdgeLt = new List<CEdge>();
                CPolygon LSFace = null;  //from the Larger-Scale map
                RecursivelyCollectingVertices(cedge, ref InsideSameFaceCptLt, ref InsideSameFaceHalfCEdgeLt, ref LSFace);

                if (InsideSameFaceCptLt.Count > 0)   //InsideSameFaceCptLt.Count can certainly be used here. LSFace == null cannot be used, because when an edge belongs to an Larger-Scale polyline we still get a LSFace
                {
                    InsideSameFaceCptLtLt[LSFace.indexID] = InsideSameFaceCptLt;
                    InsideSameFaceHalfCEdgeLtLt[LSFace.indexID] = InsideSameFaceHalfCEdgeLt;
                }
            }

            return InsideSameFaceCptLtLt;
        }


        /// <summary>
        /// Recursively Collect Vertices belong to the same LSFace
        /// </summary>
        /// <remarks>the ends of the single polylines are included</remarks>
        private void RecursivelyCollectingVertices(CEdge LSMixSgHalfEdge, ref List<CPoint> InsideSameFaceCptLt, ref List<CEdge> InsideSameFaceHalfCEdgeLt, ref CPolygon LSFace)
        {
            if (LSMixSgHalfEdge.isTraversed == false)
            {
                LSMixSgHalfEdge.isTraversed = true;
                if (LSMixSgHalfEdge.BelongedCpl.enumScale == CEnumScale.Single)
                {
                    InsideSameFaceHalfCEdgeLt.Add(LSMixSgHalfEdge);
                    if (LSMixSgHalfEdge.FrCpt.isAdded == false)
                    {
                        InsideSameFaceCptLt.Add(LSMixSgHalfEdge.FrCpt);
                        LSMixSgHalfEdge.FrCpt.isAdded = true;
                    }
                    RecursivelyCollectingVertices(LSMixSgHalfEdge.cedgeTwin, ref InsideSameFaceCptLt, ref InsideSameFaceHalfCEdgeLt, ref LSFace);   //twin                    
                }
                else if (LSMixSgHalfEdge.BelongedCpl.enumScale == CEnumScale.Larger)  //the polygon is found
                {
                    LSFace = LSMixSgHalfEdge.cpgIncidentFace2;    //any cpgIncidentFace2 of the edges is available, because they are the same
                    if (LSFace.OuterCmptCEdge == null)   //only the SuperFace dosen't have OuterComponent
                    {
                        return;  //we return here to avoid a very deep recursion, which may lead to StackOverflowException
                    }
                }

                RecursivelyCollectingVertices(LSMixSgHalfEdge.cedgeNext, ref InsideSameFaceCptLt, ref InsideSameFaceHalfCEdgeLt, ref LSFace);   //next
                CollectingVerticesInnerOuterComponents(LSMixSgHalfEdge.cpgIncidentFace, ref InsideSameFaceCptLt, ref InsideSameFaceHalfCEdgeLt, ref LSFace);   //other components
            }
        }

        private void CollectingVerticesInnerOuterComponents(CPolygon LSMoreDetailedFace, ref List<CPoint> InsideSameFaceCptLt, ref List<CEdge> InsideSameFaceHalfCEdgeLt, ref CPolygon LSFace)
        {
            if (LSMoreDetailedFace.OuterCmptCEdge != null)
            {
                RecursivelyCollectingVertices(LSMoreDetailedFace.OuterCmptCEdge, ref InsideSameFaceCptLt, ref InsideSameFaceHalfCEdgeLt, ref LSFace);
            }
            if (LSMoreDetailedFace.InnerCmptCEdgeLt != null)
            {
                foreach (CEdge InnerCEdge in LSMoreDetailedFace.InnerCmptCEdgeLt)
                {
                    RecursivelyCollectingVertices(InnerCEdge, ref InsideSameFaceCptLt, ref InsideSameFaceHalfCEdgeLt, ref LSFace);
                }
            }
        }

        /// <summary>
        /// Calculate the move vectors of single polylines by Inverse Distance Weighting (IDW)
        /// </summary>
        /// <remarks>the ends of the single polylines are included</remarks>
        private void CalSgMoveVectorIDW(List<CPoint> InsideSameFaceCptLt, CPolygon LSFace)
        {
            foreach (CPoint cpt in InsideSameFaceCptLt)
            {
                if (cpt.BelongedCpl.enumScale != CEnumScale.Larger) //If an end of a single polyline is at the same time a vertex of a larger-scale polyline (this single polyline dosen't represent a hole), then MoveVectorPtLt has already been calculated from the corresponding polylines
                {
                    CalSgMoveVectorIDW(cpt, LSFace);
                }
            }
        }

        private void CalSgMoveVectorIDW(CPoint cpt, CPolygon LSFace)
        {
            double dblIntegralWeightedMoveX = 0;
            double dblIntegralWeightedMoveY = 0;
            double dblIntegralWeight = 0;

            if (LSFace.OuterCmptCEdge != null)
            {
                CalSgMoveVectorIDW(cpt, LSFace.OuterCmptCEdge, ref dblIntegralWeightedMoveX, ref dblIntegralWeightedMoveY, ref dblIntegralWeight);
            }

            if (LSFace.InnerCmptCEdgeLt != null)
            {
                foreach (CEdge cedge in LSFace.InnerCmptCEdgeLt)
                {
                    CalSgMoveVectorIDW(cpt, cedge, ref dblIntegralWeightedMoveX, ref dblIntegralWeightedMoveY, ref dblIntegralWeight);
                }
            }

            double dblMoveX = dblIntegralWeightedMoveX / dblIntegralWeight;
            double dblMoveY = dblIntegralWeightedMoveY / dblIntegralWeight;

            CCorrCpts CorrCpt = new CCorrCpts(cpt, dblMoveX, dblMoveY);   // notice that the CorrCpt will be recorded into cpt by the construction of CCorrCpts
        }

        private void CalSgMoveVectorIDW(CPoint cpt, CEdge cedge, ref double dblIntegralWeightedMoveX, ref double dblIntegralWeightedMoveY, ref double dblIntegralWeight)
        {
            double dblDisSquare = CGeoFunc.CalDisSquare(cpt, cedge.FrCpt);
            double dblWeight = 1 / (dblDisSquare);

            CMoveVector pMoveVector = cedge.FrCpt.PairCorrCpt.pMoveVector;
            dblIntegralWeightedMoveX += dblWeight * pMoveVector.X;
            dblIntegralWeightedMoveY += dblWeight * pMoveVector.Y;
            dblIntegralWeight += dblWeight;

            if (cedge.cedgeNext2.isStartEdge2 == false)
            {
                CalSgMoveVectorIDW(cpt, cedge.cedgeNext2, ref dblIntegralWeightedMoveX, ref dblIntegralWeightedMoveY, ref dblIntegralWeight);
            }
        }
        #endregion


        #endregion

        #region shared by ByCptbCtgl and ByRubbersheeting

        private DlgTransform SetDlgTransform(string strTransform)
        {
            switch (strTransform)
            {
                case "Compatible Triangulations":
                    return CTTransform;
                case "Rubber Sheeting":
                    return RSTransform;
                default: throw new ArgumentException("An undefined method! ");
            }
        }

        //private void ConstructDCELs(CParameterInitialize pParameterInitialize, List<CPolyline> pInterLSCPlLt, List<CPolyline> pSgCPlLt, out CDCEL pInterLSDCEL, out CDCEL pInterLSMixSgDCEL)
        //{
        //    pInterLSDCEL = new CDCEL(pInterLSCPlLt);
        //    pInterLSDCEL.ConstructDCEL();
        //    pInterLSDCEL.ExportEdgeAttributes(2);
        //    //List<CPoint> FieldCptLt = GenerateFieldCpt(pInterLSDCEL);            
        //    pInterLSDCEL.ClearEdgeAttributes();

        //    List<CPolyline> BSMixSgCPlLt = new List<CPolyline>(pInterLSCPlLt.Count + pSgCPlLt.Count);
        //    BSMixSgCPlLt.AddRange(pInterLSCPlLt);
        //    BSMixSgCPlLt.AddRange(pSgCPlLt);
        //    pInterLSMixSgDCEL = new CDCEL(BSMixSgCPlLt);
        //    pInterLSMixSgDCEL.ConstructDCEL();  //note the little trick that LSCPlLt is added before SgCPlLt. later we will keep only one vertex at an intersection, the little trick make sure that this kept vertex is from LSCPlLt.
        //    pInterLSMixSgDCEL.ExportEdgeAttributes(1);
        //    pInterLSMixSgDCEL.UpdateCplltEnds();

        //    pSgCPlLt.ForEach(cpl => cpl.enumScale = CEnumScale.Single);
        //    pSgCPlLt.ForEach(cpl => cpl.SetCptBelongedPolyline());
        //    pInterLSCPlLt.ForEach(cpl => cpl.enumScale = CEnumScale.Larger);
        //    pInterLSCPlLt.ForEach(cpl => cpl.SetCptBelongedPolyline());   //we also use a little trick here. we first do "SetCptBelongedCpl" for "SgCPlLt", then do it for "InterLSCPlLt", so that the shared point will use a BSCPl as its Belonged CPl
        //    CHelpFunc.SetCEdgeCEdgeTwinBelongedCpl(ref pInterLSCPlLt);
        //    CHelpFunc.SetCEdgeCEdgeTwinBelongedCpl(ref pSgCPlLt);
        //}



        #region Simplify Transformed single polylines

        public int ComputeDeleteNumber()
        {
            _intLS = 0;
            _intSS = 1;
            _intSg = 2;
            _intTransSg = 3;

            List<CPolyline> pLSCPlLt = this.ObjCGeoLtLt[_intLS].AsExpectedClass<CPolyline, object>().ToList();
            List<CPolyline> pSSCPlLt = this.ObjCGeoLtLt[_intSS].AsExpectedClass<CPolyline, object>().ToList();
            List<CPolyline> pSgCPlLt = this.ObjCGeoLtLt[_intSg].AsExpectedClass<CPolyline, object>().ToList();
            List<CPolyline> TransSgCPlLt = this.ObjCGeoLtLt[_intTransSg].AsExpectedClass<CPolyline, object>().ToList();

            double dblLSToSSRatio = CalRatioofPtNum(pLSCPlLt, pSSCPlLt);

            var LScptSS = new SortedSet<CPoint>(new CCmpCptYX_VerySmall());
            foreach (var LSCPl in pLSCPlLt)
            {
                foreach (var cpt in LSCPl.CptLt)
                {
                    LScptSS.Add(cpt);
                }
            }

            //count the number of intersections
            int intSgInnerPtNum = 0;
            List<CPoint> SgEndPtLt = new List<CPoint>(pSgCPlLt.Count * 2);
            foreach (CPolyline cpl in pSgCPlLt)
            {
                intSgInnerPtNum += (cpl.CptLt.Count - 2);

                var FrCpt = cpl.CptLt.First();
                if (LScptSS.Contains(FrCpt) == false)
                {
                    SgEndPtLt.Add(FrCpt);
                }

                var ToCpt = cpl.CptLt.GetLastT();
                if (LScptSS.Contains(ToCpt) == false)
                {
                    SgEndPtLt.Add(ToCpt);
                }
            }
            C5.LinkedList<CCorrCpts> CorrCptsLt = CGeoFunc.LookingForNeighboursByGrids(SgEndPtLt, CConstants.dblVerySmallCoord);
            int intSgIntersection = CGeoFunc.GetNumofIntersections(CorrCptsLt);

            //do we need this?*******************************************************************************
            //int intAloneEnds = CGeoFunc.GetNumofAloneEnds(EndPtLt, CorrCptsLt);
            //int intRealPtNum = intInnerPtNum + intSgIntersection + intAloneEnds;


            int intTransSgInnerPtNum = 0;
            foreach (var cpl in TransSgCPlLt)
            {
                intTransSgInnerPtNum += (cpl.CptLt.Count - 2);
            }
            int intSgRealPtNum = intSgInnerPtNum + intSgIntersection;
            int intTransSgRealPtNum = intTransSgInnerPtNum + intSgIntersection;   //intSgRealPtNum doesn't count the points on the LSCpls. pSgCPlLt and TransSgCPlLt have the same number of intersections
            int intRemainPtNum = Convert.ToInt32(Convert.ToDouble(intSgRealPtNum) / dblLSToSSRatio);

            int intDeletePtNum = intTransSgRealPtNum - intRemainPtNum;   //notice that intRemainPtNum is according to intSgRealPtNum, but intDeletePtNum is according to intTransSgRealPtNum
            return intDeletePtNum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBSAtBdLt"></param>
        /// <param name="pSSAtBdLt"></param>
        /// <param name="ParameterThreshold"></param>
        /// <remarks>the number of Administrative Boundaries in pBSAtBdLt and that in pSSAtBdLt are the same. Besides, the topological structrues are the same</remarks>
        private double CalRatioofPtNum(List<CPolyline> pLSCPlLt, List<CPolyline> pSSCPlLt)
        {
            int intBSInnerPtNum = 0;
            int intSSInnerPtNum = 0;

            List<CPoint> BSEndPtLt = new List<CPoint>(pLSCPlLt.Count * 2);
            //List<CPoint> SSEndPtLt = new List<CPoint>();
            for (int i = 0; i < pLSCPlLt.Count; i++)
            {
                intBSInnerPtNum += (pLSCPlLt[i].CptLt.Count - 2);
                intSSInnerPtNum += (pSSCPlLt[i].CptLt.Count - 2);

                BSEndPtLt.Add(pLSCPlLt[i].CptLt[0]);
                BSEndPtLt.Add(pLSCPlLt[i].CptLt[pLSCPlLt[i].CptLt.Count - 1]);
            }

            C5.LinkedList<CCorrCpts> CorrCptsLt = CGeoFunc.LookingForNeighboursByGrids(BSEndPtLt, CConstants.dblVerySmallCoord);
            int intIntersection = CGeoFunc.GetNumofIntersections(CorrCptsLt);

            //do we need this?*******************************************************************************
            //int intAloneEnds = CGeoFunc.GetNumofAloneEnds(EndPtLt, CorrCptsLt);
            //int intRealPtNum = intInnerPtNum + intSgIntersection + intAloneEnds;

            //notice that there are the same intersections of the larger-scale polylines and the smaller-scale polylines
            int intBSRealPtNum = intBSInnerPtNum + intIntersection;
            int intSSRealPtNum = intSSInnerPtNum + intIntersection;

            double dblRatioofPtNum = Convert.ToDouble(intBSRealPtNum) / Convert.ToDouble(intSSRealPtNum);
            return dblRatioofPtNum;
        }


        #endregion

        #endregion

        #endregion

        public void CGABM()
        {
            List<CPolyline> pInterLSCPlLt = this.ObjCGeoLtLt[0].AsExpectedClass<CPolyline, object>().ToList();
            List<CPolyline> pInterSSCPlLt = this.ObjCGeoLtLt[1].AsExpectedClass<CPolyline, object>().ToList();
            List<CPolyline> pInterLSSgCPlLt = this.ObjCGeoLtLt[2].AsExpectedClass<CPolyline, object>().ToList();
            List<CPolyline> pInterSSSgCPlLt = this.ObjCGeoLtLt[3].AsExpectedClass<CPolyline, object>().ToList();
            _CorrCptsLtLt = CGeoFunc.GetCorrCptsLtLt(pInterLSCPlLt, pInterSSCPlLt);
            _SgCorrCptsLtLt = CGeoFunc.GetCorrCptsLtLt(pInterLSSgCPlLt, pInterSSSgCPlLt);

            CHelpFunc.SetMoveVectorForCorrCptsLtLt(_CorrCptsLtLt);
            CHelpFunc.SetMoveVectorForCorrCptsLtLt(_SgCorrCptsLtLt);

            //to save memory
            this.ObjCGeoLtLt[0] = null;
            this.ObjCGeoLtLt[1] = null;
            this.ObjCGeoLtLt[2] = null;
            this.ObjCGeoLtLt[3] = null;





            //CTranslation pTranslation=new CTranslation ();
            //double dblSum = pTranslation.CalTranslationCorr(_CorrCptsLtLt);
            //double dblSumSg = pTranslation.CalTranslationCorr(_SgCorrCptsLtLt);
        }







        #region Display
        public void DisplayAtBd(double dblProportion)
        {
            CParameterInitialize pParameterInitialize = _ParameterInitialize;
            CParameterResult pParameterResult = _ParameterResult;
            if (dblProportion < 0 || dblProportion > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return;
            }

            //double dblLargerScale = pParameterInitialize.dblLargerScale;
            //double dblSmallerScale = pParameterInitialize.dblSmallerScale;
            //double dblTargetScale = Math.Pow(dblLargerScale, 1 - dblProportion) * Math.Pow(dblSmallerScale, dblProportion);
            //double dblIgnorableDis = 0.0001 * dblTargetScale / 111319.490793;
            //double dblIgnorableDis = 0.0001 * dblTargetScale / 100000000000;
            //

            CSaveFeature pCsfFade = new CSaveFeature(esriGeometryType.esriGeometryPolyline, 
                "Fade" + pParameterInitialize.strSaveFolderName + "_" + dblProportion.ToString());
            CSaveFeature pCsfNormal = new CSaveFeature(esriGeometryType.esriGeometryPolyline, 
                "Normal" + pParameterInitialize.strSaveFolderName + "_" + dblProportion.ToString());


            Stopwatch pStopwatch = Stopwatch.StartNew();
            var normaldisplayIplEb = GenerateInterpolatedIPl(dblProportion, _CorrCptsLtLt).ToList<IPolyline5>();
            pStopwatch.Stop();
            //pParameterResult.DisplayCPlLt = normaldisplaycpllt;

            pCsfNormal.SaveIGeosToLayer(normaldisplayIplEb);
            normaldisplayIplEb = null;
            pCsfNormal = null;
            //foreach (var pCorrCptsLt in _CorrCptsLtLt)
            //{
            //    var ipl = GenerateInterpolatedIPl(pCorrCptsLt, dblProportion);
            //    pCsfNormal.SaveFeaturesToLayer (
            //}

            //the polylines should be faded out
            pStopwatch.Start();
            var fadeddisplayIplEb = GenerateInterpolatedIPl(dblProportion, _SgCorrCptsLtLt).ToList<IPolyline5>();

            pStopwatch.Stop();
            pCsfFade.SaveIGeosToLayer(fadeddisplayIplEb);
            pParameterInitialize.tsslTime.Text = pStopwatch.ElapsedMilliseconds.ToString();
            //pParameterResult.FadedDisplayCPlLt = fadeddisplaycpllt;

        }


        #endregion



    }
}
