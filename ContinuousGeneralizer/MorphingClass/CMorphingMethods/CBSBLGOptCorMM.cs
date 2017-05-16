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
    public class CBSBLGOptCorMM
    {
        
        
        private CParameterResult _ParameterResult;

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private CParameterInitialize _ParameterInitialize;

        public CBSBLGOptCorMM()
        {

        }

        public CBSBLGOptCorMM(CPolyline frcpl, CPolyline tocpl)
        {
            _FromCpl = frcpl;
            _ToCpl = tocpl;
        }

        public CBSBLGOptCorMM(CParameterInitialize ParameterInitialize)
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

        //基于弯曲的Morphing方法（删除小孩子弯曲）
        public void BSBLGOptCorMMMorphing()
        {
            //CParameterInitialize ParameterInitialize = _ParameterInitialize;
            //CPolyline frcpl = _LSCPlLt[0];
            //CPolyline tocpl = _SSCPlLt[0];
            //CHelpFunc.PreviousWork(ref frcpl, ref tocpl);

            ////计算极小值
            //
            //long lngStartTime = System.Environment.TickCount;  //开始时间
                       
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
            //C5.LinkedList<CCorrespondSegment> BSCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            //BSCorrespondSegmentLk.AddRange(OptMPBBSL.DWByMPBBSL(ParameterInitialize, pParameterVariableFrom, pParameterVariableTo, ParameterThreshold));

            ////**************BLG树处理过程**************//
            //CMPBDPBL OptMPBDPBL = new CMPBDPBL();
            //C5.LinkedList<CCorrespondSegment> BLGCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            //for (int j = 0; j < BSCorrespondSegmentLk.Count; j++)
            //{
            //    BLGCorrespondSegmentLk.AddRange(OptMPBDPBL.DWByDPLADefine(BSCorrespondSegmentLk[j].CFrPolyline, BSCorrespondSegmentLk[j].CToPolyline, ParameterThreshold));
            //}

            ////**************OptCorMM处理过程**************//
            //COptCorMM OptOptCorMM = new COptCorMM();
            //C5.LinkedList<CCorrespondSegment> OptCorMMCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            //for (int j = 0; j < BLGCorrespondSegmentLk.Count; j++)
            //{
            //    OptCorMMCorrespondSegmentLk.AddRange(OptOptCorMM.DWByOptCor(BLGCorrespondSegmentLk[j].CFrPolyline, BLGCorrespondSegmentLk[j].CToPolyline, ParameterInitialize.intMaxBackK));
            //}


            ////按指定方式对对应线段进行点匹配，提取对应点
            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
            //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(OptCorMMCorrespondSegmentLk, "Linear");

            ////保存对应线
            //CHelpFunc.SaveCtrlLine(OptCorMMCorrespondSegmentLk, "BSBLGOptCorMMControlLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCorrespondLine(pResultPtLt, "BSBLGOptCorMMCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////计算并显示运行时间
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //_ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = frcpl;
            //ParameterResult.ToCpl = tocpl;
            //ParameterResult.pCorrespondSegmentLk = OptCorMMCorrespondSegmentLk;
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