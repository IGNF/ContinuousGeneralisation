using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;

using MorphingClass.CGeometry;
using MorphingClass.CUtility;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CEntity;

namespace MorphingClass.CEvaluationMethods
{


    public class CTranslation
    {
        private CDataRecords _pDataRecords;
        private List<CParameterResult> _pParameterResultToExcelLt = new List<CParameterResult>();
        //private double _dblSumLength = 0;
        //private double _intIntersectNum = 0;

        public CTranslation()
        {

        }

        //public CTranslation(double dblSumLength)
        //{
        //    _dblSumLength = dblSumLength;
        //}

        public CTranslation(CDataRecords pDataRecords)
        {
            _pDataRecords = pDataRecords;
            //_pDataRecords.ParameterResultToExcel = _pParameterResultToExcel;
            //_pDataRecords.ParameterResultToExcelLt = _pParameterResultToExcelLt;
            //_pParameterResultToExcel.strEvaluationMethod = "Translation";
        }


        #region CalTranslation

        /// <summary>计算Translation指标值，并保存输出</summary>
        /// <returns>Translation指标值</returns>
        /// <remarks></remarks>
        public double CalTranslation()
        {
            CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            List<CPoint> resultptlt = ParameterResult.CResultPtLt;
            CParameterInitialize ParameterInitialize = _pDataRecords.ParameterInitialize;
            StatusStrip ststMain = ParameterInitialize.ststMain;
            ToolStripStatusLabel tsslTime = ParameterInitialize.tsslTime;
            ToolStripStatusLabel tsslMessage = ParameterInitialize.tsslMessage;
            ToolStripProgressBar tspbMain = ParameterInitialize.tspbMain;
            tsslMessage.Text = "正在计算Translation...";
            ststMain.Refresh();
            long lngStartTime = System.Environment.TickCount;

            //生成计算Translation指标值的线段
            List<CPoint> CFrPtLtToExcel = new List<CPoint>();
            List<CPoint> CToPtLtToExcel = new List<CPoint>();
            List<CPoint> translationptlt = new List<CPoint>();
            int intptnum = 0;
            for (int i = 0; i < resultptlt.Count; i++)
            {
                for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
                {
                    double dblX = resultptlt[i].CorrespondingPtLt[j].X - resultptlt[i].X;
                    double dblY = resultptlt[i].CorrespondingPtLt[j].Y - resultptlt[i].Y;
                    CPoint cpt = new CPoint(intptnum, dblX, dblY);
                    translationptlt.Add(cpt);

                    CFrPtLtToExcel.Add(resultptlt[i]);
                    CToPtLtToExcel.Add(resultptlt[i].CorrespondingPtLt[j]);
                    intptnum = intptnum + 1;
                }
                tspbMain.Value = (i + 1) * 50 / (resultptlt.Count);
            }

            //添加第一个元素
            List<double> dblTranslationLt = new List<double>();
            List<double> dblSumTranslationLt = new List<double>();
            dblTranslationLt.Add(0);
            dblSumTranslationLt.Add(0);
            double dblSumLenth = 0;
            for (int i = 1; i < translationptlt.Count; i++)
            {
                double dblLength = CGeoFunc.CalDis(translationptlt[i - 1], translationptlt[i]);
                dblTranslationLt.Add(dblLength);
                dblSumLenth = dblSumLenth + dblLength;
                dblSumTranslationLt.Add(dblSumLenth);

                tspbMain.Value = (i + 1) * 50 / (translationptlt.Count) + 50;
            }

            CParameterResult pParameterResultToExcel = new CParameterResult();
            pParameterResultToExcel.strEvaluationMethod = "Translation";
            pParameterResultToExcel.FromPtLt = CFrPtLtToExcel;
            pParameterResultToExcel.ToPtLt = CToPtLtToExcel;
            pParameterResultToExcel.TranlationPtLt = translationptlt;
            pParameterResultToExcel.dblTranslationLt = dblTranslationLt;
            pParameterResultToExcel.dblSumTranslationLt = dblSumTranslationLt;
            _pDataRecords.ParameterResultToExcel = pParameterResultToExcel;

            long lngEndTime = System.Environment.TickCount;
            long lngTime = lngEndTime - lngStartTime;
            tsslTime.Text = "TranslationRunning Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间
            tsslMessage.Text = "Translation计算完成！";

            return dblSumLenth;
        }


