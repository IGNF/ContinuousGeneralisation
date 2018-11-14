using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ESRI.ArcGIS.Geometry;

namespace MorphingClass.CGeometry
{
    public class CTriangle : PolygonClass
    {
        /// <summary>
        /// 为了节省时间，初始化三角形多边形时是不产生三角形的边和多边形的，而仅仅按顺时针方向存储三角形的三个顶点。
        /// 要使用三角形的边（多边形）进行空间分析，需执行函数FormCEdge（FormPolygon），生成边（多边形）之后，方可使用
        /// </summary>
        /// <remarks>为了方便使用，该三角形建立的时候顺便建立一些对应关系：边_CEdgeLt[i]一定是本三角形与三角形_SETriangleLt[i]的公共边，
        /// 且边_CEdgeLt[i]一定是点_CptLt[i]的对边</remarks>

        private int _intTID;
        private int _intSETriangleNum;    // 共边三角形数量(SETriangle: Share Edge Triangle)
        private List<CPoint> _CptLt;      // 三角形的顶点列(顺时针)
        private List<CEdge> _CEdgeLt;     // 三角形的边列(顺时针)
        private List<CTriangle> _SETriangleLt;

        private bool _blnBuildBend;       //标识是否建立弯曲

        private bool _isBuildBendVisit;   //是否在建立弯曲的层次结构时已经访问过了
        private bool _isCrustTriangle;    //外壳三角形，即包含超三角形顶点的三角形
        private bool _isNeedSide;         // 是否被判定（用于建立某侧约束三角网）
        private bool _isNeedSide2;        // 是否被判定（用于建立某侧约束三角网）
        private bool _isNeedSide3;        // 是否被判定（用于建立某侧约束三角网）
        private bool _isSuperTriangle;    // 是否超三角形
        private bool _isSideJudge;        // 是否被判定（用于建立某侧约束三角网）
        private bool _isSideJudge2;       // 是否被判定（用于建立某侧约束三角网）
        private bool _isRightSide;        // 是否在右边（用于建立某侧约束三角网）
        
        private bool _isTriTypeJudge;     // 是否被确定为某类三角形



        private CPoint _CentroidCpt;

        private string _strTriType;       //三角形类型，分为“I类”“II类”“III类”“IV类”
        private string _strSide = "Undefined";            //折线的左边"Left"或是右边"Right"，初始值为"Undefined"


        /// <summary>
        /// 标识是否建立弯曲
        /// </summary>
        public bool blnBuildBend
        {
            get { return _blnBuildBend; }
            set { _blnBuildBend = value; }
        }


        /// <summary>
        /// 是否在建立弯曲的层次结构时已经访问过了
        /// </summary>
        public bool isBuildBendVisit
        {
            get { return _isBuildBendVisit; }
            set { _isBuildBendVisit = value; }
        }


        /// <summary>
        /// 外壳三角形，即包含超三角形顶点的三角形
        /// </summary>
        public bool isCrustTriangle
        {
            get { return _isCrustTriangle; }
            set { _isCrustTriangle = value; }
        }

        /// <summary>
        /// 是否被判定（用于建立某侧约束三角网）
        /// </summary>
        public bool isNeedSide
        {
            get { return _isNeedSide; }
            set { _isNeedSide = value; }
        }

        /// <summary>
        /// 是否被判定（用于建立某侧约束三角网）
        /// </summary>
        public bool isNeedSide2
        {
            get { return _isNeedSide2; }
            set { _isNeedSide2 = value; }
        }

        /// <summary>
        /// 是否被判定（用于建立某侧约束三角网）
        /// </summary>
        public bool isNeedSide3
        {
            get { return _isNeedSide3; }
            set { _isNeedSide3 = value; }
        }

        /// <summary>
        /// 是否被判定（用于建立某侧约束三角网）
        /// </summary>
        public bool isSideJudge
        {
            get { return _isSideJudge; }
            set { _isSideJudge = value; }
        }

        /// <summary>
        /// 是否被判定（用于建立某侧约束三角网）
        /// </summary>
        public bool isSideJudge2
        {
            get { return _isSideJudge2; }
            set { _isSideJudge2 = value; }
        }


        /// <summary>
        /// 是否在右边（用于建立某侧约束三角网）
        /// </summary>
        public bool isRightSide
        {
            get { return _isRightSide; }
            set { _isRightSide = value; }
        }

        /// <summary>
        /// 是否超三角形
        /// </summary>
        public bool isSuperTriangle
        {
            get { return _isSuperTriangle; }
            set { _isSuperTriangle = value; }
        }

        /// <summary>
        /// 三角形的序号
        /// </summary>
        public int TID
        {
            get { return _intTID; }
            set { _intTID = value; }
        }

