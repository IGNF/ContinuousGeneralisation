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
using MorphingClass.CEvaluationMethods;

namespace MorphingClass.CMorphingMethods
{
    /// <summary>CMRL</summary>
    /// <remarks>必须保证所有线段的方向都是从河流的上游指向河流的下游（也可写出更智能的代码进行处理）
    ///          结果存储：
    ///                   河流的对应特征点结果存储在它们自己的“pBSRiver.CResultPtLt中”；
    ///               此外，pCorrespondRiverNet.CResultPtLtLt则存储了对应河网中各河流的对应特征点结果；
    ///               最终，ParameterResult.CResultPtLtLt存储了各对应河网中各河流的对应特征点结果。
    ///</remarks>
    public class CMRL
    {
        
        
        private CParameterResult _ParameterResult;

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CParameterInitialize _ParameterInitialize;

        public CMRL()
        {

        }

        public CMRL(CParameterInitialize ParameterInitialize)
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
            _LSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pBSFLayer);
            _SSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSSFLayer);
        }

        public void MRLMorphing()
        {
            
            //List<CPolyline> LSCPlLt = _LSCPlLt;
            //List<CPolyline> SSCPlLt = _SSCPlLt;

            //CParameterInitialize pParameterInitialize = _ParameterInitialize;
            //CParameterThreshold pParameterThreshold = new CParameterThreshold();
            //pParameterThreshold.dblBuffer =CHelpFunc.CalBuffer(LSCPlLt, SSCPlLt);                      //计算缓冲区半径大小
            //pParameterThreshold.dblVerySmall = pParameterThreshold.dblBuffer/200;
            ////pParameterThreshold.dblVerySmall = CGeoFunc.CalVerySmall(LSCPlLt);         //计算极小值
            //pParameterThreshold.dblOverlapRatio = pParameterInitialize.dblOverlapRatio;

            //long lngTime1=0;
            //long lngTime2=0;

            //List<CRiverNet> CBSRiverNetLt = BuildRiverNetLt(LSCPlLt, pParameterThreshold, ref lngTime1);     //依据线数据建立河网
            //List<CRiverNet> CSSRiverNetLt = BuildRiverNetLt(SSCPlLt, pParameterThreshold, ref lngTime2);     //依据线数据建立河网

            //CHelpFunc.SaveBuffers(CBSRiverNetLt, CSSRiverNetLt, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);  //保存Buffer


            //List<CCorrespondRiverNet> pCorrespondRiverNetLt = FindCorrespondRiverNetLt(CBSRiverNetLt, CSSRiverNetLt, pParameterThreshold);  //找对应河网

            //List<List<CPoint>> CResultPtLtLt = new List<List<CPoint>>();
            //List<CRiver> CResultRiverLt = new List<CRiver>();
            //for (int i = 0; i < pCorrespondRiverNetLt.Count; i++)
            //{
                
            //    FindCorrespondRiverLt(pCorrespondRiverNetLt[i], pParameterThreshold);  //找到对应河流，对应河流的数据记录在对应河网列中
            //    BuildCorrespondence(pCorrespondRiverNetLt[i], pParameterThreshold);                         //建立Morphing关系
            //    CResultPtLtLt.AddRange(pCorrespondRiverNetLt[i].CResultPtLtLt);
            //    //CResultRiverLt.AddRange(pCorrespondRiverNetLt[i].CResultRiverLt);
            //}

            
            ////保存对应线
            //CHelpFunc.SaveCorrespondLine(CResultPtLtLt, "MRLCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.CResultCorrespondRiverNetLt = pCorrespondRiverNetLt;
            //ParameterResult.CResultPtLtLt = CResultPtLtLt;
            ////ParameterResult.CResultRiverLt = CResultRiverLt;
            //_ParameterResult = ParameterResult;




            ////CMathStatistic.KillProcess();


        }

        #region 依据线数据建立河网列(包括建立干支关系，添加各支流成员)
        /// <summary>依据线数据建立河网列(包括建立干支关系，添加各支流成员)</summary>
        /// <param name="CPlLt">线数据</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <returns>河网列</returns>
        public List<CRiverNet> BuildRiverNetLt(List<CPolyline> CPlLt, CParameterThreshold pParameterThreshold,ref long lngTime)
        {
            double dblVerySmall = CConstants.dblVerySmallCoord;
            double dblBuffer = pParameterThreshold.dblBuffer;

            //根据线数据生成河流数据
            long lngStartTime = System.Environment.TickCount;
            List<CRiver> CAllRiverLt = new List<CRiver>();
            for (int i = 0; i < CPlLt.Count; i++)
            {
                CRiver pRiver = new CRiver(i, CPlLt[i],dblBuffer, dblVerySmall);
                CAllRiverLt.Add(pRiver);
            }
            long lngEndTime = System.Environment.TickCount;
            lngTime = lngEndTime - lngStartTime;

            CreateRiverRelationship(ref CAllRiverLt);   //创建各河流间的干支关系
            List<CRiverNet> pRiverNetLt = CreateRiverNetLt(CAllRiverLt, dblVerySmall); // 创建河网
            return pRiverNetLt;
        }

        /// <summary>创建各河流间的干支关系</summary>
        /// <param name="CAllRiverLt">河流数据</param>
        private void CreateRiverRelationship(ref List<CRiver> CAllRiverLt)
        {
            //创建各河流间的干支关系
            for (int i = 0; i < CAllRiverLt.Count - 1; i++)
            {
                IRelationalOperator pSmallBufferRel = CAllRiverLt[i].pSmallBufferGeo as IRelationalOperator;
                IRelationalOperator pToptSmallBufferGeo = CAllRiverLt[i].pToptSmallBufferGeo as IRelationalOperator;
                for (int j = i + 1; j < CAllRiverLt.Count; j++)
                {
                    bool isDisjoint = pSmallBufferRel.Disjoint(CAllRiverLt[j].pPolyline);
                    if (isDisjoint == false)
                    {
                        if (pToptSmallBufferGeo.Disjoint(CAllRiverLt[j].pToptSmallBufferGeo) == false) //如果两条河流的终点重合，则互相之间不存在干支关系
                        {
                            continue;
                        }

                        if (CAllRiverLt[i].CMainStream != null) //如果"河流i"有了干河流，则"河流j"必然是它的支流
                        {
                            CAllRiverLt[j].CMainStream = CAllRiverLt[i];
                            CAllRiverLt[i].CTributaryLt.Add(CAllRiverLt[j]);
                        }
                        else if (CAllRiverLt[j].CMainStream != null) //如果"河流j"有了干河流，则"河流i"必然是它的支流
                        {
                            CAllRiverLt[i].CMainStream = CAllRiverLt[j];
                            CAllRiverLt[j].CTributaryLt.Add(CAllRiverLt[i]);
                        }
                        else
                        {
                            bool isDisjoint2 = pSmallBufferRel.Disjoint(CAllRiverLt[j].pPolyline.ToPoint);
                            if (isDisjoint2 == false)
                            {
                                CAllRiverLt[j].CMainStream = CAllRiverLt[i];
                                CAllRiverLt[i].CTributaryLt.Add(CAllRiverLt[j]);
                            }
                            else
                            {
                                CAllRiverLt[i].CMainStream = CAllRiverLt[j];
                                CAllRiverLt[j].CTributaryLt.Add(CAllRiverLt[i]);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>创建河网，同时还记录了Tocpt2</summary>
        /// <param name="CAllRiverLt">河流数据</param>
        /// <returns>河网列</returns>
        private List<CRiverNet> CreateRiverNetLt(List<CRiver> CAllRiverLt,double dblVerySmall)
        {
            List<CRiver> CMasterRiverLt = new List<CRiver>();
            //找到各主干流
            for (int i = 0; i < CAllRiverLt.Count; i++)
            {
                if (CAllRiverLt[i].CMainStream == null)
                {
                    //没有干流的河流即为主干流
                    CMasterRiverLt.Add(CAllRiverLt[i]);
                }
            }

            //建立河网列
            List<CRiverNet> pRiverNetLt = new List<CRiverNet>();
            for (int i = 0; i < CMasterRiverLt.Count; i++)
            {
                CRiverNet pRiverNet = new CRiverNet(i, CMasterRiverLt[i]); //新建河网并添加主干流
                RecursiveFindTocpt2(CMasterRiverLt[i], dblVerySmall);      //找到当前河流与其各支流的相交点(属于当前河流)，并记录在支流河流数据中
                RecursiveSettleRiver(ref pRiverNet, CMasterRiverLt[i]);    //递归添加属于该河网的河流
                pRiverNetLt.Add(pRiverNet);
            }

            return pRiverNetLt;
        }

        /// <summary>找到当前河流与其各支流的相交点(属于当前河流)，并记录在支流河流数据中(顺便完成了支流记录从上游到下游的排序)</summary>
        /// <param name="pRiver">当前河流</param>
        /// <param name="dblVerySmall">一个非常小的值</param>
        /// <remarks>对于一条“其干流有对应河流”的河流，其变化（收缩、平移）时，由于其干流在不断移动，会造成脱节或相交，
        /// 因此要记录此处相交点，以方便后续处理</remarks>
        public void RecursiveFindTocpt2(CRiver pRiver, double dblVerySmall)
        {
            if (pRiver.CTributaryLt.Count == 0)
            {
                return;  //如果不存在支流，则直接返回
            }

            //数据准备
            List<CRiver> pTributaryLt = new List<CRiver>();
            pTributaryLt.AddRange(pRiver.CTributaryLt);

            //找到每条支流的终点"Tocpt"，并添加到数组pTributaryTocptLt
            List<CPoint> pTributaryTocptLt = new List<CPoint>();
            for (int i = 0; i < pTributaryLt.Count; i++)
            {
                pTributaryTocptLt.Add(pTributaryLt[i].CptLt[pTributaryLt[i].CptLt.Count - 1]);
            }

            //寻找各支流的Tocpt2
            List<CRiver> pNewTributaryLt = new List<CRiver>();
            for (int i = 0; i < pRiver.CptLt.Count; i++)
            {
                for (int j = pTributaryTocptLt.Count - 1; j >= 0; j--)  
                {
                    //为顾及“多叉路口”的情况，此处找到交点后应继续寻找(一个点可能对应多个支流)，因此if中没有放入"break"
                    if (pRiver.CptLt[i].Equals2D(pTributaryTocptLt[j]))
                    {
                        pTributaryLt[j].Tocpt2 = pRiver.CptLt[i];
                        pNewTributaryLt.Add(pTributaryLt[j]);  //从上游到下游记录支流
                        pTributaryLt.RemoveAt(j);       //为节省时间，找到即移除该河流
                        pTributaryTocptLt.RemoveAt(j);  //为节省时间，找到"Tocpt2"即移除该点
                    }
                }
            }
            pRiver.CTributaryLt = pNewTributaryLt;  //得到排好序的支流

            //依次对该河流的支流进行操作
            for (int i = 0; i < pRiver.CTributaryLt.Count; i++)
            {
                RecursiveFindTocpt2(pRiver.CTributaryLt[i], dblVerySmall);
            }
        }

        /// <summary>递归添加属于该河网的河流</summary>
        /// <param name="pRiverNet">河网</param>
        /// <param name="CurrentRiver">当前河流</param>
        private void RecursiveSettleRiver(ref CRiverNet pRiverNet, CRiver CurrentRiver)
        {
            if (CurrentRiver.CTributaryLt.Count == 0)
            {
                return;
            }

            for (int i = 0; i < CurrentRiver.CTributaryLt.Count; i++)
            {
                pRiverNet.CRiverLt.Add(CurrentRiver.CTributaryLt[i]);
                RecursiveSettleRiver(ref pRiverNet, CurrentRiver.CTributaryLt[i]);
            }
        }
        
        #endregion

        #region 找各种对应关系(包括对应河网、对应河流)
        /// <summary>找到对应河网</summary>
        /// <param name="CBSRiverNetLt">大比例尺表达河网</param>
        /// <param name="CSSRiverNetLt">小比例尺表达河网</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <returns>对应河网列</returns>
        /// <remarks></remarks>
        public List<CCorrespondRiverNet> FindCorrespondRiverNetLt(List<CRiverNet> CBSRiverNetLt, List<CRiverNet> CSSRiverNetLt, CParameterThreshold pParameterThreshold)
        {
            //数据准备
            List<CRiverNet> pBSRiverNetLt = new List<CRiverNet>();
            pBSRiverNetLt.AddRange(CBSRiverNetLt);
            List<CRiverNet> pSSRiverNetLt = new List<CRiverNet>();
            pSSRiverNetLt.AddRange(CSSRiverNetLt);

            //建立对应河网关系
            List<CCorrespondRiverNet> CCorrespondRiverNetLt = new List<CCorrespondRiverNet>();
            for (int i = 0; i < pBSRiverNetLt.Count; i++)
            {
                bool blnIsOverlap = false;
                //通过计算主干流是否重叠来判断两河网是否为同一河网的不同比例尺表达
                for (int j = 0; j < pSSRiverNetLt.Count; j++)
                {
                    blnIsOverlap = CGeoFunc.IsOverlap(pBSRiverNetLt[i].CMasterStream, pSSRiverNetLt[j].CMasterStream, pParameterThreshold.dblOverlapRatio);
                    if (blnIsOverlap == true)
                    {
                        //建立河网对应关系
                        pBSRiverNetLt[i].CCorrRiverNet = pSSRiverNetLt[j];
                        pSSRiverNetLt[j].CCorrRiverNet = pBSRiverNetLt[i];
                        CCorrespondRiverNet pCorrespondRiverNet = new CCorrespondRiverNet(pBSRiverNetLt[i], pSSRiverNetLt[j]);
                        pCorrespondRiverNet.blnCorr = true;
                        CCorrespondRiverNetLt.Add(pCorrespondRiverNet);
                        pSSRiverNetLt.RemoveAt(j);
                        break;
                    }
                }
                if (blnIsOverlap == false)
                {
                    pBSRiverNetLt[i].CCorrRiverNet = null;
                    CCorrespondRiverNet pCorrespondRiverNet = new CCorrespondRiverNet(pBSRiverNetLt[i], null);
                    pCorrespondRiverNet.blnCorr = false;
                    CCorrespondRiverNetLt.Add(pCorrespondRiverNet);
                }
            }
            return CCorrespondRiverNetLt;
        }

        /// <summary>对于一对对应河网，进一步找对应河流，同时记录对应支流对应交汇点</summary>
        /// <param name="CBSRiverNetLt">大比例尺表达河网</param>
        /// <param name="CSSRiverNetLt">小比例尺表达河网</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <returns>对应河网列</returns>
        /// <remarks>对应河流的数据记录在对应河网</remarks>
        public void FindCorrespondRiverLt(CCorrespondRiverNet pCorrespondRiverNet, CParameterThreshold pParameterThreshold)
        {
            List<CCorrespondRiver> pCorrespondRiverLt = new List<CCorrespondRiver>();  //仅记录成功匹配的河流对
            if (pCorrespondRiverNet.blnCorr == true)
            {
                //递归找到对应河流，同时记录对应支流对应交汇点
                RecursiveFindCorrespondRiverLt(ref pCorrespondRiverLt, pCorrespondRiverNet.CBSRiverNet.CMasterStream, pCorrespondRiverNet.CSSRiverNet.CMasterStream, pParameterThreshold);
            }
            pCorrespondRiverNet.CCorrespondRiverLt = pCorrespondRiverLt;
        }


        /// <summary>递归找到对应河流，同时记录对应支流对应交汇点</summary>
        /// <param name="CCorrespondRiverLt">对应河流记录列</param>
        /// <param name="pBSRiver">大比例尺表达河流</param>
        /// <param name="pSSRiver">小比例尺表达河流</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <remarks ></remarks>
        private void RecursiveFindCorrespondRiverLt(ref List<CCorrespondRiver> CCorrespondRiverLt,CRiver pBSRiver,CRiver pSSRiver,  CParameterThreshold pParameterThreshold)
        {
            //建立河流对应关系
            pBSRiver.CCorrRiver = pSSRiver;
            pSSRiver.CCorrRiver = pBSRiver;
            CCorrespondRiver pCorrespondRiver = new CCorrespondRiver(pSSRiver, pBSRiver);
            pCorrespondRiver.blnCorr = true;
            CCorrespondRiverLt.Add(pCorrespondRiver);

            //如果小比例尺表达河流分支不存在
            if (pSSRiver.CTributaryLt == null)
            {
                return;
            }

            //数据准备
            pBSRiver.CCorrTriJunctionPtLt = new List<CPoint>();
            pSSRiver.CCorrTriJunctionPtLt = new List<CPoint>();
            List<CRiver> pBSTributaryLt = pBSRiver.CTributaryLt;
            List<CRiver> pSSTributaryLt = new List<CRiver>();
            pSSTributaryLt.AddRange(pSSRiver.CTributaryLt);

            //遍历寻找对应河流
            for (int i = 0; i < pBSTributaryLt.Count; i++)
            {
                bool blnIsOverlap = false;
                for (int j = 0; j < pSSTributaryLt.Count; j++)
                {
                    blnIsOverlap = CGeoFunc.IsOverlap(pBSTributaryLt[i], pSSTributaryLt[j], pParameterThreshold.dblOverlapRatio);  //判断两河流是否重叠
                    if (blnIsOverlap == true)
                    {
                        pBSRiver.CCorrTriJunctionPtLt.Add(pBSTributaryLt[i].Tocpt2);
                        pSSRiver.CCorrTriJunctionPtLt.Add(pSSTributaryLt[j].Tocpt2);
                        RecursiveFindCorrespondRiverLt(ref CCorrespondRiverLt, pBSTributaryLt[i], pSSTributaryLt[j], pParameterThreshold); //递归找到对应河流
                        pSSTributaryLt.RemoveAt(j);
                        break;
                    }
                }
            }

            //记录对应交汇点的目的是有助于将原河流进行对应分割以提高精度
            //大多数情况下，交汇点都是“三叉路口”，此时数组"CCorrTriJunctionPtLt"中不会有重合点
            //但当交汇点为“四、五甚至更多叉路口”时，数组"CCorrTriJunctionPtLt"中有重合点，应只保留其中一个点
            for (int i = pBSRiver.CCorrTriJunctionPtLt.Count -1; i >0; i--)
            {
                if (pBSRiver.CCorrTriJunctionPtLt[i].Equals2D (pBSRiver.CCorrTriJunctionPtLt[i-1]))
                {
                    pBSRiver.CCorrTriJunctionPtLt.RemoveAt(i);
                    pSSRiver.CCorrTriJunctionPtLt.RemoveAt(i);
                }
            }


        }

        #endregion

        #region 建立河网间的对应特征点关系(包括没有对应河流的情况)
        /// <summary>建立河网间的对应特征点关系(包括没有对应河流的情况)</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <remarks>判断方法：相交缓冲区多边形面积的两倍除以两缓冲区面积之和</remarks>
        private void BuildCorrespondence(CCorrespondRiverNet pCorrespondRiverNet, CParameterThreshold pParameterThreshold)
        {
            CRiverNet pBSRiverNet = pCorrespondRiverNet.CBSRiverNet;
            CRiverNet pSSRiverNet = pCorrespondRiverNet.CSSRiverNet;
            pCorrespondRiverNet.CResultPtLtLt = new List<List<CPoint>>();
            //pCorrespondRiverNet.CResultRiverLt = new List<CRiver>();

            double dblLengthSumRatio = pBSRiverNet.CMasterStream.pPolyline.Length / pSSRiverNet.CMasterStream.pPolyline.Length;
            pParameterThreshold.dblLengthSumRatio = dblLengthSumRatio;
            //注意：不管主干流存不存在对应河流，都从此处开始，因为不存在对应河流则在此函数中自动转入处理不存在对应河流的函数
            RecursiveDWExistCorr(pCorrespondRiverNet, pParameterThreshold, pBSRiverNet.CMasterStream);
            
        }

        /// <summary>处理上一级河流有对应河流的情况</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="dblLengthSumRatio">小比例尺表达河流</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <remarks>RecursiveDWExistCorr：RecursiveDealWithExistCorrepondenceRiver
        /// 注意：不管主干流存不存在对应河流，都从此处开始，因为不存在对应河流则在此函数中自动转入处理不存在对应河流的函数</remarks>
        public void RecursiveDWExistCorr(CCorrespondRiverNet pCorrespondRiverNet, CParameterThreshold pParameterThreshold, CRiver pBSRiver)
        {
            if (pBSRiver.CCorrRiver != null)
            {
                DWExistCorr(pCorrespondRiverNet, pParameterThreshold, pBSRiver);
                //处理当前河流的支流
                for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
                {
                    RecursiveDWExistCorr(pCorrespondRiverNet, pParameterThreshold, pBSRiver.CTributaryLt[i]);
                }
            }
            else
            {
                pBSRiver.dblReductionRatio = 1;
                //pCorrespondRiverNet.CResultRiverLt.Add(pBSRiver);
                //处理当前河流的支流
                for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
                {
                    RecursiveDWNotExistCorr(pCorrespondRiverNet, pBSRiver.CTributaryLt[i], 1);
                }
            }

        }

        /// <summary>处理上一级河流有对应河流的情况</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="dblLengthSumRatio">小比例尺表达河流</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <remarks>DWExistCorr：DealWithExistCorrepondenceRiver
        /// 河流的对应特征点结果存储在它们自己的“pBSRiver.CResultPtLt中”；
        /// 此外pCorrespondRiverNet.CResultPtLtLt则存储了对应河网中各河流的对应特征点结果</remarks>
        public void DWExistCorr(CCorrespondRiverNet pCorrespondRiverNet, CParameterThreshold pParameterThreshold, CRiver pBSRiver)
        {
            //pBSRiver.CResultPtLt = new List<CPoint>();
            //CRiver pSSRiver = pBSRiver.CCorrRiver;
            //CMPBDP OptMPBDP = new CMPBDP();
            //if ((pBSRiver.CCorrTriJunctionPtLt != null) &&
            //    (pBSRiver.CCorrTriJunctionPtLt.Count != 0) &&
            //    (pBSRiver.CCorrTriJunctionPtLt.Count == pSSRiver.CCorrTriJunctionPtLt.Count))
            //{
            //    //数据准备
            //    List<CPoint> pBSCorrTriJunctionPtLt = new List<CPoint>();
            //    pBSCorrTriJunctionPtLt.Add(pBSRiver.CptLt[0]);
            //    pBSCorrTriJunctionPtLt.AddRange(pBSRiver.CCorrTriJunctionPtLt);
            //    pBSCorrTriJunctionPtLt.Add(pBSRiver.CptLt[pBSRiver.CptLt.Count - 1]);

            //    List<CPoint> pSSCorrTriJunctionPtLt = new List<CPoint>();
            //    pSSCorrTriJunctionPtLt.Add(pSSRiver.CptLt[0]);
            //    pSSCorrTriJunctionPtLt.AddRange(pSSRiver.CCorrTriJunctionPtLt);
            //    pSSCorrTriJunctionPtLt.Add(pSSRiver.CptLt[pSSRiver.CptLt.Count - 1]);

            //    List<CPolyline> pBSsubcpllt = new List<CPolyline>();
            //    List<CPolyline> pSSsubcpllt = new List<CPolyline>();
            //    for (int i = 0; i < pBSCorrTriJunctionPtLt.Count - 1; i++)
            //    {
            //        CPolyline pBSsubcpl = pBSRiver.GetSubPolyline(pBSCorrTriJunctionPtLt[i], pBSCorrTriJunctionPtLt[i + 1]);
            //        CPolyline pSSsubcpl = pSSRiver.GetSubPolyline(pSSCorrTriJunctionPtLt[i], pSSCorrTriJunctionPtLt[i + 1]);

            //        MessageBox.Show("CMRL.cs: Row499 is needed to be improved");
            //        //OptMPBDP.DivideCplForDP(pBSsubcpl);
            //        //OptMPBDP.DivideCplForDP(pSSsubcpl);

            //        pBSsubcpllt.Add(pBSsubcpl);
            //        pSSsubcpllt.Add(pSSsubcpl);
            //    }

            //    CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
            //    CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //    //double dblSumLength = frcpl.pPolyline.Length + tocpl.pPolyline.Length;
            //    CTranslation pTranslation = new CTranslation();
            //    double dblMin = double.MaxValue;
            //    int intIndex = 0;
            //    for (int i = 0; i < 25; i++)
            //    {
            //        ParameterThreshold.dblDLengthBound = pParameterThreshold.dblLengthSumRatio * (1 - 0.02 * i);
            //        ParameterThreshold.dblULengthBound = pParameterThreshold.dblLengthSumRatio / (1 - 0.02 * i);

            //        List<CPoint> ResultPtLt = new List<CPoint>();
            //        for (int j = 0; j < pBSsubcpllt.Count; j++)
            //        {
            //            //进行弯曲匹配，提取对应线段
            //            C5.LinkedList<CCorrSegment> CorrespondSegmentLk = new C5.LinkedList<CCorrSegment>();
            //            MessageBox.Show("CMRL.cs: Row523 is needed to be improved");
            //            //OptMPBDP.SubPolylineMatch(pBSsubcpllt[j], pSSsubcpllt[j], ParameterThreshold, ref CorrespondSegmentLk);

            //            //按指定方式对对应线段进行点匹配，提取对应点                
            //            ResultPtLt.AddRange(pAlgorithmsHelper.BuildPointCorrespondence(CorrespondSegmentLk, "Linear"));

            //            if ((j > 0) && (i < pBSsubcpllt.Count - 1))
            //            {   //考虑到线段相接处的顶点被两次求解对应点，最好删除一对对应点
            //                ResultPtLt.RemoveAt(ResultPtLt.Count - 1);
            //            }
            //        }

            //        double dblTranslation = pTranslation.CalTranslation(ResultPtLt);
            //        if (dblTranslation < dblMin)
            //        {
            //            intIndex = i;
            //            dblMin = dblTranslation;
            //        }
            //    }

            //    //求出最佳解
            //    ParameterThreshold.dblDLengthBound = pParameterThreshold.dblLengthSumRatio * (1 - 0.02 * intIndex);
            //    ParameterThreshold.dblULengthBound = pParameterThreshold.dblLengthSumRatio / (1 - 0.02 * intIndex);
            //    List<CPoint> pResultPtLt = new List<CPoint>();
            //    for (int j = 0; j < pBSsubcpllt.Count; j++)
            //    {
            //        //进行弯曲匹配，提取对应线段
            //        C5.LinkedList<CCorrSegment> CorrespondSegmentLk = new C5.LinkedList<CCorrSegment>();
            //        MessageBox.Show("CMRL.cs: Row551 is needed to be improved");
            //        //OptMPBDP.SubPolylineMatch(pBSsubcpllt[j], pSSsubcpllt[j], ParameterThreshold, ref CorrespondSegmentLk);

            //        //按指定方式对对应线段进行点匹配，提取对应点                
            //        pResultPtLt.AddRange(pAlgorithmsHelper.BuildPointCorrespondence(CorrespondSegmentLk, "Linear"));

            //        if ((j > 0) && (j < pBSsubcpllt.Count - 1))
            //        {   //考虑到线段相接处的顶点被两次求解对应点，最好删除一对对应点
            //            pResultPtLt.RemoveAt(pResultPtLt.Count - 1);
            //        }
            //    }

            //    pBSRiver.CResultPtLt = pResultPtLt;
            //    pCorrespondRiverNet.CResultPtLtLt.Add(pResultPtLt);


            //}
            //else
            //{
            //    CPolyline pBScpl = new CPolyline(pBSRiver);
            //    CPolyline pSScpl = new CPolyline(pSSRiver);
            //    MessageBox.Show("CMRL.cs: Row570 is needed to be improved");
            //    //pBSRiver.CResultPtLt = OptMPBDP.DWByDP(pBScpl, pSScpl, pParameterThreshold.dblLengthSumRatio, "Linear");
            //    pCorrespondRiverNet.CResultPtLtLt.Add(pBSRiver.CResultPtLt);
            //}
        }

        /// <summary>处理上一级河流没有对应河流的情况</summary>
        /// <param name="pCorrespondRiverNet">对应河网数据</param>
        /// <param name="pBSRiver">当前大比例尺表达河流</param>
        /// <param name="dblMainReductionRatio">上一级河流缩减比率</param>
        /// <remarks>RecursiveDWNotExistCorr：RecursiveDealWithNotExistCorrepondenceRiver</remarks>
        private void RecursiveDWNotExistCorr(CCorrespondRiverNet pCorrespondRiverNet, CRiver pBSRiver, double dblMainReductionRatio)
        {
            CRiver pMainStream = pBSRiver.CMainStream;
            IPoint intersectipt = pBSRiver.pPolyline.ToPoint;
            double dblFromStartLength = CGeoFunc.CalDistanceFromStartPoint((IPolyline5)pMainStream, intersectipt, false);
            double dblCurrentReductionRatio = pMainStream.pPolyline.Length / dblFromStartLength;
            pBSRiver.dblReductionRatio = dblCurrentReductionRatio * dblMainReductionRatio;
            //pCorrespondRiverNet.CResultRiverLt.Add(pBSRiver);

            //处理当前河流的支流
            for (int i = 0; i < pBSRiver.CTributaryLt.Count; i++)
            {
                RecursiveDWNotExistCorr(pCorrespondRiverNet, pBSRiver.CTributaryLt[i], pBSRiver.dblReductionRatio);
            }
        }



      
        #endregion     

        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
