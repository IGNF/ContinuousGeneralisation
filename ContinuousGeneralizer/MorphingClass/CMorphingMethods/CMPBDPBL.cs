using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CGeneralizationMethods;

namespace MorphingClass.CMorphingMethods
{
    public class CMPBDPBL
    {
        //private List<CPolyline> _InterLSCPlLt = new List<CPolyline>();  //BS:LargerScale
        //private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        //private CDPSimplification _pDPSimplification = new CDPSimplification();
        //private CParameterResult _ParameterResult;

        //private CParameterInitialize _ParameterInitialize;

        //public CMPBDPBL()
        //{

        //}

        //public CMPBDPBL(CParameterInitialize ParameterInitialize)
        //{

        //    //获取当前选择的点要素图层
        //    //大比例尺要素图层
        //    IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLargerScaleLayer.SelectedIndex);
                                                                       
        //    //小比例尺要素图层
        //    IFeatureLayer pSSFLayer =(IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboSmallerScaleLayer.SelectedIndex);

        //    ParameterInitialize.dblAngleBound = Convert.ToDouble(ParameterInitialize.txtAngleBound.Text)*Math.PI /180;
        //    ParameterInitialize.pBSFLayer = pBSFLayer;
        //    ParameterInitialize.pSSFLayer = pSSFLayer;
        //    _ParameterInitialize = ParameterInitialize;

        //    //获取线数组
        //    _InterLSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pBSFLayer);
        //    _SSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSSFLayer);
        //}


        //public void MPBDPBLMorphing()
        //{
        //    CParameterInitialize ParameterInitialize = _ParameterInitialize;
        //    CGeoFunc.SetCPInterLScaleEdgeLengthPtBelong(ref _InterLSCPlLt, CEnumScale.Larger);
        //    CGeoFunc.SetCPInterLScaleEdgeLengthPtBelong(ref _SSCPlLt, CEnumScale.Smaller);
        //    CPolyline frcpl = _InterLSCPlLt[0];
        //    CPolyline tocpl = _SSCPlLt[0];

        //    //计算标准向量
        //    double dblX = tocpl.CptLt[0].X - frcpl.CptLt[0].X;
        //    double dblY = tocpl.CptLt[0].Y - frcpl.CptLt[0].Y;
        //    CPoint StandardVectorCpt = new CPoint(0, dblX, dblY);

        //    CGeoFunc.CalDistanceParameters<CPolyline, CPolyline>(frcpl);
            

        //    CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
        //    //double dbInterLSumLength = frcpl.pPolyline.Length + tocpl.pPolyline.Length;
        //    CTranslation pTranslation = new CTranslation();
        //    CDeflection pDeflection = new CDeflection();
        //    CParameterThreshold ParameterThreshold = new CParameterThreshold();

        //    long lngStartTime = System.Environment.TickCount;  //开始时间

        //    //have to upgrade the codes
        //    //frcpl.SetVirtualPolyline();
        //    //tocpl.SetVirtualPolyline();
        //    //_pDPSimplification.DivideCplByDP(frcpl, frcpl.pVirtualPolyline);
        //    //_pDPSimplification.DivideCplByDP(tocpl, tocpl.pVirtualPolyline);

        //    int intIndex = 0;
        //    double dblMin = double.MaxValue;
        //    List<double> dblTranslationLt = new List<double>();
        //    ParameterThreshold.dblAngleBound = ParameterInitialize.dblAngleBound;
        //    for (int i = 0; i < 25; i++)
        //    {
        //        ParameterThreshold.dblDLengthBound = 1 * (1 - 0.02 * i);
        //        ParameterThreshold.dblULengthBound = 1 / (1 - 0.02 * i);

        //        //进行弯曲匹配，提取对应线段
        //        C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //        SubPolylineMatchLA(frcpl,frcpl.pVirtualPolyline , tocpl,tocpl.pVirtualPolyline , ParameterThreshold, ref CorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

        //        //按指定方式对对应线段进行点匹配，提取对应点                
        //        //List<CPoint> ResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(CorrespondSegmentLk, "Linear");
        //        //double dblDeflection = pDeflection.CalDeflection(ResultPtLt,StandardVectorCpt ,dbInterLSmallDis ,dblVerySmall);
        //        //double dblTranslation = pTranslation.CalTranslation(ResultPtLt);
        //        //dblTranslationLt.Add(dblTranslation);
        //        //if (dblDeflection < dblMin)
        //        //{
        //        //    intIndex = i;
        //        //    dblMin = dblDeflection;
        //        //}
        //    } 

