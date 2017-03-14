using System;
using MorphingClass.CGeometry;
using System.Collections.Generic;
using System.Text;
using System.Collections;

//using MorphingClass;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CUtility
{
    //Note:
    //the first variable is the new element, while the second variable has been already in the data structure
    //return  1:put the first variable after the second one
    //return -1:put the first variable in front of the second one
    //public class CCompare
    //{

    //}
    


    public class CCompareDbl_VerySmall : IComparer<double>
    {
        public static CCompareDbl_VerySmall pCompareDbl_VerySmall = new CCompareDbl_VerySmall();

        public int Compare(double x, double y)
        {
            return CCompareMethods.CompareDbl_VerySmall(x, y);
        }
    }

    public class CCompareDbl : IComparer<double>
    {
        //CaseInsensitiveComparer MyCompare = new CaseInsensitiveComparer();
        public int Compare(double x, double y)
        {
            return CCompareMethods.CompareDblPostLocateEqual(x, y);
        }
    }

    public class CAACCompare : IComparer<IList<object>>
    {
        public int Compare(IList<object> lt1, IList<object> lt2)
        {
            int intResult = CCompareMethods.CompareDbl_VerySmall(Convert.ToDouble(lt1[3]), Convert.ToDouble(lt2[3]));  //overesimation factor
            if (intResult == 0)
            {
                intResult = CCompareMethods.CompareDbl_VerySmall(Convert.ToDouble(lt1[1]), Convert.ToDouble(lt2[1]));  //patch number
            }
            if (intResult == 0)
            {
                intResult = CCompareMethods.CompareDbl_VerySmall(Convert.ToDouble(lt1[2]), Convert.ToDouble(lt2[2]));  //adjacency number
            }
            if (intResult == 0)
            {
                intResult = CCompareMethods.CompareDbl_VerySmall(Convert.ToDouble(lt1[0]), Convert.ToDouble(lt2[0]));  //domain id
            }
            return -intResult;
        }
    }


    public class CDblReverseCompare : IComparer<double>
    {
        //CaseInsensitiveComparer MyCompare = new CaseInsensitiveComparer();
        public int Compare(double x, double y)
        {
            int iResult;
            if (x < y)
                iResult = 1;
            else
                iResult = -1;
            return iResult;
        }
    }
    public class CIntCompare : IComparer<int>
    {
        public int Compare(int x, int y)
        {
            int iResult;
            if (x > y)
                iResult = 1;
            else
                iResult = -1;
            return iResult;
            //t
        }
    }

    

    public class CCompareCptYX_VerySmall : IComparer<CPoint>
    {
        public static CCompareCptYX_VerySmall pCompareCptYX_VerySmall = new CCompareCptYX_VerySmall();

        public int Compare(CPoint cpt1, CPoint cpt2)
        {
            return CCompareMethods.CompareCptYX(cpt1, cpt2);
        }
    }

    //public class CECompareCptYX_VerySmall : IEqualityComparer<CPoint>
    //{
    //    public static CECompareCptYX_VerySmall pCompareCptYX_E_VerySmall = new CECompareCptYX_VerySmall();

    //    public bool Equals(CPoint cpt1, CPoint cpt2)
    //    {
    //        return CCompareMethods.ConvertCompareToBool(CCompareMethods.CompareCptYX(cpt1, cpt2));
    //    }
    //}


    public class CCompareCPatch_Area_CphGID : IComparer<CPatch>
    {
        public int Compare(CPatch cph1, CPatch cph2)
        {
            int intResult = CCompareMethods.CompareDbl_VerySmall(cph1.dblArea, cph2.dblArea);
            if (intResult == 0)
            {
                intResult = cph1.GID.CompareTo(cph2.GID);
            }
            return intResult;
        }
    }

    public class CCompareCPatch_Compactness_CphGID : IComparer<CPatch>
    {
        public int Compare(CPatch cph1, CPatch cph2)
        {
            int intResult = CCompareMethods.CompareDbl_VerySmall(cph1.dblComp, cph2.dblComp);
            //int intResult = cph1.dblComp.CompareTo(cph2.dblComp);
            if (intResult == 0)
            {
                intResult = cph1.GID.CompareTo(cph2.GID);
            }
            return intResult;
        }
    }

    public class CCompareCPatch_CpgGID : IComparer<CPatch>
    {
        public int Compare(CPatch cph1, CPatch cph2)
        {
            int intResult = cph1.intSumCpgGID.CompareTo(cph2.intSumCpgGID);
            if (intResult == 0)
            {
                intResult = CCompareMethods.CompareColConsideringCount(cph1.CpgSS, cph2.CpgSS, cpg => cpg.GID);
            }
            return intResult;
        }
    }

    /// <summary>
    /// we assume that there will be no more than one CPatch that contains the same set of polygons
    /// </summary>
    public class CCompareCCorrCphs_CphsGID : IComparer<CCorrCphs>
    {
        public int Compare(CCorrCphs CorrCphs1, CCorrCphs CorrCphs2)
        {
            return CCompareMethods.CompareDual(CorrCphs1, CorrCphs2, CorrCphs => CorrCphs.FrCph.GID, CorrCphs => CorrCphs.ToCph.GID);
        }
    }



    /// <summary>
    /// we assume that there will be no more than one CPatch that contains the same set of polygons
    /// </summary>
    public class CCompareCRegion_CphGIDTypeIndex : IComparer<CRegion>
    {
        public int Compare(CRegion crg1, CRegion crg2)
        {
            return CCompareMethods.CompareCRegion_CphGIDTypeIndex (crg1 ,crg2);
        }
    }

    public class CCompareCRegion_CphGID : IComparer<CRegion>
    {
        public int Compare(CRegion crg1, CRegion crg2)
        {
            return CCompareMethods.CompareCRegion_CphGIDTypeIndex(crg1, crg2);
        }
    }

    public class CCompareCRegion_Cost_CphGIDTypeIndex : IComparer<CRegion>
    {
        public int Compare(CRegion crg1, CRegion crg2)
        {
            //int intResult = crg1.d.CompareTo(crg2.d);
            int intResult = CCompareMethods.CompareDbl_VerySmall(crg1.d, crg2.d);
            if (intResult == 0)
            {
                intResult = CCompareMethods.CompareCRegion_CphGIDTypeIndex(crg1, crg2);
            }
            return intResult;
        }
    }

    public class CCompareCRegion_MinArea_CphGIDTypeIndex : IComparer<CRegion>
    {
        public int Compare(CRegion crg1, CRegion crg2)
        {
            //int intResult = crg1.GetCphCol().GetFirstT().dblArea.CompareTo(crg2.GetCphCol().GetFirstT().dblArea);
            int intResult = CCompareMethods.CompareDbl_VerySmall(crg1.GetCphCol().GetFirstT().dblArea, crg2.GetCphCol().GetFirstT().dblArea);
            if (intResult == 0)
            {
                intResult = CCompareMethods.CompareCRegion_CphGIDTypeIndex(crg1, crg2);
            }
            return intResult;
        }
    }

    public class CCompareCRegion_CostExact_CphGIDTypeIndex : IComparer<CRegion>
    {
        public int Compare(CRegion crg1, CRegion crg2)
        {
            int intResult = CCompareMethods.CompareDbl_VerySmall(crg1.dblCostExact, crg2.dblCostExact);
            //int intResult = crg1.dblCostExact.CompareTo(crg2.dblCostExact);
            if (intResult == 0)
            {
                intResult = CCompareMethods.CompareCRegion_CphGIDTypeIndex(crg1, crg2);
            }
            return intResult;
        }
    }

    //public class CCompareCRegion_CompareDblPreLocateEqual : IComparer<CRegion>
    //{
    //    public int Compare(CRegion crg1, CRegion crg2)
    //    {
    //        return CCompareMethods.CompareDblPostLocateEqual(crg1.d, crg2.d);
    //    }
    //}

    //public class CCompareRegion : IComparer<CRegion>
    //{
    //    public int Compare(CRegion crg1, CRegion crg2)
    //    {
    //        return CCompareMethods.Compare(crg1.IDLk, crg2.IDLk);           
    //    }
    //}

    /// <summary>
    /// Compare two cedges, by coordinates
    /// </summary>
    public class CCompareCEdgeCoordinates : IComparer<CEdge>
    {
        public int Compare(CEdge cedge1, CEdge cedge2)
        {
            return CCompareMethods.CompareCEdgeCoordinates(cedge1, cedge2);
        }
    }

    //public class CCompareEdge_CptTinNodeTagValue : IComparer<CEdge>
    //{
    //    public int Compare(CEdge cedge1, CEdge cedge2)
    //    {
    //        //we use the difference of the TagValue, so that we know the boundary edges form the difference is 1
    //        int intResultDiff = (cedge1.ToCpt.pTinNode.TagValue - cedge1.FrCpt.pTinNode.TagValue).CompareTo(cedge2.ToCpt.pTinNode.TagValue - cedge2.FrCpt.pTinNode.TagValue);
    //        int intResultFF = (cedge1.FrCpt.pTinNode.TagValue).CompareTo(cedge2.FrCpt.pTinNode.TagValue);

    //        return CCompareMethods.HandleTwoResults(intResultDiff, intResultFF);
    //    }
    //}

    public class CCompareEdge_CptIndexIDDiff : IComparer<CEdge>
    {
        public int Compare(CEdge cedge1, CEdge cedge2)
        {
            //we use the difference of the TagValue, so that we know the boundary edges form the difference is 1
            int intResultDiff = (cedge1.ToCpt.indexID - cedge1.FrCpt.indexID).CompareTo(cedge2.ToCpt.indexID - cedge2.FrCpt.indexID);
            int intResultFF = cedge1.FrCpt.indexID.CompareTo(cedge2.FrCpt.indexID);

            return CCompareMethods.HandleTwoResults(intResultDiff, intResultFF);
        }
    }
   
}
