using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

using ContinuousGeneralizer;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;


namespace ContinuousGeneralizer.RoadNetwork
{
    public partial class FrmClassification : Form
    {

        

        CDataRecords _DataRecords;

        public FrmClassification()
        {
            InitializeComponent();
        }




        public FrmClassification(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmClassification_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            ParameterInitialize.m_mapPolyline = new MapClass();
            
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;

            //Read all the layers
            CHelpFunc.FrmOperation(ref ParameterInitialize);
            throw new ArgumentException("improve loading layesr!");
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            ////dialogue for saving
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //string strPath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strPath);

            ////获取当前选择的点要素图层
            ////大比例尺要素图层
            //IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapPolyline.get_Layer(ParameterInitialize.cboLargerScaleLayer.Items.Count
            //                                                           - ParameterInitialize.cboLargerScaleLayer.SelectedIndex - 1);
            ////小比例尺要素图层
            //IFeatureLayer pSSFLayer = (IFeatureLayer)ParameterInitialize.m_mapPolyline.get_Layer(ParameterInitialize.cboSmallerScaleLayer.Items.Count
            //                                               - ParameterInitialize.cboSmallerScaleLayer.SelectedIndex - 1);

            //ParameterInitialize.pBSFLayer = pBSFLayer;
            //ParameterInitialize.pSSFLayer = pSSFLayer;

            ////获取线数组
            //List<CPolyline> LSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pBSFLayer);
            //List<CPolyline> SSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSSFLayer);

            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //ParameterThreshold.dblBuffer = CGeoFunc.CalMidLength(LSCPlLt);
            //ParameterThreshold.dblVerySmall = CGeoFunc.CalVerySmall(LSCPlLt);
            //double dblBound = 0.95;
            //ParameterThreshold.dblDLengthBound = dblBound;
            //ParameterThreshold.dblULengthBound = 1 / dblBound;

            //List<CPolyline> pLSCPlLt = new List<CPolyline>();   //the code has to be checked**************************************

            //List<CPolyline> SingleCPlLt = new List<CPolyline>();
            //pLSCPlLt = MatchingPolyline(ref LSCPlLt, ref SSCPlLt, ref SingleCPlLt, ParameterThreshold);   //the code has to be checked**************************************


            ////Save
            //CHelpFunc.SaveCPlLt(SingleCPlLt, "SingleCPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);


            ////List<CPolyline> SSCPlLt = new List<CPolyline>();

            //FrmRightAngelDPS FrmRightAngelDPS = new FrmRightAngelDPS();

            //FrmRightAngelDPS.Rebuild(ref pLSCPlLt, CConstants.dblVerySmall);   //the code has to be checked**************************************
            //List<CPolyline> SSAttentionCPlLt = new List<CPolyline>();
            //List<CPolyline> BSAttentionCPlLt = new List<CPolyline>();

            //MatchingPolyline1(ref pLSCPlLt, ref SSCPlLt, ref BSAttentionCPlLt, ref SSAttentionCPlLt, ParameterThreshold);   //the code has to be checked**************************************
            //List<CCorrSegment> pCorrCplLt = UnionBSCPl(ref SSCPlLt, ref BSAttentionCPlLt, ref SSAttentionCPlLt, ParameterThreshold);
            //List<CPolyline> LSCPlLt1 = new List<CPolyline>();
            //List<CPolyline> SSCPlLt1 = new List<CPolyline>();
            //for (int i = 0; i < pCorrCplLt.Count; i++)
            //{
            //    LSCPlLt1.Add(pCorrCplLt[i].CFrPolyline);
            //    SSCPlLt1.Add(pCorrCplLt[i].CToPolyline);
            //}
            //CHelpFunc.SaveCPlLt(LSCPlLt1, "LSCPlLt1", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCPlLt(SSCPlLt1, "SSCPlLt1", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCPlLt(BSAttentionCPlLt, "BSAttentionCPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
            //CHelpFunc.SaveCPlLt(SSAttentionCPlLt, "SSAttentionCPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }

        public List<CPolyline> MatchingPolyline(ref List<CPolyline> pLSCPlLt, ref List<CPolyline> pSSCPlLt, ref List<CPolyline> pSingleCPlLt, CParameterThreshold pParameterThreshold)
        {
            //for (int i = 0; i < pLSCPlLt.Count; i++)
            //{
            //    pLSCPlLt[i].CreateBuffer(pParameterThreshold.dblBuffer / 3);
            //    pLSCPlLt[i].CorrCGeoLt = new List<CPolyline>();
            //}
            //for (int i = 0; i < pSSCPlLt.Count; i++)
            //{
            //    pSSCPlLt[i].CreateBuffer(pParameterThreshold.dblBuffer / 3);
            //    pSSCPlLt[i].CorrCGeoLt = new List<CPolyline>();
            //}



            //List<CPolyline> newLSCPlLt = new List<CPolyline>();

            //newLSCPlLt.AddRange(pLSCPlLt);

            //for (int i = 0; i < pLSCPlLt.Count; i++)
            //{

            //    //if (pLSCPlLt[i].dblLength < 2 * pParameterThreshold.dblBuffer)
            //    //{
            //    //    //BSAttentionCPlLt.Add(pLSCPlLt[i]);
            //    //    ////pSingleCPlLt.Add(pLSCPlLt[i]);
            //    //    //continue;
            //    //}
            //    //else
            //    //{
            //    SortedList<double, CCorrCplInfo> pCorrCplInfoSLt = new SortedList<double, CCorrCplInfo>(new CCmpDbl());
            //    double dblBSBufferArea = pLSCPlLt[i].dblBufferArea;

            //    for (int j = 0; j < pSSCPlLt.Count; j++)
            //    {
            //        double dblOverlapArea = CGeoFunc.CalOverlapArea(pLSCPlLt[i].pBufferGeo, pSSCPlLt[j].pBufferGeo);
            //        double dblOverlapRatio = dblOverlapArea / dblBSBufferArea;
            //        CCorrCplInfo pCorrCplInfo = new CCorrCplInfo(pSSCPlLt[j], dblOverlapRatio, dblOverlapArea);
            //        pCorrCplInfoSLt.Add(dblOverlapRatio, pCorrCplInfo);
            //    }

            //    int intCount = 0;
            //    for (int j = pCorrCplInfoSLt.Count - 1; j >= 0; j--)
            //    {
            //        if (pCorrCplInfoSLt.Values[j].dblOverlapRatio > 0.5)
            //        {
            //            intCount++;
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }

            //    if (intCount == 0)
            //    {
            //        pSingleCPlLt.Add(pLSCPlLt[i]);
            //        newLSCPlLt.Remove(pLSCPlLt[i]);
            //    }
            //    //    else if (intCount == 1)
            //    //    {
            //    //        CCorrCplInfo pCorrCplInfo2 = pCorrCplInfoSLt.Values[pCorrCplInfoSLt.Count - 1];
            //    //        pLSCPlLt[i].CorrCPlLt.Add(pCorrCplInfo2.pCorrCpl);
            //    //        pCorrCplInfo2.pCorrCpl.CorrCPlLt.Add(pLSCPlLt[i]);
            //    //    }
            //    //    else
            //    //    {
            //    //        BSAttentionCPlLt.Add(pLSCPlLt[i]);
            //    //        //pSingleCPlLt.Add(pLSCPlLt[i]);
            //    //    }
            //    ////}
            //}
            //return newLSCPlLt;
            return null;
        }

