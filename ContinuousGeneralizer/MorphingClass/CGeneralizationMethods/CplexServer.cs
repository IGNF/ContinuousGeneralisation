using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

using ILOG.Concert;
using ILOG.CPLEX;

namespace MorphingClass.CGeneralizationMethods
{
    public class CplexServer
    {
        // define class to transfer model to server
        [System.Serializable()]
        internal class ModelData
        {
            internal IModel model;
            internal INumVar[] vars;
            internal ModelData(IModel m, INumVar[] v)
            {
                model = m;
                vars = v;
            }
        }

        // define class to transfer back solution
        [System.Serializable()]
        internal class SolutionData
        {
            internal Cplex.CplexStatus status;
            internal double obj;
            internal double[] vals;
        }

        public static void Main(string[] args)
        {
            try
            {
                // setup files to transfer model to server
                string mfile = "Model.dat";
                string sfile = "Solution.dat";


                // build model
                INumVar[][] var = new INumVar[1][];
                IRange[][] rng = new IRange[1][];

                CplexModeler model = new CplexModeler();
                PopulateByRow(model, var, rng);

                FileStream mstream = new FileStream(mfile, FileMode.Create);
                BinaryFormatter formatter = new BinaryFormatter();
                formatter.Serialize(mstream, new ModelData(model, var[0]));
                mstream.Close();

                // start server
                Server server = new Server(mfile, sfile);

                SolutionData sol = null;
                FileStream sstream = new FileStream(sfile, FileMode.Open);
                sol = (SolutionData)formatter.Deserialize(sstream);
                sstream.Close();

                System.Console.WriteLine("Solution status = " + sol.status);

                if (sol.status.Equals(Cplex.CplexStatus.Optimal))
                {
                    System.Console.WriteLine("Solution value = " + sol.obj);
                    int ncols = var[0].Length;
                    for (int j = 0; j < ncols; ++j)
                        System.Console.WriteLine("Variable " + j + ": Value = " + sol.vals[j]);
                }

            }
            catch (ILOG.Concert.Exception e)
            {
                System.Console.WriteLine("Concert exception '" + e + "' caught");
            }
            catch (System.Exception t)
            {
                System.Console.WriteLine("terminating due to exception " + t);
            }
        }


        // The following method populates the problem with data for the
        // following linear program:
        //
        //    Maximize
        //     x1 + 2 x2 + 3 x3
        //    Subject To
        //     - x1 + x2 + x3 <= 20
        //     x1 - 3 x2 + x3 <= 30
        //    Bounds
        //     0 <= x1 <= 40
        //    End
        //
        // using the IModeler API

        internal static void PopulateByRow(IModeler model,
                                  INumVar[][] var,
                                  IRange[][] rng)
        {
            double[] lb = { 0.0, 0.0, 0.0 };
            double[] ub = { 40.0, System.Double.MaxValue, System.Double.MaxValue };
            string[] varname = { "x1", "x2", "x3" };
            INumVar[] x = model.NumVarArray(3, lb, ub, varname);
            var[0] = x;

            double[] objvals = { 1.0, 2.0, 3.0 };
            model.AddMaximize(model.ScalProd(x, objvals));

            rng[0] = new IRange[2];
            rng[0][0] = model.AddLe(model.Sum(model.Prod(-1.0, x[0]),
                                              model.Prod(1.0, x[1]),
                                              model.Prod(1.0, x[2])), 20.0, "c1");
            rng[0][1] = model.AddLe(model.Sum(model.Prod(1.0, x[0]),
                                              model.Prod(-3.0, x[1]),
                                              model.Prod(1.0, x[2])), 30.0, "c2");
        }


        // The server class
        internal class Server
        {
            internal string sfile;
            internal string mfile;

            internal Server(string modelfile, string solutionfile)
            {
                mfile = modelfile;
                sfile = solutionfile;
                try
                {
                    FileStream mstream = new FileStream(mfile, FileMode.Open);
                    FileStream sstream = new FileStream(sfile, FileMode.Create);

                    Cplex cplex = new Cplex();
                    ModelData data = null;

                    BinaryFormatter formatter = new BinaryFormatter();
                    data = (ModelData)formatter.Deserialize(mstream);
                    mstream.Close();


                    cplex.SetModel(data.model);

                    SolutionData sol = new SolutionData();
                    if (cplex.Solve())
                    {
                        sol.obj = cplex.ObjValue;
                        sol.vals = cplex.GetValues(data.vars);
                    }
                    sol.status = cplex.GetCplexStatus();

                    formatter.Serialize(sstream, sol);
                    sstream.Close();

                    cplex.End();
                }
                catch (System.Exception t)
                {
                    System.Console.WriteLine("server terminates due to " + t);
                }
            }
        }
    }
}
