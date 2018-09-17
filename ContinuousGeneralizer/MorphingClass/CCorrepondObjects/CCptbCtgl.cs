using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Windows.Forms;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Carto;

using MorphingClass.CUtility;
using MorphingClass.CGeometry;
using MorphingClass.CGeometry.CGeometryBase;

namespace MorphingClass.CCorrepondObjects
{
    public class CCptbCtgl : CBasicBase  //CCompatibleTriangulation
    {
        private IEnvelope _pEnvRgl;  //specify a place to present the regular polygon
        private CTriangulation _FrCtgl;
        private CTriangulation _ToCtgl;

        public bool blnSave { set; get; }  //try to change the value in CCGABM.cs

        //private CDCEL _FrRglDCEL;

        //public CCptbCtgl(CTriangulation frctgl, CTriangulation toctgl, int intID = -1)
        //{
        //    _FrCtgl = frctgl;
        //    _ToCtgl = toctgl;

        //    _intID = intID;
        //}


        /// <summary>
        ///  
        /// </summary>
        /// <param name="frcpg">counter clockwise</param>
        /// <param name="tocpg">counter clockwise</param>
        /// <param name="blnMaxCommonChords"></param>
        /// <param name="blnSave"></param>
        /// <param name="intID"></param>
        public CCptbCtgl(CPolygon frcpg, CPolygon tocpg, bool blnMaxCommonChords = true, bool fblnSave = false, int intID = -1)
        {
            this.blnSave = fblnSave;

            frcpg.JudgeAndFormCEdgeLt();
            tocpg.JudgeAndFormCEdgeLt();
            frcpg.JudgeAndSetPolygon();
            IEnvelope ipgEnv = frcpg.pPolygon.Envelope;
            double dblMoveHight = 2 * ipgEnv.Height;
            _pEnvRgl = new EnvelopeClass();
            _pEnvRgl.PutCoords(ipgEnv.XMin, ipgEnv.YMin + dblMoveHight, ipgEnv.XMax, ipgEnv.YMax + dblMoveHight);

            var FrConstructingCEdgeLt = new List<CEdge>(frcpg.CEdgeLt);
            var ToConstructingCEdgeLt = new List<CEdge>(tocpg.CEdgeLt);

            List<CPolyline> FrConstraintCplLt = null;
            List<CPolyline> ToConstraintCplLt = null;
            if (blnMaxCommonChords==true)
            {
                List<CEdge> FrChordCEdgelt;
                List<CEdge> ToChordCEdgelt;
                SeparateCpgsByDynamic(frcpg, tocpg, out FrChordCEdgelt, out ToChordCEdgelt);

                FrConstraintCplLt = CHelpFunc.GenerateCplLtByCEdgeLt(FrChordCEdgelt);
                ToConstraintCplLt = CHelpFunc.GenerateCplLtByCEdgeLt(ToChordCEdgelt);

                FrConstructingCEdgeLt.AddRange(FrChordCEdgelt);
                ToConstructingCEdgeLt.AddRange(ToChordCEdgelt);
            }


            _FrCtgl = new CTriangulation(frcpg);
            _FrCtgl.Triangulate(FrConstraintCplLt, FrConstructingCEdgeLt, "Fr", fblnSave);
            //_FrCtgl.ConstructingCEdgeLt= FrConstructingCEdgeLt;

            _ToCtgl = new CTriangulation(tocpg);
            _ToCtgl.Triangulate(ToConstraintCplLt, ToConstructingCEdgeLt, "To", fblnSave);
            //_ToCtgl.ConstructingCEdgeLt = ToConstructingCEdgeLt;

            

            _intID = intID;
        }

        
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>if we use the information of CoStartEdges from triangulation, then we may construct DCEL faster. 
        /// but currently i have not done that, the implementation would not imrpove the order time</remarks>
        public void ConstructCcptbCtgl()
        {
            CTriangulation frctgl = _FrCtgl;
            CTriangulation toctgl = _ToCtgl;
            bool fblnSave = this.blnSave;
            var intRglExteriorEdgeNum = frctgl.CptLt.Count;

            CDCEL FrRglDCEL;
            CDCEL ToRglDCEL;
            ConstructRglDCEL(frctgl, toctgl, out FrRglDCEL, out ToRglDCEL, _pEnvRgl, fblnSave);

            //in FrRglDCEL, cptlt, cpt.AxisAngleCEdgeLt, HalfEdgeLt and CEdgelt has been updated. 
            //FaceCpgLt should not be used before updating
            CombineAndTriangulateDCEL(ref FrRglDCEL, ref ToRglDCEL, intRglExteriorEdgeNum, fblnSave);


            RefineOriginalCtgl(frctgl, FrRglDCEL, CEnumScale.Larger);    //CptLt, CEdgeLt, HalfEdgeLt, FaceCpgLt are updated
            RefineOriginalCtgl(toctgl, FrRglDCEL, CEnumScale.Smaller);   //CptLt, CEdgeLt, HalfEdgeLt, FaceCpgLt are updated

            if (fblnSave == true)
            {
                CSaveFeature.SaveCEdgeEb(frctgl.CEdgeLt, "CompatibleFrCtgl", blnVisible: false);
                CSaveFeature.SaveCptEb(frctgl.CptLt, "CompatibleFrCpt", blnVisible: false);

                CSaveFeature.SaveCEdgeEb(toctgl.CEdgeLt, "CompatibleToCtgl", blnVisible: false);
                CSaveFeature.SaveCptEb(toctgl.CptLt, "CompatibleToCpt", blnVisible: false);
            }
        }

        private void ConstructRglDCEL(CTriangulation frctgl, CTriangulation toctgl,
            out CDCEL FrRglDCEL, out CDCEL ToRglDCEL, IEnvelope pEnvRgl, bool blnSave = false)
        {
            if (frctgl.pTinAdvanced2.DataNodeCount != toctgl.pTinAdvanced2.DataNodeCount)
            {
                throw new ArgumentException("The numbers of points are not the same!");
            }

            FrRglDCEL = ConstructRglDCEL(frctgl, pEnvRgl, CEnumScale.Larger, blnSave);
            //note that we use the same Extent with the Fr one
            ToRglDCEL = ConstructRglDCEL(toctgl, pEnvRgl, CEnumScale.Smaller, blnSave);

        }


