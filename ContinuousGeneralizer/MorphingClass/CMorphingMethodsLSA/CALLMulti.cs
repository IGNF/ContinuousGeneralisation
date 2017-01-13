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
    /// ������С����ԭ����Morphing�������ԽǶȺͱ߳�Ϊ����(Least Squares Alogrithm_Coordinate, Angle and Length)
    /// </summary>
    /// <remarks>�˼����ȡ��ǶȺͶ��߶μ��Ӧ������룬������Ϊƽ���������ƽ��������ƽ��</remarks>
    public class CALLMulti
    {
        private CDataRecords _DataRecords;                    //���ݼ�¼
        private double _dblTX;
        
        

        public CALLMulti()
        {

        }

        public CALLMulti(CDataRecords pDataRecords, double dblTX)
        {
            _DataRecords = pDataRecords;
            _dblTX = dblTX;
        }

        public CALLMulti(CDataRecords pDataRecords)
        {
            _DataRecords = pDataRecords;
            CPolyline FromCpl = pDataRecords.ParameterResult.FromCpl;
            _dblTX = FromCpl.pPolyline.Length / FromCpl.CptLt .Count  / 10000000000;   //������ֵ����
        }


        /// <summary>
        /// ��ʾ�����ص�����ֵ��״Ҫ��
        /// </summary>
        /// <param name="intInterNum">Inter: Interpolation</param>
        /// <returns>��״Ҫ��</returns>
        public void ALLMultiMorphing()
        {
            List<CPolyline> cpllt = GetTargetcpllt();
            cpllt.Insert(0, _DataRecords.ParameterResult.FromCpl);
            cpllt.Add(_DataRecords.ParameterResult.ToCpl);
            _DataRecords.ParameterResult.CResultPlLt = cpllt;








            //// ����滭�ۼ�
            //IMapControl4 m_mapControl = _DataRecords.ParameterInitialize.m_mapControl;
            //IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            //pGra.DeleteAllElements();
            //CHelperFunction.ViewPolyline(m_mapControl, cpl);  //��ʾ���ɵ��߶�
            //return cpl;
        }

        /// <summary>
        /// ��ȡ��״Ҫ��
        /// </summary>
        /// <param name="intInterNum">Inter: Interpolation</param>
        /// <returns>�ڴ�����״Ҫ��ʱ��������ԭ��״Ҫ�صı߽��п�������״Ҫ�ش������������������������״Ҫ��</returns>
        public List<CPolyline> GetTargetcpllt()
        {
            int intIterationNum = Convert.ToInt32(_DataRecords.ParameterInitialize.txtIterationNum.Text);
            int intInterNum = Convert.ToInt32(_DataRecords.ParameterInitialize.txtInterpolationNum.Text);
            List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;   //��ȡ���ݺ󣬴˴�ResultPtLt�еĶ�Ӧ��Ϊһһ��Ӧ
            double dblTX = _dblTX;
            double dblInterval = 1 / Convert.ToDouble(intInterNum + 1);


            int intPtNum = pCorrCptsLt.Count;
            int intXYNum = intPtNum * 2;
            int intMultiXYNum = intInterNum * intXYNum;


            //���㳤�ȳ�ʼֵ��ȫ�����㣩
            double[,] adblLength0 = new double[intInterNum, intPtNum - 1];
            double[,] adblLength = new double[intInterNum, intPtNum - 1];   //˳�㶨���м�ֵ����
            for (int j = 0; j < pCorrCptsLt.Count - 1; j++)
            {
                double dblfrsublength = CGeometricMethods.CalDis(pCorrCptsLt[j + 1].FrCpt, pCorrCptsLt[j].FrCpt);
                double dbltosublength = CGeometricMethods.CalDis(pCorrCptsLt[j + 1].ToCpt, pCorrCptsLt[j].ToCpt);
                for (int i = 0; i < intInterNum; i++)
                {
                    double dblProportion = (i+1) * dblInterval;
                    adblLength0[i, j] = (1 - dblProportion) * dblfrsublength + dblProportion * dbltosublength;
                }
                //pCorrCptsLt[j].FrCpt.isCtrl = false;  //�����ʼ���Ժ��������Ӷ�Ӧ��Ϊ�̶��㣬�ʴ�������Ϊfalse
            }

            //����Ƕȳ�ʼֵ��ȫ�����㣩
            double[,] adblAngle0 = new double[intInterNum, intPtNum - 2];
            double[,] adblAngle = new double[intInterNum, intPtNum - 2];
            for (int j = 0; j < pCorrCptsLt.Count - 2; j++)
            {
                //�ϴ��������״Ҫ���ϵļн�
                double dblfrAngle = CGeometricMethods.CalAngle2(pCorrCptsLt[j].FrCpt, pCorrCptsLt[j + 1].FrCpt, pCorrCptsLt[j + 2].FrCpt);
                //��С��������״Ҫ���ϵļн�
                double dbltoAngle = CGeometricMethods.CalAngle2(pCorrCptsLt[j].ToCpt, pCorrCptsLt[j + 1].ToCpt, pCorrCptsLt[j + 2].ToCpt);

                //�Ƕȳ�ʼֵ
                for (int i = 0; i < intInterNum; i++)
                {
                    double dblProportion = (i + 1) * dblInterval;
                    adblAngle0[i, j] = (1 - dblProportion) * dblfrAngle + dblProportion * dbltoAngle;
                }
            }

            //���߶μ��Ӧ��������ʼֵ��ȫ�����㣩��ȫ������Ϊ0
            //Ŀ��ֵҲ����Ϊ0
            double[,] adblIntervalDis0 = new double[intInterNum + 1, intPtNum];
            double[,] adblIntervalDis = new double[intInterNum + 1, intPtNum];   //˳�㶨���м�ֵ����
            for (int j = 0; j < pCorrCptsLt.Count; j++)
            {
                //���ȳ�ʼֵ
                for (int i = 0; i <= intInterNum; i++)
                {
                    adblIntervalDis0[i, j] = 0;
                    adblIntervalDis[i, j] = 0;
                }
            }


            //���������ʼֵ���Լ����߶η�λ�ǳ�ʼֵ
            //ע�⣺Ĭ�Ϲ̶���һ����
            pCorrCptsLt[0].FrCpt.isCtrl = true;
            pCorrCptsLt[1].FrCpt.isCtrl = true;
            VBMatrix X0 = new VBMatrix(intMultiXYNum, 1);
            double[,] adblAzimuth = new double[intInterNum, intPtNum - 1];
            for (int i = 0; i < intInterNum; i++)
            {
                double dblProportion = (i + 1) * dblInterval;
                double dblnewX0 = (1 - dblProportion) * pCorrCptsLt[0].FrCpt.X + dblProportion * pCorrCptsLt[0].ToCpt.X;
                double dblnewY0 = (1 - dblProportion) * pCorrCptsLt[0].FrCpt.Y + dblProportion * pCorrCptsLt[0].ToCpt.Y;
                double dblnewX1 = (1 - dblProportion) * pCorrCptsLt[1].FrCpt.X + dblProportion * pCorrCptsLt[1].ToCpt.X;
                double dblnewY1 = (1 - dblProportion) * pCorrCptsLt[1].FrCpt.Y + dblProportion * pCorrCptsLt[1].ToCpt.Y;
                adblAzimuth[i, 0] = CGeometricMethods.CalAxisAngle(dblnewX0, dblnewY0, dblnewX1, dblnewY1);

                int intBasicIndex = i * intXYNum;
                X0[intBasicIndex + 0, 0] = dblnewX0;
                X0[intBasicIndex + 1, 0] = dblnewY0;
                X0[intBasicIndex + 2, 0] = dblnewX1;
                X0[intBasicIndex + 3, 0] = dblnewY1;
            }

            //������
            //�Ƿ�̶����������
            pCorrCptsLt[pCorrCptsLt.Count - 2].FrCpt.isCtrl = true;
            pCorrCptsLt[pCorrCptsLt.Count - 1].FrCpt.isCtrl = true;

            for (int j = 2; j < intPtNum; j++)
            {
                for (int i = 0; i < intInterNum; i++)
                {
                    int intBasicIndexIJ = i * intXYNum + 2 * j;
                    double dblProportion = (i + 1) * dblInterval;
                    X0[intBasicIndexIJ + 0, 0] = (1 - dblProportion) * pCorrCptsLt[j].FrCpt.X + dblProportion * pCorrCptsLt[j].ToCpt.X;
                    X0[intBasicIndexIJ + 1, 0] = (1 - dblProportion) * pCorrCptsLt[j].FrCpt.Y + dblProportion * pCorrCptsLt[j].ToCpt.Y;
                    double dblAngle = CGeometricMethods.CalAngle2(X0[intBasicIndexIJ - 4, 0], X0[intBasicIndexIJ - 3, 0], X0[intBasicIndexIJ - 2, 0], X0[intBasicIndexIJ - 1, 0], X0[intBasicIndexIJ - 0, 0], X0[intBasicIndexIJ + 1, 0]);  //����ʵ�ʼн� 
                    adblAzimuth[i, j - 1] = adblAzimuth[i, j - 2] + dblAngle - Math.PI;
                }
            }


            //for (int j = 2; j < intPtNum; j++)
            //{

            //    if (pCorrCptsLt[j].FrCpt.isCtrl == false)
            //    {
            //        for (int i = 0; i < intInterNum; i++)
            //        {
            //            int intBasicIndexI = i * intXYNum;
            //            adblAzimuth[i, j - 1] = adblAzimuth[i, j - 2] + adblAngle0[i, j - 2] - Math.PI;
            //            X0[intBasicIndexI + 2 * j + 0, 0] = X0[intBasicIndexI + 2 * (j - 1) + 0, 0] + adblLength0[i, j - 1] * Math.Cos(adblAzimuth[i, j - 1]);
            //            X0[intBasicIndexI + 2 * j + 1, 0] = X0[intBasicIndexI + 2 * (j - 1) + 1, 0] + adblLength0[i, j - 1] * Math.Sin(adblAzimuth[i, j - 1]);
            //        }
            //    }
            //    else
            //    {
            //        for (int i = 0; i < intInterNum; i++)
            //        {
            //            int intBasicIndexIJ = i * intXYNum + 2 * j;
            //            double dblProportion = (i + 1) * dblInterval;
            //            X0[intBasicIndexIJ + 0, 0] = (1 - dblProportion) * pCorrCptsLt[j].FrCpt.X + dblProportion * pCorrCptsLt[j].ToCpt.X;
            //            X0[intBasicIndexIJ + 1, 0] = (1 - dblProportion) * pCorrCptsLt[j].FrCpt.Y + dblProportion * pCorrCptsLt[j].ToCpt.Y;
            //            double dblAngle = CGeometricMethods.CalAngle2(X0[intBasicIndexIJ - 4, 0], X0[intBasicIndexIJ - 3, 0], X0[intBasicIndexIJ - 2, 0], X0[intBasicIndexIJ - 1, 0], X0[intBasicIndexIJ - 0, 0], X0[intBasicIndexIJ + 1, 0]);  //����ʵ�ʼн� 
            //            adblAzimuth[i, j - 1] = adblAzimuth[i, j - 2] + dblAngle - Math.PI;
            //        }
            //    }
            //}


            //ͳ�Ʋ�ֵ����
            int intKnownPt = 0;           //�̶������Ŀ
            int intUnknownPt = 0;         //�ǹ̶������Ŀ

            List<int> intKnownLocationLt = new List<int>();  //��¼��֪������
            //ע�⣺���ڸ�ѭ������һ��Ĭ����������FromCpl�ĵ�һ������ֻ��һ����Ӧ��
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
            int intUnknownXY = intUnknownPt * 2;   //ÿ���㶼��X��Y����
            int intMultiUnknownXY = intInterNum * intUnknownXY;

            //�ҳ����ȹ̶���λ��(���һ���߶ε�ǰ�������㶼�̶�����ó��ȹ̶�)�����⣬���ȹ̶���ñߵķ�λ��Ҳ�̶�
            List<int> intKnownLengthLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 1; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1)
                {
                    intKnownLengthLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownLength = intPtNum - 1 - intKnownLengthLt.Count;

            //�ҳ��Ƕȹ̶���λ��(���һ���̶������ǰ�������㶼�̶�����ýǶȹ̶�)
            List<int> intKnownAngleLt = new List<int>();
            for (int i = 0; i < intKnownLocationLt.Count - 2; i++)
            {
                if ((intKnownLocationLt[i + 1] - intKnownLocationLt[i]) == 1 && (intKnownLocationLt[i + 2] - intKnownLocationLt[i + 1]) == 1)
                {
                    intKnownAngleLt.Add(intKnownLocationLt[i]);
                }
            }
            int intUnknownAngle = intPtNum - 2 - intKnownAngleLt.Count;

            //���ȽǶ�δ֪��
            int intUnknownLengthAngle = intUnknownLength + intUnknownAngle;

            //��δ֪������
            int intMultiUnknownLength = intInterNum * intUnknownLength;
            int intMultiUnknownAngle = intInterNum * intUnknownAngle;
            int intMultiUnknownLengthAngle = intMultiUnknownLength + intMultiUnknownAngle;
            int intMultiUnknownInterval = (intInterNum + 1) * intUnknownPt;
            int intSumConstraints = intMultiUnknownLength + intMultiUnknownAngle + intMultiUnknownInterval;

            //����Ȩ�ؾ���
            VBMatrix P = new VBMatrix(intSumConstraints, intSumConstraints);
            for (int i = 0; i < intMultiUnknownLength; i++)  //����Ȩ��
            {
                P[i, i] = 1;
            }
            for (int i = 0; i < intMultiUnknownAngle; i++)   //�Ƕ�Ȩ��
            {
                P[intMultiUnknownLength + i, intMultiUnknownLength + i] = 40;
            }
            for (int i = 0; i < intMultiUnknownInterval; i++)   //�Ƕ�Ȩ��
            {
                P[intMultiUnknownLengthAngle + i, intMultiUnknownLengthAngle + i] = 10000000000000;
            }




            //for (int i = 0; i < intInterNum; i++)
            //{
            //    int intBasicIndex2 = i * intUnknownLengthAngle;
            //    for (int j = 0; j < intUnknownLength; j++)
            //    {
            //        P[intBasicIndex2 + j, intBasicIndex2 + j] = 1;
            //    }
            //    for (int j = 0; j < intUnknownAngle; j++)
            //    {
            //        P[intBasicIndex2 + intUnknownLength + j, intBasicIndex2 + intUnknownLength + j] = 1;
            //    }
            //}

            //Xmix��洢��XA��X0�����»��ֵ���˾����ڹ�ʽ�Ƶ��в������ڣ�ֻ��Ϊ�˷����д�����������
            VBMatrix Xmix = new VBMatrix(intMultiXYNum, 1);
            for (int i = 0; i < intMultiXYNum; i++)
            {
                Xmix[i, 0] = X0[i, 0];
            }

            //�����������ֵ����XA
            VBMatrix XA = new VBMatrix(intMultiUnknownXY, 1);
            VBMatrix XA0 = new VBMatrix(intMultiUnknownXY, 1);
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

            //����ϵ������A(�����̶�����ĵ���ֵ)��A�ĵ���ֵ����ѭ���и���
            VBMatrix A = new VBMatrix(intSumConstraints, intMultiUnknownXY);
            double dblJudge1 = 0;   //��ֵ�����ж��Ƿ�Ӧ������ѭ��
            double dblJudge2 = 0;   //��ֵ�����ж��Ƿ�Ӧ������ѭ��
            double dblJudge3 = 0;   //��ֵ�����ж��Ƿ�Ӧ������ѭ��
            int intJudgeIndex = intMultiUnknownXY / 4;
            int intIterativeCount = 0;

            for (int k = 0; k < 2; k++)
            {
                //break;
                do
                {

                    
                    if (intIterativeCount >= intIterationNum)
                    {
                        break;
                    }
                    intIterativeCount += 1;


                    VBMatrix matl = new VBMatrix(intSumConstraints, 1);


                    //����ϵ������A��0�е�"intUnknownLength"�еĸ�Ԫ�أ����߶γ��ȶԸ�δ֪����ƫ����ֵ
                    //�ȼ������ĸֵ��ע�⣺��ĸʵ��������ƫ�����һ����ֵ����ȴǡ�õ�������֮����룬�������㹫ʽ�������㹫ʽ��ͬ
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

                    //�����µķ�λ��
                    for (int i = 0; i < intInterNum; i++)
                    {
                        int intBasicIndexA1 = i * intXYNum;
                        //��һ���߶εķ�λ��
                        adblAzimuth[i, 0] = CGeometricMethods.CalAxisAngle(Xmix[intBasicIndexA1 + 0, 0], Xmix[intBasicIndexA1 + 1, 0], Xmix[intBasicIndexA1 + 2, 0], Xmix[intBasicIndexA1 + 3, 0]);
                        //�����߶εķ�λ��
                        for (int j = 1; j < intPtNum - 1; j++)
                        {
                            int intBasicIndexIJA1 = intBasicIndexA1 + 2 * j;
                            double dblAngle = CGeometricMethods.CalAngle2(Xmix[intBasicIndexIJA1 - 2, 0], Xmix[intBasicIndexIJA1 - 1, 0],
                                                                        Xmix[intBasicIndexIJA1 + 0, 0], Xmix[intBasicIndexIJA1 + 1, 0],
                                                                        Xmix[intBasicIndexIJA1 + 2, 0], Xmix[intBasicIndexIJA1 + 3, 0]);
                            adblAzimuth[i, j] = adblAzimuth[i, j - 1] + dblAngle - Math.PI;
                        }
                    }

                    //�����µĶ��߶μ��Ӧ�������
                    for (int j = 0; j < intPtNum; j++)
                    {
                        int int2J = 2 * j;
                        //Դ�߶����һ�����߶μ��Ӧ�����
                        adblIntervalDis[0, j] = Math.Sqrt((pCorrCptsLt[j].FrCpt.X - Xmix[int2J + 0, 0]) * (pCorrCptsLt[j].FrCpt.X - Xmix[int2J + 0, 0]) +
                                                          (pCorrCptsLt[j].FrCpt.Y - Xmix[int2J + 1, 0]) * (pCorrCptsLt[j].FrCpt.Y - Xmix[int2J + 1, 0]));
                        //Ŀ���߶�����������߶μ��Ӧ�����
                        adblIntervalDis[intInterNum, j] = Math.Sqrt((pCorrCptsLt[j].ToCpt.X - Xmix[(intInterNum - 1) * intXYNum + int2J + 0, 0]) * (pCorrCptsLt[j].ToCpt.X - Xmix[(intInterNum - 1) * intXYNum + int2J + 0, 0]) +
                                                                    (pCorrCptsLt[j].ToCpt.Y - Xmix[(intInterNum - 1) * intXYNum + int2J + 1, 0]) * (pCorrCptsLt[j].ToCpt.Y - Xmix[(intInterNum - 1) * intXYNum + int2J + 1, 0]));
                        //�������߶μ��Ӧ�����
                        for (int i = 1; i < intInterNum; i++)
                        {
                            adblIntervalDis[i, j] = Math.Sqrt((Xmix[(i - 1) * intXYNum + int2J + 0, 0] - Xmix[i * intXYNum + int2J + 0, 0]) * (Xmix[(i - 1) * intXYNum + int2J + 0, 0] - Xmix[i * intXYNum + int2J + 0, 0]) +
                                                              (Xmix[(i - 1) * intXYNum + int2J + 1, 0] - Xmix[i * intXYNum + int2J + 1, 0]) * (Xmix[(i - 1) * intXYNum + int2J + 1, 0] - Xmix[i * intXYNum + int2J + 1, 0]));
                        }
                    }

                    //����ϵ�������й��ڳ���ֵ�ĵ������֣�ע�⣺�����ľ�����㹫ʽΪ��һ����������ǰһ��������꣩
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
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 0] = -Math.Cos(adblAzimuth[i, intSumCount]);
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 1] = -Math.Sin(adblAzimuth[i, intSumCount]);
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 2] = -A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 0];
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 3] = -A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 1];

                                matl[i * intUnknownLength + j, 0] =adblLength0[i, intSumCount]- adblLength[i, intSumCount];   //ͼ���㣬˳�����matl                            
                            }
                            intUnKnownCount2 += 1;
                        }
                        else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == false && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == true)
                        {
                            for (int i = 0; i < intInterNum; i++)
                            {
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 0] = -Math.Cos(adblAzimuth[i, intSumCount]);
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 1] = -Math.Sin(adblAzimuth[i, intSumCount]);

                                matl[i * intUnknownLength + j, 0] = adblLength0[i, intSumCount]- adblLength[i, intSumCount];   //ͼ���㣬˳�����matl                            
                            }
                            intUnKnownCount2 += 1;
                        }
                        else if (pCorrCptsLt[intSumCount].FrCpt.isCtrl == true && pCorrCptsLt[intSumCount + 1].FrCpt.isCtrl == false)
                        {
                            for (int i = 0; i < intInterNum; i++)
                            {
                                //ע���������������"pCorrCptsLt[intSumCount].FrCpt.isCtrl == true"��ռλ�ӣ�����ռ�У�������������ȻΪ" 2 * intUnKnownCount + 0"��" 2 * intUnKnownCount + 1"��������+2,+3
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 0] = Math.Cos(adblAzimuth[i, intSumCount]);
                                A[i * intUnknownLength + j, i * intUnknownXY + intBasicIndexL2 + 1] = Math.Sin(adblAzimuth[i, intSumCount]);

                                matl[i * intUnknownLength + j, 0] =adblLength0[i, intSumCount]- adblLength[i, intSumCount];   //ͼ���㣬˳�����matl                            
                            }
                            intKnownCount2 += 1;
                        }
                        else
                        {
                            intKnownCount2 += 1;
                            j -= 1;
                        }
                    }

                    //����ϵ�������й��ڼн�ֵ�ĵ�������
                    int intKnownCount3 = 0;
                    int intUnKnownCount3 = 0;
                    for (int j = 0; j < intUnknownAngle; j++)
                    {
                        //����̫�����ˣ���Ȼ����������ʱ��н�ʱ��ֶ���������ۣ���������ĵ�����ʽȴ��һ�µģ���ʡ�˲��ٱ�̾�����������
                        int intSumCount = intKnownCount3 + intUnKnownCount3;


                        //��������׼��
                        double[] adblA2 = new double[intInterNum];
                        double[] adblB2 = new double[intInterNum];
                        for (int i = 0; i < intInterNum; i++)
                        {
                            adblA2[i] = adblLength[i, intSumCount + 0] * adblLength[i, intSumCount + 0];
                            adblB2[i] = adblLength[i, intSumCount + 1] * adblLength[i, intSumCount + 1];
                        }

                        //��ʼ����ϵ��ֵ�����ڽ������������������Ͻ��а����������˰����·�ʽ����
                        if (pCorrCptsLt[intUnKnownCount3 + intKnownCount3].FrCpt.isCtrl == true && pCorrCptsLt[intUnKnownCount3 + intKnownCount3 + 1].FrCpt.isCtrl == true && pCorrCptsLt[intUnKnownCount3 + intKnownCount3 + 2].FrCpt.isCtrl == true)
                        {
                            intKnownCount3 += 1;
                            j -= 1;
                        }
                        else
                        {
                            for (int i = 0; i < intInterNum; i++)
                            {
                                int intBasicIndexFi3 = i * intXYNum + 2 * intSumCount;
                                double dblNewAngle = CGeometricMethods.CalAngle2(Xmix[intBasicIndexFi3 + 0, 0], Xmix[intBasicIndexFi3 + 1, 0],
                                                                               Xmix[intBasicIndexFi3 + 2, 0], Xmix[intBasicIndexFi3 + 3, 0],
                                                                               Xmix[intBasicIndexFi3 + 4, 0], Xmix[intBasicIndexFi3 + 5, 0]);

                                adblAngle[i, j] = dblNewAngle;
                                matl[intMultiUnknownLength + i * intUnknownAngle + j, 0] =adblAngle0[i, intSumCount]- dblNewAngle;      //ͼ���㣬˳�����matl
                            }

                            int intPreTrueNum = 0;
                            int intUnKnownCount3orginal = intUnKnownCount3;
                            int intKnownCount3orginal = intKnownCount3;
                            if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 0].FrCpt.isCtrl == false)
                            {
                                //X1,Y1�ĵ���ֵ(ע�⣺�ò����Ǽ��������ֵΪ�����ĸ���)
                                for (int i = 0; i < intInterNum; i++)
                                {
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * intUnKnownCount3orginal + 0] = -(Xmix[i * intXYNum + 2 * intSumCount + 3, 0] - Xmix[i * intXYNum + 2 * intSumCount + 1, 0]) / adblA2[i];
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * intUnKnownCount3orginal + 1] = (Xmix[i * intXYNum + 2 * intSumCount + 2, 0] - Xmix[i * intXYNum + 2 * intSumCount + 0, 0]) / adblA2[i];
                                }

                                intUnKnownCount3 += 1;
                            }
                            else
                            {
                                intPreTrueNum += 1;
                                intKnownCount3 += 1;
                            }

                            if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 1].FrCpt.isCtrl == false)
                            {
                                //X2,Y2�ĵ���ֵ
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
                            if (pCorrCptsLt[intUnKnownCount3orginal + intKnownCount3orginal + 2].FrCpt.isCtrl == false)
                            {
                                //X3,Y3�ĵ���ֵ
                                for (int i = 0; i < intInterNum; i++)
                                {
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * (intUnKnownCount3orginal - intPreTrueNum) + 4] = -(Xmix[i * intXYNum + 2 * intSumCount + 5, 0] - Xmix[i * intXYNum + 2 * intSumCount + 3, 0]) / adblB2[i];
                                    A[intMultiUnknownLength + i * intUnknownAngle + j, i * intUnknownXY + 2 * (intUnKnownCount3orginal - intPreTrueNum) + 5] = (Xmix[i * intXYNum + 2 * intSumCount + 4, 0] - Xmix[i * intXYNum + 2 * intSumCount + 2, 0]) / adblB2[i];
                                }
                            }
                        }
                    }

                    //����ϵ�������й��ڶ��߶μ��Ӧ�������ĵ������֣�ע�⣺�����ľ�����㹫ʽΪ��һ����������ǰһ��������꣩
                    int intKnownCount4 = 0;
                    int intUnKnownCount4 = 0;
                    for (int j = 0; j < intUnknownPt; j++)
                    {
                        int intSumCount4 = intKnownCount4 + intUnKnownCount4;
                        int intBasicIndexL4 = 2 * intUnKnownCount4;
                        if (pCorrCptsLt[intSumCount4].FrCpt.isCtrl == false)
                        {
                            int l = 0;
                            //Դ�߶����һ�����߶μ��Ӧ����뵼��
                            A[intMultiUnknownLengthAngle + l * intUnknownPt + j, (l - 0) * intUnknownXY + intBasicIndexL4 + 0] = (Xmix[l * intXYNum + 2 * intSumCount4 + 0, 0] - pCorrCptsLt[intSumCount4].FrCpt.X) / adblIntervalDis[l, intSumCount4];
                            A[intMultiUnknownLengthAngle + l * intUnknownPt + j, (l - 0) * intUnknownXY + intBasicIndexL4 + 1] = (Xmix[l * intXYNum + 2 * intSumCount4 + 1, 0] - pCorrCptsLt[intSumCount4].FrCpt.Y) / adblIntervalDis[l, intSumCount4];

                            matl[intMultiUnknownLengthAngle + l * intUnknownPt + j, 0] =adblIntervalDis0[l, intSumCount4]- adblIntervalDis[l, intSumCount4];      //ͼ���㣬˳�����matl

                            //��������߶���Ŀ���߶μ��Ӧ����뵼��
                            l = intInterNum;
                            A[intMultiUnknownLengthAngle + l  * intUnknownPt + j, (l  - 1) * intUnknownXY + intBasicIndexL4 + 0] = -(pCorrCptsLt[intSumCount4].ToCpt.X - Xmix[(l  - 1) * intXYNum + 2 * intSumCount4 + 0, 0]) / adblIntervalDis[l , intSumCount4];
                            A[intMultiUnknownLengthAngle + l  * intUnknownPt + j, (l  - 1) * intUnknownXY + intBasicIndexL4 + 1] = -(pCorrCptsLt[intSumCount4].ToCpt.Y - Xmix[(l  - 1) * intXYNum + 2 * intSumCount4 + 1, 0]) / adblIntervalDis[l , intSumCount4];

                            matl[intMultiUnknownLengthAngle + l  * intUnknownPt + j, 0] =adblIntervalDis0[l , intSumCount4]- adblIntervalDis[l , intSumCount4];      //ͼ���㣬˳�����matl

                            //�������߶μ��Ӧ����뵼��
                            for (int i = 1; i < intInterNum; i++)
                            {
                                A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 1) * intUnknownXY + intBasicIndexL4 + 0] = -(Xmix[i * intXYNum + 2 * intSumCount4 + 0, 0] - Xmix[(i - 1) * intXYNum + 2 * intSumCount4 + 0, 0]) / adblIntervalDis[i, intSumCount4];
                                A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 1) * intUnknownXY + intBasicIndexL4 + 1] = -(Xmix[i * intXYNum + 2 * intSumCount4 + 1, 0] - Xmix[(i - 1) * intXYNum + 2 * intSumCount4 + 1, 0]) / adblIntervalDis[i, intSumCount4];
                                A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 0) * intUnknownXY + intBasicIndexL4 + 0] = -A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 1) * intUnknownXY + +intBasicIndexL4 + 0];
                                A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 0) * intUnknownXY + intBasicIndexL4 + 1] = -A[intMultiUnknownLengthAngle + i * intUnknownPt + j, (i - 1) * intUnknownXY + +intBasicIndexL4 + 1];

                                matl[intMultiUnknownLengthAngle + i * intUnknownPt + j, 0] =adblIntervalDis0[i, intSumCount4]- adblIntervalDis[i, intSumCount4];      //ͼ���㣬˳�����matl
                                //double dblIntervalDis = adblIntervalDis[i, intSumCount4];
                                //double dblIntervalDis0 = adblIntervalDis0[i, intSumCount4];
                                //double dblmatl = dblIntervalDis - dblIntervalDis0;
                                //double ttt = 6;
                            }

                            intUnKnownCount4 += 1;
                        }
                        else
                        {
                            intKnownCount4 += 1;
                            j -= 1;
                        }
                    }



                    int tt = 5;

                    //CHelperFunctionExcel.ExportDataToExcel2(A, "matA", _DataRecords.ParameterInitialize.strSavePath);
                    //CHelperFunctionExcel.ExportDataToExcelP(P, "matP", _DataRecords.ParameterInitialize.strSavePath);
                    //CHelperFunctionExcel.ExportDataToExcel2(matl, "matmatl", _DataRecords.ParameterInitialize.strSavePath);



                    //ƽ��
                    VBMatrix Temp = A.Trans() * P * A;
                    VBMatrix InvTemp = Temp.Inv(Temp);
                    VBMatrix x = InvTemp * A.Trans() * P * matl;

                    XA += x;


                    //CHelperFunctionExcel.ExportDataToExcel2(XA, "matXA", _DataRecords.ParameterInitialize.strSavePath);

                    //��¼��ƽ��ɹ�
                    //�������ֵ
                    //VBMatrix Xc = XA - XA0;
                    //�۲�ֵ����ֵ����V
                    VBMatrix V = A * x - matl;
                    //VtPVֵ
                    double dblVtPV = (V.Trans() * P * V).MatData[0, 0];

                    _DataRecords.ParameterInitialize.txtVtPV.Text = "   VtPV = " + dblVtPV.ToString();


                    //VBMatrix L = new VBMatrix(intSumConstraints, 1);
                    //for (int i = 0; i < intInterNum; i++)
                    //{
                    //    for (int j = 0; j < intUnknownLength; j++)
                    //    {
                    //        L[i * intUnknownLength + j, 0] = adblLength[i, j];
                    //    }

                    //    for (int j = 0; j < intUnknownAngle; j++)
                    //    {
                    //        L[intInterNum * intUnknownLength + i * intUnknownAngle + j, 0] = adblAngle[i, j];
                    //    }
                    //}
                    //for (int i = 0; i <= intInterNum; i++)
                    //{
                    //    for (int j = 0; j < intUnknownPt; j++)
                    //    {
                    //        L[intInterNum * intUnknownLengthAngle + i * intUnknownPt + j, 0] = adblIntervalDis[i, j];
                    //    }
                    //}


                    //VBMatrix LPlusV = L + V;
                    //VBMatrix AX = A * XA;

                    //CHelperFunctionExcel.ExportDataToExcel2(LPlusV, "matLPlusV", _DataRecords.ParameterInitialize.strSavePath);
                    //CHelperFunctionExcel.ExportDataToExcel2(AX, "matAX", _DataRecords.ParameterInitialize.strSavePath);



                    //����Xmix
                    int intSumCount5 = 0;
                    for (int j = 0; j < intUnknownPt; j++)
                    {
                        if (pCorrCptsLt[intSumCount5].FrCpt.isCtrl == false)
                        {
                            for (int i = 0; i < intInterNum; i++)
                            {
                                Xmix[i * intXYNum + intSumCount5 * 2 + 0, 0] = XA[i * intUnknownXY + j * 2 + 0, 0];
                                Xmix[i * intXYNum + intSumCount5 * 2 + 1, 0] = XA[i * intUnknownXY + j * 2 + 1, 0];
                            }
                        }
                        else
                        {
                            j -= 1;
                        }
                        intSumCount5 += 1;
                    }



                    //����ֻ�����ȡ�����м�ֵ�Թ۲��Ƿ�����
                    dblJudge1 = Math.Abs(x[1 * intJudgeIndex, 0]);
                    dblJudge2 = Math.Abs(x[2 * intJudgeIndex, 0]);
                    dblJudge3 = Math.Abs(x[3 * intJudgeIndex, 0]);

                //} while ((dblJudge1 > dblTX) || (dblJudge2 > dblTX) || (dblJudge3 > dblTX));
                }while ((dblJudge1 >= 0) || (dblJudge2 >= 0) || (dblJudge3 >= 0)) ;
                break;








                for (int i = 0; i < intMultiUnknownLength; i++)  //����Ȩ��
                {
                    P[i, i] = 1;
                }
                for (int i = 0; i < intMultiUnknownAngle; i++)   //�Ƕ�Ȩ��
                {
                    P[intMultiUnknownLength + i, intMultiUnknownLength + i] = 39.48;
                }
                for (int i = 0; i < intMultiUnknownInterval; i++)   //�Ƕ�Ȩ��
                {
                    P[intMultiUnknownLengthAngle + i, intMultiUnknownLengthAngle + i] = 1;
                }
            }
           

            //����Ŀ���߶�
            List<CPolyline> cpllt = new List<CPolyline>();
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

            return cpllt;
        }


    }
}