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
        /// we care about indexID, which is the index of a point in a list. we also set the TagValue for a triangulation, where indexID = TagValue - 1
        /// if we really want to construct a CDT of a mix of polygon (polyline) and points, please add polygon (polyline) first, then add points.
        /// because if we add points first, the triangulatiosn has already be constructed, and then if an tin edge intersect an edge of polygon (polyline), then both edges are simply split</remarks>
        private CPolygon _CPg;
        private ITinEdit _pTinEdit;
        private ITinAdvanced2 _pTinAdvanced2;
        private List<ITinEdge> _pTinEdgeLt;
        private int _intN;
        public SortedDictionary<CPoint, CPoint> cptSD { set; get; }   //the dictionary of Cpts of this triangulation, cptSD only contains the meeting point of the start and end of the polygon once
        //public SortedDictionary<CPoint, CPoint> CptSD { get; set; }  //we maintaine this SD so that for a point from single polyline, we can know whether this single point overlaps a point of a larger-scale polyline

        public CTriangulation(CPolygon cpg)
        {
            _CPg = cpg;
            var cptlt = new List<CPoint>();
            cptlt.AddRange(cpg.CptLt);  //_CptLt might change in future by, e.g., constructing compatible triangulations
            cptlt.RemoveLastT();  // the last one has the same coordinates with the first one
            this.cptSD = TestCloseCpts(cptlt);


            _intN = this.cptSD.Count;
            this.CptLt = cptlt;
        }

        private SortedDictionary<CPoint, CPoint> TestCloseCpts(List<CPoint> cptlt)
        {
           var cptSD = cptlt.ToSD(cpt => cpt, CCmpCptYX_VerySmall.sComparer);

           if (cptSD.Count == cptlt.Count)
           {
               return cptSD;
           }
           else
           {
               throw new ArgumentException("some points are too close to each other!");
               //return false;
           }
        }


        public ITinEdit Triangulate()
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
            int intCount1 = pTinAdvanced2.DataNodeCount;
            //TinEdit.AddShapeZ((IGeometry)cpg.pPolygon, esriTinSurfaceType.esriTinHardClip, 0);  //we are not allowed to use AddShapeZ because it will report that there is no Z value in the shape, even we already set Z value to 0
            //TinEdit.AddShape((IGeometry)cpg.pPolygon, esriTinSurfaceType.esriTinHardClip, 0);  //this must be done after adding any feature

            TinEdit.AddShape((IGeometry)cpg.pPolygon, esriTinSurfaceType.esriTinHardClip, 0);  //this must be done after adding any feature
            //TinEdit.AddShape((IGeometry)cpg.pPolygon, esriTinSurfaceType.esriTinContour, 0);  //this must be done after adding any feature
            //TinEdit.AddShape((IGeometry)cpg.pPolygon, esriTinSurfaceType.esriTinHardClip, 0);
            TinEdit.Refresh();
            //TinEdit.StopEditing(false);

            int intCount2 = pTinAdvanced2.DataNodeCount;
            //TinEdit.d
            HandleException(intCount1, intCount2, this.cptSD, TinEdit);
            //double dblverysmall = CConstants.dblVerySmall;


            _pTinAdvanced2 = pTinAdvanced2;


            //int intTCOUNT = _pTinAdvanced2.DataTriangleCount;

            //int intTCOUNT2 = _pTinAdvanced2.TriangleCount;

            //int intCount3 = 0;
            //for (int i = 0; i < _pTinAdvanced2.TriangleCount; i++)
            //{
            //    ITinTriangle pTinTriangle = _pTinAdvanced2.GetTriangle(i + 1);

            //    if (pTinTriangle.IsInsideDataArea == true)
            //    {
            //        intCount3++;
            //    }
            //}


            _pTinEdit = TinEdit;

            return TinEdit;
        }


        private void HandleException(int intCount1, int intCount2, SortedDictionary<CPoint, CPoint> pCptSD, ITinEdit pTinEdit)
        {
            if (intCount1==intCount2)
            {
                return;
            }
            else if (intCount1<intCount2)
            {
                var pTinAdvanced2 = pTinEdit as ITinAdvanced2;
                //pTinEdit.s
                
                var nullnodelt = new List<ITinNode>(intCount2 - intCount1);
                var ITinNodeDataAreaLt = GetITinNodeLt(pTinAdvanced2);

                foreach (var pTinNode in ITinNodeDataAreaLt)
                {
                    var cpt = new CPoint(pTinNode);
                    if (pCptSD.ContainsKey(cpt)==false)
                    {
                        nullnodelt.Add(pTinNode);
                    }
                }

                foreach (var pTinNode in nullnodelt)
                {
                    pTinEdit.DeleteNode(pTinNode.Index);
                }

                pTinEdit.Refresh();
                //pTinEdit.StopEditing(false);
                //pTinEdit.DeleteNode(ITinNodeDataAreaLt[2].Index);

                //for (int i = 0; i < length; i++)
                //{
                    
                //}

                Console .WriteLine("We have deleted some nodes of a triangulation because when we set a constraint to a triangulation, it produces more points: " + nullnodelt .Count);




            }
            else if (intCount1 > intCount2)
            {
                throw new ArgumentException("after add some constraints, the DataNodeCount decreases!");
            }

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


        /// <summary>
        /// 
        /// </summary>
        /// <returns>the boundary edges are stored in the front of the returned list.</returns>
        /// <remarks>this method may be only used for the construction of compatible triangulations</remarks>
        public List<CEdge> GenerateRealTinCEdgeLt()
        {
            var vertexlt = this.CptLt;
            vertexlt.SetIndexID();
            var vertexSD = this.cptSD;



            var ITinEdgeLt = GenerateITinEdgeLt();

            var tincedgeSS = new SortedSet<CEdge>(new CCmpEdge_CptIndexIDDiff());   //we use the difference of the TagValue, so that we know the boundary edges form the difference is 1

            foreach (var tinedge in ITinEdgeLt)
            {
                //there are always a pair of edges between a pair of triangles, but we only need one edge. So we reverse some edges than we can delete one edge of each pair
                var pFrNode = tinedge.FromNode;
                var pToNode = tinedge.ToNode;

                CPoint frcpt, tocpt;
                FindCorrCptInSDByTinNode(pFrNode, vertexSD, out frcpt);
                FindCorrCptInSDByTinNode(pToNode, vertexSD, out tocpt);

                if (frcpt.indexID < tocpt.indexID)
                {
                    tincedgeSS.Add(new CEdge(frcpt, tocpt));  //notice that the duplicate cedge will not be added into tincedgeSS
                }
                else
                {
                    tincedgeSS.Add(new CEdge(tocpt, frcpt));  //notice that the duplicate cedge will not be added into tincedgeSS
                }
            }
            var tincedgelt = tincedgeSS.ToList();

            //the boundary edges are stored in the front of the returned list
            //remove the last boundary cedge (with the largest difference of IndexID) and insert it into the correct position
            var lastcedge = tincedgelt.GetLastT();
            var newlastcedge = new CEdge(lastcedge.ToCpt, lastcedge.FrCpt);
            tincedgelt.Insert(_intN - 1, newlastcedge);
            tincedgelt.RemoveLastT();

            this.CEdgeLt = tincedgelt;
            return tincedgelt;
        }



        private bool FindCorrCptInSDByTinNode(ITinNode pTinNode, SortedDictionary<CPoint, CPoint> CptSD, out CPoint outcpt)
        {
            CPoint tincpt = new CPoint(pTinNode);
            bool isFound = CptSD.TryGetValue(tincpt, out outcpt);

            if (isFound == false)
            {
                Console.WriteLine("cannot find a point" + tincpt.X + "  " + tincpt.Y + "   " + " of a TinNode in: " + this.ToString() + ".cs   ");
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="cedgelt"></param>
        ///// <returns>if cedge.pTinEdge.FromNode.TagValue smaller than cedge.pTinEdge.ToNode.TagValue, it is consistent, otherwise not.
        ///// therefore, the returned edges have the attribute that cedge.pTinEdge.FromNode.TagValue is always smaller than cedge.pTinEdge.ToNode.TagValue. 
        ///// This is also true for FrCpt.pTinNode and ToCpt.pTinNode</returns>
        //public List<CEdge> GetConsistentCEdge(List<CEdge> cedgelt)
        //{
        //    var newcedgelt = new List<CEdge>(cedgelt.Count);
        //    foreach (var cedge in cedgelt)
        //    {
        //        if (cedge.pTinEdge.FromNode.TagValue == cedge.FrCpt.pTinNode.TagValue ||
        //            cedge.pTinEdge.ToNode.TagValue == cedge.ToCpt.pTinNode.TagValue)
        //        {
        //            newcedgelt.Add(cedge);
        //        }
        //    }
        //    return newcedgelt;
        //}



        public List<ITinEdge> GenerateITinEdgeLt(bool blnDataArea = true)
        {
            ITinAdvanced2 ptinadvanced2 = _pTinAdvanced2;
            List<ITinEdge> ITinEdgeLt = new List<ITinEdge>(ptinadvanced2.EdgeCount);
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

        public void CompareCptltAndNode()
        {
            var cptlt = this.CptLt;
            //var cptSD = cptlt.ToSD(cpt => cpt, new CCmpCptYX());

            ITinAdvanced2 ptinadvanced2 = _pTinAdvanced2;
            var cptNodeDataAreaLt = new List<CPoint>(ptinadvanced2.DataNodeCount); 
            for (int i = 0; i < ptinadvanced2.NodeCount; i++)
            {
                var ptinnode = ptinadvanced2.GetNode(i+1);

                if (ptinnode.IsInsideDataArea)
                {
                    cptNodeDataAreaLt.Add(new CPoint(ptinnode));


                    //var cptNode = new CPoint(ptinnode);
                    //cptSD.Remove(cptNode);
                }
            }

            int intLeftCount1 = CompareCptltAndNode(cptlt, cptNodeDataAreaLt);
            int intLeftCount2 = CompareCptltAndNode(cptNodeDataAreaLt, cptlt);
            //ptinadvanced2.edit
        }

        private int CompareCptltAndNode(List <CPoint > cptlt1, List <CPoint > cptlt2)
        {
            var cptSD = cptlt1.ToSD(cpt => cpt, new CCmpCptYX_VerySmall());
            foreach (var cpt2 in cptlt2)
            {
                cptSD.Remove(cpt2);
            }
            return cptSD.Count;
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






        public CPolygon CPg
        {
            get { return _CPg; }
            set { _CPg = value; }
        }


        public ITinEdit pTinEdit
        {
            get { return _pTinEdit; }
            set { _pTinEdit = value; }
        }

        public List<ITinEdge> pTinEdgeLt
        {
            get { return _pTinEdgeLt; }
            set { _pTinEdgeLt = value; }
        }

        public ITinAdvanced2 pTinAdvanced2
        {
            get { return _pTinAdvanced2; }
            set { _pTinAdvanced2 = value; }
        }

    }
}
