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
    public class CAreaAgg_Greedy : CAreaAgg_Base
    {
        public CAreaAgg_Greedy()
        {

        }

        public CAreaAgg_Greedy(CParameterInitialize ParameterInitialize, 
            string strSpecifiedFieldName = null, string strSpecifiedValue = null)
        {
            CConstants.strMethod = "Greedy";
            Preprocessing(ParameterInitialize, strSpecifiedFieldName, strSpecifiedValue);
        }

        public void AreaAggregation()
        {

            SetupBasic();
            

            CRegion._lngEstCountEdgeNumber = 0;
            CRegion._lngEstCountEdgeLength = 0;
            CRegion._lngEstCountEqual = 0;

            //we must run AStar first
            var strLineLt = File.ReadLines(_ParameterInitialize.strMxdPathBackSlash + "AStar200000_" 
                + _ParameterInitialize.strAreaAggregation + "_"+ CConstants.strShapeConstraint+ ".txt").ToList();
            var EstStepsCostVPDt = new Dictionary<int, CValPair<int, double>>(strLineLt.Count-1);  //record the results from A*
            for (int i = 1; i < strLineLt.Count; i++) //the first line is for headings
            {
                var strDetail = strLineLt[i].Split(new char[] { ' ', '\t' }, 
                    StringSplitOptions.RemoveEmptyEntries); //use white space to split
                EstStepsCostVPDt.Add(Convert.ToInt32(strDetail[0]), 
                    new CValPair<int, double>(Convert.ToInt32(strDetail[1]), Convert.ToDouble(strDetail[2])));
            }




              //@"C:\MyWork\DailyWork\ContinuousGeneralisation\RunContinuousGeneralizer\CallRecord.txt").Last();

            for (int i = _intStart; i < _intEndCount; i++)
            {
                Greedy(LSCrgLt[i], SSCrgLt[i], this.StrObjLtDt, this._adblTD,
                    EstStepsCostVPDt, _ParameterInitialize.strAreaAggregation);
                CheckIfForgetSequence(LSCrgLt[i], SSCrgLt[i], _ParameterInitialize.chkTesting.Checked);
                CHelpFunc.Displaytspb(i - _intStart + 1, _intEndCount - _intStart);
            }

            this.strLineLt = strLineLt;
            //_EstStepsCostVPDt = EstStepsCostVPDt;
            EndAffairs(_intEndCount);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LSCrg"></param>
        /// <param name="SSCrg"></param>
        /// <param name="StrObjLtDt"></param>
        /// <param name="adblTD"></param>
        /// <param name="EstStepsCostVPDt">Results from A*</param>
        /// <param name="strAreaAggregation"></param>
        /// <returns></returns>
        public CRegion Greedy(CRegion LSCrg, CRegion SSCrg, CStrObjLtDt StrObjLtDt, double[,] adblTD,
            Dictionary<int, CValPair<int, double>> EstStepsCostVPDt, string strAreaAggregation)
        {
            long lngStartMemory = GC.GetTotalMemory(true);
            var pStopwatchOverHead = Stopwatch.StartNew();

            var ExistingCorrCphsSD0 = LSCrg.SetInitialAdjacency();  //also count the number of edges
            AddLineToStrObjLtDt(StrObjLtDt, LSCrg);

            
            Console.WriteLine();
            Console.WriteLine("Crg:  ID  " + LSCrg.ID + ";    n  " + LSCrg.GetCphCount() + ";    m  " +
                    LSCrg.AdjCorrCphsSD.Count + "   " + CConstants.strShapeConstraint + "   " + strAreaAggregation);

            
            long lngTimeOverHead = pStopwatchOverHead.ElapsedMilliseconds;
            pStopwatchOverHead.Stop();

            var pStopwatchLast = new Stopwatch();
            long lngTime = 0;

            CRegion resultcrg = new CRegion(-2);
            try
            {
                pStopwatchLast.Restart();
                var ExistingCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>
                    (ExistingCorrCphsSD0, ExistingCorrCphsSD0.Comparer);
                LSCrg.cenumColor = CEnumColor.white;

                resultcrg = Compute(LSCrg, SSCrg, SSCrg.GetSoloCphTypeIndex(), 
                    strAreaAggregation, ExistingCorrCphsSD, StrObjLtDt, adblTD);
            }
            catch (System.OutOfMemoryException ex)
            {
                Console.WriteLine(ex.Message);
            }
            lngTime = pStopwatchLast.ElapsedMilliseconds + lngTimeOverHead;

            
            Console.WriteLine("d: " + resultcrg.d
                + "            Type: " + resultcrg.dblCostExactType
                + "            Compactness: " + resultcrg.dblCostExactComp);

            CValPair<int, double> outEstStepsCostVP;
            EstStepsCostVPDt.TryGetValue(LSCrg.ID, out outEstStepsCostVP);
            if (outEstStepsCostVP.val1 == 0 &&
    CCmpMethods.CmpDbl_CoordVerySmall(outEstStepsCostVP.val2, resultcrg.d) == 0)
            {
                StrObjLtDt.SetLastObj("EstSteps/Gap%", 0); //optimal solutions
            }
            else
            {
                StrObjLtDt.SetLastObj("EstSteps/Gap%", 100); //not sure, at least feasible solutions
            }
            //we don't need to +1 because +1 is already included in _intStaticGID
            //int intExploredRegionAll = CRegion._intStaticGID - CRegion._intStartStaticGIDLast; 
            StrObjLtDt.SetLastObj("#Edges", CRegion._intEdgeCount);
            StrObjLtDt.SetLastObj("Time_F(ms)", lngTime);
            StrObjLtDt.SetLastObj("Time_L(ms)", lngTime);
            StrObjLtDt.SetLastObj("Time(ms)", lngTime);
            StrObjLtDt.SetLastObj("Memory(MB)", CHelpFunc.GetConsumedMemoryInMB(false, lngStartMemory));

            Console.WriteLine("We have visited " + 
                CRegion._intNodeCount + " Nodes and " + CRegion._intEdgeCount + " Edges.");

            return resultcrg;
        }

        private CRegion Compute(CRegion lscrg, CRegion sscrg, int intFinalTypeIndex, string strAreaAggregation,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, CStrObjLtDt StrObjLtDt, double[,] padblTD)
        {
            CRegion._intNodeCount = 1;
            CRegion._intEdgeCount = 0;
            var currentCrg = lscrg;

            //after an aggregation, we whould have the largest compactness
            //it's urgent to remove the smallest one
            while (currentCrg.GetCphCount() > 1)
            {
                var smallestcph = currentCrg.GetSmallestCph();
                

                var CphRecordsEb = currentCrg.GetNeighborCphRecords(smallestcph);
                double dblMinCost = double.MaxValue;
                CCorrCphs minunitingCorrCphs = null;
                CPatch minunitedcph = null;
                CPatch minactivecph = null;
                CPatch minpassivecph = null;

                foreach (var cphrecord in CphRecordsEb)
                {
                    var neighborcph = cphrecord.Cph;
                    var unitingCorrCphs = cphrecord.CorrCphs;
                    var unitedcph = smallestcph.Unite(neighborcph, unitingCorrCphs.dblSharedSegLength);
                    CPatch activecph = neighborcph;
                    CPatch passivecph = smallestcph;
                    if (padblTD[currentCrg.GetCphTypeIndex(smallestcph), intFinalTypeIndex] <
                        padblTD[currentCrg.GetCphTypeIndex(neighborcph), intFinalTypeIndex])
                    {
                        activecph = smallestcph;
                        passivecph = neighborcph;
                    }

                    double dblCostType = padblTD[currentCrg.GetCphTypeIndex(activecph), currentCrg.GetCphTypeIndex(passivecph)] 
                        * passivecph.dblArea / lscrg.dblArea;

                    double dblCostShape = 0;
                    if (CConstants.strShapeConstraint == "MaxAvgC_EdgeNo" ||
                        CConstants.strShapeConstraint == "MaxAvgC_Comb")
                    {
                        if (lscrg.GetCphCount() - 2>0)
                        {
                            double dblNewSumComp = currentCrg.dblSumComp - cphrecord.CorrCphs.FrCph.dblComp - 
                                cphrecord.CorrCphs.ToCph.dblComp + unitedcph.dblComp;
                            double dblNewAvgComp = dblNewSumComp / (currentCrg.GetCphCount() - 1);
                            dblCostShape = (1 - dblNewAvgComp) / (lscrg.GetCphCount() - 2);
                        }
                        
                    }
                    else if (CConstants.strShapeConstraint == "MinIntBound")
                    {
                        if (lscrg.GetCphCount() - 2 > 0)
                        {
                            double dblNewLength = currentCrg.dblInteriorSegLength - cphrecord.CorrCphs.dblSharedSegLength;
                            dblCostShape = dblNewLength * (lscrg.GetCphCount() - 1) / (lscrg.GetCphCount() - 2)
                                / (currentCrg.GetCphCount() - 1) / lscrg.dblInteriorSegLength;
                        }
                    }
                    else
                    {
                        //CConstants.strShapeConstraint == "MaxMinC_EdgeNo" ||
                        //CConstants.strShapeConstraint == "MaxMinC_Comb" ||
                        throw new ArgumentException("We didn't consider the case!");
                    }

                    double dblCost = (1 - CAreaAgg_Base.dblLamda) * dblCostType + CAreaAgg_Base.dblLamda * dblCostShape;

                    if (dblCost < dblMinCost)
                    {
                        dblMinCost = dblCost;
                        minunitingCorrCphs = unitingCorrCphs;
                        minunitedcph = unitedcph;
                        minactivecph = activecph;
                        minpassivecph = passivecph;
                    }
                }

                var newAdjCorrCphsSD = CRegion.ComputeNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD
                    (currentCrg.AdjCorrCphsSD, minunitingCorrCphs, minunitedcph, ExistingCorrCphsSD);
                var newcrg = currentCrg.GenerateCrgChildAndComputeCost(lscrg, newAdjCorrCphsSD,
             minactivecph, minpassivecph, minunitedcph, minunitingCorrCphs, padblTD);

                newcrg.d = newcrg.dblCostExact;

                CRegion._intNodeCount++;
                CRegion._intEdgeCount++;
                currentCrg = newcrg;
            }

            RecordResultForCrg(StrObjLtDt, lscrg, currentCrg, intFinalTypeIndex);

            return currentCrg;
        }


        //private CCphRecord GetNeighborCphByCompactness(CRegion crg, CPatch cph)
        //{
        //    double dblSumComp = 0;  //the sum compactness of neighbours
        //    var CphRecordsEb = crg.GetNeighborCphRecords(cph);
        //    foreach (var cphrecord in CphRecordsEb)
        //    {
        //        dblSumComp += cphrecord.Cph.dblComp;
        //    }

        //    double dblMaxSumComp = 0;
        //    CCphRecord maxCphRecord = null;
        //    foreach (var cphrecord in CphRecordsEb)
        //    {
        //        var unitedcph = cph.Unite(cphrecord.Cph, cphrecord.CorrCphs.dblSharedSegLength);
        //        double dblNewSumComp = dblSumComp - cphrecord.Cph.dblComp + unitedcph.dblComp;

        //        if (dblMaxSumComp < dblNewSumComp)
        //        {
        //            dblMaxSumComp = dblNewSumComp;
        //            maxCphRecord = cphrecord;
        //        }
        //    }

        //    return maxCphRecord;
        //}

        //private CCphRecord GetNeighborCphByLength(CRegion crg, CPatch cph)
        //{
        //    var CphRecordsEb = crg.GetNeighborCphRecords(cph);

        //    double dblMaxSharedLength = 0;
        //    CCphRecord maxCphRecord = null;
        //    foreach (var cphrecord in CphRecordsEb)
        //    {
        //        if (dblMaxSharedLength < cphrecord.CorrCphs.dblSharedSegLength)
        //        {
        //            maxCphRecord = cphrecord;
        //            dblMaxSharedLength = cphrecord.CorrCphs.dblSharedSegLength;
        //        }
        //    }

        //    return maxCphRecord;
        //}
    }
}
