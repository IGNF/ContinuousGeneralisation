using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CGeneralizationMethods;

namespace MorphingClass.CGeometry
{
    public class CRegion : CBasicBase
    {
        public static CRegion _pCrg;

        public static int _intNodeCount;  //The nodes of a map graph
        public static int _intEdgeCount;  //The edges of a map graph
        public static int _intStartStaticGIDLast;
        public static int _intStartStaticGIDAll;
        public static int _intStaticGID;
        public static int _intStaticTest;

        //public LinkedList<int> _IDLk;  //ID of the Region, where the ID is a LinkedList
        public static CCmpCRegion_Cost_CphGIDTypeIndex pCmpCRegion_Cost_CphGIDTypeIndex = new CCmpCRegion_Cost_CphGIDTypeIndex();  //this variable should be used for the queue Q

        //this comparer should be used for integrate the sequences for output 
        public static CCmpCRegion_MinArea_CphGIDTypeIndex pCmpCRegion_MinArea_CphGIDTypeIndex = new CCmpCRegion_MinArea_CphGIDTypeIndex();

        //this comparer should be used for integrate the sequences for output 
        public static CCmpCRegion_CostExact_CphGIDTypeIndex pCmpCRegion_CostExact_CphGIDTypeIndex = new CCmpCRegion_CostExact_CphGIDTypeIndex();

        //this comparer should be used for checking existing Crgs
        public static CCmpCRegion_CphGIDTypeIndex pCmpCRegion_CphGIDTypeIndex = new CCmpCRegion_CphGIDTypeIndex();

        //this comparer should be used for counting uncolored Crgs
        public static CCmpCRegion_CphGID pCmpCRegion_CphGID = new CCmpCRegion_CphGID();  //this variable should be used for CRegion itself

        //public static CCmpCRegion_CompareDblPreLocateEqual pCmpCRegion_CompareDblPreLocateEqual = new CCmpCRegion_CompareDblPreLocateEqual();
        //public static 

        public SortedDictionary<CCorrCphs, CCorrCphs> AdjCorrCphsSD { get; set; }  //compare GID of CorrCphs

        public SortedDictionary<CPatch, int> CphTypeIndexSD_Area_CphGID { get; set; }  //Why did I use SortedDictionary? We use this comparator CPatch .pCmpCPatch_Area_CphGID
        //public SortedSet<CPatch> CphSS_Compactness_CphGID { get; set; }  //Why did I use SortedDictionary? We use this comparator CPatch .pCmpCPatch_Area_CphGID

        public static long _lngEstCountEdgeNumber;
        public static long _lngEstCountEdgeLength;
        public static long _lngEstCountEqual;

        //public SortedSet<CPatch> CphAreaSS { get; set; }
        public int intSumCphGID { get; set; }
        public int intSumTypeIndex { get; set; }
        //public int intEdgeCount { get; set; }
        public int intInteriorEdgeCount { get; set; }
        public int intExteriorEdgeCount { get; set; }
        public double dblInteriorSegLength { get; set; }
        public double dblExteriorSegLength { get; set; }

        /// <summary>this is the real compactness 2*Sqrt(pi*A)/L.</summary>
        public double dblMinComp { get; set; }
        public double dblAvgComp { get; set; }

        //private double _dbld = double.MaxValue;
        public double d { get; set; }

        public CRegion parent { get; set; }
        public CRegion child { get; set; }
        public CEnumColor cenumColor { get; set; }
        public CValTri<CPatch, CPatch, CPatch> AggedCphs { get; set; }


        public double dblArea { get; set; }
        public double dblSumComp { get; set; }

        //public double dblCostExact { get; set; }
        //public double dblCostEst { get; set; }
        //public double dblCostExactType { get; set; }
        //public double dblCostEstType { get; set; }
        //public double _dblCostEstType;
        //public double dblCostExactComp { get; set; }
        //public double dblCostEstComp { get; set; }
        public double dblCostExactArea { get; set; }
        public double dblCostEstArea { get; set; }

        public double _dblCostEstType = 0;
        public double dblCostEstType
        {
            get { return _dblCostEstType; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.pCmpCoordDbl_VerySmall);
                _dblCostEstType = value;
            }
        }

