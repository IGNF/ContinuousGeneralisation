using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

using MorphingClass.CUtility;
using MorphingClass.CEntity;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CGeometry
{
    public class CTriangulation : CDCEL
    {
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>usually, we don't care about ID of a point
        /// we care about indexID, which is the index of a point in a list. 
        /// we also set the TagValue for a triangulation, where indexID = TagValue - 1
        /// if we really want to construct a CDT of a mix of polygon (polyline) and points, 
        /// please add polygon (polyline) first, then add points.
        /// because if we add points first, the triangulatiosn has already be constructed, 
        /// and then if an tin edge intersect an edge of polygon (polyline), then both edges are simply split</remarks>
        private CPolygon _CPg;
        private ITinAdvanced2 _pTinAdvanced2;
        private List<ITinEdge> _pTinEdgeLt;

        //the dictionary of Cpts of this triangulation, 
        //cptSD only contains the meeting point of the start and end of the polygon once
        //we maintaine this SD so that for a point from single polyline, 
        //we can know whether this single point overlaps a point of a larger-scale polyline
        public SortedDictionary<CPoint, CPoint> CptSD { set; get; }  //using CCmpCptYX_VerySmall.sComparer
        public List<CEdge> ConstructingCEdgeLt { set; get; }
        public List<CEdge> NewCEdgeLt { set; get; } = new List<CEdge>();

        public CTriangulation(CPolygon cpg)
        {
            _CPg = cpg;
            var cptlt = new List<CPoint>(cpg.CptLt);
            cptlt.RemoveLastT();  // the last one has the same coordinates with the first one
            this.CptSD = CHelpFunc.TestCloseCpts(cptlt);
            this.CptLt = cptlt;

            //cpg.JudgeAndFormCEdgeLt();
            //this.ConstructingCEdgeLt.AddRange(cpg.CEdgeLt);
        }

        public CTriangulation(CPolygon cpg, List<CPoint> fcptlt)
        {
            _CPg = cpg;
            var cptlt = new List<CPoint>(fcptlt);
            this.CptSD = CHelpFunc.TestCloseCpts(cptlt);
            this.CptLt = cptlt;

            //cpg.JudgeAndFormCEdgeLt();
            //this.ConstructingCEdgeLt.AddRange(cpg.CEdgeLt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConstraintCpllt"></param>
        /// <param name="KnownCEdgeLt"></param>
        /// <param name="strIdentity"></param>
        /// <param name="blnSave"></param>
        /// <remarks>If the "data area" is not set deliberately, then ArcEngine will set a default "data area".
        /// The default data area excludes super edges and super nodes</remarks>
        public void Triangulate(List<CPolyline> ConstraintCpllt = null, List<CEdge> KnownCEdgeLt = null,
            string strIdentity = "", bool blnSave = false)
        {
            var cpg = _CPg;
            //cpg.pPolygon = null;


            cpg.JudgeAndSetPolygon();
            IPointCollection4 pCol = cpg.pPolygon as IPointCollection4;
            int intCount = pCol.PointCount;
            var pEnv = cpg.pPolygon.Envelope;

            ITinEdit TinEdit = new TinClass();
            TinEdit.InitNew(pEnv);
            ITinEdit2 TinEdit2 = TinEdit as ITinEdit2;
            TinEdit2.SetToConstrainedDelaunay();  //this must be done before adding any feature 
            var pTinAdvanced2 = TinEdit as ITinAdvanced2;
            ITinFeatureEdit pTinFeatureEdit = TinEdit as ITinFeatureEdit;
            cpg.JudgeAndSetZToZero();  //we need z coordinate to construct triangulation            
            pTinFeatureEdit.AddPolygonZ(cpg.pPolygon, esriTinEdgeType.esriTinHardEdge, 1, 1, 1, null);

            if (ConstraintCpllt != null)
            {
                foreach (var cpl in ConstraintCpllt)
                {
                    cpl.JudgeAndSetPolyline();
                    cpl.JudgeAndSetZToZero();
                    pTinFeatureEdit.AddPolylineZ(cpl.pPolyline, esriTinEdgeType.esriTinHardEdge, 1, 1, null);
                }
            }
            _pTinAdvanced2 = pTinAdvanced2;

            //we are not allowed to use AddShapeZ. 
            //it will report that there is no Z value in the shape, even we already set Z value to 0
            //The reason may be that we actually need polyhedron
            //TinEdit.AddShapeZ((IGeometry)cpg.pPolygon, esriTinSurfaceType.esriTinHardClip, 0); 
            //********************************************
            //this function set the "data area" of the TIN, 
            //we avoid to use this function because it may introduce new points
            //the new poins can be very close to the original points
            //TinEdit.AddShape((IGeometry)cpg.pPolygon, esriTinSurfaceType.esriTinHardClip, 0);
            //TinEdit.Refresh();


            if (pTinAdvanced2.DataNodeCount != this.CptLt.Count)
            {
                //Usually, KnownCEdgeLt saves all the constraints for the triangulation
                CSaveFeature.SaveCEdgeEb(KnownCEdgeLt, "KnownCEdgeLt"+ strIdentity);
                CSaveFeature.SaveCptEb(this.CptLt, "CptLtForKnownCEdgeLt"+ strIdentity);

                var NodeCptLt = GetCptLtFromTinAdvanced(pTinAdvanced2, true);
                CSaveFeature.SaveCptEb(NodeCptLt, "TinNode" + strIdentity);
                //var TinCEdgeLt= get
                var ExtraNodeCptLt = new List<CPoint>(pTinAdvanced2.DataNodeCount - this.CptLt.Count);
                foreach (var nodeCpt in NodeCptLt)
                {
                    if (this.CptSD.ContainsKey(nodeCpt)==false)
                    {
                        ExtraNodeCptLt.Add(nodeCpt);
                    }
                }
                CSaveFeature.SaveCptEb(ExtraNodeCptLt, "ExtraNodeCptLt" + strIdentity);

                var TinCEdgeLt= GetCEdgeLtFromTinAdvanced(pTinAdvanced2);
                CSaveFeature.SaveCEdgeEb(TinCEdgeLt, "TinCEdgeLt" + strIdentity);

                throw new ArgumentException("the numbers of points should be the same!");
            }
            this.CptLt.SetIndexID();

           
            var ITinEdgeLt = GenerateITinEdgeLt();
            var tincedgeSS = new SortedSet<CEdge>(new CCmpEdge_CptGID_BothDirections());
            var tincedgelt = new List<CEdge>();
            if (KnownCEdgeLt != null)
            {
                tincedgeSS = new SortedSet<CEdge>(KnownCEdgeLt, new CCmpEdge_CptGID_BothDirections());
                tincedgelt.AddRange(KnownCEdgeLt);
            }

            cpg.SetAxisAngleAndReverseLt();
            var cptSD = this.CptSD;
            foreach (var tinedge in ITinEdgeLt)
            {
                //there are always a pair of edges between a pair of triangles, but we only need one edge. 
                //So we reverse some edges than we can delete one edge of each pair
                var pFrNode = tinedge.FromNode;
                var pToNode = tinedge.ToNode;

                CPoint frcpt, tocpt;
                FindCorrCptInSDByTinNode(pFrNode, cptSD, out frcpt);
                FindCorrCptInSDByTinNode(pToNode, cptSD, out tocpt);

                CEdge newCEdge;
                if (frcpt.indexID < tocpt.indexID)
                {
                    newCEdge = new CEdge(frcpt, tocpt);
                }
                else if (frcpt.indexID > tocpt.indexID)
                {
                    newCEdge = new CEdge(tocpt, frcpt);
                }
                else
                {
                    throw new ArgumentException("should not happen!");
                }

                //test if the new edge is outside the boundary polygon (cpg)
                if (newCEdge.FrCpt.indexID < cpg.CEdgeLt.Count)
                {
                    //the new edge starts from a point which constitues the boundary polygon (cpg)
                    newCEdge.SetAxisAngle();
                    if (CGeoFunc.IsInbetween_Counterclockwise(cpg.AxisAngleLt[newCEdge.FrCpt.indexID], 
                        newCEdge.dblAxisAngle, cpg.ReverseAxisAngleLt[newCEdge.FrCpt.indexID]) ==false)
                    {
                        //the new edge is outside the boundary polygon (cpg)
                        continue;
                    }
                }

                //add the new edge if the edge is not added yet
                if (tincedgeSS.Add(newCEdge))
                {
                    this.NewCEdgeLt.Add(newCEdge);
                    tincedgelt.Add(newCEdge);
                }
            }

            if (blnSave == true)
            {
                CSaveFeature.SaveCEdgeEb(tincedgelt, "tincedgelt" + strIdentity, blnVisible: false);
            }

            this.CEdgeLt = tincedgelt;
        }

        private List<ITinNode> GetITinNodeLt(ITinAdvanced2 ptinadvanced2, bool blnDataArea = true)
        {
            var ITinNodeLt = new List<ITinNode>(ptinadvanced2.NodeCount);
            for (int i = 0; i < ptinadvanced2.NodeCount; i++)
            {
                var ptinnode = ptinadvanced2.GetNode(i + 1);

                if (blnDataArea == true)
                {
                    if (ptinnode.IsInsideDataArea)
                    {
                        ITinNodeLt.Add(ptinnode);
                    }
                }
                else
                {
                    ITinNodeLt.Add(ptinnode);
                }

            }
            //_pTinEdgeLt = ITinEdgeLt;
            return ITinNodeLt;

        }

        private List<CPoint> GetCptLtFromTinAdvanced(ITinAdvanced2 ptinadvanced2, bool blnDataArea = true)
        {
            var ITinNodeLt = GetITinNodeLt(ptinadvanced2, blnDataArea);
            var CptLt = new List<CPoint>(ITinNodeLt.Count);
            foreach (var ptinNode in ITinNodeLt)
            {
                CptLt.Add(new CPoint(ptinNode));
            }
            return CptLt;
        }

        private List<CEdge> GetCEdgeLtFromTinAdvanced(ITinAdvanced2 ptinadvanced2, bool blnDataArea = true)
        {
            var ITinEdgeLt = GenerateITinEdgeLt(blnDataArea);
            var CEdgeLt = new List<CEdge>(ITinEdgeLt.Count);
            foreach (var ptinEdge in ITinEdgeLt)
            {
                CEdgeLt.Add(new CEdge(ptinEdge));
            }
            return CEdgeLt;            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <returns>the boundary edges are stored in the front of the returned list.</returns>
        /// <remarks>this method may be only used for the construction of compatible triangulations</remarks>
        public List<CEdge> GenerateRealTinCEdgeLt(List<CEdge> KnownCEdgeLt = null)
        {
            this.CptLt.SetIndexID();
            var vertexSD = this.CptSD;


            var ITinEdgeLt = GenerateITinEdgeLt();
            var tincedgeSS = new SortedSet<CEdge>(new CCmpEdge_CptGID_BothDirections());
            var tincedgelt = new List<CEdge>();
            if (KnownCEdgeLt!=null)
            {
                tincedgeSS = new SortedSet<CEdge>(KnownCEdgeLt, new CCmpEdge_CptGID_BothDirections());
                tincedgelt.AddRange(KnownCEdgeLt);
            }

            foreach (var tinedge in ITinEdgeLt)
            {
                //there are always a pair of edges between a pair of triangles, but we only need one edge. 
                //So we reverse some edges than we can delete one edge of each pair
                var pFrNode = tinedge.FromNode;
                var pToNode = tinedge.ToNode;

                CPoint frcpt, tocpt;
                FindCorrCptInSDByTinNode(pFrNode, vertexSD, out frcpt);
                FindCorrCptInSDByTinNode(pToNode, vertexSD, out tocpt);

                CEdge newCEdge;
                if (frcpt.indexID < tocpt.indexID)
                {
                    newCEdge = new CEdge(frcpt, tocpt);
                }
                else if (frcpt.indexID > tocpt.indexID)
                {
                    newCEdge = new CEdge(tocpt, frcpt);
                }
                else
                {
                    throw new ArgumentException("should not happen!");
                }

                //notice that the duplicated cedge will not be added into tincedgeSS
                if (tincedgeSS.Add(newCEdge)==true)
                {
                    tincedgelt.Add(newCEdge);
                }
            }


            this.CEdgeLt = tincedgelt;
            return tincedgelt;
        }



        private bool FindCorrCptInSDByTinNode(ITinNode pTinNode, SortedDictionary<CPoint, CPoint> CptSD, out CPoint outcpt)
        {
            CPoint tincpt = new CPoint(pTinNode);
            bool isFound = CptSD.TryGetValue(tincpt, out outcpt);

            if (isFound == false)
            {
                Console.WriteLine("cannot find a point" + tincpt.X + "  " + tincpt.Y 
                    + "   " + " of a TinNode in: " + this.ToString() + ".cs   ");
                double dblOriginaldblVerySmall = CConstants.dblVerySmallCoord;

                do
                {
                    CConstants.dblVerySmallCoord = CConstants.dblVerySmallCoord * 10;
                    isFound = CptSD.TryGetValue(tincpt, out outcpt);
                } while (isFound == false);
                CConstants.dblVerySmallCoord = dblOriginaldblVerySmall;
            }

            return isFound;
        }



        public List<ITinEdge> GenerateITinEdgeLt(bool blnDataArea = true)
        {
            ITinAdvanced2 ptinadvanced2 = _pTinAdvanced2;
            var ITinEdgeLt = new List<ITinEdge>(ptinadvanced2.EdgeCount);

            for (int i = 0; i < ptinadvanced2.EdgeCount; i++)
            {
                ITinEdge ptinedge = ptinadvanced2.GetEdge(i + 1);
                if (blnDataArea == true)
                {
                    if (ptinedge.IsInsideDataArea)
                    {
                        ITinEdgeLt.Add(ptinedge);
                    }
                }
                else
                {
                    ITinEdgeLt.Add(ptinedge);
                }
            }
            _pTinEdgeLt = ITinEdgeLt;
            return ITinEdgeLt;
        }

        public void SaveDataAreaITinEdgeLt(string strName)
        {
            var ITinEdgeLt = _pTinEdgeLt;
            var CEdgeLt = new List<CEdge>(ITinEdgeLt.Count);
            foreach (var pTinEdge in ITinEdgeLt)
            {
                CEdgeLt.Add(new CEdge(pTinEdge));
            }

            CSaveFeature.SaveCEdgeEb(CEdgeLt, "TIN_"+strName);
        }

        /// <summary>
        /// the Quadrangle should be convex
        /// </summary>
        /// <param name="firstCEdge"></param>
        /// <param name="intIndex"></param>
        public static CEdge TriangulateQuadrangle_Delaunay(CEdge firstCEdge)
        {
            //the four edges constitutes a counter clockwise loop
            var secondCEdge = firstCEdge.cedgeNext;
            var thirdCEdge = secondCEdge.cedgeNext;
            var fourthCEdge = thirdCEdge.cedgeNext;

            firstCEdge.JudgeAndSetAxisAngle();
            secondCEdge.JudgeAndSetAxisAngle();
            thirdCEdge.JudgeAndSetAxisAngle();
            fourthCEdge.JudgeAndSetAxisAngle();

            //var firstAngle = Math.PI - CGeoFunc.CalAngle_Counterclockwise(fourthCEdge.dblAxisAngle, firstCEdge.dblAxisAngle);
            //var secondAngle = Math.PI - CGeoFunc.CalAngle_Counterclockwise(firstCEdge.dblAxisAngle, secondCEdge.dblAxisAngle);
            //var thirdAngle = Math.PI - CGeoFunc.CalAngle_Counterclockwise(secondCEdge.dblAxisAngle, thirdCEdge.dblAxisAngle);
            //var fourthAngle = Math.PI - CGeoFunc.CalAngle_Counterclockwise(thirdCEdge.dblAxisAngle, fourthCEdge.dblAxisAngle);

            //var minAngle = Math.Min(firstAngle, Math.Min(secondAngle, Math.Min(thirdAngle, fourthAngle)));

            CEdge newCEdge1 = new CEdge(firstCEdge.FrCpt, thirdCEdge.FrCpt);
            CEdge newCEdge2 = new CEdge(firstCEdge.ToCpt, thirdCEdge.ToCpt);
            newCEdge1.SetAxisAngle();
            newCEdge2.SetAxisAngle();

            //var dblAngle11= CGeoFunc.CalAngle_Counterclockwise(firstCEdge.dblAxisAngle, newCEdge1.dblAxisAngle);
            //var dblAngle12 = CGeoFunc.CalAngle_Counterclockwise(newCEdge1.dblAxisAngle, secondCEdge.dblAxisAngle);
            //var dblAngle13 = Math.PI - CGeoFunc.CalAngle_Counterclockwise(newCEdge1.dblAxisAngle, thirdCEdge.dblAxisAngle);
            //var dblAngle14 = Math.PI - CGeoFunc.CalAngle_Counterclockwise(fourthCEdge.dblAxisAngle, newCEdge1.dblAxisAngle);

            //var minAngle1 = Math.Min(dblAngle11, Math.Min(dblAngle12, Math.Min(dblAngle13, dblAngle14)));

            //var dblAngle21 = CGeoFunc.CalAngle_Counterclockwise(secondCEdge.dblAxisAngle, newCEdge2.dblAxisAngle);
            //var dblAngle22 = CGeoFunc.CalAngle_Counterclockwise(newCEdge2.dblAxisAngle, thirdCEdge.dblAxisAngle);
            //var dblAngle23 = Math.PI - CGeoFunc.CalAngle_Counterclockwise(newCEdge2.dblAxisAngle, fourthCEdge.dblAxisAngle);
            //var dblAngle24 = Math.PI - CGeoFunc.CalAngle_Counterclockwise(firstCEdge.dblAxisAngle, newCEdge2.dblAxisAngle);

            //var minAngle2 = Math.Min(dblAngle11, Math.Min(dblAngle12, Math.Min(dblAngle13, dblAngle14)));

            var dblMinAngle1 = GetDividedMinAngle(newCEdge1, firstCEdge, secondCEdge, thirdCEdge, fourthCEdge);
            var dblMinAngle2 = GetDividedMinAngle(newCEdge2, secondCEdge, thirdCEdge, fourthCEdge, firstCEdge);

            if (dblMinAngle1 <= dblMinAngle2)
            {
                return newCEdge1;
            }
            else
            {
                return newCEdge2;
            }
        }

        private static double GetDividedMinAngle(CEdge newCEdge, 
            CEdge firstCEdge, CEdge secondCEdge, CEdge thirdCEdge, CEdge fourthCEdge)
        {
            var dblAngle1 = CGeoFunc.CalAngle_Counterclockwise(firstCEdge.dblAxisAngle, newCEdge.dblAxisAngle);
            var dblAngle2 = CGeoFunc.CalAngle_Counterclockwise(newCEdge.dblAxisAngle, secondCEdge.dblAxisAngle);
            var dblAngle3 = Math.PI - CGeoFunc.CalAngle_Counterclockwise(newCEdge.dblAxisAngle, thirdCEdge.dblAxisAngle);
            var dblAngle4 = Math.PI - CGeoFunc.CalAngle_Counterclockwise(fourthCEdge.dblAxisAngle, newCEdge.dblAxisAngle);

            return Math.Min(dblAngle1, Math.Min(dblAngle2, Math.Min(dblAngle3, dblAngle4)));
        }



        //public List<CEdge > GenerateDataAreaCEdgeLt()
        //{
        //    var tinedgelt = GenerateDataAreaITinEdgeLt();
        //    List<CEdge> cedgelt = new List<CEdge>(tinedgelt.Count);
        //    foreach (var tinedge in tinedgelt)
        //    {

        //        cedgelt.Add(new CEdge(tinedge));

        //    }
        //    return cedgelt;
        //}

        //public List<ITinEdge> GenerateAllITinEdgeLt()
        //{
        //    ITinAdvanced2 ptinadvanced2 = _pTinAdvanced2;
        //    List<ITinEdge> ITinEdgeLt = new List<ITinEdge>(ptinadvanced2.DataEdgeCount);
        //    for (int i = 0; i < ptinadvanced2.EdgeCount; i++)
        //    {
        //        ITinEdge ptinedge = ptinadvanced2.GetEdge(i + 1);
        //        ITinEdgeLt.Add(ptinedge);
        //    }
        //    _pTinEdgeLt = ITinEdgeLt;
        //    return ITinEdgeLt;
        //}



        public ITinAdvanced2 pTinAdvanced2
        {

            get { return _pTinAdvanced2; }
            set { _pTinAdvanced2 = value; }
        }

    }
}
