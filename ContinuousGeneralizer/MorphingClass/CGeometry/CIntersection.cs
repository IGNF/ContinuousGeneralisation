using System;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CUtility;

namespace MorphingClass.CGeometry
{
    public class CIntersection
    {
        private CEdge _CEdge1;   //usually, CEdge1 is the edge itself, CEdge2 is the other edge
        private CEdge _CEdge2;   //usually, CEdge1 is the edge itself, CEdge2 is the other edge
        private CPoint _IntersectCpt;
        private CEdge _OverlapCEdge;
        private CEnumIntersectionType _enumIntersectionType;   //-1, no intersect; 0, the intersection on both boundaires; 1, the intersection on the boundary of CEdge2 but not on the boundary of CEdge1; 2, analogously to 1; 3, CEdge1 and CEdge2 cross each other; 4, overlap
        public CPolygon CEdge2IncidentFace { set; get; }  //we use this for constructing DCEL

        public CIntersection()
        {

        }

        public CIntersection(CEdge cedge1, CEdge cedge2)
        {
            _CEdge1 = cedge1;
            _CEdge2 = cedge2;
        }

        public CIntersection(CEdge cedge1, CEdge cedge2, CPoint pIntersectCpt, CEdge pOverlapCEdge, 
            CEnumIntersectionType penumIntersectionType)
        {
            _CEdge1 = cedge1;
            _CEdge2 = cedge2;
            _IntersectCpt = pIntersectCpt;
            _OverlapCEdge = pOverlapCEdge;
            _enumIntersectionType = penumIntersectionType;
        }

        //public void SetSlope(ref List <CEdge > CEdgeLt)
        //{
        //    CGeoFunc.SetSlope(ref CEdgeLt);
        //}


        /// <summary>Detect the intesection between two edges</summary>
        /// <returns >the intersection point</returns>
        /// <remarks>the edges have to be set slope before using this function
        ///                  If the two edges overlap, then the IntersectCpt is the point with larger XY coordinate of the two ends of the overlap line segment
        /// </remarks>
        public void DetectIntersection()
        {
            CEdge cedge1 = _CEdge1;
            CEdge cedge2 = _CEdge2;

            cedge1.JudgeAndSetSlope();
            cedge2.JudgeAndSetSlope();

            CPoint IntersectCpt = null;
            CEdge overlapcedge = null;
            CEnumIntersectionType penumIntersectionType = CEnumIntersectionType.NoNo;


            if (cedge1.blnHasSlope == true && cedge2.blnHasSlope == true)   //this is the normal case
            {
                //if (CCmpMethods.Cmp(cedge1.dblSlope, cedge2.dblSlope)==0)  //parallel
                if (CCmpMethods.CmpDbl_ConstVerySmall(cedge1.dblSlope, cedge2.dblSlope) == 0)  //parallel
                {
                    if (CCmpMethods.CmpDbl_CoordVerySmall(cedge1.dblYIntercept, cedge2.dblYIntercept)==0)   //parallel and with the same YIntercept
                    {
                        penumIntersectionType = DetectIntersectionParralel(cedge1, cedge2, out IntersectCpt, out overlapcedge);
                    }
                    else    //parallel but not with the same YIntercept
                    {
                        penumIntersectionType = CEnumIntersectionType.NoNo;
                    }
                }
                else  //not parallel
                {
                    penumIntersectionType = DetectIntersectionNormal(cedge1, cedge2, out IntersectCpt);
                }
            }
            else if (cedge1.blnHasSlope == true && cedge2.blnHasSlope == false)
            {
                penumIntersectionType = DetectIntersectionOneNoSlope(cedge1, cedge2, out IntersectCpt);
            }
            else if (cedge1.blnHasSlope == false && cedge2.blnHasSlope == true)
            {
                penumIntersectionType = DetectIntersectionOneNoSlope(cedge2, cedge1, out IntersectCpt);
                penumIntersectionType = ReverseIntersectionType(penumIntersectionType);
            }
            else if (cedge1.blnHasSlope == false && cedge2.blnHasSlope == false)
            {
                if (CCmpMethods.CmpDbl_CoordVerySmall(cedge1.FrCpt.X, cedge2.FrCpt.X) == 0)   //parallel and with the same X Coordinate
                {
                    penumIntersectionType = DetectIntersectionParralel(cedge1, cedge2, out IntersectCpt, out overlapcedge);
                }
                else    //parallel but not with the same X Coordinate
                {
                    penumIntersectionType = CEnumIntersectionType.NoNo;
                }
            }

            _enumIntersectionType = penumIntersectionType;
            _IntersectCpt = IntersectCpt;
            _OverlapCEdge = overlapcedge;

        }

        


