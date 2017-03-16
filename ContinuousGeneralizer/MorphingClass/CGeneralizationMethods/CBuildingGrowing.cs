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
using ESRI.ArcGIS.DataManagementTools;


using ClipperLib;

namespace MorphingClass.CGeneralizationMethods
{
    /// <summary>
    /// Continuous Aggregation of Maps based on Dijkstra: CAMDijkstra
    /// </summary>
    /// <remarks></remarks>
    public class CBuildingGrowing : CMorphingBaseCpg
    {
        //private static double _dblFactorClipper = 100000000;
        //private double _dblBufferRadius = -2 * _dblFactorClipper;
        private void UpdateStartEnd(ref int intStart, ref int intEnd)
        {
            //intStart = 543;
            //intEnd = intStart + 1;
            //intEnd = 2;
        }

        private static int _intDigits = 6;

        private static int _intStart = 0;
        private static int _intEnd = _intStart+1;




        #region Preprocessing
        public CBuildingGrowing()
        {

        }

        public CBuildingGrowing(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolygon, CPolygon>(ParameterInitialize, 1, 0);



        }


        public void BuildingGrowing(
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

            double dblStartScale = dblLS;
            double dblr0 = 0;
            var inputcpglt = LSCpgLt;
            double dblFactorClipper = 100000000;
            int intStart = 0;
            int intEnd = intStart + intOutput;
            CHelperFunction.Displaytspb(0.5, intEnd - intStart, pParameterInitialize.tspbMain);
            for (int i = intStart; i < intEnd; i++)
            {
                double dblScale = dblStartScale + (i + 1) * 5000;
                switch (i)
                {
                    case 0:
                        dblScale = 25000;
                        break;
                    case 1:
                        dblScale = 50000;
                        break;
                    case 2:
                        dblScale = 100000;
                        break;
                    case 3:
                        dblScale = 250000;
                        break;
                    default:
                        break;
                }

                //ESRI.ArcGIS.Geoprocessing

                double dblhalfD = 0.0001 * dblScale;  //d=0.2mm * dblScale
                double dblA = 0.00000016 * dblScale * dblScale; //A=0.16 mm^2 * dblScale * dblScale
                double dblr = 0.0001 * dblScale;  //r= 1/4 sqrt(A)= 0.1 * dblScale
                //dblr0 = 0.0001 * (dblScale-5000);  //r= 1/4 sqrt(A)= 0.1 * dblScale
                //if (dblScale ==20000) //at the first step,  dblr0 = 0 
                //{
                //    dblr0 = 0;
                //}
                double dblradus1 = dblhalfD + dblr - dblr0;
                var cpgeb1 = BufferAndMerge(pParameterInitialize, inputcpglt, dblradus1, 
                    strBufferStyle, dblMiterLimit, dblA, dblFactorClipper, "OverBuffering").IGeosToCGeoEB().ToList();
                CSaveFeature.SaveCGeoEb(cpgeb1, esriGeometryType.esriGeometryPolygon,
                    dblScale + "_Step1_OverBuffering" + dblradus1+"_" + pParameterInitialize.pFLayerLt[0].Name + CHelperFunction.GetTimeStampWithPrefix(),
                    pParameterInitialize, intRed: 255);

                double dblradus2 = -dblhalfD;
                var cpgeb2 = BufferAndMerge(pParameterInitialize, cpgeb1, dblradus2, 
                    strBufferStyle, dblMiterLimit, dblA, dblFactorClipper, "SubSideBuffering").IGeosToCGeoEB().ToList();
                CSaveFeature.SaveCGeoEb(cpgeb2, esriGeometryType.esriGeometryPolygon,
    dblScale + "_Step2_SubSideBuffering" + dblradus2 + "_" + pParameterInitialize.pFLayerLt[0].Name + CHelperFunction.GetTimeStampWithPrefix(),
    pParameterInitialize, intBlue: 255);

                double dblradus3 = -(dblr - dblr0) / 10;
                var cpgeb3 = BufferAndMerge(pParameterInitialize, cpgeb2, dblradus3, 
                    strBufferStyle, dblMiterLimit, dblA, dblFactorClipper, "Erosion").IGeosToCGeoEB().ToList();
                CSaveFeature.SaveCGeoEb(cpgeb3, esriGeometryType.esriGeometryPolygon,
    dblScale + "_Step3_Erosion" + dblradus3 + "_" + pParameterInitialize.pFLayerLt[0].Name + CHelperFunction.GetTimeStampWithPrefix(),
    pParameterInitialize, intRed: 255, intBlue: 255);

                double dblradus4 = -dblradus3;
                var outputcpglt = BufferAndMerge(pParameterInitialize, cpgeb3, dblradus4, 
                    strBufferStyle, dblMiterLimit, dblA, dblFactorClipper, "Compensation").IGeosToCGeoEB().ToList();
                CSaveFeature.SaveCGeoEb(outputcpglt, esriGeometryType.esriGeometryPolygon,
     dblScale + "_Step4(Output)_Compensation" + dblradus4 + "_" + pParameterInitialize.pFLayerLt[0].Name + CHelperFunction.GetTimeStampWithPrefix(),
    pParameterInitialize, intGreen: 255);

                inputcpglt = outputcpglt;
                dblr0 = dblr;
                CHelperFunction.Displaytspb((i+1- intStart), intEnd - intStart, pParameterInitialize.tspbMain);
            }

        }


