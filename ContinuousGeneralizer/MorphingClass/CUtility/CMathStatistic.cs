using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorphingClass.CUtility
{
    public class CMathStatistic
    {
        //private static  List<double> _dblDataLt;
        //private static  CHelpFuncExcel _pHelperFunctionExcel=new CHelpFuncExcel ();


        //public static  CMathStatistic(List<double> pdblDataLt)
        //{
        //    _dblDataLt = pdblDataLt;

        //}

        /// <summary>
        /// use Box-Muller transform to randomly generate Gaussian distribution numbers 
        /// </summary>
        /// <remarks>"rand" generates random numbers according to the current time and the generated index, 
        /// which means that you will get the same random numbers if you always initialize a new "rand" and generate the first random number at the same time, because it takes no time to new "rand" and generate a number.
        /// Therefore, we have to get "rand" as a parameter here to avoid generating the same  random numbers</remarks>
        public static void BoxMuller(Random rand, out double dblX, out double dblY)
        {
            //Random rand = new Random(); //reuse this if you are generating many
            double u1 = rand.NextDouble(); //these are uniform(0,1) random doubles
            double u2 = rand.NextDouble();

            double dblpart1 = Math.Sqrt(-2.0 * Math.Log(u1));
            double dblpart2 = 2.0 * Math.PI * u2;
            dblX = dblpart1 * Math.Cos(dblpart2);
            dblY = dblpart1 * Math.Sin(dblpart2);
        }




        ///// <summary>
        ///// 计算平均值
        ///// </summary>
        ///// <param name="dblDatalt">数据列</param>
        ///// <returns>平均值</returns>
        //public static  double CalAverage(List<double> dblDataLt)
        //{
        //    double dblSum = 0;
        //    for (int i = 0; i < dblDataLt.Count; i++)
        //    {
        //        dblSum = dblSum + dblDataLt[i];
        //    }
        //    double dblAverage = dblSum / dblDataLt.Count;
        //    return dblAverage;
        //}


        /// <summary>
        /// 计算中值
        /// </summary>
        /// <param name="dblDatalt">数据列</param>
        /// <remarks>如果数的个数为偶数，则返回值为中间两个值的平均值</remarks>
        /// <returns>中值</returns>
        public static double CalMid(List<double> dblDataLt)
        {
            List<double> dbldatalt = new List<double>(dblDataLt.Count);
            dbldatalt.AddRange(dblDataLt);
            dbldatalt.Sort();

            int intRemain = dbldatalt.Count % 2;
            double dblMid = 0;
            if (intRemain == 0)
            {
                int intIndex = dbldatalt.Count / 2;
                dblMid = (dbldatalt[intIndex - 1] + dbldatalt[intIndex]) / 2;
            }
            else if (intRemain == 1)
            {
                int intIndex = dbldatalt.Count / 2;
                dblMid = dbldatalt[intIndex];
            }
            return dblMid;
        }

        ///// <summary>
        ///// 计算标准差（样本）
        ///// </summary>
        ///// <param name="dblDatalt">数据列</param>
        ///// <returns>标准差</returns>
        //public static  double CalStdev(List<double> dblDataLt)
        //{
        //    double dblStedv = CHelpFuncExcel.ECalStdev(dblDataLt);
        //    return dblStedv;
        //}

        ///// <summary>
        ///// 计算标准差（整体）
        ///// </summary>
        ///// <param name="dblDatalt">数据列</param>
        ///// <returns>标准差</returns>
        //public static  double CalStdevP(List<double> dblDataLt)
        //{
        //    double dblStedvP = CHelpFuncExcel.ECalStdevP(dblDataLt);
        //    return dblStedvP;
        //}
    }
}
