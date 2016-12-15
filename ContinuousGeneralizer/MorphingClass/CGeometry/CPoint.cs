using System;
using C5;
using System.Collections.Generic;
using System.Text;

using SCG = System.Collections.Generic;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase ;

using MorphingClass.CEntity ;
using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects ;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CGeometry
{
    public class CPoint : CGeometricBase<CPoint>
    {
       

        public double X { set; get; }
        public double Y { set; get; }
        public double Z { set; get; }

        //public static string _fmtcpt="{}{}"

        //SCG.lis
        private static int _intStaticGID;
        private bool _isCtrl;                       //是否为控制点"Control"
        //private bool _isFindCorrespondence;

        private bool _isAdded;
        //private bool _isRemoved;

        private int _intTrajectory;
        //private int _intCorrespondenceNum;

        //private TreeSet<int> _intTS;
        //private List<int> _intCorrespondenceNumLt;
        private List<CPoint> _CorrespondingPtLt;
        //private List<CPoint> _MoveVectorPtLt;
        //private CPoint _CorrespondingPt;
        
        private double _dblTime;
        private double _dblAngleV;
        //private double _dblKeyX;
        //private double _dblKeyY;
        //private double _dblAbsLengthFromStart;
        //private double _dblEdgeLength;
        //public double dblEdgeRatio { set; get; }
        //private double _dblRatioLengthFromStart;
        //private 

        //private string _strBelong;
        //private string _strSweepStatus;
        

        //对点或缓冲区进行"红绿蓝"染色


        //private int _intLID;  //the id of the polyline to which this point belongs
        
        //private int _intXID;  //the id of the points according to the X coordinate

        private IPoint _pPoint;
        private ITinNode _pTinNode;

        private bool _isMoveable;
        private bool _isSteiner;

        //private double this.X;
        //private double this.Y;
        //private double this.Z;

        private CPolyline _BelongedCPolyline;
        //private CEdge _BelongedCEdge;
        private CEdge _IncidentCEdge;   //we always set the edge with the smallest axis angle as the IncidentCEdge
        private CEdge _IncidentCEdge1;
        private CEdge _IncidentCEdge2;
        private CIntersection _ClosestLeftCIntersection;
        private List<CIntersection> _pIntersectionLt;
        private CPolygon _HoleCpg;
        private object _BelongedObject;

        private List<CEdge> _AxisAngleCEdgeLt;

        private CCorrCpts _PairCorrCpt;
        //private CRiver _BelongedCRiver;
        //private CLinearEntity _BelongedCLinearEntity;
        //private CAtBd _BelongedCAtBd;

        public double dblAbsLengthFromStart { set; get; }
        public double dblRatioLengthFromStart { set; get; }
        public double dblEdgeLength { set; get; }

        /// <summary>
        /// 实例化一个点
        /// </summary>
        /// <param name="intID">点号</param>
        /// <param name="dblX">X坐标</param> 
        /// <param name="dblY">Y坐标</param> 
        /// <param name="dblZ">Z坐标</param> 
        /// <returns></returns>
        /// <remarks>Main Constructor</remarks>
        public CPoint(int intID = -1, double dblX = 0, double dblY = 0, double dblZ = 0, bool isSetPoint = false, double dblFactor = 1)
        {
            this.ID = intID;
            this.X = dblX * dblFactor;
            this.Y = dblY * dblFactor;
            this.Z = dblZ * dblFactor;
            this.GID = _intStaticGID++;

            if (isSetPoint == true)
            {
                SetPoint();
            }
        }

        public CPoint(int intID, IPoint pPoint, double dblFactor = 1)
            : this(intID, pPoint.X, pPoint.Y, pPoint.Z, false, dblFactor)
        {
            this.pPoint = pPoint;
        }

        public CPoint(ITinNode fTinNode, bool isSetPoint = false)
            : this(fTinNode.TagValue - 1, fTinNode.X, fTinNode.Y, fTinNode.Z, isSetPoint)
        {
            this.indexID = fTinNode.TagValue - 1;
            this._pTinNode = fTinNode;
        }

        //public CPoint(int intID, int intGID, double dblX, double dblY, double dblZ, bool isSetPoint = false)
        //    : this(intID, dblX, dblY, dblZ, isSetPoint)
        //{
        //    this._intGID = intGID;
        //}

        public double DistanceTo(CPoint other)
        {
            return CGeometricMethods.CalDis(this, other);
        }


        public IPoint JudgeAndSetPoint(double dblFactor=1)
        {
            if (_pPoint == null)
            {
                return SetPoint(dblFactor);
            }
            else
            {
                return _pPoint;
            }
        }

        //this method takes a lot of time, please do not use it if not necessary
        public IPoint SetPoint(double dblFactor=1)
        {
            IPoint ipt = new PointClass();
            ipt.ID = this.ID;
            ipt.PutCoords(this.X / dblFactor, this.Y / dblFactor);
            ipt.Z = this.Z / dblFactor;
            this.pPoint = ipt;
            return ipt;
        }

        public void PrintMySelf()
        {

            //var output = string.Format("{0,3}{1,10}{2,13}{3,10}{4,9}{5,10}{6,7}{7,22}{8,7}{9,22}",
            //    "ID:", this.ID, ";    indexID:", this.indexID , ";    GID:" , this.GID , ";    X:", this.X , ";    Y:", this.Y);

            var output = string.Format(CConstants.strFmtIDs6 + "{6,7}{7,22}{8,7}{9,22}",
                "ID:", this.ID, ";    indexID:", this.indexID, ";    GID:", this.GID, ";    X:", this.X, ";    Y:", this.Y);
            
            
            
            Console.WriteLine(output);


            //Console.WriteLine("ID: " + this.ID + "    indexID:" + this.indexID + "    GID:" + this.GID + "  X:" + this.X  +"  Y:" + this.X);
            

        }

        public override IGeometry JudgeAndSetAEGeometry()
        {
            return JudgeAndSetPoint();
        }

        public override void SetAEGeometryNull()
        {
            _pPoint = null;
        }

        public override IGeometry GetAEObject()
        {
            return _pPoint;
        }

        //public virtual void JudgeAndSetZToZero()
        //{
        //    _pPoint.Z = 0;
        //    this.Z = 0;
        //}

        public int Compare(CPoint other)
        {
            return CCompareMethods.CompareCptYX(this, other);
        }


        /// <summary>
        /// Makes a planar checks for if the points is spatially equal to another point.
        /// </summary>
        /// <param name="other">Point to check against</param>
        /// <returns>True if X and Y values are the same</returns>
        public bool Equals2D(CPoint other)
        {
            return CCompareMethods.ConvertCompareToBool(CCompareMethods.CompareCptYX(this, other));
        }


        /// <summary>
        /// Makes a planar checks for if the points is spatially equal to another point.
        /// </summary>
        /// <param name="other">Point to check against</param>
        /// <returns>True if X and Y values are the same</returns>
        public bool Equals2D(CPoint other, double dblVerySmall)
        {
            return CCompareMethods.ConvertCompareToBool(CCompareMethods.CompareCptYX(this, other));
        }







        public double dblTime
        {
            get { return _dblTime;}
            set { _dblTime = value; }
        }



        public override CPoint Copy()
        {
            return new CPoint(this.ID, this.X, this.Y, this.Z);
        }


        //public string strBelong
        //{
        //    get { return _strBelong; }
        //    set { _strBelong = value; }
        //}



        //public string strSweepStatus
        //{
        //    get { return _strSweepStatus; }
        //    set { _strSweepStatus = value; }
        //}

        public ITinNode pTinNode
        {
            get { return _pTinNode; }
            set { _pTinNode = value; }
        }

        public IPoint pPoint
        {
            get { return _pPoint; }
            set 
            { 
                _pPoint = value;
                //_pGeo = value;
            }
        }

        //public TreeSet<int> intTS
        //{
        //    get { return _intTS; }
        //    set { _intTS = value; }
        //}




        //是否为控制点"Control"
        public bool isCtrl
        {
            get { return _isCtrl; }
            set { _isCtrl = value; }
        }
        
        public bool isMoveable
        {
            get { return _isMoveable; }
            set { _isMoveable = value; }
        }


        public bool isAdded
        {
            get { return _isAdded; }
            set { _isAdded = value; }
        }
        
        //public bool isRemoved
        //{
        //    get { return _isRemoved; }
        //    set { _isRemoved = value; }
        //}

        //public bool isFindCorrespondence
        //{
        //    get { return _isFindCorrespondence; }
        //    set { _isFindCorrespondence = value; }
        //}

        public int intTrajectory
        {
            get { return _intTrajectory; }
            set { _intTrajectory = value; }
        }

        //public int intCorrespondenceNum
        //{
        //    get { return _intCorrespondenceNum; }
        //    set { _intCorrespondenceNum = value; }
        //}

        //public List<int> intCorrespondenceNumLt
        //{
        //    get { return _intCorrespondenceNumLt; }
        //    set { _intCorrespondenceNumLt = value; }
        //}

        public List<CPoint> CorrespondingPtLt
        {
            get { return _CorrespondingPtLt; }
            set { _CorrespondingPtLt = value; }
        }

        //public List<CPoint> MoveVectorPtLt
        //{
        //    get { return _MoveVectorPtLt; }
        //    set { _MoveVectorPtLt = value; }
        //}


        //public CPoint CorrespondingPt
        //{
        //    get { return _CorrespondingPt; }
        //    set { _CorrespondingPt = value; }
        //}

        public double dblAngleV
        {
            get { return _dblAngleV; }
            set { _dblAngleV = value; }
        }





        //public int LID
        //{
        //    get { return _intLID; }
        //    set { _intLID = value; }
        //}



        //public int XID
        //{
        //    get { return _intXID; }
        //    set { _intXID = value; }
        //}

        public CPolyline BelongedCPolyline
        {
            get { return _BelongedCPolyline; }
            set { _BelongedCPolyline = value; }
        }

        //public CEdge BelongedCEdge
        //{
        //    get { return _BelongedCEdge; }
        //    set { _BelongedCEdge = value; }
        //}

        /// <summary>
        /// we always set the edge with the smallest axis angle as the IncidentCEdge
        /// </summary>
        public CEdge IncidentCEdge
        {
            get { return _IncidentCEdge; }
            set { _IncidentCEdge = value; }
        }

        public CEdge IncidentCEdge1
        {
            get { return _IncidentCEdge1; }
            set { _IncidentCEdge1 = value; }
        }

        public CEdge IncidentCEdge2
        {
            get { return _IncidentCEdge2; }
            set { _IncidentCEdge2 = value; }
        }

        public CIntersection ClosestLeftCIntersection
        {
            get { return _ClosestLeftCIntersection; }
            set { _ClosestLeftCIntersection = value; }
        }

        public List<CIntersection> IntersectionLt
        {
            get { return _pIntersectionLt; }
            set { _pIntersectionLt = value; }
        }

        
        public CPolygon HoleCpg
        {
            get { return _HoleCpg; }
            set { _HoleCpg = value; }
        }
        
        public object BelongedObject
        {
            get { return _BelongedObject; }
            set { _BelongedObject = value; }
        }

        public CCorrCpts PairCorrCpt
        {
            get { return _PairCorrCpt; }
            set { _PairCorrCpt = value; }
        }


        public List<CEdge> AxisAngleCEdgeLt
        {
            get { return _AxisAngleCEdgeLt; }
            set { _AxisAngleCEdgeLt = value; }
        }

        public bool isSteiner
        {
            get { return _isSteiner; }
            set { _isSteiner = value; }
        }







        
        //public CLinearEntity BelongedCLinearEntity
        //{
        //    get { return _BelongedCLinearEntity; }
        //    set { _BelongedCLinearEntity = value; }
        //}

        //public CRiver BelongedCRiver
        //{
        //    get { return _BelongedCRiver; }
        //    set { _BelongedCRiver = value; }
        //}



        //public CAtBd BelongedCAtBd
        //{
        //    get { return _BelongedCAtBd; }
        //    set { _BelongedCAtBd = value; }
        //}



        
    }
}
