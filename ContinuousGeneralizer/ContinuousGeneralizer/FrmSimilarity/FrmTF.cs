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
    public partial class FrmTF : Form
    {
        //TF:Turning Function

        private CFrmOperation _FrmOperation;
        private CDataRecords _DataRecords;                    //数据记录
        
        
        CParameterInitialize _ParameterInitialize;


        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmTF()
        {
            InitializeComponent();
        }

        public FrmTF(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmTF_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.pMap = ParameterInitialize.m_mapControl.Map;
            ParameterInitialize.m_mapFeature = new MapClass();
            ParameterInitialize.m_mapAll = new MapClass();
            ParameterInitialize.cboLargerScaleLayer = this.cboLargerScaleLayer;
            ParameterInitialize.cboSmallerScaleLayer = this.cboSmallerScaleLayer;

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
            _FrmOperation.FrmLoadMulticbo();
        }



        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //ParameterInitialize.strSavePath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);


            //获取当前选择的点要素图层
            //大比例尺要素图层
            IFeatureLayer pBSFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLargerScaleLayer.SelectedIndex);
                                                                       
            //小比例尺要素图层
            IFeatureLayer pSSFLayer =(IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboSmallerScaleLayer.SelectedIndex);
                                                           

            ParameterInitialize.pBSFLayer = pBSFLayer;
            ParameterInitialize.pSSFLayer = pSSFLayer;


            //获取线数组
            List<CPolyline> _LSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pBSFLayer);
            List<CPolyline> _SSCPlLt = CHelperFunction.GetCPlLtByFeatureLayer(pSSFLayer);
            CPolyline frcpl = _LSCPlLt[0];
            CPolyline tocpl = _SSCPlLt[0];

            List<CCorrCpts> pCorrCptsLt = BuildCorrespondences(frcpl, tocpl);
            double dblSimilarity = CalSimilarity(pCorrCptsLt, frcpl ,tocpl);
            MessageBox.Show(dblSimilarity.ToString());
        }


        private List<CCorrCpts> BuildCorrespondences(CPolyline frcpl, CPolyline tocpl)
        {
            CLinearInterpolationA pLinearInterpolationA = new CLinearInterpolationA();
            List<CPoint> pResultPtLt = pLinearInterpolationA.CLI(frcpl, tocpl);
            List<CCorrCpts> pCorrCptsLt = CHelperFunction.TransferResultptltToCorrCptsLt(pResultPtLt);

            return pCorrCptsLt;
        }


        private void btnReadData_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFG = new OpenFileDialog();
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            if (OFG.FileName == null || OFG.FileName == "") return;
            _DataRecords.ParameterResult = CHelperFunctionExcel.InputDataResultPtLt(OFG.FileName);
        }

        private void btnRunData_Click(object sender, EventArgs e)
        {
            CParameterResult pParameterResult = _DataRecords.ParameterResult;
            List<CCorrCpts> pCorrCptsLt = pParameterResult.CCorrCptsLt;
            double dblSimilarity = CalSimilarity(pCorrCptsLt, pParameterResult.FromCpl, pParameterResult.ToCpl);
            MessageBox.Show(dblSimilarity.ToString());
        }

        private double CalSimilarity(List<CCorrCpts> pCorrCptsLt, CPolyline frcpl, CPolyline tocpl)
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

            //各线段累积方位角（IAzimuth: Integral Azimuth）
            double[] adblFrIAzimuth = new double[pCorrCptsLt.Count - 1];
            double[] adblToIAzimuth = new double[pCorrCptsLt.Count - 1];
            adblFrIAzimuth[0] = CGeometricMethods.CalAxisAngle(dblFrDiffX[0], dblFrDiffY[0]);
            adblToIAzimuth[0] = CGeometricMethods.CalAxisAngle(dblToDiffX[0], dblToDiffY[0]);
            for (int i = 1; i < pCorrCptsLt.Count - 1; i++)
            {
                adblFrIAzimuth[i] = adblFrIAzimuth[i - 1] + CGeometricMethods.CalAngle2(pCorrCptsLt[i - 1].FrCpt, pCorrCptsLt[i].FrCpt, pCorrCptsLt[i + 1].FrCpt) - Math.PI;
                adblToIAzimuth[i] = adblToIAzimuth[i - 1] + CGeometricMethods.CalAngle2(pCorrCptsLt[i - 1].ToCpt, pCorrCptsLt[i].ToCpt, pCorrCptsLt[i + 1].ToCpt) - Math.PI;
            }
            double dblAreaFr = 0;
            double dblAreaTo = 0;
            double dblAreaDiff = 0;
            for (int i = 0; i < pCorrCptsLt.Count - 1; i++)
            {
                double dblRatioFr = dblFrDis[i] / frcpl.pPolyline.Length;
                double dblRatioTo = dblToDis[i] / tocpl.pPolyline.Length;
                double dblRatioAv = (dblFrDis[i] + dblToDis[i]) / (frcpl.pPolyline.Length + tocpl.pPolyline.Length);

                dblAreaFr += (Math.Abs(adblFrIAzimuth[i]) * dblFrDis[i]);
                dblAreaTo += (Math.Abs(adblToIAzimuth[i]) * dblToDis[i]);
                dblAreaDiff += (Math.Abs((adblFrIAzimuth[i] - adblToIAzimuth[i])) * (dblFrDis[i] + dblToDis[i]));
            }
            dblAreaFr = dblAreaFr / frcpl.pPolyline.Length;
            dblAreaTo = dblAreaTo / tocpl.pPolyline.Length;
            dblAreaDiff = dblAreaDiff / (frcpl.pPolyline.Length + tocpl.pPolyline.Length);

            double dblDis = 0;
            if (dblAreaFr >= dblAreaTo)
            {
                dblDis = dblAreaDiff / dblAreaFr;
            }
            else
            {
                dblDis = dblAreaDiff / dblAreaTo;
            }

            double dblSimilarity = 1 - dblDis;
            return dblSimilarity;


        }


    }


}