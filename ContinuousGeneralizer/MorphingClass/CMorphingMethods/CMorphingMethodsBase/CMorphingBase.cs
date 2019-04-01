using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;

using MorphingClass.CAid;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CEvaluationMethods;

namespace MorphingClass.CMorphingMethods.CMorphingMethodsBase
{
    public class CMorphingBase
    {
        protected string _strMorphingMethod;
        protected CEvaluation _pEvaluation;

        protected int _intLayerCount = 2;

        /// <summary>ObjIGeoLtLt is ready after we have read the data from layers</summary>        
        public List<List<object>> ObjIGeoLtLt { get; set; }
        /// <summary>ObjCGeoLtLt is not null only when blnIGeoToCGeo == true</summary>  
        public List<List<CGeoBase>> ObjCGeoLtLt { get; set; }
        public List<List<string>> strFieldNameLtLt { get; set; }
        public List<List<esriFieldType>> esriFieldTypeLtLt { get; set; }
        protected List<List<List<object>>> _ObjValueLtLtLt;
        protected List<CEdge> _AllReadCEdgeLt;

        protected List<List<CCorrCpts>> _CorrCptsLtLt;
        protected List<List<CCorrCpts>> _SgCorrCptsLtLt;

        //protected SortedDictionary<int, ISymbol> _intTypeFillSymbolSD;

        protected CParameterResult _ParameterResult;
        protected CParameterInitialize _ParameterInitialize;

        protected CPoint _StandardVectorCpt;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T">As long as T inherits from CPolyBase, 
        /// it doesn't matter in any case if T is CPolyline, CPolygon, or something else</typeparam>
        /// <typeparam name="CGeo"></typeparam>
        /// <param name="ParameterInitialize"></param>
        /// <param name="intLayerCount"></param>
        /// <param name="intStartLayer"></param>
        /// <param name="blnIGeoToCGeo"></param>
        /// <param name="strSpecifiedFieldName">read the features which have the specified values of the specified attribute</param>
        /// <param name="strSpecifiedValue"></param>
        public virtual void Construct<T>(CParameterInitialize ParameterInitialize, int intStartLayer = 0, int intLayerCount = 2, 
            bool blnIGeoToCGeo = true, string strSpecifiedFieldName = null, 
            string strSpecifiedValue = null, bool blnCreateFileGdbWorkspace = false, bool blnCalDistanceParameters=true) 
            where T : CPolyBase
        {
            IMap pm_mapFeature = ParameterInitialize.m_mapFeature;

            var FLayerLt = new List<IFeatureLayer>(intLayerCount);
            for (int i = intStartLayer; i < intStartLayer + intLayerCount; i++)
            {
                var pFLayer = (IFeatureLayer)pm_mapFeature.
                    get_Layer(ParameterInitialize.cboLayerLt[i].SelectedIndex);  //larger-scale layer
                FLayerLt.Add(pFLayer);
            }

            Construct<T>(ParameterInitialize, FLayerLt, blnIGeoToCGeo,
                strSpecifiedFieldName, strSpecifiedValue, blnCreateFileGdbWorkspace, blnCalDistanceParameters);
        }


        public virtual void Construct<T>(CParameterInitialize ParameterInitialize, List<IFeatureLayer> FLayerLt, 
            bool blnIGeoToCGeo = true, string strSpecifiedFieldName = null,
             string strSpecifiedValue = null, bool blnCreateFileGdbWorkspace = false, bool blnCalDistanceParameters = true)
             where T : CPolyBase
        {
            Console.WriteLine("");
            Console.WriteLine("memory consumption: " + Math.Round(Convert.ToDouble(GC.GetTotalMemory(true)) / 1048576, 1) + "MB");
            CHelpFunc.SetSavePath(ParameterInitialize, blnCreateFileGdbWorkspace);


            ParameterInitialize.pFLayerLt = FLayerLt;

            //int intMinCount = Math.Min(pm_mapFeature.LayerCount, intLayerCount);   //need to be improved
            var intLayerCount = FLayerLt.Count;
            var pObjIGeoLtLt = new List<List<object>>(intLayerCount);
            var pstrFieldNameLtLt = new List<List<string>>(intLayerCount);
            var pesriFieldTypeLtLt = new List<List<esriFieldType>>(intLayerCount);
            var pObjValueLtLtLt = new List<List<List<object>>>(intLayerCount);


            foreach (var pFLayer in FLayerLt)
            {
                pstrFieldNameLtLt.Add(GetFieldNameLt(pFLayer.FeatureClass));
                pesriFieldTypeLtLt.Add(GetFieldTypeLt(pFLayer.FeatureClass));
                pObjIGeoLtLt.Add(CHelpFunc.GetObjLtByFeatureLayer(
                    pFLayer, out List<List<object>> pObjValueLtLt, strSpecifiedFieldName, strSpecifiedValue));
                pObjValueLtLtLt.Add(pObjValueLtLt);
                
            }

            List<CGeoBase> pFirstLayerObjCGeoLt = null;
            if (blnIGeoToCGeo == true)
            {
                var pObjCGeoLtLt = new List<List<CGeoBase>>(intLayerCount);
                for (int i = 0; i < pObjIGeoLtLt.Count; i++)
                {
                    pObjCGeoLtLt.Add(CHelpFunc.GenerateCGeoEbAccordingToInputLt(pObjIGeoLtLt[i]).ToList());
                }
                this.ObjCGeoLtLt = pObjCGeoLtLt;

                //we get the data in order to compute the Distance Parameters
                pFirstLayerObjCGeoLt = pObjCGeoLtLt[0];
            }
            else
            {
                //we get the data in order to compute the Distance Parameters
                pFirstLayerObjCGeoLt = CHelpFunc.GenerateCGeoEbAccordingToInputLt(pObjIGeoLtLt[0]).ToList();
            }

            //we compute the parameters of distance according to the features in the first layer
            CGeoFunc.CalDistanceParameters<T>
                (pFirstLayerObjCGeoLt.Select(obj => obj as T).ToList(), blnCalDistanceParameters);



            //_CPlLt = pObjCGeoLtLt[0];
            this.ObjIGeoLtLt = pObjIGeoLtLt;
            this.strFieldNameLtLt = pstrFieldNameLtLt;
            this.esriFieldTypeLtLt = pesriFieldTypeLtLt;
            

            _ObjValueLtLtLt = pObjValueLtLtLt;
            _ParameterInitialize = ParameterInitialize;
        }


