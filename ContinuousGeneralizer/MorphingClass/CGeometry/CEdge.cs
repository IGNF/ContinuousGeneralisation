using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using MorphingClass.CUtility;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CGeometry
{
    //public class CEdge : LineClass  PolylineClass
    public class CEdge : CLineBase<CEdge>
    {
        public double dblLength { get; set; } = CConstants.dblSpecialValue;
        public double dblLengthSquare { get; set; }
        public double dblLengthSquareReciprocal { get; set; }
        public double dblValueForDis { get; set; }
        public double dblIncrX { get; set; }
        public double dblIncrY { get; set; }

        /// <summary>
        /// a list of indices for EdgeGrid 
        /// </summary>
        public List<CValPair<int, int>> RowColVpLt { get; set; }

        private static int _intStaticGID;

        private bool _isBelongToPolyline;   //Is this cedge belongs to a polyline (linear feature) 

        private double _dblSlope = CConstants.dblSpecialValue;
        private double _dblYIntercept;
        //private double _dblRatioforMovePt;
        //private double _dblDifffromMovePtX;
        //private double _dblDifffromMovePtY;

        private bool _isIncidentCEdgeForCpt;
        private bool _blnHasSlope;
        private bool _isStartEdge;
        private bool _isStartEdge1;
        private bool _isStartEdge2;
        private int _intIncrease;



        //private int _intTime = 0;       //匹配成功的次数
        //private CMoveInformation _pMoveInformation;
        //private ILine _pLine;
        protected IPolyline5 _pPolyline;  //because we cannot really save ILine, so we have to use polyline; we can only create a feauture class for esriGeometryPoint, Multipoint, Polyline, Polygon, and MultiPatch
        private ITinEdge _pTinEdge;
        private CEdge _cedgeTwin;
        private CEdge _cedgePrev;
        private CEdge _cedgeNext;
        private CPolygon _cpgIncidentFace;
        //private CEdge _cedgeTwin1;                //we may need to construct the Doubly Connected Edge List twice: once for more-detailed districts, once for less-detailed districts
        private CEdge _cedgePrev1;                //we may need to construct the Doubly Connected Edge List twice: once for more-detailed districts, once for less-detailed districts
        private CEdge _cedgeNext1;               //we may need to construct the Doubly Connected Edge List twice: once for more-detailed districts, once for less-detailed districts
        private CPolygon _cpgIncidentFace1;                //we may need to construct the Doubly Connected Edge List twice: once for more-detailed districts, once for less-detailed districts
        
        //private CEdge _cedgeTwin2;                //we may need to construct the Doubly Connected Edge List twice: once for more-detailed districts, once for less-detailed districts
        private CEdge _cedgePrev2;                //we may need to construct the Doubly Connected Edge List twice: once for more-detailed districts, once for less-detailed districts
        private CEdge _cedgeNext2;               //we may need to construct the Doubly Connected Edge List twice: once for more-detailed districts, once for less-detailed districts
        private CPolygon _cpgIncidentFace2;                //we may need to construct the Doubly Connected Edge List twice: once for more-detailed districts, once for less-detailed districts
        //private LinkedList<CIntersectWith> _IntersectWithLk;
        private List<CIntersection> _IntersectLt;
        private List<List<CEdge>> _CEdgeCellLtLt;

        private double _dblAxisAngle = CConstants.dblSpecialValue;

        private CEdge _ParentCEdge;   //used in the construction of compatible triangulations. we set Parent CEdge so that we know whether or not an edge has been traversed (we don't need to traverse its twin edge), and we know the sub edges of the edge
        private CEdge _CorrRglCEdge;
        private List<CEdge> _CorrRglSubCEdgeLt;
        public SortedDictionary<double, CPoint> BreakCptSD { get; set; }
        public CPolygon LinkedFrCpg { get; set; }
        public CPolygon LinkedToCpg { get; set; }
        public CPolygon BelongedCpg { get; set; }
        public CPolygon BelongedOriginalCpg { get; set; }
        public CPolyline BelongedCpl { get; set; }

        //private string _strLabel;

        /// <summary>
        /// Initializes a new cedge instance
        /// </summary>
        /// <param name="eFrCpt">Start cedge vertex index</param>
        /// <param name="eToCpt">End cedge vertex index</param>
        public CEdge()
        {
            this.GID = _intStaticGID++;
        }

        /// <summary>
        /// Initializes a new cedge instance
        /// </summary>
        /// <param name="eFrCpt">Start cedge vertex index</param>
        /// <param name="eToCpt">End cedge vertex index</param>
        public CEdge(CPoint eFrCpt, CPoint eToCpt, bool isSetLength = false)
        {
            this.GID = _intStaticGID++;
            _FrCpt = eFrCpt;
            _ToCpt = eToCpt;

            _intIncrease = eToCpt.Compare(eFrCpt);

            if (isSetLength == true)
            {
                SetLength();
            }
        }

        public CEdge(ITinEdge pTinEdge, bool isSetLength = false)
            : this(pTinEdge, pTinEdge.FromNode, pTinEdge.ToNode, isSetLength)
        {
        }

        public CEdge(ITinEdge pTinEdge,ITinNode pFrNode, ITinNode pToNode, bool isSetLength = false)
            : this(new CPoint(pFrNode), new CPoint(pToNode), isSetLength)
        {
            _pTinEdge = pTinEdge;
        }

        ///// <summary>
        ///// 指定点沿着曲线到起点的距离
        ///// </summary>
        ///// <param name="pPoint">待计算的点</param>
        ///// <param name="pPolyline">点所在的曲线</param>
        ///// <param name="blnasRatio">指定计算相对距离还是绝对距离</param>
        ///// <returns></returns>
        //public double CalDistanceFromStartPoint(IPoint pPoint, bool blnasRatio)
        //{
        //    IPoint outPoint = new PointClass();
        //    double distanceAlongCurve = 0;//该点在曲线上最近的点距曲线起点的距离
        //    double distanceFromCurve = 0;//该点到曲线的直线距离
        //    bool bRightSide = false;
        //    this.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, pPoint, blnasRatio, outPoint, ref distanceAlongCurve, ref distanceFromCurve, ref bRightSide);
        //    return distanceAlongCurve;
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="distanceAlongCurve"></param>
        /// <param name="distanceFromCurve"></param>
        /// <param name="bRightSide"></param>
        /// <returns>the point</returns>
        //public CPoint QueryPointByDistances(double distanceAlongCurve, bool basRatioAlong, double distanceFromCurve, bool bRightSide)
        //{
        //    ILine pNomalLine = new LineClass();
        //    this._pLine.QueryNormal(esriSegmentExtension.esriExtendEmbedded, 
        //distanceAlongCurve, basRatioAlong, distanceFromCurve, pNomalLine);
        //    //this.SetEmpty();
        //    IPoint outpoint = new PointClass();
        //    if (bRightSide == true)
        //    {
        //        pNomalLine.QueryPoint(esriSegmentExtension.esriNoExtension, 1, true, outpoint);
        //    }
        //    else
        //    {
        //        pNomalLine.QueryPoint(esriSegmentExtension.esriExtendAtFrom, -1, true, outpoint);
        //    }

        //    CPoint cpt = new CPoint(0, outpoint);
        //    return cpt;
        //}

        /// <summary>
        /// you must setlength before using this function
        /// </summary>
        public double QueryPtHeight(CPoint querycpt)
        {
            if (this.blnHasSlope == true)
            {
                return Math.Abs(this.dblYIntercept + this.dblSlope * querycpt.X - querycpt.Y) * this.dblValueForDis;
            }
            else
            {
                return Math.Abs(querycpt.X - this.FrCpt.X);
            }
        }

        //public double DistanceTo(CEdge querycedge, bool blnCheckIntersect = true)
        //{
            //if (blnCheckIntersect == true)
            //{
            //    if (this.IsIntersectWith(querycedge))
            //    {
            //        return 0;
            //    }
            //}

            //var dis1 = this.FrCpt.DistanceTo(querycedge);
            //var dis2 = this.ToCpt.DistanceTo(querycedge);
            //var dis3 = querycedge.FrCpt.DistanceTo(this);
            //var dis4 = querycedge.ToCpt.DistanceTo(this);

            //return Math.Min(dis1, Math.Min(dis2, Math.Min(dis3, dis4)));
        //}



        public CIntersection IntersectWith(CEdge pcedge)
        {
            var pIntersection = new CIntersection(this, pcedge);
            pIntersection.DetectIntersection();
            return pIntersection;
        }

        public bool IsIntersectWith(CEdge pcedge, 
            bool blnTouchBothEnds = false, bool blnTouchEndEdge = false, bool blnOverlap = false)
        {
            var pIntersection = new CIntersection(this, pcedge);
            pIntersection.DetectIntersection();

            return pIntersection.JudgeIntersect(blnTouchBothEnds, blnTouchEndEdge, blnOverlap);
        }

        /// <summary>
        /// It turns out that we do not really need this function
        /// </summary>
        /// <param name="pcedge"></param>
        /// <returns></returns>
        public bool IsTouchWith(CEdge pcedge)
        {
           var pIntersection = new CIntersection(this, pcedge);
           return pIntersection.IsTouch();
        }


        /// <summary>
        /// QueryMovedPt
        /// </summary>
        /// <param name="dblRatioforMovePt">the ratio of length1 (start point to target point) to length2 (start point to end point) on a base line</param>
        /// <param name="dblLengthforMovePt">the distance of the original point (on the polyline) to the targest point (on the base line)</param>
        /// <param name="dblAngleDiffforMovePt">the angle difference of segment1 (target point to original point) and segment2 (target point to end point)</param>
        ///  <param name="dblProportion">t</param>
        ///  <param name="intID">ID of the original point</param>
        /// <returns></returns>
        public CPoint QueryMovedPt(double dblRatioforMovePt, double dblLengthforMovePt, double dblAngleDiffforMovePt, double dblProportion, int intID)
        {

            double dblAngle = _dblAxisAngle - dblAngleDiffforMovePt;
            CPoint targetcpt = CGeoFunc.GetInbetweenCpt(_FrCpt, _ToCpt, dblRatioforMovePt, -1);
            double dblresultLength= (1-dblProportion )*dblLengthforMovePt ;
            double dblresultX = targetcpt.X + dblresultLength * Math.Cos(dblAngle);
            double dblresultY = targetcpt.Y + dblresultLength * Math.Sin(dblAngle);

            return new CPoint(intID, dblresultX, dblresultY);
        }

        //public CEdge GetEdgeIncrease()
        //{
        //    int intCompare = CCmpMethods.Cmp(_FrCpt, _ToCpt);
        //    if (intCompare<=0)
        //    {
        //        return this;
        //    }
        //    else
        //    {
        //        return new CEdge (_ToCpt,_FrCpt);
        //    }
        //}

        public double CalInbetweenCptProportion(CPoint cpt)
        {
            return CGeoFunc.CalInbetweenCptProportion(cpt, _FrCpt, _ToCpt);
        }

        public CPoint GetInbetweenCpt(double dblProportion, int intID = -1)
        {
            return CGeoFunc.GetInbetweenCpt(_FrCpt, _ToCpt, dblProportion, intID);
        }

        public double GetIncrY()
        {
            return this.ToCpt.Y - this.FrCpt.Y;
        }

        public CEdge CreateTwinCEdge()
        {
            if (_cedgeTwin == null)  //in some applications, we have already set twin edge, and we don't want to lose the information of cedgePrev2, cpgIncidentFace2, etc. e.g., in morphing of boundaries
            {
                CEdge TwinCEdge = new CEdge(_ToCpt, _FrCpt);
                TwinCEdge.cedgeTwin = this;
                _cedgeTwin = TwinCEdge;
                JudgeAndSetAxisAngle();
                SetTwinCEdgeAxisAngle();
                //TwinCEdge.BelongedOriginalCpg = this.BelongedOriginalCpg;
            }

            SetCEdgePrevNextAsTwin();
            return _cedgeTwin;
        }

        public void SetCEdgePrevNextAsTwin()
        {
            CEdge cedge = this;
            cedge.cedgePrev = cedge.cedgeTwin;
            cedge.cedgeNext = cedge.cedgeTwin;

            cedge.cedgeTwin.cedgePrev = cedge;
            cedge.cedgeTwin.cedgeNext = cedge;
        }


        public void SetCEdgeToCpts()
        {
            this.FrCpt.OutCEdge = this;
            this.ToCpt.InCEdge = this;
        }

        public void SetLength()
        {
            this.dblIncrX = _ToCpt.X - _FrCpt.X;
            this.dblIncrY = _ToCpt.Y - _FrCpt.Y;
            this.dblLengthSquare= this.dblIncrX * this.dblIncrX + this.dblIncrY * this.dblIncrY;
            this.dblLength = Math.Sqrt(dblLengthSquare);         
        }

        public void SetLengthSquareReciprocal()
        {
            this.dblLengthSquareReciprocal = 1 / this.dblLengthSquare;

        }


        /// <summary>
        /// only if you set length, will this function be safe
        /// </summary>
        public double GetUnsafeInbetweenX(double t)
        {
            return _FrCpt.X + t * this.dblIncrX;
        }

        /// <summary>
        /// only if you set length, will this function be safe
        /// </summary>
        public double GetUnsafeInbetweenY(double t)
        {
            return _FrCpt.Y + t * this.dblIncrY;
        }

        public void JudgeAndSetLength()
        {
            if (this.dblLength == CConstants.dblSpecialValue)
            {
                SetLength();
            }
        }

        public double JudgeAndSetAxisAngle()
        {
            if (_dblAxisAngle == CConstants.dblSpecialValue)
            {
                return SetAxisAngle();
            }
            else
            {
                return _dblAxisAngle;
            }
        }

        public double SetAxisAngle()
        {
            _dblAxisAngle = CGeoFunc.CalAxisAngle(_FrCpt, _ToCpt);
            return _dblAxisAngle;
        }

        public void SetTwinCEdgeAxisAngle()
        {
            _cedgeTwin.dblAxisAngle = CGeoFunc.CalReversedCEdgeAxisAngle(_dblAxisAngle);
        }

        public void JudgeAndSetSlope()
        {
            if (_dblSlope == CConstants.dblSpecialValue)
            {
                SetSlope();
            }
        }



        public bool SetSlope()
        {
            this.dblIncrX = _ToCpt.X - _FrCpt.X;
            this.dblIncrY = _ToCpt.Y - _FrCpt.Y;
            //for we want to check topological relationships between edges, we use dblVerySmallCoordFixed instead of dblVerySmall, 
            //where dblVerySmall will be changed during checking topological relationships
            //if (CCmpMethods.CmpDblRange(dblXDiff, 0, CConstants.dblVerySmallCoordFixed) == 0)
            //if (dblXDiff == 0)   //this case would result we cannot identify a verticle line
            if (CCmpMethods.CmpCoordDbl_VerySmall(this.dblIncrX, 0) == 0)
            {
                _blnHasSlope = false;
            }
            else
            {
                _blnHasSlope = true;
                _dblSlope = this.dblIncrY / this.dblIncrX;
                _dblYIntercept = _FrCpt.Y - _dblSlope * _FrCpt.X;
            }

            return _blnHasSlope;
        }

        public void SetDenominatorForDis()
        {
            this.dblValueForDis = 1 / Math.Sqrt(1 + _dblSlope * _dblSlope);
        }
        

        //public void CalMoveInformation(CPoint cpt)
        //{
        //    CPoint frcpt = _FrCpt;
        //    CPoint tocpt = _ToCpt;

        //    double dblPreDis = frcpt.DistanceTo(cpt);
        //    double dblSucDis = tocpt.DistanceTo(cpt);
        //    double dblRatio = dblPreDis / (dblPreDis + dblSucDis);

        //    double dblTargetX = (1 - dblRatio) * frcpt.X + dblRatio * tocpt.X;
        //    double dblTargetY = (1 - dblRatio) * frcpt.Y + dblRatio * tocpt.Y;

        //    _dblRatioforMovePt = dblRatio;
        //    _dblDifffromMovePtX = cpt.X - dblTargetX;
        //    _dblDifffromMovePtY = cpt.Y - dblTargetY;
        //}


        ////this method takes a lot of time, please do not use it if not necessary
        //public ILine SetLine()
        //{
        //    ILine pline = new LineClass();
        //    pline.PutCoords(_FrCpt.JudgeAndSetPoint(), _ToCpt.JudgeAndSetPoint());
        //    _pLine = pline;
        //    return pline;
        //}

        //public ILine JudgeAndSetLine()
        //{
        //    if (_pLine == null)
        //    {
        //        return SetLine();
        //    }
        //    else
        //    {
        //        return _pLine;
        //    }
        //}

        public override IGeometry JudgeAndSetAEGeometry()
        {
            return JudgeAndSetPolyline();
        }

        public override IGeometry GetAEObject()
        {
            return _pPolyline;
        }

        public IPolyline5 JudgeAndSetPolyline()
        {
            if (_pPolyline == null)
            {
                return SetPolyline();
            }
            else
            {
                return _pPolyline;
            }
        }

        //this method takes a lot of time, please do not use it if not necessary
        public IPolyline5 SetPolyline()
        {
            var cptlt = new List<CPoint>(2);
            cptlt.Add(_FrCpt);
            cptlt.Add(_ToCpt);
            _pPolyline = CGeoFunc.GetIplFromCptLt(cptlt);
            return _pPolyline;
        }

        //public int Compare(CEdge other)
        //{
        //    return CCmpMethods.CmpCEdgeCoordinates(this, other, true);
        //}

        public void PrintMySelf()
        {
            Console.WriteLine("-----------------------Print an Edge--------------------------");
            Console.WriteLine(string.Format(CConstants.strFmtIDs6 + "{6,7}{7,22}", 
                "ID:", this.ID, ";    indexID:", this.indexID, ";    GID:", this.GID, ";    Angle:", this.dblAxisAngle));
            this.FrCpt.PrintMySelf();
            this.ToCpt.PrintMySelf();

            if (this.cedgePrev != null)
            {
                Console.WriteLine("Previous GID: " + this.cedgePrev.GID + 
                    "    Next GID: " + this.cedgeNext.GID + "    Twin GID:" + this.cedgeTwin.GID);
            }
            Console.WriteLine("***********************End Print an Edge**************************");
            Console.WriteLine("");
        }

        //public CEdge LargerAxisAngleCEdge()
        //{
        //    return this .cedgePrev .cedgeTwin ;
        //}

        //public CEdge SmallerAxisAngleCEdge()
        //{
        //    return this.cedgeTwin.cedgeNext;
        //}

        //public CEdge LargerAxisAngleCEdge
        //{
        //    get { return _cedgePrev.cedgeTwin; }
        //    set { _cedgePrev.cedgeTwin = value; }
        //}

        //public CEdge SmallerAxisAngleCEdge
        //{
        //    get { return _cedgeTwin.cedgeNext; }
        //    set { _cedgeTwin.cedgeNext = value; }
        //}

        public CEdge GetLargerAxisAngleCEdge(int intIndex = 0)
        {
            if (intIndex == 0)
            {
                return _cedgePrev.cedgeTwin;
            }
            else if (intIndex == 1)
            {
                return _cedgePrev1.cedgeTwin;
            }
            else //if (intIndex == 2)
            {
                return _cedgePrev2.cedgeTwin;
            }
        }

        public CEdge GetSmallerAxisAngleCEdge(int intIndex = 0)
        {
            if (intIndex == 0)
            {
                return _cedgeTwin.cedgeNext;
            }
            else if (intIndex == 1)
            {
                return _cedgeTwin.cedgeNext1;
            }
            else //if (intIndex == 2)
            {
                return _cedgeTwin.cedgeNext2;
            }
        }



        ///// <summary>
        ///// 匹配成功的次数
        ///// </summary>
        //public int intTime
        //{
        //    get { return _intTime; }
        //    set { _intTime = value; }
        //}





        public double dblSlope
        {
            get { return _dblSlope; }
            set { _dblSlope = value; }
        }


        /// <summary>
        /// 该边是否为折线上的边
        /// </summary>
        public bool isBelongToPolyline
        {
            get { return _isBelongToPolyline; }
            set { _isBelongToPolyline = value; }
        }

        //public ILine pLine
        //{
        //    get { return _pLine; }
        //    set { _pLine = value; }
        //}

        public ITinEdge pTinEdge
        {
            get { return _pTinEdge; }
            set { _pTinEdge = value; }
        }

        public CPolygon cpgIncidentFace
        {
            get { return _cpgIncidentFace; }
            set { _cpgIncidentFace = value; }
        }

        public CEdge cedgeTwin
        {
            get { return _cedgeTwin; }
            set { _cedgeTwin = value; }
        }

        public CEdge cedgePrev
        {
            get { return _cedgePrev; }
            set { _cedgePrev = value; }
        }

        public CEdge cedgeNext
        {
            get { return _cedgeNext; }
            set { _cedgeNext = value; }
        }

        public CPolygon cpgIncidentFace1
        {
            get { return _cpgIncidentFace1; }
            set { _cpgIncidentFace1 = value; }
        }

        //public CEdge cedgeTwin1
        //{
        //    get { return _cedgeTwin1; }
        //    set { _cedgeTwin1 = value; }
        //}

        public CEdge cedgePrev1
        {
            get { return _cedgePrev1; }
            set { _cedgePrev1 = value; }
        }

        public CEdge cedgeNext1
        {
            get { return _cedgeNext1; }
            set { _cedgeNext1 = value; }
        }

        public CPolygon cpgIncidentFace2
        {
            get { return _cpgIncidentFace2; }
            set { _cpgIncidentFace2 = value; }
        }

        //public CEdge cedgeTwin2
        //{
        //    get { return _cedgeTwin2; }
        //    set { _cedgeTwin2 = value; }
        //}

        public CEdge cedgePrev2
        {
            get { return _cedgePrev2; }
            set { _cedgePrev2 = value; }
        }

        public CEdge cedgeNext2
        {
            get { return _cedgeNext2; }
            set { _cedgeNext2 = value; }
        }

        //public LinkedList<CIntersectWith> IntersectWithLk
        //{
        //    get { return _IntersectWithLk; }
        //    set { _IntersectWithLk = value; }
        //}

        public List<CIntersection> IntersectLt
        {
            get { return _IntersectLt; }
            set { _IntersectLt = value; }
        }

        ///// <summary>
        ///// a list of cells, and a cell is a list of edges
        ///// </summary>
        //public List<List<CEdge>> CEdgeCellLtLt
        //{
        //    get { return _CEdgeCellLtLt; }
        //    set { _CEdgeCellLtLt = value; }
        //}



        //private double _dblSlope;
        //private double _YIntercept;

        public double dblYIntercept
        {
            get { return _dblYIntercept; }
            set { _dblYIntercept = value; }
        }

        public bool blnHasSlope
        {
            get { return _blnHasSlope; }
            set { _blnHasSlope = value; }
        }

        public bool isIncidentCEdgeForCpt
        {
            get { return _isIncidentCEdgeForCpt; }
            set { _isIncidentCEdgeForCpt = value; }
        }

        public bool isStartEdge
        {
            get { return _isStartEdge; }
            set { _isStartEdge = value; }
        }

        public bool isStartEdge1
        {
            get { return _isStartEdge1; }
            set { _isStartEdge1 = value; }
        }

        public bool isStartEdge2
        {
            get { return _isStartEdge2; }
            set { _isStartEdge2 = value; }
        }

        /// <summary>
        /// if (tocpt.y > frcpt.y) or (tocpt.y == frcpt.y && tocpt.x >= frcpt.x), then intIncrease = true 
        /// </summary>
        public int intIncrease
        {
            get { return _intIncrease; }
            set { _intIncrease = value; }
        }


        

        public double dblAxisAngle
        {
            get { return _dblAxisAngle; }
            set { _dblAxisAngle = value; }
        }

        //public double dblRatioforMovePt
        //{
        //    get { return _dblRatioforMovePt; }
        //    set { _dblRatioforMovePt = value; }
        //}

        //public double dblDifffromMovePtX
        //{
        //    get { return _dblDifffromMovePtX; }
        //    set { _dblDifffromMovePtX = value; }
        //}

        //public double dblDifffromMovePtY
        //{
        //    get { return _dblDifffromMovePtY; }
        //    set { _dblDifffromMovePtY = value; }
        //}



        public CEdge CorrRglCEdge
        {
            get { return _CorrRglCEdge; }
            set { _CorrRglCEdge = value; }
        }
        
        public CEdge ParentCEdge
        {
            get { return _ParentCEdge; }
            set { _ParentCEdge = value; }
        }

        public List<CEdge> CorrRglSubCEdgeLt
        {
            get { return _CorrRglSubCEdgeLt; }
            set { _CorrRglSubCEdgeLt = value; }
        }
    }
}