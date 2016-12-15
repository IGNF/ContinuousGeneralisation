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
    /// 多次使用独立弯曲结构(Recursive Independent Bend Structures)
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CRIBSBLG
    {
        
        
        private CTriangulator _Triangulator = new CTriangulator();
        private CParameterResult _ParameterResult;

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private double _dblCDTNum = 1;   //由于本方法要多次创建CDT，为了使各生成的CDT在名字上有所区别，故用此变量

        private CParameterInitialize _ParameterInitialize;

        public CRIBSBLG()
        {

        }

        public CRIBSBLG(CPolyline frcpl, CPolyline tocpl)
        {
            _FromCpl = frcpl;
            _ToCpl = tocpl;
        }

        public CRIBSBLG(CParameterInitialize ParameterInitialize)
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

        //基于弯曲的Morphing方法
        public void RIBSBLGMorphing()
        {
            //CParameterInitialize ParameterInitialize = _ParameterInitialize;
            //CMPBBSL OptMPBBSL = new CMPBBSL();

            //CPolyline frcpl = _LSCPlLt[0];
            //CPolyline tocpl = _SSCPlLt[0];

            //_FromCpl = frcpl;
            //_ToCpl = tocpl;

            ////计算极小值
            //
            //long lngStartTime = System.Environment.TickCount;  //开始时间

            ////计算阈值参数
            //double dblBound = 0.95;
            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //ParameterThreshold.dblAngleBound = 0.262;
            //ParameterThreshold.dblDLengthBound = dblBound;
            //ParameterThreshold.dblULengthBound = 1 / dblBound;
            //ParameterThreshold.dblVerySmall = dblVerySmall;

            ////**************RIBS弯曲处理过程**************//
            //CRIBS OptRIBS = new CRIBS(frcpl, tocpl);
            //C5.LinkedList<CCorrespondSegment> pComplexCorrespondSegmentLk = OptRIBS.DWByRIBSMorphing(ParameterInitialize, ParameterThreshold);

            ////**************BLG树处理过程**************//
            //CMPBDPBL OptMPBDPBL = new CMPBDPBL();//作用
            //C5.LinkedList<CCorrespondSegment> pBLGCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            ////先对整个线段进行处理
            //pBLGCorrespondSegmentLk.AddRange(OptMPBDPBL.DWByDPLADefine(frcpl, tocpl, ParameterThreshold));
            ////再对由弯曲处理阶段获得的对应线段进行分段处理
            //for (int j = 0; j < pComplexCorrespondSegmentLk.Count; j++)
            //{
            //    pBLGCorrespondSegmentLk.AddRange(OptMPBDPBL.DWByDPLADefine(pComplexCorrespondSegmentLk[j].CFrPolyline, pComplexCorrespondSegmentLk[j].CToPolyline, ParameterThreshold));
            //}

            //C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = CHelperFunction.DetectCorrespondSegment(frcpl, tocpl, pBLGCorrespondSegmentLk);

            ////按指定方式对对应线段进行点匹配，提取对应点
            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();
            //List<CPoint> pResultPtLt = new List<CPoint>();
            //pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pCorrespondSegmentLk, "Linear");

            ////保存对应线
            //CHelperFunction.SaveCtrlLine(pCorrespondSegmentLk, "CRIBSBLGControlLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);
            //CHelperFunction.SaveCorrespondLine(pResultPtLt, "CRIBSBLGCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////计算并显示运行时间
            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //_ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间

            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = frcpl;
            //ParameterResult.ToCpl = tocpl;
            //ParameterResult.pCorrespondSegmentLk = pCorrespondSegmentLk;
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
