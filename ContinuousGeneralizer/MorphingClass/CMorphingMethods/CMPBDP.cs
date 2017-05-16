using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CMorphingMethods
{
    public class CMPBDP
    {
        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        
        
        private CParameterResult _ParameterResult;

        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private CParameterInitialize _ParameterInitialize;

        public CMPBDP()
        {

        }

        public CMPBDP(CParameterInitialize ParameterInitialize)
        {

            //获取当前选择的点要素图层
            //大比例尺要素图层
            IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLargerScaleLayer.SelectedIndex);
                                                                       
            //小比例尺要素图层
            IFeatureLayer pSSFLayer =(IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboSmallerScaleLayer.SelectedIndex);
                                                           

            ParameterInitialize.pBSFLayer = pBSFLayer;
            ParameterInitialize.pSSFLayer = pSSFLayer;
            _ParameterInitialize = ParameterInitialize;

            //获取线数组
            _LSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pBSFLayer);
            _SSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSSFLayer);
        }


        public void MPBDPMorphing()
        {
            //CParameterInitialize ParameterInitialize = _ParameterInitialize;
            //CPolyline frcpl = _LSCPlLt[0];
            //CPolyline tocpl = _SSCPlLt[0];

            //DivideCplByDP(frcpl);
            //DivideCplByDP(tocpl);

            //double dblLengthSumRatio = frcpl.pPolyline.Length / tocpl.pPolyline.Length;
            ////double dblLengthBound = _ParameterInitialize.dblLengthBound;

            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
            ////double dblSumLength = frcpl.pPolyline.Length + tocpl.pPolyline.Length;
            //CTranslation pTranslation = new CTranslation();
            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            ////ParameterThreshold.dblLengthBound = dblLengthBound;

            //int intIndex = 0;
            //double dblMin = double.MaxValue;
            //List<double> dblTranslationLt = new List<double>();
            //for (int i = 0; i < 50; i++)
            //{
            //    ParameterThreshold.dblDLengthBound = dblLengthSumRatio * (1 - 0.02 * i);
            //    ParameterThreshold.dblULengthBound = dblLengthSumRatio / (1 - 0.02 * i);

            //    //进行弯曲匹配，提取对应线段
            //    C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            //    SubPolylineMatch(frcpl, tocpl, ParameterThreshold, ref CorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

            //    //按指定方式对对应线段进行点匹配，提取对应点                
            //    List<CPoint> ResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(CorrespondSegmentLk, "Linear");
            //    double dblTranslation = pTranslation.CalTranslation(ResultPtLt);
            //    dblTranslationLt.Add(dblTranslation);
            //    if (dblTranslation < dblMin)
            //    {
            //        intIndex = i;
            //        dblMin = dblTranslation;
            //    }
            //}


            ////double dblRatio = Convert.ToDouble(ParameterInitialize.txtLengthBound.Text);
            ////ParameterThreshold.dblDLengthBound = dblLengthSumRatio * dblRatio;
            ////ParameterThreshold.dblULengthBound = dblLengthSumRatio / dblRatio;


            //ParameterThreshold.dblDLengthBound = dblLengthSumRatio * (1 - 0.02 * intIndex);
            //ParameterThreshold.dblULengthBound = dblLengthSumRatio / (1 - 0.02 * intIndex);

            ////进行弯曲匹配，提取对应线段
            //C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            //SubPolylineMatch(frcpl, tocpl, ParameterThreshold, ref pCorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

            ////按指定方式对对应线段进行点匹配，提取对应点
            //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");


            ////保存对应线
            ////CHelpFuncExcel.ExportDataltToExcel(dblTranslationLt, "translationlt0", _ParameterInitialize.strSavePath);
            //CHelpFunc.SaveCtrlLine(pCorrespondSegmentLk, "MPBDPControlLine", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCorrespondLine(pResultPtLt, "MPBDPCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = frcpl;
            //ParameterResult.ToCpl = tocpl;

            //ParameterResult.CResultPtLt = pResultPtLt;
            //_ParameterResult = ParameterResult;
        }


        ///// <summary>用基于BLG树的方法处理Morphing问题</summary>
        ///// <param name="frcpl">大比例尺表达线段</param>
        ///// <param name="tocpl">大比例尺表达线段</param>
        ///// <param name="pParameterThreshold">阈值参数</param>
        ///// <returns>若两河流为同一河流，返回true，否则返回false</returns>
        ///// <remarks>DWByDP:DealWithByDP
        ///// 本方法主要用于外部对DP算法的调用</remarks>
        //public List<CPoint> DWByDP(CPolyline frcpl, CPolyline tocpl, double dblLengthSumRatio, string strMethod)
        //{
        //    if (frcpl.pPolyline.Length ==0||tocpl.pPolyline.Length ==0)
        //    {
        //        List<CPoint> cptlt00 = new List<CPoint>();
        //        return cptlt00;
        //    }


        //    DivideCplByDP(frcpl);
        //    DivideCplByDP(tocpl);


        //    CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
        //    //double dblSumLength = frcpl.pPolyline.Length + tocpl.pPolyline.Length;
        //    CTranslation pTranslation = new CTranslation();
        //    CParameterThreshold ParameterThreshold = new CParameterThreshold();

        //    int intIndex = 0;
        //    double dblMin = double.MaxValue;
        //    List<double> dblTranslationLt = new List<double>();
        //    for (int i = 0; i < 25; i++)
        //    {
        //        ParameterThreshold.dblDLengthBound = dblLengthSumRatio * (1 - 0.02 * i);
        //        ParameterThreshold.dblULengthBound = dblLengthSumRatio / (1 - 0.02 * i);

        //        //进行弯曲匹配，提取对应线段
        //        C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //        SubPolylineMatch(frcpl, tocpl, ParameterThreshold, ref CorrespondSegmentLk);

        //        //按指定方式对对应线段进行点匹配，提取对应点                
        //        List<CPoint> ResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(CorrespondSegmentLk, strMethod);
        //        double dblTranslation = pTranslation.CalTranslation(ResultPtLt);
        //        dblTranslationLt.Add(dblTranslation);
        //        if (dblTranslation < dblMin)
        //        {
        //            intIndex = i;
        //            dblMin = dblTranslation;
        //        }
        //    }

        //    ParameterThreshold.dblDLengthBound = dblLengthSumRatio * (1 - 0.02 * intIndex);
        //    ParameterThreshold.dblULengthBound = dblLengthSumRatio / (1 - 0.02 * intIndex);


        //    //进行弯曲匹配，提取对应线段
        //    C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //    SubPolylineMatch(frcpl, tocpl, ParameterThreshold, ref pCorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

        //    //按指定方式对对应线段进行点匹配，提取对应点
        //    List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");

        //    return pResultPtLt;
        //}

        ///// <summary>用基于BLG树的方法处理Morphing问题，指定边长比参数</summary>
        ///// <param name="frcpl">大比例尺表达线段</param>
        ///// <param name="tocpl">大比例尺表达线段</param>
        ///// <param name="pParameterThreshold">阈值参数</param>
        ///// <returns>若两河流为同一河流，返回true，否则返回false</returns>
        ///// <remarks>DWByDP:DealWithByDP
        ///// 本方法主要用于外部对DP算法的调用</remarks>
        //public C5.LinkedList<CCorrespondSegment> DWByDPDefine(CPolyline frcpl, CPolyline tocpl, CParameterThreshold ParameterThreshold)
        //{
        //    if (frcpl.pPolyline.Length == 0 || tocpl.pPolyline.Length == 0)
        //    {
        //        C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk00 = new C5.LinkedList<CCorrespondSegment>();
        //        return CorrespondSegmentLk00;
        //    }

        //    DivideCplByDP(frcpl);
        //    DivideCplByDP(tocpl);           

        //    //进行弯曲匹配，提取对应线段
        //    C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //    SubPolylineMatch(frcpl, tocpl, ParameterThreshold, ref pCorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

        //    return pCorrespondSegmentLk;
        //}

        ///// <summary>用基于BLG树的方法处理Morphing问题，指定边长比参数</summary>
        ///// <param name="frcpl">大比例尺表达线段</param>
        ///// <param name="tocpl">大比例尺表达线段</param>
        ///// <param name="pParameterThreshold">阈值参数</param>
        ///// <returns>若两河流为同一河流，返回true，否则返回false</returns>
        ///// <remarks>DWByDP:DealWithByDP
        ///// 本方法主要用于外部对DP算法的调用</remarks>
        //public C5.LinkedList<CCorrespondSegment> DWByDPDefine(CPolyline frcpl, CPolyline tocpl, double dblBound)
        //{
        //    if (frcpl.pPolyline.Length == 0 || tocpl.pPolyline.Length == 0)
        //    {
        //        C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk00 = new C5.LinkedList<CCorrespondSegment>();
        //        return CorrespondSegmentLk00;
        //    }

        //    DivideCplByDP(frcpl);
        //    DivideCplByDP(tocpl);

        //    CParameterThreshold ParameterThreshold = new CParameterThreshold();
        //    ParameterThreshold.dblDLengthBound = dblBound;
        //    ParameterThreshold.dblDLengthBound = 1/dblBound;

        //    //进行弯曲匹配，提取对应线段
        //    C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
        //    SubPolylineMatch(frcpl, tocpl, ParameterThreshold, ref pCorrespondSegmentLk);//弯曲树匹配和孩子弯曲匹配都由此函数来执行

        //    return pCorrespondSegmentLk;
        //}

        //public void DivideCplByDP(CPolyline dcpl)
        //{
        //    List<CPoint> dcptlt = dcpl.CptLt;
        //    int intptnum = dcptlt.Count;

        //    if (intptnum <= 2)
        //    {
        //        return;
        //    }

        //    //找到距离基础边最远的点
        //    CEdge pEdge = new CEdge(dcptlt[0], dcptlt[intptnum - 1]);
        //    double dblMaxDis = -1;
        //    int intMaxDisIndex = 0;
        //    double dblAlongDis = 0;
        //    double dblFromDis = 0;
        //    IPoint outipt = new PointClass();
        //    bool blnright = new bool();
        //    for (int i = 1; i < intptnum - 1; i++)
        //    {
        //        pEdge.QueryPointAndDistance(esriSegmentExtension.esriExtendEmbedded, (IPoint)dcptlt[i], false, outipt, ref dblAlongDis, ref dblFromDis, ref blnright);
        //        if (dblFromDis > dblMaxDis)
        //        {
        //            dblMaxDis = dblFromDis;
        //            intMaxDisIndex = i;
        //        }
        //    }

        //    //分别对左右子边执行分割操作
        //    dcpl.DivideByCpt(dcptlt[intMaxDisIndex]);
        //    DivideCplByDP(dcpl.CLeftPolyline);
        //    DivideCplByDP(dcpl.CRightPolyline);
        //}

        ///// <summary>基于曲线长度的匹配方法</summary>
        ///// <param name="CFrPolyline">大比例尺表达线段</param>
        ///// <param name="CToPolyline">大比例尺表达线段</param>
        ///// <param name="ParameterThreshold">阈值参数</param>
        ///// <param name="CorrespondSegmentLk">对应线段</param>
        //public void SubPolylineMatch(CPolyline CFrPolyline, CPolyline CToPolyline, CParameterThreshold ParameterThreshold, ref C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk)
        //{
        //    //如果其中一个线段没有孩子，则直接匹配并结束
        //    if (CFrPolyline.CLeftPolyline == null || CToPolyline.CLeftPolyline == null)
        //    {
        //        CCorrespondSegment CorrespondSegment = new CCorrespondSegment(CFrPolyline, CToPolyline);
        //        CorrespondSegmentLk.Add(CorrespondSegment);
        //        return;
        //    }

        //    double dblRatioLL = CFrPolyline.CLeftPolyline.pPolyline.Length / CToPolyline.CLeftPolyline.pPolyline.Length;
        //    double dblRatioRR = CFrPolyline.CRightPolyline.pPolyline.Length / CToPolyline.CRightPolyline.pPolyline.Length;

        //    int intInsertIndex = CorrespondSegmentLk.Count;

        //    if (dblRatioLL >= ParameterThreshold.dblDLengthBound && dblRatioLL <= ParameterThreshold.dblULengthBound &&
        //        dblRatioRR >= ParameterThreshold.dblDLengthBound && dblRatioRR <= ParameterThreshold.dblULengthBound)
        //    {
        //        //相应线段长度相近
        //        SubPolylineMatch(CFrPolyline.CLeftPolyline, CToPolyline.CLeftPolyline, ParameterThreshold, ref CorrespondSegmentLk);
        //        SubPolylineMatch(CFrPolyline.CRightPolyline, CToPolyline.CRightPolyline, ParameterThreshold, ref CorrespondSegmentLk);
        //    }
        //    else
        //    {
        //        CCorrespondSegment CorrespondSegment = new CCorrespondSegment(CFrPolyline, CToPolyline);
        //        CorrespondSegmentLk.Add(CorrespondSegment);
        //        return;
        //    }

        //}




        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
