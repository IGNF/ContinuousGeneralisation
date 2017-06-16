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

namespace MorphingClass.CGeneralizationMethods
{
    /// <summary>
    /// Building Growing
    /// </summary>
    /// <remarks></remarks>
    public class CBldgGrow : CMorphingBaseCpg
    {
        private static double _dblFactorClipper;
        private static int _intI = 0;

        //private double _dblBufferRadius = -2 * _dblFactorClipper;
        private void UpdateStartEnd(ref int intStart, ref int intEnd)
        {
            //intStart = 543;
            //intEnd = intStart + 1;
            //intEnd = 2;
        }

        private static int _intDigits = 6;

        private static int _intStart = 0;
        private static int _intEnd = _intStart + 1;



        #region Preprocessing
        public CBldgGrow()
        {

        }

        public CBldgGrow(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolygon, CPolygon>(ParameterInitialize, 1, 0, blnCreateFileGdbWorkspace: false);



        }


        public void BldgGrow(
            //double dblBufferRadius, 
            string strBufferStyle, double dblMiterLimit, double dblLS, double dblSS, int intOutput)
        {
            //**********************************************//
            //I may need to do buffering based on Miterjoint in a more clever way
            //if the distance from the miter point to original line is larger than dblMiterLimit*dblBufferRadius, we, instead of calling
            //the normal square method, make a square so that the farthest distance to the original line exactly dblMiterLimit*dblBufferRadius


            //we suppose that a hole is not separated to several parts. a hole can contain other holes.

            Stopwatch pStopwatch = Stopwatch.StartNew();
            CParameterInitialize pParameterInitialize = _ParameterInitialize;

            var LSCpgLt = this.ObjCGeoLtLt[0].AsExpectedClass<CPolygon, object>().ToList();
            //var LSIpgLt = this.ObjIGeoLtLt[0].AsExpectedClass<IPolygon5, object>().ToList();
            //var SSIpgLt = this.ObjIGeoLtLt[1].AsExpectedClass<IPolygon5, object>().ToList();
            //var LSIplLt = this.ObjIGeoLtLt[2].AsExpectedClass<IPolyline5, object>().ToList();
            //var SSIplLt = this.ObjIGeoLtLt[3].AsExpectedClass<IPolyline5, object>().ToList();

            //int intCount = 0;
            //var cptlt = new List<CPoint>
            //{
            //    new CPoint(intCount++, 100,100),
            //    new CPoint(intCount++, 200,100),
            //    new CPoint(intCount++, 200,200),
            //    new CPoint(intCount++, 300,200),
            //    new CPoint(intCount++, 300,100),
            //    new CPoint(intCount++, 400,100),
            //    new CPoint(intCount++, 400,300),
            //    new CPoint(intCount++, 300,200),
            //    new CPoint(intCount++, 200,200),
            //    new CPoint(intCount++, 100,200),
            //    new CPoint(intCount++, 100,100),
            //};
            //var newcpg = new CPolygon(0, cptlt);
            //LSCpgLt = new List<CPolygon> { newcpg };

            double dblLastScale = dblLS;
            //double dblStartScale = dblLS;
            //double dblGrowLast = 0;
            //double dblDLast = 0.2 * dblStartScale;
            double dblFactorClipper = 100000000;
            _dblFactorClipper = dblFactorClipper;
            var MagnifiedCpgLt = ScaleCpgLt(LSCpgLt, dblFactorClipper).ToList();
            CConstants.dblVerySmallCoord *= dblFactorClipper;
            int intStart = 0;
            int intEnd = intStart + intOutput;
            CHelpFunc.Displaytspb(0.5, intEnd - intStart, pParameterInitialize.tspbMain);
            double dblScale = 0;
            for (int i = intStart; i < intEnd; i++)
            {
                //double dblScale = dblStartScale + (i + 1) * 5;
                switch (i)
                {
                    case 0:
                        dblScale = 50;
                        //dblScale = 25;
                        break;
                    case 1:
                        dblScale = 100;
                        //dblScale = 50;
                        break;
                    case 2:
                        dblScale = 250;
                        //dblScale = 100;
                        break;
                    case 3:
                        dblScale = 500;
                        //CConstants.blnStop = true;
                        //dblScale = 250;
                        _intI = 3;
                        break;
                    case 4:
                        dblScale = 1000;
                        //CConstants.blnStop = true;
                        //dblScale = 500;
                        _intI = 3;
                        break;
                    default:
                        break;
                }




                //double dblD = 0.2 * dblScale;  //dblD is the increase limit and the limit distance between two buildings
                //double dblGrow = 0.75 * (dblD - dblDLast);  //0.15(dblScale-dblStartScale)
                ////double dblhalfD = 0.1 * dblScale;  //d=0.2mm * dblScale
                ////double dblGrow = 0.1 * dblScale;
                //double dblAreaLimit = 0.16 * dblScale * dblScale; //dblAreaLimit=0.16 mm^2 * dblScale * dblScale
                ////double dblHoleAreaLimit = 50* dblAreaLimit; //dblHoleAreaLimit=8 mm^2 * dblScale * dblScale
                //double dblGrowSpeed = dblGrow + dblD;  //0.35*dblScale - 0.15*dblStartScale
                //double dblHoleAreaLimit = Math.PI* dblGrowSpeed* dblGrowSpeed; //dblHoleAreaLimit=8 mm^2 * dblScale * dblScale
                ////double dblr = dblhalfD;  //r= 1/4 sqrt(A)= 0.1 * dblScale
                ////double dblSimpR = 0.2 * dblScale;  //a building should be large enough in order to have some detailes
                double dblSqrtAreaLimitMap = 1;
                double dblAqrtAreaLimitReal = dblSqrtAreaLimitMap * dblScale;
                double dblAreaLimit = dblAqrtAreaLimitReal * dblAqrtAreaLimitReal; //dblAreaLimit= 16 mm^2 * dblScale * dblScale
                
                double dblD = 0.2 * dblScale;  //epsilon * dblScale, dblD is the increase limit and the limit distance between two buildings
                double dblGrow = 0.4 * dblSqrtAreaLimitMap * (dblScale - dblLastScale);  //lambda/2 * dblAqrtAreaLimit * (dblScale-dblStartScale)
                //double dblhalfD = 0.1 * dblScale;  //d=0.2mm * dblScale
                //double dblGrow = 0.1 * dblScale;
                
                double dblGrowSpeed = dblGrow + dblD;  //0.35*dblScale - 0.15*dblStartScale
                //double dblHoleAreaLimit = Math.PI * dblGrowSpeed * dblGrowSpeed; //dblHoleAreaLimit=8 mm^2 * dblScale * dblScale
                double dblHoleAreaLimit = 8  * dblScale * dblScale; //dblHoleAreaLimit=8 mm^2 * dblScale * dblScale
                //double dblr = dblhalfD;  //r= 1/4 sqrt(A)= 0.1 * dblScale
                //double dblSimpR = 0.2 * dblScale;  //a building should be large enough in order to have some detailes

                MagnifiedCpgLt.ForEach(cpg => cpg.RemoveClosePoints());
                MagnifiedCpgLt.ForEach(cpg => cpg.FormCEdgeLt());
                MagnifiedCpgLt.ForEach(cpg => cpg.SetCEdgeLtLength());
                //CEdgeLt of a mergedcpg consists of original edges of polygons and bridges; CptLt of mergedcpg is not defined
                var MergedCpgLt = MergeCloseCpgsAndAddBridges(MagnifiedCpgLt, dblD * dblFactorClipper, dblGrow * dblFactorClipper);
                var outputcpglt = new List<CPolygon>(MergedCpgLt.Count);
                foreach (var mergedcpg in MergedCpgLt)
                {
                    //mergedcpg.FormCEdgeLt();
                    outputcpglt.Add(BufferDilateErodeSimplify_OneBuilding(pParameterInitialize, mergedcpg, dblD * dblFactorClipper,
                        dblGrow * dblFactorClipper, strBufferStyle, dblMiterLimit, dblFactorClipper));
                }

                //var outputcpglt = new List<CPolygon>();
                //foreach (var cpg in MagnifiedCpgLt)
                //{
                //    outputcpglt.Add(
                //        HandleOneBuilding(pParameterInitialize, cpg, dblD * dblFactorClipper, dblDLast * dblFactorClipper, strBufferStyle, dblMiterLimit, dblFactorClipper));
                //}

                //dblA = 0;
                //dblr0 = 0.0001 * (dblScale-5000);  //r= 1/4 sqrt(A)= 0.1 * dblScale
                //if (dblScale ==20000) //at the first step,  dblr0 = 0 
                //{
                //    dblr0 = 0;
                //}
                //            double dblradius1 = dblGrow - dblGrowLast + dblSimpR;
                //            var cpglt1 = BufferAndMerge(pParameterInitialize, inputcpglt, dblradius1,
                //                strBufferStyle, dblMiterLimit, dblA, dblFactorClipper, "OverBuffering").IGeosToCGeoEB().ToList();
                //            var lyroverbuffer = CSaveFeature.SaveCGeoEb(cpglt1, esriGeometryType.esriGeometryPolygon,
                //                 dblScale + "_Step1_OverBuffering" + dblradius1 + "_" + pParameterInitialize.pFLayerLt[0].Name + CHelpFunc.GetTimeStampWithPrefix(),
                //                 pParameterInitialize, intRed: 255);


                //            double dblradius2 = -dblSimpR - dblSimpR;
                //            var cpglt2 = BufferAndMerge(pParameterInitialize, cpglt1, dblradius2,
                //                strBufferStyle, dblMiterLimit, dblA, dblFactorClipper, "SubSideBuffering").IGeosToCGeoEB().ToList();
                //            CSaveFeature.SaveCGeoEb(cpglt2, esriGeometryType.esriGeometryPolygon,
                //dblScale + "_Step2_SubSideBuffering" + dblradius2 + "_" + pParameterInitialize.pFLayerLt[0].Name + CHelpFunc.GetTimeStampWithPrefix(),
                //pParameterInitialize, intBlue: 255);

                //            //            double dblradius3 = -(dblr - dblr0) / 10;
                //            //            var cpglt3 = BufferAndMerge(pParameterInitialize, cpglt2, dblradius3,
                //            //                strBufferStyle, dblMiterLimit, dblA, dblFactorClipper, "Erosion").IGeosToCGeoEB().ToList();
                //            //            CSaveFeature.SaveCGeoEb(cpglt3, esriGeometryType.esriGeometryPolygon,
                //            //dblScale + "_Step3_Erosion" + dblradius3 + "_" + pParameterInitialize.pFLayerLt[0].Name + CHelpFunc.GetTimeStampWithPrefix(),
                //            //pParameterInitialize, intRed: 255, intBlue: 255);

                //            double dblradius4 = dblSimpR;
                //            var outputcpglt = BufferAndMerge(pParameterInitialize, cpglt2, dblradius4,
                //                strBufferStyle, dblMiterLimit, dblA, dblFactorClipper, "Compensation").IGeosToCGeoEB().ToList();
                //            CSaveFeature.SaveCGeoEb(outputcpglt, esriGeometryType.esriGeometryPolygon,
                // dblScale + "_Step4(Output)_Compensation" + dblradius4 + "_" + pParameterInitialize.pFLayerLt[0].Name + CHelpFunc.GetTimeStampWithPrefix(),
                //pParameterInitialize, intGreen: 255);

                var scaledbackcptEb = ScaleCpgLt(outputcpglt, 1 / dblFactorClipper);
                var largecpgeb = GetLargeCpgEb(scaledbackcptEb, dblAreaLimit, dblHoleAreaLimit);

                CSaveFeature.SaveCGeoEb(largecpgeb, esriGeometryType.esriGeometryPolygon,
     dblScale + "k_Enlarge" + dblD + "m_" + CHelpFunc.GetTimeStampWithPrefix(),
    pParameterInitialize, intGreen: 255);

                MagnifiedCpgLt = outputcpglt;
                //dblGrowLast = dblGrow;
                //dblDLast = dblD;
                dblLastScale = dblScale;
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
                ////pSimplifyBuilding.simplification_tolerance = dblD;
                ////pSimplifyBuilding.minimum_area = dblA;
                ////pSimplifyBuilding.CheckConflict = "CHECK_CONFLICT ";

                ////GP.Execute(pSimplifyBuilding,null);
                #endregion



            }

            CConstants.dblVerySmallCoord /= dblFactorClipper;

        }

