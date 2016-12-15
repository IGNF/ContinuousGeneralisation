using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.COMSupport;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;

using MorphingClass.CUtility;

namespace ContinuousGeneralizer.FrmAid
{
    /// <summary>
    /// compare the results of ILPs, with and without involving the smallest area.
    /// we compare how many areas for which we can find optimal solutions with and without involving the smallest area.
    /// we compare the average time that we need to compute for the areas that can be found optimal solutions by two methods
    /// </summary>
    public partial class FrmCompareExcel : Form
    {
        public FrmCompareExcel()
        {
            InitializeComponent();
        }

        private void btnRun_Click(object sender, EventArgs e)
        {
            string strFileNameAll = "C:\\Study\\MyWork\\00New Result\\20160829_Astar_ILP\\20160827_015953_ILP_All_MinimizeInteriorBoundaries_100000000_BothSolved";
            string strFileNameSmallest = "C:\\Study\\MyWork\\00New Result\\20160829_Astar_ILP\\20160828_030805_ILP_Smallest_MinimizeInteriorBoundaries_100000000_BothSolved";

            var DataAllSD = GetDataSD(strFileNameAll);
            var DataSmallestSD = GetDataSD(strFileNameSmallest);

            var DataAllEt = DataAllSD.Values.GetEnumerator();
            var DataSmallestEt = DataSmallestSD.Values.GetEnumerator();

            int intCountAll = 0;
            int intCountSmallest = 0;
            int intCountBoth = 0;
            double dblTimeAll = 0;
            double dblTimeSmallest = 0;
            while (DataAllEt.MoveNext() && DataSmallestEt.MoveNext())
            {
                var aobjall = DataAllEt.Current;
                var aobjsmallest = DataSmallestEt.Current;

                int intFactorAll = Convert.ToInt32(aobjall[4]);
                int intFactorSmallest = Convert.ToInt32(aobjsmallest[4]);
                if (intFactorAll == 1 && intFactorSmallest>1)
                {
                    intCountAll++;
                }
                else if (intFactorAll > 1 && intFactorSmallest==1)
                {
                    intCountSmallest++;
                }
                else if (intFactorAll == 1 && intFactorSmallest==1)
                {
                    intCountBoth++;

                    dblTimeAll += Convert.ToDouble(aobjall[aobjall.GetUpperBound(0) - 2]);
                    dblTimeSmallest += Convert.ToDouble(aobjsmallest[aobjsmallest.GetUpperBound(0) - 2]);
                }
            }

            double dblAvgAll = dblTimeAll / intCountBoth;
            double dblAvgSmallest = dblTimeSmallest / intCountBoth;
        }

        private SortedDictionary<int, object[]> GetDataSD(string strFileName)
        {
            var aObj = CHelperFunctionExcel.ReadDataFromExcel(strFileName);
            var DataSD = new SortedDictionary<int, object[]>();  //the elements will be sorted according to ID

            for (int i = 1; i < aObj.GetUpperBound(0); i++)
            {
                DataSD.Add(Convert.ToInt32(aObj[i][0]), aObj[i]);
            }

            return DataSD;
        }
    }
}
