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

        public CIntersection(CEdge cedge1, CEdge cedge2, CPoint pIntersectCpt, CEdge pOverlapCEdge, CEnumIntersectionType penumIntersectionType)
        {
            _CEdge1 = cedge1;
            _CEdge2 = cedge2;
            _IntersectCpt = pIntersectCpt;
            _OverlapCEdge = pOverlapCEdge;
            _enumIntersectionType = penumIntersectionType;
        }

        //public void SetSlope(ref List <CEdge > CEdgeLt)
        //{
        //    CGeometricMethods.SetSlope(ref CEdgeLt);
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
                //if (CCompareMethods.Compare(cedge1.dblSlope, cedge2.dblSlope)==0)  //parallel
                if (CCompareMethods.CompareDbl_VerySmall(cedge1.dblSlope, cedge2.dblSlope) == 0)  //parallel
                {
                    if (CCompareMethods.CompareDbl_VerySmall(cedge1.dblYIntercept, cedge2.dblYIntercept)==0)   //parallel and with the same YIntercept
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
            else    if (cedge1.blnHasSlope == true && cedge2.blnHasSlope == false)
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
                if (CCompareMethods.CompareDbl_VerySmall(cedge1.FrCpt.X, cedge2.FrCpt.X) == 0)   //parallel and with the same X Coordinate
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
            IntersectCpt = new CPoint(-1);  //the intersection of the two lines, not the two line segments
            IntersectCpt.X = cedgeNoSlope.FrCpt.X;
            IntersectCpt.Y = IntersectCpt.X * cedge.dblSlope + cedge.dblYIntercept;

            return DetermineIntersectCase(cedge, cedgeNoSlope, ref IntersectCpt);
        }

        /// <summary>
        ///the intersection of two line segments. The two segments both have slope and not parrallel
        /// </summary>
        private CEnumIntersectionType DetectIntersectionNormal(CEdge cedge1, CEdge cedge2, out CPoint IntersectCpt)
        {
            IntersectCpt = new CPoint(-1);  //the intersection of the two lines, not the two line segments
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
            string strInCEdge1 = CGeometricMethods.InCEdge(IntersectCpt, cedge1);
            string strInCEdge2 = CGeometricMethods.InCEdge(IntersectCpt, cedge2);

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

        private CEnumIntersectionType DetectIntersectionParralel(CEdge cedge1, CEdge cedge2, out CPoint IntersectCpt, out CEdge overlapcedge)
        {
            CEdge auxCEdge1 = cedge1;
            bool blnReversed1 = false;
            if (CCompareMethods.CompareCptXY(cedge1.FrCpt, cedge1.ToCpt) == 1)
            {
                auxCEdge1 = new CEdge(cedge1.ToCpt, cedge1.FrCpt);
                blnReversed1 = true;
            }

            CEdge auxCEdge2 = cedge2;
            bool blnReversed2 = false;
            if (CCompareMethods.CompareCptXY(cedge2.FrCpt, cedge2.ToCpt) == 1)
            {
                auxCEdge2 = new CEdge(cedge2.ToCpt, cedge2.FrCpt);
                blnReversed2 = true;
            }

            CEnumIntersectionType penumIntersectionType = DetectIntersectionParralelIncrease(auxCEdge1, auxCEdge2, out IntersectCpt, out overlapcedge);
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


        private CEnumIntersectionType DetectIntersectionParralelIncrease(CEdge cedge1, CEdge cedge2, out CPoint IntersectCpt, out CEdge overlapcedge)
        {
            if (CCompareMethods.CompareCptXY(cedge1.ToCpt, cedge2.ToCpt) <= 0)   //we compare both x and y so that we can handle two edges which have no slope
            {
                return DetectIntersectionParralelIncreaseCEdge2Righter(cedge1, cedge2, out IntersectCpt, out overlapcedge);
            }
            else
            {
                CEnumIntersectionType enumIntersectionType = DetectIntersectionParralelIncreaseCEdge2Righter(cedge2, cedge1, out IntersectCpt, out overlapcedge);
                return ReverseIntersectionType(enumIntersectionType);
            }
        }

        private CEnumIntersectionType DetectIntersectionParralelIncreaseCEdge2Righter(CEdge cedge1, CEdge cedge2, out CPoint IntersectCpt, out CEdge overlapcedge)
        {
            IntersectCpt = null;
            overlapcedge = null;

            if (CCompareMethods.CompareCptXY(cedge1.ToCpt, cedge2.FrCpt) == -1)   //we compare both x and y so that we can handle two edges haveing no slope
            {
                return CEnumIntersectionType.NoNo;
            }
            else if (CCompareMethods.CompareCptXY(cedge1.ToCpt, cedge2.FrCpt) == 0)   //we compare both x and y so that we can handle two edges haveing no slope
            {
                IntersectCpt = cedge1.ToCpt;
                return CEnumIntersectionType.ToFr;
            }
            else // if (CCompareMethods.CompareXY(cedge1.ToCpt, cedge2.FrCpt) == 1)   //we compare both x and y so that we can handle two edges haveing no slope
            {
                CPoint frcpt = null;
                if (CCompareMethods.CompareCptXY(cedge1.FrCpt, cedge2.FrCpt) == 1)   //we compare both x and y so that we can handle two edges haveing no slope
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
        ///                   InIn: cross </remarks>
        public CEnumIntersectionType enumIntersectionType
        {
            get { return _enumIntersectionType; }
            //set { _enumIntersectionType = value; }
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
    }

    //public class IntersectionType
    //{
    //  string  _NoNo=CEnumIntersectionType.NoNo;
    //  string _FrFr = CEnumIntersectionType.FrFr;
    //  string _FrIn = CEnumIntersectionType.FrIn;
    //  string _FrTo = CEnumIntersectionType.FrTo;
    //  string _InFr = CEnumIntersectionType.InFr;
    //  string _InIn = CEnumIntersectionType.InIn;
    //  string _InTo = CEnumIntersectionType.InTo;
    //  string _ToFr = CEnumIntersectionType.ToFr;
    //  string _ToIn = CEnumIntersectionType.ToIn;
    //  string _ToTo = CEnumIntersectionType.ToTo;
    //  string _Overlap = "_Overlap";
    //  //string _Part = "_Part";
    //  //string _Conver = "_Conver";

    //  public string ReverseType(string)
    //  {




    //  }



    //}


}
