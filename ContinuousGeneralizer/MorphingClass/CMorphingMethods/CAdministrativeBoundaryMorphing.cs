using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using C5;
using SCG = System.Collections.Generic;

using MorphingClass.CEntity;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using ESRI.ArcGIS.Controls;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CGeneralizationMethods;

namespace MorphingClass.CMorphingMethods
{
    /// <summary>CAtBdMorphing</summary>
    /// <remarks></remarks>
    public class CAtBdMorphing
    {
        
        
        //private CDPSimplify _pDPSimplify = new CDPSimplify();
        private CParameterResult _ParameterResult;
        private object _Missing = Type.Missing;

        public List<CPolyline> _LSCPlLt = new List<CPolyline>();  //BS:LargerScale
        public List<CPolyline> _SSCPlLt = new List<CPolyline>();  //SS:SmallerScale
        public List<CPolyline> _SgCPlLt = new List<CPolyline>();  //Sg:Single

        private CParameterInitialize _ParameterInitialize;

        public CAtBdMorphing()
        {

        }

        public CAtBdMorphing(CParameterInitialize ParameterInitialize)
        {

            //Get the feature layers selected
            //Lager-scale layer
            IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLargerScaleLayer.SelectedIndex);

            //Smaller-scale layer
            IFeatureLayer pSSFLayer =(IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboSmallerScaleLayer.SelectedIndex);                                                           

            //Layer of single boundaries which are from larger-scale layer
            IFeatureLayer pSgFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLayer.SelectedIndex);
           

            ParameterInitialize.pBSFLayer = pBSFLayer;
            ParameterInitialize.pSSFLayer = pSSFLayer;
            ParameterInitialize.pSgFLayer = pSgFLayer;
            _ParameterInitialize = ParameterInitialize;