        //    ParameterThreshold.dblDLengthBound = 1 * (1 - 0.02 * intIndex);
        //    ParameterThreshold.dblULengthBound = 1 / (1 - 0.02 * intIndex);
             
        //    //进行弯曲匹配，提取对应线段
        //    C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //    SubPolylineMatchLA(frcpl, frcpl.pVirtualPolyline, tocpl, tocpl.pVirtualPolyline, ParameterThreshold, ref pCorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

        //    //按指定方式对对应线段进行点匹配，提取对应点
        //    //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");

        //    //计算并显示运行时间
        //    long lngEndTime = System.Environment.TickCount;
        //    long lngTime = lngEndTime - lngStartTime;
        //    ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

        //    //保存指标值及对应线
        //    CHelpFuncExcel.ExportDataltToExcel(dblTranslationLt, "translationlt0", ParameterInitialize.strSavePath);
        //    //CHelpFunc.SaveCtrlLine(pCorrespondSegmentLk, "MPBDPBLControlLine", dblVerySmall, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        //    //CHelpFunc.SaveCorrespondLine(pResultPtLt, "MPBDPBLCorrLine", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

        //    //获取结果，全部记录在_ParameterResult中
        //    CParameterResult ParameterResult = new CParameterResult();
        //    ParameterResult.FromCpl = frcpl;
        //    ParameterResult.ToCpl = tocpl;
        //    ParameterResult.lngTime = lngTime;
        //    //ParameterResult.CResultPtLt = pResultPtLt;
        //    _ParameterResult = ParameterResult;
        //}


        ///// <summary>用基于BLG树的方法处理Morphing问题</summary>
        ///// <param name="frcpl">大比例尺表达线段</param>
        ///// <param name="tocpl">大比例尺表达线段</param>
        ///// <param name="pParameterThreshold">阈值参数</param>
        ///// <returns>若两河流为同一河流，返回true，否则返回faInterLSe</returns>
        ///// <remarks>DWByDP:DealWithByDP
        ///// 本方法主要用于外部对DP算法的调用</remarks>
        //public List<CPoint> DWByDP(CPolyline frcpl, CPolyline tocpl, string strMethod)
        //{
        //    //if (frcpl.pPolyline.Length ==0||tocpl.pPolyline.Length ==0)
        //    //{
        //    //    List<CPoint> cptlt00 = new List<CPoint>();
        //    //    return cptlt00;
        //    //}


        //    //DivideCplByDP(frcpl);
        //    //DivideCplByDP(tocpl);


        //    //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
        //    ////double dbInterLSumLength = frcpl.pPolyline.Length + tocpl.pPolyline.Length;
        //    //CTranslation pTranslation = new CTranslation();
        //    //CParameterThreshold ParameterThreshold = new CParameterThreshold();
        //    ////ParameterThreshold.dblLengthBound = dblLengthBound;

        //    //int intIndex = 0;
        //    //double dblMin = double.MaxValue;
        //    //List<double> dblTranslationLt = new List<double>();
        //    //for (int i = 0; i < 5; i++)
        //    //{
        //    //    ParameterThreshold.dblDLengthBound = 1 * (1 - 0.02 * i);
        //    //    ParameterThreshold.dblULengthBound = 1 / (1 - 0.02 * i);

        //    //    //进行弯曲匹配，提取对应线段
        //    //    C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //    //    SubPolylineMatchLA(frcpl, tocpl, ParameterThreshold, ref CorrespondSegmentLk);

        //    //    //按指定方式对对应线段进行点匹配，提取对应点                
        //    //    List<CPoint> ResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(CorrespondSegmentLk, strMethod);
        //    //    double dblTranslation = pTranslation.CalTranslation(ResultPtLt);
        //    //    dblTranslationLt.Add(dblTranslation);
        //    //    if (dblTranslation < dblMin)
        //    //    {
        //    //        intIndex = i;
        //    //        dblMin = dblTranslation;
        //    //    }
        //    //}

        //    ////CHelpFuncExcel.ExportDataltToExcel(dblTranslationLt, "translationlt0", _ParameterInitialize.strSavePath);


        //    //ParameterThreshold.dblDLengthBound = 1 * (1 - 0.02 * intIndex);
        //    //ParameterThreshold.dblULengthBound = 1 / (1 - 0.02 * intIndex);


        //    ////进行弯曲匹配，提取对应线段
        //    //C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //    //SubPolylineMatchLA(frcpl, tocpl, ParameterThreshold, ref pCorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

        //    ////按指定方式对对应线段进行点匹配，提取对应点
        //    //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");

        //    //return pResultPtLt;

        //    return null;
        //}

