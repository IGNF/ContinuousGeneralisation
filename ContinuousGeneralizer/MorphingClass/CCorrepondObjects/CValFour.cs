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
    public class CValFour<T1, T2, T3, T4> 
    {
        public T1 val1 { get; set; }
        public T2 val2 { get; set; }
        public T3 val3 { get; set; }
        public T4 val4 { get; set; }
        //public IComparer<T1> cmp1 { get; set; }
        //public IComparer<T2> cmp2 { get; set; }

        public CValFour()
        {
        }

        public CValFour(T1 pVal1, T2 pVal2, T3 pVal3, T4 pVal4
            /*, IComparer<T1> pCmp1 = null, IComparer<T2> pCmp2 = null*/)
        {
            this.val1 = pVal1;
            this.val2 = pVal2;
            this.val3 = pVal3;
            this.val4 = pVal4;
        }

        //public int CompareTo(CValPair<T1, T2> other)
        //{
        //    return CCmpMethods.CmpDual(this, other, valpair => valpair.val1, valpair => valpair.val2, this.cmp1, this.cmp2);
        //}
    }
   
}
