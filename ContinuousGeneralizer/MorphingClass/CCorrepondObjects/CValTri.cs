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

using MorphingClass.CUtility;

namespace MorphingClass.CCorrepondObjects
{
    public class CValTri<T1, T2, T3> 
    {
        public T1 valActive { get; set; }
        public T2 valPassive { get; set; }
        public T3 valResult { get; set; }
        //public IComparer<T1> cmp1 { get; set; }
        //public IComparer<T2> cmp2 { get; set; }

        public CValTri()
        {
        }

        public CValTri(T1 valactive, T2 valpassive, T3 valresult
            /*, IComparer<T1> pCmp1 = null, IComparer<T2> pCmp2 = null*/)
        {
            this.valActive = valactive;
            this.valPassive = valpassive;
            this.valResult = valresult;
        }

        //public int CompareTo(CValPair<T1, T2> other)
        //{
        //    return CCmpMethods.CmpDual(this, other, valpair => valpair.val1, valpair => valpair.val2, this.cmp1, this.cmp2);
        //}
    }
   
}
