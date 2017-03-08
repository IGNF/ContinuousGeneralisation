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
    public class CCompareMethods
    {
       //public static CCompareDbl_VerySmall _pCompareDbl_VerySmall = new CCompareDbl_VerySmall();

        public static bool ConvertCompareToBool(int intCompare)
        {
            if (intCompare == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static int CompareDblPostLocateEqual(double dbl1, double dbl2)
        {
            int iResult;
            if (dbl1 < dbl2)
                iResult = -1;
            else
                iResult = 1;
            return iResult;
        }

        //public static int CompareDblPreLocateEqual(double dbl1, double dbl2)
        //{
        //    int iResult;
        //    if (dbl1 <= dbl2)
        //        iResult = -1;
        //    else
        //        iResult = 1;
        //    return iResult;
        //}

        ///// <summary>
        ///// Compare two doubles
        ///// </summary>
        ///// <returns>1, if the first parameter larger than the second one; -1, smaller; 0, equal</returns>
        ///// <remarks>please notice that the default value of _dblVerySmall is 0.0000000000000001, but we usually set a new _dblVerySmall with respect to the data</remarks>
        //public static int Compare(double dbl1, double dbl2)
        //{
        //    return CompareDbl(dbl1, dbl2);
        //}

        //public static int CompareSlope(double dbl1, double dbl2)
        //{
        //    return CompareDbl(dbl1, dbl2);
        //}

        /// <summary>
        /// Compare two doubles
        /// </summary>
        /// <returns>1, if the first parameter larger than the second one; -1, smaller; 0, equal</returns>
        /// <remarks>please notice that the default value of _dblVerySmall is 0.0000000000000001, but we usually set a new _dblVerySmall with respect to the data</remarks>
        public static int CompareDbl_VerySmall(double dbl1, double dbl2)
        {
            //dblVerySmall = 0.1;

            double dblVerySmall = CConstants.dblVerySmallSlope;

            double dblDiff = dbl1 - dbl2;

            if (dblDiff > dblVerySmall)
            {
                return 1;
            }
            else if (dblDiff < -dblVerySmall)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// Compare two cedges, by coordinates
        /// </summary>
        /// <returns>1, if the first parameter larger than the second one; -1, smaller; 0, equal</returns>
        /// <remarks>First comapre FrCpts. If the two cedges have the same FrCpts, then compare ToCpts. 
        /// If they neither have the same FrCpts nor have the same ToCpts, then compare FrCpt with ToCpt as well as ToCpt with FrCpt to find whether they are equal</remarks>
        public static int CompareCEdgeCoordinates(CEdge cedge1, CEdge cedge2)
        {
            var frcpt1 = cedge1.FrCpt;
            var tocpt1 = cedge1.ToCpt;
            var frcpt2 = cedge2.FrCpt;
            var tocpt2 = cedge2.ToCpt;

            //cedge1.PrintMySelf();
            //cedge2.PrintMySelf();
            //when we use a SortedSet to store edges, we want that the edge will have the same position no matter which end is the start
            if (cedge1 .intIncrease ==-1)  
            {
                frcpt1=cedge1.ToCpt;
                tocpt1 = cedge1.FrCpt;
            }

            if (cedge2.intIncrease == -1)
            {
                frcpt2 = cedge2.ToCpt;
                tocpt2 = cedge2.FrCpt;
            }


            int intResultFF = CompareCptYX(frcpt1, frcpt2);
            int intResultTT = CompareCptYX(tocpt1, tocpt2);

            int intResult = CCompareMethods.HandleTwoResults(intResultFF, intResultTT);
            return intResult;
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
        /// <remarks>First comapre y coordinates. If the two points have the same y coordinates, then compare x coordinates</remarks>
        public static int CompareCptYX(CPoint cpt1, CPoint cpt2, bool blnVerySmall = true)
        {
            IComparer<double> cmp = null;
            if (blnVerySmall==true)
            {
                cmp = CCompareDbl_VerySmall.pCompareDbl_VerySmall;
            }

            return CompareDual(cpt1, cpt2, cpt => cpt.Y, cpt => cpt.X, cmp, cmp);
        }

        public static int CompareCptXY(CPoint cpt1, CPoint cpt2, bool blnVerySmall = true)
        {
            IComparer<double> cmp = null;
            if (blnVerySmall == true)
            {
                cmp = CCompareDbl_VerySmall.pCompareDbl_VerySmall;
            }

            return CompareDual(cpt1, cpt2, cpt => cpt.X, cpt => cpt.Y, cmp, cmp);
        }


        //public static int CompareGID<CGeo>(CGeo cgeo1, CGeo cgeo2) where CGeo: CGeometricBase <CGeo>
        //{
        //   return cgeo1.GID.CompareTo(cgeo2.GID);
        //}


        ///// <summary>
        ///// Compare two doubles
        ///// </summary>
        ///// <returns>true, if the two doubles are equal; false, not equal</returns>
        //public static bool CompareTF(double dbl1, double dbl2)
        //{
        //    return ConvertCompareToBool(Compare(dbl1, dbl2));
        //}



        public static int CompareSquare(double dbl1, double dbl2)
        {
            double dblDiff = dbl1 - dbl2;

            if (dblDiff > CConstants.dblVerySmallSquare)
            {
                return 1;
            }
            else if (dblDiff < -CConstants.dblVerySmallSquare)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public static int ComparePower4(double dbl1, double dbl2)
        {
            double dblDiff = dbl1 - dbl2;

            if (dblDiff > CConstants.dblVerySmallPower4)
            {
                return 1;
            }
            else if (dblDiff < -CConstants.dblVerySmallPower4)
            {
                return -1;
            }
            else
            {
                return 0;
            }
        }

        public static int CompareCRegion_CphGIDTypeIndex(CRegion crg1, CRegion crg2)
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
                intResult = CCompareMethods.CompareWithSameElements(crg1.GetCphCol(), crg2.GetCphCol(), cph => cph);  //this will compare the GID of every CPatch in the two SortedDictionary
            }

            if (intResult == 0)
            {
                intResult = CCompareMethods.CompareWithSameElements(crg1.GetCphTypeIndexCol(), crg2.GetCphTypeIndexCol(), intTypeIndex => intTypeIndex);
            }
            return intResult;
        }

        //public static int CompareCRegion_CphGID(CRegion crg1, CRegion crg2)
        //{
        //    int intResult = crg1.CphTypeIndexSD_Area_CphGID.Count.CompareTo(crg2.CphTypeIndexSD_Area_CphGID.Count);

        //    if (intResult == 0)
        //    {
        //        intResult = crg1.intSumCphGID.CompareTo(crg2.intSumCphGID);
        //    }

        //    if (intResult == 0)
        //    {
        //        intResult = CCompareMethods.CompareWithSameElements(crg1.CphTypeIndexSD_Area_CphGID, crg2.CphTypeIndexSD_Area_CphGID, CphTypeIndexKVP => CphTypeIndexKVP.Key);  //this will compare the GID of every CPatch in the two SortedDictionary
        //    }

        //    return intResult;
        //}

        public static int CompareColConsideringCount<T, TOrder>(ICollection<T> pCol1, ICollection<T> pCol2, Func<T, TOrder> orderFunc = null, IComparer<TOrder> cmp = null, bool blnReverse = false)
        {
            int intResult = pCol1.Count.CompareTo(pCol2.Count);

            if (intResult == 0)
            {
                intResult = CCompareMethods.CompareWithSameElements(pCol1, pCol2, orderFunc, cmp);
            }

            if (blnReverse == true)
            {
                intResult = intResult * -1;
            }
            return intResult;
        }

        public static int CompareWithSameElements<T, TOrder>(IEnumerable<T> TEb1, IEnumerable<T> TEb2, Func<T, TOrder> orderFunc, IComparer<TOrder> cmp = null, bool blnReverse = false)
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


        //public static int Compare<T, TOrder>(IEnumerable<T> TEb1, IEnumerable<T> TEb2, Func<T, TOrder> orderFunc, IComparer<TOrder> cmp = null, bool blnReverse = false)
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
        //public static int Compare(SCG.SortedSet<int> intSS1, SCG.SortedSet<int> intSS2)
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
        /// <returns> 1   If dblValue is smaller than dblbound1;
        ///                 2    If dblValue is equal to dblbound1 and smaller than dblbound2
        ///                 0    If dblValue is equal to dblbound1 and equal to dblbound2; this is a very special case, but can happen
        ///                 3    If dblValue is larger than dblbound1 and smaller than dblbound2,                 
        ///                 4    If dblValue is larger than dblbound1 and equal to dblbound2, 
        ///                 5    If dblValue is larger than dblbound1 and larger than dblbound2,</returns>
        public static int Compare(double dblValue, double dblbound1, double dblbound2)
        {
            int intResult = 0;
            if (dblbound1 <= dblbound2)
            {
                intResult = CompareIncrease(dblValue, dblbound1, dblbound2);
            }
            else
            {
                intResult = CompareIncrease(dblValue, dblbound2, dblbound1);
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

        /// <summary>whether dblValue is in (dblbound1, dblbound2)</summary>
        /// <returns> 1   If dblValue is smaller than dblbound1;
        ///           2    If dblValue is equal to dblbound1 and smaller than dblbound2
        ///           0    If dblValue is equal to dblbound1 and equal to dblbound2; this is a very special case, but can happen
        ///           3    If dblValue is larger than dblbound1 and smaller than dblbound2,                 
        ///           4    If dblValue is larger than dblbound1 and equal to dblbound2, 
        ///           5    If dblValue is larger than dblbound1 and larger than dblbound2,</returns>
        public static int CompareIncrease(double dblValue, double dblbound1, double dblbound2)
        {
            int intCompare1 = CompareDbl_VerySmall(dblValue, dblbound1);
            int intCompare2 = CompareDbl_VerySmall(dblValue, dblbound2);

            if (intCompare1 == -1)
            {
                if (intCompare2 == -1)
                {
                    return 1;
                }
                else if (intCompare2 == 0)
                {
                    MessageBox.Show("This cannot happen! Compare!");
                    return -1;
                }
                else  //if  (intCompare2==1)
                {
                    MessageBox.Show("This cannot happen! Compare!");
                    return -1;
                }
            }
            else if (intCompare1 == 0)
            {
                if (intCompare2 == -1)
                {
                    return 2;
                }
                else if (intCompare2 == 0)  //this is a very special case!!!
                {
                    return 0;
                }
                else  //if  (intCompare2==1)
                {
                    MessageBox.Show("This cannot happen! Compare!");
                    return -1;
                }
            }
            else  //if  (intCompare1==1)
            {
                if (intCompare2 == -1)
                {
                    return 3;
                }
                else if (intCompare2 == 0)  //this is a very special case!!!
                {
                    return 4;
                }
                else  //if  (intCompare2==1)
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
        //public static int CompareDual<T, TOrder>(T T1, T T2, Func<T, TOrder> orderFunc1, Func<T, TOrder> orderFunc2, IComparer<TOrder> cmp = null)
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
        public static int CompareDual<T, TOrder1, TOrder2>(T T1, T T2, Func<T, TOrder1> orderFunc1, 
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

        ///// <summary>whether dblValue is in (dblbound1, dblbound2) or  (dblbound2, dblbound1) </summary>
        ///// <returns> If dblValue is in the range, return 1;
        /////                 If dblValue is on the boundary of the range, return 0
        /////                 If dblValue is outside of the boundary, return -1</returns>
        //public static int InBetween(double dblValue, double dblbound1, double dblbound2)
        //{
        //    if (dblbound1 > dblbound2)
        //    {
        //        CHelperFunction.Swap(ref dblbound1, ref dblbound2);
        //    }

        //    int intCompare1 = Compare(dblValue, dblbound1);
        //    int intCompare2 = Compare(dblValue, dblbound2);

        //    if (intCompare1 == -1 || intCompare2 == 1)
        //    {
        //        return -1;
        //    }
        //    else if (intCompare1 == 0 || intCompare2 == 0)
        //    {
        //        return 0;
        //    }
        //    else
        //    {
        //        return 1;
        //    }
        //}
    }

    




    
}
