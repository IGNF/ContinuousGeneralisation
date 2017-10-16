using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorphingClass.CUtility
{
    public class CMath
    {
        public static double JudgeNegtiveReturn0(double dblValue, string str=null)
        {
            if (dblValue <0)
            {
                //Console.Write(str + ";  ");
                return 0;
            }
            else
            {
                return dblValue;
            }
        }

        public static int GetFirstDigit(double dblValue)
        {
            int intValue = Convert.ToInt32(Math.Truncate(Math.Abs(dblValue)));
            while (intValue >= 10)
            {
                intValue /= 10;
            }
            return intValue;
        }

        /// <summary>
        /// return the value consisting of first digit and zeros
        /// </summary>
        /// <param name="dblValue"></param>
        /// <returns></returns>
        public static int GetNumberTidy(double dblValue)
        {
            if (dblValue<1)
            {
                throw new ArgumentException("dblValue must be larger than or equal to 1");
            }

            //double dblAbs = Math.Abs(dblValue);
            double dblAbs = dblValue;
            int intCount = 0;
            while (dblAbs >= 10)
            {
                dblAbs /= 10;
                intCount++;
            }

            int intFirstDigit = Convert.ToInt32(Math.Floor(dblAbs));
            int intValue = intFirstDigit * Convert.ToInt32(Math.Pow(10, intCount));

            return intValue;
        }
    }
}
