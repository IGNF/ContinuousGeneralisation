using System;
using System.Collections.Generic;
using System.Text;

using MorphingClass.CGeometry ;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

namespace MorphingClass.CUtility
{
    public class CParameterVariable
    {
        private int _intIndex;
        private string _strPolyline;
        private CPolyline _CPolyline;
        private IFeatureLayer _pFeatureLayer;   //要素图层(一般为线要素及其外包多边形，用于生成CDT)
        private string _strName;
        private double _dblVerySmall;


        /// <summary>属性：序号</summary>
        public int intIndex
        {
            get { return _intIndex; }
            set { _intIndex = value; }
        }

        /// <summary>属性：极小值</summary>
        public double dblVerySmall
        {
            get { return _dblVerySmall; }
            set { _dblVerySmall = value; }
        }

        /// <summary>属性：名字</summary>
        public string strName
        {
            get { return _strName; }
            set { _strName = value; }
        }

        /// <summary>属性：线段名称</summary>
        public string strPolyline
        {
            get { return _strPolyline; }
            set { _strPolyline = value; }
        }

        /// <summary>属性：线要素</summary>
        public CPolyline CPolyline
        {
            get { return _CPolyline; }
            set { _CPolyline = value; }
        }

        /// <summary>属性：要素图层(一般为线要素及其外包多边形，用于生成CDT)</summary>
        public IFeatureLayer pFeatureLayer
        {
            get { return _pFeatureLayer; }
            set { _pFeatureLayer = value; }
        }



        public CParameterVariable(int vintIndex, string vstrPolyline)
        {
            _intIndex = vintIndex;
            _strPolyline = vstrPolyline;
        }

        public CParameterVariable(CPolyline vCPolyline, string vstrName, IFeatureLayer vpFeatureLayer,double vdblVerySmall)
        {
            _CPolyline = vCPolyline;
            _strName = vstrName;
            _pFeatureLayer = vpFeatureLayer;
            _dblVerySmall = vdblVerySmall;
        }

        public CParameterVariable(CPolyline vCPolyline, string vstrName, double vdblVerySmall)
        {
            _CPolyline = vCPolyline;
            _strName = vstrName;
            _dblVerySmall = vdblVerySmall;
        }

        public void CreateLayerByCpl()
        {


        }
    }
}