        /// <summary>
        /// 共边三角形个数
        /// </summary>
        public int SETriangleNum
        {
            get { return _intSETriangleNum; }
            set { _intSETriangleNum = value; }
        }


        /// <summary>
        /// 三角形的顶点列(顺时针)
        /// </summary>
        public List<CPoint> CptLt
        {
            get { return _CptLt; }
        }

        /// <summary>
        /// 三角形的边列(顺时针)
        /// </summary>
        public List<CEdge> CEdgeLt
        {
            get { return _CEdgeLt; }
        }

        /// <summary>
        /// 共边三角形
        /// </summary>
        public List<CTriangle> SETriangleLt
        {
            get { return _SETriangleLt; }
            set { _SETriangleLt = value; }
        }

        /// <summary>
        /// 是否被确定为某类三角形
        /// </summary>
        public bool isTriTypeJudge
        {
            get { return _isTriTypeJudge; }
            set { _isTriTypeJudge = value; }
        }


        /// <summary>
        /// 三角形中心点
        /// </summary>
        public CPoint CentroidCpt
        {
            get
            {
                _CentroidCpt = new CPoint();
                for (int i = 0; i < _CptLt.Count; i++)
                {
                    _CentroidCpt.X = _CentroidCpt.X + _CptLt[i].X;
                    _CentroidCpt.Y = _CentroidCpt.Y + _CptLt[i].Y;

                }
                _CentroidCpt.X = _CentroidCpt.X / 3;
                _CentroidCpt.Y = _CentroidCpt.Y / 3;
                return _CentroidCpt;
            }
        }

        /// <summary>
        /// 三角形类型："I"、"II"、"III"、"VI"
        /// </summary>
        public string strTriType
        {
            get { return _strTriType; }
            set { _strTriType = value; }
        }


        /// <summary>折线的左边"Left"或是右边"Right"</summary>
        public string strSide
        {
            get { return _strSide; }
            set { _strSide = value; }
        }

        /// <summary>
        /// Initializes a new instance of a triangle
        /// </summary>
        public CTriangle(int intTID)
        {
            _intTID = intTID;
        }

        /// <summary>
        /// Initializes a new instance of a triangle
        /// </summary>
        /// <param name="point1">Vertex 1</param>
        /// <param name="point2">Vertex 2</param>
        /// <param name="point3">Vertex 3</param>
        public CTriangle(CPoint point1, CPoint point2, CPoint point3)
        {
            List<CPoint> cptlt = new List<CPoint>(3);
            cptlt.Add(point1);
            cptlt.Add(point2);
            cptlt.Add(point3);
            _CptLt = cptlt;

            _intTID = -1;
            FormCEdge(cptlt);
            FormPolygon(cptlt);
        }

        /// <summary>
        /// Initializes a new instance of a triangle
        /// </summary>
        /// <param name="point1">Vertex 1</param>
        /// <param name="point2">Vertex 2</param>
        /// <param name="point3">Vertex 3</param>
        public CTriangle(int intTID, CPoint point1, CPoint point2, CPoint point3)
        {

            List<CPoint> cptlt = new List<CPoint>(3);
            cptlt.Add(point1);
            cptlt.Add(point2);
            cptlt.Add(point3);
            _CptLt = cptlt;

            _intTID = intTID;
            FormCEdge(cptlt);
            FormPolygon(cptlt);
        }


        /// <summary>
        /// 生成边(顺时针)
        /// </summary>
        /// <remarks>如果已经生成过边，则直接返回</remarks>
        private void FormCEdge(List<CPoint> cptlt)
        {
            CEdge Edge0 = new CEdge(_CptLt[1], _CptLt[2]);
            CEdge Edge1 = new CEdge(_CptLt[2], _CptLt[0]);
            CEdge Edge2 = new CEdge(_CptLt[0], _CptLt[1]);

            List<CEdge> cedgelt = new List<CEdge>(3);
            cedgelt.Add(Edge0);
            cedgelt.Add(Edge1);
            cedgelt.Add(Edge2);
            _CEdgeLt = cedgelt;
        }

        /// <summary>
        /// 生成多边形(顺时针)
        /// </summary>
        /// <remarks >如果已经生成过边，则直接返回</remarks>
        private void FormPolygon(List<CPoint> cptlt)
        {
            //定义收集点的容器，并添加三角形的三个顶点
            IPointCollection4 pPntCtl = new PolygonClass();
            object Missing = Type.Missing;
            pPntCtl.AddPoint((IPoint)_CptLt[0], ref Missing, ref Missing);
            pPntCtl.AddPoint((IPoint)_CptLt[1], ref Missing, ref Missing);
            pPntCtl.AddPoint((IPoint)_CptLt[2], ref Missing, ref Missing);

            //将容器中的点以多边形的方式输出
            this.SetPointCollection(pPntCtl);
            this.Close();
        }

    }
}
