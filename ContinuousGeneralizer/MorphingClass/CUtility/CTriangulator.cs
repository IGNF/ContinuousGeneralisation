using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;

using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DataSourcesFile;

namespace MorphingClass.CUtility
{
    /// <summary>
    /// Performs the Delauney triangulation on a set of vertices.
    /// </summary>
    /// <remarks>
    /// Based on Paul Bourke's "An Algorithm for Interpolating Irregularly-Spaced Data
    /// with Applications in Terrain Modelling"
    /// http://astronomy.swin.edu.au/~pbourke/modelling/triangulate/
    /// </remarks>
    public class CTriangulator
    {
        private int _intMaxID;
        private object _Missing = Type.Missing;
        private double _dblVerySmall;
        

        #region 创建三角网（非AE代码）


        //private int _intBendDepthCount;
        //private double _dblBendDepthSum;
        //private double _dblBendForestDepthAverage;


        ///// <summary>属性：弯曲森林平均深度</summary>
        //public double dblBendForestDepthAverage
        //{
        //    get { return _dblBendForestDepthAverage; }
        //}

        /// <summary>
        /// Performs Delauney triangulation on a set of points.
        /// </summary>
        /// <remarks>
        /// <para>
        /// The triangulation doesn't support multiple points with the same planar location.
        /// Vertex-lists with duplicate points may result in strange triangulation with intersecting EdgeLt.
        /// To avoid adding multiple points to your vertex-list you can use the following anonymous predicate
        /// method:
        /// <code>
        /// if(!Vertices.Exists(delegate(Triangulator.Geometry.Point p) { return pNew.Equals2D(p); }))
        ///		Vertices.Add(pNew);
        /// </code>
        /// </para>
        /// <para>The triangulation algorithm may be described in pseudo-code as follows:
        /// <code>
        /// subroutine Triangulate
        /// input : vertex list
        /// output : triangle list
        ///    initialize the triangle list
        ///    determine the supertriangle
        ///    add supertriangle vertices to the end of the vertex list
        ///    add the supertriangle to the triangle list
        ///    for each sample point in the vertex list
        ///       initialize the cedge buffer
        ///       for each triangle currently in the triangle list
        ///          calculate the triangle circumcircle center and radius
        ///          if the point lies in the triangle circumcircle then
        ///             add the three triangle EdgeLt to the cedge buffer
        ///             remove the triangle from the triangle list
        ///          endif
        ///       endfor
        ///       delete all doubly specified EdgeLt from the cedge buffer
        ///          this leaves the EdgeLt of the enclosing polygon only
        ///       add to the triangle list all triangles formed between the point 
        ///          and the EdgeLt of the enclosing polygon
        ///    endfor
        ///    remove any triangles from the triangle list that use the supertriangle vertices
        ///    remove the supertriangle vertices from the vertex list
        /// end
        /// </code>
        ///// </para>
        /// </remarks>
        /// <param name="Vertex">List of vertices to triangulate.</param>        
        /// <returns>Triangles referencing vertex indices arranged in clockwise order
        /// 最终，顶点列不变，而返回的三角形列中包含含超三角形顶点的所有三角形</returns>        
        public List<CTriangle> Triangulate(ref List<CPoint> VertexLt)
        {

            int intVertexCount = VertexLt.Count;
            if (intVertexCount < 3)
                throw new ArgumentException("Need at least three vertices for triangulation");

            int intTriMax = 4 * intVertexCount;

            // Find the maximum and minimum vertex bounds.
            // This is to allow calculation of the bounding supertriangle
            double xmin = VertexLt[0].X;
            double ymin = VertexLt[0].Y;
            double xmax = xmin;
            double ymax = ymin;
            for (int i = 1; i < intVertexCount; i++)
            {
                if (VertexLt[i].X < xmin) xmin = VertexLt[i].X;
                if (VertexLt[i].X > xmax) xmax = VertexLt[i].X;
                if (VertexLt[i].Y < ymin) ymin = VertexLt[i].Y;
                if (VertexLt[i].Y > ymax) ymax = VertexLt[i].Y;
            }

            double dx = xmax - xmin;
            double dy = ymax - ymin;
            double dmax = (dx > dy) ? dx : dy;

            double xmid = (xmax + xmin) * 0.5;
            double ymid = (ymax + ymin) * 0.5;

            // Set up the supertriangle
            // This is a triangle which encompasses all the sample points.
            // The supertriangle coordinates are added to the end of the
            // vertex list. The supertriangle is the first triangle in
            // the triangle list.
            int intMaxID = GetMaxID(VertexLt);
            _intMaxID = intMaxID;
            VertexLt.Add(new CPoint(intMaxID + 1, (xmid - 2 * dmax), (ymid - dmax)));
            VertexLt.Add(new CPoint(intMaxID + 2, xmid, (ymid + 2 * dmax)));
            VertexLt.Add(new CPoint(intMaxID + 3, (xmid + 2 * dmax), (ymid - dmax)));

            List<CTriangle> TriangleLt = new List<CTriangle>();
            TriangleLt.Add(new CTriangle(VertexLt[intVertexCount], VertexLt[intVertexCount + 1], VertexLt[intVertexCount + 2])); //SuperTriangle placed at index 0

            // Include each point one at a time into the existing mesh
            for (int i = 0; i < intVertexCount; i++)
            {
                List<CEdge> EdgeLt = new List<CEdge>(); //[trimax * 3];
                // Set up the cedge buffer.
                // If the point (Vertex(i).x,Vertex(i).y) lies inside the circumcircle then the
                // three EdgeLt of that triangle are added to the cedge buffer and the triangle is removed from list.
                for (int j = 0; j < TriangleLt.Count; j++)
                {
                    if (InCircle(VertexLt[i], TriangleLt[j].CptLt[0], TriangleLt[j].CptLt[1], TriangleLt[j].CptLt[2]))
                    {
                        EdgeLt.Add(new CEdge(TriangleLt[j].CptLt[0], TriangleLt[j].CptLt[1]));
                        EdgeLt.Add(new CEdge(TriangleLt[j].CptLt[1], TriangleLt[j].CptLt[2]));
                        EdgeLt.Add(new CEdge(TriangleLt[j].CptLt[2], TriangleLt[j].CptLt[0]));
                        TriangleLt.RemoveAt(j);
                        j--;
                    }
                }
                if (i >= intVertexCount) continue; //In case we the last duplicate point we removed was the last in the array(应该不会发生这个情况)

                // Remove duplicate EdgeLt
                // Note: if all triangles are specified anticlockwise then all
                // interior EdgeLt are opposite pointing in direction.
                for (int j = EdgeLt.Count - 2; j >= 0; j--)
                {
                    for (int k = EdgeLt.Count - 1; k >= j + 1; k--)
                    {
                        if (EdgeLt[j].Equals(EdgeLt[k]))
                        {
                            EdgeLt.RemoveAt(k);
                            EdgeLt.RemoveAt(j);
                            k--;
                            continue;
                        }
                    }
                }
                // Form new triangles for the current point
                // Skipping over any tagged EdgeLt.
                // All EdgeLt are arranged in clockwise order.
                for (int j = 0; j < EdgeLt.Count; j++)
                {
                    if (TriangleLt.Count >= intTriMax)
                        throw new ApplicationException("Exceeded maximum EdgeLt");
                    TriangleLt.Add(new CTriangle(EdgeLt[j].FrCpt, EdgeLt[j].ToCpt, VertexLt[i]));
                }
                EdgeLt.Clear();
                EdgeLt = null;
            }

            //// Remove triangles with supertriangle vertices
            //// These are triangles which have a vertex number greater than nv
            //for (int i = TriangleLt.Count - 1; i >= 0; i--)
            //{
            //    if (TriangleLt[i].CptLt[0].ID > intMaxID || TriangleLt[i].CptLt[1].ID > intMaxID || TriangleLt[i].CptLt[2].ID > intMaxID)
            //        TriangleLt[i].isCrustTriangle = true;
            //        //TriangleLt.RemoveAt(i);

            //}

            //Remove SuperTriangle vertices
            VertexLt.RemoveAt(VertexLt.Count - 1);
            VertexLt.RemoveAt(VertexLt.Count - 1);
            VertexLt.RemoveAt(VertexLt.Count - 1);
            TriangleLt.TrimExcess();
            return TriangleLt;
        }

