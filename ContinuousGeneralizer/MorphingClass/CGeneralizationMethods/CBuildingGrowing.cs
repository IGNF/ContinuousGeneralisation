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

using ClipperLib;

namespace MorphingClass.CGeneralizationMethods
{
    /// <summary>
    /// Continuous Aggregation of Maps based on Dijkstra: CAMDijkstra
    /// </summary>
    /// <remarks></remarks>
    public class CBuildingGrowing : CMorphingBaseCpg
    {
        private static double _dblFactorClipper = 100000000;
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
            Construct<CPolygon, CPolygon>(ParameterInitialize, 1, 0, true, _dblFactorClipper);



        }


        public void BuildingGrowing(double dblBufferRadius, string strBufferStyle, double dblMiterLimit, double dblLS, double dblSS, int intOutput)
        {
            //**********************************************//
            //I may need to do buffering based on Miterjoint in a more clever way
            //if the distance from the miter point to original line is larger than dblMiterLimit*dblBufferRadius, we, instead of calling
            //the normal square method, make a square so that the farthest distance to the original line exactly dblMiterLimit*dblBufferRadius


            //we suppose that a hole is not separated to several parts. a hole can contain other holes.

            Stopwatch pStopwatch = Stopwatch.StartNew();
            CParameterInitialize pParameterInitialize = _ParameterInitialize;

            var LSCpgLt = this.ObjCGeoLtLt[0].ToExpectedClass<CPolygon, object>().ToList();
            //var LSIpgLt = this.ObjIGeoLtLt[0].ToExpectedClass<IPolygon5, object>().ToList();
            //var SSIpgLt = this.ObjIGeoLtLt[1].ToExpectedClass<IPolygon5, object>().ToList();
            //var LSIplLt = this.ObjIGeoLtLt[2].ToExpectedClass<IPolyline5, object>().ToList();
            //var SSIplLt = this.ObjIGeoLtLt[3].ToExpectedClass<IPolyline5, object>().ToList();

            var ipgeb = ConstructBuffersForBuildings(LSCpgLt, dblBufferRadius, strBufferStyle, dblMiterLimit);


            CSaveFeature pSaveFeature = new CSaveFeature(esriGeometryType.esriGeometryPolygon, pParameterInitialize.pFLayerLt[0].Name + "_Buffer_" + CHelperFunction.GetTimeStamp(),
                pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl, fintGreen: 255);
            pSaveFeature.SaveIGeoEbToLayer(ipgeb);
            //pSaveFeature.SaveCGeoEbToLayer(GenerateCpgEbByPaths(Paths));
        }


