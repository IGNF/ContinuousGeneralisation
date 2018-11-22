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

//using C5;
using SCG = System.Collections.Generic;

using MorphingClass.CEntity;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
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

        //face  2: Guangdong 
        //face 27: Beijing 
        //face  7: Hunan
        //face 23: Gansu
        //face 14: Shanghai
        //face 26: Tianjin
        //face 28: between Beijing and Tianjin
        //face 10: Chongqin
        protected static int _intStart; //default: 0
        protected static int _intEndCount; //default: the number of interior faces 
        //comment the following if you want to process on all instances
        protected void UpdateStartEnd()
        {
            _intStart = 26;
            _intEndCount = _intStart + 1;
        }


        private bool _blnSave = false;
        //private bool _blnSave = true;

        




        public CCGABM()
        {

        }



        public CCGABM(CParameterInitialize ParameterInitialize, int intStartLayer = 0, int intLayerCount = 2,
             bool blnIGeoToCGeo = true)
        {
            Construct<CPolyline>(ParameterInitialize, intStartLayer, intLayerCount, blnIGeoToCGeo);
        }

        #region IdentifyAddFaceNumber

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>construct DCEL for InterLSCPlLt, and set the two face numbers for every Cpl. 
        /// since InterSSCPlLt are stored with the same order as InterLSCPlLt,
        /// we also set the same faces numbers for every Cpl from InterSSCPlLt for a SgCpl, 
        /// we get a point on it and test in which face this point is</remarks>
        public void IdentifyAddFaceNumber()
        {
            var pParameterInitialize = _ParameterInitialize;
            int intInterLS = 0;
            int intInterSS = 1;
            int intSg = 2;


            var pInterLSCPlLt = this.ObjCGeoLtLt[intInterLS].Select(cgeo => cgeo as CPolyline).ToList();
            var pSgCPlLt = this.ObjCGeoLtLt[intSg].Select(cgeo => cgeo as CPolyline).ToList();

            var pInterLSDCEL = new CDCEL(pInterLSCPlLt);
            pInterLSDCEL.ConstructDCEL();

            var InterLSobjlt1 = new List<object>(pInterLSCPlLt.Count);
            var InterLSobjlt2 = new List<object>(pInterLSCPlLt.Count);
            for (int i = 0; i < pInterLSCPlLt.Count; i++)
            {
                int indexID1 = pInterLSCPlLt[i].CEdgeLt[0].cpgIncidentFace.indexID;
                int indexID2 = pInterLSCPlLt[i].CEdgeLt[0].cedgeTwin.cpgIncidentFace.indexID;

                //we store the smaller index at "FaceNum_1", and store the larger index at "FaceNum_2"
                int intSmallerindexID;
                int intLargerindexID;
                CHelpFunc.CompareAndOrder(indexID1, indexID2, ID => ID, out intSmallerindexID, out intLargerindexID);

                InterLSobjlt1.Add(intSmallerindexID);
                InterLSobjlt2.Add(intLargerindexID);
            }

            //it should be true that a polyline of pInterSSCPlLt has the same indices of faces 
            //as the corresponding polyline of pInterLSCPlLt does
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[intInterLS].FeatureClass,
                esriFieldType.esriFieldTypeInteger, "FaceNum1", InterLSobjlt1);
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[intInterLS].FeatureClass,
                esriFieldType.esriFieldTypeInteger, "FaceNum2", InterLSobjlt2);
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[intInterSS].FeatureClass,
                esriFieldType.esriFieldTypeInteger, "FaceNum1", InterLSobjlt1);
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[intInterSS].FeatureClass,
                esriFieldType.esriFieldTypeInteger, "FaceNum2", InterLSobjlt2);


            var Sgobjlt = new List<object>(pSgCPlLt.Count);
            for (int i = 0; i < pSgCPlLt.Count; i++)
            {
                Sgobjlt.Add(DetectFaceForSg(pSgCPlLt[i], pInterLSDCEL).indexID);
            }
            CSaveFeature.AddFieldandAttribute(pParameterInitialize.pFLayerLt[intSg].FeatureClass,
                esriFieldType.esriFieldTypeInteger, "FaceNum", Sgobjlt);

        }
        #endregion

        #region Transform

        public void Transform()
        {


            int intInterLS = 0;
            int intInterSS = 1;
            int intSg = 2;


            var pParameterInitialize = CConstants.ParameterInitialize;
            var pstrFieldNameLtLt = this.strFieldNameLtLt;
            var pObjValueLtLtLt = this.ObjValueLtLtLt;

            var InterLSIplLt = this.ObjIGeoLtLt[intInterLS].Select(obj => obj as IPolyline5).ToList();
            var InterSSIplLt = this.ObjIGeoLtLt[intInterSS].Select(obj => obj as IPolyline5).ToList();
            var SgIplLt = this.ObjIGeoLtLt[intSg].Select(obj => obj as IPolyline5).ToList();

            var pStopwatch = new Stopwatch();
            pStopwatch.Start();

            var intInterLSFaceNumIndex1 = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[intInterLS], "FaceNum1");
            var intInterLSFaceNumIndex2 = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[intInterLS], "FaceNum2");
            var intSgFaceNumIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[intSg], "FaceNum");

            //count the faces
            var intInterLSFaceNumSS = new SortedSet<int>();
            foreach (var objlt in pObjValueLtLtLt[intInterLS])
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

            var pInterLSObjValueLtLt = pObjValueLtLtLt[intInterLS];  //the value table of LSLayer
            for (int i = 0; i < pInterLSObjValueLtLt.Count; i++)
            {
                //pLSObjValueLtLt[i][intInterLSFaceNumIndex1] is the index of a face
                InterLSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex1]].Add(InterLSIplLt[i]);
                InterLSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex2]].Add(InterLSIplLt[i]);

                //we use the same index, i.e.,pLSObjValueLtLt[i][intInterLSFaceNumIndex1], as we use for LS
                InterSSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex1]].Add(InterSSIplLt[i]);
                InterSSIplLtLt[(int)pInterLSObjValueLtLt[i][intInterLSFaceNumIndex2]].Add(InterSSIplLt[i]);
            }

            var SgIplLtLt = new List<List<IPolyline5>>(intInterLSFaceCount);
            var intSgIndexLtLt = new List<List<int>>(intInterLSFaceCount);
            SgIplLtLt.EveryElementNew();
            intSgIndexLtLt.EveryElementNew();

            var pSgObjValueLtLt = pObjValueLtLtLt[intSg];  //the value table of LSLayer

            for (int i = 0; i < pSgObjValueLtLt.Count; i++)
            {
                //pLSObjValueLtLt[i][intInterLSFaceNumIndex1] is the index of a face
                SgIplLtLt[(int)pSgObjValueLtLt[i][intSgFaceNumIndex]].Add(SgIplLt[i]);
                //we record the index of every polyline 
                //so that later we can store the transformed polylines with the same orders as SgIplLt 
                intSgIndexLtLt[(int)pSgObjValueLtLt[i][intSgFaceNumIndex]].Add(i);
            }

            //var dlgTransform = SetDlgTransform(pParameterInitialize.cboTransform.Text);
            var TransSgIGeoLt = new List<IGeometry>(SgIplLt.Count);
            TransSgIGeoLt.EveryElementValue(null);
            CHelpFunc.Displaytspb(0.5, intInterLSFaceCount);

            _intStart = 0;
            _intEndCount = intInterLSFaceCount;
            UpdateStartEnd();

            for (int i = _intStart; i < _intEndCount; i++)
            {
                Console.WriteLine("Face Num: " + i);
                if (SgIplLtLt[i].Count != 0) //face 0 is the outer face, the count is zero
                {
                    List<CPolyline> TransSgCplLt;
                    switch (pParameterInitialize.cboTransform.Text)
                    {
                        case "CT Max Common Chords":
                            TransSgCplLt = CTTransform(InterLSIplLtLt[i], InterSSIplLtLt[i], SgIplLtLt[i], true);
                            break;
                        case "Compatible Triangulations":
                            TransSgCplLt = CTTransform(InterLSIplLtLt[i], InterSSIplLtLt[i], SgIplLtLt[i], false);
                            break;
                        case "Rubber Sheeting":
                            TransSgCplLt = RSTransform(InterLSIplLtLt[i], InterSSIplLtLt[i], SgIplLtLt[i]);
                            break;
                        default: throw new ArgumentOutOfRangeException("a case doesn't exist!");
                    }

                    //var TransSgCplEb = dlgTransform(InterLSIplLtLt[i], InterSSIplLtLt[i], SgIplLtLt[i]);

                    int intCount = 0;
                    foreach (var TransSgCpl in TransSgCplLt)
                    {
                        TransSgIGeoLt[intSgIndexLtLt[i][intCount++]] = TransSgCpl.JudgeAndSetAEGeometry();
                    }
                }
                CHelpFunc.Displaytspb(i + 1, intInterLSFaceCount);
            }

            CHelpFunc.DisplayRunTime(pStopwatch.ElapsedMilliseconds);
            CSaveFeature.SaveIGeoEb(TransSgIGeoLt, esriGeometryType.esriGeometryPolyline, "TransSgCPlLt",
                 this.strFieldNameLtLt[intSg], this.esriFieldTypeLtLt[intSg], _ObjValueLtLtLt[intSg]);
        }
               
        #region CTTransform (compatible triangulation)
        /// <summary>
        /// Transform interior polylines in one province
        /// </summary>
        /// <param name="pParameterInitialize"></param>
        /// <param name="InterLSIplLt"></param>
        /// <param name="InterSSIplLt"></param>
        /// <param name="SgIplLt"></param>
        /// <returns></returns>
        /// <remarks>InterLSIplLt and InterSSIplLt can be clockwise of counterclockwise.
        /// we will construct DCEL, in which the directions will be counterclockwise.
        /// InterLSIplLt and InterSSIplLt should have the same direction and the corresponding start points</remarks>
        private List<CPolyline> CTTransform(List<IPolyline5> InterLSIplLt, List<IPolyline5> InterSSIplLt,
            List<IPolyline5> SgIplLt, bool blnMaxCommonChords = true)
        {
            CConstants.dblVerySmallCoord /= 10;  //this assignment should equal to _dblVerySmallDenominator = 10000000


            var InterLSCplLt = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(InterLSIplLt).ToList();
            var InterSSCplLt = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(InterSSIplLt).ToList();
            var SgCplEb = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(SgIplLt);

            //there are only two faces: the super face and a normal face. Face *.FaceCpgLt[1] is the normal face
            var pInterLSDCEL = new CDCEL(InterLSCplLt);
            pInterLSDCEL.ConstructDCEL();
            pInterLSDCEL.FaceCpgLt[1].SetOuterFaceCptlt(false, true, InterLSCplLt[0].CptLt[0]);

            //there are only two faces: the super face and a normal face. Face *.FaceCpgLt[1] is the normal face
            var pInterSSDCEL = new CDCEL(InterSSCplLt);
            pInterSSDCEL.ConstructDCEL();
            pInterSSDCEL.FaceCpgLt[1].SetOuterFaceCptlt(false, true, InterSSCplLt[0].CptLt[0]);

            //we maintaine this SD so that for a point from single polyline, 
            //we can know whether this single point overlaps a point of a larger-scale polyline
            var pInterLSCptSD = pInterLSDCEL.FaceCpgLt[1].CptLt.ToSD(cpt => cpt, new CCmpCptYX_VerySmall());


            var pCptbCtgl = new CCptbCtgl(pInterLSDCEL.FaceCpgLt[1], pInterSSDCEL.FaceCpgLt[1],
                blnMaxCommonChords, _blnSave);
            pCptbCtgl.ConstructCcptbCtgl();
            var TransSgCPlLt = new List<CPolyline>(SgIplLt.Count);
            foreach (var SgCpl in SgCplEb)
            {
                TransSgCPlLt.Add(GenerateCorrSgCpl(pCptbCtgl, SgCpl, pInterLSCptSD));
            }

            CConstants.dblVerySmallCoord *= 10;
            return TransSgCPlLt;
        }


        private CPolygon DetectFaceForSg(CPolyline SgCpl, CDCEL pInterLSDCEL)
        {
            var identitycpt = CGeoFunc.GetInbetweenCpt(SgCpl.CptLt[0], SgCpl.CptLt[1], 0.5);

            //comparing to the method which traverses along DCEL, this method only needs to detect the face once 
            return pInterLSDCEL.DetectCloestLeftCorrectCEdge(identitycpt).cpgIncidentFace;
        }

        //private void SgCplTraverse(CTriangulation pFrCtgl, List <CEdge > SgCEdgeLt, ref int intIndex)
        private CPolyline GenerateCorrSgCpl(CCptbCtgl pCptbCtgl, CPolyline SgCpl, SortedDictionary<CPoint, CPoint> pInterLSCptSD)
        {
            CTriangulation pFrCtgl = pCptbCtgl.FrCtgl;
            CTriangulation pToCtgl = pCptbCtgl.ToCtgl;
            var AffineCptLt = new List<CPoint>(SgCpl.CptLt.Count);
            AffineCptLt.Add(CalFirstAffineCpt(pCptbCtgl, SgCpl.CptLt[0], pInterLSCptSD));   //the first vertex

            SgCpl.JudgeAndFormCEdgeLt();
            //CareCEdge is an edge that current edge may cross with current triangle
            //CareCEdgeLt has three edges at most
            var CareCEdgeLt = FindCareCEdgeLtForFirstCEdge(pFrCtgl, SgCpl.CEdgeLt[0], pInterLSCptSD);
            if (SgCpl.CEdgeLt.Count == 1 && CCmpMethods.CmpCEdgeCoord(CareCEdgeLt[0], SgCpl.CEdgeLt[0]) == 0)
            {
                //this is a special case where a single polyline has only one edge (SgCEdge),
                //at the same time, this edge is used by the combined triangulation
                var newCEdge = pToCtgl.HalfEdgeLt[CareCEdgeLt[0].indexID];
                AffineCptLt.Add(newCEdge.ToCpt);
                return new CPolyline(SgCpl.ID, AffineCptLt);
            }


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
                        //this case is actually only for the fisrt vertex of a SgCpl, 
                        //because for other intersections which coincide a triangulation node, 
                        //we would not add the two neighbour edges in to CareCEdgeLt
                        case CEnumIntersectionType.FrFr:
                            CareCEdgeLt = GetCareCEdgeLtCptCoincident(carecedge.FrCpt, CurrentCEdge);
                            isFoundExit = true;
                            break;
                        //this case is actually only for the fisrt vertex of a SgCpl, 
                        //because for other intersections which coincide a triangulation node, 
                        //we would not add the two neighbour edges in to CareCEdgeLt
                        case CEnumIntersectionType.FrIn:
                            //this can happen, when the first SgCpt is on an triangle edge of FrCtgl
                            CareCEdgeLt = GetCareCEdgeLt(carecedge.cedgeTwin, false);
                            isFoundExit = true;
                            break;
                        //this case is actually only for the fisrt vertex of a SgCpl, 
                        //because for other intersections which coincide a triangulation node, 
                        //we would not add the two neighbour edges in to CareCEdgeLt
                        case CEnumIntersectionType.FrTo:
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
                        //maybe we can just ignore overlap, because if there is overlap, then there is also other cases
                        case CEnumIntersectionType.Overlap:
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
            var dblProp = pIntersection.CEdge2.CalInbetweenCptProportion(pIntersection.IntersectCpt);
            return pToCtgl.HalfEdgeLt[pIntersection.CEdge2.indexID].GetInbetweenCpt(dblProp);
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
            var atrianglecpt = new CPoint[3];
            for (int i = 0; i < 3; i++)
            {
                atrianglecpt[i] = CurrentCEdge.FrCpt;
                CurrentCEdge = CurrentCEdge.cedgeNext;
            }

            double dblLamda1, dblLamda2, dblLamda3;
            CGeoFunc.CalBarycentricCoordinates(SgCpt, atrianglecpt[0], atrianglecpt[1], atrianglecpt[2],
                out dblLamda1, out dblLamda2, out dblLamda3);
            var AffineCpt = CGeoFunc.CalCartesianCoordinates(pToCtgl.CptLt[atrianglecpt[0].indexID],
                pToCtgl.CptLt[atrianglecpt[1].indexID], pToCtgl.CptLt[atrianglecpt[2].indexID],
                dblLamda1, dblLamda2, dblLamda3, SgCpt.ID);
            return AffineCpt;
        }

        private List<CEdge> FindCareCEdgeLtForFirstCEdge(CTriangulation pFrCtgl,
            CEdge FirstSgCEdge, SortedDictionary<CPoint, CPoint> pInterLSCptSD)
        {
            var pFrCpt = FirstSgCEdge.FrCpt;
            var CareCEdgeLt = new List<CEdge>();

            CPoint outcpt;
            bool blnContainsKey = pInterLSCptSD.TryGetValue(pFrCpt, out outcpt);
            if (blnContainsKey == true)
            {
                CareCEdgeLt = GetCareCEdgeLtCptCoincident(outcpt, FirstSgCEdge, true);
            }
            else
            {
                //we have already computed FirstSgCEdge.FrCpt.ClosestLeftCIntersection.CEdge2 
                //when we computed the affine point for the first point
                CareCEdgeLt = GetCareCEdgeLt(pFrCpt.ClosestLeftCIntersection.CEdge2, true);
            }

            return CareCEdgeLt;
        }


        private List<CEdge> GetCareCEdgeLtCptCoincident(CPoint LSNodeCpt, CEdge SgCEdge, bool blnAllowOverlap = false)
        {
            var SmallerAxisAngleCEdge = CDCEL.FindSmallerAxisAngleCEdgebyCEdge(LSNodeCpt, SgCEdge, blnAllowOverlap);
            var CareCEdgeLt = new List<CEdge>(1);
            if (SgCEdge.dblAxisAngle != SmallerAxisAngleCEdge.dblAxisAngle)
            {
                CareCEdgeLt.Add(SmallerAxisAngleCEdge.cedgeNext);
            }
            else
            {
                if (blnAllowOverlap == true)
                {
                    //this should be a special case where a single polyline has only one edge (SgCEdge),
                    //at the same time, this edge is used by the combined triangulation
                    CareCEdgeLt.Add(SmallerAxisAngleCEdge);
                }
                else
                {
                    throw new ArgumentException("we did not consider overlap!");
                }
            }

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

        private List<CPolyline> RSTransform(
            List<IPolyline5> InterLSIplLt, List<IPolyline5> InterSSIplLt, List<IPolyline5> SgIplLt)
        {
            var pInterLSCplLt = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(InterLSIplLt).ToList();
            var pInterSSCplLt = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(InterSSIplLt).ToList();
            var SgCplEb = CHelpFunc.GenerateCGeoEbAccordingToGeoEb<CPolyline>(SgIplLt);

            var pCorrCptsLtLt = CGeoFunc.GetCorrCptsLtLt(pInterLSCplLt, pInterSSCplLt);
            CHelpFunc.SetMoveVectorForCorrCptsLtLt(pCorrCptsLtLt);

            var pInterLSCptSD = pInterLSCplLt.GetAllCptEb<CPolyline, CPolyline>().ToSD(cpt => cpt, new CCmpCptYX_VerySmall());
            var TransSgCPlLt = new List<CPolyline>(SgIplLt.Count);
            foreach (var SgCpl in SgCplEb)
            {
                TransSgCPlLt.Add(RSGenerateCorrSgCpl(SgCpl, pInterLSCptSD));
            }
            return TransSgCPlLt;
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
            cptlt.Add(RSComputeEndCpt(sgcptlt.Last(), pInterLSCptSD));  //the last point

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


        #endregion
               
        #region Simplify Transformed single polylines

        public int ComputeDeleteNumber()
        {
            int _intLS = 0;
            int _intSS = 1;
            int intSg = 2;
            int _intTransSg = 3;

            var pLSCPlLt = this.ObjCGeoLtLt[_intLS].Select(cgeo => cgeo as CPolyline).ToList();
            var pSSCPlLt = this.ObjCGeoLtLt[_intSS].Select(cgeo => cgeo as CPolyline).ToList();
            var pSgCPlLt = this.ObjCGeoLtLt[intSg].Select(cgeo => cgeo as CPolyline).ToList();
            var TransSgCPlLt = this.ObjCGeoLtLt[_intTransSg].Select(cgeo => cgeo as CPolyline).ToList();

            double dblLSToSSRatio = CalRatioEdgeNum(pLSCPlLt, pSSCPlLt);

            int intSgEdgeNum = 0;
            foreach (CPolyline cpl in pSgCPlLt)
            {
                intSgEdgeNum += (cpl.CptLt.Count - 1);
            }

            int intRemainEdgeNum = Convert.ToInt32(Convert.ToDouble(intSgEdgeNum) / dblLSToSSRatio);

            int intTransSgEdgeNum = 0;
            foreach (var cpl in TransSgCPlLt)
            {
                intTransSgEdgeNum += (cpl.CptLt.Count - 1);
            }

            int intDeleteNum = intTransSgEdgeNum - intRemainEdgeNum;
            return intDeleteNum;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pLSCPlLt"></param>
        /// <param name="pSSCPlLt"></param>
        /// <param name="ParameterThreshold"></param>
        /// <remarks>the number of Administrative Boundaries in pLSCPlLt and that in pSSCPlLt are the same. 
        /// Besides, the topological structrues are the same</remarks>
        private double CalRatioEdgeNum(List<CPolyline> pLSCPlLt, List<CPolyline> pSSCPlLt)
        {
            int intLSEdgeNum = 0;
            int intSSEdgeNum = 0;

            for (int i = 0; i < pLSCPlLt.Count; i++)
            {
                intLSEdgeNum += (pLSCPlLt[i].CptLt.Count - 1);
                intSSEdgeNum += (pSSCPlLt[i].CptLt.Count - 1);
            }

            return Convert.ToDouble(intLSEdgeNum) / Convert.ToDouble(intSSEdgeNum);
        }


        #endregion

        #endregion



        public void CGABM()
        {
            int intInterLS = 0;
            int intInterSS = 1;
            int intSg = 2;
            int intInterLSSg = 3;
            int intInterSSSg = 4;

            var pstrFieldNameLtLt = this.strFieldNameLtLt;
            var pObjValueLtLtLt = this.ObjValueLtLtLt;

            var intInterLSFaceNumIndex1 = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[intInterLS], "FaceNum1");
            var intInterLSFaceNumIndex2 = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[intInterLS], "FaceNum2");
            var intSgFaceNumIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[intSg], "FaceNum");

            //count the faces
            var intInterLSFaceNumSS = new SortedSet<int>();
            foreach (var objlt in pObjValueLtLtLt[intInterLS])
            {
                intInterLSFaceNumSS.Add((int)objlt[intInterLSFaceNumIndex1]);
                intInterLSFaceNumSS.Add((int)objlt[intInterLSFaceNumIndex2]);
            }
            var intInterLSFaceCount = intInterLSFaceNumSS.Count;

            _intStart = 0;
            _intEndCount = intInterLSFaceCount;
            UpdateStartEnd();

            var pInterLSCplLt = new List<CPolyline>();
            var pInterSSCplLt = new List<CPolyline>();
            var pObjValueLSLtLt = pObjValueLtLtLt[intInterLS]; //the values for the layer of the larger-scale polylines
            for (int i = 0; i < pObjValueLSLtLt.Count; i++)
            {
                int intFaceNum1 = (int)pObjValueLSLtLt[i][intInterLSFaceNumIndex1];
                int intFaceNum2 = (int)pObjValueLSLtLt[i][intInterLSFaceNumIndex2];

                if ((intFaceNum1 >= _intStart && intFaceNum1 < _intEndCount)||
                    (intFaceNum2 >= _intStart && intFaceNum2 < _intEndCount))
                {
                    pInterLSCplLt.Add(this.ObjCGeoLtLt[intInterLS][i] as CPolyline);
                    pInterSSCplLt.Add(this.ObjCGeoLtLt[intInterSS][i] as CPolyline);
                }
            }

            var pInterLSSgCplLt = new List<CPolyline>();
            var pInterSSSgCplLt = new List<CPolyline>();
            var pObjValueSgLtLt = pObjValueLtLtLt[intSg]; //the values for the layer of the larger-scale polylines
            for (int i = 0; i < pObjValueSgLtLt.Count; i++)
            {
                int intFaceNum = (int)pObjValueSgLtLt[i][intSgFaceNumIndex];

                if (intFaceNum >= _intStart && intFaceNum < _intEndCount)
                {
                    pInterLSSgCplLt.Add(this.ObjCGeoLtLt[intInterLSSg][i] as CPolyline);
                    pInterSSSgCplLt.Add(this.ObjCGeoLtLt[intInterSSSg][i] as CPolyline);
                }
            }
            
            _CorrCptsLtLt = CGeoFunc.GetCorrCptsLtLt(pInterLSCplLt, pInterSSCplLt);
            _SgCorrCptsLtLt = CGeoFunc.GetCorrCptsLtLt(pInterLSSgCplLt, pInterSSSgCplLt);

            CHelpFunc.SetMoveVectorForCorrCptsLtLt(_CorrCptsLtLt);
            CHelpFunc.SetMoveVectorForCorrCptsLtLt(_SgCorrCptsLtLt);

            ////to save memory
            //this.ObjCGeoLtLt[0] = null;
            //this.ObjCGeoLtLt[1] = null;
            //this.ObjCGeoLtLt[2] = null;
            //this.ObjCGeoLtLt[3] = null;



            

            //CTranslation pTranslation=new CTranslation ();
            //double dblSum = pTranslation.CalTranslationCorr(_CorrCptsLtLt);
            //double dblSumSg = pTranslation.CalTranslationCorr(_SgCorrCptsLtLt);
        }







        #region Display
        public void DisplayAtBd(double dblProp)
        {
            var pParameterInitialize = _ParameterInitialize;
            var pParameterResult = _ParameterResult;
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return;
            }

            //double dblLargerScale = pParameterInitialize.dblLargerScale;
            //double dblSmallerScale = pParameterInitialize.dblSmallerScale;
            //double dblTargetScale = Math.Pow(dblLargerScale, 1 - dblProp) * Math.Pow(dblSmallerScale, dblProp);
            //double dblIgnorableDis = 0.0001 * dblTargetScale / 111319.490793;
            //double dblIgnorableDis = 0.0001 * dblTargetScale / 100000000000;
            //


            var pStopwatch = Stopwatch.StartNew();
            var normaldisplayCplLt = GenerateInterpolatedCplLt(dblProp);
            var fadeddisplayCplLt = GenerateInterpolatedCplLt(dblProp, _SgCorrCptsLtLt);
            pStopwatch.Stop();

            int intRGB = Convert.ToInt32(dblProp * 255);
            var pStopwatchSave = Stopwatch.StartNew();
            CSaveFeature.SaveCplEb(fadeddisplayCplLt, dblProp.ToString() + "_Lower", intRed: intRGB, intGreen: intRGB, intBlue: intRGB, dblWidth: 0.5);
            CSaveFeature.SaveCplEb(normaldisplayCplLt, dblProp.ToString() + "_Higher", dblWidth: 1.2);
            pStopwatchSave.Stop();

            CHelpFunc.DisplayRunTime(pStopwatch.ElapsedMilliseconds, "Generate", pStopwatchSave.ElapsedMilliseconds, "ToShape");
        }


        #endregion



    }
}
