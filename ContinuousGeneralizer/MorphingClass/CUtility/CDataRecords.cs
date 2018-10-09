using System;
using System.Collections.Generic;
using System.Text;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DataSourcesFile;


namespace MorphingClass.CUtility
{
    public class CDataRecords
    {
        private CParameterInitialize _ParameterInitialize = new CParameterInitialize();
        private CParameterResult _ParameterResult = new CParameterResult();
        private CParameterResult _ParameterResultToExcel = new CParameterResult();
        private List<CParameterResult> _ParameterResultToExcelLt = new List<CParameterResult>();

        /// <summary>属性：初始化参数类</summary>
        public CParameterInitialize ParameterInitialize
        {
            get { return _ParameterInitialize; }
            set { _ParameterInitialize = value; }
        }

        /// <summary>属性：结果参数类</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }

        /// <summary>属性：导出到EXCEL结果参数类</summary>
        public CParameterResult ParameterResultToExcel
        {
            get { return _ParameterResultToExcel; }
            set { _ParameterResultToExcel = value; }
        }

        /// <summary>属性：导出到EXCEL结果参数类</summary>
        public List<CParameterResult> ParameterResultToExcelLt
        {
            get { return _ParameterResultToExcelLt; }
            set { _ParameterResultToExcelLt = value; }
        }

        /// <summary>初始化函数</summary>
        public CDataRecords()
        {
            //ParameterResult = new CParameterResult();
        }


    }
}
