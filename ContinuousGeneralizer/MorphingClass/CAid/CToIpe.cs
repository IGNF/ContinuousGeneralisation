using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Display;
//using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;

using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;

namespace MorphingClass.CAid
{
    public class CToIpe
    {

        public static void SaveToIpe(List<IFeatureLayer> pFLayerLt, IEnvelope pFLayerEnv, CEnvelope pIpeEnv,
            bool blnGroup, string strBoundWidth, CParameterInitialize ParameterInitialize, bool IncreaseHeight = true)
        {
            //save path
            //CHelpFunc.SetSavePath(ParameterInitialize);

            //        double dblFactorIpeToLayer = pIpeEnv.Height / pFLayerEnv.Height;
            //        double dblLegend16 = 16 / dblFactorIpeToLayer;
            //        int intLegendInt = CMath.GetNumberTidy(dblLegend16);
            //        double dblLegentInt = intLegendInt * dblFactorIpeToLayer;


            //        //add legend (unit and a sample line), draw a line with length 32 in ipe
            //        string strData = CIpeDraw.writeIpeText(dblLegend16 + " " + ParameterInitialize.m_mapControl.MapUnits.ToString(), 320, 80) +
            //            CIpeDraw.drawIpeEdge(320, 64, 336, 64);

            //        strData += CIpeDraw.writeIpeText(intLegendInt + " " + ParameterInitialize.m_mapControl.MapUnits.ToString(), 320, 32) +
            //CIpeDraw.drawIpeEdge(320, 16, 320 + dblLegentInt, 16) +
            //CIpeDraw.drawIpeEdge(320, 16, 320, 20) + CIpeDraw.drawIpeEdge(320 + dblLegentInt, 16, 320 + dblLegentInt, 20);
            var strData = GetScaleLegend(pFLayerEnv, pIpeEnv, CHelpFunc.GetUnits(ParameterInitialize.m_mapControl.MapUnits));

            for (int i = 0; i < pFLayerLt.Count; i++)
            {
                var pFLayer = pFLayerLt[i] as IFeatureLayer;

                if (blnGroup == true)
                {
                    strData += "<group>\n";
                }

                strData += CToIpe.GetDataOfFeatureLayer(pFLayer, pFLayerEnv, pIpeEnv, strBoundWidth,true);

                if (blnGroup == true)
                {
                    strData += "</group>\n";
                }

                if (IncreaseHeight == true)
                {
                    pIpeEnv.YMin += pIpeEnv.Height;
                    pIpeEnv.YMax += pIpeEnv.Height;
                }
            }

            string strFullName = ParameterInitialize.strSavePath + "\\" + CHelpFunc.GetTimeStamp() + ".ipe";
            using (var writer = new System.IO.StreamWriter(strFullName, true))
            {
                writer.Write(CIpeDraw.GenerateIpeContentByData(strData));
            }

            System.Diagnostics.Process.Start(@strFullName);
        }

        public static string GetScaleLegend(IEnvelope pFLayerEnv, CEnvelope pIpeEnv, string strMapUnits)
        {
            double dblFactorIpeToLayer = pIpeEnv.Height / pFLayerEnv.Height;
            double dblLegend16 = 16 / dblFactorIpeToLayer;
            int intLegendInt = CMath.GetNumberTidy(dblLegend16);
            double dblLegentInt = intLegendInt * dblFactorIpeToLayer;


            ////add legend (unit and a sample line), draw a line with length 16 in ipe
            //string strData = CIpeDraw.writeIpeText(dblLegend16 + " " + ParameterInitialize.m_mapControl.MapUnits.ToString(), 320, 80) +
            //    CIpeDraw.drawIpeEdge(320, 64, 336, 64);

            //add legend (unit and a sample line), draw a line with length 32 in ipe so that we can easily compare shrinks with other figures
            return CIpeDraw.writeIpeText(intLegendInt + " " + strMapUnits, 320, 32) +
    CIpeDraw.drawIpeEdge(320, 16, 320 + dblLegentInt, 16) +
    CIpeDraw.drawIpeEdge(320, 16, 320, 20) + CIpeDraw.drawIpeEdge(320 + dblLegentInt, 16, 320 + dblLegentInt, 20);

        }

