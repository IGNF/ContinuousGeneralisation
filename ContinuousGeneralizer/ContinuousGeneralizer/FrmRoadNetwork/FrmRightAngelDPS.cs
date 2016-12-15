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
using MorphingClass.CGeneralizationMethods;
using ContinuousGeneralizer;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;
using MorphingClass.CCorrepondObjects;

namespace ContinuousGeneralizer.RoadNetwork
{
    public partial class FrmRightAngelDPS : Form
    {

        private CDataRecords _DataRecords;                    //数据记录
        private CFrmOperation _FrmOperation;
        //private CHelperFunction _HelperFunction = new CHelperFunction();
        //private CHelperFunctionExcel _HelperFunctionExcel = new CHelperFunctionExcel();
        //private CDPSimplification _pDPSimplification = new CDPSimplification();


        public FrmRightAngelDPS()
        {
            InitializeComponent();
        }
        public FrmRightAngelDPS(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmRightAngelDPS_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapPolyline = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLayer = this.cboLayer;

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoad();
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            ////获取当前选择的要素图层
            //IFeatureLayer pFeatureLayer = (IFeatureLayer)ParameterInitialize.m_mapAll.get_Layer(ParameterInitialize.cboLayer.Items.Count
            //                                                           - ParameterInitialize.cboLayer.SelectedIndex - 1);
            //ParameterInitialize.pFeatureLayer = pFeatureLayer;

            ////获取线数组
            //List<CPolyline> CPolylineLt = CHelperFunction.GetCPlLtByFeatureLayer(pFeatureLayer);

            ////建立文件夹，保存之后生成的要素图层

            //ParameterInitialize.strSavePath = txtPath.Text;
            //ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);

            //double dblDistanceThreshold = Convert.ToDouble(this.TxtDistanceThreshold.Text);
            //double dblProportion = Convert.ToDouble(this.TxtProportion.Text);
            //double dblAngelThreshold = Convert.ToDouble(this.TxtAngelThreshold.Text);
            ////输出生成的线状要素地图
            //List<CPolyline> CPLLt = new List<CPolyline>();
            //for (int i = 0; i < CPolylineLt.Count; i++)
            //{
            //    CPolyline cpl = new CPolyline(i, CPolylineLt[i].CptLt);
            //    CPLLt.Add(cpl);
            //}


            //double verysmall = CGeometricMethods.CalVerySmall(CPLLt);
            //this.Rebuild(ref CPLLt, verysmall);
            //List<CPolyline> CPLLT = new List<CPolyline>();
            //for (int i = 0; i < CPLLt.Count; i++)
            //{
            //    List<CPolyline> cpllt = new List<CPolyline>();
            //    RightAngelSimplification(CPLLt[i], ref cpllt, dblDistanceThreshold, verysmall);
            //    List<CPolyline> Csumpllt = new List<CPolyline>();
            //    for (int j = 0; j < cpllt.Count; j++)
            //    {
            //        _pDPSimplification.DivideCplByDP(cpllt[j], cpllt[j].pVirtualPolyline);
            //    }
            //    for (int j = 0; j < cpllt.Count; j++)
            //    {
            //        List<CPoint> newcptlt = new List<CPoint>();
            //        _pDPSimplification.RecursivelyGetNewCptLt(cpllt[j], cpllt[j].pVirtualPolyline, ref newcptlt, dblDistanceThreshold);
            //        List<CPolyline> Cpllt = new List<CPolyline>();
            //        Cpllt.Add(cpllt[j]);
            //        for (int k = 0; k < newcptlt.Count; k++)
            //        {
            //            int intnum = 0;
            //            for (int m = 0; m < Cpllt.Count; m++)
            //            {
            //                for (int s = 1; s < Cpllt[m].CptLt.Count - 1; s++)
            //                {
            //                    if (newcptlt[k].Equals2D(Cpllt[m].CptLt[s], verysmall))
            //                    {
            //                        List<CPoint> cptlt1 = new List<CPoint>();
            //                        List<CPoint> cptlt2 = new List<CPoint>();
            //                        for (int n = 0; n <= s; n++)
            //                        {
            //                            cptlt1.Add(Cpllt[m].CptLt[n]);
            //                        }
            //                        for (int n = s; n < Cpllt[m].CptLt.Count; n++)
            //                        {
            //                            cptlt2.Add(Cpllt[m].CptLt[n]);
            //                        }
            //                        Cpllt.RemoveAt(m);
            //                        CPolyline cpl1 = new CPolyline(Cpllt.Count + 1, cptlt1);
            //                        Cpllt.Add(cpl1);
            //                        CPolyline cpl2 = new CPolyline(Cpllt.Count + 2, cptlt2);
            //                        Cpllt.Add(cpl2);
            //                        intnum += 1;
            //                        break;
            //                    }
            //                }

            //                if (intnum == 1)
            //                {
            //                    break;
            //                }
            //            }
            //        }
            //        Csumpllt.AddRange(Cpllt);
            //    }
            //    //已经对线进行了顾及直角的DP分割，下一步将进行点的移动和线的重新连接    
            //    List<CPolyline> Cnewsumpllt = new List<CPolyline>();
            //    List<CPolyline> displaycpllt = new List<CPolyline>();
            //    for (int j = 0; j < Csumpllt.Count; j++)
            //    {
            //        List<CPoint> cptlt0 = new List<CPoint>();
            //        for (int k = 0; k < Csumpllt[j].CptLt.Count; k++)
            //        {
            //            CPoint cpt = new CPoint(k, (IPoint)Csumpllt[j].CptLt[k]);
            //            cptlt0.Add(cpt);
            //        }
            //        CPolyline cpl1 = new CPolyline(i, cptlt0);
            //        Cnewsumpllt.Add(cpl1);
            //    }
            //    for (int j = 0; j < Cnewsumpllt.Count; j++)
            //    {
            //        _pDPSimplification.DivideCplByDP(Cnewsumpllt[j], Cnewsumpllt[j].pVirtualPolyline);
            //        List<CPoint> newcptlt = new List<CPoint>();
            //        CEdge newBaseLine = new CEdge(Cnewsumpllt[j].CptLt[0], Cnewsumpllt[j].CptLt[Csumpllt[j].CptLt.Count - 1]);
            //        RecursivelyMovePt(Cnewsumpllt[j], Cnewsumpllt[j].pVirtualPolyline, ref newBaseLine, ref newcptlt, dblProportion);
            //        newcptlt.Insert(0, Cnewsumpllt[j].CptLt[0]);
            //        newcptlt.Add(Cnewsumpllt[j].CptLt[Csumpllt[j].CptLt.Count - 1]);
            //        CPolyline newcpl = new CPolyline(j, newcptlt);
            //        displaycpllt.Add(newcpl);
            //        //到这一步，本算法已经结束，下面只是使用Rebuild的方法连接起来所有的polyline
            //        this.Rebuild(ref displaycpllt, verysmall);

            //    }
            //    CPLLT.Add(displaycpllt[0]);

            //}
            //int intProportion = Convert.ToInt16(dblProportion * 255);

            //List<object> objlt = new List<object>();
            //for (int i = 0; i < CPLLT.Count; i++)
            //{
            //    objlt.Add(CPLLT[i]);
            //}
            //CSaveFeature pSaveFeature = new CSaveFeature(objlt,  "objlt", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl, esriGeometryType.esriGeometryPolyline, 255 - intProportion, 255 - intProportion, 255 - intProportion);
            //pSaveFeature.SaveFeaturesToLayer();

        }

