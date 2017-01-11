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
        public static int _intNodesCount;
        public static int _intStartStaticGIDLast;
        public static int _intStartStaticGIDAll;
        public static int _intStaticGID;
        public static int _intStaticTest;

        //public LinkedList<int> _IDLk;  //ID of the Region, where the ID is a LinkedList
        public static CCompareCRegion_Cost_CphGIDTypeIndex pCompareCRegion_Cost_CphGIDTypeIndex = new CCompareCRegion_Cost_CphGIDTypeIndex();  //this variable should be used for the queue Q

        //this comparer should be used for integrate the sequences for output 
        public static CCompareCRegion_MinArea_CphGIDTypeIndex pCompareCRegion_MinArea_CphGIDTypeIndex = new CCompareCRegion_MinArea_CphGIDTypeIndex();

        //this comparer should be used for integrate the sequences for output 
        public static CCompareCRegion_CostExact_CphGIDTypeIndex pCompareCRegion_CostExact_CphGIDTypeIndex = new CCompareCRegion_CostExact_CphGIDTypeIndex();

        //this comparer should be used for checking existing Crgs
        public static CCompareCRegion_CphGIDTypeIndex pCompareCRegion_CphGIDTypeIndex = new CCompareCRegion_CphGIDTypeIndex();

        //this comparer should be used for counting uncolored Crgs
        public static CCompareCRegion_CphGID pCompareCRegion_CphGID = new CCompareCRegion_CphGID();  //this variable should be used for CRegion itself
        
        //public static CCompareCRegion_CompareDblPreLocateEqual pCompareCRegion_CompareDblPreLocateEqual = new CCompareCRegion_CompareDblPreLocateEqual();
        //public static 

        public SortedDictionary<CCorrCphs, CCorrCphs> AdjCorrCphsSD { get; set; }  //compare GID of CorrCphs

        public SortedDictionary<CPatch, int> CphTypeIndexSD_Area_CphGID { get; set; }  //Why did I use SortedDictionary? We use this comparator CPatch .pCompareCPatch_Area_CphGID
        //public SortedSet<CPatch> CphSS_Compactness_CphGID { get; set; }  //Why did I use SortedDictionary? We use this comparator CPatch .pCompareCPatch_Area_CphGID

        public static long _lngEstimationCountEdgeNumber;
        public static long _lngEstimationCountEdgeLength;
        public static long _lngEstimationCountEqual;

        //public SortedSet<CPatch> CphAreaSS { get; set; }
        public int intSumCphGID { get; set; }
        public int intSumTypeIndex { get; set; }
        //public int intEdgeCount { get; set; }
        public int intInteriorEdgeCount { get; set; }
        public int intExteriorEdgeCount { get; set; }
        public double  dblInteriorSegmentLength { get; set; }
        public double dblExteriorSegmentLength { get; set; }

        /// <summary>this is the real compactness 2*Sqrt(pi*A)/L.</summary>
        public double dblMinComp { get; set; }
        public double dblAvgComp { get; set; } 

        private double _dbld = double.MaxValue;
        public double d{ get; set; }

        public CRegion parent { get; set; }
        public CRegion child { get; set; }
        public CEnumColor cenumColor { get; set; }

        public double dblArea { get; set; }
        public double dblSumComp { get; set; }

        //public double dblCostExact { get; set; }
        public double dblCostEstimated { get; set; }
        public double dblCostExactType { get; set; }
        public double dblCostEstimatedType { get; set; }
        //public double _dblCostEstimatedType;
        public double dblCostExactCompactness { get; set; }
        public double dblCostEstimatedCompactness { get; set; }
        public double dblCostExactArea { get; set; }
        public double dblCostEstimatedArea { get; set; }

        double _dblCostExact = 0;
        public double dblCostExact 
        {
            get { return _dblCostExact; }
            set
            {
                if ((value < 0) || (value > double.MaxValue / 3))
                {
                    throw new ArgumentException("incorrect value!");
                }

                _dblCostExact = value;
            }
        }


        //public double dblCostEstimatedType
        //{
        //    get { return _dblCostEstimatedType; }
        //    set
        //    {
        //        if (value > 2*1712083)
        //        {
        //            throw new ArgumentException("incorrect value!");
        //        }

        //        _dblCostEstimatedType = value;
        //    }
        //}

        public CRegion()
            : this(-1)
        {
            this.GID = _intStaticGID++;
        }

        //public CRegion(int intID)
        //    : this(intID, new SortedDictionary<CPatch, int>(CPatch.pCompareCPatch_CpgGID))
        //{
        //}


        //public CRegion(int intID, SortedDictionary<CPatch, int> pCphTypeIndexSD)
        //{
        //    this.ID = intID;
        //    this.CphTypeIndexSD = new SortedDictionary<CPatch, int>(pCphTypeIndexSD, CPatch.pCompareCPatch_CpgGID);
        //    this.d = double.MaxValue;
        //    this.parent = null;
        //    this.cenumColor = CEnumColor.white;
        //}

        //public CRegion(int intID, string strShapeConstraint)
        public CRegion(int intID)
        {
            this.ID = intID;
            this.GID = _intStaticGID++;
            this.CphTypeIndexSD_Area_CphGID = new SortedDictionary<CPatch, int>(CPatch.pCompareCPatch_Area_CphGID);


            //intID==-2 is for a temporary Crg, and thus should not be counted
            if (intID==-2)
            {
                _intStaticGID--;
            }
            //this.d = double.MaxValue;
            
            this.parent = null;
            this.cenumColor = CEnumColor.white;
        }

        public ICollection<CPatch> GetCphCol()
        {
            return this.CphTypeIndexSD_Area_CphGID.Keys;
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
            if (this.CphTypeIndexSD_Area_CphGID.Count>1)
            {
                throw new ArgumentOutOfRangeException("There are more than one elements!");
            }

            return this.CphTypeIndexSD_Area_CphGID.GetFirstT().Value;
        }

        public int GetCphTypeIndex(CPatch cph)
        {
            int intTypeIndex;
            if (this.CphTypeIndexSD_Area_CphGID.TryGetValue(cph, out intTypeIndex)==false)
            {
                throw new ArgumentNullException ("The patch does not exist!");
            }
            return intTypeIndex;
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
            var ExistingCorrCphsSD0 = new SortedDictionary<CCorrCphs, CCorrCphs>(CCorrCphs.pCompareCCorrCphs_CphsGID); // we need this variable here, because it has different comparator with pAdjCorrCphsSD
            SortedDictionary<CEdge, CPatch> cedgeSD = new SortedDictionary<CEdge, CPatch>(new CCompareCEdgeCoordinates());  //why SortedDictionary? Because we want to get the value of an element. The element may have the same key with another element.
            var pAdjCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>();

            if (this .GetCphCount() > 1)
            {
                foreach (var cph in this.GetCphCol())
                {
                    foreach (var cedgelt in cph.GetSoloCpg().CEdgeLtLt)
                    {
                        foreach (CEdge cedge in cedgelt)  //Note that there is only one element in cph.CpgSS
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
                                    ExsitedCorrCphs.dblSharedSegmentLength += cedge.dblLength;
                                    ExsitedCorrCphs.intSharedCEdgeCount++;
                                }
                                else
                                {
                                    //List<CEdge> NewCphSharedEdgesLt = new List<CEdge>(1);
                                    //CorrCphs.SharedCEdgeLt = new List<CEdge>(1);
                                    CorrCphs.SharedCEdgeLt.Add(cedge);
                                    CorrCphs.dblSharedSegmentLength += cedge.dblLength;
                                    CorrCphs.intSharedCEdgeCount++;

                                    pAdjCorrCphsSD.Add(CorrCphs, CorrCphs);
                                    ExistingCorrCphsSD0.Add(CorrCphs, CorrCphs);
                                }

                                this.intInteriorEdgeCount++;
                                this.dblInteriorSegmentLength += cedge.dblLength;
                                cedgeSD.Remove(cedge);    //every edge belongs to two polygons, if we have found the two polygons, we can remove the shared edge from the SortedDictionary
                            }
                            else  //if cedge doesn't exist in cedgeSD, then we add this cedge
                            {
                                cedgeSD.Add(cedge, cph);
                                //this.intEdgeCount++;
                            }
                        } 
                    }
                }
            }

            this.intExteriorEdgeCount = cedgeSD.Count;
            foreach (var cedgekvp in cedgeSD)
            {
                this.dblExteriorSegmentLength += cedgekvp.Key.dblLength;
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
                foreach (var item in AggregateSmallestAndUpdateQ(lscrg, sscrg,Q, ExistingCrgSDLt, ExistingCphSDLt, ExistingCorrCphsSD, 
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
            var newAdjCorrCphsSD = ComputeNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD(pAdjCorrCphsSD, unitingCorrCphs,newcph,ExistingCorrCphsSD);


            var frcph = unitingCorrCphs.FrCph;
            var tocph = unitingCorrCphs.ToCph;
            int intfrTypeIndex = this.GetCphTypeIndex(frcph);
            int inttoTypeIndex = this.GetCphTypeIndex(tocph);
            

            //if the two cphs have the same type, then we only need to aggregate the smaller one into the larger one (this will certainly have smaller cost in terms of area)
            //otherwise, we need to aggregate from two directions
            if (intfrTypeIndex == inttoTypeIndex)
            {
                yield return GenerateCrgAndUpdateQ(lscrg, sscrg, Q, ExistingCrgSDLt, newAdjCorrCphsSD, frcph, tocph, newcph, unitingCorrCphs, 
                    intFinalTypeIndex, padblTD, intFactor);
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
               return  null;
            }
        }

        private CPatch ComputeNewCph(CCorrCphs unitingCorrCphs, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt)
        {
            var newcph = unitingCorrCphs.FrCph.Unite(unitingCorrCphs.ToCph, unitingCorrCphs.dblSharedSegmentLength);

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
            var incrementalAdjCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>(CCorrCphs.pCompareCCorrCphs_CphsGID);

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
                    newAdjCorrCphs.dblSharedSegmentLength += AddKey.dblSharedSegmentLength;
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
            CCorrCphs unitingCorrCphs, int intFinalTypeIndex, double[,] padblTD, int intFactor=1)
        {
            int intactiveTypeIndex = this.GetCphTypeIndex(activecph);
            int intpassiveTypeIndex = this.GetCphTypeIndex(passivecph);

            var newcrg = this.GenerateCrgChildAndComputeCost(lscrg, newAdjCorrCphsSD,
             activecph, passivecph, unitedcph, unitingCorrCphs, intactiveTypeIndex, intpassiveTypeIndex, padblTD);
            
            CRegion outcrg;
            if (ExistingCrgSDLt[newcrg.GetCphCount()].TryGetValue(newcrg, out outcrg))
            {
                int intResult = newcrg.dblCostExact.CompareTo(outcrg.dblCostExact);
                if (intResult == -1)
                {
                    //from the idea of A* algorithm, we know that outcrg must be in Q

                    if (Q.Remove(outcrg) == false)
                    {
                        throw new ArgumentException("outcrg should be removed!");
                    }
                    //Q.Remove(outcrg);  //there is no decrease key function for SortedSet, so we have to remove it and later add it again
                    outcrg.cenumColor = newcrg.cenumColor;
                    outcrg.dblCostExactType = newcrg.dblCostExactType;                  //*****the cost will need be updated if we integrate more evaluations
                    outcrg.dblCostExactCompactness = newcrg.dblCostExactCompactness;                  //*****the cost will need be updated if we integrate more evaluations
                    outcrg.dblCostExactArea = newcrg.dblCostExactArea;
                    outcrg.dblCostExact = newcrg.dblCostExact;
                    outcrg.d = newcrg.dblCostExact + outcrg.dblCostEstimated;

                    outcrg.parent = newcrg.parent;
                    newcrg = outcrg;
                    Q.Add(newcrg);
                    //CRegion._intStaticGID--;
                }
                else
                {
                    //we don't need to do operation Q.Add(newcrg);
                }
            }
            else
            {
                ComputeEstimatedCost(lscrg, sscrg, newcrg, activecph, passivecph, unitedcph, intactiveTypeIndex, intpassiveTypeIndex,
                    intFinalTypeIndex, padblTD, intFactor);
                Q.Add(newcrg);
                ExistingCrgSDLt[newcrg.GetCphCount()].Add(newcrg, newcrg);
                CRegion._intNodesCount++;
            }

            return newcrg;
        }

        public CRegion GenerateCrgChildAndComputeCost(CRegion lscrg, SortedDictionary<CCorrCphs, CCorrCphs> newAdjCorrCphsSD, 
            CPatch activecph, CPatch passivecph, CPatch unitedcph, CCorrCphs unitingCorrCphs, int intactiveTypeIndex, int intpassiveTypeIndex,  double[,] padblTD)
        {
            var newCphTypeIndexSD = new SortedDictionary<CPatch, int>(this.CphTypeIndexSD_Area_CphGID, CPatch.pCompareCPatch_Area_CphGID);
            newCphTypeIndexSD.Remove(activecph);
            newCphTypeIndexSD.Remove(passivecph);
            newCphTypeIndexSD.Add(unitedcph, intactiveTypeIndex);

            CRegion newcrg = new CRegion(this.ID);
            newcrg.dblArea = this.dblArea;
            newcrg.cenumColor = CEnumColor.gray;
            newcrg.parent = this;
            newcrg.AdjCorrCphsSD = newAdjCorrCphsSD;
            newcrg.CphTypeIndexSD_Area_CphGID = newCphTypeIndexSD;
            newcrg.intSumCphGID = this.intSumCphGID - activecph.GID - passivecph.GID + unitedcph.GID;
            newcrg.intSumTypeIndex = this.intSumTypeIndex - intpassiveTypeIndex;
            //newcrg.intEdgeCount = this.intEdgeCount - intDecreaseEdgeCount;
            newcrg.intInteriorEdgeCount = this.intInteriorEdgeCount - unitingCorrCphs.intSharedCEdgeCount;
            newcrg.intExteriorEdgeCount = this.intExteriorEdgeCount;
            newcrg.dblInteriorSegmentLength = this.dblInteriorSegmentLength - unitingCorrCphs.dblSharedSegmentLength;
            newcrg.dblExteriorSegmentLength = this.dblExteriorSegmentLength;

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
            this.dblCostEstimatedType = 0;
            foreach (var kvp in this.CphTypeIndexSD_Area_CphGID)
            {
                this.dblCostEstimatedType += kvp.Key.dblArea * padblTD[kvp.Value, sscrg.CphTypeIndexSD_Area_CphGID.GetFirstT().Value];   //there is only one element in targetCrg
                //dblCostEstimatedTypetest += kvp.Key.dblArea * padblTD[kvp.Value, targetCrg.CphTypeIndexSD_Area_CphGID.GetFirstT().Value];
            }
            this.dblCostEstimatedType = intFactor * this.dblCostEstimatedType;
            this.dblCostEstimated = (1 - CCAMDijkstra.dblLamda) * this.dblCostEstimatedType;

            if (CConstants.strShapeConstraint == "MaximizeMinArea")
            {
                //this.dblCostEstimatedArea = intFactor * EstimateSumMinArea(this);
                //this.dblCostEstimated += this.dblCostEstimatedArea;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp")
            {

            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp_Combine")
            {
                this.dblCostEstimatedCompactness = intFactor * BalancedEstimatedMinComp_Combine(this, this, sscrg);
                this.dblCostEstimated += CCAMDijkstra.dblLamda * (this.dblArea * this.dblCostEstimatedCompactness);
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp")
            {
                this.dblCostEstimatedCompactness = intFactor * BalancedEstimatedMinComp_EdgeNumber(this, this, sscrg);
                this.dblCostEstimated += CCAMDijkstra.dblLamda * (this.dblArea * this.dblCostEstimatedCompactness);
            }
            else if (CConstants.strShapeConstraint == "MinimizeInteriorBoundaries")
            {
                throw new ArgumentException("You need to update the compactness!");

                this.dblCostEstimatedCompactness = intFactor * BalancedEstimatedCompactnessInteriorLength_Basic(this, this);
                double dblWeightedCostEstimatedCompactness = this.dblArea * this.dblCostEstimatedCompactness;
                this.dblCostEstimated += dblWeightedCostEstimatedCompactness;
            }

            this.d = this.dblCostEstimated;
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
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp" ||
                CConstants.strShapeConstraint == "MaximizeAvgComp_Combine")
            {
                //divide by intTimeNum - 2, because at each step the value for AvgComp can be 1 and there are intotal intTimeNum - 2 steps; 
                //only when intTimeNum - 1 > 0, we are in this function and run the following codes
                if (intTimeNum - NewCrg.GetCphCount() > 1)  //we have exact cost from t=3
                {
                    NewCrg.dblCostExactCompactness = ParentCrg.dblCostExactCompactness + (1 - ParentCrg.dblAvgComp) / (intTimeNum - 2);
                }
                else
                {
                    NewCrg.dblCostExactCompactness = 0;
                }

                NewCrg.dblCostExact = (1 - CCAMDijkstra.dblLamda) * NewCrg.dblCostExactType +
                    CCAMDijkstra.dblLamda * NewCrg.dblArea * NewCrg.dblCostExactCompactness;
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp"
                || CConstants.strShapeConstraint == "MaximizeMinComp_Combine")
            {
                //divide by intTimeNum - 2, because at each step the value for AvgComp can be 1 and there are intotal intTimeNum - 2 steps; 
                //only when intTimeNum - 1 > 0, we are in this function and run the following codes
                if (intTimeNum - NewCrg.GetCphCount() > 1)  //we have exact cost from t=3
                {
                    NewCrg.dblCostExactCompactness = ParentCrg.dblCostExactCompactness + (1 - ParentCrg.dblMinComp) / (intTimeNum - 2);
                }
                else
                {
                    NewCrg.dblCostExactCompactness = 0;
                }

                NewCrg.dblCostExact = (1 - CCAMDijkstra.dblLamda) * NewCrg.dblCostExactType + 
                    CCAMDijkstra.dblLamda * NewCrg.dblArea * NewCrg.dblCostExactCompactness;
            }
            else if (CConstants.strShapeConstraint == "MinimizeInteriorBoundaries")
            {
                //divide by intTimeNum - 1, because at each step the value for AvgComp can be 1; At the step of ParentCrg intTimeNum - t == NewCrg.GetCphCount();
                NewCrg.dblCostExactCompactness = ParentCrg.dblCostExactCompactness +
                    ParentCrg.dblInteriorSegmentLength / lscrg.dblInteriorSegmentLength / (NewCrg.GetCphCount());
                NewCrg.dblCostExact = (1 - CCAMDijkstra.dblLamda) * NewCrg.dblCostExactType +
                    CCAMDijkstra.dblLamda * NewCrg.dblArea * NewCrg.dblCostExactCompactness;
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
        public void ComputeEstimatedCost(CRegion lscrg, CRegion sscrg, CRegion NewCrg, CPatch activecph, CPatch passivecph, CPatch unitedcph, 
            int intactiveTypeIndex, int intpassiveTypeIndex, int intFinalTypeIndex, double[,] padblTD, int intFactor)
        {
            var ParentCrg = NewCrg.parent;
            NewCrg.dblCostEstimatedType = ParentCrg.dblCostEstimatedType + intFactor * passivecph.dblArea 
                * (padblTD[intactiveTypeIndex, intFinalTypeIndex] - padblTD[intpassiveTypeIndex, intFinalTypeIndex]);


            if (CConstants.strShapeConstraint == "MaximizeMinArea")
            {
                //NewCrg.dblCostEstimatedArea = intFactor*EstimateSumMinArea(NewCrg);  //will we do this twice????

                //NewCrg.dblCostEstimated = NewCrg.dblCostEstimatedType + NewCrg.dblCostEstimatedArea;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp")
            {
                throw new ArgumentOutOfRangeException("I haven't considered this case yet!");
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp_Combine")
            {

            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp_Comine")
            {
                NewCrg.dblCostEstimatedCompactness = intFactor * BalancedEstimatedMinComp_Combine(NewCrg, lscrg, sscrg);

                //to make dblCostEstimatedCompactness comparable to dblCostEstimatedType and to avoid digital problems, we time dblCostEstimatedCompactness by area
                //we will adjust the value later
                NewCrg.dblCostEstimated = (1 - CCAMDijkstra.dblLamda) * NewCrg.dblCostEstimatedType + 
                    CCAMDijkstra.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstimatedCompactness;   
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp")
            {
                NewCrg.dblCostEstimatedCompactness = intFactor * BalancedEstimatedMinComp_EdgeNumber(NewCrg, lscrg, sscrg);

                //to make dblCostEstimatedCompactness comparable to dblCostEstimatedType and to avoid digital problems, we time dblCostEstimatedCompactness by area
                //we will adjust the value later
                NewCrg.dblCostEstimated = (1 - CCAMDijkstra.dblLamda) * NewCrg.dblCostEstimatedType + 
                    CCAMDijkstra.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstimatedCompactness;   
            }
            else if (CConstants.strShapeConstraint == "MinimizeInteriorBoundaries")
            {
                NewCrg.dblCostEstimatedCompactness = intFactor * BalancedEstimatedCompactnessInteriorLength_Basic(NewCrg, lscrg);

                ////to make dblCostEstimatedCompactness comparable to dblCostEstimatedType and to avoid digital problems, we time dblCostEstimatedCompactness by area
                ////we will adjust the value later
                NewCrg.dblCostEstimated = NewCrg.dblCostEstimatedType + NewCrg.dblArea * NewCrg.dblCostEstimatedCompactness;
            }
            else if (CConstants.strShapeConstraint == "NonShape")
            {
                NewCrg.dblCostEstimated = NewCrg.dblCostEstimatedType;
            }

            //double dblWeight = 0.5;
            NewCrg.d = NewCrg.dblCostExact + NewCrg.dblCostEstimated;
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
        private double BalancedEstimatedMinComp_Combine(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            if (crg.GetCphCount() == 1)
            {
                return 0;
            }

            var EstimateEdgeNumberEt = EstimateMinComp_EdgeNumber(crg).GetEnumerator();
            var EstimateEdgeLengthEt = EstimateMinComp_EdgeLength(crg).GetEnumerator();   //we need to improve this estimation to make sure this is an upper bound

            double dblSumCompValue = 0;
            while (EstimateEdgeNumberEt.MoveNext() && EstimateEdgeLengthEt.MoveNext())
            {
                if (EstimateEdgeNumberEt.Current < EstimateEdgeLengthEt.Current)
                {
                    CRegion._lngEstimationCountEdgeNumber++;
                }
                else if (EstimateEdgeNumberEt.Current > EstimateEdgeLengthEt.Current)
                {
                    CRegion._lngEstimationCountEdgeLength++;
                }
                else
                {
                    CRegion._lngEstimationCountEqual++;
                }

                dblSumCompValue += (1 - Math.Min(EstimateEdgeNumberEt.Current, EstimateEdgeLengthEt.Current));
            }

            return dblSumCompValue / (lscrg.GetCphCount() - 1);
        }

        #region EstimateMinComp_EdgeNumber
        /// <summary>
        /// 
        /// </summary>
        /// <param name="crg">crg.GetCphCount() > 1</param>
        /// <param name="lscrg">lscrg.GetCphCount() > 2</param>
        /// <param name="sscrg"></param>
        /// <returns></returns>
        private double BalancedEstimatedMinComp_EdgeNumber(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            // if lscrg.GetCphCount() <= 2, the domain only have two polygons
            // if crg.GetCphCount() <= 1, we are at the last step
            if (lscrg.GetCphCount() <= 2)
            {
                return 0;
            }

            return EstimatedMinComp_Common(EstimateMinComp_EdgeNumber(crg), crg, lscrg, sscrg);
        }

        private double EstimatedMinComp_Common(IEnumerable<double> EstimateEb, CRegion crg, CRegion lscrg, CRegion sscrg)
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
        private IEnumerable<double> EstimateMinComp_EdgeNumber(CRegion crg)
        {
            var intEdgeCountSS = new SortedSet<int>(new CIntCompare());
            foreach (var pCorrCphs in crg.AdjCorrCphsSD.Keys)
            {
                intEdgeCountSS.Add(pCorrCphs.intSharedCEdgeCount);
            }

            IEnumerator<int> intEdgeCountSSEt = intEdgeCountSS.GetEnumerator();
            int intEdgeCountAtmost = crg.intExteriorEdgeCount + 2 * crg.intInteriorEdgeCount;
            int intEstimationPolygonNum = crg.GetCphCount();

            while (intEstimationPolygonNum > 1)
            {
                intEdgeCountSSEt.MoveNext();
                double dblAverageEdgeCount = Math.Floor(Convert.ToDouble(intEdgeCountAtmost) / Convert.ToDouble(intEstimationPolygonNum));
                yield return EstimateCompactnessEdgeNumberOneCrgInstance(dblAverageEdgeCount);

                intEdgeCountAtmost -= (2 * intEdgeCountSSEt.Current);
                intEstimationPolygonNum--;
            }
        }


        private double EstimateCompactnessEdgeNumberOneCrgInstance(double dblEdgeCount)
        {
            double dblPIOverCount = Math.PI / dblEdgeCount;
            return Math.Sqrt(dblPIOverCount / Math.Tan(dblPIOverCount));
        }
        #endregion

        #region EstimateMinComp_EdgeLength
        private double BalancedEstimatedMinComp_EdgeLength(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            if (crg.GetCphCount() == 1)
            {
                return 0;
            }

            return EstimatedMinComp_Common(EstimateMinComp_EdgeLength(crg), crg, lscrg, sscrg);
        }

        private IEnumerable<double> EstimateMinComp_EdgeLength(CRegion crg)
        {
            throw new ArgumentException("We need to update the cost functions!");

            //double dblEstimatedCompactness
            var EstimateEt = EstimateInteriorLength(crg).GetEnumerator();
            int intEstimationPolygonNum = crg.GetCphCount() - 1;
            while (EstimateEt.MoveNext())
            {
                double dblEdgeLength = crg.dblExteriorSegmentLength + 2 * EstimateEt.Current;
                yield return EstimateCompactnessEdgeLengthOneCrgInstance(intEstimationPolygonNum--, crg.dblArea, dblEdgeLength);
            }
        }

        private double EstimateCompactnessEdgeLengthOneCrgInstance(int intPatchNum, double dblArea, double dblLength)
        {
            double dblEstimatedCompactness = 2 * Math.Sqrt(Math.PI * intPatchNum * dblArea) / dblLength;

            if (dblEstimatedCompactness > 1)
            {
                return 1;
            }
            return dblEstimatedCompactness;
        }
        #endregion

        #region BalancedEstimatedCompactnessInteriorLength_Basic
        private double BalancedEstimatedCompactnessInteriorLength_Basic(CRegion crg, CRegion lscrg)
        {
            if (crg.GetCphCount() == 1)
            {
                return 0;
            }

            var EstimateEt = EstimateInteriorLength(crg).GetEnumerator();

            //n-t == crg.GetCphCount() - 1
            int intNminusT = crg.GetCphCount() - 1;
            //we include current step so that we can make the ration type/comp
            double dblSumCompValue = crg.dblInteriorSegmentLength / intNminusT;  
            while (EstimateEt.MoveNext())
            {
                intNminusT--;
                dblSumCompValue += EstimateEt.Current / intNminusT;
            }

            return dblSumCompValue / lscrg.dblInteriorSegmentLength;
        }

        /// <summary>
        /// Estimate Interior Lengths for the future steps, without current step
        /// </summary>
        /// <param name="crg"></param>
        /// <param name="lscrg"></param>
        /// <returns></returns>
        private IEnumerable<double> EstimateInteriorLength(CRegion crg)
        {
            int intPolygonNum = crg.GetCphCount();
            var dblSegmentLengthSS = new SortedSet<double>(new CDblCompare());  //items in dblEdgeLengthSS is ordered from largest to smallest
            foreach (var pCorrCphs in crg.AdjCorrCphsSD.Keys)
            {
                dblSegmentLengthSS.Add(pCorrCphs.dblSharedSegmentLength);
            }
            IEnumerator<double> dblSegmentLengthSSEt = dblSegmentLengthSS.GetEnumerator();            

            //we compute from the last step (only one interior boundary) to the current step (many interior boundaries).             
            double dblInteriorLength = 0;
            var dblInteriorLengthLt = new List<double>(intPolygonNum - 2);
            for (int i = 0; i < intPolygonNum - 2; i++)
            {
                dblSegmentLengthSSEt.MoveNext();
                dblInteriorLength += dblSegmentLengthSSEt.Current;

                dblInteriorLengthLt.Add(dblInteriorLength);
            }

            for (int i = 0; i < dblInteriorLengthLt.Count; i++)
            {
                yield return dblInteriorLengthLt[dblInteriorLengthLt.Count - i - 1];
            }
        }


        #endregion


        ////**************estimate the first step considering type changing
        //private double EstimateCompactnessForFirstStep(CRegion crg)
        //{
        //    double dblMaxMinComp = 0;

        //    var pCphTypeIndexSD_Area_CphGID = new SortedSet<CPatch>(crg.GetCphCol(), crg.CphTypeIndexSD_Area_CphGID.Comparer);  
        //    foreach (var pAdjacency_CorrCphs in crg.AdjCorrCphsSD.Keys)
        //    {                              
        //        pCphTypeIndexSD_Area_CphGID.Remove(pAdjacency_CorrCphs.FrCph);
        //        pCphTypeIndexSD_Area_CphGID.Remove(pAdjacency_CorrCphs.ToCph);

        //        var newcph = pAdjacency_CorrCphs.FrCph.Unite(pAdjacency_CorrCphs.ToCph, pAdjacency_CorrCphs.dblSharedSegmentLength);
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
        //    dblSumCompValue +=  EstimateCompactnessForOneCrgInstance(intEdgeCountAtmost);

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
        //    NewCrg.dblCostEstimatedType = ParentCrg.dblCostEstimatedType + intFactor*passivecph.dblArea * (padblTD[intactiveTypeIndex, intFinalTypeIndex] - padblTD[intpassiveTypeIndex, intFinalTypeIndex]);



        //    if (CConstants.strShapeConstraint == "MaximizeMinArea")
        //    {
        //        //NewCrg.dblCostExactArea = ParentCrg.dblCostExactArea + passivecph.dblArea * ComputeCostArea(NewCrg.CphTypeIndexSD.Keys, NewCrg.dblArea);
        //        //NewCrg.dblCostEstimatedArea = intFactor*EstimateSumMinArea(NewCrg);  //will we do this twice????

        //        //NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblCostExactArea;
        //        //NewCrg.dblCo stEstimated = NewCrg.dblCostEstimatedType + NewCrg.dblCostEstimatedArea;
        //    }
        //    else if (CConstants.strShapeConstraint == "MaximizeAvgComp")
        //    {
        //        NewCrg.dblSumComp = ParentCrg.dblSumComp - activecph.dblArea * activecph.dblComp - passivecph.dblArea * passivecph.dblComp + unitedcph.dblArea * unitedcph.dblComp;

        //        if (intTimeNum > 2)
        //        {
        //            NewCrg.dblCostExactCompactness = ParentCrg.dblCostExactCompactness + NewCrg.dblSumComp / (intTimeNum - 1);   //divide by intTimeNum because at each step, the value for AvgComp can be 1; intTimeNum - 1 == LSCrg.GetCphCount() - 2
        //        }
        //        else
        //        {
        //            NewCrg.dblCostExactCompactness = 0;
        //        }
        //        NewCrg.dblCostEstimatedCompactness = intFactor * EstimateAvgCompes(NewCrg, intTimeNum);


        //        NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblCostExactCompactness;
        //        NewCrg.dblCostEstimated = NewCrg.dblCostEstimatedType + NewCrg.dblCostEstimatedCompactness;
        //    }
        //    else if (CConstants.strShapeConstraint == "NonShape")
        //    {
        //        NewCrg.dblCostExact = NewCrg.dblCostExactType;
        //        NewCrg.dblCostEstimated = NewCrg.dblCostEstimatedType;
        //    }

        //    //double dblWeight = 0.5;
        //    NewCrg.d = NewCrg.dblCostExact + NewCrg.dblCostEstimated;
        //}

        //public double EstimateSumMinArea(CRegion pCrg)
        //{
        //    pCrg.SetInitialAdjacencyForCph();
        //    SortedSet<CPatch> tempCphAreaSS = new SortedSet<CPatch>(pCrg.CphTypeIndexSD.Keys, CPatch.pCompareCPatch_Area_CphGID);


        //    double dblEstimateSumMinArea = 0;
        //    for (int i = 0; i < pCrg.CphTypeIndexSD.Count - 1; i++)
        //    {
        //        var min1 = tempCphAreaSS.Min;  //get the smallest one
        //        tempCphAreaSS.Remove(min1);
        //        var min2 = min1.Adjacent_CphsCEdgeLtSD.GetFirstT().Key;
        //        tempCphAreaSS.Remove(min2);

        //        var unitedcph = min1.Unite(min2);
        //        unitedcph.Adjacent_CphsCEdgeLtSD = new SortedDictionary<CPatch, List<CEdge>>(CPatch.pCompareCPatch_Area_CphGID);
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
        //        cph.Adjacent_CphsCEdgeLtSD = new SortedDictionary<CPatch, List<CEdge>>(CPatch.pCompareCPatch_Area_CphGID);
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