        /// <summary>
        /// Returns true if the point (p) lies inside the circumcircle made up by points (p1,p2,p3)
        /// </summary>
        /// <remarks>
        /// NOTE: A point on the cedge is inside the circumcircle
        /// </remarks>
        /// <param name="p">Point to check</param>
        /// <param name="p1">First point on circle</param>
        /// <param name="p2">Second point on circle</param>
        /// <param name="p3">Third point on circle</param>
        /// <returns>true if p is inside circle</returns>
        private static bool InCircle(CPoint cpt, CPoint cpt1, CPoint cpt2, CPoint cpt3)
        {
            //Return TRUE if the point (xp,yp) lies inside the circumcircle
            //made up by points (x1,y1) (x2,y2) (x3,y3)
            //NOTE: A point on the cedge is inside the circumcircle

            if (System.Math.Abs(cpt1.Y - cpt2.Y) < double.Epsilon && System.Math.Abs(cpt2.Y - cpt3.Y) < double.Epsilon)
            {
                //INCIRCUM - F - Points are coincident !!
                return false;
            }

            double m1, m2;
            double mx1, mx2;
            double my1, my2;
            double xc, yc;

            if (System.Math.Abs(cpt2.Y - cpt1.Y) < double.Epsilon)
            {
                m2 = -(cpt3.X - cpt2.X) / (cpt3.Y - cpt2.Y);
                mx2 = (cpt2.X + cpt3.X) * 0.5;
                my2 = (cpt2.Y + cpt3.Y) * 0.5;
                //Calculate CircumCircle center (xc,yc)
                xc = (cpt2.X + cpt1.X) * 0.5;
                yc = m2 * (xc - mx2) + my2;
            }
            else if (System.Math.Abs(cpt3.Y - cpt2.Y) < double.Epsilon)
            {
                m1 = -(cpt2.X - cpt1.X) / (cpt2.Y - cpt1.Y);
                mx1 = (cpt1.X + cpt2.X) * 0.5;
                my1 = (cpt1.Y + cpt2.Y) * 0.5;
                //Calculate CircumCircle center (xc,yc)
                xc = (cpt3.X + cpt2.X) * 0.5;
                yc = m1 * (xc - mx1) + my1;
            }
            else
            {
                m1 = -(cpt2.X - cpt1.X) / (cpt2.Y - cpt1.Y);
                m2 = -(cpt3.X - cpt2.X) / (cpt3.Y - cpt2.Y);
                mx1 = (cpt1.X + cpt2.X) * 0.5;
                mx2 = (cpt2.X + cpt3.X) * 0.5;
                my1 = (cpt1.Y + cpt2.Y) * 0.5;
                my2 = (cpt2.Y + cpt3.Y) * 0.5;
                //Calculate CircumCircle center (xc,yc)
                xc = (m1 * mx1 - m2 * mx2 + my2 - my1) / (m1 - m2);
                yc = m1 * (xc - mx1) + my1;
            }

            double dx = cpt2.X - xc;
            double dy = cpt2.Y - yc;
            double rsqr = dx * dx + dy * dy;
            //double r = Math.Sqrt(rsqr); //Circumcircle radius
            dx = cpt.X - xc;
            dy = cpt.Y - yc;
            double drsqr = dx * dx + dy * dy;

            return (drsqr <= rsqr);
        }



        /// <summary>
        /// 获取最大的ID号
        /// </summary>
        /// <param name="cptlt">顶点列</param> 
        /// <returns>点列中各点的“ID”的最大值</returns>
        private int GetMaxID(List<CPoint> cptlt)
        {
            int intMaxID = 0;
            for (int i = 0; i < cptlt.Count; i++)
            {
                if (intMaxID < cptlt[i].ID)
                {
                    intMaxID = cptlt[i].ID;
                }
            }
            return intMaxID;
        }
        #endregion

        /// <summary>
        /// 创建最小外包多边形
        /// </summary>
        /// <param name="cpl">折线</param>
        /// <param name="dblVerySmall">极小值</param> 
        /// <remarks>由于精度不一致，在判断顶点相等的时候可能会有一些麻烦(本方法中采用寻找原始对应点来代替精度不一致点的方法解决)</remarks>
        public List<CPoint> CreateConvexHullEdgeLt2(CPolyline cpl, double dblVerySmall)
        {
            List<CPoint> cptlt = new List<CPoint>(cpl.CptLt.Count);
            cptlt.AddRange(cpl.CptLt);

            ITopologicalOperator pTop = cpl.pPolyline as ITopologicalOperator;            
            pTop.Simplify();

            IGeometry pCHGeo = pTop.ConvexHull();
            IPointCollection4 pCol = pCHGeo as IPointCollection4;

            List<CPoint> CHPtLt = new List<CPoint>(pCol.PointCount);  //存储外包矩形顶点(包括首尾点)
            for (int i = 0; i < pCol.PointCount - 1; i++)//pCol中已经包含了重合的首尾点，因此不需遍历最后一个点
            {
                CPoint cpt=new CPoint (i,pCol.get_Point(i));
                for (int j = cptlt.Count-1; j >= 0; j--)
                {
                    if (cpt.Equals2D (cptlt[j],dblVerySmall))
                    {
                        CHPtLt.Add(cptlt[j]);
                        cptlt.RemoveAt(j);
                        break;
                    }
                }
            }
            CHPtLt.Add(CHPtLt[0]);

            return CHPtLt;
        }

