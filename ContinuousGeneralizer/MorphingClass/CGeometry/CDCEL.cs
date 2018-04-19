using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;

using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry.CGeometryBase;


namespace MorphingClass.CGeometry
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Doubly-Connected Edge List</remarks>
    public class CDCEL/* : CPolyBase<CPolyline>*/
    {
        //output, we always store one edge and then imediately its twin edge; 
        //in the construction of compatible triangulations we always store the outer edges in the front of this list
        private List<CEdge> _HalfEdgeLt;
        //output; superface is the first element in this list,i.e., _FaceCpgLt[0]. 
        //if a face has OuterCmptCEdge == null, then this face is super face  
        private List<CPolygon> _FaceCpgLt;      
        private CEdgeGrid _pEdgeGrid;
        public List<CEdge> CEdgeLt { get; set; }
        public List<CPoint> CptLt { get; set; }

        //private List<CPolyline> _CplLt;   //could be input
        //public List<CEdge> OriginalCEdgeLt { get; set; }
        //public List<CPoint> OriginalCptLt{ get; set; }
        //public List<CPolyline> CplLt { get; set; }

        public CDCEL()
        {
        }

        public CDCEL(List<CPolyline> fcpllt)
            :this(CGeoFunc.GetCEdgeLtFromCpbLt<CPolyline, CPolyline>(fcpllt))
        {           
        }

        public CDCEL(List<CEdge> cedgelt)
        {
            this.CEdgeLt = new List<CEdge>();
            this.CEdgeLt.AddRange(cedgelt);
        }

        public void ConstructDCEL()
        {
            var cedgelt = this.CEdgeLt;
            
            //report if there is a short edge
            //short edges may cause problems. Take a polygon as an example, an edge and its twin edge may refer to the same face
            CGeoFunc.CheckShortEdges(cedgelt);

            if (CGeoFunc.ExistDuplicate(cedgelt, new CCmpEqCEdgeCoord())==true)
            {
                throw new ArgumentException("There are duplicated edges! Please keep one copy for each edge!");
            }

            _HalfEdgeLt = ConstructHalfEdgeLt(cedgelt);  //already set indexID
            this.CptLt = ConstructVertexLt(_HalfEdgeLt);         //already set indexID

            //ShowEdgeRelationshipAroundAllCpt();
            ConstructFaceLt();       //already set indexID 
        }


        #region Construct Half Edge List
        /// <summary>Construct Half Edge List</summary>
        /// <remarks>could be faster if we do it based on grid</remarks>
        private List<CEdge> ConstructHalfEdgeLt(List<CEdge> cedgelt)
        {
            var fHalfEdgeLt = new List<CEdge>(cedgelt.Count * 2);

            foreach (CEdge cedge in cedgelt)
            {
                cedge.CreateTwinCEdge();

                fHalfEdgeLt.Add(cedge);
                fHalfEdgeLt.Add(cedge.cedgeTwin);
            }

            var CoStartHalfCEdgeDt = IdentifyCoStartCEdge(fHalfEdgeLt);
            OrderCEdgeLtAccordAxisAngle(CoStartHalfCEdgeDt);
            ConstrcutRelationshipBetweenEdges(CoStartHalfCEdgeDt);

            //foreach (var halfedge in fHalfEdgeLt)
            //{
            //    halfedge.PrintMySelf();
            //}
            


            fHalfEdgeLt.SetIndexID();

            return fHalfEdgeLt;
        }

        public static Dictionary<CPoint, List<CEdge>> IdentifyCoStartCEdge(List<CEdge> fHalfEdgeLt)
        {
            var CoStartHalfCEdgeDt = new Dictionary<CPoint, List<CEdge>>(new CCmpEqCptXY());
            foreach (CEdge cedge in fHalfEdgeLt)
            {
                List<CEdge> cedgeLt;
                bool isExisted = CoStartHalfCEdgeDt.TryGetValue(cedge.FrCpt, out cedgeLt);
                if (isExisted == true)
                {
                    cedgeLt.Add(cedge);
                }
                else
                {
                    cedgeLt = new List<CEdge>(2);  //we know that there is at least two edges starting at the same vertex
                    cedgeLt.Add(cedge);
                    CoStartHalfCEdgeDt.Add(cedge.FrCpt, cedgeLt);
                }
            }
            return CoStartHalfCEdgeDt;
        }

        public static void OrderCEdgeLtAccordAxisAngle(Dictionary<CPoint, List<CEdge>> CoStartHalfCEdgeDt)
        {
            foreach (var kvp in CoStartHalfCEdgeDt)
            {
                var AxisAngleCEdgeLt = kvp.Value.OrderBy(cedge => cedge.dblAxisAngle).ToList();
                var sharedcpt = kvp.Key;
                sharedcpt.AxisAngleCEdgeLt = AxisAngleCEdgeLt;   //we save this so that we may use it in combine triangulations
                sharedcpt.IncidentCEdge = AxisAngleCEdgeLt[0];
                sharedcpt.IncidentCEdge.isIncidentCEdgeForCpt = true;
            }
        }

        public static void ConstrcutRelationshipBetweenEdges(Dictionary<CPoint, List<CEdge>> CoStartHalfCEdgeDt)
        {
            //vertexLt = new List<CPoint>(CoStartCEdgeSD.Count);
            foreach (var kvp in CoStartHalfCEdgeDt)
            {
                var AxisAngleCEdgeEt = kvp.Key. AxisAngleCEdgeLt.GetEnumerator();
                AxisAngleCEdgeEt.MoveNext();
                CEdge SmallerAxisAngleCEdge = AxisAngleCEdgeEt.Current;
                //SmallerAxisAngleCEdge may be from LS or SS, so we set its FrCpt as a Cpt from LS
                SmallerAxisAngleCEdge.FrCpt = kvp.Key;
                //we also need to set cedgeTwin.ToCpt here because we would not have chance to set it any more 
                SmallerAxisAngleCEdge.cedgeTwin.ToCpt = kvp.Key;  
                while (AxisAngleCEdgeEt.MoveNext())
                {
                    InsertCEdgeBySmaller(SmallerAxisAngleCEdge, AxisAngleCEdgeEt.Current);
                    SmallerAxisAngleCEdge = AxisAngleCEdgeEt.Current;
                }
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
        /// <param name="cpt"></param>
        /// <param name="newcedge">newcedge should have a twin edge</param>
        public static void InsertCEdgeAtCpt(CPoint cpt, CEdge newcedge)
        {
            var SmallerAxisAngleCEdge = FindSmallerAxisAngleCEdgebyCEdge(cpt, newcedge, false);
            InsertCEdgeBySmaller(SmallerAxisAngleCEdge, newcedge);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="prevcedge"></param>
        /// <param name="insertedcedge"></param>
        /// <param name="newcedge">newcedge should have a twin edge</param>
        /// <remarks>prevcedge, insertedcedge, and nextcedge should have the same start</remarks>
        public static void InsertCEdgeBySmaller(CEdge SmallerAxisAngleCEdge, CEdge newcedge)
        {
            var LargerAxisAngleCEdge = SmallerAxisAngleCEdge.GetLargerAxisAngleCEdge();

            //maintain the IncidentCEdge information
            //newcedge.dblAxisAngle may just larger than all the Costarted edges,
            //because SmallerAxisAngleCEdge may be the edge with largest AxisAngle
            if (LargerAxisAngleCEdge.isIncidentCEdgeForCpt == true && newcedge.dblAxisAngle < LargerAxisAngleCEdge.dblAxisAngle)
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
            newcedge.FrCpt = SmallerAxisAngleCEdge.FrCpt;              //we will keep only one vertex at an intersection
            newcedge.cedgeTwin.ToCpt = SmallerAxisAngleCEdge.FrCpt;    //we will keep only one vertex at an intersection
        }

        #endregion

        #region Construct Vertex List
        /// <summary>Construct Vertex List</summary>
        /// <remarks>this mehtod will not change the order of vertices of regular polygon 
        /// in the construction of compatible triangulations</remarks>
        private List<CPoint> ConstructVertexLt(List<CEdge> halfcedgelt)
        {
            var fcptlt = new List<CPoint>();
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
            var halfcedgelt = _HalfEdgeLt;
            var rawfaceLt = InitializeFaceForEdgeLt(halfcedgelt).ToList();   //initialize a face for each edge loop
            FindTheLeftMostVertexForFace(ref halfcedgelt);                      //find the left most vertex for each face, record it in "face.LeftMostCpt"

            DetermineOuterOrHole(rawfaceLt);                            //determine whether a face is a hole or an outer component by the angle of the left most vertex
            GetOuterOrInnerComponents(ref rawfaceLt);             //record this face into outer component or inner components

            List<CPolygon> FaceCpgLt = MergeFaces(this.CEdgeLt, ref rawfaceLt);
            FaceCpgLt.SetIndexID();
            _FaceCpgLt = FaceCpgLt;
            return FaceCpgLt;
        }

        #region InitializeFaceForEdgeLt
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cedgelt"></param>
        /// <remarks>we generate some empty faces</remarks>
        private IEnumerable<CPolygon> InitializeFaceForEdgeLt(List<CEdge> halfcedgelt)
        {
            halfcedgelt.ForEach(cedge => cedge.isTraversed = false);
          
            int intID = 0;
            foreach (var cedge in halfcedgelt)
            {
                if (cedge.isTraversed == true) continue;

                var currentcedge = cedge;
                var cpgIncidentFace = new CPolygon(intID++);
                do
                {
                    currentcedge.cpgIncidentFace = cpgIncidentFace;
                    currentcedge.isTraversed = true;
                    currentcedge = currentcedge.cedgeNext;
                } while (currentcedge.isTraversed == false);

                yield return cpgIncidentFace;
            }
        }
        #endregion

        #region FindTheLeftMostVertex
        private void FindTheLeftMostVertexForFace(ref List<CEdge> halfcedgelt)
        {
            halfcedgelt.ForEach(cedge => cedge.isTraversed = false);

            foreach (var cedge in halfcedgelt)
            {
                if (cedge.isTraversed == true) continue;

                var cedgeStartAtLeftMost = GetCEdgeStartAtExtreme(cedge, CCmpCptXY_VerySmall.sComparer, -1);
                var cpgIncidentFace = cedge.cpgIncidentFace;
                cpgIncidentFace.LeftMostCpt = cedgeStartAtLeftMost.FrCpt;
                cpgIncidentFace.cedgeStartAtLeftMost = cedgeStartAtLeftMost;
            }
        }

        #endregion

        private void DetermineOuterOrHole(List<CPolygon> rawfaceLt)
        {
            foreach (CPolygon cpg in rawfaceLt)
            {
                var AxisAngleCEdgeLt = cpg.LeftMostCpt.AxisAngleCEdgeLt;

                //find UpperCorrectAxisAngleCEdge and LowerCorrectAxisAngleCEdge
                //UpperCorrectAxisAngleCEdge has angle most close to 90 degrees
                //LowerCorrectAxisAngleCEdge has angle most close to 270 degrees
                var UpperCorrectAxisAngleCEdge = AxisAngleCEdgeLt[0];
                var dblUpperAngleToHalfPI = MakeAngleCorrect(
                    CGeoFunc.CalAngle_Counterclockwise(AxisAngleCEdgeLt[0].dblAxisAngle, CConstants.dblHalfPI));



                var LowerCorrectAxisAngleCEdge = AxisAngleCEdgeLt[0];
                var dblLowerAngleToHalfPI = dblUpperAngleToHalfPI;
                for (int i = 1; i < AxisAngleCEdgeLt.Count; i++)
                {
                    double dblAngleToHalfPI = MakeAngleCorrect(
                    CGeoFunc.CalAngle_Counterclockwise(AxisAngleCEdgeLt[i].dblAxisAngle, CConstants.dblHalfPI));


                    if (dblAngleToHalfPI <= dblUpperAngleToHalfPI)
                    {
                        dblUpperAngleToHalfPI = dblAngleToHalfPI;
                        UpperCorrectAxisAngleCEdge = AxisAngleCEdgeLt[i];
                    }

                    if (dblAngleToHalfPI > dblLowerAngleToHalfPI)
                    {
                        dblLowerAngleToHalfPI = dblAngleToHalfPI;
                        LowerCorrectAxisAngleCEdge = AxisAngleCEdgeLt[i];
                    }
                }


                cpg.IsHole = false;  //a hole means that the direction is clockwise
                var TestCEdgeStartAtExtreme = cpg.cedgeStartAtLeftMost;
                do
                {
                    if (TestCEdgeStartAtExtreme.cpgIncidentFace.GID == cpg.GID)  //TestCEdgeStartAtExtreme belongs to face cpg
                    {
                        //if the component is a hole, the following must hold
                        if (TestCEdgeStartAtExtreme.GID == UpperCorrectAxisAngleCEdge.GID
                            && TestCEdgeStartAtExtreme.cedgePrev.GID == LowerCorrectAxisAngleCEdge.cedgeTwin.GID)
                        {
                            cpg.IsHole = true;  //inner boundary
                            break;
                        }
                    }
                    TestCEdgeStartAtExtreme = TestCEdgeStartAtExtreme.GetLargerAxisAngleCEdge();
                } while (TestCEdgeStartAtExtreme.GID != cpg.cedgeStartAtLeftMost.GID);
            }
        }

        private static double MakeAngleCorrect(double dblAngle)
        {
            if ( CCmpMethods.CmpDbl_ConstVerySmall(dblAngle,CConstants.dblTwoPI)==0)
            {
                dblAngle = 0;
            }
            return dblAngle;
        }


        /// <summary>
        /// there are more than one edge in the face starting from the vertex
        /// we should try rightmost, uppermost, or lowest vertex
        /// </summary>
        /// <param name="cedge"></param>
        /// <returns></returns>
        private CEdge GetValidCEdgeStartAtExtreme(CEdge cedge)
        {
            var cedgeStartAtExtreme = GetCEdgeStartAtExtreme(cedge, CCmpCptXY_VerySmall.sComparer, 1);  //rightmost

            if (IsOnlyOneEdgeStartAtFrCpt(cedgeStartAtExtreme) == false)
            {
                cedgeStartAtExtreme = GetCEdgeStartAtExtreme(cedge, CCmpCptYX_VerySmall.sComparer, 1);  //uppermost
                if (IsOnlyOneEdgeStartAtFrCpt(cedgeStartAtExtreme) == false)
                {
                    cedgeStartAtExtreme = GetCEdgeStartAtExtreme(cedge, CCmpCptYX_VerySmall.sComparer, -1);  //lowest
                    if (IsOnlyOneEdgeStartAtFrCpt(cedgeStartAtExtreme) == false)
                    {
                        throw new ArgumentOutOfRangeException("The four extreme points are not available to test holes or outer!");
                    }
                }
            }

            return cedgeStartAtExtreme;
        }

        /// <summary>
        /// there are more than one edge in the face starting from the vertex
        /// </summary>
        /// <param name="cedge"></param>
        /// <returns></returns>
        private bool IsOnlyOneEdgeStartAtFrCpt(CEdge cedge)
        {
            return (cedge.GID == cedge.cedgeTwin.cedgeNext.cedgeTwin.cedgeNext.GID);
        }

        private CEdge GetCEdgeStartAtExtreme(CEdge cedge, IComparer<CPoint> pCmpCpt, int intValue)
        {
            var CEdgeStartAtExtreme = cedge;
            var currentcedge = cedge.cedgeNext;
            while (currentcedge.GID != cedge.GID)
            {
                if (pCmpCpt.Compare(currentcedge.FrCpt, CEdgeStartAtExtreme.FrCpt) == intValue)
                {
                    CEdgeStartAtExtreme = currentcedge;
                }
                currentcedge.isTraversed = true;  //this sentence is only useful for funcion FindTheLeftMostVertexForFace
                currentcedge = currentcedge.cedgeNext;
            }

            return CEdgeStartAtExtreme;
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
                    cpg.OuterCmptCEdge = cpg.cedgeStartAtLeftMost;
                    //cpg.OuterCmptCEdge.isStartEdge = true;
                }
                else
                {
                    cpg.InnerCmptCEdgeLt = new List<CEdge>();
                    cpg.InnerCmptCEdgeLt.Add(cpg.cedgeStartAtLeftMost);
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
                    cpg.LeftMostCpt.HoleCpg = cpg;
                    holeleftcptlt.Add(cpg.LeftMostCpt);
                }
            }

            //CEdgeGrid pEdgeGrid = new CEdgeGrid(cedgelt);  //put edges in the cells of a grid
            //_pEdgeGrid = CGeoFunc.DetectCloestLeftCorrectCEdge(cedgelt, ref holeleftcptlt);

            _pEdgeGrid = new CEdgeGrid(cedgelt);  //put edges in the cells of a grid
            DetectCloestLeftCorrectCEdge(holeleftcptlt);

            int intCount = 0;
            CPolygon SuperFace = new CPolygon();
            SuperFace.InnerCmptCEdgeLt = new List<CEdge>();

            foreach (var rawface in rawfaceLt)
            {
                if (rawface.IsHole == true)
                {
                    //LeftFace can be a hole or the face inluding the current hole
                    //we merge the information of faces and store the information in the LeftFace
                    //we don't need to care about the OuterCmptCEdge, because it is always from the LeftFace which will be kept
                    CPolygon TargetFace = null;
                    if (rawface.LeftMostCpt.ClosestLeftCIntersection != null)  //there is an edge to the left
                    {
                        //LeftFace is the original incident face of rawface.LeftMostCpt.ClosestLeftCIntersection.CEdge2
                        CPolygon LeftFace = rawface.LeftMostCpt.ClosestLeftCIntersection.CEdge2IncidentFace;

                        if (LeftFace.IsHole == true)
                        {
                            //TargetCpg may or may not be LeftFace because the cedgeInnerComponent of a face may be updated to indicate another cpgIncidentFace
                            TargetFace = LeftFace.InnerCmptCEdgeLt[0].cpgIncidentFace;
                            TargetFace.InnerCmptCEdgeLt.AddRange(rawface.InnerCmptCEdgeLt);
                        }
                        else  //LeftFace is the TargetCpg
                        {
                            if (LeftFace.InnerCmptCEdgeLt == null)
                            {
                                LeftFace.InnerCmptCEdgeLt = rawface.InnerCmptCEdgeLt;
                            }
                            else
                            {
                                LeftFace.InnerCmptCEdgeLt.AddRange(rawface.InnerCmptCEdgeLt);
                            }
                            TargetFace = LeftFace;
                        }
                    }
                    else
                    {
                        SuperFace.InnerCmptCEdgeLt.AddRange(rawface.InnerCmptCEdgeLt);
                        TargetFace = SuperFace;
                    }

                    foreach (var cedgeInnerComponent in rawface.InnerCmptCEdgeLt)
                    {
                        cedgeInnerComponent.cpgIncidentFace = TargetFace;
                    }
                    intCount++;
                }
            }

            //add each face into FaceCpgLt
            foreach (CPolygon rawface in rawfaceLt)
            {
                rawface.isTraversed = false;
            }
            List<CPolygon> FaceCpgLt = new List<CPolygon>(rawfaceLt.Count - intCount + 1);
            FaceCpgLt.Add(SuperFace);
            SuperFace.isTraversed = true;
            foreach (CPolygon rawface in rawfaceLt)
            {
                if (rawface.OuterCmptCEdge != null)
                {
                    if (rawface.OuterCmptCEdge.cpgIncidentFace.isTraversed == false)
                    {
                        FaceCpgLt.Add(rawface.OuterCmptCEdge.cpgIncidentFace);
                        rawface.OuterCmptCEdge.cpgIncidentFace.isTraversed = true;
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
                if (face.InnerCmptCEdgeLt != null)
                {
                    foreach (var cedgeInnerComponent in face.InnerCmptCEdgeLt)
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
                //we use 1 to store the DCEL information of the mix of Larger-scale and Single linear features 
                //(notice that Single linear features are actually also with larger-scale)
                foreach (CEdge cedge in _HalfEdgeLt)  
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <remarks >the first and last point of the list is the same one</remarks>
        //public void TraverseEachFaceToGenerateCptLt()
        //{
        //    foreach (var face in _FaceCpgLt)
        //    {
        //        if (face.OuterCmptCEdge != null)
        //        {
        //            face.TraverseFaceToGenerateCptLt();
        //        }
        //    }
        //}



        /// <summary>Detect a closest left edge for each point </summary>
        /// <remarks>create a line segment starting at a point and directing to left, 
        /// see if this line segment intersect with any other cedges.
        /// even an end of another edge on this line segment, it will be considered as intersect</remarks>
        public void DetectCloestLeftCorrectCEdge(List<CPoint> cptlt)
        {
            foreach (CPoint cpt in cptlt)
            {
                DetectCloestLeftCorrectCEdge(cpt);
            }
        }


        public CEdge DetectCloestLeftCorrectCEdge(CPoint cpt)
        {
            CGeoFunc.DetectCloestLeftCEdge(cpt, _pEdgeGrid);
            return CGeoFunc.GetCorrectEdge(cpt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cedge"></param>
        /// <remarks> cpt is a vertex in the CDCEL</remarks>
        public static CEdge FindSmallerAxisAngleCEdgebyCEdge(CPoint cpt, CEdge cedge,  bool blnAllowOverlap=false)
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
                if (blnAllowOverlap == true)
                {
                    return CurrentCEdge;
                }
                else
                {
                    throw new ArgumentException("edges overlap each other.");
                }
            }

            //test other edges (IncidentCEdge)
            do
            {
                //int intCompare = CCmpMethods.Cmp(CurrentCEdge.dblAxisAngle, cedge.dblAxisAngle);
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
                    if (blnAllowOverlap == true)
                    {
                        return CurrentCEdge;
                    }
                    else
                    {
                        throw new ArgumentException("edges overlap each other.");
                    }
                }
            } while (CurrentCEdge.GID != IncidentCEdge.GID);

            //if cedge.dblAxisAngle is larger than the AxisAngles of all the edges, 
            //then the edge with largest AxisAngle is the one we are looking for
            return CurrentCEdge.GetSmallerAxisAngleCEdge();  
        }

        //public static 

        public void UpdateCEdgeLtByHalfCEdgeLt()
        {
            var halfcedgelt = _HalfEdgeLt;
            var cedgelt = new List<CEdge>(halfcedgelt.Count / 2);
            int intI = 0;
            while (intI < halfcedgelt.Count)
            {
                cedgelt.Add(halfcedgelt[intI]);
                intI += 2;
            }
            this.CEdgeLt = cedgelt;
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
            var cptlt = this.CptLt;
            Console.WriteLine("-------------------------------start Around All Cpt------------------------------");
            foreach (var cpt in cptlt)
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

        public void ShowEdgeRelationshipAllCEdges()
        {
            var halfcedgelt = this.HalfEdgeLt;
            for (int i = 0; i < halfcedgelt.Count; i++)
            {
                ShowEdgeRelationship(halfcedgelt[i]);
                i++;
            }
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
            Console.WriteLine(strName +"(indexID "+ cedge.indexID+ ")" + ":  " + 
                cedge.FrCpt.indexID + "   " + cedge.ToCpt.indexID 
                + "   AxisAngle:" + cedge.dblAxisAngle 
                + "   FrX:" + cedge.FrCpt.X + "   FrY:" + cedge.FrCpt.Y 
                + "   ToX:" + cedge.ToCpt.X + "   ToY:" + cedge.ToCpt.Y);
        }


        ///// <summary>
        ///// we have to run GenerateDataAreaCEdgeLt first
        ///// </summary>
        ///// <param name="pParameterInitialize"></param>
        ///// <param name="str"></param>
        //public void SaveCEdgeLt(string str)
        //{
        //    CSaveFeature.SaveCEdgeEb(this.CEdgeLt, str);
        //}


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
