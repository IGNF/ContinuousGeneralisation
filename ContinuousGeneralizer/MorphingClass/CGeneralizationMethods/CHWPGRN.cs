using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Maplex;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;

using MorphingClass.CEntity;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace MorphingClass.CGeneralizationMethods
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>河网汇水区域的层次化剖分与地图综合：The Hierarchical Watershed Partitioning and Generalization of River Network(艾廷华等，刘耀林，黄亚锋,2007,测绘学报)</remarks>
    public class CHWPGRN
    {


        private CParameterResult _ParameterResult;

        private List<CPolyline> _CPlLt;

        private CParameterInitialize _ParameterInitialize;

        public CHWPGRN()
        {

        }

        public CHWPGRN(CParameterInitialize ParameterInitialize)
        {

            //获取当前选择的点要素图层
            IFeatureLayer pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLayer.SelectedIndex);

            ParameterInitialize.pFeatureLayer = pFeatureLayer;
            ParameterInitialize.dblLevelExponent = Convert.ToDouble(ParameterInitialize.txtLevelExponent.Text);
            ParameterInitialize.dblOrderExponent = Convert.ToDouble(ParameterInitialize.txtOrderExponent.Text);
            _ParameterInitialize = ParameterInitialize;

            //获取线数组
            _CPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pFeatureLayer);

        }

        public void HWPGRNGeneralization()
        {


            //List<CPolyline> CPlLt = _CPlLt;
            //CParameterInitialize pParameterInitialize = _ParameterInitialize;

            //CParameterThreshold pParameterThreshold = new CParameterThreshold();
            //pParameterThreshold.dblVerySmall = CGeometricMethods.CalVerySmall(CPlLt);














            //List<CRiverNet> CRiverNetLt = BuildRiverNetLt(CPlLt, pParameterThreshold);     //依据线数据建立河网
            //CalWeightiness(CRiverNetLt, pParameterInitialize);                              //计算河流的重要性


            ////将河网中的河流数据结果提取出来，以河流列的形式存储
            //List<CRiver> CResultRiverLt = new List<CRiver>();
            //for (int i = 0; i < CRiverNetLt.Count; i++)
            //{
            //    CResultRiverLt.AddRange(CRiverNetLt[i].CRiverLt);
            //}
            //CHelperFunction.CalWeightinessUnitary(CResultRiverLt);



            //获取结果，全部记录在_ParameterResult中
            CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.CResultRiverLt = CResultRiverLt;
            _ParameterResult = ParameterResult;
        }

        #region 依据线数据建立河网列(包括建立干支关系，添加各支流成员)
        /// <summary>依据线数据建立河网列(包括建立干支关系，添加各支流成员)</summary>
        /// <param name="CPlLt">线数据</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <returns>河网列</returns>
        public List<CRiverNet> BuildRiverNetLt(List<CPolyline> CPlLt, CParameterThreshold pParameterThreshold)
        {
            double dblVerySmall = CConstants.dblVerySmall;

            //根据线数据生成河流数据
            List<CRiver> CAllRiverLt = new List<CRiver>();
            for (int i = 0; i < CPlLt.Count; i++)
            {
                CRiver pRiver = new CRiver(i, CPlLt[i], dblVerySmall);
                CAllRiverLt.Add(pRiver);
            }

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
                for (int j = i + 1; j < CAllRiverLt.Count; j++)
                {
                    bool isDisjoint = pSmallBufferRel.Disjoint(CAllRiverLt[j].pPolyline);
                    if (isDisjoint == false)
                    {
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

        /// <summary>创建河网，同时还记录了Tocpt2，计算了河流的层次和等级</summary>
        /// <param name="CAllRiverLt">河流数据</param>
        /// <returns>河网列</returns>
        private List<CRiverNet> CreateRiverNetLt(List<CRiver> CAllRiverLt, double dblVerySmall)
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
                RecursiveSettleRiver(ref pRiverNet, CMasterRiverLt[i]);    //递归添加属于该河网的河流
                //RecursiveFindTocpt2(CMasterRiverLt[i], dblVerySmall);      //找到当前河流与其各支流的相交点(属于当前河流)，并记录在支流河流数据中
                CalLeverAndOrder(CMasterRiverLt[i]);
                pRiverNetLt.Add(pRiverNet);
            }

            return pRiverNetLt;
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

        /// <summary>计算河流的层次和等级</summary>
        /// <param name="pMasterRiver">主干流</param>
        /// <remarks>主干流的层次为1，支流的层次为干流的层次+1；河流的等级为河流的所有支流数量之和再加1</remarks>
        public void CalLeverAndOrder(CRiver pMasterRiver)
        {
            //主河流的层次为1
            pMasterRiver.dblLevel = 1;

            //计算等级
            for (int i = 0; i < pMasterRiver.CTributaryLt.Count; i++)
            {
                RecursiveCalLeverAndOrder(pMasterRiver.CTributaryLt[i]);
                pMasterRiver.dblOrder = pMasterRiver.dblOrder + pMasterRiver.CTributaryLt[i].dblOrder;
            }
            pMasterRiver.dblOrder = pMasterRiver.dblOrder + 1;
        }


        /// <summary>递归计算河流的层次和等级</summary>
        /// <param name="pRiver">当前河流</param>
        public void RecursiveCalLeverAndOrder(CRiver pRiver)
        {
            //计算层次
            pRiver.dblLevel = pRiver.CMainStream.dblLevel + 1;

            //计算等级
            for (int i = 0; i < pRiver.CTributaryLt.Count; i++)
            {
                RecursiveCalLeverAndOrder(pRiver.CTributaryLt[i]);
                pRiver.dblOrder = pRiver.dblOrder + pRiver.CTributaryLt[i].dblOrder;
            }
            pRiver.dblOrder = pRiver.dblOrder + 1;


        }



        #endregion



        /// <summary>计算河流的重要性</summary>
        /// <param name="CAllRiverLt">河流数据</param>
        private void CalWeightiness(List<CRiverNet> CRiverNetLt, CParameterInitialize pParameterInitialize)
        {
            //先找到最大层次
            double dblMaxLevel = 0;
            for (int i = 0; i < CRiverNetLt.Count; i++)
            {
                for (int j = 0; j < CRiverNetLt[i].CRiverLt.Count; j++)
                {
                    if (CRiverNetLt[i].CRiverLt[j].dblLevel > dblMaxLevel)
                    {
                        dblMaxLevel = CRiverNetLt[i].CRiverLt[j].dblLevel;
                    }
                }
            }

            //根据公式计算河流重要性
            for (int i = 0; i < CRiverNetLt.Count; i++)
            {
                for (int j = 0; j < CRiverNetLt[i].CRiverLt.Count; j++)
                {
                    double dblOrderWeightiness = Math.Pow(CRiverNetLt[i].CRiverLt[j].dblOrder, pParameterInitialize.dblOrderExponent);
                    double dblLevelWeightiness = Math.Pow((dblMaxLevel - CRiverNetLt[i].CRiverLt[j].dblLevel + 1), pParameterInitialize.dblOrderExponent);
                    CRiverNetLt[i].CRiverLt[j].dblWeightiness = CRiverNetLt[i].CRiverLt[j].pPolyline.Length * dblOrderWeightiness * dblLevelWeightiness;
                }
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
