using System;
using System.Collections.Generic;
using System.Linq;
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
using MorphingClass.CAid;

namespace ContinuousGeneralizer.FrmAid
{
    public partial class FrmTestDistanceMetric : Form
    {
        private CFrmOperation _FrmOperation;
        private CDataRecords _DataRecords;


        public FrmTestDistanceMetric()
        {
            InitializeComponent();
        }

        public FrmTestDistanceMetric(CDataRecords pDataRecords)
        {
            InitializeComponent();
            _DataRecords = pDataRecords;
            pDataRecords.ParameterInitialize.frmCurrentForm = this;
            //m_mapControl = m_MapControl;
            //_tsslTime = tsslTime;
        }

        private void FrmTestDistanceMetric_Load(object sender, EventArgs e)
        {
            //we don't really need to load, since we don't need layers from ArcGIS


            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;
            //ParameterInitialize.cboLayerLt = new List<ComboBox>(0);

            ////进行Load操作，初始化变量
            //_FrmOperation = new CFrmOperation(ref ParameterInitialize);
        }

        public void btnRun_Click(object sender, EventArgs e)
        {
            //CParameterInitialize ParameterInitialize = _DataRecords.ParameterInitialize;

            ////ReadDataFromExcel
            //var aObj = CHelpFuncExcel.ReadDataFromExcel();
            //if (aObj == null) return;

            ////get distance
            //int intDataRow = aObj.GetUpperBound(0) - 1;  //substract 1 because usullay, there is a title for each row and each colomun
            //int intDataCol = aObj.GetUpperBound(1) - 1;  //note that intDataRow == intDataCol
            //var adblDisMat = new double[intDataRow, intDataCol];
            //for (int i = 0; i < intDataRow; i++)
            //{
            //    for (int j = 0; j < intDataCol; j++)
            //    {
            //        adblDisMat[i, j] = Convert.ToDouble(aObj[i + 1, j + 1]);
            //    }
            //}


            ////whether larger or equal to 0
            //for (int i = 0; i < intDataRow; i++)
            //{
            //    for (int j = 0; j < intDataCol; j++)
            //    {
            //        if (adblDisMat[i, j] < 0)
            //        {
            //            MessageBox.Show("Not Metric: A value is smaller than 0!" + "  " + i + ",  " + j);
            //            return;
            //        }
            //    }
            //}

            ////whether a distance from a position to the same position is 0
            //for (int i = 0; i < intDataRow; i++)
            //{
            //    if (adblDisMat[i, i] != 0)
            //    {
            //        MessageBox.Show("Not Metric: A distance from a position to the same position is not 0!" + "  " + i + ",  " + i);
            //        return;
            //    }
            //}

            ////whether symmetry
            //for (int i = 0; i < intDataRow; i++)
            //{
            //    for (int j = i + 1; j < intDataCol; j++)
            //    {
            //        if (adblDisMat[i, j] != adblDisMat[j, i])
            //        {
            //            MessageBox.Show("Not Metric: Not symmetry!" + "  " + i + ",  " + j);
            //            return;
            //        }
            //    }
            //}

            ////triangle inequality
            //for (int i = 0; i < intDataRow; i++)
            //{
            //    for (int j = 0; j < intDataRow; j++)
            //    {
            //        for (int k = 0; k < intDataRow; k++)
            //        {
            //            if (adblDisMat[i, j] > (adblDisMat[i, k] + adblDisMat[k, j]))
            //            {
            //                MessageBox.Show("Not Metric: Not triangle inequality!" + "  " + i + ",  " + j + ",  " + k);

            //                double dblij = adblDisMat[i, j];
            //                double dblik = adblDisMat[i, k];
            //                double dblkj = adblDisMat[k, j];

            //                return;
            //            }
            //        }
            //    }
            //}


            //MessageBox.Show("Metric!");
        }



    }
}