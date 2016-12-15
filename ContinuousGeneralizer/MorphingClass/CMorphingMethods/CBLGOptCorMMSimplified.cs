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
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingAlgorithms;

namespace MorphingClass.CMorphingMethods
{
    /// <summary>
    /// 递归调用弯曲结构及BLG树结构的方法
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CBLGOptCorMMSimplified
    {
        
        
        private CParameterResult _ParameterResult;

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        private CParameterInitialize _ParameterInitialize;

        public CBLGOptCorMMSimplified()
        {

        }

        public CBLGOptCorMMSimplified(CPolyline frcpl, CPolyline tocpl)
        {
            _FromCpl = frcpl;
            _ToCpl = tocpl;
        }

        public CBLGOptCorMMSimplified(CParameterInitialize ParameterInitialize)
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
            _LSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pBSFLayer);
            _SSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pSSFLayer);
        }

        //基于弯曲的Morphing方法
        public void BLGOptCorMMSimplifiedMorphing()
        {
            //CParameterInitialize ParameterInitialize = _ParameterInitialize;
            //CPolyline frcpl = _LSCPlLt[0];
            //CPolyline tocpl = _SSCPlLt[0];

            ////计算极小值
            //CGeometricMethods.CalDistanceParameters(frcpl);
            //

            //long lngStartTime = System.Environment.TickCount;  //开始时间            

            ////计算阈值参数
            //double dblBound = 0.98;
            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //ParameterThreshold.dblFrLength = frcpl.pPolyline.Length;
            //ParameterThreshold.dblToLength = tocpl.pPolyline.Length;
            //ParameterThreshold.dblDLengthBound = dblBound;
            //ParameterThreshold.dblULengthBound = 1 / dblBound;

            ////**************BLG树处理过程**************//
            ////经过BLG树处理后的最终对应线段
            //CMPBDPBL OptMPBDPBL = new CMPBDPBL();//作用
            //C5.LinkedList<CCorrespondSegment> pBLGCorrespondSegmentLk = OptMPBDPBL.DWByDPLADefine(frcpl, tocpl, ParameterThreshold);

            ////**************OptCorSimplified处理过程**************//           
            //COptCorMMSimplified OptCorMMSimplified = new COptCorMMSimplified();
            //C5.LinkedList<CCorrespondSegment> pOptCorCorrespondSegmentLk = new C5.LinkedList<CCorrespondSegment>();
            //double dblX = tocpl.CptLt[0].X - frcpl.CptLt[0].X;
            //double dblY = tocpl.CptLt[0].Y - frcpl.CptLt[0].Y;
            //CPoint StandardVectorCpt = new CPoint(0, dblX, dblY);
            //for (int j = 0; j < pBLGCorrespondSegmentLk.Count; j++)
            //{
            //    pOptCorCorrespondSegmentLk.AddRange(OptCorMMSimplified.DWByOptCorMMSimplified(pBLGCorrespondSegmentLk[j].CFrPolyline, pBLGCorrespondSegmentLk[j].CToPolyline, ParameterInitialize.intMaxBackK, StandardVectorCpt, dblSmallDis);
            //}

            ////按指定方式对对应线段进行点匹配，提取对应点
            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();//作用调用对象的函数
            //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pOptCorCorrespondSegmentLk, "Linear");

            ////保存对应线
            //CHelperFunction.SaveCtrlLine(pOptCorCorrespondSegmentLk, "BLGOptCorMMSimplifiedControlLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);
            //CHelperFunction.SaveCorrespondLine(pResultPtLt, "BLGOptCorMMSimplifiedCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

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
