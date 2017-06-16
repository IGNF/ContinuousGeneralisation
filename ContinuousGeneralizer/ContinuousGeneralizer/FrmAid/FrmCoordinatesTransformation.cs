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
using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmCoordinatesTransformation : Form
    {
        private CFrmOperation _FrmOperation;
        
        
        CDataRecords _DataRecords;
        protected object _Missing = Type.Missing;

        public FrmCoordinatesTransformation()
        {
            InitializeComponent();
        }




        public FrmCoordinatesTransformation(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmCoordinatesTransformation_Load(object sender, EventArgs e)
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
            double dblLongtitudeDegree = Convert.ToDouble(this.txtLongtitudeDegree.Text);
            //double dblLongtitudeMinute = Convert.ToDouble(this.txtLongtitudeMinute .Text);
            //double dblLongtitudeSecond = Convert.ToDouble(this.txtLongtitudeSecond .Text);
            double dblLatitudeDegree = Convert.ToDouble(this.txtLatitudeDegree.Text);
            //double dblLatitudeMinute = Convert.ToDouble(this.txtLatitudeMinute.Text);
            //double dblLatitudeSecond = Convert.ToDouble(this.txtLatitudeSecond.Text);
            double dblX = Convert.ToDouble(this.txtX.Text);
            double dblY = Convert.ToDouble(this.txtY.Text);
            double dblFactorX = Convert.ToDouble(this.txtFactorX.Text);
            double dblFactorY = Convert.ToDouble(this.txtFactorY.Text);

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

            }
            else if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline))
            {
                List<CPolyline> CPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pFeatureLayer);
                List<CPolyline> CPlLtNew = new List<CPolyline>(CPlLt.Count);
                for (int i = 0; i < CPlLt.Count; i++)
                {
                    List<CPoint> newcptlt = new List<CPoint>(CPlLt[i].CptLt.Count);
                    for (int j = 0; j < CPlLt[i].CptLt.Count; j++)
                    {
                        double dblnewX = dblX + (CPlLt[i].CptLt[j].X - dblLongtitudeDegree) * dblFactorX;
                        double dblnewY = dblY + (CPlLt[i].CptLt[j].Y - dblLatitudeDegree) * dblFactorY;
                        CPoint newcpt = new CPoint(j, dblnewX, dblnewY);
                        newcptlt.Add(newcpt);
                    }
                    CPolyline newcpl = new CPolyline(i, newcptlt);
                    CPlLtNew.Add(newcpl);
                }

                CHelpFunc.SaveCPlLt(CPlLtNew, pFeatureLayer.Name + "_Transformed", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

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
                //CPolygon newcpg = new CPolygon(i, newcptlt);
                //newcpg.SetPolygonAndEdge();
                //    CPolygonLtNew.Add(newcpg);
                //}

                //CHelpFunc.SaveCPolygons(CPolygonLtNew, pFeatureLayer.Name + "_Transformed", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            }



            long lngEndTime = System.Environment.TickCount;//记录结束时间
            _DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时























        }


    }
}
