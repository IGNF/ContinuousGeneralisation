using System;
using System.Collections.Generic;
using System.Windows.Forms;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;

//using MorphingClass;

namespace MorphingClass.CUtility
{
    //Note:
    //the first variable is the new element, while the second variable has been already in the data structure
    //return  1:put the first variable after the second one
    //return -1:put the first variable in front of the second one
    public class CCmpMethods
    {
        //public static CCmpDbl_CoordVerySmall _pCmpDbl_CoordVerySmall = new CCmpDbl_CoordVerySmall();

        public static bool ConvertCmpToBool(int intCmp)
        {
            if (intCmp == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int CmpDblPostLocateEqual(double dbl1, double dbl2)
        {
            int iResult;
            if (dbl1 < dbl2)
                iResult = -1;
            else
                iResult = 1;
            return iResult;
        }

        public static int CmpDblRange(double dbl1, double dbl2, double dblR)
        {
            double dblDiff = dbl1 - dbl2;
            if (dblDiff>dblR)
            {
                return 1;
            }
            else if (dblDiff <= dblR && dblDiff >= -dblR)
            {
                return 0;
            }
            else  //if (dblDiff < -R)
            {
                return -1;
            }
        }

        /// <summary>
        /// Compare two doubles
        /// </summary>
        /// <returns>1, if the first parameter larger than the second one; -1, smaller; 0, equal</returns>
        /// <remarks>please notice that the default value of _dblVerySmall is 0.0000000000000001, 
        /// but we usually set a new _dblVerySmall with respect to the data</remarks>
        public static int CmpDbl_CoordVerySmall(double dbl1, double dbl2)
        {
           return  CmpDblRange(dbl1, dbl2, CConstants.dblVerySmallCoord);
        }

        public static int CmpDbl_ConstVerySmall(double dbl1, double dbl2)
        {
            return CmpDblRange(dbl1, dbl2, CConstants.dblVerySmallConst);
        }

        /// <summary>
        /// Compare two cedges, by coordinates
        /// </summary>
        /// <returns>1, if the first parameter larger than the second one; -1, smaller; 0, equal</returns>
        /// <remarks>First comapre FrCpts. If the two cedges have the same FrCpts, then compare ToCpts. 
        /// If they neither have the same FrCpts nor have the same ToCpts, 
        /// then compare FrCpt with ToCpt as well as ToCpt with FrCpt to find whether they are equal</remarks>
        public static int CmpCEdgeCoord(CEdge cedge1, CEdge cedge2, bool blnMayFlip = false)
        {
            GetCpts(cedge1, blnMayFlip, out CPoint frcpt1, out CPoint tocpt1);
            GetCpts(cedge2, blnMayFlip, out CPoint frcpt2, out CPoint tocpt2);

            int intResultFF = CmpCptYX(frcpt1, frcpt2);
            int intResultTT = CmpCptYX(tocpt1, tocpt2);
            return CCmpMethods.HandleTwoResults(intResultFF, intResultTT);
        }

        public static int CmpCEdgeGID(CEdge cedge1, CEdge cedge2, bool blnMayFlip = false)
        {
            CPoint frcpt1;
            CPoint tocpt1;
            GetCpts(cedge1, blnMayFlip, out frcpt1, out tocpt1);

            CPoint frcpt2;
            CPoint tocpt2;
            GetCpts(cedge2, blnMayFlip, out frcpt2, out tocpt2);

            int intResultFF = frcpt1.GID.CompareTo(frcpt2.GID);
            int intResultTT = tocpt1.GID.CompareTo(tocpt2.GID);
            return CCmpMethods.HandleTwoResults(intResultFF, intResultTT);
        }

        public static void GetCpts(CEdge cedge, bool blnMayFlip, out CPoint frcpt, out CPoint tocpt)
        {
            frcpt = cedge.FrCpt;
            tocpt = cedge.ToCpt;

            if (blnMayFlip == true)
            {
                //when we use a SortedSet to store edges, 
                //we want that the edge will have the same position no matter which end is the start
                if (cedge.intIncrease == -1)
                {
                    frcpt = cedge.ToCpt;
                    tocpt = cedge.FrCpt;
                }
            }
        }


        public static int HandleTwoResults(int intResult1, int intResult2)
        {
            if (intResult1 != 0 && intResult2 != 0)
            {
                return intResult1;
            }
            else if (intResult1 != 0 && intResult2 == 0)
            {
                return intResult1;
            }
            else if (intResult1 == 0 && intResult2 != 0)
            {
                return intResult2;
            }
            else
            {
                return 0;
            }
        }


        /// <summary>
        /// Compare two points, by coordinates
        /// </summary>
        /// <returns>1, if the first parameter larger than the second one; -1, smaller; 0, equal</returns>
        /// <remarks>First comapre y coordinates. 
        /// If the two points have the same y coordinates, then compare x coordinates</remarks>
        public static int CmpCptYX(CPoint cpt1, CPoint cpt2, bool blnVerySmall = true)
        {
            IComparer<double> cmp = null;
            if (blnVerySmall==true)
            {
                cmp = CCmpDbl_CoordVerySmall.sComparer;
            }

            return CmpDual(cpt1, cpt2, cpt => cpt.Y, cpt => cpt.X, cmp, cmp);
        }

        public static int CmpCptXY(CPoint cpt1, CPoint cpt2, bool blnVerySmall = true)
        {
            IComparer<double> cmp = null;
            if (blnVerySmall == true)
            {
                cmp = CCmpDbl_CoordVerySmall.sComparer;
            }

            return CmpDual(cpt1, cpt2, cpt => cpt.X, cpt => cpt.Y, cmp, cmp);
        }
        

        public static int CmpSquare(double dbl1, double dbl2)
        {
            return CmpDblRange(dbl1, dbl2, CConstants.dblVerySmallSquare);
        }

        public static int CmpPower4(double dbl1, double dbl2)
        {
            return CmpDblRange(dbl1, dbl2, CConstants.dblVerySmallPower4);
        }

        public static int CmpCrg_CphGIDTypeIndex(CRegion crg1, CRegion crg2)
        {
            int intResult = crg1.GetCphCount().CompareTo(crg2.GetCphCount());

            if (intResult==0)
            {
                intResult = crg1.intSumCphGID.CompareTo(crg2.intSumCphGID);
            }

            if (intResult == 0)
            {
                intResult = crg1.intSumTypeIndex.CompareTo(crg2.intSumTypeIndex);
            }

            if (intResult == 0)
            {
                //this will compare the GID of every CPatch in the two SortedDictionary
                intResult = CCmpMethods.CmpWithSameElements(crg1.GetCphCol(), crg2.GetCphCol(), cph => cph);  
            }

            if (intResult == 0)
            {
                intResult = CCmpMethods.CmpWithSameElements(crg1.GetCphTypeIndexCol(), crg2.GetCphTypeIndexCol(), 
                    intTypeIndex => intTypeIndex);
            }
            return intResult;
        }


        public static int CmpCrg_nmID(CRegion crg1, CRegion crg2)
        {
            int intResult = crg1.GetCphCount().CompareTo(crg2.GetCphCount());

            if (intResult == 0)
            {
                intResult = crg1.GetAdjCount().CompareTo(crg2.GetAdjCount());
            }

            if (intResult == 0)
            {
                intResult = crg1.ID.CompareTo(crg2.ID);
            }

            if (intResult == 0)
            {
                throw new ArgumentOutOfRangeException("this should not happen!");
            }

            return intResult;
        }

        public static int CmpColConsideringCount<T, TOrder>(ICollection<T> pCol1, ICollection<T> pCol2, 
            Func<T, TOrder> orderFunc = null, IComparer<TOrder> cmp = null, bool blnReverse = false)
        {
            int intResult = pCol1.Count.CompareTo(pCol2.Count);

            if (intResult == 0)
            {
                intResult = CCmpMethods.CmpWithSameElements(pCol1, pCol2, orderFunc, cmp);
            }

            if (blnReverse == true)
            {
                intResult = intResult * -1;
            }
            return intResult;
        }

        public static int CmpWithSameElements<T, TOrder>(IEnumerable<T> TEb1, IEnumerable<T> TEb2, 
            Func<T, TOrder> orderFunc =null, IComparer<TOrder> cmp = null, bool blnReverse = false)
        {
            if (cmp == null) { cmp = Comparer<TOrder>.Default; }

            var TEt1 = TEb1.GetEnumerator();
            var TEt2 = TEb2.GetEnumerator();

            int intResult = 0;
            while (TEt1.MoveNext() && TEt2.MoveNext())
            {
                intResult = cmp.Compare(orderFunc(TEt1.Current), orderFunc(TEt2.Current));
                if (intResult != 0)
                {
                    break;
                }
            }

            if (blnReverse == true)
            {
                intResult = intResult * -1;
            }
            return intResult;
        }


        //public static int Cmp<T, TOrder>(IEnumerable<T> TEb1, IEnumerable<T> TEb2, 
        //Func<T, TOrder> orderFunc, IComparer<TOrder> cmp = null, bool blnReverse = false)
        //{
        //    if (cmp == null) { cmp = Comparer<TOrder>.Default; }

        //    var TEt1 = TEb1.GetEnumerator();
        //    var TEt2 = TEb2.GetEnumerator();

        //    int intResult = 0;
        //    while (true)
        //    {
        //        bool blnMoveNext1 = TEt1.MoveNext();
        //        bool blnMoveNext2 = TEt2.MoveNext();

        //        if (blnMoveNext1 == true && blnMoveNext2 == true)
        //        {
        //            intResult = cmp.Compare(orderFunc(TEt1.Current), orderFunc(TEt2.Current));
        //            if (intResult != 0)
        //            {
        //                break ;
        //            }
        //        }
        //        else if (blnMoveNext1 == true && blnMoveNext2 == false)
        //        {
        //            intResult = 1;
        //            break;
        //        }
        //        else if (blnMoveNext1 == false && blnMoveNext2 == true)
        //        {
        //            intResult = -1;
        //            break;
        //        }
        //        else //if (blnMoveNext1 == false && blnMoveNext2 == false)
        //        {
        //            intResult = 0;
        //            break;
        //        }
        //    }

        //    if (blnReverse == true )
        //    {
        //        intResult = intResult * -1;
        //    }
        //    return intResult;
        //}

        ///// <summary>
        ///// Compare two Linkedlists of intergers.
        ///// </summary>
        ///// <returns>1, if the first parameter larger than the second one; -1, smaller; 0, equal</returns>
        ///// <remarks></remarks>
        //public static int Cmp(SCG.SortedSet<int> intSS1, SCG.SortedSet<int> intSS2)
        //{
        //    int intResult = 0;
        //    SortedSet<int>.Enumerator intSSEnu1 = intSS1.GetEnumerator();
        //    SortedSet<int>.Enumerator intSSEnu2 = intSS2.GetEnumerator();
        //    do
        //    {
        //        bool blnMoveNext1 = intSSEnu1.MoveNext();
        //        bool blnMoveNext2 = intSSEnu2.MoveNext();

        //        if (blnMoveNext1 == false && blnMoveNext2 == false)
        //        {
        //            intResult = 0;
        //            break;
        //        }
        //        else if (blnMoveNext1 != false && blnMoveNext2 == false)
        //        {
        //            intResult = 1;
        //            break;
        //        }
        //        else if (blnMoveNext1 == false && blnMoveNext2 != false)
        //        {
        //            intResult = -1;
        //            break;
        //        }
        //        else
        //        {
        //            intResult =intSSEnu1.Current.CompareTo(intSSEnu2.Current);
        //            if (intResult != 0)
        //            {
        //                break;
        //            }
        //        }
        //    } while (true);

        //    return intResult;
        //}

        ///mark
        /// <summary>whether dblValue is in (dblbound1, dblbound2)</summary>
        /// <returns> 
        /// 1   If dblValue is smaller than dblbound1;
        /// 2   If dblValue is equal to dblbound1 and smaller than dblbound2
        /// 0   If dblValue is equal to dblbound1 and equal to dblbound2; this is a very special case, but can happen
        /// 3   If dblValue is larger than dblbound1 and smaller than dblbound2,                 
        /// 4   If dblValue is larger than dblbound1 and equal to dblbound2, 
        /// 5   If dblValue is larger than dblbound1 and larger than dblbound2,</returns>
        public static int CmpThreeCoord(double dblValue, double dblbound1, double dblbound2)
        {
            int intResult = 0;
            if (dblbound1 <= dblbound2)
            {
                intResult = CmpIncrease(dblValue, dblbound1, dblbound2);
            }
            else
            {
                intResult = CmpIncrease(dblValue, dblbound2, dblbound1);
                if (intResult != 0)
                {
                    intResult = 6 - intResult;
                }
                else
                {
                    intResult = 0;
                }
            }
            return intResult;
        }


        public static bool IsInbetween(double dblValue, double dblbound1, double dblbound2)
        {
            if (dblbound1 <= dblbound2)
            {
                return IsInbetweenIncrease(dblValue, dblbound1, dblbound2);
            }
            else
            {
                return IsInbetweenIncrease(dblValue, dblbound2, dblbound1);
            }
        }

        public static bool IsInbetweenIncrease(double dblValue, double dblbound1, double dblbound2)
        {
            if (dblValue < dblbound1 - CConstants.dblVerySmallCoord || 
                dblValue > dblbound2 + CConstants.dblVerySmallCoord)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>whether dblValue is in (dblbound1, dblbound2)</summary>
        /// <returns> 
        /// 1   If dblValue is smaller than dblbound1;
        /// 2   If dblValue is equal to dblbound1 and smaller than dblbound2
        /// 0   If dblValue is equal to dblbound1 and equal to dblbound2; this is a very special case, but can happen
        /// 3   If dblValue is larger than dblbound1 and smaller than dblbound2,                 
        /// 4   If dblValue is larger than dblbound1 and equal to dblbound2, 
        /// 5   If dblValue is larger than dblbound1 and larger than dblbound2,</returns>
        public static int CmpIncrease(double dblValue, double dblbound1, double dblbound2)
        {
            int intCmp1 = CmpDbl_CoordVerySmall(dblValue, dblbound1);
            int intCmp2 = CmpDbl_CoordVerySmall(dblValue, dblbound2);

            if (intCmp1 == -1)
            {
                if (intCmp2 == -1)
                {
                    return 1;
                }
                else if (intCmp2 == 0)
                {
                    MessageBox.Show("This cannot happen! Compare!");
                    return -1;
                }
                else  //if  (intCmp2==1)
                {
                    MessageBox.Show("This cannot happen! Compare!");
                    return -1;
                }
            }
            else if (intCmp1 == 0)
            {
                if (intCmp2 == -1)
                {
                    return 2;
                }
                else if (intCmp2 == 0)  //this is a very special case!!!
                {
                    return 0;
                }
                else  //if  (intCmp2==1)
                {
                    MessageBox.Show("This cannot happen! Compare!");
                    return -1;
                }
            }
            else  //if  (intCmp1==1)
            {
                if (intCmp2 == -1)
                {
                    return 3;
                }
                else if (intCmp2 == 0)  //this is a very special case!!!
                {
                    return 4;
                }
                else  //if  (intCmp2==1)
                {
                    return 5;
                }
            }
        }

        ///// <summary>
        ///// compare two attributes of the elements
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <typeparam name="TOrder"></typeparam>
        ///// <param name="T1"></param>
        ///// <param name="T2"></param>
        ///// <param name="orderFunc1"></param>
        ///// <param name="orderFunc2"></param>
        ///// <param name="cmp"></param>
        ///// <returns></returns>
        //public static int CmpDual<T, TOrder>(T T1, T T2, 
        //Func<T, TOrder> orderFunc1, Func<T, TOrder> orderFunc2, IComparer<TOrder> cmp = null)
        //{
        //    if (cmp == null) { cmp = Comparer<TOrder>.Default; }
            
        //    int intResult = cmp.Compare(orderFunc1(T1), orderFunc1(T2));
        //    if (intResult == 0)  //if (cpt1.Y == cpt2.Y)
        //    {
        //        intResult = cmp.Compare(orderFunc2(T1), orderFunc2(T2));
        //    }
        //    return intResult;
        //}

        /// <summary>
        /// compare two attributes with different types
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <typeparam name="TOrder"></typeparam>
        /// <param name="T1"></param>
        /// <param name="T2"></param>
        /// <param name="orderFunc1"></param>
        /// <param name="orderFunc2"></param>
        /// <param name="cmp"></param>
        /// <returns></returns>
        public static int CmpDual<T, TOrder1, TOrder2>(T T1, T T2, Func<T, TOrder1> orderFunc1, 
            Func<T, TOrder2> orderFunc2, IComparer<TOrder1> cmp1 = null, IComparer<TOrder2> cmp2 = null)
        {
            if (cmp1 == null) { cmp1 = Comparer<TOrder1>.Default; }
            if (cmp2 == null) { cmp2 = Comparer<TOrder2>.Default; }

            int intResult = cmp1.Compare(orderFunc1(T1), orderFunc1(T2));
            if (intResult == 0)  //if (cpt1.Y == cpt2.Y)
            {
                intResult = cmp2.Compare(orderFunc2(T1), orderFunc2(T2));
            }
            return intResult;
        }

        public static int CmpTernary<T, TOrder1, TOrder2, TOrder3>(T T1, T T2,
            Func<T, TOrder1> orderFunc1, Func<T, TOrder2> orderFunc2, Func<T, TOrder3> orderFunc3,
            IComparer<TOrder1> cmp1 = null, IComparer<TOrder2> cmp2 = null, IComparer<TOrder3> cmp3 = null)
        {
            if (cmp1 == null) { cmp1 = Comparer<TOrder1>.Default; }
            int intResult = cmp1.Compare(orderFunc1(T1), orderFunc1(T2));
            if (intResult == 0)  //if (cpt1.Y == cpt2.Y)
            {
                if (cmp2 == null) { cmp2 = Comparer<TOrder2>.Default; }
                intResult = cmp2.Compare(orderFunc2(T1), orderFunc2(T2));
                if (intResult == 0)  //if (cpt1.Y == cpt2.Y)
                {
                    if (cmp3 == null) { cmp3 = Comparer<TOrder3>.Default; }
                    intResult = cmp3.Compare(orderFunc3(T1), orderFunc3(T2));
                }
            }
            return intResult;
        }

        public static int CmpGeneric<T, TOrder>(T T1, T T2, Func<T, TOrder> orderFunc,
            IComparer<TOrder> cmp = null)
        {
            cmp = CHelpFunc.SetOrDefaultCmp(cmp);
            return cmp.Compare(orderFunc(T1), orderFunc(T2));
        }

        ///// <summary>whether dblValue is in (dblbound1, dblbound2) or  (dblbound2, dblbound1) </summary>
        ///// <returns> If dblValue is in the range, return 1;
        /////                 If dblValue is on the boundary of the range, return 0
        /////                 If dblValue is outside of the boundary, return -1</returns>
        //public static int InBetween(double dblValue, double dblbound1, double dblbound2)
        //{
        //    if (dblbound1 > dblbound2)
        //    {
        //        CHelpFunc.Swap(ref dblbound1, ref dblbound2);
        //    }

        //    int intCmp1 = Compare(dblValue, dblbound1);
        //    int intCmp2 = Compare(dblValue, dblbound2);

        //    if (intCmp1 == -1 || intCmp2 == 1)
        //    {
        //        return -1;
        //    }
        //    else if (intCmp1 == 0 || intCmp2 == 0)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return 1;
        //    }
        //}

        public static double SnapValueToTarget(double dblValue, double dblTarget, double dblSnapRange)
        {
            if (CCmpMethods.CmpDblRange(dblValue, dblTarget, dblSnapRange) == 0)
            {
                return dblTarget;
            }
            else
            {
                return dblValue;
            }

        }
    }

    




    
}