        ///// <summary>用基于BLG树的方法处理Morphing问题，指定边长比参数</summary>
        ///// <param name="frcpl">大比例尺表达线段</param>
        ///// <param name="tocpl">大比例尺表达线段</param>
        ///// <param name="pParameterThreshold">阈值参数</param>
        ///// <returns>若两河流为同一河流，返回true，否则返回faInterLSe</returns>
        ///// <remarks>DWByDP:DealWithByDP based on Lengths of the base-lines
        ///// 本方法主要用于外部对DP算法的调用</remarks>
        //public C5.LinkedList<CCorrespondSegment> DWByDPLDefine(CPolyline frcpl, CPolyline tocpl, CParameterThreshold ParameterThreshold)
        //{
        //    //if (frcpl.pPolyline.Length == 0 || tocpl.pPolyline.Length == 0)
        //    //{
        //    //    C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk00 = new C5.LinkedList<CCorrespondSegment>();
        //    //    return CorrespondSegmentLk00;
        //    //}

        //    //DivideCplByDP(frcpl);
        //    //DivideCplByDP(tocpl);

        //    ////进行弯曲匹配，提取对应线段
        //    //C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //    //SubPolylineMatchL(frcpl, tocpl, ParameterThreshold, ref pCorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

        //    //return pCorrespondSegmentLk;
        //    return null;
        //}

        ///// <summary>用基于BLG树的方法处理Morphing问题，指定边长比参数</summary>
        ///// <param name="frcpl">大比例尺表达线段</param>
        ///// <param name="tocpl">大比例尺表达线段</param>
        ///// <param name="pParameterThreshold">阈值参数</param>
        ///// <returns>若两河流为同一河流，返回true，否则返回faInterLSe</returns>
        ///// <remarks>DWByDPLA:DealWithByDP based on Lengths and Angles of the base-lines
        ///// 本方法主要用于外部对DP算法的调用</remarks>
        //public C5.LinkedList<CCorrespondSegment> DWByDPLADefine(CPolyline frcpl, CPolyline tocpl, CParameterThreshold ParameterThreshold)
        //{
        //    //if (frcpl.pPolyline.Length == 0 || tocpl.pPolyline.Length == 0)
        //    //{
        //    //    C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk00 = new C5.LinkedList<CCorrespondSegment>();
        //    //    return CorrespondSegmentLk00;
        //    //}

        //    //DivideCplByDP(frcpl);
        //    //DivideCplByDP(tocpl);           

        //    ////进行弯曲匹配，提取对应线段
        //    //C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //    //SubPolylineMatchLA(frcpl, tocpl, ParameterThreshold, ref pCorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

        //    //return pCorrespondSegmentLk;
        //    return null;
        //}

       
        ///// <summary>用基于BLG树的方法处理Morphing问题，指定边长比参数</summary>
        ///// <param name="frcpl">大比例尺表达线段</param>
        ///// <param name="tocpl">大比例尺表达线段</param>
        ///// <param name="pParameterThreshold">阈值参数</param>
        ///// <returns>若两河流为同一河流，返回true，否则返回faInterLSe</returns>
        ///// <remarks>DWByDP:DealWithByDP
        ///// 本方法主要用于外部对DP算法的调用</remarks>
        //public C5.LinkedList<CCorrespondSegment> DWByDPDefine(CPolyline frcpl, CPolyline tocpl, double dblBound)
        //{
        //    //if (frcpl.pPolyline.Length == 0 || tocpl.pPolyline.Length == 0)
        //    //{
        //    //    C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk00 = new C5.LinkedList<CCorrespondSegment>();
        //    //    return CorrespondSegmentLk00;
        //    //}

        //    ////DivideCplByDP(frcpl);
        //    ////DivideCplByDP(tocpl);

        //    //CParameterThreshold ParameterThreshold = new CParameterThreshold();
        //    //ParameterThreshold.dblDLengthBound = dblBound;
        //    //ParameterThreshold.dblDLengthBound = 1 / dblBound;

        //    ////进行弯曲匹配，提取对应线段
        //    //C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //    //SubPolylineMatchLA(frcpl, tocpl, ParameterThreshold, ref pCorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

