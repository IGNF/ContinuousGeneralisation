using System;
using System.IO;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

using MorphingClass.CAid;
using MorphingClass.CEntity;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;


using ESRI.ArcGIS.Catalog;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Editor;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.GeoAnalyst;
using ESRI.ArcGIS.Maplex;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.Geoprocessing;
using ESRI.ArcGIS.AnalysisTools;
using ESRI.ArcGIS.CartographyTools;

using ClipperLib;
using Path = System.Collections.Generic.List<ClipperLib.IntPoint>;
using Paths = System.Collections.Generic.List<System.Collections.Generic.List<ClipperLib.IntPoint>>;

namespace MorphingClass.CMorphingMethods
{
    /// <summary>
    /// Building Growing
    /// </summary>
    /// <remarks>Cpipe: CpgPairIncrCPtEdgeDis</remarks>
    public class CSTS : CMorphingBaseCpg
    {
        public List<CPolygon> MergedCpgLt { get; set; }

        private List<CPolyline> _OLCplLt;
        private List<CPolyline> _OSCplLt;
        private List<CPolyline> _NLCplLt;
        private List<CPolyline> _NSCplLt;


        private double _intRound = 0;
        private static int _intI = 3;
        private double _dblTotalGrow;
        //private double _dblTargetDepsilon;
        private double _dblEpsilon;
        private double _dblDilation;
        private double _dblErosion;
        //private double _dblSimplifyEpsilon;
        private double _dblLambda;

        private static double _dblStartScale;
        private static double _dblTargetScale;


        private static double _dblAreaLimit;
        private static double _dblHoleAreaLimit;


        private static int _intStart = 0;
        private static int _intEnd = _intStart + 1;

        private Dictionary<CValPairIncr<CPolygon>, CptEdgeDis> _CloseCpipeDt;

        public CSTS()
        {

        }

        public CSTS(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline>(ParameterInitialize, 4, 0);
        }


        public void STS(
            //string strBufferStyle, double dblMiterLimit, string strSimplification,
            //double dblLS, double dblSS, int intOutputMapCount
            )
        {
            var pParameterInitialize = _ParameterInitialize;

            //Old Larger-scale, Old Smaller-scale, New Larger-scale, New Smaller-scale
            _OLCplLt = this.ObjCGeoLtLt[0].AsExpectedClassEb<CPolyline, CGeoBase>().ToList();
            _OSCplLt = this.ObjCGeoLtLt[1].AsExpectedClassEb<CPolyline, CGeoBase>().ToList();
            _NLCplLt = this.ObjCGeoLtLt[2].AsExpectedClassEb<CPolyline, CGeoBase>().ToList();
            _NSCplLt = this.ObjCGeoLtLt[3].AsExpectedClassEb<CPolyline, CGeoBase>().ToList();

            Output(0.00, 0.00);
            Output(0.33, 0.00);
            Output(0.67, 0.00);
            Output(1.00, 0.00);

            Output(0.00, 0.33);
            Output(0.33, 0.33);
            Output(0.67, 0.33);
            Output(1.00, 0.33);

            Output(0.00, 0.67);
            Output(0.33, 0.67);
            Output(0.67, 0.67);
            Output(1.00, 0.67);

            Output(0.00, 1.00);
            Output(0.33, 1.00);
            Output(0.67, 1.00);
            Output(1.00, 1.00);



            //Output(0.3,0);
            //Output(0, 0.6);
            //Output(0.7, 0.2);
            //Output(0.6, 0.9);
        }


