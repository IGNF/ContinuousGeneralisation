using System;
using C5;
using System.Collections.Generic;
using System.Text;

using SCG = System.Collections.Generic;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;

using MorphingClass.CEntity;
using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CGeometry
{
    public class CMoveVector :CBasicBase
    {
        private double _dblX;
        private double _dblY;
        private double _dblZ;

        public CMoveVector (int intID, double dblX, double dblY)
        {
            _intID = intID;
            _dblX = dblX;
            _dblY = dblY;
        }


        public double DistanceTo(CMoveVector other)
        {
            return CGeometricMethods.CalDis(this, other);
        }


        public double X
        {
            get { return _dblX; }
            set { _dblX = value; }
        }

        public double Y
        {
            get { return _dblY; }
            set { _dblY = value; }
        }

        public double Z
        {
            get { return _dblZ; }
            set { _dblZ = value; }
        }
        

    }
}
