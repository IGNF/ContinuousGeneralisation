using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using MorphingClass.CEntity;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CAid
{
    public class CTopologyChecker : CMorphingBaseCpl
    {
        public CTopologyChecker()
        {

        }


        public CTopologyChecker(CParameterInitialize ParameterInitialize, int intLayerCount = 2)
        {
            Construct<CPolyline>(ParameterInitialize, 0, intLayerCount);

            GetAllReadCEdgeLt<CPolyline>();

            this.ObjCGeoLtLt[0] = null;
            if (intLayerCount==2)
            {
                this.ObjCGeoLtLt[1] = null;
            }            
        }


        public void TopologyCheck()
        {
            var ParameterInitialize = _ParameterInitialize;
            List<CEdge> pAllReadCEdgeLt = _AllReadCEdgeLt;

            long lngStartTime = System.Environment.TickCount;

            double dblVerySmall = CConstants.dblVerySmallCoord;

            throw new ArgumentException("consider setting of dblverysmall!");
            for (int i = 0; i < 10; i++)
            {
                CConstants.dblVerySmallCoord = dblVerySmall / Math.Pow(10, i - 5);
                var fEdgeGrid = new CEdgeGrid(pAllReadCEdgeLt);
                var IntersectionLt = fEdgeGrid.DetectIntersectionsOfExistingEdges(true, true, true);

                foreach (var cedge in pAllReadCEdgeLt)
                {
                    cedge.FrCpt.IntersectionLt = new List<CIntersection>();
                    cedge.ToCpt.IntersectionLt = new List<CIntersection>();

                    cedge.FrCpt.isTraversed = false;
                    cedge.ToCpt.isTraversed = false;
                }

                List<CPoint> CrossCptLt = new List<CPoint>();
                //List<CEdge> OverlapEdgeLt = new List<CEdge>();  //to save memory, we don't record overlap edges anymore, instead, we only record an end of the overlap edge
                List<CPoint> OverlapCptLt = new List<CPoint>();
                foreach (CIntersection pIntersection in IntersectionLt)
                {
                    switch (pIntersection.enumIntersectionType)
                    {
                        case CEnumIntersectionType.NoNo:
                            break;
                        case CEnumIntersectionType.FrFr:
                            pIntersection.CEdge1.FrCpt.IntersectionLt.Add(pIntersection);   //End Intersection
                            pIntersection.CEdge2.FrCpt.IntersectionLt.Add(pIntersection);   //End Intersection
                            break;
                        case CEnumIntersectionType.FrIn:
                            //CrossCptLt.AddLast(pIntersection.IntersectCpt);     //Cross
                            //pIntersection.CEdge1.FrCpt.IntersectionLt.Add(pIntersection);   //we don't want to notice this intersection twice, so we add this intersection to the vertice
                            break;
                        case CEnumIntersectionType.FrTo:
                            pIntersection.CEdge1.FrCpt.IntersectionLt.Add(pIntersection);   //End Intersection
                            pIntersection.CEdge2.ToCpt.IntersectionLt.Add(pIntersection);   //End Intersection
                            break;
                        case CEnumIntersectionType.InFr:
                            //CrossCptLt.AddLast(pIntersection.IntersectCpt);     //Cross
                            //pIntersection.CEdge2.FrCpt.IntersectionLt.Add(pIntersection);   //we don't want to notice this intersection twice, so we add this intersection to the vertice
                            break;
                        case CEnumIntersectionType.InIn:
                            CrossCptLt.Add(pIntersection.IntersectCpt);     //Cross
                            break;
                        case CEnumIntersectionType.InTo:
                            //CrossCptLt.AddLast(pIntersection.IntersectCpt);     //Cross
                            //pIntersection.CEdge2.ToCpt.IntersectionLt.Add(pIntersection);   //we don't want to notice this intersection twice, so we add this intersection to the vertice
                            break;
                        case CEnumIntersectionType.ToFr:
                            pIntersection.CEdge1.ToCpt.IntersectionLt.Add(pIntersection);   //End Intersection
                            pIntersection.CEdge2.FrCpt.IntersectionLt.Add(pIntersection);   //End Intersection
                            break;
                        case CEnumIntersectionType.ToIn:
                            //CrossCptLt.AddLast(pIntersection.IntersectCpt);     //Cross
                            //pIntersection.CEdge1.ToCpt.IntersectionLt.Add(pIntersection);   //we don't want to notice this intersection twice, so we add this intersection to the vertice
                            break;
                        case CEnumIntersectionType.ToTo:
                            pIntersection.CEdge1.ToCpt.IntersectionLt.Add(pIntersection);   //End Intersection
                            pIntersection.CEdge2.ToCpt.IntersectionLt.Add(pIntersection);   //End Intersection
                            break;
                        case CEnumIntersectionType.Overlap:
                            //OverlapEdgeLt.Add(pIntersection.OverlapCEdge);
                            OverlapCptLt.Add(pIntersection.IntersectCpt);
                            //pIntersection.OverlapCEdge.FrCpt.IntersectionLt.Add(pIntersection);   //we don't want to notice this intersection twice, so we add this intersection to the vertice
                            //pIntersection.OverlapCEdge.ToCpt.IntersectionLt.Add(pIntersection);   //we don't want to notice this intersection twice, so we add this intersection to the vertice
                            break;
                        default:
                            break;
                    }
                }
                CConstants.dblVerySmallCoord = dblVerySmall;

                List<CPoint> UnLinkedCptLt = new List<CPoint>();
                foreach (var cedge in pAllReadCEdgeLt)
                {
                    if (cedge.FrCpt.isTraversed == false && cedge.FrCpt.IntersectionLt.Count == 0)
                    {
                        UnLinkedCptLt.Add(cedge.FrCpt);
                    }

                    if (cedge.ToCpt.isTraversed == false && cedge.ToCpt.IntersectionLt.Count == 0)
                    {
                        UnLinkedCptLt.Add(cedge.ToCpt);
                    }

                    cedge.FrCpt.IntersectionLt = null;
                    cedge.ToCpt.IntersectionLt = null;

                    cedge.FrCpt.isTraversed = true;
                    cedge.ToCpt.isTraversed = true;
                }

                IntersectionLt = null;

                CSaveFeature.SaveCGeoEb(UnLinkedCptLt, esriGeometryType.esriGeometryPoint, 
                    "UnLinkedCpt_" + i.ToString() + "__" + UnLinkedCptLt.Count,  blnVisible :false  );
                CHelpFunc.SetAEGeometryNull(UnLinkedCptLt);
                
                CSaveFeature.SaveCGeoEb(CrossCptLt, esriGeometryType.esriGeometryPoint, 
                    "Crosses_" + i.ToString() + "__" + CrossCptLt.Count, blnVisible: false);
                CHelpFunc.SetAEGeometryNull(CrossCptLt);
                
                CSaveFeature.SaveCGeoEb(OverlapCptLt, esriGeometryType.esriGeometryPoint, 
                    "OverlapCpt_" + i.ToString() + "__" + OverlapCptLt.Count,  blnVisible: false);
                CHelpFunc.SetAEGeometryNull(OverlapCptLt);
                
            }

            long lngEndTime = System.Environment.TickCount;//记录结束时间
            ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时
        }
    }
}
