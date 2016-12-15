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

        public SortedDictionary<CCorrCphs, CCorrCphs> Adjacency_CorrCphsSD { get; set; }  //compare GID of CorrCphs

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
        public double dblMinCompactness { get; set; } 

        private double _dbld = double.MaxValue;
        public double d
        {
            get { return _dbld; }
            set
            {
                if ((value < 0) || (value > double.MaxValue / 3))
                {
                    MessageBox.Show("incorrect value!");
                    //throw new ArgumentException("incorrect value!");
                    //double  ss = value;
                }

                _dbld = value;
            }
        }

        public CRegion parent { get; set; }
        public CRegion child { get; set; }
        public CEnumColor cenumColor { get; set; }

        public double dblArea { get; set; }
        public double dblSumCompactness { get; set; }

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
            if (CConstants.blnComputeCompactness == true)
            {
                this.dblMinCompactness = double.MaxValue;
            }

            //intID==-2 is for a temporary Crg, and thus should not be counted
            if (intID==-2)
            {
                _intStaticGID--;
            }
            //this.d = double.MaxValue;
            
            this.parent = null;
            this.cenumColor = CEnumColor.white;
        }


        public int GetCphCount()
        {
            return this.CphTypeIndexSD_Area_CphGID.Count;
        }

        public void AddCph(CPatch Cph, int pintTypeIndex)
        {
            this.CphTypeIndexSD_Area_CphGID.Add(Cph, pintTypeIndex);
            
            this.intSumCphGID += Cph.GID;
            this.intSumTypeIndex += pintTypeIndex;
            this.dblArea += Cph.dblArea;

            if (CConstants.blnComputeCompactness == true)
            {
                this.dblMinCompactness = Math.Min(this.dblMinCompactness, Cph.dblCompactness);
            }
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
            var ExistingCorrCphsSD0 = new SortedDictionary<CCorrCphs, CCorrCphs>(CCorrCphs.pCompareCCorrCphs_CphsGID); // we need this variable here, because it has different comparator with pAdjacency_CorrCphsSD
            SortedDictionary<CEdge, CPatch> cedgeSD = new SortedDictionary<CEdge, CPatch>(new CCompareCEdgeCoordinates());  //why SortedDictionary? Because we want to get the value of an element. The element may have the same key with another element.
            var pAdjacency_CorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>();

            if (this .GetCphCount() > 1)
            {
                foreach (var cph in this.CphTypeIndexSD_Area_CphGID.Keys)
                {
                    foreach (var cedgelt in cph.CpgSS.Min.CEdgeLtLt)
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

                                    pAdjacency_CorrCphsSD.Add(CorrCphs, CorrCphs);
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
            //to use less memory, we don't save the shared edgelist. in the method of MaximizeMinimumArea, we don't need the shared edgelist.
            foreach (var item in pAdjacency_CorrCphsSD)
            {
                item.Value.SharedCEdgeLt.Clear();
            }
            //******************************************************************************************************//
            foreach (var cphkvp in CphTypeIndexSD_Area_CphGID)
            {
                cphkvp.Key.AdjacentCphSS = new SortedSet<CPatch>();
            }
            foreach (var pAdjacency_CorrCphs in pAdjacency_CorrCphsSD.Keys)
            {
                pAdjacency_CorrCphs.FrCph.AdjacentCphSS.Add(pAdjacency_CorrCphs.ToCph);
                pAdjacency_CorrCphs.ToCph.AdjacentCphSS.Add(pAdjacency_CorrCphs.FrCph);
            }

            this.Adjacency_CorrCphsSD = pAdjacency_CorrCphsSD;
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
            var pAdjacency_CorrCphsSD = this.Adjacency_CorrCphsSD;
            var mincph = this.CphTypeIndexSD_Area_CphGID.GetFirstT().Key;

            //for every pair of neighboring Cphs, we aggregate them and generate a new Crg
            foreach (var unitingCorrCphs in pAdjacency_CorrCphsSD.Keys)
            {
                CPatch neighborcph = TryGetNeighbor(mincph, unitingCorrCphs);
                if (neighborcph == null)
                {
                    continue;
                }

                foreach (var item in AggregateAndUpdateQ_Common(lscrg, sscrg, Q, pAdjacency_CorrCphsSD, unitingCorrCphs, ExistingCrgSDLt, 
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
            var pAdjacency_CorrCphsSD = this.Adjacency_CorrCphsSD;

            //for every pair of neighboring Cphs, we aggregate them and generate a new Crg
            foreach (var unitingCorrCphs in pAdjacency_CorrCphsSD.Keys)
            {
                foreach (var item in AggregateAndUpdateQ_Common(lscrg, sscrg, Q, pAdjacency_CorrCphsSD, unitingCorrCphs, ExistingCrgSDLt, 
                    ExistingCphSDLt, ExistingCorrCphsSD, intFinalTypeIndex, padblTD, intFactor))
                {
                    yield return item;
                }
            }
        }

        private IEnumerable<CRegion> AggregateAndUpdateQ_Common(CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q, SortedDictionary<CCorrCphs, 
            CCorrCphs> pAdjacency_CorrCphsSD, CCorrCphs unitingCorrCphs, List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, 
            List<SortedDictionary<CPatch, CPatch>> ExistingCphSDLt, SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, 
            int intFinalTypeIndex, double[,] padblTD, int intFactor)
        {
            var newAdjacency_CorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>(pAdjacency_CorrCphsSD);  //make a copy
            newAdjacency_CorrCphsSD.Remove(unitingCorrCphs);
            var newcph = ComputeNewCph(unitingCorrCphs, ExistingCphSDLt);


            //update the adjacencies of the two united cph
            List<CCorrCphs> RemoveKeyLt;  //as we are not allowed to remove in the foreach loop, we record the elments needed to remove and do it later
            List<CCorrCphs> AddKeyLt;
            ComputeAddAndRemoveKeyLt(newAdjacency_CorrCphsSD, unitingCorrCphs, newcph, out AddKeyLt, out RemoveKeyLt);
            SortedDictionary<CCorrCphs, CCorrCphs> tempAdjacencyCorrCphsSD = ComputeTempAdjacencyCorrCphsSD(AddKeyLt, ExistingCorrCphsSD);
            UpdateNewAdjacency_CorrCphsSDAndExistingCorrCphsSD(newAdjacency_CorrCphsSD, ExistingCorrCphsSD, RemoveKeyLt, tempAdjacencyCorrCphsSD);


            var frcph = unitingCorrCphs.FrCph;
            var tocph = unitingCorrCphs.ToCph;
            int intfrTypeIndex;
            int inttoTypeIndex;
            this.CphTypeIndexSD_Area_CphGID.TryGetValue(frcph, out intfrTypeIndex);
            this.CphTypeIndexSD_Area_CphGID.TryGetValue(tocph, out inttoTypeIndex);

            //if the two cphs have the same type, then we only need to aggregate the smaller one into the larger one (this will certainly have smaller cost in terms of area)
            //otherwise, we need to aggregate from two directions
            if (intfrTypeIndex == inttoTypeIndex)
            {
                yield return GenerateCrg(lscrg, sscrg, Q, ExistingCrgSDLt, newAdjacency_CorrCphsSD, frcph, tocph, newcph, unitingCorrCphs, 
                    intFinalTypeIndex, padblTD, intFactor);
            }
            else
            {
                //The aggregate can happen from two directions
                yield return GenerateCrg(lscrg, sscrg, Q, ExistingCrgSDLt, newAdjacency_CorrCphsSD, frcph, tocph, newcph, unitingCorrCphs, 
                    intFinalTypeIndex, padblTD, intFactor);
                yield return GenerateCrg(lscrg, sscrg, Q, ExistingCrgSDLt, newAdjacency_CorrCphsSD, tocph, frcph, newcph, unitingCorrCphs, 
                    intFinalTypeIndex, padblTD, intFactor);
            }
        }

        private CPatch TryGetNeighbor(CPatch mincph, CCorrCphs unitingCorrCphs)
        {
            if (mincph.CompareTo(unitingCorrCphs.FrCph) == 0)
            {
                return unitingCorrCphs.ToCph;
            }
            else if (mincph.CompareTo(unitingCorrCphs.ToCph) == 0)
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

        private void ComputeAddAndRemoveKeyLt(SortedDictionary<CCorrCphs, CCorrCphs> newAdjacency_CorrCphsSD, CCorrCphs unitingCorrCphs, 
            CPatch newcph, out List<CCorrCphs> AddKeyLt, out List<CCorrCphs> RemoveKeyLt)
        {
            RemoveKeyLt = new List<CCorrCphs>();  //as we are not allowed to remove in the foreach loop, we record the elments needed to remove and do it later
            AddKeyLt = new List<CCorrCphs>();
            foreach (var pCorrCphs in newAdjacency_CorrCphsSD.Keys)
            {
                if (pCorrCphs.FrCph.CompareTo(unitingCorrCphs.FrCph) == 0 || pCorrCphs.FrCph.CompareTo(unitingCorrCphs.ToCph) == 0)
                {
                    RemoveKeyLt.Add(pCorrCphs);
                    AddKeyLt.Add(new CCorrCphs(pCorrCphs.ToCph, newcph, pCorrCphs));
                }
                else if (pCorrCphs.ToCph.CompareTo(unitingCorrCphs.FrCph) == 0 || pCorrCphs.ToCph.CompareTo(unitingCorrCphs.ToCph) == 0)
                {
                    RemoveKeyLt.Add(pCorrCphs);
                    AddKeyLt.Add(new CCorrCphs(pCorrCphs.FrCph, newcph, pCorrCphs));
                }
            }

        }

        private SortedDictionary<CCorrCphs, CCorrCphs> ComputeTempAdjacencyCorrCphsSD(List<CCorrCphs> AddKeyLt, 
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD)
        {
            var tempAdjacencyCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>(CCorrCphs.pCompareCCorrCphs_CphsGID);

            foreach (var AddKey in AddKeyLt)
            {
                CCorrCphs AddKeyCorrCphs;
                bool isExisting = ExistingCorrCphsSD.TryGetValue(AddKey, out AddKeyCorrCphs);


                CCorrCphs newAdjacentCorrCphs;
                bool isAdjacencyExisting = tempAdjacencyCorrCphsSD.TryGetValue(AddKey, out newAdjacentCorrCphs);

                if (isExisting == true && isAdjacencyExisting == true)
                {
                    //do nothing
                }
                else if (isExisting == true && isAdjacencyExisting == false)
                {
                    tempAdjacencyCorrCphsSD.Add(AddKeyCorrCphs, AddKeyCorrCphs);
                }
                else if (isExisting == false && isAdjacencyExisting == true) //AddKeyLt[i] already exists. this means that AddKeyLt[i].FrCph was adjacent to both the united cphs. in this case we simply combine the shared edges
                {
                    newAdjacentCorrCphs.SharedCEdgeLt.AddRange(AddKey.SharedCEdgeLt);
                    newAdjacentCorrCphs.dblSharedSegmentLength += AddKey.dblSharedSegmentLength;
                    newAdjacentCorrCphs.intSharedCEdgeCount += AddKey.intSharedCEdgeCount;
                }
                else //if (isExisting == false && isAdjacencyExisting == false)
                {
                    tempAdjacencyCorrCphsSD.Add(AddKey, AddKey);
                }
            }

            return tempAdjacencyCorrCphsSD;
        }

        private void UpdateNewAdjacency_CorrCphsSDAndExistingCorrCphsSD(SortedDictionary<CCorrCphs, CCorrCphs> newAdjacency_CorrCphsSD, 
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, List<CCorrCphs> RemoveKeyLt, 
            SortedDictionary<CCorrCphs, CCorrCphs> tempAdjacencyCorrCphsSD)
        {
            foreach (var RemoveKey in RemoveKeyLt)
            {
                newAdjacency_CorrCphsSD.Remove(RemoveKey);
            }

            foreach (var tempAdjacencyCorrCphsKvp in tempAdjacencyCorrCphsSD)
            {
                newAdjacency_CorrCphsSD.Add(tempAdjacencyCorrCphsKvp.Key, tempAdjacencyCorrCphsKvp.Value);

                if (ExistingCorrCphsSD.ContainsKey(tempAdjacencyCorrCphsKvp.Key) == false)
                {
                    ExistingCorrCphsSD.Add(tempAdjacencyCorrCphsKvp.Key, tempAdjacencyCorrCphsKvp.Value);
                }
            }
        }

        /// <summary>
        /// compute cost during generating a new Crg
        /// </summary>
        /// <param name="newAdjacency_CorrCphsSD"></param>
        /// <param name="activecph"></param>
        /// <param name="passivecph"></param>
        /// <param name="unitedcph"></param>
        /// <param name="intFinalTypeIndex"></param>
        /// <param name="padblTD"></param>
        /// <returns></returns>
        private CRegion GenerateCrg(CRegion lscrg, CRegion sscrg, SortedSet<CRegion> Q, List<SortedDictionary<CRegion, CRegion>> ExistingCrgSDLt, 
            SortedDictionary<CCorrCphs, CCorrCphs> newAdjacency_CorrCphsSD, CPatch activecph, CPatch passivecph, CPatch unitedcph, 
            CCorrCphs unitingCorrCphs, int intFinalTypeIndex, double[,] padblTD, int intFactor)
        {
            var newCphTypeIndexSD = new SortedDictionary<CPatch, int>(this.CphTypeIndexSD_Area_CphGID, CPatch.pCompareCPatch_Area_CphGID);
            int intactiveTypeIndex;
            int intpassiveTypeIndex;
            newCphTypeIndexSD.TryGetValue(activecph, out intactiveTypeIndex);
            newCphTypeIndexSD.TryGetValue(passivecph, out intpassiveTypeIndex);


            newCphTypeIndexSD.Remove(activecph);
            newCphTypeIndexSD.Remove(passivecph);
            newCphTypeIndexSD.Add(unitedcph, intactiveTypeIndex);


            CRegion newcrg = new CRegion(this.ID);
            newcrg.dblArea = this.dblArea;
            newcrg.cenumColor = CEnumColor.gray;
            newcrg.parent = this;
            newcrg.Adjacency_CorrCphsSD = newAdjacency_CorrCphsSD;
            newcrg.CphTypeIndexSD_Area_CphGID = newCphTypeIndexSD;
            newcrg.intSumCphGID = this.intSumCphGID - activecph.GID - passivecph.GID + unitedcph.GID;
            newcrg.intSumTypeIndex = this.intSumTypeIndex - intpassiveTypeIndex;
            //newcrg.intEdgeCount = this.intEdgeCount - intDecreaseEdgeCount;
            newcrg.intInteriorEdgeCount = this.intInteriorEdgeCount - unitingCorrCphs.intSharedCEdgeCount;            
            newcrg.intExteriorEdgeCount = this.intExteriorEdgeCount;
            newcrg.dblInteriorSegmentLength = this.dblInteriorSegmentLength - unitingCorrCphs.dblSharedSegmentLength;
            newcrg.dblExteriorSegmentLength = this.dblExteriorSegmentLength;

            if (CConstants.blnComputeCompactness == true)
            {
                newcrg.dblMinCompactness = newcrg.CphTypeIndexSD_Area_CphGID.Keys.Min(cph => cph.dblCompactness);
            }

            ComputeExactCost(lscrg, newcrg, activecph, passivecph, unitedcph, intactiveTypeIndex, intpassiveTypeIndex, padblTD);


            CRegion outcrg;
            if (ExistingCrgSDLt[newcrg.GetCphCount()].TryGetValue(newcrg, out outcrg))
            {
                int intResult = newcrg.dblCostExact.CompareTo(outcrg.dblCostExact);
                if (intResult == -1)
                {
                    //from the idea of A* algorithm, we know that outcrg must be in Q
                    Q.Remove(outcrg);  //there is no decrease key function for SortedSet, so we have to remove it and later add it again
                    outcrg.cenumColor = newcrg.cenumColor;
                    outcrg.dblCostExactType = newcrg.dblCostExactType;                  //*****the cost will need be updated if we integrate more evaluations
                    outcrg.dblCostExactCompactness = newcrg.dblCostExactCompactness;                  //*****the cost will need be updated if we integrate more evaluations
                    outcrg.dblCostExactArea = newcrg.dblCostExactArea;
                    outcrg.dblCostExact = newcrg.dblCostExact;
                    outcrg.d = newcrg.dblCostExact + outcrg.dblCostEstimated;

                    outcrg.parent = newcrg.parent;
                    newcrg = outcrg;

                    //CRegion._intStaticGID--;
                }
            }
            else
            {
                ComputeEstimatedCost(lscrg, sscrg, newcrg, activecph, passivecph, unitedcph, intactiveTypeIndex, intpassiveTypeIndex, 
                    intFinalTypeIndex, padblTD, intFactor);

                ExistingCrgSDLt[newcrg.GetCphCount()].Add(newcrg, newcrg);
                CRegion._intNodesCount++;
            }
            Q.Add(newcrg);

            return newcrg;
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

            if (CConstants.strShapeConstraint == "MaximizeMinimumArea")
            {
                //this.dblCostEstimatedArea = intFactor * EstimateSumMinArea(this);
                //this.dblCostEstimated += this.dblCostEstimatedArea;
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinimumCompactness_Combine")
            {
                this.dblCostEstimatedCompactness = intFactor * BalancedEstimatedMinimumCompactness_Combine(this, this, sscrg);
                this.dblCostEstimated +=  CCAMDijkstra.dblLamda * (this.dblArea * this.dblCostEstimatedCompactness);
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinimumCompactness")
            {
                this.dblCostEstimatedCompactness = intFactor * BalancedEstimatedMinimumCompactness_EdgeNumber(this, this, sscrg);
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



            if (CConstants.strShapeConstraint == "MaximizeMinimumArea")
            {
                //NewCrg.dblCostExactArea = ParentCrg.dblCostExactArea + passivecph.dblArea * ComputeCostArea(NewCrg.CphTypeIndexSD.Keys, NewCrg.dblArea);

                //NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblCostExactArea;
            }
            else if (CConstants.strShapeConstraint == "MaximizeAverageCompactness")
            {
                //NewCrg.dblSumCompactness = ParentCrg.dblSumCompactness - activecph.dblArea * activecph.dblCompactness
                //    - passivecph.dblArea * passivecph.dblCompactness + unitedcph.dblArea * unitedcph.dblCompactness;

                //if (NewCrg.GetCphCount() >= 2)
                //{
                //    //divide by intTimeNum - 1, because at each step the value for AverageCompactness can be 1; only when intTimeNum - 1 > 0, we are in this function and run the following codes
                //    NewCrg.dblCostExactCompactness = ParentCrg.dblCostExactCompactness + NewCrg.dblSumCompactness / (intTimeNum - 1);
                //}
                //else
                //{
                //    NewCrg.dblCostExactCompactness = ParentCrg.dblCostExactCompactness;
                //}

                //NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblCostExactCompactness;
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinimumCompactness"
                || CConstants.strShapeConstraint == "MaximizeMinimumCompactness_Combine")
            {
                //divide by intTimeNum - 2, because at each step the value for AverageCompactness can be 1; only when intTimeNum - 1 > 0, we are in this function and run the following codes
                if (lscrg.GetCphCount() - NewCrg.GetCphCount() > 1)  //we have exact cost from t=3
                {
                    NewCrg.dblCostExactCompactness = ParentCrg.dblCostExactCompactness + (1 - ParentCrg.dblMinCompactness) / (intTimeNum - 2);
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
                //divide by intTimeNum - 1, because at each step the value for AverageCompactness can be 1; At the step of ParentCrg intTimeNum - t == NewCrg.GetCphCount();
                NewCrg.dblCostExactCompactness = ParentCrg.dblCostExactCompactness +
                    ParentCrg.dblInteriorSegmentLength / lscrg.dblInteriorSegmentLength / (NewCrg.GetCphCount());
                NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblArea * NewCrg.dblCostExactCompactness;
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


            if (CConstants.strShapeConstraint == "MaximizeMinimumArea")
            {
                //NewCrg.dblCostEstimatedArea = intFactor*EstimateSumMinArea(NewCrg);  //will we do this twice????

                //NewCrg.dblCostEstimated = NewCrg.dblCostEstimatedType + NewCrg.dblCostEstimatedArea;
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinimumCompactness_Comine")
            {
                NewCrg.dblCostEstimatedCompactness = intFactor * BalancedEstimatedMinimumCompactness_Combine(NewCrg, lscrg, sscrg);

                //to make dblCostEstimatedCompactness comparable to dblCostEstimatedType and to avoid digital problems, we time dblCostEstimatedCompactness by area
                //we will adjust the value later
                NewCrg.dblCostEstimated = (1 - CCAMDijkstra.dblLamda) * NewCrg.dblCostEstimatedType + 
                    CCAMDijkstra.dblLamda * NewCrg.dblArea * NewCrg.dblCostEstimatedCompactness;   
            }
            else if (CConstants.strShapeConstraint == "MaximizeMinimumCompactness")
            {
                NewCrg.dblCostEstimatedCompactness = intFactor * BalancedEstimatedMinimumCompactness_EdgeNumber(NewCrg, lscrg, sscrg);

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
        private double BalancedEstimatedMinimumCompactness_Combine(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            if (crg.GetCphCount() == 1)
            {
                return 0;
            }

            var EstimateEdgeNumberEt = EstimateMinimumCompactness_EdgeNumber(crg).GetEnumerator();
            var EstimateEdgeLengthEt = EstimateMinimumCompactness_EdgeLength(crg).GetEnumerator();   //we need to improve this estimation to make sure this is an upper bound

            double dblSumCompactnessValue = 0;
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

                dblSumCompactnessValue += (1 - Math.Min(EstimateEdgeNumberEt.Current, EstimateEdgeLengthEt.Current));
            }

            return dblSumCompactnessValue / (lscrg.GetCphCount() - 1);
        }

        #region EstimateMinimumCompactness_EdgeNumber
        /// <summary>
        /// 
        /// </summary>
        /// <param name="crg">crg.GetCphCount() > 1</param>
        /// <param name="lscrg">lscrg.GetCphCount() > 2</param>
        /// <param name="sscrg"></param>
        /// <returns></returns>
        private double BalancedEstimatedMinimumCompactness_EdgeNumber(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            // if lscrg.GetCphCount() <= 2, the domain only have two polygons
            // if crg.GetCphCount() <= 1, we are at the last step
            if (lscrg.GetCphCount() <= 2)
            {
                return 0;
            }

            return EstimatedMinimumCompactness_Common(EstimateMinimumCompactness_EdgeNumber(crg), crg, lscrg, sscrg);
        }

        private double EstimatedMinimumCompactness_Common(IEnumerable<double> EstimateEb, CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            var EstimateEt = EstimateEb.GetEnumerator();

            //we include current step so that we can make the ration type/comp
            double dblSumCompactnessValue = 0;  //for the current crg, whose dblMinCompactness is known
            while (EstimateEt.MoveNext())
            {
                dblSumCompactnessValue += (1 - EstimateEt.Current);
            }

            return dblSumCompactnessValue / (lscrg.GetCphCount() - 2);
        }

        /// <summary>
        /// from time t to n-1
        /// </summary>
        /// <param name="crg"></param>
        /// <returns></returns>
        private IEnumerable<double> EstimateMinimumCompactness_EdgeNumber(CRegion crg)
        {
            var intEdgeCountSS = new SortedSet<int>(new CIntCompare());
            foreach (var pCorrCphs in crg.Adjacency_CorrCphsSD.Keys)
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

        #region EstimateMinimumCompactness_EdgeLength
        private double BalancedEstimatedMinimumCompactness_EdgeLength(CRegion crg, CRegion lscrg, CRegion sscrg)
        {
            if (crg.GetCphCount() == 1)
            {
                return 0;
            }

            return EstimatedMinimumCompactness_Common(EstimateMinimumCompactness_EdgeLength(crg), crg, lscrg, sscrg);
        }

        private IEnumerable<double> EstimateMinimumCompactness_EdgeLength(CRegion crg)
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
            double dblSumCompactnessValue = crg.dblInteriorSegmentLength / intNminusT;  
            while (EstimateEt.MoveNext())
            {
                intNminusT--;
                dblSumCompactnessValue += EstimateEt.Current / intNminusT;
            }

            return dblSumCompactnessValue / lscrg.dblInteriorSegmentLength;
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
            foreach (var pCorrCphs in crg.Adjacency_CorrCphsSD.Keys)
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
        //    double dblMaxMinCompactness = 0;

        //    var pCphTypeIndexSD_Area_CphGID = new SortedSet<CPatch>(crg.CphTypeIndexSD_Area_CphGID.Keys, crg.CphTypeIndexSD_Area_CphGID.Comparer);  
        //    foreach (var pAdjacency_CorrCphs in crg.Adjacency_CorrCphsSD.Keys)
        //    {                              
        //        pCphTypeIndexSD_Area_CphGID.Remove(pAdjacency_CorrCphs.FrCph);
        //        pCphTypeIndexSD_Area_CphGID.Remove(pAdjacency_CorrCphs.ToCph);

        //        var newcph = pAdjacency_CorrCphs.FrCph.Unite(pAdjacency_CorrCphs.ToCph, pAdjacency_CorrCphs.dblSharedSegmentLength);
        //        pCphTypeIndexSD_Area_CphGID.Add(newcph);

        //        dblMaxMinCompactness = Math.Max(dblMaxMinCompactness, pCphTypeIndexSD_Area_CphGID.Min(cph => cph.dblCompactness));

        //        pCphTypeIndexSD_Area_CphGID.Remove(newcph);
        //        pCphTypeIndexSD_Area_CphGID.Add(pAdjacency_CorrCphs.FrCph);
        //        pCphTypeIndexSD_Area_CphGID.Add(pAdjacency_CorrCphs.ToCph);
        //    }

        //    return dblMaxMinCompactness;
        //}


        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="crg"></param>
        ///// <returns></returns>
        ///// <remarks></remarks>
        //private double EstimateAverageCompactnesses(CRegion crg, int intTimeNum)
        //{
        //var intEdgeCountSS = new SortedSet<int>(new CIntCompare());
        //foreach (var pCorrCphs in crg.Adjacency_CorrCphsSD.Keys)
        //{
        //    intEdgeCountSS.Add(pCorrCphs.intSharedCEdgeCount);
        //}

        //int intEdgeCountAtmost = crg.intExteriorEdgeCount + crg.intInteriorEdgeCount;
        //int intLeftStepNum = crg.GetCphCount() - 1;
        //int intCount = intLeftStepNum;
        //double dblSumCompactnessValue = 0;
        //foreach (var intEdgeCount in intEdgeCountSS)
        //{
        //    if (intCount <= 1)
        //    {
        //        break;
        //    }

        //    intEdgeCountAtmost -= intEdgeCount;
        //    dblSumCompactnessValue +=  EstimateCompactnessForOneCrgInstance(intEdgeCountAtmost);

        //    intCount--;
        //}

        //dblSumCompactnessValue *= crg.dblArea;  //the patch with largest campactness can be as large as the whole domain

        //if (dblSumCompactnessValue > 0)
        //{
        //    return dblSumCompactnessValue / (intTimeNum - 1);
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



        //    if (CConstants.strShapeConstraint == "MaximizeMinimumArea")
        //    {
        //        //NewCrg.dblCostExactArea = ParentCrg.dblCostExactArea + passivecph.dblArea * ComputeCostArea(NewCrg.CphTypeIndexSD.Keys, NewCrg.dblArea);
        //        //NewCrg.dblCostEstimatedArea = intFactor*EstimateSumMinArea(NewCrg);  //will we do this twice????

        //        //NewCrg.dblCostExact = NewCrg.dblCostExactType + NewCrg.dblCostExactArea;
        //        //NewCrg.dblCo stEstimated = NewCrg.dblCostEstimatedType + NewCrg.dblCostEstimatedArea;
        //    }
        //    else if (CConstants.strShapeConstraint == "MaximizeAverageCompactness")
        //    {
        //        NewCrg.dblSumCompactness = ParentCrg.dblSumCompactness - activecph.dblArea * activecph.dblCompactness - passivecph.dblArea * passivecph.dblCompactness + unitedcph.dblArea * unitedcph.dblCompactness;

        //        if (intTimeNum > 2)
        //        {
        //            NewCrg.dblCostExactCompactness = ParentCrg.dblCostExactCompactness + NewCrg.dblSumCompactness / (intTimeNum - 1);   //divide by intTimeNum because at each step, the value for AverageCompactness can be 1; intTimeNum - 1 == LSCrg.GetCphCount() - 2
        //        }
        //        else
        //        {
        //            NewCrg.dblCostExactCompactness = 0;
        //        }
        //        NewCrg.dblCostEstimatedCompactness = intFactor * EstimateAverageCompactnesses(NewCrg, intTimeNum);


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

        //    foreach (var kvp in this.Adjacency_CorrCphsSD)
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