        //#region 创建最小外包多边形（非AE代码）
        ///// <summary>
        ///// 创建最小外包多边形
        ///// </summary>
        ///// <param name="CVetexLt">顶点列</param> 
        ///// <param name="dblVerySmall">极小值</param> 
        ///// <remarks></remarks>
        //public List<CEdge> CreateConvexHullEdgeLt(List<CPoint> CVetexLt, double dblVerySmall)
        //{
        //    //先找到x坐标最小的点作为起始点
        //    double dblMinX = CVetexLt[0].X;
        //    int intBeginIndex = 0;
        //    for (int i = 0; i < CVetexLt.Count; i++)
        //    {
        //        if (CVetexLt[i].X < dblMinX)
        //        {
        //            dblMinX = CVetexLt[i].X;
        //            intBeginIndex = i;
        //        }

        //    }

        //    List<CEdge> CEdgeLt = new List<CEdge>();
        //    CreateConvexHull(CVetexLt, ref CEdgeLt, intBeginIndex, intBeginIndex, dblVerySmall);
        //    return CEdgeLt;
        //}


        //private void CreateConvexHull(List<CPoint> CVetexLt, ref  List<CEdge> CEdgeLt, int intCurrentIndex, int intBeginIndex, double dblVerySmall)
        //{

        //    //找到凸包多边形
        //    //每次生成一条边，判断其它所有点是否都在该边的右边，如果是的，则该边为一凸包边
        //    int i = 0;
        //    IPoint ipt1 = new PointClass(); IPoint ipt2 = new PointClass();
        //    double dblAlongDis1 = new double(); double dblAlongDis2 = new double();
        //    double dblFromDis1 = new double(); double dblFromDis2 = new double();
        //    bool blnRight1 = new bool(); bool blnRight2 = new bool();
        //    for (i = 0; i < CVetexLt.Count; i++)
        //    {

        //        if (intCurrentIndex == i) continue;
        //        CEdge cln = new CEdge(CVetexLt[intCurrentIndex], CVetexLt[i]);
        //        bool isExistLeft = false;
        //        for (int j = 0; j < CVetexLt.Count; j++)
        //        {
        //            if (i == j || intCurrentIndex == j) continue;
        //            cln.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, CVetexLt[j], true, ipt1, ref dblAlongDis1, ref dblFromDis1, ref blnRight1);
        //            cln.QueryPointAndDistance(esriSegmentExtension.esriExtendEmbedded, CVetexLt[j], true, ipt2, ref dblAlongDis2, ref dblFromDis2, ref blnRight2);


        //            if (blnRight1 == true) continue;  //如果点在cln的右边是没问题的                    
        //            else if (blnRight1 == false)
        //            {
        //                //点不在cln的右边，可能出现点在cln上，或点在cln的延长线上的问题
        //                //    如果点在cln的延长线上，则cln可作为凸包多边形的边
        //                //    如果点在cln上，则cln不可作为凸包多边形的边
        //                if (dblFromDis1 > 0 && dblFromDis2 < dblVerySmall) continue;  //点在cln的延长线上
        //                else //点在cln上或点在cln左边
        //                {
        //                    isExistLeft = true;
        //                    break;
        //                }
        //            }
        //        }
        //        if (isExistLeft == false)
        //        {
        //            CEdgeLt.Add(cln);
        //            break;
        //        }
        //    }

        //    if (intBeginIndex == i) return;//若果回到了起点则退出
        //    else CreateConvexHull(CVetexLt, ref CEdgeLt, i, intBeginIndex, dblVerySmall);
        //}
        //#endregion
        

        //#region 创建约束三角网（非AE代码）
        ///// <summary>
        ///// 创建约束三角网
        ///// </summary>
        ///// <param name="CTriangleLt">多边形列(非约束Delaunay三角网)</param>
        ///// <param name="CEdgeLt">约束边</param> 
        ///// <remarks></remarks>
        //public void CreateCDT(ref List<CTriangle> CTriangleLt, List<CEdge> CEdgeLt)
        //{
        //    for (int i = 0; i < CEdgeLt.Count; i++)
        //    {

        //        //生成约束线段
        //        CEdge cln = CEdgeLt[i];
        //        IRelationalOperator pRelationalOperator = (IRelationalOperator)cln;

        //        //记录与约束线段相交的三角形，若没有，则结束，并继续遍历其它约束线段
        //        List<CTriangle> CrossTriangleLt = new List<CTriangle>();
        //        for (int j = CTriangleLt.Count - 1; j >= 0; j--)
        //        {
        //            if (pRelationalOperator.Crosses(CTriangleLt[j]))
        //            {
        //                CrossTriangleLt.Add(CTriangleLt[j]);
        //                CTriangleLt.RemoveAt(j);
        //            }
        //        }
        //        if (CrossTriangleLt.Count == 0) continue;
        //        //if (CrossTriangleLt.Count == 0) return;

        //        //分别找到pln左右侧的线段
        //        List<CEdge> LeftBLLt = new List<CEdge>();
        //        List<CEdge> RightBLLt = new List<CEdge>();
        //        for (int j = 0; j < CrossTriangleLt.Count; j++)
        //        {
        //            IPoint centerpt = new PointClass();    //第一条线段的中点
        //            IPoint ipt = new PointClass();
        //            double dblAlongDis = new double();
        //            double dblFromDis = new double();
        //            bool blnRight = new bool();
        //            for (int l = 0; l < CrossTriangleLt[j].CEdgeLt.Count; l++)
        //            {
        //                if (pRelationalOperator.Crosses(CrossTriangleLt[j].CEdgeLt[l]) == false)
        //                {
        //                    //找到线段l的中点centerpt，并根据centerpt位于线段cln的左右，来判断l位于线段cln的左右
        //                    centerpt = new PointClass();
        //                    CrossTriangleLt[j].CEdgeLt[l].QueryPoint(esriSegmentExtension.esriNoExtension, 0.5, true, centerpt);
        //                    cln.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, centerpt, true, ipt, ref dblAlongDis, ref dblFromDis, ref blnRight);
        //                    if (blnRight) RightBLLt.Add(CrossTriangleLt[j].CEdgeLt[l]);
        //                    else LeftBLLt.Add(CrossTriangleLt[j].CEdgeLt[l]);
        //                }
        //            }
        //        }

        //        //将左侧的点排序(顺时针)
        //        List<CPoint> LeftPtLt = new List<CPoint>();
        //        LeftPtLt.Add(cln.FrCpt);
        //        Sortcpt(ref LeftPtLt, LeftBLLt);

        //        //将右侧的点排序(顺时针)
        //        List<CPoint> RightPtLt = new List<CPoint>();
        //        RightPtLt.Add(cln.ToCpt);
        //        Sortcpt(ref RightPtLt, RightBLLt);

        //        List<CTriangle> LeftDTLt = new List<CTriangle>();
        //        CreateDT(ref LeftDTLt, ref LeftPtLt, LeftBLLt);

        //        List<CTriangle> RightDTLt = new List<CTriangle>();
        //        CreateDT(ref RightDTLt, ref RightPtLt, RightBLLt);

        //        for (int j = 0; j < LeftDTLt.Count; j++)
        //        {
        //            CTriangleLt.Add(LeftDTLt[j]);
        //        }
        //        for (int j = 0; j < RightDTLt.Count; j++)
        //        {
        //            CTriangleLt.Add(RightDTLt[j]);
        //        }
        //    }
        //}

