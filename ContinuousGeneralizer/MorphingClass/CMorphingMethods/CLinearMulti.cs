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
    /// <summary>CLinearMulti</summary>
    /// <remarks>必须保证所有线段的方向都是从河流的上游指向河流的下游（也可写出更智能的代码进行处理）
    ///          结果存储：
    ///                   河流的对应特征点结果存储在它们自己的“pBSRiver.CResultPtLt中”；
    ///               此外，pCorrespondRiverNet.CResultPtLtLt则存储了对应河网中各河流的对应特征点结果；
    ///               最终，ParameterResult.CResultPtLtLt存储了各对应河网中各河流的对应特征点结果。
    ///          Multi，意为操作两个文件
    ///</remarks>
    public class CLinearMulti
    {
        
        
        private CParameterResult _ParameterResult;

        private List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        private List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CParameterInitialize _ParameterInitialize;

        public CLinearMulti()
        {

        }

        public CLinearMulti(CParameterInitialize ParameterInitialize)
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

        public void LinearMultiMorphing()
        {

            //List<CPolyline> LSCPlLt = _LSCPlLt;
            //List<CPolyline> SSCPlLt = _SSCPlLt;

            //CParameterInitialize pParameterInitialize = _ParameterInitialize;
            //CParameterThreshold pParameterThreshold = new CParameterThreshold();
            //pParameterThreshold.dblBuffer = CHelperFunction.CalBuffer(LSCPlLt, SSCPlLt);                      //计算缓冲区半径大小
            //pParameterThreshold.dblVerySmall = pParameterThreshold.dblBuffer / 200;
            //pParameterThreshold.dblOverlapRatio = pParameterInitialize.dblOverlapRatio;

            ////根据大比例尺表达线数据生成河流数据
            //List<CRiver> CBSRiverLt = new List<CRiver>();
            //for (int i = 0; i < LSCPlLt.Count; i++)
            //{
            //    CRiver pRiver = new CRiver(i, LSCPlLt[i],pParameterThreshold.dblBuffer, CConstants.dblVerySmall);
            //    CBSRiverLt.Add(pRiver);
            //}

            ////根据小比例尺表达线数据生成河流数据
            //List<CRiver> CSSRiverLt = new List<CRiver>();
            //for (int i = 0; i < SSCPlLt.Count; i++)
            //{
            //    CRiver pRiver = new CRiver(i, SSCPlLt[i], pParameterThreshold.dblBuffer, CConstants.dblVerySmall);
            //    CSSRiverLt.Add(pRiver);
            //}

            //List<CCorrespondRiver> pCorrespondRiverLt = FindCorrespondRiverLt(CBSRiverLt, CSSRiverLt, pParameterThreshold);
            ////CLinear 

            //List<List<CPoint>> CResultPtLtLt = new List<List<CPoint>>();
            //CLinearInterpolationA pLinearInterpolation = new CLinearInterpolationA();
            //for (int i = 0; i < pCorrespondRiverLt.Count; i++)
            //{
            //    CPolyline frcpl = new CPolyline(pCorrespondRiverLt[i].CFromRiver);
            //    CPolyline tocpl = new CPolyline(pCorrespondRiverLt[i].CToRiver);
            //    List<CPoint> ResultPtLt = pLinearInterpolation.CLI(frcpl, tocpl);
            //    CResultPtLtLt.Add(ResultPtLt);
            //}

            //CHelperFunction.SaveCorrespondLine(CResultPtLtLt, "LinearCorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);


            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.CResultPtLtLt = CResultPtLtLt;
            //_ParameterResult = ParameterResult;

        }


        /// <summary>找对应河流</summary>
        /// <param name="CBSRiverLt">大比例尺表达河流</param>
        /// <param name="CSSRiverLt">小比例尺表达河流</param>
        /// <param name="pParameterThreshold">阈值参数</param>
        /// <returns>对应河流列</returns>
        /// <remarks>对应河流的数据记录在对应河网</remarks>
        private List<CCorrespondRiver> FindCorrespondRiverLt(List<CRiver> CBSRiverLt, List<CRiver> CSSRiverLt, CParameterThreshold pParameterThreshold)
        {
            List<CCorrespondRiver> pCorrespondRiverLt = new List<CCorrespondRiver>();
            for (int i = 0; i < CBSRiverLt.Count; i++)
            {
                for (int j = 0; j < CSSRiverLt.Count; j++)
                {
                    bool blnIsOverlap = CGeometricMethods.IsOverlap(CBSRiverLt[i], CSSRiverLt[j], pParameterThreshold.dblOverlapRatio);
                    if (blnIsOverlap == true)
                    {
                        CBSRiverLt[i].CCorrRiver = CSSRiverLt[j];
                        CSSRiverLt[j].CCorrRiver = CBSRiverLt[i];
                        CCorrespondRiver pCorrespondRiver = new CCorrespondRiver(CBSRiverLt[i], CSSRiverLt[j]);
                        pCorrespondRiverLt.Add(pCorrespondRiver);
                        CSSRiverLt.RemoveAt(j);
                        break;
                    }
                }
            }
            return pCorrespondRiverLt;
        }



        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
