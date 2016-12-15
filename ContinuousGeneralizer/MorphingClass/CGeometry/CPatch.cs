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
        public static CCompareCPatch_CpgGID pCompareCPatch_CpgGID = new CCompareCPatch_CpgGID();  //this variable should be used for CPatch itself
        public static CCompareCPatch_Area_CphGID pCompareCPatch_Area_CphGID = new CCompareCPatch_Area_CphGID();  //this variable should be used for CPatch itself
        public static CCompareCPatch_Compactness_CphGID pCompareCPatch_Compactness_CphGID = new CCompareCPatch_Compactness_CphGID();


        public SortedSet<CPolygon> CpgSS { get; set; }
        public int intSumCpgGID { get; set; }

        /// <summary>this is the real compactness 2*Sqrt(pi*A)/L.</summary>
        public double dblCompactness { get; set; }  
        
        public double dblArea { get; set; }
        public double dblLength { get; set; }

        public bool isCore { get; set; }

        //public SortedDictionary<CPatch , List<CEdge>> Adjacent_CphsCEdgeLtSD { get; set; }  //only temporary for compute area estimation

        public SortedSet <CPatch> AdjacentCphSS { get; set; }  //only for the ILP


        public CPatch()
        {

            this.GID = _intStaticGID++;
        }


        public CPatch(CPolygon cpg, int intID, int pintTypeIndex)
        {
            this.ID = intID;
            this.CpgSS = new SortedSet<CPolygon>();
            this.CpgSS.Add(cpg);
            this.dblArea = (cpg.pPolygon as IArea).Area;
            this.dblLength = cpg.pPolygon.Length;
            this.intSumCpgGID = cpg.GID;
            this.GID = _intStaticGID++;
            this.intTypeIndex = pintTypeIndex;

            if (CConstants.blnComputeCompactness == true)
            {
                this.dblCompactness = CGeometricMethods.CalCompactness(this.dblArea, this.dblLength);
            }
        }

        //public double ComputeCompactness()
        //{
        //    this.dblCompactness = CGeometricMethods.CalCompactness(this.dblArea, this.dblLength);
        //    return this.dblCompactness;
        //}

        //public CPatch(IEnumerable<CPolygon> cpgEb, int intType, int intTypeIndex, double dblArea)
        //{
        //    this.CpgSS = new SortedSet<CPolygon>(cpgEb);
        //    this.dblArea = dblArea;
        //    this.GID = _intStaticGID++;
        //}

        public CPatch(IEnumerable<CPatch> cphEb)
        {
            this.CpgSS = new SortedSet<CPolygon>();
            foreach (var cph in cphEb)
            {
                this.CpgSS.UnionWith(cph.CpgSS);
                this.dblArea += cph.dblArea;
                this.intSumCpgGID += cph.intSumCpgGID;
            }            
            this.GID = _intStaticGID++;
        }

        public CPatch Unite(CPatch other, double dblSharedSegmentLength)
        {
            var unitedcph = UniteNoAttribute(other);
            unitedcph.dblArea = this.dblArea + other.dblArea;
            unitedcph.dblLength = this.dblLength + other.dblLength - 2 * dblSharedSegmentLength;
            if (CConstants.blnComputeCompactness == true)
            {
                unitedcph.dblCompactness = CGeometricMethods.CalCompactness(unitedcph.dblArea, unitedcph.dblLength);
            }

            return unitedcph;
        }

        private CPatch UniteNoAttribute(CPatch other)
        {
            var unitedcph = new CPatch();
            unitedcph.CpgSS = new SortedSet<CPolygon>(this.CpgSS);
            unitedcph.CpgSS.UnionWith(other.CpgSS);
            unitedcph.intSumCpgGID = this.intSumCpgGID + other.intSumCpgGID;

            if (this.isCore ==true || other .isCore ==true )
            {
                unitedcph.isCore = true;
            }
            return unitedcph;
        }


        public IPolygon4 MergeCpgSS()
        {
            return CGeometricMethods.MergeCpgEbAE(this.CpgSS);
        }

    }
}
