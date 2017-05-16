using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS;
using ESRI.ArcGIS.ADF.BaseClasses;
using ESRI.ArcGIS.ADF.CATIDs;
using ESRI.ArcGIS.ADF.COMSupport;
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

using MorphingClass.CUtility;
using MorphingClass.CGeometry;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmCalGeo_Ipe : Form
    {
        public FrmCalGeo_Ipe()
        {
            InitializeComponent();
        }

        private void btnLength_Click(object sender, EventArgs e)
        {
            string strType = "";
            var cptlt = GetCptLt(rtbInput.Text, out strType);
            if (strType == "Polyline")
            {
                CPolyline cpl = new CPolyline(0, cptlt);
                txtLength.Text = cpl.SetLengthSimple().ToString();
            }
            else
            {
                cptlt.Add(cptlt[0]);
                CPolygon cpg = new CPolygon(0, cptlt);
                txtLength.Text = cpg.SetLengthSimple().ToString();
            }            
        }


        private void btnArea_Click(object sender, EventArgs e)
        {
            string strType = "";
            var cptlt = GetCptLt(rtbInput.Text, out strType);
            cptlt.Add(cptlt[0]);
            CPolygon cpg = new CPolygon(0, cptlt);

            cpg.SetAreaSimple();
            txtArea.Text = cpg.dblAreaSimple.ToString();
        }

        private void btnCentroid_Click(object sender, EventArgs e)
        {
            string strType = "";
            var cptlt = GetCptLt(rtbInput.Text, out strType);
            cptlt.Add(cptlt[0]);
            CPolygon cpg = new CPolygon(0, cptlt);

            var ccpt = cpg.SetCentroidCptSimple();            
            txtCentroidX.Text = ccpt.X.ToString();
            txtCentroidY.Text = ccpt.Y.ToString();
            txtCentroidXY.Text = txtCentroidX.Text + " " + txtCentroidY.Text;
        }

        private List<CPoint> GetCptLt(string str, out string strType)
        {
            char[] charSeparators = new char[] { ' ', 'm', '\n', 'l' };
            var result = str.Split(charSeparators, StringSplitOptions.RemoveEmptyEntries);
            int intMod = result.GetLength(0) % 2;
            if (intMod==0)
            {
                strType = "Polyline";
            }
            else
            {
                strType = "Polygon";
            }


            int intCount = (result.GetLength(0) - intMod) / 2;

            var cptlt = new List<CPoint>();
            for (int i = 0; i < intCount; i++)
            {
                cptlt.Add(new CPoint(i, Convert.ToDouble(result[2 * i]), Convert.ToDouble(result[2 * i+1])));            
            }            

            return cptlt;
        }


    }
}
