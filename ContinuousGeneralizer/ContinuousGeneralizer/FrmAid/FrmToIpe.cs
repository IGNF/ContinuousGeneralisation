using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS;
//using ESRI.ArcGIS.ADF;
//using ESRI.ArcGIS.ADF.BaseClasses;
//using ESRI.ArcGIS.ADF.CATIDs;
//using ESRI.ArcGIS.ADF.COMSupport;
//using ESRI.ArcGIS.ADF.Resources;
using ESRI.ArcGIS.Analyst3D;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.DisplayUI;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.GlobeCore;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;


using ContinuousGeneralizer;
using MorphingClass.CUtility;
using MorphingClass.CMorphingMethods;
using MorphingClass.CGeometry;


namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmToIpe : Form
    {
        private CFrmOperation _FrmOperation;

        CDataRecords _DataRecords;


        public FrmToIpe()
        {
            InitializeComponent();
        }

        public FrmToIpe(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmToIpe_Load(object sender, EventArgs e)
        {
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            ParameterInitialize.cboLayerLt = new List<ComboBox>(1);
            ParameterInitialize.cboLayerLt.Add(this.cboLayer);
            this.cboSize.SelectedIndex = 0;

            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //get parameters
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            IFeatureLayer pFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature.get_Layer(ParameterInitialize.cboLayerLt[0].SelectedIndex);
            double dblIpeMinX = Convert.ToDouble(this.txtIpeMinX.Text);
            double dblIpeMaxX = Convert.ToDouble(this.txtIpeMaxX.Text);
            double dblIpeMinY = Convert.ToDouble(this.txtIpeMinY.Text);
            double dblIpeMaxY = Convert.ToDouble(this.txtIpeMaxY.Text);
            double dblIpeHeight = dblIpeMaxY - dblIpeMinY;
            int intRed = Convert.ToInt16(this.txtRed.Text);
            int intGreen = Convert.ToInt16(this.txtGreen.Text);
            int intBlue = Convert.ToInt16(this.txtBlue.Text);
            string strSize = this.cboSize.SelectedItem.ToString();
            bool blnSaveIntoSameFile = this.chkSaveIntoSameFile.Checked;
            bool blnGroup = this.chkGroup.Checked;

            //save path
            CHelperFunction.SetSavePath(ParameterInitialize);

            IMap m_mapFeature = ParameterInitialize.m_mapFeature;
            IEnvelope pEnvelope = pFLayer.AreaOfInterest;
            double dblMinX = pEnvelope.XMin;
            double dblMinY = pEnvelope.YMin;
            double dblMaxX = pEnvelope.XMax;
            double dblMaxY = pEnvelope.YMax;



            //double dblFactor = pEnvelope.Width / (dblIpeMaxX - dblIpeMinX);
            //LinkedList <string > strLkText=new LinkedList<string> ();
            string strFileName = "Ipe";
            int intCount = 0;
            string strHead = "";
            strHead += CIpeDraw.getIpePreamble();
            strHead += CIpeDraw.getIpeConf();

            string strData = "";

            for (int i = 0; i < m_mapFeature.LayerCount; i++)
            {
                IFeatureLayer pFeatureLayer = m_mapFeature.get_Layer(i) as IFeatureLayer;

                if (pFeatureLayer.Visible == false)
                {
                    continue;
                }

                if (blnGroup == true)
                {
                    strData += "<group>\n";
                }

                //if (blnSaveIntoSameFile == false)
                //{
                //    strFileName = "Ipe" + pFeatureLayer.Name + "_" + i;
                //}

                double dblYIncrease = intCount * dblIpeHeight / 2;
                //dblYIncrease = 0;
                CEnvelope pEnvelopeIpe = new CEnvelope(dblIpeMinX, dblIpeMinY + dblYIncrease, dblIpeMaxX, dblIpeMaxY + dblYIncrease);
                if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPoint))
                {
                    //List<CPoint> CptLt = CHelperFunction.GetCPtLtFromPointFeatureLayer(pFeatureLayer);
                    //CHelperFunction.SaveToIpe(CptLt, strFileName, pEnvelope, pEnvelopeIpe, ParameterInitialize, intRed, intGreen, intBlue, strSize, blnGroup);
                }
                else if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolyline))
                {
                    strData += CHelperFunction.SaveToIpeIpl(pFeatureLayer, strFileName, pEnvelope, pEnvelopeIpe, ParameterInitialize);
                }
                else if ((pFeatureLayer.FeatureClass != null) && (pFeatureLayer.FeatureClass.ShapeType == esriGeometryType.esriGeometryPolygon))
                {
                    strData += CHelperFunction.SaveToIpeIpg(pFeatureLayer, strFileName, pEnvelope, pEnvelopeIpe, ParameterInitialize);
                }

                if (blnGroup == true)
                {
                    strData += "</group>\n";
                }


                intCount++;
            }

            string strEnd = CIpeDraw.getIpeEnd();

            using (var writer = new System.IO.StreamWriter(ParameterInitialize.strSavePath + "\\" + strFileName + ".ipe", true))
            {
                writer.Write(strHead);
                writer.Write(strData);
                writer.Write(strEnd);
            }


            string strsavedfilename=ParameterInitialize.strSavePath + "\\" + strFileName + ".ipe";
            System.Diagnostics.Process.Start(@strsavedfilename);
        }
    }
}