using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;
namespace MorphingClass.CMorphingAlgorithms
{
    public class CVertexInsertionA
    {
        



        /// <summary>
        /// VI方法获取对应点，此函数为LI方法的入口函数(仅对一对对应线段处理的时候从此处调用)
        /// </summary>
        /// <param name="CFrPolyline">大比例尺线段</param>
        /// <param name="CToPolyline">小比例尺线段</param>
        /// <param name="ResultPtLt">结果数组</param>
        /// <remarks>应注意这样一种情况：每一对对应线段的第一个点都在上一轮中处理过了</remarks>
        public List<CPoint> CVI(CPolyline CFrPolyline, CPolyline CToPolyline)
        {
            List<CPoint> ResultPtLt = new List<CPoint>();
            CPoint cfrpt0 = CFrPolyline.CptLt[0];
            cfrpt0.CorrespondingPtLt = new List<CPoint>();
            cfrpt0.CorrespondingPtLt.Add(CToPolyline.CptLt[0]);
            ResultPtLt.Add(cfrpt0);
            VI(CFrPolyline, CToPolyline, ref ResultPtLt);
            return ResultPtLt;
        }

        /// <summary>
        /// 用LI方法获取对应点(对多对应线段处理的时候从此处调用)
        /// </summary>
        /// <param name="CFrPolyline">大比例尺线段</param>
        /// <param name="CToPolyline">小比例尺线段</param>
        /// <param name="ResultPtLt">结果数组</param>
        /// <remarks>应注意这样一种情况：每一对对应线段的第一个点都在上一轮中处理过了</remarks>
        public void VI(CPolyline CFrPolyline, CPolyline CToPolyline, ref List<CPoint> ResultPtLt)
        {
            List<CPoint> frptlt = CFrPolyline.CptLt;
            List<CPoint> toptlt = CToPolyline.CptLt;
            for (int i = 1; i < frptlt.Count; i++)
                frptlt[i].CorrespondingPtLt = new List<CPoint>();

            //其中一个线段只有一个点的情形
            if (frptlt.Count == 1)
            {
                for (int i = 1; i < toptlt.Count; i++)
                    frptlt[0].CorrespondingPtLt.Add(toptlt[i]);
                ResultPtLt.Add(frptlt[0]);
                return;
            }
            else if (toptlt.Count == 1)
            {
                for (int i = 1; i < frptlt.Count; i++)
                {
                    frptlt[i].CorrespondingPtLt.Add(toptlt[0]);
                    ResultPtLt.Add(frptlt[i]);
                }
                return;
            }

            //为不影响原来的数据，定义新的点列
            List<CPoint> nfrptlt = new List<CPoint>();
            for (int i = 0; i < frptlt.Count ; i++)
                nfrptlt.Add(frptlt[i]);
            List<CPoint> ntoptlt = new List<CPoint>();
            for (int i = 0; i < toptlt.Count; i++)
                ntoptlt.Add(toptlt[i]);

            //给顶点较少的点列插入顶点，使顶点数相等
            if (nfrptlt.Count > ntoptlt.Count)
            {
                int inttInsertNum = nfrptlt.Count - ntoptlt.Count;
                InsertVertex(ntoptlt, inttInsertNum);
            }
            else if (nfrptlt.Count < ntoptlt.Count)
            {
                int intfInsertNum = ntoptlt.Count - nfrptlt.Count;
                InsertVertex(nfrptlt, intfInsertNum);
            }

            //一一对应
            for (int i = 1; i < nfrptlt.Count; i++)
            {
                nfrptlt[i].CorrespondingPtLt.Add(ntoptlt[i]);
                ResultPtLt.Add(nfrptlt[i]);
            }           
           
        }



        /// <summary>
        /// 插入一定数量的点
        /// </summary>
        /// <param name="cptlt">点列</param>
        /// <param name="intInsertNum">需插入点的数量</param>
        private void InsertVertex(List<CPoint> cptlt, int intInsertNum)
        {
            //将每个点及与其之前点的距离存入按距离从小到大存入数组
            SortedList<double, CPoint> dblDisLt = new SortedList<double, CPoint>(new CDblCompare());
            for (int i = 1; i < cptlt.Count; i++)
            {
                double dblDis = CGeometricMethods.CalDis(cptlt[i - 1], cptlt[i]);
                dblDisLt.Add(dblDis, cptlt[i]);
            }

            //插入intInsertNum个点（每次在最长边的中点插入点）
            for (int i = 0; i < intInsertNum; i++)
            {

                CPoint cpt0 = dblDisLt.Values[dblDisLt.Values.Count -1];
                int intIndex = cptlt.IndexOf(cpt0);
                //创建要插入的点
                double dblnewX = (cptlt[intIndex - 1].X + cptlt[intIndex].X) / 2;
                double dblnewY = (cptlt[intIndex - 1].Y + cptlt[intIndex].Y) / 2;
                CPoint newcpt = new CPoint(0, dblnewX, dblnewY);
                newcpt.CorrespondingPtLt = new List<CPoint>();
                cptlt.Insert(intIndex, newcpt);//插入点

                //删除原来的点及距离记录，插入两个新记录
                double dblnewDis = dblDisLt.Keys[dblDisLt.Values.Count - 1] / 2;
                dblDisLt.RemoveAt(dblDisLt.Values.Count - 1);
                dblDisLt.Add(dblnewDis, newcpt);
                dblDisLt.Add(dblnewDis, cpt0);
            }
        }


 

    }
}