        public static string GetDataOfFeatureLayer(IFeatureLayer pFLayer, IEnvelope pFLayerEnv, 
            CEnvelope pIpeEnv, string strBoundWidth, bool blnDrawBound=false)
        {
            string str = "";
            if (blnDrawBound == true)
            {
                str += CIpeDraw.drawIpeEdge(pIpeEnv.XMin, pIpeEnv.YMin, pIpeEnv.XMin, pIpeEnv.YMax, "white");
            }

            IFeatureClass pFeatureClass = pFLayer.FeatureClass;
            int intFeatureCount = pFeatureClass.FeatureCount(null);
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //注意此处的参数(****,false)！！！            
            var pRenderer = (pFLayer as IGeoFeatureLayer).Renderer;
            for (int i = 0; i < intFeatureCount; i++)
            {
                //at the last round of this loop, pFeatureCursor.NextFeature() will return null
                IFeature pFeature = pFeatureCursor.NextFeature();
                
                switch (pFeatureClass.ShapeType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        str += TranIptToIpe(pFeature, pRenderer, pFLayerEnv, pIpeEnv, strBoundWidth);
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        str += TranIplToIpe(pFeature, pRenderer, pFLayerEnv, pIpeEnv, strBoundWidth);
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        str += TranIpgToIpe(pFeature, pRenderer, pFLayerEnv, pIpeEnv, strBoundWidth);
                        break;
                    default:
                        break;
                }
            }

            return str;
        }

        /// <summary>
        /// Save a feature layer of IPolyline to Ipe
        /// </summary>
        private static string TranIptToIpe(IFeature pFeature, IFeatureRenderer pRenderer, IEnvelope pFLayerEnv,
            CEnvelope pIpeEnv, string strBoundWidth)
        {
            var pMarkerSymbol = pRenderer.SymbolByFeature[pFeature] as IMarkerSymbol;
            var pMarkerSymbolRgbColor = pMarkerSymbol.Color as IRgbColor;

            if (strBoundWidth == "")
            {
                strBoundWidth = pMarkerSymbol.Size.ToString();
            }

            var ipt = pFeature.Shape as IPoint;
            return CIpeDraw.DrawIpt(ipt, pFLayerEnv, pIpeEnv, "disk", new CColor(pMarkerSymbolRgbColor), strBoundWidth);
        }



        /// <summary>
        /// Save a feature layer of IPolyline to Ipe
        /// </summary>
        private static string TranIplToIpe(IFeature pFeature, IFeatureRenderer pRenderer, IEnvelope pFLayerEnv,
            CEnvelope pIpeEnv, string strBoundWidth)
        {
            var pLineSymbol = pRenderer.SymbolByFeature[pFeature] as ILineSymbol;
            var pLineSymbolRgbColor = pLineSymbol.Color as IRgbColor;
            if (strBoundWidth == "")
            {
                strBoundWidth = pLineSymbol.Width.ToString();
            }

            //get the feature
            CPolyline cpl = new CPolyline(0, pFeature.Shape as IPolyline5);

            //append the string
            return CIpeDraw.DrawCpl(cpl, pFLayerEnv, pIpeEnv,
                new CColor(pLineSymbolRgbColor), strBoundWidth);
        }

        /// <summary>
        /// Save a feature layer of IPolygon to Ipe
        /// </summary>
        private static string TranIpgToIpe(IFeature pFeature, IFeatureRenderer pRenderer, 
            IEnvelope pFLayerEnv, CEnvelope pIpeEnv, string strBoundWidth)
        {
            var pFillSymbol = pRenderer.SymbolByFeature[pFeature] as IFillSymbol;
            return TranIpgToIpe((IPolygon4)pFeature.Shape, pFillSymbol, pFLayerEnv, pIpeEnv, strBoundWidth);
        }

