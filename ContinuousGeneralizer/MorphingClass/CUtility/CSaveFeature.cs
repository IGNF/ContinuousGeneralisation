using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Forms;

using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry ;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;

using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CUtility
{
    public class CSaveFeature
    {
        public IFeatureLayer pFeatureLayer { get; set; }
        //private int _intKnown = 2;   //we don't need the first three FieldNames, i.e., "FID", "Shape"

        public CSaveFeature()
        {

        }

        /// <summary>
        /// 初始化函数
        /// </summary>
        /// <param name="pstrLayerName">图层名称</param> 
        /// <param name="pstrPath">保存路径</param>
        /// <param name="pesriGeometryType">图层类型</param> 
        /// <remarks>If I have time, I will put the parameters of colors and width in one parameter!</remarks>
        public CSaveFeature(esriGeometryType pesriGeometryType, string pstrLayerName, IWorkspace pWorkspace, IMapControl4 pm_mapControl,
            List<string> pstrFieldNameLt = null, List<esriFieldType> pesriFieldTypeLt = null,
            int fintRed = 0, int fintGreen = 0, int fintBlue = 0, double dblWidth = 1, string strSymbolLayerPath = null, bool blnVisible = true)
        {
            this.pFeatureLayer = CreateFeatureLayer(pesriGeometryType, pstrLayerName, pWorkspace, pm_mapControl, pstrFieldNameLt, pesriFieldTypeLt, fintRed, fintGreen, fintBlue, dblWidth,strSymbolLayerPath, blnVisible);
        }

        private IFeatureLayer CreateFeatureLayer(esriGeometryType pesriGeometryType, string pstrLayerName, IWorkspace pWorkspace, IMapControl4 pm_mapControl,
            List<string> pstrFieldNameLt = null, List<esriFieldType> pesriFieldTypeLt = null,
            int fintRed = 0, int fintGreen = 0, int fintBlue = 0, double dblWidth = 1, string strSymbolLayerPath=null, bool blnVisible=true)
        {

            IFeatureClass pFeatureClass = CreateFeatureClass(pesriGeometryType, pstrLayerName, pWorkspace, pm_mapControl, pstrFieldNameLt, pesriFieldTypeLt);
            IFeatureLayer pFLayer = new FeatureLayerClass();
            pFLayer.FeatureClass = pFeatureClass;
            pFLayer.Name = pFeatureClass.AliasName;
            pFLayer.SpatialReference = pm_mapControl.SpatialReference;

            RenderLayer(ref pFLayer, pesriGeometryType, fintRed, fintGreen, fintBlue, dblWidth, strSymbolLayerPath);

            //IDataLayer2 pDataLayer2 = pFLayer as IDataLayer2;
            //pDataLayer2 .RelativeBase.

            //save Layer as layer file ".lyr"
            //create a new LayerFile instance
            ILayerFile layerFile = new LayerFileClass();
            //create a new layer file
            layerFile.New(pWorkspace.PathName + "\\" + pstrLayerName + ".lyr");
            //attach the layer file with the actual layer
            layerFile.ReplaceContents((ILayer)pFLayer);
            //save the layer file
            layerFile.Save();


            //***********************************************是否添加到当前文档中来***********************************************//
            //m_mapControl.AddLayer(pFLayer,m_mapControl .LayerCount);
            pm_mapControl.AddLayer(pFLayer);
            pFLayer.Visible = blnVisible;
            pm_mapControl.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGeography, null, null);
            return pFLayer;
        }

        public IFeatureLayer SaveCGeosToLayer<T>(IEnumerable<T> pCGeoEb, List<string> pstrFieldNameLt = null, List<List<object>> pobjectValueLtLt = null)
            where T : CGeometricBase<T>
        {
            if (pCGeoEb != null)
            {
                CommitFeatures(CHelpFunc.JudgeAndSetAEGeometry(pCGeoEb), pstrFieldNameLt, pobjectValueLtLt);
            }

            return this.pFeatureLayer;

        }

        //public IEnumerable<object> 

        public IFeatureLayer SaveIGeosToLayer<T>(IEnumerable<T> pIGeoEb, List<string> pstrFieldNameLt = null, List<List<object>> pobjectValueLtLt = null)
            where T : IGeometry
        {
            if (pIGeoEb != null)
            {
                var objshapeEb = pIGeoEb.AsExpectedClass<object, T>();
                CommitFeatures(objshapeEb, pstrFieldNameLt, pobjectValueLtLt);
            }

            return this.pFeatureLayer;
        }

        public IFeatureLayer SaveIGeosToLayer(IGeometryCollection pGeoCol, List<string> pstrFieldNameLt = null, List<List<object>> pobjectValueLtLt = null)
        {
            var objshapeEb = CHelpFunc.GetTEbFromIGeoCol<object>(pGeoCol);
            CommitFeatures(objshapeEb, pstrFieldNameLt, pobjectValueLtLt);
            return this.pFeatureLayer;
        }



        //public IFeatureLayer SaveFeaturesToLayer(IEnumerable<object> pobjectShapeEb, List<string> pstrFieldNameLt = null, List<List<object>> pobjectValueLtLt = null)
        //{
        //    IFeatureClass pFeatureClass = this.pFeatureLayer.FeatureClass;
        //    CommitFeatures(ref pFeatureClass, pobjectShapeEb, pstrFieldNameLt, pobjectValueLtLt);
        //    return this.pFeatureLayer;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pobjectShapeEb"></param>
        /// <param name="pstrFieldNameLt"></param>
        /// <param name="pobjectValueLtLt"></param>
        /// <remarks>it should be ok if pobjectShapeEb is null or empty</remarks>
        private void CommitFeatures(IEnumerable<object> pobjectShapeEb, List<string> pstrFieldNameLt = null, List<List<object>> pobjectValueLtLt = null)
        {
            var pFeatureClass = this.pFeatureLayer.FeatureClass;
            if (pobjectShapeEb != null)
            {
                var intIndexLt = GenerateIndexLt(pstrFieldNameLt, pFeatureClass);
                IFeatureBuffer pFeatureBuffer = pFeatureClass.CreateFeatureBuffer();
                IFeature pFeature = pFeatureBuffer as IFeature;
                IFeatureCursor pFeatureCursor = pFeatureClass.Insert(true);

                int intCount = 0;
                foreach (var pobjectShape in pobjectShapeEb)
                {
                    List<object> pobjectValueLt = null;
                    if (pobjectValueLtLt != null)
                    {
                        pobjectValueLt = pobjectValueLtLt[intCount];
                    }

                    InsertOneFeature(pFeatureCursor, pFeatureBuffer, pFeature, pobjectShape, intIndexLt, pstrFieldNameLt, pobjectValueLt);
                    intCount++;
                }
                pFeatureCursor.Flush();
            }
        }


        //public void CommitFeature(object pobj, List<string> pstrFieldNameLt = null, List<object> pobjectValueLt = null)
        //{
        //    var pFeatureClass = this.pFeatureLayer.FeatureClass;
        //    if (pobj != null)
        //    {
        //        var intIndexLt = GenerateIndexLt(pstrFieldNameLt, pFeatureClass);

        //        IFeatureBuffer pFeatureBuffer = pFeatureClass.CreateFeatureBuffer();
        //        IFeature pFeature = pFeatureBuffer as IFeature;
        //        IFeatureCursor pFeatureCursor = pFeatureClass.Insert(true);

        //        InsertOneFeature(pFeatureCursor, pFeatureBuffer,pFeature, pobj, intIndexLt, pstrFieldNameLt, pobjectValueLt);
        //        pFeatureCursor.Flush();
        //    }
        //}


        private List<int> GenerateIndexLt(List<string> pstrFieldNameLt, IFeatureClass pFeatureClass)
        {
            var intIndexLt = new List<int>();
            if (pstrFieldNameLt != null)
            {
                intIndexLt = new List<int>(pstrFieldNameLt.Count);
                for (int i =0; i < pstrFieldNameLt.Count; i++)     //we don't need the first three FieldNames, i.e., "FID", "Shape", and "Id"
                {
                    int intIndex = pFeatureClass.FindField(pstrFieldNameLt[i]);
                    intIndexLt.Add(intIndex);
                }
            }
            return intIndexLt;
        }

        private void InsertOneFeature(IFeatureCursor pFeatureCursor ,IFeatureBuffer pFeatureBuffer, IFeature pFeature,  object pobj,List<int>intIndexLt, List<string> pstrFieldNameLt, List<object> pobjectValueLt)
        {
            pFeature.Shape = pobj as IGeometry;
            if (pstrFieldNameLt != null)
            {
                for (int j = 0; j < pobjectValueLt.Count; j++)
                {
                    pFeature.set_Value(intIndexLt[j], pobjectValueLt[j]);
                }
            }
            pFeatureCursor.InsertFeature(pFeatureBuffer);
        }

        
        /// <summary>
        /// Coder:  梁  爽
        /// Date:   2008-10-16
        /// Content:该函数实现要素类的创建，首先用接口IGeometryDefEdit设置要素类的类型，即是Point,Polylin还是Polygon
        ///         然后用接口IFieldEdit设置要素类的字段，这里设置了三个字段，分别是ID,OID和Shape。最后通过接口IFeatureWorkspace
        ///         的CreateFeatureClass方法创建要素类，函数返回IFeatureClass接口类型
        /// </summary>
        /// <param name="pWorkspace">设置工作空间</param>
        /// <param name="pGeometryType">设置要素的类型，即是点，线，还是面</param>
        /// <param name="strFeatName">设置要素类的名称</param>
        /// <param name="ValCoordinatType">传递一个数值型参数，该参数为1-UnkownCoordinateSystem，2-GeographicCoordinateSystem
        ///            或3ProjectedCoordinateSystem</param>
        /// <returns></returns>
        private IFeatureClass CreateFeatureClass(esriGeometryType pesriGeometryType, string pstrLayerName, IWorkspace pWorkspace, IMapControl4 pm_mapControl,
            List<string> pstrFieldNameLt = null, List<esriFieldType> pesriFieldTypeLt = null)
        {
            //创建要素空间
            IFeatureWorkspace pFeatureWorkspace = (IFeatureWorkspace)pWorkspace;
            IGeometryDefEdit pGeometryDefEdit = new GeometryDefClass();
            pGeometryDefEdit.GeometryType_2 = pesriGeometryType;   //_esriGeometryType could be: esriGeometryPoint, esriGeometryMultipoint, esriGeometryPolyline, esriGeometryPolygon, and esriGeometryMultiPatch.
            pGeometryDefEdit.SpatialReference_2 = pm_mapControl.SpatialReference;

            //新建字段
            IFieldsEdit pFieldsEdit = new FieldsClass();
            //添加“Shape”字段
            IFieldEdit pField;
            pField = new FieldClass();
            pField.Type_2 = esriFieldType.esriFieldTypeGeometry;
            pField.GeometryDef_2 = pGeometryDefEdit;
            pField.Name_2 = "Shape";
            pFieldsEdit.AddField((IField)pField);
            //添加其它字段
            if (pstrFieldNameLt != null)
            {
                for (int i = 0; i < pstrFieldNameLt.Count; i++)  //we don't need the first three FieldNames, i.e., "FID", "Shape", and "Id"
                {
                    pFieldsEdit.AddField((IField)GenerateFieldEdit(pesriFieldTypeLt[i], pstrFieldNameLt[i]));
                }
            }


            string strFullName = pWorkspace.PathName + "\\" + pstrLayerName;
            File.Delete(strFullName + ".dbf");
            File.Delete(strFullName + ".lyr");
            File.Delete(strFullName + ".prj");
            File.Delete(strFullName + ".shp");
            //File.Delete(strFullName + ".shp.PALANQUE.2296.5388.sr.lock");
            File.Delete(strFullName + ".shx");

            try
            {
                return pFeatureWorkspace.CreateFeatureClass(pstrLayerName, (IFields)pFieldsEdit, null, null, esriFeatureType.esriFTSimple, "Shape&quot;", "");
            }
            catch
            {
                return pFeatureWorkspace.CreateFeatureClass(pstrLayerName, (IFields)pFieldsEdit, null, null, esriFeatureType.esriFTSimple, "Shape", "");
            }
        }

        public void RenderLayer(ref IFeatureLayer fLayer, esriGeometryType fesriGeometryType, int fintRed, int fintGreen, int fintBlue, double fdblWidth, string strSymbolLayerPath)
        {
            if (strSymbolLayerPath == null)
            {
                IRgbColor pRgbColor = new RgbColorClass();
                pRgbColor.Red = fintRed;
                pRgbColor.Green = fintGreen;
                pRgbColor.Blue = fintBlue;

                ISimpleRenderer pSimpleRenderer = new SimpleRendererClass();

                switch (fesriGeometryType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        ISimpleMarkerSymbol pSimpleMarkerSymbol = new SimpleMarkerSymbolClass();
                        pSimpleMarkerSymbol.Color = pRgbColor as IColor;
                        pSimpleMarkerSymbol.Size = fdblWidth;
                        pSimpleRenderer.Symbol = pSimpleMarkerSymbol as ISymbol;
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        ISimpleLineSymbol pSimpleLineSymbol = new SimpleLineSymbolClass();
                        pSimpleLineSymbol.Color = pRgbColor as IColor;
                        pSimpleLineSymbol.Width = fdblWidth;
                        pSimpleRenderer.Symbol = pSimpleLineSymbol as ISymbol;
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        ISimpleFillSymbol pSimpleFillSymbol = new SimpleFillSymbolClass();
                        pSimpleFillSymbol.Color = pRgbColor as IColor;
                        pSimpleFillSymbol.Color = pRgbColor as IColor;
                        pSimpleFillSymbol.Outline.Width = fdblWidth;
                        pSimpleRenderer.Symbol = pSimpleFillSymbol as ISymbol;
                        break;
                    default:
                        return;
                }
                //fLayer.
                IGeoFeatureLayer pGeoFeaturelayer = fLayer as IGeoFeatureLayer;
                pGeoFeaturelayer.Renderer = pSimpleRenderer as IFeatureRenderer;
            }
            else
            {
                //In the shapefile properties, under the Symbology tab, you have the Import button which allow you to import the symbology definition from another shapefile or layer file. The code below is doing exactly the same. 
                //You need to create a layer file with the desired symbology definition and then call this sub. 

                IGxLayer pGxLayer;
                IGxFile pGxFile;
                ILayer pSymLayer;
                IGeoFeatureLayer pLyr;
                IGeoFeatureLayer pGeoSymLyr;

                pGxLayer = new GxLayer();
                pGxFile = pGxLayer as IGxFile;

                pGxFile.Path = strSymbolLayerPath;

                pSymLayer = pGxLayer.Layer;
                pLyr = fLayer as IGeoFeatureLayer ;
                pGeoSymLyr = pSymLayer as IGeoFeatureLayer ;
                pLyr.Renderer = pGeoSymLyr.Renderer;
                
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="pesriFieldType"></param>
        /// <param name="strFieldName"></param>
        /// <returns></returns>
        /// <remarks>usually, when FieldIndex == 0, strFieldName is FID </remarks>
        public static int TryAddField(IFeatureClass pFeatureClass, esriFieldType pesriFieldType, string pstrFieldName)
        {
            var intIndex = pFeatureClass.FindField(pstrFieldName);   //If FindField returns -1, the Field could not be found in the Fields collection. 
            if (intIndex == -1)  //strFieldName doesn't exist
            {
                return AddField(pFeatureClass, pesriFieldType,pstrFieldName);
            }
            else
            {
                return intIndex;
            }
        }

        public static int AddField(IFeatureClass pFeatureClass, esriFieldType pesriFieldType, string pstrFieldName)
        {
            IFieldEdit pFieldEdit = GenerateFieldEdit(pesriFieldType, pstrFieldName);
            pFeatureClass.AddField((IField)pFieldEdit);
            return pFeatureClass.Fields.FieldCount - 1;
        }


        private static IFieldEdit GenerateFieldEdit(esriFieldType pesriFieldType, string strFieldName)
        {
            IFieldEdit pFieldEdit;
            pFieldEdit = new FieldClass();
            pFieldEdit.Type_2 = pesriFieldType;
            pFieldEdit.Name_2 = strFieldName;

            return pFieldEdit;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="pFeatureClass"></param>
        /// <param name="pesriFieldType"></param>
        /// <param name="pstrFieldName"></param>
        /// <param name="pobjEb"></param>
        /// <remarks>the number of features in pFeatureClass and the count of pobjEb should be the same</remarks>
        public static void AddFieldandAttribute(IFeatureClass pFeatureClass, esriFieldType pesriFieldType, string pstrFieldName, IEnumerable<object> pobjEb)
        {
            var intIndex = TryAddField(pFeatureClass, pesriFieldType, pstrFieldName);

            SetFieldValue(pFeatureClass, intIndex, pobjEb);
        }

        public static void SetFieldValue(IFeatureClass pFeatureClass, int intIndex, IEnumerable<object> pobjEb)
        {
            int intFeatureCount = pFeatureClass.FeatureCount(null);
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //Note the parameter(****,false)！！！
            var pobjEt = pobjEb.GetEnumerator();

            for (int i = 0; i < intFeatureCount; i++)
            {
                pobjEt.MoveNext();
                IFeature pFeature = pFeatureCursor.NextFeature();
                pFeature.set_Value(intIndex, pobjEt.Current);
                pFeature.Store();
            }
        }

        public static void SetFieldValue(IFeatureClass pFeatureClass, int intIndex, object pobj)
        {
            int intFeatureCount = pFeatureClass.FeatureCount(null);
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //Note the parameter(****,false)！！！
            for (int i = 0; i < intFeatureCount; i++)
            {
                IFeature pFeature = pFeatureCursor.NextFeature();
                pFeature.set_Value(intIndex, pobj);
                pFeature.Store();
            }
        }

        


        public static int FindFieldNameIndex(List<string> strFieldNameLt, string strFieldName)
        {
            for (int i = 0; i < strFieldNameLt.Count; i++)
            {
                if (strFieldNameLt[i] == strFieldName)
                {
                    return i;
                }
            }

            throw new ArgumentNullException("There is no matched field name!");
        }


        public static IFeatureLayer SaveCpgEb(IEnumerable<CPolygon> CpgEb, string strFileName,
            List<string> pstrFieldNameLt = null, List<esriFieldType> pesriFieldTypeLt = null,
            List<List<object>> pobjectValueLtLt = null,
            int intRed = 0, int intGreen = 0, int intBlue = 0, double dblWidth = 1,
            string strSymbolLayerPath = null, bool blnVisible = true)
        {
            return SaveCGeoEb(CpgEb, esriGeometryType.esriGeometryPolygon, strFileName,
                CConstants.ParameterInitialize.pWorkspace, CConstants.ParameterInitialize.m_mapControl,
                pstrFieldNameLt, pesriFieldTypeLt, pobjectValueLtLt, intRed, intGreen, intBlue, dblWidth, strSymbolLayerPath, blnVisible);
        }


        public static IFeatureLayer SaveCGeoEb<T>(IEnumerable<T> CGeoEb, esriGeometryType pesriGeometryType, string strFileName, 
            List<string> pstrFieldNameLt = null, List<esriFieldType> pesriFieldTypeLt = null,
            List<List<object>> pobjectValueLtLt = null, 
            int intRed = 0, int intGreen = 0, int intBlue = 0, double dblWidth = 1,
            string strSymbolLayerPath = null, bool blnVisible = true)
             where T : CGeometricBase<T>
        {
            return SaveCGeoEb(CGeoEb, pesriGeometryType, strFileName, 
                CConstants.ParameterInitialize.pWorkspace, CConstants.ParameterInitialize.m_mapControl,
                pstrFieldNameLt, pesriFieldTypeLt, pobjectValueLtLt, intRed, intGreen, intBlue, dblWidth, strSymbolLayerPath, blnVisible);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="CGeoEb"></param>
        /// <param name="pesriGeometryType"></param>
        /// <param name="strFileName"></param>
        /// <param name="pWorkspace"></param>
        /// <param name="m_mapControl"></param>
        /// <param name="pstrFieldNameLt">pstrFieldNameLt should not contain "FID" and "Shape", these two attributes will be generated automatically</param>
        /// <param name="pesriFieldTypeLt"></param>
        /// <param name="pobjectValueLtLt"></param>
        /// <param name="intRed"></param>
        /// <param name="intGreen"></param>
        /// <param name="intBlue"></param>
        /// <param name="dblWidth"></param>
        /// <param name="blnVisible"></param>
        /// <returns></returns>
        public static IFeatureLayer SaveCGeoEb<T>(IEnumerable<T> CGeoEb, esriGeometryType pesriGeometryType, string strFileName,
            IWorkspace pWorkspace, IMapControl4 m_mapControl, List<string> pstrFieldNameLt = null, List<esriFieldType> pesriFieldTypeLt = null, 
            List<List<object>> pobjectValueLtLt = null, int intRed = 0, int intGreen = 0, int intBlue = 0, double dblWidth = 1, 
            string strSymbolLayerPath = null, bool blnVisible = true)
            where T : CGeometricBase<T>
        {
            if (CGeoEb.GetEnumerator().MoveNext() == false)
            {
                return null;
            }

            CSaveFeature pSaveFeature = new CSaveFeature(pesriGeometryType, strFileName, pWorkspace, m_mapControl, pstrFieldNameLt, pesriFieldTypeLt, intRed, intGreen, intBlue, dblWidth,strSymbolLayerPath, blnVisible);
            IFeatureLayer pFLayer = pSaveFeature.SaveCGeosToLayer(CGeoEb, pstrFieldNameLt, pobjectValueLtLt);

            return pFLayer;
        }


        


        public static IFeatureLayer SaveIGeoEb<T>(IEnumerable<T> IGeoEb, esriGeometryType pesriGeometryType, string strFileName,
            CParameterInitialize pParameterInitialize, List<string> pstrFieldNameLt = null, List<esriFieldType> pesriFieldTypeLt = null,
            List<List<object>> pobjectValueLtLt = null, int intRed = 0, int intGreen = 0, int intBlue = 0, double dblWidth = 1,
            string strSymbolLayerPath = null, bool blnVisible = true)
             where T : IGeometry
        {
            return SaveIGeoEb(IGeoEb, pesriGeometryType, strFileName, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl,
               pstrFieldNameLt, pesriFieldTypeLt, pobjectValueLtLt, intRed, intGreen, intBlue, dblWidth, strSymbolLayerPath, blnVisible);
        }

        private static IFeatureLayer SaveIGeoEb<T>(IEnumerable<T> IGeoEb, esriGeometryType pesriGeometryType, string strFileName,
            IWorkspace pWorkspace, IMapControl4 m_mapControl, List<string> pstrFieldNameLt = null, List<esriFieldType> pesriFieldTypeLt = null, 
            List<List<object>> pobjectValueLtLt = null, int intRed = 0, int intGreen = 0, int intBlue = 0, double dblWidth = 1, string strSymbolLayerPath = null, bool blnVisible = true)
            where T : IGeometry
        {
            if (IGeoEb.GetEnumerator().MoveNext() == false)
            {
                return null;
            }

            CSaveFeature pSaveFeature = new CSaveFeature(pesriGeometryType, strFileName, pWorkspace, m_mapControl, pstrFieldNameLt, pesriFieldTypeLt, intRed, intGreen, intBlue, dblWidth, strSymbolLayerPath, blnVisible);
            IFeatureLayer pFLayer = pSaveFeature.SaveIGeosToLayer(IGeoEb, pstrFieldNameLt, pobjectValueLtLt);

            return pFLayer;
        }


        public static IFeatureLayer SaveCEdgeEb(IEnumerable<CEdge> CGeoEb, string strFileName)
        {
            return SaveCGeoEb(CGeoEb, esriGeometryType.esriGeometryPolyline, strFileName);
        }
    }
}
