using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
//using System.Windows.Forms .

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Output;

using MorphingClass.CGeometry;
using MorphingClass.CUtility;


namespace MorphingClass.CEvaluationMethods
{
    public class CIntegral
    {
        private CDataRecords _pDataRecords;
        
        private CParameterResult _pParameterResultToExcel = new CParameterResult();
        



        //private double _dbInterLSmallDis;
        //private double _dblVerySmall;
        //private double _intIntersectNum = 0;
        private object _Missing = Type.Missing;



        public CIntegral()
        {

        }



        public CIntegral(CDataRecords pDataRecords)
        {
            _pDataRecords = pDataRecords;
            _pDataRecords.ParameterResultToExcel = _pParameterResultToExcel;
            _pParameterResultToExcel.strEvaluationMethod = "Integral";

            //_pParameterResultToExcel.dblIntegralLt = new List<double>();
            //_pParameterResultToExcel.dbInterLSumIntegralLt = new List<double>();
            //_pParameterResultToExcel.FromPtLt = new List<CPoint>();
            //_pParameterResultToExcel.ToPtLt = new List<CPoint>();


        }


        public double CalIntegral()
        {
            MessageBox.Show("need to be improved!");


            //CParameterResult ParameterResult = _pDataRecords.ParameterResult;
            //CParameterInitialize ParameterInitialize=_pDataRecords.ParameterInitialize;
            //StatusStrip ststMain = ParameterInitialize.ststMain;
            //TooInterLStripStatusLabel tsslTime = ParameterInitialize.tsslTime;
            //TooInterLStripStatusLabel tsslMessage = ParameterInitialize.tsslMessage;
            //TooInterLStripProgressBar tspbMain = ParameterInitialize.tspbMain;
            //tsslMessage.Text = "正在计算Integral...";
            //tspbMain.Value = 0;
            //ststMain.Refresh();
            //long lngStartTime = System.Environment.TickCount;

            //List<CPoint> frptlt = ParameterResult.FromCpl .CptLt ;
            //List<CPoint> resultptlt = ParameterResult.CResultPtLt;
            //double dblMinDis = CGeometricMethods.CalDis(frptlt[0], frptlt[1]);
            //for (int i = 1; i < frptlt.Count - 1; i++)
            //{
            //    double dblDis = CGeometricMethods.CalDis(frptlt[i], frptlt[i + 1]);
            //    if (dblDis < dblMinDis) dblMinDis = dblDis;
            //}
            //double dbInterLSmallDis = dblMinDis / 10;
            //_dbInterLSmallDis = dbInterLSmallDis;


            //double dbInterLSumArea = 0;
            
            //List<CPoint> CFrPtLtToExcel = new List<CPoint>();
            //List<CPoint> CToPtLtToExcel = new List<CPoint>();
            //List<double> dblIntegralLt = new List<double>();
            //List<double> dbInterLSumIntegralLt = new List<double>();

            ////添加第一个元素
            //CFrPtLtToExcel.Add(resultptlt[0]);
            //CToPtLtToExcel.Add(resultptlt[0].CorrespondingPtLt [0]);
            //dblIntegralLt.Add(0);
            //dbInterLSumIntegralLt.Add(0);
            
            //for (int i = 0; i < resultptlt.Count - 1; i++)
            //{
                
            //    IPointCollection4 pCol0 = new PolygonClass();
            //    IPoint ipt0 = new PointClass();
            //    IPoint ipt2 = new PointClass();
            //    if (resultptlt[i].CorrespondingPtLt.Count > 1)
            //    {
            //        //fromcpl中的一个点对应tocpl中多个点的情形，直接计算各三角形面积
            //        int j = 0;
            //        for (j = 0; j < resultptlt[i].CorrespondingPtLt.Count - 1; j++)
            //        {

            //            IPointCollection4 pCol1 = new PolygonClass();
            //            //顺时针添加顶点
            //            pCol1.AddPoint(resultptlt[i]);
            //            pCol1.AddPoint(resultptlt[i].CorrespondingPtLt[j + 1]);
            //            pCol1.AddPoint(resultptlt[i].CorrespondingPtLt[j]);
            //            IArea pArea1 = (IArea)pCol1;
            //            //if (pArea1.Area < 0)
            //            //    throw new ArithmeticException("计算积分面积（三角形）时出现负值！");
            //            dbInterLSumArea = dbInterLSumArea + Math.Abs(pArea1.Area);
            //            CFrPtLtToExcel.Add(resultptlt[i]);
            //            CToPtLtToExcel.Add(resultptlt[i].CorrespondingPtLt[j + 1]);
            //            dblIntegralLt.Add(pArea1.Area);
            //            dbInterLSumIntegralLt.Add(dbInterLSumArea);

            //        }
            //        //将resultptlt[i]及其对应点序列中的最后一个对应点作为下一个四边形的两个顶点
            //        ipt0 = resultptlt[i];
            //        ipt2 = resultptlt[i].CorrespondingPtLt[resultptlt[i].CorrespondingPtLt.Count - 1];
            //    }
            //    eInterLSe if (resultptlt[i].CorrespondingPtLt.Count == 1)
            //    {
            //        //将resultptlt[i]及其对应点序列中的第一个对应点（也仅有一个）作为下一个四边形的两个顶点
            //        //fromcpl中的多个点对应tocpl中一个点的情形也包括在这里
            //        ipt0 = resultptlt[i];
            //        ipt2 = resultptlt[i].CorrespondingPtLt[0];
            //    }
            //    //将resultptlt[i + 1]及其第一个对应点作为下一个四边形的另外两个顶点
            //    double dblQuadrangleArea = CalIntegralAreaofAnyQuadrangle(ipt0, (IPoint)resultptlt[i + 1],
            //                                                              ipt2, (IPoint)resultptlt[i + 1].CorrespondingPtLt[0], dbInterLSmallDis);
            //    dbInterLSumArea = dbInterLSumArea + dblQuadrangleArea;

            //    CFrPtLtToExcel.Add(resultptlt[i + 1]);
            //    CToPtLtToExcel.Add(resultptlt[i + 1].CorrespondingPtLt[0]);
            //    dblIntegralLt.Add(dblQuadrangleArea);
            //    dbInterLSumIntegralLt.Add(dbInterLSumArea);

            //    tspbMain.Value = (i + 1) * 100 / (resultptlt.Count - 1);
            //}
            //_pParameterResultToExcel.FromPtLt = CFrPtLtToExcel;
            //_pParameterResultToExcel.ToPtLt = CToPtLtToExcel;
            //_pParameterResultToExcel.dblIntegralLt=dblIntegralLt;
            //_pParameterResultToExcel.dbInterLSumIntegralLt = dbInterLSumIntegralLt;

            //long lngEndTime = System.Environment.TickCount;
            //long lngTime = lngEndTime - lngStartTime;
            //tsslTime.Text = "IntegralRunning Time: " + Convert.ToString(lngTime) + "ms";  //显示运行时间
            //tsslMessage.Text = "Integral计算完成！";
            //return dbInterLSumArea;
            return 0;
        }