        //Stopwatch _pstopwatch = new Stopwatch();
        #region Output
        public void Output(double dblT, double dblS)
        {
            var strTS = _ParameterInitialize.strTS;
            var x = dblT;
            var y = dblS;

            if (strTS == "vario_vario")
            {

            }
            else if (strTS == "vario_separate")
            {
                y = SetForSeparate(dblS, 0.5);
            }
            else if (strTS == "separate_vario")
            {
                x = SetForSeparate(dblT, 0.5);
            }
            else //if (strTS == "separate_separate")
            {
                x = SetForSeparate(dblT, 0.5);
                y = SetForSeparate(dblS, 0.5);
            }

            var a00 = (1 - x) * (1 - y);    //lower left
            var a10 = x * (1 - y);          //lower right
            var a01 = (1 - x) * y;          //upper left
            var a11 = x * y;                //upper right            

            var dblDiffX = _NSCplLt[0].CptLt[0].X - _OLCplLt[0].CptLt[0].X;
            var dblDiffY = _NSCplLt[0].CptLt[0].Y - _OLCplLt[0].CptLt[0].Y;
            //var dblDiffXPlus= dblDiffX+

            var dblBaseX = dblT * dblDiffX;
            var dblBaseY = dblS * dblDiffY;
            var newCplLt = new List<CPolyline>(_OLCplLt.Count);
            for (int i = 0; i < _OLCplLt.Count; i++)
            {
                var olcpl = _OLCplLt[i];
                var oscpl = _OSCplLt[i];
                var nlcpl = _NLCplLt[i];
                var nscpl = _NSCplLt[i];

                var newcptlt = new List<CPoint>(olcpl.CptLt.Count);
                for (int j = 0; j < olcpl.CptLt.Count; j++)
                {
                    var dblX = BilinearInterpolateUnit(a00, a10, a01, a11, 
                        olcpl.CptLt[j].X - olcpl.CptLt[j].X, nlcpl.CptLt[j].X - olcpl.CptLt[j].X - dblDiffX, 
                        oscpl.CptLt[j].X - olcpl.CptLt[j].X, nscpl.CptLt[j].X - olcpl.CptLt[j].X - dblDiffX);
                    var dblY = BilinearInterpolateUnit(a00, a10, a01, a11,
                        olcpl.CptLt[j].Y - olcpl.CptLt[j].Y, nlcpl.CptLt[j].Y - olcpl.CptLt[j].Y,
                        oscpl.CptLt[j].Y - olcpl.CptLt[j].Y - dblDiffY, nscpl.CptLt[j].Y - olcpl.CptLt[j].Y - dblDiffY);

                    //var dblBaseX = CGeoFunc.GetInbetweenDbl(olcpl.CptLt[j].X, nscpl.CptLt[j].X, dblT);
                    //var dblBaseY = CGeoFunc.GetInbetweenDbl(olcpl.CptLt[j].Y, nscpl.CptLt[j].Y, dblS);
                    newcptlt.Add(new CPoint(j, dblX + dblBaseX+ olcpl.CptLt[j].X, dblY + dblBaseY+ olcpl.CptLt[j].Y));
                }
                newCplLt.Add(new CPolyline(i, newcptlt));
            }

            CSaveFeature.SaveCplEb(newCplLt, dblT.ToString() + " " + dblS.ToString());

            //CConstants.dblVerySmallCoord *= CConstants.dblFclipper;
            //var resultCpgEb = GetResultCpgEb(this.MergedCpgLt, dblProportion, strSimplification, strBufferStyle, dblMiterLimit);
            //CSaveFeature.SaveCpgEb(resultCpgEb,
            //    dblProportion + "_Growing", intBlue: 255, blnVisible: false);
            //CConstants.dblVerySmallCoord /= CConstants.dblFclipper;
        }

        private double BilinearInterpolateUnit(double x, double y, double dbl00, double dbl10, double dbl01, double dbl11)
        {
            //If we choose a coordinate system in which the four points 
            //where f is known are (0, 0), (0, 1), (1, 0), and(1, 1), 
            //then the interpolation formula simplifies to
            //f(x, y) = f(0,0)(1 - x)(1 - y) + f(1, 0)x(1 - y) + f(0, 1)(1 - x)y + f(1, 1)xy,
            return dbl00 * (1 - x) * (1 - y) + dbl10 * x * (1 - y) + dbl01 * (1 - x) * y + dbl11 * x * y;
        }

        private double BilinearInterpolateUnit(double a00, double a10, double a01, double a11,
            double dbl00, double dbl10, double dbl01, double dbl11)
        {
            //If we choose a coordinate system in which the four points 
            //where f is known are (0, 0), (0, 1), (1, 0), and(1, 1), 
            //then the interpolation formula simplifies to
            //f(x, y) = f(0,0)(1 - x)(1 - y) + f(1, 0)x(1 - y) + f(0, 1)(1 - x)y + f(1, 1)xy,
            return dbl00 * a00 + dbl10 * a10 + dbl01 * a01 + dbl11 * a11;
        }


        private double SetForSeparate(double dblValue, double dblBound)
        {
            if (dblValue <= dblBound)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
        #endregion
    }
}
