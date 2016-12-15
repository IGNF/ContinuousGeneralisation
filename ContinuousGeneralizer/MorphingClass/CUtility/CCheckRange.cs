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
    public class CCheckRange : IComparable<CCheckRange>
    {
        public string strFileName { get; set; }
        public int intStartline { get; set; }
        public int intEndline { get; set; }
        public string strMethod { get; set; }

        public CCheckRange(string fstrFileName, int fintStartline, int fintEndline, string fstrMethod=null)
        {
            this.strFileName = fstrFileName;
            this.intStartline = fintStartline;
            this.intEndline = fintEndline;
            this.strMethod = fstrMethod;
        }

        public int CompareTo(CCheckRange other)
        {
           return  CCompareMethods.CompareDual(this, other, checkrange => checkrange.strFileName, checkrange => checkrange.intStartline);
        }

    }
}