        /// <summary>Detect the intesection between two edges</summary>
        private CEnumIntersectionType DetectIntersectionOneNoSlope(CEdge cedge, CEdge cedgeNoSlope, out CPoint IntersectCpt)
        {
            IntersectCpt = new CPoint();  //the intersection of the two lines, not the two line segments
            IntersectCpt.X = cedgeNoSlope.FrCpt.X;
            IntersectCpt.Y = IntersectCpt.X * cedge.dblSlope + cedge.dblYIntercept;

            return DetermineIntersectCase(cedge, cedgeNoSlope, ref IntersectCpt);
        }

        /// <summary>
        ///the intersection of two line segments. The two segments both have slope and not parrallel
        /// </summary>
        private CEnumIntersectionType DetectIntersectionNormal(CEdge cedge1, CEdge cedge2, out CPoint IntersectCpt)
        {
            IntersectCpt = new CPoint();  //the intersection of the two lines, not the two line segments
            IntersectCpt.X = (cedge2.dblYIntercept - cedge1.dblYIntercept) / (cedge1.dblSlope - cedge2.dblSlope);
            IntersectCpt.Y = IntersectCpt.X * cedge1.dblSlope + cedge1.dblYIntercept;

            return DetermineIntersectCase(cedge1, cedge2, ref IntersectCpt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cedge1"></param>
        /// <param name="cedge2"></param>
        /// <param name="IntersectCpt"></param>
        /// <returns></returns>
        private CEnumIntersectionType DetermineIntersectCase(CEdge cedge1, CEdge cedge2, ref CPoint IntersectCpt)
        {
            string strInCEdge1 = InCEdge(IntersectCpt, cedge1);
            string strInCEdge2 = InCEdge(IntersectCpt, cedge2);

            //to save memory, we always use the original points if the intersection overlaps an original point
            //we set the intersection to null if there is no intersection
            if (strInCEdge1=="No"||strInCEdge2=="No")
            {
                IntersectCpt = null;
            }
            else if (strInCEdge1=="Fr")
            {
                IntersectCpt = cedge1.FrCpt;
            }
            else if (strInCEdge1=="To")
            {
                IntersectCpt = cedge1.ToCpt;
            }
            else //if(strInCEdge1 == "In")
            {
                if (strInCEdge2=="Fr")
                {
                    IntersectCpt=cedge2.FrCpt;
                }
                else if (strInCEdge2 == "To")
                {
                    IntersectCpt = cedge2.ToCpt;
                }
                else //if(strInCEdge2 == "In")
                {
                    //IntersectCpt = IntersectCpt;
                }
            }


            if (strInCEdge1=="No"||strInCEdge2=="No")
            {
                return CEnumIntersectionType.NoNo;  //do we really need do this seperately?
            }
            else
            {
                string strIntersectionType = strInCEdge1 + strInCEdge2;
                return (CEnumIntersectionType)Enum.Parse(typeof(CEnumIntersectionType), strIntersectionType);
            }
        }

        private CEnumIntersectionType DetectIntersectionParralel(CEdge cedge1, CEdge cedge2, 
            out CPoint IntersectCpt, out CEdge overlapcedge)
        {
            CEdge auxCEdge1 = cedge1;
            bool blnReversed1 = false;
            if (CCmpMethods.CmpCptXY(cedge1.FrCpt, cedge1.ToCpt) == 1)
            {
                auxCEdge1 = new CEdge(cedge1.ToCpt, cedge1.FrCpt);
                blnReversed1 = true;
            }

            CEdge auxCEdge2 = cedge2;
            bool blnReversed2 = false;
            if (CCmpMethods.CmpCptXY(cedge2.FrCpt, cedge2.ToCpt) == 1)
            {
                auxCEdge2 = new CEdge(cedge2.ToCpt, cedge2.FrCpt);
                blnReversed2 = true;
            }

            CEnumIntersectionType penumIntersectionType = 
                DetectIntersectionParralelIncrease(auxCEdge1, auxCEdge2, out IntersectCpt, out overlapcedge);
            if (penumIntersectionType == CEnumIntersectionType.FrTo || penumIntersectionType == CEnumIntersectionType.ToFr)   //besides, penumIntersectionType coud be CEnumIntersectionType.NoNo or CEnumIntersectionType.Overlap
            {
                string substr1 = penumIntersectionType.ToString().Substring(0, 2);
                string substr2 = penumIntersectionType.ToString().Substring(2, 2);
                if (blnReversed1 == true)
                {
                    substr1 = ReverseEnd(substr1);
                }
                if (blnReversed2 == true)
                {
                    substr2 = ReverseEnd(substr2);
                }

                string strIntersectionType = substr1 + substr2;
                return (CEnumIntersectionType)Enum.Parse(typeof(CEnumIntersectionType), strIntersectionType);
            }
            return penumIntersectionType;
        }

        private string ReverseEnd(string strEnd)
        {
            if (strEnd == "Fr")
            {
                return "To";
            }
            else if (strEnd == "To")
            {
                return "Fr";
            }
            else
            {
                MessageBox.Show("Mistake in ReverseEnd!");
                return null;
            }
        }


        private CEnumIntersectionType DetectIntersectionParralelIncrease(CEdge cedge1, CEdge cedge2, 
            out CPoint IntersectCpt, out CEdge overlapcedge)
        {
            if (CCmpMethods.CmpCptXY(cedge1.ToCpt, cedge2.ToCpt) <= 0)   //we compare both x and y so that we can handle two edges which have no slope
            {
                return DetectIntersectionParralelIncreaseCEdge2Righter(cedge1, cedge2, out IntersectCpt, out overlapcedge);
            }
            else
            {
                CEnumIntersectionType enumIntersectionType = 
                    DetectIntersectionParralelIncreaseCEdge2Righter(cedge2, cedge1, out IntersectCpt, out overlapcedge);
                return ReverseIntersectionType(enumIntersectionType);
            }
        }

        private CEnumIntersectionType DetectIntersectionParralelIncreaseCEdge2Righter(CEdge cedge1, CEdge cedge2, 
            out CPoint IntersectCpt, out CEdge overlapcedge)
        {
            IntersectCpt = null;
            overlapcedge = null;

            if (CCmpMethods.CmpCptXY(cedge1.ToCpt, cedge2.FrCpt) == -1)   //we compare both x and y so that we can handle two edges haveing no slope
            {
                return CEnumIntersectionType.NoNo;
            }
            else if (CCmpMethods.CmpCptXY(cedge1.ToCpt, cedge2.FrCpt) == 0)   //we compare both x and y so that we can handle two edges haveing no slope
            {
                IntersectCpt = cedge1.ToCpt;
                return CEnumIntersectionType.ToFr;
            }
            else // if (CCmpMethods.CmpXY(cedge1.ToCpt, cedge2.FrCpt) == 1)   //we compare both x and y so that we can handle two edges haveing no slope
            {
                CPoint frcpt = null;
                if (CCmpMethods.CmpCptXY(cedge1.FrCpt, cedge2.FrCpt) == 1)   //we compare both x and y so that we can handle two edges haveing no slope
                {
                    frcpt = cedge1.FrCpt;
                }
                else
                {
                    frcpt = cedge2.FrCpt;
                }
                //overlapcedge = new CEdge(frcpt, cedge1.ToCpt);  //to save memory, we don't record the overlapcedge
                IntersectCpt = cedge1.ToCpt;
                return CEnumIntersectionType.Overlap;
            }
        }

        public bool JudgeIntersect(
           bool blnTouchBothEnds = false, bool blnTouchEndEdge = false, bool blnOverlap = false)
        {
            bool blnIntersect = false;
            switch (this.enumIntersectionType)
            {
                case CEnumIntersectionType.NoNo:
                    blnIntersect = false;
                    break;
                case CEnumIntersectionType.FrFr:
                    blnIntersect = blnTouchBothEnds;
                    break;
                case CEnumIntersectionType.FrIn:
                    blnIntersect = blnTouchEndEdge;
                    break;
                case CEnumIntersectionType.FrTo:
                    blnIntersect = blnTouchBothEnds;
                    break;
                case CEnumIntersectionType.InFr:
                    blnIntersect = blnTouchEndEdge;
                    break;
                case CEnumIntersectionType.InIn:
                    blnIntersect = true;
                    break;
                case CEnumIntersectionType.InTo:
                    blnIntersect = blnTouchEndEdge;
                    break;
                case CEnumIntersectionType.ToFr:
                    blnIntersect = blnTouchBothEnds;
                    break;
                case CEnumIntersectionType.ToIn:
                    blnIntersect = blnTouchEndEdge;
                    break;
                case CEnumIntersectionType.ToTo:
                    blnIntersect = blnTouchBothEnds;
                    break;
                case CEnumIntersectionType.Overlap:
                    blnIntersect = blnOverlap;
                    break;
                default:
                    break;
            }
            return blnIntersect;
        }

        


        public CIntersection ReverseIntersection()
        {
            return new CIntersection(_CEdge2, _CEdge1, _IntersectCpt, _OverlapCEdge, ReverseIntersectionType(_enumIntersectionType));
        }

        private CEnumIntersectionType ReverseIntersectionType(CEnumIntersectionType penumIntersectionType)
        {
            CEnumIntersectionType ReversedEnumIntersectionType = penumIntersectionType;
            switch (penumIntersectionType )
            {
                case CEnumIntersectionType.NoNo:
                    break;
                case CEnumIntersectionType.FrFr:
                    break;
                case CEnumIntersectionType.FrIn:
                    ReversedEnumIntersectionType = CEnumIntersectionType.InFr;
                    break;
                case CEnumIntersectionType.FrTo:
                    ReversedEnumIntersectionType = CEnumIntersectionType.ToFr;
                    break;
                case CEnumIntersectionType.InFr:
                    ReversedEnumIntersectionType = CEnumIntersectionType.FrIn;
                    break;
                case CEnumIntersectionType.InIn:
                    break;
                case CEnumIntersectionType.InTo:
                    ReversedEnumIntersectionType = CEnumIntersectionType.ToIn;
                    break;
                case CEnumIntersectionType.ToFr:
                    ReversedEnumIntersectionType = CEnumIntersectionType.FrTo;
                    break;
                case CEnumIntersectionType.ToIn:
                    ReversedEnumIntersectionType = CEnumIntersectionType.InTo;
                    break;
                case CEnumIntersectionType.ToTo:
                    break;
                case CEnumIntersectionType.Overlap:
                    break;
                default:
                    break;
            }
            return ReversedEnumIntersectionType;
        }

        ///// <summary>whether a point is in cedge (we already know that the point is on the line that goes through the cedge)</summary>
        ///// <returns> -1, nonsense,
        /////                   1, this point is not on the Edge
        /////                   2, this point is the FrCpt
        /////                   3, this point is in the Edge
        /////                   4, this point is the ToCpt</returns>
        /////                   <remarks></remarks>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpt"></param>
        /// <param name="cedge"></param>
        /// <returns> "No", "Fr", "In", "To"</returns>
        public static string InCEdge(CPoint cpt, CEdge cedge)
        {

            //we test both x and y. if we only test one coordinate, we may get wrong result.
            //      for example, there is an intersection that is actually in an edge, and this edge is almost vertical.
            //      if we only test the x coordinate, then we will get that the intersection is the end (on the boundary) of this edge 
            //         because we use dblVerysmall to judge

            //cedge.SetLength();
            int intCompareX = CCmpMethods.CmpThreeCoord(cpt.X, cedge.FrCpt.X, cedge.ToCpt.X);
            int intCompareY = CCmpMethods.CmpThreeCoord(cpt.Y, cedge.FrCpt.Y, cedge.ToCpt.Y);

            //the result is a result of a 6*6 matrix
            string strInCEdge = "";
            if (intCompareX == 1 || intCompareX == 5 || intCompareY == 1 || intCompareY == 5)   //this point is not on the Edge
            {
                strInCEdge = "No";
            }
            else if (intCompareX == 3 || intCompareY == 3)   //this point is in the Edge
            {
                strInCEdge = "In";
            }
            else // if (intCompareX, intCompareY = 0, 2, 4),    //this point is one of the ends of the Edge
            {
                if (intCompareX == 0)
                {
                    if (intCompareY == 0 || intCompareY == 2)
                    {
                        strInCEdge = "Fr";
                    }
                    else  //if  (intCompareY==4)
                    {
                        strInCEdge = "To";
                    }
                }
                else if (intCompareX == 2)
                {
                    if (intCompareY == 0 || intCompareY == 2)
                    {
                        strInCEdge = "Fr";
                    }
                    else  //if  (intCompareY==4)    //unfortunately, this can happen!!!
                    {
                        //MessageBox.Show("This cannot happen! InCEdge!");  //
                        strInCEdge = "In";
                    }
                }
                else //if (intCompareX == 4)
                {
                    if (intCompareY == 0 || intCompareY == 4)
                    {
                        strInCEdge = "To";
                    }
                    else  //if  (intCompareY==2)
                    {
                        //MessageBox.Show("This cannot happen! InCEdge!");
                        strInCEdge = "In";
                    }
                }
            }

            return strInCEdge;
        }


        /// <summary>
        /// The concept of touch is simpler than the type of intersection
        /// </summary>
        /// <returns></returns>
        /// <remarks>It turns out that we do not really need this function</remarks>
        public bool IsTouch()
        {
            CEdge cedge1 = _CEdge1;
            CEdge cedge2 = _CEdge2;

            cedge1.JudgeAndSetSlope();
            cedge2.JudgeAndSetSlope();

            CPoint IntersectCpt = null;
            //CEdge overlapcedge = null;
            //CEnumIntersectionType penumIntersectionType = CEnumIntersectionType.NoNo;


            if (cedge1.blnHasSlope == true && cedge2.blnHasSlope == true)   //this is the normal case
            {
                //if (CCmpMethods.Cmp(cedge1.dblSlope, cedge2.dblSlope)==0)  //parallel
                if (CCmpMethods.CmpDbl_ConstVerySmall(cedge1.dblSlope, cedge2.dblSlope) == 0)  //parallel
                {
                    if (CCmpMethods.CmpDbl_CoordVerySmall(cedge1.dblYIntercept, cedge2.dblYIntercept) == 0)
                    {
                        //parallel and with the same YIntercept
                        return IsTouchParralel(cedge1, cedge2, out IntersectCpt);
                    }
                    else    //parallel but not with the same YIntercept
                    {
                        return false;
                    }
                }
                else  //not parallel
                {
                    return IsTouchNormal(cedge1, cedge2, out IntersectCpt);
                }
            }
            else if (cedge1.blnHasSlope == true && cedge2.blnHasSlope == false)
            {
                return IsTouchOneNoSlope(cedge1, cedge2, out IntersectCpt);
            }
            else if (cedge1.blnHasSlope == false && cedge2.blnHasSlope == true)
            {
                return IsTouchOneNoSlope(cedge2, cedge1, out IntersectCpt);
            }
            else if (cedge1.blnHasSlope == false && cedge2.blnHasSlope == false)
            {
                //parallel and with the same X Coordinate
                if (CCmpMethods.CmpDbl_CoordVerySmall(cedge1.FrCpt.X, cedge2.FrCpt.X) == 0)   
                {
                    return IsTouchParralel(cedge1, cedge2, out IntersectCpt);
                }
                else    //parallel but not with the same X Coordinate
                {
                    return false;
                }
            }

            //_enumIntersectionType = penumIntersectionType;
            _IntersectCpt = IntersectCpt;
            //_OverlapCEdge = overlapcedge;
            throw new ArgumentException("unexpected case!");

        }

        /// <summary>
        ///the intersection of two line segments. The two segments both have slope and not parrallel
        /// </summary>
        private bool IsTouchNormal(CEdge cedge1, CEdge cedge2, out CPoint IntersectCpt)
        {
            IntersectCpt = new CPoint();  //the intersection of the two lines, not the two line segments
            IntersectCpt.X = (cedge2.dblYIntercept - cedge1.dblYIntercept) / (cedge1.dblSlope - cedge2.dblSlope);
            IntersectCpt.Y = IntersectCpt.X * cedge1.dblSlope + cedge1.dblYIntercept;

            return DetermineIsTouch(cedge1, cedge2, ref IntersectCpt);
        }

        private bool IsTouchParralel(CEdge cedge1, CEdge cedge2, out CPoint IntersectCpt)
        {
            if (IsOnCEdge(cedge1.FrCpt, cedge2) == true)
            {
                IntersectCpt = cedge1.FrCpt;
                return true;
            }
            else if (IsOnCEdge(cedge1.ToCpt, cedge2) == true)
            {
                IntersectCpt = cedge1.ToCpt;
                return true;
            }
            else if (IsOnCEdge(cedge2.FrCpt, cedge1) == true)
            {
                IntersectCpt = cedge2.FrCpt;
                return true;
            }
            else if (IsOnCEdge(cedge2.ToCpt, cedge1) == true)
            {
                IntersectCpt = cedge2.ToCpt;
                return true;
            }
            else
            {
                IntersectCpt = null;
                return false;
            }
        }

        /// <summary>Detect the intesection between two edges</summary>
        private bool IsTouchOneNoSlope(CEdge cedge, CEdge cedgeNoSlope, out CPoint IntersectCpt)
        {
            IntersectCpt = new CPoint();  //the intersection of the two lines, not the two line segments
            IntersectCpt.X = cedgeNoSlope.FrCpt.X;
            IntersectCpt.Y = IntersectCpt.X * cedge.dblSlope + cedge.dblYIntercept;

            return DetermineIsTouch(cedge, cedgeNoSlope, ref IntersectCpt);
        }

        private bool DetermineIsTouch(CEdge cedge1, CEdge cedge2, ref CPoint IntersectCpt)
        {
            if (IsOnCEdge(IntersectCpt, cedge1) == false || IsOnCEdge(IntersectCpt, cedge2) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsOnCEdge(CPoint cpt, CEdge cedge)
        {
            if (CCmpMethods.IsInbetween(cpt.X, cedge.FrCpt.X, cedge.ToCpt.X) == false ||
                CCmpMethods.IsInbetween(cpt.Y, cedge.FrCpt.Y, cedge.ToCpt.Y) == false)
            {
                return false;
            }
            else
            {
                return true;
            }
        }


        public CEdge CEdge1
        {
            get { return _CEdge1; }
            //set { _CEdge1 = value; }
        }

        public CEdge CEdge2
        {
            get { return _CEdge2; }
            set { _CEdge2 = value; }
        }

        public CEdge OverlapCEdge
        {
            get { return _OverlapCEdge; }
            //set { _OverlapCEdge = value; }
        }

        public CPoint IntersectCpt
        {
            get { return _IntersectCpt; }
            set { _IntersectCpt = value; }

        }



        /// <summary>
        /// COULD BE: NoNo, FrFr, FrIn, FrTo, InFr, InIn, InTo, ToFr, ToIn, ToTo, Overlap
        /// </summary>
        /// <remarks >NoNo: no intersection
        ///           InIn: cross </remarks>
        public CEnumIntersectionType enumIntersectionType
        {
            get { return _enumIntersectionType; }
            //set { _enumIntersectionType = value; }
        }
    }



    


}
