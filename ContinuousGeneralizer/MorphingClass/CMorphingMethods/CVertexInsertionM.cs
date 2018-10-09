using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
namespace MorphingClass.CMorphingMethods
{
    public class CVertexInsertionM
    {
        private CPolyline _FromCpl;
        private CPolyline _ToCpl;

        
        private CParameterInitialize _ParameterInitialize;
        private CParameterResult _ParameterResult;

        public CVertexInsertionM(CParameterInitialize ParameterInitialize)
        {
            //获取当前选择的点要素图层
            IFeatureLayer pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLayer.SelectedIndex);
                                                                      
            ParameterInitialize.pFeatureLayer = pFeatureLayer;
            _ParameterInitialize = ParameterInitialize;

            //获取线数组
            List<CPolyline> CPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pFeatureLayer);

            //线段长较长的为大比例尺地图上的线段
            if (CPlLt[0].pPolyline.Length >= CPlLt[1].pPolyline.Length)
            {
                _FromCpl = CPlLt[0];
                _ToCpl = CPlLt[1];
            }
            else
            {
                _FromCpl = CPlLt[1];
                _ToCpl = CPlLt[0];
            }
        }

        public void VIMorphing()
        {
            //CPolyline frcpl = _FromCpl;
            //CPolyline tocpl = _ToCpl;

            //CVertexInsertionA pVertexInsertionA = new CVertexInsertionA();
            //List<CPoint> ResultPtLt = pVertexInsertionA.CVI(frcpl, tocpl);
            //CHelpFunc.SaveCorrespondLine(ResultPtLt, "VICorrLine", _ParameterInitialize.pWorkspace, _ParameterInitialize.m_mapControl);

            ////获取结果，全部记录在_DataRecords中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.FromCpl = _FromCpl;
            //ParameterResult.ToCpl = _ToCpl;
            //ParameterResult.CResultPtLt = ResultPtLt;
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
