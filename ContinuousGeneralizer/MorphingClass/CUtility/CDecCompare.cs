using System;
using MorphingClass.CGeometry;
using System.Collections.Generic;
using System.Text;
using System.Collections;

//using MorphingClass;

namespace MorphingClass.CUtility
{
    //Note:
    //the first variable is the new element, while the second variable has been already in the data structure
    //return  1:put the first variable after the second one
    //return -1:put the first variable in front of the second one
    public class CDecCompare
    {
       
    }

    public class CDblDecCompare : IComparer<double>
    {
        //CaseInsensitiveComparer MyCompare = new CaseInsensitiveComparer();
        public int Compare(double x, double y)
        {
            int iResult;
            if (x < y)
                iResult = -1;
            else
                iResult = 1;
            return iResult;
        }

    }


    public class CDblReverseDecCompare : IComparer<double>
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
    public class CIntDecCompare : IComparer<int>
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

    //public class CCptYDecCompare : IComparer<double>
    //{
    //    //CaseInsensitiveComparer MyCompare = new CaseInsensitiveComparer();
    //    public int Compare(CPoint cpt1, CPoint cpt2)
    //    {
    //        int iResult;
    //        if (cpt1 > y)
    //            iResult = 1;
    //        else
    //            iResult = -1;
    //        return iResult;
    //    }
    //}




    public class CCptYXReverseDecCompareTreeSet : IComparer<CPoint>
    {
        //int _intCount = 0;

        //public CCptYXReverseDecCompareTreeSet()
        //{

        //}

        //public CCptYXReverseDecCompareTreeSet(int intCount)
        //{
        //    _intCount = intCount;
        //}

        public int Compare(CPoint cpt1, CPoint cpt2)
        {
            int iResult = -1;
            if (cpt1.Y < cpt2.Y)
                iResult = 1;
            else if (cpt1.Y > cpt2.Y)
            {
                iResult = -1;
            }
            else 
            {
                if (cpt1.X > cpt2.X)
                {
                    iResult = 1;
                }
                else if (cpt1.X < cpt2.X)
                {
                     iResult = -1;
                }
                else 
                {
                    iResult = 0;
                }
            }
            //_intCount++;
            return iResult;
        }
    }


    public class CCptXYDecCompareTreeSet : IComparer<CPoint>
    {
        public int Compare(CPoint cpt1, CPoint cpt2)
        {
            int iResult = -1;
            if (cpt1.X > cpt2.X)
                iResult = 1;
            else if (cpt1.X < cpt2.X)
            {
                iResult = -1;
            }
            else
            {
                if (cpt1.Y > cpt2.Y)
                {
                    iResult = 1;
                }
                else if (cpt1.Y < cpt2.Y)
                {
                    iResult = -1;
                }
                else
                {
                    iResult = 0;
                }
            }
            return iResult;
        }
    }

    //public class CDecComparePatch : IComparer<CPatch>
    //{
    //    //public int Compare(CPatch cpatch1, CPatch cpatch2)
    //    //{
    //    //    int iResult = CGeometricMethods.Compare(crg1.IDLk, crg2.IDLk);
    //    //    return iResult;
    //    //}
    //}

    public class CYXDecComparePoint : IComparer<CPoint>
    {
        public int Compare(CPoint cpt1, CPoint cpt2)
        {
            int iResult = CGeometricMethods.Compare(cpt1, cpt2);
            return iResult;
        }
    }

    public class CDecCompareRegion : IComparer<CRegion>
    {
        public int Compare(CRegion crg1, CRegion crg2)
        {
            int iResult = CGeometricMethods.Compare(crg1.IDLk, crg2.IDLk);           
            return iResult;
        }
    }

    public class CDecCompareEdge : IComparer<CEdge >
    {
        public int Compare(CEdge cedge1, CEdge cedge2)
        {
            int iResult = CGeometricMethods.Compare(cedge1, cedge2);
            return iResult;
        }
    }

    public class CDecComparePolygon : IComparer<CPolygon>
    {
        public int Compare(CPolygon cpg1, CPolygon cpg2)
        {
            int iResult = CGeometricMethods.Compare(cpg1.ID, cpg2.ID);
            return iResult;
        }
    }


    public class CCptYXReverseDecCompare : IComparer<CPoint>
    {
        public int Compare(CPoint cpt1, CPoint cpt2)
        {
            int iResult = -1;
            if (cpt1.Y < cpt2.Y)
                iResult = 1;
            else if (cpt1.Y == cpt2.Y)
            {
                if (cpt1.X > cpt2.X)
                {
                    iResult = 1;
                }
                else if (cpt1.X == cpt2.X)
                {
                    if (cpt1.GID >cpt2.GID)
                    {
                        iResult = 1;
                    }
                    else if (cpt1.GID == cpt2.GID)
                    {
                        iResult = 0;
                    }
                }
            }
            //iResult = -1;
            return iResult;
        }
    }

    public class CCptYGIDReverseDecCompare : IComparer<CPoint>
    {
        public int Compare(CPoint cpt1, CPoint cpt2)
        {
            int iResult = -1;
            if (cpt1.Y < cpt2.Y)
                iResult = 1;
            else if (cpt1.Y == cpt2.Y)
            {
                if (cpt1.GID > cpt2.GID)
                {
                    iResult = 1;
                }
                else if (cpt1.GID == cpt2.GID)
                {
                    iResult = 0;
                }
            }
            //iResult = -1;
            return iResult;
        }
    }

    public class CCptYTraversedXReverseDecCompare : IComparer<CPoint>
    {
        public int Compare(CPoint cpt1, CPoint cpt2)
        {
            int iResult = -1;
            if (cpt1.Y < cpt2.Y)
                iResult = 1;
            else if (cpt1.Y == cpt2.Y)
            {
                if (cpt1.isTraversed == true && cpt2.isTraversed == false )
                {
                    iResult = 1;
                }
                else if (cpt1.isTraversed == false  && cpt2.isTraversed == true )
                {
                     iResult = -1;
                }
                else
                {
                    if (cpt1.X > cpt2.X)
                    {
                        iResult = 1;
                    }
                    else if (cpt1.X == cpt2.X)
                    {
                        if (cpt1.GID > cpt2.GID)
                        {
                            iResult = 1;
                        }
                        else if (cpt1.GID == cpt2.GID)
                        {
                            iResult = 0;
                        }
                    }
                }
            }
            //iResult = -1;
            return iResult;
        }
    }

    public class CCptXGIDDecCompare : IComparer<CPoint>
    {
        public int Compare(CPoint cpt1, CPoint cpt2)
        {
            int iResult = -1;
            if (cpt1.X > cpt2.X)
                iResult = 1; 
            else if (cpt1.X == cpt2.X)
            {
                if (cpt1.GID > cpt2.GID)
                {
                    iResult = 1;
                }
                else if (cpt1.GID == cpt2.GID)
                {
                    iResult = 0;
                }
            }
            return iResult;
        }
    }
}
