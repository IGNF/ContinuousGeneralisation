using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CEntity;

namespace MorphingClass.CCorrepondObjects
{
    public class CCorrespondObject
    {
       object _obj1=new object() ;
        object _obj2 = new object();


        public CCorrespondObject(object pobj1, object pobj2)
        {
            _obj1 = pobj1;
            _obj2 = pobj2;
        }

        /// <summary>属性：对象1</summary>
        public object obj1
        {
            get { return _obj1; }
            set { _obj1 = value; }
        }

        /// <summary>属性：对象2</summary>
        public object obj2
        {
            get { return _obj2; }
            set { _obj2 = value; }
        }





    }
}
