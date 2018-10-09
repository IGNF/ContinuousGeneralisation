using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

using MorphingClass.CCorrepondObjects;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;

using VBClass;

namespace MorphingClass.CMorphingExtend
{
    public class CLandingTimeLI
    {
        public List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        public List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale

        private CDataRecords _DataRecords;                    //records of data
        private CParameterResult _ParameterResult;
        
        
        private CParameterInitialize _ParameterInitialize;

        public CLandingTimeLI()
        {

        }

        public CLandingTimeLI(CParameterInitialize ParameterInitialize)
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

        public CLandingTimeLI(CDataRecords pDataRecords)
        {
            _DataRecords = pDataRecords;
        }

        /// <summary>
        /// 显示并返回单个插值面状要素
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>面状要素</returns>
        public List<CPolyline> GetLandingTimeLICPlLt()
        {
            List<CPolyline> cpllt = _DataRecords.ParameterResult.CInitialPlLt;
            CPoint EndCpt = CalEndCpt(cpllt);
            //缩放比例系数
            double dblRationXT = 0;
            double dblRationYH = 0;
            CalRatios(cpllt, ref dblRationXT,ref dblRationYH);

            List<CPolyline> THcpllt = new List<CPolyline>();
            for (int i = 0; i < cpllt.Count ; i++)
            {
                CPolyline THcpl = GetTHCpl(cpllt[i], EndCpt,dblRationXT ,dblRationYH);
                THcpllt.Add(THcpl);
            }
            //_DataRecords.ParameterResult.pc = THcpllt;


            //CPolyline cpl = CGeoFunc.GetTargetcpl(dblProp);

            //// 清除绘画痕迹
            //IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            //IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            //pGra.DeleteAllElements();
            //CHelpFunc.ViewPolylines(m_mapControl, THcpllt);  //显示生成的线段
            _DataRecords.ParameterResult.CResultPlLt = THcpllt;
            return THcpllt;
        }

        /// <summary>
        /// 计算终点坐标
        /// </summary>
        /// <param name="cpllt">线数组</param>
        /// <returns>终点</returns>
        private CPoint CalEndCpt(List <CPolyline > cpllt)
        {
            double dblSumX = 0;
            double dblSumY = 0;
            for (int i = 0; i < cpllt.Count ; i++)
            {
                dblSumX += cpllt[i].CptLt[cpllt[i].CptLt.Count - 1].X;
                dblSumY += cpllt[i].CptLt[cpllt[i].CptLt.Count - 1].Y;
            }

            double dblEndX = dblSumX / cpllt.Count ;
            double dblEndY = dblSumY / cpllt.Count;

            CPoint cpt = new CPoint(-1, dblEndX, dblEndY);
            return cpt;
        }

        /// <summary>
        /// 计算缩放系数
        /// </summary>
        /// <param name="cpllt">线数组</param>
        /// <param name="dblXT">X坐标与时间的缩放比</param>
        /// <param name="dblYH">Y坐标与高度的缩放比</param>
        private void CalRatios(List<CPolyline> cpllt, ref double dblRationXT, ref double dblRationYH)
        {
            //初始化各值
            double dblMaxX = cpllt[0].CptLt [0].X ;
            double dblMinX = cpllt[0].CptLt[0].X;
            double dblMaxY = cpllt[0].CptLt[0].Y ;
            double dblMinY = cpllt[0].CptLt[0].Y;
            double dblMaxZ = cpllt[0].CptLt[0].Z ;
            double dblMaxDiffTime = cpllt[0].CptLt[cpllt[0].CptLt.Count - 1].dblTime - cpllt[0].CptLt[0].dblTime;

            for (int i = 0; i < cpllt.Count ; i++)
            {
                for (int j = 0; j < cpllt[i].CptLt .Count ; j++)
                {
                    if (cpllt[i].CptLt[j].X >dblMaxX)
                    {
                        dblMaxX = cpllt[i].CptLt[j].X;
                    }

                    if (cpllt[i].CptLt[j].X < dblMinX)
                    {
                        dblMinX = cpllt[i].CptLt[j].X;
                    }

                    if (cpllt[i].CptLt[j].Y > dblMaxY)
                    {
                        dblMaxY = cpllt[i].CptLt[j].Y;
                    }

                    if (cpllt[i].CptLt[j].Y < dblMinY)
                    {
                        dblMinY = cpllt[i].CptLt[j].Y;
                    }

                    if (cpllt[i].CptLt[j].Z > dblMaxZ)
                    {
                        dblMaxZ = cpllt[i].CptLt[j].Z;
                    }
                }

                double dblDiffTime = cpllt[i].CptLt[cpllt[i].CptLt.Count - 1].dblTime - cpllt[i].CptLt[0].dblTime;
                if (dblDiffTime>dblMaxDiffTime)
                {
                    dblMaxDiffTime = dblDiffTime;
                }
            }

            //由于X、Y都可以向两个方向延伸，而T和Z只可以向一个方向延伸，所以要格外除2
            dblRationXT = (dblMaxX - dblMinX) / dblMaxDiffTime;
            dblRationYH = (dblMaxY - dblMinY) / dblMaxZ;

            //dblRationXT = (dblMaxX - dblMinX) / dblMaxDiffTime / 2;
            //dblRationYH = (dblMaxY - dblMinY) / dblMaxZ / 2;
        }

