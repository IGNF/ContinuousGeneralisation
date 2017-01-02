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
                    LSCrg.Adjacency_CorrCphsSD.Count + "   " + CConstants.strShapeConstraint + "   " + strAreaAggregation);

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

                    //resultcrg = ComputeAccordFactor(LSCrg, SSCrg, strAreaAggregation, ExistingCorrCphsSD, intFactor, StrObjLtSD, intQuitCount);
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

        private CRegion Compute(CRegion lscrg, CRegion sscrg, string strAreaAggregation, CStrObjLtSD StrObjLtSD,
            List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, int intFinalTypeIndex, double[,] padblTD)
        {
            var currentCrg = lscrg;

            //after an aggregation, we whould have the largest compactness
            //it's urgent to remove the smallest one
            while (currentCrg.GetCphCount() > 1)
            {
                var smallestcph = currentCrg.GetSmallestCph();
                var smallestneighborcph = new CPatch(false);
                smallestneighborcph.dblArea = lscrg.dblArea;  //just an initial value
                CCorrCphs unitingCorrCphs = null;
                foreach (var pCorrCphs in currentCrg.Adjacency_CorrCphsSD.Keys)
                {
                    var neighborcph = CRegion.TryGetNeighbor(smallestcph, pCorrCphs);
                    if (neighborcph != null && neighborcph.dblArea < smallestneighborcph.dblArea)
                    {
                        smallestneighborcph = neighborcph;
                        unitingCorrCphs = pCorrCphs;
                    }
                }

                var unitedcph = smallestcph.Unite(smallestneighborcph, unitingCorrCphs.dblSharedSegmentLength);
                CPatch activecph = smallestneighborcph;
                CPatch passivecph = smallestcph;
                if (padblTD[smallestcph.intTypeIndex, sscrg.GetSmallestCph().intTypeIndex] <
                     padblTD[smallestneighborcph.intTypeIndex, sscrg.GetSmallestCph().intTypeIndex])
                {
                    activecph = smallestcph;
                    passivecph = smallestneighborcph;
                }


                int intfrTypeIndex = currentCrg.GetCphTypeIndex(activecph);
                int inttoTypeIndex = currentCrg.GetCphTypeIndex(passivecph);

                var newAdjacency_CorrCphsSD = CRegion.ComputeNewAdjacency_CorrCphsSDAndUpdateExistingCorrCphsSD
                    (currentCrg.Adjacency_CorrCphsSD, unitingCorrCphs, unitedcph, ExistingCorrCphsSD);
                var newcrg = currentCrg.GenerateCrgChild(lscrg, newAdjacency_CorrCphsSD,
             activecph, passivecph, unitedcph, unitingCorrCphs, padblTD);

                currentCrg = newcrg;
            }


            return null;
        }





    }
}
