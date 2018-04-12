using System;
using MorphingClass.CGeometry;
using System.Collections.Generic;
using System.Text;
using System.Linq;
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
    //public class CCmp
    //{

    //}
    


    public class CCmpCoordDbl_VerySmall : Comparer<double>
    {
        public static CCmpCoordDbl_VerySmall sComparer = new CCmpCoordDbl_VerySmall();

        public override int Compare(double x, double y)
        {
            return CCmpMethods.CmpCoordDbl_VerySmall(x, y);
        }
    }

    public class CCmpDbl : Comparer<double>
    {
        //CaseInsensitiveComparer MyCompare = new CaseInsensitiveComparer();
        public override int Compare(double x, double y)
        {
            return CCmpMethods.CmpDblPostLocateEqual(x, y);
        }
    }

    //public class CCmpCptEdgeDis_BridgeLength : Comparer<CptEdgeDis>
    //{
    //    public static CCmpCptEdgeDis_BridgeLength sComparer = new CCmpCptEdgeDis_BridgeLength();
    //    public override int Compare(CptEdgeDis x, CptEdgeDis y)
    //    {
    //        return CCmpMethods.CmpDual(x, y, cptEdgeDis => cptEdgeDis.dblBridgeLength, cptEdgeDis => cptEdgeDis.GID);
    //    }
    //}

    public class CCmpCptEdgeDis_Dis : Comparer<CptEdgeDis>
    {
        public static CCmpCptEdgeDis_Dis sComparer = new CCmpCptEdgeDis_Dis();
        public override int Compare(CptEdgeDis x, CptEdgeDis y)
        {
            return CCmpMethods.CmpDual(x, y, cptEdgeDis => cptEdgeDis.dblDis, cptEdgeDis => cptEdgeDis.GID);
        }
    }

    public class CCmpCptEdgeDis_T : Comparer<CptEdgeDis>
    {
        public static CCmpCptEdgeDis_T sComparer = new CCmpCptEdgeDis_T();
        public override int Compare(CptEdgeDis x, CptEdgeDis y)
        {
            return CCmpMethods.CmpDual(x,y, cptEdgeDis=> cptEdgeDis.t, cptEdgeDis=> cptEdgeDis.GID);
        }
    }

    public class CAACCmp : Comparer<IList<object>>
    {
        public override int Compare(IList<object> lt1, IList<object> lt2)
        {
            int intResult = CCmpMethods.CmpDbl_ConstVerySmall
                (Convert.ToDouble(lt1[3]), Convert.ToDouble(lt2[3]));  //overesimation factor
            if (intResult == 0)
            {
                intResult = CCmpMethods.CmpDbl_ConstVerySmall
                    (Convert.ToDouble(lt1[1]), Convert.ToDouble(lt2[1]));  //patch number
            }
            if (intResult == 0)
            {
                intResult = CCmpMethods.CmpDbl_ConstVerySmall
                    (Convert.ToDouble(lt1[2]), Convert.ToDouble(lt2[2]));  //adjacency number
            }
            if (intResult == 0)
            {
                intResult = CCmpMethods.CmpDbl_ConstVerySmall
                    (Convert.ToDouble(lt1[0]), Convert.ToDouble(lt2[0]));  //domain id
            }
            return -intResult;
        }
    }


    public class CDblReverseCompare : Comparer<double>
    {
        //CaseInsensitiveComparer MyCompare = new CaseInsensitiveComparer();
        public override int Compare(double x, double y)
        {
            int iResult;
            if (x < y)
                iResult = 1;
            else
                iResult = -1;
            return iResult;
        }
    }
    public class CIntCompare : Comparer<int>
    {
        public override int Compare(int x, int y)
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

    public class CCmpCptXY_VerySmall : Comparer<CPoint>
    {
        public static CCmpCptXY_VerySmall sComparer = new CCmpCptXY_VerySmall();

        public override int Compare(CPoint cpt1, CPoint cpt2)
        {
            return CCmpMethods.CmpCptXY(cpt1, cpt2);
        }
    }

    public class CCmpCptYX_VerySmall : Comparer<CPoint>
    {
        public static CCmpCptYX_VerySmall sComparer = new CCmpCptYX_VerySmall();

        public override int Compare(CPoint cpt1, CPoint cpt2)
        {
            return CCmpMethods.CmpCptYX(cpt1, cpt2);
        }
    }

    //public class CECompareCptYX_VerySmall : IEqualityComparer<CPoint>
    //{
    //    public static CECompareCptYX_VerySmall pCmpCptYX_E_VerySmall = new CECompareCptYX_VerySmall();

    //    public bool Equals(CPoint cpt1, CPoint cpt2)
    //    {
    //        return CCmpMethods.ConvertCompareToBool(CCmpMethods.CmpCptYX(cpt1, cpt2));
    //    }
    //}


    public class CCmpCPatch_Area_CphGID : Comparer<CPatch>
    {
        public override int Compare(CPatch cph1, CPatch cph2)
        {
            int intResult = CCmpMethods.CmpCoordDbl_VerySmall(cph1.dblArea, cph2.dblArea);
            if (intResult == 0)
            {
                intResult = cph1.GID.CompareTo(cph2.GID);
            }
            return intResult;
        }
    }

    public class CCmpCPatch_Compactness_CphGID : Comparer<CPatch>
    {
        public override int Compare(CPatch cph1, CPatch cph2)
        {
            int intResult = CCmpMethods.CmpDbl_ConstVerySmall(cph1.dblComp, cph2.dblComp);
            //int intResult = cph1.dblComp.CompareTo(cph2.dblComp);
            if (intResult == 0)
            {
                intResult = cph1.GID.CompareTo(cph2.GID);
            }
            return intResult;
        }
    }

    public class CCmpCPatch_CpgGID : Comparer<CPatch>
    {
        public override int Compare(CPatch cph1, CPatch cph2)
        {
            int intResult = cph1.intSumCpgGID.CompareTo(cph2.intSumCpgGID);
            if (intResult == 0)
            {
                intResult = CCmpMethods.CmpColConsideringCount(cph1.CpgSS, cph2.CpgSS, cpg => cpg.GID);
            }
            return intResult;
        }
    }

    /// <summary>
    /// we assume that there will be no more than one CPatch that contains the same set of polygons
    /// </summary>
    public class CCmpCCorrCphs_CphsGID : Comparer<CCorrCphs>
    {
        public override int Compare(CCorrCphs CorrCphs1, CCorrCphs CorrCphs2)
        {
            return CCmpMethods.CmpDual(CorrCphs1, CorrCphs2, CorrCphs => CorrCphs.FrCph.GID, CorrCphs => CorrCphs.ToCph.GID);
        }
    }



    /// <summary>
    /// we assume that there will be no more than one CPatch that contains the same set of polygons
    /// </summary>
    public class CCmpCRegion_CphGIDTypeIndex : Comparer<CRegion>
    {
        public override int Compare(CRegion crg1, CRegion crg2)
        {
            return CCmpMethods.CmpCRegion_CphGIDTypeIndex (crg1 ,crg2);
        }
    }

    public class CCmpCRegion_CphGID : Comparer<CRegion>
    {
        public override int Compare(CRegion crg1, CRegion crg2)
        {
            return CCmpMethods.CmpCRegion_CphGIDTypeIndex(crg1, crg2);
        }
    }

    public class CCmpCRegion_Cost_CphGIDTypeIndex : Comparer<CRegion>
    {
        public override int Compare(CRegion crg1, CRegion crg2)
        {
            //int intResult = crg1.d.CompareTo(crg2.d);
            int intResult = CCmpMethods.CmpDbl_ConstVerySmall(crg1.d, crg2.d);
            if (intResult == 0)
            {
                intResult = CCmpMethods.CmpCRegion_CphGIDTypeIndex(crg1, crg2);
            }
            return intResult;
        }
    }

    public class CCmpCRegion_MinArea_CphGIDTypeIndex : Comparer<CRegion>
    {
        public override int Compare(CRegion crg1, CRegion crg2)
        {
            //int intResult = crg1.GetCphCol().GetFirstT().dblArea.CompareTo(crg2.GetCphCol().GetFirstT().dblArea);
            int intResult = CCmpMethods.CmpCoordDbl_VerySmall
                (crg1.GetCphCol().First().dblArea, crg2.GetCphCol().First().dblArea);
            if (intResult == 0)
            {
                intResult = CCmpMethods.CmpCRegion_CphGIDTypeIndex(crg1, crg2);
            }
            return intResult;
        }
    }

    public class CCmpCRegion_CostExact_CphGIDTypeIndex : Comparer<CRegion>
    {
        public override int Compare(CRegion crg1, CRegion crg2)
        {
            int intResult = CCmpMethods.CmpDbl_ConstVerySmall(crg1.dblCostExact, crg2.dblCostExact);
            //int intResult = crg1.dblCostExact.CompareTo(crg2.dblCostExact);
            if (intResult == 0)
            {
                intResult = CCmpMethods.CmpCRegion_CphGIDTypeIndex(crg1, crg2);
            }
            return intResult;
        }
    }

    //public class CCmpCRegion_CompareDblPreLocateEqual : Comparer<CRegion>
    //{
    //    public override int Compare(CRegion crg1, CRegion crg2)
    //    {
    //        return CCmpMethods.CmpDblPostLocateEqual(crg1.d, crg2.d);
    //    }
    //}

    //public class CCmpRegion : Comparer<CRegion>
    //{
    //    public override int Compare(CRegion crg1, CRegion crg2)
    //    {
    //        return CCmpMethods.Cmp(crg1.IDLk, crg2.IDLk);           
    //    }
    //}

    /// <summary>
    /// Compare two cedges, by coordinates
    /// </summary>
    public class CCmpCEdgeCoordinates : Comparer<CEdge>
    {
        public override int Compare(CEdge cedge1, CEdge cedge2)
        {
            return CCmpMethods.CmpCEdgeCoord(cedge1, cedge2, true);
        }
    }


    public class CCmpEdge_CptIndexIDDiff : Comparer<CEdge>
    {
        public override int Compare(CEdge cedge1, CEdge cedge2)
        {
            int intResultDiff = (cedge1.ToCpt.indexID - cedge1.FrCpt.indexID)
                .CompareTo(cedge2.ToCpt.indexID - cedge2.FrCpt.indexID);
            int intResultFF = cedge1.FrCpt.indexID.CompareTo(cedge2.FrCpt.indexID);

            return CCmpMethods.HandleTwoResults(intResultDiff, intResultFF);
        }
    }





    public class CCmpEdge_CptGID_BothDirections : Comparer<CEdge>
    {
        public override int Compare(CEdge cedge1, CEdge cedge2)
        {
            int intSmallGID1 = Math.Min(cedge1.FrCpt.GID, cedge1.ToCpt.GID);
            int intLargeGID1 = Math.Max(cedge1.FrCpt.GID, cedge1.ToCpt.GID);
            int intSmallGID2 = Math.Min(cedge2.FrCpt.GID, cedge2.ToCpt.GID);
            int intLargeGID2 = Math.Max(cedge2.FrCpt.GID, cedge2.ToCpt.GID);

            int intSmallResult = intSmallGID1.CompareTo(intSmallGID2);
            int intLargeResult = intLargeGID1.CompareTo(intLargeGID2);

            return CCmpMethods.HandleTwoResults(intSmallResult, intLargeResult);
        }
    }

}