        ///// <summary>
        ///// 将点列cptlt中的点以指定点为起点顺时针排序
        ///// </summary>
        ///// <param name="cptlt">顶点列</param> 
        ///// <param name="cBLlt">边界数组</param> 
        ///// <returns >点列中各点的“ID”的最大值</returns>
        //private void Sortcpt(ref List<CPoint> cptlt, List<CEdge> cBLlt)
        //{
        //    List<CEdge> newBLlt = new List<CEdge>();//为了不破坏原来的边数组，新定义一个数组，并对新数组进行操作
        //    for (int i = 0; i < cBLlt.Count; i++)
        //    {
        //        newBLlt.Add(cBLlt[i]);
        //    }

        //    while (newBLlt.Count > 0)
        //    {
        //        CPoint lastcpt = cptlt[cptlt.Count - 1];
        //        for (int j = 0; j < newBLlt.Count; j++)
        //        {
        //            if (lastcpt.Equals2D(newBLlt[j].FrCpt))
        //            {
        //                cptlt.Add(newBLlt[j].ToCpt);
        //                newBLlt.RemoveAt(j);
        //                break;
        //            }
        //            else if (lastcpt.Equals2D(newBLlt[j].ToCpt))
        //            {
        //                cptlt.Add(newBLlt[j].FrCpt);
        //                newBLlt.RemoveAt(j);
        //                break;
        //            }
        //        }
        //    }
        //}


        ///// <summary>
        ///// 将点列cptlt中的点以指定点为起点顺时针排序
        ///// </summary>
        ///// <param name="DTLt">空的多边形列</param>  
        ///// <param name="cptlt">顶点列</param> 
        ///// <param name="cBLlt">边界数组</param> 
        //private void CreateDT(ref List<CTriangle> DTLt, ref List<CPoint> cptlt, List<CEdge> cBLlt)
        //{
        //    //创建三角形
        //    CreateTriangulate(ref DTLt, ref cptlt, cBLlt, 0, cptlt.Count - 1);

        //    //局部LOP优化
        //    LOPOptimize(ref DTLt);
        //}

        ///// <summary>
        ///// 递归的方法创建三角形
        ///// </summary>
        ///// <param name="DTLt">多边形列</param>  
        ///// <param name="cptlt">顶点列</param> 
        ///// <param name="cBLlt">边界数组</param> 
        ///// <param name="intI">序号，指定cptlt中的第intI个点</param> 
        ///// <param name="intJ">序号，指定cptlt中的第intJ个点</param> 
        //private void CreateTriangulate(ref List<CTriangle> DTLt, ref List<CPoint> cptlt, List<CEdge> cBLlt, int intI, int intJ)
        //{

        //    CEdge cpl = new CEdge(cptlt[intI], cptlt[intJ]);
        //    for (int i = 0; i < cBLlt.Count; i++)
        //    {
        //        if (cpl.Equals(cBLlt[i]))
        //            return;
        //    }

        //    double dblMinDis = double.MaxValue;
        //    int intMinDisIndex = new int();
        //    for (int i = intI + 1; i < intJ; i++)
        //    {
        //        IPoint ipt = new PointClass();
        //        double dblDisAlong = new double();
        //        double dblDisFrom = new double();
        //        bool blnRight = new bool();
        //        cpl.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, cptlt[i], false, ipt, ref dblDisAlong, ref dblDisFrom, ref blnRight);
        //        if (blnRight)
        //        {
        //            MessageBox.Show("点在线的右边了！请查看CCDT：780行！");
        //        }
        //        if (dblDisFrom < dblMinDis)
        //        {
        //            dblMinDis = dblDisFrom;
        //            intMinDisIndex = i;
        //        }
        //    }

        //    CTriangle Triangle = new CTriangle(cptlt[intI], cptlt[intMinDisIndex], cptlt[intJ]);
        //    DTLt.Add(Triangle);

        //    //递推调用
        //    CreateTriangulate(ref DTLt, ref  cptlt, cBLlt, intI, intMinDisIndex);
        //    CreateTriangulate(ref DTLt, ref  cptlt, cBLlt, intMinDisIndex, intJ);
        //}


        ///// <summary>
        ///// 局部LOP优化
        ///// </summary>
        ///// <param name="DTLt">新生成的多边形列</param>  
        //private void LOPOptimize(ref List<CTriangle> DTLt)
        //{
        //    int i, j, l;
        //    int m = new int();

        //    bool blnIsChange = true;
        //    while (blnIsChange == true)
        //    {
        //        blnIsChange = false;
        //        for (i = 0; i < DTLt.Count - 1; i++)
        //        {
        //            for (j = i + 1; j < DTLt.Count; j++)
        //            {
        //                //找到两个三角形的公共边
        //                bool IsFindSameEdge = false;
        //                for (l = 0; l < DTLt[i].CEdgeLt.Count; l++)
        //                {
        //                    for (m = 0; m < DTLt[j].CEdgeLt.Count; m++)
        //                    {
        //                        if (DTLt[i].CEdgeLt[l].Equals(DTLt[j].CEdgeLt[m]))
        //                        {
        //                            IsFindSameEdge = true;
        //                            break;
        //                        }
        //                    }
        //                    if (IsFindSameEdge)
        //                        break;
        //                }
        //                if (IsFindSameEdge == false)  //如果两个三角形不存在公共边，则直接遍历下一个三角形
        //                    continue;

        //                //如果其中一个三角形的外接圆包含另外一个三角形的顶点，则交换对角线
        //                if (InCircle(DTLt[i].CptLt[l], DTLt[j].CptLt[0], DTLt[j].CptLt[1], DTLt[j].CptLt[2]) ||
        //                    InCircle(DTLt[j].CptLt[m], DTLt[i].CptLt[0], DTLt[i].CptLt[1], DTLt[i].CptLt[2]))
        //                {

        //                    //为什么能直接这样添加三角形？
        //                    //以“三角形i的l点”为起点，“三角形j的m点”为终点做向量“lm”，则由于三角形i是顺时针的，
        //                    //所以“三角形i的l边的FrCpt”必然在lm的左边，“三角形i的l边的ToCpt”必然在lm的右边，因此可以lm为分界线，
        //                    //生成左右两个顺时针三角形
        //                    CTriangle LeftTriangle = new CTriangle(DTLt[i].CptLt[l], DTLt[i].CEdgeLt[l].FrCpt, DTLt[j].CptLt[m]);
        //                    CTriangle RightTriangle = new CTriangle(DTLt[j].CptLt[m], DTLt[i].CEdgeLt[l].ToCpt, DTLt[i].CptLt[l]);

        //                    DTLt.RemoveAt(j);
        //                    DTLt.RemoveAt(i);
        //                    DTLt.Add(LeftTriangle);
        //                    DTLt.Add(RightTriangle);
        //                    blnIsChange = true;
        //                }
        //            }
        //            if (blnIsChange == true)
        //                break;
        //        }
        //    }

        //}
        //#endregion
        

