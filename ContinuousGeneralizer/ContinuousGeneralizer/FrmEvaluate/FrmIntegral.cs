using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

using ContinuousGeneralizer;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmEvaluate
{
    public partial class FrmIntegral : Form
    {
        private CDataRecords _pDataRecords;
        
        private double _dblSmallDis;

        public FrmIntegral(CDataRecords pDataRecords)
        {
            _pDataRecords = pDataRecords;
            InitializeComponent();
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //CParameterResult ParameterResult = _pDataRecords.ParameterResult;

            //List<CPoint> frptlt = ParameterResult.FromPtLt;
            //List<CPoint> resultptlt = ParameterResult.CResultPtLt;

            //double dblMinDis = CGeoFunc.CalDis(frptlt[0], frptlt[1]);
            //for (int i = 1; i < frptlt.Count - 1; i++)
            //{
            //    double dblDis = CGeoFunc.CalDis(frptlt[i], frptlt[i + 1]);
            //    if (dblDis < dblMinDis) dblMinDis = dblDis;
            //}
            //double dblSmallDis = dblMinDis / 10;
            //_dblSmallDis = dblSmallDis;


            //double dblSumArea = 0;
            //object Missing = Type.Missing;
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
            //            pCol1.AddPoint(resultptlt[i], ref Missing, ref Missing);
            //            pCol1.AddPoint(resultptlt[i].CorrespondingPtLt[j + 1], ref Missing, ref Missing);
            //            pCol1.AddPoint(resultptlt[i].CorrespondingPtLt[j], ref Missing, ref Missing);
            //            IArea pArea1 = (IArea)pCol1;
            //            if (pArea1.Area < 0)
            //                throw new ArithmeticException("计算积分面积（三角形）时出现负值！");
            //            dblSumArea = dblSumArea + Math.Abs(pArea1.Area);
            //        }
            //        //将resultptlt[i]及其对应点序列中的最后一个对应点最为下一个四边形的两个顶点
            //        ipt0 = resultptlt[i];
            //        ipt2 = resultptlt[i].CorrespondingPtLt[resultptlt[i].CorrespondingPtLt.Count - 1];
            //    }
            //    else if (resultptlt[i].CorrespondingPtLt.Count == 1)
            //    {                    
            //        //将resultptlt[i]及其对应点序列中的第一个对应点（也仅有一个）最为下一个四边形的两个顶点
            //        //fromcpl中的多个点对应tocpl中一个点的情形也包括在这里
            //        ipt0 = resultptlt[i];
            //        ipt2 = resultptlt[i].CorrespondingPtLt[0];
            //    }
            //    //将resultptlt[i + 1]及其第一个对应点作为下一个四边形的另外两个顶点
            //    double dblQuadrangleArea = CalIntegralAreaofAnyQuadrangle(ipt0, (IPoint)resultptlt[i + 1],
            //                                                              ipt2, (IPoint)resultptlt[i + 1].CorrespondingPtLt[0], dblSmallDis);
            //    dblSumArea = dblSumArea + Math.Abs(dblQuadrangleArea);
            //}

            //this.txtIntegral.Text = dblSumArea.ToString();





        }


        //private double CalIntegralAreaofAnyQuadrangle(IPoint ifrcplfrpt, IPoint ifrcpltopt, IPoint itocplfrpt, IPoint itocpltopt, double dblSmallDis)
        //{
        //    double dblQuadrangleArea = 0;
        //    CEdge frcedge = new CEdge(ifrcplfrpt, ifrcpltopt);
        //    CEdge tocedge = new CEdge(itocplfrpt, itocpltopt);

        //    int intSegmentNum = Convert.ToInt32(frcedge.Length / dblSmallDis);
        //    object Missing = Type.Missing;
        //    IPoint ipt0 = ifrcplfrpt;
        //    IPoint ipt2 = itocplfrpt;
        //    for (int k = 0; k < intSegmentNum; k++)
        //    {
        //        double dblRatio = (k + 1) / Convert.ToDouble(intSegmentNum);
        //        IPoint ipt1 = new PointClass();
        //        frcedge.QueryPoint(esriSegmentExtension.esriNoExtension, dblRatio, true, ipt1);
        //        IPoint ipt3 = new PointClass();
        //        tocedge.QueryPoint(esriSegmentExtension.esriNoExtension, dblRatio, true, ipt3);

        //        IPointCollection4 pCol = new PolygonClass();
        //        //顺时针添加顶点
        //        pCol.AddPoint(ipt0, ref Missing, ref Missing);
        //        pCol.AddPoint(ipt1, ref Missing, ref Missing);
        //        pCol.AddPoint(ipt3, ref Missing, ref Missing);
        //        pCol.AddPoint(ipt2, ref Missing, ref Missing);
        //        IArea pArea = (IArea)pCol;
        //        if (pArea.Area < 0)
        //            throw new ArithmeticException("计算积分面积（三角形）时出现负值！");
        //        dblQuadrangleArea = dblQuadrangleArea + Math.Abs(pArea.Area);

        //        ipt0 = ipt1;
        //        ipt2 = ipt3;
        //    }
        //    return dblQuadrangleArea;
        //}



    }
}