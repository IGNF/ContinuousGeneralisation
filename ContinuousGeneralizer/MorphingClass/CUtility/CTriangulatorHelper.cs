using System;
using System.Linq;
using System.Collections.Generic;
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
    //public class CTriangulatorHelper
    //{
    //    /// <summary>
    //    /// 统计弯曲信息，包括每个弯曲的平均深度和最大深度
    //    /// </summary>
    //    /// <param name="CTriangleLt">多边形列</param>
    //    /// <param name="CHiberarchyBend">含层次结构的弯曲</param> 
    //    /// <remarks>建立弯曲的层次结构森林过程中同时还要完成两个任务：
    //    /// 1、统计整个曲线的弯曲平均深度（每个弯曲底部到弯曲入口所经历的三角形的数量的平均值）
    //    /// 2、统计每个弯曲（包括每个子弯曲）的深度（即从该弯曲开始到弯曲底部所经历的三角形的数量）
    //    /// </remarks>
    //    public double StaticBendInfo(ref CBendForest FromBendForest, ref CBendForest ToBendForest)
    //    {
    //        //FromTriangleLt[i].a
    //        //计算每个弯曲的平均深度和最大深度
    //        FromBendForest.dblDeepBendDepthLt = new List<double>();
    //        for (int i = 0; i < FromBendForest.Count; i++)
    //            CalBendDepth(FromBendForest.ElementAt(i).Value);

    //        //计算每个弯曲的平均深度和最大深度
    //        ToBendForest.dblDeepBendDepthLt = new List<double>();
    //        for (int i = 0; i < ToBendForest.Count; i++)
    //            CalBendDepth(ToBendForest.ElementAt(i).Value);
            
    //        double dblDepthSumRatio = CalBendForestDepthSumRatio(FromBendForest, ToBendForest);
    //        return dblDepthSumRatio;
    //    }
















    //    /// <summary>
    //    /// 计算每个弯曲的深度
    //    /// </summary>
    //    /// <param name="Bend">当前遍历的弯曲</param>
    //    /// <remarks>从叶子结点开始计算（即从最底层的弯曲开始遍历）；为实验需要，同时计算最大深度和平均深度</remarks>
    //    private void CalBendDepth(CBend Bend)
    //    {
    //        double dblCurrentDepth = 0;
    //        for (int i = 0; i < Bend.CTriangleLt.Count; i++)
    //        {
    //            dblCurrentDepth = dblCurrentDepth + Bend.CTriangleLt[i].Area;
    //        }

    //        if (Bend.CLeftBend != null)
    //        {
    //            CalBendDepth(Bend.CLeftBend);
    //            CalBendDepth(Bend.CRightBend);
    //        }
    //        else
    //        {                
    //            Bend.intPathCount = 1;
    //            Bend.dblBendDepthAverage = dblCurrentDepth;
    //            Bend.dblBendDepthMax = dblCurrentDepth;
    //            Bend.dblBendDepthSum = dblCurrentDepth;
    //            return;
    //        }

    //        //计算该弯曲最大深度
    //        if (Bend.CLeftBend.dblBendDepthMax >= Bend.CRightBend.dblBendDepthMax)
    //            Bend.dblBendDepthMax = Bend.CLeftBend.dblBendDepthMax + dblCurrentDepth;
    //        else
    //            Bend.dblBendDepthMax = Bend.CRightBend.dblBendDepthMax + dblCurrentDepth;

    //        //计算该弯曲平均深度
    //        int intPathCountSum = Bend.CLeftBend.intPathCount + Bend.CRightBend.intPathCount;
    //        Bend.intPathCount = intPathCountSum;
    //        Bend.dblBendDepthAverage = dblCurrentDepth + (Bend.CLeftBend.dblBendDepthAverage * Bend.CLeftBend.intPathCount +
    //                                                      Bend.CRightBend.dblBendDepthAverage * Bend.CRightBend.intPathCount) / intPathCountSum;

    //        //计算弯曲的总深度
    //        Bend.dblBendDepthSum =dblCurrentDepth + Bend.CLeftBend.dblBendDepthSum + Bend.CRightBend.dblBendDepthSum;
    //    }



    //    /// <summary>
    //    /// 计算曲线的深度
    //    /// </summary>
    //    /// <param name="pBendForest">根据某曲线生成的弯曲森林</param>
    //    /// <remarks></remarks>
    //    private void CalBendForestDepth(ref CBendForest pBendForest)
    //    {
    //        int intPathCount = 0;
    //        double dblBendDepthSum = 0;
    //        double dblBendDepthMax = 0;
    //        for (int i = 0; i < pBendForest.Count; i++)
    //        {
    //            intPathCount = intPathCount + pBendForest.ElementAt(i).Value.intPathCount;
    //            dblBendDepthSum = dblBendDepthSum + pBendForest.ElementAt(i).Value.dblBendDepthAverage * pBendForest.ElementAt(i).Value.intPathCount;
    //            if (dblBendDepthMax < pBendForest.ElementAt(i).Value.dblBendDepthMax)
    //                dblBendDepthMax = pBendForest.ElementAt(i).Value.dblBendDepthMax;

    //        }
    //        pBendForest.dblDepthAverage = dblBendDepthSum / intPathCount;
    //        pBendForest.dblDepthMax = dblBendDepthMax;
    //        pBendForest.intPathCount = intPathCount;
    //    }


    //    /// <summary>
    //    /// 计算两个弯曲森林的协方标准差
    //    /// </summary>
    //    /// <param name="pBendForest">根据某曲线生成的弯曲森林</param>
    //    /// <remarks></remarks>
    //    private double  CalBendForestCovariance(List<double> dblFromDeepBendDepthLt, List<double> dblToDeepBendDepthLt)
    //    {
    //        //数据准备
    //        double dblStep = Convert.ToDouble(dblFromDeepBendDepthLt.Count) / Convert.ToDouble(dblToDeepBendDepthLt.Count);
    //        double dblHalfStep = dblStep / 2;

    //        List<double> dblExtractiveDepthLt = new List<double>();
    //        for (int i = 0; i < dblToDeepBendDepthLt.Count ; i++)
    //        {
    //            int intIndex = Convert.ToInt32(i * dblStep + dblHalfStep);
    //            dblExtractiveDepthLt.Add(dblFromDeepBendDepthLt[intIndex]);
    //        }


    //        double dblFromSum = 0;
    //        for (int i = 0; i < dblExtractiveDepthLt.Count; i++)
    //            dblFromSum = dblFromSum + dblExtractiveDepthLt[i];
    //        double dblFromAverage = dblFromSum / dblExtractiveDepthLt.Count;

    //        double dblToSum = 0;
    //        for (int i = 0; i < dblToDeepBendDepthLt.Count; i++)
    //            dblToSum = dblToSum + dblToDeepBendDepthLt[i];
    //        double dblToAverage = dblToSum / dblExtractiveDepthLt.Count;

    //        double dblSum = 0;
    //        for (int i = 0; i < dblExtractiveDepthLt.Count; i++)
    //            dblSum = dblSum + (dblExtractiveDepthLt[i] - dblFromAverage) * (dblToDeepBendDepthLt[i] - dblToAverage);
    //        double dblAverage = dblSum / dblExtractiveDepthLt.Count;
    //        double dblCovariance = Math.Sqrt(dblAverage);

    //        return dblCovariance;










    //    }

    //    /// <summary>
    //    /// 计算两个弯曲森林弯曲深度差的标准差
    //    /// </summary>
    //    /// <param name="pBendForest">根据某曲线生成的弯曲森林</param>
    //    /// <remarks>SD : Standard Deviation </remarks>
    //    private double CalBendForestDepthDiffSD(List<double> dblFromDeepBendDepthLt, List<double> dblToDeepBendDepthLt)
    //    {
    //        int intCount = dblToDeepBendDepthLt.Count;

    //        //由于From里面多，先从From里面提取与To等量的数据
    //        double dblStep = Convert.ToDouble(dblFromDeepBendDepthLt.Count) / Convert.ToDouble(dblToDeepBendDepthLt.Count);
    //        double dblHalfStep = dblStep / 2;

    //        List<double> dblExtractiveDepthLt = new List<double>();
    //        for (int i = 0; i < intCount; i++)
    //        {
    //            int intIndex = Convert.ToInt32(i * dblStep + dblHalfStep);
    //            dblExtractiveDepthLt.Add(dblFromDeepBendDepthLt[intIndex]);
    //        }


    //        List<double> dblDepthDiffLt = new List<double>();
    //        double dblDepthDiffSum = 0;
    //        for (int i = 0; i < intCount; i++)
    //        {
    //            double dblDepthDiff = dblExtractiveDepthLt[i] - dblToDeepBendDepthLt[i];
    //            dblDepthDiffLt.Add(dblDepthDiff);
    //            dblDepthDiffSum = dblDepthDiffSum + dblDepthDiff;
    //        }
    //        double dblDepthDiffAverage = dblDepthDiffSum / intCount;

    //        double dblSum = 0;
    //        for (int i = 0; i < intCount; i++)
    //            dblSum = dblSum + (dblDepthDiffLt[i] - dblDepthDiffAverage) * (dblDepthDiffLt[i] - dblDepthDiffAverage);
    //        double dblAverage = dblSum / (dblExtractiveDepthLt.Count-1);
    //        double dblDepthDiffSD = Math.Sqrt(dblAverage);


    //        return dblDepthDiffSD;

    //    }


    //    private double CalBendForestDepthSumRatio(CBendForest FromBendForest, CBendForest ToBendForest)
    //    {
    //        double dblFromSum = 0;
    //        for (int i = 0; i < FromBendForest.Count ; i++)
    //            dblFromSum = dblFromSum + FromBendForest.ElementAt(i).Value.dblBendDepthSum;

    //        double dblToSum = 0;
    //        for (int i = 0; i < ToBendForest.Count; i++)
    //            dblToSum = dblToSum + ToBendForest.ElementAt(i).Value.dblBendDepthSum;           

    //        double dblRatio = dblFromSum / dblToSum;
    //        return dblRatio;
    //    }












    //}
}
