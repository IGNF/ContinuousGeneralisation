﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MorphingClass.CUtility
{
    public static class CConstants
    {
        

        private static double _dblVerySmall = 0.0000000000000000001;  //please notice that the default value of _dblVerySmall is 0.000000000001, but we usually set a new _dblVerySmall with respect to the data
        private static double _dblVerySmallSquare = 0.0000000000000000001;  //please notice that the default value of _dblVerySmall is 0.000000000001, but we usually set a new _dblVerySmall with respect to the data
        private static double _dblVerySmallPower3 = 0.0000000000000000001;  //please notice that the default value of _dblVerySmall is 0.000000000001, but we usually set a new _dblVerySmall with respect to the data
        private static double _dblVerySmallPower4 = 0.0000000000000000001;  //please notice that the default value of _dblVerySmall is 0.000000000001, but we usually set a new _dblVerySmall with respect to the data
        
        private static double _dblVerySmallSpecial = 0.0000000000000000001;  //can be changed when we check topological relationship
        private static double _dblMidLength = 1;
        private static double _dblSmallDis = 0.1;
        private static double _dblVerySmallSlope = 0.00000001;  //we should consider slope somehow different

        //10 X e6 is not good for calculate integral
        //10 X e4 and 10 X e5 is not good for constructing compatible triangulations of data Jiangxionecounty, because there are some intersections too close each other and we may identify a InIn relationship to InFr relationship
        //10 X e6 is not good for constructing compatible triangulations of data MainlandChina, because there are some intersections too close each other and we may identify a InIn relationship to InFr relationship; 10 X e7 works well
        private static double _dblVerySmallDenominator = 1000000; //I test it many times. it seems 10 X e6 is appropriate value. This value may need change when we have different data sets
        private static double _dblSmallDisDenominator = 1000;
        private static double _dblSpecialValue = -Math.Pow(Math.PI, 10);  //we use the special value to initial some number
        private static double _dblTwoPI = 2 * Math.PI;
        private static double _dblTwoSqrtPI = 2 * Math .Sqrt ( Math.PI);
        private static double _dblHalfDoubleMax = double.MaxValue / 2;

        private static double _dblLowerBound = 0.98;
        private static double _dblUpperBound = 1 / _dblLowerBound;

        private static double _dblLowerBoundLoose = 0.9;
        private static double _dblUpperBoundLoose = 1 / _dblLowerBoundLoose;

        public static int _intMaxTypeChange = 6;

        private static string _strFmtIDs6="{0,3}{1,10}{2,13}{3,10}{4,9}{5,10}";
        private static Comparer<int> _ComparerInt = Comparer<int>.Default;
        private static Comparer<double> _ComparerDbl = Comparer<double>.Default;

        public static bool blnComputeMinComp { set; get; }
        public static bool blnComputeAvgComp { set; get; }

        public static string strMethod { set; get; }
        public static string strShapeConstraint { set; get; }
            //        var output = string.Format("{0,3}{1,10}{2,13}{3,10}{4,9}{5,10}{6,7}{7,22}{8,7}{9,22}",
            //    "ID:", this.ID, ";    indexID:", this.indexID , ";    GID:" , this.GID , ";    X:", this.X , ";    Y:", this.Y);
            //Console.WriteLine(output);
        //private static SortedSet

        //private static int _intCptGID = 0;
        //private static int _intCplGID = 0;
        //private static int _intCpgGID = 0;

        public static Comparer<int> ComparerInt
        {
            get { return _ComparerInt; }
        }

        public static Comparer<double> ComparerDbl
        {
            get { return _ComparerDbl; }
        }

        public static int intMaxTypeChange
        {
            get { return _intMaxTypeChange; }
            set { _intMaxTypeChange = value; }
        }


        public static string strFmtIDs6
        {
            get { return _strFmtIDs6; }
            set { _strFmtIDs6 = value; }
        }

        /// <summary>属性：</summary>
        public static double dblVerySmall
        {
            get { return _dblVerySmall; }
            set
            {
                double X = _dblMidLength;
                double y = value;

                double XX = X * X;
                double XXX = XX * X;
                double XXXX = XX * XX;
                double yy = y * y;
                double yyy = yy * y;
                double yyyy = yy * yy;

                _dblVerySmall = y;
                _dblVerySmallSquare = 2 * X * y + yy;
                _dblVerySmallPower3 = 3 * XX * y + 3 * X * yy + yyy;
                _dblVerySmallPower4 = 4 * XXX * y + 6 * XX * yy + 4 * X * yyy + yyyy;
            }
        }

        /// <summary>属性：</summary>
        public static double dblVerySmallSquare
        {
            get { return _dblVerySmallSquare; }
            set { _dblVerySmallSquare = value; }
        }

        public static double dblVerySmallPower3
        {
            get { return _dblVerySmallPower3; }
            set { _dblVerySmallPower3 = value; }
        }
        
        public static double dblVerySmallPower4
        {
            get { return _dblVerySmallPower4; }
            set { _dblVerySmallPower4 = value; }
        }

        /// <summary>属性：</summary>
        public static double dblVerySmallSpecial
        {
            get { return _dblVerySmallSpecial; }
            set { _dblVerySmallSpecial = value; }
        }

        public static double dblVerySmallDenominator
        {
            get { return _dblVerySmallDenominator; }
            set { _dblVerySmallDenominator = value; }
        }

        public static double dblSmallDisDenominator
        {
            get { return _dblSmallDisDenominator; }
            set { _dblSmallDisDenominator = value; }
        }

        /// <summary>属性：</summary>
        public static double dblMidLength
        {
            get { return _dblMidLength; }
            set { _dblMidLength = value; }
        }

        /// <summary>属性：</summary>
        public static double dblSmallDis
        {
            get { return _dblSmallDis; }
            set { _dblSmallDis = value; }
        }

        public static double dblTwoSqrtPI
        {
            get { return _dblTwoSqrtPI; }
            set { _dblTwoSqrtPI = value; }
        }

        
        public static double dblTwoPI
        {
            get { return _dblTwoPI; }
            set { _dblTwoPI = value; }
        }

        public static double dblSpecialValue
        {
            get { return _dblSpecialValue; }
            set { _dblSpecialValue = value; }
        }

        public static double dblHalfDoubleMax
        {
            get { return _dblHalfDoubleMax; }
            set { _dblHalfDoubleMax = value; }
        }

        public static double dblLowerBound
        {
            get { return _dblLowerBound; }
            set { _dblLowerBound = value; }
        }

        public static double dblUpperBound
        {
            get { return _dblUpperBound; }
            set { _dblUpperBound = value; }
        }

        public static double dblLowerBoundLoose
        {
            get { return _dblLowerBoundLoose; }
            set { _dblLowerBoundLoose = value; }
        }

        public static double dblUpperBoundLoose
        {
            get { return _dblUpperBoundLoose; }
            set { _dblUpperBoundLoose = value; }
        }

        public static double dblVerySmallSlope
        {
            get { return _dblVerySmallSlope; }
            set { _dblVerySmallSlope = value; }
        }

        

        //private static int _intCptGID = 0;
        //private static int _intCplGID = 0;
        //private static int _intCpgGID = 0;
    }
}