        private static List<string> GetFieldNameLt(IFeatureClass pFeatureClass)
        {
            var strFieldNameLt = new List<string>(pFeatureClass.Fields.FieldCount);
            for (int i = 2; i < pFeatureClass.Fields.FieldCount; i++)  //we don't need the first two values, i.e., Id and shape
            {
                strFieldNameLt.Add(pFeatureClass.Fields.get_Field(i).Name);
            }
            return strFieldNameLt;
        }

        private static List<esriFieldType> GetFieldTypeLt(IFeatureClass pFeatureClass)
        {
            var esriFieldTypeLt = new List<esriFieldType>(pFeatureClass.Fields.FieldCount);
            for (int i = 2; i < pFeatureClass.Fields.FieldCount; i++)  //we don't need the first two values, i.e., Id and shape
            {
                esriFieldTypeLt.Add(pFeatureClass.Fields.get_Field(i).Type);
            }
            return esriFieldTypeLt;
        }

        public List<CEdge> GetAllReadCEdgeLt<CGeo>()
                        where CGeo : class
        {
            var pAllReadCEdgeLt = new List<CEdge>();
            foreach (var objlt in this.ObjCGeoLtLt)
            {
                foreach (var obj in objlt)
                {
                    var cpb = obj as CPolyBase;
                    cpb.JudgeAndFormCEdgeLt();
                    pAllReadCEdgeLt.AddRange(cpb.CEdgeLt);
                }
            }

            _AllReadCEdgeLt = pAllReadCEdgeLt;
            return pAllReadCEdgeLt;
        }

        //public virtual List<TCGeo> GenerateInterpolatedLt(double dblPropotion)
        //{
        //    return null;
        //}

        //public virtual List<TCGeo> GenerateInterpolatedLt(double dblPropotion, List<List<CCorrCpts>> pCorrCptsLtLt)
        //{
        //    return null;
        //}


        protected CPoint SetStandardVectorCpt(int intIndex, CPoint frcpt=null , CPoint tocpt=null )
        {
            CPoint StandardVectorCpt = new CPoint(0, 0, 0);
            switch (intIndex)
            {
                case 0:
                    break;
                case 1:
                    StandardVectorCpt.X = tocpt.X - frcpt.X;
                    StandardVectorCpt.Y = tocpt.Y - frcpt.Y;
                    break;
                default:
                    break;
            }

            _StandardVectorCpt = StandardVectorCpt;
            return StandardVectorCpt;
        }


        public string strMorphingMethod
        {
            get { return _strMorphingMethod; }
            //set { _strMorphingMethod = value; }
        }

        public List<List<List<object>>> ObjValueLtLtLt
        {
            get { return _ObjValueLtLtLt; }
            //set { _ObjValueLtLtLt = value; }
        }

        public List<CEdge> AllReadCEdgeLt
        {
            get { return _AllReadCEdgeLt; }
            //set { _LSCPlLt = value; }
        }

        public List<List<CCorrCpts>> CorrCptsLtLt
        {
            get { return _CorrCptsLtLt; }
            //set { _CorrCptsLtLt = value; }
        }

        public List<List<CCorrCpts>> SgCorrCptsLtLt
        {
            get { return _SgCorrCptsLtLt; }
            //set { _CorrCptsLtLt = value; }
        }


        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            //set { _ParameterResult = value; }
        }

        public CPoint StandardVectorCpt
        {
            get { return _StandardVectorCpt; }
            set { _StandardVectorCpt = value; }
        }
    }
}