        /// <summary>
        /// 创建约束三角网(利用AE的TIN结构)
        /// </summary>
        /// <param name="cpl">待创建约束三角网的线段</param>
        /// <remarks>注意：本方法将改变cpl的值</remarks>
        public List<CTriangle> CreateCDT(IFeatureLayer pFeatureLayer,ref CPolyline cpl, double dblVerySmall)
        {
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            IGeoDataset pGDS = (IGeoDataset)pFeatureClass;

            IEnvelope pEnv = (IEnvelope)pGDS.Extent;
            pEnv.SpatialReference = pGDS.SpatialReference;
            double dblSuperlength = 2 * (pEnv.Width + pEnv.Height);

            IFields pFields = pFeatureClass.Fields;

            IField pHeightFiled = new FieldClass();
            //注意：此处的代码确实不太好，出于无奈才这样写的，以后找时间整理
            try
            {
                pHeightFiled = pFields.get_Field(pFields.FindField("Id"));
            }
            catch (Exception)
            {
                pHeightFiled = pFields.get_Field(pFields.FindField("ID"));
                throw;
            }


            ITinEdit pTinEdit = new TinClass();
            pTinEdit.InitNew(pEnv);
            object Missing = Type.Missing;
            pTinEdit.AddFromFeatureClass(pFeatureClass, null, pHeightFiled, null, esriTinSurfaceType.esriTinHardLine, ref Missing);

            ITinAdvanced2 pTinAdvanced2 = (ITinAdvanced2)pTinEdit;
            List<IPolygon4> PolygonLt = new List<IPolygon4>(pTinAdvanced2.TriangleCount);
            for (int i = 1; i <= pTinAdvanced2.TriangleCount; i++)  //获取所有三角形
            {
                ITinTriangle pTinTriangle = pTinAdvanced2.GetTriangle(i);
                IPointCollection4 pCol = new PolygonClass();
                for (int j = 0; j < 3; j++)
                {
                    ITinEdge pTinEdge = new TinEdgeClass();
                    pTinEdge = pTinTriangle.get_Edge(j);
                    IPoint ipt = new PointClass();
                    ipt.PutCoords(pTinEdge.FromNode.X, pTinEdge.FromNode.Y);
                    pCol.AddPoint(ipt, ref Missing, ref Missing);
                }
                IPolygon4 pPolygon = pCol as IPolygon4;
                pPolygon.Close();
                PolygonLt.Add(pPolygon);
            }

            //提取三角形，将超三角形标记为一类三角形
            List<CTriangle> CTriangleLt = new List<CTriangle>(PolygonLt.Count);
            List<CPoint> ctempptlt = new List<CPoint>(cpl.CptLt.Count);
            ctempptlt.AddRange(cpl.CptLt);
            for (int i = 0; i < PolygonLt.Count; i++)
            {
                //由于新生成的TIN中的三角网顶点与原始顶点可能不一致，在原始顶点列中找到相应顶点并构建三角网
                IPointCollection4 pCol = PolygonLt[i] as IPointCollection4;
                CPoint cpt0 = new CPoint();
                CPoint cpt1 = new CPoint();
                CPoint cpt2 = new CPoint();
                FindSamePoint(pCol.get_Point(0), cpl,ref ctempptlt, dblVerySmall, ref cpt0);
                FindSamePoint(pCol.get_Point(1), cpl, ref ctempptlt, dblVerySmall, ref cpt1);
                FindSamePoint(pCol.get_Point(2), cpl, ref ctempptlt, dblVerySmall, ref cpt2);

                CTriangle pCTriangle = new CTriangle(i, cpt0, cpt1, cpt2);
                if (PolygonLt[i].Length > dblSuperlength) pCTriangle.strTriType = "I";  //标记一类超三角形（超三角形）
                CTriangleLt.Add(pCTriangle);
            }

            //构建新的折线
            SortedDictionary<double, CPoint> newcptslt = new SortedDictionary<double, CPoint>(new CCmpDbl());
            for (int i = 0; i < ctempptlt.Count; i++)
            {
                double dblFromStartDis = CGeoFunc.CalDistanceFromStartPoint(cpl.pPolyline, (IPoint)ctempptlt[i], true);
                newcptslt.Add(dblFromStartDis, ctempptlt[i]);
            }

            List<CPoint> newcptlt = newcptslt.Values.ToList();
                //new List<CPoint>();
            //newcptlt.AddRange(newcptslt.Values);
            //newcptlt.AddRange ()
            //for (int i = 0; i < newcptslt.Count; i++)
            //{
            //    newcptlt.Add(newcptslt.Values[i]);
            //}

            //for (int i = 0; i < newcptlt.Count ; i++)
            //{
            //    newcptlt[i].ID = i;
            //}
            cpl = new CPolyline(cpl.ID, newcptlt);

            return CTriangleLt;

        }



        /// <summary>
        /// 获取共边三角形
        /// </summary>
        /// <param name="CTriangleLt">多边形列</param>
        /// <param name="CVetexLt">顶点列</param> 
        /// <remarks>SE:Share Edge 共边；每个三角形的三条边都有一个共边三角形(或为空共边三角形)</remarks>
        public void GetSETriangle(ref List<CTriangle> CTriangleLt, double dblVerySmall)
        {
            int l, m;
            m = 0;

            //初始化每个三角形，并设置其三个相邻三角形为空三角形
            for (int i = 0; i < CTriangleLt.Count; i++)
            {
                List<CTriangle> pTriangleLt = new List<CTriangle>(3);
                for (int j = 0; j < 3; j++)
                {
                    var pTriangle = new CTriangle(-2); //TID = -2: empty triangle
                    pTriangleLt.Add(pTriangle);
                }
                CTriangleLt[i].SETriangleLt = pTriangleLt;
            }

            for (int i = 0; i < CTriangleLt.Count; i++)
            {
                int intCount = 0;
                for (l = 0; l < CTriangleLt[i].CEdgeLt.Count; l++)
                {
                    if (CTriangleLt[i].SETriangleLt[l].TID != -2)
                    {
                        intCount = intCount + 1;
                        continue;  //如果该边对应的共边三角形已经存在，则不再遍历（每条边另外只有一个共边三角形）
                    }

                    for (int j = i + 1; j < CTriangleLt.Count; j++)
                    {
                        //找到两个三角形的公共边
                        bool IsFindSameEdge = false;
                        for (m = 0; m < CTriangleLt[j].CEdgeLt.Count; m++)
                        {
                            if (CTriangleLt[j].SETriangleLt[m].TID != -2) continue;
                            double disdiff = Math.Abs(CTriangleLt[i].CEdgeLt[l].dblLength - CTriangleLt[j].CEdgeLt[m].dblLength);
                            if (CTriangleLt[i].CEdgeLt[l].Equals(CTriangleLt[j].CEdgeLt[m]))
                            {
                                CTriangleLt[i].SETriangleLt[l] = CTriangleLt[j];
                                CTriangleLt[j].SETriangleLt[m] = CTriangleLt[i];
                                IsFindSameEdge = true;
                                intCount = intCount + 1;
                                break;
                            }
                        }
                        if (IsFindSameEdge)
                            break;
                    }

                    if (intCount == 3)//找到了三个共边三角形则不需要再找了
                        break;
                }
                CTriangleLt[i].SETriangleNum = intCount;
            }
        }