        public double _dblCostEstComp = 0;
        public double dblCostEstComp
        {
            get { return _dblCostEstComp; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.pCmpCoordDbl_VerySmall);
                _dblCostEstComp = value;
            }
        }

        public double _dblCostExactType = 0;
        public double dblCostExactType
        {
            get { return _dblCostExactType; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.pCmpCoordDbl_VerySmall);
                _dblCostExactType = value;
            }
        }

        public double _dblCostExactComp = 0;
        public double dblCostExactComp
        {
            get { return _dblCostExactComp; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.pCmpCoordDbl_VerySmall);
                _dblCostExactComp = value;
            }
        }

        private double _dblCostExact = 0;
        public double dblCostExact
        {
            get { return _dblCostExact; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.pCmpCoordDbl_VerySmall);
                _dblCostExact = value;
            }
        }

        private double _dblCostEst = 0;
        public double dblCostEst
        {
            get { return _dblCostEst; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.pCmpCoordDbl_VerySmall);
                _dblCostEst = value;
            }
        }



        //public double dblCostEstType
        //{
        //    get { return _dblCostEstType; }
        //    set
        //    {
        //        if (value > 2*1712083)
        //        {
        //            throw new ArgumentException("incorrect value!");
        //        }

        //        _dblCostEstType = value;
        //    }
        //}

        public CRegion()
            : this(-1)
        {
        }

        //public CRegion(int intID)
        //    : this(intID, new SortedDictionary<CPatch, int>(CPatch.pCmpCPatch_CpgGID))
        //{
        //}


        //public CRegion(int intID, SortedDictionary<CPatch, int> pCphTypeIndexSD)
        //{
        //    this.ID = intID;
        //    this.CphTypeIndexSD = new SortedDictionary<CPatch, int>(pCphTypeIndexSD, CPatch.pCmpCPatch_CpgGID);
        //    this.d = double.MaxValue;
        //    this.parent = null;
        //    this.cenumColor = CEnumColor.white;
        //}

        //public CRegion(int intID, string strShapeConstraint)
        public CRegion(int intID)
        {
            this.ID = intID;
            this.GID = _intStaticGID++;
            this.CphTypeIndexSD_Area_CphGID = new SortedDictionary<CPatch, int>(CPatch.pCmpCPatch_Area_CphGID);


            ////intID==-2 is for a temporary Crg, and thus should not be counted
            //if (intID==-2)
            //{
            //    _intStaticGID--;
            //}
            //this.d = double.MaxValue;

            this.parent = null;
            this.cenumColor = CEnumColor.white;
        }

        public ICollection<CPatch> GetCphCol()
        {
            return this.CphTypeIndexSD_Area_CphGID.Keys;
        }

        public ICollection<int> GetCphTypeIndexCol()
        {
            return this.CphTypeIndexSD_Area_CphGID.Values;
        }

        public int GetCphTypeIndex(CPatch cph)
        {
            int intTypeIndex;
            if (this.CphTypeIndexSD_Area_CphGID.TryGetValue(cph, out intTypeIndex) == false)
            {
                throw new ArgumentNullException("The patch does not exist!");
            }
            return intTypeIndex;
        }

        public int GetCphCount()
        {
            return this.CphTypeIndexSD_Area_CphGID.Count;
        }

        public int GetAdjCount()
        {
            return this.AdjCorrCphsSD.Count;
        }

        public CPatch GetSmallestCph()
        {
            return this.CphTypeIndexSD_Area_CphGID.GetFirstT().Key;
        }

        public int GetSoloCphTypeIndex()
        {
            if (this.CphTypeIndexSD_Area_CphGID.Count > 1)
            {
                throw new ArgumentOutOfRangeException("There are more than one elements!");
            }

            return this.CphTypeIndexSD_Area_CphGID.GetFirstT().Value;
        }



        public IEnumerable<CCphRecord> GetNeighborCphRecords(CPatch cph)
        {
            foreach (var pCorrCphs in this.AdjCorrCphsSD.Keys)
            {
                var neighborcph = TryGetNeighbor(cph, pCorrCphs);
                if (neighborcph != null)
                {
                    yield return new CCphRecord(neighborcph, pCorrCphs);
                }
            }
        }

        public void AddCph(CPatch Cph, int pintTypeIndex)
        {
            this.CphTypeIndexSD_Area_CphGID.Add(Cph, pintTypeIndex);

            this.intSumCphGID += Cph.GID;
            this.intSumTypeIndex += pintTypeIndex;
            this.dblArea += Cph.dblArea;


        }

        public double ComputeMinComp()
        {
            this.dblMinComp = this.GetCphCol().Min(cph => cph.dblComp);
            return this.dblMinComp;
        }



        public void SetCoreCph(int intTypeIndex)
        {
            CPatch CoreCph = new CPatch();
            foreach (var kvp in this.CphTypeIndexSD_Area_CphGID)
            {
                if (kvp.Key.dblArea > CoreCph.dblArea && kvp.Value == intTypeIndex)
                {
                    CoreCph = kvp.Key;
                }
            }

            if (CoreCph.dblArea == 0)
            {
                throw new ArgumentException("Either no CoreCph or Cph without area!");
            }
            CoreCph.isCore = true;
        }

        //public void AddCph(SortedDictionary<CPatch, int> pCphTypeIndexSD)
        //{
        //    foreach (var pCphTypeIndex in pCphTypeIndexSD)
        //    {
        //        this.CphTypeIndexSD.Add(pCphTypeIndex.Key, pCphTypeIndex.Value);
        //    }
        //}



        /// <summary>
        /// Get the adjacnet relationships between CPatches
        /// </summary>
        /// <param name="cphlt"></param>
        public SortedDictionary<CCorrCphs, CCorrCphs> SetInitialAdjacency()
        {
            var ExistingCorrCphsSD0 = new SortedDictionary<CCorrCphs, CCorrCphs>(CCorrCphs.pCmpCCorrCphs_CphsGID); // we need this variable here, because it has different comparator with pAdjCorrCphsSD
            SortedDictionary<CEdge, CPatch> cedgeSD = new SortedDictionary<CEdge, CPatch>(new CCmpCEdgeCoordinates());  //why SortedDictionary? Because we want to get the value of an element. The element may have the same key with another element.
            var pAdjCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>();

            if (this.GetCphCount() > 1)
            {
                foreach (var cph in this.GetCphCol())
                {
                    var cpgSK = new Stack<CPolygon>();
                    cpgSK.Push(cph.GetSoloCpg());

                    do
                    {
                        var cpg = cpgSK.Pop();
                        foreach (CEdge cedge in cpg.CEdgeLt)  //Note that there is only one element in cph.CpgSS
                        {
                            //cedge.PrintMySelf();

                            CPatch EdgeShareCph;
                            bool isShareEdge = cedgeSD.TryGetValue(cedge, out EdgeShareCph);
                            if (isShareEdge == true)  //if cedge already exists in cedgeSD, then we now have found the Patch which shares this cedge
                            {
                                CCorrCphs ExsitedCorrCphs;   //SharedEdgesLk belongs to cph.AdjacentSD
                                var CorrCphs = new CCorrCphs(cph, EdgeShareCph);

                                bool isKnownAdjacent = ExistingCorrCphsSD0.TryGetValue(CorrCphs, out ExsitedCorrCphs);  //whether we have already known that cph and EdgeShareCph are adjacent patches
                                if (isKnownAdjacent == true)
                                {
                                    ExsitedCorrCphs.SharedCEdgeLt.Add(cedge);
                                    ExsitedCorrCphs.dblSharedSegLength += cedge.dblLength;
                                    ExsitedCorrCphs.intSharedCEdgeCount++;
                                }
                                else
                                {
                                    //List<CEdge> NewCphSharedEdgesLt = new List<CEdge>(1);
                                    //CorrCphs.SharedCEdgeLt = new List<CEdge>(1);
                                    CorrCphs.SharedCEdgeLt.Add(cedge);
                                    CorrCphs.dblSharedSegLength += cedge.dblLength;
                                    CorrCphs.intSharedCEdgeCount++;

                                    pAdjCorrCphsSD.Add(CorrCphs, CorrCphs);
                                    ExistingCorrCphsSD0.Add(CorrCphs, CorrCphs);
                                }

                                this.intInteriorEdgeCount++;
                                this.dblInteriorSegLength += cedge.dblLength;
                                cedgeSD.Remove(cedge);    //every edge belongs to two polygons, if we have found the two polygons, we can remove the shared edge from the SortedDictionary
                            }
                            else  //if cedge doesn't exist in cedgeSD, then we add this cedge
                            {
                                cedgeSD.Add(cedge, cph);
                                //this.intEdgeCount++;
                            }
                        }

                        if (cpg.HoleCpgLt != null)
                        {
                            foreach (var holecpg in cpg.HoleCpgLt)
                            {
                                cpgSK.Push(holecpg);
                            }
                        }
                    } while (cpgSK.Count > 0);
                }
            }

            this.intExteriorEdgeCount = cedgeSD.Count;
            foreach (var cedgekvp in cedgeSD)
            {
                this.dblExteriorSegLength += cedgekvp.Key.dblLength;
            }
            //******************************************************************************************************//
            //to use less memory, we don't save the shared edgelist. in the method of MaximizeMinArea, we don't need the shared edgelist.
            foreach (var item in pAdjCorrCphsSD)
            {
                item.Value.SharedCEdgeLt.Clear();
            }
            //******************************************************************************************************//
            foreach (var cphkvp in CphTypeIndexSD_Area_CphGID)
            {
                cphkvp.Key.AdjacentCphSS = new SortedSet<CPatch>();
            }
            foreach (var pAdjacency_CorrCphs in pAdjCorrCphsSD.Keys)
            {
                pAdjacency_CorrCphs.FrCph.AdjacentCphSS.Add(pAdjacency_CorrCphs.ToCph);
                pAdjacency_CorrCphs.ToCph.AdjacentCphSS.Add(pAdjacency_CorrCphs.FrCph);
            }

            if (CConstants.blnComputeMinComp == true)
            {
                this.ComputeMinComp();
            }
            else if (CConstants.blnComputeAvgComp == true)
            {
                foreach (var cph in this.GetCphCol())
                {
                    this.dblSumComp += cph.dblComp;
                }

                this.dblAvgComp = this.dblSumComp / this.GetCphCount();
            }

            this.AdjCorrCphsSD = pAdjCorrCphsSD;
            return ExistingCorrCphsSD0;
        }

        public IEnumerable<CRegion> AggregateAndUpdateQ(CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q, string strAreaAggregation,
            List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, int intFinalTypeIndex, double[,] padblTD, int intFactor)
        {
            if (strAreaAggregation == "Smallest")
            {
                foreach (var item in AggregateSmallestAndUpdateQ(lscrg, sscrg, Q, ExistingCrgSDLt, ExistingCphSDLt, ExistingCorrCphsSD,
                    intFinalTypeIndex, padblTD, intFactor))
                {
                    yield return item;
                }
            }
            else
            {
                foreach (var item in AggregateAllAndUpdateQ(lscrg, sscrg, Q, ExistingCrgSDLt, ExistingCphSDLt, ExistingCorrCphsSD,
                    intFinalTypeIndex, padblTD, intFactor))
                {
                    yield return item;
                }
            }
        }


        public IEnumerable<CRegion> AggregateSmallestAndUpdateQ(CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q,
            List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, int intFinalTypeIndex, double[,] padblTD, int intFactor)
        {
            var pAdjCorrCphsSD = this.AdjCorrCphsSD;
            var mincph = this.GetSmallestCph();

            //for every pair of neighboring Cphs, we aggregate them and generate a new Crg
            foreach (var pCphRecord in this.GetNeighborCphRecords(mincph))
            {
                foreach (var item in AggregateAndUpdateQ_Common(lscrg, sscrg, Q, pAdjCorrCphsSD, pCphRecord.CorrCphs, ExistingCrgSDLt,
                    ExistingCphSDLt, ExistingCorrCphsSD, intFinalTypeIndex, padblTD, intFactor))
                {
                    yield return item;
                }
            }
        }


        public IEnumerable<CRegion> AggregateAllAndUpdateQ(CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q,
            List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, int intFinalTypeIndex, double[,] padblTD, int intFactor)
        {
            var pAdjCorrCphsSD = this.AdjCorrCphsSD;

            //for every pair of neighboring Cphs, we aggregate them and generate a new Crg
            foreach (var unitingCorrCphs in pAdjCorrCphsSD.Keys)
            {
                foreach (var item in AggregateAndUpdateQ_Common(lscrg, sscrg, Q, pAdjCorrCphsSD, unitingCorrCphs, ExistingCrgSDLt,
                    ExistingCphSDLt, ExistingCorrCphsSD, intFinalTypeIndex, padblTD, intFactor))
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<CRegion> AggregateAndUpdateQ_Common(CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q, SortedDictionary<CCorrCphs,
            CCorrCphs> pAdjCorrCphsSD, CCorrCphs unitingCorrCphs, List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt,
            List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt, SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD,
            int intFinalTypeIndex, double[,] padblTD, int intFactor)
        {

            var newcph = ComputeNewCph(unitingCorrCphs, ExistingCphSDLt);
            var newAdjCorrCphsSD = ComputeNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD(pAdjCorrCphsSD, unitingCorrCphs, newcph, ExistingCorrCphsSD);


            var frcph = unitingCorrCphs.FrCph;
            var tocph = unitingCorrCphs.ToCph;
            int intfrTypeIndex = this.GetCphTypeIndex(frcph);
            int inttoTypeIndex = this.GetCphTypeIndex(tocph);


            //if the two cphs have the same type, then we only need to aggregate the smaller one into the larger one (this will certainly have smaller cost in terms of area)
            //otherwise, we need to aggregate from two directions
            if (intfrTypeIndex == inttoTypeIndex)
            {
                if (frcph.dblArea>= tocph.dblArea)
                {
                    yield return GenerateCrgAndUpdateQ(lscrg, sscrg, Q, ExistingCrgSDLt, newAdjCorrCphsSD, 
                        frcph, tocph, newcph, unitingCorrCphs, intFinalTypeIndex, padblTD, intFactor);
                }
                else
                {
                    yield return GenerateCrgAndUpdateQ(lscrg, sscrg, Q, ExistingCrgSDLt, newAdjCorrCphsSD,
                        tocph, frcph, newcph, unitingCorrCphs, intFinalTypeIndex, padblTD, intFactor);
                }                
            }
            else
            {
                //The aggregate can happen from two directions
                yield return GenerateCrgAndUpdateQ(lscrg, sscrg, Q, ExistingCrgSDLt, newAdjCorrCphsSD, frcph, tocph, newcph, unitingCorrCphs,
                    intFinalTypeIndex, padblTD, intFactor);
                yield return GenerateCrgAndUpdateQ(lscrg, sscrg, Q, ExistingCrgSDLt, newAdjCorrCphsSD, tocph, frcph, newcph, unitingCorrCphs,
                    intFinalTypeIndex, padblTD, intFactor);
            }
        }

        public static CPatch TryGetNeighbor(CPatch cph, CCorrCphs unitingCorrCphs)
        {
            if (cph.CompareTo(unitingCorrCphs.FrCph) == 0)
            {
                return unitingCorrCphs.ToCph;
            }
            else if (cph.CompareTo(unitingCorrCphs.ToCph) == 0)
            {
                return unitingCorrCphs.FrCph;
            }
            else
            {
                return null;
            }
        }

        private CPatch ComputeNewCph(CCorrCphs unitingCorrCphs, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt)
        {
            var newcph = unitingCorrCphs.FrCph.Unite(unitingCorrCphs.ToCph, unitingCorrCphs.dblSharedSegLength);

            //test whether this newcph has already been constructed before
            CPatch outcph;
            if (ExistingCphSDLt[newcph.CpgSS.Count].TryGetValue(newcph, out outcph))
            {
                newcph = outcph;
            }
            else
            {
                ExistingCphSDLt[newcph.CpgSS.Count].Add(newcph, newcph);
            }
            return newcph;
        }


        #region ComputeNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD
        public static SortedDictionary<CCorrCphs, CCorrCphs> ComputeNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD(
            SortedDictionary<CCorrCphs, CCorrCphs> pAdjCorrCphsSD,
            CCorrCphs unitingCorrCphs, CPatch newcph, SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD)
        {
            List<CCorrCphs> AddKeyLt;
            var pNewAdjCorrCphsLt_WithoutReplaced =
                ComputeNewAdjCorrCphsSD_WithoutReplaced(pAdjCorrCphsSD, unitingCorrCphs, newcph, out AddKeyLt);  //make a copy
            var incrementalAdjCorrCphsSD = ComputeIncrementalAdjCorrCphsSD(AddKeyLt, ExistingCorrCphsSD);

            return GenerateNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD
                (pNewAdjCorrCphsLt_WithoutReplaced, ExistingCorrCphsSD, incrementalAdjCorrCphsSD);
        }

        private static List<CCorrCphs> ComputeNewAdjCorrCphsSD_WithoutReplaced(SortedDictionary<CCorrCphs, CCorrCphs> pAdjCorrCphsSD,
            CCorrCphs unitingCorrCphs, CPatch newcph, out List<CCorrCphs> AddKeyLt)
        {
            var newAdjCorrCphsLt = new List<CCorrCphs>(pAdjCorrCphsSD.Count);
            AddKeyLt = new List<CCorrCphs>();
            foreach (var pCorrCphs in pAdjCorrCphsSD.Keys)
            {
                if (pCorrCphs.CompareTo(unitingCorrCphs) == 0)
                {
                    continue;
                }
                else if (pCorrCphs.FrCph.CompareTo(unitingCorrCphs.FrCph) == 0 || pCorrCphs.FrCph.CompareTo(unitingCorrCphs.ToCph) == 0)
                {
                    AddKeyLt.Add(new CCorrCphs(pCorrCphs.ToCph, newcph, pCorrCphs));
                }
                else if (pCorrCphs.ToCph.CompareTo(unitingCorrCphs.FrCph) == 0 || pCorrCphs.ToCph.CompareTo(unitingCorrCphs.ToCph) == 0)
                {
                    AddKeyLt.Add(new CCorrCphs(pCorrCphs.FrCph, newcph, pCorrCphs));
                }
                else
                {
                    newAdjCorrCphsLt.Add(pCorrCphs);
                }
            }

            return newAdjCorrCphsLt;
        }

        private static SortedDictionary<CCorrCphs, CCorrCphs> ComputeIncrementalAdjCorrCphsSD(List<CCorrCphs> AddKeyLt,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD)
        {
            var incrementalAdjCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>(CCorrCphs.pCmpCCorrCphs_CphsGID);

            foreach (var AddKey in AddKeyLt)
            {
                CCorrCphs AddKeyCorrCphs;
                bool isExisting = ExistingCorrCphsSD.TryGetValue(AddKey, out AddKeyCorrCphs);


                CCorrCphs newAdjCorrCphs;
                bool isAdjExisting = incrementalAdjCorrCphsSD.TryGetValue(AddKey, out newAdjCorrCphs);

                if (isExisting == true && isAdjExisting == true)
                {
                    //do nothing; this situation can happen
                }
                else if (isExisting == true && isAdjExisting == false)
                {
                    incrementalAdjCorrCphsSD.Add(AddKeyCorrCphs, AddKeyCorrCphs);
                }
                else if (isExisting == false && isAdjExisting == true) //AddKeyLt[i] already exists. this means that AddKeyLt[i].FrCph was adjacent to both the united cphs. in this case we simply combine the shared edges
                {
                    newAdjCorrCphs.SharedCEdgeLt.AddRange(AddKey.SharedCEdgeLt);
                    newAdjCorrCphs.dblSharedSegLength += AddKey.dblSharedSegLength;
                    newAdjCorrCphs.intSharedCEdgeCount += AddKey.intSharedCEdgeCount;
                }
                else //if (isExisting == false && isAdjacencyExisting == false)
                {
                    incrementalAdjCorrCphsSD.Add(AddKey, AddKey);
                }
            }

            return incrementalAdjCorrCphsSD;
        }

        private static SortedDictionary<CCorrCphs, CCorrCphs> GenerateNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD(List<CCorrCphs> pNewAdjCorrCphsLt_WithoutReplaced,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD,
            SortedDictionary<CCorrCphs, CCorrCphs> incrementalAdjCorrCphsSD)
        {
            var pNewAdjCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>();

            foreach (var pCorrCphs in pNewAdjCorrCphsLt_WithoutReplaced)
            {
                pNewAdjCorrCphsSD.Add(pCorrCphs, pCorrCphs);
            }

            foreach (var pAdjCorrCphsKvp in incrementalAdjCorrCphsSD)
            {
                pNewAdjCorrCphsSD.Add(pAdjCorrCphsKvp.Key, pAdjCorrCphsKvp.Value);

                if (ExistingCorrCphsSD.ContainsKey(pAdjCorrCphsKvp.Key) == false)
                {
                    ExistingCorrCphsSD.Add(pAdjCorrCphsKvp.Key, pAdjCorrCphsKvp.Value);
                }
            }

            return pNewAdjCorrCphsSD;
        }
        #endregion


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
        public CRegion GenerateCrgAndUpdateQ(CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q, List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt,
            SortedDictionary<CCorrCphs, CCorrCphs> newAdjCorrCphsSD, CPatch activecph, CPatch passivecph, CPatch unitedcph,
            CCorrCphs unitingCorrCphs, int intFinalTypeIndex, double[,] padblTD, int intFactor = 1)
        {
            int intactiveTypeIndex = this.GetCphTypeIndex(activecph);
            int intpassiveTypeIndex = this.GetCphTypeIndex(passivecph);

            var newcrg = this.GenerateCrgChildAndComputeCost(lscrg, newAdjCorrCphsSD,
             activecph, passivecph, unitedcph, unitingCorrCphs, intactiveTypeIndex, intpassiveTypeIndex, padblTD);

            //var intGIDLt = new List<int>();
            //var intTypeLt = new List<int>();
            //if (newcrg.intSumCphGID==65794 &&newcrg.intSumTypeIndex==172)
            //{

            //    foreach (var cph in newcrg.GetCphCol())
            //    {
            //        intGIDLt.Add(cph.GID);
            //    }


            //    foreach (var inttype in newcrg.GetCphTypeIndexCol())
            //    {
            //        intTypeLt.Add(inttype);
            //    }

            //    foreach (var item in Q)
            //    {
            //        if (item.intSumCphGID== 65794 && item.intSumTypeIndex == 172)
            //        {
            //            int ss = 5;
            //        }
            //    }
            //}


            CRegion outcrg;
            if (ExistingCrgSDLt[newcrg.GetCphCount()].TryGetValue(newcrg, out outcrg))
            {
                int intResult = newcrg.dblCostExact.CompareTo(outcrg.dblCostExact);
                //int intResult = CCmpMethods.CmpCoordDbl_VerySmall(newcrg.dblCostExact, outcrg.dblCostExact);

                if (intResult == -1)
                {
                    //from the idea of A* algorithm, we know that outcrg must be in Q
                    //var Q = new SortedSet<CRegion>(CRegion.pCmpCRegion_Cost_CphGIDTypeIndex);
                    //there is no decrease key function for SortedSet, so we have to remove it and later add it again
                    if (Q.Remove(outcrg) == true) 
                    {
                        //we don't use newcrg dicrectly because some regions may use ourcrg as their children
                        outcrg.cenumColor = newcrg.cenumColor;
                        outcrg.dblCostExactType = newcrg.dblCostExactType;
                        outcrg.dblCostExactComp = newcrg.dblCostExactComp;
                        outcrg.dblCostExactArea = newcrg.dblCostExactArea;
                        outcrg.dblCostExact = newcrg.dblCostExact;
                        outcrg.d = newcrg.dblCostExact + outcrg.dblCostEst;

                        outcrg.AggedCphs = newcrg.AggedCphs;
                        outcrg.parent = newcrg.parent;
                        newcrg = outcrg;
                        Q.Add(newcrg);
                    }
                    else
                    {
                        if (intFactor == 1)
                        {
                            throw new ArgumentException("outcrg should be removed!");
                        }
                        else
                        {
                            // if intFactor != 1, we are overestimating, outcrg may have been removed as the node with least cost
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
                ComputeEstCost(lscrg, sscrg, newcrg, activecph, passivecph, unitedcph, intactiveTypeIndex, intpassiveTypeIndex,
                    intFinalTypeIndex, padblTD, intFactor);
                Q.Add(newcrg);
                ExistingCrgSDLt[newcrg.GetCphCount()].Add(newcrg, newcrg);
                CRegion._intNodeCount++;
            }
            CRegion._intEdgeCount++;

            return newcrg;
        }

        public CRegion GenerateCrgChildAndComputeCost(CRegion lscrg, SortedDictionary<CCorrCphs, CCorrCphs> newAdjCorrCphsSD,
            CPatch activecph, CPatch passivecph, CPatch unitedcph, CCorrCphs unitingCorrCphs,
            int intactiveTypeIndex, int intpassiveTypeIndex, double[,] padblTD)
        {
            var newCphTypeIndexSD = new SortedDictionary<CPatch, int>(this.CphTypeIndexSD_Area_CphGID, CPatch.pCmpCPatch_Area_CphGID);
            newCphTypeIndexSD.Remove(activecph);
            newCphTypeIndexSD.Remove(passivecph);
            newCphTypeIndexSD.Add(unitedcph, intactiveTypeIndex);

            //****if I update the codes below, then I should consider updating the codes in function GenerateCrgAndUpdateQ
            //for transfering information to outcrg
            //e.g., outcrg.newCph = newcrg.newCph;
            CRegion newcrg = new CRegion(this.ID);
            newcrg.dblArea = this.dblArea;
            newcrg.cenumColor = CEnumColor.gray;
            newcrg.parent = this;
            newcrg.AggedCphs = new CValTri<CPatch, CPatch, CPatch>(activecph, passivecph, unitedcph);
            newcrg.AdjCorrCphsSD = newAdjCorrCphsSD;
            newcrg.CphTypeIndexSD_Area_CphGID = newCphTypeIndexSD;
            newcrg.intSumCphGID = this.intSumCphGID - activecph.GID - passivecph.GID + unitedcph.GID;
            newcrg.intSumTypeIndex = this.intSumTypeIndex - intpassiveTypeIndex;
            //newcrg.intEdgeCount = this.intEdgeCount - intDecreaseEdgeCount;
            newcrg.intInteriorEdgeCount = this.intInteriorEdgeCount - unitingCorrCphs.intSharedCEdgeCount;
            newcrg.intExteriorEdgeCount = this.intExteriorEdgeCount;
            newcrg.dblInteriorSegLength = this.dblInteriorSegLength - unitingCorrCphs.dblSharedSegLength;
            newcrg.dblExteriorSegLength = this.dblExteriorSegLength;

            if (CConstants.blnComputeMinComp == true)
            {
                ComputeMinCompIncremental(newcrg, this, activecph, passivecph, unitedcph);
            }
            else if (CConstants.blnComputeAvgComp == true)
            {
                newcrg.dblSumComp = this.dblSumComp - activecph.dblComp
                    - passivecph.dblComp + unitedcph.dblComp;
                newcrg.dblAvgComp = newcrg.dblSumComp / newcrg.GetCphCount();
            }

            ComputeExactCost(lscrg, newcrg, activecph, passivecph, unitedcph, intactiveTypeIndex, intpassiveTypeIndex, padblTD);

            return newcrg;
        }

        public void ComputeMinCompIncremental(CRegion newcrg, CRegion parentcrg, CPatch activecph, CPatch passivecph, CPatch unitedcph)
        {
            if (unitedcph.dblComp <= parentcrg.dblMinComp)
            {
                newcrg.dblMinComp = unitedcph.dblComp;
            }
            else  //unitedcph.dblComp > parentcrg.dblMinComp
            {
                if (activecph.dblComp == parentcrg.dblMinComp || passivecph.dblComp == parentcrg.dblMinComp)
                {
                    //the patch owns parentcrg.dblMinComp does not exist anymore, so we recompute a new dblMinComp
                    newcrg.ComputeMinComp();
                }
                else
                {
                    newcrg.dblMinComp = parentcrg.dblMinComp;
                }
            }
        }

        public void InitialEstimatedCost(CRegion sscrg, double[,] padblTD, int intFactor)
        {
            this.dblCostEstType = 0;
            foreach (var kvp in this.CphTypeIndexSD_Area_CphGID)
            {
                this.dblCostEstType += kvp.Key.dblArea * padblTD[kvp.Value, sscrg.GetSoloCphTypeIndex()];   //there is only one element in targetCrg
            }
            this.dblCostEstType = intFactor * this.dblCostEstType;

            if (CConstants.strShapeConstraint == "MaximizeMinArea")
            {
                //this.dblCostEstArea = intFactor * EstimateSumMinArea(this);
                //this.dblCostEst += this.dblCostEstArea;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp_EdgeNumber")
            {
                this.dblCostEstComp = intFactor * BalancedEstAvgComp_EdgeNumber(this, this, sscrg);
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp_Combine")
            {
                this.dblCostEstComp = intFactor * BalancedEstMinComp_Combine(this, this, sscrg);
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp_EdgeNumber")
            {
                this.dblCostEstComp = intFactor * BalancedEstMinComp_EdgeNumber(this, this, sscrg);
            }
            else if (CConstants.strShapeConstraint == "MinimizeInteriorBoundaries")
            {
                this.dblCostEstComp = intFactor * BalancedEstCompInteriorLength_Basic(this, this);
            }

            this.dblCostEst = (1 - CAreaAgg_AStar.dblLamda) * this.dblCostEstType + 
                CAreaAgg_AStar.dblLamda * this.dblArea * this.dblCostEstComp;

            this.d = this.dblCostEst;
        }

        public void ComputeExactCost(CRegion lscrg, CRegion NewCrg, CPatch activecph, CPatch passivecph, CPatch unitedcph,
            int intactiveTypeIndex, int intpassiveTypeIndex, double[,] padblTD)
        {
            var ParentCrg = NewCrg.parent;
            NewCrg.dblCostExactType = ParentCrg.dblCostExactType + passivecph.dblArea * padblTD[intactiveTypeIndex, intpassiveTypeIndex];
            var intTimeNum = lscrg.GetCphCount();



            if (CConstants.strShapeConstraint == "MaximizeMinArea")
            {
                //NewCrg.dblCostExactArea = ParentCrg.dblCostExactArea + passivecph.dblArea * ComputeCostArea(NewCrg.CphTypeIndexSD.Keys, NewCrg.dblArea);

                //NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblCostExactArea;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp_EdgeNumber" ||
                CConstants.strShapeConstraint == "MaximizeAvgComp_Combine")
            {
                //divide by intTimeNum - 2, because at each step the value for AvgComp can be 1 and there are intotal intTimeNum - 2 steps; 
                //only when intTimeNum - 1 > 0, we are in this function and run the following codes
                if (intTimeNum - NewCrg.GetCphCount() > 1)  //we have exact cost from t=3
                {
                    NewCrg.dblCostExactComp = ParentCrg.dblCostExactComp + (1 - ParentCrg.dblAvgComp) / (intTimeNum - 2);
                }
                else
                {
                    NewCrg.dblCostExactComp = 0;
                }

                NewCrg.dblCostExact = (1 - CAreaAgg_AStar.dblLamda) * NewCrg.dblCostExactType +
                    CAreaAgg_AStar.dblLamda * NewCrg.dblArea * NewCrg.dblCostExactComp;
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp_EdgeNumber"
                || CConstants.strShapeConstraint == "MaximizeMinComp_Combine")
            {
                //divide by intTimeNum - 2, because at each step the value for AvgComp can be 1 and there are intotal intTimeNum - 2 steps; 
                //only when intTimeNum - 1 > 0, we are in this function and run the following codes
                if (intTimeNum - NewCrg.GetCphCount() > 1)  //we have exact cost from t=3
                {
                    NewCrg.dblCostExactComp = ParentCrg.dblCostExactComp + (1 - ParentCrg.dblMinComp) / (intTimeNum - 2);
                }
                else
                {
                    NewCrg.dblCostExactComp = 0;
                }

                NewCrg.dblCostExact = (1 - CAreaAgg_AStar.dblLamda) * NewCrg.dblCostExactType +
                    CAreaAgg_AStar.dblLamda * NewCrg.dblArea * NewCrg.dblCostExactComp;
            }
            else if (CConstants.strShapeConstraint == "MinimizeInteriorBoundaries")
            {
                //divide by intTimeNum - 2, because at each step the value for InteriorSegLength can be 1 and there are intotal intTimeNum - 2 steps; 
                //only when intTimeNum - 1 > 0, we are in this function and run the following codes
                if (intTimeNum - NewCrg.GetCphCount() > 1)  //we have exact cost from t=3
                {
                    // lscrg.dblInteriorSegLength * NewCrg.GetCphCount() / (intTimeNum - 1) is the expected interior length at time t-1
                    NewCrg.dblCostExactComp = ParentCrg.dblCostExactComp +
                    ParentCrg.dblInteriorSegLength
                    / (lscrg.dblInteriorSegLength * NewCrg.GetCphCount() / (intTimeNum - 1))
                    / (intTimeNum - 2);
                }
                else
                {
                    NewCrg.dblCostExactComp = 0;
                }

                NewCrg.dblCostExact = (1 - CAreaAgg_AStar.dblLamda) * NewCrg.dblCostExactType +
                    CAreaAgg_AStar.dblLamda * NewCrg.dblArea * NewCrg.dblCostExactComp;
            }
            else if (CConstants.strShapeConstraint == "NonShape")
            {
                NewCrg.dblCostExact = NewCrg.dblCostExactType;
            }

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
        /// <remarks>if we change the cost method, then we will also need to change the codes of InitialSubCostEstimated in CRegion.cs, the codes of updating existing outcrg </remarks>
        public void ComputeEstCost(CRegion lscrg, CRegion sscrg, CRegion NewCrg, CPatch activecph, CPatch passivecph, CPatch unitedcph, 
            int intactiveTypeIndex, int intpassiveTypeIndex, int intFinalTypeIndex, double[,] padblTD, int intFactor)
        {
            var ParentCrg = NewCrg.parent;
            var test = intFactor * passivecph.dblArea
                * (padblTD[intactiveTypeIndex, intFinalTypeIndex] - padblTD[intpassiveTypeIndex, intFinalTypeIndex]);
            NewCrg.dblCostEstType = ParentCrg.dblCostEstType + intFactor * passivecph.dblArea 
                * (padblTD[intactiveTypeIndex, intFinalTypeIndex] - padblTD[intpassiveTypeIndex, intFinalTypeIndex]);


            if (CConstants.strShapeConstraint == "MaximizeMinArea")
            {
                //NewCrg.dblCostEstArea = intFactor*EstimateSumMinArea(NewCrg);  //will we do this twice????

                //NewCrg.dblCostEst = NewCrg.dblCostEstType + NewCrg.dblCostEstArea;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp_EdgeNumber")
            {
                NewCrg.dblCostEstComp = intFactor * BalancedEstAvgComp_EdgeNumber(NewCrg, lscrg, sscrg);

                //to make dblCostEstComp comparable to dblCostEstType and to avoid digital problems, we time dblCostEstComp by area
                //we will adjust the value later
                NewCrg.dblCostEst = (1 - CAreaAgg_AStar.dblLamda) * NewCrg.dblCostEstType +
                    CAreaAgg_AStar.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstComp;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp_Combine")
            {

            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp_Comine")
            {
                NewCrg.dblCostEstComp = intFactor * BalancedEstMinComp_Combine(NewCrg, lscrg, sscrg);

                //to make dblCostEstComp comparable to dblCostEstType and to avoid digital problems, we time dblCostEstComp by area
                //we will adjust the value later
                NewCrg.dblCostEst = (1 - CAreaAgg_AStar.dblLamda) * NewCrg.dblCostEstType + 
                    CAreaAgg_AStar.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstComp;   
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp_EdgeNumber")
            {
                NewCrg.dblCostEstComp = intFactor * BalancedEstMinComp_EdgeNumber(NewCrg, lscrg, sscrg);

                //to make dblCostEstComp comparable to dblCostEstType and to avoid digital problems, we time dblCostEstComp by area
                //we will adjust the value later
                NewCrg.dblCostEst = (1 - CAreaAgg_AStar.dblLamda) * NewCrg.dblCostEstType + 
                    CAreaAgg_AStar.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstComp;   
            }
            else if (CConstants.strShapeConstraint == "MinimizeInteriorBoundaries")
            {
                NewCrg.dblCostEstComp = intFactor * BalancedEstCompInteriorLength_Basic(NewCrg, lscrg);

                ////to make dblCostEstComp comparable to dblCostEstType and to avoid digital problems, we time dblCostEstComp by area
                ////we will adjust the value later
                NewCrg.dblCostEst = (1 - CAreaAgg_AStar.dblLamda) * NewCrg.dblCostEstType +
                    CAreaAgg_AStar.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstComp; 
            }
            else if (CConstants.strShapeConstraint == "NonShape")
            {
                NewCrg.dblCostEst = NewCrg.dblCostEstType;
            }

            //double dblWeight = 0.5;
            NewCrg.d = NewCrg.dblCostExact + NewCrg.dblCostEst;
        }



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
            var EstEdgeLengthEt = EstimateMinComp_EdgeLength(crg, lscrg).GetEnumerator();   //we need to improve this estimation to make sure this is an upper bound

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
        private double BalancedEstAvgComp_EdgeNumber(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            // if lscrg.GetCphCount() <= 2, the domain only have two polygons
            // A value will be divided by (lscrg.GetCphCount() - 2). To avoid being divided by 0, we directly return 0.
            if (lscrg.GetCphCount() <= 2)
            {
                return 0;
            }

            return EstimatedComp_Common(EstimateAvgComp_EdgeNumber(crg, lscrg), crg, lscrg, sscrg);
        }


        /// <summary>
        /// from time t to n-1
        /// </summary>
        /// <param name="crg"></param>
        /// <returns></returns>
        private IEnumerable<double> EstimateAvgComp_EdgeNumber(CRegion crg, CRegion lscrg)
        {
            double dblSumComp = 0;
            var dblCompSS = new SortedSet<CValPair<double,int>>();
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
            
            int intEdgeCountAtmost = crg.intExteriorEdgeCount + crg.intInteriorEdgeCount;
            int intEstCount = crg.GetCphCount();

            IEnumerator<int> intEdgeCountSSEt = intEdgeCountSS.GetEnumerator();
            double dblCompRegularPolygon = 0;
            while (intEstCount > 1)
            {
                //if intEstCount == lscrg.GetCphCount(), we are estimating from the start map (t==1)
                //we define that the estimation value of the start map is 0, therefore we skip intEstCount == lscrg.GetCphCount()
                if (intEstCount < lscrg.GetCphCount())
                {
                    double dblAvgComp = dblSumComp / intEstCount;
                    yield return dblAvgComp;
                }   
                
                //remove the two smallest compactnesses
                for (int i = 0; i < 2; i++)
                {
                    dblSumComp -= dblCompSS.Min().val1;
                    if (dblCompSS.Remove(dblCompSS.Min())==false)
                    {
                        throw new ArgumentException("failed to remove the smallest element!");
                    }
                }

                intEdgeCountSSEt.MoveNext();
                intEdgeCountAtmost -= intEdgeCountSSEt.Current;
                dblCompRegularPolygon = CGeoFunc.CalCompRegularPolygon(intEdgeCountAtmost);

                dblCompSS.Add(new CValPair<double, int>(dblCompRegularPolygon, intCompCount++));
                dblSumComp += dblCompRegularPolygon;
                intEstCount--;                
            }
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
                    int intAverageEdgeCount = Convert.ToInt32(Math.Floor(Convert.ToDouble(intEdgeCountAtmost) / Convert.ToDouble(intEstCount)));
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

        #region BalancedEstCompInteriorLength_Basic
        private double BalancedEstCompInteriorLength_Basic(CRegion crg, CRegion lscrg)
        {
            // if lscrg.GetCphCount() <= 2, the domain only have two polygons
            // A value will be divided by (lscrg.GetCphCount() - 2). To avoid being divided by 0, we directly return 0.
            if (lscrg.GetCphCount() <= 2)
            {
                return 0;
            }

            var EstimateLt = EstimateInteriorLength(crg, lscrg);
            double dblSum = 0;
            for (int i = 0; i < EstimateLt.Count; i++)
            {
                dblSum += (EstimateLt[i] / (i + 1));
            }

            return dblSum / (lscrg.dblInteriorSegLength / (lscrg.GetCphCount() - 1)) / (lscrg.GetCphCount() - 2);
        }

        /// <summary>
        /// Estimate Interior Lengths for the future steps, without current step
        /// </summary>
        /// <param name="crg"></param>
        /// <param name="lscrg"></param>
        /// <returns></returns>
        /// <remarks>from time t to n-1</remarks>
        private List<double> EstimateInteriorLength(CRegion crg, CRegion lscrg)
        {
            var dblSegLengthSS = new SortedSet<double>(new CCmpDbl());  //items in dblEdgeLengthSS is ordered from largest to smallest
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

            //from time n-1 to t
            //we compute from the last step (only one interior boundary) to the current step (many interior boundaries).             
            double dblInteriorLength = 0;
            var dblInteriorLengthLt = new List<double>(intEstCount - 1);
            for (int i = 0; i < intEstCount - 1; i++)
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


        ////**************estimate the first step considering type changing
        //private double EstimateCompForFirstStep(CRegion crg)
        //{
        //    double dblMaxMinComp = 0;

        //    var pCphTypeIndexSD_Area_CphGID = new SortedSet<CPatch>(crg.GetCphCol(), crg.CphTypeIndexSD_Area_CphGID.Comparer);  
        //    foreach (var pAdjacency_CorrCphs in crg.AdjCorrCphsSD.Keys)
        //    {                              
        //        pCphTypeIndexSD_Area_CphGID.Remove(pAdjacency_CorrCphs.FrCph);
        //        pCphTypeIndexSD_Area_CphGID.Remove(pAdjacency_CorrCphs.ToCph);

        //        var newcph = pAdjacency_CorrCphs.FrCph.Unite(pAdjacency_CorrCphs.ToCph, pAdjacency_CorrCphs.dblSharedSegLength);
        //        pCphTypeIndexSD_Area_CphGID.Add(newcph);

        //        dblMaxMinComp = Math.Max(dblMaxMinComp, pCphTypeIndexSD_Area_CphGID.Min(cph => cph.dblComp));

        //        pCphTypeIndexSD_Area_CphGID.Remove(newcph);
        //        pCphTypeIndexSD_Area_CphGID.Add(pAdjacency_CorrCphs.FrCph);
        //        pCphTypeIndexSD_Area_CphGID.Add(pAdjacency_CorrCphs.ToCph);
        //    }

        //    return dblMaxMinComp;
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="crg"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //private double EstimateAvgCompes(CRegion crg, int intTimeNum)
        //{
        //var intEdgeCountSS = new SortedSet<int>(new CIntCompare());
        //foreach (var pCorrCphs in crg.AdjCorrCphsSD.Keys)
        //{
        //    intEdgeCountSS.Add(pCorrCphs.intSharedCEdgeCount);
        //}

        //int intEdgeCountAtmost = crg.intExteriorEdgeCount + crg.intInteriorEdgeCount;
        //int intLeftStepNum = crg.GetCphCount() - 1;
        //int intCount = intLeftStepNum;
        //double dblSumCompValue = 0;
        //foreach (var intEdgeCount in intEdgeCountSS)
        //{
        //    if (intCount <= 1)
        //    {
        //        break;
        //    }

        //    intEdgeCountAtmost -= intEdgeCount;
        //    dblSumCompValue +=  EstimateCompForOneCrgInstance(intEdgeCountAtmost);

        //    intCount--;
        //}

        //dblSumCompValue *= crg.dblArea;  //the patch with largest campactness can be as large as the whole domain

        //if (dblSumCompValue > 0)
        //{
        //    return dblSumCompValue / (intTimeNum - 1);
        //}
        //else
        //{
        //    return 0;
        //}
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="activecph"></param>
        ///// <param name="passivecph"></param>
        ///// <param name="unitedcph"></param>
        ///// <param name="SSCph"></param>
        ///// <param name="NewCrg"></param>
        ///// <param name="padblTD"></param>
        ///// <remarks>if we change the cost method, then we will also need to change the codes of InitialSubCostEstimated in CRegion.cs, the codes of updating existing outcrg </remarks>
        //public void ComputeCost(CPatch activecph, CPatch passivecph, CPatch unitedcph, int intactiveTypeIndex, int intpassiveTypeIndex, int intFinalTypeIndex, CRegion NewCrg, double[,] padblTD, int intTimeNum, int intFactor =1 )
        //{
        //    var ParentCrg = NewCrg.parent;
        //    //double dblCostType = passivecph.dblArea * padblTD[intactiveTypeIndex, intpassiveTypeIndex];
        //    NewCrg.dblCostExactType = ParentCrg.dblCostExactType + passivecph.dblArea * padblTD[intactiveTypeIndex, intpassiveTypeIndex];
        //    NewCrg.dblCostEstType = ParentCrg.dblCostEstType + intFactor*passivecph.dblArea * (padblTD[intactiveTypeIndex, intFinalTypeIndex] - padblTD[intpassiveTypeIndex, intFinalTypeIndex]);



        //    if (CConstants.strShapeConstraint == "MaximizeMinArea")
        //    {
        //        //NewCrg.dblCostExactArea = ParentCrg.dblCostExactArea + passivecph.dblArea * ComputeCostArea(NewCrg.CphTypeIndexSD.Keys, NewCrg.dblArea);
        //        //NewCrg.dblCostEstArea = intFactor*EstimateSumMinArea(NewCrg);  //will we do this twice????

        //        //NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblCostExactArea;
        //        //NewCrg.dblCo stEstimated = NewCrg.dblCostEstType + NewCrg.dblCostEstArea;
        //    }
        //    else if (CConstants.strShapeConstraint == "MaximizeAvgComp")
        //    {
        //        NewCrg.dblSumComp = ParentCrg.dblSumComp - activecph.dblArea * activecph.dblComp - passivecph.dblArea * passivecph.dblComp + unitedcph.dblArea * unitedcph.dblComp;

        //        if (intTimeNum > 2)
        //        {
        //            NewCrg.dblCostExactComp = ParentCrg.dblCostExactComp + NewCrg.dblSumComp / (intTimeNum - 1);   //divide by intTimeNum because at each step, the value for AvgComp can be 1; intTimeNum - 1 == LSCrg.GetCphCount() - 2
        //        }
        //        else
        //        {
        //            NewCrg.dblCostExactComp = 0;
        //        }
        //        NewCrg.dblCostEstComp = intFactor * EstimateAvgCompes(NewCrg, intTimeNum);


        //        NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblCostExactComp;
        //        NewCrg.dblCostEst = NewCrg.dblCostEstType + NewCrg.dblCostEstComp;
        //    }
        //    else if (CConstants.strShapeConstraint == "NonShape")
        //    {
        //        NewCrg.dblCostExact = NewCrg.dblCostExactType;
        //        NewCrg.dblCostEst = NewCrg.dblCostEstType;
        //    }

        //    //double dblWeight = 0.5;
        //    NewCrg.d = NewCrg.dblCostExact + NewCrg.dblCostEst;
        //}

        //public double EstimateSumMinArea(CRegion pCrg)
        //{
        //    pCrg.SetInitialAdjacencyForCph();
        //    SortedSet<CPatch> tempCphAreaSS = new SortedSet<CPatch>(pCrg.CphTypeIndexSD.Keys, CPatch.pCmpCPatch_Area_CphGID);


        //    double dblEstimateSumMinArea = 0;
        //    for (int i = 0; i < pCrg.CphTypeIndexSD.Count - 1; i++)
        //    {
        //        var min1 = tempCphAreaSS.Min;  //get the smallest one
        //        tempCphAreaSS.Remove(min1);
        //        var min2 = min1.Adjacent_CphsCEdgeLtSD.GetFirstT().Key;
        //        tempCphAreaSS.Remove(min2);

        //        var unitedcph = min1.Unite(min2);
        //        unitedcph.Adjacent_CphsCEdgeLtSD = new SortedDictionary<CPatch, List<CEdge>>(CPatch.pCmpCPatch_Area_CphGID);
        //        tempCphAreaSS.Add(unitedcph);

        //        dblEstimateSumMinArea += min1.dblArea * ComputeCostArea(tempCphAreaSS, pCrg.dblArea);


        //        //update the adjacencies
        //        foreach (var kvp in min1.Adjacent_CphsCEdgeLtSD)
        //        {
        //            if (kvp.Key.GID != min2.GID)
        //            {
        //                kvp.Key.Adjacent_CphsCEdgeLtSD.Remove(min1);

        //                kvp.Key.Adjacent_CphsCEdgeLtSD.Add(unitedcph, kvp.Value);
        //                unitedcph.Adjacent_CphsCEdgeLtSD.Add(kvp.Key, kvp.Value);
        //            }
        //        }

        //        foreach (var kvp in min2.Adjacent_CphsCEdgeLtSD)
        //        {
        //            if (kvp.Key.GID != min1.GID)
        //            {

        //                kvp.Key.Adjacent_CphsCEdgeLtSD.Remove(min2);

        //                List<CEdge> outedgelt = null;
        //                if (kvp.Key.Adjacent_CphsCEdgeLtSD.TryGetValue(unitedcph, out outedgelt))    //if cedge already exists in cedgeSD, then we now have found the Patch which shares this cedge
        //                {
        //                    outedgelt.AddRange(kvp.Value);
        //                }
        //                else
        //                {
        //                    kvp.Key.Adjacent_CphsCEdgeLtSD.Add(unitedcph, kvp.Value);
        //                    unitedcph.Adjacent_CphsCEdgeLtSD.Add(kvp.Key, kvp.Value);
        //                }
        //            }

        //        }
        //    }

        //    foreach (var cph in pCrg.CphTypeIndexSD.Keys)
        //    {
        //        cph.Adjacent_CphsCEdgeLtSD = null;
        //    }

        //    return dblEstimateSumMinArea;


        //}

        //public void SetInitialAdjacencyForCph()
        //{
        //    foreach (var cph in this.CphTypeIndexSD.Keys)
        //    {
        //        cph.Adjacent_CphsCEdgeLtSD = new SortedDictionary<CPatch, List<CEdge>>(CPatch.pCmpCPatch_Area_CphGID);
        //    }

        //    foreach (var kvp in this.AdjCorrCphsSD)
        //    {
        //        var frcph = kvp.Key.FrCph;
        //        var tocph = kvp.Key.ToCph;

        //        var cedgelt = new List<CEdge>(kvp.Value);

        //        frcph.Adjacent_CphsCEdgeLtSD.Add(tocph, cedgelt);
        //        tocph.Adjacent_CphsCEdgeLtSD.Add(frcph, cedgelt);
        //    }
        //}


        //private double ComputeCostArea(ICollection <CPatch> pCphAreaSS, double dblTotalArea)
        //{
        //    return (1 - pCphAreaSS.GetFirstT().dblArea / (dblTotalArea / pCphAreaSS.Count));
        //}
    }
}
