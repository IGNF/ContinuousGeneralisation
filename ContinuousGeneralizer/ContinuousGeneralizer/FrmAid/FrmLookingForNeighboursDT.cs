using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

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
    public partial class FrmLookingForNeighboursDT : Form
    {

        private CFrmOperation _FrmOperation;
        
        
        CDataRecords _DataRecords;
        protected object _Missing = Type.Missing;

        public FrmLookingForNeighboursDT()
        {
            InitializeComponent();
        }




        public FrmLookingForNeighboursDT(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmLookingForNeighboursDT_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            ParameterInitialize.cboLayer = this.cboLayer;

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //string strSelectedLayer = this.cboLayer.Text;
            //double dblThreshold = Convert.ToDouble(this.txtThreshold.Text);
            //IFeatureLayer pFeatureLayer = null;

            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            ////获取当前选择的要素图层
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
            //    MessageBox.Show("请选择要素图层！");
            //    return;
            //}

            ////弹出保存对话框
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //string strPath = SFD.FileName;
            //ParameterInitialize.strSavePath = strPath;
            //ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strPath);


            //long lngStartTime = System.Environment.TickCount; //记录开始时间
            //long lngTimeForDT = 0;
            //List<CPoint> CptLt = LookingForNeighboursDT(pFeatureLayer, dblThreshold, ref lngTimeForDT);   //*******************Action*******************//
            //long lngEndTime = System.Environment.TickCount;//记录结束时间
            //_DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时

            ////int intOutPut = 0;
            ////for (int j = 0; j < CptLt.Count; j++)
            ////{
            ////    intOutPut += CptLt[j].intTS.Count;
            ////}




            ////save links
            ////there is a bug to creat feature class based on "esriGeometryType.esriGeometryLine", so i used polyline here.
            //int intCount = 0;
            //List<object> iplobjlt = new List<object>();
            //List<List<double>> dbldetailedltlt = new List<List<double>>();
            //for (int i = 0; i < CptLt.Count; i++)
            //{
            //    IDirectedEnumerable<int> intDE = CptLt[i].intTS.RangeFrom(i + 1);
            //    foreach (int j in intDE)
            //    {
            //        IPoint fript = new PointClass(); fript.PutCoords(CptLt[i].pTinNode.X, CptLt[i].pTinNode.Y);
            //        IPoint toipt = new PointClass(); toipt.PutCoords(CptLt[j - 1].pTinNode.X, CptLt[j - 1].pTinNode.Y);

            //        IPointCollection4 pCol = new PolylineClass();
            //        pCol.AddPoint(fript);
            //        pCol.AddPoint(toipt);
            //        IPolyline5 pPolyline = pCol as IPolyline5;

            //        iplobjlt.Add(pPolyline);
            //        fript.SetEmpty();
            //        toipt.SetEmpty();
            //        intCount++;

            //         List<double> dbldetailedlt = new List<double>(6);
            //        if (CptLt[i].pTinNode.X < CptLt[j - 1].pTinNode.X || (CptLt[i].pTinNode.X == CptLt[j - 1].pTinNode.X && CptLt[i].pTinNode.Y < CptLt[j - 1].pTinNode.Y) || (CptLt[i].pTinNode.X == CptLt[j - 1].pTinNode.X && CptLt[i].pTinNode.Y == CptLt[j - 1].pTinNode.Y && CptLt[i].pTinNode.Index < CptLt[j - 1].pTinNode.Index))
            //        {
            //            dbldetailedlt.Add(CptLt[i].pTinNode.Index); dbldetailedlt.Add(CptLt[i].pTinNode.X); dbldetailedlt.Add(CptLt[i].pTinNode.Y); dbldetailedlt.Add(CptLt[j - 1].pTinNode.Index); dbldetailedlt.Add(CptLt[j - 1].pTinNode.X); dbldetailedlt.Add(CptLt[j - 1].pTinNode.Y);
            //        }
            //        else
            //        {
            //            dbldetailedlt.Add(CptLt[j - 1].pTinNode.Index); dbldetailedlt.Add(CptLt[j - 1].pTinNode.X); dbldetailedlt.Add(CptLt[j - 1].pTinNode.Y); dbldetailedlt.Add(CptLt[i].pTinNode.Index); dbldetailedlt.Add(CptLt[i].pTinNode.X); dbldetailedlt.Add(CptLt[i].pTinNode.Y);
            //        }
            //        dbldetailedltlt.Add(dbldetailedlt);
            //    }
            //}
            //CHelpFuncExcel.ExportDataltltToExcelSW(dbldetailedltlt, CptLt.Count.ToString() + "_" + dblThreshold.ToString() + "Links", ParameterInitialize.strSavePath);
            //CHelpFunc.SaveESRIObjltfast(iplobjlt, esriGeometryType.esriGeometryPolyline, CptLt.Count.ToString() + "_" + dblThreshold.ToString() + "Links", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

            ////Dispose&SetEmpty
            //for (int j = 0; j < CptLt.Count; j++)
            //{
            //    CptLt[j].intTS.Dispose();
            //    //CptLt[j].SetEmpty2();
            //    CptLt[j].pTinNode.SetEmpty();
            //}

        }

        private void btnRunMulti_Click(object sender, EventArgs e)
        {
            double dblThreshold = Convert.ToDouble(this.txtThreshold.Text);
            IFeatureLayer pFeatureLayer = null;
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            //弹出保存对话框
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            string strPath = SFD.FileName;
            ParameterInitialize.strSavePath = strPath;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strPath);
            string strTINPath = strPath + "\\TIN";
            string strTINPath2 = strPath + "\\TIN2";
            System.IO.Directory.CreateDirectory(strTINPath);
            System.IO.Directory.CreateDirectory(strTINPath2);
            int intLayerCount = ParameterInitialize.m_mapFeature.LayerCount;
            List<List<double>> OutPutLtLt = new List<List<double>>(intLayerCount);

            //some stupid codes  :-)
            IFeatureLayer pFeatureLayerLast = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(intLayerCount - 1);
            List<CPoint> CptLtLast = CHelpFunc.GetCPtLtFromPointFeatureLayer(pFeatureLayerLast);
            int intPtNumLast = CptLtLast.Count;

            long lngMemory = 0;
            long lngMemoryDT = 0;
            long lngMemoryDTProcess = 0;
            int intOutPut = 0;
            long lngTimeForSearching = 0;
            long lngTimeForDT = 0;
            List<CPoint> CptLt = new List<CPoint>();

            OutPutLtLt = new List<List<double>>(6);
            for (int i = 0; i < intLayerCount; i++)
                //for (int i = 0; i < 2; i++)
            {
                List<double> OutPutLt = new List<double>(8);

                //double ss = intPtNumLast / Convert.ToDouble(159);
                //double kk = Math.Pow(intPtNumLast / Convert.ToDouble(CptLt.Count), 0.5);
                //double dblThreshold2 = dblThreshold * Math.Pow(intPtNumLast / Convert.ToDouble(CptLt.Count), 0.5);  //---------------dblThreshold------------------------------------------------//
                pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(i);
                //long lngMemory2 = GC.GetTotalMemory(true);
                
                lngMemoryDT = 0; lngMemoryDTProcess = 0; intOutPut = 0; lngTimeForSearching = 0; lngTimeForDT = 0;
                CptLt = LookingForNeighboursDT(pFeatureLayer, dblThreshold,-1, ref lngTimeForSearching, ref lngTimeForDT, ref lngMemory, ref lngMemoryDT, ref lngMemoryDTProcess, ref intOutPut, strTINPath);   //*******************Action*******************//
                //long lngMemory3 = GC.GetTotalMemory(true);
                OutPutLt.Add(Convert.ToDouble(lngTimeForSearching + lngTimeForDT) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngTimeForDT) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(lngMemoryDT) / 1048576);
                OutPutLt.Add(Convert.ToDouble(lngMemoryDTProcess) / 1048576);
                //int intOutPut = 0;
                //for (int j = 0; j < CptLt.Count; j++)
                //{
                //    intOutPut += CptLt[j].intTS.Count;
                //}
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 2 / 1000);


                //long lngMemory2 = GC.GetTotalMemory(true);
                lngMemoryDT = 0; lngMemoryDTProcess = 0; intOutPut = 0; lngTimeForSearching = 0; lngTimeForDT = 0;
                CptLt = LookingForNeighboursDT(pFeatureLayer, dblThreshold, intPtNumLast, ref lngTimeForSearching, ref lngTimeForDT, ref lngMemory, ref lngMemoryDT, ref lngMemoryDTProcess, ref intOutPut, strTINPath2);   //*******************Action*******************//
                //long lngMemory3 = GC.GetTotalMemory(true);
                OutPutLt.Add(Convert.ToDouble(lngTimeForSearching) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngTimeForDT) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(lngMemoryDT) / 1048576);
                OutPutLt.Add(Convert.ToDouble(lngMemoryDTProcess) / 1048576);
                //int intOutPut = 0;
                //for (int j = 0; j < CptLt.Count; j++)
                //{
                //    intOutPut += CptLt[j].intTS.Count;
                //}
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 2 / 1000);


                OutPutLtLt.Add(OutPutLt);
                CHelpFunc.Displaytspb(i + 1, intLayerCount);
                pFeatureLayer = null;
                CptLt = null;
                OutPutLt = null;
            }


            CHelpFuncExcel.ExportDataltltToExcel(OutPutLtLt, "Time&Output_ChangeThreshold", ParameterInitialize.strSavePath);


            MessageBox.Show("Done!");
        }


        private List<CPoint> LookingForNeighboursDT(IFeatureLayer pFeatureLayer, double dblThreshold, int intPtNumLast, ref long lngTimeForSearching, ref long lngTimeForDT, ref long lngMemory, ref long lngMemoryDT, ref long lngMemoryDTProcess, ref int intOutPut, string strTINPath)
        {
            lngMemory = GC.GetTotalMemory(true);
            //lngMemoryDT = GC.GetTotalMemory(true);
            //lngMemoryDTProcess = Process.GetProcessesByName("ContinuousGeneralizer.vshost")[0].WorkingSet64;
            long lngStartTime = System.Environment.TickCount; //记录开始时间
            
            //lngMemoryDT = Process.GetProcessesByName("ContinuousGeneralizer.vshost")[0].WorkingSet64;
                       //Process .GetProcessesByName("ContinuousGeneralizer.vshost")[0].
            //create TIN
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            IGeoDataset pGDS = (IGeoDataset)pFeatureClass;
            IEnvelope pEnv = (IEnvelope)pGDS.Extent;
            pEnv.SpatialReference = pGDS.SpatialReference;
            IFields pFields = pFeatureClass.Fields;
            IField pHeightFiled = new FieldClass();

            try
            {
                pHeightFiled = pFields.get_Field(pFields.FindField("Id"));
            }
            catch (Exception)
            {
                pHeightFiled = pFields.get_Field(pFields.FindField("ID"));
                throw;
            }

            //ITinWorkspace
            ITinEdit pTinEdit = new TinClass();
            pTinEdit.InitNew(pEnv);
            object Missing = Type.Missing;

            pTinEdit.AddFromFeatureClass(pFeatureClass, null, pHeightFiled, null, esriTinSurfaceType.esriTinHardLine, ref Missing);

            lngTimeForDT = System.Environment.TickCount - lngStartTime;   //Time for constructing DT
            //long lngMemoryafterTinEdit = GC.GetTotalMemory(false) - lngMemoryDT;
            //GC.Collect();
            //lngMemoryDT = GC.GetTotalMemory(true) - lngMemoryDT;
            //lngMemoryDTProcess = Process.GetProcessesByName("ContinuousGeneralizer.vshost")[0].WorkingSet64 - lngMemoryDTProcess;

            //C5.LinkedList<int> intLLt = new C5.LinkedList<int>();

            
            ITinNodeCollection pTinNodeCollection = (ITinNodeCollection)pTinEdit;
            List<CPoint> CptLt = new List<CPoint>(pTinNodeCollection.NodeCount);
            for (int i = 1; i <= pTinNodeCollection.NodeCount; i++)   //i=1-4: super node
            {
                ITinNode pNode = pTinNodeCollection.GetNode(i);
                CPoint cpt = new CPoint(pNode);
                //cpt.intTS = new C5.TreeSet<int>();
                CptLt.Add(cpt);
            }
            //long lngMemoryafterfetching = GC.GetTotalMemory(true);
            //lngMemoryDT = GC.GetTotalMemory(true) - lngMemoryDT;

            //Looking for neighbours based on Breadth First Search

            if (intPtNumLast !=-1)
            {
                dblThreshold = dblThreshold * Math.Pow(Convert.ToDouble(intPtNumLast) / Convert.ToDouble(CptLt.Count - 4), 0.5);  //---------------dblThreshold------------------------------------------------//
                    
                    //Math.Pow(1.8 / Convert.ToDouble(CptLt.Count - 4), 0.5);//---------------dblThreshold------------------------------------------------//
            }
            //double dblThresholdDT = (1 + Math.Sqrt(2)) / 2 * dblThreshold;

            lngTimeForSearching = System.Environment.TickCount;  //the start time             
             SCG .LinkedList<int> intTargetLLt = new SCG.LinkedList<int>();
             SCG.LinkedList<int> intAllLLt = new SCG.LinkedList<int>();
            for (int i = 0; i < CptLt.Count; i++)
            {
                CptLt[i].isTraversed = true;

                intTargetLLt = new SCG.LinkedList<int>();                
                intTargetLLt.AddLast(CptLt[i].pTinNode .Index);
                intAllLLt = new SCG.LinkedList<int>();
                intAllLLt.AddLast(CptLt[i].pTinNode.Index);

                while (intTargetLLt.Count > 0)
                {
                    intTargetLLt = BSF(CptLt[i], ref CptLt, ref intTargetLLt, ref intAllLLt, dblThreshold, ref intOutPut);
                }
                intOutPut--;  //the point will take itself as a close point, so we have to minus 1
                //CptLt[i].intTS.Remove(CptLt[i].pTinNode.Index);
                RestoreIsTraversed(ref CptLt, ref intAllLLt);
                //intAllLLt.Dispose();
            }
            //long lngMemoryaftersearch = GC.GetTotalMemory(true);
            lngTimeForSearching = System.Environment.TickCount - lngTimeForSearching;  //the result time
            lngMemory = GC.GetTotalMemory(true) - lngMemory;
            pTinEdit.SaveAs(strTINPath +"\\" +pFeatureLayer.Name, true);
            long lngFileSize = CHelpFunc.GetDirectoryLength(strTINPath + "\\" + pFeatureLayer.Name);
            lngMemory += lngFileSize;
            lngMemoryDT = lngFileSize;

            return CptLt;
        }


        //Breadth First Search
        private SCG.LinkedList<int> BSF(CPoint Originalcpt, ref List<CPoint> cptlt, ref SCG.LinkedList<int> intTargetLLt, ref SCG.LinkedList<int> intAllLLt, double dblThreshold, ref int intOutPut)
        {
            SCG.LinkedList<int> intnewLLt = new SCG.LinkedList<int>();
            //double dblThresholdDT = (1 + Math.Sqrt(2)) / 2 * dblThreshold;
            //double dblThresholdDT =  Math.Sqrt(2) * dblThreshold;
            double dblThresholdDT = ((Math.Sqrt(7) + 1) / 2) * dblThreshold;

            foreach (int i in intTargetLLt)
            {
                //CPoint cpt = cptlt[i - 1];
                //double dblXDiff = Math.Abs(cpt.pTinNode.X - Originalcpt.pTinNode.X);
                //double dblYDiff = Math.Abs(cpt.pTinNode.Y - Originalcpt.pTinNode.Y);
                //if (dblXDiff <= dblThreshold && dblYDiff <= dblThreshold)
                //{
                //    intOutPut++;
                //    //Originalcpt.intTS.Add(cpt.pTinNode.Index);   //Note that if one element is already in "intTS", a new element with the same value will not be added and no exception will be thrown
                //    //cpt.intTS.Add(Originalcpt.pTinNode.Index);   //Note that if one element is already in "intTS", a new element with the same value will not be added and no exception will be thrown

                //    ITinNodeArray pTinNodeArray = cpt.pTinNode.GetAdjacentNodes();
                //    for (int j = 0; j < pTinNodeArray.Count; j++)
                //    {
                //        ITinNode pAdjacentNode = pTinNodeArray.get_Element(j);
                //        if (cptlt[pAdjacentNode.Index - 1].isTraversed == false)
                //        {
                //            cptlt[pAdjacentNode.Index - 1].isTraversed = true;
                //            intnewLLt.Add(pAdjacentNode.Index);
                //        }
                //    }
                //}


                CPoint cpt = cptlt[i - 1];
                double dblXDiff = Math.Abs(cpt.pTinNode.X - Originalcpt.pTinNode.X);
                double dblYDiff = Math.Abs(cpt.pTinNode.Y - Originalcpt.pTinNode.Y);
                if (dblXDiff <= dblThreshold && dblYDiff <= dblThreshold)
                {
                    intOutPut++;
                    //Originalcpt.intTS.Add(cpt.pTinNode.Index);   //Note that if one element is already in "intTS", a new element with the same value will not be added and no exception will be thrown
                    //cpt.intTS.Add(Originalcpt.pTinNode.Index);   //Note that if one element is already in "intTS", a new element with the same value will not be added and no exception will be thrown                   
                }

                if (dblXDiff <= dblThresholdDT && dblYDiff <= dblThresholdDT)
                {
                    ITinNodeArray pTinNodeArray = cpt.pTinNode.GetAdjacentNodes();
                    for (int j = 0; j < pTinNodeArray.Count; j++)
                    {
                        ITinNode pAdjacentNode = pTinNodeArray.get_Element(j);
                        if (cptlt[pAdjacentNode.Index - 1].isTraversed == false)
                        {
                            cptlt[pAdjacentNode.Index - 1].isTraversed = true;
                            intnewLLt.AddLast(pAdjacentNode.Index);
                            intAllLLt.AddLast(pAdjacentNode.Index);
                        }
                    }
                }
            }
          
            //intTargetLLt.Dispose();
            return intnewLLt;
        }

        private void RestoreIsTraversed(ref List<CPoint> cptlt, ref SCG.LinkedList<int> intAllLLt)
        {
            foreach (int  i in intAllLLt)
            {
                cptlt[i-1].isTraversed = false;
            }
        }

       

    }
}
