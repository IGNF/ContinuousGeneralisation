//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//using ESRI.ArcGIS.Geometry;

//using MorphingClass.CUtility;
//using MorphingClass.CEntity;

//namespace MorphingClass.CGeometry
//{
//    public class CPolygon
//    {
//         /// <summary>
//        /// 
//        /// 要使用边（多边形）进行空间分析，需执行函数FormCEdge（FormPolygon），生成边（多边形）之后，方可使用
//        /// </summary>
//        /// <remarks></remarks>

//        private int _intID;

//        private List<CPoint> _CptLt;      // vertices(clockwise)??? ; the first point and the last point in cptlt must have the same coordinates
//        private List<CEdge> _CEdgeLt;     // cedges(clockwise)???
//        private CEdge _cedgeOuterComponent;
//        private List<CEdge> _cedgeLkInnerComponents;     //counter clockwise
//        private SortedDictionary<CPolygon, LinkedList<CEdge>> _AdjacentSD;    //Why SortedDictionary? Because we may need to unite the adjacent elements of two elements, and we also need to consider the shared borders
//        private CPatch _cpatch;
//        private int _intType;

//        private IPolygon4 _pPolygon;

//        private CPoint _CentroidCpt;

//       

//        /// <summary>
//        /// 序号
//        /// </summary>
//        public int ID
//        {
//            get { return _intID; }
//            set { _intID = value; }
//        }



//        /// <summary>
//        /// vertices(clockwise)???
//        /// </summary>
//        public List<CPoint> CptLt
//        {
//            get { return _CptLt; }
//        }

//        /// <summary>
//        /// cedges(clockwise)???
//        /// </summary>
//        public List<CEdge> CEdgeLt
//        {
//            get { return _CEdgeLt; }
//        }

//        /// <summary>
//        /// (counter clockwise)
//        /// </summary>
//        public List<CEdge> cedgeLkInnerComponents
//        {
//            get { return _cedgeLkInnerComponents; }
//            set { _cedgeLkInnerComponents = value; }
//        }


//        public CEdge cedgeOuterComponent
//        {
//            get { return _cedgeOuterComponent; }
//            set { _cedgeOuterComponent = value; }
//        }

//        public SortedDictionary<CPolygon, LinkedList<CEdge>> AdjacentSD
//        {
//            get { return _AdjacentSD; }
//            set { _AdjacentSD = value; }
//        } 

//        /// <summary>
//        /// 中心点
//        /// </summary>
//        public CPoint CentroidCpt
//        {
//            get
//            {
//                _CentroidCpt = new CPoint();
//                for (int i = 0; i < _CptLt.Count; i++)
//                {
//                    _CentroidCpt.X = _CentroidCpt.X + _CptLt[i].X;
//                    _CentroidCpt.Y = _CentroidCpt.Y + _CptLt[i].Y;

//                }
//                _CentroidCpt.X = _CentroidCpt.X / _CptLt.Count;
//                _CentroidCpt.Y = _CentroidCpt.Y / _CptLt.Count;
//                return _CentroidCpt;
//            }
//        }

//        public int intType
//        {
//            get { return _intType; }
//            set { _intType = value; }
//        }

//        public CPatch cpatch
//        {
//            get { return _cpatch; }
//            set { _cpatch = value; }
//        }

//        public IPolygon4 pPolygon
//        {
//            get { return _pPolygon; }
//            set { _pPolygon = value; }
//        }
//        /// <summary>
//        /// Initializes a new instance of a triangle
//        /// </summary>
//        public CPolygon()
//        {
//            _intID = -2;
//        }

//        ///// <summary>
//        ///// Initializes a new instance of a CPolygon
//        ///// </summary>
//        //public CPolygon(List<CPoint> cptlt)
//        //{           
//        //    _CptLt = cptlt;

//        //    _intID = -1;
//        //    FormCEdge(cptlt);
//        //    FormPolygon(cptlt);
//        //}

//        /// <summary>
//        /// Initializes a new instance of a CPolygon, the first point and the last point in cptlt must have the same coordinates
//        /// </summary>
//        public CPolygon(int intID, List<CPoint> cptlt)
//        {
//            _CptLt = cptlt;

//            _intID = intID;
//            _CEdgeLt = CGeoFunc.FormCEdgeLt(cptlt);
//            //FormPolygon(cptlt);
//        }

//        public CPolygon(int intID, IPolygon4 pPolygon)
//        {
//            _intID = intID;
//            this.pPolygon = pPolygon;
//            List<CPoint> cptlt = CHelpFunc.GetCPtLtByIPG(pPolygon);
//            _CptLt = cptlt;

//            _CEdgeLt = CGeoFunc.FormCEdgeLt(cptlt);
//            //FormPolygon(cptlt);
//        }


//        public void SetPolygonAndEdge()
//        {
//            if (this .pPolygon ==null)
//            {
//                IPointCollection4 pPntCtl = CHelpFunc.GetPointCollectionFromCptLt(cptlt);
//                this.pPolygon = pPntCtl as IPolygon4;
//                this.pPolygon.Close();
//            }

//            if (true)
//            {
//                ISegmentCollection pSegmentCollection = this.pPolygon as ISegmentCollection;
//                List<CEdge> pedgelt = _CEdgeLt;
//                for (int i = 0; i < pedgelt.Count ; i++)
//                {
//                    pedgelt[i].pLine = pSegmentCollection.get_Segment(i) as ILine;
//                }
//            }

//        }

//        public void SetEdgeIncidentFace()
//        {
//            foreach (CEdge  cedge in _CEdgeLt)
//            {
//                cedge.cpgIncidentFace = this;
//            }
//        }



//    }
//}
