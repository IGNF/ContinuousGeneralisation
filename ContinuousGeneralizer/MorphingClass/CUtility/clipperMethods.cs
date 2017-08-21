using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Geometry;

using MorphingClass.CAid;
using MorphingClass.CEntity;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;

using ClipperLib;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace MorphingClass.CUtility
{
    public static class clipperMethods
    {
        public static Paths Offset_Paths(Paths inputpaths, double dbldelta,
           string strBufferStyle = "Miter", double dblMiterLimit = 2)
        {
            //Console.WriteLine("dblMiterLimit = " + dblMiterLimit);

            //keep in mind that the first point and the last point of a output path are not identical
            //the direction of a path of outcome is counter-clockwise, whereas it is clockwise for a IPolygon            
            ClipperOffset pClipperOffset = new ClipperOffset(dblMiterLimit);
            switch (strBufferStyle)
            {
                case "Miter":
                    //Property MiterLimit sets the maximum distance in multiples of delta that vertices can be offset from 
                    //their original positions before squaring is applied. (Squaring truncates a miter by 'cutting it off' 
                    //at 1 × delta distance from the original vertex.)
                    pClipperOffset.AddPaths(inputpaths, JoinType.jtMiter, EndType.etClosedPolygon);
                    break;
                case "Round":
                    //pClipperOffset.ArcTolerance *= dblFactorClipper;
                    pClipperOffset.AddPaths(inputpaths, JoinType.jtRound, EndType.etClosedPolygon);
                    break;
                case "Square":
                    pClipperOffset.AddPaths(inputpaths, JoinType.jtSquare, EndType.etClosedPolygon);
                    break;
                default:
                    break;
            }

            var offsetPaths = new Paths();
            pClipperOffset.Execute(ref offsetPaths, dbldelta);

            return offsetPaths;
        }

        public static PolyTree Offset_PolyTree(Paths inputpaths, double dbldelta, string strBufferStyle = "Miter",
            double dblMiterLimit = 2)
        {
            //Console.WriteLine("dblMiterLimit = " + dblMiterLimit);

            //keep in mind that the first point and the last point of a path are not identical
            //the direction of a path of outcome is counter-clockwise, whereas it is clockwise for a IPolygon
            ClipperOffset pClipperOffset = new ClipperOffset(dblMiterLimit);
            switch (strBufferStyle)
            {
                case "Miter":
                    //Property MiterLimit sets the maximum distance in multiples of delta that vertices can be offset from 
                    //their original positions before squaring is applied. (Squaring truncates a miter by 'cutting it off' 
                    //at 1 × delta distance from the original vertex.)
                    pClipperOffset.AddPaths(inputpaths, JoinType.jtMiter, EndType.etClosedPolygon);
                    break;
                case "Round":
                    //pClipperOffset.ArcTolerance *= dblFactorClipper;
                    pClipperOffset.AddPaths(inputpaths, JoinType.jtRound, EndType.etClosedPolygon);
                    break;
                case "Square":
                    pClipperOffset.AddPaths(inputpaths, JoinType.jtSquare, EndType.etClosedPolygon);
                    break;
                default:
                    break;
            }

            var pPolyTree = new PolyTree();
            pClipperOffset.Execute(ref pPolyTree, dbldelta);

            return pPolyTree;
        }



        public static Paths Clip_Paths(Paths subjpaths, bool blnSubjClosed,
           Paths clippaths, bool blnClipClosed, ClipType pClipType)
        {
            Clipper pClipper = new Clipper();
            if (pClipper.AddPaths(subjpaths, PolyType.ptSubject, blnSubjClosed) == false ||
                pClipper.AddPaths(clippaths, PolyType.ptClip, blnClipClosed) == false)
            {
                throw new ArgumentException("failed to add paths!");
            }

            Paths clippedPaths = new Paths();
            if (pClipper.Execute(pClipType, clippedPaths) == false)
            {
                throw new ArgumentException("failed to cut!");
            }

            return clippedPaths;
        }

        public static PolyTree Clip_PolyTree(Paths subjpaths, bool blnSubjClosed,
            Paths clippaths, bool blnClipClosed, ClipType pClipType)
        {
            Clipper pClipper = new Clipper();

            //var blnadd = pClipper.AddPaths(subjpaths, PolyType.ptSubject, blnSubjClosed);
            //var blnad2 = pClipper.AddPaths(clippaths, PolyType.ptClip, blnClipClosed);

            if (pClipper.AddPaths(subjpaths, PolyType.ptSubject, blnSubjClosed) == false ||
                pClipper.AddPaths(clippaths, PolyType.ptClip, blnClipClosed) == false)
            {
                throw new ArgumentException("failed to add paths!");
            }

            PolyTree clippedPolyTree = new ClipperLib.PolyTree();
            if (pClipper.Execute(pClipType, clippedPolyTree) == false)
            {
                throw new ArgumentException("failed to cut!");
            }

            return clippedPolyTree;
        }


        public static List<List<CPolygon>> GroupCpgsByOverlapIndependently(List<CPolygon> cpglt,
double dblGrow, double dblDilation, double dblEpsilon, string strBufferStyle = "Miter", double dblMiterLimit = 2)
        {
            var cpglyDilationPolyTree = GetPolyTreeForGroupedCpgs(cpglt, dblGrow, dblDilation, dblEpsilon, strBufferStyle, dblMiterLimit);
            var groupedcpgltlt = GroupCpgsByOverlap(cpglt, cpglyDilationPolyTree);

            int intCpgCount = 0;
            groupedcpgltlt.ForEach(groupedcpglt => intCpgCount += groupedcpglt.Count);

            if (intCpgCount != cpglt.Count)
            {
                throw new ArgumentException("some polygons are lost!");
            }

            return groupedcpgltlt;
        }

        private static PolyTree GetPolyTreeForGroupedCpgs(List<CPolygon> cpglt,
double dblGrow, double dblDilation, double dblEpsilon, string strBufferStyle = "Miter", double dblMiterLimit = 2)
        {
            var cpgltPaths = new Paths(cpglt.Count);
            cpglt.ForEach(cpg => cpgltPaths.AddRange(cpg.ExteriorPaths));
            var overgrownPaths = Offset_Paths(cpgltPaths, dblGrow + Math.Sqrt(5) * dblEpsilon / 2, strBufferStyle, dblMiterLimit);

            var overdilationPaths = Offset_Paths(overgrownPaths, dblDilation, strBufferStyle, dblMiterLimit);
            var stablePolyTree = Offset_PolyTree(overdilationPaths, -dblDilation, strBufferStyle, dblMiterLimit);

            //var stablePolyTreePaths = Clipper.PolyTreeToPaths(stablePolyTree);
            //CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
            //stablePolyTreePaths, true), 1 / CConstants.dblFclipper), CHelpFunc.GetTimeStamp());

            return stablePolyTree;
        }


        //public static List<List<CPolygon>> DilateAndOverlapPaths(List<CPolygon> cpglt, List<List<CPolygon>> GroupedCpgLtLt,
        //    double dblGrow, double dblDilation, double dblEpsilon, string strBufferStyle = "Miter", double dblMiterLimit = 2)
        //{
        //    var GrownPaths = new Paths();
        //    foreach (var groupedCpgLt in GroupedCpgLtLt)
        //    {
        //        var groupedPaths = new Paths();
        //        foreach (var cpg in groupedCpgLt)
        //        {
        //            groupedPaths.AddRange(cpg.ExteriorPaths);
        //        }

        //        var overdilationPaths = Offset_Paths(groupedPaths, dblGrow + dblDilation, strBufferStyle, dblMiterLimit);
        //        var grownpaths = Offset_Paths(overdilationPaths, -dblDilation, strBufferStyle, dblMiterLimit);
        //        GrownPaths.AddRange(grownpaths);
        //    }

        //    //if two polygons intersect, they will be too close without overdiating dblEpsilon / 2
        //    var AllOverDilationPolyTree = Offset_PolyTree(GrownPaths, Math.Sqrt(5) * dblEpsilon / 2, strBufferStyle, dblMiterLimit);

        //    CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
        //        Clipper.PolyTreeToPaths(AllOverDilationPolyTree), true), 1 / CConstants.dblFclipper),
        //        CHelpFunc.GetTimeStamp());

        //    return GroupCpgsByOverlap(cpglt, AllOverDilationPolyTree);
        //}



        private static List<List<CPolygon>> GroupCpgsByOverlap(List<CPolygon> cpglt, PolyTree polyTree)
        {
            var GroupedCpgLtLt = new List<List<CPolygon>>(polyTree.ChildCount);
            var testCpgLt = cpglt;
            var QueueNode = new Queue<PolyNode>(polyTree.Childs);

            while (QueueNode.Count > 0)
            {
                var polynode = QueueNode.Dequeue();
                var nodePaths = new Paths { polynode.Contour };
                if (polynode.ChildCount > 0)
                {
                    foreach (var holepolynode in polynode.Childs)
                    {
                        nodePaths.Add(holepolynode.Contour);

                        //do it recursively
                        if (holepolynode.ChildCount > 0)
                        {
                            foreach (var exteriorpolynode in holepolynode.Childs)
                            {
                                QueueNode.Enqueue(exteriorpolynode);
                            }
                        }
                    }
                }

                //overlap
                var remainCpgLt = new List<CPolygon>(testCpgLt.Count);
                var groupCpglt = new List<CPolygon>();
                foreach (var testCpg in testCpgLt)
                {
                    var clippedPaths = Clip_Paths(nodePaths, true,
                        testCpg.ExteriorPaths, true, ClipType.ctIntersection);
                    if (clippedPaths == null || clippedPaths.Count == 0)
                    {
                        remainCpgLt.Add(testCpg);
                    }
                    else
                    {
                        groupCpglt.Add(testCpg);
                    }
                }
                GroupedCpgLtLt.Add(groupCpglt);
                testCpgLt = remainCpgLt;
            }

            return GroupedCpgLtLt;
        }     


        public static PolyTree ClipOneComponent_BufferDilateErode(CPolygon cpg,
            double dblGrow, double dblDilation, double dblErosion, 
            Paths clipPaths, double dblFclipper, string strBufferStyle = "Miter", double dblMiterLimit = 2)
        {
            var ExteriorOffsetPaths = clipperMethods.DilateErodeOffsetCpgExterior_Paths
    (cpg, dblGrow, dblDilation, dblErosion, strBufferStyle, dblMiterLimit);
            var clippedPolyTree = clipperMethods.Clip_PolyTree(ExteriorOffsetPaths, true, clipPaths, true, ClipType.ctIntersection);


            //var clippedPolyTree = clipperMethods.Clip_PolyTree(ExteriorOffsetPaths, true, clipPaths.GetRange(0,1), true, ClipType.ctIntersection);
            //if (clipPaths.Count > 1)
            //{
            //    clippedPolyTree = clipperMethods.Clip_PolyTree(Clipper.PolyTreeToPaths(clippedPolyTree), true, clipPaths.GetRange(1, clipPaths.Count - 1), true, ClipType.ctDifference);
            //}


    //        if (clipPaths.Count>5)
    //        {
    //            CSaveFeature.SaveCGeoEb(clipperMethods.ScaleCpgLt(CHelpFunc.MakeLt(cpg), 1 / dblFclipper), 
    //                esriGeometryType.esriGeometryPolygon, "Cpg" + CHelpFunc.GetTimeStampWithPrefix(), pParameterInitialize);
    //            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
    //ExteriorOffsetPaths, true), 1 / dblFclipper), "ExteriorOffsetPaths" +CHelpFunc.GetTimeStampWithPrefix(), pParameterInitialize);
    //            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
    //clipPaths, true), 1 / dblFclipper), "clipPaths" + CHelpFunc.GetTimeStampWithPrefix(), pParameterInitialize);

    //            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathToCEdgeEb(
    //    clippedPolyTree.Childs[0].Contour, true), 1 / dblFclipper), "clipperPolyTree0" + CHelpFunc.GetTimeStampWithPrefix(), pParameterInitialize);
    //            //            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathToCEdgeEb(
    //            //    clippedPolyTree.Childs[1].Contour, true), 1 / dblFclipper), "clipperPolyTree1", pParameterInitialize);
    //        }



            return clippedPolyTree;
        }


        


        public static Paths DilateErodeOffsetCpgExterior_Paths(CPolygon Cpg, double dblGrow, double dblDilation, double dblErosion,
    string strBufferStyle, double dblMiterLimit)
        {
            double dblHoleIndicator = 1;
            if (Cpg.IsHole == true)
            {
                dblHoleIndicator = -1;
            }

            if (Cpg.ExteriorPaths == null)
            {
                Cpg.ExteriorPaths = CHelpFunc.MakeLt(GeneratePathByCpgExterior(Cpg));
            }
            var overdilationPaths = Offset_Paths(Cpg.ExteriorPaths,
                    dblHoleIndicator * (dblGrow + dblDilation), strBufferStyle, dblMiterLimit);
            var erosionpaths = Offset_Paths(overdilationPaths,
                    dblHoleIndicator * (-dblDilation - dblErosion), strBufferStyle, dblMiterLimit);


            //var ExteriorPath = GeneratePathByCpgExterior(Cpg);

            //var growndilationPaths = Offset_Paths(CHelpFunc.MakeLt(ExteriorPath), 
            //    dblHoleIndicator * (dblGrow ), strBufferStyle, dblMiterLimit);

            //var overdilationPaths = Offset_Paths(growndilationPaths, 
            //    dblHoleIndicator * (dblDilation), strBufferStyle, dblMiterLimit);

            //var backPaths = Offset_Paths(overdilationPaths, 
            //    dblHoleIndicator * (-dblDilation), strBufferStyle, dblMiterLimit);

            //var erosionpaths = Offset_Paths(backPaths, 
            //    dblHoleIndicator * (- dblErosion), strBufferStyle, dblMiterLimit);



//            CSaveFeature.SaveCGeoEb(clipperMethods.ScaleCpgLt(CHelpFunc.MakeLt(Cpg), 1 / CConstants.dblFclipper),
//                esriGeometryType.esriGeometryPolygon, "Cpg" + CHelpFunc.GetTimeStampWithPrefix(), CConstants.ParameterInitialize);
////            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
////growndilationPaths, true), 1 / CConstants.dblFclipper), "growndilationPaths" + CHelpFunc.GetTimeStampWithPrefix(), CConstants.ParameterInitialize);
//            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
// overdilationPaths, true), 1 / CConstants.dblFclipper), "overdilationPaths" + CHelpFunc.GetTimeStampWithPrefix(), CConstants.ParameterInitialize);
////            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
////backPaths, true), 1 / CConstants.dblFclipper), "backPaths" + CHelpFunc.GetTimeStampWithPrefix(), CConstants.ParameterInitialize);
//            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
//erosionpaths, true), 1 / CConstants.dblFclipper), "erosionpaths" + CHelpFunc.GetTimeStampWithPrefix(), CConstants.ParameterInitialize);


            return Offset_Paths(erosionpaths, dblHoleIndicator * dblErosion, strBufferStyle, dblMiterLimit);




        }

        public static PolyTree DilateErodeOffsetCpgExterior_PolyTree(CPolygon Cpg,
    double dblGrow, double dblDilation, double dblErosion, string strBufferStyle, double dblMiterLimit)
        {
            double dblHoleIndicator = 1;
            if (Cpg.IsHole == true)
            {
                dblHoleIndicator = -1;
            }

            if (Cpg.ExteriorPaths == null)
            {
                Cpg.ExteriorPaths = CHelpFunc.MakeLt(GeneratePathByCpgExterior(Cpg));
            }
            var overdilationPaths = Offset_Paths(Cpg.ExteriorPaths,
                    dblHoleIndicator * (dblGrow + dblDilation), strBufferStyle, dblMiterLimit);
            var erosionpaths = Offset_Paths(overdilationPaths,
                    dblHoleIndicator * (-dblDilation - dblErosion), strBufferStyle, dblMiterLimit);
            return Offset_PolyTree(erosionpaths, dblHoleIndicator * dblErosion, strBufferStyle, dblMiterLimit);
        }

        /// <summary>
        /// OLH: One Level Hole; we assume that there is only one polygon in pPolyTree
        /// </summary>
        /// <param name="pPolyTree"></param>
        /// <returns></returns>
        /// <remarks></remarks>
        public static CPolygon GenerateOLHCpgByPolyTree(PolyTree pPolyTree, int intID)
        {
            if (pPolyTree.ChildCount > 1)
            {
                throw new ArgumentOutOfRangeException("there should be no more than 1 polygon!");
            }

            return GenerateOLHCpgEbByPolyNode(pPolyTree.Childs[0], intID);
        }

        public static IEnumerable<CPolygon> GenerateOLHCpgEbByPolyTree(PolyTree pPolyTree, int intID, bool isExteriorHole = false)
        {
            foreach (var polyNode in pPolyTree.Childs)
            {
                yield return GenerateOLHCpgEbByPolyNode(polyNode, intID, isExteriorHole);
            }
        }

        public static CPolygon GenerateOLHCpgEbByPolyNode(PolyNode cpgnode, int intID, bool isExteriorHole = false)
        {
            var cptlt = ContourToCptEb(cpgnode.Contour, isExteriorHole).ToList();
            var holecptlt = GetOLHCptLtEb(cpgnode, isExteriorHole).ToLtLt();

            return new CPolygon(intID, cptlt, holecptlt);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpgNode"></param>
        /// <param name="blnMakeIdentical"></param>
        /// <returns></returns>
        /// <remarks>OLH: One Level Hole</remarks>
        public static IEnumerable<IEnumerable<CPoint>> GetOLHCptLtEb(PolyNode cpgNode, bool isExteriorHole = false)
        {
            foreach (var holenode in cpgNode.Childs)
            {
                if (holenode.ChildCount > 0)
                {
                    throw new ArgumentException("unconsidered case: the hole contains holes!");
                }

                yield return ContourToCptEb(holenode.Contour, isExteriorHole);
            }
        }

        /// <summary>we do reverse because the direction of contour is different from the direction of our Cpg</summary>
        public static IEnumerable<CPoint> ContourToCptEb(Path Contour, bool isExteriorHole = false)
        {
            var ContourAddFirst = CGeoFunc.AddFirstAsLastForEb(Contour);
            if (isExteriorHole == false)
            {
                ContourAddFirst = ContourAddFirst.Reverse();
            }

            int intCount = 0;
            foreach (var intPt in ContourAddFirst)
            {
                yield return new CPoint(intCount++, intPt.X, intPt.Y);
            }
        }


        public static List<CEdge> ConvertPathsToCEdgeLt(Paths Paths, bool AddCEdge_lastcptTofirstcpt = false)
        {
            var CEdgeLt = new List<CEdge>();
            foreach (var path in Paths)
            {
                CEdgeLt.AddRange(ConvertPathToCEdgeEb(path, AddCEdge_lastcptTofirstcpt));
            }
            return CEdgeLt;
        }


        public static int CalEdgeCountPaths(Paths paths)
        {
            int intEdgeCount = 0;
            foreach (var path in paths)
            {
                intEdgeCount += path.Count - 1;
            }
            return intEdgeCount;
        }

        public static double CalLengthPaths(Paths paths)
        {
            double dblLength = 0;
            foreach (var path in paths)
            {
                dblLength += CGeoFunc.CalLengthForIntptEb(path);
            }
            return dblLength;
        }


        //public static IEnumerable<IEnumerable<CEdge>> ConvertPathsToCEdgeEbEb(Paths paths)
        //{
        //    foreach (var path in paths)
        //    {
        //        yield return CGeoFunc.FormCEdgeEb(ConvertPathToCptEb(path));
        //    }
        //}

        //public static IEnumerable<IEnumerable<CPoint>> ConvertPathsToCptEbEb(Paths paths)
        //{
        //    foreach (var path in paths)
        //    {
        //        yield return ConvertPathToCptEb(path);
        //    }
        //}

        public static IEnumerable<CEdge> ConvertPathToCEdgeEb(Path path, bool AddCEdge_lastcptTofirstcpt = false)
        {
            return CGeoFunc.FormCEdgeEb(ConvertPathToCptEb(path), AddCEdge_lastcptTofirstcpt);
        }

        public static IEnumerable<CPoint> ConvertPathToCptEb(Path path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                yield return new CPoint(i, path[i].X, path[i].Y);
            }
        }

        //public static IEnumerable<List<IntPoint>> GeneratePathsByCpgltCedgeltVP(
        //    CValPair<List<CPolygon>, List<CEdge>> cpgltcedgeltVP)
        //{
        //    //generate paths for polygons
        //    foreach (var path in GeneratePathsByCpgCol(cpgltcedgeltVP.val1))
        //    {
        //        yield return path;
        //    }

        //    //generate a path for each edge
        //    foreach (var cedge in cpgltcedgeltVP.val2)
        //    {
        //        yield return GeneratePathEbByCEdge(cedge).ToList();
        //    }
        //}



        //public static IEnumerable<List<IntPoint>> GeneratePathEbByCpgCol(ICollection<CPolygon> cpgCol)
        //{
        //    foreach (var cpg in cpgCol)
        //    {
        //        foreach (var path in GeneratePathEbByCpg(cpg))
        //        {
        //            yield return path;
        //        }
        //    }
        //}

        public static Paths GeneratePathsByCpgEb(IEnumerable<CPolygon> cpgEb)
        {
            Paths paths = new Paths();
            foreach (var cpg in cpgEb)
            {
                paths.AddRange(GeneratePathEbByCpg(cpg));
            }

            return paths;
        }

        public static Paths GenerateClipPathsByCpgEb(IEnumerable<CPolygon> cpgEb)
        {
            Paths paths = new Paths();
            foreach (var cpg in cpgEb)
            {
                paths.AddRange(GenerateClipPathEbByCpg(cpg));
            }

            return paths;
        }

        private static IEnumerable<Path> GenerateClipPathEbByCpg(CPolygon cpg)
        {
            yield return GeneratePathByCptEb(cpg.CptLt, true);

            if (cpg.HoleCpgLt != null)
            {
                foreach (var holecpg in cpg.HoleCpgLt)
                {
                    yield return GeneratePathByCptEb(holecpg.CptLt, true);
                }
            }
        }

        public static Paths GeneratePathsByCpg(CPolygon cpg)
        {
            return GeneratePathEbByCpg(cpg).ToList();
        }

        private static IEnumerable<Path> GeneratePathEbByCpg(CPolygon cpg)
        {
            yield return GeneratePathByCptEb(cpg.CptLt, true);

            if (cpg.HoleCpgLt != null)
            {
                foreach (var holecpg in cpg.HoleCpgLt)
                {
                    yield return GeneratePathByCptEb(cpg.CptLt, true);
                }
            }
        }

        public static Path GeneratePathByCpgExterior(CPolygon cpg)
        {
            return GeneratePathByCptEb(cpg.CptLt, true);
        }

        public static Path GeneratePathByCptEb(IEnumerable<CPoint> cpteb, bool blnReverse = false)
        {
            var IntPtEb = GenerateIntptEbByCptEb(cpteb);
            if (blnReverse == true)
            {
                return IntPtEb.Reverse().ToList();
            }
            else
            {
                return IntPtEb.ToList();
            }
        }

        public static IEnumerable<IntPoint> GenerateIntptEbByCptEb(IEnumerable<CPoint> cpteb)
        {
            foreach (var cpt in cpteb)
            {
                yield return new IntPoint(cpt.X, cpt.Y);
            }
        }

        /// <summary>generate a path for each edge</summary>
        public static Paths GeneratePathsByCEdgeLt(List<CEdge> cedgelt)
        {
            return GeneratePathEbByCEdgeLt(cedgelt).ToList();
        }

        public static IEnumerable<Path> GeneratePathEbByCEdgeLt(List<CEdge> cedgelt)
        {
            //generate a path for each edge
            foreach (var cedge in cedgelt)
            {
                yield return GeneratePathByCEdge(cedge);
            }
        }

        public static Path GeneratePathByCEdge(CEdge cedge)
        {
            return new Path
            {
                new IntPoint(cedge.FrCpt.X, cedge.FrCpt.Y),
                new IntPoint(cedge.ToCpt.X, cedge.ToCpt.Y)
            };
        }

        //public static Path GenerateClosedClockwisePathByCEdge(CEdge cedge)
        //{
        //    return new Path
        //    {
        //        new IntPoint(cedge.FrCpt.X, cedge.FrCpt.Y),
        //        new IntPoint(cedge.ToCpt.X, cedge.ToCpt.Y),
        //        GetArtefactPt(cedge),
        //        new IntPoint(cedge.FrCpt.X, cedge.FrCpt.Y)
        //    };
        //}

        //public static IntPoint GetArtefactPt(CEdge cedge)
        //{
        //    var frcpt = cedge.FrCpt;
        //    var tocpt = cedge.ToCpt;

        //    if (tocpt.Y > frcpt.Y)
        //    {
        //        return new IntPoint(tocpt.X + 2 * CConstants.dblVerySmallCoordClipper, tocpt.Y);
        //    }
        //    else if (tocpt.Y < frcpt.Y)
        //    {
        //        return new IntPoint(tocpt.X - 2 * CConstants.dblVerySmallCoordClipper, tocpt.Y);
        //    }
        //    else // if (tocpt.Y == frcpt.Y)
        //    {
        //        if (tocpt.X > frcpt.X)
        //        {
        //            return new IntPoint(tocpt.X, tocpt.Y - 2 * CConstants.dblVerySmallCoordClipper);
        //        }
        //        else if (tocpt.Y < frcpt.Y)
        //        {
        //            return new IntPoint(tocpt.X, tocpt.Y + 2 * CConstants.dblVerySmallCoordClipper);
        //        }
        //        else
        //        {
        //            throw new ArgumentException("frcpt and tocpt are identical!");
        //        }
        //    }
        //}




        //public static Path GeneratePathByCptEbFromCpg(IEnumerable<CPoint> cpteb, bool blnReverse = false)
        //{



        //    return GenerateIntptEbByCptEb(cpteb.Reverse()).ToList();

        //    //return GenerateIntptEbByCptEb(cpteb).ToList();
        //}

        


        #region ScaleGeos
        public static IEnumerable<CPolygon> ScaleCpgLt(IEnumerable<CPolygon> cpglt, double dblFactor)
        {
            foreach (var cpg in cpglt)
            {
                var scaledcpg = new CPolygon(cpg.ID, ScaleCptEb(cpg.CptLt, dblFactor).ToList());

                //scale the holes of cpg
                var cpgPairSK = new Stack<CValPair<CPolygon, CPolygon>>();
                cpgPairSK.Push(new CValPair<CPolygon, CPolygon>(cpg, scaledcpg));
                do
                {
                    var cpggroup = cpgPairSK.Pop();
                    var recursivecpg = cpggroup.val1;
                    var scaledrecursivecpg = cpggroup.val2;
                    if (recursivecpg.HoleCpgLt != null && recursivecpg.HoleCpgLt.Count > 0)
                    {
                        scaledrecursivecpg.HoleCpgLt = new List<CPolygon>(recursivecpg.HoleCpgLt.Count);
                        foreach (var holecpg in recursivecpg.HoleCpgLt)
                        {
                            var scaledholecpg = new CPolygon(holecpg.ID, ScaleCptEb(holecpg.CptLt, dblFactor).ToList());
                            scaledholecpg.IsHole = true;
                            scaledrecursivecpg.HoleCpgLt.Add(scaledholecpg);
                            //scaledholecpg.ParentCpg = scaledrecursivecpg;

                            cpgPairSK.Push(new CValPair<CPolygon, CPolygon>(holecpg, scaledholecpg));
                        }
                    }

                } while (cpgPairSK.Count > 0);

                yield return scaledcpg;
            }
        }

        public static IEnumerable<CEdge> ScaleCEdgeEb(IEnumerable<CEdge> cedgeeb, double dblFactor)
        {
            foreach (var cedge in cedgeeb)
            {
                yield return new CEdge(ScaleCpt(cedge.FrCpt, dblFactor), ScaleCpt(cedge.ToCpt, dblFactor));
            }
        }

        public static IEnumerable<IEnumerable<CPoint>> ScaleCptEbEb(IEnumerable<IEnumerable<CPoint>> cptebeb, double dblFactor)
        {
            foreach (var cpteb in cptebeb)
            {
                yield return ScaleCptEb(cpteb, dblFactor);
            }
        }

        public static IEnumerable<CPoint> ScaleCptEb(IEnumerable<CPoint> cpteb, double dblFactor)
        {
            foreach (var cpt in cpteb)
            {
                yield return ScaleCpt(cpt, dblFactor);
            }
        }

        public static CPoint ScaleCpt(CPoint cpt, double dblFactor)
        {
            return new CPoint(cpt.ID, cpt.X * dblFactor, cpt.Y * dblFactor);
        }
        #endregion

        public static IEnumerable<CEdge> GenerateCEdgeEbByPathsCptEbEb(IEnumerable<IEnumerable<CPoint>> pathsCptEbEb)
        {
            foreach (var cptEb in pathsCptEbEb)
            {
                foreach (var cedge in CGeoFunc.FormCEdgeEb(cptEb, true))
                {
                    yield return cedge;
                }
            }
        }


        //public IEnumerable<CEdge> GenerateCEdgeEbByPaths(Paths paths)
        //{
        //    foreach (var path in paths)
        //    {
        //        foreach (var cedge in CGeoFunc.FormCEdgeEb(cptEb, false))
        //        {
        //            yield return cedge;
        //        }
        //    }
        //}

        public static IEnumerable<CPolyline> GenerateCplEbByPathCptEbEb(IEnumerable<IEnumerable<CPoint>> pathCptEbEb)
        {
            int intCount = 0;
            foreach (var cpteb in pathCptEbEb)
            {
                var cptlt = cpteb.ToList();
                cptlt.Add(new CPoint(cptlt.Count, cptlt[0].X, cptlt[0].Y));
                yield return new CPolyline(intCount++, cptlt);
            }
        }

    }
}