        public void RightAngelSimplification(CPolyline cpl, ref List<CPolyline> Cpllt, double dblThreshold, double verysmall)
        {
            // step1 首先顾及直角，对于每一条polyline，遍历线的除首尾点以外的点，找出夹角为90°±5°的特殊顶点
            double dblAngelThreshold = Convert.ToDouble(this.TxtAngelThreshold.Text);
            List<CPoint> CptLt = new List<CPoint>();
            for (int i = 1; i < cpl.CptLt.Count - 1; i++)
            {
                List<CPoint> cptlt = new List<CPoint>();
                // Step2 求各个特殊顶点包括扩展边在内的临边，主要是求扩展边  
                if (Math.Abs(CGeometricMethods.CalAngle(cpl.CptLt[i - 1], cpl.CptLt[i], cpl.CptLt[i + 1]) - Math.PI / 2) <= dblAngelThreshold * Math.PI / 180)
                {
                    cptlt.Add((cpl.CptLt[i - 1]));
                    cptlt.Add((cpl.CptLt[i + 1]));

                    for (int j = i - 1; j > 0; j--)
                    {
                        if (CGeometricMethods.CalAngle(cpl.CptLt[i - 1], cpl.CptLt[i], cpl.CptLt[j - 1], cpl.CptLt[j]) < dblAngelThreshold * Math.PI / 180)
                        {
                            cptlt.Remove(cpl.CptLt[j]);
                            cptlt.Add(cpl.CptLt[j - 1]);

                        }
                        else
                        {
                            break;
                        }
                    }
                    for (int j = i + 1; j < cpl.CptLt.Count - 1; j++)
                    {
                        if (CGeometricMethods.CalAngle(cpl.CptLt[i], cpl.CptLt[i + 1], cpl.CptLt[j], cpl.CptLt[j + 1]) < dblAngelThreshold * Math.PI / 180)
                        {
                            cptlt.Remove(cpl.CptLt[j]);
                            cptlt.Add(cpl.CptLt[j + 1]);

                        }
                        else
                        {
                            break;
                        }
                    }
                }
                //Step3 找出各个满足条件（两条邻边足够长）的特殊顶点，然后将它们视为断点进行分割

                if (cptlt.Count == 2)
                {
                    if (CGeometricMethods.CalDis(cpl.CptLt[i], cptlt[0]) >= dblThreshold && CGeometricMethods.CalDis(cpl.CptLt[i], cptlt[1]) >= dblThreshold)
                    {
                        CptLt.AddRange(cptlt);
                        CptLt.Add(cpl.CptLt[i]);
                    }
                }
            }
            //删除CptLt中的重复点
            for (int i = 0; i < CptLt.Count; i++)
            {
                for (int j = CptLt.Count - 1; j >= i + 1; j--)
                {
                    if (CptLt[i].Equals2D(CptLt[j], verysmall))
                    {
                        CptLt.RemoveAt(j);
                    }
                }
            }
            //分割断点
            for (int i = CptLt.Count - 1; i >= 0; i--)
            {
                if (CptLt[i].Equals2D(cpl.CptLt[0], verysmall) || CptLt[i].Equals2D(cpl.CptLt[cpl.CptLt.Count - 1], verysmall))
                {
                    CptLt.RemoveAt(i);
                }
            }
            List<CPolyline> Cpllt1 = new List<CPolyline>();
            Cpllt1.Add(cpl);
            for (int i = 0; i < CptLt.Count; i++)
            {
                int intnum = 0;
                for (int j = 0; j < Cpllt1.Count; j++)
                {
                    for (int k = 1; k < Cpllt1[j].CptLt.Count - 1; k++)
                    {
                        if (CptLt[i].Equals2D(Cpllt1[j].CptLt[k], verysmall))
                        {
                            List<CPoint> cptlt1 = new List<CPoint>();
                            List<CPoint> cptlt2 = new List<CPoint>();
                            for (int n = 0; n <= k; n++)
                            {
                                cptlt1.Add(Cpllt1[j].CptLt[n]);
                            }
                            for (int n = k; n < Cpllt1[j].CptLt.Count; n++)
                            {
                                cptlt2.Add(Cpllt1[j].CptLt[n]);
                            }
                            Cpllt1.RemoveAt(j);
                            CPolyline cpl1 = new CPolyline(Cpllt1.Count + 1, cptlt1);
                            Cpllt1.Add(cpl1);
                            CPolyline cpl2 = new CPolyline(Cpllt1.Count + 2, cptlt2);
                            Cpllt1.Add(cpl2);
                            intnum += 1;
                            break;
                        }
                    }

                    if (intnum == 1)
                    {
                        break;
                    }
                }
            }
            for (int i = 0; i < Cpllt1.Count; i++)
            {
                List<CPoint> cptlt0 = new List<CPoint>();
                for (int j = 0; j < Cpllt1[i].CptLt.Count; j++)
                {
                    CPoint cpt = new CPoint(j, (IPoint)Cpllt1[i].CptLt[j]);
                    cptlt0.Add(cpt);
                }
                CPolyline cpl1 = new CPolyline(i, cptlt0);
                Cpllt.Add(cpl1);
            }

        }

