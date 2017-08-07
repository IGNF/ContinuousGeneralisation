using System;
using System.Collections.Generic;
using System.Text;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace MorphingClass.CMorphingAlgorithms
{
    /// <summary>
    /// please consider use CLIA, which returns pairs of corresponding points
    /// </summary>
    public class CLinearInterpolationA
    {
        //private CPolyline _FrCPl;
        //private CPolyline _ToCPl;
        //private CPolyline _InterFrCPl;
        //private CPolyline _InterToCPl;
        //private List<CPoint> _CResultPtLt;
        //private 
        



        /// <summary>
        /// LI方法获取对应点，此函数为LI方法的入口函数(仅对一对对应线段处理的时候从此处调用，因为该函数顾及了线段的第一个顶点)
        /// </summary>
        /// <param name="CFrPolyline">大比例尺线段，可以只有一个顶点</param>
        /// <param name="CToPolyline">小比例尺线段，可以只有一个顶点</param>
        /// <param name="ResultPtLt">结果数组</param>
        /// <remarks></remarks>
        public List<CPoint> CLI(CPolyline CFrPolyline, CPolyline CToPolyline)
        {
            List<CPoint> ResultPtLt = new List<CPoint>(CFrPolyline.CptLt.Count + CToPolyline.CptLt.Count - 1);  //please consider the special case: one segment corresponds to one point
            CPoint cfrpt0 = CFrPolyline.CptLt[0];
            cfrpt0.CorrespondingPtLt = new List<CPoint>(1);
            //CToPolyline.CptLt[0].isCtrl = true;  //标记为控制点
            cfrpt0.CorrespondingPtLt.Add(CToPolyline.CptLt[0]);
            ResultPtLt.Add(cfrpt0);
            LI(CFrPolyline, CToPolyline, ref ResultPtLt);
            return ResultPtLt;
        }

        /// <summary>
        /// 用LI方法获取对应点(对多对应线段处理的时候从此处调用，因为该函数未顾及线段的第一个顶点)
        /// </summary>
        /// <param name="CFrPolyline">大比例尺线段，可以只有一个顶点</param>
        /// <param name="CToPolyline">小比例尺线段，可以只有一个顶点</param>
        /// <param name="ResultPtLt">结果数组</param>
        /// <remarks>应注意这样一种情况：每一对对应线段的第一个点都在上一轮中处理过了</remarks>
        public void LI(CPolyline CFrPolyline, CPolyline CToPolyline, ref List<CPoint> ResultPtLt)
        {
            List<CPoint> frptlt = CFrPolyline.CptLt;
            List<CPoint> toptlt = CToPolyline.CptLt;

            for (int i = 1; i < frptlt.Count; i++)
                frptlt[i].CorrespondingPtLt = new List<CPoint>(1);
            //for (int i = 1; i < toptlt.Count; i++)
            //    toptlt[i].isCtrl = false;

            //由于前面的处理，frptlt和toptlt的顶点数不可能同时为1
            //此过程中已经包含了最后一个点的匹配“i < **ptlt.Count”已经指示到最后一个点了
            if (frptlt.Count == 1)
            {
                frptlt[0].CorrespondingPtLt = new List<CPoint>(toptlt.Count);
                for (int i = 0; i < toptlt.Count; i++)
                {
                    //toptlt[i].isCtrl = true;
                    frptlt[0].CorrespondingPtLt.Add(toptlt[i]);
                }
                //frptlt[0] has already been added in ResultPtLt
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
            else
            {
                CHelpFunc.SetAbsAndRatioLength(ref CFrPolyline, CEnumScale.Larger);
                CHelpFunc.SetAbsAndRatioLength(ref CToPolyline, CEnumScale.Smaller);

                int intFrCount = 1;
                int intToCount = 1;
                int intFrRatioNum = frptlt.Count - 1;  //we don't need add the first one and the last one
                int intToRatioNum = toptlt.Count - 1;  //we don't need add the first one and the last one
                List<KeyValuePair<double, CPoint>> dblRatioKVPLt = new List<KeyValuePair<double, CPoint>>(intFrRatioNum + intToRatioNum - 2);
                List< CPoint> CombinedCPtLt =  new List<CPoint> (intFrRatioNum + intToRatioNum - 2);
                while (intFrCount < intFrRatioNum || intToCount < intToRatioNum)
                {
                    if (frptlt[intFrCount].dblRatioLengthFromStart < toptlt[intToCount].dblRatioLengthFromStart)
                    {
                        CombinedCPtLt.Add(frptlt[intFrCount]);
                        intFrCount++;
                    }
                    else if (frptlt[intFrCount].dblRatioLengthFromStart > toptlt[intToCount].dblRatioLengthFromStart)
                    {
                        CombinedCPtLt.Add(toptlt[intToCount]);
                        intToCount++;
                    }
                    else
                    {
                        CombinedCPtLt.Add(frptlt[intFrCount]);
                        intFrCount++;
                        intToCount++;
                    }
                }


                int intFrKnownIndex = 0;
                int intToKnownIndex = 0;

                foreach (CPoint comcpt in CombinedCPtLt)
                {
                    if (comcpt.BelongedCpl.enumScale == CEnumScale.Larger)
                    {
                        //CPoint 
                        CPoint cpt = CGeoFunc.QueryCPointByLength(comcpt, toptlt, ref intToKnownIndex);
                        comcpt.CorrespondingPtLt.Add(cpt);
                        ResultPtLt.Add(comcpt);
                    }
                    else if (comcpt.BelongedCpl.enumScale == CEnumScale.Smaller)
                    {
                        CPoint cpt = CGeoFunc.QueryCPointByLength(comcpt, frptlt, ref intFrKnownIndex);
                        cpt.CorrespondingPtLt = new List<CPoint>(1);
                        cpt.CorrespondingPtLt.Add(comcpt);
                        ResultPtLt.Add(cpt);
                    }
                }

                //添加最后一个点（两线的最后一个点相对应）
                //toptlt[toptlt.Count - 1].isCtrl = true;  //标记为控制点
                frptlt[frptlt.Count - 1].CorrespondingPtLt.Add(toptlt[toptlt.Count - 1]);
                ResultPtLt.Add(frptlt[frptlt.Count - 1]);
            }


        }


        //public List<CPolyline> GenerateInterCPlLt(List<List<CPoint>> pResultPtLtLt)
        //{
        //    List<CPolyline> pcpllt = new List<CPolyline>(pResultPtLtLt.Count);
        //    for (int i = 0; i < pResultPtLtLt.Count ; i++)
        //    {
        //        pcpllt.Add(GenerateInterCPl(i, pResultPtLtLt[i]));
        //    }

        //    return pcpllt;
        //}

        //public CPolyline GenerateInterCPl(int intID, List<CPoint> pResultPtLt)
        //{
        //    List<CPoint> pcptlt = new List<CPoint>(pResultPtLt.Count);
        //    foreach (CPoint  pctp in pResultPtLt)
        //    {
        //        pcptlt.Add(pctp);
        //    }

        //    CPolyline cpl = new CPolyline(intID, pcptlt);
        //    return cpl;
        //}

        //public void GenerateInterMoveVector(List<List<CPoint>> pResultPtLtLt)
        //{
        //    foreach (List <CPoint > pResultPtLt in pResultPtLtLt)
        //    {
        //        GenerateInterMoveVector(pResultPtLt);
        //    }
        //}

        //public void  GenerateInterMoveVector(List<CPoint> pResultPtLt)
        //{
        //    foreach (CPoint pcpt in pResultPtLt)
        //    {
        //        pcpt.MoveVectorPtLt = new List<CPoint>(pcpt.CorrespondingPtLt .Count );
        //        foreach (CPoint  CorrCpt in pcpt .CorrespondingPtLt )
        //        {
        //            CPoint MoveVector = CGeoFunc.CalMoveVector(pcpt, CorrCpt); ;
        //            pcpt.MoveVectorPtLt.Add(MoveVector);
        //        }
        //    }
        //}\\\\\\\
    }
}
