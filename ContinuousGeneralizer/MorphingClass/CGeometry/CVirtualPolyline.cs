using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geometry;

using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CGeometry
{
    public class CVirtualPolyline : CGeometricBase<CVirtualPolyline>
    {
        protected int _intFrID;                            //Index in the List.
        protected int _intToID;                            //Index in the List.

        protected int _intMaxDisID;             //距基线最远点的顶点序号       
        protected double _dblMaxDis;           //距基线最远点与基线之间的距离
        protected double _dblMaxDis1;
        protected double _Ratio;
        protected double _Angel;
        protected double _dblRatioLocationAlongBaseline;
        protected bool _isMaxDisCptRightSide;

        public double dblMaxDisLargerThanChildren { set; get; }

        private double _dblRatioforMovePt;
        private double _dblLengthforMovePt;
        private double _dblAngleDiffforMovePt;
        //private double _dblDifffromMovePtX;
        //private double _dblDifffromMovePtY;

        protected CVirtualPolyline _CParentPolyline;
        protected CVirtualPolyline _CLeftPolyline;
        protected CVirtualPolyline _CRightPolyline;

        protected CEdge _pBaseLine;     //弯曲基线(总是由点号小的点指向点号大的点)

        //protected CPoint _FrCpt;
        //protected CPoint _ToCpt;
        //protected CPoint _MaxDisCpt;


        public CVirtualPolyline()
        {
            
        }

        public CVirtualPolyline(int intID, int fintFrID, int fintToID)
        {
            _intID = intID;
            _intFrID = fintFrID;
            _intToID = fintToID;
            _dblMaxDis = -1;
        }


        public void DivideByID(int intDivideIndex)
        {
            _CLeftPolyline = new CVirtualPolyline(_intID, _intFrID, intDivideIndex);
            _CRightPolyline = new CVirtualPolyline(_intID + intDivideIndex, intDivideIndex, _intToID);
        }

        public void SetBaseLine(CPoint frcpt, CPoint tocpt)
        {
            _pBaseLine = new CEdge(frcpt, tocpt);
            _pBaseLine.SetLength();
        }

        /// <summary>属性：直角DP算法的最大距离</summary>
        public double dblMaxDis1
        {
            get { return _dblMaxDis1; }
            set { _dblMaxDis1 = value; }
        }

        /// <summary>the default value is -1</summary>
        public double dblMaxDis
        {
            get { return _dblMaxDis; }
            set { _dblMaxDis = value; }
        }

        /// <summary>属性：某点在基线上投影点的相对位置</summary>
        public double dblRatioLocationAlongBaseline
        {
            get { return _dblRatioLocationAlongBaseline; }
            set { _dblRatioLocationAlongBaseline = value; }
        }

        /// <summary>属性：</summary>
        public int intFrID
        {
            get { return _intFrID; }
            set { _intFrID = value; }
        }

        /// <summary>属性：角度</summary>
        public double Angel
        {
            get { return _Angel; }
            set { _Angel = value; }
        }

        /// <summary>属性：比例</summary>
        public double Ratio
        {
            get { return _Ratio; }
            set { _Ratio = value; }
        }

        /// <summary>属性：</summary>
        public int intToID
        {
            get { return _intToID; }
            set { _intToID = value; }
        }

        /// <summary>属性：距基线最远点的顶点序号</summary>
        public int intMaxDisID
        {
            get { return _intMaxDisID; }
            set { _intMaxDisID = value; }
        }

        //public CPoint FrCpt
        //{
        //    get { return _FrCpt; }
        //    set { _FrCpt = value; }
        //}

        //public CPoint ToCpt
        //{
        //    get { return _ToCpt; }
        //    set { _ToCpt = value; }
        //}

        //public CPoint MaxDisCpt
        //{
        //    get { return _MaxDisCpt; }
        //    set { _MaxDisCpt = value; }
        //}

        public bool isMaxDisCptRightSide
        {
            get { return _isMaxDisCptRightSide; }
            set { _isMaxDisCptRightSide = value; }
        }

        public double dblRatioforMovePt
        {
            get { return _dblRatioforMovePt; }
            set { _dblRatioforMovePt = value; }
        }

        public double dblLengthforMovePt
        {
            get { return _dblLengthforMovePt; }
            set { _dblLengthforMovePt = value; }
        }

        public double dblAngleDiffforMovePt
        {
            get { return _dblAngleDiffforMovePt; }
            set { _dblAngleDiffforMovePt = value; }
        }
        
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

        /// <summary>父亲折线</summary>
        public CVirtualPolyline CParentPolyline
        {
            get { return _CParentPolyline; }
            set { _CParentPolyline = value; }
        }

        /// <summary>左分支折线</summary>
        public CVirtualPolyline CLeftPolyline
        {
            get { return _CLeftPolyline; }
            set { _CLeftPolyline = value; }
        }

        /// <summary>右分支折线</summary>
        public CVirtualPolyline CRightPolyline
        {
            get { return _CRightPolyline; }
            set { _CRightPolyline = value; }
        }

        /// <summary>弯曲基线</summary>
        public CEdge pBaseLine
        {
            get { return _pBaseLine; }
            set { _pBaseLine = value; }
        }


    }
}