        /// <summary>计算Integral指标值</summary>
        /// <param name="resultptlt">对应结果数组</param>
        /// <returns>Integral指标值</returns>
        /// <remarks>已释放内存</remarks>
        public double CalIntegral(List<CPoint> resultptlt)
        {
            ////生成计算Integral指标值的线段
            //double dblIntegral = 0;
            //IPoint ifrpt = resultptlt[0];
            //IPoint itopt = resultptlt[0].CorrespondingPtLt[0];

            //for (int i = 0; i < resultptlt.Count; i++)
            //{
            //    //一对一、一对多、多对一都包含于此
            //    for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
            //    {
            //        dblIntegral += CaInterLSubIntegral(ifrpt, resultptlt[i], itopt, resultptlt[i].CorrespondingPtLt[j], _dbInterLSmallDis, _dblVerySmall);                    
            //        itopt = resultptlt[i].CorrespondingPtLt[j];
            //    }
            //    ifrpt = resultptlt[i];
            //}

            //return dblIntegral;
            return 0;
        }


        private double CaInterLSubIntegral(CPoint frfrcpt, CPoint frtocpt, CPoint tofrcpt, CPoint totocpt, CPoint StandardVectorCpt, double dbInterLSmallDis, double dblVerySamll)
        {
            CPoint newfrfrcpt = new CPoint(0, frfrcpt.X + StandardVectorCpt.X, frfrcpt.Y + StandardVectorCpt.Y);
            CPoint newfrtocpt = new CPoint(0, frtocpt.X + StandardVectorCpt.X, frtocpt.Y + StandardVectorCpt.Y);

            CEdge frcedge = new CEdge(newfrfrcpt, newfrtocpt);
            CEdge tocedge = new CEdge(tofrcpt, totocpt);
            frcedge.SetLength();
            tocedge.SetLength();


            if (CCompareMethods.CompareCEdgeCoordinates(frcedge, tocedge)==0 ||
               (frcedge.dblLength == 0 && tocedge.dblLength == 0))   //为了应付刚开始时有重合的对应点
            {
                return 0;
            }


            double dblLength = frcedge.dblLength;
            if (frcedge.dblLength < tocedge.dblLength)
            {
                dblLength = tocedge.dblLength;
            }

            int intSegmentNum = Convert.ToInt32(dblLength / dbInterLSmallDis) + 1;
            double frlength = frcedge.dblLength / intSegmentNum;
            double tolength = tocedge.dblLength / intSegmentNum;

            //梯形面积（因为所有的上底和下底都相同，因此可以先将各个梯形的高相加，再在循环外乘以高、除以2）
            double dbledgelength = 0;
            double dblRatio = 1 / Convert.ToDouble(intSegmentNum);
            double dblCurrentRatio = 0;
            for (int k = 0; k < intSegmentNum; k++)
            {
                double dblfrx = (1 - dblCurrentRatio) * newfrfrcpt.X + dblCurrentRatio * newfrtocpt.X;
                double dblfry = (1 - dblCurrentRatio) * newfrfrcpt.Y + dblCurrentRatio * newfrtocpt.Y;
                double dbltox = (1 - dblCurrentRatio) * tofrcpt.X + dblCurrentRatio * totocpt.X;
                double dbltoy = (1 - dblCurrentRatio) * tofrcpt.Y + dblCurrentRatio * totocpt.Y;

                dbledgelength += CGeometricMethods.CalDis(dblfrx, dblfry, dbltox, dbltoy);
                dblCurrentRatio += dblRatio;
            }

            double dbInterLSubIntegral = dbledgelength * (frlength + tolength) / 2;
            return dbInterLSubIntegral;
        }

