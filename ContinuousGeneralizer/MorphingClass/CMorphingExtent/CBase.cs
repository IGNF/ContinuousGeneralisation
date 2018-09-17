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

namespace MorphingClass.CMorphingExtend
{
    /// <summary>
    ///
    /// </summary>
    /// <remarks>
    /// </remarks>
    public class CBase
    {
        private CDataRecords _DataRecords;                    //records of data

        
        

        public CBase()
        {

        }

        public CBase(CDataRecords pDataRecords)
        {
            _DataRecords = pDataRecords;
        }


        /// <summary>
        /// 显示并返回单个插值面状要素
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProportion">差值参数</param>
        /// <returns>面状要素</returns>
        public CPolyline DisplayInterpolation(double dblProportion)
        {
            if (dblProportion < 0 || dblProportion > 1)
            {
                MessageBox.Show("请输入正确参数！");
                return null;
            }

            CPolyline cpl = GetTargetcpl(dblProportion);

            // 清除绘画痕迹
            IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            m_mapControl.ActiveView.Refresh();
            CHelpFunc.ViewPolyline(m_mapControl, cpl);  //显示生成的线段
            return cpl;
        }

        /// <summary>
        /// 获取线状要素
        /// </summary>
        /// <param name="pDataRecords">数据记录</param>
        /// <param name="dblProportion">插值参数</param>
        /// <returns>在处理面状要素时，本程序将原面状要素的边界切开，按线状要素处理，处理完后再重新生成面状要素</returns>
        public CPolyline GetTargetcpl(double dblProportion)
        {
            CParameterResult pParameterResult = _DataRecords.ParameterResult;

            //获取周长
            double dblFrLength = pParameterResult.FromCpl.pPolyline.Length;
            double dblToLength = pParameterResult.ToCpl.pPolyline.Length;


            //确定循环阈值（初始多边形平均边长的千分之一）
            double dblTX = dblFrLength / pParameterResult.FromCpl.CptLt .Count  / 1000;

            List<CCorrCpts> pCorrCptsLt = pParameterResult.CCorrCptsLt;   //读取数据后，此处ResultPtLt中的对应点为一一对应
            //pCorrCptsLt[2].FrCpt.isCtrl = false;
            //pCorrCptsLt[2].CorrespondingPtLt[0].FrCpt.isCtrl = false;

            //统计插值点数
            int intKnownNum = 0;           //须固定不参与平差的点的数目
            int intUnknownNum = 0;         //须参与平差的点的数目

            List<int> intKnownLocationLt = new List<int>();  //记录已知点的序号
            //注意：对于该循环，有一个默认条件，即FromCpl的第一个顶点只有一个对应点
            for (int i = 0; i < pCorrCptsLt.Count; i++)
            {
                if (pCorrCptsLt[i].FrCpt.isCtrl == true)
                {
                    intKnownLocationLt.Add(i);
                    intKnownNum += 1;
                }
                else
                {
                    intUnknownNum += 1;
                }
            }
            int intSumNum = intKnownNum + intUnknownNum;
            int intUnknownXY = intUnknownNum * 2;   //每个点都有X、Y坐标
            int intSumXY = intSumNum * 2;   //每个点都有X、Y坐标

            //找出长度固定的位置(如果一个线段的前后两个点都固定，则该长度固定)
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
            for (int i = 1; i < intKnownLocationLt.Count - 1; i++)
            {
                if ((intKnownLocationLt[i] - intKnownLocationLt[i - 1]) == 1 && (intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1)
                {
                    intKnownAngleLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownAngle = pCorrCptsLt.Count - 2 - intKnownAngleLt.Count;

            int intUnknownXYLengthAngle = intUnknownXY + intUnknownLength + intUnknownAngle;

            //定义权重矩阵
            VBMatrix P = new VBMatrix(intUnknownXYLengthAngle, intUnknownXYLengthAngle);
            for (int i = 0; i < intUnknownXY; i++)
            {
                P[i, i] = 1;
            }
            for (int i = 0; i < intUnknownLength; i++)
            {
                P[intUnknownXY + i, intUnknownXY + i] = 10;
            }
            for (int i = 0; i < intUnknownAngle; i++)
            {
                P[intUnknownXY + intUnknownLength + i, intUnknownXY + intUnknownLength + i] = 100;
            }

            //计算初始值矩阵X0
            VBMatrix X0 = new VBMatrix(intSumXY, 1);
            int intCount = 0;
            for (int i = 0; i < pCorrCptsLt.Count; i++)
            {
                X0[intCount, 0] = (1 - dblProportion) * pCorrCptsLt[i].FrCpt.X + dblProportion * pCorrCptsLt[i].ToCpt.X;
                X0[intCount + 1, 0] = (1 - dblProportion) * pCorrCptsLt[i].FrCpt.Y + dblProportion * pCorrCptsLt[i].ToCpt.Y;
                intCount += 2;
            }

            //计算长度初始值（全部计算）
            double[] adblLength0 = new double[intSumNum - 1];
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                double dblfrsublength = Math.Pow((pCorrCptsLt[i + 1].ToCpt.X - pCorrCptsLt[i].ToCpt.X) * (pCorrCptsLt[i + 1].ToCpt.X - pCorrCptsLt[i].ToCpt.X)
                                               + (pCorrCptsLt[i + 1].ToCpt.Y - pCorrCptsLt[i].ToCpt.Y) * (pCorrCptsLt[i + 1].ToCpt.Y - pCorrCptsLt[i].ToCpt.Y), 0.5);

                double dbltosublength = Math.Pow((pCorrCptsLt[i + 1].ToCpt.X - pCorrCptsLt[i].ToCpt.X) * (pCorrCptsLt[i + 1].ToCpt.X - pCorrCptsLt[i].ToCpt.X)
                                               + (pCorrCptsLt[i + 1].ToCpt.Y - pCorrCptsLt[i].ToCpt.Y) * (pCorrCptsLt[i + 1].ToCpt.Y - pCorrCptsLt[i].ToCpt.Y), 0.5);
                adblLength0[i] = (1 - dblProportion) * dblfrsublength + dblProportion * dbltosublength;
            }

            //计算角度初始值（全部计算）
            double[] adblAngle0 = new double[intSumNum - 2];
            for (int i = 0; i < pCorrCptsLt.Count - 2; i++)
            {
                //较大比例尺线状要素上的夹角
                double dblfrAngle = CGeoFunc.CalAngle_Counterclockwise(pCorrCptsLt[i].FrCpt, pCorrCptsLt[i + 1].FrCpt, pCorrCptsLt[i + 2].FrCpt);
                //较小比例尺线状要素上的夹角
                double dbltoAngle = CGeoFunc.CalAngle_Counterclockwise(pCorrCptsLt[i].ToCpt, pCorrCptsLt[i + 1].ToCpt, pCorrCptsLt[i + 2].ToCpt);

                //角度初始值
                adblAngle0[i] = (1 - dblProportion) * dblfrAngle + dblProportion * dbltoAngle;
            }

            //Xmix里存储了XA和X0的最新混合值
            VBMatrix Xmix = new VBMatrix(intSumXY, 1);
            for (int i = 0; i < X0.Row; i++)
            {
                Xmix[i, 0] = X0[i, 0];
            }

            //定义坐标近似值矩阵XA
            VBMatrix XA = new VBMatrix(intUnknownXY, 1);
            int intKnownCount = 0;
            for (int i = 0; i < intUnknownNum; i++)
            {
                if ((i + intKnownCount) != intKnownLocationLt[intKnownCount])  //当前遍历位置“i + intKnownCount”是否在intKnownLocationLt的位置“intKnownCount”中记录
                {
                    XA[i * 2, 0] = X0[(i + intKnownCount) * 2, 0];
                    XA[i * 2 + 1, 0] = X0[(i + intKnownCount) * 2 + 1, 0];
                }
                else
                {
                    intKnownCount += 1;
                    i -= 1;
                }
            }

            //定义系数矩阵（有关长度和角度的值将在循环中给定）
            VBMatrix A = new VBMatrix(intUnknownXYLengthAngle, intUnknownXY);
            for (int i = 0; i < intUnknownXY; i++)
            {
                A[i, i] = 1;
            }

            double dblJudge = 0;   //该值用于判断是否应该跳出循环
            double dblOldJudege = 0;
            int intIterativeCount = 0;
            double[] dblSubDis = new double[intSumNum - 1];
            VBMatrix matl = new VBMatrix(intUnknownXYLengthAngle, 1);
            do
            {
                matl = new VBMatrix(intUnknownXYLengthAngle, 1);
                int intSumCount1 = 0;
                for (int i = 0; i < intUnknownNum; i++)
                {
                    if (pCorrCptsLt[intSumCount1].FrCpt.isCtrl == false)
                    {
                        matl[2 * i, 0] = XA[2 * i, 0] - X0[intSumCount1 * 2, 0];
                        matl[2 * i + 1, 0] = XA[2 * i + 1, 0] - X0[intSumCount1 * 2 + 1, 0];
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCount1 += 1;
                }

                //计算系数矩阵A第"intUnknownXY"行到"intUnknownXY+intUnknownLength-1"行的各元素，即线段长度对各未知数求偏导的值
                //先计算各分母值（注意：分母实际上是求偏导后的一部分值，但却恰好等于两点之间距离，因此其计算公式与距离计算公式相同
                dblSubDis = new double[intSumNum - 1];
                for (int i = 0; i < intSumNum - 1; i++)
                {
                    dblSubDis[i] = Math.Pow((Xmix[2 * i, 0] - Xmix[2 * i + 2, 0]) * (Xmix[2 * i, 0] - Xmix[2 * i + 2, 0]) + (Xmix[2 * i + 1, 0] - Xmix[2 * i + 3, 0]) * (Xmix[2 * i + 1, 0] - Xmix[2 * i + 3, 0]), 0.5);
                }
                //开始计算系数矩阵第"intUnknownXY"行到"intUnknownXY+intUnknownLength-1"行的各元素
                int intKnownCount2 = 0;
                int intUnKnownCount2 = 0;
                for (int j = 0; j < intUnknownLength; j++)
                {
                    try
                    {
                        int intSumCount = intKnownCount2 + intUnKnownCount2;
                        if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == false && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == false)
                        {
                            A[intUnknownXY + j, 2 * intUnKnownCount2 + 0] = (Xmix[2 * intSumCount, 0] - Xmix[2 * intSumCount + 2, 0]) / dblSubDis[intSumCount];
                            A[intUnknownXY + j, 2 * intUnKnownCount2 + 1] = (Xmix[2 * intSumCount + 1, 0] - Xmix[2 * intSumCount + 3, 0]) / dblSubDis[intSumCount];
                            A[intUnknownXY + j, 2 * intUnKnownCount2 + 2] = -A[intUnknownXY + j, 2 * intUnKnownCount2];
                            A[intUnknownXY + j, 2 * intUnKnownCount2 + 3] = -A[intUnknownXY + j, 2 * intUnKnownCount2 + 1];

                            matl[intUnknownXY + j, 0] = dblSubDis[intSumCount] - adblLength0[intSumCount];   //图方便，顺便计算matl

                            intUnKnownCount2 += 1;
                        }
                        else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == false && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == true)
                        {
                            A[intUnknownXY + j, 2 * intUnKnownCount2 + 0] = (Xmix[2 * intSumCount, 0] - Xmix[2 * intSumCount + 2, 0]) / dblSubDis[intSumCount];
                            A[intUnknownXY + j, 2 * intUnKnownCount2 + 1] = (Xmix[2 * intSumCount + 1, 0] - Xmix[2 * intSumCount + 3, 0]) / dblSubDis[intSumCount];

                            matl[intUnknownXY + j, 0] = dblSubDis[intSumCount] - adblLength0[intSumCount];   //图方便，顺便计算matl

                            intUnKnownCount2 += 1;
                        }
                        else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == false)
                        {
                            //注意这种情况，由于"pCorrCptsLt[intSumCount].FrCpt.isCtrl == true"不占位子（即不占列），因此列序号依然为" 2 * intUnKnownCount2 + 0"和" 2 * intUnKnownCount2 + 1"，而不是+2,+3
                            A[intUnknownXY + j, 2 * intUnKnownCount2 + 0] = -(Xmix[2 * intSumCount, 0] - Xmix[2 * intSumCount + 2, 0]) / dblSubDis[intSumCount];
                            A[intUnknownXY + j, 2 * intUnKnownCount2 + 1] = -(Xmix[2 * intSumCount + 1, 0] - Xmix[2 * intSumCount + 3, 0]) / dblSubDis[intSumCount];

                            matl[intUnknownXY + j, 0] = dblSubDis[intSumCount] - adblLength0[intSumCount];   //图方便，顺便计算matl

                            intKnownCount2 += 1;
                        }
                        else
                        {
                            intKnownCount2 += 1;
                            j -= 1;
                        }
                    }
                    catch (Exception e)
                    {

                        throw;
                    }

                }

                //计算系数矩阵A第"intUnknownXY+intUnknownLength"行到"intUnknownXY+intUnknownLength+intUnknownAngle"行的各元素，即角度对各未知数求偏导的值
                int intKnownCount3 = 0;
                int intUnKnownCount3 = 0;
                int intUnKnownXYLength = intUnknownXY + intUnknownLength;
                for (int j = 0; j < intUnknownAngle; j++)
                {
                    //真是太幸运了，虽然求两向量逆时针夹角时需分多种情况讨论，但各情况的导数形式却是一致的，节省了不少编程精力啊，哈哈
                    int intSumCount = intKnownCount3 + intUnKnownCount3;

                    //常用数据准备
                    double dblA2 = dblSubDis[intSumCount] * dblSubDis[intSumCount];
                    double dblB2 = dblSubDis[intSumCount + 1] * dblSubDis[intSumCount + 1];

                    //开始计算系数值，由于将以下三个情况排列组合将有八种情况，因此按如下方式计算
                    if (pCorrCptsLt[intUnKnownCount3 + intKnownCount3].FrCpt.isCtrl == true && pCorrCptsLt[intUnKnownCount3 + intKnownCount3 + 1].FrCpt.isCtrl == true && pCorrCptsLt[intUnKnownCount3 + intKnownCount3 + 2].FrCpt.isCtrl == true)
                    {
                        intKnownCount3 += 1;
                        j -= 1;
                    }
                    else
                    {
                        double dblNewAngle = CGeoFunc.CalAngle_Counterclockwise(Xmix[2 * intSumCount + 0, 0], Xmix[2 * intSumCount + 1, 0],
                                                                       Xmix[2 * intSumCount + 2, 0], Xmix[2 * intSumCount + 3, 0],
                                                                       Xmix[2 * intSumCount + 4, 0], Xmix[2 * intSumCount + 5, 0]);
                        matl[intUnKnownXYLength + j, 0] = dblNewAngle - adblAngle0[intSumCount];   //图方便，顺便计算matl

                        int intPreTrueNum = 0;
                        int intUnKnownCount3orginal = intUnKnownCount3;
                        int intKnownCount3orginal = intKnownCount3;
                        if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3 + 0].FrCpt.isCtrl == false)
                        {
                            A[intUnKnownXYLength + j, 2 * intUnKnownCount3orginal + 0] = -(Xmix[2 * intSumCount + 1, 0] - Xmix[2 * intSumCount + 3, 0]) / dblA2;
                            A[intUnKnownXYLength + j, 2 * intUnKnownCount3orginal + 1] = (Xmix[2 * intSumCount + 0, 0] - Xmix[2 * intSumCount + 2, 0]) / dblA2;

                            intUnKnownCount3 += 1;
                        }
                        else
                        {
                            intPreTrueNum += 1;
                            intKnownCount3 += 1;
                        }

                        if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 1].FrCpt.isCtrl == false)  //注意：对中间点的X、Y求导时，其导数由两部分组成，且为第二部分和第一部分的差
                        {
                            A[intUnKnownXYLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 2] = (Xmix[2 * intSumCount + 5, 0] - Xmix[2 * intSumCount + 3, 0]) / dblB2 - (Xmix[2 * intSumCount + 1, 0] - Xmix[2 * intSumCount + 3, 0]) / dblA2;
                            A[intUnKnownXYLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 3] = -(Xmix[2 * intSumCount + 4, 0] - Xmix[2 * intSumCount + 2, 0]) / dblB2 + (Xmix[2 * intSumCount + 0, 0] - Xmix[2 * intSumCount + 2, 0]) / dblA2;
                        }
                        else
                        {
                            intPreTrueNum += 1;
                        }
                        if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 2].FrCpt.isCtrl == false)
                        {

                            A[intUnKnownXYLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 4] = -(Xmix[2 * intSumCount + 5, 0] - Xmix[2 * intSumCount + 3, 0]) / dblB2;
                            A[intUnKnownXYLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 5] = (Xmix[2 * intSumCount + 4, 0] - Xmix[2 * intSumCount + 2, 0]) / dblB2;
                        }
                    }
                    #region 用余弦值求夹角，无法判定角度方向，效果不好
                    //int intSumCount = intKnownCount3 + intUnKnownCount3;

                    ////常用数据准备
                    //double dblA2 = dblSubDis[intSumCount] * dblSubDis[intSumCount];
                    //double dblB2 = dblSubDis[intSumCount + 1] * dblSubDis[intSumCount + 1];
                    //double dblAB = dblSubDis[intSumCount] * dblSubDis[intSumCount + 1];
                    //double dblC2 = (Xmix[2 * intSumCount, 0] - Xmix[2 * intSumCount + 4, 0]) * (Xmix[2 * intSumCount, 0] - Xmix[2 * intSumCount + 4, 0])
                    //             + (Xmix[2 * intSumCount + 1, 0] - Xmix[2 * intSumCount + 5, 0]) * (Xmix[2 * intSumCount + 1, 0] - Xmix[2 * intSumCount + 5, 0]);
                    //double dblA2B2mC2 = dblA2 + dblB2 - dblC2;
                    //double dblpart = 1 / Math.Sqrt(4 * dblA2 * dblB2 - dblA2B2mC2 * dblA2B2mC2);

                    ////开始计算系数值，由于将以下三个情况排列组合将有八种情况，因此按如下方式计算
                    //if (pCorrCptsLt[intUnKnownCount3 + intKnownCount3].FrCpt.isCtrl == true && pCorrCptsLt[intUnKnownCount3 + intKnownCount3 + 1].FrCpt.isCtrl == true && pCorrCptsLt[intUnKnownCount3 + intKnownCount3 + 2].FrCpt.isCtrl == true)
                    //{
                    //    intKnownCount3 += 1;
                    //    j -= 1;
                    //}
                    //else
                    //{
                    //    matl[intUnKnownXYLength + j, 0] = Math.Acos(dblA2B2mC2 / 2 / dblAB) - adblAngle0[intSumCount];   //图方便，顺便计算matl

                    //    int intPreTrueNum = 0;
                    //    int intUnKnownCount3orginal = intUnKnownCount3;
                    //    int intKnownCount3orginal = intKnownCount3;
                    //    if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3 + 0].FrCpt.isCtrl == false)
                    //    {
                    //        A[intUnKnownXYLength + j, 2 * intUnKnownCount3orginal + 0] = dblpart * (2 * dblA2 * (Xmix[2 * intSumCount + 2, 0] - Xmix[2 * intSumCount + 4, 0]) + dblA2B2mC2 * (Xmix[2 * intSumCount + 0, 0] - Xmix[2 * intSumCount + 2, 0])) / dblA2;
                    //        A[intUnKnownXYLength + j, 2 * intUnKnownCount3orginal + 1] = dblpart * (2 * dblA2 * (Xmix[2 * intSumCount + 3, 0] - Xmix[2 * intSumCount + 5, 0]) + dblA2B2mC2 * (Xmix[2 * intSumCount + 1, 0] - Xmix[2 * intSumCount + 3, 0])) / dblA2;
                    //        intUnKnownCount3 += 1;
                    //    }
                    //    else
                    //    {
                    //        intPreTrueNum += 1;
                    //        intKnownCount3 += 1;
                    //    }

                    //    if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 1].FrCpt.isCtrl == false)
                    //    {
                    //        A[intUnKnownXYLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 2] = dblpart * (2 * dblAB * (Xmix[2 * intSumCount + 4, 0] + Xmix[2 * intSumCount + 0, 0] - 2 * Xmix[2 * intSumCount + 2, 0]) + dblA2B2mC2 * (dblB2 * (Xmix[2 * intSumCount + 2, 0] - Xmix[2 * intSumCount + 0, 0]) + dblA2 * (Xmix[2 * intSumCount + 2, 0] - Xmix[2 * intSumCount + 4, 0]))) / dblAB;
                    //        A[intUnKnownXYLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 3] = dblpart * (2 * dblAB * (Xmix[2 * intSumCount + 5, 0] + Xmix[2 * intSumCount + 1, 0] - 2 * Xmix[2 * intSumCount + 3, 0]) + dblA2B2mC2 * (dblB2 * (Xmix[2 * intSumCount + 3, 0] - Xmix[2 * intSumCount + 1, 0]) + dblA2 * (Xmix[2 * intSumCount + 3, 0] - Xmix[2 * intSumCount + 5, 0]))) / dblAB;
                    //    }
                    //    else
                    //    {
                    //        intPreTrueNum += 1;
                    //    }
                    //    if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 2].FrCpt.isCtrl == false)
                    //    {
                    //        A[intUnKnownXYLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 4] = dblpart * (2 * dblB2 * (Xmix[2 * intSumCount + 2, 0] - Xmix[2 * intSumCount + 0, 0]) + dblA2B2mC2 * (Xmix[2 * intSumCount + 4, 0] - Xmix[2 * intSumCount + 2, 0])) / dblB2;
                    //        A[intUnKnownXYLength + j, 2 * (intUnKnownCount3orginal - intPreTrueNum) + 5] = dblpart * (2 * dblA2 * (Xmix[2 * intSumCount + 3, 0] - Xmix[2 * intSumCount + 1, 0]) + dblA2B2mC2 * (Xmix[2 * intSumCount + 5, 0] - Xmix[2 * intSumCount + 3, 0])) / dblB2;
                    //    }
                    //}
                    #endregion
                }





                //记录一个值以协助判断是否可以退出循环
                double dblLast = XA[0, 0];

                ////计算新的近似值
                //SaveFileDialog SFD = new SaveFileDialog();
                //SFD.ShowDialog();
                //     CHelpFuncExcel.ExportDataToExcelA(A, "maxA", SFD.FileName);
                //CHelpFuncExcel.ExportDataToExcelP(P, "maxP", SFD.FileName);
                //CHelpFuncExcel.ExportDataToExcel2(matl, "maxmatl", SFD.FileName);





                //VBMatrix Temp =A.Trans ()  * P * A;
                //Temp.InvertGaussJordan();
                //XA -= Temp * A.Transpose() * P * matl;

                XA -= InvAtPAAtPmatl(A, P, matl);

                //更新Xmix和matl
                int intKnownCount4 = 0;
                for (int i = 0; i < intUnknownNum; i++)
                {
                    if ((i + intKnownCount4) != intKnownLocationLt[intKnownCount4])
                    {
                        Xmix[(i + intKnownCount4) * 2, 0] = XA[i * 2, 0];
                        Xmix[(i + intKnownCount4) * 2 + 1, 0] = XA[i * 2 + 1, 0];
                    }
                    else
                    {
                        intKnownCount4 += 1;
                        i -= 1;
                    }
                }

                dblJudge = Math.Abs(dblLast - XA[0, 0]);
                intIterativeCount += 1;
                if (intIterativeCount >= 10000)
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
            List<CPoint> CTargetPtLt = new List<CPoint>();
            for (int i = 0; i < intSumNum; i++)
            {
                CPoint cpt = new CPoint(i);
                cpt.X = Xmix[2 * i, 0];
                cpt.Y = Xmix[2 * i + 1, 0];
                CTargetPtLt.Add(cpt);
            }
            CPolyline cpl = new CPolyline(0, CTargetPtLt);
            return cpl;
        }




        /// <summary>
        /// 计算Inv(A'*P*A)*A'*P*matl，时间复杂度（2n）^2，n为顶点个数
        /// </summary>
        /// <param name="A">系数矩阵：该矩阵从第0行到第i行为对角矩阵，i行之后为全矩阵</param>
        /// <param name="P">权重矩阵：对角矩阵</param>
        /// <param name="matl">初值矩阵：列向量</param>
        /// <returns>鉴于本实验矩阵的特殊性，可以使用该函数进行计算。</returns>
        public VBMatrix InvAtPAAtPmatl(VBMatrix A, VBMatrix P, VBMatrix matl)
        {
            int intRowA = A.Row;
            int intColA = A.Col;
            int intDiffRowColA = intRowA - intColA;

            //计算A'*P
            VBMatrix MatrixAtP = new VBMatrix(intColA, intRowA);
            for (int i = 0; i < intColA; i++)
            {
                //第i行i列的值
                MatrixAtP[i, i] = A[i, i] * P[i, i];
                //第i行最后intDiffRowColA列的值
                for (int j = intColA; j < intRowA; j++)
                {
                    MatrixAtP[i, j] = A[j, i] * P[j, j];
                }
            }

            //计算A'*P*A
            VBMatrix MatrixAtPA = new VBMatrix(intColA, intColA);
            for (int i = 0; i < intColA; i++)
            {
                //第i行i列的值(即对角线上的值)
                double dblValue = MatrixAtP[i, i] * A[i, i];
                for (int j = intColA; j < intRowA; j++)
                {
                    dblValue += MatrixAtP[i, j] * A[j, i];
                }
                MatrixAtPA[i, i] = dblValue;

                //第i行j列和j行i列的值(注意：MatrixAtPA[i, j] == MatrixAtPA[j, i])
                for (int j = i + 1; j < intColA; j++)
                {
                    for (int k = 0; k < intDiffRowColA; k++)
                    {
                        MatrixAtPA[i, j] += MatrixAtP[i, intColA + k] * A[intColA + k, j];
                    }
                    MatrixAtPA[j, i] = MatrixAtPA[i, j];
                }
            }

            //计算A'*P*matl
            VBMatrix MatrixAtPmatl = new VBMatrix(intColA, 1);
            for (int i = 0; i < intColA; i++)
            {
                double dblValue = MatrixAtP[i, i] * matl[i, 0];
                for (int j = intColA; j < intRowA; j++)
                {
                    dblValue += MatrixAtP[i, j] * matl[j, 0];
                }
                MatrixAtPmatl[i, 0] = dblValue;
            }

            //计算Inv(A'*P*A)*A'*P*matl
            MatrixAtPA.Inv(MatrixAtPA);
            VBMatrix MatrixResult = MatrixAtPA * MatrixAtPmatl;

            return MatrixResult;
        }


        #region 关于面状要素的代码
        ///// <summary>
        ///// 获取面状要素
        ///// </summary>
        ///// <param name="pDataRecords">数据记录</param>
        ///// <param name="dblProportion">差值参数</param>
        ///// <returns>在处理面状要素时，本程序将原面状要素的边界切开，按线状要素处理，处理完后再重新生成面状要素</returns>
        //public CPolyline CGeoFunc.GetTargetcpl(double dblProportion)
        //{
        //    CParameterResult pParameterResult = _DataRecords.ParameterResult;

        //    //获取周长
        //    double dblFrLength = pParameterResult.FromCpl.pPolyline.Length;
        //    double dblToLength = pParameterResult.ToCpl.pPolyline.Length;

        //    //确定循环阈值（初始多边形平均边长的千分之一）
        //    double dblTX = dblFrLength / pParameterResult.FromCpl.CptLt .Count  / 1000;

        //    ////获取面积
        //    //double dblFrArea = 0;
        //    //List<CPoint> frcptlt = pParameterResult.FromCpl.CptLt;
        //    //for (int i = 0; i < frcptlt.Count - 1; i++)
        //    //{
        //    //    dblFrArea += -frcptlt[i].X * frcptlt[i + 1].Y + frcptlt[i + 1].X * frcptlt[i].Y;
        //    //}
        //    //dblFrArea += -frcptlt[frcptlt.Count - 1].X * frcptlt[0].Y + frcptlt[0].X * frcptlt[frcptlt.Count - 1].Y;

        //    //double dblToArea = 0;
        //    //List<CPoint> tocptlt = pParameterResult.ToCpl.CptLt;
        //    //for (int i = 0; i < tocptlt.Count - 1; i++)
        //    //{
        //    //    dblToArea += -tocptlt[i].X * tocptlt[i + 1].Y + tocptlt[i + 1].X * tocptlt[i].Y;
        //    //}
        //    //dblToArea += -tocptlt[tocptlt.Count - 1].X * tocptlt[0].Y + tocptlt[0].X * tocptlt[tocptlt.Count - 1].Y;

        //    //List<CPoint> pCorrCptsLt = pParameterResult.pCorrCptsLt;
        //    //由于第一对对应点与最后一对对应点重合，删除最后一对对应点
        //    //if (pCorrCptsLt[pCorrCptsLt.Count -1].CorrespondingPtLt .Count ==1)
        //    //{
        //    //    pCorrCptsLt.RemoveAt(pCorrCptsLt.Count - 1);
        //    //}
        //    //else if (pCorrCptsLt[pCorrCptsLt.Count -1].CorrespondingPtLt .Count >1)
        //    //{
        //    //    pCorrCptsLt[pCorrCptsLt.Count - 1].CorrespondingPtLt.RemoveAt(pCorrCptsLt[pCorrCptsLt.Count - 1].CorrespondingPtLt.Count - 1);
        //    //}

        //    List<CPoint> pCorrCptsLt = pParameterResult.pCorrCptsLt;
        //    //统计插值点数
        //    int intInsertNum = 0;
        //    for (int i = 0; i < pCorrCptsLt.Count; i++)
        //    {
        //        for (int j = 0; j < pCorrCptsLt[i].FrCpt.CorrespondingPtLt.Count; j++)
        //        {
        //            if (pCorrCptsLt[i].FrCpt.CorrespondingPtLt[pCorrCptsLt[i].FrCpt.CorrespondingPtLt.Count - 1].FrCpt.isCtrl == true)
        //            {
        //                intInsertNum += pCorrCptsLt[i].FrCpt.CorrespondingPtLt.Count - 1;
        //            }
        //            else
        //            {
        //                intInsertNum += pCorrCptsLt[i].FrCpt.CorrespondingPtLt.Count;
        //            }
        //        }
        //    }
        //    int intCordinateNum = intInsertNum * 2;   //每个点都有X、Y坐标

        //    //定义权重矩阵（顾及周长因此+1）
        //    VBMatrix P = new VBMatrix(intCordinateNum + 1, intCordinateNum + 1);
        //    for (int i = 0; i < intCordinateNum; i++)
        //    {
        //        P[i, i] = 1;
        //    }
        //    P[intCordinateNum, intCordinateNum] = 100;  //周长权重
        //    //P[intCordinateNum + 1, intCordinateNum + 1] = 100;  //面积权重

        //    //计算初始值矩阵X0（包括周长）
        //    VBMatrix X0 = new VBMatrix(intCordinateNum + 1, 1);
        //    int intCount = 0;
        //    for (int i = 0; i < pCorrCptsLt.Count; i++)
        //    {
        //        for (int j = 0; j < pCorrCptsLt[i].FrCpt.CorrespondingPtLt.Count; j++)
        //        {
        //            X0[intCount, 0] = (1 - dblProportion) * pCorrCptsLt[i].FrCpt.X + dblProportion * pCorrCptsLt[i].FrCpt.CorrespondingPtLt[j].X;
        //            X0[intCount + 1, 0] = (1 - dblProportion) * pCorrCptsLt[i].FrCpt.Y + dblProportion * pCorrCptsLt[i].FrCpt.CorrespondingPtLt[j].Y;
        //            intCount += 2;
        //        }
        //    }
        //    X0[intCount, 0] = (1 - dblProportion) * dblFrLength + dblProportion * dblToLength;
        //    //X0[intCount + 1, 0] = (1 - dblProportion) * dblFrArea + dblProportion * dblToArea;

        //    //定义近似值矩阵XA
        //    VBMatrix XA = new VBMatrix(intCordinateNum, 1);
        //    for (int i = 0; i < intCordinateNum; i++)
        //    {
        //        XA[i, 0] = X0[i, 0];
        //    }

        //    //定义系数矩阵（最后两行的值将在循环中给定）
        //    VBMatrix A = new VBMatrix(intCordinateNum + 1, intCordinateNum);
        //    for (int i = 0; i < intCordinateNum; i++)
        //    {
        //        A[i, i] = 1;
        //    }

        //    double dblJudge = 0;   //该值用于判断是否应该跳出循环
        //    int intIterativeCount = 0;
        //    do
        //    {
        //        //计算系数A矩阵第intCordinateNum行的各元素，即周长对各未知数求偏导的值
        //        //先计算各分母值（注意：分母实际上是求偏导后的一部分值，但却恰好等于两点之间距离，因此其计算公式与距离计算公式相同
        //        double[] dblDis = new double[intInsertNum];
        //        for (int i = 0; i < intInsertNum - 1; i++)
        //        {
        //            dblDis[i] = Math.Pow((XA[2 * i, 0] - XA[2 * i + 2, 0]) * (XA[2 * i, 0] - XA[2 * i + 2, 0]) + (XA[2 * i + 1, 0] - XA[2 * i + 3, 0]) * (XA[2 * i + 1, 0] - XA[2 * i + 3, 0]), 0.5);
        //        }
        //        dblDis[intInsertNum - 1] = Math.Pow((XA[0, 0] - XA[intCordinateNum - 2, 0]) * (XA[0, 0] - XA[intCordinateNum - 2, 0]) + (XA[1, 0] - XA[intCordinateNum - 1, 0]) * (XA[1, 0] - XA[intCordinateNum - 1, 0]), 0.5);
        //        //开始计算系数矩阵第intCordinateNum行的各元素
        //        A[intCordinateNum, 0] = (XA[0, 0] - XA[2, 0]) / dblDis[0] + (XA[0, 0] - XA[intCordinateNum - 2, 0]) / dblDis[intInsertNum - 1];
        //        A[intCordinateNum, 1] = (XA[1, 0] - XA[3, 0]) / dblDis[0] + (XA[1, 0] - XA[intCordinateNum - 1, 0]) / dblDis[intInsertNum - 1];
        //        for (int j = 1; j < intInsertNum - 1; j++)
        //        {
        //            A[intCordinateNum, 2 * j] = (XA[2 * j, 0] - XA[2 * j + 2, 0]) / dblDis[j] + (XA[2 * j, 0] - XA[2 * j - 2, 0]) / dblDis[j - 1];
        //            A[intCordinateNum, 2 * j + 1] = (XA[2 * j + 1, 0] - XA[2 * j + 3, 0]) / dblDis[j] + (XA[2 * j + 1, 0] - XA[2 * j - 1, 0]) / dblDis[j - 1];
        //        }
        //        A[intCordinateNum, intCordinateNum - 2] = (XA[intCordinateNum - 2, 0] - XA[0, 0]) / dblDis[intInsertNum - 1] + (XA[intCordinateNum - 2, 0] - XA[intCordinateNum - 4, 0]) / dblDis[intInsertNum - 1];
        //        A[intCordinateNum, intCordinateNum - 1] = (XA[intCordinateNum - 1, 0] - XA[1, 0]) / dblDis[intInsertNum - 1] + (XA[intCordinateNum - 1, 0] - XA[intCordinateNum - 3, 0]) / dblDis[intInsertNum - 1];

        //        //////计算系数矩阵第intCordinateNum+1行的各元素，即面积对各未知数求偏导的值（注意：顶点的存储顺序为顺时针，按正常计算公式结果为负，因此需多加一个负号）
        //        //A[intCordinateNum + 1, 0] = XA[intCordinateNum - 1, 0] - XA[3, 0];
        //        //A[intCordinateNum + 1, 1] = XA[2, 0] - XA[intCordinateNum - 2, 0];
        //        //for (int j = 1; j < intInsertNum - 1; j++)
        //        //{
        //        //    A[intCordinateNum + 1, 2 * j] = XA[2 * j - 1, 0] - XA[2 * j + 3, 0];
        //        //    A[intCordinateNum + 1, 2 * j + 1] = XA[2 * j + 2, 0] - XA[2 * j - 2, 0];
        //        //}
        //        //A[intCordinateNum + 1, intCordinateNum - 2] = XA[intCordinateNum - 3, 0] - XA[1, 0];
        //        //A[intCordinateNum + 1, intCordinateNum - 1] = XA[0, 0] - XA[intCordinateNum - 4, 0];

        //        //计算matl矩阵
        //        VBMatrix matl = new VBMatrix(intCordinateNum + 1, 1);
        //        for (int i = 0; i < intCordinateNum; i++)
        //        {
        //            matl[i, 0] = XA[i, 0] - X0[i, 0];
        //        }
        //        //新多边形周长
        //        double dblSumLength = 0;
        //        for (int i = 0; i < intInsertNum; i++)
        //        {
        //            dblSumLength += dblDis[i];
        //        }
        //        matl[intCordinateNum, 0] = dblSumLength - X0[intCordinateNum, 0];
        //        ////新多边形面积
        //        //double dblSumArea = 0;
        //        //for (int i = 0; i < intInsertNum - 2; i++)
        //        //{
        //        //    dblSumArea += -XA[2 * i, 0] * XA[2 * i + 3, 0] + XA[2 * i + 2, 0] * XA[2 * i + 1, 0];
        //        //}
        //        //dblSumArea += -XA[intCordinateNum - 2, 0] * XA[1, 0] + XA[0, 0] * XA[intCordinateNum - 1, 0];
        //        //matl[intCordinateNum + 1, 0] = dblSumArea - X0[intCordinateNum + 1, 0];

        //        //记录一个值以协助判断是否可以退出循环
        //        double dblLast = XA[0, 0];

        //        //计算新的近似值


        //        //CHelpFuncExcel.ExportDataToExcelA(A, "maxA", _DataRecords.ParameterInitialize.strSavePath);
        //        //CHelpFuncExcel.ExportDataToExcelP(P, "maxP", _DataRecords.ParameterInitialize.strSavePath);
        //        //CHelpFuncExcel.ExportDataToExcel2(matl, "maxmatl", _DataRecords.ParameterInitialize.strSavePath);





        //        //VBMatrix Temp = A.Transpose() * P * A;
        //        //Temp.InvertGaussJordan();
        //        //XA -= Temp * A.Transpose() * P * matl;

        //        XA -= InvAtPAAtPmatl(A, P, matl);
        //        dblJudge = Math.Abs(dblLast - XA[0, 0]);

        //        intIterativeCount += 1;
        //    } while (dblJudge > dblTX);

        //    //生成目标线段
        //    List<CPoint> CTargetPtLt = new List<CPoint>();
        //    for (int i = 0; i < intInsertNum; i++)
        //    {
        //        CPoint cpt = new CPoint(i);
        //        cpt.X = XA[2 * i, 0];
        //        cpt.Y = XA[2 * i + 1, 0];
        //        CTargetPtLt.Add(cpt);
        //    }
        //    CPoint cpt0 = new CPoint(0);
        //    cpt0.X = XA[0, 0];
        //    cpt0.Y = XA[1, 0];
        //    CTargetPtLt.Add(cpt0);
        //    CPolyline cpl = new CPolyline(0, CTargetPtLt);
        //    return cpl;
        //}
        #endregion




        ///// <summary>属性：处理结果</summary>
        //public CParameterInitialize ParameterInitialize
        //{
        //    get { return _ParameterInitialize; }
        //    set { _ParameterInitialize = value; }
        //}

        ///// <summary>属性：处理结果</summary>
        //public CParameterResult ParameterResult
        //{
        //    get { return _ParameterResult; }
        //    set { _ParameterResult = value; }
        //}
    }
}