        /// <summary>
        /// Save a feature layer of IPolygon to Ipe
        /// </summary>
        public static string TranIpgToIpe(IPolygon4 ipg, IFillSymbol pFillSymbol, 
            IEnvelope pFLayerEnv, CEnvelope pIpeEnv, string strBoundWidth)
        {
            //get the color of the filled part
            //we are not allowed to directly use "var pFillRgbColor = pFillSymbol.Color as IRgbColor;"
            //Nor can we use "var pFillRgbColor = pFillSymbol.Color.RGB as IRgbColor;"
            //pFillSymbol.Color.RGB has type 'int'
            IColor pFillSymbolColor = new RgbColorClass();
            pFillSymbolColor.RGB = pFillSymbol.Color.RGB;
            var pFillSymbolRgbColor = pFillSymbolColor as IRgbColor;

            //get the color of the out line                
            var pOutlineRgbColor = pFillSymbol.Outline.Color as IRgbColor;
            if (strBoundWidth == "")
            {
                strBoundWidth = pFillSymbol.Outline.Width.ToString();
            }

            //get the feature
            CPolygon cpg = new CPolygon(0, ipg);

            //append the string
            return CIpeDraw.DrawCpg(cpg, pFLayerEnv, pIpeEnv, new CColor(pOutlineRgbColor),
new CColor(pFillSymbolRgbColor), strBoundWidth);
        }

        /// <summary>
        /// Save a feature layer of IPolygon to Ipe
        /// </summary>
        public static string TranIpgBoundToIpe(IPolygon ipg,
            IEnvelope pFLayerEnv, CEnvelope pIpeEnv, CColor StrokeColor, string strBoundWidth, string strDash)
        {
            ////get the color of the filled part
            ////we are not allowed to directly use "var pFillRgbColor = pFillSymbol.Color as IRgbColor;"
            ////Nor can we use "var pFillRgbColor = pFillSymbol.Color.RGB as IRgbColor;"
            ////pFillSymbol.Color.RGB has type 'int'
            //IColor pFillSymbolColor = new RgbColorClass();
            //pFillSymbolColor.RGB = pFillSymbol.Color.RGB;
            //var pFillSymbolRgbColor = pFillSymbolColor as IRgbColor;

            ////get the color of the out line                
            //var pOutlineRgbColor = pFillSymbol.Outline.Color as IRgbColor;
            //if (strBoundWidth == "")
            //{
            //    strBoundWidth = pFillSymbol.Outline.Width.ToString();
            //}

            //get the feature
            CPolygon cpg = new CPolygon(0, ipg as IPolygon4);

            //append the string
            return CIpeDraw.DrawCpgBound(cpg, pFLayerEnv, pIpeEnv, StrokeColor, strBoundWidth, strDash);
        }


        //public static SortedDictionary<T, ISymbol> GetKeySymbolSD<T>(IFeatureLayer pFLayer, List<List<object>> pObjValueLtLt, int intKeyIndex)
        //    where T : IComparable<T>
        //{
        //    IFeatureClass pFeatureClass = pFLayer.FeatureClass;
        //    int intFeatureCount = pFeatureClass.FeatureCount(null);
        //    IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //注意此处的参数(****,false)！！！            
        //    var pRenderer = (pFLayer as IGeoFeatureLayer).Renderer;
        //    SortedDictionary<T, ISymbol> KeySymbolSD = new SortedDictionary<T, ISymbol>();
        //    for (int i = 0; i < intFeatureCount; i++)
        //    {
        //        //at the last round of this loop, pFeatureCursor.NextFeature() will return null
        //        IFeature pFeature = pFeatureCursor.NextFeature();
        //        var TKey = (T)pObjValueLtLt[i][intKeyIndex];
        //        if (KeySymbolSD.ContainsKey(TKey) == false)
        //        {
        //            KeySymbolSD.Add(TKey, pRenderer.SymbolByFeature[pFeature]);
        //        }
        //    }
        //    return KeySymbolSD;
        //}

