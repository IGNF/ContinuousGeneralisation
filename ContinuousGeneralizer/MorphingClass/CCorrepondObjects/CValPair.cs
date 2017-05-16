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
    public class CValPair<T1, T2> : IComparable<CValPair<T1, T2>>
    {
        public T1 val1 { get; set; }
        public T2 val2 { get; set; }
        public IComparer<T1> cmp1 { get; set; }
        public IComparer<T2> cmp2 { get; set; }

        public CValPair()
        {
        }

        public CValPair(T1 pVal1, T2 pVal2, IComparer<T1> pCmp1 = null, IComparer<T2> pCmp2 = null)
        {
            this.val1 = pVal1;
            this.val2 = pVal2;
            this.cmp1 = CHelpFunc.SetOrDefaultCmp(pCmp1);
            this.cmp2 = CHelpFunc.SetOrDefaultCmp(pCmp2);
        }

        public int CompareTo(CValPair<T1, T2> other)
        {
            return CCmpMethods.CmpDual(this, other, valpair => valpair.val1, valpair => valpair.val2, this.cmp1, this.cmp2);
        }


    }


    public class CPairValIncrease<T> : CValPair<T, T>
        where T: IComparable<T>
    {
        public IComparer<T> cmp { get; set; }
        public CPairValIncrease()
        {

        }


        public CPairValIncrease(T pVal1, T pVal2, IComparer<T> pCmp = null)
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
        }

        public int CompareTo(CPairValIncrease<T> other)
        {
            return CCmpMethods.CmpDual(this, other, valpair => valpair.val1, valpair => valpair.val2, this.cmp, this.cmp);
        }
    }



}