        public void RecursivelyMovePt(CPolyline cpl, CVirtualPolyline pVtPl, ref CEdge newBaseLine, ref List<CPoint> newcptlt, double dblPropotion)
        {
            if (pVtPl.CLeftPolyline != null)
            {
                CPoint newcpt = new CPoint();
                Calcpt(ref newcpt, ref newBaseLine, pVtPl, dblPropotion, cpl);

               var newLeftBaseLine = new CEdge(newBaseLine.FrCpt, newcpt);
               var newRightBaseLine = new CEdge(newcpt, newBaseLine.ToCpt);

                RecursivelyMovePt(cpl, pVtPl.CLeftPolyline, ref newLeftBaseLine, ref newcptlt, dblPropotion);
                newcptlt.Add(newcpt);
                RecursivelyMovePt(cpl, pVtPl.CRightPolyline, ref newRightBaseLine, ref newcptlt, dblPropotion);
            }
        }

        public void Calcpt(ref CPoint newcpt, ref CEdge newBaseLine, CVirtualPolyline pVtPl, double dblPropotion, CPolyline cpl)
        {
            newcpt.X = newBaseLine.FrCpt.X + (newBaseLine.ToCpt.X - newBaseLine.FrCpt.X) * pVtPl.Ratio;
            newcpt.Y = newBaseLine.FrCpt.Y + (newBaseLine.ToCpt.Y - newBaseLine.FrCpt.Y) * pVtPl.Ratio;
            double dblStartAngel = CGeometricMethods.CalAxisAngle(newcpt, newBaseLine.ToCpt);
            double dblEndAngel = dblStartAngel + pVtPl.Angel;
            if (dblEndAngel < 0)
            {
                dblEndAngel = dblEndAngel + 2 * Math.PI;
            }
            if (dblEndAngel > 2 * Math.PI)
            {
                dblEndAngel = dblEndAngel - 2 * Math.PI;
            }
            double dblTan = Math.Abs(Math.Tan(dblEndAngel));
            if (dblEndAngel > 0 && dblEndAngel < Math.PI / 2)
            {
                newcpt.X = newcpt.X + dblPropotion * pVtPl.dblMaxDis1 / Math.Pow(1 + dblTan * dblTan, 0.5);
                newcpt.Y = newcpt.Y + dblPropotion * pVtPl.dblMaxDis1 * dblTan / Math.Pow(1 + dblTan * dblTan, 0.5);
            }
            if (dblEndAngel > Math.PI / 2 && dblEndAngel < Math.PI)
            {
                newcpt.X = newcpt.X - dblPropotion * pVtPl.dblMaxDis1 / Math.Pow(1 + dblTan * dblTan, 0.5);
                newcpt.Y = newcpt.Y + dblPropotion * pVtPl.dblMaxDis1 * dblTan / Math.Pow(1 + dblTan * dblTan, 0.5);
            }
            if (dblEndAngel > Math.PI && dblEndAngel < 3 * Math.PI / 2)
            {
                newcpt.X = newcpt.X - dblPropotion * pVtPl.dblMaxDis1 / Math.Pow(1 + dblTan * dblTan, 0.5);
                newcpt.Y = newcpt.Y - dblPropotion * pVtPl.dblMaxDis1 * dblTan / Math.Pow(1 + dblTan * dblTan, 0.5);
            }
            if (dblEndAngel > 3 * Math.PI / 2 && dblEndAngel < 2 * Math.PI)
            {
                newcpt.X = newcpt.X + dblPropotion * pVtPl.dblMaxDis1 / Math.Pow(1 + dblTan * dblTan, 0.5);
                newcpt.Y = newcpt.Y - dblPropotion * pVtPl.dblMaxDis1 * dblTan / Math.Pow(1 + dblTan * dblTan, 0.5);
            }

        }
        public void Rebuild(ref List<CPolyline> displaycpllt, double verysmall)
        {
            List<CPoint> CExteriorPLt = new List<CPoint>();
            for (int i = 0; i < displaycpllt.Count; i++)
            {
                CExteriorPLt.Add(displaycpllt[i].CptLt[0]);
                CExteriorPLt.Add(displaycpllt[i].CptLt[displaycpllt[i].CptLt.Count - 1]);
            }
            List<CPoint> CExteriorPLt2 = new List<CPoint>();
            for (int i = 0; i < CExteriorPLt.Count; i++)
            {
                int intcount = 0;
                for (int j = 0; j < CExteriorPLt.Count; j++)
                {
                    if (CExteriorPLt[i].Equals2D(CExteriorPLt[j], verysmall) == true)
                    {
                        intcount += 1;
                    }
                }
                if (intcount == 2)
                {
                    CExteriorPLt2.Add(CExteriorPLt[i]);
                }

            }
            for (int i = 0; i < CExteriorPLt2.Count; i++)
            {
                for (int j = CExteriorPLt2.Count - 1; j >= i + 1; j--)
                {
                    if (CExteriorPLt2[i].Equals2D(CExteriorPLt2[j], verysmall))
                    {
                        CExteriorPLt2.RemoveAt(j);
                        break;
                    }
                }
            }
            for (int i = 0; i < CExteriorPLt2.Count; i++)
            {
                int intnum1 = 0;
                int intnum2 = 0;
                int intnum3 = displaycpllt.Count;
                for (int j = 0; j < intnum3; j++)
                {
                    if (CExteriorPLt2[i].Equals2D(displaycpllt[j].CptLt[0], verysmall) == true || CExteriorPLt2[i].Equals2D(displaycpllt[j].CptLt[displaycpllt[j].CptLt.Count - 1], verysmall) == true)
                    {
                        intnum1 = j;
                        break;
                    }

                }
                for (int k = intnum1 + 1; k < intnum3; k++)
                {
                    if (CExteriorPLt2[i].Equals2D(displaycpllt[k].CptLt[0], verysmall) == true || CExteriorPLt2[i].Equals2D(displaycpllt[k].CptLt[displaycpllt[k].CptLt.Count - 1], verysmall) == true)
                    {
                        intnum2 = k;
                    }

                }

                if (intnum2 != 0)
                {
                    List<CPoint> cptlt = new List<CPoint>();
                    if (CExteriorPLt2[i].Equals2D(displaycpllt[intnum1].CptLt[0], 0.0001) == true && CExteriorPLt2[i].Equals2D(displaycpllt[intnum2].CptLt[displaycpllt[intnum2].CptLt.Count - 1], verysmall) == true)
                    {
                        for (int m = 0; m < displaycpllt[intnum2].CptLt.Count; m++)
                        {
                            cptlt.Add(displaycpllt[intnum2].CptLt[m]);
                        }
                        for (int n = 1; n < displaycpllt[intnum1].CptLt.Count; n++)
                        {
                            cptlt.Add(displaycpllt[intnum1].CptLt[n]);
                        }
                    }

                    else if (CExteriorPLt2[i].Equals2D(displaycpllt[intnum1].CptLt[0], 0.0001) == true && CExteriorPLt2[i].Equals2D(displaycpllt[intnum2].CptLt[0], verysmall) == true)
                    {
                        for (int m = displaycpllt[intnum2].CptLt.Count - 1; m >= 0; m--)
                        {
                            cptlt.Add(displaycpllt[intnum2].CptLt[m]);
                        }
                        for (int n = 1; n < displaycpllt[intnum1].CptLt.Count; n++)
                        {
                            cptlt.Add(displaycpllt[intnum1].CptLt[n]);
                        }
                    }
                    else if (CExteriorPLt2[i].Equals2D(displaycpllt[intnum1].CptLt[displaycpllt[intnum1].CptLt.Count - 1], 0.0001) == true && CExteriorPLt2[i].Equals2D(displaycpllt[intnum2].CptLt[0], verysmall) == true)
                    {
                        for (int m = 0; m < displaycpllt[intnum1].CptLt.Count; m++)
                        {
                            cptlt.Add(displaycpllt[intnum1].CptLt[m]);
                        }
                        for (int n = 1; n < displaycpllt[intnum2].CptLt.Count; n++)
                        {
                            cptlt.Add(displaycpllt[intnum2].CptLt[n]);
                        }
                    }
                    else if (CExteriorPLt2[i].Equals2D(displaycpllt[intnum1].CptLt[displaycpllt[intnum1].CptLt.Count - 1], 0.0001) == true && CExteriorPLt2[i].Equals2D(displaycpllt[intnum2].CptLt[displaycpllt[intnum2].CptLt.Count - 1], verysmall) == true)
                    {

                        for (int n = 0; n < displaycpllt[intnum1].CptLt.Count; n++)
                        {
                            cptlt.Add(displaycpllt[intnum1].CptLt[n]);
                        }
                        for (int m = displaycpllt[intnum2].CptLt.Count - 2; m >= 0; m--)
                        {
                            cptlt.Add(displaycpllt[intnum2].CptLt[m]);
                        }
                    }

                    CPolyline cpl = new CPolyline(displaycpllt.Count + 1, cptlt);
                    displaycpllt.RemoveAt(intnum1);
                    displaycpllt.RemoveAt(intnum2 - 1);
                    displaycpllt.Add(cpl);

                }
            }
        }

        private void Choose_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            //建立文件夹，保存之后生成的要素图层
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            txtPath.Text = SFD.FileName;

        }

        private void Cancel_Click(object sender, EventArgs e)
        {
            //退出窗口
            this.Hide();
        }




    }
}