        //    //return pCorrespondSegmentLk;
        //    return null;
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="CFrPolyline"></param>
        ///// <param name="CToPolyline"></param>
        ///// <param name="ParameterThreshold"></param>
        ///// <param name="CorrespondSegmentLk"></param>
        ///// <remarks>angles are considered</remarks>
        //public void SubPolylineMatchLA(CPolyline frcpl, CVirtualPolyline vfrcpl, CPolyline tocpl, CVirtualPolyline vtocpl, CParameterThreshold ParameterThreshold, ref C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk)
        //{
        //    //如果其中一个线段没有孩子，则直接匹配并结束
        //    if (vfrcpl.CLeftPolyline == null || vtocpl.CLeftPolyline == null)
        //    {
        //        CPolyline frsubcpl = frcpl.GetSubPolyline(vfrcpl.intFrID, vfrcpl.intToID);
        //        CPolyline tosubcpl = tocpl.GetSubPolyline(vtocpl.intFrID, vtocpl.intToID);
        //        CCorrespondSegment CorrespondSegment = new CCorrespondSegment(frsubcpl, tosubcpl);
        //        CorrespondSegmentLk.Add(CorrespondSegment);
        //        return;
        //    }

        //    double dblRatioLL = vfrcpl.CLeftPolyline .pBaseLine.Length / vtocpl.CLeftPolyline.pBaseLine.Length;
        //    double dblRatioRR = vfrcpl.CRightPolyline.pBaseLine.Length / vtocpl.CRightPolyline.pBaseLine.Length;

        //    double dblFrDiffLLX = vfrcpl.CLeftPolyline.pBaseLine.ToCpt.X - vfrcpl.CLeftPolyline.pBaseLine.FrCpt.X;
        //    double dblFrDiffLLY = vfrcpl.CLeftPolyline.pBaseLine.ToCpt.Y - vfrcpl.CLeftPolyline.pBaseLine.FrCpt.Y;
        //    double dblToDiffLLX = vtocpl.CLeftPolyline.pBaseLine.ToCpt.X - vtocpl.CLeftPolyline.pBaseLine.FrCpt.X;
        //    double dblToDiffLLY = vtocpl.CLeftPolyline.pBaseLine.ToCpt.Y - vtocpl.CLeftPolyline.pBaseLine.FrCpt.Y;
        //    double dblAngleDiffLL = CGeoFunc.CalAngle(dblFrDiffLLX, dblFrDiffLLY, dblToDiffLLX, dblToDiffLLY);

        //    double dblFrDiffRRX = vfrcpl.CRightPolyline.pBaseLine.ToCpt.X - vfrcpl.CRightPolyline.pBaseLine.FrCpt.X;
        //    double dblFrDiffRRY = vfrcpl.CRightPolyline.pBaseLine.ToCpt.Y - vfrcpl.CRightPolyline.pBaseLine.FrCpt.Y;
        //    double dblToDiffRRX = vtocpl.CRightPolyline.pBaseLine.ToCpt.X - vtocpl.CRightPolyline.pBaseLine.FrCpt.X;
        //    double dblToDiffRRY = vtocpl.CRightPolyline.pBaseLine.ToCpt.Y - vtocpl.CRightPolyline.pBaseLine.FrCpt.Y;
        //    double dblAngleDiffRR = CGeoFunc.CalAngle(dblFrDiffRRX, dblFrDiffRRY, dblToDiffRRX, dblToDiffRRY);

        //    if ((dblRatioLL >= ParameterThreshold.dblDLengthBound) && (dblRatioLL <= ParameterThreshold.dblULengthBound) &&
        //        (dblRatioRR >= ParameterThreshold.dblDLengthBound) && (dblRatioRR <= ParameterThreshold.dblULengthBound) &&
        //        (Math.Abs(dblAngleDiffLL) <= ParameterThreshold.dblAngleBound) && (dblAngleDiffRR <= ParameterThreshold.dblAngleBound))
        //    {
        //        //相应线段长度相近
        //        SubPolylineMatchLA(frcpl,vfrcpl.CLeftPolyline, tocpl, vtocpl.CLeftPolyline, ParameterThreshold, ref CorrespondSegmentLk);
        //        SubPolylineMatchLA(frcpl, vfrcpl.CRightPolyline, tocpl, vtocpl.CRightPolyline, ParameterThreshold, ref CorrespondSegmentLk);
        //    }
        //    eInterLSe
        //    {
        //        CPolyline frsubcpl = frcpl.GetSubPolyline(vfrcpl.intFrID, vfrcpl.intToID);
        //        CPolyline tosubcpl = tocpl.GetSubPolyline(vtocpl.intFrID, vtocpl.intToID);
        //        CCorrespondSegment CorrespondSegment = new CCorrespondSegment(frsubcpl, tosubcpl);
        //        CorrespondSegmentLk.Add(CorrespondSegment);
        //        return;
        //    }
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="CFrPolyline"></param>
        ///// <param name="CToPolyline"></param>
        ///// <param name="ParameterThreshold"></param>
        ///// <param name="CorrespondSegmentLk"></param>
        ///// <remarks>angles are not considered</remarks>
        //public void SubPolylineMatchL(CPolyline CFrPolyline, CPolyline CToPolyline, CParameterThreshold ParameterThreshold, ref C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk)
        //{
        //    //如果其中一个线段没有孩子，则直接匹配并结束
        //    if (CFrPolyline.CLeftPolyline == null || CToPolyline.CLeftPolyline == null)
        //    {
        //        CCorrespondSegment CorrespondSegment = new CCorrespondSegment(CFrPolyline, CToPolyline);
        //        CorrespondSegmentLk.Add(CorrespondSegment);
        //        return;
        //    }

