using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;
//using ESRI.ArcGIS.std
using ESRI.ArcGIS.esriSystem;

using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry;
using MorphingClass.CEntity;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CUtility
{
    public static class CDrawInActiveView
    {
        public static void AddPointElement(IGraphicsContainer pGraphicsContainer)
        {
            IPoint ipt = new PointClass();
            ipt.PutCoords(10, 10);


            IRgbColor rgbColor = new RgbColorClass();
            rgbColor.Red = 255;
            rgbColor.Green = 0;
            rgbColor.Blue = 0;

            

            ISimpleMarkerSymbol pSimpleSym = new SimpleMarkerSymbolClass();
            pSimpleSym.Style = esriSimpleMarkerStyle.esriSMSCross;
            pSimpleSym.Size = 10;
            pSimpleSym.Color = rgbColor as IColor;

            IMarkerElement pMarkerElem = null;
            IElement pElem;
            pElem = new MarkerElementClass();
            pElem.Geometry = ipt;
            pMarkerElem = pElem as IMarkerElement;
            pMarkerElem.Symbol = pSimpleSym;
            pGraphicsContainer.AddElement(pElem, 0);


            //pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }


        public static void DrawArrow(IActiveView pActiveView, CPoint frcpt, CPoint tocpt,
            double dblArrowLength = 6, double dblArrowWidth = 6,
            int intRed = 0, int intGreen = 0, int intBlue = 0)
        {
            var rgbColor = CHelpFunc.GenerateIRgbColor(intRed, intGreen, intBlue);

            //Define an arrow marker  
            IArrowMarkerSymbol arrowMarkerSymbol = new ArrowMarkerSymbolClass();
            arrowMarkerSymbol.Color = rgbColor;
            //arrowMarkerSymbol.Size = 6;  //it seems size has no effect
            arrowMarkerSymbol.Length = dblArrowLength;
            arrowMarkerSymbol.Width = dblArrowWidth;
            //Add an offset to make sure the square end of the line is hidden  
            //arrowMarkerSymbol.XOffset = 0.8;

            //Create cartographic line symbol  
            ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass();
            cartographicLineSymbol.Color = rgbColor;
            cartographicLineSymbol.Width = 1;

            //Define simple line decoration  
            ISimpleLineDecorationElement simpleLineDecorationElement = new SimpleLineDecorationElementClass();
            //Place the arrow at the end of the line (the "To" point in the geometry below)  
            simpleLineDecorationElement.AddPosition(1);
            simpleLineDecorationElement.MarkerSymbol = arrowMarkerSymbol;

            //Define line decoration  
            ILineDecoration lineDecoration = new LineDecorationClass();
            lineDecoration.AddElement(simpleLineDecorationElement);

            //Set line properties  
            ILineProperties lineProperties = (ILineProperties)cartographicLineSymbol;
            lineProperties.LineDecoration = lineDecoration;

            //Define line element  
            ILineElement lineElement = new LineElementClass();
            lineElement.Symbol = (ILineSymbol)cartographicLineSymbol;

            var cpl = new CPolyline(-1, frcpt, tocpt);

            //Cast to Element and set geometry  
            var element = (IElement)lineElement;
            element.Geometry = cpl.JudgeAndSetPolyline();

            //Set the name  
            //IElementProperties3 elementProperties3.Name = elementName;

            //Add element to graphics container  
            pActiveView.GraphicsContainer.AddElement(element, 0);


            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);



            #region a useful example https://community.esri.com/thread/69395
            ////Set the element name  
            //string elementName = "ArrowTest";

            ////Get the graphics container from the page layout (set elsewhere)  
            //IActiveView activeView = (IActiveView)pageLayout;
            //IGraphicsContainer graphicsContainer = (IGraphicsContainer)pageLayout;

            ////Find all existing elements with specified name  
            ////Build a list of elements and then loop over the list to delete them  
            //List<IElement> elementsToDelete = new List<IElement>();
            //graphicsContainer.Reset();
            //IElementProperties3 elementProperties3 = null;
            //IElement element = null;
            //while ((element = graphicsContainer.Next()) != null)
            //{
            //    elementProperties3 = (IElementProperties3)element;
            //    if (elementProperties3.Name == elementName)
            //    {
            //        elementsToDelete.Add(element);
            //    }
            //}

            //foreach (IElement elementToDelete in elementsToDelete)
            //{
            //    graphicsContainer.DeleteElement(elementToDelete);
            //}

            ////Define color  
            //IRgbColor rgbColor = new RgbColorClass();
            //rgbColor.RGB = Color.Black.ToArgb();

            ////Define an arrow marker  
            //IArrowMarkerSymbol arrowMarkerSymbol = new ArrowMarkerSymbolClass();
            //arrowMarkerSymbol.Color = rgbColor;
            //arrowMarkerSymbol.Size = 6;
            //arrowMarkerSymbol.Length = 8;
            //arrowMarkerSymbol.Width = 6;
            ////Add an offset to make sure the square end of the line is hidden  
            //arrowMarkerSymbol.XOffset = 0.8;

            ////Create cartographic line symbol  
            //ICartographicLineSymbol cartographicLineSymbol = new CartographicLineSymbolClass();
            //cartographicLineSymbol.Color = rgbColor;
            //cartographicLineSymbol.Width = 1;

            ////Define simple line decoration  
            //ISimpleLineDecorationElement simpleLineDecorationElement = new SimpleLineDecorationElementClass();
            ////Place the arrow at the end of the line (the "To" point in the geometry below)  
            //simpleLineDecorationElement.AddPosition(1);
            //simpleLineDecorationElement.MarkerSymbol = arrowMarkerSymbol;

            ////Define line decoration  
            //ILineDecoration lineDecoration = new LineDecorationClass();
            //lineDecoration.AddElement(simpleLineDecorationElement);

            ////Set line properties  
            //ILineProperties lineProperties = (ILineProperties)cartographicLineSymbol;
            //lineProperties.LineDecoration = lineDecoration;

            ////Define line element  
            //ILineElement lineElement = new LineElementClass();
            //lineElement.Symbol = (ILineSymbol)cartographicLineSymbol;

            ////Create the line geometry  
            //IPoint fromPoint = new PointClass();
            //fromPoint.X = 4.0;
            //fromPoint.Y = 0.8;

            //IPoint toPoint = new PointClass();
            //toPoint.X = 5.0;
            //toPoint.Y = 0.8;

            //IPolyline polyline = new PolylineClass();
            //polyline.FromPoint = fromPoint;
            //polyline.ToPoint = toPoint;

            ////Cast to Element and set geometry  
            //element = (IElement)lineElement;
            //element.Geometry = polyline;

            ////Set the name  
            //elementProperties3.Name = elementName;

            ////Add element to graphics container  
            //graphicsContainer.AddElement(element, 0);

            ////Clear the graphics selection (graphics are selected when added)  
            //IGraphicsContainerSelect graphicsContainerSelect = (IGraphicsContainerSelect)graphicsContainer;
            //graphicsContainerSelect.UnselectAllElements();

            //activeView.Refresh();
            #endregion
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pActiveView"></param>
        /// <param name="strText"></param>
        /// <param name="dblX">the center of the text</param>
        /// <param name="dblY">the bottom of characters like "5", "b", etc.; not the bottome of characters like "q"</param>
        /// <param name="dblSize">the size of the characters, not the size of the text box</param>
        /// <param name="intRed"></param>
        /// <param name="intGreen"></param>
        /// <param name="intBlue"></param>
        /// <remarks>
        /// https://resources.arcgis.com/en/help/arcobjects-net/componenthelp/index.html#//00480000021m000000
        /// </remarks>
        public static void DrawTextMarker(IActiveView pActiveView, string strText,
           double dblX, double dblY, double dblSize = 10, int intRed = 0, int intGreen = 0, int intBlue = 0)
        {
            IElement element;

            ITextElement textElement = new TextElementClass();
            element = textElement as IElement;

            ITextSymbol textSymbol = new TextSymbolClass();
            textSymbol.Color = CHelpFunc.GenerateIRgbColor(intRed, intGreen, intBlue);
            textSymbol.Size = dblSize;
            //textSymbol.Font = dblSize;
            //textSymbol.HorizontalAlignment = GetHorizontalAlignment();
            //textSymbol.VerticalAlignment = GetVerticalAlignment();           

            IPoint ipt = new PointClass();
            ipt.PutCoords(dblX, dblY);
            element.Geometry = ipt;

            textElement.Symbol = textSymbol;
            textElement.Text = strText;

            //textElement.
            pActiveView.GraphicsContainer.AddElement(element, 0);
            pActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }




            //红色线 
            public static void ViewPolyline(IMapControl4 pMapControl, CPolyline cpl)
        {
            //设置线段属性
            ILineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
            pSimpleLineSymbol.Width = 2;
            pSimpleLineSymbol.Color = CHelpFunc.GenerateIRgbColor(255, 0, 0) as IColor;

            //生成线段
            ILineElement pLineElement = new LineElementClass();
            pLineElement.Symbol = pSimpleLineSymbol;
            IElement pElement = pLineElement as IElement;
            pElement.Geometry = cpl.pPolyline;

            //显示线段
            IGraphicsContainer pGra = pMapControl.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            pGra.AddElement(pElement, 0);
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);  //此语句必不可少，否则添加的要素不会实时显示
        }

        //红色线 
        public static void ViewPolylines(IMapControl4 pMapControl, List<CPolyline> cpllt)
        {
            //设置线段属性
            ILineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
            pSimpleLineSymbol.Width = 1;
            pSimpleLineSymbol.Color = CHelpFunc.GenerateIRgbColor(255, 0, 0) as IColor;


            //生成线段
            IElementCollection pEleCol = new ElementCollectionClass();

            for (int i = 0; i < cpllt.Count; i++)
            {
                ILineElement pLineElement = new LineElementClass();
                pLineElement.Symbol = pSimpleLineSymbol;
                IElement pElement = pLineElement as IElement;
                pElement.Geometry = cpllt[i].pPolyline;
                pEleCol.Add(pElement, 0);
            }

            //显示线段
            IGraphicsContainer pGra = pMapControl.Map as IGraphicsContainer;
            IActiveView pAv = pGra as IActiveView;
            pGra.AddElements(pEleCol, 5);
            pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
        }


        /// <summary>
        /// 显示并返回单个插值线段
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>插值线段</returns>
        public static CPolyline DisplayInterpolation(CDataRecords pDataRecords, double dblProp)
        {
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return null;
            }
            List<CPoint> CResultPtLt = pDataRecords.ParameterResult.CResultPtLt;
            CPolyline cpl = CGeoFunc.GetTargetcpl(CResultPtLt, dblProp);
            cpl.SetPolyline();
            // 清除绘画痕迹
            IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            //m_mapControl.ActiveView.Refresh();   //由于在下一步“ViewPolyline”中有刷新的命令，此语句可省略
            ViewPolyline(m_mapControl, cpl);  //显示生成的线段
            return cpl;
        }

        /// <summary>
        /// 显示并返回多个插值线段
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>插值线段</returns>
        public static List<CPolyline> DisplayInterpolations(CDataRecords pDataRecords, double dblProp)
        {
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return null;
            }
            List<List<CPoint>> CResultPtLtLt = pDataRecords.ParameterResult.CResultPtLtLt;

            List<CPolyline> cpllt = new List<CPolyline>();
            for (int i = 0; i < CResultPtLtLt.Count; i++)
            {
                CPolyline cpl = CGeoFunc.GetTargetcpl(CResultPtLtLt[i], dblProp);
                cpllt.Add(cpl);
            }


            // 清除绘画痕迹
            IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            //m_mapControl.ActiveView.Refresh();   //由于在下一步“ViewPolyline”中有刷新的命令，此语句可省略
            ViewPolylines(m_mapControl, cpllt);  //显示生成的线段
            return cpllt;
        }
    }
}
