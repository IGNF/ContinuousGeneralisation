using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Display;


namespace MorphingClass.CUtility
{
    public class CColor
    {
        public int intRed { get; set; }
        public int intGreen { get; set; }
        public int intBlue { get; set; }

        public CColor()
        {


        }

        public CColor(IRgbColor pRgbColor)
        {
            this.intRed = pRgbColor.Red;
            this.intGreen = pRgbColor.Green;
            this.intBlue = pRgbColor.Blue;
        }

        public CColor(int fintRed, int fintGreen,int fintBlue)
        {
            this.intRed = fintRed;
            this.intGreen = fintGreen;
            this.intBlue = fintBlue;
        }
    }
}