        private IEnumerable<CPolygon> GetLargeCpgEb(IEnumerable<CPolygon> cpgeb, double dblAreaLimit, double dblHoleAreaLimit)
        {
            //dblHoleArea = 0;
            foreach (var cpg in cpgeb)
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
                    yield return cpg;
                }
            }
        }


        //    #region GroupCloseCpgsAndAddBridges
        //    private List<CPolygon> MergeCloseCpgsAndAddBridges(List<CPolygon> cpglt, double dblD, double dblGrowR)
        //    {
        //        //var MayCloseCpgPairSS = GetMayCloseCpgPairSS(cpglt, dblD, dblGrowR);
        //        //var CloseInfoSDLt = GetCloseCpgPairsInfo(cpglt, MayCloseCpgPairSS, dblD, dblGrowR);

        //        var CloseInfoSDLt = GetCloseCpgPairsInfo(cpglt, dblD, dblGrowR);
        //        var GroupedCpgsAndBridgesEb = GroupCpgsAndBridges(cpglt, CloseInfoSDLt).ToList();
        //        return MergeGroupedCpgsAndBridgesEb(GroupedCpgsAndBridgesEb).ToList();
        //    }

        //    private static List<SortedDictionary<CPairValIncrease<CPolygon>, List<CptEdgeDis>>> GetCloseCpgPairsInfo(
        //List<CPolygon> cpglt, double dblD, double dblGrowR)
        //    {
        //        var dblCloseDis = dblD + 2 * dblGrowR;
        //        cpglt.SetIndexID();

        //        var cedgelt = new List<CEdge>();
        //        var CEdgeCpgSD = new SortedDictionary<CEdge, CPolygon>();
        //        foreach (var cpg in cpglt)
        //        {
        //            foreach (var cedge in cpg.CEdgeLt)
        //            {
        //                cedgelt.Add(cedge);
        //                CEdgeCpgSD.Add(cedge, cpg);
        //            }
        //        }

        //        var CloseCpgPairPtEdgeDisSD = new SortedDictionary<CPairValIncrease<CPolygon>, List< CptEdgeDis>>();
        //        foreach (var pEdgeRelation in CGeoFunc.DetectCEdgeRelations(cedgelt, dblCloseDis))
        //        {
        //            if (pEdgeRelation.pEnumDisMode == CEnumDisMode.InIn)
        //            {
        //                throw new ArgumentException("polygons intersect with each other!");
        //            }

        //            CPolygon cpg1;
        //            CPolygon cpg2;
        //            CEdgeCpgSD.TryGetValue(pEdgeRelation.CEdge1, out cpg1);
        //            CEdgeCpgSD.TryGetValue(pEdgeRelation.CEdge2, out cpg2);

        //            if (cpg1.GID != cpg2.GID)
        //            {
        //                List<CptEdgeDis> cptEdgeDisLt;
        //                var cpgpair = new CPairValIncrease<CPolygon>(cpg1, cpg2);
        //                if (CloseCpgPairPtEdgeDisSD.TryGetValue(cpgpair, out cptEdgeDisLt))
        //                {
        //                    cptEdgeDisLt.Add(pEdgeRelation.cptEdgeDis);
        //                }
        //                else
        //                {
        //                    cptEdgeDisLt = new List<CptEdgeDis> { pEdgeRelation.cptEdgeDis };
        //                    CloseCpgPairPtEdgeDisSD.Add(cpgpair, cptEdgeDisLt);
        //                }
        //            }
        //        }

        //        var CloseInfoSDLt = new List<SortedDictionary<CPairValIncrease<CPolygon>, List<CptEdgeDis>>>(cpglt.Count);
        //        CloseInfoSDLt.EveryElementValue(null);
        //        foreach (var CloseCpgPairPtEdgeDisKvp in CloseCpgPairPtEdgeDisSD)
        //        {
        //            var cpgpair = CloseCpgPairPtEdgeDisKvp.Key;
        //            var cpg1 = cpgpair.val1;
        //            var cpg2 = cpgpair.val2;
        //            var PtEdgeDisLt = CloseCpgPairPtEdgeDisKvp.Value;

        //            if (CloseInfoSDLt[cpg1.indexID] == null && CloseInfoSDLt[cpg2.indexID] == null)
        //            {
        //                CloseInfoSDLt[cpg1.indexID] = new SortedDictionary<CPairValIncrease<CPolygon>, List<CptEdgeDis>>();
        //                CloseInfoSDLt[cpg1.indexID].Add(cpgpair, PtEdgeDisLt);
        //                CloseInfoSDLt[cpg2.indexID] = CloseInfoSDLt[cpg1.indexID];
        //            }
        //            else if (CloseInfoSDLt[cpg1.indexID] != null && CloseInfoSDLt[cpg2.indexID] == null)
        //            {
        //                CloseInfoSDLt[cpg1.indexID].Add(cpgpair, PtEdgeDisLt);
        //                CloseInfoSDLt[cpg2.indexID] = CloseInfoSDLt[cpg1.indexID];
        //            }
        //            else if (CloseInfoSDLt[cpg1.indexID] == null && CloseInfoSDLt[cpg2.indexID] != null)
        //            {
        //                CloseInfoSDLt[cpg2.indexID].Add(cpgpair, PtEdgeDisLt);
        //                CloseInfoSDLt[cpg1.indexID] = CloseInfoSDLt[cpg2.indexID];
        //            }
        //            else // if (CloseInfoSDLt[cpg1.indexID] != null && CloseInfoSDLt[cpg2.indexID] != null)
        //            {
        //                if (CloseInfoSDLt[cpg1.indexID].Keys.First().CompareTo(CloseInfoSDLt[cpg2.indexID].Keys.First()) != 0)
        //                {
        //                    if (CloseInfoSDLt[cpg1.indexID].Count >= CloseInfoSDLt[cpg2.indexID].Count)
        //                    {
        //                        AggregateCloseInfoSD(CloseInfoSDLt[cpg1.indexID], CloseInfoSDLt[cpg2.indexID], CloseInfoSDLt);
        //                    }
        //                    else
        //                    {
        //                        AggregateCloseInfoSD(CloseInfoSDLt[cpg2.indexID], CloseInfoSDLt[cpg1.indexID], CloseInfoSDLt);
        //                    }
        //                }
        //                CloseInfoSDLt[cpg1.indexID].Add(cpgpair, PtEdgeDisLt);
        //            }

        //        }

        //        return CloseInfoSDLt;
        //    }

        //    private static void AggregateCloseInfoSD(
        //       SortedDictionary<CPairValIncrease<CPolygon>, List<CptEdgeDis>> MoreInfoSD,
        //       SortedDictionary<CPairValIncrease<CPolygon>, List<CptEdgeDis>> FewerInfoSD,
        //       List<SortedDictionary<CPairValIncrease<CPolygon>, List<CptEdgeDis>>> CloseInfoSDLt)
        //    {
        //        foreach (var FewerInfoKvp in FewerInfoSD)
        //        {
        //            MoreInfoSD.Add(FewerInfoKvp.Key, FewerInfoKvp.Value);
        //            CloseInfoSDLt[FewerInfoKvp.Key.val1.indexID] = MoreInfoSD;
        //            CloseInfoSDLt[FewerInfoKvp.Key.val2.indexID] = MoreInfoSD;
        //        }
        //    }


        //    private static IEnumerable<CValPair<List<CptEdgeDis>, List<CPolygon>>> GroupCpgsAndBridges(
        //        List<CPolygon> cpglt, List<SortedDictionary<CPairValIncrease<CPolygon>, List<CptEdgeDis>>> CloseInfoSDLt)
        //    {

        //        cpglt.ForEach(cpg => cpg.isTraversed = false);

        //        for (int i = 0; i < CloseInfoSDLt.Count; i++)
        //        {
        //            var closeinforsd = CloseInfoSDLt[i];
        //            //the polygon is far away from other polygons
        //            if (closeinforsd == null)
        //            {
        //                var CptEdgeDislt = new List<CptEdgeDis>();
        //                var groupcpglt = new List<CPolygon> { cpglt[i] };                    
        //                yield return new CValPair<List<CptEdgeDis>, List<CPolygon>>(CptEdgeDislt, groupcpglt);
        //            }
        //            else
        //            {
        //                //all the polygons in this entry have been handled because of an earlier entry
        //                var FirstCpg = closeinforsd.First().Key.val1;
        //                if (FirstCpg.isTraversed == true)
        //                {
        //                    continue;
        //                }
        //                else
        //                {
        //                    yield return GroupCpgsAndBridgesForEachCluster(closeinforsd);
        //                }
        //            }                
        //        }
        //    }

        //    private static CValPair<List<CptEdgeDis>, List<CPolygon>> GroupCpgsAndBridgesForEachCluster(
        //       SortedDictionary<CPairValIncrease<CPolygon>, List<CptEdgeDis>> CloseInfoSD)
        //    {
        //        var cpgSS = new SortedSet<CPolygon>();
        //        var CptEdgeDisLt = new List<CptEdgeDis>();



        //        foreach (var closeinfo in CloseInfoSD)
        //        {
        //            cpgSS.Add(closeinfo.Key.val1);
        //            cpgSS.Add(closeinfo.Key.val2);

        //            CptEdgeDisLt.AddRange(closeinfo.Value);
        //        }

        //        return new CValPair<List<CptEdgeDis>, List<CPolygon>>(CptEdgeDisLt, cpgSS.ToList());

        //        //var closeinforsdEt = CloseInfoSD.GetEnumerator();
        //        //closeinforsdEt.MoveNext();
        //        //var Firstcloseinforsd = closeinforsdEt.Current;

        //        //var CpgCptEdgeDisLtSD = new SortedDictionary<CPolygon, List<CptEdgeDis>>();
        //        //do
        //        //{
        //        //    //add all close relations to CptEdgeDisLt
        //        //    UpdateCpgCptEdgeDisLtSD(ref CpgCptEdgeDisLtSD, closeinforsdEt.Current.Key.val1, closeinforsdEt.Current.Value);
        //        //    UpdateCpgCptEdgeDisLtSD(ref CpgCptEdgeDisLtSD, closeinforsdEt.Current.Key.val2, closeinforsdEt.Current.Value);
        //        //} while (closeinforsdEt.MoveNext());

        //        //var intCpgCount = CpgCptEdgeDisLtSD.Count;
        //        //var FirstCpg = Firstcloseinforsd.Key.val1;
        //        ////FirstCpg.isTraversed = true;  //we should not do this
        //        //var groupcpglt = new List<CPolygon>(intCpgCount);
        //        ////groupcpglt.Add(FirstCpg);  //we should not do this 
        //        //var CptEdgeDislt = new List<CptEdgeDis>(intCpgCount - 1);
        //        //var SmallestCptEdgeDis = Firstcloseinforsd.Key;
        //        //var queueSS = new SortedSet<CValPair<CptEdgeDis, CPolygon>>();
        //        //queueSS.Add(new CValPair<CptEdgeDis, CPolygon>(SmallestCptEdgeDis, FirstCpg));
        //        //do
        //        //{
        //        //    //brige polygons by shortest connections, to avoid crossed bridges
        //        //    var minCptEdgeDisCpg = queueSS.Min;
        //        //    queueSS.Remove(minCptEdgeDisCpg);

        //        //    var BridgedCpg = minCptEdgeDisCpg.val2;
        //        //    if (BridgedCpg.isTraversed == true)
        //        //    {
        //        //        continue;
        //        //    }
        //        //    BridgedCpg.isTraversed = true;
        //        //    groupcpglt.Add(BridgedCpg);
        //        //    CptEdgeDislt.Add(minCptEdgeDisCpg.val1);

        //        //    List<CptEdgeDis> CptEdgeDisLt;
        //        //    CpgCptEdgeDisLtSD.TryGetValue(BridgedCpg, out CptEdgeDisLt);

        //        //    foreach (var cptEdgeDis in CptEdgeDisLt)
        //        //    {
        //        //        CPairValIncrease<CPolygon> outPairCpg;
        //        //        if (CloseInfoSD.TryGetValue(cptEdgeDis, out outPairCpg))
        //        //        {
        //        //            if (outPairCpg.val1.isTraversed == false)
        //        //            {
        //        //                queueSS.Add(new CValPair<CptEdgeDis, CPolygon>(cptEdgeDis, outPairCpg.val1));
        //        //            }
        //        //            if (outPairCpg.val2.isTraversed == false)
        //        //            {
        //        //                queueSS.Add(new CValPair<CptEdgeDis, CPolygon>(cptEdgeDis, outPairCpg.val2));
        //        //            }
        //        //        }
        //        //        else
        //        //        {
        //        //            throw new ArgumentException("This is impossible!");
        //        //        }
        //        //    }

        //        //} while (queueSS.Count > 0 && groupcpglt.Count < intCpgCount);
        //        //CptEdgeDislt.RemoveAt(0);  //the first element has been added twice
        //        //return new CValPair<List<CptEdgeDis>, List<CPolygon>>(CptEdgeDislt, groupcpglt);
        //    }

        //    private static void UpdateCpgCptEdgeDisLtSD(
        //        ref SortedDictionary<CPolygon, List<CptEdgeDis>> CpgCptEdgeDisLtSD, CPolygon cpg, List<CptEdgeDis> cptEdgeDisLt)
        //    {
        //        List<CptEdgeDis> outCptEdgeDisLt;
        //        if (CpgCptEdgeDisLtSD.TryGetValue(cpg, out outCptEdgeDisLt))
        //        {
        //            outCptEdgeDisLt.AddRange(cptEdgeDisLt);
        //        }
        //        else
        //        {
        //            outCptEdgeDisLt = new List<CptEdgeDis> {};
        //            outCptEdgeDisLt.AddRange(cptEdgeDisLt);
        //            CpgCptEdgeDisLtSD.Add(cpg, outCptEdgeDisLt);
        //        }
        //    }

        //    private IEnumerable<CPolygon> MergeGroupedCpgsAndBridgesEb(
        //        IEnumerable<CValPair<List<CptEdgeDis>, List<CPolygon>>> GroupedCpgsAndBridgesEb)
        //    {
        //        foreach (var GroupedCpgsAndBridges in GroupedCpgsAndBridgesEb)
        //        {
        //            if (GroupedCpgsAndBridges.val2.Count == 1)  //this polygon is alone; no need to merge
        //            {
        //                yield return GroupedCpgsAndBridges.val2[0];
        //            }
        //            else  //we construct a new polygon with the help of DCEL
        //            {
        //                yield return MergeGroupedCpgsAndBridges(GroupedCpgsAndBridges);
        //            }
        //        }
        //    }

        //    private CPolygon MergeGroupedCpgsAndBridges(CValPair<List<CptEdgeDis>, List<CPolygon>> GroupedCpgsAndBridges)
        //    {
        //        var CptEdgeDisLt = GroupedCpgsAndBridges.val1;
        //        var cpglt = GroupedCpgsAndBridges.val2;
        //        cpglt.ForEach(cpg => cpg.isTraversed = false);

        //        //some buildings may share boundaries, so we may have same edge 
        //        var CEdgeSS = new SortedSet<CEdge>(new CCmpCEdgeCoordinates());  //we use sortedset because we may remove some same edges
        //        cpglt.ForEach(cpg => cpg.CEdgeLt.ForEach(cedge => CEdgeSS.Add(cedge)));
        //        CGeoFunc.CheckShortEdges(CEdgeSS);

        //        var AllCEdgeLt = new List<CEdge>(CEdgeSS.Count + cpglt.Count); //the count is roughtly
        //        var CEdgeCptEdgeDisLtSD = new SortedDictionary<CEdge, List<CptEdgeDis>>();
        //        foreach (var cptEdgeDis in CptEdgeDisLt)
        //        {
        //            //if (CCmpMethods.CmpCoordDbl_VerySmall(cptEdgeDis.dblDis, 0) == 0), then the two buildings touch each other
        //            if (CCmpMethods.CmpCoordDbl_VerySmall(cptEdgeDis.dblDis, 0) != 0)
        //            {
        //                //add the bridge as an edge
        //                CEdgeSS.Add(new CEdge(cptEdgeDis.Cpt, cptEdgeDis.CptOnTargetCEdge));
        //            }

        //            ////if the smallest distance of two polygons happen to between a point and an edge
        //            ////this edge can be the closest edge of the polygon to several polygons, 
        //            ////so we need to split the edge into several edges
        //            //if (cptEdgeDis.t != 0 && cptEdgeDis.t != 1)
        //            //{
        //            //    List<CptEdgeDis> outCptEdgeDislt;
        //            //    if (CEdgeCptEdgeDisLtSD.TryGetValue(cptEdgeDis.TargetCEdge, out outCptEdgeDislt) == false)
        //            //    {
        //            //        outCptEdgeDislt = new List<CptEdgeDis>(1);
        //            //        outCptEdgeDislt.Add(cptEdgeDis);
        //            //        CEdgeCptEdgeDisLtSD.Add(cptEdgeDis.TargetCEdge, outCptEdgeDislt);
        //            //    }
        //            //    else
        //            //    {
        //            //        outCptEdgeDislt.Add(cptEdgeDis);
        //            //    }
        //            //}
        //        }

        //        //AllCEdgeLt.AddRange(CEdgeSS);

        //        var IntersectionLt = CGeoFunc.DetectIntersections(CEdgeSS.ToList(), false, true, true);
        //        var C SortedSet


        //        var CEdgeBreakPointSDSD = new SortedDictionary<CEdge, SortedSet<CptEdgeDis>>();
        //        foreach (var cedgeRelation in CEdgeRelationLt)
        //        {
        //            if (cedgeRelation.cptEdgeDis == 0)
        //            {
        //                if (cedgeRelation.pEnumDisMode == CEnumDisMode.)
        //                {

        //                }
        //            }
        //        }






        //        var CEdgeRelationLt = CGeoFunc.DetectCEdgeRelations(CEdgeSS.ToList(), CConstants.dblVerySmallCoord);

        //        var CEdgeCptEdgeDisSSSD = new SortedDictionary<CEdge, SortedSet<CptEdgeDis>>();
        //        foreach (var cedgeRelation in CEdgeRelationLt)
        //        {
        //            if (cedgeRelation.cptEdgeDis==0)
        //            {
        //                if (cedgeRelation.pEnumDisMode==CEnumDisMode.)
        //                {

        //                }
        //            }
        //        }






        //        ////We need to split the edge into several edges
        //        //foreach (var CEdgeCptEdgeDisLt in CEdgeCptEdgeDisLtSD)
        //        //{
        //        //    var TargetCEdge = CEdgeCptEdgeDisLt.Key;
        //        //    if (CEdgeSS.Remove(TargetCEdge) == false)
        //        //    {
        //        //        throw new ArgumentException("This case should not be possible!");
        //        //    }

        //        //    AllCEdgeLt.AddRange(GenerateNewCEdgeEb(TargetCEdge, CEdgeCptEdgeDisLt.Value));
        //        //}
        //        //CGeoFunc.CheckShortEdges(AllCEdgeLt);
        //        //if (CGeoFunc.ExistDuplicate(AllCEdgeLt, new CCmpCEdgeCoordinates()))
        //        //{
        //        //    throw new ArgumentException("There are duplicated edges!");
        //        //}






        //        var holecpglt = new List<CPolygon>();
        //        foreach (var cpg in cpglt)
        //        {
        //            if (cpg.HoleCpgLt != null)
        //            {
        //                holecpglt.AddRange(cpg.HoleCpgLt);
        //            }
        //        }

        //        var mergedcpg = new CPolygon(cpglt[0].ID);
        //        mergedcpg.CEdgeLt = AllCEdgeLt;
        //        mergedcpg.HoleCpgLt = holecpglt;
        //        return mergedcpg;

        //        //CSaveFeature.SaveCGeoEb(ScaleCEdgeEb(AllCEdgeLt, 1 / _dblFactorClipper), esriGeometryType.esriGeometryPolyline,
        //        //    "TestEdgeLt" + CHelpFunc.GetTimeStampWithPrefix(), _ParameterInitialize);


        //        //CDCEL pDCEL = new CGeometry.CDCEL(AllCEdgeLt);
        //        //pDCEL.ConstructDCEL();


        //        //return new CPolygon(cpglt[0].ID, pDCEL.FaceCpgLt[0].GetOnlyInnerCptLt(), holecpglt);
        //    }




        //    private static IEnumerable<CEdge> GenerateNewCEdgeEb(CEdge TargetCEdge, List<CptEdgeDis> CptEdgeDisLt)
        //    {
        //        var cpteb = CGeoFunc.RemoveClosePointsForCptEb(GetOrderedCptEb(TargetCEdge, CptEdgeDisLt));

        //        var cpter = cpteb.GetEnumerator();
        //        cpter.MoveNext();
        //        var precpt = cpter.Current;
        //        while (cpter.MoveNext())
        //        {
        //            if (CCmpMethods.CmpCptXY(precpt, cpter.Current) == 0)
        //            {
        //                throw new ArgumentException("small edge!");
        //            }
        //            yield return new CEdge(precpt, cpter.Current);
        //            precpt = cpter.Current;
        //        }
        //    }

        //    private static IEnumerable<CPoint> GetOrderedCptEb(CEdge TargetCEdge, List<CptEdgeDis> CptEdgeDisLt)
        //    {
        //        CptEdgeDisLt.Sort(CCmpCptEdgeDis_T.pCmpCptEdgeDis_T);

        //        yield return TargetCEdge.FrCpt;
        //        foreach (var cptEdgeDis in CptEdgeDisLt)
        //        {
        //            yield return cptEdgeDis.CptOnTargetCEdge;
        //        }
        //        yield return TargetCEdge.ToCpt;
        //    }

        //    #endregion

        #region GroupCloseCpgsAndAddBridges, probably, non-crossing bridges (unfinished)
        //        private List<CPolygon> MergeCloseCpgsAndAddBridges(List<CPolygon> cpglt, double dblD, double dblGrowR)
        //        {
        //            var CloseInfoSDLt = GetCloseCpgPairsInfo(cpglt, dblD, dblGrowR);
        //            var GroupedCpgsAndBridgesEb = GroupCpgsAndBridges(cpglt, CloseInfoSDLt).ToList();
        //            return MergeGroupedCpgsAndBridgesEb(GroupedCpgsAndBridgesEb).ToList();
        //        }        

        //        private static List<SortedDictionary<CPairValIncrease<CPolygon>, SortedSet<CptEdgeDis>>> GetCloseCpgPairsInfo(
        //    List<CPolygon> cpglt, double dblD, double dblGrowR)
        //        {
        //            var dblCloseDis = dblD + 2 * dblGrowR;
        //            cpglt.SetIndexID();

        //            var cedgelt = new List<CEdge>();
        //            var CEdgeCpgSD = new SortedDictionary<CEdge, CPolygon>();
        //            foreach (var cpg in cpglt)
        //            {
        //                foreach (var cedge in cpg.CEdgeLt)
        //                {
        //                    cedgelt.Add(cedge);
        //                    CEdgeCpgSD.Add(cedge, cpg);
        //                }
        //            }

        //            var CloseCpgPairPtEdgeDisSD = new SortedDictionary<CPairValIncrease<CPolygon>, SortedSet< CptEdgeDis>>();
        //            foreach (var pEdgeRelation in CGeoFunc.DetectCEdgeRelations(cedgelt, dblCloseDis))
        //            {
        //                if (pEdgeRelation.pEnumDisMode == CEnumDisMode.InIn)
        //                {
        //                    throw new ArgumentException("polygons intersect with each other!");
        //                }

        //                CPolygon cpg1;
        //                CPolygon cpg2;
        //                CEdgeCpgSD.TryGetValue(pEdgeRelation.CEdge1, out cpg1);
        //                CEdgeCpgSD.TryGetValue(pEdgeRelation.CEdge2, out cpg2);

        //                if (cpg1.GID != cpg2.GID)
        //                {
        //                    SortedSet<CptEdgeDis> cptEdgeDisSS;
        //                    var cpgpair = new CPairValIncrease<CPolygon>(cpg1, cpg2);
        //                    if (CloseCpgPairPtEdgeDisSD.TryGetValue(cpgpair, out cptEdgeDisSS))
        //                    {
        //                        cptEdgeDisSS.Add(pEdgeRelation.cptEdgeDis);
        //                    }
        //                    else
        //                    {
        //                        cptEdgeDisSS = new SortedSet<CptEdgeDis>();
        //                        cptEdgeDisSS.Add(pEdgeRelation.cptEdgeDis);
        //                        CloseCpgPairPtEdgeDisSD.Add(cpgpair, cptEdgeDisSS);
        //                    }
        //                }
        //            }

        //            var CloseInfoSDLt = new List<SortedDictionary<CPairValIncrease<CPolygon>, SortedSet<CptEdgeDis>>>(cpglt.Count);
        //            CloseInfoSDLt.EveryElementValue(null);
        //            foreach (var CloseCpgPairPtEdgeDisKvp in CloseCpgPairPtEdgeDisSD)
        //            {
        //                var cpgpair = CloseCpgPairPtEdgeDisKvp.Key;
        //                var cpg1 = cpgpair.val1;
        //                var cpg2 = cpgpair.val2;
        //                var PtEdgeDisSS = CloseCpgPairPtEdgeDisKvp.Value;

        //                if (CloseInfoSDLt[cpg1.indexID] == null && CloseInfoSDLt[cpg2.indexID] == null)
        //                {
        //                    CloseInfoSDLt[cpg1.indexID] = new SortedDictionary<CPairValIncrease<CPolygon>, SortedSet<CptEdgeDis>>();
        //                    CloseInfoSDLt[cpg1.indexID].Add( cpgpair, PtEdgeDisSS);
        //                    CloseInfoSDLt[cpg2.indexID] = CloseInfoSDLt[cpg1.indexID];
        //                }
        //                else if (CloseInfoSDLt[cpg1.indexID] != null && CloseInfoSDLt[cpg2.indexID] == null)
        //                {
        //                    CloseInfoSDLt[cpg1.indexID].Add(cpgpair, PtEdgeDisSS);
        //                    CloseInfoSDLt[cpg2.indexID] = CloseInfoSDLt[cpg1.indexID];
        //                }
        //                else if (CloseInfoSDLt[cpg1.indexID] == null && CloseInfoSDLt[cpg2.indexID] != null)
        //                {
        //                    CloseInfoSDLt[cpg2.indexID].Add(cpgpair, PtEdgeDisSS);
        //                    CloseInfoSDLt[cpg1.indexID] = CloseInfoSDLt[cpg2.indexID];
        //                }
        //                else // if (CloseInfoSDLt[cpg1.indexID] != null && CloseInfoSDLt[cpg2.indexID] != null)
        //                {
        //                    if (CloseInfoSDLt[cpg1.indexID].Keys.First().CompareTo(CloseInfoSDLt[cpg2.indexID].Keys.First()) != 0)
        //                    {
        //                        if (CloseInfoSDLt[cpg1.indexID].Count >= CloseInfoSDLt[cpg2.indexID].Count)
        //                        {
        //                            AggregateCloseInfoSD(CloseInfoSDLt[cpg1.indexID], CloseInfoSDLt[cpg2.indexID], CloseInfoSDLt);
        //                        }
        //                        else
        //                        {
        //                            AggregateCloseInfoSD(CloseInfoSDLt[cpg2.indexID], CloseInfoSDLt[cpg1.indexID], CloseInfoSDLt);
        //                        }
        //                    }
        //                    CloseInfoSDLt[cpg1.indexID].Add(cpgpair, PtEdgeDisSS);
        //                }

        //            }

        //            return CloseInfoSDLt;
        //        }

        //        private static void AggregateCloseInfoSD(
        //           SortedDictionary<CPairValIncrease<CPolygon>, SortedSet<CptEdgeDis>> MoreInfoSD,
        //           SortedDictionary<CPairValIncrease<CPolygon>, SortedSet<CptEdgeDis>> FewerInfoSD,
        //           List<SortedDictionary<CPairValIncrease<CPolygon>, SortedSet<CptEdgeDis>>> CloseInfoSDLt)
        //        {
        //            foreach (var FewerInfoKvp in FewerInfoSD)
        //            {
        //                MoreInfoSD.Add(FewerInfoKvp.Key, FewerInfoKvp.Value);
        //                CloseInfoSDLt[FewerInfoKvp.Key.val1.indexID] = MoreInfoSD;
        //                CloseInfoSDLt[FewerInfoKvp.Key.val2.indexID] = MoreInfoSD;
        //            }
        //        }


        //        private static IEnumerable<CValPair<List<CptEdgeDis>, List<CPolygon>>> GroupCpgsAndBridges(
        //            List<CPolygon> cpglt, List<SortedDictionary<CPairValIncrease<CPolygon>, SortedSet<CptEdgeDis>>> CloseInfoSDLt)
        //        {

        //            cpglt.ForEach(cpg => cpg.isTraversed = false);

        //            for (int i = 0; i < CloseInfoSDLt.Count; i++)
        //            {
        //                var closeinforsd = CloseInfoSDLt[i];
        //                //the polygon is far away from other polygons
        //                if (closeinforsd == null)
        //                {
        //                    var CptEdgeDislt = new List<CptEdgeDis>();
        //                    var groupcpglt = new List<CPolygon> { cpglt[i] };
        //                    yield return new CValPair<List<CptEdgeDis>, List<CPolygon>>(CptEdgeDislt, groupcpglt);
        //                }
        //                else
        //                {
        //                    //all the polygons in this entry have been handled because of an earlier entry
        //                    var FirstCpg = closeinforsd.First().Key.val1;
        //                    if (FirstCpg.isTraversed == true)
        //                    {
        //                        continue;
        //                    }
        //                    else
        //                    {
        //                        yield return GroupCpgsAndBridgesForEachCluster(closeinforsd);
        //                    }
        //                }
        //            }
        //        }

        //        private static CValPair<List<CptEdgeDis>, List<CPolygon>> GroupCpgsAndBridgesForEachCluster(
        //           SortedDictionary<CPairValIncrease<CPolygon>, SortedSet<CptEdgeDis>> CloseInfoSD)
        //        {
        //            var closeinforsdEt = CloseInfoSD.GetEnumerator();
        //            closeinforsdEt.MoveNext();
        //            //var Firstcloseinforsd = closeinforsdEt.Current;

        //            var CpgCptEdgeDisLtSD = new SortedDictionary<CPolygon, List<CptEdgeDis>>();
        //            var minCptEdgeDisCpgPairSD = new SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>();
        //            do
        //            {
        //                //for each polygon, add all close relations to CptEdgeDisLt
        //                UpdateCpgCptEdgeDisLtSD(ref CpgCptEdgeDisLtSD, closeinforsdEt.Current.Key.val1, closeinforsdEt.Current.Value.Min);
        //                UpdateCpgCptEdgeDisLtSD(ref CpgCptEdgeDisLtSD, closeinforsdEt.Current.Key.val2, closeinforsdEt.Current.Value.Min);

        //                minCptEdgeDisCpgPairSD.Add(closeinforsdEt.Current.Value.Min, closeinforsdEt.Current.Key);
        //            } while (closeinforsdEt.MoveNext());

        //            //find the polygon related to smallest distance to other polygons
        //            var SmallestCptEdgeDis = new CptEdgeDis(double.MaxValue);
        //            CPolygon SmallestRelatedCpg = null;
        //            foreach (var CpgCptEdgeDisLtKvp in CpgCptEdgeDisLtSD)
        //            {
        //                foreach (var cptEdgeDis in CpgCptEdgeDisLtKvp.Value)
        //                {
        //                    if (cptEdgeDis.CompareTo(SmallestCptEdgeDis) == -1)
        //                    {
        //                        SmallestCptEdgeDis = cptEdgeDis;
        //                        SmallestRelatedCpg = CpgCptEdgeDisLtKvp.Key;
        //                    }
        //                }
        //            }


        //            var intCpgCount = CpgCptEdgeDisLtSD.Count;
        //            //var FirstCpg = Firstcloseinforsd.Key.val1;
        //            //FirstCpg.isTraversed = true;  //we should not do this
        //            var groupcpglt = new List<CPolygon>(intCpgCount);
        //            //groupcpglt.Add(FirstCpg);  //we should not do this 
        //            var BridgeCptEdgeDislt = new List<CptEdgeDis>(intCpgCount);
        //            //var UnusedCptEdgeDislt = new List<CptEdgeDis>(intCpgCount);
        //            //var SmallestCptEdgeDis = Firstcloseinforsd.Key;
        //            var queueSS = new SortedSet<CValPair<CptEdgeDis, CPolygon>>();
        //            queueSS.Add(new CValPair<CptEdgeDis, CPolygon>(SmallestCptEdgeDis, SmallestRelatedCpg));
        //            do
        //            {
        //                //brige polygons by shortest connections, to avoid crossed bridges
        //                var minCptEdgeDisCpg = queueSS.Min;
        //                queueSS.Remove(minCptEdgeDisCpg);

        //                var BridgedCpg = minCptEdgeDisCpg.val2;
        //                if (BridgedCpg.isTraversed == true)
        //                {
        //                    //UnusedCptEdgeDislt.Add(minCptEdgeDisCpg.val1);
        //                    continue;
        //                }
        //                BridgedCpg.isTraversed = true;
        //                groupcpglt.Add(BridgedCpg);
        //                BridgeCptEdgeDislt.Add(minCptEdgeDisCpg.val1);

        //                List<CptEdgeDis> CptEdgeDisLt;
        //                CpgCptEdgeDisLtSD.TryGetValue(BridgedCpg, out CptEdgeDisLt);

        //                foreach (var cptEdgeDis in CptEdgeDisLt)
        //                {
        //                    CPairValIncrease<CPolygon> outPairCpg;
        //                    if (minCptEdgeDisCpgPairSD.TryGetValue(cptEdgeDis, out outPairCpg))
        //                    {
        //                        if (outPairCpg.val1.isTraversed == false)
        //                        {
        //                            queueSS.Add(new CValPair<CptEdgeDis, CPolygon>(cptEdgeDis, outPairCpg.val1));
        //                        }
        //                        if (outPairCpg.val2.isTraversed == false)
        //                        {
        //                            queueSS.Add(new CValPair<CptEdgeDis, CPolygon>(cptEdgeDis, outPairCpg.val2));
        //                        }
        //                    }
        //                    else
        //                    {
        //                        throw new ArgumentException("This is impossible!");
        //                    }
        //                }

        //            } while (queueSS.Count > 0 && groupcpglt.Count < intCpgCount);
        //            BridgeCptEdgeDislt.RemoveAt(0);  //the first element has been added twice

        //            var AllCEdgeLt = new List<CEdge>();
        //            var RemainCptEdgeDisSS = new SortedSet<CptEdgeDis>();
        //            foreach (var CptEdgeDisSS in CloseInfoSD.Values)
        //            {
        //                foreach (var CptEdgeDis in CptEdgeDisSS)
        //                {
        //                    CptEdgeDis.SetConnectEdge();
        //                    RemainCptEdgeDisSS.Add(CptEdgeDis);
        //                    AllCEdgeLt.Add(CptEdgeDis.ConnectCEdge);
        //                }
        //            }

        //            BridgeCptEdgeDislt.ForEach(CptEdgeDis => RemainCptEdgeDisSS.Remove(CptEdgeDis));         
        //            groupcpglt.ForEach(cpg => AllCEdgeLt.AddRange(cpg.CEdgeLt));

        //            CGeoFunc.DetectIntersections(AllCEdgeLt, false, false, true);


        //            foreach (var RemainCptEdgeDis in RemainCptEdgeDisSS)
        //            {
        //                if (RemainCptEdgeDis.ConnectCEdge.IntersectLt.Count == 0)
        //                {
        //                    BridgeCptEdgeDislt.Add(RemainCptEdgeDis);
        //                }                
        //            }


        //            return new CValPair<List<CptEdgeDis>, List<CPolygon>>(BridgeCptEdgeDislt, groupcpglt);
        //        }

        //        private static void UpdateCpgCptEdgeDisLtSD(
        //            ref SortedDictionary<CPolygon, List<CptEdgeDis>> CpgCptEdgeDisLtSD, CPolygon cpg, CptEdgeDis cptEdgeDis)
        //        {
        //            List<CptEdgeDis> outCptEdgeDisLt;
        //            if (CpgCptEdgeDisLtSD.TryGetValue(cpg, out outCptEdgeDisLt))
        //            {
        //                outCptEdgeDisLt.Add(cptEdgeDis);
        //            }
        //            else
        //            {
        //                outCptEdgeDisLt = new List<CptEdgeDis> { cptEdgeDis };
        //                CpgCptEdgeDisLtSD.Add(cpg, outCptEdgeDisLt);
        //            }
        //        }

        //        private IEnumerable<CPolygon> MergeGroupedCpgsAndBridgesEb(
        //            IEnumerable<CValPair<List<CptEdgeDis>, List<CPolygon>>> GroupedCpgsAndBridgesEb)
        //        {
        //            foreach (var GroupedCpgsAndBridges in GroupedCpgsAndBridgesEb)
        //            {
        //                if (GroupedCpgsAndBridges.val2.Count == 1)  //this polygon is alone; no need to merge
        //                {
        //                    yield return GroupedCpgsAndBridges.val2[0];
        //                }
        //                else  //we construct a new polygon with the help of DCEL
        //                {
        //                    yield return MergeGroupedCpgsAndBridges(GroupedCpgsAndBridges);
        //                }
        //            }
        //        }

        //        private CPolygon MergeGroupedCpgsAndBridges(CValPair<List<CptEdgeDis>, List<CPolygon>> GroupedCpgsAndBridges)
        //        {
        //            var CptEdgeDisLt = GroupedCpgsAndBridges.val1;
        //            var cpglt = GroupedCpgsAndBridges.val2;
        //            cpglt.ForEach(cpg => cpg.isTraversed = false);

        //            //some buildings may share boundaries, so we may have same edge 
        //            var CEdgeSS = new SortedSet<CEdge>(new CCmpCEdgeCoordinates());  //we use sortedset because we may remove some same edges
        //            cpglt.ForEach(cpg => cpg.CEdgeLt.ForEach(cedge => CEdgeSS.Add(cedge)));
        //            CGeoFunc.CheckShortEdges(CEdgeSS);

        //            var AllCEdgeLt = new List<CEdge>(CEdgeSS.Count + cpglt.Count); //the count is roughtly
        //            var CEdgeCptEdgeDisLtSD = new SortedDictionary<CEdge, List<CptEdgeDis>>();
        //            foreach (var cptEdgeDis in CptEdgeDisLt)
        //            {
        //                //if (CCmpMethods.CmpCoordDbl_VerySmall(cptEdgeDis.dblDis, 0) == 0), then the two buildings touch each other
        //                if (CCmpMethods.CmpCoordDbl_VerySmall(cptEdgeDis.dblDis, 0) != 0)
        //                {
        //                    //add the bridge as an edge
        //                    AllCEdgeLt.Add(cptEdgeDis.ConnectCEdge);
        //                }

        //                //if the smallest distance of two polygons happen to between a point and an edge
        //                //this edge can be the closest edge of the polygon to several polygons, 
        //                //so we need to split the edge into several edges
        //                if (cptEdgeDis.t != 0 && cptEdgeDis.t != 1)
        //                {
        //                    List<CptEdgeDis> outCptEdgeDislt;
        //                    if (CEdgeCptEdgeDisLtSD.TryGetValue(cptEdgeDis.TargetCEdge, out outCptEdgeDislt) == false)
        //                    {
        //                        outCptEdgeDislt = new List<CptEdgeDis>(1);
        //                        outCptEdgeDislt.Add(cptEdgeDis);
        //                        CEdgeCptEdgeDisLtSD.Add(cptEdgeDis.TargetCEdge, outCptEdgeDislt);
        //                    }
        //                    else
        //                    {
        //                        outCptEdgeDislt.Add(cptEdgeDis);
        //                    }
        //                }
        //            }

        //            //CGeoFunc.CheckShortEdges(AllCEdgeLt);

        //            //We need to split the edge into several edges
        //            foreach (var CEdgeCptEdgeDisLt in CEdgeCptEdgeDisLtSD)
        //            {
        //                var TargetCEdge = CEdgeCptEdgeDisLt.Key;
        //                if (CEdgeSS.Remove(TargetCEdge) == false)
        //                {
        //                    throw new ArgumentException("This case should not be possible!");
        //                }

        //                AllCEdgeLt.AddRange(GenerateNewCEdgeEb(TargetCEdge, CEdgeCptEdgeDisLt.Value));
        //            }
        //            CGeoFunc.CheckShortEdges(AllCEdgeLt);
        //            //if (CGeoFunc.ExistDuplicate(AllCEdgeLt, new CCmpCEdgeCoordinates()))
        //            //{
        //            //    throw new ArgumentException("There are duplicated edges!");
        //            //}



        //            AllCEdgeLt.AddRange(CEdgeSS);


        //            var holecpglt = new List<CPolygon>();
        //            foreach (var cpg in cpglt)
        //            {
        //                if (cpg.HoleCpgLt != null)
        //                {
        //                    holecpglt.AddRange(cpg.HoleCpgLt);
        //                }
        //            }


        //            //var mergedcpg = new CPolygon(cpglt[0].ID);
        //            //mergedcpg.CEdgeLt = AllCEdgeLt;
        //            //mergedcpg.HoleCpgLt = holecpglt;
        //            //return mergedcpg;

        //            //CSaveFeature.SaveCGeoEb(ScaleCEdgeEb(AllCEdgeLt, 1 / _dblFactorClipper), esriGeometryType.esriGeometryPolyline,
        //            //    "TestEdgeLt" + CHelpFunc.GetTimeStampWithPrefix(), _ParameterInitialize);


        //            CDCEL pDCEL = new CGeometry.CDCEL(AllCEdgeLt);
        //            pDCEL.ConstructDCEL();


        //var mergedcpg = new CPolygon(cpglt[0].ID, 
        //    pDCEL.FaceCpgLt[0].GetOnlyInnerCptLt(), 
        //    pDCEL.FaceCpgLt[0].cedgeLkInnerComponents.GetFirstT().cedgeTwin.cpgIncidentFace.GetOnlyInnerCptLt());

        //            //return new CPolygon(cpglt[0].ID, pDCEL.FaceCpgLt[0].GetOnlyInnerCptLt(), holecpglt);
        //        }




        //        private static IEnumerable<CEdge> GenerateNewCEdgeEb(CEdge TargetCEdge, List<CptEdgeDis> CptEdgeDisLt)
        //        {
        //            var cpteb = CGeoFunc.RemoveClosePointsForCptEb(GetOrderedCptEb(TargetCEdge, CptEdgeDisLt));

        //            var cpter = cpteb.GetEnumerator();
        //            cpter.MoveNext();
        //            var precpt = cpter.Current;
        //            while (cpter.MoveNext())
        //            {
        //                if (CCmpMethods.CmpCptXY(precpt, cpter.Current) == 0)
        //                {
        //                    throw new ArgumentException("small edge!");
        //                }
        //                yield return new CEdge(precpt, cpter.Current);
        //                precpt = cpter.Current;
        //            }
        //        }

        //        private static IEnumerable<CPoint> GetOrderedCptEb(CEdge TargetCEdge, List<CptEdgeDis> CptEdgeDisLt)
        //        {
        //            CptEdgeDisLt.Sort(CCmpCptEdgeDis_T.pCmpCptEdgeDis_T);

        //            yield return TargetCEdge.FrCpt;
        //            foreach (var cptEdgeDis in CptEdgeDisLt)
        //            {
        //                yield return cptEdgeDis.CptOnTargetCEdge;
        //            }
        //            yield return TargetCEdge.ToCpt;
        //        }

        #endregion

        #region GroupCloseCpgsAndAddBridges single bridges
        private List<CPolygon> MergeCloseCpgsAndAddBridges(List<CPolygon> cpglt, double dblD, double dblGrowR)
        {
            //var MayCloseCpgPairSS = GetMayCloseCpgPairSS(cpglt, dblD, dblGrowR);
            //var CloseInfoSDLt = GetCloseCpgPairsInfo(cpglt, MayCloseCpgPairSS, dblD, dblGrowR);

            var CloseInfoSDLt = GetCloseCpgPairsInfo(cpglt, dblD, dblGrowR);
            var GroupedCpgsAndBridgesEb = GroupCpgsAndBridges(cpglt, CloseInfoSDLt).ToList();
            return MergeGroupedCpgsAndBridgesEb(GroupedCpgsAndBridgesEb).ToList();
        }

        #region Obsolete: GetMayCloseCpgPairSS
        //private SortedSet<CPairValIncrease<CPolygon>> GetMayCloseCpgPairSS(List<CPolygon> cpglt, double dblD, double dblGrowR)
        //{
        //    cpglt.ForEach(cpg => cpg.SetEnvelope());
        //    var dblDilatedDis = 0.5 * dblD + dblGrowR;
        //    var DilatedCEnvLt = new List<CEnvelope>(cpglt.Count);
        //    cpglt.ForEach(cpg => DilatedCEnvLt.Add(cpg.CEnv.GetDilatedCEnv(dblDilatedDis)));

        //    var CEdgeCpgSD = new SortedDictionary<CEdge, CPolygon>();
        //    for (int i = 0; i < DilatedCEnvLt.Count; i++)
        //    {
        //        var DilatedCEnv = DilatedCEnvLt[i];
        //        DilatedCEnv.SetEdges();
        //        CEdgeCpgSD.Add(DilatedCEnv.LeftCEdge, cpglt[i]);
        //        CEdgeCpgSD.Add(DilatedCEnv.LowerCEdge, cpglt[i]);
        //        CEdgeCpgSD.Add(DilatedCEnv.RightCEdge, cpglt[i]);
        //        CEdgeCpgSD.Add(DilatedCEnv.UpperCEdge, cpglt[i]);
        //    }

        //    var pIntersectionLt = CGeoFunc.DetectIntersections(CEdgeCpgSD.Keys.ToList(), true, true, true);

        //    var MayCloseCpgPairSS = new SortedSet<CPairValIncrease<CPolygon>>();
        //    foreach (var pIntersection in pIntersectionLt)
        //    {
        //        //get the polygons that may intersect with each other
        //        CPolygon cpg1;
        //        CPolygon cpg2;
        //        if (!CEdgeCpgSD.TryGetValue(pIntersection.CEdge1, out cpg1))
        //        {
        //            throw new ArgumentException("There is a problem!");
        //        }
        //        if (!CEdgeCpgSD.TryGetValue(pIntersection.CEdge2, out cpg2))
        //        {
        //            throw new ArgumentException("There is a problem!");
        //        }

        //        if (cpg1.GID != cpg2.GID)
        //        {
        //            //the same copies will be added only once without reporting problem because of SortedSet
        //            MayCloseCpgPairSS.Add(new CPairValIncrease<CPolygon>(cpg1, cpg2));
        //        }
        //    }

        //    return MayCloseCpgPairSS;
        //}

        //private static List<SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>> GetCloseCpgPairsInfo(
        //    List<CPolygon> cpglt, SortedSet<CPairValIncrease<CPolygon>> MayCloseCpgPairSS, double dblD, double dblGrowR)
        //{
        //    var dblCloseDis = dblD + 2 * dblGrowR;
        //    cpglt.SetIndexID();
        //    var CloseInfoSDLt = new List<SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>>(cpglt.Count);
        //    CloseInfoSDLt.EveryElementValue(null);
        //    foreach (var mayCloseCpgPair in MayCloseCpgPairSS)
        //    {
        //        var cpg1 = mayCloseCpgPair.val1;
        //        var cpg2 = mayCloseCpgPair.val2;

        //        //we assume that there is no intersection between polygons
        //        //we don't check whether two polygons intersect
        //        var PtEdgeDis = cpg1.CalMinDis(cpg2);

        //        if (PtEdgeDis.dblDis < dblCloseDis)
        //        {
        //            if (CloseInfoSDLt[cpg1.indexID] == null && CloseInfoSDLt[cpg2.indexID] == null)
        //            {
        //                CloseInfoSDLt[cpg1.indexID] = new SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>();
        //                //CloseInfoSDLt[cpg1.indexID].Item = new CPointer<SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>>();
        //                CloseInfoSDLt[cpg1.indexID].Add(PtEdgeDis, mayCloseCpgPair);
        //                CloseInfoSDLt[cpg2.indexID] = CloseInfoSDLt[cpg1.indexID];
        //            }
        //            else if (CloseInfoSDLt[cpg1.indexID] != null && CloseInfoSDLt[cpg2.indexID] == null)
        //            {
        //                CloseInfoSDLt[cpg1.indexID].Add(PtEdgeDis, mayCloseCpgPair);
        //                CloseInfoSDLt[cpg2.indexID] = CloseInfoSDLt[cpg1.indexID];
        //            }
        //            else if (CloseInfoSDLt[cpg1.indexID] == null && CloseInfoSDLt[cpg2.indexID] != null)
        //            {
        //                CloseInfoSDLt[cpg2.indexID].Add(PtEdgeDis, mayCloseCpgPair);
        //                CloseInfoSDLt[cpg1.indexID] = CloseInfoSDLt[cpg2.indexID];
        //            }
        //            else // if (CloseInfoSDLt[cpg1.indexID] != null && CloseInfoSDLt[cpg2.indexID] != null)
        //            {
        //                if (CloseInfoSDLt[cpg1.indexID].Keys.First().CompareTo(CloseInfoSDLt[cpg2.indexID].Keys.First()) != 0)
        //                {
        //                    if (CloseInfoSDLt[cpg1.indexID].Count >= CloseInfoSDLt[cpg2.indexID].Count)
        //                    {
        //                        AggregateCloseInfoSD(CloseInfoSDLt[cpg1.indexID], CloseInfoSDLt[cpg2.indexID], CloseInfoSDLt);
        //                    }
        //                    else
        //                    {
        //                        AggregateCloseInfoSD(CloseInfoSDLt[cpg2.indexID], CloseInfoSDLt[cpg1.indexID], CloseInfoSDLt);
        //                    }
        //                }
        //                CloseInfoSDLt[cpg1.indexID].Add(PtEdgeDis, mayCloseCpgPair);
        //            }
        //        }
        //    }

        //    return CloseInfoSDLt;
        //}
        #endregion


        private static List<SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>> GetCloseCpgPairsInfo(
    List<CPolygon> cpglt, double dblD, double dblGrowR)
        {
            //var dblCloseDis = dblD + 2 * dblGrowR;
            var dblCloseDis = dblD + 4 * dblGrowR;  //we use 4 because we allow that the buffer can be at most 2*dblGrowR away
            cpglt.SetIndexID();

            var cedgelt = new List<CEdge>();
            var CEdgeCpgSD = new SortedDictionary<CEdge, CPolygon>();
            foreach (var cpg in cpglt)
            {
                foreach (var cedge in cpg.CEdgeLt)
                {
                    cedgelt.Add(cedge);
                    CEdgeCpgSD.Add(cedge, cpg);
                }
            }

            var CloseCpgPairPtEdgeDisSD = new SortedDictionary<CPairValIncrease<CPolygon>, CptEdgeDis>();
            foreach (var pEdgeRelation in CGeoFunc.DetectCEdgeRelations(cedgelt, dblCloseDis))
            {
                if (pEdgeRelation.pEnumDisMode == CEnumDisMode.InIn)
                {
                    throw new ArgumentException("polygons intersect with each other!");
                }

                CPolygon cpg1;
                CPolygon cpg2;
                CEdgeCpgSD.TryGetValue(pEdgeRelation.CEdge1, out cpg1);
                CEdgeCpgSD.TryGetValue(pEdgeRelation.CEdge2, out cpg2);

                if (cpg1.GID != cpg2.GID)
                {
                    CptEdgeDis cptEdgeDis;
                    var cpgpair = new CPairValIncrease<CPolygon>(cpg1, cpg2);
                    if (CloseCpgPairPtEdgeDisSD.TryGetValue(cpgpair, out cptEdgeDis))
                    {
                        if (pEdgeRelation.cptEdgeDis.dblDis < cptEdgeDis.dblDis)
                        {
                            CloseCpgPairPtEdgeDisSD.Remove(cpgpair);
                            CloseCpgPairPtEdgeDisSD.Add(cpgpair, pEdgeRelation.cptEdgeDis);
                        }
                    }
                    else
                    {
                        CloseCpgPairPtEdgeDisSD.Add(cpgpair, pEdgeRelation.cptEdgeDis);
                    }
                }
            }

            var CloseInfoSDLt = new List<SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>>(cpglt.Count);
            CloseInfoSDLt.EveryElementValue(null);
            foreach (var CloseCpgPairPtEdgeDisKvp in CloseCpgPairPtEdgeDisSD)
            {
                var cpgpair = CloseCpgPairPtEdgeDisKvp.Key;
                var cpg1 = cpgpair.val1;
                var cpg2 = cpgpair.val2;
                var PtEdgeDis = CloseCpgPairPtEdgeDisKvp.Value;

                if (CloseInfoSDLt[cpg1.indexID] == null && CloseInfoSDLt[cpg2.indexID] == null)
                {
                    CloseInfoSDLt[cpg1.indexID] = new SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>();
                    CloseInfoSDLt[cpg1.indexID].Add(PtEdgeDis, cpgpair);
                    CloseInfoSDLt[cpg2.indexID] = CloseInfoSDLt[cpg1.indexID];
                }
                else if (CloseInfoSDLt[cpg1.indexID] != null && CloseInfoSDLt[cpg2.indexID] == null)
                {
                    CloseInfoSDLt[cpg1.indexID].Add(PtEdgeDis, cpgpair);
                    CloseInfoSDLt[cpg2.indexID] = CloseInfoSDLt[cpg1.indexID];
                }
                else if (CloseInfoSDLt[cpg1.indexID] == null && CloseInfoSDLt[cpg2.indexID] != null)
                {
                    CloseInfoSDLt[cpg2.indexID].Add(PtEdgeDis, cpgpair);
                    CloseInfoSDLt[cpg1.indexID] = CloseInfoSDLt[cpg2.indexID];
                }
                else // if (CloseInfoSDLt[cpg1.indexID] != null && CloseInfoSDLt[cpg2.indexID] != null)
                {
                    if (CloseInfoSDLt[cpg1.indexID].Keys.First().CompareTo(CloseInfoSDLt[cpg2.indexID].Keys.First()) != 0)
                    {
                        if (CloseInfoSDLt[cpg1.indexID].Count >= CloseInfoSDLt[cpg2.indexID].Count)
                        {
                            AggregateCloseInfoSD(CloseInfoSDLt[cpg1.indexID], CloseInfoSDLt[cpg2.indexID], CloseInfoSDLt);
                        }
                        else
                        {
                            AggregateCloseInfoSD(CloseInfoSDLt[cpg2.indexID], CloseInfoSDLt[cpg1.indexID], CloseInfoSDLt);
                        }
                    }
                    CloseInfoSDLt[cpg1.indexID].Add(PtEdgeDis, cpgpair);
                }

            }

            return CloseInfoSDLt;
        }

        private static void AggregateCloseInfoSD(
           SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>> MoreInfoSD,
           SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>> FewerInfoSD,
           List<SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>> CloseInfoSDLt)
        {
            foreach (var FewerInfoKvp in FewerInfoSD)
            {
                MoreInfoSD.Add(FewerInfoKvp.Key, FewerInfoKvp.Value);
                CloseInfoSDLt[FewerInfoKvp.Value.val1.indexID] = MoreInfoSD;
                CloseInfoSDLt[FewerInfoKvp.Value.val2.indexID] = MoreInfoSD;
            }
        }


        private static IEnumerable<CValPair<List<CptEdgeDis>, List<CPolygon>>> GroupCpgsAndBridges(
            List<CPolygon> cpglt, List<SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>>> CloseInfoSDLt)
        {

            cpglt.ForEach(cpg => cpg.isTraversed = false);

            for (int i = 0; i < CloseInfoSDLt.Count; i++)
            {
                var closeinforsd = CloseInfoSDLt[i];
                //the polygon is far away from other polygons
                if (closeinforsd == null)
                {
                    var CptEdgeDislt = new List<CptEdgeDis>();
                    var groupcpglt = new List<CPolygon> { cpglt[i] };
                    yield return new CValPair<List<CptEdgeDis>, List<CPolygon>>(CptEdgeDislt, groupcpglt);
                }
                else
                {
                    //all the polygons in this entry have been handled because of an earlier entry
                    var FirstCpg = closeinforsd.First().Value.val1;
                    if (FirstCpg.isTraversed == true)
                    {
                        continue;
                    }
                    else
                    {
                        yield return GroupCpgsAndBridgesForEachCluster(closeinforsd);
                    }
                }
            }
        }

        private static CValPair<List<CptEdgeDis>, List<CPolygon>> GroupCpgsAndBridgesForEachCluster(
           SortedDictionary<CptEdgeDis, CPairValIncrease<CPolygon>> CloseInfoSD)
        {
            var closeinforsdEt = CloseInfoSD.GetEnumerator();
            closeinforsdEt.MoveNext();
            var Firstcloseinforsd = closeinforsdEt.Current;

            var CpgCptEdgeDisLtSD = new SortedDictionary<CPolygon, List<CptEdgeDis>>();
            do
            {
                //add all close relations to CptEdgeDisLt
                UpdateCpgCptEdgeDisLtSD(ref CpgCptEdgeDisLtSD, closeinforsdEt.Current.Value.val1, closeinforsdEt.Current.Key);
                UpdateCpgCptEdgeDisLtSD(ref CpgCptEdgeDisLtSD, closeinforsdEt.Current.Value.val2, closeinforsdEt.Current.Key);
            } while (closeinforsdEt.MoveNext());

            var intCpgCount = CpgCptEdgeDisLtSD.Count;
            var FirstCpg = Firstcloseinforsd.Value.val1;
            //FirstCpg.isTraversed = true;  //we should not do this
            var groupcpglt = new List<CPolygon>(intCpgCount);
            //groupcpglt.Add(FirstCpg);  //we should not do this 
            var CptEdgeDislt = new List<CptEdgeDis>(intCpgCount - 1);
            var SmallestCptEdgeDis = Firstcloseinforsd.Key;
            var queueSS = new SortedSet<CValPair<CptEdgeDis, CPolygon>>();
            queueSS.Add(new CValPair<CptEdgeDis, CPolygon>(SmallestCptEdgeDis, FirstCpg));
            do
            {
                //brige polygons by shortest connections, to avoid crossed bridges
                var minCptEdgeDisCpg = queueSS.Min;
                queueSS.Remove(minCptEdgeDisCpg);

                var BridgedCpg = minCptEdgeDisCpg.val2;
                if (BridgedCpg.isTraversed == true)
                {
                    continue;
                }
                BridgedCpg.isTraversed = true;
                groupcpglt.Add(BridgedCpg);
                CptEdgeDislt.Add(minCptEdgeDisCpg.val1);

                List<CptEdgeDis> CptEdgeDisLt;
                CpgCptEdgeDisLtSD.TryGetValue(BridgedCpg, out CptEdgeDisLt);

                foreach (var cptEdgeDis in CptEdgeDisLt)
                {
                    CPairValIncrease<CPolygon> outPairCpg;
                    if (CloseInfoSD.TryGetValue(cptEdgeDis, out outPairCpg))
                    {
                        if (outPairCpg.val1.isTraversed == false)
                        {
                            queueSS.Add(new CValPair<CptEdgeDis, CPolygon>(cptEdgeDis, outPairCpg.val1));
                        }
                        if (outPairCpg.val2.isTraversed == false)
                        {
                            queueSS.Add(new CValPair<CptEdgeDis, CPolygon>(cptEdgeDis, outPairCpg.val2));
                        }
                    }
                    else
                    {
                        throw new ArgumentException("This is impossible!");
                    }
                }

            } while (queueSS.Count > 0 && groupcpglt.Count < intCpgCount);
            CptEdgeDislt.RemoveAt(0);  //the first element has been added twice
            return new CValPair<List<CptEdgeDis>, List<CPolygon>>(CptEdgeDislt, groupcpglt);
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

        private IEnumerable<CPolygon> MergeGroupedCpgsAndBridgesEb(
            IEnumerable<CValPair<List<CptEdgeDis>, List<CPolygon>>> GroupedCpgsAndBridgesEb)
        {
            foreach (var GroupedCpgsAndBridges in GroupedCpgsAndBridgesEb)
            {
                if (GroupedCpgsAndBridges.val2.Count == 1)  //this polygon is alone; no need to merge
                {
                    yield return GroupedCpgsAndBridges.val2[0];
                }
                else  //we construct a new polygon with the help of DCEL
                {
                    yield return MergeGroupedCpgsAndBridges(GroupedCpgsAndBridges);
                }
            }
        }

        private CPolygon MergeGroupedCpgsAndBridges(CValPair<List<CptEdgeDis>, List<CPolygon>> GroupedCpgsAndBridges)
        {
            var CptEdgeDisLt = GroupedCpgsAndBridges.val1;
            var cpglt = GroupedCpgsAndBridges.val2;
            cpglt.ForEach(cpg => cpg.isTraversed = false);

            //some buildings may share boundaries, so we may have same edge 
            var CEdgeSS = new SortedSet<CEdge>(new CCmpCEdgeCoordinates());  //we use sortedset because we may remove some same edges
            cpglt.ForEach(cpg => cpg.CEdgeLt.ForEach(cedge => CEdgeSS.Add(cedge)));
            CGeoFunc.CheckShortEdges(CEdgeSS);

            var AllCEdgeLt = new List<CEdge>(CEdgeSS.Count + cpglt.Count); //the count is roughtly
            var CEdgeCptEdgeDisLtSD = new SortedDictionary<CEdge, List<CptEdgeDis>>();
            foreach (var cptEdgeDis in CptEdgeDisLt)
            {
                //if (CCmpMethods.CmpCoordDbl_VerySmall(cptEdgeDis.dblDis, 0) == 0), then the two buildings touch each other
                if (CCmpMethods.CmpCoordDbl_VerySmall(cptEdgeDis.dblDis, 0) != 0)
                {
                    //add the bridge as an edge
                    AllCEdgeLt.Add(new CEdge(cptEdgeDis.Cpt, cptEdgeDis.CptOnTargetCEdge));
                }

                //if the smallest distance of two polygons happen to between a point and an edge
                //this edge can be the closest edge of the polygon to several polygons, 
                //so we need to split the edge into several edges
                if (cptEdgeDis.t != 0 && cptEdgeDis.t != 1)
                {
                    List<CptEdgeDis> outCptEdgeDislt;
                    if (CEdgeCptEdgeDisLtSD.TryGetValue(cptEdgeDis.TargetCEdge, out outCptEdgeDislt) == false)
                    {
                        outCptEdgeDislt = new List<CptEdgeDis>(1);
                        outCptEdgeDislt.Add(cptEdgeDis);
                        CEdgeCptEdgeDisLtSD.Add(cptEdgeDis.TargetCEdge, outCptEdgeDislt);
                    }
                    else
                    {
                        outCptEdgeDislt.Add(cptEdgeDis);
                    }
                }
            }

            //CGeoFunc.CheckShortEdges(AllCEdgeLt);

            //We need to split the edge into several edges
            foreach (var CEdgeCptEdgeDisLt in CEdgeCptEdgeDisLtSD)
            {
                var TargetCEdge = CEdgeCptEdgeDisLt.Key;
                if (CEdgeSS.Remove(TargetCEdge) == false)
                {
                    throw new ArgumentException("This case should not be possible!");
                }

                AllCEdgeLt.AddRange(GenerateNewCEdgeEb(TargetCEdge, CEdgeCptEdgeDisLt.Value));
            }
            CGeoFunc.CheckShortEdges(AllCEdgeLt);
            //if (CGeoFunc.ExistDuplicate(AllCEdgeLt, new CCmpCEdgeCoordinates()))
            //{
            //    throw new ArgumentException("There are duplicated edges!");
            //}



            AllCEdgeLt.AddRange(CEdgeSS);


            var holecpglt = new List<CPolygon>();
            foreach (var cpg in cpglt)
            {
                if (cpg.HoleCpgLt != null)
                {
                    holecpglt.AddRange(cpg.HoleCpgLt);
                }
            }


            //var mergedcpg = new CPolygon(cpglt[0].ID);
            //mergedcpg.CEdgeLt = AllCEdgeLt;
            //mergedcpg.HoleCpgLt = holecpglt;
            //return mergedcpg;

            //CSaveFeature.SaveCGeoEb(ScaleCEdgeEb(AllCEdgeLt, 1 / _dblFactorClipper), esriGeometryType.esriGeometryPolyline,
            //    "TestEdgeLt" + CHelpFunc.GetTimeStampWithPrefix(), _ParameterInitialize);


            CDCEL pDCEL = new CGeometry.CDCEL(AllCEdgeLt);
            pDCEL.ConstructDCEL();


            return new CPolygon(cpglt[0].ID, pDCEL.FaceCpgLt[0].GetOnlyInnerCptLt(), holecpglt);
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
            CptEdgeDisLt.Sort(CCmpCptEdgeDis_T.pCmpCptEdgeDis_T);

            yield return TargetCEdge.FrCpt;
            foreach (var cptEdgeDis in CptEdgeDisLt)
            {
                yield return cptEdgeDis.CptOnTargetCEdge;
            }
            yield return TargetCEdge.ToCpt;
        }

        #endregion

        #region HandleOneBuilding
        /// <summary>
        /// 
        /// </summary>
        /// <param name="mergedcpg">CEdgeLt of a mergedcpg consists of original edges of polygons and bridges; 
        /// CptLt of mergedcpg is not defined</param>
        /// <param name="dblD"></param>
        /// <param name="dblDlast"></param>
        /// <returns></returns>
        /// <remarks>decide when to generate edgelt</remarks>
        private CPolygon BufferDilateErodeSimplify_OneBuilding(CParameterInitialize pParameterInitialize, CPolygon mergedcpg,
            double dblD, double dblGrowR, string strBufferStyle, double dblMiterLimit, double dblFactorClipper)
        {
            var newcpg = new CPolygon(mergedcpg.ID);
            BufferDilateErodeSimplify_OneComponent(pParameterInitialize, ref newcpg, mergedcpg, dblD,
                        dblGrowR, strBufferStyle, dblMiterLimit, dblFactorClipper);

            if (mergedcpg.HoleCpgLt != null)
            {
                if (newcpg.HoleCpgLt == null)
                {
                    newcpg.HoleCpgLt = new List<CPolygon>(mergedcpg.HoleCpgLt.Count);
                }
                foreach (var holecpg in mergedcpg.HoleCpgLt)
                {
                    BufferDilateErodeSimplify_OneComponent(pParameterInitialize, ref newcpg, holecpg, dblD,
                        dblGrowR, strBufferStyle, dblMiterLimit, dblFactorClipper);
                }
            }
            return newcpg;
        }

        private void BufferDilateErodeSimplify_OneComponent(CParameterInitialize pParameterInitialize, ref CPolygon newcpg, CPolygon componentCpg,
            double dblD, double dblGrowR, string strBufferStyle, double dblMiterLimit, double dblFactorClipper)
        {
            double dblHoleIndicator = 1;
            if (componentCpg.IsHole == true)
            {
                dblHoleIndicator = -1;
            }

            var inputpaths = GeneratePathsByCpgExterior(componentCpg).ToList();

            //CSaveFeature.SaveCEdgeEb(ScaleCEdgeEb(cpg.CEdgeLt, 1 / _dblFactorClipper), "CEdge" + CHelpFunc.GetTimeStampWithPrefix(), pParameterInitialize);


            //var inputpaths = GeneratePathsByCEdgeLt(cpg.CEdgeLt).ToList();
            double dblOverDilated = dblD/2; //To avoid breaking the polygon when we errod, dblOverDilated should not be too large 
            var Paths = ClipperOffset_Paths(inputpaths, dblHoleIndicator*( dblGrowR + dblOverDilated), strBufferStyle, dblMiterLimit);
            //if (_intI==3)
            //{
            //    //save the enlarged
            //    //var enlargedpaths = Clipper.ClosedPathsFromPolyTree(pPolyTree);
            //    var pathsCptEbEb_Raw = ConvertPathsToCptEbEb(Paths, 1);
            //    var pathsCptEbEb = CGeoFunc.RemoveClosePointsForCptEbEb(pathsCptEbEb_Raw, false);
            //    var scaledpathsCptEbEb = ScaleCptEbEb(pathsCptEbEb, 1 / dblFactorClipper);
            //    CSaveFeature.SaveCGeoEb(GenerateCplEbByPathCptEbEb(scaledpathsCptEbEb), esriGeometryType.esriGeometryPolyline,
            //     "OverDilated_" + dblD + "_meters_" + CHelpFunc.GetTimeStamp(),
            //    pParameterInitialize);
            //}


            var erosionpaths = ClipperOffset_Paths(Paths, -2* dblHoleIndicator * dblOverDilated, strBufferStyle, dblMiterLimit);
            //if (_intI == 3)
            //{
            //    //save the enlarged
            //    //var erosionpaths = Clipper.ClosedPathsFromPolyTree(polytree);
            //    var pathsCptEbEb_Raw2 = ConvertPathsToCptEbEb(erosionpaths, 1);
            //    var pathsCptEbEb2 = CGeoFunc.RemoveClosePointsForCptEbEb(pathsCptEbEb_Raw2, false);
            //    var scaledpathsCptEbEb2 = ScaleCptEbEb(pathsCptEbEb2, 1 / dblFactorClipper);
            //    CSaveFeature.SaveCGeoEb(GenerateCplEbByPathCptEbEb(scaledpathsCptEbEb2), esriGeometryType.esriGeometryPolyline,
            //     "Erosion_" + dblD + "_meters_" + CHelpFunc.GetTimeStamp(),
            //    pParameterInitialize);
            //}

            var polytree = ClipperOffset_PolyTree(erosionpaths, dblHoleIndicator*dblOverDilated, strBufferStyle, dblMiterLimit);
            //if (_intI == 3)
            //{
            //    //save the enlarged
            //    var okpaths = Clipper.ClosedPathsFromPolyTree(polytree);
            //    var pathsCptEbEb_Raw3 = ConvertPathsToCptEbEb(okpaths, 1);
            //    var pathsCptEbEb3 = CGeoFunc.RemoveClosePointsForCptEbEb(pathsCptEbEb_Raw3, false);
            //    var scaledpathsCptEbEb3 = ScaleCptEbEb(pathsCptEbEb3, 1 / dblFactorClipper);
            //    CSaveFeature.SaveCGeoEb(GenerateCplEbByPathCptEbEb(scaledpathsCptEbEb3), esriGeometryType.esriGeometryPolyline,
            //     "Fine_" + dblD + "_meters_" + CHelpFunc.GetTimeStamp(),
            //    pParameterInitialize);
            //}



            ////save the enlarged
            //var pathsCptEbEb_Raw = ConvertPathsToCptEbEb(Paths, 1);
            //var pathsCptEbEb = CGeoFunc.RemoveClosePointsForCptEbEb(pathsCptEbEb_Raw, false);
            //var pathsCptLtLt = ScaleCptEbEb(pathsCptEbEb, 1 / dblFactorClipper).ToLtLt();
            //CSaveFeature.SaveCGeoEb(GenerateCplEbByPathCptEbEb(pathsCptLtLt), esriGeometryType.esriGeometryPolyline,
            // "OverDilated_" + dblD + "_meters_" + CHelpFunc.GetTimeStamp(),
            //pParameterInitialize);


            //******************************edges fewer than four?
            SimplifyFreeEdges(polytree, ref newcpg, componentCpg, dblD);
        }

        private  List<List<IntPoint>> ClipperOffset_Paths(List<List<IntPoint>> inputpaths, double dbldelta, string strBufferStyle, 
            double dblMiterLimit)
        {
            //keep in mind that the first point and the last point of a path are not identical
            //the direction of a path of outcome is counter-clockwise, whereas it is clockwise for a IPolygon
            ClipperOffset pClipperOffset = new ClipperOffset();            
            switch (strBufferStyle)
            {
                case "Miter":
                    //Property MiterLimit sets the maximum distance in multiples of delta that vertices can be offset from 
                    //their original positions before squaring is applied. (Squaring truncates a miter by 'cutting it off' 
                    //at 1 × delta distance from the original vertex.)
                    pClipperOffset.MiterLimit = dblMiterLimit;
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

            var Paths = new List<List<IntPoint>>();
            pClipperOffset.Execute(ref Paths, dbldelta);

            return Paths;
        }

        private PolyTree ClipperOffset_PolyTree(List<List<IntPoint>> inputpaths, double dbldelta, string strBufferStyle,
            double dblMiterLimit)
        {
            //keep in mind that the first point and the last point of a path are not identical
            //the direction of a path of outcome is counter-clockwise, whereas it is clockwise for a IPolygon
            ClipperOffset pClipperOffset = new ClipperOffset();
            switch (strBufferStyle)
            {
                case "Miter":
                    //Property MiterLimit sets the maximum distance in multiples of delta that vertices can be offset from 
                    //their original positions before squaring is applied. (Squaring truncates a miter by 'cutting it off' 
                    //at 1 × delta distance from the original vertex.)
                    pClipperOffset.MiterLimit = dblMiterLimit;
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




        private void SimplifyFreeEdges(PolyTree pPolyTree, ref CPolygon newcpg,  CPolygon componentCpg, double dblD)
        {

            //var childnode = pPolyTree.Childs[0];
            //var cptlt = ContourToCptEb(childnode.Contour, true).ToList();
            //var holecptlt = GetHoleCptLtEb(childnode, true).ToLtLt();
            //var holeChildren=pChild.Childs

            //add ConflictCEdgeLt
            componentCpg.FormCEdgeLt();
            var ConflictCEdgeLt = new List<CEdge>(componentCpg.CEdgeLt.Count);
            ConflictCEdgeLt.AddRange(componentCpg.CEdgeLt);
            componentCpg.CorrCGeoLt = new List<CPolygon>(1);
            if (componentCpg.IsHole == false)
            {
                var childnode = pPolyTree.Childs[0];
                var cptlt = ContourToCptEb(childnode.Contour, true).ToList();
                var holecptlt = GetOnlyLevelCptLtEb(childnode, true).ToLtLt();

                var dilatedcpg = new CPolygon(componentCpg.ID, cptlt, holecptlt);
                dilatedcpg.SetGeometricProperties();

                ConflictCEdgeLt.AddRange(dilatedcpg.CEdgeLt);
                dilatedcpg.HoleCpgLt.ForEach(cpg => ConflictCEdgeLt.AddRange(cpg.CEdgeLt));

                var simplifiedcpg = SimplifyAccordRightAnglesAndExistEdges(dilatedcpg, ConflictCEdgeLt, dblD);
                componentCpg.CorrCGeoLt.Add(simplifiedcpg);  //for a polygon, there is only one component for clipping
                newcpg = new CPolygon(componentCpg.ID, simplifiedcpg.CptLt, simplifiedcpg.GetHoleCptLtEb());
            }
            else // if (componentCpg.IsHole == true)
            {
                var dilatedcpglt = new List<CPolygon>(pPolyTree.ChildCount);
                foreach (var cpteb in GetOnlyLevelCptLtEb(pPolyTree,true))
                {
                    var dilatedcpg = new CPolygon(componentCpg.ID, cpteb.ToList());
                    dilatedcpg.SetGeometricProperties();
                    dilatedcpg.IsHole = true;

                    dilatedcpglt.Add(dilatedcpg);
                    ConflictCEdgeLt.AddRange(dilatedcpg.CEdgeLt);
                }

                foreach (var dilatedcpg in dilatedcpglt)
                {
                    var simplifiedcpg = SimplifyAccordRightAnglesAndExistEdges(dilatedcpg, ConflictCEdgeLt, dblD);
                    componentCpg.CorrCGeoLt.Add(simplifiedcpg);  //for a hole, there maybe many components for clipping
                    newcpg.HoleCpgLt.Add(simplifiedcpg);
                }
            }
        }

        private IEnumerable<CPoint> ContourToCptEb(List<IntPoint> Contour, bool blnMakeIdentical = true)
        {
            int intCount = 0;
            for (int i = 0; i < Contour.Count; i++)
            {
                yield return new CPoint(intCount++, Contour[i].X, Contour[i].Y);
            }

            if (blnMakeIdentical == true)
            {
                yield return new CPoint(intCount++, Contour[0].X, Contour[0].Y);
            }
        }

        private IEnumerable<IEnumerable<CPoint>> GetOnlyLevelCptLtEb(PolyNode polyNode, bool blnMakeIdentical = true)
        {
            foreach (var childnode in polyNode.Childs)
            {
                if (childnode.ChildCount > 0)
                {
                    throw new ArgumentException("unconsidered case: the hole contains holes!");
                }

                yield return ContourToCptEb(childnode.Contour, true);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpg"></param>
        /// <param name="ocpg"></param>
        /// <param name="dblThreshold"></param>
        /// <returns>the first vertex and the last vertex are identical</returns>
        private CPolygon SimplifyAccordRightAnglesAndExistEdges(CPolygon cpg, List<CEdge> ConflictCEdgeLt, double dblThreshold)
        {

            //if (cpg.HoleCpgLt != null)
            //{
            //    foreach (var holecpg in cpg.HoleCpgLt)
            //    {
            //        ConflictCEdgeLt.AddRange(holecpg.CEdgeLt);
            //    }
            //}
            //if (ocpg.HoleCpgLt != null)
            //{
            //    foreach (var holecpg in ocpg.HoleCpgLt)
            //    {
            //        ConflictCEdgeLt.AddRange(holecpg.CEdgeLt);
            //    }
            //}

            //simplifiedcptlt = CGeoFunc.SetCptLtDirection(simplifiedcptlt, true, true);
            //return new CPolygon(cpg.ID, simplifiedcptlt);
            //generate new polygon
            var newcptlt = SimplifyCptltAccordRightAnglesAndExistEdges(cpg, ConflictCEdgeLt, dblThreshold);
            newcptlt= CGeoFunc.SetCptLtDirection(newcptlt, true, true);
            if (cpg.HoleCpgLt != null)
            {
                List<List<CPoint>> newholecptltlt;
                newholecptltlt = new List<List<CPoint>>(cpg.HoleCpgLt.Count);
                foreach (var holecpg in cpg.HoleCpgLt)
                {
                    var newholecptlt = SimplifyCptltAccordRightAnglesAndExistEdges(holecpg, ConflictCEdgeLt, dblThreshold);
                    newholecptlt= CGeoFunc.SetCptLtDirection(newholecptlt, false, true);
                    newholecptltlt.Add(newholecptlt);
                }
                return new CPolygon(cpg.ID, newcptlt, newholecptltlt);
            }
            else
            {
                return new CPolygon(cpg.ID, newcptlt);
            }
        }

        private List<CPoint> SimplifyCptltAccordRightAnglesAndExistEdges(CPolygon cpg, List<CEdge> ConflictCEdgeLt, double dblThreshold)
        {
            //the first vertex and the last vertex of cpg.CptLt point to the same address in memory
            var cptlt = cpg.CptLt.CopyEbWithoutFirstLastT(true, false).ToList();
            cptlt.ForEach(cpt => cpt.isCtrl = false);

            var dupcptlt = new List<CPoint>(2 * cptlt.Count);
            dupcptlt.AddRange(cptlt);
            dupcptlt.AddRange(cptlt);

            var cedgelt = cpg.CEdgeLt;
            var lastcedge = cedgelt.GetLastT();
            var pdblAngleDiffLt = cpg.dblAngleDiffLt;
            //bool isExistNonCtrl = false;
            int intIndexCtrl = -1;
            var lastcpt = cptlt.GetLastT();
            for (int i = 0; i < cptlt.Count; i++)
            {
                if ((CCmpMethods.CmpDblRange(pdblAngleDiffLt[i], CConstants.dblHalfPI, CConstants.dblFiveDegreeRad) == 0 ||
                   CCmpMethods.CmpDblRange(pdblAngleDiffLt[i], CConstants.dblThreeSecondPI, CConstants.dblFiveDegreeRad) == 0)
                   && lastcedge.dblLength >= dblThreshold && cedgelt[i].dblLength >= dblThreshold)
                {
                    lastcpt.isCtrl = true;
                    dupcptlt[i].isCtrl = true;
                    dupcptlt[i + 1].isCtrl = true;
                    intIndexCtrl = i;
                }

                lastcpt = cptlt[i];
                lastcedge = cedgelt[i];
            }

            var newcptlt = new List<CPoint>(cptlt.Count);  //newcptlt has at most cptlt.Count points
            if (intIndexCtrl == -1) //there is no control points, we perform DP algorithm to the building
            {
                newcptlt.AddRange(DPSimplifyAccordExistEdges(cpg.CptLt, ConflictCEdgeLt, dblThreshold));
            }
            else //there are some control points, we perform DP algorithm to the split polylines
            {
                int intCurrentIndex = intIndexCtrl;
                var processcptlt = new List<CPoint> { new CPoint() };
                CPoint currentcpt = dupcptlt[intCurrentIndex];
                do
                {
                    if (currentcpt.isCtrl == true)
                    {
                        if (processcptlt.Count >= 2)
                        {
                            processcptlt.Add(currentcpt);
                            newcptlt.AddRange(
                                DPSimplifyAccordExistEdges(processcptlt, ConflictCEdgeLt, dblThreshold).CopyEbWithoutFirstLastT(false, true));

                            processcptlt = new List<CPoint> { new CPoint() };
                        }
                        else
                        {
                            newcptlt.Add(currentcpt);
                        }
                        processcptlt[0] = currentcpt;  //make processcptlt ready for adding points                                          
                    }
                    else
                    {
                        processcptlt.Add(currentcpt);
                    }
                    currentcpt = dupcptlt[++intCurrentIndex];
                } while (currentcpt.GID != dupcptlt[intIndexCtrl].GID);

                newcptlt.Add(dupcptlt[intIndexCtrl]);
            }

            return newcptlt;
        }


        //*******************check the codes below***************************
        private IEnumerable<CPoint> DPSimplifyAccordExistEdges(List<CPoint> cptlt, List<CEdge> ConflictCEdgeLt, double dblThreshold)
        {
            if (cptlt.Count <= 2)
            {
                throw new ArgumentOutOfRangeException("There is no points for simplification!");
            }

            var IndexSk = new Stack<CValPair<int, int>>();
            IndexSk.Push(new CValPair<int, int>(0, cptlt.Count - 1));

            do
            {
                var StartEndVP = IndexSk.Pop();
                var intIndexDiff = StartEndVP.val2 - StartEndVP.val1;
                if (intIndexDiff > 1)  //cptlt consists of a polygline
                {
                    var cedgebaseline = new CEdge(cptlt[StartEndVP.val1], cptlt[StartEndVP.val2]);
                    var IndexDisVP = ComputeMaxIndexDisVP(cedgebaseline, cptlt, StartEndVP.val1 + 1, StartEndVP.val2 - 1);


                    //If the shortcut intersects with the original polygon or at least one point is too far away,
                    //then we should not use this shortcut                    
                    var blninsersect = BlnIntersect(cedgebaseline, ConflictCEdgeLt);
                    if (blninsersect || IndexDisVP.val2 >= dblThreshold)
                    {
                        IndexSk.Push(new CValPair<int, int>(IndexDisVP.val1, StartEndVP.val2));
                        IndexSk.Push(new CValPair<int, int>(StartEndVP.val1, IndexDisVP.val1));
                    }
                    else  //otherwise, we use this shortcut
                    {
                        yield return cptlt[StartEndVP.val1];
                    }
                }
                else if (intIndexDiff == 1)   //cptlt only has two points
                {
                    yield return cptlt[StartEndVP.val1];
                }
                else
                {
                    throw new ArgumentOutOfRangeException("I didn't consider this case before!");
                }

            } while (IndexSk.Count > 0);
            yield return cptlt.GetLastT();
        }





        private bool BlnIntersect(CEdge cedge, List<CEdge> cedgelt)
        {
            foreach (var item in cedgelt)
            {
                //item.PrintMySelf();
                if (cedge.IntersectWith(item).enumIntersectionType != CEnumIntersectionType.NoNo)
                {
                    return true;
                }
            }

            return false;
        }

        private CValPair<int, double> ComputeMaxIndexDisVP(CEdge cedge, List<CPoint> cptlt, int intStart, int intEnd)
        {
            var MaxDisVP = new CValPair<int, double>();
            MaxDisVP.val1 = intStart;
            MaxDisVP.val2 = 0;

            for (int i = intStart; i <= intEnd; i++)
            {
                double dblDis = cedge.QueryPtHeight(cptlt[i]);
                if (dblDis > MaxDisVP.val2)
                {
                    MaxDisVP.val1 = i;
                    MaxDisVP.val2 = dblDis;
                }
            }
            return MaxDisVP;
        }
        #endregion


        #region BufferAndMerge
        //private IEnumerable<IPolygon4> BufferAndMerge(CParameterInitialize pParameterInitialize, ICollection<CPolygon> CpgCol, double dblBufferRadius,
        //    string strBufferStyle, double dblMiterLimit, double dblLimitArea, double dblFactorClipper, string strName)
        //{
        //    if (CpgCol.Count == 0)
        //    {
        //        throw new ArgumentNullException("There is no input!");
        //    }


        //    #region Construct based on arcobjects
        //    //IGeometryCollection geometryBag = new GeometryBagClass();
        //    //foreach (var item in this.ObjIGeoLtLt[0].AsExpectedClass<IGeometry, object>())
        //    //{
        //    //    geometryBag.AddGeometry(item as IGeometry);
        //    //}


        //    //IBufferConstruction pBufferConstruction = new BufferConstructionClass();
        //    //IGeometryCollection pOutGeometryCollection = new GeometryBagClass();
        //    //IBufferConstructionProperties pBufferConstructionProperties = pBufferConstruction as IBufferConstructionProperties;
        //    //pBufferConstructionProperties.EndOption = esriBufferConstructionEndEnum.esriBufferFlat;
        //    //pBufferConstructionProperties.UnionOverlappingBuffers = true;
        //    //pBufferConstruction.ConstructBuffers(geometryBag as IEnumGeometry, _dblBufferRadius, pOutGeometryCollection);
        //    #endregion

        //    //keep in mind that the first point and the last point of a path are not identical
        //    //the direction of a path of outcome is counter-clockwise, whereas it is clockwise for a IPolygon
        //    ClipperOffset pClipperOffset = new ClipperOffset();
        //    var inputpaths = GeneratePathsByCpgCol(CpgCol, dblFactorClipper).ToList();
        //    switch (strBufferStyle)
        //    {
        //        case "Miter":
        //            //Property MiterLimit sets the maximum distance in multiples of delta that vertices can be offset from 
        //            //their original positions before squaring is applied. (Squaring truncates a miter by 'cutting it off' 
        //            //at 1 × delta distance from the original vertex.)
        //            pClipperOffset.MiterLimit = dblMiterLimit;
        //            pClipperOffset.AddPaths(inputpaths, JoinType.jtMiter, EndType.etClosedPolygon);
        //            break;
        //        case "Round":
        //            pClipperOffset.ArcTolerance *= dblFactorClipper;
        //            pClipperOffset.AddPaths(inputpaths, JoinType.jtRound, EndType.etClosedPolygon);
        //            break;
        //        case "Square":
        //            pClipperOffset.AddPaths(inputpaths, JoinType.jtSquare, EndType.etClosedPolygon);
        //            break;
        //        default:
        //            break;
        //    }

        //    var Paths = new List<List<IntPoint>>();
        //    pClipperOffset.Execute(ref Paths, dblBufferRadius * dblFactorClipper);
        //    var pathsCptEbEb_Raw = ConvertPathsToCptEbEb(Paths, dblFactorClipper);
        //    var pathsCptLtLt = CGeoFunc.RemoveClosePointsForCptEbEb(pathsCptEbEb_Raw, false).ToLtLt();


        //    //var 
        //    //        CSaveFeature.SaveCGeoEb(GenerateCplEbByPaths(pathsCptLtLt), esriGeometryType.esriGeometryPolyline,
        //    // "BufferLine_" + strName + "_" + pParameterInitialize.pFLayerLt[0].Name + CHelpFunc.GetTimeStamp(),
        //    //pParameterInitialize);
        //    if (pathsCptLtLt.SelectMany(cpt => cpt).AreAnyDuplicates(CCmpCptYX_VerySmall.pCmpCptYX_VerySmall))
        //    {
        //        throw new ArgumentException("There are very close points!");
        //    }


        //    Clipper pclipper = new Clipper();
        //    //pclipper.
        //    //pclipper.Execute(ClipType.ctIntersection,)

        //    var cedgelt = GenerateCEdgeEbByPathsCptEbEb(pathsCptLtLt).ToList();

        //    CDCEL pDCEL = new CDCEL(cedgelt);
        //    pDCEL.ConstructDCEL();



        //    var superface = pDCEL.FaceCpgLt[0];
        //    var CpgShowStack = new Stack<CPolygon>();
        //    foreach (var cedgeInnerComponent in superface.cedgeLkInnerComponents)
        //    {
        //        CpgShowStack.Push(cedgeInnerComponent.cedgeTwin.cpgIncidentFace);
        //    }

        //    while (CpgShowStack.Count > 0)
        //    {
        //        var CpgShow = CpgShowStack.Pop();

        //        //generate outer ring
        //        var outercptlt = CpgShow.GetOuterCptEb(true).ToList();
        //        var outerring = CGeoFunc.GenerateRingByCptlt(outercptlt);

        //        //generate the polygon with outter ring and inner rings
        //        IPolygon4 ipg = new PolygonClass();
        //        IGeometryCollection pGeoCol = ipg as IGeometryCollection;
        //        pGeoCol.AddGeometry(outerring);
        //        //add inner rings
        //        if (CpgShow.cedgeLkInnerComponents != null)
        //        {

        //            foreach (var faceCEdgeInnerComponent in CpgShow.cedgeLkInnerComponents)
        //            {
        //                var innercptlt = CpgShow.GetInnerCptEb(faceCEdgeInnerComponent, false).ToList();
        //                var innerring = CGeoFunc.GenerateRingByCptlt(innercptlt);
        //                pGeoCol.AddGeometry(innerring);

        //                //holes inteior of an inner ring: we consider these holes as independent polygons
        //                var furthercedgeLkInnerComponents = faceCEdgeInnerComponent.cedgeTwin.cpgIncidentFace.cedgeLkInnerComponents;
        //                if (furthercedgeLkInnerComponents != null)
        //                {
        //                    foreach (var furthercedgeInnerComponent in furthercedgeLkInnerComponents)
        //                    {
        //                        furthercedgeInnerComponent.cedgeTwin.PrintMySelf();
        //                        CpgShowStack.Push(furthercedgeInnerComponent.cedgeTwin.cpgIncidentFace);
        //                    }
        //                }
        //            }
        //        }

        //        ipg.SimplifyEx(true, true, true);
        //        if (strName != "Compensation" || (ipg as IArea).Area >= dblLimitArea)
        //        {
        //            yield return ipg;
        //        }

        //    }

        //}


        private static IEnumerable<IEnumerable<CPoint>> ConvertPathsToCptEbEb(List<List<IntPoint>> Paths, double dblFactorClipper)
        {
            foreach (var path in Paths)
            {
                yield return ConvertPathToCptEb(path, dblFactorClipper);
            }
        }

        private static IEnumerable<CPoint> ConvertPathToCptEb(List<IntPoint> path, double dblFactorClipper)
        {
            for (int i = 0; i < path.Count; i++)
            {
                yield return new CPoint(i, path[i].X / dblFactorClipper, path[i].Y / dblFactorClipper);
            }
        }

        //private static IEnumerable<List<IntPoint>> GeneratePathsByCpgltCedgeltVP(
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


        private static IEnumerable<List<IntPoint>> GeneratePathsByCpgCol(ICollection<CPolygon> cpgCol)
        {
            foreach (var cpg in cpgCol)
            {
                foreach (var path in GeneratePathsByCpg(cpg))
                {
                    yield return path;
                }                
            }
        }

        private static IEnumerable<List<IntPoint>> GeneratePathsByCpg(CPolygon cpg)
        {
            yield return GeneratePathEbByCptEb(cpg.CptLt).ToList();

            if (cpg.HoleCpgLt!=null)
            {
                foreach (var holecpg in cpg.HoleCpgLt)
                {
                    yield return GeneratePathEbByCptEb(holecpg.CptLt).ToList();
                }
            }
        }

        private static IEnumerable<List<IntPoint>> GeneratePathsByCEdgeLt(List<CEdge> cedgelt)
        {
            //generate a path for each edge
            foreach (var cedge in cedgelt)
            {
                yield return GeneratePathEbByCEdge(cedge).ToList();
            }
        }

        private static IEnumerable<List<IntPoint>> GeneratePathsByCpgExterior(CPolygon cpg)
        {
            yield return GeneratePathEbByCptEb(cpg.CptLt).ToList();
        }

        private static IEnumerable<IntPoint> GeneratePathEbByCptEb(IEnumerable<CPoint> cpteb)
        {
            foreach (var cpt in cpteb)
            {
                yield return new IntPoint(cpt.X , cpt.Y);
            }
        }

        private static IEnumerable<IntPoint> GeneratePathEbByCEdge(CEdge cedge)
        {
            yield return new IntPoint(cedge.FrCpt.X, cedge.FrCpt.Y);
            yield return new IntPoint(cedge.ToCpt.X, cedge.ToCpt.Y );
        }


        private static IEnumerable<CPolygon> ScaleCpgLt(IEnumerable<CPolygon> cpglt, double dblFactor)
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
                            scaledrecursivecpg.HoleCpgLt.Add(scaledholecpg);
                            //scaledholecpg.ParentCpg = scaledrecursivecpg;

                            cpgPairSK.Push(new CValPair<CPolygon, CPolygon>(holecpg, scaledholecpg));
                        }
                    }

                } while (cpgPairSK.Count > 0);

                yield return scaledcpg;
            }
        }

        private static IEnumerable<CEdge> ScaleCEdgeEb(IEnumerable<CEdge> cedgeeb, double dblFactor)
        {
            foreach (var cedge in cedgeeb)
            {
                yield return new CEdge(ScaleCpt(cedge.FrCpt, dblFactor), ScaleCpt(cedge.ToCpt, dblFactor));
            }
        }

        private static IEnumerable<IEnumerable<CPoint>> ScaleCptEbEb(IEnumerable<IEnumerable<CPoint>> cptebeb, double dblFactor)
        {
            foreach (var cpteb in cptebeb)
            {
                yield return ScaleCptEb(cpteb, dblFactor);
            }
        }

        private static IEnumerable<CPoint> ScaleCptEb(IEnumerable<CPoint> cpteb, double dblFactor)
        {
            foreach (var cpt in cpteb)
            {
                yield return ScaleCpt(cpt, dblFactor);
            }
        }

        private static CPoint ScaleCpt(CPoint cpt, double dblFactor)
        {
            return new CPoint(cpt.ID, cpt.X * dblFactor, cpt.Y * dblFactor);
        }

        private IEnumerable<CEdge> GenerateCEdgeEbByPathsCptEbEb(IEnumerable<IEnumerable<CPoint>> pathsCptEbEb)
        {
            foreach (var cptEb in pathsCptEbEb)
            {
                foreach (var cedge in CGeoFunc.FormCEdgeEb(cptEb, false))
                {
                    yield return cedge;
                }
            }
        }

        private IEnumerable<CPolyline> GenerateCplEbByPathCptEbEb(IEnumerable<IEnumerable<CPoint>> pathCptEbEb)
        {
            int intCount = 0;
            foreach (var cpteb in pathCptEbEb)
            {
                var cptlt = cpteb.ToList();
                cptlt.Add(new CPoint(cptlt.Count, cptlt[0].X, cptlt[0].Y));
                yield return new CPolyline(intCount++, cptlt);
            }
        }
        #endregion

        #endregion


    }
}
