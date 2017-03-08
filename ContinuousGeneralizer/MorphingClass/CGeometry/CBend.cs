using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;

using MorphingClass.CUtility;

namespace MorphingClass.CGeometry
{
    public class CBend: PolylineClass
    {
        /// <summary>
        /// 为了节省时间，初始化弯曲时不产生弯曲线段，而仅仅依照原线段顺序存储各节点
        /// 要使用弯曲线段行空间分析，需执行函数FormBend，生成弯曲线段之后，方可使用
        /// </summary>
        private List<CPoint> _CptLt=new List<CPoint> ();
        private List<CBend> _pBendLt;             //所有弯曲的列表（包括重合弯曲）
        private List<int> _intTIDLt=new List<int> ();
        private List<CTriangle> _CTriangleLt = new List<CTriangle>();  //属于本弯曲的三角形
        private SortedList<double, CBend> _pCorrespondBendLt = new SortedList<double, CBend>(new CCompareDbl());  //对应弯曲列表
        private int _intPathCount;

        private int _ID;
        private int _intPathTriCount;   //路径TID数量：该弯曲到各弯曲底部的三角形数量之和（越高层的三角形被重复计算的次数越多）
        private double _dblBendDepthAverage;  //该弯曲的平均深度
        private double _dblBendDepthMax;
        private double _dblBendDepthSum;
        private double _dblBendDepthSumHelp;
        private double _dblStartRL;        //该弯曲在原线状要素上的相对起始位置(Start Ratio Location)
        private double _dblEndRL;          //该弯曲在原线状要素上的相对终点位置(End Ratio Location)

        private string _strSide = "Undefined";         //该弯曲在折线的左边"Left"或是右边"Right"，初始值为"Undefined"

        private CBend _CParentBend;
        private CBend _CLeftBend;
        private CBend _CRightBend;

        private ILine _pBaseLine;     //弯曲基线(总是由点号小的点指向点号大的点)
        private CPolyline _pBaseLineDeep; //该弯曲孩子弯曲的外基线(总是由点号小的点指向点号大的点)

        private bool _isMatch;

        /// <summary>所有弯曲的列表（包括重合弯曲）</summary>
        public List<CBend> pBendLt
        {
            get { return _pBendLt; }
            set { _pBendLt = value; }
        }

        /// <summary>点列</summary>
        public List<CPoint> CptLt
        {            
            get { return _CptLt; }
            set { _CptLt = value; }
        }

        /// <summary>左分支弯曲</summary>
        public List<int> intTIDLt
        {
            get { return _intTIDLt; }
            set { _intTIDLt = value; }
        }

        /// <summary>属于本弯曲的三角形</summary>
        public List<CTriangle> CTriangleLt
        {
            get { return _CTriangleLt; }
            set { _CTriangleLt = value; }
        }

        /// <summary>对应弯曲列表</summary>
        public SortedList<double, CBend> pCorrespondBendLt
        {
            get { return _pCorrespondBendLt; }
            set { _pCorrespondBendLt = value; }
        }

        /// <summary>弯曲序号</summary>
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        /// <summary>分支数量（低层次弯曲数量）</summary>
        public int intPathCount
        {
            get { return _intPathCount; }
            set { _intPathCount = value; }
        }

        /// <summary>路径TID数量：该弯曲到各弯曲底部的三角形数量之和（越高层的三角形被重复计算的次数越多）</summary>
        public int intPathTriCount
        {
            get { return _intPathTriCount; }
            set { _intPathTriCount = value; }
        }

        /// <summary>该弯曲的平均深度</summary>
        public double dblBendDepthAverage
        {
            get { return _dblBendDepthAverage; }
            set { _dblBendDepthAverage = value; }
        }

        /// <summary>该弯曲的最大深度</summary>
        public double dblBendDepthMax
        {
            get { return _dblBendDepthMax; }
            set { _dblBendDepthMax = value; }
        }
        
        /// <summary>该弯曲的最大深度</summary>
        public double dblBendDepthSum
        {
            get { return _dblBendDepthSum; }
            set { _dblBendDepthSum = value; }
        }


        /// <summary>该弯曲的最大深度</summary>
        public double dblBendDepthSumHelp
        {
            get { return _dblBendDepthSumHelp; }
            set { _dblBendDepthSumHelp = value; }
        }

        /// <summary>该弯曲在原线状要素上的相对起始位置(Start Ratio Location)</summary>
        public double dblStartRL
        {
            get { return _dblStartRL; }
            set { _dblStartRL = value; }
        }

        /// <summary>该弯曲在原线状要素上的相对终点位置(End Ratio Location)</summary>
        public double dblEndRL
        {
            get { return _dblEndRL; }
            set { _dblEndRL = value; }
        }