        /// <summary>计算Translation指标值</summary>
        /// <param name="resultptlt">对应结果数组</param>
        /// <returns>Translation指标值</returns>
        /// <remarks>已释放内存</remarks>
        public double CalTranslation(List<CPoint> resultptlt)
        {
            //生成计算Translation指标值的线段
            double dblSumLenth = 0;
            double dblLastX = resultptlt[0].CorrespondingPtLt[0].X - resultptlt[0].X;
            double dblLastY = resultptlt[0].CorrespondingPtLt[0].Y - resultptlt[0].Y;
            for (int i = 0; i < resultptlt.Count; i++)
            {
                for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
                {
                    double dblX = resultptlt[i].CorrespondingPtLt[j].X - resultptlt[i].X;
                    double dblY = resultptlt[i].CorrespondingPtLt[j].Y - resultptlt[i].Y;
                    dblSumLenth+=CGeoFunc.CalDis(dblLastX, dblLastY, dblX, dblY);
                    dblLastX = dblX;
                    dblLastY = dblY;
                }
            }
            return dblSumLenth;
        }

       

        /// <summary>Calculate translation values for a List<List<CCorrCpts>> which has been recorded in ParameterResult</summary>
        /// <remarks></remarks>
        public double CalTranslationCorr()
        {
            CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            List<List<CCorrCpts>> CorrCptsLtLt = ParameterResult.pMorphingBase.CorrCptsLtLt;
            List<List<double>> dblTranslationLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            List<List<double>> dblSumTranslationLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            double dblSUM = CalTranslationCorr(CorrCptsLtLt, dblTranslationLtLt, dblSumTranslationLtLt);

            ParameterResult.strEvaluationMethod = "Translation";
            ParameterResult.dblTranslationLtLt = dblTranslationLtLt;
            ParameterResult.dblSumTranslationLtLt = dblSumTranslationLtLt;

            return dblSUM;
        }

        public double CalTranslationCorr(List<List<CCorrCpts>> CorrCptsLtLt, List<List<double>> dblTranslationLtLt = null, List<List<double>> dblSumTranslationLtLt = null)
        {
            if (dblTranslationLtLt == null)
            {
                dblTranslationLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            }
            if (dblSumTranslationLtLt == null)
            {
                dblSumTranslationLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            }

            double dblSUM = 0;
            foreach (List<CCorrCpts> CorrCptsLt in CorrCptsLtLt)
            {
                List<double> dblTranslationLt = new List<double>(CorrCptsLt.Count);
                List<double> dblSumTranslationLt = new List<double>(CorrCptsLt.Count);

                CalTranslationCorr(CorrCptsLt, dblTranslationLt, dblSumTranslationLt);
                dblTranslationLtLt.Add(dblTranslationLt);
                dblSumTranslationLtLt.Add(dblSumTranslationLt);
                dblSUM += dblSumTranslationLt[dblSumTranslationLt.Count - 1];
            }

            return dblSUM;
        }

