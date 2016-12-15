using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

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
using MorphingClass.CMorphingMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace MorphingClass.CGeneralizationMethods
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Douglas-Peucker线综合算法</remarks>
    public class CDP
    {


        private CParameterResult _ParameterResult;

        private List<CPolyline> _CPlLt;

        private CMPBDPBL _pMPBDPBL = new CMPBDPBL();
        private CParameterInitialize _ParameterInitialize;

        public CDP()
        {

        }

        public CDP(CParameterInitialize ParameterInitialize)
        {

            //获取当前选择的点要素图层
            IFeatureLayer pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLayer.SelectedIndex);

            ParameterInitialize.pFeatureLayer = pFeatureLayer;
            _ParameterInitialize = ParameterInitialize;

            //获取线数组
            _CPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pFeatureLayer);

        }

        //public void DPGeneralization()
        //{
        //   CPolyline cpl = _CPlLt[0];
        //    CParameterInitialize pParameterInitialize = _ParameterInitialize;           
        //    _pMPBDPBL.DivideCplByDP(cpl);

        //    //获取结果，全部记录在_ParameterResult中
        //    List<CPolyline> cresultpllt = new List<CPolyline>();
        //    cresultpllt.Add(cpl);
        //    CParameterResult ParameterResult = new CParameterResult();
        //    ParameterResult.CResultPlLt = cresultpllt;
        //    _ParameterResult = ParameterResult;
        //}

        ///// <summary>
        ///// 显示并返回单个插值线段
        ///// </summary>
        ///// <param name="pDataRecords">数据记录</param>
        ///// <param name="dblDPBound">阈值参数</param>
        ///// <returns>插值线段</returns>
        //public CPolyline DisplayInterpolation(CDataRecords pDataRecords, double dblDPBound)
        //{
        //    CPolyline cpl = _CPlLt[0];
        //    CPolyline resultcpl = _pMPBDPBL.GetCplByDP(cpl, dblDPBound);

        //    // 清除绘画痕迹
        //    IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
        //    IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
        //    pGra.DeleteAllElements();
        //    m_mapControl.ActiveView.Refresh();
        //    CHelperFunction.ViewPolyline(m_mapControl, resultcpl);  //显示生成的线段
        //    return resultcpl;
        //}

        /////// <summary>
        /////// 显示并返回单个插值线段
        /////// </summary>
        /////// <param name="pDataRecords">数据记录</param>
        /////// <param name="dblProportion">差值参数</param>
        /////// <returns>插值线段</returns>
        ////public CPolyline DisplayInterpolation(CDataRecords pDataRecords, int intResidualNum)
        ////{
        ////    CPolyline cpl = _CPlLt[0];
        ////    CPolyline resultcpl = _pMPBDPBL.GetCplByDP(cpl, dblDPBound);

        ////    // 清除绘画痕迹
        ////    IMapControl4 m_mapControl = pDataRecords.ParameterInitialize.m_mapControl;
        ////    IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
        ////    pGra.DeleteAllElements();
        ////    m_mapControl.ActiveView.Refresh();
        ////    CHelperFunction.ViewPolyline(m_mapControl, resultcpl);  //显示生成的线段
        ////    return resultcpl;
        ////}

        //public CPolyline GetCplByDP(CPolyline cpl, double dblDPBound)
        //{
        //    CPolyline resultcpl = _pMPBDPBL.GetCplByDP(cpl, dblDPBound);
        //    int intptnum = resultcpl.CptLt.Count;
        //    return resultcpl;
        //}



        public void DivideCplByDP(CPolyline dcpl, CVirtualPolyline pVtPl)
        {
            List<CPoint> dcptlt = dcpl.CptLt;
            if ((pVtPl.intToID - pVtPl.intFrID) < 2)
            {
                return;
            }

            //找到距离基础边最远的点
            CEdge pEdge = new CEdge(dcptlt[pVtPl.intFrID], dcptlt[pVtPl.intToID]);
            double dblMaxDis = -1;
            int intMaxDisID = 0;
            double dblRatioLocationAlongBaseline = 0;
            double dblAlongDis = 0;
            double dblFromDis = 0;
            bool isMaxDisCptRightSide = false;
            IPoint outipt = new PointClass();
            bool blnright = new bool();
            for (int i = pVtPl.intFrID + 1; i < pVtPl.intToID; i++)
            {
                dblFromDis = CGeometricMethods.CalDisPointToLine(dcptlt[pVtPl.intFrID], dcptlt[pVtPl.intToID], dcptlt[i]);
                if (dblFromDis > dblMaxDis)
                {
                    dblMaxDis = dblFromDis;
                    intMaxDisID = i;
                    dblRatioLocationAlongBaseline = dblAlongDis;
                    isMaxDisCptRightSide = blnright;
                }
                //outipt.SetEmpty();
            }


            //分别对左右子边执行分割操作
            pVtPl.intMaxDisID = intMaxDisID;
            pVtPl.dblMaxDis = dblMaxDis;
            pVtPl.dblRatioLocationAlongBaseline = dblRatioLocationAlongBaseline;
            pVtPl.isMaxDisCptRightSide = isMaxDisCptRightSide;
            pVtPl.DivideByID(intMaxDisID);

            DivideCplByDP(dcpl, pVtPl.CLeftPolyline);
            DivideCplByDP(dcpl, pVtPl.CRightPolyline);
        }


        public void RecursivelyCollectMaxDis(CVirtualPolyline pVtPl, ref SortedList<double, int> dblMaxDisSLt)
        {
            if (pVtPl.CLeftPolyline == null)
            {
                return;
            }
            dblMaxDisSLt.Add(pVtPl.dblMaxDis, pVtPl.intMaxDisID);
            RecursivelyCollectMaxDis(pVtPl.CLeftPolyline, ref dblMaxDisSLt);
            RecursivelyCollectMaxDis(pVtPl.CRightPolyline, ref dblMaxDisSLt);
        }

        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
