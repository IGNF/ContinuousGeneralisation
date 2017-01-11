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
    public class CAreaAggregation_Greedy : CAreaAggregation_Base
    {
        public CAreaAggregation_Greedy()
        {

        }

        public CAreaAggregation_Greedy(CParameterInitialize ParameterInitialize, string strSpecifiedFieldName = null, string strSpecifiedValue = null)
        {
            Preprocessing(ParameterInitialize, strSpecifiedFieldName, strSpecifiedValue);
        }

        public void AreaAggregation()
        {

            SetupBasic();
            

            CRegion._lngEstimationCountEdgeNumber = 0;
            CRegion._lngEstimationCountEdgeLength = 0;
            CRegion._lngEstimationCountEqual = 0;

            for (int i = _intStart; i < _intEnd; i++)
            {
                Greedy(LSCrgLt[i], SSCrgLt[i], this.StrObjLtSD, _ParameterInitialize.strAreaAggregation);
            }
            Console.WriteLine();
            Console.WriteLine("Estimation functions that we used:");
            Console.WriteLine("By EdgeNumber: " + CRegion._lngEstimationCountEdgeNumber +
                ";   By EdgeLength: " + CRegion._lngEstimationCountEdgeLength +
                ";   EqualCases: " + CRegion._lngEstimationCountEqual);

        }


        public CRegion Greedy(CRegion LSCrg, CRegion SSCrg, CStrObjLtSD StrObjLtSD, string strAreaAggregation)
        {
            var ExistingCorrCphsSD0 = LSCrg.SetInitialAdjacency();  //also count the number of edges

            Stopwatch pStopwatchOverHead = new Stopwatch();
            pStopwatchOverHead.Start();
            int intFactor = _intFactor;
            CRegion resultcrg = new CRegion(-2);
            CRegion._intStartStaticGIDAll = CRegion._intStaticGID;


            AddLineToStrObjLtSD(StrObjLtSD, LSCrg);

            long lngStartMemory = 0;
            Console.WriteLine();
            Console.WriteLine("Crg:  ID  " + LSCrg.ID + ";    n  " + LSCrg.CphTypeIndexSD_Area_CphGID.Count + ";    m  " +
                    LSCrg.AdjCorrCphsSD.Count + "   " + CConstants.strShapeConstraint + "   " + strAreaAggregation);

            lngStartMemory = GC.GetTotalMemory(true);
            long lngTimeOverHead = pStopwatchOverHead.ElapsedMilliseconds;
            pStopwatchOverHead.Stop();

            Stopwatch pStopwatchLast = new Stopwatch();
            bool blnRecordTimeFirst = false;
            long lngTimeFirst = 0;
            long lngTimeLast = 0;
            long lngTimeAll = lngTimeOverHead;
            do
            {
                try
                {
                    CRegion._intNodesCount = 1;
                    CRegion._intStartStaticGIDLast = CRegion._intStaticGID;
                    pStopwatchLast.Restart();
                    var ExistingCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>(ExistingCorrCphsSD0, ExistingCorrCphsSD0.Comparer);
                    LSCrg.cenumColor = CEnumColor.white;

                    resultcrg = Compute(LSCrg, SSCrg, strAreaAggregation, ExistingCorrCphsSD, StrObjLtSD, this._adblTD);
                }
                catch (System.OutOfMemoryException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (blnRecordTimeFirst == false)
                {
                    lngTimeFirst = pStopwatchLast.ElapsedMilliseconds + lngTimeOverHead;
                    blnRecordTimeFirst = true;
                }
                lngTimeLast = pStopwatchLast.ElapsedMilliseconds + lngTimeOverHead;
                lngTimeAll += pStopwatchLast.ElapsedMilliseconds;

                intFactor *= 2;
            } while (resultcrg.ID == -2);
            intFactor /= 2;
            StrObjLtSD.SetLastObj("Factor", intFactor);
            Console.WriteLine("d: " + resultcrg.d
                + "            Type: " + resultcrg.dblCostExactType
                + "            Compactness: " + resultcrg.dblCostExactCompactness);

            int intExploredRegionAll = CRegion._intStaticGID - CRegion._intStartStaticGIDLast;  //we don't need to +1 because +1 is already included in _intStaticGID
            double dblConsumedMemoryInMB = CHelperFunction.GetConsumedMemoryInMB(false);

            StrObjLtSD.SetLastObj("#Edges", intExploredRegionAll);
            StrObjLtSD.SetLastObj("TimeFirst(ms)", lngTimeFirst);
            StrObjLtSD.SetLastObj("TimeLast(ms)", lngTimeLast);
            StrObjLtSD.SetLastObj("Time(ms)", lngTimeAll);
            StrObjLtSD.SetLastObj("Memory(MB)", CHelperFunction.GetConsumedMemoryInMB(false, lngStartMemory));

            Console.WriteLine("Factor:" + intFactor + "      We have visited " + intExploredRegionAll + " Regions.");

            return resultcrg;
        }

        private CRegion Compute(CRegion lscrg, CRegion sscrg, string strAreaAggregation,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, CStrObjLtSD StrObjLtSD, double[,] padblTD)
        {
            CRegion._intNodesCount = 1;
            var currentCrg = lscrg;

            //after an aggregation, we whould have the largest compactness
            //it's urgent to remove the smallest one
            while (currentCrg.GetCphCount() > 1)
            {
                var smallestcph = currentCrg.GetSmallestCph();
                //var smallestneighborcph = new CPatch();
                //smallestneighborcph.dblArea = lscrg.dblArea;  //just an initial value
                //CCorrCphs unitingCorrCphs = null;
                //foreach (var pCorrCphs in currentCrg.AdjCorrCphsSD.Keys)
                //{
                //    var neighborcph = CRegion.TryGetNeighbor(smallestcph, pCorrCphs);
                //    if (neighborcph != null && neighborcph.dblArea < smallestneighborcph.dblArea)
                //    {
                //        smallestneighborcph = neighborcph;
                //        unitingCorrCphs = pCorrCphs;
                //    }
                //}

                CCphRecord pcphRecord = null;
                if (CConstants.strShapeConstraint == "MaximizeMinComp" || CConstants.strShapeConstraint == "MaximizeMinComp_Combine" ||
                    CConstants.strShapeConstraint == "MaximizeAvgComp" || CConstants.strShapeConstraint == "MaximizeAvgComp_Combine")
                {
                    pcphRecord = GetNeighborCphByCompactness(currentCrg, smallestcph);
                }
                else if (CConstants.strShapeConstraint == "MinimizeInteriorBoundaries")
                {
                    pcphRecord = GetNeighborCphByLength(currentCrg, smallestcph);
                }

                var neighborcph = pcphRecord.Cph;
                var unitingCorrCphs = pcphRecord.CorrCphs;
                var unitedcph = smallestcph.Unite(neighborcph, unitingCorrCphs.dblSharedSegmentLength);
                CPatch activecph = neighborcph;
                CPatch passivecph = smallestcph;
                int intFinalTypeIndex = sscrg.GetCphTypeIndex(sscrg.GetSmallestCph());
                if (padblTD[currentCrg.GetCphTypeIndex(smallestcph), intFinalTypeIndex] <
                    padblTD[currentCrg.GetCphTypeIndex(neighborcph), intFinalTypeIndex])
                {
                    activecph = smallestcph;
                    passivecph = neighborcph;
                }


                int intfrTypeIndex = currentCrg.GetCphTypeIndex(activecph);
                int inttoTypeIndex = currentCrg.GetCphTypeIndex(passivecph);

                var newAdjCorrCphsSD = CRegion.ComputeNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD
                    (currentCrg.AdjCorrCphsSD, unitingCorrCphs, unitedcph, ExistingCorrCphsSD);
                var newcrg = currentCrg.GenerateCrgChildAndComputeCost(lscrg, newAdjCorrCphsSD,
             activecph, passivecph, unitedcph, unitingCorrCphs, intfrTypeIndex, inttoTypeIndex, padblTD);

                newcrg.d = newcrg.dblCostExact;

                CRegion._intNodesCount++;
                currentCrg = newcrg;
            }

            RecordResultForCrg(StrObjLtSD, lscrg, currentCrg, sscrg.GetSoloCphTypeIndex());



            return currentCrg;
        }


        private CCphRecord GetNeighborCphByCompactness(CRegion crg, CPatch cph)
        {
            double dblSumComp = 0;  //the sum compactness of neighbours
            var CphRecordsEb = crg.GetNeighborCphRecords(cph);
            foreach (var cphrecord in CphRecordsEb)
            {
                dblSumComp += cphrecord.Cph.dblComp;
            }


            double dblMaxSumComp = 0;
            CCphRecord maxCphRecord = null;
            foreach (var cphrecord in CphRecordsEb)
            {
                var unitedcph = cph.Unite(cphrecord.Cph, cphrecord.CorrCphs.dblSharedSegmentLength);
                double dblNewSumComp = dblSumComp - cphrecord.Cph.dblComp + unitedcph.dblComp;

                if (dblMaxSumComp < dblNewSumComp)
                {
                    dblMaxSumComp = dblNewSumComp;
                    maxCphRecord = cphrecord;
                }
            }

            return maxCphRecord;
        }

        private CCphRecord GetNeighborCphByLength(CRegion crg, CPatch cph)
        {
            var CphRecordsEb = crg.GetNeighborCphRecords(cph);

            double dblMaxSharedLength = 0;
            CCphRecord maxCphRecord = null;
            foreach (var cphrecord in CphRecordsEb)
            {
                if (dblMaxSharedLength < cphrecord.CorrCphs.dblSharedSegmentLength)
                {
                    maxCphRecord = cphrecord;
                }
            }

            return maxCphRecord;
        }
    }
}
