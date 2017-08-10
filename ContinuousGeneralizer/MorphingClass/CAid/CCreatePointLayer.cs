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
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CAid
{
    public class CCreatePointLayer : CMorphingBaseCpl
    {
        public CCreatePointLayer()
        {

        }


        public CCreatePointLayer(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline, CPolyline>(ParameterInitialize, 1);
            GetAllReadCPlLt();
        }


        public void CreatePointLayer()
        {
            CParameterInitialize ParameterInitialize = _ParameterInitialize;
            List<CPolyline> CPolylineLt = _AllReadCPlLt;

            //long lngStartTime = System.Environment.TickCount; //记录开始时间
             //pstrFieldNameLt, pesriFieldTypeLt, pobjectValueLtLt,

            var pstrFieldNameLt = new List<string> { "PID" };
            var pesriFieldTypeLt = new List<esriFieldType> { esriFieldType.esriFieldTypeInteger };
            var pobjectValueLtLt = new List<List<object>>(CPolylineLt.Count);

            //读取线数据
            //List<List<CPoint>> cptltlt = new List<List<CPoint>>(CPolylineLt.Count);
            List<CPoint> cptlt = new List<CPoint>();
            for (int i = 0; i < CPolylineLt.Count; i++)
            {
                cptlt.AddRange(CPolylineLt[i].CptLt);
                
                for (int j = 0; j < CPolylineLt[i].CptLt .Count; j++)
                {
                    //var pobjectValueLt = new List<object>();
                    //pobjectValueLt.Add(j);
                    //pobjectValueLtLt.Add(pobjectValueLt);

                    pobjectValueLtLt.Add(new List<object> { j });
                }                
            }
            CSaveFeature.SaveCGeoEb(cptlt, esriGeometryType.esriGeometryPoint, ParameterInitialize.strSaveFolderName + "Points", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl, pstrFieldNameLt, pesriFieldTypeLt, pobjectValueLtLt);



            //CSaveFeature.SaveCGeoEb(cptltlt[0], esriGeometryType.esriGeometryPoint, ParameterInitialize.strSaveFolder + "Points", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            ////确定保存信息
            //CSaveFeature pSaveFeature = new CSaveFeature("PtOnPl", strPath, ParameterInitialize.m_mapControl, esriGeometryType.esriGeometryPoint);
            //List<List<object>> objectvalueltlt = new List<List<object>>();
            //List<object> objshapelt = new List<object>();
            //for (int i = 0; i < cptltlt.Count; i++)
            //{
            //    for (int j = 0; j < cptltlt[i].Count; j++)
            //    {
            //        objshapelt.Add(cptltlt[i][j]);
            //        List<object> objvaluelt = new List<object>();//每个要素有一个数据记录
            //        objvaluelt.Add(j);
            //        objectvalueltlt.Add(objvaluelt);
            //    }
            //}

            //List<esriFieldType> esriFieldTypeLt = new List<esriFieldType>();
            //esriFieldTypeLt.Add(esriFieldType.esriFieldTypeInteger);
            //List<string> strFieldNameLt = new List<string>();
            //strFieldNameLt.Add("OnID");

            //pSaveFeature.esriFieldTypeLt = esriFieldTypeLt;
            //pSaveFeature.objectShapeLt = objshapelt;
            //pSaveFeature.objectValueLtLt = objectvalueltlt;
            //pSaveFeature.strFieldNameLt = strFieldNameLt;

            ////保存点数据为要素图层
            //pSaveFeature.SaveFeaturesToLayer();

            //long lngEndTime = System.Environment.TickCount;//记录结束时间
            //_DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时
        }
    }
}
