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



namespace MorphingClass.CMorphingMethodsLSA
{
    /// <summary>
    /// 基于最小二乘原理的Morphing方法，以角度和边长为参数(Least Squares Alogrithm_Coordinate, Angle and Length)
    /// </summary>
    /// <remarks>顾及长度、角度和多线段间对应顶点距离，以坐标为平差参数进行平差，多结果间接平差</remarks>
    public class CALALMulti
    {
        private CDataRecords _DataRecords;                    //数据记录
        private double _dblTX;
        
        

        public CALALMulti()
        {

        }

        public CALALMulti(CDataRecords pDataRecords, double dblTX)
        {
            _DataRecords = pDataRecords;
            _dblTX = dblTX;
        }

        public CALALMulti(CDataRecords pDataRecords)
        {
            _DataRecords = pDataRecords;
            CPolyline FromCpl = pDataRecords.ParameterResult.FromCpl;
            _dblTX = FromCpl.pPolyline.Length / FromCpl.CptLt .Count  / 1000;   //计算阈值参数
        }


        /// <summary>
        /// 显示并返回单个插值面状要素
        /// </summary>
        /// <param name="intInterNum">Inter: Interpolation</param>
        /// <returns>面状要素</returns>
        public void ALALMultiMorphing()
        {
            List<CPolyline> cpllt = GetTargetcpllt();
            cpllt.Insert(0, _DataRecords.ParameterResult.FromCpl);
            cpllt.Add(_DataRecords.ParameterResult.ToCpl);
            _DataRecords.ParameterResult.CResultPlLt = cpllt;








            //// 清除绘画痕迹
            //IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            //IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            //pGra.DeleteAllElements();
            //CHelperFunction.ViewPolyline(m_mapControl, cpl);  //显示生成的线段
            //return cpl;
        }