        ///// <summary>获取折线某边的三角形</summary>
        ///// <param name="CTriangleLt">多边形列</param>
        ///// <param name="pPolyline">折线</param> 
        ///// <param name="blnRight">是否右边的多边形</param>  
        ///// <remarks>blnRight为True时，该函数返回折线右边的三角形，blnRight为false时，该函数返回折线左边的三角形
        /////          由于直接用整条线段来判断会出现一些小问题，所以这里采用分段判断的方法</remarks>
        //public void GetSideTriangle2(ref List<CTriangle> CTriangleLt, List<CEdge> CEdgeLt, bool blnRight)
        //{

        //    //判断每个三角形的每条边是否属于边集，如果是，则依据该边，判断该三角形在边的哪一侧
        //    double dblAlongDis = new double();
        //    double dblFromDis = new double();
        //    for (int i = 0; i < CTriangleLt.Count; i++)
        //    {
        //        for (int j = 0; j < CTriangleLt[i].CEdgeLt.Count; j++)
        //        {
        //            for (int l = 0; l < CEdgeLt.Count; l++)
        //            {
        //                if (CTriangleLt[i].CEdgeLt[j].Equals(CEdgeLt[l]))
        //                {
        //                    CTriangleLt[i].CEdgeLt[j].isBelongToPolyline = true;//记录该边为折线上的边，方便到时候判断该三角形为哪类三角形
        //                    if (CTriangleLt[i].isSideJudge == false)  //如果已判断过，则无需再判断
        //                    {
        //                        IPoint ipt = new PointClass();
        //                        bool blnRightSide = new bool();
        //                        CEdgeLt[l].QueryPointAndDistance(esriSegmentExtension.esriExtendEmbedded, CTriangleLt[i].CentroidCpt, false, ipt, ref dblAlongDis, ref dblFromDis, ref blnRightSide);
        //                        //是否需要找的一侧的三角形
        //                        if (blnRightSide == blnRight) CTriangleLt[i].isNeedSide = true;
        //                        else CTriangleLt[i].isNeedSide = false;
        //                        CTriangleLt[i].isSideJudge = true;
        //                    }

        //                }
        //            }
        //        }
        //    }

        //    for (int i = 0; i < CTriangleLt.Count; i++)
        //    {
        //        if (CTriangleLt[i].isSideJudge == true) continue;
        //        for (int j = 0; j < CTriangleLt[i].SETriangleLt.Count; j++)
        //        {
        //            if (CTriangleLt[i].SETriangleLt[j].TID == -2) continue;  //如果该相邻三角形不存在，则直接遍历下一个相邻三角形
        //            if (CTriangleLt[i].SETriangleLt[j].isNeedSide == true)
        //            {
        //                CTriangleLt[i].isNeedSide2 = true;
        //                break;
        //            }
                        
        //        }
        //    }

        //    for (int i = 0; i < CTriangleLt.Count; i++)
        //    {
        //        if (CTriangleLt[i].isSideJudge == true || CTriangleLt[i].isNeedSide2 == true) continue;
        //        for (int j = 0; j < CTriangleLt[i].SETriangleLt.Count; j++)
        //        {
        //            if (CTriangleLt[i].SETriangleLt[j].TID == -2) continue;  //如果该相邻三角形不存在，则直接遍历下一个相邻三角形
        //            if (CTriangleLt[i].SETriangleLt[j].isNeedSide2 == true)
        //            {
        //                CTriangleLt[i].isNeedSide3 = true;
        //                break;
        //            } 
        //        }
        //    }

        //    //添加并返回输出三角形
        //    for (int i = 0; i < CTriangleLt.Count; i++)
        //    {
        //        if (CTriangleLt[i].isNeedSide == true || CTriangleLt[i].isNeedSide2 == true || CTriangleLt[i].isNeedSide3 == true)
        //            CTriangleLt[i].isNeedSide = true;
        //    }
        //}

        /// <summary>确定三角形在折线的哪边("Left"或"Right")</summary>
        /// <param name="CTriangleLt">多边形列</param>
        /// <param name="CEdgeLt">折线边数组</param> 
        /// <remarks>由于直接用整条线段来判断会出现一些小问题，所以这里采用分段判断的方法</remarks>
        public void ConfirmTriangleSide(ref List<CTriangle> CTriangleLt, CPolyline cpl, double dblVerySmall)
        {
            //判断每个三角形的每条边是否属于边集，如果是，则依据该边，判断该三角形在边的哪一侧 
            //判断三角形的那条边是边界边
            double dblAlongDis = new double();
            double dblFromDis = new double();
            for (int i = 0; i < CTriangleLt.Count; i++)
            {
                int intCount = 0;
                for (int j = 0; j < CTriangleLt[i].CEdgeLt.Count; j++)
                {
                    CEdge cedge=CTriangleLt[i].CEdgeLt[j];


                    IPoint outPointFr = new PointClass();
                    double distanceAlongCurveFr = 0;//该点在曲线上最近的点距曲线起点的距离
                    double distanceFromCurveFr = 0;//该点到曲线的直线距离
                    bool bRightSideFr = false;
                    MessageBox.Show("CTriangulator.cs: Row914 is needed to be improved");
                    //cpl.pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, cedge.FromPoint, false, outPointFr, ref distanceAlongCurveFr, ref distanceFromCurveFr, ref bRightSideFr);

                    IPoint outPointTo = new PointClass();
                    double distanceAlongCurveTo = 0;//该点在曲线上最近的点距曲线起点的距离
                    double distanceFromCurveTo = 0;//该点到曲线的直线距离
                    bool bRightSideTo = false;
                    MessageBox.Show("CTriangulator.cs: Row921 is needed to be improved");
                    //cpl.pPolyline.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, cedge.ToPoint, false, outPointTo, ref distanceAlongCurveTo, ref distanceFromCurveTo, ref bRightSideTo);

                    double dblDisDiff = Math.Abs(Math.Abs(distanceAlongCurveTo - distanceAlongCurveFr) - cedge.dblLength);
                    if ((dblDisDiff < dblVerySmall) && (distanceFromCurveFr < dblVerySmall) && (distanceFromCurveTo < dblVerySmall))
                    {
                        cedge.isBelongToPolyline = true;
                        if (CTriangleLt[i].isSideJudge == false)  //如果已判断过，则无需再判断
                        {
                            CEdge CRightDirectionEdge = new CEdge();
                            if (distanceAlongCurveFr < distanceAlongCurveTo)
                            {
                                CRightDirectionEdge = cedge;
                            }
                            else if (distanceAlongCurveFr > distanceAlongCurveTo)
                            {
                                CRightDirectionEdge = new CEdge(cedge.ToCpt, cedge.FrCpt);
                            }
                            else
                            {
                                MessageBox.Show("判断三角形位于多边形左右时有重合点！");
                            }

                            IPoint ipt = new PointClass();
                            bool blnRightSide = new bool();
                            MessageBox.Show("CTriangulator.cs: Row944 is needed to be improved");
                            //CRightDirectionEdge.QueryPointAndDistance(esriSegmentExtension.esriExtendEmbedded, CTriangleLt[i].CentroidCpt.pPoint, false, ipt, ref dblAlongDis, ref dblFromDis, ref blnRightSide);
                            //是否需要找的一侧的三角形
                            if (blnRightSide == true) CTriangleLt[i].strSide = "Right";
                            else CTriangleLt[i].strSide = "Left";
                            CTriangleLt[i].isSideJudge = true;
                        }

                        intCount++;
                    }

                    //if (intCount==3)
                    //{
                    //    int t = 5;
                    //}                 

                }
            }

            //确定本身没有折线边而其相邻三角形中有折线边的三角形的“左右”情况
            for (int i = 0; i < CTriangleLt.Count; i++)
            {
                if (CTriangleLt[i].isSideJudge == true)
                {
                    CTriangle pCTriangle = CTriangleLt[i];
                    RecursiveConfirmTriangleSide(ref pCTriangle);
                }
            }          
        }

