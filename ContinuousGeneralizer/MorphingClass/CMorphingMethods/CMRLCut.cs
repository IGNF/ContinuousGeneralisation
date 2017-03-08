using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using MorphingClass.CEntity;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;

namespace MorphingClass.CMorphingMethods
{
    /// <summary>CMRLCut</summary>
    /// <remarks>必须保证所有线段的方向都是从河流的上游指向河流的下游（也可写出更智能的代码进行处理）</remarks>
    public class CMRLCut
    {
        
        
        private CMRL _OptMRL = new CMRL();
        private CParameterResult _ParameterResult;

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CParameterInitialize _ParameterInitialize;

        public CMRLCut()
        {

        }

        public CMRLCut(CParameterInitialize ParameterInitialize)
        {

            //获取当前选择的点要素图层
            //大比例尺要素图层
            IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLargerScaleLayer.SelectedIndex);
                                                                       
            //小比例尺要素图层
            IFeatureLayer pSSFLayer =(IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboSmallerScaleLayer.SelectedIndex);
                                                           


            ParameterInitialize.pBSFLayer = pBSFLayer;
            ParameterInitialize.pSSFLayer = pSSFLayer;
            ParameterInitialize.dblOverlapRatio = Convert.ToDouble(ParameterInitialize.txtOverlapRatio.Text);
            _ParameterInitialize = ParameterInitialize;

