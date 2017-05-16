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

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmBezierDetectPointfarthest : Form
    {
        private CFrmOperation _FrmOperation;
        
        CDataRecords _DataRecords;


        public FrmBezierDetectPointfarthest()
        {
            InitializeComponent();
        }

        public FrmBezierDetectPointfarthest(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmBezierDetectPointfarthest_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLayer = this.cboLayer;
            ParameterInitialize.txtError = this.txtError;

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoad();
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            string strSelectedLayer = this.cboLayer.Text;
            IFeatureLayer pFeatureLayer = null;

            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            try
            {
                for (int i = 0; i < ParameterInitialize.m_mapFeature.LayerCount; i++)
                {
                    if (strSelectedLayer == ParameterInitialize.m_mapFeature.get_Layer(i).Name)
                    {
                        pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(i);
                    }
                }
 
            }
            catch (Exception)
            {
                MessageBox.Show("请选择要素图层！");
                return;
            }

            //弹出保存对话框
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            string strPath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strPath);
            double dblError = Convert.ToDouble(ParameterInitialize.txtError.Text);

            long lngStartTime = System.Environment.TickCount; //记录开始时间

            //读取线数据
            List<CPolyline> CPolylineLt = CHelpFunc.GetCPlLtByFeatureLayer(pFeatureLayer);
            List<CPolyline> crtpllt = new List<CPolyline>();     
            for (int i = 0; i < CPolylineLt.Count ; i++)
            {
                List<CPoint> cptlt = CPolylineLt[i].CptLt;
                CPolyline crtpl = GetBezierDetectedCpl(cptlt, dblError);
                crtpllt.Add(crtpl);
            }

            CHelpFunc.SaveCPlLt(crtpllt, "BezierDetectedPl", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);


            long lngEndTime = System.Environment.TickCount;//记录结束时间
            _DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时
        }

        private CPolyline GetBezierDetectedCpl(List<CPoint> cptlt, double dblErrorDis)
        {
            double dblMinDis = FindMinLength(cptlt);
            double dblSmallDis = dblMinDis / 4;


            List<CPoint> crtptlt = BezierDetectPoint(cptlt, dblSmallDis, dblErrorDis);
            CPolyline crtpl = new CPolyline(0, crtptlt);
            return crtpl;
        }


        private double FindMinLength(List<CPoint> cptlt)
        {
            double dblMinDis = double.MaxValue;
            for (int i = 0; i < cptlt.Count - 1; i++)
            {
                double dblDis = CGeoFunc.CalDis(cptlt[i], cptlt[i + 1]);
                if (dblDis < dblMinDis)
                {
                    dblMinDis = dblDis;
                }
            }
            return dblMinDis;

        }


        private List<CPoint> BezierDetectPoint(List<CPoint> cptlt, double dblSmallDis, double dblErrorDis)
        {
            List<CPoint> crtptlt = new List<CPoint>();  //crtptlt:characteristicptlt
            crtptlt.Add(cptlt[0]);



            //List<CPoint> beziercptlt = new List<CPoint>();
            IPointCollection4 pOriginalCol = new PolylineClass();
            object Missing = Type.Missing;
            bool blnNew = true;
            for (int i = 1; i < cptlt.Count - 1; i++)
            {
                if (blnNew == true)
                {
                    pOriginalCol =new PolylineClass();
                    blnNew = false;
                    pOriginalCol.AddPoint((IPoint)cptlt[i - 1], ref Missing, ref Missing);
                    pOriginalCol.AddPoint((IPoint)cptlt[i], ref Missing, ref Missing);
                    pOriginalCol.AddPoint((IPoint)cptlt[i + 1], ref Missing, ref Missing);

                    //beziercptlt.Add(cptlt[i - 1]);
                    //beziercptlt.Add(cptlt[i ]);
                    //beziercptlt.Add(cptlt[i + 1]);
                    i = i + 1;
                }
                else
                {
                    pOriginalCol.AddPoint((IPoint)cptlt[i], ref Missing, ref Missing);
                    //beziercptlt.Add(cptlt[i]);  
                }

                IPolyline5 pOriginalpl = pOriginalCol as IPolyline5;
                double dblBuBeLength = pOriginalpl.Length / 3;  //dblBuBeLength: dblBuildBezierLength
                int intPointCount = pOriginalCol.PointCount;

                //确定始端控制点2
                IPointCollection4 pfrCol = new PolylineClass();
                pfrCol.AddPoint(pOriginalCol.get_Point(0), ref Missing, ref Missing);
                pfrCol.AddPoint(pOriginalCol.get_Point(1), ref Missing, ref Missing);
                IPolyline5 pfrpl = pfrCol as IPolyline5;
                IPoint froutpt = new PointClass();   //始端控制点2
                pfrpl.QueryPoint(esriSegmentExtension.esriExtendAtTo, dblBuBeLength, false, froutpt);

                //确定终端控制点2
                IPointCollection4 ptoCol = new PolylineClass();
                ptoCol.AddPoint(pOriginalCol.get_Point(intPointCount - 1), ref Missing, ref Missing);
                ptoCol.AddPoint(pOriginalCol.get_Point(intPointCount - 2), ref Missing, ref Missing);
                IPolyline5 ptopl = ptoCol as IPolyline5;
                IPoint tooutpt = new PointClass();   //终端控制点2
                ptopl.QueryPoint(esriSegmentExtension.esriExtendAtTo, dblBuBeLength, false, tooutpt);


                //创建BezierCurve
                IBezierCurve pBezierCurve = new BezierCurveClass();
                pBezierCurve.PutCoord(0, pOriginalCol.get_Point(0));
                pBezierCurve.PutCoord(1, froutpt);
                pBezierCurve.PutCoord(2, tooutpt);
                pBezierCurve.PutCoord(3, pOriginalCol.get_Point(intPointCount - 1));
                double dblMaxDis = 0;
                int intMaxIndex = 0;
                for (int j = 1; j < pOriginalCol.PointCount -1; j++)
                {
                    IPoint Originalpt=pOriginalCol.get_Point (j);
                    IPoint outpt=new PointClass ();
                    double dblAlongDis=new double ();
                    double dblFromDis=new double ();
                    bool blnRight=new bool ();

                    IPoint bezierpt = new PointClass();
                    pBezierCurve.QueryPointAndDistance(esriSegmentExtension.esriNoExtension, Originalpt, false, outpt, ref dblAlongDis, ref dblFromDis,ref blnRight);
                    if (dblFromDis > dblMaxDis)
                    {
                        dblMaxDis = dblFromDis;
                        intMaxIndex = i + j + 1 - pOriginalCol.PointCount;  //i为原始线段上的点号，j为当前顾及线段的点号，pOriginalCol.PointCount为当前顾及线段的点数
                    }
                }

                if (dblMaxDis > dblErrorDis)
                {
                    blnNew = true;
                    crtptlt.Add(cptlt[intMaxIndex]);
                    i = intMaxIndex + 1;
                }
            }



            crtptlt.Add(cptlt[cptlt.Count - 1]);
            return crtptlt;

        }



    }
}