using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CGeometry;
using ESRI.ArcGIS.Geometry;

namespace MorphingClass.CUtility
{
    /**
 * This class helps creating ipe-files in Java by providing functions for most
 * common objects. Currently supports: Mark, Rectangle, Path, Edge, Text
 * label, Circle, Circular Arc, Spline, Splinegon
 * 
 * Usage: The functions create an UML-string that corresponds to the specified
 * object in Ipe. Every file has to start with getIpePreamble(), followed by
 * getIpeConf(), and has to end with getIpeEnd().
 * 
 * @author Martin Fink
 * @author Philipp Kindermann
 */
    public static class CIpeDraw
    {

        /**
         * Draws a mark.
         * 
         * @param x
         *            x-coordinate
         * @param y
         *            y-coordinate
         * @param shape
         * 			  shape: disk, fdisk, circle, box, square, fsquare, cross
         * @param color
         *            color
         * @param size
         *            size: tiny, small, normal, large
         * @return
         */
        public static string drawIpeMark(double x, double y, string shape = "disk", string color = "black", string size = "normal")
        {
            return "<use name=\"mark/" + shape + "(sx)\" pos=\"" + AddXY(x, y)
                    + "\" size=\"" + size + "\" stroke=\"" + color + "\"/>\n";
        }

        /**
         * Draws a rectangle.
         * 
         * @param x1
         *            left-most x-coordinate
         * @param y1
         *            bottom-most y-coordinate
         * @param x2
         *            right-most x-coordinate
         * @param y2
         *            top-most y-coordinate
         * @param color
         *            color
         * @param pen
         *            pen width: normal, heavier, fat, ultrafat
         * @param dash
         *            dash style: normal, dashed, dotted, dash dotted, dash dot
         *            dotted
         * @return
         */
        public static string drawIpeBox(int x1, int y1, int x2, int y2,
                string outlinecolor = "black", string fillcolor = "white", string pen = "normal", string dash = "normal")
        {
            return "<path stroke=\"" + outlinecolor + "\" fill=\"" + fillcolor + "\" pen=\"" + pen + "\" dash=\""
                    + dash + "\">\n " + x1 + " " + y2 + " m\n " + x1 + " " + y1
                    + " l\n " + x2 + " " + y1 + " l\n " + x2 + " " + y2
                    + " l\n h\n" + "</path>\n";
        }




        /// <summary>
        /// 
        /// </summary>
        /// <param name="x">x-coordinates of the points</param>
        /// <param name="y">y-coordinates of the points</param>
        /// <param name="color"></param>
        /// <param name="pen">normal, heavier, fat, ultrafat</param>
        /// <param name="dash">normal, dashed, dotted, dash dotted, dash dot dotted</param>
        /// <param name="cap">normal: butt;      0: butt;      1: round;      2: square</param>
        /// <param name="join">normal: round;      0: miter;      1: round;      2: bevel</param>
        /// <returns></returns>
        public static string drawIpePath(double[] x, double[] y, string color = "black", 
            string pen = "normal", string dash = "normal", string cap = "normal", string join = "normal")
        {
            string s = "<path stroke=\"" + color + "\"";
            s += AddOtherAttribute(pen, dash, cap, join) + ">\n";

            //add coordinates
            s += AddXY(x[0], y[0]) + " m\n";
            for (int i = 1; i < x.Length; i++)
            {
                s += AddXY(x[i], y[i]) + " l\n";
            }
            s += "</path>\n";
            return s;
        }

        public static string drawIpeBoxPath(double dblXMin, double dblYMin, double dblXMax, double dblYMax, 
            string color = "black", string pen = "normal", string dash = "normal")
        {
            string s = "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
                    + dash + "\">\n" + AddXY(dblXMin, dblYMin) + " m\n";
            s += AddXY(dblXMax, dblYMin) + " l\n";
            s += AddXY(dblXMax, dblYMax) + " l\n";
            s += AddXY(dblXMin, dblYMax) + " l\n";
            s += AddXY(dblXMin, dblYMin) + " l\n";
            s += "</path>\n";
            return s;
        }