        #region BufferAndMerge
        private IEnumerable<IPolygon4> BufferAndMerge(CParameterInitialize pParameterInitialize, ICollection<CPolygon> CpgCol, double dblBufferRadius,
            string strBufferStyle, double dblMiterLimit, double dblLimitArea, double dblFactorClipper, string strName)
        {
            #region Construct based on arcobjects
            //IGeometryCollection geometryBag = new GeometryBagClass();
            //foreach (var item in this.ObjIGeoLtLt[0].AsExpectedClass<IGeometry, object>())
            //{
            //    geometryBag.AddGeometry(item as IGeometry);
            //}


            //IBufferConstruction pBufferConstruction = new BufferConstructionClass();
            //IGeometryCollection pOutGeometryCollection = new GeometryBagClass();
            //IBufferConstructionProperties pBufferConstructionProperties = pBufferConstruction as IBufferConstructionProperties;
            //pBufferConstructionProperties.EndOption = esriBufferConstructionEndEnum.esriBufferFlat;
            //pBufferConstructionProperties.UnionOverlappingBuffers = true;
            //pBufferConstruction.ConstructBuffers(geometryBag as IEnumGeometry, _dblBufferRadius, pOutGeometryCollection);
            #endregion

            //keep in mind that the first point and the last point of a path are not identical
            //the direction of a path of outcome is counter-clockwise, whereas it is clockwise for a IPolygon
            Clipper pclipper = new Clipper();
            ClipperOffset pClipperOffset = new ClipperOffset();

            var inputpaths = GeneratePathsByCpgCol(CpgCol, dblFactorClipper);
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
                    pClipperOffset.ArcTolerance *= dblFactorClipper;
                    pClipperOffset.AddPaths(inputpaths, JoinType.jtRound, EndType.etClosedPolygon);
                    break;
                case "Square":
                    pClipperOffset.AddPaths(inputpaths, JoinType.jtSquare, EndType.etClosedPolygon);
                    break;
                default:
                    break;
            }

            var Paths = new List<List<IntPoint>>();
            pClipperOffset.Execute(ref Paths, dblBufferRadius * dblFactorClipper);
            var pathsCptEbEb_Raw = ConvertPathsToCptEbEb(Paths, dblFactorClipper);
            var pathsCptLtLt = CGeometricMethods.RemoveClosePointsOnPaths(pathsCptEbEb_Raw, false).ToLtLt();


            //var 
            //        CSaveFeature.SaveCGeoEb(GenerateCplEbByPaths(pathsCptLtLt), esriGeometryType.esriGeometryPolyline,
            // "BufferLine_" + strName + "_" + pParameterInitialize.pFLayerLt[0].Name + CHelperFunction.GetTimeStamp(),
            //pParameterInitialize);
            if (pathsCptLtLt.SelectMany(cpt => cpt).AreAnyDuplicates(CCompareCptYX_VerySmall.pCompareCptYX_VerySmall))
            {
                throw new ArgumentException("There are very close points!");
            }
            

            var cedgelt = GenerateCEdgeEbByPathsCptEbEb(pathsCptLtLt).ToList();

