using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

        public CEnvelope(double fdblXMin, double fdblYMin, double fdblXMax, double fdblYMax)
        {
            _dblXMin = fdblXMin;
            _dblYMin = fdblYMin;
            _dblXMax = fdblXMax;
            _dblYMax = fdblYMax;

            _dblWidth = fdblXMax - fdblXMin;
            _dblHeigth = fdblYMax - fdblYMin;
        }

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
            set { _dblWidth = value; }
        }

        public double Height
        {
            get { return _dblHeigth; }
            set { _dblHeigth = value; }
        }

    }
}
