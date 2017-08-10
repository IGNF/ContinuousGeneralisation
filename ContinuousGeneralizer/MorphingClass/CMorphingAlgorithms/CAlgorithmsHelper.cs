using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CMorphingAlgorithms
{
    public class CAlgorithmsHelper
    {




        /// <summary>
        /// 按指定方式对对应线段进行点匹配，提取对应点
        /// </summary>
        /// <param name="CorrespondSegmentLk">对应线段列</param>
        /// <param name="strCorrType">匹配方式</param>
        public List<CPoint> BuildPointCorrespondence(LinkedList<CCorrespondSegment> CorrespondSegmentLk, string strCorrType)
        {            
            List<CPoint> ResultPtLt = new List<CPoint>();
            switch (strCorrType)
            {
                case "Linear":ResultPtLt= ByLinear(CorrespondSegmentLk); break;

                default:ResultPtLt= ByLinear(CorrespondSegmentLk); break;
            }

            return ResultPtLt;
        }






        /// <summary>
        /// 通过线要素获取点数组
        /// </summary>
        /// <param name="cpl">线要素</param>
        /// <returns>点数组</returns>
        private List<CPoint> ByLinear(LinkedList<CCorrespondSegment> CorrespondSegmentLk)
        {
            int intCount = 1;
            foreach (CCorrespondSegment pCorrespondSegment in CorrespondSegmentLk)
            {
                intCount += (pCorrespondSegment.CFrPolyline.CptLt.Count + pCorrespondSegment.CToPolyline.CptLt.Count - 3);
            }
            List<CPoint> CResultPtLt = new List<CPoint>(intCount);
            ////添加第一个点
            CPoint cpt0 =CorrespondSegmentLk.First.Value.CFrPolyline.CptLt[0];
            cpt0.CorrespondingPtLt = new List<CPoint>();
            //CorrespondSegmentLk[0].CToPolyline.CptLt[0].isCtrl = true;  //标记为控制点
            cpt0.CorrespondingPtLt.Add(CorrespondSegmentLk.First.Value.CToPolyline.CptLt[0]);
            CResultPtLt.Add(cpt0);

            CLinearInterpolationA pLinearInterpolationA = new CLinearInterpolationA();
            foreach (CCorrespondSegment pCorrespondSegment in CorrespondSegmentLk)
            {
                pLinearInterpolationA.LI(pCorrespondSegment.CFrPolyline, pCorrespondSegment.CToPolyline, ref CResultPtLt);
            }

            //如果一个点的对应点列中有两个（或多个）相同的元素，删除后面的重复元素，仅保留第一个元素
            for (int i = 0; i < CResultPtLt.Count; i++)
            {
                for (int j = CResultPtLt[i].CorrespondingPtLt.Count - 1; j >= 0; j--)
                {
                    for (int k = j - 1; k >= 0; k--)
                    {
                        if (CResultPtLt[i].CorrespondingPtLt[j].Equals2D (CResultPtLt[i].CorrespondingPtLt[k]))
                        {
                            CResultPtLt[i].CorrespondingPtLt.RemoveAt(j);
                            break;
                        }
                    }
                }
            }


            return CResultPtLt;
        }






    }
}
