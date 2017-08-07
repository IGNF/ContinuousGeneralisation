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

        //private double _dblBufferRadius = -2 * _dblFclipper;
        private void UpdateStartEnd(ref int intStart, ref int intEnd)
        {
            //intStart = 543;
            //intEnd = intStart + 1;
            //intEnd = 2;
        }

        private static int _intDigits = 6;

        private static int _intStart = 0;
        private static int _intEnd = _intStart + 1;



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


            //we suppose that a hole is not separated to several parts. a hole can contain other holes.

            Stopwatch pStopwatch = Stopwatch.StartNew();
            CParameterInitialize pParameterInitialize = _ParameterInitialize;

            var LSCpgLt = this.ObjCGeoLtLt[0].AsExpectedClass<CPolygon, object>().ToList();

            double dblStartScale = dblLS;
            double dblFclipper = CConstants.dblFclipper;

            var MagnifiedCpgLt = clipperMethods.ScaleCpgLt(LSCpgLt, dblFclipper).ToList();
            CConstants.dblVerySmallCoord *= dblFclipper;
            int intStart = 0;
            int intEnd = intStart + intOutputMapCount;
            CHelpFunc.Displaytspb(0.5, intEnd - intStart, pParameterInitialize.tspbMain);
            double dblTargetScale = 0;
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


                _dblAreaLimit = 4;//area limit 4 mm^2
                //double dblAreaLimitTargetScale= dblAreaLimit * dblTargetScale * dblTargetScale;

                _dblEpsilon = 0.2;
                double dblTargetEpsilon = _dblEpsilon * dblTargetScale * dblFclipper;


                _dblLambda = 0.8;
                _dblTotalGrow = _dblLambda / 2 * Math.Sqrt(_dblAreaLimit) * (dblTargetScale - dblStartScale);
                double dblTotalGrow = _dblTotalGrow * dblFclipper;

                _dblHoleAreaLimit = 8; //dblHoleAreaLimit=8 mm^2 * dblTargetScale * dblTargetScale
                double dblHoleAreaLimitTargetScale = _dblHoleAreaLimit * dblTargetScale * dblTargetScale * dblFclipper * dblFclipper;

                //_dblDilation = Math.Sqrt(_dblHoleAreaLimit / Math.PI);
                _dblDilation = _dblEpsilon / 2;
                double dblTargetDilation = _dblDilation * dblTargetScale * dblFclipper;

                _dblErosion = _dblEpsilon / 2; //To avoid breaking the polygon when we errod, dblOverDilated should not be too large 
                double dblTargetErosion = _dblErosion * dblTargetScale * dblFclipper;


                MagnifiedCpgLt.ForEach(cpg => cpg.RemoveClosePoints());
                MagnifiedCpgLt.ForEach(cpg => cpg.FormCEdgeLt());
                var mergedCpgLt = MergeCloseCpgsAndAddBridges(MagnifiedCpgLt, dblTotalGrow, dblTargetEpsilon);

                //dblGrow, dblDilation, dblErosion, dblEpsilon

                var targetcpglt = new List<CPolygon>();
                foreach (var mergedcpg in mergedCpgLt)
                {
                    SetClipCpgLt_BufferDilateErodeSimplify(mergedcpg,
                        dblTotalGrow, dblTargetDilation, dblTargetErosion, dblTargetEpsilon,
                        dblHoleAreaLimitTargetScale, strSimplification, strBufferStyle, dblMiterLimit);

                    var targetCpg = mergedcpg.ClipCpgLt[0];
                    
                    if (mergedcpg.HoleCpgLt != null)
                    {
                        if (targetCpg.HoleCpgLt==null)
                        {
                            targetCpg.HoleCpgLt = new List<CPolygon>();
                        }

                        foreach (var holecpg in mergedcpg.HoleCpgLt)
                        {
                            foreach (var clipholecpg in holecpg.ClipCpgLt)
                            {
                                targetCpg.HoleCpgLt.Add(clipholecpg);
                            }
                        }
                    }
                    targetcpglt.Add(targetCpg);
                }




                this.MergedCpgLt = mergedCpgLt;
                var scaledbackcpgEb = clipperMethods.ScaleCpgLt(targetcpglt, 1 / dblFclipper);
                //remove small polygons and small holes
                //var remainedcpgeb = GetLargeCpgEb(scaledbackcpgEb, dblAreaLimit, dblHoleAreaLimit);

                CSaveFeature.SaveCGeoEb(scaledbackcpgEb, esriGeometryType.esriGeometryPolygon, strSimplification + "_" +
     dblTargetScale + "k_TargetForm" + dblTargetEpsilon + "m_" + CHelpFunc.GetTimeStampWithPrefix(),
    pParameterInitialize, intGreen: 255);

                //MagnifiedCpgLt = targetcpglt;
                dblStartScale = dblTargetScale;
                CHelpFunc.Displaytspb((i + 1 - intStart), intEnd - intStart, pParameterInitialize.tspbMain);




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





        #region GroupCloseCpgsAndAddBridges single bridges

    //    private List<CPolygon> IterativeMergeCloseCpgsAndAddBridges(List<CPolygon> cpglt,
    //double dblGrow, double dblDilation, double dblErosion, double dblEpsilon)
    //    {
    //        var mergedcpglt = cpglt;
    //        do
    //        {
    //            cpglt = mergedcpglt;
    //            mergedcpglt = MergeCloseCpgsAndAddBridges(cpglt, dblGrow, dblDilation, dblErosion, dblEpsilon);
    //        } while (mergedcpglt.Count<cpglt.Count);

    //        return mergedcpglt;
    //    }

        private List<CPolygon> MergeCloseCpgsAndAddBridges(List<CPolygon> cpglt, double dblGrow, double dblEpsilon)
        {
            var CloseCpipeDt = GetCloseCpipeSD(cpglt, dblGrow, dblEpsilon);
            var CptEdgeDisSSLt = GetCptEdgeDisSSLt_FromCpipeDt(cpglt, CloseCpipeDt);
            return GetMergedCpgLt_Prim(cpglt, CptEdgeDisSSLt);
        }

        private List<CPolygon> GetMergedCpgLt_Prim(List<CPolygon> cpglt, List<SortedSet<CptEdgeDis>> CptEdgeDisSSLt)
        {
            cpglt.ForEach(cpg => cpg.IsSubCpg = false);

            //*****************When computing intermediate resulats (second time of using this function),
            //we can do this step in a simpler way, we don't really need to use BFS. 
            //Instead, we know that all the polygons are connected as a tree
            var GroupedCptEdgeDisSSLt = GroupCpgsAndBridges_Prim(cpglt, CptEdgeDisSSLt).ToList();

            var mergedCpgLt = new List<CPolygon>();
            //add merged polygons
            foreach (var GroupedCptEdgeDisLt in GroupedCptEdgeDisSSLt)
            {
                //merge grouped cpgs by DCEL; add SubCpgLt and BridgeCptEdgeDisSS 
                mergedCpgLt.Add(MergeGroupedCpgsAndBridges(GroupedCptEdgeDisLt));
            }
            //add alone polygons
            foreach (var cpg in cpglt)
            {
                if (cpg.IsSubCpg == false)
                {
                    cpg.SubCpgLt = new List<CPolygon>
                    {
                        cpg
                    };

                    mergedCpgLt.Add(cpg);
                }
            }
            return mergedCpgLt;
        }


        private Dictionary<CValPairIncr<CPolygon>, CptEdgeDis>
            GetCloseCpipeSD(List<CPolygon> cpglt, double dblGrow, double dblEpsilon)
        {
            var grownCpgLt = GetGrownExteriorCpgLt(cpglt, dblGrow);

            //Each Bridge in CloseGrownCpgPairPtEdgeDisSD is from a grown Cpg to another grown Cpg
            var grownCloseCpgPairPtEdgeDisDt = GetCloseCpgPairPtEdgeDisDt_EdgeRelation(grownCpgLt, dblEpsilon*Math.Sqrt(5)/2);

            //Each Bridge in CloseCpgPairPtEdgeDisSD is from an original Cpg to another original Cpg
            var CloseCpipeDt = new Dictionary<CValPairIncr<CPolygon>, CptEdgeDis>();
            //each building can grow up to 2*dblGrow
            double dblCloseDis = dblEpsilon * Math.Sqrt(5) / 2 + 4 * dblGrow;  //dblCloseDis just need to be large enough
            cpglt.SetIndexID();
            foreach (var cpg in cpglt)
            {
                foreach (var cedge in cpg.CEdgeLt)
                {
                    cedge.BelongedCpg = cpg;
                }
            }
            foreach (var CloseGrownCpgPairPtEdgeDt in grownCloseCpgPairPtEdgeDisDt)
            {
                var cpg1 = cpglt[CloseGrownCpgPairPtEdgeDt.Key.val1.indexID];
                var cpg2 = cpglt[CloseGrownCpgPairPtEdgeDt.Key.val2.indexID];

                var subCloseCpgPairPtEdgeDisDt = GetCloseCpgPairPtEdgeDisDt_EdgeRelation(CHelpFunc.MakeLt(cpg1, cpg2), dblCloseDis);
                var kvp = subCloseCpgPairPtEdgeDisDt.First();  //there is only one element in subCloseCpgPairPtEdgeDisSD
                kvp.Value.SetConnectEdge();  //set ConnectEdge for all the bridges
                CloseCpipeDt.Add(kvp.Key, kvp.Value);
            }

            return CloseCpipeDt;
        }

        /// <summary>get the grownCpg of each cpg's exterior ring</summary>
        private List<CPolygon> GetGrownExteriorCpgLt(List<CPolygon> cpglt, double dblGrow)
        {
            var grownCpgLt = new List<CPolygon>(cpglt.Count);
            foreach (var cpg in cpglt)
            {
                var ExteriorPath = clipperMethods.GeneratePathByCpgExterior(cpg);
                var ExteriorOffsetPolyTree = clipperMethods.Offset_PolyTree(CHelpFunc.MakeLt(ExteriorPath), dblGrow);
                var ExteriorOffsetCpg = clipperMethods.GenerateOLHCpgByPolyTree(ExteriorOffsetPolyTree, cpg.ID);

                //ExteriorOffsetCpg.pPolyTree = ExteriorOffsetPolyTree;
                //cpg.ExteriorOffsetCpg = ExteriorOffsetCpg;

                grownCpgLt.Add(ExteriorOffsetCpg);
            }

            grownCpgLt.SetIndexID();
            foreach (var cpg in grownCpgLt)
            {
                cpg.FormCEdgeLt();
                foreach (var cedge in cpg.CEdgeLt)
                {
                    cedge.BelongedCpg = cpg;
                }
            }

            return grownCpgLt;
        }

        private Dictionary<CValPairIncr<CPolygon>, CptEdgeDis> GetCloseCpgPairPtEdgeDisDt_EdgeRelation(
    List<CPolygon> cpglt, double dblCloseDis)
        {
            var cedgelt = new List<CEdge>();
            foreach (var cpg in cpglt)
            {
                cedgelt.AddRange(cpg.CEdgeLt);
            }

            var CloseCpipeDt = new Dictionary<CValPairIncr<CPolygon>, CptEdgeDis>(cpglt.Count * 2, new CCmpEqCpgPairIncr());
            foreach (var pEdgeRelation in CGeoFunc.DetectCEdgeRelations(cedgelt, dblCloseDis))
            {
                //if (pEdgeRelation.pEnumDisMode == CEnumDisMode.InIn)
                //{
                //    throw new ArgumentException("polygons intersect with each other!");
                //}

                //pEdgeRelation.CEdge1.PrintMySelf();
                //pEdgeRelation.CEdge2.PrintMySelf();

                var cpg1 = pEdgeRelation.CEdge1.BelongedCpg;
                var cpg2 = pEdgeRelation.CEdge2.BelongedCpg;
                if (cpg1.GID != cpg2.GID)
                {
                    CptEdgeDis cptEdgeDis;
                    var cpgpair = new CValPairIncr<CPolygon>(cpg1, cpg2);
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

            return CloseCpipeDt;
        }


        private static List<SortedSet<CptEdgeDis>> GetCptEdgeDisSSLt_FromCpipeDt(List<CPolygon> cpglt,
            Dictionary<CValPairIncr<CPolygon>, CptEdgeDis> CloseCpgPairPtEdgeDisDt)
        {
            var CptEdgeDisSSLt = new List<SortedSet<CptEdgeDis>>(cpglt.Count);
            CptEdgeDisSSLt.EveryElementValue(null);
            foreach (var CloseCpgPairPtEdgeDisKvp in CloseCpgPairPtEdgeDisDt)
            {
                var cpgpair = CloseCpgPairPtEdgeDisKvp.Key;
                var cpg1 = cpgpair.val1;
                var cpg2 = cpgpair.val2;
                var PtEdgeDis = CloseCpgPairPtEdgeDisKvp.Value;
                PtEdgeDis.CpgPairIncr = cpgpair;

                if (CptEdgeDisSSLt[cpg1.indexID] == null && CptEdgeDisSSLt[cpg2.indexID] == null)
                {
                    CptEdgeDisSSLt[cpg1.indexID] = new SortedSet<CptEdgeDis>(CCmpCptEdgeDis_Dis.sComparer);
                    CptEdgeDisSSLt[cpg1.indexID].Add(PtEdgeDis);
                    CptEdgeDisSSLt[cpg2.indexID] = CptEdgeDisSSLt[cpg1.indexID];
                }
                else if (CptEdgeDisSSLt[cpg1.indexID] != null && CptEdgeDisSSLt[cpg2.indexID] == null)
                {
                    CptEdgeDisSSLt[cpg1.indexID].Add(PtEdgeDis);
                    CptEdgeDisSSLt[cpg2.indexID] = CptEdgeDisSSLt[cpg1.indexID];
                }
                else if (CptEdgeDisSSLt[cpg1.indexID] == null && CptEdgeDisSSLt[cpg2.indexID] != null)
                {
                    CptEdgeDisSSLt[cpg2.indexID].Add(PtEdgeDis);
                    CptEdgeDisSSLt[cpg1.indexID] = CptEdgeDisSSLt[cpg2.indexID];
                }
                else // if (CptEdgeDisSSLt[cpg1.indexID] != null && CptEdgeDisSSLt[cpg2.indexID] != null)
                {
                    if (CptEdgeDisSSLt[cpg1.indexID].Min.CompareTo(CptEdgeDisSSLt[cpg2.indexID].Min) != 0)
                    {
                        if (CptEdgeDisSSLt[cpg1.indexID].Count >= CptEdgeDisSSLt[cpg2.indexID].Count)
                        {
                            AggregateCloseInfoSD(CptEdgeDisSSLt[cpg1.indexID], CptEdgeDisSSLt[cpg2.indexID], CptEdgeDisSSLt);
                        }
                        else
                        {
                            AggregateCloseInfoSD(CptEdgeDisSSLt[cpg2.indexID], CptEdgeDisSSLt[cpg1.indexID], CptEdgeDisSSLt);
                        }
                    }
                    CptEdgeDisSSLt[cpg1.indexID].Add(PtEdgeDis);
                }

            }

            return CptEdgeDisSSLt;
        }

        private static void AggregateCloseInfoSD(SortedSet<CptEdgeDis> MoreInfoSD, SortedSet<CptEdgeDis> FewerInfoSD,
           List<SortedSet<CptEdgeDis>> CptEdgeDisSSLt)
        {
            foreach (var FewerInfoKvp in FewerInfoSD)
            {
                MoreInfoSD.Add(FewerInfoKvp);
                CptEdgeDisSSLt[FewerInfoKvp.CpgPairIncr.val1.indexID] = MoreInfoSD;
                CptEdgeDisSSLt[FewerInfoKvp.CpgPairIncr.val2.indexID] = MoreInfoSD;
            }
        }



        /// <summary>return lists of grouped CptEdgeDis, alone polygons are not included</summary>
        private static IEnumerable<SortedSet<CptEdgeDis>> GroupCpgsAndBridges_Prim(List<CPolygon> cpglt,
            List<SortedSet<CptEdgeDis>> CptEdgeDisSSLt)
        {
            cpglt.ForEach(cpg => cpg.isTraversed = false);

            for (int i = 0; i < CptEdgeDisSSLt.Count; i++)
            {
                var CptEdgeDisSS = CptEdgeDisSSLt[i];
                if (CptEdgeDisSS == null)  //the polygon is far away from other polygons
                {
                    continue;
                    //cpglt[i].IsSubCpg = true;
                }

                var FirstCpg = CptEdgeDisSS.Min.CpgPairIncr.val1;
                if (FirstCpg.isTraversed == true)  //all the polygons in this entry have been handled because of an earlier entry
                {
                    continue;
                }

                foreach (var CptEdgeDis in CptEdgeDisSS)
                {
                    CptEdgeDis.CpgPairIncr.val1.IsSubCpg = true;
                    CptEdgeDis.CpgPairIncr.val2.IsSubCpg = true;
                }

                yield return GroupCpgsAndBridgesForEachCluster_Prim(CptEdgeDisSS);
            }
        }

        private static SortedSet<CptEdgeDis> GroupCpgsAndBridgesForEachCluster_Prim(SortedSet<CptEdgeDis> CloseInfoSD)
        {
            var CptEdgeDisSSEt = CloseInfoSD.GetEnumerator();
            CptEdgeDisSSEt.MoveNext();
            var FirstCptEdgeDisSS = CptEdgeDisSSEt.Current;

            var CpgCptEdgeDisLtSD = new SortedDictionary<CPolygon, List<CptEdgeDis>>();
            do
            {
                //add all close relations to CptEdgeDisLt
                UpdateCpgCptEdgeDisLtSD(ref CpgCptEdgeDisLtSD, CptEdgeDisSSEt.Current.CpgPairIncr.val1, CptEdgeDisSSEt.Current);
                UpdateCpgCptEdgeDisLtSD(ref CpgCptEdgeDisLtSD, CptEdgeDisSSEt.Current.CpgPairIncr.val2, CptEdgeDisSSEt.Current);
            } while (CptEdgeDisSSEt.MoveNext());

            var intCpgCount = CpgCptEdgeDisLtSD.Count;
            var FirstCpg = FirstCptEdgeDisSS.CpgPairIncr.val1;
            //FirstCpg.isTraversed = true;  //we should not do this
            var groupcpglt = new List<CPolygon>(intCpgCount);
            //groupcpglt.Add(FirstCpg);  //we should not do this 
            var BridgeCptEdgeDisSS = new SortedSet<CptEdgeDis>(CCmpCptEdgeDis_Dis.sComparer);
            var SmallestCptEdgeDis = FirstCptEdgeDisSS;
            //var queueSS = new SortedSet<CValPair<CptEdgeDis, CPolygon>>();
            var queueSD = new SortedDictionary<CptEdgeDis, CPolygon>(CCmpCptEdgeDis_Dis.sComparer);

            queueSD.Add(SmallestCptEdgeDis, FirstCpg);
            do
            {
                //brige polygons by shortest connections, to avoid crossed bridges
                var minCptEdgeDisCpgKvp = queueSD.First();
                queueSD.Remove(minCptEdgeDisCpgKvp.Key);

                var BridgedCpg = minCptEdgeDisCpgKvp.Value;
                if (BridgedCpg.isTraversed == true)  //this may be effective only for FirstCpg
                {
                    continue;
                }
                BridgedCpg.isTraversed = true;
                groupcpglt.Add(BridgedCpg);
                BridgeCptEdgeDisSS.Add(minCptEdgeDisCpgKvp.Key);

                List<CptEdgeDis> outCptEdgeDisLt;
                CpgCptEdgeDisLtSD.TryGetValue(BridgedCpg, out outCptEdgeDisLt);

                foreach (var cptEdgeDis in outCptEdgeDisLt)
                {
                    var cpg1 = cptEdgeDis.CpgPairIncr.val1;
                    var cpg2 = cptEdgeDis.CpgPairIncr.val2;

                    if (cpg1.isTraversed == false)
                    {
                        queueSD.Add(cptEdgeDis, cpg1);
                    }
                    if (cpg2.isTraversed == false)
                    {
                        queueSD.Add(cptEdgeDis, cpg2);
                    }
                }

            } while (queueSD.Count > 0 && groupcpglt.Count < intCpgCount);
            return BridgeCptEdgeDisSS;
        }

        private static void UpdateCpgCptEdgeDisLtSD(
            ref SortedDictionary<CPolygon, List<CptEdgeDis>> CpgCptEdgeDisLtSD, CPolygon cpg, CptEdgeDis cptEdgeDis)
        {
            List<CptEdgeDis> outCptEdgeDisLt;
            if (CpgCptEdgeDisLtSD.TryGetValue(cpg, out outCptEdgeDisLt))
            {
                outCptEdgeDisLt.Add(cptEdgeDis);
            }
            else
            {
                outCptEdgeDisLt = new List<CptEdgeDis> { cptEdgeDis };
                CpgCptEdgeDisLtSD.Add(cpg, outCptEdgeDisLt);
            }
        }

        private CPolygon MergeGroupedCpgsAndBridges(SortedSet<CptEdgeDis> CptEdgeDisSS)
        {
            //we use SortedSet so that each cpg will be added once
            var cpgss = new SortedSet<CPolygon>();
            foreach (var CptEdgeDis in CptEdgeDisSS)
            {
                cpgss.Add(CptEdgeDis.CpgPairIncr.val1);
                cpgss.Add(CptEdgeDis.CpgPairIncr.val2);
            }
            var cpglt = cpgss.ToList();
            cpglt.ForEach(cpg => cpg.isTraversed = false);

            //some buildings may share boundaries, so we may have same edge 
            //we use HashSet, instead of List, because we may remove some same edges
            var CEdgeHS = new HashSet<CEdge>(new CCmpEqCEdgeCoord());
            cpglt.ForEach(cpg => cpg.CEdgeLt.ForEach(cedge => CEdgeHS.Add(cedge)));
            CGeoFunc.CheckShortEdges(CEdgeHS);

            var BridgeCEdgeLt = new List<CEdge>(cpglt.Count);
            var AllCEdgeLt = new List<CEdge>(CEdgeHS.Count + cpglt.Count); //the count is roughtly
            var CEdgeCptEdgeDisLtSD = new SortedDictionary<CEdge, List<CptEdgeDis>>();
            foreach (var cptEdgeDis in CptEdgeDisSS)
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
                    List<CptEdgeDis> outCptEdgeDislt;
                    if (CEdgeCptEdgeDisLtSD.TryGetValue(cptEdgeDis.ToCEdge, out outCptEdgeDislt) == false)
                    {
                        outCptEdgeDislt = new List<CptEdgeDis>(1);
                        outCptEdgeDislt.Add(cptEdgeDis);
                        CEdgeCptEdgeDisLtSD.Add(cptEdgeDis.ToCEdge, outCptEdgeDislt);
                    }
                    else
                    {
                        outCptEdgeDislt.Add(cptEdgeDis);
                    }
                }
            }
            AllCEdgeLt.AddRange(BridgeCEdgeLt);


            //CGeoFunc.CheckShortEdges(AllCEdgeLt);

            //We need to split the edge into several edges
            foreach (var CEdgeCptEdgeDisLt in CEdgeCptEdgeDisLtSD)
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



            //CSaveFeature.SaveCGeoEb(clipperMethods.ScaleCEdgeEb(AllCEdgeLt, 1 / _dblFclipper), esriGeometryType.esriGeometryPolyline,
            //    "TestEdgeLt" + CHelpFunc.GetTimeStampWithPrefix(), _ParameterInitialize);


            CDCEL pDCEL = new CGeometry.CDCEL(AllCEdgeLt);
            pDCEL.ConstructDCEL();

            var mergedcpg = new CPolygon(cpglt[0].ID, pDCEL.FaceCpgLt[0].GetOnlyInnerCptLt(), holecpglt);
            mergedcpg.SubCpgLt = cpglt;
            mergedcpg.BridgeCptEdgeDisSS = CptEdgeDisSS;
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
                yield return new CEdge(precpt, cpter.Current);
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
            mergedcpg.FormCEdgeLt();
            mergedcpg.ClipCpgLt = new List<CGeometry.CPolygon>();

            var ExteriorOffsetPolyTree = clipperMethods.DilateErodeOffsetCpgExterior_PolyTree
                (mergedcpg, dblGrow, dblDilation, dblErosion, strBufferStyle, dblMiterLimit);
            var ExteriorOffsetCpg = clipperMethods.GenerateOLHCpgByPolyTree(ExteriorOffsetPolyTree, mergedcpg.ID);

            //we also simplify the holes from Exterior ring
            var targetCpg = CDPSimplify.SimplifyAccordRightAnglesAndExistEdges(ExteriorOffsetCpg, mergedcpg.CEdgeLt, strSimplification, 2 * dblEpsilon);
            mergedcpg.ClipCpgLt.Add(targetCpg);
            //if (targetCpg.HoleCpgLt != null)
            //{
            //    mergedcpg.ClipCpgLt.AddRange(targetCpg.HoleCpgLt);  //we should not do this
            //}


            //add original holes, which are stored in mergedcpg
            if (mergedcpg.HoleCpgLt != null && mergedcpg.HoleCpgLt.Count > 0)
            {
                //get the BufferDilateErodeSimplify holes
                var holecpglt = new List<CPolygon>(mergedcpg.HoleCpgLt.Count);
                mergedcpg.SetAreaSimple();
                foreach (var holecpg in mergedcpg.HoleCpgLt)
                {
                    if (holecpg.dblAreaSimple < dblHoleAreaLimit)
                    {
                        continue;
                    }

                    var holeOffsetPolyTree = clipperMethods.DilateErodeOffsetCpgExterior_PolyTree(holecpg, 
                        dblGrow, dblDilation, dblErosion, strBufferStyle, dblMiterLimit);

                    holecpg.ClipCpgLt = new List<CPolygon>();
                    foreach (var OffsetHoleCpg in clipperMethods.GenerateOLHCpgEbByPolyTree(holeOffsetPolyTree, holecpg.ID, true))
                    {
                        var simplifiedcpg = CDPSimplify.SimplifyAccordRightAnglesAndExistEdges(OffsetHoleCpg,
                            holecpg.CEdgeLt, strSimplification, 2 * dblEpsilon);
                        holecpg.ClipCpgLt.Add(simplifiedcpg);
                    }
                }

                ////add the large holes into targetCpg
                //if (targetCpg.HoleCpgLt == null)
                //{
                //    targetCpg.HoleCpgLt = new List<CPolygon>(holecpglt.Count);
                //}
                //foreach (var holecpg in holecpglt)
                //{
                //    holecpg.SetAreaSimple();
                //    if (holecpg.dblAreaSimple > dblHoleAreaLimit)
                //    {
                //        targetCpg.HoleCpgLt.Add(holecpg);
                //    }
                //}
            }
            //return targetCpg;
        }

        #region Output
        public void Output(double dblProportion)
        {
            var resultCpgEb = GetResultCpgEb(this.MergedCpgLt, dblProportion);
            CSaveFeature.SaveCGeoEb(resultCpgEb, esriGeometryType.esriGeometryPolygon,
                dblProportion + "_Growing", _ParameterInitialize, intBlue: 255);
        }


        public void MakeAnimations()
        {
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
                _ParameterInitialize.pFLayerLt[0], _ParameterInitialize.pFLayerLt[0].AreaOfInterest, CConstants.pIpeEnv, "0.05");

            string strFullName = _ParameterInitialize.strSavePath + "\\" + CHelpFunc.GetTimeStamp() + ".ipe";
            using (var writer = new System.IO.StreamWriter(strFullName, true))
            {
                writer.Write(CIpeDraw.GenerateIpeContentByDataWithLayerInfo(strContent));
            }

            System.Diagnostics.Process.Start(@strFullName);
        }

        private string strDataOfLayers(int intLayerNum, List<string> strLayerNameLt, IFeatureLayer pFLayer,
            IEnvelope pFLayerEnv, CEnvelope pIpeEnv, string strBoundWidth)
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

                strDataAllLayers += CToIpe.GetScaleLegend(pFLayerEnv, pIpeEnv, CHelpFunc.GetUnits(_ParameterInitialize.m_mapControl.MapUnits));

                //add the Content of animations
                strDataAllLayers += "<group>\n";
                strDataAllLayers += CIpeDraw.drawIpeEdge(pIpeEnv.XMin, pIpeEnv.YMin, pIpeEnv.XMin, pIpeEnv.YMax, "white");
                foreach (var cpg in GetResultCpgEb(this.MergedCpgLt, Convert.ToDouble(strLayerNameLt[i])))
                {
                    strDataAllLayers += CIpeDraw.DrawCpg(cpg, pFLayerEnv, pIpeEnv,
                        new CUtility.CColor(128, 128, 128), new CUtility.CColor(0, 255, 0), strBoundWidth);
                }
                strDataAllLayers += "</group>\n";
                //strDataAllLayers += CToIpe.TranIpgToIpe(IpgLt[i - 1], pFillSymbolLt[i - 1], pFLayerEnv, pIpeEnv, strBoundWidth);
            }

            return strDataAllLayers;
        }

        private IEnumerable<CPolygon> GetResultCpgEb(List<CPolygon> mergedCpgLt, double dblProportion)
        {
            //dblProportion = 0.6;
            double dblCurrentScale = _dblStartScale + dblProportion * (_dblTargetScale - _dblStartScale);
            double dblAreaLimitCurrentScale = _dblAreaLimit * dblCurrentScale * dblCurrentScale
                * CConstants.dblFclipper * CConstants.dblFclipper;
            double dblHoleAreaLimitTargetScale = _dblHoleAreaLimit * dblCurrentScale * dblCurrentScale
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
                var GrownAndClippedCpgLt = GrowAndClipMergedCpg(mergedcpg, dblProportion).ToList();
                if (GrownAndClippedCpgLt.Count == 1)  //the subcpgs are merged into one 
                {
                    var blnlargeEnoughCpg = CheckLargeCpgAndRemoveSmallHoles(GrownAndClippedCpgLt[0],
                        dblAreaLimitCurrentScale, dblHoleAreaLimitTargetScale);
                    if (blnlargeEnoughCpg == true)
                    {
                        AllGrownAndClippedCpgLt.Add(GrownAndClippedCpgLt[0]);
                    }
                    else
                    {
                        mergedcpg.WasTooSmall = true;
                    }
                }
                else
                {
                    //remove small holes
                    CheckLargeCpgEbAndRemoveSmallHoles(GrownAndClippedCpgLt,
                        dblAreaLimitCurrentScale, dblHoleAreaLimitTargetScale).ToList();
                    AllGrownAndClippedCpgLt.AddRange(GrownAndClippedCpgLt);
                }
            }

            var scaledbackcpgLt = clipperMethods.ScaleCpgLt(AllGrownAndClippedCpgLt, 1 / CConstants.dblFclipper).ToList();
            return scaledbackcpgLt;
        }

        private IEnumerable<bool> CheckLargeCpgEbAndRemoveSmallHoles(IEnumerable<CPolygon> cpgeb,
            double dblAreaLimit, double dblHoleAreaLimit)
        {
            //dblHoleArea = 0;
            foreach (var cpg in cpgeb)
            {
                yield return CheckLargeCpgAndRemoveSmallHoles(cpg, dblAreaLimit, dblHoleAreaLimit);
            }
        }

        private bool CheckLargeCpgAndRemoveSmallHoles(CPolygon cpg, double dblAreaLimit, double dblHoleAreaLimit)
        {
            cpg.SetAreaSimple();
            if (cpg.dblAreaSimple >= dblAreaLimit)
            {
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
                return true;
            }
            else
            {
                return false;
            }
        }

        private IEnumerable<CPolygon> GrowAndClipMergedCpg(CPolygon mergedcpg, double dblProportion)
        {
            //dblProportion = 0.99;
            double dblCurrentScale = _dblStartScale + dblProportion * (_dblTargetScale - _dblStartScale);
            double dblCurrentGrow = dblProportion * _dblTotalGrow * CConstants.dblFclipper;
            double dblCurrentEpsilon = _dblEpsilon * dblCurrentScale * CConstants.dblFclipper;
            double dblCurrentDilation = _dblDilation * dblCurrentScale * CConstants.dblFclipper;
            double dblCurrentErosion = _dblErosion * dblCurrentScale * CConstants.dblFclipper;


            //double dblCurrentSimplifyDepsilon=_dblSimplifyEpsilon * dblCurrentScale * _dblFclipper;

            var clipPathsFirstLevel = clipperMethods.GenerateClipPathsByCpgEb(mergedcpg.ClipCpgLt);
            if (mergedcpg.SubCpgLt.Count == 1)
            {
                yield return GrowAndClipCpg(mergedcpg, dblCurrentGrow, dblCurrentDilation, dblCurrentErosion, clipPathsFirstLevel);
            }
            else
            {
                var SubCpgLt = mergedcpg.SubCpgLt;
                SubCpgLt.SetIndexID();
                var grownCpgLt = GetGrownExteriorCpgLt(SubCpgLt, dblCurrentGrow);

                var CloseCpgPairPtEdgeDisDt = new Dictionary<CValPairIncr<CPolygon>, CptEdgeDis>(new CCmpEqCpgPairIncr());
                foreach (var BridgeCptEdgeDis in mergedcpg.BridgeCptEdgeDisSS)
                {
                    //compute real minimum distance of two buildings, after growing
                    var cpg1 = grownCpgLt[BridgeCptEdgeDis.CpgPairIncr.val1.indexID];
                    var cpg2 = grownCpgLt[BridgeCptEdgeDis.CpgPairIncr.val2.indexID];

                    var subCloseCpgPairPtEdgeDisDt =
                        GetCloseCpgPairPtEdgeDisDt_EdgeRelation(CHelpFunc.MakeLt(cpg1, cpg2), dblCurrentEpsilon);

                    if (subCloseCpgPairPtEdgeDisDt.Count > 0)
                    {
                        CloseCpgPairPtEdgeDisDt.Add(BridgeCptEdgeDis.CpgPairIncr, BridgeCptEdgeDis);
                    }
                }

                //in the preprocess, we also need to compute CptEdgeDisSSLt. At that stage, we have no idea of closeness
                //at this stage, we already know which edge is a part of the MST of the totally merged building
                var CptEdgeDisSSLt = GetCptEdgeDisSSLt_FromCpipeDt(SubCpgLt, CloseCpgPairPtEdgeDisDt);

                var subMergedCpgLt = GetMergedCpgLt_Prim(SubCpgLt, CptEdgeDisSSLt);
                foreach (var subMergedCpg in subMergedCpgLt)
                {
                    //CSaveFeature.SaveCGeoEb(CHelpFunc.MakeLt(subMergedCpg), esriGeometryType.esriGeometryPolygon, "subMergedCpg", _ParameterInitialize);
                    yield return GrowAndClipCpg(subMergedCpg, dblCurrentGrow, dblCurrentDilation, dblCurrentErosion, clipPathsFirstLevel);
                }
            }
        }

        private CPolygon GrowAndClipCpg(CPolygon cpg,
            double dblGrow, double dblDilation, double dblErosion, Paths clipPathsFirstLevel)
        {
            var clippedPolyTree = clipperMethods.ClipOneComponent_BufferDilateErode(cpg,  
                dblGrow, dblDilation, dblErosion, clipPathsFirstLevel, CConstants.dblFclipper, _ParameterInitialize);
            var clippedCpg = clipperMethods.GenerateOLHCpgByPolyTree(clippedPolyTree, cpg.ID);


            if (cpg.HoleCpgLt != null && cpg.HoleCpgLt.Count > 0)
            {
                if (clippedCpg.HoleCpgLt == null)
                {
                    clippedCpg.HoleCpgLt = new List<CPolygon>(cpg.HoleCpgLt.Count);
                }

                foreach (var holecpg in cpg.HoleCpgLt)
                {
                    var clipPaths = clipperMethods.GenerateClipPathsByCpgEb(holecpg.ClipCpgLt);
                    var clippedHolePolyTree = clipperMethods.ClipOneComponent_BufferDilateErode(holecpg,
                        dblGrow, dblDilation, dblErosion, clipPaths, CConstants.dblFclipper, _ParameterInitialize);

                    //*******************check the direction of the HoleCpgs
                    foreach (var clippedHoleCpg in clipperMethods.GenerateOLHCpgEbByPolyTree(clippedHolePolyTree, holecpg.ID, true))
                    {
                        clippedCpg.HoleCpgLt.Add(clippedHoleCpg);
                    }
                }
            }

            return clippedCpg;
        }





        #endregion

    }
}
