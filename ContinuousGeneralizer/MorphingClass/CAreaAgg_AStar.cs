﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

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
    /// <summary>
    /// Continuous Aggregation of Maps based on Dijkstra: AreaAgg_AStar
    /// </summary>
    /// <remarks></remarks>
    public class CAreaAgg_AStar : CAreaAgg_Base
    {
        #region Preprocessing
        public CAreaAgg_AStar()
        {

        }

        public CAreaAgg_AStar(CParameterInitialize ParameterInitialize, 
            string strSpecifiedFieldName = null, string strSpecifiedValue = null)
        {
            CConstants.strMethod = "AStar";
            Preprocessing(ParameterInitialize, strSpecifiedFieldName, strSpecifiedValue);
        }


        public void AreaAggregation(int intQuitCount)
        {
            SetupBasic();

            CRegion._lngEstCountEdgeNumber = 0;
            CRegion._lngEstCountEdgeLength = 0;
            CRegion._lngEstCountEqual = 0;


            //var proc = Process.GetCurrentProcess();
            for (int i = _intStart; i < _intEndCount; i++)
            {
                //we computer a sequence of regions, 
                //for each region, we record a parent and a child if applicable
                AStar(LSCrgLt[i], SSCrgLt[i], this.StrObjLtDt, _ParameterInitialize.strAreaAggregation, this._adblTD, intQuitCount);
                CheckIfForgetSequence(LSCrgLt[i], SSCrgLt[i], _ParameterInitialize.chkTesting.Checked);
                CHelpFunc.Displaytspb(i - _intStart + 1, _intEndCount - _intStart );
            }

            EndAffairs(_intEndCount);

            //Console.WriteLine("{0,25}{1,25}{2,25}{3,25}{4,25}{5,25}",
            //    "PagedMemorySize64", "PagedSystemMemorySize64",
            //    "PeakPagedMemorySize64", "PeakVirtualMemorySize64",
            //    "PeakWorkingSet64", "PrivateMemorySize64");
            //for (int i = 0; i < _lngMemoryLtLt.Count; i++)
            //{
            //    Console.WriteLine("{0,25}{1,25}{2,25}{3,25}{4,25}{5,25}",
            //    _lngMemoryLtLt[i][0], _lngMemoryLtLt[i][1],
            //    _lngMemoryLtLt[i][2], _lngMemoryLtLt[i][3],
            //    _lngMemoryLtLt[i][4], _lngMemoryLtLt[i][5]);
            //}

        }
        #endregion




        #region AStar
        public CRegion AStar(CRegion LSCrg, CRegion SSCrg, CStrObjLtDt StrObjLtDt, string strAreaAggregation, 
            double[,] padblTD, int intQuitCount = 200000, Process proc=null)
        {
            long lngStartMemory = GC.GetTotalMemory(true);
            

            var pStopwatchOverHead = Stopwatch.StartNew();


            var ExistingCorrCphsSD0 = LSCrg.SetInitialAdjacency();  //also count the number of edges
            AddLineToStrObjLtDt(StrObjLtDt, LSCrg);


            Console.WriteLine();
            Console.WriteLine("Crg:  ID  " + LSCrg.ID + ";    n  " + LSCrg.GetCphCount() + ";    m  " +
                    LSCrg.AdjCorrCphsSD.Count + "   " + intQuitCount + "   " + 
                    CConstants.strShapeConstraint + "   " + strAreaAggregation);

            
            var lngTimeOverHead = pStopwatchOverHead.ElapsedMilliseconds;
            pStopwatchOverHead.Stop();

            var pStopwatchLast =new Stopwatch ();
            bool blnRecordTime_F = false;
            long lngTime_F = 0; //running time of the first trying
            long lngTime_L = 0; //running time of last trying
            long lngTimeAll = lngTimeOverHead;
            var resultcrg = new CRegion(-2);
            int intEstSteps = 0;
            int intRound = _intRound;
            do
            {
                intEstSteps = Convert.ToInt32(Math.Pow(2, intRound)) - 1;
                try
                {

                    //CRegion._intStartStaticGIDLast = CRegion._intStaticGID;
                    pStopwatchLast.Restart();
                    var ExistingCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>
                        (ExistingCorrCphsSD0, ExistingCorrCphsSD0.Comparer);
                    LSCrg.cenumColor = CEnumColor.white;

                    resultcrg = ComputeAccordEstSteps(LSCrg, SSCrg, strAreaAggregation, ExistingCorrCphsSD,
                        intEstSteps, StrObjLtDt, padblTD, intQuitCount);
                }
                catch (System.OutOfMemoryException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (blnRecordTime_F == false)
                {
                    lngTime_F = pStopwatchLast.ElapsedMilliseconds + lngTimeOverHead;
                    blnRecordTime_F = true;
                }
                lngTime_L = pStopwatchLast.ElapsedMilliseconds + lngTimeOverHead;
                lngTimeAll += pStopwatchLast.ElapsedMilliseconds;

                if (resultcrg.ID != -2)
                {
                    break;
                }
                if (intEstSteps>1000)
                {
                    intEstSteps = 1024;
                    throw new ArgumentException("We cannot solve the problem! Impossible!");
                }
                
                intRound++;
            } while (true);
            StrObjLtDt.SetLastObj("EstSteps/Gap%", intEstSteps);
            Console.WriteLine("d: " + resultcrg.d 
                + "            Type: " + resultcrg.dblCostExactType 
                + "            Compactness: " + resultcrg.dblCostExactComp);

            //we don't need to +1 because +1 is already included in _intStaticGID
            //int intExploredRegionAll = CRegion._intStaticGID - CRegion._intStartStaticGIDLast;  
            //double dblConsumedMemoryInMB = CHelpFunc.GetConsumedMemoryInMB(false);

            StrObjLtDt.SetLastObj("#Edges", CRegion._intEdgeCount);
            StrObjLtDt.SetLastObj("Time_F(ms)", lngTime_F);
            StrObjLtDt.SetLastObj("Time_L(ms)", lngTime_L);
            StrObjLtDt.SetLastObj("Time(ms)", lngTimeAll);
            StrObjLtDt.SetLastObj("Memory(MB)", CHelpFunc.GetConsumedMemoryInMB(false, lngStartMemory));

            Console.WriteLine("EstSteps/Gap%: " + intEstSteps + "      We have visited " + 
                CRegion._intNodeCount + " Nodes and " + CRegion._intEdgeCount + " Edges.");

            return resultcrg;
        }

        private CRegion ComputeAccordEstSteps(CRegion LSCrg, CRegion SSCrg, string strAreaAggregation,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, int intEstSteps, CStrObjLtDt StrObjLtDt,
            double[,] padblTD, int intQuitCount = 200000)
        {
            int intRegionID = LSCrg.ID;  //all the regions generated in this function will have the same intRegionID

            ComputeEstCost(LSCrg, SSCrg, LSCrg, padblTD, intEstSteps);

            //LSCrg.InitialEstimatedCost(SSCrg, padblTD, intEstSteps);
            //LSCrg.SetCoreCph(intSSTypeIndex);

            //a region represents a node in graph, ExistingCrgSD stores all the nodes
            //we use this dictionary to make sure that if the two patches have the same cpgs, then they have the same GID
            var ExistingCphSDLt = new List<SortedDictionary<CPatch, CPatch>>(LSCrg.GetCphCount() + 1);
            for (int i = 0; i < ExistingCphSDLt.Capacity; i++)
            {
                var Element = new SortedDictionary<CPatch, CPatch>(CPatch.pCmpCPatch_CpgGID);
                ExistingCphSDLt.Add(Element);
            }

            var ExistingCrgSDLt = new List<SortedDictionary<CRegion, CRegion>>(LSCrg.GetCphCount() + 1);
            for (int i = 0; i < ExistingCrgSDLt.Capacity; i++)
            {
                //we don't compare exact cost first because of there may be rounding problems 
                var Element = new SortedDictionary<CRegion, CRegion>(CRegion.pCmpCrg_CphGIDTypeIndex);
                ExistingCrgSDLt.Add(Element);
            }
            ExistingCrgSDLt[LSCrg.GetCphCount()].Add(LSCrg, LSCrg);

            var FinalOneCphCrg = new CRegion(intRegionID);
            var Q = new SortedSet<CRegion>(CRegion.pCmpCrg_Cost_CphGIDTypeIndex);
            int intCount = 0;
            CRegion._intNodeCount = 1;
            CRegion._intEdgeCount = 0;
            Q.Add(LSCrg);
            while (true)
            {
                intCount++;
                var u = Q.Min;
                if (Q.Remove(u) == false)
                {
                    if (Q.Count == 0)
                    {
                        throw new ArgumentNullException("Q should not be empty!");
                    }

                    throw new ArgumentException
                        ("cannot move an element in this queue! A solution might be to make dblVerySmall smaller!");
                }
                u.cenumColor = CEnumColor.black;

                //List<CRegion> crgcol = new List<CRegion>();
                //crgcol.Add(u);

                //OutputMap(crgcol, this._TypePVDt, u.d, intCount, pParameterInitialize);

                //MessageBox.Show("click for next!");

                //if (CConstants.strShapeConstraint == "MaxMinC_EdgeNo" || 
                //    CConstants.strShapeConstraint == "MinIntBound")
                //{
                //    Console.WriteLine("Crg:  ID  " + u.ID + ";      GID:" + u.GID + ";      CphNum:" +  u.GetCphCount() + 
                //        ";      d:" + u.d / u.dblArea + ";      ExactCost:" + u.dblCostExact / u.dblArea + 
                //        ";      Compactness:" + u.dblCostExactComp + ";      Type:" + u.dblCostExactType / u.dblArea);
                //}
                //else if (CConstants.strShapeConstraint == "NonShape")
                //{
                //    Console.WriteLine("Crg:  ID  " + u.ID + ";      GID:" + u.GID + ";      CphNum:" + u.GetCphCount() + 
                //        ";      d:" + u.d + ";      ExactCost:" + u.dblCostExactType);
                //}

                //at the beginning, resultcrg.d is double.MaxValue. 
                //Later, when we first encounter that there is only one CPatch in LSCrg, 
                //resultcrg.d will be changed to the real cost
                //u.d contains estimation, and resultcrg.d doesn't contains. 
                //if u.d > resultcrg.d, then resultcrg.d must already be the smallest cost
                if (u.GetCphCount() == 1)
                {
                    if (u.GetSoloCphTypeIndex() == SSCrg.GetSoloCphTypeIndex())
                    {
                        //Console.WriteLine("The number of nodes we can forget:   " + intCount);
                        //Console.WriteLine("The nodes in the stack:   " + Q.Count);

                        //int intCrgCount = 0;
                        //foreach (var item in ExistingCrgSDLt)
                        //{
                        //    intCrgCount += item.Count;
                        //}

                        FinalOneCphCrg = u;
                        break;
                    }
                    else
                    {
                        //int u_type;
                        //int SSCrg_type;
                        //    //= SSCrg.GetSoloCphTypeIndex();

                        //_TypePVDt.Dt_R.TryGetValue(u.GetSoloCphTypeIndex(), out u_type);
                        //_TypePVDt.Dt_R.TryGetValue(SSCrg.GetSoloCphTypeIndex(), out SSCrg_type);

                        //The target region will be reached earlier than other regions with types other than the target type 
                        //because of the distance.
                        //In future, we can prevent regions that do not own the target type 
                        //so that we can cut some branches earlier ************************Improve********************************
                        throw new ArgumentException("this should be impossible in practice!");
                        //continue;
                    }
                }


                foreach (var newcrg in AggregateAndUpdateQ(u, LSCrg, SSCrg, Q, strAreaAggregation,
                    ExistingCrgSDLt, ExistingCphSDLt, ExistingCorrCphsSD, _adblTD, intEstSteps))
                {
                    //int intExploredRegionLast = CRegion._intStaticGID - CRegion._intStartStaticGIDLast;  
                    //we don't need to +1 because +1 is already included in _intStaticGID

                    if (CRegion._intNodeCount > intQuitCount)
                    {
                        //if we have visited 2000000 regions but haven't found an optimum aggregation sequence, 
                        //then we return null and overestimate in the heuristic function 
                        return new CRegion(-2);
                    }
                }
            }

            RecordResultForCrg(StrObjLtDt, LSCrg, FinalOneCphCrg, SSCrg.GetSoloCphTypeIndex());
            return FinalOneCphCrg;
        }





        public IEnumerable<CRegion> AggregateAndUpdateQ(CRegion crg, CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q,
            string strAreaAggregation,
            List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, double[,] padblTD, int intEstSteps)
        {
            if (strAreaAggregation == _strSmallest)
            {
                foreach (var item in AggregateSmallestAndUpdateQ(crg, lscrg, sscrg, Q,
                    ExistingCrgSDLt, ExistingCphSDLt, ExistingCorrCphsSD, padblTD, intEstSteps))
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in AggregateAllAndUpdateQ(crg, lscrg, sscrg, Q,
                    ExistingCrgSDLt, ExistingCphSDLt, ExistingCorrCphsSD, padblTD, intEstSteps))
                {
                    yield return item;
                }
            }
        }


        public IEnumerable<CRegion> AggregateSmallestAndUpdateQ(CRegion crg, CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q,
            List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, double[,] padblTD, int intEstSteps)
        {
            var pAdjCorrCphsSD = crg.AdjCorrCphsSD;
            var mincph = crg.GetSmallestCph();

            var pCphRecords = crg.GetNeighborCphRecords(mincph).ToList();
            if (pCphRecords.Count == 0)
            {
                CSaveFeature.SaveCpgEb(mincph.CpgSS, "PolygonsOfMincph");                    
                throw new ArgumentNullException("There is no neighbor of mincph!");
            }

            //for every pair of neighboring Cphs, we aggregate them and generate a new Crg
            foreach (var pCphRecord in pCphRecords)
            {
                foreach (var item in AggregateAndUpdateQ_Common(crg, lscrg, sscrg, Q, pAdjCorrCphsSD, pCphRecord.CorrCphs,
                    ExistingCrgSDLt, ExistingCphSDLt, ExistingCorrCphsSD, padblTD, intEstSteps))
                {
                    yield return item;
                }
            }
        }


        public IEnumerable<CRegion> AggregateAllAndUpdateQ(CRegion crg, CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q,
            List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, double[,] padblTD, int intEstSteps)
        {
            var pAdjCorrCphsSD = crg.AdjCorrCphsSD;

            //for every pair of neighboring Cphs, we aggregate them and generate a new Crg
            foreach (var unitingCorrCphs in pAdjCorrCphsSD.Keys)
            {
                foreach (var item in AggregateAndUpdateQ_Common(crg, lscrg, sscrg, Q, pAdjCorrCphsSD, unitingCorrCphs,
                    ExistingCrgSDLt, ExistingCphSDLt, ExistingCorrCphsSD, padblTD, intEstSteps))
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<CRegion> AggregateAndUpdateQ_Common(CRegion crg, CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q,
            SortedDictionary<CCorrCphs, CCorrCphs> pAdjCorrCphsSD, CCorrCphs unitingCorrCphs,
            List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, double[,] padblTD, int intEstSteps)
        {

            var newcph = crg.ComputeNewCph(unitingCorrCphs, ExistingCphSDLt);
            var newAdjCorrCphsSD = CRegion.ComputeNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD(pAdjCorrCphsSD,
                unitingCorrCphs, newcph, ExistingCorrCphsSD);


            var frcph = unitingCorrCphs.FrCph;
            var tocph = unitingCorrCphs.ToCph;
            int intfrTypeIndex = crg.GetCphTypeIndex(frcph);
            int inttoTypeIndex = crg.GetCphTypeIndex(tocph);


            //if the two cphs have the same type, then we only need to aggregate the smaller one into the larger one 
            //(this will certainly have smaller cost in terms of area)
            //otherwise, we need to aggregate from two directions
            if (intfrTypeIndex == inttoTypeIndex)
            {
                if (frcph.dblArea >= tocph.dblArea)
                {
                    yield return GenerateCrgAndUpdateQ(crg, lscrg, sscrg, Q, ExistingCrgSDLt, newAdjCorrCphsSD,
                        frcph, tocph, newcph, unitingCorrCphs, padblTD, intEstSteps);
                }
                else
                {
                    yield return GenerateCrgAndUpdateQ(crg, lscrg, sscrg, Q, ExistingCrgSDLt, newAdjCorrCphsSD,
                        tocph, frcph, newcph, unitingCorrCphs, padblTD, intEstSteps);
                }
            }
            else
            {
                //The aggregate can happen from two directions
                yield return GenerateCrgAndUpdateQ(crg, lscrg, sscrg, Q, ExistingCrgSDLt, newAdjCorrCphsSD,
                    frcph, tocph, newcph, unitingCorrCphs, padblTD, intEstSteps);
                yield return GenerateCrgAndUpdateQ(crg, lscrg, sscrg, Q, ExistingCrgSDLt, newAdjCorrCphsSD,
                    tocph, frcph, newcph, unitingCorrCphs, padblTD, intEstSteps);
            }
        }


        /// <summary>
        /// compute cost during generating a new Crg
        /// </summary>
        /// <param name="newAdjCorrCphsSD"></param>
        /// <param name="activecph"></param>
        /// <param name="passivecph"></param>
        /// <param name="unitedcph"></param>
        /// <param name="intFinalTypeIndex"></param>
        /// <param name="padblTD"></param>
        /// <returns></returns>
        public CRegion GenerateCrgAndUpdateQ(CRegion crg, CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q,
            List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, SortedDictionary<CCorrCphs, CCorrCphs> newAdjCorrCphsSD,
            CPatch activecph, CPatch passivecph, CPatch unitedcph, CCorrCphs unitingCorrCphs,
            double[,] padblTD, int intEstSteps)
        {
            int intactiveTypeIndex = crg.GetCphTypeIndex(activecph);
            int intpassiveTypeIndex = crg.GetCphTypeIndex(passivecph);

            var newcrg = crg.GenerateCrgChildAndComputeExactCost(lscrg, newAdjCorrCphsSD,
             activecph, passivecph, unitedcph, unitingCorrCphs, padblTD);

            if (ExistingCrgSDLt[newcrg.GetCphCount()].TryGetValue(newcrg, out CRegion outcrg))
            {
                int intResult = newcrg.dblCostExact.CompareTo(outcrg.dblCostExact);
                //int intResult = CCmpMethods.CmpDbl_CoordVerySmall(newcrg.dblCostExact, outcrg.dblCostExact);
                

                if (intResult == -1)
                {
                    //from the idea of A* algorithm, we know that outcrg must be in Q
                    //var Q = new SortedSet<CRegion>(CRegion.pCmpCrg_Cost_CphGIDTypeIndex);
                    //there is no decrease key function for SortedSet, so we have to remove it and later add it again
                    if (Q.Remove(outcrg) == true)
                    {

                        //if (outcrg.GID == 1698)
                        //{
                        //    Console.WriteLine("Before Replacing Region 1698");
                        //    foreach (var cpg in outcrg.CphCpgSD_Area_CphGID.Values)
                        //    {
                        //        Console.WriteLine(cpg.ID);
                        //    }
                        //}


                        //we don't use newcrg dicrectly because some regions may use outcrg as their child
                        //outcrg.cenumColor = newcrg.cenumColor;
                        //outcrg.dblCostExactType = newcrg.dblCostExactType;
                        //outcrg.dblCostExactComp = newcrg.dblCostExactComp;
                        //outcrg.dblCostExactArea = newcrg.dblCostExactArea;
                        //outcrg.dblCostExact = newcrg.dblCostExact;
                        //outcrg.d = newcrg.dblCostExact + outcrg.dblCostEst;
                        //outcrg.CphCpgSD_Area_CphGID = newcrg.CphCpgSD_Area_CphGID;
                        //outcrg.AggCphs = newcrg.AggCphs;
                        //outcrg.parent = newcrg.parent;
                        //newcrg = outcrg;


                        newcrg.dblCostEst = outcrg.dblCostEst;
                        newcrg.dblCostEstType = outcrg.dblCostEstType;
                        newcrg.dblCostEstComp = outcrg.dblCostEstComp;
                        newcrg.dblCostEstArea = outcrg.dblCostEstArea;
                        newcrg.d = newcrg.dblCostExact + outcrg.dblCostEst;



                        //var cphcount = newcrg.GetCphCount();
                        //var dblCostExactType = newcrg.dblCostExactType;
                        //var dblCostEstType = newcrg.dblCostEstType;
                        //var dblCostExactComp = newcrg.dblCostExactComp * newcrg.dblArea;
                        //var dblCostEstComp = newcrg.dblCostEstComp * newcrg.dblArea;

                        //if ((newcrg.GetCphCount() == 3 || newcrg.GetCphCount() == 4) 
                        //    && newcrg.dblCostEstType == 0 && newcrg.dblCostEstComp == 0)
                        //{
                        //    int sst = 9;
                        //}

                        //var dblCostExactArea = newcrg.dblCostExactArea;
                        //var dblCostExact = newcrg.dblCostExact;
                        //var d = newcrg.dblCostExact + newcrg.dblCostEst;
                        //Console.WriteLine("  GID: " + newcrg.GID + "    CphCount: " + cphcount +
                        //    "    EstType: " + dblCostEstType + "    EstComp: " + dblCostEstComp +
                        //    "    EstSum: " + newcrg.dblCostEst);

                        //if (outcrg.GID == 1682)
                        //{
                        //    Console.WriteLine("Replacing Region 1682");
                        //    foreach (var cpg in outcrg.CphCpgSD_Area_CphGID.Values)
                        //    {
                        //        Console.WriteLine(cpg.ID);
                        //    }
                        //}

                        Q.Add(newcrg);

                        //We also need to update ExistingCrgSDLt, otherwise we will have an error when Q.Remove(outcrg):
                        //we will take outcrg from ExistingCrgSDLt and then remove outcrg from Q.
                        //outcrg and newcrg are the same accoriding to the comparer of ExistingCrgSDLt,
                        //but they are different accoriding to the comparer of Q.
                        ExistingCrgSDLt[newcrg.GetCphCount()].Remove(outcrg);
                        ExistingCrgSDLt[newcrg.GetCphCount()].Add(newcrg, newcrg);                        
                    }
                    else
                    {
                        if (intEstSteps == 0)
                        {
                            throw new ArgumentException("We should be able to remove outcrg! We might be overestimating!");
                        }
                        else
                        {
                            // if intEstSteps != 0, we are overestimating, 
                            // outcrg may have been removed from queue Q as the node with least cost
                        }
                    }
                }
                else
                {
                    //we don't need to do operation Q.Add(newcrg);
                }
            }
            else
            {
                ComputeEstCost(lscrg, sscrg, newcrg,  padblTD, intEstSteps);

                Q.Add(newcrg);
                ExistingCrgSDLt[newcrg.GetCphCount()].Add(newcrg, newcrg);
                CRegion._intNodeCount++;
            }
            CRegion._intEdgeCount++;





            //the returned newcrg is just for counting, and thus not important
            return newcrg;
        }
        

        /// <summary>
        /// 
        /// </summary>
        /// <param name="activecph"></param>
        /// <param name="passivecph"></param>
        /// <param name="unitedcph"></param>
        /// <param name="SSCph"></param>
        /// <param name="NewCrg"></param>
        /// <param name="padblTD"></param>
        /// <remarks>if we change the cost method, then we will also need to 
        /// change the codes of InitialSubCostEstimated in CRegion.cs, the codes of updating existing outcrg </remarks>
        public void ComputeEstCost(CRegion lscrg, CRegion sscrg, CRegion NewCrg,  double[,] padblTD, int intEstSteps)
        {
            NewCrg.dblCostEstType = BalancedEstType(NewCrg, sscrg.GetSoloCphTypeIndex(), padblTD, intEstSteps);

            if (CConstants.strShapeConstraint == "MaximizeMinArea")
            {
                //NewCrg.dblCostEstArea = intEstSteps*EstimateSumMinArea(NewCrg);  //will we do this twice????

                //NewCrg.dblCostEst = NewCrg.dblCostEstType + NewCrg.dblCostEstArea;
            }
            else if (CConstants.strShapeConstraint == "MaxAvgC_EdgeNo")
            {
                NewCrg.dblCostEstComp = BalancedEstAvgComp_EdgeNumber(NewCrg, lscrg, sscrg, intEstSteps);

                //to make dblCostEstComp comparable to dblCostEstType and to avoid digital problems, we time dblCostEstComp by area
                //we will adjust the value later
                NewCrg.dblCostEst = (1 - CAreaAgg_Base.dblLamda) * NewCrg.dblCostEstType +
                    CAreaAgg_Base.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstComp;
            }
            else if (CConstants.strShapeConstraint == "MaxAvgC_Comb")
            {

            }
            else if (CConstants.strShapeConstraint == "MaxMinC_Comb")
            {
                //NewCrg.dblCostEstComp = intEstSteps * BalancedEstMinComp_Combine(NewCrg, lscrg, sscrg);

                ////to make dblCostEstComp comparable to dblCostEstType and to avoid digital problems, we time dblCostEstComp by area
                ////we will adjust the value later
                //NewCrg.dblCostEst = (1 - CAreaAgg_Base.dblLamda) * NewCrg.dblCostEstType +
                //    CAreaAgg_Base.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstComp;
            }
            else if (CConstants.strShapeConstraint == "MaxMinC_EdgeNo")
            {
                //NewCrg.dblCostEstComp = intEstSteps * BalancedEstMinComp_EdgeNumber(NewCrg, lscrg, sscrg);

                ////to make dblCostEstComp comparable to dblCostEstType and to avoid digital problems, we time dblCostEstComp by area
                ////we will adjust the value later
                //NewCrg.dblCostEst = (1 - CAreaAgg_Base.dblLamda) * NewCrg.dblCostEstType +
                //    CAreaAgg_Base.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstComp;
            }
            else if (CConstants.strShapeConstraint == "MinIntBound")
            {
                NewCrg.dblCostEstComp = BalancedEstCompInteriorLength_Basic(NewCrg, lscrg, intEstSteps);

                //to make dblCostEstComp comparable to dblCostEstType and to avoid digital problems, we time dblCostEstComp by area
                //we will adjust the value later
                NewCrg.dblCostEst = (1 - CAreaAgg_Base.dblLamda) * NewCrg.dblCostEstType +
                    CAreaAgg_Base.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstComp;
            }
            else if (CConstants.strShapeConstraint == "NonShape")
            {
                NewCrg.dblCostEst = NewCrg.dblCostEstType;
            }

            //double dblWeight = 0.5;
            NewCrg.d = NewCrg.dblCostExact + NewCrg.dblCostEst;
        }



        private double BalancedEstType(CRegion crg, int intFinalTypeIndex, double[,] padblTD, int intEstSteps)
        {
            int intOverestimationCount = Math.Min(intEstSteps, crg.GetCphCount());
            var pCphCpgEt = crg.CphCpgSD_Area_CphGID.GetEnumerator();
            double dblCostEst = 0;
            for (int i = 0; i < intOverestimationCount; i++)
            {
                pCphCpgEt.MoveNext();

                dblCostEst += pCphCpgEt.Current.Key.dblArea *
                    padblTD[pCphCpgEt.Current.Value.intTypeIndex, intFinalTypeIndex] * intEstSteps;  //overestimation         
            }

            for (int i = intOverestimationCount; i < crg.GetCphCount(); i++)
            {
                pCphCpgEt.MoveNext();
                dblCostEst += pCphCpgEt.Current.Key.dblArea *
                    padblTD[pCphCpgEt.Current.Value.intTypeIndex, intFinalTypeIndex];  //estimation
            }

            return dblCostEst;
        }



        /// <summary>
        /// from time t to n-1
        /// </summary>
        /// <param name="crg"></param>
        /// <returns></returns>
        private IEnumerable<double> EstimateAvgComp_EdgeNumber(CRegion crg, CRegion lscrg, int intEstSteps)
        {
            double dblSumComp = 0;
            var dblCompSS = new SortedSet<CValPair<double, int>>();
            int intCompCount = 0;
            foreach (var cph in crg.GetCphCol())
            {
                dblSumComp += cph.dblComp;
                dblCompSS.Add(new CValPair<double, int>(cph.dblComp, intCompCount++));
            }

            var intEdgeCountSS = new SortedSet<int>(new CIntCompare());
            foreach (var pCorrCphs in crg.AdjCorrCphsSD.Keys)
            {
                intEdgeCountSS.Add(pCorrCphs.intSharedCEdgeCount);
            }

            int intEdgeCountRemain = crg.intExteriorEdgeCount + crg.intInteriorEdgeCount;
            int intCphCount = crg.GetCphCount();

            IEnumerator<int> intEdgeCountSSEt = intEdgeCountSS.GetEnumerator();
            int intEstCount = 0;
            while (intCphCount > 1)
            {
                //if intEstCount == lscrg.GetCphCount(), we are estimating from the start map (t==1)
                //we define that the estimation value of the start map is 0, therefore we skip intEstCount == lscrg.GetCphCount()
                if (intCphCount < lscrg.GetCphCount())
                {
                    if (intEstCount < intEstSteps)
                    {
                        yield return 0;  //overestimation
                        intEstCount++;
                    }
                    else
                    {
                        double dblAvgComp = dblSumComp / intCphCount;
                        yield return dblAvgComp;  //normal estimation
                    }
                }

                //remove the two smallest compactnesses
                for (int i = 0; i < 2; i++)
                {
                    dblSumComp -= dblCompSS.Min().val1;
                    if (dblCompSS.Remove(dblCompSS.Min()) == false)
                    {
                        throw new ArgumentException("failed to remove the smallest element!");
                    }
                }

                intEdgeCountSSEt.MoveNext();
                intEdgeCountRemain -= intEdgeCountSSEt.Current;

                //double dblNewComp = 0;
                //if (intEstCount < intEstSteps)
                //{
                //    dblNewComp = 0;  //overestimation
                //    intEstCount++;
                //}
                //else
                //{
                //    dblNewComp = CGeoFunc.CalCompRegularPolygon(intEdgeCountRemain); //normal estimation
                //}

                double dblNewComp = CGeoFunc.CalCompRegularPolygon(intEdgeCountRemain); //normal estimation

                dblCompSS.Add(new CValPair<double, int>(dblNewComp, intCompCount++));
                dblSumComp += dblNewComp;
                intCphCount--;
            }
        }



        #region BalancedEstCompInteriorLength_Basic
        private double BalancedEstCompInteriorLength_Basic(CRegion crg, CRegion lscrg, int intEstSteps)
        {
            // if lscrg.GetCphCount() <= 2, the domain only have two polygons
            // A value will be divided by (lscrg.GetCphCount() - 2). To avoid being divided by 0, we directly return 0.
            if (lscrg.GetCphCount() <= 2 || crg.GetCphCount() <= 1)
            {
                return 0;
            }

            int intOverestimationCount = Math.Min(intEstSteps, crg.GetCphCount() - 2);

            var EstimateLt = EstimateInteriorLength(crg, lscrg);
            double dblSum = crg.dblInteriorSegLength / (crg.GetCphCount() - 1);  //n-s= crg.GetCphCount()-1
            for (int i = 0; i < intOverestimationCount; i++)
            {
                dblSum += crg.dblInteriorSegLength / (crg.GetCphCount() - (i + 2));  //overestimation; n-s= crg.GetCphCount()-(i+2)
            }
            for (int i = 0; i < EstimateLt.Count - intOverestimationCount; i++)
            {
                dblSum += (EstimateLt[i] / (i + 1));  //normal estimation; n-s = i+1= crg.GetCphCount()-1
            }

            double dblEstComp = dblSum / (lscrg.dblInteriorSegLength / (lscrg.GetCphCount() - 1)) / (lscrg.GetCphCount() - 2);

            if (dblEstComp == 0)
            {
                throw new ArgumentException("impossible!");
            }

            return dblEstComp;
        }

        /// <summary>
        /// Estimate Interior Lengths for the future steps, without current step
        /// </summary>
        /// <param name="crg"></param>
        /// <param name="lscrg"></param>
        /// <returns></returns>
        /// <remarks>from time n-1 to t+1</remarks>
        private List<double> EstimateInteriorLength(CRegion crg, CRegion lscrg)
        {
            var dblSegLengthSS = new SortedSet<double>(new CCmpDbl());
            foreach (var pCorrCphs in crg.AdjCorrCphsSD.Keys)
            {
                dblSegLengthSS.Add(pCorrCphs.dblSharedSegLength);
            }
            IEnumerator<double> dblSegLengthSSEt = dblSegLengthSS.GetEnumerator();


            int intEstCount = crg.GetCphCount();
            //if intEstCount == lscrg.GetCphCount(), we are estimating from the start map (t==1)
            //we define that the estimation value of the start map is 0, therefore we skip intEstCount == lscrg.GetCphCount()
            if (crg.GetCphCount() == lscrg.GetCphCount())
            {
                intEstCount--;
            }

            //from time n-1 to t+1
            //we compute from the last step (only one interior boundary) to the current step (many interior boundaries).             
            double dblInteriorLength = 0;
            var dblInteriorLengthLt = new List<double>(intEstCount - 1);
            for (int i = 0; i < intEstCount - 2; i++)
            {
                dblSegLengthSSEt.MoveNext();
                dblInteriorLength += dblSegLengthSSEt.Current;

                dblInteriorLengthLt.Add(dblInteriorLength);
            }

            return dblInteriorLengthLt;

            //for (int i = 0; i < dblInteriorLengthLt.Count; i++)
            //{
            //    yield return dblInteriorLengthLt[dblInteriorLengthLt.Count - i - 1];
            //}
        }


        #endregion











        /// <summary>
        /// currently doesn't work
        /// </summary>
        /// <param name="crg"></param>
        /// <param name="intTimeNum"></param>
        /// <returns></returns>
        /// <remarks>we need to improve this estimation to make sure this is an upper bound.
        /// We don't need to compute one step further because the estimation based on edge number will "never" be 0</remarks>
        /// 
        private double BalancedEstMinComp_Combine(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            if (crg.GetCphCount() == 1)
            {
                return 0;
            }

            var EstEdgeNumberEt = EstimateMinComp_EdgeNumber(crg, lscrg).GetEnumerator();
            //we need to improve this estimation to make sure this is an upper bound
            var EstEdgeLengthEt = EstimateMinComp_EdgeLength(crg, lscrg).GetEnumerator();

            double dblSumCompValue = 0;
            while (EstEdgeNumberEt.MoveNext() && EstEdgeLengthEt.MoveNext())
            {
                if (EstEdgeNumberEt.Current < EstEdgeLengthEt.Current)
                {
                    CRegion._lngEstCountEdgeNumber++;
                }
                else if (EstEdgeNumberEt.Current > EstEdgeLengthEt.Current)
                {
                    CRegion._lngEstCountEdgeLength++;
                }
                else
                {
                    CRegion._lngEstCountEqual++;
                }

                dblSumCompValue += (1 - Math.Min(EstEdgeNumberEt.Current, EstEdgeLengthEt.Current));
            }

            return dblSumCompValue / (lscrg.GetCphCount() - 1);
        }

        #region EstimateAvgComp_EdgeNumber
        /// <summary>
        /// 
        /// </summary>
        /// <param name="crg">crg.GetCphCount() > 1</param>
        /// <param name="lscrg">lscrg.GetCphCount() > 2</param>
        /// <param name="sscrg"></param>
        /// <returns></returns>
        private double BalancedEstAvgComp_EdgeNumber(CRegion crg, CRegion lscrg, CRegion sscrg, int intEstSteps)
        {
            // if lscrg.GetCphCount() <= 2, the domain only have two polygons
            // A value will be divided by (lscrg.GetCphCount() - 2). To avoid being divided by 0, we directly return 0.
            if (lscrg.GetCphCount() <= 2 || crg.GetCphCount() <= 1)
            {
                return 0;
            }

            return EstimatedComp_Common(EstimateAvgComp_EdgeNumber(crg, lscrg, intEstSteps), crg, lscrg, sscrg);
        }


        


        #endregion

        #region EstimateMinComp_EdgeNumber
        /// <summary>
        /// 
        /// </summary>
        /// <param name="crg">crg.GetCphCount() > 1</param>
        /// <param name="lscrg">lscrg.GetCphCount() > 2</param>
        /// <param name="sscrg"></param>
        /// <returns></returns>
        private double BalancedEstMinComp_EdgeNumber(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            // if lscrg.GetCphCount() <= 2, the domain only have two polygons
            // A value will be divided by (lscrg.GetCphCount() - 2). To avoid being divided by 0, we directly return 0.
            if (lscrg.GetCphCount() <= 2)
            {
                return 0;
            }

            return EstimatedComp_Common(EstimateMinComp_EdgeNumber(crg, lscrg), crg, lscrg, sscrg);
        }

        private double EstimatedComp_Common(IEnumerable<double> EstimateEb, CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            var EstimateEt = EstimateEb.GetEnumerator();

            //we include current step so that we can make the ration type/comp
            double dblSumCompValue = 0;  //for the current crg, whose dblMinComp is known
            while (EstimateEt.MoveNext())
            {
                dblSumCompValue += (1 - EstimateEt.Current);
            }

            return dblSumCompValue / (lscrg.GetCphCount() - 2);
        }

        /// <summary>
        /// from time t to n-1
        /// </summary>
        /// <param name="crg"></param>
        /// <returns></returns>
        private IEnumerable<double> EstimateMinComp_EdgeNumber(CRegion crg, CRegion lscrg)
        {
            var intEdgeCountSS = new SortedSet<int>(new CIntCompare());
            foreach (var pCorrCphs in crg.AdjCorrCphsSD.Keys)
            {
                intEdgeCountSS.Add(pCorrCphs.intSharedCEdgeCount);
            }

            IEnumerator<int> intEdgeCountSSEt = intEdgeCountSS.GetEnumerator();
            int intEdgeCountAtmost = crg.intExteriorEdgeCount + 2 * crg.intInteriorEdgeCount;
            int intEstCount = crg.GetCphCount();

            while (intEstCount > 1)
            {
                intEdgeCountSSEt.MoveNext();

                //if intEstCount == lscrg.GetCphCount(), we are estimating from the start map (t==1)
                //we define that the estimation value of the start map is 0, therefore we skip intEstCount == lscrg.GetCphCount()
                if (intEstCount < lscrg.GetCphCount())
                {
                    int intAverageEdgeCount = Convert.ToInt32(
                        Math.Floor(Convert.ToDouble(intEdgeCountAtmost) / Convert.ToDouble(intEstCount)));
                    yield return CGeoFunc.CalCompRegularPolygon(intAverageEdgeCount);
                }

                intEdgeCountAtmost -= (2 * intEdgeCountSSEt.Current);
                intEstCount--;
            }
        }


        #endregion

        #region EstimateMinComp_EdgeLength
        private double BalancedEstMinComp_EdgeLength(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            if (crg.GetCphCount() == 1)
            {
                return 0;
            }

            return EstimatedComp_Common(EstimateMinComp_EdgeLength(crg, lscrg), crg, lscrg, sscrg);
        }

        private IEnumerable<double> EstimateMinComp_EdgeLength(CRegion crg, CRegion lscrg)
        {
            throw new ArgumentException("We need to update the cost functions!");

            //double dblEstComp
            var EstimateEt = EstimateInteriorLength(crg, lscrg).GetEnumerator();
            int intEstCount = crg.GetCphCount() - 1;
            while (EstimateEt.MoveNext())
            {
                double dblEdgeLength = crg.dblExteriorSegLength + 2 * EstimateEt.Current;
                yield return EstimateCompEdgeLengthOneCrgInstance(intEstCount--, crg.dblArea, dblEdgeLength);
            }
        }

        private double EstimateCompEdgeLengthOneCrgInstance(int intPatchNum, double dblArea, double dblLength)
        {
            double dblEstComp = 2 * Math.Sqrt(Math.PI * intPatchNum * dblArea) / dblLength;

            if (dblEstComp > 1)
            {
                return 1;
            }
            return dblEstComp;
        }
        #endregion

       













        private void AddVirtualItem(List<List<object>> pobjDataLtLt, CRegion LSCrg, string strAreaAggregation, int intCount)
        {
            List<object> objDataLt = new List<object>(14);
            objDataLt.Add(LSCrg.ID);
            objDataLt.Add(LSCrg.GetCphCount());
            objDataLt.Add(LSCrg.GetAdjCount());
            objDataLt.Add(strAreaAggregation);
            for (int i = 4; i < intCount; i++)
            {
                objDataLt.Add(-1);
            }
            pobjDataLtLt.Add(objDataLt);
        }
        #endregion

        

        


    }
}
