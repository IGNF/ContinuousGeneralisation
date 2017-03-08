using System;
using System.IO;
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
        protected int _intStartFactor = 1;
        //private int _intStartFactor = 8;

        protected static int _intStart; //=0
        protected static int _intEnd; //=this.SSCrgLt.Count
        protected void UpdateStartEnd()
        {
            //_intStart = 436;
            //_intEnd = _intStart + 1;

            dblLamda = 0.5;
            //dblLamda2 = 1 - dblLamda1;
        }

        public List<CRegion> InitialCrgLt { set; get; }
        public List<CRegion> LSCrgLt { set; get; }
        public List<CRegion> SSCrgLt { set; get; }

        //public int intTotalTimeNum { set; get; } //Note that intTotalTimeNum may count a step that only changes the type of a polygon (without aggregation)

        protected double[,] _adblTD;
        protected CValMap_SD<int, int> _TypePVSD;

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
            if (CConstants.strShapeConstraint == "MaximizeMinComp_EdgeNumber" || CConstants.strShapeConstraint == "MaximizeMinComp_Combine")
            {
                CConstants.blnComputeMinComp = true;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp_EdgeNumber" || CConstants.strShapeConstraint == "MaximizeAvgComp_Combine")
            {
                CConstants.blnComputeAvgComp = true;
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
            var pTypePVSD = new CValMap_SD<int, int>();
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
            //private CValMap_SD<int, int> _RegionPVSD;
            var pRegionPVSD = new CValMap_SD<int, int>();
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
            this.LSCrgLt = GenerateCrgLt(pLSCPgLt, pSSCPgLt.Count, pObjValueLtLtLt[0], intLSTypeATIndex, intLSRegionNumATIndex, _TypePVSD, pRegionPVSD);
            this.SSCrgLt = GenerateCrgLt(pSSCPgLt, pSSCPgLt.Count, pObjValueLtLtLt[1], intSSTypeATIndex, intSSRegionNumATIndex, _TypePVSD, pRegionPVSD);

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
            StrObjLtSD.SetLastObj("m", LSCrg.AdjCorrCphsSD.Count);
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
        protected List<CRegion> GenerateCrgLt(List<CPolygon> pCpgLt, int intCrgNum, List<List<object>> pObjValueLtLt, int intTypeATIndex, int intRegionNumATIndex, CValMap_SD<int, int> pTypePVSD, CValMap_SD<int, int> pRegionPVSD)
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


        #region Output


        protected void RecordResultForCrg(CStrObjLtSD StrObjLtSD, CRegion LSCrg, CRegion FinalOneCphCrg, int intSSTypeIndex)
        {
            if (FinalOneCphCrg.GetSoloCphTypeIndex() != intSSTypeIndex)
            {
                throw new ArgumentException("type is not correct!");
            }

            SetRegionChild(FinalOneCphCrg);
            AdjustCost(FinalOneCphCrg, 2);

            double dblRoundedCostEstimatedType = Math.Round(LSCrg.dblCostEstType, _intDigits);
            double dblRoundedCostExactType = Math.Round(FinalOneCphCrg.dblCostExactType, _intDigits);
            double dblRoundedCostEstComp = Math.Round(LSCrg.dblCostEstComp, _intDigits);
            double dblRoundedCostExactComp = Math.Round(FinalOneCphCrg.dblCostExactComp, _intDigits);

            double dblRatioTypeCE = 1;
            double dblRatioCompCE = 1;
            double dblRatioTypeComp = 1;

            if (LSCrg.GetCphCount() > 1)
            {
                if (LSCrg.dblCostEstType > 0)  //if LSCrg.dblCostEstType == 0, then we define dblRatioTypeCE = 1
                {
                    dblRatioTypeCE = Math.Round(FinalOneCphCrg.dblCostExactType / LSCrg.dblCostEstType, _intDigits);
                }

                dblRatioCompCE = Math.Round(FinalOneCphCrg.dblCostExactComp / LSCrg.dblCostEstComp, _intDigits);
                dblRatioTypeComp = Math.Round(FinalOneCphCrg.dblCostExactType / FinalOneCphCrg.dblCostExactComp, _intDigits);
            }

            if (CConstants.strMethod == "Greedy")
            {
                dblRoundedCostEstimatedType = -1;
                dblRatioTypeCE = -1;
                dblRoundedCostEstComp = -1;
                dblRatioCompCE = -1;
            }

            StrObjLtSD.SetLastObj("#Nodes", CRegion._intNodeCount);
            StrObjLtSD.SetLastObj("EstType", dblRoundedCostEstimatedType);
            StrObjLtSD.SetLastObj("CostType", dblRoundedCostExactType);
            StrObjLtSD.SetLastObj("RatioTypeCE", dblRatioTypeCE);
            StrObjLtSD.SetLastObj("EstComp", dblRoundedCostEstComp);
            StrObjLtSD.SetLastObj("CostComp", dblRoundedCostExactComp);
            StrObjLtSD.SetLastObj("RatioCompCE", dblRatioCompCE);
            StrObjLtSD.SetLastObj("RatioTypeComp", dblRatioTypeComp);
            StrObjLtSD.SetLastObj("WeightedSum", Math.Round(FinalOneCphCrg.dblCostExact, _intDigits));

        }

        /// <summary>
        /// after A star algorithm, we set the aggregation chain for each region
        /// </summary>
        /// <param name="pCrgLt"></param>
        private void SetRegionChild(CRegion crg)
        {
            while (crg.parent != null)
            {
                var parentcrg = crg.parent;
                parentcrg.child = crg;

                crg = parentcrg;
            }

            this.InitialCrgLt.Add(crg);
        }

        private void AdjustCost(CRegion crg, int intEvaluationNum)
        {
            double dblAdjust = crg.dblArea;
            do
            {
                crg.dblCostEst /= dblAdjust;
                crg.dblCostExact /= dblAdjust;
                crg.dblCostExactType /= dblAdjust;
                crg.dblCostEstType /= dblAdjust;
                crg.d /= (dblAdjust);

                crg = crg.parent;
            } while (crg != null);
        }

        public void Output(double dblProportion)
        {
            var pParameterInitialize = _ParameterInitialize;
            var pInitialCrgLt = this.InitialCrgLt;
            int intTotalTimeNum = 1;
            for (int i = 0; i < InitialCrgLt.Count; i++)
            {
                intTotalTimeNum += InitialCrgLt[i].GetCphCount() - 1;
            }
            int intOutputStepNum = Convert.ToInt32(Math.Floor((intTotalTimeNum - 1) * dblProportion));

            var OutputCrgLt = new List<CRegion>(this.InitialCrgLt.Count);
            var CrgSS = new SortedSet<CRegion>();

            if (pParameterInitialize.strAreaAggregation == "Smallest")
            {
                CrgSS = new SortedSet<CRegion>(this.InitialCrgLt, CRegion.pCompareCRegion_MinArea_CphGIDTypeIndex);
            }
            else
            {
                CrgSS = new SortedSet<CRegion>(this.InitialCrgLt, CRegion.pCompareCRegion_CostExact_CphGIDTypeIndex);  //*******************we may need to change comparator here to smallest area**********//
            }


            for (int i = 1; i <= intOutputStepNum; i++)
            {
                var currentMinCrg = CrgSS.Min;
                CrgSS.Remove(currentMinCrg);
                var newCrg = currentMinCrg.child;

                if (newCrg == null)  //if there is no child anymore, then we must output this Crg
                {
                    OutputCrgLt.Add(currentMinCrg);
                    i--;
                }
                else
                {
                    CrgSS.Add(newCrg);
                }
            }

            OutputCrgLt.AddRange(CrgSS);

            OutputMap(OutputCrgLt, this._TypePVSD, dblProportion, intOutputStepNum + 1, pParameterInitialize);
        }


        public static void OutputMap(IEnumerable<CRegion> OutputCrgLt, CValMap_SD<int, int> pTypePVSD, double dblProportion,
            int intTime, CParameterInitialize pParameterInitialize)
        {
            int intAttributeNum = 2;
            var pstrFieldNameLt = new List<string>(intAttributeNum);
            pstrFieldNameLt.Add("OBJART");
            pstrFieldNameLt.Add("RegionNum");

            var pesriFieldTypeLt = new List<esriFieldType>(intAttributeNum);
            pesriFieldTypeLt.Add(esriFieldType.esriFieldTypeInteger);
            pesriFieldTypeLt.Add(esriFieldType.esriFieldTypeInteger);

            var pobjectValueLtLt = new List<List<object>>();
            var CpgLt = new List<CPolygon>();
            var IpgLt = new List<IPolygon4>();
            foreach (var crg in OutputCrgLt)
            {
                foreach (var CphTypeIndexKVP in crg.CphTypeIndexSD_Area_CphGID)
                {
                    IpgLt.Add(CphTypeIndexKVP.Key.MergeCpgSS());
                    var pobjectValueLt = new List<object>(intAttributeNum);
                    int intType;
                    pTypePVSD.SD_R.TryGetValue(CphTypeIndexKVP.Value, out intType);
                    pobjectValueLt.Add(intType);
                    pobjectValueLt.Add(crg.ID);
                    pobjectValueLtLt.Add(pobjectValueLt);
                }
            }

            CSaveFeature.SaveIGeoEb(IpgLt, esriGeometryType.esriGeometryPolygon, dblProportion.ToString() + "_#" + IpgLt.Count + "_Step" + intTime.ToString() + "_" + CHelperFunction.GetTimeStamp(),
                pParameterInitialize, pstrFieldNameLt, pesriFieldTypeLt, pobjectValueLtLt, strSymbolLayerPath: pParameterInitialize.strPath + "complete.lyr");
        }



        public static void SaveData(CStrObjLtSD StrObjLtSD, CParameterInitialize pParameterInitialize, string strMethod, int intQuitCount)
        {
            int intAtrNum = StrObjLtSD.Count;
            int intCrgNum = StrObjLtSD.Values.GetFirstT().Count;

            var pobjDataLtLt = new List<IList<object>>(intCrgNum);
            var TempobjDataLtLt = new List<IList<object>>(intAtrNum);

            //order the the lists according to the order of the keys
            foreach (var strKey in strKeyLt)
            {
                List<object> valuelt;
                StrObjLtSD.TryGetValue(strKey, out valuelt);
                TempobjDataLtLt.Add(valuelt);
            }

            //
            for (int j = 0; j < intCrgNum; j++)
            {
                var pobjDataLt = new List<object>(intAtrNum);
                for (int i = 0; i < intAtrNum; i++)
                {
                    pobjDataLt.Add(TempobjDataLtLt[i][j]);
                }
                pobjDataLtLt.Add(pobjDataLt);
            }

            SortedSet<IList<object>> objDataLtSS = new SortedSet<IList<object>>(pobjDataLtLt, new CAACCompare());

            CHelperFunctionExcel.ExportToExcel(objDataLtSS,
                CHelperFunction.GetTimeStamp() + "_" + strMethod + "_" + pParameterInitialize.strAreaAggregation + "_" +
                CConstants.strShapeConstraint + "_" + intQuitCount, pParameterInitialize.strSavePath, CCAMDijkstra.strKeyLt);
            ExportForLatex(objDataLtSS, CCAMDijkstra.strKeyLt, pParameterInitialize.strSavePath);
            ExportIDOverEstimation(objDataLtSS, pParameterInitialize.strSavePath);
            ExportStatistic(StrObjLtSD, pParameterInitialize.strSavePath);
        }

        public static void ExportStatistic(CStrObjLtSD StrObjLtSD, string strSavePath)
        {
            string strData = "";

            List<object> objFactorLt;
            StrObjLtSD.TryGetValue("Factor", out objFactorLt);
            double dblLogFactorSum = 0;
            int intOverEstCount = 0;
            var intFactorCountlt = new List<int>(15);
            intFactorCountlt.EveryElementNew();
            for (int i = 0; i < objFactorLt.Count; i++)
            {
                double dblFactor = Convert.ToDouble(objFactorLt[i]);
                double dblLogFactor = Math.Log(dblFactor, 2);
                dblLogFactorSum += dblLogFactor;
                intFactorCountlt[Convert.ToInt16(dblLogFactor)]++;
                if (dblFactor > 1)
                {
                    intOverEstCount++;
                }
            }

            strData += ("& " + string.Format("{0,3}", intOverEstCount));
            strData += (" & " + string.Format("{0,3}", dblLogFactorSum));
            strData += GetSumWithSpecifiedStyle(StrObjLtSD, "#Edges", "{0,10}", 0);
            strData += GetSumWithSpecifiedStyle(StrObjLtSD, "#Nodes", "{0,8}", 0);
            strData += GetSumWithSpecifiedStyle(StrObjLtSD, "CostType", "{0,4}", 1);
            strData += GetSumWithSpecifiedStyle(StrObjLtSD, "CostComp", "{0,4}", 1);
            strData += GetSumWithSpecifiedStyle(StrObjLtSD, "WeightedSum", "{0,4}", 1);
            strData += GetSumWithSpecifiedStyle(StrObjLtSD, "Time(ms)", "{0,4}", 1, 3600000);
            //strData += ;

            //to generate coordinates like (1,6), where x is for the index of overestimation factor, 
            //and y is for the number of domains that used the factor 
            for (int i = 0; i < intFactorCountlt.Count; i++)
            {
                strData += "\n(" + i + "," + intFactorCountlt[i] + ")";
            }

            using (var writer = new StreamWriter(strSavePath + "\\" + CHelperFunction.GetTimeStamp() + "_" + "StatisticsForLatex" + ".txt", true))
            {
                writer.Write(strData);
            }
        }

        private static string GetSumWithSpecifiedStyle(CStrObjLtSD StrObjLtSD, string strKey, string strformat, int intRound, double dblTime = 1)
        {
            List<object> objLt;
            StrObjLtSD.TryGetValue(strKey, out objLt);
            double dblSum = 0;
            for (int i = 0; i < objLt.Count; i++)
            {
                dblSum += Convert.ToDouble(objLt[i]);
            }
            dblSum /= dblTime;

            string strData = Uniquedigits(dblSum, intRound);
            return " & " + string.Format(strformat, strData);
        }

        private static string Uniquedigits(double dblValue, int intRound)
        {
            string strData = "";
            if (intRound > 0)
            {
                string strformatdigits = "0.";
                while (intRound > 0)
                {
                    strformatdigits += "0";
                    intRound--;
                }
                strData = dblValue.ToString(strformatdigits);
            }
            else
            {
                strData = dblValue.ToString();
            }
            return strData;
        }

        public static void ExportForLatex(IEnumerable<IList<object>> objDataLtEb, IEnumerable<object> objHeadEb, string strSavePath)
        {
            //fetch some values that we want to exprot for latex
            IList<string> strFieldLt = new List<string>
            {
                "ID",
                "n",
                "m",
                "Factor",
                "RatioTypeCE",
                "RatioCompCE",
                //"RatioCompType",
                //"WeightedSum",
                "Time(ms)"  //we will output time with unit second
            };

            List<int> intIndexLt = new List<int>(strFieldLt.Count);
            foreach (var strField in strFieldLt)
            {
                int intCount = 0;
                foreach (var objHead in objHeadEb)
                {
                    if (strField == objHead.ToString())
                    {
                        intIndexLt.Add(intCount);
                    }
                    intCount++;
                }
            }

            //{index[,length][:formatString]}; 
            //length: If positive, the parameter is right-aligned; if negative, it is left-aligned.
            //const string format = "{0,6}";
            string strData = "";

            foreach (var objDataLt in objDataLtEb)
            {
                strData += string.Format("{0,3}", objDataLt[intIndexLt[0]]);  //for ID
                for (int i = 1; i < intIndexLt.Count - 1; i++)
                {
                    int intIndex = intIndexLt[i];

                    if (i == 1 || i == 2) // for n and m
                    {
                        strData += (" & " + string.Format("{0,2}", objDataLt[intIndex].ToString()));
                    }
                    else if (i == 3)  //for overestimation facotr
                    {
                        strData += (" & " + string.Format("{0,3}", objDataLt[intIndex].ToString()));
                    }
                    else if (i == 4)
                    {
                        strData += (" & " + string.Format("{0,5}", Convert.ToDouble(objDataLt[intIndex]).ToString("0.000")));
                    }
                    else if (i == 5)
                    {
                        strData += (" & " + string.Format("{0,7}", Convert.ToDouble(objDataLt[intIndex]).ToString("0.000")));
                    }
                    else  //for time
                    {
                        //strData += (" & " + string.Format(format, Math.Round(Convert.ToDouble(objDataLt[intIndex]), _intDigits).ToString("0.000")));
                        strData += (" & " + string.Format("{0,5}", Convert.ToDouble(objDataLt[intIndex]).ToString("0.000")));
                    }
                }
                strData += (" & " + string.Format("{0,5}", (Convert.ToDouble(objDataLt[intIndexLt.GetLast_T()]) / 1000).ToString("0.0")));
                strData += ("\\" + "\\" + "\n");
            }

            using (var writer = new StreamWriter(strSavePath + "\\" + CHelperFunction.GetTimeStamp() + "_" + "DetailsForLatex" + ".txt", true))
            {
                writer.Write(strData);
            }
        }

        private static void ExportIDOverEstimation(IEnumerable<IList<object>> objDataLtEb, string strSavePath)
        {
            string strData = "";
            foreach (var objDataLt in objDataLtEb)
            {
                if (Convert.ToInt32(objDataLt[3]) > 1)
                {
                    strData += "intSpecifiedIDLt.Add(" + objDataLt[0] + ");\n";
                }
                else
                {
                    break;
                }
            }
            using (var writer = new StreamWriter(strSavePath + "\\" + CHelperFunction.GetTimeStamp() + "_" + "FormalizedOverEstimationID" + ".txt", true))
            {
                writer.Write(strData);
            }
        }
        #endregion
    }
}