        public double CalTranslationCorr(List<CCorrCpts> CorrCptsLt, List<double> dblTranslationLt = null, List<double> dblSumTranslationLt = null)
        {
            //if (dblTranslationLt == null)
            //{
            //    dblTranslationLt = new List<double>(CorrCptsLt.Count);
            //}
            //if (dblSumTranslationLt == null)
            //{
            //    dblSumTranslationLt = new List<double>(CorrCptsLt.Count);
            //}

            //double dblSumLenth = 0;
            //dblTranslationLt.Add(0);
            //dblSumTranslationLt.Add(0);
            //LinkedListNode<CCorrCpts> LastCorrCpt = CorrCptsLt.First;
            //LinkedListNode<CCorrCpts> CurrentCorrCpt = LastCorrCpt.Next;

            //while (CurrentCorrCpt != null)
            //{
            //    double dblLength = LastCorrCpt.Value.pMoveVector.DistanceTo(CurrentCorrCpt.Value.pMoveVector);
            //    dblSumLenth += dblLength;
            //    dblTranslationLt.Add(dblLength);
            //    dblSumTranslationLt.Add(dblSumLenth);

            //    LastCorrCpt = CurrentCorrCpt;
            //    CurrentCorrCpt = CurrentCorrCpt.Next;
            //}
            //return dblSumLenth;

            return 0;
        }

        /// <summary>计算多条对应线状要素的Translation指标值总和</summary>
        /// <returns>Translation指标值</returns>
        /// <remarks></remarks>
        public double CalTranslations()
        {
            CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            List<List<CPoint>> resultptltlt = ParameterResult.CResultPtLtLt;
            CParameterInitialize ParameterInitialize = _pDataRecords.ParameterInitialize;
            StatusStrip ststMain = ParameterInitialize.ststMain;
            ToolStripStatusLabel tsslTime = ParameterInitialize.tsslTime;
            ToolStripStatusLabel tsslMessage = ParameterInitialize.tsslMessage;
            ToolStripProgressBar tspbMain = ParameterInitialize.tspbMain;
            tsslMessage.Text = "正在计算Translation...";
            ststMain.Refresh();
            long lngStartTime = System.Environment.TickCount;

            double dblAllSumLength = 0;
            for (int k = 0; k < resultptltlt.Count; k++)
            {
                List<CPoint> resultptlt = resultptltlt[k];
                //生成计算Translation指标值的线段
                List<CPoint> CFrPtLtToExcel = new List<CPoint>();
                List<CPoint> CToPtLtToExcel = new List<CPoint>();
                List<CPoint> translationptlt = new List<CPoint>();
                int intptnum = 0;
                for (int i = 0; i < resultptlt.Count; i++)
                {
                    for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
                    {
                        double dblX = resultptlt[i].CorrespondingPtLt[j].X - resultptlt[i].X;
                        double dblY = resultptlt[i].CorrespondingPtLt[j].Y - resultptlt[i].Y;
                        CPoint cpt = new CPoint(intptnum, dblX, dblY);
                        translationptlt.Add(cpt);

                        CFrPtLtToExcel.Add(resultptlt[i]);
                        CToPtLtToExcel.Add(resultptlt[i].CorrespondingPtLt[j]);
                        intptnum = intptnum + 1;
                    }
                }

                //添加第一个元素
                List<double> dblTranslationLt = new List<double>();
                List<double> dblSumTranslationLt = new List<double>();
                dblTranslationLt.Add(0);
                dblSumTranslationLt.Add(0);
                double dblSumLenth = 0;
                for (int i = 1; i < translationptlt.Count; i++)
                {
                    double dblLength = CGeoFunc.CalDis(translationptlt[i - 1], translationptlt[i]);
                    dblTranslationLt.Add(dblLength);
                    dblSumLenth = dblSumLenth + dblLength;
                    dblSumTranslationLt.Add(dblSumLenth);
                }
                dblAllSumLength = dblAllSumLength + dblSumLenth;

                CParameterResult pParameterResultToExcel = new CParameterResult();
                pParameterResultToExcel.strEvaluationMethod = "Translation";
                pParameterResultToExcel.FromPtLt = CFrPtLtToExcel;
                pParameterResultToExcel.ToPtLt = CToPtLtToExcel;
                pParameterResultToExcel.TranlationPtLt = translationptlt;
                pParameterResultToExcel.dblTranslationLt = dblTranslationLt;
                pParameterResultToExcel.dblSumTranslationLt = dblSumTranslationLt;
                _pDataRecords.ParameterResultToExcelLt.Add(pParameterResultToExcel);
            }

            long lngEndTime = System.Environment.TickCount;
            long lngTime = lngEndTime - lngStartTime;
            tsslTime.Text = "TranslationRunning Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间
            tsslMessage.Text = "Translation计算完成！";

            return dblAllSumLength;
        }



