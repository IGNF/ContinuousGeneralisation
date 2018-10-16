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
    /// 基于最小二乘原理的Morphing方法，以“角度、边长、与上一个结果顶点之间的距离（三类）”为观测值(Least Squares Alogrithm_Coordinate, Angle and Length)
    /// </summary>
    /// <remarks>顾及长度和角度和与与上一个结果顶点之间的距离，以坐标为平差参数进行平差，标准间接平差</remarks>
    public class CApLL
    {
        private CDataRecords _DataRecords;                    //records of data
        private double _dblTX;
        
        
        private CPAL _pCAL = new CPAL();

        private double _dblMaxLengthV;
        private double _dblMinLengthV;
        private double _dblDiffLengthV;
        private double _dblMaxAngleV;
        private double _dblMinAngleV;
        private double _dblDiffAngleV;


        //private double _dblMaxELength;   //the max value of Edges' Lengths

        public CApLL()
        {

        }

        public CApLL(CDataRecords pDataRecords, double dblTX)
        {
            _DataRecords = pDataRecords;
            _dblTX = dblTX;
            //CalParameters();
            //_dblMaxELength = pDataRecords.ParameterResult.FromCpl.GetMaxELength() / 100; 
        }

        public CApLL(CDataRecords pDataRecords)
        {
            _DataRecords = pDataRecords;
            CPolyline FromCpl = pDataRecords.ParameterResult.FromCpl;
            _dblTX = FromCpl.pPolyline.Length / FromCpl.CptLt .Count  / 10000;   //计算阈值参数
            //CalParameters();
            //_dblMaxELength = pDataRecords.ParameterResult.FromCpl.GetMaxELength()/100;
        }

        private void CalParameters()
        {
            _DataRecords.ParameterInitialize.tsslMessage.Text = "CALCCulating Parameters......";
            _DataRecords.ParameterInitialize.ststMain.Refresh();

            CPolyline cpl = GetTargetcpl(0.005, _DataRecords.ParameterResult.FromCpl);

            double dblMaxLengthV = 0;
            double dblMinLengthV = cpl.pPolyline.Length;
            double dblMaxAngleV = 0;
            double dblMinAngleV = Math.PI;
            for (int j = 2; j < 200; j++)
            {
                cpl=GetTargetcpl(j*0.005,cpl);

                for (int i = 0; i < cpl.SubCPlLt.Count; i++)
                {
                    if (cpl.SubCPlLt[i].dblLengthV == 0)  //如果是0，肯定是固定边引起的，不参与计算
                    {
                        continue;
                    }

                    if (Math.Abs(cpl.SubCPlLt[i].dblLengthV) > dblMaxLengthV)
                    {
                        dblMaxLengthV = Math.Abs(cpl.SubCPlLt[i].dblLengthV);
                    }
                    if (Math.Abs(cpl.SubCPlLt[i].dblLengthV) < dblMinLengthV)
                    {
                        dblMinLengthV = Math.Abs(cpl.SubCPlLt[i].dblLengthV);
                    }
                }

                for (int i = 1; i < cpl.CptLt.Count - 1; i++)
                {
                    if (cpl.CptLt[i].dblAngleV == 0)  //如果是0，肯定是固定角引起的，不参与计算
                    {
                        continue;
                    }

                    if (Math.Abs(cpl.CptLt[i].dblAngleV) > dblMaxAngleV)
                    {
                        dblMaxAngleV = Math.Abs(cpl.CptLt[i].dblAngleV);
                    }
                    if (Math.Abs(cpl.CptLt[i].dblAngleV) < dblMinAngleV)
                    {
                        dblMinAngleV = Math.Abs(cpl.CptLt[i].dblAngleV);
                    }
                }
            }

            //double dblLengthInterval=(dblMaxLengthV-dblMinLengthV)/
            _dblMaxLengthV = dblMaxLengthV;
            _dblMinLengthV = dblMinLengthV;
            _dblDiffLengthV = dblMaxLengthV - dblMinLengthV;

            _dblMaxAngleV = dblMaxAngleV;
            _dblMinAngleV = dblMinAngleV;
            _dblDiffAngleV = dblMaxAngleV - dblMinAngleV;

            _DataRecords.ParameterInitialize.tsslMessage.Text = "Ready!";
            _DataRecords.ParameterInitialize.ststMain.Refresh();
           
        }

        /// <summary>
        /// 显示并返回单个插值面状要素
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>面状要素</returns>
        public CPolyline DisplayInterpolation(double dblProp, CPolyline lastcpl)
        {


            CPolyline cpl = GetTargetcpl(dblProp, lastcpl);

            //_DataRecords.ParameterInitialize.txtT.Text = "   t = " + dblProp.ToString();
            //_DataRecords.ParameterInitialize.txtVtPV.Text ="   VtPV = " + cpl.dblVtPV.ToString();

            //// 清除绘画痕迹
            //IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            //IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            //pGra.DeleteApLLElements();

            //if (dblProp == 0)
            //{
            //    int tt = 5;
            //}
            //else if (dblProp == 1)
            //{
            //    int ss = 5;
            //}


            ////对线段进行染色
            //for (int i = 0; i < cpl.SubCPlLt.Count; i++)
            //{
            //    double dblColor = 255;
            //    int intColor = Convert.ToInt32((Math.Abs(cpl.SubCPlLt[i].dblLengthV) - _dblMinLengthV) / _dblDiffLengthV * dblColor);
            //    intColor = Math.Abs(intColor);
            //    if (Math.Abs(cpl.SubCPlLt[i].dblLengthV) < _dblTX)
            //    {
            //        cpl.SubCPlLt[i].intGreen = 255;
            //    }

            //    else if (cpl.SubCPlLt[i].dblLengthV > 0)
            //    {
            //        if (intColor <= 255)
            //        {
            //            cpl.SubCPlLt[i].intRed = 255;
            //            cpl.SubCPlLt[i].intGreen = 255 - intColor;
            //            cpl.SubCPlLt[i].intBlue = 255 - intColor;
            //        }
            //        else
            //        {
            //            cpl.SubCPlLt[i].intRed = 255;
            //        }
            //    }
            //    else if (cpl.SubCPlLt[i].dblLengthV < 0)
            //    {
            //        if (intColor <= 255)
            //        {
            //            cpl.SubCPlLt[i].intRed = 255 - intColor;
            //            cpl.SubCPlLt[i].intGreen = 255 - intColor;
            //            cpl.SubCPlLt[i].intBlue = 255;
            //        }
            //        else
            //        {
            //            cpl.SubCPlLt[i].intBlue = 255;
            //        }
            //    }
            //}  

            //for (int i = 0; i < cpl.SubCPlLt.Count; i++)
            //{
            //    CPolyline subcpl = cpl.SubCPlLt[i];
            //    CDrawInActiveView.ViewPolylineByRGB(m_mapControl, subcpl, subcpl.intRed, subcpl.intGreen, subcpl.intBlue);  //显示生成的线段
            //}

            ////对缓冲区进行染色
            //for (int i = 1; i < cpl.CptLt.Count -1; i++)
            //{
            //    double dblColor = 255;
            //    int intColor = Convert.ToInt32((Math.Abs(cpl.CptLt [i].dblAngleV ) - _dblMinAngleV) / _dblDiffAngleV * dblColor);
            //    intColor = Math.Abs(intColor);


            //    if (Math .Abs (cpl.CptLt[i].dblAngleV)<_dblTX)
            //    {
            //        cpl.CptLt[i].intGreen = 255;
            //    }
            //    else if (cpl.CptLt[i].dblAngleV > 0)
            //    {
            //        if (intColor <= 255)
            //        {
            //            cpl.CptLt[i].intRed = 255;
            //            cpl.CptLt[i].intGreen = 255 - intColor;
            //            cpl.CptLt[i].intBlue = 255 - intColor;
            //        }
            //        else
            //        {
            //            cpl.CptLt[i].intRed = 255;
            //        }
            //    }
            //    else if (cpl.CptLt[i].dblAngleV < 0)
            //    {
            //        if (intColor <= 255)
            //        {
            //            cpl.CptLt[i].intRed = 255 - intColor;
            //            cpl.CptLt[i].intGreen = 255 - intColor;
            //            cpl.CptLt[i].intBlue = 255;
            //        }
            //        else
            //        {
            //            cpl.CptLt[i].intBlue = 255;
            //        }
            //    }
            //}

            //for (int i = 1; i < cpl.CptLt.Count-1; i++)
            //{
            //    CPoint cpt=cpl.CptLt[i];
            //    CHelpFunc.ViewPolygonGeometryByRGB(m_mapControl, cpt.Buffer(5), cpt.intRed, cpt.intGreen, cpt.intBlue);  //显示缓冲区多边形
            //}

            ////显示线段
            //IActiveView pAv = pGra as IActiveView;
            //pAv.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);



            // 清除绘画痕迹
            IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            CDrawInActiveView.ViewPolyline(m_mapControl, cpl);  //显示生成的线段


            return cpl;
        }

        /// <summary>
        /// 获取线状要素
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">插值参数</param>
        /// <returns>在处理面状要素时，本程序将原面状要素的边界切开，按线状要素处理，处理完后再重新生成面状要素</returns>
        public CPolyline GetTargetcpl(double dblProp, CPolyline lastcpl)
        {

            if (dblProp == 0)
            {
                int aa = 5;
            }

            List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;   //读取数据后，此处ResultPtLt中的对应点为一一对应
            double dblTX = _dblTX;

            int intPtNum = pCorrCptsLt.Count;
            int intXYNum = 2 * intPtNum;

            //计算长度初始值（全部计算）
            double[] adblLength0 = new double[intPtNum - 1];
            double[] adblFrLength0 = new double[intPtNum - 1];
            double[] adblToLength0 = new double[intPtNum - 1];
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                double dblfrsublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].FrCpt, pCorrCptsLt[i].FrCpt);
                adblFrLength0[i] = dblfrsublength;
                double dbltosublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].ToCpt, pCorrCptsLt[i].ToCpt);
                adblToLength0[i] = dbltosublength;
                adblLength0[i] = (1 - dblProp) * dblfrsublength + dblProp * dbltosublength;
            }

            //计算角度初始值（全部计算）
            double[] adblAngle0 = new double[intPtNum - 2];
            for (int i = 0; i < pCorrCptsLt.Count - 2; i++)
            {
                //较大比例尺线状要素上的夹角
                double dblfrAngle = CGeoFunc.CalAngle_Counterclockwise(pCorrCptsLt[i].FrCpt, pCorrCptsLt[i + 1].FrCpt, pCorrCptsLt[i + 2].FrCpt);
                //较小比例尺线状要素上的夹角
                double dbltoAngle = CGeoFunc.CalAngle_Counterclockwise(pCorrCptsLt[i].ToCpt, pCorrCptsLt[i + 1].ToCpt, pCorrCptsLt[i + 2].ToCpt);

                //角度初始值
                adblAngle0[i] = (1 - dblProp) * dblfrAngle + dblProp * dbltoAngle;
            }

            //计算坐标初始值，以及各线段方位角初始值
            //注意：默认固定第一条边
            pCorrCptsLt[0].FrCpt.isCtrl = true;
            pCorrCptsLt[1].FrCpt.isCtrl = true;
            //固定最后两条边
            pCorrCptsLt[intPtNum - 1].FrCpt.isCtrl = true;
            pCorrCptsLt[intPtNum - 2].FrCpt.isCtrl = true;

            VBMatrix X0 = new VBMatrix(intXYNum, 1);

            //以上一次结果的值作为新的估算值
            List<CPoint> lastcptlt = lastcpl.CptLt;
            for (int i = 0; i < intPtNum; i++)
            {
                if (pCorrCptsLt[i].FrCpt.isCtrl == false)
                {
                    X0[2 * i + 0, 0] = lastcptlt[i].X;
                    X0[2 * i + 1, 0] = lastcptlt[i].Y;
                }
                else
                {
                    X0[2 * i + 0, 0] = (1 - dblProp) * pCorrCptsLt[i].FrCpt.X + dblProp * pCorrCptsLt[i].ToCpt.X;
                    X0[2 * i + 1, 0] = (1 - dblProp) * pCorrCptsLt[i].FrCpt.Y + dblProp * pCorrCptsLt[i].ToCpt.Y;
                }
            }

            //统计插值点数
            int intKnownPt = 0;           //固定点的数目
            int intUnknownPt = 0;         //非固定点的数目

            List<int> intKnownLocationLt = new List<int>();  //记录已知点的序号
            //注意：对于该循环，有一个默认条件，即FromCpl的第一个顶点只有一个对应点
            for (int i = 0; i < pCorrCptsLt.Count; i++)
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
            int intUnknownXY = intUnknownPt * 2;   //每个点都有X、Y坐标

            //找出长度固定的位置(如果一个线段的前后两个点都固定，则该长度固定)。另外，长度固定则该边的方位角也固定
            List<int> intKnownLengthLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 1; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1)
                {
                    intKnownLengthLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownLength = intPtNum - 1 - intKnownLengthLt.Count;

            //找出角度固定的位置(如果一个固定顶点的前后两个点都固定，则该角度固定)
            List<int> intKnownAngleLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 2; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1 && (intKnownLocationLt[i + 2] - intKnownLocationLt[i + 1]) == 1)
                {
                    intKnownAngleLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownAngle = intPtNum - 2 - intKnownAngleLt.Count;

            //长度及角度未知量（方便使用）
            int intUnknownLengthAngle = intUnknownLength + intUnknownAngle;
            //总未知量
            int intUnknownLengthAnglePt = intUnknownLength + intUnknownAngle +intUnknownPt;

            //定义权重矩阵
            int intKnownCount = 0;
            int intUnKnownCount = 0;
            VBMatrix P = new VBMatrix(intUnknownLengthAnglePt, intUnknownLengthAnglePt);
            for (int i = 0; i < intUnknownLength; i++)
            {
                P[i, i] = 1;
            }
            for (int i = 0; i < intUnknownAngle; i++)
            {
                int intSumCount = intKnownCount + intUnKnownCount;

                //开始计算系数值，由于将以下三个情况排列组合将有八种情况，因此按如下方式计算
                if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount+1].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 2].FrCpt.isCtrl == true)
                {                    
                    i -= 1;
                }
                else
                {
                    double dblWeight = 0;
                    if (pCorrCptsLt[intSumCount+1].FrCpt.isCtrl == false)
                    {
                        dblWeight = adblFrLength0[intSumCount] + adblFrLength0[intSumCount + 1] + adblToLength0[intSumCount] + adblToLength0[intSumCount + 1]; 
                    }
                    else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == false && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 2].FrCpt.isCtrl == false)
                    {
                        dblWeight = adblFrLength0[intSumCount] + adblFrLength0[intSumCount + 1] + adblToLength0[intSumCount] + adblToLength0[intSumCount + 1];
                    }
                    else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 2].FrCpt.isCtrl == false)
                    {
                        dblWeight = adblFrLength0[intSumCount + 1] + adblToLength0[intSumCount + 1];
                    }
                    else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == false && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 2].FrCpt.isCtrl == true)
                    {
                        dblWeight =  adblFrLength0[intSumCount] +  adblToLength0[intSumCount];
                    }

                    P[intUnknownLength + i, intUnknownLength + i] = dblWeight;
                }

                if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == true)
                {
                    intKnownCount += 1;
                }
                else
                {
                    intUnKnownCount += 1;
                }
            }
            for (int i = 0; i < intUnknownPt; i++)
            {
                P[intUnknownLengthAngle + i, intUnknownLengthAngle + i] = 0.000000000000001;
            }

            //定义坐标近似值矩阵XA
            VBMatrix XA = new VBMatrix(intUnknownXY, 1);
            VBMatrix XA0 = new VBMatrix(intUnknownXY, 1);
            int intSumCount0 = 0;
            for (int i = 0; i < intUnknownPt; i++)
            {
                if (pCorrCptsLt[intSumCount0].FrCpt.isCtrl == false)
                {
                    XA0[i * 2 + 0, 0] = X0[intSumCount0 * 2 + 0, 0];
                    XA0[i * 2 + 1, 0] = X0[intSumCount0 * 2 + 1, 0];

                    XA[i * 2 + 0, 0] = XA0[i * 2 + 0, 0]-0.0000000001;
                    XA[i * 2 + 1, 0] = XA0[i * 2 + 1, 0] - 0.0000000001;
                }
                else
                {
                    i -= 1;
                }
                intSumCount0 += 1;
            }

            //Xmix里存储了XA和X0的最新混合值（此矩阵在公式推导中并不存在，只是为了方便编写代码而建立）
            VBMatrix Xmix = new VBMatrix(intXYNum, 1);
            for (int i = 0; i < intXYNum; i++)
            {
                Xmix[i, 0] = X0[i, 0];
            }

            //近似值与观测值之差matl，平差中的-l
            VBMatrix matl = new VBMatrix(intUnknownLengthAnglePt, 1);

            //定义系数矩阵A(各方程对坐标的导数值)，A的导数值将在循环中给出
            VBMatrix A = new VBMatrix(intUnknownLengthAnglePt, intUnknownXY);
            double dblJudge1 = 0;   //该值用于判断是否应该跳出循环
            double dblJudge2 = 0;   //该值用于判断是否应该跳出循环
            int intJudgeIndex = intUnknownLength / 4;
            int intIterativeCount = 0;
            double[] adblSubDis = new double[intPtNum - 1];
            double[] adblAngle = new double[intPtNum - 2];
            double[] adblAzimuth = new double[intPtNum - 1]; 
            do
            {
                //计算系数矩阵A第0行到"intUnknownLength"行的各元素，即线段长度对各未知数求偏导的值
                for (int i = 0; i < intPtNum - 1; i++)
                {
                    adblSubDis[i] = Math.Pow((Xmix[2 * i, 0] - Xmix[2 * i + 2, 0]) * (Xmix[2 * i, 0] - Xmix[2 * i + 2, 0]) + (Xmix[2 * i + 1, 0] - Xmix[2 * i + 3, 0]) * (Xmix[2 * i + 1, 0] - Xmix[2 * i + 3, 0]), 0.5);
                }
                //计算新的方位角
                adblAzimuth[0] = CGeoFunc.CalAxisAngle(Xmix[0, 0], Xmix[1, 0], Xmix[2, 0], Xmix[3, 0]);
                for (int i = 1; i < intPtNum - 1; i++)
                {
                    adblAngle[i - 1] = CGeoFunc.CalAngle_Counterclockwise(Xmix[i * 2 - 2, 0], Xmix[i * 2 - 1, 0], Xmix[i * 2, 0], Xmix[i * 2 + 1, 0], Xmix[i * 2 + 2, 0], Xmix[i * 2 + 3, 0]);
                    adblAzimuth[i] = adblAzimuth[i - 1] + adblAngle[i - 1] - Math.PI;
                }

                //计算系数矩阵中关于长度值的导数部分
                _pCAL.CalADevLength(pCorrCptsLt, 0, intUnknownLength, ref A, ref matl, adblSubDis, adblAzimuth, adblLength0);

                //计算系数矩阵中关于夹角值的导数部分
                _pCAL.CalADevAngle(pCorrCptsLt, intUnknownLength, intUnknownAngle, Xmix, ref A, ref matl, adblSubDis, adblAngle, adblAngle0);

                //计算系数矩阵中关于与上一个LSA结果坐标距离的导数部分
                for (int i = 0; i < intUnknownPt ; i++)
                {
                    //平差后的点与估计值点之间的距离
                    double dblDis = Math.Pow((XA[2 * i, 0] - XA0[2 * i, 0]) * (XA[2 * i, 0] - XA0[2 * i, 0]) + (XA[2 * i + 1, 0] - XA0[2 * i + 1, 0]) * (XA[2 * i + 1, 0] - XA0[2 * i + 1, 0]), 0.5);

                    A[intUnknownLengthAngle + i, i * 2 + 0] = (XA[2 * i + 0, 0] - XA0[2 * i + 0, 0]) / dblDis;
                    A[intUnknownLengthAngle + i, i * 2 + 1] = (XA[2 * i + 1, 0] - XA0[2 * i + 1, 0]) / dblDis; 

                    //if (dblDis==0)
                    //{
                    //    //导数值
                    //    A[intUnknownLengthAngle + i, i * 2 + 0] = 0;
                    //    A[intUnknownLengthAngle + i, i * 2 + 1] = 0; 
                    //}
                    //else
                    //{
                    //    //导数值
                    //    A[intUnknownLengthAngle + i, i * 2 + 0] = (XA[2 * i + 0, 0] - XA0[2 * i + 0, 0]) / dblDis;
                    //    A[intUnknownLengthAngle + i, i * 2 + 1] = (XA[2 * i + 1, 0] - XA0[2 * i + 1, 0]) / dblDis; 
                    //}

                    matl[intUnknownLengthAngle + i, 0] = -dblDis;    //图方便，顺便计算matl
                }

                //CHelpFuncExcel.ExportDataToExcel2(A, "maxA", _DataRecords.ParameterInitialize.strSavePath);
                //CHelpFuncExcel.ExportDataToExcelP(P, "maxP", _DataRecords.ParameterInitialize.strSavePath);
                //CHelpFuncExcel.ExportDataToExcel2(matl, "maxmatl", _DataRecords.ParameterInitialize.strSavePath);


                //平差（阻尼最小二乘法）
                VBMatrix Temp = A.Trans() * P * A;                
                //VBMatrix E = new VBMatrix(intUnknownXY, intUnknownXY);  //单位矩阵
                //for (int i = 0; i < intUnknownXY; i++)
                //{
                //    E[i, i] = 1;
                //}
                //Temp = Temp + E;
                VBMatrix InvTemp = Temp.Inv(Temp);
                VBMatrix x = InvTemp * A.Trans() * P * matl;

                //CHelpFuncExcel.ExportDataToExcel2(x, "maxX", _DataRecords.ParameterInitialize.strSavePath);
                //CHelpFuncExcel.ExportDataToExcel2(XA, "maxXA", _DataRecords.ParameterInitialize.strSavePath);

                XA += x;

                //对坐标值进行改正
                int intSumCount5 = 0;
                for (int i = 0; i < intUnknownPt; i++)
                {
                    if (pCorrCptsLt[intSumCount5].FrCpt.isCtrl == false)
                    {
                        Xmix[intSumCount5 * 2 + 0, 0] = XA[i * 2 + 0, 0];
                        Xmix[intSumCount5 * 2 + 1, 0] = XA[i * 2 + 1, 0];
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCount5 += 1;
                }

                if (intIterativeCount == 50)
                {
                    int kk = 5;
                }

                intIterativeCount += 1;


                if (intIterativeCount >= 20)
                {
                    break;
                }

                //这里只是随便取两个中间值以观测是否收敛
                dblJudge1 = Math.Abs(x[intJudgeIndex, 0]);
                dblJudge2 = Math.Abs(x[3 * intJudgeIndex, 0]);
            } while ((dblJudge1 > dblTX) || (dblJudge2 > dblTX));


            //生成目标线段
            List<CPoint> CTargetPtLt = new List<CPoint>();
            for (int i = 0; i < intPtNum; i++)
            {
                CPoint cpt = new CPoint(i);
                cpt.X = Xmix[2 * i, 0];
                cpt.Y = Xmix[2 * i + 1, 0];

                if (pCorrCptsLt[i].FrCpt.isCtrl == true)
                {
                    cpt.isCtrl = true;
                }
                else
                {
                    cpt.isCtrl = false;
                }

                CTargetPtLt.Add(cpt);
            }
            CPolyline cpl = new CPolyline(0, CTargetPtLt);
            cpl.CreateSubPllt();

            //记录各平差成果
            //坐标改正值
            VBMatrix Xc = XA-XA0;
            //观测值改正值矩阵V
            VBMatrix V = A * Xc + matl;
            //VtPV值
            cpl.dblVtPV = (V.Trans() * P * V).MatData[0, 0];


            int intUnKnownCountL6 = 0;
            for (int i = 0; i < intPtNum - 1; i++)
            {
                if (pCorrCptsLt[i].FrCpt.isCtrl == false || pCorrCptsLt[i + 1].FrCpt.isCtrl == false)
                {
                    cpl.SubCPlLt[i].dblLengthV = cpl.SubCPlLt[i].pPolyline.Length - adblLength0[i];
                    //double dblLength = cpl.SubCPlLt[i].Length;
                    //double dblLength0 = adblLength0[i];


                    intUnKnownCountL6 += 1;
                }
                else
                {
                    cpl.SubCPlLt[i].dblLengthV = 0;
                }
            }

            int intUnKnownCountA6 = 0;
            for (int i = 0; i < intPtNum - 2; i++)
            {
                if (pCorrCptsLt[i].FrCpt.isCtrl == false || pCorrCptsLt[i + 1].FrCpt.isCtrl == false || pCorrCptsLt[i + 2].FrCpt.isCtrl == false)
                {
                    double dblAngle = CGeoFunc.CalAngle_Counterclockwise(cpl.CptLt[i], cpl.CptLt[i + 1], cpl.CptLt[i + 2]);
                    cpl.CptLt[i + 1].dblAngleV = dblAngle - adblAngle0[i];
                    //double dblAngle = CGeoFunc.CalAngle_Counterclockwise(cpl.CptLt[i], cpl.CptLt[i + 1], cpl.CptLt[i + 2]);
                    //cpl.CptLt[i + 1].dblAngleV = V[intUnknownLength + intUnKnownCountA6, 0];
                    intUnKnownCountA6 += 1;
                }
                else
                {                    
                    cpl.CptLt[i + 1].dblAngleV = 0;
                }
            }

            return cpl;

        }


    }
}
