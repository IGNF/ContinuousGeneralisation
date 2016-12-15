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


    public class CLengthDiff
    {
        private CDataRecords _pDataRecords;
        
        private List<CParameterResult> _pParameterResultToExcelLt = new List<CParameterResult>();
        //private double _dblSumLength = 0;
        //private double _intIntersectNum = 0;

        public CLengthDiff()
        {

        }


        public CLengthDiff(CDataRecords pDataRecords)
        {
            _pDataRecords = pDataRecords;
            //_pDataRecords.ParameterResultToExcel = _pParameterResultToExcel;
            //_pDataRecords.ParameterResultToExcelLt = _pParameterResultToExcelLt;
            //_pParameterResultToExcel.strEvaluationMethod = "LengthDiff";
        }


        /// <summary>计算LengthDiff指标值，并保存输出</summary>
        /// <returns>LengthDiff指标值</returns>
        /// <remarks></remarks>
        public double CalLengthDiff()
        {
            MessageBox.Show("wrong call in clengthdiff!");
            //CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            //List<CPoint> resultptlt = ParameterResult.CResultPtLt;
            //CParameterInitialize ParameterInitialize = _pDataRecords.ParameterInitialize;
            //StatusStrip ststMain = ParameterInitialize.ststMain;
            //ToolStripStatusLabel tsslTime = ParameterInitialize.tsslTime;
            //ToolStripStatusLabel tsslMessage = ParameterInitialize.tsslMessage;
            //ToolStripProgressBar tspbMain = ParameterInitialize.tspbMain;
            //tsslMessage.Text = "正在计算LengthDiff...";
            //ststMain.Refresh();
            //long lngStartTime = System.Environment.TickCount;

            ////生成计算LengthDiff指标值的线段
            //List<CPoint> CFrPtLtToExcel = new List<CPoint>();
            //List<CPoint> CToPtLtToExcel = new List<CPoint>();
            //List<CPoint> LengthDiffptlt = new List<CPoint>();
            //int intptnum = 0;
            //for (int i = 0; i < resultptlt.Count; i++)
            //{
            //    for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
            //    {
            //        double dblX = resultptlt[i].CorrespondingPtLt[j].X - resultptlt[i].X;
            //        double dblY = resultptlt[i].CorrespondingPtLt[j].Y - resultptlt[i].Y;
            //        CPoint cpt = new CPoint(intptnum, dblX, dblY);
            //        LengthDiffptlt.Add(cpt);

            //        CFrPtLtToExcel.Add(resultptlt[i]);
            //        CToPtLtToExcel.Add(resultptlt[i].CorrespondingPtLt[j]);
            //        intptnum = intptnum + 1;
            //    }
            //    tspbMain.Value = (i + 1) * 50 / (resultptlt.Count);
            //}

            ////添加第一个元素
            //List<double> dblLengthDiffLt = new List<double>();
            //List<double> dblSumLengthDiffLt = new List<double>();
            //dblLengthDiffLt.Add(0);
            //dblSumLengthDiffLt.Add(0);
            //double dblSumLenth = 0;
            //for (int i = 1; i < LengthDiffptlt.Count; i++)
            //{
            //    double dblLength = CGeometricMethods.CalDis(LengthDiffptlt[i - 1], LengthDiffptlt[i]);
            //    dblLengthDiffLt.Add(dblLength);
            //    dblSumLenth = dblSumLenth + dblLength;
            //    dblSumLengthDiffLt.Add(dblSumLenth);

            //    tspbMain.Value = (i + 1) * 50 / (LengthDiffptlt.Count) + 50;
            //}

            //CParameterResult pParameterResultToExcel = new CParameterResult();
            //pParameterResultToExcel.strEvaluationMethod = "LengthDiff";
            //pParameterResultToExcel.FromPtLt = CFrPtLtToExcel;
            //pParameterResultToExcel.ToPtLt = CToPtLtToExcel;
            //pParameterResultToExcel.TranlationPtLt = LengthDiffptlt;
            //pParameterResultToExcel.dblLengthDiffLt = dblLengthDiffLt;
            //pParameterResultToExcel.dblSumLengthDiffLt = dblSumLengthDiffLt;
            //_pDataRecords.ParameterResultToExcel = pParameterResultToExcel;

            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //tsslTime.Text = "LengthDiffRunning Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间
            //tsslMessage.Text = "LengthDiff计算完成！";

            //return dblSumLenth;
            return 0;
        }


        /// <summary>计算LengthDiff指标值</summary>
        /// <param name="resultptlt">对应结果数组</param>
        /// <returns>LengthDiff指标值</returns>
        /// <remarks>已释放内存</remarks>
        public double CalLengthDiff(List<CPoint> resultptlt)
        {
            MessageBox.Show("wrong call in clengthdiff!");
            ////生成计算Integral指标值的线段
            //double dblSumLengthDiff = 0;
            //for (int i = 0; i < resultptlt.Count - 1; i++)
            //{

            //    IPointCollection4 pCol0 = new PolygonClass();
            //    CPoint cpt0 = new CPoint();
            //    CPoint cpt2 = new CPoint();
            //    if (resultptlt[i].CorrespondingPtLt.Count > 1)
            //    {
            //        //fromcpl中的一个点对应tocpl中多个点的情形，直接计算各三角形面积
            //        for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count - 1; j++)
            //        {
            //            double dblLengthDiff = CGeometricMethods.CalDis(resultptlt[i].CorrespondingPtLt[j], resultptlt[i].CorrespondingPtLt[j + 1]);
            //            dblSumLengthDiff += dblLengthDiff;
            //        }
            //        //将resultptlt[i]及其对应点序列中的最后一个对应点作为下一个四边形的两个顶点
            //        cpt0 = resultptlt[i];
            //        cpt2 = resultptlt[i].CorrespondingPtLt[resultptlt[i].CorrespondingPtLt.Count - 1];
            //    }
            //    else if (resultptlt[i].CorrespondingPtLt.Count == 1)
            //    {
            //        //将resultptlt[i]及其对应点序列中的第一个对应点（也仅有一个）作为下一个四边形的两个顶点
            //        //fromcpl中的多个点对应tocpl中一个点的情形也包括在这里
            //        cpt0 = resultptlt[i];
            //        cpt2 = resultptlt[i].CorrespondingPtLt[0];
            //    }

            //    double dblLength1 = CGeometricMethods.CalDis(cpt0, resultptlt[i + 1]);
            //    double dblLength2 = CGeometricMethods.CalDis(cpt2, resultptlt[i + 1].CorrespondingPtLt[0]);
            //    double dblLengthDiff2 = Math.Abs(dblLength1 - dblLength2);

            //    dblSumLengthDiff += dblLengthDiff2;
            //}

            //return dblSumLengthDiff;
            return 0;
        }


        /// <summary>计算RatioLengthDiff指标值</summary>
        /// <param name="resultptlt">对应结果数组</param>
        /// <returns>RatioLengthDiff指标值</returns>
        /// <remarks>further weighted by the ratio</remarks>
        public double CalRatioLengthDiff(List<CPoint> resultptlt, CPolyline frcpl, CPolyline tocpl)
        {
            double dblSumRatioLengthDiff = 0;
            CPoint frlastcpt = resultptlt[0];
            CPoint tolastcpt = resultptlt[0].CorrespondingPtLt[0];
            for (int i = 0; i < resultptlt.Count; i++)
            {
                double dblfrlength = CGeometricMethods.CalDis(resultptlt[i], frlastcpt);//计算权重值长度
                for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
                {
                    double dbltolength = CGeometricMethods.CalDis(resultptlt[i].CorrespondingPtLt[j], tolastcpt);//计算权重值长度
                    dblSumRatioLengthDiff = Math.Abs(dblfrlength - dbltolength) * (dblfrlength + dbltolength);

                    tolastcpt = resultptlt[i].CorrespondingPtLt[j];  //更新tolastcpt
                }
                frlastcpt = resultptlt[i];
            }

            dblSumRatioLengthDiff = dblSumRatioLengthDiff / (frcpl.pPolyline.Length + tocpl.pPolyline.Length);
            return dblSumRatioLengthDiff;
        }
 

      

    }
}
