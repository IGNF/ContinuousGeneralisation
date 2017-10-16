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
        public static CCmpCRegion_Cost_CphGIDTypeIndex pCmpCRegion_Cost_CphGIDTypeIndex 
            = new CCmpCRegion_Cost_CphGIDTypeIndex();  //this variable should be used for the queue Q

        //this comparer should be used for integrate the sequences for output 
        public static CCmpCRegion_MinArea_CphGIDTypeIndex pCmpCRegion_MinArea_CphGIDTypeIndex 
            = new CCmpCRegion_MinArea_CphGIDTypeIndex();

        //this comparer should be used for integrate the sequences for output 
        public static CCmpCRegion_CostExact_CphGIDTypeIndex pCmpCRegion_CostExact_CphGIDTypeIndex 
            = new CCmpCRegion_CostExact_CphGIDTypeIndex();

        //this comparer should be used for checking existing Crgs
        public static CCmpCRegion_CphGIDTypeIndex pCmpCRegion_CphGIDTypeIndex = new CCmpCRegion_CphGIDTypeIndex();

        //this comparer should be used for counting uncolored Crgs
        public static CCmpCRegion_CphGID pCmpCRegion_CphGID 
            = new CCmpCRegion_CphGID();  //this variable should be used for CRegion itself


        public SortedDictionary<CCorrCphs, CCorrCphs> AdjCorrCphsSD { get; set; }  //compare GID of CorrCphs

        //Why did I use SortedDictionary? We use this comparator CPatch .pCmpCPatch_Area_CphGID
        public SortedDictionary<CPatch, int> CphTypeIndexSD_Area_CphGID { get; set; } 

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
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.sComparer);
                _dblCostEstType = value;
                if (value < 0)
                {
                    _dblCostEstType = 0;
                }
            }
        }

        public double _dblCostEstComp = 0;
        public double dblCostEstComp
        {
            get { return _dblCostEstComp; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.sComparer);
                _dblCostEstComp = value;
                if (value < 0)
                {
                    _dblCostEstComp = 0;
                }
            }
        }

        public double _dblCostExactType = 0;
        public double dblCostExactType
        {
            get { return _dblCostExactType; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.sComparer);
                _dblCostExactType = value;
                if (value < 0)
                {
                    _dblCostExactType = 0;
                }
            }
        }

        public double _dblCostExactComp = 0;
        public double dblCostExactComp
        {
            get { return _dblCostExactComp; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.sComparer);
                _dblCostExactComp = value;
                if (value < 0)
                {
                    _dblCostExactComp = 0;
                }
            }
        }

        private double _dblCostExact = 0;
        public double dblCostExact
        {
            get { return _dblCostExact; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.sComparer);
                _dblCostExact = value;
                if (value < 0)
                {
                    _dblCostExact = 0;
                }
            }
        }

        private double _dblCostEst = 0;
        public double dblCostEst
        {
            get { return _dblCostEst; }
            set
            {
                CHelpFunc.InBoundOrReport(value, 0, CConstants.dblVeryLarge, CCmpCoordDbl_VerySmall.sComparer);
                _dblCostEst = value;
                if (value < 0)
                {
                    _dblCostEst = 0;
                }
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
            return this.CphTypeIndexSD_Area_CphGID.First().Key;
        }

        public int GetSoloCphTypeIndex()
        {
            if (this.CphTypeIndexSD_Area_CphGID.Count > 1)
            {
                throw new ArgumentOutOfRangeException("There are more than one elements!");
            }

            return this.CphTypeIndexSD_Area_CphGID.First().Value;
        }



        public IEnumerable<CCphRecord> GetNeighborCphRecords(CPatch cph)
        {
            foreach (var pCorrCphs in this.AdjCorrCphsSD.Keys)
            {
                var neighborcph = TryGetNeighbor(cph, pCorrCphs);
                if (neighborcph != null)
                {
                    var cphRecord = new CCphRecord(neighborcph, pCorrCphs);
                    yield return cphRecord;
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
            // we need this variable here, because it has different comparator with pAdjCorrCphsSD
            var ExistingCorrCphsSD0 = new SortedDictionary<CCorrCphs, CCorrCphs>(CCorrCphs.pCmpCCorrCphs_CphsGID);
            //why SortedDictionary? Because we want to get the value of an element. 
            //The element may have the same key with another element.
            SortedDictionary<CEdge, CPatch> cedgeSD = new SortedDictionary<CEdge, CPatch>(new CCmpCEdgeCoordinates());  
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

                            //if cedge already exists in cedgeSD, then we now have found the Patch which shares this cedge
                            if (isShareEdge == true)  
                            {
                                CCorrCphs ExsitedCorrCphs;   //SharedEdgesLk belongs to cph.AdjacentSD
                                var CorrCphs = new CCorrCphs(cph, EdgeShareCph);

                                //whether we have already known that cph and EdgeShareCph are adjacent patches
                                bool isKnownAdjacent = ExistingCorrCphsSD0.TryGetValue(CorrCphs, out ExsitedCorrCphs);  
                                if (isKnownAdjacent == true)
                                {
                                    ExsitedCorrCphs.SharedCEdgeLt.Add(cedge);
                                    ExsitedCorrCphs.dblSharedSegLength += cedge.dblLength;
                                    ExsitedCorrCphs.intSharedCEdgeCount++;
                                }
                                else
                                {
                                    //List<CEdge> NewCphSharedEdgesLt = new List<CEdge>();
                                    //CorrCphs.SharedCEdgeLt = new List<CEdge>();
                                    CorrCphs.SharedCEdgeLt.Add(cedge);
                                    CorrCphs.dblSharedSegLength += cedge.dblLength;
                                    CorrCphs.intSharedCEdgeCount++;

                                    pAdjCorrCphsSD.Add(CorrCphs, CorrCphs);
                                    ExistingCorrCphsSD0.Add(CorrCphs, CorrCphs);
                                }

                                this.intInteriorEdgeCount++;
                                this.dblInteriorSegLength += cedge.dblLength;
                                //every edge belongs to two polygons, if we have found the two polygons, 
                                //we can remove the shared edge from the SortedDictionary
                                cedgeSD.Remove(cedge);    
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
            //to use less memory, we don't save the shared edgelist. in the method of MaximizeMinArea, 
            //we don't need the shared edgelist.
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

        public CPatch ComputeNewCph(CCorrCphs unitingCorrCphs, List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt)
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

            //pNewAdjCorrCphsLt_WithoutReplaced is list of AdjCorrCphs without the patches related to uniting
            List<CCorrCphs> AddKeyLt;  //CCorrCphs that are new keys for new AdjCorrCphsSD
            var pNewAdjCorrCphsLt_WithoutReplaced =
                ComputeNewAdjCorrCphsSD_WithoutReplaced(pAdjCorrCphsSD, unitingCorrCphs, newcph, out AddKeyLt);
            var incrementalAdjCorrCphsSD = ComputeIncrementalAdjCorrCphsSD(AddKeyLt, ExistingCorrCphsSD);

            var NewAdjCorrCphsSD= GenerateNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD
                (pNewAdjCorrCphsLt_WithoutReplaced, ExistingCorrCphsSD, incrementalAdjCorrCphsSD);
            return NewAdjCorrCphsSD;
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
                else if (pCorrCphs.FrCph.CompareTo(unitingCorrCphs.FrCph) == 0 ||
                         pCorrCphs.FrCph.CompareTo(unitingCorrCphs.ToCph) == 0)
                {
                    AddKeyLt.Add(new CCorrCphs(pCorrCphs.ToCph, newcph, pCorrCphs));
                }
                else if (pCorrCphs.ToCph.CompareTo(unitingCorrCphs.FrCph) == 0 ||
                         pCorrCphs.ToCph.CompareTo(unitingCorrCphs.ToCph) == 0)
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

        /// <summary>
        /// compute the relationships between the new patch and its surrounding patches
        /// </summary>
        /// <param name="AddKeyLt"></param>
        /// <param name="ExistingCorrCphsSD"></param>
        /// <returns></returns>
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
                else if (isExisting == false && isAdjExisting == true)
                //AddKeyLt[i] already exists. this means that AddKeyLt[i].FrCph was adjacent to both the united cphs. 
                //in this case we simply combine the shared edges
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

        private static SortedDictionary<CCorrCphs, CCorrCphs> GenerateNewAdjCorrCphsSDAndUpdateExistingCorrCphsSD(
            List<CCorrCphs> pNewAdjCorrCphsLt_WithoutReplaced,
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


        

        public CRegion GenerateCrgChildAndComputeCost(CRegion lscrg, SortedDictionary<CCorrCphs, CCorrCphs> newAdjCorrCphsSD,
            CPatch activecph, CPatch passivecph, CPatch unitedcph, CCorrCphs unitingCorrCphs, double[,] padblTD)
        {
            var intactiveTypeIndex = this.GetCphTypeIndex(activecph);
            var intpassiveTypeIndex = this.GetCphTypeIndex(passivecph);

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

            double dblTypeCost = passivecph.dblArea * padblTD[intactiveTypeIndex, intpassiveTypeIndex];
            ComputeExactCost(lscrg, newcrg, dblTypeCost);
            
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



        public void ComputeExactCost(CRegion lscrg, CRegion NewCrg, double dblTypeCost)
        {
            var ParentCrg = NewCrg.parent;
            NewCrg.dblCostExactType = ParentCrg.dblCostExactType + dblTypeCost;
            var intTimeNum = lscrg.GetCphCount();


            if (CConstants.strShapeConstraint == "MaximizeMinArea")
            {
                //NewCrg.dblCostExactArea = ParentCrg.dblCostExactArea + passivecph.dblArea 
                //* ComputeCostArea(NewCrg.CphTypeIndexSD.Keys, NewCrg.dblArea);

                //NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblCostExactArea;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAvgComp_EdgeNumber" ||
                CConstants.strShapeConstraint == "MaximizeAvgComp_Combine")
            {
                //divide by intTimeNum - 2, because at each step the value for AvgComp can be 1 and 
                //there are intotal intTimeNum - 2 steps; 
                //only when intTimeNum - 1 > 0, we are in this function and run the following codes
                if (intTimeNum - NewCrg.GetCphCount() > 1)  //we have exact cost from t=3
                {
                    NewCrg.dblCostExactComp = ParentCrg.dblCostExactComp + (1 - ParentCrg.dblAvgComp) / (intTimeNum - 2);
                }
                else
                {
                    NewCrg.dblCostExactComp = 0;
                }

                NewCrg.dblCostExact = (1 - CAreaAgg_Base.dblLamda) * NewCrg.dblCostExactType +
                    CAreaAgg_Base.dblLamda * NewCrg.dblArea * NewCrg.dblCostExactComp;
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinComp_EdgeNumber"
                || CConstants.strShapeConstraint == "MaximizeMinComp_Combine")
            {
                //divide by intTimeNum - 2, because at each step the value for AvgComp can be 1 and 
                //there are intotal intTimeNum - 2 steps; 
                //only when intTimeNum - 1 > 0, we are in this function and run the following codes
                if (intTimeNum - NewCrg.GetCphCount() > 1)  //we have exact cost from t=3
                {
                    NewCrg.dblCostExactComp = ParentCrg.dblCostExactComp + (1 - ParentCrg.dblMinComp) / (intTimeNum - 2);
                }
                else
                {
                    NewCrg.dblCostExactComp = 0;
                }

                NewCrg.dblCostExact = (1 - CAreaAgg_Base.dblLamda) * NewCrg.dblCostExactType +
                    CAreaAgg_Base.dblLamda * NewCrg.dblArea * NewCrg.dblCostExactComp;
            }
            else if (CConstants.strShapeConstraint == "MinimizeInteriorBoundaries")
            {
                //divide by intTimeNum - 2, because at each step the value for InteriorSegLength can be 1 and 
                //there are intotal intTimeNum - 2 steps; 
                //only when intTimeNum - 1 > 0, we are in this function and run the following codes
                if (intTimeNum - NewCrg.GetCphCount() > 1)  //we have exact cost from t=3
                {
                    // lscrg.dblInteriorSegLength * NewCrg.GetCphCount() / (intTimeNum - 1) 
                    //is the expected interior length at time t-1
                    NewCrg.dblCostExactComp = ParentCrg.dblCostExactComp +
                    ParentCrg.dblInteriorSegLength
                    / (lscrg.dblInteriorSegLength * NewCrg.GetCphCount() / (intTimeNum - 1))
                    / (intTimeNum - 2);
                }
                else
                {
                    NewCrg.dblCostExactComp = 0;
                }

                NewCrg.dblCostExact = (1 - CAreaAgg_Base.dblLamda) * NewCrg.dblCostExactType +
                    CAreaAgg_Base.dblLamda * NewCrg.dblArea * NewCrg.dblCostExactComp;
            }
            else if (CConstants.strShapeConstraint == "NonShape")
            {
                NewCrg.dblCostExact = NewCrg.dblCostExactType;
            }

        }


        


        
        
    }
}
