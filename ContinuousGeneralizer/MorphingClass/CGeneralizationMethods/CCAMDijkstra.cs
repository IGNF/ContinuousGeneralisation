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

using ILOG.Concert;
using ILOG.CPLEX;

namespace MorphingClass.CGeneralizationMethods
{
    /// <summary>
    /// Continuous Aggregation of Maps based on Dijkstra: CAMDijkstra
    /// </summary>
    /// <remarks></remarks>
    public class CCAMDijkstra : CAreaAggregation_Base
    {
        #region Preprocessing
        public CCAMDijkstra()
        {

        }

        public CCAMDijkstra(CParameterInitialize ParameterInitialize, string strSpecifiedFieldName = null, string strSpecifiedValue = null)
        {
            Preprocessing(ParameterInitialize, strSpecifiedFieldName, strSpecifiedValue);
        }


        public void CAMDijkstra(int intQuitCount, string strMethod)
        {
            SetupBasic();

             switch (strMethod)
             {
                 case "AStar":
                     CRegion._lngEstCountEdgeNumber = 0;
                     CRegion._lngEstCountEdgeLength = 0;
                     CRegion._lngEstCountEqual = 0;

                    //CRegion._pCrg = new CGeometry.CRegion();
                    //CRegion._pCrg






                     for (int i = _intStart; i < _intEnd; i++)
                     {
                         AStar(LSCrgLt[i], SSCrgLt[i], this.StrObjLtSD, _ParameterInitialize.strAreaAggregation, this._adblTD, intQuitCount);
                     }
                     //Console.WriteLine();
                     //Console.WriteLine("Estimation functions that we used:");
                     //Console.WriteLine("By EdgeNumber: " + CRegion._lngEstCountEdgeNumber +
                     //    ";   By EdgeLength: " + CRegion._lngEstCountEdgeLength +
                     //    ";   EqualCases: " + CRegion._lngEstCountEqual);
                     break;
                 case "ILP":
                     for (int i = _intStart; i < _intEnd; i++)
                     {
                         ILP(LSCrgLt[i], SSCrgLt[i], this.StrObjLtSD, this._adblTD, _ParameterInitialize.strAreaAggregation);
                     }
                     break;
                 case "ILP_Extend":
                     //update the address stored in Path.txt
                     using (var writer = new System.IO.StreamWriter(
                         "C:\\Study\\Programs\\ContinuousGeneralizer\\ContinuousGeneralizer64\\InputPath\\Path.txt", false))
                     {
                         writer.Write(_ParameterInitialize.strSavePath);
                     }
                     ExportadblTD(_ParameterInitialize.strSavePath, this._adblTD);
                     for (int i = _intStart; i < _intEnd; i++)
                     {
                         ILP_Extend(LSCrgLt[i], SSCrgLt[i], this._adblTD);
                     }
                     //RunContinuousGeneralizer64();
                     break;
                 default:
                     break;
             }
        }
        #endregion