            CDCEL pDCEL = new CDCEL(cedgelt);
            pDCEL.ConstructDCEL();
            


            var superface = pDCEL.FaceCpgLt[0];
            var CpgShowStack = new Stack<CPolygon>();
            foreach (var cedgeInnerComponent in superface.cedgeLkInnerComponents)
            {
                CpgShowStack.Push(cedgeInnerComponent.cedgeTwin.cpgIncidentFace);
            }

            while (CpgShowStack.Count > 0)
            {
                var CpgShow = CpgShowStack.Pop();

                //generate outer ring
                var outercptlt = CpgShow.GetOuterCptLt(true);
                var outerring = CGeometricMethods.GenerateRingByCptlt(outercptlt);

                //generate the polygon with outter ring and inner rings
                IPolygon4 ipg = new PolygonClass();
                IGeometryCollection pGeoCol = ipg as IGeometryCollection;
                pGeoCol.AddGeometry(outerring);
                //add inner rings
                if (CpgShow.cedgeLkInnerComponents != null)
                {



                    foreach (var faceCEdgeInnerComponent in CpgShow.cedgeLkInnerComponents)
                    {
                        var innercptlt = CpgShow.GetInnerCptLt(faceCEdgeInnerComponent, false);
                        var innerring = CGeometricMethods.GenerateRingByCptlt(innercptlt);
                        pGeoCol.AddGeometry(innerring);

                        var furthercedgeLkInnerComponents = faceCEdgeInnerComponent.cedgeTwin.cpgIncidentFace.cedgeLkInnerComponents;
                        if (furthercedgeLkInnerComponents != null)
                        {
                            foreach (var furthercedgeInnerComponent in furthercedgeLkInnerComponents)
                            {
                                furthercedgeInnerComponent.cedgeTwin.PrintMySelf();
                                CpgShowStack.Push(furthercedgeInnerComponent.cedgeTwin.cpgIncidentFace);
                            }
                        }
                    }
                }

                ipg.SimplifyEx(true, true, true);
                if (strName != "Compensation" || (ipg as IArea).Area>= dblLimitArea)
                {
                    yield return ipg;
                }
                
            }

        }

        public IEnumerable<IEnumerable<CPoint>> ConvertPathsToCptEbEb(List<List<IntPoint>> Paths, double dblFactorClipper)
        {
            var cptltlt = new List<List<double>>(Paths.Count);
            foreach (var path in Paths)
            {
                yield return ConvertPathToCptEb(path, dblFactorClipper);
            }
        }

        public IEnumerable<CPoint> ConvertPathToCptEb(List<IntPoint> path, double dblFactorClipper)
        {
            for (int i = 0; i < path.Count; i++)
            {
                yield return new CPoint(i, path[i].X/ dblFactorClipper, (double)path[i].Y / dblFactorClipper);
            }
        }

        public List<List<IntPoint>> GeneratePathsByCpgCol(ICollection<CPolygon> cpgCol, double dblFactorClipper)
        {
            var paths = new List<List<IntPoint>>(cpgCol.Count);
            foreach (var cpg in cpgCol)
            {
                //var path = new List<IntPoint>(cpg.CptLtLt[0].Count);
                foreach (var cptlt in cpg.CptLtLt)
                {
                    var path = new List<IntPoint>(cptlt.Count);
                    for (int i = 0; i < cptlt.Count-1; i++)
                    {
                        var cpt = cptlt[i];
                        path.Add(new IntPoint(cpt.X* dblFactorClipper, cpt.Y* dblFactorClipper));
                    }
                    paths.Add(path);
                }
            }
            return paths;
        }

        public IEnumerable<CEdge> GenerateCEdgeEbByPathsCptEbEb(IEnumerable<IEnumerable<CPoint>> pathsCptEbEb)
        {
            foreach (var cptEb in pathsCptEbEb)
            {
                foreach (var cedge in CGeometricMethods.FormCEdgeEb(cptEb,false))
                {
                    yield return cedge;
                }
            }
        }

        public IEnumerable<CPolyline> GenerateCplEbByPaths(IEnumerable<IEnumerable<CPoint>> pathsCptEbEb)
        {
            int intCount = 0;
            foreach (var cpteb in pathsCptEbEb)
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
