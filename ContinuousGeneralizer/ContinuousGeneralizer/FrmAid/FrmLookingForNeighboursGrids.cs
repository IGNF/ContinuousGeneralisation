using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Diagnostics;
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
    public partial class FrmLookingForNeighboursGrids : Form
    {
        protected object _Missing = Type.Missing;
        
        CDataRecords _DataRecords;

        public FrmLookingForNeighboursGrids()
        {
            InitializeComponent();
        }




        public FrmLookingForNeighboursGrids(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmLookingForNeighboursGrids_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            ParameterInitialize.cboLayer = this.cboLayer;

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
            //cboFunction.SelectedIndex = 0;
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //string strSelectedLayer = this.cboLayer.Text;
            //double dblThreshold = Convert.ToDouble(this.txtThreshold.Text);
            //IFeatureLayer pFeatureLayer = null;

            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            ////get the selected features layers
            //try
            //{
            //    for (int i = 0; i < ParameterInitialize.m_mapFeature.LayerCount; i++)
            //    {
            //        if (strSelectedLayer == ParameterInitialize.m_mapFeature.get_Layer(i).Name)
            //        {
            //            pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(i);
            //        }
            //    }

            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Please select a feature layer");
            //    return;
            //}

            ////dialogue for saving
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //string strPath = SFD.FileName;
            //ParameterInitialize.strSavePath = strPath;
            //ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strPath);
            //List<CPoint> CptLt = CHelpFunc.GetCPtLtByFeatureLayer(pFeatureLayer);
            //for (int i = 0; i < CptLt.Count; i++)
            //{
            //    CptLt[i].GID = i;
            //    CptLt[i].intTS = new TreeSet<int>();
            //}
            //GC.Collect();
            //long lngStartTime = System.Environment.TickCount; //record the start time          
            //long lngMemory = 0;
            ////dblThreshold = Math.Pow(1.8 / Convert.ToDouble(CptLt.Count), 0.5);//---------------dblThreshold------------------------------------------------//
            ////------------------------------------------------Start------------------------------------------------//
            //Grids(ref CptLt, dblThreshold, ParameterInitialize.m_mapControl.Extent);

            //long lngEndTime = System.Environment.TickCount;//记录结束时间
            //_DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时


            //save links
            //there is a bug to creat feature class based on "esriGeometryType.esriGeometryLine", so i used polyline here.
            //int intCount = 0;
            //List<object> iplobjlt = new List<object>();
            //List<List<double>> dbldetailedltlt = new List<List<double>>();
            //for (int i = 0; i < CptLt.Count; i++)
            //{
            //    IDirectedEnumerable<int> intDE = CptLt[i].intTS.RangeFrom(i);
            //    foreach (int j in intDE)
            //    {
            //        IPointCollection4 pCol = new PolylineClass();
            //        pCol.AddPoint((IPoint)CptLt[i]);
            //        pCol.AddPoint((IPoint)CptLt[j]);
            //        IPolyline5 pPolyline = pCol as IPolyline5;

            //        iplobjlt.Add(pPolyline);
            //        intCount++;


            //        //List<double> dbldetailedlt = new List<double>(6);
            //        //if (CptLt[i].X < CptLt[j].X || (CptLt[i].X == CptLt[j].X && CptLt[i].Y < CptLt[j].Y) || (CptLt[i].X == CptLt[j].X && CptLt[i].Y == CptLt[j].Y && CptLt[i].GID < CptLt[j].GID))
            //        //{
            //        //    dbldetailedlt.Add(CptLt[i].GID); dbldetailedlt.Add(CptLt[i].X); dbldetailedlt.Add(CptLt[i].Y); dbldetailedlt.Add(CptLt[j].GID); dbldetailedlt.Add(CptLt[j].X); dbldetailedlt.Add(CptLt[j].Y);
            //        //}
            //        //else
            //        //{
            //        //    dbldetailedlt.Add(CptLt[j].GID); dbldetailedlt.Add(CptLt[j].X); dbldetailedlt.Add(CptLt[j].Y); dbldetailedlt.Add(CptLt[i].GID); dbldetailedlt.Add(CptLt[i].X); dbldetailedlt.Add(CptLt[i].Y);
            //        //}
            //        //dbldetailedltlt.Add(dbldetailedlt);
            //    }
            //}
            //CHelpFuncExcel.ExportDataltltToExcelSW(dbldetailedltlt, CptLt.Count.ToString() + "_" + dblThreshold.ToString() + "Links", ParameterInitialize.strSavePath);
            //CHelpFunc.SaveESRIObjltfast(iplobjlt, esriGeometryType.esriGeometryPolyline, CptLt.Count.ToString() + "_" + dblThreshold.ToString() + "Links", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }


        private void btnRunMulti_Click(object sender, EventArgs e)
        {

            double dblThreshold = Convert.ToDouble(this.txtThreshold.Text);
            double dblGridSize = Convert.ToDouble(this.txtGridSize.Text);
            IFeatureLayer pFeatureLayer = null;
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            //dialogue for saving
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            string strPath = SFD.FileName;
            ParameterInitialize.strSavePath = strPath;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strPath);

            int intLayerCount = ParameterInitialize.m_mapFeature.LayerCount;
            List<List<double>> OutPutLtLt = new List<List<double>>(intLayerCount);

            //some stupid codes  :-)
            IFeatureLayer pFeatureLayerLast = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(intLayerCount - 1);
            List<CPoint> CptLtLast = CHelpFunc.GetCPtLtFromPointFeatureLayer(pFeatureLayerLast);
            int intPtNumLast = CptLtLast.Count;

            long lngTime = 0;
            long lngStartMemory = 0;
            long lngMemory = 0;
            int intOutPut = 0;

            for (int i = 9; i < intLayerCount; i++)
            {
                List<double> OutPutLt = new List<double>(6);
                lngStartMemory = GC.GetTotalMemory(true);
                //long lngMemory1 = GC.GetTotalMemory(true);    
                pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(i);
                List<CPoint> CptLt = CHelpFunc.GetCPtLtFromPointFeatureLayer(pFeatureLayer);
                //GC.GetTotalMemory(true);
                //long lngMemory2 = GC.GetTotalMemory(true);   
                //for (int j = 0; j < CptLt.Count; j++)
                //{
                //    CptLt[j].GID = j;
                //    //CptLt[j].intTS = new TreeSet<int>();
                //}
                //long lngMemory3 = GC.GetTotalMemory(true);   
                //dblThreshold = Math.Pow(1.8 / Convert.ToDouble(CptLt.Count), 0.5);//---------------dblThreshold------------------------------------------------//


                //222222222222222222
                lngTime = 0; lngMemory = lngStartMemory; intOutPut = 0;
                double dblThreshold2 = dblThreshold;
                double dblGridSize2 = dblGridSize * Math.Pow(Convert.ToDouble(intPtNumLast) / Convert.ToDouble(CptLt.Count), 0.5);
                Grids(ref CptLt, dblThreshold2, dblGridSize2, ref lngTime, ref lngMemory, ref intOutPut);
                OutPutLt.Add(Convert.ToDouble(lngTime) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 1000);

                //3333333333333333
                lngTime = 0; lngMemory = lngStartMemory; intOutPut = 0;
                double dblThreshold3 = dblThreshold * Math.Pow(Convert.ToDouble(intPtNumLast) / Convert.ToDouble(CptLt.Count), 0.5);
                double dblGridSize3 = dblGridSize * Math.Pow(Convert.ToDouble(intPtNumLast) / Convert.ToDouble(CptLt.Count), 0.5);
                Grids(ref CptLt, dblThreshold3, dblGridSize3, ref lngTime, ref lngMemory, ref intOutPut);
                OutPutLt.Add(Convert.ToDouble(lngTime) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 1000);

                //dblTimeLt.Add(lngTime);
                //lngMemoryLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                ////lngStartMemoryLt.Add(lngStartMemory);

                ////int intOutPut = 0;
                ////for (int j = 0; j < CptLt.Count; j++)
                ////{
                ////    intOutPut += CptLt[j].intTS.Count;
                ////}
                //dblOutPutLt.Add(intOutPut);

                OutPutLtLt.Add(OutPutLt);
                ParameterInitialize.tspbMain.Value = (i + 1) * 100 / intLayerCount;
                pFeatureLayer = null;
                CptLt = null;
                OutPutLt = null;
            }

            CHelpFuncExcel.ExportDataltltToExcel(OutPutLtLt, "Time&Memory&Output", ParameterInitialize.strSavePath);

          

            MessageBox.Show("Done!");


        }




        private void Grids(ref List<CPoint> cptlt, double dblThreshold, double dblGridSize, ref long lngTime, ref long lngMemory, ref int intOutPut)
        {
            lngTime = System.Environment.TickCount;
            CEnvelope pEnvelope =CGeoFunc . GetEnvelope(cptlt);
            
            dblGridSize = Math.Max(dblGridSize, dblThreshold);
            int intRow = Convert.ToInt32(Math.Truncate(pEnvelope.Height / dblGridSize)) + 1;  //+1, so that the bordered point can be covered
            int intCol= Convert.ToInt32(Math.Truncate(pEnvelope.Width / dblGridSize)) + 1;   //+1, so that the bordered point can be covered

            long lngTime2 = System.Environment.TickCount;
            SCG.LinkedList<int>[,] aintLLtGridContent = new SCG.LinkedList<int>[intRow, intCol];
            for (int i = 0; i < intRow; i++)
            {
                for (int j = 0; j < intCol; j++)
                {
                    aintLLtGridContent[i, j] = new SCG.LinkedList<int>();
                }
            }
            lngTime2 = System.Environment.TickCount - lngTime2;

            long lngTime3 = System.Environment.TickCount;
            foreach (CPoint cpt in cptlt)
            {
                int rownum = Convert.ToInt32(Math.Truncate((cpt.Y - pEnvelope.YMin ) / dblGridSize));
                int colnum = Convert.ToInt32(Math.Truncate((cpt.X - pEnvelope.XMin) / dblGridSize));
                aintLLtGridContent[rownum, colnum].AddLast(cpt.GID);
            }

            for (int i = 0; i < intRow; i++)
            {
                for (int j = 0; j < intCol; j++)
                {
                    LookingForNeighboursInGridItself(ref cptlt, aintLLtGridContent[i, j], dblThreshold, ref intOutPut);

                    if (j + 1 < intCol)  //Right
                    {
                        LookingForNeighboursInGrids(ref cptlt, aintLLtGridContent[i, j], aintLLtGridContent[i, j + 1], dblThreshold, ref intOutPut);
                    }

                    if (i + 1 < intRow) //Upper
                    {
                        if (j - 1 >= 0)  //UpperLeft
                        {
                            LookingForNeighboursInGrids(ref cptlt, aintLLtGridContent[i, j], aintLLtGridContent[i + 1, j - 1], dblThreshold, ref intOutPut);
                        }
                        //UpperMiddle
                        LookingForNeighboursInGrids(ref cptlt, aintLLtGridContent[i, j], aintLLtGridContent[i + 1, j], dblThreshold, ref intOutPut);

                        if (j + 1 < intCol)  //UpperRight
                        {
                            LookingForNeighboursInGrids(ref cptlt, aintLLtGridContent[i, j], aintLLtGridContent[i + 1, j + 1], dblThreshold, ref intOutPut);
                        }
                    }
                }
            }
            lngTime3 = System.Environment.TickCount - lngTime3;
            lngTime = System.Environment.TickCount - lngTime;
            lngMemory = GC.GetTotalMemory(true) - lngMemory;
            ////Dispose
            //for (int i = 0; i < intRow; i++)
            //{
            //    for (int j = 0; j < intCol; j++)
            //    {
            //        aintLLtGridContent[i, j].Dispose();
            //    }

            //}

        }

        private void LookingForNeighboursInGridItself(ref List<CPoint> cptlt, SCG.LinkedList<int> intLLtGridContent, double dblThreshold, ref int intOutPut)
        {
            if (intLLtGridContent.Count ==0)
            {
                return;
            }
            int intCountI = 0;
            foreach (int intI  in intLLtGridContent)
            {
                int intCountJ =0;
                foreach (int intJ in intLLtGridContent)
                {
                    if (intCountJ>intCountI )
                    {
                        double dblXDiff = Math.Abs(cptlt[intI].X - cptlt[intJ].X);
                        double dblYDiff = Math.Abs(cptlt[intI].Y - cptlt[intJ].Y);
                        if (dblXDiff <= dblThreshold && dblYDiff <= dblThreshold)
                        {
                            //cptlt[intI].intTS.Add(intJ);
                            //cptlt[intJ].intTS.Add(intI);

                            intOutPut++;
                        }
                    }
                    else
                    {
                        intCountJ++;
                    }
                }
                intCountI++;
            }
        }

        private void LookingForNeighboursInGrids(ref List<CPoint> cptlt, SCG.LinkedList<int> intLLtGridContent1, SCG.LinkedList<int> intLLtGridContent2, double dblThreshold, ref int intOutPut)
        {
            if (intLLtGridContent1.Count == 0 || intLLtGridContent1.Count == 0)
            {
                return;
            }

            foreach (int intGID1 in intLLtGridContent1)
            {
                foreach (int intGID2 in intLLtGridContent2)
                {
                    double dblXDiff = Math.Abs(cptlt[intGID1].X - cptlt[intGID2].X);
                    double dblYDiff = Math.Abs(cptlt[intGID1].Y - cptlt[intGID2].Y);
                    if (dblXDiff <= dblThreshold && dblYDiff <= dblThreshold)
                    {
                        //cptlt[intGID1].intTS.Add(intGID2);
                        //cptlt[intGID2].intTS.Add(intGID1);

                        intOutPut++;
                    }
                }
            }
        }
    }
}