        private CDCEL ConstructRglDCEL(CTriangulation ctgl, IEnvelope pEnv, CEnumScale enumScale, bool blnSave = false)
        {
            //ctgl.cpg
            //var realIndCEdgelt = ctgl.GenerateRealTinCEdgeLt(ctgl.ConstructingCEdgeLt);
            var realIndCEdgelt = ctgl.CEdgeLt;
            if (blnSave == true)
            {
                CSaveFeature.SaveCEdgeEb(realIndCEdgelt, "ActualIndCDT" + enumScale.ToString(), blnVisible: false);
                CSaveFeature.SaveCptEb(ctgl.CptLt, "ActualCpt" + enumScale.ToString(), blnVisible: false);
            }

            //RglCpg is counter clockwise
            var RglCpg = CGeoFunc.CreateRegularCpg(pEnv, ctgl.pTinAdvanced2.DataNodeCount,false);

            //rglcpg.CEdgeLt[0].PrintMySelf();
            //rglcpg.CptLt [0].PrintMySelf();
            //rglcpg.CptLt[1].PrintMySelf();
            RglCpg.CptLt.SetIndexID();
            var RglCEdgeLt = new List<CEdge>(ctgl.pTinAdvanced2.DataEdgeCount);

            foreach (var realcedge in realIndCEdgelt)
            {
                realcedge.enumScale = enumScale;   //set the scale of the original edge
                CEdge rglcedge = new CEdge(RglCpg.CptLt[realcedge.FrCpt.indexID], RglCpg.CptLt[realcedge.ToCpt.indexID]);

                //"rglcedge" and "originalcedge" have the same direction 
                //why do we set parent edge----------------------------------
                rglcedge.ParentCEdge = realcedge;  
                realcedge.CorrRglCEdge = rglcedge;
                RglCEdgeLt.Add(rglcedge);
            }

            if (blnSave == true)
            {
                CSaveFeature.SaveCEdgeEb(RglCEdgeLt, "RglIndCDT" + _intID + enumScale.ToString(), blnVisible: false);
                CSaveFeature.SaveCptEb(RglCpg.CptLt, "RglCpt" + enumScale.ToString(), blnVisible: false);
            }


            CDCEL pDCEL = new CDCEL(RglCEdgeLt);
            pDCEL.ConstructDCEL();

            foreach (var cedge in pDCEL.HalfEdgeLt)
            {
                if (cedge.ParentCEdge == null)
                {
                    cedge.ParentCEdge = cedge.cedgeTwin.ParentCEdge;
                }
            }

            return pDCEL;
        }

        #region CombineAndTriangulateDCEL

        private void CombineAndTriangulateDCEL(ref CDCEL FrRglDCEL, ref CDCEL ToRglDCEL, 
            int intRglExteriorEdgeNum, bool blnSave = false)
        {
            //CombineCoStartEdges(ref FrRglDCEL,ref ToRglDCEL );
            //set tempID to identify the same edges, which is faster than comparing coordinates of the ends of edges 

            InsertCEdgeBySmallersIntoFrRglDCEL(ref FrRglDCEL, ref ToRglDCEL);
            UpdateCEdgeLtAndHalfCEdgeLt_AroundCpts(ref FrRglDCEL, intRglExteriorEdgeNum, blnSave);
            //we do this before we triangulate all the faces, 
            //because we just want to know the breaks of the original edges.
            GetCorrRglSubCEdgeLtForParentCEdge(ref FrRglDCEL);
            if (blnSave == true)
            {
                CSaveFeature.SaveCptEb(FrRglDCEL.CptLt, "OverlapRglCDTCpt", blnVisible: false);
                //CSaveFeature.SaveCEdgeEb(FrRglDCEL.CEdgeLt, "OverlapRglCDT", blnVisible: false);
            }


            TriangulateFacesDelaunay(ref FrRglDCEL, intRglExteriorEdgeNum, blnSave);
            

            //TriangulateFaces(ref FrRglDCEL);
            UpdateCEdgeLtAndHalfCEdgeLt_AroundCpts(ref FrRglDCEL, intRglExteriorEdgeNum, blnSave);

            if (blnSave == true)
            {
                //CSaveFeature.SaveCptEb(FrRglDCEL.CptLt, "OverlapRglCDTCpt");
                CSaveFeature.SaveCEdgeEb(FrRglDCEL.CEdgeLt, "TriangulatedCombinedDCEL", blnVisible: false);
            }
        }

