using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using C5;
using SCG = System.Collections.Generic;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Display;

using ContinuousGeneralizer;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmCreateRandomPointLayer : Form
    {


        CDataRecords _DataRecords;

        public FrmCreateRandomPointLayer()
        {
            InitializeComponent();
        }




        public FrmCreateRandomPointLayer(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmCreateRandomPointLayer_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            
            
            
            //ParameterInitialize.cboLayer = this.cboLayer;

            //进行Load操作，初始化变量
            //_FrmOperation = new CFrmOperation(ref ParameterInitialize);
            //throw new ArgumentException("improve loading layesr!");
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //string strSelectedLayer = this.cboLayer.Text;
            int intPointNum = Convert.ToInt32(this.txtPointNum.Text);
            double dblMinX = Convert.ToDouble(this.txtMinX.Text);
            double dblMinY = Convert.ToDouble(this.txtMinY.Text);
            double dblMaxX = Convert.ToDouble(this.txtMaxX.Text);
            double dblMaxY = Convert.ToDouble(this.txtMaxY.Text);
            int intNumberofSets = Convert.ToInt16(this.txtNumberofSets.Text);

            //弹出保存对话框
            SaveFileDialog SFD = new SaveFileDialog();
            SFD.ShowDialog();
            string strPath = SFD.FileName;
            ParameterInitialize.pWorkspace = CHelpFunc.OpenWorkspace(strPath);


            long lngStartTime = System.Environment.TickCount; //记录开始时间

            double dblScaleX = dblMaxX - dblMinX;
            double dblScaleY = dblMaxY - dblMinY;
            Random rand = new Random();

            int intActualNum = intPointNum;
            for (int k = 0; k < intNumberofSets; k++)
            {
                List<CPoint> cptlt = new List<CPoint>(intActualNum);
                for (int i = 0; i < intActualNum; i++)
                {

                    //double dblX = dblMinX + dblScaleX * Convert.ToDouble(Convert.ToInt64(rand.NextDouble() * 10000000000)) / 10000000000;
                    //double dblY = dblMinY + dblScaleY * Convert.ToDouble(Convert.ToInt64(rand.NextDouble() * 10000000000)) / 10000000000;
                    double dblX = dblMinX + rand.NextDouble();
                    double dblY = dblMinY + rand.NextDouble();
                    CPoint cpt = new CPoint(i, dblX, dblY);
                    cptlt.Add(cpt);
                }


                //确定保存信息
                //CSaveFeature.SaveCGeoEb(cptlt, esriGeometryType.esriGeometryPoint, "Random Point_" + intActualNum, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl);
                //CSaveFeature pSaveFeature = new CSaveFeature("Random Point_" + intActualNum, ParameterInitialize.pWorkspace, ParameterInitialize.m_mapControl, esriGeometryType.esriGeometryPoint);
                //List<List<object>> objectvalueltlt = new List<List<object>>(cptlt.Count);
                //List<object> objshapelt = new List<object>(cptlt.Count);

                //for (int i = 0; i < cptlt.Count; i++)
                //{
                //    objshapelt.Add(cptlt[i]);

                //    List<object> objvaluelt = new List<object>(1);
                //    objvaluelt.Add(i);
                //    objectvalueltlt.Add(objvaluelt);
                //}


                //List<esriFieldType> esriFieldTypeLt = new List<esriFieldType>();
                //esriFieldTypeLt.Add(esriFieldType.esriFieldTypeInteger);
                //List<string> strFieldNameLt = new List<string>();
                //strFieldNameLt.Add("Id");

                //pSaveFeature.esriFieldTypeLt = esriFieldTypeLt;
                //pSaveFeature.objectShapeLt = objshapelt;
                //pSaveFeature.objectValueLtLt = objectvalueltlt;
                //pSaveFeature.strFieldNameLt = strFieldNameLt;

                ////保存点数据为要素图层
                //pSaveFeature.SaveFeaturesToLayer();
                //intActualNum = intActualNum + intPointNum;
            }



            long lngEndTime = System.Environment.TickCount;//记录结束时间
            _DataRecords.ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时



        }


    }
}
