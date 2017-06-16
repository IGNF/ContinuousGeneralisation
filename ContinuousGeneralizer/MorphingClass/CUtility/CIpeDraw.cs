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
 * Usage: The functions create an UML-String that corresponds to the specified
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
        public static String drawIpeMark(double x, double y, String shape= "disk", String color = "black", String size="normal")
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
        public static String drawIpeBox(int x1, int y1, int x2, int y2,
                String outlinecolor= "black", String fillcolor = "white", String pen = "normal", String dash = "normal")
        {
            return "<path stroke=\"" + outlinecolor + "\" fill=\"" + fillcolor + "\" pen=\"" + pen + "\" dash=\""
                    + dash + "\">\n " + x1 + " " + y2 + " m\n " + x1 + " " + y1
                    + " l\n " + x2 + " " + y1 + " l\n " + x2 + " " + y2
                    + " l\n h\n" + "</path>\n";
        }
        

        /**
         * Draws a path between points.
         * 
         * @param x
         *            x-coordinates of the points
         * @param y
         *            y-coordinates of the points
         * @param color
         *            color
         * @param pen
         *            pen width: normal, heavier, fat, ultrafat
         * @param dash
         *            dash style: normal, dashed, dotted, dash dotted, dash dot
         *            dotted
         * @return
         */
        public static String drawIpePath(double[] x, double[] y, String color= "black",
                String pen= "normal", String dash= "normal")
        {
            String s = "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
                    + dash + "\">\n" + AddXY(x[0], y[0]) + " m\n";
            for (int i = 1; i < x.Length; i++)
            {
                s += AddXY(x[i], y[i]) + " l\n";
            }
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
        public static String drawIpeEdge(double x1, double y1, double x2, double y2, String color = "black",
                String pen = "normal", String dash = "normal")
        {
            return drawIpePath(new double[] { x1, x2 }, new double[] { y1, y2 }, color,
                    pen, dash);
        }


        public static string DrawIpt(IPoint ipt, IEnvelope pEnvelopeLayer, CEnvelope pEnvelopeIpe, string strShape,
    CColor StrokeColor, string strWidth = "normal")
        {
            double dblFactor = pEnvelopeIpe.Height / pEnvelopeLayer.Height;
            double dblx= CGeoFunc.CoordinateTransform(
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

            string str = GetStrokeStarting(StrokeColor, strWidth, null);
            str += AddCoordinates(cptlt, pEnvelopeLayer, pEnvelopeIpe, dblFactor);
            str += "</path>\n";

            return str;
        }

        public static string DrawCpg(CPolygon cpg, IEnvelope pEnvelopeLayer, CEnvelope pEnvelopeIpe,
            CColor StrokeColor, CColor FillColor, string strWidth = "normal")
        {
            double dblFactor = pEnvelopeIpe.Height / pEnvelopeLayer.Height;
            string str = GetStrokeStarting(StrokeColor, strWidth, FillColor);
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

        public static string GetStrokeStarting(CColor StrokeColor, string strWidth = "normal", CColor FillColor = null)
        {
            string str = "<path stroke=\"" + AddColor(StrokeColor) + "\" ";
            if (FillColor != null)
            {
                str += "fill=\"" + AddColor(FillColor) + "\" ";
            }
            str += "pen=\"" + strWidth + "\">\n";
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
            CEnvelope pEnvelopeIpe, double dblFactor, int intSubtract=0)
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
        public static String writeIpeText(String text, int x, int y, String color= "black",
                String size="normal")
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

        public static String SpecifyLayerByWritingText(string strLayerName, String text, int x, int y, String color = "black",
        String size= "normal")
        {
            return "<text layer=\"" + strLayerName+ "\" transformations=\"translations\" pos=\""
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
        public static String drawIpeCircle(int x, int y, double radius,
                String color, String pen, String dash)
        {
            //String sf = new DecimalFormat("####.000").format(radius);
            //return "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
            //        + dash + "\">\n " + sf + " 0 0 " + sf + " " + x + " " + y
            //        + " e\n</path>\n";
            return null;
        }

        /**
         * Draws an undashed circle.
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
         * @return
         */
        public static String drawIpeCircle(int x, int y, double radius,
                String color, String pen)
        {
            return drawIpeCircle(x, y, radius, color, pen, "normal");
        }

        /**
         * Draws an undashed circle with pen width "normal".
         * 
         * @param x
         *            x-coordinate of the center
         * @param y
         *            y-coordinate of the center
         * @param radius
         *            radius
         * @param color
         *            color
         * @return
         */
        public static String drawIpeCircle(int x, int y, double radius, String color)
        {
            return drawIpeCircle(x, y, radius, color, "normal", "normal");
        }

        /**
         * Draws an undashed circle with pen width "normal" and color "black".
         * 
         * @param x
         *            x-coordinate of the center
         * @param y
         *            y-coordinate of the center
         * @param radius
         *            radius
         * @return
         */
        public static String drawIpeCircle(int x, int y, double radius)
        {
            return drawIpeCircle(x, y, radius, "black", "normal", "normal");
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
        public static String drawIpeCircularArc(int xCenter, int yCenter,
                int xStart, int yStart, int xEnd, int yEnd, String color,
                String pen, String dash)
        {
            //double radius = Math.Sqrt(Math.Pow(xStart - xCenter, 2)
            //        + Math.Pow(yStart - yCenter, 2));
            //String sf = new DecimalFormat("####.000").format(radius);
            //return "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
            //        + dash + "\">\n " + xStart + " " + yStart + " m\n " + sf
            //        + " 0 0 " + sf + " " + xCenter + " " + yCenter + " " + xEnd
            //        + " " + yEnd + " a\n</path>\n";
            return null;
        }

        /**
         * Draws an undashed circular arc in a mathematical positive sense.
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
         * @return
         */
        public static String drawIpeCircularArc(int xCenter, int yCenter,
                int xStart, int yStart, int xEnd, int yEnd, String color, String pen)
        {
            return drawIpeCircularArc(xCenter, yCenter, xStart, yStart, xEnd, yEnd,
                    color, pen, "normal");
        }

        /**
         * Draws an undashed circular arc in a mathematical positive sense with pen
         * width "normal".
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
         * @return
         */
        public static String drawIpeCircularArc(int xCenter, int yCenter,
                int xStart, int yStart, int xEnd, int yEnd, String color)
        {
            return drawIpeCircularArc(xCenter, yCenter, xStart, yStart, xEnd, yEnd,
                    color, "normal", "normal");
        }

        /**
         * Draws an undashed circular arc in a mathematical positive sense with pen
         * width "normal" and color "black".
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
         * @return
         */
        public static String drawIpeCircularArc(int xCenter, int yCenter,
                int xStart, int yStart, int xEnd, int yEnd)
        {
            return drawIpeCircularArc(xCenter, yCenter, xStart, yStart, xEnd, yEnd,
                    "black", "normal", "normal");
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
        public static String drawIpeSpline(int[] x, int[] y, String color,
                String pen, String dash)
        {
            String s = "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
                    + dash + "\">\n " + x[0] + " " + y[0] + " m";
            for (int i = 1; i < x.Length; i++)
            {
                s += "\n " + x[i] + " " + y[i];
            }
            s += " s\n</path>\n";
            return s;
        }

        /**
         * Draws an undashed spline.
         * 
         * @param x
         *            x-coordinates of the control points.
         * @param y
         *            y-coordinates of the control points.
         * @param color
         *            color
         * @param pen
         *            pen width: normal, heavier, fat, ultrafat
         * @return
         */
        public static String drawIpeSpline(int[] x, int[] y, String color,
                String pen)
        {
            return CIpeDraw.drawIpeSpline(x, y, color, pen, "normal");
        }

        /**
         * Draws an undashed spline with pen width "normal".
         * 
         * @param x
         *            x-coordinates of the control points.
         * @param y
         *            y-coordinates of the control points.
         * @param color
         *            color
         * @return
         */
        public static String drawIpeSpline(int[] x, int[] y, String color)
        {
            return CIpeDraw.drawIpeSpline(x, y, color, "normal", "normal");
        }

        /**
         * Draws an undashed spline with pen width "normal" and color "black".
         * 
         * @param x
         *            x-coordinates of the control points.
         * @param y
         *            y-coordinates of the control points.
         * @return
         */
        public static String drawIpeSpline(int[] x, int[] y)
        {
            return CIpeDraw.drawIpeSpline(x, y, "black", "normal", "normal");
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
        public static String drawIpeSplinegon(int[] x, int[] y, String color,
                String pen, String dash)
        {
            String s = "<path stroke=\"" + color + "\" pen=\"" + pen + "\" dash=\""
                    + dash + "\">\n " + x[0] + " " + y[0];
            for (int i = 1; i < x.Length; i++)
            {
                s += "\n " + x[i] + " " + y[i];
            }
            s += " u\n</path>\n";
            return s;
        }

        /**
         * Draws an undashed splinegon.
         * 
         * @param x
         *            x-coordinates of the control points.
         * @param y
         *            y-coordinates of the control points.
         * @param color
         *            color
         * @param pen
         *            pen width: normal, heavier, fat, ultrafat
         * @return
         */
        public static String drawIpeSplinegon(int[] x, int[] y, String color,
                String pen)
        {
            return CIpeDraw.drawIpeSplinegon(x, y, color, pen, "normal");
        }

        /**
         * Draws an undashed splinegon with pen width "normal".
         * 
         * @param x
         *            x-coordinates of the control points.
         * @param y
         *            y-coordinates of the control points.
         * @param color
         *            color
         * @return
         */
        public static String drawIpeSplinegon(int[] x, int[] y, String color)
        {
            return CIpeDraw.drawIpeSplinegon(x, y, color, "normal", "normal");
        }

        /**
         * Draws an undashed splinegon with pen width "normal" and color "black".
         * 
         * @param x
         *            x-coordinates of the control points.
         * @param y
         *            y-coordinates of the control points.
         * @return
         */
        public static String drawIpeSplinegon(int[] x, int[] y)
        {
            return CIpeDraw.drawIpeSplinegon(x, y, "black", "normal", "normal");
        }

        ///**
        // * Creates a new page.
        // * 
        // * @return
        // */
        //public static String newPage()
        //{
        //    return "</page>\n<page>\n<layer name=\"alpha\"/>\n<view layers=\"alpha\" active=\"alpha\"/>\n";
        //}

        public static string GenerateIpeContentByData(string strData = null)
        {
            return getIpePreamble()+ getIpeConf()+ GeneratePageByData(strData)+ getIpeEnd();
        }

        public static string GeneratePageByData(string strData = null)
        {
            return "<page>\n" + AddDefaultLayerAndView() + strData + "</page>\n";
        }

        public static string GenerateIpeContentByDataWithLayerInfo(string strDataWithLayerInfo = null)
        {
            return getIpePreamble() + getIpeConf() + GeneratePageByDataWithLayerInfo(strDataWithLayerInfo) + getIpeEnd();
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

        public static String AddLayer(string strLayerName = "alpha")
        {
            return "<layer name=\""+ strLayerName + "\"/>\n";
        }

        public static string AddDefaultLayerAndView()
        {
            return AddLayer() + AddView();
        }

        public static String AddViews(IEnumerable<String> strDisplayLayerEb, string strActiveLayer)
        {
            var strDisplayLayerEt = strDisplayLayerEb.GetEnumerator();
            strDisplayLayerEt.MoveNext();

            string strDisplayLayers = strDisplayLayerEt.Current;
            while (strDisplayLayerEt.MoveNext())
            {
                strDisplayLayers+=(" "+ strDisplayLayerEt.Current);
            }

            return AddView(strDisplayLayers, strActiveLayer);
        }

        public static String AddView(String strDisplayLayer = "alpha", string strActiveLayer = "alpha")
        {
            return "<view layers=\"" + strDisplayLayer + "\" active=\"" + strActiveLayer + "\"/>\n";
        }


        /**
         * Closes the file.
         * 
         * @return
         */
        public static String getIpeEnd()
        {
            return "</ipe>\n";
            //return "</page>\n</ipe>\n";
        }

        /**
         * The mandatory preamble for an ipe-file.
         * 
         * @return
         */
        public static String getIpePreamble()
        {
            return "<?xml version=\"1.0\"?>\n    <!DOCTYPE ipe SYSTEM \"ipe.dtd\">\n    <ipe version=\"70107\" creator=\"Ipe 7.2.2\">\n    <info created=\"D:20160217195754\" modified=\"D:20160217210409\"/>\n    <preamble>\\usepackage[english]{babel}</preamble>\n";
        }

        /**
         * Configuration of the standard objects in ipe.
         * 
         * @return
         */
        public static String getIpeConf()
        {
            return "    <ipestyle name=\"basic\">\n    <symbol name=\"arrow/arc(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"sym-stroke\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/farc(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"white\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"mark/circle(sx)\" transformations=\"translations\">\n    <path fill=\"sym-stroke\">\n    0.6 0 0 0.6 0 0 e\n    0.4 0 0 0.4 0 0 e\n    </path>\n    </symbol>\n    <symbol name=\"mark/disk(sx)\" transformations=\"translations\">\n    <path fill=\"sym-stroke\">\n    0.6 0 0 0.6 0 0 e\n    </path>\n    </symbol>\n    <symbol name=\"mark/fdisk(sfx)\" transformations=\"translations\">\n    <group>\n    <path fill=\"sym-fill\">\n    0.5 0 0 0.5 0 0 e\n    </path>\n    <path fill=\"sym-stroke\" fillrule=\"eofill\">\n    0.6 0 0 0.6 0 0 e\n    0.4 0 0 0.4 0 0 e\n    </path>\n    </group>\n    </symbol>\n    <symbol name=\"mark/box(sx)\" transformations=\"translations\">\n    <path fill=\"sym-stroke\" fillrule=\"eofill\">\n    -0.6 -0.6 m\n    0.6 -0.6 l\n    0.6 0.6 l\n    -0.6 0.6 l\n    h\n    -0.4 -0.4 m\n    0.4 -0.4 l\n    0.4 0.4 l\n    -0.4 0.4 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"mark/square(sx)\" transformations=\"translations\">\n    <path fill=\"sym-stroke\">\n    -0.6 -0.6 m\n    0.6 -0.6 l\n    0.6 0.6 l\n    -0.6 0.6 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"mark/fsquare(sfx)\" transformations=\"translations\">\n    <group>\n    <path fill=\"sym-fill\">\n    -0.5 -0.5 m\n    0.5 -0.5 l\n    0.5 0.5 l\n    -0.5 0.5 l\n    h\n    </path>\n    <path fill=\"sym-stroke\" fillrule=\"eofill\">\n    -0.6 -0.6 m\n    0.6 -0.6 l\n    0.6 0.6 l\n    -0.6 0.6 l\n    h\n    -0.4 -0.4 m\n    0.4 -0.4 l\n    0.4 0.4 l\n    -0.4 0.4 l\n    h\n    </path>\n    </group>\n    </symbol>\n    <symbol name=\"mark/cross(sx)\" transformations=\"translations\">\n    <group>\n    <path fill=\"sym-stroke\">\n    -0.43 -0.57 m\n    0.57 0.43 l\n    0.43 0.57 l\n    -0.57 -0.43 l\n    h\n    </path>\n    <path fill=\"sym-stroke\">\n    -0.43 0.57 m\n    0.57 -0.43 l\n    0.43 -0.57 l\n    -0.57 0.43 l\n    h\n    </path>\n    </group>\n    </symbol>\n    <symbol name=\"arrow/fnormal(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"white\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/pointed(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"sym-stroke\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -0.8 0 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/fpointed(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"white\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -0.8 0 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/linear(spx)\">\n    <path stroke=\"sym-stroke\" pen=\"sym-pen\">\n    -1 0.333 m\n    0 0 l\n    -1 -0.333 l\n    </path>\n    </symbol>\n    <symbol name=\"arrow/fdouble(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"white\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    -1 0 m\n    -2 0.333 l\n    -2 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/double(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"sym-stroke\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    -1 0 m\n    -2 0.333 l\n    -2 -0.333 l\n    h\n    </path>\n    </symbol>\n    <pen name=\"heavier\" value=\"0.8\"/>\n    <pen name=\"fat\" value=\"1.2\"/>\n    <pen name=\"ultrafat\" value=\"2\"/>\n    <symbolsize name=\"large\" value=\"5\"/>\n    <symbolsize name=\"small\" value=\"2\"/>\n    <symbolsize name=\"tiny\" value=\"1.1\"/>\n    <arrowsize name=\"large\" value=\"10\"/>\n    <arrowsize name=\"small\" value=\"5\"/>\n    <arrowsize name=\"tiny\" value=\"3\"/>\n    <color name=\"red\" value=\"1 0 0\"/>\n    <color name=\"green\" value=\"0 1 0\"/>\n    <color name=\"blue\" value=\"0 0 1\"/>\n    <color name=\"yellow\" value=\"1 1 0\"/>\n    <color name=\"orange\" value=\"1 0.647 0\"/>\n    <color name=\"gold\" value=\"1 0.843 0\"/>\n    <color name=\"purple\" value=\"0.627 0.125 0.941\"/>\n    <color name=\"gray\" value=\"0.745\"/>\n    <color name=\"brown\" value=\"0.647 0.165 0.165\"/>\n    <color name=\"navy\" value=\"0 0 0.502\"/>\n    <color name=\"pink\" value=\"1 0.753 0.796\"/>\n    <color name=\"seagreen\" value=\"0.18 0.545 0.341\"/>\n    <color name=\"turquoise\" value=\"0.251 0.878 0.816\"/>\n    <color name=\"violet\" value=\"0.933 0.51 0.933\"/>\n    <color name=\"darkblue\" value=\"0 0 0.545\"/>\n    <color name=\"darkcyan\" value=\"0 0.545 0.545\"/>\n    <color name=\"darkgray\" value=\"0.663\"/>\n    <color name=\"darkgreen\" value=\"0 0.392 0\"/>\n    <color name=\"darkmagenta\" value=\"0.545 0 0.545\"/>\n    <color name=\"darkorange\" value=\"1 0.549 0\"/>\n    <color name=\"darkred\" value=\"0.545 0 0\"/>\n    <color name=\"lightblue\" value=\"0.678 0.847 0.902\"/>\n    <color name=\"lightcyan\" value=\"0.878 1 1\"/>\n    <color name=\"lightgray\" value=\"0.827\"/>\n    <color name=\"lightgreen\" value=\"0.565 0.933 0.565\"/>\n    <color name=\"lightyellow\" value=\"1 1 0.878\"/>\n    <dashstyle name=\"dashed\" value=\"[4] 0\"/>\n    <dashstyle name=\"dotted\" value=\"[1 3] 0\"/>\n    <dashstyle name=\"dash dotted\" value=\"[4 2 1 2] 0\"/>\n    <dashstyle name=\"dash dot dotted\" value=\"[4 2 1 2 1 2] 0\"/>\n    <textsize name=\"large\" value=\"\\large\"/>\n    <textsize name=\"Large\" value=\"\\Large\"/>\n    <textsize name=\"LARGE\" value=\"\\LARGE\"/>\n    <textsize name=\"huge\" value=\"\\huge\"/>\n    <textsize name=\"Huge\" value=\"\\Huge\"/>\n    <textsize name=\"small\" value=\"\\small\"/>\n    <textsize name=\"footnote\" value=\"\\footnotesize\"/>\n    <textsize name=\"tiny\" value=\"\\tiny\"/>\n    <textstyle name=\"center\" begin=\"\\begin{center}\" end=\"\\end{center}\"/>\n    <textstyle name=\"itemize\" begin=\"\\begin{itemize}\" end=\"\\end{itemize}\"/>\n    <textstyle name=\"item\" begin=\"\\begin{itemize}\\item{}\" end=\"\\end{itemize}\"/>\n    <gridsize name=\"4 pts\" value=\"4\"/>\n    <gridsize name=\"8 pts (~3 mm)\" value=\"8\"/>\n    <gridsize name=\"16 pts (~6 mm)\" value=\"16\"/>\n    <gridsize name=\"32 pts (~12 mm)\" value=\"32\"/>\n    <gridsize name=\"10 pts (~3.5 mm)\" value=\"10\"/>\n    <gridsize name=\"20 pts (~7 mm)\" value=\"20\"/>\n    <gridsize name=\"14 pts (~5 mm)\" value=\"14\"/>\n    <gridsize name=\"28 pts (~10 mm)\" value=\"28\"/>\n    <gridsize name=\"56 pts (~20 mm)\" value=\"56\"/>\n    <anglesize name=\"90 deg\" value=\"90\"/>\n    <anglesize name=\"60 deg\" value=\"60\"/>\n    <anglesize name=\"45 deg\" value=\"45\"/>\n    <anglesize name=\"30 deg\" value=\"30\"/>\n    <anglesize name=\"22.5 deg\" value=\"22.5\"/>\n    <tiling name=\"falling\" angle=\"-60\" step=\"4\" width=\"1\"/>\n    <tiling name=\"rising\" angle=\"30\" step=\"4\" width=\"1\"/>\n    <layout paper=\"595 842\" origin=\"0 0\" frame=\"595 842\" skip=\"32\" crop=\"yes\"/>\n    </ipestyle>\n";

            //return "    <ipestyle name=\"basic\">\n    <symbol name=\"arrow/arc(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"sym-stroke\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/farc(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"white\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"mark/circle(sx)\" transformations=\"translations\">\n    <path fill=\"sym-stroke\">\n    0.6 0 0 0.6 0 0 e\n    0.4 0 0 0.4 0 0 e\n    </path>\n    </symbol>\n    <symbol name=\"mark/disk(sx)\" transformations=\"translations\">\n    <path fill=\"sym-stroke\">\n    0.6 0 0 0.6 0 0 e\n    </path>\n    </symbol>\n    <symbol name=\"mark/fdisk(sfx)\" transformations=\"translations\">\n    <group>\n    <path fill=\"sym-fill\">\n    0.5 0 0 0.5 0 0 e\n    </path>\n    <path fill=\"sym-stroke\" fillrule=\"eofill\">\n    0.6 0 0 0.6 0 0 e\n    0.4 0 0 0.4 0 0 e\n    </path>\n    </group>\n    </symbol>\n    <symbol name=\"mark/box(sx)\" transformations=\"translations\">\n    <path fill=\"sym-stroke\" fillrule=\"eofill\">\n    -0.6 -0.6 m\n    0.6 -0.6 l\n    0.6 0.6 l\n    -0.6 0.6 l\n    h\n    -0.4 -0.4 m\n    0.4 -0.4 l\n    0.4 0.4 l\n    -0.4 0.4 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"mark/square(sx)\" transformations=\"translations\">\n    <path fill=\"sym-stroke\">\n    -0.6 -0.6 m\n    0.6 -0.6 l\n    0.6 0.6 l\n    -0.6 0.6 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"mark/fsquare(sfx)\" transformations=\"translations\">\n    <group>\n    <path fill=\"sym-fill\">\n    -0.5 -0.5 m\n    0.5 -0.5 l\n    0.5 0.5 l\n    -0.5 0.5 l\n    h\n    </path>\n    <path fill=\"sym-stroke\" fillrule=\"eofill\">\n    -0.6 -0.6 m\n    0.6 -0.6 l\n    0.6 0.6 l\n    -0.6 0.6 l\n    h\n    -0.4 -0.4 m\n    0.4 -0.4 l\n    0.4 0.4 l\n    -0.4 0.4 l\n    h\n    </path>\n    </group>\n    </symbol>\n    <symbol name=\"mark/cross(sx)\" transformations=\"translations\">\n    <group>\n    <path fill=\"sym-stroke\">\n    -0.43 -0.57 m\n    0.57 0.43 l\n    0.43 0.57 l\n    -0.57 -0.43 l\n    h\n    </path>\n    <path fill=\"sym-stroke\">\n    -0.43 0.57 m\n    0.57 -0.43 l\n    0.43 -0.57 l\n    -0.57 0.43 l\n    h\n    </path>\n    </group>\n    </symbol>\n    <symbol name=\"arrow/fnormal(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"white\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/pointed(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"sym-stroke\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -0.8 0 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/fpointed(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"white\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -0.8 0 l\n    -1 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/linear(spx)\">\n    <path stroke=\"sym-stroke\" pen=\"sym-pen\">\n    -1 0.333 m\n    0 0 l\n    -1 -0.333 l\n    </path>\n    </symbol>\n    <symbol name=\"arrow/fdouble(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"white\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    -1 0 m\n    -2 0.333 l\n    -2 -0.333 l\n    h\n    </path>\n    </symbol>\n    <symbol name=\"arrow/double(spx)\">\n    <path stroke=\"sym-stroke\" fill=\"sym-stroke\" pen=\"sym-pen\">\n    0 0 m\n    -1 0.333 l\n    -1 -0.333 l\n    h\n    -1 0 m\n    -2 0.333 l\n    -2 -0.333 l\n    h\n    </path>\n    </symbol>\n    <pen name=\"heavier\" value=\"0.8\"/>\n    <pen name=\"fat\" value=\"1.2\"/>\n    <pen name=\"ultrafat\" value=\"2\"/>\n    <symbolsize name=\"large\" value=\"5\"/>\n    <symbolsize name=\"small\" value=\"2\"/>\n    <symbolsize name=\"tiny\" value=\"1.1\"/>\n    <arrowsize name=\"large\" value=\"10\"/>\n    <arrowsize name=\"small\" value=\"5\"/>\n    <arrowsize name=\"tiny\" value=\"3\"/>\n    <color name=\"red\" value=\"1 0 0\"/>\n    <color name=\"green\" value=\"0 1 0\"/>\n    <color name=\"blue\" value=\"0 0 1\"/>\n    <color name=\"yellow\" value=\"1 1 0\"/>\n    <color name=\"orange\" value=\"1 0.647 0\"/>\n    <color name=\"gold\" value=\"1 0.843 0\"/>\n    <color name=\"purple\" value=\"0.627 0.125 0.941\"/>\n    <color name=\"gray\" value=\"0.745\"/>\n    <color name=\"brown\" value=\"0.647 0.165 0.165\"/>\n    <color name=\"navy\" value=\"0 0 0.502\"/>\n    <color name=\"pink\" value=\"1 0.753 0.796\"/>\n    <color name=\"seagreen\" value=\"0.18 0.545 0.341\"/>\n    <color name=\"turquoise\" value=\"0.251 0.878 0.816\"/>\n    <color name=\"violet\" value=\"0.933 0.51 0.933\"/>\n    <color name=\"darkblue\" value=\"0 0 0.545\"/>\n    <color name=\"darkcyan\" value=\"0 0.545 0.545\"/>\n    <color name=\"darkgray\" value=\"0.663\"/>\n    <color name=\"darkgreen\" value=\"0 0.392 0\"/>\n    <color name=\"darkmagenta\" value=\"0.545 0 0.545\"/>\n    <color name=\"darkorange\" value=\"1 0.549 0\"/>\n    <color name=\"darkred\" value=\"0.545 0 0\"/>\n    <color name=\"lightblue\" value=\"0.678 0.847 0.902\"/>\n    <color name=\"lightcyan\" value=\"0.878 1 1\"/>\n    <color name=\"lightgray\" value=\"0.827\"/>\n    <color name=\"lightgreen\" value=\"0.565 0.933 0.565\"/>\n    <color name=\"lightyellow\" value=\"1 1 0.878\"/>\n    <dashstyle name=\"dashed\" value=\"[4] 0\"/>\n    <dashstyle name=\"dotted\" value=\"[1 3] 0\"/>\n    <dashstyle name=\"dash dotted\" value=\"[4 2 1 2] 0\"/>\n    <dashstyle name=\"dash dot dotted\" value=\"[4 2 1 2 1 2] 0\"/>\n    <textsize name=\"large\" value=\"\\large\"/>\n    <textsize name=\"Large\" value=\"\\Large\"/>\n    <textsize name=\"LARGE\" value=\"\\LARGE\"/>\n    <textsize name=\"huge\" value=\"\\huge\"/>\n    <textsize name=\"Huge\" value=\"\\Huge\"/>\n    <textsize name=\"small\" value=\"\\small\"/>\n    <textsize name=\"footnote\" value=\"\\footnotesize\"/>\n    <textsize name=\"tiny\" value=\"\\tiny\"/>\n    <textstyle name=\"center\" begin=\"\\begin{center}\" end=\"\\end{center}\"/>\n    <textstyle name=\"itemize\" begin=\"\\begin{itemize}\" end=\"\\end{itemize}\"/>\n    <textstyle name=\"item\" begin=\"\\begin{itemize}\\item{}\" end=\"\\end{itemize}\"/>\n    <gridsize name=\"4 pts\" value=\"4\"/>\n    <gridsize name=\"8 pts (~3 mm)\" value=\"8\"/>\n    <gridsize name=\"16 pts (~6 mm)\" value=\"16\"/>\n    <gridsize name=\"32 pts (~12 mm)\" value=\"32\"/>\n    <gridsize name=\"10 pts (~3.5 mm)\" value=\"10\"/>\n    <gridsize name=\"20 pts (~7 mm)\" value=\"20\"/>\n    <gridsize name=\"14 pts (~5 mm)\" value=\"14\"/>\n    <gridsize name=\"28 pts (~10 mm)\" value=\"28\"/>\n    <gridsize name=\"56 pts (~20 mm)\" value=\"56\"/>\n    <anglesize name=\"90 deg\" value=\"90\"/>\n    <anglesize name=\"60 deg\" value=\"60\"/>\n    <anglesize name=\"45 deg\" value=\"45\"/>\n    <anglesize name=\"30 deg\" value=\"30\"/>\n    <anglesize name=\"22.5 deg\" value=\"22.5\"/>\n    <tiling name=\"falling\" angle=\"-60\" step=\"4\" width=\"1\"/>\n    <tiling name=\"rising\" angle=\"30\" step=\"4\" width=\"1\"/>\n    <layout paper=\"595 842\" origin=\"0 0\" frame=\"595 842\" skip=\"32\" crop=\"yes\"/>\n    </ipestyle>\n    <page>\n    <layer name=\"alpha\"/>\n    <view layers=\"alpha\" active=\"alpha\"/>\n";
        }
    }
}
