using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

using ContinuousGeneralizer;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmSimilarity
{
    public partial class FrmSimAngle3 : Form
    {
        private CFrmOperation _FrmOperation;
        private CDataRecords _DataRecords;                    //数据记录
        
        


        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmSimAngle3()
        {
            InitializeComponent();
        }

        public FrmSimAngle3(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmSimAngle3_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;
            ParameterInitialize.cboLayer = this.cboLayer;
            this.cboRotate.Text = "No";
            this.cboMirrorimage.Text = "No";
            this.cboRoMiRo.Text = "No";

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoadMulticbo();
            _FrmOperation.FrmLoad();
        }



        private void btnRunAll_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //ParameterInitialize.strSavePath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(ParameterInitialize.strSavePath);


            //获取当前选择的点要素图层
            //大比例尺要素图层
            IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLargerScaleLayer.SelectedIndex);
                                                                       
            //小比例尺要素图层
            IFeatureLayer pSSFLayer =(IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboSmallerScaleLayer.SelectedIndex);
                                                           

            ParameterInitialize.pBSFLayer = pBSFLayer;
            ParameterInitialize.pSSFLayer = pSSFLayer;


            //获取线数组
            List<CPolyline> _LSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pBSFLayer);
            List<CPolyline> _SSCPlLt = CHelpFunc.GetCPlLtByFeatureLayer(pSSFLayer);
            CPolyline frcpl = _LSCPlLt[0];
            CPolyline tocpl = _SSCPlLt[0];
            //tocpl.
            //找到旋转中心点（注意：仅旋转tocpl）
            double dblCentralX = (tocpl.pPolyline.Envelope.XMin + tocpl.pPolyline.Envelope.XMax) / 2;  //该值同时又是镜像中心轴
            double dblCentralY = (tocpl.pPolyline.Envelope.YMin + tocpl.pPolyline.Envelope.YMax) / 2;
            IPoint icentralpt = new PointClass();
            icentralpt.PutCoords(dblCentralX, dblCentralY);



            if (this.cboRotate.Text == "No" && this.cboMirrorimage.Text == "No")  //既不旋转，也不镜像
            {
                List<CCorrCpts> pCorrCptsLt = BuildCorrespondences(frcpl, tocpl);
                double dblSimilarity = CalSimilarity(pCorrCptsLt);
                MessageBox.Show(dblSimilarity.ToString());
            }
            else if (this.cboRotate.Text == "No" && this.cboMirrorimage.Text == "Yes")  //不旋转，只镜像（左右镜像）
            {
                //生成镜像线状要素
                CPolyline tocpl2 = Mirror(tocpl, icentralpt);

                ////保存线
                //List <CPolyline > cpllt=new List<CPolyline> ();
                //cpllt.Add (tocpl2);
                //CHelpFunc.SaveCPlLt(cpllt, "sss", ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);

                List<CCorrCpts> pCorrCptsLt = BuildCorrespondences(frcpl, tocpl2);
                double dblSimilarity = CalSimilarity(pCorrCptsLt);
                MessageBox.Show(dblSimilarity.ToString());
            }
            else if (this.cboRotate.Text == "Yes" && this.cboMirrorimage.Text == "No")  //只旋转，不镜像
            {
                SortedList<double, int> dblSimilaritySLt = Rotate(frcpl, tocpl, icentralpt);
                MessageBox.Show(dblSimilaritySLt.Keys[dblSimilaritySLt.Count - 1].ToString() + "；       i =" + dblSimilaritySLt.Values[dblSimilaritySLt.Count - 1].ToString());
            }
            else if (this.cboRotate.Text == "Yes" && this.cboMirrorimage.Text == "Yes")  //既旋转，又镜像
            {
                
                //镜像前的旋转
                SortedList<double, int> dblSimilaritySLt1 = Rotate(frcpl, tocpl, icentralpt);
                MessageBox.Show("镜像前：" + dblSimilaritySLt1.Keys[dblSimilaritySLt1.Count - 1].ToString() + "；       i =" + dblSimilaritySLt1.Values[dblSimilaritySLt1.Count - 1].ToString());

                //镜像后的旋转
                CPolyline tocpl2 = Mirror(tocpl, icentralpt);
                SortedList<double, int> dblSimilaritySLt2 = Rotate(frcpl, tocpl2, icentralpt);
                MessageBox.Show("镜像后：" + dblSimilaritySLt2.Keys[dblSimilaritySLt2.Count - 1].ToString() + "；       i =" + dblSimilaritySLt2.Values[dblSimilaritySLt2.Count - 1].ToString());
            }
        }

        private CPolyline Mirror(CPolyline tocpl, IPoint icentralpt)
        {
            //生成镜像线状要素
            List<CPoint> tocptlt = tocpl.CptLt;
            List<CPoint> newcptlt = new List<CPoint>();
            for (int i = tocptlt.Count - 1; i >= 0; i--)
            {
                double dblnewX = 2 * icentralpt.X - tocptlt[i].X;
                double dblnewY =tocptlt[i].Y;
                CPoint cpt = new CPoint(tocptlt.Count - i - 1, dblnewX, dblnewY);
                newcptlt.Add(cpt);
            }
            CPolyline tocpl2 = new CPolyline(0, newcptlt);
            return tocpl2;
        }

        private SortedList<double, int> Rotate(CPolyline frcpl, CPolyline tocpl, IPoint icentralpt)
        {
            IPointCollection4 pCol = new PolylineClass();
            pCol.AddPointCollection(tocpl as IPointCollection4);
            IPolyline5 newpolyline = pCol as IPolyline5;
            ITransform2D pTransform2D = newpolyline as ITransform2D;

            SortedList<double, int> dblSimilaritySLt = new SortedList<double, int>(new CCmpDbl());
            double dblRotateAngle = 2 * Math.PI / 360;
            for (int i = 0; i < 360; i++)
            {
                pTransform2D.Rotate(icentralpt, dblRotateAngle);
                CPolyline tocpl2 = new CPolyline(i, newpolyline);
                List<CCorrCpts> pCorrCptsLt = BuildCorrespondences(frcpl, tocpl2);
                double dblSimilarity = CalSimilarity(pCorrCptsLt);
                dblSimilaritySLt.Add(dblSimilarity, i);
            }

            return dblSimilaritySLt;
        }


        private List<CCorrCpts>  BuildCorrespondences(CPolyline frcpl, CPolyline tocpl2)
        {
            ////由基于基线长度的DP算法建立对应点关系
            //CMPBDPBL pMPBDPBL = new CMPBDPBL();
            ////计算阈值参数
            //double dblBound = 0.98;
            //CParameterThreshold ParameterThreshold = new CParameterThreshold();
            //ParameterThreshold.dblDLengthBound = dblBound;
            //ParameterThreshold.dblULengthBound = 1 / dblBound;
            ////按指定方式对对应线段进行点匹配，提取对应点
            //C5.LinkedList<CCorrespondSegment> pBLGCorrespondSegmentLk = pMPBDPBL.DWByDPLDefine(frcpl, tocpl2, ParameterThreshold);
            //CAlgorithmsHelper pAlgorithmsHelper = new CAlgorithmsHelper();//作用调用对象的函数
            //List<CPoint> pResultPtLt = pAlgorithmsHelper.BuildPointCorrespondence(pBLGCorrespondSegmentLk, "Linear");
            //List<CCorrCpts> pCorrCptsLt = CHelpFunc.TransferResultptltToCorrCptsLt(pResultPtLt);

            //return pCorrCptsLt;
            return null;
        }




        private void btnReadData_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFG = new OpenFileDialog();
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            if (OFG.FileName == null || OFG.FileName == "") return;
            _DataRecords.ParameterResult = CHelpFuncExcel.InputDataResultPtLt(OFG.FileName);
        }

        private void btnRunData_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;
            double dblSimilarity = CalSimilarity(pCorrCptsLt);
            MessageBox.Show(dblSimilarity.ToString());
        }

        private double CalSimilarity(List<CCorrCpts> pCorrCptsLt)
        {
            //数据准备
            //X、Y的差值
            double[] dblFrDiffX = new double[pCorrCptsLt.Count - 1];
            double[] dblFrDiffY = new double[pCorrCptsLt.Count - 1];
            double[] dblToDiffX = new double[pCorrCptsLt.Count - 1];
            double[] dblToDiffY = new double[pCorrCptsLt.Count - 1];
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                dblFrDiffX[i] = pCorrCptsLt[i + 1].FrCpt.X - pCorrCptsLt[i].FrCpt.X;
                dblFrDiffY[i] = pCorrCptsLt[i + 1].FrCpt.Y - pCorrCptsLt[i].FrCpt.Y;
                dblToDiffX[i] = pCorrCptsLt[i + 1].ToCpt.X - pCorrCptsLt[i].ToCpt.X;
                dblToDiffY[i] = pCorrCptsLt[i + 1].ToCpt.Y - pCorrCptsLt[i].ToCpt.Y;
            }
            //各线段长度
            double[] dblFrDis = new double[pCorrCptsLt.Count - 1];
            double[] dblToDis = new double[pCorrCptsLt.Count - 1];
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                dblFrDis[i] = Math.Sqrt(dblFrDiffX[i] * dblFrDiffX[i] + dblFrDiffY[i] * dblFrDiffY[i]);
                dblToDis[i] = Math.Sqrt(dblToDiffX[i] * dblToDiffX[i] + dblToDiffY[i] * dblToDiffY[i]);
            }
            //Fr线段和To线段之间的角度差（设定为必然小于180度，用余弦求）
            double[] dblDiffAngle = new double[pCorrCptsLt.Count - 1];
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                dblDiffAngle[i] = CGeoFunc.CalAngle(dblFrDiffX[i], dblFrDiffY[i], dblToDiffX[i], dblToDiffY[i]);
            }

            //计算最终的值
            double dblSumNumerator = 0;
            double dblSumDenominator = 0;
            //double dblSumDenominator = dblFrAngle[0] * (dblFrDis[pCorrCptsLt.Count - 2] + dblFrDis[0]) + dblToAngle[0] * (dblToDis[pCorrCptsLt.Count - 2] + dblToDis[0]);
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                dblSumNumerator += dblDiffAngle[i] * (dblFrDis[i] + dblToDis[i]);
                //dblSumDenominator += dblFrAngle[i] * (dblFrDis[i-1] + dblFrDis[i]) + dblToAngle[i] * (dblToDis[i-1] + dblToDis[i]);
                dblSumDenominator += dblFrDis[i] + dblToDis[i];
            }

            double dblSimilarity = 1 - dblSumNumerator / (Math.PI * dblSumDenominator);
            return dblSimilarity;
        }



        private void btnRunCircle_Click(object sender, EventArgs e)
        {
            double dblRadius = Convert.ToDouble(this.txtRadius.Text);
            string strSelectedLayer = this.cboLayer.Text;
            IFeatureLayer pFeatureLayer = null;

            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            //获取线状要素
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
            //读取线数据
            List<CPolyline> CPolylineLt = CHelpFunc.GetCPlLtByFeatureLayer(pFeatureLayer);
            CPolyline cpl = CPolylineLt[0];

            //将线状要素分成3600份（得到3601个坐标点）
            List<CPoint> cptlt = new List<CPoint>();
            for (int i = 0; i <= 3600; i++)
            {
                IPoint outPoint=new PointClass ();
                double dblRatio = Convert .ToDouble (i) / 3600;
                cpl.pPolyline.QueryPoint(esriSegmentExtension.esriNoExtension, dblRatio, true, outPoint);
                CPoint cpt = new CPoint(i, outPoint);
                cptlt.Add(cpt);
            }

            //将圆分成3600份（得到3601个坐标点）         
            List<CPoint> CircleCptLt = new List<CPoint>();
            double dblAdd = Math.PI / 1800;
            double dblHalfPI = Math.PI / 2;
            for (int i = 0; i <= 3600; i++)
            {
                double dblAngle = i * dblAdd;
                double dblCricleX = dblRadius * Math.Cos(dblAngle + dblHalfPI);  //由于本方法是从“最顶上的点”开始的，因此需加“PI / 2”
                double dblCricleY = dblRadius * Math.Sin(dblAngle + dblHalfPI);  //由于本方法是从“最顶上的点”开始的，因此需加“PI / 2”
                CPoint cpt = new CPoint(i, dblCricleX, dblCricleY);
                CircleCptLt.Add(cpt);
            }
            
            //由于物体是均匀的，各长度一致，因此不需定义数组
            double dblFrDis = CGeoFunc.CalDis(CircleCptLt[0], CircleCptLt[1]);
            double dblToDis = CGeoFunc.CalDis(cptlt[0], cptlt[1]);

            //Fr线段和To线段之间的角度差（设定为必然小于180度，用余弦求）
            double[] dblDiffAngle = new double[3600];
            for (int i = 0; i <3600; i++)
            {
                dblDiffAngle[i] = CGeoFunc.CalAngle(CircleCptLt[i], CircleCptLt[i+1], cptlt[i], cptlt[i+1]);
            }

            //计算最终的值
            double dblSumNumerator = 0;
            double dblSumDenominator = 0;
            for (int i = 0; i < 3600; i++)
            {
                dblSumNumerator += dblDiffAngle[i] * (dblFrDis + dblToDis);
                dblSumDenominator += dblFrDis + dblToDis;
            }

            double dblSimilarity = 1 - dblSumNumerator / (Math.PI * dblSumDenominator);
            MessageBox.Show(dblSimilarity.ToString());
            
        
        }


    }
}
