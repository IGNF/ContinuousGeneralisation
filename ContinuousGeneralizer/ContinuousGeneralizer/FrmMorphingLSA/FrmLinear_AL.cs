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
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethodsLSA;
using MorphingClass.CGeometry;
using MorphingClass.CCorrepondObjects;

namespace ContinuousGeneralizer.FrmMorphingLSA
{
    /// <summary>
    /// 顾及面状要素面积和周长的变形方法
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    /// <remarks>LSA:Least Squares Adjustment;AL:Area and Length</remarks>
    public partial class FrmLinear_AL : Form
    {
        private CDataRecords _DataRecords;                    //数据记录
        
        
        private double[] _adblFrLength = new double[0];
        private double[] _adblFrAngle = new double[0];
        private double[] _adblToLength = new double[0];
        private double[] _adblToAngle = new double[0];
        //private CLinear_AL _pLinear_AL;


        private CPolyline _RelativeInterpolationCpl;
        private double _dblProportion = 0.5;

        /// <summary>属性：数据记录</summary>
        public CDataRecords DataRecords
        {
            get { return _DataRecords; }
            set { _DataRecords = value; }
        }


        public FrmLinear_AL()
        {
            InitializeComponent();
        }

        public FrmLinear_AL(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
        }

        private void FrmLinear_AL_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            CConstants.strMethod = "Linear_AL";
        }

        private void btnReadData_Click(object sender, EventArgs e)
        {
            OpenFileDialog OFG = new OpenFileDialog();
            OFG.Filter = "xlsx files (*.xlsx)|*.xlsx|All files (*.*)|*.*";
            OFG.ShowDialog();
            if (OFG.FileName == null || OFG.FileName == "") return;
            _DataRecords.ParameterResult = CHelperFunctionExcel.InputDataResultPtLt(OFG.FileName);

        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //CPolyline FrCpl =_DataRecords.ParameterResult .FromCpl;
            //CPolyline ToCpl = _DataRecords.ParameterResult.ToCpl ;

            //int intPtNum = FrCpl.CptLt .Count ;

            ////计算长度初始值（全部计算）    
            //_adblFrLength = new double[intPtNum - 2];
            //_adblFrAngle = new double[intPtNum - 2];
            //_adblToLength = new double[intPtNum - 2];
            //_adblToAngle = new double[intPtNum - 2];


            ////计算两原线状要素的长度值和夹角值
            //for (int j = 0; j < intPtNum - 2; j++)
            //{
            //    //由于最后两个点直接由插值得出，故此处未计算最后两个点之间的长度值
            //    _adblFrLength[j] = CGeometricMethods.CalDis(FrCpl.CptLt[j], FrCpl.CptLt[j + 1]);
            //    _adblToLength[j] = CGeometricMethods.CalDis(ToCpl.CptLt[j], ToCpl.CptLt[j + 1]);

            //    _adblFrAngle[j] = CGeometricMethods.CalAngle2(FrCpl.CptLt[j], FrCpl.CptLt[j + 1], FrCpl.CptLt[j + 2]);
            //    _adblToAngle[j] = CGeometricMethods.CalAngle2(ToCpl.CptLt[j], ToCpl.CptLt[j + 1], ToCpl.CptLt[j + 2]);
            //}

            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //SaveFileDialog SFD = new SaveFileDialog();
            //SFD.ShowDialog();
            //if (SFD.FileName == null || SFD.FileName == "") return;
            //ParameterInitialize.strSavePath = SFD.FileName;
            //ParameterInitialize.pWorkspace = CHelperFunction.OpenWorkspace(ParameterInitialize.strSavePath);

        }

        private void btn010_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.1;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn020_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.2;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn030_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.3;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn040_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.4;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn050_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.5;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn060_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.6;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn070_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.7;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn080_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.8;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn090_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.9;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn000_Click(object sender, EventArgs e)
        {
            _dblProportion = 0;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn025_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.25;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn075_Click(object sender, EventArgs e)
        {
            _dblProportion = 0.75;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }
        private void btn100_Click(object sender, EventArgs e)
        {
            _dblProportion = 1;
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }

        private void btnInputedScale_Click(object sender, EventArgs e)
        {
            _dblProportion = Convert.ToDouble(this.txtProportion.Text);
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);

        }

