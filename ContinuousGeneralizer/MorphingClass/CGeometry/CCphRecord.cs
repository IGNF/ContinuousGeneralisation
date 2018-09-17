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

using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CGeometry
{
    public class CCphRecord
    {
        public CPatch Cph { get; set; }
        public CCorrCphs CorrCphs { get; set; }

        public CCphRecord(CPatch cph, CCorrCphs corrcphs)
        {
            this.Cph = cph;
            this.CorrCphs = corrcphs;
        }
    }
}
