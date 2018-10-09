using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

using ContinuousGeneralizer;
using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmDeletePointsWithSameCoordinates : Form
    {

        
        
        CDataRecords _DataRecords;

        public FrmDeletePointsWithSameCoordinates()
        {
            InitializeComponent();
        }




        public FrmDeletePointsWithSameCoordinates(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmDeletePointsWithSameCoordinates_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            ParameterInitialize.cboLayer = this.cboLayer;

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            string strSelectedLayer = this.cboLayer.Text;
            IFeatureLayer pFeatureLayer = null;

            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            //get the selected features layers
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
                MessageBox.Show("Please select a feature layer");
                return;
            }

            //dialogue for saving
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            string strPath = SFD.FileName;
            //string strName=SFD.
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strPath);


            long lngStartTime = System.Environment.TickCount; //record the start time


            if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint))
            {
                var cptlt = CHelpFunc.GetCPtLtFromPointFeatureLayer(pFeatureLayer);
                var CorrCptsLt = CGeoFunc.LookingForNeighboursByGrids(cptlt, 0);
               foreach (var cpt in cptlt)
               {
                   cpt.isCtrl = true;
                   cpt.isTraversed = false;
               }
               foreach (var pCorrCpts in CorrCptsLt)
               {
                   if (pCorrCpts.FrCpt .isTraversed ==false && pCorrCpts.ToCpt .isTraversed ==false)
                   {
                       pCorrCpts.ToCpt.isCtrl = false;
                   }
                   else
                   {
                       pCorrCpts.FrCpt.isCtrl = false;
                       pCorrCpts.ToCpt.isCtrl = false;
                   }
                   pCorrCpts.FrCpt.isTraversed = true;
                   pCorrCpts.ToCpt.isTraversed = true;
               }

                var resultcptlt = new List<CPoint>(cptlt.Count );
               foreach (var cpt in cptlt)
               {
                   if (cpt.isCtrl == true)
                   {
                       resultcptlt.Add(cpt);
                   }
               }
               //CSaveFeature.SaveCGeoEb(resultcptlt, esriGeometryType.esriGeometryPoint, "CptLtWithoutSameXY", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            }
            else if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline))
            {
                //List<CPolyline> CPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pFeatureLayer);
                //List<CPolyline> CPlLtNew = new List<CPolyline>();
                //for (int i = 0; i < CPlLt.Count; i++)
                //{
                //    List<CPoint> newcptlt = new List<CPoint>();
                //    for (int j = 0; j < CPlLt[i].CptLt.Count; j++)
                //    {
                //        double dblnewX = CPlLt[i].CptLt[j].X * dblEnlargementFactor;
                //        double dblnewY = CPlLt[i].CptLt[j].Y * dblEnlargementFactor;
                //        CPoint newcpt = new CPoint(j, dblnewX, dblnewY);
                //        newcptlt.Add(newcpt);
                //    }
                //    CPolyline newcpl = new CPolyline(i, newcptlt);
                //    CPlLtNew.Add(newcpl);
                //}

                //CHelpFunc.SaveCPlLt(CPlLtNew, pFeatureLayer.Name + "_Enlarged", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            }
            else if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon))
            {
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

                //CHelpFunc.SaveCPolygons(CPolygonLtNew, pFeatureLayer.Name + "_Enlarged", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            }



            long lngEndTime = System.Environment.TickCount;//记录结束时间
            _DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时
        }


    }
}
