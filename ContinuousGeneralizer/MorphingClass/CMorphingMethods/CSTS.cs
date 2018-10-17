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
using MorphingClass.CGeneralizationMethods;


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
            var pAxMapControl = pParameterInitialize.pAxMapControl;

            
            pAxMapControl.Dock = DockStyle.None;
            pAxMapControl.Width = 320;
            pAxMapControl.Height = 320;
            double dblWHRation = pAxMapControl.Width / pAxMapControl.Height;

            double dblCenterX = 155;
            double dblCenterY = 180;
            //IPoint cptMC = new PointClass();
            //cptMC.PutCoords(170, 180);

            IEnvelope newEnvelope = new EnvelopeClass();
            newEnvelope.XMin = dblCenterX - pAxMapControl.Width / 1.2;
            newEnvelope.XMax = dblCenterX + pAxMapControl.Width / 1.2;
            newEnvelope.YMin = dblCenterY - pAxMapControl.Height / 1.2;
            newEnvelope.YMax = dblCenterY + pAxMapControl.Height / 1.2;
            //newEnvelope.XMin = dblCenterX - pAxMapControl.Width ;
            //newEnvelope.XMax = dblCenterX + pAxMapControl.Width ;
            //newEnvelope.YMin = dblCenterY - pAxMapControl.Height ;
            //newEnvelope.YMax = dblCenterY + pAxMapControl.Height;
            pAxMapControl.Extent = newEnvelope;

            //pAxMapControl.MapScale = 1 / 6000000;
            //pAxMapControl.CenterAt(cptMC);
            //Old Larger-scale, Old Smaller-scale, New Larger-scale, New Smaller-scale
            _OLCplLt = this.ObjCGeoLtLt[0].AsExpectedClassEb<CPolyline, CGeoBase>().ToList();
            _OSCplLt = this.ObjCGeoLtLt[1].AsExpectedClassEb<CPolyline, CGeoBase>().ToList();
            _NLCplLt = this.ObjCGeoLtLt[2].AsExpectedClassEb<CPolyline, CGeoBase>().ToList();
            _NSCplLt = this.ObjCGeoLtLt[3].AsExpectedClassEb<CPolyline, CGeoBase>().ToList();



            var cpllt00 = Output(0.00, 0.00);
            var cpllt10 = Output(0.33, 0.00);
            var cpllt20 = Output(0.67, 0.00);
            var cpllt30 = Output(1.00, 0.00);

            var cpllt01 = Output(0.00, 0.33);
            var cpllt11 = Output(0.33, 0.33);
            var cpllt21 = Output(0.67, 0.33);
            var cpllt31 = Output(1.00, 0.33);

            var cpllt02 = Output(0.00, 0.67);
            var cpllt12 = Output(0.33, 0.67);
            var cpllt22 = Output(0.67, 0.67);
            var cpllt32 = Output(1.00, 0.67);

            var cpllt03 = Output(0.00, 1.00);
            var cpllt13 = Output(0.33, 1.00);
            var cpllt23 = Output(0.67, 1.00);
            var cpllt33 = Output(1.00, 1.00);

            double dblMinY00 = cpllt00[0].CptLt.Min(cpt => cpt.Y);
            double dblMinY01 = cpllt01[0].CptLt.Min(cpt => cpt.Y);
            double dblMinY02 = cpllt02[0].CptLt.Min(cpt => cpt.Y);
            double dblMinY03 = cpllt03[0].CptLt.Min(cpt => cpt.Y);

            double dblMaxY00 = cpllt00[0].CptLt.Max(cpt => cpt.Y);
            double dblMaxY01 = cpllt01[0].CptLt.Max(cpt => cpt.Y);
            double dblMaxY02 = cpllt02[0].CptLt.Max(cpt => cpt.Y);
            double dblMaxY03 = cpllt03[0].CptLt.Max(cpt => cpt.Y);

            double dblMidY00 = (dblMinY00 + dblMaxY00) / 2;
            double dblMidY01 = (dblMinY01 + dblMaxY01) / 2;
            double dblMidY02 = (dblMinY02 + dblMaxY02) / 2;
            double dblMidY03 = (dblMinY03 + dblMaxY03) / 2;


            double dblMinX00 = cpllt00[0].CptLt.Min(cpt => cpt.X);
            double dblMinX10 = cpllt10[0].CptLt.Min(cpt => cpt.X);
            double dblMinX20 = cpllt20[0].CptLt.Min(cpt => cpt.X);
            double dblMinX30 = cpllt30[0].CptLt.Min(cpt => cpt.X);

            double dblMaxX00 = cpllt00[0].CptLt.Max(cpt => cpt.X);
            double dblMaxX10 = cpllt10[0].CptLt.Max(cpt => cpt.X);
            double dblMaxX20 = cpllt20[0].CptLt.Max(cpt => cpt.X);
            double dblMaxX30 = cpllt30[0].CptLt.Max(cpt => cpt.X);

            double dblMidX00 = (dblMinX00 + dblMaxX00) / 2;
            double dblMidX10 = (dblMinX10 + dblMaxX10) / 2;
            double dblMidX20 = (dblMinX20 + dblMaxX20) / 2;
            double dblMidX30 = (dblMinX30 + dblMaxX30) / 2;









            var olcpl = _OLCplLt[0];
            var oscpl = _OSCplLt[0];
            var nlcpl = _NLCplLt[0];
            var nscpl = _NSCplLt[0];

            double dblXAxisStart = -30;
            double dblXAxisEnd = 400;
            double dblYAxisStart = -30;
            double dblYAxisEnd = 400;

            var strTS = _ParameterInitialize.strTS;
            var LinkCplLt = new List<CPolyline>();
            var BreakCplLt = new List<CPolyline>();
            int intIndex = 0;
            if (strTS == "vario_vario")
            {
                //time transition
                LinkCplLt.Add(new CPolyline(intIndex++, olcpl.CptLt[1], nlcpl.CptLt[1]));
                LinkCplLt.Add(new CPolyline(intIndex++, olcpl.CptLt[9], nlcpl.CptLt[9]));
                LinkCplLt.Add(new CPolyline(intIndex++, oscpl.CptLt[2], nscpl.CptLt[2]));
                LinkCplLt.Add(new CPolyline(intIndex++, oscpl.CptLt[8], nscpl.CptLt[8]));

                //scale transition
                LinkCplLt.Add(new CPolyline(intIndex++, olcpl.CptLt[0], oscpl.CptLt[0]));
                LinkCplLt.Add(new CPolyline(intIndex++, olcpl.CptLt[2], oscpl.CptLt[2]));
                LinkCplLt.Add(new CPolyline(intIndex++, nlcpl.CptLt[0], nscpl.CptLt[0]));
                LinkCplLt.Add(new CPolyline(intIndex++, nlcpl.CptLt[2], nscpl.CptLt[2]));
            }
            else if (strTS == "vario_separate")
            {
                //time transition
                //LinkCplLt.Add(new CPolyline(intIndex++, olcpl.CptLt[1], nlcpl.CptLt[1]));
                //LinkCplLt.Add(new CPolyline(intIndex++, olcpl.CptLt[9], nlcpl.CptLt[9]));
                //LinkCplLt.Add(new CPolyline(intIndex++, oscpl.CptLt[2], nscpl.CptLt[2]));
                //LinkCplLt.Add(new CPolyline(intIndex++, oscpl.CptLt[8], nscpl.CptLt[8]));

                ////scale transition
                //LinkCplLt.Add(new CPolyline(intIndex++, olcpl.CptLt[0], oscpl.CptLt[0]));
                //LinkCplLt.Add(new CPolyline(intIndex++, olcpl.CptLt[2], oscpl.CptLt[2]));
                //LinkCplLt.Add(new CPolyline(intIndex++, nlcpl.CptLt[0], nscpl.CptLt[0]));
                //LinkCplLt.Add(new CPolyline(intIndex++, nlcpl.CptLt[2], nscpl.CptLt[2]));


            }
            else if (strTS == "separate_vario")
            {
            }
            else //if (strTS == "separate_separate")
            {

            }

            CSaveFeature.SaveCplEb(LinkCplLt, strTS + "_Help", intRed: 128, intGreen: 128, intBlue: 128);
                //pesriSimpleLineStyle: esriSimpleLineStyle.esriSLSDash);




            double dblTextSize = 8;
            CDrawInActiveView.DrawArrow(pAxMapControl.ActiveView, 
                new CPoint(-1, dblXAxisStart - 30, -30), new CPoint(-1, dblXAxisEnd, -30)); //x arrow; horizontal
            CDrawInActiveView.DrawArrow(pAxMapControl.ActiveView, 
                new CPoint(-1, -30, dblYAxisStart - 30), new CPoint(-1, -30, dblYAxisEnd)); //y arrow; vertical            
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "M (scale=1/M)", -20, 410, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "time", 380, -10, dblTextSize);

            //scales
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "1,000", -60, dblMidY00, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2,000", -60, dblMidY01, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "3,000", -60, dblMidY02, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "4,000", -60, dblMidY03, dblTextSize);

            //times
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2015", dblMidX00, -50, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2016", dblMidX10, -50, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2017", dblMidX20, -50, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2018", dblMidX30, -50, dblTextSize);





            //Output(0.3,0);
            //Output(0, 0.6);
            //Output(0.7, 0.2);
            //Output(0.6, 0.9);
        }


        //Stopwatch _pstopwatch = new Stopwatch();
        #region Output
        public List<CPolyline> Output(double dblT, double dblS)
        {
            var strTS = _ParameterInitialize.strTS;
            var dblt = dblT;
            var dbls = dblS;
            double dblExtraX = 300;
            double dblExtraY = 300;

            if (strTS == "vario_vario")
            {

            }
            else if (strTS == "vario_separate")
            {
                dbls = SetForSeparate(dblS, 0.5);
            }
            else if (strTS == "separate_vario")
            {
                dblt = SetForSeparate(dblT, 0.5);
            }
            else //if (strTS == "separate_separate")
            {
                dblt = SetForSeparate(dblT, 0.5);
                dbls = SetForSeparate(dblS, 0.5);
            }

            var olcpl = _OLCplLt[0];
            var oscpl = _OSCplLt[0];
            var nlcpl = _NLCplLt[0];
            var nscpl = _NSCplLt[0];

            var olcplmoved = olcpl;
            var oscplmoved = oscpl.Move(0, -dblExtraY);
            var nlcplmoved = nlcpl.Move(-dblExtraX, 0);
            var nscplmoved = nscpl.Move(-dblExtraX, -dblExtraY);

            //double dblMaxDis = olcpl.CptLt[9].DistanceTo(oscpl.CptLt[9]);
            //double dblMaxDis = 22.36068;
            //double dblMoveT = dblMaxDis * dblT;
            //double dblMoveS = dblMaxDis * dblS;

            //var InterLargerCpl = InterpolateCpl(olcplmoved, nlcplmoved, dblT);
            //var InterSmallerCpl = InterpolateCpl(oscplmoved, nscplmoved, dblT);
            //var InterCpl = InterpolateCpl(InterLargerCpl, InterSmallerCpl, dblS);


            var InterLargerCpl = CGeoFunc.GetInbetweenCpl(olcplmoved, nlcplmoved, dblt);
            var InterSmallerCpl = CGeoFunc.GetInbetweenCpl(oscplmoved, nscplmoved, dblt);
            var InterCpl = CGeoFunc.GetInbetweenCpl(InterLargerCpl, InterSmallerCpl, dbls);

            var simplifedcpllt = CDPSimplify.DPSimplify(CHelpFunc.MakeLt(InterCpl), dblThresholdDis: 200000 * CConstants.dblVerySmallCoord);


            //var simplifedcptlt = CImaiIriSimplify.ImaiIriSimplify(InterCpl.CptLt, 100000 * CConstants.dblVerySmallCoord).ToList();
            //CDPSimplify.dp
            //CSaveFeature.SaveCpl(InterLargerCpl, "InterLargerCpl_" + dblT.ToString() + " " + dblS.ToString());
            //CSaveFeature.SaveCpl(InterSmallerCpl, "InterSmallerCpl_" + dblT.ToString() + " " + dblS.ToString());
            //CSaveFeature.SaveCpl(InterCpl, "InterCpl_" + dblT.ToString() + " " + dblS.ToString());


            var dblBaseX = dblT * dblExtraX;
            var dblBaseY = dblS * dblExtraY;
            var movedInterCpl = new CPolyline(0, CGeoFunc.MoveCptEb(simplifedcpllt[0].CptLt, dblBaseX, dblBaseY).ToList());

            CSaveFeature.SaveCpl(movedInterCpl, dblT.ToString() + " " + dblS.ToString());

            return CHelpFunc.MakeLt(movedInterCpl);






            //var a00 = (1 - x) * (1 - y);    //lower left
            //var a10 = x * (1 - y);          //lower right
            //var a01 = (1 - x) * y;          //upper left
            //var a11 = x * y;                //upper right            

            ////var dblDiffX = _NSCplLt[0].CptLt[0].X - _OLCplLt[0].CptLt[0].X;
            ////var dblDiffY = _NSCplLt[0].CptLt[0].Y - _OLCplLt[0].CptLt[0].Y;

            //var dblDiffX = 300;
            //var dblDiffY = 400;

            ////var dblDiffXPlus= dblDiffX+

            //var dblBaseX = dblT * dblDiffX;
            //var dblBaseY = dblS * dblDiffY;
            //var newCplLt = new List<CPolyline>(_OLCplLt.Count);
            //for (int i = 0; i < _OLCplLt.Count; i++)
            //{
            //    var olcpl = _OLCplLt[i];
            //    var oscpl = _OSCplLt[i];
            //    var nlcpl = _NLCplLt[i];
            //    var nscpl = _NSCplLt[i];

            //    var newcptlt = new List<CPoint>(olcpl.CptLt.Count);
            //    for (int j = 0; j < olcpl.CptLt.Count; j++)
            //    {
            //        var dblX = BilinearInterpolateUnit(a00, a10, a01, a11,
            //            olcpl.CptLt[j].X - olcpl.CptLt[j].X, nlcpl.CptLt[j].X - olcpl.CptLt[j].X - dblDiffX,
            //            oscpl.CptLt[j].X - olcpl.CptLt[j].X, nscpl.CptLt[j].X - olcpl.CptLt[j].X - dblDiffX);
            //        var dblY = BilinearInterpolateUnit(a00, a10, a01, a11,
            //            olcpl.CptLt[j].Y - olcpl.CptLt[j].Y, nlcpl.CptLt[j].Y - olcpl.CptLt[j].Y,
            //            oscpl.CptLt[j].Y - olcpl.CptLt[j].Y - dblDiffY, nscpl.CptLt[j].Y - olcpl.CptLt[j].Y - dblDiffY);

            //        newcptlt.Add(new CPoint(j, dblX + dblBaseX + olcpl.CptLt[j].X, dblY + dblBaseY + olcpl.CptLt[j].Y));
            //    }

            //    CImaiIriSimplify.ImaiIriSimplify(newcptlt, CConstants.dblVerySmallCoord);

            //    newCplLt.Add(new CPolyline(i, newcptlt));
            //}

            //CSaveFeature.SaveCplEb(newCplLt, dblT.ToString() + " " + dblS.ToString());

        }

        private CPolyline InterpolateCpl(CPolyline frcpl, CPolyline tocpl, double dblProp)
        {
            var dblMaxDis = CGeoFunc.FindFarthestDisCpls(frcpl, tocpl);
            var dblAbsoluteMove = dblMaxDis * dblProp;
            var newCptLt = new List<CPoint>(frcpl.CptLt.Count);
            for (int i = 0; i < frcpl.CptLt.Count; i++)
            {
                double dblDis = frcpl.CptLt[i].DistanceTo(tocpl.CptLt[i]);

                if (dblAbsoluteMove < dblDis)
                {
                    double dblPropMove = dblAbsoluteMove / dblDis;
                    var newcpt= CGeoFunc.GetInbetweenCpt(frcpl.CptLt[i], tocpl.CptLt[i], dblPropMove);
                    newCptLt.Add(newcpt);
                }
                else
                {
                    newCptLt.Add(tocpl.CptLt[i]);
                }
            }

            return new CPolyline(frcpl.ID, newCptLt);
        }


        //public void Output(double dblT, double dblS)
        //{
        //    var strTS = _ParameterInitialize.strTS;
        //    var x = dblT;
        //    var y = dblS;

        //    if (strTS == "vario_vario")
        //    {

        //    }
        //    else if (strTS == "vario_separate")
        //    {
        //        y = SetForSeparate(dblS, 0.5);
        //    }
        //    else if (strTS == "separate_vario")
        //    {
        //        x = SetForSeparate(dblT, 0.5);
        //    }
        //    else //if (strTS == "separate_separate")
        //    {
        //        x = SetForSeparate(dblT, 0.5);
        //        y = SetForSeparate(dblS, 0.5);
        //    }

        //    var a00 = (1 - x) * (1 - y);    //lower left
        //    var a10 = x * (1 - y);          //lower right
        //    var a01 = (1 - x) * y;          //upper left
        //    var a11 = x * y;                //upper right            

        //    //var dblDiffX = _NSCplLt[0].CptLt[0].X - _OLCplLt[0].CptLt[0].X;
        //    //var dblDiffY = _NSCplLt[0].CptLt[0].Y - _OLCplLt[0].CptLt[0].Y;

        //    var dblDiffX = 300;
        //    var dblDiffY = 400;

        //    //var dblDiffXPlus= dblDiffX+

        //    var dblBaseX = dblT * dblDiffX;
        //    var dblBaseY = dblS * dblDiffY;
        //    var newCplLt = new List<CPolyline>(_OLCplLt.Count);
        //    for (int i = 0; i < _OLCplLt.Count; i++)
        //    {
        //        var olcpl = _OLCplLt[i];
        //        var oscpl = _OSCplLt[i];
        //        var nlcpl = _NLCplLt[i];
        //        var nscpl = _NSCplLt[i];

        //        var newcptlt = new List<CPoint>(olcpl.CptLt.Count);
        //        for (int j = 0; j < olcpl.CptLt.Count; j++)
        //        {
        //            var dblX = BilinearInterpolateUnit(a00, a10, a01, a11,
        //                olcpl.CptLt[j].X - olcpl.CptLt[j].X, nlcpl.CptLt[j].X - olcpl.CptLt[j].X - dblDiffX,
        //                oscpl.CptLt[j].X - olcpl.CptLt[j].X, nscpl.CptLt[j].X - olcpl.CptLt[j].X - dblDiffX);
        //            var dblY = BilinearInterpolateUnit(a00, a10, a01, a11,
        //                olcpl.CptLt[j].Y - olcpl.CptLt[j].Y, nlcpl.CptLt[j].Y - olcpl.CptLt[j].Y,
        //                oscpl.CptLt[j].Y - olcpl.CptLt[j].Y - dblDiffY, nscpl.CptLt[j].Y - olcpl.CptLt[j].Y - dblDiffY);

        //            newcptlt.Add(new CPoint(j, dblX + dblBaseX + olcpl.CptLt[j].X, dblY + dblBaseY + olcpl.CptLt[j].Y));
        //        }

        //        CImaiIriSimplify.ImaiIriSimplify(newcptlt, CConstants.dblVerySmallCoord);

        //        newCplLt.Add(new CPolyline(i, newcptlt));
        //    }

        //    CSaveFeature.SaveCplEb(newCplLt, dblT.ToString() + " " + dblS.ToString());
        //}



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
