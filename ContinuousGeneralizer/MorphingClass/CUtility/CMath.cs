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
    }
}