            //获取线数组
            _LSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pBSFLayer);
            _SSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pSSFLayer);
        }


        public void MRLCutMorphing()
        {
            //long lngStartTime1 = 0;
            //long lngEndTime1 = 0;

            //List<CPolyline> LSCPlLt = _LSCPlLt;
            //List<CPolyline> SSCPlLt = _SSCPlLt;

            //CParameterInitialize pParameterInitialize = _ParameterInitialize;
            //CParameterThreshold pParameterThreshold = new CParameterThreshold();

            //lngStartTime1 = System.Environment.TickCount;
            //pParameterThreshold.dblBuffer = CHelperFunction.CalBuffer(LSCPlLt, SSCPlLt);      //计算缓冲区半径大小
            //lngEndTime1 = System.Environment.TickCount;
            //long lngTime1 = lngEndTime1 - lngStartTime1;

            //pParameterThreshold.dblVerySmall = CGeometricMethods.CalVerySmall(LSCPlLt[0]);         //计算极小值
            //pParameterThreshold.dblOverlapRatio = pParameterInitialize.dblOverlapRatio;

            //long lngTime2=0;
            //long lngTime3 = 0;
            //List<CRiverNet> CBSRiverNetLt = _OptMRL.BuildRiverNetLt(LSCPlLt, pParameterThreshold, ref lngTime2);     //依据线数据建立河网
            //List<CRiverNet> CSSRiverNetLt = _OptMRL.BuildRiverNetLt(SSCPlLt, pParameterThreshold, ref lngTime3);     //依据线数据建立河网


            //long lngStartTime4 = System.Environment.TickCount;
            //CalLengthSum(CBSRiverNetLt);   //计算河网中每一条河流的长度总和
            //CalLengthSum(CSSRiverNetLt);   //计算河网中每一条河流的长度总和

            ////CHelperFunction.SaveBuffers(CBSRiverNetLt, CSSRiverNetLt, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);  //保存Buffer


            //List<CCorrespondRiverNet> pCorrespondRiverNetLt =_OptMRL.FindCorrespondRiverNetLt(CBSRiverNetLt, CSSRiverNetLt, pParameterThreshold);  //找对应河网

            //List<List<CPoint>> CResultPtLtLt = new List<List<CPoint>>();
            //for (int i = 0; i < pCorrespondRiverNetLt.Count; i++)
            //{
            //    _OptMRL.FindCorrespondRiverLt(pCorrespondRiverNetLt[i], pParameterThreshold);  //找到对应河流，对应河流的数据记录在对应河网列中
            //    BuildCorrespondence(pCorrespondRiverNetLt[i], pParameterThreshold);            //建立有对应河流的Morphing关系
            //    CResultPtLtLt.AddRange(pCorrespondRiverNetLt[i].CResultPtLtLt);
            //}

            ////处理没有对应河流的Morphing关系(以切割时刻为指导)
            //CalCutTime(CBSRiverNetLt);
            //long lngEndTime4 = System.Environment.TickCount;
            //long lngTime4 = lngEndTime4 - lngStartTime4;

            ////计算并显示运行时间
            //long lngTimeSum = lngTime1 + lngTime2 + lngTime3 + lngTime4;
            //_ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTimeSum) + "ms";  //显示运行时间

            ////保存对应线
            //CHelperFunction.SaveCorrespondLine(CResultPtLtLt, "MRLCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.CResultCorrespondRiverNetLt = pCorrespondRiverNetLt;
            //ParameterResult.CResultPtLtLt = CResultPtLtLt;
            //ParameterResult.lngTime = lngTimeSum;
            //_ParameterResult = ParameterResult;
        }

        /// <summary>计算河网中每一条河流的长度总和</summary>
        /// <param name="pRiverNetLt">河网列</param>
        /// <remarks>河流的长度总和：该河流与其各层支流的长度之和 </remarks>
        private void CalLengthSum(List<CRiverNet> pRiverNetLt)
        {
            for (int i = 0; i < pRiverNetLt.Count ; i++)
            {
                RecursiveCalLengthSum(pRiverNetLt[i].CMasterStream);
            }
        }

        /// <summary>递归计算该河流的长度总和(该河流及其所有支流的长度之和)</summary>
        /// <param name="pRiver">当前河流</param>
        private void RecursiveCalLengthSum(CRiver pRiver)
        {
            pRiver.LengthSum = pRiver.pPolyline.Length;
            if (pRiver.CTributaryLt.Count > 0)
            {                
                for (int i = 0; i < pRiver.CTributaryLt.Count; i++)
                {
                    RecursiveCalLengthSum(pRiver.CTributaryLt[i]);          //先计算该支流的长度总和，再计算当前河流的长度总和
                    pRiver.LengthSum =pRiver.LengthSum+ pRiver.CTributaryLt[i].LengthSum;
                }
            }
        }




        /// <summary>建立河网间的对应河流的对应特征点关系（不包括没有对应河流的情况）</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <remarks>判断方法：相交缓冲区多边形面积的两倍除以两缓冲区面积之和</remarks>
        private void BuildCorrespondence(CCorrespondRiverNet pCorrespondRiverNet, CParameterThreshold pParameterThreshold)
        {
            CRiverNet pBSRiverNet = pCorrespondRiverNet.CBSRiverNet;
            CRiverNet pSSRiverNet = pCorrespondRiverNet.CSSRiverNet;
            pCorrespondRiverNet.CResultPtLtLt = new List<List<CPoint>>();

            double dblLengthSumRatio = pBSRiverNet.CMasterStream.pPolyline.Length / pSSRiverNet.CMasterStream.pPolyline.Length;
            pParameterThreshold.dblLengthSumRatio = dblLengthSumRatio;
            //注意：不管主干流存不存在对应河流，都从此处开始，因为不存在对应河流则在此函数中自动不作处理
            RecursiveDWExistCorrCut(pCorrespondRiverNet, pParameterThreshold, pBSRiverNet.CMasterStream);
        }

        /// <summary>处理上一级河流有对应河流的情况</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <remarks>RecursiveDWExistCorr：RecursiveDealWithExistCorrepondenceRiver</remarks>
        private void RecursiveDWExistCorrCut(CCorrespondRiverNet pCorrespondRiverNet, CParameterThreshold pParameterThreshold, CRiver pBSRiver)
        {
            if (pBSRiver.CCorrRiver != null)
            {
                _OptMRL.DWExistCorr(pCorrespondRiverNet, pParameterThreshold, pBSRiver);

                //处理当前河流的支流
                for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
                {
                    RecursiveDWExistCorrCut(pCorrespondRiverNet, pParameterThreshold, pBSRiver.CTributaryLt[i]);
                }
            }
        }



        /// <summary>计算切割时刻</summary>
        /// <param name="CBSRiverNetLt">大比例尺表达河网列</param>
        /// <remarks></remarks>
        private void CalCutTime(List<CRiverNet> CBSRiverNetLt)
        {
            SortedList<double, CRiver> dblDataSLt = new SortedList<double, CRiver>(new CCompareDbl());
            //添加大比例尺表达图层中(包括各河网)的所有没有对应河流的河流
            for (int i = 0; i < CBSRiverNetLt.Count; i++)
            {
                for (int j = 0; j < CBSRiverNetLt[i].CRiverLt.Count; j++)
                {
                    if (CBSRiverNetLt[i].CRiverLt[j].CCorrRiver == null)  //有对应河流
                    {
                        dblDataSLt.Add(CBSRiverNetLt[i].CRiverLt[j].LengthSum, CBSRiverNetLt[i].CRiverLt[j]);
                    }
                }
            }

            int intCount = dblDataSLt.Count;
            double dblInterval = (dblDataSLt.Keys[intCount - 1] - dblDataSLt.Keys[0]) / (intCount - 1);
            double dblDCutBound = dblDataSLt.Keys[0] - dblInterval;
            double dblUCutBound = dblDataSLt.Keys[intCount - 1] + dblInterval;
            double dblDis = dblUCutBound - dblDCutBound;
            for (int i = 0; i < dblDataSLt.Count; i++)
            {
                CRiver pRiver = dblDataSLt.Values[i];
                pRiver.dblCutTime = (pRiver.LengthSum - dblDCutBound) / dblDis;
            }
        }



        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
