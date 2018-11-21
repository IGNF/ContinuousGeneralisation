using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;


using MorphingClass.CEntity;
using MorphingClass.CEvaluationMethods;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CMorphingAlgorithms;
using MorphingClass.CCorrepondObjects;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;

namespace MorphingClass.CAid
{
    public class CSelectRandomly : CMorphingBaseCpl
    {


        public CSelectRandomly()
        {

        }


        public CSelectRandomly(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline>(ParameterInitialize, 0, 1, false, blnCalDistanceParameters: false);
        }


        public void SelectRandomly(double dblPortion)
        {
            var iptlt = this.ObjIGeoLtLt[0].Select(igeo => igeo as IPoint).ToList();

            var remainIptLt = new List<IPoint>(Convert.ToInt32(dblPortion * iptlt.Count));
            var rand = new Random();
            foreach (var ipt in iptlt)
            {
                var dblrand = rand.NextDouble();
                if (dblrand < dblPortion)
                {
                    remainIptLt.Add(ipt);
                }
            }

            CSaveFeature.SaveIGeoEb(remainIptLt, esriGeometryType.esriGeometryPoint, dblPortion.ToString());



            //long lngEndTime = System.Environment.TickCount;//记录结束时间
            //ParameterInitialize.tsslTime.Text = "Running Time: " + Convert.ToString(lngEndTime - lngStartTime) + "ms";  //显示运行时
        }
    }
}
