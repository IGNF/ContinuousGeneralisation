using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;


using MorphingClass.CEntity;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CAid
{
    public class CCreatePointLayer : CMorphingBase
    {
        public CCreatePointLayer()
        {

        }

        public CCreatePointLayer(CParameterInitialize ParameterInitialize, int intLayerCount = 1,
            int intStartLayer = 0, bool blnIGeoToCGeo = true)
        {
            Construct<CPolyBase>(ParameterInitialize, intLayerCount, intStartLayer);
        }


        public CCreatePointLayer(CParameterInitialize ParameterInitialize, List<IFeatureLayer> pFLayerLt)
        {
            Construct<CPolyBase>(ParameterInitialize, pFLayerLt);
        }


        public void CreatePointLayer()
        {
            var ParameterInitialize = _ParameterInitialize;
            for (int i = 0; i < ObjCGeoLtLt.Count; i++)
            {
                var cpblt = ObjCGeoLtLt[i].AsExpectedClassEb<CPolyBase, CGeoBase>().ToList();
                SavePointLayer(cpblt, ParameterInitialize.pFLayerLt[i].Name +"_Pt");
            }
        }


        private IFeatureLayer SavePointLayer(List<CPolyBase> CpbLt, string strName)
        {
            var pstrFieldNameLt = new List<string> { "PID" };
            var pesriFieldTypeLt = new List<esriFieldType> { esriFieldType.esriFieldTypeInteger };
            var pobjectValueLtLt = new List<List<object>>(CpbLt.Count);

            //读取线数据
            var cptlt = new List<CPoint>();
            for (int i = 0; i < CpbLt.Count; i++)
            {
                for (int j = 0; j < CpbLt[i].CptLt.Count; j++)
                {
                    cptlt.Add(CpbLt[i].CptLt[j]);
                    pobjectValueLtLt.Add(new List<object> { j });
                }
            }

            return CSaveFeature.SaveCptEb(cptlt, strName, pstrFieldNameLt, pesriFieldTypeLt, pobjectValueLtLt, dblWidth:5);
        }
    }
}