        /// <summary>
        /// Get desired polylines
        /// </summary>
        /// <param name="intInterNum">Inter: Interpolation</param>
        /// <returns></returns>
        public List<CPolyline> GetTargetcpllt()
        {
            int intIterationNum = Convert.ToInt32(_DataRecords.ParameterInitialize.txtIterationNum.Text);     //the maximum itrative times
            int intInterNum = Convert.ToInt32(_DataRecords.ParameterInitialize.txtInterpolationNum.Text);     //the number of interpolated polylines desired
            List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;   //corresponding points
            double dblTX = _dblTX;    //the threshold of jumping out the while loop
            double dblInterval = 1 / Convert.ToDouble(intInterNum + 1);


            int intPtNum = pCorrCptsLt.Count;
            int intXYNum = intPtNum * 2;
            int intMultiXYNum = intInterNum * intXYNum;


            //Calculation of the designed lengths in polylines(including fixed ones)
            double[,] adblLength0 = new double[intInterNum, intPtNum - 1];
            double[,] adblLength = new double[intInterNum, intPtNum - 1];   //this array is for the changed lengths during the while loop
            for (int j = 0; j < pCorrCptsLt.Count - 1; j++)
            {
                double dblfrsublength = CGeometricMethods.CalDis(pCorrCptsLt[j + 1].FrCpt, pCorrCptsLt[j].FrCpt);
                double dbltosublength = CGeometricMethods.CalDis(pCorrCptsLt[j + 1].ToCpt, pCorrCptsLt[j].ToCpt);
                for (int i = 0; i < intInterNum; i++)
                {
                    double dblProportion = (i+1) * dblInterval;
                    adblLength0[i, j] = (1 - dblProportion) * dblfrsublength + dblProportion * dbltosublength;
                }
                pCorrCptsLt[j].FrCpt.isCtrl = false;
            }

            //Calculation of the designed angles in polylines(including fixed ones)
            double[,] adblAngle0 = new double[intInterNum, intPtNum - 2];
            double[,] adblAngle = new double[intInterNum, intPtNum - 2];   //this array is for the changed angles during the while loop
            for (int j = 0; j < pCorrCptsLt.Count - 2; j++)
            {
                double dblfrAngle = CGeometricMethods.CalAngle_Counterclockwise(pCorrCptsLt[j].FrCpt, pCorrCptsLt[j + 1].FrCpt, pCorrCptsLt[j + 2].FrCpt);
                double dbltoAngle = CGeometricMethods.CalAngle_Counterclockwise(pCorrCptsLt[j].ToCpt, pCorrCptsLt[j + 1].ToCpt, pCorrCptsLt[j + 2].ToCpt);
                for (int i = 0; i < intInterNum; i++)
                {
                    double dblProportion = (i + 1) * dblInterval;
                    adblAngle0[i, j] = (1 - dblProportion) * dblfrAngle + dblProportion * dbltoAngle;
                }
            }

            //Calculation of the designed lengths between polylines(including fixed ones)
            double[,] adblIntervalDis0 = new double[intInterNum + 1, intPtNum];
            double[,] adblIntervalDis = new double[intInterNum + 1, intPtNum];   //this array is for the changed lengths during the while loop
            for (int j = 0; j < pCorrCptsLt.Count; j++)
            {
                double dblSumDis = pCorrCptsLt[j].FrCpt.DistanceTo(pCorrCptsLt[j].ToCpt);
                double dblDis = dblSumDis / (intInterNum + 1);
                for (int i = 0; i <= intInterNum; i++)
                {
                    adblIntervalDis0[i, j] = dblDis;
                }
            }

            //Calculation of the designed angles between polylines(including fixed ones)
            double[,] adblIntervalAngle0 = new double[intInterNum, intPtNum];
            double[,] adblIntervalAngle = new double[intInterNum, intPtNum];   //this array is for the changed angles during the while loop
            for (int j = 0; j < pCorrCptsLt.Count; j++)
            {
                for (int i = 0; i < intInterNum; i++)
                {
                    adblIntervalAngle0[i, j] = Math .PI;
                }
            }

            //Calculation of the coordinates obtained by linear interpolation(including fixed ones)
            //theses coordinates will be used as estimates
            VBMatrix X0 = new VBMatrix(intMultiXYNum, 1);
            for (int i = 0; i < intInterNum; i++)
            {
                int intPreMultiXYNum = i * intXYNum;
                double dblProportion = (i + 1) * dblInterval;
                for (int j = 0; j < intPtNum; j++)
                {
                    X0[intPreMultiXYNum + 2 * j + 0, 0] = (1 - dblProportion) * pCorrCptsLt[j].FrCpt.X + dblProportion * pCorrCptsLt[j].ToCpt.X;
                    X0[intPreMultiXYNum + 2 * j + 1, 0] = (1 - dblProportion) * pCorrCptsLt[j].FrCpt.Y + dblProportion * pCorrCptsLt[j].ToCpt.Y;
                }
            }

            pCorrCptsLt[0].FrCpt.isCtrl = true;  //fix the first vertex
            pCorrCptsLt[1].FrCpt.isCtrl = true;  //fix the second vertex
            pCorrCptsLt[pCorrCptsLt.Count - 2].FrCpt.isCtrl = true;   //fix the second last vertex
            pCorrCptsLt[pCorrCptsLt.Count - 1].FrCpt.isCtrl = true;   //fix the last vertex


            int intKnownPt = 0;           //the number of fixed vertices
            int intUnknownPt = 0;         //the number of unfixed vertices

            List<int> intKnownLocationLt = new List<int>();  //the indices of fixed verticves
            for (int i = 0; i < intPtNum; i++)
            {
                if (pCorrCptsLt[i].FrCpt.isCtrl == true)
                {
                    intKnownLocationLt.Add(i);
                    intKnownPt += 1;
                }
                else
                {
                    intUnknownPt += 1;
                }
            }
            int intUnknownXY = intUnknownPt * 2;
            int intMultiUnknownXY = intInterNum * intUnknownXY;

            //the number of fixed lengths in polylines
            List<int> intKnownLengthLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 1; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1)
                {
                    intKnownLengthLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownLength = intPtNum - 1 - intKnownLengthLt.Count;

            //the number of fixed angles in polylines
            List<int> intKnownAngleLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 2; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1 && (intKnownLocationLt[i + 2] - intKnownLocationLt[i + 1]) == 1)
                {
                    intKnownAngleLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownAngle = intPtNum - 2 - intKnownAngleLt.Count;            

            //some numbers of unknowns
            int intUnknownLengthAngle = intUnknownLength + intUnknownAngle;
            int intMultiUnknownLength = intInterNum * intUnknownLength;
            int intMultiUnknownAngle = intInterNum * intUnknownAngle;
            int intMultiUnknownLengthAngle = intMultiUnknownLength + intMultiUnknownAngle;
            int intMultiUnknownIntervalLength = (intInterNum + 1) * intUnknownPt;
            int intMultiUnknownLAL = intMultiUnknownLength + intMultiUnknownAngle + intMultiUnknownIntervalLength;
            int intMultiUnknownIntervalAngle = intInterNum * intUnknownPt;
            int intSumConstraints = intMultiUnknownLength + intMultiUnknownAngle + intMultiUnknownIntervalLength + intMultiUnknownIntervalAngle;

            //Matrix of Weights***************************************************************************************Matrix of Weights************************************************************************************************Matrix of Weights//
            VBMatrix P = new VBMatrix(intSumConstraints, intSumConstraints);
            double dblLengthP = 0.0036;
            double dblIntervalLengthP = 1;//***********************************************************************************/
            for (int i = 0; i < intMultiUnknownLength; i++)  //weights of lengths in polylines
            {
                P[i, i] = dblLengthP;
            }
            for (int i = 0; i < intMultiUnknownAngle; i++)   //weights of angles in polylines
            {
                P[intMultiUnknownLength + i, intMultiUnknownLength + i] = 40;
            }
            for (int i = 0; i < intMultiUnknownIntervalLength; i++)   //weights of lengths between polylines
            {
                P[intMultiUnknownLengthAngle + i, intMultiUnknownLengthAngle + i] = 0.0036 * dblIntervalLengthP;
            }
            for (int i = 0; i < intMultiUnknownIntervalAngle; i++)   //weights of angles between polylines
            {
                P[intMultiUnknownLAL + i, intMultiUnknownLAL + i] = 40 * dblIntervalLengthP;
            }


            //Xmix:the newest coordinates (including fixed vertices)
            VBMatrix Xmix = new VBMatrix(intMultiXYNum, 1);
            for (int i = 0; i < intMultiXYNum; i++)
            {
                Xmix[i, 0] = X0[i, 0];
            }


            VBMatrix XA = new VBMatrix(intMultiUnknownXY, 1);    //Adjusted unknowns
            VBMatrix XA0 = new VBMatrix(intMultiUnknownXY, 1);   //the original coordinates of unfixed vertices
            int intSumCount0 = 0;
            for (int j = 0; j < intUnknownPt; j++)
            {
                if (pCorrCptsLt[intSumCount0].FrCpt.isCtrl == false)
                {
                    for (int i = 0; i < intInterNum; i++)
                    {
                        XA[i * intUnknownXY + j * 2 + 0, 0] = X0[i * intXYNum + intSumCount0 * 2 + 0, 0];
                        XA[i * intUnknownXY + j * 2 + 1, 0] = X0[i * intXYNum + intSumCount0 * 2 + 1, 0];

                        XA0[i * intUnknownXY + j * 2 + 0, 0] = XA[i * intUnknownXY + j * 2 + 0, 0];
                        XA0[i * intUnknownXY + j * 2 + 1, 0] = XA[i * intUnknownXY + j * 2 + 1, 0];
                    }
                }
                else
                {
                    j -= 1;
                }
                intSumCount0 += 1;
            }


            VBMatrix A = new VBMatrix(intSumConstraints, intMultiUnknownXY);     //partial derivation matrix (lengths in polylines, angles in polylines, lengths between polylines and angles between polylines)
            VBMatrix matl = new VBMatrix(intSumConstraints, 1);                  //the differences between designed values and approximation values (lengths in polylines, angles in polylines, lengths between polylines and angles between polylines)
            VBMatrix V = new VBMatrix(intSumConstraints, 1);                     //corrections of designed values (lengths in polylines, angles in polylines, lengths between polylines and angles between polylines)
            VBMatrix x = new VBMatrix(intMultiUnknownXY, 1);                     //corrections of coordinates
            x[0, 0] = 100000;
            VBMatrix LPlusV = new VBMatrix(intSumConstraints, 1);                //corrected designed values (lengths in polylines, angles in polylines, lengths between polylines and angles between polylines)
            VBMatrix oldAx = new VBMatrix(intSumConstraints, 1);                 //A*x
            
         
            int intIterativeCount = 0;
            List<CPolyline> cpllt = new List<CPolyline>();
            for (int k = 0; k < 100; k++)
            {
                intIterativeCount = 0;
                intIterationNum++;
                //break;
                do
                {


                    if (intIterativeCount >= intIterationNum)
                    {
                        break;
                    }
                    intIterativeCount += 1;

                     

                    #region Calculation of Derivations

                    //Calculation of lengths of the segments in polylines
                    for (int i = 0; i < intInterNum; i++)
                    {
                        int intBasicIndexS1 = i * intXYNum;
                        for (int j = 0; j < intPtNum - 1; j++)
                        {
                            int intBasicIndexIJS1 = intBasicIndexS1 + 2 * j;
                            adblLength[i, j] = Math.Sqrt((Xmix[intBasicIndexIJS1 + 0, 0] - Xmix[intBasicIndexIJS1 + 2, 0]) * (Xmix[intBasicIndexIJS1 + 0, 0] - Xmix[intBasicIndexIJS1 + 2, 0]) +
                                                         (Xmix[intBasicIndexIJS1 + 1, 0] - Xmix[intBasicIndexIJS1 + 3, 0]) * (Xmix[intBasicIndexIJS1 + 1, 0] - Xmix[intBasicIndexIJS1 + 3, 0]));
                        }
                    }

                    //Calculation of angles between the segments in polylines
                    for (int i = 0; i < intInterNum; i++)
                    {
                        for (int j = 0; j < intPtNum - 2; j++)
                        {
                            int intBasicIndexIJA1 = i * intXYNum + 2 * j;
                            adblAngle[i, j] = CGeometricMethods.CalAngle_Counterclockwise(Xmix[intBasicIndexIJA1 + 0, 0], Xmix[intBasicIndexIJA1 + 1, 0],
                                                                        Xmix[intBasicIndexIJA1 + 2, 0], Xmix[intBasicIndexIJA1 + 3, 0],
                                                                        Xmix[intBasicIndexIJA1 + 4, 0], Xmix[intBasicIndexIJA1 + 5, 0]);
                        }
                    }

                    //Calculation of lengths of the segments between polylines
                    for (int j = 0; j < intPtNum; j++)
                    {
                        int int2J = 2 * j;
                        //the lengths between the larger-scale polyline and the first generated polyline
                        adblIntervalDis[0, j] = Math.Sqrt((pCorrCptsLt[j].FrCpt.X - Xmix[int2J + 0, 0]) * (pCorrCptsLt[j].FrCpt.X - Xmix[int2J + 0, 0]) +
                                                          (pCorrCptsLt[j].FrCpt.Y - Xmix[int2J + 1, 0]) * (pCorrCptsLt[j].FrCpt.Y - Xmix[int2J + 1, 0]));
                        //the lengths between the last generated polyline and the smaller-scale polyline
                        adblIntervalDis[intInterNum, j] = Math.Sqrt((pCorrCptsLt[j].ToCpt.X - Xmix[(intInterNum - 1) * intXYNum + int2J + 0, 0]) * (pCorrCptsLt[j].ToCpt.X - Xmix[(intInterNum - 1) * intXYNum + int2J + 0, 0]) +
                                                                    (pCorrCptsLt[j].ToCpt.Y - Xmix[(intInterNum - 1) * intXYNum + int2J + 1, 0]) * (pCorrCptsLt[j].ToCpt.Y - Xmix[(intInterNum - 1) * intXYNum + int2J + 1, 0]));
                        //the lengths between the generated polylines
                        for (int i = 1; i < intInterNum; i++)
                        {
                            adblIntervalDis[i, j] = Math.Sqrt((Xmix[(i - 1) * intXYNum + int2J + 0, 0] - Xmix[i * intXYNum + int2J + 0, 0]) * (Xmix[(i - 1) * intXYNum + int2J + 0, 0] - Xmix[i * intXYNum + int2J + 0, 0]) +
                                                              (Xmix[(i - 1) * intXYNum + int2J + 1, 0] - Xmix[i * intXYNum + int2J + 1, 0]) * (Xmix[(i - 1) * intXYNum + int2J + 1, 0] - Xmix[i * intXYNum + int2J + 1, 0]));
                        }
                    }

                    //Calculation of the angles between polylines
                    for (int j = 0; j < intPtNum; j++)
                    {
                        int int2J = 2 * j;
                        //the angles between the larger-scale polyline, the first generated polyline and the second generated polyline
                        int l = 0;
                        adblIntervalAngle[l, j] = CGeometricMethods.CalAngle_Counterclockwise(pCorrCptsLt[j].FrCpt.X, pCorrCptsLt[j].FrCpt.Y,
                                                                            Xmix[(l - 0) * intXYNum + int2J + 0, 0], Xmix[(l - 0) * intXYNum + int2J + 1, 0],
                                                                            Xmix[(l + 1) * intXYNum + int2J + 0, 0], Xmix[(l + 1) * intXYNum + int2J + 1, 0]);

                        //the angles between the second last generated polyline,the last generated polyline and the smaller-scale polyline
                        l = intInterNum - 1;
                        adblIntervalAngle[l, j] = CGeometricMethods.CalAngle_Counterclockwise(Xmix[(l - 1) * intXYNum + int2J + 0, 0], Xmix[(l - 1) * intXYNum + int2J + 1, 0],
                                                                            Xmix[(l - 0) * intXYNum + int2J + 0, 0], Xmix[(l - 0) * intXYNum + int2J + 1, 0],
                                                                            pCorrCptsLt[j].ToCpt.X, pCorrCptsLt[j].ToCpt.Y);

                        //the angles between the generated polylines
                        for (int i = 1; i < intInterNum - 1; i++)
                        {
                            adblIntervalAngle[i, j] = CGeometricMethods.CalAngle_Counterclockwise(Xmix[(i - 1) * intXYNum + int2J + 0, 0], Xmix[(i - 1) * intXYNum + int2J + 1, 0],
                                                                                Xmix[(i - 0) * intXYNum + int2J + 0, 0], Xmix[(i - 0) * intXYNum + int2J + 1, 0],
                                                                                Xmix[(i + 1) * intXYNum + int2J + 0, 0], Xmix[(i + 1) * intXYNum + int2J + 1, 0]);
                        }
                    }

                    //Calculation of A: from Row 0 to Row (intMultiUnknownLength - 1), i.e. the partial derivations of lengths in polylines
                    int intKnownCount2 = 0;
                    int intUnKnownCount2 = 0;
                    for (int j = 0; j < intUnknownLength; j++)
                    {
                        int intSumCount = intKnownCount2 + intUnKnownCount2;
                        int intBasicIndexL2 = 2 * intUnKnownCount2;
                        if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == false && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == false)
                        {
                            for (int i = 0; i < intInterNum; i++)
                            {
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 0] = -(Xmix[i * intXYNum + 2 * intSumCount + 2, 0] - Xmix[i * intXYNum + 2 * intSumCount + 0, 0]) / adblLength[i, intSumCount];
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 1] = -(Xmix[i * intXYNum + 2 * intSumCount + 3, 0] - Xmix[i * intXYNum + 2 * intSumCount + 1, 0]) / adblLength[i, intSumCount];
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 2] = -A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 0];
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 3] = -A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 1];

