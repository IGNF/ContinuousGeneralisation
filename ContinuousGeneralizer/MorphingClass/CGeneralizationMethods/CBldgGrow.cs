using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

using MorphingClass.CAid;
using MorphingClass.CEntity;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;


using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Maplex;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.CartographyTools;

using ClipperLib;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace MorphingClass.CGeneralizationMethods
{
    /// <summary>
    /// Building Growing
    /// </summary>
    /// <remarks>Cpipe: CpgPairIncrCPtEdgeDis</remarks>
    public class CBldgGrow : CMorphingBaseCpg
    {
        public List<CPolygon> MergedCpgLt { get; set; }

        private double _intRound = 0;
        private static int _intI = 3;
        private double _dblTotalGrow;
        //private double _dblTargetDepsilon;
        private double _dblEpsilon;
        private double _dblDilation;
        private double _dblErosion;
        //private double _dblSimplifyEpsilon;
        private double _dblLambda;

        private static double _dblStartScale;
        private static double _dblTargetScale;
        

        private static double _dblAreaLimit;
        private static double _dblHoleAreaLimit;
        

        private static int _intStart = 0;
        private static int _intEnd = _intStart + 1;

        private Dictionary<CValPairIncr<CPolygon>, CptEdgeDis> _CloseCpipeDt;

        public CBldgGrow()
        {

        }

        public CBldgGrow(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolygon, CPolygon>(ParameterInitialize, 1, 0, blnCreateFileGdbWorkspace: false);
        }


        public void BldgGrow(string strBufferStyle, double dblMiterLimit, string strSimplification,
            double dblLS, double dblSS, int intOutputMapCount)
        {

            //**********************************************//
            //I may need to do buffering based on Miterjoint in a more clever way
            //if the distance from the miter point to original line is larger than dblMiterLimit*dblBufferRadius, we, instead of calling
            //the normal square method, make a square so that the farthest distance to the original line exactly dblMiterLimit*dblBufferRadius
            
            var pParameterInitialize = _ParameterInitialize;

            var LSCpgLt = this.ObjCGeoLtLt[0].AsExpectedClass<CPolygon, object>().ToList();
            //int intEdgeCount = 0;
            //double dblArea = 0;
            //foreach (var cpg in LSCpgLt)
            //{
            //    intEdgeCount += cpg.GetEdgeCount();
            //    cpg.SetAreaSimple();
            //    dblArea += cpg.dblAreaSimple;
            //}




            double dblStartScale = dblLS;
            double dblFclipper = CConstants.dblFclipper;

            var MagnifiedCpgLt = clipperMethods.ScaleCpgEb(LSCpgLt, dblFclipper).ToList();
            CConstants.dblVerySmallCoord *= dblFclipper;
            int intStart = 0;
            int intEnd = intStart + intOutputMapCount;
            CHelpFunc.Displaytspb(0.5, intEnd - intStart);
            double dblTargetScale = 0;
            var pStopwatch = Stopwatch.StartNew();
            for (int i = intStart; i < intEnd; i++)
            {
                //double dblTargetScale = dblStartScale + (i + 1) * 5;
                switch (i)
                {
                    case 0:
                        dblTargetScale = 50;
                        //dblTargetScale = 25;
                        break;
                    case 1:
                        dblTargetScale = 100;
                        //dblTargetScale = 50;
                        break;
                    case 2:
                        dblTargetScale = 250;
                        //dblTargetScale = 100;
                        break;
                    case 3:
                        dblTargetScale = 500;
                        //CConstants.blnStop = true;
                        //dblTargetScale = 250;
                        _intI = 3;
                        break;
                    case 4:
                        dblTargetScale = 1000;
                        //CConstants.blnStop = true;
                        //dblTargetScale = 500;
                        _intI = 3;
                        break;
                    default:
                        break;
                }

                _dblStartScale = dblStartScale;
                _dblTargetScale = dblTargetScale;


                //_dblAreaLimit = 4; //area limit 0.16 mm^2
                _dblAreaLimit = 0.16; //area limit 0.16 mm^2
                //double dblAreaLimitTargetScale= dblAreaLimit * dblTargetScale * dblTargetScale;

                _dblEpsilon = 0.2;
                double dblTargetEpsilon = _dblEpsilon * dblTargetScale * dblFclipper;


                //_dblLambda = 0.8;
                //_dblTotalGrow = _dblLambda / 2 * Math.Sqrt(_dblAreaLimit) * (dblTargetScale - dblStartScale);
                
                _dblTotalGrow = 25;
                //_dblTotalGrow *= 50;
                double dblTotalGrow = _dblTotalGrow * dblFclipper;


                _dblHoleAreaLimit = 8; //dblHoleAreaLimit=8 mm^2 * dblTargetScale * dblTargetScale
                double dblHoleAreaLimitTargetScale = _dblHoleAreaLimit * dblTargetScale * dblTargetScale * dblFclipper * dblFclipper;

                //_dblErosion = _dblTotalGrow / 2;
                _dblErosion = 1.5* _dblEpsilon / 2; //To avoid breaking the polygon when we errod, dblOverDilated should not be too large 
                double dblTargetErosion = _dblErosion * dblTargetScale * dblFclipper;
                //dblTargetErosion = 0;
                //dblTargetErosion = dblTotalGrow / 2;

                //_dblDilation should be larger than dblEpsilon * Math.Sqrt(5) / 2 + 4 * dblGrow, because of function GroupAndGetShortestCpipeSD_Overlap
                //_dblDilation = Math.Sqrt(_dblHoleAreaLimit / Math.PI);
                //_dblDilation = _dblEpsilon / 2;
                double dblTargetDilation = (dblTotalGrow - dblMiterLimit * dblTargetErosion) / (dblMiterLimit - 1);
                //dblTargetDilation = 0;
                //double dblTargetDilation = dblTotalGrow;
                //dblTargetDilation= dblTotalGrow / 2;
                
                foreach (var MagnifiedCpg in MagnifiedCpgLt)
                {
                    MagnifiedCpg.RemoveClosePoints();
                    //MagnifiedCpg.SubCpgLt = new List<CPolygon> { MagnifiedCpg };
                    MagnifiedCpg.FormCEdgeLt();
                    MagnifiedCpg.CEdgeLt.ForEach(cedge => cedge.BelongedCpg = MagnifiedCpg);
                    MagnifiedCpg.SetExteriorPath();
                    //MagnifiedCpg. = CHelpFunc.MakeLt(clipperMethods.GeneratePathByCpgExterior(MagnifiedCpg));
                }





                #region Generate buffers (commented)
                //var allpaths = new Paths();
                //foreach (var MagnifiedCpg in MagnifiedCpgLt)
                //{
                //    allpaths.AddRange(MagnifiedCpg.GetAllPaths());
                //}
                //var offsetpaths = clipperMethods.Offset_Paths(allpaths,
                //dblTotalGrow, strBufferStyle, dblMiterLimit);

                //CSaveFeature.SavePathEbAsCpgEb(offsetpaths, "offsetpaths");
                #endregion

                //var mergedCpgLt = MergeCloseCpgsAndAddBridges(MagnifiedCpgLt, dblTotalGrow, dblTargetDilation, dblTargetEpsilon);
                var mergedCpgLt = MergeCloseCpgsAndAddBridges(MagnifiedCpgLt,true,
                    dblTotalGrow, dblTargetDilation, dblTargetEpsilon,strBufferStyle, dblMiterLimit);

                //dblGrow, dblDilation, dblErosion, dblEpsilon


                for (int j = 0; j < mergedCpgLt.Count; j++)
                {
                    var mergedcpg = mergedCpgLt[j];

                    SetClipCpgLt_BufferDilateErodeSimplify(mergedcpg,
                        dblTotalGrow, dblTargetDilation, dblTargetErosion, dblTargetEpsilon,
                        dblHoleAreaLimitTargetScale, strSimplification, strBufferStyle, dblMiterLimit);

                    //if (mergedcpg.BridgeCptEdgeDisLt != null)
                    //{
                    //    var BridgeCpipeDt = new Dictionary<CValPairIncr<CPolygon>, CptEdgeDis>
                    //(mergedcpg.BridgeCptEdgeDisLt.Count, new CCmpEqCpgPairIncr());
                    //    foreach (var BridgeCptEdgeDis in mergedcpg.BridgeCptEdgeDisLt)
                    //    {
                    //        BridgeCpipeDt.Add(new CValPairIncr<CPolygon>(BridgeCptEdgeDis.FrCEdge.BelongedCpg,
                    //            BridgeCptEdgeDis.ToCEdge.BelongedCpg), BridgeCptEdgeDis);
                    //    }
                    //    mergedcpg.BridgeCpipeDt = BridgeCpipeDt;
                    //}


                    mergedcpg.LastTimePaths = new Paths();
                    if (mergedcpg.SubCpgLt == null || mergedcpg.SubCpgLt.Count < 2)
                    {
                        mergedcpg.LastTimePaths.AddRange(mergedcpg.GetAllPaths());
                    }
                    else
                    {
                        foreach (var subcpg in mergedcpg.SubCpgLt)
                        {
                            mergedcpg.LastTimePaths.AddRange(subcpg.GetAllPaths());
                        }
                    }

                    dblStartScale = dblTargetScale;
                    CHelpFunc.Displaytspb(j + 1, mergedCpgLt.Count);
                }

                int intEdgecountbeforesimplification = CDPSimplify._intEdgeCountBefore;
                int intEdgecountaftersimplification = CDPSimplify._intEdgeCountAfter;
                CDPSimplify._intEdgeCountBefore = 0;
                CDPSimplify._intEdgeCountAfter = 0;
                CHelpFunc.DisplayRunTime(pStopwatch.ElapsedMilliseconds);
                

                this.MergedCpgLt = mergedCpgLt;

                var clipcpgEb = GetClipCpgEb(mergedCpgLt);
                CSaveFeature.SaveCpgEb(clipperMethods.ScaleCpgEb(clipcpgEb, 1 / dblFclipper),
    strSimplification + "_" + dblTargetScale + "k_ClipForm" + dblTargetEpsilon + "m", intOutlineRed: 255,  
    pesriSimpleFillStyle: esriSimpleFillStyle.esriSFSHollow, blnVisible: false);

                CSaveFeature.SaveCpgEb(clipperMethods.ScaleCpgEb(mergedCpgLt, 1 / dblFclipper),
    strSimplification + "_" + dblTargetScale + "k_BridgedCpg" + dblTargetEpsilon + "m_",
    pesriSimpleFillStyle: esriSimpleFillStyle.esriSFSHollow, blnVisible: false);

                #region Geoprocessor Samples
                //// Initialize the geoprocessor. 
                ////IGeoProcessor2 gp2 = new GeoProcessorClass();
                //Geoprocessor gp = gp = new Geoprocessor();


                ////ESRI.ArcGIS.AnalysisTools.Buffer pBuffer = new ESRI.ArcGIS.AnalysisTools.Buffer(
                ////    pParameterInitialize.strSavePathBackSlash + lyroverbuffer.Name,
                ////    pParameterInitialize.strSavePathBackSlash + lyroverbuffer.Name + "_Buffered", 5);

                ////pBuffer.in_features = pParameterInitialize.m_mapControl.Layer[0] as IDataset;


                //AggregatePolygons pAggregatePolygons = new AggregatePolygons();
                //pAggregatePolygons.in_features = pParameterInitialize.m_mapControl.Layer[0] as IDataset;
                //pAggregatePolygons.out_feature_class = pParameterInitialize.pWorkspace.PathName + "\\" + "My_Agg";
                ////strReturnName = pWorkspace.PathName + "\\" + pDataset.Name + "_Agg"; ;

                //pAggregatePolygons.orthogonality_option = "ORTHOGONAL";
                //pAggregatePolygons.aggregation_distance = "500 Meters";
                ////pAggregatePolygons.
                //gp.Execute(pAggregatePolygons, null);

                ////execute the buffer tool (very easy :-))
                ////gp.Execute(buffer, null);
                ////IGeoProcessorResult results = (IGeoProcessorResult)gp.Execute(buffer, null);

                ////ESRI.ArcGIS.CoverageTools.SimplifyBuilding pSimplifyBuilding = new SimplifyBuilding();
                ////pSimplifyBuilding.in_cover = pParameterInitialize.strSavePathBackSlash + lyroverbuffer.Name;
                ////pSimplifyBuilding.out_cover = pParameterInitialize.strSavePathBackSlash + lyroverbuffer.Name +"_Simplified";
                ////pSimplifyBuilding.simplification_tolerance = dblEpsilon;
                ////pSimplifyBuilding.minimum_area = dblA;
                ////pSimplifyBuilding.CheckConflict = "CHECK_CONFLICT ";

                ////GP.Execute(pSimplifyBuilding,null);
                #endregion

            }

            CConstants.dblVerySmallCoord /= dblFclipper;

        }

        private IEnumerable<CPolygon> GetClipCpgEb(List<CPolygon> mergedCpgLt)
        {
            for (int j = 0; j < mergedCpgLt.Count; j++)
            {
                var mergedcpg = mergedCpgLt[j];

                var targetCpg = new CPolygon(mergedcpg.ID, mergedcpg.ClipCpgLt[0].CptLt);
                if (mergedcpg.ClipCpgLt[0].HoleCpgLt != null)
                {
                    targetCpg.HoleCpgLt = new List<CPolygon>(mergedcpg.ClipCpgLt[0].HoleCpgLt.Count);
                    foreach (var holeclipcpg in mergedcpg.ClipCpgLt[0].HoleCpgLt)
                    {
                        var holecpg = new CPolygon(holeclipcpg.ID, holeclipcpg.CptLt);
                        holecpg.IsHole = true;
                        targetCpg.HoleCpgLt.Add(holecpg);
                    }
                }

                if (mergedcpg.HoleCpgLt != null)
                {
                    if (targetCpg.HoleCpgLt == null)
                    {
                        targetCpg.HoleCpgLt = new List<CPolygon>();
                    }

                    foreach (var holecpg in mergedcpg.HoleCpgLt)
                    {
                        if (holecpg.ClipCpgLt == null)
                        {
                            continue;
                        }

                        foreach (var holeclipcpg in holecpg.ClipCpgLt)
                        {
                            var originalholecpg = new CPolygon(holeclipcpg.ID, holeclipcpg.CptLt);
                            originalholecpg.IsHole = true;
                            targetCpg.HoleCpgLt.Add(originalholecpg);
                        }
                    }
                }                
                yield return targetCpg;
            }
        }



        #region GroupCloseCpgsAndAddBridges single bridges        

        private List<CPolygon> MergeCloseCpgsAndAddBridges(List<CPolygon> cpglt, bool blnGoalMap,
            double dblGrow, double dblDilation, double dblEpsilon, string strBufferStyle = "Miter",
            double dblMiterLimit = 2)
        {            
            var CloseCpipeDt = _CloseCpipeDt;
            if (blnGoalMap == true) //when we want to generate built-up areas at goal map, BridgeCpipeDt == null
            {
                double dblCloseDis = 3 * dblGrow + Math.Sqrt(5) / 2 * dblEpsilon + dblDilation;
                CloseCpipeDt = GetCloseCpgPairPtEdgeDisDt_EdgeRelation(cpglt, dblCloseDis);
                _CloseCpipeDt = CloseCpipeDt;
            }


            var groupedcpgCptEdgeDisltlt = new List<CValPair<List<CPolygon>, List<CptEdgeDis>>>(cpglt.Count);
            foreach (var cpg in cpglt)
            {
                groupedcpgCptEdgeDisltlt.Add(new CValPair<List<CPolygon>, List<CptEdgeDis>>
                    (new List<CPolygon> { cpg }, new List<CptEdgeDis>()));
            }

            int intGroupCount = 0;
            do
            {
                intGroupCount = groupedcpgCptEdgeDisltlt.Count;
                var GroupedCpgLtLt = clipperMethods.MergeGroupedCpgs
                    (cpglt, groupedcpgCptEdgeDisltlt, dblGrow, dblDilation, dblEpsilon, strBufferStyle, dblMiterLimit);

                groupedcpgCptEdgeDisltlt = new List<CValPair<List<CPolygon>, List<CptEdgeDis>>>();
                foreach (var GroupedCpgLt in GroupedCpgLtLt)
                {
                    var CptEdgeDisLt = new List<CptEdgeDis>(GroupedCpgLt.Count);

                    if (GroupedCpgLt.Count > 1)
                    {
                        for (int i = 0; i < GroupedCpgLt.Count - 1; i++)
                        {
                            for (int j = i + 1; j < GroupedCpgLt.Count; j++)
                            {
                                CptEdgeDis outCptEdgeDis;
                                if (CloseCpipeDt.TryGetValue(new CValPairIncr<CPolygon>(GroupedCpgLt[i], GroupedCpgLt[j]),
                                    out outCptEdgeDis))
                                {
                                    CptEdgeDisLt.Add(outCptEdgeDis);
                                }
                            }
                        }

                        CptEdgeDisLt = GroupCpgsAndBridgesForEachCluster_Prim(CptEdgeDisLt).ToList();
                    }

                    groupedcpgCptEdgeDisltlt.Add(new CValPair<List<CPolygon>, List<CptEdgeDis>>(GroupedCpgLt, CptEdgeDisLt));
                }
            } while (groupedcpgCptEdgeDisltlt.Count < intGroupCount);



            var mergedCpgLt = new List<CPolygon>();
            //mergedCpgLt.Add(MergeGroupedCpgsAndBridges(GroupedCptEdgeDisSSLt[11]));
            //add merged polygons
            foreach (var groupedcpgCptEdgeDislt in groupedcpgCptEdgeDisltlt)
            {
                if (groupedcpgCptEdgeDislt.val2.Count > 0)
                {
                    //merge grouped cpgs by DCEL; add SubCpgLt and BridgeCptEdgeDisSS 
                    mergedCpgLt.Add(MergeGroupedCpgsAndBridges(groupedcpgCptEdgeDislt.val2));
                }
                else
                {
                    mergedCpgLt.AddRange(groupedcpgCptEdgeDislt.val1);  //normally, there is only one element
                }
            }
            return mergedCpgLt;
        }
        

        private Dictionary<CValPairIncr<CPolygon>, CptEdgeDis> GetCloseCpgPairPtEdgeDisDt_EdgeRelation(
    List<CPolygon> cpglt, double dblCloseDis)
        {
            var cedgelt = new List<CEdge>();
            cpglt.ForEach(cpg => cedgelt.AddRange(cpg.CEdgeLt));

            var CloseCpipeDt = new Dictionary<CValPairIncr<CPolygon>, CptEdgeDis>(cpglt.Count * 2, new CCmpEqCpgPairIncr());
            foreach (var pEdgeRelation in CGeoFunc.DetectCEdgeRelations(cedgelt, dblCloseDis, true))
            {
                var cpg1 = pEdgeRelation.CEdge1.BelongedCpg;
                var cpg2 = pEdgeRelation.CEdge2.BelongedCpg;
                if (cpg1.GID != cpg2.GID)
                {
                    CptEdgeDis cptEdgeDis;
                    var cpgpair = new CValPairIncr<CPolygon>(cpg1, cpg2);
                    pEdgeRelation.cptEdgeDis.CpgPairIncr = cpgpair;
                    if (CloseCpipeDt.TryGetValue(cpgpair, out cptEdgeDis))
                    {
                        if (pEdgeRelation.cptEdgeDis.dblDis < cptEdgeDis.dblDis)
                        {
                            CloseCpipeDt[cpgpair] = pEdgeRelation.cptEdgeDis;
                        }
                    }
                    else
                    {
                        CloseCpipeDt.Add(cpgpair, pEdgeRelation.cptEdgeDis);
                    }
                }
            }

            foreach (var cptEdgeDis in CloseCpipeDt.Values)
            {
                cptEdgeDis.SetConnectEdge(); //set ConnectEdge for all the bridges
            }

            return CloseCpipeDt;
        }
        
        

        private static HashSet<CptEdgeDis> GroupCpgsAndBridgesForEachCluster_Prim(IEnumerable<CptEdgeDis> CptEdgeDisEb)
        {
            foreach (var cptEdgeDis in CptEdgeDisEb)
            {
                cptEdgeDis.CpgPairIncr.val1.isTraversed = false;
                cptEdgeDis.CpgPairIncr.val2.isTraversed = false;
            }

            var CpgCptEdgeDisLtDt = AttachCptEdgeDisLtToCpg(CptEdgeDisEb);

            int minIndex;
            var mincptEdgeDis = CptEdgeDisEb.GetMin(cptEdgeDis => cptEdgeDis.dblDis, out minIndex);

            var intCpgCount = CpgCptEdgeDisLtDt.Count;
            var FirstCpg = mincptEdgeDis.CpgPairIncr.val1;
            FirstCpg.isTraversed = true;
            var groupcpglt = new List<CPolygon>(intCpgCount);
            groupcpglt.Add(FirstCpg);
            var BridgeCptEdgeDisSS = new HashSet<CptEdgeDis>();
            var QueueSS = new SortedSet<CptEdgeDis>(CCmpCptEdgeDis_Dis.sComparer);
            FindandAddBridgeCptEdgeDisIntoQueue(QueueSS, FirstCpg, CpgCptEdgeDisLtDt);
            do
            {
                //brige polygons by shortest connections, to avoid crossed bridges
                var minCptEdgeDis = QueueSS.Min;
                QueueSS.Remove(minCptEdgeDis);

                var cpg1 = minCptEdgeDis.CpgPairIncr.val1;
                var cpg2 = minCptEdgeDis.CpgPairIncr.val2;
                CPolygon BridgedCpg = null;
                if (cpg1.isTraversed == true && cpg2.isTraversed == true)
                {
                    continue;
                }
                else if (cpg1.isTraversed == false && cpg2.isTraversed == true)
                {
                    BridgedCpg = cpg1;
                }
                else if (cpg1.isTraversed == true && cpg2.isTraversed == false)
                {
                    BridgedCpg = cpg2;
                }
                else
                {
                    throw new ArgumentException("impossible!");
                }
                BridgedCpg.isTraversed = true;
                groupcpglt.Add(BridgedCpg);
                BridgeCptEdgeDisSS.Add(minCptEdgeDis);

                FindandAddBridgeCptEdgeDisIntoQueue(QueueSS, BridgedCpg, CpgCptEdgeDisLtDt);
            } while (QueueSS.Count > 0 && groupcpglt.Count < intCpgCount);


            return BridgeCptEdgeDisSS;
        }

        private static void FindandAddBridgeCptEdgeDisIntoQueue(SortedSet<CptEdgeDis> QueueSS, CPolygon cpg,
            Dictionary<CPolygon, List<CptEdgeDis>> CpgCptEdgeDisLtDt)
        {
            List<CptEdgeDis> outCptEdgeDisLt;
            CpgCptEdgeDisLtDt.TryGetValue(cpg, out outCptEdgeDisLt);

            foreach (var cptEdgeDis in outCptEdgeDisLt)
            {
                QueueSS.Add(cptEdgeDis);
            }
        }

        private static Dictionary<CPolygon, List<CptEdgeDis>> AttachCptEdgeDisLtToCpg(IEnumerable<CptEdgeDis> CptEdgeDisEb)
        {
            var CpgCptEdgeDisLtDt = new Dictionary<CPolygon, List<CptEdgeDis>>();
            foreach (var cptEdgeDis in CptEdgeDisEb)
            {
                //add all close relations to CptEdgeDisLt
                UpdateCpgCptEdgeDisLtDt(ref CpgCptEdgeDisLtDt, cptEdgeDis.CpgPairIncr.val1, cptEdgeDis);
                UpdateCpgCptEdgeDisLtDt(ref CpgCptEdgeDisLtDt, cptEdgeDis.CpgPairIncr.val2, cptEdgeDis);
            }


            return CpgCptEdgeDisLtDt;
        }

        private static void UpdateCpgCptEdgeDisLtDt(
            ref Dictionary<CPolygon, List<CptEdgeDis>> CpgCptEdgeDisLtDt, CPolygon cpg, CptEdgeDis cptEdgeDis)
        {
            if (CpgCptEdgeDisLtDt.ContainsKey(cpg) == false)
            {
                CpgCptEdgeDisLtDt[cpg] = new List<CptEdgeDis>();
            }
            CpgCptEdgeDisLtDt[cpg].Add(cptEdgeDis);
        }

        private CPolygon MergeGroupedCpgsAndBridges(List<CptEdgeDis> BridgeCptEdgeDisLt)
        {
            //we use HashSet so that each cpg will be added once
            var cpghs = new HashSet<CPolygon>();
            foreach (var CptEdgeDis in BridgeCptEdgeDisLt)
            {
                cpghs.Add(CptEdgeDis.CpgPairIncr.val1);
                cpghs.Add(CptEdgeDis.CpgPairIncr.val2);
            }
            var cpglt = cpghs.ToList();
            cpglt.ForEach(cpg => cpg.isTraversed = false);

            //some buildings may share boundaries, so we may have same edge 
            //we use HashSet, instead of List, because we may remove some same edges
            var CEdgeHS = new HashSet<CEdge>(new CCmpEqCEdgeCoord());
            cpglt.ForEach(cpg => cpg.CEdgeLt.ForEach(cedge => CEdgeHS.Add(cedge)));
            CGeoFunc.CheckShortEdges(CEdgeHS);

            var BridgeCEdgeLt = new List<CEdge>(cpglt.Count);
            var AllCEdgeLt = new List<CEdge>(CEdgeHS.Count + cpglt.Count); //the count is roughtly
            var CEdgeCptEdgeDisLtDt = new Dictionary<CEdge, List<CptEdgeDis>>();
            foreach (var cptEdgeDis in BridgeCptEdgeDisLt)
            {
                //if (CCmpMethods.CmpCoordDbl_VerySmall(cptEdgeDis.dblDis, 0) == 0), then the two buildings touch each other
                if (CCmpMethods.CmpCoordDbl_VerySmall(cptEdgeDis.dblDis, 0) != 0)
                {
                    //add the bridge as an edge
                    BridgeCEdgeLt.Add(cptEdgeDis.ConnectCEdge);
                }

                //if the smallest distance of two polygons happen to between a point and an edge
                //this edge can be the closest edge of the polygon to several polygons, 
                //so we will split the edge into several edges
                //here we record all the cases
                if (cptEdgeDis.t != 0 && cptEdgeDis.t != 1)
                {
                    if (CEdgeCptEdgeDisLtDt.ContainsKey(cptEdgeDis.ToCEdge) == false)
                    {
                        CEdgeCptEdgeDisLtDt[cptEdgeDis.ToCEdge] = new List<CptEdgeDis>();
                    }
                    CEdgeCptEdgeDisLtDt[cptEdgeDis.ToCEdge].Add(cptEdgeDis);
                }
            }
            AllCEdgeLt.AddRange(BridgeCEdgeLt);
            //CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(AllCEdgeLt, 1 / CConstants.dblFclipper), 
            //    "BridgeCEdgeLt" + CHelpFunc.GetTimeStampWithPrefix());


            //CGeoFunc.CheckShortEdges(AllCEdgeLt);

            //We need to split the edge into several edges
            foreach (var CEdgeCptEdgeDisLt in CEdgeCptEdgeDisLtDt)
            {
                var TargetCEdge = CEdgeCptEdgeDisLt.Key;
                if (CEdgeHS.Remove(TargetCEdge) == false)
                {
                    throw new ArgumentException("This case should not be possible!");
                }

                AllCEdgeLt.AddRange(GenerateNewCEdgeEb(TargetCEdge, CEdgeCptEdgeDisLt.Value));
            }
            //CGeoFunc.CheckShortEdges(AllCEdgeLt);
            //if (CGeoFunc.ExistDuplicate(AllCEdgeLt, new CCmpCEdgeCoordinates()))
            //{
            //    throw new ArgumentException("There are duplicated edges!");
            //}



            AllCEdgeLt.AddRange(CEdgeHS);


            var holecpglt = new List<CPolygon>();
            foreach (var cpg in cpglt)
            {
                if (cpg.HoleCpgLt != null)
                {
                    holecpglt.AddRange(cpg.HoleCpgLt);
                }
            }


            //CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(AllCEdgeLt, 1 / CConstants.dblFclipper),
            //    "TestEdgeLt" + CHelpFunc.GetTimeStampWithPrefix());


            CDCEL pDCEL = new CGeometry.CDCEL(AllCEdgeLt);
            pDCEL.ConstructDCEL();

            var mergedcpg = new CPolygon(cpglt[0].ID, pDCEL.FaceCpgLt[0].GetOnlyInnerCptLt(), holecpglt);
            mergedcpg.CEdgeLt = pDCEL.FaceCpgLt[0].GetOnlyInnerCEdgeLt();
            mergedcpg.BridgeCptEdgeDisLt = BridgeCptEdgeDisLt;
            mergedcpg.SubCpgLt = cpglt;

            
            return mergedcpg;
        }




        private static IEnumerable<CEdge> GenerateNewCEdgeEb(CEdge TargetCEdge, List<CptEdgeDis> CptEdgeDisLt)
        {
            var cpteb = CGeoFunc.RemoveClosePointsForCptEb(GetOrderedCptEb(TargetCEdge, CptEdgeDisLt));

            var cpter = cpteb.GetEnumerator();
            cpter.MoveNext();
            var precpt = cpter.Current;
            while (cpter.MoveNext())
            {
                if (CCmpMethods.CmpCptXY(precpt, cpter.Current) == 0)
                {
                    throw new ArgumentException("small edge!");
                }

                CEdge newcedge= new CEdge(precpt, cpter.Current);
                newcedge.BelongedCpg = TargetCEdge.BelongedCpg;
                yield return newcedge;
                precpt = cpter.Current;
            }
        }

        private static IEnumerable<CPoint> GetOrderedCptEb(CEdge TargetCEdge, List<CptEdgeDis> CptEdgeDisLt)
        {
            CptEdgeDisLt.Sort(CCmpCptEdgeDis_T.sComparer);

            yield return TargetCEdge.FrCpt;
            foreach (var cptEdgeDis in CptEdgeDisLt)
            {
                yield return cptEdgeDis.ToCpt;
            }
            yield return TargetCEdge.ToCpt;
        }

        #endregion

        private void SetClipCpgLt_BufferDilateErodeSimplify(CPolygon mergedcpg,
            double dblGrow, double dblDilation, double dblErosion, double dblEpsilon,
            double dblHoleAreaLimit, string strSimplification, string strBufferStyle = "Miter", double dblMiterLimit = 2)
        {
            //we also simplify the holes from Exterior ring
            var targetCpgEb = clipperMethods.DilateErodeOffsetSimplifyOneComponent(mergedcpg,
                        dblGrow, dblDilation, dblErosion, dblEpsilon,
                        strSimplification, true, strBufferStyle, dblMiterLimit);
            mergedcpg.ClipCpgLt = targetCpgEb.ToList();
            if (mergedcpg.ClipCpgLt.Count > 1)
            {
                throw new ArgumentException("there should be only one ClipCpg!");
            }

            //if (targetCpg.HoleCpgLt != null)
            //{
            //    mergedcpg.ClipCpgLt.AddRange(targetCpg.HoleCpgLt);  //we should not do this
            //}

            //add original holes, which are stored in mergedcpg
            if (mergedcpg.HoleCpgLt != null && mergedcpg.HoleCpgLt.Count > 0)
            {
                //get the BufferDilateErodeSimplify holes
                //mergedcpg.SetAreaSimple();
                foreach (var holecpg in mergedcpg.HoleCpgLt)
                {
                    //if (holecpg.dblAreaSimple < dblHoleAreaLimit)
                    //{
                    //    continue;
                    //}

                    var holeClipCpgEb = clipperMethods.DilateErodeOffsetSimplifyOneComponent(holecpg,
                        dblGrow, dblDilation, dblErosion, dblEpsilon,
                        strSimplification, false, strBufferStyle, dblMiterLimit);
                    holecpg.ClipCpgLt = holeClipCpgEb.ToList();
                }
            }
        }

        //Stopwatch _pstopwatch = new Stopwatch();
        #region Output
        public void Output(double dblProportion, string strSimplification, string strBufferStyle, double dblMiterLimit)
        {
            CConstants.dblVerySmallCoord *= CConstants. dblFclipper;
            var resultCpgEb = GetResultCpgEb(this.MergedCpgLt, dblProportion, strSimplification, strBufferStyle, dblMiterLimit);
            CSaveFeature.SaveCpgEb(resultCpgEb,
                dblProportion + "_Growing", intBlue: 255, blnVisible: false);
            CConstants.dblVerySmallCoord /= CConstants.dblFclipper;
        }


        public void MakeAnimations(string strSimplification, string strBufferStyle, double dblMiterLimit)
        {
            CConstants.dblVerySmallCoord *= CConstants.dblFclipper;
            int intLayerNum = 10;
            var strLayerNameLt = new List<string>(intLayerNum + 1);
            for (int i = 0; i <= intLayerNum; i++)
            {
                strLayerNameLt.Add(String.Format("{0:0.00}", Convert.ToDouble(i) / intLayerNum));
            }

            string strContent = CIpeDraw.GetDataOfLayerNames(strLayerNameLt);
            strContent += CIpeDraw.GetDataOfViews(strLayerNameLt, false);

            //The Content of animations are obtained here
            strContent += strDataOfLayers(intLayerNum, strLayerNameLt,
                _ParameterInitialize.pFLayerLt[0], _ParameterInitialize.pFLayerLt[0].AreaOfInterest, CConstants.pIpeEnv, "0.05", 
                strSimplification, strBufferStyle, dblMiterLimit);

            string strFullName = _ParameterInitialize.strSavePath + "\\" + CHelpFunc.GetTimeStamp() + ".ipe";
            using (var writer = new System.IO.StreamWriter(strFullName, true))
            {
                writer.Write(CIpeDraw.GenerateIpeContentByDataWithLayerInfo(strContent));
            }

            CConstants.dblVerySmallCoord /= CConstants.dblFclipper;
            System.Diagnostics.Process.Start(@strFullName);
        }

        private string strDataOfLayers(int intLayerNum, List<string> strLayerNameLt, IFeatureLayer pFLayer,
            IEnvelope pFLayerEnv, CEnvelope pIpeEnv, string strBoundWidth, 
            string strSimplification, string strBufferStyle, double dblMiterLimit)
        {
            //for the first layer, we add all the patches
            string strDataAllLayers = CIpeDraw.SpecifyLayerByWritingText(strLayerNameLt[0], "removable", 320, 64);
            strDataAllLayers += CToIpe.GetScaleLegend(pFLayerEnv, pIpeEnv, CHelpFunc.GetUnits(_ParameterInitialize.m_mapControl.MapUnits));
            strDataAllLayers += CIpeDraw.writeIpeText(strLayerNameLt[0], 320, 128)
               + "<group>\n" + CToIpe.GetDataOfFeatureLayer(pFLayer, pFLayerEnv, pIpeEnv, strBoundWidth, true) + "</group>\n";

            //for each of other layers, we only add the new patch
            for (int i = 1; i < strLayerNameLt.Count; i++)
            {
                strDataAllLayers += CIpeDraw.SpecifyLayerByWritingText(strLayerNameLt[i], "removable", 320, 64);

                //draw a rectangle to cover the patch number of the last layer
                strDataAllLayers += CIpeDraw.drawIpeBox(304, 112, 384, 160, "white");

                //add layer name and a text of patch numbers
                strDataAllLayers += CIpeDraw.writeIpeText(strLayerNameLt[i], 320, 128);

                strDataAllLayers += CToIpe.GetScaleLegend(pFLayerEnv, pIpeEnv, 
                    CHelpFunc.GetUnits(_ParameterInitialize.m_mapControl.MapUnits));

                //add the Content of animations
                strDataAllLayers += "<group>\n";
                strDataAllLayers += CIpeDraw.drawIpeEdge(pIpeEnv.XMin, pIpeEnv.YMin, pIpeEnv.XMin, pIpeEnv.YMax, "white");
                foreach (var cpg in GetResultCpgEb(this.MergedCpgLt, Convert.ToDouble(strLayerNameLt[i]), 
                    strSimplification, strBufferStyle, dblMiterLimit))
                {
                    strDataAllLayers += CIpeDraw.DrawCpg(cpg, pFLayerEnv, pIpeEnv,
                        new CUtility.CColor(0, 0, 0), new CUtility.CColor(230, 230, 230), strBoundWidth);
                }
                strDataAllLayers += "</group>\n";
                //strDataAllLayers += CToIpe.TranIpgToIpe(IpgLt[i - 1], pFillSymbolLt[i - 1], pFLayerEnv, pIpeEnv, strBoundWidth);
            }

            return strDataAllLayers;
        }

        private IEnumerable<CPolygon> GetResultCpgEb(List<CPolygon> mergedCpgLt, 
            double dblProportion, string strSimplification, string strBufferStyle, double dblMiterLimit)
        {
            //dblProportion = 0.6;
            CConstants.ParameterInitialize.tspbMain.Value = Convert.ToInt32(dblProportion * 100);


            double dblCurrentScale = _dblStartScale + dblProportion * (_dblTargetScale - _dblStartScale);
            double dblAreaLimitCurrentScale = _dblAreaLimit * dblCurrentScale * dblCurrentScale
                * CConstants.dblFclipper * CConstants.dblFclipper;
            double dblHoleAreaLimitCurrentScale = _dblHoleAreaLimit * dblCurrentScale * dblCurrentScale
                * CConstants.dblFclipper * CConstants.dblFclipper;

            //**********consider that some holes are already too small
            

            var AllGrownAndClippedCpgLt = new List<CPolygon>(mergedCpgLt.Count);
            foreach (var mergedcpg in mergedCpgLt)
            {
                if (mergedcpg.WasTooSmall == true)
                {
                    continue;
                }                

                //there is not SubCpgLt in GrownAndClippedCpg
                var GrownAndClippedCpgLt = GrowAndClipMergedCpg_Overlap(mergedcpg, 
                    dblProportion, strSimplification, strBufferStyle, dblMiterLimit).ToList();
                var dblArea = ComputeCpgEbAreaAndRemoveSmallHoles(GrownAndClippedCpgLt, dblHoleAreaLimitCurrentScale);

                if (dblArea >= dblAreaLimitCurrentScale)
                {
                    AllGrownAndClippedCpgLt.AddRange(GrownAndClippedCpgLt);
                }
                else
                {
                    mergedcpg.WasTooSmall = true;
                }
            }

            var scaledbackcpgLt = clipperMethods.ScaleCpgEb(AllGrownAndClippedCpgLt, 1 / CConstants.dblFclipper).ToList();
            return scaledbackcpgLt;
        }

        private double ComputeCpgEbAreaAndRemoveSmallHoles(IEnumerable<CPolygon> cpgeb, double dblHoleAreaLimit)
        {
            double dblAreaSum = 0;
            foreach (var cpg in cpgeb)
            {
                dblAreaSum += ComputeCpgAreaAndRemoveSmallHoles(cpg, dblHoleAreaLimit);                
            }
            return dblAreaSum;
        }

        private double ComputeCpgAreaAndRemoveSmallHoles(CPolygon cpg, double dblHoleAreaLimit)
        {
            cpg.SetAreaSimple();

            if (cpg.HoleCpgLt != null)
            {
                var holecpglt = new List<CPolygon>(cpg.HoleCpgLt.Count);
                foreach (var holecpg in cpg.HoleCpgLt)
                {
                    if (holecpg.dblAreaSimple >= dblHoleAreaLimit)
                    {
                        holecpglt.Add(holecpg);
                    }
                }
                cpg.HoleCpgLt = holecpglt;
            }
            return cpg.dblAreaSimple;
        }

        private IEnumerable<CPolygon> GrowAndClipMergedCpg_Overlap(CPolygon mergedcpg,
            double dblProportion, string strSimplification, string strBufferStyle, double dblMiterLimit)
        {
            //dblProportion = 0.2;
            //dblProportion = 1;
            double dblCurrentScale = _dblStartScale + dblProportion * (_dblTargetScale - _dblStartScale);
            double dblCurrentGrow = dblProportion * _dblTotalGrow * CConstants.dblFclipper;
            double dblCurrentEpsilon = Math.Min(dblCurrentGrow / dblMiterLimit, _dblEpsilon * dblCurrentScale * CConstants.dblFclipper);
            double dblCurrentErosion = _dblErosion * dblCurrentScale * CConstants.dblFclipper;

            //double dblCurrentDilation = Math.Max(dblCurrentEpsilon / 2, dblProportion * _dblDilation * dblCurrentScale * CConstants.dblFclipper);
            double dblCurrentDilation = (dblCurrentGrow - dblMiterLimit * dblCurrentErosion) / (dblMiterLimit - 1);
            //dblCurrentErosion = 0;
            //dblCurrentDilation = 0;

            var clipPathsFirstLevel = clipperMethods.GenerateClipPathsByCpgEb(mergedcpg.ClipCpgLt);
            var LastAndClippedPath = new Paths();
            if (mergedcpg.SubCpgLt == null || mergedcpg.SubCpgLt.Count < 2)
            {
                LastAndClippedPath.AddRange(DilateErodeOffsetSimplifyCpg(mergedcpg,
                    dblCurrentGrow, dblCurrentDilation, dblCurrentErosion, dblCurrentEpsilon,
                    strSimplification, strBufferStyle, dblMiterLimit));
            }
            else
            {
                //var SubCpgLt = mergedcpg.SubCpgLt;
                var submergedcpglt = MergeCloseCpgsAndAddBridges(mergedcpg.SubCpgLt, false, 
                    dblCurrentGrow, dblCurrentDilation, dblCurrentEpsilon);
                //var BridgeCpipeDt = mergedcpg.BridgeCpipeDt;
                foreach (var submergedcpg in submergedcpglt)
                {
                    LastAndClippedPath.AddRange(DilateErodeOffsetSimplifyCpg(submergedcpg,
                            dblCurrentGrow, dblCurrentDilation, dblCurrentErosion, dblCurrentEpsilon,
                            strSimplification, strBufferStyle, dblMiterLimit));
                }
            }

            //CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb(
            //    clipperMethods.ConvertPathsToCEdgeEb(LastAndClippedPath, true), 1 / CConstants.dblFclipper),
            //    "LastAndClippedPath" + CHelpFunc.GetTimeStampWithPrefix());

            LastAndClippedPath.AddRange(mergedcpg.LastTimePaths);
            var unitedPaths = clipperMethods.Clip_Paths(LastAndClippedPath, true, mergedcpg.LastTimePaths, true, ClipType.ctUnion);
            //var unitedPaths = LastAndClippedPath;

            var clippedPolyTree =
                clipperMethods.Clip_PolyTree(unitedPaths, true, clipPathsFirstLevel, true, ClipType.ctIntersection);

            mergedcpg.LastTimePaths = Clipper.PolyTreeToPaths(clippedPolyTree);
            var GrownAndClippedCpg = clipperMethods.GenerateCpgEbByPolyTree(clippedPolyTree, mergedcpg.ID, true);
            return GrownAndClippedCpg;
        }

        private Paths DilateErodeOffsetSimplifyCpg(CPolygon cpg, 
            double dblGrow, double dblDilation, double dblErosion, double dblEpsilon,
           string strSimplification, string strBufferStyle, double dblMiterLimit)
        {
            var rawPaths = new Paths();

            var rawCpgEb = clipperMethods.DilateErodeOffsetSimplifyOneComponent(cpg,
                        dblGrow, dblDilation, dblErosion, dblEpsilon,
                         strSimplification, true, strBufferStyle, dblMiterLimit);
            //CSaveFeature.SaveCpgEb(rawCpgEb, "rawCpgEb" + CHelpFunc.GetTimeStampWithPrefix());
            foreach (var rawCpg in rawCpgEb)
            {
                rawCpg.SetExteriorPath();
                rawPaths.AddRange(rawCpg.GetAllPaths());
            }


            if (cpg.HoleCpgLt != null && cpg.HoleCpgLt.Count > 0)
            {
                foreach (var holecpg in cpg.HoleCpgLt)
                {
                    if (holecpg.ClipCpgLt == null || holecpg.ClipCpgLt.Count == 0)
                    {
                        continue;
                    }

                    var rawholePaths = new Paths();
                    var rawholeCpgEb = clipperMethods.DilateErodeOffsetSimplifyOneComponent(holecpg,
                       dblGrow, dblDilation, dblErosion, dblEpsilon,
                       strSimplification, true, strBufferStyle, dblMiterLimit);
                    foreach (var rawholeCpg in rawholeCpgEb)
                    {
                        rawholeCpg.SetExteriorPath();
                        rawholePaths.AddRange(rawholeCpg.GetAllPaths());
                    }


                    var clipPaths = clipperMethods.GenerateClipPathsByCpgEb(holecpg.ClipCpgLt);
                    var clippedPaths = clipperMethods.Clip_Paths(rawholePaths, true, clipPaths, true, ClipType.ctIntersection);
                    rawPaths.AddRange(clippedPaths);
                }
            }

            return rawPaths;
        }

        #endregion

    }
}
