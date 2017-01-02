using System;
using System.Collections.Generic;
using System.Diagnostics;
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

using MorphingClass.CAid;
using MorphingClass.CEntity;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CGeneralizationMethods
{
    public class CAreaAggregation_Base : CMorphingBaseCpg
    {
        public static double dblLamda = 0.5;
        //public static double dblLamda2 = 1 - dblLamda1;

        //for some prompt settings
        protected int _intFactor = 1;
        //private int _intFactor = 8;

        protected static int _intStart; //=0
        protected static int _intEnd; //=this.SSCrgLt.Count
        protected void UpdateStartEnd()
        {
            _intStart = 248;
            _intEnd = _intStart + 1;

            dblLamda = 0.5;
            //dblLamda2 = 1 - dblLamda1;
        }

        public List<CRegion> InitialCrgLt { set; get; }
        public List<CRegion> LSCrgLt { set; get; }
        public List<CRegion> SSCrgLt { set; get; }

        //public int intTotalTimeNum { set; get; } //Note that intTotalTimeNum may count a step that only changes the type of a polygon (without aggregation)

        protected double[,] _adblTD;
        protected CPairVal_SD<int, int> _TypePVSD;

        public double dblCost { set; get; }
        public CStrObjLtSD StrObjLtSD { set; get; }

        //if we change the list, we may need to change the comparer named CAACCompare
        public static IList<string> strKeyLt = new List<string>
        {
            "ID",
            "n",
            "m",
            "Factor",
            "#Edges",
            "#Nodes",
            "EstType",
            "CostType",
            "RatioTypeCE",
            "EstComp",
            "CostComp",
            "RatioCompCE",
            "RatioTypeComp",
            "WeightedSum",
            "TimeFirst(ms)",
            "TimeLast(ms)",
            "Time(ms)",
            "Memory(MB)"
        };

        protected static int _intDigits = 6;



        protected void Preprocessing(CParameterInitialize ParameterInitialize, string strSpecifiedFieldName = null, string strSpecifiedValue = null)
        {
            Construct<CPolygon, CPolygon>(ParameterInitialize, 2, 0, true,1, strSpecifiedFieldName, strSpecifiedValue);
            CConstants.strShapeConstraint = ParameterInitialize.cboShapeConstraint.Text;
            if (CConstants.strShapeConstraint == "MaximizeMinimumCompactness" || CConstants.strShapeConstraint == "MaximizeMinimumCompactness_Combine" ||
                CConstants.strShapeConstraint == "MaximizeAverageCompactness" || CConstants.strShapeConstraint == "MaximizeAverageCompactness_Combine")
            {
                CConstants.blnComputeCompactness = true;
            }

            if (ParameterInitialize.chkSmallest.Checked == true)
            {
                ParameterInitialize.strAreaAggregation = "Smallest";
            }
            else
            {
                ParameterInitialize.strAreaAggregation = "All";
            }

            //read type distance
            var aObj = CHelperFunctionExcel.ReadDataFromExcel(ParameterInitialize.strPath + "TypeDistance.xlsx");
            if (aObj == null) throw new ArgumentNullException("Failed to read TypeDistance.xlsx");

            int intDataRow = aObj.GetUpperBound(0);
            int intDataCol = intDataRow;  //note that intDataRow == intDataCol

            //set an index for each type, so that we can access a type distance directly
            //var intTypeIndexSD = new SortedDictionary<int, int>();
            var pTypePVSD = new CPairVal_SD<int, int>();
            int intTypeIndex = 0;
            for (int i = 0; i < intDataRow; i++)
            {
                int intType = Convert.ToInt32(aObj[i + 1][0]);
                if (pTypePVSD.SD.ContainsKey(intType) == false)
                {
                    pTypePVSD.SD.Add(intType, intTypeIndex++);
                }
            }
            pTypePVSD.CreateSD_R();
            _TypePVSD = pTypePVSD;

            var adblTypeDistance = new double[intDataRow, intDataCol];
            for (int i = 0; i < intDataRow; i++)
            {
                for (int j = 0; j < intDataCol; j++)
                {
                    adblTypeDistance[i, j] = Convert.ToDouble(aObj[i + 1][j + 1]);
                }
            }

            _adblTD = adblTypeDistance;

        }


        protected void SetupBasic()
        {

            var pLSCPgLt = this.ObjCGeoLtLt[0].ToExpectedClass<CPolygon, object>().ToList();
            var pSSCPgLt = this.ObjCGeoLtLt[1].ToExpectedClass<CPolygon, object>().ToList();

            //this.intTotalTimeNum = pLSCPgLt.Count - pSSCPgLt.Count + 1;

            foreach (var cpg in pLSCPgLt)
            {
                cpg.FormCEdgeLtLt();
                cpg.SetCEdgeLtLtLength();
            }

            foreach (var cpg in pSSCPgLt)
            {
                cpg.FormCEdgeLtLt();
            }

            //get region number for each polygon
            var pstrFieldNameLtLt = this.strFieldNameLtLt;
            var pObjValueLtLtLt = this.ObjValueLtLtLt;
            //var intTypeIndexSD=_intTypeIndexSD;

            var intLSTypeATIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[0], "OBJART");  //RegionNumATIndex: the index of RegionNum in the attribute table 
            var intSSTypeATIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[1], "OBJART");
            //var CgbEb=pLSCPgLk.ToExpectedClass<CGeometricBase<CPolygon>, CGeometricBase<CPolygon>>();
            CHelperFunction.GetCgbTypeAndTypeIndex(pLSCPgLt.ToExpectedClass<CPolygon, CPolygon>(), _ObjValueLtLtLt[0], 0, _TypePVSD);
            CHelperFunction.GetCgbTypeAndTypeIndex(pSSCPgLt.ToExpectedClass<CPolygon, CPolygon>(), _ObjValueLtLtLt[1], 0, _TypePVSD);


            var intLSRegionNumATIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[0], "RegionNum");  //RegionNumATIndex: the index of RegionNum in the attribute table 
            var intSSRegionNumATIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[1], "RegionNum");
            //private CPairVal_SD<int, int> _RegionPVSD;
            var pRegionPVSD = new CPairVal_SD<int, int>();
            int intRegionIndex = 0;
            for (int i = 0; i < pObjValueLtLtLt[1].Count; i++)
            {
                int intRegionNum = Convert.ToInt32(pObjValueLtLtLt[1][i][intSSRegionNumATIndex]);
                if (pRegionPVSD.SD.ContainsKey(intRegionNum) == false)
                {
                    pRegionPVSD.SD.Add(intRegionNum, intRegionIndex++);
                }
            }

            //ssign the polygons as well as attributes from a featureLayer into regions, without considering costs
            this .LSCrgLt = GenerateCrgLt(pLSCPgLt, pSSCPgLt.Count, pObjValueLtLtLt[0], intLSTypeATIndex, intLSRegionNumATIndex, _TypePVSD, pRegionPVSD);
            this. SSCrgLt = GenerateCrgLt(pSSCPgLt, pSSCPgLt.Count, pObjValueLtLtLt[1], intSSTypeATIndex, intSSRegionNumATIndex, _TypePVSD, pRegionPVSD);

            using (var writer = new System.IO.StreamWriter(_ParameterInitialize.strSavePathBackSlash + CHelperFunction.GetTimeStamp()
                + "_" + "AreaAggregation.txt", false))
            {
                writer.Write(_ParameterInitialize.strAreaAggregation);
            }

            //apply A* algorithm to each region
            this.InitialCrgLt = new List<CRegion>(pSSCPgLt.Count);
            //var ResultCrgLt = new List<CRegion>(pSSCPgLt.Count);
            this.StrObjLtSD = new CStrObjLtSD(CCAMDijkstra.strKeyLt, pSSCPgLt.Count);

            _intStart = 0;
            _intEnd = this.SSCrgLt.Count;

            UpdateStartEnd();

        }

        #region Common

        protected void AddLineToStrObjLtSD(CStrObjLtSD StrObjLtSD, CRegion LSCrg)
        {
            var et = StrObjLtSD.GetEnumerator();
            while (et.MoveNext())
            {
                et.Current.Value.Add(-1);
            }

            StrObjLtSD.SetLastObj("ID", LSCrg.ID);
            StrObjLtSD.SetLastObj("n", LSCrg.CphTypeIndexSD_Area_CphGID.Count);
            StrObjLtSD.SetLastObj("m", LSCrg.Adjacency_CorrCphsSD.Count);
            StrObjLtSD.SetLastObj("Factor", 100000000);
        }


        /// <summary>
        /// assign the polygons as well as attributes from a featureLayer into regions, without considering costs
        /// </summary>
        /// <param name="pCpgLt"></param>
        /// <param name="intCrgNum"></param>
        /// <param name="pObjValueLtLt"></param>
        /// <param name="intTypeATIndex"></param>
        /// <param name="intRegionNumATIndex"></param>
        /// <param name="pTypePVSD"></param>
        /// <param name="pRegionPVSD"></param>
        /// <returns></returns>
        protected List<CRegion> GenerateCrgLt(List<CPolygon> pCpgLt, int intCrgNum, List<List<object>> pObjValueLtLt, int intTypeATIndex, int intRegionNumATIndex, CPairVal_SD<int, int> pTypePVSD, CPairVal_SD<int, int> pRegionPVSD)
        {
            var pCrgLt = new List<CRegion>(intCrgNum);
            pCrgLt.EveryElementNew();

            for (int i = 0; i < pCpgLt.Count; i++)
            {
                //get the type index
                int intType = Convert.ToInt32(pObjValueLtLt[i][intTypeATIndex]);
                int intTypeIndex;
                pTypePVSD.SD.TryGetValue(intType, out intTypeIndex);

                //get the RegionNum index
                var intRegionNum = Convert.ToInt32(pObjValueLtLt[i][intRegionNumATIndex]);
                int intRegionIndex;
                pRegionPVSD.SD.TryGetValue(intRegionNum, out intRegionIndex);

                //add the Cph into the corresponding Region
                pCrgLt[intRegionIndex].AddCph(new CPatch(pCpgLt[i], -1, intTypeIndex), intTypeIndex);
                pCrgLt[intRegionIndex].ID = intRegionNum;  //set the ID for each region
            }

            //set the ID of patches
            foreach (var crg in pCrgLt)
            {
                int intCount = 0;
                foreach (var cph in crg.CphTypeIndexSD_Area_CphGID.Keys)
                {
                    cph.ID = intCount++;
                }
            }
            return pCrgLt;
        }
        #endregion
    }
}
