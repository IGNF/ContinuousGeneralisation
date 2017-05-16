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
    /// <remarks>以坐标、长度和角度为参数进行平差</remarks>
    public class CPAL
    {
        private CDataRecords _DataRecords;                    //数据记录
        private double _dblTX;
        
        

        public CPAL()
        {

        }

        public CPAL(CDataRecords pDataRecords,double dblTX)
        {
            _DataRecords = pDataRecords;
            _dblTX = dblTX;
        }

        public CPAL(CDataRecords pDataRecords)
        {
            _DataRecords = pDataRecords;
            CPolyline FromCpl = pDataRecords.ParameterResult.FromCpl;
            _dblTX = FromCpl.pPolyline.Length / FromCpl.CptLt .Count  / 1000000;   //计算阈值参数
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
            List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;   //读取数据后，此处ResultPtLt中的对应点为一一对应
            double dblTX = _dblTX;

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

            int intUnknownXY = intUnknownNum * 2;   //每个点都有X、Y坐标
            int intPtNum = pCorrCptsLt.Count;
            int intXYNum = 2 * intPtNum;


            //找出长度固定的位置(如果一个线段的前后两个点都固定，则该长度固定)
            List<int> intKnownLengthLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count-1 ; i++)
            {
                if ((intKnownLocationLt[i+1]-intKnownLocationLt[i])==1)
                {
                    intKnownLengthLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownLength = pCorrCptsLt.Count - 1 - intKnownLengthLt.Count;

            //找出角度固定的位置(如果一个固定顶点的前后两个点都固定，则该角度固定)
            List<int> intKnownAngleLt = new List<int>();
            for (int i = 1; i < intKnownLocationLt.Count - 1; i++)
            {
                if ((intKnownLocationLt[i] - intKnownLocationLt[i - 1]) == 1 && (intKnownLocationLt[i+1] - intKnownLocationLt[i ]) == 1)
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
                P[intUnknownXY + i, intUnknownXY + i] = 100;
            }
            for (int i = 0; i < intUnknownAngle; i++)
            {
                P[intUnknownXY + intUnknownLength + i, intUnknownXY + intUnknownLength + i] = 10000;
            }
  
            //计算初始值矩阵X0
            VBMatrix X0 = new VBMatrix(intXYNum, 1);
            int intCount = 0;
            for (int i = 0; i < pCorrCptsLt.Count; i++)
            {
                X0[intCount, 0] = (1 - dblProportion) * pCorrCptsLt[i].FrCpt.X + dblProportion * pCorrCptsLt[i].ToCpt.X;
                X0[intCount + 1, 0] = (1 - dblProportion) * pCorrCptsLt[i].FrCpt.Y + dblProportion * pCorrCptsLt[i].ToCpt.Y;
                intCount += 2;
            }

            //计算长度初始值（全部计算）
            double[] adblLength0 = new double[intPtNum-1];
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                double dblfrsublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].FrCpt, pCorrCptsLt[i].FrCpt);
                double dbltosublength = CGeoFunc.CalDis(pCorrCptsLt[i + 1].ToCpt, pCorrCptsLt[i].ToCpt);
                adblLength0[i] = (1 - dblProportion) * dblfrsublength + dblProportion * dbltosublength;
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
                adblAngle0[i] = (1 - dblProportion) * dblfrAngle + dblProportion * dbltoAngle;
            }

            //Xmix里存储了XA和X0的最新混合值
            VBMatrix Xmix = new VBMatrix(intXYNum, 1);
            for (int i = 0; i < X0.Row; i++)
            {
                Xmix[i, 0] = X0[i, 0];
            }

            //定义坐标近似值矩阵XA
            VBMatrix XA = new VBMatrix(intUnknownXY, 1);
            int intSumCount0 = 0;
            for (int i = 0; i < intUnknownNum; i++)
            {
                if (pCorrCptsLt[intSumCount0].FrCpt.isCtrl == false)
                {
                    XA[i * 2, 0] = X0[intSumCount0 * 2, 0];
                    XA[i * 2 + 1, 0] = X0[intSumCount0 * 2 + 1, 0];
                }
                else
                {
                    i -= 1;
                }
                intSumCount0 += 1;
            }

            //定义系数矩阵（有关长度和角度的值将在循环中给定）
            VBMatrix A = new VBMatrix(intUnknownXYLengthAngle, intUnknownXY); 
            for (int i = 0; i < intUnknownXY; i++)
            {
                A[i, i] = 1;
            }

            double dblJudge1 = 0;   //该值用于判断是否应该跳出循环
            double dblJudge2 = 0;   //该值用于判断是否应该跳出循环
            int intJudgeIndex = intUnknownXY  / 4;
            int intIterativeCount = 0;
            double[] adblSubDis = new double[intPtNum - 1];
            double[] adblAngle = new double[intPtNum - 2];
            double[] adblAzimuth = new double[intPtNum - 1];            
            VBMatrix matl = new VBMatrix(intUnknownXYLengthAngle, 1);
            do
            {
                
                int intSumCount1 = 0;
                for (int i = 0; i < intUnknownNum; i++)
                {
                    if (pCorrCptsLt[intSumCount1].FrCpt.isCtrl == false)
                    {
                        matl[2 * i,0] = XA[2 * i, 0] - X0[intSumCount1 * 2, 0];
                        matl[2 * i + 1,0] = XA[2 * i + 1, 0] - X0[intSumCount1 * 2 + 1, 0];
                    }
                    else
                    {
                        i -= 1;
                    }
                    intSumCount1 += 1;
                }

                //计算系数矩阵A第"intUnknownXY"行到"intUnknownXY+intUnknownLength-1"行的各元素，即线段长度对各未知数求偏导的值
                //先计算各分母值（注意：分母实际上是求偏导后的一部分值，但却恰好等于两点之间距离，因此其计算公式与距离计算公式相同
                for (int i = 0; i < intPtNum-1; i++)
                {
                    adblSubDis[i] = Math.Pow((Xmix[2 * i, 0] - Xmix[2 * i + 2, 0]) * (Xmix[2 * i, 0] - Xmix[2 * i + 2, 0]) + (Xmix[2 * i + 1, 0] - Xmix[2 * i + 3, 0]) * (Xmix[2 * i + 1, 0] - Xmix[2 * i + 3, 0]), 0.5);
                }
                //计算新的夹角及方位角
                adblAzimuth[0] = CGeoFunc.CalAxisAngle(Xmix[0, 0], Xmix[1, 0], Xmix[2, 0], Xmix[3, 0]);
                for (int i = 1; i < intPtNum - 1; i++)
                {
                    adblAngle [i-1] = CGeoFunc.CalAngle_Counterclockwise(Xmix[i * 2 - 2, 0], Xmix[i * 2 - 1, 0], Xmix[i * 2, 0], Xmix[i * 2 + 1, 0], Xmix[i * 2 + 2, 0], Xmix[i * 2 + 3, 0]);
                    adblAzimuth[i] = adblAzimuth[i - 1] + adblAngle[i - 1] - Math.PI;
                }

                //开始计算系数矩阵第"intUnknownXY"行到"intUnknownXY+intUnknownLength-1"行的各元素
                CalADevLength(pCorrCptsLt, intUnknownXY,intUnknownLength, ref A, ref matl, adblSubDis, adblAzimuth, adblLength0);                
         
                 //计算系数矩阵A第"intUnknownXY+intUnknownLength"行到"intUnknownXY+intUnknownLength+intUnknownAngle"行的各元素，即角度对各未知数求偏导的值
                CalADevAngle(pCorrCptsLt, intUnknownXY + intUnknownLength, intUnknownAngle,Xmix , ref A, ref matl, adblSubDis,adblAngle , adblAngle0);                


                ////计算新的近似值
                //SaveFileDialog SFD = new SaveFileDialog();
                //SFD.ShowDialog();
                //     CHelpFuncExcel.ExportDataToExcelA(A, "maxA", SFD.FileName);
                //CHelpFuncExcel.ExportDataToExcelP(P, "maxP", SFD.FileName);
                //CHelpFuncExcel.ExportDataToExcel2(matl, "maxmatl", SFD.FileName);





                //VBMatrix Temp =A.Trans ()  * P * A;
                //Temp.InvertGaussJordan();
                //XA -= Temp * A.Transpose() * P * matl;

                //平差
                VBMatrix x = InvAtPAAtPmatl(A, P, matl);
                XA -= x;

                //更新Xmix
                int intSumCount4 = 0;
                for (int i = 0; i < intUnknownNum; i++)
                {
                    if (pCorrCptsLt[intSumCount4].FrCpt.isCtrl == false)
                    {
                        Xmix[intSumCount4 * 2, 0] = XA[i * 2, 0];
                        Xmix[intSumCount4 * 2 + 1, 0] = XA[i * 2 + 1, 0];
                   }
                    else
                    {
                        i -= 1;
                    }
                    intSumCount4 += 1;                 
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









        public void CalADevLength(List<CCorrCpts> pCorrCptsLt, int intBaseIndex, int intUnknownLength, ref VBMatrix A, ref VBMatrix matl, double[] dblSubDis, double[] adblAzimuth, double[] adblLength0)
        {
            //计算系数矩阵中关于长度值的导数部分（注意：隐含的距离计算公式为后一个点的坐标减前一个点的坐标）
            int intKnownCount = 0;
            int intUnKnownCount = 0;
            for (int i = 0; i < intUnknownLength; i++)
            {
                int intSumCount = intKnownCount + intUnKnownCount;
                if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == false && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == false)
                {
                    A[intBaseIndex + i, 2 * intUnKnownCount + 0] = -Math.Cos(adblAzimuth[intSumCount]);
                    A[intBaseIndex + i, 2 * intUnKnownCount + 1] = -Math.Sin(adblAzimuth[intSumCount]);
                    A[intBaseIndex + i, 2 * intUnKnownCount + 2] = -A[intBaseIndex + i, 2 * intUnKnownCount + 0];
                    A[intBaseIndex + i, 2 * intUnKnownCount + 3] = -A[intBaseIndex + i, 2 * intUnKnownCount + 1];

                    matl[intBaseIndex + i, 0] = adblLength0[intSumCount] - dblSubDis[intSumCount];   //图方便，顺便计算matl

                    intUnKnownCount += 1;
                }
                else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == false && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == true)
                {
                    A[intBaseIndex + i, 2 * intUnKnownCount + 0] = -Math.Cos(adblAzimuth[intSumCount]);
                    A[intBaseIndex + i, 2 * intUnKnownCount + 1] = -Math.Sin(adblAzimuth[intSumCount]);

                    matl[intBaseIndex + i, 0] = adblLength0[intSumCount] - dblSubDis[intSumCount];   //图方便，顺便计算matl

                    intUnKnownCount += 1;
                }
                else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == false)
                {
                    //注意这种情况，由于"pCorrCptsLt[intSumCount].FrCpt.isCtrl == true"不占位子（即不占列），因此列序号依然为" 2 * intUnKnownCount + 0"和" 2 * intUnKnownCount + 1"，而不是+2,+3
                    A[intBaseIndex + i, 2 * intUnKnownCount + 0] = Math.Cos(adblAzimuth[intSumCount]);
                    A[intBaseIndex + i, 2 * intUnKnownCount + 1] = Math.Sin(adblAzimuth[intSumCount]);

                    matl[intBaseIndex + i, 0] = adblLength0[intSumCount] - dblSubDis[intSumCount];   //图方便，顺便计算matl

                    intKnownCount += 1;
                }
                else
                {
                    intKnownCount += 1;
                    i -= 1;
                }
            }
        }

        public void CalADevAngle(List<CCorrCpts> pCorrCptsLt, int intBaseIndex, int intUnknownAngle, VBMatrix Xmix, ref VBMatrix A, ref VBMatrix matl, double[] adblSubDis, double[] adblAngle, double[] adblAngle0)
        {

            //计算系数矩阵A第"intUnknownXY+intUnknownLength"行到"intUnknownXY+intUnknownLength+intUnknownAngle"行的各元素，即角度对各未知数求偏导的值
            int intKnownCount = 0;
            int intUnKnownCount = 0;
            for (int i = 0; i < intUnknownAngle; i++)
            {
                //真是太幸运了，虽然求两向量逆时针夹角时需分多种情况讨论，但各情况的导数形式却是一致的，节省了不少编程精力啊，哈哈
                int intSumCount = intKnownCount + intUnKnownCount;

                //常用数据准备
                double dblA2 = adblSubDis[intSumCount] * adblSubDis[intSumCount];
                double dblB2 = adblSubDis[intSumCount + 1] * adblSubDis[intSumCount + 1];

                //开始计算系数值，由于将以下三个情况排列组合将有八种情况，因此按如下方式计算
                if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 2].FrCpt.isCtrl == true)
                {
                    intKnownCount += 1;
                    i -= 1;
                }
                else
                {
                    matl[intBaseIndex + i, 0] = adblAngle0[intSumCount] - adblAngle[intSumCount];   //图方便，顺便计算matl

                    int intPreTrueNum = 0;
                    int intUnKnownCountorginal = intUnKnownCount;
                    int intKnownCountorginal = intKnownCount;
                    if (pCorrCptsLt[intUnKnownCountorginal + intKnownCountorginal + 0].FrCpt.isCtrl == false)
                    {
                        //X1,Y1的导数值(注意：该部分是减数，因此值为导数的负数)
                        A[intBaseIndex + i, 2 * intUnKnownCountorginal + 0] = -(Xmix[2 * intSumCount + 3, 0] - Xmix[2 * intSumCount + 1, 0]) / dblA2;
                        A[intBaseIndex + i, 2 * intUnKnownCountorginal + 1] = (Xmix[2 * intSumCount + 2, 0] - Xmix[2 * intSumCount + 0, 0]) / dblA2;

                        intUnKnownCount += 1;
                    }
                    else
                    {
                        intPreTrueNum += 1;
                        intKnownCount += 1;
                    }

                    if (pCorrCptsLt[intUnKnownCountorginal + intKnownCountorginal + 1].FrCpt.isCtrl == false)
                    {
                        //X2,Y2的导数值(注意：后半部分是由减数产生的导数，因此值为导数的负数)                       
                        A[intBaseIndex + i, 2 * (intUnKnownCountorginal - intPreTrueNum) + 2] = (Xmix[2 * intSumCount + 5, 0] - Xmix[2 * intSumCount + 3, 0]) / dblB2 + (Xmix[2 * intSumCount + 3, 0] - Xmix[2 * intSumCount + 1, 0]) / dblA2;
                        A[intBaseIndex + i, 2 * (intUnKnownCountorginal - intPreTrueNum) + 3] = -(Xmix[2 * intSumCount + 4, 0] - Xmix[2 * intSumCount + 2, 0]) / dblB2 - (Xmix[2 * intSumCount + 2, 0] - Xmix[2 * intSumCount + 0, 0]) / dblA2;
                    }
                    else
                    {
                        intPreTrueNum += 1;
                    }
                    if (pCorrCptsLt[intUnKnownCountorginal + intKnownCountorginal + 2].FrCpt.isCtrl == false)
                    {
                        //X3,Y3的导数值
                        A[intBaseIndex + i, 2 * (intUnKnownCountorginal - intPreTrueNum) + 4] = -(Xmix[2 * intSumCount + 5, 0] - Xmix[2 * intSumCount + 3, 0]) / dblB2;
                        A[intBaseIndex + i, 2 * (intUnKnownCountorginal - intPreTrueNum) + 5] = (Xmix[2 * intSumCount + 4, 0] - Xmix[2 * intSumCount + 2, 0]) / dblB2;
                    }
                }

            }


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