        #region ConstructBuffersForBuildings
        private IEnumerable<IPolygon> ConstructBuffersForBuildings(ICollection<CPolygon> CpgCol, double dblBufferRadius, string strBufferStyle, double dblMiterLimit)
        {
            #region Construct based on arcobjects
            //IGeometryCollection geometryBag = new GeometryBagClass();
            //foreach (var item in this.ObjIGeoLtLt[0].ToExpectedClass<IGeometry, object>())
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

            switch (strBufferStyle)
            {
                case "Miter":
                    pClipperOffset.MiterLimit = dblMiterLimit;
                    pClipperOffset.AddPaths(GeneratePathsByCpgCol(CpgCol), JoinType.jtMiter, EndType.etClosedPolygon);
                    break;
                case "Round":
                    pClipperOffset.ArcTolerance *= _dblFactorClipper;
                    pClipperOffset.AddPaths(GeneratePathsByCpgCol(CpgCol), JoinType.jtRound, EndType.etClosedPolygon);
                    break;
                case "Square":
                    pClipperOffset.AddPaths(GeneratePathsByCpgCol(CpgCol), JoinType.jtSquare, EndType.etClosedPolygon);
                    break;
                default:
                    break;
            }

            var Paths = new List<List<IntPoint>>();
            pClipperOffset.Execute(ref Paths, dblBufferRadius * _dblFactorClipper);

            //CSaveFeature pSaveFeatureCpl = new CSaveFeature(esriGeometryType.esriGeometryPolyline, pParameterInitialize.pFLayerLt[0].Name + "_BufferLine_" + CHelperFunction.GetTimeStamp(),
            //    pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl, fintBlue: 255);
            //pSaveFeatureCpl.SaveCGeoEbToLayer(GenerateCplEbByPaths(Paths));



            var cpglt = GenerateCpgEbByPaths(Paths).ToList();
            cpglt.ForEach(cpg => cpg.FormCEdgeLtLt());
            var cedgelt = new List<CEdge>();
            cpglt.ForEach(cpg => cpg.CEdgeLtLt.ForEach(cpgcedgelt => cedgelt.AddRange(cpgcedgelt)));

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
                var outercptlt = CpgShow.GetOuterCptLt(true);
                var outerring = CGeometricMethods.GenerateRingByCptlt(outercptlt,_dblFactorClipper);

                IPolygon ipg = new PolygonClass();
                IGeometryCollection pGeoCol = ipg as IGeometryCollection;
                pGeoCol.AddGeometry(outerring);

                if (CpgShow.cedgeLkInnerComponents != null)
                {
                    foreach (var faceCEdgeInnerComponent in CpgShow.cedgeLkInnerComponents)
                    {
                        var innercptlt = CpgShow.GetInnerCptLt(faceCEdgeInnerComponent, false);
                        var innerring = CGeometricMethods.GenerateRingByCptlt(innercptlt, _dblFactorClipper);
                        pGeoCol.AddGeometry(innerring);

                        var furthercedgeLkInnerComponents = faceCEdgeInnerComponent.cedgeTwin.cpgIncidentFace.cedgeLkInnerComponents;
                        if (furthercedgeLkInnerComponents != null)
                        {
                            foreach (var furthercedgeInnerComponent in furthercedgeLkInnerComponents)
                            {
                                CpgShowStack.Push(furthercedgeInnerComponent.cedgeTwin.cpgIncidentFace);
                            }
                        }
                    }
                }
                yield return ipg;
            }

        }

        public IEnumerable<IEnumerable<CPoint>> GenerateCptEbEbByPaths(List<List<IntPoint>> Paths)
        {
            foreach (var path in Paths)
            {
                yield return GenerateCptEbByPath(path);
            }
        }

        public IEnumerable<CPoint> GenerateCptEbByPath(List<IntPoint> path)
        {
            for (int i = 0; i < path.Count; i++)
            {
                yield return new CPoint(i, path[i].X, (double)path[i].Y);
            }
        }

        public List<List<IntPoint>> GeneratePathsByCpgCol(ICollection<CPolygon> cpgCol)
        {
            var paths = new List<List<IntPoint>>(cpgCol.Count);
            foreach (var cpg in cpgCol)
            {
                //if (cpg.CptLtLt.Count > 1)
                //{
                //    throw new ArgumentOutOfRangeException("I didn't consider the case that there are more than 1 interior boundary!");
                //}

                //var path = new List<IntPoint>(cpg.CptLtLt[0].Count);
                foreach (var cptlt in cpg.CptLtLt)
                {
                    var path = new List<IntPoint>(cptlt.Count);
                    for (int i = 0; i < cptlt.Count-1; i++)
                    {
                        var cpt = cptlt[i];
                        path.Add(new IntPoint(cpt.X, cpt.Y));
                    }
                    paths.Add(path);
                }
            }
            return paths;
        }

        public IEnumerable<CPolygon> GenerateCpgEbByPaths(List<List<IntPoint>> Paths)
        {
            int intCount = 0;
            foreach (var cpteb in GenerateCptEbEbByPaths(Paths))
            {
                var cptlt = cpteb.ToList();
                cptlt.Add(new CPoint(cptlt.Count, cptlt[0].X, cptlt[0].Y));
                yield return new CPolygon(intCount++, cptlt);
            }
        }

        public IEnumerable<CPolyline> GenerateCplEbByPaths(List<List<IntPoint>> Paths)
        {
            int intCount = 0;
            foreach (var cpteb in GenerateCptEbEbByPaths(Paths))
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
