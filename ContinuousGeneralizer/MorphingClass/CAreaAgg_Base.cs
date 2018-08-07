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
    public class CAreaAgg_Base : CMorphingBaseCpg
    {
        //*******************************************************
        //set strMethod at the beginning so that we can save files during ILP using the same prefix


        public static double dblLamda = 0.5; // 1-dblLamda is for type; dblLamda is for shape


        //1170 is from _All_MinimizeInteriorBoundaries_200000000
        //160s is from _Smallest_MinimizeInteriorBoundaries_200000
        //600s: 10min
        //public double dblTimeLimit { get; set; } = 113; //in seconds
        public double dblTimeLimit { get; set; } = 112; //in seconds, from 111.5 s
                                                        //public double dblTimeLimit { get; set; } = 300; //in seconds
                                                        //public static double dblLamda2 = 1 - dblLamda1;

        public static string _strResultFileName = "";
        //for some prompt settings
        protected int _intRound = 0;
        //protected int _intRound = 7;

        protected static int _intStart; //=0
        protected static int _intEndCount; //=this.SSCrgLt.Count
        protected List< List<long>> _lngMemoryLtLt = new List<List<long>>();
        protected static bool _blnTesting = true; //if we are testing, we forget about aggregation sequences
        protected static Dictionary<int, CValPair<int, double>> _EstStepsCostVPDt;
        protected static List<string> _strLineLt; //sort according to ID

        //comment the following if you want to process on all instances
        protected void UpdateStartEnd()
        {
            //_intStart = 90;
            //_intEndCount = _intStart + 10;

            if (CConstants.strRunContinuousGeneralizer!="")
            {
                var lastLine = File.ReadLines(
               @"C:\MyWork\DailyWork\ContinuousGeneralisation\RunContinuousGeneralizer\CallRecord.txt").Last();
                _intStart = Convert.ToInt32(lastLine);
                _intEndCount = _intStart + 1;
            }
        }

        //if we are testing memory consumption (_blnTestMemoryConsumption = true), 
        //then our method will forget the computed sequence for each region
        //protected static bool _blnTestMemoryConsumption = true;


        public List<CRegion> InitialCrgLt { set; get; }
        public List<CRegion> LSCrgLt { set; get; }
        public List<CRegion> SSCrgLt { set; get; }

        protected double[,] _adblTD;
        protected CValMap_Dt<int, int> _TypePVDt;
        protected Dictionary<int, ISymbol> _intTypeSymbolDt;


        public double dblCost { set; get; }
        public CStrObjLtDt StrObjLtDt { set; get; }

        //if we change the list, we may need to change the comparer named CAACCmp
        public static IList<string> strKeyLt = new List<string>
        {
            "ID",
            "n",
            "m",
            "EstSteps",
            "#Nodes",
            "#Edges",            
            "EstType",
            "CostType",
            "R_TypeCE",
            "EstComp",
            "CostComp",
            "R_CompCE",
            "R_TypeComp",
            "Cost",
            "Time_F(ms)",  //time for the first attempt
            "Time_L(ms)",  //time for the last attempt
            "Time(ms)",    //total time
            "Memory(MB)"
        };

        protected static int _intDigits = 6;



        protected void Preprocessing(CParameterInitialize ParameterInitialize, 
            string strSpecifiedFieldName = null, string strSpecifiedValue = null)
        {
            Construct<CPolygon, CPolygon>(ParameterInitialize, 2, 0, true, 1, strSpecifiedFieldName, strSpecifiedValue);
            CConstants.strShapeConstraint = ParameterInitialize.cboShapeConstraint.Text;
            if (CConstants.strShapeConstraint == "MaximizeMinComp_EdgeNumber" || 
                CConstants.strShapeConstraint == "MaximizeMinComp_Combine")
            {
                CConstants.blnComputeMinComp = true;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp_EdgeNumber" || 
                CConstants.strShapeConstraint == "MaximizeAvgComp_Combine")
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
            var aObj = CHelpFuncExcel.ReadDataFromExcel(ParameterInitialize.strMxdPathBackSlash + "TypeDistance.xlsx");
            if (aObj == null) throw new ArgumentNullException("Failed to read TypeDistance.xlsx");

            int intDataRow = aObj.GetUpperBound(0);
            int intDataCol = intDataRow;  //note that intDataRow == intDataCol

            //set an index for each type, so that we can access a type distance directly
            //var intTypeIndexSD = new SortedDictionary<int, int>();
            var pTypePVDt = new CValMap_Dt<int, int>();
            int intTypeIndex = 0;
            for (int i = 0; i < intDataRow; i++)
            {
                int intType = Convert.ToInt32(aObj[i + 1][0]);
                if (pTypePVDt.Dt.ContainsKey(intType) == false)
                {
                    pTypePVDt.Dt.Add(intType, intTypeIndex++);
                }
            }
            pTypePVDt.CreateSD_R();
            _TypePVDt = pTypePVDt;

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
            var pLSCPgLt = this.ObjCGeoLtLt[0].AsExpectedClass<CPolygon, object>().ToList();
            var pSSCPgLt = this.ObjCGeoLtLt[1].AsExpectedClass<CPolygon, object>().ToList();

            //this.intTotalTimeNum = pLSCPgLt.Count - pSSCPgLt.Count + 1;

            foreach (var cpg in pLSCPgLt)
            {
                cpg.FormCEdgeLt();
                cpg.SetCEdgeLtLength();
            }
            

            foreach (var cpg in pSSCPgLt)
            {
                cpg.FormCEdgeLt();
            }

            //get region number for each polygon
            var pstrFieldNameLtLt = this.strFieldNameLtLt;
            var pObjValueLtLtLt = this.ObjValueLtLtLt;
            //var intTypeIndexSD=_intTypeIndexSD;

            //RegionNumATIndex: the index of RegionNum in the attribute table 
            var intLSTypeATIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[0], "OBJART");
            var intSSTypeATIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[1], "OBJART");
            //var CgbEb=pLSCPgLk.AsExpectedClass<CGeometricBase<CPolygon>, CGeometricBase<CPolygon>>();
            CHelpFunc.GetCgbTypeAndTypeIndex(pLSCPgLt.AsExpectedClass<CPolygon, CPolygon>(), _ObjValueLtLtLt[0], 0, _TypePVDt);
            CHelpFunc.GetCgbTypeAndTypeIndex(pSSCPgLt.AsExpectedClass<CPolygon, CPolygon>(), _ObjValueLtLtLt[1], 0, _TypePVDt);

            //RegionNumATIndex: the index of RegionNum in the attribute table 
            var intLSRegionNumATIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[0], "RegionNum");
            var intSSRegionNumATIndex = CSaveFeature.FindFieldNameIndex(pstrFieldNameLtLt[1], "RegionNum");
            //private CValMap_Dt<int, int> _RegionPVDt;
            var pRegionPVDt = new CValMap_Dt<int, int>();
            int intRegionIndex = 0;
            for (int i = 0; i < pObjValueLtLtLt[1].Count; i++)
            {
                int intRegionNum = Convert.ToInt32(pObjValueLtLtLt[1][i][intSSRegionNumATIndex]);
                if (pRegionPVDt.Dt.ContainsKey(intRegionNum) == false)
                {
                    pRegionPVDt.Dt.Add(intRegionNum, intRegionIndex++);
                }
            }

            //assign the polygons as well as attributes from a featureLayer into regions, without considering costs
            this.LSCrgLt = GenerateCrgLt(pLSCPgLt, pSSCPgLt.Count, 
                pObjValueLtLtLt[0], intLSTypeATIndex, intLSRegionNumATIndex, _TypePVDt, pRegionPVDt);
            this.SSCrgLt = GenerateCrgLt(pSSCPgLt, pSSCPgLt.Count, 
                pObjValueLtLtLt[1], intSSTypeATIndex, intSSRegionNumATIndex, _TypePVDt, pRegionPVDt);

            _intTypeSymbolDt = CToIpe.GetKeySymbolDt(_ParameterInitialize.pFLayerLt[0], pObjValueLtLtLt[0], intLSTypeATIndex);


            //using (var writer = new System.IO.StreamWriter(_ParameterInitialize.strSavePathBackSlash +
            //    CHelpFunc.GetTimeStamp() + "_" + "AreaAggregation.txt", false))
            //{
            //    writer.Write(_ParameterInitialize.strAreaAggregation);
            //}

            //apply A* algorithm to each region
            this.InitialCrgLt = new List<CRegion>(pSSCPgLt.Count);
            //var ResultCrgLt = new List<CRegion>(pSSCPgLt.Count);
            this.StrObjLtDt = new CStrObjLtDt(CAreaAgg_AStar.strKeyLt, pSSCPgLt.Count);

            _intStart = 0;
            _intEndCount = this.SSCrgLt.Count;




            UpdateStartEnd();
        }


        protected void EndAffairs(int intNewStart)
        {
            if (CConstants.strRunContinuousGeneralizer != "")
            {
                using (var writer = new StreamWriter(
                    @"C:\MyWork\DailyWork\ContinuousGeneralisation\RunContinuousGeneralizer\DirectoryPath.txt", false))
                {
                    //we save this path so that we can delte the folder in RunContinuousGeneralizer
                    writer.WriteLine(CConstants.ParameterInitialize.strSavePath);
                }

                var proc = Process.GetCurrentProcess();
                var dblMemoryMB = Convert.ToDouble(proc.PeakWorkingSet64) / 1048576; //bytes to MB
                var strMemoryMB = string.Format("{0,15}", dblMemoryMB.ToString("0.000"));

                using (var writer = new StreamWriter(
                    @"C:\MyWork\DailyWork\ContinuousGeneralisation\RunContinuousGeneralizer\CallRecord.txt", true))
                {
                    writer.WriteLine(strMemoryMB);
                }
                proc.Kill();

            }
        }

        #region Common

        protected void AddLineToStrObjLtDt(CStrObjLtDt StrObjLtDt, CRegion LSCrg)
        {
            var et = StrObjLtDt.GetEnumerator();
            while (et.MoveNext())
            {
                et.Current.Value.Add(-1);
            }

            StrObjLtDt.SetLastObj("ID", LSCrg.ID);
            StrObjLtDt.SetLastObj("n", LSCrg.CphTypeIndexSD_Area_CphGID.Count);
            StrObjLtDt.SetLastObj("m", LSCrg.AdjCorrCphsSD.Count);
            StrObjLtDt.SetLastObj("EstSteps", -1);  //default value
        }


        /// <summary>
        /// assign the polygons as well as attributes from a featureLayer into regions, without considering costs
        /// </summary>
        /// <param name="pCpgLt"></param>
        /// <param name="intCrgNum"></param>
        /// <param name="pObjValueLtLt"></param>
        /// <param name="intTypeATIndex"></param>
        /// <param name="intRegionNumATIndex"></param>
        /// <param name="pTypePVDt"></param>
        /// <param name="pRegionPVDt"></param>
        /// <returns></returns>
        protected List<CRegion> GenerateCrgLt(List<CPolygon> pCpgLt, int intCrgNum, 
            List<List<object>> pObjValueLtLt, int intTypeATIndex, int intRegionNumATIndex, 
            CValMap_Dt<int, int> pTypePVDt, CValMap_Dt<int, int> pRegionPVDt)
        {
            var pCrgLt = new List<CRegion>(intCrgNum);
            pCrgLt.EveryElementNew();

            for (int i = 0; i < pCpgLt.Count; i++)
            {
                //get the type index
                int intType = Convert.ToInt32(pObjValueLtLt[i][intTypeATIndex]);
                int intTypeIndex;
                pTypePVDt.Dt.TryGetValue(intType, out intTypeIndex);

                //get the RegionNum index
                var intRegionNum = Convert.ToInt32(pObjValueLtLt[i][intRegionNumATIndex]);
                int intRegionIndex;
                pRegionPVDt.Dt.TryGetValue(intRegionNum, out intRegionIndex);

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


        protected void RecordResultForCrg(CStrObjLtDt StrObjLtDt, CRegion LSCrg, CRegion FinalOneCphCrg, int intSSTypeIndex)
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

            double dblR_TypeCE = 1;
            double dblR_CompCE = 1;
            double dblR_TypeComp = 1;

            if (LSCrg.GetCphCount() > 1)
            {
                if (LSCrg.dblCostEstType > 0)  //if LSCrg.dblCostEstType == 0, then we define dblR_TypeCE = 1
                {
                    dblR_TypeCE = Math.Round(FinalOneCphCrg.dblCostExactType / LSCrg.dblCostEstType, _intDigits);
                }

                if (LSCrg.dblCostEstComp > 0)
                {
                    dblR_CompCE = Math.Round(FinalOneCphCrg.dblCostExactComp / LSCrg.dblCostEstComp, _intDigits);
                }

                if (FinalOneCphCrg.dblCostExactComp>0)
                {
                    dblR_TypeComp = Math.Round(FinalOneCphCrg.dblCostExactType / FinalOneCphCrg.dblCostExactComp, _intDigits);
                }                
            }

            if (CConstants.strMethod == "Greedy")
            {
                dblRoundedCostEstimatedType = -1;
                dblR_TypeCE = -1;
                dblRoundedCostEstComp = -1;
                dblR_CompCE = -1;
            }

            StrObjLtDt.SetLastObj("#Nodes", CRegion._intNodeCount);
            StrObjLtDt.SetLastObj("EstType", dblRoundedCostEstimatedType);
            StrObjLtDt.SetLastObj("CostType", dblRoundedCostExactType);
            StrObjLtDt.SetLastObj("R_TypeCE", dblR_TypeCE);
            StrObjLtDt.SetLastObj("EstComp", dblRoundedCostEstComp);
            StrObjLtDt.SetLastObj("CostComp", dblRoundedCostExactComp);
            StrObjLtDt.SetLastObj("R_CompCE", dblR_CompCE);
            StrObjLtDt.SetLastObj("R_TypeComp", dblR_TypeComp);
            StrObjLtDt.SetLastObj("Cost", Math.Round(FinalOneCphCrg.dblCostExact, _intDigits));

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
            double dblAdjust = 1 / crg.dblArea;
            do
            {
                crg.dblCostEst *= dblAdjust;
                crg.dblCostExact *= dblAdjust;
                crg.dblCostExactType *= dblAdjust;
                crg.dblCostEstType *= dblAdjust;
                crg.d *= dblAdjust;

                crg = crg.parent;
            } while (crg != null);
        }

        public void Output(double dblProportion)
        {
            var pParameterInitialize = _ParameterInitialize;
            var pInitialCrgLt = this.InitialCrgLt;
            int intTotalCphCount = 0;
            for (int i = 0; i < InitialCrgLt.Count; i++)
            {
                intTotalCphCount += pInitialCrgLt[i].GetCphCount();
            }
            int intTotalTimeNum = intTotalCphCount - pInitialCrgLt.Count + 1;
            int intOutputStepNum = Convert.ToInt32(Math.Floor((intTotalTimeNum - 1) * dblProportion));

            var OutputCrgLt = new List<CRegion>(pInitialCrgLt.Count);
            var CrgSS = new SortedSet<CRegion>();

            if (pParameterInitialize.strAreaAggregation == "Smallest")
            {
                CrgSS = new SortedSet<CRegion>(pInitialCrgLt, CRegion.pCmpCrg_MinArea_CphGIDTypeIndex);
            }
            else
            {
                CrgSS = new SortedSet<CRegion>(pInitialCrgLt, CRegion.pCmpCrg_CostExact_CphGIDTypeIndex);
            }


            for (int i = 0; i < intOutputStepNum; i++)
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
            //SortedDictionary<int, ISymbol> intTypeFillSymbolSD
            OutputCrgLt.AddRange(CrgSS);

            OutputMap(OutputCrgLt, this._TypePVDt, dblProportion, intOutputStepNum + 1, pParameterInitialize);
        }


        public static void OutputMap(IEnumerable<CRegion> OutputCrgLt, CValMap_Dt<int, int> pTypePVDt, double dblProportion,
            int intTime, CParameterInitialize pParameterInitialize)
        {
            List<string> pstrFieldNameLt;
            List<esriFieldType> pesriFieldTypeLt;
            SetAttributes(out pstrFieldNameLt, out pesriFieldTypeLt);

            var pobjectValueLtLt = new List<List<object>>();
            var CpgLt = new List<CPolygon>();
            var IpgLt = new List<IPolygon4>();
            foreach (var crg in OutputCrgLt)
            {
                foreach (var CphTypeIndexKVP in crg.CphTypeIndexSD_Area_CphGID)
                {
                    IpgLt.Add(CphTypeIndexKVP.Key.JudgeAndMergeCpgSSToIpg());
                    var pobjectValueLt = new List<object>(2);
                    int intType;
                    pTypePVDt.Dt_R.TryGetValue(CphTypeIndexKVP.Value, out intType);
                    pobjectValueLt.Add(intType);
                    pobjectValueLt.Add(crg.ID);
                    pobjectValueLtLt.Add(pobjectValueLt);
                }
            }

            CSaveFeature.SaveIGeoEb(IpgLt, esriGeometryType.esriGeometryPolygon,
        dblProportion.ToString() + "_#" + IpgLt.Count + "_Step" + intTime.ToString(),
        pstrFieldNameLt, pesriFieldTypeLt, pobjectValueLtLt, 
        strSymbolLayerPath: pParameterInitialize.strMxdPathBackSlash + "complete.lyr");
        }

        private static void SetAttributes(out List<string> pstrFieldNameLt, out List<esriFieldType> pesriFieldTypeLt)
        {
            pstrFieldNameLt = new List<string>
            {
                "OBJART",
                "RegionNum"
            };

            pesriFieldTypeLt = new List<esriFieldType>
            {
                esriFieldType.esriFieldTypeInteger,
                esriFieldType.esriFieldTypeInteger
            };
        }



        public static void SaveData(CStrObjLtDt StrObjLtDt,
            CParameterInitialize pParameterInitialize, string strMethod, string strParameter = "")
        {
            int intAtrNum = StrObjLtDt.Count;
            int intCrgNum = StrObjLtDt.Values.First().Count;

            var pobjDataLtLt = new List<IList<object>>(intCrgNum);
            var TempobjDataLtLt = new List<IList<object>>(intAtrNum);

            //order the the lists according to the order of the keys
            foreach (var strKey in CAreaAgg_Base.strKeyLt)
            {
                List<object> valuelt;
                StrObjLtDt.TryGetValue(strKey, out valuelt);
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

            //sort according to overestimation factor, n, m, ID
            var objDataLtONMIdSS = new SortedSet<IList<object>>(pobjDataLtLt, new CAACCmp());

            var objDataIdLtSD = new Dictionary<int, IList<object>>();
            foreach (var pobjDataLt in pobjDataLtLt)
            {
                objDataIdLtSD.Add(Convert.ToInt32(pobjDataLt[0]), pobjDataLt);
            }

            //save results under the same filename
            _strResultFileName = CHelpFunc.GetTimeStamp() + "_" + strMethod + strParameter + "_" +
                pParameterInitialize.strAreaAggregation + "_" + CConstants.strShapeConstraint;

            //excel
            CHelpFuncExcel.ExportToExcel(objDataLtONMIdSS,
                _strResultFileName, pParameterInitialize.strSavePath, CAreaAgg_AStar.strKeyLt);
            //txt
            ExportStatistic(StrObjLtDt, _strResultFileName, pParameterInitialize.strSavePath);
            ExportDetailsForLatex(objDataLtONMIdSS, CAreaAgg_Base.strKeyLt, _strResultFileName, pParameterInitialize.strSavePath);
            ExportIDOverEstimation(objDataLtONMIdSS, _strResultFileName, pParameterInitialize.strSavePath, strMethod);

            if (strMethod == "Greedy" && pobjDataLtLt.Count == 734)
            {
                ExportCmpGreedyAStar(_strLineLt, objDataLtONMIdSS, _strResultFileName, pParameterInitialize.strSavePath);
            }

            if (strMethod == "AStar" && pobjDataLtLt.Count == 734)
            {
                ExportAStarCost(objDataLtONMIdSS, strMethod + strParameter + "_" +
                    pParameterInitialize.strAreaAggregation + "_" + CConstants.strShapeConstraint,
                    pParameterInitialize.strMxdPath);
            }


        }


        public static void ExportCmpGreedyAStar(List<string> strLineLt,
            SortedSet<IList<object>> objDataLtONMIdSS, string strName, string strSavePath)
        {
            var IDCostDt = new Dictionary<int, double>();
            foreach (var objDataLt in objDataLtONMIdSS)
            {
                IDCostDt.Add(Convert.ToInt32(objDataLt[0]), Convert.ToDouble(objDataLt[13]));
            }

            //string strData = "\n\nComparison between Greedy and AStar200000:\n";
            string strData = string.Format("{0,5}{1,10}{2,16}{3,16}{4,16}{5,16}",
                "ID", "EstSteps", "AStarCost", "GreedyCost", "Greedy-AStar", "Diff/AStar") + "\n";
            for (int i = 1; i < strLineLt.Count; i++) //the first line is for headings
            {
                var strDetail = strLineLt[i].Split(new char[] { ' ', '\t' }, 
                    StringSplitOptions.RemoveEmptyEntries); //use white space to split
                var intID = Convert.ToInt32(strDetail[0]);
                var dblAStarCost = Convert.ToDouble(strDetail[2]);
                double dblGreedyCost;
                IDCostDt.TryGetValue(intID, out dblGreedyCost);
                var dblDiff = dblGreedyCost - dblAStarCost;
                double dblRatio = 10000;
                if (dblGreedyCost==0)
                {
                    //in this case, dblAStarCost == 0
                    dblRatio = 1; 
                }
                else if (dblAStarCost > 0)
                {
                    dblRatio = dblDiff / dblAStarCost;
                }


                strData += string.Format("{0,5}{1,10}{2,16}{3,16}{4,16}{5,16}", strDetail[0], strDetail[1], strDetail[2],
                dblGreedyCost.ToString("F4"), dblDiff.ToString("F4"), dblRatio.ToString("F4")) + "\n";
            }

            using (var writer = new StreamWriter(strSavePath + "\\" + strName + ".txt", true))
            {
                writer.Write(strData);
            }
        }

        public static void ExportAStarCost(SortedSet<IList<object>> objDataLtONMIdSS, string strName, string strSavePath)
        {
            var objDataLtLt = objDataLtONMIdSS.ToList();
            string strData = string.Format("{0,5}{1,10}{2,16}", "ID", "EstSteps", "Cost") + "\n";
            for (int i = 0; i < objDataLtLt.Count; i++)
            {
                var pobjdatalt = objDataLtLt[i];
                //EstSteps: 3; Cost: 13
                strData += string.Format("{0,5}{1,10}{2,16}", pobjdatalt[0], pobjdatalt[3], 
                    Convert.ToDouble(pobjdatalt[13]).ToString("F8")) + "\n";
            }

            using (var writer = new StreamWriter(strSavePath + "\\" + strName + ".txt", false))
            {
                writer.Write(strData);
            }
        }

        public static void ExportStatistic(CStrObjLtDt StrObjLtDt, string strName, string strSavePath)
        { 
            List<object> objEstStepsLt;
            StrObjLtDt.TryGetValue("EstSteps", out objEstStepsLt);
            List<object> objNLt;
            StrObjLtDt.TryGetValue("n", out objNLt);
            double dblLogEstStepsSum = 0;
            int intOverEstCount = 0;
            var dblRngCountLtLt = new List <List< double>>
            {
                //total number, optimal number, feasible number, no solution
                new List<double> { 0, 0, 0, 0 }, //n: 1-5
                new List<double> { 0, 0, 0, 0 }, //n: 6-10
                new List<double> { 0, 0, 0, 0 }, //n: 11-15
                new List<double> { 0, 0, 0, 0 }, //n: 16-20
                new List<double> { 0, 0, 0, 0 }, //n: 21-25
                new List<double> { 0, 0, 0, 0 }, //n: 26-36
            };
            
            var intEstStepsCountlt = new List<int>(21); //make a sufficient capacity 
            intEstStepsCountlt.EveryElementNew();
            for (int i = 0; i < objEstStepsLt.Count; i++)
            {
                double dblEstSteps = Convert.ToDouble(objEstStepsLt[i]);
                double dblLogEstSteps = 0;
                if (dblEstSteps > 0)
                {
                    dblLogEstSteps = Math.Log(dblEstSteps + 1, 2);
                    dblLogEstStepsSum += dblLogEstSteps;
                }
                intEstStepsCountlt[Convert.ToInt16(dblLogEstSteps)]++;
                if (dblEstSteps > 1)
                {
                    intOverEstCount++;
                }

                int intI = Convert.ToInt32(objNLt[i])/ 5;
                if (intI > 5)
                {
                    intI = 5;
                }
                dblRngCountLtLt[intI][0]++;
                if (dblEstSteps == 0)
                {
                    dblRngCountLtLt[intI][1]++;  //optimal solution
                }
                else if (dblEstSteps>0 && dblEstSteps<20000)
                {
                    dblRngCountLtLt[intI][2]++;  //feasible solution
                }
                else if (dblEstSteps == 20000)
                {
                    dblRngCountLtLt[intI][3]++;  //no solution
                }
                else
                {
                    throw new ArgumentOutOfRangeException("enexpected dblEstSteps");
                }
            }

            string strData = "& #over  & #repeatition  & #Nodes  & #Edges  & CostType  & CostComp  & Cost  & Time(min)\n";
            strData += ("& " + string.Format("{0,3}", intOverEstCount)); //symbol "&" is for the use in Latex
            strData += (" & " + string.Format("{0,3}", dblLogEstStepsSum));  //repetitions
            strData += GetSumWithSpecifiedStyle(StrObjLtDt, "#Nodes", "{0,8}", 0);
            strData += GetSumWithSpecifiedStyle(StrObjLtDt, "#Edges", "{0,10}", 0);            
            strData += GetSumWithSpecifiedStyle(StrObjLtDt, "CostType", "{0,4}", 1);
            strData += GetSumWithSpecifiedStyle(StrObjLtDt, "CostComp", "{0,4}", 1);
            strData += GetSumWithSpecifiedStyle(StrObjLtDt, "Cost", "{0,4}", 1);
            strData += GetSumWithSpecifiedStyle(StrObjLtDt, "Time(ms)", "{0,4}", 1, 60000) + "\n";  //all the time, minutes in statistics
            //strData += ;

            //to generate coordinates like (1,6), where x is for the index of overestimation factor, 
            //and y is for the number of domains that used the factor 
            for (int i = 0; i < intEstStepsCountlt.Count; i++)
            {
                strData += "(" + i + "," + intEstStepsCountlt[i] + ")\n";
            }

            var strFormat = "{0,9}{1,9}{2,9}{3,9}{4,9}{5,9}{6,9}{7,9}";
            strData += string.Format(strFormat, "range", "total", "optimal", "percent", "feasible", "percent", "no", "percent")+ "\n";


            for (int i = 0; i < dblRngCountLtLt.Count; i++)
            {
                var dblRngCountLt = dblRngCountLtLt[i];
                string strRange = "";
                switch (i)
                {
                    case 0:
                        strRange = "1-5";
                        break;
                    case 1:
                        strRange = "6-10";
                        break;
                    case 2:
                        strRange = "11-15";
                        break;
                    case 3:
                        strRange = "16-20";
                        break;
                    case 4:
                        strRange = "21-25";
                        break;
                    case 5:
                        strRange = "26-36";
                        break;
                    default:
                        break;
                }
                double dblSum = dblRngCountLt[0];
                strData += string.Format(strFormat, strRange, dblSum,
                    dblRngCountLt[1], (100 * dblRngCountLt[1] / dblSum).ToString("0.0"),
                    dblRngCountLt[2], (100 * dblRngCountLt[2] / dblSum).ToString("0.0"),
                    dblRngCountLt[3], (100 * dblRngCountLt[3] / dblSum).ToString("0.0")) + "\n";
            }

            using (var writer = new StreamWriter(strSavePath + "\\" + strName + ".txt", true))
            {
                writer.Write(strData);
            }
        }

        private static string GetSumWithSpecifiedStyle(CStrObjLtDt StrObjLtDt, 
            string strKey, string strformat, int intRound, double dblTimeUnit = 1)
        {
            List<object> objLt;
            StrObjLtDt.TryGetValue(strKey, out objLt);
            double dblSum = 0;
            for (int i = 0; i < objLt.Count; i++)
            {
                dblSum += Convert.ToDouble(objLt[i]);
            }
            dblSum /= dblTimeUnit;

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

        public static void ExportDetailsForLatex(IEnumerable<IList<object>> objDataLtEb, 
            IEnumerable<object> objHeadEb, string strName, string strSavePath)
        {
            //fetch some values that we want to exprot for latex
            IList<string> strFieldLt = new List<string>
            {
                "ID",
                "n",
                "m",
                "EstSteps",
                "CostType",
                "CostComp",
                "R_TypeCE",
                "R_CompCE",
                //"R_CompType",
                //"Cost",
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
            //const string format = "{i,j}"; i: the i-th argument, j:j positions
            string strData = "\n\n\n";  //make some gap lines

            foreach (var objDataLt in objDataLtEb)
            {
                strData += string.Format("{0,3}", objDataLt[intIndexLt[0]]);  //for ID
                for (int i = 1; i < intIndexLt.Count - 1; i++)
                {
                    int intIndex = intIndexLt[i];

                    if (i == 1 || i == 2 || i == 3) // for n and m, EstSteps
                    {
                        strData += (" & " + string.Format("{0,3}", objDataLt[intIndex].ToString()));
                    }
                    else if (i == 4 || i == 5 || i == 6 || i == 7)
                    {
                        strData += (" & " + string.Format("{0,7}", Convert.ToDouble(objDataLt[intIndex]).ToString("0.000")));
                    }
                }
                // for time
                strData += (" & " + string.Format("{0,5}", 
                    (Convert.ToDouble(objDataLt[intIndexLt.GetLastT()]) / 1000).ToString("0.0")));
                strData += ("\\" + "\\" + "\n"); // add \\ at the end of a line, which means line break in latex
            }

            using (var writer = new StreamWriter(strSavePath + "\\" + strName + ".txt", true))
            {
                writer.Write(strData);
            }
        }

        private static void ExportIDOverEstimation(IEnumerable<IList<object>> objDataLtEb, string strName, string strSavePath, string strMethod)
        {
            string strDataNo = "\n\n\n" + "find NO solution:\n";
            string strDataNoForm = "";
            string strDataFeasible = "\n\n\n" + "find feasible solution:\n";  //make some gap lines
            string strDataFeasibleForm = "";
            //string strData = "";            
            foreach (var objDataLt in objDataLtEb)
            {
                double dblEstSteps = Convert.ToDouble(objDataLt[3]); //for ILP, this is the gap between to an optimal solution
                if (dblEstSteps > 0)  //"EstSteps"
                {
                    if (dblEstSteps == 20000) //this is for ILP, where we find no solution
                    {
                        strDataNo += objDataLt[0] + "\n";
                        strDataNoForm += "intSpecifiedIDLt.Add(" + objDataLt[0] + ");\n";
                    }
                    else
                    {
                        strDataFeasible += objDataLt[0] + "\n";
                        strDataFeasibleForm += "intSpecifiedIDLt.Add(" + objDataLt[0] + ");\n";
                    }                    
                }
                else
                {
                    break;
                }
            }
            using (var writer = new StreamWriter(strSavePath + "\\" + strName + ".txt", true))
            {
                writer.Write(strDataNo);
                writer.Write(strDataNoForm);
                writer.Write(strDataFeasible);
                writer.Write(strDataFeasibleForm);
            }
        }

        public IEnumerable<IFeatureLayer> AggregateStepByStep()
        {
            var pParameterInitialize = _ParameterInitialize;
            var pFLayer = pParameterInitialize.pFLayerLt[0];
            var pFLayerEnv = pFLayer.AreaOfInterest;


            var pInitialCrgLt = this.InitialCrgLt;
            int intTotalCphCount = 0;
            for (int i = 0; i < pInitialCrgLt.Count; i++)
            {
                intTotalCphCount += pInitialCrgLt[i].GetCphCount();
            }
            int intOutputStepNum = intTotalCphCount - pInitialCrgLt.Count;

            //List<IFillSymbol> pFillSymbolLt;
            List<int> TypeIndexLt;
            List<IPolygon4> passiveIptLt;
            var newIpgLt = GenerateAggregatedIpgLt(pInitialCrgLt, intOutputStepNum, pParameterInitialize.strAreaAggregation,
                out passiveIptLt, out TypeIndexLt);

            for (int i = 0; i < newIpgLt.Count; i++)
            {
                List<string> pstrFieldNameLt;
                List<esriFieldType> pesriFieldTypeLt;
                SetAttributes(out pstrFieldNameLt, out pesriFieldTypeLt);
                var pobjectValueLtLt = new List<List<object>>();

                var pobjectValueLt = new List<object>(2);
                int intType;
                _TypePVDt.Dt_R.TryGetValue(TypeIndexLt[i], out intType);
                pobjectValueLt.Add(intType);
                pobjectValueLt.Add(-1);
                pobjectValueLtLt.Add(pobjectValueLt);

                var ipglt = new List<IPolygon4> { newIpgLt[i] };

                yield return CSaveFeature.SaveIGeoEb(ipglt, esriGeometryType.esriGeometryPolygon, 
        "#" + (intTotalCphCount - i - 1).ToString() + "_Step" + (i + 1).ToString(),
        pstrFieldNameLt, pesriFieldTypeLt, pobjectValueLtLt, 
        strSymbolLayerPath: pParameterInitialize.strMxdPathBackSlash + "complete.lyr");

            }
        }

        public void DisplayStepByStep(IEnumerable<IFeatureLayer> IFLayerEb)
        {
            foreach (var pFLayer in IFLayerEb)
            {
                //do nothing, so that we save layer in function "AggregateStepByStep"
            }
        }


        public void DetailToIpe()
        {
            var pParameterInitialize = _ParameterInitialize;
            var pFLayer = pParameterInitialize.pFLayerLt[0];
            var pFLayerEnv = pFLayer.AreaOfInterest;
            //string strBoundWidth = "0.05";
            string strBoundWidth = "normal";


            var pInitialCrgLt = this.InitialCrgLt;
            int intTotalCphCount = 0;
            for (int i = 0; i < pInitialCrgLt.Count; i++)
            {
                intTotalCphCount += pInitialCrgLt[i].GetCphCount();
            }
            int intOutputStepNum = intTotalCphCount - pInitialCrgLt.Count;

            var startCpgEb = GenerateStartCpgEb(pInitialCrgLt);

            List<int> intTypeIndexLt;
            List<IPolygon4> passiveIpgLt;
            var newIpgLt = GenerateAggregatedIpgLt(pInitialCrgLt, intOutputStepNum, pParameterInitialize.strAreaAggregation,
                out passiveIpgLt, out intTypeIndexLt);
            var pFillSymbolLt = GetFillSymbolLt(intTypeIndexLt, _TypePVDt, _intTypeSymbolDt);

            var strLayerNameLt = GetLayerNames(intTotalCphCount, pInitialCrgLt.Count);
            string strIpeCont = CIpeDraw.GetDataOfLayerNames(strLayerNameLt);
            strIpeCont += CIpeDraw.GetDataOfViewsAreaAgg(strLayerNameLt);
            strIpeCont += strDataOfBaseLayer(pInitialCrgLt, _TypePVDt, _intTypeSymbolDt,
                strLayerNameLt, pFLayerEnv, CConstants.pIpeEnv, strBoundWidth);
            strIpeCont += strDataOfCphs(startCpgEb, newIpgLt, passiveIpgLt, 
                pFillSymbolLt, strLayerNameLt, pFLayerEnv, CConstants.pIpeEnv, strBoundWidth);

            string strFullName = pParameterInitialize.strSavePath + "\\" + CHelpFunc.GetTimeStamp() + ".ipe";
            using (var writer = new System.IO.StreamWriter(strFullName, true))
            {
                writer.Write(CIpeDraw.GenerateIpeContentByDataWithLayerInfo(strIpeCont));
            }

            System.Diagnostics.Process.Start(@strFullName);
        }

        private static IEnumerable<CPolygon> GenerateStartCpgEb(List<CRegion> pInitialCrgLt)
        {
            foreach (var crg in pInitialCrgLt)
            {
                foreach (var cph in crg.GetCphCol())
                {
                    foreach (var cpg in cph.CpgSS)
                    {
                        yield return cpg;
                    }
                }
            }
        }

        private static List<IPolygon4> GenerateAggregatedIpgLt(List<CRegion> pInitialCrgLt, int intOutputStepNum,
            string strAreaAggregation, out List<IPolygon4> passiveIptLt, out List<int> TypeIndexLt)
        {
            var CrgSS = new SortedSet<CRegion>();
            if (strAreaAggregation == "Smallest")
            {
                CrgSS = new SortedSet<CRegion>(pInitialCrgLt, CRegion.pCmpCrg_MinArea_CphGIDTypeIndex);
            }
            else
            {
                CrgSS = new SortedSet<CRegion>(pInitialCrgLt, CRegion.pCmpCrg_CostExact_CphGIDTypeIndex);
            }

            var newIpgLt = new List<IPolygon4>(intOutputStepNum);
            passiveIptLt = new List<IPolygon4>(intOutputStepNum);
            TypeIndexLt = new List<int>(intOutputStepNum);
            for (int i = 0; i < intOutputStepNum; i++)
            {
                var currentMinCrg = CrgSS.Min;
                CrgSS.Remove(currentMinCrg);
                var newCrg = currentMinCrg.child;

                if (newCrg == null)  //if there is no child anymore, then we must output this Crg
                {
                    i--;
                }
                else
                {
                    //get the new polygon
                    newIpgLt.Add(newCrg.AggCphs.valResult.JudgeAndMergeCpgSSToIpg());                    
                    TypeIndexLt.Add(newCrg.GetCphTypeIndex(newCrg.AggCphs.valResult));
                    passiveIptLt.Add(newCrg.AggCphs.valPassive.JudgeAndMergeCpgSSToIpg());

                    CrgSS.Add(newCrg);
                }
            }
            return newIpgLt;

        }

        private static List<IFillSymbol> GetFillSymbolLt(List<int> intTypeIndexLt,
            CValMap_Dt<int, int> pTypePVDt, Dictionary<int, ISymbol> pintTypeSymbolDt)
        {
            var pFillSymbolLt = new List<IFillSymbol>(intTypeIndexLt.Count);
            foreach (var intTypeIndex in intTypeIndexLt)
            {
                //get fillsymbol of the polygon
                int intType;
                pTypePVDt.Dt_R.TryGetValue(intTypeIndex, out intType);
                ISymbol pSymbol;
                pintTypeSymbolDt.TryGetValue(intType, out pSymbol);
                pFillSymbolLt.Add(pSymbol as IFillSymbol);
            }

            return pFillSymbolLt;
        }


        private static string strDataOfBaseLayer(List<CRegion> pInitialCrgLt, 
            CValMap_Dt<int, int> pTypePVDt, Dictionary<int, ISymbol> pintTypeSymbolDt,
List<string> strLayerNameLt,
 IEnvelope pFLayerEnv, CEnvelope pIpeEnv, string strBoundWidth)
        {
            //for the first layer, we add all the patches
            string strIpeContAllLayers = CIpeDraw.SpecifyLayerByWritingText(strLayerNameLt[0], "removable", 320, 64);
            strIpeContAllLayers += CIpeDraw.writeIpeText(strLayerNameLt[0], 320, 128);  //write the number of patches
            strIpeContAllLayers += "<group>\n";
            foreach (var crg in pInitialCrgLt)
            {
                foreach (var kvp in crg.CphTypeIndexSD_Area_CphGID)
                {
                    int intType;
                    pTypePVDt.Dt_R.TryGetValue(kvp.Value, out intType);
                    ISymbol pSymbol;
                    pintTypeSymbolDt.TryGetValue(intType, out pSymbol);
                    var pfillSymbol=pSymbol as IFillSymbol;
                    strIpeContAllLayers += CToIpe.TranCpgToIpe(kvp.Key.GetSoloCpg(), 
                        pfillSymbol, pFLayerEnv, pIpeEnv, strBoundWidth);
                }
            }
            strIpeContAllLayers += "</group>\n";            
            

            return strIpeContAllLayers;
        }

        private static string strDataOfCphs(IEnumerable<CPolygon> startCpgEb, List<IPolygon4> IpgLt, 
            List<IPolygon4> passiveIpgLt, List<IFillSymbol> pFillSymbolLt, List<string> strLayerNameLt,
            IEnvelope pFLayerEnv, CEnvelope pIpeEnv, string strBoundWidth)
        {
            var passiveColor = new CColor(255, 153, 0);  //orange
            var resultColor = new CColor(255, 153, 0);
            string strActive = "heavier";
            string strPassive = "fat";
            
            //var resultColor = new CColor(45, 121, 147);

            ////for the first layer, we add all the patches
            //string strIpeContAllLayers = CIpeDraw.SpecifyLayerByWritingText(strLayerNameLt[0], "removable", 320, 64);
            //strIpeContAllLayers += CIpeDraw.writeIpeText(strLayerNameLt[0], 320, 128);  //write the number of patches
            //strIpeContAllLayers += "<group>\n";
            //foreach (var cpg in startCpgEb)
            //{
            //    strIpeContAllLayers+= CIpeDraw.DrawCpg(cpg, pFLayerEnv, pIpeEnv, resultColor, strBoundWidth);
            //}
            //strIpeContAllLayers += "</group>\n";            


            var strIpeContAllLayers = CIpeDraw.SpecifyLayerByWritingText(strLayerNameLt[1], "removable", 320, 64);
            strIpeContAllLayers += 
                "<group>\n" +
                CToIpe.TranIpgBoundToIpe(IpgLt[0], pFLayerEnv, pIpeEnv, resultColor, strActive) +
                CToIpe.TranIpgBoundToIpe(passiveIpgLt[0], pFLayerEnv, pIpeEnv, passiveColor, strPassive) +
                "</group>\n";



            //for each of other layers, we only add the new patch
            for (int i = 2; i < strLayerNameLt.Count-2; i++)
            {
                strIpeContAllLayers += CIpeDraw.SpecifyLayerByWritingText(strLayerNameLt[i], "removable", 320, 64);

                //draw a rectangle to cover the patch number of the last layer
                strIpeContAllLayers += CIpeDraw.drawIpeBox(304, 112, 384, 160, "white");

                //add a text of patch numbers
                strIpeContAllLayers += CIpeDraw.writeIpeText(strLayerNameLt[i], 320, 128);

                //add the data=================================================================
                strIpeContAllLayers += CToIpe.TranIpgToIpe(
                    IpgLt[i/2 - 1], pFillSymbolLt[i / 2 - 1], pFLayerEnv, pIpeEnv, strBoundWidth);
                

                strIpeContAllLayers += CIpeDraw.SpecifyLayerByWritingText(strLayerNameLt[i+1], "removable", 320, 64);
                strIpeContAllLayers +=
                    "<group>\n" +
                    CToIpe.TranIpgBoundToIpe(IpgLt[i / 2], pFLayerEnv, pIpeEnv, resultColor, strActive) +
                    CToIpe.TranIpgBoundToIpe(passiveIpgLt[i / 2], pFLayerEnv, pIpeEnv, passiveColor, strPassive) +               
                    "</group>\n";
                i++;
            }

            strIpeContAllLayers += CIpeDraw.SpecifyLayerByWritingText(strLayerNameLt[strLayerNameLt.Count-2], "removable", 320, 64);

            //draw a rectangle to cover the patch number of the last layer
            strIpeContAllLayers += CIpeDraw.drawIpeBox(304, 112, 384, 160, "white");

            //add a text of patch numbers
            strIpeContAllLayers += CIpeDraw.writeIpeText(strLayerNameLt[strLayerNameLt.Count - 2], 320, 128);

            //add the data
            strIpeContAllLayers += CToIpe.TranIpgToIpe(IpgLt[strLayerNameLt.Count /2-2], 
                pFillSymbolLt[strLayerNameLt.Count  / 2 -2], pFLayerEnv, pIpeEnv, strBoundWidth);


            return strIpeContAllLayers;
        }

        /// <summary>
        /// We use the patch number as the layer names
        /// </summary>
        /// <param name="intCphCount"></param>
        /// <param name="intCrgCount"></param>
        /// <returns></returns>
        private static List<string> GetLayerNames(int intCphCount, int intCrgCount)
        {
            var strLayerNameLt = new List<string>(2 * (intCphCount - intCrgCount + 1));
            //var strLayerNameLt = new List<string>(intCphCount - intCrgCount + 1);
            for (int i = intCphCount; i >= intCrgCount; i--)
            {
                strLayerNameLt.Add(i.ToString());
                strLayerNameLt.Add(i.ToString() + "a");
            }
            return strLayerNameLt;
        }

        /// <summary>
        /// Forget sequence so that we can release the memory
        /// </summary>
        /// <param name="lscrg"></param>
        /// <param name="sscrg"></param>
        /// <param name="blnTesting"></param>
        /// <remarks>This function has little influence, e.g., 0.001 MB for a region with 32 polygons and 74 adjacencies</remarks>
        protected static void CheckIfForgetSequence(CRegion lscrg, CRegion sscrg, bool blnTesting)
        {
            //var lngmemorycheck1 = GC.GetTotalMemory(true);  //for testing
            //var lngmemorycheck2 = GC.GetTotalMemory(true);  //for testing

            if (blnTesting == true)
            {
                lscrg.AdjCorrCphsSD = null;
                lscrg.child = null;

                sscrg.AdjCorrCphsSD = null;
                sscrg.parent = null;
            }

            //var lngmemory = GC.GetTotalMemory(true);                                           //for testing
            //var lngmemorycomsuption = CHelpFunc.GetConsumedMemoryInMB(true, lngmemorycheck2);  //for testing
        }

        #endregion
    }
}