        /// <summary>该弯曲位于线状要素的哪一边</summary>
        public string strSide
        {
            get { return _strSide; }
            set { _strSide = value; }
        }

        /// <summary>父亲弯曲</summary>
        public CBend CParentBend
        {
            get { return _CParentBend; }
            set { _CParentBend = value; }
        }

        /// <summary>左分支弯曲</summary>
        public CBend CLeftBend
        {
            get { return _CLeftBend; }
            set { _CLeftBend = value; }
        }

        /// <summary>右分支弯曲</summary>
        public CBend CRightBend
        {
            get { return _CRightBend; }
            set { _CRightBend = value; }
        }

        /// <summary>分支数量（低层次弯曲数量）</summary>
        public bool isMatch
        {
            get { return _isMatch; }
            set { _isMatch = value; }
        }

        /// <summary>弯曲基线</summary>
        public ILine pBaseLine
        {
            get { return _pBaseLine; }
            set { _pBaseLine = value; }
        }

        /// <summary>该弯曲孩子弯曲的外基线</summary>
        public CPolyline pBaseLineDeep
        {
            get { return _pBaseLineDeep; }
            set { _pBaseLineDeep = value; }
        }


        //public CBend()
        //{


        //}

        public CBend(List<CPoint> cptlt)
        {
            _CptLt = cptlt;
            FormBend(cptlt);
        }

        public CBend(List <CPoint > cptlt,string strSide)
        {
            _CptLt = cptlt;
            _strSide = strSide;
            FormBend(cptlt);
        }

        

        /// <summary>
        /// 通过弯曲上两节点获取该弯曲的子弯曲
        /// </summary>
        /// <param name="cpt1">需获取的子弯曲上的终点之一</param>
        /// <param name="cpt2">需获取的子弯曲上的终点之一 2</param>
        public CBend GetSubBend(CPoint cpt1,CPoint cpt2, string strSide,double dblVerySmall)
        {
            if (cpt1.Equals2D(cpt2) == true)
            {
                MessageBox.Show("两个点相同，无法提取弯曲");
                return null;
            }

            List<CPoint> ptlt = new List<CPoint>();
            int intBegin = 0;
            for (int i = 0; i < _CptLt.Count ; i++)
            {
                //第一次遇到相同点的时候（intBegin==1），即是需提取弯曲的初始点；第二次遇到相同点时（intBegin==2），即是需提取弯曲的终点
                if (_CptLt[i].Equals2D(cpt1, dblVerySmall) == true || _CptLt[i].Equals2D(cpt2, dblVerySmall) == true) intBegin = intBegin + 1;
                if (intBegin > 0) ptlt.Add(_CptLt[i]);
                if (intBegin == 2) break;
            }

            CBend CSubBend = new CBend(ptlt, strSide);
            return CSubBend;
        }


        /// <summary>
        /// 通过弯曲上两节点的点号获取该弯曲的子弯曲
        /// </summary>
        /// <param name="ID1">需获取的子弯曲上的终点之一的点号</param>
        /// <param name="ID1">需获取的子弯曲上的终点之一的点号 2</param>
        public CBend GetSubBend(int ID1, int ID2, string strSide)
        {
            if (ID1 == ID2)
            {
                MessageBox.Show("两个点相同，无法提取弯曲");
                return null;
            }

            List<CPoint> ptlt = new List<CPoint>();
            int intBegin = 0;
            for (int i = 0; i < _CptLt.Count; i++)
            {
                //第一次遇到相同点的时候（intBegin==1），即是需提取弯曲的初始点；第二次遇到相同点时（intBegin==2），即是需提取弯曲的终点
                if (_CptLt[i].ID == ID1 || _CptLt[i].ID == ID2) intBegin = intBegin + 1;
                if (intBegin > 0) ptlt.Add(_CptLt[i]);
                if (intBegin == 2) break;
            }

            CBend CSubBend = new CBend(ptlt, strSide);
            return CSubBend;
        }













        /// <summary>
        /// 按照原线段方向生成弯曲
        /// </summary>
        /// <remarks>如果已经生成过边，则直接返回</remarks>
        public void FormBend(List <CPoint > cptlt)
        {
            object Missing = Type.Missing;
            IPointCollection4 pCol = new PolylineClass();
            for (int i = 0; i < cptlt.Count; i++)
            {
                pCol.AddPoint((IPoint)cptlt[i], ref Missing, ref Missing);
            }
            this.SetPointCollection(pCol);

            //生成基线
            _pBaseLine = new LineClass();
            _pBaseLine.PutCoords((IPoint)cptlt[0], (IPoint)cptlt[cptlt.Count - 1]);
            
        }





    }
}