        public void MatchingPolyline1(ref List<CPolyline> pLSCPlLt, ref List<CPolyline> pSSCPlLt, ref List<CPolyline> BSAttentionCPlLt, ref List<CPolyline> SSAttentionCPlLt, CParameterThreshold pParameterThreshold)
        {
            //for (int i = 0; i < pLSCPlLt.Count; i++)
            //{
            //    pLSCPlLt[i].CreateBuffer(pParameterThreshold.dblBuffer / 3);
            //    pLSCPlLt[i].CorrCGeoLt = new List<CGeoBase> new List<CPolyline>();
            //}
            //for (int i = 0; i < pSSCPlLt.Count; i++)
            //{
            //    pSSCPlLt[i].CreateBuffer(pParameterThreshold.dblBuffer / 3);
            //    pSSCPlLt[i].CorrCGeoLt = new List<CPolyline>();
            //}


            //List<CPolyline> pLSCPlLt1 = new List<CPolyline>();
            //for (int i = 0; i < pLSCPlLt.Count; i++)
            //{

            //    //if (pLSCPlLt[i].dblLength < 2 * pParameterThreshold.dblBuffer)
            //    //{
            //    //    //BSAttentionCPlLt.Add(pLSCPlLt[i]);
            //    //    ////pSingleCPlLt.Add(pLSCPlLt[i]);
            //    //    //continue;
            //    //}
            //    //else
            //    //{
            //    SortedList<double, CCorrCplInfo> pCorrCplInfoSLt = new SortedList<double, CCorrCplInfo>(new CCmpDbl());
            //    double dblBSBufferArea = pLSCPlLt[i].dblBufferArea;

            //    for (int j = 0; j < pSSCPlLt.Count; j++)
            //    {
            //        double dblOverlapArea = CGeoFunc.CalOverlapArea(pLSCPlLt[i].pBufferGeo, pSSCPlLt[j].pBufferGeo);
            //        double dblOverlapRatio = dblOverlapArea / dblBSBufferArea;
            //        CCorrCplInfo pCorrCplInfo = new CCorrCplInfo(pSSCPlLt[j], dblOverlapRatio, dblOverlapArea);
            //        pCorrCplInfoSLt.Add(dblOverlapRatio, pCorrCplInfo);
            //    }

            //    int intCount = 0;
            //    for (int j = pCorrCplInfoSLt.Count - 1; j >= 0; j--)
            //    {
            //        if (pCorrCplInfoSLt.Values[j].dblOverlapRatio > 0.5)
            //        {
            //            intCount++;
            //        }
            //        else
            //        {
            //            break;
            //        }
            //    }

            //    if (intCount == 0)
            //    {
            //        //pSingleCPlLt.Add(pLSCPlLt[i]);
            //    }
            //    else
            //    //if (intCount == 1)
            //    {
            //        CCorrCplInfo pCorrCplInfo2 = pCorrCplInfoSLt.Values[pCorrCplInfoSLt.Count - 1];
            //        pLSCPlLt[i].CorrCGeoLt.Add(pCorrCplInfo2.pCorrCpl);
            //        pCorrCplInfo2.pCorrCpl.CorrCGeoLt.Add(pLSCPlLt[i]);

            //    }
            //    //else
            //    //{
            //    //    BSAttentionCPlLt.Add(pLSCPlLt[i]);
            //    //    //pSingleCPlLt.Add(pLSCPlLt[i]);
            //    //}

            //}

        }

