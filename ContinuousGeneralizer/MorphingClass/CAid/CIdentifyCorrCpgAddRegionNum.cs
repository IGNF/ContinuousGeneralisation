using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using MorphingClass.CEntity;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CAid
{
    public class CIdentifyCorrCpgAddRegionNum : CMorphingBaseCpg
    {


        public CIdentifyCorrCpgAddRegionNum()
        {

        }


        public CIdentifyCorrCpgAddRegionNum(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolygon, CPolygon>(ParameterInitialize, 2);
        }


        public void IdentifyCorrCpgAddRegionNum()
        {
            
            CParameterInitialize ParameterInitialize = _ParameterInitialize;
            var pLSCPgLt = this.ObjCGeoLtLt[0].ToExpectedClass<CPolygon, object>().ToList();
            var pSSCPgLt = this.ObjCGeoLtLt[1].ToExpectedClass<CPolygon, object>().ToList();

            long lngStartTime = System.Environment.TickCount;
            List<CPolygon> LSAttentionCPgLt = new List<CPolygon>();
            List<CPolygon> SSAttentionCPgLt = new List<CPolygon>();
            CMatchAndMergePolygons.MatchCpg(ref pLSCPgLt, ref pSSCPgLt, ref LSAttentionCPgLt, ref SSAttentionCPgLt);


            List<object> LSRgNumLt = new List<object>(pLSCPgLt.Count);
            LSRgNumLt.EveryElementValue(-1);
            List<object> SSRgNumLt = new List<object>(pSSCPgLt.Count);


            foreach (var sscpg in pSSCPgLt)
            {
                SSRgNumLt.Add(sscpg.ID);

                foreach (var corrcgeo in sscpg.CorrCGeoLt)
                {
                    LSRgNumLt[corrcgeo.ID] = sscpg.ID;
                }
            }

            CSaveFeature.AddFieldandAttribute(ParameterInitialize.pFLayerLt[0].FeatureClass, esriFieldType.esriFieldTypeInteger, "RegionNum", LSRgNumLt);
            CSaveFeature.AddFieldandAttribute(ParameterInitialize.pFLayerLt[1].FeatureClass, esriFieldType.esriFieldTypeInteger, "RegionNum", SSRgNumLt);



            CSaveFeature.SaveCGeoEb(SSAttentionCPgLt, esriGeometryType.esriGeometryPolygon, ParameterInitialize.cboLayerLt[1].Text + "_Attention", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            CSaveFeature.SaveCGeoEb(LSAttentionCPgLt, esriGeometryType.esriGeometryPolygon, ParameterInitialize.cboLayerLt[0].Text + "_Attention", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }



    }
}
