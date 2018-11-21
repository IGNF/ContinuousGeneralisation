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
    /// 基于最小二乘原理的Morphing方法，以角度和边长为参数(Least Squares Alogrithm_Angle and Length)
    /// </summary>
    /// <remarks>该方法以边长和角度作为变量进行平差，坐标由平差后的边长角度推算出来</remarks>
    public class CAL_AL
    {
        private CDataRecords _DataRecords;                    //records of data
        private double _dblTX;
        
        
        private CPAL _pCAL = new CPAL();

        public CAL_AL()
        {

        }

        public CAL_AL(CDataRecords pDataRecords,double dblTX)
        {
            _DataRecords = pDataRecords;
            _dblTX = dblTX;
        }

        public CAL_AL(CDataRecords pDataRecords)
        {
            _DataRecords = pDataRecords;
            CPolyline FromCpl = pDataRecords.ParameterResult.FromCpl;
            _dblTX = FromCpl.pPolyline.Length / FromCpl.CptLt .Count  / 10000000000;   //计算阈值参数
        }


        /// <summary>
        /// 显示并返回单个插值面状要素
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">差值参数</param>
        /// <returns>面状要素</returns>
        public CPolyline DisplayInterpolation(double dblProp)
        {
            if (dblProp < 0 || dblProp > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return null;
            }

            CPolyline cpl = GetTargetcpl(dblProp);

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
        public CPolyline GetTargetcpl(double dblProp)
        {
            List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;   //Read Datasets后，此处ResultPtLt中的对应点为一一对应
            double dblTX = _dblTX;

            int intPtNum = pCorrCptsLt.Count;
            //计算长度初始值（全部计算）
            double[] adblLength0 = new double[intPtNum - 1];
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                double dblfrsublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].FrCpt, pCorrCptsLt[i].FrCpt);
                double dbltosublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].ToCpt, pCorrCptsLt[i].ToCpt);
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

            //生成点数组（初始值），同时计算各线段方位角混合值
            //注意：默认固定第一条边
            List <CPoint > cptlt=new List<CPoint> ();
            double[] adblAzimuth = new double[intPtNum - 1];
            //计算第一个点和第二个点
            double dblnewX0 = (1 - dblProp) * pCorrCptsLt[0].FrCpt.X + dblProp * pCorrCptsLt[0].ToCpt.X;
            double dblnewY0 = (1 - dblProp) * pCorrCptsLt[0].FrCpt.Y + dblProp * pCorrCptsLt[0].ToCpt.Y;
            CPoint newcpt0 = new CPoint(0, dblnewX0, dblnewY0);
            newcpt0.isCtrl = true;
            cptlt.Add(newcpt0);
            double dblnewX1 = (1 - dblProp) * pCorrCptsLt[1].FrCpt.X + dblProp * pCorrCptsLt[1].ToCpt.X;
            double dblnewY1 = (1 - dblProp) * pCorrCptsLt[1].FrCpt.Y + dblProp * pCorrCptsLt[1].ToCpt.Y;
            CPoint newcpt1 = new CPoint(1, dblnewX1, dblnewY1);
            newcpt1.isCtrl = true;
            cptlt.Add(newcpt1);
            adblAzimuth[0] = CGeoFunc.CalAxisAngle(newcpt0, newcpt1);
            ////后面的其它点（不固定最后两个点）
            ////pCorrCptsLt[intPtNum - 2].FrCpt.isCtrl = false;
            ////pCorrCptsLt[intPtNum - 1].FrCpt.isCtrl = false;
            //for (int i = 2; i < pCorrCptsLt.Count; i++)
            //{
            //    CPoint newcpt = new CPoint();
            //    if (pCorrCptsLt[i].FrCpt.isCtrl == false)
            //    {
            //        adblAzimuth[i - 1] = adblAzimuth[i - 2] + adblAngle0[i - 2] - Math.PI;
            //        double dblnewX = cptlt[i - 1].X + adblLength0[i - 1] * Math.Cos(adblAzimuth[i - 1]);
            //        double dblnewY = cptlt[i - 1].Y + adblLength0[i - 1] * Math.Sin(adblAzimuth[i - 1]);
            //        newcpt = new CPoint(i, dblnewX, dblnewY);
            //    }
            //    else
            //    {
            //        double dblnewX = (1 - dblProp) * pCorrCptsLt[i].FrCpt.X + dblProp * pCorrCptsLt[i].ToCpt.X;
            //        double dblnewY = (1 - dblProp) * pCorrCptsLt[i].FrCpt.Y + dblProp * pCorrCptsLt[i].ToCpt.Y;
            //        newcpt = new CPoint(i, dblnewX, dblnewY);
            //        newcpt.isCtrl = true;

            //        //计算角度：不能采用“CGeoFunc.CalAxisAngle”，因为此处的方位角不一定在0到2Pi之间，采用重新连接法
            //        double dblAngle = CGeoFunc.CalAngle_Counterclockwise(cptlt[cptlt.Count - 2], cptlt[cptlt.Count - 1], newcpt);  //计算实际夹角 
            //        adblAzimuth[i - 1] = adblAzimuth[i - 2] + dblAngle - Math.PI;
            //    }
            //    cptlt.Add(newcpt);
            //}

            //后面的其它点（固定最后两个点）
            for (int i = 2; i < pCorrCptsLt.Count - 2; i++)
            {
                CPoint newcpt = new CPoint();
                if (pCorrCptsLt[i].FrCpt.isCtrl == false)
                {
                    adblAzimuth[i - 1] = adblAzimuth[i - 2] + adblAngle0[i - 2] - Math.PI;
                    double dblnewX = cptlt[i - 1].X + adblLength0[i - 1] * Math.Cos(adblAzimuth[i - 1]);
                    double dblnewY = cptlt[i - 1].Y + adblLength0[i - 1] * Math.Sin(adblAzimuth[i - 1]);
                    newcpt = new CPoint(i, dblnewX, dblnewY);
                }
                else
                {
                    double dblnewX = (1 - dblProp) * pCorrCptsLt[i].FrCpt.X + dblProp * pCorrCptsLt[i].ToCpt.X;
                    double dblnewY = (1 - dblProp) * pCorrCptsLt[i].FrCpt.Y + dblProp * pCorrCptsLt[i].ToCpt.Y;
                    newcpt = new CPoint(i, dblnewX, dblnewY);
                    newcpt.isCtrl = true;

                    //计算角度：不能采用“CGeoFunc.CalAxisAngle”，因为此处的方位角不一定在0到2Pi之间，采用重新连接法
                    double dblAngle = CGeoFunc.CalAngle_Counterclockwise(cptlt[cptlt.Count - 2], cptlt[cptlt.Count - 1], newcpt);  //计算实际夹角 
                    adblAzimuth[i - 1] = adblAzimuth[i - 2] + dblAngle - Math.PI;
                }
                cptlt.Add(newcpt);
            }
            //计算最后两个点
            double dblnewXlast1 = (1 - dblProp) * pCorrCptsLt[pCorrCptsLt.Count - 2].FrCpt.X + dblProp * pCorrCptsLt[pCorrCptsLt.Count - 2].ToCpt.X;
            double dblnewYlast1 = (1 - dblProp) * pCorrCptsLt[pCorrCptsLt.Count - 2].FrCpt.Y + dblProp * pCorrCptsLt[pCorrCptsLt.Count - 2].ToCpt.Y;
            CPoint newcptlast1 = new CPoint(pCorrCptsLt.Count - 2, dblnewXlast1, dblnewYlast1);
            newcptlast1.isCtrl = true;
            cptlt.Add(newcptlast1);
            double dblAnglelast1 = CGeoFunc.CalAngle_Counterclockwise(cptlt[cptlt.Count - 3], cptlt[cptlt.Count - 2], cptlt[cptlt.Count - 1]);  //计算实际夹角 
            adblAzimuth[pCorrCptsLt.Count - 3] = adblAzimuth[pCorrCptsLt.Count - 4] + dblAnglelast1 - Math.PI;

            double dblnewXlast0 = (1 - dblProp) * pCorrCptsLt[pCorrCptsLt.Count - 1].FrCpt.X + dblProp * pCorrCptsLt[pCorrCptsLt.Count - 1].ToCpt.X;
            double dblnewYlast0 = (1 - dblProp) * pCorrCptsLt[pCorrCptsLt.Count - 1].FrCpt.Y + dblProp * pCorrCptsLt[pCorrCptsLt.Count - 1].ToCpt.Y;
            CPoint newcptlast0 = new CPoint(pCorrCptsLt.Count - 1, dblnewXlast0, dblnewYlast0);
            newcptlast0.isCtrl = true;
            cptlt.Add(newcptlast0);
            double dblAnglelast0 = CGeoFunc.CalAngle_Counterclockwise(cptlt[cptlt.Count - 3], cptlt[cptlt.Count - 2], cptlt[cptlt.Count - 1]);  //计算实际夹角 
            adblAzimuth[pCorrCptsLt.Count - 2] = adblAzimuth[pCorrCptsLt.Count - 3] + dblAnglelast0 - Math.PI;


            //统计插值点数
            int intKnownNum = 0;           //固定点的数目
            int intUnknownNum = 0;         //非固定点的数目

            List<int> intKnownLocationLt = new List<int>();  //记录已知点的序号
            //注意：对于该循环，有一个默认条件，即FromCpl的第一个顶点只有一个对应点
            for (int i = 0; i < cptlt.Count; i++)
            {
                if (cptlt[i].isCtrl == true)
                {
                    intKnownLocationLt.Add(i);
                    intKnownNum += 1;
                }
                else
                {
                    intUnknownNum += 1;
                }
            }

            //找出长度固定的位置(如果一个线段的前后两个点都固定，则该长度固定)。另外，长度固定则该边的方位角也固定
            List<int> intKnownLengthLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 1; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1)
                {
                    intKnownLengthLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownLength = pCorrCptsLt.Count - 1 - intKnownLengthLt.Count;

            //找出角度固定的位置(如果一个固定顶点的前后两个点都固定，则该角度固定)
            List<int> intKnownAngleLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 2; i++)
            {
                if ((intKnownLocationLt[i+1] - intKnownLocationLt[i]) == 1 && (intKnownLocationLt[i + 2] - intKnownLocationLt[i+1]) == 1)
                {
                    intKnownAngleLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownAngle = pCorrCptsLt.Count - 2 - intKnownAngleLt.Count;

            //总未知量
            int intUnknownLengthAngle = intUnknownLength + intUnknownAngle;

            //坐标约束个数
            int intXYCst = (intKnownLocationLt.Count - 1 - intKnownLengthLt.Count) * 2; //如果两个相邻点都是控制点，则这两个相邻点之间不存在坐标约束
            //夹角约束个数
            int intAngleCst = intKnownLengthLt.Count - 1-intKnownAngleLt.Count;  //同坐标约束，如果有两相邻边都为已知边，则之间不存在夹角约束
            //总约束个数
            int intSumCst = intUnknownLengthAngle + intXYCst + intAngleCst;

            //定义权重矩阵
            VBMatrix P = new VBMatrix(intSumCst, intSumCst);
            for (int i = 0; i < intUnknownLength; i++)
            {
                P[i, i] = 1;
            }
            for (int i = 0; i < intUnknownAngle; i++)
            {
                P[intUnknownLength + i, intUnknownLength + i] = 1000;
            }
            for (int i = 0; i < intXYCst; i++)
            {
                P[intUnknownLengthAngle + i, intUnknownLengthAngle + i] = 1;
            }
            for (int i = 0; i < intAngleCst; i++)
            {
                P[intUnknownLengthAngle + intXYCst + i, intUnknownLengthAngle + intXYCst + i] =1;
            }

            //计算初始值矩阵X0（注意：此处的X0中并未包含任何坐标，而是长度和夹角的初值）
            VBMatrix X0 = new VBMatrix(intPtNum * 2 - 3, 1);
            for (int i = 0; i < (intPtNum - 1); i++)
            {
                X0[i, 0] = adblLength0[i];
            }
            for (int i = 0; i < (intPtNum - 2); i++)
            {
                X0[intPtNum - 1 + i, 0] = adblAngle0[i];
            }

            //Xmix里存储了XA和X0的最新混合值（此矩阵在公式推导中并不存在，只是为了方便编写代码而建立）
            VBMatrix Xmix = new VBMatrix(intPtNum * 2 - 3, 1);
            for (int i = 0; i < X0.Row; i++)
            {
                Xmix[i, 0] = X0[i, 0];
            }

            //定义近似值矩阵XA（注意：同上，此处的XA中并未包含任何坐标，而是长度和夹角的近似值）
            VBMatrix XA = new VBMatrix(intUnknownLengthAngle, 1);
            int intSumCountL = 0;
            for (int i = 0; i < intUnknownLength; i++) //长度近似值部分
            {
                if (cptlt[intSumCountL].isCtrl == false || cptlt[intSumCountL + 1].isCtrl == false)
                {
                    XA[i, 0] = X0[intSumCountL, 0];
                }
                else
                {
                    i -= 1;
                }
                intSumCountL += 1;
            }
            int intSumCountA = 0;
            for (int i = intUnknownLength; i < intUnknownLengthAngle; i++) //角度近似值部分
            {
                if (cptlt[intSumCountA].isCtrl == false || cptlt[intSumCountA + 1].isCtrl == false || cptlt[intSumCountA + 2].isCtrl == false)
                {
                    XA[i, 0] = X0[intPtNum -1 + intSumCountA, 0];
                }
                else
                {
                    i -= 1;
                }
                intSumCountA += 1;
            }

            //定义系数矩阵，系数矩阵来源于四类约束方程：1、长度本身；2、角度本身；3、X、Y的闭合差；4、方位角闭合差
            //此处仅给出“长度本身”和“角度本身”的系数，有关“ X、Y的闭合差”和“方位角闭合差”的系数将在循环中给出
            VBMatrix A = new VBMatrix(intSumCst, intUnknownLengthAngle);
            for (int i = 0; i < intUnknownLengthAngle; i++)
            {
                A[i, i] = 1;
            }

            double dblJudge1 = 0;   //该值用于判断是否应该跳出循环
            double dblJudge2 = 0;   //该值用于判断是否应该跳出循环
            int intJudgeIndex = intUnknownLength  / 4;
            int intIterativeCount = 0;

            do
            {               
                VBMatrix matl = new VBMatrix(intSumCst, 1);

                //计算matl关于“长度本身”这部分的值
                int intSumCountL1 = 0;
                for (int i = 0; i < intUnknownLength; i++)
                {
                    if (cptlt[intSumCountL1].isCtrl == false || cptlt[intSumCountL1 + 1].isCtrl == false)
                    {
                        matl[i, 0] = XA[i, 0] - X0[intSumCountL1, 0];
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCountL1 += 1;
                }
                //计算matl关于“角度本身”这部分的值
                int intSumCountA1 = 0;
                for (int i = intUnknownLength; i < intUnknownLengthAngle; i++)
                {
                    if (cptlt[intSumCountA1].isCtrl == false || cptlt[intSumCountA1 + 1].isCtrl == false || cptlt[intSumCountA1 + 2].isCtrl == false)
                    {
                        matl[i, 0] = XA[i, 0] - X0[intPtNum -1 + intSumCountA1, 0];
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCountA1 += 1;
                }

                //计算系数矩阵A第"intUnknownLengthAngle"行到"intUnknownLengthAngle + intXYCst - 1"行的各元素，即从由坐标约束产生的各偏导值
                if (intKnownLocationLt .Count >=2)
                {                    
                    int intRow = intUnknownLengthAngle;
                    int intLastIsCtrl = 0;
                    for (int i = 0; i < cptlt.Count; i++)
                    {
                        if (cptlt[i].isCtrl == true)
                        {
                            intLastIsCtrl = i;
                            break;
                        }
                    }
                    int intKnownLength = 0;
                    int intKnownAngle = 0;
                    for (int i = intLastIsCtrl + 1; i < cptlt.Count; i++)
                    {
                        if (cptlt[i].isCtrl == true && cptlt[i - 1].isCtrl != true)
                        {
                            double dblSumDerX = new double();
                            double dblSumDerY = new double();

                            for (int j = intLastIsCtrl; j < i; j++)  //注意，此处的j不可以等于0，因为等于0的时候该处不存在夹角。当然，由于之前规定前两个固定，因此j>=1
                            {
                                dblSumDerX = 0;
                                dblSumDerY = 0;

                                //关于X的约束方程，由于分别可以从左侧和右侧推算，因此有两个约束方程
                                A[intRow, j - intKnownLength] = Math.Cos(adblAzimuth[j]);  //关于长度的偏导值
                                for (int k = j; k < i; k++)
                                {
                                    dblSumDerX -= (adblLength0[k] * Math.Sin(adblAzimuth[k]));  //关于夹角的偏导值
                                }
                                A[intRow, j - 1 - intKnownAngle + intUnknownLength] = dblSumDerX;

                                   //关于Y的约束方程，行加1，列相同
                                A[intRow + 1, j - intKnownLength] = Math.Sin(adblAzimuth[j]);  //关于长度的偏导值
                                for (int k = j; k < i; k++)
                                {
                                    dblSumDerY += (adblLength0[k] * Math.Cos(adblAzimuth[k]));  //关于夹角的偏导值
                                }
                                A[intRow + 1, j - 1 - intKnownAngle + intUnknownLength] = dblSumDerY;
                               

                                if (j == intLastIsCtrl)
                                {
                                    matl[intRow + 0, 0] = dblSumDerY - (cptlt[i].X - cptlt[intLastIsCtrl].X);   //图方便，顺便计算matl（此处之所以利用之前的成果dblSumDerY，是因为数值上正好相等）
                                    matl[intRow + 1, 0] = -dblSumDerX - (cptlt[i].Y - cptlt[intLastIsCtrl].Y);   //图方便，顺便计算matl（此处之所以利用之前的成果-dblSumDerX，是因为数值上正好相等）
                                }
                            }

                            intRow += 2;
                            intLastIsCtrl = i;
                        }
                        else if (cptlt[i].isCtrl == true && cptlt[i - 1].isCtrl == true)  //相邻两个点的坐标都知道，不产生约束条件
                        {
                            intKnownLength += 1;
                            if (i >= 2)
                            {
                                if (cptlt[i - 2].isCtrl == true)
                                {
                                    intKnownAngle += 1;
                                }
                            }
                            intLastIsCtrl = i;
                        }
                    }
                }

                //计算系数矩阵A第"intUnknownLengthAngle + intXYCst"行到"intUnknownLengthAngle + intXYCst + intAngleCst - 1 (即intSumCst - 1)"行的各元素，即夹角闭合差产生的各偏导值
                if (intKnownLengthLt.Count >= 2)
                {                    
                    int intRow = intUnknownLengthAngle + intXYCst;
                    int intLastIsCtrl = 0;
                    for (int i = 0; i < cptlt.Count-1; i++)
                    {
                        if (cptlt[i].isCtrl == true && cptlt[i + 1].isCtrl == true)
                        {
                            intLastIsCtrl = i;
                            break;
                        }
                    }
                    int intKnownAngle = 0;
                    for (int i = intLastIsCtrl + 1; i < cptlt.Count-1; i++)
                    {
                        if (cptlt[i].isCtrl == true && cptlt[i + 1].isCtrl == true && cptlt[i - 1].isCtrl != true)
                        {
                            double dblAngleSum = 0;
                            for (int j = intLastIsCtrl; j < i; j++)
                            {
                                //关于夹角的约束方程
                                A[intRow, j - intKnownAngle + intUnknownLength] = 1;
                                //夹角累计值，为计算matl做准备
                                dblAngleSum += adblAngle0[j];
                            }

                            matl[intRow, 0] = dblAngleSum - (i - intLastIsCtrl) * Math.PI - (adblAzimuth[i] - adblAzimuth[intLastIsCtrl]);   //图方便，顺便计算matl

                            double tt = (i - intLastIsCtrl) * Math.PI;
                            double ss = (adblAzimuth[i] - adblAzimuth[intLastIsCtrl]);

                            intRow += 1;
                            intLastIsCtrl = i;
                        }
                        else if (cptlt[i].isCtrl == true && cptlt[i + 1].isCtrl == true && cptlt[i - 1].isCtrl == true)  //相邻三个点的坐标都知道，不产生约束条件
                        {
                            intKnownAngle += 1;
                            intLastIsCtrl = i;
                        }
                    }
                }
                
                //计算新的近似值
                //SaveFileDialog SFD = new SaveFileDialog();
                //SFD.ShowDialog();
                //CHelpFuncExcel.ExportDataToExcelA(A, "maxA", _DataRecords.ParameterInitialize.strSavePath);
                //CHelpFuncExcel.ExportDataToExcelP(P, "maxP", _DataRecords.ParameterInitialize.strSavePath);
                //CHelpFuncExcel.ExportDataToExcel2(matl, "maxmatl", _DataRecords.ParameterInitialize.strSavePath);

                //平差
                VBMatrix x = _pCAL.InvAtPAAtPmatl(A, P, matl);
                XA -= x;

                //更新Xmix
                int intSumCountL4 = 0;
                for (int i = 0; i < intUnknownLength; i++) //长度近似值部分
                {
                    if (cptlt[intSumCountL4].isCtrl == false || cptlt[intSumCountL4 + 1].isCtrl == false)
                    {
                        Xmix[intSumCountL4, 0] = XA[i, 0];   //其实此处的“intSumCountL4”等于"intPtNum-1"
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCountL4 += 1;
                }
                int intSumCountA4 = 0;
                for (int i = intUnknownLength; i < intUnknownLengthAngle; i++) //角度近似值部分
                {
                    if (cptlt[intSumCountA4].isCtrl == false || cptlt[intSumCountA4 + 1].isCtrl == false || cptlt[intSumCountA4 + 2].isCtrl == false)
                    {
                        Xmix[intPtNum - 1 + intSumCountA4, 0] = XA[ i, 0];   //其实此处的“intSumCountL4”等于"intPtNum-1"
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCountA4 += 1;
                }

                //更新adblAzimuth和cptlt
                for (int i = 2; i < cptlt.Count; i++)
                {
                    if (cptlt[i].isCtrl == false)
                    {
                        adblAzimuth[i - 1] = adblAzimuth[i - 2] + Xmix[intPtNum -1+ i - 2,0] - Math.PI;
                        double dblnewX = cptlt[i - 1].X + Xmix[i - 1, 0] * Math.Cos(adblAzimuth[i - 1]);
                        double dblnewY = cptlt[i - 1].Y + Xmix[i - 1, 0] * Math.Sin(adblAzimuth[i - 1]);
                        CPoint newcpt = new CPoint(i, dblnewX, dblnewY);
                        //double dbloldX = cptlt[i].X;
                        //double dbloldY = cptlt[i].Y;
                        cptlt.RemoveAt (i);
                        cptlt.Insert(i, newcpt);
                    }
                }

                intIterativeCount += 1;
                if (intIterativeCount >= 1000)
                {
                    break;
                }
                dblJudge1 = Math.Abs(x[intJudgeIndex, 0]);
                dblJudge2 = Math.Abs(x[3 * intJudgeIndex, 0]);
            } while ((dblJudge1 > dblTX) || (dblJudge2 > dblTX));


            //生成目标线段
            CPolyline cpl = new CPolyline(0, cptlt);
            return cpl;
        }


        /// <summary>
        /// 获取线状要素（实验结果说明：该步骤中用到的约束是多余的）
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProp">插值参数</param>
        /// <returns>在处理面状要素时，本程序将原面状要素的边界切开，按线状要素处理，处理完后再重新生成面状要素</returns>
        public CPolyline GetTargetcpl2(double dblProp)
        {
            List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;   //Read Datasets后，此处ResultPtLt中的对应点为一一对应
            double dblTX = _dblTX;

            int intPtNum = pCorrCptsLt.Count;
            //计算长度初始值（全部计算）
            double[] adblLength0 = new double[intPtNum - 1];
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                double dblfrsublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].FrCpt, pCorrCptsLt[i].FrCpt);
                double dbltosublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].ToCpt, pCorrCptsLt[i].ToCpt);
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

            //生成点数组（初始值），同时计算各线段方位角混合值
            //注意：默认固定第一条边
            List<CPoint> cptlt = new List<CPoint>();
            double[] adblAzimuth = new double[intPtNum - 1];
            //计算第一个点和第二个点
            double dblnewX0 = (1 - dblProp) * pCorrCptsLt[0].FrCpt.X + dblProp * pCorrCptsLt[0].ToCpt.X;
            double dblnewY0 = (1 - dblProp) * pCorrCptsLt[0].FrCpt.Y + dblProp * pCorrCptsLt[0].ToCpt.Y;
            CPoint newcpt0 = new CPoint(0, dblnewX0, dblnewY0);
            newcpt0.isCtrl = true;
            cptlt.Add(newcpt0);
            double dblnewX1 = (1 - dblProp) * pCorrCptsLt[1].FrCpt.X + dblProp * pCorrCptsLt[1].ToCpt.X;
            double dblnewY1 = (1 - dblProp) * pCorrCptsLt[1].FrCpt.Y + dblProp * pCorrCptsLt[1].ToCpt.Y;
            CPoint newcpt1 = new CPoint(1, dblnewX1, dblnewY1);
            newcpt1.isCtrl = true;
            cptlt.Add(newcpt1);
            adblAzimuth[0] = CGeoFunc.CalAxisAngle(newcpt0, newcpt1);
            ////后面的其它点（不固定最后两个点）
            //for (int i = 2; i < pCorrCptsLt.Count ; i++)
            //{
            //    CPoint newcpt = new CPoint();
            //    if (pCorrCptsLt[i].FrCpt.isCtrl == false)
            //    {
            //        adblAzimuth[i - 1] = adblAzimuth[i - 2] + adblAngle0[i - 2] - Math.PI;
            //        double dblnewX = cptlt[i - 1].X + adblLength0[i - 1] * Math.Cos(adblAzimuth[i - 1]);
            //        double dblnewY = cptlt[i - 1].Y + adblLength0[i - 1] * Math.Sin(adblAzimuth[i - 1]);
            //        newcpt = new CPoint(i, dblnewX, dblnewY);
            //    }
            //    else
            //    {
            //        double dblnewX = (1 - dblProp) * pCorrCptsLt[i].FrCpt.X + dblProp * pCorrCptsLt[i].ToCpt.X;
            //        double dblnewY = (1 - dblProp) * pCorrCptsLt[i].FrCpt.Y + dblProp * pCorrCptsLt[i].ToCpt.Y;
            //        newcpt = new CPoint(i, dblnewX, dblnewY);
            //        newcpt.isCtrl = true;
            //        adblAzimuth[i - 1] = CGeoFunc.CalAxisAngle(cptlt[i-1], newcpt);
            //    }
            //    cptlt.Add(newcpt);
            //}

            //后面的其它点（固定最后两个点）
            for (int i = 2; i < pCorrCptsLt.Count - 2; i++)
            {
                CPoint newcpt = new CPoint();
                if (pCorrCptsLt[i].FrCpt.isCtrl == false)
                {
                    adblAzimuth[i - 1] = adblAzimuth[i - 2] + adblAngle0[i - 2] - Math.PI;
                    double dblnewX = cptlt[i - 1].X + adblLength0[i - 1] * Math.Cos(adblAzimuth[i - 1]);
                    double dblnewY = cptlt[i - 1].Y + adblLength0[i - 1] * Math.Sin(adblAzimuth[i - 1]);
                    newcpt = new CPoint(i, dblnewX, dblnewY);
                }
                else
                {
                    double dblnewX = (1 - dblProp) * pCorrCptsLt[i].FrCpt.X + dblProp * pCorrCptsLt[i].ToCpt.X;
                    double dblnewY = (1 - dblProp) * pCorrCptsLt[i].FrCpt.Y + dblProp * pCorrCptsLt[i].ToCpt.Y;
                    newcpt = new CPoint(i, dblnewX, dblnewY);
                    newcpt.isCtrl = true;
                    adblAzimuth[i - 1] = CGeoFunc.CalAxisAngle(cptlt[i - 1], newcpt);
                }
                cptlt.Add(newcpt);
            }
            //计算最后两个点
            double dblnewXlast1 = (1 - dblProp) * pCorrCptsLt[pCorrCptsLt.Count - 2].FrCpt.X + dblProp * pCorrCptsLt[pCorrCptsLt.Count - 2].ToCpt.X;
            double dblnewYlast1 = (1 - dblProp) * pCorrCptsLt[pCorrCptsLt.Count - 2].FrCpt.Y + dblProp * pCorrCptsLt[pCorrCptsLt.Count - 2].ToCpt.Y;
            CPoint newcptlast1 = new CPoint(pCorrCptsLt.Count - 2, dblnewXlast1, dblnewYlast1);
            newcptlast1.isCtrl = true;
            cptlt.Add(newcptlast1);
            adblAzimuth[pCorrCptsLt.Count - 3] = CGeoFunc.CalAxisAngle(cptlt[cptlt.Count - 2], cptlt[cptlt.Count - 1]);

            double dblnewXlast0 = (1 - dblProp) * pCorrCptsLt[pCorrCptsLt.Count - 1].FrCpt.X + dblProp * pCorrCptsLt[pCorrCptsLt.Count - 1].ToCpt.X;
            double dblnewYlast0 = (1 - dblProp) * pCorrCptsLt[pCorrCptsLt.Count - 1].FrCpt.Y + dblProp * pCorrCptsLt[pCorrCptsLt.Count - 1].ToCpt.Y;
            CPoint newcptlast0 = new CPoint(pCorrCptsLt.Count - 1, dblnewXlast0, dblnewYlast0);
            newcptlast0.isCtrl = true;
            cptlt.Add(newcptlast0);
            adblAzimuth[pCorrCptsLt.Count - 2] = CGeoFunc.CalAxisAngle(newcptlast1, newcptlast0);


            //统计插值点数
            int intKnownNum = 0;           //固定点的数目
            int intUnknownNum = 0;         //非固定点的数目

            List<int> intKnownLocationLt = new List<int>();  //记录已知点的序号
            //注意：对于该循环，有一个默认条件，即FromCpl的第一个顶点只有一个对应点
            for (int i = 0; i < cptlt.Count; i++)
            {
                if (cptlt[i].isCtrl == true)
                {
                    intKnownLocationLt.Add(i);
                    intKnownNum += 1;
                }
                else
                {
                    intUnknownNum += 1;
                }
            }

            //找出长度固定的位置(如果一个线段的前后两个点都固定，则该长度固定)。另外，长度固定则该边的方位角也固定
            List<int> intKnownLengthLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 1; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1)
                {
                    intKnownLengthLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownLength = pCorrCptsLt.Count - 1 - intKnownLengthLt.Count;

            //找出角度固定的位置(如果一个固定顶点的前后两个点都固定，则该角度固定)
            List<int> intKnownAngleLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 2; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1 && (intKnownLocationLt[i + 2] - intKnownLocationLt[i + 1]) == 1)
                {
                    intKnownAngleLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownAngle = pCorrCptsLt.Count - 2 - intKnownAngleLt.Count;

            //总未知量
            int intUnknownLengthAngle = intUnknownLength + intUnknownAngle;

            //坐标约束个数
            int intXYCst = (intKnownLocationLt.Count - 1 - intKnownLengthLt.Count) * 4; //如果两个相邻点都是控制点，则这两个相邻点之间不存在坐标约束
            //夹角约束个数
            int intAngleCst = intKnownLengthLt.Count - 1 - intKnownAngleLt.Count;  //同坐标约束，如果有两相邻边都为已知边，则之间不存在夹角约束
            //总约束个数
            int intSumCst = intUnknownLengthAngle + intXYCst + intAngleCst;

            //定义权重矩阵
            VBMatrix P = new VBMatrix(intSumCst, intSumCst);
            for (int i = 0; i < intUnknownLength; i++)
            {
                P[i, i] = 1;
            }
            for (int i = 0; i < intUnknownAngle; i++)
            {
                P[intUnknownLength + i, intUnknownLength + i] = 10;
            }
            for (int i = 0; i < intXYCst; i++)
            {
                P[intUnknownLengthAngle + i, intUnknownLengthAngle + i] = 1;
            }
            for (int i = 0; i < intAngleCst; i++)
            {
                P[intUnknownLengthAngle + intXYCst + i, intUnknownLengthAngle + intXYCst + i] = 1;
            }

            //计算初始值矩阵X0（注意：此处的X0中并未包含任何坐标，而是长度和夹角的初值）
            VBMatrix X0 = new VBMatrix(intPtNum * 2 - 3, 1);
            for (int i = 0; i < (intPtNum - 1); i++)
            {
                X0[i, 0] = adblLength0[i];
            }
            for (int i = 0; i < (intPtNum - 2); i++)
            {
                X0[intPtNum - 1 + i, 0] = adblAngle0[i];
            }

            //Xmix里存储了XA和X0的最新混合值（此矩阵在公式推导中并不存在，只是为了方便编写代码而建立）
            VBMatrix Xmix = new VBMatrix(intPtNum * 2 - 3, 1);
            for (int i = 0; i < X0.Row; i++)
            {
                Xmix[i, 0] = X0[i, 0];
            }

            //定义近似值矩阵XA（注意：同上，此处的XA中并未包含任何坐标，而是长度和夹角的近似值）
            VBMatrix XA = new VBMatrix(intUnknownLengthAngle, 1);
            int intSumCountL = 0;
            for (int i = 0; i < intUnknownLength; i++) //长度近似值部分
            {
                if (cptlt[intSumCountL].isCtrl == false || cptlt[intSumCountL + 1].isCtrl == false)
                {
                    XA[i, 0] = X0[intSumCountL, 0];
                }
                else
                {
                    i -= 1;
                }
                intSumCountL += 1;
            }
            int intSumCountA = 0;
            for (int i = intUnknownLength; i < intUnknownLengthAngle; i++) //角度近似值部分
            {
                if (cptlt[intSumCountA].isCtrl == false || cptlt[intSumCountA + 1].isCtrl == false || cptlt[intSumCountA + 2].isCtrl == false)
                {
                    XA[i, 0] = X0[intPtNum - 1 + intSumCountA, 0];
                }
                else
                {
                    i -= 1;
                }
                intSumCountA += 1;
            }

            //定义系数矩阵，系数矩阵来源于四类约束方程：1、长度本身；2、角度本身；3、X、Y的闭合差；4、方位角闭合差
            //此处仅给出“长度本身”和“角度本身”的系数，有关“ X、Y的闭合差”和“方位角闭合差”的系数将在循环中给出
            VBMatrix A = new VBMatrix(intSumCst, intUnknownLengthAngle);
            for (int i = 0; i < intUnknownLengthAngle; i++)
            {
                A[i, i] = 1;
            }

            double dblJudge = 0;   //该值用于判断是否应该跳出循环
            double dblOldJudege = 0;
            int intIterativeCount = 0;

            do
            {
                VBMatrix matl = new VBMatrix(intSumCst, 1);

                //计算matl关于“长度本身”这部分的值
                int intSumCountL1 = 0;
                for (int i = 0; i < intUnknownLength; i++)
                {
                    if (cptlt[intSumCountL1].isCtrl == false || cptlt[intSumCountL1 + 1].isCtrl == false)
                    {
                        matl[i, 0] = XA[i, 0] - X0[intSumCountL1, 0];
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCountL1 += 1;
                }
                //计算matl关于“角度本身”这部分的值
                int intSumCountA1 = 0;
                for (int i = intUnknownLength; i < intUnknownLengthAngle; i++)
                {
                    if (cptlt[intSumCountA1].isCtrl == false || cptlt[intSumCountA1 + 1].isCtrl == false || cptlt[intSumCountA1 + 2].isCtrl == false)
                    {
                        matl[i, 0] = XA[i, 0] - X0[intPtNum - 1 + intSumCountA1, 0];
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCountA1 += 1;
                }

                //计算系数矩阵A第"intUnknownLengthAngle"行到"intUnknownLengthAngle + intXYCst - 1"行的各元素，即从由坐标约束产生的各偏导值
                if (intKnownLocationLt.Count >= 2)
                {
                    int intRow = intUnknownLengthAngle;
                    int intLastIsCtrl = 0;
                    for (int i = 0; i < cptlt.Count; i++)
                    {
                        if (cptlt[i].isCtrl == true)
                        {
                            intLastIsCtrl = i;
                            break;
                        }
                    }
                    int intKnownLength = 0;
                    int intKnownAngle = 0;
                    for (int i = intLastIsCtrl + 1; i < cptlt.Count; i++)
                    {
                        if (cptlt[i].isCtrl == true && cptlt[i - 1].isCtrl != true)
                        {
                            double dblSumDerX = new double();
                            double dblSumDerY = new double();
                            double dblSumDerX2 = new double();
                            double dblSumDerY2 = new double();

                            for (int j = intLastIsCtrl; j < i; j++)  //注意，此处的j不可以等于0，因为等于0的时候该处不存在夹角。当然，由于之前规定前两个固定，因此j>=1
                            {
                                dblSumDerX = 0;
                                dblSumDerY = 0;
                                dblSumDerX2 = 0;
                                dblSumDerY2 = 0;

                                //关于X的约束方程，由于分别可以从左侧和右侧推算，因此有两个约束方程
                                A[intRow, j - intKnownLength] = Math.Cos(adblAzimuth[j]);  //关于长度的偏导值
                                for (int k = j; k < i; k++)
                                {
                                    dblSumDerX -= (adblLength0[k] * Math.Sin(adblAzimuth[k]));  //关于夹角的偏导值
                                }
                                A[intRow, j - 1 - intKnownAngle + intUnknownLength] = dblSumDerX;

                                A[intRow + 1, j - intKnownLength] = A[intRow, j - intKnownLength];  //关于长度的偏导值
                                for (int k = intLastIsCtrl; k <= j; k++)
                                {
                                    dblSumDerX2 += (adblLength0[k] * Math.Sin(adblAzimuth[k]));  //关于夹角的偏导值
                                }
                                A[intRow + 1, j - intKnownAngle + intUnknownLength] = dblSumDerX2;

                                //关于Y的约束方程，行加1，列相同
                                A[intRow + 2, j - intKnownLength] = Math.Sin(adblAzimuth[j]);  //关于长度的偏导值
                                for (int k = j; k < i; k++)
                                {
                                    dblSumDerY += (adblLength0[k] * Math.Cos(adblAzimuth[k]));  //关于夹角的偏导值
                                }
                                A[intRow + 2, j - 1 - intKnownAngle + intUnknownLength] = dblSumDerY;

                                //关于Y的约束方程，行加1，列相同
                                A[intRow + 3, j - intKnownLength] = A[intRow + 2, j - intKnownLength];  //关于长度的偏导值
                                for (int k = intLastIsCtrl; k <= j; k++)
                                {
                                    dblSumDerY2 -= (adblLength0[k] * Math.Cos(adblAzimuth[k]));  //关于夹角的偏导值
                                }
                                A[intRow + 3, j - intKnownAngle + intUnknownLength] = dblSumDerY2;

                                if (j == intLastIsCtrl)
                                {
                                    matl[intRow + 0, 0] = dblSumDerY - (cptlt[i].X - cptlt[intLastIsCtrl].X);   //图方便，顺便计算matl（此处之所以利用之前的成果dblSumDerY，是因为数值上正好相等）
                                    matl[intRow + 2, 0] = -dblSumDerX - (cptlt[i].Y - cptlt[intLastIsCtrl].Y);   //图方便，顺便计算matl（此处之所以利用之前的成果-dblSumDerX，是因为数值上正好相等）
                                }

                                if (j == (i - 1))
                                {
                                    matl[intRow + 1, 0] = -dblSumDerY2 - (cptlt[i].X - cptlt[intLastIsCtrl].X);   //图方便，顺便计算matl（此处之所以利用之前的成果dblSumDerY，是因为数值上正好相等）
                                    matl[intRow + 3, 0] = dblSumDerX2 - (cptlt[i].Y - cptlt[intLastIsCtrl].Y);   //图方便，顺便计算matl（此处之所以利用之前的成果-dblSumDerX，是因为数值上正好相等）
                                }
                            }

                            intRow += 4;
                            intLastIsCtrl = i;
                        }
                        else if (cptlt[i].isCtrl == true && cptlt[i - 1].isCtrl == true)  //相邻两个点的坐标都知道，不产生约束条件
                        {
                            intKnownLength += 1;
                            if (i >= 2)
                            {
                                if (cptlt[i - 2].isCtrl == true)
                                {
                                    intKnownAngle += 1;
                                }
                            }
                            intLastIsCtrl = i;
                        }
                    }
                }

                //计算系数矩阵A第"intUnknownLengthAngle + intXYCst"行到"intUnknownLengthAngle + intXYCst + intAngleCst - 1 (即intSumCst - 1)"行的各元素，即夹角闭合差产生的各偏导值
                if (intKnownLengthLt.Count >= 2)
                {
                    int intRow = intUnknownLengthAngle + intXYCst;
                    int intLastIsCtrl = 0;
                    for (int i = 0; i < cptlt.Count - 1; i++)
                    {
                        if (cptlt[i].isCtrl == true && cptlt[i + 1].isCtrl == true)
                        {
                            intLastIsCtrl = i;
                            break;
                        }
                    }
                    int intKnownAngle = 0;
                    for (int i = intLastIsCtrl + 1; i < cptlt.Count - 1; i++)
                    {
                        if (cptlt[i].isCtrl == true && cptlt[i + 1].isCtrl == true && cptlt[i - 1].isCtrl != true)
                        {
                            double dblAngleSum = 0;
                            for (int j = intLastIsCtrl; j < i; j++)
                            {
                                //关于夹角的约束方程
                                A[intRow, j - intKnownAngle + intUnknownLength] = 1;
                                //夹角累计值，为计算matl做准备
                                dblAngleSum += adblAngle0[j];
                            }

                            matl[intRow, 0] = dblAngleSum - (i - intLastIsCtrl) * Math.PI - (adblAzimuth[i] - adblAzimuth[intLastIsCtrl]);   //图方便，顺便计算matl

                            intRow += 1;
                            intLastIsCtrl = i;
                        }
                        else if (cptlt[i].isCtrl == true && cptlt[i + 1].isCtrl == true && cptlt[i - 1].isCtrl == true)  //相邻三个点的坐标都知道，不产生约束条件
                        {
                            intKnownAngle += 1;
                            intLastIsCtrl = i;
                        }
                    }
                }

                //记录一个值以协助判断是否可以退出循环
                double dblLast = XA[0, 0];

                //计算新的近似值
                //SaveFileDialog SFD = new SaveFileDialog();
                //SFD.ShowDialog();
                //CHelpFuncExcel.ExportDataToExcelA(A, "maxA", _DataRecords.ParameterInitialize.strSavePath);
                //CHelpFuncExcel.ExportDataToExcelP(P, "maxP", _DataRecords.ParameterInitialize.strSavePath);
                //CHelpFuncExcel.ExportDataToExcel2(matl, "maxmatl", _DataRecords.ParameterInitialize.strSavePath);

                //平差
                XA -= _pCAL.InvAtPAAtPmatl(A, P, matl);

                //更新Xmix
                int intSumCountL4 = 0;
                for (int i = 0; i < intUnknownLength; i++) //长度近似值部分
                {
                    if (cptlt[intSumCountL4].isCtrl == false || cptlt[intSumCountL4 + 1].isCtrl == false)
                    {
                        Xmix[intSumCountL4, 0] = XA[i, 0];   //其实此处的“intSumCountL4”等于"intPtNum-1"
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCountL4 += 1;
                }
                int intSumCountA4 = 0;
                for (int i = intUnknownLength; i < intUnknownLengthAngle; i++) //角度近似值部分
                {
                    if (cptlt[intSumCountA4].isCtrl == false || cptlt[intSumCountA4 + 1].isCtrl == false || cptlt[intSumCountA4 + 2].isCtrl == false)
                    {
                        Xmix[intPtNum - 1 + intSumCountA4, 0] = XA[i, 0];   //其实此处的“intSumCountL4”等于"intPtNum-1"
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCountA4 += 1;
                }

                //更新adblAzimuth和cptlt
                for (int i = 2; i < cptlt.Count; i++)
                {
                    if (cptlt[i].isCtrl == false)
                    {
                        adblAzimuth[i - 1] = adblAzimuth[i - 2] + Xmix[intPtNum - 1 + i - 2, 0] - Math.PI;
                        double dblnewX = cptlt[i - 1].X + Xmix[i - 1, 0] * Math.Cos(adblAzimuth[i - 1]);
                        double dblnewY = cptlt[i - 1].Y + Xmix[i - 1, 0] * Math.Sin(adblAzimuth[i - 1]);
                        CPoint newcpt = new CPoint(i, dblnewX, dblnewY);
                        double dbloldX = cptlt[i].X;
                        double dbloldY = cptlt[i].Y;
                        cptlt.RemoveAt(i);
                        cptlt.Insert(i, newcpt);
                    }
                }

                dblJudge = Math.Abs(dblLast - XA[0, 0]);
                intIterativeCount += 1;
                if (intIterativeCount >= 1000)
                {
                    break;
                }
                if (dblOldJudege == dblJudge)
                {
                    break;
                }

                dblOldJudege = dblJudge;
            } while (dblJudge > dblTX);


            //生成目标线段
            CPolyline cpl = new CPolyline(0, cptlt);
            return cpl;
        }
    }
}