        //    double dblRatioLL = CFrPolyline.CLeftPolyline.pBaseLine.Length / CToPolyline.CLeftPolyline.pBaseLine.Length;
        //    double dblRatioRR = CFrPolyline.CRightPolyline.pBaseLine.Length / CToPolyline.CRightPolyline.pBaseLine.Length;

        //    if ((dblRatioLL >= ParameterThreshold.dblDLengthBound) && (dblRatioLL <= ParameterThreshold.dblULengthBound) &&
        //        (dblRatioRR >= ParameterThreshold.dblDLengthBound) && (dblRatioRR <= ParameterThreshold.dblULengthBound))
        //    {
        //        //相应线段长度相近
        //        SubPolylineMatchL(CFrPolyline.CLeftPolyline, CToPolyline.CLeftPolyline, ParameterThreshold, ref CorrespondSegmentLk);
        //        SubPolylineMatchL(CFrPolyline.CRightPolyline, CToPolyline.CRightPolyline, ParameterThreshold, ref CorrespondSegmentLk);
        //    }
        //    eInterLSe
        //    {
        //        CCorrespondSegment CorrespondSegment = new CCorrespondSegment(CFrPolyline, CToPolyline);
        //        CorrespondSegmentLk.Add(CorrespondSegment);
        //        return;
        //    }
        //}

        /////// <summary>DP算法化简线状要素</summary>
        /////// <param name="dcpl">当前遍历线段</param>
        /////// <param name="dblDPBound">阈值参数</param>
        /////// <return>DP算法化简后得到的线状要素</return>
        ////public CPolyline GetCplByDP(CPolyline dcpl, double dblDPBound)
        ////{
        ////    DivideCplByDP(dcpl);

        ////    List<CPoint> newcptlt = new List<CPoint>();
        ////    newcptlt.Add(dcpl.CptLt[0]);
        ////    //继续深层次遍历
        ////    RecursivelyGetnewcptlt(dcpl, ref newcptlt, dblDPBound);
        ////    CPolyline cpl = new CPolyline(0, newcptlt);
        ////    return cpl;
        ////}


        /////// <summary>递归获取DP算法化简得到的心现状要素数组</summary>
        /////// <param name="dcpl">当前遍历线段</param>
        /////// <param name="newcptlt">记录新的顶点数组</param>
        /////// <param name="dblDPBound">阈值参数</param>
        ////public void RecursivelyGetnewcptlt(CPolyline dcpl,ref List<CPoint> newcptlt,double dblDPBound)
        ////{
        ////    if (dcpl.dblMaxDis >= dblDPBound)
        ////    {
        ////        //考虑到顶点的记录顺序，如下安排
        ////        RecursivelyGetnewcptlt(dcpl.CLeftPolyline, ref newcptlt, dblDPBound);
        ////        RecursivelyGetnewcptlt(dcpl.CRightPolyline, ref newcptlt, dblDPBound);
        ////    }
        ////    eInterLSe
        ////    {
        ////        newcptlt.Add(dcpl.CptLt[dcpl.CptLt.Count - 1]); 
        ////    }
            

        //    //RecursivelyGetnewcptlt(dcpl.CLeftPolyline, ref newcptlt, dblDPBound);


        //    //if (dcpl.dblDPMaxDis >= dblDPBound)
        //    //{
        //    //    //考虑到顶点的记录顺序，如下安排
        //    //    RecursivelyGetnewcptlt(dcpl.CLeftPolyline, ref newcptlt, dblDPBound);
        //    //    newcptlt.Add(dcpl.CptLt[dcpl.intMaxDisNum]);
        //    //    RecursivelyGetnewcptlt(dcpl.CRightPolyline, ref newcptlt, dblDPBound);
        //    //}
        //    //newcptlt.Add(dcpl.CptLt[dcpl.CptLt.Count - 1]);
        ////}


        ///// <summary>属性：处理结果</summary>
        //public CParameterResult ParameterResult
        //{
        //    get { return _ParameterResult; }
        //    set { _ParameterResult = value; }
        //}
    }
}