        #region InsertCEdgeBySmallersIntoFrRglDCEL
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FrRglDCEL"></param>
        /// <param name="ToRglDCEL"></param>
        /// <remarks>FrRglDCEL.CptLt is updated at the end of this function</remarks>
        private void InsertCEdgeBySmallersIntoFrRglDCEL(ref CDCEL FrRglDCEL, ref CDCEL ToRglDCEL)
        {
            var frcptEt = FrRglDCEL.CptLt.GetEnumerator();
            var tocptEt = ToRglDCEL.CptLt.GetEnumerator();
            var IntersectCptLt = new List<CPoint>();
            int indexID = FrRglDCEL.CptLt.Count;


            while (frcptEt.MoveNext() && tocptEt.MoveNext())
            {
                var CoStartCpt = frcptEt.Current;
                var FrIncidentCEdge = frcptEt.Current.IncidentCEdge;
                var ToIncidentCEdge = tocptEt.Current.IncidentCEdge;                

                //FrIncidentCEdge.PrintMySelf();
                //ToIncidentCEdge.PrintMySelf();
                
                var FrLastCEdge = FrIncidentCEdge.GetSmallerAxisAngleCEdge();
                var ToLastCEdge = ToIncidentCEdge.GetSmallerAxisAngleCEdge();

                //take care of the first ToIncidentCEdge
                CEdge FrStopCEdge = FrIncidentCEdge;
                CEdge ToStopCEdge = ToIncidentCEdge;

                var SmallerAxisAngleCEdge = FrIncidentCEdge;
                // we can compare directly because the two regular polygons have the same coordiantes
                if (ToIncidentCEdge.dblAxisAngle < FrIncidentCEdge.dblAxisAngle)  
                {
                    //ToCurrentCEdge.PrintMySelf();
                    //FrCurrentCEdge.PrintMySelf();

                    //in this case we will insert ToIncidentCEdge into the DCEL, 
                    //then FrLastCEdge.GetLargerAxisAngleCEdge().dblAxisAngle == ToIncidentCEdge.dblAxisAngle
                    FrStopCEdge = ToIncidentCEdge;  
                    SmallerAxisAngleCEdge = FrLastCEdge;
                }
                else if (ToIncidentCEdge.dblAxisAngle > FrIncidentCEdge.dblAxisAngle)
                {
                    //FrStopCEdge = FrIncidentCEdge;
                }
                else //if (ToCurrentCEdge.dblAxisAngle == FrCurrentCEdge.dblAxisAngle)
                {
                    //FrStopCEdge = FrIncidentCEdge;
                }

                var FrCurrentCEdge = FrIncidentCEdge;
                var ToCurrentCEdge = ToIncidentCEdge;
                //var SmallerAxisAngleCEdge = FrIncidentCEdge.GetSmallerAxisAngleCEdge();

                //We will use "IncidentCEdge.dblAxisAngle == CurrentCEdge.dblAxisAngle" to know 
                //whether the do-while loop should be stopped. But at the beginning, IncidentCEdge == CurrentCEdge, 
                //so we should make sure that CurrentCEdge is at least changed once
                bool blnToCurrentCEdgeChanged = false;

                //We will use "IncidentCEdge.dblAxisAngle == CurrentCEdge.dblAxisAngle" to know 
                //whether the do-while loop should be stopped. But at the beginning, IncidentCEdge == CurrentCEdge, 
                //so we should make sure that CurrentCEdge is at least changed once
                bool blnFrCurrentCEdgeChanged = false;  

                do
                {
                    // we can compare directly because the two regular polygons have the same coordiantes
                    if (ToCurrentCEdge.dblAxisAngle < FrCurrentCEdge.dblAxisAngle)
                    {
                        ToCurrentCEdge = InsertCEdge(ToCurrentCEdge, ref SmallerAxisAngleCEdge, ref IntersectCptLt, ref indexID);
                        blnToCurrentCEdgeChanged = true;
                    }
                    else if (ToCurrentCEdge.dblAxisAngle > FrCurrentCEdge.dblAxisAngle)
                    {
                        SmallerAxisAngleCEdge = FrCurrentCEdge;
                        FrCurrentCEdge = FrCurrentCEdge.GetLargerAxisAngleCEdge();

                        blnFrCurrentCEdgeChanged = true;
                    }
                    else //if (ToCurrentCEdge.dblAxisAngle == FrCurrentCEdge.dblAxisAngle)
                    {
                        SmallerAxisAngleCEdge = FrCurrentCEdge;

                        FrCurrentCEdge = FrCurrentCEdge.GetLargerAxisAngleCEdge();
                        ToCurrentCEdge = ToCurrentCEdge.GetLargerAxisAngleCEdge();


                        blnToCurrentCEdgeChanged = true;
                        blnFrCurrentCEdgeChanged = true;
                    }

                    //if ToCurrentCEdge or FrCurrentCEdge reaches the StopCEdge, then the do-while loop is done.   
                    //we can use "==" directly because we didn't recompute the axis angle, instead we assign it
                } while (((FrCurrentCEdge.dblAxisAngle == FrStopCEdge.dblAxisAngle && blnFrCurrentCEdgeChanged == true)  
                       || (ToCurrentCEdge.dblAxisAngle == ToStopCEdge.dblAxisAngle && blnToCurrentCEdgeChanged == true))
                       == false);
                //at the beginning, if ToCurrentCEdge.dblAxisAngle > FrCurrentCEdge.dblAxisAngle, 
                //then ToCurrentCEdge is still the ToIncidentCEdge, 
                //so CCmpMethods.Cmp(ToIncidentCEdge.dblAxisAngle, ToCurrentCEdge.dblAxisAngle) == 0

                //insert the residual to-edges. the residual to-edges are thoses edges have axis angles larger than all the from-edges
                //we have to consider that ToCurrentCEdge may not be ToIncidentCEdge
                if (ToCurrentCEdge.dblAxisAngle != ToStopCEdge.dblAxisAngle)   
                {
                    //in this case, the reason that the previous do-while loop stopped is that 
                    //FrCurrentCEdge.dblAxisAngle == FrStopCEdge.dblAxisAngle. 
                    //Note that, SmallerAxisAngleCEdge == FrStopCEdge.GetSmallerAxisAngleCEdge()
                    //SmallerAxisAngleCEdge = SmallerAxisAngleCEdge.GetLargerAxisAngleCEdge();
                    do
                    {
                        ToCurrentCEdge = InsertCEdge(ToCurrentCEdge, ref SmallerAxisAngleCEdge, ref IntersectCptLt, ref indexID);
                    } while (ToIncidentCEdge.dblAxisAngle != ToCurrentCEdge.dblAxisAngle);
                }

            }
            FrRglDCEL.CptLt.AddRange(IntersectCptLt);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ToCurrentCEdge">this is the edge to be inserted</param>
        /// <param name="SmallerAxisAngleCEdge"></param>
        /// <param name="IntersectCptLt"></param>
        /// <param name="indexID"></param>
        /// <returns></returns>
        private CEdge InsertCEdge(CEdge ToCurrentCEdge, ref CEdge SmallerAxisAngleCEdge, 
            ref List<CPoint> IntersectCptLt, ref int indexID)
        {
            //we have to do this first, because ToCurrentCEdge.GetLargerAxisAngleCEdge() will be changed by inserting
            var NextToCurrentCEdge = ToCurrentCEdge.GetLargerAxisAngleCEdge();

            //the reason we create a new edge is that if we use this original ToCurrentCEdge, 
            //the CDCEL relationship of this edge will be changed. 
            //In this case, when we traverse around ToCurrentCEdge.ToCpt, 
            //we cannot trace the to-edges coming after ToCurrentCEdge 
            var newToCurrentCEdge = CreateNewCEdge(ToCurrentCEdge, ToCurrentCEdge.FrCpt, ToCurrentCEdge.ToCpt);  
            CDCEL.InsertCEdgeBySmaller(SmallerAxisAngleCEdge, newToCurrentCEdge);
            //ShowEdgeRelationship(newToCurrentCEdge);


            TraverseDCEL(newToCurrentCEdge, ref IntersectCptLt, ref indexID);
            //SmallerAxisAngleCEdge = newToCurrentCEdge cannot be used because ToCurrentCEdge may be split, 
            //and we need the split sub cedge to be inserted
            SmallerAxisAngleCEdge = SmallerAxisAngleCEdge.GetLargerAxisAngleCEdge();   
            return NextToCurrentCEdge;
        }

        private void TraverseDCEL(CEdge cedge, ref List<CPoint> IntersectCptLt, ref int indexID)
        {
            //int intFrindexID = cedge.FrCpt.indexID;
            //int intToindexID = cedge.ToCpt.indexID;

            CEdge subcedge = cedge;
            bool blnToTo = false;
            CEdge currentcedge = subcedge.cedgeTwin.cedgeNext.cedgeNext;
            do
            {
                var pIntersection = subcedge.IntersectWith(currentcedge);
                //subcedge.ToCpt.PrintMySelf();
                //currentcedge.ToCpt.PrintMySelf();

                switch (pIntersection.enumIntersectionType)
                {
                    case CEnumIntersectionType.InIn:
                        //Console.WriteLine("****************start intersection*******************");
                        //Console.WriteLine("Edge1:  " + intsubcedgeFrindexID + "   " + intsubcedgeToindexID);
                        //Console.WriteLine("Edge2:  " + intFrCurrentindexID + "   " + intToCurrentindexID);
                        //Console.WriteLine("Intersection:  " + indexID);
                        //Console.WriteLine("****************end intersection*******************");

                        //pIntersection.IntersectCpt.PrintMySelf();
                        pIntersection.IntersectCpt.indexID = indexID++;
                        pIntersection.IntersectCpt.isSteiner = true;
                        IntersectCptLt.Add(pIntersection.IntersectCpt);
                        subcedge = HandleInIn(pIntersection);
                        currentcedge = subcedge.cedgeTwin.cedgeNext.cedgeNext;
                        break;
                    case CEnumIntersectionType.ToTo:
                        CDCEL.InsertCEdgeBySmaller(currentcedge.cedgeNext, subcedge.cedgeTwin);
                        //ShowEdgeRelationship(subcedge.cedgeTwin);
                        blnToTo = true;
                        break;
                    case CEnumIntersectionType.NoNo:
                        currentcedge = currentcedge.cedgeNext;
                        break;
                    default:
                        Console.WriteLine("-----------------------Intersection--------------------------");
                        subcedge.PrintMySelf();
                        currentcedge.PrintMySelf();
                        pIntersection.IntersectCpt.PrintMySelf();

                        var  dblAngleLt =new List <double > ();
                        var test=currentcedge;
                        do
                        {
                            dblAngleLt.Add(test.dblAxisAngle);
                            test = test.GetLargerAxisAngleCEdge();
                        } while (test.GID !=currentcedge.GID );

                        

                        //Console.WriteLine("subcedge    :" + "FrCpt:  " + subcedge.FrCpt.X + "  " + subcedge.FrCpt.Y 
                        //+ ";       ToCpt:  " + subcedge.ToCpt.X + "  " + subcedge.ToCpt.Y);
                        //Console.WriteLine("currentcedge:" + "FrCpt:  " + currentcedge.FrCpt.X + "  " + currentcedge.FrCpt.Y 
                        //+ ";       ToCpt:  " + currentcedge.ToCpt.X + "  " + currentcedge.ToCpt.Y);
                        //Console.WriteLine("Intersection:" + pIntersection.IntersectCpt.X + "  " + pIntersection.IntersectCpt.Y);
                        Console.WriteLine("---------------  --------End-------------  -------------");
                        //MessageBox.Show("Impossible case in: " + this.ToString() + ".cs   " + "TraverseDCEL");
                        throw new ArgumentException("Impossible case!");

                }
            } while (blnToTo == false);
        }

        private CEdge HandleInIn(CIntersection pIntersection)
        {
            CEdge newPreCEdge, newSucCEdge, currentPreCEdge, currentSucCEdge;  //these four edges have the same From vertex
            CreateEdgesAndReplace(pIntersection.CEdge1, pIntersection.IntersectCpt, out newPreCEdge, out newSucCEdge);
            CreateEdgesAndReplace(pIntersection.CEdge2, pIntersection.IntersectCpt, out currentPreCEdge, out currentSucCEdge);

            CDCEL.InsertCEdgeBySmaller(newSucCEdge, currentSucCEdge);
            CDCEL.InsertCEdgeBySmaller(currentSucCEdge, newPreCEdge);
            CDCEL.InsertCEdgeBySmaller(newPreCEdge, currentPreCEdge);

            var MinAxisAngleCEdge = FindMinAxisAngleCEdgeAroundCpt(newSucCEdge);
            MinAxisAngleCEdge.isIncidentCEdgeForCpt = true;
            pIntersection.IntersectCpt.IncidentCEdge = MinAxisAngleCEdge;
            return newSucCEdge;
        }





        private void CreateEdgesAndReplace(CEdge originalcedge, CPoint breakcpt, out CEdge precedge, out CEdge succedge)
        {
            precedge = CreateNewCEdge(originalcedge, originalcedge.FrCpt, breakcpt).cedgeTwin;
            succedge = CreateNewCEdge(originalcedge, breakcpt, originalcedge.ToCpt);

            CDCEL.ReplaceCEdge(originalcedge, precedge.cedgeTwin);
            CDCEL.ReplaceCEdge(originalcedge.cedgeTwin, succedge.cedgeTwin);
        }


        private CEdge CreateNewCEdge(CEdge originalcedge, CPoint frcpt, CPoint tocpt)
        {
            var cedge = new CEdge(frcpt, tocpt);
            //we set the AxisAngle because we may want to maintain the AxisAngleCEdgeLt of a vertex
            cedge.dblAxisAngle = originalcedge.dblAxisAngle;  
            cedge.CreateTwinCEdge();
            cedge.ParentCEdge = originalcedge.ParentCEdge;
            cedge.cedgeTwin.ParentCEdge = originalcedge.ParentCEdge;
            return cedge;
        }

        private CEdge FindMinAxisAngleCEdgeAroundCpt(CEdge cedge)
        {
            CEdge MinAxisAngleCEdge = cedge;
            CEdge currentcedge = cedge.GetLargerAxisAngleCEdge();
            while (cedge.dblAxisAngle != currentcedge.dblAxisAngle)
            {
                if (currentcedge.dblAxisAngle < MinAxisAngleCEdge.dblAxisAngle)
                {
                    MinAxisAngleCEdge = currentcedge;

                    //notice that we traverse along an angle-increasing direction, 
                    //so if we find an edge with a smaller AxisAngle, 
                    //then this edge must be real MinAxisAngleCEdge
                    break;  
                }
                currentcedge = currentcedge.GetLargerAxisAngleCEdge();
            }
            return MinAxisAngleCEdge;
        }

        #endregion

        private void GetCorrRglSubCEdgeLtForParentCEdge(ref CDCEL FrRglDCEL)
        {
            //for each edge, we test whether it is the first sub edge of its parent edge. 
            //If so, we can find all the sub edges of a parent edge by traversing along the direction
            foreach (var cpt in FrRglDCEL.CptLt)
            {
                //find the edge with min axisangle
                var incidentcedge = cpt.IncidentCEdge;
                var CoStartCEdge = incidentcedge;

                do
                {
                    var ParentCEdge = CoStartCEdge.ParentCEdge;
                    //we cannot compare coordinates or GIDs here 
                    //because cedge is from regular polygon while ParentCEdge is from real polygon
                    if (CoStartCEdge.FrCpt.indexID == ParentCEdge.FrCpt.indexID)  
                    {
                        var SubCEdge = CoStartCEdge;
                        ParentCEdge.CorrRglSubCEdgeLt = new List<CEdge>(); //at least one sub edge
                        do
                        {
                            SubCEdge.SetLength();   //set length
                            ParentCEdge.CorrRglSubCEdgeLt.Add(SubCEdge);

                            //we know that only four edges share an intersection, resulted by the intersection of two edges
                            SubCEdge = SubCEdge.cedgeNext.cedgeTwin.cedgeNext;  
                        } while (SubCEdge.FrCpt.isSteiner == true);

                        //notice that an edge from toctgl of a pair of corresponding edges was removed before, 
                        //therefore we didn't refer to the parent edge of removed edge. 
                        //as a result, ParentCEdge.CorrRglSubCEdgeLt of that edge is null. to keep consistent, 
                        //we set ParentCEdge.CorrRglSubCEdgeLt of an edge from frctgl to null if there is only one sub edge
                        if (ParentCEdge.CorrRglSubCEdgeLt.Count < 2)
                        {
                            ParentCEdge.CorrRglSubCEdgeLt = null;
                        }
                    }
                    CoStartCEdge = CoStartCEdge.GetLargerAxisAngleCEdge();
                } while (CoStartCEdge.GID != incidentcedge.GID);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="FrRglDCEL">At this point, there is no proper faces</param>
        /// <param name="intRglExteriorEdgeNum"></param>
        /// <param name="blnSave"></param>
        /// <remarks>We avoid to use ArcEngine because ArcEngine may introduce unnecessary points</remarks>
        private void TriangulateFacesDelaunay(ref CDCEL FrRglDCEL, int intRglExteriorEdgeNum, bool blnSave =false)
        {
            //set the isTraversed attribute to flase
            SetCEdgesIsTraversedFalse_AroundCpts(ref FrRglDCEL);

            //we don't need to triangulate super face. so we set isTraversed of all the outercedge to true
            CEdge outercedge = FrRglDCEL.HalfEdgeLt[1];   //we know that FrRglDCEL.HalfEdgeLt[1] is an outer cedge
            do
            {
                outercedge.isTraversed = true;
                outercedge = outercedge.cedgeNext;
            } while (outercedge.isTraversed == false);

            //triangulate faces
            foreach (var firstCEdge in FrRglDCEL.HalfEdgeLt)
            {
                if (firstCEdge.isTraversed)
                {
                    continue;
                }

                var secondCEdge = firstCEdge.cedgeNext;
                var thirdCEdge = secondCEdge.cedgeNext;
                var fourthCEdge = thirdCEdge.cedgeNext;
                firstCEdge.isTraversed = true;
                secondCEdge.isTraversed = true;
                thirdCEdge.isTraversed = true;
                if (firstCEdge.GID== fourthCEdge.GID)  //this is a triangle
                {
                    continue;
                }
                else if (firstCEdge.GID == fourthCEdge.cedgeNext.GID)  //this is a quadrangle
                {
                    fourthCEdge.isTraversed = true;
                    var DiagonalCEdge = CTriangulation.TriangulateQuadrangle_Delaunay(firstCEdge);
                    DiagonalCEdge.CreateTwinCEdge();
                    if (DiagonalCEdge.FrCpt.GID== firstCEdge.FrCpt.GID)
                    {
                        CDCEL.InsertCEdgeBySmaller(firstCEdge, DiagonalCEdge);
                        CDCEL.InsertCEdgeBySmaller(thirdCEdge, DiagonalCEdge.cedgeTwin);
                    }
                    else //if (DiagonalCEdge.FrCpt.GID== firstCEdge.ToCpt.GID)
                    {
                        CDCEL.InsertCEdgeBySmaller(secondCEdge, DiagonalCEdge);
                        CDCEL.InsertCEdgeBySmaller(fourthCEdge, DiagonalCEdge.cedgeTwin);
                    }
                }
                else //a convex polygon which has at least 5 edges; we take advantage of ArcEngine
                {
                    var newCptLt = new List<CPoint>(5);  //at least five points
                    newCptLt.Add(firstCEdge.FrCpt);
                    var newCEdgeLt = new List<CEdge>(5);

                    var currentCEdge = firstCEdge;
                    do
                    {
                        newCptLt.Add(currentCEdge.ToCpt);
                        newCEdgeLt.Add(currentCEdge);
                        currentCEdge.isTraversed = true;
                        currentCEdge = currentCEdge.cedgeNext;
                    } while (currentCEdge.GID != firstCEdge.GID);

                    var newcpg = new CPolygon(firstCEdge.FrCpt.indexID, newCptLt);
                    var subCtgl = new CTriangulation(newcpg);
                    subCtgl.Triangulate(null, newCEdgeLt, "Combined" + newcpg.ID, blnSave);

                    foreach (var newCEdge in subCtgl.NewCEdgeLt)
                    {
                        newCEdge.CreateTwinCEdge();
                        CDCEL.InsertCEdgeAtCpt(newCEdge.FrCpt, newCEdge);
                        CDCEL.InsertCEdgeAtCpt(newCEdge.ToCpt, newCEdge.cedgeTwin);
                    }
                }
            }













            //foreach (var cpt in FrRglDCEL.CptLt)
            //{
            //    var incidentcedge = cpt.IncidentCEdge;
            //    var CoStartCEdge = incidentcedge;

            //    do
            //    {
            //        if (CoStartCEdge.isTraversed == false)
            //        {
            //            CoStartCEdge.isTraversed = true;
            //            CoStartCEdge.cedgeNext.isTraversed = true;
            //            CoStartCEdge.cedgeNext.cedgeNext.isTraversed = true;

            //            //we define lastPreCEdge so that we will know where we should insert
            //            CEdge SmallerAxisAngleCEdge = CoStartCEdge;
            //            CEdge currentcedge = CoStartCEdge.cedgeNext.cedgeNext;
            //            while (CoStartCEdge.FrCpt.GID != currentcedge.ToCpt.GID)
            //            {
            //                CEdge newcedge = new CEdge(CoStartCEdge.FrCpt, currentcedge.FrCpt);
            //                newcedge.CreateTwinCEdge();
            //                CDCEL.InsertCEdgeBySmaller(SmallerAxisAngleCEdge, newcedge);
            //                CDCEL.InsertCEdgeBySmaller(currentcedge, newcedge.cedgeTwin);

            //                newcedge.isTraversed = false;
            //                newcedge.cedgeTwin.isTraversed = false;
            //                currentcedge.isTraversed = false;

            //                currentcedge = currentcedge.cedgeNext;
            //                SmallerAxisAngleCEdge = newcedge;
            //            }
            //        }

            //        CoStartCEdge = CoStartCEdge.GetLargerAxisAngleCEdge();
            //    } while (CoStartCEdge.GID != incidentcedge.GID);
            //}
            


            //var ExteriorCptLt = new List<CPoint>(FrRglDCEL.CptLt.GetRange(0, intRglExteriorEdgeNum));
            //ExteriorCptLt.Add(FrRglDCEL.CptLt[0]);
            //var ExteriorRglCpg = new CPolygon(-2, ExteriorCptLt);

            //var ctgl = new CTriangulation(ExteriorRglCpg, FrRglDCEL.CptLt);

            //var ExistingInteriorCEdgeLt = FrRglDCEL.CEdgeLt
            //    .GetRange(intRglExteriorEdgeNum, FrRglDCEL.CEdgeLt.Count - intRglExteriorEdgeNum);
            //var ExistingInteriorCplLt = CHelpFunc.GenerateCplLtByCEdgeLt(ExistingInteriorCEdgeLt);


            //ctgl.Triangulate(ExistingInteriorCplLt, FrRglDCEL.CEdgeLt, "Combined", blnSave);

            ////ctgl.GenerateRealTinCEdgeLt(FrRglDCEL.CEdgeLt);

            ////insert new edges into the FrRglDCEL
            //ctgl.CEdgeLt.ForEach(cedge => cedge.isTraversed = false);
            ////after this step, a cedge with cedge.isTraversed = false is a new edge
            //FrRglDCEL.CEdgeLt.ForEach(cedge => cedge.isTraversed = true);

            //foreach (var newCEdge in ctgl.CEdgeLt)
            //{
            //    if (newCEdge.isTraversed==true) //not real new edge
            //    {
            //        continue;
            //    }
            //    newCEdge.isTraversed = true;

            //    newCEdge.CreateTwinCEdge();
            //    CDCEL.InsertCEdgeAtCpt(newCEdge.FrCpt, newCEdge);
            //    CDCEL.InsertCEdgeAtCpt(newCEdge.ToCpt, newCEdge.cedgeTwin);
            //}            
        }




        //private void TriangulateFaces(ref CDCEL FrRglDCEL)
        //{
        //    //set the isTraversed attribute to flase
        //    SetCEdgesIsTraversedFalse_AroundCpts(ref FrRglDCEL);

        //    //we don't need to triangulate super face. so we set isTraversed of all the outercedge to true
        //    CEdge outercedge = FrRglDCEL.HalfEdgeLt[1];   //we know that FrRglDCEL.HalfEdgeLt[1] is an outer cedge
        //    do
        //    {
        //        outercedge.isTraversed = true;
        //        outercedge = outercedge.cedgeNext;
        //    } while (outercedge.isTraversed == false);

        //    //triangulate faces
        //    foreach (var cpt in FrRglDCEL.CptLt)
        //    {
        //        var incidentcedge = cpt.IncidentCEdge;
        //        var CoStartCEdge = incidentcedge;

        //        do
        //        {
        //            if (CoStartCEdge.isTraversed == false)
        //            {
        //                CoStartCEdge.isTraversed = true;
        //                CoStartCEdge.cedgeNext.isTraversed = true;
        //                CoStartCEdge.cedgeNext.cedgeNext.isTraversed = true;

        //                //we define lastPreCEdge so that we will know where we should insert
        //                CEdge SmallerAxisAngleCEdge = CoStartCEdge;  
        //                CEdge currentcedge = CoStartCEdge.cedgeNext.cedgeNext;
        //                while (CoStartCEdge.FrCpt.GID != currentcedge.ToCpt.GID)
        //                {
        //                    CEdge newcedge = new CEdge(CoStartCEdge.FrCpt, currentcedge.FrCpt);
        //                    newcedge.CreateTwinCEdge();
        //                    CDCEL.InsertCEdgeBySmaller(SmallerAxisAngleCEdge, newcedge);
        //                    CDCEL.InsertCEdgeBySmaller(currentcedge, newcedge.cedgeTwin);

        //                    newcedge.isTraversed = false;
        //                    newcedge.cedgeTwin.isTraversed = false;
        //                    currentcedge.isTraversed = false;

        //                    currentcedge = currentcedge.cedgeNext;
        //                    SmallerAxisAngleCEdge = newcedge;
        //                }
        //            }

        //            CoStartCEdge = CoStartCEdge.GetLargerAxisAngleCEdge();
        //        } while (CoStartCEdge.GID != incidentcedge.GID);
        //    }
        //}

        private void UpdateCEdgeLtAndHalfCEdgeLt_AroundCpts(ref CDCEL FrRglDCEL, int intRglExteriorEdgeNum, bool blnSave=false)
        {
            SetCEdgesIsTraversedFalse_AroundCpts(ref FrRglDCEL);
            //the first 2*intRglEdgeNum half edges are maintained
            var HalfCEdgeLt = FrRglDCEL.HalfEdgeLt.GetRange(0, 2 * intRglExteriorEdgeNum); 
            HalfCEdgeLt.ForEach(cedge => cedge.isTraversed = true);
            foreach (var cpt in FrRglDCEL.CptLt)
            {
                var incidentcedge = cpt.IncidentCEdge;
                var CoStartCEdge = incidentcedge;

                do
                {
                    if (CoStartCEdge.isTraversed == false)
                    {
                        HalfCEdgeLt.Add(CoStartCEdge);
                        HalfCEdgeLt.Add(CoStartCEdge.cedgeTwin);

                        CoStartCEdge.isTraversed = true;
                        CoStartCEdge.cedgeTwin.isTraversed = true;
                    }

                    CoStartCEdge = CoStartCEdge.GetLargerAxisAngleCEdge();
                } while (CoStartCEdge.GID != incidentcedge.GID);
            }

            HalfCEdgeLt.SetIndexID();

            FrRglDCEL.HalfEdgeLt = HalfCEdgeLt;            
            FrRglDCEL.UpdateCEdgeLtByHalfCEdgeLt();
        }

        private void SetCEdgesIsTraversedFalse_AroundCpts(ref CDCEL pDCEL)
        {
            //set the isTraversed attribute to flase
            foreach (var cpt in pDCEL.CptLt)
            {
                var incidentcedge = cpt.IncidentCEdge;
                var CoStartCEdge = incidentcedge;

                do
                {
                    CoStartCEdge.isTraversed = false;
                    CoStartCEdge = CoStartCEdge.GetLargerAxisAngleCEdge();
                } while (CoStartCEdge.GID != incidentcedge.GID);
            }
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ctgl"></param>
        /// <param name="FrRglDCEL"></param>
        /// <param name="enumScale"></param>
        /// <remarks >at the beginning of this method, there is not DCEL in ctgl</remarks>
        private void RefineOriginalCtgl(CTriangulation ctgl, CDCEL FrRglDCEL, CEnumScale enumScale)
        {
            //FrRglDCEL.ShowEdgeRelationshipAroundAllCpt();


            //generate new vertices without coordinates.
            //we have to do this because we may refer some vertices we do not know yet
            int intIntersectionNumber = FrRglDCEL.CptLt.Count - ctgl.CptLt.Count;
            var IntersectCptLt = new List<CPoint>(intIntersectionNumber);
            for (int i = 0; i < intIntersectionNumber; i++)
            {
                CPoint cpt = new CPoint();
                cpt.indexID = ctgl.CptLt.Count + i;
                cpt.isSteiner = true;
                IntersectCptLt.Add(cpt);
            }
            ctgl.CptLt.AddRange(IntersectCptLt);

            //set the coordinates
            ctgl.CEdgeLt.ForEach(cedge => cedge.SetLength());
            ctgl.CEdgeLt.ForEach(cedge => cedge.CorrRglCEdge.SetLength());
            foreach (var realcedge in ctgl.CEdgeLt)
            {
                if (realcedge.CorrRglSubCEdgeLt != null)  //this means realcedge.CorrRglSubCEdgeLt.Count > 1
                {
                    double dblRealLength = realcedge.dblLength;
                    double dblRglLength = realcedge.CorrRglCEdge.dblLength;
                    double dblCurrentLenght = 0;
                    for (int i = 0; i < realcedge.CorrRglSubCEdgeLt.Count - 1; i++)
                    {
                        dblCurrentLenght += realcedge.CorrRglSubCEdgeLt[i].dblLength;
                        double dblProportion = dblCurrentLenght / dblRglLength;

                        ctgl.CptLt[realcedge.CorrRglSubCEdgeLt[i].ToCpt.indexID].X =
                            CGeoFunc.GetInbetweenDbl(realcedge.FrCpt.X, realcedge.ToCpt.X, dblProportion);
                        ctgl.CptLt[realcedge.CorrRglSubCEdgeLt[i].ToCpt.indexID].Y =
                            CGeoFunc.GetInbetweenDbl(realcedge.FrCpt.Y, realcedge.ToCpt.Y, dblProportion);
                    }
                }
            }

            //FrRglDCEL .HalfEdgeLt .ForEach(cedge=>cedge .isTraversed =false );
            int intI = 0;
            var FrHalfEdgeLt = FrRglDCEL.HalfEdgeLt;
            var realhalfcedgelt = new List<CEdge>(FrHalfEdgeLt.Count);
            while (intI < FrHalfEdgeLt.Count)
            {
                var frhalfcedge = FrHalfEdgeLt[intI];
                CEdge realcedge = new CEdge(ctgl.CptLt[frhalfcedge.FrCpt.indexID], ctgl.CptLt[frhalfcedge.ToCpt.indexID]);
                realcedge.CreateTwinCEdge();
                realcedge.indexID = intI;
                realcedge.cedgeTwin.indexID = intI + 1;
                realhalfcedgelt.Add(realcedge);
                realhalfcedgelt.Add(realcedge.cedgeTwin);

                intI += 2;
            }


            //construct relationship between edges
            for (int i = 0; i < realhalfcedgelt.Count; i++)
            {
                realhalfcedgelt[i].cedgePrev = realhalfcedgelt[FrHalfEdgeLt[i].cedgePrev.indexID];
                realhalfcedgelt[i].cedgeNext = realhalfcedgelt[FrHalfEdgeLt[i].cedgeNext.indexID];
            }
            ctgl.HalfEdgeLt = realhalfcedgelt;
            ctgl.UpdateCEdgeLtByHalfCEdgeLt();


            var CoStartHalfCEdgeDt = CDCEL.IdentifyCoStartCEdge(realhalfcedgelt);
            CDCEL.OrderCEdgeLtAccordAxisAngle(CoStartHalfCEdgeDt);

            //ctgl.ShowEdgeRelationshipAllCEdges();



            //generate FaceCptLt
            ctgl.ConstructFaceLt();
        }


        #region SeparateCpgsByDynamic
        private void SeparateCpgsByDynamic(CPolygon frcpg, CPolygon tocpg,
            out List<CEdge> FrChordCEdgelt, out List<CEdge> ToChordCEdgelt)
        {
            
            int intCount = frcpg.CptLt.Count - 1;
            var ablnValidChords = GetCoValidChords(frcpg, tocpg);

            var Table = new CTable(intCount, intCount, true);
            var aCell = Table.aCell;


            for (int i = 0; i < intCount - 1; i++)
            {
                int j = i + 1;
                aCell[i, j] = new CCell(i, 1, 0, i, j);
            }

            //the running time is O(n^3)
            //when k=2, we are dealing with triangles
            for (int k = 2; k < intCount; k++)
            {
                for (int i = 0; i < intCount - k; i++)
                {
                    int j = i + k;

                    int intIncrease = 1;
                    if (ablnValidChords[i, j] == false)
                    {
                        intIncrease = 0;
                    }

                    var maxCell = new CCell();
                    for (int l = i + 1; l < j; l++)
                    {
                        double dblCost = aCell[i, l].dblCost + aCell[l, j].dblCost + intIncrease;
                        //when two separations have the same cost, we pick the one which splits the curve more balancedly.
                        //if there are two indexes splitting the same balancedly, we choose the smaller one
                        //e.g., when i=3 and j =10, then the average is 6.5. If both l=5 and l=8 are available, then we pick l=5.
                        double dblCostHelp = -Math.Abs(l - Math.Floor(Convert.ToDouble(i + j) / 2));
                        var newcell = new CCell(l, dblCost, dblCostHelp, i, j);
                        //using GetMaxCell(maxCell, newcell) is much faster 
                        //than using CHelpFunc.Max(maxCell, newcell, cell => cell.dblCost, cell => cell.dblCostHelp);
                        maxCell = GetMaxCell(maxCell, newcell);
                    }
                    aCell[i, j] = maxCell;
                }
            }

            var intCommonNum = Convert.ToInt32(aCell[0, intCount - 1].dblCost);
            //Table.PrintaCell();

            FrChordCEdgelt = GenerateInteriorCplLt(aCell, frcpg.CptLt, ablnValidChords);
            ToChordCEdgelt = GenerateInteriorCplLt(aCell, tocpg.CptLt, ablnValidChords);


            if (this.blnSave == true)
            {
                CSaveFeature.SaveCEdgeEb(FrChordCEdgelt, "CommonChordsFr", blnVisible: false);
                CSaveFeature.SaveCEdgeEb(ToChordCEdgelt, "CommonChordsTo", blnVisible: false);
            }
        }

        private CCell GetMaxCell(CCell T1, CCell T2)
        {
            if (T1.dblCost > T2.dblCost)
            {
                return T1;
            }
            else if (T1.dblCost < T2.dblCost)
            {
                return T2;
            }
            else
            {
                if (T1.dblCostHelp > T2.dblCostHelp)
                {
                    return T1;
                }
                else if (T1.dblCostHelp < T2.dblCostHelp)
                {
                    return T2;
                }
                else
                {
                    return T1;
                }
            }
        }

        private List<CEdge> GenerateInteriorCplLt(CCell[,] aCell, List<CPoint> OriginalCptLt, bool[,] ablnValidChords)
        {
            var intCount = OriginalCptLt.Count - 1; //intCount is the number of vertices without duplicates
            var intCommonNum = Convert.ToInt32(aCell[0, intCount - 1].dblCost);
            var ChordCEdgelt = new List<CEdge>(intCommonNum);

            var CellStack = new Stack<CCell>();
            CellStack.Push(aCell[0, intCount - 1]);
            while (CellStack.Count > 0)
            {
                var currentCell = CellStack.Pop();
                int intIndex = currentCell.intBackK1;
                var backcell1 = aCell[currentCell.intRowIndex, intIndex];
                var backcell2 = aCell[intIndex, currentCell.intColIndex];

                HandleOneCell(backcell2, CellStack, OriginalCptLt, ablnValidChords, ref ChordCEdgelt);
                HandleOneCell(backcell1, CellStack, OriginalCptLt, ablnValidChords, ref ChordCEdgelt);
            }

            return ChordCEdgelt;
        }

        private void HandleOneCell(CCell cell, Stack<CCell> CellStack, List<CPoint> OriginalCptLt,
            bool[,] ablnValidChords, ref List<CEdge> ChordCEdgelt)
        {
            int intIndexDiff = cell.intColIndex - cell.intRowIndex; //it holds cell.intColIndex > cell.intRowIndex
            if (intIndexDiff >= 2)
            {
                if (ablnValidChords[cell.intRowIndex, cell.intColIndex]==true)
                {
                    ChordCEdgelt.Add(new CEdge(OriginalCptLt[cell.intRowIndex], OriginalCptLt[cell.intColIndex]));
                }               

                if (intIndexDiff > 2)
                {
                    CellStack.Push(cell);
                }
            }
        }

        /// <summary>
        /// common valid chords; the original edges of the polygons are also valid
        /// </summary>
        /// <param name="frcpg"></param>
        /// <param name="tocpg"></param>
        /// <returns></returns>
        private bool[,] GetCoValidChords(CPolygon frcpg, CPolygon tocpg)
        {
            var FrValidChords = GetValidChords(frcpg);
            var ToValidChords = GetValidChords(tocpg);

            int intCount = frcpg.CptLt.Count - 1;
            var ablnValidChords = new bool[intCount, intCount];
            for (int i = 0; i < intCount; i++)
            {
                for (int j = 0; j < intCount; j++)
                {
                    ablnValidChords[i, j] = FrValidChords[i, j] && ToValidChords[i, j];
                }
            }

            //ablnValidChords[0, intCount - 1] represents an edge, but not a chord. Therefore,
            ablnValidChords[0, intCount - 1] = true;
            return ablnValidChords;
        }

        private bool[,] GetValidChords(CPolygon cpg)
        {
            var intCptNum = cpg.CptLt.Count - 1;
            var cpgcptlt = cpg.CptLt.GetRange(0, intCptNum);
            cpgcptlt.SetIndexID();

            var ablnValidChords = new bool[intCptNum, intCptNum];
            var fEdgeGrid = new CEdgeGrid(cpg.CEdgeLt);

            var IgnoreCEdgeLt = new List<CEdge>(cpg.CEdgeLt.Count + 1);
            IgnoreCEdgeLt.Add(cpg.CEdgeLt.GetLastT());
            IgnoreCEdgeLt.AddRange(cpg.CEdgeLt);

            cpg.SetAxisAngleAndReverseLt();
            var lastTraversedCEdgeLt = new List<CEdge>();
            var PossibleTouchCEdgeLt = new List<CEdge>();
            for (int i = 0; i < intCptNum - 1; i++)
            {
                lastTraversedCEdgeLt.ForEach(cedge => cedge.isTraversed = false);
                lastTraversedCEdgeLt.Clear();
                //PossibleTouchCEdgeLt.Clear();

                ablnValidChords[i, i + 1] = true;
                var IgnoreCEdgeSS = new SortedSet<CEdge>
                {
                    IgnoreCEdgeLt[i],
                    IgnoreCEdgeLt[i + 1]
                };

                var lastCEdge = cpg.CEdgeLt[i]; //last edge from point i to point j
                lastCEdge.JudgeAndSetAxisAngle();
                CEdge outTouchCEdge = null;
                for (int j = i + 2; j < intCptNum; j++)
                {
                    //this is the default value
                    //ablnValidChords[i, j] = false;

                    var newcedge = new CEdge(cpgcptlt[i], cpgcptlt[j]);
                    newcedge.SetAxisAngle();
                    if (CGeoFunc.IsInbetween_Counterclockwise(cpg.AxisAngleLt[i],
                        newcedge.dblAxisAngle, cpg.ReverseAxisAngleLt[i]) == false)
                    {
                        outTouchCEdge = null;  //this is necessary
                        //the new edge is outside the boundary polygon (cpg)
                        continue;
                    }

                    //if newcedge.dblAxisAngle is inbetween cpg.AxisAngleLt[i] and cpg.ReverseAxisAngleLt[i]:
                    //if ablnValidChords[i, j - 1] == true and the angle from lastCEdge to newcedge is clockwise, then continue;
                    //if ablnValidChords[i, j - 1] == true and the angle from lastCEdge to newcedge is counter clockwise,
                    //  then test touch without taking into account edges in AdvantageTraversedCEdgeLt;
                    //if ablnValidChords[i, j - 1] == false, then we first test edges in 
                    //  PossibleTouchCEdgeLt (if there are elements) and test other edges.

                    //excluding some invalid cases
                    if (ablnValidChords[i, j - 1] == true)  //in this case, we certainly have appropriate lastCEdge
                    {
                        outTouchCEdge = null;
                        double dblAngle = CGeoFunc.CalAngle_Counterclockwise(lastCEdge.dblAxisAngle, newcedge.dblAxisAngle);
                        if (dblAngle == 0 || dblAngle >= Math.PI)
                        {
                            //if dblAngle == 0, lastCEdge and newcedge is on the same line
                            //if dblAngle >= Math.PI newcedge must intersect with at least one edge of the polygon                            
                            continue;
                        }
                        else
                        {
                            var AdvantageTraversedCEdgeLt = new List<CEdge>(lastTraversedCEdgeLt.Count);
                            foreach (var cedge in lastTraversedCEdgeLt)
                            {
                                if (cedge.FrCpt.indexID > i && cedge.FrCpt.indexID < j)
                                {
                                    // we do not need to test this edge later
                                    AdvantageTraversedCEdgeLt.Add(cedge);
                                }
                                else
                                {
                                    cedge.isTraversed = false;
                                }
                            }
                            lastTraversedCEdgeLt = AdvantageTraversedCEdgeLt;
                        }
                    }
                    else //if (ablnValidChords[i, j - 1] == false)
                    {
                        //lastTraversedCEdgeLt may contain some edges from an earlier attempt, 
                        //where ablnValidChords[i, j'] == true
                        lastTraversedCEdgeLt.ForEach(cedge => cedge.isTraversed = false);
                        lastTraversedCEdgeLt.Clear();
                    }


                    //if (ablnValidChords[i, j - 1] == false), then we have to test regularly
                    IgnoreCEdgeSS.Add(IgnoreCEdgeLt[j]);
                    IgnoreCEdgeSS.Add(IgnoreCEdgeLt[j + 1]);

                    if (outTouchCEdge != null)
                    {
                        PossibleTouchCEdgeLt.Clear();
                        PossibleTouchCEdgeLt.Add(outTouchCEdge);
                        PossibleTouchCEdgeLt.Add(IgnoreCEdgeLt[outTouchCEdge.FrCpt.indexID]);  //the edge before
                        PossibleTouchCEdgeLt.Add(IgnoreCEdgeLt[outTouchCEdge.ToCpt.indexID + 1]);  //the edge after
                    }

                    if (fEdgeGrid.BlnTouch(newcedge, lastTraversedCEdgeLt, PossibleTouchCEdgeLt,
                        out outTouchCEdge, IgnoreCEdgeSS) == false)
                    {
                        ablnValidChords[i, j] = true;
                    }
                    else
                    {
                        //Set the TraversedCEdge to false
                        foreach (var TraversedCEdge in lastTraversedCEdgeLt)
                        {
                            TraversedCEdge.isTraversed = false;
                        }
                    }

                    IgnoreCEdgeSS.Remove(IgnoreCEdgeLt[j]);
                    IgnoreCEdgeSS.Remove(IgnoreCEdgeLt[j + 1]);
                    lastCEdge = newcedge;
                }
            }

            //CHelpFunc.PrintArray2D(ablnValidChords, 10);

            return ablnValidChords;
        }
        #endregion


        public CTriangulation FrCtgl
        {
            get { return _FrCtgl; }
            set { _FrCtgl = value; }
        }

        public CTriangulation ToCtgl
        {
            get { return _ToCtgl; }
            set { _ToCtgl = value; }
        }



    }
}
