using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geometry;

using MorphingClass.CUtility;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CGeometry
{
    public class CPatch : CLandCoverBase
    {
        private static int _intStaticGID;
        public static CCmpCPatch_CpgGID pCmpCPatch_CpgGID = new CCmpCPatch_CpgGID();  //this variable should be used for CPatch itself
        public static CCmpCPatch_Area_CphGID pCmpCPatch_Area_CphGID = new CCmpCPatch_Area_CphGID();  //this variable should be used for CPatch itself
        public static CCmpCPatch_Compactness_CphGID pCmpCPatch_Compactness_CphGID = new CCmpCPatch_Compactness_CphGID();

        private IPolygon4 _pPolygon;
        public IPolygon4 pPolygon
        {
            get { return _pPolygon; }
            set
            {
                _pPolygon = value;
                //_pGeo = value;
            }
        }

        public SortedSet<CPolygon> CpgSS { get; set; }
        public int intSumCpgGID { get; set; }

        /// <summary>this is the real compactness 2*Sqrt(pi*A)/L.</summary>
        public double dblComp { get; set; }

        public double dblArea { get; set; }
        public double dblLength { get; set; }

        public bool isCore { get; set; }

        //public SortedDictionary<CPatch , List<CEdge>> Adjacent_CphsCEdgeLtSD { get; set; }  //only temporary for compute area estimation

        public SortedSet<CPatch> AdjacentCphSS { get; set; }  //only for the ILP


        public CPatch()
        {
            this.GID = _intStaticGID++;
        }



        public CPatch(CPolygon cpg, int intID, int pintTypeIndex)
            : this()
        {
            this.ID = intID;
            this.CpgSS = new SortedSet<CPolygon>();
            this.CpgSS.Add(cpg);
            this.dblArea = (cpg.pPolygon as IArea).Area;
            this.dblLength = cpg.pPolygon.Length;
            this.intSumCpgGID = cpg.GID;
            this.intTypeIndex = pintTypeIndex;

            if (CConstants.blnComputeMinComp == true || CConstants.blnComputeAvgComp == true)
            {
                this.dblComp = CGeoFunc.CalCompactness(this.dblArea, this.dblLength);
            }
        }

        //public double ComputeCompactness()
        //{
        //    this.dblComp = CGeoFunc.CalCompactness(this.dblArea, this.dblLength);
        //    return this.dblComp;
        //}

        //public CPatch(IEnumerable<CPolygon> cpgEb, int intType, int intTypeIndex, double dblArea)
        //{
        //    this.CpgSS = new SortedSet<CPolygon>(cpgEb);
        //    this.dblArea = dblArea;
        //    this.GID = _intStaticGID++;
        //}

        public CPatch(IEnumerable<CPatch> cphEb)
            :this()
        {
            this.CpgSS = new SortedSet<CPolygon>();
            foreach (var cph in cphEb)
            {
                this.CpgSS.UnionWith(cph.CpgSS);
                this.dblArea += cph.dblArea;
                this.intSumCpgGID += cph.intSumCpgGID;
            }
        }

        public CPatch Unite(CPatch other, double dblSharedSegLength)
        {
            var unitedcph = UniteNoAttribute(other);
            unitedcph.dblArea = this.dblArea + other.dblArea;
            unitedcph.dblLength = this.dblLength + other.dblLength - 2 * dblSharedSegLength;
            if (CConstants.blnComputeMinComp == true || CConstants.blnComputeAvgComp == true)
            {
                unitedcph.dblComp = CGeoFunc.CalCompactness(unitedcph.dblArea, unitedcph.dblLength);
            }

            return unitedcph;
        }

        private CPatch UniteNoAttribute(CPatch other)
        {
            var unitedcph = new CPatch();
            unitedcph.CpgSS = new SortedSet<CPolygon>(this.CpgSS);
            unitedcph.CpgSS.UnionWith(other.CpgSS);
            unitedcph.intSumCpgGID = this.intSumCpgGID + other.intSumCpgGID;

            if (this.isCore == true || other.isCore == true)
            {
                unitedcph.isCore = true;
            }
            return unitedcph;
        }

        public CPolygon GetSoloCpg()
        {
            if (this.CpgSS.Count > 1)
            {
                throw new ArgumentOutOfRangeException("There are more than one elements!");
            }

            return this.CpgSS.Min;
        }


        public CPatch GetSmallestNeighbor()
        {
            if (CConstants.strMethod != "Greedy")
            {
                throw new ArgumentOutOfRangeException("This method can only be used in the Greedy algorithm");
            }


            return this.AdjacentCphSS.Min();
        }



        public IPolygon4 MergeCpgSSToIpg()
        {
            _pPolygon= CGeoFunc.MergeCpgEbToIpg(this.CpgSS);
            return _pPolygon;
        }

        public IPolygon4 JudgeAndMergeCpgSSToIpg()
        {
            if (_pPolygon == null)
            {
                return MergeCpgSSToIpg();
            }
            else
            {
                return _pPolygon;
            }
        }
    }
}
