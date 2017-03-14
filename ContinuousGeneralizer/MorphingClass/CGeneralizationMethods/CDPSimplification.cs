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
using MorphingClass.CCorrepondObjects;


namespace MorphingClass.CGeneralizationMethods
{
    /// <summary>
    /// 
    /// </summary>
    /// <remarks>Douglas-Peucker线综合算法</remarks>
    public class CDPSimplification : CMorphingBaseCpl
    {
      //510546

        Stopwatch _pStopwatch = new Stopwatch();
        public CDPSimplification()
        {

        }

        public CDPSimplification(List<CPolyline> pCPlLt)
        {
            _CPlLt = pCPlLt;
            DivideByDP(pCPlLt);
        }

        public CDPSimplification(CParameterInitialize ParameterInitialize)
        {
            Construct<CPolyline, CPolyline>(ParameterInitialize, 1);
            _CPlLt = this.ObjCGeoLtLt[0].AsExpectedClass<CPolyline, object>().ToList();
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
        /// DPSimplification
        /// </summary>
        /// <remarks>one of the three parameters should be a real parameter, and the others are -1</remarks>
        public void DPSimplification(double dblThresholdDis, double dblRemainPointsRatio, double dblRemainPoints, double dblDeleteNum)
        {
            int intRemainPoints = Convert.ToInt16(dblRemainPoints);
            List<CPolyline> cpllt = _CPlLt;
            //CGeometricMethods.CalDistanceParameters<CPolyline, CPolyline>(cpllt);


            _pStopwatch.Start();
            //get threshold
            double dblTDis = CalThresholdDis(dblThresholdDis, dblRemainPointsRatio, dblRemainPoints, dblDeleteNum);

            List<CPolyline> newcpllt = new List<CPolyline>(cpllt.Count);  // the capacity is not so good
            for (int i = 0; i < cpllt.Count; i++)
            {
                Console.WriteLine("polyline number: " + i);
                List<CPoint> cptlt = cpllt[i].CptLt;
                var firstcpt = cptlt.GetFirstT();
                var last_cpt = cptlt.GetLast_T();

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
        private double CalThresholdDis(double dblThresholdDis, double dblRemainPointsRatio, double dblRemainPoints, double dblDeleteNum)
        {
            int intRemainPoints = Convert.ToInt16(dblRemainPoints);

            //get threshold
            double dblTDis = 0;
            if (dblThresholdDis != -1 && dblRemainPointsRatio == -1 && intRemainPoints == -1 && dblDeleteNum == -1)  // By ThresholdDis
            {
                dblTDis = dblThresholdDis;
            }
            else
            {
                if (dblThresholdDis == -1 && dblRemainPointsRatio == -1 && intRemainPoints == -1 && dblDeleteNum != -1)
                {
                    
                }
                int intInnerPtNum = 0;
                List<CPolyline> cptllt = _CPlLt;
                List<CPoint> EndPtLt = new List<CPoint>(cptllt.Count * 2);
                for (int i = 0; i < cptllt.Count; i++)
                {
                    intInnerPtNum += (cptllt[i].CptLt.Count - 2);

                    EndPtLt.Add(cptllt[i].CptLt[0]);
                    EndPtLt.Add(cptllt[i].CptLt[cptllt[i].CptLt.Count - 1]);
                }

                C5.LinkedList<CCorrCpts> CorrCptsLt = CGeometricMethods.LookingForNeighboursByGrids(EndPtLt, CConstants.dblVerySmall);
                int intSgIntersection = CGeometricMethods.GetNumofIntersections(CorrCptsLt);
                int intAloneEnds = CGeometricMethods.GetNumofAloneEnds(EndPtLt, CorrCptsLt);
                int intRealPtNum = intInnerPtNum + intSgIntersection + intAloneEnds;

                int intDeletePtNum = -1;
                if (dblThresholdDis == -1 && dblRemainPointsRatio != -1 && intRemainPoints == -1 && dblDeleteNum == -1)  // By RemainPointsRatio
                {
                    intDeletePtNum = Convert.ToInt32(intRealPtNum * (1 - dblRemainPointsRatio));
                }
                else if (dblThresholdDis == -1 && dblRemainPointsRatio == -1 && intRemainPoints != -1 && dblDeleteNum == -1)  // By RemainPoints
                {
                    intDeletePtNum = intRealPtNum - intRemainPoints;
                }
                else if (dblThresholdDis == -1 && dblRemainPointsRatio == -1 && intRemainPoints == -1 && dblDeleteNum != -1)  // By RemainPoints
                {
                    intDeletePtNum = Convert.ToInt32(dblDeleteNum);
                } 
                else
                {
                    throw new ArgumentOutOfRangeException("invalid parameters!");
                }

                dblTDis = CalTDisByDeletePtNum(cptllt, intInnerPtNum, intDeletePtNum);
                //GetTDis(intDeletePtNum, dblMaxDisLt);
            }
            return dblTDis;
        }

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
                    for (int i = subVtPl.intFrID + 1; i < subVtPl.intToID; i++)
                    {
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



            ////找到距离基础边最远的点
            ////CEdge pEdge = new CEdge(dcptlt[pVtPl.intFrID], dcptlt[pVtPl.intToID]);            
            //double dblMaxDis = -1;
            //int intMaxDisID = -1;
            //double dblFromDis = 0;
            //for (int i = pVtPl.intFrID + 1; i < pVtPl.intToID; i++)
            //{
            //    dblFromDis = pVtPl.pBaseLine.QueryPtHeight(dcptlt[i]);
            //    if (dblFromDis > dblMaxDis)
            //    {
            //        dblMaxDis = dblFromDis;
            //        intMaxDisID = i;
            //    }
            //}
            //pVtPl.pBaseLine = null; //to use as little memory as possible

            ////分别对左右子边执行分割操作
            //pVtPl.intMaxDisID = intMaxDisID;
            //pVtPl.dblMaxDis = dblMaxDis;
            //pVtPl.DivideByID(intMaxDisID);

            ////move upward this part, set a maxdis for all sub branches*************************************************************************
            //DivideCplByDP(dcpl, pVtPl.CLeftPolyline);
            //DivideCplByDP(dcpl, pVtPl.CRightPolyline);
        }

        private double CalTDisByDeletePtNum<T>(List<T> CPlLt, int intInnerPtNum, int intDeletePtNum) where T : CPolyline
        {
            var dblMaxDisLt = new List<double>(intInnerPtNum);
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
        /// <remarks>if the height of a point is smaller than the height of a lower-level height, the larger lower-level will be used</remarks>
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

                    //double dblMaxDis = Math.Max(currentVtpl.dblMaxDis, Math.Max(currentVtpl.CLeftPolyline.dblMaxDisLargerThanChildren, currentVtpl.CRightPolyline.dblMaxDisLargerThanChildren));  //get the max distance by considering lower-level
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

        private void RecursivelyGetNewCptLtPG(CPolyline cpl, CVirtualPolyline pVtPl, ref List<CPoint> newcptlt, double dblTDis, int intDepth)
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
                dblTDis = dblMaxDisLt.GetLast_T() + CConstants.dblVerySmall;
            }
            else
            {
                //we should find a value larger than dblMaxDisLt[intDeletePtNum-1], so that we can delete at least intDeletePtNum points
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
                    dblTDis = dblMaxDisLt[intDeletePtNum - 1] + CConstants.dblVerySmall;
                }
            }
            return dblTDis;
        }

        #region DPMorph
        public void DPMorph(double dblThresholdDis, double dblRemainPointsRatio, double dblRemainPoints, double dblPropotion)
        {
            //int intRemainPoints = Convert.ToInt16(dblRemainPoints);
            //List<CPolyline> cptllt = _CPlLt;
            //CGeometricMethods.CalDistanceParameters(cptllt);

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
            CPoint targetcpt = CGeometricMethods.GetInbetweenCpt(frcpt, tocpt, dblRatio, -1);
            pVtPl.dblRatioforMovePt = dblRatio;
            pVtPl.dblLengthforMovePt = CGeometricMethods.CalDis(targetcpt, cpt);
            pVtPl.dblAngleDiffforMovePt = CGeometricMethods.CalAngle_Counterclockwise(cpt, targetcpt, tocpt);
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
        private void RecursivelyMovePt(CPolyline cpl, CVirtualPolyline pVtPl, ref CEdge newBaseLine, ref List<CPoint> newcptlt, double dblPropotion)
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
                    newcpt = newBaseLine.QueryMovedPt(pVtPl.dblRatioforMovePt, pVtPl.dblLengthforMovePt, pVtPl.dblAngleDiffforMovePt, dblPropotion, cpl.CptLt[pVtPl.intMaxDisID].ID);
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

        


       

        /// <summary>属性：处理结果</summary>
        public CParameterResult ParameterResult
        {
            get { return _ParameterResult; }
            set { _ParameterResult = value; }
        }
    }
}
