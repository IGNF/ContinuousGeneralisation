using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CCorrepondObjects;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;

namespace MorphingClass.CMorphingMethods
{
    /// <summary>
    /// 递归调用弯曲结构及BLG树结构的方法
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CBSBLGOptCor
    {
        
        
        private CParameterResult _ParameterResult;

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private CParameterInitialize _ParameterInitialize;

        public CBSBLGOptCor()
        {

        }

        public CBSBLGOptCor(CPolyline frcpl, CPolyline tocpl)
        {
            _FromCpl = frcpl;
            _ToCpl = tocpl;
        }

        public CBSBLGOptCor(CParameterInitialize ParameterInitialize)
        {
            //获取当前选择的点要素图层
            //大比例尺要素图层
            IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLargerScaleLayer.SelectedIndex);
                                                                       
            //小比例尺要素图层
            IFeatureLayer pSSFLayer =(IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboSmallerScaleLayer.SelectedIndex);
                                                           


            ParameterInitialize.pBSFLayer = pBSFLayer;
            ParameterInitialize.pSSFLayer = pSSFLayer;
            ParameterInitialize.intMaxBackK = Convert.ToInt32(ParameterInitialize.txtMaxBackK.Text);
            _ParameterInitialize = ParameterInitialize;

            //获取线数组
            _LSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pBSFLayer);
            _SSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSSFLayer);
        }

        //基于弯曲的Morphing方法
        public void BSBLGOptCorMorphing()
        {
            //var ParameterInitialize = _ParameterInitialize;
            //CPolyline frcpl = _LSCPlLt[0];
            //CPolyline tocpl = _SSCPlLt[0];


            ////计算极小值
            //
            //long lngStartTime = System.Environment.TickCount;  //开始时间
            //CHelpFunc.PreviousWorkCpl(ref frcpl, CEnumScale.Larger);
            //CHelpFunc.PreviousWorkCpl(ref frcpl, CEnumScale.Smaller);

            //////建立CDT并获取弯曲森林
            ////CBendForest FromLeftBendForest = new CBendForest();
            ////CBendForest FromRightBendForest = new CBendForest();
            ////CParameterVariable pParameterVariableFrom = new CParameterVariable(frcpl, "FromCDT", ParameterInitialize.pBSFLayer, dblVerySmall);
            ////OptMPBBSL.GetBendForest(pParameterVariableFrom, ref FromLeftBendForest, ref FromRightBendForest, ParameterInitialize);//获取折线的弯曲森林

            ////CBendForest ToLeftBendForest = new CBendForest();
            ////CBendForest ToRightBendForest = new CBendForest();
            ////CParameterVariable pParameterVariableTo = new CParameterVariable(tocpl, "ToCDT", ParameterInitialize.pSSFLayer, dblVerySmall);
            ////OptMPBBSL.GetBendForest(pParameterVariableTo, ref ToLeftBendForest, ref ToRightBendForest, ParameterInitialize);

            //////整理弯曲森林，获得弯曲数组
            ////SortedList<double, CBend> FromLeftBendSlt = OptMPBBSL.NeatenBendForest(frcpl, FromLeftBendForest);//整理弯曲数组中的弯曲
            ////SortedList<double, CBend> FromRightBendSlt = OptMPBBSL.NeatenBendForest(frcpl, FromRightBendForest);
            ////SortedList<double, CBend> ToLeftBendSlt = OptMPBBSL.NeatenBendForest(tocpl, ToLeftBendForest);
            ////SortedList<double, CBend> ToRightBendSlt = OptMPBBSL.NeatenBendForest(tocpl, ToRightBendForest);


            //////计算阈值参数
            ////CParameterThreshold ParameterThreshold = new CParameterThreshold();
            ////ParameterThreshold.dblFrLength = frcpl.pPolyline.Length;
            ////ParameterThreshold.dblToLength = tocpl.pPolyline.Length;
            ////ParameterThreshold.dblDLengthBound = 1 * 0.98;
            ////ParameterThreshold.dblULengthBound = 1 / 0.98;

            //////**************弯曲处理过程**************//
            //////弯曲树匹配，寻找对应独立弯曲
            ////List<CCorrespondBend> pIndependCorrespondBendLt = new List<CCorrespondBend>();
            ////pIndependCorrespondBendLt.AddRange(OptMPBBSL.BendTreeMatch(FromLeftBendForest, ToLeftBendForest, ParameterThreshold));
            ////pIndependCorrespondBendLt.AddRange(OptMPBBSL.BendTreeMatch(FromRightBendForest, ToRightBendForest, ParameterThreshold));

            //////弯曲匹配，寻找对应弯曲
            ////List<CCorrespondBend> pCorrespondBendLt = OptMPBBSL.BendMatch(pIndependCorrespondBendLt, ParameterThreshold);

            ////提取对应线段
            ////C5.LinkedList<CCorrSegment> pBSCorrespondSegmentLk = OptMPBBSL.DetectCorrespondSegment(frcpl, tocpl, pCorrespondBendLt);


            ////计算阈值参数
            //double dblBound = 0.98;
            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //ParameterThreshold.dblFrLength = frcpl.pPolyline.Length;
            //ParameterThreshold.dblToLength = tocpl.pPolyline.Length;
            //ParameterThreshold.dblDLengthBound = dblBound;
            //ParameterThreshold.dblULengthBound = 1 / dblBound;

            ////**************弯曲处理过程**************//
            //CMPBBSL OptMPBBSL = new CMPBBSL();
            //CParameterVariable pParameterVariableFrom = new CParameterVariable(frcpl, "FromCDT", null, dblVerySmall);
            //CParameterVariable pParameterVariableTo = new CParameterVariable(tocpl, "ToCDT", null, dblVerySmall);
            //C5.LinkedList<CCorrSegment> pBSCorrespondSegmentLk = new C5.LinkedList<CCorrSegment>();
            //pBSCorrespondSegmentLk.AddRange(OptMPBBSL.DWByMPBBSL(ParameterInitialize, pParameterVariableFrom, pParameterVariableTo, ParameterThreshold));

            ////**************BLG树处理过程**************//
            ////经过BLG树处理后的最终对应线段
            //CMPBDPBL OptMPBDPBL = new CMPBDPBL();//作用
            //C5.LinkedList<CCorrSegment> pBLGCorrespondSegmentLk = new C5.LinkedList<CCorrSegment>();
            //for (int j = 0; j < pBSCorrespondSegmentLk.Count; j++)
            //{
            //    pBLGCorrespondSegmentLk.AddRange(OptMPBDPBL.DWByDPLADefine(pBSCorrespondSegmentLk[j].CFrPolyline, pBSCorrespondSegmentLk[j].CToPolyline, ParameterThreshold));
            //}

            ////**************OptCor处理过程**************//
            //COptCor OptOptCor = new COptCor();
            //C5.LinkedList<CCorrSegment> pOptCorCorrespondSegmentLk = new C5.LinkedList<CCorrSegment>();
            //for (int j = 0; j < pBLGCorrespondSegmentLk.Count; j++)
            //{
            //    List<CPolyline> CFrEdgeLt = CGeoFunc.CreateCplLt(pOptCorCorrespondSegmentLk[j].CFrPolyline.CptLt);
            //    List<CPolyline> CToEdgeLt = CGeoFunc.CreateCplLt(pOptCorCorrespondSegmentLk[j].CToPolyline.CptLt);

            //    pOptCorCorrespondSegmentLk.AddRange(OptOptCor.DWByOptCor(frcpl, tocpl, CFrEdgeLt, CToEdgeLt, ParameterInitialize.intMaxBackK));
            //}

            //////**************OptCor处理过程**************//
            ////COptCorSimplified OptCorSimplified = new COptCorSimplified();
            ////C5.LinkedList<CCorrSegment> pOptCorCorrespondSegmentLk = new C5.LinkedList<CCorrSegment>();
            ////double dblX = tocpl.CptLt[0].X - frcpl.CptLt[0].X;
            ////double dblY = tocpl.CptLt[0].Y - frcpl.CptLt[0].Y;
            ////CPoint StandardVectorCpt = new CPoint(0, dblX, dblY);
            ////for (int j = 0; j < pBLGCorrespondSegmentLk.Count; j++)
            ////{
            ////    pOptCorCorrespondSegmentLk.AddRange(OptCorSimplified.DWByOptCorSimplified(pBLGCorrespondSegmentLk[j].CFrPolyline, pBLGCorrespondSegmentLk[j].CToPolyline, ParameterInitialize.intMaxBackK, StandardVectorCpt));
            ////}

            ////按指定方式对对应线段进行点匹配，提取对应点
            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();//作用调用对象的函数
            //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pOptCorCorrespondSegmentLk, "Linear");

            ////保存对应线
            //CHelpFunc.SaveCtrlLine(pOptCorCorrespondSegmentLk, "BSBLGOptCorControlLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCorrespondLine(pResultPtLt, "BSBLGOptCorCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////计算并显示运行时间
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //_ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            ////获取结果，全部记录在ParameterResult中
            //CTranslation pTranslation = new CTranslation();
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = frcpl;
            //ParameterResult.ToCpl = tocpl;
            //double pdblTranslation = pTranslation.CalTranslation(pResultPtLt);
            //ParameterResult.dblTranslation = pdblTranslation;
            //ParameterResult.pCorrespondSegmentLk = pOptCorCorrespondSegmentLk;
            //ParameterResult.CResultPtLt = pResultPtLt;
            //ParameterResult.lngTime = lngTime;
            //_ParameterResult = ParameterResult;
        }


        /// <summary>属性：处理结果</summary>
        public CParameterInitialize ParameterInitialize
        {
            get { return _ParameterInitialize; }
            set { _ParameterInitialize = value; }
        }

        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