        #endregion

        #region CalRatioTranslation
        /// <summary>计算Translation指标值，并保存输出</summary>
        /// <returns>Translation指标值</returns>
        /// <remarks>further weighted by the ratio</remarks>
        public double CalRatioTranslation()
        {
            //MessageBox.Show("wrong call in ctranslation!");

            CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            List<CPoint> resultptlt = ParameterResult.CResultPtLt;
            CParameterInitialize ParameterInitialize = _pDataRecords.ParameterInitialize;
            StatusStrip ststMain = ParameterInitialize.ststMain;
            ToolStripStatusLabel tsslTime = ParameterInitialize.tsslTime;
            ToolStripStatusLabel tsslMessage = ParameterInitialize.tsslMessage;
            ToolStripProgressBar tspbMain = ParameterInitialize.tspbMain;
            tsslMessage.Text = "正在计算Translation...";
            ststMain.Refresh();
            long lngStartTime = System.Environment.TickCount;

            //生成计算Translation指标值的线段
            List<CPoint> CFrPtLtToExcel = new List<CPoint>();
            List<CPoint> CToPtLtToExcel = new List<CPoint>();
            List<CPoint> translationptlt = new List<CPoint>();
            List<double> dblWeightlt = new List<double>();        //记录相应的权重值（注意：由于程序的需要，再后面的计算中，有意使得dblWeightlt[0]==0）
            double dblSumLength = _pDataRecords.ParameterResult.FromCpl.pPolyline.Length + _pDataRecords.ParameterResult.ToCpl.pPolyline.Length;



            int intptnum = 0;
            CPoint frlastcpt = resultptlt[0];
            CPoint tolastcpt = resultptlt[0].CorrespondingPtLt[0];
            for (int i = 0; i < resultptlt.Count; i++)
            {
                for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
                {
                    double dblX = resultptlt[i].CorrespondingPtLt[j].X - resultptlt[i].X;
                    double dblY = resultptlt[i].CorrespondingPtLt[j].Y - resultptlt[i].Y;
                    CPoint cpt = new CPoint(intptnum, dblX, dblY);
                    translationptlt.Add(cpt);

                    double dblLengthi = CGeoFunc.CalDis(resultptlt[i], frlastcpt);//计算权重值长度
                    double dblLengthj = CGeoFunc.CalDis(resultptlt[i].CorrespondingPtLt[j], tolastcpt);//计算权重值长度
                    double dblWeight = (dblLengthi + dblLengthj) / dblSumLength;
                    dblWeightlt.Add(dblWeight);
                    tolastcpt = resultptlt[i].CorrespondingPtLt[j];  //更新tolastcpt

                    CFrPtLtToExcel.Add(resultptlt[i]);
                    CToPtLtToExcel.Add(resultptlt[i].CorrespondingPtLt[j]);
                    intptnum = intptnum + 1;
                }
                frlastcpt = resultptlt[i];  //更新frlastcpt
                tspbMain.Value = (i + 1) * 50 / (resultptlt.Count);
            }

            //添加第一个元素
            List<double> dblTranslationLt = new List<double>();
            List<double> dblSumTranslationLt = new List<double>();
            dblTranslationLt.Add(0);
            dblSumTranslationLt.Add(0);
            double dblSumTranslation = 0;
            for (int i = 1; i < translationptlt.Count; i++)
            {
                double dblRatioLength = CGeoFunc.CalDis(translationptlt[i - 1], translationptlt[i]) * dblWeightlt[i];
                dblTranslationLt.Add(dblRatioLength);
                dblSumTranslation = dblSumTranslation + dblRatioLength;
                dblSumTranslationLt.Add(dblSumTranslation);

                tspbMain.Value = (i + 1) * 50 / (translationptlt.Count) + 50;
            }

            CParameterResult pParameterResultToExcel = new CParameterResult();
            pParameterResultToExcel.strEvaluationMethod = "Translation";
            pParameterResultToExcel.FromPtLt = CFrPtLtToExcel;
            pParameterResultToExcel.ToPtLt = CToPtLtToExcel;
            pParameterResultToExcel.TranlationPtLt = translationptlt;
            pParameterResultToExcel.dblTranslationLt = dblTranslationLt;
            pParameterResultToExcel.dblSumTranslationLt = dblSumTranslationLt;
            _pDataRecords.ParameterResultToExcel = pParameterResultToExcel;

            long lngEndTime = System.Environment.TickCount;
            long lngTime = lngEndTime - lngStartTime;
            tsslTime.Text = "TranslationRunning Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间
            tsslMessage.Text = "Translation计算完成！";

            return dblSumTranslation;
            //return 0;
        }

