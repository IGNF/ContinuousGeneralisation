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
    /// 基于弯曲结构的线状要素Morphing变换方法（以长度为匹配依据：Length）
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CMPMBS
    {
        
        
        private CParameterResult _ParameterResult;

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private CParameterInitialize _ParameterInitialize;

        public CMPMBS()
        {

        }

        public CMPMBS(CPolyline frcpl, CPolyline tocpl)
        {
            _FromCpl = frcpl;
            _ToCpl = tocpl;
        }

        public CMPMBS(CParameterInitialize ParameterInitialize)
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

        //基于弯曲的Morphing方法（删除小孩子弯曲）
        public void MPBBSLMorphing()
        {
            //var ParameterInitialize = _ParameterInitialize;
            //CPolyline frcpl = _LSCPlLt[0];
            //CPolyline tocpl = _SSCPlLt[0];

            ////计算极小值
            //CGeoFunc.CalDistanceParameters(_LSCPlLt, _SSCPlLt);

            //long lngStartTime = System.Environment.TickCount;  //开始时间

            //CMPBBSL OptMPBBSL = new CMPBBSL();
            
            ////建立CDT并获取弯曲森林
            //CBendForest FromLeftBendForest = new CBendForest();
            //CBendForest FromRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableFrom = new CParameterVariable(frcpl, "FromCDT", ParameterInitialize.pBSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableFrom, ref FromLeftBendForest, ref FromRightBendForest, ParameterInitialize);

            //CBendForest ToLeftBendForest = new CBendForest();
            //CBendForest ToRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableTo = new CParameterVariable(tocpl, "ToCDT", ParameterInitialize.pSSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableTo, ref ToLeftBendForest, ref ToRightBendForest, ParameterInitialize);

            ////整理弯曲森林，获得弯曲数组
            //SortedList<double, CBend> FromLeftBendSlt = OptMPBBSL.NeatenBendForest(frcpl, FromLeftBendForest);
            //SortedList<double, CBend> FromRightBendSlt = OptMPBBSL.NeatenBendForest(frcpl, FromRightBendForest);
            //SortedList<double, CBend> ToLeftBendSlt = OptMPBBSL.NeatenBendForest(tocpl, ToLeftBendForest);
            //SortedList<double, CBend> ToRightBendSlt = OptMPBBSL.NeatenBendForest(tocpl, ToRightBendForest);

            //////计算比例阈值
            ////double dblRatioBound = CalRatioBound(frcpl);

            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
            //CTranslation pTranslation = new CTranslation();

            //////计算阈值参数
            ////CParameterThreshold ParameterThreshold = new CParameterThreshold();
            ////ParameterThreshold.dblFrLength = frcpl.pPolyline.Length;
            ////ParameterThreshold.dblToLength = tocpl.pPolyline.Length;

            ////List<double> dblTranslationLt = new List<double>();
            ////SortedList<double, int> ResultsSlt = new SortedList<double, int>(new CCmpDbl());

            ////计算阈值参数
            //double dblBound = 0.98;
            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //ParameterThreshold.dblFrLength = frcpl.pPolyline.Length;
            //ParameterThreshold.dblToLength = tocpl.pPolyline.Length;
            //ParameterThreshold.dblDLengthBound = dblBound;
            //ParameterThreshold.dblULengthBound = 1 / dblBound;

            ////弯曲树匹配，寻找对应独立弯曲
            //List<CCorrespondBend> LeftIndependCorrespondBendLt = new List<CCorrespondBend>();
            //LeftIndependCorrespondBendLt = OptMPBBSL.BendTreeMatch(FromLeftBendForest, ToLeftBendForest, ParameterThreshold);

            //List<CCorrespondBend> RightIndependCorrespondBendLt = new List<CCorrespondBend>();
            //RightIndependCorrespondBendLt = OptMPBBSL.BendTreeMatch(FromRightBendForest, ToRightBendForest, ParameterThreshold);












            //for (int i = 0; i <= 25; i++)
            //{
            //    ParameterThreshold.dblDLengthBound = 1 * (1 - 0.02 * i);
            //    ParameterThreshold.dblULengthBound = 1 / (1 - 0.02 * i);


            //    //弯曲树匹配，寻找对应独立弯曲
            //    List<CCorrespondBend> IndependCorrespondBendLt = new List<CCorrespondBend>();
            //    IndependCorrespondBendLt.AddRange(BendTreeMatch(FromLeftBendForest, ToLeftBendForest, ParameterThreshold));
            //    IndependCorrespondBendLt.AddRange(BendTreeMatch(FromRightBendForest, ToRightBendForest, ParameterThreshold));

            //    //弯曲匹配，寻找对应弯曲
            //    List<CCorrespondBend> CorrespondBendLt = BendMatch(IndependCorrespondBendLt, ParameterThreshold);

            //    //提取对应线段
            //    C5.LinkedList<CCorrSegment> CorrespondSegmentLk = DetectCorrespondSegment(frcpl, tocpl, CorrespondBendLt);

            //    //按指定方式对对应线段进行点匹配，提取对应点                
            //    List<CPoint> ResultPtLt = new List<CPoint>();
            //    ResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(CorrespondSegmentLk, "Linear");

            //    double dblTranslation = pTranslation.CalTranslation(ResultPtLt);
            //    dblTranslationLt.Add(dblTranslation);

            //    ResultsSlt.Add(dblTranslation, i);
            //}


            ////必须重新算一遍！！！！！！
            ////理由：如果采用SortedList<double, CParameterResult> ResultsSlt = new SortedList<double, CParameterResult>(new CCmpDbl())记录结果，
            ////      则由于基本单位是CPoint，最后必然影响CParameterResult中的ResultPtLt值
            //int intIndex = ResultsSlt.Values[0];
            //ParameterThreshold.dblDLengthBound = 1 * (1 - 0.02 * intIndex);
            //ParameterThreshold.dblULengthBound = 1 / (1 - 0.02 * intIndex);


            ////弯曲树匹配，寻找对应独立弯曲
            //List<CCorrespondBend> pIndependCorrespondBendLt = new List<CCorrespondBend>();
            //pIndependCorrespondBendLt.AddRange(BendTreeMatch(FromLeftBendForest, ToLeftBendForest, ParameterThreshold));
            //pIndependCorrespondBendLt.AddRange(BendTreeMatch(FromRightBendForest, ToRightBendForest, ParameterThreshold));

            ////弯曲匹配，寻找对应弯曲
            //List<CCorrespondBend> pCorrespondBendLt = BendMatch(pIndependCorrespondBendLt, ParameterThreshold);

            ////提取对应线段
            //C5.LinkedList<CCorrSegment> pCorrespondSegmentLk = DetectCorrespondSegment(frcpl, tocpl, pCorrespondBendLt);

            ////按指定方式对对应线段进行点匹配，提取对应点                
            //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");

            ////计算并显示运行时间
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            ////保存指标值及对应线            
            //CHelpFuncExcel.ExportDataltToExcel(dblTranslationLt, "translationlt0", _ParameterInitialize.strSavePath);
            //CHelpFunc.SaveCtrlLine(pCorrespondSegmentLk, "MPBBSLControlLine", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
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

       




































        

        public List<CCorrespondBend> MBendMatch(List<CCorrespondBend> pIndependCorrespondBendLt,CParameterThreshold ParameterThreshold, string strSide)
        {
            //List<CCorrespondBend> pCorrespondBendLt = new List<CCorrespondBend>();
            //pCorrespondBendLt.AddRange(pIndependCorrespondBendLt);
            //for (int i = 0; i < pIndependCorrespondBendLt.Count; i++)
            //{
            //    RecursiveBendMatch(pIndependCorrespondBendLt[i].CFromBend, pIndependCorrespondBendLt[i].CToBend, ref pCorrespondBendLt, ParameterThreshold);
            //}
            //return pCorrespondBendLt;
            return null;

        }

        private void RecursiveMBendMatch(CBend pFromBend, CBend pToBend, ref List<CCorrespondBend> pCorrespondBendLt, CParameterThreshold ParameterThreshold)
        {





            ////建立CDT并获取弯曲森林
            //CBendForest FromLeftBendForest = new CBendForest();
            //CBendForest FromRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableFrom = new CParameterVariable(frcpl, "FromCDT", ParameterInitialize.pBSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableFrom, ref FromLeftBendForest, ref FromRightBendForest, ParameterInitialize);

            //CBendForest ToLeftBendForest = new CBendForest();
            //CBendForest ToRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableTo = new CParameterVariable(tocpl, "ToCDT", ParameterInitialize.pSSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableTo, ref ToLeftBendForest, ref ToRightBendForest, ParameterInitialize);

            ////整理弯曲森林，获得弯曲数组
            //SortedList<double, CBend> FromLeftBendSlt = OptMPBBSL.NeatenBendForest(frcpl, FromLeftBendForest);
            //SortedList<double, CBend> FromRightBendSlt = OptMPBBSL.NeatenBendForest(frcpl, FromRightBendForest);
            //SortedList<double, CBend> ToLeftBendSlt = OptMPBBSL.NeatenBendForest(tocpl, ToLeftBendForest);
            //SortedList<double, CBend> ToRightBendSlt = OptMPBBSL.NeatenBendForest(tocpl, ToRightBendForest);















            //if (pFromBend.CLeftBend == null || pToBend.CLeftBend == null)
            //{
            //    return;
            //}

            //double dblRatioLL = pFromBend.CLeftBend.pBaseLine.Length / pToBend.CLeftBend.pBaseLine.Length;
            //double dblRatioRR = pFromBend.CRightBend.pBaseLine.Length / pToBend.CRightBend.pBaseLine.Length;



            //if ((dblRatioLL >= ParameterThreshold.dblDLengthBound) && (dblRatioLL <= ParameterThreshold.dblULengthBound) &&
            //    (dblRatioRR >= ParameterThreshold.dblDLengthBound) && (dblRatioRR <= ParameterThreshold.dblULengthBound))
            //{
            //    //左右弯曲分别对应
            //    CCorrespondBend pLeftCorrespondBend = new CCorrespondBend(pFromBend.CLeftBend, pToBend.CLeftBend);
            //    CCorrespondBend pRightCorrespondBend = new CCorrespondBend(pFromBend.CRightBend, pToBend.CRightBend);
            //    pCorrespondBendLt.Add(pLeftCorrespondBend);
            //    pCorrespondBendLt.Add(pRightCorrespondBend);

            //    //继续往下遍历
            //    RecursiveBendMatch(pFromBend.CLeftBend, pToBend.CLeftBend, ref pCorrespondBendLt, ParameterThreshold);
            //    RecursiveBendMatch(pFromBend.CRightBend, pToBend.CRightBend, ref pCorrespondBendLt, ParameterThreshold);
            //}           
        }

        /// <summary>
        /// 获取折线的弯曲森林
        /// </summary>
        /// <param name="cpl">折线</param>
        /// <param name="pLeftBendForest">折线左边的弯曲森林</param>
        /// <param name="pRightBendForest">折线右边的弯曲森林</param>
        /// <param name="strName">留作保存三角网用</param>
        /// <remarks>创建约束三角网时，并未使用外包多边形作为约束边</remarks>
        public void MGetBendForest(CParameterVariable pParameterVariable, ref CBendForest pBendForest, string strSide, CParameterInitialize pParameterInitialize)
        {
            //List<CPoint> cptlt = pParameterVariable.CPolyline.CptLt;
            //List<CEdge> CEdgeLt = new List<CEdge>();
            //for (int i = 0; i < cptlt.Count - 1; i++)
            //{
            //    CEdge pEdge = new CEdge(cptlt[i], cptlt[i + 1]);
            //    CEdgeLt.Add(pEdge);
            //}

            //CTriangulator OptCDT = new CTriangulator();
            //List<CTriangle> CDTLt = OptCDT.CreateCDT(pParameterVariable);

            ////for (int i = 0; i < CDTLt.Count; i++) CDTLt[i].TID = i;  //到此为止，约束三角形建立完成，各三角形不再发生变化，将各三角形编号           
            //OptCDT.GetSETriangle(ref CDTLt);  //确定共边三角形
            //OptCDT.ConfirmTriangleSide(ref CDTLt, CEdgeLt); //确定各三角形位于折线的左右边
            //OptCDT.SignTriTypeAll(ref CDTLt);   //标记I、II、III、VI类三角形

            //pLeftBendForest = OptCDT.BuildBendForestNeed2(ref CDTLt, cptlt, "Left");
            //pRightBendForest = OptCDT.BuildBendForestNeed2(ref CDTLt, cptlt, "Right");

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
