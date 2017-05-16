using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MorphingClass.CGeometry;

namespace MorphingClass.CUtility
{
    public class CEnvelope
    {
        private double _dblXMin;
        private double _dblYMin;
        private double _dblXMax;
        private double _dblYMax;
        private double _dblWidth;
        private double _dblHeigth;



        public CPoint LowerLeftCpt { set; get; }
        public CPoint LowerRightCpt { set; get; }
        public CPoint UpperRightCpt { set; get; }
        public CPoint UpperLeftCpt { set; get; }

        public CEdge LeftCEdge { set; get; }
        public CEdge LowerCEdge { set; get; }
        public CEdge RightCEdge { set; get; }
        public CEdge UpperCEdge { set; get; }

        public CEnvelope(double fdblXMin, double fdblYMin, double fdblXMax, double fdblYMax)
        {
            _dblXMin = fdblXMin;
            _dblYMin = fdblYMin;
            _dblXMax = fdblXMax;
            _dblYMax = fdblYMax;

            _dblWidth = fdblXMax - fdblXMin;
            _dblHeigth = fdblYMax - fdblYMin;
        }

        public CEnvelope GetDilatedCEnv(double dblD)
        {
            return new CUtility.CEnvelope
                (_dblXMin - dblD, _dblYMin - dblD, _dblXMax + dblD, _dblYMax + dblD);
        }

        public void SetCornerCpts()
        {
            this.LowerLeftCpt = new CGeometry.CPoint(0, _dblXMin, _dblYMin);
            this.LowerRightCpt = new CGeometry.CPoint(1, _dblXMax, _dblYMin);
            this.UpperRightCpt = new CGeometry.CPoint(2, _dblXMax, _dblYMax);
            this.UpperLeftCpt = new CGeometry.CPoint(3, _dblXMin, _dblYMax);
        }

        public void SetEdges()
        {
            SetCornerCpts();

            this.LeftCEdge = new CEdge(this.UpperLeftCpt, this.LowerLeftCpt);
            this.LowerCEdge = new CEdge(this.LowerLeftCpt, this.LowerRightCpt);
            this.RightCEdge = new CEdge(this.LowerRightCpt, this.UpperRightCpt);
            this.UpperCEdge = new CEdge(this.UpperRightCpt, this.UpperLeftCpt);
        }

        //public int CompareTo(CEnvelope other)
        //{
        //    CCmpMethods.CmpDual(this, other, cenv => cenv.XMin, cenv => cenv.YMin);

        //    //return this.GID.CompareTo(other.GID);
        //}

        public double XMin
        {
            get { return _dblXMin; }
            set { _dblXMin = value; }
        }

        public double YMin
        {
            get { return _dblYMin; }
            set { _dblYMin = value; }
        }

        public double XMax
        {
            get { return _dblXMax; }
            set { _dblXMax = value; }
        }

        public double YMax
        {
            get { return _dblYMax; }
            set { _dblYMax = value; }
        }


        public double Width
        {
            get { return _dblWidth; }
            //set { _dblWidth = value; }
        }

        public double Height
        {
            get { return _dblHeigth; }
            //set { _dblHeigth = value; }
        }

    }
}