        /**
        * Draws an edge between two points.
        * 
        * @param x1
        *            x-coordinate of point 1
        * @param y1
        *            y-coordinate of point 1
        * @param x2
        *            x-coordinate of point 2
        * @param y2
        *            y-coordinate of point 2
        * @param color
        *            color
        * @param pen
        *            pen width: normal, heavier, fat, ultrafat
        * @param dash
        *            dash style: normal, dashed, dotted, dash dotted, dash dot
        *            dotted
        * @return
        */
        public static string drawIpeEdge(double x1, double y1, double x2, double y2, string color = "black",
            string pen = "normal", string dash = "normal", string cap = "normal", string join = "normal")
        {
            return drawIpePath(new double[] { x1, x2 }, new double[] { y1, y2 }, color, pen, dash, cap, join);
        }


        public static string DrawIpt(IPoint ipt, IEnvelope pEnvelopeLayer, CEnvelope pEnvelopeIpe, string strShape,
    CColor StrokeColor, string strWidth = "normal")
        {
            double dblFactor = pEnvelopeIpe.Height / pEnvelopeLayer.Height;
            double dblx = CGeoFunc.CoordinateTransform(
                    ipt.X, pEnvelopeLayer.XMin, pEnvelopeIpe.XMin, dblFactor);
            double dbly = CGeoFunc.CoordinateTransform(
                    ipt.Y, pEnvelopeLayer.YMin, pEnvelopeIpe.YMin, dblFactor);

            return drawIpeMark(dblx, dbly, strShape, AddColor(StrokeColor), strWidth);
        }



        public static string DrawCpl(CPolyline cpl, IEnvelope pEnvelopeLayer, CEnvelope pEnvelopeIpe,
            CColor StrokeColor, string strWidth = "normal")
        {
            double dblFactor = pEnvelopeIpe.Height / pEnvelopeLayer.Height;

            var cptlt = cpl.CptLt;

            string str = GetStrokeStarting(StrokeColor, strWidth, "normal", "1", "1", null);
            str += AddCoordinates(cptlt, pEnvelopeLayer, pEnvelopeIpe, dblFactor);
            str += "</path>\n";

            return str;
        }

        public static string DrawCpg(CPolygon cpg, IEnvelope pEnvelopeLayer, CEnvelope pEnvelopeIpe,
            CColor StrokeColor, CColor FillColor, string strWidth = "normal", string strDash = "normal")
        {
            double dblFactor = pEnvelopeIpe.Height / pEnvelopeLayer.Height;
            string str = GetStrokeStarting(StrokeColor, strWidth, strDash, "1", "1", FillColor);
            str += AddPolygonCoordinates(cpg.CptLt, pEnvelopeLayer, pEnvelopeIpe, dblFactor);

            //draw holes and fill wholes with white
            if (cpg.HoleCpgLt != null && cpg.HoleCpgLt.Count > 0)
            {
                foreach (var holecpg in cpg.HoleCpgLt)
                {
                    str += AddPolygonCoordinates(holecpg.CptLt, pEnvelopeLayer, pEnvelopeIpe, dblFactor);
                    if (holecpg.HoleCpgLt != null && holecpg.HoleCpgLt.Count > 0)
                    {
                        throw new ArgumentOutOfRangeException("We didn't consider this case!");
                    }
                }
            }

            str += "</path>\n";
            return str;
        }

