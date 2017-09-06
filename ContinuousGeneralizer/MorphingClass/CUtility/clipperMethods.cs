using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;

using MorphingClass.CAid;
using MorphingClass.CEntity;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeneralizationMethods;

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
            ClipperOffset pClipperOffset = new ClipperOffset(dblMiterLimit, 0.25 * CConstants.dblFclipper);
            switch (strBufferStyle)
            {
                case "Miter":
                    //Property MiterLimit sets the maximum distance in multiples of delta that vertices can be offset from 
                    //their original positions before squaring is applied. (Squaring truncates a miter by 'cutting it off' 
                    //at 1 × delta distance from the original vertex.)
                    pClipperOffset.AddPaths(inputpaths, JoinType.jtMiter, EndType.etClosedPolygon);
                    break;
                case "Round":
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
            ClipperOffset pClipperOffset = new ClipperOffset(dblMiterLimit, 0.25 * CConstants.dblFclipper);
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


        private static List<List<CPolygon>> GroupCpgsByOverlap(List<CPolygon> cpglt, PolyTree polyTree)
        {
            var GroupedCpgLtLt = new List<List<CPolygon>>(polyTree.ChildCount);
            var testCpgLt = cpglt;
            
            foreach (var nodePaths in GetOneLevelPathsEbFromPolyTree(polyTree))
            {
                //overlap
                var remainCpgLt = new List<CPolygon>(testCpgLt.Count);
                var groupCpglt = new List<CPolygon>();
                foreach (var testCpg in testCpgLt)
                {
                    var clippedPaths = Clip_Paths(nodePaths, true,
                        CHelpFunc.MakeLt(testCpg.ExteriorPath), true, ClipType.ctIntersection);
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


        private static IEnumerable<PolyNode> GetOneLevelPolyNode(PolyTree polyTree)
        {
            var QueueNode = new Queue<PolyNode>(polyTree.Childs);

            while (QueueNode.Count > 0)
            {
                var polynode = QueueNode.Dequeue();
                yield return polynode;


                //do it recursively
                if (polynode.ChildCount > 0)
                {
                    foreach (var holepolynode in polynode.Childs)
                    {
                        if (holepolynode.ChildCount > 0)
                        {
                            foreach (var exteriorpolynode in holepolynode.Childs)
                            {
                                QueueNode.Enqueue(exteriorpolynode);
                            }
                        }
                    }
                }
            }
        }


        private static IEnumerable<Paths> GetOneLevelPathsEbFromPolyTree(PolyTree polyTree)
        {
            foreach (var polynode in GetOneLevelPolyNode(polyTree))
            {
                yield return GetPathsFromOnelevelPolyNode(polynode);
            }
        }

        private static Paths GetPathsFromOnelevelPolyNode(PolyNode polynode)
        {
            var nodePaths = new Paths { polynode.Contour };
            if (polynode.ChildCount > 0)
            {
                foreach (var holepolynode in polynode.Childs)
                {
                    nodePaths.Add(holepolynode.Contour);
                }
            }
            return nodePaths;
        }


        public static List<List<CPolygon>> IterativelyGroupCpgsByOverlapIndependently(List<CPolygon> cpglt,
double dblGrow, double dblDilation, double dblEpsilon, string strBufferStyle = "Miter", double dblMiterLimit = 2)
        {
            //var cpglyDilationPolyTree = IterativelyGetPolyTreeForGroupedCpgs
            //    (cpglt, dblGrow, dblDilation, dblEpsilon, strBufferStyle, dblMiterLimit);
            //var groupedcpgltlt = GroupCpgsByOverlap(cpglt, cpglyDilationPolyTree);

            var groupedcpgltlt= IterativelyGetPolyTreeForGroupedCpgs
                (cpglt, dblGrow, dblDilation, dblEpsilon, strBufferStyle, dblMiterLimit);

            int intCpgCount = 0;
            groupedcpgltlt.ForEach(groupedcpglt => intCpgCount += groupedcpglt.Count);

            if (intCpgCount != cpglt.Count)
            {
                throw new ArgumentException("some polygons are lost!");
            }

            return groupedcpgltlt;
        }


        private static List<List<CPolygon>> IterativelyGetPolyTreeForGroupedCpgs(List<CPolygon> cpglt,
double dblGrow, double dblDilation, double dblEpsilon, string strBufferStyle = "Miter", double dblMiterLimit = 2)
        {
            double dblDisOverlap = Math.Sqrt(5) * dblEpsilon / 2;
            var groupedcpgltlt = new List<List<CPolygon>>(cpglt.Count);
            foreach (var cpg in cpglt)
            {
                groupedcpgltlt.Add(new List<CGeometry.CPolygon> { cpg });
            }

            int intGroupCount = 0;
            int intRound = 0;
            do
            {
                intGroupCount = groupedcpgltlt.Count;
                var alloverdilationPaths = new Paths();
                var allbackpaths = new Paths();
                var alloverlapGrownPaths = new Paths();
                foreach (var groupedcpglt in groupedcpgltlt)
                {
                    var groupedoriginalpaths = new Paths(groupedcpglt.Count);
                    foreach (var cpg in groupedcpglt)
                    {
                        groupedoriginalpaths.AddRange(cpg.GetAllPaths());
                    }
                    var overdilationPaths = Offset_Paths(groupedoriginalpaths, dblGrow + dblDilation, strBufferStyle, dblMiterLimit);
                    var backPaths = Offset_Paths(overdilationPaths, -dblDilation, strBufferStyle, dblMiterLimit);
                    var overlapgrownpaths = Offset_Paths(backPaths, dblDisOverlap, "Round");

                    alloverdilationPaths.AddRange(overdilationPaths);
                    allbackpaths.AddRange(backPaths);
                    alloverlapGrownPaths.AddRange(overlapgrownpaths);
                }
                var finalPolyTree = Offset_PolyTree(alloverlapGrownPaths, 0, strBufferStyle, dblMiterLimit);
                groupedcpgltlt = GroupCpgsByOverlap(cpglt, finalPolyTree);

                //CSaveFeature.SavePathEbAsCpgEb(alloverdilationPaths,
                //    intRound + "_alloverdilationPaths_" + (dblGrow + dblDilation) / CConstants.dblFclipper + "m",
                //    pesriSimpleFillStyle: esriSimpleFillStyle.esriSFSNull, blnVisible: false);
                //CSaveFeature.SavePathEbAsCpgEb(allbackpaths, 
                //    intRound + "backPaths" + dblGrow / CConstants.dblFclipper + "m",
                //    pesriSimpleFillStyle: esriSimpleFillStyle.esriSFSNull, blnVisible: false);
                //CSaveFeature.SavePathEbAsCpgEb(alloverlapGrownPaths, 
                //    intRound + "_alloverlapGrownPaths" + (dblGrow + dblDisOverlap) / CConstants.dblFclipper + "m",
                //    pesriSimpleFillStyle: esriSimpleFillStyle.esriSFSNull, blnVisible: false);

                intRound++;
            } while (groupedcpgltlt.Count < intGroupCount);


            return groupedcpgltlt;
        }

        public static Paths ClipOneComponent_BufferDilateErode_Paths(CPolygon cpg,
            double dblGrow, double dblDilation, double dblErosion,
            Paths clipPaths, double dblFclipper, string strBufferStyle = "Miter", double dblMiterLimit = 2)
        {
            var ExteriorOffsetPaths = clipperMethods.DilateErodeOffsetCpgExterior_Paths
    (cpg, dblGrow, dblDilation, dblErosion, strBufferStyle, dblMiterLimit);
            var clippedPaths = clipperMethods.Clip_Paths(ExteriorOffsetPaths, true, clipPaths, true, ClipType.ctIntersection);

            return clippedPaths;
        }


        public static IEnumerable<CPolygon> DilateErodeOffsetSimplifyOneComponent(CPolygon cpg,
            double dblGrow, double dblDilation, double dblErosion, double dblEpsilon,
            string strSimplification, bool blnReverse, string strBufferStyle = "Miter", double dblMiterLimit = 2)
        {
            cpg.JudgeAndFormCEdgeLt();
            //cpg.FormCEdgeLt();

            var ExteriorOffsetPolyTree = clipperMethods.DilateErodeOffsetCpgExterior_PolyTree
                (cpg, dblGrow, dblDilation, dblErosion, strBufferStyle, dblMiterLimit);

            foreach (var OffsetCpg in clipperMethods.GenerateCpgEbByPolyTree(ExteriorOffsetPolyTree, cpg.ID, blnReverse))
            {
                var simplifiedcpg = CDPSimplify.SimplifyCpgAccordExistEdges(OffsetCpg,
                    cpg.CEdgeLt, strSimplification, 1.5 * dblEpsilon);
                //var simplifiedcpg = OffsetCpg;
                yield return simplifiedcpg;
            }
        }



        public static Paths DilateErodeOffsetCpgExterior_Paths(CPolygon Cpg, double dblGrow, double dblDilation, double dblErosion,
    string strBufferStyle, double dblMiterLimit)
        {
            double dblHoleIndicator = 1;
            if (Cpg.IsHole == true)
            {
                dblHoleIndicator = -1;
            }

            if (Cpg.ExteriorPath == null)
            {
                Cpg.SetExteriorPath();
            }
            var overdilationPaths = Offset_Paths(CHelpFunc.MakeLt(Cpg.ExteriorPath),
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



            //CSaveFeature.SaveCpgEb(clipperMethods.ScaleCpgLt(CHelpFunc.MakeLt(Cpg), 1 / CConstants.dblFclipper), 
            //    "Cpg");
            ////            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
            ////growndilationPaths, true), 1 / CConstants.dblFclipper), "growndilationPaths", CConstants.ParameterInitialize);
            //CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(overdilationPaths, true), 
            //    1 / CConstants.dblFclipper), "overdilationPaths");
            ////            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
            ////backPaths, true), 1 / CConstants.dblFclipper), "backPaths", CConstants.ParameterInitialize);
            //CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(erosionpaths, true), 
            //    1 / CConstants.dblFclipper), "erosionpaths");


            return Offset_Paths(erosionpaths, dblHoleIndicator * dblErosion, strBufferStyle, dblMiterLimit);




        }

        public static PolyTree DilateErodeOffsetCpgExterior_PolyTree(CPolygon Cpg,
    double dblGrow, double dblDilation, double dblErosion, string strBufferStyle, double dblMiterLimit)
        {
            //dblDilation = dblDilation / 2;
            //dblErosion = dblGrow;
            //dblGrow = 0;
            //dblErosion = dblDilation;

            double dblHoleIndicator = 1;
            if (Cpg.IsHole == true)
            {
                dblHoleIndicator = -1;
            }

            if (Cpg.ExteriorPath == null)
            {
                Cpg.SetExteriorPath();
            }
            var overdilationPaths = Offset_Paths(CHelpFunc.MakeLt(Cpg.ExteriorPath),
                    dblHoleIndicator * (dblGrow + dblDilation), strBufferStyle, dblMiterLimit);
            var erosionpaths = Offset_Paths(overdilationPaths,
                    dblHoleIndicator * (-dblDilation - dblErosion), strBufferStyle, dblMiterLimit);
            var normalPolyTree = Offset_PolyTree(erosionpaths, dblHoleIndicator * dblErosion, strBufferStyle, dblMiterLimit);

            ////CSaveFeature.SaveCpgEb(clipperMethods.ScaleCpgEb(CHelpFunc.MakeLt(Cpg), 1 / CConstants.dblFclipper),
            ////    "Cpg" + CHelpFunc.GetTimeStampWithPrefix());
            //////            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
            //////growndilationPaths, true), 1 / CConstants.dblFclipper), "growndilationPaths", CConstants.ParameterInitialize);
            //CSaveFeature.SaveCplEb(clipperMethods.ScaleCplEb(clipperMethods.ConvertPathsToCplEb(overdilationPaths, true, true),
            //    1 / CConstants.dblFclipper), "overdilationPaths", blnVisible: false);
            ////            CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(clipperMethods.ConvertPathsToCEdgeLt(
            ////backPaths, true), 1 / CConstants.dblFclipper), "backPaths", CConstants.ParameterInitialize);
            //CSaveFeature.SaveCplEb(clipperMethods.ScaleCplEb(clipperMethods.ConvertPathsToCplEb(erosionpaths, true, true),
            //    1 / CConstants.dblFclipper), "erosionpaths", blnVisible: false);

            //CSaveFeature.SaveCplEb(clipperMethods.ScaleCplEb(clipperMethods.ConvertPathsToCplEb(
            //    Clipper.PolyTreeToPaths(normalPolyTree), true, true), 1 / CConstants.dblFclipper),
            //    "normalpaths", blnVisible: false);

            return normalPolyTree;
        }

        ///// <summary>
        ///// OLH: One Level Hole; we assume that there is only one polygon in pPolyTree
        ///// </summary>
        ///// <param name="pPolyTree"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //public static CPolygon GenerateOLHCpgByPolyTree(PolyTree pPolyTree, int intID)
        //{
        //    if (pPolyTree.ChildCount > 1)
        //    {
        //        throw new ArgumentOutOfRangeException("there should be no more than 1 polygon!");
        //    }

        //    return GenerateOLHCpgEbByPolyNode(pPolyTree.Childs[0], intID);
        //}



        public static IEnumerable<CPolygon> GenerateCpgEbByPolyTree(PolyTree polyTree, int intID, bool blnReverse = false)
        {
            foreach (var polynode in GetOneLevelPolyNode(polyTree))
            {
                yield return GenerateOLHCpgEbByPolyNode(polynode, intID, blnReverse);
            }
        }

        //private static CPolygon GenerateCpgEbByPolyNode(PolyNode cpgnode, int intID, bool blnReverse = false)
        //{
        //    var cptlt = ContourToCptEb(cpgnode.Contour, blnReverse).ToList();
        //    var holecptlt = GetOLHCptLtEb(cpgnode, blnReverse).ToLtLt();

        //    return new CPolygon(intID, cptlt, holecptlt);
        //}


        //public static IEnumerable<CPolygon> GenerateOLHCpgEbByPolyTree(PolyTree pPolyTree, int intID, bool blnReverse = false)
        //{
        //    foreach (var polyNode in pPolyTree.Childs)
        //    {
        //        yield return GenerateOLHCpgEbByPolyNode(polyNode, intID, blnReverse);
        //    }
        //}

        private static CPolygon GenerateOLHCpgEbByPolyNode(PolyNode cpgnode, int intID, bool blnReverse = false)
        {
            //CSaveFeature.SavePathEb(CHelpFunc.MakeLt(cpgnode.Contour), "OLHCpg");

            var cptlt = ConvertPathToCptEb(cpgnode.Contour, blnReverse).ToList();
            var holecptlt = GetOLHCptLtEb(cpgnode, blnReverse).ToLtLt();

            var cpg= new CPolygon(intID, cptlt, holecptlt);
            //CSaveFeature.SaveCpgEb(clipperMethods.ScaleCpgEb(CHelpFunc.MakeLt(cpg), 1 / CConstants.dblFclipper), "Cpg");

            return cpg;
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpgNode"></param>
        /// <param name="blnMakeIdentical"></param>
        /// <returns></returns>
        /// <remarks>OLH: One Level Hole</remarks>
        private static IEnumerable<IEnumerable<CPoint>> GetOLHCptLtEb(PolyNode cpgNode, bool blnReverse = false)
        {
            foreach (var holenode in cpgNode.Childs)
            {
                yield return ConvertPathToCptEb(holenode.Contour, blnReverse);
            }
        }

        public static IEnumerable<CPolyline> ConvertPathsToCplEb(IEnumerable<Path> PathEb, 
            bool blnReverse = false, bool AddFirstcptAsLastcpg = false)
        {
            int intID = 0;
            foreach (var path in PathEb)
            {
               yield return new CPolyline(intID++, ConvertPathToCptEb(path, blnReverse, AddFirstcptAsLastcpg).ToList());
            }
        }

        public static IEnumerable<CPolygon> ConvertPathsToCpgEb(IEnumerable<Path> PathEb, 
            bool blnReverse = false, bool AddFirstcptAsLastcpg = false)
        {
            int intID = 0;
            foreach (var path in PathEb)
            {
                yield return new CPolygon(intID++, ConvertPathToCptEb(path, blnReverse, AddFirstcptAsLastcpg).ToList());
            }
        }

        public static IEnumerable<CEdge> ConvertPathsToCEdgeEb(Paths Paths, bool blnReverse = false, bool AddFirstcptAsLastcpg = false)
        {
            foreach (var path in Paths)
            {
                foreach (var cedge in ConvertPathToCEdgeEb(path, blnReverse, AddFirstcptAsLastcpg))
                {
                    yield return cedge;
                }                
            }
        }


        public static IEnumerable<CEdge> ConvertPathToCEdgeEb(Path path, bool blnReverse = false, bool AddFirstcptAsLastcpg = false)
        {
            return CGeoFunc.FormCEdgeEb(ConvertPathToCptEb(path, blnReverse, AddFirstcptAsLastcpg));
        }

        /// <summary>we do reverse because the direction of contour is different from the direction of our Cpg</summary>
        public static IEnumerable<CPoint> ConvertPathToCptEb(Path path, bool blnReverse = false, bool AddFirstcptAsLastcpg = true)
        {
            var newpath = path;
            if (blnReverse == true)
            {
                newpath= Enumerable.Reverse(path).ToList();
            }

            int intID = 0;
            foreach (var intpt in newpath)
            {
                yield return new CPoint(intID++, intpt.X, intpt.Y);
            }

            if (AddFirstcptAsLastcpg == true)
            {
                yield return new CPoint(intID, newpath[0].X, newpath[0].Y);
            }
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

        //public static Path GeneratePathByCpgExterior(CPolygon cpg)
        //{
        //    return GeneratePathByCptEb(cpg.CptLt, true);
        //}

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


        #region ScaleGeos
        public static IEnumerable<CPolygon> ScaleCpgEb(IEnumerable<CPolygon> cpgeb, double dblFactor)
        {
            foreach (var cpg in cpgeb)
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

        public static IEnumerable<CPolyline> ScaleCplEb(IEnumerable<CPolyline> cpleb, double dblFactor)
        {
            foreach (var cpl in cpleb)
            {
                yield return new CPolyline(cpl.ID, ScaleCptEb(cpl.CptLt, dblFactor).ToList());
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