        /// <summary>计算RatioTranslation指标值</summary>
        /// <param name="resultptlt">对应结果数组</param>
        /// <returns>RatioTranslation指标值</returns>
        /// <remarks>已释放内存，further weighted by the ratio</remarks>
        public double CalRatioTranslation(List<CPoint> resultptlt, CPolyline frcpl, CPolyline tocpl)
        {

            //生成计算Translation指标值的线段
            int intptnum = 0;
            List<CPoint> translationptlt = new List<CPoint>();
            List<double> dblWeightlt = new List<double>();        //记录相应的权重值

            CPoint frlastcpt = resultptlt[0];                             //重新设定frlastcpt
            CPoint tolastcpt = resultptlt[0].CorrespondingPtLt[0];       //重新设定tolastcpt
            for (int i = 0; i < resultptlt.Count; i++)
            {
                for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
                {
                    double dblX = resultptlt[i].CorrespondingPtLt[j].X - resultptlt[i].X;
                    double dblY = resultptlt[i].CorrespondingPtLt[j].Y - resultptlt[i].Y;
                    CPoint cpt = new CPoint(intptnum, dblX, dblY);

                    double dblLengthi = CGeoFunc.CalDis(resultptlt[i], frlastcpt);//计算权重值长度
                    double dblLengthj = CGeoFunc.CalDis(resultptlt[i].CorrespondingPtLt[j], tolastcpt);//计算权重值长度
                    double dblWeight = dblLengthi + dblLengthj;
                    dblWeightlt.Add(dblWeight);
                    tolastcpt = resultptlt[i].CorrespondingPtLt[j];  //更新tolastcpt

                    intptnum = intptnum + 1;
                    translationptlt.Add(cpt);
                }
                frlastcpt = resultptlt[i];  //更新frlastcpt
            }

            //计算Translation指标值
            double dblSumRatioTranslation = 0;
            for (int i = 1; i < translationptlt.Count; i++)
            {
                double dblRatioLength = CGeoFunc.CalDis(translationptlt[i - 1], translationptlt[i]) * dblWeightlt[i];
                dblSumRatioTranslation += dblRatioLength;

                //translationptlt[i - 1].SetEmpty();  //释放内存
            }
            dblSumRatioTranslation = dblSumRatioTranslation / (frcpl.pPolyline.Length + tocpl.pPolyline.Length);

            return dblSumRatioTranslation;
        }


