using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Maplex;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;

namespace MorphingClass.CUtility
{
    public class CStrObjLtDt: Dictionary<string ,List<object>>
    {
        public CStrObjLtDt(IEnumerable<string> strKeyEb, int intCount = 0)
        {
            var strKeyEt = strKeyEb.GetEnumerator();
            while (strKeyEt.MoveNext())
            {
                var objlt = new List<object>(intCount);
                this.Add(strKeyEt.Current, objlt);
            }
        }


        public void AddObj(string strKey, object obj)
        {
            List<object> objlt;
            if (this.TryGetValue(strKey, out objlt)==false)
            {
                throw new ArgumentOutOfRangeException("Specified key doesn't exist!");   
            }
            objlt.Add(obj);
        }

        public void SetLastObj(string strKey, object obj)
        {
            List<object> objlt;
            if (this.TryGetValue(strKey, out objlt) == false)
            {
                throw new ArgumentOutOfRangeException("Specified key doesn't exist!");
            }
            objlt.SetLastT(obj);
        }

        public void Merge(CStrObjLtDt StrObjLtDt)
        {
            var et1 = this.GetEnumerator();
            var et2 = StrObjLtDt.GetEnumerator();

            while (et1.MoveNext() && et2.MoveNext())
            {
                et1.Current.Value.AddRange(et2.Current.Value);
            }

            
        }
    }
}
