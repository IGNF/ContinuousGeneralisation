using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;

using MorphingClass.CEntity;
using MorphingClass.CMorphingMethods;
using MorphingClass.CMorphingMethods.CMorphingMethodsBase;
using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;
using MorphingClass.CCorrepondObjects;


namespace MorphingClass.CGeneralizationMethods
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Douglas-Peucker线综合算法</remarks>
    public class CDPSimplify : CMorphingBaseCpl
    {
        //510546
        public static int _intEdgeCountBefore = 0;
        public static int _intEdgeCountAfter = 0;

        Stopwatch _pStopwatch = new Stopwatch();
        public CDPSimplify()
        {

        }

        public CDPSimplify(List<CPolyline> pCPlLt)
        {
            _CPlLt = pCPlLt;
            DivideByDP(pCPlLt);
        }

        public CDPSimplify(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline>(ParameterInitialize, 1);
            _CPlLt = this.ObjCGeoLtLt[0].AsExpectedClassEb<CPolyline, CGeoBase>().ToList();
            DivideByDP(_CPlLt);
        }

        private void DivideByDP(List<CPolyline> cptllt)
        {
            _pStopwatch.Start();
            for (int i = 0; i < cptllt.Count; i++)
            {
                cptllt[i].SetVirtualPolyline();
                DivideCplByDP(cptllt[i], cptllt[i].pVirtualPolyline);
            }
            _pStopwatch.Stop ();
        }


        /// <summary>
        /// DPSimplify
        /// </summary>
        /// <remarks>one of the three parameters should be a real parameter, and the others are -1</remarks>
        public void DPSimplify(double dblThresholdDis, double dblRemainPointsRatio, double dblRemainPoints, double dblDeleteNum)
        {
            int intRemainPoints = Convert.ToInt16(dblRemainPoints);
            List<CPolyline> cpllt = _CPlLt;
            //CGeoFunc.CalDistanceParameters<CPolyline, CPolyline>(cpllt);


            _pStopwatch.Start();
            //get threshold
            double dblTDis = CalThresholdDis(dblThresholdDis, dblRemainPointsRatio, dblRemainPoints, dblDeleteNum);

            List<CPolyline> newcpllt = new List<CPolyline>(cpllt.Count);  // the capacity is not so good
            for (int i = 0; i < cpllt.Count; i++)
            {
                Console.WriteLine("polyline number: " + i);
                List<CPoint> cptlt = cpllt[i].CptLt;
                var firstcpt = cptlt.First();
                var last_cpt = cptlt.GetLastT();

                List<CPoint> newcptlt = new List<CPoint>(cptlt.Count);  // the capacity is not so good
                newcptlt.Add(firstcpt);  //first point
                //middle point
                if (firstcpt.Compare(last_cpt) != 0)  //Polyline
                {
                    GetNewCptLt(cpllt[i], cpllt[i].pVirtualPolyline, ref newcptlt, dblTDis);
                }
                else  //Polygon
                {
                    RecursivelyGetNewCptLtPG(cpllt[i], cpllt[i].pVirtualPolyline, ref newcptlt, dblTDis, 2);
                }
                newcptlt.Add(last_cpt);  //last point
                CPolyline newcpl = new CPolyline(i, newcptlt);
                newcpllt.Add(newcpl);
            }
            _pStopwatch.Stop();
            _ParameterInitialize.tsslTime.Text = _pStopwatch.ElapsedMilliseconds.ToString();

            CParameterResult ParameterResult = new CParameterResult();
            ParameterResult.CResultPlLt = newcpllt;
            _ParameterResult = ParameterResult;
        }


        

        /// <summary>
        /// Calculate the Threshold for the DP algorithm
        /// </summary>
        /// <param name="dblThresholdDis"></param>
        /// <param name="dblRemainPointsRatio"></param>
        /// <param name="dblRemainPoints"></param>
        /// <param name="dblVerySmall"></param>
        /// <remarks>there are three ways to calculate the threshold.</remarks>
        private double CalThresholdDis(double dblThresholdDis, 
            double dblRemainPointsRatio, double dblRemainPoints, double dblDeleteNum)
        {
            int intRemainPoints = Convert.ToInt16(dblRemainPoints);

            //get threshold
            double dblTDis = 0;
            if (dblThresholdDis != -1 && dblRemainPointsRatio == -1 && intRemainPoints == -1 && dblDeleteNum == -1)  
            {
                // By ThresholdDis
                dblTDis = dblThresholdDis;
            }
            else
            {
                int intEdgeNum = 0;
                List<CPolyline> cptllt = _CPlLt;
                for (int i = 0; i < cptllt.Count; i++)
                {
                    intEdgeNum += (cptllt[i].CptLt.Count - 1);
                }

                int intDeleteEdgeNum = -1;
                if (dblThresholdDis == -1 && dblRemainPointsRatio != -1 && intRemainPoints == -1 && dblDeleteNum == -1)  
                {
                    // By RemainPointsRatio
                    intDeleteEdgeNum = Convert.ToInt32(intEdgeNum * (1 - dblRemainPointsRatio));
                }
                else if (dblThresholdDis == -1 && dblRemainPointsRatio == -1 && intRemainPoints != -1 && dblDeleteNum == -1)  
                {
                    // By RemainPoints
                    intDeleteEdgeNum = intEdgeNum - intRemainPoints;
                }
                else if (dblThresholdDis == -1 && dblRemainPointsRatio == -1 && intRemainPoints == -1 && dblDeleteNum != -1)  
                {
                    // By RemainPoints
                    intDeleteEdgeNum = Convert.ToInt32(dblDeleteNum);
                } 
                else
                {
                    throw new ArgumentOutOfRangeException("invalid parameters!");
                }

                dblTDis = CalTDisByDeletePtNum(cptllt, intDeleteEdgeNum);
                //GetTDis(intDeletePtNum, dblMaxDisLt);
            }
            return dblTDis;
        }

        /// <summary>
        /// find the farthest point for each contructured baseline
        /// </summary>
        /// <param name="dcpl"></param>
        /// <param name="pVtPl"></param>
        public void DivideCplByDP(CPolyline dcpl, CVirtualPolyline pVtPl)
        {
            List<CPoint> dcptlt = dcpl.CptLt;
            Stack<CVirtualPolyline> pVtPlSk = new Stack<CVirtualPolyline>();
            pVtPlSk.Push(pVtPl);

            do
            {
                var subVtPl = pVtPlSk.Pop();
                if ((subVtPl.intToID - subVtPl.intFrID >= 2))
                {
                    double dblMaxDis = -1;
                    int intMaxDisID = -1;
                    double dblFromDis = 0;
                    subVtPl.SetBaseLine(dcptlt[subVtPl.intFrID], dcptlt[subVtPl.intToID]);
                    subVtPl.pBaseLine.SetSlope();
                    subVtPl.pBaseLine.SetDenominatorForDis();
                    for (int i = subVtPl.intFrID + 1; i < subVtPl.intToID; i++)
                    {
                        //throw new ArgumentException("make sure you have set length for pBaseLine!");
                        dblFromDis = subVtPl.pBaseLine.QueryPtHeight(dcptlt[i]);
                        if (dblFromDis > dblMaxDis)
                        {
                            dblMaxDis = dblFromDis;
                            intMaxDisID = i;
                        }
                    }

                    subVtPl.intMaxDisID = intMaxDisID;
                    subVtPl.dblMaxDis = dblMaxDis;
                    subVtPl.DivideByID(intMaxDisID);
                    pVtPlSk.Push(subVtPl.CRightPolyline);
                    pVtPlSk.Push(subVtPl.CLeftPolyline);

                    subVtPl.pBaseLine = null; //to use as little memory as possible
                }
            } while (pVtPlSk.Count > 0);            
        }

        private double CalTDisByDeletePtNum<T>(List<T> CPlLt, int intDeletePtNum) where T : CPolyline
        {
            var dblMaxDisLt = new List<double>();
            for (int i = 0; i < CPlLt.Count; i++)
            {
               dblMaxDisLt.AddRange(CollectMaxDis(CPlLt[i].pVirtualPolyline));
            }
            dblMaxDisLt.Sort();
            return GetTDis(intDeletePtNum, dblMaxDisLt);
        }





        /// <summary>
        /// Recursively Collect MaxDis (height) of the points to their base lines
        /// </summary>
        /// <remarks>if the height of a point is smaller than the height of a lower-level height, 
        /// the larger lower-level will be used</remarks>
        private List<double> CollectMaxDis(CVirtualPolyline pVtPl)
        {
            int intCount = pVtPl.intToID - pVtPl.intFrID - 1;
            var dblMaxDisLt = new List<double>(intCount);  //we don't need to consider the MaxDis of the two ends of the polyline

            if (intCount == 0)
            {
                return dblMaxDisLt;
            }
            else if (intCount<0)
            {
                throw new ArgumentOutOfRangeException("intCount cannot be negative!");
            }


            Stack<CVirtualPolyline> pVtPlSk = new Stack<CVirtualPolyline>();
            var currentVtpl = pVtPl;
            do
            {
                if (currentVtpl.dblMaxDis >= 0)  //this implies that there are at least three vertices on currentVtpl
                {
                    pVtPlSk.Push(currentVtpl);
                    currentVtpl = currentVtpl.CLeftPolyline;
                }
                else
                {
                    currentVtpl = pVtPlSk.Pop();

                    //get the max distance by considering lower-level
                    //double dblMaxDis = Math.Max(currentVtpl.dblMaxDis, 
                    //Math.Max(currentVtpl.CLeftPolyline.dblMaxDisLargerThanChildren, 
                    //currentVtpl.CRightPolyline.dblMaxDisLargerThanChildren));  
                    //currentVtpl.dblMaxDisLargerThanChildren = dblMaxDis;

                    double dblMaxDis = currentVtpl.dblMaxDis;
                    dblMaxDisLt.Add(dblMaxDis);

                    currentVtpl = currentVtpl.CRightPolyline;
                    if (currentVtpl.dblMaxDis >= 0)
                    {
                        pVtPlSk.Push(currentVtpl);
                        currentVtpl = currentVtpl.CLeftPolyline;
                    }
                }
            } while (pVtPlSk.Count > 0);

            if (intCount!=dblMaxDisLt.Count)
            {
                throw new ArgumentException("they should be the same!");
            }

            return dblMaxDisLt;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpl"></param>
        /// <param name="pVtPl"></param>
        /// <param name="newBaseLine"></param>
        /// <param name="newcptlt"></param>
        /// <param name="dblPropotion"></param>
        /// <remarks>Notice that this function gets the vertices without the first and last vertices</remarks>
        public void GetNewCptLt(CPolyline cpl, CVirtualPolyline pVtPl, ref List<CPoint> newcptlt, double dblTDis)
        {
            if (pVtPl.dblMaxDis <dblTDis)
            {
                return;
            }

            List<CPoint> dcptlt = cpl.CptLt;
            Stack<CVirtualPolyline> pVtPlSk = new Stack<CVirtualPolyline>();

            var currentVtpl = pVtPl;
            do
            {
                if (currentVtpl.dblMaxDis >= dblTDis)  //this implies that there are at least three vertices on currentVtpl
                {
                    pVtPlSk.Push(currentVtpl);
                    currentVtpl = currentVtpl.CLeftPolyline;  
                }
                else
                {
                    currentVtpl = pVtPlSk.Pop();
                    newcptlt.Add(dcptlt[currentVtpl.intMaxDisID]);

                    currentVtpl = currentVtpl.CRightPolyline;
                    if (currentVtpl.dblMaxDis >= dblTDis)
                    {
                        pVtPlSk.Push(currentVtpl);
                        currentVtpl = currentVtpl.CLeftPolyline;
                    }
                }
            } while (pVtPlSk.Count >0);
        }

        private void RecursivelyGetNewCptLtPG(CPolyline cpl, CVirtualPolyline pVtPl, 
            ref List<CPoint> newcptlt, double dblTDis, int intDepth)
        {
            if (pVtPl.CLeftPolyline == null)
            {
                return;
            }

            if (intDepth <= 0)
            {
                GetNewCptLt(cpl, pVtPl, ref newcptlt, dblTDis);

                //if (pVtPl.dblMaxDis >= dblTDis)
                //{
                //    GetNewCptLt(cpl, pVtPl.CLeftPolyline, ref newcptlt, dblTDis);
                //    newcptlt.Add(cpl.CptLt[pVtPl.intMaxDisID]);
                //    GetNewCptLt(cpl, pVtPl.CRightPolyline, ref newcptlt, dblTDis);
                //}
            }
            else   //at least keep 3 vertices for a polyline
            {
                intDepth--;
                if (pVtPl.CLeftPolyline .dblMaxDis >=pVtPl.CRightPolyline .dblMaxDis )
                {
                    RecursivelyGetNewCptLtPG(cpl, pVtPl.CLeftPolyline, ref newcptlt, dblTDis, intDepth);
                    newcptlt.Add(cpl.CptLt[pVtPl.intMaxDisID]);
                    GetNewCptLt(cpl, pVtPl.CRightPolyline, ref newcptlt, dblTDis);
                }
                else
                {
                    GetNewCptLt(cpl, pVtPl.CLeftPolyline, ref newcptlt, dblTDis);
                    newcptlt.Add(cpl.CptLt[pVtPl.intMaxDisID]);
                    RecursivelyGetNewCptLtPG(cpl, pVtPl.CRightPolyline, ref newcptlt, dblTDis, intDepth);
                }
            }

        }

        private static double GetTDis(int intDeletePtNum, List<double> dblMaxDisLt)
        {
            double dblTDis = 0;
            if (intDeletePtNum >= dblMaxDisLt.Count)
            {
                dblTDis = dblMaxDisLt.GetLastT() + CConstants.dblVerySmallCoord;
            }
            else
            {
                //we should find a value larger than dblMaxDisLt[intDeletePtNum-1], 
                //so that we can delete at least intDeletePtNum points
                bool blnDefined = false;
                for (int i = intDeletePtNum; i < dblMaxDisLt.Count; i++)
                {
                    if (dblMaxDisLt[i] > dblMaxDisLt[intDeletePtNum - 1])
                    {
                        dblTDis = (dblMaxDisLt[i] + dblMaxDisLt[intDeletePtNum - 1]) / 2;
                        blnDefined = true;
                        break;
                    }
                }

                if (blnDefined == false)  //if dblMaxDisLt[intDeletePtNum-1] is the largest MaxDis
                {
                    dblTDis = dblMaxDisLt[intDeletePtNum - 1] + CConstants.dblVerySmallCoord;
                }
            }
            return dblTDis;
        }

        #region DPMorph
        public void DPMorph(double dblThresholdDis, double dblRemainPointsRatio, double dblRemainPoints, double dblPropotion)
        {
            //int intRemainPoints = Convert.ToInt16(dblRemainPoints);
            //List<CPolyline> cptllt = _CPlLt;
            //CGeoFunc.CalDistanceParameters(cptllt);

            ////get threshold
            //double dblTDis = CalThresholdDis(dblThresholdDis, dblRemainPointsRatio, dblRemainPoints, dblVerySmall);

            //List<CPolyline> newcpllt = new List<CPolyline>(cptllt.Count);
            //foreach (CPolyline cpl in cptllt)
            //{
            //    ConfirmMoveInfo(cpl, cpl.pVirtualPolyline, dblTDis);
            //    newcpllt.Add(DPCplMorph(cpl, dblPropotion));
            //}

            //CParameterResult ParameterResult = new CParameterResult();
            //ParameterResult.CResultPlLt = newcpllt;
            //_ParameterResult = ParameterResult;
        }


        public void ConfirmMoveInfo(CPolyline cpl, CVirtualPolyline pVtPl, double dblTDis)
        {
            List<CPoint> cptlt = cpl.CptLt;
            cptlt[0].isMoveable = false;
            cptlt[cptlt.Count - 1].isMoveable = false;
            for (int j = 1; j < cptlt.Count - 1; j++)
            {
                cptlt[j].isMoveable = true;
            }
            RecursivelyConfirmMoveInfo(cpl, cpl.pVirtualPolyline, dblTDis);
        }

        private void RecursivelyConfirmMoveInfo(CPolyline cpl, CVirtualPolyline pVtPl, double dblTDis)
        {
            if (pVtPl.CLeftPolyline == null)
            {
                return;
            }

            if (pVtPl.dblMaxDis >= dblTDis)
            {
                cpl.CptLt[pVtPl.intMaxDisID].isMoveable = false;
                RecursivelyConfirmMoveInfo(cpl, pVtPl.CLeftPolyline, dblTDis);
                RecursivelyConfirmMoveInfo(cpl, pVtPl.CRightPolyline, dblTDis);
            }
            else
            {
                SubRecursivelyConfirmMoveInfo(cpl, pVtPl, dblTDis);
            }
        }

        private void SubRecursivelyConfirmMoveInfo(CPolyline cpl, CVirtualPolyline pVtPl, double dblTDis)
        {
            if (pVtPl.CLeftPolyline == null)
            {
                return;
            }

            cpl.CptLt[pVtPl.intMaxDisID].isMoveable = true;
            CPoint frcpt = cpl.CptLt[pVtPl.intFrID];
            CPoint tocpt = cpl.CptLt[pVtPl.intToID];
            CPoint cpt = cpl.CptLt[pVtPl.intMaxDisID];

            double dblPreDis = frcpt.DistanceTo(cpt);
            double dblSucDis = tocpt.DistanceTo(cpt);
            double dblTotalDis = dblPreDis + dblSucDis;
            double dblRatio = 0;
            if (dblTotalDis != 0)
            {
                dblRatio = dblPreDis / dblTotalDis;
            }

            //double dblTargetX = (1 - dblRatio) * frcpt.X + dblRatio * tocpt.X;
            //double dblTargetY = (1 - dblRatio) * frcpt.Y + dblRatio * tocpt.Y;
            //CPoint targetcpt = new CPoint(-1, dblTargetX,dblTargetY);
            CPoint targetcpt = CGeoFunc.GetInbetweenCpt(frcpt, tocpt, dblRatio, -1);
            pVtPl.dblRatioforMovePt = dblRatio;
            pVtPl.dblLengthforMovePt = CGeoFunc.CalDis(targetcpt, cpt);
            pVtPl.dblAngleDiffforMovePt = CGeoFunc.CalAngle_Counterclockwise(cpt, targetcpt, tocpt);
            //pVtPl.dblDifffromMovePtX = cpt.X - dblTargetX;
            //pVtPl.dblDifffromMovePtY = cpt.Y - dblTargetY;

            SubRecursivelyConfirmMoveInfo(cpl, pVtPl.CLeftPolyline, dblTDis);
            SubRecursivelyConfirmMoveInfo(cpl, pVtPl.CRightPolyline, dblTDis);

        }

        public CPolyline DPCplMorph(CPolyline cpl, double dblPropotion)
        {
            List<CPoint> cptlt = cpl.CptLt;

            List<CPoint> newcptlt = new List<CPoint>(cptlt.Count);
            newcptlt.Add(cptlt[0]);  //add the first point
            CEdge pBaseLine = new CEdge(cptlt[0], cptlt[cptlt.Count - 1]);
            RecursivelyMovePt(cpl, cpl.pVirtualPolyline, ref pBaseLine, ref newcptlt, dblPropotion);
            newcptlt.Add(cptlt[cptlt.Count - 1]); //add the last point

            CPolyline newcpl = new CPolyline(cpl.ID, newcptlt);
            return newcpl;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpl"></param>
        /// <param name="pVtPl"></param>
        /// <param name="newBaseLine"></param>
        /// <param name="newcptlt"></param>
        /// <param name="dblPropotion"></param>
        /// <remarks>Notice that this function gets the vertices without the first and last ones</remarks>
        private void RecursivelyMovePt(CPolyline cpl, CVirtualPolyline pVtPl, 
            ref CEdge newBaseLine, ref List<CPoint> newcptlt, double dblPropotion)
        {
            if (pVtPl.CLeftPolyline != null)
            {
                CEdge newLeftBaseLine = new CEdge();
                CEdge newRightBaseLine = new CEdge();
                CPoint newcpt = new CPoint();

                if (cpl.CptLt[pVtPl.intMaxDisID].isMoveable == false)
                {
                    newcpt = cpl.CptLt[pVtPl.intMaxDisID];
                    newLeftBaseLine = new CEdge(newBaseLine.FrCpt, newcpt);
                    newRightBaseLine = new CEdge(newcpt, newBaseLine.ToCpt);
                }
                else
                {
                    newBaseLine.SetAxisAngle();
                    newcpt = newBaseLine.QueryMovedPt(pVtPl.dblRatioforMovePt, pVtPl.dblLengthforMovePt, 
                        pVtPl.dblAngleDiffforMovePt, dblPropotion, cpl.CptLt[pVtPl.intMaxDisID].ID);
                    newLeftBaseLine = new CEdge(newBaseLine.FrCpt, newcpt);
                    newRightBaseLine = new CEdge(newcpt, newBaseLine.ToCpt);
                }

                RecursivelyMovePt(cpl, pVtPl.CLeftPolyline, ref newLeftBaseLine, ref newcptlt, dblPropotion);
                newcptlt.Add(newcpt);
                RecursivelyMovePt(cpl, pVtPl.CRightPolyline, ref newRightBaseLine, ref newcptlt, dblPropotion);
            }
            //newBaseLine.SetEmpty2();

        }
        #endregion



        //public static CPolygon SimplifyCpgFreeEdges(CPolygon EnlargedCpg, List<CEdge> OriginalCEdgeLt, double dblDepsilon)
        //{
        //    //var ConflictCEdgeLt = new List<CEdge>(OriginalCEdgeLt);
        //    EnlargedCpg.SetGeometricProperties();
        //    //ConflictCEdgeLt.AddRange(EnlargedCpg.CEdgeLt);

        //    ////***********why do we consider holes?
        //    //if (EnlargedCpg.HoleCpgLt != null)
        //    //{
        //    //    EnlargedCpg.HoleCpgLt.ForEach(holecpg => ConflictCEdgeLt.AddRange(holecpg.CEdgeLt));
        //    //}

        //    return SimplifyAccordRightAnglesAndExistEdges(EnlargedCpg, ConflictCEdgeLt, dblDepsilon);
        //}



        /// <summary>
        /// the angle from the baseline to the firstedge should smaller than PI
        /// </summary>
        /// <param name="cpg"></param>
        /// <param name="ocpg"></param>
        /// <param name="dblThreshold"></param>
        /// <returns>the first vertex and the last vertex are identical</returns>
        public static CPolygon SimplifyCpgAccordExistEdges(CPolygon EnlargedCpg,
            List<CEdge> OriginalCEdgeLt, string strSimplification, double dblThreshold)
        {
            //generate new polygon
            var newcptlt = SimplifyAccordExistEdges(EnlargedCpg, OriginalCEdgeLt, strSimplification, dblThreshold);
            if (EnlargedCpg.HoleCpgLt != null)
            {
                List<List<CPoint>> newholecptltlt;
                newholecptltlt = new List<List<CPoint>>(EnlargedCpg.HoleCpgLt.Count);
                foreach (var holeEnlargedCpg in EnlargedCpg.HoleCpgLt)
                {
                    var newholecptlt = SimplifyAccordExistEdges(holeEnlargedCpg, OriginalCEdgeLt, strSimplification, dblThreshold);
                    newholecptltlt.Add(newholecptlt);
                }
                var simplifiedCpg = new CPolygon(EnlargedCpg.ID, newcptlt, newholecptltlt);
                //simplifiedCpg.HoleCpgLt.ForEach(holecpg => holecpg.IsOriginal = false);  //these holes are 

                return simplifiedCpg;
            }
            else
            {
                return new CPolygon(EnlargedCpg.ID, newcptlt);
            }
        }

        private static List<CPoint> SimplifyAccordExistEdges(CPolygon EnlargedCpg,
            List<CEdge> OriginalCEdgeLt, string strSimplification, double dblThreshold)
        {
            //EnlargedCpg.SetGeometricProperties();
            EnlargedCpg.FormCEdgeLt();
            //EnlargedCpg.SetCEdgeLtLength();
            EnlargedCpg.SetCEdgeToCpts();
        //EnlargedCpg.SetCEdgeLtAxisAngle();
        //EnlargedCpg.SetAngleDiffLt();


        //CSaveFeature.SaveCpgEb(clipperMethods.ScaleCpgEb(CHelpFunc.MakeLt(EnlargedCpg), 1 / CConstants.dblFclipper), 
        //"EnlargedCpg", pesriSimpleFillStyle: esriSimpleFillStyle.esriSFSHollow, blnVisible: false);

            var EnlargedCEdgeHS = new HashSet<CEdge>(EnlargedCpg.CEdgeLt);
            //CSaveFeature.SaveCEdgeEb(clipperMethods.ScaleCEdgeEb( EnlargedCEdgeHS, 1 / CConstants.dblFclipper), "EnlargedCEdgeHS",
            //     blnVisible: false);
            _intEdgeCountBefore += EnlargedCpg.CptLt.Count - 1;
            var simplifiedcptlt = SimplifyAccordExistEdges(EnlargedCpg.CptLt, OriginalCEdgeLt, 
                EnlargedCEdgeHS, strSimplification, dblThreshold).ToList();
            _intEdgeCountAfter += simplifiedcptlt.Count - 1;

            //CPolygon simplifiedcpg = new CPolygon(EnlargedCpg.ID,
            //    clipperMethods.ScaleCptEb(simplifiedcptlt, 1 / CConstants.dblFclipper).ToList());
            //CSaveFeature.SaveCpg(simplifiedcpg, "simplifiedcpg",
            //    pesriSimpleFillStyle: esriSimpleFillStyle.esriSFSHollow, blnVisible: false);

            return simplifiedcptlt;
        }

        #region obsolete: SimplifyAccordRightAnglesAndExistEdges
        ///// <summary>
        ///// the angle from the baseline to the firstedge should smaller than PI
        ///// </summary>
        ///// <param name="cpg"></param>
        ///// <param name="ocpg"></param>
        ///// <param name="dblThreshold"></param>
        ///// <returns>the first vertex and the last vertex are identical</returns>
        //public static CPolygon SimplifyAccordRightAnglesAndExistEdges(CPolygon EnlargedCpg,
        //    List<CEdge> OriginalCEdgeLt, string strSimplification, double dblThreshold)
        //{
        //    //generate new polygon
        //    var newcptlt = SimplifyCptltAccordRightAnglesAndExistEdges(EnlargedCpg, OriginalCEdgeLt, strSimplification, dblThreshold);
        //    if (EnlargedCpg.HoleCpgLt != null)
        //    {
        //        List<List<CPoint>> newholecptltlt;
        //        newholecptltlt = new List<List<CPoint>>(EnlargedCpg.HoleCpgLt.Count);
        //        foreach (var holeEnlargedCpg in EnlargedCpg.HoleCpgLt)
        //        {
        //            var newholecptlt = SimplifyCptltAccordRightAnglesAndExistEdges(holeEnlargedCpg, OriginalCEdgeLt, strSimplification, dblThreshold);
        //            newholecptltlt.Add(newholecptlt);
        //        }
        //        var simplifiedCpg= new CPolygon(EnlargedCpg.ID, newcptlt, newholecptltlt);
        //        //simplifiedCpg.HoleCpgLt.ForEach(holecpg => holecpg.IsOriginal = false);  //these holes are 

        //        return simplifiedCpg;
        //    }
        //    else
        //    {
        //        return new CPolygon(EnlargedCpg.ID, newcptlt);
        //    }
        //}

        //private static List<CPoint> SimplifyCptltAccordRightAnglesAndExistEdges(CPolygon EnlargedCpg, 
        //    List<CEdge> OriginalCEdgeLt, string strSimplification, double dblThreshold)
        //{

        //    //EnlargedCpg.SetGeometricProperties();
        //    EnlargedCpg.FormCEdgeLt();
        //    EnlargedCpg.SetCEdgeLtLength();
        //    EnlargedCpg.SetCEdgeToCpts();
        //    EnlargedCpg.SetCEdgeLtAxisAngle();
        //    EnlargedCpg.SetAngleDiffLt();


        //    //the first vertex and the last vertex of EnlargedCpg.CptLt point to the same address in memory
        //    var cptlt = EnlargedCpg.CptLt.CopyEbWithoutFirstLastT(true, false).ToList();
        //    cptlt.ForEach(cpt => cpt.isCtrl = false);

        //    var dupcptlt = new List<CPoint>(2 * cptlt.Count);
        //    dupcptlt.AddRange(cptlt);
        //    dupcptlt.AddRange(cptlt);

        //    var cedgelt = EnlargedCpg.CEdgeLt;
        //    var lastcedge = cedgelt.GetLastT();
        //    var pdblAngleDiffLt = EnlargedCpg.dblAngleDiffLt;

        //    //the angle is almost 90 degrees, we fix the three points
        //    int intIndexCtrl = -1;
        //    var lastcpt = cptlt.GetLastT();
        //    for (int i = 0; i < cptlt.Count; i++)
        //    {
        //        if ((CCmpMethods.CmpDblRange(pdblAngleDiffLt[i], CConstants.dblHalfPI, CConstants.dblFiveDegreeRad) == 0 ||
        //           CCmpMethods.CmpDblRange(pdblAngleDiffLt[i], CConstants.dblThreeSecondPI, CConstants.dblFiveDegreeRad) == 0)
        //           && lastcedge.dblLength >= dblThreshold && cedgelt[i].dblLength >= dblThreshold)
        //        {
        //            lastcpt.isCtrl = true;
        //            dupcptlt[i].isCtrl = true;
        //            dupcptlt[i + 1].isCtrl = true;
        //            intIndexCtrl = i;
        //        }

        //        lastcpt = cptlt[i];
        //        lastcedge = cedgelt[i];
        //    }

        //    var newcptlt = new List<CPoint>(EnlargedCpg.CptLt.Count);  //newcptlt has at most cptlt.Count points
        //    if (intIndexCtrl == -1) //there is no control points, we perform DP algorithm to the building
        //    {
        //        newcptlt.AddRange(SimplifyAccordExistEdges(EnlargedCpg.CptLt, OriginalCEdgeLt, strSimplification, dblThreshold));

        //    }
        //    else //there are some control points, we perform DP algorithm to the split polylines
        //    {
        //        int intCurrentIndex = intIndexCtrl;
        //        var processcptlt = new List<CPoint> { new CPoint() };
        //        CPoint currentcpt = dupcptlt[intCurrentIndex];
        //        do
        //        {
        //            if (currentcpt.isCtrl == true)
        //            {
        //                if (processcptlt.Count >= 2)
        //                {
        //                    processcptlt.Add(currentcpt);
        //                    newcptlt.AddRange(
        //                        SimplifyAccordExistEdges(processcptlt, OriginalCEdgeLt, strSimplification, dblThreshold)
        //                        .CopyEbWithoutFirstLastT(false, true));

        //                    processcptlt = new List<CPoint> { new CPoint() };
        //                }
        //                else
        //                {
        //                    newcptlt.Add(currentcpt);
        //                }
        //                processcptlt[0] = currentcpt;  //make processcptlt ready for adding points                                          
        //            }
        //            else
        //            {
        //                processcptlt.Add(currentcpt);
        //            }
        //            currentcpt = dupcptlt[++intCurrentIndex];
        //        } while (currentcpt.GID != dupcptlt[intIndexCtrl].GID);

        //        newcptlt.Add(dupcptlt[intIndexCtrl]);
        //    }

        //    return newcptlt;
        //}

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cptlt">usually, the first point and the last point should not be identical</param>
        /// <param name="OriginalCEdgeLt"></param>
        /// <param name="EnlargedCEdgeHS"></param>
        /// <param name="strSimplification"></param>
        /// <param name="dblThreshold"></param>
        /// <returns></returns>
        private static IEnumerable<CPoint> SimplifyAccordExistEdges(List<CPoint> cptlt,
            List<CEdge> OriginalCEdgeLt, HashSet<CEdge> EnlargedCEdgeHS, string strSimplification, double dblThreshold)
        {
            switch (strSimplification)
            {
                case "Non":
                    return cptlt;
                case "DP":
                    //throw new ArgumentException("need to be implemented");
                    return DPSimplifyAccordExistEdges(cptlt, OriginalCEdgeLt, EnlargedCEdgeHS, dblThreshold);
                case "Imai-Iri":
                    return ImaiIriSimplifyAccordExistEdges(cptlt, OriginalCEdgeLt, EnlargedCEdgeHS, dblThreshold);
                default:
                    Console.WriteLine("Default case");
                    return null;
            }
        }

        /// <summary>
        /// we can improve by saving a pEdgeGrid for every orginal cpg; ********************************
        /// for the EnlargedCEdgeHS, we make another pEdgeGrid ********************************
        /// </summary>
        /// <param name="cptlt"></param>
        /// <param name="OriginalCEdgeLt"></param>
        /// <param name="EnlargedCEdgeHS"></param>
        /// <param name="dblThreshold"></param>
        /// <returns></returns>
        private static IEnumerable<CPoint> ImaiIriSimplifyAccordExistEdges(List<CPoint> cptlt,
            List<CEdge> OriginalCEdgeLt, HashSet<CEdge> EnlargedCEdgeHS, double dblThreshold)
        {
            if (cptlt.Count <= 2)
            {
                throw new ArgumentOutOfRangeException("There is no points for simplification!");
            }

            var allcedgelt = new List<CEdge>(OriginalCEdgeLt);
            allcedgelt.AddRange(EnlargedCEdgeHS);
            var pEdgeGrid = new CEdgeGrid(allcedgelt);

            var CNodeLt = new List<CNode>(cptlt.Count);
            CNodeLt.EveryElementNew();
            CNodeLt.SetIndexID();
            for (int i = 0; i < cptlt.Count - 1; i++)
            {
                CNodeLt[i].NbrCNodeLt = new List<CGeometry.CNode>();
                CNodeLt[i].NbrCNodeLt.Add(CNodeLt[i + 1]);
                for (int j = i + 2; j < cptlt.Count; j++)
                {
                    var subcptlt = cptlt.GetRange(i, j - i + 1);
                    int intIndexMaxdis;
                    if (IsCutValid(subcptlt, pEdgeGrid, dblThreshold, out intIndexMaxdis))
                    {
                        CNodeLt[i].NbrCNodeLt.Add(CNodeLt[j]);
                    }                    
                }
            }

            BFS(CNodeLt[0], CNodeLt.GetLastT());
            var currentCNode = CNodeLt[0];
            while (currentCNode != null)
            {
                yield return cptlt[currentCNode.indexID];
                currentCNode = currentCNode.NextCNode;
            }
        }


        public static void BFS(CNode startCNode, CNode goalCNode)
        {
            var CNodeQueue = new Queue<CNode>();
            CNodeQueue.Enqueue(startCNode);
            bool isFoundGoal = false;

            while (CNodeQueue.Count > 0 && isFoundGoal == false)
            {
                var currentCNode = CNodeQueue.Dequeue();
                foreach (var nbrcnode in currentCNode.NbrCNodeLt)
                {
                    if (nbrcnode.strColor == "white")
                    {
                        nbrcnode.strColor = "gray";
                        nbrcnode.PrevCNode = currentCNode;

                        CNodeQueue.Enqueue(nbrcnode);
                    }

                    if (nbrcnode.GID == goalCNode.GID)
                    {
                        isFoundGoal = true;
                    }
                }
                currentCNode.strColor = "black";
            }

            // set NextCNode for each Node on the path
            var backCNode = goalCNode;
            while (backCNode.GID != startCNode.GID)
            {
                backCNode.PrevCNode.NextCNode = backCNode;

                backCNode = backCNode.PrevCNode;
            }

        }

        private static bool IsCutValid(List<CPoint> cptlt, CEdgeGrid pEdgeGrid,   
            double dblThreshold, out int intIndexMaxDis)
        {            
            //the distances from all the removed points to cedgebaseline should be smaller than a threshold
            var cedgebaseline = new CEdge(cptlt[0], cptlt.GetLastT());
            var IndexDisVP = ComputeMaxIndexDisVP(cedgebaseline, cptlt, 1, cptlt.Count - 1);
            intIndexMaxDis = IndexDisVP.val1;
            if (IndexDisVP.val2 >= dblThreshold)
            {
                return false;
            }

            //if the baseline is outside of the polygon, then the baseline is not a valid choice
            throw new ArgumentException(
                "IsClockwise is not helpful. instead test if the edge between the two edges. see CCptbCtgl");
            if (CGeoFunc.IsClockwise(cptlt, false) == false)
            {
                return false;
            }

            //we don't test the four edges
            var IgnoreCEdgeSS = new SortedSet<CEdge>
            {
                cedgebaseline.FrCpt.InCEdge,
                cedgebaseline.FrCpt.OutCEdge,
                cedgebaseline.ToCpt.InCEdge,
                cedgebaseline.ToCpt.OutCEdge
            };
            var blnIntersect = pEdgeGrid.BlnIntersect(cedgebaseline, true, true, true, IgnoreCEdgeSS);

            return !blnIntersect;

        }

        private static bool IsBaselineRightHandSide(CEdge CEdgeBaseline, CEdge CEdgeRef)
        {
            double dblAngle = CGeoFunc.CalAngle_Counterclockwise(CEdgeBaseline, CEdgeRef);
            if (dblAngle < Math.PI)  //it doesn't matter we are simplifying a exterior ring or a interior ring (hole)
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        ////*******************check the codes below***************************
        ////****************** should we keep at least three points?
        /// <summary>
        /// for an exterior ring, cptlt should be clockwise; for a hole, cptlt should be counter clockwise
        /// </summary>
        private static IEnumerable<CPoint> DPSimplifyAccordExistEdges(List<CPoint> cptlt,
            List<CEdge> OriginalCEdgeLt, HashSet<CEdge> EnlargedCEdgeHS, double dblThreshold)
        {
            if (cptlt.Count <= 2)
            {
                throw new ArgumentOutOfRangeException("There is no points for simplification!");
            }

            var allcedgelt = new List<CEdge>(OriginalCEdgeLt);
            allcedgelt.AddRange(EnlargedCEdgeHS);
            var pEdgeGrid = new CEdgeGrid(allcedgelt);

            var IndexSk = new Stack<CValPair<int, int>>();
            IndexSk.Push(new CValPair<int, int>(0, cptlt.Count - 1));

            do
            {
                var StartEndVP = IndexSk.Pop();

                var subcptlt = cptlt.GetRange(StartEndVP.val1, StartEndVP.val2 - StartEndVP.val1 + 1);
                int intIndexMaxdis;
                if (subcptlt.Count <= 2 || IsCutValid(subcptlt, pEdgeGrid, dblThreshold, out intIndexMaxdis))
                {
                    yield return cptlt[StartEndVP.val1];
                }
                else
                {
                    IndexSk.Push(new CValPair<int, int>(intIndexMaxdis + StartEndVP.val1, StartEndVP.val2));
                    IndexSk.Push(new CValPair<int, int>(StartEndVP.val1, intIndexMaxdis + StartEndVP.val1));
                }
            } while (IndexSk.Count > 0);
            yield return cptlt.GetLastT();
        }


        /// <summary>
        /// 
        /// </summary>
        private static CValPair<int, double> ComputeMaxIndexDisVP(CEdge cedge, List<CPoint> cptlt, int intStart, int intEnd)
        {
            cedge.SetSlope();
            cedge.SetDenominatorForDis();

            var MaxDisVP = new CValPair<int, double>();
            MaxDisVP.val1 = intStart;
            MaxDisVP.val2 = 0;

            for (int i = intStart; i <= intEnd; i++)
            {
                double dblDis = cedge.QueryPtHeight(cptlt[i]);
                if (dblDis > MaxDisVP.val2)
                {
                    MaxDisVP.val1 = i;
                    MaxDisVP.val2 = dblDis;
                }
            }
            return MaxDisVP;
        }


    }
}
