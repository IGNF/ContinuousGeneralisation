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

            tltCboSize.SetToolTip(this.cboSize, 
                "0.05 Thin; 0.5 Normal; 0.8 Heavier; 1.2 Fat; 2 Ultrafat. 0.05 is too small for points!");


            //进行Load操作，初始化变量
            _FrmOperation = new CFrmOperation(ref ParameterInitialize);
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //get parameters
            CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            IFeatureLayer pFLayer = (IFeatureLayer)ParameterInitialize.m_mapFeature
                .get_Layer(ParameterInitialize.cboLayerLt[0].SelectedIndex);
            CEnvelope pEnvelopeIpe = new CEnvelope(
                Convert.ToDouble(this.txtIpeMinX.Text), Convert.ToDouble(this.txtIpeMinY.Text),
                Convert.ToDouble(this.txtIpeMaxX.Text), Convert.ToDouble(this.txtIpeMaxY.Text));

            string strBoundWidth = this.cboSize.SelectedItem.ToString();
            if (chkOverrideWidth.Checked == false)
            {
                strBoundWidth = "";
            }


            bool blnSaveIntoSameFile = this.chkSaveIntoSameFile.Checked;
            bool blnGroup = this.chkGroup.Checked;

            //save path
            CHelpFunc.SetSavePath(ParameterInitialize);

            IMap m_mapFeature = ParameterInitialize.m_mapFeature;
            IEnvelope pEnvelopeLayer = pFLayer.AreaOfInterest;

            double dblFactorIpeToLayer = pEnvelopeIpe.Height / pEnvelopeLayer.Height;
            int intCount = 0;
            string strHead = "";
            strHead += CIpeDraw.getIpePreamble();
            strHead += CIpeDraw.getIpeConf();

            //add legend (unit and a sample line)
            string strData = CIpeDraw.writeIpeText(32 / dblFactorIpeToLayer + " " + m_mapFeature.MapUnits.ToString(), 320, 80) +
                CIpeDraw.drawIpeEdge(320, 64, 352, 64);

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

                strData += SaveToIpe(pFeatureLayer, pEnvelopeLayer, pEnvelopeIpe, strBoundWidth);

                if (blnGroup == true)
                {
                    strData += "</group>\n";
                }

                pEnvelopeIpe.YMin += pEnvelopeIpe.Height;
                pEnvelopeIpe.YMax += pEnvelopeIpe.Height;
                intCount++;
            }

            string strEnd = CIpeDraw.getIpeEnd();
            string strFullName = ParameterInitialize.strSavePath + "\\" + CHelpFunc.GetTimeStamp() + ".ipe";
            using (var writer = new System.IO.StreamWriter(strFullName, true))
            {
                writer.Write(strHead);
                writer.Write(strData);
                writer.Write(strEnd);
            }


            //string strsavedfilename = ParameterInitialize.strSavePath + "\\" + strFileName + ".ipe";
            System.Diagnostics.Process.Start(@strFullName);
        }


        public static string SaveToIpe(IFeatureLayer pFeatureLayer, IEnvelope pEnvelopeLayer, CEnvelope pEnvelopeIpe, 
            string strBoundWidth)
        {
            string str = "";
            IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
            int intFeatureCount = pFeatureClass.FeatureCount(null);
            IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //注意此处的参数(****,false)！！！            
            var pRenderer = (pFeatureLayer as IGeoFeatureLayer).Renderer;
            for (int i = 0; i < intFeatureCount; i++)
            {
                //at the last round of this loop, pFeatureCursor.NextFeature() will return null
                IFeature pFeature = pFeatureCursor.NextFeature();

                switch (pFeatureClass.ShapeType)
                {
                    case esriGeometryType.esriGeometryPoint:
                        str += TranIptToIpe(pFeature, pRenderer, pEnvelopeLayer, pEnvelopeIpe, strBoundWidth);
                        break;
                    case esriGeometryType.esriGeometryPolyline:
                        str += TranIplToIpe(pFeature, pRenderer, pEnvelopeLayer, pEnvelopeIpe, strBoundWidth);
                        break;
                    case esriGeometryType.esriGeometryPolygon:
                        str += TranIpgToIpe(pFeature, pRenderer, pEnvelopeLayer, pEnvelopeIpe, strBoundWidth);
                        break;
                    default:
                        break;
                }
            }

            return str;
        }

        /// <summary>
        /// Save a feature layer of IPolyline to Ipe
        /// </summary>
        public static string TranIptToIpe(IFeature pFeature, IFeatureRenderer pRenderer, IEnvelope pEnvelopeLayer,
            CEnvelope pEnvelopeIpe, string strBoundWidth)
        {
            var pMarkerSymbol = pRenderer.SymbolByFeature[pFeature] as IMarkerSymbol;
            var pMarkerSymbolRgbColor = pMarkerSymbol.Color as IRgbColor;

            if (strBoundWidth == "")
            {
                strBoundWidth = pMarkerSymbol.Size.ToString();
            }

            var ipt = pFeature.Shape as IPoint;
            return CIpeDraw.DrawIpt(ipt, pEnvelopeLayer, pEnvelopeIpe, "disk", new CColor(pMarkerSymbolRgbColor), strBoundWidth);
        }



        /// <summary>
        /// Save a feature layer of IPolyline to Ipe
        /// </summary>
        public static string TranIplToIpe(IFeature pFeature, IFeatureRenderer pRenderer, IEnvelope pEnvelopeLayer,
            CEnvelope pEnvelopeIpe, string strBoundWidth)
        {
            var pLineSymbol = pRenderer.SymbolByFeature[pFeature] as ILineSymbol;
            var pLineSymbolRgbColor = pLineSymbol.Color as IRgbColor;
            if (strBoundWidth == "")
            {
                strBoundWidth = pLineSymbol.Width.ToString();
            }

            //get the feature
            CPolyline cpl = new CPolyline(0, pFeature.Shape as IPolyline5);

            //append the string
            return CIpeDraw.DrawCpl(cpl, pEnvelopeLayer, pEnvelopeIpe,
                new CColor(pLineSymbolRgbColor), strBoundWidth);
        }

        /// <summary>
        /// Save a feature layer of IPolygon to Ipe
        /// </summary>
        public static string TranIpgToIpe(IFeature pFeature, IFeatureRenderer pRenderer, IEnvelope pEnvelopeLayer,
    CEnvelope pEnvelopeIpe, string strBoundWidth)
        {
            var pFillSymbol = pRenderer.SymbolByFeature[pFeature] as IFillSymbol;

            //get the color of the filled part
            //we are not allowed to directly use "var pFillRgbColor = pFillSymbol.Color as IRgbColor;"
            //Nor can we use "var pFillRgbColor = pFillSymbol.Color.RGB as IRgbColor;"
            //pFillSymbol.Color.RGB has type 'int'
            IColor pFillSymbolColor = new RgbColorClass();
            pFillSymbolColor.RGB = pFillSymbol.Color.RGB;
            var pFillSymbolRgbColor = pFillSymbolColor as IRgbColor;

            //get the color of the out line                
            var pOutlineRgbColor = pFillSymbol.Outline.Color as IRgbColor;
            if (strBoundWidth == "")
            {
                strBoundWidth = pFillSymbol.Outline.Width.ToString();
            }

            //get the feature
            CPolygon cpg = new CPolygon(0, pFeature.Shape as IPolygon4);

            //append the string
            return CIpeDraw.DrawCpg(cpg, pEnvelopeLayer, pEnvelopeIpe, new CColor(pOutlineRgbColor),
new CColor(pFillSymbolRgbColor), strBoundWidth);
        }



        #region Obsolete: A case using IClassBreaksRenderer and ILookupSymbol
        ///// <summary>
        ///// SaveToIpe
        ///// </summary>
        ///// <param name="CPlLt"></param>
        ///// <param name="strWidth">normal, heavier, fat, ultrafat</param>
        //public static string SaveToIpeIpg(IFeatureLayer pFeatureLayer, string strFileName, IEnvelope pEnvelopeLayer, CEnvelope pEnvelopeIpe, CParameterInitialize pParameterInitialize)
        //{
        //    string str = "";
        //    IFeatureClass pFeatureClass = pFeatureLayer.FeatureClass;
        //    int intFeatureCount = pFeatureClass.FeatureCount(null);
        //    IFeatureCursor pFeatureCursor = pFeatureClass.Search(null, false);    //注意此处的参数(****,false)！！！            

        //    IGeoFeatureLayer pGeoFeaturelayer = pFeatureLayer as IGeoFeatureLayer;
        //    IClassBreaksRenderer pClassBreaksRenderer = pGeoFeaturelayer.Renderer as IClassBreaksRenderer;

        //    IFeature pFeature = pFeatureCursor.NextFeature();
        //    ILookupSymbol pLookupSymbol = pClassBreaksRenderer as ILookupSymbol;
        //    pLookupSymbol.LookupSymbol(true, pFeature);

        //    for (int i = 0; i < intFeatureCount; i++)
        //    {
        //        //get the colors
        //        ISimpleFillSymbol pSimpleFillSymbol = pLookupSymbol.LookupSymbol(true, pFeature) as ISimpleFillSymbol;
        //        IRgbColor pOutlineRgbColor = pSimpleFillSymbol.Outline.Color as IRgbColor;
        //        IColor pFillSymbolColor = new RgbColorClass();
        //        pFillSymbolColor.RGB = pSimpleFillSymbol.Color.RGB;
        //        IRgbColor pFillSymbolRgbColor = pFillSymbolColor as IRgbColor;

        //        //get the feature
        //        IPolygon4 ipg = pFeature.Shape as IPolygon4;
        //        //SetZCoordinates(ipg as IPointCollection4);  //set the z coordinates, it may be used in Constructing TIN
        //        CPolygon cpg = new CPolygon(i, ipg as IPolygon4);

        //        //append the string
        //        str += CIpeDraw.DrawCpg(cpg, pEnvelopeLayer, pEnvelopeIpe, new CColor(pOutlineRgbColor), new CColor(pFillSymbolRgbColor), pSimpleFillSymbol.Outline.Width.ToString());

        //        pFeature = pFeatureCursor.NextFeature();  //at the last round of this loop, pFeatureCursor.NextFeature() will return null
        //    }

        //    return str;
        //}
        #endregion



    }
}