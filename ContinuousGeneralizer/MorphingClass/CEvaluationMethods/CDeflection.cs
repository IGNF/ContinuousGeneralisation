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


    public class CDeflection
    {
        private CDataRecords _pDataRecords;
        
        private List<CParameterResult> _pParameterResultToExcelLt = new List<CParameterResult>();


        public CDeflection()
        {

        }

        public CDeflection(CDataRecords pDataRecords)
        {
            _pDataRecords = pDataRecords;
            //_pDataRecords.ParameterResultToExcel = _pParameterResultToExcel;
            //_pDataRecords.ParameterResultToExcelLt = _pParameterResultToExcelLt;
            //_pParameterResultToExcel.strEvaluationMethod = "Deflection";
        }


        #region CalDeflection

        /// <summary>计算Deflection指标值，并保存输出</summary>
        /// <returns>Deflection指标值</returns>
        /// <remarks></remarks>
        public double CalDeflection()
        {
            //CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            //List<CPoint> resultptlt = ParameterResult.CResultPtLt;
            //CPolyline frcpl = ParameterResult.FromCpl ;
            //CParameterInitialize ParameterInitialize = _pDataRecords.ParameterInitialize;
            //StatusStrip ststMain = ParameterInitialize.ststMain;
            //ToolStripStatusLabel tsslTime = ParameterInitialize.tsslTime;
            //ToolStripStatusLabel tsslMessage = ParameterInitialize.tsslMessage;
            //ToolStripProgressBar tspbMain = ParameterInitialize.tspbMain;
            //tsslMessage.Text = "正在计算Deflection...";
            //ststMain.Refresh();
            //long lngStartTime = System.Environment.TickCount;


            ////计算标准向量
            //double dblStandardX = resultptlt[0].CorrespondingPtLt[0].X - resultptlt[0].X;
            //double dblStandardY = resultptlt[0].CorrespondingPtLt[0].Y - resultptlt[0].Y;
            //CPoint StandardVectorCpt = new CPoint(0, dblStandardX, dblStandardY);

            ////CGeoFunc.CalDistanceParameters(frcpl);
            ////

            ////生成计算Deflection指标值的线段
            //List<CPoint> CFrPtLtToExcel = new List<CPoint>();
            //List<CPoint> CToPtLtToExcel = new List<CPoint>();
            //List<CPoint> Deflectionptlt = new List<CPoint>();
            //List<double> dblDeflectionLt = new List<double>();
            //List<double> dblSumDeflectionLt = new List<double>();

            //int intptnum = 0;
            //double dblSumDeflection = 0;
            //CPoint cfrpt = resultptlt[0];
            //CPoint ctopt = resultptlt[0].CorrespondingPtLt[0];


            //for (int i = 0; i < resultptlt.Count; i++)
            //{
            //    for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
            //    {
            //        double dblX = resultptlt[i].CorrespondingPtLt[j].X - resultptlt[i].X - StandardVectorCpt.X;
            //        double dblY = resultptlt[i].CorrespondingPtLt[j].Y - resultptlt[i].Y - StandardVectorCpt.Y;
            //        CPoint cpt = new CPoint(intptnum, dblX, dblY);
            //        Deflectionptlt.Add(cpt);
            //        CFrPtLtToExcel.Add(resultptlt[i]);
            //        CToPtLtToExcel.Add(resultptlt[i].CorrespondingPtLt[j]);

            //        double dblDeflection = CalSubDeflection(cfrpt, resultptlt[i], ctopt, resultptlt[i].CorrespondingPtLt[j], StandardVectorCpt, dblSmallDis, dblVerySmall);
            //        dblDeflectionLt.Add(dblDeflection);
            //        dblSumDeflection += dblDeflection;
            //        dblSumDeflectionLt.Add(dblSumDeflection);

            //        ctopt = resultptlt[i].CorrespondingPtLt[j];
            //        intptnum = intptnum + 1;
            //    }
            //    cfrpt = resultptlt[i];
            //    tspbMain.Value = (i + 1) / (resultptlt.Count);
            //}

            //CParameterResult pParameterResultToExcel = new CParameterResult();
            //pParameterResultToExcel.strEvaluationMethod = "Deflection";
            //pParameterResultToExcel.FromPtLt = CFrPtLtToExcel;
            //pParameterResultToExcel.ToPtLt = CToPtLtToExcel;
            //pParameterResultToExcel.TranlationPtLt = Deflectionptlt;
            //pParameterResultToExcel.dblTranslationLt  = dblDeflectionLt;
            //pParameterResultToExcel.dblSumTranslationLt = dblSumDeflectionLt;
            //_pDataRecords.ParameterResultToExcel = pParameterResultToExcel;

            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //tsslTime.Text = "DeflectionRunning Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间
            //tsslMessage.Text = "Deflection计算完成！";

            //return dblSumDeflection;
            return 0;
        }


        /// <summary>计算Deflection指标值</summary>
        /// <param name="resultptlt">对应结果数组</param>
        /// <returns>Deflection指标值</returns>
        /// <remarks></remarks>
        public double CalDeflection(List<CPoint> resultptlt, CPoint StandardVectorCpt, double dblSmallDis, double dblVerySmall)
        {            
            //double dblDeflection = 0;
            //CPoint cfrpt = resultptlt[0];
            //CPoint ctopt = resultptlt[0].CorrespondingPtLt[0];

            //for (int i = 0; i < resultptlt.Count; i++)
            //{
            //    //一对一、一对多、多对一都包含于此
            //    for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
            //    {
            //        dblDeflection += CalSubDeflection(cfrpt, resultptlt[i], ctopt, resultptlt[i].CorrespondingPtLt[j], StandardVectorCpt, dblSmallDis, dblVerySmall);
            //        ctopt = resultptlt[i].CorrespondingPtLt[j];
            //    }
            //    cfrpt = resultptlt[i];
            //}

            //return dblDeflection;
            return 0;
        }


        ///// <summary>计算Deflection指标值</summary>
        ///// <param name="resultptlt">对应结果数组</param>
        ///// <returns>Deflection指标值</returns>
        ///// <remarks></remarks>
        //public double CalDeflection(List<CCorrCpts> pCorrCptsLt, CPoint StandardVectorCpt, double dblSmallDis, double dblVerySmall)
        //{
        //    ////生成计算Deflection指标值的线段
        //    //double dblDeflection = 0;
        //    //for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
        //    //{
        //    //    dblDeflection += CalSubDeflection(pCorrCptsLt[i].FrCpt, pCorrCptsLt[i + 1].FrCpt, pCorrCptsLt[i].ToCpt, pCorrCptsLt[i + 1].ToCpt, StandardVectorCpt, dblSmallDis, dblVerySmall);
        //    //}
        //    //return dblDeflection;
        //    return 0;
        //}

        /// <summary>计算Deflection指标值</summary>
        /// <param name="resultptlt">对应结果数组</param>
        /// <returns>Deflection指标值</returns>
        /// <remarks>已释放内存</remarks> 
        private double CalSubDeflection(CPoint frfrcpt, CPoint frtocpt, CPoint tofrcpt, CPoint totocpt, CPoint StandardVectorCpt, double dblSmallDis, double dblVerySamll)
        {
            //CPoint newfrfrcpt = new CPoint(0, frfrcpt.X + StandardVectorCpt.X, frfrcpt.Y + StandardVectorCpt.Y);
            //CPoint newfrtocpt = new CPoint(0, frtocpt.X + StandardVectorCpt.X, frtocpt.Y + StandardVectorCpt.Y);

            //CEdge frcedge = new CEdge(newfrfrcpt, newfrtocpt);
            //CEdge tocedge = new CEdge(tofrcpt, totocpt);
            //frcedge.SetLength();
            //tocedge.SetLength();

            //if (frcedge.Equals(tocedge, dblVerySamll) ||
            //   (frcedge.Length == 0 && tocedge.Length == 0))   //为了应付刚开始时有重合的对应点
            //{
            //    return 0;
            //}

            //double dblLength = Math.Max(frcedge.Length, tocedge.Length);            

            //int intSegmentNum = Convert.ToInt32(dblLength / dblSmallDis) + 1;
            //double frlength = frcedge.Length / intSegmentNum;
            //double tolength = tocedge.Length / intSegmentNum;

            ////梯形面积（因为所有的上底和下底都相同，因此可以先将各个梯形的高相加，再在循环外乘以高、除以2）
            //double dbledgelength = 0;
            //double dblRatio = 1/ Convert.ToDouble(intSegmentNum);
            //double dblCurrentRatio = 0;
            //for (int k = 0; k < intSegmentNum; k++)
            //{
            //    double dblfrx = (1 - dblCurrentRatio) * newfrfrcpt.X + dblCurrentRatio * newfrtocpt.X;
            //    double dblfry = (1 - dblCurrentRatio) * newfrfrcpt.Y + dblCurrentRatio * newfrtocpt.Y;
            //    double dbltox = (1 - dblCurrentRatio) * tofrcpt.X + dblCurrentRatio * totocpt.X;
            //    double dbltoy = (1 - dblCurrentRatio) * tofrcpt.Y + dblCurrentRatio * totocpt.Y;
                
            //    dbledgelength += CGeoFunc.CalDis(dblfrx, dblfry, dbltox, dbltoy);
            //    dblCurrentRatio += dblRatio;
            //}

            //double dblSubDeflection = dbledgelength * (frlength + tolength) / 2;
            //return dblSubDeflection;

            return 0;
        }


        /// <summary>Calculate Deflection values for a List<List<CCorrCpts>> which has been recorded in ParameterResult</summary>
        /// <remarks></remarks>
        public double CalDeflectionCorr()
        {
            //CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            //List<List<CCorrCpts>> CorrCptsLtLt = ParameterResult.pMorphingBase.CorrCptsLtLt;
            //List<List<double>> dblDeflectionLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            //List<List<double>> dblSumDeflectionLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            //double dblSUM = CalDeflectionCorr(CorrCptsLtLt, dblDeflectionLtLt, dblSumDeflectionLtLt);

            //ParameterResult.strEvaluationMethod = "Deflection";
            //ParameterResult.dblDeflectionLtLt = dblDeflectionLtLt;
            //ParameterResult.dblSumDeflectionLtLt = dblSumDeflectionLtLt;

            //return dblSUM;
            return 0;
        }

        public double CalDeflectionCorr(List<List<CCorrCpts>> CorrCptsLtLt, List<List<double>> dblDeflectionLtLt = null, List<List<double>> dblSumDeflectionLtLt = null)
        {
            //if (dblDeflectionLtLt == null)
            //{
            //    dblDeflectionLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            //}
            //if (dblSumDeflectionLtLt == null)
            //{
            //    dblSumDeflectionLtLt = new List<List<double>>(CorrCptsLtLt.Count);
            //}

            //double dblSUM = 0;
            //foreach (LinkedList<CCorrCpts> CorrCptsLt in CorrCptsLtLt)
            //{
            //    List<double> dblDeflectionLt = new List<double>(CorrCptsLt.Count);
            //    List<double> dblSumDeflectionLt = new List<double>(CorrCptsLt.Count);

            //    CalDeflectionCorr(CorrCptsLt, dblDeflectionLt, dblSumDeflectionLt);
            //    dblDeflectionLtLt.Add(dblDeflectionLt);
            //    dblSumDeflectionLtLt.Add(dblSumDeflectionLt);
            //    dblSUM += dblSumDeflectionLt[dblSumDeflectionLt.Count - 1];
            //}

            //return dblSUM;

            return 0;
        }

        public double CalDeflectionCorr(LinkedList<CCorrCpts> CorrCptsLt, List<double> dblDeflectionLt = null, List<double> dblSumDeflectionLt = null)
        {
            //if (dblDeflectionLt == null)
            //{
            //    dblDeflectionLt = new List<double>(CorrCptsLt.Count);
            //}
            //if (dblSumDeflectionLt == null)
            //{
            //    dblSumDeflectionLt = new List<double>(CorrCptsLt.Count);
            //}

            //double dblSumLenth = 0;
            //dblDeflectionLt.Add(0);
            //dblSumDeflectionLt.Add(0);
            //LinkedListNode<CCorrCpts> LastCorrCpt = CorrCptsLt.First;
            //LinkedListNode<CCorrCpts> CurrentCorrCpt = LastCorrCpt.Next;

            //while (CurrentCorrCpt != null)
            //{
            //    double dblLength = LastCorrCpt.Value.pMoveVector.DistanceTo(CurrentCorrCpt.Value.pMoveVector);
            //    dblSumLenth += dblLength;
            //    dblDeflectionLt.Add(dblLength);
            //    dblSumDeflectionLt.Add(dblSumLenth);

            //    LastCorrCpt = CurrentCorrCpt;
            //    CurrentCorrCpt = CurrentCorrCpt.Next;
            //}
            //return dblSumLenth;

            return 0;
        }
        #endregion
    }
}