            //获取线数组
            _LSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pBSFLayer);
            _SSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSSFLayer);
            _SgCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSgFLayer);
        }


        public void AtBdMorphing()
        {
            //long lngStartTime1 = 0;
            //long lngEndTime1 = 0;

            //List<CPolyline> LSCPlLt = _LSCPlLt;
            //List<CPolyline> SSCPlLt = _SSCPlLt;
            //List<CPolyline> SgCPlLt = _SgCPlLt;

            
            ////int intBSNum = CHelpFunc.GetPtNumFromCplLt(LSCPlLt);
            ////int intSSNum = CHelpFunc.GetPtNumFromCplLt(SSCPlLt);
            ////int intSgNum = CHelpFunc.GetPtNumFromCplLt(SgCPlLt);

            //CParameterInitialize pParameterInitialize = _ParameterInitialize;
            //CParameterThreshold pParameterThreshold = new CParameterThreshold();
            //pParameterThreshold.dblBuffer =CGeoFunc.CalMidLength(LSCPlLt);
            //pParameterThreshold.dblVerySmall = CGeoFunc.CalVerySmall(LSCPlLt);
            //pParameterThreshold.dblOverlapRatio = pParameterInitialize.dblOverlapRatio;
            //pParameterThreshold.dblAngleBound = 0.262;
            //double dblBound = 0.98;
            //pParameterThreshold.dblDLengthBound = dblBound;
            //pParameterThreshold.dblULengthBound = 1 / dblBound;

            ////create AtBdLt based on data
            //List<CAtBd> pBSAtBdLt = new List<CAtBd>();
            //List<CAtBd> pSSAtBdLt = new List<CAtBd>();
          
            //for (int i = 0; i < LSCPlLt.Count; i++)
            //{
            //    LSCPlLt[i].SetEdgeLength();
            //    SSCPlLt[i].SetEdgeLength();
            //    CAtBd pBSAtBd = new CAtBd(i, LSCPlLt[i]);
            //    pBSAtBdLt.Add(pBSAtBd);
            //    CAtBd pSSAtBd = new CAtBd(i, SSCPlLt[i]);
            //    pSSAtBdLt.Add(pSSAtBd);
            //}

            //List<CAtBd> pSgAtBdLt = new List<CAtBd>();
            //for (int i = 0; i < SgCPlLt.Count; i++)
            //{
            //    CAtBd pAtBd = new CAtBd(i, SgCPlLt[i]);
            //    pSgAtBdLt.Add(pAtBd);
            //}

            ////handle
            //long lngTime1 = System.Environment.TickCount;
            //DWCorrAtBd(pBSAtBdLt, pSSAtBdLt, pParameterThreshold);
            //long lngCorrTime = System.Environment.TickCount - lngTime1;

            //long lngTime2 = System.Environment.TickCount;
            //DWSingleAtBd(pSgAtBdLt, pBSAtBdLt, pParameterThreshold);

            //double dblRatioofPtNum = CalRatioofPtNum(pBSAtBdLt, pSSAtBdLt, pParameterThreshold);
            //ConfirmSgPt(pSgAtBdLt, dblRatioofPtNum, pParameterThreshold);
            //long lngSingleTime = System.Environment.TickCount - lngTime2;


            ////获取结果，全部记录在_ParameterResult中
            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.CBSAtBdLt = pBSAtBdLt;
            //ParameterResult.CSSAtBdLt = pSSAtBdLt;
            //ParameterResult.CSgAtBdLt = pSgAtBdLt;
            ////ParameterResult.lngTime = lngTimeSum;
            //_ParameterResult = ParameterResult;
        }

        private void DWCorrAtBd(List<CAtBd> pBSAtBdLt, List<CAtBd> pSSAtBdLt, CParameterThreshold ParameterThreshold)
        {

            CLinearInterpolationA pLinearInterpolation = new CLinearInterpolationA();
            for (int i = 0; i < pBSAtBdLt.Count; i++)
            {
                CPolyline frcpl=pBSAtBdLt[i] as CPolyline;
                CPolyline tocpl=pSSAtBdLt[i] as CPolyline;
                //CHelpFunc.PreviousWork(ref frcpl, ref tocpl);

                pBSAtBdLt[i].CResultPtLt = pLinearInterpolation.CLI(frcpl, tocpl);

                ////By DP Algorithm
                //_pDPSimplify.DivideCplForDP(pBSAtBdLt[i] as CPolyline, pBSAtBdLt[i].pVirtualPolyline);
                //_pDPSimplify.DivideCplForDP(pSSAtBdLt[i] as CPolyline, pSSAtBdLt[i].pVirtualPolyline);

                //C5.LinkedList<CCorrSegment> CorrespondSegmentLk = new C5.LinkedList<CCorrSegment>();
                //SubPolylineMatchLA(pBSAtBdLt[i] as CPolyline, pBSAtBdLt[i].pVirtualPolyline, pSSAtBdLt[i] as CPolyline, pSSAtBdLt[i].pVirtualPolyline, ParameterThreshold, ref CorrespondSegmentLk);

                ////linear interpolation
                //pBSAtBdLt[i].CResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(CorrespondSegmentLk, "Linear");
            }
        }

        private void DWSingleAtBd(List<CAtBd> pSgAtBdLt, List<CAtBd> pBSAtBdLt, CParameterThreshold ParameterThreshold)
        {
            FindFrcpt2AndTocpt2Grid(pSgAtBdLt, pBSAtBdLt, ParameterThreshold);
            for (int i = 0; i < pSgAtBdLt.Count; i++)
            {
                pSgAtBdLt[i].SetVirtualPolyline();
                //_pDPSimplify.DivideCplForDP(pSgAtBdLt[i] as CPolyline, pSgAtBdLt[i].pVirtualPolyline);
            }
        }

        private void FindFrcpt2AndTocpt2Grid(List<CAtBd> pSgAtBdLt, List<CAtBd> pBSAtBdLt, CParameterThreshold ParameterThreshold)
        {
            //SgCptLt for pSgAtBdLt
            List<CPoint> SgCptLt = new List<CPoint>(pSgAtBdLt.Count * 2);
            foreach (CAtBd pAtBd in pSgAtBdLt)
            {
                CPoint cpt0 = pAtBd.CptLt[0];
                cpt0.BelongedObject = pAtBd;
                CPoint cptlast = pAtBd.CptLt[pAtBd.CptLt.Count - 1];
                cptlast.BelongedObject = pAtBd;
                SgCptLt.Add(cpt0);
                SgCptLt.Add(cptlast);
            }

            //BSCptLt for pBSAtBdLt
            int intCount = 0;
            foreach (CAtBd pAtBd in pBSAtBdLt)
            {
                intCount += pAtBd.CptLt.Count;
            }
            List<CPoint> BSCptLt = new List<CPoint>(intCount);
            foreach (CAtBd pAtBd in pBSAtBdLt)
            {
                foreach (CPoint cpt in pAtBd.CptLt)
                {
                    //cpt.BelongedObject = pAtBd;
                    BSCptLt.Add(cpt);
                }
            }

            //LookingForNeighboursByGrids
            var CorrCptsLt = CGeoFunc.LookingForNeighboursByGrids(SgCptLt, BSCptLt, CConstants.dblVerySmallCoord);

            //FindFrcpt2AndTocpt2
            foreach (CCorrCpts CorrCpt in CorrCptsLt)
            {
                CorrCpt.FrCpt.isTraversed = false;
            }
            foreach (CCorrCpts CorrCpt in CorrCptsLt)
            {
                CPoint frcpt = CorrCpt.FrCpt;
                if (frcpt.isTraversed == false)  //we only need one
                {
                    CAtBd pAtBd = frcpt.BelongedObject as CAtBd;
                    if (frcpt.ID ==0)
                    {
                        pAtBd.Frcpt2 = CorrCpt.ToCpt;
                    }
                    else
                    {
                        pAtBd.Tocpt2 = CorrCpt.ToCpt;
                    }
                    frcpt.isTraversed = true;
                }
            }
        }

        #region FindFrcpt2AndTocpt2Grid, which are not used anymore
        //private void FindFrcpt2AndTocpt2SweepLine(List<CAtBd> pSgAtBdLt, List<CAtBd> pBSAtBdLt, CParameterThreshold ParameterThreshold)
        //{
        //    double dblVerySmall = ParameterThreshold.dblVerySmall;
        //    int intGIDCount = 0;
        //    SortedSet<CPoint> EventQueueYGID = new SortedSet<CPoint>(new CCptYGIDReverseCompare());
        //    for (int i = 0; i < pSgAtBdLt.Count; i++)
        //    {
        //        CPoint cpt0 = pSgAtBdLt[i].CptLt[0];
        //        CPoint cptlast = pSgAtBdLt[i].CptLt[pSgAtBdLt[i].CptLt.Count - 1];
        //        cpt0.LID = i;
        //        cpt0.GID = intGIDCount;
        //        cpt0.strBelong = "Sg";
        //        EventQueueYGID.Add(cpt0);
        //        intGIDCount++;

        //        cptlast.LID = i;
        //        cptlast.GID = intGIDCount;
        //        cptlast.strBelong = "Sg";
        //        EventQueueYGID.Add(cptlast);
        //        intGIDCount++;
        //    }

        //    for (int i = 0; i < pBSAtBdLt.Count; i++)
        //    {
        //        foreach (CPoint cpt in pBSAtBdLt[i].CptLt)
        //        {
        //            cpt.GID = intGIDCount;
        //            cpt.LID = i;
        //            cpt.strBelong = "BS";
        //            //BSCptLt.Add(cpt);
        //            intGIDCount++;

        //            //make two copies
        //            CPoint ucpt = new CPoint(cpt.ID, cpt.X, cpt.Y + dblVerySmall);
        //            ucpt.GID = intGIDCount;
        //            ucpt.LID = cpt.LID;
        //            ucpt.strBelong = cpt.strBelong;
        //            ucpt.strSweepStatus = "Up";
        //            ucpt.CorrespondingPt = cpt;
        //            EventQueueYGID.Add(ucpt);
        //            intGIDCount++;

        //            CPoint dcpt = new CPoint(cpt.ID, cpt.X, cpt.Y - dblVerySmall);
        //            dcpt.GID = intGIDCount;
        //            dcpt.LID = cpt.LID;
        //            dcpt.strBelong = cpt.strBelong;
        //            dcpt.strSweepStatus = "Down";
        //            dcpt.CorrespondingPt = cpt;
        //            EventQueueYGID.Add(dcpt);
        //            intGIDCount++;
        //        }
        //    }

        //    //sweep
        //    SortedSet<CPoint> StripCPtXGID = new SortedSet<CPoint>(new CCptXGIDCompare());
        //    //double dblSweepY = EventQueueYTD.ElementAt(0).Key.Y;
        //    foreach (CPoint cpt in EventQueueYGID)
        //    {
        //        if (cpt.strBelong == "BS")
        //        {
        //            if (cpt.strSweepStatus == "Up")
        //            {
        //                StripCPtXGID.Add(cpt.CorrespondingPt);
        //            }
        //            else if (cpt.strSweepStatus == "Down")
        //            {
        //                StripCPtXGID.Remove(cpt.CorrespondingPt);
        //            }
        //        }
        //        else
        //        {
        //            CPoint frcpt = new CPoint(-1, -1, cpt.X - dblVerySmall, cpt.Y);
        //            CPoint tocpt = new CPoint(intGIDCount, intGIDCount, cpt.X + dblVerySmall, cpt.Y);

        //            SortedSet<CPoint> StripView = StripCPtXGID.GetViewBetween(frcpt, tocpt);
        //            foreach (CPoint cptview in StripView)
        //            {
        //                if (cpt.ID == 0)
        //                {
        //                    pSgAtBdLt[cpt.LID].Frcpt2 = cptview;
        //                }
        //                else
        //                {
        //                    pSgAtBdLt[cpt.LID].Tocpt2 = cptview;
        //                }
        //                break;  // we only need one
        //            }

        //            frcpt = null;
        //            tocpt = null;
        //        }
        //    }
        //}

        //private void FindFrcpt2AndTocpt2Grid(List<CAtBd> pSgAtBdLt, List<CAtBd> pBSAtBdLt, CParameterThreshold ParameterThreshold)
        //{
        //    List<CPolyline> SgCPlLt = CHelpFunc.GetCplLtFromAtBdLt(pSgAtBdLt);
        //    List<CPolyline> BScpllt = CHelpFunc.GetCplLtFromAtBdLt(pBSAtBdLt);

        //    IEnvelope pSgEnvelope = CHelpFunc.GetEnvelope(SgCPlLt);
        //    IEnvelope pBSEnvelope = CHelpFunc.GetEnvelope(BScpllt);

        //    IEnvelope pUnionEnvelope = new EnvelopeClass();
        //    pUnionEnvelope.XMin = Math.Min(pSgEnvelope.XMin, pBSEnvelope.XMin);
        //    pUnionEnvelope.YMin = Math.Min(pSgEnvelope.YMin, pBSEnvelope.YMin);
        //    pUnionEnvelope.XMax = Math.Max(pSgEnvelope.XMax, pBSEnvelope.XMax);
        //    pUnionEnvelope.YMax = Math.Max(pSgEnvelope.YMax, pBSEnvelope.YMax);

        //    int intSgPtNum = SgCPlLt.Count * 2;
        //    int intBSPtNum = CHelpFunc.SumPointsNumber(BScpllt);

        //    int intMaxPtNum = Math.Max(intSgPtNum, intBSPtNum);
        //    int intRowColCount = Convert.ToInt32(Math.Sqrt(Convert.ToDouble(intMaxPtNum)));

        //    double dblWidth = Math.Max((pUnionEnvelope.XMax - pUnionEnvelope.XMin) / Convert.ToDouble(intRowColCount), CConstants.dblVerySmall);
        //    double dblHeight = Math.Max((pUnionEnvelope.YMax - pUnionEnvelope.YMin) / Convert.ToDouble(intRowColCount), CConstants.dblVerySmall);

        //    intRowColCount++;  //+1, so that the bordered point can be covered
        //    C5.LinkedList<CPoint>[,] aSgCptLkGrid = new C5.LinkedList<CPoint>[intRowColCount, intRowColCount];
        //    C5.LinkedList<CPoint>[,] aBSCptLkGrid = new C5.LinkedList<CPoint>[intRowColCount, intRowColCount];
        //    for (int i = 0; i < intRowColCount; i++)
        //    {
        //        for (int j = 0; j < intRowColCount; j++)
        //        {
        //            aSgCptLkGrid[i, j] = new C5.LinkedList<CPoint>();
        //            aBSCptLkGrid[i, j] = new C5.LinkedList<CPoint>();
        //        }
        //    }

        //    //Fill points in Grids
        //    for (int i = 0; i < pSgAtBdLt.Count; i++)
        //    {
        //        List<CPoint> cptlt = pSgAtBdLt[i].CptLt;
        //        cptlt[0].BelongedCPolyline = pSgAtBdLt[i] as CPolyline;
        //        cptlt[cptlt.Count - 1].BelongedCPolyline = pSgAtBdLt[i] as CPolyline;
        //        CHelpFunc.FillCptinGrid(cptlt[0], dblWidth, dblHeight, pUnionEnvelope, aSgCptLkGrid);
        //        CHelpFunc.FillCptinGrid(cptlt[cptlt.Count - 1], dblWidth, dblHeight, pUnionEnvelope, aSgCptLkGrid);
        //    }
        //    for (int i = 0; i < pBSAtBdLt.Count; i++)
        //    {
        //        List<CPoint> cptlt = pBSAtBdLt[i].CptLt;
        //        foreach (CPoint cpt in cptlt)
        //        {
        //            cpt.LID = i;
        //            CHelpFunc.FillCptinGrid(cpt, dblWidth, dblHeight, pUnionEnvelope, aBSCptLkGrid);
        //        }
        //    }

        //    for (int i = 0; i < intRowColCount; i++)
        //    {
        //        for (int j = 0; j < intRowColCount; j++)
        //        {
        //            //the first column
        //            int intJ = j - 1;
        //            if (intJ >= 0)
        //            {
        //                //Lower
        //                if (i - 1 >= 0)
        //                {
        //                    LookingForNeighboursInGrid(ref aSgCptLkGrid[i, j], aBSCptLkGrid[i - 1, intJ], pSgAtBdLt, pBSAtBdLt, CConstants.dblVerySmall);
        //                }
        //                //Midle
        //                LookingForNeighboursInGrid(ref aSgCptLkGrid[i, j], aBSCptLkGrid[i, intJ], pSgAtBdLt, pBSAtBdLt, CConstants.dblVerySmall);
        //                //Upper
        //                if (i + 1 < intRowColCount)
        //                {
        //                    LookingForNeighboursInGrid(ref aSgCptLkGrid[i, j], aBSCptLkGrid[i + 1, intJ], pSgAtBdLt, pBSAtBdLt, CConstants.dblVerySmall);
        //                }
        //            }

        //            //the second column
        //            intJ = j;
        //            //Lower
        //            if (i - 1 >= 0)
        //            {
        //                LookingForNeighboursInGrid(ref aSgCptLkGrid[i, j], aBSCptLkGrid[i - 1, intJ], pSgAtBdLt, pBSAtBdLt, CConstants.dblVerySmall);
        //            }
        //            //Midle
        //            LookingForNeighboursInGrid(ref aSgCptLkGrid[i, j], aBSCptLkGrid[i, intJ], pSgAtBdLt, pBSAtBdLt, CConstants.dblVerySmall);
        //            //Upper
        //            if (i + 1 < intRowColCount)
        //            {
        //                LookingForNeighboursInGrid(ref aSgCptLkGrid[i, j], aBSCptLkGrid[i + 1, intJ], pSgAtBdLt, pBSAtBdLt, CConstants.dblVerySmall);
        //            }

        //            //the third column
        //            intJ = j + 1;
        //            if (intJ < intRowColCount)
        //            {
        //                //Lower
        //                if (i - 1 >= 0)
        //                {
        //                    LookingForNeighboursInGrid(ref aSgCptLkGrid[i, j], aBSCptLkGrid[i - 1, intJ], pSgAtBdLt, pBSAtBdLt, CConstants.dblVerySmall);
        //                }
        //                //Midle
        //                LookingForNeighboursInGrid(ref aSgCptLkGrid[i, j], aBSCptLkGrid[i, intJ], pSgAtBdLt, pBSAtBdLt, CConstants.dblVerySmall);
        //                //Upper
        //                if (i + 1 < intRowColCount)
        //                {
        //                    LookingForNeighboursInGrid(ref aSgCptLkGrid[i, j], aBSCptLkGrid[i + 1, intJ], pSgAtBdLt, pBSAtBdLt, CConstants.dblVerySmall);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void LookingForNeighboursInGrid(ref C5.LinkedList<CPoint> SgCptLkGrid, C5.LinkedList<CPoint> BSCptLkGrid, List<CAtBd> pSgAtBdLt, List<CAtBd> pBSAtBdLt, double dblVerySmall)
        //{
        //    foreach (CPoint Sgcpt in SgCptLkGrid)
        //    {
        //        if (Sgcpt.CorrespondingPt == null)
        //        {
        //            foreach (CPoint BScpt in BSCptLkGrid)
        //            {
        //                if (Sgcpt.Equals2D(BScpt, dblVerySmall) == true)
        //                {
        //                    Sgcpt.CorrespondingPt = BScpt;
        //                    if (Sgcpt.ID == 0)
        //                    {
        //                        //Sgcpt.
        //                        pSgAtBdLt[Sgcpt.LID].Frcpt2 = BScpt;
        //                        //pSgAtBdLt[Sgcpt.LID].pFrAtBd = pBSAtBdLt[BScpt.LID];
        //                    }
        //                    else
        //                    {
        //                        pSgAtBdLt[Sgcpt.LID].Tocpt2 = BScpt;
        //                        //pSgAtBdLt[Sgcpt.LID].pToAtBd = pBSAtBdLt[BScpt.LID];
        //                    }
        //                    break;  // we only need one
        //                }
        //            }
        //        }
        //    }
        //}

        #endregion
       
        /// <summary>
        /// 
        /// </summary>
        /// <param name="pBSAtBdLt"></param>
        /// <param name="pSSAtBdLt"></param>
        /// <param name="ParameterThreshold"></param>
        /// <remarks>the number of Administrative Boundaries in pBSAtBdLt and that in pSSAtBdLt are the same.Besides, the topological structrues are the same</remarks>
        private double CalRatioofPtNum(List<CAtBd> pBSAtBdLt, List<CAtBd> pSSAtBdLt, CParameterThreshold ParameterThreshold)
        {
            int intBSInnerPtNum = 0;
            int intSSInnerPtNum = 0;

            List<CPoint> BSEndPtLt = new List<CPoint>(pBSAtBdLt.Count*2);
            //List<CPoint> SSEndPtLt = new List<CPoint>();
            for (int i = 0; i < pBSAtBdLt.Count; i++)
            {
                intBSInnerPtNum += (pBSAtBdLt[i].CptLt.Count - 2);
                intSSInnerPtNum += (pSSAtBdLt[i].CptLt.Count - 2);

                BSEndPtLt.Add(pBSAtBdLt[i].CptLt[0]);
                BSEndPtLt.Add(pBSAtBdLt[i].CptLt[pBSAtBdLt[i].CptLt.Count - 1]);
            }

            var CorrCptsLt = CGeoFunc.LookingForNeighboursByGrids(BSEndPtLt, CConstants.dblVerySmallCoord);
            int intIntersection = CGeoFunc.GetNumofIntersections(CorrCptsLt);

            //do we need this?*******************************************************************************
            //int intAloneEnds = CGeoFunc.GetNumofAloneEnds(EndPtLt, CorrCptsLt);
            //int intRealPtNum = intInnerPtNum + intSgIntersection + intAloneEnds;

            //notice that there are the same intersections of the larger-scale polylines and the smaller-scale polylines
            int intBSRealPtNum = intBSInnerPtNum + intIntersection;
            int intSSRealPtNum = intSSInnerPtNum + intIntersection;

            double dblRatioofPtNum = Convert.ToDouble(intBSRealPtNum) / Convert.ToDouble(intSSRealPtNum);
            return dblRatioofPtNum;
        }

        private void ConfirmSgPt(List<CAtBd> pSgAtBdLt, double dblRatioofPtNum, CParameterThreshold ParameterThreshold)
        {
            //int intSgInnerPtNum = 0;
            //List<CPoint> SgEndPtLt = new List<CPoint>(pSgAtBdLt.Count *2);
            //for (int i = 0; i < pSgAtBdLt.Count; i++)
            //{
            //    intSgInnerPtNum += (pSgAtBdLt[i].CptLt.Count-2);

            //    if (pSgAtBdLt[i].Frcpt2 ==null )
            //    {
            //        SgEndPtLt.Add(pSgAtBdLt[i].CptLt[0]);
            //    }

            //    if (pSgAtBdLt[i].Tocpt2 == null)
            //    {
            //        SgEndPtLt.Add(pSgAtBdLt[i].CptLt[pSgAtBdLt[i].CptLt.Count - 1]);
            //    } 
            //}

            //C5.LinkedList<CCorrCpts> CorrCptsLt = CGeoFunc.LookingForNeighboursByGrids(SgEndPtLt, CConstants.dblVerySmall);
            //int intSgIntersection = CGeoFunc.GetNumofIntersections(CorrCptsLt);

            ////do we need this?*******************************************************************************
            ////int intAloneEnds = CGeoFunc.GetNumofAloneEnds(EndPtLt, CorrCptsLt);
            ////int intRealPtNum = intInnerPtNum + intSgIntersection + intAloneEnds;

            //int intSgRealPtNum = intSgInnerPtNum + intSgIntersection;   //intSgRealPtNum doesn't count the points on the BSCpls
            //int intRemainPtNum = Convert.ToInt32(Convert.ToDouble(intSgRealPtNum) / dblRatioofPtNum);
            //int intDeletePtNum = intSgRealPtNum-intRemainPtNum;


            //double dblTDis = CDPSimplify.CalTDisByDeletePtNum(pSgAtBdLt, intSgInnerPtNum, intDeletePtNum);


            //for (int i = 0; i < pSgAtBdLt.Count; i++)
            //{
            //    _pDPSimplify.ConfirmMoveInfo(pSgAtBdLt[i] as CPolyline, pSgAtBdLt[i].pVirtualPolyline, dblTDis);
            //}
        }


        
       

        

        //public void SubPolylineMatchLA(CPolyline CFrPolyline, CVirtualPolyline CFrVtPl, CPolyline CToPolyline, CVirtualPolyline CToVtPl, CParameterThreshold ParameterThreshold, ref C5.LinkedList<CCorrSegment> CorrespondSegmentLk)
        //{
        //    List<CPoint> frcptlt = CFrPolyline.CptLt;
        //    List<CPoint> tocptlt = CToPolyline.CptLt;

        //    //如果其中一个线段没有孩子，则直接匹配并结束
        //    if (CFrVtPl.CLeftPolyline == null || CToVtPl.CLeftPolyline == null)
        //    {
        //        List<CPoint> frsubcptlt = new List<CPoint>();
        //        for (int i = CFrVtPl.intFrID; i <= CFrVtPl.intToID; i++)
        //        {
        //            frsubcptlt.Add(frcptlt[i]);
        //        }
        //        CPolyline frsubcpl = new CPolyline(CFrVtPl.intFrID, frsubcptlt);

        //        List<CPoint> tosubcptlt = new List<CPoint>();
        //        for (int i = CToVtPl.intFrID; i <= CToVtPl.intToID; i++)
        //        {
        //            tosubcptlt.Add(tocptlt[i]);
        //        }
        //        CPolyline tosubcpl = new CPolyline(CFrVtPl.intFrID, tosubcptlt);

        //        CCorrSegment CorrespondSegment = new CCorrSegment(frsubcpl, tosubcpl);
        //        CorrespondSegmentLk.Add(CorrespondSegment);
        //        return;
        //    }

        //    double dblRatioLL = frcptlt[CFrVtPl.CLeftPolyline.intFrID].DistanceTo(frcptlt[CFrVtPl.CLeftPolyline.intToID]) / tocptlt[CToVtPl.CLeftPolyline.intFrID].DistanceTo(tocptlt[CToVtPl.CLeftPolyline.intToID]);
        //    double dblRatioRR = frcptlt[CFrVtPl.CRightPolyline.intFrID].DistanceTo(frcptlt[CFrVtPl.CRightPolyline.intToID]) / tocptlt[CToVtPl.CRightPolyline.intFrID].DistanceTo(tocptlt[CToVtPl.CRightPolyline.intToID]);

        //    double dblFrDiffLLX = frcptlt[CFrVtPl.CLeftPolyline.intToID].X - frcptlt[CFrVtPl.CLeftPolyline.intFrID].X;
        //    double dblFrDiffLLY = frcptlt[CFrVtPl.CLeftPolyline.intToID].Y - frcptlt[CFrVtPl.CLeftPolyline.intFrID].Y;
        //    double dblToDiffLLX = tocptlt[CToVtPl.CLeftPolyline.intToID].X - tocptlt[CToVtPl.CLeftPolyline.intFrID].X;
        //    double dblToDiffLLY = tocptlt[CToVtPl.CLeftPolyline.intToID].Y - tocptlt[CToVtPl.CLeftPolyline.intFrID].Y;
        //    double dblAngleDiffLL = CGeoFunc.CalAngle_Counterclockwise(dblFrDiffLLX, dblFrDiffLLY, dblToDiffLLX, dblToDiffLLY);

        //    double dblFrDiffRRX = frcptlt[CFrVtPl.CRightPolyline.intToID].X - frcptlt[CFrVtPl.CRightPolyline.intFrID].X;
        //    double dblFrDiffRRY = frcptlt[CFrVtPl.CRightPolyline.intToID].Y - frcptlt[CFrVtPl.CRightPolyline.intFrID].Y;
        //    double dblToDiffRRX = tocptlt[CToVtPl.CRightPolyline.intToID].X - tocptlt[CToVtPl.CRightPolyline.intFrID].X;
        //    double dblToDiffRRY = tocptlt[CToVtPl.CRightPolyline.intToID].Y - tocptlt[CToVtPl.CRightPolyline.intFrID].Y;
        //    double dblAngleDiffRR = CGeoFunc.CalAngle_Counterclockwise(dblFrDiffRRX, dblFrDiffRRY, dblToDiffRRX, dblToDiffRRY);

        //    if ((dblRatioLL >= ParameterThreshold.dblDLengthBound) && (dblRatioLL <= ParameterThreshold.dblULengthBound) &&
        //        (dblRatioRR >= ParameterThreshold.dblDLengthBound) && (dblRatioRR <= ParameterThreshold.dblULengthBound) &&
        //        (Math.Abs(dblAngleDiffLL) <= ParameterThreshold.dblAngleBound) && (dblAngleDiffRR <= ParameterThreshold.dblAngleBound))
        //    {
        //        //相应线段长度相近
        //        SubPolylineMatchLA(CFrPolyline, CFrVtPl.CLeftPolyline, CToPolyline, CToVtPl.CLeftPolyline, ParameterThreshold, ref CorrespondSegmentLk);
        //        SubPolylineMatchLA(CFrPolyline, CFrVtPl.CRightPolyline, CToPolyline, CToVtPl.CRightPolyline, ParameterThreshold, ref CorrespondSegmentLk);
        //    }
        //    else
        //    {
        //        List<CPoint> frsubcptlt = new List<CPoint>();
        //        for (int i = CFrVtPl.intFrID; i <= CFrVtPl.intToID; i++)
        //        {
        //            frsubcptlt.Add(frcptlt[i]);
        //        }
        //        CPolyline frsubcpl = new CPolyline(CFrVtPl.intFrID, frsubcptlt);

        //        List<CPoint> tosubcptlt = new List<CPoint>();
        //        for (int i = CToVtPl.intFrID; i <= CToVtPl.intToID; i++)
        //        {
        //            tosubcptlt.Add(tocptlt[i]);
        //        }
        //        CPolyline tosubcpl = new CPolyline(CFrVtPl.intFrID, tosubcptlt);


        //        CCorrSegment CorrespondSegment = new CCorrSegment(frsubcpl, tosubcpl);
        //        CorrespondSegmentLk.Add(CorrespondSegment);
        //        return;
        //    }
        //}

        #region Display
        public void DisplayAtBd(double dblProp)
        {
            CParameterInitialize pParameterInitialize = _ParameterInitialize;
            CParameterResult pParameterResult = _ParameterResult;
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return;
            }

            double dblLargerScale = pParameterInitialize.dblLargerScale;
            double dblSmallerScale = pParameterInitialize.dblSmallerScale;
            double dblTargetScale = Math.Pow(dblLargerScale, 1 - dblProp) * Math.Pow(dblSmallerScale, dblProp);
            double dblIgnorableDis = 0.0001 * dblTargetScale / 111319.490793;
            //double dblIgnorableDis = 0.0001 * dblTargetScale / 100000000000;
            //
            long lngTime3 = System.Environment.TickCount;
            List<CPolyline> normaldisplaycpllt = new List<CPolyline>(pParameterResult.CBSAtBdLt.Count);
            for (int i = 0; i < pParameterResult.CBSAtBdLt.Count; i++)
            { 
                CPolyline cpl = CGeoFunc.GetTargetcpl(i, pParameterResult.CBSAtBdLt[i].CResultPtLt, dblProp);
                normaldisplaycpllt.Add(cpl);
            }
            pParameterResult.DisplayCPlLt = normaldisplaycpllt;
            long lngInterpolateTime = System.Environment.TickCount - lngTime3;

            //the polylines should be faded out
            List<CPolyline> fadeddisplaycpllt = new List<CPolyline>(pParameterResult.CSgAtBdLt.Count);
            for (int i = 0; i < pParameterResult.CSgAtBdLt.Count; i++)
            {
                CAtBd pAtBd = pParameterResult.CSgAtBdLt[i];
                //List<CPoint> newcptlt = _pDPSimplify.DPCplMorph(pAtBd as CPolyline, dblProp).CptLt;
                List<CPoint> newcptlt = new List<CPoint>();

                //the first point
                if (pAtBd.Frcpt2 == null)
                {
                    newcptlt[0] = pAtBd.CptLt[0];
                }
                else
                {
                    newcptlt[0] = CGeoFunc.GetInbetweenCpt(pAtBd.Frcpt2, pAtBd.Frcpt2.CorrespondingPtLt[0], dblProp, 0);
                    int dblBSCplID = pAtBd.Frcpt2.BelongedCpl.ID;
                    DWIntersect(normaldisplaycpllt[dblBSCplID], ref newcptlt, dblIgnorableDis);
                }

                //the last point
                if (pAtBd.Tocpt2 == null)
                {
                    newcptlt[newcptlt.Count - 1] = pAtBd.CptLt[pAtBd.CptLt.Count - 1];
                }
                else
                {
                    CPoint cptlast = CGeoFunc.GetInbetweenCpt(pAtBd.Tocpt2, pAtBd.Tocpt2.CorrespondingPtLt[0], dblProp, pAtBd.CptLt.Count - 1);
                    newcptlt[newcptlt.Count - 1] = cptlast;
                    CHelpFunc.ReverseCptLt(ref newcptlt);
                    int dblBSCplID = pAtBd.Tocpt2.BelongedCpl.ID;
                    DWIntersect(normaldisplaycpllt[dblBSCplID], ref newcptlt, dblIgnorableDis);
                    CHelpFunc.ReverseCptLt(ref newcptlt);
                }

                CPolyline newcpl = new CPolyline(i, newcptlt);
                fadeddisplaycpllt.Add(newcpl);
            }
            pParameterResult.FadedDisplayCPlLt = fadeddisplaycpllt;

            //// 清除绘画痕迹
            //IMapControl4 m_mapControl = _ParameterInitialize.m_mapControl;
            //IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            //pGra.DeleteAllElements();
            for (int i = 0; i < normaldisplaycpllt.Count ; i++)
            {
                normaldisplaycpllt[i].SetPolyline();
            }
            for (int i = 0; i < fadeddisplaycpllt.Count; i++)
            {
                fadeddisplaycpllt[i].SetPolyline();
            }

            ////check
            //List<CPolyline> TopCheckCpllt = new List<CPolyline>(normaldisplaycpllt.Count + fadeddisplaycpllt.Count);
            //TopCheckCpllt.AddRange(normaldisplaycpllt);
            //TopCheckCpllt.AddRange(fadeddisplaycpllt);
            //SCG.LinkedList<CCorrSegment> pCorrCplLk;
            //SCG.LinkedList<CPolyline> pSelfIntersectCplLk;
            //bool isCross = CGeoFunc.CheckCross(TopCheckCpllt, out pCorrCplLk);
            //bool isSelfIntersect = CGeoFunc.CheckSelfIntersect(TopCheckCpllt, out pSelfIntersectCplLk);

            //CHelpFunc.SaveCPlLt(normaldisplaycpllt, "City" + pParameterInitialize.strSaveFolder + "_" + dblProp.ToString(), pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);
            //int intColor = Convert.ToInt16(dblProp * 255);
            //CHelpFunc.SaveCPlLt(fadeddisplaycpllt, CEnumScale.Single + pParameterInitialize.strSaveFolder + "_" + dblProp.ToString(), pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl, intColor, intColor, intColor, 1);

            //CEnvelope pEnvelopeIpe = new CEnvelope(0, 0, 600, 600);
            //CHelpFunc.SaveToIpe(normaldisplaycpllt,  "Ipe" + "_" + dblProp.ToString(), pParameterInitialize.pBSFLayer.AreaOfInterest, pEnvelopeIpe, pParameterInitialize, 0, 0, 0, "normal", true);
            //CHelpFunc.SaveToIpe(fadeddisplaycpllt, "Ipe" + "_" + dblProp.ToString(), pParameterInitialize.pBSFLayer.AreaOfInterest, pEnvelopeIpe, pParameterInitialize, intColor, intColor, intColor, "normal", true);









            //for (int i = 0; i < normaldisplaycpllt.Count; i++)
            //{
            //    CDrawInActiveView.ViewPolylineByRGB(m_mapControl, normaldisplaycpllt[i], 0, 0, 0, 1);
            //}

            //
            //for (int i = 0; i < fadeddisplaycpllt.Count; i++)
            //{
            //    CDrawInActiveView.ViewPolylineByRGB(m_mapControl, fadeddisplaycpllt[i], intColor, intColor, intColor, 1);
            //}
            //m_mapControl.ActiveView.Refresh();   //由于在下一步“ViewPolyline”中有刷新的命令，此语句可省略

        }


        private void DWIntersect(CPolyline pBSCpl, ref List<CPoint> cptlt, double dblIgnorableDis)
        {
            IPointCollection4 pCol = new PolylineClass();
            for (int i = 0; i < cptlt.Count; i++)
            {
                cptlt[i].SetPoint();
                pCol.AddPoint(cptlt[i].pPoint);
            }
            IPolyline5 ipl = pCol as IPolyline5;
            pBSCpl.SetPolyline();
            IRelationalOperator pRel = pBSCpl.pPolyline as IRelationalOperator;
            bool isCrosses = pRel.Crosses(ipl);
            if (isCrosses == true)
            {
                ITopologicalOperator pTop = pBSCpl.pPolyline as ITopologicalOperator;
                IGeometry pGeoIntersect = pTop.Intersect(ipl, esriGeometryDimension.esriGeometry0Dimension);
                IPointCollection4 pColIntersect = pGeoIntersect as IPointCollection4;

                double dblMaxDis = 0;
                for (int j = 0; j < pColIntersect.PointCount; j++)
                {                   
                    double dblDis = CGeoFunc.CalDistanceFromStartPoint(ipl, pColIntersect.get_Point(j), false);
                    if (dblDis > dblMaxDis)
                    {
                        dblMaxDis = dblDis;
                    }
                }

                ICurve pSubCurve;
                ipl.GetSubcurve(dblMaxDis, ipl.Length, false, out pSubCurve);
                //IPolyline5 Cutipl = pSubCurve as IPolyline5;
                IPointCollection4 iplCutCol = pSubCurve as IPointCollection4;

                //the new first segment
                IPointCollection4 pSegCol = new PolylineClass();
                pSegCol.AddPoint(ipl.FromPoint, _Missing, _Missing);
                pSegCol.AddPoint(iplCutCol.get_Point(1), _Missing, _Missing);
                IPolyline5 seg = pSegCol as IPolyline5;
                bool isCrossesSeg;

                int intIndex = 0;
                while (intIndex < iplCutCol.PointCount - 1)
                {
                    intIndex++;
                    pSegCol.UpdatePoint(1, iplCutCol.get_Point(intIndex));
                    isCrossesSeg = pRel.Crosses(seg);
                    if (isCrossesSeg == false)
                    {
                        iplCutCol.RemovePoints(1, intIndex - 1);
                        iplCutCol.UpdatePoint(0, ipl.FromPoint);
                        break;
                    }
                    if (seg.Length >= dblIgnorableDis)
                    {
                        double dblOriginalIntersectionDis = CGeoFunc.CalDistanceFromStartPoint(pBSCpl.pPolyline, pCol.get_Point (0), false);
                        double dblRealisticIntersectionDis = CGeoFunc.CalDistanceFromStartPoint(pBSCpl.pPolyline, iplCutCol.get_Point(0), false);

                        IPointCollection4 pColBSCpl = pBSCpl.pPolyline as IPointCollection4;
                        double dblSumDis = 0;
                        for (int i = 0; i < pColBSCpl.PointCount-1; i++)
                        {
                            double dblDis=CGeoFunc.CalDis(pColBSCpl.get_Point(i), pColBSCpl.get_Point(i + 1));
                            dblSumDis += dblDis;
                            if (dblSumDis>=dblRealisticIntersectionDis)
                            {
                                double dblDisPre = Math.Abs(dblSumDis - dblDis - dblOriginalIntersectionDis);
                                double dblDisNext = Math.Abs(dblSumDis - dblOriginalIntersectionDis);
                                IPoint intersectpt = new PointClass();
                                if (dblDisPre <= dblDisNext)
                                {
                                    intersectpt = pColBSCpl.get_Point(i);
                                }
                                else
                                {
                                    intersectpt = pColBSCpl.get_Point(i+1);
                                }
                                iplCutCol.UpdatePoint(0, intersectpt);
                                break;
                            }
                        }
                        break;
                    }
                }
                cptlt = CHelpFunc.GetCptEbByICol(iplCutCol).ToList();
            }
        }
        #endregion
       


        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