                                matl[i * intUnknownLength + j, 0] = adblLength0[i, intSumCount] - adblLength[i, intSumCount];   //calculation of l                           
                            }
                            intUnKnownCount2 += 1;
                        }
                        else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == false && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == true)
                        {
                            for (int i = 0; i < intInterNum; i++)
                            {
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 0] = -(Xmix[i * intXYNum + 2 * intSumCount + 2, 0] - Xmix[i * intXYNum + 2 * intSumCount + 0, 0]) / adblLength[i, intSumCount];
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 1] = -(Xmix[i * intXYNum + 2 * intSumCount + 3, 0] - Xmix[i * intXYNum + 2 * intSumCount + 1, 0]) / adblLength[i, intSumCount];

                                matl[i * intUnknownLength + j, 0] = adblLength0[i, intSumCount] - adblLength[i, intSumCount];   //calculation of l                           
                            }
                            intUnKnownCount2 += 1;
                        }
                        else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == false)
                        {
                            for (int i = 0; i < intInterNum; i++)
                            {
                                //Notice: "pCorrCptsLt[intSumCount].FrCpt.isCtrl == true", there is no position for vertex "intSumCount". 
                                //Therefore,the indices of the columns of Matrix A are +0 and +1, rather than +2 and +3.
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 0] = (Xmix[i * intXYNum + 2 * intSumCount + 2, 0] - Xmix[i * intXYNum + 2 * intSumCount + 0, 0]) / adblLength[i, intSumCount];
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 1] = (Xmix[i * intXYNum + 2 * intSumCount + 3, 0] - Xmix[i * intXYNum + 2 * intSumCount + 1, 0]) / adblLength[i, intSumCount];

                                matl[i * intUnknownLength + j, 0] = adblLength0[i, intSumCount] - adblLength[i, intSumCount];   //calculation of l                            
                            }
                            intKnownCount2 += 1;
                        }
                        else
                        {
                            intKnownCount2 += 1;
                            j -= 1;
                        }
                    }

                    //Calculation of A: from Row intMultiUnknownLength to Row (intMultiUnknownLengthAngle - 1), i.e. the partial derivations of angles in polylines
                    int intKnownCount3 = 0;
                    int intUnKnownCount3 = 0;
                    for (int j = 0; j < intUnknownAngle; j++)
                    {
                        //fortunately, althought the calculations of angles are dependent on different situations, the calculations of partial derivations of these angles are the same
                        int intSumCount = intKnownCount3 + intUnKnownCount3;

                        //preparation of some constants
                        double[] adblA2 = new double[intInterNum];
                        double[] adblB2 = new double[intInterNum];
                        for (int i = 0; i < intInterNum; i++)
                        {
                            adblA2[i] = adblLength[i, intSumCount + 0] * adblLength[i, intSumCount + 0];
                            adblB2[i] = adblLength[i, intSumCount + 1] * adblLength[i, intSumCount + 1];
                        }
                        
                        if (pCorrCptsLt[intUnKnownCount3 + intKnownCount3].FrCpt.isCtrl == true && pCorrCptsLt[intUnKnownCount3 + intKnownCount3 + 1].FrCpt.isCtrl == true && pCorrCptsLt[intUnKnownCount3 + intKnownCount3 + 2].FrCpt.isCtrl == true)
                        {
                            intKnownCount3 += 1;
                            j -= 1;
                        }
                        else
                        {
                            for (int i = 0; i < intInterNum; i++)
                            {
                                matl[intMultiUnknownLength + i * intUnknownAngle + j, 0] = adblAngle0[i, intSumCount] - adblAngle[i, intSumCount];   //calculation of l
                            }

                            int intPreTrueNum = 0;
                            int intUnKnownCount3orginal = intUnKnownCount3;
                            int intKnownCount3orginal = intKnownCount3;

                            //partial derivations of first vertex
                            if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 0].FrCpt.isCtrl == false)
                            {                                
                                for (int i = 0; i < intInterNum; i++)
                                {
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * intUnKnownCount3orginal + 0] = -(Xmix[i * intXYNum + 2 * intSumCount + 3, 0] - Xmix[i * intXYNum + 2 * intSumCount + 1, 0]) / adblA2[i];
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * intUnKnownCount3orginal + 1] = +(Xmix[i * intXYNum + 2 * intSumCount + 2, 0] - Xmix[i * intXYNum + 2 * intSumCount + 0, 0]) / adblA2[i];
                                }

                                intUnKnownCount3 += 1;
                            }
                            else
                            {
                                intPreTrueNum += 1;
                                intKnownCount3 += 1;
                            }

                            //partial derivations of second vertex
                            if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 1].FrCpt.isCtrl == false)
                            {
                                for (int i = 0; i < intInterNum; i++)
                                {
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * (intUnKnownCount3orginal - intPreTrueNum) + 2] = (Xmix[i * intXYNum + 2 * intSumCount + 5, 0] - Xmix[i * intXYNum + 2 * intSumCount + 3, 0]) / adblB2[i]
                                                                                                                                                             + (Xmix[i * intXYNum + 2 * intSumCount + 3, 0] - Xmix[i * intXYNum + 2 * intSumCount + 1, 0]) / adblA2[i];
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * (intUnKnownCount3orginal - intPreTrueNum) + 3] = -(Xmix[i * intXYNum + 2 * intSumCount + 4, 0] - Xmix[i * intXYNum + 2 * intSumCount + 2, 0]) / adblB2[i]
                                                                                                                                                              - (Xmix[i * intXYNum + 2 * intSumCount + 2, 0] - Xmix[i * intXYNum + 2 * intSumCount + 0, 0]) / adblA2[i];
                                }
                            }
                            else
                            {
                                intPreTrueNum += 1;
                            }

                            //partial derivations of third vertex
                            if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 2].FrCpt.isCtrl == false)
                            {
                                for (int i = 0; i < intInterNum; i++)
                                {
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * (intUnKnownCount3orginal - intPreTrueNum) + 4] = -(Xmix[i * intXYNum + 2 * intSumCount + 5, 0] - Xmix[i * intXYNum + 2 * intSumCount + 3, 0]) / adblB2[i];
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * (intUnKnownCount3orginal - intPreTrueNum) + 5] = +(Xmix[i * intXYNum + 2 * intSumCount + 4, 0] - Xmix[i * intXYNum + 2 * intSumCount + 2, 0]) / adblB2[i];
                                }
                            }
                        }
                    }
                     
                    //Calculation of A: from Row intMultiUnknownLengthAngle to Row (intMultiUnknownLAL - 1), i.e. the partial derivations of lengths between polylines
                    int intKnownCount4 = 0;
                    int intUnKnownCount4 = 0;
                    for (int j = 0; j < intUnknownPt; j++)
                    {
                        int intSumCount4 = intKnownCount4 + intUnKnownCount4;
                        int intBasicIndexL4 = 2 * intUnKnownCount4;
                        if (pCorrCptsLt[intSumCount4].FrCpt.isCtrl == false)
                        {
                            int l = 0;

                            //the partial derivations of lengths between the larger-scale polyline and the first generated polyline
                            A[intMultiUnknownLengthAngle + l * intUnknownPt + j, (l - 0) * intUnknownXY + intBasicIndexL4 + 0] = (Xmix[l * intXYNum + 2 * intSumCount4 + 0, 0] - pCorrCptsLt[intSumCount4].FrCpt.X) / adblIntervalDis[l, intSumCount4];
                            A[intMultiUnknownLengthAngle + l * intUnknownPt + j, (l - 0) * intUnknownXY + intBasicIndexL4 + 1] = (Xmix[l * intXYNum + 2 * intSumCount4 + 1, 0] - pCorrCptsLt[intSumCount4].FrCpt.Y) / adblIntervalDis[l, intSumCount4];

                            matl[intMultiUnknownLengthAngle + l * intUnknownPt + j, 0] = adblIntervalDis0[l, intSumCount4] - adblIntervalDis[l, intSumCount4];      //calculation of l

                            //the partial derivations of lengths between the last generated polyline and the smaller-scale polyline 
                            l = intInterNum;
                            A[intMultiUnknownLengthAngle + l * intUnknownPt + j, (l - 1) * intUnknownXY + intBasicIndexL4 + 0] = -(pCorrCptsLt[intSumCount4].ToCpt.X - Xmix[(l - 1) * intXYNum + 2 * intSumCount4 + 0, 0]) / adblIntervalDis[l, intSumCount4];
                            A[intMultiUnknownLengthAngle + l * intUnknownPt + j, (l - 1) * intUnknownXY + intBasicIndexL4 + 1] = -(pCorrCptsLt[intSumCount4].ToCpt.Y - Xmix[(l - 1) * intXYNum + 2 * intSumCount4 + 1, 0]) / adblIntervalDis[l, intSumCount4];

                            matl[intMultiUnknownLengthAngle + l * intUnknownPt + j, 0] = adblIntervalDis0[l, intSumCount4] - adblIntervalDis[l, intSumCount4];      //calculation of l

                            //the partial derivations of lengths between the last generated polylines
                            for (int i = 1; i < intInterNum; i++)
                            {
                                A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 1) * intUnknownXY + intBasicIndexL4 + 0] = -(Xmix[i * intXYNum + 2 * intSumCount4 + 0, 0] - Xmix[(i - 1) * intXYNum + 2 * intSumCount4 + 0, 0]) / adblIntervalDis[i, intSumCount4];
                                A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 1) * intUnknownXY + intBasicIndexL4 + 1] = -(Xmix[i * intXYNum + 2 * intSumCount4 + 1, 0] - Xmix[(i - 1) * intXYNum + 2 * intSumCount4 + 1, 0]) / adblIntervalDis[i, intSumCount4];
                                A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 0) * intUnknownXY + intBasicIndexL4 + 0] = -A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 1) * intUnknownXY + +intBasicIndexL4 + 0];
                                A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 0) * intUnknownXY + intBasicIndexL4 + 1] = -A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 1) * intUnknownXY + +intBasicIndexL4 + 1];

                                matl[intMultiUnknownLengthAngle + i * intUnknownPt + j, 0] = adblIntervalDis0[i, intSumCount4] - adblIntervalDis[i, intSumCount4];      //calculation of l
                            }

                            intUnKnownCount4 += 1;
                        }
                        else
                        {
                            intKnownCount4 += 1;
                            j -= 1;
                        }
                    }

                    //Calculation of A: from Row intMultiUnknownLAL to Row (intSumConstraints - 1), i.e. the partial derivations of angles between polylines
                    int intKnownCount5 = 0;
                    int intUnKnownCount5 = 0;
                    for (int j = 0; j < intUnknownPt; j++)
                    {
                        int intSumCount5 = intKnownCount5 + intUnKnownCount5;
                        int intBasicIndexL5 = 2 * intUnKnownCount5;
                        if (pCorrCptsLt[intSumCount5].FrCpt.isCtrl == false)
                        {
                            //calculation of l
                            for (int i = 0; i < intInterNum; i++)
                            {
                                matl[intMultiUnknownLAL + i * intUnknownPt + j, 0] = adblIntervalAngle0[i, intSumCount5] - adblIntervalAngle[i, intSumCount5];      //calculation of l
                            }


                            //the partial derivations of angles between the larger-scale polyline, the first generated polyline and the second generated polyline
                            int l = 0;
                            //the derivations of the vertex in the first generated polyline
                            A[intMultiUnknownLAL + l * intUnknownPt + j, (l - 0) * intUnknownXY + intBasicIndexL5 + 0] = +CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    Xmix[(l + 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l + 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[l + 1, intSumCount5], "x1")
                                                                                                                        - CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    pCorrCptsLt[intSumCount5].FrCpt.X,
                                                                                                                                                    pCorrCptsLt[intSumCount5].FrCpt.Y, adblIntervalDis[l + 0, intSumCount5], "x1");

                            A[intMultiUnknownLAL + l * intUnknownPt + j, (l - 0) * intUnknownXY + intBasicIndexL5 + 1] = +CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    Xmix[(l + 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l + 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[l + 1, intSumCount5], "y1")
                                                                                                                        - CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    pCorrCptsLt[intSumCount5].FrCpt.X,
                                                                                                                                                    pCorrCptsLt[intSumCount5].FrCpt.Y, adblIntervalDis[l + 0, intSumCount5], "y1");
                            //the derivations of the vertex in the second generated polyline
                            A[intMultiUnknownLAL + l * intUnknownPt + j, (l + 1) * intUnknownXY + intBasicIndexL5 + 0] = +CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    Xmix[(l + 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l + 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[l + 1, intSumCount5], "x2");

                            A[intMultiUnknownLAL + l * intUnknownPt + j, (l + 1) * intUnknownXY + intBasicIndexL5 + 1] = +CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    Xmix[(l + 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l + 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[l + 1, intSumCount5], "y2");



                            //the partial derivations of angles between the second last generated polyline,the last generated polyline and the smaller-scale polyline
                            l = intInterNum - 1;
                            //the derivations of the vertex in the second last generated polyline
                            A[intMultiUnknownLAL + l * intUnknownPt + j, (l - 1) * intUnknownXY + intBasicIndexL5 + 0] = -CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    Xmix[(l - 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[l + 0, intSumCount5], "x2");

                            A[intMultiUnknownLAL + l * intUnknownPt + j, (l - 1) * intUnknownXY + intBasicIndexL5 + 1] = -CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    Xmix[(l - 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[l + 0, intSumCount5], "y2");

                            //the derivations of the vertex in the last generated polyline
                            A[intMultiUnknownLAL + l * intUnknownPt + j, (l - 0) * intUnknownXY + intBasicIndexL5 + 0] = +CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    pCorrCptsLt[intSumCount5].ToCpt.X,
                                                                                                                                                    pCorrCptsLt[intSumCount5].ToCpt.Y, adblIntervalDis[l + 1, intSumCount5], "x1")
                                                                                                                        - CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    Xmix[(l - 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[l + 0, intSumCount5], "x1");

                            A[intMultiUnknownLAL + l * intUnknownPt + j, (l - 0) * intUnknownXY + intBasicIndexL5 + 1] = +CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    pCorrCptsLt[intSumCount5].ToCpt.X,
                                                                                                                                                    pCorrCptsLt[intSumCount5].ToCpt.Y, adblIntervalDis[l + 1, intSumCount5], "y1")
                                                                                                                        - CGeometricMethods.DerArctan(Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                    Xmix[(l - 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                    Xmix[(l - 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[l + 0, intSumCount5], "y1");

                            //the partial derivations of angles between generated polylines
                            for (int i = 1; i < intInterNum - 1; i++)
                            {
                                //the first vertex
                                A[intMultiUnknownLAL + i * intUnknownPt + j, (i - 1) * intUnknownXY + intBasicIndexL5 + 0] = -CGeometricMethods.DerArctan(Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                        Xmix[(i - 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[i + 0, intSumCount5], "x2");

                                A[intMultiUnknownLAL + i * intUnknownPt + j, (i - 1) * intUnknownXY + intBasicIndexL5 + 1] = -CGeometricMethods.DerArctan(Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                        Xmix[(i - 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[i + 0, intSumCount5], "y2");

                                //the second vertex
                                A[intMultiUnknownLAL + i * intUnknownPt + j, (i - 0) * intUnknownXY + intBasicIndexL5 + 0] = +CGeometricMethods.DerArctan(Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                        Xmix[(i + 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i + 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[i + 1, intSumCount5], "x1")
                                                                                                                            - CGeometricMethods.DerArctan(Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                        Xmix[(i - 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[i + 0, intSumCount5], "x1");

                                A[intMultiUnknownLAL + i * intUnknownPt + j, (i - 0) * intUnknownXY + intBasicIndexL5 + 1] = +CGeometricMethods.DerArctan(Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                        Xmix[(i + 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i + 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[i + 1, intSumCount5], "y1")
                                                                                                                            - CGeometricMethods.DerArctan(Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                        Xmix[(i - 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[i + 0, intSumCount5], "y1");

                                //the third vertex
                                A[intMultiUnknownLAL + i * intUnknownPt + j, (i + 1) * intUnknownXY + intBasicIndexL5 + 0] = +CGeometricMethods.DerArctan(Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                        Xmix[(i + 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i + 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[i + 1, intSumCount5], "x2");
                                A[intMultiUnknownLAL + i * intUnknownPt + j, (i + 1) * intUnknownXY + intBasicIndexL5 + 1] = +CGeometricMethods.DerArctan(Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i - 0) * intXYNum + 2 * intSumCount5 + 1, 0],
                                                                                                                                                        Xmix[(i + 1) * intXYNum + 2 * intSumCount5 + 0, 0],
                                                                                                                                                        Xmix[(i + 1) * intXYNum + 2 * intSumCount5 + 1, 0], adblIntervalDis[i + 1, intSumCount5], "y2");
                            }

                            intUnKnownCount5 += 1;
                        }
                        else
                        {
                            intKnownCount5 += 1;
                            j -= 1;
                        }
                    }
                    #endregion                    
                   

                    int kk = intIterativeCount;
                    double dblLength0 = CGeometricMethods.CalLengthofVector(x);
                    if (dblLength0<= dblTX )
                    {
                        break;
                    }


                    int tt = 5;

                    //Save matrices as excel documents
                    //CHelperFunctionExcel.ExportDataToExcel2(A, "matA", _DataRecords.ParameterInitialize.strSavePath);
                    //CHelperFunctionExcel.ExportDataToExcelP(P, "matP", _DataRecords.ParameterInitialize.strSavePath);
                    //CHelperFunctionExcel.ExportDataToExcel2(matl, "matmatl", _DataRecords.ParameterInitialize.strSavePath);

                    //VBMatrix AxMinusl = oldAx - matl;

                    VBMatrix Temp = A.Trans() * P * A;
                    VBMatrix InvTemp = Temp.Inv(Temp);
                    x = InvTemp * A.Trans() * P * matl;

                    XA += x;                    

                    //CHelperFunctionExcel.ExportDataToExcel2(XA, "matXA", _DataRecords.ParameterInitialize.strSavePath);

                    V = A * x - matl;
                    oldAx = A * x;
                    //VtPV
                    double dblVtPV = (V.Trans() * P * V).MatData[0, 0];

                    _DataRecords.ParameterInitialize.txtVtPV.Text = "   VtPV = " + dblVtPV.ToString();


                    VBMatrix L = new VBMatrix(intSumConstraints, 1);
                    VBMatrix LK = new VBMatrix(intSumConstraints, 1);
                    

                    int intKnownCount7 = 0;
                    int intUnKnownCount7 = 0;
                    int intSumCount7=0;
                    for (int i = 0; i < intInterNum; i++)
                    {
                        intSumCount7 = 0;
                        for (int j = 0; j < intUnknownLength; j++)
                        {
                            if (pCorrCptsLt[intSumCount7].FrCpt.isCtrl == false  || pCorrCptsLt[intSumCount7+1].FrCpt.isCtrl == false )
                            {
                                L[i * intUnknownLength + j, 0] = adblLength0[i, intSumCount7];
                                LK[i * intUnknownLength + j, 0] = adblLength[i, intSumCount7]; 
                            }
                            else
                            {
                                j--;
                            }
                            intSumCount7++;
                        }
                    }

                    intSumCount7 = 0;
                    for (int i = 0; i < intInterNum; i++)
                    {
                        intSumCount7 = 0;
                        for (int j = 0; j < intUnknownAngle; j++)
                        {
                            if (pCorrCptsLt[intSumCount7].FrCpt.isCtrl == false || pCorrCptsLt[intSumCount7 + 1].FrCpt.isCtrl == false || pCorrCptsLt[intSumCount7 + 2].FrCpt.isCtrl == false)
                            {
                                L[intMultiUnknownLength + i * intUnknownAngle + j, 0] = adblAngle0[i, intSumCount7];
                                LK[intMultiUnknownLength + i * intUnknownAngle + j, 0] = adblAngle[i, intSumCount7];
                            }
                            else
                            {
                                j--;
                            }
                            intSumCount7++;
                        }
                    }

                    intSumCount7 = 0;
                    for (int i = 0; i <= intInterNum; i++)
                    {
                        intSumCount7 = 0;
                        for (int j = 0; j < intUnknownPt; j++)
                        {
                            if (pCorrCptsLt[intSumCount7].FrCpt.isCtrl == false )
                            {
                                L[intMultiUnknownLengthAngle + i * intUnknownPt + j, 0] = adblIntervalDis0[i, intSumCount7];
                                LK[intMultiUnknownLengthAngle + i * intUnknownPt + j, 0] = adblIntervalDis[i, intSumCount7];
                            }
                            else
                            {
                                j--;
                            }
                            intSumCount7++;
                        }                        
                    }

                    intSumCount7 = 0;
                    for (int i = 0; i < intInterNum; i++)
                    {
                        intSumCount7 = 0;
                        for (int j = 0; j < intUnknownPt; j++)
                        {
                            if (pCorrCptsLt[intSumCount7].FrCpt.isCtrl == false)
                            {
                                L[intMultiUnknownLAL + i * intUnknownPt + j, 0] = adblIntervalAngle0[i, intSumCount7];
                                LK[intMultiUnknownLAL + i * intUnknownPt + j, 0] = adblIntervalAngle[i, intSumCount7];
                            }
                            else
                            {
                                j--;
                            }
                            intSumCount7++;
                        }                        
                    }
                    VBMatrix LPlusVMinusLK = LPlusV - LK;
                   
                    LPlusV = L + V;                    
                    VBMatrix AxPlusLKMinusLMinusV = A * x + LK - LPlusV;
                    
                    //VBMatrix AX = A * XA;

                    //CHelperFunctionExcel.ExportDataToExcel2(LPlusV, "matLPlusV", _DataRecords.ParameterInitialize.strSavePath);
                    //CHelperFunctionExcel.ExportDataToExcel2(AX, "matAX", _DataRecords.ParameterInitialize.strSavePath);



                    //Improve Xmix
                    int intSumCount6 = 0;
                    for (int j = 0; j < intUnknownPt; j++)
                    {
                        if (pCorrCptsLt[intSumCount6].FrCpt.isCtrl == false)
                        {
                            for (int i = 0; i < intInterNum; i++)
                            {
                                Xmix[i * intXYNum + intSumCount6 * 2 + 0, 0] = XA[i * intUnknownXY + j * 2 + 0, 0];
                                Xmix[i * intXYNum + intSumCount6 * 2 + 1, 0] = XA[i * intUnknownXY + j * 2 + 1, 0];
                            }
                        }
                        else
                        {
                            j -= 1;
                        }
                        intSumCount6 += 1;
                    }

                    
                    int ii = intIterativeCount;

                    double dblLength = CGeometricMethods.CalLengthofVector(x);
                //} while (dblLength > dblTX);
                } while (true);
                //break;




                VBMatrix VLength = V.GetSubMatrix(0, intMultiUnknownLength, 0, 1);
                VBMatrix VAngle = V.GetSubMatrix(intMultiUnknownLength, intMultiUnknownAngle, 0, 1);
                VBMatrix VLengthInterval = V.GetSubMatrix(intMultiUnknownLengthAngle, intMultiUnknownIntervalLength, 0, 1);
                VBMatrix VAngleInterval = V.GetSubMatrix(intMultiUnknownLAL, intMultiUnknownIntervalAngle, 0, 1);

                VBMatrix PLength = P.GetSubMatrix(0, intMultiUnknownLength, 0, intMultiUnknownLength);
                VBMatrix PAngle = P.GetSubMatrix(intMultiUnknownLength, intMultiUnknownAngle, intMultiUnknownLength, intMultiUnknownAngle);
                VBMatrix PLengthInterval = P.GetSubMatrix(intMultiUnknownLengthAngle, intMultiUnknownIntervalLength, intMultiUnknownLengthAngle, intMultiUnknownIntervalLength);
                VBMatrix PAngleInterval = P.GetSubMatrix(intMultiUnknownLAL, intMultiUnknownIntervalAngle, intMultiUnknownLAL, intMultiUnknownIntervalAngle);

                VBMatrix VtPVLength = VLength.Trans() * PLength * VLength;
                VBMatrix VtPVAngle = VAngle.Trans() * PAngle * VAngle;
                VBMatrix VtPVLengthInterval = VLengthInterval.Trans() * PLengthInterval * VLengthInterval;
                VBMatrix VtPVAngleInterval = VAngleInterval.Trans() * PAngleInterval * VAngleInterval;

                double dblVtPVLength = VtPVLength[0, 0];
                double dblVtPVAngle = VtPVAngle[0, 0];
                double dblVtPVLengthInterval = VtPVLengthInterval[0, 0];
                double dblVtPVAngleInterval = VtPVAngleInterval[0, 0];

                double pdblVtPV = (V.Trans() * P * V)[0, 0];

                //生成目标线段                
                for (int i = 0; i < intInterNum; i++)
                {
                    List<CPoint> cptlt = new List<CPoint>();
                    for (int j = 0; j < intPtNum; j++)
                    {
                        CPoint cpt = new CPoint(j, Xmix[i * intXYNum + j * 2, 0], Xmix[i * intXYNum + j * 2 + 1, 0]);
                        cptlt.Add(cpt);
                    }
                    CPolyline cpl = new CPolyline(i, cptlt);
                    cpllt.Add(cpl);
                }



                //for (int i = 0; i < intMultiUnknownLength; i++)  //长度权重
                //{
                //    P[i, i] = 1;
                //}
                //for (int i = 0; i < intMultiUnknownAngle; i++)   //角度权重
                //{
                //    P[intMultiUnknownLength + i, intMultiUnknownLength + i] = 39.48;
                //}
                //for (int i = 0; i < intMultiUnknownInterval; i++)   //角度权重
                //{
                //    P[intMultiUnknownLengthAngle + i, intMultiUnknownLengthAngle + i] = 1;
                //}
            }



           

            return cpllt;
        }


    }
}
