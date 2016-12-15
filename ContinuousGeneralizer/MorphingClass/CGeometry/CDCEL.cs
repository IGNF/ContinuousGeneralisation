using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry.CGeometryBase;


namespace MorphingClass.CGeometry
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Doubly-Connected Edge List</remarks>
    public class CDCEL : CPolyBase<CPolyline>
    {
        private List<CPolyline> _CplLt;   //could be input
        private List<CEdge> _OriginalCEdgeLt;
        private List<CPoint> _OriginalCptLt;
        private List<CEdge> _HalfEdgeLt;  //output, we always store one edge and then imediately its twin edge; in the construction of compatible triangulations we always store the outer edges in the front of this list
        private List<CPolygon> _FaceCpgLt;      //output; superface is the first element in this list,i.e., _FaceCpgLt[0]. if a face has cedgeOuterComponent == null, then this face is super face
        private CEdgeGrid _pEdgeGrid;

        public CDCEL()
        {
        }

        public CDCEL(List<CPolyline> fcpllt)
        {
            _CplLt = fcpllt;
            _CEdgeLt = CGeometricMethods.GetCEdgeLtFromCpbLt<CPolyline, CPolyline>(fcpllt);
        }

        public CDCEL(List<CEdge> cedgelt)
        {
            _CEdgeLt = new List<CEdge>();
            _CEdgeLt.AddRange(cedgelt);
        }

        /// <summary>Update the two ends of all the polylines</summary>
        /// <remarks>at the beginning, the polylines have their own ends even in the same intersection. 
        /// we unified the vertices of the edges when we constructed doubly connected edge list, 
        /// now we also need to update the vertices of the polylines</remarks>
        public void UpdateCplltEnds()
        {
            List<CPolyline> fcpllt = _CplLt;
            foreach (CPolyline cpl in fcpllt)
            {
                int intCount = cpl.CEdgeLt.Count;
                cpl.CptLt[0] = cpl.CEdgeLt[0].FrCpt;
                cpl.CptLt[intCount] = cpl.CEdgeLt[intCount - 1].ToCpt;
            }
        }

        public void ConstructDCEL()
        {
            _CEdgeLt = _CEdgeLt.RemoveLaterDuplicate(new CCompareCEdgeCoordinates()).ToList();
            List<CEdge> cedgelt = _CEdgeLt;


            _HalfEdgeLt = ConstructHalfEdgeLt(cedgelt);  //already set indexID
            this.CptLt = ConstructVertexLt(_HalfEdgeLt);         //already set indexID
            //ShowEdgeRelationshipAroundAllCpt();
            ConstructFaceLt();       //already set indexID           

            _OriginalCEdgeLt = new List<CEdge>();
            _OriginalCptLt = new List<CPoint>();
            _OriginalCEdgeLt.AddRange(cedgelt);
            _OriginalCptLt.AddRange(this.CptLt);
        }


        #region Construct Half Edge List
        /// <summary>Construct Half Edge List</summary>
        /// <remarks>could be faster if we do it based on grid</remarks>
        private List<CEdge> ConstructHalfEdgeLt(List<CEdge> cedgelt)
        {
            List<CEdge> fHalfEdgeLt = new List<CEdge>(cedgelt.Count * 2);

            foreach (CEdge cedge in cedgelt)
            {
                cedge.CreateTwinCEdge();

                fHalfEdgeLt.Add(cedge);
                fHalfEdgeLt.Add(cedge.cedgeTwin);
            }

            SortedDictionary<CPoint, List<CEdge>> CoStartCEdgeSD = IdentifyCoStartCEdge(fHalfEdgeLt);
            ConstrcutRelationshipBetweenEdges(CoStartCEdgeSD);

            fHalfEdgeLt.SetIndexID();

            return fHalfEdgeLt;
        }

        private SortedDictionary<CPoint, List<CEdge>> IdentifyCoStartCEdge(List<CEdge> fHalfEdgeLt)
        {
            SortedDictionary<CPoint, List<CEdge>> CoStartCEdgeSD = new SortedDictionary<CPoint, List<CEdge>>(new CCompareCptYX());
            foreach (CEdge cedge in fHalfEdgeLt)
            {
                List<CEdge> cedgeLt;
                bool isExisted = CoStartCEdgeSD.TryGetValue(cedge.FrCpt, out cedgeLt);
                if (isExisted == true)
                {
                    cedgeLt.Add(cedge);
                }
                else
                {
                    cedgeLt = new List<CEdge>(2);  //we know that there is at least two edges starting at the same vertex
                    cedgeLt.Add(cedge);
                    CoStartCEdgeSD.Add(cedge.FrCpt, cedgeLt);
                }
            }
            return CoStartCEdgeSD;
        }

        private void ConstrcutRelationshipBetweenEdges(SortedDictionary<CPoint, List<CEdge>> CoStartCEdgeSD)
        {
            //vertexLt = new List<CPoint>(CoStartCEdgeSD.Count);
            foreach (var kvp in CoStartCEdgeSD)
            {
                var AxisAngleCEdgeLt = kvp.Value.OrderBy(cedge => cedge.dblAxisAngle).ToList();
                var AxisAngleCEdgeEt = AxisAngleCEdgeLt.GetEnumerator();
                AxisAngleCEdgeEt.MoveNext();
                CEdge SmallerAxisAngleCEdge = AxisAngleCEdgeEt.Current;
                SmallerAxisAngleCEdge.FrCpt = kvp.Key;  //SmallerAxisAngleCEdge may be from LS or SS, so we set its FrCpt as a Cpt from LS
                SmallerAxisAngleCEdge.cedgeTwin.ToCpt = kvp.Key;  //we also need to set cedgeTwin.ToCpt here because we would not have chance to set it any more
                while (AxisAngleCEdgeEt.MoveNext())
                {
                    InsertCEdgeBySmaller(SmallerAxisAngleCEdge, AxisAngleCEdgeEt.Current);
                    SmallerAxisAngleCEdge = AxisAngleCEdgeEt.Current;
                }

                var sharedcpt = kvp.Key;
                sharedcpt.AxisAngleCEdgeLt = AxisAngleCEdgeLt;   //we save this so that we may use it in combine triangulations
                sharedcpt.IncidentCEdge = AxisAngleCEdgeLt[0];
                sharedcpt.IncidentCEdge.isIncidentCEdgeForCpt = true;
                //vertexLt.Add(sharedcpt);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prevcedge"></param>
        /// <param name="insertedcedge"></param>
        /// <param name="nextcedge"></param>
        /// <remarks>newcedge and replacedcedge should have the same start</remarks>
        public static void ReplaceCEdge(CEdge replacedcedge, CEdge newcedge)
        {
            if (replacedcedge.isIncidentCEdgeForCpt == true)
            {
                //replacedcedge.isIncidentCEdgeForCpt = false;   //is this necessary?

                newcedge.isIncidentCEdgeForCpt = true;
                replacedcedge.FrCpt.IncidentCEdge = newcedge;
            }

            InsertCEdgeInbetween(replacedcedge.cedgeTwin.cedgeNext, newcedge, replacedcedge.cedgePrev.cedgeTwin);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="prevcedge"></param>
        /// <param name="insertedcedge"></param>
        /// <param name="nextcedge"></param>
        /// <remarks>prevcedge, insertedcedge, and nextcedge should have the same start</remarks>
        public static void InsertCEdgeBySmaller(CEdge SmallerAxisAngleCEdge, CEdge newcedge)
        {
            var LargerAxisAngleCEdge = SmallerAxisAngleCEdge.GetLargerAxisAngleCEdge();

            //maintain the IncidentCEdge information
            if (LargerAxisAngleCEdge.isIncidentCEdgeForCpt == true && newcedge.dblAxisAngle < LargerAxisAngleCEdge.dblAxisAngle)  //newcedge.dblAxisAngle may just larger than all the Costarted edges
            {
                LargerAxisAngleCEdge.isIncidentCEdgeForCpt = false;
                newcedge.isIncidentCEdgeForCpt = true;
                LargerAxisAngleCEdge.FrCpt.IncidentCEdge = newcedge;
            }

            InsertCEdgeInbetween(SmallerAxisAngleCEdge, newcedge, LargerAxisAngleCEdge);
        }

        public static void InsertCEdgeInbetween(CEdge SmallerAxisAngleCEdge, CEdge newcedge, CEdge LargerAxisAngleCEdge)
        {
            newcedge.cedgePrev = LargerAxisAngleCEdge.cedgeTwin;
            LargerAxisAngleCEdge.cedgeTwin.cedgeNext = newcedge;

            newcedge.cedgeTwin.cedgeNext = SmallerAxisAngleCEdge;
            SmallerAxisAngleCEdge.cedgePrev = newcedge.cedgeTwin;

            //link the edges
            newcedge.FrCpt = SmallerAxisAngleCEdge.FrCpt;                     //we will keep only one vertex at an intersection
            newcedge.cedgeTwin.ToCpt = SmallerAxisAngleCEdge.FrCpt;           //we will keep only one vertex at an intersection
        }

        #endregion

        #region Construct Vertex List
        /// <summary>Construct Vertex List</summary>
        /// <remarks>this mehtod will not change the order of vertices of regular polygon in the construction of compatible triangulations</remarks>
        private List<CPoint> ConstructVertexLt(List<CEdge> halfcedgelt)
        {
            List<CPoint> fcptlt = new List<CPoint>();
            foreach (CEdge halfcedge in halfcedgelt)
            {
                halfcedge.FrCpt.isAdded = false;
            }

            foreach (CEdge halfcedge in halfcedgelt)
            {
                if (halfcedge.FrCpt.isAdded == false)
                {
                    fcptlt.Add(halfcedge.FrCpt);
                    halfcedge.FrCpt.isAdded = true;
                }
            }
            fcptlt.SetIndexID();
            return fcptlt;
        }
        #endregion

        #region Construct Face List

        public List<CPolygon> ConstructFaceLt()
        {
            List<CEdge> cedgelt = _CEdgeLt;
            var rawfaceLt = InitializeFaceForEdgeLt(ref cedgelt);   //initialize a face for each edge loop
            FindTheLeftMostVertex(ref cedgelt);                      //find the left most vertex for each face, record it in "face.LeftMostCpt"

            DetermineOuterOrHole(rawfaceLt);                            //determine whether a face is a hole or an outer component by the angle of the left most vertex
            GetOuterOrInnerComponents(ref rawfaceLt);             //record this face into outer component or inner components

            List<CPolygon> FaceCpgLt = MergeFaces(cedgelt, ref rawfaceLt);
            FaceCpgLt.SetIndexID();
            _FaceCpgLt = FaceCpgLt;
            return FaceCpgLt;
        }

        #region InitializeFaceForEdgeLt
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cedgelt"></param>
        /// <param name="cgpLk">we generate some empty faces</param>
        private List<CPolygon> InitializeFaceForEdgeLt(ref List<CEdge> cedgelt)
        {
            var cgpLk = new List<CPolygon>();

            foreach (CEdge cedge in cedgelt)
            {
                cedge.cpgIncidentFace = null;
            }

            int intID = 0;
            foreach (CEdge cedge in cedgelt)
            {
                InitializeFaceForEdge(cedge, ref cgpLk, ref intID);
                InitializeFaceForEdge(cedge.cedgeTwin, ref cgpLk, ref intID);
            }
            return cgpLk;
        }

        private void InitializeFaceForEdge(CEdge cedge, ref List<CPolygon> cgpLk, ref int intID)
        {
            if (cedge.cpgIncidentFace == null)
            {
                CPolygon cpgIncidentFace = new CPolygon(intID);
                intID++;
                cgpLk.Add(cpgIncidentFace);
                do
                {
                    cedge.cpgIncidentFace = cpgIncidentFace;
                    cedge = cedge.cedgeNext;
                } while (cedge.cpgIncidentFace == null);
            }
        }
        #endregion

        #region FindTheLeftMostVertex
        private void FindTheLeftMostVertex(ref List<CEdge> cedgelt)
        {
            foreach (CEdge cedge in cedgelt)
            {
                cedge.isTraversed = false;
                cedge.cedgeTwin.isTraversed = false;
            }

            foreach (CEdge cedge in cedgelt)
            {
                FindTheLeftMostVertex(cedge);
                FindTheLeftMostVertex(cedge.cedgeTwin);
            }
        }

        private void FindTheLeftMostVertex(CEdge cedge)
        {
            if (cedge.isTraversed == false)
            {
                CPoint lowestleftmostcpt = cedge.FrCpt;
                cedge.cpgIncidentFace.cedgeStartAtLeftMost = cedge;
                cedge.cpgIncidentFace.LeftMostCpt = lowestleftmostcpt;
                do
                {
                    cedge.isTraversed = true;
                    cedge = cedge.cedgeNext;
                    int intResult = CCompareMethods.CompareXY(lowestleftmostcpt, cedge.FrCpt);
                    if (intResult == 1)
                    {
                        lowestleftmostcpt = cedge.FrCpt;
                        cedge.cpgIncidentFace.cedgeStartAtLeftMost = cedge;
                        cedge.cpgIncidentFace.LeftMostCpt = lowestleftmostcpt;
                    }
                } while (cedge.isTraversed == false);

                //cedge.cpgIncidentFace.LeftMostCpt = lowestleftmostcpt;
            }
        }

        #endregion

        private void DetermineOuterOrHole(List<CPolygon> rawfaceLt)
        {
            foreach (CPolygon cpg in rawfaceLt)
            {
                CEdge cedgeStartAtLeftMost = cpg.cedgeStartAtLeftMost;
                double dblAngle = CGeometricMethods.CalAngle2(cedgeStartAtLeftMost, cedgeStartAtLeftMost.cedgePrev.cedgeTwin);
                if (dblAngle < Math.PI)
                {
                    cpg.IsHole = false;
                }
                else if (dblAngle > Math.PI)
                {
                    cpg.IsHole = true;
                }
                else
                {
                    MessageBox.Show("what happened about the angle?  CECEL.cs!");
                }
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="rawfaceLt"></param>
        /// <remarks>For each edges' loop, there is a face. We have already decided whether a face is a hole or not.
        /// In this function, we assign an OuterComponent if the face is not a hole. We assign an inner component if the face is a hole
        /// Later, We will merge these faces</remarks>
        private void GetOuterOrInnerComponents(ref List<CPolygon> rawfaceLt)
        {
            foreach (CPolygon cpg in rawfaceLt)
            {
                cpg.cedgeStartAtLeftMost.isStartEdge = true;   //we label isStartEdge so that we know the start (and know when we should stop) when we traverse the edges of the face
                if (cpg.IsHole == false)
                {
                    cpg.cedgeOuterComponent = cpg.cedgeStartAtLeftMost;
                    //cpg.cedgeOuterComponent.isStartEdge = true;
                }
                else
                {
                    cpg.cedgeLkInnerComponents = new LinkedList<CEdge>();
                    cpg.cedgeLkInnerComponents.AddLast(cpg.cedgeStartAtLeftMost);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cedgelt"></param>
        /// <param name="rawfaceLt"></param>
        /// <returns></returns>
        private List<CPolygon> MergeFaces(List<CEdge> cedgelt, ref List<CPolygon> rawfaceLt)
        {
            List<CPoint> holeleftcptlt = new List<CPoint>(rawfaceLt.Count);
            foreach (CPolygon cpg in rawfaceLt)
            {
                if (cpg.IsHole == true)
                {
                    cpg.cedgeStartAtLeftMost.FrCpt.HoleCpg = cpg;
                    holeleftcptlt.Add(cpg.cedgeStartAtLeftMost.FrCpt);
                }
            }

            //CEdgeGrid pEdgeGrid = new CEdgeGrid(cedgelt);  //put edges in the cells of a grid
            //_pEdgeGrid = CGeometricMethods.DetectCloestLeftCorrectCEdge(cedgelt, ref holeleftcptlt);

            _pEdgeGrid = new CEdgeGrid(cedgelt);  //put edges in the cells of a grid
            DetectCloestLeftCorrectCEdge(holeleftcptlt);

            int intCount = 0;
            CPolygon SuperFace = new CPolygon();
            SuperFace.cedgeLkInnerComponents = new LinkedList<CEdge>();

            foreach (var rawface in rawfaceLt)
            {
                if (rawface.IsHole == true)
                {
                    //LeftFace can be a hole or the face inluding the current hole
                    //we merge the information of faces and store the information in the LeftFace
                    //we don't need to care about the cedgeOuterComponent, because it is always from the LeftFace which will be kept
                    CPolygon TargetFace = null;
                    if (rawface.LeftMostCpt.ClosestLeftCIntersection != null)  //there is an edge to the left
                    {
                        //LeftFace is the original incident face of rawface.LeftMostCpt.ClosestLeftCIntersection.CEdge2
                        CPolygon LeftFace = rawface.LeftMostCpt.ClosestLeftCIntersection.CEdge2IncidentFace;

                        if (LeftFace.IsHole == true)
                        {
                            //TargetCpg may or may not be LeftFace because the cedgeInnerComponent of a face may be updated to indicate another cpgIncidentFace
                            TargetFace = LeftFace.cedgeLkInnerComponents.First.Value.cpgIncidentFace;
                            TargetFace.cedgeLkInnerComponents.AppendRange(rawface.cedgeLkInnerComponents);
                        }
                        else  //LeftFace is the TargetCpg
                        {
                            if (LeftFace.cedgeLkInnerComponents == null)
                            {
                                LeftFace.cedgeLkInnerComponents = rawface.cedgeLkInnerComponents;
                            }
                            else
                            {
                                LeftFace.cedgeLkInnerComponents.AppendRange(rawface.cedgeLkInnerComponents);
                            }
                            TargetFace = LeftFace;
                        }
                    }
                    else
                    {
                        SuperFace.cedgeLkInnerComponents.AppendRange(rawface.cedgeLkInnerComponents);
                        TargetFace = SuperFace;
                    }

                    foreach (var cedgeInnerComponent in rawface.cedgeLkInnerComponents)
                    {
                        cedgeInnerComponent.cpgIncidentFace = TargetFace;
                    }
                    intCount++;
                }
            }


            foreach (CPolygon rawface in rawfaceLt)
            {
                rawface.isTraversed = false;
            }
            List<CPolygon> FaceCpgLt = new List<CPolygon>(rawfaceLt.Count - intCount + 1);
            FaceCpgLt.Add(SuperFace);
            SuperFace.isTraversed = true;
            foreach (CPolygon rawface in rawfaceLt)
            {
                if (rawface.cedgeOuterComponent != null)
                {
                    if (rawface.cedgeOuterComponent.cpgIncidentFace.isTraversed == false)
                    {
                        FaceCpgLt.Add(rawface.cedgeOuterComponent.cpgIncidentFace);
                        rawface.cedgeOuterComponent.cpgIncidentFace.isTraversed = true;
                    }
                }
            }

            UpdateEdgeIncidentFace(FaceCpgLt);

            return FaceCpgLt;
        }


        private void UpdateEdgeIncidentFace(IEnumerable<CPolygon> faceeb)
        {

            foreach (var face in faceeb)
            {
                if (face.cedgeLkInnerComponents != null)
                {
                    foreach (var cedgeInnerComponent in face.cedgeLkInnerComponents)
                    {
                        //cedgeInnerComponent has already indicated to the face, we need to update other edges
                        var currentcedge = cedgeInnerComponent.cedgeNext;
                        do
                        {
                            currentcedge.cpgIncidentFace = face;
                            currentcedge = currentcedge.cedgeNext;
                        } while (currentcedge.GID != cedgeInnerComponent.GID);
                    }
                }
            }
        }
        #endregion

        public void ExportEdgeAttributes(int intIndex)
        {
            if (intIndex == 1)
            {
                foreach (CEdge cedge in _HalfEdgeLt)  //we use 1 to store the DCEL information of the mix of Larger-scale and Single linear features (notice that Single linear features are actually also with larger-scale)
                {
                    cedge.cedgePrev1 = cedge.cedgePrev;
                    cedge.cedgeNext1 = cedge.cedgeNext;
                    cedge.cpgIncidentFace1 = cedge.cpgIncidentFace;
                    cedge.isStartEdge1 = cedge.isStartEdge;
                    cedge.FrCpt.IncidentCEdge1 = cedge.FrCpt.IncidentCEdge;

                    cedge.indexID1 = cedge.indexID;
                    cedge.FrCpt.indexID1 = cedge.FrCpt.indexID;
                    cedge.ToCpt.indexID1 = cedge.ToCpt.indexID;
                }
            }
            else if (intIndex == 2)  //we use 2 to store the DCEL information of Larger-scale linear features
            {
                foreach (CEdge cedge in _HalfEdgeLt)
                {
                    cedge.cedgePrev2 = cedge.cedgePrev;
                    cedge.cedgeNext2 = cedge.cedgeNext;
                    cedge.cpgIncidentFace2 = cedge.cpgIncidentFace;
                    cedge.isStartEdge2 = cedge.isStartEdge;
                    cedge.FrCpt.IncidentCEdge2 = cedge.FrCpt.IncidentCEdge;

                    cedge.indexID2 = cedge.indexID;
                    cedge.FrCpt.indexID2 = cedge.FrCpt.indexID;
                    cedge.ToCpt.indexID2 = cedge.ToCpt.indexID;
                }
            }
        }

        public void ClearEdgeAttributes()
        {
            foreach (CEdge cedge in _HalfEdgeLt)
            {
                cedge.cedgePrev = null;
                cedge.cedgeNext = null;
                cedge.cpgIncidentFace = null;
                cedge.isStartEdge = false;
                cedge.FrCpt.IncidentCEdge = null;

                cedge.indexID = -1;
                cedge.FrCpt.indexID = -1;
                cedge.ToCpt.indexID = -1;
            }
        }

        public void ImportEdgeAttributes(int intIndex)
        {
            if (intIndex == 1)
            {
                foreach (CEdge cedge in _HalfEdgeLt)
                {
                    cedge.cedgePrev = cedge.cedgePrev1;
                    cedge.cedgeNext = cedge.cedgeNext1;
                    cedge.cpgIncidentFace = cedge.cpgIncidentFace1;
                    cedge.isStartEdge = cedge.isStartEdge1;
                    cedge.FrCpt.IncidentCEdge = cedge.FrCpt.IncidentCEdge1;

                    cedge.indexID = cedge.indexID1;
                    cedge.FrCpt.indexID = cedge.FrCpt.indexID1;
                    cedge.ToCpt.indexID = cedge.ToCpt.indexID1;
                }
            }
            else if (intIndex == 2)
            {
                foreach (CEdge cedge in _HalfEdgeLt)
                {
                    cedge.cedgePrev = cedge.cedgePrev2;
                    cedge.cedgeNext = cedge.cedgeNext2;
                    cedge.cpgIncidentFace = cedge.cpgIncidentFace2;
                    cedge.isStartEdge = cedge.isStartEdge2;
                    cedge.FrCpt.IncidentCEdge = cedge.FrCpt.IncidentCEdge2;

                    cedge.indexID = cedge.indexID2;
                    cedge.FrCpt.indexID = cedge.FrCpt.indexID2;
                    cedge.ToCpt.indexID = cedge.ToCpt.indexID2;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <remarks >the first and last point of the list is the same one</remarks>
        public void TraverseEachFaceToGenerateCptLt()
        {
            foreach (var face in _FaceCpgLt)
            {
                if (face.cedgeOuterComponent != null)
                {
                    face.TraverseFaceToGenerateCptLt();
                }
            }
        }



        /// <summary>Detect a closest left edge for each point </summary>
        /// <remarks>create a line segment starting at a point and directing to left, see if this line segment intersect with any other cedges.
        ///                  even an end of another edge on this line segment, it will be considered as intersect</remarks>
        public void DetectCloestLeftCorrectCEdge(List<CPoint> cptlt)
        {
            foreach (CPoint cpt in cptlt)
            {
                DetectCloestLeftCorrectCEdge(cpt);
            }
        }


        public CEdge DetectCloestLeftCorrectCEdge(CPoint cpt)
        {
            CGeometricMethods.DetectCloestLeftCEdge(cpt, _pEdgeGrid);
            return CGeometricMethods.GetCorrectEdge(cpt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cedge"></param>
        /// <remarks> cpt is a vertex in the CDCEL</remarks>
        public static CEdge FindSmallerAxisAngleCEdgebyCEdge(CPoint cpt, CEdge cedge)
        {
            cedge.JudgeAndSetAxisAngle();

            var IncidentCEdge = cpt.IncidentCEdge;
            var CurrentCEdge = IncidentCEdge;

            //test the first edge (IncidentCEdge)
            if (CurrentCEdge.dblAxisAngle > cedge.dblAxisAngle)
            {
                return CurrentCEdge.GetSmallerAxisAngleCEdge();
            }
            else if (CurrentCEdge.dblAxisAngle < cedge.dblAxisAngle)
            {
                CurrentCEdge = CurrentCEdge.GetLargerAxisAngleCEdge();
            }
            else
            {
                MessageBox.Show("Overlap is not considered when traversing SgCpl in a Triangulation. In: CDCEL.cs");
            }

            //test other edges (IncidentCEdge)
            do
            {
                //int intCompare = CCompareMethods.Compare(CurrentCEdge.dblAxisAngle, cedge.dblAxisAngle);
                //if (intCompare == 1)
                if (CurrentCEdge.dblAxisAngle > cedge.dblAxisAngle)
                {
                    return CurrentCEdge.GetSmallerAxisAngleCEdge();
                }
                else if (CurrentCEdge.dblAxisAngle < cedge.dblAxisAngle)
                {
                    CurrentCEdge = CurrentCEdge.GetLargerAxisAngleCEdge();
                }
                else
                {
                    MessageBox.Show("Overlap is not considered when traversing SgCpl in a Triangulation. In: CDCEL.cs");
                }
            } while (CurrentCEdge.dblAxisAngle != IncidentCEdge.dblAxisAngle);


            return CurrentCEdge.GetSmallerAxisAngleCEdge();  //if cedge.dblAxisAngle is larger than the AxisAngles of all the edges, then the edge with largest AxisAngle is the one we are looking for
        }

        //public static 

        public List<CEdge> UpdateCEdgeLtByHalfCEdgeLt()
        {
            var halfcedgelt = _HalfEdgeLt;
            var cedgelt = new List<CEdge>(halfcedgelt.Count / 2);
            int intI = 0;
            while (intI < halfcedgelt.Count)
            {
                cedgelt.Add(halfcedgelt[intI]);
                intI += 2;
            }
            _CEdgeLt = cedgelt;
            return cedgelt;
        }

        public static bool IsVertexIntersection(CPoint cpt)
        {
            if (cpt.IncidentCEdge.indexID == cpt.IncidentCEdge.GetLargerAxisAngleCEdge().GetLargerAxisAngleCEdge().indexID)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public void ShowEdgeRelationshipAroundAllCpt()
        {
            //_vertex
            var cptlt = this.CptLt;

            Console.WriteLine("-------------------------------start Around All Cpt------------------------------");
            foreach (var cpt in this.CptLt)
            {
                ShowEdgeRelationshipAroundCpt(cpt);
            }
            Console.WriteLine("-------------------------------end Around All Cpt------------------------------");
        }

        public static void ShowEdgeRelationshipAroundCpt(CPoint cpt)
        {
            CEdge IncidentCEdge = cpt.IncidentCEdge;
            CEdge currentcedge = IncidentCEdge;
            Console.WriteLine("-------------------------------start Around Cpt------------------------------");
            do
            {
                CDCEL.ShowEdgeRelationship(currentcedge);

                currentcedge = currentcedge.GetLargerAxisAngleCEdge();
            } while (IncidentCEdge.dblAxisAngle != currentcedge.dblAxisAngle);
            Console.WriteLine("-------------------------------end Around Cpt------------------------------");
        }

        public static void ShowEdgeRelationship(CEdge cedge)
        {
            Console.WriteLine("-----------------start show edge--------------------");
            WriteInformationForCEdge(cedge, "this      edge");
            WriteInformationForCEdge(cedge.cedgePrev, "this prev edge");
            WriteInformationForCEdge(cedge.cedgeNext, "this next edge");
            WriteInformationForCEdge(cedge.cedgeTwin, "twin      edge");
            WriteInformationForCEdge(cedge.cedgeTwin.cedgePrev, "twin prev edge");
            WriteInformationForCEdge(cedge.cedgeTwin.cedgeNext, "twin next edge");
            Console.WriteLine("-----------------end show edge--------------------");
        }

        private static void WriteInformationForCEdge(CEdge cedge, string strName)
        {
            Console.WriteLine(strName + ":  " + cedge.FrCpt.indexID + "   " + cedge.ToCpt.indexID + "   AxisAngle:" + cedge.dblAxisAngle + "   FrX:" + cedge.FrCpt.X + "   FrY:" + cedge.FrCpt.Y + "   ToX:" + cedge.ToCpt.X + "   ToY:" + cedge.ToCpt.Y);
        }

        /// <summary></summary>
        public List<CPolyline> CplLt
        {
            get { return _CplLt; }
            set { _CplLt = value; }
        }

        public List<CEdge> OriginalCEdgeLt
        {
            get { return _OriginalCEdgeLt; }
            set { _OriginalCEdgeLt = value; }
        }

        public List<CPoint> OriginalCptLt
        {
            get { return _OriginalCptLt; }
            set { _OriginalCptLt = value; }
        }

        /// <summary></summary>
        /// <remarks>  //output, we always store one edge and then imediately its twin edge</remarks>
        public List<CEdge> HalfEdgeLt
        {
            get { return _HalfEdgeLt; }
            set { _HalfEdgeLt = value; }
        }

        /// <summary></summary>
        public List<CPolygon> FaceCpgLt
        {
            get { return _FaceCpgLt; }
            set { _FaceCpgLt = value; }
        }

        /// <summary></summary>
        public CEdgeGrid pEdgeGrid
        {
            get { return _pEdgeGrid; }
            set { _pEdgeGrid = value; }
        }



    }
}