        /// <summary>
        /// 获取飞机高度与时间关系的折线
        /// </summary>
        /// <param name="cpl">待计算线状要素</param>
        /// <param name="EndCpt">终点</param>
        /// <param name="dblXT">X坐标与时间的缩放比</param>
        /// <param name="dblYH">Y坐标与高度的缩放比</param>
        /// <returns>飞机高度与时间关系的折线</returns>
        /// <remarks>TH:Height respect to Time</remarks>
        private CPolyline GetTHCpl(CPolyline cpl, CPoint EndCpt, double dblRationXT, double dblRationYH)
        {
            List<CPoint> cptlt = cpl.CptLt;
            double dblTn = cptlt[cptlt.Count - 1].dblTime;
            double dblHn = cptlt[cptlt.Count - 1].Z;

            //注意：这里生成新坐标的过程中，实际上隐含了坐标系统"TH"到"XY"的变化，这里采用默认变换
            List<CPoint> newcptlt = new List<CPoint>();
            for (int i = 0; i < cptlt.Count; i++)
            {
                double dblnewX = EndCpt.X + (cptlt[i].dblTime - dblTn) * dblRationXT;
                double dblnewY = EndCpt.Y + (cptlt[i].Z - dblHn) * dblRationYH;
                CPoint newcpt = new CPoint(i, dblnewX, dblnewY);
                newcptlt.Add(newcpt);
            }

            CPolyline newcpl = new CPolyline(cpl.ID, newcptlt);
            return newcpl;
        }

        /// <summary>
        /// 确定对应关系（一一对应）
        /// </summary>
        /// <param name="XYCpl">平面XY线状要素</param>
        /// <param name="THCpl">时间高度线状要素</param>
        /// <returns>飞机高度与时间关系的折线</returns>
        /// <remarks>HT:Height respect to Time
        ///          固定倒数两个点</remarks>
        public List<CCorrCpts> GetCorrCptsLt(CPolyline XYCpl,CPolyline THCpl)
        {
            List<CCorrCpts> pCorrCptsLt = new List<CCorrCpts>();
            for (int i = 0; i < XYCpl.CptLt .Count ; i++)
            {
                CCorrCpts pCorrCpts = new CCorrCpts(XYCpl.CptLt[i], THCpl.CptLt[i]);
                pCorrCptsLt.Add(pCorrCpts);
            }

            pCorrCptsLt[pCorrCptsLt.Count - 2].FrCpt.isCtrl = true;
            pCorrCptsLt[pCorrCptsLt.Count - 2].ToCpt.isCtrl = true;
            pCorrCptsLt[pCorrCptsLt.Count - 1].FrCpt.isCtrl = true;
            pCorrCptsLt[pCorrCptsLt.Count - 1].ToCpt.isCtrl = true;
            return pCorrCptsLt;
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
