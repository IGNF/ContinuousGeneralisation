using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using C5;
using SCG = System.Collections.Generic;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

using ContinuousGeneralizer;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmGaussianPerturbation: Form
    {
        private CFrmOperation _FrmOperation;


        CDataRecords _DataRecords;
        protected object _Missing = Type.Missing;

        public FrmGaussianPerturbation()
        {
            InitializeComponent();
        }




        public FrmGaussianPerturbation(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmGaussianPerturbation_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            ParameterInitialize.cboLayer = this.cboLayer;

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //获取参数
            double dblFactor = Convert.ToDouble(this.txtFactor.Text);

            //读取图层数据
            string strSelectedLayer = this.cboLayer.Text;
            IFeatureLayer pFeatureLayer = null;
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //获取当前选择的要素图层
            try
            {
                for (int i = 0; i < ParameterInitialize.m_mapFeature.LayerCount; i++)
                {
                    if (strSelectedLayer == ParameterInitialize.m_mapFeature.get_Layer(i).Name)
                    {
                        pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(i);
                    }
                }

            }
            catch (Exception)
            {
                MessageBox.Show("请选择要素图层！");
                return;
            }

            //弹出保存对话框
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            string strPath = SFD.FileName;
            //string strName=SFD.
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strPath);


            long lngStartTime = System.Environment.TickCount; //记录开始时间


            if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint))
            {
                List<CPoint> cptlt = CHelpFunc.GetCPtLtFromPointFeatureLayer(pFeatureLayer);
                if (dblFactor == -1)
                {
                    CEnvelope pEnvelope = CGeoFunc.GetEnvelope(cptlt);
                    dblFactor = Math.Sqrt(pEnvelope.Width * pEnvelope.Height / Convert.ToDouble(cptlt.Count)); //Average Distance
                    this.txtAverageDis.Text = dblFactor.ToString();
                }
                double dblFactor3 = dblFactor / 6;  //suppose that 3*StandardDeviation = dblAverageDis/2, where StandardDeviation = 1 (Notice that dblFactor3 is not StandardDeviation)


                List<CPoint> GPcptlt = new List<CPoint>(cptlt.Count);  //cptlt by Gaussian Perturbation
                Random rand = new Random();
                foreach (CPoint cpt in cptlt)
                {
                    double dblGPX, dblGPY;
                    CMathStatistic.BoxMuller(rand, out dblGPX, out dblGPY);
                    CPoint gpcpt = new CPoint(cpt.ID, cpt.X + dblFactor3 * dblGPX, cpt.Y + dblFactor3 * dblGPY);
                    gpcpt.SetPoint();
                    GPcptlt.Add(gpcpt);
                }
                //CSaveFeature.SaveCGeoEb(GPcptlt, esriGeometryType.esriGeometryPoint, "GPcptlt", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);


                for (int i = 0; i < 10; i++)
                {
                    double dblProbalibity = (i + 1) * 0.1;
                    List<CPoint> subcptlt, gpsubcptlt;
                    CreateSubSet(cptlt, GPcptlt, dblProbalibity, rand, out subcptlt, out gpsubcptlt);

                    subcptlt.AddRange(gpsubcptlt);
                    //CHelpFunc.SaveESRIObjltfast(subcptlt, esriGeometryType.esriGeometryPoint, "MixedPoint" + i.ToString(), ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
                    //CSaveFeature.SaveCGeoEb(subcptlt, esriGeometryType.esriGeometryPoint, "MixedPoint" + i.ToString() + "_" + subcptlt.Count.ToString(), ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

                }















                //if (dblFactor ==-1)
                //{                    
                //    C5.LinkedList<CCorrCpts> CCorrCptsLk = CGeoFunc.LookingForNeighboursByGrids(cptlt, 0.00003);
                //    double dblMinDis = cptlt[0].DistanceTo(cptlt[1]);
                //    foreach (CCorrCpts pCorrCpts in CCorrCptsLk)
                //    {
                //        double dblDis = pCorrCpts.FrCpt.DistanceTo(pCorrCpts.ToCpt);
                //        if (dblDis==0)
                //        {
                //            int ss = 5;
                //        }
                //        if (dblDis < dblMinDis && dblDis>0)
                //        {
                //            dblMinDis = dblDis;
                //        }
                //    }
                //    dblFactor = dblMinDis;
                //}






            }
            else if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline))
            {
                //List<CPolyline> CPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pFeatureLayer);
                //List<CPolyline> CPlLtNew = new List<CPolyline>(CPlLt.Count);
                //for (int i = 0; i < CPlLt.Count; i++)
                //{
                //    List<CPoint> newcptlt = new List<CPoint>(CPlLt[i].CptLt.Count);
                //    for (int j = 0; j < CPlLt[i].CptLt.Count; j++)
                //    {
                //        double dblnewX = dblX + (CPlLt[i].CptLt[j].X - dblLongtitudeDegree) * dblFactorX;
                //        double dblnewY = dblY + (CPlLt[i].CptLt[j].Y - dblLatitudeDegree) * dblFactorY;
                //        CPoint newcpt = new CPoint(j, dblnewX, dblnewY);
                //        newcptlt.Add(newcpt);
                //    }
                //    CPolyline newcpl = new CPolyline(i, newcptlt);
                //    CPlLtNew.Add(newcpl);
                //}

                //CHelpFunc.SaveCPlLt(CPlLtNew, pFeatureLayer.Name + "_Transformed", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            }
            else if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon))
            {
                //----------------------------------------------------------------------------------------------//
                //-------------------------------王航请注意---代码写在这里---------------------------------------//
                //----------------------------------------------------------------------------------------------//

                ////获取多边形数组
                //List<CPolygon> CPolygonLt = CHelpFunc.GetCPolygonLtByFeatureLayer(pFeatureLayer);
                //List<CPolygon> CPolygonLtNew = new List<CPolygon>();
                //for (int i = 0; i < CPolygonLt.Count; i++)
                //{
                //    List<CPoint> newcptlt = new List<CPoint>();
                //    for (int j = 0; j < CPolygonLt[i].CptLt.Count; j++)
                //    {
                //        double dblnewX = CPolygonLt[i].CptLt[j].X * dblEnlargementFactor;
                //        double dblnewY = CPolygonLt[i].CptLt[j].Y * dblEnlargementFactor;
                //        CPoint newcpt = new CPoint(j, dblnewX, dblnewY);
                //        newcptlt.Add(newcpt);
                //    }
                //    CPolygon newcpg = new CPolygon(i, newcptlt);
                //    CPolygonLtNew.Add(newcpg);
                //}

                //CHelpFunc.SaveCPolygons(CPolygonLtNew, pFeatureLayer.Name + "_Transformed", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            }



            long lngEndTime = System.Environment.TickCount;//记录结束时间
            _DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时

            MessageBox.Show("Done!");





















        }


        private void CreateSubSet(List<CPoint> cptlt1, List<CPoint> cptlt2, double dblProbability, Random rand, out List<CPoint> subcptlt1, out List<CPoint> subcptlt2)
        {
            int intPtCount = cptlt1.Count;
            int intSubPtCount = Convert.ToInt32(intPtCount * dblProbability) + 1;
            subcptlt1 = new List<CPoint>(intSubPtCount);
            subcptlt2 = new List<CPoint>(intSubPtCount);
            for (int i = 0; i < intPtCount; i++)
            {
                if (rand.NextDouble() < dblProbability)
                {
                    subcptlt1.Add(cptlt1[i]);
                    subcptlt2.Add(cptlt2[i]);
                }
            }
        }
    }
}
