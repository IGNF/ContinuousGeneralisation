using System;
using System.Linq;
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
    /// 多次使用独立弯曲结构(Recursive Independent Bend Structures)
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CRIBS
    {
        
        
        private CTriangulator _Triangulator = new CTriangulator();
        private CParameterResult _ParameterResult;

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private double _dblCDTNum = 1;   //由于本方法要多次创建CDT，为了使各生成的CDT在名字上有所区别，故用此变量

        private CParameterInitialize _ParameterInitialize;

        public CRIBS()
        {

        }

        public CRIBS(CPolyline frcpl, CPolyline tocpl)
        {
            _FromCpl = frcpl;
            _ToCpl = tocpl;
        }

        public CRIBS(CParameterInitialize ParameterInitialize)
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

        //基于弯曲的Morphing方法
        public void RIBSMorphing()
        {
            //var ParameterInitialize = _ParameterInitialize;
            //CMPBBSL OptMPBBSL = new CMPBBSL();

            //CPolyline frcpl = _LSCPlLt[0];
            //CPolyline tocpl = _SSCPlLt[0];

            //_FromCpl=frcpl;
            //_ToCpl = tocpl;

            ////*******为什么不调用"OptMPBBSL.DWByMPBBSL" ?
            ////*******因为这里的弯曲匹配方法(独立弯曲、子弯曲)不一样，增加了新的重建三角网的内容

            ////计算极小值
            //
            //long lngStartTime = System.Environment.TickCount;  //开始时间

            //List<CPoint> frchcptlt = _Triangulator.CreateConvexHullEdgeLt2(frcpl, dblVerySmall);
            //CPolyline frchcpl = new CPolyline(0, frchcptlt);    //大比例尺折线外包多边形线段

            //List<CPoint> tochcptlt = _Triangulator.CreateConvexHullEdgeLt2(tocpl, dblVerySmall);
            //CPolyline tochcpl = new CPolyline(0, tochcptlt);    //小比例尺折线外包多边形线段

            ////添加约束数据生成图层，以便于利用AE中的功能(ct:constraint)
            //List<CPolyline> frctcpllt = new List<CPolyline>(); frctcpllt.Add(frcpl); frctcpllt.Add(frchcpl);
            //List<CPolyline> toctcpllt = new List<CPolyline>(); toctcpllt.Add(tocpl); toctcpllt.Add(tochcpl);
            //IFeatureLayer pBSFLayer = CHelpFunc.SaveCPlLt(frctcpllt, "frctcpllt" + _dblCDTNum, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //IFeatureLayer pSSFLayer = CHelpFunc.SaveCPlLt(toctcpllt, "toctcpllt" + _dblCDTNum, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            ////建立CDT并获取弯曲森林
            //CBendForest FromLeftBendForest = new CBendForest();
            //CBendForest FromRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableFrom = new CParameterVariable(frcpl, "FromCDT" + _dblCDTNum, pBSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableFrom, ref FromLeftBendForest, ref FromRightBendForest, ParameterInitialize);

            //CBendForest ToLeftBendForest = new CBendForest();
            //CBendForest ToRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableTo = new CParameterVariable(tocpl, "ToCDT" + _dblCDTNum, pSSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableTo, ref ToLeftBendForest, ref ToRightBendForest, ParameterInitialize);

            ////整理弯曲森林，获得弯曲数组
            //OptMPBBSL.NeatenBendForest(frcpl, FromLeftBendForest);
            //OptMPBBSL.NeatenBendForest(frcpl, FromRightBendForest);
            //OptMPBBSL.NeatenBendForest(tocpl, ToLeftBendForest);
            //OptMPBBSL.NeatenBendForest(tocpl, ToRightBendForest);

            //_dblCDTNum = _dblCDTNum + 1;


            ////计算阈值参数
            //double dblBound = 0.99;
            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //ParameterThreshold.dblAngleBound = Math.PI * (1 - dblBound);
            //ParameterThreshold.dblDLengthBound = dblBound;
            //ParameterThreshold.dblULengthBound = 1 / dblBound;
            //ParameterThreshold.dblVerySmall = dblVerySmall;

            ////弯曲树匹配，寻找对应独立弯曲
            //List<CCorrespondBend> IndependCorrespondBendLt = new List<CCorrespondBend>();
            //IndependCorrespondBendLt.AddRange(BendTreeMatch(FromLeftBendForest, ToLeftBendForest, ParameterThreshold, ParameterInitialize));
            //IndependCorrespondBendLt.AddRange(BendTreeMatch(FromRightBendForest, ToRightBendForest, ParameterThreshold, ParameterInitialize));

            ////弯曲匹配，寻找对应弯曲
            //List<CCorrespondBend> CorrespondBendLt = BendMatch(IndependCorrespondBendLt, ParameterThreshold, ParameterInitialize);

            ////提取对应线段
            //C5.LinkedList<CCorrSegment> pCorrespondSegmentLk = CHelpFunc.DetectCorrespondSegment(frcpl, tocpl, CorrespondBendLt);

            ////按指定方式对对应线段进行点匹配，提取对应点   
            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper(); 
            //List<CPoint> pResultPtLt = new List<CPoint>();
            //pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");

            ////计算并显示运行时间
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //_ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            ////保存对应线
            //CHelpFunc.SaveCtrlLine(pCorrespondSegmentLk,"CRIBSControlLine" + dblBound, _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCorrespondLine(pResultPtLt, "CRIBSCorrLine" + dblBound, _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = frcpl;
            //ParameterResult.ToCpl = tocpl;
            //ParameterResult.pCorrespondSegmentLk = pCorrespondSegmentLk;
            //ParameterResult.CResultPtLt = pResultPtLt;
            //ParameterResult.lngTime = lngTime;
            //_ParameterResult = ParameterResult;
        }

        //基于弯曲的Morphing方法
        public C5.LinkedList<CCorrSegment> DWByRIBSMorphing(CParameterInitialize pParameterInitialize, CParameterThreshold ParameterThreshold)
        {
            //CParameterInitialize ParameterInitialize = pParameterInitialize;
            //CMPBBSL OptMPBBSL = new CMPBBSL();

            //CPolyline frcpl = _FromCpl;
            //CPolyline tocpl = _ToCpl;
            //double dblVerySmall = ParameterThreshold.dblVerySmall;


            ////*******为什么不调用"OptMPBBSL.DWByMPBBSL" ?
            ////*******因为这里的弯曲匹配方法(独立弯曲、子弯曲)不一样，增加了新的重建三角网的内容

            //List<CPoint> frchcptlt = _Triangulator.CreateConvexHullEdgeLt2(frcpl, dblVerySmall);
            //CPolyline frchcpl = new CPolyline(0, frchcptlt);    //大比例尺折线外包多边形线段

            //List<CPoint> tochcptlt = _Triangulator.CreateConvexHullEdgeLt2(tocpl, dblVerySmall);
            //CPolyline tochcpl = new CPolyline(0, tochcptlt);    //小比例尺折线外包多边形线段

            ////添加约束数据生成图层，以便于利用AE中的功能(ct:constraint)
            //List<CPolyline> frctcpllt = new List<CPolyline>(); frctcpllt.Add(frcpl); frctcpllt.Add(frchcpl);
            //List<CPolyline> toctcpllt = new List<CPolyline>(); toctcpllt.Add(tocpl); toctcpllt.Add(tochcpl);
            //IFeatureLayer pBSFLayer = CHelpFunc.SaveCPlLt(frctcpllt, "frctcpllt" + _dblCDTNum, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //IFeatureLayer pSSFLayer = CHelpFunc.SaveCPlLt(toctcpllt, "toctcpllt" + _dblCDTNum, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            ////建立CDT并获取弯曲森林
            //CBendForest FromLeftBendForest = new CBendForest();
            //CBendForest FromRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableFrom = new CParameterVariable(frcpl, "FromCDT" + _dblCDTNum, ParameterInitialize.pBSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableFrom, ref FromLeftBendForest, ref FromRightBendForest, ParameterInitialize);

            //CBendForest ToLeftBendForest = new CBendForest();
            //CBendForest ToRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableTo = new CParameterVariable(tocpl, "ToCDT" + _dblCDTNum, ParameterInitialize.pSSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableTo, ref ToLeftBendForest, ref ToRightBendForest, ParameterInitialize);

            //_dblCDTNum = _dblCDTNum + 1;



            ////弯曲树匹配，寻找对应独立弯曲
            //List<CCorrespondBend> IndependCorrespondBendLt = new List<CCorrespondBend>();
            //IndependCorrespondBendLt.AddRange(BendTreeMatch(FromLeftBendForest, ToLeftBendForest, ParameterThreshold, ParameterInitialize));
            //IndependCorrespondBendLt.AddRange(BendTreeMatch(FromRightBendForest, ToRightBendForest, ParameterThreshold, ParameterInitialize));

            ////弯曲匹配，寻找对应弯曲
            //List<CCorrespondBend> CorrespondBendLt = BendMatch(IndependCorrespondBendLt, ParameterThreshold, ParameterInitialize);

            ////提取对应线段
            //C5.LinkedList<CCorrSegment> pComplexCorrespondSegmentLk = CHelpFunc.DetectCorrespondSegment(frcpl, tocpl, CorrespondBendLt);

            //return pComplexCorrespondSegmentLk;
            return null;
        }



        /// <summary>
        /// 进行弯曲匹配，提取对应线段
        /// </summary>
        /// <param name="CFromBendForest">大比例尺弯曲森林</param>
        /// <param name="CToBendForest">小比例尺弯曲森林</param>
        /// <param name="ParameterThreshold">参数容器</param>
        /// <param name="CorrespondSegmentLk">对应线段列</param>
        /// <remarks>弯曲树匹配和孩子弯曲匹配都由本函数来执行
        /// 注意：此处的弯曲树匹配及弯曲匹配与"MPBBSL"中有所不同，此处增加了递归识别独立弯曲</remarks>
        public List<CCorrespondBend> BendTreeMatch(CBendForest CFromBendForest, CBendForest CToBendForest, CParameterThreshold ParameterThreshold,CParameterInitialize pParameterInitialize)
        {
            //大比例尺独立弯曲
            SortedDictionary<double, CBend> pFromIndependBendSlt = new SortedDictionary<double, CBend>(new CCmpDbl());
            for (int i = 0; i < CFromBendForest.Count; i++)
            {
                pFromIndependBendSlt.Add(CFromBendForest.ElementAt(i).Value.dblStartRL, CFromBendForest.ElementAt(i).Value);
            }
            //小比例尺独立弯曲
            SortedDictionary<double, CBend> pToIndependBendSlt = new SortedDictionary<double, CBend>(new CCmpDbl());
            for (int i = 0; i < CToBendForest.Count; i++)
            {
                pToIndependBendSlt.Add(CToBendForest.ElementAt(i).Value.dblStartRL, CToBendForest.ElementAt(i).Value);
            }

            //获取对应弯曲
            List<CCorrespondBend> pIndependCorrespondBendLt = IndependBendMatch(pFromIndependBendSlt, pToIndependBendSlt, ParameterThreshold,pParameterInitialize);

            return pIndependCorrespondBendLt;
        }




        /// <summary>
        /// 弯曲匹配
        /// </summary>
        /// <param name="pFromBendSlt">大比例尺弯曲列表</param>
        /// <param name="pToBendSlt">小比例尺弯曲列表</param>
        /// <param name="dblRatioBound">比例阈值</param>
        /// <returns>对应弯曲列表</returns>
        /// <remarks></remarks>
        private List<CCorrespondBend> IndependBendMatch(SortedDictionary<double, CBend> pFromBendSlt, SortedDictionary<double, CBend> pToBendSlt,
                                                        CParameterThreshold ParameterThreshold, CParameterInitialize pParameterInitialize)
        {
            //清理弯曲的对应弯曲列表
            for (int i = 0; i < pFromBendSlt.Count; i++)
            {
                pFromBendSlt.ElementAt (i).Value.pCorrespondBendLt.Clear();
            }
            for (int i = 0; i < pToBendSlt.Count; i++)
            {
                pToBendSlt.ElementAt (i).Value.pCorrespondBendLt.Clear();
            }


            //搜索符合要求的对应弯曲
            List<CCorrespondBend> pCorrespondBendLt = new List<CCorrespondBend>();
            int intLastMatchj = 0;   //该值并不作精确要求，仅为中间搜索点的估算值
            for (int i = 0; i < pFromBendSlt.Values.Count; i++)
            {
                CBend pfrbend = pFromBendSlt.ElementAt (i).Value;
                double dblRatioLengthi = pfrbend.Length / ParameterThreshold.dblFrLength;

                //int dblTempMatchj = 0;
                //为了节省时间，从中间向两边搜索
                //以intLastMatchj为基准前进搜索
                for (int j = intLastMatchj; j < pToBendSlt.Values.Count; j++)
                {
                    CBend ptobend = pToBendSlt.ElementAt (i).Value;
                    double dblRatioLengthj = ptobend.Length / ParameterThreshold.dblToLength;
                    double dblStartDiff = pfrbend.dblStartRL - ptobend.dblStartRL;
                    double dblEndDiff = pfrbend.dblEndRL - ptobend.dblEndRL;
                    double dblAngleDiff = pfrbend.pBaseLine.Angle - ptobend.pBaseLine.Angle;

                    //计算相对位置差阈值
                    double dblRatioBoundj;
                    if (dblRatioLengthi >= dblRatioLengthj)
                    {
                        dblRatioBoundj = 0.25 * dblRatioLengthi;
                    }
                    else
                    {
                        dblRatioBoundj = 0.25 * dblRatioLengthj;
                    }

                    if (dblStartDiff < (-dblRatioBoundj))
                    {
                        break; //如果已经超出一定范围，则没必要再向后搜索了
                    }

                    double dblLengthRatio = pfrbend.pBaseLine.Length / ptobend.pBaseLine.Length;

                    if ((Math.Abs(dblStartDiff) <= dblRatioBoundj) && (Math.Abs(dblEndDiff) <= dblRatioBoundj) && (Math.Abs(dblAngleDiff) <= ParameterThreshold.dblAngleBound)
                        && (dblLengthRatio >= ParameterThreshold.dblDLengthBound) && (dblLengthRatio <= ParameterThreshold.dblULengthBound))
                    {
                        CCorrespondBend pCorrespondBend = new CCorrespondBend(pFromBendSlt.ElementAt(i).Value, pToBendSlt.ElementAt(i).Value);
                        pCorrespondBendLt.Add(pCorrespondBend);

                        //*******************************//
                        List<CCorrespondBend> pRIBSCorrespondBendLt = RBS(pCorrespondBend, ParameterThreshold, pParameterInitialize);  
                        pCorrespondBendLt.AddRange(pRIBSCorrespondBendLt);

                        intLastMatchj = j;
                    }
                }
            }

            return pCorrespondBendLt;
        }

        public List<CCorrespondBend> BendMatch(List<CCorrespondBend> pIndependCorrespondBendLt, CParameterThreshold ParameterThreshold, CParameterInitialize pParameterInitialize)
        {
            List<CCorrespondBend> pCorrespondBendLt = new List<CCorrespondBend>();
            pCorrespondBendLt.AddRange(pIndependCorrespondBendLt);
            for (int i = 0; i < pIndependCorrespondBendLt.Count; i++)
            {
                RecursiveBendMatch(pIndependCorrespondBendLt[i].CFromBend, pIndependCorrespondBendLt[i].CToBend, ref pCorrespondBendLt, ParameterThreshold, pParameterInitialize);
            }
            return pCorrespondBendLt;

        }

        private void RecursiveBendMatch(CBend pFromBend, CBend pToBend, ref List<CCorrespondBend> pCorrespondBendLt, 
                                        CParameterThreshold ParameterThreshold, CParameterInitialize pParameterInitialize)
        {
            if (pFromBend.CLeftBend == null || pToBend.CLeftBend == null)
            {
                return;
            }

            double dblRatioLL = pFromBend.CLeftBend.pBaseLine.Length / pToBend.CLeftBend.pBaseLine.Length;
            double dblRatioRR = pFromBend.CRightBend.pBaseLine.Length / pToBend.CRightBend.pBaseLine.Length;


            MessageBox.Show("CRIBS: need to be improved!");
            double dblAngleDiffLL = pFromBend.CLeftBend.pBaseLine.Angle - pToBend.CLeftBend.pBaseLine.Angle;
            double dblAngleDiffRR = pFromBend.CRightBend.pBaseLine.Angle - pToBend.CRightBend.pBaseLine.Angle;

            if ((dblRatioLL >= ParameterThreshold.dblDLengthBound) && (dblRatioLL <= ParameterThreshold.dblULengthBound) &&
                (dblRatioRR >= ParameterThreshold.dblDLengthBound) && (dblRatioRR <= ParameterThreshold.dblULengthBound) &&
                (Math.Abs(dblAngleDiffLL) <= ParameterThreshold.dblAngleBound) && (dblAngleDiffRR <= ParameterThreshold.dblULengthBound))
            {
                //左右弯曲分别对应
                CCorrespondBend pLeftCorrespondBend = new CCorrespondBend(pFromBend.CLeftBend, pToBend.CLeftBend);
                CCorrespondBend pRightCorrespondBend = new CCorrespondBend(pFromBend.CRightBend, pToBend.CRightBend);
                pCorrespondBendLt.Add(pLeftCorrespondBend);
                pCorrespondBendLt.Add(pRightCorrespondBend);

                //*******************************//
                List<CCorrespondBend> pRIBSLeftCorrespondBendLt = RBS(pLeftCorrespondBend, ParameterThreshold, pParameterInitialize);
                pCorrespondBendLt.AddRange(pRIBSLeftCorrespondBendLt);
                List<CCorrespondBend> pRIBSRightCorrespondBendLt = RBS(pRightCorrespondBend, ParameterThreshold, pParameterInitialize);
                pCorrespondBendLt.AddRange(pRIBSRightCorrespondBendLt);

                //继续往下遍历
                RecursiveBendMatch(pFromBend.CLeftBend, pToBend.CLeftBend, ref pCorrespondBendLt, ParameterThreshold, pParameterInitialize);
                RecursiveBendMatch(pFromBend.CRightBend, pToBend.CRightBend, ref pCorrespondBendLt, ParameterThreshold, pParameterInitialize);
            }
            else  //此步必不可少，作用还是很明显的
            {
                double dblRatioLR = pFromBend.CLeftBend.pBaseLine.Length / pToBend.CRightBend.pBaseLine.Length;
                double dblRatioRL = pFromBend.CRightBend.pBaseLine.Length / pToBend.CLeftBend.pBaseLine.Length;


                if ((dblRatioLL >= ParameterThreshold.dblDLengthBound) && (dblRatioLR >= ParameterThreshold.dblDLengthBound))
                {
                    RecursiveBendMatch(pFromBend.CLeftBend, pToBend, ref pCorrespondBendLt, ParameterThreshold, pParameterInitialize);
                }
                if ((dblRatioRL >= ParameterThreshold.dblDLengthBound) && (dblRatioRR >= ParameterThreshold.dblDLengthBound))
                {
                    RecursiveBendMatch(pFromBend.CRightBend, pToBend, ref pCorrespondBendLt, ParameterThreshold, pParameterInitialize);
                }
            }
        }

        /// <summary>
        /// 递归调用弯曲结构
        /// </summary>
        /// <param name="CCorrespondBend">对应弯曲</param>
        /// <param name="ParameterThreshold">阈值参数</param>
        /// <param name="pRightBendForest">折线右边的弯曲森林</param>
        /// <param name="strName">留作保存三角网用</param>
        /// <remarks></remarks>
        public List<CCorrespondBend> RBS(CCorrespondBend pCorrespondBend, CParameterThreshold ParameterThreshold, CParameterInitialize pParameterInitialize)
        {
            string strSide = pCorrespondBend.CFromBend.strSide;
            CPolyline subfrcpl = new CPolyline(0,pCorrespondBend.CFromBend.CptLt);
            CPolyline subtocpl = new CPolyline(0,pCorrespondBend.CToBend.CptLt);

            List<CPoint> subfrchcptlt = _Triangulator.CreateConvexHullEdgeLt2(subfrcpl, CConstants.dblVerySmallCoord);
            CPolyline subfrchcpl = new CPolyline(0, subfrchcptlt);    //大比例尺折线外包多边形线段

            List<CPoint> subtochcptlt = _Triangulator.CreateConvexHullEdgeLt2(subtocpl, CConstants.dblVerySmallCoord);
            CPolyline subtochcpl = new CPolyline(0, subtochcptlt);    //小比例尺折线外包多边形线段

            //添加数据生成图层，以便于利用AE中的功能
            List<CPolyline> subfrcpllt = new List<CPolyline>(); subfrcpllt.Add(subfrcpl); subfrcpllt.Add(subfrchcpl);
            List<CPolyline> subtocpllt = new List<CPolyline>(); subtocpllt.Add(subtocpl); subtocpllt.Add(subtochcpl);
            IFeatureLayer pBSFLayer = CHelpFunc.SaveCPlLt(subfrcpllt, "subfrcpl" + _dblCDTNum, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);
            IFeatureLayer pSSFLayer = CHelpFunc.SaveCPlLt(subtocpllt, "subtocpl" + _dblCDTNum, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);

            CParameterVariable pParameterVariableFrom = new CParameterVariable(subfrcpl, "subFromCDT" + _dblCDTNum, pBSFLayer, CConstants.dblVerySmallCoord);
            CParameterVariable pParameterVariableTo = new CParameterVariable(subtocpl, "subToCDT" + _dblCDTNum, pSSFLayer, CConstants.dblVerySmallCoord);

            _dblCDTNum = _dblCDTNum + 1;

            CMPBBSL OptMPBBSL = new CMPBBSL();

            //建立CDT并获取弯曲森林
            CBendForest FromBendForest = new CBendForest();
            GetSideBendForest(pParameterVariableFrom, ref FromBendForest, pParameterInitialize, strSide);

            CBendForest ToBendForest = new CBendForest();
            GetSideBendForest(pParameterVariableTo, ref ToBendForest, pParameterInitialize, strSide);            

            //弯曲树匹配，寻找对应独立弯曲
            List<CCorrespondBend> IndependCorrespondBendLt = BendTreeMatch(FromBendForest, ToBendForest, ParameterThreshold, pParameterInitialize);

            //弯曲匹配，寻找对应弯曲
            List<CCorrespondBend> CorrespondBendLt = BendMatch(IndependCorrespondBendLt, ParameterThreshold, pParameterInitialize);

            return CorrespondBendLt;
        }

        /// <summary>
        /// 获取折线某一边的弯曲森林
        /// </summary>
        /// <param name="cpl">折线</param>
        /// <param name="pParameterVariable">变量参数：包含线状要素，需建立CDT的图层，CDT被保存的文件名称</param>
        /// <param name="pBendForest">待建立的一边的弯曲森林</param>
        /// <param name="pParameterInitialize">参数</param>
        /// <param name="strForeSide">弯曲在线状要素的侧边(右侧或左侧)，此处将建立另外一侧的弯曲森林</param>
        /// <remarks>注意：本方法中的CreateCDT将会改变pParameterVariable中的CPolyline</remarks>
        public void GetSideBendForest(CParameterVariable pParameterVariable, ref CBendForest pBendForest, CParameterInitialize pParameterInitialize, string strForeSide)
        {
            CTriangulator OptCDT = new CTriangulator();
            CPolyline newcpl = new CPolyline(pParameterVariable.CPolyline.ID, pParameterVariable.CPolyline.CptLt);
            List<CTriangle> CDTLt = OptCDT.CreateCDT(pParameterVariable.pFeatureLayer, ref newcpl, pParameterVariable.dblVerySmall);
            pParameterVariable.CPolyline = newcpl;

            //for (int i = 0; i < CDTLt.Count; i++) CDTLt[i].TID = i;  //到此为止，约束三角形建立完成，各三角形不再发生变化，将各三角形编号           
            OptCDT.GetSETriangle(ref CDTLt, pParameterVariable.dblVerySmall);  //确定共边三角形
            OptCDT.ConfirmTriangleSide(ref CDTLt, pParameterVariable.CPolyline, pParameterVariable.dblVerySmall); //确定各三角形位于折线的左右边
            OptCDT.SignTriTypeAll(ref CDTLt);   //标记I、II、III、VI类三角形

            if (strForeSide == "Left")
            {
                pBendForest = OptCDT.BuildBendForestNeed2(ref CDTLt, pParameterVariable.CPolyline.CptLt, "Right", pParameterVariable.dblVerySmall);
            }
            else if (strForeSide == "Right")
            {
                pBendForest = OptCDT.BuildBendForestNeed2(ref CDTLt, pParameterVariable.CPolyline.CptLt, "Left",pParameterVariable.dblVerySmall);
            }
            else
            {
                MessageBox.Show("GetSideBendForest出现问题！");
            }

            //保存三角网
            //List<CTriangle> CTriangleLt = new List<CTriangle>();
            //for (int i = 0; i < CDTLt.Count; i++)
            //{
            //    if (CDTLt[i].strTriType != "I")
            //    {
            //        CTriangleLt.Add(CDTLt[i]);
            //    }
            //}
            //CHelpFunc.SaveTriangles(CTriangleLt, pParameterVariable.strName, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);
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