        public static string DrawCpgBound(CPolygon cpg, IEnvelope pEnvelopeLayer, CEnvelope pEnvelopeIpe,
    CColor StrokeColor, string strWidth = "normal", string strDash = "normal")
        {
            double dblFactor = pEnvelopeIpe.Height / pEnvelopeLayer.Height;
            string str = GetStrokeStarting(StrokeColor, strWidth, strDash, "1", "1");
            str += AddCoordinates(cpg.CptLt, pEnvelopeLayer, pEnvelopeIpe, dblFactor);

            //draw holes and fill wholes with white
            if (cpg.HoleCpgLt != null && cpg.HoleCpgLt.Count > 0)
            {
                foreach (var holecpg in cpg.HoleCpgLt)
                {
                    str += AddCoordinates(holecpg.CptLt, pEnvelopeLayer, pEnvelopeIpe, dblFactor);
                    if (holecpg.HoleCpgLt != null && holecpg.HoleCpgLt.Count > 0)
                    {
                        throw new ArgumentOutOfRangeException("We didn't consider this case!");
                    }
                }
            }

            str += "</path>\n";
            return str;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="StrokeColor"></param>
        /// <param name="strWidth">normal, heavier, fat, ultrafat</param>
        /// <param name="strDash">normal, dashed, dotted, dash dotted, dash dot dotted</param>
        /// <param name="cap">normal: butt; 0: butt; 1: round; 2: square</param>
        /// <param name="join">normal: round; 0: miter; 1: round; 2: bevel</param>
        /// <param name="FillColor"></param>
        /// <returns></returns>
        public static string GetStrokeStarting(CColor StrokeColor, string strWidth = "normal", 
            string strDash="normal", string cap= "normal", string join = "normal", CColor FillColor = null)
        {
            string str = "<path stroke=\"" + AddColor(StrokeColor) + "\"";
            if (FillColor != null)
            {
                str += "fill=\"" + AddColor(FillColor) + "\"";
            }
            str += AddOtherAttribute(strWidth, strDash, cap, join) + ">\n";
            return str;
        }
        

        private static string AddPolygonCoordinates(List<CPoint> cptlt, IEnvelope pEnvelopeLayer,
            CEnvelope pEnvelopeIpe, double dblFactor)
        {
            return AddCoordinates(cptlt, pEnvelopeLayer, pEnvelopeIpe, dblFactor, 1) + "h\n";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cptlt"></param>
        /// <param name="pEnvelopeLayer"></param>
        /// <param name="pEnvelopeIpe"></param>
        /// <param name="dblFactor"></param>
        /// <param name="intSubtract">In ipe, the first vertex and the last vertex are not identical.
        /// Therefore, 0 if shape is polyline, 1 if shape is polygon</param>
        /// <returns></returns>
        /// <remarks>
        ///*********************polyline*****************************
        ///<path stroke="1 0.643 0.161" pen="fat">
        ///96 192 m
        ///160 144 l
        ///208 224 l
        ///</path>
        ///*********************multi polylines*****************************
        ///<path stroke="0 1 0.161" pen="fat">
        ///96 96 m
        ///112 32 l
        ///176 48 l
        ///208 112 l
        ///256 80 m
        ///256 32 l
        ///304 32 l
        ///336 80 l
        ///</path>
        ///*********************polygon******************************************
        ///<path stroke="1 0.643 0.208" fill="0.212 0.388 0.047" pen="fat">
        ///400 224 m
        ///352 128 l
        ///448 80 l
        ///544 192 l
        ///h
        ///</path>
        ///*********************polygon with two holes*********************
        ///<path stroke="1 0.2 0.502" fill="0.094 1 0" pen="fat">
        ///400 224 m
        ///352 128 l
        ///448 80 l
        ///544 192 l
        ///h
        ///416 176 m
        ///384 160 l
        ///416 128 l
        ///h
        ///448 192 m
        ///448 144 l
        ///496 176 l
        ///h
        ///</path>
        /// </remarks> 
        public static string AddCoordinates(List<CPoint> cptlt, IEnvelope pEnvelopeLayer,
            CEnvelope pEnvelopeIpe, double dblFactor, int intSubtract = 0)
        {
            int intRealCount = cptlt.Count - intSubtract;

            double[] dblx = new double[intRealCount];
            double[] dbly = new double[intRealCount];

            for (int i = 0; i < intRealCount; i++)
            {
                dblx[i] = CGeoFunc.CoordinateTransform(
                    cptlt[i].X, pEnvelopeLayer.XMin, pEnvelopeIpe.XMin, dblFactor);
                dbly[i] = CGeoFunc.CoordinateTransform(
                    cptlt[i].Y, pEnvelopeLayer.YMin, pEnvelopeIpe.YMin, dblFactor);
            }

            string str = AddXY(dblx[0], dbly[0]) + " m\n";
            for (int j = 1; j < dblx.Length; j++)
            {
                str += AddXY(dblx[j], dbly[j]) + " l\n";
            }

            return str;
        }

        public static string AddXY(double dblX, double dblY)
        {
            return dblX + " " + dblY;
        }

        public static string AddColor(CColor ccolor)
        {
            double dblIpeRed = Convert.ToDouble(ccolor.intRed) / 255;
            double dblIpeGreen = Convert.ToDouble(ccolor.intGreen) / 255;
            double dblIpeBlue = Convert.ToDouble(ccolor.intBlue) / 255;

            return (dblIpeRed + " " + dblIpeGreen + " " + dblIpeBlue);
        }


        private static string AddOtherAttribute(string pen = "normal", string dash = "normal", string cap = "normal", string join = "normal")
        {
            string strOtherAttribute = "";
            if (dash != "normal")
            {
                strOtherAttribute += " dash=\"" + dash + "\"";
            }
            if (pen != "normal")
            {
                strOtherAttribute += " pen=\"" + pen + "\"";
            }
            if (cap != "normal")
            {
                strOtherAttribute += " cap=\"" + cap + "\"";
            }
            if (join != "normal")
            {
                strOtherAttribute += " join=\"" + join + "\"";
            }
            return strOtherAttribute;
        }









        /**
         * Places a text label at a specific point.
         * 
         * @param text
         *            The text
         * @param x
         *            x-coordinate of the box
         * @param y
         *            y-coordinate of the box
         * @param color
         *            text-color
         * @param size
         *            text-size
         * @return
         */
        public static string writeIpeText(string text, int x, int y, 
            string color = "black", string size = "normal")
        {
            return "<text transformations=\"translations\" pos=\""
                    + x
                    + " "
                    + y
                    + "\" stroke=\""
                    + color
                    + "\" type=\"label\" width=\"190\" height=\"10\" depth=\"0\" valign=\"baseline\" size=\""
                    + size + "\">" + text + "</text>\n";
        }

        public static string SpecifyLayerByWritingText(string strLayerName, string text, int x, int y, 
            string color = "black", string size = "normal")
        {
            return "<text layer=\"" + strLayerName + "\" transformations=\"translations\" pos=\""
                    + x
                    + " "
                    + y
                    + "\" stroke=\""
                    + color
                    + "\" type=\"label\" width=\"190\" height=\"10\" depth=\"0\" valign=\"baseline\" size=\""
                    + size + "\">" + text + "</text>\n";
        }




        /**
         * Draws a circle.
         * 
         * @param x
         *            x-coordinate of the center
         * @param y
         *            y-coordinate of the center
         * @param radius
         *            radius
         * @param color
         *            color
         * @param pen
         *            pen width: normal, heavier, fat, ultrafat
         * @param dash
         *            dash style: normal, dashed, dotted, dash dotted, dash dot
         *            dotted
         * @return
         */
        public static string drawIpeCircle(int x, int y, double radius, string color = "black",
                string pen = "normal", string dash = "normal")
        {
            //string sf = new DecimalFormat("####.000").format(radius);
            //return "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
            //        + dash + "\">\n " + sf + " 0 0 " + sf + " " + x + " " + y
            //        + " e\n</path>\n";
            return null;
        }


        /**
         * Draws a circular arc in a mathematical positive sense.
         * 
         * @param xCenter
         *            x-coordinate of the center
         * @param yCenter
         *            y-coordinate of the center
         * @param xStart
         *            x-coordinate of the starting point on the circle
         * @param yStart
         *            y-coordinate of the starting point on the circle
         * @param xEnd
         *            x-coordinate of the end point on the circle
         * @param yEnd
         *            y-coordinate of the end point on the circle
         * @param color
         *            color
         * @param pen
         *            pen width: normal, heavier, fat, ultrafat
         * @param dash
         *            dash style: normal, dashed, dotted, dash dotted, dash dot
         *            dotted
         * @return
         */
        public static string drawIpeCircularArc(int xCenter, int yCenter,
                int xStart, int yStart, int xEnd, int yEnd, string color = "black",
                string pen = "normal", string dash = "normal")
        {
            //double radius = Math.Sqrt(Math.Pow(xStart - xCenter, 2)
            //        + Math.Pow(yStart - yCenter, 2));
            //string sf = new DecimalFormat("####.000").format(radius);
            //return "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
            //        + dash + "\">\n " + xStart + " " + yStart + " m\n " + sf
            //        + " 0 0 " + sf + " " + xCenter + " " + yCenter + " " + xEnd
            //        + " " + yEnd + " a\n</path>\n";
            return null;
        }

        /**
         * Draws a spline.
         * 
         * @param x
         *            x-coordinates of the control points.
         * @param y
         *            y-coordinates of the control points.
         * @param color
         *            color
         * @param pen
         *            pen width: normal, heavier, fat, ultrafat
         * @param dash
         *            dash style: normal, dashed, dotted, dash dotted, dash dot
         *            dotted
         * @return
         */
        public static string drawIpeSpline(int[] x, int[] y, 
            string color = "black", string pen = "normal", string dash = "normal")
        {
            string s = "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
                    + dash + "\">\n " + x[0] + " " + y[0] + " m";
            for (int i = 1; i < x.Length; i++)
            {
                s += "\n " + x[i] + " " + y[i];
            }
            s += " s\n</path>\n";
            return s;
        }


        /**
         * Draws a splinegon.
         * 
         * @param x
         *            x-coordinates of the control points.
         * @param y
         *            y-coordinates of the control points.
         * @param color
         *            color
         * @param pen
         *            pen width: normal, heavier, fat, ultrafat
         * @param dash
         *            dash style: normal, dashed, dotted, dash dotted, dash dot
         *            dotted
         * @return
         */
        public static string drawIpeSplinegon(int[] x, int[] y, 
            string color = "black", string pen = "normal", string dash = "normal")
        {
            string s = "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
                    + dash + "\">\n " + x[0] + " " + y[0];
            for (int i = 1; i < x.Length; i++)
            {
                s += "\n " + x[i] + " " + y[i];
            }
            s += " u\n</path>\n";
            return s;
        }

        ///**
        // * Creates a new page.
        // * 
        // * @return
        // */
        //public static string newPage()
        //{
        //    return "</page>\n<page>\n<layer name=\"alpha\"/>\n<view layers=\"alpha\" active=\"alpha\"/>\n";
        //}

        public static string GenerateIpeContentByData(string strData = null)
        {
            return getIpePreamble() + GeneratePageByData(strData) + getIpeEnd();
        }

        public static string GeneratePageByData(string strData = null)
        {
            return "<page>\n" + AddDefaultLayerAndView() + strData + "</page>\n";
        }

        public static string GenerateIpeContentByDataWithLayerInfo(string strDataWithLayerInfo = null)
        {
            return getIpePreamble() + GeneratePageByDataWithLayerInfo(strDataWithLayerInfo) + getIpeEnd();
        }

        public static string GeneratePageByDataWithLayerInfo(string strDataWithLayerInfo = null)
        {
            return "<page>\n" + strDataWithLayerInfo + "</page>\n";
        }

        //public static string AddPage(string strContent = null)
        //{
        //    if (strContent != null)
        //    {
        //        return "<page>\n" + strContent + "</page>\n";
        //    }
        //    else
        //    {
        //        return "<page>\n" + AddDefaultLayerAndView() + "</page>\n";
        //    }
        //}

        public static string AddLayer(string strLayerName = "alpha")
        {
            return "<layer name=\"" + strLayerName + "\"/>\n";
        }

        public static string AddDefaultLayerAndView()
        {
            return AddLayer() + AddView();
        }

        public static string AddViews(IEnumerable<string> strDisplayLayerEb, string strActiveLayer)
        {
            var strDisplayLayerEt = strDisplayLayerEb.GetEnumerator();
            strDisplayLayerEt.MoveNext();

            string strDisplayLayers = strDisplayLayerEt.Current;
            while (strDisplayLayerEt.MoveNext())
            {
                strDisplayLayers += (" " + strDisplayLayerEt.Current);
            }

            return AddView(strDisplayLayers, strActiveLayer);
        }

        public static string AddView(string strDisplayLayers = "alpha", string strActiveLayer = "alpha")
        {
            return "<view layers=\"" + strDisplayLayers + "\" active=\"" + strActiveLayer + "\"/>\n";
        }


        /**
         * Closes the file.
         * 
         * @return
         */
        public static string getIpeEnd()
        {
            return "</ipe>\n";
        }

        /**
         * The mandatory preamble for an ipe-file.
         * 
         * @return
         */
        public static string getIpePreamble()
        {
            var strTime = CHelpFunc.GetTimeStampForIpe();
            #region strIpePreamble
            string strIpePreamble =
@"<?xml version=""1.0""?>
<!DOCTYPE ipe SYSTEM ""ipe.dtd"">
<ipe version=""70206"" creator=""Ipe 7.2.7"">
<info created=""D:" + strTime + @""" modified=""D:" + strTime + @"""/>
<preamble>\usepackage[english]{babel}</preamble>
<ipestyle name=""basic"">
<symbol name=""arrow/arc(spx)"">
<path stroke=""sym-stroke"" fill=""sym-stroke"" pen=""sym-pen"">
0 0 m
-1 0.333 l
-1 -0.333 l
h
</path>
</symbol>
<symbol name=""arrow/farc(spx)"">
<path stroke=""sym-stroke"" fill=""white"" pen=""sym-pen"">
0 0 m
-1 0.333 l
-1 -0.333 l
h
</path>
</symbol>
<symbol name=""arrow/ptarc(spx)"">
<path stroke=""sym-stroke"" fill=""sym-stroke"" pen=""sym-pen"">
0 0 m
-1 0.333 l
-0.8 0 l
-1 -0.333 l
h
</path>
</symbol>
<symbol name=""arrow/fptarc(spx)"">
<path stroke=""sym-stroke"" fill=""white"" pen=""sym-pen"">
0 0 m
-1 0.333 l
-0.8 0 l
-1 -0.333 l
h
</path>
</symbol>
<symbol name=""mark/circle(sx)"" transformations=""translations"">
<path fill=""sym-stroke"">
0.6 0 0 0.6 0 0 e
0.4 0 0 0.4 0 0 e
</path>
</symbol>
<symbol name=""mark/disk(sx)"" transformations=""translations"">
<path fill=""sym-stroke"">
0.6 0 0 0.6 0 0 e
</path>
</symbol>
<symbol name=""mark/fdisk(sfx)"" transformations=""translations"">
<group>
<path fill=""sym-fill"">
0.5 0 0 0.5 0 0 e
</path>
<path fill=""sym-stroke"" fillrule=""eofill"">
0.6 0 0 0.6 0 0 e
0.4 0 0 0.4 0 0 e
</path>
</group>
</symbol>
<symbol name=""mark/box(sx)"" transformations=""translations"">
<path fill=""sym-stroke"" fillrule=""eofill"">
-0.6 -0.6 m
0.6 -0.6 l
0.6 0.6 l
-0.6 0.6 l
h
-0.4 -0.4 m
0.4 -0.4 l
0.4 0.4 l
-0.4 0.4 l
h
</path>
</symbol>
<symbol name=""mark/square(sx)"" transformations=""translations"">
<path fill=""sym-stroke"">
-0.6 -0.6 m
0.6 -0.6 l
0.6 0.6 l
-0.6 0.6 l
h
</path>
</symbol>
<symbol name=""mark/fsquare(sfx)"" transformations=""translations"">
<group>
<path fill=""sym-fill"">
-0.5 -0.5 m
0.5 -0.5 l
0.5 0.5 l
-0.5 0.5 l
h
</path>
<path fill=""sym-stroke"" fillrule=""eofill"">
-0.6 -0.6 m
0.6 -0.6 l
0.6 0.6 l
-0.6 0.6 l
h
-0.4 -0.4 m
0.4 -0.4 l
0.4 0.4 l
-0.4 0.4 l
h
</path>
</group>
</symbol>
<symbol name=""mark/cross(sx)"" transformations=""translations"">
<group>
<path fill=""sym-stroke"">
-0.43 -0.57 m
0.57 0.43 l
0.43 0.57 l
-0.57 -0.43 l
h
</path>
<path fill=""sym-stroke"">
-0.43 0.57 m
0.57 -0.43 l
0.43 -0.57 l
-0.57 0.43 l
h
</path>
</group>
</symbol>
<symbol name=""arrow/fnormal(spx)"">
<path stroke=""sym-stroke"" fill=""white"" pen=""sym-pen"">
0 0 m
-1 0.333 l
-1 -0.333 l
h
</path>
</symbol>
<symbol name=""arrow/pointed(spx)"">
<path stroke=""sym-stroke"" fill=""sym-stroke"" pen=""sym-pen"">
0 0 m
-1 0.333 l
-0.8 0 l
-1 -0.333 l
h
</path>
</symbol>
<symbol name=""arrow/fpointed(spx)"">
<path stroke=""sym-stroke"" fill=""white"" pen=""sym-pen"">
0 0 m
-1 0.333 l
-0.8 0 l
-1 -0.333 l
h
</path>
</symbol>
<symbol name=""arrow/linear(spx)"">
<path stroke=""sym-stroke"" pen=""sym-pen"">
-1 0.333 m
0 0 l
-1 -0.333 l
</path>
</symbol>
<symbol name=""arrow/fdouble(spx)"">
<path stroke=""sym-stroke"" fill=""white"" pen=""sym-pen"">
0 0 m
-1 0.333 l
-1 -0.333 l
h
-1 0 m
-2 0.333 l
-2 -0.333 l
h
</path>
</symbol>
<symbol name=""arrow/double(spx)"">
<path stroke=""sym-stroke"" fill=""sym-stroke"" pen=""sym-pen"">
0 0 m
-1 0.333 l
-1 -0.333 l
h
-1 0 m
-2 0.333 l
-2 -0.333 l
h
</path>
</symbol>
<pen name=""heavier"" value=""0.8""/>
<pen name=""fat"" value=""1.2""/>
<pen name=""ultrafat"" value=""2""/>
<symbolsize name=""large"" value=""5""/>
<symbolsize name=""small"" value=""2""/>
<symbolsize name=""tiny"" value=""1.1""/>
<arrowsize name=""large"" value=""10""/>
<arrowsize name=""small"" value=""5""/>
<arrowsize name=""tiny"" value=""3""/>
<color name=""red"" value=""1 0 0""/>
<color name=""green"" value=""0 1 0""/>
<color name=""blue"" value=""0 0 1""/>
<color name=""yellow"" value=""1 1 0""/>
<color name=""orange"" value=""1 0.647 0""/>
<color name=""gold"" value=""1 0.843 0""/>
<color name=""purple"" value=""0.627 0.125 0.941""/>
<color name=""gray"" value=""0.745""/>
<color name=""brown"" value=""0.647 0.165 0.165""/>
<color name=""navy"" value=""0 0 0.502""/>
<color name=""pink"" value=""1 0.753 0.796""/>
<color name=""seagreen"" value=""0.18 0.545 0.341""/>
<color name=""turquoise"" value=""0.251 0.878 0.816""/>
<color name=""violet"" value=""0.933 0.51 0.933""/>
<color name=""darkblue"" value=""0 0 0.545""/>
<color name=""darkcyan"" value=""0 0.545 0.545""/>
<color name=""darkgray"" value=""0.663""/>
<color name=""darkgreen"" value=""0 0.392 0""/>
<color name=""darkmagenta"" value=""0.545 0 0.545""/>
<color name=""darkorange"" value=""1 0.549 0""/>
<color name=""darkred"" value=""0.545 0 0""/>
<color name=""lightblue"" value=""0.678 0.847 0.902""/>
<color name=""lightcyan"" value=""0.878 1 1""/>
<color name=""lightgray"" value=""0.827""/>
<color name=""lightgreen"" value=""0.565 0.933 0.565""/>
<color name=""lightyellow"" value=""1 1 0.878""/>
<dashstyle name=""dashed"" value=""[4] 0""/>
<dashstyle name=""dotted"" value=""[1 3] 0""/>
<dashstyle name=""dash dotted"" value=""[4 2 1 2] 0""/>
<dashstyle name=""dash dot dotted"" value=""[4 2 1 2 1 2] 0""/>
<textsize name=""large"" value=""\large""/>
<textsize name=""Large"" value=""\Large""/>
<textsize name=""LARGE"" value=""\LARGE""/>
<textsize name=""huge"" value=""\huge""/>
<textsize name=""Huge"" value=""\Huge""/>
<textsize name=""small"" value=""\small""/>
<textsize name=""footnote"" value=""\footnotesize""/>
<textsize name=""tiny"" value=""\tiny""/>
<textstyle name=""center"" begin=""\begin{center}"" end=""\end{center}""/>
<textstyle name=""itemize"" begin=""\begin{itemize}"" end=""\end{itemize}""/>
<textstyle name=""item"" begin=""\begin{itemize}\item{}"" end=""\end{itemize}""/>
<gridsize name=""4 pts"" value=""4""/>
<gridsize name=""8 pts (~3 mm)"" value=""8""/>
<gridsize name=""16 pts (~6 mm)"" value=""16""/>
<gridsize name=""32 pts (~12 mm)"" value=""32""/>
<gridsize name=""10 pts (~3.5 mm)"" value=""10""/>
<gridsize name=""20 pts (~7 mm)"" value=""20""/>
<gridsize name=""14 pts (~5 mm)"" value=""14""/>
<gridsize name=""28 pts (~10 mm)"" value=""28""/>
<gridsize name=""56 pts (~20 mm)"" value=""56""/>
<anglesize name=""90 deg"" value=""90""/>
<anglesize name=""60 deg"" value=""60""/>
<anglesize name=""45 deg"" value=""45""/>
<anglesize name=""30 deg"" value=""30""/>
<anglesize name=""22.5 deg"" value=""22.5""/>
<opacity name=""10%"" value=""0.1""/>
<opacity name=""30%"" value=""0.3""/>
<opacity name=""50%"" value=""0.5""/>
<opacity name=""75%"" value=""0.75""/>
<tiling name=""falling"" angle=""-60"" step=""4"" width=""1""/>
<tiling name=""rising"" angle=""30"" step=""4"" width=""1""/>
</ipestyle>";
            #endregion
            
            return strIpePreamble;
        }

        /// <summary>
        /// Prepare each layer for Ipe
        /// </summary>
        /// <param name="strLayerNameLt"></param>
        /// <returns></returns>
        public static string GetDataOfLayerNames(List<string> strLayerNameLt)
        {
            string strData = "";
            foreach (var strName in strLayerNameLt)
            {
                strData += CIpeDraw.AddLayer(strName);
            }
            return strData;
        }

        /// <summary>
        /// Incrementally dislay the layers
        /// </summary>
        /// <param name="strLayerNameLt"></param>
        /// <returns></returns>
        public static string GetDataOfViewsAreaAgg(List<string> strLayerNameLt)
        {
            var strIpeCont = GetIpeContentViewAll(strLayerNameLt);

            //display the data
            string strDisplayLayers = strLayerNameLt[0];
            strIpeCont += CIpeDraw.AddView(strDisplayLayers, strLayerNameLt[0]);

            //add other views
            strIpeCont += CIpeDraw.AddView(strDisplayLayers + " "+ strLayerNameLt[1], strLayerNameLt[1]);
            
            for (int i = 2; i < strLayerNameLt.Count; i++)
            {
                strDisplayLayers += " " + strLayerNameLt[i];
                strIpeCont += CIpeDraw.AddView(strDisplayLayers + " " + strLayerNameLt[++i], strLayerNameLt[i]);                
            }
            return strIpeCont;
        }

        /// <summary>
        /// Incrementally dislay the layers
        /// </summary>
        /// <param name="strLayerNameLt"></param>
        /// <returns></returns>
        public static string GetDataOfViews(List<string> strLayerNameLt, bool blnViewPre = true)
        {
            var strIpeCont = GetIpeContentViewAll(strLayerNameLt);

            //add other views
            string strView = strLayerNameLt[0];
            strIpeCont += CIpeDraw.AddView(strView, strLayerNameLt[0]);
            for (int i = 1; i < strLayerNameLt.Count; i++)
            {
                if (blnViewPre == true)
                {
                    strView += " " + strLayerNameLt[i];
                }
                else
                {
                    strView = strLayerNameLt[i];
                }

                strIpeCont += CIpeDraw.AddView(strView, strLayerNameLt[i]);
            }
            return strIpeCont;
        }

        /// <summary>
        /// add a view that we can see all the data so that we can move or scale our data
        /// </summary>
        /// <param name="strLayerNameLt"></param>
        /// <returns></returns>
        private static string GetIpeContentViewAll(List<string> strLayerNameLt)
        {
            var strViewAll = strLayerNameLt[0];
            for (int i = 1; i < strLayerNameLt.Count; i++)
            {
                strViewAll += " " + strLayerNameLt[i];
            }
            return CIpeDraw.AddView(strViewAll, strLayerNameLt[0]);
        }

    }
}
