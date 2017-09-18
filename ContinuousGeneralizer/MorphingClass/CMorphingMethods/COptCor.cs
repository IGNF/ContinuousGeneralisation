using System;
using System.Diagnostics;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;

using MorphingClass.CEntity;
using MorphingClass.CEvaluationMethods ;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CMorphingMethods
{
    /// <summary>COptCor</summary>
    /// <remarks>
    /// </remarks>
    public class COptCor : CMorphingBaseCpl
    {

        //protected CEvaluation _Evaluation = new CEvaluation();

        protected int _intMaxBackKforI = 5;
        protected int _intMulti = 1;  //sometimes we need to look for the smallest sufficient look-back parameter, so we need to test a series (_intMulti) look-back parameters
        protected int _intIncrease = 1;
        protected List < List<CPoint>> FrCptLtLt { get; set; }
        protected List<List<CPoint>>  ToCptLtLt { get; set; }

        delegate CTable DlgCreateTable(List<CPoint> frcptlt, List<CPoint> tocptlt, int intMaxBackKforI, int intMaxBackKforJ);

        //protected 

        public COptCor()
        {
            //Enumerable
        }


        public COptCor(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline, CPolyline>(ParameterInitialize, blnIGeoToCGeo : false);

            this.FrCptLtLt = CHelpFunc.GetCptEbEbByIColEb(this.ObjIGeoLtLt[0]).ToLtLt();
            this.ToCptLtLt = CHelpFunc.GetCptEbEbByIColEb(this.ObjIGeoLtLt[1]).ToLtLt();
            
            this.ObjIGeoLtLt=null ;  //to save memory
        }


        


        /// <summary>多个结果的Morphing方法</summary>
        /// <remarks>对于指定的回溯参数K，不断计算增大的结果，直到Translation值稳定</remarks>
        public CParameterResult OptCorMorphing()
        {
            CParameterInitialize pParameterInitialize = _ParameterInitialize;

            var pFrCptLtLt = this.FrCptLtLt;
            var pToCptLtLt = this.ToCptLtLt;
            var pObjValueLtLtLt = this.ObjValueLtLtLt;

            //var intFieldIndex = pParameterInitialize.pFLayerLt[0].FeatureClass.FindField(pParameterInitialize.txtAttributeOfKnown.Text);
            //if (intFieldIndex == -1)  //try to add an attribute, if this attribute is already existed, then do nothing, otherwise, add this attribute and set value to -1
            //{
            //    intFieldIndex = CSaveFeature.AddField(pParameterInitialize.pFLayerLt[0].FeatureClass, esriFieldType.esriFieldTypeDouble, pParameterInitialize.txtAttributeOfKnown.Text);
            //    //CSaveFeature.SetFieldValue(pParameterInitialize.pFLayerLt[0].FeatureClass, intFieldIndex, -1);

            //    this.strFieldNameLtLt[0].Add(pParameterInitialize.txtAttributeOfKnown.Text);
            //    this.esriFieldTypeLtLt[0].Add(esriFieldType.esriFieldTypeDouble);

            //    for (int i = 0; i < pFrCptLtLt.Count; i++)
            //    {
            //        pObjValueLtLtLt[0][i].Add(0);
            //    }
            //}


            int intMaxBackKforI = Convert.ToInt32(pParameterInitialize.txtMaxBackK.Text);
            int intMaxBackKforJ = SetintMaxBackKforJ(pParameterInitialize.cboIntMaxBackKforJ.SelectedIndex, intMaxBackKforI);
            int intMulti = Convert.ToInt32(pParameterInitialize.txtMulti.Text);
            int intIncrease = Convert.ToInt32(pParameterInitialize.txtIncrease.Text);
            CConstants .strMethod = pParameterInitialize.cboMorphingMethod.Text;

            //delegation of morphing method
            DlgCreateTable dlgCreateTable = SetDlgCreateTable(pParameterInitialize.cboMorphingMethod.Text);
            _pEvaluation = new CEvaluation(pParameterInitialize.cboEvaluationMethod.SelectedIndex);
            var StandardVectorCpt = SetStandardVectorCpt(pParameterInitialize.cboStandardVector.SelectedIndex, pFrCptLtLt[0].First(), pToCptLtLt[0].First());

            long lngStartTime1 = System.Environment.TickCount;  //lngTime1

            Stopwatch pStopwatch1 = Stopwatch.StartNew();
            foreach (var cptlt in pFrCptLtLt)
            {
                CGeoFunc.SetEdgeLengthOnToCpt(cptlt);
            }
            foreach (var cptlt in pToCptLtLt)
            {
                CGeoFunc.SetEdgeLengthOnToCpt(cptlt);
            }
            CGeoFunc.IntegrateStandardVectorCpt(pFrCptLtLt, StandardVectorCpt);
            
            //generate cptltltlt
            var frcptltltlt = new List<List<List<CPoint>>>(pFrCptLtLt.Count);
            var tocptltltlt = new List<List<List<CPoint>>>(pToCptLtLt.Count);
            SetCptLtLtLt(pParameterInitialize.chkCoincidentPoints.Checked, pFrCptLtLt, pToCptLtLt, ref frcptltltlt, ref  tocptltltlt);
            pStopwatch1.Stop();


            long lngTimeSetLength = System.Environment.TickCount - lngStartTime1;  //lngTime1

            var dblDistanceLt = new List<double>(intMulti);
            var dblTimeLt = new List<double>(intMulti);

            var pCorrCptsLtLt = new List<List<CCorrCpts>>(pFrCptLtLt.Count);  //we only record the last one, when i = intMulti -1
            var pCtrlCptsLtLt = new List<List<CCorrCpts>>(pToCptLtLt.Count);   //we only record the last one, when i = intMulti -1


            for (int i = 0; i < intMulti; i++)
            {
                //CSaveFeature CsfCtrlpl = new CSaveFeature(esriGeometryType.esriGeometryPolyline, pParameterInitialize.strMorphingMethod + "CtrlLine" + "_" + intMaxBackKforI, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl, blnVisible: false);
                //CSaveFeature CsfCorrpl = new CSaveFeature(esriGeometryType.esriGeometryPolyline, pParameterInitialize.strMorphingMethod + "CorrLine" + "_" + intMaxBackKforI, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl, blnVisible: false);
                //CSaveFeature CsfInter1 = new CSaveFeature(esriGeometryType.esriGeometryPolyline, "Inter" + pParameterInitialize.pFLayerLt[1].Name + "_" + intMaxBackKforI, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl, blnVisible: false);
                //CSaveFeature CsfInter0 = new CSaveFeature(esriGeometryType.esriGeometryPolyline, "Inter" + pParameterInitialize.pFLayerLt[0].Name + "_" + intMaxBackKforI, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl, blnVisible: false);
                //CExcelSaver pExcelSaver = new CExcelSaver(pParameterInitialize.strSavePathBackSlash + "TimeRecorder_" + intMaxBackKforI);

                //Stopwatch pStopwatch2 = Stopwatch.StartNew();
                double dblTotalDistance = 0;
                long lngTotalOptCorTime=0;

                IFeatureCursor pFeatureCursor = pParameterInitialize.pFLayerLt[0].FeatureClass.Search(null, false);
                for (int j = 0; j < pFrCptLtLt.Count; j++)   //for every polyline                
                //for (int j = 0; j < 100; j++)
                {
                    Stopwatch pStopwatch2 = Stopwatch.StartNew();
                    double dblDistance = 0;
                    //double dblComputedDis = Convert.ToDouble(pObjValueLtLtLt[0][j][intFieldIndex]);
                    double dblComputedDis = 0;
                    if (dblComputedDis > 0 && intMaxBackKforI >= (pFrCptLtLt[j].Count - 1) && intMaxBackKforJ >= (pToCptLtLt[j].Count - 1))
                    {
                        dblDistance = dblComputedDis;                        
                    }
                    else
                    {
                        var frcptltlt = frcptltltlt[j];   //cptltlt are all the points of a polyline
                        var tocptltlt = tocptltltlt[j];

                        var TableLt = new List<CTable>(frcptltlt.Count);

                        Console.WriteLine("intMulti= " + i + ",  Line ID:" + j);

                        for (int k = 0; k < frcptltlt.Count; k++)
                        {
                            //Console.WriteLine("intMulti= " + i + ",  Line ID:" + j + ",  Segment ID:" + k);
                            var Table = dlgCreateTable(frcptltlt[k], tocptltlt[k], intMaxBackKforI, intMaxBackKforJ);
                            dblDistance += Table.dblEvaluation;

                            //Console.WriteLine("cost:  " + dblDistance);
                            TableLt.Add(Table);
                        }
                        dblDistance /= _pEvaluation.dblCorrection;

                        pStopwatch2.Stop();
                        lngTotalOptCorTime += pStopwatch2.ElapsedMilliseconds;

                        CGeoFunc.RemoveStandardVectorCpt(pFrCptLtLt [j], StandardVectorCpt);
                        List<CCorrCpts> CtrlCptsLt;
                        var pCorrCptsLt = GetCorrespondences(TableLt, frcptltlt, tocptltlt, pFrCptLtLt[j].Count, pToCptLtLt[j].Count, out CtrlCptsLt);
                        pCorrCptsLtLt.Add(pCorrCptsLt);
                        pCtrlCptsLtLt.Add(CtrlCptsLt);


                        CHelpFunc.SetMoveVectorForCorrCptsLt(pCorrCptsLt);   //this will also set MoveVector for CtrlCptsLt
                        //CsfInter0.SaveIGeoEbToLayer(CHelpFunc.MakeEb(1, GenerateInterpolatedIPl(pCorrCptsLt, 0)));
                        //CsfInter1.SaveIGeoEbToLayer(CHelpFunc.MakeEb(1, GenerateInterpolatedIPl(pCorrCptsLt, 1)));
                        //CsfCtrlpl.SaveIGeoEbToLayer(GenerateCorrIPlEb(CtrlCptsLt));
                        //CsfCorrpl.SaveIGeoEbToLayer(GenerateCorrIPlEb(pCorrCptsLt));
                        
                        //if the look-back parameter is larger than the points number of a pair of polylines, than we don't need to try a larger look-back parameter
                        //Therefore, we record the distance for the two polylines
                        //if (intMaxBackKforI >= (pFrCptLtLt[j].Count - 1) && intMaxBackKforJ >= (pToCptLtLt[j].Count - 1))
                        //{
                        //    pObjValueLtLtLt[0][j][intFieldIndex] = dblDistance;
                        //    for (int l = intFeatureNum; l < pFrCptLtLt.Count; l++)  //find the feature and record it
                        //    {
                        //        IFeature pFeature = pFeatureCursor.NextFeature();
                        //        if (j == l)
                        //        {
                        //            pFeature.set_Value(intFieldIndex, dblDistance);
                        //            pFeature.Store();
                        //            intFeatureNum = j + 1;  //we record the index, so that next time we can start from this index
                        //            break;
                        //        }
                        //    }
                        //}
                        //pExcelSaver.WriteLine(CHelpFunc.MakeEb<object>(5, j, pLSCPlLt[j].CptLt.Count, pSSCPlLt[j].CptLt.Count, dblDistance, pStopwatch2.ElapsedMilliseconds));
                    }
                    dblTotalDistance += dblDistance;
                }

                dblTimeLt.Add(pStopwatch1.ElapsedMilliseconds + lngTotalOptCorTime);
                dblDistanceLt.Add(dblTotalDistance);
                //pExcelSaver.Close ();
                //CHelpFuncExcel.ExportDataltltToExcel(tablevalueltlt, intMaxBackKforI + "Tableltlt0", pParameterInitialize.strSavePath);
                //保存对应线
                CHelpFuncExcel.ExportDataltToExcel(dblTimeLt, intMaxBackKforI + "Timelt0", pParameterInitialize.strSavePath);
                CHelpFuncExcel.ExportDataltToExcel(dblDistanceLt, intMaxBackKforI + "Distancelt0", pParameterInitialize.strSavePath);

                CHelpFunc.Displaytspb(i + 1, intMulti);
                pParameterInitialize.txtEvaluation.Text = dblDistanceLt.GetLastT().ToString();
                pParameterInitialize.tsslTime.Text = "Running Time: " + dblTimeLt.GetLastT().ToString();

                intMaxBackKforI = intMaxBackKforI + intIncrease;
                _CorrCptsLtLt = pCorrCptsLtLt;
            }


            double dblStandardLength = CGeoFunc.CalDis(0, 0, StandardVectorCpt.X, StandardVectorCpt.Y);
            intMaxBackKforI--;
            CHelpFunc.SaveCtrlLine(pCtrlCptsLtLt, intMaxBackKforI + "_" + CConstants.strMethod + "CtrlLine", dblStandardLength, pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);
            CHelpFunc.SaveCorrLine(pCorrCptsLtLt, intMaxBackKforI + "_" + CConstants.strMethod + "CorrLine", pParameterInitialize.pWorkspace, pParameterInitialize.m_mapControl);

            CHelpFunc.SetMoveVectorForCorrCptsLtLt(_CorrCptsLtLt);

            //the results will be recorded in _ParameterResult
            CParameterResult ParameterResult = new CParameterResult();
            ParameterResult.pMorphingBaseCpl = this as CMorphingBaseCpl;
            ParameterResult.pMorphingBase = this as CMorphingBase;
            _ParameterResult = ParameterResult;
            return ParameterResult;
        }




        ///// <summary>创建T矩阵</summary>
        ///// <param name="frcpl">大比例尺线状要素</param>
        ///// <param name="tocpl">小比例尺线状要素</param> 
        ///// <param name="CFrEdgeLt">大比例尺线段（可能只是线状要素的一部分）</param>  
        /////  <param name="CToEdgeLt">小比例尺线段（可能只是线状要素的一部分）</param> 
        ///// <param name="intMaxBackKforI">回溯系数</param> 
        ///// <returns>对应线段</returns>
        //public C5.LinkedList<CCorrespondSegment> DWByOptCor(CPolyline frcpl, CPolyline tocpl, List<CPolyline> CFrEdgeLt, List<CPolyline> CToEdgeLt, int intMaxBackKforI)
        //{
        //    List<CPolyline> frlastcpllt = new List<CPolyline>(frcpl.CptLt.Count - 1);
        //    List<CPolyline> tolastcpllt = new List<CPolyline>(frcpl.CptLt.Count - 1);
        //    CTable Table = CreatT(frcpl, tocpl, CFrEdgeLt, CToEdgeLt, ref frlastcpllt, ref tolastcpllt, intMaxBackKforI);  //创建T矩阵
        //    C5.LinkedList<CCorrespondSegment> pCorrespondSegmentLk = GetCorrespondences(T, frcpl, tocpl, CFrEdgeLt, CToEdgeLt, frlastcpllt, tolastcpllt);

        //    return pCorrespondSegmentLk;

        //}








        /// <summary>创建T矩阵</summary>
        /// <param name="frcpl">大比例尺线状要素</param>
        /// <param name="tocpl">小比例尺线状要素</param> 
        /// <param name="intMaxBackKforI">回溯系数</param> 
        /// <returns>T矩阵</returns>
        public virtual CTable CreatTableOptCor(List<CPoint> frcptlt, List<CPoint> tocptlt, int intMaxBackKforI, int intMaxBackKforJ)
        {
            int intFrPtNum = frcptlt.Count;
            int intToPtNum = tocptlt.Count;

            //it would be nice if we could calculate these arrays previously, but we don't have so much memory
            //CPolyline[,] afrsubcpl = CreateSubCpl(frcpl, intMaxBackKforI);
            //CPolyline[,] atosubcpl = CreateSubCpl(tocpl, intMaxBackKforJ);

            //T.aCell[i, j].dblEvaluation is actually the cost till the ends of i segments on polyline frcpl and j segments on polyline tocpl
            CTable Table = new CTable(intFrPtNum, intToPtNum);

            //we do this separately because we don't need to traverse back for the first row and first col
            CalTFisrtRowCol(ref Table, frcptlt, tocptlt);

            for (int i = 1; i < intFrPtNum; i++)               //计算各空格值
            {
                int intBackKforI = Math.Min(i, intMaxBackKforI);   //程序刚开始执行时，之前已遍历过线段数较少，可能小于intMaxBackKforI
                for (int j = 1; j < intToPtNum; j++)
                {
                    int intBackKforJ = Math.Min(j, intMaxBackKforJ);   //程序刚开始执行时，之前已遍历过线段数较少，可能小于intMaxBackKforI

                    CCell minCCell = new CCell();
                    minCCell.dblEvaluation = CConstants.dblHalfDoubleMax;

                    minCCell = CHelpFunc.Min(TraverseBack(ref Table, i, j, frcptlt, tocptlt, 0, 1, 0, 1), minCCell, table => table.dblEvaluation);  //including 0 to 1, 1 to 0, 1 to 1
                    minCCell = CHelpFunc.Min(TraverseBack(ref Table, i, j, frcptlt, tocptlt, 2, intBackKforI, 1, 1), minCCell, table => table.dblEvaluation); //including 2 to 1 until intBackKforI to 1
                    minCCell = CHelpFunc.Min(TraverseBack(ref Table, i, j, frcptlt, tocptlt, 1, 1, 2, intBackKforJ), minCCell, table => table.dblEvaluation); //including 1 to 2 until 1 to intBackKforJ

                    Table.aCell[i, j] = minCCell;
                }
            }
            return Table;
        }

        /// <summary>
        /// use a dynamic programming algorithm to compute correspondences between two polylines
        /// </summary>
        /// <param name="frcpl">the larger-scale polyline</param>
        /// <param name="tocpl">the smaller-scale polyline</param>
        /// <param name="intMaxBackKforI">look-back parameter for the larger-scale polyline</param>
        /// <param name="intMaxBackKforJ">look-back parameter for the smaller-scale polyline</param>
        /// <param name="StandardVectorCpt">if frcpl and tocpl are not in the same place, we use StandardVectorCpt to offset their distance</param>
        /// <returns>two dimensional table which we can traverse back to construct correspondences between the two polylines</returns>
        /// <remarks>to guarantee that there are the same number of points on the two polylines after interpolation, 
        ///          we only   allow  correspondences of one segment to one segment, or one segment to multi-segment;
        ///          we do NOT allow a correspondence of one segment to a single point.</remarks>
        public virtual CTable CreatTableOptCorSimplified(List<CPoint> frcptlt, List<CPoint> tocptlt, int intMaxBackKforI, int intMaxBackKforJ)
        {
            int intFrPtNum = frcptlt.Count;
            int intToPtNum = tocptlt.Count;

            AdjustMaxBackK(ref intMaxBackKforI, intMaxBackKforI, intFrPtNum, intToPtNum);  //if we used a relatively small intMaxBackKforJ, then we check whether this intMaxBackKforJ would be enough to avoid point-segment correspondences
            AdjustMaxBackK(ref intMaxBackKforJ, intMaxBackKforI, intToPtNum, intFrPtNum);  //if we used a relatively small intMaxBackKforJ, then we check whether this intMaxBackKforJ would be enough to avoid point-segment correspondences

            //T is the two dimensional table, which will be returned as the result.
            //T.aCell[i, j].dblEvaluation is actually the cost till the ends of i segments on polyline frcpl and j segments on polyline tocpl
            CTable Table = new CTable(intFrPtNum, intToPtNum);

            //we compute the first row and column of table Table here because we don't need to traverse back
            CalTFisrtRowCol(ref Table, frcptlt, tocptlt);

            //We do this to handle the case that the look-back parameter is not enough, then we will have to allow segment-point matching.
            //For example: if there are 25 segments on frcpl and 5 segments on tocpl, then we need intMaxBackKforI >= 5;
            //if our intMaxBackKforI == 3, then we can only build the correspondences between the last 15 segments of frcpl and the 5 segments of tocpl.
            //The first 10 segments of frcpl have to correspond to the first point of tocpl.
            for (int i = 1; i < intFrPtNum; i++)
            {
                Table.aCell[i, 0].dblEvaluation = Table.aCell[i, 0].dblEvaluation + CConstants.dblHalfDoubleMax;  //CConstants.dblHalfDoubleMax is a very large number
            }
            for (int j = 1; j < intToPtNum; j++)
            {
                Table.aCell[0, j].dblEvaluation = Table.aCell[0, j].dblEvaluation + CConstants.dblHalfDoubleMax;  //CConstants.dblHalfDoubleMax is a very large number
            }

            //compute other rows and columns of table Table
            for (int i = 1; i < intFrPtNum; i++)
            {
                int intBackKforI = Math.Min(i, intMaxBackKforI);   //at some beginning steps, the number of traversed segments is smaller than the look-back parameter
                for (int j = 1; j < intToPtNum; j++)
                {
                    int intBackKforJ = Math.Min(j, intMaxBackKforJ);   //at some beginning steps, the number of traversed segments is smaller than the look-back parameter

                    CCell minCCell = new CCell();
                    minCCell.dblEvaluation = CConstants.dblHalfDoubleMax;  //CConstants.dblHalfDoubleMax is a very large number

                    minCCell = CHelpFunc.Min(TraverseBack(ref Table, i, j, frcptlt, tocptlt, 1, 1, 1, 1), minCCell, table => table.dblEvaluation);  //only including 1 segment to 1 segment
                    minCCell = CHelpFunc.Min(TraverseBack(ref Table, i, j, frcptlt, tocptlt, 2, intBackKforI, 1, 1), minCCell, table => table.dblEvaluation); //including 2 segments to 1 segment until intBackKforI segments to 1 segment
                    minCCell = CHelpFunc.Min(TraverseBack(ref Table, i, j, frcptlt, tocptlt, 1, 1, 2, intBackKforJ), minCCell, table => table.dblEvaluation); //including 1 segment to 2 segments until 1 segment to intBackKforJ segments

                    Table.aCell[i, j] = minCCell;
                }
            }

            return Table;
        }

        /// <summary>创建T矩阵</summary>
        /// <param name="frcpl">大比例尺线状要素</param>
        /// <param name="tocpl">小比例尺线状要素</param> 
        /// <param name="intMaxBackKforI">回溯系数</param> 
        /// <returns>T矩阵</returns>
        public CTable CreatTableOptCorMM(List<CPoint> frcptlt, List<CPoint> tocptlt, int intMaxBackKforI, int intMaxBackKforJ)
        {
            int intFrPtNum = frcptlt.Count;
            int intToPtNum = tocptlt.Count;

            //注意：T矩阵中的序号跟原文算法中的序号是统一的，但线数组中的序号则应减1
            CTable Table = new CTable(intFrPtNum, intToPtNum);  //T.aCell[i, j] is actually the value of the first i segments and j segments

            //we do this separately because we don't need to traverse back for the first row and first col
            CalTFisrtRowCol(ref Table, frcptlt, tocptlt);

            for (int i = 1; i < intFrPtNum; i++)               //计算各空格值
            {
                int intBackKforI = Math.Min(i, intMaxBackKforI);   //程序刚开始执行时，之前已遍历过线段数较少，可能小于intMaxBackKforI
                for (int j = 1; j < intToPtNum; j++)
                {
                    int intBackKforJ = Math.Min(j, intMaxBackKforJ);   //程序刚开始执行时，之前已遍历过线段数较少，可能小于intMaxBackKforI                    
                    Table.aCell[i, j] = TraverseBack(ref Table, i, j, frcptlt, tocptlt, 0, intBackKforI, 0, intBackKforJ);
                }
            }

            return Table;
        }


        /// <summary>创建T矩阵</summary>
        /// <param name="frcpl">大比例尺线状要素</param>
        /// <param name="tocpl">小比例尺线状要素</param> 
        /// <param name="intMaxBackKforI">回溯系数</param> 
        /// <returns>T矩阵</returns>
        /// <remarks></remarks>
        public CTable CreatTableOptCorMMSimplified(List<CPoint> frcptlt, List<CPoint> tocptlt, int intMaxBackKforI, int intMaxBackKforJ)
        {
            int intFrPtNum = frcptlt.Count;
            int intToPtNum = tocptlt.Count;

            //注意：T矩阵中的序号跟原文算法中的序号是统一的，但线数组中的序号则应减1
            CTable Table = new CTable(intFrPtNum, intToPtNum);  //T.aCell[i, j] is actually the value of the first i segments and j segments

            //we do this separately because we don't need to traverse back for the first row and first col
            CalTFisrtRowCol(ref Table, frcptlt, tocptlt);

            //we do this because when the look-back parameter is not enough, we have to "allow" 0-1 matching 
            for (int i = 1; i < intFrPtNum; i++)
            {
                Table.aCell[i, 0].dblEvaluation = Table.aCell[i, 0].dblEvaluation + CConstants.dblHalfDoubleMax;
            }

            for (int j = 1; j < intToPtNum; j++)
            {
                Table.aCell[0, j].dblEvaluation = Table.aCell[0, j].dblEvaluation + CConstants.dblHalfDoubleMax;
            }

            for (int i = 1; i < intFrPtNum; i++)               //计算各空格值
            {
                int intBackKforI = Math.Min(i, intMaxBackKforI);
                for (int j = 1; j < intToPtNum; j++)
                {
                    int intBackKforJ = Math.Min(j, intMaxBackKforJ);
                    //T.aCell[i, j] = TraverseBack(ref Table, i, j, frcptlt, tocptlt, intBackKforI, intBackKforJ, 1, 1);
                    Table.aCell[i, j] = TraverseBack(ref Table, i, j, frcptlt, tocptlt, 1, intBackKforI, 1, intBackKforJ);
                }
            }

            return Table;
        }

        /// <summary>
        /// we compute the first row and column of table Table here because we don't need to traverse back
        /// </summary>
        private void CalTFisrtRowCol(ref CTable Table, List<CPoint> frcptlt, List<CPoint> tocptlt)
        {
            Table.aCell[0, 0] = new CCell(0, 0, 0);

            for (int j = 1; j < tocptlt.Count; j++)
            {
                Table.aCell[0, j] = TraverseBack(ref Table, 0, j, frcptlt, tocptlt, 0, 0, 1, 1);
            }

            for (int i = 1; i < frcptlt.Count; i++)
            {
                Table.aCell[i, 0] = TraverseBack(ref Table, i, 0, frcptlt, tocptlt, 1, 1, 0, 0);
            }
        }

        private CPolyline[,] CreateSubCpl(CPolyline cpl, int intMaxBackKforI)
        {
            List<CPoint> cptlt = cpl.CptLt;
            CPolyline[,] asubcpl = new CPolyline[cptlt.Count, intMaxBackKforI + 1];
            for (int i = 0; i < cptlt.Count; i++)
            {
                int intBack = Math.Min(i, intMaxBackKforI);
                for (int j = 0; j < intBack + 1; j++)
                {
                    asubcpl[i, j] = cpl.GetSubPolyline(cptlt[i - j], cptlt[i]);
                }
            }

            return asubcpl;
        }

        /// <summary>
        /// TraverseBack
        /// </summary>
        /// <param name="T">the table</param>
        /// <param name="intI">the index of current segment on polyline frcpl</param>
        /// <param name="intJ">the index of current segment on polyline tocpl</param>
        /// <param name="frcptlt">point list of frcpl</param>
        /// <param name="tocptlt">point list of tocpl</param>
        /// <param name="intMinBackKforI">We will traverse back one segment by one segment from intI-intMinBackKforI to intI-intMaxBackKforI</param>
        /// <param name="intMaxBackKforI">We will traverse back one segment by one segment from intI-intMinBackKforI to intI-intMaxBackKforI</param>
        /// <param name="intMinBackKforJ">We will traverse back one segment by one segment from intJ-intMinBackKforJ to intJ-intMaxBackKforJ</param>
        /// <param name="intMaxBackKforJ">We will traverse back one segment by one segment from intJ-intMinBackKforJ to intJ-intMaxBackKforJ</param>
        /// <returns>the CCell with minimum dblEvaluation among all the CTables</returns>
        private CCell TraverseBack(ref CTable T, int intI, int intJ, List<CPoint> frcptlt, List<CPoint> tocptlt, int intMinBackKforI, int intMaxBackKforI, int intMinBackKforJ, int intMaxBackKforJ)
        {
            CCell minCCell = new CCell();
            minCCell.dblEvaluation = CConstants.dblHalfDoubleMax;
            if (intMinBackKforI == 0)   //we do this separately because we don't need to traverse when k1==0
            {
                minCCell = CHelpFunc.Min(CalCell(ref T, intI, intJ, 0, 1, frcptlt, tocptlt), minCCell, table => table.dblEvaluation);
                intMinBackKforI++;
            }

            if (intMinBackKforJ == 0)   //we do this separately because we don't need to traverse when k2==0
            {
                minCCell = CHelpFunc.Min(CalCell(ref T, intI, intJ, 1, 0, frcptlt, tocptlt), minCCell, table => table.dblEvaluation);
                intMinBackKforJ++;
            }

            for (int k1 = intMinBackKforI; k1 <= intMaxBackKforI; k1++)
            {
                for (int k2 = intMinBackKforJ; k2 <= intMaxBackKforJ; k2++)   //k1 and k2 can be 0 at the same time because we have already set T.aCell[i, j].dblEvaluation = double.MaxValue. So it doesn't matter
                {
                    minCCell = CHelpFunc.Min(CalCell(ref T, intI, intJ, k1, k2, frcptlt, tocptlt), minCCell, table => table.dblEvaluation);
                }
            }

            //find the minimum one
            return minCCell;
        }

        private CCell CalCell(ref CTable T, int intI, int intJ, int k1, int k2, List<CPoint> frcptlt, List<CPoint> tocptlt)
        {
            double dblEvaluation = T.aCell[intI - k1, intJ - k2].dblEvaluation + CalDistance(frcptlt.GetRange(intI - k1, k1 + 1), tocptlt.GetRange(intJ - k2, k2 + 1));
            var ccell = new CCell(k1, k2, dblEvaluation);
            //tableij.CorrCptsLt = CorrCptsLtij; //to use memory as little as possible, we don't record CorrCptsLtij

            return ccell;
        }

        /// <summary>compute the cost</summary>
        /// <param name="subfrcptlt">it is allowed that there is only one point in subfrcptlt</param>
        /// <param name="subtocptlt">it is allowed that there is only one point in subfrcptlt</param>
        /// <param name="StandardVectorCpt">if frcpl and tocpl are not in the same place, we use StandardVectorCpt to offset their distance</param>
        /// <returns>the cost</returns>
        protected double CalDistance(List<CPoint> subfrcptlt, List<CPoint> subtocptlt)
        {
            CLIA pLIA = new CLIA(subfrcptlt, subtocptlt, true);  //linear interpolation between subfrcptlt and subtocptlt

            double dblEvaluation = _pEvaluation.CalEvaluationCorr(pLIA.CLI(), _pEvaluation.dlgEvaluationMethod, pLIA.dblFrTotalLength, pLIA.dblToTotalLength);

            return dblEvaluation;
        }


        /// <summary>以回溯的方式寻找对应线段</summary>
        /// <returns>对应线段数组</returns>
        protected List<CCorrCpts> GetCorrespondences(List<CTable> TableLt, List<List<CPoint>> frcptltlt, List<List<CPoint>> tocptltlt, int inFrCptNum, int intToCptNum, out List<CCorrCpts> CtrlCptsLt)
        {

            List<CCorrCpts> CorrCptsLt = new List<CCorrCpts>(inFrCptNum + intToCptNum - 2);
            CtrlCptsLt = new List<CCorrCpts>(Convert.ToInt32(inFrCptNum / intToCptNum) + 1);

            var CCorrCptsFirst = new CCorrCpts(frcptltlt[0][0], tocptltlt[0][0]);
            CorrCptsLt.Add(CCorrCptsFirst);
            CtrlCptsLt.Add(CCorrCptsFirst);  // add the first pair of control points


            for (int i = 0; i < TableLt.Count; i++)
            {
                List<CCorrCpts> subCtrlCptsLt;
                CorrCptsLt.AddRange(GetCorrespondences(TableLt[i], frcptltlt[i], tocptltlt[i], out subCtrlCptsLt));
                CtrlCptsLt.AddRange(subCtrlCptsLt);
            }

            return CorrCptsLt;
        }

        /// <summary>以回溯的方式寻找对应线段</summary>
        /// <returns>对应线段数组</returns>
        protected List<CCorrCpts> GetCorrespondences(CTable Table, List<CPoint> frcptlt, List<CPoint> tocptlt, out List<CCorrCpts> CtrlCptsLt)
        {
            var CPairIntLt = new List<CPairInt>(frcptlt.Count + tocptlt.Count);

            int inti = Table.intRow - 1;
            int intj = Table.intCol - 1;

            //record all the nodes
            int intCtrlCount = 1;  //number of pairs of control points
            while (inti > 0 || intj > 0)
            {
                int intI = inti;
                int intJ = intj;

                CPairIntLt.Add(new CPairInt(intI, intJ));

                inti = inti - Table.aCell[intI, intJ].intBackK1;
                intj = intj - Table.aCell[intI, intJ].intBackK2;

                intCtrlCount++;
            }

            //get results
            List<CCorrCpts> CorrCptsLt = new List<CCorrCpts>(frcptlt.Count + tocptlt.Count - 2);
            CtrlCptsLt = new List<CCorrCpts>(intCtrlCount);
            //var CCorrCptsFirst = new CCorrCpts(frcptlt[0], tocptlt[0]);
            //CorrCptsLt.Add(CCorrCptsFirst);
            //CtrlCptsLt.Add(CCorrCptsFirst);  // add the first pair of control points

            for (int i = CPairIntLt.Count - 1; i >= 0; i--)
            {
                int intI = CPairIntLt[i].Int1;
                int intJ = CPairIntLt[i].Int2;

                var subfrcptlt = frcptlt.GetRange(intI - Table.aCell[intI, intJ].intBackK1, Table.aCell[intI, intJ].intBackK1 + 1);
                var subtocptlt = tocptlt.GetRange(intJ - Table.aCell[intI, intJ].intBackK2, Table.aCell[intI, intJ].intBackK2 + 1);

                CLIA pLIA = new CLIA(subfrcptlt, subtocptlt, true);
                CorrCptsLt.AddRange(pLIA.CLI(false));  //to reduce memory using of Table, we interpolate here again
                CtrlCptsLt.Add(CorrCptsLt.GetLastT());
            }

            return CorrCptsLt;
        }





        private DlgCreateTable SetDlgCreateTable(string strMorphingMethod)
        {
            switch (strMorphingMethod)
            {
                case "OptCor":
                    return CreatTableOptCor;
                case "OptCorSimplified":
                    return CreatTableOptCorSimplified;
                case "OptCorMM":
                    return CreatTableOptCorMM;
                case "OptCorMMSimplified":
                    return CreatTableOptCorMMSimplified;
                default: throw new ArgumentException("An undefined method! ");
            }
        }

        private int SetintMaxBackKforJ(int intSelectedIndex, int intMaxBackKforI)
        {
            switch (intSelectedIndex)
            {
                case 0:
                    return 1;
                case 1:
                    return 2;
                case 2:
                    return 5;
                case 3:
                    return intMaxBackKforI;
                default:
                    throw new ArgumentException("An undefined look-back parameter! In: " + this.ToString() + ".cs   ");
            }
        }


        private void SetCptLtLtLt(bool blnSplit, List<List<CPoint>> frcptltlt, List<List<CPoint>> tocptltlt, ref List<List<List<CPoint>>> frcptltltlt, ref List<List<List<CPoint>>> tocptltltlt)
        {
            if (blnSplit == true)
            {
                SplitByCoincidentPoints(frcptltlt, tocptltlt, ref frcptltltlt, ref tocptltltlt);
            }
            else
            {
                for (int i = 0; i < frcptltlt.Count; i++)
                {
                    List<List<CPoint>> plfrcptltlt = new List<List<CPoint>>();
                    List<List<CPoint>> pltocptltlt = new List<List<CPoint>>(1);

                    plfrcptltlt.Add(frcptltlt[i]);
                    pltocptltlt.Add(tocptltlt[i]);

                    frcptltltlt.Add(plfrcptltlt);
                    tocptltltlt.Add(pltocptltlt);
                }
            }
        }

        private void SplitByCoincidentPoints(List<List<CPoint>> frcptltlt, List<List<CPoint>> tocptltlt, ref List<List<List<CPoint>>> frcptltltlt, ref List<List<List<CPoint>>> tocptltltlt)
        {
            for (int i = 0; i < frcptltlt.Count; i++)
            {
                List<List<CPoint>> subfrcptltlt;
                List<List<CPoint>> subtocptltlt;
                SplitByCoincidentPoints(frcptltlt[i], tocptltlt[i], out subfrcptltlt, out subtocptltlt);
                frcptltltlt.Add(subfrcptltlt);
                tocptltltlt.Add(subtocptltlt);
            }
        }


        private void SplitByCoincidentPoints(List<CPoint> frcptlt, List<CPoint> tocptlt, out List<List<CPoint>> frcptltlt, out List<List<CPoint>> tocptltlt)
        {
            //look for points having the same coordinates
            var tocptltWithoutEnds = new List<CPoint>();
            if (tocptlt.Count > 2)
            {
                tocptltWithoutEnds = tocptlt.GetRange(1, tocptlt.Count - 2);
            }
            var tocptsd = tocptltWithoutEnds.ToSD(cpt => cpt, CCmpCptYX_VerySmall.sComparer);

            LinkedList<CCorrCpts> CtrlCptsLk = new LinkedList<CCorrCpts>();
            CtrlCptsLk.AddLast(new CCorrCpts(frcptlt[0], tocptlt[0]));     //the first pair
            for (int i = 1; i < frcptlt.Count - 1; i++)
            {
                var frcpt = frcptlt[i];
                CPoint outtocpt;
                bool blnGetValue = tocptsd.TryGetValue(frcpt, out outtocpt);
                if (blnGetValue == true)
                {
                    CCorrCpts CtrlCpts = new CCorrCpts(frcpt, outtocpt);
                    CtrlCptsLk.AddLast(CtrlCpts);
                    tocptsd.Remove(frcpt);
                }
            }
            CtrlCptsLk.AddLast(new CCorrCpts(frcptlt.GetLastT(), tocptlt.GetLastT()));   //the last pair

          
            //check whether the CtrlCptsLk is valid
            List<LinkedListNode<CCorrCpts>> InvalidCorrCptsNodeLt = new List<LinkedListNode<CCorrCpts>>();
            var prenode = CtrlCptsLk.First;
            while (prenode.Next != null)
            {
                var prefrcpt = prenode.Value.FrCpt;
                var pretocpt = prenode.Value.ToCpt;

                var currentnode = prenode.Next;
                var currentfrcpt = currentnode.Value.FrCpt;
                var currenttocpt = currentnode.Value.ToCpt;

                if (currentfrcpt.ID < prefrcpt.ID || currenttocpt.ID < pretocpt.ID)
                {
                    InvalidCorrCptsNodeLt.Add(currentnode);
                }

                prenode = currentnode;
            }

            //delelte all the conflict nodes from CtrlCptsLk
            foreach (var CorrCptsNode in InvalidCorrCptsNodeLt)
            {
                var currentfrcpt = CorrCptsNode.Value.FrCpt;
                var currenttocpt = CorrCptsNode.Value.ToCpt;

                while (CorrCptsNode.Previous != null)
                {
                    var prenode2 = CorrCptsNode.Previous;
                    var prefrcpt = prenode2.Value.FrCpt;
                    var pretocpt = prenode2.Value.ToCpt;

                    if (currentfrcpt.ID < prefrcpt.ID || currenttocpt.ID < pretocpt.ID)
                    {
                        CtrlCptsLk.Remove(prenode2);
                    }
                    else
                    {
                        break;
                    }
                }
                CtrlCptsLk.Remove(CorrCptsNode);
                Console.WriteLine("Invalid splits of polylines happened!");
            }

            frcptltlt = new List<List<CPoint>>(CtrlCptsLk.Count - 1);
            tocptltlt = new List<List<CPoint>>(CtrlCptsLk.Count - 1);

            var prenode3 = CtrlCptsLk.First;
            while (prenode3.Next != null)
            {
                var prefrcpt = prenode3.Value.FrCpt;
                var pretocpt = prenode3.Value.ToCpt;

                var currentnode = prenode3.Next;
                var currentfrcpt = currentnode.Value.FrCpt;
                var currenttocpt = currentnode.Value.ToCpt;

                var subfrcptlt = frcptlt.GetRange(prefrcpt.ID, currentfrcpt.ID - prefrcpt.ID + 1);
                var subtocptlt = tocptlt.GetRange(pretocpt.ID, currenttocpt.ID - pretocpt.ID + 1);

                frcptltlt.Add(subfrcptlt);
                tocptltlt.Add(subtocptlt);

                prenode3 = currentnode;
            }
        }

        private void AdjustMaxBackK(ref int intMaxBackIorJ, int intMaxBack, int intThisPtNum, int intOtherPtNum)
        {
            intMaxBackIorJ = Math.Max(intMaxBackIorJ, Convert.ToInt32(Math.Ceiling(Convert.ToDouble(intThisPtNum - 1) / Convert.ToDouble(intOtherPtNum - 1))));

            if (intMaxBackIorJ > intMaxBack)
            {
                Console.WriteLine("We need a larger look-back parameter, at least " + intMaxBackIorJ);
            }
        }
    }
}
