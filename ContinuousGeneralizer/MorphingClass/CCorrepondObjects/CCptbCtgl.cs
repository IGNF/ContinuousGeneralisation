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
        private bool _blnSave;
        private int _intN;  // the number of edges of the regular polygon

        //private CDCEL _FrRglDCEL;

        //public CCptbCtgl(CTriangulation frctgl, CTriangulation toctgl, int intID = -1)
        //{
        //    _FrCtgl = frctgl;
        //    _ToCtgl = toctgl;

        //    _intID = intID;
        //}

        public CCptbCtgl(CPolygon frcpg, CPolygon tocpg, bool blnMaxCommonChords = true, bool blnSave = false, int intID = -1)
        {
            _blnSave = blnSave;

            frcpg.JudgeAndSetPolygon();
            IEnvelope ipgEnv = frcpg.pPolygon.Envelope;
            double dblMoveHight = 2 * ipgEnv.Height;
            _pEnvRgl = new EnvelopeClass();
            _pEnvRgl.PutCoords(ipgEnv.XMin, ipgEnv.YMin + dblMoveHight, ipgEnv.XMax, ipgEnv.YMax + dblMoveHight);



            _FrCtgl = new CTriangulation(frcpg);
            _FrCtgl.Triangulate();



            //_FrCtgl.CompareCptltAndNode();

            _ToCtgl = new CTriangulation(tocpg);
            _ToCtgl.Triangulate();

            //_ToCtgl.CompareCptltAndNode();


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
            bool blnSave = _blnSave;
            _intN = frctgl.CptLt.Count;

            CDCEL FrRglDCEL;
            CDCEL ToRglDCEL;
            ConstructRglDCEL(frctgl, toctgl, out FrRglDCEL, out ToRglDCEL, _pEnvRgl, blnSave);

            //in FrRglDCEL, cptlt, cpt.AxisAngleCEdgeLt, HalfEdgeLt and CEdgelt has been updated. 
            //FaceCpgLt should not be used before updating
            CombineAndTriangulateDCEL(ref FrRglDCEL, ref ToRglDCEL, blnSave);


            RefineOriginalCtgl(frctgl, FrRglDCEL, CEnumScale.Larger);    //CptLt, CEdgeLt, HalfEdgeLt, FaceCpgLt are updated
            RefineOriginalCtgl(toctgl, FrRglDCEL, CEnumScale.Smaller);   //CptLt, CEdgeLt, HalfEdgeLt, FaceCpgLt are updated

            if (blnSave == true)
            {
                CSaveFeature.SaveCEdgeEb(frctgl.CEdgeLt, "RefinedFrCtgl", blnVisible: false);
                CSaveFeature.SaveCptEb(frctgl.CptLt, "RefinedToCpt", blnVisible: false);

                CSaveFeature.SaveCEdgeEb(toctgl.CEdgeLt, "RefinedToCtgl", blnVisible: false);
                CSaveFeature.SaveCptEb(toctgl.CptLt, "RefinedToCpt", blnVisible: false);
            }
        }

        private void ConstructRglDCEL(CTriangulation frctgl, CTriangulation toctgl, 
            out CDCEL FrRglDCEL, out CDCEL ToRglDCEL, IEnvelope pEnvRgl, bool blnSave=false)
        {
            if (frctgl.pTinAdvanced2.DataNodeCount!=toctgl.pTinAdvanced2.DataNodeCount)
            {
                throw new ArgumentException("The numbers of points are not the same!");
            }

            FrRglDCEL = ConstructRglDCEL(frctgl, pEnvRgl, CEnumScale.Larger, blnSave);
            //note that we use the same Extent with the Fr one
            ToRglDCEL = ConstructRglDCEL(toctgl, pEnvRgl, CEnumScale.Smaller, blnSave);  

        }


        private CDCEL ConstructRglDCEL(CTriangulation ctgl, IEnvelope pEnv, CEnumScale enumScale, bool blnSave = false)
        {
            var realIndCEdgelt = ctgl.GenerateRealTinCEdgeLt();
            if (blnSave == true)
            {
                CSaveFeature.SaveCEdgeEb(realIndCEdgelt, enumScale.ToString() + "RealIndCDT", blnVisible: false);
                CSaveFeature.SaveCptEb(ctgl.CptLt, enumScale.ToString() + "RealCpt", blnVisible: false);
            }


            var RglCpg = CGeoFunc.CreateRegularCpg(pEnv, ctgl.pTinAdvanced2.DataNodeCount);

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
                CSaveFeature.SaveCEdgeEb(RglCEdgeLt, enumScale.ToString() + "RglIndCDT" + _intID, blnVisible: false);
                CSaveFeature.SaveCptEb(RglCpg.CptLt, enumScale.ToString() + "RglCpt", blnVisible: false);
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

        private void CombineAndTriangulateDCEL(ref CDCEL FrRglDCEL, ref CDCEL ToRglDCEL, bool blnSave = false)
        {
            //CombineCoStartEdges(ref FrRglDCEL,ref ToRglDCEL );
            //set tempID to identify the same edges, which is faster than comparing coordinates of the ends of edges 

            InsertCEdgeBySmallersIntoFrRglDCEL(ref FrRglDCEL, ref ToRglDCEL);

            //we do this before we triangulate all the faces, 
            //because we just want to know the breaks of the original edges.
            GetCorrRglSubCEdgeLtForParentCEdge(ref FrRglDCEL);
            if (blnSave == true)
            {
                CSaveFeature.SaveCptEb(FrRglDCEL.CptLt, "OverlapRglCDTCpt", blnVisible: false);
                CSaveFeature.SaveCEdgeEb(FrRglDCEL.CEdgeLt, "OverlapRglCDT", blnVisible: false);
            }
            TriangulateFaces(ref FrRglDCEL);
            UpdateAxisAngleCEdgeLtAndHalfCEdgeLt(ref FrRglDCEL, blnSave);

            if (blnSave == true)
            {
                //CSaveFeature.SaveCptEb(FrRglDCEL.CptLt, "OverlapRglCDTCpt");
                CSaveFeature.SaveCEdgeEb(FrRglDCEL.CEdgeLt, "CombinedAndTriangulatedDCEL", blnVisible: false);
            }
        }

        #region InsertCEdgeBySmallersIntoFrRglDCEL
        private void InsertCEdgeBySmallersIntoFrRglDCEL(ref CDCEL FrRglDCEL, ref CDCEL ToRglDCEL)
        {
            var frcptEt = FrRglDCEL.CptLt.GetEnumerator();
            var tocptEt = ToRglDCEL.CptLt.GetEnumerator();
            var IntersectCptLt = new List<CPoint>();
            int indexID = FrRglDCEL.CptLt.Count;

            //int indexIDNextto74 = 0;
            //_FrRglDCEL = FrRglDCEL;
            //indexIDNextto74 = _FrRglDCEL.HalfEdgeLt[74].cedgeNext.indexID;
            int intCount = 0;
            while (frcptEt.MoveNext() && tocptEt.MoveNext())
            {
                //if (intCount == 13)
                //{
                //    int ss = 5;
                //}
                //if (intCount == 14)
                //{
                //    int st = 5;
                //}
                //Console.WriteLine("point number: " + intCount++);

                var CoStartCpt = frcptEt.Current;
                var FrIncidentCEdge = frcptEt.Current.IncidentCEdge;
                var ToIncidentCEdge = tocptEt.Current.IncidentCEdge;



                //var FrCptIndexIDFrIncidentCEdge = FrIncidentCEdge.FrCpt.indexID;      //FrCpt IndexID
                //var ToCptIndexIDFrIncidentCEdge = FrIncidentCEdge.ToCpt.indexID;      //ToCpt IndexID
                //var FrCptIndexIDToIncidentCEdge = ToIncidentCEdge.FrCpt.indexID;      //FrCpt IndexID
                //var ToCptIndexIDToIncidentCEdge = ToIncidentCEdge.ToCpt.indexID;      //ToCpt IndexID

                //FrIncidentCEdge.PrintMySelf();
                //ToIncidentCEdge.PrintMySelf();
                
                var FrLastCEdge = FrIncidentCEdge.GetSmallerAxisAngleCEdge();
                var ToLastCEdge = ToIncidentCEdge.GetSmallerAxisAngleCEdge();

                //var FrCptIndexIDFrLastCEdge = FrLastCEdge.FrCpt.indexID;      //FrCpt IndexID
                //var ToCptIndexIDFrLastCEdge = FrLastCEdge.ToCpt.indexID;      //ToCpt IndexID
                //var FrCptIndexIDToLastCEdge = ToLastCEdge.FrCpt.indexID;      //FrCpt IndexID
                //var ToCptIndexIDToLastCEdge = ToLastCEdge.ToCpt.indexID;      //ToCpt IndexID


                //this is prepare for that ToIncidentCEdge.dblAxisAngle < FrIncidentCEdge.dblAxisAngle
                //var SmallerAxisAngleCEdge = FrLastCEdge;  
                //var intCompare = CCmpMethods.Cmp(ToCurrentCEdge.dblAxisAngle, FrCurrentCEdge.dblAxisAngle);
                //if (intCompare == -1)

                //take care of the first ToIncidentCEdge
                CEdge FrStopCEdge = FrIncidentCEdge;
                CEdge ToStopCEdge = ToIncidentCEdge;

                var SmallerAxisAngleCEdge = FrIncidentCEdge;
                // we can compare directly because the two regular polygons have the same coordiantes
                if (ToIncidentCEdge.dblAxisAngle < FrIncidentCEdge.dblAxisAngle)  
                {
                    //ToCurrentCEdge.PrintMySelf();
                    //FrCurrentCEdge.PrintMySelf();

                    //we assign FrIncidentCEdge so that we can know when to stop in the do-while loop, 
                    //otherwise it would be complicated since a to-edge has been inserted in FrRglDCEL
                    //FrIncidentCEdge = SmallerAxisAngleCEdge;

                    //in this case we will insert ToIncidentCEdge into the DCEL, 
                    //then FrLastCEdge.GetLargerAxisAngleCEdge().dblAxisAngle == ToIncidentCEdge.dblAxisAngle
                    FrStopCEdge = ToIncidentCEdge;  
                    SmallerAxisAngleCEdge = FrLastCEdge;
                }
                //if (intCompare == -1)
                else if (ToIncidentCEdge.dblAxisAngle > FrIncidentCEdge.dblAxisAngle)
                {
                    //FrStopCEdge = FrIncidentCEdge;
                }
                //else //if (intCompare == 0)
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
                    //Console.WriteLine(FrCurrentCEdge.FrCpt.indexID + "   " + FrCurrentCEdge.ToCpt.indexID);
                    //Console.WriteLine(ToCurrentCEdge.FrCpt.indexID + "   " + ToCurrentCEdge.ToCpt.indexID);
                    //var FrCptIndexIDFrCurrentCEdge = FrCurrentCEdge.FrCpt.indexID;      //FrCpt IndexID
                    //var ToCptIndexIDFrCurrentCEdge = FrCurrentCEdge.ToCpt.indexID;      //ToCpt IndexID
                    //var FrCptIndexIDToCurrentCEdge = ToCurrentCEdge.FrCpt.indexID;      //FrCpt IndexID
                    //var ToCptIndexIDToCurrentCEdge = ToCurrentCEdge.ToCpt.indexID;      //ToCpt IndexID

                    //intCompare = CCmpMethods.Cmp(ToCurrentCEdge.dblAxisAngle, FrCurrentCEdge.dblAxisAngle);

                    //if (intCompare == -1)
                    // we can compare directly because the two regular polygons have the same coordiantes
                    if (ToCurrentCEdge.dblAxisAngle < FrCurrentCEdge.dblAxisAngle)
                    {
                        ToCurrentCEdge = InsertCEdge(ToCurrentCEdge, ref SmallerAxisAngleCEdge, ref IntersectCptLt, ref indexID);
                        blnToCurrentCEdgeChanged = true;
                    }
                    //if (intCompare == -1)
                    else if (ToCurrentCEdge.dblAxisAngle > FrCurrentCEdge.dblAxisAngle)
                    {
                        SmallerAxisAngleCEdge = FrCurrentCEdge;
                        FrCurrentCEdge = FrCurrentCEdge.GetLargerAxisAngleCEdge();

                        blnFrCurrentCEdgeChanged = true;
                    }
                    //else //if (intCompare == 0)
                    else //if (ToCurrentCEdge.dblAxisAngle == FrCurrentCEdge.dblAxisAngle)
                    {
                        SmallerAxisAngleCEdge = FrCurrentCEdge;

                        FrCurrentCEdge = FrCurrentCEdge.GetLargerAxisAngleCEdge();
                        ToCurrentCEdge = ToCurrentCEdge.GetLargerAxisAngleCEdge();


                        blnToCurrentCEdgeChanged = true;
                        blnFrCurrentCEdgeChanged = true;
                    }

                    //FrCptIndexIDFrCurrentCEdge = FrCurrentCEdge.FrCpt.indexID;      //FrCpt IndexID
                    //ToCptIndexIDFrCurrentCEdge = FrCurrentCEdge.ToCpt.indexID;      //ToCpt IndexID
                    //FrCptIndexIDToCurrentCEdge = ToCurrentCEdge.FrCpt.indexID;      //FrCpt IndexID
                    //ToCptIndexIDToCurrentCEdge = ToCurrentCEdge.ToCpt.indexID;      //ToCpt IndexID

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

        private CEdge InsertCEdge(CEdge ToCurrentCEdge, ref CEdge SmallerAxisAngleCEdge, 
            ref List<CPoint> IntersectCptLt, ref int indexID)
        {
            //var FrCptIndexIDToCurrentCEdge = ToCurrentCEdge.FrCpt.indexID;      //FrCpt IndexID
            //var ToCptIndexIDToCurrentCEdge = ToCurrentCEdge.ToCpt.indexID;      //ToCpt IndexID

            //we have to do this first, because ToCurrentCEdge.GetLargerAxisAngleCEdge() will be changed by inserting
            var NextToCurrentCEdge = ToCurrentCEdge.GetLargerAxisAngleCEdge();  
            //var FrCptIndexIDNextToCurrentCEdge = NextToCurrentCEdge.FrCpt.indexID;      //FrCpt IndexID
            //var ToCptIndexIDNextToCurrentCEdge = NextToCurrentCEdge.ToCpt.indexID;      //ToCpt IndexID

            //var FrCptIndexIDSmallerAxisAngleCEdge = SmallerAxisAngleCEdge.FrCpt.indexID;      //FrCpt IndexID
            //var ToCptIndexIDSmallerAxisAngleCEdge = SmallerAxisAngleCEdge.ToCpt.indexID;      //ToCpt IndexID

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
                //int intsubcedgeFrindexID = subcedge.FrCpt.indexID;
                //int intsubcedgeToindexID = subcedge.ToCpt.indexID;
                //int intFrCurrentindexID = currentcedge.FrCpt.indexID;
                //int intToCurrentindexID = currentcedge.ToCpt.indexID;

                CIntersection pIntersection = subcedge.IntersectWith(currentcedge);
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
                    //we cannot compare coordinates here because cedge is from regular polygon 
                    //while ParentCEdge is from real polygon
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
                } while (CoStartCEdge.dblAxisAngle != incidentcedge.dblAxisAngle);
            }
        }

        private void TriangulateFaces(ref CDCEL FrRglDCEL)
        {
            //set the isTraversed attribute to flase
            SetCEdgesIsTraversedFalse(ref FrRglDCEL);

            //we don't need to triangulate super face. so we set isTraversed of all the outercedge to true
            CEdge outercedge = FrRglDCEL.HalfEdgeLt[1];   //we know that FrRglDCEL.HalfEdgeLt[1] is an outer cedge
            do
            {
                outercedge.isTraversed = true;
                outercedge = outercedge.cedgeNext;
            } while (outercedge.isTraversed == false);

            //triangulate faces
            foreach (var cpt in FrRglDCEL.CptLt)
            {
                var incidentcedge = cpt.IncidentCEdge;
                var CoStartCEdge = incidentcedge;

                do
                {
                    if (CoStartCEdge.isTraversed == false)
                    {
                        CoStartCEdge.isTraversed = true;
                        CoStartCEdge.cedgeNext.isTraversed = true;
                        CoStartCEdge.cedgeNext.cedgeNext.isTraversed = true;

                        //we define lastPreCEdge so that we will know where we should insert
                        CEdge SmallerAxisAngleCEdge = CoStartCEdge;  
                        CEdge currentcedge = CoStartCEdge.cedgeNext.cedgeNext;
                        while (CoStartCEdge.FrCpt.indexID != currentcedge.ToCpt.indexID)
                        {
                            CEdge newcedge = new CEdge(CoStartCEdge.FrCpt, currentcedge.FrCpt);
                            newcedge.CreateTwinCEdge();
                            CDCEL.InsertCEdgeBySmaller(SmallerAxisAngleCEdge, newcedge);
                            CDCEL.InsertCEdgeBySmaller(currentcedge, newcedge.cedgeTwin);

                            newcedge.isTraversed = false;
                            newcedge.cedgeTwin.isTraversed = false;
                            currentcedge.isTraversed = false;

                            currentcedge = currentcedge.cedgeNext;
                            SmallerAxisAngleCEdge = newcedge;
                        }
                    }

                    CoStartCEdge = CoStartCEdge.GetLargerAxisAngleCEdge();
                } while (CoStartCEdge.dblAxisAngle != incidentcedge.dblAxisAngle);
            }
        }

        private void UpdateAxisAngleCEdgeLtAndHalfCEdgeLt(ref CDCEL FrRglDCEL, bool blnSave=false)
        {
            SetCEdgesIsTraversedFalse(ref FrRglDCEL);
            var HalfCEdgeLt = FrRglDCEL.HalfEdgeLt.GetRange(0, 2 * _intN); //the first 2*_intN half edges are maintained
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
                } while (CoStartCEdge.dblAxisAngle != incidentcedge.dblAxisAngle);
            }

            HalfCEdgeLt.SetIndexID();

            FrRglDCEL.HalfEdgeLt = HalfCEdgeLt;            
            FrRglDCEL.UpdateCEdgeLtByHalfCEdgeLt();
        }

        private void SetCEdgesIsTraversedFalse(ref CDCEL pDCEL)
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
                } while (CoStartCEdge.dblAxisAngle != incidentcedge.dblAxisAngle);
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


            //generate new vertices without coordinates, we have to do this because we may refer some vertices we do not know yet
            int intIntersectionNumber = FrRglDCEL.CptLt.Count - _intN;
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
