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
    public partial class FrmLookingForNeighboursSweepLine : Form
    {
        protected object _Missing = Type.Missing;
        
        
        
        CDataRecords _DataRecords;

        public FrmLookingForNeighboursSweepLine()
        {
            InitializeComponent();
        }




        public FrmLookingForNeighboursSweepLine(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmLookingForNeighboursSweepLine_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            ParameterInitialize.cboLayer = this.cboLayer;

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
            cboFunction.SelectedIndex = 0;
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
            //for (int i = 0; i < CptLt.Count ; i++)
            //{
            //    CptLt[i].GID = i;
            //    CptLt[i].intTS = new TreeSet<int>();
            //}

            //long lngStartTime = System.Environment.TickCount; //record the start time          

            ////******************************************Start************************************************************************************//
            ////dblThreshold = 30000 / Convert.ToDouble(CptLt.Count);
            //if (cboFunction.Text == "Two Copies")
            //{
            //    SweepLineTwoCopiesTreeSet(ref CptLt, dblThreshold);
            //}
            //else if (cboFunction.Text == "Two Copies Without C5")
            //{
            //    SweepLineTwoCopiesWithoutC5(ref CptLt, dblThreshold);
            //}
            //else if (cboFunction.Text == "Two Sweep Lines")
            //{
            //    SweepLineTwoSweepLines(ref CptLt, dblThreshold);
            //}           


            //long lngEndTime = System.Environment.TickCount;//记录结束时间
            //_DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时


            ////save links
            ////there is a bug to creat feature class based on "esriGeometryType.esriGeometryLine", so i used polyline here.
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


            //        List<double> dbldetailedlt = new List<double>(6);
            //        if (CptLt[i].X < CptLt[j].X || (CptLt[i].X == CptLt[j].X && CptLt[i].Y < CptLt[j].Y) || (CptLt[i].X == CptLt[j].X && CptLt[i].Y == CptLt[j].Y && CptLt[i].GID < CptLt[j].GID))
            //        {
            //            dbldetailedlt.Add(CptLt[i].GID); dbldetailedlt.Add(CptLt[i].X); dbldetailedlt.Add(CptLt[i].Y); dbldetailedlt.Add(CptLt[j].GID); dbldetailedlt.Add(CptLt[j].X); dbldetailedlt.Add(CptLt[j].Y);
            //        }
            //        else
            //        {
            //            dbldetailedlt.Add(CptLt[j].GID); dbldetailedlt.Add(CptLt[j].X); dbldetailedlt.Add(CptLt[j].Y); dbldetailedlt.Add(CptLt[i].GID); dbldetailedlt.Add(CptLt[i].X); dbldetailedlt.Add(CptLt[i].Y);
            //        }
            //        dbldetailedltlt.Add(dbldetailedlt);
            //    }
            //}
            //CHelpFuncExcel.ExportDataltltToExcelSW(dbldetailedltlt, CptLt.Count.ToString() + "_" + dblThreshold.ToString() + "Links", ParameterInitialize.strSavePath);
            //CHelpFunc.SaveESRIObjltfast(iplobjlt, esriGeometryType.esriGeometryPolyline, CptLt.Count.ToString() + "_" + dblThreshold.ToString() + "Links", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }


        private void btnRunMulti_Click(object sender, EventArgs e)
        {
            double dblThreshold = Convert.ToDouble(this.txtThreshold.Text);
            
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

            IFeatureLayer pFeatureLayer = null;
            for (int i = 0; i < intLayerCount; i++)
            {
                List<double> OutPutLt = new List<double>(18);
                lngStartMemory = GC.GetTotalMemory(true);

                pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(i);
                List<CPoint> CptLt = CHelpFunc.GetCPtLtFromPointFeatureLayer(pFeatureLayer);
                //for (int j = 0; j < CptLt.Count; j++)
                //{
                //    CptLt[j].GID = j;
                //    //CptLt[j].intTS = new TreeSet<int>();
                //}
                //long lngMemory1 = GC.GetTotalMemory(true);
                double dblThreshold2 = dblThreshold * Math.Pow(Convert.ToDouble(intPtNumLast) / Convert.ToDouble(CptLt.Count), 0.5);//---------------dblThreshold------------------------------------------------//
                //------------------------------------------------Start------------------------------------------------//
                //long lngStartTime = System.Environment.TickCount; //record the start time
                //----------------------------------------SweepLineTwoCopiesTreeSet----------------------------------------//
                lngTime = 0; lngMemory = lngStartMemory; intOutPut = 0;
                SweepLineTwoCopiesTreeSet(ref CptLt, dblThreshold, ref lngTime, ref lngMemory, ref intOutPut);
                OutPutLt.Add(Convert.ToDouble(lngTime) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 1000);

                lngTime = 0; lngMemory = lngStartMemory; intOutPut = 0;
                SweepLineTwoCopiesTreeSet(ref CptLt, dblThreshold2, ref lngTime, ref lngMemory, ref intOutPut);
                OutPutLt.Add(Convert.ToDouble(lngTime) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 1000);

                //----------------------------------------SortedSet----------------------------------------//
                lngTime = 0; lngMemory = lngStartMemory; intOutPut = 0;
                SweepLineTwoCopiesSortedSet(ref CptLt, dblThreshold, ref lngTime, ref lngMemory, ref intOutPut);
                OutPutLt.Add(Convert.ToDouble(lngTime) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 1000);

                lngTime = 0; lngMemory = lngStartMemory; intOutPut = 0;
                SweepLineTwoCopiesSortedSet(ref CptLt, dblThreshold2, ref lngTime, ref lngMemory, ref intOutPut);
                OutPutLt.Add(Convert.ToDouble(lngTime) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 1000);

                //----------------------------------------TwoCopiesSortedDictionary----------------------------------------//
                lngTime = 0; lngMemory = lngStartMemory; intOutPut = 0;
                SweepLineTwoCopiesWithoutC5(ref CptLt, dblThreshold, ref lngTime, ref lngMemory, ref intOutPut);
                OutPutLt.Add(Convert.ToDouble(lngTime) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 1000);

                lngTime = 0; lngMemory = lngStartMemory; intOutPut = 0;
                SweepLineTwoCopiesWithoutC5(ref CptLt, dblThreshold2, ref lngTime, ref lngMemory, ref intOutPut);
                OutPutLt.Add(Convert.ToDouble(lngTime) / 1000);
                OutPutLt.Add(Convert.ToDouble(lngMemory) / 1048576);
                OutPutLt.Add(Convert.ToDouble(intOutPut) / 1000);

                OutPutLtLt.Add(OutPutLt);
                ParameterInitialize.tspbMain.Value = (i + 1) * 100 / intLayerCount;
                pFeatureLayer = null;
                CptLt = null;
                OutPutLt = null;
            }

            CHelpFuncExcel.ExportDataltltToExcel(OutPutLtLt, "Time&Output", ParameterInitialize.strSavePath);








            //dblTimeLt = new List<double>(intLayerCount);
            //dblOutPutLt = new List<double>(intLayerCount);
            //lngMemoryLt = new List<double>(intLayerCount);
            //OutPutLtLt = new List<List<double>>(3);
            ////for (int i = 0; i < intLayerCount; i++)
            //for (int i = 0; i < intLayerCount; i++)
            //{
            //    long lngMemory = GC.GetTotalMemory(true);

            //    pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(intLayerCount - 1 - i);
            //    List<CPoint> CptLt = CHelpFunc.GetCPtLtByFeatureLayer(pFeatureLayer);
            //    for (int j = 0; j < CptLt.Count; j++)
            //    {
            //        CptLt[j].GID = j;
            //        //CptLt[j].intTS = new TreeSet<int>();
            //    }
            //    //long lngMemory1 = GC.GetTotalMemory(true);
            //    dblThreshold = Math.Pow(1.8 / Convert.ToDouble(CptLt.Count), 0.5);//---------------dblThreshold------------------------------------------------//
            //    //------------------------------------------------Start------------------------------------------------//
            //    int intOutPut = 0;
            //    long lngStartTime = System.Environment.TickCount; //record the start time
            //    if (cboFunction.Text == "Two Copies")
            //    {
            //        SweepLineTwoCopiesTreeSet(ref CptLt, dblThreshold, ref lngMemory, ref intOutPut);
            //    }
            //    else if (cboFunction.Text == "Two Copies Without C5")
            //    {
            //        SweepLineTwoCopiesWithoutC5(ref CptLt, dblThreshold, ref lngMemory, ref intOutPut);
            //    }
            //    else if (cboFunction.Text == "Two Sweep Lines")
            //    {
            //        SweepLineTwoSweepLines(ref CptLt, dblThreshold);
            //    }


            //    dblTimeLt.Add(System.Environment.TickCount - lngStartTime);
            //    lngMemoryLt.Add(Convert.ToDouble(lngMemory) / 1048576);
            //    dblOutPutLt.Add(intOutPut);

            //    //int intOutPut = 0;
            //    //for (int j = 0; j < CptLt.Count; j++)
            //    //{
            //    //    intOutPut += CptLt[j].intTS.Count;
            //    //}
            //    //dblOutPutLt.Add(intOutPut / 2);

            //    //for (int j = 0; j < CptLt.Count; j++)
            //    //{
            //    //    CptLt[j].intTS.Dispose();
            //    //    CptLt[j].SetEmpty2();
            //    //}

            //    ParameterInitialize.tspbMain.Value = (i + 1) * 100 / intLayerCount;
            //    pFeatureLayer = null;
            //    CptLt = null;
            //}

            //OutPutLtLt.Add(dblTimeLt);
            //OutPutLtLt.Add(lngMemoryLt);
            //OutPutLtLt.Add(dblOutPutLt);
            //CHelpFuncExcel.ExportColDataltltToExcel(OutPutLtLt, "Time&Output_ChangeThreshold", ParameterInitialize.strSavePath);





            MessageBox.Show("Done!");


        }

        private void SweepLineTwoSweepLines(ref List<CPoint> cptlt, double dblThreshold)
        {
            //TreeSet<CPoint> EventTS = new TreeSet<CPoint>(new CCptYXReverseCompare());
            //for (int i = 0; i < cptlt.Count ; i++)
            //{
            //    EventTS.Add(cptlt[i]);
            //}
            //List <CPoint > EventLt=EventTS .ToList();

            //int intAddIndex = 0;
            //int intRemoveIndex = 0;
            //double dblDBound = EventLt[0].Y;
            //double dblUBound = EventLt[0].Y + dblThreshold;
            //double dblBottom = EventLt[EventLt.Count -1].Y ;

            //TreeSet<CPoint> HorizontalCPtXTS = new TreeSet<CPoint>(new CCptXCompare());
            //do
            //{
            //     double dblUDiff = dblUBound - EventLt[intRemoveIndex].Y;
            //    double dblDDiff = dblDBound - EventLt[intAddIndex].Y;
            //    if (dblUDiff < dblDDiff)
            //    {
            //        HorizontalCPtXTS.Remove(EventLt[intRemoveIndex]);
            //        dblDBound -= dblUDiff;
            //        dblUBound = EventLt[intRemoveIndex].Y;
            //        intRemoveIndex++;
            //    }
            //    else
            //    {
            //        CPoint cpt = EventLt[intAddIndex]; 
            //        cpt.intTS = new TreeSet<int>();
            //        if (HorizontalCPtXTS.Count >0)
            //        {
            //            CPoint frcpt = new CPoint(cpt.X - dblThreshold, cpt.Y); frcpt.GID = -1;
            //            CPoint tocpt = new CPoint(-2, cpt.X + dblThreshold, cpt.Y); tocpt.GID = -2;

            //            IDirectedEnumerable<CPoint> pDECpt = HorizontalCPtXTS.RangeFromTo(frcpt, tocpt);
            //            foreach (CPoint querycpt in pDECpt)
            //            {
            //                cpt.intTS.Add(querycpt.ID);
            //                querycpt.intTS.Add(cpt.ID);
            //            }

            //            frcpt.SetEmpty2();
            //            tocpt.SetEmpty2();
            //        }

            //        HorizontalCPtXTS.Add(cpt);
            //        dblDBound = EventLt[intAddIndex].Y;
            //        dblUBound -= dblDDiff;
            //        intAddIndex++;

            //    }
            //} while (dblDBound > dblBottom);
        }


        private void SweepLineTwoCopiesTreeSet(ref List<CPoint> cptlt, double dblThreshold, ref long lngTime, ref long lngMemory, ref int intOutPut)
        {
            ////long lngMemory0 = GC.GetTotalMemory(true);
            //lngTime = System.Environment.TickCount;
            //TreeSet<CPoint> EventTS = new TreeSet<CPoint>(new CCptYGIDReverseCompare());
            //int intCount = cptlt.Count;
            //for (int i = 0; i < cptlt.Count; i++)
            //{
            //    cptlt[i].isTraversed = false;
            //    EventTS.Add(cptlt[i]);

            //    CPoint cpt = new CPoint(intCount, intCount, cptlt[i].X, cptlt[i].Y - dblThreshold);
            //    cpt.CorrCGeo = cptlt[i];
            //    cpt.isTraversed = true;
            //    EventTS.Add(cpt);
            //    intCount++;
            //}
            ////long lngMemory2 = GC.GetTotalMemory(true);
            //TreeSet<CPoint> HorizontalCPtXTS = new TreeSet<CPoint>(new CCptXGIDCompare());
            //foreach (CPoint cpt in EventTS)
            //{
            //    if (cpt.isTraversed == true)
            //    {
            //        HorizontalCPtXTS.Remove(cpt.CorrCGeo);
            //    }
            //    else
            //    {

            //        //if (HorizontalCPtXTS.Count > 0)
            //        //{
            //        CPoint frcpt = new CPoint(-1, cpt.X - dblThreshold, cpt.Y);
            //        CPoint tocpt = new CPoint(intCount, intCount, cpt.X + dblThreshold, cpt.Y);

            //        IDirectedEnumerable<CPoint> pDECpt = HorizontalCPtXTS.RangeFromTo(frcpt, tocpt);
            //        intOutPut += pDECpt.Count();
            //        //foreach (CPoint querycpt in pDECpt)
            //        //{
            //        //    cpt.intTS.Add(querycpt.GID);
            //        //    querycpt.intTS.Add(cpt.GID);                  
            //        //}

            //        //frcpt.SetEmpty2();
            //        //tocpt.SetEmpty2();
            //        //}

            //        HorizontalCPtXTS.Add(cpt);
            //    }
            //}
            ////long lngMemory3 = GC.GetTotalMemory(true);
            //lngTime = System.Environment.TickCount - lngTime;
            //lngMemory = GC.GetTotalMemory(true) - lngMemory;

        }

        private void SweepLineTwoCopiesWithoutC5(ref List<CPoint> cptlt, double dblThreshold, ref long lngTime, ref long lngMemory, ref int intOutPut)
        {
            //lngTime = System.Environment.TickCount;
            //SortedDictionary<CPoint, CPoint> EventSD = new SortedDictionary<CPoint, CPoint>(new CCptYGIDReverseCompare());

            //int intCount = cptlt.Count;
            //for (int i = 0; i < cptlt.Count; i++)
            //{
            //    cptlt[i].isTraversed = false;
            //    EventSD.Add(cptlt[i], cptlt[i]);

            //    CPoint cpt = new CPoint(intCount, intCount, cptlt[i].X, cptlt[i].Y - dblThreshold);
            //    cpt.CorrCGeo = cptlt[i];
            //    cpt.isTraversed = true;
            //    EventSD.Add(cpt, cpt);
            //    intCount++;
            //}

            ////    IEnumerable<SCG.KeyValuePair<double, CPoint>> query = HorizontalCPtXSD.Where(kvp => (kvp.Key >= kvpSgEnd.Value.X - dblVerySmall && kvp.Key <= kvpSgEnd.Value.X + dblVerySmall));
            ////    List<SCG.KeyValuePair<double, CPoint>> kvpquerylt = query.ToList();


            //SortedDictionary<CPoint, CPoint> HorizontalCPtXSD = new SortedDictionary<CPoint, CPoint>(new CCptXGIDCompare());
            ////SCG.id 


            //foreach (SCG.KeyValuePair<CPoint, CPoint> kvpevent in EventSD)
            //{
            //    CPoint cpt = kvpevent.Key;
            //    if (cpt.isTraversed == true)
            //    {
            //        HorizontalCPtXSD.Remove(cpt.CorrCGeo);
            //    }
            //    else
            //    {
            //        IEnumerable<SCG.KeyValuePair<CPoint, CPoint>> querycptlt = HorizontalCPtXSD.Where(kvp => ((kvp.Key.X >= cpt.X - dblThreshold) && (kvp.Key.X <= cpt.X + dblThreshold)));
            //        intOutPut += querycptlt.Count();
            //        //foreach (SCG.KeyValuePair<CPoint, CPoint> kvpquery in querycptlt)
            //        //{
            //        //    cpt.intTS.Add(kvpquery.Key.GID);
            //        //    kvpquery.Key.intTS.Add(cpt.GID);
            //        //}
            //        HorizontalCPtXSD.Add(cpt, cpt);
            //    }
            //}
            //lngTime = System.Environment.TickCount - lngTime;
            //lngMemory = GC.GetTotalMemory(true) - lngMemory;
            //Math.
        }


        private void SweepLineTwoCopiesSortedSet(ref List<CPoint> cptlt, double dblThreshold, ref long lngTime, ref long lngMemory, ref int intOutPut)
        {
            ////long lngMemory0 = GC.GetTotalMemory(true);
            //lngTime = System.Environment.TickCount;
            //SortedSet<CPoint> EventTS = new SortedSet<CPoint>(new CCptYGIDReverseCompare());
            //int intCount = cptlt.Count;
            //for (int i = 0; i < cptlt.Count; i++)
            //{
            //    cptlt[i].isTraversed = false;
            //    EventTS.Add(cptlt[i]);

            //    CPoint cpt = new CPoint(intCount, intCount, cptlt[i].X, cptlt[i].Y - dblThreshold);
            //    cpt.CorrCGeo = cptlt[i];
            //    cpt.isTraversed = true;
            //    EventTS.Add(cpt);
            //    intCount++;
            //}
            ////long lngMemory2 = GC.GetTotalMemory(true);
            //SortedSet<CPoint> HorizontalCPtXTS = new SortedSet<CPoint>(new CCptXGIDCompare());
            //foreach (CPoint cpt in EventTS)
            //{
            //    if (cpt.isTraversed == true)
            //    {
            //        HorizontalCPtXTS.Remove(cpt.CorrCGeo);
            //    }
            //    else
            //    {

            //        //if (HorizontalCPtXTS.Count > 0)
            //        //{
            //        CPoint frcpt = new CPoint(-1, cpt.X - dblThreshold, cpt.Y);
            //        CPoint tocpt = new CPoint(intCount, intCount, cpt.X + dblThreshold, cpt.Y);

            //        SortedSet<CPoint> pDECpt = HorizontalCPtXTS.GetViewBetween(frcpt, tocpt);
            //        intOutPut += pDECpt.Count;
            //        //foreach (CPoint querycpt in pDECpt)
            //        //{
            //        //    cpt.intTS.Add(querycpt.GID);
            //        //    querycpt.intTS.Add(cpt.GID);                  
            //        //}

            //        //frcpt.SetEmpty2();
            //        //tocpt.SetEmpty2();
            //        //}

            //        HorizontalCPtXTS.Add(cpt);
            //    }
            //}
            ////long lngMemory3 = GC.GetTotalMemory(true);
            //lngTime = System.Environment.TickCount - lngTime;
            //lngMemory = GC.GetTotalMemory(true) - lngMemory;

        }

    }
}
