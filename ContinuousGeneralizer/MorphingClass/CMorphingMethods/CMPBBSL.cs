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
    /// 基于弯曲结构的线状要素Morphing变换方法（以基线长度为匹配依据：Length）
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CMPBBSL
    {
        
        
        private CParameterResult _ParameterResult;
        private CTriangulator _Triangulator = new CTriangulator();

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private CParameterInitialize _ParameterInitialize;

        public CMPBBSL()
        {

        }

        public CMPBBSL(CPolyline frcpl, CPolyline tocpl)
        {
            _FromCpl = frcpl;
            _ToCpl = tocpl;
        }

        public CMPBBSL(CParameterInitialize ParameterInitialize)
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
        public void MPBBSLMorphing()
        {          
            //CParameterInitialize ParameterInitialize = _ParameterInitialize;
            //CGeoFunc.SetCPlScaleEdgeLengthPtBelong(ref _LSCPlLt, CEnumScale.Larger);
            //CGeoFunc.SetCPlScaleEdgeLengthPtBelong(ref _SSCPlLt, CEnumScale.Smaller);
            //CPolyline frcpl = _LSCPlLt[0];
            //CPolyline tocpl = _SSCPlLt[0];

            ////计算极小值
            //
            //long lngStartTime = System.Environment.TickCount;  //开始时间

            //List<CPoint> frchcptlt = _Triangulator.CreateConvexHullEdgeLt2(frcpl, dblVerySmall);
            //CPolyline frchcpl = new CPolyline(0, frchcptlt);    //大比例尺折线外包多边形线段
            //frchcpl.SetPolyline();

            //List<CPoint> tochcptlt = _Triangulator.CreateConvexHullEdgeLt2(tocpl, dblVerySmall);
            //CPolyline tochcpl = new CPolyline(0, tochcptlt);    //小比例尺折线外包多边形线段
            //tochcpl.SetPolyline();

            ////添加约束数据生成图层，以便于利用AE中的功能(ct:constraint)
            //List<CPolyline> frctcpllt = new List<CPolyline>(); frctcpllt.Add(frcpl); frctcpllt.Add(frchcpl);
            //List<CPolyline> toctcpllt = new List<CPolyline>(); toctcpllt.Add(tocpl); toctcpllt.Add(tochcpl);
            //IFeatureLayer pBSFLayer = CHelpFunc.SaveCPlLt(frctcpllt, "frctcpllt", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //IFeatureLayer pSSFLayer = CHelpFunc.SaveCPlLt(toctcpllt, "toctcpllt", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            ////建立CDT并获取弯曲森林
            //CBendForest FromLeftBendForest = new CBendForest();
            //CBendForest FromRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableFrom = new CParameterVariable(frcpl, "FromCDT", pBSFLayer, dblVerySmall);
            //GetBendForest(pParameterVariableFrom,ref FromLeftBendForest, ref FromRightBendForest,ParameterInitialize);

            //CBendForest ToLeftBendForest = new CBendForest();
            //CBendForest ToRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableTo = new CParameterVariable(tocpl, "ToCDT", pSSFLayer, dblVerySmall);
            //GetBendForest(pParameterVariableTo, ref ToLeftBendForest, ref ToRightBendForest, ParameterInitialize);

            ////整理弯曲森林，获得弯曲数组
            //NeatenBendForest(frcpl, FromLeftBendForest);
            //NeatenBendForest(frcpl, FromRightBendForest);
            //NeatenBendForest(tocpl, ToLeftBendForest);
            //NeatenBendForest(tocpl, ToRightBendForest);

            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
            ////double dblSumLength = frcpl.pPolyline.Length + tocpl.pPolyline.Length;
            //CTranslation pTranslation = new CTranslation();

            ////计算阈值参数
            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //ParameterThreshold.dblFrLength = frcpl.pPolyline.Length;
            //ParameterThreshold.dblToLength = tocpl.pPolyline.Length;
            //ParameterThreshold.dblAngleBound = 0.262;

            //List<double> dblTranslationLt = new List<double>();
            //SortedDictionary<double, int> ResultsSlt = new SortedDictionary<double, int>(new CCmpDbl());
            //for (int i = 0; i <= 25; i++)
            //{
            //    //ParameterThreshold.dblDLengthBound = 1 * (1 - 0.02 * i);
            //    //ParameterThreshold.dblULengthBound = 1 / (1 - 0.02 * i);


            //    ////弯曲树匹配，寻找对应独立弯曲
            //    //List<CCorrespondBend> IndependCorrespondBendLt = new List<CCorrespondBend>();
            //    //IndependCorrespondBendLt.AddRange(BendTreeMatch(FromLeftBendForest, ToLeftBendForest, ParameterThreshold));
            //    //IndependCorrespondBendLt.AddRange(BendTreeMatch(FromRightBendForest, ToRightBendForest, ParameterThreshold));

            //    ////弯曲匹配，寻找对应弯曲
            //    //List<CCorrespondBend> CorrespondBendLt = BendMatch(IndependCorrespondBendLt, ParameterThreshold);

            //    ////提取对应线段
            //    //C5.LinkedList<CCorrespondSegment> CorrespondSegmentLk = CHelpFunc.DetectCorrespondSegment(frcpl, tocpl, CorrespondBendLt);
            //    ////CHelpFunc.PreviousWorkCSeLt(ref CorrespondSegmentLk);

            //    ////按指定方式对对应线段进行点匹配，提取对应点                
            //    //List<CPoint> ResultPtLt = new List<CPoint>();
            //    //ResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(CorrespondSegmentLk, "Linear");

            //    //double dblTranslation = pTranslation.CalTranslation(ResultPtLt);
            //    //dblTranslationLt.Add(dblTranslation);

            //    //ResultsSlt.Add(dblTranslation, i);
            //}

            
            ////必须重新算一遍！！！！！！
            ////理由：如果采用SortedList<double, CParameterResult> ResultsSlt = new SortedList<double, CParameterResult>(new CCmpDbl())记录结果，
            ////      则由于基本单位是CPoint（类似调用指针），最后必然影响CParameterResult中的ResultPtLt值
            ////int intIndex = ResultsSlt.Values[0];
            ////ParameterThreshold.dblDLengthBound = 1 * (1 - 0.02 * intIndex);
            ////ParameterThreshold.dblULengthBound = 1 / (1 - 0.02 * intIndex);

            //ParameterThreshold.dblDLengthBound = 1 * 0.95;
            //ParameterThreshold.dblULengthBound = 1 / 0.95;

            ////弯曲树匹配，寻找对应独立弯曲
            //List<CCorrespondBend> pIndependCorrespondBendLt = new List<CCorrespondBend>();
            //pIndependCorrespondBendLt.AddRange(BendTreeMatch(FromLeftBendForest, ToLeftBendForest, ParameterThreshold));
            //pIndependCorrespondBendLt.AddRange(BendTreeMatch(FromRightBendForest, ToRightBendForest, ParameterThreshold));

            ////弯曲匹配，寻找对应弯曲
            //List<CCorrespondBend> pCorrespondBendLt = BendMatch(pIndependCorrespondBendLt, ParameterThreshold);

            ////提取对应线段
            //LinkedList<CCorrespondSegment> pCorrespondSegmentLk = CHelpFunc.DetectCorrespondSegment(frcpl, tocpl, pCorrespondBendLt);
            ////CHelpFunc.PreviousWorkCSeLt(ref pCorrespondSegmentLk);

            ////按指定方式对对应线段进行点匹配，提取对应点
            //List<CPoint> pResultPtLt= pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");

            ////计算并显示运行时间
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            ////保存指标值及对应线            
            //CHelpFuncExcel.ExportDataltToExcel(dblTranslationLt, "translationlt0", _ParameterInitialize.strSavePath);
            //CHelpFunc.SaveCtrlLine(pCorrespondSegmentLk, "MPBBSLControlLine",dblVerySmall , ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCorrespondLine(pResultPtLt, "MPBBSLCorrLine", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            ////获取结果，全部记录在ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = frcpl;
            //ParameterResult.ToCpl = tocpl;
            //double pdblTranslation = pTranslation.CalTranslation(pResultPtLt);
            //ParameterResult.dblTranslation = pdblTranslation;
            //ParameterResult.pCorrespondSegmentLk = pCorrespondSegmentLk;
            //ParameterResult.CResultPtLt = pResultPtLt;
            //ParameterResult.lngTime = lngTime;
            //_ParameterResult = ParameterResult;
        }


        /// <summary>
        /// 用基于弯曲的Morphing方法进行处理
        /// </summary>
        /// <param name="pParameterInitialize">参数</param>
        /// <param name="pParameterVariableFrom">有关大比例尺折线的参数变量，主要储存了大比例尺线要素、文件保存时的名字、大比例尺图层和极小值</param>
        /// <param name="pParameterVariableTo">有关小比例尺折线的参数变量，主要储存了：小比例尺线要素、文件保存时的名字、小比例尺图层和极小值</param>
        /// <param name="ParameterThreshold">阈值参数，主要储存了：大、小比例尺线要素长度，弯曲基线比阈值范围</param>
        /// <remarks>创建约束三角网时，已使用外包多边形作为约束边</remarks>
        public LinkedList<CCorrespondSegment> DWByMPBBSL(CParameterInitialize pParameterInitialize, CParameterVariable pParameterVariableFrom, CParameterVariable pParameterVariableTo, CParameterThreshold ParameterThreshold)
        {
            CPolyline frcpl = pParameterVariableFrom.CPolyline;
            frcpl.SetPolyline();
            CPolyline tocpl = pParameterVariableTo.CPolyline;
            tocpl.SetPolyline();

            List<CPoint> frchcptlt = _Triangulator.CreateConvexHullEdgeLt2(frcpl, pParameterVariableFrom.dblVerySmall);
            CPolyline frchcpl = new CPolyline(0, frchcptlt);    //大比例尺折线外包多边形线段
            frchcpl.SetPolyline();

            List<CPoint> tochcptlt = _Triangulator.CreateConvexHullEdgeLt2(tocpl, pParameterVariableFrom.dblVerySmall);
            CPolyline tochcpl = new CPolyline(0, tochcptlt);    //小比例尺折线外包多边形线段
            tochcpl.SetPolyline();

            //添加约束数据生成图层，以便于利用AE中的功能(ct:constraint)
            List<CPolyline> frctcpllt = new List<CPolyline>(); frctcpllt.Add(frcpl); frctcpllt.Add(frchcpl);
            List<CPolyline> toctcpllt = new List<CPolyline>(); toctcpllt.Add(tocpl); toctcpllt.Add(tochcpl);
            IFeatureLayer pBSFLayer = CHelpFunc.SaveCPlLt(frctcpllt, "frctcpllt", pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);
            IFeatureLayer pSSFLayer = CHelpFunc.SaveCPlLt(toctcpllt, "toctcpllt", pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);

            pParameterVariableFrom.pFeatureLayer = pBSFLayer;
            pParameterVariableTo.pFeatureLayer = pSSFLayer;

            //建立CDT并获取弯曲森林
            CBendForest FromLeftBendForest = new CBendForest();
            CBendForest FromRightBendForest = new CBendForest();
            GetBendForest(pParameterVariableFrom, ref FromLeftBendForest, ref FromRightBendForest, pParameterInitialize);

            CBendForest ToLeftBendForest = new CBendForest();
            CBendForest ToRightBendForest = new CBendForest();
            GetBendForest(pParameterVariableTo, ref ToLeftBendForest, ref ToRightBendForest, pParameterInitialize);




            //弯曲树匹配，寻找对应独立弯曲
            List<CCorrespondBend> IndependCorrespondBendLt = new List<CCorrespondBend>();
            IndependCorrespondBendLt.AddRange(BendTreeMatch(FromLeftBendForest, ToLeftBendForest, ParameterThreshold));
            IndependCorrespondBendLt.AddRange(BendTreeMatch(FromRightBendForest, ToRightBendForest, ParameterThreshold));

            //弯曲匹配，寻找对应弯曲
            List<CCorrespondBend> CorrespondBendLt = BendMatch(IndependCorrespondBendLt, ParameterThreshold);

            //提取对应线段
            LinkedList<CCorrespondSegment> CorrespondSegmentLk = CHelpFunc.DetectCorrespondSegment(frcpl, tocpl, CorrespondBendLt);
            //CHelpFunc.PreviousWorkCSeLt(ref CorrespondSegmentLk);

            return CorrespondSegmentLk;
        }

        /// <summary>
        /// 获取折线的弯曲森林
        /// </summary>
        /// <param name="cpl">折线</param>
        /// <param name="pLeftBendForest">折线左边的弯曲森林</param>
        /// <param name="pRightBendForest">折线右边的弯曲森林</param>
        /// <param name="strName">留作保存三角网用</param>
        /// <remarks>创建约束三角网时，并未使用外包多边形作为约束边
        ///          注意：本方法中的CreateCDT将会改变pParameterVariable中的CPolyline</remarks>
        public void GetBendForest(CParameterVariable pParameterVariable, ref CBendForest pLeftBendForest, ref CBendForest pRightBendForest, CParameterInitialize pParameterInitialize)
        {
            //List<CPoint> cptlt = pParameterVariable.CPolyline.CptLt;
            //List<CEdge> CEdgeLt = new List<CEdge>();
            //for (int i = 0; i < cptlt.Count - 1; i++)
            //{
            //    CEdge pEdge = new CEdge(cptlt[i], cptlt[i + 1]);
            //    CEdgeLt.Add(pEdge);
            //}

            CTriangulator OptCDT = new CTriangulator();
            CPolyline newcpl = new CPolyline(pParameterVariable.CPolyline.ID, pParameterVariable.CPolyline.CptLt);
            List<CTriangle> CDTLt = OptCDT.CreateCDT(pParameterVariable.pFeatureLayer, ref newcpl, pParameterVariable.dblVerySmall);
            pParameterVariable.CPolyline = newcpl;

            //for (int i = 0; i < CDTLt.Count; i++) CDTLt[i].TID = i;  //到此为止，约束三角形建立完成，各三角形不再发生变化，将各三角形编号           
            OptCDT.GetSETriangle(ref CDTLt, pParameterVariable.dblVerySmall);  //确定共边三角形
            OptCDT.ConfirmTriangleSide(ref CDTLt, pParameterVariable.CPolyline, pParameterVariable.dblVerySmall); //确定各三角形位于折线的左右边
            OptCDT.SignTriTypeAll(ref CDTLt);   //标记I、II、III、VI类三角形

            pLeftBendForest = OptCDT.BuildBendForestNeed2(ref CDTLt, pParameterVariable.CPolyline.CptLt, "Left", pParameterVariable.dblVerySmall);
            pRightBendForest = OptCDT.BuildBendForestNeed2(ref CDTLt, pParameterVariable.CPolyline.CptLt, "Right", pParameterVariable.dblVerySmall);


            //保存三角网
            List<CTriangle> CTriangleLt = new List<CTriangle>();
            for (int i = 0; i < CDTLt.Count; i++)
            {
                if (CDTLt[i].strTriType != "I")
                {
                    CTriangleLt.Add(CDTLt[i]);
                }
            }
            //CHelpFunc.SaveTriangles(CTriangleLt, pParameterVariable.strName, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);
        }

        /// <summary>
        /// 整理弯曲森林中的弯曲
        /// </summary>
        /// <param name="cpl">折线</param>
        /// <param name="pBendForest">弯曲森林</param>
        /// <returns>按相对位置排序的弯曲列表</returns>
        /// <remarks>计算各弯曲的相对起始位置；并给按顺序编号</remarks>
        public SortedDictionary<double, CBend> NeatenBendForest(CPolyline cpl, CBendForest pBendForest)
        {
            SortedDictionary<double, CBend> pBendSlt = new SortedDictionary<double, CBend>(new CCmpDbl());
            //整理弯曲
            for (int i = 0; i < pBendForest.Count; i++)
            {
                CBend pBend = pBendForest.ElementAt(i).Value;
                RecursiveNeatenBendForest(cpl, pBend, pBendSlt);
            }

            //给弯曲编号
            for (int i = 0; i < pBendSlt.Count; i++)
            {
                pBendSlt.ElementAt(i).Value.ID = i;
            }

            return pBendSlt;

        }

        /// <summary>
        /// 递归整理弯曲
        /// </summary>
        /// <param name="cpl">折线</param>
        /// <param name="pBendForest">弯曲</param>
        /// <param name="pBendSlt">弯曲列表</param>
        private void RecursiveNeatenBendForest(CPolyline cpl, CBend pBend, SortedDictionary<double, CBend> pBendSlt)
        {
            if (pBend == null)
            {
                return;
            }

            pBend.dblStartRL = CGeoFunc.CalDistanceFromStartPoint(cpl.pPolyline, pBend.FromPoint, true);
            pBend.dblEndRL = CGeoFunc.CalDistanceFromStartPoint(cpl.pPolyline, pBend.ToPoint, true);
            pBendSlt.Add(pBend.dblStartRL, pBend);

            RecursiveNeatenBendForest(cpl, pBend.CLeftBend, pBendSlt);
            RecursiveNeatenBendForest(cpl, pBend.CRightBend, pBendSlt);
        }

        /// <summary>
        /// 进行弯曲匹配，提取对应线段
        /// </summary>
        /// <param name="CFromBendForest">大比例尺弯曲森林</param>
        /// <param name="CToBendForest">小比例尺弯曲森林</param>
        /// <param name="ParameterThreshold">参数容器</param>
        /// <param name="CorrespondSegmentLk">对应线段列</param>
        /// <remarks>弯曲树匹配和孩子弯曲匹配都由本函数来执行</remarks>
        public List<CCorrespondBend> BendTreeMatch(CBendForest CFromBendForest, CBendForest CToBendForest, CParameterThreshold ParameterThreshold)
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
            List<CCorrespondBend> pIndependCorrespondBendLt = IndependBendMatch(pFromIndependBendSlt, pToIndependBendSlt, ParameterThreshold);

            return pIndependCorrespondBendLt;
        }

        /// <summary>
        /// 进行弯曲匹配，提取对应线段
        /// </summary>
        /// <param name="CFromBendForest">大比例尺弯曲森林</param>
        /// <param name="CToBendForest">小比例尺弯曲森林</param>
        /// <param name="ParameterThreshold">参数容器</param>
        /// <param name="CorrespondSegmentLk">对应线段列</param>
        /// <remarks>弯曲树匹配和孩子弯曲匹配都由本函数来执行</remarks>
        public List<CCorrespondBend> BendTreeMatch2(CBendForest CFromBendForest, CBendForest CToBendForest, CParameterThreshold ParameterThreshold)
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
            List<CCorrespondBend> pIndependCorrespondBendLt = IndependBendMatch2(pFromIndependBendSlt, pToIndependBendSlt, ParameterThreshold);

            return pIndependCorrespondBendLt;
        }


        /// <summary>
        /// 弯曲匹配(全局等同搜索)
        /// </summary>
        /// <param name="pFromBendSlt">大比例尺弯曲列表</param>
        /// <param name="pToBendSlt">小比例尺弯曲列表</param>
        /// <param name="dblRatioBound">比例阈值</param>
        /// <returns>对应弯曲列表</returns>
        /// <remarks></remarks>
        private List<CCorrespondBend> IndependBendMatch(SortedDictionary<double, CBend> pFromBendSlt, SortedDictionary<double, CBend> pToBendSlt,
                                                        CParameterThreshold ParameterThreshold)
        {
            //清理弯曲的对应弯曲列表
            foreach (CBend pBend in pFromBendSlt.Values)
            {
                pBend.pCorrespondBendLt.Clear();
            }
            foreach (CBend pBend in pToBendSlt.Values)
            {
                pBend.pCorrespondBendLt.Clear();
            }

            //搜索符合要求的对应弯曲
            List<CCorrespondBend> pCorrespondBendLt = new List<CCorrespondBend>();
            int intLastMatchj = 0;   //该值并不作精确要求，仅为中间搜索点的估算值
            for (int i = 0; i < pFromBendSlt.Values.Count; i++)
            {
                CBend pfrbend = pFromBendSlt.ElementAt(i).Value;
                for (int j = 0; j < pToBendSlt.Values.Count; j++)
                {
                    CBend ptobend = pToBendSlt.ElementAt(i).Value;
                    double dblAngleDiff = pfrbend.pBaseLine.Angle - ptobend.pBaseLine.Angle;
                    double dblLengthRatio = pfrbend.pBaseLine.Length / ptobend.pBaseLine.Length;

                    if ((Math.Abs(dblAngleDiff) <= ParameterThreshold.dblAngleBound)
                        && (dblLengthRatio >= ParameterThreshold.dblDLengthBound) && (dblLengthRatio <= ParameterThreshold.dblULengthBound))
                    {
                        CCorrespondBend pCorrespondBend = new CCorrespondBend(pfrbend, ptobend);
                        pCorrespondBendLt.Add(pCorrespondBend);
                    }
                }
            }

            return pCorrespondBendLt;
        }

        /// <summary>
        /// 弯曲匹配(全局等同搜索)
        /// </summary>
        /// <param name="pFromBendSlt">大比例尺弯曲列表</param>
        /// <param name="pToBendSlt">小比例尺弯曲列表</param>
        /// <param name="dblRatioBound">比例阈值</param>
        /// <returns>对应弯曲列表</returns>
        /// <remarks></remarks>
        private List<CCorrespondBend> IndependBendMatch2(SortedDictionary<double, CBend> pFromBendSlt, SortedDictionary<double, CBend> pToBendSlt,
                                                        CParameterThreshold ParameterThreshold)
        {
            //清理弯曲的对应弯曲列表
            foreach (CBend  pBend in pFromBendSlt.Values )
            {
                pBend.pCorrespondBendLt.Clear();
            }
            foreach (CBend pBend in pToBendSlt.Values)
            {
                pBend.pCorrespondBendLt.Clear();
            }

            //搜索符合要求的对应弯曲
            List<CCorrespondBend> pCorrespondBendLt = new List<CCorrespondBend>();
            int intI = 0;
            int intJ = 0;
            while (intI < pFromBendSlt.Count && intJ < pToBendSlt.Count)
            {
                CBend pfrbend = pFromBendSlt.ElementAt(intI).Value;
                CBend ptobend = pToBendSlt.ElementAt(intJ).Value;
                double dblRatioLengthi = pfrbend.Length / ParameterThreshold.dblFrLength;
                double dblRatioLengthj = ptobend.Length / ParameterThreshold.dblToLength;
                double dblStartDiff = pfrbend.dblStartRL - ptobend.dblStartRL;
                double dblEndDiff = pfrbend.dblEndRL - ptobend.dblEndRL;

                //计算相对位置差阈值
                double dblRatioBound;
                if (dblRatioLengthi <= dblRatioLengthj)
                {
                    dblRatioBound = 0.5 * dblRatioLengthi;
                }
                else
                {
                    dblRatioBound = 0.5 * dblRatioLengthj;
                }

                double dblLengthRatio = pfrbend.pBaseLine.Length / ptobend.pBaseLine.Length;
                if ((Math.Abs(dblStartDiff) < dblRatioBound) && (Math.Abs(dblEndDiff) < dblRatioBound)
                    && (dblLengthRatio >= ParameterThreshold.dblDLengthBound) && (dblLengthRatio <= ParameterThreshold.dblULengthBound))
                {
                    CCorrespondBend pCorrespondBend = new CCorrespondBend(pFromBendSlt.ElementAt(intI).Value, pToBendSlt.ElementAt(intJ).Value);
                    pCorrespondBendLt.Add(pCorrespondBend);
                    intI++;
                    intJ++;
                }
                else
                {
                    if ((ptobend.dblEndRL -pfrbend .dblStartRL) > (0.5*dblRatioLengthi) )
                    {
                        intI++;
                    }
                    if ((pfrbend.dblEndRL - ptobend.dblStartRL) > (0.5 * dblRatioLengthj))
                    {
                        intJ++;
                    }
                }
            }



            //int intLastMatchj = 0;   //该值并不作精确要求，仅为中间搜索点的估算值
            //for (int i = 0; i < pFromBendSlt.Values.Count; i++)
            //{
            //    CBend pfrbend = pFromBendSlt.Values[i];
            //    double dblRatioLengthi = pfrbend.Length / ParameterThreshold.dblFrLength;


            //    //int dblTempMatchj = 0;
            //    //为了节省时间，从中间向两边搜索
            //    //以intLastMatchj为基准前进搜索
            //    for (int j = intLastMatchj; j < pToBendSlt.Values.Count; j++)
            //    {
            //        CBend ptobend = pToBendSlt.Values[j];
            //        double dblRatioLengthj = ptobend.Length / ParameterThreshold.dblToLength;
            //        double dblStartDiff = pfrbend.dblStartRL - ptobend.dblStartRL;
            //        double dblEndDiff = pfrbend.dblEndRL - ptobend.dblEndRL;
            //        double dblAngleDiff = pfrbend.pBaseLine.Angle - ptobend.pBaseLine.Angle;

            //        //计算相对位置差阈值
            //        double dblRatioBoundj;
            //        if (dblRatioLengthi >= dblRatioLengthj)
            //        {
            //            dblRatioBoundj = 0.25 * dblRatioLengthi;
            //        }
            //        else
            //        {
            //            dblRatioBoundj = 0.25 * dblRatioLengthj;
            //        }

            //        if (dblStartDiff < (-dblRatioBoundj))
            //        {
            //            break; //如果已经超出一定范围，则没必要再向后搜索了
            //        }

            //        double dblLengthRatio = pfrbend.pBaseLine.Length / ptobend.pBaseLine.Length;
            //        if ((Math.Abs(dblStartDiff) <= dblRatioBoundj) && (Math.Abs(dblEndDiff) <= dblRatioBoundj) && (Math.Abs(dblAngleDiff) <= ParameterThreshold.dblAngleBound)
            //            && (dblLengthRatio >= ParameterThreshold.dblDLengthBound) && (dblLengthRatio <= ParameterThreshold.dblULengthBound))
            //        {
            //            CCorrespondBend pCorrespondBend = new CCorrespondBend(pFromBendSlt.Values[i], pToBendSlt.Values[j]);
            //            pCorrespondBendLt.Add(pCorrespondBend);
            //            intLastMatchj = j;
            //        }
            //    }
            //}
           
            return pCorrespondBendLt;
        }

        #region 弯曲匹配(全局等同搜索)
        ///// <summary>
        ///// 弯曲匹配(全局等同搜索)
        ///// </summary>
        ///// <param name="pFromBendSlt">大比例尺弯曲列表</param>
        ///// <param name="pToBendSlt">小比例尺弯曲列表</param>
        ///// <param name="dblRatioBound">比例阈值</param>
        ///// <returns>对应弯曲列表</returns>
        ///// <remarks></remarks>
        //private List<CCorrespondBend> BendMatch(SortedList<double, CBend> pFromBendSlt, SortedList<double, CBend> pToBendSlt, double dblRatioBound)
        //{
        //    //清理弯曲的对应弯曲列表
        //    for (int i = 0; i < pFromBendSlt.Count ; i++)
        //    {
        //        pFromBendSlt.Values[i].pCorrespondBendLt.Clear();
        //    }
        //    for (int i = 0; i < pToBendSlt.Count ; i++)
        //    {
        //        pToBendSlt.Values[i].pCorrespondBendLt.Clear();
        //    }


        //    //搜索符合要求的对应弯曲
        //    int intLastMatchj = 0;   //该值并不作精确要求，仅为中间搜索点的估算值
        //    for (int i = 0; i < pFromBendSlt.Values.Count; i++)
        //    {
        //        CBend pfrbend = pFromBendSlt.Values[i];
        //        double dblRatioBoundi = dblRatioBound * pfrbend.Length;
        //        int dblTempMatchj = 0;
        //        //为了节省时间，从中间向两边搜索
        //        //以intLastMatchj为基准前进搜索
        //        for (int j = intLastMatchj; j < pToBendSlt.Values.Count; j++)
        //        {
        //            CBend ptobend = pToBendSlt.Values[j];
        //            double dblStartDiff = pfrbend.dblStartRL - ptobend.dblStartRL;
        //            double dblEndDiff = Math.Abs(pfrbend.dblEndRL - ptobend.dblEndRL);
        //            if (dblStartDiff < (-dblRatioBoundi))
        //            {
        //                break;
        //            }
        //            if ((Math.Abs(dblStartDiff) < dblRatioBoundi) && (Math.Abs(dblEndDiff) < dblRatioBoundi))
        //            {
        //                double dblSumDiff = dblStartDiff + dblEndDiff;
        //                pfrbend.pCorrespondBendLt.Add(dblSumDiff, ptobend);
        //                ptobend.pCorrespondBendLt.Add(dblSumDiff, pfrbend);
        //                dblTempMatchj = j;
        //            }
        //        }

        //        //以intLastMatchj为基准后退搜索
        //        for (int j = intLastMatchj - 1; j >= 0; j--)
        //        {
        //            CBend ptobend = pToBendSlt.Values[j];
        //            double dblStartDiff = pfrbend.dblStartRL - ptobend.dblStartRL;
        //            double dblEndDiff = Math.Abs(pfrbend.dblEndRL - ptobend.dblEndRL);
        //            if (dblStartDiff > dblRatioBoundi)
        //            {
        //                break;
        //            }
        //            if ((Math.Abs(dblStartDiff) < dblRatioBoundi) && (Math.Abs(dblEndDiff) < dblRatioBoundi))
        //            {
        //                double dblSumDiff = dblStartDiff + dblEndDiff;
        //                pfrbend.pCorrespondBendLt.Add(dblSumDiff, ptobend);
        //                ptobend.pCorrespondBendLt.Add(dblSumDiff, pfrbend);
        //                dblTempMatchj = j;
        //            }
        //        }

        //        intLastMatchj = dblTempMatchj;
        //    }

        //    //如果两个弯曲互为最合适弯曲，则找到一对对应弯曲
        //    List<CCorrespondBend> pCorrespondBendLt = new List<CCorrespondBend>();
        //    for (int i = 0; i < pFromBendSlt.Values.Count; i++)
        //    {
        //        if (pFromBendSlt.Values[i].pCorrespondBendLt.Count > 0)
        //        {
        //            CBend optimumtobend = pFromBendSlt.Values[i].pCorrespondBendLt.Values[0];
        //            if (optimumtobend.pCorrespondBendLt.Count > 0)
        //            {
        //                if (pFromBendSlt.Values[i].ID == optimumtobend.pCorrespondBendLt.Values[0].ID)
        //                {
        //                    CCorrespondBend pCorrespondBend = new CCorrespondBend(pFromBendSlt.Values[i], optimumtobend);
        //                    pCorrespondBendLt.Add(pCorrespondBend);
        //                }
        //            }
        //        }
        //    }

        //    return pCorrespondBendLt;
        //}
        #endregion

        public List<CCorrespondBend> BendMatch(List<CCorrespondBend> pIndependCorrespondBendLt, CParameterThreshold ParameterThreshold)
        {
            List<CCorrespondBend> pCorrespondBendLt = new List<CCorrespondBend>();
            pCorrespondBendLt.AddRange(pIndependCorrespondBendLt);
            for (int i = 0; i < pIndependCorrespondBendLt.Count; i++)
            {
                RecursiveBendMatch(pIndependCorrespondBendLt[i].CFromBend, pIndependCorrespondBendLt[i].CToBend, ref pCorrespondBendLt, ParameterThreshold);
            }
            return pCorrespondBendLt;

        }

        private void RecursiveBendMatch(CBend pFromBend, CBend pToBend, ref List<CCorrespondBend> pCorrespondBendLt, CParameterThreshold ParameterThreshold)
        {
            if (pFromBend.CLeftBend == null || pToBend.CLeftBend == null)
            {
                return;
            }

            double dblRatioLL = pFromBend.CLeftBend.pBaseLine.Length / pToBend.CLeftBend.pBaseLine.Length;
            double dblRatioRR = pFromBend.CRightBend.pBaseLine.Length / pToBend.CRightBend.pBaseLine.Length;
            //double dblRatioAPAP = (pFromBend.CLeftBend.Length + pFromBend.CRightBend.Length) / (pToBend.CLeftBend.Length + pToBend.CRightBend.Length);
            //double dblRatioLA = pFromBend.CLeftBend.Length / pToBend.Length;
            //double dblRatioRA = pFromBend.CRightBend.Length / pToBend.Length;



            if ((dblRatioLL >= ParameterThreshold.dblDLengthBound) && (dblRatioLL <= ParameterThreshold.dblULengthBound) &&
                (dblRatioRR >= ParameterThreshold.dblDLengthBound) && (dblRatioRR <= ParameterThreshold.dblULengthBound))
            {
                //左右弯曲分别对应
                CCorrespondBend pLeftCorrespondBend = new CCorrespondBend(pFromBend.CLeftBend, pToBend.CLeftBend);
                CCorrespondBend pRightCorrespondBend = new CCorrespondBend(pFromBend.CRightBend, pToBend.CRightBend);
                pCorrespondBendLt.Add(pLeftCorrespondBend);
                pCorrespondBendLt.Add(pRightCorrespondBend);

                //继续往下遍历
                RecursiveBendMatch(pFromBend.CLeftBend, pToBend.CLeftBend, ref pCorrespondBendLt, ParameterThreshold);
                RecursiveBendMatch(pFromBend.CRightBend, pToBend.CRightBend, ref pCorrespondBendLt, ParameterThreshold);
            }
            else  //此步必不可少，作用还是很明显的
            {
                double dblRatioLR = pFromBend.CLeftBend.pBaseLine.Length / pToBend.CRightBend.pBaseLine.Length;
                double dblRatioRL = pFromBend.CRightBend.pBaseLine.Length / pToBend.CLeftBend.pBaseLine.Length;


                if ((dblRatioLL >= ParameterThreshold.dblDLengthBound) && (dblRatioLR >= ParameterThreshold.dblDLengthBound))
                {
                    RecursiveBendMatch(pFromBend.CLeftBend, pToBend, ref pCorrespondBendLt, ParameterThreshold);
                }
                if ((dblRatioRL >= ParameterThreshold.dblDLengthBound) && (dblRatioRR >= ParameterThreshold.dblDLengthBound))
                {
                    RecursiveBendMatch(pFromBend.CRightBend, pToBend, ref pCorrespondBendLt, ParameterThreshold);
                }
            }
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