        /// <summary>计算RatioIntegral指标值</summary>
        /// <param name="resultptlt">对应结果数组</param>
        /// <returns>RatioIntegral指标值</returns>
        /// <remarks></remarks>
        public double CalRatioIntegral(List<CPoint> resultptlt,CPolyline frcpl,CPolyline tocpl)
        {
            //CGeometricMethods.CalDistanceParameters(frcpl);
            //

            ////生成计算Integral指标值的线段
            //double dbInterLSumRatioIntegral = 0;
            //CPoint frlastcpt = resultptlt[0];
            //CPoint tolastcpt = resultptlt[0].CorrespondingPtLt[0];

            ////StandardVectorCpt
            //double dblX = tocpl.CptLt[0].X - frcpl.CptLt[0].X;
            //double dblY = tocpl.CptLt[0].Y - frcpl.CptLt[0].Y;
            //CPoint StandardVectorCpt = new CPoint(0, dblX, dblY);

            //for (int i = 0; i < resultptlt.Count; i++)
            //{
            //    double dblfrlength = CGeometricMethods.CalDis(resultptlt[i], frlastcpt);//计算权重值长度
            //    //一对一、一对多、多对一都包含于此
            //    for (int j = 0; j < resultptlt[i].CorrespondingPtLt.Count; j++)
            //    {
            //        double dbltolength = CGeometricMethods.CalDis(resultptlt[i].CorrespondingPtLt[j], tolastcpt);//计算权重值长度
            //        double dbInterLSubintegral = CaInterLSubIntegral(frlastcpt, resultptlt[i], tolastcpt, resultptlt[i].CorrespondingPtLt[j],StandardVectorCpt, dbInterLSmallDis, dblVerySmall);
            //        dbInterLSumRatioIntegral = dbInterLSumRatioIntegral + dbInterLSubintegral * (dblfrlength + dbltolength);

            //        tolastcpt = resultptlt[i].CorrespondingPtLt[j];
            //    }
            //    frlastcpt = resultptlt[i];
            //}
            //dbInterLSumRatioIntegral = dbInterLSumRatioIntegral / (frcpl.pPolyline.Length + tocpl.pPolyline.Length);

            //return dbInterLSumRatioIntegral;
            return 0;
        }


        ///// <summary>Property:</summary>
        //public double dbInterLSmallDis
        //{
        //    get { return _dbInterLSmallDis; }
        //    set { _dbInterLSmallDis = value; }
        //}

        ///// <summary>Property: very small distance</summary>
        //public double dblVerySmall
        //{
        //    get { return _dblVerySmall; }
        //    set { _dblVerySmall = value; }
        //}

        
    }
}
