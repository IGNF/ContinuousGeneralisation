using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CCorrepondObjects ;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;



namespace MorphingClass.CMorphingAlgorithms
{
    /// <summary>
    /// a class based Algorithm of Linear Interpolation
    /// </summary>
    /// <remarks>the result is in InterFrCPl</remarks>
    public class CLIA
    {

        private List<CPoint> _FrCptLt;
        private List<CPoint> _ToCptLt;
        public double dblFrTotalLength { set; get; }
        public double dblToTotalLength { set; get; }
        //public IEnumerable<CCorrCpts> IEnumerable<CCorrCpts> { set; get; }
        //private List<CCorrCpts> _CorrCptsLt;


        public CLIA(CPolyline frcpl, CPolyline tocpl, bool isDP = false)
        {
            _FrCptLt = frcpl.CptLt;
            _ToCptLt = tocpl.CptLt;

            Preprocess(frcpl.CptLt, tocpl.CptLt, isDP);
        }

        public CLIA(List<CPoint> frcptlt, List<CPoint> tocptlt, bool isDP = false)
        {
            _FrCptLt = frcptlt;
            _ToCptLt = tocptlt;

            Preprocess(frcptlt, tocptlt, isDP);
        }

        private void Preprocess(List<CPoint> frcptlt, List<CPoint> tocptlt, bool isDP = false)
        {
            CGeoFunc.CalAbsAndRatioLengthFromStart(frcptlt, isDP);
            CGeoFunc.CalAbsAndRatioLengthFromStart(tocptlt, isDP);

            this.dblFrTotalLength = frcptlt.GetLastT().dblAbsLengthFromStart;
            this.dblToTotalLength = tocptlt.GetLastT().dblAbsLengthFromStart;
        }

        /// <summary>
        /// linear interpolation between two lists of points
        /// </summary>
        /// <param name="isDP">whether we are running a Dynamic Programming algorithm (the default value is false)</param>
        /// <remarks></remarks>
        public IEnumerable<CCorrCpts> CLI(bool blnReturnFirstPair=true)
        {
            var frcptlt = _FrCptLt;
            var tocptlt = _ToCptLt;

            if (blnReturnFirstPair == true)
            {
                yield return new CCorrCpts(frcptlt[0], tocptlt[0]);  //the first pair
            }


            //middle pairs
            if (frcptlt.Count > 1 && tocptlt.Count > 1)
            {
                int intFrCount = 1;
                int intToCount = 1;
                int intFrRatioNum = frcptlt.Count - 1;  //we don't need add the first one and the last one
                int intToRatioNum = tocptlt.Count - 1;  //we don't need add the first one and the last one

                while (intFrCount < intFrRatioNum || intToCount < intToRatioNum)
                {
                    if (frcptlt[intFrCount].dblRatioLengthFromStart < tocptlt[intToCount].dblRatioLengthFromStart)
                    {
                        CPoint cpt = CGeoFunc.QueryCPointByLength(frcptlt[intFrCount], intToCount, tocptlt);

                        //we record "dblRatioLengthFromStart" so that we can compute the distance between neighbour points (e.g. distance between tocptlt[i-1] to tocptlt[i]) easily
                        //The original points, which are sotred in frcptlt or tocptlt have already had "dblRatioLengthFromStart"
                        cpt.dblRatioLengthFromStart = frcptlt[intFrCount].dblRatioLengthFromStart;

                        yield return new CCorrCpts(frcptlt[intFrCount], cpt);

                        intFrCount++;
                    }
                    else if (frcptlt[intFrCount].dblRatioLengthFromStart > tocptlt[intToCount].dblRatioLengthFromStart)
                    {
                        CPoint cpt = CGeoFunc.QueryCPointByLength(tocptlt[intToCount], intFrCount, frcptlt);
                        cpt.dblRatioLengthFromStart = tocptlt[intToCount].dblRatioLengthFromStart;

                        yield return new CCorrCpts(cpt, tocptlt[intToCount]);

                        intToCount++;
                    }
                    else
                    {
                        yield return new CCorrCpts(frcptlt[intFrCount], tocptlt[intToCount]);
                        intFrCount++;
                        intToCount++;
                    }
                }

                //the last pair
                yield return new CCorrCpts(frcptlt.GetLastT(), tocptlt.GetLastT());
            }
            else if (frcptlt.Count > 1 && tocptlt.Count == 1)
            {
                for (int i = 1; i < frcptlt.Count; i++)
                {
                    yield return new CCorrCpts(frcptlt[i], tocptlt[0]);
                }
            }
            if (frcptlt.Count == 1 && tocptlt.Count > 1)
            {
                for (int i = 1; i < tocptlt.Count; i++)
                {
                    yield return new CCorrCpts(frcptlt[0], tocptlt[i]);
                }
            }
            else //if (frcptlt.Count == 1 && tocptlt.Count == 1)
            {
                //do nothing
            }
        }

    }
}