        private void btnReduce_Click(object sender, EventArgs e)
        {
            try
            {
                _dblProportion = _dblProportion - 0.02;
                pbScale.Value = Convert.ToInt16(100 * _dblProportion);
                _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
            }
            catch (Exception)
            {
                MessageBox.Show("不能再减小了！");
            }

        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            _dblProportion = _dblProportion + 0.02;
            pbScale.Value = Convert.ToInt16(100 * _dblProportion);
            _RelativeInterpolationCpl = DisplayInterpolation(_dblProportion, _DataRecords.ParameterInitialize.m_mapControl);
        }

        private void btnSaveInterpolation_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            List<CPolyline> cpllt = new List<CPolyline>();
            cpllt.Add(_RelativeInterpolationCpl);
            string strFileName = _dblProportion.ToString();
            CHelperFunction.SaveCPlLt(cpllt, strFileName, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
        }

        private CPolyline DisplayInterpolation(double dblProportion, IMapControl4 m_mapControl)
        {
            List<CCorrCpts> pCorrCptsLt = _DataRecords.ParameterResult.CCorrCptsLt;
            int intPtNum = pCorrCptsLt.Count;

            List<CPoint> newcptlt = new List<CPoint>();
            //先确定最后两个点
            double dblLastX = (1 - dblProportion) * pCorrCptsLt[intPtNum - 1].FrCpt.X + dblProportion * pCorrCptsLt[intPtNum - 1].ToCpt.X;
            double dblLastY = (1 - dblProportion) * pCorrCptsLt[intPtNum - 1].FrCpt.Y + dblProportion * pCorrCptsLt[intPtNum - 1].ToCpt.Y;
            CPoint newlastcpt = new CPoint(intPtNum - 1, dblLastX, dblLastY);
            double dblLast1X = (1 - dblProportion) * pCorrCptsLt[intPtNum - 2].FrCpt.X + dblProportion * pCorrCptsLt[intPtNum - 2].ToCpt.X;
            double dblLast1Y = (1 - dblProportion) * pCorrCptsLt[intPtNum - 2].FrCpt.Y + dblProportion * pCorrCptsLt[intPtNum - 2].ToCpt.Y;
            CPoint newlast1cpt = new CPoint(intPtNum - 2, dblLast1X, dblLast1Y);
            newcptlt.Insert(0, newlastcpt);
            newcptlt.Insert(0, newlast1cpt);

            double dblPreAxisAngle = CGeometricMethods.CalAxisAngle(newlast1cpt, newlastcpt);   //前一个线段的绝对角
            for (int i = pCorrCptsLt.Count - 3; i >= 0; i--)
            {
                //目标夹角大小
                double dblAngle = (1 - dblProportion) * _adblFrAngle[i] + dblProportion * _adblToAngle[i];
                //绝对角
                double dblCurAxisAngle = dblPreAxisAngle - dblAngle;      //当前线段的绝对角(0到2*PI)
                if (dblCurAxisAngle >= 2 * Math.PI)
                {
                    dblCurAxisAngle = dblCurAxisAngle - 2 * Math.PI;
                }
                else if (dblCurAxisAngle < 0)
                {
                    dblCurAxisAngle = dblCurAxisAngle + 2 * Math.PI;
                }

                //目标长度
                double dblLength = (1 - dblProportion) * _adblFrLength[i] + dblProportion * _adblToLength[i];
                //新坐标
                double dblNewX = newcptlt[0].X + dblLength * Math.Cos(dblCurAxisAngle);
                double dblNewY = newcptlt[0].Y + dblLength * Math.Sin(dblCurAxisAngle);
                CPoint newcpt = new CPoint(i, dblNewX, dblNewY);
                newcptlt.Insert(0, newcpt);
                //更新dblPreAxisAngle(将向量旋转180°)
                if (dblCurAxisAngle >= Math.PI)
                {
                    dblPreAxisAngle = dblCurAxisAngle - Math.PI;
                }
                else
                {
                    dblPreAxisAngle = dblCurAxisAngle + Math.PI;
                }
            }
            CPolyline newcpl = new CPolyline(0, newcptlt);

            IGraphicsContainer pGra = m_mapControl.Map as IGraphicsContainer;
            pGra.DeleteAllElements();
            CHelperFunction.ViewPolyline(m_mapControl, newcpl);
            return newcpl;
        }



    }
}