        public List<CCorrSegment> UnionBSCPl(ref List<CPolyline> pSSCPlLt, ref List<CPolyline> BSAttentionCPlLt, ref List<CPolyline> SSAttentionCPlLt, CParameterThreshold pParameterThreshold)
        {
            //for (int i = 0; i < pSSCPlLt.Count; i++)
            //{
            //    if (pSSCPlLt[i].CorrCGeoLt.Count == 0)
            //    {
            //        SSAttentionCPlLt.Add(pSSCPlLt[i]);
            //    }
            //    else
            //    //if (pSSCPlLt[i].CorrCPlLt.Count == 1)
            //    {
            //        List<CPolyline> CorrCPlLt = new List<CPolyline>();
            //        CorrCPlLt.AddRange(pSSCPlLt[i].CorrCGeoLt);

            //        CPolyline pUnionPolyline = CorrCPlLt[0].CopyCpl();
            //        CorrCPlLt.RemoveAt(0);

            //        bool isSuccessful = true;
            //        while (CorrCPlLt.Count > 0)
            //        {
            //            bool isUnioned = false;
            //            for (int j = 0; j < CorrCPlLt.Count; j++)
            //            {
            //                pUnionPolyline.UnionCpl(CorrCPlLt[j], ref isUnioned);

            //                if (isUnioned == true)
            //                {
            //                    CorrCPlLt.RemoveAt(j);
            //                    break;
            //                }
            //            }

            //            if (isUnioned == false)
            //            {
            //                isSuccessful = false;
            //                break;
            //            }
            //        }

            //        double dblRatio = pUnionPolyline.pBaseLine.dblLength / pSSCPlLt[i].pBaseLine.dblLength;
            //        if (dblRatio < pParameterThreshold.dblDLengthBound || dblRatio > pParameterThreshold.dblULengthBound)
            //        {
            //            isSuccessful = false;
            //        }

            //        if (isSuccessful == false)
            //        {
            //            BSAttentionCPlLt.AddRange(pSSCPlLt[i].CorrCGeoLt);
            //            SSAttentionCPlLt.Add(pSSCPlLt[i]);
            //        }
            //        else
            //        {
            //            pSSCPlLt[i].CorrCGeo = pUnionPolyline;
            //        }
            //    }
            //}

            //List<CCorrSegment> pCorrCplLt = new List<CCorrSegment>();
            //for (int i = 0; i < pSSCPlLt.Count; i++)
            //{
            //    if (pSSCPlLt[i].CorrCGeo != null && pSSCPlLt[i] != null)
            //    {
            //        CCorrSegment pCorrCpl = new CCorrSegment(pSSCPlLt[i].CorrCGeo, pSSCPlLt[i]);
            //        pCorrCplLt.Add(pCorrCpl);
            //    }
            //}
            //return pCorrCplLt;
            return null;
        }

    }
}
