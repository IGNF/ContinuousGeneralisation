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
    /// <remarks>顾及长度和角度，以坐标为平差参数进行平差，并对这三个变量的观测值进行改正，（此为附有参数的条件平差？）</remarks>
    public class CALm
    {
        private CDataRecords _DataRecords;                    //records of data
        private double _dblTX;
        
        

        public CALm()
        {

        }

        public CALm(CDataRecords pDataRecords, double dblTX)
        {
            _DataRecords = pDataRecords;
            _dblTX = dblTX;
        }

        public CALm(CDataRecords pDataRecords)
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
            CHelpFunc.ViewPolyline(m_mapControl, cpl);  //显示生成的线段
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
            List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;   //读取数据后，此处ResultPtLt中的对应点为一一对应
            double dblTX = _dblTX;

            int intPtNum = pCorrCptsLt.Count;
            //计算长度初始值（全部计算）
            double[] adblLength0 = new double[intPtNum - 1];
            double[] adblLength = new double[intPtNum - 1];   //可以变化的值
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                double dblfrsublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].FrCpt, pCorrCptsLt[i].FrCpt);
                double dbltosublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].ToCpt, pCorrCptsLt[i].ToCpt);
                adblLength0[i] = (1 - dblProp) * dblfrsublength + dblProp * dbltosublength;
                adblLength[i] = adblLength0[i];
            }

            //计算角度初始值（全部计算）
            double[] adblAngle0 = new double[intPtNum - 2];
            double[] adblAngle = new double[intPtNum - 2];  //可以变化的值
            for (int i = 0; i < pCorrCptsLt.Count - 2; i++)
            {
                //较大比例尺线状要素上的夹角
                double dblfrAngle = CGeoFunc.CalAngle_Counterclockwise(pCorrCptsLt[i].FrCpt, pCorrCptsLt[i + 1].FrCpt, pCorrCptsLt[i + 2].FrCpt);
                //较小比例尺线状要素上的夹角
                double dbltoAngle = CGeoFunc.CalAngle_Counterclockwise(pCorrCptsLt[i].ToCpt, pCorrCptsLt[i + 1].ToCpt, pCorrCptsLt[i + 2].ToCpt);

                //角度初始值
                adblAngle0[i] = (1 - dblProp) * dblfrAngle + dblProp * dbltoAngle;
                adblAngle[i] = adblAngle0[i];
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
            int intKnownPt = 0;           //固定点的数目
            int intUnknownPt = 0;         //非固定点的数目

            List<int> intKnownLocationLt = new List<int>();  //记录已知点的序号
            //注意：对于该循环，有一个默认条件，即FromCpl的第一个顶点只有一个对应点
            for (int i = 0; i < cptlt.Count; i++)
            {
                if (cptlt[i].isCtrl == true)
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
            int intUnknownLength = cptlt.Count - 1 - intKnownLengthLt.Count;

            //找出角度固定的位置(如果一个固定顶点的前后两个点都固定，则该角度固定)
            List<int> intKnownAngleLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 2; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1 && (intKnownLocationLt[i + 2] - intKnownLocationLt[i + 1]) == 1)
                {
                    intKnownAngleLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownAngle = cptlt.Count - 2 - intKnownAngleLt.Count;

            //总未知量
            int intUnknownLengthAngle = intUnknownLength + intUnknownAngle;

            //定义权重矩阵
            VBMatrix P = new VBMatrix(intUnknownLengthAngle, intUnknownLengthAngle);
            for (int i = 0; i < intUnknownLength; i++)
            {
                P[i, i] = 1;
            }
            for (int i = 0; i < intUnknownAngle; i++)
            {
                P[intUnknownLength + i, intUnknownLength + i] = 1;
            }

            //定义权重矩阵倒数
            VBMatrix QLL = new VBMatrix(intUnknownLengthAngle, intUnknownLengthAngle);
            for (int i = 0; i < intUnknownLength; i++)
            {
                QLL[i, i] = 1;
            }
            for (int i = 0; i < intUnknownAngle; i++)
            {
                QLL[intUnknownLength + i, intUnknownLength + i] = 1;
            }
            //for (int i = 0; i < intXYCst; i++)
            //{
            //    P[intUnknownLengthAngle + i, intUnknownLengthAngle + i] = 1;
            //}
            //for (int i = 0; i < intAngleCst; i++)
            //{
            //    P[intUnknownLengthAngle + intXYCst + i, intUnknownLengthAngle + intXYCst + i] =100000;
            //}

            //计算初始值矩阵X0
            VBMatrix X0 = new VBMatrix(2 * intPtNum, 1);
            for (int i = 0; i < intPtNum; i++)
            {
                X0[2 * i, 0] = cptlt[i].X;
                X0[2 * i + 1, 0] = cptlt[i].Y;
            }
            //Xmix里存储了XA和X0的最新混合值（此矩阵在公式推导中并不存在，只是为了方便编写代码而建立）
            VBMatrix Xmix = new VBMatrix(2 * intPtNum, 1);
            for (int i = 0; i < (2 * intPtNum); i++)
            {
                Xmix[i, 0] = X0[i, 0];
            }

            //定义系数矩阵B(各方程对长度、角度的导数值)
            VBMatrix B = new VBMatrix(intUnknownLengthAngle, intUnknownLengthAngle);
            for (int i = 0; i < intUnknownLengthAngle; i++)
            {
                B[i, i] = -1;
            }

            //定义系数矩阵A(各方程对坐标的导数值)，A的导数值将在循环中给出
            VBMatrix A = new VBMatrix(intUnknownLengthAngle, intUnknownXY);
            double dblJudge1 = 0;   //该值用于判断是否应该跳出循环
            double dblJudge2 = 0;   //该值用于判断是否应该跳出循环
            int intJudgeIndex = intUnknownLength  / 4;
            int intIterativeCount = 0;
            double[] dblSubDis = new double[intPtNum - 1];
            do
            {
                VBMatrix w = new VBMatrix(intUnknownLengthAngle, 1);

                //计算系数矩阵A第0行到"intUnknownLength"行的各元素，即线段长度对各未知数求偏导的值
                //先计算各分母值（注意：分母实际上是求偏导后的一部分值，但却恰好等于两点之间距离，因此其计算公式与距离计算公式相同
                dblSubDis = new double[intPtNum - 1];
                for (int i = 0; i < intPtNum - 1; i++)
                {
                    dblSubDis[i] = Math.Pow((Xmix[2 * i, 0] - Xmix[2 * i + 2, 0]) * (Xmix[2 * i, 0] - Xmix[2 * i + 2, 0]) + (Xmix[2 * i + 1, 0] - Xmix[2 * i + 3, 0]) * (Xmix[2 * i + 1, 0] - Xmix[2 * i + 3, 0]), 0.5);
                }
                //计算新的方位角
                adblAzimuth[0] = CGeoFunc.CalAxisAngle(Xmix[0, 0], Xmix[1, 0], Xmix[2, 0], Xmix[3, 0]);
                for (int i = 1; i < intPtNum - 1; i++)
                {
                    double dblAngle = CGeoFunc.CalAngle_Counterclockwise(Xmix[i * 2 - 2, 0], Xmix[i * 2 - 1, 0], Xmix[i * 2, 0], Xmix[i * 2 + 1, 0], Xmix[i * 2 + 2, 0], Xmix[i * 2 + 3, 0]);
                    adblAzimuth[i] = adblAzimuth[i-1] + dblAngle - Math.PI;
                }

                //开始计算系数矩阵第"intUnknownXY"行到"intUnknownXY+intUnknownLength-1"行的各元素
                int intKnownCount2 = 0;
                int intUnKnownCount2 = 0;
                for (int j = 0; j < intUnknownLength; j++)
                {
                    int intSumCount = intKnownCount2 + intUnKnownCount2;
                    if (cptlt[intSumCount].isCtrl == false && cptlt[intSumCount + 1].isCtrl == false)
                    {
                        A[j, 2 * intUnKnownCount2 + 0] = -Math.Cos(adblAzimuth[intSumCount]);
                        A[j, 2 * intUnKnownCount2 + 1] = -Math.Sin(adblAzimuth[intSumCount]);
                        A[j, 2 * intUnKnownCount2 + 2] = -A[j, 2 * intUnKnownCount2 + 0];
                        A[j, 2 * intUnKnownCount2 + 3] = -A[j, 2 * intUnKnownCount2 + 1];

                        w[j, 0] = dblSubDis[intSumCount] - adblLength[intSumCount];   //图方便，顺便计算matl

                        intUnKnownCount2 += 1;
                    }
                    else if (cptlt[intSumCount].isCtrl == false && cptlt[intSumCount + 1].isCtrl == true)
                    {
                        A[j, 2 * intUnKnownCount2 + 0] = -Math.Cos(adblAzimuth[intSumCount]);
                        A[j, 2 * intUnKnownCount2 + 1] = -Math.Sin(adblAzimuth[intSumCount]);

                        w[j, 0] = dblSubDis[intSumCount] - adblLength[intSumCount];   //图方便，顺便计算matl

                        intUnKnownCount2 += 1;
                    }
                    else if (cptlt[intSumCount].isCtrl == true && cptlt[intSumCount + 1].isCtrl == false)
                    {
                        //注意这种情况，由于"pCorrCptsLt[intSumCount].FrCpt.isCtrl == true"不占位子（即不占列），因此列序号依然为" 2 * intUnKnownCount2 + 0"和" 2 * intUnKnownCount2 + 1"，而不是+2,+3
                        A[j, 2 * intUnKnownCount2 + 0] = Math.Cos(adblAzimuth[intSumCount]);
                        A[j, 2 * intUnKnownCount2 + 1] = Math.Sin(adblAzimuth[intSumCount]);

                        w[j, 0] = dblSubDis[intSumCount] - adblLength[intSumCount];   //图方便，顺便计算matl

                        intKnownCount2 += 1;
                    }
                    else
                    {
                        intKnownCount2 += 1;
                        j -= 1;
                    }
                }

                //计算系数矩阵A第"intUnknownXY+intUnknownLength"行到"intUnknownXY+intUnknownLength+intUnknownAngle"行的各元素，即角度对各未知数求偏导的值
                int intKnownCount3 = 0;
                int intUnKnownCount3 = 0;
                for (int j = 0; j < intUnknownAngle; j++)
                {
                    //真是太幸运了，虽然求两向量逆时针夹角时需分多种情况讨论，但各情况的导数形式却是一致的，节省了不少编程精力啊，哈哈
                    int intSumCount = intKnownCount3 + intUnKnownCount3;

                    //常用数据准备
                    double dblA2 = dblSubDis[intSumCount] * dblSubDis[intSumCount];
                    double dblB2 = dblSubDis[intSumCount + 1] * dblSubDis[intSumCount + 1];

                    //开始计算系数值，由于将以下三个情况排列组合将有八种情况，因此按如下方式计算
                    if (cptlt[intUnKnownCount3 + intKnownCount3].isCtrl == true && cptlt[intUnKnownCount3 + intKnownCount3 + 1].isCtrl == true && cptlt[intUnKnownCount3 + intKnownCount3 + 2].isCtrl == true)
                    {
                        intKnownCount3 += 1;
                        j -= 1;
                    }
                    else
                    {
                        double dblNewAngle = CGeoFunc.CalAngle_Counterclockwise(Xmix[2 * intSumCount + 0, 0], Xmix[2 * intSumCount + 1, 0],
                                                                       Xmix[2 * intSumCount + 2, 0], Xmix[2 * intSumCount + 3, 0],
                                                                       Xmix[2 * intSumCount + 4, 0], Xmix[2 * intSumCount + 5, 0]);
                        w[intUnknownLength + j, 0] = dblNewAngle - adblAngle[intSumCount];   //图方便，顺便计算matl

                        int intPreTrueNum = 0;
                        int intUnKnownCount3orginal = intUnKnownCount3;
                        int intKnownCount3orginal = intKnownCount3;
                        if (cptlt[intUnKnownCount3orginal + intKnownCount3orginal + 0].isCtrl == false)
                        {
                            //X1,Y1的导数值(注意：该部分是减数，因此值为导数的负数)
                            A[intUnknownLength + j, 2 * intUnKnownCount3orginal + 0] = -(Xmix[2 * intSumCount + 3, 0] - Xmix[2 * intSumCount + 1, 0]) / dblA2;
                            A[intUnknownLength + j, 2 * intUnKnownCount3orginal + 1] = (Xmix[2 * intSumCount + 2, 0] - Xmix[2 * intSumCount + 0, 0]) / dblA2;

                            intUnKnownCount3 += 1;
                        }
                        else
                        {
                            intPreTrueNum += 1;
                            intKnownCount3 += 1;
                        }

                        if (cptlt[intUnKnownCount3orginal + intKnownCount3orginal + 1].isCtrl == false)
                        {
                            //X2,Y2的导数值
                            //A[intUnknownLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 2] = -(Xmix[2 * intSumCount + 5, 0] - Xmix[2 * intSumCount + 3, 0]) / dblB2 - (Xmix[2 * intSumCount + 3, 0] - Xmix[2 * intSumCount + 1, 0]) / dblA2;
                            //A[intUnknownLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 3] = (Xmix[2 * intSumCount + 4, 0] - Xmix[2 * intSumCount + 2, 0]) / dblB2 + (Xmix[2 * intSumCount + 2, 0] - Xmix[2 * intSumCount + 0, 0]) / dblA2;

                            A[intUnknownLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 2] = (Xmix[2 * intSumCount + 5, 0] - Xmix[2 * intSumCount + 3, 0]) / dblB2 + (Xmix[2 * intSumCount + 3, 0] - Xmix[2 * intSumCount + 1, 0]) / dblA2;
                            A[intUnknownLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 3] = -(Xmix[2 * intSumCount + 4, 0] - Xmix[2 * intSumCount + 2, 0]) / dblB2 - (Xmix[2 * intSumCount + 2, 0] - Xmix[2 * intSumCount + 0, 0]) / dblA2;
                        }
                        else
                        {
                            intPreTrueNum += 1;
                        }
                        if (cptlt[intUnKnownCount3orginal + intKnownCount3orginal + 2].isCtrl == false)
                        {
                            //X3,Y3的导数值
                            A[intUnknownLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 4] = -(Xmix[2 * intSumCount + 5, 0] - Xmix[2 * intSumCount + 3, 0]) / dblB2;
                            A[intUnknownLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 5] = (Xmix[2 * intSumCount + 4, 0] - Xmix[2 * intSumCount + 2, 0]) / dblB2;
                        }
                    }
                }

                //一个中间值
                VBMatrix InvBQLLBt = new VBMatrix(intUnknownLengthAngle, intUnknownLengthAngle);
                for (int i = 0; i < intUnknownLengthAngle; i++)  //注意：此处之所以可以这样计算是因为B矩阵是元素为-1的对角矩阵，且QLL是对角矩阵
                {
                    InvBQLLBt[i, i] = 1 / (QLL[i, i] * QLL[i, i]);
                }

                //计算Q22
                VBMatrix TempQ22 = A.Trans() * InvBQLLBt * A;
                VBMatrix NegQ22 = TempQ22.Inv(TempQ22);
                VBMatrix Q22 = new VBMatrix(intUnknownXY, intUnknownXY);
                for (int i = 0; i < intUnknownXY; i++)
                {
                    for (int j = 0; j < intUnknownXY; j++)
                    {
                        Q22[i, j] = -NegQ22[i, j];
                    }
                }

                //计算Q12
                VBMatrix NegQ12 = InvBQLLBt * A * Q22;
                VBMatrix Q12 = new VBMatrix(intUnknownLengthAngle, intUnknownXY);
                for (int i = 0; i < intUnknownLengthAngle; i++)
                {
                    for (int j = 0; j < intUnknownXY; j++)
                    {
                        Q12[i, j] = -NegQ12[i, j];
                    }
                }

                //计算Q21
                VBMatrix Q21 = Q12.Trans();

                //计算Q11********************************************************************
                VBMatrix I = new VBMatrix(intUnknownLengthAngle, intUnknownLengthAngle);
                for (int i = 0; i < intUnknownLengthAngle; i++)
                {
                    I[i, i] = 1;
                }
                VBMatrix  Q11= InvBQLLBt*(I-A*Q21 );                
             
                //计算负v
                VBMatrix Negv = QLL * B.Trans() * Q11 * w;

                //计算负x
                VBMatrix Negx = Q21 * w;

                //对观测值进行改正
                int intSumCountL4 = 0;
                for (int i = 0; i < intUnknownLength; i++) //长度近似值部分
                {
                    if (cptlt[intSumCountL4].isCtrl == false || cptlt[intSumCountL4 + 1].isCtrl == false)
                    {
                        adblLength[intSumCountL4] = adblLength[intSumCountL4] - Negv[i, 0];
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
                        adblAngle[intSumCountA4] = adblAngle[intSumCountA4] - Negv[i, 0];
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCountA4 += 1;
                }

                //对坐标值进行改正
                int intSumCount5 = 0;
                for (int i = 0; i < intUnknownPt; i++)
                {
                    if (cptlt[intSumCount5].isCtrl == false)
                    {
                        Xmix[intSumCount5 * 2, 0] = Xmix[intSumCount5 * 2, 0] - Negx[i * 2, 0];
                        Xmix[intSumCount5 * 2 + 1, 0] = Xmix[intSumCount5 * 2 + 1, 0] - Negx[i * 2 + 1, 0];
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


                if (intIterativeCount >= 1000)
                {
                    break;
                }


                //这里只是随便取两个中间值以观测是否收敛

                dblJudge1 = Math.Abs(Negx[intJudgeIndex, 0]);
                dblJudge2 = Math.Abs(Negx[3 * intJudgeIndex, 0]);
            } while ((dblJudge1 > dblTX) || (dblJudge2 > dblTX));


            //生成目标线段
            List<CPoint> CTargetPtLt = new List<CPoint>();
            for (int i = 0; i < intPtNum; i++)
            {
                CPoint cpt = new CPoint(i);
                cpt.X = Xmix[2 * i, 0];
                cpt.Y = Xmix[2 * i + 1, 0];
                CTargetPtLt.Add(cpt);
            }
            CPolyline cpl = new CPolyline(0, CTargetPtLt);
            return cpl;

        }


    }
}
