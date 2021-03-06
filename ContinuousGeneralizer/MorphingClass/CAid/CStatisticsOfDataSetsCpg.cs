﻿using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using MorphingClass.CEntity;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CAid
{
    public class CStatisticsOfDataSetsCpg : CMorphingBaseCpg
    {


        public CStatisticsOfDataSetsCpg()
        {

        }


        public CStatisticsOfDataSetsCpg(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolygon>(ParameterInitialize, 0, 1);
        }


        public void StatisticsOfDataSets()
        {
            var ParameterInitialize = _ParameterInitialize;
            List<CPolygon> pCPgLt = this.ObjCGeoLtLt[0].Select(cgeo => cgeo as CPolygon).ToList();

            long lngStartTime = System.Environment.TickCount;
            double dblArea = 0;
            foreach (var cpg in pCPgLt)
            {
                var pArea = cpg.pPolygon as IArea;
                dblArea += pArea.Area;
            }
            Console.WriteLine("TotalArea: " + dblArea);


            long lngEndTime = System.Environment.TickCount;//记录结束时间
            ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时
        }
    }
}