        public static SortedDictionary<int, ISymbol> GetKeySymbolSD(IFeatureLayer pFLayer, List<List<object>> pObjValueLtLt, int intKeyIndex)
        {
            IFeatureClass pFeatureClass = pFLayer.FeatureClass;
            int intFeatureCount = pFeatureClass.FeatureCount(null);
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //注意此处的参数(****,false)！！！            
            var pRenderer = (pFLayer as IGeoFeatureLayer).Renderer;
            var KeySymbolSD = new SortedDictionary<int, ISymbol>();
            for (int i = 0; i < intFeatureCount; i++)
            {
                //at the last round of this loop, pFeatureCursor.NextFeature() will return null
                IFeature pFeature = pFeatureCursor.NextFeature();
                var TKey = Convert.ToInt32(pObjValueLtLt[i][intKeyIndex]);
                if (KeySymbolSD.ContainsKey(TKey) == false)
                {
                    KeySymbolSD.Add(TKey, pRenderer.SymbolByFeature[pFeature]);
                }
            }
            return KeySymbolSD;
        }


        #region Obsolete: A case using IClassBreaksRenderer and ILookupSymbol
        ///// <summary>
        ///// SaveToIpe
        ///// </summary>
        ///// <param name="CPlLt"></param>
        ///// <param name="strWidth">normal, heavier, fat, ultrafat</param>
        //public static string SaveToIpeIpg(IFeatureLayer pFeatureLayer, string strFileName, IEnvelope pFLayerEnv, CEnvelope pIpeEnv, CParameterInitialize pParameterInitialize)
        //{
        //    string str = "";
        //    IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
        //    int intFeatureCount = pFeatureClass.FeatureCount(null);
        //    IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //注意此处的参数(****,false)！！！            

        //    IGeoFeatureLayer pGeoFeaturelayer = pFeatureLayer as IGeoFeatureLayer;
        //    IClassBreaksRenderer pClassBreaksRenderer = pGeoFeaturelayer.Renderer as IClassBreaksRenderer;

        //    IFeature pFeature = pFeatureCursor.NextFeature();
        //    ILookupSymbol pLookupSymbol = pClassBreaksRenderer as ILookupSymbol;
        //    pLookupSymbol.LookupSymbol(true, pFeature);

        //    for (int i = 0; i < intFeatureCount; i++)
        //    {
        //        //get the colors
        //        ISimpleFillSymbol pSimpleFillSymbol = pLookupSymbol.LookupSymbol(true, pFeature) as ISimpleFillSymbol;
        //        IRgbColor pOutlineRgbColor = pSimpleFillSymbol.Outline.Color as IRgbColor;
        //        IColor pFillSymbolColor = new RgbColorClass();
        //        pFillSymbolColor.RGB = pSimpleFillSymbol.Color.RGB;
        //        IRgbColor pFillSymbolRgbColor = pFillSymbolColor as IRgbColor;

        //        //get the feature
        //        IPolygon4 ipg = pFeature.Shape as IPolygon4;
        //        //SetZCoordinates(ipg as IPointCollection4);  //set the z coordinates, it may be used in Constructing TIN
        //        CPolygon cpg = new CPolygon(i, ipg as IPolygon4);

        //        //append the string
        //        str += CIpeDraw.DrawCpg(cpg, pFLayerEnv, pIpeEnv, new CColor(pOutlineRgbColor), new CColor(pFillSymbolRgbColor), pSimpleFillSymbol.Outline.Width.ToString());

        //        pFeature = pFeatureCursor.NextFeature();  //at the last round of this loop, pFeatureCursor.NextFeature() will return null
        //    }

        //    return str;
        //}
        #endregion


    }
}
