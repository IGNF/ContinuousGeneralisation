﻿using System;
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
        public SortedDictionary<CPoint, CPoint> cptSD { set; get; }  //using CCmpCptYX_VerySmall.sComparer
        public List<CEdge> ConstructingCEdgeLt { set; get; }

        public CTriangulation(CPolygon cpg)
        {
            _CPg = cpg;
            var cptlt = new List<CPoint>(cpg.CptLt);
            cptlt.RemoveLastT();  // the last one has the same coordinates with the first one
            this.cptSD = CHelpFunc.TestCloseCpts(cptlt);
            this.CptLt = cptlt;

            //cpg.JudgeAndFormCEdgeLt();
            //this.ConstructingCEdgeLt.AddRange(cpg.CEdgeLt);
        }

        public CTriangulation(CPolygon cpg, List<CPoint> fcptlt)
        {
            _CPg = cpg;
            var cptlt = new List<CPoint>(fcptlt);
            this.cptSD = CHelpFunc.TestCloseCpts(cptlt);
            this.CptLt = cptlt;

            //cpg.JudgeAndFormCEdgeLt();
            //this.ConstructingCEdgeLt.AddRange(cpg.CEdgeLt);
        }


        public ITinEdit Triangulate(List<CPolyline> ConstraintCpllt=null, string strIdentity = "", bool blnSave=false)
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

            //pTinFeatureEdit.AddPolylineZ
            int intCount1 = pTinAdvanced2.DataNodeCount;            

            if (blnSave==true)
            {
                TinEdit.SaveAs(CConstants.ParameterInitialize.strSavePathBackSlash + "TinBeforeSettingDataArea" + strIdentity );
            }

            //we are not allowed to use AddShapeZ. 
            //it will report that there is no Z value in the shape, even we already set Z value to 0
            //The reason may be that we actually need polyhedron
            //TinEdit.AddShapeZ((IGeometry)cpg.pPolygon, esriTinSurfaceType.esriTinHardClip, 0); 

            //this function set the "data area" of the TIN
            TinEdit.AddShape((IGeometry)cpg.pPolygon, esriTinSurfaceType.esriTinHardClip, 0);
            TinEdit.Refresh();

            if (blnSave == true)
            {
                TinEdit.SaveAs(CConstants.ParameterInitialize.strSavePathBackSlash + "TinAfterSettingDataArea" + strIdentity );
            }

            int intCount2 = pTinAdvanced2.DataNodeCount;
            HandleException(intCount1, intCount2, this.cptSD, TinEdit);
            //double dblverysmall = CConstants.dblVerySmall;


            _pTinAdvanced2 = pTinAdvanced2;
            

            return TinEdit;
        }


        private void HandleException(int intCount1, int intCount2, SortedDictionary<CPoint, CPoint> pCptSD, ITinEdit pTinEdit)
        {
            if (intCount1 == intCount2)
            {
                return;
            }
            else if (intCount1 < intCount2)
            {
                var pTinAdvanced2 = pTinEdit as ITinAdvanced2;
                //pTinEdit.s

                var nullnodelt = new List<ITinNode>(intCount2 - intCount1);
                var ITinNodeDataAreaLt = GetITinNodeLt(pTinAdvanced2);

                foreach (var pTinNode in ITinNodeDataAreaLt)
                {
                    var cpt = new CPoint(pTinNode);
                    if (pCptSD.ContainsKey(cpt) == false)
                    {
                        nullnodelt.Add(pTinNode);
                    }
                }

                foreach (var pTinNode in nullnodelt)
                {
                    pTinEdit.DeleteNode(pTinNode.Index);
                }

                pTinEdit.Refresh();

                Console.Write("We have deleted some nodes of a triangulation");
                Console.WriteLine("because when we set a constraint to a triangulation, it produces more points: "
                    + nullnodelt.Count);

            }
            else if (intCount1 > intCount2)
            {
                //this is useful to test if we have correct compatible triangulations
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
        public List<CEdge> GenerateRealTinCEdgeLt(List<CEdge> KnownCEdgeLt = null)
        {
            this.CptLt.SetIndexID();
            var vertexSD = this.cptSD;


            var ITinEdgeLt = GenerateITinEdgeLt();

            //we use the difference of the TagValue, so that we know the boundary edges form the difference is 1
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
            //var tincedgelt = tincedgeSS.ToList();

            //the boundary edges are stored in the front of the returned list
            //remove the last boundary cedge (with the largest difference of IndexID) and insert it into the correct position
            //var lastcedge = tincedgelt.GetLastT();
            //var newlastcedge = new CEdge(lastcedge.ToCpt, lastcedge.FrCpt);
            //tincedgelt.Insert(this.CptLt.Count - 1, newlastcedge);
            //tincedgelt.RemoveLastT();

            this.CEdgeLt = tincedgelt;
            return tincedgelt;
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <returns>the boundary edges are stored in the front of the returned list.</returns>
        ///// <remarks>this method may be only used for the construction of compatible triangulations</remarks>
        //public List<CEdge> GenerateRealTinCEdgeLt(List<CEdge> KnownCEdgeLt = null)
        //{
        //    this.CptLt.SetIndexID();
        //    var vertexSD = this.cptSD;


        //    var ITinEdgeLt = GenerateITinEdgeLt();

        //    //we use the difference of the TagValue, so that we know the boundary edges form the difference is 1
        //    var tincedgeSS = new SortedSet<CEdge>(new CCmpEdge_CptIndexIDDiff());

        //    foreach (var tinedge in ITinEdgeLt)
        //    {
        //        //there are always a pair of edges between a pair of triangles, but we only need one edge. 
        //        //So we reverse some edges than we can delete one edge of each pair
        //        var pFrNode = tinedge.FromNode;
        //        var pToNode = tinedge.ToNode;

        //        CPoint frcpt, tocpt;
        //        FindCorrCptInSDByTinNode(pFrNode, vertexSD, out frcpt);
        //        FindCorrCptInSDByTinNode(pToNode, vertexSD, out tocpt);

        //        if (frcpt.indexID < tocpt.indexID)
        //        {
        //            tincedgeSS.Add(new CEdge(frcpt, tocpt));  //notice that the duplicated cedge will not be added into tincedgeSS
        //        }
        //        else
        //        {
        //            tincedgeSS.Add(new CEdge(tocpt, frcpt));  //notice that the duplicated cedge will not be added into tincedgeSS
        //        }
        //    }
        //    var tincedgelt = tincedgeSS.ToList();

        //    //the boundary edges are stored in the front of the returned list
        //    //remove the last boundary cedge (with the largest difference of IndexID) and insert it into the correct position
        //    var lastcedge = tincedgelt.GetLastT();
        //    var newlastcedge = new CEdge(lastcedge.ToCpt, lastcedge.FrCpt);
        //    tincedgelt.Insert(this.CptLt.Count - 1, newlastcedge);
        //    tincedgelt.RemoveLastT();

        //    this.CEdgeLt = tincedgelt;
        //    return tincedgelt;
        //}



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
