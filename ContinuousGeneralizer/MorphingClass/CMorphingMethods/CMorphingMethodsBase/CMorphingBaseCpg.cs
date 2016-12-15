using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CMorphingMethods.CMorphingMethodsBase
{
    public class CMorphingBaseCpg : CMorphingBase
    {
        protected List<CPolygon> _LSCPgLt;  //BS:LargerScale
        protected List<CPolygon> _SSCPgLt;  //SS:SmallerScale

    }
}