        private void RecursiveConfirmTriangleSide(ref CTriangle CCurentTriangle)
        {
            for (int i = 0; i < CCurentTriangle.SETriangleLt.Count; i++)
            {
                if (CCurentTriangle.SETriangleLt[i].TID == -2) continue;  //如果该相邻三角形不存在，则直接遍历下一个相邻三角形
                if (CCurentTriangle.SETriangleLt[i].isSideJudge == false && CCurentTriangle.SETriangleLt[i].strTriType ==null)
                {
                    CCurentTriangle.SETriangleLt[i].strSide = CCurentTriangle.strSide;
                    CCurentTriangle.SETriangleLt[i].isSideJudge = true;
                    CTriangle CSETriangle = CCurentTriangle.SETriangleLt[i];
                    RecursiveConfirmTriangleSide(ref CSETriangle);
                }
            }
        }

        /// <summary>标记三角形类型“II”、“III”、“IV”</summary>
        /// <param name="CTriangleLt">多边形列</param>
        /// <remarks>“I”类三角形已在创建约束三角网时标记</remarks>
        public void SignTriTypeAll(ref List<CTriangle> CTriangleLt)
        {
            for (int i = 0; i < CTriangleLt.Count; i++)
            {
                if (CTriangleLt[i].strTriType != "I")
                {
                    int intCount = 0;
                    for (int j = 0; j < CTriangleLt[i].CEdgeLt.Count; j++)
                    {
                        if (CTriangleLt[i].CEdgeLt[j].isBelongToPolyline == true)  //在判断该三角形是否在线段某侧的时候已经标记了该边是否为折线边
                            intCount = intCount + 1;
                    }
                    switch (intCount)
                    {
                        case 0: CTriangleLt[i].strTriType = "IV"; break;    //没有折线边，则其必定有三个相邻三角形，因此其为"IV"类三角形
                        case 1: CTriangleLt[i].strTriType = "III"; break;   //有一条折线边，则其必定有两个相邻三角形，因此其为"III"类三角形
                        case 2: CTriangleLt[i].strTriType = "II"; break;    //有两条折线边，则其必定只有一个相邻三角形，因此其为"II"类三角形
                        case 3: CTriangleLt[i].strTriType = "V"; break;    //有两条折线边，则其必定只有一个相邻三角形，因此其为"II"类三角形
                        default: MessageBox.Show("未知错误？"); break;
                    }
                }
            }

        }

        ///// <summary>标记三角形类型“I”、“II”、“III”、“IV”</summary>
        ///// <param name="CTriangleLt">多边形列</param>
        ///// <remarks>为了节省时间，只有需要一侧的三角形才标识了“II”、“III”、“IV”类三角形</remarks>
        //public void SignTriTypeNeed(ref List<CTriangle> CTriangleLt)
        //{
        //    //初始化三角形类型
        //    for (int i = 0; i < CTriangleLt.Count; i++)
        //        CTriangleLt[i].strTriType = "";
            

        //    for (int i = 0; i < CTriangleLt.Count ; i++)
        //    {
        //        bool blnIsI = false;
        //        for (int j = 0; j < CTriangleLt[i].CptLt.Count ; j++)
        //        {
        //            if (CTriangleLt[i].CptLt[j].ID > _intMaxID)
        //            {
        //                blnIsI = true;
        //                CTriangleLt[i].isNeedSide = false;
        //                break;
        //            }
        //        }

        //        if (blnIsI == true) CTriangleLt[i].strTriType = "I";
        //        else if (CTriangleLt[i].isNeedSide == true)  //注意：为了节省时间，只有需要一侧的三角形才标识了“II”、“III”、“IV”类三角形
        //        {
        //            int intCount = 0;
        //            for (int j = 0; j < CTriangleLt[i].CEdgeLt.Count; j++)
        //            {
        //                if (CTriangleLt[i].CEdgeLt[j].isBelongToPolyline == true)  //在判断该三角形是否在线段某侧的时候已经标记了该边是否为折线边
        //                    intCount = intCount + 1;
        //            }
        //            switch (intCount)
        //            {
        //                case 0: CTriangleLt[i].strTriType = "IV"; break;    //没有折线边，则其必定有三个相邻三角形，因此其为"IV"类三角形
        //                case 1: CTriangleLt[i].strTriType = "III"; break;   //有一条折线边，则其必定有两个相邻三角形，因此其为"III"类三角形
        //                case 2: CTriangleLt[i].strTriType = "II"; break;    //有两条折线边，则其必定只有一个相邻三角形，因此其为"II"类三角形
        //                default: MessageBox.Show("某三角形的三条边都是折线边？");break;
        //            }
        //        }
        //    }
        //}


        /// <summary>建立弯曲的层次结构森林（因为一个曲线可能会有多个独立的最高级别的弯曲）</summary>
        /// <param name="CTriangleLt">多边形列</param>
        /// <param name="CPtlt">折线的点数组</param> 
        /// <param name="strSide">折线的某一边“Left”、“Right”</param>  
        /// <remarks>由指定的strSide决定获取某边弯曲森林
        /// </remarks>
        public CBendForest BuildBendForestNeed2(ref List<CTriangle> CTriangleLt, List<CPoint> CPtlt, string strSide, double dblVerySmall)
        {
            CBend COriginalBend = new CBend(CPtlt);  //新建原始弯曲，虽然该“弯曲”本应由多个独立弯曲组合而成，但这样建立，方便使用
            //SortedList<int, CBend> CBendForest = new SortedList<int, CBend>(new CIntCompare());  //层次结构弯曲森林

            CBendForest pBendForest = new CBendForest();

            for (int i = 0; i < CTriangleLt.Count; i++)
            {
                if (CTriangleLt[i].strSide != strSide) continue;    //如果不是所需三角形，则不需遍历
                if (CTriangleLt[i].strTriType != "I")  //如果该三角形本身不是“I”类三角形
                {
                    for (int j = 0; j < CTriangleLt[i].SETriangleLt.Count; j++)
                    {
                        if (CTriangleLt[i].CEdgeLt[j].isBelongToPolyline == false && CTriangleLt[i].SETriangleLt[j].strTriType == "I")
                        {
                            //如果该三角形本身不是“I”类三角形，而其相邻三角形中有“I”类三角形，且该公共边不是折线边，则该三角形为一弯曲的入口，建立弯曲                        
                            CBend CHiberarchyBend = COriginalBend.GetSubBend(CTriangleLt[i].CEdgeLt[j].FrCpt, CTriangleLt[i].CEdgeLt[j].ToCpt, strSide, dblVerySmall);
                            BuildHiberarchyOfBend(CTriangleLt, CHiberarchyBend, CTriangleLt[i], CTriangleLt[i].SETriangleLt[j], strSide, dblVerySmall);
                            pBendForest.Add(CHiberarchyBend.CptLt[0].ID, CHiberarchyBend);
                            break;
                        }
                    }
                }
            }
            return pBendForest;
        }

