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
    public class CValPairIncr<T> : CValPair<T, T>
        where T: IComparable<T>
    {
        public IComparer<T> cmp { get; set; }
        public CValPairIncr()
        {

        }


        public CValPairIncr(T pVal1, T pVal2, IComparer<T> pCmp = null)
        {
            this.val1 = pVal1;
            this.val2 = pVal2;
            this.cmp = CHelpFunc.SetOrDefaultCmp(pCmp);

            //var 

            if (this.cmp.Compare(pVal1, pVal2) == 1)
            {
                this.val1 = pVal2;
                this.val2 = pVal1;
            }

            this.cmp1 = this.cmp;
            this.cmp2 = this.cmp;
        }

        //public int CompareTo(CValPairIncr<T> other)
        //{
        //    return CCmpMethods.CmpDual(this, other, valpair => valpair.val1, valpair => valpair.val2, this.cmp, this.cmp);
        //}
    }



}
