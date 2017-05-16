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
    public class CUnifyDirectionsPolylines : CMorphingBaseCpl
    {


        public CUnifyDirectionsPolylines()
        {

        }


        public CUnifyDirectionsPolylines(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline ,CPolyline>(ParameterInitialize, 2);
        }


        public void UnifyDirectionsPolylines()
        {
            CParameterInitialize ParameterInitialize = _ParameterInitialize;
            //List<CPolyline> pCPlLt = this.ObjCGeoLtLt[0].AsExpectedClass<CPolyline, object>().ToList();

            List<CPolyline> LSCPlLt = this.ObjCGeoLtLt[0].AsExpectedClass<CPolyline, object>().ToList();
            List<CPolyline> SSCPlLt = this.ObjCGeoLtLt[1].AsExpectedClass<CPolyline, object>().ToList();

            for (int i = 0; i < LSCPlLt.Count; i++)
            {
                double dblAngleDiff = CGeoFunc.CalAngle_Counterclockwise(LSCPlLt[i].FrCpt, LSCPlLt[i].ToCpt, SSCPlLt[i].FrCpt, SSCPlLt[i].ToCpt);

                if ((Math.Abs(dblAngleDiff) > (Math.PI / 2) && Math.Abs(dblAngleDiff) < (3 * Math.PI / 2)))
                {
                    LSCPlLt[i].ReverseCpl();
                }
            }

            CSaveFeature.SaveCGeoEb(LSCPlLt, esriGeometryType.esriGeometryPolyline, ParameterInitialize.pFLayerLt[0].Name + "UnifiedDirections", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

        }
    }
}