        #region ILP
        public CRegion ILP(CRegion LSCrg, CRegion SSCrg, CStrObjLtSD StrObjLtSD, double[,] adblTD, string strAreaAggregation)
        {
            double dblMemoryInMB1 = CHelpFunc.GetConsumedMemoryInMB(true);

            LSCrg.SetInitialAdjacency();  //also count the number of edges

            System.Console.WriteLine();
            System.Console.WriteLine();
            System.Console.WriteLine("Crg:  ID  " + LSCrg.ID + ";    n  " + LSCrg.CphTypeIndexSD_Area_CphGID.Count + ";    m  " +
                LSCrg.AdjCorrCphsSD.Count + "  " + _ParameterInitialize.strAreaAggregation + "================================="
                + LSCrg.ID + "  " + _ParameterInitialize.strAreaAggregation + "==============================");
            

            Stopwatch pStopwatch = new Stopwatch();
            pStopwatch.Start();
            AddLineToStrObjLtSD(StrObjLtSD, LSCrg);

            double dblMemoryInMB2 = CHelpFunc.GetConsumedMemoryInMB(true);
            Cplex cplex = new Cplex();

            double dblMemoryInMB3 = CHelpFunc.GetConsumedMemoryInMB(true);

            CRegion crg = new CRegion(-1);
            bool blnSoloved = true;
            try
            {
                //Step 3
                //Cplex cplex = new Cplex();
                IIntVar[][][] var2;
                IIntVar[][][][] var3;
                IIntVar[][][][][] var4;
                IRange[][] rng;

                PopulateByRow(cplex, out var2, out var3, out var4, out rng, LSCrg, SSCrg, adblTD, strAreaAggregation);
                double dblMemoryInMB4 = CHelpFunc.GetConsumedMemoryInMB(true);
                // Step 11
                //cplex.ExportModel("lpex1.lp");

                // Step 9
                cplex.SetParam(Cplex.DoubleParam.TiLim, 1170);  //1170 is from _All_MinimizeInteriorBoundaries_200000000
                //cplex.SetParam(Cplex.IntParam.ParallelMode, 1);
                //cplex.SetParam(Cplex.ParallelMode.Deterministic,cplex.pa);

                //ILOG.Concert.

                if (cplex.Solve())
                {

                    //***********Gap for ILP************

                    #region Display x, y, z, and s
                    //for (int i = 0; i < var3[0].GetLength(0); i++)
                    //{

                    //    Console.WriteLine("Variable x; Time: " + (i + 1).ToString());

                    //    foreach (var x1 in var3[0][i])
                    //    {
                    //        //CPatch 



                    //        double[] x = cplex.GetValues(x1);


                    //        foreach (var x0 in x)
                    //        {
                    //            Console.Write(x0 + "     ");
                    //        }
                    //        Console.WriteLine();

                    //    }
                    //    Console.WriteLine();


                    //}

                    #region Display y and z
                    //if (var4[0] != null)
                    //{
                    //    Console.WriteLine("");
                    //    //Console.WriteLine("Variable y:");
                    //    for (int i = 0; i < var4[0].GetLength(0); i++)
                    //    {
                    //        Console.WriteLine("Variable y; Time: " + (i + 1).ToString());
                    //        foreach (var y2 in var4[0][i])
                    //        {
                    //            foreach (var y1 in y2)
                    //            {

                    //                double[] y = cplex.GetValues(y1);


                    //                foreach (var y0 in y)
                    //                {
                    //                    Console.Write(y0 + "     ");
                    //                }
                    //                Console.WriteLine();
                    //            }

                    //            Console.WriteLine();
                    //        }
                    //        //Console.WriteLine();
                    //    }
                    //}

                    //if (var4[1] != null)
                    //{
                    //    Console.WriteLine("");
                    //    //Console.WriteLine("Variable z:");
                    //    for (int i = 0; i < var4[1].GetLength(0); i++)
                    //    {
                    //        Console.WriteLine("Variable z; Time: " + (i + 1).ToString());
                    //        foreach (var z2 in var4[1][i])
                    //        {
                    //            foreach (var z1 in z2)
                    //            {

                    //                double[] z = cplex.GetValues(z1);


                    //                foreach (var z0 in z)
                    //                {
                    //                    Console.Write(z0 + "     ");
                    //                }
                    //                Console.WriteLine();

                    //            }
                    //            Console.WriteLine();
                    //        }
                    //        //Console.WriteLine();
                    //    }
                    //}
                    #endregion


                    //if (_ParameterInitialize.strAreaAggregation == "Smallest")
                    //{
                    //    Console.WriteLine("");
                    //    Console.WriteLine("Variable s:");
                    //    if (var2[0] != null)
                    //    {
                    //        for (int i = 0; i < var2[0].GetLength(0); i++)
                    //        {


                    //            double[] s = cplex.GetValues(var2[0][i]);


                    //            foreach (var s0 in s)
                    //            {
                    //                Console.Write(s0 + "     ");
                    //            }
                    //            Console.WriteLine();

                    //        }
                    //    }
                    //}
                    #endregion

                    #region Display other results
                    //double[] dj = cplex.GetReducedCosts(var3[0][0][0]);
                    //double[] dj2 = cplex.GetReducedCosts((var3);
                    //double[] pi = cplex.GetDuals(rng[0]);
                    //double[] slack = cplex.GetSlacks(rng[0]);
                    //Console.WriteLine("");
                    //cplex.Output().WriteLine("Solution status = "
                    //+ cplex.GetStatus());
                    //cplex.Output().WriteLine("Solution value = "
                    //+ cplex.ObjValue);
                    //objDataLt[13] = cplex.ObjValue;
                    //int nvars = x.Length;
                    //for (int j = 0; j < nvars; ++j)
                    //{
                    //    cplex.Output().WriteLine("Variable :"
                    //    + j
                    //    + " Value = "
                    //    + x[j]
                    //    + " Reduced cost = "
                    //    + dj[j]);
                    //}
                    //int ncons = slack.Length;
                    //for (int i = 0; i < ncons; ++i)
                    //{
                    //    cplex.Output().WriteLine("Constraint:"
                    //    + i
                    //    + " Slack = "
                    //    + slack[i]
                    //    //+ " Pi = "
                    //    //+ pi[i]
                    //    );
                    //}
                    #endregion
                    
                }

                Console.WriteLine("");
                cplex.Output().WriteLine("Solution status = "
                + cplex.GetStatus());
                cplex.Output().WriteLine("Solution value = "
                + cplex.ObjValue);
                string strStatus = cplex.GetStatus().ToString();
                //StrObjLtSD.SetLastObj("#Edges", strStatus);
                StrObjLtSD.SetLastObj("WeightedSum", cplex.ObjValue);

                if (strStatus == "Optimal")
                {
                    StrObjLtSD.SetLastObj("Factor", 1);
                }
                else if (strStatus == "Feasible")
                {
                    StrObjLtSD.SetLastObj("Factor", 5000);
                }
            }
            catch (ILOG.Concert.Exception e)
            {
                blnSoloved = false;
                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
            catch (System.OutOfMemoryException e2)
            {
                blnSoloved = false;
                System.Console.WriteLine("System exception '" + e2);
            }
            finally
            {
                double dblMemoryInMB = CHelpFunc.GetConsumedMemoryInMB(false);
                if (blnSoloved == false)
                {
                    crg.ID = -2;
                    System.Console.WriteLine("We have used memory " + dblMemoryInMB + "MB.");
                    Console.WriteLine("Crg:  ID  " + LSCrg.ID + ";    n  " + LSCrg.CphTypeIndexSD_Area_CphGID.Count + ";    m  " + 
                        LSCrg.AdjCorrCphsSD.Count + "  could not be solved by ILP!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!");
                }
                StrObjLtSD.SetLastObj("TimeLast(ms)", pStopwatch.ElapsedMilliseconds);
                StrObjLtSD.SetLastObj("Time(ms)", pStopwatch.ElapsedMilliseconds);
                StrObjLtSD.SetLastObj("Memory(MB)", dblMemoryInMB);
                cplex.End();
            }



            return crg;
        }

        // Step 4
        internal static void PopulateByRow(IMPModeler model,out
        IIntVar[][][] var2, out
        IIntVar[][][][] var3, out IIntVar[][][][][] var4, out
        IRange[][] rng, CRegion lscrg, CRegion sscrg, double[,] adblTD, string strAreaAggregation)
        {
            var aCph = lscrg.CphTypeIndexSD_Area_CphGID.Keys.ToArray();
            int intCpgCount = lscrg.GetCphCount();
            double dblILPSmallValue = 0;
            
            IIntVar[][][] x = new IIntVar[intCpgCount][][];
            for (int i = 0; i < intCpgCount; i++)
            {
                x[i] = new IIntVar[intCpgCount][];
                for (int j = 0; j < intCpgCount; j++)
                {
                    x[i][j] = model.BoolVarArray(intCpgCount);
                }
            }

            //cost in terms of type change
            var y = Generate4DNumVar(model, intCpgCount - 1, intCpgCount, intCpgCount, intCpgCount);

            //cost in terms of compactness (length of interior boundaries)
            var z = Generate4DNumVar(model, intCpgCount - 2, intCpgCount, intCpgCount, intCpgCount);


            var3 = new IIntVar[1][][][];
            var4 = new IIntVar[2][][][][];
            var3[0] = x;
            var4[0] = y;
            var4[1] = z;

            //add minimizations
            ILinearNumExpr pTypeCostExpr = model.LinearNumExpr();
            //ILinearNumExpr pTypeCostAssitantExpr = model.LinearNumExpr();
            for (int i = 0; i < intCpgCount - 1; i++)   //i represents indices
            {
                for (int j = 0; j < intCpgCount; j++)
                {
                    for (int k = 0; k < intCpgCount; k++)
                    {
                        for (int l = 0; l < intCpgCount; l++)
                        {
                            double dblCoe = aCph[j].dblArea * adblTD[aCph[k].intTypeIndex, aCph[l].intTypeIndex];
                            pTypeCostExpr.AddTerm(y[i][j][k][l], dblCoe);
                            //pTypeCostAssitantExpr.AddTerm(y[i][j][k][l], dblILPSmallValueMinimization);
                        }
                    }
                }
            }

            double dblCompCostFirstPart = 1 / Convert.ToDouble(intCpgCount - 1);  //this is actually for t=1, whose compactness is known
            ILinearNumExpr pCompCostSecondPartExpr = model.LinearNumExpr();
            var pAdjCorrCphsSD = lscrg.AdjCorrCphsSD;
            double dblConst = Convert.ToDouble(intCpgCount - 2) / Convert.ToDouble(intCpgCount - 1);
            
            for (int i = 2; i < intCpgCount; i++)   //i represents time
            {
                double dblNminusT = intCpgCount - i;
                double dblTemp = (intCpgCount - i) * dblConst;
                dblCompCostFirstPart += 1 / dblNminusT;
                double dblNorm = lscrg.dblInteriorSegLength * dblNminusT;

                foreach (var pCorrCphs in pAdjCorrCphsSD.Keys)  //we don't need to divide the value by 2 because every boundary is only counted once
                {
                    for (int l = 0; l < intCpgCount; l++)
                    {
                        pCompCostSecondPartExpr.AddTerm(pCorrCphs.dblSharedSegLength / dblNorm, z[i - 2][pCorrCphs.FrCph.ID][pCorrCphs.ToCph.ID][l]);
                    }
                }
            }

            if (intCpgCount == 1)
            {
                model.AddMinimize(pTypeCostExpr);  //we just use an empty expression
            }
            else
            {
                //Our Model***************************************
                var Ftp = model.Prod(pTypeCostExpr, 1 / lscrg.dblArea);
                var Fcp = model.Sum(dblCompCostFirstPart, model.Negative(pCompCostSecondPartExpr));
                model.AddMinimize(model.Prod(model.Sum(Ftp, Fcp), 0.5));
            }




            //constraints
            IList<IRange> IRangeLt = new List<IRange>();
            //to restrict y
            for (int i = 0; i < intCpgCount - 1; i++)   //i represents indices
            {
                for (int j = 0; j < intCpgCount; j++)
                {
                    for (int k = 0; k < intCpgCount; k++)
                    {
                        //IRangeLt.Add(model.AddLe(model.Sum(y[i][j][k][k], x[i][j][k], x[i + 1][j][k]), 2.0 , "RestrictY"));

                        for (int l = 0; l < intCpgCount; l++)
                        {
                            if (k != l)
                            {
                                IRangeLt.Add(model.AddLe(model.Sum(model.Negative(y[i][j][k][l]), x[i][j][k], x[i + 1][j][l]), 1.0 + dblILPSmallValue, "RestrictY"));
                            }
                        }
                    }
                }
            }

            //model.
            //to restrict z
            for (int i = 0; i < intCpgCount - 2; i++)   //i represents indices
            {
                for (int j = 0; j < intCpgCount; j++)
                {
                    for (int k = j; k < intCpgCount; k++)
                    {
                        for (int l = 0; l < intCpgCount; l++)
                        {
                            IRangeLt.Add(model.AddLe(model.Sum(z[i][j][k][l], model.Negative(x[i + 1][j][l])), dblILPSmallValue, "RestrictZ1"));
                            IRangeLt.Add(model.AddLe(model.Sum(z[i][j][k][l], model.Negative(x[i + 1][k][l])), dblILPSmallValue, "RestrictZ2"));

                            //z[i][j][k][l]=z[i][k][j][l]
                            IRangeLt.Add(model.AddEq(model.Sum(z[i][j][k][l], model.Negative(z[i][k][j][l])), 0.0, "RestrictZ3"));
                        }
                    }
                }
            }

            //a polygon $p$ is assigned to exactly one polygon at a step $\tau$
            for (int i = 0; i < intCpgCount; i++)   //i represents indices
            {
                for (int j = 0; j < intCpgCount; j++)
                {
                    ILinearNumExpr pOneCenterExpr = model.LinearNumExpr();
                    for (int l = 0; l < intCpgCount; l++)
                    {
                        pOneCenterExpr.AddTerm(x[i][j][l], 1.0);
                    }

                    IRangeLt.Add(model.AddEq(pOneCenterExpr, 1.0, "AssignToOnePg"));
                }
            }

            //a polygon must be assigned to a center
            for (int i = 0; i < intCpgCount; i++)   //i represents indices
            {
                for (int j = 0; j < intCpgCount; j++)
                {
                    for (int l = 0; l < intCpgCount; l++)
                    {
                        IRangeLt.Add(model.AddLe(model.Sum(x[i][j][l], model.Negative(x[i][l][l])), dblILPSmallValue, "AssignedIsCenter__" + i + "__" + j + "__" + l));
                    }
                }
            }


            //If two polygons have been aggregated into one polygon, then they will 
            //be aggregated together in later steps. Our sixth constraint achieve this by requiring
            for (int i = 1; i < intCpgCount - 2; i++)   //i represents indices
            {
                for (int j = 0; j < intCpgCount; j++)
                {
                    for (int k = 0; k < intCpgCount; k++)
                    {
                        ILinearNumExpr pAssignTogetherExprPre = model.LinearNumExpr();
                        ILinearNumExpr pAssignTogetherExprAfter = model.LinearNumExpr();
                        for (int l = 0; l < intCpgCount; l++)
                        {
                            pAssignTogetherExprPre.AddTerm(z[i - 1][j][k][l], 1.0);
                            pAssignTogetherExprAfter.AddTerm(z[i][j][k][l], -1.0);
                        }
                        IRangeLt.Add(model.AddLe(model.Sum(pAssignTogetherExprPre, pAssignTogetherExprAfter), 0.0, "AssignTogether"));
                    }
                }
            }

            //polygon $p$ can be assigned to center $o$ if at least one of $p$'s neighbors has already been assigned to center $o$            
            for (int i = 1; i < intCpgCount - 1; i++)   //i represents indices
            {
                for (int j = 0; j < intCpgCount; j++)
                {
                    for (int k = 0; k < intCpgCount; k++)
                    {
                        if (j==k)  //the considered point is free to be assigned to itself
                        {
                            continue;
                        }

                        ILinearNumExpr pContiguityExpr = model.LinearNumExpr();
                        //pContiguityExpr.AddTerm(x[i][j][k], 1.0);  //including polygon j itself
                        foreach (var pAdjacentCph in aCph[j].AdjacentCphSS)
                        {
                            pContiguityExpr.AddTerm(x[i][pAdjacentCph.ID][k], 1.0);
                        }
                        IRangeLt.Add(model.AddLe(model.Sum(x[i][j][k], model.Negative(pContiguityExpr)), 0.0, "Contiguity"));
                    }
                }
            }

            //only one patch is aggregated into another patch at each step
            for (int i = 0; i < intCpgCount; i++)   //i represents indices
            {
                ILinearNumExpr pOneAggregationExpr = model.LinearNumExpr();
                for (int j = 0; j < intCpgCount; j++)
                {
                    pOneAggregationExpr.AddTerm(x[i][j][j], 1.0);
                }
                IRangeLt.Add(model.AddEq(pOneAggregationExpr, intCpgCount - i, "CountCenters"));
            }

            //a center can disappear, but will never reappear afterwards
            for (int i = 1; i < intCpgCount; i++)   //i represents indices
            {
                for (int j = 0; j < intCpgCount; j++)
                {
                    IRangeLt.Add(model.AddGe(model.Sum(x[i - 1][j][j], model.Negative(x[i][j][j])), 0.0, "SteadyCenters"));
                }
            }


            //to make sure that the final aggregated polygon has the same color as the target polygon
            ILinearNumExpr pFinalStateExpr = model.LinearNumExpr();
            for (int i = 0; i < intCpgCount; i++)
            {
                if (aCph[i].intTypeIndex == sscrg.CphTypeIndexSD_Area_CphGID.Keys.GetFirstT().intTypeIndex)
                {
                    pFinalStateExpr.AddTerm(x[intCpgCount - 1][i][i], 1.0);
                }
            }
            IRangeLt.Add(model.AddEq(pFinalStateExpr, 1.0, "EnsureTarget"));

            var2 = new IIntVar[1][][];
            if (strAreaAggregation == "Smallest")
            {
                //add more constraints to y
                for (int i = 0; i < intCpgCount - 1; i++)   //i represents indices
                {
                    for (int j = 0; j < intCpgCount; j++)
                    {
                        for (int k = 0; k < intCpgCount; k++)
                        {
                            for (int l = 0; l < intCpgCount; l++)
                            {
                                if (k != l)
                                {
                                    IRangeLt.Add(model.AddLe(model.Sum(y[i][j][k][l], model.Negative(x[i][j][k])), dblILPSmallValue, "RestrictY1_S"));
                                    IRangeLt.Add(model.AddLe(model.Sum(y[i][j][k][l], model.Negative(x[i + 1][j][l])), dblILPSmallValue, "RestrictY2_S"));
                                }
                            }
                        }
                    }
                }


                //add more constraints to z
                for (int i = 0; i < intCpgCount - 2; i++)   //i represents indices
                {
                    for (int j = 0; j < intCpgCount; j++)
                    {
                        for (int k = j+1; k < intCpgCount; k++)
                        {
                            for (int l = 0; l < intCpgCount; l++)
                            {
                                IRangeLt.Add(model.AddLe(model.Sum(model.Negative(z[i][j][k][l]), x[i][j][l], x[i][k][l]), 1.0 + dblILPSmallValue, "RestrictZ_S"));

                            }
                        }
                    }
                }

                IIntVar[][] s = new IIntVar[intCpgCount - 1][];
                for (int i = 0; i < intCpgCount - 1; i++)
                {
                    s[i] = model.BoolVarArray(intCpgCount);
                }
                var2[0] = s;

                //there is only one smallest patch will be involved in each aggregation step
                for (int i = 0; i < intCpgCount - 1; i++)   //i represents indices
                {
                    ILinearNumExpr pOneSmallestExpr = model.LinearNumExpr();
                    for (int j = 0; j < intCpgCount; j++)
                    {
                        pOneSmallestExpr.AddTerm(s[i][j], 1.0);
                    }

                    IRangeLt.Add(model.AddEq(pOneSmallestExpr, 1.0, "OneSmallest"));
                }

                //forces that the aggregation must involve the smallest patch.
                for (int i = 0; i < intCpgCount - 1; i++)   //i represents indices
                {
                    for (int j = 0; j < intCpgCount; j++)
                    {
                        ILinearNumExpr pInvolveSmallestExpr = model.LinearNumExpr();
                        for (int k = 0; k < intCpgCount; k++)
                        {
                            for (int l = 0; l < intCpgCount; l++)
                            {
                                if (j != k && j != l)
                                {
                                    pInvolveSmallestExpr.AddTerm(y[i][k][l][j], 1.0);
                                }

                                if (l != k && l != j)
                                {
                                    pInvolveSmallestExpr.AddTerm(y[i][k][j][l], 1.0);
                                }

                                //pInvolveSmallestExpr.AddTerm(y[i][k][l][j], 1.0);
                                //pInvolveSmallestExpr.AddTerm(y[i][k][j][l], 1.0);
                            }
                        }

                        IRangeLt.Add(model.AddLe(model.Sum(s[i][j], model.Negative(pInvolveSmallestExpr)), 0.0, "InvolveSmallest"));
                    }
                }

                //To guarantee that patch $o$ involved in aggregation is indeed the smallest patch
                for (int i = 0; i < intCpgCount - 1; i++)   //i represents indices
                {
                    var aAreaExpr = ComputeAreaExpr(model, x[i], aCph);
                    for (int j = 0; j < intCpgCount; j++)
                    {
                        for (int k = 0; k < intCpgCount; k++)
                        {
                            var pSumExpr = model.Sum(model.Negative(model.Sum(s[i][j], x[i][k][k])), 2.0);
                            var pProdExpr = model.Prod(pSumExpr, lscrg.dblArea);

                            //A_{t,o}-A_{t,r}-M(2-S_{t,o}-x_{t,r,r}) <= 0
                            IRangeLt.Add(model.AddLe(model.Sum(aAreaExpr[j], model.Negative(aAreaExpr[k]), model.Negative(pProdExpr)), 0.0, "IndeedSmallest"));
                        }
                    }
                }
            }



            //***************compare with number of constraints counted manually************
            rng=new IRange[1][];
            rng[0] = new IRange[IRangeLt.Count];
            for (int i = 0; i < IRangeLt.Count; i++)
            {
                rng[0][i] = IRangeLt[i];
            }
        }

        internal static ILinearNumExpr[] ComputeAreaExpr(IMPModeler model, IIntVar[][] x, CPatch[] aCph)
        {
            int intCpgCount = aCph.GetLength(0);
            ILinearNumExpr[] aAreaExpr = new ILinearNumExpr[intCpgCount];
            for (int i = 0; i < intCpgCount; i++)
            {
                aAreaExpr[i] = model.LinearNumExpr();
                for (int j = 0; j < intCpgCount; j++)
                {
                    aAreaExpr[i].AddTerm(x[j][i], aCph[j].dblArea);
                }
            }

            return aAreaExpr;
        }

        internal static IIntVar[][][][] Generate4DNumVar(IMPModeler model, int intCount1, int intCount2, int intCount3, int intCount4)
        {
            if (intCount1<0)
            {
                intCount1 = 0;
            }

            IIntVar[][][][] x = new IIntVar[intCount1][][][];
            for (int i = 0; i < intCount1; i++)
            {
                x[i] = new IIntVar[intCount2][][];
                for (int j = 0; j < intCount2; j++)
                {
                    x[i][j] = new IIntVar[intCount3][];
                    for (int k = 0; k < intCount3; k++)
                    {
                        x[i][j][k] = model.BoolVarArray(intCount4);
                    }
                }
            }

            return x;
        }
        #endregion

        #region ILP_Extend


        public CRegion ILP_Extend(CRegion LSCrg, CRegion SSCrg, double[,] adblTD)
        {
            var ExistingCorrCphsSD0 = LSCrg.SetInitialAdjacency();  //also count the number of edges
            var aCph = LSCrg.CphTypeIndexSD_Area_CphGID.Keys.ToArray();
            int intCpgCount = LSCrg.CphTypeIndexSD_Area_CphGID.Count;

            //create a directory for current LSCrg
            System.IO.Directory.CreateDirectory(_ParameterInitialize.strSavePath + "\\" + LSCrg.ID);

            //Output aCph: GID Area intTypeIndex {adjacentcph.ID ...}           
            string strCphs = "";
            foreach (var cph in aCph)
            {
                strCphs += cph.GID + " " + cph.dblArea + " " + cph.intTypeIndex;
                foreach (var adjacentcph in cph.AdjacentCphSS)
                {
                    strCphs += " " + adjacentcph.ID;
                }
                strCphs += "\n";
            }
            using (var writer = new StreamWriter(_ParameterInitialize.strSavePath + "\\" + LSCrg.ID + "\\" + "aCph.txt", false))
            {
                writer.Write(strCphs);
            }

            //Output LSCrg: ID Area dblInteriorSegLength intTargetTypeIndex {corrcphs: GID FrCph.ID ToCph.ID dblSharedSegLength ...}
            string strLSCrg = LSCrg.ID + " " + LSCrg.dblArea + " " + LSCrg.dblInteriorSegLength + " " + SSCrg.CphTypeIndexSD_Area_CphGID.GetFirstT().Key.intTypeIndex + "\n";
            foreach (var corrcphs in LSCrg.AdjCorrCphsSD.Keys)
            {
                strLSCrg +=corrcphs.GID + " " + corrcphs.FrCph.ID + " " + corrcphs.ToCph.ID + " " + corrcphs.dblSharedSegLength + "\n";
            }
            using (var writer = new StreamWriter(_ParameterInitialize.strSavePath + "\\" + LSCrg.ID + "\\" + "LSCrg.txt", false))
            {
                writer.Write(strLSCrg);
            }

            

            //System.Console.WriteLine();
            //System.Console.WriteLine();
            //System.Console.WriteLine(LSCrg.ID + "  " + _ParameterInitialize.strAreaAggregation + "============================================="
            //    + LSCrg.ID + "  " + _ParameterInitialize.strAreaAggregation + "=================================================");
            


            return null;
        }

        private void ExportadblTD(string strSavePath, double[,] adblTD)
        {
            //Output adblTD
            string strTD = adblTD.GetLength(0) + " " + adblTD.GetLength(1) +"\n";
            for (int i = 0; i < adblTD.GetLength(0); i++)
            {
                strTD += adblTD[i, 0];
                for (int j = 1; j < adblTD.GetLength(1); j++)
                {
                    strTD += " " + adblTD[i, j];
                }
                strTD += "\n";
            }
            using (var writer = new StreamWriter(strSavePath + "\\" + "adblTD.txt", false))
            {
                writer.Write(strTD);
            }
        }

        private void RunContinuousGeneralizer64()
        {
            Process process = new Process();
            // Configure the process using the StartInfo properties.
            process.StartInfo.FileName = @"C:\Study\Programs\ContinuousGeneralizer\ContinuousGeneralizer64\ContinuousGeneralizer64\bin\Debug\ContinuousGeneralizer64.exe";
            process.StartInfo.Arguments = "-n";
            process.StartInfo.WindowStyle = ProcessWindowStyle.Maximized;
            process.Start();
            process.WaitForExit();// Waits here for the process to exit.
        }
                      
        #endregion


        #region AStar
        public CRegion AStar(CRegion LSCrg, CRegion SSCrg, CStrObjLtSD StrObjLtSD, string strAreaAggregation, double[,] padblTD, int intQuitCount = 200000)
        {
            var ExistingCorrCphsSD0 = LSCrg.SetInitialAdjacency();  //also count the number of edges
            
            Stopwatch pStopwatchOverHead = new Stopwatch();
            pStopwatchOverHead.Start();
            int intFactor = _intStartFactor;
            
            //CRegion._intStartStaticGIDAll = CRegion._intStaticGID;
            

            AddLineToStrObjLtSD(StrObjLtSD, LSCrg);

            long lngStartMemory = 0;
            Console.WriteLine();
            Console.WriteLine("Crg:  ID  " + LSCrg.ID + ";    n  " + LSCrg.CphTypeIndexSD_Area_CphGID.Count + ";    m  " +
                    LSCrg.AdjCorrCphsSD.Count + "   " + intQuitCount + "   " + CConstants.strShapeConstraint + "   " + strAreaAggregation);

            lngStartMemory = GC.GetTotalMemory(true);
            long lngTimeOverHead = pStopwatchOverHead.ElapsedMilliseconds;
            pStopwatchOverHead.Stop();

            Stopwatch pStopwatchLast=new Stopwatch ();
            bool blnRecordTimeFirst = false;
            long lngTimeFirst = 0;
            long lngTimeLast = 0;
            long lngTimeAll = lngTimeOverHead;
            CRegion resultcrg = new CRegion(-2);
            do
            {
                try
                {
                    
                    //CRegion._intStartStaticGIDLast = CRegion._intStaticGID;
                    pStopwatchLast.Restart();
                    var ExistingCorrCphsSD = new SortedDictionary<CCorrCphs, CCorrCphs>(ExistingCorrCphsSD0, ExistingCorrCphsSD0.Comparer);
                    LSCrg.cenumColor = CEnumColor.white;

                    resultcrg = ComputeAccordFactor(LSCrg, SSCrg, strAreaAggregation, ExistingCorrCphsSD, intFactor, StrObjLtSD, padblTD, intQuitCount);
                }
                catch (System.OutOfMemoryException ex)
                {
                    Console.WriteLine(ex.Message);
                }

                if (blnRecordTimeFirst == false)
                {
                    lngTimeFirst = pStopwatchLast.ElapsedMilliseconds + lngTimeOverHead;
                    blnRecordTimeFirst = true;
                }
                lngTimeLast = pStopwatchLast.ElapsedMilliseconds + lngTimeOverHead;
                lngTimeAll += pStopwatchLast.ElapsedMilliseconds;

                intFactor *= 2;
            } while (resultcrg.ID == -2);
            intFactor /= 2;
            StrObjLtSD.SetLastObj("Factor", intFactor);
            Console.WriteLine("d: " + resultcrg.d 
                + "            Type: " + resultcrg.dblCostExactType 
                + "            Compactness: " + resultcrg.dblCostExactComp);

            //int intExploredRegionAll = CRegion._intStaticGID - CRegion._intStartStaticGIDLast;  //we don't need to +1 because +1 is already included in _intStaticGID
            double dblConsumedMemoryInMB = CHelpFunc.GetConsumedMemoryInMB(false);

            StrObjLtSD.SetLastObj("#Edges", CRegion._intEdgeCount);
            StrObjLtSD.SetLastObj("TimeFirst(ms)", lngTimeFirst);
            StrObjLtSD.SetLastObj("TimeLast(ms)", lngTimeLast);
            StrObjLtSD.SetLastObj("Time(ms)", lngTimeAll);
            StrObjLtSD.SetLastObj("Memory(MB)", CHelpFunc.GetConsumedMemoryInMB(false, lngStartMemory));

            Console.WriteLine("Factor:" + intFactor + "      We have visited " + 
                CRegion._intNodeCount + " Nodes and " + CRegion._intEdgeCount + " Edges.");

            return resultcrg;
        }

        private CRegion ComputeAccordFactor(CRegion LSCrg, CRegion SSCrg, string strAreaAggregation,
            SortedDictionary<CCorrCphs, CCorrCphs> ExistingCorrCphsSD, int intFactor, CStrObjLtSD StrObjLtSD, double[,] padblTD, int intQuitCount = 200000)
        {
            int intRegionID = LSCrg.ID;  //all the regions generated in this function will have the same intRegionID
            int intSSTypeIndex = SSCrg.CphTypeIndexSD_Area_CphGID.GetFirstT().Value;

            LSCrg.InitialEstimatedCost(SSCrg, padblTD, intFactor);
            //LSCrg.SetCoreCph(intSSTypeIndex);

            //a region represents a node in graph, ExistingCrgSD stores all the nodes
            var ExistingCphSDLt = new List<SortedDictionary<CPatch, CPatch>>(LSCrg.CphTypeIndexSD_Area_CphGID.Count + 1);  //we use this dictionary to make sure that if the two patches have the same cpgs, then they have the same GID
            for (int i = 0; i < ExistingCphSDLt.Capacity; i++)
            {
                var Element = new SortedDictionary<CPatch, CPatch>(CPatch.pCmpCPatch_CpgGID);
                ExistingCphSDLt.Add(Element);
            }

            var ExistingCrgSDLt = new List<SortedDictionary<CRegion, CRegion>>(LSCrg.GetCphCount() + 1);
            for (int i = 0; i < ExistingCrgSDLt.Capacity; i++)
            {
                var Element = new SortedDictionary<CRegion, CRegion>(CRegion.pCmpCRegion_CphGIDTypeIndex);  //we don't compare exact cost first because of there may be rounding problems 
                ExistingCrgSDLt.Add(Element);
            }
            ExistingCrgSDLt[LSCrg.GetCphCount()].Add(LSCrg, LSCrg);

            var FinalOneCphCrg = new CRegion(intRegionID);
            var Q = new SortedSet<CRegion>(CRegion.pCmpCRegion_Cost_CphGIDTypeIndex);
            int intCount = 0;
            CRegion._intNodeCount = 1;
            CRegion._intEdgeCount = 0;
            Q.Add(LSCrg);
            while (true)
            {
                intCount++;
                var u = Q.Min;
                if (Q.Remove(u) == false)
                {
                    throw new ArgumentException("cannot move an element in this queue! A solution might be make dblVerySmall smaller!");
                }

                //List<CRegion> crgcol = new List<CRegion>();
                //crgcol.Add(u);

                //OutputMap(crgcol, this._TypePVSD, u.d, intCount, pParameterInitialize);

                //MessageBox.Show("click for next!");

                //if (CConstants.strShapeConstraint == "MaximizeMinComp_EdgeNumber" || CConstants.strShapeConstraint == "MinimizeInteriorBoundaries")
                //{
                //    Console.WriteLine("Crg:  ID  " + u.ID + ";      GID:" + u.GID + ";      CphNum:" + u.CphTypeIndexSD_Area_CphGID.Count + ";      d:" + u.d +
                //    ";      ExactCost:" + u.dblCostExact + ";      Compactness:" + u.dblCostExactComp + ";      Type:" + u.dblCostExactType / u.dblArea);
                //}
                //else if (CConstants.strShapeConstraint == "NonShape")
                //{
                //    Console.WriteLine("Crg:  ID  " + u.ID + ";      GID:" + u.GID + ";      CphNum:" + u.CphTypeIndexSD_Area_CphGID.Count + ";      d:" + u.d +
                //    ";      ExactCost:" + u.dblCostExactType);
                //}

                //at the beginning, resultcrg.d is double.MaxValue. Later, when we first encounter that there is only one CPatch in LSCrg, resultcrg.d will be changed to the real cost
                //u.d contains estimation, and resultcrg.d doesn't contains. if u.d > resultcrg.d, then resultcrg.d must already be the smallest cost
                if (u.GetCphCount() == 1)
                {
                    Console.WriteLine("The number of nodes we can forget:   " + intCount);
                    Console.WriteLine("The nodes in the stack:   " + Q.Count);

                    //int intCrgCount = 0;
                    //foreach (var item in ExistingCrgSDLt)
                    //{
                    //    intCrgCount += item.Count;
                    //}

                    FinalOneCphCrg = u;
                    break;
                }


                foreach (var newcrg in u.AggregateAndUpdateQ(LSCrg, SSCrg, Q, strAreaAggregation, ExistingCrgSDLt, ExistingCphSDLt, ExistingCorrCphsSD, intSSTypeIndex, _adblTD, intFactor))
                {
                    //int intExploredRegionLast = CRegion._intStaticGID - CRegion._intStartStaticGIDLast;  //we don't need to +1 because +1 is already included in _intStaticGID

                    if (CRegion._intNodeCount > intQuitCount)
                    {
                        return new CRegion(-2);  //if we have visited 2000000 regions but haven't found an optimum aggregation sequence, then we return null and overestimate in the heuristic function 
                    }
                }
                u.cenumColor = CEnumColor.black;
            }

            RecordResultForCrg(StrObjLtSD, LSCrg, FinalOneCphCrg, intSSTypeIndex);
            return FinalOneCphCrg;
        }

        private void AddVirtualItem(List<List<object>> pobjDataLtLt, CRegion LSCrg, string strAreaAggregation, int intCount)
        {
            List<object> objDataLt = new List<object>(14);
            objDataLt.Add(LSCrg.ID);
            objDataLt.Add(LSCrg.GetCphCount());
            objDataLt.Add(LSCrg.GetAdjCount());
            objDataLt.Add(strAreaAggregation);
            for (int i = 4; i < intCount; i++)
            {
                objDataLt.Add(-1);
            }
            pobjDataLtLt.Add(objDataLt);
        }
        #endregion

        

        


    }
}
