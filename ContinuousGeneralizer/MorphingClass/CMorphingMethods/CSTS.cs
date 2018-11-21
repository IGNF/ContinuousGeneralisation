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
    /// STS: space-time-scale model
    /// </summary>
    /// <remarks></remarks>
    public class CSTS : CMorphingBaseCpg
    {
        public List<CPolygon> MergedCpgLt { get; set; }

        private List<CPolyline> _OLCplLt;
        private List<CPolyline> _OSCplLt;
        private List<CPolyline> _NLCplLt;
        private List<CPolyline> _NSCplLt;


        public CSTS()
        {

        }

        public CSTS(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline>(ParameterInitialize, 4, 0);
        }


        public void STS()
        {
            var pParameterInitialize = _ParameterInitialize;
            var pAxMapControl = pParameterInitialize.pAxMapControl;


            pAxMapControl.Dock = DockStyle.None;
            pAxMapControl.Width = 320;
            pAxMapControl.Height = 310;
            double dblWHRation = pAxMapControl.Width / pAxMapControl.Height;

            double dblCenterX = 305;
            double dblCenterY = 345;
            //IPoint cptMC = new PointClass();
            //cptMC.PutCoords(170, 180);

            IEnvelope newEnvelope = new EnvelopeClass();
            double dblRatio = 1;
            newEnvelope.XMin = dblCenterX - pAxMapControl.Width / dblRatio;
            newEnvelope.XMax = dblCenterX + pAxMapControl.Width / dblRatio;
            newEnvelope.YMin = dblCenterY - pAxMapControl.Height / dblRatio;
            newEnvelope.YMax = dblCenterY + pAxMapControl.Height / dblRatio;
            //newEnvelope.XMin = dblCenterX - pAxMapControl.Width ;
            //newEnvelope.XMax = dblCenterX + pAxMapControl.Width ;
            //newEnvelope.YMin = dblCenterY - pAxMapControl.Height ;
            //newEnvelope.YMax = dblCenterY + pAxMapControl.Height;
            pAxMapControl.Extent = newEnvelope;

            //pAxMapControl.MapScale = 1 / 6000000;
            //pAxMapControl.CenterAt(cptMC);
            //Old Larger-scale, Old Smaller-scale, New Larger-scale, New Smaller-scale
            _OLCplLt = this.ObjCGeoLtLt[0].Select(cgeo => cgeo as CPolyline).ToList();
            _OSCplLt = this.ObjCGeoLtLt[1].Select(cgeo => cgeo as CPolyline).ToList();
            _NLCplLt = this.ObjCGeoLtLt[2].Select(cgeo => cgeo as CPolyline).ToList();
            _NSCplLt = this.ObjCGeoLtLt[3].Select(cgeo => cgeo as CPolyline).ToList();


            var olcpl = _OLCplLt[0];
            var oscpl = _OSCplLt[0];
            var nlcpl = _NLCplLt[0];
            var nscpl = _NSCplLt[0];

            double dblXAxisStart = 60;
            double dblXAxisEnd = 620;
            double dblYAxisStart = 70;
            double dblYAxisEnd = 620;

            double dblScaleMidY = (olcpl.CptLt[0].Y + oscpl.CptLt[0].Y) / 2;
            double dblTimeMidX = (olcpl.CptLt[4].X + nlcpl.CptLt[4].X) / 2;

            var strTS = _ParameterInitialize.strTS;
            var LinkCplLt = new List<CPolyline>();
            var SeparatorCplLt = new List<CPolyline>();
            var scaleSeparatorCpl = new CPolyline(
                new CPoint(dblXAxisStart + 5, dblScaleMidY), new CPoint(dblXAxisEnd - 5, dblScaleMidY));
            var timeSeparatorCpl = new CPolyline(
                new CPoint(dblTimeMidX, dblYAxisStart + 5), new CPoint(dblTimeMidX, dblYAxisEnd - 5));

            if (strTS == "vario_vario")
            {
                //time transition
                LinkCplLt.AddRange(GenerateVarioLinkCplEb(olcpl, nlcpl, oscpl, nscpl, 4, 12));
                //scale transition
                LinkCplLt.AddRange(GenerateVarioLinkCplEb(olcpl, oscpl, nlcpl, nscpl, 0, 8));
            }
            else if (strTS == "vario_separate")
            {
                //time transition
                LinkCplLt.AddRange(GenerateVarioLinkCplEb(olcpl, nlcpl, oscpl, nscpl, 4, 12));
                //scale transition 
                LinkCplLt.AddRange(GenerateSeparateLinkCplEb(olcpl, oscpl, nlcpl, nscpl, 0, 8, dblScaleMidY, "Scale"));

                SeparatorCplLt.Add(scaleSeparatorCpl);
            }
            else if (strTS == "separate_vario")
            {
                //time transition
                LinkCplLt.AddRange(GenerateSeparateLinkCplEb(olcpl, nlcpl, oscpl, nscpl, 4, 12, dblTimeMidX, "Time"));                
                //scale transition
                LinkCplLt.AddRange(GenerateVarioLinkCplEb(olcpl, oscpl, nlcpl, nscpl, 0, 8));

                SeparatorCplLt.Add(timeSeparatorCpl);
            }
            else //if (strTS == "separate_separate")
            {
                //time transition
                LinkCplLt.AddRange(GenerateSeparateLinkCplEb(olcpl, nlcpl, oscpl, nscpl, 4, 12, dblTimeMidX, "Time"));
                //scale transition
                LinkCplLt.AddRange(GenerateSeparateLinkCplEb(olcpl, oscpl, nlcpl, nscpl, 0, 8, dblScaleMidY, "Scale"));

                SeparatorCplLt.Add(scaleSeparatorCpl);
                SeparatorCplLt.Add(timeSeparatorCpl);
            }


            CSaveFeature.SaveCplEb(SeparatorCplLt, strTS + "_Separator", intRed: 204, intGreen: 204, intBlue: 204,
                pesriSimpleLineStyle: esriSimpleLineStyle.esriSLSDot);
            //pesriSimpleLineStyle: esriSimpleLineStyle.esriSLSDash);
            CSaveFeature.SaveCplEb(LinkCplLt, strTS + "_Link", intRed: 150, intGreen: 150, intBlue: 150);




            double dblXExtra = 400;
            double dblYExtra = 400;
            var olIRgbColor = CHelpFunc.GenerateIRgbColor(230, 97, 1);
            var nlIRgbColor = CHelpFunc.GenerateIRgbColor(94, 60, 153);


            var cpllt00 = Output(0.00, 0.00, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt10 = Output(0.33, 0.00, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt20 = Output(0.67, 0.00, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt30 = Output(1.00, 0.00, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);

            var cpllt01 = Output(0.00, 0.33, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt11 = Output(0.33, 0.33, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt21 = Output(0.67, 0.33, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt31 = Output(1.00, 0.33, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);

            var cpllt02 = Output(0.00, 0.67, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt12 = Output(0.33, 0.67, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt22 = Output(0.67, 0.67, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt32 = Output(1.00, 0.67, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);

            var cpllt03 = Output(0.00, 1.00, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt13 = Output(0.33, 1.00, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt23 = Output(0.67, 1.00, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);
            var cpllt33 = Output(1.00, 1.00, olIRgbColor, nlIRgbColor, dblXExtra, dblYExtra);

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






            double dblTextSize = 8;
            CDrawInActiveView.DrawArrow(pAxMapControl.ActiveView,
                new CPoint(dblXAxisStart - 30, dblYAxisStart), new CPoint(dblXAxisEnd, dblYAxisStart),6,5 ); //x arrow; horizontal
            CDrawInActiveView.DrawArrow(pAxMapControl.ActiveView,
                new CPoint(dblXAxisStart, dblYAxisStart - 30), new CPoint(dblXAxisStart, dblYAxisEnd),6, 5); //y arrow; vertical            
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "M (scale=1/M)", dblXAxisStart + 20, dblYAxisEnd + 10, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "time", dblXAxisEnd - 20, dblYAxisStart - 25, dblTextSize);

            //scales
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2,000", dblXAxisStart - 40, dblMidY00 - dblTextSize / 2, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "4,000", dblXAxisStart - 40, dblMidY01 - dblTextSize / 2, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "6,000", dblXAxisStart - 40, dblMidY02 - dblTextSize / 2, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "8,000", dblXAxisStart - 40, dblMidY03 - dblTextSize / 2, dblTextSize);

            //times
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2015", dblMidX00, dblYAxisStart - 30, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2016", dblMidX10, dblYAxisStart - 30, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2017", dblMidX20, dblYAxisStart - 30, dblTextSize);
            CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "2018", dblMidX30, dblYAxisStart - 30, dblTextSize);

            var pExportActiveViewCS_Net = new MorphingClass.CCommon.ExportActiveViewCS_Net();
            pExportActiveViewCS_Net.ExportActiveViewParameterized(300, 1, "EMF",
                pParameterInitialize.strMxdPathBackSlash + strTS, true);


            ////inputs
            //CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "input", dblXAxisStart + 30, dblYAxisStart + 15, dblTextSize);
            //CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "input", dblXAxisStart + 30, oscpl.CptLt[0].Y + 40, dblTextSize);
            //CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "input", nlcpl.CptLt[5].X + 10, dblYAxisStart + 15, dblTextSize);
            //CDrawInActiveView.DrawTextMarker(pAxMapControl.ActiveView, "input", nlcpl.CptLt[5].X + 10, oscpl.CptLt[0].Y + 40, dblTextSize);



            //Output(0.3,0);
            //Output(0, 0.6);
            //Output(0.7, 0.2);
            //Output(0.6, 0.9);
        }

        private IEnumerable<CPolyline> GenerateVarioLinkCplEb(CPolyline frcpl1, CPolyline tocpl1, CPolyline frcpl2, CPolyline tocpl2,
            int intIndex1, int intIndex2)
        {
            yield return new CPolyline(frcpl1.CptLt[intIndex1], tocpl1.CptLt[intIndex1]);
            yield return new CPolyline(frcpl1.CptLt[intIndex2], tocpl1.CptLt[intIndex2]);
            yield return new CPolyline(frcpl2.CptLt[intIndex1], tocpl2.CptLt[intIndex1]);
            yield return new CPolyline(frcpl2.CptLt[intIndex2], tocpl2.CptLt[intIndex2]);
        }

        private IEnumerable<CPolyline> GenerateSeparateLinkCplEb(CPolyline frcpl1, CPolyline tocpl1, CPolyline frcpl2, CPolyline tocpl2,
    int intIndex1, int intIndex2, double dblMid, string strDimension)
        {
            var SeparateLinkCplEb1 = GenerateOneSeparateLinkCplEb(frcpl1, tocpl1, intIndex1, intIndex2, dblMid, strDimension);
            var SeparateLinkCplEb2 = GenerateOneSeparateLinkCplEb(frcpl2, tocpl2, intIndex1, intIndex2, dblMid, strDimension);
            return SeparateLinkCplEb1.Concat(SeparateLinkCplEb2);
        }

        private IEnumerable<CPolyline> GenerateOneSeparateLinkCplEb(CPolyline frcpl, CPolyline tocpl, int intIndex1, int intIndex2, double dblMid, string strDimension)
        {
            CPoint frmidLeftCpt;
            CPoint tomidLeftCpt;
            CPoint frmidRightCpt;
            CPoint tomidRightCpt;
            if (strDimension == "Time")
            {
                frmidLeftCpt = new CPoint(dblMid, frcpl.CptLt[intIndex1].Y);
                tomidLeftCpt = new CPoint(dblMid, tocpl.CptLt[intIndex1].Y);
                frmidRightCpt = new CPoint(dblMid, frcpl.CptLt[intIndex2].Y);
                tomidRightCpt = new CPoint(dblMid, tocpl.CptLt[intIndex2].Y);
            }
            else //if (strDimension == "scale")
            {
                frmidLeftCpt = new CPoint(frcpl.CptLt[intIndex1].X, dblMid);
                tomidLeftCpt = new CPoint(tocpl.CptLt[intIndex1].X, dblMid);
                frmidRightCpt = new CPoint(frcpl.CptLt[intIndex2].X, dblMid);
                tomidRightCpt = new CPoint(tocpl.CptLt[intIndex2].X, dblMid);
            }                        

            yield return new CPolyline(frcpl.CptLt[intIndex1], frmidLeftCpt);
            yield return new CPolyline(frmidLeftCpt, tomidLeftCpt);
            yield return new CPolyline(tomidLeftCpt, tocpl.CptLt[intIndex1]);
            yield return new CPolyline(frcpl.CptLt[intIndex2], frmidRightCpt);
            yield return new CPolyline(frmidRightCpt, tomidRightCpt);
            yield return new CPolyline(tomidRightCpt, tocpl.CptLt[intIndex2]);
        }


        //Stopwatch _pstopwatch = new Stopwatch();
        #region Output
        public List<CPolyline> Output(double dblT, double dblS, IRgbColor olIRgbColor, IRgbColor nlIRgbColor, double dblExtraX = 300, double dblExtraY = 300)
        {
            var strTS = _ParameterInitialize.strTS;
            var dblt = dblT;
            var dbls = dblS;

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

            double dblRTime = CGeoFunc.GetInbetweenDbl(olIRgbColor.Red, nlIRgbColor.Red, dblt);
            double dblGTime = CGeoFunc.GetInbetweenDbl(olIRgbColor.Green, nlIRgbColor.Green, dblt);
            double dblBTime = CGeoFunc.GetInbetweenDbl(olIRgbColor.Blue, nlIRgbColor.Blue, dblt);
            int intRTS = Convert.ToInt32(CGeoFunc.GetInbetweenDbl(dblRTime, CGeoFunc.GetInbetweenDbl(dblRTime, 255, 0.8), dbls));
            int intGTS = Convert.ToInt32(CGeoFunc.GetInbetweenDbl(dblGTime, CGeoFunc.GetInbetweenDbl(dblGTime, 255, 0.8), dbls));
            int intBTS = Convert.ToInt32(CGeoFunc.GetInbetweenDbl(dblBTime, CGeoFunc.GetInbetweenDbl(dblBTime, 255, 0.8), dbls));

            var olcpl = _OLCplLt[0];
            var oscpl = _OSCplLt[0];
            var nlcpl = _NLCplLt[0];
            var nscpl = _NSCplLt[0];

            var olcplmoved = olcpl;
            var oscplmoved = oscpl.Move(0, -dblExtraY);
            var nlcplmoved = nlcpl.Move(-dblExtraX, 0);
            var nscplmoved = nscpl.Move(-dblExtraX, -dblExtraY);


            var InterLargerCpl = CGeoFunc.GetInbetweenCpl(olcplmoved, nlcplmoved, dblt);
            var InterSmallerCpl = CGeoFunc.GetInbetweenCpl(oscplmoved, nscplmoved, dblt);
            var InterCpl = CGeoFunc.GetInbetweenCpl(InterLargerCpl, InterSmallerCpl, dbls);

            //var simplifedcpllt = CDPSimplify.DPSimplify(CHelpFunc.MakeLt(InterCpl), dblThresholdDis: 300000 * CConstants.dblVerySmallCoord);
            int intPtNum = olcpl.CptLt.Count - 1;
            var simplifedcpllt = CDPSimplify.DPSimplify(CHelpFunc.MakeLt(InterCpl), dblRemainPoints: intPtNum * ((1 - dbls) * 3 + 1) / 4);

            //var simplifedcptlt = CImaiIriSimplify.ImaiIriSimplify(InterCpl.CptLt, 100000 * CConstants.dblVerySmallCoord).ToList();
            //CDPSimplify.dp
            //CSaveFeature.SaveCpl(InterLargerCpl, "InterLargerCpl_" + dblT.ToString() + " " + dblS.ToString());
            //CSaveFeature.SaveCpl(InterSmallerCpl, "InterSmallerCpl_" + dblT.ToString() + " " + dblS.ToString());
            //CSaveFeature.SaveCpl(InterCpl, "InterCpl_" + dblT.ToString() + " " + dblS.ToString());


            var dblBaseX = dblT * dblExtraX;
            var dblBaseY = dblS * dblExtraY;
            var movedInterCpl = new CPolyline(0, CGeoFunc.MoveCptEb(simplifedcpllt[0].CptLt, dblBaseX, dblBaseY).ToList());

            //CSaveFeature.SaveCpl(movedInterCpl, dblT.ToString() + " " + dblS.ToString());
            var movedInterCpg = new CPolygon(movedInterCpl.CptLt);
            CSaveFeature.SaveCpg(movedInterCpg, dblT.ToString() + " " + dblS.ToString(), intRed: intRTS, intGreen: intGTS, intBlue: intBTS);


            var cpbLt = new List<CPolyBase>
            {movedInterCpl };
            CCreatePointLayer.SavePointLayer(cpbLt, dblT.ToString() + " " + dblS.ToString(), 3);

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