        /// <summary>计算多条对应线状要素的Translation指标值总和</summary>
        /// <returns>Translation指标值</returns>
        /// <remarks>further weighted by the ratio</remarks>
        public double CalRatioTranslations()
        {
            //CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            //List<List<CPoint>> resultptltlt = ParameterResult.CResultPtLtLt;
            //CParameterInitialize ParameterInitialize = _pDataRecords.ParameterInitialize;
            //StatusStrip ststMain = ParameterInitialize.ststMain;
            //ToolStripStatusLabel tsslTime = ParameterInitialize.tsslTime;
            //ToolStripStatusLabel tsslMessage = ParameterInitialize.tsslMessage;
            //ToolStripProgressBar tspbMain = ParameterInitialize.tspbMain;
            //tsslMessage.Text = "正在计算Translation...";
            //ststMain.Refresh();
            //long lngStartTime = System.Environment.TickCount;

            //double dblAllSumLength = 0;
            //for (int k = 0; k < resultptltlt.Count; k++)
            //{
            //    List<CPoint> resultptlt = resultptltlt[k];
            //    //生成计算Translation指标值的线段
            //    List<CPoint> CFrPtLtToExcel = new List<CPoint>();
            //    List<CPoint> CToPtLtToExcel = new List<CPoint>();
            //    List<CPoint> translationptlt = new List<CPoint>();
            //    int intptnum = 0;
            //    for (int i = 0; i < resultptlt.Count; i++)
            //    {
            //        CPoint frlastcpt = resultptlt[0];
            //        CPoint tolastcpt = resultptlt[0].CorrespondingPtLt[0];
            //        for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
            //        {
            //            double dblX = resultptlt[i].CorrespondingPtLt[j].X - resultptlt[i].X;
            //            double dblY = resultptlt[i].CorrespondingPtLt[j].Y - resultptlt[i].Y;
            //            CPoint cpt = new CPoint(intptnum, dblX, dblY);
            //            translationptlt.Add(cpt);

            //            double dblLengthi = CGeoFunc.CalDis(resultptlt[i], frlastcpt);//计算权重值长度
            //            double dblLengthj = CGeoFunc.CalDis(resultptlt[i].CorrespondingPtLt[j], tolastcpt);//计算权重值长度
            //            double dblWeight = (dblLengthi + dblLengthj) / dblSumLength;
            //            dblWeightlt.Add(dblWeight);
            //            tolastcpt = resultptlt[i].CorrespondingPtLt[j];  //更新tolastcpt

            //            CFrPtLtToExcel.Add(resultptlt[i]);
            //            CToPtLtToExcel.Add(resultptlt[i].CorrespondingPtLt[j]);
            //            intptnum = intptnum + 1;
            //        }
            //        frlastcpt = resultptlt[i];  //更新frlastcpt
            //    }

            //    //添加第一个元素
            //    List<double> dblTranslationLt = new List<double>();
            //    List<double> dblSumTranslationLt = new List<double>();
            //    dblTranslationLt.Add(0);
            //    dblSumTranslationLt.Add(0);
            //    double dblSumLenth = 0;
            //    for (int i = 1; i < translationptlt.Count; i++)
            //    {
            //        double dblRatioLength = CGeoFunc.CalDis(translationptlt[i - 1], translationptlt[i]) * dblWeightlt[i];
            //        dblTranslationLt.Add(dblRatioLength);
            //        dblSumTranslation = dblSumTranslation + dblRatioLength;
            //        dblSumTranslationLt.Add(dblSumTranslation);
            //    }
            //    dblAllSumLength = dblAllSumLength + dblSumLenth;

            //    CParameterResult pParameterResultToExcel = new CParameterResult();
            //    pParameterResultToExcel.strEvaluationMethod = "Translation";
            //    pParameterResultToExcel.FromPtLt = CFrPtLtToExcel;
            //    pParameterResultToExcel.ToPtLt = CToPtLtToExcel;
            //    pParameterResultToExcel.TranlationPtLt = translationptlt;
            //    pParameterResultToExcel.dblTranslationLt = dblTranslationLt;
            //    pParameterResultToExcel.dblSumTranslationLt = dblSumTranslationLt;
            //    _pDataRecords.ParameterResultToExcelLt.Add(pParameterResultToExcel);
            //}

            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //tsslTime.Text = "TranslationRunning Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间
            //tsslMessage.Text = "Translation计算完成！";

            //return dblAllSumLength;
            return 0;
        }



        #endregion

       



 

      

    }
}