        /// <summary>递归建立弯曲结构</summary>
        /// <param name="CTriangleLt">多边形列</param>
        /// <param name="CHiberarchyBend">含层次结构的弯曲</param> 
        /// <param name="CCurrentTri">当前三角形</param>  
        /// <param name="FrontTID">上一三角形在“CTriangleLt”中的序号</param> 
        /// <remarks></remarks>
        private void BuildHiberarchyOfBend(List<CTriangle> CTriangleLt, CBend CHiberarchyBend, CTriangle CCurrentTri, CTriangle CFrontTri, string strSide, double dblVerySmall)
        {
            CHiberarchyBend.CTriangleLt.Add(CFrontTri);

            switch (CCurrentTri.strTriType)
            {
                case "II":     //如果该三角形为“II”类三角形，则结束
                    CHiberarchyBend.CLeftBend = null;                
                    CHiberarchyBend.CRightBend = null;
                    return;
                case "III":    //如果该三角形为“III”类三角形，则继续遍历下去
                    for (int i = 0; i < CCurrentTri.SETriangleLt.Count ; i++)
                    {
                        if (CCurrentTri.SETriangleLt[i].TID == -2) 
                            throw new ArgumentException("内部三角形的邻近三角形居然有不存在的情况？");  //此种情况应该不会出现
                        else if (CCurrentTri.SETriangleLt[i].TID == CFrontTri.TID || CCurrentTri.SETriangleLt[i].strTriType == "I") continue;  //如果该邻近三角形是上一个三角形，则跳过
                        else if (CCurrentTri.CEdgeLt[i].isBelongToPolyline == true) continue;  //如果该侧是折线边，跳过
                        else
                        {
                            BuildHiberarchyOfBend(CTriangleLt, CHiberarchyBend, CCurrentTri.SETriangleLt[i], CCurrentTri, strSide, dblVerySmall);
                            break;
                        } 
                    }
                    break;

                case "IV":     //"IV"类三角形的情况麻烦一点
                    //首先，找到与之前三角形对应的边，并找到FromPoint和ToPoint
                    int FrontEdgeNum=0;
                    for (int i = 0; i < CCurrentTri.SETriangleLt .Count ; i++)
                    {
                        if (CCurrentTri.SETriangleLt[i].TID == -2) 
                            MessageBox.Show("内部三角形的邻近三角形居然有不存在的情况？"); //此种情况应该不会出现
                        else if (CCurrentTri.SETriangleLt[i].TID == CFrontTri.TID)
                        {
                            FrontEdgeNum = i;
                            break;
                        }
                    }
                    CPoint CFrCpt = CCurrentTri.CEdgeLt[FrontEdgeNum].FrCpt;
                    CPoint CToCpt = CCurrentTri.CEdgeLt[FrontEdgeNum].ToCpt;                    

                    //这里做出一个规定：由于三角形的点、边及邻近三角形都是按顺时针方向存储的，
                    //因此，与“FrontEdgeNum”边有公共点CFrCpt的边对应的分支为“右孩子（RightChild）”
                    //      与“FrontEdgeNum”边有公共点CToCpt的边对应的分支为“左孩子（LeftChild）”
                    for (int i = 0; i < CCurrentTri.CEdgeLt.Count; i++)
                    {
                        if (i == FrontEdgeNum) continue;//如果是本边，则跳过

                        if (CCurrentTri.CEdgeLt[i].FrCpt.Equals2D(CToCpt)) //左孩子
                        {
                            CHiberarchyBend.CLeftBend = CHiberarchyBend.GetSubBend(CCurrentTri.CEdgeLt[i].FrCpt, CCurrentTri.CEdgeLt[i].ToCpt, strSide, dblVerySmall); //建立弯曲
                            CHiberarchyBend.CLeftBend.CParentBend = CHiberarchyBend; //与父亲弯曲相连
                            BuildHiberarchyOfBend(CTriangleLt, CHiberarchyBend.CLeftBend, CCurrentTri.SETriangleLt[i], CCurrentTri, strSide, dblVerySmall);
                        }
                        else if (CCurrentTri.CEdgeLt[i].ToCpt.Equals2D(CFrCpt)) //右孩子
                        {
                            CHiberarchyBend.CRightBend = CHiberarchyBend.GetSubBend(CCurrentTri.CEdgeLt[i].FrCpt, CCurrentTri.CEdgeLt[i].ToCpt, strSide, dblVerySmall); //建立弯曲
                            CHiberarchyBend.CRightBend.CParentBend = CHiberarchyBend; //与父亲弯曲相连
                            BuildHiberarchyOfBend(CTriangleLt, CHiberarchyBend.CRightBend, CCurrentTri.SETriangleLt[i], CCurrentTri, strSide, dblVerySmall);
                        }
                    }

                    break;
                default:
                    MessageBox.Show("递归建立弯曲出错"); 
                    break;
            }
        }

        /// <summary>找相同点</summary>
        /// <param name="ipt">目标点</param>
        /// <param name="cptlt">点数组</param> 
        /// <param name="dblVerySmall">极小值</param>  
        /// <remarks>如果目标点有相同点，则返回相同点，否则，返回当前目标点</remarks>
        private void FindSamePoint(IPoint ipt, CPolyline cpl, ref List<CPoint> ctempptlt, double dblVerySmall, ref CPoint cpt)
        {
            cpt = new CPoint(ipt);
            for (int i = 0; i < ctempptlt.Count; i++)
            {
                if (cpt.Equals2D(ctempptlt[i]))
                {
                    cpt = ctempptlt[i];
                    return;
                }
            }

            //If the point cpt doesn't exist in ctempptlt
            double dblAlongDis = 0;
            double dblFromDis = 0;
            bool blnIsRight=false ;
            IPoint outpt = new PointClass();
            cpl.pPolyline.QueryPointAndDistance(esriSegmentExtension.esriExtendEmbedded, ipt, false, outpt, ref dblAlongDis, ref dblFromDis, ref blnIsRight);
            if (dblFromDis<dblVerySmall)
            {
                var newcpt = new CPoint(outpt);
                cpt = newcpt;
                ctempptlt.Add(newcpt);
            }
        }



    }
}
