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
    /// 基于弯曲结构的线状要素Morphing变换方法（以长度为匹配依据：Length），并进一步引用BLG树的方法
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CMPBBSLDP
    {
        
        
        private CParameterResult _ParameterResult;
        private CTriangulator _Triangulator = new CTriangulator();

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private CParameterInitialize _ParameterInitialize;

        public CMPBBSLDP(CPolyline frcpl, CPolyline tocpl)
        {
            _FromCpl = frcpl;
            _ToCpl = tocpl;
        }

        public CMPBBSLDP(CParameterInitialize ParameterInitialize)
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
            _LSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pBSFLayer);
            _SSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pSSFLayer);
        }

        //基于弯曲的Morphing方法（删除小孩子弯曲）
        public void MPBBSLDPMorphing()
        {
            //CParameterInitialize ParameterInitialize = _ParameterInitialize;
            //CMPBBSL OptMPBBSL = new CMPBBSL();
            ////OptMPBBSL.ParameterInitialize = ParameterInitialize;
            //CGeometricMethods.SetCPlScaleEdgeLengthPtBelong(ref _LSCPlLt, CEnumScale.Larger);
            //CGeometricMethods.SetCPlScaleEdgeLengthPtBelong(ref _SSCPlLt, CEnumScale.Smaller);
            //CPolyline frcpl = _LSCPlLt[0];
            //CPolyline tocpl = _SSCPlLt[0];

            ////计算极小值
            // 
            //long lngStartTime = System.Environment.TickCount;  //开始时间



            //////计算阈值参数
            ////double dblBound = 0.94;
            ////CParameterThreshold ParameterThreshold = new CParameterThreshold();
            ////ParameterThreshold.dblFrLength = frcpl.pPolyline.Length;
            ////ParameterThreshold.dblToLength = tocpl.pPolyline.Length;
            ////ParameterThreshold.dblDLengthBound = dblBound;
            ////ParameterThreshold.dblULengthBound = 1 / dblBound;

            //////**************弯曲处理过程**************//
            ////CMPBBSL OptMPBBSL = new CMPBBSL();
            ////CParameterVariable pParameterVariableFrom = new CParameterVariable(frcpl, "FromCDT", ParameterInitialize.pBSFLayer, dblVerySmall);
            ////CParameterVariable pParameterVariableTo = new CParameterVariable(tocpl, "ToCDT", ParameterInitialize.pSSFLayer, dblVerySmall);
            ////C5.LinkedList<CCorrespondSegment> pBSCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            ////pBSCorrespondSegmentLk.AddRange(OptMPBBSL.DWByMPBBSL(ParameterInitialize, pParameterVariableFrom, pParameterVariableTo, ParameterThreshold));

            //List<CPoint> frchcptlt = _Triangulator.CreateConvexHullEdgeLt2(frcpl, dblVerySmall);
            //CPolyline frchcpl = new CPolyline(0, frchcptlt);    //大比例尺折线外包多边形线段
            //frchcpl.SetPolyline();

            //List<CPoint> tochcptlt = _Triangulator.CreateConvexHullEdgeLt2(tocpl, dblVerySmall);
            //CPolyline tochcpl = new CPolyline(0, tochcptlt);    //小比例尺折线外包多边形线段
            //tochcpl.SetPolyline();

            ////添加约束数据生成图层，以便于利用AE中的功能(ct:constraint)
            //List<CPolyline> frctcpllt = new List<CPolyline>(); frctcpllt.Add(frcpl); frctcpllt.Add(frchcpl);
            //List<CPolyline> toctcpllt = new List<CPolyline>(); toctcpllt.Add(tocpl); toctcpllt.Add(tochcpl);
            //IFeatureLayer pBSFLayer = CHelperFunction.SaveCPlLt(frctcpllt, "frctcpllt", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //IFeatureLayer pSSFLayer = CHelperFunction.SaveCPlLt(toctcpllt, "toctcpllt", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            //////**************弯曲处理过程**************//
            ////建立CDT并获取弯曲森林
            //CBendForest FromLeftBendForest = new CBendForest();
            //CBendForest FromRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableFrom = new CParameterVariable(frcpl, "FromCDT", pBSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableFrom, ref FromLeftBendForest, ref FromRightBendForest, ParameterInitialize);//获取折线的弯曲森林

            //CBendForest ToLeftBendForest = new CBendForest();
            //CBendForest ToRightBendForest = new CBendForest();
            //CParameterVariable pParameterVariableTo = new CParameterVariable(tocpl, "ToCDT", pSSFLayer, dblVerySmall);
            //OptMPBBSL.GetBendForest(pParameterVariableTo, ref ToLeftBendForest, ref ToRightBendForest, ParameterInitialize);

            ////整理弯曲森林，获得弯曲数组
            //OptMPBBSL.NeatenBendForest(frcpl, FromLeftBendForest);
            //OptMPBBSL.NeatenBendForest(frcpl, FromRightBendForest);
            //OptMPBBSL.NeatenBendForest(tocpl, ToLeftBendForest);
            //OptMPBBSL.NeatenBendForest(tocpl, ToRightBendForest);

            ////double dblSumLength = frcpl.pPolyline.Length + tocpl.pPolyline.Length;
            //CTranslation pTranslation = new CTranslation();
            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();//作用调用对象的函数
            //CMPBDPBL OptMPBDPBL = new CMPBDPBL();//作用

            ////阈值参数
            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //ParameterThreshold.dblFrLength = frcpl.pPolyline.Length;
            //ParameterThreshold.dblToLength = tocpl.pPolyline.Length;
            //ParameterThreshold.dblAngleBound = 0.262;

            //List<double> dblTranslationLt = new List<double>(26);
            //SortedDictionary<double, int> ResultsSlt = new SortedDictionary<double, int>(new CCompareDbl());
            ////循环的方法适合找最优值
            //for (int i = 0; i <= 25; i++)
            //{
            //    double dblBound = 1 - 0.02 * i;
            //    ParameterThreshold.dblDLengthBound = dblBound;
            //    ParameterThreshold.dblULengthBound = 1 / dblBound;

            //    //弯曲树匹配，寻找对应独立弯曲
            //    List<CCorrespondBend> IndependCorrespondBendLt = new List<CCorrespondBend>();
            //    IndependCorrespondBendLt.AddRange(OptMPBBSL.BendTreeMatch2(FromLeftBendForest, ToLeftBendForest, ParameterThreshold));
            //    IndependCorrespondBendLt.AddRange(OptMPBBSL.BendTreeMatch2(FromRightBendForest, ToRightBendForest, ParameterThreshold));

            //    //弯曲匹配，寻找对应弯曲
            //    List<CCorrespondBend> CorrespondBendLt = OptMPBBSL.BendMatch(IndependCorrespondBendLt, ParameterThreshold);

            //    //提取对应线段
            //    //C5.LinkedList<CCorrespondSegment> BSCorrespondSegmentLk = CHelperFunction.DetectCorrespondSegment(frcpl, tocpl, CorrespondBendLt);

            //    //**************BLG树处理过程**************//
            //    //经过BLG树处理后的最终对应线段                
            //    //C5.LinkedList<CCorrespondSegment> BLGCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            //    for (int j = 0; j < BSCorrespondSegmentLk.Count; j++)//AddRange函数作用？？？？？
            //    {
            //        BLGCorrespondSegmentLk.AddAll(OptMPBDPBL.DWByDPLDefine(BSCorrespondSegmentLk[j].CFrPolyline, BSCorrespondSegmentLk[j].CToPolyline, ParameterThreshold));
            //    }

            //    //按指定方式对对应线段进行点匹配，提取对应点                
            //    //List<CPoint> ResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(BLGCorrespondSegmentLk, "Linear");
            //    double dblTranslation = pTranslation.CalTranslation(ResultPtLt);
            //    dblTranslationLt.Add(dblTranslation);

            //    ResultsSlt.Add(dblTranslation, i);
            //}


            ////必须重新算一遍！！！！！！
            ////理由：如果采用SortedList<double, CParameterResult> ResultsSlt = new SortedList<double, CParameterResult>(new CCompareDbl())记录结果，
            ////      则由于基本单位是CPoint（类似调用指针），最后必然影响CParameterResult中的ResultPtLt值
            //int intIndex = ResultsSlt.ElementAt(0).Value;
            //ParameterThreshold.dblDLengthBound = 1 * (1 - 0.02 * intIndex);
            //ParameterThreshold.dblULengthBound = 1 / (1 - 0.02 * intIndex);

            ////ParameterThreshold.dblDLengthBound = 0.96;
            ////ParameterThreshold.dblULengthBound = 1/0.96;

            ////弯曲树匹配，寻找对应独立弯曲
            //List<CCorrespondBend> pIndependCorrespondBendLt = new List<CCorrespondBend>();
            //pIndependCorrespondBendLt.AddRange(OptMPBBSL.BendTreeMatch2(FromLeftBendForest, ToLeftBendForest, ParameterThreshold));
            //pIndependCorrespondBendLt.AddRange(OptMPBBSL.BendTreeMatch2(FromRightBendForest, ToRightBendForest, ParameterThreshold));

            ////弯曲匹配，寻找对应弯曲
            //List<CCorrespondBend> pCorrespondBendLt = OptMPBBSL.BendMatch(pIndependCorrespondBendLt, ParameterThreshold);

            ////提取对应线段
            //C5.LinkedList<CCorrespondSegment> pBSCorrespondSegmentLk = CHelperFunction.DetectCorrespondSegment(frcpl, tocpl, pCorrespondBendLt);

            ////经过BLG树处理后的最终对应线段
            //C5.LinkedList<CCorrespondSegment> pBLGCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            //for (int j = 0; j < pBSCorrespondSegmentLk.Count; j++)
            //{
            //    pBLGCorrespondSegmentLk.AddAll(OptMPBDPBL.DWByDPLDefine(pBSCorrespondSegmentLk[j].CFrPolyline, pBSCorrespondSegmentLk[j].CToPolyline, ParameterThreshold));
            //}

            ////按指定方式对对应线段进行点匹配，提取对应点                
            //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pBLGCorrespondSegmentLk, "Linear"); 

            ////计算并显示运行时间
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //_ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            ////保存对应线
            //CHelperFunctionExcel.ExportDataltToExcel(dblTranslationLt, "translationlt0", _ParameterInitialize.strSavePath);
            //CHelperFunction.SaveCtrlLine(pBLGCorrespondSegmentLk, "BSBLGControlLine", dblVerySmall, _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);
            //CHelperFunction.SaveCorrespondLine(pResultPtLt, "BSBLGCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////获取结果，全部记录在ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = frcpl;
            //ParameterResult.ToCpl = tocpl;
            //double pdblTranslation = pTranslation.CalTranslation(pResultPtLt);
            //ParameterResult.dblTranslation = pdblTranslation;
            //ParameterResult.pCorrespondSegmentLk = pBLGCorrespondSegmentLk;
            //ParameterResult.CResultPtLt = pResultPtLt;
            //ParameterResult.lngTime = lngTime;
            //_ParameterResult = ParameterResult;
        }


        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
